using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veslink.Data;
using Veslink.Entities;

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
                                                                    company);

            this.VesselsDB = ParseObjectToVessels(reportData);
        }

        private List<Vessel> ParseObjectToVessels(List<Root> roots)
        {
            List<Vessel> vessels = new List<Vessel>();
            foreach (var root in roots.OrderBy(o => o.fields.VesselName).GroupBy(g => new { g.fields.VesselCode, g.fields.VesselName }))
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
                                                        .OrderBy(o => o.dataSources.FirstOrDefault().fields.ShortName)
                                                        .GroupBy(g => g.dataSources.FirstOrDefault().fields.ShortName)
                                                        .Select(c =>
                                                                new Charterer()
                                                                {
                                                                    Cargos = 
                                                                    c.OrderBy(o => o.fields.CargoShortName)
                                                                    .Select(s => new Cargo()
                                                                    {
                                                                        CargoID = s.fields.CargoID,
                                                                        CargoShortName = s.fields.CargoShortName,
                                                                        CargoQty = s.fields.CPQty
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

        public byte[] GenerateExcel(string vesselCode, string voyageNo, string chartererId, string cargoId)
        {
            //Get and Parse Itinerary from API CHEM 1
            this.VesselSelected.VoyageSelected.Itinerary = ReportData.GetItinerariesReportAsync(vesselCode, voyageNo);
            this.VesselSelected.VoyageSelected.ChartererSelected.ChartererItinerary = ReportData.GetChartererItineraryAsync(vesselCode, voyageNo, chartererId, cargoId);


            //this.VesselSelected.VoyageSelected.ChartererSelected.VoyageItineraries = ReportData.GetItinerariesReportAsync(vesselCode, voyageNo, chartererId, cargoId);

            //Get and Parse Cargo detail from API CHEM 4
            this.VesselSelected.VoyageSelected.VoyageCargos = ReportData.GetCargoReportAsync(vesselCode, voyageNo, chartererId, cargoId);
            this.VesselSelected.VoyageSelected.ChartererSelected.VoyageCargos = this.VesselSelected.VoyageSelected.VoyageCargos
                                                                                .Where(s => s.CounterpartyShortName == chartererId
                                                                                            && (cargoId == "" || s.CargoID == int.Parse(cargoId))).ToList();

            //Get and Parse Voyage Summary from API CHEM 3
            this.VesselSelected.VoyageSelected.ChartererSelected.VoyageLegSummaries = ReportData.GetVoyageSummaryReportAsync(vesselCode, voyageNo, chartererId, cargoId);

            //Get contact information from API CHEM 5
            this.VesselSelected.VoyageSelected.ContactInformation = ReportData.GetVoyageContactAsync(vesselCode, voyageNo);

            if (this.VesselSelected.VoyageSelected.ChartererSelected.ChartererItinerary != null && this.VesselSelected.VoyageSelected.ChartererSelected.ChartererItinerary.Count > 0)
                return CreateExcel();
            else
                return null;
            //vessel.VoyageLegSummaries = await ReportData.GetVoyageReportAsync();
            //vessel.VoyageCargos = await ReportData.GetCargoReportAsync();


        }

        public byte[] CreateExcel()
        {
            byte[] result = null;
            XLWorkbook workbook;
            CultureInfo ci = CultureInfo.InvariantCulture;
            // This is invariant
            NumberFormatInfo formatInfo = new NumberFormatInfo();
            // Set the 'splitter' for thousands
            formatInfo.NumberGroupSeparator = ",";
            // Set the decimal seperator
            formatInfo.NumberDecimalSeparator = ".";
            MemoryStream stream = new MemoryStream(Properties.Resources.ReportTemplate);
            workbook = new XLWorkbook(stream);

            //Assign the sheet
            var sheet = workbook.Worksheet(1);

            #region Header

            #region Vessel And Cargo Data
            sheet.Cell("F9").Value = VesselSelected.IMONo.ToString();
            sheet.Cell("F10").Value = VesselSelected.VesselName.ToString();
            sheet.Cell("F11").Value = VesselSelected.VesselType.ToString();
            sheet.Cell("F12").Value = VesselSelected.Size;
            sheet.Cell("F14").Value = VesselSelected.VoyageSelected.VoyageNo.ToString();
            #endregion

            List<VoyageItinerary> voyageItinerary = this.VesselSelected.VoyageSelected.Itinerary;

            #region Preceding Ballast Leg            

            int commencedPort = 0;//El puerto de inicio del ballast es el primer puerto del itinerario
            int loadPort = voyageItinerary.FindIndex(i => i.PortFunc == "L");

            int distance = GetDistanceSailed(commencedPort, loadPort);

            sheet.Cell("F18").Value = voyageItinerary[commencedPort].PortName.ToString();
            sheet.Cell("F20").Value = voyageItinerary[loadPort].PortName.ToString();

            //Ballast Start Date
            DateTime ballastStartDate = (DateTime)voyageItinerary[commencedPort].EtdGmt;
                        
            sheet.Cell("F22").Value = ballastStartDate.ToString("dd/MM/yyyy");
            sheet.Cell("F23").Value = ballastStartDate.ToString("hh:mm");

            //Ballast End Date
            DateTime ballastEndDate = (DateTime)((loadPort - 1 >= 0 && voyageItinerary[loadPort - 1].PortFunc == "W") ?
                                                                     voyageItinerary[loadPort - 1].EtdGmt :
                                                                     voyageItinerary[loadPort].EtdGmt);

            sheet.Cell("F24").Value = ballastEndDate.ToString("dd/MM/yyyy");
            sheet.Cell("F25").Value = ballastEndDate.ToString("hh:mm");

            //Distancia
            sheet.Cell("F26").Value = Double.Parse(distance.ToString(ci), formatInfo);
            #endregion

            #region Laden Leg

            int dischargePort = voyageItinerary.FindLastIndex(i => i.PortFunc == "D");

            sheet.Cell("F32").Value = voyageItinerary[dischargePort].PortName.ToString();

            //Fecha de inicio del viaje, es la misma que la del primer puerto de carga
            sheet.Cell("F34").Value = sheet.Cell("F24").Value;
            sheet.Cell("F35").Value = sheet.Cell("F25").Value;

            //Fecha del ultimo puerto de descarga del viaje
            DateTime voyageEndDate = (DateTime)voyageItinerary[dischargePort].EtdGmt;
            sheet.Cell("F36").Value = voyageEndDate.ToString("dd/MM/yyyy");
            sheet.Cell("F37").Value = voyageEndDate.ToString("hh:mm");

            #endregion

            #endregion

            #region Cargo Quantities, Distances, and Consumption

            #region Variables para calcular

            int row = 45;
            VoyageItinerary nextPort = null;            
            int cargoQuantity = 0;
            int charterCargoQuantity = 0;
            List<VoyageItinerary> cargoItinerary = voyageItinerary.SkipWhile((i, index) => index < loadPort).ToList();
            int lastPort = cargoItinerary.Count - 1;
            List<Fuel> totalConsumedList = ConfigurationManager.AppSettings["Fuel"]
                                            .Split(';')
                                            .Select(s => new Fuel()
                                            {
                                                FuelType = s,
                                                Consumed = 0
                                            }).ToList();
            #endregion

            #region Ballast Leg

            //Calculo de consumos de Ballast
            List<Fuel> consumed = GetFuelConsumed(voyageItinerary[commencedPort], voyageItinerary[loadPort]);
            int col = 10;
            sheet.Cell("C44").Value = 2;//Establece check para fila de ballast

            foreach (Fuel fuel in consumed)
            {
                sheet.Column(col).Cell(44).Value = Double.Parse(fuel.Consumed.ToString(ci), formatInfo);
                col++;
            }

            #endregion

            #region Detalle de Puertos

            //Recorre itinerario sin considerar ballast leg
            foreach (var itinerary in cargoItinerary.Select((element, index) => new { element, index }))
            {
                //Genera dos filas en excel por cada PUERTO
                do
                {
                    #region Itinerario y distancia

                    //No realiza nada para las filas de Port Stay (Filas impares)
                    //Registra los itinerarios de las filas Port (Filas pares)
                    if (row % 2 == 0)
                    {
                        nextPort = itinerary.index < lastPort ? cargoItinerary[itinerary.index + 1] : null;

                        if (nextPort != null)
                        {
                            sheet.Column("D").Cell(row).Value = itinerary.element.PortName;
                            sheet.Column("F").Cell(row).Value = nextPort.PortName;
                            sheet.Column("G").Cell(row).Value = nextPort.Miles;
                        }
                    }

                    #endregion

                    #region Cálculo de Cargos

                    //Si la fila es Port Stay (Fila impar) se suma lo que se carga en el puerto
                    if (row % 2 != 0)
                    {
                        if (itinerary.element.PortFunc == "L")
                        {
                            cargoQuantity += Convert.ToInt32(this.VesselSelected.VoyageSelected.VoyageCargos
                                                            .Where(w => w.FunctionCode == itinerary.element.PortFunc
                                                                        && w.PortNo == itinerary.element.PortNo
                                                                        && w.VoyageSeqNo == itinerary.element.Seq)
                                                            .Sum(s => s.BLQuantity));

                            charterCargoQuantity += Convert.ToInt32(this.VesselSelected.VoyageSelected.ChartererSelected.VoyageCargos
                                                                    .Where(w => w.FunctionCode == itinerary.element.PortFunc
                                                                                && w.PortNo == itinerary.element.PortNo
                                                                                && w.VoyageSeqNo == itinerary.element.Seq)
                                                                    .Sum(s => s.BLQuantity));
                        }
                    }
                    //Si la fila es Leg (Fila par) se resta la descarga
                    else
                    {
                        if (itinerary.element.PortFunc == "D")
                        {
                            cargoQuantity -= Convert.ToInt32(this.VesselSelected.VoyageSelected.VoyageCargos
                                                            .Where(w => w.FunctionCode == itinerary.element.PortFunc
                                                                        && w.PortNo == itinerary.element.PortNo
                                                                        && w.VoyageSeqNo == itinerary.element.Seq)
                                                            .Sum(s => s.BLQuantity));

                            charterCargoQuantity -= Convert.ToInt32(this.VesselSelected.VoyageSelected.ChartererSelected.VoyageCargos
                                                                    .Where(w => w.FunctionCode == itinerary.element.PortFunc
                                                                                && w.PortNo == itinerary.element.PortNo
                                                                                && w.VoyageSeqNo == itinerary.element.Seq)
                                                                    .Sum(s => s.BLQuantity));
                        }
                    }

                    if (row == 45 || nextPort != null)
                    {
                        sheet.Column("H").Cell(row).Value = cargoQuantity;
                        sheet.Column("I").Cell(row).Value = charterCargoQuantity;
                    }

                    #endregion

                    #region Cálculo de Consumos

                    if (row == 45 || nextPort != null)
                    {
                        VoyageItinerary endPort = nextPort != null ? nextPort : itinerary.element;
                        consumed = GetFuelConsumed(itinerary.element, endPort);
                        col = 10;
                        foreach (Fuel fuel in consumed)
                        {
                            sheet.Column(col).Cell(row).Value = Double.Parse(fuel.Consumed.ToString(ci), formatInfo);
                            col++;
                        }
                    }

                    #endregion

                    if (row == 45 || nextPort != null)
                        //Setea check de validación de fila
                        sheet.Column("C").Cell(row).Value = 2;

                    row++;
                } while (row % 2 == 0);
                                
            }

            #endregion

            #endregion

            #region Contact Information
            if (this.VesselSelected.VoyageSelected.ContactInformation != null)
            {
                sheet.Cell("E71").Value = 2;//Check de validación de información de contacto
                sheet.Cell("G72").Value = this.VesselSelected.VoyageSelected.ContactInformation.UserFullName.ToString();
                sheet.Cell("G73").Value = this.VesselSelected.VoyageSelected.ContactInformation.UserEmail.ToString();
                sheet.Cell("G74").Value = "Yes";
            }
            #endregion

            #region Escribe archivo

            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms, true, true);
                result = ms.ToArray();
            }

            stream.Close();

            #endregion

            return result;
        }                

        /// <summary>
        /// Suma las distancias entre un puerto a otro
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="endPort"></param>
        /// <returns></returns>
        private int GetDistanceSailed(int startPort, int endPort, string portType = "")
        {
            int distance = 0;            
            if (startPort >= 0 && endPort >= 0 && startPort <= endPort)
            {
                VoyageItinerary sPortNode = this.VesselSelected.VoyageSelected.Itinerary[startPort];
                VoyageItinerary ePortNode = this.VesselSelected.VoyageSelected.Itinerary[endPort];
                var ports = this.VesselSelected.VoyageSelected.Itinerary
                                                .GroupBy(g => new { g.PortName, g.Miles, g.Order })
                                                .OrderBy(o => o.Key.Order)
                                                .ToList();


                int currentNode = ports.FindIndex(i => i.Key.Order == sPortNode.Order);
                int lastNode = ports.FindIndex(i => i.Key.Order == ePortNode.Order);

                int lenght = this.VesselSelected.VoyageSelected.Itinerary.Count;

                if (portType != "" && portType == "L")
                    lastNode--; //No se considera el puerto de carga

                while ((currentNode <= lastNode))
                {
                    distance += ports[currentNode].Key.Miles;

                    //if ((currentNode + 1) == endPort)
                    //    distance += this.VesselSelected.VoyageSelected.ChartererSelected.VoyageItineraries[currentNode + 1].Miles;

                    currentNode++;
                }
            }
            return distance;
        }

        /// <summary>
        /// Calcula el consumo para los distintos tipos de combustibles entre un puerto a otro
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="endPort"></param>
        /// <returns></returns>
        private List<Fuel> GetFuelConsumed(VoyageItinerary startPort, VoyageItinerary endPort)
        {
            List<Fuel> fuelConsumedList = ConfigurationManager.AppSettings["Fuel"]
                                          .Split(';')
                                            .Select(s => new Fuel()
                                            {
                                                FuelType = s,
                                                FuelTypeMap = ConfigurationManager.AppSettings[s] ?? s,
                                                Consumed = 0
                                            }).ToList();

            foreach (Fuel fuel in fuelConsumedList)
            {
                double consumed = 0;

                VoyageLegSummary consumeLegStart = this.VesselSelected.VoyageSelected.ChartererSelected.VoyageLegSummaries
                                                    .Where(f => f.PortNo == startPort.PortNo 
                                                                && f.Seq == startPort.Seq
                                                                && fuel.FuelTypeMap.Contains(f.FuelType))
                                                    .GroupBy(g => g.PortNo)
                                                    .Select(s => new VoyageLegSummary
                                                    {
                                                        PortNo = s.Key,
                                                        RobArrival = s.Sum(sm => sm.RobArrival),
                                                        RobDeparture = s.Sum(sm => sm.RobDeparture),
                                                        OprQty = s.Sum(sm => sm.OprQty)
                                                    }).FirstOrDefault();

                VoyageLegSummary consumeLegEnd = this.VesselSelected.VoyageSelected.ChartererSelected.VoyageLegSummaries
                                                    .Where(f => f.PortNo == endPort.PortNo
                                                                && f.Seq == endPort.Seq
                                                                && fuel.FuelTypeMap.Contains(f.FuelType))
                                                    .GroupBy(g => g.PortNo)
                                                    .Select(s => new VoyageLegSummary
                                                    {
                                                        PortNo = s.Key,
                                                        RobArrival = s.Sum(sm => sm.RobArrival),
                                                        RobDeparture = s.Sum(sm => sm.RobDeparture),
                                                        OprQty = s.Sum(sm => sm.OprQty)
                                                    }).FirstOrDefault();

                if (consumeLegStart != null)
                {
                    if (startPort != endPort)
                        consumed = consumeLegStart.RobDeparture - consumeLegEnd.RobArrival;
                    else
                        consumed = (consumeLegStart.RobArrival - consumeLegStart.RobDeparture) + consumeLegStart.OprQty;
                }
                else
                    consumed = 0;

                fuel.Consumed += consumed;
            }

            return fuelConsumedList;
        }        
    }
}
