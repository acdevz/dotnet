using System.Globalization;
using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using OrderService.Protos;
using OrderApp.Models.Orders;

namespace OrderApp.Controllers;

[Route("orders")]
public class OrderController : Controller
{
    private readonly Order.OrderClient _client;
    private readonly ILogger<OrderController> _logger;

    public OrderController(
        Order.OrderClient client,
        ILogger<OrderController> logger)
    {
        _client = client;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var response = await _client.GetOrdersAsync(
            new Empty(),
            cancellationToken: cancellationToken);

        var model = response.Orders
            .Select(x => new OrderListItemViewModel
            {
                OrderId = x.OrderId,
                CustomerName = x.CustomerName,
                OrderDate = ParseTimestamp(x.OrderDate)
            })
            .ToList();

        return View(model);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View(new CreateOrderViewModel());
    }

    [ValidateAntiForgeryToken]
    [HttpPost("create")]
    public async Task<IActionResult> Create(
        CreateOrderViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var order = await _client.CreateOrderAsync(
                new CreateOrderRequest
                {
                    CustomerName = model.CustomerName.Trim()
                },
                cancellationToken: cancellationToken);

            TempData["SuccessMessage"] =
                $"Order {order.OrderId} was created.";

            return RedirectToAction(
                nameof(Track),
                new { id = order.OrderId });
        }
        catch (RpcException exception)
        {
            ModelState.AddModelError(
                string.Empty,
                exception.Status.Detail);

            return View(model);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Track(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await _client.GetOrderAsync(
                new GetOrderRequest
                {
                    OrderId = id
                },
                cancellationToken: cancellationToken);

            var model = new OrderTrackViewModel
            {
                OrderId = order.OrderId,
                CustomerName = order.CustomerName,
                OrderDate = ParseTimestamp(order.OrderDate)
            };

            return View(model);
        }
        catch (RpcException exception) when (exception.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return NotFound(exception.Status.Detail);
        }
    }

    [ValidateAntiForgeryToken]
    [HttpPost("{id:int}/statuses")]
    public async Task<IActionResult> AddStatus(
        int id,
        [FromForm] AddStatusViewModel model,
        CancellationToken cancellationToken)
    {
        if (id != model.OrderId)
        {
            return BadRequest(new
            {
                message = "The order ID does not match."
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = FirstValidationError()
            });
        }

        try
        {
            var response = await _client.AddStatusAsync(
                new AddStatusRequest
                {
                    OrderId = id,
                    Status = model.Status.Trim()
                },
                cancellationToken: cancellationToken);

            return Ok(new
            {
                message = "Status added.",
                statusId = response.StatusId
            });
        }
        catch (RpcException exception)
        {
            return StatusCode(
                MapStatusCode(exception.StatusCode),
                new
                {
                    message = exception.Status.Detail
                });
        }
    }

    [ValidateAntiForgeryToken]
    [HttpPost("{id:int}/notes")]
    public async Task<IActionResult> AddNote(
        int id,
        [FromForm] AddNoteViewModel model,
        CancellationToken cancellationToken)
    {
        if (id != model.OrderId)
        {
            return BadRequest(new
            {
                message = "The order ID does not match."
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = FirstValidationError()
            });
        }

        try
        {
            var response = await _client.SendNoteAsync(
                new OrderNoteRequest
                {
                    OrderId = id,
                    Note = model.Note.Trim()
                },
                cancellationToken: cancellationToken);

            return Ok(new
            {
                message = "Note added.",
                statusId = response.StatusId
            });
        }
        catch (RpcException exception)
        {
            return StatusCode(
                MapStatusCode(exception.StatusCode),
                new
                {
                    message = exception.Status.Detail
                });
        }
    }

    [HttpGet("{id:int}/stream")]
    public async Task Stream(
        int id,
        CancellationToken cancellationToken)
    {
        Response.StatusCode = StatusCodes.Status200OK;
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";
        Response.Headers["X-Accel-Buffering"] = "no";

        try
        {
            using var call = _client.TrackOrder(
                new TrackOrderRequest
                {
                    OrderId = id
                },
                cancellationToken: cancellationToken);

            await foreach (var status in call.ResponseStream.ReadAllAsync(cancellationToken))
            {
                var payload = new
                {
                    statusId = status.StatusId,
                    orderId = status.OrderId,
                    status = status.Status,
                    timestamp = status.Timestamp,
                    isCustomerNote = status.IsCustomerNote
                };

                var json = JsonSerializer.Serialize(payload);

                await Response.WriteAsync(
                    $"id: {status.StatusId}\n",
                    cancellationToken);

                await Response.WriteAsync(
                    "event: order-status\n",
                    cancellationToken);

                await Response.WriteAsync(
                    $"data: {json}\n\n",
                    cancellationToken);

                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Browser disconnected from order {OrderId}.",
                id);
        }
        catch (RpcException exception) when (exception.StatusCode == Grpc.Core.StatusCode.Cancelled)
        {
            _logger.LogInformation(
                "gRPC tracking call cancelled for order {OrderId}.",
                id);
        }
        catch (RpcException exception)
        {
            _logger.LogWarning(
                exception,
                "gRPC stream failed for order {OrderId}.",
                id);

            if (!Response.HasStarted)
            {
                Response.StatusCode = StatusCodes.Status502BadGateway;
            }
        }
    }

    private string FirstValidationError()
    {
        return ModelState.Values
            .SelectMany(x => x.Errors)
            .Select(x => x.ErrorMessage)
            .FirstOrDefault()
            ?? "The submitted data is invalid.";
    }

    private static DateTimeOffset ParseTimestamp(string timestamp)
    {
        return DateTimeOffset.Parse(
            timestamp,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind);
    }

    private static int MapStatusCode(StatusCode statusCode)
    {
        return statusCode switch
        {
            Grpc.Core.StatusCode.InvalidArgument =>
                StatusCodes.Status400BadRequest,

            Grpc.Core.StatusCode.NotFound =>
                StatusCodes.Status404NotFound,

            Grpc.Core.StatusCode.Unavailable =>
                StatusCodes.Status503ServiceUnavailable,

            _ =>
                StatusCodes.Status502BadGateway
        };
    }
}
