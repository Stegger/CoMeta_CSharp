using System;
using System.Collections.Generic;
using System.Linq;
using CoMeta.Helpers;
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
            //todo Implement this
            
        }
        
        
        
    }
}