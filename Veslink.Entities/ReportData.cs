using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veslink.Entities
{
    public class Fields
    {
        public string VesselCode { get; set; }
        public int VoyageNo { get; set; }
        public string VesselName { get; set; }
        public DateTime CommenceDateGmt { get; set; }
        public string IMONo { get; set; }
        public string CompanyCode { get; set; }
        public string VesselType { get; set; }
        public double DWT { get; set; }
        public int CargoID { get; set; }
        public string CargoShortName { get; set; }
        public string ShortName { get; set; }        
        public int LSMiles { get; set; }
        public int Seq { get; set; }
        public string FixtureNo { get; set; }
        public string PortFunc { get; set; }
        public int Order { get; set; }
        public string PortName { get; set; }
        public DateTime EtdGmt { get; set; }
        public int Miles { get; set; }
        public DateTime? EtaGmt { get; set; }
        public string EstimateID { get; set; }
        public double CPQty { get; set; }
        public string CounterpartyShortName { get; set; }
        public int VoyageSeqNo { get; set; }
        public string FunctionCode { get; set; }
        public double BLQuantity { get; set; }
    }

    public class DataSource2
    {
        public string id { get; set; }
        public string joinType { get; set; }
        public Fields fields { get; set; }
        public List<Value> values { get; set; }
    }

    public class Value
    {
        public Fields fields { get; set; }
        public List<DataSource2> dataSources { get; set; }
    }

    public class Root
    {
        public string id { get; set; }
        public Fields fields { get; set; }
        public List<DataSource2> dataSources { get; set; }
    }
}
