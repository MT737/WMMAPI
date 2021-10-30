using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IAccountRepository _accountRepository;

        public AccountController(ILogger<AccountController> logger, IAccountRepository accountRepository)
        {
            _logger = logger;
            _accountRepository = accountRepository;
        }
        
        [HttpGet]
        public IActionResult GetAccounts()
        {
            try
            {
                // Get list of accounts
                List<AccountModel> accountsWithBalance = _accountRepository
                    .GetList(Guid.Parse(User.Identity.Name))
                    .Select(x => new AccountModel(x)).ToList();

                // Get balances
                foreach (var item in accountsWithBalance)
                {
                    item.Balance = _accountRepository.GetBalance(item.AccountId, Guid.Parse(User.Identity.Name), item.IsAsset);
                }

                return Ok(accountsWithBalance);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //TODO: Add endpoint for returning paged list?

        [HttpPost]
        public IActionResult AddAccount([FromBody] AddAccountModel model)
        {            
            try
            {
                var account = model.ToDB(Guid.Parse(User.Identity.Name));
                _accountRepository.AddAccount(account);

                //TODO: Need to add functionality for setting initial balance
                //Can leverage transaction repo...
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
                _accountRepository.ModifyAccount(account);

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
