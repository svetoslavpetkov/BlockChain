using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BlockChain.Core;
using Node.Domain;

namespace Node.Controllers
{
    [Produces("application/json")]
    [Route("api/transaction")]
    public class TransactionController : Controller
    {
        private Domain.Node Node { get; set; }
        private ITransactionQuery TransactionQuery { get; set; }

        public TransactionController(Domain.Node node, ITransactionQuery transactionQuery)
        {
            Node = node;
            TransactionQuery = transactionQuery;
        }

        [HttpPost("new")]
        public IActionResult New([FromBody]Transaction transaction)
        {
            try
            {
                Node.AddTransaction(transaction);
                return Ok();
            }
            catch (BalanceNotEnoughException ex)
            {
                return BadRequest(new { Error = ex.Message.ToString() });
            }

        }

        [HttpGet("{tx}")]
        [ProducesResponseType(typeof(GetTransactionApiModel), 200)]
        public IActionResult Get(string tx)
        {
            GetTransactionApiModel result = TransactionQuery.Get(tx);

            if (result == null)
                return NotFound($"transaction with id {tx} is not found");

            return Ok(result);
        }

        [HttpGet("pending")]
        [ProducesResponseType(typeof(IEnumerable<GetTransactionApiModel>), 200)]
        public IActionResult GetPending()
        {
            IEnumerable<GetTransactionApiModel> pendingTx = TransactionQuery.GetPending();
            return Ok(pendingTx);
        }
    }
}
