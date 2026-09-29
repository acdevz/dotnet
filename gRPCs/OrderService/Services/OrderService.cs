using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Entities;
using OrderService.Protos;

namespace OrderService.Services;

public class OrderService : Order.OrderBase
{
    private readonly OrdersDbContext _db;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        OrdersDbContext db,
        ILogger<OrderService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public override async Task<OrderReply> CreateOrder(
        CreateOrderRequest request,
        ServerCallContext context)
    {
        var customerName = request.CustomerName.Trim();

        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    "Customer name is required."));
        }

        if (customerName.Length > 100)
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    "Customer name cannot exceed 100 characters."));
        }

        var order = new OrderEntity
        {
            CustomerName = customerName,
            OrderDate = DateTime.UtcNow
        };

        order.Statuses.Add(new OrderStatusEntity
        {
            Status = "Order created",
            Timestamp = DateTime.UtcNow,
            IsCustomerNote = false
        });

        _db.Orders.Add(order);

        await _db.SaveChangesAsync(context.CancellationToken);

        return MapOrder(order);
    }

    public override async Task<OrderReply> GetOrder(
        GetOrderRequest request,
        ServerCallContext context)
    {
        var order = await _db.Orders
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.OrderId == request.OrderId,
                context.CancellationToken);

        if (order is null)
        {
            throw OrderNotFound(request.OrderId);
        }

        return MapOrder(order);
    }

    public override async Task<GetOrdersReply> GetOrders(
        Empty request,
        ServerCallContext context)
    {
        var orders = await _db.Orders
            .AsNoTracking()
            .OrderByDescending(x => x.OrderDate)
            .ToListAsync(context.CancellationToken);

        var reply = new GetOrdersReply();

        reply.Orders.AddRange(orders.Select(MapOrder));

        return reply;
    }

    public override async Task<OrderStatusReply> AddStatus(
        AddStatusRequest request,
        ServerCallContext context)
    {
        var statusText = request.Status.Trim();

        if (string.IsNullOrWhiteSpace(statusText))
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    "A status value is required."));
        }

        if (statusText.Length > 200)
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    "Status cannot exceed 200 characters."));
        }

        await EnsureOrderExistsAsync(
            request.OrderId,
            context.CancellationToken);

        var status = new OrderStatusEntity
        {
            OrderId = request.OrderId,
            Status = statusText,
            Timestamp = DateTime.UtcNow,
            IsCustomerNote = false
        };

        _db.OrderStatuses.Add(status);

        await _db.SaveChangesAsync(context.CancellationToken);

        return MapStatus(status);
    }

    public override async Task<OrderStatusReply> SendNote(
        OrderNoteRequest request,
        ServerCallContext context)
    {
        var note = request.Note.Trim();

        if (string.IsNullOrWhiteSpace(note))
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    "A note is required."));
        }

        if (note.Length > 200)
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    "Note cannot exceed 200 characters."));
        }

        await EnsureOrderExistsAsync(
            request.OrderId,
            context.CancellationToken);

        var status = new OrderStatusEntity
        {
            OrderId = request.OrderId,
            Status = note,
            Timestamp = DateTime.UtcNow,
            IsCustomerNote = true
        };

        _db.OrderStatuses.Add(status);

        await _db.SaveChangesAsync(context.CancellationToken);

        return MapStatus(status);
    }

    public override async Task TrackOrder(
        TrackOrderRequest request,
        IServerStreamWriter<OrderStatusReply> responseStream,
        ServerCallContext context)
    {
        await EnsureOrderExistsAsync(
            request.OrderId,
            context.CancellationToken);

        var lastStatusId = 0;

        try
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                // Create a fresh query on each iteration.
                // AsNoTracking prevents the DbContext change tracker
                // from accumulating streamed status entities.
                var statuses = await _db.OrderStatuses
                    .AsNoTracking()
                    .Where(x =>
                        x.OrderId == request.OrderId &&
                        x.StatusId > lastStatusId)
                    .OrderBy(x => x.StatusId)
                    .ToListAsync(context.CancellationToken);

                foreach (var status in statuses)
                {
                    await responseStream.WriteAsync(
                        MapStatus(status),
                        context.CancellationToken);

                    lastStatusId = status.StatusId;
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(1),
                    context.CancellationToken);
            }
        }
        catch (OperationCanceledException)
            when (context.CancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Tracking stream disconnected for order {OrderId}.",
                request.OrderId);
        }
    }

    private async Task EnsureOrderExistsAsync(
        int orderId,
        CancellationToken cancellationToken)
    {
        var exists = await _db.Orders
            .AsNoTracking()
            .AnyAsync(
                x => x.OrderId == orderId,
                cancellationToken);

        if (!exists)
        {
            throw OrderNotFound(orderId);
        }
    }

    private static RpcException OrderNotFound(int orderId)
    {
        return new RpcException(
            new Status(
                StatusCode.NotFound,
                $"Order {orderId} was not found."));
    }

    private static OrderReply MapOrder(OrderEntity order)
    {
        return new OrderReply
        {
            OrderId = order.OrderId,
            CustomerName = order.CustomerName,
            OrderDate = order.OrderDate.ToString("O")
        };
    }

    private static OrderStatusReply MapStatus(
        OrderStatusEntity status)
    {
        return new OrderStatusReply
        {
            StatusId = status.StatusId,
            OrderId = status.OrderId,
            Status = status.Status,
            Timestamp = status.Timestamp.ToString("O"),
            IsCustomerNote = status.IsCustomerNote
        };
    }
}
