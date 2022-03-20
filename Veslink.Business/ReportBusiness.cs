using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veslink.Data;
using Veslink.Entities;
using IronXL;

namespace Veslink.Business
{
    public class ReportBusiness
    {
        public List<Vessel> VesselsDB { get; set; }
        public Vessel VesselSelected { get; set; }

        public List<string> GetCompanies()
        {
            return ReportData.GetCompanies();
        }

        public void GetVessels(DateTime startDate, DateTime endDate, string company)
        {
            List<Root> reportData = ReportData.GetVesselReportAsync(startDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                                                                    endDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                                                                    company).GetAwaiter().GetResult();

            this.VesselsDB = ParseObjectToVessels(reportData);
        }

        private List<Vessel> ParseObjectToVessels(List<Root> roots)
        {
            List<Vessel> vessels = new List<Vessel>();
            foreach (var root in roots.GroupBy(g => new { g.fields.VesselCode, g.fields.VesselName }))
            {
                vessels.Add(new Vessel()
                {
                    VesselCode = root.Key.VesselCode,
                    VesselName = root.Key.VesselName,
                    VesselType = root.FirstOrDefault().dataSources[0].fields.VesselType,
                    IMONo = root.FirstOrDefault().dataSources[0].fields.IMONo,
                    Size = root.FirstOrDefault().dataSources[0].fields.DWT,
                    Voyages = root
                              .OrderBy(o => o.fields.VoyageNo)
                              .Select(x => 
                                        new Voyage() 
                                        {
                                            VoyageNo = x.fields.VoyageNo, 
                                            CommenceDateGmt = x.fields.CommenceDateGmt,
                                            Charterers = x.dataSources[1].values
                                                        .GroupBy(g => g.dataSources.FirstOrDefault().fields.FullName)
                                                        .Select(c =>
                                                                new Charterer()
                                                                {
                                                                    Cargos = c.Select(s => new Cargo()
                                                                    {
                                                                        CargoID = s.fields.CargoID,
                                                                        CargoShortName = s.fields.CargoShortName
                                                                    }).ToList(),
                                                                    ChartererName = c.Key
                                                                }).ToList()
                                        }).ToList()
                });
            }

            return vessels;
        }

        public void SetSelectedVessel(string vesselCode)
        {
            if (!String.IsNullOrEmpty(vesselCode))
                this.VesselSelected = this.VesselsDB.FirstOrDefault(s => s.VesselCode == vesselCode);
        }

        public void SetSelectedVoyage(int voyageNumber)
        {
            this.VesselSelected.VoyageSelected = this.VesselSelected.Voyages.FirstOrDefault(s => s.VoyageNo == voyageNumber);
        }

        public void SetSelectedCharterer(string chartererSelected)
        {
            this.VesselSelected.VoyageSelected.ChartererSelected = this.VesselSelected.VoyageSelected.Charterers
                                                                    .FirstOrDefault(s => s.ChartererName == chartererSelected);
        }

        public void SetSelectedCargo(int cargoID)
        {
            this.VesselSelected
                .VoyageSelected
                .ChartererSelected
                .CargoSelected = this.VesselSelected
                                     .VoyageSelected
                                     .ChartererSelected
                                     .Cargos.FirstOrDefault(s => s.CargoID == cargoID);
        }

        public string GenerateExcel(string vesselCode, string voyageNo)
        {
            Root root = ReportData.GetItinerariesReportAsync(vesselCode, voyageNo);
            CreateLinkedListItinerary(root);
            return CreateExcel();
            //vessel.VoyageLegSummaries = await ReportData.GetVoyageReportAsync();
            //vessel.VoyageCargos = await ReportData.GetCargoReportAsync();


        }

        private void CreateLinkedListItinerary(Root root)
        {
            LinkedListNode<VoyageItinerary> linkedListNode = null;
            LinkedList<VoyageItinerary> itineraries = new LinkedList<VoyageItinerary>();

            foreach (Value value in root.dataSources[0].values.OrderBy(o => o.fields.Seq))
            {
                if (linkedListNode == null)
                {
                    linkedListNode = new LinkedListNode<VoyageItinerary>(new VoyageItinerary()
                    {
                        Seq = value.fields.Seq,
                        FixtureNo = value.fields.FixtureNo,
                        PortFunc = value.fields.PortFunc,
                        Order = value.fields.Order,
                        PortName = value.fields.PortName,
                        EtaGmt = value.fields.EtaGmt,
                        EtdGmt = value.fields.EtdGmt,
                        Miles = value.fields.Miles,
                        LSMiles = value.fields.LSMiles
                    });

                    itineraries.AddLast(linkedListNode);
                }
                else
                {
                    itineraries.AddLast(new VoyageItinerary()
                    {
                        Seq = value.fields.Seq,
                        FixtureNo = value.fields.FixtureNo,
                        PortFunc = value.fields.PortFunc,
                        Order = value.fields.Order,
                        PortName = value.fields.PortName,
                        EtaGmt = value.fields.EtaGmt,
                        EtdGmt = value.fields.EtdGmt,
                        Miles = value.fields.Miles,
                        LSMiles = value.fields.LSMiles
                    });
                }                
            }

            this.VesselSelected.VoyageSelected.VoyageItineraries = linkedListNode;
        }

