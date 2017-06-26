using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;

public partial class Reports_wipDailyYMG : pPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlFtyCd.DataSource = MESComment.MesRpt.GetFactoryCd();
            ddlFtyCd.DataTextField = "FACTORY_ID";
            ddlFtyCd.DataValueField = "FACTORY_ID";
            ddlFtyCd.DataBind();

            //Added By ZouShiChang ON 2013.09.23 Start MES024
            ddlProdFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
            ddlProdFactory.DataTextField = "FACTORY_ID";
            ddlProdFactory.DataValueField = "FACTORY_ID";
            ddlProdFactory.DataBind();

            ddlToProdFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
            ddlToProdFactory.DataTextField = "FACTORY_ID";
            ddlToProdFactory.DataValueField = "FACTORY_ID";
            ddlToProdFactory.DataBind();

            ddlProcessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
            ddlProcessType.DataTextField = "PROCESS_TYPE_ID";
            ddlProcessType.DataValueField = "PROCESS_TYPE_VALUE";
            ddlProcessType.DataBind();

            ddlToProcessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
            ddlToProcessType.DataTextField = "PROCESS_TYPE_ID";
            ddlToProcessType.DataValueField = "PROCESS_TYPE_VALUE";
            ddlToProcessType.DataBind();

            if (Request.QueryString["site"] != null)
            {
                ddlFtyCd.SelectedValue = Request.QueryString["site"].ToString();
                GetProcess();
                GetToProcess();
                if (Request.QueryString["site"].ToString().ToUpper().Equals("YMG"))
                {
                    this.ddGroupName.Enabled = true;
                    MESComment.CommFunction.LoadGroupDropDownList(base.CurrentSite, this.ddGroupName);
                }
            }
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
        if (Check_Query())
        {
            this.Msg.Visible = false;
            SetData();
        }
        else
        {
            this.Msg.Visible = true;
        }
    }

    protected Boolean Check_Query()
    {
        if (txtStartDate.Text != "" && txtEndDate.Text != "")
        {
            DateTime DFrom = DateTime.Parse(txtStartDate.Text);
            DateTime DTo = DateTime.Parse(txtEndDate.Text);
            if (DFrom > DTo)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else if (txtStartDate.Text == "" && txtEndDate.Text == "")
        {
            if (txtJoNo.Text != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    protected void SetData()
    {
        StringBuilder htmlBuilder;
        StringBuilder htmlSummaryBuilder;

        if (txtEndDate.Text == "")
        {
            txtEndDate.Text = txtStartDate.Text;
        }
        string GroupName = this.ddGroupName.SelectedValue.ToString();
        divBody.InnerHtml = "";
        divSummary.InnerHtml = "";
        
        
        htmlBuilder=new StringBuilder("");
        htmlBuilder.Append("<table border='1' cellspacing='0' cellpadding='0'style='font-size:12px;border-collapse:collapse'>");
        htmlBuilder.Append( "<tr class='tr2style' align='center'>");
        htmlBuilder.Append( "<td>Customer Name</td>");
        htmlBuilder.Append( "<td>Style No.</td>");
        htmlBuilder.Append( "<td width='50'>S/C No</td>");
        htmlBuilder.Append( "<td width='80'>J/O No</td>");
        htmlBuilder.Append( "<td width='60'>Order Qty</td>");
        htmlBuilder.Append( "<td width='60'>Cut Qty</td>");
        htmlBuilder.Append( "<td width='85'>BPO Del date</td>");
        htmlBuilder.Append( "<td>EX－factory Date</td>");
        htmlBuilder.Append( "<td>Garment Type</td>");
        htmlBuilder.Append( "<td>FAB Type</td>");
        htmlBuilder.Append( "<td>Sewing SAH</td>");
        htmlBuilder.Append( "<td>Wash Type</td>");
        if (ddlprocessCd.SelectedItem.Value.ToString() == "")
        {
            htmlBuilder.Append( "<td>Process CD</td>");
        }
        htmlBuilder.Append("<td>Prod Line</td>");
        htmlBuilder.Append( "<td>To Prod Line</td>");
        htmlBuilder.Append( "<td>Status</td>");
        htmlBuilder.Append( "<td>Today</td>");
        htmlBuilder.Append(  "<td>SAH Produced</td>");
        htmlBuilder.Append(  "<td>Mth TTL</td>");
        htmlBuilder.Append(  "<td>JO TTL</td>");
        htmlBuilder.Append(  "<td>Up To Day</td>");
        htmlBuilder.Append(  "<td>Daily Defect Qty</td>");
        htmlBuilder.Append(  "</tr>");

        DataTable Detail = MESComment.wipDailySql.GetDailyOutputByJOSectionDetail(ddlgarmentType.SelectedItem.Value, ddlprodLine.SelectedItem.Value, ddlprocessCd.SelectedItem.Value,ddlProcessType.SelectedItem.Value,ddlProdFactory.SelectedItem.Value,ddlToprodLine.SelectedItem.Value, ddlToprocessCd.SelectedItem.Value,ddlToGarmentType.SelectedItem.Value,ddlToProcessType.SelectedItem.Value,ddlToProdFactory.SelectedItem.Value,txtStartDate.Text, txtEndDate.Text, ddlFtyCd.SelectedItem.Value, txtJoNo.Text.Trim(), "", OrderByList.SelectedValue.ToString().Trim(), GroupName);
        string color = "";
        double sahprod = 0;
        string process = "";
        string process2 = "";
        string prodline = "";
        string prodline2 = "";
        string next_prodline = "";
        string next_prodline2 = "";
        string JoNo = "";
        string JoNo2 = "";
        double SummaryQtyT = 0;
        double SummaryQtyS = 0;
        double SummaryQtyM = 0;
        double SummaryQtyJ = 0;
        double SummaryQtyU = 0;
        double SummaryQtyD = 0;
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
            
            htmlBuilder.Append("<tr bgcolor=" + color + "><td>" + Detail.Rows[i]["CUSTOMER_NAME"] + "</td><td>" + Detail.Rows[i]["STYLE_NO"] + "</td><td align='right'>" + Detail.Rows[i]["SC_NO"] + "</td><td align='right'>" + Detail.Rows[i]["JOB_ORDER_NO"] + "</td>");
            htmlBuilder.Append( "<td align='right'>" + Detail.Rows[i]["ORDER_QTY"] + "</td><td align='right'>" + Detail.Rows[i]["CUT_QTY"] + "</td><td align='right'>" + Detail.Rows[i]["BUYER_PO_DEL_DATE"] + "</td><td>" + Detail.Rows[i]["LATEST_EX_FACTORY_DATE"] + "</td><td>" + Detail.Rows[i]["GARMENT_TYPE_CD"] + "</td>");
            htmlBuilder.Append( "<td>" + Detail.Rows[i]["FAB_TYPE_CD"] + "</td><td align='right'>" + Detail.Rows[i]["SAH"] + "</td><td align='center'>" + Detail.Rows[i]["WASH_TYPE_CD"] + "</td>");
            
            if (ddlprocessCd.SelectedItem.Value.ToString() == "")
            {
                htmlBuilder.Append( "<td>" + Detail.Rows[i]["Process_CD"] + "</td>");
            }

            process = Detail.Rows[i]["Process_CD"].ToString();
            prodline = Detail.Rows[i]["PRODUCTION_LINE_CD"].ToString();
            next_prodline = Detail.Rows[i]["TO_PROD_LINE"].ToString();
            JoNo = Detail.Rows[i]["JOB_ORDER_NO"].ToString();
            htmlBuilder.Append( "<td>" + Detail.Rows[i]["PRODUCTION_LINE_CD"] + "</td><td>" + Detail.Rows[i]["TO_PROD_LINE"] + "</td>");
            htmlBuilder.Append( "<td>" + Detail.Rows[i]["JO_CLEAR_FLAG"] + "</td><td>" + Detail.Rows[i]["DAILY"] + "</td>");

            SummaryQtyT += double.Parse((Detail.Rows[i]["DAILY"].ToString().Equals("") ? "0" : Detail.Rows[i]["DAILY"].ToString()));

            if ((" " + Detail.Rows[i]["Process_CD"].ToString()).IndexOf("SEW") > 0)
            {
                sahprod = double.Parse((Detail.Rows[i]["DAILY"].ToString().Equals("") ? "0" : Detail.Rows[i]["DAILY"].ToString())) / 12 * double.Parse(Detail.Rows[i]["SAH"].ToString());
                SummaryQtyS += sahprod;
                htmlBuilder.Append( "<td align='right'>" + sahprod.ToString("f2") + "</td>");
            }
            else
            {
                htmlBuilder.Append("<td></td>");
            }

            SummaryQtyM += double.Parse((Detail.Rows[i]["MTH_TTL"].ToString().Equals("") ? "0" : Detail.Rows[i]["MTH_TTL"].ToString()));
            SummaryQtyJ += double.Parse((Detail.Rows[i]["JO_TTL"].ToString().Equals("") ? "0" : Detail.Rows[i]["JO_TTL"].ToString()));
            SummaryQtyU += double.Parse((Detail.Rows[i]["JOUPToDay"].ToString().Equals("") ? "0" : Detail.Rows[i]["JOUPToDay"].ToString()));
            
            htmlBuilder.Append( "<td align='right'>" + Detail.Rows[i]["MTH_TTL"] + "</td><td align='right'>" + Detail.Rows[i]["JO_TTL"] + "</td>");
            htmlBuilder.Append( "<td align='right'>" + Detail.Rows[i]["JOUPToDay"] + "</td>");
            
            if (JoNo != JoNo2 || process != process2 || prodline != prodline2 || next_prodline != next_prodline2 || JoNo != JoNo2 || i == 0)
            {
                htmlBuilder.Append( "<td align='right'>" + Detail.Rows[i]["DAILY_DEFECT_QTY"] + "</td></tr>");
                
                if (Detail.Rows[i]["DAILY_DEFECT_QTY"].ToString() != "" && Detail.Rows[i]["DAILY_DEFECT_QTY"].ToString() != "NULL")
                {
                    SummaryQtyD += double.Parse(Detail.Rows[i]["DAILY_DEFECT_QTY"].ToString());
                }
            }
            else
            {
                htmlBuilder.Append( "<td> </td></tr>");
            }
            prodline2 = prodline;
            next_prodline2 = next_prodline;
            process2 = process;
            JoNo2 = JoNo;

        }

        if (color.Equals("white"))
        {
            color = "#f2f2f2";
        }
        else
        {
            color = "white";
        }

        if (ddlprocessCd.SelectedItem.Value.ToString() == "")
        {
            htmlBuilder.Append( "<tr ><td class='tdbackcolor'  colspan='16' align='center'><b>Summary:</b></td>");
        }
        else
        {
            htmlBuilder.Append( "<tr ><td class='tdbackcolor'  colspan='15' align='center'><b>Summary:</b></td>");
        }

       htmlBuilder.Append("<td class='tdbackcolor' align='right'>" + SummaryQtyT + "</td><td class='tdbackcolor' align='right'>" + SummaryQtyS.ToString("f2") + "</td><td class='tdbackcolor' align='right'>" + SummaryQtyM + "</td><td class='tdbackcolor' align='right'>" + SummaryQtyJ + "</td><td class='tdbackcolor' align='right'>");
       htmlBuilder.Append( SummaryQtyU + "</td><td class='tdbackcolor' align='right'>" + SummaryQtyD + "</td></tr>");
       htmlBuilder.Append("</table>");


       htmlSummaryBuilder = new StringBuilder("");
       htmlSummaryBuilder.Append( "<table width='250'  border='1' cellspacing='0' cellpadding='0' style='font-size:12px;border-collapse:collapse'><tr class='tr2style'>");
        int cols = 1;
        if (ddlprocessCd.SelectedItem.Value.ToString() == "")
        {
            htmlSummaryBuilder.Append("<td>Process CD</td>");
            cols++;
        }
        htmlSummaryBuilder.Append( "<td>Production Line</td><td>To Production Line</td><td>Qty</td></tr>");

        DataTable Summary = MESComment.wipDailySql.GetDailyOutputByJOSectionSummary(txtJoNo.Text, ddlprodLine.SelectedItem.Value, ddlgarmentType.SelectedItem.Value, ddlprocessCd.SelectedItem.Value,ddlProcessType.SelectedItem.Value,ddlProdFactory.SelectedItem.Value, ddlToprodLine.SelectedItem.Value, ddlToGarmentType.SelectedItem.Value, ddlToprocessCd.SelectedItem.Value,ddlToProcessType.SelectedItem.Value,ddlToProdFactory.SelectedItem.Value,txtStartDate.Text, txtEndDate.Text, ddlFtyCd.SelectedItem.Value, GroupName);
        double SummaryQty1 = 0;
        for (int i = 0; i < Summary.Rows.Count; i++)
        {
            if (i % 2 == 0)
            {
                color = "white";
            }
            else
            {
                color = "#f2f2f2";
            }
            htmlSummaryBuilder.Append( "<tr bgcolor=" + color + ">");
            if (ddlprocessCd.SelectedItem.Value.ToString() == "")
            {
                htmlSummaryBuilder.Append("<td style='white-space:nowrap'>" + Summary.Rows[i]["Process_CD"] + "</td>");
            }
            htmlSummaryBuilder.Append( "<td  style='white-space:nowrap'>" + Summary.Rows[i]["PRODUCTION_LINE_CD"] + "</td><td style='white-space:nowrap'>" + Summary.Rows[i]["TO_PROD_LINE"] + "</td><td align='right' style='white-space:nowrap'>" + Summary.Rows[i]["DAILY"] + "</td></tr>");
            SummaryQty1 += double.Parse(Summary.Rows[i]["DAILY"].ToString());
        }
        htmlSummaryBuilder.Append( "<tr class='tdbackcolor'><td align='right' style='white-space:nowrap'><b>Total QTY</b></td><td colspan='" + cols.ToString() + "'></td><td ><b>" + SummaryQty1 + "</b></td></tr>");
        htmlSummaryBuilder.Append( "</table>");



        htmlSummaryBuilder.Append("<tr><td style='white-space:nowrap'><br/><strong><b>Summary Of The Day</b></strong></td></tr></tr><tr><td><table id='tablesummary' width='250'  border='1' cellspacing='0' cellpadding='0' style='font-size:12px;border-collapse:collapse'><tr class='tr2style'>");


        htmlSummaryBuilder.Append( "<td style='white-space:nowrap'>Process CD</td>");
        //============集体重置=======        
        htmlSummaryBuilder.Append("<td style='white-space:nowrap'>Prod Line</td><td style='white-space:nowrap'>Total Pcs</td><td style='white-space:nowrap'>SAH Produced</td></tr>");
        // DataTable Summary = MESComment.MesRpt.GetDailyOutputByJOSectionSummary(txtJoNo.Text,ddlprodLine.SelectedItem.Value,ddlprocessCd.SelectedItem.Value,txtStartDate.Text,ddlFtyCd.SelectedItem.Value);        
        double SummaryQty2 = 0;
        double sahprc2 = 0;
        int Int_Color = 0;
        //==========================
        //选出distinct的Process和production line;DataTable joList = dataDetail.DefaultView.ToTable(true, new string[] { "JOB_ORDER_NO", "CUSTOMER_NAME", "TOTAL_CUT_QTY", "TOTAL_OUTPUT_QTY" });
        Summary.CaseSensitive = false;//DataTable不分区大小写 Added By ZouShiChang on 2014.01.03
        DataTable ProcessAndLine = Summary.DefaultView.ToTable(true, new string[] { "Process_CD", "PRODUCTION_LINE_CD" });
        foreach (DataRow row in ProcessAndLine.Rows)
        {
            if (Int_Color % 2 == 0)
            {
                color = "white";
            }
            else
            {
                color = "#f2f2f2";
            }

            string Process_CD = row["Process_CD"].ToString();
            string PRODUCTION_LINE_CD = row["PRODUCTION_LINE_CD"].ToString();
            string SQL = "Process_CD = '" + Process_CD + "'";
            DataRow[] RowsForRowCount = Summary.Select(SQL);
            int rc = RowsForRowCount.Length;

            htmlSummaryBuilder.Append( "<tr bgcolor=" + color + "><td style='white-space:nowrap'>" + Process_CD + "</td>");

            SQL = "Process_CD = '" + Process_CD + "' AND PRODUCTION_LINE_CD='" + PRODUCTION_LINE_CD + "'";
            DataRow[] RowsForCalculate = Summary.Select(SQL);
            double SummaryQty3 = 0;
            double sahprod3 = 0;
            for (int i = 0; i < RowsForCalculate.Length; i++)
            {//将同部门和同Line的数据全部Summary起来，当成一条数据;
                sahprod3 += double.Parse((RowsForCalculate[i]["SAH_PRODUCE"].ToString().Equals("") ? "0" : RowsForCalculate[i]["SAH_PRODUCE"].ToString()));
                SummaryQty3 += double.Parse((RowsForCalculate[i]["DAILY"].ToString().Equals("") ? "0" : RowsForCalculate[i]["DAILY"].ToString()));
            }
            SummaryQty2 += SummaryQty3;
            sahprc2 += sahprod3;
            Int_Color++;

            htmlSummaryBuilder.Append("<td style='white-space:nowrap'>" + PRODUCTION_LINE_CD + "</td>");
            htmlSummaryBuilder.Append( "<td align='right' style='white-space:nowrap'>" + SummaryQty3 + "</td>");
            htmlSummaryBuilder.Append( "<td align='right' style='white-space:nowrap'>" + sahprod3 + "</td></tr>");
        }
        htmlSummaryBuilder.Append("<tr class='tdbackcolor'><td align='right'  style='white-space:nowrap'><b>Total QTY</b></td><td></td>");
        htmlSummaryBuilder.Append( "<td align='right' style='white-space:nowrap'><b>" + SummaryQty2 + "</b></td>");
        htmlSummaryBuilder.Append( "<td align='right' style='white-space:nowrap'><b>" + sahprc2 + "</b></td></tr>");
        htmlSummaryBuilder.Append("</table></td></tr>");

        divBody.InnerHtml = htmlBuilder.ToString();
        divSummary.InnerHtml = htmlSummaryBuilder.ToString();

    }

    protected void OrderByList_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetData();
    }


    protected void ddlFtyCd_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetProcess();
        GetToProcess();
    }
    protected void ddlGMT_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetProcess();
        GetToProcess();
    }
    protected void GetProcess()
    {
        ddlprocessCd.DataSource = MESComment.MesRpt.GetProcessCd(ddlFtyCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value);
       
        ddlprocessCd.DataTextField = "NM";
        ddlprocessCd.DataValueField = "PRC_CD";
        ddlprocessCd.DataBind();

    }
    protected void GetToProcess()
    {
        ddlToprocessCd.DataSource = MESComment.MesRpt.GetToProcessCd(ddlFtyCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value, ddlprocessCd.SelectedItem.Value);
        ddlToprocessCd.DataTextField = "NM";
        ddlToprocessCd.DataValueField = "PRC_CD";
        ddlToprocessCd.DataBind();
    }
    protected void ddlprocessCd_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlprodLine.DataSource = MESComment.MesRpt.GetPrdLine(ddlFtyCd.SelectedItem.Value, ddlprocessCd.SelectedItem.Value,ddlgarmentType.SelectedItem.Value);
        ddlprodLine.DataTextField = "PRODUCTION_LINE_NAME";
        ddlprodLine.DataValueField = "PRODUCTION_LINE_CD";
        ddlprodLine.DataBind();
        GetToProcess();
    }
    protected void ddlToprocessCd_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlToprodLine.DataSource = MESComment.MesRpt.GetPrdLine(ddlFtyCd.SelectedItem.Value, ddlToprocessCd.SelectedItem.Value,ddlgarmentType.SelectedItem.Value);
        ddlToprodLine.DataTextField = "PRODUCTION_LINE_NAME";
        ddlToprodLine.DataValueField = "PRODUCTION_LINE_CD";
        ddlToprodLine.DataBind();
    }
}
