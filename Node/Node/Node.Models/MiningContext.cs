
using System;

namespace Node.Models
{
    public class MiningContext
    {
        public int BlockIndex { get; set; }
        public string BlockData { get; set; }
        public string PrevBlockHash { get; set; }
        public int Difficulty { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
