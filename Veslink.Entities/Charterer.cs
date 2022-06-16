using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veslink.Entities
{
    public class Charterer
    {
        public string ChartererName { get; set; }
        public Cargo CargoSelected { get; set; }
        public List<Cargo> Cargos { get; set; }
        public List<VoyageCargo> VoyageCargos { get; set; }
        public List<VoyageLegSummary> VoyageLegSummaries { get; set; }
    }
}
