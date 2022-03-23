using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Models.TransactionModels;
using static WMMAPI.Helpers.Globals.ErrorMessages;

namespace WMMAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionService _transactionService;

        public Guid UserId { get; set; }

        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService)
        {
            _logger = logger;
            _transactionService = transactionService;
            
            UserId = User != null ? Guid.Parse(User.Identity.Name) : Guid.Empty;
        }

        [HttpGet]
        public IActionResult GetTransactions()
        {
            if (UserId == Guid.Empty)
                return BadRequest(new ExceptionResponse(AuthenticationError));

            try
            {
                IList<TransactionModel> transactions = _transactionService
                    .GetList(UserId, true)
                    .Select(t => new TransactionModel(t))
                    .ToList();
                return Ok(transactions);
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                // TODO Add logging here to get the actual error.
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpPost]
        public IActionResult AddTransaction([FromBody] AddTransactionModel model)
        {
            if (UserId == Guid.Empty)
                return BadRequest(new ExceptionResponse(AuthenticationError));

            try
            {
                var dbModel = model.ToDB(UserId);
                _transactionService.AddTransaction(dbModel);

                // Pull saved transaction from db in order to pull names (Convenience feature for API consumer)
                TransactionModel returnModel = new TransactionModel(_transactionService
                    .Get(dbModel.Id, UserId, true));

                return Ok(returnModel);
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                // TODO Add logging here to get the actual error.
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpPut]
        public IActionResult ModifyTransaction([FromBody] TransactionModel model)
        {
            if (UserId == Guid.Empty)
                return BadRequest(new ExceptionResponse(AuthenticationError));

            try
            {
                var dbModel = model.ToDB(UserId);
                _transactionService.ModifyTransaction(dbModel);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                // TODO Add logging here to get the actual error.
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpDelete, Route("{id}")]
        public IActionResult DeleteTransaction(Guid transactionId)
        {
            if (UserId == Guid.Empty)
                return BadRequest(new ExceptionResponse(AuthenticationError));

            try
            {
                _transactionService.DeleteTransaction(UserId, transactionId);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                // TODO Add logging here to get the actual error.
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }
    }
}
