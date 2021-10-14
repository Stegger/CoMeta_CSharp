using Microsoft.EntityFrameworkCore;
using MySecurity.Entities;

namespace MySecurity.Data
{
    public class SecurityContext : DbContext
    {

        public SecurityContext(DbContextOptions contextOptions) : base(contextOptions)
        {
        }

        public DbSet<User> Users { get; set; }
        

    }
}