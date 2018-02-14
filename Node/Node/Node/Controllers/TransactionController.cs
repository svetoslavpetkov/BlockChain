using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlockChain.Core;
using Node.Domain.ApiModels;

namespace Node.Controllers
{
    [Route("api/transaction")]
    public class TransactionController : Controller
    {
        private Domain.Node Node { get; set; }
        public TransactionController(Domain.Node node)
        {
            Node = node;
        }

        [HttpPost("new")]
        public void New([FromBody]Transaction transaction)
        {
            Node.AddTransaction(transaction);
        }

        [HttpGet("{tx}")]
        public IActionResult Get(string tx)
        {
            GetTransactionApiModel result = Node.PendingTransactions
                .Where(t => t.TransactionHash == tx)
                .Select(t=> GetTransactionApiModel.FromTransaction(t))
                .FirstOrDefault();
            if (result == null )
            {
                result = Node.BlockChain
                    .SelectMany(b => b.Value.Transactions.Select(t => new { BlockIndex = b.Key, Transaction = t }))
                    .Where(t => t.Transaction.TransactionHash == tx)
                    .Select(x => GetTransactionApiModel.FromTransaction(x.Transaction, x.BlockIndex))
                    .FirstOrDefault();
            }


            if (result != null)
            {
                return Ok(GetTransactionApiModel.FromTransaction(result));
            }

            return NotFound($"transaction with id {tx} is not found");
        }


        [HttpGet("pending")]
        public IEnumerable<GetTransactionApiModel> GetPending()
        {
            return Node.PendingTransactions.Select(p => GetTransactionApiModel.FromTransaction(p));
        }
    }
}
