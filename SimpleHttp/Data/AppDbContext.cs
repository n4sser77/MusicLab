using Microsoft.EntityFrameworkCore;
using SimpleHttp.Services;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }  // Must be a property
}
