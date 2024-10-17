using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EsemkaCorporation
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoggedEmployee _loggedEmployee = LoggedInEmployee.GetLoggedEmployee();
            if (_loggedEmployee != null)
            {
                string name = _loggedEmployee.name;

                lblWelcome.Text = "Welcome, " + name;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Profile p = new Profile();
            p.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Mutation m = new Mutation();
            m.Show(); 
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Promotion p = new Promotion();
            p.Show();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Login l = new Login();
            l.Show();
            this.Close();
        }
    }
}
