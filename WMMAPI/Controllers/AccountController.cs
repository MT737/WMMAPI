using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WMMAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {        
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }        
    }
}
