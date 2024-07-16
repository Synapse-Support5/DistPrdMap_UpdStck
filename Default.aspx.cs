using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DistPrdMap_UpdStck.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DistPrdMap_UpdStck
{
    public partial class _Default : Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["SqlConn"].ToString());
        DataTable dt = new DataTable();
        DataTable resdt = new DataTable();
        DataSet ds1 = new DataSet();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        #region Login_Click
        protected void Login_Click(object sender, EventArgs e)
        {
            BtnLogin();
        }
        #endregion

        #region BtnSubmit
        public async Task BtnLogin()
        {
            try
            {
                string empId = EmpId.Text;
                string password = Password.Text;

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd1 = new SqlCommand("Nonwisdom_DistPrdMap_AccessLoad", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@session_Name", empId);
                cmd1.Parameters.AddWithValue("@password", password);

                cmd1.CommandTimeout = 6000;
                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                da.Fill(resdt);

                if (resdt.Rows.Count > 0)
                {
                    DistPrdMapLink.Visible = true;
                    UpdateStockLink.Visible = true;
                    lblUserName.Text = "Welcome " + resdt.Rows[0][0].ToString();
                    abc.Visible = false;
                }
                else
                {
                    Response.Redirect("AccessDeniedPage.aspx");
                }
                con.Close();
            }
            catch (Exception ex)
            {
                
            }
        }
        #endregion

    }
}