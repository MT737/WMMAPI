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
using WMMAPI.Services.UserService.UserModels;
using static WMMAPI.Helpers.ClaimsHelpers;
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
        private readonly AppSettings _appSettings;
        
        public Guid UserId { get; set; }

        public UserController(ILogger<UserController> logger, IUserService userService, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _userService = userService;
            _appSettings = appSettings.Value;
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
                    return BadRequest(new ExceptionResponse(InvalidEmailAndPassword));

                try
                {
                    // Create an authentication token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
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
                catch (Exception)
                {
                    // TODO Log these for sure as they will be due to JWT work.
                    return BadRequest(new ExceptionResponse(GenericErrorMessage));
                }
            }
            catch (Exception ex)
            {
                // TODO  Log these? They are likely due to password authentication issues.
                return BadRequest(new ExceptionResponse(ex.Message));
            }            
        }

        [AllowAnonymous]
        [HttpPut("register")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult RegisterUser([FromBody] RegisterUserModel user)
        {
            try
            {
                User dbUser = user.ToDB();
                _userService.Create(dbUser, user.Password);
                return StatusCode(StatusCodes.Status204NoContent);
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
                UserId = GetUserId(UserId, User);
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
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }            
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult ModifyUser([FromBody] UpdateUserModel model)
        {
            try
            {
                UserId = GetUserId(UserId, User);
                var dbUser = model.ToDB(UserId);    
                _userService.Modify(dbUser, model.Password);
                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteUser()
        {
            try
            {
                UserId = GetUserId(UserId, User);
                _userService.RemoveUser(UserId);
                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
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
