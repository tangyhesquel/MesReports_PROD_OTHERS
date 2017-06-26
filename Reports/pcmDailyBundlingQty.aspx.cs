using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_pcmDailyBundlingQty : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlFtyCd.DataSource = MESComment.MesRpt.GetFactoryCd();
            ddlFtyCd.DataTextField = "FACTORY_ID";
            ddlFtyCd.DataValueField = "FACTORY_ID";
            ddlFtyCd.DataBind();
        }
        if (Request.QueryString["site"] != null)
        {
            ddlFtyCd.SelectedValue = Request.QueryString["site"].ToString();
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divBody.InnerHtml = "";
        divBody.InnerHtml += "<table width='100%' border='1' cellspacing='0' cellpadding='0' style='font-size: 12px;border-collapse: collapse'><tr><td class='tr2style'>Trx Date</td><td class='tr2style'>Job Order No</td><td class='tr2style'>Cut Line</td><td class='tr2style'>Cut Lot No</td><td class='tr2style'>Order Qty</td><td class='tr2style'>Today's Bundling Qty</td><td class='tr2style'>Total Bundling Qty</td></tr>";
        DataTable dt = MESComment.pcmDailyBundlingQty.GetDailyBundlingQty(ddlFtyCd.SelectedItem.Value, txtJoNo.Text.Trim(), txtCutLine.Text.Trim(), txtStartDate.Text, txtEndDate.Text);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            divBody.InnerHtml += "<tr><td >&nbsp;" + dt.Rows[i]["TRX_DATE"] + "</td><td >&nbsp;" + dt.Rows[i]["JOB_ORDER_NO"] + "</td><td >&nbsp;" + dt.Rows[i]["CUT_LINE"] + "</td><td >&nbsp;" + dt.Rows[i]["CUTLOT_NO"] + "</td>";
            divBody.InnerHtml += "<td >&nbsp;" + dt.Rows[i]["ORDER_QTY"] + "</td><td >&nbsp;" + dt.Rows[i]["TODAY_BUNDLING_QTY"] + "</td><td >&nbsp;" + dt.Rows[i]["TOTAL_BUNDLING_QTY"] + "</td></tr>	";
        }
        divBody.InnerHtml += "<tr><td colspan='7'>&nbsp;</td></tr>";
        DataTable dt1 = MESComment.pcmDailyBundlingQty.GetDailyBundlingQtyLineSummary(ddlFtyCd.SelectedItem.Value, txtJoNo.Text.Trim(), txtCutLine.Text.Trim(), txtStartDate.Text, txtEndDate.Text);
        for (int i = 0; i < dt1.Rows.Count; i++)
        {
            divBody.InnerHtml += "<tr><td colspan='4' align='right' class='tr2style'>" + dt1.Rows[i]["CUT_LINE"] + "</td><td>" + dt1.Rows[i]["ORDER_QTY"] + "</td><td>" + dt1.Rows[i]["TODAY_BUNDLING_QTY"] + "</td><td>" + dt1.Rows[i]["TOTAL_BUNDLING_QTY"] + "</td></tr>";
        }
        divBody.InnerHtml += "<tr><td colspan='7'>&nbsp;</td></tr>";
        DataTable dt2 = MESComment.pcmDailyBundlingQty.GetDailyBundlingQtySummary(ddlFtyCd.SelectedItem.Value, txtJoNo.Text.Trim(), txtCutLine.Text.Trim(), txtStartDate.Text, txtEndDate.Text);
        for (int i = 0; i < dt2.Rows.Count; i++)
        {
            divBody.InnerHtml += "<tr><td colspan='4' align='right' class='tr2style'>Total</td><td>" + dt2.Rows[i]["ORDER_QTY"] + "</td><td>" + dt2.Rows[i]["TODAY_BUNDLING_QTY"] + "</td><td>" + dt2.Rows[i]["TOTAL_BUNDLING_QTY"] + "</td></tr>";
        }
        divBody.InnerHtml += "</table>";
    }
}
