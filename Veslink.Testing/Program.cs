using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veslink.Business;

namespace Veslink.Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ReportBusiness reportBusiness = new ReportBusiness();

            DateTime startDate = new DateTime(2021, 1, 1);
            DateTime endDate = new DateTime(2021, 12, 31);
            reportBusiness.GetVessels(startDate, endDate, "UGIN");
            reportBusiness.SetSelectedVessel("GGPA");
            reportBusiness.SetSelectedVoyage(71);
            reportBusiness.SetSelectedCharterer("CARGILL AMERICAS  INC");

            reportBusiness.GenerateExcel("GGPA", "71", "CARGILL AMERICAS  INC", "");
        }
    }
}
