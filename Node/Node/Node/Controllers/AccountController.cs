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
        public AccountController(Domain.Node node)
        {
            Node = node;
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
            return Ok(Node.GetTransactions(address, true)
                        .Select( tx => GetTransactionApiModel.FromTransaction(tx))
                        .OrderByDescending(tx => tx.DateCreated)
                        .Take(count)
                        .ToList());
        }
    }
}