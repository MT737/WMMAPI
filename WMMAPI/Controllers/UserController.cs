using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using WMMAPI.Database;
using WMMAPI.Interfaces;
using WMMAPI.Repositories;
using WMMAPI.ViewModels;

namespace WMMAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _userRepository;

        public UserController(ILogger<UserController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpGet]
        public UserViewModel Get(Guid userId)
        {
            return new UserViewModel(_userRepository.Get(userId));
        }
    }
}
