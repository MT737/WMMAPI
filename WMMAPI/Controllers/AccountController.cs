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
        public IActionResult GetAccountsByUserId()
        {
            Guid userId = Guid.Parse(User.Identity.Name);            
            try
            {
                // Get list of accounts
                var accounts = _accountRepository.GetList(userId);
                List<AccountModel> accountsWithBalance = accounts.Select(x => new AccountModel(x)).ToList();

                // Get balances
                foreach (var item in accountsWithBalance)
                {
                    item.Balance = _accountRepository.GetBalance(item.AccountId, item.UserId, item.IsAsset);
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
        public IActionResult AddAccount(AddAccountModel model)
        {
            Guid userId = Guid.Parse(User.Identity.Name);
            try
            {
                var account = model.ToDB(userId);
                _accountRepository.AddAccount(account);

                //TODO: Need to add functionality for setting initial balance
                //Can leverage transaction repo...
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public IActionResult ModifyAccount(UpdateAccountModel model)
        {            
            Guid userId = Guid.Parse(User.Identity.Name);
            try
            {
                var account = model.ToDB(userId);
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
