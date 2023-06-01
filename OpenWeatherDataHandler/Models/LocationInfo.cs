using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace OpenWeatherDataHandler.Models
{
    public class LocationInfo
    {
        public class Location
        {
            [JsonProperty("id")]
            public float? Id { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("state")]
            public string? State { get; set; }

            [JsonProperty("country")]
            public string? Country { get; set; }

            [JsonProperty("coord")]
            public Coord Coord { get; set; }
            public int InternalId { get; set; }
        }

        public class Coord
        {
            [JsonProperty("lon")]
            public double Lon { get; set; }

            [JsonProperty("lat")]
            public double Lat { get; set; }
        }
    }
}
