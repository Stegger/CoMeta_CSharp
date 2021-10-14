using System;
using System.Collections.Generic;
using System.Linq;
using CoMeta.Models;
using MySecurity.Authentication;

namespace CoMeta.Data
{
    public class InMemoryInitializer : IDbInitializer
    {
        public void Initialize(CoMetaContext context)
        {
            // Delete the database, if it already exists. You need to clean and build
            // the solution for this to take effect.
            context.Database.EnsureDeleted();

            // Create the database, if it does not already exists. If the database
            // already exists, no action is taken (and no effort is made to ensure it
            // is compatible with the model for this context).
            context.Database.EnsureCreated();

            context.Messages.Add(new Message()
            {
                ReceiverId = 1,
                ReceiverName = "admin",
                SenderId = 1,
                SendeName = "admin",
                Text = "This is a message from the initializer"
            });

            context.SaveChanges();

        }
    }
}