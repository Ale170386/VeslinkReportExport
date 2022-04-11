using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veslink.Entities
{
    public class VoyageCargo
    {
        [JsonProperty("VoyageCargoHandlings.BLQuantity")]
        public double BLQuantity { get; set; }

        [JsonProperty("VoyageCargoHandlings.CPUnit")]
        public string CPUnit { get; set; }
        [JsonProperty("VoyageCargoHandlings.Cargo.Counterparty.ShortName")]
        public string CounterpartyShortName { get; set; }

        [JsonProperty("VoyageCargoHandlings.PortNo")]
        public int PortNo { get; set; }

        [JsonProperty("VoyageCargoHandlings.Port.Name")]
        public string PortName { get; set; }

        [JsonProperty("VoyageCargoHandlings.CargoID")]
        public int CargoID { get; set; }                

        [JsonProperty("VoyageCargoHandlings.OrdNo")]
        public int OrdNo { get; set; }

        [JsonProperty("VoyageCargoHandlings.SeqNo")]
        public int SeqNo { get; set; }

        [JsonProperty("VoyageCargoHandlings.VoyageSeqNo")]
        public int VoyageSeqNo { get; set; }

        [JsonProperty("VoyageCargoHandlings.FunctionCode")]
        public string FunctionCode { get; set; }
        [JsonProperty("VoyageCargoHandlings.VoyageItinerary.EtaGmt")]
        public DateTime EtaGmt { get; set; }
    }
}
