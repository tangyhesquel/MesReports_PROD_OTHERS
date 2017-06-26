using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.Common;
using System.Collections.Generic;

public partial class IEContractReport : pPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            MESComment.CommFunction.SetDropDownList(ddlContractor, "prd_subcontractor_master", "subcontractor_cd", "subcontractor_name", "MES", base.CurrentSite);

            MESComment.CommFunction.DoAutoComplete("CONTRACT_NO", "PRD_OUTSOURCE_CONTRACT", "CONTRACT_NO", "MES");
            ddlReport.Items.Clear();
            ddlReport.Items.Add(new ListItem("IE单次合同发货情况统计", "Report1"));
            ddlReport.Items.Add(new ListItem("IE单次合同外发收发数情况统计", "Report2"));
            ddlReport.Items.Add(new ListItem("IE单次合同PPO发布数情况统计", "Report3"));
        }

        switch (ddlReport.SelectedValue)
        {
            case "Report1":
                this.ReportTitle.Text = "IE单次合同发货情况统计";
                this.gvBody.Visible = true; this.gvIssue.Visible = false; this.gvPPO.Visible = false;
                break;
            case "Report2":
                this.ReportTitle.Text = "IE单次合同外发收发数情况统计";
                this.gvBody.Visible = false; this.gvIssue.Visible = true; this.gvPPO.Visible = false;
                break;
            case "Report3":
                this.ReportTitle.Text = "IE单次合同PPO发布数情况统计";
                this.gvBody.Visible = false; this.gvIssue.Visible = false; this.gvPPO.Visible = true;
                break;
        }
        this.lblMessage.Visible = false;
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.QueryData();
    }

    /// <summary>
    /// 重载VerifyRenderingInServerForm
    /// 此函数用于excel导出
    /// 是必须要的函数,请不要删除
    /// </summary>
    /// <param name="control"></param>
    public override void VerifyRenderingInServerForm(Control control)
    {
        //base.VerifyRenderingInServerForm(control); 
    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        if (this.QueryData() == false)
            return;

        //List<GridView> gvList = new List<GridView>();
        //gvList.Add(gvBody); gvList.Add(gvIssue); gvList.Add(gvPPO);
        //MESComment.Excel.ToExcel(gvList, "IEContractReport" + DateTime.Today.ToShortDateString());
        switch (ddlReport.SelectedValue)
        {
            case "Report1":
                MESComment.Excel.ToExcel(this.gvBody, "IEContractReport" + DateTime.Today.ToShortDateString());
                break;
            case "Report2":
                MESComment.Excel.ToExcel(this.gvIssue, "IEContractReport" + DateTime.Today.ToShortDateString());
                break;
            case "Report3":
                MESComment.Excel.ToExcel(this.gvPPO, "IEContractReport" + DateTime.Today.ToShortDateString());
                break;
        }


    }

    private bool CheckQueryCondition()
    {

        string strContractNo = this.txtContractNO.Text.Trim();
        string strContract = ddlContractor.SelectedValue;
        string strBeginDate = this.txtBeginDate.Text.Trim();
        string strEndDate = this.txtEndDate.Text.Trim();

        if (strContractNo.Length == 0 && strContract.Length == 0 && (strBeginDate.Length == 0 && strEndDate.Length == 0))
        {
            this.lblMessage.Text = "Please choose condition to query!";
            this.lblMessage.Visible = true;
            return false;
        }
        if (strBeginDate.Length == 0 && strEndDate.Length != 0 || strBeginDate.Length != 0 && strEndDate.Length == 0)
        {
            this.lblMessage.Text = "Please select accurate  Date!";
            this.lblMessage.Visible = true;
            return false;
        }
        return true;
    }

    /*
    private bool QueryData()
    {
        try
        {
            DataTable mainTb = new DataTable("MainTb");
            mainTb.Columns.Add("SUBCONTRACTOR_NAME");
            mainTb.Columns.Add("TRANS_DATE");
            mainTb.Columns.Add("CONTRACT_NO");
            mainTb.Columns.Add("JOB_ORDER_NO");
            mainTb.Columns.Add("ORDER_QTY");
            mainTb.Columns.Add("GOOD_NAME");
            mainTb.Columns.Add("PLAN_ISSUE_QTY");
            mainTb.Columns.Add("PROCESS_REMARK");
            mainTb.Columns.Add("SUB_CONTRACT_PRICE");
            mainTb.Columns.Add("SEND_DPARTMENT");
            mainTb.Columns.Add("RECEIVE_POINT");
            mainTb.Columns.Add("RECEIPT_DEPARTMENT");
            mainTb.Columns.Add("OUTPUT_QTY");
            mainTb.Columns.Add("RECEIPT_QTY");
            mainTb.Columns.Add("PULLOUT_QTY");
            mainTb.Columns.Add("ISSUE_QTY");
            mainTb.Columns.Add("RETURN_QTY");
            mainTb.Columns.Add("STYLE_CHN_DESC");
            mainTb.Columns.Add("REMARK");

            DataTable PPOtb = new DataTable("PPOtb_");
            PPOtb = mainTb.Clone();

            if (false == this.CheckQueryCondition())
                return false;

            string strContractNO = this.txtContractNO.Text.Trim();
            string strContract = this.ddlContractor.SelectedValue == "-1" ? "" : ddlContractor.SelectedValue;
            string strBeginDate = this.txtBeginDate.Text.Trim();
            string strEndDate = this.txtEndDate.Text.Trim();

            string jobList = string.Empty;
            string FactoryCD;
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                FactoryCD = Request.QueryString["site"].ToString().ToUpper();
            }
            else
            {
                FactoryCD = "GEG";
            }
            string subcontractor = ddlContractor.SelectedValue.ToString().Trim();
            //DataTable JOList = MESComment.IEContractReport.GetIEContractReport_JOList_data(FactoryCD, strContractNO, subcontractor, strBeginDate, strEndDate);
            DataTable JOList = GetIEContractReport_JOList_data(FactoryCD, strContractNO, subcontractor, strBeginDate, strEndDate);

            if (JOList.Rows.Count == 0)
            {
                this.lblMessage.Text = "No data found!";
                this.lblMessage.Visible = true;
                return false;
            }

            DataTable wipdt = GetIEContractReport_WIP_data(FactoryCD, strContractNO, strContract, strBeginDate, strEndDate);
            if (wipdt.Rows.Count == 0)
            {
                this.lblMessage.Text = "No data found!";
                this.lblMessage.Visible = true;
            }
            if (ddlReport.SelectedValue == "Report2")
            {
                this.gvBody.Visible = false;
                this.gvPPO.Visible = false;
                this.gvIssue.DataSource = wipdt;
                this.gvIssue.DataBind();
                return true;
            }
            //添加查询条件语句
            for (int count = 0; count < wipdt.Rows.Count; count++)
            {
                jobList += (0 == count ? "'" + wipdt.Rows[count]["JOB_ORDER_NO"].ToString() + " '" : ",'" + wipdt.Rows[count]["JOB_ORDER_NO"].ToString() + "'");
            }
            if (jobList.Length < 0)
            {
                jobList = "''";
            }

            DataTable ppodt = GetIEContractReportData_ppo(jobList, FactoryCD, txtBeginDate.Text, txtEndDate.Text);

            if (wipdt.Rows.Count > 0)
            {
                for (int i = 0; i < wipdt.Rows.Count; i++)
                {
                    DataRow[] checkrows = PPOtb.Select(string.Format("JOB_ORDER_NO='{0}' and CONTRACT_NO='{1}'", wipdt.Rows[i]["JOB_ORDER_NO"].ToString().Trim(), wipdt.Rows[i]["CONTRACT_NO"].ToString().Trim()));
                    if (checkrows.Length == 0)
                    {
                        DataRow[] rows_ = ppodt.Select(string.Format("JOB_ORDER_NO='{0}'", wipdt.Rows[i]["JOB_ORDER_NO"].ToString().Trim()));
                        for (int l = 0; l < rows_.Length; l++)
                        {
                            DataRow row_ = PPOtb.NewRow();
                            row_["SUBCONTRACTOR_NAME"] = wipdt.Rows[i]["SUBCONTRACTOR_NAME"];
                            row_["TRANS_DATE"] = rows_[l]["TRANS_DATE"];
                            row_["CONTRACT_NO"] = wipdt.Rows[i]["CONTRACT_NO"];
                            row_["JOB_ORDER_NO"] = wipdt.Rows[i]["JOB_ORDER_NO"];
                            row_["ORDER_QTY"] = wipdt.Rows[i]["ORDER_QTY"];
                            row_["GOOD_NAME"] = wipdt.Rows[i]["GOOD_NAME"];
                            row_["PLAN_ISSUE_QTY"] = wipdt.Rows[i]["PLAN_ISSUE_QTY"];
                            row_["PROCESS_REMARK"] = wipdt.Rows[i]["PROCESS_REMARK"];
                            row_["SUB_CONTRACT_PRICE"] = wipdt.Rows[i]["SUB_CONTRACT_PRICE"];

                            row_["SEND_DPARTMENT"] = wipdt.Rows[i]["SEND_DPARTMENT"];
                            row_["RECEIVE_POINT"] = wipdt.Rows[i]["RECEIVE_POINT"];
                            row_["RECEIPT_DEPARTMENT"] = wipdt.Rows[i]["RECEIPT_DEPARTMENT"];

                            row_["ISSUE_QTY"] = (rows_[l]["ISSUE_QTY"].ToString().Equals("") ? 0 : rows_[l]["ISSUE_QTY"]);
                            row_["RETURN_QTY"] = (rows_[l]["RETURN_QTY"].ToString().Equals("") ? 0 : rows_[l]["RETURN_QTY"]);
                            row_["STYLE_CHN_DESC"] = rows_[l]["STYLE_CHN_DESC"];
                            row_["REMARK"] = rows_[l]["REMARKS"];
                            PPOtb.Rows.Add(row_);
                        }

                    }
                }
            }
            if (PPOtb.Rows.Count == 0)
            {
                this.lblMessage.Text = "No data found!";
                this.lblMessage.Visible = true;
            }
            if (ddlReport.SelectedValue == "Report3")
            {
                PPOtb.Columns.Remove("SEND_DPARTMENT");
                PPOtb.Columns.Remove("RECEIVE_POINT");
                PPOtb.Columns.Remove("RECEIPT_DEPARTMENT");

                PPOtb.Columns.Remove("OUTPUT_QTY");
                PPOtb.Columns.Remove("RECEIPT_QTY");
                PPOtb.Columns.Remove("PULLOUT_QTY");

                this.gvBody.Visible = false;
                this.gvIssue.Visible = false;

                this.gvPPO.DataSource = PPOtb;
                this.gvPPO.DataBind();
                return true;
            }

            //set gvissue data
            DataRow[] rows = null; DataRow row = null;
            if (wipdt.Rows.Count > 0)
            {
                for (int i = 0; i < wipdt.Rows.Count; i++)
                {
                    row = mainTb.NewRow();
                    //for (int c = 0; c < wipdt.Columns.Count; c++)
                    for (int c = 0; c < 15; c++)
                    {
                        row[c] = wipdt.Rows[i][c];
                    }
                    row[17] = wipdt.Rows[i][15];
                    mainTb.Rows.Add(row);
                }
            }

            for (int i = 0; i < PPOtb.Rows.Count; i++)
            {
                string SelectString = string.Format("TRANS_DATE='{0}' and CONTRACT_NO='{1}'  and JOB_ORDER_NO='{2}' ", PPOtb.Rows[i]["Trans_date"].ToString(), PPOtb.Rows[i]["CONTRACT_NO"].ToString(), PPOtb.Rows[i]["JOB_ORDER_NO"].ToString());
                rows = mainTb.Select(SelectString);
                if (rows.Length > 0)
                {
                    for (int r = 0; r < rows.Length; r++)
                    {
                        rows[r]["ISSUE_QTY"] = (PPOtb.Rows[i]["ISSUE_QTY"].ToString().Equals("") ? 0 : PPOtb.Rows[i]["ISSUE_QTY"]);
                        rows[r]["RETURN_QTY"] = (PPOtb.Rows[i]["RETURN_QTY"].ToString().Equals("") ? 0 : PPOtb.Rows[i]["RETURN_QTY"]);
                        rows[r]["REMARK"] = PPOtb.Rows[i]["REMARK"];
                    }
                }
                else
                {
                    SelectString = string.Format(" ISNULL(TRANS_DATE,'')='{0}' and CONTRACT_NO='{1}'  and JOB_ORDER_NO='{2}' ", "", PPOtb.Rows[i]["CONTRACT_NO"].ToString(), PPOtb.Rows[i]["JOB_ORDER_NO"].ToString());
                    DataRow[] rows_for_del = mainTb.Select(SelectString);
                    if (rows_for_del.Length > 0)
                    {
                        for (int r = 0; r < rows_for_del.Length; r++)
                        {
                            rows_for_del[r].Delete();
                        }
                    }
                    row = mainTb.NewRow();
                    row["SUBCONTRACTOR_NAME"] = PPOtb.Rows[i]["SUBCONTRACTOR_NAME"];
                    row["TRANS_DATE"] = PPOtb.Rows[i]["TRANS_DATE"];
                    row["CONTRACT_NO"] = PPOtb.Rows[i]["CONTRACT_NO"];
                    row["JOB_ORDER_NO"] = PPOtb.Rows[i]["JOB_ORDER_NO"];
                    row["ORDER_QTY"] = PPOtb.Rows[i]["ORDER_QTY"];
                    row["GOOD_NAME"] = PPOtb.Rows[i]["GOOD_NAME"];
                    row["PLAN_ISSUE_QTY"] = PPOtb.Rows[i]["PLAN_ISSUE_QTY"];
                    row["PROCESS_REMARK"] = PPOtb.Rows[i]["PROCESS_REMARK"];
                    row["SUB_CONTRACT_PRICE"] = PPOtb.Rows[i]["SUB_CONTRACT_PRICE"];

                    row["SEND_DPARTMENT"] = PPOtb.Rows[i]["SEND_DPARTMENT"];
                    row["RECEIVE_POINT"] = PPOtb.Rows[i]["RECEIVE_POINT"];
                    row["RECEIPT_DEPARTMENT"] = PPOtb.Rows[i]["RECEIPT_DEPARTMENT"];

                    row["OUTPUT_QTY"] = (PPOtb.Rows[i]["OUTPUT_QTY"].ToString().Equals("") ? 0 : PPOtb.Rows[i]["OUTPUT_QTY"]);
                    row["RECEIPT_QTY"] = (PPOtb.Rows[i]["RECEIPT_QTY"].ToString().Equals("") ? 0 : PPOtb.Rows[i]["RECEIPT_QTY"]);
                    row["PULLOUT_QTY"] = (PPOtb.Rows[i]["PULLOUT_QTY"].ToString().Equals("") ? 0 : PPOtb.Rows[i]["PULLOUT_QTY"]);

                    row["TRANS_DATE"] = PPOtb.Rows[i]["Trans_date"];
                    row["ISSUE_QTY"] = (PPOtb.Rows[i]["ISSUE_QTY"].ToString().Equals("") ? 0 : PPOtb.Rows[i]["ISSUE_QTY"]);
                    row["RETURN_QTY"] = (PPOtb.Rows[i]["RETURN_QTY"].ToString().Equals("") ? 0 : PPOtb.Rows[i]["RETURN_QTY"]);
                    row["STYLE_CHN_DESC"] = PPOtb.Rows[i]["STYLE_CHN_DESC"];
                    row["REMARK"] = PPOtb.Rows[i]["REMARK"];
                    mainTb.Rows.Add(row);

                }
            }

            if (mainTb.Rows.Count == 0)
            {

                this.lblMessage.Text = "No data found!";
                this.lblMessage.Visible = true;
            }
            this.lblMessage.Text = "";
            DataView body_v = new DataView(mainTb);
            body_v.RowFilter = "OUTPUT_QTY<>0 OR RECEIPT_QTY<>0 OR PULLOUT_QTY<>0 OR ISSUE_QTY<>'' OR RETURN_QTY<>''OR ISSUE_QTY='0' OR RETURN_QTY='0'"; //Added By ZouShiChang ON 2014.05.06

            body_v.Sort = "SUBCONTRACTOR_NAME,CONTRACT_NO,JOB_ORDER_NO,TRANS_DATE";

            if (ddlReport.SelectedValue == "Report1")
            {
                this.gvPPO.Visible = false;
                this.gvIssue.Visible = false;
                this.gvBody.DataSource = body_v;
                this.gvBody.DataBind();
            }


            return true;
        }
        catch (Exception ex)
        {
            this.lblMessage.Text = ex.Message;
            this.lblMessage.Visible = true;
            return false;
        }
    }
    */
    private bool QueryData()
    {
        try
        {
            DataTable mainTb = new DataTable("MainTb");
            mainTb.Columns.Add("SUBCONTRACTOR_NAME");
            mainTb.Columns.Add("TRANS_DATE");
            mainTb.Columns.Add("CONTRACT_NO");
            mainTb.Columns.Add("JOB_ORDER_NO");
            mainTb.Columns.Add("ORDER_QTY");
            mainTb.Columns.Add("GOOD_NAME");
            mainTb.Columns.Add("PLAN_ISSUE_QTY");
            mainTb.Columns.Add("PROCESS_REMARK");
            mainTb.Columns.Add("SUB_CONTRACT_PRICE");
            mainTb.Columns.Add("SEND_DPARTMENT");
            mainTb.Columns.Add("RECEIVE_POINT");
            mainTb.Columns.Add("RECEIPT_DEPARTMENT");
            mainTb.Columns.Add("OUTPUT_QTY");
            mainTb.Columns.Add("RECEIPT_QTY");
            mainTb.Columns.Add("PULLOUT_QTY");
            mainTb.Columns.Add("ISSUE_QTY");
            mainTb.Columns.Add("RETURN_QTY");
            mainTb.Columns.Add("STYLE_CHN_DESC");
            mainTb.Columns.Add("REMARK");

            DataTable PPOtb = new DataTable("PPOtb_");
            PPOtb = mainTb.Clone();

            if (false == this.CheckQueryCondition())
                return false;

            string strContractNO = this.txtContractNO.Text.Trim();
            string strContract = this.ddlContractor.SelectedValue == "-1" ? "" : ddlContractor.SelectedValue;
            string strBeginDate = this.txtBeginDate.Text.Trim();
            string strEndDate = this.txtEndDate.Text.Trim();

            string jobList = string.Empty;
            string FactoryCD;
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                FactoryCD = Request.QueryString["site"].ToString().ToUpper();
            }
            else
            {
                FactoryCD = "GEG";
            }
            string subcontractor = ddlContractor.SelectedValue.ToString().Trim();

            DbConnection MESConn = MESComment.DBUtility.GetConnection("MES");

            string SQL = @"  IF OBJECT_ID('tempdb..#TEMP_JO') IS NOT NULL
                                BEGIN        
                                        DROP TABLE #TEMP_JO 
                                END;
                         SELECT JO_NO INTO #TEMP_JO FROM JO_HD WHERE 1=2";

            MESComment.DBUtility.GetTable(SQL, MESConn);



            //DataTable MesJOList;
            //DataTable JOList = MESComment.IEContractReport.GetIEContractReport_JOList_data(FactoryCD, strContractNO, subcontractor, strBeginDate, strEndDate);
            if (strContractNO != "" && strBeginDate == "" && strEndDate == "")
            {
                //MesJOList = GetIEContractReport_MesJOList_data_ContractNo(FactoryCD, strContractNO, subcontractor);
                MESComment.DBUtility.GetTable(GetIEContractReport_MesJOList_data_ContractNo(FactoryCD, strContractNO, subcontractor), MESConn);
            }
            else
            {
                //MesJOList = GetIEContractReport_MesJOList_data(FactoryCD, strContractNO, subcontractor, strBeginDate, strEndDate);
                MESComment.DBUtility.GetTable(GetIEContractReport_MesJOList_data(FactoryCD, strContractNO, subcontractor, strBeginDate, strEndDate), MESConn);
            }

            string JOList = "";
            //if (MesJOList.Rows.Count > 0)
            //{
            //    JOList = MesJOList.Rows[0][0].ToString();
            //}
            if (strBeginDate != "" && strEndDate != "")
            {
                DataTable InvJOList = GetIEContractReport_InvJOList_data(FactoryCD, txtBeginDate.Text, txtEndDate.Text);
                for (int i = 0; i < InvJOList.Rows.Count; i++)
                {
                    JOList = JOList + "INSERT INTO #TEMP_JO VALUES ('" + InvJOList.Rows[i][0].ToString().Trim() + "');";
                }
                MESComment.DBUtility.GetTable(JOList, MESConn);
            }

            //if (JOList == "")
            //{
            //    this.lblMessage.Text = "No data found!";
            //    this.lblMessage.Visible = true;
            //    return false;
            //}

            //JOList = JOList.Substring(0, JOList.Length - 1);



            DataTable wipdt = GetIEContractReport_WIP_data(JOList, FactoryCD, strContractNO, strContract, strBeginDate, strEndDate, MESConn);
            if (wipdt.Rows.Count == 0)
            {
                this.lblMessage.Text = "No data found!";
                this.lblMessage.Visible = true;
                this.gvBody.Visible = false;
                return false;
            }
            if (ddlReport.SelectedValue == "Report2")
            {
                this.gvBody.Visible = false;
                this.gvPPO.Visible = false;
                this.gvIssue.DataSource = wipdt;
                this.gvIssue.DataBind();
                return true;
            }

            //添加查询条件语句
            for (int count = 0; count < wipdt.Rows.Count; count++)
            {
                jobList += (0 == count ? "'" + wipdt.Rows[count]["JOB_ORDER_NO"].ToString().Trim() + "'" : ",'" + wipdt.Rows[count]["JOB_ORDER_NO"].ToString().Trim() + "'");
            }
            if (jobList.Length < 0)
            {
                jobList = "''";
            }


            DataTable ppodt = GetIEContractReportData_ppo(jobList, FactoryCD, txtBeginDate.Text, txtEndDate.Text);

            if (wipdt.Rows.Count > 0)
            {
                for (int i = 0; i < wipdt.Rows.Count; i++)
                {
                    DataRow[] checkrows = PPOtb.Select(string.Format("JOB_ORDER_NO='{0}' and CONTRACT_NO='{1}'", wipdt.Rows[i]["JOB_ORDER_NO"].ToString().Trim(), wipdt.Rows[i]["CONTRACT_NO"].ToString().Trim()));
                    if (checkrows.Length == 0)
                    {
                        DataRow[] rows_ = ppodt.Select(string.Format("JOB_ORDER_NO='{0}'", wipdt.Rows[i]["JOB_ORDER_NO"].ToString().Trim()));
                        for (int l = 0; l < rows_.Length; l++)
                        {
                            DataRow row_ = PPOtb.NewRow();
                            row_["SUBCONTRACTOR_NAME"] = wipdt.Rows[i]["SUBCONTRACTOR_NAME"];
                            row_["TRANS_DATE"] = rows_[l]["TRANS_DATE"];
                            row_["CONTRACT_NO"] = wipdt.Rows[i]["CONTRACT_NO"];
                            row_["JOB_ORDER_NO"] = wipdt.Rows[i]["JOB_ORDER_NO"];
                            row_["ORDER_QTY"] = wipdt.Rows[i]["ORDER_QTY"];
                            row_["GOOD_NAME"] = wipdt.Rows[i]["GOOD_NAME"];
                            row_["PLAN_ISSUE_QTY"] = wipdt.Rows[i]["PLAN_ISSUE_QTY"];
                            row_["PROCESS_REMARK"] = wipdt.Rows[i]["PROCESS_REMARK"];
                            row_["SUB_CONTRACT_PRICE"] = wipdt.Rows[i]["SUB_CONTRACT_PRICE"];

                            row_["SEND_DPARTMENT"] = wipdt.Rows[i]["SEND_DPARTMENT"];
                            row_["RECEIVE_POINT"] = wipdt.Rows[i]["RECEIVE_POINT"];
                            row_["RECEIPT_DEPARTMENT"] = wipdt.Rows[i]["RECEIPT_DEPARTMENT"];

                            row_["ISSUE_QTY"] = (rows_[l]["ISSUE_QTY"].ToString().Equals("") ? 0 : rows_[l]["ISSUE_QTY"]);
                            row_["RETURN_QTY"] = (rows_[l]["RETURN_QTY"].ToString().Equals("") ? 0 : rows_[l]["RETURN_QTY"]);
                            row_["STYLE_CHN_DESC"] = rows_[l]["STYLE_CHN_DESC"];
                            row_["REMARK"] = rows_[l]["REMARKS"];
                            PPOtb.Rows.Add(row_);
                        }

                    }
                }
            }
            if (PPOtb.Rows.Count == 0)
                //{
                //    this.lblMessage.Text = "No data found!";
                //    this.lblMessage.Visible = true;
                //    return false;
                //}
                if (ddlReport.SelectedValue == "Report3")
                {
                    PPOtb.Columns.Remove("SEND_DPARTMENT");
                    PPOtb.Columns.Remove("RECEIVE_POINT");
                    PPOtb.Columns.Remove("RECEIPT_DEPARTMENT");

                    PPOtb.Columns.Remove("OUTPUT_QTY");
                    PPOtb.Columns.Remove("RECEIPT_QTY");
                    PPOtb.Columns.Remove("PULLOUT_QTY");

                    this.gvBody.Visible = false;
                    this.gvIssue.Visible = false;

                    this.gvPPO.DataSource = PPOtb;
                    this.gvPPO.DataBind();
                    return true;
                }

            //set gvissue data
            DataRow[] rows = null; DataRow row = null;
            if (wipdt.Rows.Count > 0)
            {
                for (int i = 0; i < wipdt.Rows.Count; i++)
                {
                    row = mainTb.NewRow();
                    //for (int c = 0; c < wipdt.Columns.Count; c++)
                    for (int c = 0; c < 15; c++)
                    {
                        row[c] = wipdt.Rows[i][c];
                    }
                    row[17] = wipdt.Rows[i][15];
                    mainTb.Rows.Add(row);
                }
            }

            for (int i = 0; i < PPOtb.Rows.Count; i++)
            {
                string SelectString = string.Format("TRANS_DATE='{0}' and CONTRACT_NO='{1}'  and JOB_ORDER_NO='{2}' ", PPOtb.Rows[i]["Trans_date"].ToString(), PPOtb.Rows[i]["CONTRACT_NO"].ToString(), PPOtb.Rows[i]["JOB_ORDER_NO"].ToString());
                rows = mainTb.Select(SelectString);
                if (rows.Length > 0)
                {
                    for (int r = 0; r < rows.Length; r++)
                    {
                        rows[r]["ISSUE_QTY"] = (PPOtb.Rows[i]["ISSUE_QTY"].ToString().Equals("") ? 0 : PPOtb.Rows[i]["ISSUE_QTY"]);
                        rows[r]["RETURN_QTY"] = (PPOtb.Rows[i]["RETURN_QTY"].ToString().Equals("") ? 0 : PPOtb.Rows[i]["RETURN_QTY"]);
                        rows[r]["REMARK"] = PPOtb.Rows[i]["REMARK"];
                    }
                }
                else
                {
                    SelectString = string.Format(" ISNULL(TRANS_DATE,'')='{0}' and CONTRACT_NO='{1}'  and JOB_ORDER_NO='{2}' ", "", PPOtb.Rows[i]["CONTRACT_NO"].ToString(), PPOtb.Rows[i]["JOB_ORDER_NO"].ToString());
                    DataRow[] rows_for_del = mainTb.Select(SelectString);
                    if (rows_for_del.Length > 0)
                    {
                        for (int r = 0; r < rows_for_del.Length; r++)
                        {
                            rows_for_del[r].Delete();
                        }
                    }
                    row = mainTb.NewRow();
                    row["SUBCONTRACTOR_NAME"] = PPOtb.Rows[i]["SUBCONTRACTOR_NAME"];
                    row["TRANS_DATE"] = PPOtb.Rows[i]["TRANS_DATE"];
                    row["CONTRACT_NO"] = PPOtb.Rows[i]["CONTRACT_NO"];
                    row["JOB_ORDER_NO"] = PPOtb.Rows[i]["JOB_ORDER_NO"];
                    row["ORDER_QTY"] = PPOtb.Rows[i]["ORDER_QTY"];
                    row["GOOD_NAME"] = PPOtb.Rows[i]["GOOD_NAME"];
                    row["PLAN_ISSUE_QTY"] = PPOtb.Rows[i]["PLAN_ISSUE_QTY"];
                    row["PROCESS_REMARK"] = PPOtb.Rows[i]["PROCESS_REMARK"];
                    row["SUB_CONTRACT_PRICE"] = PPOtb.Rows[i]["SUB_CONTRACT_PRICE"];

                    row["SEND_DPARTMENT"] = PPOtb.Rows[i]["SEND_DPARTMENT"];
                    row["RECEIVE_POINT"] = PPOtb.Rows[i]["RECEIVE_POINT"];
                    row["RECEIPT_DEPARTMENT"] = PPOtb.Rows[i]["RECEIPT_DEPARTMENT"];

                    row["OUTPUT_QTY"] = (PPOtb.Rows[i]["OUTPUT_QTY"].ToString().Equals("") ? 0 : PPOtb.Rows[i]["OUTPUT_QTY"]);
                    row["RECEIPT_QTY"] = (PPOtb.Rows[i]["RECEIPT_QTY"].ToString().Equals("") ? 0 : PPOtb.Rows[i]["RECEIPT_QTY"]);
                    row["PULLOUT_QTY"] = (PPOtb.Rows[i]["PULLOUT_QTY"].ToString().Equals("") ? 0 : PPOtb.Rows[i]["PULLOUT_QTY"]);

                    row["TRANS_DATE"] = PPOtb.Rows[i]["Trans_date"];
                    row["ISSUE_QTY"] = (PPOtb.Rows[i]["ISSUE_QTY"].ToString().Equals("") ? 0 : PPOtb.Rows[i]["ISSUE_QTY"]);
                    row["RETURN_QTY"] = (PPOtb.Rows[i]["RETURN_QTY"].ToString().Equals("") ? 0 : PPOtb.Rows[i]["RETURN_QTY"]);
                    row["STYLE_CHN_DESC"] = PPOtb.Rows[i]["STYLE_CHN_DESC"];
                    row["REMARK"] = PPOtb.Rows[i]["REMARK"];
                    mainTb.Rows.Add(row);

                }
            }

            if (mainTb.Rows.Count == 0)
            {

                this.lblMessage.Text = "No data found!";
                this.lblMessage.Visible = true;
                this.gvBody.Visible = false;
            }
            this.lblMessage.Text = "";
            DataView body_v = new DataView(mainTb);
            body_v.RowFilter = "OUTPUT_QTY<>0 OR RECEIPT_QTY<>0 OR PULLOUT_QTY<>0 OR ISSUE_QTY<>'' OR RETURN_QTY<>''OR ISSUE_QTY='0' OR RETURN_QTY='0'"; //Added By ZouShiChang ON 2014.05.06

            body_v.Sort = "SUBCONTRACTOR_NAME,CONTRACT_NO,JOB_ORDER_NO,TRANS_DATE";

            if (ddlReport.SelectedValue == "Report1")
            {
                this.gvPPO.Visible = false;
                this.gvIssue.Visible = false;
                this.gvBody.DataSource = body_v;
                this.gvBody.DataBind();
            }

            return true;
        }
        catch (Exception ex)
        {
            this.lblMessage.Text = ex.Message;
            this.lblMessage.Visible = true;
            this.gvBody.Visible = false;
            return false;
        }
    }




    string gvHeaderStr = "外发加工商<br>Subcontractor Name,收发时间<br>Trans Date,合同号<br>Contract No,制单号<br>Job Order No,订单数<br>Order QTY,规格<br>Good Name,计划外发数<br>Plan Issue QTY,外发工序<br>Process Remark," +
                    "含税加工单价<br>Subcontracting　Price (With Tax),发出部门<br>Send Department,接收点<br>Receive Point,接收部门<br>Receipt Department,发出数量<br>Output Qty,接收数量<br>Receipt Qty,下数<br>Pullout Qty,发布数<br>Issue Qty,退布数<br>Return Qty,款式描述<br>Style Chn Desc,备注<br>Remark";
    string gvIssueHeaderStr = "外发加工商<br>Subcontractor Name,收发时间<br>Trans Date,合同号<br>Contract No,制单号<br>Job Order No,订单数<br>Order QTY,规格<br>Good Name,计划外发数<br>Plan Issue QTY,外发工序<br>Process Remark," +
                    "含税加工单价<br>Subcontracting　Price (With Tax),发出部门<br>Send Department,接收点<br>Receive Point,接收部门<br>Receipt Department,发出数量<br>Output Qty,接收数量<br>Receipt Qty,下数<br>Pullout Qty,款式描述<br>Style Chn Desc";

    string gvPPOHeaderStr = "外发加工商<br>Subcontractor Name,收发时间<br>Trans Date,合同号<br>Contract No,制单号<br>Job Order No,订单数<br>Order QTY,规格<br>Good Name,计划外发数<br>Plan Issue QTY,外发工序<br>Process Remark," +
                    "含税加工单价<br>Subcontracting　Price (With Tax),PPO发布数<br>Issue Qty,退布数<br>Return Qty,款式描述<br>Style Chn Desc,备注<br>Remark";

    protected void gvIssue_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                string[] hStr = gvIssueHeaderStr.Split(new char[] { ',' });
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Text = hStr[i].ToString();
                }
                break;
            case DataControlRowType.DataRow:

                break;
        }
    }
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                string[] hStr = gvHeaderStr.Split(new char[] { ',' });
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Text = hStr[i].ToString();
                }
                break;
            case DataControlRowType.DataRow:

                break;
        }
    }
    protected void gvPPO_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                string[] hStr = gvPPOHeaderStr.Split(new char[] { ',' });
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Text = hStr[i].ToString();
                }
                break;
            case DataControlRowType.DataRow:

                break;
        }
    }


    private static string GetIEContractReport_MesJOList_data_ContractNo(string FactoryCD, string strContractNO, string subcontractor)
    {
        string SQL = "INSERT INTO #TEMP_JO ";
        SQL = SQL + "SELECT DISTINCT JOB_ORDER_NO FROM PRD_OUTSOURCE_CONTRACT AS A INNER JOIN PRD_OUTSOURCE_CONTRACT_DT AS B ON A.CONTRACT_NO=B.CONTRACT_NO ";
        SQL = SQL + " WHERE 1=1 ";
        if (!subcontractor.Equals("") && !subcontractor.Equals("-1"))
        {
            SQL += " AND A.subcontractor = '" + subcontractor + "'";
        }
        if (!strContractNO.Equals(""))
        {
            SQL += "  AND A.CONTRACT_NO = '" + strContractNO + "'";
        }

        //DataTable dt = MESComment.DBUtility.GetTable(SQL, "MES");
        return SQL;
    }

    private static string GetIEContractReport_MesJOList_data(string FactoryCD, string strContractNO, string subcontractor, string TranDateBegin, string TranDateEnd)
    {
        string SQL = "";
        SQL = "INSERT INTO #TEMP_JO ";
        SQL = SQL + " SELECT DISTINCT JOB_ORDER_NO FROM PRD_JO_OUTPUT_TRX AS A INNER JOIN dbo.PRD_OUTSOURCE_CONTRACT AS B ON B.CONTRACT_NO = A.CONTRACT_NO ";
        SQL = SQL + " WHERE A.FACTORY_CD='" + FactoryCD + "' ";
        if (!subcontractor.Equals("") && !subcontractor.Equals("-1"))
        {
            SQL += " AND subcontractor = '" + subcontractor + "'";
        }
        if (!strContractNO.Equals(""))
        {
            SQL += "  AND A.CONTRACT_NO = '" + strContractNO + "'";
        }

        if (!TranDateBegin.Equals("") && !TranDateEnd.Equals(""))
        {
            SQL += " AND A.TRX_DATE>='" + TranDateBegin + "' AND A.TRX_DATE<='" + TranDateEnd + "'";
        }
        SQL = SQL + " UNION ALL  ";
        SQL = SQL + "  SELECT DISTINCT JOB_ORDER_NO FROM   OAS_SENDING_RECEIVING_HD AS A INNER JOIN OAS_SENDING_RECEIVING_DT AS B ON B.DOC_NO = A.DOC_NO  ";
        SQL = SQL + " INNER JOIN dbo.PRD_OUTSOURCE_CONTRACT AS C ON  A.REF_CONTRACT_NO=C.CONTRACT_NO ";
        SQL = SQL + " WHERE   A.FACTORY_CD='" + FactoryCD + "' ";
        if (!subcontractor.Equals("") && !subcontractor.Equals("-1"))
        {
            SQL += " AND C.subcontractor = '" + subcontractor + "'";
        }
        if (!strContractNO.Equals(""))
        {
            SQL += "  AND C.CONTRACT_NO = '" + strContractNO + "'";
        }
        if (!TranDateBegin.Equals("") && !TranDateEnd.Equals(""))
        {
            SQL = SQL + "  AND CONVERT(NVARCHAR(100), A.CONFIRM_DATE, 23) >= '" + TranDateBegin + "' AND CONVERT(NVARCHAR(100), A.CONFIRM_DATE, 23) <= '" + TranDateEnd + "' ";
        }

        //DataTable dt = MESComment.DBUtility.GetTable(SQL, "MES");
        return SQL;
    }


    private static DataTable GetIEContractReport_InvJOList_data(string FactoryCD, string txtBeginDate, string txtEndDate)
    {

        string sql = "";
        sql += " SELECT DISTINCT l.job_order_no ";
        sql += " FROM inv_issue_hd h, inv_issue_line l, inv_item i, sc_hd a, po_hd b    ";
        sql += " WHERE h.issue_hd_id = l.issue_hd_id      AND l.inv_item_id = i.inv_item_id      ";
        sql += " AND h.inv_fty_cd = '" + FactoryCD + "'      AND h.status = 'F'      AND h.item_type_cd = 'F'      ";
        sql += " AND l.job_order_no = b.PO_NO      AND a.sc_no = b.sc_no      AND i.product_category IN ('K', 'W')      ";
        sql += " AND h.trans_cd NOT IN ('ITSK', 'ITSW') ";
        if (!txtBeginDate.Equals("") && !txtEndDate.Equals(""))
        {
            sql += " AND to_char(h.trans_date,'yyyy-mm-dd') >='" + txtBeginDate + "' AND to_char(h.trans_date,'yyyy-mm-dd') <='" + txtEndDate + "'";
        }
        sql += " OR h.trans_date IS NULL";


        DataTable dt = MESComment.DBUtility.GetTable(sql, "INV");
        return dt;
    }


    private static DataTable GetIEContractReport_WIP_data(string job_order_no, string FactoryCD, string ContractNO, string subcontractor, string TranDateBegin, string TranDateEnd, DbConnection dbCon)
    {




        string strNO1Where = ""; string strNO2Where = ""; string strDateWhere = ""; string strDate2Where = ""; string subcontractorWhere = "";
        string JOListWhere = "";
        if (ContractNO != string.Empty)
        {
            strNO1Where = " and a.contract_no in( '" + ContractNO + "') ";
            strNO2Where = " and d.contract_no in( '" + ContractNO + "') ";
        }
        else
        {
            JOListWhere = " AND EXISTS(";
            JOListWhere += " SELECT 1 FROM prd_outsource_contract A,prd_outsource_contract_dt B";
            JOListWhere += " WHERE A.CONTRACT_NO = B.CONTRACT_NO";
            if (!subcontractor.Equals(""))
            {
                JOListWhere += "  AND subcontractor = '" + subcontractor + "'";
            }
            JOListWhere += "  AND FACTORY_CD = '" + FactoryCD + "'";
            if (!TranDateBegin.Equals("") && !TranDateEnd.Equals(""))
            {
                JOListWhere += "  AND ISSUER_DATE >=DATEADD(MONTH,-3,'" + TranDateBegin + "')";
                JOListWhere += "  AND ISSUER_DATE <=DATEADD(MONTH,+3,'" + TranDateEnd + "')";
            }
            JOListWhere += "  AND JOB_ORDER_NO = M.JOB_ORDER_NO) ";

        }
        if (TranDateBegin != string.Empty)
        {
            strDateWhere = " and ((a.trx_date >=cast('" + TranDateBegin + "' as datetime)  AND a.trx_date <=cast('" + TranDateEnd + "' as datetime)) OR  a.trx_date IS NULL)  ";
            strDate2Where = " and ((trx_date >=cast('" + TranDateBegin + "' as datetime)  AND trx_date <=cast('" + TranDateEnd + "' as datetime)) OR  trx_date IS NULL) ";
        }
        if (subcontractor != string.Empty)
            subcontractorWhere = " and c.subcontractor_cd = '" + subcontractor + "'  ";

        //Modified by MF on 20150516, filter prd_outsource_contract with status <> 'CAN'
        StringBuilder sqlbu = new StringBuilder();
        sqlbu.AppendFormat("select SUBCONTRACTOR_NAME,CONVERT(varchar(100), N.TRX_DATE, 23) as Trans_date , M.CONTRACT_NO, M.JOB_ORDER_NO,ORDER_QTY,GOOD_NAME,plan_issue_qty,PROCESS_REMARK,SUB_CONTRACT_PRICE,SEND_DPARTMENT,RECEIVE_POINT, " +
                               "         RECEIPT_DEPARTMENT, isnull(N.output_qty,0) as OUTPUT_QTY,isnull(N.RECEIPT_QTY,0) as RECEIPT_QTY, isnull(N.PULLOUT_QTY,0) as PULLOUT_QTY, STYLE_CHN_DESC  " +
                               "     from " +
                               "         (select c.subcontractor_name,a.contract_no,	b.job_order_no,	B.GOOD_NAME ,sum(b.plan_issue_qty) as plan_issue_qty,(select sum(qty) from jo_dt where jo_no=b.job_order_no) AS order_qty,b.process_remark, " +
                               "            ISNULL(b.SUB_CONTRACT_PRICE,0) AS SUB_CONTRACT_PRICE,a.SEND_Dpartment,	a.receive_point, " +
                               "             a.Receipt_Department,	b.send_id,	g.style_chn_desc " +
                               "         from prd_outsource_contract a,prd_outsource_contract_dt b,prd_subcontractor_master c,Jo_hd d,sc_hd g " +
                               "         where " +
                               "             c.subcontractor_cd=a.subcontractor AND a.contract_no = b.contract_no  AND b.job_order_no = d.JO_NO AND d.sc_no = g.sc_no " +
                               "             and d.sc_no=g.sc_no {0} {1}" +
                               "             and b.JOB_ORDER_NO IN (SELECT JO_NO FROM #TEMP_JO) and a.status<>'CAN'" +
                               "             group by c.subcontractor_name, a.contract_no, b.job_order_no, g.style_chn_desc, b.process_remark, B.GOOD_NAME, " +
                               "             a.SEND_Dpartment, a.receive_point, a.Receipt_Department, b.send_id,b.sub_contract_price) M " +
                               "         LEFT join " +
                               "         ( select tot.trx_date,tot.JOB_ORDER_NO,tot.contract_no,tot.send_id,sum(isnull(tot.output_qty,0)) as OUTPUT_QTY, " +
                               "             sum(isnull(tot.receipt_qty,0))as RECEIPT_QTY,sum(isnull(tot.PULLOUT_QTY,0)) as PULLOUT_QTY " +
                               "           from( " +
                               "                 select a.trx_date,b.Job_order_no,b.contract_no,b.send_id,output_QTY=(case when b.Process_cd=d.SEND_DPARTMENT and b.PROCESS_GARMENT_TYPE=D.GARMENT_TYPE and b.process_type=d.SENDER_PROCESS_TYPE then sum(isnull(b.output_qty,0)) else 0 end),  " +
                               "                     RECEIPT_QTY=(case when  b.PROCESS_cd=d.receive_point and b.PROCESS_GARMENT_TYPE=D.GARMENT_TYPE and b.PROCESS_TYPE=d.RECEIVER_PROCESS_TYPE then sum(isnull(b.output_qty,0)) else 0 end),0 as PULLOUT_QTY  " +
                               "                  from prd_jo_output_hd a join prd_jo_output_trx b on a.doc_no=b.doc_no join prd_outsource_contract d on d.factory_cd='" + FactoryCD + "' " +
                               "                       and a.factory_cd=d.factory_cd and ((b.Process_cd=d.SEND_DPARTMENT and b.PROCESS_GARMENT_TYPE=D.GARMENT_TYPE and b.process_type=d.SENDER_PROCESS_TYPE ) or (b.PROCESS_cd=d.receive_point and b.PROCESS_GARMENT_TYPE=D.GARMENT_TYPE and b.PROCESS_TYPE=d.RECEIVER_PROCESS_TYPE )) " +
                               "                       join prd_outsource_contract_dt e on  b.job_order_no=e.job_order_no " +
                               "                       and b.contract_no=e.contract_no and d.contract_no=e.contract_no and b.send_id=e.send_id and b.output_qty<>0 " +
                               "                  WHERE a.factory_cd='" + FactoryCD + "'  {3}{2} " +
                               "             and d.status<>'CAN' group by a.trx_date,b.job_order_no,b.contract_no,b.send_id,b.Process_cd,B.PROCESS_TYPE,B.PROCESS_GARMENT_TYPE,d.SEND_DPARTMENT,d.GARMENT_TYPE,d.SENDER_PROCESS_TYPE,d.RECEIVER_PROCESS_TYPE,a.Next_PROCESS_cd,d.receive_point  " +
                               "             union " +
                               "             select a.trx_date,b.job_order_no,b.contract_no,b.send_id,0 as output_qty,0 as RECEIPT_QTY, " +
                               "                     SUM(ISNULL(c.PULLOUT_QTY,0)) AS PULLOUT_QTY  " +
                               "             from PRD_JO_DISCREPANCY_PULLOUT_HD a  " +
                               "                     JOIN PRD_JO_DISCREPANCY_PULLOUT_TRX b ON A.DOC_NO=B.DOC_NO AND A.FACTORY_CD=B.FACTORY_CD AND A.TRX_DATE=B.TRX_DATE  " +
                               "                     JOIN PRD_JO_PULLOUT_REASON c ON c.TRX_ID=B.TRX_ID AND c.FACTORY_CD=A.FACTORY_CD and c.REASON_CD in('GEG025','GEG026') and c.PULLOUT_QTY<>0 " +
                               "                     join prd_outsource_contract d on d.factory_cd='" + FactoryCD + "' and a.factory_cd=d.factory_cd and a.PROCESS_cd=d.receive_point and a.PROCESS_TYPE=d.RECEIVER_PROCESS_TYPE and A.GARMENT_TYPE=D.GARMENT_TYPE " +
                               "                     join prd_outsource_contract_dt e on  b.job_order_no=e.job_order_no and b.contract_no=e.contract_no and d.contract_no=e.contract_no and b.send_id=e.send_id " +
                               "             WHERE d.factory_cd='" + FactoryCD + "' {3}{2} and d.status<>'CAN'" +
                               "             GROUP BY  a.trx_date,B.JOB_ORDER_NO,b.contract_no,b.send_id  " +
            //Added By ZouShiChang On 2014.04.18 Start
                               @"   UNION
                                        SELECT  CAST(Trans_date AS DATETIME ) AS trx_date ,
                                        JOB_ORDER_NO ,
                                        CONTRACT_NO AS contract_no ,                                        
                                        SEND_ID ,
                                        SUM(sendQty) AS OUTPUT_QTY ,
                                        SUM(recQty) AS RECEIPT_QTY ,
                                        SUM(disqty) AS PULLOUT_QTY
                                FROM    ( SELECT    CONVERT(NVARCHAR(100), a.CONFIRM_DATE, 23) AS Trans_date ,
                                                    a.REF_CONTRACT_NO AS CONTRACT_NO ,
                                                    b.JOB_ORDER_NO ,
                                                    B.SEND_ID ,
                                                    SUM(b.QTY) AS sendQty ,
                                                    0 AS recQty ,
                                                    0 AS disqty
                                          FROM      dbo.OAS_SENDING_RECEIVING_HD a
                                                    INNER JOIN dbo.OAS_SENDING_RECEIVING_DT b ON b.DOC_NO = a.DOC_NO
                                          WHERE     a.TRX_TYPE = 'S'
                                                    AND IS_REAL_SENDING_RECEIVING = 'Y'
                                                    AND STATUS = 'C'
                                                    AND FACTORY_CD='" + FactoryCD + @"'
                                          GROUP BY  a.REF_CONTRACT_NO ,
                                                    b.JOB_ORDER_NO ,
                                                    CONVERT(NVARCHAR(100), a.CONFIRM_DATE, 23) ,
                                                    B.SEND_ID
                                          UNION ALL
                                          SELECT    CONVERT(NVARCHAR(100), a.CONFIRM_DATE, 23) AS Trans_date ,
                                                    a.REF_CONTRACT_NO AS CONTRACT_NO ,
                                                    b.JOB_ORDER_NO ,
                                                    B.SEND_ID ,
                                                    0 AS sendQty ,
                                                    SUM(b.QTY) AS recQty ,
                                                    0 AS disqty
                                          FROM      dbo.OAS_SENDING_RECEIVING_HD a
                                                    INNER JOIN dbo.OAS_SENDING_RECEIVING_DT b ON b.DOC_NO = a.DOC_NO
                                          WHERE     a.TRX_TYPE = 'R'
                                                    AND IS_REAL_SENDING_RECEIVING = 'Y'
                                                    AND STATUS = 'C'
                                                    AND FACTORY_CD='" + FactoryCD + @"'
                                          GROUP BY  a.REF_CONTRACT_NO ,
                                                    b.JOB_ORDER_NO ,
                                                    CONVERT(NVARCHAR(100), a.CONFIRM_DATE, 23) ,
                                                    B.SEND_ID
                                          UNION ALL
                                          SELECT    CONVERT(NVARCHAR(100), a.CONFIRM_DATE, 23) AS Trans_date ,
                                                    a.REF_CONTRACT_NO AS CONTRACT_NO ,
                                                    b.JOB_ORDER_NO ,
                                                    B.SEND_ID ,
                                                    0 AS sendQty ,
                                                    0 AS recQty ,
                                                    SUM(c.QTY) AS disqty
                                          FROM      dbo.OAS_SENDING_RECEIVING_HD A
                                                    INNER JOIN dbo.OAS_SENDING_RECEIVING_DT B ON B.DOC_NO = A.DOC_NO
                                                    INNER JOIN dbo.OAS_SENDING_RECEIVING_DISCREPANCY C ON C.TRX_ID = B.TRX_ID
                                          WHERE     A.TRX_TYPE = 'R'
                                                    AND IS_REAL_SENDING_RECEIVING = 'Y'
                                                    AND STATUS = 'C'
                                                    AND C.REASON_CD IN ( 'GEG025', 'GEG026' )
                                                    AND FACTORY_CD='" + FactoryCD + @"'
                                          GROUP BY  a.REF_CONTRACT_NO ,
                                                    b.JOB_ORDER_NO ,
                                                    CONVERT(NVARCHAR(100), a.CONFIRM_DATE, 23) ,
                                                    B.SEND_ID
                                        ) A
                                /*WHERE Trans_date>='" + TranDateBegin + @"' AND Trans_date<='" + TranDateEnd + @"'      */
                                GROUP BY Trans_date ,
                                        CONTRACT_NO ,
                                        JOB_ORDER_NO ,
                                        SEND_ID " +

                               //Added By ZouShiChang On 2014.04.18 End
                               " ) tot " +
                               " where 1=1 {4} " +
                               "         group by tot.trx_date,tot.JOB_ORDER_NO,tot.contract_no,tot.send_id " +
                               "         ) N " +
                               "         on M.job_order_no=N.job_order_no and M.SEND_ID=N.SEND_ID " +
                                "          order by SUBCONTRACTOR_NAME,TRX_DATE, CONTRACT_NO, JOB_ORDER_NO", strNO1Where, subcontractorWhere, strDateWhere, strNO2Where, strDate2Where, JOListWhere);

        DataTable dt = MESComment.DBUtility.GetTable(sqlbu.ToString(), dbCon);
        return dt;
    }

    private static DataTable GetIEContractReportData_ppo(string job_order_no, string FactoryCD, string txtBeginDate, string txtEndDate)
    {
        //在以下写入sql语句
        DataTable jobtb = new DataTable();
        jobtb.Columns.Add("jobno");
        string[] jobstr = job_order_no.Split(new char[] { ',' });
        DataRow row = null;
        for (int i = 0; i < jobstr.Length; i++)
        {
            row = jobtb.NewRow();
            row["jobno"] = jobstr[i].ToString().Trim().Replace(" ", "");
            jobtb.Rows.Add(row);
        }

        DbCommand dbcom = MESComment.DBUtility.InsertTempData(jobtb, "INV");
        //start modification by LimML on 26/08/2015: add h.prod_line_cd in ('K-OUTSOURCE','W-OUTSOURCE')
        string sql = "";
        sql += " SELECT DISTINCT  h.inv_fty_cd, to_char(h.trans_date,'yyyy-mm-dd') AS Trans_date, l.job_order_no, ";
        sql += " SUM (NVL (l.trans_qty, 0)) AS issue_qty,          ";
        sql += " SUM (NVL (l.rtw_qty, 0)) AS return_qty, ";
        sql += " max(a.style_chn_desc) as style_chn_desc,";
        sql += " max(replace(replace(h.remarks,chr(13)||chr(10),''),chr(13),'')) as remarks,'n' as status     ";
        sql += " FROM inv_issue_hd h, inv_issue_line l, inv_item i, sc_hd a, po_hd b    ";
        sql += " WHERE h.issue_hd_id = l.issue_hd_id      AND l.inv_item_id = i.inv_item_id      ";
        sql += " AND h.inv_fty_cd = '" + FactoryCD + "'      AND h.status = 'F'      AND h.item_type_cd = 'F'      ";
        sql += " AND l.job_order_no = b.PO_NO      AND a.sc_no = b.sc_no      AND i.product_category IN ('K', 'W')      ";
        sql += " AND h.trans_cd NOT IN ('ITSK', 'ITSW') AND h.prod_line_cd in ('K-OUTSOURCE','W-OUTSOURCE') AND l.job_order_no IN (select f1 From inventory.inv_tmp1)  ";
        if (!txtBeginDate.Equals("") && !txtEndDate.Equals(""))
        {
            sql += " AND to_char(h.trans_date,'yyyy-mm-dd') >='" + txtBeginDate + "' AND to_char(h.trans_date,'yyyy-mm-dd') <='" + txtEndDate + "'";
        }
        sql += " OR h.trans_date IS NULL";
        sql += " GROUP BY h.inv_fty_cd,          l.job_order_no,          h.trans_date     ";
        sql += " ORDER BY h.inv_fty_cd,l.job_order_no, to_char(h.trans_date,'yyyy-mm-dd')";
        DataTable dt = MESComment.DBUtility.GetTable(sql, "INV", dbcom);
        //DataTable dt = MESComment.DBUtility.GetTable(sql, "INV");
        return dt;
    }
}