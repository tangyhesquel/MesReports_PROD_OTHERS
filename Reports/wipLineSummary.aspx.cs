using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;

public partial class Reports_wipDaily : pPage
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
        string strContent = "";
        string strTitle = "";
        Random ro = new Random(1000);
        DbConnection MESConn = MESComment.DBUtility.GetConnection("MES");
        strContent = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
        strTitle = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
        MESComment.MesRpt.SP_Pro_DropTmpTable(strContent, strTitle);
        DataTable TTitle = MESComment.MesRpt.GetWIPLineResultList(ddlFtyCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value, "", "", txtStartDate.Text, txtToDate.Text, ddlprocessCd.SelectedItem.Value, ddlprodLine.SelectedItem.Value, strContent, strTitle, MESConn);
        divBody.InnerHtml = "";
        divBody.InnerHtml += "<table border='1' cellspacing='1' cellpadding='1' style='font-size:12px;border-collapse:collapse'>";
        divBody.InnerHtml += "<tr class='tr2style' align='center'>";
        divBody.InnerHtml += "<td width='200' >&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </td>";

        for (int i = 0; i < TTitle.Rows.Count; i++)
        {
            divBody.InnerHtml += "<td >" + TTitle.Rows[i]["productionline"] + "</td>";
        }

        divBody.InnerHtml += "</tr>";

        DataTable Detail = MESComment.MesRpt.GetDailyOutputByJOSectionLineSummary(strContent, strTitle);
        string color = "";
        // double sahprod = 0;
        double[] ttlqtya = new double[TTitle.Rows.Count];
        double[] ttlqtyb = new double[TTitle.Rows.Count];
        double[] ttlqtyc = new double[TTitle.Rows.Count];
        double[] ttlqtyd = new double[TTitle.Rows.Count];
        for (int i = 0; i < Detail.Rows.Count; i++)
        {
            if (i % 2 == 0)
            {
                color = "white";
            }
            else
            {
                color = "#f2f2f2";
            }
            divBody.InnerHtml += "<tr  bgcolor=" + color + ">";
            if (i == 0 || i % 4 == 0)
            {
                divBody.InnerHtml += "<td rowspan=5 width='200'> " + Convert.ToDateTime(Detail.Rows[i]["TRXDATE"].ToString()).ToString("d") + "</td>";
            }
            for (int j = 0; j < TTitle.Rows.Count; j++)
            {

                divBody.InnerHtml += "<td width='200' align='right'>" + Detail.Rows[i][TTitle.Rows[j]["productionline"].ToString()] + "</td>";
                if (Detail.Rows[i][TTitle.Rows[j]["productionline"].ToString()].ToString() != "&nbsp;")
                {
                    if (Detail.Rows[i]["DTYPE"].ToString() == "1")
                    {
                        ttlqtya[j] += double.Parse(Detail.Rows[i][TTitle.Rows[j]["productionline"].ToString()].ToString());
                    }
                    if (Detail.Rows[i]["DTYPE"].ToString() == "2")
                    {
                        ttlqtyb[j] += double.Parse(Detail.Rows[i][TTitle.Rows[j]["productionline"].ToString()].ToString());
                    }
                    if (Detail.Rows[i]["DTYPE"].ToString() == "3")
                    {
                        ttlqtyc[j] += double.Parse(Detail.Rows[i][TTitle.Rows[j]["productionline"].ToString()].ToString());
                    }
                    if (Detail.Rows[i]["DTYPE"].ToString() == "4")
                    {
                        ttlqtyd[j] += double.Parse(Detail.Rows[i][TTitle.Rows[j]["productionline"].ToString()].ToString());
                    }
                }
            }
            divBody.InnerHtml += "</tr>";
            if (Detail.Rows[i]["DTYPE"].ToString() == "4")
            {
                divBody.InnerHtml += "<tr ><td colspan='" + TTitle.Rows.Count + "'>&nbsp;<br/></td></tr>";
            }

        }

        for (int k = 0; k < 4; k++)
        {
            if (k % 2 == 0)
            {
                color = "white";
            }
            else
            {
                color = "#f2f2f2";
            }
            divBody.InnerHtml += "<tr  bgcolor=" + color + ">";

            if (k == 0)
            {
                divBody.InnerHtml += "<td width='200'> Total Pcs</td>";
            }
            if (k == 1)
            {
                divBody.InnerHtml += "<td width='200'> Total (DZ)</td>";
            }
            if (k == 2)
            {
                divBody.InnerHtml += "<td width='200'> SAM Pcs Prd</td>";
            }
            if (k == 3)
            {
                divBody.InnerHtml += "<td width='200'> SAH Prd</td>";
            }
            for (int j = 0; j < TTitle.Rows.Count; j++)
            {
                if (k == 0)
                {
                    divBody.InnerHtml += "<td width='200' align='right'> " + ttlqtya[j] + "</td>";
                }
                if (k == 1)
                {
                    divBody.InnerHtml += "<td width='200' align='right'> " + ttlqtyb[j] + "</td>";
                }
                if (k == 2)
                {
                    divBody.InnerHtml += "<td width='200' align='right'> " + ttlqtyc[j] + "</td>";
                }
                if (k == 3)
                {
                    divBody.InnerHtml += "<td width='200' align='right'> " + ttlqtyd[j] + "</td>";
                }

            }
            divBody.InnerHtml += "</tr>";
        }
        divBody.InnerHtml += "</table>";

    }




    protected void ddlFtyCd_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlprocessCd.DataSource = MESComment.MesRpt.GetProcessCd(ddlFtyCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value);
        ddlprocessCd.DataTextField = "NM";
        ddlprocessCd.DataValueField = "PRC_CD";
        ddlprocessCd.DataBind();
    }
    protected void ddlGMT_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlprocessCd.DataSource = MESComment.MesRpt.GetProcessCd(ddlFtyCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value);
        ddlprocessCd.DataTextField = "NM";
        ddlprocessCd.DataValueField = "PRC_CD";
        ddlprocessCd.DataBind();
    }
    protected void ddlprocessCd_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlprodLine.DataSource = MESComment.MesRpt.GetPrdLine(ddlFtyCd.SelectedItem.Value, ddlprocessCd.SelectedItem.Value,ddlgarmentType.SelectedItem.Value);
        ddlprodLine.DataTextField = "PRODUCTION_LINE_NAME";
        ddlprodLine.DataValueField = "PRODUCTION_LINE_CD";
        ddlprodLine.DataBind();
    }
}
