namespace Linky.Sandbox.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ShipCollection
    {
        public IList<Ship> Ships { get; set; }
        [JsonIgnore]
        public int? PreviousPage { get; set; }
        [JsonIgnore]
        public int? NextPage { get; set; }
    }
}