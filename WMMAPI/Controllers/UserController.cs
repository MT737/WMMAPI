using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using WMMAPI.Database;
using WMMAPI.Repositories;
using WMMAPI.ViewModels;

namespace WMMAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;        

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public UserViewModel Get(Guid userId)
        {
            var context = new WMMContext();
            UserRepository repo = new UserRepository(context);
            return new UserViewModel(repo.Get(userId));
        }
    }
}
