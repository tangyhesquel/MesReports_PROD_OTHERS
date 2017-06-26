using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_JOProcessReport : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlFactoryCd.DataSource = MESComment.MesRpt.GetFactoryCd();
            ddlFactoryCd.DataBind();
        }
        if (Request.QueryString["site"] != "")
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                ddlFactoryCd.SelectedValue = Request.QueryString["site"].ToString();
            }
            else
            {
                ddlFactoryCd.SelectedValue = "GEG";
            }

        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divBody.InnerHtml = "";
        DataTable dt1 = MESComment.MesRpt.GetOsJoProcess(txtstartDate.Text,txtendDate.Text,ddlFactoryCd.SelectedItem.Value,txtContractNo.Text.Trim());
        foreach (DataRow row in dt1.Rows)
        {
            divBody.InnerHtml += "<tr>";
            divBody.InnerHtml += "<td>" + row["JOB_ORDER_NO"] + "</td>";
            divBody.InnerHtml += "<td>" + row["CONTRACT_NO"] + "</td>";
            divBody.InnerHtml += "<td>" + row["SUB_CONTRACT_PRICE"] + "</td>";
            divBody.InnerHtml += "<td>" + row["EMB_PRICE"] + "</td>";
            divBody.InnerHtml += "<td>" + row["PRINT_PRICE"] + "</td>";
            divBody.InnerHtml += "<td>" + row["WASH_PRICE"] + "</td>";
            divBody.InnerHtml += "<td>" + row["PROCESS_CD"] + "</td>";
            divBody.InnerHtml += "<td>" + row["SUBCONTRACTOR_NAME"] + "</td>";
            divBody.InnerHtml += "<td>" + row["PROCESS_REMARK"] + "</td>";
            divBody.InnerHtml += "<td>" + row["ISSUER_DATE"] + "</td>";
            divBody.InnerHtml += "</tr>";
        }
    }
}
