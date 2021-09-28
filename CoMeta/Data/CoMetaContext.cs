using System;
using System.Linq;
using CoMeta.Models;
using Microsoft.EntityFrameworkCore;

namespace CoMeta.Data
{
    public class CoMetaContext : DbContext
    {
        public CoMetaContext(DbContextOptions<CoMetaContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            long idx = 1;
            foreach (RoleTypes roleTypes in Enum.GetValues(typeof(RoleTypes)).Cast<RoleTypes>())
            {
                Role role = new Role()
                {
                    Id = idx,
                    Name = roleTypes.ToString()
                };
                modelBuilder.Entity<Role>().HasData(role);
                idx++;
            }
            //base.OnModelCreating(modelBuilder);
        }
        
    }
}