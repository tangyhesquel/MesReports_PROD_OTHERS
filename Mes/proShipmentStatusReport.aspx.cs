using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;

public partial class Reports_proShipmentStatusReport : pPage
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


            reportDiv.Visible = false;
            if (Request.QueryString["site"] != null)
            {
                if (Request.QueryString["site"].ToUpper().ToString().Trim().Equals("DEV"))
                {
                    ddlFactoryCode.SelectedValue = "GEG";
                }
                else
                {
                    ddlFactoryCode.SelectedValue = Request.QueryString["site"].ToUpper().ToString().Trim();
                }
                GetProcess();
            }
        }

    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.lblMessage.Visible = false;
        string StartDate = txtStartDate.Text.Trim();
        string EndDate = txtEndDate.Text.Trim();

        if (StartDate.Length > 0 && EndDate.Length > 0)
        {
            int SDate = Convert.ToInt32(StartDate.Replace("-", ""));
            int EDate = Convert.ToInt32(EndDate.Replace("-", ""));
            if (EDate < SDate)
            {
                this.lblMessage.Text = "Date From cannot after Date To ! ";
                this.lblMessage.Visible = true;
                return;
            }
        }

        divBody.InnerHtml = "";
        double WIPTotal = 0;
        double ttlqtyi = 0;
        double ttlqtyl = 0;
        double ttlqtyo = 0;
        double ttlqtyu = 0;
        double ttlqtyr = 0;
        double ttlqtyWash = 0;
        double ttlqtyQc = 0;
        string strTitle = "";
        // string Line = "";
        Random ro = new Random(1000);
        DbConnection MESConn = MESComment.DBUtility.GetConnection("MES");
        strTitle = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();

        //DataTable dt1 = MESComment.MesRpt.GetPrdShipmentStatus(ddlFactoryCode.SelectedItem.Value, txtStartDate.Text, txtEndDate.Text, ddlprocessCd.SelectedItem.Value, ddlProcessType.SelectedItem.Value, ddlProdFactory.SelectedItem.Value, strTitle, MESConn);
        //DataTable dt1 = MESComment.MesRpt.GetPrdShipmentStatus(ddlFactoryCode.SelectedItem.Value, txtStartDate.Text, txtEndDate.Text, ddlGarmenType.SelectedItem.Value, ddlprocessCd.SelectedItem.Value, ddlProcessType.SelectedItem.Value, ddlProdFactory.SelectedItem.Value, strTitle, MESConn);
        DataTable dt1 = MESComment.proShipmentStatusReport.GetPrdShipmentStatus(ddlFactoryCode.SelectedItem.Value, txtStartDate.Text, txtEndDate.Text, ddlGarmenType.SelectedItem.Value, ddlprocessCd.SelectedItem.Value, ddlProcessType.SelectedItem.Value, ddlProdFactory.SelectedItem.Value, strTitle, MESConn);
        if (dt1.Rows.Count <= 0)
        {
            reportDiv.Visible = false;
            this.lblMessage.Text = "NO DATA.";
            this.lblMessage.Visible = true;
            return;
        }

        reportDiv.Visible = true;
        divBody.InnerHtml += "<table width='100%' border='1' cellspacing='0' cellpadding='0' style='font-size:12px;border-collapse:collapse'>";
        divBody.InnerHtml += "<tr class='tr2style'><td width='70' align='left'>Sew Line</td>";
        //Modified By Zikuan - MES-107 Add ShipMode column
        //Modified by Jin Song - MES128 Add Buyer PO and Style No column
        divBody.InnerHtml += "<td width='200' align='left'>Buyer</td><td width='100' align='left'>JO No</td><td width='100' align='left'>Buyer PO</td><td width='100' align='left'>Style No</td><td width='60' align='left'>WashType</td><td width='60' align='left'>ShipMode</td><td width='80' align='left'>Del.Date</td><td width='70' align='left'>OrderQtY</td>";
        divBody.InnerHtml += "<td width='70' align='left'>CutQTY</td><td width='70' align='left'>SewQTY</td><td width='70' align='left'>SewBal</td>";
        divBody.InnerHtml += "<td width='70' align='left'>Sew Comp</td>";
        //Added By ZouShiChang ON 2013.09.13 Start MES024 
        //if (this.Page.Request.QueryString.Get("site").ToString().ToUpper() != "DEV" && this.Page.Request.QueryString.Get("site").ToString().ToUpper() != "TIL")
        //{
        //    divBody.InnerHtml += "<td width='70' align='left'>Wash&QC Bal</td>";
        //}
        //else
        //{
        divBody.InnerHtml += "<td width='70' align='left'>Wash Bal</td>";
        divBody.InnerHtml += "<td width='70' align='left'>QC Bal</td>";
        //}      
        //Added By ZouShiChang ON 2013.09.13 End MES024 
        divBody.InnerHtml += "<td width='70' align='left'>Completion Form Date</td>";
        divBody.InnerHtml += "<td width='70' align='left'>Irg Rec</td>";
        divBody.InnerHtml += "<td width='70' align='left'>Irg Not Rec</td><td width='70' align='left'>Irg(%)</td><td width='70' align='left'>Irg Qty</td>";
        divBody.InnerHtml += "<td width='70' align='left'>Irg Not Press</td><td width='70' align='left'>Pkg(%)</td>";
        divBody.InnerHtml += "<td width='70' align='left'>Pack Line</td>";
        divBody.InnerHtml += "<td width='80' align='left'>ClosedQTY</td><td width='80' align='left'>NO#CTN</td><td width='70' align='left'>Insp Sts.</td><td width='90' align='left'>Closing Fig</td><td width='150' align='left'>Repairs</td><td width='150' align='left'>Remarks</td></tr>";

        for (int i = 0; i < dt1.Rows.Count; i++)
        {

            divBody.InnerHtml += "<tr>";
            for (int j = 0; j < dt1.Columns.Count; j++)
            {
                //Modified by Zikuan - MES-107
                //if (j < 6 || j > 16)
                //Modified by Jin Song MES128
                if (j != 6 && (j < 8 || j > 18))
                {
                    //if (dt1.Rows[i]["ShipMode"].ToString().Equals("S"))
                    //    divBody.InnerHtml += "<td align='left' style='color:Red'>" + dt1.Rows[i][j] + "</td>";
                    //else
                    divBody.InnerHtml += "<td align='left'>" + dt1.Rows[i][j] + "</td>";
                }
                else if (j == 6)
                {
                    if (dt1.Rows[i]["ShipMode"].ToString().Equals("A"))
                        divBody.InnerHtml += "<td align='left' style='color:Red'>" + dt1.Rows[i][j] + "</td>";
                    else
                        divBody.InnerHtml += "<td align='left'>" + dt1.Rows[i][j] + "</td>";
                }
                //End of modification MES128
                else
                {
                    divBody.InnerHtml += "<td align='right'>" + dt1.Rows[i][j] + "</td>";
                }
            }
            divBody.InnerHtml += "</tr>";
            ttlqtyi += double.Parse(dt1.Rows[i]["CutQTY"].ToString());
            ttlqtyl += double.Parse(dt1.Rows[i]["SewQty"].ToString());
            ttlqtyo += double.Parse(dt1.Rows[i]["SewBal"].ToString());
            //Added By ZouShiChang ON 2013.09.13 Start MES024 
            //if (this.Page.Request.QueryString.Get("site").ToString().ToUpper() != "DEV" && this.Page.Request.QueryString.Get("site").ToString().ToUpper() != "TIL")
            //{
            //    ttlqtyu += double.Parse(dt1.Rows[i]["Wash_QC_Bal"].ToString());
            //}
            //else
            //{
            ttlqtyWash += double.Parse(dt1.Rows[i]["Wash_Bal"].ToString());
            ttlqtyQc += double.Parse(dt1.Rows[i]["QC_Bal"].ToString());
            //}              
            //Added By ZouShiChang ON 2013.09.13 End MES024  
            ttlqtyr += double.Parse(dt1.Rows[i]["IrgRec"].ToString());
            WIPTotal += double.Parse(dt1.Rows[i]["IrgQTY"].ToString());

        }
        //Modify by Zikuan MES-107
        divBody.InnerHtml += "<tr   class='tr2style'><td align='left' colspan=8>Sub Total:</td>";
        divBody.InnerHtml += "<td></td><td align='right'>" + ttlqtyi + "</td>";
        divBody.InnerHtml += "<td align='right'>" + ttlqtyl + "</td>";
        divBody.InnerHtml += "<td align='right'>" + ttlqtyo + "</td>";

        if (this.Page.Request.QueryString.Get("site").ToString().ToUpper() != "DEV" && this.Page.Request.QueryString.Get("site").ToString().ToUpper() != "TIL")
        {
            divBody.InnerHtml += "<td></td><td align='right'>" + ttlqtyu + "</td>";
        }
        else
        {
            divBody.InnerHtml += "<td></td><td align='right'>" + ttlqtyWash + "</td>";
            divBody.InnerHtml += "<td align='right'>" + ttlqtyQc + "</td>";
        }

        divBody.InnerHtml += "<td></td><td align='right'>" + ttlqtyr + "</td><td></td><td></td>";
        divBody.InnerHtml += "<td align='right'>" + WIPTotal + "</td><td colspan=10></td>";
        divBody.InnerHtml += "</tr>";
        divBody.InnerHtml += "</table>";
    }

    protected void GetProcess()
    {
        ddlprocessCd.DataSource = MESComment.MesRpt.GetProcessCd(ddlFactoryCode.SelectedItem.Value, "");
        ddlprocessCd.DataTextField = "NM";
        ddlprocessCd.DataValueField = "PRC_CD";
        ddlprocessCd.DataBind();
    }

    protected void ddlFtyCd_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetProcess();
    }

}
