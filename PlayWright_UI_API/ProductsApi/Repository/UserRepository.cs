using Microsoft.AspNetCore.Identity;
using ProductsApi.Models;

namespace ProductsApi.Repository;

public class UserRepository
{
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ProductRepository _productRepository;

    public UserRepository(ProductRepository productRepository, IPasswordHasher<User> passwordHasher)
    {
        _productRepository = productRepository;
        _passwordHasher = passwordHasher;

        _users.ForEach(user =>
        {
            user.Password = _passwordHasher.HashPassword(user, user.Password);
        });
    }
    private readonly List<User> _users =
    [
        new User
        {
            Id = 1,
            Username = "acdevs",
            Password = "4444",
            FullName = "Aman Chandra",
            Email = "aman@gmail.com",
            City = "Bengaluru"
        },
        new User
        {
            Id = 2,
            Username = "kabdus",
            Password = "8888",
            FullName = "Abdus Samad Khan",
            Email = "abdus@gmail.com",
            City = "Lucknow"
        },
    ];

    public IEnumerable<User> GetAll()
    {
        foreach (var user in _users)
        {
            user.Products = _productRepository
                .GetByOwnerId(user.Id)
                .ToList();
        }

        return _users;
    }

    public User? GetById(int id)
    {
        var user = _users.FirstOrDefault(user => user.Id == id);

        if (user is null)
        {
            return null;
        }

        user.Products = _productRepository
            .GetByOwnerId(user.Id)
            .ToList();

        return user;
    }

    public User? GetByUsername(string username)
    {
        var user = _users.FirstOrDefault(user =>
            user.Username.Equals(
                username,
                StringComparison.OrdinalIgnoreCase));

        if (user is null)
        {
            return null;
        }

        user.Products = _productRepository
            .GetByOwnerId(user.Id)
            .ToList();

        return user;
    }

    public User Create(User user)
    {
        user.Id = _users.Count == 0
            ? 1
            : _users.Max(existingUser => existingUser.Id) + 1;

        user.Products = [];

        _users.Add(user);

        return user;
    }

    public User? Update(int id, User updatedUser)
    {
        var existingUser = _users.FirstOrDefault(user => user.Id == id);

        if (existingUser is null)
        {
            return null;
        }

        existingUser.Username = updatedUser.Username;
        existingUser.FullName = updatedUser.FullName;
        existingUser.Email = updatedUser.Email;
        existingUser.City = updatedUser.City;
        existingUser.Password = updatedUser.Password;

        existingUser.Products = _productRepository
            .GetByOwnerId(existingUser.Id)
            .ToList();

        return existingUser;
    }
}