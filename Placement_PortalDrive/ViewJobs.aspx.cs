using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace HR_Module
{
    public partial class ViewJobs : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadJobs();
            }
        }

        private void LoadJobs()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["mydbConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT JobId, Title, Description, Location, Salary, CreatedAt, DriveDate FROM Jobs";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable jobsTable = new DataTable();
                    adapter.Fill(jobsTable);

                    GridViewJobs.DataSource = jobsTable;
                    GridViewJobs.DataBind();
                }
            }
        }

        protected void SaveJob(object sender, EventArgs e)
        {
            string title = titleTextBox.Text;
            string description = descriptionTextArea.Text;
            string location = locationTextBox.Text;
            decimal salary= Convert.ToDecimal( salaryTextBox.Text);
            string driveDate = DriveDate.Text.Trim();

            if (!decimal.TryParse(salaryTextBox.Text, out salary))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Invalid salary amount!');", true);
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["mydbConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO Jobs (Title, Description, Location, Salary,DriveDate) VALUES (@Title, @Description, @Location, @Salary,@DriveDate)";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@Location", location);
                    cmd.Parameters.AddWithValue("@Salary", salary);
                    cmd.Parameters.AddWithValue("@DriveDate", driveDate);
                    ;

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Job posted successfully!');", true);
                        LoadJobs(); 
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Failed to post job.');", true);
                    }

                    titleTextBox.Text = "";
                    descriptionTextArea.Text= "";
                    locationTextBox.Text= "";
                    salaryTextBox.Text = "";
                    DriveDate.Text="";

                }
            }
        }

      
    }
}
