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
        List<Block> GetBlocksForSync(int startIndex, int count, string nodeAddress);
        void SyncPeers();
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

        public void SyncPeers()
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
                    NewBlockApiModel apiModel = new NewBlockApiModel();
                    apiModel.Block = BlockSyncApiModel.FromBlock(block);
                    apiModel.NodeAddress = Current.Url;

                    client.Post("api/block/new", apiModel);
                }
                catch (Exception) { }
            }
        }

        public void AddNewlyConnectedPeer(Peer peer)
        {
            Peers.Add(peer);
        }

        public List<Block> GetBlocksForSync(int startIndex, int count, string nodeAddress)
        {
            RestClient client = new RestClient(nodeAddress);
            var blockModels = client.Get<List<BlockSyncApiModel>>($"api/getblocksByFromIndexAndCount/{startIndex}/{count}");
            List<Block> blocks = new List<Block>();

            foreach (var bm in blockModels)
                blocks.Add(Block.ReCreateBlock(bm));

            return blocks;
        }
    }
}
