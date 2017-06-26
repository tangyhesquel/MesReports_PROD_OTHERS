using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_WIPReduceQuantity : pPage
{
    public string JoNo = "";
    public string SCNo = "";
    public string GMTDate = "";
    public string CutDate = "";
    public string StyleNo = "";
    public string StyleDesc = "";
    public bool Status;
    public double OrderQty = 0;
    public double CutQty = 0;
    public double CutReduce = 0;
    public double ActualQty = 0;
    public double OverShortageQty = 0;
    public double OverShortagePercent = 0;
    public double Cut2DiscrepancyQty = 0;
    public double EmbDiscrepancyQty = 0;
    public double PrintDiscrepancyQty = 0;
    public string strOverShortagePercent = "";
    public string SummaryType = ""; //Added By ZouShiChang ON 2014.02.26 Start
    public string StatusType = "";//Added By ZouShiChang ON 2014.03.04 Start
    public string strShowJoStatus = ""; //Added By ZouShichang ON 2014.04.28 

    Boolean existFlag = false;
    Boolean SummaryByGO = false;
    String[] columnArray = { "ORDER_QTY", "CUT_QTY", "SSample_QTY", "CSample_QTY", "ReCut_QTY", "CUT_REDUCE", "ACTUAL_QTY", "OVER_SHORTAGE_PERCENT", "OVER_SHORTAGE_QTY", "CUT_DISCREPANCY_QTY", "EMB_DISCREPANCY_QTY", "PRINT_DISCREPANCY_QTY", "Final_Qty" };
    string[] displayArray = { "Order-Qty", "Cut-Qty", "SSample-Qty", "CSample-Qty", "ReCut-QTY", "Cut-Reduce", "Actual-Qty", "Over/Shortage Percent ", "Over/Shortage Qty", "Cut2DiscrepancyQty", "EmbDiscrepancyQty", "PrintDiscrepancyQty", "FinalQty" };

    protected void Page_Load(object sender, EventArgs e)
    {
        string FactoryCd = "";
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                FactoryCd = Request.QueryString["site"].ToString();
            }
            else
            {
                FactoryCd = "GEG";
            }
            ddlFactory.SelectedValue = FactoryCd;
        }
        if (this.DDSummaryBy.SelectedValue.Contains("GO"))
        {
            this.SummaryByGO = true;
        }

        if (Request.QueryString["JoNo"] != null)
        {
            JoNo = Request.QueryString["JoNo"].ToString();
        }
        if (Request.QueryString["GoNo"] != null)
        {
            SCNo = Request.QueryString["GoNo"].ToString();
        }
        if (Request.QueryString["SummaryType"] != null)
        {
            SummaryType = Request.QueryString["SummaryType"].ToString();
        }
        if (Request.QueryString["Status"] != null)
        {
            StatusType = Request.QueryString["Status"].ToString();
        }



        if (!IsPostBack)
        {
            ddlFactory.DataSource = MESComment.MesRpt.GetFactoryCd();
            ddlFactory.DataTextField = "FACTORY_ID";
            ddlFactory.DataValueField = "FACTORY_ID";
            ddlFactory.DataBind();
            if (FactoryCd.ToUpper().Equals("EAV") || FactoryCd.ToUpper().Equals("EGV"))
            {
                this.DDSummaryBy.SelectedValue = "GO";
            }

            this.txtJoNo.Text = JoNo.ToString();
            this.txtGO.Text = SCNo.ToString();
            
            //Added by MF on 20160419, JO Combination - eMI Report displayed
            if (SummaryType.ToString() == "3")
            {
                this.DDSummaryBy.SelectedValue = "LOT";
            }
            else
            {
                this.DDSummaryBy.SelectedValue = "JO";
            }
            //End of added by MF on 20160419, JO Combination - eMI report displayed

            //this.DDSummaryBy.SelectedValue = SummaryType.ToString();
            if (JoNo.ToString() != "" || SCNo.ToString() != "" || SummaryType.ToString() != "")
            {
                queryDiv.Visible = false;
                this.btnQuery_Click1(null, null);
            }

        }
    }



    protected void btnQuery_Click1(object sender, EventArgs e)
    {
        this.gvDetail.InnerHtml = "";
        this.divMeg.InnerHtml = "";
        if (Check())
        {
            SetQuery();
        }
        else
        {
            this.divMeg.InnerHtml = "<table width='100%'  style='color:Red; font-size:12px'><tr><td align='left'>PLS input GO/JO ! </td></tr></table>";
        }
    }

    private Boolean Check()
    {
        if (this.txtJoNo.Text.Equals(""))
        {
            if (this.txtGO.Text.Equals(""))
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
            return true;
        }
    }

    private void SetQuery()
    {
        string JO = txtJoNo.Text;
        string GO = txtGO.Text;
        string FactoryCD = ddlFactory.SelectedValue.ToString();
        if (this.SummaryByGO)
        {
            SummaryByGONO(GO, FactoryCD);
        }
        else
        {
            //Modified by MF on 20150812, JO Combination
            if (DDSummaryBy.SelectedValue == "LOT")
                SummaryByLOT(JO, GO, FactoryCD);
            else
                SummaryByJONO(JO, GO, FactoryCD);
            //End of modified by MF on 20150812, JO Combination
        }
    }

    private void SummaryByGONO(string GO, string FactoryCD)
    {

        DataTable dtHeader = GetReduceQuantityHeadByGO(GO, FactoryCD);
        if (dtHeader.Rows.Count > 0)
        {

            ShowGOHeader(dtHeader);
            DataTable dtSize = MESComment.wipReduceQuantitySql.GetReduceQuantitySizeByGO(GO);
            DataTable dtColor = MESComment.wipReduceQuantitySql.GetReduceQuantityColorByGO(GO);
            DataTable dtQuantity = MESComment.wipReduceQuantitySql.GetReduceQuantityByGO(GO, FactoryCD);
            if (dtSize.Rows.Count > 0 && dtColor.Rows.Count > 0 && dtQuantity.Rows.Count > 0)
            {
                ShowQuantityDetailByGO(dtSize, dtColor, dtQuantity);
            }
            bool IsGO = true;
            DataTable dtMissColor = MESComment.wipReduceQuantitySql.GetCanceledColor_Size_CUTQTY(GO, IsGO, FactoryCD);
            if (dtMissColor.Rows.Count > 0)
            {
                //ShowMissingColorDetailByGO(dtMissColor);
                ShowMissingColorDetail(dtMissColor);
            }
        }
        else
        {
            this.gvDetail.InnerHtml = "<table width='100%'  style='color:Red; font-size:large'><tr><td align='center'><b>No Data</b></td></tr></table>";
        }
    }

    //Added by MF on 20150812, JO Combination
    private void SummaryByLOT(string JO, string GO, string FactoryCD)
    {

        DataTable DT = MESComment.MesRpt.GetJO(JO, GO, FactoryCD);

        //DataTable DT = GetJoByLOT(JO, GO, FactoryCD);

        if (DT.Rows.Count > 0)
        {

            DataTable dtGoNoInfo = GetGoNoInfo(DT.Rows[0]["SC_NO"].ToString().Trim());
            ShowGoNoInfo(dtGoNoInfo);

            if (GO == "")
            {
                GO = DT.Rows[0]["SC_NO"].ToString().Trim();
            }
            DataTable dtJoNoInfo = GetGoNoJoInfoByLOT(GO, JO);
            ShowJoNoInfoByLOT(dtJoNoInfo);
            foreach (DataRow dr in DT.Rows)
            {
                JO = dr["JO_NO"].ToString().Trim();


                DataTable dtOverShortagePercent = GetJoInfo(JO);
                if (dtOverShortagePercent.Rows.Count > 0)
                {
                    strOverShortagePercent = dtOverShortagePercent.Rows[0][5].ToString().Trim();
                }

                DataTable dtHeader = MESComment.wipReduceQuantitySql.GetReduceQuantityHead(JO);
                if (dtHeader.Rows.Count > 0)
                {
                    ShowJOHeader(dtHeader);
                }

                DataTable dtSize = MESComment.wipReduceQuantitySql.GetReduceQuantitySize(JO);
                DataTable dtColor = MESComment.wipReduceQuantitySql.GetReduceQuantityColor(JO, false);
                if (dtColor.Rows.Count > 0)
                {
                    ShowColorDetail(dtColor);
                }
                //通过添加bool变量来控制获取的字段;因为获取全字段的话,会获取到重复Color,对 GetReduceQuantity 造成数据重复错误;
                DataTable dtColor_Just_Color = MESComment.wipReduceQuantitySql.GetReduceQuantityColor(JO, true);
                DataTable dtQuantity = GetReduceQuantityByLOT(JO, FactoryCD);
                if (dtSize.Rows.Count > 0 && dtColor_Just_Color.Rows.Count > 0 && dtQuantity.Rows.Count > 0)
                {
                    ShowQuantityDetailByLOT(dtSize, dtColor_Just_Color, dtQuantity, JO, FactoryCD);
                    //ShowQuantityDetail(dtSize, dtColor, dtQuantity);
                }
                bool IsGO = false;

                DataTable dtMissColor = MESComment.wipReduceQuantitySql.GetCanceledColor_Size_CUTQTY(JO, IsGO, FactoryCD);
                if (dtMissColor.Rows.Count > 0)
                {
                    ShowMissingColorDetail(dtMissColor);
                }
                this.gvDetail.InnerHtml += " <tr style='page-break-after: always'><td> </td></tr>"; //Added by Jin Song - Fix bug 7/21/14
            }
        }
        else
        {
            this.gvDetail.InnerHtml = "<table width='100%'  style='color:Red; font-size:large'><tr><td align='center'><b>No Data</b></td></tr></table>";
        }
    }

    //Added by MF on 20150812, JO Combination
    private void ShowJoNoInfoByLOT(DataTable JoNoInfo)
    {
        string seq = JoNoInfo.Rows[0]["SEQ"].ToString();
        int i = 0;

        string strJoNo = "", strOrderQty = "", strActualQty = "", strOverShortShip = "", strOverShortagePecrcent = "";
        string strHtml = "";
        this.gvDetail.InnerHtml += "<tr><td> <table width='600px' border='1' cellspacing='0' cellpadding='0' style='font-size: 12px; border-collapse: collapse'>";
        strJoNo = "<tr class='tr2style'><td width='130' >C/T#:</td>";
        strOrderQty = "<tr><td width='130'>Order Qty:</td>";
        strActualQty = "<tr><td width='130' >Actual Qty:</td>";
        strOverShortShip = "<tr style='font-weight:bold'><td width='130' >Over/Short Ship%</td>";
        strOverShortagePecrcent = "<tr style='font-weight:bold'><td width='130' >Over/Shortage Percent</td>";

        for(int j=0;j<JoNoInfo.Rows.Count;j++)
        {
            if (JoNoInfo.Rows[j]["SEQ"].ToString() == seq && (i % 10 != 0 || j == 0))
            {
                strJoNo += "<td >" + JoNoInfo.Rows[j]["DISPLAY_JO"].ToString() + "</td>";
                strOrderQty += "<td >" + JoNoInfo.Rows[j]["ORDER_QTY"].ToString() + "</td>";
                strActualQty += "<td >" + JoNoInfo.Rows[j]["Actual_QTY"].ToString() + "</td>";
                strOverShortShip += "<td  >" + JoNoInfo.Rows[j]["OVER_SHORT_SHIP"].ToString() + "</td>";
                strOverShortagePecrcent += "<td >" + JoNoInfo.Rows[j]["OVER_SHORTAGE_PERCENT"].ToString() + "</td>";
            }
            else if ((i % 10 == 0 && j <= JoNoInfo.Rows.Count - 1 && j!=0) || JoNoInfo.Rows[j]["SEQ"].ToString() != seq)
            {
                strJoNo += "</tr>";
                strOrderQty += "</tr>";
                strActualQty += "</tr>";
                strOverShortShip += "</tr>";
                strOverShortagePecrcent += "</tr>";
                strHtml += strJoNo + strOrderQty + strActualQty + strOverShortShip + strOverShortagePecrcent;
                strJoNo = "<tr class='tr2style'><td width='130' >C/T#:</td>";
                strOrderQty = "<tr><td width='130'>Order Qty:</td>";
                strActualQty = "<tr><td width='130' >Actual Qty:</td>";
                strOverShortShip = "<tr style='font-weight:bold'><td width='130' >Over/Short Ship%</td>";
                strOverShortagePecrcent = "<tr style='font-weight:bold'><td width='130' >Over/Shortage Percent</td>";

                if (i % 10 == 0 || JoNoInfo.Rows[j]["SEQ"].ToString() != seq)
                {
                    seq = JoNoInfo.Rows[j]["SEQ"].ToString();
                    strJoNo += "<td >" + JoNoInfo.Rows[j]["DISPLAY_JO"].ToString() + "</td>";
                    strOrderQty += "<td >" + JoNoInfo.Rows[j]["ORDER_QTY"].ToString() + "</td>";
                    strActualQty += "<td >" + JoNoInfo.Rows[j]["Actual_QTY"].ToString() + "</td>";
                    strOverShortShip += "<td  >" + JoNoInfo.Rows[j]["OVER_SHORT_SHIP"].ToString() + "</td>";
                    strOverShortagePecrcent += "<td >" + JoNoInfo.Rows[j]["OVER_SHORTAGE_PERCENT"].ToString() + "</td>";
                }
                i = 0;
            }
            i++;
        }
        this.gvDetail.InnerHtml += strHtml + strJoNo + "</tr>" + strOrderQty + "</tr>" + strActualQty + "</tr>" + strOverShortShip + "</tr>" + strOverShortagePecrcent + "</tr></table></td></tr>";

    }


    //Added by MF on 20150812, JO Combination
    private void ShowQuantityDetailByLOT(DataTable dtSize, DataTable dtColor, DataTable dtQuantity, string JO, string FactoryCD)
    {
        double OrderQty = 0;
        double CutQty = 0;
        double CutReduce = 0;
        double ActualQty = 0;
        double OverShortageQty = 0;
        //double OverShortagePercent = 0;
        double Cut2DiscrepancyQty = 0;
        double EmbDiscrepancyQty = 0;
        double PrintDiscrepancyQty = 0;
        double FinalQty = 0;

        DataTable dtQuantitySelect = new DataTable();
        DataRow[] dr;
        dtQuantitySelect = dtQuantity.Clone();
        int diss = 0;

        this.gvDetail.InnerHtml += "<tr><td>";
        this.gvDetail.InnerHtml += "<table width='95%' border='1' cellspacing='0' cellPadding='0' style='font-size: 12px; border-collapse: collapse'>"; //Change width - Fix bug 7/21/2014
        for (int i = 0; i < dtQuantity.Columns.Count - 3; i++)
        {
            if (i == 0)
            {
                this.gvDetail.InnerHtml += "<th class='tr2style' style='font-weight:bold;font-style:italic; Font-Size:14px'>Colour/Size</th>";
            }
            else
            {
                this.gvDetail.InnerHtml += "<th class='tr2style' style='font-weight:bold;font-style:italic; Font-Size:14px'>" + dtQuantity.Columns[i].ColumnName.ToString() + "</th>";
            }
        }
        this.gvDetail.InnerHtml += "<th class='tr2style' style='font-weight:bold;font-style:italic; Font-Size:14px'>Lot No</th>";
        this.gvDetail.InnerHtml += " </tr>";

        for (int h = 0; h < dtColor.Rows.Count; h++)
        {
            diss = 0;
            dr = dtQuantity.Select("COLOR_CD='" + dtColor.Rows[h][0].ToString() + "'");
            dtQuantitySelect.Clear();
            //RowSpan=dtQuantitySelect.Count();
            foreach (DataRow row in dr)  // 将查询的结果添加到dt中；
            {
                dtQuantitySelect.Rows.Add(row.ItemArray);
            }
            //rowspan
            if (h % 2 == 0)
            {
                gvDetail.InnerHtml += "<tr bgColor='white'><td rowspan='" + dr.Count() + "' width='150'>" + dtColor.Rows[h][0].ToString() + "(" + dtColor.Rows[h][1].ToString() + ")" + "</td>";
            }
            else
            {
                gvDetail.InnerHtml += "<tr bgColor='#f2f2f2'><td rowspan='" + dr.Count() + "' width='150'>" + dtColor.Rows[h][0].ToString() + "(" + dtColor.Rows[h][1].ToString() + ")" + "</td>";
            }

            //his.gvDetail.InnerHtml += "<tr><td rowspan= " + dr.Count() + ">" + dtColor.Rows[h][0].ToString() + "</td>"; 
            for (int i = 0; i < dtQuantitySelect.Rows.Count; i++)
            {
                switch (dtQuantitySelect.Rows[i]["Remark"].ToString().Trim().Substring(0, dtQuantitySelect.Rows[i]["Remark"].ToString().Trim().Length - 1))
                {
                    case "Order-Qt":
                        if (dtQuantitySelect.Rows[i]["LOT_NO"].ToString() == "")
                            OrderQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "Cut-Qt":
                        CutQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "Csample-Qt":
                        CutQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "SSample-Qt":
                        CutQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "Re-Cut":
                        CutQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "Cut-Reduc":
                        CutReduce += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "Actual-Qt":
                        if (dtQuantitySelect.Rows[i]["LOT_NO"].ToString() == "")
                            ActualQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "Over/Shortage Qt":
                        OverShortageQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "Cut2DiscrepancyQt":
                        Cut2DiscrepancyQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "EmbDiscrepancyQt":
                        EmbDiscrepancyQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "PrintDiscrepancyQt":
                        PrintDiscrepancyQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "FinalQt":
                        FinalQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;

                }

                for (int j = 1; j < dtQuantitySelect.Columns.Count - 2; j++)
                {

                    string style = "";
                    if (dtQuantitySelect.Rows[i]["Remark"].ToString().Trim().Equals("Actual-Qty") || dtQuantitySelect.Rows[i]["Remark"].ToString().Trim().Equals("FinalQty"))
                    {

                        style = "style='font-weight:bold;font-style:italic; Font-Size:14px'";

                    }
                    if (dtQuantitySelect.Rows[i]["Remark"].ToString().Trim().Equals("Over/Shortage Percent") && dtQuantitySelect.Rows[i][j].ToString().Length > 1)
                    {

                        if (dtQuantitySelect.Rows[i][j].ToString().Substring(0, 1) == "-")
                        {
                            style = "style='font-weight:bold;font-style:italic; Font-Size:14px;COLOR:RED'";
                        }
                        else
                        {
                            style = "style='font-weight:bold;font-style:italic; Font-Size:14px;COLOR:BLUE'";
                        }
                    }

                    if (j == dtQuantitySelect.Columns.Count - 3)
                    {
                        if (h % 2 == 0)
                        {
                            if (dtQuantitySelect.Rows[i][j].ToString() != "")
                            {
                                if (diss == 0)
                                {
                                    gvDetail.InnerHtml += "<td bgColor='white' rowspan='3'>" + dtQuantitySelect.Rows[i][j].ToString() + "</td>";
                                }
                                diss += 1;

                                if (diss == 3)
                                {
                                    diss = 0;
                                }
                            }
                            else
                            {
                                gvDetail.InnerHtml += "<td bgColor='white'>" + dtQuantitySelect.Rows[i][j].ToString() + "</td>";
                                diss = 0;
                            }
                        }
                        else
                        {
                            if (dtQuantitySelect.Rows[i][j].ToString() != "")
                            {
                                if (diss == 0)
                                {
                                    gvDetail.InnerHtml += "<td bgColor='#f2f2f2' rowspan='3'>" + dtQuantitySelect.Rows[i][j].ToString() + "</td>";
                                }
                                diss += 1;

                                if (diss == 3)
                                {
                                    diss = 0;
                                }
                            }
                            else
                            {
                                gvDetail.InnerHtml += "<td bgColor='#f2f2f2'>" + dtQuantitySelect.Rows[i][j].ToString() + "</td>";
                                diss = 0;
                            }
                        }
                    }
                    else
                    {
                        if (h % 2 == 0)
                        {
                            this.gvDetail.InnerHtml += "<td bgColor='white' " + style + " >" + dtQuantitySelect.Rows[i][j].ToString() + "</td>";
                        }
                        else
                        {
                            this.gvDetail.InnerHtml += "<td bgColor='#f2f2f2' " + style + ">" + dtQuantitySelect.Rows[i][j].ToString() + "</td>";
                        }
                    }
                }
                this.gvDetail.InnerHtml += "</tr>";
            }

        }
        gvDetail.InnerHtml += "</table></td></tr>";

        this.gvDetail.InnerHtml += " <tr><td>&nbsp;</td></tr>";//空一行,美观;
        ShowTotal(OrderQty, CutQty, CutReduce, ActualQty, OverShortageQty, strOverShortagePercent, Cut2DiscrepancyQty, EmbDiscrepancyQty, PrintDiscrepancyQty, FinalQty);
    }


    //Added by MF on 20150812, JO Combination
    public static DataTable GetReduceQuantityByLOT(string JoNo, string FactoryCd)
    {
        string SQL = @"EXEC PROC_ACTUAL_CUTTING_REPORT_BYLOT '" + FactoryCd + "','" + JoNo + "'";

        return MESComment.DBUtility.GetTable(SQL, "MES");
    }

    //Added by MF on 20150812, JO Combination
    private static DataTable GetGoNoJoInfoByLOT(string GoNo, string JoNo)
    {
        string SQL = @"  DECLARE @HEADER TABLE (SC_NO NVARCHAR(20),COMBINE_JO_NO NVARCHAR(20),JO_NO NVARCHAR(20),LOT_NO INT, DISPLAY_JO NVARCHAR(20), ORDER_QTY INT,ACTUAL_QTY INT,
	                        OVER_SHORT_SHIP NVARCHAR(20),OVER_SHORTAGE_PERCENT NVARCHAR(20), CUT_REDUCE INT, SEQ INT, C_SEQ INT)

                        DECLARE @MAX_NO INT

                        INSERT INTO @HEADER
                        SELECT A.SC_NO, B.COMBINE_JO_NO, A.JO_NO, A.LOT_NO, A.JO_NO, ISNULL(A.TOTAL_QTY,0) AS ORDER_QTY, ISNULL(C.ACTUAL_QTY,0), 
	                        '+' + CAST(E.PERCENT_OVER_ALLOWED AS NVARCHAR(10)) + '-' + CAST(E.PERCENT_SHORT_ALLOWED AS NVARCHAR(10)), 
	                        CASE WHEN A.TOTAL_QTY = 0 THEN '' ELSE CAST (CONVERT(DECIMAL(18, 3), ROUND(( ISNULL(D.CUT_QTY, 0)
                                                                      - ISNULL(F.CUT_REDUCE, 0)
                                                                      - ISNULL(A.total_qty, 0) ) * 100
                                                                    / A.total_qty, 3)) AS NVARCHAR(8))
                                + '%' END AS OVER_SHORTAGE_PERCENT , 
	                        ISNULL(F.CUT_REDUCE,0), ROW_NUMBER() OVER (ORDER BY A.LOT_NO) AS SEQ, 1
                        FROM JO_HD A WITH(NOLOCK)
	                        LEFT JOIN JO_COMBINE_MAPPING B WITH(NOLOCK) ON A.JO_NO = B.ORIGINAL_JO_NO
	                        LEFT JOIN ( SELECT JOB_ORDER_NO, SUM(QTY) AS ACTUAL_QTY FROM CUT_BUNDLE_HD WITH(NOLOCK) WHERE STATUS='Y' GROUP BY JOB_ORDER_NO
		                        ) C ON A.JO_NO = C.JOB_ORDER_NO
	                        LEFT JOIN ( SELECT JOB_ORDER_NO, SUM(QTY) AS CUT_QTY FROM CUT_BUNDLE_HD WITH(NOLOCK) WHERE STATUS='Y' AND TRX_TYPE='NM' GROUP BY JOB_ORDER_NO
		                        ) D ON A.JO_NO = D.JOB_ORDER_NO
	                        LEFT JOIN SC_LOT E WITH(NOLOCK) ON A.SC_NO = E.SC_NO AND A.LOT_NO = E.LOT_NO
	                        LEFT JOIN ( SELECT JOB_ORDER_NO, SUM(REDUCE_QTY) AS CUT_REDUCE FROM CUT_BUNDLE_REDUCE_TRX WITH(NOLOCK) GROUP BY JOB_ORDER_NO
	                        ) F ON A.JO_NO = F.JOB_ORDER_NO
                        WHERE A.SC_NO = '" + GoNo + @"' AND A.STATUS <> 'D' 
	                        AND NOT EXISTS (SELECT ORIGINAL_JO_NO FROM JO_COMBINE_MAPPING CB WITH(NOLOCK) WHERE CB.ORIGINAL_JO_NO = A.JO_NO)
                        ORDER BY A.LOT_NO


                        INSERT INTO @HEADER
                        SELECT A.SC_NO, B.COMBINE_JO_NO, A.JO_NO, A.LOT_NO, A.JO_NO, ISNULL(A.TOTAL_QTY,0) AS ORDER_QTY, 0, 
	                        '+' + CAST(E.PERCENT_OVER_ALLOWED AS NVARCHAR(10)) + '-' + CAST(E.PERCENT_SHORT_ALLOWED AS NVARCHAR(10)), 
	                        '', 0, 0, 1
                        FROM JO_HD A WITH(NOLOCK)
	                        LEFT JOIN JO_COMBINE_MAPPING B WITH(NOLOCK) ON A.JO_NO = B.ORIGINAL_JO_NO
	                        LEFT JOIN ( SELECT JOB_ORDER_NO, SUM(QTY) AS ACTUAL_QTY FROM CUT_BUNDLE_HD WITH(NOLOCK) WHERE STATUS='Y' GROUP BY JOB_ORDER_NO
		                        ) C ON A.JO_NO = C.JOB_ORDER_NO
	                        LEFT JOIN SC_LOT E WITH(NOLOCK) ON A.SC_NO = E.SC_NO AND A.LOT_NO = E.LOT_NO
                        WHERE A.SC_NO = '" + GoNo + @"' AND A.STATUS <> 'D' 
	                        AND EXISTS (SELECT ORIGINAL_JO_NO FROM JO_COMBINE_MAPPING CB WITH(NOLOCK) WHERE CB.ORIGINAL_JO_NO = A.JO_NO)
                        ORDER BY A.LOT_NO

                        UPDATE A SET A.SEQ = B.SEQ
                        FROM @HEADER A INNER JOIN @HEADER B ON A.COMBINE_JO_NO=B.JO_NO


                        --UPDATE COMBINE JO OVER_SHORT_SHIP
                        UPDATE A SET A.OVER_SHORT_SHIP = '+' + CAST(B.PERCENT_OVER_ALLOWED AS NVARCHAR(10)) + '-' + CAST(B.PERCENT_SHORT_ALLOWED AS NVARCHAR(10))
                        FROM @HEADER A INNER JOIN 
                        (
                        SELECT A.COMBINE_JO_NO,CAST(ROUND(((SUM(B.TOTAL_QTY*(1+C.PERCENT_OVER_ALLOWED/100))/SUM(B.TOTAL_QTY))-1)*100,2)AS DECIMAL(38,2)) AS PERCENT_OVER_ALLOWED, 
                        CAST(ROUND(((SUM(B.TOTAL_QTY*(1-C.PERCENT_SHORT_ALLOWED/100))/SUM(B.TOTAL_QTY))-1)*-100,2)AS DECIMAL(38,2)) AS PERCENT_SHORT_ALLOWED
                        FROM JO_COMBINE_MAPPING A WITH(NOLOCK)
                        INNER JOIN JO_HD B WITH(NOLOCK) ON A.ORIGINAL_JO_NO = B.JO_NO
                        INNER JOIN SC_LOT C WITH(NOLOCK) ON B.SC_NO = C.SC_NO AND B.LOT_NO = C.LOT_NO
                        GROUP BY A.COMBINE_JO_NO)B ON A.JO_NO = B.COMBINE_JO_NO
                        WHERE A.JO_NO LIKE '%CB%'


                        --UPDATE COMBINE JO OVER_SHORTAGE_PERCENT
                        UPDATE A SET OVER_SHORTAGE_PERCENT = CASE WHEN A.ORDER_QTY = 0 THEN '' ELSE CAST (CONVERT(DECIMAL(18, 3), ROUND(( ISNULL(D.CUT_QTY, 0)
                                                                      - ISNULL(F.CUT_REDUCE, 0) - ISNULL(A.ORDER_QTY, 0) ) * 100
                                                                    / A.ORDER_QTY, 3)) AS NVARCHAR(8)) + '%' END
                        FROM @HEADER A 
                        LEFT JOIN ( SELECT JOB_ORDER_NO, SUM(QTY) AS CUT_QTY FROM CUT_BUNDLE_HD WITH(NOLOCK) WHERE STATUS='Y' AND TRX_TYPE='NM' GROUP BY JOB_ORDER_NO
		                        ) D ON A.JO_NO = D.JOB_ORDER_NO
                        LEFT JOIN ( SELECT JOB_ORDER_NO, SUM(REDUCE_QTY) AS CUT_REDUCE FROM CUT_BUNDLE_REDUCE_TRX WITH(NOLOCK) GROUP BY JOB_ORDER_NO
	                        ) F ON A.JO_NO = F.JOB_ORDER_NO
                        WHERE A.JO_NO LIKE '%CB%'


                        --UPDATE ACTUAL_QTY FOR ORIGINAL_JO
                        UPDATE @HEADER SET A.ACTUAL_QTY = A.ACTUAL_QTY + B.QTY
                        FROM @HEADER A INNER JOIN 
                        (SELECT COMBINE_JO_NO, ORIGINAL_JO_NO, SUM(QTY) AS QTY FROM PRD_DISTR_COMBINE_TO_ORIGINAL WITH(NOLOCK)
                        WHERE PROCESS_CD = 'CUT' AND TRX_TYPE='ACTUAL_CUT'
                        GROUP BY COMBINE_JO_NO, ORIGINAL_JO_NO
                        ) B ON A.JO_NO = B.ORIGINAL_JO_NO AND A.COMBINE_JO_NO = B.COMBINE_JO_NO


                        --UPDATE OVER_SHORTAGE_PERCENT AND CUT_REDUCE FOR ORIGINAL_JO
                        UPDATE A SET A.CUT_REDUCE = B.CUT_REDUCE
                        FROM @HEADER A INNER JOIN 
                        (SELECT COMBINE_JO_NO, ORIGINAL_JO_NO, SUM(QTY) AS CUT_REDUCE FROM PRD_DISTR_COMBINE_TO_ORIGINAL WITH(NOLOCK)
                        WHERE QTY < 0 AND PROCESS_CD='CUT'
                        GROUP BY COMBINE_JO_NO, ORIGINAL_JO_NO
                        ) B ON A.JO_NO = B.ORIGINAL_JO_NO AND A.COMBINE_JO_NO = B.COMBINE_JO_NO

                        UPDATE @HEADER 
                        SET OVER_SHORTAGE_PERCENT = CASE WHEN ORDER_QTY = 0 THEN '' ELSE CAST (CONVERT (DECIMAL(38,3) ,ROUND((CAST(ACTUAL_QTY AS DECIMAL(38,3)) - 
	                        CAST(ORDER_QTY AS DECIMAL(38,3))) * 100.000 / CAST(ISNULL(ORDER_QTY,1) AS DECIMAL(38,3)), 3)) AS NVARCHAR(8)) + '%' END
                        WHERE OVER_SHORTAGE_PERCENT = ''

                        SET @MAX_NO = (SELECT MAX(SEQ)+1 FROM @HEADER)

                        --UPDATE LOT_NO/JO_NO
                        UPDATE @HEADER SET DISPLAY_JO = JO_NO WHERE COMBINE_JO_NO IS NOT NULL
                        UPDATE @HEADER SET JO_NO = COMBINE_JO_NO WHERE COMBINE_JO_NO IS NOT NULL
                        UPDATE @HEADER SET SEQ = @MAX_NO WHERE COMBINE_JO_NO IS NULL AND JO_NO NOT LIKE '%CB%'
                        UPDATE @HEADER SET C_SEQ = 0 WHERE DISPLAY_JO LIKE '%CB%'

                        SELECT SC_NO, JO_NO, DISPLAY_JO, ORDER_QTY,ACTUAL_QTY,OVER_SHORT_SHIP,OVER_SHORTAGE_PERCENT,CUT_REDUCE,SEQ FROM @HEADER WHERE 1=1";
        if (GoNo != "")
        {
            SQL += @" AND JO_NO IN (SELECT JO_NO FROM JO_HD WHERE STATUS<>'D' AND SC_NO='" + GoNo + @"')";
        }
        if (JoNo != "")
        {
            SQL += @" AND  JO_NO IN (
                        SELECT  *
                        FROM    FN_SPLIT_STRING_TB('" + JoNo + @"',',') )";
        }
        SQL += @"  ORDER BY SEQ, C_SEQ";
        return MESComment.DBUtility.GetTable(SQL, "MES");
    }

    //Added by MF on 20150812, JO Combination
    private static DataTable GetJoByLOT(string JoNo, string GoNo, string Factroy_CD)
    {
        string SQL = "";
        SQL += " SELECT SC_NO,JO_NO FROM JO_HD WHERE STATUS<>'D' AND NOT EXISTS (SELECT ORIGINAL_JO_NO FROM JO_COMBINE_MAPPING WITH(NOLOCK) WHERE ORIGINAL_JO_NO=JO_HD.JO_NO) ";
        if (!GoNo.Equals(""))
        {
            SQL += " AND SC_NO = '" + GoNo + "'";
        }
        if (!JoNo.Equals(""))
        {
            //SQL += "AND JO_NO = '" + JoNo + "' ";
            SQL += " AND JO_NO IN (SELECT * FROM FN_SPLIT_STRING_TB('" + JoNo + "',',')) ";
        }
        SQL += " AND FACTORY_CD= '" + Factroy_CD + "'";
        return MESComment.DBUtility.GetTable(SQL, "MES");
    }

    private void SummaryByJONO(string JO, string GO, string FactoryCD)
    {

        DataTable DT = MESComment.MesRpt.GetJO(JO, GO, FactoryCD);

        if (DT.Rows.Count > 0)
        {

            DataTable dtGoNoInfo = GetGoNoInfo(DT.Rows[0]["SC_NO"].ToString().Trim());
            ShowGoNoInfo(dtGoNoInfo);
            DataTable dtJoNoInfo = GetGoNoJoInfo(GO, JO);
            ShowJoNoInfo(dtJoNoInfo);
            foreach (DataRow dr in DT.Rows)
            {
                JO = dr["JO_NO"].ToString().Trim();
             
                DataTable dtOverShortagePercent = GetJoInfo(JO);
                if (dtOverShortagePercent.Rows.Count > 0)
                {
                    strOverShortagePercent = dtOverShortagePercent.Rows[0][5].ToString().Trim();
                }

                DataTable dtHeader = MESComment.wipReduceQuantitySql.GetReduceQuantityHead(JO);
                if (dtHeader.Rows.Count > 0)
                {
                    ShowJOHeader(dtHeader);
                }

                DataTable dtSize = MESComment.wipReduceQuantitySql.GetReduceQuantitySize(JO);
                DataTable dtColor = MESComment.wipReduceQuantitySql.GetReduceQuantityColor(JO, false);
                if (dtColor.Rows.Count > 0)
                {
                    ShowColorDetail(dtColor);
                }
                //通过添加bool变量来控制获取的字段;因为获取全字段的话,会获取到重复Color,对 GetReduceQuantity 造成数据重复错误;
                DataTable dtColor_Just_Color = MESComment.wipReduceQuantitySql.GetReduceQuantityColor(JO, true);
                DataTable dtQuantity = MESComment.wipReduceQuantitySql.GetReduceQuantity(JO, FactoryCD);
                if (dtSize.Rows.Count > 0 && dtColor_Just_Color.Rows.Count > 0 && dtQuantity.Rows.Count > 0)
                {
                    ShowQuantityDetail(dtSize, dtColor_Just_Color, dtQuantity, JO, FactoryCD);
                    //ShowQuantityDetail(dtSize, dtColor, dtQuantity);
                }
                bool IsGO = false;

                DataTable dtMissColor = MESComment.wipReduceQuantitySql.GetCanceledColor_Size_CUTQTY(JO, IsGO, FactoryCD);
                if (dtMissColor.Rows.Count > 0)
                {
                    ShowMissingColorDetail(dtMissColor);
                }
                this.gvDetail.InnerHtml += " <tr style='page-break-after: always'><td> </td></tr>"; //Added by Jin Song - Fix bug 7/21/14
            }
        }
        else
        {
            this.gvDetail.InnerHtml = "<table width='100%'  style='color:Red; font-size:large'><tr><td align='center'><b>No Data</b></td></tr></table>";
        }
    }


    private void ShowGoNoInfo(DataTable GoNoInfo)
    {
        //Change width by Jin Song -  Fix bug 7/21/2014
        this.gvDetail.InnerHtml += " <tr ><td><table width='95%' border='1' cellspacing='0' cellpadding='0' style='font-size: 12px; border-collapse: collapse'>";
        this.gvDetail.InnerHtml += "<tr><td width='10%' >BUYER:</td><td>" + GoNoInfo.Rows[0][0].ToString() + "</td></tr>";
        this.gvDetail.InnerHtml += "<tr><td width='10%' >S/C No:</td><td>" + GoNoInfo.Rows[0][1].ToString() + "</td></tr>";
        this.gvDetail.InnerHtml += "<tr><td width='10%'>Style No:</td><td>" + GoNoInfo.Rows[0][2].ToString() + "</td></tr>";
        this.gvDetail.InnerHtml += "<tr><td width='10%'>PPO#</td><td>" + GoNoInfo.Rows[0][3].ToString() + "</td></tr>";
        this.gvDetail.InnerHtml += "</table></td></tr>";

    }

    private void ShowJoNoInfo(DataTable JoNoInfo)
    {
        int i = 1;
        string strJoNo = "", strOrderQty = "", strActualQty = "", strOverShortShip = "", strOverShortagePecrcent = "";
        string strHtml = "";
        this.gvDetail.InnerHtml += "<tr><td> <table width='600px' border='1' cellspacing='0' cellpadding='0' style='font-size: 12px; border-collapse: collapse'>";
        strJoNo = "<tr><td width='130' >C/T#:</td>";
        strOrderQty = "<tr><td width='130'>Order Qty:</td>";
        strActualQty = "<tr><td width='130' >Actual Qty:</td>";
        strOverShortShip = "<tr style='font-weight:bold'><td width='130' >Over/Short Ship%</td>";
        strOverShortagePecrcent = "<tr style='font-weight:bold'><td width='130' >Over/Shortage Percent</td>";

        foreach (DataRow dr in JoNoInfo.Rows)
        {

            strJoNo += "<td >" + dr["JO_NO"].ToString() + "</td>";
            strOrderQty += "<td >" + dr["ORDER_QTY"].ToString() + "</td>";
            strActualQty += "<td >" + dr["Actual_QTY"].ToString() + "</td>";
            strOverShortShip += "<td  >" + dr["OVER_SHORT_SHIP"].ToString() + "</td>";
            strOverShortagePecrcent += "<td >" + dr["OVER_SHORTAGE_PERCENT"].ToString() + "</td>";
            if (i % 10 == 0)
            {
                strJoNo += "</tr>";
                strOrderQty += "</tr>";
                strActualQty += "</tr>";
                strOverShortShip += "</tr>";
                strOverShortagePecrcent += "</tr>";
                strHtml += strJoNo + strOrderQty + strActualQty + strOverShortShip + strOverShortagePecrcent;
                strJoNo = "<tr><td width='130' >C/T#:</td>";
                strOrderQty = "<tr><td width='130'>Order Qty:</td>";
                strActualQty = "<tr><td width='130' >Actual Qty:</td>";
                strOverShortShip = "<tr style='font-weight:bold'><td width='130' >Over/Short Ship%</td>";
                strOverShortagePecrcent = "<tr style='font-weight:bold'><td width='130' >Over/Shortage Percent</td>";
            }
            i++;
        }
        this.gvDetail.InnerHtml += strHtml + strJoNo + "</tr>" + strOrderQty + "</tr>" + strActualQty + "</tr>" + strOverShortShip + "</tr>" + strOverShortagePecrcent + "</tr></table></td></tr>";

    }
    //Added By ZouShichang ON 2014.04.25 End

    private void ShowJOStatus(string Status, string Color)
    {
        if (!this.gvDetail.InnerHtml.Equals(""))
        {//插入分页符;
            this.gvDetail.InnerHtml += " <tr style='page-break-after: always'><td> </td></tr>";
        }
        this.gvDetail.InnerHtml += " <tr><td align='right' style='color:" + Color + "'>";
        //this.gvDetail.InnerHtml += " <h1><asp:Literal runat='server' ID='statusLiteral' EnableViewState='false' Text=''></asp:Literal></h1>";
        this.gvDetail.InnerHtml += " <h1>" + Status + "</h1>";
        this.gvDetail.InnerHtml += " </td></tr>";
        this.gvDetail.InnerHtml += " <tr><td>&nbsp;</td></tr>";//空一行,美观;
    }




    private void ShowJOHeader(DataTable dtHeader)
    {
        
        this.gvDetail.InnerHtml += " <tr><td><tr><td><strong><font size='3px'>JO Header</font></strong></td></tr></td></tr>";//JoHeader标题;
        //Header内容;
        this.gvDetail.InnerHtml += " <tr><td>";
        this.gvDetail.InnerHtml += " <table width='95%' border='1' cellspacing='0' cellpadding='0' style='font-size: 12px; border-collapse: collapse'>"; //Change width - Fix bug 7/21/2014
        this.gvDetail.InnerHtml += " <tr>";
        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>JO No</td>";
        this.gvDetail.InnerHtml += " <td width='150'>" + dtHeader.Rows[0]["joNo"].ToString() + "</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>S/C No.</td>";
        this.gvDetail.InnerHtml += " <td>" + dtHeader.Rows[0]["scNo"].ToString() + "</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>GMT DATE</td>";
        this.gvDetail.InnerHtml += " <td>" + dtHeader.Rows[0]["gmtDelDate"].ToString() + "</td>";

        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>Status:</td>";
        if (dtHeader.Rows[0]["COMPLETE_STATUS"].ToString() == "Finished")
        {
            this.gvDetail.InnerHtml += "<th align='left' style='color:blue'>" + dtHeader.Rows[0]["COMPLETE_STATUS"].ToString() + "</th>";
        }
        else
        {
            this.gvDetail.InnerHtml += "<th align='left' style='color:Red'>" + dtHeader.Rows[0]["COMPLETE_STATUS"].ToString() + "</th>";
        }

        this.gvDetail.InnerHtml += " </tr><tr>";
        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>Start Cut Date</td>";
        this.gvDetail.InnerHtml += " <td width='150'>" + dtHeader.Rows[0]["date"].ToString() + "</td>";

        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>Complete Cut Date</td>";
        this.gvDetail.InnerHtml += "<td color='Blue'>" + dtHeader.Rows[0]["COMPLETION_DATE"].ToString() + "</td>";

        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>Style No.</td>";
        this.gvDetail.InnerHtml += " <td>" + dtHeader.Rows[0]["styleNo"].ToString() + "</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>Season</td>";
        this.gvDetail.InnerHtml += " <td>" + dtHeader.Rows[0]["season"].ToString() + "</td>";
        this.gvDetail.InnerHtml += " </tr>";
        this.gvDetail.InnerHtml += " </table>";
        this.gvDetail.InnerHtml += " </td></tr>";
        this.gvDetail.InnerHtml += " <tr><td>&nbsp;</td></tr>";//空一行,美观;
    }

    private void ShowGOHeader(DataTable dtHeader)
    {//JoHeader标题;
        this.gvDetail.InnerHtml += " <tr><td>";
        this.gvDetail.InnerHtml += " <table border='1' cellspacing='0' cellpadding='0' style='font-size: 12px; border-collapse: collapse'>";
        this.gvDetail.InnerHtml += " <tr><td  class='tr2style'><strong><font size='3px'>GO NO :</font></strong></td>";
        this.gvDetail.InnerHtml += " <td><strong><font size='3px'>&nbsp;&nbsp;" + dtHeader.Rows[0]["SC_NO"].ToString() + "&nbsp;&nbsp;</font></strong></td>";
        this.gvDetail.InnerHtml += " <td  class='tr2style'><strong><font size='3px'>Buyer :</font></strong></td>";
        this.gvDetail.InnerHtml += " <td><strong><font size='3px'>&nbsp;&nbsp;" + dtHeader.Rows[0]["SHORT_NAME"].ToString() + "&nbsp;&nbsp;</font></strong></td>";
        this.gvDetail.InnerHtml += " <td  class='tr2style'><strong><font size='3px'>Del.Date :</font></strong></td>";
        this.gvDetail.InnerHtml += " <td><strong><font size='3px'>&nbsp;&nbsp;" + dtHeader.Rows[0]["DEL_DATE"].ToString() + "&nbsp;&nbsp;</font></strong></td>";
        this.gvDetail.InnerHtml += " <td  class='tr2style'><strong><font size='3px'>Over% / Short% :</font></strong></td>";
        this.gvDetail.InnerHtml += " <td><strong><font size='3px'>&nbsp;&nbsp;" + dtHeader.Rows[0]["OverShort"].ToString() + "&nbsp;&nbsp;</font></strong></td></tr>";
        this.gvDetail.InnerHtml += " <tr><td  class='tr2style'><strong><font size='3px'>Total Order QTY :</font></strong></td>";
        string Total_Ord_Qty = (dtHeader.Rows[0]["TOTAL_ORDER_QTY"].ToString().Trim().Equals("")) ? "0" : dtHeader.Rows[0]["TOTAL_ORDER_QTY"].ToString();
        this.gvDetail.InnerHtml += " <td><strong><font size='3px'>&nbsp;&nbsp;" + Total_Ord_Qty + "&nbsp;pcs&nbsp;&nbsp;</font></strong></td>";
        this.gvDetail.InnerHtml += " <td  class='tr2style'><strong><font size='3px'>Total Cut QTY :</font></strong></td>";
        string Total_Cut_Qty = (dtHeader.Rows[0]["TOTAL_CUT_QTY"].ToString().Trim().Equals("")) ? "0" : dtHeader.Rows[0]["TOTAL_CUT_QTY"].ToString();
        this.gvDetail.InnerHtml += " <td><strong><font size='3px'>&nbsp;&nbsp;" + Total_Cut_Qty + "&nbsp;pcs&nbsp;&nbsp;</font></strong></td>";
        this.gvDetail.InnerHtml += " <td  class='tr2style'><strong><font size='3px'>Final Qty :</font></strong></td>";
        string Total_Final_Qty = (dtHeader.Rows[0]["Final_Qty"].ToString().Trim().Equals("")) ? "0" : dtHeader.Rows[0]["Final_Qty"].ToString();
        this.gvDetail.InnerHtml += " <td><strong><font size='3px'>&nbsp;&nbsp;" + Total_Final_Qty + "&nbsp;pcs&nbsp;&nbsp;</font></strong></td>";
        this.gvDetail.InnerHtml += " <td  class='tr2style'><strong><font size='3px'>Last Bed No :</font></strong></td>";
        this.gvDetail.InnerHtml += " <td><strong><font size='3px'>&nbsp;&nbsp;" + dtHeader.Rows[0]["LAST_BED_NO"].ToString() + "&nbsp;&nbsp;</font></strong></td></tr>";
        this.gvDetail.InnerHtml += " </table>";
        this.gvDetail.InnerHtml += " </td></tr>";
        this.gvDetail.InnerHtml += " <tr><td>&nbsp;</td></tr>";//空一行,美观;
    }

    private void ShowColorDetail(DataTable dtColor)
    {
        //Color标题;
        this.gvDetail.InnerHtml += " <tr><td>";
        this.gvDetail.InnerHtml += " <table border='1' width='95%' cellspacing='0' cellPadding='0' style='font-size: 12px; border-collapse: collapse'>"; //Change width - Fix bug 7/21/2014
        this.gvDetail.InnerHtml += " <tr><td style='width:50' class='tr2style'>Color</td><td style='width:150' class='tr2style'>Color Desc</td><td style='width:100' class='tr2style'>PPO YPD</td>";
        this.gvDetail.InnerHtml += " <td style='width:120' class='tr2style'>Marker Net YPD</td><td style='width:80' class='tr2style'>Marker YPD</td><td style='width:80' class='tr2style'>Bulk YPD</td>";
        foreach (DataRow dr in dtColor.Rows)
        {
            this.gvDetail.InnerHtml += "<tr>";
            this.gvDetail.InnerHtml += "<td>&nbsp;" + dr["COLOR_CODE"].ToString() + "&nbsp;</td>";
            this.gvDetail.InnerHtml += "<td>&nbsp;" + dr["COLOR_DESC"].ToString() + "&nbsp;</td>";
            this.gvDetail.InnerHtml += "<td>&nbsp;" + dr["PPO_NET_YPD_NEW"].ToString() + "&nbsp;</td>";
            this.gvDetail.InnerHtml += "<td>&nbsp;" + dr["MARKER_NET_YPD"].ToString() + "&nbsp;</td>";
            this.gvDetail.InnerHtml += "<td>&nbsp;" + dr["MARKER_YPD"].ToString() + "&nbsp;</td>";
            this.gvDetail.InnerHtml += "<td>&nbsp;" + dr["BULK_YPD"].ToString() + "&nbsp;</td>";
            this.gvDetail.InnerHtml += "</tr>";
        }
        this.gvDetail.InnerHtml += " </table>";
        this.gvDetail.InnerHtml += " </td></tr>";
        this.gvDetail.InnerHtml += " <tr><td>&nbsp;</td></tr>";//空一行,美观;
    }

    private void ShowQuantityDetail(DataTable dtSize, DataTable dtColor, DataTable dtQuantity, string JO, string FactoryCD)
    {
        double OrderQty = 0;
        double CutQty = 0;
        double CutReduce = 0;
        double ActualQty = 0;
        double OverShortageQty = 0;
        //double OverShortagePercent = 0;
        double Cut2DiscrepancyQty = 0;
        double EmbDiscrepancyQty = 0;
        double PrintDiscrepancyQty = 0;
        double FinalQty = 0;

        DataTable dtQuantitySelect = new DataTable();
        DataRow[] dr;
        dtQuantitySelect = dtQuantity.Clone();

        this.gvDetail.InnerHtml += "<tr><td>";
        this.gvDetail.InnerHtml += "<table width='95%' border='1' cellspacing='0' cellPadding='0' style='font-size: 12px; border-collapse: collapse'>"; //Change width - Fix bug 7/21/2014
        for (int i = 0; i < dtQuantity.Columns.Count - 1; i++)
        {
            if (i == 0)
            {
                this.gvDetail.InnerHtml += "<th class='tr2style' style='font-weight:bold;font-style:italic; Font-Size:14px'>Colour/Size</th>";
            }
            else
            {
                this.gvDetail.InnerHtml += "<th class='tr2style' style='font-weight:bold;font-style:italic; Font-Size:14px'>" + dtQuantity.Columns[i].ColumnName.ToString() + "</th>";
            }
        }
        this.gvDetail.InnerHtml += " </tr>";

        for (int h = 0; h < dtColor.Rows.Count; h++)
        {
            dr = dtQuantity.Select("COLOR_CD='" + dtColor.Rows[h][0].ToString() + "'");
            dtQuantitySelect.Clear();
            //RowSpan=dtQuantitySelect.Count();
            foreach (DataRow row in dr)  // 将查询的结果添加到dt中；
            {
                dtQuantitySelect.Rows.Add(row.ItemArray);
            }
            //rowspan
            if (h % 2 == 0)
            {
                gvDetail.InnerHtml += "<tr bgColor='white'><td rowspan='" + dr.Count() + "' width='150'>" + dtColor.Rows[h][0].ToString() + "(" + dtColor.Rows[h][1].ToString() + ")" + "</td>";
            }
            else
            {
                gvDetail.InnerHtml += "<tr bgColor='#f2f2f2'><td rowspan='" + dr.Count() + "' width='150'>" + dtColor.Rows[h][0].ToString() + "(" + dtColor.Rows[h][1].ToString() + ")" + "</td>";
            }

            //his.gvDetail.InnerHtml += "<tr><td rowspan= " + dr.Count() + ">" + dtColor.Rows[h][0].ToString() + "</td>"; 
            for (int i = 0; i < dtQuantitySelect.Rows.Count; i++)
            {
                switch (dtQuantitySelect.Rows[i]["Remark"].ToString().Trim().Substring(0, dtQuantitySelect.Rows[i]["Remark"].ToString().Trim().Length - 1))
                {
                    case "Order-Qt":
                        OrderQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "Cut-Qt":
                        CutQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "Csample-Qt":
                        CutQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "SSample-Qt":
                        CutQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "Re-Cut":
                        CutQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "Cut-Reduc":
                        CutReduce += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "Actual-Qt":
                        ActualQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "Over/Shortage Qt":
                        OverShortageQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "Cut2DiscrepancyQt":
                        Cut2DiscrepancyQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "EmbDiscrepancyQt":
                        EmbDiscrepancyQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "PrintDiscrepancyQt":
                        PrintDiscrepancyQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;
                    case "FinalQt":
                        FinalQty += double.Parse(dtQuantitySelect.Rows[i]["Total"].ToString());
                        break;

                }

                for (int j = 1; j < dtQuantitySelect.Columns.Count - 1; j++)
                {

                    string style = "";
                    if (dtQuantitySelect.Rows[i]["Remark"].ToString().Trim().Equals("Actual-Qty") || dtQuantitySelect.Rows[i]["Remark"].ToString().Trim().Equals("FinalQty"))
                    {

                        style = "style='font-weight:bold;font-style:italic; Font-Size:14px'";

                    }
                    if (dtQuantitySelect.Rows[i]["Remark"].ToString().Trim().Equals("Over/Shortage Percent") && dtQuantitySelect.Rows[i][j].ToString().Length > 1)
                    {

                        if (dtQuantitySelect.Rows[i][j].ToString().Substring(0, 1) == "-")
                        {
                            style = "style='font-weight:bold;font-style:italic; Font-Size:14px;COLOR:RED'";
                        }
                        else
                        {
                            style = "style='font-weight:bold;font-style:italic; Font-Size:14px;COLOR:BLUE'";
                        }
                    }
                    if (h % 2 == 0)
                    {
                        this.gvDetail.InnerHtml += "<td bgColor='white' " + style + " >" + dtQuantitySelect.Rows[i][j].ToString() + "</td>";
                    }
                    else
                    {
                        this.gvDetail.InnerHtml += "<td bgColor='#f2f2f2' " + style + ">" + dtQuantitySelect.Rows[i][j].ToString() + "</td>";
                    }
                }
                this.gvDetail.InnerHtml += "</tr>";
            }

        }
        gvDetail.InnerHtml += "</table></td></tr>";

        this.gvDetail.InnerHtml += " <tr><td>&nbsp;</td></tr>";//空一行,美观;
        ShowTotal(OrderQty, CutQty, CutReduce, ActualQty, OverShortageQty, strOverShortagePercent, Cut2DiscrepancyQty, EmbDiscrepancyQty, PrintDiscrepancyQty, FinalQty);
    }


    private void ShowQuantityDetailByGO(DataTable dtSize, DataTable dtColor, DataTable dtQuantity)
    {
        int rowspan = 14;
        string Color_1 = "#f2f2f2";
        //Quantity 标题;
        this.gvDetail.InnerHtml += "<tr><td>";
        this.gvDetail.InnerHtml += "<table width='100%' border='1' cellspacing='0' cellPadding='0' style='font-size: 12px; border-collapse: collapse'>";
        this.gvDetail.InnerHtml += "<tr><td class='tr2style'>Colour/Size</td>";
        this.gvDetail.InnerHtml += "<td class='tr2style'>&nbsp;</td>";
        for (int i = 0; i < dtSize.Rows.Count; i++)
        {
            gvDetail.InnerHtml += "<td align='center' class='tr2style'>" + dtSize.Rows[i][0] + "</td>";
        }
        //gvDetail.InnerHtml += "<td align='center' class='tr2style'>Total</td><td align='center' class='tr2style'>Over Cut.</td></tr>";
        gvDetail.InnerHtml += "<td align='center' class='tr2style'>Total</td></tr>";
        //Quantity 内容;
        //循环byColor；
        foreach (DataRow dr_color in dtColor.Rows)
        {
            if (Color_1.Equals("#f2f2f2"))
            {
                Color_1 = "white";
            }
            else
            {
                Color_1 = "#f2f2f2";
            }
            string Color_Code = dr_color["COLOR_CODE"].ToString();
            string BigFont = "";
            if (Color_Code.ToUpper().Contains("TOTAL"))
            {
                BigFont = "<b/>";
                Color_1 = "#efefe7";
                gvDetail.InnerHtml += "<tr style='background-color:" + Color_1 + "'><td rowspan='" + 6 + "' style='width:120px' >" + BigFont + Color_Code + ":" + dr_color["COLOR_DESC"].ToString() + "</td>";
            }
            else
            {
                gvDetail.InnerHtml += "<tr style='background-color:" + Color_1 + "'><td rowspan='" + rowspan + "' style='width:120px' >" + BigFont + Color_Code + ":" + dr_color["COLOR_DESC"].ToString() + "</td>";
            }


            string SQL = "COLOR_CODE = '" + Color_Code + "'";
            DataRow[] dt_one_color = dtQuantity.Select(SQL);
            int column = dtQuantity.Columns.IndexOf("SIZE_CODE");
            double Total_Order = 0.00;
            double Total_Cut = 0.00;
            double Total_Actual = 0.00;
            double Total_Cut_Disc_Qty = 0.00;
            double Total_Emb_Disc_Qty = 0.00;
            double Total_Print_Disc_Qty = 0.00;
            double Total_Final = 0.00;
            string Total_Over_Cut = "";
            //循环byColor，byType；
            do
            {
                column++;
                string Font_Style = "";
                if (Color_Code.ToUpper().Contains("TOTAL") && !(GetTotalStringView(dtQuantity.Columns[column].ColumnName.ToString().ToUpper())))
                {
                    continue;
                }

                switch (dtQuantity.Columns[column].ColumnName.ToString().ToUpper())
                {
                    case "ORDER_QTY":
                        gvDetail.InnerHtml += "<tr style='font-weight:bold'><td  style='background-color:" + Color_1 + "'>ORD</td>";
                        break;
                    case "CUT_QTY":
                        gvDetail.InnerHtml += "<tr  style='background-color:" + Color_1 + "'><td>Cut-Qty</td>";
                        Font_Style = "<I/>"; ;
                        break;
                    case "SSAMPLE_QTY":
                        gvDetail.InnerHtml += "<tr><td  style='background-color:" + Color_1 + "'>SSample-Qty</td>";
                        break;
                    case "CSAMPLE_QTY":
                        gvDetail.InnerHtml += "<tr><td  style='background-color:" + Color_1 + "'>CSample-Qty</td>";
                        break;
                    case "CUT_REDUCE":
                        gvDetail.InnerHtml += "<tr><td  style='background-color:" + Color_1 + "'>Cut-Reduce</td>";
                        break;
                    case "RECUT_QTY":
                        gvDetail.InnerHtml += "<tr><td  style='background-color:" + Color_1 + "'>Re-Cut</td>";
                        break;
                    case "ACTUAL_QTY":
                        gvDetail.InnerHtml += "<tr style='font-weight:bold'><td  style='background-color:" + Color_1 + "'>Actual-Qty</td>";
                        Font_Style = "<I/>"; ;
                        break;
                    case "OVER_SHORTAGE_PERCENT":
                        gvDetail.InnerHtml += "<tr style='font-weight:bold;background-color:" + Color_1 + "'><td style='background-color:" + Color_1 + "'>%</td>";
                        break;
                    case "CUT_DISCREPANCY_QTY":
                        gvDetail.InnerHtml += "<tr><td  style='background-color:" + Color_1 + "'>Cut2DiscrepancyQty</td>";
                        break;
                    case "EMB_DISCREPANCY_QTY":
                        gvDetail.InnerHtml += "<tr><td  style='background-color:" + Color_1 + "'>EmbDiscrepancyQty</td>";
                        break;
                    case "PRINT_DISCREPANCY_QTY":
                        gvDetail.InnerHtml += "<tr><td  style='background-color:" + Color_1 + "'>PrintDiscrepancyQty</td>";
                        break;
                    case "FINAL_QTY":
                        gvDetail.InnerHtml += "<tr style='font-weight:bold'><td  style='background-color:" + Color_1 + "'>FinalQty</td>";
                        break;
                    case "OVER_SHORTAGE_PERCENT_FINAL_QTY":
                        gvDetail.InnerHtml += "<tr style='font-weight:bold'><td  style='background-color:" + Color_1 + "'>%</td>";
                        break;
                }

                foreach (DataRow dr_size in dtSize.Rows)
                {
                    //循环byColor，byType，bySize；
                    string Size_Code = dr_size["SIZE_CODE"].ToString().ToUpper();
                    string QTY = "NNN";
                    for (int int_row = 0; int_row < dt_one_color.Length; int_row++)
                    {
                        if (dt_one_color[int_row]["SIZE_CODE"].ToString().ToUpper().Equals(Size_Code))
                        {
                            QTY = dt_one_color[int_row][column].ToString();
                            if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("ORDER_QTY"))
                            {
                                Total_Order += Double.Parse(dt_one_color[int_row][column].ToString());
                            }
                            else if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("CUT_QTY"))
                            {
                                Total_Cut += Double.Parse(dt_one_color[int_row][column].ToString());
                            }
                            else if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("ACTUAL_QTY"))
                            {
                                Total_Actual += Double.Parse(dt_one_color[int_row][column].ToString());
                            }
                            else if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("CUT_DISCREPANCY_QTY"))
                            {
                                Total_Cut_Disc_Qty += Double.Parse(dt_one_color[int_row][column].ToString());
                            }
                            else if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("EMB_DISCREPANCY_QTY"))
                            {
                                Total_Emb_Disc_Qty += Double.Parse(dt_one_color[int_row][column].ToString());
                            }
                            else if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("PRINT_DISCREPANCY_QTY"))
                            {
                                Total_Print_Disc_Qty += Double.Parse(dt_one_color[int_row][column].ToString());
                            }
                            else if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("FINAL_QTY"))
                            {
                                Total_Final += Double.Parse(dt_one_color[int_row][column].ToString());
                            }
                        }
                    }
                    if (QTY.Equals("NNN"))
                    {
                        gvDetail.InnerHtml += "<td>&nbsp;</td>";
                    }
                    else
                    {
                        if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("OVER_SHORTAGE_PERCENT"))
                        {
                            if (QTY != "" && QTY.Substring(0, 1).ToString() == "-")
                            {
                                gvDetail.InnerHtml += "<td  align='right' style='background-color:" + Color_1 + "; color:Red'>" + BigFont + QTY + "</td>";
                            }
                            else
                            {
                                gvDetail.InnerHtml += "<td  align='right' style='background-color:" + Color_1 + ";color:Blue'>" + BigFont + QTY + "</td>";
                            }
                        }
                        else
                        {
                            gvDetail.InnerHtml += "<td  align='right'  style='background-color:" + Color_1 + "'>" + BigFont + Font_Style + QTY + "</td>";
                        }
                    }
                }
                //SIZE之后是Total；Total之后，是Over_CUT;
                string underline = "<b>";
                string underlineEnd = "</b>";
                string FontSize = "";
                if (Color_Code.ToUpper().Contains("TOTAL"))
                {
                    underline = "<u><b>";
                    underlineEnd = "</b></u>";
                    FontSize = "; font-size:16px";
                }
                if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("ORDER_QTY"))
                {
                    gvDetail.InnerHtml += "<td  align='right'>" + underline + Total_Order + underlineEnd + "</td></tr>";

                }
                else if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("CUT_QTY"))
                {
                    gvDetail.InnerHtml += "<td  align='right'>" + underline + Font_Style + Total_Cut + underlineEnd + "</td>";

                }
                else if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("ACTUAL_QTY"))
                {
                    gvDetail.InnerHtml += "<td  align='right'>" + underline + Font_Style + Total_Actual + underlineEnd + "</td>";
                }
                else if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("OVER_SHORTAGE_PERCENT"))
                {
                    Total_Over_Cut = (Total_Actual * Total_Order == 0.00) ? "&nbsp;" : ((Total_Actual - Total_Order) * 100 / Total_Order).ToString("#,##0.000") + "%";
                    if (Total_Over_Cut != "" && Total_Over_Cut.Substring(0, 1).ToString() == "-")
                    {
                        gvDetail.InnerHtml += "<td align='right' style='border-bottom:0; border-top:0; color:Red" + FontSize + "' >" + underline + Total_Over_Cut + underlineEnd + "</td></tr>";
                    }
                    else
                    {
                        gvDetail.InnerHtml += "<td align='right' style='border-bottom:0; border-top:0;" + FontSize + "color:blue' >" + underline + Total_Over_Cut + underlineEnd + "</td></tr>";
                    }
                }
                else if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("CUT_DISCREPANCY_QTY"))
                {
                    gvDetail.InnerHtml += "<td  align='right'>" + underline + Font_Style + Total_Cut_Disc_Qty + underlineEnd + "</td>";
                }
                else if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("EMB_DISCREPANCY_QTY"))
                {
                    gvDetail.InnerHtml += "<td  align='right'>" + underline + Font_Style + Total_Emb_Disc_Qty + underlineEnd + "</td>";
                }
                else if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("PRINT_DISCREPANCY_QTY"))
                {
                    gvDetail.InnerHtml += "<td  align='right'>" + underline + Font_Style + Total_Print_Disc_Qty + underlineEnd + "</td>";
                }
                else if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("FINAL_QTY"))
                {
                    gvDetail.InnerHtml += "<td  align='right'>" + underline + Font_Style + Total_Final + underlineEnd + "</td>";
                }

                else if (dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("OVER_SHORTAGE_PERCENT_FINAL_QTY"))
                {
                    Total_Over_Cut = (Total_Final * Total_Order == 0.00) ? "&nbsp;" : ((Total_Final - Total_Order) * 100 / Total_Order).ToString("#,##0.000") + "%";
                    if (Total_Over_Cut != "" && Total_Over_Cut.Substring(0, 1).ToString() == "-")
                    {
                        gvDetail.InnerHtml += "<td align='right' style='border-bottom:0; border-top:0; color:Red" + FontSize + "' >" + underline + Total_Over_Cut + underlineEnd + "</td></tr>";
                    }
                    else
                    {
                        gvDetail.InnerHtml += "<td align='right' style='border-bottom:0; border-top:0;" + FontSize + "color:blue' >" + underline + Total_Over_Cut + underlineEnd + "</td></tr>";
                    }
                }

                else
                {

                    gvDetail.InnerHtml += "<td style='border-top:0'>&nbsp;</td></tr>";
                }
            } while (!dtQuantity.Columns[column].ColumnName.ToString().ToUpper().Equals("OVER_SHORTAGE_PERCENT_FINAL_QTY"));
        }
        this.gvDetail.InnerHtml += " </table>";
        this.gvDetail.InnerHtml += " </td>";
        this.gvDetail.InnerHtml += " </tr>";
        this.gvDetail.InnerHtml += " <tr><td>&nbsp;</td></tr>";//空一行,美观;
    }


    private void ShowTotal(double OrderQty, double CutQty, double CutReduce, double ActualQty, double OverShortageQty, string OverShortagePercent, double Cut2DiscrepancyQty, double EmbDiscrepancyQty, double PrintDiscrepancyQty, double FinalQty)
    {
        this.gvDetail.InnerHtml += " <tr>";
        this.gvDetail.InnerHtml += " <td>";
        this.gvDetail.InnerHtml += " <table width='95%' border='1' cellspacing='0' cellPadding='0' style='font-weight:bold;font-size: 12px;border-collapse: collapse'>"; //Change width - Fix bug 7/21/2014
        this.gvDetail.InnerHtml += " <tr>";
        this.gvDetail.InnerHtml += " <td class='tr2style'>Total Order-Qty</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style'>Total Cut-Qty</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style'>Total Cut-Reduce</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style'>Total Actual-Qty</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style'>Total Over/Shortage</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style'>Total Over/Shortage Percent</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style'>Total Cut2DiscrepancyQty</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style'>Total EmbDiscrepancyQty</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style'>Total PrintDiscrepancyQty</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style'>Total FinalQty</td>";
        this.gvDetail.InnerHtml += " </tr><tr>";
        this.gvDetail.InnerHtml += " <td>" + OrderQty.ToString() + "</td>";
        this.gvDetail.InnerHtml += " <td>" + CutQty.ToString() + "</td>";
        this.gvDetail.InnerHtml += " <td>" + CutReduce.ToString() + "</td>";
        this.gvDetail.InnerHtml += " <td>" + ActualQty.ToString() + "</td>";
        this.gvDetail.InnerHtml += " <td>" + OverShortageQty.ToString() + "</td>";
        //this.gvDetail.InnerHtml += " <td>" + OverShortagePercent.ToString("#,##0.000") + "%</td>";
        this.gvDetail.InnerHtml += " <td>" + OverShortagePercent + "</td>";
        this.gvDetail.InnerHtml += " <td>" + Cut2DiscrepancyQty.ToString() + "</td>";
        this.gvDetail.InnerHtml += " <td>" + EmbDiscrepancyQty.ToString() + "</td>";
        this.gvDetail.InnerHtml += " <td>" + PrintDiscrepancyQty.ToString() + "</td>";
        this.gvDetail.InnerHtml += " <td>" + FinalQty.ToString() + "</td>";
        this.gvDetail.InnerHtml += " </tr>";
        this.gvDetail.InnerHtml += " </table>";
        this.gvDetail.InnerHtml += " </td>";
        this.gvDetail.InnerHtml += " </tr>";
        this.gvDetail.InnerHtml += " <tr><td>&nbsp;</td></tr>";//空一行,美观;
    }

    private void ShowMissingColorDetail(DataTable dtMissColor)
    {
        double Total_miss = 0.0;
        this.gvDetail.InnerHtml += "<tr>";
        this.gvDetail.InnerHtml += "<td>";
        this.gvDetail.InnerHtml += "<strong><font size='3px'>Canceled Color/Size Actual Cut Info</font></strong><br/>";
        this.gvDetail.InnerHtml += "</td>";
        this.gvDetail.InnerHtml += "</tr>";
        this.gvDetail.InnerHtml += "<tr>";
        this.gvDetail.InnerHtml += "<td>";
        this.gvDetail.InnerHtml += "<table  width='20%' border='1' cellspacing='0' cellPadding='0' style='font-size: 12px; border-collapse: collapse'>";
        this.gvDetail.InnerHtml += "<tr><td  class='tr2style'>Size</td><td  class='tr2style'>Color</td><td  class='tr2style'>Cut Qty</td></tr>";
        for (int i = 0; i < dtMissColor.Rows.Count; i++)
        {
            gvDetail.InnerHtml += "<tr><td  align='right'>" + dtMissColor.Rows[i][0].ToString() + "</td>";
            gvDetail.InnerHtml += "<td  align='right'>" + dtMissColor.Rows[i][1].ToString() + "</td>";
            gvDetail.InnerHtml += "<td  align='right'>" + dtMissColor.Rows[i][2].ToString() + "</td>";
            Total_miss += Int32.Parse(dtMissColor.Rows[i][2].ToString());
            gvDetail.InnerHtml += "</tr>";
        }
        this.gvDetail.InnerHtml += "<tr><td align='right' colspan='2' style='font-size:14px; font-weight: bolder'>Total :</td><td align='right' style='color:Red; font-size:14px; font-weight: bolder'>" + Total_miss + "</td></tr>";
        this.gvDetail.InnerHtml += "</table>";
        this.gvDetail.InnerHtml += "</td>";
        this.gvDetail.InnerHtml += "</tr>";
        this.gvDetail.InnerHtml += " <tr><td>&nbsp;</td></tr>";//空一行,美观;
    }

    private static bool GetTotalStringView(string RowStr)
    {
        string[] totaldisplayArray = { "ORDER_QTY", "ACTUAL_QTY", "OVER_SHORTAGE_PERCENT", "FINAL_QTY", "OVER_SHORTAGE_PERCENT_FINAL_QTY" };
        bool totalStrView = false;
        for (int i = 0; i <= totaldisplayArray.Length - 1; i++)
        {
            if (totaldisplayArray[i].ToString().ToUpper() == RowStr.ToUpper())
            {
                totalStrView = true;
                return totalStrView;
            }
        }

        return totalStrView;
    }

    //Added By ZouShichang ON 2014.04.25 Start
    private static DataTable GetGoNoInfo(string GoNo)
    {

        string SQL = "";
        SQL = @"SELECT CUST.SHORT_NAME,HD.SC_NO,HD.STYLE_NO,LEFT(StuList,LEN(StuList)-1) as PPO FROM (
        SELECT GO_NO,
        (SELECT DISTINCT PPO_NO +' / '  FROM CP_FABRIC_ITEM 
        WHERE GO_NO=A.GO_NO AND GO_NO='" + GoNo + @"' FOR XML PATH('')) AS StuList
        FROM CP_FABRIC_ITEM A
        WHERE A.GO_NO='" + GoNo + @"'
        GROUP BY GO_NO )B RIGHT JOIN SC_HD AS HD ON B.GO_NO=HD.SC_NO INNER JOIN dbo.GEN_CUSTOMER AS CUST ON HD.CUSTOMER_CD=CUST.CUSTOMER_CD
        WHERE HD.SC_NO='" + GoNo + "' ";


        return MESComment.DBUtility.GetTable(SQL, "MES");
    }

    private static DataTable GetJoInfo(string JoNo)
    {
        //Bug fix by ZK to add in checking if Order Qty = 0
        //Bug fix by MF on 20140821,add Cut_Qty to calculate Over/Shortage percent
        string SQL = "";
        SQL = @" SELECT A.SC_NO ,
                        A.JO_NO ,
                        A.total_qty AS ORDER_QTY ,
                        B.Actual_QTY ,
                        '+' + CAST(D.PERCENT_OVER_ALLOWED AS NVARCHAR(10)) + '/-'
                        + CAST(D.PERCENT_SHORT_ALLOWED AS NVARCHAR(10)) AS OVER_SHORT_SHIP ,
                        CASE WHEN A.total_qty = 0 THEN '' ELSE CAST (CONVERT(DECIMAL(18, 3), ROUND(( ISNULL(E.Cut_QTY, 0)
                                                        - ISNULL(C.CUT_REDUCE, 0)
                                                        - ISNULL(A.total_qty, 0) ) * 100
                                                      / A.total_qty, 3)) AS  NVARCHAR(20))+'%' END AS OVER_SHORTAGE_PERCENT ,
                        C.CUT_REDUCE
                 FROM   JO_HD AS A
                        INNER JOIN ( SELECT JOB_ORDER_NO ,
                                            SUM(QTY) AS Actual_QTY
                                     FROM   dbo.CUT_BUNDLE_HD
                                     WHERE  JOB_ORDER_NO = '" + JoNo + @"'
                                            AND STATUS = 'Y'
                                     GROUP BY JOB_ORDER_NO
                                   ) AS B ON A.JO_NO = B.JOB_ORDER_NO
                        INNER JOIN ( SELECT JOB_ORDER_NO ,
                                            SUM(QTY) AS Cut_QTY
                                     FROM   dbo.CUT_BUNDLE_HD
                                     WHERE  JOB_ORDER_NO = '" + JoNo + @"'
                                            AND STATUS = 'Y'
                                            -- Add filter TRX_TYPE='NM' to correct the calculation for Over/Shortage Percent
                                            AND TRX_TYPE = 'NM'
                                     GROUP BY JOB_ORDER_NO
                                   ) AS E ON A.JO_NO = E.JOB_ORDER_NO
                        LEFT JOIN ( SELECT JOB_ORDER_NO ,
                                            SUM(REDUCE_QTY) AS CUT_REDUCE
                                     FROM   dbo.CUT_BUNDLE_REDUCE_TRX
                                     WHERE  JOB_ORDER_NO = '" + JoNo + @"'
                                     GROUP BY JOB_ORDER_NO
                                   ) AS C ON A.JO_NO = C.JOB_ORDER_NO
                        INNER JOIN SC_LOT AS D ON A.SC_NO = D.SC_NO
                                                  AND A.LOT_NO = D.LOT_NO
                 WHERE  JO_NO LIKE ( '" + JoNo + @"' ) AND A.STATUS <> 'D' ";

        return MESComment.DBUtility.GetTable(SQL, "MES");
    }

    //Modified by MF on 20160215, JO Combination
    private static DataTable GetGoNoJoInfo(string GoNo, string JoNo)
    {
        //Bug Fix by Jin Song 7/30/2014 (Add TRX_TYPE=NM)
        //Bug Fix by ZK 8/4/2014 add Order Qty = 0 checking
        //Bug fix by MF on 20140821,add Cut_Qty to calculate Over/Shortage percent
        string SQL = @"  SELECT A.SC_NO ,
                        A.JO_NO ,
                        CAST(A.total_qty AS INT) AS ORDER_QTY ,
                        B.Actual_QTY , CASE WHEN A.JO_NO LIKE '%CB%' THEN '+' + CAST(CB_SHIP.PERCENT_OVER_ALLOWED AS NVARCHAR(10)) 
	                    + '/-' + CAST(CB_SHIP.PERCENT_SHORT_ALLOWED AS NVARCHAR(10)) else
                        '+' + CAST(D.PERCENT_OVER_ALLOWED AS NVARCHAR(10)) + '/-'
                        + CAST(D.PERCENT_SHORT_ALLOWED AS NVARCHAR(10)) End AS OVER_SHORT_SHIP ,
                        CASE WHEN A.total_qty=0 THEN '' ELSE CAST (CONVERT(DECIMAL(18, 3), ROUND(( ISNULL(E.Cut_QTY, 0)
                                                              - ISNULL(C.CUT_REDUCE, 0)
                                                              - ISNULL(A.total_qty, 0) ) * 100
                                                            / A.total_qty, 3)) AS NVARCHAR(20))
                        + '%' END AS OVER_SHORTAGE_PERCENT ,
                        ISNULL(C.CUT_REDUCE, 0) AS CUT_REDUCE
                 FROM   JO_HD AS A
                        LEFT JOIN ( SELECT JOB_ORDER_NO ,
                                            SUM(QTY) AS Actual_QTY
                                     FROM   dbo.CUT_BUNDLE_HD
                                     WHERE  STATUS = 'Y'
                                     GROUP BY JOB_ORDER_NO
                                   ) AS B ON A.JO_NO = B.JOB_ORDER_NO
                        LEFT JOIN ( SELECT JOB_ORDER_NO ,
                                            SUM(QTY) AS Cut_QTY
                                     FROM   dbo.CUT_BUNDLE_HD
                                     WHERE  STATUS = 'Y'
                                            -- Add filter TRX_TYPE='NM' to correct the calculation for Over/Shortage Percent
                                            AND TRX_TYPE = 'NM'
                                     GROUP BY JOB_ORDER_NO
                                   ) AS E ON A.JO_NO = E.JOB_ORDER_NO
                        LEFT JOIN ( SELECT  JOB_ORDER_NO ,
                                            SUM(REDUCE_QTY) AS CUT_REDUCE
                                    FROM    dbo.CUT_BUNDLE_REDUCE_TRX                                  
                                    GROUP BY JOB_ORDER_NO
                                  ) AS C ON A.JO_NO = C.JOB_ORDER_NO
                        LEFT JOIN SC_LOT AS D ON A.SC_NO = D.SC_NO
                                                  AND A.LOT_NO = D.LOT_NO
                        LEFT JOIN (
		                    SELECT A.COMBINE_JO_NO,CAST(ROUND(((SUM(B.TOTAL_QTY*(1+C.PERCENT_OVER_ALLOWED/100))/SUM(B.TOTAL_QTY))-1)*100,2)AS DECIMAL(38,2)) AS PERCENT_OVER_ALLOWED, 
                                CAST(ROUND(((SUM(B.TOTAL_QTY*(1-C.PERCENT_SHORT_ALLOWED/100))/SUM(B.TOTAL_QTY))-1)*-100,2)AS DECIMAL(38,2)) AS PERCENT_SHORT_ALLOWED
                                FROM JO_COMBINE_MAPPING A WITH(NOLOCK)
                                INNER JOIN JO_HD B WITH(NOLOCK) ON A.ORIGINAL_JO_NO = B.JO_NO
                                INNER JOIN SC_LOT C WITH(NOLOCK) ON B.SC_NO = C.SC_NO AND B.LOT_NO = C.LOT_NO";

        if (GoNo != "")
        {
            SQL += @" WHERE C.SC_NO='" + GoNo + @"'";
        }
        if (JoNo != "" & GoNo == "")
        {
            SQL += @" WHERE A.COMBINE_JO_NO='" + JoNo + @"'";
        }

        SQL += @" GROUP BY A.COMBINE_JO_NO) CB_SHIP ON CB_SHIP.COMBINE_JO_NO=A.JO_NO WHERE 1=1 
                            AND NOT EXISTS(SELECT ORIGINAL_JO_NO FROM JO_COMBINE_MAPPING WITH(NOLOCK) WHERE ORIGINAL_JO_NO=A.JO_NO)";
        if (GoNo != "")
        {
            SQL += @" AND JO_NO IN (SELECT JO_NO FROM JO_HD WHERE STATUS<>'D' AND SC_NO='" + GoNo + @"')";
        }
        if (JoNo != "")
        {
            SQL += @" AND  JO_NO IN (
                        SELECT  *
                        FROM    FN_SPLIT_STRING_TB('" + JoNo + @"',
                                                   ',') );";
        }
        return MESComment.DBUtility.GetTable(SQL, "MES");
    }

    private static DataTable GetReCutColInfo(string JoNo, string Factroy_CD)
    {

        string SQL = "";
        SQL = " SELECT DISTINCT MAX(REPAIR_CUT_NUM) AS MAX_NUM FROM cut_lay ";
        SQL = SQL + "   WHERE  FACTORY_CD='" + Factroy_CD + "' AND JOB_ORDER_NO='" + JoNo + "' AND CUT_TYPE='R' ";
        return MESComment.DBUtility.GetTable(SQL, "MES");
    }
    private static DataTable GetReduceQuantityHeadByGO(string GoNo, string FactoryCd)
    {
        string SQL = @" 
                        DECLARE @dtjo TABLE(
                                  JO_NO VARCHAR(20)
                           )
                                 INSERT INTO @dtjo
                                 SELECT JO_NO FROM JO_HD WHERE SC_NO='" + GoNo + @"' AND isnull(CREATED_COMBINE_JO_FLAG,'N')='N' AND STATUS not in ('D','X')
                                 UNION
                                 SELECT JO_NO FROM JO_HD a WITH ( NOLOCK )
                                 inner JOIN dbo.JO_COMBINE_MAPPING b  WITH ( NOLOCK ) ON a.JO_NO=b.COMBINE_JO_NO
                                 WHERE SC_NO='" + GoNo + @"' AND STATUS not in ('D','X')

                        SELECT  A.SC_NO ,
                        SHORT_NAME ,
                        DEL_DATE = ( SELECT CONVERT(VARCHAR(10), MIN(BUYER_PO_DEL_DATE), 120)
                                     FROM   JO_HD WITH ( NOLOCK )
                                     WHERE  STATUS not in ('D','X') AND SC_NO = '" + GoNo + @"'
                                   ) ,
                        OverShort=(   SELECT   CAST(CAST(ROUND(( SUM(PERCENT_OVER_ALLOWED * 0.01 * a.total_qty)
                                          / C.TOTAL_QTY ) * 100, 2) AS DECIMAL(18, 2)) AS NVARCHAR(10))
                        + '%/'
                        + CAST(CAST(ROUND(( SUM(PERCENT_SHORT_ALLOWED * 0.01 * A.total_qty)
                                            / C.TOTAL_QTY ) * 100, 2) AS DECIMAL(18, 2)) AS NVARCHAR(10))
                        + '%' AS OverShart
               FROM     JO_HD AS A
                        INNER JOIN SC_LOT AS B ON A.SC_NO = B.SC_NO
                                                  AND A.LOT_NO = B.LOT_NO
                        INNER JOIN ( SELECT SC_NO ,
                                            SUM(TOTAL_QTY) AS TOTAL_QTY
                                     FROM   JO_HD
                                     WHERE STATUS not in ('D','X') AND  SC_NO = '" + GoNo + @"'
                                     GROUP BY SC_NO
                                   ) AS C ON A.SC_NO = C.SC_NO
               WHERE    STATUS not in ('D','X')
                        AND A.SC_NO = '" + GoNo + @"'
               GROUP BY C.TOTAL_QTY),                                  
                        CAST(SUM(C.TOTAL_QTY) AS DECIMAL(18, 0)) AS TOTAL_ORDER_QTY ,
                        TOTAL_CUT_QTY = ( SELECT    A.CutQty - ISNULL(B.ReduceQty, 0)
                                          FROM      ( SELECT    SUM(c.RATIO * b.PLYS) AS CutQty
                                                      FROM      dbo.CUT_LAY AS a WITH ( NOLOCK )
                                                                INNER JOIN dbo.CUT_LAY_HD c
                                                                WITH ( NOLOCK ) ON c.LAY_ID = A.LAY_ID
                                                                INNER JOIN dbo.CUT_LAY_DT
                                                                AS b WITH ( NOLOCK ) ON c.LAY_TRANS_ID = b.LAY_TRANS_ID
                                                                INNER JOIN JO_DT E WITH ( NOLOCK ) ON A.JOB_ORDER_NO = E.JO_NO
                                                                          AND B.COLOR_CD = E.COLOR_CODE
                                                                          AND ( C.SIZE_CD
                                                                          + ' '
                                                                          + C.SIZE_CD2 ) = ( E.SIZE_CODE1
                                                                          + ' '
                                                                          + E.SIZE_CODE2 )
                                                      WHERE     EXISTS ( SELECT
                                                                          JO_NO
                                                                         FROM
                                                                          JO_HD WITH ( NOLOCK )
                                                                         WHERE
                                                                          STATUS not in ('D','X') AND SC_NO = '" + GoNo + @"'
                                                                          AND JO_NO = A.JOB_ORDER_NO )
                                                                AND ( A.FACTORY_CD = '" +FactoryCd+@"' )
                                                                AND ( A.PRINT_STATUS = 'Y' )
                                                    ) A ,
                                                    ( SELECT    SUM(REDUCE_QTY) AS ReduceQty
                                                      FROM      CUT_bundle_reduce_TRX A
                                                                WITH ( NOLOCK )
                                                      WHERE     EXISTS ( SELECT
                                                                          JO_NO
                                                                         FROM
                                                                          JO_HD WITH ( NOLOCK )
                                                                         WHERE
                                                                          STATUS not in ('D','X') AND SC_NO = '" + GoNo + @"'
                                                                          AND JO_NO = A.JOB_ORDER_NO )
                                                                AND ( A.FACTORY_CD = '" +FactoryCd+@"' )
                                                    ) B
                                        ) ,
                        FINAL_QTY=(   SELECT   ISNULL(CUT_QTY,0) - ISNULL(CUT_REDUCE,0) - ISNULL(DISCREPANCY_QTY,0)
               FROM     ( SELECT    B.SC_NO ,
                                    SUM(QTY) AS CUT_QTY
                          FROM      dbo.CUT_BUNDLE_HD AS A
                                    INNER JOIN JO_HD AS B ON A.JOB_ORDER_NO = B.JO_NO
                          WHERE     SC_NO = '" + GoNo + @"'
                                    AND B.STATUS not in ('D','X')
                          GROUP BY  B.SC_NO
                        ) AS AA
                        LEFT JOIN ( SELECT  B.SC_NO ,
                                            SUM(REDUCE_QTY) AS CUT_REDUCE
                                    FROM    dbo.CUT_BUNDLE_REDUCE_TRX AS A
                                            INNER JOIN JO_HD AS B ON A.JOB_ORDER_NO = B.JO_NO
                                    WHERE   SC_NO = '" + GoNo + @"'
                                            AND B.STATUS not in ('D','X')
                                    GROUP BY B.SC_NO
                                  ) AS BB ON AA.SC_NO = BB.SC_NO
                        LEFT JOIN ( SELECT  C.SC_NO ,
                                            SUM(DISCREPANCY_QTY) AS DISCREPANCY_QTY
                                    FROM    dbo.PRD_JO_DISCREPANCY_PULLOUT_HD AS A
                                            INNER JOIN dbo.PRD_JO_DISCREPANCY_PULLOUT_TRX
                                            AS B ON A.DOC_NO = B.DOC_NO
                                            INNER JOIN JO_HD AS C ON B.JOB_ORDER_NO = C.JO_NO
                                    WHERE   PROCESS_CD IN ( 'CUT', 'PRT', 'EMB' )
                                            AND C.STATUS not in ('D','X') AND C.SC_NO = '" + GoNo + @"'
                                    GROUP BY SC_NO
                                  ) AS CC ON AA.SC_NO = CC.SC_NO		),                
                            
                        LAST_BED_NO = ( SELECT  MAX(LAY_NO)
                                        FROM    dbo.CUT_LAY AS a WITH ( NOLOCK )
                                                INNER JOIN JO_HD B WITH ( NOLOCK ) ON B.JO_NO = a.JOB_ORDER_NO
                                        WHERE B.STATUS not in ('D','X') AND  SC_NO = '" + GoNo + @"'
                                      )
                FROM    SC_HD A WITH ( NOLOCK )
                        INNER JOIN GEN_CUSTOMER B WITH ( NOLOCK ) ON A.CUSTOMER_CD = B.CUSTOMER_CD
                        INNER JOIN JO_HD C WITH ( NOLOCK ) ON A.SC_NO = C.SC_NO AND C.JO_NO IN (SELECT JO_NO FROM @dtjo)
                WHERE A.SC_NO = '" + GoNo + @"'
                GROUP BY A.SC_NO ,
                        SHORT_NAME    ";


        return MESComment.DBUtility.GetTable(SQL, "MES");
    }

    //Added By ZouShichang ON 2014.04.25 End

}