        public string CreateExcel()
        {            
            WorkBook workbook = WorkBook.Load(Properties.Resources.ReportTemplate);
            WorkSheet sheet = workbook.DefaultWorkSheet;

            #region Vessel And Cargo Data

            sheet["C9"].Value = this.VesselSelected.IMONo;
            sheet["C10"].Value = this.VesselSelected.VesselName;
            sheet["C11"].Value = this.VesselSelected.VesselType;
            sheet["C12"].Value = this.VesselSelected.Size;
            sheet["C13"].Value = this.VesselSelected.VoyageSelected.VoyageNo;
            #endregion

            LinkedListNode<VoyageItinerary> loadPort = null;
            LinkedListNode<VoyageItinerary> commencedPort = null;

            #region Last Ballast Leg            
            GetPort(this.VesselSelected.VoyageSelected.VoyageItineraries, "L", ref loadPort);
            GetPort(loadPort.Previous, "C", ref commencedPort);

            int distance = GetDistanceSailed(commencedPort, loadPort);

            sheet["C17"].Value = commencedPort.Value.PortName;
            sheet["C18"].Value = loadPort.Value.PortName;
            sheet["C19"].Value = commencedPort.Value.EtdGmt;
            sheet["C20"].Value = loadPort.Previous != null && loadPort.Previous.Value.PortFunc == "W" ? loadPort.Previous.Value.EtdGmt : loadPort.Value.EtdGmt;
            sheet["C21"].Value = distance;
            #endregion

            LinkedListNode<VoyageItinerary> dischargePort = null;
            GetPort(loadPort, "D", ref dischargePort);

            #region Loaded Leg
            distance = GetDistanceSailed(loadPort.Next, dischargePort);

            sheet["C25"].Value = loadPort.Value.PortName;
            sheet["C26"].Value = dischargePort.Value.PortName;
            sheet["C27"].Value = loadPort.Value.EtaGmt;
            sheet["C28"].Value = dischargePort.Value.EtdGmt;
            sheet["C29"].Value = distance;

            #endregion

            string fileName = $"VesselReport_{DateTime.Now:yyyyMMddhhmmss}.xlsx";
            workbook.SaveAs(fileName);
            return fileName;
        }

        /// <summary>
        /// Busca puerto de Carga
        /// </summary>
        /// <param name="itinerary"></param>
        /// <param name="loadPort"></param>
        private void GetPort(LinkedListNode<VoyageItinerary> itinerary, string portType, ref LinkedListNode<VoyageItinerary> resultPort)
        {
            switch (portType)
            {
                case "L":

                    if (itinerary != null && itinerary.Value.PortFunc != "L")
                        GetPort(itinerary.Next, portType, ref resultPort);
                    else
                        resultPort = itinerary;

                    break;

                case "C":

                    if (itinerary != null && itinerary.Value.PortFunc == "W")
                        GetPort(itinerary.Previous, portType, ref resultPort);
                    else
                        resultPort = itinerary;

                    break;
                case "D":

                    if (itinerary != null)
                    {
                        if (itinerary.Value.PortFunc == "D")
                            resultPort = itinerary;

                        GetPort(itinerary.Next, portType, ref resultPort);
                    }

                    break;
            }
        }
        
        /// <summary>
        /// Suma las distancias entre un puerto a otro
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="endPort"></param>
        /// <returns></returns>
        private int GetDistanceSailed(LinkedListNode<VoyageItinerary> startPort, LinkedListNode<VoyageItinerary> endPort)
        {
            int distance = 0;
            var currentNode = startPort;
            while ((currentNode != null) && (currentNode != endPort))
            {
                distance += currentNode.Value.Miles;

                if (currentNode.Next == endPort)
                    distance += currentNode.Next.Value.Miles;

                currentNode = currentNode.Next;
            }

            return distance;
        }
    }
}
