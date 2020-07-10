
using System.Web.Http;

namespace NCWebApi.Controllers
{

    public class HealthController : ApiController
    {
        [HttpGet]
        [Route("Health")]
        public string Index()
        {
            return "OK";
        }
    }
}
