using System;
using System.Collections.Generic;
using System.Linq;
using CoMeta.Helpers;
using CoMeta.Models;

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

            context.Roles.Add(new Role() { Name = "User" });
            context.Roles.Add(new Role() { Name = "Administrator" });
            context.SaveChanges();
            var admin = context.Roles.FirstOrDefault(role => role.Name.Equals("Administrator"));

            var authenticationHelper = new AuthenticationHelper(Array.Empty<byte>());
            var password = "password123"; //Why not?

            //Since I'm seeding data I create a password hash + salt manually:
            authenticationHelper.CreatePasswordHash(password, out var pass, out var salt);
            context.Users.Add(new User()
            {
                Role = "Administrator",
                Username = "admin",
                PasswordHash = pass,
                PasswordSalt = salt
            });
            context.SaveChanges();
        }
    }
}