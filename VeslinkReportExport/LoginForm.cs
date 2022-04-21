using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Configuration;
using log4net;

namespace VeslinkReportExport
{
    public partial class LoginForm : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (ConfigurationManager.AppSettings["user"] != txtUser.Text)
                {
                    MessageBox.Show("Wrong user", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (ConfigurationManager.AppSettings["pass"] != txtPassword.Text)
                {                    
                    MessageBox.Show("Wrong password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Log.Info("Successfully logged in");
                //Trace.TraceInformation($"{ DateTime.Now } - Successfully logged in");
                //Trace.Flush();

                this.Hide();
                Menu menu = new Menu();
                menu.Closed += (s, args) => this.Close();
                menu.Show();
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while trying to log in:" + ex.Message );
                //Trace.TraceError($"{ DateTime.Now } - An error occurred while trying to log in: { ex.Message }");
                //Trace.Flush();
            }

            
        }
    }
}
