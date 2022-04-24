using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veslink.Entities
{
    public class ChartererItinerary
    {
        [JsonProperty("VoyageCargoHandlings.VoyageItinerary.PortFunc")]
        public string PortFunc { get; set; }

        [JsonProperty("VoyageCargoHandlings.VoyageItineraryOrderNo")]
        public int Order { get; set; }

        [JsonProperty("VoyageCargoHandlings.Port.Name")]
        public string PortName { get; set; }

        [JsonProperty("VoyageCargoHandlings.VoyageItinerary.EtaGmt")]
        public DateTime? EtaGmt { get; set; }

        [JsonProperty("VoyageCargoHandlings.VoyageItinerary.EtdGmt")]
        public DateTime? EtdGmt { get; set; }

        [JsonProperty("VoyageCargoHandlings.VoyageItinerary.Miles")]
        public int Miles { get; set; }

        [JsonProperty("VoyageCargoHandlings.SeqNo")]
        public int Seq { get; set; }

        [JsonProperty("VoyageCargoHandlings.Cargo.CounterpartyShortName")]
        public string CounterpartyShortName { get; set; }

        [JsonProperty("VoyageCargoHandlings.CargoID")]
        public int CargoID { get; set; }

        [JsonProperty("VoyageCargoHandlings.VoyageItinerary.PortNo")]
        public int PortNo { get; set; }
    }
}
