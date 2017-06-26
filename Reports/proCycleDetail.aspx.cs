using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_proCycleDetail : pPage
{
    string[] headTitle = { "Process", "Production Line", "BPO Date", "Job Order NO", "Style No", "Buyer Code", "Buyer Name", "Wash Type", "Actual Cut Qty", "Ex Fty Date", "Transaction Date", "Opening", "In", "Out", "Discrepancy", "PullOut", "Wip", "Complete Date", "Prod Complete" }; //Modified by MunFoong on 2014.07.24, MES-135
    string[] DetheadTitle = { "Process", "Production Line", "Wip", "Out", "dCT" };
    int totalWipQty = 0, totalOutQty = 0;
    //Modified by Zikuan MES-069
    int totalOpening = 0;
    //End Modified
    public float totalWip, totalOut, ct;
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
            ExcTable.Visible = false;

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
                ddlProcessCode.DataSource = MESComment.MesRpt.GetProcessCode(ddlFactoryCode.SelectedItem.Value, ddlGarmentType.SelectedItem.Value);
                ddlProcessCode.DataTextField = "SHORT_NAME";
                ddlProcessCode.DataValueField = "PROCESS_CD";
                ddlProcessCode.DataBind();
                ddlProductionLine.DataSource = MESComment.MesRpt.GetPrdLine(ddlFactoryCode.SelectedItem.Value, ddlProcessCode.SelectedItem.Value, ddlGarmentType.SelectedItem.Value);
                ddlProductionLine.DataTextField = "PRODUCTION_LINE_NAME";
                ddlProductionLine.DataValueField = "PRODUCTION_LINE_CD";
                ddlProductionLine.DataBind();

                ddlProcessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
                ddlProcessType.DataTextField = "PROCESS_TYPE_ID";
                ddlProcessType.DataValueField = "PROCESS_TYPE_VALUE";
                ddlProcessType.DataBind();


                ddlProdFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
                ddlProdFactory.DataTextField = "FACTORY_ID";
                ddlProdFactory.DataValueField = "FACTORY_ID";
                ddlProdFactory.DataBind();


                //if (Request.QueryString["site"].ToString().ToUpper().Equals("YMG"))
                //{
                this.ddGroupName.Enabled = true;
                MESComment.CommFunction.LoadGroupDropDownList(base.CurrentSite, this.ddGroupName);
                //}
            }

        }

    }
    private bool CheckQueryCondition()
    {
        string strBeginDate = this.txtStartDate.Text.Trim();
        string strEndDate = this.txtEndDate.Text.Trim();
        if (Request.QueryString["site"] == null)
        {
            this.lblMessage.Text = "Not Data Connection!";
            this.lblMessage.Visible = true;
            this.gvBody.Visible = false;
            this.ExcTable.Visible = false;
            return false;
        }
        if (strBeginDate.Length == 0 && strEndDate.Length == 0)
        {
            this.lblMessage.Text = "Please choose condition to query!";
            this.lblMessage.Visible = true;
            this.gvBody.Visible = false;
            this.ExcTable.Visible = false;
            return false;
        }
        if (strBeginDate.Length == 0 && strEndDate.Length != 0 || strBeginDate.Length != 0 && strEndDate.Length == 0)
        {
            this.lblMessage.Text = "Please select accurate  Date!";
            this.lblMessage.Visible = true;
            this.gvBody.Visible = false;
            this.ExcTable.Visible = false;
            return false;
        }
        //Added by Jin Song MES115
        DateTime BeginDate = Convert.ToDateTime(strBeginDate);
        DateTime EndDate = Convert.ToDateTime(strEndDate);
        double TotalDays = (EndDate - BeginDate).TotalDays;
        if (this.ddlProcessCode.SelectedValue == "")
        {
            if (TotalDays > 0)
            {
                this.lblMessage.Text = "Only 1 day query is allowed if Process Code = All.";
                this.lblMessage.Visible = true;
                this.gvBody.Visible = false;
                this.ExcTable.Visible = false;
                return false;
            }
        }
        //End Add MES115

        return true;
    }

    private void SetQuery()
    {
        if (true == this.CheckQueryCondition())
        {
            string GroupName = this.ddGroupName.SelectedValue;
            //Modified by Jin Song MES115
            if (cbDetail.Checked == true || cbSummary.Checked == true)
            {
                DataTable dtBody = MESComment.proCycleDetailSql.GetProCycleDetailList(ddlFactoryCode.SelectedItem.Value, ddlGarmentType.SelectedItem.Value, ddlWashType.SelectedItem.Value, ddlProcessCode.SelectedItem.Value, ddlProcessType.SelectedItem.Value, ddlProdFactory.SelectedItem.Value, ddlProductionLine.SelectedItem.Value, ddlOutSource.SelectedItem.Value, txtStartDate.Text, txtEndDate.Text, cbCheck.Checked, CheckBoxByProdLine.Checked, CbLineNm.Checked, GroupName, CbProcessCloseStatus.Checked);

                this.gvBody.DataSource = dtBody;
                this.gvBody.DataBind();

                if (gvBody.Rows.Count > 0)
                {
                    ExcTable.Visible = true;
                    lblMessage.Visible = false;
                    // return;
                    //Added by Jin Song MES115
                    if (this.cbDetail.Checked == true)
                    {
                        gvBody.Visible = true;
                        if (this.ddlProcessCode.SelectedItem.Value != "")
                            this.dvTotal.Visible = true;
                    }

                    if (this.cbSummary.Checked == true)
                    {
                        DataTable dtDct = new DataTable();
                        DataTable dtTemp = dtBody.DefaultView.ToTable(true, new string[] { "PROCESS_CD", "PRODUCTION_LINE_CD" });
                        dtDct.Columns.Add("PROCESS_CD");
                        //dtDct.Columns.Add("OPENING");
                        //Added by Jin Song MES115
                        dtDct.Columns.Add("Production Line");
                        //End of Add MES115
                        dtDct.Columns.Add("WIP");
                        dtDct.Columns.Add("OUT");
                        dtDct.Columns.Add("dCT");
                        double Total_WIP = 0.0;
                        double Total_OPENING = 0.0;
                        double Total_OUT = 0.0;
                        double Total_DCT = 0.0;
                        string All_DCT = "";
                        string PROCESS = "";
                        //foreach (DataRow dr in dtTemp.Rows)
                        for (int a = 0; a < dtTemp.Rows.Count; a++)
                        {
                            //string PROCESS = dr["PROCESS_CD"].ToString();
                            PROCESS = dtTemp.Rows[a]["PROCESS_CD"].ToString();
                            string SQL = "PROCESS_CD = '" + PROCESS + "'";
                            //Added by Jin Song MES115
                            string PRODUCTION_LINE = dtTemp.Rows[a]["PRODUCTION_LINE_CD"].ToString();
                            if (this.CheckBoxByProdLine.Checked == true)
                            {
                                SQL += "AND PRODUCTION_LINE_CD = '" + PRODUCTION_LINE + "'";
                            }
                            //End of add MES115
                            double WIP = 0.0;
                            double OUT = 0.0;
                            //Modified by Zikuan MES-069
                            double OPENING = 0.0;
                            //End Modified MES-069

                            //Modified by Jin Song MES115
                            //double dCT = 0.0;
                            string dCT = "";
                            //End of modification MES115

                            DataRow[] drs = dtBody.Select(SQL);
                            if (drs.Length > 0)
                            {
                                for (int i = 0; i < drs.Length; i++)
                                {
                                    WIP += Double.Parse(drs[i]["wip_qty"].ToString());
                                    //Modified by Zikuan MES-069
                                    OPENING += Double.Parse(drs[i]["opening_qty"].ToString());
                                    //End Modified MES-069
                                    OUT += Double.Parse(drs[i]["out_qty"].ToString());
                                }
                                //Modified by Zikuan MES-069
                                //dCT = OUT.Equals(0) ? "" : (WIP / OUT).ToString("#,##0.0000");
                                
                                //Modified by MF on 20160126, dCT formula change to Day End WIP/ WIP out
                                //dCT = OUT.Equals(0) ? "0" : (((OPENING + WIP) / 2) / OUT).ToString("#,##0.0000");
                                dCT = OUT.Equals(0) ? "0" : (WIP / OUT).ToString("#,##0.0000");
                                //End of modified by MF on 20160126, dCT formula change to Day End WIP/ WIP out

                                //End Modified MES-069
                            }
                            
                            
                            //Total_WIP += WIP;
                            //Total_OPENING += OPENING;
                            //Total_OUT += OUT;
                            //if (!dCT.Equals(""))
                            //{
                            //    Total_DCT += Double.Parse(dCT);
                            //}
                            //if (this.CheckBoxByProdLine.Checked == false)
                            //    dtDct.Rows.Add(new object[] { PROCESS, WIP, OUT, dCT });
                            //else
                            //Modified by Jin Song MES115; Bug Fix by Zikuan 20140609 Process not match with detail
                            if (a == 0 || dtTemp.Rows[a]["PROCESS_CD"].ToString().ToUpper() != dtTemp.Rows[a - 1]["PROCESS_CD"].ToString().ToUpper() || dtTemp.Rows[a]["PRODUCTION_LINE_CD"].ToString().ToUpper() != dtTemp.Rows[a - 1]["PRODUCTION_LINE_CD"].ToString().ToUpper())
                            {
                                dtDct.Rows.Add(new object[] { PROCESS, PRODUCTION_LINE.ToUpper(), WIP, OUT, dCT });
                                Total_WIP += WIP;
                                Total_OPENING += OPENING;
                                Total_OUT += OUT;
                            }
                            if (CheckBoxByProdLine.Checked == true)
                            {
                                if (a != 0 && a + 1 != dtTemp.Rows.Count)
                                    if (PROCESS.ToUpper() != dtTemp.Rows[a + 1]["PROCESS_CD"].ToString().ToUpper()) //Modified by Jin Song 2014-08-18, Bug Fix
                                    {
                                        //Modified by MF on 20160126, dCT formula change to Day End WIP/ WIP out
                                        //All_DCT = Total_OUT.Equals(0) ? "0" : (((Total_WIP + Total_OPENING) / 2) / Total_OUT).ToString("f4");
                                        All_DCT = Total_OUT.Equals(0) ? "0" : (Total_WIP / Total_OUT).ToString("f4");
                                        //End of modified by MF on 20160126, dCT formula change to Day End WIP/ WIP out

                                        dtDct.Rows.Add(new object[] { PROCESS.ToUpper(), "ALL", Total_WIP.ToString(), Total_OUT.ToString(), All_DCT }); //Modified by Jin Song 2014-08-18, Bug Fix
                                        if (!All_DCT.Equals(""))
                                            Total_DCT += Double.Parse(All_DCT);
                                        Total_WIP = 0.0;
                                        Total_OUT = 0.0;
                                        Total_OPENING = 0.0;
                                        All_DCT = "";
                                    }
                            }
                            else if (!dCT.Equals(""))
                            {
                                Total_DCT += Double.Parse(dCT);
                            }

                            //End of modification MES115
                        }

                        if (CheckBoxByProdLine.Checked == true)
                        {
                            //Modified by MF on 20160126, dCT formula change to Day End WIP/ WIP out
                            //All_DCT = Total_OUT.Equals(0) ? "0" : (((Total_WIP + Total_OPENING) / 2) / Total_OUT).ToString("f4");
                            All_DCT = Total_OUT.Equals(0) ? "0" : (Total_WIP / Total_OUT).ToString("f4");
                            //Modified by MF on 20160126, dCT formula change to Day End WIP/ WIP out

                            dtDct.Rows.Add(new object[] { PROCESS, "ALL", Total_WIP.ToString(), Total_OUT.ToString(), All_DCT });
                            //Added by Jin Song to show Total dCT
                            if (ddlProcessCode.SelectedValue == "")
                            {
                                Total_DCT += Double.Parse(All_DCT);
                                dtDct.Rows.Add(new object[] { "Total DCT:", "", "", "", Total_DCT.ToString("#,##0.0000") });
                            }
                            lblTitle.Text = "Summary (by Production Line)";
                        }
                        else if (ddlProcessCode.SelectedValue == "")
                        {
                            dtDct.Rows.Add(new object[] { "Total DCT:", "", "", "", Total_DCT.ToString("#,##0.0000") });
                            lblTitle.Text = "Summary";
                        }
                        else
                            lblTitle.Text = "Summary";

                        this.gvDct.Visible = true;
                        lblTitle.Visible = true;
                        this.gvDct.DataSource = dtDct;
                        this.gvDct.DataBind();
                    }
                    else
                    {
                        this.gvDct.Visible = false;
                        this.lblTitle.Visible = false;
                    }
                }
                else
                {
                    ExcTable.Visible = false;
                    lblMessage.Text = "Not Found any transaction data!";
                    lblMessage.Visible = true;
                }
            }
            else
            {
                ExcTable.Visible = false;
                lblMessage.Text = "Please choose Detail Data, Summary Data or both.";
                lblMessage.Visible = true;
            }
            //End of Modification MES115



            //Added by Jin Song MES115
            //if (this.ddlProcessCode.SelectedItem.Value != "")
            //{
            //    DataTable dtWipControl = MESComment.proCycleDetailSql.GetWipControlByLine(ddlFactoryCode.SelectedItem.Value, ddlProcessCode.SelectedItem.Value);

            //    if (dtWipControl.Rows[0]["WipControl"].Equals("Y"))
            //    {
            //        DataTable dtSummary = MESComment.proCycleDetailSql.GetSummaryByLine(ddlFactoryCode.SelectedItem.Value, ddlGarmentType.SelectedItem.Value, ddlWashType.SelectedItem.Value, ddlProcessCode.SelectedItem.Value, ddlProcessType.SelectedItem.Value, ddlProdFactory.SelectedItem.Value, ddlProductionLine.SelectedItem.Value, ddlOutSource.SelectedItem.Value, txtStartDate.Text, txtEndDate.Text, cbCheck.Checked, GroupName);

            //        this.gvSum.DataSource = dtSummary;
            //        this.gvSum.DataBind();
            //        this.gvSum.Visible = true;
            //        this.lblTitle.Visible = true;
            //    }

            //}
            //End of Modification MES115
        }
    }
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();
                //First Row

                if (CbProcessCloseStatus.Checked)
                {
                    for (int i = 0; i < headTitle.Length; i++)
                    {
                        tcHeader.Add(new TableHeaderCell());
                        tcHeader[i].Text = headTitle[i];
                    }

                }
                else
                {
                    for (int i = 0; i < headTitle.Length - 2; i++)
                    {
                        tcHeader.Add(new TableHeaderCell());
                        tcHeader[i].Text = headTitle[i];
                    }
                }
                break;
        }
        if (e.Row.RowIndex >= 0)
        {
            //每次添加列的时候都要注意是否影响到这里的总计;
            totalOutQty += Convert.ToInt32(e.Row.Cells[13].Text); //Modified by MunFoong on 2014.07.24, MES-135
            //Modified by Zikuan MES-069
            totalOpening += Convert.ToInt32(e.Row.Cells[11].Text); //Modified by MunFoong on 2014.07.24, MES-135
            //End Modified MES-069
            if (CbProcessCloseStatus.Checked)
            {
                totalWipQty += Convert.ToInt32(e.Row.Cells[e.Row.Cells.Count - 3].Text);
            }
            else
            {
                totalWipQty += Convert.ToInt32(e.Row.Cells[e.Row.Cells.Count - 1].Text);
            }

        }

        lblTotalWipQty.Text = totalWipQty.ToString();
        lblTotalOutQty.Text = totalOutQty.ToString();

        //Modified by MF on 20160126,dCT formula change to Day End WIP/ WIP out
        //lblCT.Text = totalOutQty.Equals(0) ? "0" : (((double.Parse(totalWipQty.ToString()) + (double.Parse(totalOpening.ToString()))) / 2) / double.Parse(totalOutQty.ToString())).ToString("f4");
        lblCT.Text = totalOutQty.Equals(0) ? "0" : (double.Parse(totalWipQty.ToString()) / double.Parse(totalOutQty.ToString())).ToString("f4");
        //End of modified by MF on 20160126,dCT formula change to Day End WIP/ WIP out
    }

    protected void gvBody_RowDataBoundDct(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();
                //First Row
                for (int i = 0; i < DetheadTitle.Length; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Text = DetheadTitle[i];
                }
                break;
        }
        //Added by Jin Song MES115
        if (e.Row.Cells[1].Text == "ALL" ||e.Row.Cells[0].Text == "Total DCT:")
        {
            e.Row.Font.Bold = true;
            e.Row.ForeColor = System.Drawing.Color.Black;
        }
        //End of Add MES115
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        totalOutQty = 0;
        totalWipQty = 0;
        lblCT.Text = "";
        lblTotalWipQty.Text = "";
        lblTotalOutQty.Text = "";
        this.lblMessage.Visible = false;
        SetQuery();
    }
    protected void ddlGarmentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlProcessCode.DataSource = MESComment.MesRpt.GetProcessCode(ddlFactoryCode.SelectedItem.Value, ddlGarmentType.SelectedItem.Value);
        ddlProcessCode.DataBind();
        totalOutQty = 0;
        totalWipQty = 0;
        lblCT.Text = "";
        lblTotalWipQty.Text = "";
        lblTotalOutQty.Text = "";
        //Added by Jin Song MES115
        lblTitle.Visible = false;
        //End of add MES115
    }
    protected void ddlProcessCode_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlProductionLine.DataSource = MESComment.MesRpt.GetPrdLine(ddlFactoryCode.SelectedItem.Value, ddlProcessCode.SelectedItem.Value, ddlGarmentType.SelectedItem.Value);
        ddlProductionLine.DataTextField = "PRODUCTION_LINE_NAME";
        ddlProductionLine.DataValueField = "PRODUCTION_LINE_CD";
        ddlProductionLine.DataBind();
        totalOutQty = 0;
        totalWipQty = 0;
        lblCT.Text = "";
        lblTotalWipQty.Text = "";
        lblTotalOutQty.Text = "";
        //Added by Jin Song MES115
        lblTitle.Visible = false;
        //End of add MES115
    }
}
