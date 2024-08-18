using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;

namespace Placement_PortalDrive
{
    public partial class CollegeDashboard2 : System.Web.UI.Page
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["mydbConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindStudentData();
                
                DisplayTotalStudents();
                LoadStudentData();
                LoadJobs();
                LoadStudentDataApply();
            }
                
        }

        private void BindStudentData()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT Id,GR,FullName,EmailID,MobileNo,Gender,Branch,CurrentYear FROM Students";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    StudentsGridView.DataSource = dt;
                    StudentsGridView.DataBind();
                }
            }
        }


        private void LoadJobs()
        {
            string connectionString = WebConfigurationManager.ConnectionStrings["mydbConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT Title, Description, Location, Salary, DriveDate FROM Jobs";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvJobs.DataSource = dt;
                    gvJobs.DataBind();
                }
            }
        }


        private void LoadStudentDataApply()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["mydbConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Name, ContactNo, Branch, Marks10th, Marks12th, GraduateMarks, MCAMarks FROM StudentInfo";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable studentTable = new DataTable();
                    adapter.Fill(studentTable);

                    GridView1.DataSource = studentTable;
                    GridView1.DataBind();
                }
            }
        }
           
            protected void SaveDetails_Click(object sender, EventArgs e)
         {
            
            string companyName = CompanyName.Text.Trim();
            string driveDate = DriveDate.Text.Trim();
            string location = Location.Text.Trim();
            string position = Position.Text.Trim();
            string vacancy = Vacancy.Text.Trim();
            string branch = Branch.Text.Trim(); 

            
            if (string.IsNullOrEmpty(companyName) || string.IsNullOrEmpty(driveDate) || string.IsNullOrEmpty(location) || string.IsNullOrEmpty(position) || string.IsNullOrEmpty(vacancy) || string.IsNullOrEmpty(branch))
            {
               
                return;
            }

           
            string connectionString = ConfigurationManager.ConnectionStrings["mydbConnectionString"].ConnectionString;
            string query = "INSERT INTO Company (CompanyName, DriveDate, Location, Position, Vacancy, Branch) VALUES (@CompanyName, @DriveDate, @Location, @Position, @Vacancy, @Branch)";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CompanyName", companyName);
                        cmd.Parameters.AddWithValue("@DriveDate", driveDate);
                        cmd.Parameters.AddWithValue("@Location", location);
                        cmd.Parameters.AddWithValue("@Position", position);
                        cmd.Parameters.AddWithValue("@Vacancy", int.Parse(vacancy));
                        cmd.Parameters.AddWithValue("@Branch", branch);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

               
                CompanyName.Text = "";
                DriveDate.Text = "";
                Location.Text = "";
                Position.Text = "";
                Vacancy.Text = "";
                Branch.Text = "";
            }
            catch (Exception ex)
            {
            }
        }

        private void DisplayTotalStudents()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Students", con))
                {
                    con.Open();
                    int totalStudents = (int)cmd.ExecuteScalar();
                    TotalStudentsLabel.Text = $"Total Number of Students: {totalStudents}";
                }
            }
        }

        protected void StudentsGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int studentId = Convert.ToInt32(StudentsGridView.DataKeys[e.RowIndex].Value);
            string fullName = ((TextBox)StudentsGridView.Rows[e.RowIndex].FindControl("txtFullName")).Text;
            string email = ((TextBox)StudentsGridView.Rows[e.RowIndex].FindControl("txtEmail")).Text;
            string mobileNo = ((TextBox)StudentsGridView.Rows[e.RowIndex].FindControl("txtMobileNo")).Text;
            string gr = ((TextBox)StudentsGridView.Rows[e.RowIndex].FindControl("txtGR")).Text;
            string gender = ((DropDownList)StudentsGridView.Rows[e.RowIndex].FindControl("ddlGender")).SelectedValue;
            string courseName = ((TextBox)StudentsGridView.Rows[e.RowIndex].FindControl("txtCourseName")).Text;
            int year = Convert.ToInt32(((TextBox)StudentsGridView.Rows[e.RowIndex].FindControl("txtYear")).Text);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE Students SET StudentFullName = @FullName, Email = @Email, MobileNo = @MobileNo, GR = @GR, Gender = @Gender, CourseName = @CourseName, Year = @Year WHERE Id = @StudentId", con))
                {
                    cmd.Parameters.AddWithValue("@FullName", (object)fullName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo", (object)mobileNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GR", (object)gr ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender", (object)gender ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CourseName", (object)courseName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Year", (object)year ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            StudentsGridView.EditIndex = -1;
            BindStudentData();
        }


        protected void StudentsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int studentId = Convert.ToInt32(StudentsGridView.DataKeys[e.RowIndex].Value);
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Students WHERE Id = @StudentId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            
            BindStudentData();
        }





        protected void StudentsGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            StudentsGridView.EditIndex = e.NewEditIndex;
            BindStudentData(); 
        }

        protected void StudentsGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            StudentsGridView.EditIndex = -1;
            BindStudentData(); 
        }

        private void LoadStudentData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["mydbConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM StudentAdmission";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                GridViewStudents.DataSource = dt;
                GridViewStudents.DataBind();
            }
        }

        
        protected void btnGenerateGR_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
               
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["mydbConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

               
                string selectQuery = "SELECT StudentID, GR FROM StudentAdmission WHERE Email = @Email";
                using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn))
                {
                    selectCmd.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int studentID = reader.GetInt32(0);
                            string existingGR = reader["GR"] as string;

                            if (string.IsNullOrEmpty(existingGR))
                            {
                               
                                string generatedGR = GenerateGRNumber();

                                
                                reader.Close(); 
                                string updateQuery = "UPDATE StudentAdmission SET GR = @GR WHERE StudentID = @StudentID";
                                using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@GR", generatedGR);
                                    updateCmd.Parameters.AddWithValue("@StudentID", studentID);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                               
                            }
                        }
                    }
                }
            }
            LoadStudentData();
        }

      
        public string GenerateGRNumber()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["mydbConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

               
                string query = "SELECT MAX(CAST(GR AS INT)) FROM StudentAdmission WHERE ISNUMERIC(GR) = 1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    int lastGRNumber = 0;

                    if (result != DBNull.Value)
                    {
                        lastGRNumber = Convert.ToInt32(result);
                    }

                   
                    int newGRNumber = lastGRNumber + 1;

                    
                    return newGRNumber.ToString("D10");
                }
            }
        }

    }
}