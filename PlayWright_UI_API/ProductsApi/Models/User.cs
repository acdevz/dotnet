namespace ProductsApi.Models;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    // One User owns many Products
    public List<Product> Products { get; set; } = [];
}