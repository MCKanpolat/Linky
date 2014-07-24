namespace Linky.Sandbox.Controllers
{
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Routing;
    using Models;

    [RoutePrefix("api/ships")]
    public class ShipsController : ApiController
    {
        // GET api/ships
        [Route]
        public IEnumerable<Ship> Get()
        {
            return new[] {new Ship {Id = 1, Name = "Millennium Falcon"}, new Ship {Id = 42, Name = "Heart of Gold"}};
        }

        // GET api/ships/5
        [Route("{id}")]
        [LinksFrom(typeof (Ship), "self")]
        public Ship Get(int id)
        {
            return id == 1
                ? new Ship {Id = 1, Name = "Millennium Falcon"}
                : id == 42
                    ? new Ship {Id = 42, Name = "Heart of Gold"}
                    : null;
        }
    }
}