using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Net.Http.Headers;
using System.Net.Http;
using DistPrdMap_UpdStck.Models;
using System.Drawing;
using System.Net;

namespace DistPrdMap_UpdStck
{
    public partial class DistPrdMap : System.Web.UI.Page
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
                SqlCommand cmd1 = new SqlCommand("Nonwisdom_DistPrdMap_DrpDowns", con);
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
                DistributorErpId.DataValueField = resdt.Columns["DistErpId"].ToString();
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
                SqlCommand cmd1 = new SqlCommand("Nonwisdom_DistPrdMap_DrpDowns", con);
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

        #region btnAdd_Click
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            AddSelectedMappings();
        }
        #endregion

        #region AddSelectedMappings
        private void AddSelectedMappings()
        {
            string distributorErpId = DistributorErpId.SelectedValue;
            string action = ActionDrp.SelectedValue;
            List<DistributorToProductMapping> selectedProductErpIds = new List<DistributorToProductMapping>();

            foreach (GridViewRow row in PrdErpId.Rows)
            {
                HtmlInputCheckBox chkBox = (HtmlInputCheckBox)row.FindControl("CheckBox1");
                if (chkBox != null && chkBox.Checked)
                {
                    anyCheckboxSelected = true;
                    HiddenField hiddenPrdErpId = (HiddenField)row.FindControl("HiddenPrdErpId");
                    string productErpId = hiddenPrdErpId.Value;
                    selectedProductErpIds.Add(new DistributorToProductMapping
                    {
                        DistributorErpId = distributorErpId,
                        ProductErpId = productErpId,
                        Action = action
                    });
                }
            }

            if (distributorErpId == "")
            {
                showToast("Please select DistErpId", "toast-danger");
                return;
            }
            else if (!anyCheckboxSelected)
            {
                showToast("Please select atleast one Product ErpId", "toast-danger");
                return;
            }
            else if (action == "")
            {
                showToast("Please select Action Type Attach/Detach", "toast-danger");
                return;
            }

            if (selectedProductErpIds.Count > 0)
            {
                // Store the selected mappings in a hidden field
                var existingMappings = JsonConvert.DeserializeObject<List<DistributorToProductMapping>>(hiddenSelectedMappings.Value) ?? new List<DistributorToProductMapping>();
                existingMappings.AddRange(selectedProductErpIds);
                hiddenSelectedMappings.Value = JsonConvert.SerializeObject(existingMappings);
            }
            showToast("DistErpId : " + distributorErpId + " added", "toast-success");
            ClearForm();
        }
        #endregion

        #region BtnSubmit
        public async Task BtnSubmit()
        {
            try
            {
                var allMappings = hiddenSelectedMappings.Value;
                //var groupedMappings = allMappings.GroupBy(m => m.DistributorErpId).ToList();

                if (!string.IsNullOrEmpty(allMappings))
                {
                    var allMappingsJson = JsonConvert.DeserializeObject<List<DistributorToProductMapping>>(allMappings);

                    foreach (var mapping in allMappingsJson)
                    {
                        var payload = new List<DistributorToProductMapping> { mapping };
                        await CallApiAsync(payload);
                    }
                }

                ClearForm();
                hiddenSelectedMappings.Value = string.Empty;
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }
        }
        #endregion

        #region CallApi
        private async Task CallApiAsync(List<DistributorToProductMapping> payload)
        {
            try
            {
                //HttpClient client = new HttpClient();
                //client.Timeout = TimeSpan.FromMinutes(50);

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(50);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    client.BaseAddress = new Uri("https://api.fieldassist.in/api/V3/Distributor/DistributorToProductMapping");
                    var authToken = Encoding.ASCII.GetBytes("Wipro_CC_Integration:pYbcDOXNmTDuT^)3iiG9");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    
                    var jsonPayload = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(client.BaseAddress, content);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    //var jsonResponse = JsonConvert.DeserializeObject<JObject>(responseContent);
                    //var message = jsonResponse["ResponseList"]?[0]?["Message"]?.ToString();
                    //message = message.Replace("'", "\\'");

                    //if (response.IsSuccessStatusCode)
                    //{
                    //    showToast(message, "toast-success");
                    //}
                    //else
                    //{
                    //    showToast(message, "toast-danger");
                    //}


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

                    var responseList = jsonResponse["ResponseList"];


                    //variables to store in db
                    foreach (var item in responseList)
                    {
                        var distributorErpId = item["ERPId"]?.ToString();
                        var productErpId = item["Message"].ToString().Split(new string[] { "ProductErpId: '" }, StringSplitOptions.None)[1].Split('\'')[0];
                        var status = item["ResponseStatus"]?.ToString();
                        string actionDrp = Session["ActionDrpVal"].ToString();

                        // Save distributorErpId, productErpId, and status to your database tables
                        // Example code for saving to the database:
                        SaveToDatabase(distributorErpId, productErpId, status, actionDrp);
                    }
                }
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }
        }
        #endregion

        #region SaveToDatabase
        public void SaveToDatabase(string distributorErpId, string productErpId, string status, string actionDrp)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd1 = new SqlCommand("Nonwisdom_Distprdmap_UpdStck_Log", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@Process", "DISTPRDMAP");
                cmd1.Parameters.AddWithValue("@DistributorErpId", distributorErpId);
                cmd1.Parameters.AddWithValue("@productErpId", productErpId);
                cmd1.Parameters.AddWithValue("@PrdNm", "");
                cmd1.Parameters.AddWithValue("@Quantity", "");
                cmd1.Parameters.AddWithValue("@actionDrp", actionDrp);
                cmd1.Parameters.AddWithValue("@status", status);
                cmd1.Parameters.AddWithValue("@DistCode", "");
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
            ActionDrp.ClearSelection();

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
        protected void ActionDrp_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ActionDrpVal"] = ActionDrp.SelectedItem.Text;
        }

        protected void DistributorErpId_SelectedIndexChanged(object sender, EventArgs e)
        {
            PrdErpIdLoad();
        }

        #endregion
    }
}