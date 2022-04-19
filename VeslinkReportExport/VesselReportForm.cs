using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Veslink.Business;
using Veslink.Entities;

namespace VeslinkReportExport
{
    public partial class VesselReportForm : Form
    {
        ReportBusiness reportBusiness = null;
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public VesselReportForm()
        {
            InitializeComponent();

            reportBusiness = new ReportBusiness();

            dtFrom.Format = DateTimePickerFormat.Custom;
            dtFrom.CustomFormat = "dd-MM-yyyy";
            dtFrom.Value = DateTime.Now.AddMonths(-6);

            dtTo.Format = DateTimePickerFormat.Custom;
            dtTo.CustomFormat = "dd-MM-yyyy";

            try
            {
                List<string> companies = reportBusiness.GetCompanies();
                foreach (string company in companies)
                    ddlCompany.Items.Add(company);

                Log.Info(" Vessel Report Generator has started successfully");
                //Trace.TraceInformation($"{ DateTime.Now } - Vessel Report Generator has started successfully");
                //Trace.Flush();
            }
            catch (Exception ex)
            {
                Log.Error(" An error occurred while loading companies: " + ex.Message);
                //Trace.TraceError($"{ DateTime.Now } - An error occurred while loading companies: { ex.Message }");
                //Trace.Flush();
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            ComboItem comboVessel = ddlVessel.SelectedItem as ComboItem;
            ComboItem comboVoyage = ddlVoyage.SelectedItem as ComboItem;
            ComboItem comboCharterer = ddlCharterer.SelectedItem as ComboItem;
            ComboItem comboCargo = ddlCargo.SelectedItem as ComboItem;

            try
            {
                Log.Info("  Generating report for the following parameters Vessel: " + comboVessel.Value + " - Voyage:" + comboVoyage.Value);
                //Trace.TraceInformation($"{ DateTime.Now } - Generating report for the following parameters Vessel: { comboVessel.Value } - Voyage: { comboVoyage.Value }");
                //Trace.Flush();

                Cursor = Cursors.WaitCursor;
                byte[] fileGenerated = reportBusiness.GenerateExcel(comboVessel.Value.ToString(),
                                                                    comboVoyage.Value.ToString(),
                                                                    comboCharterer.Value.ToString(),
                                                                    comboCargo != null ? comboCargo.Value.ToString() : "");

                if (fileGenerated != null)
                {
                    // Displays a SaveFileDialog so the user can save Excel
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "Excel |*.xlsx"; ;
                    saveFileDialog1.Title = "Save an Excel File";
                    saveFileDialog1.FileName = $"{comboVessel.Value.ToString()}_{comboVoyage.Value.ToString()}_{comboCharterer.Value.ToString()}";
                    saveFileDialog1.ShowDialog();

                    // If the file name is not an empty string open it for saving.
                    if (saveFileDialog1.FileName != "")
                    {
                        // Saves the Excel via a FileStream created by the OpenFile method.
                        System.IO.FileStream fs =
                            (System.IO.FileStream)saveFileDialog1.OpenFile();

                        fs.Write(fileGenerated, 0, fileGenerated.Length);

                        fs.Close();
                    }

                    MessageBox.Show("The report has been generated successfully");

                    Log.Info(" Report has been generated successfully - FileName: " + saveFileDialog1.FileName);
                    //Trace.TraceInformation($"{ DateTime.Now } - Report has been generated successfully - FileName: { saveFileDialog1.FileName }");
                    //Trace.Flush();
                }
                else
                {                    
                    MessageBox.Show("There are no data for the selected parameters");
                }

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                string cargo = "";
                if (comboCargo != null)
                    cargo = comboCargo.Value.ToString();

                Log.Error(" An error occurred while generating the excel file \n" + $"Parameters:\n" +
                    $"DateFrom: {dtFrom.Value} - DateTo: {dtTo.Value} - Company: {ddlCompany.Text} - Vessel: {comboVessel.Value} - Voyage: {comboVoyage.Value} - Charterer: {comboCharterer.Value} - Cargo: {cargo}\n" +
                    $"Error: { ex.StackTrace }");
                /*Trace.TraceError($"{ DateTime.Now } - An error occurred while generating the excel file \n" +
                    $"Parameters:\n" +
                    $"DateFrom: {dtFrom.Value} - DateTo: {dtTo.Value} - Company: {ddlCompany.Text} - Vessel: {comboVessel.Value} - Voyage: {comboVoyage.Value} - Charterer: {comboCharterer.Value} - Cargo: {cargo}\n" +
                    $"Error: { ex.StackTrace }");
                Trace.Flush();*/

                Cursor = Cursors.Default;

                MessageBox.Show("The report could not be generated, for more details check the application log");
            }            
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            cleanControls("All");
            GetVessels();
        }

        private void dtFrom_DropDown(object sender, EventArgs e)
        {
            dtFrom.ValueChanged -= dtFrom_ValueChanged;
        }

        private void dtFrom_CloseUp(object sender, EventArgs e)
        {
            dtFrom.ValueChanged += dtFrom_ValueChanged;
            dtFrom_ValueChanged(sender, e);
        }

        private void GetVessels()
        {
            try
            {
                if (dtFrom.Value.Date > dtTo.Value.Date)
                    MessageBox.Show("Date From must be less than or equal to Date To");

                if (ddlCompany.Text != "" && dtTo.Value.Date >= dtFrom.Value.Date)
                {
                    Cursor = Cursors.WaitCursor;
                    reportBusiness.GetVessels(dtFrom.Value, dtTo.Value, ddlCompany.Text);
                    Cursor = Cursors.Default;

                    if (reportBusiness.VesselsDB != null)
                    {
                        ddlVessel.Enabled = true;
                        ddlVessel.Items.Clear();
                        foreach (Vessel vessel in reportBusiness.VesselsDB)
                            ddlVessel.Items.Add(new ComboItem() { Value = vessel.VesselCode, Text = vessel.VesselName });
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error($" An error occurred while loading vessels \n" +
                    $"Parameters:\n" +
                    $"DateFrom: {dtFrom.Value} - DateTo: {dtTo.Value} - Company: {ddlCompany.Text}\n" +
                    $"Error: { ex.StackTrace }");
                /*Trace.TraceError($"{ DateTime.Now } - An error occurred while loading vessels \n" +
                    $"Parameters:\n" +
                    $"DateFrom: {dtFrom.Value} - DateTo: {dtTo.Value} - Company: {ddlCompany.Text}\n" +
                    $"Error: { ex.StackTrace }");
                Trace.Flush();*/
            }

        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            cleanControls("All");
            GetVessels();
        }

        private void dtTo_DropDown(object sender, EventArgs e)
        {
            dtTo.ValueChanged -= dtTo_ValueChanged;
        }

        private void dtTo_CloseUp(object sender, EventArgs e)
        {
            dtTo.ValueChanged += dtTo_ValueChanged;
            dtTo_ValueChanged(sender, e);
        }

        private void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            cleanControls("All");
            GetVessels();
        }

        private void ddlVessel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboItem comboItem = ddlVessel.SelectedItem as ComboItem;
            try
            {
                cleanControls("Vessel");                
                reportBusiness.SetSelectedVessel(comboItem.Value.ToString());

                if (reportBusiness.VesselSelected != null)
                {
                    ddlVoyage.Enabled = true;
                    ddlVoyage.Items.Clear();
                    foreach (Voyage voyage in reportBusiness.VesselSelected.Voyages)
                        ddlVoyage.Items.Add(new ComboItem() { Value = voyage.VoyageNo, Text = voyage.VoyageNo.ToString() });
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred while loading voyages \n" +
                    $"Parameters:\n" +
                    $"DateFrom: {dtFrom.Value} - DateTo: {dtTo.Value} - Company: {ddlCompany.Text} - VesselCode: {comboItem.Value}\n" +
                    $"Error: { ex.StackTrace }");

                /*Trace.TraceError($"{ DateTime.Now } - An error occurred while loading voyages \n" +
                     $"Parameters:\n" +
                     $"DateFrom: {dtFrom.Value} - DateTo: {dtTo.Value} - Company: {ddlCompany.Text} - VesselCode: {comboItem.Value}\n" +
                     $"Error: { ex.StackTrace }");
                 Trace.Flush();*/
            }

        }

        private void ddlVoyage_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboItem comboItem = ddlVoyage.SelectedItem as ComboItem;            
            try
            {
                cleanControls("Voyage");                
                reportBusiness.SetSelectedVoyage(int.Parse(comboItem.Value.ToString()));

                if (reportBusiness.VesselSelected.VoyageSelected != null)
                {
                    ddlCharterer.Enabled = true;
                    ddlCharterer.Items.Clear();
                    foreach (Charterer charterer in reportBusiness.VesselSelected.VoyageSelected.Charterers)
                        ddlCharterer.Items.Add(new ComboItem() { Value = charterer.ChartererName, Text = charterer.ChartererName });
                }
            }
            catch (Exception ex)
            {
                ComboItem comboVessel = ddlVessel.SelectedItem as ComboItem;

                Log.Error($"An error occurred while loading charterers \n" +
                    $"Parameters:\n" +
                    $"DateFrom: {dtFrom.Value} - DateTo: {dtTo.Value} - Company: {ddlCompany.Text} - VesselCode: {comboVessel.Value} - Voyage: {comboItem.Value}\n" +
                    $"Error: { ex.StackTrace }");

                /*Trace.TraceError($"{ DateTime.Now } - An error occurred while loading charterers \n" +
                     $"Parameters:\n" +
                     $"DateFrom: {dtFrom.Value} - DateTo: {dtTo.Value} - Company: {ddlCompany.Text} - VesselCode: {comboVessel.Value} - Voyage: {comboItem.Value}\n" +
                     $"Error: { ex.StackTrace }");
                 Trace.Flush();*/
            }
        }

