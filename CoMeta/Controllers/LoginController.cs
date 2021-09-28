using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoMeta.Data;
using CoMeta.Helpers;
using CoMeta.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoMeta.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IRepository<User> _userRepository;
        private IAuthenticationHelper _authHelper;

        public LoginController(IRepository<User> db, IAuthenticationHelper authHelper)
        {
            _userRepository = db;
            _authHelper = authHelper;
        }

        // POST: api/Login
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Post([FromBody] LoginInput model)
        {
            User user = _userRepository.GetAll().FirstOrDefault(user => user.Username.Equals(model.Username));

            //Did we find a user with the given username?
            if (user == null)
                return Unauthorized();

            //Was the correct password given?
            if (!_authHelper.VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized();
            
            //Authentication succesful
            return Ok(new
            {
                username = user.Username,
                token = _authHelper.GenerateToken(user)
            });
            
        }
        
    }
}
