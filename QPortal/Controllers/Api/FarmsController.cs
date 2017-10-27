using QPortal.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace QPortal.Controllers.Api
{
    public class FarmsController : ApiController
    {

        // GET /api/farms/1/1
        [Route("api/Farms/{id}/{node}")]
        public IHttpActionResult GetFarmNode(int id, int node)
        {
            Node one = new Node { Id = 1, Name = "Ambiente Instituzionale", Link = "http://localhost:4848/sense/app/C%3A%5CUsers%5Cbrasoveanum%5CDocuments%5CQlik%5CSense%5CApps%5CExecutive%20Dashboard.qvf" };
            Node two = new Node { Id = 2, Name = "Self Service Analysis", Link = "https://www.youtube.com/embed/vGF-f3arb04" };

            List<Node> Nodes = new List<Node>();

            Nodes.Add(one);
            Nodes.Add(two);

            List<Farms> FarmsList = new List<Farms>();

            Farms first = new Farms() { Id = "1", Name = "Farm 1", Nodes = Nodes };
            Farms second = new Farms() { Id = "2", Name = "Farm 2", Nodes = Nodes };

            FarmsList.Add(first);
            FarmsList.Add(second);

            var requested = FarmsList[id].Nodes[node];

            return Ok(requested);
        }
    }
}
