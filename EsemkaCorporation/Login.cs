using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EsemkaCorporation
{
    public partial class Login : Form
    {
        Connect Con = new Connect();

        public Login()
        {
            InitializeComponent();
        }

        private void btnL_Click(object sender, EventArgs e)
        {
            if (txtE.Text.Equals("") || txtP.Text.Equals(""))
            {
                lblMessage.Text = "Fill the both of fields";
                lblMessage.Visible = true;
            }
            else
            {
                SqlConnection con = Con.GetConnection();
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT e.id as EmployeeID, e.name as EmployeeName, e.email as Email, e.phone_number as PhoneNumber, e.hire_date as HireDate, j.name as Position, jl.name as JobLevel, d.name as Department, j.supervisor_job_id as SupervisorId " +
                    "FROM employee e " +
                    "JOIN position p ON p.employee_id = e.id " +
                    "JOIN job j ON j.id = p.job_id " +
                    "JOIN job_level jl ON jl.id = j.job_level_id " +
                    "JOIN department d ON d.id = j.department_id " +
                    "WHERE e.email = @email AND e.password = @pass AND p.deleted_at IS NULL", con);

                cmd.Parameters.AddWithValue("@email", txtE.Text);
                cmd.Parameters.AddWithValue("@pass", txtP.Text);

                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        int id = Convert.ToInt32(dr["EmployeeID"]);
                        string name = dr["EmployeeName"].ToString();
                        string email = dr["Email"].ToString();
                        string phoneNumber = dr["PhoneNumber"].ToString();
                        DateTime hireDate = Convert.ToDateTime(dr["HireDate"]);
                        string position = dr["Position"].ToString();
                        string jobLevel = dr["JobLevel"].ToString();
                        string department = dr["Department"].ToString();
                        int supervisorId = Convert.ToInt32(dr["SupervisorId"]);

                        LoggedEmployee loggedEmployee = new LoggedEmployee(id, name, email, phoneNumber, hireDate, position, jobLevel, department, supervisorId);

                        LoggedInEmployee.SetLoggedEmployee(loggedEmployee);

                        MainForm mf = new MainForm();
                        MessageBox.Show("Login Successfull", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        mf.Show();
                        this.Hide();
                    }
                }
                else
                {
                    lblMessage.Text = "Email or password wrong!";
                    lblMessage.Visible = true;
                }
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
