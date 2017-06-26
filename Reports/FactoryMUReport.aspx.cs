using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;
using System.IO;

public partial class Reports_GEGMUReport : pPage
{   //lml - mu add production line
    string[] strHeader = { 
                             "Start Cutting_Date","Job_Order_No", "YPD Job No", "Ship_Date", "Customer", "GarmentOrder", "PPO Number", "Fabric Width", "Long/short_sleeve", "Wash type", "Fabric Pattern", "Marker Utilization (%)","Fabric (YDS)","Garment (DZ)","YPD","RATIO","Buyer's max over shipment allowance","Buyer's max over cut allowance</th></tr><tr>",
                             "PPO order yds","Allocated","Issued","Marker Audited","Ship Yardage","Cutting Wastage","Leftover fabric","LeftOver Fab Reason Code","LeftOver Fab Reason","Remnant","SRN","RTW","Order(DZ)","Cut","Shipped","Sample","Pull-out","Leftover Garment","Sewing Wastage","Washing Wastage","Unacc Gmt","PPO MKR YPD","BULK NET YPD","GIVEN CUT YPD","BULK MKR YPD","YPD Var","CUT YPD","SHIP YPD","Ship-to-Cut","Ship-to-Receipt","Ship-to-Order","Cut-to-Receipt","Cut-to-Order</th></tr><tr>",
                             "Yds","%","Defect Fab","Defect Panels","Odd Loss","Splice","Cutting Rej Panels","Match Lost","EndLost","Short Fab","Sewing match loss","Grade A","Grade B","Grade C","Total","DZ","%","DZ","%"
                         };
    string userid = "";
    DataTable getalldatae = null;
    private string reporttype = "fmr";
    object[] Total;
    public string strRunNO;
    public string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

    private void beginProgress()
    {
        //根据ProgressBar.htm显示进度条界面

        string templateFileName = Path.Combine(Server.MapPath("."), "ProgressBar.htm");
        StreamReader reader = new StreamReader(@templateFileName, System.Text.Encoding.GetEncoding("GB2312"));
        string html = reader.ReadToEnd();
        reader.Close();
        Response.Write(html);
        Response.Flush();
    }


    private void SetTiile(string Strt)
    {
        string jsBlock = "<script>SetTiile('" + Strt + "'); </script>";
        Response.Write(jsBlock);
        Response.Flush();
    }
    private void setProgress(int percent, string msg1, string msg2)
    {
        string jsBlock = "<script>SetPorgressBar('" + percent.ToString() + "','" + msg1 + "','" + msg2 + "'); </script>";
        Response.Write(jsBlock);
        Response.Flush();
    }

    private void finishProgress()
    {
        string jsBlock = "<script>SetCompleted();</script>";
        Response.Write(jsBlock);
        Response.Flush();
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["userid"] != null)
        {
            userid = Request.QueryString["userid"].ToString();
        }
        if (!IsPostBack)
        {
            ddlFtyCd.DataSource = MESComment.MesRpt.GetFactoryList(userid);
            ddlFtyCd.DataTextField = "DEPARTMENT_ID";
            ddlFtyCd.DataValueField = "DEPARTMENT_ID";
            ddlFtyCd.DataBind();
            ddlReport.Items.Clear();
            ddlReport.Items.Add(new ListItem("MU Standard Report", "1"));
            ddlReport.Items.Add(new ListItem("Factory MU Fabric Wastage Report", "2"));
            ddlReport.Items.Add(new ListItem("Factory MU Garment Wastage Report", "3"));

            if (Request.QueryString["site"] != null)
            {
                if (Request.QueryString["site"].ToUpper().Equals("DEV"))
                {
                    ddlFtyCd.SelectedValue = "GEG";
                }
                else
                {
                    ddlFtyCd.SelectedValue = Request.QueryString["site"].ToString();
                }
            }
        }


