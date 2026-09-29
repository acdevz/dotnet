using ProductsApi.Models;

namespace ProductsApi.Repository;

public class ProductRepository
{
    private readonly List<Product> _products =
    [
        new Product
        {
            Id = 1,
            Name = "Tata Nexon Die-Cast Model",
            Description = "Collectible miniature model of Tata Nexon.",
            Price = 1299.00m,
            Category = "Collectibles",
            IsAvailable = true,
            OwnerId = 1
        },
        new Product
        {
            Id = 2,
            Name = "Mysore Sandal Soap Gift Pack",
            Description = "Traditional sandalwood soap gift collection.",
            Price = 450.00m,
            Category = "Personal Care",
            IsAvailable = true,
            OwnerId = 1
        },
        new Product
        {
            Id = 3,
            Name = "Banarasi Silk Saree",
            Description = "Handwoven silk saree sourced from Varanasi.",
            Price = 7499.00m,
            Category = "Fashion",
            IsAvailable = true,
            OwnerId = 2
        },
        new Product
        {
            Id = 4,
            Name = "Jaipur Blue Pottery Vase",
            Description = "Decorative handcrafted pottery from Jaipur.",
            Price = 1899.00m,
            Category = "Home Decor",
            IsAvailable = true,
            OwnerId = 2
        },
        new Product
        {
            Id = 5,
            Name = "Assam Tea Collection",
            Description = "Premium loose-leaf tea sourced from Assam.",
            Price = 799.00m,
            Category = "Groceries",
            IsAvailable = true,
            OwnerId = 2
        }
    ];

    public IEnumerable<Product> GetAll()
    {
        return _products;
    }

    public Product? GetById(int id)
    {
        return _products.FirstOrDefault(product => product.Id == id);
    }

    public IEnumerable<Product> GetByOwnerId(int ownerId)
    {
        return _products.Where(product => product.OwnerId == ownerId);
    }

    public Product Create(Product product)
    {
        product.Id = _products.Count == 0
            ? 1
            : _products.Max(existingProduct => existingProduct.Id) + 1;

        _products.Add(product);

        return product;
    }

    public Product? Update(int id, Product updatedProduct)
    {
        var existingProduct = GetById(id);

        if (existingProduct is null)
        {
            return null;
        }

        existingProduct.Name = updatedProduct.Name;
        existingProduct.Description = updatedProduct.Description;
        existingProduct.Price = updatedProduct.Price;
        existingProduct.Category = updatedProduct.Category;
        existingProduct.IsAvailable = updatedProduct.IsAvailable;
        existingProduct.OwnerId = updatedProduct.OwnerId;

        return existingProduct;
    }
}