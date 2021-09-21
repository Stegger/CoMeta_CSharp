using System;
using System.Collections.Generic;
using CoMeta.Data;
using CoMeta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoMeta.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IRepository<User> _userRepo;

        public UserController(IRepository<User> userRepo, ILogger<UserController> logger)
        {
            _logger = logger;
            _userRepo = userRepo;
        }

        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _userRepo.GetAll();
        }

        [HttpGet("{id:long}", Name = "Get")]
        public IActionResult Get(long id)
        {
            var item = _userRepo.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            if (user == null)
                return BadRequest();
            
            _userRepo.Add(user);
            return CreatedAtRoute("Get", new { id = user.Id }, user);
        }
        
    }
    
}