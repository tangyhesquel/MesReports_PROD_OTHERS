using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_proCycleSummary : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["userId"] != null)
            {
                ddlFactoryCode.DataSource = MESComment.MesRpt.GetFactoryList(Request.QueryString["userId"].ToString());
            }
            else
            {
                ddlFactoryCode.DataSource = MESComment.MesRpt.GetFactoryList("");
            }
            ddlFactoryCode.DataValueField = "DEPARTMENT_ID";
            ddlFactoryCode.DataTextField = "DEPARTMENT_ID";
            ddlFactoryCode.DataBind();
            reportDiv.Visible = false;
        }
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString() != "DEV")
            {
                ddlFactoryCode.SelectedValue = Request.QueryString["site"].ToString();
            }
            else
            {
                ddlFactoryCode.SelectedValue = "GEG";
            }
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divBody.InnerHtml = "";
        float ctTotal = 0;
        DataTable dt = MESComment.proCycleSummary.GetProCycleSummaryHeardList(ddlFactoryCode.SelectedItem.Value);
        if (dt.Rows.Count > 0)
        {
            reportDiv.Visible = true;
            divBody.InnerHtml += "<table width='100%' border='1' cellspacing='0' cellpadding='0' style='font-size:12px;border-collapse:collapse'><tr>";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                divBody.InnerHtml += "<td class='tr2style' bgcolor='#efefe7' align='center'>" + dt.Rows[i]["key_cd"] + "</td>";
            }
            divBody.InnerHtml += "<td class='tr2style' bgcolor='#efefe7' rowspan='2' align='center'>Total CT</td></tr><tr>";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                divBody.InnerHtml += "<td class='tr2style' bgcolor='#efefe7' align='center'>" + dt.Rows[i]["key_name"] + "</td>";
            }
            divBody.InnerHtml += "</tr><tr>";
            DataTable dt1 = MESComment.proCycleSummary.GetProCycleSummaryList(ddlFactoryCode.SelectedItem.Value, ddlGarmentType.SelectedItem.Value, ddlOutsourceType.SelectedItem.Value, ddlWashType.SelectedItem.Value, txtStartDate.Text, txtEndDate.Text, cbCheck.Checked);
           
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                ctTotal = ctTotal + float.Parse(dt1.Rows[i]["CT"].ToString());
            }
         
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                divBody.InnerHtml += "<td align='center'>" + dt1.Rows[i]["CT"] + "</td>";
            }
            divBody.InnerHtml += "<td align='center'>" + ctTotal + "</td></tr></table>";
        }
    }
}
