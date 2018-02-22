using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Node.Domain.ApiModels;

namespace Node.Controllers
{
    [Produces("application/json")]
    [Route("api/account")]
    public class AccountController : Controller
    {
        private Domain.Node Node { get; set; }
        private Domain.ITransactionQuery TransactionQuery { get; set; }

        public AccountController(Domain.Node node, Domain.ITransactionQuery transactionQuery)
        {
            Node = node;
            TransactionQuery = transactionQuery;
        }

        [HttpGet("{address}/ballance")]
        public IActionResult GetBallance(string address)
        {
            ulong balance = Node.GetBalance(address);
            return Ok(balance);
        }

        [HttpGet("{address}/latesttransactions/{count}")]
        public IActionResult GetTransactions(string address, int count)
        {
            var txs = TransactionQuery.GetTransactions(address, count);
            return Ok(txs);
        }
    }
}