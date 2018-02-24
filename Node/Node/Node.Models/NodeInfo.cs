using System;

namespace Node.Domain
{
    public class NodeInfo
    {
        public Guid Identifier { get; set; }
        public string Name { get; private set; }
        public string About { get; private set; }

        public NodeInfo()
        {
            Identifier = Guid.NewGuid(); 
            Name = $"Node {Identifier.ToString().Substring(6).ToUpper()}";
            About = "Node v1.0";    
        }
    }
}
