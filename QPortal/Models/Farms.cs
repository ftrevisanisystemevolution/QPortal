using System.Collections.Generic;

namespace QPortal.Models
{
    public class Farms
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Node> Nodes { get; set; }
    }
}