using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veslink.Entities
{
    //ReporteNavesCHEM_1
    public class VoyageItinerary
    {
        [JsonProperty("VoyageItineraries.PortFunc")]
        public string PortFunc { get; set; }

        [JsonProperty("VoyageItineraries.Order")]
        public int Order { get; set; }

        [JsonProperty("VoyageItineraries.PortName")]
        public string PortName { get; set; }

        [JsonProperty("VoyageItineraries.EtaGmt")]
        public DateTime? EtaGmt { get; set; }

        [JsonProperty("VoyageItineraries.EtdGmt")]
        public DateTime? EtdGmt { get; set; }

        [JsonProperty("VoyageItineraries.Miles")]
        public int Miles { get; set; }

        [JsonProperty("VoyageItineraries.LSMiles")]
        public int LSMiles { get; set; }

        [JsonProperty("VoyageItineraries.FixtureNo")]
        public string FixtureNo { get; set; }

        [JsonProperty("VoyageItineraries.Seq")]
        public int Seq { get; set; }
    }
}