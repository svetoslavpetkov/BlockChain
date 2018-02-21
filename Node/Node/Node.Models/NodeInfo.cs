using System;

namespace Node.Domain
{
    public class NodeInfo
    {
        public Guid Identifier { get; set; } = new Guid();
        public string Name { get; private set; }
        public string About { get; private set; }

        public NodeInfo()
        {
            Name = $"Node {Identifier.ToString().Substring(6).ToUpper()}";
            About = "Node v1.0";    
        }
    }
}
