using System.Collections.Generic;

namespace QPortal.Models
{
    public class Farms
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Node> Nodes { get; set; }
        public string role { get; set; }
        public string superuserid { get; set; }
        public string superuserdom { get; set; }
        public string centralnode { get; set; }
    }
}