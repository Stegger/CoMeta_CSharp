using System;
using System.Collections.Generic;
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

        public DbSet<Message> Messages { get; set; }
    }
}