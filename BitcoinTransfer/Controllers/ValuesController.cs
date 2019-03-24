using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitcoinTransfer.Services;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace BitcoinTransfer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        BitcoinNetworkService BitcoinNetworkService = new BitcoinNetworkService();
        string memo = "ready frost slot boy enroll primary crunch observe dice category daughter where";
        string address = "miudc6pyYwdVTxuDYCNdsJDESxUZz3wGKq";
        string privateKey = "cPK1Gssv5UD79shik4yoCqpXUuYXoMArwEGjPPfzXHVnzZCRZErm";               

        public ValuesController()
        {
            //BitcoinNetworkService.MssGenerateMnemo(out memo);
            //BitcoinNetworkService.MssGenerateAddress(memo, 2, true, out address, out privateKey);
            //decimal conf;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
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
