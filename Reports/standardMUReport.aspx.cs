using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;

public partial class Reports_standardMUReport : pPage
{
    string[] strHeader = { 
                             "Fty Job order", "YPD Job No", "Dlvy  Date", "Buyer", "sale Contract", "PPO Number", "Fabric Width", "Long/ short sleeve", "Wash type", "Fabric Pattern", "Marker Utilization (%)","Fabric (YDS)","Garment (DZ)","YPD","RATIO","Buyer's max over shipment allowance","Buyer's min short shipment allowance</th></tr><tr>",
                             "PPO order yds","Allocated","Allocated Qty (Without Discount)","Issued","Marker Audited","Ship Yardage","Cutting Wastage","Leftover fabric","SRN","RTW","Order(DZ)","Cut","Shipped","Sample","Pull-out","Leftover Garment","Sewing Wastage","Washing Wastage","Unacc Gmt","PPO MKR YPD","BULK NET YPD","GIVEN CUT YPD","BULK MKR YPD","YPD Var","CUT YPD","SHIP YPD","Ship-to-Cut","Ship-to-Receipt","Ship-to-Order","Cut-to-Receipt","Cut-to-Order</th></tr><tr>",
                             "Yds","%","Grade A","Grade B","Grade C","Total","DZ","%","DZ","%"
                         };
    string userid = "";

    public int ship_order_count_95, ship_order_count_100, YPD_Var_big, YPD_Var_small;
    public double ship_order_sum;
    public string FabricPattern = "";
    public double Total_Allocated = 0;//开放数据

    public double Total_Allocated1 = 0;//开放数据

    public double Total_Ship_Yardage = 0;//开放数据

