using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veslink.Entities
{
    public class VoyageLegSummary
    {
        public string VesselName { get; set; }
        public int VoyageNo { get; set; }
        public string VesselCode { get; set; }

        [JsonProperty("VoyageItineraries.VoyageItineraryBunkers.FuelType")]
        public string FuelType { get; set; }

        [JsonProperty("VoyageItineraries.VoyageItineraryBunkers.RobArrival")]
        public double RobArrival { get; set; }

        [JsonProperty("VoyageItineraries.VoyageItineraryBunkers.RobDeparture")]
        public double RobDeparture { get; set; }

        [JsonProperty("VoyageItineraries.VoyageItineraryBunkers.Seq")]
        public int Seq { get; set; }

        [JsonProperty("VoyageItineraries.PortName")]
        public string PortName { get; set; }

        [JsonProperty("VoyageItineraries.PortFunc")]
        public string PortFunc { get; set; }

        [JsonProperty("VoyageItineraries.PortNo")]
        public int PortNo { get; set; }

        [JsonProperty("VoyageItineraries.Order")]
        public int Order { get; set; }
        [JsonProperty("VoyageItineraries.VoyageItineraryBunkers.OprQty")]
        public double OprQty { get; set; }


    }
}
