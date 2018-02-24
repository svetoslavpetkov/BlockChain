using BlockChain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Domain
{
    public interface INodeSynchornizator
    {
        List<Peer> Peers { get; }

        List<Block> Sync();
        void BroadcastTransaction(Transaction tx);
        void BroadcastBlock(Block block);
        void AddNewlyConnectedPeer(Peer peer);
        List<BlockSyncApiModel> GetBlocksForSync(int fromIndex, int count);
    }

    public class NodeSynchornizator : INodeSynchornizator
    {   
        private class NodeDetails
        {
            public string Name { get; set; }
        }

        public List<Peer> Peers { get; private set; }
        private Peer Current { get; set; }

        public NodeSynchornizator(Peer currentNode)
        {
            Peers = new List<Peer>();
            Current = currentNode;
        }

        private void SyncPeers()
        {
            for (int port = 5500; port < 5600; port++)
            {
                string url = $"http://localhost:{port}";
                RestClient cl = new RestClient(url);
                Peer foundNode = cl.Post<Peer, Peer>("peer/connect", Current);
                if (foundNode != null)
                    Peers.Add(foundNode);
            }
        }

        public void Init(Peer peer)
        {
            Current = peer;
        }

        public List<Block> Sync()
        {
            return null;
        }

        public void BroadcastTransaction(Transaction tx)
        {
            foreach (Peer p in Peers)
            {
                try
                {
                    RestClient client = new RestClient(p.Url);
                    client.Post("api/transaction/new", tx);
                }
                catch (Exception) { }
            }
        }

        public void BroadcastBlock(Block block)
        {
            foreach (Peer p in Peers)
            {
                try
                {
                    RestClient client = new RestClient(p.Url);
                    client.Post("api/block/new", block);
                }
                catch (Exception) { }
            }
        }

        public void AddNewlyConnectedPeer(Peer peer)
        {
            Peers.Add(peer);
        }

        private BlockSyncApiModel Convert(Block b)
        {
            return null;
          // return  Block.ReCreateBlock(b);
        }

        public List<BlockSyncApiModel> GetBlocksForSync(int fromIndex, int count)
        {
            throw new NotImplementedException();
        }
    }
}