        private void ddlCharterer_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboItem comboItem = ddlCharterer.SelectedItem as ComboItem;
            try
            {
                cleanControls("Charterer");                
                reportBusiness.SetSelectedCharterer(comboItem.Value.ToString());

                if (reportBusiness.VesselSelected.VoyageSelected.ChartererSelected != null)
                {
                    ddlCargo.Enabled = true;
                    btnExcel.Enabled = true;
                    ddlCargo.Items.Clear();
                    foreach (Cargo cargo in reportBusiness.VesselSelected.VoyageSelected.ChartererSelected.Cargos)
                        ddlCargo.Items.Add(new ComboItem() { Value = cargo.CargoID, Text = $"{cargo.CargoShortName} ({cargo.CargoQty})" });
                }
            }
            catch (Exception ex)
            {
                ComboItem comboVessel = ddlVessel.SelectedItem as ComboItem;
                ComboItem comboVoyage = ddlVoyage.SelectedItem as ComboItem;

                Log.Error("$An error occurred while loading cargos \n" +
                    $"Parameters:\n" +
                    $"DateFrom: {dtFrom.Value} - DateTo: {dtTo.Value} - Company: {ddlCompany.Text} - VesselCode: {comboVessel.Value} - Voyage: {comboVoyage.Value} - Charterer:{comboItem.Value}\n" +
                    $"Error: { ex.StackTrace }");
                /*Trace.TraceError($"{ DateTime.Now } - An error occurred while loading cargos \n" +
                    $"Parameters:\n" +
                    $"DateFrom: {dtFrom.Value} - DateTo: {dtTo.Value} - Company: {ddlCompany.Text} - VesselCode: {comboVessel.Value} - Voyage: {comboVoyage.Value} - Charterer:{comboItem.Value}\n" +
                    $"Error: { ex.StackTrace }");
                Trace.Flush();*/
            }

        }

