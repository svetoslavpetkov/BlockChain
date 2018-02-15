using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Domain.ApiModels
{
    public class BlockMinedApiModel
    {
        public string MinerAddress { get; set; }

        public int Nonce { get; set; }

        public string Hash { get; set; }

        //string minerAddress, int nonce, string hash
    }
}