    public double Total_Leftover = 0;//开放数据
    private string reporttype = "smr";
    public string strRunNO = "";
    private string Strmsg = "";
    public string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
    DataTable StandardGisInfo;
    DataTable StandardMUReportList;

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
            divSummary.Visible = false;
            if (userid != "")
            {
                DataTable dtAuthorise = MESComment.MUReportSql.LockAuthorise(userid);

                if (dtAuthorise.Rows.Count > 0)
                {
                    btnLock.Visible = true;

                }
                else
                {
                    btnLock.Visible = false;

                }
            }
            else
            {
                btnLock.Visible = false;

            }
        }
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString() != "DEV")
            {
                ddlFtyCd.SelectedValue = Request.QueryString["site"].ToString();
            }
            else
            {
                ddlFtyCd.SelectedValue = "GEG";
            }
        }
    }

    /// <summary>
    /// add by ming 20160714
    /// </summary>
    /// <param name="JoList"></param>
    /// <param name="ReportType"></param>
    /// <returns></returns>
    private DataTable GetMasterTable(string JoList)
    {
        int Step = 1;
        int StepTotal = 15;
        Random ro = new Random(1000);
        strRunNO = Convert.ToString(string.Format("{0:yy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(10).ToString();
        string Strmsg = "|--Process Standard MU Data begin(Total " + StepTotal + " Step)";
      
        SetTiile("Standard MU Report");
        Strmsg += "<br/>|--1. Process JO list begin...";
        setProgress(Step * 100 / StepTotal, Strmsg, "Process JO list ...(" + Step + "/" + StepTotal + ")");
        int Row1 = 1;
        int Row2 = 1;
        DbConnection invConn = null;

        //查所有符合条件的JO

        //从取FIN OUTPUT改回GIS DataTable StandardJOList = MESComment.MUReportSql.GetJoListforStandardMu(JoList, ddlFtyCd.SelectedItem.Value, ddlGarmentType.SelectedItem.Value, txtFromDate.Text, txtToDate.Text, strRunNO);
        //取GIS的出货数，单号为OriJo
        DataTable StandardJOList = MESComment.MUReportSql.GetStandardGisInfo(JoList, ddlFtyCd.SelectedItem.Value, ddlGarmentType.SelectedItem.Value, ddlOutSource.SelectedItem.Value, txtFromDate.Text, txtToDate.Text, true, false, strRunNO);
        if (StandardJOList.Rows.Count <= 0)
        {
            finishProgress();
            return StandardJOList;
        }

        //取CB单
        Dictionary<string, List<string>> CombineJo = MESComment.MUReportSql.GetCombineJo(GetFormatJo(StandardJOList));
        //当从gis取出cb单，但却不是所有的ori单时,(即cb单，只有部分ori开了发票）
        List<string> oriJoList = new List<string>();
        System.Text.StringBuilder JoListall = new System.Text.StringBuilder(); ////所有单，CB单取Origal_JO_NO
        System.Text.StringBuilder cbJolist = new System.Text.StringBuilder(); //所有cb的单，只包含CB单
        System.Text.StringBuilder JoCBList =new System.Text.StringBuilder(); //所有单,包括NO CB 和CB单
        //Dictionary<string, List<string>> CombineJo = new Dictionary<string, List<string>>();//只取CB单，用于合并单
        DataTable jobtb = new DataTable();    //所有单，CB拆分源始单，用户查询
        DataTable cbjobtb = new DataTable();  //包所有的Origal_JO_NO和CB JO
        jobtb.Columns.Add("jobno");
        cbjobtb.Columns.Add("jobno");
        DataRow row_ = null;
        foreach (DataRow dr in StandardJOList.Rows)
        {
            if(!oriJoList.Contains(dr["JO_NO"].ToString()))
            {
                oriJoList.Add(dr["JO_NO"].ToString());
                JoListall.AppendFormat("'{0}',", dr["JO_NO"]);
            }
            row_ = jobtb.NewRow();
            row_["jobno"] = dr["JO_NO"];
            jobtb.Rows.Add(row_);
            row_ = cbjobtb.NewRow();
            row_["jobno"] = dr["JO_NO"];
            cbjobtb.Rows.Add(row_);
           
            JoCBList.AppendFormat("'{0}',", dr["JO_NO"]);
        }

        foreach (string key in CombineJo.Keys)
        {
            cbJolist.AppendFormat("'{0}',", key);
            JoCBList.AppendFormat("'{0}',", key);
            row_ = cbjobtb.NewRow();
            row_["jobno"] = key;
            cbjobtb.Rows.Add(row_);
            foreach (string jo in CombineJo[key])
            {
                if (!oriJoList.Contains(jo))
                {
                    oriJoList.Add(jo);
                    JoListall.AppendFormat("'{0}',", jo);
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
        //JoList = JoList.Substring(0, JoList.Length - 1);//不要逗号；
        //if (JoCBList.Length>0)
        //     JoCBList = JoCBList.Remove(JoCBList.Length - 1, 1);
        //取GO信息,因为SC_LOT表没有CB单，所以只能取源单


        DataTable StandardGOList = MESComment.MUReportSql.GetGoListforStandardMu(JoListall.ToString(), ddlFtyCd.SelectedItem.Value, txtFromDate.Text, txtToDate.Text, ddlGarmentType.SelectedItem.Value);
        StandardGOList = GetCombineSCTable(CombineJo, StandardGOList);
        DataTable StandardGisInfo = null;
        DataTable StandardWidthNpatternList = null;
        DataTable LeftOver = null;
        DbConnection eelConn = null;
        DbConnection MESConn = null;
        //DbConnection GisConn = null;
        DataTable StandardSRNAndRTW = null;
        DataTable LeftoverPPOList = null;
        DbConnection hlConn = null;
       
        DataTable LeftoverAB = null;
        Strmsg += "<br/>|--Process JO list finish.";
        Row1++;
        Row2 = 0;
        Row2++;
      
        setProgress(Step * 100 / StepTotal, Strmsg, "Get Mes Fin OutPut Data ...(" + Step + "/" + StepTotal + ")");
        //取SC_NO,JO,Ship_date,Ship_QTY,LASTJO  --这里取回FIN OUT PUT,ShipDate 取PROD_COMPLETE_DATE
        //StandardGisInfo = MESComment.MUReportSql.GetStandardGisInfo(JoCBList.ToString(), ddlFtyCd.SelectedItem.Value, ddlGarmentType.SelectedItem.Value); xuzm 20161115
        StandardGisInfo=GetCombineGISTable(CombineJo, StandardJOList);
        Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Get Invoice Data End.<br/>|--&nbsp;&nbsp;&nbsp;&nbsp;Process Fin OutPut Data End.";
        Row1++;
        Row2 = 0;
        Strmsg += "<br/>|--" + Row1 + ". Get GO/PPO Mapping data Begin:";
        Step++;
        setProgress(Step * 100 / StepTotal, Strmsg, "Get GO/PPO Mapping data ...(" + Step + "/" + StepTotal + ")");

        eelConn = MESComment.DBUtility.GetConnection("EEL");
        
        MESComment.DBUtility.InsertRptTempData(jobtb, eelConn);
        //MESComment.DBUtility.InsertRptTempData(cbjobtb, hlConn); 
        //取SC_NO,PPO_NO,WIDTH,PATTERN_TYPE,PPO_YPD,YPD_JOB_NO
        StandardWidthNpatternList = MESComment.MUReportSql.GetStandardWidthNpatternList(eelConn);
        Strmsg += "<br/>|--&nbsp;&nbsp;&nbsp;&nbsp;Get GO/PPO Mapping data End";
        Row1++;
        Row2 = 0;
        Row2++;
        Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Push Jo List to Fab Inv System Begin:";
        Step++;
        setProgress(Step * 100 / StepTotal, Strmsg, "Push Jo List to Fab Inv System ...(" + Step + "/" + StepTotal + ")");
        invConn = MESComment.DBUtility.GetConnection("INV");
        //MESComment.DBUtility.InsertRptTempData(jobtb, invConn);
        MESComment.DBUtility.InsertRptTempData(cbjobtb, invConn); 
        Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Push Jo List to Fab Inv System End";
        Row2++;
        Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Get Fabric Issue/Leftover Garment Data Begin:";
        Step++;
        setProgress(Step * 100 / StepTotal, Strmsg, "Get Fabric Issue/Leftover Garment Data ...(" + Step + "/" + StepTotal + ")");
        //取RTM，SRN，LeaveOverA,B;sew_qty_b,wash_qty_b ；改为取RTM，SRN和sew_qty_b,wash_qty_b，LeaveOverA,B取MES LeaveOver 
        StandardSRNAndRTW = MESComment.MUReportSql.GetStatudardGRN(ddlFtyCd.SelectedItem.Value, invConn);
        StandardSRNAndRTW = GetCombineTable(CombineJo, StandardSRNAndRTW);
        //LeftoverAB = MESComment.MUReportSql.GetStatudardLeftoverAB(ddlFtyCd.SelectedItem.Value, JoListall.ToString());
        LeftoverAB = MESComment.MUReportSql.GetStatudardLeftoverAB(ddlFtyCd.SelectedItem.Value, invConn);
        LeftoverAB = GetCombineTable(CombineJo, LeftoverAB);
        CombineLeftoverAB(StandardSRNAndRTW, LeftoverAB);
        Row2++;
        Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Push PPO list to Fab Inv system Begin:";
        Step++;
        setProgress(Step * 100 / StepTotal, Strmsg, "Push PPO list to Fab Inv system...(" + Step + "/" + StepTotal + ")");
        //插入所有PPO
        MESComment.DBUtility.InsertRptPPOTempData(StandardWidthNpatternList, invConn);//OK
        Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Push PPO list to Fab Inv system End";
        Row2++;
        Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Get Fabric LeftOver Data Begin:";
        Step++;
        setProgress(Step * 100 / StepTotal, Strmsg, "Get Fabric LeftOver Data...(" + Step + "/" + StepTotal + ")");
        //通过PPO 取LeftOver
        LeftOver = MESComment.MUReportSql.GetStandardLeftOver(ddlFtyCd.SelectedItem.Value, invConn);//lml
        Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Get Fabric LeftOver Data End";
        Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Get Fabric Issue/Leftover Garment Data End";

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

        MESConn = MESComment.DBUtility.GetConnection("MES_UPDATE");//OK

        MESComment.DBUtility.InsertMESMUData(StandardGOList, strRunNO, reporttype, MESConn);

        Row2++;
        Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Push Ship data(GIS) Begin...";
        Step++;
        setProgress(Step * 100 / StepTotal, Strmsg, "Push Ship data(GIS)...(" + Step + "/" + StepTotal + ")");
        //插入Ship_date,Ship_QTY
        MESComment.DBUtility.InsertMESRptShipData(StandardGisInfo, strRunNO, ddlFtyCd.SelectedItem.Value, MESConn);//OK        
        Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Push Ship data(GIS) End";

        Row2++;
        //MESConn = MESComment.DBUtility.GetConnection("MES_UPDATE");//OK
        Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Push PPO/GO data Begin...";
        Step++;
        setProgress(Step * 100 / StepTotal, Strmsg, "Push PPO/GO data ...(" + Step + "/" + StepTotal + ")");
        //SC_NO,PPO_NO,WIDTH,PATTERN_TYPE,PPO_YPD,YPD_JOB_NO
        MESComment.DBUtility.InsertMESRptWidthNpatternData(StandardWidthNpatternList, strRunNO, ddlFtyCd.SelectedItem.Value, MESConn);//OK
        Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Push PPO/GO data End";

        Row2++;
        //MESConn = MESComment.DBUtility.GetConnection("MES_UPDATE");//OK
        Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Push Fabric & Left Garment data Begin...";
        Step++;
        setProgress(Step * 100 / StepTotal, Strmsg, "Push Fabric & Left Garment data ...(" + Step + "/" + StepTotal + ")");
        //RTM，SRN，LeaveOverA,B;sew_qty_b,wash_qty_b ；改为取RTM，SRN和sew_qty_b,wash_qty_b，LeaveOverA,B取MES LeaveOver 
        MESComment.DBUtility.InsertMESRptFGISData(StandardSRNAndRTW, strRunNO, ddlFtyCd.SelectedItem.Value, MESConn);//LML
        Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Push Fabric & Left Garment data  End";

        Row2++;
        Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--" + Row1 + "." + Row2 + " Push Fabric LeftOver data Begin...";
        Step++;
        setProgress(Step * 100 / StepTotal, Strmsg, " Push Fabric LeftOver data ...(" + Step + "/" + StepTotal + ")");
        //LeftOver
        MESComment.DBUtility.InsertMESRptLeftOverData(LeftOver, strRunNO, ddlFtyCd.SelectedItem.Value, MESConn);



        /////////////////////////// EEL ////////////////////////////////////////
        DataTable StandardOrderQty = null;
        DataTable StandardOrderPPO = null;
        hlConn = MESComment.DBUtility.GetConnection("HL");
        LeftoverPPOList = MESComment.MUReportSql.GetSCList(strRunNO);
        //插入所有PPO
        MESComment.DBUtility.InsertRptTempData(LeftoverPPOList, eelConn);

        Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Push Fabric LeftOver data  End";
        Row1++;
        Row2 = 0;
        Strmsg += "<br/>|--" + Row1 + ". Get GO PPO order QTY Begin:";
        Step++;
        setProgress(Step * 100 / StepTotal, Strmsg, "Get GO PPO order QTY ...(" + Step + "/" + StepTotal + ")");
        //StandardOrderQty = MESComment.MUReportSql.GetStandardGoQty(eelConn);//ok
        StandardOrderPPO = MESComment.MUReportSql.GetStandardPPONO(eelConn);
        MESComment.DBUtility.InsertPPOTempData(StandardOrderPPO, hlConn);
        StandardOrderQty = MESComment.MUReportSql.GetStandardGoQty_New(hlConn);
        MESComment.DBUtility.CloseConnection(ref eelConn);
        MESComment.DBUtility.CloseConnection(ref hlConn);
        MESComment.DBUtility.InsertMESRptStandardOrderQtyData(StandardOrderQty, strRunNO, ddlFtyCd.SelectedItem.Value, MESConn);//OK
        Strmsg += "<br/>|--&nbsp;&nbsp;&nbsp;&nbsp;Get GO PPO order QTY End.";

        Strmsg += "<br/>|--&nbsp;&nbsp;&nbsp;&nbsp;Push Data to MES End";
        Row1++;
        Row2 = 0;
        Strmsg += "<br/>|--" + Row1 + ". Process all data in mes begin...";
        Step++;
        setProgress(Step * 100 / StepTotal, Strmsg, "Process all data in mes  ...(" + Step + "/" + StepTotal + ")");
        MESComment.DBUtility.CloseConnection(ref MESConn);
        DataTable StandardMUReportList = null;
       
        StandardMUReportList = MESComment.MUReportSql.GetStandardMUReportList(ddlFtyCd.SelectedItem.Value, strRunNO, "New");
   
        return StandardMUReportList;
    }
   
    /// <summary>
    /// 已弃用
    /// </summary>
    /// <param name="JoList"></param>
    /// <returns></returns>
    private DataTable GetMasterTable1(string JoList)
    {
       
        JoList = "";
        string JoCBList = "";
        DataTable jobtb = new DataTable();
        DataTable jobcbtb = new DataTable();
        jobtb.Columns.Add("jobno");
        jobcbtb.Columns.Add("jobno");
        DataRow row_ = null;
        Dictionary<string, List<string>> CombineJo = new Dictionary<string, List<string>>();
        for (int i = StandardGisInfo.Rows.Count-1; i>=0; i--)
        {
            if (StandardGisInfo.Rows[i]["JO_NO"].ToString() != "")
            {
                JoList += "'" + StandardGisInfo.Rows[i]["ORIGINAL_JO_NO"].ToString() + "',";
                row_ = jobtb.NewRow();
                row_["jobno"] = StandardGisInfo.Rows[i]["ORIGINAL_JO_NO"].ToString();
                jobtb.Rows.Add(row_);
                if (!CombineJo.ContainsKey(StandardGisInfo.Rows[i]["JO_NO"].ToString()))
                {
                    JoCBList += "'" + StandardGisInfo.Rows[i]["JO_NO"].ToString() + "',";
                    CombineJo.Add(StandardGisInfo.Rows[i]["JO_NO"].ToString(), new List<string>() { StandardGisInfo.Rows[i]["ORIGINAL_JO_NO"].ToString() });
                }
                else
                {
                    CombineJo[StandardGisInfo.Rows[i]["JO_NO"].ToString()].Add(StandardGisInfo.Rows[i]["ORIGINAL_JO_NO"].ToString());
                    StandardGisInfo.Rows.RemoveAt(i);
                }
            }
        }

        JoList = JoList.Substring(0, JoList.Length - 1);
        JoCBList = JoCBList.Substring(0, JoCBList.Length - 1);
      
        /////////////////////////// EEL ////////////////////////////////////////
      
        DbConnection eelConn = MESComment.DBUtility.GetConnection("EEL");
        MESComment.DBUtility.InsertRptTempData(jobtb, eelConn);
       
        DataTable StandardWidthNpatternList = MESComment.MUReportSql.GetStandardWidthNpatternList(eelConn);
        
        DataTable StandardOrderQty = MESComment.MUReportSql.GetStandardOrderQty(eelConn);
      
        DataTable StandardJoOrderQty = MESComment.MUReportSql.GetJoOrderQty(eelConn);
      
        DataTable StandardGoOrderQty = MESComment.MUReportSql.GetStandardGoOrderQty(eelConn);
        MESComment.DBUtility.CloseConnection(ref eelConn);

        /////////////////////////// INV ////////////////////////////////////////
        DbConnection invConn = MESComment.DBUtility.GetConnection("INV");
       
        MESComment.DBUtility.InsertRptTempData(jobtb, invConn);
       
        //DataTable StandardMUReportList = MESComment.MUReportSql.GetStandardMUReportList(invConn);
        StandardMUReportList = MESComment.MUReportSql.GetStandardMUReportList(invConn);
        
        DataTable StandardFGIS = MESComment.MUReportSql.GetStandardFromFGIS(ddlFtyCd.SelectedItem.Value, invConn);
        
        DataTable StandardRTW = MESComment.MUReportSql.GetStandardRTW(ddlFtyCd.SelectedItem.Value, invConn);
       
        DataTable StandardSRN = MESComment.MUReportSql.GetStandardSRN(ddlFtyCd.SelectedItem.Value, invConn);
        MESComment.DBUtility.CloseConnection(ref invConn);



       
        DataTable StandardMES = MESComment.MUReportSql.GetStandardFromMES(ddlFtyCd.SelectedItem.Value, JoCBList);
    
        DataTable StandardMUCutInfo = MESComment.MUReportSql.GetStandardMUCutInfo(JoCBList, ddlFtyCd.SelectedItem.Value, txtFromDate.Text, txtToDate.Text);
      
        DataTable StandardGivenCutAllowance = MESComment.MUReportSql.GetStandardGivenCutAllowance(JoCBList, ddlFtyCd.SelectedItem.Value);
       
        DataTable RemnantDataTable = MESComment.MUReportSql.GetStandardRemnantByJoList(JoCBList);
      
        DataTable ScQtyDataTable = MESComment.MUReportSql.Get_TTL_SC_QTY_BY_PO_LIST(JoList, ddlFtyCd.SelectedItem.Value);

     

        GetCombineJobTable(CombineJo, StandardMUReportList);


        StandardFGIS = GetCombineFGISTable(CombineJo, StandardFGIS);


        StandardRTW = GetCombineRTWSRNTable(CombineJo, StandardRTW);


        StandardSRN = GetCombineRTWSRNTable(CombineJo, StandardSRN);

        StandardJoOrderQty = GetCombineJoOrderTable(CombineJo, StandardJoOrderQty);

        ScQtyDataTable = GetCombineJoOrderTable(CombineJo, ScQtyDataTable);

      
      
        ///Close Connection(conn)

        ///conn = get connection("INV")
        ///insert temp table
        ///search data
        ///....
        ///Close Connection(conn)
        StandardMUReportList.Columns.Add("CUT_DATE");
        StandardMUReportList.Columns.Add("PULLOUT_QTY");
        StandardMUReportList.Columns.Add("DISCREPANCY_QTY");
        StandardMUReportList.Columns.Add("PPO_NO");
        StandardMUReportList.Columns.Add("WIDTH");
        StandardMUReportList.Columns.Add("PATTERN_TYPE");
        StandardMUReportList.Columns.Add("PPO_YPD");
        StandardMUReportList.Columns.Add("YPD_JOB_NO");
        StandardMUReportList.Columns.Add("ORDER_QTY");
        StandardMUReportList.Columns.Add("SAMPLE_QTY");
        StandardMUReportList.Columns.Add("MU");
        StandardMUReportList.Columns.Add("ALLOCATED_QTY");
        StandardMUReportList.Columns.Add("BULK_MKR_YPD");
        StandardMUReportList.Columns.Add("BULK_NET_YPD");
        StandardMUReportList.Columns.Add("GIVEN_CUT_YPD");
        StandardMUReportList.Columns.Add("MA");
        StandardMUReportList.Columns.Add("REMNANT");
        StandardMUReportList.Columns.Add("SHIP_DATE");
        StandardMUReportList.Columns.Add("SHIP_QTY");
        StandardMUReportList.Columns.Add("CUTQTY");
        StandardMUReportList.Columns.Add("ORDERQTY");
        StandardMUReportList.Columns.Add("GMT_QTY_A");
        StandardMUReportList.Columns.Add("GMT_QTY_B");
        StandardMUReportList.Columns.Add("SRNQty");
        StandardMUReportList.Columns.Add("RTW_QTY");
        StandardMUReportList.Columns.Add("Issued");
        StandardMUReportList.Columns.Add("Leftover");
        StandardMUReportList.Columns.Add("allocatedQty");
        StandardMUReportList.Columns.Add("allocatedQty(nodiscount)");
        StandardMUReportList.Columns.Add("ShipYardage");
        StandardMUReportList.Columns.Add("OVERSHIP");
        StandardMUReportList.Columns.Add("SewingDz");
        StandardMUReportList.Columns.Add("WashingDz");
        StandardMUReportList.Columns.Add("UnaccGmt");
        StandardMUReportList.Columns.Add("GMT_QTY_TOTAL");
        
       
       
        String AllPPOList = "";


        for (int j = 0; j < StandardWidthNpatternList.Rows.Count; j++)
        {
            if (StandardWidthNpatternList.Rows[j]["PPO_NO"].ToString() != "")
            {
                AllPPOList += "'" + StandardWidthNpatternList.Rows[j]["PPO_NO"].ToString() + "',";
            }
        }

        if (AllPPOList != "")
            AllPPOList = AllPPOList.Substring(0, AllPPOList.Length - 1);
        
        DataTable leftOverTable = MESComment.MUReportSql.GetStandardLeftOver(AllPPOList, ddlFtyCd.SelectedItem.Value);
     

        double SEW_WASTAGE = 0, WASH_WASTAGE = 0;
       
        for (int i = 0; i < StandardMUReportList.Rows.Count; i++)
        {
            DataRow row = StandardMUReportList.Rows[i];
            String poNoStr = row["PO_NO"].ToString();
            String scNoStr = row["SC_NO"].ToString();
            double myCutQty = 0;
            for (int j = 0; j < StandardMES.Rows.Count; j++)
            {
                
                //Modified by MF on 20160205, add .Trim() to extinguish the space in JO
                if (StandardMES.Rows[j]["job_order_no"].ToString().ToLower().Trim() == poNoStr.ToLower().Trim())
                {
                    StandardMUReportList.Rows[i]["CUTQTY"] = double.Parse(StandardMES.Rows[j]["cutqty"].ToString()).ToString("f2");
                    StandardMUReportList.Rows[i]["CUT_DATE"] = StandardMES.Rows[j]["CUT_DATE"];
                    StandardMUReportList.Rows[i]["PULLOUT_QTY"] = double.Parse(StandardMES.Rows[j]["PULLOUT_QTY"].ToString()).ToString("f2");
                    StandardMUReportList.Rows[i]["DISCREPANCY_QTY"] = double.Parse(StandardMES.Rows[j]["DISCREPANCY_QTY"].ToString()).ToString("f2");
                    StandardMUReportList.Rows[i]["SAMPLE_QTY"] = double.Parse(StandardMES.Rows[j]["SAMPLE_QTY"].ToString());
                    StandardMUReportList.Rows[i]["GMT_QTY_A"] = (StandardMES.Rows[j]["gmt_qty_b"].ToDouble() / 12).ToString("f2");
                    StandardMUReportList.Rows[i]["GMT_QTY_B"] = (StandardMES.Rows[j]["GMT_QTY_B"].ToDouble() / 12).ToString("f2");    
                    myCutQty = double.Parse(StandardMES.Rows[j]["cutqty"].ToString());
                    SEW_WASTAGE = StandardMES.Rows[j]["SEW_WASTAGE_C"].ToString() == "" ? double.Parse("0.00") : double.Parse(StandardMES.Rows[j]["SEW_WASTAGE_C"].ToString());
                    WASH_WASTAGE = StandardMES.Rows[j]["WASH_WASTAGE_C"].ToString() == "" ? double.Parse("0.00") : double.Parse(StandardMES.Rows[j]["WASH_WASTAGE_C"].ToString());
                    break;
                }
                //End of modified by MF on 20160205, add .Trim() to extinguish the space in JO
            }
            String ppoNo = "";
            String width = "";
            String patternType = "";

            double bulkYpd = 0;

            String ppoYpdNo = "";
            String orderQty = "0.00";

            for (int j = 0; j < StandardWidthNpatternList.Rows.Count; j++)
            {
                String scNoStrTmp = StandardWidthNpatternList.Rows[j]["SC_NO"].ToString();
                String ppoNoStr = StandardWidthNpatternList.Rows[j]["PPO_NO"].ToString();
                if (scNoStrTmp.ToLower() == scNoStr.ToLower())
                {
                    if (ppoNo.IndexOf(StandardWidthNpatternList.Rows[j]["PPO_NO"].ToString()) < 0)
                    {
                        if (StandardWidthNpatternList.Rows[j]["PPO_NO"].ToString() != "")
                        {
                            if (ppoNo != "")
                            {
                                ppoNo += ";" + StandardWidthNpatternList.Rows[j]["PPO_NO"].ToString();
                            }
                            else
                            {
                                ppoNo += "" + StandardWidthNpatternList.Rows[j]["PPO_NO"].ToString();
                            }
                        }
                    }
                    if (ppoYpdNo.IndexOf(StandardWidthNpatternList.Rows[j]["YPD_JOB_NO"].ToString()) < 0)
                    {
                        if (StandardWidthNpatternList.Rows[j]["YPD_JOB_NO"].ToString() != "")
                        {
                            if (ppoYpdNo != "")
                            {
                                ppoYpdNo += ";" + StandardWidthNpatternList.Rows[j]["YPD_JOB_NO"].ToString();
                            }
                            else
                            {
                                ppoYpdNo += "" + StandardWidthNpatternList.Rows[j]["YPD_JOB_NO"].ToString();
                            }
                        }
                    }
                    if (width.IndexOf(StandardWidthNpatternList.Rows[j]["WIDTH"].ToString()) < 0)
                    {
                        if (StandardWidthNpatternList.Rows[j]["WIDTH"].ToString() != "")
                        {
                            if (width != "")
                            {
                                width += ";" + StandardWidthNpatternList.Rows[j]["WIDTH"].ToString();
                            }
                            else
                            {
                                width += "" + StandardWidthNpatternList.Rows[j]["WIDTH"].ToString();
                            }
                        }
                    }
                    if (patternType.IndexOf(StandardWidthNpatternList.Rows[j]["PATTERN_TYPE"].ToString()) < 0)
                    {
                        if (StandardWidthNpatternList.Rows[j]["PATTERN_TYPE"].ToString() != "")
                        {
                            if (patternType != "")
                            {
                                patternType += ";" + StandardWidthNpatternList.Rows[j]["PATTERN_TYPE"].ToString();
                            }
                            else
                            {
                                patternType += "" + StandardWidthNpatternList.Rows[j]["PATTERN_TYPE"].ToString();
                            }
                        }
                    }
                }
            }
       
      
            StandardMUReportList.Rows[i]["PPO_NO"] = ppoNo;
            StandardMUReportList.Rows[i]["WIDTH"] = width;
            StandardMUReportList.Rows[i]["PATTERN_TYPE"] = patternType;
            StandardMUReportList.Rows[i]["YPD_JOB_NO"] = ppoYpdNo;

            for (int j = 0; j < StandardMUCutInfo.Rows.Count; j++)
            {
                String poNoStrTmp = StandardMUCutInfo.Rows[j]["JOB_ORDER_NO"].ToString();
                if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                {
                    bulkYpd = 0;
                    if (StandardMUCutInfo.Rows[j]["BULK_NET_YPD"].ToString() != "")
                    {
                        bulkYpd = double.Parse(StandardMUCutInfo.Rows[j]["BULK_NET_YPD"].ToString());
                    }
                    if (StandardMUCutInfo.Rows[j]["PPO_NET_YPD"].ToString() != "")
                    {
                        StandardMUReportList.Rows[i]["PPO_YPD"] = double.Parse(StandardMUCutInfo.Rows[j]["PPO_NET_YPD"].ToString()).ToString("f2");
                    }
                    double myMA = myCutQty * bulkYpd; ;
                    StandardMUReportList.Rows[i]["MU"] = double.Parse(StandardMUCutInfo.Rows[j]["MU"].ToString()).ToString("f2") + "%";
                    StandardMUReportList.Rows[i]["BULK_MKR_YPD"] = double.Parse(StandardMUCutInfo.Rows[j]["BULK_MARKER_YPD"].ToString()).ToString("f2");
                    StandardMUReportList.Rows[i]["BULK_NET_YPD"] = double.Parse(StandardMUCutInfo.Rows[j]["BULK_NET_YPD"].ToString()).ToString("f2");
                    StandardMUReportList.Rows[i]["MA"] = myMA.ToString("f2");
                }
            }

            for (int j = 0; j < StandardFGIS.Rows.Count; j++)
            {
                String poNoStrTmp = StandardFGIS.Rows[j]["job_order_no"].ToString();
                if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                {
                    //StandardMUReportList.Rows[i]["GMT_QTY_A"] = StandardFGIS.Rows.Count > 0 ? (double.Parse(StandardFGIS.Rows[j]["gmt_qty_a"].ToString()) / 12).ToString("f2") : double.Parse("0.00").ToString("f2");
                    //StandardMUReportList.Rows[i]["GMT_QTY_B"] = StandardFGIS.Rows.Count > 0 ? (double.Parse(StandardFGIS.Rows[j]["gmt_qty_b"].ToString()) / 12).ToString("f2") : double.Parse("0.00").ToString("f2");
                    SEW_WASTAGE += StandardFGIS.Rows[j]["sew_qty_b"].ToString() == "" ? double.Parse("0.00") : double.Parse(StandardFGIS.Rows[j]["sew_qty_b"].ToString());
                    WASH_WASTAGE += StandardFGIS.Rows[j]["wash_qty_b"].ToString() == "" ? double.Parse("0.00") : double.Parse(StandardFGIS.Rows[j]["wash_qty_b"].ToString());
                    break;
                }
            }

            StandardMUReportList.Rows[i]["GMT_QTY_TOTAL"] = (double.Parse(StandardMUReportList.Rows[i]["GMT_QTY_A"].ToString() == "" ? "0" : StandardMUReportList.Rows[i]["GMT_QTY_A"].ToString()) + double.Parse(StandardMUReportList.Rows[i]["GMT_QTY_B"].ToString() == "" ? "0" : StandardMUReportList.Rows[i]["GMT_QTY_B"].ToString()) + double.Parse(StandardMUReportList.Rows[i]["DISCREPANCY_QTY"].ToString() == "" ? "0" : StandardMUReportList.Rows[i]["DISCREPANCY_QTY"].ToString())).ToString("f2");

            //StandardMUReportList.Rows[i]["REMNANT"] = GetStringValueByJoFromDataTable(RemnantDataTable, poNoStr);
            StandardMUReportList.Rows[i]["REMNANT"] = 0;

            for (int j = 0; j < StandardGisInfo.Rows.Count; j++)
            {
                String poNoStrTmp = StandardGisInfo.Rows[j]["JO_NO"].ToString();
                if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                {
                    StandardMUReportList.Rows[i]["SHIP_DATE"] = StandardGisInfo.Rows[j]["SHIP_DATE"];

                    StandardMUReportList.Rows[i]["SHIP_QTY"] = Convert.ToDecimal(StandardMUReportList.Rows[i]["SHIP_QTY"] == DBNull.Value ? "0" : StandardMUReportList.Rows[i]["SHIP_QTY"])
                                        + Convert.ToDecimal(StandardGisInfo.Rows[j]["SHIP_QTY"].ToString() == "" ?
                                                double.Parse("0.00").ToString("f2") :
                                                (double.Parse(StandardGisInfo.Rows[j]["SHIP_QTY"].ToString()) / 12).ToString("f2")
                                             );
                    break;
                }
            }
            string JoOrderQty = "0.00";
            for (int j = 0; j < StandardJoOrderQty.Rows.Count; j++)
            {
                String poNoStrTmp = StandardJoOrderQty.Rows[j]["JO_NO"].ToString();
                if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                {
                    if (StandardJoOrderQty.Rows[j]["OrderQTY"].ToString() != "")
                    {
                        JoOrderQty = StandardJoOrderQty.Rows[j]["OrderQTY"].ToString();
                    }
                    StandardMUReportList.Rows[i]["ORDERQTY"] = StandardJoOrderQty.Rows[j]["OrderQTY"].ToString() == "" ? double.Parse("0.00").ToString("f2") : double.Parse(StandardJoOrderQty.Rows[j]["OrderQTY"].ToString()).ToString("f2");
                    StandardMUReportList.Rows[i]["OVERSHIP"] = StandardJoOrderQty.Rows[j]["OVERSHIP"].ToString() == "" ? double.Parse("0.00").ToString("f2") : double.Parse(StandardJoOrderQty.Rows[j]["OVERSHIP"].ToString()).ToString("f2") + "%";
                    break;
                }
            }
            string GoOrderQty = "0.00";
            for (int j = 0; j < StandardGoOrderQty.Rows.Count; j++)
            {
                String scNoStrTmp = StandardGoOrderQty.Rows[j]["sc_NO"].ToString();
                if (scNoStr.ToLower() == scNoStrTmp.ToLower())
                {
                    if (StandardGoOrderQty.Rows[j]["OrderQTY"].ToString() != "")
                    {
                        GoOrderQty = StandardGoOrderQty.Rows[j]["OrderQTY"].ToString();
                    }
                    break;
                }
            }
            for (int m = 0; m < StandardOrderQty.Rows.Count; m++)
            {
                String myscNoStrTmp = StandardOrderQty.Rows[m]["SC_NO"].ToString();
                if (myscNoStrTmp.ToLower() == scNoStr.ToLower())
                {
                    if (StandardOrderQty.Rows[m]["ORDER_QTY"].ToString() != "")
                    {
                        orderQty = StandardOrderQty.Rows[m]["ORDER_QTY"].ToString();
                    }
                    break;
                }
              
            }
            double sc_qty = 0;


            string scQtyString = GetStringValueByJoFromDataTable(ScQtyDataTable, poNoStr);
            sc_qty = scQtyString != "" ? double.Parse(scQtyString) : double.Parse(JoOrderQty);

            double OrderQty;
            if (sc_qty == 0)
                OrderQty = 0;
            else
                OrderQty = double.Parse(JoOrderQty) / sc_qty * double.Parse(orderQty);

            StandardMUReportList.Rows[i]["ORDER_QTY"] = OrderQty.ToString("f2");
            double rtw_qty_i = 0;
            double srn_qty_i = 0;
            double rtw_qty = 0;
            double srn_qty = 0;
            for (int j = 0; j < StandardRTW.Rows.Count; j++)
            {
                String poNoStrTmp = StandardRTW.Rows[j]["JO_NO"].ToString();
                if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                {
                    StandardMUReportList.Rows[i]["RTW_QTY"] = StandardRTW.Rows[j]["rtw_qty"].ToString() == "" ? double.Parse("0.00").ToString("f2") : double.Parse(StandardRTW.Rows[j]["rtw_qty"].ToString()).ToString("f2");
                    rtw_qty=double.Parse(StandardMUReportList.Rows[i]["RTW_QTY"].ToString());
                    rtw_qty_i = double.Parse(StandardRTW.Rows[j]["rtw_qty_i"].ToString() == "" ? double.Parse("0.00").ToString("f2") : double.Parse(StandardRTW.Rows[j]["rtw_qty_i"].ToString()).ToString("f2"));
                    break;
                }
            }

            for (int j = 0; j < StandardSRN.Rows.Count; j++)
            {
                String poNoStrTmp = StandardSRN.Rows[j]["jo_no"].ToString();
                if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                {
                    StandardMUReportList.Rows[i]["SRNQty"] = StandardSRN.Rows[j]["srn_qty"].ToString() == "" ? double.Parse("0.00").ToString("f2") : double.Parse(StandardSRN.Rows[j]["srn_qty"].ToString()).ToString("f2");
                    srn_qty = double.Parse(StandardMUReportList.Rows[i]["SRNQty"].ToString());
                    srn_qty_i = double.Parse(StandardSRN.Rows[j]["srn_qty_i"].ToString() == "" ? double.Parse("0.00").ToString("f2") : double.Parse(StandardSRN.Rows[j]["srn_qty_i"].ToString()).ToString("f2"));
                    break;
                }
            }
            StandardMUReportList.Rows[i]["Issued"] = srn_qty_i - rtw_qty_i;
            StandardMUReportList.Rows[i]["allocatedQty(nodiscount)"] = srn_qty - rtw_qty;

            String PPOList = "";
            double leftOverQty;
            double leftOverQtynodiscount;





            leftOverQty = 0;
            leftOverQtynodiscount = 0;
            if (StandardMUReportList.Rows[i]["PPO_NO"].ToString() != "")
            {
                string[] ppoArray = StandardMUReportList.Rows[i]["PPO_NO"].ToString().Split(';');
                for (int j = 0; j < ppoArray.Length; j++)
                {
                    PPOList += "'" + ppoArray[j] + "'" + ",";
                }
                PPOList = PPOList.Substring(0, PPOList.Length - 1);

            }


            if (StandardGisInfo.Select("JO_NO='" + poNoStr + "'")[0][4].ToString() == "Y" && PPOList != "")
            {

                double sc_qty1 = double.Parse(GetStringValueByJoFromDataTable(ScQtyDataTable, poNoStr));



                foreach (var r in leftOverTable.Select("PPO_NO IN(" + PPOList + ")"))
                {
                    leftOverQty += Convert.ToDouble(r["Leftover_QTY"]);
                    leftOverQtynodiscount += Convert.ToDouble(r["Leftover_QTY1"]);
                }

                double left_qty = leftOverQty;
                //Added By ZouShiChang ON 2014.02.13 Start
                //处理报表显示正无穷大的问题

                if (sc_qty1 * left_qty != 0)
                {
                    StandardMUReportList.Rows[i]["Leftover"] = (double.Parse(GoOrderQty) / sc_qty1 * left_qty).ToString("f2");
                    leftOverQtynodiscount = double.Parse(GoOrderQty) / sc_qty1 * leftOverQtynodiscount;
                }
                else
                {
                    StandardMUReportList.Rows[i]["Leftover"] = 0.0;
                    leftOverQtynodiscount = 0.0;
                }
                //Added By ZouShiChang ON 2014.02.13 End
            }
            else
            {
                StandardMUReportList.Rows[i]["Leftover"] = 0.00;
            }

            StandardMUReportList.Rows[i]["allocatedQty"] = (double.Parse(StandardMUReportList.Rows[i]["Leftover"].ToString() == "" ? "0.00" : StandardMUReportList.Rows[i]["Leftover"].ToString()) + double.Parse(StandardMUReportList.Rows[i]["Issued"].ToString() == "" ? "0.00" : StandardMUReportList.Rows[i]["Issued"].ToString())).ToString("f2");
            StandardMUReportList.Rows[i]["allocatedQty(nodiscount)"] = (leftOverQtynodiscount + StandardMUReportList.Rows[i]["allocatedQty(nodiscount)"].ToDouble()).ToString("f2");
            if (StandardMUReportList.Rows[i]["BULK_MKR_YPD"].ToString() != "")
            {
                StandardMUReportList.Rows[i]["ShipYardage"] = (double.Parse(StandardMUReportList.Rows[i]["SHIP_QTY"].ToString() == "" ? "0.00" : StandardMUReportList.Rows[i]["SHIP_QTY"].ToString()) * double.Parse(StandardMUReportList.Rows[i]["BULK_MKR_YPD"].ToString() == "" ? "0.00" : StandardMUReportList.Rows[i]["BULK_MKR_YPD"].ToString())).ToString("f2");
            }

            StandardMUReportList.Rows[i]["SewingDz"] = SEW_WASTAGE;

            StandardMUReportList.Rows[i]["WashingDz"] = WASH_WASTAGE;
            StandardMUReportList.Rows[i]["UnaccGmt"] = ((double.Parse(StandardMUReportList.Rows[i]["CUTQTY"].ToString() == "" ? "0" : StandardMUReportList.Rows[i]["CUTQTY"].ToString()) + double.Parse(StandardMUReportList.Rows[i]["PULLOUT_QTY"].ToString() == "" ? "0" : StandardMUReportList.Rows[i]["PULLOUT_QTY"].ToString())) - (double.Parse(StandardMUReportList.Rows[i]["SHIP_QTY"].ToString() == "" ? "0" : StandardMUReportList.Rows[i]["SHIP_QTY"].ToString()) + double.Parse(StandardMUReportList.Rows[i]["SAMPLE_QTY"].ToString() == "" ? "0" : StandardMUReportList.Rows[i]["SAMPLE_QTY"].ToString()) + double.Parse(StandardMUReportList.Rows[i]["GMT_QTY_TOTAL"].ToString() == "" ? "0" : StandardMUReportList.Rows[i]["GMT_QTY_TOTAL"].ToString()))).ToString("f2");
            for (int j = 0; j < StandardGivenCutAllowance.Rows.Count; j++)
            {
                String poNoStrTmp = StandardGivenCutAllowance.Rows[j]["JO_NO"].ToString();
                if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                {
                    StandardMUReportList.Rows[i]["GIVEN_CUT_YPD"] = ((double.Parse(StandardGivenCutAllowance.Rows[j]["Given_Cut_Allowance"].ToString() == "" ? "0" : StandardGivenCutAllowance.Rows[j]["Given_Cut_Allowance"].ToString()) + 1) * double.Parse(StandardMUReportList.Rows[i]["BULK_NET_YPD"].ToString() == "" ? "0" : StandardMUReportList.Rows[i]["BULK_NET_YPD"].ToString())).ToString("f2");
                }
            }
        }
        return StandardMUReportList;
    }
    /// <summary>
    /// 已弃用
    /// </summary>
    /// <param name="JoList"></param>
    /// <returns></returns>
    private void GetCombineJobTable(Dictionary<string, List<string>> CombineJo, DataTable dt)
    {
        bool iscombine = false;
        foreach (string key in CombineJo.Keys)
        {
            iscombine = false;
            foreach (string job in CombineJo[key])
            {
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    if (dt.Rows[i]["PO_NO"].ToString() == job)
                    {
                        if (!iscombine)
                        {
                            dt.Rows[i]["PO_NO"] = key;
                            iscombine = true;
                        }
                        else
                        {
                            dt.Rows.RemoveAt(i);
                        }
                        break;
                    }
                }
            }
        }
    }
    /// <summary>
    /// 已弃用
    /// </summary>
    /// <param name="JoList"></param>
    /// <returns></returns>
    private DataTable GetCombineFGISTable(Dictionary<string, List<string>> CombineJo, DataTable StandardFGIS)
    {

        DataTable dt = StandardFGIS.Clone();
        bool iscombine = false;
        DataRow _dr = null;
        foreach (string key in CombineJo.Keys)
        {
            iscombine = false;
            foreach (string job in CombineJo[key])
            {
                foreach (DataRow dr in StandardFGIS.Rows)
                {
                    if (dr["job_order_no"].ToString() == job)
                    {
                        if (!iscombine)
                        {
                            _dr = dt.NewRow();
                            _dr.ItemArray = dr.ItemArray;
                            _dr["job_order_no"] = key;
                            dt.Rows.Add(_dr);
                            iscombine = true;
                        }
                        else
                        {
                            //job_order_no, SUM(gmt_qty_a) AS gmt_qty_a,SUM(gmt_qty_b) AS gmt_qty_b,SUM(sew_qty_b) AS sew_qty_b,SUM(wash_qty_b) AS wash_qty_b
                            //_dr["gmt_qty_a"] = _dr["gmt_qty_a"].ToDouble() + dr["gmt_qty_a"].ToDouble();
                            //_dr["gmt_qty_b"] = _dr["gmt_qty_b"].ToDouble() + dr["gmt_qty_b"].ToDouble();
                            _dr["sew_qty_b"] = _dr["sew_qty_b"].ToDouble() + dr["sew_qty_b"].ToDouble();
                            _dr["wash_qty_b"] = _dr["wash_qty_b"].ToDouble() + dr["wash_qty_b"].ToDouble();
             
                        }
                    }
                }
                //for (int i = dt.Rows.Count - 1; i >= 0; i--)
                //{
                //    if (dt.Rows[i]["PO_NO"].ToString() == job)
                //    {
                //        if (!iscombine)
                //        {
                //            dt.Rows[i]["PO_NO"] = key;
                //            iscombine = true;
                //        }
                //        else
                //        {
                //            dt.Rows.RemoveAt(i);
                //        }
                //        break;
                //    }
                //}
            }
        }
        return dt;
    }
    /// <summary>
    /// 已弃用
    /// </summary>
    /// <param name="JoList"></param>
    /// <returns></returns>
    private DataTable GetCombineRTWSRNTable(Dictionary<string, List<string>> CombineJo, DataTable RTWSRN)
    {

        DataTable dt = RTWSRN.Clone();
        bool iscombine = false;
        DataRow _dr = null;
        foreach (string key in CombineJo.Keys)
        {
            iscombine = false;
            foreach (string job in CombineJo[key])
            {
                foreach (DataRow dr in RTWSRN.Rows)
                {
                    if (dr["jo_no"].ToString() == job)
                    {
                        if (!iscombine)
                        {
                            _dr = dt.NewRow();
                            _dr.ItemArray = dr.ItemArray;
                            _dr["jo_no"] = key;
                            dt.Rows.Add(_dr);
                            iscombine = true;
                        }
                        else
                        {
                            //job_order_no, SUM(gmt_qty_a) AS gmt_qty_a,SUM(gmt_qty_b) AS gmt_qty_b,SUM(sew_qty_b) AS sew_qty_b,SUM(wash_qty_b) AS wash_qty_b
                            _dr[1] = _dr[1].ToDouble() + dr[1].ToDouble();
                            _dr[2] = _dr[2].ToDouble() + dr[2].ToDouble();
                           

                        }
                    }
                }
               
            }
        }
        return dt;
    }
    /// <summary>
    /// 已弃用
    /// </summary>
    /// <param name="JoList"></param>
    /// <returns></returns>
    private DataTable GetCombineJoOrderTable(Dictionary<string, List<string>> CombineJo, DataTable JoOrder)
    {

        DataTable dt = JoOrder.Clone();
        bool iscombine = false;
        DataRow _dr = null;
        foreach (string key in CombineJo.Keys)
        {
            iscombine = false;
            foreach (string job in CombineJo[key])
            {
                foreach (DataRow dr in JoOrder.Rows)
                {
                    if (dr[0].ToString() == job)
                    {
                        if (!iscombine)
                        {
                            _dr = dt.NewRow();
                            _dr.ItemArray = dr.ItemArray;
                            _dr[0] = key;
                            dt.Rows.Add(_dr);
                            iscombine = true;
                        }
                        else
                        {
                            //job_order_no, SUM(gmt_qty_a) AS gmt_qty_a,SUM(gmt_qty_b) AS gmt_qty_b,SUM(sew_qty_b) AS sew_qty_b,SUM(wash_qty_b) AS wash_qty_b
                            _dr[1] = _dr[1].ToDouble() + dr[1].ToDouble();
                            


                        }
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
     
        DataTable dt = Originaldt.Copy();
        if (CombineJo.Count == 0)
            return dt;
        for (int i = 1; i < dt.Columns.Count; i++)
        {
            dt.Columns[i].ReadOnly = false;
        }
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
        dt.Columns["PERCENT_SHORT_ALLOWED"].ReadOnly = false;
        if (CombineJo.Count == 0)
            return dt;
        bool iscombine = false;
        DataRow _dr = null;
        double overqty = 0;
        double shortqty = 0;
        double total = 0;
        foreach (string key in CombineJo.Keys)
        {
            iscombine = false;
            overqty = 0;
            shortqty = 0;
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
                    overqty += selectrows[0]["total_qty"].ToDouble() * (selectrows[0]["PERCENT_OVER_ALLOWED"].ToDouble() - 100) / 100;
                    shortqty += selectrows[0]["total_qty"].ToDouble() * (100-selectrows[0]["PERCENT_SHORT_ALLOWED"].ToDouble()) / 100;
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
            {
                _dr["PERCENT_OVER_ALLOWED"] = (overqty / total + 1) * 100;
                _dr["PERCENT_SHORT_ALLOWED"] = (1-shortqty / total) * 100;
            }
        }
        return dt;
    }
    /// <summary>
    /// add by ming 因为Leftover_A和Leftover_B取自MES，所以抽出来，然后合并到SRN
    /// </summary>
    /// <param name="srndt"></param>
    /// <param name="leftoverAB"></param>
    private void CombineLeftoverAB(DataTable srndt, DataTable leftoverAB)
    {
        srndt.Columns.Add("gmt_qty_a");
        srndt.Columns.Add("gmt_qty_b");
        foreach (DataRow dr in leftoverAB.Rows)
        {
            DataRow[] selectrows = srndt.Select("JO_NO='" + dr["JO_NO"].ToString() + "'");
            if (selectrows.Count() == 0)
            {
                DataRow newtr = srndt.NewRow();
                newtr["JO_NO"] = dr["JO_NO"];
                newtr["gmt_qty_a"] = dr["gmt_qty_a"];
                newtr["gmt_qty_b"] = dr["gmt_qty_b"];
                srndt.Rows.Add(newtr);
            }
            else
            {
                selectrows[0]["gmt_qty_a"] = selectrows[0]["gmt_qty_a"].ToDouble() + dr["gmt_qty_a"].ToDouble();
                selectrows[0]["gmt_qty_b"] = selectrows[0]["gmt_qty_b"].ToDouble() + dr["gmt_qty_b"].ToDouble();
            }
        }
    }
    /// <summary>
    /// 已弃用
    /// </summary>
    /// <param name="JoList"></param>
    /// <returns></returns>
    private string GetStringValueByJoFromDataTable(DataTable table, string jo)
    {
        string value = "0.00";

        if (table.Rows.Count < 1) return value;
        DataRow[] rows = table.Select("JOB_ORDER_NO='" + jo + "'");
        if (rows.Length < 1) return value;
        if (rows[0]["VALUE"].ToString() != "")
        {
            value = Convert.ToDouble(rows[0]["VALUE"]).ToString("f2");
        }
        return value;
    }

    /// <summary>
    /// 合并GIS信息，SHIP_DATE取最早日期
    /// </summary>
    /// <param name="CombineJo"></param>
    /// <param name="Originaldt"></param>
    /// <returns></returns>
    public DataTable GetCombineGISTable(Dictionary<string, List<string>> CombineJo, DataTable Originaldt)
    {

        DataTable dt = Originaldt.Copy();
        dt.Columns["SC_NO"].ReadOnly = false;
        dt.Columns["JO_NO"].ReadOnly = false;
        dt.Columns["SHIP_DATE"].ReadOnly = false;
        dt.Columns["SHIP_QTY"].ReadOnly = false;
        dt.Columns["LASTJO"].ReadOnly = false;
        if (CombineJo.Count == 0)
            return dt;
        bool iscombine = false;
        DataRow _dr = null;

        foreach (string key in CombineJo.Keys)
        {
            iscombine = false;
            DataRow[] cbrows = dt.Select("JO_NO='" + key + "'");//是否已存在CB单
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
                            _dr["JO_NO"] = key;
                        }
                        else
                        {
                            _dr = cbrows[0];
                            if (_dr["SHIP_DATE"].ToDate() < selectrows[0]["SHIP_DATE"].ToDate())
                            {
                                _dr["SHIP_DATE"] = selectrows[0]["SHIP_DATE"];
                            }
                            _dr["SHIP_QTY"] = _dr["SHIP_QTY"].ToDouble() + selectrows[0]["SHIP_QTY"].ToDouble();

                           
                            dt.Rows.Remove(selectrows[0]);
                        }
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

    protected void gvDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();
                //First Row
                for (int i = 0; i < 11; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("rowspan", "3");
                    tcHeader[i].Text = strHeader[i];
                }

                tcHeader.Add(new TableHeaderCell());
                tcHeader[11].Attributes.Add("colspan", "11");
                tcHeader[11].Text = strHeader[11];

                tcHeader.Add(new TableHeaderCell());
                tcHeader[12].Attributes.Add("colspan", "14");
                tcHeader[12].Text = strHeader[12];

                tcHeader.Add(new TableHeaderCell());
                tcHeader[13].Attributes.Add("colspan", "7");
                tcHeader[13].Text = strHeader[13];

                tcHeader.Add(new TableHeaderCell());
                tcHeader[14].Attributes.Add("colspan", "5");
                tcHeader[14].Text = strHeader[14];

                tcHeader.Add(new TableHeaderCell());
                tcHeader[15].Attributes.Add("rowspan", "3");
                tcHeader[15].Text = strHeader[15];

                tcHeader.Add(new TableHeaderCell());
                tcHeader[16].Attributes.Add("rowspan", "3");
                tcHeader[16].Text = strHeader[16];
                //Second Row
                for (int i = 17; i < 23; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("rowspan", "2");
                    tcHeader[i].Text = strHeader[i];
                }

                tcHeader.Add(new TableHeaderCell());
                tcHeader[23].Attributes.Add("colspan", "2");
                tcHeader[23].Text = strHeader[23];

                for (int i = 24; i < 32; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("rowspan", "2");
                    tcHeader[i].Text = strHeader[i];
                }

                tcHeader.Add(new TableHeaderCell());
                tcHeader[32].Attributes.Add("colspan", "4");
                tcHeader[32].Text = strHeader[32];

                for (int i = 33; i < 35; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("colspan", "2");
                    tcHeader[i].Text = strHeader[i];
                }

                for (int i = 35; i < 48; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("rowspan", "2");
                    tcHeader[i].Text = strHeader[i];
                }
                //Third Row
                for (int i = 48; i < 58; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Text = strHeader[i];
                }

                //bgcolor
                for (int i = 0; i < 58; i++)
                {
                    tcHeader[i].Attributes.Add("bgcolor", "#CCFFCC");
                }
                break;
            case DataControlRowType.DataRow:
                e.Row.Cells[5].Style.Add("word-break", "break-all");
                break;
            case DataControlRowType.Footer:
                DataTable Summarydata = MESComment.MUReportSql.GetStandardMUReportSummary(ddlFtyCd.SelectedItem.Value, strRunNO);
                if (Summarydata.Rows.Count == 1)
                {
                    e.Row.Cells[0].Text = "Total/Weighted Average";
                    //e.Row.Cells[0].ColumnSpan = 2;
                    e.Row.Cells[10].Text = Summarydata.Rows[0]["MU"].ToString();
                    e.Row.Cells[11].Text = Summarydata.Rows[0]["ORDER_QTY"].ToString();
                    e.Row.Cells[12].Text = Summarydata.Rows[0]["allocatedQty"].ToString();
                    e.Row.Cells[13].Text = Summarydata.Rows[0]["allocatedQty(nodiscount)"].ToString();
                    e.Row.Cells[14].Text = Summarydata.Rows[0]["Issued"].ToString();
                    e.Row.Cells[15].Text = Summarydata.Rows[0]["MA"].ToString();
                    e.Row.Cells[16].Text = Summarydata.Rows[0]["ShipYardage"].ToString();
                    e.Row.Cells[17].Text = Summarydata.Rows[0]["CutWastageYPD"].ToString();
                    e.Row.Cells[18].Text = Summarydata.Rows[0]["CutWastageYPDPer"].ToString();
                    //e.Row.Cells[19].Text = Summarydata.Rows[0]["DEFECT_FAB"].ToString();
                    //e.Row.Cells[20].Text = Summarydata.Rows[0]["DEFECT_PANELS"].ToString();
                    //e.Row.Cells[21].Text = Summarydata.Rows[0]["ODD_LOSS"].ToString();
                    //e.Row.Cells[22].Text = Summarydata.Rows[0]["SPLICE_LOSS"].ToString();
                    //e.Row.Cells[23].Text = Summarydata.Rows[0]["CUT_REJ_PANELS"].ToString();
                    //e.Row.Cells[24].Text = Summarydata.Rows[0]["MATCH_LOSS"].ToString();
                    //e.Row.Cells[25].Text = Summarydata.Rows[0]["END_LOSS"].ToString();
                    //e.Row.Cells[26].Text = Summarydata.Rows[0]["SHORT_YDS"].ToString();
                    //e.Row.Cells[27].Text = Summarydata.Rows[0]["SEW_MATCH_LOSS"].ToString();
                    e.Row.Cells[19].Text = Summarydata.Rows[0]["Leftover"].ToString();
                    //e.Row.Cells[31].Text = Summarydata.Rows[0]["REMNANT"].ToString();
                    e.Row.Cells[20].Text = Summarydata.Rows[0]["SRNQty"].ToString();
                    e.Row.Cells[21].Text = Summarydata.Rows[0]["RTW_QTY"].ToString();
                    e.Row.Cells[22].Text = Summarydata.Rows[0]["ORDERQTY"].ToString();
                    e.Row.Cells[23].Text = Summarydata.Rows[0]["CUTQTY"].ToString();
                    e.Row.Cells[24].Text = Summarydata.Rows[0]["SHIP_QTY"].ToString();
                    e.Row.Cells[25].Text = Summarydata.Rows[0]["SAMPLE_QTY"].ToString();
                    e.Row.Cells[26].Text = Summarydata.Rows[0]["PULLOUT_QTY"].ToString();
                    e.Row.Cells[27].Text = Summarydata.Rows[0]["GMT_QTY_A"].ToString();
                    e.Row.Cells[28].Text = Summarydata.Rows[0]["GMT_QTY_B"].ToString();
                    e.Row.Cells[29].Text = Summarydata.Rows[0]["DISCREPANCY_QTY"].ToString();
                    e.Row.Cells[30].Text = Summarydata.Rows[0]["GMT_QTY_TOTAL"].ToString();
                    e.Row.Cells[31].Text = Summarydata.Rows[0]["SewingDz"].ToString();
                    e.Row.Cells[32].Text = Summarydata.Rows[0]["SewingPercent"].ToString();
                    e.Row.Cells[33].Text = Summarydata.Rows[0]["WashingDz"].ToString();
                    e.Row.Cells[34].Text = Summarydata.Rows[0]["WashingPercent"].ToString();
                    e.Row.Cells[35].Text = Summarydata.Rows[0]["UnaccGmt"].ToString();
                    e.Row.Cells[36].Text = Summarydata.Rows[0]["PPO_YPD"].ToString();
                    e.Row.Cells[37].Text = Summarydata.Rows[0]["BULK_NET_YPD"].ToString();
                    e.Row.Cells[38].Text = Summarydata.Rows[0]["GIVEN_CUT_YPD"].ToString();
                    e.Row.Cells[39].Text = Summarydata.Rows[0]["BULK_MKR_YPD"].ToString();
                    e.Row.Cells[40].Text = Summarydata.Rows[0]["YPD_Var"].ToString();
                    e.Row.Cells[41].Text = Summarydata.Rows[0]["cutYPD"].ToString();
                    e.Row.Cells[42].Text = Summarydata.Rows[0]["ShipYPD"].ToString();
                    e.Row.Cells[43].Text = Summarydata.Rows[0]["ShipToCut"].ToString();
                    e.Row.Cells[44].Text = Summarydata.Rows[0]["ShipToRecv"].ToString();
                    e.Row.Cells[45].Text = Summarydata.Rows[0]["ShipToOrder"].ToString();
                    e.Row.Cells[46].Text = Summarydata.Rows[0]["cut_to_receipt"].ToString();
                    e.Row.Cells[47].Text = Summarydata.Rows[0]["cut_to_order"].ToString();
                    e.Row.Cells[48].Text = Summarydata.Rows[0]["OVERSHIP"].ToString();
                    e.Row.Cells[49].Text = Summarydata.Rows[0]["SHORTSHIP"].ToString();
                    e.Row.Font.Bold = true;
                }
                for (int cells = 0; cells < e.Row.Cells.Count; cells++)
                {
                    e.Row.Cells[cells].Attributes.Add("bgcolor", "#F7F6F3");
                }
                break;
        }
    }
    /// <summary>
    /// 已弃用
    /// </summary>
    /// <param name="JoList"></param>
    /// <returns></returns>
    private DataTable GenerateDetail(DataTable StandardMUReportList)
    {

        StandardMUReportList.Columns.Add("CutWastageYPD");
        StandardMUReportList.Columns.Add("CutWastageYPDPer");
        StandardMUReportList.Columns.Add("SewingPercent");
        StandardMUReportList.Columns.Add("WashingPercent");
        StandardMUReportList.Columns.Add("YPD_Var");
        StandardMUReportList.Columns.Add("cutYPD");
        StandardMUReportList.Columns.Add("ShipYPD");
        StandardMUReportList.Columns.Add("ShipToCut");
        StandardMUReportList.Columns.Add("cut_to_receipt");
        StandardMUReportList.Columns.Add("ShipToRecv");
        StandardMUReportList.Columns.Add("cut_to_order");
        StandardMUReportList.Columns.Add("ShipToOrder");
        for (int i = 0; i < StandardMUReportList.Rows.Count; i++)
        {
            double issueQty = 0;

            double CutWastageYPD = 0;
            double CutWastageYPDPer = 0;
            double MAQty = 0;
            double RemnantQty = 0;
            double allocatedQty = 0;
            double allocatedQty1 = 0;
            double PPOMkrYpd = 0;
            double BulkMkrYpd = 0;
            double cutQty = 0;
            double cutYPD = 0;
            double giveCutYpd = 0;

            double orderQty = 0;
            double cut_to_order = 0;
            double cut_to_receipt = 0;

            double ShippedQty = 0;
            double sampleQty = 0;
            string ShipYPD = "";
            double ShipToCut = 0;
            double ShipToRecv = 0;
            double ShipToOrder = 0;

            double PullOutQty = 0;
            double SewingDz = 0;
            double WashingDz = 0;
            double SewingPercent = 0;
            double WashingPercent = 0;
            if (StandardMUReportList.Rows[i]["Issued"].ToString() != "")
            {
                issueQty = double.Parse(StandardMUReportList.Rows[i]["Issued"].ToString());
            }
            if (StandardMUReportList.Rows[i]["MA"].ToString() != "")
            {
                MAQty = double.Parse(StandardMUReportList.Rows[i]["MA"].ToString());
            }
            if (StandardMUReportList.Rows[i]["REMNANT"].ToString() != "")
            {
                RemnantQty = double.Parse(StandardMUReportList.Rows[i]["REMNANT"].ToString());
            }
            if (StandardMUReportList.Rows[i]["PULLOUT_QTY"].ToString() != "")
            {
                PullOutQty = double.Parse(StandardMUReportList.Rows[i]["PULLOUT_QTY"].ToString());
            }
            if (StandardMUReportList.Rows[i]["SAMPLE_QTY"].ToString() != "")
            {
                sampleQty = double.Parse(StandardMUReportList.Rows[i]["SAMPLE_QTY"].ToString());
            }
            if (StandardMUReportList.Rows[i]["PPO_YPD"].ToString() != "")
            {
                PPOMkrYpd = double.Parse(StandardMUReportList.Rows[i]["PPO_YPD"].ToString());
            }
            if (StandardMUReportList.Rows[i]["BULK_MKR_YPD"].ToString() != "")
            {
                BulkMkrYpd = double.Parse(StandardMUReportList.Rows[i]["BULK_MKR_YPD"].ToString());
            }
            if (StandardMUReportList.Rows[i]["GIVEN_CUT_YPD"].ToString() != "")
            {
                giveCutYpd = double.Parse(StandardMUReportList.Rows[i]["GIVEN_CUT_YPD"].ToString());
            }
            if (StandardMUReportList.Rows[i]["allocatedQty"].ToString() != "")
            {
                allocatedQty = double.Parse(StandardMUReportList.Rows[i]["allocatedQty"].ToString());
            }
            if (StandardMUReportList.Rows[i]["allocatedQty(nodiscount)"].ToString() != "")
            {
                allocatedQty1 = StandardMUReportList.Rows[i]["allocatedQty(nodiscount)"].ToDouble();
            }
            CutWastageYPD = issueQty - MAQty - RemnantQty;
            SewingDz = StandardMUReportList.Rows[i]["SewingDz"].ToString() == "" ? 0 : double.Parse(StandardMUReportList.Rows[i]["SewingDz"].ToString());
            WashingDz = StandardMUReportList.Rows[i]["WashingDz"].ToString() == "" ? 0 : double.Parse(StandardMUReportList.Rows[i]["WashingDz"].ToString());
            if (StandardMUReportList.Rows[i]["SHIP_QTY"].ToString() != "")
            {
                ShippedQty = double.Parse(StandardMUReportList.Rows[i]["SHIP_QTY"].ToString());
                if ((ShippedQty - PullOutQty) != 0)
                {
                    ShipYPD = ((issueQty - RemnantQty) / (ShippedQty - PullOutQty)).ToString("f2");
                }
                else
                {
                    ShipYPD = "";
                }
            }
            if (StandardMUReportList.Rows[i]["CUTQTY"].ToString() != "")
            {
                cutQty = double.Parse(StandardMUReportList.Rows[i]["CUTQTY"].ToString());
                if (cutQty != 0)
                {
                    cutYPD = (issueQty - RemnantQty) / cutQty;
                    ShipToCut = (ShippedQty + sampleQty - PullOutQty) / cutQty * 100;
                    SewingPercent = SewingDz / cutQty;
                    WashingPercent = WashingDz / cutQty;

                }
            }
            if (StandardMUReportList.Rows[i]["ORDERQTY"].ToString() != "")
            {
                orderQty = double.Parse(StandardMUReportList.Rows[i]["ORDERQTY"].ToString());
                if (orderQty != 0)
                {
                    cut_to_order = cutQty / orderQty * 100;
                    ShipToOrder = ShippedQty / orderQty * 100;
                }
            }
            if (issueQty - RemnantQty != 0)
            {
                CutWastageYPDPer = CutWastageYPD / (issueQty - RemnantQty) * 100;
            }
            if (allocatedQty != 0)
            {
                cut_to_receipt = cutQty * giveCutYpd / allocatedQty * 100;
                ShipToRecv = (ShippedQty + sampleQty - PullOutQty) * BulkMkrYpd / allocatedQty * 100;
            }
            StandardMUReportList.Rows[i]["CutWastageYPD"] = CutWastageYPD.ToString("f2");
            StandardMUReportList.Rows[i]["CutWastageYPDPer"] = CutWastageYPDPer.ToString("f2") + "%";
            StandardMUReportList.Rows[i]["SewingPercent"] = SewingPercent.ToString("f2") + "%";
            StandardMUReportList.Rows[i]["WashingPercent"] = WashingPercent.ToString("f2") + "%";
            StandardMUReportList.Rows[i]["YPD_Var"] = (BulkMkrYpd - PPOMkrYpd).ToString("f2");
            StandardMUReportList.Rows[i]["cutYPD"] = cutYPD.ToString("f2");
            StandardMUReportList.Rows[i]["ShipYPD"] = ShipYPD;
            StandardMUReportList.Rows[i]["ShipToCut"] = ShipToCut.ToString("f2") + "%";
            StandardMUReportList.Rows[i]["cut_to_receipt"] = cut_to_receipt.ToString("f2") + "%";
            StandardMUReportList.Rows[i]["ShipToRecv"] = ShipToRecv.ToString("f2") + "%";
            StandardMUReportList.Rows[i]["cut_to_order"] = cut_to_order.ToString("f2") + "%";
            StandardMUReportList.Rows[i]["ShipToOrder"] = ShipToOrder.ToString("f2") + "%";

            if (ShipToOrder < 95)
            {
                ship_order_count_95++;
            }
            else if (ShipToOrder < 100)
            {
                ship_order_count_100++;
                ship_order_sum += double.Parse(StandardMUReportList.Rows[i]["SHIP_QTY"].ToString() == "" ? "0" : StandardMUReportList.Rows[i]["SHIP_QTY"].ToString()) + double.Parse(StandardMUReportList.Rows[i]["SAMPLE_QTY"].ToString() == "" ? "0" : StandardMUReportList.Rows[i]["SAMPLE_QTY"].ToString());
            }

            if (BulkMkrYpd - PPOMkrYpd > 0.25)
            {
                YPD_Var_big++;
            }
            else if (BulkMkrYpd - PPOMkrYpd < -0.25)
            {
                YPD_Var_small++;
            }
        }
        DataRow row = StandardMUReportList.NewRow();
        row["PO_NO"] = "";
        StandardMUReportList.Rows.Add(row);
        finishProgress();
        return StandardMUReportList;
       
    }


    /// <summary>
    /// 已弃用
    /// </summary>
    /// <param name="JoList"></param>
    /// <returns></returns>
    private void GenerateTotalRow()
    {
        double Total_MU = 0;
        double Total_PPO_order_yds = 0;
        //double Total_Allocated = 0;//开放数据

        double Total_Issued = 0;
        double Total_MA = 0;
        //double Total_Ship_Yardage = 0;//开放数据

        double Total_Yds = 0;
        //double Total_Leftover = 0;//开放数据

        double Total_Remnant = 0;
        double Total_SRN = 0;
        double Total_RTW = 0;
        double Total_Order = 0;
        double Total_Cut = 0;
        double Total_Shipped = 0;
        double Total_Sample = 0;
        double Total_Pull_out = 0;
        double Total_Grade_A = 0;
        double Total_Grade_B = 0;
        double Total_Grade_C = 0;
        double Total_Grade_total = 0;
        double Total_Sewing_DZ = 0;
        double Total_Washing_DZ = 0;
        double Total_Unacc_Gmt = 0;
        double Total_PPO_MKR_YPD = 0;
        double Total_BULK_NET_YPD = 0;
        double Total_GIVEN_CUT_YPD = 0;
        double Total_BULK_MKR_YPD = 0;
        double Total_YPD_Var = 0;
        double Total_Ship_to_Receipt = 0;
        double Total_Cut_to_Receipt = 0;
        double Total_Over_Ship = 0;

        double Issue_Remant = 0;


        String mu = "";
        //int JO_NO = 0, MU = 10, ORDER_QTY = 11, allocatedQty = 12, Issued = 14, MA = 15, ShipYardage = 16, CutWastageYPD = 17, CutWastageYPDPer = 18, Leftover = 19, REMNANT = 19, SRNQty = 20, RTW_QTY = 21, ORDERQTY = 22, CUTQTY = 23, SHIP_QTY = 24, SAMPLE_QTY = 25;
        int JO_NO = 0, MU = 10, ORDER_QTY = 11, allocatedQty = 12, allocatedQty1 = 13, Issued = 14, MA = 15, ShipYardage = 16, CutWastageYPD = 17, CutWastageYPDPer = 18, Leftover = 19, SRNQty = 20, RTW_QTY = 21, ORDERQTY = 22, CUTQTY = 23, SHIP_QTY = 24, SAMPLE_QTY = 25;
        int PULLOUT_QTY = 26, GMT_QTY_A = 27, GMT_QTY_B = 28, DISCREPANCY_QTY = 29, GMT_QTY_TOTAL = 30, SewingDz = 31, SewingPercent = 32, WashingDz = 33, WashingPercent = 34, UnaccGmt = 35, PPO_YPD = 36, BULK_NET_YPD = 37, GIVEN_CUT_YPD = 38, BULK_MKR_YPD = 39;
        int YPD_Var = 40, cutYPD = 41, ShipYPD = 42, ShipToCut = 43, ShipToRecv = 44, ShipToOrder = 45, cut_to_receipt = 46, cut_to_order = 47, OVERSHIP = 48;
        double[,] FabricPatternValue = { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };
        for (int i = 0; i < gvDetail.Rows.Count; i++)
        {
            if (gvDetail.Rows[i].Cells[MU].Text != "&nbsp;")
            {
                mu = gvDetail.Rows[i].Cells[MU].Text.Substring(0, gvDetail.Rows[i].Cells[MU].Text.Length - 1);
            }
            Total_MU += double.Parse(mu == "" ? "0" : mu) * double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
            Total_PPO_order_yds += double.Parse(gvDetail.Rows[i].Cells[ORDER_QTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDER_QTY].Text);
            Total_Allocated += double.Parse(gvDetail.Rows[i].Cells[allocatedQty].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[allocatedQty].Text);
            Total_Allocated1 += gvDetail.Rows[i].Cells[allocatedQty1].Text.ToDouble();
            Total_Issued += double.Parse(gvDetail.Rows[i].Cells[Issued].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[Issued].Text);
            Total_MA += double.Parse(gvDetail.Rows[i].Cells[MA].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[MA].Text);
            Total_Ship_Yardage += double.Parse(gvDetail.Rows[i].Cells[ShipYardage].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ShipYardage].Text);
            Total_Yds += double.Parse(gvDetail.Rows[i].Cells[CutWastageYPD].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[CutWastageYPD].Text);
            //Issue_Remant += double.Parse(gvDetail.Rows[i].Cells[Issued].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[Issued].Text) - double.Parse(gvDetail.Rows[i].Cells[REMNANT].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[REMNANT].Text);
            Issue_Remant += double.Parse(gvDetail.Rows[i].Cells[Issued].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[Issued].Text);
            Total_Leftover += double.Parse(gvDetail.Rows[i].Cells[Leftover].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[Leftover].Text);
            //Total_Remnant += gvDetail.Rows[i].Cells[REMNANT].ToDouble(); //double.Parse(gvDetail.Rows[i].Cells[REMNANT].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[REMNANT].Text);
            Total_SRN += double.Parse(gvDetail.Rows[i].Cells[SRNQty].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[SRNQty].Text);
            Total_RTW += double.Parse(gvDetail.Rows[i].Cells[RTW_QTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[RTW_QTY].Text);
            Total_Order += double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
            Total_Cut += double.Parse(gvDetail.Rows[i].Cells[CUTQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[CUTQTY].Text);
            Total_Shipped += double.Parse(gvDetail.Rows[i].Cells[SHIP_QTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[SHIP_QTY].Text);
            Total_Sample += double.Parse(gvDetail.Rows[i].Cells[SAMPLE_QTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[SAMPLE_QTY].Text);
            Total_Pull_out += double.Parse(gvDetail.Rows[i].Cells[PULLOUT_QTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[PULLOUT_QTY].Text);
            Total_Grade_A += double.Parse(gvDetail.Rows[i].Cells[GMT_QTY_A].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[GMT_QTY_A].Text);
            Total_Grade_B += double.Parse(gvDetail.Rows[i].Cells[GMT_QTY_B].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[GMT_QTY_B].Text);
            Total_Grade_C += double.Parse(gvDetail.Rows[i].Cells[DISCREPANCY_QTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[DISCREPANCY_QTY].Text);
            Total_Grade_total += double.Parse(gvDetail.Rows[i].Cells[GMT_QTY_TOTAL].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[GMT_QTY_TOTAL].Text);
            Total_Sewing_DZ += double.Parse(gvDetail.Rows[i].Cells[SewingDz].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[SewingDz].Text);
            Total_Washing_DZ += double.Parse(gvDetail.Rows[i].Cells[WashingDz].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[WashingDz].Text);
            Total_Unacc_Gmt += double.Parse(gvDetail.Rows[i].Cells[UnaccGmt].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[UnaccGmt].Text);
            Total_PPO_MKR_YPD += double.Parse(gvDetail.Rows[i].Cells[PPO_YPD].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[PPO_YPD].Text) * double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
            Total_BULK_NET_YPD += double.Parse(gvDetail.Rows[i].Cells[BULK_NET_YPD].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[BULK_NET_YPD].Text) * double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
            Total_GIVEN_CUT_YPD += double.Parse(gvDetail.Rows[i].Cells[GIVEN_CUT_YPD].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[GIVEN_CUT_YPD].Text) * double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
            Total_BULK_MKR_YPD += double.Parse(gvDetail.Rows[i].Cells[BULK_MKR_YPD].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[BULK_MKR_YPD].Text) * double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
            Total_YPD_Var += double.Parse(gvDetail.Rows[i].Cells[BULK_MKR_YPD].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[BULK_MKR_YPD].Text) - double.Parse(gvDetail.Rows[i].Cells[PPO_YPD].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[PPO_YPD].Text);
            Total_Ship_to_Receipt += (double.Parse(gvDetail.Rows[i].Cells[SHIP_QTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[SHIP_QTY].Text) + double.Parse(gvDetail.Rows[i].Cells[SAMPLE_QTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[SAMPLE_QTY].Text) - double.Parse(gvDetail.Rows[i].Cells[PULLOUT_QTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[PULLOUT_QTY].Text)) * double.Parse(gvDetail.Rows[i].Cells[BULK_MKR_YPD].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[BULK_MKR_YPD].Text);
            Total_Cut_to_Receipt += double.Parse(gvDetail.Rows[i].Cells[CUTQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[CUTQTY].Text) * double.Parse(gvDetail.Rows[i].Cells[GIVEN_CUT_YPD].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[GIVEN_CUT_YPD].Text);
            Total_Over_Ship += double.Parse(gvDetail.Rows[i].Cells[OVERSHIP].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[OVERSHIP].Text.Substring(0, gvDetail.Rows[i].Cells[OVERSHIP].Text.Length - 1)) * double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);


            switch (gvDetail.Rows[i].Cells[9].Text)
            {
                case "Check":
                    FabricPatternValue[0, 0] += double.Parse(mu == "" ? "0" : mu) * double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
                    FabricPatternValue[0, 1] += double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
                    break;
                case "Solid":
                    FabricPatternValue[1, 0] += double.Parse(mu == "" ? "0" : mu) * double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
                    FabricPatternValue[1, 1] += double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
                    break;
                case "Stripe":
                    FabricPatternValue[2, 0] += double.Parse(mu == "" ? "0" : mu) * double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
                    FabricPatternValue[2, 1] += double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
                    break;
                case "Check;Solid":
                case "Solid;Check":
                    FabricPatternValue[3, 0] += double.Parse(mu == "" ? "0" : mu) * double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
                    FabricPatternValue[3, 1] += double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
                    break;
                case "Check:Stripe":
                case "Stripe:Check":
                    FabricPatternValue[4, 0] += double.Parse(mu == "" ? "0" : mu) * double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
                    FabricPatternValue[4, 1] += double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
                    break;
                case "Solid:Stripe":
                case "Stripe:Solid":
                    FabricPatternValue[5, 0] += double.Parse(mu == "" ? "0" : mu) * double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
                    FabricPatternValue[5, 1] += double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
                    break;
                case "Check;Solid;Stripe":
                case "Check;Stripe;Solid":
                case "Solid;Check;Stripe":
                case "Solid;Stripe;Check":
                case "Stripe;Solid;Check":
                case "Stripe;Check;Solid":
                    FabricPatternValue[6, 0] += double.Parse(mu == "" ? "0" : mu) * double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
                    FabricPatternValue[6, 1] += double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
                    break;
            }
        }

        for (int i = 0; i < 7; i++)
        {
            if (FabricPatternValue[i, 1] != 0)
            {
                FabricPattern += (FabricPatternValue[i, 0] / FabricPatternValue[i, 1]).ToString("0.00") + "%,";
            }
            else
            {
                FabricPattern += "0.00%,";
            }
        }

        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[JO_NO].Text = "";
        //if (Total_Order != 0)
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[MU - 1].Text = (Total_MU / Total_Order).ToString("f2") + "%";
        //}
        //else
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[MU - 1].Text = "0.00%";
        //}
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ORDER_QTY - 1].Text = Total_PPO_order_yds.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[allocatedQty - 1].Text = Total_Allocated.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[allocatedQty1 - 1].Text = Total_Allocated1.ToString("f2");

        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[Issued - 1].Text = Total_Issued.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[MA - 1].Text = Total_MA.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipYardage - 1].Text = Total_Ship_Yardage.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[CutWastageYPD - 1].Text = Total_Yds.ToString("f2");
        //if (Issue_Remant != 0)
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[CutWastageYPDPer - 1].Text = (Total_Yds / Issue_Remant * 100).ToString("f2") + "%";
        //}
        //else
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[CutWastageYPDPer - 1].Text = "0.00%";
        //}
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[Leftover - 1].Text = Total_Leftover.ToString("f2");
        ////gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[REMNANT - 1].Text = Total_Remnant.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[SRNQty - 1].Text = Total_SRN.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[RTW_QTY - 1].Text = Total_RTW.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ORDERQTY - 1].Text = Total_Order.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[CUTQTY - 1].Text = Total_Cut.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[SHIP_QTY - 1].Text = Total_Shipped.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[SAMPLE_QTY - 1].Text = Total_Sample.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[PULLOUT_QTY - 1].Text = Total_Pull_out.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[GMT_QTY_A - 1].Text = Total_Grade_A.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[GMT_QTY_B - 1].Text = Total_Grade_B.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[DISCREPANCY_QTY - 1].Text = Total_Grade_C.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[GMT_QTY_TOTAL - 1].Text = Total_Grade_total.ToString("f2");
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[SewingDz - 1].Text = Total_Sewing_DZ.ToString("f2");
        //if (Total_Cut != 0)
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[SewingPercent - 1].Text = (Total_Sewing_DZ / Total_Cut).ToString("f2") + "%";
        //}
        //else
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[SewingPercent - 1].Text = "0.00%";
        //}
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[WashingDz - 1].Text = Total_Washing_DZ.ToString("f2");
        //if (Total_Cut != 0)
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[WashingPercent - 1].Text = (Total_Washing_DZ / Total_Cut).ToString("f2") + "%";
        //}
        //else
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[WashingPercent - 1].Text = "0.00%";
        //}
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[UnaccGmt - 1].Text = Total_Unacc_Gmt.ToString("f2");
        //if (Total_Order != 0)
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[PPO_YPD - 1].Text = (Total_PPO_MKR_YPD / Total_Order).ToString("f2");
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[BULK_NET_YPD - 1].Text = (Total_BULK_NET_YPD / Total_Order).ToString("f2");
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[GIVEN_CUT_YPD - 1].Text = (Total_GIVEN_CUT_YPD / Total_Order).ToString("f2");
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[BULK_MKR_YPD - 1].Text = (Total_BULK_MKR_YPD / Total_Order).ToString("f2");
        //}
        //else
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[PPO_YPD - 1].Text = "0.00%";
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[BULK_NET_YPD - 1].Text = "0.00%";
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[GIVEN_CUT_YPD - 1].Text = "0.00%";
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[BULK_MKR_YPD - 1].Text = "0.00%";
        //}

        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[YPD_Var - 1].Text = Total_YPD_Var.ToString("f2");
        //if (Total_Cut != 0)
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[cutYPD - 1].Text = (Issue_Remant / Total_Cut).ToString("f2");
        //}
        //else
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[cutYPD - 1].Text = "0.00%";
        //}
        //if (Total_Shipped - Total_Pull_out != 0)
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipYPD - 1].Text = (Issue_Remant / (Total_Shipped - Total_Pull_out)).ToString("f2");
        //}
        //else
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipYPD - 1].Text = "0.00%";
        //}
        //if (Total_Cut != 0)
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipToCut - 1].Text = ((Total_Shipped + Total_Sample - Total_Pull_out) / Total_Cut * 100).ToString("f2") + "%";
        //}
        //else
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipToCut - 1].Text = "0.00%";
        //}
        //if (Total_Allocated != 0)
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipToRecv - 1].Text = (Total_Ship_to_Receipt / Total_Allocated * 100).ToString("f2") + "%";
        //}
        //else
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipToRecv - 1].Text = "0.00%";
        //}
        //if (Total_Order != 0)
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipToOrder - 1].Text = (Total_Shipped / Total_Order * 100).ToString("f2") + "%";
        //}
        //else
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipToOrder - 1].Text = "0.00%";
        //}

        //if (Total_Allocated != 0)
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[cut_to_receipt - 1].Text = (Total_Cut_to_Receipt / Total_Allocated * 100).ToString("f2") + "%";
        //}
        //else
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[cut_to_receipt - 1].Text = "0.00%";
        //}
        //if (Total_Order != 0)
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[cut_to_order - 1].Text = (Total_Cut / Total_Order * 100).ToString("f2") + "%";
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[OVERSHIP - 1].Text = (Total_Over_Ship / Total_Order).ToString("f2") + "%";
        //}
        //else
        //{
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[cut_to_order - 1].Text = "0.00%";
        //    gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[OVERSHIP - 1].Text = "0.00%";
        //}
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[48].Text = "";
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[0].ColumnSpan = 2;
        //gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[0].Text = "<b>Total/Weighted Average</b>";
    }

    private void GenerateSummary()
    {
        if (StandardMUReportList.Rows.Count <= 0)
            return;
        double shipqty = 0;
        double cutqty = 0;
        double sampleqty = 0;
        double pulloutqty = 0;
        double bulkmkrypd = 0;
        double allocatedqty = 0;
        double orderqty = 0;
        double cutwastageypd = 0;
        double issueqty = 0;
        double mu = 0;
        double overship = 0;
        double ppomkrypd = 0;
        double shiptoorder = 0;
        double gmtqtytotal=0;
        double unaccgmt = 0;
        double shipyardage = 0;
        double allocatewithnodiscount = 0;
        double leftover = 0;


        double ttlshipqty = 0;
        double ttlcutqty = 0;
        double ttlsampleqty = 0;
        double ttlpulloutqty = 0;
        double ttlbulkmkrypd = 0;
        double ttlallocatedqty = 0;
        double ttlorderqty = 0;
        int ttlshiptoorder95 = 0;
        int ttlshiptoorder100 = 0;
        int ttlypdvariancebig = 0;
        int ttlypdvariancesmall = 0;
        double ttlmu=0;
        double ttlovership = 0;
        double ttlcutwastageypd = 0;
        double ttlissue = 0;
        double ttlgmtqtytotal = 0;
        double ttlunaccgmt = 0;
        double ttlshipyardage = 0;
        double ttlallocatewithnodiscount = 0;
        double ttlleftover = 0;
        double ttlshipordersum = 0;
        double[,] FabricPatternValue = { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };
      
        foreach (DataRow dr in StandardMUReportList.Rows)
        {
            shipqty = dr["SHIP_QTY"].ToDouble();
            cutqty = dr["CUTQTY"].ToDouble();
            sampleqty = dr["SAMPLE_QTY"].ToDouble();
            pulloutqty = dr["PULLOUT_QTY"].ToDouble();
            bulkmkrypd = dr["BULK_MKR_YPD"].ToDouble();
            allocatedqty = dr["allocatedQty"].ToDouble();
            orderqty = dr["ORDERQTY"].ToDouble();
            cutwastageypd = dr["CutWastageYPD"].ToDouble();
            issueqty = dr["Issued"].ToDouble();
            mu = dr["MU"].ToDouble() / 100;
            overship = dr["OVERSHIP"].ToDouble() / 100;
            shiptoorder = dr["ShipToOrder"].ToDouble();
            ppomkrypd = dr["BULK_MKR_YPD"].ToDouble() - dr["PPO_YPD"].ToDouble();
            gmtqtytotal = dr["GMT_QTY_TOTAL"].ToDouble();
            unaccgmt = dr["UnaccGmt"].ToDouble();
            shipyardage = dr["ShipYardage"].ToDouble();
            allocatewithnodiscount = dr["allocatedQty(nodiscount)"].ToDouble();
            leftover = dr["Leftover"].ToDouble();

            ttlshipqty += shipqty;
            ttlcutqty += cutqty;
            ttlsampleqty += sampleqty;
            ttlpulloutqty += pulloutqty;
            ttlbulkmkrypd += bulkmkrypd;
            ttlallocatedqty += allocatedqty;
            ttlorderqty += orderqty;
            ttlmu += mu * orderqty;
            ttlovership += (1 + overship) * orderqty;
            ttlcutwastageypd += cutwastageypd;
            ttlissue += issueqty;
            ttlgmtqtytotal += gmtqtytotal;
            ttlunaccgmt +=unaccgmt;
            ttlshipyardage += shipyardage;
            ttlallocatewithnodiscount += allocatewithnodiscount;
            ttlleftover += leftover;
           
            if (shiptoorder < 95)
            {
                ttlshiptoorder95++;
            }
            else if (shiptoorder < 100)
            {
                ttlshipordersum = ttlshipordersum + shipqty + sampleqty;
                ttlshiptoorder100++;
            }
            if (bulkmkrypd - ppomkrypd > 0.25)
            {
                ttlypdvariancebig++;
            }
            else if (bulkmkrypd - ppomkrypd < -0.25)
            {
                ttlypdvariancesmall++;
            }
            switch (dr["PATTERN_TYPE"].ToString())
            {
                case "Check":
                    FabricPatternValue[0, 0] += mu * orderqty;
                    FabricPatternValue[0, 1] += orderqty;
                    break;
                case "Solid":
                    FabricPatternValue[1, 0] += mu * orderqty;
                    FabricPatternValue[1, 1] +=  orderqty;
                    break;
                case "Stripe":
                    FabricPatternValue[2, 0] += mu * orderqty;
                    FabricPatternValue[2, 1] += orderqty;
                    break;
                case "Check;Solid":
                case "Solid;Check":
                    FabricPatternValue[3, 0] += mu * orderqty;
                    FabricPatternValue[3, 1] += orderqty;
                    break;
                case "Check:Stripe":
                case "Stripe:Check":
                    FabricPatternValue[4, 0] += mu * orderqty;
                    FabricPatternValue[4, 1] += orderqty;
                    break;
                case "Solid:Stripe":
                case "Stripe:Solid":
                    FabricPatternValue[5, 0] += mu * orderqty;
                    FabricPatternValue[5, 1] += orderqty;
                    break;
                case "Check;Solid;Stripe":
                case "Check;Stripe;Solid":
                case "Solid;Check;Stripe":
                case "Solid;Stripe;Check":
                case "Stripe;Solid;Check":
                case "Stripe;Check;Solid":
                    FabricPatternValue[6, 0] += mu * orderqty;
                    FabricPatternValue[6, 1] += orderqty;
                    break;
            }
            
            
        }
        for (int i = 0; i < 7; i++)
        {
            if (FabricPatternValue[i, 1] != 0)
            {
                FabricPattern += (FabricPatternValue[i, 0] / FabricPatternValue[i, 1]).ToString("0.00") + "%,";
            }
            else
            {
                FabricPattern += "0.00%,";
            }
        }
        lblshiptocut.InnerText = ((ttlshipqty + ttlsampleqty - ttlpulloutqty).divide(ttlcutqty) * 100).ToString("f2");
        lblshiptorec.InnerText = (((ttlshipqty + ttlsampleqty - ttlpulloutqty) * ttlbulkmkrypd).divide(ttlallocatedqty) * 100).ToString("f");
        lblshiptoorder.InnerText = (ttlshipqty.divide(ttlorderqty) * 100).ToString("f2");
        lblcuttoorder.InnerText = (ttlcutqty.divide(ttlorderqty) * 100).ToString("f2");
        lblcuttorec.InnerText = (ttlcutqty.divide(ttlallocatedqty) * 100).ToString("f2");
        lblshiporder1.InnerText = (ttlshiptoorder95 / StandardMUReportList.Rows.Count * 100).ToString("f2");
        lblshiporder2.InnerText = (ttlshiptoorder100 / StandardMUReportList.Rows.Count * 100).ToString("f2");
        lblmu.InnerText = (ttlmu.divide(ttlorderqty) * 100).ToString("f2");
        lblttlovership.InnerText = (ttlovership.divide(ttlorderqty) * 100).ToString("f2");
        lblttlleftfab.InnerText = (ttlcutwastageypd.divide(ttlissue) * 100).ToString("f2");
        lblypdvariancebig.InnerText = (ttlypdvariancebig / StandardMUReportList.Rows.Count * 100).ToString("f2");
        lblypdvariancesmall.InnerText = (ttlypdvariancesmall / StandardMUReportList.Rows.Count * 100).ToString("f2");
        lblcutwastper.InnerText = (ttlcutwastageypd.divide(ttlissue)*100).ToString("f2");
        lblleftovergar.InnerText = (ttlgmtqtytotal.divide(ttlcutqty) * 100).ToString("f2");
        lblunaccountablegar.InnerText = (ttlcutqty.divide(ttlunaccgmt) * 100).ToString("f2");
        lblleftovertotal.InnerText = ttlgmtqtytotal.ToString("f2");
        lblorderqty.InnerText = ttlorderqty.ToString("f2");
        lblcutqty.InnerText = ttlcutqty.ToString("f2");
        lblshippedqty.InnerText = (ttlshipqty + ttlsampleqty).ToString("f2");
        lblunaccountableqty.InnerText = ttlunaccgmt.ToString("f2");
        lblshipyardage.InnerText = ttlshipyardage.ToString("f2");
        lblallocate.InnerText = ttlallocatedqty.ToString("f2");
        lblallocatewithnodiscount.InnerText = allocatewithnodiscount.ToString("f2");
        lbltotalleftover.InnerText = allocatewithnodiscount.ToString("f2");
        lbltotalleftover.InnerText = ttlleftover.ToString("f2");
        lblypdvarbig.InnerText = ttlypdvariancebig.ToString();
        lblypdvarsmall.InnerText = ttlypdvariancesmall.ToString();
        lblshiporder95.InnerText = ttlshiptoorder95.ToString();
        lblshiporder100.InnerText = ttlshiptoorder100.ToString();
        lblshipordersum.InnerText = ttlshipordersum.ToString("f2");

    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
       
        this.nodata.Visible = false;
        gvDetail.Visible = false;
        divSummary.Visible = false;
        if (!CheckQuery())
            return;
        string JoList = "";
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
                this.nodata.Visible = true;
                return;
            }

        }
        beginProgress();
        StandardMUReportList = MESComment.MUReportSql.GetLockMuReport(JoList, ddlFtyCd.SelectedItem.Value, ddlGarmentType.SelectedItem.Value, txtFromDate.Text, txtToDate.Text);
        if (StandardMUReportList.Rows.Count > 0)
        {
            strRunNO = StandardMUReportList.Rows[0]["RunNo"].ToString();
            btnLock.Enabled = false;
             gvDetail.Visible = true;
             divSummary.Visible = true;
          
        }
        else
        {

            btnLock.Enabled = true;
            JoList = MESComment.MUReportSql.GetOriJo(JoList);//add by ming 转换成源jo
            StandardMUReportList = GetMasterTable(JoList);
            if (StandardMUReportList.Rows.Count <= 0)
            {
                this.nodata.Visible = true;
            }
            else
            {
                gvDetail.Visible = true;
                divSummary.Visible = true;
               
            }
        }
        btnLock.CommandArgument = strRunNO;
        gvDetail.DataSource = StandardMUReportList;
        gvDetail.DataBind();
        GenerateSummary();
        finishProgress();
      
    }
    protected void btnLock_Click(object sender, EventArgs e)
    {
        strRunNO = btnLock.CommandArgument;
        StandardMUReportList = MESComment.MUReportSql.GetStandardMUReportList(ddlFtyCd.SelectedItem.Value, strRunNO,"");
        gvDetail.DataSource = StandardMUReportList;
        gvDetail.DataBind();
        GenerateSummary();
        if (!IsCanLock())
        {
           
            return;
        }
        if (MESComment.MUReportSql.LockMuReport(strRunNO))
            ResponseClient("locksuccess");
        else
            ResponseClient("lockfail");
       
           
    }
    /// <summary>
    /// add by ming 检查是否符合Lock条件
    /// </summary>
    /// <returns></returns>
    private bool IsCanLock()
    {
        DataTable mureport = MESComment.MUReportSql.GetMUReportByRunNo(strRunNO);
        if (mureport.Rows.Count == 0)
        {
            ResponseClient("lockrecord");
            return false;
        }
        if (txtJoNo.Text != "")
        {
            ResponseClient("lockno");
            return false;
        }
        if (ddlFtyCd.SelectedIndex == 0)
        {
            ResponseClient("lockfty");
            return false;
        }
        if (ddlGarmentType.SelectedIndex == 0)
        {
            ResponseClient("lockgarmenttype");
            return false;
        }
        if (ddlOutSource.SelectedIndex != 0)
        {
            ResponseClient("locksamgroup");
            return false;
        }
        
        DateTime fromdate =mureport.Rows[0]["FromDate"].ToDate();
        DateTime todate = mureport.Rows[0]["ToDate"].ToDate();
        if (fromdate.Year != todate.Year)
        {
            ResponseClient("lockmonth");
            return false;
        }
        if (fromdate.Month != todate.Month)
        {
            ResponseClient("lockmonth");
            return false;
        }
        if ((todate.Day - fromdate.Day + 1) != DateTime.DaysInMonth(fromdate.Year, fromdate.Month))
        {
            ResponseClient("lockday");
            
            return false;
        }

        //判断当月是否锁
        if (mureport.Rows[0]["LOCK_FLAG"].ToString() == "Y")
        {
            ResponseClient("lockexists");
            return false;
        }
        DateTime pretodate = fromdate.AddDays(-1);
        DateTime prefromdate = fromdate.AddMonths(-1); // fromdate.Month(1-DateTime.DaysInMonth(pretodate.Year, pretodate.Month));
        //判断上月是否已锁
        DataTable premureport = MESComment.MUReportSql.GetExistsMuList(mureport.Rows[0]["FTY_CD"].ToString(), prefromdate.DateToDateStr(), pretodate.DateToDateStr(),"");
        if (premureport.Rows.Count == 0)
        {
            ResponseClient("lockpremonth");
            return false;
        }

        return true;
       


    }
    /// <summary>
    ///把CB单分解
    /// </summary>
    /// <param name="JoList"></param>
    /// <returns></returns>


    public bool CheckQuery()
    {
        if (txtJoNo.Text.Trim().Length == 0)
        {
            if (txtFromDate.Text.Trim().Length == 0)
            {
                ResponseClient("txtdatefrom");
                return false;
            }
            if (txtToDate.Text.Trim().Length == 0)
            {
                ResponseClient("txtdateto");
                return false;
            }
            DateTime fromdate = txtFromDate.Text.ToDate();
            DateTime todate = txtToDate.Text.ToDate();
            if (fromdate.Year != todate.Year)
            {
                ResponseClient("overmonth");
                return false;
            }
            if (fromdate.Month != todate.Month)
            {
                ResponseClient("overmonth");
                return false;
            }
           
        }
        return true;

    }

    private void ResponseClient(string msgtype)
    {
        string msg = "";
        switch (msgtype)
        {
            case "txtjo":
                msg = "JO_NO不能为空！";
                break;
            case "txtdatefrom":
                msg = "Fin Close Date From 必须选择日期！";
                break;
            case "txtdateto":
                msg = "Fin Close Date To 必须选择日期！";
                break;
            case "locksuccess":
                msg = "锁定成功！";
                break;
            case "lockfail":
                msg = "锁定失败！";
                break;
            case "overmonth":
                msg = "不能跨月查询！";
                break;
            case "lockno":
                msg = "进行锁定，JO_NO必须为空！";
                break;
            case "lockfty":
                msg = "进行锁定，必须选择工厂！";
                break;
            case "lockgarmenttype":
                msg = "进行锁定，必须选择Garment Type！";
                break;
            case "locksamgroup":
                msg = "进行锁定，SAM GROUP必须选择All！";
                break;
            case "lockrecord":
                msg = "没有要进行锁定的记录！";
                break;
            case "lockmonth":
                msg = "不能跨月锁定！";
                break;
            case "lockday":
                msg = "必须按月锁定！";
                break;
            case "lockexists":
                msg = "此月份数据已锁定！";
                break;
            case "lockpremonth":
                msg = "前一月份数据未锁定，请先锁定前一月份！";
                break;
        }
        if (this.ClientScript.IsStartupScriptRegistered("ResponseClient"))
            this.ClientScript.RegisterStartupScript(typeof(string), "ResponseClient1", string.Format("alert('{0}')", msg), true);
        else
            this.ClientScript.RegisterStartupScript(typeof(string), "ResponseClient", string.Format("alert('{0}')", msg), true);
    }

    public string GetFormatJo(DataTable dt)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (DataRow dr in dt.Rows)
        {
            sb.Append(",'" + dr["JO_NO"] + "'");
        }
        if (sb.Length > 0)
            sb = sb.Remove(0, 1);
        return sb.ToString();
    }

}
