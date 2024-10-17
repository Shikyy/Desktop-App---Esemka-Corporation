using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EsemkaCorporation
{
    public partial class Profile : Form
    {
        Connect Con = new Connect();
        int id, supervisorId;

        public Profile()
        {
            InitializeComponent();
        }

        void supervisorData()
        {
            try
            {
                SqlConnection con = Con.GetConnection();
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT e.name as Name, e.email as Email, e.phone_number as PhoneNumber, e.hire_date as HireDate, j.name as Position, jl.name as JobLevel, d.name as Department " +
                    "FROM employee e " +
                    "JOIN position p ON p.employee_id = e.id " +
                    "JOIN job j ON j.id = p.job_id " +
                    "JOIN job_level jl ON j.job_level_id = jl.id " +
                    "JOIN department d ON d.id = j.department_id " +
                    "WHERE j.supervisor_job_id = @supervisorId", con);

                cmd.Parameters.AddWithValue("@supervisorId", LoggedInEmployee.GetLoggedEmployee().supervisorId);

                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    string name = dr["Name"].ToString();
                    string email = dr["Email"].ToString();
                    string phoneNumber = dr["PhoneNumber"].ToString();
                    string hireDate = dr["HireDate"].ToString();
                    string position = dr["Position"].ToString();
                    string jobLevel = dr["JobLevel"].ToString();
                    string department = dr["Department"].ToString();

                    txtN.Text = name;
                    txtE.Text = email;
                    txtPn.Text = phoneNumber;
                    txtHd.Text = hireDate;
                    txtP.Text = position;
                    txtJl.Text = jobLevel;
                    txtD.Text = department;
                }
            }
            catch (Exception ex)
            {

            }
        }

        void displayWorksWith()
        {
            try
            {
                SqlConnection con = Con.GetConnection();
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT e.name as Name, j.name as Position " +
                    "FROM employee e " +
                    "JOIN position p ON p.employee_id = e.id " +
                    "JOIN job j ON p.job_id = j.id " +
                    "JOIN job_level jl ON j.job_level_id = jl.id " +
                    "WHERE j.supervisor_job_id = @supervisorId AND p.deleted_at IS NULL AND e.id != @employeeId", con);

                cmd.Parameters.AddWithValue("@supervisorId", supervisorId);
                cmd.Parameters.AddWithValue("@employeeId", id);

                DataSet ds = new DataSet();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(ds, "EmployeeWorksWith");

                dgvWw.DataSource = ds.Tables["EmployeeWorksWith"];
                dgvWw.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        void displaySubordinate()
        {
            try
            {
                SqlConnection con = Con.GetConnection();
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT e.name as Name, j.name as Position " +
                    "FROM employee e " +
                    "JOIN position p ON e.id = p.employee_id " +
                    "JOIN job j ON j.id = p.job_id " +
                    "JOIN job_level jl ON jl.id = j.job_level_id " +
                    "WHERE jl.id = (" +
                    "SELECT jl.id - 1 " +
                    "FROM employee e " +
                    "JOIN position p ON e.id = p.employee_id " +
                    "JOIN job j ON j.id = p.job_id " +
                    "JOIN job_level jl ON jl.id = j.job_level_id " +
                    "WHERE e.id = @employeeId AND p.deleted_at IS NULL)", con);

                cmd.Parameters.AddWithValue("@employeeId", id);
                DataSet ds = new DataSet();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(ds, "EmployeeSubor");

                dgvS.DataSource = ds.Tables["EmployeeSubor"];
                dgvS.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void displayJobHistory()
        {
            try
            {
                SqlConnection con = Con.GetConnection();
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT d.name as Department, j.name as Position " +
                    "From employee e " +
                    "JOIN position p ON e.id = p.employee_id " +
                    "JOIN job j ON j.id = p.job_id " +
                    "JOIN job_level jl ON jl.id = j.job_level_id " +
                    "JOIN department d ON d.id = j.department_id " +
                    "WHERE e.id = @employeeId AND p.deleted_at is NOT NULL " +
                    "ORDER BY jl.id DESC", con);

                cmd.Parameters.AddWithValue("@employeeId", id);

                DataSet ds = new DataSet();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(ds, "EmployeeHistory");

                dgvJh.DataSource = ds.Tables["EmployeeHistory"];
                dgvJh.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Profile_Load(object sender, EventArgs e)
        {
            LoggedEmployee _loggedEmployee = LoggedInEmployee.GetLoggedEmployee();
            if (_loggedEmployee != null)
            {
                id = _loggedEmployee.id;
                txtN.Text = _loggedEmployee.name;
                txtE.Text = _loggedEmployee.email;
                txtPn.Text = _loggedEmployee.phoneNumber;
                txtHd.Text = _loggedEmployee.hireDate.ToString();
                txtP.Text = _loggedEmployee.position;
                txtJl.Text = _loggedEmployee.jobLevel;
                txtD.Text = _loggedEmployee.department;
                supervisorId = _loggedEmployee.supervisorId;

                displayJobHistory();
                displaySubordinate();
                displayWorksWith();
            }
        }

        private void lblDs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            supervisorData();
        }

        private void btnM_Click(object sender, EventArgs e)
        {
            MainForm mf = new MainForm();
            mf.Show();
            this.Close();
        }
    }
}
