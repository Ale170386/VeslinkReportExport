using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veslink.Entities
{
    public class Vessel
    {
        public Vessel()
        {
        }        

        public string VesselCode { get; set; }
        public string VesselName { get; set; }
        public string IMONo { get; set; }
        public double Size { get; set; }
        public string VesselType { get; set; }
        public Voyage VoyageSelected { get; set; }
        public List<Voyage> Voyages { get; set; }
    }
}
