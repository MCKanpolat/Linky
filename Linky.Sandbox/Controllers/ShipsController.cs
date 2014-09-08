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
        [Route, HttpGet]
        [LinksFrom(typeof(ShipCollection), "next", Resolve = new []{"page", "NextPage"})]
        [LinksFrom(typeof(ShipCollection), "previous", Resolve = new []{"page", "PreviousPage"})]
        public ShipCollection List(int page = 1)
        {
            return new ShipCollection
            {
                Ships = new[] {new Ship {Id = 1, Name = "Millennium Falcon"}, new Ship {Id = 42, Name = "Heart of Gold"}},
                NextPage = page + 1,
                PreviousPage = page > 1 ? page - 1 : default(int?)
            };
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