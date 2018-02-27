using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{address}/unconfirmed-balance")]
        public IActionResult GetUnconfirmedBallance(string address)
        {
            ulong balance = Node.GetUnconfirmedBalance(address);
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