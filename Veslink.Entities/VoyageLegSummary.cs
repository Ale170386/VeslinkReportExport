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
        [JsonProperty("VoyageLegSummary.Bnkr1_FuelType")]
        public string Bnkr1FuelType { get; set; }

        [JsonProperty("VoyageLegSummary.Bnkr2_FuelType")]
        public string Bnkr2FuelType { get; set; }

        [JsonProperty("VoyageLegSummary.Bnkr3_FuelType")]
        public string Bnkr3FuelType { get; set; }

        [JsonProperty("VoyageLegSummary.Bnkr4_FuelType")]
        public string Bnkr4FuelType { get; set; }

        [JsonProperty("VoyageLegSummary.Bnkr5_FuelType")]
        public string Bnkr5FuelType { get; set; }

        [JsonProperty("VoyageLegSummary.Bnkr6_FuelType")]
        public string Bnkr6FuelType { get; set; }

        [JsonProperty("VoyageLegSummary.Bnkr1_Total")]
        public double Bnkr1Total { get; set; }

        [JsonProperty("VoyageLegSummary.Bnkr2_Total")]
        public double Bnkr2Total { get; set; }

        [JsonProperty("VoyageLegSummary.Bnkr3_Total")]
        public double Bnkr3Total { get; set; }

        [JsonProperty("VoyageLegSummary.Bnkr4_Total")]
        public double Bnkr4Total { get; set; }

        [JsonProperty("VoyageLegSummary.Bnkr5_Total")]
        public double Bnkr5Total { get; set; }

        [JsonProperty("VoyageLegSummary.Bnkr6_Total")]
        public double Bnkr6Total { get; set; }

        [JsonProperty("VoyageLegSummary.FromPortName")]
        public string FromPortName { get; set; }

        [JsonProperty("VoyageLegSummary.FromPortFunc")]
        public string FromPortFunc { get; set; }

        [JsonProperty("VoyageLegSummary.ToPortName")]
        public string ToPortName { get; set; }

        [JsonProperty("VoyageLegSummary.Distance")]
        public double Distance { get; set; }

        [JsonProperty("VoyageLegSummary.FromGMT")]
        public DateTime FromGMT { get; set; }

        [JsonProperty("VoyageLegSummary.FromPortSeq")]
        public int FromPortSeq { get; set; }

        [JsonProperty("VoyageLegSummary.ToGMT")]
        public DateTime ToGMT { get; set; }

    }
}