        switch (ddlReport.SelectedValue)
        {
            case "1":
                this.ReportTitle.Text = "Factory MU Report";
                this.DateFromLabel.Text = "Cut From Date : ";
                this.DateToLable.Text = "Cut To Date : ";
                this.gvDetail.Visible = true; this.gvFabricDetail.Visible = false; this.gvGarmentDetail.Visible = false;
               // this.cbProductionLine.Visible = false;
                break;
            case "2":
                this.ReportTitle.Text = "Factory MU Fabric Wastage Report";
                this.DateFromLabel.Text = "Cut Completion Date From : ";
                this.DateToLable.Text = "Cut Completion Date To : ";
                this.gvDetail.Visible = false; this.gvFabricDetail.Visible = true; this.gvGarmentDetail.Visible = false;
                //this.cbProductionLine.Visible = false;
                break;
            case "3":
                this.ReportTitle.Text = "Factory MU Garment Wastage Report";
                this.DateFromLabel.Text = "Ship From Date : ";
                this.DateToLable.Text = "Ship To Date : ";
                this.gvDetail.Visible = false; this.gvFabricDetail.Visible = false; this.gvGarmentDetail.Visible = true;
                this.cbProductionLine.Visible = true;
                break;
        }
    }

    private DataTable GetMasterTable(string JoList, string ReportType)
    {
        int StepTotal = 0;
        switch (ReportType)
        {//记得连小步骤都要计算在内.
            case "1":
                StepTotal = 15;
                break;
            case "2":
                StepTotal = 12;
                break;
            case "3":
                StepTotal = 8;
                break;

        }
        int Step = 1;
        Random ro = new Random(1000);
        strRunNO = Convert.ToString(string.Format("{0:yy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(10).ToString();
        string Strmsg = "|--Process MU Data begin(Total " + StepTotal + " Step)";
        
        beginProgress();
        SetTiile("Factory MU Report");
        Strmsg += "<br/>|--1. Process JO list begin...";
        setProgress(Step * 100 / StepTotal, Strmsg, "Process JO list ...(" + Step + "/" + StepTotal + ")");
        int Row1 = 1;
        int Row2 = 1;
        DbConnection invConn = null;
        DataTable StandardJOList = MESComment.MUReportSql.GetJoList(JoList, ddlFtyCd.SelectedItem.Value, ddlGarmentType.SelectedItem.Value, ddlOutSource.SelectedItem.Value, txtFromDate.Text, txtToDate.Text, cbShipJo.Checked, cbNoPost.Checked, strRunNO);
        if (StandardJOList.Rows.Count <= 0)
        {
            finishProgress();
            return StandardJOList;
        }

        System.Text.StringBuilder JoListall = new System.Text.StringBuilder(); ////所有单，CB单取Origal_JO_NO
        System.Text.StringBuilder cbJolist = new System.Text.StringBuilder(); //所有cb的单，只包含CB单
        System.Text.StringBuilder JoCBList = new System.Text.StringBuilder(); //所有单,包括NO CB 和CB单
        Dictionary<string, List<string>> CombineJo = new Dictionary<string, List<string>>();//只取CB单，用于合并单
        DataTable jobtb = new DataTable();    //所有单，CB拆分源始单，用户查询
        DataTable cbjobtb = new DataTable();  //包所有的Origal_JO_NO和CB JO
        jobtb.Columns.Add("jobno");
        cbjobtb.Columns.Add("jobno");
        DataRow row_ = null;
        for (int i = 0; i < StandardJOList.Rows.Count; i++)
        {
            if (StandardJOList.Rows[i]["JO_NO"].ToString() != "")
            {
                //JoList += "'" + StandardJOList.Rows[i]["ORIGINAL_JO_NO"].ToString() + "',";
                JoListall.AppendFormat("'{0}',", StandardJOList.Rows[i]["ORIGINAL_JO_NO"]);
                row_ = jobtb.NewRow();
                row_["jobno"] = StandardJOList.Rows[i]["ORIGINAL_JO_NO"].ToString();
                jobtb.Rows.Add(row_);
                row_ = cbjobtb.NewRow();
                row_["jobno"] = StandardJOList.Rows[i]["ORIGINAL_JO_NO"].ToString();
                cbjobtb.Rows.Add(row_);

                if (StandardJOList.Rows[i]["ORIGINALJO"] == "")
                {
                    JoCBList.AppendFormat("'{0}',", StandardJOList.Rows[i]["JO_NO"]);
                    row_ = cbjobtb.NewRow();
                    row_["jobno"] = StandardJOList.Rows[i]["JO_NO"].ToString();
                    cbjobtb.Rows.Add(row_);

                }
                else //CB
                {
                    if (!CombineJo.ContainsKey(StandardJOList.Rows[i]["JO_NO"].ToString()))
                    {
                        JoCBList.AppendFormat("'{0}',", StandardJOList.Rows[i]["JO_NO"]);
                        cbJolist.AppendFormat("'{0}',", StandardJOList.Rows[i]["JO_NO"]);
                        CombineJo.Add(StandardJOList.Rows[i]["JO_NO"].ToString(), new List<string>() { StandardJOList.Rows[i]["ORIGINAL_JO_NO"].ToString() });
                        row_ = cbjobtb.NewRow();
                        row_["jobno"] = StandardJOList.Rows[i]["JO_NO"].ToString();
                        cbjobtb.Rows.Add(row_);
                    }
                    else
                    {
                        CombineJo[StandardJOList.Rows[i]["JO_NO"].ToString()].Add(StandardJOList.Rows[i]["ORIGINAL_JO_NO"].ToString());
                    }
                }
            }
        }
        if (JoListall.Length > 0)
        {
            JoListall = JoListall.Remove(JoListall.Length - 1, 1);
        }
        if (cbJolist.Length > 0)
        {
            cbJolist = cbJolist.Remove(cbJolist.Length - 1, 1);
        }
        if (JoCBList.Length > 0)
        {
            JoCBList = JoCBList.Remove(JoCBList.Length - 1, 1);
        }

        //JoList = "";

        //DataTable jobtb = new DataTable();
        //jobtb.Columns.Add("jobno");
        //DataRow row_ = null;
        //for (int i = 0; i < StandardJOList.Rows.Count; i++)
        //{
        //    if (StandardJOList.Rows[i]["JO_NO"].ToString() != "")
        //    {
        //        JoList += "'" + StandardJOList.Rows[i]["JO_NO"].ToString() + "',";
        //        row_ = jobtb.NewRow();
        //        row_["jobno"] = StandardJOList.Rows[i]["JO_NO"].ToString();
        //        jobtb.Rows.Add(row_);
        //    }
        //}

        //取GO信息,因为SC_LOT表没有CB单，所以只能取源单
        DataTable StandardGOList = MESComment.MUReportSql.GetGoListforStandardMu(JoListall.ToString(), ddlFtyCd.SelectedItem.Value, txtFromDate.Text, txtToDate.Text, ddlGarmentType.SelectedItem.Value);
        StandardGOList = GetCombineSCTable(CombineJo, StandardGOList);

        
        DataTable StandardGisInfo = null;
        DataTable StandardWidthNpatternList = null;
        DataTable LeftOver = null;
        DbConnection eelConn = null;
        DbConnection MESConn = null;
        DbConnection GisConn = null;
        DataTable StandardSRNAndRTW = null;
        DataTable LeftoverPPOList = null;
        DataTable StandardOrderQty = null;
        Strmsg += "<br/>|--Process JO list finish.";
        if (!ReportType.Equals("2"))
        {
            Row1++;
            Row2 = 0;
            Row2++;
            Strmsg += "<br/>|--" + Row1 + ". Process Invoice Data Begin:<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Put Jo List To GIS system begin.";
            Step++;
            setProgress(Step * 100 / StepTotal, Strmsg, "Put Jo List To GIS system ...(" + Step + "/" + StepTotal + ")");
            GisConn = MESComment.DBUtility.GetConnection("GIS");//OK
            MESComment.DBUtility.InsertGISRptTempData(jobtb, strRunNO, GisConn);        //OK
            MESComment.DBUtility.CloseConnection(ref GisConn);//OK
            Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Put Jo List To GIS system end.";
            Row2++;
            Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Get Invoice Data Begin..";
            Step++;
            setProgress(Step * 100 / StepTotal, Strmsg, "Get Invoice Data ...(" + Step + "/" + StepTotal + ")");
            //Added By ZouShiChang On 2014.02.14 Start
            //StandardGisInfo = MESComment.MUReportSql.GetLocalGisInfo(JoList, ddlFtyCd.SelectedItem.Value, ddlGarmentType.SelectedItem.Value, ddlOutSource.SelectedItem.Value, txtFromDate.Text, txtToDate.Text, cbShipJo.Checked, cbNoPost.Checked, strRunNO);
            //JoList = JoList.Substring(0, JoList.Length - 1);//不要逗号；
            //要合并CB
            StandardGisInfo = MESComment.MUReportSql.GetStandardGisInfo(JoListall.ToString(), ddlFtyCd.SelectedItem.Value, ddlGarmentType.SelectedItem.Value, ddlOutSource.SelectedItem.Value, txtFromDate.Text, txtToDate.Text, cbShipJo.Checked, cbNoPost.Checked, strRunNO);
            StandardGisInfo = GetCombineGisTable(CombineJo, StandardGisInfo);

            //Added By ZouShiChang On 2014.02.14 End            
            Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Get Invoice Data End.<br/>|--&nbsp;&nbsp;&nbsp;&nbsp;Process Invoice Data End.";
        }
        if (!ReportType.Equals("3"))
        {
            Row1++;
            Row2 = 0;
            Strmsg += "<br/>|--" + Row1 + ". Get GO/PPO Mapping data Begin:";
            Step++;
            setProgress(Step * 100 / StepTotal, Strmsg, "Get GO/PPO Mapping data ...(" + Step + "/" + StepTotal + ")");
            //JoList = JoList.Substring(0, JoList.Length - 1);//不要逗号；

            ///conn = Get Connection("eel")
            ///insert temp table
            ///search data
            /////////////////////////// EEL ////////////////////////////////////////
            eelConn = MESComment.DBUtility.GetConnection("EEL");
            MESComment.DBUtility.InsertRptTempData(jobtb, eelConn);
            StandardWidthNpatternList = MESComment.MUReportSql.GetStandardWidthNpatternList(eelConn);
            Strmsg += "<br/>|--&nbsp;&nbsp;&nbsp;&nbsp;Get GO/PPO Mapping data End";
        }
        Row1++;
        Row2 = 0;
        if (ReportType.Equals("2"))
        {
            Strmsg += "|<br/>--" + Row1 + ". Get Fabric Issue/Leftover Garment Data Begin...";
        }
        else if (ReportType.Equals("3"))
        {
            Strmsg += "|<br/>--" + Row1 + ". Push Left Garment data Begin...";
        }
        Row2++;
        Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Push Jo List to Fab Inv System Begin:";
        Step++;
        setProgress(Step * 100 / StepTotal, Strmsg, "Push Jo List to Fab Inv System ...(" + Step + "/" + StepTotal + ")");

        /////////////////////////// INV ////////////////////////////////////////

        invConn = MESComment.DBUtility.GetConnection("INV");
        MESComment.DBUtility.InsertRptTempData(cbjobtb, invConn);
        Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Push Jo List to Fab Inv System End";

        //Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--4.2 Get Fabric Issue/Leftover Garment Data Begin:";
        //setProgress(30, Strmsg, "Get Fabric Issue/Leftover Garment Data ...(6/15)");

        if (ReportType.Equals("2"))
        {
            Row2++;
            Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Get Fabric Issue Data Begin:";
            Step++;
            setProgress(Step * 100 / StepTotal, Strmsg, "Get Fabric Issue Data ...(" + Step + "/" + StepTotal + ")");
            //要合并CB
            StandardSRNAndRTW = MESComment.MUReportSql.GetStandardLeftSRNAndRTW(ddlFtyCd.SelectedItem.Value, invConn);
           
            Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Get Fabric Issue Data End";
        }
        else if (ReportType.Equals("3"))
        {
            Row2++;
            Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Get Fabric Issue Data Begin:";
            Step++;
            setProgress(Step * 100 / StepTotal, Strmsg, "Get Fabric Issue Data ...(" + Step + "/" + StepTotal + ")");
            //invConn = MESComment.DBUtility.GetConnection("INV");
            //要合并CB
            StandardSRNAndRTW = MESComment.MUReportSql.GetStandardLeftGM(ddlFtyCd.SelectedItem.Value, invConn);
            
        }
        else
        {
            Row2++;
            Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Get Fabric Issue/Leftover Garment Data Begin:";
            Step++;
            setProgress(Step * 100 / StepTotal, Strmsg, "Get Fabric Issue/Leftover Garment Data ...(" + Step + "/" + StepTotal + ")");
            //要合并CB
            StandardSRNAndRTW = MESComment.MUReportSql.GetStandardLeftGMAndSRNAndRTW(ddlFtyCd.SelectedItem.Value, invConn);
           
        }
        StandardSRNAndRTW = GetCombineTable(CombineJo, StandardSRNAndRTW);
        if (!ReportType.Equals("3"))
        {
            Row2++;
            Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Push PPO list to Fab Inv system Begin:";
            Step++;
            setProgress(Step * 100 / StepTotal, Strmsg, "Push PPO list to Fab Inv system...(" + Step + "/" + StepTotal + ")");
            MESComment.DBUtility.InsertRptPPOTempData(StandardWidthNpatternList, invConn);//OK
            Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Push PPO list to Fab Inv system End";
            Row2++;
            Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Get Fabric LeftOver Data Begin:";
            Step++;
            setProgress(Step * 100 / StepTotal, Strmsg, "Get Fabric LeftOver Data...(" + Step + "/" + StepTotal + ")");
            LeftOver = MESComment.MUReportSql.GetLocalLeftOver(ddlFtyCd.SelectedItem.Value, invConn);//lml
            Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Get Fabric LeftOver Data End";
        }
        if (ReportType.Equals("2"))
        {
            Strmsg += "<br/>|--&nbsp;&nbsp;&nbsp;&nbsp;Get Fabric Issue/Leftover Garment Data End";
        }
        else if (ReportType.Equals("3"))
        {
            Strmsg += "<br/>|--&nbsp;&nbsp;&nbsp;&nbsp;Push Left Garment data  End";
        }
        else
        {
            Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Get Fabric Issue/Leftover Garment Data End";
        }
        MESConn = MESComment.DBUtility.GetConnection("MES_UPDATE");//OK
        MESComment.DBUtility.InsertMESMUData(StandardGOList, strRunNO, reporttype, MESConn);
        if (!ReportType.Equals("3"))
        {
            Row1++;
            Row2 = 0;
            Step++;
            setProgress(Step * 100 / StepTotal, Strmsg, "Push Data to MES...(" + Step + "/" + StepTotal + ")");
            MESComment.DBUtility.CloseConnection(ref invConn);
            //Step++;
            setProgress(Step * 100 / StepTotal, Strmsg, "Push Data to MES Begin ...(" + Step + "/" + StepTotal + ")");
            /////////////////////////// INV ////////////////////////////////////////
            Strmsg += "</br>|--" + Row1 + ". Push Data to MES Begin:";
            //Step++;
            setProgress(Step * 100 / StepTotal, Strmsg, "Push Data to MES...(" + Step + "/" + StepTotal + ")");
        }
        if (!ReportType.Equals("2"))
        {
        
         
            Row2++;
            Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Push Ship data(GIS) Begin...";
            Step++;
            setProgress(Step * 100 / StepTotal, Strmsg, "Push Ship data(GIS)...(" + Step + "/" + StepTotal + ")");
            MESComment.DBUtility.InsertMESRptShipData(StandardGisInfo, strRunNO, ddlFtyCd.SelectedItem.Value, MESConn);//OK        
            Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Push Ship data(GIS) End";
        }
        if (!ReportType.Equals("3"))
        {
            Row2++;
            //MESConn = MESComment.DBUtility.GetConnection("MES_UPDATE");//OK
            Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Push PPO/GO data Begin...";
            Step++;
            setProgress(Step * 100 / StepTotal, Strmsg, "Push PPO/GO data ...(" + Step + "/" + StepTotal + ")");
            MESComment.DBUtility.InsertMESRptWidthNpatternData(StandardWidthNpatternList, strRunNO, ddlFtyCd.SelectedItem.Value, MESConn);//OK
            Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Push PPO/GO data End";
        }
        Row2++;
        //MESConn = MESComment.DBUtility.GetConnection("MES_UPDATE");//OK
        Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Push Fabric & Left Garment data Begin...";
        Step++;
        setProgress(Step * 100 / StepTotal, Strmsg, "Push Fabric & Left Garment data ...(" + Step + "/" + StepTotal + ")");
        MESComment.DBUtility.InsertMESRptFGISData(StandardSRNAndRTW, strRunNO, ddlFtyCd.SelectedItem.Value, MESConn);//LML
        Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Push Fabric & Left Garment data  End";
        if (!ReportType.Equals("3"))
        {
            Row2++;
            Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Push Fabric LeftOver data Begin...";
            Step++;
            setProgress(Step * 100 / StepTotal, Strmsg, " Push Fabric LeftOver data ...(" + Step + "/" + StepTotal + ")");
            MESComment.DBUtility.InsertMESRptLeftOverData(LeftOver, strRunNO, ddlFtyCd.SelectedItem.Value, MESConn);

            /////////////////////////// EEL ////////////////////////////////////////
            //modify by ming 因为在ppo_dt在local escm 没有，改为hl
            MESComment.DBUtility.CloseConnection(ref eelConn);
            eelConn = MESComment.DBUtility.GetConnection("HL");
            //modify by ming 
            LeftoverPPOList = MESComment.MUReportSql.GetSCList(strRunNO);
            MESComment.DBUtility.InsertRptTempData(LeftoverPPOList, eelConn);

            Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Push Fabric LeftOver data  End";
            Row1++;
            Row2 = 0;
            Strmsg += "<br/>|--" + Row1 + ". Get GO PPO order QTY Begin:";
            Step++;
            setProgress(Step * 100 / StepTotal, Strmsg, "Get GO PPO order QTY ...(" + Step + "/" + StepTotal + ")");
            StandardOrderQty = MESComment.MUReportSql.GetLocalOrderQty(eelConn);//ok
            MESComment.DBUtility.CloseConnection(ref eelConn);
            MESComment.DBUtility.InsertMESRptStandardOrderQtyData(StandardOrderQty, strRunNO, ddlFtyCd.SelectedItem.Value,MESConn);//OK
            Strmsg += "<br/>|--&nbsp;&nbsp;&nbsp;&nbsp;Get GO PPO order QTY End.";
        }
        Strmsg += "<br/>|--&nbsp;&nbsp;&nbsp;&nbsp;Push Data to MES End";
        Row1++;
        Row2 = 0;
        Strmsg += "<br/>|--" + Row1 + ". Process all data in mes begin...";
        Step++;
        setProgress(Step * 100 / StepTotal, Strmsg, "Process all data in mes  ...(" + Step + "/" + StepTotal + ")");
        MESComment.DBUtility.CloseConnection(ref MESConn);

        DataTable StandardMUReportList = null;
        switch (ReportType)
        {
            case "1":
                StandardMUReportList = MESComment.MUReportSql.GetLocalMUReportList(ddlFtyCd.SelectedItem.Value, strRunNO, "New");
                break;
            case "2":
                StandardMUReportList = MESComment.MUReportSql.GetLocalMUReportFabricList(ddlFtyCd.SelectedItem.Value, strRunNO, "New");
                break;
            case "3":
                StandardMUReportList = MESComment.MUReportSql.GetLocalMUReportGarmentList(ddlFtyCd.SelectedItem.Value, strRunNO, txtFromDate.Text, txtToDate.Text, "New",cbProductionLine.Checked);
                break;
        }
        return StandardMUReportList;
    }
    /// <summary>
    /// 合并GO信息，buyer_po_del_date取最早日期
    /// </summary>
    /// <param name="CombineJo"></param>
    /// <param name="Originaldt"></param>
    /// <returns></returns>
    public DataTable GetCombineSCTable(Dictionary<string, List<string>> CombineJo, DataTable Originaldt)
    {

        DataTable dt = Originaldt.Copy();
        dt.Columns["JOB_ORDER_NO"].ReadOnly = false;
        dt.Columns["buyer_po_del_date"].ReadOnly = false;
        dt.Columns["total_qty"].ReadOnly = false;
        dt.Columns["PERCENT_OVER_ALLOWED"].ReadOnly = false;
        if (CombineJo.Count == 0)
            return dt;
        bool iscombine = false;
        DataRow _dr = null;
        double overqty = 0;
        double total = 0;
        foreach (string key in CombineJo.Keys)
        {
            iscombine = false;
            overqty = 0;
            total = 0;
            _dr = null;
            foreach (string job in CombineJo[key])
            {
                DataRow[] selectrows = dt.Select("JOB_ORDER_NO='" + job + "'");

                if (selectrows.Count() == 0)
                {
                    continue;
                }
                else
                {
                    total += selectrows[0]["total_qty"].ToDouble();
                    //if (selectrows[0]["PERCENT_OVER_ALLOWED"].ToDouble() > 0)
                    //{
                    //    overqty += selectrows[0]["total_qty"].ToDouble() * (selectrows[0]["PERCENT_OVER_ALLOWED"].ToDouble() - 1) / 100;
                    //}
                    //else
                    //{
                    //    overqty += 0;
                    //}
                    overqty += selectrows[0]["total_qty"].ToDouble() * (selectrows[0]["PERCENT_OVER_ALLOWED"].ToDouble()-100) / 100;
                    if (!iscombine)
                    {
                        _dr = selectrows[0];
                        _dr["JOB_ORDER_NO"] = key;
                        iscombine = true;
                    }
                    else
                    {
                        if (_dr["buyer_po_del_date"].ToDate() > selectrows[0]["buyer_po_del_date"].ToDate())
                        {
                            _dr["buyer_po_del_date"] = selectrows[0]["buyer_po_del_date"];
                        }

                        _dr["total_qty"] = _dr["total_qty"].ToDouble() + selectrows[0]["total_qty"].ToDouble();

                        dt.Rows.Remove(selectrows[0]);
                    }

                }


            }
            if (_dr != null)
                _dr["PERCENT_OVER_ALLOWED"] = (overqty / total+1)*100;
        }
        return dt;
    }
    /// <summary>
    /// 合并Gis信息，ShipDate取最大日期
    /// </summary>
    /// <param name="CombineJo"></param>
    /// <param name="Originaldt"></param>
    /// <returns></returns>
    public DataTable GetCombineGisTable(Dictionary<string, List<string>> CombineJo, DataTable Originaldt)
    {

        DataTable dt = Originaldt.Copy();
        dt.Columns["JO_NO"].ReadOnly = false;
        dt.Columns["SHIP_DATE"].ReadOnly = false;
        dt.Columns["SHIP_QTY"].ReadOnly = false;
     
        if (CombineJo.Count == 0)
            return dt;
        bool iscombine = false;
        DataRow _dr = null;
   
        foreach (string key in CombineJo.Keys)
        {
            iscombine = false;
         
            _dr = null;
            foreach (string job in CombineJo[key])
            {
                DataRow[] selectrows = dt.Select("JO_NO='" + job + "'");

                if (selectrows.Count() == 0)
                {
                    continue;
                }
                else
                {
                    
                    if (!iscombine)
                    {
                        _dr = selectrows[0];
                        _dr["JO_NO"] = key;
                        iscombine = true;
                    }
                    else
                    {
                        if (_dr["SHIP_DATE"].ToDate() < selectrows[0]["SHIP_DATE"].ToDate())
                        {
                            _dr["SHIP_DATE"] = selectrows[0]["SHIP_DATE"];
                        }

                        _dr["SHIP_QTY"] = _dr["SHIP_QTY"].ToDouble() + selectrows[0]["SHIP_QTY"].ToDouble();

                        dt.Rows.Remove(selectrows[0]);
                    }

                }


            }
           
        }
        return dt;
    }
    /// <summary>
    /// add by ming 把源始JO合并用CB
    /// </summary>
    /// <param name="JoList"></param>
    /// <returns></returns>
    public DataTable GetCombineTable(Dictionary<string, List<string>> CombineJo, DataTable Originaldt)
    {
        if (Originaldt == null)
            return Originaldt;
        DataTable dt = Originaldt.Copy();
        if (CombineJo.Count == 0)
            return dt;
        bool iscombine = false;
        DataRow _dr = null;
        foreach (string key in CombineJo.Keys)
        {
            iscombine = false;
            DataRow[] cbrows = dt.Select("jo_no='" + key + "'");//是否已存在CB单
            foreach (string job in CombineJo[key])
            {
                DataRow[] selectrows = dt.Select("jo_no='" + job + "'");
                if (selectrows.Count() == 0)
                {
                    continue;
                }
                else
                {
                    if (!iscombine)
                    {
                        if (cbrows.Length == 0)
                        {
                            _dr = selectrows[0];
                            _dr[0] = key;
                        }
                        else
                        {
                            _dr = cbrows[0];
                            for (int i = 1; i < Originaldt.Columns.Count; i++)
                            {
                                _dr[i] = _dr[i].ToDouble() + selectrows[0][i].ToDouble();

                            }
                            dt.Rows.Remove(selectrows[0]);
                        }
                        iscombine = true;
                    }
                    else
                    {
                        for (int i = 1; i < Originaldt.Columns.Count; i++)
                        {
                            _dr[i] = _dr[i].ToDouble() + selectrows[0][i].ToDouble();

                        }
                        dt.Rows.Remove(selectrows[0]);
                    }

                }


            }
        }
        return dt;
    }
    protected void gvDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();
                //First Row
                for (int i = 0; i < 12; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("rowspan", "3");
                    tcHeader[i].Text = strHeader[i];
                }

                tcHeader.Add(new TableHeaderCell());
                tcHeader[12].Attributes.Add("colspan", "22");
                tcHeader[12].Text = strHeader[12];

                tcHeader.Add(new TableHeaderCell());
                tcHeader[13].Attributes.Add("colspan", "14");
                tcHeader[13].Text = strHeader[13];

                tcHeader.Add(new TableHeaderCell());
                tcHeader[14].Attributes.Add("colspan", "7");
                tcHeader[14].Text = strHeader[14];

                tcHeader.Add(new TableHeaderCell());
                tcHeader[15].Attributes.Add("colspan", "5");
                tcHeader[15].Text = strHeader[15];

                tcHeader.Add(new TableHeaderCell());
                tcHeader[16].Attributes.Add("rowspan", "3");
                tcHeader[16].Text = strHeader[16];

                tcHeader.Add(new TableHeaderCell());
                tcHeader[17].Attributes.Add("rowspan", "3");
                tcHeader[17].Text = strHeader[17];
                //Second Row
                for (int i = 18; i < 23; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("rowspan", "2");
                    tcHeader[i].Text = strHeader[i];
                }

                tcHeader.Add(new TableHeaderCell());
                tcHeader[23].Attributes.Add("colspan", "11");
                tcHeader[23].Text = strHeader[23];

                for (int i = 24; i < 35; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("rowspan", "2");
                    tcHeader[i].Text = strHeader[i];
                }

                tcHeader.Add(new TableHeaderCell());
                tcHeader[35].Attributes.Add("colspan", "4");
                tcHeader[35].Text = strHeader[35];

                for (int i = 36; i < 38; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("colspan", "2");
                    tcHeader[i].Text = strHeader[i];
                }

                for (int i = 38; i < 51; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("rowspan", "2");
                    tcHeader[i].Text = strHeader[i];
                }
                //Third Row
                for (int i = 51; i < 70; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Text = strHeader[i];
                }

                //bgcolor
                for (int i = 0; i < 70; i++)
                {
                    tcHeader[i].Attributes.Add("bgcolor", "#F7F6F3");
                }
                break;
            case DataControlRowType.DataRow:
                e.Row.Cells[5].Style.Add("word-break", "break-all");
                break;

            case DataControlRowType.Footer:
                DataTable Summarydata = MESComment.MUReportSql.GetLocalMUReportSummary(ddlFtyCd.SelectedItem.Value, strRunNO);
                if (Summarydata.Rows.Count == 1)
                {
                    e.Row.Cells[0].Text = "Total/Weighted Average";
                    e.Row.Cells[11].Text = Summarydata.Rows[0]["MU"].ToString();
                    e.Row.Cells[12].Text = Summarydata.Rows[0]["PPO_ORDER_YDS"].ToString();
                    e.Row.Cells[13].Text = Summarydata.Rows[0]["ALLOCATED"].ToString();
                    e.Row.Cells[14].Text = Summarydata.Rows[0]["Issued"].ToString();
                    e.Row.Cells[15].Text = Summarydata.Rows[0]["MARKER_AUDITED"].ToString();
                    e.Row.Cells[16].Text = Summarydata.Rows[0]["ShipYardage"].ToString();
                    e.Row.Cells[17].Text = Summarydata.Rows[0]["CUT_WSTG_YDS"].ToString();
                    e.Row.Cells[18].Text = Summarydata.Rows[0]["CutWastageYPDPer"].ToString();
                    e.Row.Cells[19].Text = Summarydata.Rows[0]["DEFECT_FAB"].ToString();
                    e.Row.Cells[20].Text = Summarydata.Rows[0]["DEFECT_PANELS"].ToString();
                    e.Row.Cells[21].Text = Summarydata.Rows[0]["ODD_LOSS"].ToString();
                    e.Row.Cells[22].Text = Summarydata.Rows[0]["SPLICE_LOSS"].ToString();      
                    e.Row.Cells[23].Text = Summarydata.Rows[0]["CUT_REJ_PANELS"].ToString();
                    e.Row.Cells[24].Text = Summarydata.Rows[0]["MATCH_LOSS"].ToString();
                    e.Row.Cells[25].Text = Summarydata.Rows[0]["END_LOSS"].ToString();
                    e.Row.Cells[26].Text = Summarydata.Rows[0]["SHORT_YDS"].ToString();
                    e.Row.Cells[27].Text = Summarydata.Rows[0]["SEW_MATCH_LOSS"].ToString();
                    e.Row.Cells[28].Text = Summarydata.Rows[0]["LEFTOVER_FAB"].ToString();
                    e.Row.Cells[31].Text = Summarydata.Rows[0]["REMNANT"].ToString();
                    e.Row.Cells[32].Text = Summarydata.Rows[0]["SRNQty"].ToString();
                    e.Row.Cells[33].Text = Summarydata.Rows[0]["RTW_QTY"].ToString();
                    e.Row.Cells[34].Text = Summarydata.Rows[0]["ORDERQTY"].ToString();
                    e.Row.Cells[35].Text = Summarydata.Rows[0]["CUTQTY"].ToString();
                    e.Row.Cells[36].Text = Summarydata.Rows[0]["SHIP_QTY"].ToString();
                    e.Row.Cells[37].Text = Summarydata.Rows[0]["SAMPLE_QTY"].ToString();
                    e.Row.Cells[38].Text = Summarydata.Rows[0]["PULLOUT_QTY"].ToString();
                    e.Row.Cells[39].Text = Summarydata.Rows[0]["GMT_QTY_A"].ToString();
                    e.Row.Cells[40].Text = Summarydata.Rows[0]["GMT_QTY_B"].ToString();
                    e.Row.Cells[41].Text = Summarydata.Rows[0]["DISCREPANCY_QTY"].ToString();
                    e.Row.Cells[42].Text = Summarydata.Rows[0]["GMT_QTY_TOTAL"].ToString();
                    e.Row.Cells[43].Text = Summarydata.Rows[0]["SewingDz"].ToString();
                    e.Row.Cells[44].Text = Summarydata.Rows[0]["SewingPercent"].ToString();
                    e.Row.Cells[45].Text = Summarydata.Rows[0]["WashingDz"].ToString();
                    e.Row.Cells[46].Text = Summarydata.Rows[0]["WashingPercent"].ToString();
                    e.Row.Cells[47].Text = Summarydata.Rows[0]["UnaccGmt"].ToString();
                    e.Row.Cells[48].Text = Summarydata.Rows[0]["PPO_YPD"].ToString();
                    e.Row.Cells[49].Text = Summarydata.Rows[0]["BULK_NET_YPD"].ToString();
                    e.Row.Cells[50].Text = Summarydata.Rows[0]["GIVEN_CUT_YPD"].ToString();
                    e.Row.Cells[51].Text = Summarydata.Rows[0]["BULK_MKR_YPD"].ToString();
                    e.Row.Cells[52].Text = Summarydata.Rows[0]["YPD_Var"].ToString();
                    e.Row.Cells[53].Text = Summarydata.Rows[0]["cutYPD"].ToString();
                    e.Row.Cells[54].Text = Summarydata.Rows[0]["ShipYPD"].ToString();
                    e.Row.Cells[55].Text = Summarydata.Rows[0]["ShipToCut"].ToString();
                    e.Row.Cells[56].Text = Summarydata.Rows[0]["ShipToRecv"].ToString();
                    e.Row.Cells[57].Text = Summarydata.Rows[0]["ShipToOrder"].ToString();
                    e.Row.Cells[58].Text = Summarydata.Rows[0]["cut_to_receipt"].ToString();
                    e.Row.Cells[59].Text = Summarydata.Rows[0]["cut_to_order"].ToString();
                    e.Row.Cells[60].Text = Summarydata.Rows[0]["OVERSHIP"].ToString();
                    e.Row.Cells[61].Text = Summarydata.Rows[0]["OVER_CUT"].ToString();
                    e.Row.Font.Bold = true;
                }
                for (int cells = 0; cells < e.Row.Cells.Count; cells++)
                {
                    e.Row.Cells[cells].Attributes.Add("bgcolor", "#F7F6F3");
                }
                    break;
        }
        finishProgress();
        DateTime dateT = DateTime.Now;
        this.PrintDate.Text = dateT.ToString("yyyy-MM-dd HH:mm:ss");
    }

    protected void gvFabricDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (Total != null)
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.Header:
                    //bgcolor
                    for (int i = 0; i < Total.Count(); i++)
                    {
                        e.Row.Cells[i].Text = getalldatae.Columns[i].ColumnName.ToString();                        
                    }
                    break;
                case DataControlRowType.DataRow:
                    e.Row.Cells[5].Style.Add("word-break", "break-all");
                    e.Row.Cells[7].Style.Add("word-break", "break-all");
                        break;
                case DataControlRowType.Footer:
                    for (int i = 0; i < Total.Count(); i++)
                    {
                        e.Row.Cells[i].Text = Total[i].ToString();
                    }
                    break;
            }
        }
        finishProgress();
        DateTime dateT = DateTime.Now;
        this.PrintDate.Text = dateT.ToString("yyyy-MM-dd HH:mm:ss");
    }

    protected void gvGarmentDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                int leftoverC = -1;//LEFTOVER_C_1子字段的起始位置;
                int countC = 0;//LEFTOVER_C子字段数;
                string[] Column = new string[getalldatae.Columns.Count];
                //string[] ColumnC = { "裁片问题车间下数", "车间欠配套下数", "车间疵品半成品下数", "车间大疵", "印花下数" };
                string[] ColumnC = { "Cut panels / Sewing REJ", "Mismatched quantity of CUT Panels / Sewing REJ", "Defective Garment of semifinished product/ Sewing REJ", "Grade C / SEW REJ", "Printing REJ" };
                for (int i = 0; i < getalldatae.Columns.Count; i++)
                {
                    Column[i] = getalldatae.Columns[i].ColumnName.ToString();
                    if (Column[i] == "LEFTOVER_C_1")
                    {
                        leftoverC = i;
                    }
                    if (Column[i].Contains("LEFTOVER_C"))
                    {
                        countC++;
                    }
                }
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();
                //First Row
                for (int i = 0; i < leftoverC; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("rowspan", "2");
                    tcHeader[i].Text = Column[i];
                    tcHeader[i].Attributes.Add("bgcolor", "#F7F6F3");
                }

                tcHeader.Add(new TableHeaderCell());
                tcHeader[leftoverC].Attributes.Add("colspan", countC.ToString());
                tcHeader[leftoverC].Text = "Grade C";
                tcHeader[leftoverC].Attributes.Add("bgcolor", "#F7F6F3");

                int j = leftoverC + 1;
                for (int i = leftoverC + countC; i < getalldatae.Columns.Count; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[j].Attributes.Add("rowspan", "2");
                    tcHeader[j].Text = Column[i];
                    tcHeader[j].Attributes.Add("bgcolor", "#F7F6F3");
                    j++;
                }
                tcHeader[j - 1].Text += "</th></tr><tr>";//换行;
                //Second Row
                for (int i = 0; i < countC; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[j].Text = ColumnC[i];
                    tcHeader[j].Attributes.Add("bgcolor", "#F7F6F3");
                    j++;
                }
                break;

            case DataControlRowType.DataRow:
                e.Row.Cells[5].Style.Add("word-break", "break-all");
                break;

            case DataControlRowType.Footer:
                break;
        }
        finishProgress();
        DateTime dateT = DateTime.Now;
        this.PrintDate.Text = dateT.ToString("yyyy-MM-dd HH:mm:ss");
    }

    private bool CheckQueryCondition()
    {
        string Strjo = this.txtJoNo.Text.Trim();
        string strBeginDate = this.txtFromDate.Text.Trim();
        string strEndDate = this.txtToDate.Text.Trim();

        if (Strjo.Length == 0 && strBeginDate.Length == 0 && strEndDate.Length == 0)
        {
            this.lblMessage.Text = "Please choose condition to query!";
            this.lblMessage.Visible = true;
            this.gvDetail.Visible = false;
            return false;
        }

        if (Strjo.Length == 0)
        {
            if (strBeginDate.Length == 0 && strEndDate.Length != 0 || strBeginDate.Length != 0 && strEndDate.Length == 0)
            {
                this.lblMessage.Text = "Please select accurate  Date!";
                this.lblMessage.Visible = true;
                this.gvDetail.Visible = false;
                return false;
            }
        }
        if (strBeginDate.Length != 0 && strEndDate.Length != 0 && Convert.ToDateTime(strBeginDate) > Convert.ToDateTime(strEndDate))
        {
            this.lblMessage.Text = "Selected To Date must after From Date!";
            this.lblMessage.Visible = true;
            this.gvDetail.Visible = false;
            return false;
        }

        return true;
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        switch (ddlReport.SelectedValue)
        {
            case "1":
                gvDetailGetData();
                break;
            case "2":
                gvFabricDetailGetData();
                break;
            case "3":
                gvGarmentDetailGetData();
                break;
        }
    }

    protected void gvDetailGetData()
    {
        this.lblMessage.Visible = false;
        if (true == this.CheckQueryCondition())
        {
            gvDetail.Visible = false;
            gvDetail.DataSource = null;
            gvDetail.DataBind();
            string JoList = "";
            if (this.txtFromDate.Text.Trim().Length != 0 && this.txtToDate.Text.Trim().Length != 0)
            {
                DataTable CheckMudata = MESComment.MUReportSql.GetExistsMuList(ddlFtyCd.SelectedItem.Value, txtFromDate.Text, txtToDate.Text, strRunNO);
                if (CheckMudata.Rows.Count == 1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "Select data from existing!";
                    strRunNO = CheckMudata.Rows[0]["RunNo"].ToString();
                    beginProgress();
                    SetTiile("Factory MU Report");
                    setProgress(1, "Process Mu data from MES system", "");
                    gvDetail.DataSource = MESComment.MUReportSql.GetLocalMUReportList(ddlFtyCd.SelectedItem.Value, strRunNO, "Old");
                    if (cbChecked.Checked)
                    {
                        gvDetail.Visible = true;
                    }
                    gvDetail.DataBind();
                }
                else
                {
                    if (txtJoNo.Text != "")
                    {
                        string[] value = txtJoNo.Text.Trim().Split(';');
                        for (int i = 0; i < value.Length; i++)
                        {
                            if (value[i] != "")
                            {
                                JoList += "'" + value[i] + "',";
                            }
                        }
                        JoList = JoList.Substring(0, JoList.Length - 1);
                        JoList = MESComment.MUReportSql.GetJo(JoList);
                        if (JoList.Length == 0)
                        {
                            this.lblMessage.Text = "Not Found any Data!";
                            this.lblMessage.Visible = true;
                            this.gvDetail.Visible = false;
                            finishProgress();
                            return;
                        }
                    }

                    if (cbChecked.Checked)
                    {
                        gvDetail.Visible = true;
                    }
                    getalldatae = GetMasterTable(JoList, ddlReport.Items[0].Value);

                    if (getalldatae.Rows.Count <= 0)
                    {
                        this.lblMessage.Text = "Not Found any Data!";
                        this.lblMessage.Visible = true;
                        this.gvDetail.Visible = false;
                        finishProgress();
                    }
                    else
                    {
                        gvDetail.DataSource = getalldatae;
                        gvDetail.DataBind();
                    }
                }
            }
            else
            {
                if (txtJoNo.Text != "")
                {
                    string[] value = txtJoNo.Text.Trim().Split(';');
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (value[i] != "")
                        {
                            JoList += "'" + value[i] + "',";
                        }
                    }
                    JoList = JoList.Substring(0, JoList.Length - 1);
                    JoList = MESComment.MUReportSql.GetJo(JoList);
                    if (JoList.Length == 0)
                    {
                       this.lblMessage.Text = "Not Found any Data!";
                    this.lblMessage.Visible = true;
                    this.gvDetail.Visible = false;
                        return;
                    }
                }

                if (cbChecked.Checked)
                {
                    gvDetail.Visible = true;
                }

                getalldatae = GetMasterTable(JoList, ddlReport.Items[0].Value);

                if (getalldatae.Rows.Count <= 0)
                {
                    this.lblMessage.Text = "Not Found any Data!";
                    this.lblMessage.Visible = true;
                    this.gvDetail.Visible = false;
                }
                else
                {
                    gvDetail.DataSource = getalldatae;
                    gvDetail.DataBind();
                }
            }


        }

    }

    protected void gvFabricDetailGetData()
    {
        this.lblMessage.Visible = false;

        if (true == this.CheckQueryCondition())
        {
            gvFabricDetail.Visible = false;
            gvFabricDetail.DataSource = null;
            gvFabricDetail.DataBind();            
            string JoList = "";
            if (this.txtFromDate.Text.Trim().Length != 0 && this.txtToDate.Text.Trim().Length != 0)
            {
                DataTable CheckMudata = MESComment.MUReportSql.GetExistsMuList(ddlFtyCd.SelectedItem.Value, txtFromDate.Text, txtToDate.Text, strRunNO);

                if (CheckMudata.Rows.Count == 1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "Select data from existing!";
                    strRunNO = CheckMudata.Rows[0]["RunNo"].ToString();
                    beginProgress();
                    SetTiile("Factory MU Report");
                    setProgress(1, "Process Mu data from MES system", "");

                    getalldatae = MESComment.MUReportSql.GetLocalMUReportFabricList(ddlFtyCd.SelectedItem.Value, strRunNO, "Old");
                    int maxRows = getalldatae.Rows.Count - 1;
                    Total = getalldatae.Rows[maxRows].ItemArray;
                    getalldatae.Rows[maxRows].Delete();
                    gvFabricDetail.DataSource = getalldatae;

                    if (cbChecked.Checked)
                    {
                        gvFabricDetail.Visible = true;
                    }
                    gvFabricDetail.DataBind();
                }
                else
                {
                    if (txtJoNo.Text != "")
                    {
                        string[] value = txtJoNo.Text.Trim().Split(';');
                        for (int i = 0; i < value.Length; i++)
                        {
                            if (value[i] != "")
                            {
                                JoList += "'" + value[i] + "',";
                            }
                        }
                        JoList = JoList.Substring(0, JoList.Length - 1);
                        JoList = MESComment.MUReportSql.GetJo(JoList);
                        if (JoList.Length == 0)
                        {
                            this.lblMessage.Text = "Not Found any Data!";
                            this.lblMessage.Visible = true;
                            this.gvDetail.Visible = false;
                            finishProgress();
                            return;
                        }
                    }

                    if (cbChecked.Checked)
                    {
                        gvFabricDetail.Visible = true;
                    }

                    getalldatae = GetMasterTable(JoList, ddlReport.Items[1].Value);
                    if (getalldatae.Rows.Count > 0)
                    {
                        int maxRow = getalldatae.Rows.Count - 1;
                        Total = getalldatae.Rows[maxRow].ItemArray;
                        getalldatae.Rows[maxRow].Delete();
                    }

                    if (getalldatae.Rows.Count <= 0)
                    {
                        this.lblMessage.Text = "Not Found any Data!";
                        this.lblMessage.Visible = true;
                        this.gvFabricDetail.Visible = false;
                        finishProgress();
                    }
                    else
                    {
                        gvFabricDetail.DataSource = getalldatae;
                        gvFabricDetail.DataBind();
                    }
                }
            }
            else
            {
                if (txtJoNo.Text != "")
                {
                    string[] value = txtJoNo.Text.Trim().Split(';');
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (value[i] != "")
                        {
                            JoList += "'" + value[i] + "',";
                        }
                    }
                    JoList = JoList.Substring(0, JoList.Length - 1);
                }

                if (cbChecked.Checked)
                {
                    gvFabricDetail.Visible = true;
                }

                getalldatae = GetMasterTable(JoList, ddlReport.Items[1].Value);

                if (getalldatae.Rows.Count > 0)
                {
                    int maxRow = getalldatae.Rows.Count - 1;
                    Total = getalldatae.Rows[maxRow].ItemArray;
                    getalldatae.Rows[maxRow].Delete();
                }

                if (getalldatae.Rows.Count <= 0)
                {
                    this.lblMessage.Text = "Not Found any Data!";
                    this.lblMessage.Visible = true;
                    this.gvFabricDetail.Visible = false;
                    finishProgress();
                }
                else
                {
                    gvFabricDetail.DataSource = getalldatae;
                    gvFabricDetail.DataBind();
                }
            }
        }
    }

    protected void gvGarmentDetailGetData()
    {
        this.lblMessage.Visible = false;

        if (true == this.CheckQueryCondition())
        {
            gvGarmentDetail.Visible = false;
            gvGarmentDetail.DataSource = null;
            gvGarmentDetail.DataBind();
            string JoList = "";
            if (this.txtFromDate.Text.Trim().Length != 0 && this.txtToDate.Text.Trim().Length != 0)
            {
                DataTable CheckMudata = MESComment.MUReportSql.GetExistsMuList(ddlFtyCd.SelectedItem.Value, txtFromDate.Text, txtToDate.Text, strRunNO);

                if (CheckMudata.Rows.Count == 1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "Select data from existing!";
                    strRunNO = CheckMudata.Rows[0]["RunNo"].ToString();
                    beginProgress();
                    SetTiile("Factory MU Report");
                    setProgress(1, "Process Mu data from MES system", "");
                    gvGarmentDetail.DataSource = MESComment.MUReportSql.GetLocalMUReportGarmentList(ddlFtyCd.SelectedItem.Value, strRunNO, txtFromDate.Text, txtToDate.Text, "Old",false);

                    if (cbChecked.Checked)
                    {
                        gvGarmentDetail.Visible = true;
                    }
                    gvGarmentDetail.DataBind();
                }
                else
                {
                    if (txtJoNo.Text != "")
                    {
                        string[] value = txtJoNo.Text.Trim().Split(';');
                        for (int i = 0; i < value.Length; i++)
                        {
                            if (value[i] != "")
                            {
                                JoList += "'" + value[i] + "',";
                            }
                        }
                        JoList = JoList.Substring(0, JoList.Length - 1);
                        JoList = MESComment.MUReportSql.GetJo(JoList);
                    }

                    if (cbChecked.Checked)
                    {
                        gvGarmentDetail.Visible = true;
                    }
                    getalldatae = GetMasterTable(JoList, ddlReport.Items[2].Value);

                    if (getalldatae.Rows.Count <= 0)
                    {
                        this.lblMessage.Text = "Not Found any Data!";
                        this.lblMessage.Visible = true;
                        this.gvGarmentDetail.Visible = false;
                        finishProgress();
                    }
                    else
                    {
                        gvGarmentDetail.DataSource = getalldatae;
                        gvGarmentDetail.DataBind();
                    }
                }
            }
            else
            {
                if (txtJoNo.Text != "")
                {
                    string[] value = txtJoNo.Text.Trim().Split(';');
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (value[i] != "")
                        {
                            JoList += "'" + value[i] + "',";
                        }
                    }
                    JoList = JoList.Substring(0, JoList.Length - 1);
                    JoList = MESComment.MUReportSql.GetJo(JoList);
                    if (JoList.Length == 0)
                    {
                        this.lblMessage.Text = "Not Found any Data!";
                        this.lblMessage.Visible = true;
                        this.gvDetail.Visible = false;
                        finishProgress();
                        return;
                    }
                }
                if (cbChecked.Checked)
                {
                    gvGarmentDetail.Visible = true;
                }
                getalldatae = GetMasterTable(JoList, ddlReport.Items[2].Value);
                if (getalldatae.Rows.Count <= 0)
                {
                    this.lblMessage.Text = "Not Found any Data!";
                    this.lblMessage.Visible = true;
                    this.gvGarmentDetail.Visible = false;
                    finishProgress();
                }
                else
                {
                    gvGarmentDetail.DataSource = getalldatae;
                    gvGarmentDetail.DataBind();
                }
            }
        }
    }


}
