using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Models.AccountModels;
using static WMMAPI.Helpers.Globals.ErrorMessages;
using static WMMAPI.Helpers.ClaimsHelpers;

namespace WMMAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {        
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;

        public Guid UserId { get; set; }

        public AccountController(ILogger<AccountController> logger, IAccountService accountService)
        {
            _logger = logger;
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult GetAccounts()
        {
            try
            {
                UserId = GetUserId(UserId, User);
                
                // Get list of accounts
                var accountsWithBalance = _accountService.GetList(UserId);

                return Ok(accountsWithBalance);
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

        [HttpPost]
        public IActionResult AddAccount([FromBody] AddAccountModel model)
        {
            try
            {
                UserId = GetUserId(UserId, User);

                var account = model.ToDB(UserId);
                _accountService.AddAccount(account);

                //TODO: Need to add functionality for setting initial balance                
                return Ok(new AccountModel(account));
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
        public IActionResult ModifyAccount([FromBody] UpdateAccountModel model)
        {
            try
            {
                UserId = GetUserId(UserId, User);

                var account = model.ToDB(UserId);
                _accountService.ModifyAccount(account);

                //TODO: Add functionality for adjusting balance
                return Ok();
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
