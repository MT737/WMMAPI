using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Models.AccountModels;

namespace WMMAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {        
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;

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
                // Get list of accounts
                var accountsWithBalance = _accountService
                    .GetList(Guid.Parse(User.Identity.Name));

                return Ok(accountsWithBalance);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddAccount([FromBody] AddAccountModel model)
        {            
            try
            {
                var account = model.ToDB(Guid.Parse(User.Identity.Name));
                _accountService.AddAccount(account);

                //TODO: Need to add functionality for setting initial balance                
                return Ok(new AccountModel(account));
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public IActionResult ModifyAccount([FromBody] UpdateAccountModel model)
        {
            try
            {
                var account = model.ToDB(Guid.Parse(User.Identity.Name));
                _accountService.ModifyAccount(account);

                //TODO: Add functionality for adjusting balance
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
