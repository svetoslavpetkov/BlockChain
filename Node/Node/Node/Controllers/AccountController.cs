using Microsoft.AspNetCore.Mvc;

namespace Node.Controllers
{
    [Produces("application/json")]
    [Route("api/account")]
    public class AccountController : Controller
    {
        private Domain.IAddressQuery AddressQuery { get; set; }
        private Domain.ITransactionQuery TransactionQuery { get; set; }

        public AccountController(Domain.IAddressQuery addressQuery, Domain.ITransactionQuery transactionQuery)
        {
            AddressQuery = addressQuery;
            TransactionQuery = transactionQuery;
        }

        [HttpGet("{address}/ballance")]
        public IActionResult GetBallance(string address)
        {
            ulong balance = AddressQuery.GetBalance(address);
            return Ok(balance);
        }

        [HttpGet("{address}/unconfirmed-balance")]
        public IActionResult GetUnconfirmedBallance(string address)
        {
            ulong balance = AddressQuery.GetUnconfirmedBalance(address);
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