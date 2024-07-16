using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DistPrdMap_UpdStck.Models;
using System.Drawing;
using System.Net;

namespace DistPrdMap_UpdStck
{
    public partial class UpdateStock : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["SqlConn"].ToString());
        DataTable dt = new DataTable();
        DataTable resdt = new DataTable();
        DataSet ds1 = new DataSet();
        bool anyCheckboxSelected = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AccessLoad();
                DistErpLoad();
            }
        }

        #region AccessLoad
        public void AccessLoad()
        {
            try
            {
                //Session["name"] = "G116036";
                Session["name"] = Request.ServerVariables["REMOTE_USER"].Substring(6);

                if (Session["name"].ToString() != "")
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    SqlCommand cmd1 = new SqlCommand("Nonwisdom_DistPrdMap_AccessLoad", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@session_Name", Session["name"].ToString());

                    cmd1.CommandTimeout = 6000;
                    SqlDataAdapter da = new SqlDataAdapter(cmd1);
                    da.Fill(resdt);

                    if (resdt.Rows.Count > 0)
                    {
                        //lblUserName.Text = "User Name > " + resdt.Rows[0][0].ToString() + ": User ID > " + Session["name"].ToString();
                        lblUserName.Text = "Welcome : " + resdt.Rows[0][0].ToString();
                        hdnBusinessType.Value = resdt.Rows[0][2].ToString();
                        hdnRole.Value = resdt.Rows[0][3].ToString();
                    }
                    else
                    {
                        Response.Redirect("AccessDeniedPage.aspx");
                    }
                    con.Close();
                }
                else
                {
                    Response.Redirect("AccessDeniedPage.aspx");
                }
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }
        }
        #endregion

        #region DistErpLoad
        public void DistErpLoad()
        {
            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd1 = new SqlCommand("NonwisdomPulse_DistPrdMap_StckUpd_DrpDowns", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@DistCode", "");
                cmd1.Parameters.AddWithValue("@Action", "DistDrp");
                cmd1.ExecuteNonQuery();

                cmd1.CommandTimeout = 6000;

                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                resdt.Rows.Clear();
                da.Fill(resdt);
                DistributorErpId.DataSource = resdt;
                DistributorErpId.DataTextField = resdt.Columns["DistErpId"].ToString();
                DistributorErpId.DataValueField = resdt.Columns["DistCode"].ToString();
                DistributorErpId.DataBind();
                DistributorErpId.Items.Insert(0, new ListItem("DistributorErpId", ""));
                con.Close();
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }

        }
        #endregion

        #region PrdErpIdLoad
        public void PrdErpIdLoad()
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd1 = new SqlCommand("NonwisdomPulse_DistPrdMap_StckUpd_DrpDowns", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@DistCode", DistributorErpId.SelectedValue);
                cmd1.Parameters.AddWithValue("@Action", "PrdDrp");
                cmd1.ExecuteNonQuery();

                cmd1.CommandTimeout = 6000;
                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                ds1.Clear();
                da.Fill(ds1);

                DataTable dt = ds1.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    PrdErpId.DataSource = dt;
                    PrdErpId.DataBind();
                }
                else
                {
                    PrdErpId.DataSource = null;
                    PrdErpId.DataBind();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }
        }
        #endregion

        #region PrdErpId_RowDataBound
        protected void PrdErpId_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the Status value from the DataRow
                DataRowView drv = (DataRowView)e.Row.DataItem;
                int status = Convert.ToInt32(drv["Status"]);

                // Set the text color based on the Status value
                if (status == 0)
                {
                    e.Row.ForeColor = Color.Red;
                }
                else if (status == 1)
                {
                    e.Row.ForeColor = Color.Green;
                }
            }
        }
        #endregion

        #region btnSubmit_Click
        protected async void btnSubmit_Click(object sender, EventArgs e)
        {
            await BtnSubmit();
        }
        #endregion

        #region BtnSubmit
        public async Task BtnSubmit()
        {
            string distDrp = DistributorErpId.Text;
            string quantity = Quantity.Text;
            string unit = "pcs";

            try
            {
                string distributorErpId = DistributorErpId.SelectedValue;

                List<UpdateStockModel> selectedProductErpIds = new List<UpdateStockModel>();

                // Collect selected ProductErpIds
                foreach (GridViewRow row in PrdErpId.Rows)
                {
                    HtmlInputCheckBox chkBox = (HtmlInputCheckBox)row.FindControl("CheckBox1");
                    if (chkBox != null && chkBox.Checked)
                    {
                        anyCheckboxSelected = true;
                        //string productErpId = row.Cells[1].Text;
                        HiddenField hiddenPrdErpId = (HiddenField)row.FindControl("HiddenPrdErpId");
                        string productErpId = hiddenPrdErpId.Value;
                        selectedProductErpIds.Add(new UpdateStockModel
                        {
                            ProductERPId = productErpId,
                            Quantity = quantity,
                            Unit = unit
                        });
                    }
                }

                if (distDrp == "")
                {
                    showToast("Please select DistErpId", "toast-danger");
                    return;
                }
                else if (!anyCheckboxSelected)
                {
                    showToast("Please select atleast one Product ErpId", "toast-danger");
                    return;
                }
                else if (quantity == "")
                {
                    showToast("Please select quantity", "toast-danger");
                    return;
                }
                else if (!int.TryParse(quantity, out _))
                {
                    showToast("Please enter a valid numeric quantity", "toast-danger");
                    return;
                }

                if (selectedProductErpIds.Count > 0)
                {
                    // Call the API
                    await CallApiAsync(selectedProductErpIds);
                }
                else
                {
                    showToast("Please select at least one ProductErpId.", "toast-danger");
                }

                ClearForm();
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }
        }
        #endregion

        #region CallApiAsync
        private async Task CallApiAsync(List<UpdateStockModel> payload)
        {
            string distributorErpId = DistributorErpId.SelectedValue;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(50);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    client.BaseAddress = new Uri("https://api.fieldassist.in/api/V3/Distributor/UpdateStock/" + distributorErpId);
                    var authToken = Encoding.ASCII.GetBytes("Wipro_CC_Integration:pYbcDOXNmTDuT^)3iiG9");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var jsonPayload = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(client.BaseAddress, content);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JObject.Parse(responseContent);

                    var overallStatus = jsonResponse["Response"]?.ToString();
                    var message = "";

                    if (overallStatus == "Success")
                    {
                        message = "Product(s) mapped Successfully";
                        showToast(message, "toast-success");
                    }
                    else if (overallStatus == "Failure")
                    {
                        var validationMessage = jsonResponse["ResponseStatusCount"]["StatusMessage"].ToString();
                        message = validationMessage + ". Please contact team for further clarifications";
                        //message = message.Replace("'", "\\'");
                        showToast(message, "toast-danger");
                    }
                    else if (overallStatus == "PartialSuccess")
                    {
                        var validationMessage = jsonResponse["ResponseStatusCount"]["StatusMessage"].ToString();
                        message = validationMessage + ". Please contact team for further clarifications";
                        showToast(message, "toast-partial");
                    }


                    //variables to store in db
                    var responseList = jsonResponse["ResponseList"] as JArray;

                    if (responseList != null)
                    {
                        foreach (var item in responseList)
                        {
                            var prdErpId = item["ERPId"]?.ToString();
                            var prdNm = item["Message"]?.ToString().Replace("'", "");
                            var status = item["ResponseStatus"]?.ToString();
                            string distErpId = DistributorErpId.SelectedItem.Text;
                            string quantity = Quantity.Text;
                            string distcode = DistributorErpId.SelectedValue;

                            SaveToDatabasee(prdErpId, prdNm, status, distErpId, quantity, distcode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }
        }
        #endregion

        #region SaveToDatabasee
        public void SaveToDatabasee(string prdErpId, string prdNm, string status, string distErpId, string quantity, string distcode)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd1 = new SqlCommand("Nonwisdom_Distprdmap_UpdStck_Log", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@Process", "STKUPD");
                cmd1.Parameters.AddWithValue("@DistributorErpId", distErpId);
                cmd1.Parameters.AddWithValue("@productErpId", prdErpId);
                cmd1.Parameters.AddWithValue("@PrdNm", prdNm);
                cmd1.Parameters.AddWithValue("@Quantity", quantity);
                cmd1.Parameters.AddWithValue("@actionDrp", "");
                cmd1.Parameters.AddWithValue("@status", status);
                cmd1.Parameters.AddWithValue("@DistCode", distcode);
                cmd1.ExecuteNonQuery();

                cmd1.CommandTimeout = 6000;
                con.Close();
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }
        }
        #endregion

        #region ClearForm
        private void ClearForm()
        {
            DistributorErpId.ClearSelection();
            Quantity.Text = string.Empty;

            foreach (GridViewRow row in PrdErpId.Rows)
            {
                HtmlInputCheckBox chk = (HtmlInputCheckBox)row.FindControl("CheckBox1");
                chk.Checked = false;
            }
        }
        #endregion

        #region ToastNotification
        private void showToast(string message, string styleClass)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showToast", $"showToast('{message}', '{styleClass}');", true);
        }
        #endregion

        #region SelectedIndexChanged

        protected void DistributorErpId_SelectedIndexChanged(object sender, EventArgs e)
        {
            PrdErpIdLoad();
        }

        #endregion
    }
}