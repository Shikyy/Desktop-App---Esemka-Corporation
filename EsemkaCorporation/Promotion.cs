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
    public partial class Promotion : Form
    {
        Connect Con = new Connect();

        public Promotion()
        {
            InitializeComponent();
        }

        private int GetJobLevelId(int employeeId)
        {
            using (SqlConnection con = Con.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT j.job_level_id " +
                    "FROM employee e " +
                    "JOIN position p ON e.id = p.employee_id " +
                    "JOIN job j ON j.id = p.job_id " +
                    "WHERE e.id = @employeeId", con);

                cmd.Parameters.AddWithValue("@employeeId", employeeId);

                return (int)cmd.ExecuteScalar();
            }
        }

        void displayAvailablePromotion()
        {
            try
            {
                int employeeId = LoggedInEmployee.GetLoggedEmployee().id;
                int nextJobLevelId = GetJobLevelId(employeeId);

                SqlConnection con = Con.GetConnection();
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT j.id as JobId, d.name as Department, j.name as Position " +
                    "FROM job j " +
                    "JOIN job_level jl ON j.job_level_id = jl.id " +
                    "JOIN department d ON j.department_id = d.id " +
                    "LEFT JOIN (" +
                    "SELECT p.job_id, COUNT(*) as EmployeeCount " +
                    "FROM position p " +
                    "JOIN employee e ON e.id = p.employee_id " +
                    "WHERE p.deleted_at IS NULL " +
                    "GROUP BY p.job_id" +
                    ") pc ON j.id = pc.job_id " +
                    "WHERE ISNULL(pc.EmployeeCount, 0) < j.head_count " +
                    "AND jl.id = @nextJobLevelId", con);

                cmd.Parameters.AddWithValue("@nextJobLevelId", nextJobLevelId + 1);

                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);

                dgvP.DataSource = dt;
                dgvP.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dgvP.Columns["apply"] == null)
                {
                    DataGridViewButtonColumn apply = new DataGridViewButtonColumn();
                    apply.HeaderText = "Action";
                    apply.Text = "Apply";
                    apply.Name = "apply";
                    apply.UseColumnTextForButtonValue = true;
                    dgvP.Columns.Add(apply);
                }

                dgvP.Columns["JobId"].Visible = false;

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Display: " + ex.Message);
            }
        }

        private void Promotion_Load(object sender, EventArgs e)
        {
            LoggedEmployee _loggedEmployee = LoggedInEmployee.GetLoggedEmployee();
            if (_loggedEmployee != null)
            {
                txtN.Text = _loggedEmployee.name;
                txtCd.Text = _loggedEmployee.department;
                txtCp.Text = _loggedEmployee.position;
                txtCjl.Text = _loggedEmployee.jobLevel;

                displayAvailablePromotion();
            }
        }

        private void dgvP_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int jobId = Convert.ToInt32(dgvP.Rows[e.RowIndex].Cells["JobId"].Value);
                int employeeId = LoggedInEmployee.GetLoggedEmployee().id;

                using (SqlConnection con = Con.GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM promotion WHERE job_id = @jobId AND employee_id = @employeeId AND status = 'P'", con))
                    {
                        cmd.Parameters.AddWithValue("@jobId", jobId);
                        cmd.Parameters.AddWithValue("@employeeId", employeeId);

                        int count = (int)cmd.ExecuteScalar();
                        if(count > 0)
                        {
                            MessageBox.Show("You have already applied for this job", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            using (SqlCommand cmdd = new SqlCommand("INSERT INTO promotion(job_id, employee_id, status, created_at)VALUES(@jobId, @employeeId, @status, @created)", con))
                            {
                                cmdd.Parameters.AddWithValue("@jobId", jobId);
                                cmdd.Parameters.AddWithValue("@employeeId", employeeId);
                                cmdd.Parameters.AddWithValue("@status", "P");
                                cmdd.Parameters.AddWithValue("@created", DateTime.Now);

                                int check = cmdd.ExecuteNonQuery();
                                if(check > 0)
                                {
                                    MessageBox.Show("Apply has been sent", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Failed to applied job", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void btnM_Click(object sender, EventArgs e)
        {
            MainForm mf = new MainForm();
            mf.Show();
            this.Close();
        }
    }
}
