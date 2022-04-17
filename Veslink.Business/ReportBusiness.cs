using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.UserModel;
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
            this.VesselSelected.VoyageSelected.ChartererSelected.VoyageItineraries = ReportData.GetItinerariesReportAsync(vesselCode, voyageNo, chartererId, cargoId);

            //Get and Parse Cargo detail from API CHEM 4
            this.VesselSelected.VoyageSelected.VoyageCargos = ReportData.GetCargoReportAsync(vesselCode, voyageNo, chartererId, cargoId);
            this.VesselSelected.VoyageSelected.ChartererSelected.VoyageCargos = this.VesselSelected.VoyageSelected.VoyageCargos
                                                                                .Where(s => s.CounterpartyShortName == chartererId
                                                                                            && (cargoId == "" || s.CargoID == int.Parse(cargoId))).ToList();

            //Get and Parse Voyage Summary from API CHEM 3
            this.VesselSelected.VoyageSelected.ChartererSelected.VoyageLegSummaries = ReportData.GetVoyageSummaryReportAsync(vesselCode, voyageNo, chartererId, cargoId);

            //Get contact information from API CHEM 5
            this.VesselSelected.VoyageSelected.ContactInformation = ReportData.GetVoyageContactAsync(vesselCode, voyageNo);

            if (this.VesselSelected.VoyageSelected.ChartererSelected.VoyageItineraries != null && this.VesselSelected.VoyageSelected.ChartererSelected.VoyageItineraries.Count > 0)
                return CreateExcel();
            else
                return null;
            //vessel.VoyageLegSummaries = await ReportData.GetVoyageReportAsync();
            //vessel.VoyageCargos = await ReportData.GetCargoReportAsync();


        }        

        public byte[] CreateExcel()
        {
            //WorkBook workbook = WorkBook.Load(Properties.Resources.ReportTemplate);
            //WorkSheet sheet = workbook.DefaultWorkSheet;
            CultureInfo ci = CultureInfo.InvariantCulture;            

            // This is invariant
            NumberFormatInfo formatInfo = new NumberFormatInfo();
            // Set the 'splitter' for thousands
            formatInfo.NumberGroupSeparator = ",";
            // Set the decimal seperator
            formatInfo.NumberDecimalSeparator = ".";

            XSSFWorkbook hssfwb;
            using (Stream stream = new MemoryStream(Properties.Resources.ReportTemplate))
            {
                hssfwb = new XSSFWorkbook(stream);
            }

            //Assign the sheet
            ISheet sheet = hssfwb.GetSheetAt(0);

            //ICellStyle intCellStyle = hssfwb.CreateCellStyle();
            //intCellStyle.DataFormat = hssfwb.CreateDataFormat().GetFormat("[>=10000000]##\\,##\\,##\\,##0;[>=100000] ##\\,##\\,##0;##,##0.00");            


            #region Vessel And Cargo Data
            sheet.GetRow(8).GetCell(2).SetCellValue(VesselSelected.IMONo.ToString());
            sheet.GetRow(9).GetCell(2).SetCellValue(VesselSelected.VesselName.ToString());
            sheet.GetRow(10).GetCell(2).SetCellValue(VesselSelected.VesselType.ToString());

            sheet.GetRow(11).GetCell(2).SetCellType(CellType.Numeric);
            sheet.GetRow(11).GetCell(2).SetCellValue(VesselSelected.Size);

            sheet.GetRow(12).GetCell(2).SetCellValue(VesselSelected.VoyageSelected.VoyageNo.ToString());
            #endregion

            int loadPort = 0;
            int commencedPort = 0;
            List<VoyageItinerary> voyageItiners = this.VesselSelected.VoyageSelected.ChartererSelected.VoyageItineraries;

            #region Last Ballast Leg
            GetPort(0, "L", ref loadPort);
            GetPort(loadPort - 1, "C", ref commencedPort);

            int distance = GetDistanceSailed(commencedPort, loadPort);

            sheet.GetRow(16).GetCell(2).SetCellValue(voyageItiners[commencedPort].PortName.ToString());
            sheet.GetRow(17).GetCell(2).SetCellValue(voyageItiners[loadPort].PortName.ToString());
            sheet.GetRow(18).GetCell(2).SetCellValue(voyageItiners[commencedPort].EtdGmt.ToString());
            sheet.GetRow(19).GetCell(2).SetCellValue(((loadPort - 1 >= 0 && voyageItiners[loadPort - 1].PortFunc == "W") ?
                                                                     voyageItiners[loadPort - 1].EtdGmt :
                                                                     voyageItiners[loadPort].EtdGmt).ToString());

            sheet.GetRow(20).GetCell(2).SetCellType(CellType.Numeric);
            sheet.GetRow(20).GetCell(2).SetCellValue(Double.Parse(distance.ToString(ci), formatInfo));
            #endregion

            int dischargePort = 0;
            GetPort(loadPort, "D", ref dischargePort);

            #region Loaded Leg
            distance = GetDistanceSailed(loadPort + 1, dischargePort);

            sheet.GetRow(24).GetCell(2).SetCellValue(voyageItiners[loadPort].PortName.ToString());
            sheet.GetRow(25).GetCell(2).SetCellValue(voyageItiners[dischargePort].PortName.ToString());
            sheet.GetRow(26).GetCell(2).SetCellValue(voyageItiners[loadPort].EtaGmt.ToString());
            sheet.GetRow(27).GetCell(2).SetCellValue(voyageItiners[dischargePort].EtdGmt.ToString());

            sheet.GetRow(28).GetCell(2).SetCellType(CellType.Numeric);
            sheet.GetRow(28).GetCell(2).SetCellValue(Double.Parse(distance.ToString(ci), formatInfo));

            #endregion

            #region Cargo Quantities, Distances, and Consumption

            int row = 35;
            int ballastLeg = 34;
            int currentRow = 0;
            int startIndex = 0;
            int startIndexDistance = 0;
            int endIndex = -1;
            int distanceSailed = 0;
            List<Fuel> totalConsumedList = ConfigurationManager.AppSettings["Fuel"]
                                          .Split(';')
                                            .Select(s => new Fuel()
                                            {
                                                FuelType = s,
                                                Consumed = 0
                                            }).ToList();

            foreach (var voyageCargo in this.VesselSelected.VoyageSelected.ChartererSelected.VoyageCargos
                                           .GroupBy(g => new { g.EtaGmt, g.VoyageSeqNo  g.FunctionCode })
                                           .OrderBy(o => o.Key.EtaGmt)
                                           .ThenByDescending(t => t.Key.FunctionCode))
            {
                if (row % 2 == 0 && voyageCargo.Key.FunctionCode == "L") // Si estoy en la fila de Descarga y el Puerto es Carga avanzo una fila
                    row++;
                else if (row % 2 > 0 && voyageCargo.Key.FunctionCode == "D") //Si estoy en la fila de Carga y el puerto es de Descarga avanzo una fila
                    row++;

                currentRow = row;
                double QuantityAll = Double.Parse(this.VesselSelected.VoyageSelected.VoyageCargos
                                                                                  .Where(w => w.FunctionCode == voyageCargo.Key.FunctionCode
                                                                                            && w.VoyageSeqNo == voyageCargo.Key.VoyageSeqNo)
                                                                                  .Sum(s => s.BLQuantity).ToString(ci), formatInfo);

                //Información de cargos del viaje
                sheet.GetRow(currentRow).GetCell(1).SetCellType(CellType.Numeric);
                sheet.GetRow(currentRow).GetCell(1).SetCellValue(Double.Parse(QuantityAll.ToString(ci), formatInfo));

                //Información de cargos del charterer
                double QuantityCharter = Double.Parse(voyageCargo.Where(w => w.CounterpartyShortName == this.VesselSelected.VoyageSelected.ChartererSelected.ChartererName)
                                    .Sum(s => s.BLQuantity).ToString(ci), formatInfo);
                sheet.GetRow(currentRow).GetCell(2).SetCellType(CellType.Numeric);
                sheet.GetRow(currentRow).GetCell(2).SetCellValue(Double.Parse(QuantityCharter.ToString(ci), formatInfo));

                endIndex = voyageItiners.FindIndex(i => i.Seq == voyageCargo.Key.VoyageSeqNo);

                //%
                if (QuantityAll != 0)
                {
                    sheet.GetRow(currentRow).GetCell(10).SetCellType(CellType.Numeric);
                    sheet.GetRow(currentRow).GetCell(10).SetCellValue(QuantityCharter / QuantityAll);
                }

                if ((row - 1) == ballastLeg)
                {
                    GetPort(endIndex - 1, "C", ref startIndex);
                    currentRow = ballastLeg;
                    startIndexDistance = startIndex;
                }

                //Busca distancia
                distanceSailed = GetDistanceSailed(startIndexDistance, endIndex);
                //Llena distancia en columna 3(D)
                sheet.GetRow(currentRow).GetCell(3).SetCellType(CellType.Numeric);
                sheet.GetRow(currentRow).GetCell(3).SetCellValue(Double.Parse(distanceSailed.ToString(ci), formatInfo));


                //Busca consumo
                List<Fuel> consumed = GetFuelConsumed(startIndex, endIndex, voyageCargo.Key.FunctionCode);

                //LLena consumos desde columna 4(E)
                int cell = 4;
                foreach (Fuel fuel in consumed)
                {
                    //sheet.GetRow(currentRow).GetCell(cell).CellStyle = intCellStyle;
                    sheet.GetRow(currentRow).GetCell(cell).SetCellType(CellType.Numeric);
                    sheet.GetRow(currentRow).GetCell(cell).SetCellValue(Double.Parse(fuel.Consumed.ToString(ci), formatInfo));
                    //Suma al total
                    if (currentRow != ballastLeg)
                        totalConsumedList.Where(w => w.FuelType == fuel.FuelType)
                            .Select(s => { s.Consumed += fuel.Consumed; return s; }).ToList();

                    cell++;
                }

                if (voyageCargo.Key.FunctionCode == "L")
                {
                    startIndex = endIndex; // Si el puerto es de carga se considera desde el puerto de termino
                    startIndexDistance = startIndex + 1;
                }
                else
                {
                    startIndex = endIndex + 1; // si el puerto es de descarga se considera desde el puerto siguiente al de termino
                    startIndexDistance = startIndex;
                }                    

                row++;
            }

            #endregion

            #region Contact Information
            if (this.VesselSelected.VoyageSelected.ContactInformation != null)
            {
                sheet.GetRow(59).GetCell(2).SetCellValue(this.VesselSelected.VoyageSelected.ContactInformation.UserFullName.ToString());
                sheet.GetRow(60).GetCell(2).SetCellValue(this.VesselSelected.VoyageSelected.ContactInformation.UserEmail.ToString());
            }
            #endregion

            #region Sumatoria de consumos
            int f = 4;
            foreach (Fuel totalFuel in totalConsumedList)
            {
                sheet.GetRow(56).GetCell(f).SetCellValue(totalFuel.Consumed);
                f++;
            }

            #endregion

            ByteArrayOutputStream bos = new ByteArrayOutputStream();
            try
            {
                hssfwb.Write(bos);
            }
            finally
            {
                bos.Close();
            }
            
            return bos.ToByteArray();
        }        

        /// <summary>
        /// Busca puerto según tipo
        /// </summary>
        /// <param name="index"></param>
        /// <param name="portType"></param>
        /// <param name="resultPort"></param>
        /// <param name="firstOccurrence"></param>
        private void GetPort(int index, string portType, ref int resultPort)
        {
            List<VoyageItinerary> voyageItiners = this.VesselSelected.VoyageSelected.ChartererSelected.VoyageItineraries;

            int lenght = voyageItiners.Count;
            switch (portType)
            {
                case "L":

                    if (index < lenght && voyageItiners[index].PortFunc != "L")
                        GetPort(index + 1, portType, ref resultPort);
                    else
                        resultPort = index;

                    break;

                case "C":                    
                    if (index >= 0 && voyageItiners[index].PortFunc == "W")
                        GetPort(index - 1, portType, ref resultPort);
                    else
                        resultPort = index;

                    break;
                case "D":

                    if (index < lenght)
                    {
                        if (voyageItiners[index].PortFunc == "D")
                            resultPort = index;

                        GetPort(index + 1, portType, ref resultPort);
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
        private int GetDistanceSailed(int startPort, int endPort, string portType = "")
        {
            int distance = 0;
            int currentNode = startPort;
            int lenght = this.VesselSelected.VoyageSelected.ChartererSelected.VoyageItineraries.Count;

            if (portType != "" && portType == "L")                
                endPort--; //No se considera el puerto de carga

            while ((currentNode <= endPort))
            {
                distance += this.VesselSelected.VoyageSelected.ChartererSelected.VoyageItineraries[currentNode].Miles;

                //if ((currentNode + 1) == endPort)
                //    distance += this.VesselSelected.VoyageSelected.ChartererSelected.VoyageItineraries[currentNode + 1].Miles;

                currentNode++;
            }

            return distance;
        }

        /// <summary>
        /// Calcula el consumo para los distintos tipos de combustibles entre un puerto a otro
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="endPort"></param>
        /// <returns></returns>
        private List<Fuel> GetFuelConsumed(int startPort, int endPort, string portType = "")
        {
            int currentNode = startPort;            
            List<Fuel> fuelConsumedList = ConfigurationManager.AppSettings["Fuel"]
                                          .Split(';')
                                            .Select(s => new Fuel()
                                            {
                                                FuelType = s,
                                                FuelTypeMap = ConfigurationManager.AppSettings[s] ?? s,
                                                Consumed = 0
                                            }).ToList();

            if (portType != "" && portType == "L")
                endPort--; //No se considera el puerto de carga


            while (currentNode <= endPort)
            {
                foreach (Fuel fuel in fuelConsumedList)
                {
                    double consumed = GetFuelConsumed(currentNode, fuel.FuelTypeMap);
                    if (consumed > 0)
                        fuel.Consumed += consumed;
                }

                currentNode++;
            }

            return fuelConsumedList;
        }

        /// <summary>
        /// Busca consumo para el nodo actual
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="fuelType"></param>
        /// <returns></returns>
        private double GetFuelConsumed(int currentNode, string fuelType)
        {
            double consumed = 0;
            List<VoyageLegSummary> legSummaries = this.VesselSelected.VoyageSelected.ChartererSelected.VoyageLegSummaries;

            if (currentNode <= legSummaries.Count - 1)
            {
                if (fuelType.Contains(legSummaries[currentNode].Bnkr1FuelType))
                    consumed += legSummaries[currentNode].Bnkr1Total;
                if (fuelType.Contains(legSummaries[currentNode].Bnkr2FuelType))
                    consumed += legSummaries[currentNode].Bnkr2Total;
                if (fuelType.Contains(legSummaries[currentNode].Bnkr3FuelType))
                    consumed += legSummaries[currentNode].Bnkr3Total;
                if (fuelType.Contains(legSummaries[currentNode].Bnkr4FuelType))
                    consumed += legSummaries[currentNode].Bnkr4Total;
                if (fuelType.Contains(legSummaries[currentNode].Bnkr5FuelType))
                    consumed += legSummaries[currentNode].Bnkr5Total;
                if (fuelType.Contains(legSummaries[currentNode].Bnkr6FuelType))
                    consumed += legSummaries[currentNode].Bnkr6Total;
            }

            return consumed;
        }
    }
}
