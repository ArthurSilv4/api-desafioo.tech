using Microsoft.EntityFrameworkCore;
using api_desafioo.tech.Models;

namespace api_desafioo.tech.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }

}
