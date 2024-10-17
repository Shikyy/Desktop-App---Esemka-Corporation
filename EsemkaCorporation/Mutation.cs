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
    public partial class Mutation : Form
    {
        Connect Con = new Connect();

        public Mutation()
        {
            InitializeComponent();
        }

        void displayAvailableMutation()
        {
            try
            {
                SqlConnection con = Con.GetConnection();
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT j.id as JobId, d.name as Department, j.name as Position " +
                    "FROM job j " +
                    "JOIN job_level jl ON j.job_level_id = jl.id " +
                    "JOIN department d ON d.id = j.department_id " +
                    "LEFT JOIN (" +
                    "SELECT p.job_id, COUNT(*) as EmployeeCount " +
                    "FROM position p " +
                    "JOIN employee e ON p.employee_id = e.id " +
                    "GROUP BY p.job_id " +
                    ") pc ON j.id = pc.job_id " +
                    "WHERE ISNULL(pc.EmployeeCount, 0) < j.head_count", con);

                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);

                dgvM.DataSource = dt;
                dgvM.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dgvM.Columns["apply"] == null)
                {
                    DataGridViewButtonColumn apply = new DataGridViewButtonColumn();
                    apply.HeaderText = "Action";
                    apply.Name = "apply";
                    apply.UseColumnTextForButtonValue = true;
                    apply.Text = "Apply";
                    dgvM.Columns.Add(apply);
                }

                dgvM.Columns["JobId"].Visible = false;

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Mutation_Load(object sender, EventArgs e)
        {
            LoggedEmployee _loggedEmployee = LoggedInEmployee.GetLoggedEmployee();
            if (_loggedEmployee != null)
            {
                txtN.Text = _loggedEmployee.name;
                txtCd.Text = _loggedEmployee.department;
                txtCp.Text = _loggedEmployee.position;
                txtCjl.Text = _loggedEmployee.jobLevel;

                displayAvailableMutation();
            }
        }

        private void btnM_Click(object sender, EventArgs e)
        {
            MainForm mf = new MainForm();
            mf.Show();
            this.Close();
        }

        private void dgvM_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int jobId = Convert.ToInt32(dgvM.Rows[e.RowIndex].Cells["JobId"].Value);
                int employeeId = LoggedInEmployee.GetLoggedEmployee().id;

                using (SqlConnection con = Con.GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM mutation WHERE job_id = @jobId AND employee_id = @employeeId AND status = 'P'", con))
                    {
                        cmd.Parameters.AddWithValue("@jobId", jobId);
                        cmd.Parameters.AddWithValue("employeeId", employeeId);

                        int count = (int)cmd.ExecuteScalar();
                        if(count > 0)
                        {
                            MessageBox.Show("You have already applied for this job.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            using (SqlCommand cmdd = new SqlCommand("INSERT INTO mutation(job_id, employee_id, status, created_at)VALUES(@jobId, @employeeId, @status, @created)", con))
                            {
                                cmdd.Parameters.AddWithValue("@jobId", jobId);
                                cmdd.Parameters.AddWithValue("@employeeId", employeeId);
                                cmdd.Parameters.AddWithValue("@status", "P");
                                cmdd.Parameters.AddWithValue("@created", DateTime.Now);

                                int check = cmdd.ExecuteNonQuery();
                                if(check > 0)
                                {
                                    MessageBox.Show("Application has been sent.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Failed to apply.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
