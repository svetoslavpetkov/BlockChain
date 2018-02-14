using System;
using System.Collections.Generic;
using System.Text;

namespace Miner
{
    public class BlockInput
    {
        public int BlockIndex { get; set; }
        public string BlockHash { get; set; }
        public string PrevBlockHash { get; set; }
        public int Difficulty { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
