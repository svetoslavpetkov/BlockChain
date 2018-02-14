using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;

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
            Node.AddTransaction(null);
        }
    }
}
