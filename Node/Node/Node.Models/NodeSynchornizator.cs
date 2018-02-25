using BlockChain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Domain
{
    public interface INodeSynchornizator
    {
        List<Peer> Peers { get; }

        void BroadcastTransaction(Transaction tx);
        void BroadcastBlock(Block block);
        PeerApiModel AddNewlyConnectedPeer(PeerApiModel peer);
        List<Block> GetBlocksForSync(string nodeAddress);
        List<Block> SyncBlocks();

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
            for (int port = 5555; port < 5560; port++)
            {
                string url = $"http://localhost:{port}";
                if (url == Current.Url)
                {
                    continue;
                }
                RestClient cl = new RestClient(url);
                try
                {
                    PeerApiModel pm = new PeerApiModel() { Url = Current.Url, Name = Current.Name };
                    PeerApiModel foundNode = cl.Post<PeerApiModel, PeerApiModel>("api/peers/connect", pm);
                    if (foundNode != null)
                        Peers.Add(new Peer(foundNode.Url,foundNode.Name));
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void Init(Peer peer)
        {
            Current = peer;
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
                catch (Exception ex) { }
            }
        }

        public PeerApiModel AddNewlyConnectedPeer(PeerApiModel p)
        {
            Peer peer = new Peer(p.Url, p.Name);
            Peers.Add(peer);

            return PeerApiModel.FromPeer(Current);
        }

        public List<Block> GetBlocksForSync(string nodeAddress)
        {
            RestClient client = new RestClient(nodeAddress);
            var blockModels = client.Get<List<BlockSyncApiModel>>($"api/block");
            List<Block> blocks = new List<Block>();

            if(blockModels.Count > 1)
            for (int i = 1; i < blockModels.Count; i++)
                blocks.Add(Block.ReCreateBlock(blockModels[i]));

            return blocks;
        }

        public List<Block> SyncBlocks()
        {
            SyncPeers();
            List<Block> blocks = new List<Block>();
            foreach (Peer p in Peers)
            {
                try
                {
                   List<Block> peerBlock =  GetBlocksForSync(p.Url);
                    if (blocks.Count < peerBlock.Count)
                        blocks = peerBlock;
                }
                catch (Exception ex) { }
            }

            return blocks;
        }
    }
}
