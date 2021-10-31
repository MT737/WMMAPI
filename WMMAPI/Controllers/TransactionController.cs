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
        private readonly ITransactionRepository _transactionRepository;

        public TransactionController(ILogger<TransactionController> logger, ITransactionRepository transactionRepo)
        {
            _logger = logger;
            _transactionRepository = transactionRepo;
        }

        [HttpGet]
        public IActionResult GetTransactions()
        {
            try
            {
                Guid userId = Guid.Parse(User.Identity.Name);
                List<TransactionModel> transactions = _transactionRepository
                    .GetList(userId, true)
                    .Select(t => new TransactionModel(t))
                    .ToList();
                return Ok(transactions);
            }
            catch (Exception ex) //No app exceptions in this pathway
            {
                //TODO: Add logging of exception. Only including in return for testing purposes.
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
                _transactionRepository.Add(dbModel);

                // Pull saved transaction from db in order to pull names (Convenience feature for API consumer)
                TransactionModel returnModel = new TransactionModel(_transactionRepository
                    .Get(userId, dbModel.TransactionId, true));

                return Ok(returnModel);
            }
            catch (Exception ex) //No app exceptions in this pathway
            { 
                //TODO: Add logging of exception. Only including in return for testing purposes.
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
                _transactionRepository.ModifyTransaction(dbModel);
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
                _transactionRepository.DeleteTransaction(Guid.Parse(User.Identity.Name), transactionId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
