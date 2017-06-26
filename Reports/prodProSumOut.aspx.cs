using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_prodProSumOut : pPage
{
    string userid = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        userid = Request.QueryString["userid"];
        if (!IsPostBack)
        {
            ddlFactoryCd.DataSource = MESComment.MesRpt.GetFactoryList(userid);
            ddlFactoryCd.DataBind();

            ddlProductionFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
            ddlProductionFactory.DataTextField = "FACTORY_ID";
            ddlProductionFactory.DataValueField = "FACTORY_ID";
            ddlProductionFactory.DataBind();

            ddlProcessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
            ddlProcessType.DataTextField = "PROCESS_TYPE_ID";
            ddlProcessType.DataValueField = "PROCESS_TYPE_VALUE";
            ddlProcessType.DataBind();

            if (Request.QueryString["site"] != null)
            {
                if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
                {
                    ddlFactoryCd.SelectedValue = Request.QueryString["site"].ToString();
                }
                else
                {
                    ddlFactoryCd.SelectedValue = "GEG";
                }
                ddlProcessCd.DataSource = MESComment.MesRpt.GetProcessCode(ddlFactoryCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value);
                ddlProcessCd.DataBind();
            }
        }
       
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divDetail.InnerHtml = "";
        DataTable dtDetail = MESComment.MesRpt.GetProdProcessSummaryOutputList(ddlFactoryCd.SelectedItem.Value,txtStartDate.Text,txtEndDate.Text,ddlProcessCd.SelectedItem.Value,ddlProcessType.SelectedItem.Value,ddlProductionFactory.SelectedItem.Value,txtJoNo.Text.Trim(),ddlgarmentType.SelectedItem.Value,ddlWashtype.SelectedItem.Value);
        foreach (DataRow row in dtDetail.Rows)
        {
            divDetail.InnerHtml += "<tr>";
            divDetail.InnerHtml += "<td>" + row["CUSTOMER_NAME"] + "</td>";
            divDetail.InnerHtml += "<td>" + row["SC_NO"] + "</td>";
            divDetail.InnerHtml += "<td>" + row["JOB_ORDER_NO"] + "</td>";
            divDetail.InnerHtml += "<td>" + row["ORDER_QTY"] + "</td>";
            divDetail.InnerHtml += "<td>" + row["STYLE_NO"] + "</td>";
            divDetail.InnerHtml += "<td>" + row["STYLE_DESC"] + "</td>";
            divDetail.InnerHtml += "<td>" + row["WASH_TYPE_CD"] + "</td>";
            divDetail.InnerHtml += "<td>" + double.Parse(row["SAH"].ToString()).ToString("#,##0.00") + "</td>";
            divDetail.InnerHtml += "<td>" + row["BPD"] + "</td>";
            divDetail.InnerHtml += "<td>" + row["PPCD"] + "</td>";
            divDetail.InnerHtml += "<td>" + row["ROUTE_TYPE"] + "</td>";
            divDetail.InnerHtml += "<td>" + row["PROCESS_CD"] + "</td>";
            //divDetail.InnerHtml += "<td>" + row["GARMENT_TYPE"] + "</td>"; //Added By ZouShiChang ON 2013.08.26 MES 024
            divDetail.InnerHtml += "<td>" + row["OUTPUT_QTY"] + "</td>";

            divDetail.InnerHtml += "</tr>";
        }

    }
    protected void ddlgarmentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlProcessCd.DataSource = MESComment.MesRpt.GetProcessCode(ddlFactoryCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value);
        ddlProcessCd.DataBind();
    }
}
