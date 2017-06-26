using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_wipDailyWip : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlFtyCd.DataSource = MESComment.MesRpt.GetFactoryCd();
            ddlFtyCd.DataTextField = "FACTORY_ID";
            ddlFtyCd.DataValueField = "FACTORY_ID";
            ddlFtyCd.DataBind();

            //Added by MunFoong on 2014.07.24, MES-139
            if (Request.QueryString["site"].ToString() == "PTX")
            {
                this.ddGroupName.Enabled = true;
                this.ddGroupName.DataSource = MESComment.wipDailySql.GetGroupName(Request.QueryString["site"].ToString());
                //this.ddGroupName.DataSource = MESComment.wipDailySql.GetGroupName("GEG");
                this.ddGroupName.DataTextField = "SYSTEM_KEY";
                this.ddGroupName.DataValueField = "SYSTEM_KEY";
                ddGroupName.DataBind();
            }
            else
            {
                this.ddGroupName.Enabled = false;
            }
            //End of added by MunFoong on 2014.07.24, MES-139
        }

        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                ddlFtyCd.SelectedValue = Request.QueryString["site"].ToString();
            }
            else
            {
                ddlFtyCd.SelectedValue = "GEG";
            }
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divBody.InnerHtml = "";
        divBody.InnerHtml += "<table width='100%' border='1' cellspacing='0' cellpadding='0'   style='font-size:12px;border-collapse:collapse'>";
        divBody.InnerHtml += "<tr>";
        divBody.InnerHtml += "<td rowspan='2' class='tr2style'>Buyer</td>";
        divBody.InnerHtml += "<td rowspan='2' class='tr2style'>S/C No</td>";
        divBody.InnerHtml += "<td rowspan='2' class='tr2style'>J/O No</td>";
        divBody.InnerHtml += "<td rowspan='2' class='tr2style'>Wash Type</td>";
        divBody.InnerHtml += "<td rowspan='2' class='tr2style'>Style No.</td>";
        divBody.InnerHtml += "<td rowspan='2' class='tr2style'>Gmt Del Date</td>";
        divBody.InnerHtml += "<td rowspan='2' class='tr2style'>Order Qty</td>";


        DataTable List = MESComment.wipDailySql.GetDailyWip(ddlGarmentType.SelectedItem.Value, ddlWashType.SelectedItem.Value, txtDate.Text, ddlFtyCd.SelectedItem.Value, ddGroupName.SelectedItem.Value); ;
        DataTable Title = MESComment.wipDailySql.GetDailyWipTitle(ddlGarmentType.SelectedItem.Value, txtDate.Text, ddlFtyCd.SelectedItem.Value);

        for (int i = 0; i < Title.Rows.Count; i++)
        {
            divBody.InnerHtml += "<td colspan='4' class='tr2style' align='center'>" + Title.Rows[i][0] + "</td>";
        }
        divBody.InnerHtml += "</tr><tr>";

        for (int i = 0; i < Title.Rows.Count; i++)
        {
            divBody.InnerHtml += "<td class='tr2style'>Day</td><td class='tr2style'>Month</td><td class='tr2style'>JO TTL</td><td class='tr2style'>Status</td>";
        }
        divBody.InnerHtml += "</tr>";

        string JoNo = "";
        int index=0;
        for (int i = 0; i < List.Rows.Count; i++)
        {
            if (JoNo != List.Rows[i]["JO_NO"].ToString())
            {
                index = 0;
                divBody.InnerHtml += "<tr><td>" + List.Rows[i]["BUYER"] + "</td><td >" + List.Rows[i]["SC_NO"] + "</td><td >" + List.Rows[i]["JO_NO"] + "</td>";
                divBody.InnerHtml += "<td>" + List.Rows[i]["WASH_TYPE_CD"] + "</td><td >" + List.Rows[i]["STYLE_NO"] + "</td><td >" + List.Rows[i]["GMT_DEL_DATE"] + "</td><td >" + List.Rows[i]["ORDER_QTY"] + "</td>";
                for (int j = i; j < List.Rows.Count; j++)
                {
                    if (List.Rows[j]["JO_NO"].ToString() == List.Rows[i]["JO_NO"].ToString())
                    {
                        for (int m = index; m < Title.Rows.Count; m++)
                        {
                            if (List.Rows[j]["PROCESS_CD"].ToString() == Title.Rows[m][0].ToString())
                            {
                                divBody.InnerHtml += "<td >" + List.Rows[j]["DAILY"] + "</td><td >" + List.Rows[j]["MONTHLY"] + "</td><td >" + List.Rows[j]["JO_TTL"] + "</td><td >" + List.Rows[j]["JO_CLEAR_FLAG"] + "</td>";
                                if (j != List.Rows.Count - 1)
                                {
                                    if (List.Rows[j + 1]["JO_NO"].ToString() == List.Rows[i]["JO_NO"].ToString())
                                    {
                                        index = m+1;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                divBody.InnerHtml += "<td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>";
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                divBody.InnerHtml += "</tr>";
            }
            JoNo = List.Rows[i]["JO_NO"].ToString();
        }
        divBody.InnerHtml += "</table>";
    }
}
