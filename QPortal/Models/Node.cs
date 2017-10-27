namespace QPortal.Models
{
    public class Node
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Server { get; set; }
        public string VirtualProxy { get; set; }

        public string btnEnterClass { get; set; }
        public string btnArchiveClass { get; set; }
        public string btnDistributeClass { get; set; }
    }
}