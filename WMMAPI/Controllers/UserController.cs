using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Models.UserModels;
using static WMMAPI.Helpers.Globals.ErrorMessages;

namespace WMMAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        
        public Guid UserId { get; set; }
        public string Secret { get; set; } // TODO hack to allow unit testing. Find a better way

        public UserController(ILogger<UserController> logger, IUserService userService, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _userService = userService;

            UserId = User != null ? Guid.Parse(User.Identity.Name) : Guid.Empty;
            Secret = appSettings.Value != null ? appSettings.Value.Secret : null; // TODO hack to allow unit testing. Find a better way
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        [ProducesResponseType(typeof(AuthenticatedUserModel), StatusCodes.Status200OK)]
        public IActionResult AuthenticateUser([FromBody] AuthenticateUserModel model)
        {
            try
            {
                var user = _userService.Authenticate(model.EmailAddress, model.Password);

                if (user == null)
                    return BadRequest(new ExceptionResponse("Email address and password combination is incorrect."));

                try
                {
                    // Create an authentication token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(Secret);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, user.Id.ToString())
                        }),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    // return basic user info and authentication token
                    return Ok(new AuthenticatedUserModel(user, tokenString));
                }
                catch (Exception e)
                {
                    return BadRequest(new ExceptionResponse(GenericErrorMessage));
                }
            }
            catch (Exception ex)
            {
                // TODO  Errors at this level are likely internal issues with settings. Should log these instead of returning them?
                return BadRequest(new ExceptionResponse(ex.Message));
            }            
        }

        [AllowAnonymous]
        [HttpPut("register")]
        public IActionResult RegisterUser([FromBody] RegisterUserModel user)
        {
            try
            {
                User dbUser = user.ToDB();
                _userService.Create(dbUser, user.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            try
            {
                var result = _userService.GetById(UserId);
                if (result != null)
                {
                    return Ok(new UserModel(result));
                }
                return BadRequest(new ExceptionResponse("User not found"));
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpPut]
        public IActionResult ModifyUser([FromBody] UpdateUserModel model)
        {            
            try
            {
                var dbUser = model.ToDB(UserId);    
                _userService.Modify(dbUser, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpDelete]
        public IActionResult DeleteUser()
        {            
            try
            {
                _userService.RemoveUser(UserId);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }
    }
}
