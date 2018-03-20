using QPortal.Models;
using QPortal.Utility;
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
            //To Do : check if user has permission first 

            var requested = AmbitiUtility.GetAmbitoNode(id, node);

            if (requested == null)
                return NotFound();

            return Ok(requested);
        }
    }
}
