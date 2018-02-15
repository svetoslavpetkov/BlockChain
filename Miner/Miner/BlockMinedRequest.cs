using System;
using System.Collections.Generic;
using System.Text;

namespace Miner
{
    public class BlockMinedRequest
    {
        public string MinerAddress { get; set; }

        public ulong Nonce { get; set; }

        public string Hash { get; set; }
    }
}
