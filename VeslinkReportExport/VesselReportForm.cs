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
        public VesselReportForm()
        {
            InitializeComponent();

            reportBusiness = new ReportBusiness();

            dtFrom.Format = DateTimePickerFormat.Custom;
            dtFrom.CustomFormat = "dd-MM-yyyy";

            dtTo.Format = DateTimePickerFormat.Custom;
            dtTo.CustomFormat = "dd-MM-yyyy";

            try
            {
                List<string> companies = reportBusiness.GetCompanies();
                foreach (string company in companies)
                    ddlCompany.Items.Add(company);

                Trace.TraceInformation($"{ DateTime.Now } - Vessel Report Generator has started successfully");
                Trace.Flush();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"{ DateTime.Now } - An error occurred while loading companies: { ex.Message }");
                Trace.Flush();
            }            
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                ComboItem comboVessel = ddlVessel.SelectedItem as ComboItem;
                ComboItem comboVoyage = ddlVoyage.SelectedItem as ComboItem;

                Trace.TraceInformation($"{ DateTime.Now } - Generating report for the following parameters Vessel: { comboVessel.Value } - Voyage: { comboVoyage.Value }");
                Trace.Flush();

                Cursor = Cursors.WaitCursor;
                string fileGenerated = reportBusiness.GenerateExcel(comboVessel.Value.ToString(), comboVoyage.Value.ToString());
                Cursor = Cursors.Default;

                Trace.TraceInformation($"{ DateTime.Now } - Report has been generated successfully - FileName: { fileGenerated }");
                Trace.Flush();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"{ DateTime.Now } - An error occurred while generating the excel file: { ex.Message }");
                Trace.Flush();
            }            
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            GetVessels();
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
                Trace.TraceError($"{ DateTime.Now } - An error occurred while loading vessels: { ex.Message }");
                Trace.Flush();
            }

        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            GetVessels();
        }

        private void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetVessels();
        }

        private void ddlVessel_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboItem comboItem = ddlVessel.SelectedItem as ComboItem;
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
                Trace.TraceError($"{ DateTime.Now } - An error occurred while loading voyages: { ex.Message }");
                Trace.Flush();
            }

        }

        private void ddlVoyage_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboItem comboItem = ddlVoyage.SelectedItem as ComboItem;
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
                Trace.TraceError($"{ DateTime.Now } - An error occurred while loading charterers: { ex.Message }");
                Trace.Flush();
            }
        }

        private void ddlCharterer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboItem comboItem = ddlCharterer.SelectedItem as ComboItem;
                reportBusiness.SetSelectedCharterer(comboItem.Value.ToString());

                if (reportBusiness.VesselSelected.VoyageSelected.ChartererSelected != null)
                {
                    ddlCargo.Enabled = true;
                    btnExcel.Enabled = true;
                    ddlCargo.Items.Clear();
                    foreach (Cargo cargo in reportBusiness.VesselSelected.VoyageSelected.ChartererSelected.Cargos)
                        ddlCargo.Items.Add(new ComboItem() { Value = cargo.CargoID, Text = cargo.CargoID.ToString() });
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"{ DateTime.Now } - An error occurred while loading cargos: { ex.Message }");
                Trace.Flush();
            }

        }
    }
}
