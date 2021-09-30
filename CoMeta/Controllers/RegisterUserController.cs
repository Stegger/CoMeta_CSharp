using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoMeta.Data;
using CoMeta.Helpers;
using CoMeta.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoMeta.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterUserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Role> _roleRepo;
        private readonly IAuthenticationHelper _authentication;

        public RegisterUserController(IRepository<User> userRepo, IRepository<Role> roleRepo, IAuthenticationHelper authHelper,
            ILogger<UserController> logger)
        {
            _logger = logger;
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _authentication = authHelper;
        }

        // POST: api/Login
        [HttpPost]
        public IActionResult Post([FromBody] RegisterUserModel model)
        {
            var user = _userRepo.GetAll().FirstOrDefault(u => u.Username == model.Username);

            //Does already contain a user with the given username?
            if (user != null)
                return Unauthorized();

            byte[] salt;
            byte[] passwordHash;
            _authentication.CreatePasswordHash(model.Password, out passwordHash, out salt);

            //TODO Fix this by adding a RoleRepository and fetching the Role from there.. Putting in a relationship.
            List<Role> roles = new List<Role>();
            
            //I add the User role as a default for any new user:
            roles.Add(_roleRepo.GetAll().FirstOrDefault(r => r.Name.Equals("User")));

            user = new User()
            {
                Username = model.Username,
                Roles = roles,
                PasswordHash = passwordHash,
                PasswordSalt = salt
            };

            _userRepo.Add(user);
            //I get a fresh object from the db (With an ID):
            user = _userRepo.GetAll().FirstOrDefault(u => u.Username == model.Username);

            //Authentication succesful
            return Ok(new
            {
                username = user.Username,
                token = _authentication.GenerateToken(user)
            });
        }
    }
}