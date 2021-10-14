using CoMeta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySecurity.Authentication;

namespace CoMeta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserAuthenticator _userAuthenticator;

        public LoginController(IUserAuthenticator userAuthenticator)
        {
            _userAuthenticator = userAuthenticator;
        }

        // POST: api/Login
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Post([FromBody] LoginInput model)
        {
            string userToken;
            if (_userAuthenticator.Login(model.Username, model.Password, out userToken))
            {
                //Authentication successful
                return Ok(new
                {
                    username = model.Username,
                    token = userToken
                });
            }
            return Unauthorized("Unknown username and password combination");
        }
    }
}