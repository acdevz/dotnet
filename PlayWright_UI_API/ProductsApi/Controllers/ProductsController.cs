using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductsApi.Repository;

namespace ProductApi.Controllers;

[Route("api/products")]
[ApiController]
[Authorize]
public class ProductsController : ControllerBase
{
    private ProductRepository _productRepository;
    private UserRepository _userRepository;

    public ProductsController(ProductRepository productRepository, UserRepository userRepository)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
    }

    [HttpGet]
    public IActionResult GetProducts()
    {
        var username = User.Identity?.Name;
        var user = _userRepository.GetByUsername(username!);

        var products = _productRepository.GetByOwnerId(user!.Id);
        return Ok(products);
    }
}

