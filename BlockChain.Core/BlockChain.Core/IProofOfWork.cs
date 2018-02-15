using System;
using System.Collections.Generic;
using System.Text;

namespace BlockChain.Core
{
    public interface IProofOfWork
    {
        string PrecomputeData(int blockIndex, string blockDataHash, string prevBlockHash, DateTime timestamp);

        string Compute(int blockIndex, string blockDataHash, string prevBlockHash, DateTime timestamp, int nonce);

        string Compute(string precomputedData, int nonce);

        bool IsProofValid(int difficulty, int blockIndex, string blockDataHash, string prevBlockHash, DateTime timestamp, int nonce, string blockHash);

        bool IsProofValid(string blockHash, int difficulty);
    }
}
