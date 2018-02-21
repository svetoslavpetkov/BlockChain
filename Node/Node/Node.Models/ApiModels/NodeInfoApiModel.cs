using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Domain.ApiModels
{
    public class NodeInfoApiModel
    {
        public string About { get; set; }
        public string Name { get; set; }

        public int Peers { get; set; }

        public int Difficulty { get; set; }

        public int ConfirmedTransactions { get; set; }

        public int PendingTransactions { get; set; }

        public DateTime Started { get; set; }
    }
}
