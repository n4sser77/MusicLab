using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SimpleHttp.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=MyCloudServiceDB;Trusted_Connection=True;TrustServerCertificate=True;Integrated Security=True");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
