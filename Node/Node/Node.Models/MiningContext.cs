
using System;

namespace Node.Domain
{
    public class MiningContext
    {
        public int BlockIndex { get; set; }
        public string BlockHash { get; set; }
        public string PrevBlockHash { get; set; }
        public int Difficulty { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
