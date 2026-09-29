using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

public class UserServiceDbContext(DbContextOptions<UserServiceDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

}
