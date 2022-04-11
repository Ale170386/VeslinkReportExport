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

namespace VeslinkReportExport
{
    public partial class LoginForm : Form
    {
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

                Trace.TraceInformation($"{ DateTime.Now } - Successfully logged in");
                Trace.Flush();

                this.Hide();
                Menu menu = new Menu();
                menu.Closed += (s, args) => this.Close();
                menu.Show();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"{ DateTime.Now } - An error occurred while trying to log in: { ex.Message }");
                Trace.Flush();
            }

            
        }
    }
}
