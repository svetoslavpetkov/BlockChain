using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlockChain.Core;

namespace Node.Controllers
{
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {
        private Domain.Node Node { get; set; }
        public TransactionController(Domain.Node node)
        {
            Node = node;
        }

        [HttpPost]
        public void New([FromBody]Transaction transaction)
        {
            Node.AddTransaction(transaction);
        }

        [HttpGet("api/[controller]/{tx}")]
        public void Get(string tx)
        {
            var pendingTransaction = Node.PendingTransactions.FirstOrDefault(t => t.TransactionHash == tx);
        }
    }
}
