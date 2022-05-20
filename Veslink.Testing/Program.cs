using ClosedXML.Excel;
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
            DateTime endDate = new DateTime(2022, 5, 31);
            reportBusiness.GetVessels(startDate, endDate, "UTNK");
            reportBusiness.SetSelectedVessel("CMAL");
            reportBusiness.SetSelectedVoyage(64);
            reportBusiness.SetSelectedCharterer("ARCHER DANIELS MIDLAND COMPANY");

            byte[] fileGenerated = reportBusiness.GenerateExcel("CMAL", "64", "ARCHER DANIELS MIDLAND COMPANY", "");


            using (System.IO.FileStream fs = new System.IO.FileStream(@"C:\Desarrollos\Veslink\Docs\CMAL_64_ARCHER DANIELS MIDLAND COMPANY_TEST.xlsx",
                     System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                fs.Write(fileGenerated, 0, fileGenerated.Length);

                fs.Close();
            }


            //System.IO.Stream stream = new System.IO.MemoryStream(Properties.Resources.reporttemplate);

            //var workbook = new XLWorkbook(stream);
            //var ws = workbook.Worksheet(1);

            //ws.Cells("F9").Value = "1234567";
            //ws.Cells("F10").Value = "CELSIUS MALAGA";
            ////ws.Cells("F11").Value = "Chemical tanker";
            //ws.Cells("F11").Value = "20835";
            //ws.Cells("F13").Value = "64";

            ////// Change the background color of the headers
            ////var rngHeaders = ws.Range("B3:F3");
            ////rngHeaders.Style.Fill.BackgroundColor = XLColor.LightSalmon;

            ////// Change the date formats
            ////var rngDates = ws.Range("E4:E6");
            ////rngDates.Style.DateFormat.Format = "MM/dd/yyyy";

            ////// Change the income values to text
            ////var rngNumbers = ws.Range("F4:F6");
            ////foreach (var cell in rngNumbers.Cells())
            ////{
            ////    cell.DataType = XLDataType.Text;
            ////    cell.Value += " Dollars";
            ////}

            //workbook.SaveAs("BasicTable_Modified.xlsx");
        }
    }
}
