using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMMAPI.Interfaces;

namespace WMMAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionController(ILogger<TransactionController> logger, ITransactionRepository transactionRepo)
        {
            _logger = logger;
            _transactionRepository = transactionRepo;
        }

        // Get list of transactions

        // Add Transaction

        // Modify Transaction

        // Delete Transaction
    }
}
