using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Veslink.Entities;

namespace Veslink.Data
{
    public class ReportData
    {
        static HttpClient client = new HttpClient();
        static string uriVessels = ConfigurationManager.AppSettings["uriVessels"];
        static string uriVoyageItineraries = ConfigurationManager.AppSettings["uriVoyageItineraries"];
        static string uriVoyage = ConfigurationManager.AppSettings["uriVoyage"];
        static string uriCargo = ConfigurationManager.AppSettings["uriCargo"];

        public static List<string> GetCompanies()
        {
            return ConfigurationManager.AppSettings["companies"].Split(';').ToList();
        }

        //Report 0
        public static async Task<List<Root>> GetVesselReportAsync(string startDate, string endDate, string company)
        {
            List<Root> roots = null;
            string uri = $"{uriVessels}&filter[0]=Vessel.CompanyCode==%22{company}%22&filter[1]=CommenceDateGmt%3E=%22{startDate}%22&filter[2]=CommenceDateGmt%3C=%22{endDate}%22&format=doc";
            HttpResponseMessage response = await client.GetAsync(uri).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
                roots = await response.Content.ReadAsAsync<List<Root>>();

            return roots;
        }

        //Report 1
        public static Root GetItinerariesReportAsync(string vesselCode, string voyageNo)
        {
            Root root = null;
            string uri = $"{uriVoyageItineraries}&filter[0]=VesselCode==%22{vesselCode}%22&filter[1]=VoyageNo==%22{voyageNo}%22&format=doc";
            HttpResponseMessage response = client.GetAsync(uri).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
                root = response.Content.ReadAsAsync<List<Root>>().GetAwaiter().GetResult().FirstOrDefault();

            return root;
        }

        //Report 3
        public static async Task<List<VoyageLegSummary>> GetVoyageReportAsync()
        {
            List<VoyageLegSummary> voyage = null;
            HttpResponseMessage response = await client.GetAsync($"{uriVoyage}&format=json");
            if (response.IsSuccessStatusCode)
                voyage = await response.Content.ReadAsAsync<List<VoyageLegSummary>>();

            return voyage;
        }

        //Report 4
        public static async Task<List<VoyageCargo>> GetCargoReportAsync()
        {
            List<VoyageCargo> voyage = null;
            HttpResponseMessage response = await client.GetAsync($"{uriCargo}&format=json");
            if (response.IsSuccessStatusCode)
                voyage = await response.Content.ReadAsAsync<List<VoyageCargo>>();

            return voyage;
        }
    }
}
