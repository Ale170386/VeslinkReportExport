namespace VeslinkReportExport
{
    partial class VesselReportForm
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VesselReportForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnExcel = new System.Windows.Forms.Button();
            this.ddlVessel = new System.Windows.Forms.ComboBox();
            this.ddlVoyage = new System.Windows.Forms.ComboBox();
            this.ddlCompany = new System.Windows.Forms.ComboBox();
            this.ddlCharterer = new System.Windows.Forms.ComboBox();
            this.ddlCargo = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(71, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Date From";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(340, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Date To";
            // 
            // dtFrom
            // 
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtFrom.Location = new System.Drawing.Point(74, 68);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(226, 22);
            this.dtFrom.TabIndex = 1;
            this.dtFrom.CloseUp += new System.EventHandler(this.dtFrom_CloseUp);
            this.dtFrom.ValueChanged += new System.EventHandler(this.dtFrom_ValueChanged);
            this.dtFrom.DropDown += new System.EventHandler(this.dtFrom_DropDown);
            // 
            // dtTo
            // 
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtTo.Location = new System.Drawing.Point(343, 68);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(255, 22);
            this.dtTo.TabIndex = 2;
            this.dtTo.CloseUp += new System.EventHandler(this.dtTo_CloseUp);
            this.dtTo.ValueChanged += new System.EventHandler(this.dtTo_ValueChanged);
            this.dtTo.DropDown += new System.EventHandler(this.dtTo_DropDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(337, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Vessel";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(69, 200);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 16);
            this.label4.TabIndex = 5;
            this.label4.Text = "Voyage";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(71, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "Company";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(340, 198);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 16);
            this.label6.TabIndex = 10;
            this.label6.Text = "Charterer";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(71, 274);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 16);
            this.label7.TabIndex = 12;
            this.label7.Text = "Cargo";
            // 
            // btnExcel
            // 
            this.btnExcel.Enabled = false;
            this.btnExcel.Location = new System.Drawing.Point(421, 273);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(176, 42);
            this.btnExcel.TabIndex = 8;
            this.btnExcel.Text = "Excel";
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // ddlVessel
            // 
            this.ddlVessel.Enabled = false;
            this.ddlVessel.FormattingEnabled = true;
            this.ddlVessel.Location = new System.Drawing.Point(343, 136);
            this.ddlVessel.Name = "ddlVessel";
            this.ddlVessel.Size = new System.Drawing.Size(255, 24);
            this.ddlVessel.TabIndex = 4;
            this.ddlVessel.SelectedIndexChanged += new System.EventHandler(this.ddlVessel_SelectedIndexChanged);
            // 
            // ddlVoyage
            // 
            this.ddlVoyage.Enabled = false;
            this.ddlVoyage.FormattingEnabled = true;
            this.ddlVoyage.Location = new System.Drawing.Point(74, 216);
            this.ddlVoyage.Name = "ddlVoyage";
            this.ddlVoyage.Size = new System.Drawing.Size(228, 24);
            this.ddlVoyage.TabIndex = 5;
            this.ddlVoyage.SelectedIndexChanged += new System.EventHandler(this.ddlVoyage_SelectedIndexChanged);
            // 
            // ddlCompany
            // 
            this.ddlCompany.FormattingEnabled = true;
            this.ddlCompany.Location = new System.Drawing.Point(74, 136);
            this.ddlCompany.Name = "ddlCompany";
            this.ddlCompany.Size = new System.Drawing.Size(226, 24);
            this.ddlCompany.TabIndex = 3;
            this.ddlCompany.SelectedIndexChanged += new System.EventHandler(this.ddlCompany_SelectedIndexChanged);
            // 
            // ddlCharterer
            // 
            this.ddlCharterer.Enabled = false;
            this.ddlCharterer.FormattingEnabled = true;
            this.ddlCharterer.Location = new System.Drawing.Point(344, 216);
            this.ddlCharterer.Name = "ddlCharterer";
            this.ddlCharterer.Size = new System.Drawing.Size(253, 24);
            this.ddlCharterer.TabIndex = 6;
            this.ddlCharterer.SelectedIndexChanged += new System.EventHandler(this.ddlCharterer_SelectedIndexChanged);
            // 
            // ddlCargo
            // 
            this.ddlCargo.Enabled = false;
            this.ddlCargo.FormattingEnabled = true;
            this.ddlCargo.Location = new System.Drawing.Point(74, 291);
            this.ddlCargo.Name = "ddlCargo";
            this.ddlCargo.Size = new System.Drawing.Size(225, 24);
            this.ddlCargo.TabIndex = 7;
            // 
            // VesselReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(657, 382);
            this.Controls.Add(this.ddlCargo);
            this.Controls.Add(this.ddlCharterer);
            this.Controls.Add(this.ddlCompany);
            this.Controls.Add(this.ddlVoyage);
            this.Controls.Add(this.ddlVessel);
            this.Controls.Add(this.btnExcel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dtTo);
            this.Controls.Add(this.dtFrom);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "VesselReportForm";
            this.Text = "Vessel Report";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.ComboBox ddlVessel;
        private System.Windows.Forms.ComboBox ddlVoyage;
        private System.Windows.Forms.ComboBox ddlCompany;
        private System.Windows.Forms.ComboBox ddlCharterer;
        private System.Windows.Forms.ComboBox ddlCargo;
    }
}

