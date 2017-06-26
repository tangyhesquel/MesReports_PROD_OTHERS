using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_wipDaily : pPage
{
    public string FactoryCD = "";
    DataTable dtBody;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                FactoryCD = Request.QueryString["site"].ToString();
            }
            else
            {
                FactoryCD = "GEG";
            }
            if (FactoryCD.Contains("YMG"))
            {
                FactoryCD = "YMG";
                //Added by Yee Hou on July 2015
                SummaryByDDL.SelectedIndex = 1;
                if (!IsPostBack)  MESComment.CommFunction.LoadGroupDropDownList(base.CurrentSite, this.ddGroupName);
            }
            else if (FactoryCD.Contains("DEV"))
            {
                if (this.Request.QueryString.Get("factory_cd") != null)
                {
                    FactoryCD = this.Request.QueryString.Get("factory_cd").ToString();
                }
                else
                {
                    return;
                }
            }
        }
        if (!IsPostBack)
        {
            if (Request.QueryString["site"] != null)
            {
                GetProcess();
                GetToProcess();
                this.ddGroupName.DataSource = MESComment.wipDailySql.GetGroupName(FactoryCD);
                this.ddGroupName.DataTextField = "SYSTEM_KEY";
                this.ddGroupName.DataValueField = "SYSTEM_KEY";
                ddGroupName.DataBind();

                
                ddlproductFactory.DataTextField = "FACTORY_ID";
                ddlproductFactory.DataValueField = "FACTORY_ID";
                ddlproductFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
                ddlproductFactory.DataBind();

                ddlToproductFactory.DataTextField = "FACTORY_ID";
                ddlToproductFactory.DataValueField = "FACTORY_ID";
                ddlToproductFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
                ddlToproductFactory.DataBind();

                ddlProcessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
                ddlProcessType.DataTextField = "PROCESS_TYPE_ID";
                ddlProcessType.DataValueField = "PROCESS_TYPE_VALUE";
                ddlProcessType.DataBind();

                ddlToprocessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
                ddlToprocessType.DataTextField = "PROCESS_TYPE_ID";
                ddlToprocessType.DataValueField = "PROCESS_TYPE_VALUE";
                ddlToprocessType.DataBind();

            }
            this.cbTRX_DATE.Checked = true;

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
        else
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
    }

    protected void OrderByList_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetData();
    }

    protected void SetData()
    {
        string GroupName = this.ddGroupName.SelectedValue.ToString();
        string garmentType = ddlgarmentType.SelectedItem.Value;
        string prodLine = ddlprodLine.SelectedItem.Value;
        string processCd = ddlprocessCd.SelectedItem.Value;
        //Added By ZouShiChang ON 2013.09.24 Start MES024
        string processType = ddlProcessType.SelectedItem.Value;
        string productionFactory = ddlproductFactory.SelectedItem.Value;
        string togarmentType = ddlToGarmentType.SelectedItem.Value;
        string toprocessType = ddlToprocessType.SelectedItem.Value;
        string toproductionFactory = ddlToproductFactory.SelectedItem.Value;
        //Added By ZouShiChang ON 2013.09.24 Start MES024
        string ToprodLine = ddlToprodLine.SelectedItem.Value;
        string ToprocessCd = ddlToprocessCd.SelectedItem.Value;       
        string StartDate = (this.cbTRX_DATE.Checked ? txtStartDate.Text : "");
        string EndDate = (this.cbTRX_DATE.Checked ? txtEndDate.Text : "");
        string BPOStartDate = (this.cbBPO_DATE.Checked ? txtStartDate.Text : "");
        string BPOEndDate = (this.cbBPO_DATE.Checked ? txtEndDate.Text : "");
        string JONO_TEXT = txtJoNo.Text.Trim();
        string Order = OrderByList.SelectedValue.ToString().Trim();
        string color = "";
        divSummary.InnerHtml = "";

        //dtBody = MESComment.wipDailySql.GetDailyOutputByJOSectionDetail(garmentType, prodLine, processCd, ToprodLine, ToprocessCd, StartDate, EndDate, BPOStartDate, BPOEndDate, FactoryCD, JONO_TEXT, "", Order, GroupName);
        dtBody = MESComment.wipDailySql.GetDailyOutputByJOSectionDetail(garmentType, prodLine, processCd,processType,productionFactory, ToprodLine, ToprocessCd,togarmentType,toprocessType,toproductionFactory, StartDate, EndDate, BPOStartDate, BPOEndDate, FactoryCD, JONO_TEXT, "", Order, GroupName);
        this.gvBody.DataSource = dtBody;
        this.gvBody.DataBind();

        dtBody.Rows[dtBody.Rows.Count - 1].Delete();//删除合计行；

        divSummary.InnerHtml += "<table width='250'  border='1' cellspacing='0' cellpadding='0' style='font-size:12px;border-collapse:collapse'><tr class='tr2style'>";
        int cols = 1;
        if (ddlprocessCd.SelectedItem.Value.ToString() == "")
        {
            divSummary.InnerHtml += "<td>Process CD</td>";
            cols++;
        }
        divSummary.InnerHtml += "<td>Production Line</td><td>To Production Line</td><td>Qty</td></tr>";

        DataTable Summary = dtBody.DefaultView.ToTable(true, new string[] { "Process CD", "Prod Line", "To Prod Line" });
        Summary.Columns.Add("Qty");

        //Add by YeeHou on 15/07/2015
        Boolean isJOTTL;
        if (SummaryByDDL.SelectedIndex == 0)
            isJOTTL = true;
        else
            isJOTTL = false;

        foreach (DataRow dr in Summary.Rows)
        {
            double QTY1 = 0.0;
            
            string SQL = "[Process CD] ='" + dr["Process CD"].ToString() + "' AND [Prod Line] = '" + dr["Prod Line"].ToString().ToUpper() + "' AND [To Prod Line] = '" + dr["To Prod Line"].ToString() + "' ";
            DataRow[] drs = dtBody.Select(SQL);
            if (drs.Length > 0)
            {
                //Add by YeeHou on 15/07/2015
                if (isJOTTL)
                {
                    for (int i = 0; i < drs.Length; i++)
                    {
                        QTY1 += Double.Parse((drs[i]["JO TTL"].ToString().Equals("") ? "0" : drs[i]["JO TTL"].ToString()));
                    }
                }
                else
                {
                    for (int i = 0; i < drs.Length; i++)
                    {
                        QTY1 += Double.Parse((drs[i]["Today"].ToString().Equals("") ? "0" : drs[i]["Today"].ToString()));
                    }
                }
            }
            
            dr["Qty"] = QTY1.ToString();
        }
        double SummaryQty1 = 0;
        //DataRow[] orderrows = Summary.Select("", "[Prod Line] ASC");
        Summary = SortByLine(Summary);
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
            divSummary.InnerHtml += "<tr bgcolor=" + color + ">";
            if (ddlprocessCd.SelectedItem.Value.ToString() == "")
            {
                divSummary.InnerHtml += "<td style='white-space:nowrap'>" + Summary.Rows[i]["Process CD"] + "</td>";
            }
            divSummary.InnerHtml += "<td  style='white-space:nowrap'>" + Summary.Rows[i]["Prod Line"] + "</td><td style='white-space:nowrap'>" + Summary.Rows[i]["To Prod Line"] + "</td><td align='right' style='white-space:nowrap'>" + Summary.Rows[i]["Qty"] + "</td></tr>";
            SummaryQty1 += double.Parse((Summary.Rows[i]["Qty"].ToString().Equals("") ? "0" : Summary.Rows[i]["Qty"].ToString()));
        }
        divSummary.InnerHtml += "<tr class='tdbackcolor'><td align='right' style='white-space:nowrap'><b>Total QTY</b></td><td colspan='" + cols.ToString() + "'></td><td ><b>" + SummaryQty1 + "</b></td></tr>";
        divSummary.InnerHtml += "</table>";



        divSummary.InnerHtml += "<tr><td style='white-space:nowrap'><br/><strong><b>Summary Of The Day</b></strong></td></tr></tr><tr><td><table id='tablesummary' width='250'  border='1' cellspacing='0' cellpadding='0' style='font-size:12px;border-collapse:collapse'><tr class='tr2style'>";


        divSummary.InnerHtml += "<td style='white-space:nowrap'>Process CD</td>";
        //============集体重置=======        
        divSummary.InnerHtml += "<td style='white-space:nowrap'>Prod Line</td><td style='white-space:nowrap'>Total Pcs</td><td style='white-space:nowrap'>SAH Produced</td></tr>";
        // DataTable Summary = MESComment.MesRpt.GetDailyOutputByJOSectionSummary(txtJoNo.Text,ddlprodLine.SelectedItem.Value,ddlprocessCd.SelectedItem.Value,txtStartDate.Text,ddlFtyCd.SelectedItem.Value);        
        double SummaryQty2 = 0;
        double sahprc2 = 0;
        int Int_Color = 0;
        //==========================
        //选出distinct的Process和production line;DataTable joList = dataDetail.DefaultView.ToTable(true, new string[] { "JOB_ORDER_NO", "CUSTOMER_NAME", "TOTAL_CUT_QTY", "TOTAL_OUTPUT_QTY" });
        //dataset.Tables(0).DefaultView.Sort = "id desc"

        DataTable ProcessAndLine = Summary.DefaultView.ToTable(true, new string[] { "Process CD", "Prod Line" });
        //DataView dv =ProcessAndLine.Copy().DefaultView;
        //dv.Sort = "[Prod Line] ASC"; 
        //ProcessAndLine.DefaultView.Sort = "Prod Line";
        //Add by YeeHou on 15/07/2015
        //ProcessAndLine.Select(
        //ProcessAndLine = dv.Table.Copy();
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

            string Process_CD = row["Process CD"].ToString();
            string PRODUCTION_LINE_CD = row["Prod Line"].ToString();
            string SQL = "[Process CD] = '" + Process_CD + "' ";
            DataRow[] RowsForRowCount = dtBody.Select(SQL);
            int rc = RowsForRowCount.Length;

            divSummary.InnerHtml += "<tr bgcolor=" + color + "><td style='white-space:nowrap'>" + Process_CD + "</td>";

            SQL = "[Process CD] = '" + Process_CD + "' AND [Prod Line]='" + PRODUCTION_LINE_CD + "' ";
            DataRow[] RowsForCalculate = dtBody.Select(SQL);
            double SummaryQty3 = 0;
            double sahprod3 = 0;
            for (int i = 0; i < RowsForCalculate.Length; i++)
            {//将同部门和同Line的数据全部Summary起来，当成一条数据;
                sahprod3 += double.Parse((RowsForCalculate[i]["SAH Produced"].ToString().Equals("") ? "0" : RowsForCalculate[i]["SAH Produced"].ToString()));
                //Added by YeeHou on July 2015
                if (isJOTTL)
                    SummaryQty3 += double.Parse((RowsForCalculate[i]["JO TTL"].ToString().Equals("") ? "0" : RowsForCalculate[i]["JO TTL"].ToString()));
                else
                    SummaryQty3 += double.Parse((RowsForCalculate[i]["Today"].ToString().Equals("") ? "0" : RowsForCalculate[i]["Today"].ToString()));
            }
            SummaryQty2 += SummaryQty3;
            sahprc2 += sahprod3;
            Int_Color++;

            divSummary.InnerHtml += "<td style='white-space:nowrap'>" + PRODUCTION_LINE_CD + "</td>";
            divSummary.InnerHtml += "<td align='right' style='white-space:nowrap'>" + SummaryQty3 + "</td>";
            divSummary.InnerHtml += "<td align='right' style='white-space:nowrap'>" + sahprod3.ToString("#,###.##") + "</td></tr>";
        }
        divSummary.InnerHtml += "<tr class='tdbackcolor'><td align='right'  style='white-space:nowrap'><b>Total QTY</b></td><td></td>";
        divSummary.InnerHtml += "<td align='right' style='white-space:nowrap'><b>" + SummaryQty2 + "</b></td>";
        divSummary.InnerHtml += "<td align='right' style='white-space:nowrap'><b>" + sahprc2.ToString("#,###.##") + "</b></td></tr>";
        divSummary.InnerHtml += "</table></td></tr>";
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
        ddlprocessCd.DataSource = MESComment.MesRpt.GetProcessCd(FactoryCD, ddlgarmentType.SelectedItem.Value);
        ddlprocessCd.DataTextField = "NM";
        ddlprocessCd.DataValueField = "PRC_CD";
        ddlprocessCd.DataBind();
    }
    protected void GetToProcess()
    {
        ddlToprocessCd.DataSource = MESComment.MesRpt.GetToProcessCd(FactoryCD, ddlgarmentType.SelectedItem.Value, ddlprocessCd.SelectedItem.Value);
        ddlToprocessCd.DataTextField = "NM";
        ddlToprocessCd.DataValueField = "PRC_CD";
        ddlToprocessCd.DataBind();
    }
    protected void ddlprocessCd_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlprodLine.DataSource = MESComment.MesRpt.GetPrdLine(FactoryCD, ddlprocessCd.SelectedItem.Value,ddlgarmentType.SelectedItem.Value);
        ddlprodLine.DataTextField = "PRODUCTION_LINE_NAME";
        ddlprodLine.DataValueField = "PRODUCTION_LINE_CD";
        ddlprodLine.DataBind();
        GetToProcess();
    }
    protected void ddlToprocessCd_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlToprodLine.DataSource = MESComment.MesRpt.GetPrdLine(FactoryCD, ddlToprocessCd.SelectedItem.Value,ddlgarmentType.SelectedItem.Value);
        ddlToprodLine.DataTextField = "PRODUCTION_LINE_NAME";
        ddlToprodLine.DataValueField = "PRODUCTION_LINE_CD";
        ddlToprodLine.DataBind();
    }

    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                break;
            case DataControlRowType.DataRow:
                if (e.Row.RowIndex < dtBody.Rows.Count - 1)
                {
                    if (e.Row.RowIndex % 2 == 1)
                    {
                        e.Row.Attributes.Add("bgcolor", "#f2f2f2");
                    }
                    else
                    {
                        e.Row.Attributes.Add("bgcolor", "white");
                    }
                }
                else
                {
                    e.Row.Attributes.Add("bgcolor", "#EFEFE7");
                    e.Row.Attributes.Add("font-weight", "bolder");
                }
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (dtBody.Columns[i].ColumnName.Equals("SAH Produced"))
                    {
                        e.Row.Cells[i].Text = Double.Parse((e.Row.Cells[i].Text.Equals("") || e.Row.Cells[i].Text.Equals("&nbsp;")) ? "0" : e.Row.Cells[i].Text).ToString("#,###.##");
                    }
                }
                break;

        }
    }
    protected void cbTRX_DATE_CheckedChanged(object sender, EventArgs e)
    {

    }
    private DataTable SortByLine(DataTable dt)
    {
        if (!dt.Columns.Contains("Prod_LineForOrder"))
            dt.Columns.Add("Prod_LineForOrder");
        bool isnum = false;
        System.Text.StringBuilder newline = new System.Text.StringBuilder();
        System.Text.StringBuilder numline = new System.Text.StringBuilder();
        foreach (DataRow dr in dt.Rows)
        {
           if (dr["Prod Line"].ToString() == "")
                continue;
           isnum = false;
           char[] Linechars=dr["Prod Line"].ToString().ToCharArray();
           newline.Remove(0, newline.Length);
           numline.Remove(0, numline.Length);
           
           foreach (char c in Linechars)
           {
               //如果连续，则放在一起
               if (c >= 48 && c <= 57)
               {
                   numline.Append(c);
                   isnum = true;
               }
               else
               {
                   if (numline.Length > 0)
                   {
                       if (numline.Length > 1)
                       {
                           newline.Append(numline.ToString());
                       }
                       else
                       {
                           newline.Append("0" + numline.ToString());
                       }
                   }
                   newline.Append(c);
                   numline.Remove(0, numline.Length);
                   isnum = false;
               }
           }
           if (numline.Length > 0)
           {
               if (numline.Length > 1)
               {
                   newline.Append(numline.ToString());
               }
               else
               {
                   newline.Append("0" + numline.ToString());
               }
           }
           dr["Prod_LineForOrder"] = newline.ToString();


        }
        DataRow[] drs = dt.Select("", "Prod_LineForOrder ASC");
        DataTable newdt = dt.Clone();
        foreach (DataRow dr in drs)
        {
            newdt.Rows.Add(dr.ItemArray);
        }
        return newdt;

    }
  
}
