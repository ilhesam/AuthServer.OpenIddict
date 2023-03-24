using Microsoft.EntityFrameworkCore;

namespace AuthServer.OpenIddict;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
}
