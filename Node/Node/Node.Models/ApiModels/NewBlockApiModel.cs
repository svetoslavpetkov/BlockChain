namespace Node.Domain
{
    public class NewBlockApiModel
    {
        public BlockSyncApiModel Block { get; set; }
        public string NodeAddress { get; set; }
    }
}
