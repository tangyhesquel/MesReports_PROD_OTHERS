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
                             "Factory", "Fty Job order", "YPD Job No", "Dlvy  Date", "Buyer", "sale Contract", "PPO Number", "Fabric Width", "Long/ short sleeve", "Wash type", "Fabric Pattern", "Marker Utilization (%)","Fabric (YDS)","Unacc Fab","Garment (DZ)","YPD","RATIO","Buyer's max over shipment allowance</th></tr><tr>",
                             "PPO order yds","Allocated","Issued","Marker Audited","Ship Yardage","Cutting Wastage","Pull-out Fab", "Short Fab ", "Defect Fab", "Defect Panels", "FOC", "Leftover fabric","Remnant","SRN","RTW","Order(DZ)","Cut","Cutting Down Several","Shipped","Sample","Pull-out","Leftover Garment","Sewing Wastage","Washing Wastage","Unacc Gmt","PPO MKR YPD","BULK NET YPD","GIVEN CUT YPD","BULK MKR YPD","YPD Var","CUT YPD","SHIP YPD","Ship-to-Cut","Ship-to-Receipt","Ship-to-Order","Cut-to-Receipt","Cut-to-Order</th></tr><tr>",
                             "Yds","%","Grade A","Grade B","Grade C","Total","DZ","%","DZ","%"
                         };
    string userid = "";

    public int ship_order_count_95, ship_order_count_100, YPD_Var_big, YPD_Var_small;
    public double ship_order_sum;
    public string FabricPattern = "";
    public double Total_Allocated = 0;//开放数据
    public double Total_Ship_Yardage = 0;//开放数据
    public double Total_Leftover = 0;//开放数据
    public string strRunNO = "";
    public string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
    DataTable StandardOASInfo;

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
    private DataTable GetMasterTable(string GoList, string JoList)
    {
        GoList = "";
        JoList = "";
        DataTable jobtb = new DataTable();
        DataTable gotb = new DataTable();
        jobtb.Columns.Add("jobno");
        gotb.Columns.Add("gono");
        DataRow row_ = null;
        DataRow gorow_ = null;

        for (int i = 0; i < StandardOASInfo.Rows.Count; i++)
        {
            if (StandardOASInfo.Rows[i]["JOB_ORDER_NO"].ToString() != "")
            {
                JoList += "'" + StandardOASInfo.Rows[i]["JOB_ORDER_NO"].ToString() + "',";
                row_ = jobtb.NewRow();
                row_["jobno"] = StandardOASInfo.Rows[i]["JOB_ORDER_NO"].ToString();
                jobtb.Rows.Add(row_);
            }

            if (StandardOASInfo.Rows[i]["SC_NO"].ToString() != "")
            {
                GoList += "'" + StandardOASInfo.Rows[i]["SC_NO"].ToString() + "',";
                gorow_ = gotb.NewRow();
                gorow_["gono"] = StandardOASInfo.Rows[i]["SC_NO"].ToString();
                gotb.Rows.Add(gorow_);
            }
        }

        GoList = GoList.Substring(0, GoList.Length - 1);
        JoList = JoList.Substring(0, JoList.Length - 1);
        ///conn = Get Connection("eel")
        ///insert temp table
        ///search data
        /////////////////////////// EEL ////////////////////////////////////////
        DbConnection eelConn = MESComment.DBUtility.GetConnection("EEL");
        MESComment.DBUtility.InsertRptTempData(jobtb, eelConn);
        DataTable StandardWidthNpatternList = MESComment.MUReportSql.GetStandardWidthNpatternList(eelConn);
        DataTable StandardOrderQty = MESComment.OutsourceMUReportSql.GetStandardOrderQty(eelConn);
        DataTable StandardJoOrderQty = MESComment.MUReportSql.GetJoOrderQty(eelConn);
        DataTable StandardGoOrderQty = MESComment.MUReportSql.GetStandardGoOrderQty(eelConn);

        MESComment.DBUtility.CloseConnection(ref eelConn);

        /////////////////////////// INV ////////////////////////////////////////
        DbConnection invConn = MESComment.DBUtility.GetConnection("INV");
        MESComment.DBUtility.InsertRptTempData(jobtb, invConn);
        DataTable StandardInfo = MESComment.OutsourceMUReportSql.GetStandardInfo(invConn);
        DataTable StandardFGIS = MESComment.MUReportSql.GetStandardFromFGIS(ddlFtyCd.SelectedItem.Value, invConn);
        DataTable StandardRTW = MESComment.MUReportSql.GetStandardRTW(ddlFtyCd.SelectedItem.Value, invConn);
        DataTable StandardSRN = MESComment.MUReportSql.GetStandardSRN(ddlFtyCd.SelectedItem.Value, invConn);
        DataTable FocQuantity = MESComment.OutsourceMUReportSql.GetFocQty(ddlFtyCd.SelectedItem.Value, invConn);
        MESComment.DBUtility.CloseConnection(ref invConn);



        DataTable StandardMES = MESComment.MUReportSql.GetStandardFromMES(ddlFtyCd.SelectedItem.Value, JoList);
        DataTable StandardMUCutInfo = MESComment.MUReportSql.GetStandardMUCutInfo(JoList, ddlFtyCd.SelectedItem.Value, txtFromDate.Text, txtToDate.Text);
        DataTable StandardGivenCutAllowance = MESComment.MUReportSql.GetStandardGivenCutAllowance(JoList, ddlFtyCd.SelectedItem.Value);
        DataTable RemnantDataTable = MESComment.MUReportSql.GetStandardRemnantByJoList(JoList);
        DataTable ScQtyDataTable = MESComment.MUReportSql.Get_TTL_SC_QTY_BY_PO_LIST(JoList, ddlFtyCd.SelectedItem.Value);
        DataTable OutsourceMUReportJOList = MESComment.OutsourceMUReportSql.GetOutsourceJODetail(GoList);
        DataTable FabricWastage = MESComment.OutsourceMUReportSql.GetFabWastage(JoList);

        DataTable OutsourceMUReportList = new DataTable();
        ///Close Connection(conn)

        ///conn = get connection("INV")
        ///insert temp table
        ///search data
        ///....
        ///Close Connection(conn)
        //OutsourceMUReportList.Columns.Add("CUT_DATE");
        OutsourceMUReportList.Columns.Add("SC_NO");
        OutsourceMUReportList.Columns.Add("PULLOUT_QTY");
        OutsourceMUReportList.Columns.Add("PPO_NO");
        OutsourceMUReportList.Columns.Add("WIDTH");
        OutsourceMUReportList.Columns.Add("PATTERN_TYPE");
        OutsourceMUReportList.Columns.Add("PPO_YPD");
        OutsourceMUReportList.Columns.Add("YPD_JOB_NO");
        OutsourceMUReportList.Columns.Add("ORDER_QTY");
        OutsourceMUReportList.Columns.Add("SAMPLE_QTY");
        OutsourceMUReportList.Columns.Add("MU");
        OutsourceMUReportList.Columns.Add("ALLOCATED_QTY");
        OutsourceMUReportList.Columns.Add("BULK_MKR_YPD");
        OutsourceMUReportList.Columns.Add("BULK_NET_YPD");
        OutsourceMUReportList.Columns.Add("GIVEN_CUT_YPD");
        OutsourceMUReportList.Columns.Add("MA");
        OutsourceMUReportList.Columns.Add("REMNANT");
        OutsourceMUReportList.Columns.Add("SHIP_DATE");
        OutsourceMUReportList.Columns.Add("SHIP_QTY");
        OutsourceMUReportList.Columns.Add("CUTQTY");
        OutsourceMUReportList.Columns.Add("ORDERQTY");
        OutsourceMUReportList.Columns.Add("GMT_QTY_A");
        OutsourceMUReportList.Columns.Add("GMT_QTY_B");
        OutsourceMUReportList.Columns.Add("GMT_QTY_C");
        OutsourceMUReportList.Columns.Add("SRNQty");
        OutsourceMUReportList.Columns.Add("RTW_QTY");
        OutsourceMUReportList.Columns.Add("Issued");
        OutsourceMUReportList.Columns.Add("Leftover");
        OutsourceMUReportList.Columns.Add("allocatedQty");
        OutsourceMUReportList.Columns.Add("ShipYardage");
        OutsourceMUReportList.Columns.Add("OVERSHIP");
        OutsourceMUReportList.Columns.Add("SewingDz");
        OutsourceMUReportList.Columns.Add("WashingDz");
        OutsourceMUReportList.Columns.Add("UnaccGmt");
        OutsourceMUReportList.Columns.Add("GMT_QTY_TOTAL");
        OutsourceMUReportList.Columns.Add("FACTORY");
        OutsourceMUReportList.Columns.Add("PO_NO");
        OutsourceMUReportList.Columns.Add("PULLOUT_FAB");
        OutsourceMUReportList.Columns.Add("SHORT_FAB");
        OutsourceMUReportList.Columns.Add("DEFECT_FAB");
        OutsourceMUReportList.Columns.Add("DEFECT_PANEL");
        OutsourceMUReportList.Columns.Add("FOC_QTY");
        OutsourceMUReportList.Columns.Add("UNACC_FAB");
        OutsourceMUReportList.Columns.Add("CUTDISC");
        OutsourceMUReportList.Columns.Add("BUYER");
        OutsourceMUReportList.Columns.Add("WASH_TYPE_CD");
        OutsourceMUReportList.Columns.Add("STYLE_DESC");
        OutsourceMUReportList.Columns.Add("JOB_ORDER_NO");
        //OutsourceMUReportList.Columns.Add("SewingPercent");
        //OutsourceMUReportList.Columns.Add("WashingPercent");

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
        for (int cnt = 0; cnt < OutsourceMUReportJOList.Rows.Count; cnt++)
        {
            DataRow dr = OutsourceMUReportList.NewRow();
            double GoCutQty = 0;
            double GoCutDiscQty = 0;
            double GoSampleQty = 0;
            double GoLeftoverGradeAQty = 0;
            double GoLeftoverGradeBQty = 0;
            double GoLeftoverGradeCQty = 0;
            double GoShipQty = 0;
            double GoDefectFab = 0;
            double GoPulloutFab = 0;
            double GoShortFab = 0;
            double GoBulkMkrYPD = 0;
            double GoBulkNetYPD = 0;
            double GoMkrAudited = 0;
            string JoOrderQty = "0.00";
            double TotalMuJoOrderQty = 0;
            double TotalJoOrderQty = 0;
            double GoOverShip = 0;
            double rtw_qty_i = 0;
            double srn_qty_i = 0;
            double GoDefectPanel = 0;
            double GoFocQty = 0;
            double TotalGoOrderQty = 0;
            double GoLeftover = 0;
            double GoIssued = 0;
            double myIssued = 0;
            double myDefectFab = 0;
            double myPulloutFab = 0;
            double myShortFab = 0;
            double GoAllocatedQty = 0;
            double left_qty = 0;
            double GoGivenCutYpd = 0;
            double myShipQty = 0;
            double GoShipYard = 0;
            double GoUnaccFab = 0;
            double GoSrnQty = 0;
            double GoRtwQty = 0;
            double GoRemnant = 0;
            double myRemnant = 0;
            double myMA = 0;
            double myDefectPanel = 0;
            double myPulloutQty = 0;
            double GoPulloutQty = 0;
            double myLeftoverGradeAQty = 0;
            double myLeftoverGradeBQty = 0;
            double myLeftoverGradeCQty = 0;
            double mySampleQty = 0;
            double GoUnaccGmt = 0;
            double GoSewWastage = 0;
            double GoWetWastage = 0;
            double myPpoYpd = 0;
            double GoPpoYpd = 0;
            String ppoNo = "";
            String width = "";
            String patternType = "";
            String ppoYpdNo = "";
            DateTime ShipDate = DateTime.Now;

            dr["SC_NO"] = OutsourceMUReportJOList.Rows[cnt]["SC_NO"].ToString();
            dr["FACTORY"] = OutsourceMUReportJOList.Rows[cnt]["FACTORY"].ToString();


            if (cnt == 0 || OutsourceMUReportJOList.Rows[cnt]["SC_NO"].ToString() != OutsourceMUReportJOList.Rows[cnt - 1]["SC_NO"].ToString())
            {
                for (int j = 0; j < StandardInfo.Rows.Count; j++)
                {
                    if (StandardInfo.Rows[j]["SC_NO"].ToString() == OutsourceMUReportJOList.Rows[cnt]["SC_NO"].ToString())
                    {
                        if (StandardInfo.Rows[j]["BUYER"].ToString() != "")
                            dr["BUYER"] = StandardInfo.Rows[j]["BUYER"].ToString();

                        if (StandardInfo.Rows[j]["WASH_TYPE_CD"].ToString() != "")
                            dr["WASH_TYPE_CD"] = StandardInfo.Rows[j]["WASH_TYPE_CD"].ToString();

                        if (StandardInfo.Rows[j]["STYLE_DESC"].ToString() != "")
                            dr["STYLE_DESC"] = StandardInfo.Rows[j]["STYLE_DESC"].ToString();
                    }
                }

                for (int i = 0; i < OutsourceMUReportJOList.Rows.Count; i++)
                {
                    if (OutsourceMUReportJOList.Rows[i]["SC_NO"].ToString() == dr["SC_NO"].ToString())
                    {
                        dr["JOB_ORDER_NO"] += OutsourceMUReportJOList.Rows[i]["JOB_ORDER_NO"].ToString() + "; ";
                        DataRow row = OutsourceMUReportJOList.Rows[i];
                        String poNoStr = row["JOB_ORDER_NO"].ToString();
                        String scNoStr = row["SC_NO"].ToString();
                        double myCutQty = 0;
                        double myCutDiscQty = 0;
                        for (int j = 0; j < StandardMES.Rows.Count; j++)
                        {
                            if (StandardMES.Rows[j]["job_order_no"].ToString().ToLower() == poNoStr.ToLower())
                            {
                                //StandardMUReportList.Rows[i]["CUTQTY"] = double.Parse(StandardMES.Rows[j]["cutqty"].ToString()).ToString("f2");
                                //StandardMUReportList.Rows[i]["CUT_DATE"] = StandardMES.Rows[j]["CUT_DATE"];
                                myPulloutQty = double.Parse(StandardMES.Rows[j]["PULLOUT_QTY"].ToString());
                                GoPulloutQty += myPulloutQty;
                                //StandardMUReportList.Rows[i]["DISCREPANCY_QTY"] = double.Parse(StandardMES.Rows[j]["DISCREPANCY_QTY"].ToString()).ToString("f2");
                                //StandardMUReportList.Rows[i]["SAMPLE_QTY"] = double.Parse(StandardMES.Rows[j]["SAMPLE_QTY"].ToString());
                                //myCutQty = double.Parse(StandardMES.Rows[j]["cutqty"].ToString());
                                //SEW_WASTAGE = StandardMES.Rows[j]["SEW_WASTAGE_C"].ToString() == "" ? double.Parse("0.00") : double.Parse(StandardMES.Rows[j]["SEW_WASTAGE_C"].ToString());
                                //WASH_WASTAGE = StandardMES.Rows[j]["WASH_WASTAGE_C"].ToString() == "" ? double.Parse("0.00") : double.Parse(StandardMES.Rows[j]["WASH_WASTAGE_C"].ToString());
                            }
                        }
                        myCutQty = OutsourceMUReportJOList.Rows[i]["CUTQTY"].ToString() == "" ? double.Parse("0.00") : double.Parse(OutsourceMUReportJOList.Rows[i]["CUTQTY"].ToString());
                        myCutDiscQty = OutsourceMUReportJOList.Rows[i]["CUT_DISC"].ToString() == "" ? double.Parse("0.00") : double.Parse(OutsourceMUReportJOList.Rows[i]["CUT_DISC"].ToString());
                        GoCutQty += myCutQty;
                        GoCutDiscQty += myCutDiscQty;
                        mySampleQty = OutsourceMUReportJOList.Rows[i]["SAMPLE_QTY"].ToString() == "" ? double.Parse("0.00") : double.Parse(OutsourceMUReportJOList.Rows[i]["SAMPLE_QTY"].ToString());
                        GoSampleQty += mySampleQty;
                        myLeftoverGradeAQty = OutsourceMUReportJOList.Rows[i]["GMT_QTY_A"].ToString() == "" ? double.Parse("0.00") : double.Parse(OutsourceMUReportJOList.Rows[i]["GMT_QTY_A"].ToString());
                        myLeftoverGradeBQty = OutsourceMUReportJOList.Rows[i]["GMT_QTY_B"].ToString() == "" ? double.Parse("0.00") : double.Parse(OutsourceMUReportJOList.Rows[i]["GMT_QTY_B"].ToString());
                        myLeftoverGradeCQty = OutsourceMUReportJOList.Rows[i]["GMT_QTY_C"].ToString() == "" ? double.Parse("0.00") : double.Parse(OutsourceMUReportJOList.Rows[i]["GMT_QTY_C"].ToString());
                        GoLeftoverGradeAQty += myLeftoverGradeAQty;
                        GoLeftoverGradeBQty += myLeftoverGradeBQty;
                        GoLeftoverGradeCQty += myLeftoverGradeCQty;
                        myShipQty = OutsourceMUReportJOList.Rows[i]["SHIP_QTY"].ToString() == "" ? double.Parse("0.00") : double.Parse(OutsourceMUReportJOList.Rows[i]["SHIP_QTY"].ToString());
                        GoShipQty += myShipQty;
                        GoSewWastage += OutsourceMUReportJOList.Rows[i]["SEW_WASTAGE"].ToString() == "" ? double.Parse("0.00") : double.Parse(OutsourceMUReportJOList.Rows[i]["SEW_WASTAGE"].ToString());
                        GoWetWastage += OutsourceMUReportJOList.Rows[i]["WET_WASTAGE"].ToString() == "" ? double.Parse("0.00") : double.Parse(OutsourceMUReportJOList.Rows[i]["WET_WASTAGE"].ToString());
                        

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
                                            ppoNo += "; " + StandardWidthNpatternList.Rows[j]["PPO_NO"].ToString();
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

                        for (int j = 0; j < StandardOASInfo.Rows.Count; j++)
                        {
                            String poNoStrTmp = StandardOASInfo.Rows[j]["JOB_ORDER_NO"].ToString();

                            if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                            {
                                if (ShipDate == DateTime.Now)
                                    ShipDate = DateTime.Parse(StandardOASInfo.Rows[j]["SHIP_DATE"].ToString());

                                if (ShipDate > DateTime.Parse(StandardOASInfo.Rows[j]["SHIP_DATE"].ToString()))
                                    ShipDate = DateTime.Parse(StandardOASInfo.Rows[j]["SHIP_DATE"].ToString());
                                    
                            }

                        }


                        for (int j = 0; j < FabricWastage.Rows.Count; j++)
                        {
                            String poNoStrTmp = FabricWastage.Rows[j]["JOB_ORDER_NO"].ToString();
                        
                            if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                            {
                                myDefectFab = FabricWastage.Rows[j]["DEFECT_FABRIC"].ToString() == "" ? double.Parse("0.00") : double.Parse(FabricWastage.Rows[j]["DEFECT_FABRIC"].ToString());
                                myPulloutFab = FabricWastage.Rows[j]["PULL_OUT"].ToString() == "" ? double.Parse("0.00") : double.Parse(FabricWastage.Rows[j]["PULL_OUT"].ToString());
                                myShortFab = FabricWastage.Rows[j]["SHORTAGE_FABRIC"].ToString() == "" ? double.Parse("0.00") : double.Parse(FabricWastage.Rows[j]["SHORTAGE_FABRIC"].ToString());
                                
                                GoDefectFab += myDefectFab;
                                GoPulloutFab += myPulloutFab;
                                GoShortFab += myShortFab;
                            }
                            
                        }

                        for (int j = 0; j < StandardJoOrderQty.Rows.Count; j++)
                        {
                            String poNoStrTmp = StandardJoOrderQty.Rows[j]["JO_NO"].ToString();
                            
                            if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                            {
                                if (StandardJoOrderQty.Rows[j]["OrderQTY"].ToString() != "")
                                {
                                    JoOrderQty = StandardJoOrderQty.Rows[j]["OrderQTY"].ToString();
                                }

                                TotalJoOrderQty += double.Parse(JoOrderQty);
                                GoOverShip += StandardJoOrderQty.Rows[j]["OVERSHIP"].ToString() == "" ? double.Parse("0.00") : double.Parse(StandardJoOrderQty.Rows[j]["OVERSHIP"].ToString());
                            }
                            
                        }

                        double bulkNetYpd = 0;
                        double bulkMkrYpd = 0;
                        String orderQty = "0.00";
                        
                        for (int j = 0; j < StandardMUCutInfo.Rows.Count; j++)
                        {
                            String poNoStrTmp = StandardMUCutInfo.Rows[j]["JOB_ORDER_NO"].ToString();
                            if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                            {
                                bulkNetYpd = 0;
                                bulkMkrYpd = 0;
                                if (StandardMUCutInfo.Rows[j]["BULK_NET_YPD"].ToString() != "")
                                {
                                    bulkNetYpd = double.Parse(StandardMUCutInfo.Rows[j]["BULK_NET_YPD"].ToString());
                                }
                                if (StandardMUCutInfo.Rows[j]["BULK_MARKER_YPD"].ToString() != "")
                                {
                                    bulkMkrYpd = double.Parse(StandardMUCutInfo.Rows[j]["BULK_MARKER_YPD"].ToString());
                                }
                                if (StandardMUCutInfo.Rows[j]["PPO_NET_YPD"].ToString() != "")
                                {
                                    myPpoYpd = double.Parse(StandardMUCutInfo.Rows[j]["PPO_NET_YPD"].ToString());
                                }
                                myMA = myCutQty * bulkMkrYpd; 
                                TotalMuJoOrderQty += double.Parse(StandardMUCutInfo.Rows[j]["MU"].ToString()) * double.Parse(JoOrderQty);
                                GoBulkMkrYPD += double.Parse(StandardMUCutInfo.Rows[j]["BULK_MARKER_YPD"].ToString()) * double.Parse(JoOrderQty);
                                GoBulkNetYPD += double.Parse(StandardMUCutInfo.Rows[j]["BULK_NET_YPD"].ToString()) * double.Parse(JoOrderQty);
                                GoMkrAudited += myMA;
                                myDefectPanel = myCutDiscQty * bulkMkrYpd;
                                GoDefectPanel += myDefectPanel;
                                GoPpoYpd += myPpoYpd * double.Parse(JoOrderQty);
                            }
                            
                        }

                        

                        //for (int j = 0; j < StandardFGIS.Rows.Count; j++)
                        //{
                        //    String poNoStrTmp = StandardFGIS.Rows[j]["job_order_no"].ToString();
                        //    if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                        //    {
                        //        StandardMUReportList.Rows[i]["GMT_QTY_A"] = StandardFGIS.Rows.Count > 0 ? (double.Parse(StandardFGIS.Rows[j]["gmt_qty_a"].ToString()) / 12).ToString("f2") : double.Parse("0.00").ToString("f2");
                        //        StandardMUReportList.Rows[i]["GMT_QTY_B"] = StandardFGIS.Rows.Count > 0 ? (double.Parse(StandardFGIS.Rows[j]["gmt_qty_b"].ToString()) / 12).ToString("f2") : double.Parse("0.00").ToString("f2");
                        //        SEW_WASTAGE += StandardFGIS.Rows[j]["sew_qty_b"].ToString() == "" ? double.Parse("0.00") : double.Parse(StandardFGIS.Rows[j]["sew_qty_b"].ToString());
                        //        WASH_WASTAGE += StandardFGIS.Rows[j]["wash_qty_b"].ToString() == "" ? double.Parse("0.00") : double.Parse(StandardFGIS.Rows[j]["wash_qty_b"].ToString());
                        //    }
                        //}

                        //OutsourceMUReportList.Rows[i]["GMT_QTY_TOTAL"] = (double.Parse(OutsourceMUReportList.Rows[i]["GMT_QTY_A"].ToString() == "" ? "0" : OutsourceMUReportList.Rows[i]["GMT_QTY_A"].ToString()) + double.Parse(OutsourceMUReportList.Rows[i]["GMT_QTY_B"].ToString() == "" ? "0" : OutsourceMUReportList.Rows[i]["GMT_QTY_B"].ToString()) + double.Parse(OutsourceMUReportList.Rows[i]["GMT_QTY_C"].ToString() == "" ? "0" : OutsourceMUReportList.Rows[i]["GMT_QTY_C"].ToString())).ToString("f2");

                        //StandardMUReportList.Rows[i]["REMNANT"] = GetStringValueByJoFromDataTable(RemnantDataTable, poNoStr);

                        //for (int j = 0; j < StandardGisInfo.Rows.Count; j++)
                        //{
                        //    String poNoStrTmp = StandardGisInfo.Rows[j]["JO_NO"].ToString();
                            
                        //    if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                        //    {
                        //        //StandardMUReportList.Rows[i]["SHIP_DATE"] = StandardGisInfo.Rows[j]["SHIP_DATE"];

                        //        myShipQty = StandardGisInfo.Rows[j]["SHIP_QTY"].ToString() == "" ?
                        //                                    double.Parse("0.00") :
                        //                                    (double.Parse(StandardGisInfo.Rows[j]["SHIP_QTY"].ToString()) / 12);

                        //        GoShipQty += myShipQty;
                        //    }
                            
                        //}

                        

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

                        TotalGoOrderQty += OrderQty;
                        
                        for (int j = 0; j < StandardRTW.Rows.Count; j++)
                        {
                            String poNoStrTmp = StandardRTW.Rows[j]["JO_NO"].ToString();
                            
                            if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                            {
                                //StandardMUReportList.Rows[i]["RTW_QTY"] = StandardRTW.Rows[j]["rtw_qty"].ToString() == "" ? double.Parse("0.00").ToString("f2") : double.Parse(StandardRTW.Rows[j]["rtw_qty"].ToString()).ToString("f2");
                                rtw_qty_i = double.Parse(StandardRTW.Rows[j]["rtw_qty_i"].ToString() == "" ? double.Parse("0.00").ToString("f2") : double.Parse(StandardRTW.Rows[j]["rtw_qty_i"].ToString()).ToString("f2"));
                                GoRtwQty += rtw_qty_i;
                            }
                            
                        }
                        
                        for (int j = 0; j < StandardSRN.Rows.Count; j++)
                        {
                            String poNoStrTmp = StandardSRN.Rows[j]["jo_no"].ToString();
                            
                            if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                            {
                                //StandardMUReportList.Rows[i]["SRNQty"] = StandardSRN.Rows[j]["srn_qty"].ToString() == "" ? double.Parse("0.00").ToString("f2") : double.Parse(StandardSRN.Rows[j]["srn_qty"].ToString()).ToString("f2");
                                srn_qty_i = double.Parse(StandardSRN.Rows[j]["srn_qty_i"].ToString() == "" ? double.Parse("0.00").ToString("f2") : double.Parse(StandardSRN.Rows[j]["srn_qty_i"].ToString()).ToString("f2"));
                                GoSrnQty += srn_qty_i;
                            }
                           
                        }

                        myIssued = srn_qty_i - rtw_qty_i - myPulloutFab - myShortFab;
                        GoIssued += myIssued;

                        myRemnant = (srn_qty_i = rtw_qty_i - myPulloutFab - myShortFab) * 0.005;
                        GoRemnant += myRemnant;

                        GoUnaccFab += srn_qty_i - rtw_qty_i - myPulloutFab - myShortFab - myMA - myDefectFab - myDefectPanel - myRemnant;

                        String PPOList = "";
                        double leftOverQty;

                        leftOverQty = 0;

                        if (ppoNo != "")
                        {
                            string[] ppoArray = ppoNo.Split(';');
                            for (int j = 0; j < ppoArray.Length; j++)
                            {
                                PPOList += "'" + ppoArray[j] + "'" + ",";
                            }
                            PPOList = PPOList.Substring(0, PPOList.Length - 1);

                        }


                        //if (StandardGisInfo.Select("JO_NO='" + poNoStr + "'")[0][4].ToString() == "Y" && PPOList != "")
                        //{
                        if (PPOList != "")
                        {
                            double sc_qty1 = double.Parse(GetStringValueByJoFromDataTable(ScQtyDataTable, poNoStr));



                            foreach (var r in leftOverTable.Select("PPO_NO IN(" + PPOList + ")"))
                            {
                                leftOverQty += Convert.ToDouble(r["Leftover_QTY"]);
                            }

                            left_qty = leftOverQty;


                            GoLeftover += left_qty;
                        }  
                           
                        //}
                        //else
                        //{
                        //    StandardMUReportList.Rows[i]["Leftover"] = 0.00;
                        //}

                        GoAllocatedQty += left_qty + myIssued;
                        GoShipYard += myShipQty * bulkMkrYpd;
                        

                        GoUnaccGmt += (myCutQty + myPulloutQty) - (myShipQty + mySampleQty + myLeftoverGradeAQty + myLeftoverGradeBQty + myLeftoverGradeCQty);
                        for (int j = 0; j < StandardGivenCutAllowance.Rows.Count; j++)
                        {
                            String poNoStrTmp = StandardGivenCutAllowance.Rows[j]["JO_NO"].ToString();
                            if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                            {
                                GoGivenCutYpd += (double.Parse(StandardGivenCutAllowance.Rows[j]["Given_Cut_Allowance"].ToString() == "" ? "0" : StandardGivenCutAllowance.Rows[j]["Given_Cut_Allowance"].ToString()) + 1) * bulkNetYpd;
                            }
                        }

                        for (int j = 0; j < FocQuantity.Rows.Count; j++)
                        {
                            String poNoStrTmp = FocQuantity.Rows[j]["JOB_ORDER_NO"].ToString();
                            if (poNoStrTmp.ToLower() == poNoStr.ToLower())
                            {
                                GoFocQty += FocQuantity.Rows[j]["FOC_QTY"].ToString() == "" ? double.Parse("0.00") : double.Parse(FocQuantity.Rows[j]["FOC_QTY"].ToString());
                            }
                        }
                    }
                }
                dr["JOB_ORDER_NO"] = dr["JOB_ORDER_NO"].ToString().Substring(0, dr["JOB_ORDER_NO"].ToString().Length - 2);
                dr["CUTQTY"] = GoCutQty.ToString("f2");
                dr["CUTDISC"] = GoCutDiscQty.ToString("f2");
                dr["SAMPLE_QTY"] = GoSampleQty.ToString("f2");
                dr["GMT_QTY_A"] = GoLeftoverGradeAQty.ToString("f2");
                dr["GMT_QTY_B"] = GoLeftoverGradeBQty.ToString("f2");
                dr["GMT_QTY_C"] = GoLeftoverGradeCQty.ToString("f2");
                dr["GMT_QTY_TOTAL"] = (GoLeftoverGradeAQty + GoLeftoverGradeBQty + GoLeftoverGradeCQty).ToString("f2");
                dr["SHIP_QTY"] = GoShipQty.ToString("f2");
                dr["PPO_NO"] = ppoNo;
                dr["WIDTH"] = width;
                dr["PATTERN_TYPE"] = patternType;
                dr["YPD_JOB_NO"] = ppoYpdNo;
                dr["PULLOUT_FAB"] = GoPulloutFab.ToString("f2");
                dr["SHORT_FAB"] = GoShortFab.ToString("f2");
                dr["DEFECT_FAB"] = GoDefectFab.ToString("f2");
                dr["MU"] = (TotalMuJoOrderQty / TotalJoOrderQty).ToString("f2") + "%";
                dr["ORDERQTY"] = TotalJoOrderQty.ToString("f2");
                dr["OVERSHIP"] = GoOverShip.ToString("f2") + "%";
                dr["BULK_MKR_YPD"] = (GoBulkMkrYPD / TotalJoOrderQty).ToString("f2");
                dr["BULK_NET_YPD"] = (GoBulkNetYPD / TotalJoOrderQty).ToString("f2");
                dr["MA"] = GoMkrAudited.ToString("f2");
                dr["RTW_QTY"] = GoRtwQty.ToString("f2");
                dr["SRNQty"] = GoSrnQty.ToString("f2");
                dr["Issued"] = GoIssued.ToString("f2");
                dr["REMNANT"] = GoRemnant.ToString("f2");
                dr["DEFECT_PANEL"] = GoDefectPanel.ToString("f2");
                dr["FOC_QTY"] = GoFocQty.ToString("f2");
                dr["LEFTOVER"] = GoLeftover.ToString("f2");
                dr["ORDER_QTY"] = TotalGoOrderQty.ToString("f2");
                dr["allocatedQty"] = GoAllocatedQty.ToString("f2");
                dr["ShipYardage"] = GoShipYard.ToString("f2");
                dr["UNACC_FAB"] = GoUnaccFab.ToString("f2");
                dr["PULLOUT_QTY"] = GoPulloutQty.ToString("f2");
                dr["SewingDz"] = GoSewWastage.ToString("f2");
                dr["WashingDz"] = GoWetWastage.ToString("f2");
                dr["PPO_YPD"] = (GoPpoYpd / TotalJoOrderQty).ToString("f2");
                dr["SHIP_DATE"] = ShipDate.Date.ToString().Substring(0, ShipDate.Date.ToString().Length - 7);
                dr["GIVEN_CUT_YPD"] = (GoGivenCutYpd / TotalJoOrderQty).ToString("f2");
                dr["UnaccGmt"] = GoUnaccGmt.ToString("f2");
                //dr["SewingPercent"] = GoSewWastage / GoCutQty * 100 + "%";
                //dr["WashingPercent"] = GoWetWastage / GoCutQty * 100 + "%";


                OutsourceMUReportList.Rows.Add(dr);
            }
        }
        return OutsourceMUReportList;
    }


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
                tcHeader[12].Attributes.Add("colspan", "16");
                tcHeader[12].Text = strHeader[12];

                tcHeader.Add(new TableHeaderCell());
                tcHeader[13].Attributes.Add("rowspan", "3");
                tcHeader[13].Text = strHeader[13];

                tcHeader.Add(new TableHeaderCell());
                tcHeader[14].Attributes.Add("colspan", "14");
                tcHeader[14].Text = strHeader[14];

                tcHeader.Add(new TableHeaderCell());
                tcHeader[15].Attributes.Add("colspan", "7");
                tcHeader[15].Text = strHeader[15];

                tcHeader.Add(new TableHeaderCell());
                tcHeader[16].Attributes.Add("colspan", "6");
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
                tcHeader[23].Attributes.Add("colspan", "2");
                tcHeader[23].Text = strHeader[23];

                for (int i = 24; i < 39; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("rowspan", "2");
                    tcHeader[i].Text = strHeader[i];
                }

                tcHeader.Add(new TableHeaderCell());
                tcHeader[39].Attributes.Add("colspan", "4");
                tcHeader[39].Text = strHeader[39];

                for (int i = 40; i < 42; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("colspan", "2");
                    tcHeader[i].Text = strHeader[i];
                }

                for (int i = 42; i < 55; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("rowspan", "2");
                    tcHeader[i].Text = strHeader[i];
                }
                //Third Row
                for (int i = 55; i < 65; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Text = strHeader[i];
                }

                //bgcolor
                for (int i = 0; i < 65; i++)
                {
                    tcHeader[i].Attributes.Add("bgcolor", "#CCFFCC");
                }
                break;
            case DataControlRowType.DataRow:
                e.Row.Cells[5].Style.Add("word-break", "break-all");
                break;
        }
    }
    private DataTable GenerateDetail(DataTable OutsourceMUReportList)
    {

        OutsourceMUReportList.Columns.Add("CutWastageYPD");
        OutsourceMUReportList.Columns.Add("CutWastageYPDPer");
        OutsourceMUReportList.Columns.Add("SewingPercent");
        OutsourceMUReportList.Columns.Add("WashingPercent");
        OutsourceMUReportList.Columns.Add("YPD_Var");
        OutsourceMUReportList.Columns.Add("cutYPD");
        OutsourceMUReportList.Columns.Add("ShipYPD");
        OutsourceMUReportList.Columns.Add("ShipToCut");
        OutsourceMUReportList.Columns.Add("cut_to_receipt");
        OutsourceMUReportList.Columns.Add("ShipToRecv");
        OutsourceMUReportList.Columns.Add("cut_to_order");
        OutsourceMUReportList.Columns.Add("ShipToOrder");
        for (int i = 0; i < OutsourceMUReportList.Rows.Count; i++)
        {
            double issueQty = 0;

            double CutWastageYPD = 0;
            double CutWastageYPDPer = 0;
            double MAQty = 0;
            double RemnantQty = 0;
            double allocatedQty = 0;
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
            if (OutsourceMUReportList.Rows[i]["Issued"].ToString() != "")
            {
                issueQty = double.Parse(OutsourceMUReportList.Rows[i]["Issued"].ToString());
            }
            if (OutsourceMUReportList.Rows[i]["MA"].ToString() != "")
            {
                MAQty = double.Parse(OutsourceMUReportList.Rows[i]["MA"].ToString());
            }
            if (OutsourceMUReportList.Rows[i]["REMNANT"].ToString() != "")
            {
                RemnantQty = double.Parse(OutsourceMUReportList.Rows[i]["REMNANT"].ToString());
            }
            if (OutsourceMUReportList.Rows[i]["PULLOUT_QTY"].ToString() != "")
            {
                PullOutQty = double.Parse(OutsourceMUReportList.Rows[i]["PULLOUT_QTY"].ToString());
            }
            if (OutsourceMUReportList.Rows[i]["SAMPLE_QTY"].ToString() != "")
            {
                sampleQty = double.Parse(OutsourceMUReportList.Rows[i]["SAMPLE_QTY"].ToString());
            }
            if (OutsourceMUReportList.Rows[i]["PPO_YPD"].ToString() != "")
            {
                PPOMkrYpd = double.Parse(OutsourceMUReportList.Rows[i]["PPO_YPD"].ToString());
            }
            if (OutsourceMUReportList.Rows[i]["BULK_MKR_YPD"].ToString() != "")
            {
                BulkMkrYpd = double.Parse(OutsourceMUReportList.Rows[i]["BULK_MKR_YPD"].ToString());
            }
            if (OutsourceMUReportList.Rows[i]["GIVEN_CUT_YPD"].ToString() != "")
            {
                giveCutYpd = double.Parse(OutsourceMUReportList.Rows[i]["GIVEN_CUT_YPD"].ToString());
            }
            if (OutsourceMUReportList.Rows[i]["allocatedQty"].ToString() != "")
            {
                allocatedQty = double.Parse(OutsourceMUReportList.Rows[i]["allocatedQty"].ToString());
            }
            CutWastageYPD = issueQty - MAQty - RemnantQty;
            SewingDz = OutsourceMUReportList.Rows[i]["SewingDz"].ToString() == "" ? 0 : double.Parse(OutsourceMUReportList.Rows[i]["SewingDz"].ToString());
            WashingDz = OutsourceMUReportList.Rows[i]["WashingDz"].ToString() == "" ? 0 : double.Parse(OutsourceMUReportList.Rows[i]["WashingDz"].ToString());
            if (OutsourceMUReportList.Rows[i]["SHIP_QTY"].ToString() != "")
            {
                ShippedQty = double.Parse(OutsourceMUReportList.Rows[i]["SHIP_QTY"].ToString());
                if ((ShippedQty - PullOutQty) != 0)
                {
                    ShipYPD = ((issueQty - RemnantQty) / (ShippedQty - PullOutQty)).ToString("f2");
                }
                else
                {
                    ShipYPD = "";
                }
            }
            if (OutsourceMUReportList.Rows[i]["CUTQTY"].ToString() != "")
            {
                cutQty = double.Parse(OutsourceMUReportList.Rows[i]["CUTQTY"].ToString());
                if (cutQty != 0)
                {
                    cutYPD = (issueQty - RemnantQty) / cutQty;
                    ShipToCut = (ShippedQty + sampleQty - PullOutQty) / cutQty * 100;
                    SewingPercent = SewingDz / cutQty;
                    WashingPercent = WashingDz / cutQty;

                }
            }
            if (OutsourceMUReportList.Rows[i]["ORDERQTY"].ToString() != "")
            {
                orderQty = double.Parse(OutsourceMUReportList.Rows[i]["ORDERQTY"].ToString());
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
            OutsourceMUReportList.Rows[i]["CutWastageYPD"] = CutWastageYPD.ToString("f2");
            OutsourceMUReportList.Rows[i]["CutWastageYPDPer"] = CutWastageYPDPer.ToString("f2") + "%";
            OutsourceMUReportList.Rows[i]["SewingPercent"] = SewingPercent.ToString("f2") + "%";
            OutsourceMUReportList.Rows[i]["WashingPercent"] = WashingPercent.ToString("f2") + "%";
            OutsourceMUReportList.Rows[i]["YPD_Var"] = (BulkMkrYpd - PPOMkrYpd).ToString("f2");
            OutsourceMUReportList.Rows[i]["cutYPD"] = cutYPD.ToString("f2");
            OutsourceMUReportList.Rows[i]["ShipYPD"] = ShipYPD;
            OutsourceMUReportList.Rows[i]["ShipToCut"] = ShipToCut.ToString("f2") + "%";
            OutsourceMUReportList.Rows[i]["cut_to_receipt"] = cut_to_receipt.ToString("f2") + "%";
            OutsourceMUReportList.Rows[i]["ShipToRecv"] = ShipToRecv.ToString("f2") + "%";
            OutsourceMUReportList.Rows[i]["cut_to_order"] = cut_to_order.ToString("f2") + "%";
            OutsourceMUReportList.Rows[i]["ShipToOrder"] = ShipToOrder.ToString("f2") + "%";

            if (ShipToOrder < 95)
            {
                ship_order_count_95++;
            }
            else if (ShipToOrder < 100)
            {
                ship_order_count_100++;
                ship_order_sum += double.Parse(OutsourceMUReportList.Rows[i]["SHIP_QTY"].ToString() == "" ? "0" : OutsourceMUReportList.Rows[i]["SHIP_QTY"].ToString()) + double.Parse(OutsourceMUReportList.Rows[i]["SAMPLE_QTY"].ToString() == "" ? "0" : OutsourceMUReportList.Rows[i]["SAMPLE_QTY"].ToString());
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
        DataRow row = OutsourceMUReportList.NewRow();
        row["PO_NO"] = "";
        OutsourceMUReportList.Rows.Add(row);
        return OutsourceMUReportList;
    }

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
        double Total_Cut_Disc = 0;
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
        double Total_Pullout_Fab = 0;
        double Total_Short_Fab = 0;
        double Total_Defect_Fab = 0;
        double Total_Defect_Panel = 0;
        double Total_FOC = 0;
        double Issue_Remant = 0;
        double Total_UnaccFab = 0;


        String mu = "";
        int JO_NO = 1, MU = 11, ORDER_QTY = 12, allocatedQty = 13, Issued = 14, MA = 15, ShipYardage = 16, CutWastageYPD = 17, CutWastageYPDPer = 18, PulloutFab = 19, ShortFab = 20, DefectFab = 21, DefectPanel = 22, FOC = 23, Leftover = 24, REMNANT = 25, SRNQty = 26, RTW_QTY = 27, UnaccFab = 28, ORDERQTY = 29, CUTQTY = 30, CUT_DISC=31, SHIP_QTY = 32, SAMPLE_QTY = 33;
        int PULLOUT_QTY = 34, GMT_QTY_A = 35, GMT_QTY_B = 36, GMT_QTY_C = 37, GMT_QTY_TOTAL = 38, SewingDz = 39, SewingPercent = 40, WashingDz = 41, WashingPercent = 42, UnaccGmt = 43, PPO_YPD = 44, BULK_NET_YPD = 45, GIVEN_CUT_YPD = 46, BULK_MKR_YPD = 47;
        int YPD_Var = 48, cutYPD = 49, ShipYPD = 50, ShipToCut = 51, ShipToRecv = 52, ShipToOrder = 53, cut_to_receipt = 54, cut_to_order = 55, OVERSHIP = 56;
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
            Total_Issued += double.Parse(gvDetail.Rows[i].Cells[Issued].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[Issued].Text);
            Total_MA += double.Parse(gvDetail.Rows[i].Cells[MA].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[MA].Text);
            Total_Ship_Yardage += double.Parse(gvDetail.Rows[i].Cells[ShipYardage].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ShipYardage].Text);
            Total_Yds += double.Parse(gvDetail.Rows[i].Cells[CutWastageYPD].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[CutWastageYPD].Text);
            Issue_Remant += double.Parse(gvDetail.Rows[i].Cells[Issued].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[Issued].Text) - double.Parse(gvDetail.Rows[i].Cells[REMNANT].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[REMNANT].Text);
            Total_Leftover += double.Parse(gvDetail.Rows[i].Cells[Leftover].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[Leftover].Text);
            Total_Remnant += double.Parse(gvDetail.Rows[i].Cells[REMNANT].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[REMNANT].Text);
            Total_SRN += double.Parse(gvDetail.Rows[i].Cells[SRNQty].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[SRNQty].Text);
            Total_RTW += double.Parse(gvDetail.Rows[i].Cells[RTW_QTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[RTW_QTY].Text);
            Total_Order += double.Parse(gvDetail.Rows[i].Cells[ORDERQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ORDERQTY].Text);
            Total_Cut += double.Parse(gvDetail.Rows[i].Cells[CUTQTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[CUTQTY].Text);
            Total_Shipped += double.Parse(gvDetail.Rows[i].Cells[SHIP_QTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[SHIP_QTY].Text);
            Total_Sample += double.Parse(gvDetail.Rows[i].Cells[SAMPLE_QTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[SAMPLE_QTY].Text);
            Total_Pull_out += double.Parse(gvDetail.Rows[i].Cells[PULLOUT_QTY].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[PULLOUT_QTY].Text);
            Total_Grade_A += double.Parse(gvDetail.Rows[i].Cells[GMT_QTY_A].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[GMT_QTY_A].Text);
            Total_Grade_B += double.Parse(gvDetail.Rows[i].Cells[GMT_QTY_B].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[GMT_QTY_B].Text);
            Total_Grade_C += double.Parse(gvDetail.Rows[i].Cells[GMT_QTY_C].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[GMT_QTY_C].Text);
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
            Total_Pullout_Fab += double.Parse(gvDetail.Rows[i].Cells[PulloutFab].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[PulloutFab].Text);
            Total_Short_Fab += double.Parse(gvDetail.Rows[i].Cells[ShortFab].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[ShortFab].Text);
            Total_Defect_Fab += double.Parse(gvDetail.Rows[i].Cells[DefectFab].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[DefectFab].Text);
            Total_Defect_Panel += double.Parse(gvDetail.Rows[i].Cells[DefectPanel].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[DefectPanel].Text); ;
            Total_FOC += double.Parse(gvDetail.Rows[i].Cells[FOC].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[FOC].Text);
            Total_UnaccFab += double.Parse(gvDetail.Rows[i].Cells[UnaccFab].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[UnaccFab].Text);
            Total_Cut_Disc += double.Parse(gvDetail.Rows[i].Cells[CUT_DISC].Text == "&nbsp;" ? "0" : gvDetail.Rows[i].Cells[CUT_DISC].Text);

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

        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[JO_NO].Text = "";
        if (Total_Order != 0)
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[MU - 1].Text = (Total_MU / Total_Order).ToString("f2") + "%";
        }
        else
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[MU - 1].Text = "0.00%";
        }
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ORDER_QTY - 1].Text = Total_PPO_order_yds.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[allocatedQty - 1].Text = Total_Allocated.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[Issued - 1].Text = Total_Issued.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[MA - 1].Text = Total_MA.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipYardage - 1].Text = Total_Ship_Yardage.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[CutWastageYPD - 1].Text = Total_Yds.ToString("f2");
        if (Issue_Remant != 0)
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[CutWastageYPDPer - 1].Text = (Total_Yds / Issue_Remant * 100).ToString("f2") + "%";
        }
        else
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[CutWastageYPDPer - 1].Text = "0.00%";
        }
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[Leftover - 1].Text = Total_Leftover.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[REMNANT - 1].Text = Total_Remnant.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[SRNQty - 1].Text = Total_SRN.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[RTW_QTY - 1].Text = Total_RTW.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ORDERQTY - 1].Text = Total_Order.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[CUTQTY - 1].Text = Total_Cut.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[SHIP_QTY - 1].Text = Total_Shipped.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[SAMPLE_QTY - 1].Text = Total_Sample.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[PULLOUT_QTY - 1].Text = Total_Pull_out.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[GMT_QTY_A - 1].Text = Total_Grade_A.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[GMT_QTY_B - 1].Text = Total_Grade_B.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[GMT_QTY_C - 1].Text = Total_Grade_C.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[GMT_QTY_TOTAL - 1].Text = Total_Grade_total.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[SewingDz - 1].Text = Total_Sewing_DZ.ToString("f2");
        if (Total_Cut != 0)
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[SewingPercent - 1].Text = (Total_Sewing_DZ / Total_Cut).ToString("f2") + "%";
        }
        else
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[SewingPercent - 1].Text = "0.00%";
        }
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[WashingDz - 1].Text = Total_Washing_DZ.ToString("f2");
        if (Total_Cut != 0)
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[WashingPercent - 1].Text = (Total_Washing_DZ / Total_Cut).ToString("f2") + "%";
        }
        else
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[WashingPercent - 1].Text = "0.00%";
        }
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[UnaccGmt - 1].Text = Total_Unacc_Gmt.ToString("f2");
        if (Total_Order != 0)
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[PPO_YPD - 1].Text = (Total_PPO_MKR_YPD / Total_Order).ToString("f2");
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[BULK_NET_YPD - 1].Text = (Total_BULK_NET_YPD / Total_Order).ToString("f2");
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[GIVEN_CUT_YPD - 1].Text = (Total_GIVEN_CUT_YPD / Total_Order).ToString("f2");
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[BULK_MKR_YPD - 1].Text = (Total_BULK_MKR_YPD / Total_Order).ToString("f2");
        }
        else
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[PPO_YPD - 1].Text = "0.00%";
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[BULK_NET_YPD - 1].Text = "0.00%";
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[GIVEN_CUT_YPD - 1].Text = "0.00%";
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[BULK_MKR_YPD - 1].Text = "0.00%";
        }

        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[YPD_Var - 1].Text = Total_YPD_Var.ToString("f2");
        if (Total_Cut != 0)
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[cutYPD - 1].Text = (Issue_Remant / Total_Cut).ToString("f2");
        }
        else
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[cutYPD - 1].Text = "0.00%";
        }
        if (Total_Shipped - Total_Pull_out != 0)
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipYPD - 1].Text = (Issue_Remant / (Total_Shipped - Total_Pull_out)).ToString("f2");
        }
        else
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipYPD - 1].Text = "0.00%";
        }
        if (Total_Cut != 0)
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipToCut - 1].Text = ((Total_Shipped + Total_Sample - Total_Pull_out) / Total_Cut * 100).ToString("f2") + "%";
        }
        else
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipToCut - 1].Text = "0.00%";
        }
        if (Total_Allocated != 0)
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipToRecv - 1].Text = (Total_Ship_to_Receipt / Total_Allocated * 100).ToString("f2") + "%";
        }
        else
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipToRecv - 1].Text = "0.00%";
        }
        if (Total_Order != 0)
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipToOrder - 1].Text = (Total_Shipped / Total_Order * 100).ToString("f2") + "%";
        }
        else
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShipToOrder - 1].Text = "0.00%";
        }

        if (Total_Allocated != 0)
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[cut_to_receipt - 1].Text = (Total_Cut_to_Receipt / Total_Allocated * 100).ToString("f2") + "%";
        }
        else
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[cut_to_receipt - 1].Text = "0.00%";
        }
        if (Total_Order != 0)
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[cut_to_order - 1].Text = (Total_Cut / Total_Order * 100).ToString("f2") + "%";
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[OVERSHIP - 1].Text = (Total_Over_Ship / Total_Order).ToString("f2") + "%";
        }
        else
        {
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[cut_to_order - 1].Text = "0.00%";
            gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[OVERSHIP - 1].Text = "0.00%";
        }

        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[PulloutFab - 1].Text = Total_Pullout_Fab.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[ShortFab - 1].Text = Total_Short_Fab.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[DefectFab - 1].Text = Total_Defect_Fab.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[DefectPanel - 1].Text = Total_Defect_Panel.ToString("f2");
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[UnaccFab - 1].Text = Total_UnaccFab.ToString("f2");


        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[48].Text = "";
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[0].ColumnSpan = 2;
        gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[0].Text = "<b>Total/Weighted Average</b>";
    }

    //modified by LimML on 20150804 - fix bug cannot select by JO and GO
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.nodata.Visible = false;
        gvDetail.Visible = false;
        divSummary.Visible = false;
        string JoList = "";
        string[] value;
        DataTable JoNoList = new DataTable();
        string GoList = "";

        if (txtGoNo.Text != "")
        {
            value = txtGoNo.Text.Trim().Split(';');
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] != "")
                {
                    JoNoList = MESComment.OutsourceMUReportSql.GetJoNoList(value[i]);

                    GoList += "'" + value[i] + "',";
                    if (JoNoList.Rows.Count > 0)
                    {
                        txtJoNo.Text += JoNoList.Rows[0][0].ToString();
                    }
                }
            }
            GoList = GoList.Substring(0, GoList.Length - 1);

            if (JoNoList.Rows.Count > 0)
            {
                txtJoNo.Text = JoNoList.Rows[0][0].ToString().Substring(0, JoNoList.Rows[0][0].ToString().Length - 1);
            }
        }
        if (txtJoNo.Text != "")
        {
            value = txtJoNo.Text.Trim().Split(';');
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] != "")
                {
                    JoList += "'" + value[i] + "',";
                }
            }
            JoList = JoList.Substring(0, JoList.Length - 1);
            //txtJoNo.Text = "";
        }
        strRunNO = "";
        Random ro = new Random(1000);
        strRunNO = Convert.ToString(string.Format("{0:yy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(10).ToString();

        StandardOASInfo = MESComment.OutsourceMUReportSql.GetOASInfo(GoList, JoList, ddlFtyCd.SelectedItem.Value, ddlGarmentType.SelectedItem.Value, txtFromDate.Text, txtToDate.Text);

        if (StandardOASInfo.Rows.Count > 0)
        {
            if (cbChecked.Checked)
            {
                gvDetail.Visible = true;
            }
            divSummary.Visible = true;
            gvDetail.DataSource = GenerateDetail(GetMasterTable(GoList, JoList));
            gvDetail.DataBind();
            GenerateTotalRow();
        }
        else
        {
            this.nodata.Visible = true;
        }
    }
}
