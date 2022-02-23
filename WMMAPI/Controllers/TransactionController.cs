using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Models.TransactionModels;

namespace WMMAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionService _transactionService;

        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService)
        {
            _logger = logger;
            _transactionService = transactionService;
        }

        [HttpGet]
        public IActionResult GetTransactions()
        {
            try
            {
                Guid userId = Guid.Parse(User.Identity.Name);
                List<TransactionModel> transactions = _transactionService
                    .GetList(userId, true)
                    .Select(t => new TransactionModel(t))
                    .ToList();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "There was an error processing your request.", ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddTransaction([FromBody] AddTransactionModel model)
        {
            try
            {
                Guid userId = Guid.Parse(User.Identity.Name);
                var dbModel = model.ToDB(userId);
                _transactionService.AddTransaction(dbModel);

                // Pull saved transaction from db in order to pull names (Convenience feature for API consumer)
                TransactionModel returnModel = new TransactionModel(_transactionService
                    .Get(userId, dbModel.Id, true));

                return Ok(returnModel);
            }
            catch (Exception ex)
            { 
                return BadRequest(new { message = "There was an error processing your request.", ex.Message });
            }
        }

        [HttpPut]
        public IActionResult ModifyTransaction([FromBody] TransactionModel model)
        {
            try
            {
                Guid userId = Guid.Parse(User.Identity.Name);
                var dbModel = model.ToDB(userId);
                _transactionService.ModifyTransaction(dbModel);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete, Route("{id}")]
        public IActionResult DeleteTransaction(Guid transactionId)
        {
            try
            {
                _transactionService.DeleteTransaction(Guid.Parse(User.Identity.Name), transactionId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
