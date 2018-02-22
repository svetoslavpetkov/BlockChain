using Microsoft.AspNetCore.Mvc;
using Node.Domain.ApiModels;

namespace Node.Controllers
{
    [Produces("application/json")]
    [Route("api/info")]
    public class InfoController : Controller
    {
        private Domain.IInfoQuery InfoQuery { get; set; }

        public InfoController(Domain.IInfoQuery infoQuery)
        {
            InfoQuery = infoQuery;
        }

        [HttpGet]
        public IActionResult Get()
        {
            NodeInfoApiModel result = InfoQuery.GetInfo();
            return Ok(result);
        }
    }
}
