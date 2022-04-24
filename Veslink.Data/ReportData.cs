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
        static string uriChartererItinerary = ConfigurationManager.AppSettings["uriChartererItinerary"];
        static string uriVoyage = ConfigurationManager.AppSettings["uriVoyage"];
        static string uriCargo = ConfigurationManager.AppSettings["uriCargo"];
        static string uriContact = ConfigurationManager.AppSettings["uriContact"];

        public static List<string> GetCompanies()
        {
            return ConfigurationManager.AppSettings["companies"].Split(';').ToList();
        }

        //Report 0
        public static List<Root> GetVesselReportAsync(string startDate, string endDate, string company)
        {
            List<Root> roots = null;
            string uri = $"{uriVessels}&filter[0]=CompanyCode==%22{company}%22&filter[1]=CommenceDateGmt%3E=%22{startDate}%22&filter[2]=CommenceDateGmt%3C=%22{endDate}%22&format=doc";
            HttpResponseMessage response = client.GetAsync(uri).ConfigureAwait(false).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
                roots = response.Content.ReadAsAsync<List<Root>>().GetAwaiter().GetResult();

            return roots;
        }

        //Report 1
        public static List<VoyageItinerary> GetItinerariesReportAsync(string vesselCode, string voyageNo)
        {
            List<VoyageItinerary> itineraries = null;
            ;
            string uri = $"{uriVoyageItineraries}&filter[0]=VesselCode==%22{vesselCode}%22&filter[1]=VoyageNo==%22{voyageNo}%22";

            HttpResponseMessage response = client.GetAsync($"{uri}&format=json").GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
                itineraries = response.Content.ReadAsAsync<List<VoyageItinerary>>().GetAwaiter().GetResult();

            itineraries = itineraries
                            .Where(w => w.PortFunc != "P")
                            .GroupBy(g => new
                                            {
                                                g.Order,                                                
                                                g.EtaGmt,
                                                g.PortName,
                                                g.PortNo,
                                                g.PortFunc,
                                                g.Miles,                                                
                                                g.EtdGmt                                                
                                            }).Select(s => new VoyageItinerary()
                                            {
                                                Order = s.Key.Order,                                                
                                                EtaGmt = s.Key.EtaGmt,
                                                PortName = s.Key.PortName,
                                                PortNo = s.Key.PortNo,
                                                PortFunc = s.Key.PortFunc,
                                                Miles = s.Key.Miles,
                                                EtdGmt = s.Key.EtdGmt                                                
                                            }).ToList();

            return itineraries.OrderBy(o => o.Order).ToList();
        }

        //Report 1_V2
        public static List<ChartererItinerary> GetChartererItineraryAsync(string vesselCode, string voyageNo, string chartererId = "", string cargoId = "")
        {
            List<ChartererItinerary> itineraries = null;
            ;
            string uri = $"{uriChartererItinerary}&filter[0]=VesselCode==%22{vesselCode}%22&filter[1]=VoyageNo==%22{voyageNo}%22";

            if (!(String.IsNullOrEmpty(chartererId)))
                uri += $"&filter[2]=VoyageCargoHandlings.Cargo.Counterparty.ShortName==%22{chartererId.Replace("&", "%26")}%22";

            if (!(String.IsNullOrEmpty(cargoId)))
                uri += $"&filter[3]=VoyageCargoHandlings.CargoID==%22{cargoId}%22";

            HttpResponseMessage response = client.GetAsync($"{uri}&format=json").GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
                itineraries = response.Content.ReadAsAsync<List<ChartererItinerary>>().GetAwaiter().GetResult();

            itineraries = itineraries
                            .Where(w => w.PortFunc != "P")
                            .GroupBy(g => new
                            {
                                g.Order,
                                g.EtaGmt,
                                g.PortName,
                                g.PortNo,
                                g.PortFunc,
                                g.Miles,
                                g.EtdGmt,
                                g.CounterpartyShortName,
                                g.CargoID
                            }).Select(s => new ChartererItinerary()
                            {
                                Order = s.Key.Order,
                                EtaGmt = s.Key.EtaGmt,
                                PortName = s.Key.PortName,
                                PortNo = s.Key.PortNo,
                                PortFunc = s.Key.PortFunc,
                                Miles = s.Key.Miles,
                                EtdGmt = s.Key.EtdGmt,
                                CounterpartyShortName = s.Key.CounterpartyShortName,
                                CargoID = s.Key.CargoID
                            }).ToList();

            return itineraries.OrderBy(o => o.Order).ToList();
        }

        //Report 4
        public static List<VoyageCargo> GetCargoReportAsync(string vesselCode, string voyageNo, string chartererId, string cargoId)
        {
            List <VoyageCargo> cargoList = null;
            //string uri = $"{uriCargo}&filter[0]=VesselCode==%22{vesselCode}%22&filter[1]=VoyageNo==%22{voyageNo}%22&filter[2]=VoyageCargoHandlings.Cargo.Counterparty.ShortName==%22{chartererId}%22";

            //if (!(String.IsNullOrEmpty(cargoId)))
            //    uri += $"&filter[3]=VoyageCargoHandlings.CargoID==%22{cargoId}%22";

            string uri = $"{uriCargo}&filter[0]=VesselCode==%22{vesselCode}%22&filter[1]=VoyageNo==%22{voyageNo}%22";

            HttpResponseMessage response = client.GetAsync($"{uri}&format=json").GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
                cargoList = response.Content.ReadAsAsync<List<VoyageCargo>>().GetAwaiter().GetResult();

            return cargoList;
        }

        //Report 3
        public static List<VoyageLegSummary> GetVoyageSummaryReportAsync(string vesselCode, string voyageNo, string chartererId, string cargoId)
        {
            List<VoyageLegSummary> voyage = null;

            string uri = $"{uriVoyage}&filter[0]=VesselCode==%22{vesselCode}%22&filter[1]=VoyageNo==%22{voyageNo}%22&filter[2]=Cargos.CounterpartyShortName==%22{chartererId.Replace("&", "%26")}%22";

            if (!(String.IsNullOrEmpty(cargoId)))
                uri += $"&filter[3]=Cargos.CargoID==%22{cargoId}%22";

            HttpResponseMessage response = client.GetAsync($"{uri}&format=json").GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
                voyage = response.Content.ReadAsAsync<List<VoyageLegSummary>>().GetAwaiter().GetResult();

            var summary = voyage.GroupBy(g => new
            {
                g.Bnkr1FuelType,
                g.Bnkr2FuelType,
                g.Bnkr3FuelType,
                g.Bnkr4FuelType,
                g.Bnkr5FuelType,
                g.Bnkr6FuelType,
                g.Bnkr1Total,
                g.Bnkr2Total,
                g.Bnkr3Total,
                g.Bnkr4Total,
                g.Bnkr5Total,
                g.Bnkr6Total,
                g.FromPortName,
                g.FromPortSeq,
                g.FromPortFunc,
                g.ToPortName,
                g.Distance,
                g.FromGMT,
                g.ToGMT
            });

            return summary.Select(s => new VoyageLegSummary()
            {
                Bnkr1FuelType = s.Key.Bnkr1FuelType.ToString(),
                Bnkr2FuelType = s.Key.Bnkr2FuelType.ToString(),
                Bnkr3FuelType = s.Key.Bnkr3FuelType.ToString(),
                Bnkr4FuelType = s.Key.Bnkr4FuelType.ToString(),
                Bnkr5FuelType = s.Key.Bnkr5FuelType.ToString(),
                Bnkr6FuelType = s.Key.Bnkr6FuelType.ToString(),
                Bnkr1Total = s.Key.Bnkr1Total,
                Bnkr2Total = s.Key.Bnkr2Total,
                Bnkr3Total = s.Key.Bnkr3Total,
                Bnkr4Total = s.Key.Bnkr4Total,
                Bnkr5Total = s.Key.Bnkr5Total,
                Bnkr6Total = s.Key.Bnkr6Total,
                FromPortName = s.Key.FromPortName,
                FromPortSeq = s.Key.FromPortSeq,
                ToPortName = s.Key.ToPortName,
                Distance = s.Key.Distance,
                FromGMT = s.Key.FromGMT,
                ToGMT = s.Key.ToGMT
            }).OrderBy(o => o.FromGMT).ToList();

        }

        //Report 5
        public static ContactInformation GetVoyageContactAsync(string vesselCode, string voyageNo)
        {
            ContactInformation contactInformation = null;
            string uri = $"{uriContact}&filter[0]=VesselCode==%22{vesselCode}%22&filter[1]=VoyageNo==%22{voyageNo}%22";
            HttpResponseMessage response = client.GetAsync($"{uri}&format=json").ConfigureAwait(false).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
                contactInformation = response.Content.ReadAsAsync<List<ContactInformation>>().GetAwaiter().GetResult().FirstOrDefault();

            return contactInformation;
        }
    }
}
