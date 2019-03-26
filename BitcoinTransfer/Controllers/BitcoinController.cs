using System.Collections.Generic;
using System.Threading.Tasks;
using BitcoinTransfer.Interfaces.Services;
using BitcoinTransfer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BitcoinTransfer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BitcoinController : ControllerBase
    {
        private readonly IBitcoinService _bitcoinService;

        public BitcoinController(IBitcoinService bitcoinService)
        {
            _bitcoinService = bitcoinService;
            //decimal conf;
        }

        // GET api/values
        [HttpGet("getlast")]
        public async Task<ActionResult<IEnumerable<LastTransactionModel>>> Get()
        {
            return Ok(await _bitcoinService.ProcessGetLast());
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult<bool>> Post([FromBody] string toAddress, decimal amount)
        {
            await _bitcoinService.TransferBitcoin(amount, toAddress, true);
            return Accepted(true);
        }
    }
}
