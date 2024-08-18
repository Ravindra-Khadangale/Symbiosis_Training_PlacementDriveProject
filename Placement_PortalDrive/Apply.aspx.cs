using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Placement_PortalDrive
{
    public partial class Apply : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               
                string companyId = Request.QueryString["CompanyId"];

                if (!string.IsNullOrEmpty(companyId))
                {
                    LoadCompanyName(companyId);
                }
                else
                {
                    lblCompanyName.Text = "Company ID not provided.";
                }
            }
        }

        private void LoadCompanyName(string companyId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["mydbConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
              
                string query = "SELECT CompanyName FROM Company WHERE CompanyId = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    
                    cmd.Parameters.AddWithValue("@Id", companyId);


                    try
                    {
                        conn.Open();
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            lblCompanyName.Text = result.ToString();
                        }
                        else
                        {
                            lblCompanyName.Text = "Company not found.";
                        }
                    }
                    catch (Exception ex)
                    {
                        
                        lblCompanyName.Text = "Error fetching company name: " + ex.Message;
                    }
                }
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
           
            string name = txtName.Text;
            string contact = txtContact.Text;
            string branch = rblBranch.SelectedValue;
            string marks10th = txt10thMarks.Text;
            string marks12th = txt12thMarks.Text;
            string graduateMarks = txtGraduateMarks.Text;
            string mcaMarks = txtMCAMarks.Text;

            
            string connString = ConfigurationManager.ConnectionStrings["mydbConnectionString"].ConnectionString;

           
            string query = "INSERT INTO StudentInfo (Name, ContactNo, Branch, Marks10th, Marks12th, GraduateMarks, MCAMarks) " +
                           "VALUES (@Name, @ContactNo, @Branch, @Marks10th, @Marks12th, @GraduateMarks, @MCAMarks)";

            
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@ContactNo", contact);
                    cmd.Parameters.AddWithValue("@Branch", branch);
                    cmd.Parameters.AddWithValue("@Marks10th", marks10th);
                    cmd.Parameters.AddWithValue("@Marks12th", marks12th);
                    cmd.Parameters.AddWithValue("@GraduateMarks", graduateMarks);
                    cmd.Parameters.AddWithValue("@MCAMarks", mcaMarks);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Data inserted successfully!'); window.location='DashboardStudent.aspx';", true);



                    }
                    catch (Exception ex)
                    {
                        Response.Write("Error: " + ex.Message);
                    }
                }
            }


        }
    }
}
