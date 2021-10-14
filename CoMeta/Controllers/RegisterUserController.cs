using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using CoMeta.Data;
using CoMeta.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySecurity.Authentication;
using User = MySecurity.Entities.User;
using UserRepository = MySecurity.Data.UserRepository;

namespace CoMeta.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterUserController : ControllerBase
    {
        private readonly IUserAuthenticator _userAuthenticator;
        private readonly ILogger<RegisterUserController> _logger;

        public RegisterUserController(IUserAuthenticator userAuthenticator, ILogger<RegisterUserController> logger)
        {
            _userAuthenticator = userAuthenticator;
            _logger = logger;
        }

        // POST: api/Login
        [HttpPost]
        public IActionResult Post([FromBody] RegisterUserModel model)
        {
            string username = model.Username;
            string password = model.Password;

            if (_userAuthenticator.CreateUser(username, password))
            {
                //Authentication succesful
                return Ok();
            }
            else
            {
                return Problem("Could not create user with name: " + username);
            }
        }
    }
}