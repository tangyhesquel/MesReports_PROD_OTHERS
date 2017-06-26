using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;

public partial class Reports_DailyTransReport : pPage
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


            
            ddlProdFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
            ddlProdFactory.DataTextField = "FACTORY_ID";
            ddlProdFactory.DataValueField = "FACTORY_ID";
            ddlProdFactory.DataBind();

            ddlProcessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
            ddlProcessType.DataTextField = "PROCESS_TYPE_ID";
            ddlProcessType.DataValueField = "PROCESS_TYPE_VALUE";
            ddlProcessType.DataBind();

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

            reportDiv.Visible = false;
            if (Request.QueryString["site"] != null)
            {
                if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
                {
                    ddlFactoryCode.SelectedValue = Request.QueryString["site"].ToString();
                }
                else
                {
                    ddlFactoryCode.SelectedValue = "GEG";
                }
                GetProcess();
                GetProdLine();
            }
        }

    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divBody.InnerHtml = "";
        double WIPTotal = 0;
        double ttlqtyi = 0;
        double ttlqtyl = 0;
        double ttlqtyo = 0;
        double ttlqtyu = 0;
        double ttlqtyr = 0;
        string strTitle = "";
        string Line = "";
        Random ro = new Random(1000);
        DbConnection MESConn = MESComment.DBUtility.GetConnection("MES");
        strTitle = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();

        reportDiv.Visible = true;
        divBody.InnerHtml += "<table width='100%' border='1' cellspacing='0' cellpadding='0' style='font-size:12px;border-collapse:collapse'>";
        divBody.InnerHtml += "<tr class='tr2style'><td width='200' rowspan=2 align='center'>BuyerName</td><td width='80' rowspan=2 align='center'>Del.Date</td><td width='100' rowspan=2 align='center'>Wash Type</td><td width='100' rowspan=2 align='center'>JO No</td><td width='80' rowspan=2 align='center'>Ord.Pcs</td>";
        divBody.InnerHtml += "<td width='80' rowspan=2 align='center'>CutPcs</td><td width='80' rowspan=2 align='center'>SAM</td><td width='140' colspan=2 align='center'>InComing Pcs</td><td width='140' colspan=2 align='center'>Last Opn</td><td width='140' colspan=2 align='center'>Transfer Pcs</td>";
        divBody.InnerHtml += "<td width='140' colspan=2>UnAccTable</td><td width='140' colspan=2>Repairing</td><td width='80' align='center' rowspan=2>WIP</td></tr>";
        divBody.InnerHtml += "<tr class='tr2style'><td width='70' align='center'>Day</td><td width='70' align='center'>Acc</td><td width='70' align='center'>Day</td><td width='70' align='center'>Acc</td><td width='70' align='center'>Day</td><td width='70' align='center'>Acc</td>";
        divBody.InnerHtml += "<td width='70' align='center'>Out</td><td width='70' align='center'>Acc</td><td width='70' align='center'>Out</td><td width='70' align='center'>Acc</td></tr>";
        //Modified by MunFoong on 2014.07.24, MES-139
        DataTable dt1 = MESComment.proDailyTransReport.GetPrdDailyOutput(ddGroupName.SelectedItem.Value, ddlFactoryCode.SelectedItem.Value, ddlGarmentType.SelectedItem.Value, ddlWashType.SelectedItem.Value, txtStartDate.Text, ddlprocessCd.SelectedItem.Value, ddlProcessType.SelectedItem.Value, ddlProdFactory.SelectedItem.Value, ddlProductionLine.SelectedItem.Value, strTitle, MESConn);

        for (int i = 0; i < dt1.Rows.Count; i++)
        {
            if (Line != dt1.Rows[i][0].ToString())
            {
                if (i != 0)
                {
                    divBody.InnerHtml += "<tr   class='tr2style'><td></td><td></td><td></td><td align='left' colspan=4>Sub Total:</td>";
                    divBody.InnerHtml += "<td align='right'>" + ttlqtyi + "</td><td></td>";
                    divBody.InnerHtml += "<td align='right'>" + ttlqtyl + "</td><td></td>";
                    divBody.InnerHtml += "<td align='right'>" + ttlqtyo + "</td><td></td>";
                    divBody.InnerHtml += "<td align='right'>" + ttlqtyu + "</td><td></td>";
                    divBody.InnerHtml += "<td align='right'>" + ttlqtyr + "</td><td></td>";
                    divBody.InnerHtml += "<td align='right'>" + WIPTotal + "</td>";
                    divBody.InnerHtml += "</tr>";
                }
                divBody.InnerHtml += "<tr  bgcolor='#f2f2f2'><td align='left' colspan=18><b>" + dt1.Rows[i][0] + "</b></td></tr>";
                ttlqtyi = 0;
                ttlqtyl = 0;
                ttlqtyo = 0;
                ttlqtyu = 0;
                ttlqtyr = 0;
                WIPTotal = 0;
            }
            divBody.InnerHtml += "<tr>";
            for (int j = 1; j < 19; j++)
            {
                if (j < 4)
                {
                    divBody.InnerHtml += "<td align='left'>" + dt1.Rows[i][j] + "</td>";
                }
                else
                {
                    divBody.InnerHtml += "<td align='right'>" + dt1.Rows[i][j] + "</td>";
                }
            }
            divBody.InnerHtml += "</tr>";
            ttlqtyi += double.Parse(dt1.Rows[i][8].ToString());
            ttlqtyl += double.Parse(dt1.Rows[i][10].ToString());
            ttlqtyo += double.Parse(dt1.Rows[i][12].ToString());
            ttlqtyu += double.Parse(dt1.Rows[i][14].ToString());
            ttlqtyr += double.Parse(dt1.Rows[i][16].ToString());
            WIPTotal += double.Parse(dt1.Rows[i][18].ToString());
            Line = dt1.Rows[i][0].ToString();
        }
        divBody.InnerHtml += "<tr   class='tr2style'><td></td><td></td><td></td><td align='left' colspan=4>Sub Total:</td>";
        divBody.InnerHtml += "<td align='right'>" + ttlqtyi + "</td><td></td>";
        divBody.InnerHtml += "<td align='right'>" + ttlqtyl + "</td><td></td>";
        divBody.InnerHtml += "<td align='right'>" + ttlqtyo + "</td><td></td>";
        divBody.InnerHtml += "<td align='right'>" + ttlqtyu + "</td><td></td>";
        divBody.InnerHtml += "<td align='right'>" + ttlqtyr + "</td><td></td>";
        divBody.InnerHtml += "<td align='right'>" + WIPTotal + "</td>";
        divBody.InnerHtml += "</tr>";
        divBody.InnerHtml += "</table>";
    }

    protected void GetProcess()
    {
        ddlprocessCd.DataSource = MESComment.MesRpt.GetProcessCd(ddlFactoryCode.SelectedItem.Value, ddlGarmentType.SelectedItem.Value);
        ddlprocessCd.DataTextField = "NM";
        ddlprocessCd.DataValueField = "PRC_CD";
        ddlprocessCd.DataBind();
    }

    protected void ddlFtyCd_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetProcess();
    }
    protected void ddlGMT_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetProcess();
    }
    protected void ddlprocessCd_SelectedIndexChanged(object sender, EventArgs e)
    {

        GetProdLine();

    }

    protected void GetProdLine()
    {

        ddlProductionLine.DataSource = MESComment.MesRpt.GetPrdLine(ddlFactoryCode.SelectedItem.Value, ddlprocessCd.SelectedItem.Value, ddlGarmentType.SelectedItem.Value);

        ddlProductionLine.DataTextField = "PRODUCTION_LINE_NAME";

        ddlProductionLine.DataValueField = "PRODUCTION_LINE_CD";

        ddlProductionLine.DataBind();

    }



}
