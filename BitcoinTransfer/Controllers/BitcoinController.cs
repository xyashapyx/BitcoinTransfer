using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitcoinTransfer.Interfaces.Services;
using BitcoinTransfer.Services;
using BitcoinTransfer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace BitcoinTransfer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BitcoinController : ControllerBase
    {
        private readonly IBitcoinService _bitcoinService;
        string memo = "ready frost slot boy enroll primary crunch observe dice category daughter where";
        string address = "miudc6pyYwdVTxuDYCNdsJDESxUZz3wGKq";
        string privateKey = "cPK1Gssv5UD79shik4yoCqpXUuYXoMArwEGjPPfzXHVnzZCRZErm";               

        public BitcoinController(IBitcoinService bitcoinService)
        {
            _bitcoinService = bitcoinService;
            //BitcoinNetworkService.MssGenerateMnemo(out memo);
            //BitcoinNetworkService.MssGenerateAddress(memo, 2, true, out address, out privateKey);
            //decimal conf;
        }

        // GET api/values
        [HttpGet("getlast")]
        public ActionResult<IEnumerable<LastTransactionModel>> Get(string toAddress, double amount)
        {
            string transId;            
            ////BitcoinNetworkService.MssTransfer(privateKey, 0.0001m, "17dPTkqwyRH75rpYJeu1AMhzB8B1KJ9ab8", 0.00000001m, true, out transId);
            return "";
        }

        // GET api/values/5
        [HttpGet("{addressToSend}")]
        public ActionResult<string> Get(string addressToSend)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }      

    }
}
