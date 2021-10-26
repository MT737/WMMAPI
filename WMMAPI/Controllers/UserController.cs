using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using WMMAPI.Database.Models;
using WMMAPI.Interfaces;
using WMMAPI.ViewModels.User;

namespace WMMAPI.Controllers
{
    //TODO: Implement Authorization!!

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly string _contentType = "application/json";

        public UserController(ILogger<UserController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status200OK)]
        public IActionResult Get(Guid userId)
        {
            //TODO: Confirm user
            try
            {
                var result = _userRepository.Get(userId);
                if (result != null)
                {
                    return StatusCode(StatusCodes.Status200OK, result);
                }
                return Problem(
                    "User not found",
                    statusCode: StatusCodes.Status404NotFound
                    );
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                    );                
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status201Created)]
        public IActionResult CreateNewCustomer(NewUserViewModel user)
        {
            if (!_userRepository.EmailExists(user.EmailAddress))
            {
                try
                {
                    User dbUser = user.ToDB();
                    _userRepository.Add(user.ToDB());
                    return StatusCode(StatusCodes.Status201Created, dbUser);
                }
                catch (Exception ex)
                {
                    return Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError
                        );
                }
            }
            return Problem(
                detail: "Account with the provided email address already exists.",
                statusCode: StatusCodes.Status400BadRequest
                );
        }

        [HttpDelete]
        public IActionResult DeleteCustomer(Guid id)
        {
            //TODO: Confirm user

            try
            {
                _userRepository.Delete(id);
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                    );
            }
        }
    }
}