        private void cleanControls(string control)
        {
            if (control == "All")
            {
                ddlVessel.Items.Clear();
                ddlVessel.ResetText();
                ddlVessel.Enabled = false;

                ddlVoyage.Items.Clear();
                ddlVoyage.ResetText();
                ddlVoyage.Enabled = false;

                ddlCharterer.Items.Clear();
                ddlCharterer.ResetText();
                ddlCharterer.Enabled = false;

                ddlCargo.Items.Clear();
                ddlCargo.ResetText();
                ddlCargo.Enabled = false;

                btnExcel.Enabled = false;
            }
            else if (control == "Vessel")
            {
                ddlVoyage.Items.Clear();
                ddlVoyage.ResetText();
                ddlVoyage.Enabled = false;

                ddlCharterer.Items.Clear();
                ddlCharterer.ResetText();
                ddlCharterer.Enabled = false;

                ddlCargo.Items.Clear();
                ddlCargo.ResetText();
                ddlCargo.Enabled = false;
                btnExcel.Enabled = false;

            }
            else if (control == "Voyage")
            {
                ddlCharterer.Items.Clear();
                ddlCharterer.ResetText();
                ddlCharterer.Enabled = false;

                ddlCargo.Items.Clear();
                ddlCargo.ResetText();
                ddlCargo.Enabled = false;
                btnExcel.Enabled = false;
            }
            else if (control == "Charterer")
            {
                ddlCargo.Items.Clear();
                ddlCargo.ResetText();
                ddlCargo.Enabled = false;
                btnExcel.Enabled = false;
            }
            
        }
    }
}
