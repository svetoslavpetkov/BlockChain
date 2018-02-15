using System;
using System.Collections.Generic;
using System.Text;

namespace Miner
{
    public class BlockMinedRequest
    {
        public string MinerAddress { get; set; }

        public int Nonce { get; set; }

        public string Hash { get; set; }
    }
}
