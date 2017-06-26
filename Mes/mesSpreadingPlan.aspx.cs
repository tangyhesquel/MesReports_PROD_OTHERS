using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Mes_mesSpreadingPlan : pPage
{
    //S10C01638BODY1-001
    public double plysTotal, fabQtyTotal, Cuttabletotal;
    string factoryCd = "";
    string moNO = "";
    string NewType = "";
    //DataTable dtMergeSizes;

    string[] Show_Include_Wastage_FactoryCD = { "TIL", "EGV", "EAV" };
    bool Show_Include_Wastage = false;
    bool New_Allocation = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                factoryCd = Request.QueryString["site"].ToString();
            }
            else
            {
                factoryCd = "GEG";
            }

            if (Request.QueryString["svType"] != null)
            {
                HiddenField1.Value = Request.QueryString["svType"].ToString();
            }

            if (factoryCd.Contains("YMG"))
            {
                factoryCd = "YMG";
            }
            TxtFactroyCd.Text = factoryCd;

            if (factoryCd.Contains("EGM"))
            {
                cbMergSize.Visible = true;
                //cbMergSize.Checked = true;
            }
        }
        if (this.Request.QueryString.Get("Type") != null)
        {
            this.NewType = this.Request.QueryString.Get("Type").ToString();
        }
        if (!IsPostBack)
        {
	    //Modified by YeeHou on 14-07-2015 
            if (factoryCd == "TIL" || factoryCd.ToUpper().Equals("YMG"))
            {
                //Modified by Zikuan MES-102
                //cbPage.Checked = true;
                cbBnm.Checked = true;
            }
            //Added by Zikuan MES-102
            if (factoryCd == "TIL" || factoryCd == "EGM")
            {
                cbNewFormat.Checked = true;
                cbPage.Checked = false;
                cbPage.Visible = false;
            }
            else
            {
                cbNewFormat.Checked = false;
            }

            //Added by YeeHou on 28-07-2015
            if (factoryCd == "TIL")
            {
                cbSort.Checked = true;
            }
            //End Added MES-102
            if (factoryCd.ToUpper().Equals("EAV") || factoryCd.ToUpper().Equals("EGV") || factoryCd.ToUpper().Equals("EHV"))
            {
                FlagList.SelectedIndex = 2;
                cbNA.Enabled = true;
                cbFabric.Checked = true;
            }
            else if (factoryCd.ToUpper().Equals("YMG") || factoryCd.ToUpper().Equals("GEG")) //Added by MF on 20141114, enable checkbox for GEG
            {
                FlagList.SelectedIndex = 1;
                cbNA.Enabled = true;
            }
            else
            {
                cbFabric.Checked = false;
            }
        }
        if (Request.QueryString.Get("moNo") != null)
        {
            moNO = Request.QueryString.Get("moNo").ToString().Trim();
            if (moNO != "" && this.txtMoNo.Text.Trim().Equals(""))
            {
                txtMoNo.Text = moNO;
                divBody.InnerHtml = "";
                SetQuery();
            }
            else
            {
                moNO = this.txtMoNo.Text.Trim();
            }
        }
    }

    private void SetcbNA()
    {
        if (factoryCd.Equals("EAV") || factoryCd.Equals("EGV") || factoryCd.Equals("YMG") || factoryCd.Equals("EHV") || factoryCd.ToUpper().Equals("GEG")) //Added by MF on 20141114, enable checkbox for GEG
        {
            DataTable dt_Exists = MESComment.MesOutSourcePriceSql.JudageNewOrOld(txtMoNo.Text);

            if (dt_Exists.Rows.Count > 0)
            {
                this.New_Allocation = true;
            }
            else
            {
                this.New_Allocation = false;
            }
        }
    }

    private void SetQuery()
    {
        SetcbNA();
        this.NODATA.Visible = false;
        if (moNO.Equals(""))
        {
            moNO = txtMoNo.Text.Trim();
        }
        for (int i = 0; i < Show_Include_Wastage_FactoryCD.Length; i++)
        {
            if (factoryCd.ToUpper().Equals(Show_Include_Wastage_FactoryCD[i].ToString()))
            {
                Show_Include_Wastage = true;
            }
        }
        divBody.InnerHtml = "";
        DataTable dtHeader = MESComment.MesOutSourcePriceSql.GetHeaders(moNO, txtMarkerId.Text, cbBnm.Checked);
        Cuttabletotal = 0;
        if (dtHeader.Rows.Count > 0)
        {
            //DataTable dtMergeSizes = MESComment.MesOutSourcePriceSql.GetMergeSizesByMo(moNO);
            DataTable dtMergeSizes = MESComment.MesOutSourcePriceSql.GetMergeSizesByMo(moNO);
            DataTable dtSpreadings = MESComment.MesOutSourcePriceSql.GetSpreadingsByMo(moNO);
            DataTable dtAllocations = MESComment.MesOutSourcePriceSql.GetAllocationsByMo(moNO, factoryCd, cbSort.Checked ? 1 : 0);   
            DataTable dtNtransfers = MESComment.MesOutSourcePriceSql.GetNtransfersByMo(moNO);
            DataTable dtPtransfers = MESComment.MesOutSourcePriceSql.GetPtransfersByMo(moNO);


      
            foreach (DataRow row_Header in dtHeader.Rows)
            {
                Cuttabletotal += 1;
                string SQL1 = "BED_NO='" + row_Header["BED_NO"].ToString() + "' AND MARKER_ID = '" + row_Header["MARKER_ID"].ToString() + "'";
                string SQL2 = "CUT_TABLE_NO='" + row_Header["BED_NO"].ToString() + "' AND MARKER_ID = '" + row_Header["MARKER_ID"].ToString() + "'";
                
                DataRow[] row_Spreadings = dtSpreadings.Select(SQL1);
                DataRow[] row_MergeSizes = dtMergeSizes.Select(SQL2);//2013.07.15 ZouShCh ADD
                DataRow[] row_Allocations = dtAllocations.Select(SQL2);
                DataRow[] row_Ntransfers = dtNtransfers.Select(SQL2);
                DataRow[] row_Ptransfers = dtPtransfers.Select(SQL2);

                GeneratePageBody_New(row_Header, row_MergeSizes, row_Spreadings, row_Allocations, row_Ntransfers, row_Ptransfers);
            }
            GeneratePageBottom();
        }
        else
        {
            this.NODATA.Visible = true;
            this.divBody.InnerHtml = "";
            this.divBottom.InnerHtml = ""; //Added by Jin Song - Bug Fix 7/23/2014
        }
        this.cbNA.Checked = this.New_Allocation;
    }

    private void MarkerDetail(string moNo, string markerId, string bedNo)
    {

        string strMarkerHtml;
        string[] strMarkerDetailRows;
        divBody.InnerHtml += "<tr><td><table width='100%' border='1' cellspacing='0' cellPadding='0' style='font-size:11px;border-collapse:collapse'>";

        //DataTable MarkerRatioInfo = MESComment.MesOutSourcePriceSql.GetMarkerRatioInfoNew(moNo, markerId, bedNo);
        DataTable MarkerRatioInfo = MESComment.MesOutSourcePriceSql.GetMarkerRatioInfo(moNo, markerId, bedNo);
        strMarkerHtml = "<tr ><td colspan='" + MarkerRatioInfo.Rows.Count * 2 + "'>" + markerId + " / Ply Segregation Cut Lot No:" + bedNo + "</td></tr><tr >";

        //DataTable MarkerCombineCountNum = MESComment.MesOutSourcePriceSql.GetMakerCombineCountNew(moNo, markerId, bedNo);

        DataTable MarkerCombineCountNum = MESComment.MesOutSourcePriceSql.GetMakerCombineCount(moNo, markerId, bedNo);

        for (int j = 0; j < MarkerCombineCountNum.Rows.Count; j++)
        {

            strMarkerHtml += "<th  style='text-align:center' colspan='2'>" + MarkerCombineCountNum.Rows[j]["Combine_size_cd"].ToString() + "</th>";

        }

        strMarkerHtml += "</tr>";

        //DataTable sizeCount = MESComment.MesOutSourcePriceSql.GetMarkerRatioSizeCountMaxNew(moNo, markerId, bedNo);
        DataTable sizeCount = MESComment.MesOutSourcePriceSql.GetMarkerRatioSizeCountMax(moNo, markerId, bedNo);

        strMarkerDetailRows = new string[Convert.ToInt32(sizeCount.Rows[0][0].ToString())];
        for (int k = 0; k < MarkerRatioInfo.Rows.Count; k++)
        {

            //DataTable MarkerCombineSizeRatioDetail = MESComment.MesOutSourcePriceSql.GetMarkerCombineSizeDetailSize(moNo, markerId, bedNo, MarkerRatioInfo.Rows[k]["Combine_size_cd"].ToString());
            DataTable MarkerCombineSizeRatioDetail = MESComment.MesOutSourcePriceSql.GetMarkerCombineSizeRatioDetailSize(moNo, markerId, bedNo, MarkerRatioInfo.Rows[k]["Combine_size_cd"].ToString(), MarkerRatioInfo.Rows[k]["Ratio_seq"].ToString());

            for (int h = 0; h < Convert.ToInt32(sizeCount.Rows[0][0].ToString()); h++)
            {

                if (h < MarkerCombineSizeRatioDetail.Rows.Count)
                {
                    strMarkerDetailRows[h] += "<td width='40px' style='text-align:center'>" + MarkerCombineSizeRatioDetail.Rows[h]["Size_cd"].ToString() + "</td>";
                    strMarkerDetailRows[h] += "<td width='40px' style='text-align:center'>" + MarkerCombineSizeRatioDetail.Rows[h]["qty"].ToString() + "</td>";
                }
                else
                {
                    strMarkerDetailRows[h] += "<td width='40px'> </td>";
                    strMarkerDetailRows[h] += "<td width='40px'> </td>";
                }

            }

        }
        for (int j = 0; j < strMarkerDetailRows.Length; j++)
        {
            strMarkerHtml += strMarkerDetailRows[j] + "</tr>";
        }

        for (int j = 0; j < MarkerRatioInfo.Rows.Count; j++)
        {

            strMarkerHtml += "<td width='40px' colspan='2' style='text-align:center' >" + MarkerRatioInfo.Rows[j]["ply"].ToString() + "</td>";
        }
        for (int j = 0; j < MarkerRatioInfo.Rows.Count; j++)
        {


            strMarkerHtml += "</tr>";
        }

        divBody.InnerHtml += strMarkerHtml;

        divBody.InnerHtml += "</table></td></tr>";


    }



    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divBody.InnerHtml = "";
        SetQuery();
    }

    private void GeneratePageBody(DataRow row)
    {
        string[] str = null;
        switch (FlagList.SelectedValue)
        {//选择显示语言;
            case "English":
                str = new string[] { "GO", "Cut Lot No", "Marker Id", "Yard", "Inch", "Creator", "Date", "Marker Order No", "Part Type", "Size Desc", "Actual length :", "Marker Name", "Sytle Desc", "Description", "Marker Remarks" };
                break;
            case "Chinese":
                str = new string[] { "GO", "裁批号", "唛架ID", "(唛架)码", "英寸", "开单员", "开单日期", "唛架单号", "部位", "尺码分配", "Actual length :", "唛架名", "款式描述", "布备注", "唛架备注" };
                break;
            case "Vietnamese":
                str = new string[] { "GO", "Số bàn", "Số sơ đồ", "Yard", "Inch", "Người làm", "Ngày", "Số tt sơ đồ", "Loại", "Tỉ lệ size", "Kích thước thực tế", "Tên Sơ Đồ", "Mô tả mã hàng", "Mô tả chi tiết", "Marker Remarks" };
                break;
        }
        int seq = 0;

        int iblank = 0;
        divBody.InnerHtml += "<tr><td><table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
        divBody.InnerHtml += "<tr>";
        divBody.InnerHtml += "<td class='tr7style' width='95'>" + str[seq++].ToString()/*GO*/ + "</td><td>" + row["GO_NO"] + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='60'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "裁批号" : "Cut Lot No")*/ + "</td><td>" + row["BED_NO"] + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='60'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "唛架ID" : "Marker Id")*/ + "</td><td>" + row["MARKER_ID"] + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='40'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "(唛架)码" : "Yard")*/ + "</td><td>" + row["YARD"] + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='40'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "英寸" : "Inch")*/ + "</td><td>" + row["INCH"] + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='40'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "开单员" : "Creator")*/ + "</td><td>" + row["CREATOR"] + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='50'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "开单日期" : "Date")*/ + "</td><td width='80'>" + row["PRINT_DATE"] + "</td>";
        divBody.InnerHtml += "</tr>";
        divBody.InnerHtml += "<td class='tr7style' width='95'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "唛架单号" : "Marker Order No")*/ + "</td><td>" + row["MO_NO"] + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='60'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "部位" : "Part Type")*/ + "</td><td>" + row["PART_TYPE"] + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='60'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "尺码分配" : "Size Desc")*/ + "</td><td colspan=7>" + row["SIZE_DESC"] + "</td>";
        if (factoryCd.Equals("EAV") || factoryCd.Equals("EGV"))
        {
            divBody.InnerHtml += "<td class='tr7style' width='60'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Actual length :" : "Actual length :")*/ + "</td><td></td>";
        }
        else
        {
            seq++;
        }
        divBody.InnerHtml += "<tr>";
        divBody.InnerHtml += "<td class='tr7style' width='95'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "唛架名" : "Marker Name")*/ + "</td><td colspan=13>" + row["MARKER_NAME"] + "</td>";
        divBody.InnerHtml += "</tr>";
        divBody.InnerHtml += "<tr>";
        divBody.InnerHtml += "<td class='tr7style' width='95'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "款式描述" : "Sytle Desc")*/ + "</td><td colspan=13>" + row["STYLE_DESC"] + "</td>";
        divBody.InnerHtml += "</tr>";
        divBody.InnerHtml += "<tr>";
        divBody.InnerHtml += "<td class='tr7style' width='95'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "布备注" : "Description")*/ + "</td><td colspan=13>" + row["DESCRIPTION"] + "</td>";
        divBody.InnerHtml += "</tr>";
        divBody.InnerHtml += "<tr>";
        divBody.InnerHtml += "<td class='tr7style' width='95'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "唛架备注" : "Marker Remarks")*/ + "</td><td colspan=13>" + row["MO_REMARKS"] + "</td>";
        divBody.InnerHtml += "</tr>";
        divBody.InnerHtml += "</table></td></tr>";

        switch (FlagList.SelectedValue)
        {//选择显示语言;
            case "English":
                str = new string[] { "Spreading", "Color Code", "Color Desc", "Pattern", "Plys", "Cut Dir", "Mat Typ", "Thickness", "Fabric Description", "Combo", "Fab Qty", "Shade Lot", "Shrinkage", "Batch No", "Fab Wid", "Fab Qty (Inc.Wastage)", "Total" };
                break;
            case "Chinese":
                str = new string[] { "拉布", "颜色代码", "颜色描述", "样式", "层数", "Cut Dir", "Mat Typ", "Thickness", "布料", "布料颜色", "用布数", "色级", "缩水", "缸号", "布封", "领布数", "总数" };
                break;
            case "Vietnamese":
                str = new string[] { "Bàn chia tỉ lệ", "Mã màu", "Tên màu", "Loại vải", "Lớp", "Cut Dir", "Mat Typ", "Thickness", "Mô tả vải", "Màu", "Số lượng vải", "Shade lot", "Độ co rút", "Batch No", "Khổ vải", "Số lượng vải ( bao gồm hao phí)", "Tổng số" };
                break;
        }
        seq = 0;

        divBody.InnerHtml += "<tr><td><STRONG>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "拉布" : "Spreading")*/ + "</STRONG></td></tr>";
        divBody.InnerHtml += "<tr><td><table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
        divBody.InnerHtml += "<tr>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "颜色代码" : "Color Code")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "颜色描述" : "Color Desc")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "样式" : "Pattern")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "层数" : "Plys")*/ + "</td>";
        if (factoryCd.ToUpper() == "EGM")
        {
            divBody.InnerHtml += "<td class='tr7style' >" + str[seq++].ToString()/*Cut Dir*/;
            divBody.InnerHtml += "</td><td class='tr7style' >" + str[seq++].ToString()/*Mat Typ*/;
            divBody.InnerHtml += "</td><td class='tr7style' >" + str[seq++].ToString()/*Thickness*/+ "</td>";

        }
        else
        {//要考虑if中字段占的位数;
            seq++;
            seq++;
            seq++;
        }
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "布料" : "Fabric Description")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "布料颜色" : "Combo")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "用布数" : "Fab Qty")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "色级" : "Shade Lot")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "缩水" : "Shrinkage")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "缸号" : "Batch No")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "布封" : "Fab Wid")*/ + "</td>";
        if (Show_Include_Wastage)
        {
            divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "领布数" : "Fab Qty (Inc.Wastage)")*/ + "</td>";
        }
        else
        {
            seq++;
        }
        divBody.InnerHtml += "</tr>";
        DataTable dtSpread = MESComment.MesOutSourcePriceSql.GetSpreadings(txtMoNo.Text.Trim(), row["MARKER_ID"].ToString(), row["BED_NO"].ToString());
        double plysTotal = 0;
        double fabQtyTotal = 0;
        double fabQty2Total = 0;
        foreach (DataRow sRow in dtSpread.Rows)
        {
            divBody.InnerHtml += "<tr>";
            divBody.InnerHtml += "<td>" + sRow["COLOR_CD"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["COLOR_DESC"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["PATTERN_TYPE_DESC"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["PLYS"] + "</td>";
            if (factoryCd.ToUpper() == "EGM")
            {
                divBody.InnerHtml += "<td>" + sRow["CUT_DIRECTION_TYPE"] + "</td>";
                divBody.InnerHtml += "<td>" + sRow["MATERIAL_TYPE"] + "</td>";
                divBody.InnerHtml += "<td>" + sRow["FAB_THICKNESS_TYPE"] + "</td>";
            }
            divBody.InnerHtml += "<td>" + sRow["PPO_NO"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["COMBO_NAME"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["FAB_QTY"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["SHADE_LOT"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["SHRINKAGE"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["BATCH_NO"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["WIDTH"] + "</td>";
            if (Show_Include_Wastage)
            {//"领布数" : "Fab Qty (Inc.Wastage)")
                divBody.InnerHtml += "<td >" + (double.Parse(sRow["FAB_QTY"].ToString()) * 1.015).ToString("#,##0.00") + "</td>";
                fabQty2Total += double.Parse(sRow["FAB_QTY"].ToString()) * 1.015;
            }
            divBody.InnerHtml += "</tr>";
            plysTotal += double.Parse(sRow["PLYS"].ToString());
            fabQtyTotal += double.Parse(sRow["FAB_QTY"].ToString());
            if (sRow["COLOR_DESC"].ToString().Length > 15)
            {
                iblank += 1;
            }
            iblank += 1;
        }
        divBody.InnerHtml += "<tr>";
        divBody.InnerHtml += "<td>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "总数" : "Total")*/ + "</td>";
        divBody.InnerHtml += "<td></td>";
        divBody.InnerHtml += "<td></td>";
        divBody.InnerHtml += "<td>" + plysTotal.ToString("#,##0") + "</td>";
        divBody.InnerHtml += "<td></td>";
        divBody.InnerHtml += "<td></td>";
        divBody.InnerHtml += "<td>" + fabQtyTotal.ToString("#,##0.00") + "</td>";
        divBody.InnerHtml += "<td></td>";
        divBody.InnerHtml += "<td></td>";
        divBody.InnerHtml += "<td></td>";
        divBody.InnerHtml += "<td></td>";
        if (Show_Include_Wastage)
        {//"领布数" : "Fab Qty (Inc.Wastage)")
            divBody.InnerHtml += "<td>" + fabQty2Total.ToString("#,##0.00") + "</td>";
        }
        divBody.InnerHtml += "</tr>";
        divBody.InnerHtml += "</table></td></tr>";

        switch (FlagList.SelectedValue)
        {//选择显示语言;
            case "English":
                str = new string[] { "JO Allocation", "Job Order No", "Buyer PO DEL Date", "Color Cd", "Ply", "Fab Qty", "Fab Qty(Inc.Wastage)" };
                break;
            case "Chinese":
                str = new string[] { "JO 分配", "Job Order No", "Buyer PO DEL Date", "颜色代码", "层数", "用布数", "领布数" };
                break;
            case "Vietnamese":
                str = new string[] { "SL cho từng JO", "Số JO:", "Buyer PO DEL Date", "Mã màu", "Số lớp", "Số lượng vải", "Số lượng vải( bao gồm hao phí)" };
                break;
        }
        seq = 0;

        divBody.InnerHtml += "<tr><td><STRONG>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "JO 分配" : "JO Allocation")*/ + "</STRONG></td></tr>";
        divBody.InnerHtml += "<tr><td><table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
        divBody.InnerHtml += "<tr>";
        divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*Job Order No*/+ "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "BUYER PO DEL Date" : "BUYER PO DEL Date")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "颜色代码" : "Color Cd")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "层数" : "Ply")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "用布数" : "Fab Qty")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='120' nowrap='nowrap'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "领布数" : "Fab Qty(Inc.Wastage)")*/ + "</td>";
        divBody.InnerHtml += "</tr>";
        DataTable dtAllocation = MESComment.MesOutSourcePriceSql.GetAllocations(txtMoNo.Text.Trim(), row["MARKER_ID"].ToString(), row["BED_NO"].ToString());
        foreach (DataRow aRow in dtAllocation.Rows)
        {
            divBody.InnerHtml += "<tr style='font-weight:bolder'>";
            divBody.InnerHtml += "<td>" + aRow["JO_NO"] + "</td>";
            divBody.InnerHtml += "<td>" + aRow["BUYER_PO_DEL_DATE"] + "</td>";
            divBody.InnerHtml += "<td>" + aRow["COLOR_CD"] + "</td>";
            divBody.InnerHtml += "<td>" + aRow["PLYS"] + "</td>";
            divBody.InnerHtml += "<td>" + aRow["FABQTY"] + "</td>";
            //divBody.InnerHtml += "<td >" + (double.Parse(aRow["FABQTY"].ToString()) * 1.015).ToString("#,##0.00") + "</td>";
            divBody.InnerHtml += "<td>" + aRow["FABQTY2"] + "</td>";
            divBody.InnerHtml += "</tr>";

            iblank += 1;
        }
        divBody.InnerHtml += "</table></td></tr>";

        if (New_Allocation)
        {
            DataTable dtNewAllocation = MESComment.MesOutSourcePriceSql.GetNewAllcations(factoryCd, txtMoNo.Text.Trim(), row["MARKER_ID"].ToString(), row["BED_NO"].ToString());
            if (dtNewAllocation.Rows.Count > 0)
            {
                int Cols = dtNewAllocation.Columns.Count;
                int[] Total = new int[Cols];
                //Total[Total.Length - 1] = Convert.ToInt32(Total_PLYS);
                //this.gvNewAllocation.DataSource = dtNewAllocation;
                //this.gvNewAllocation.DataBind();
                //this.divNewAllocation.Visible = true;
                this.divBody.InnerHtml += "<tr><td><STRONG>JO New Allocation</STRONG></td></tr>";
                this.divBody.InnerHtml += "<tr><td><table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'><tr>";
                for (int i = 0; i < dtNewAllocation.Columns.Count; i++)
                {
                    this.divBody.InnerHtml += "<td class='tr7style'>" + dtNewAllocation.Columns[i].ColumnName.ToString() + "</td>";
                }
                this.divBody.InnerHtml += "</tr><tr>";
                foreach (DataRow NewRow in dtNewAllocation.Rows)
                {
                    for (int i = 0; i < dtNewAllocation.Columns.Count; i++)
                    {
                        if (i == dtNewAllocation.Columns.Count - 1)
                        {
                            this.divBody.InnerHtml += "<td class='tr7style'>" + NewRow[i] + "</td>";
                        }
                        else
                        {
                            this.divBody.InnerHtml += "<td>" + NewRow[i] + "</td>";
                        }
                        if (i > 0)
                        {
                            try
                            {
                                Total[i] += Int32.Parse(NewRow[i].ToString());
                            }
                            catch (Exception)
                            {
                                Total[i] += 0;
                            }
                        }
                    }
                    this.divBody.InnerHtml += "</tr>";
                }

                for (int i = 0; i < Total.Length; i++)
                {
                    if (i == 0)
                    {
                        this.divBody.InnerHtml += "<tr class='tr7style'><td>Total:</td><td></td>";//新增了颜色COL；
                    }
                    else if (i > 1)
                    {
                        this.divBody.InnerHtml += "<td>" + Total[i].ToString() + "</td>";
                    }
                }
                this.divBody.InnerHtml += "</tr>";
                divBody.InnerHtml += "</table></td></tr>";
            }
        }

        int AllocationCols = str.Length;
        switch (FlagList.SelectedValue)
        {//选择显示语言;
            case "English":
                str = new string[] { "JO Transfer", "Transfer JO From", "Size Cd", "Color Cd", "(-)Trans Qty", "Transfer JO To", "Cut Lot No To", "(+)Trans Qty", "Fabric Barcode", "Actual Plys", "Color (Màu sắc)", "Batch no. (Số batch)", "Roll no. (Số cây vải)", "Sticker yards (Số yard trên tem)", "Actual Yards: (Số yard thực sự khi đo)", "No. of layers (Tổng số lớp)", "Shade no (Phần trăm màu).", "Balance yards (Số yard còn lại)", "Damage yards (Số yard bị hư)", "MU Yards", "Splice (Tổng số chỗ nối)", "Remarks (Chú ý)", "Record By", "Spreading by:", "Date & Time", "Cutting Line", "Fabric Roll Info.", "ROLL NO. / YARDAGE", "COLOUR", "LOT NO.", "QTY", "PREPARED BY", "COUNTER", "CHECK SIZE", "DATE" };
                break;
            case "Chinese":
                str = new string[] { "抽单情况", "Transfer JO From", "尺码代码", "颜色代码", "(-)抽数", "Transfer JO To", "裁床号", "(+)抽数", "布料条码", "实际拉布", "Color (Màu sắc)", "Batch no. (Số batch)", "Roll no. (Số cây vải)", "Sticker yards (Số yard trên tem)", "Actual Yards: (Số yard thực sự khi đo)", "No. of layers (Tổng số lớp)", "Shade no (Phần trăm màu).", "Balance yards (Số yard còn lại)", "Damage yards (Số yard bị hư)", "MU Yards", "Splice (Tổng số chỗ nối)", "Remarks (Chú ý)", "记录员", "Spreading by:", "时间", "组别", "布料数据", "卷号/码数", "颜色", "批号", "数量", "PREPARED BY", "COUNTER", "CHECK SIZE", "DATE" };
                break;
            case "Vietnamese":
                str = new string[] { "Chuyển JO", "Chuyển từ JO", "Size", "Mã màu", "(-) Số lượng chuyển", "Transfer JO To", "Chuyển đến bàn", "(+) Số lượng nhận", "Mã Barcode vải", "Số lớp thực tế", "Color (Màu sắc)", "Batch no. (Số batch)", "Roll no. (Số cây vải)", "Sticker yards (Số yard trên tem)", "Actual Yards: (Số yard thực sự khi đo)", "No. of layers (Tổng số lớp)", "Shade no (Phần trăm màu).", "Balance yards (Số yard còn lại)", "Damage yards (Số yard bị hư)", "MU Yards", "Splice (Tổng số chỗ nối)", "Remarks (Chú ý)", "Được ghi bởi", "Người trải vải:", "Ngày & Tháng", "Nhóm cắt", "Thông tin cây vải", "Số cây/ Số yard", "Màu", "Số Lô", "Số lượng", "PREPARED BY", "COUNTER", "CHECK SIZE", "Date" };
                break;
        }
        seq = 0;
        DataTable dtNTrans = MESComment.MesOutSourcePriceSql.GetNtransfers(txtMoNo.Text.Trim(), row["MARKER_ID"].ToString(), row["BED_NO"].ToString());
        DataTable dtPTrans = MESComment.MesOutSourcePriceSql.GetPtransfers(txtMoNo.Text.Trim(), row["MARKER_ID"].ToString(), row["BED_NO"].ToString());
        if (1 == 1)//factoryCd.ToUpper() != "EGM")
        {
            if (dtNTrans.Rows.Count > 0)
            {
                divBody.InnerHtml += "<tr><td colspan='" + AllocationCols + "'><STRONG>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "抽单情况" : "JO Transfer")*/ + "</STRONG></td></tr>";
                divBody.InnerHtml += "<tr><td colspan='" + AllocationCols + "'><table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
                divBody.InnerHtml += "<tr>";
                divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*Transfer JO From*/ + "</td>";
                divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "尺码代码" : "Size Cd")*/ + "</td>";
                divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "颜色代码" : "Color Cd")*/ + "</td>";
                divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "(-)抽数" : "(-)Trans Qty")*/ + "</td>";
                divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Transfer JO to" : "Transfer JO To")*/ + "</td>";
                divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "裁床号" : "Cut Lot No To")*/ + "</td>";
                divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "(+)抽数" : "Size Cd")*/ + "</td>";
                divBody.InnerHtml += "</tr>";
                iblank = iblank + 2;
                foreach (DataRow nRow in dtNTrans.Rows)
                {
                    divBody.InnerHtml += "<tr>";
                    divBody.InnerHtml += "<td>" + nRow["JO_NO"] + "</td>";
                    divBody.InnerHtml += "<td >" + nRow["SIZE_CD"] + "</td>";
                    divBody.InnerHtml += "<td >" + nRow["COLOR_CD"] + "</td>";
                    divBody.InnerHtml += "<td >" + nRow["QTY_FROM"] + "</td>";
                    divBody.InnerHtml += "<td>" + nRow["JO_TO"] + "</td>";
                    divBody.InnerHtml += "<td>" + nRow["CUT_LOT_TO"] + "</td>";
                    divBody.InnerHtml += "<td >+" + nRow["QTY_TO"] + "</td>";
                    divBody.InnerHtml += "</tr>";
                    iblank += 1;
                }
                divBody.InnerHtml += "</table></td></tr>";
            }
            else
            {//要考虑if中字段占的位数;
                //divBody.InnerHtml += "</table></td></tr>";
                seq++;
                seq++;
                seq++;
                seq++;
                seq++;
                seq++;
                seq++;
                seq++;
            }
        }
        AllocationCols = str.Length;
        divBody.InnerHtml += "<tr><td>&nbsp;</td></tr>";//留空;
        if (factoryCd.ToUpper() != "TIL")
        {

            divBody.InnerHtml += "<tr>";
            divBody.InnerHtml += "	<td>";
            divBody.InnerHtml += "		<table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
            int colspan = 1;
            if (!cbFabric.Checked)
            {
                divBody.InnerHtml += "			<tr>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "布料条码" : "Fabric Barcode")*/ + "</td>";
                divBody.InnerHtml += "			     <td></td>";
                divBody.InnerHtml += "			     <td></td>";
                divBody.InnerHtml += "			     <td></td>";
                divBody.InnerHtml += "			     <td></td>";
                divBody.InnerHtml += "			     <td></td>";
                divBody.InnerHtml += "		    </tr>";
                divBody.InnerHtml += "		    <tr>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "实际拉布" : "Actual Plys")*/ + "</td>";
                divBody.InnerHtml += "			     <td></td>";
                divBody.InnerHtml += "			     <td></td>";
                divBody.InnerHtml += "			     <td></td>";
                divBody.InnerHtml += "			     <td></td>";
                divBody.InnerHtml += "			     <td></td>";
                divBody.InnerHtml += "		    </tr>";
                divBody.InnerHtml += "		</table>";
                divBody.InnerHtml += "	</td>";
                divBody.InnerHtml += "</tr>";
                divBody.InnerHtml += "<tr>";
                divBody.InnerHtml += "	<td>";
                divBody.InnerHtml += "		<table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
                divBody.InnerHtml += "			<tr>";
                //要考虑else中字段占的位数;
                seq += 12;
            }
            else
            {
                colspan = 2;
                //要考虑if中字段占的位数;
                seq += 2;
                divBody.InnerHtml += "			<tr>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Color(Màu sắc)" : "Color(Màu sắc)")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Batch no.(Số batch)" : "Batch no.(Số batch)")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Roll no.(Số cây vải)" : "Roll no.(Số cây vải)")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Sticker yards(Số yard trên tem)" : "Sticker yards(Số yard trên tem)"*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Actual Yards:(Số yard thực sự khi đo)" : "Actual Yards:(Số yard thực sự khi đo)")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "No. of layers(Tổng số lớp)" : "No. of layers(Tổng số lớp)")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Shade no(Phần trăm màu)." : "Shade no(Phần trăm màu).")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Balance yards(Số yard còn lại)" : "Balance yards(Số yard còn lại)")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Damage yards(Số yard bị hư)" : "Damage yards(Số yard bị hư)")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "MU Yards" : "MU Yards")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Splice(Tổng số chỗ nối)" : "Splice(Tổng số chỗ nối)")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Remarks (Chú ý)" : "Remarks (Chú ý)")*/ + "</td>";
                divBody.InnerHtml += "		    </tr>";
                for (int rows = 0; rows < 10; rows++)
                {//添加3行空白行;
                    divBody.InnerHtml += "<tr><td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "</tr>";
                }
            }
            divBody.InnerHtml += " <tr>";
            divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "记录员" : "Record By")*/ + "</td>";
            divBody.InnerHtml += "			     <td  colspan='" + colspan + "'></td>";
            if (cbFabric.Checked)
            {
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Spreading by:" : "Spreading by:")*/ + "</td>";
                divBody.InnerHtml += "			     <td  colspan='" + colspan + "'></td>";
            }
            else
            {
                seq++;
            }
            divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "时间" : "Date & Time")*/ + "</td>";
            divBody.InnerHtml += "			     <td  colspan='" + colspan + "'></td>";
            divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "组别" : "Cutting Line")*/ + "</td>";
            divBody.InnerHtml += "			     <td colspan='" + colspan + "'></td>";
            divBody.InnerHtml += "		    </tr>";
            divBody.InnerHtml += "		</table>";
            divBody.InnerHtml += "	</td>";
            divBody.InnerHtml += "</tr>";
            iblank = iblank + 3;
        }
        else
        {//要考虑if中字段占的位数;
            seq++;
            seq++;
            seq++;
            seq++;
            seq++;
            seq += 12;
            seq++;
        }
        if (cbPage.Checked == true)
        {
            divBody.InnerHtml += "<tr>";
            divBody.InnerHtml += "	<td height='20' style='font-size:13px;'><STRONG>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "布料数据" : "Fabric Roll Info.")*/ + "</STRONG></td>";
            divBody.InnerHtml += "</tr>";
            divBody.InnerHtml += "<tr><td>";
            divBody.InnerHtml += "	<table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;'>";
            divBody.InnerHtml += "    <tr>";
            divBody.InnerHtml += "     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "卷号/码数" : "ROLL NO. / YARDAGE")*/ + "</td>";
            divBody.InnerHtml += "     <td class='tr7style' width='200'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "颜色" : "COLOUR")*/ + "</td>";
            divBody.InnerHtml += "     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "批号" : "LOT NO.")*/ + "</td>";
            divBody.InnerHtml += "     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "数量" : "QTY")*/ + "</td>";
            divBody.InnerHtml += "     <td class='tr7style' width='300' >&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>";
            divBody.InnerHtml += "    </tr>";
            if (factoryCd.ToUpper() != "TIL")
            {
                iblank = 35 - iblank;
            }
            else
            {
                iblank = 32 - iblank;
            }
            if (Cuttabletotal != 1)
            {
                iblank = iblank + 2;
            }
            iblank += 1;
            for (int i = 0; i < iblank; i++)
            {
                divBody.InnerHtml += "    <tr>";
                divBody.InnerHtml += "     <td height='20'>&nbsp;</td>";
                divBody.InnerHtml += "     <td height='20'>&nbsp;</td>";
                divBody.InnerHtml += "     <td height='20'>&nbsp;</td>";
                divBody.InnerHtml += "     <td height='20'>&nbsp;</td>";
                if (i == 0)
                {
                    divBody.InnerHtml += "     <td rowspan=" + iblank + ">&nbsp;</td>";
                }
                divBody.InnerHtml += "     </tr>";
            }
            int ibank2 = 4;

            for (int i = 0; i < ibank2; i++)
            {
                divBody.InnerHtml += "    <tr>";
                divBody.InnerHtml += "     <td height='20' >&nbsp;</td>";
                divBody.InnerHtml += "     <td height='20' >&nbsp;</td>";
                divBody.InnerHtml += "     <td height='20' >&nbsp;</td>";
                divBody.InnerHtml += "     <td  height='20'>&nbsp;</td>";
                if (i == 0)
                {
                    divBody.InnerHtml += "     <td  rowspan=4><TABLE  width='100%'  border='0' cellspacing='0' cellpadding='0' style='font-size:13px;'>";
                    divBody.InnerHtml += "<tr><td  height='20'>" + str[seq + 0].ToString()/*PREPARED BY:*/ + "</td></tr>";
                    divBody.InnerHtml += "<tr><td  height='20'>" + str[seq + 1].ToString()/*COUNTER:*/ + "</td></tr>";
                    divBody.InnerHtml += "<tr><td  height='20'>" + str[seq + 2].ToString()/*CHECK SIZE:*/+ "</td></tr>";
                    divBody.InnerHtml += "<tr><td  height='20'>" + str[seq + 3].ToString()/*DATE:*/+ "</td></tr></table></td>";
                }
                divBody.InnerHtml += "     </tr>";
            }
            //因为for循环不能用seq++;故此这里需要进行字段占位的控制;
            seq++;
            seq++;
            seq++;
            seq++;

            divBody.InnerHtml += "    </table>";
            divBody.InnerHtml += "</td></tr>";
        }
        else
        {//要考虑if中字段占的位数;
            seq++;
            seq++;
            seq++;
            seq++;
            seq++;
            seq++;
            seq++;
            seq++;
            seq++;
        }
        divBody.InnerHtml += "<tr style='page-break-after: always'><td>&nbsp;</td></tr>";
    }

    private void GeneratePageBody_New(DataRow row_Header, DataRow[] row_MergeSizes, DataRow[] row_Spreadings, DataRow[] row_Allocations, DataRow[] row_Ntransfers, DataRow[] row_Ptransfers)
    {
        string[] str = null;
        switch (FlagList.SelectedValue)
        {//选择显示语言;
            case "English":
                str = new string[] { "GO", "Cut Lot No", "Marker Id", "Yard", "Inch", "Creator", "Date", "Marker Order No", "Part Type", "Size Desc", "Actual length :", "Marker Name", "Sytle Desc", "Description", "Marker Remarks" };
                break;
            case "Chinese":
                str = new string[] { "GO", "裁批号", "唛架ID", "(唛架)码", "英寸", "开单员", "开单日期", "唛架单号", "部位", "尺码分配", "Actual length :", "唛架名", "款式描述", "布备注", "唛架备注" };
                break;
            case "Vietnamese":
                str = new string[] { "GO", "Số bàn", "Số sơ đồ", "Yard", "Inch", "Người làm", "Ngày", "Số tt sơ đồ", "Loại", "Tỉ lệ size", "Kích thước thực tế", "Tên Sơ Đồ", "Mô tả mã hàng", "Mô tả chi tiết", "Marker Remarks" };
                break;
        }
        int seq = 0;

        int iblank = 0;
        divBody.InnerHtml += "<tr><td><table width='100%'  border='0' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
        //Added By Zikuan MES-102
        if (cbNewFormat.Checked)
        {
            divBody.InnerHtml += "<tr><td valign='top' width='60%'>";
            divBody.InnerHtml += "<table width='100%'  border='0' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
            divBody.InnerHtml += "<tr><td>";
            divBody.InnerHtml += "<table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
        }
        else
        {
            divBody.InnerHtml += "<tr><td>";
            divBody.InnerHtml += "<table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
        }
        //End Added MES-102
        divBody.InnerHtml += "<tr><td class='tr7style' width='95'>" + str[seq++].ToString()/*GO*/ + "</td><td>" + row_Header["GO_NO"] + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='60'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "裁批号" : "Cut Lot No")*/ + "</td><td>" + row_Header["BED_NO"] + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='60'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "唛架ID" : "Marker Id")*/ + "</td><td>" + row_Header["MARKER_ID"] + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='40'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "(唛架)码" : "Yard")*/ + "</td><td>" + row_Header["YARD"] + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='40'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "英寸" : "Inch")*/ + "</td><td>" + row_Header["INCH"] + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='40'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "开单员" : "Creator")*/ + "</td><td>" + row_Header["CREATOR"] + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='50'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "开单日期" : "Date")*/ + "</td><td width='80'>" + row_Header["PRINT_DATE"] + "</td>";
        divBody.InnerHtml += "</tr>";
        divBody.InnerHtml += "<td class='tr7style' width='95'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "唛架单号" : "Marker Order No")*/ + "</td><td>" + row_Header["MO_NO"] + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='60'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "部位" : "Part Type")*/ + "</td><td>" + row_Header["PART_TYPE"] + "</td>";
        //divBody.InnerHtml += "<td class='tr7style' width='60'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "尺码分配" : "Size Desc")*/ + "</td><td colspan=7>" + row_Header["SIZE_DESC"] + "</td>";
        if (factoryCd.Equals("EAV") || factoryCd.Equals("EGV"))
        {
            divBody.InnerHtml += "<td class='tr7style' width='60'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "尺码分配" : "Size Desc")*/ + "</td><td colspan=7>" + row_Header["SIZE_DESC"] + "</td>";
            divBody.InnerHtml += "<td class='tr7style' width='60'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Actual length :" : "Actual length :")*/ + "</td><td></td>";
        }
        else
        {
            divBody.InnerHtml += "<td  align='right' class='tr7style' width='60'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "尺码分配" : "Size Desc")*/ + "</td><td colspan=9>" + row_Header["SIZE_DESC"] + "</td>";
            seq++;
        }
        divBody.InnerHtml += "<tr>";
        divBody.InnerHtml += "<td class='tr7style' width='95'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "唛架名" : "Marker Name")*/ + "</td><td colspan=13>" + row_Header["MARKER_NAME"] + "</td>";
        divBody.InnerHtml += "</tr>";
        divBody.InnerHtml += "<tr>";
        divBody.InnerHtml += "<td class='tr7style' width='95'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "款式描述" : "Sytle Desc")*/ + "</td><td colspan=13>" + row_Header["STYLE_DESC"] + "</td>";
        divBody.InnerHtml += "</tr>";
        divBody.InnerHtml += "<tr>";
        divBody.InnerHtml += "<td class='tr7style' width='95'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "布备注" : "Description")*/ + "</td><td colspan=13>" + row_Header["DESCRIPTION"] + "</td>";
        divBody.InnerHtml += "</tr>";
        divBody.InnerHtml += "<tr>";
        divBody.InnerHtml += "<td class='tr7style' width='95'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "唛架备注" : "Marker Remarks")*/ + "</td><td colspan=13>" + row_Header["MO_REMARKS"] + "</td>";
        divBody.InnerHtml += "</tr>";
        divBody.InnerHtml += "</table></td></tr>";

        //Merge Sizes Start 2013.07.15 ZouShCh Alter
        if (cbMergSize.Checked == true)
        {
            //DataTable dtCombineSizeCd = MESComment.MesOutSourcePriceSql.GetCombineSizeCdByMo(moNO, row_Header["MARKER_ID"].ToString(), row_Header["BED_NO"].ToString());
            //DataTable dtSizeCd = MESComment.MesOutSourcePriceSql.GetSizeCdByMo(moNO, row_Header["MARKER_ID"].ToString(), row_Header["BED_NO"].ToString());
            DataTable dtExistMerge = MESComment.MesOutSourcePriceSql.GetMergeSizeMarkerCutTableNo(moNO, row_Header["MARKER_ID"].ToString(), row_Header["BED_NO"].ToString());
            //if (dtCombineSizeCd.Rows.Count != dtSizeCd.Rows.Count)
            if (dtExistMerge.Rows.Count > 0)
            {

                MarkerDetail(moNO, row_Header["MARKER_ID"].ToString(), row_Header["BED_NO"].ToString());

            }


        }
        //Merge Sizes End 2013.07.15 ZouShCh Alter


        switch (FlagList.SelectedValue)
        {//选择显示语言;
            case "English":
                str = new string[] { "Spreading", "Color Code", "Color Desc", "Pattern", "Plys", "Cut Dir", "Mat Typ", "Thickness", "Fabric Description", "Combo", "Fab Qty", "Shade Lot", "Shrinkage", "Batch No", "Fab Wid", "Fab Qty (Inc.Wastage)", "Total" };
                break;
            case "Chinese":
                str = new string[] { "拉布", "颜色代码", "颜色描述", "样式", "层数", "Cut Dir", "Mat Typ", "Thickness", "布料", "布料颜色", "用布数", "色级", "缩水", "缸号", "布封", "领布数", "总数" };
                break;
            case "Vietnamese":
                str = new string[] { "Bàn chia tỉ lệ", "Mã màu", "Tên màu", "Loại vải", "Lớp", "Cut Dir", "Mat Typ", "Thickness", "Mô tả vải", "Màu", "Số lượng vải", "Shade lot", "Độ co rút", "Batch No", "Khổ vải", "Số lượng vải ( bao gồm hao phí)", "Tổng số" };
                break;
        }
        seq = 0;

        divBody.InnerHtml += "<tr><td><STRONG>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "拉布" : "Spreading")*/ + "</STRONG></td></tr>";
        divBody.InnerHtml += "<tr><td><table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
        divBody.InnerHtml += "<tr>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "颜色代码" : "Color Code")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "颜色描述" : "Color Desc")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "样式" : "Pattern")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "层数" : "Plys")*/ + "</td>";
        if (factoryCd.ToUpper() == "EGM")
        {
            divBody.InnerHtml += "<td class='tr7style' >" + str[seq++].ToString()/*Cut Dir*/;
            divBody.InnerHtml += "</td><td class='tr7style' >" + str[seq++].ToString()/*Mat Typ*/;
            divBody.InnerHtml += "</td><td class='tr7style' >" + str[seq++].ToString()/*Thickness*/+ "</td>";
        }
        else
        {//要考虑if中字段占的位数;
            seq++;
            seq++;
            seq++;
        }
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "布料" : "Fabric Description")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "布料颜色" : "Combo")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "用布数" : "Fab Qty")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "色级" : "Shade Lot")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "缩水" : "Shrinkage")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "缸号" : "Batch No")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "布封" : "Fab Wid")*/ + "</td>";
        if (Show_Include_Wastage)
        {
            divBody.InnerHtml += "<td class='tr7style'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "领布数" : "Fab Qty (Inc.Wastage)")*/ + "</td>";
        }
        else
        {
            seq++;
        }
        divBody.InnerHtml += "</tr>";
        //DataTable dtSpread = MESComment.MesOutSourcePriceSql.GetSpreadings(txtMoNo.Text.Trim(), row_Header["MARKER_ID"].ToString(), row_Header["BED_NO"].ToString());
        double plysTotal = 0;
        double fabQtyTotal = 0;
        double fabQty2Total = 0;
        //foreach (DataRow sRow in dtSpread.Rows)
        foreach (DataRow sRow in row_Spreadings)
        {
            divBody.InnerHtml += "<tr>";
            divBody.InnerHtml += "<td>" + sRow["COLOR_CD"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["COLOR_DESC"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["PATTERN_TYPE_DESC"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["PLYS"] + "</td>";
            if (factoryCd.ToUpper() == "EGM")
            {
                divBody.InnerHtml += "<td>" + sRow["CUT_DIRECTION_TYPE"] + "</td>";
                divBody.InnerHtml += "<td>" + sRow["MATERIAL_TYPE"] + "</td>";
                divBody.InnerHtml += "<td>" + sRow["FAB_THICKNESS_TYPE"] + "</td>";
            }
            divBody.InnerHtml += "<td>" + sRow["PPO_NO"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["COMBO_NAME"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["FAB_QTY"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["SHADE_LOT"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["SHRINKAGE"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["BATCH_NO"] + "</td>";
            divBody.InnerHtml += "<td>" + sRow["WIDTH"] + "</td>";
            if (Show_Include_Wastage)
            {//"领布数" : "Fab Qty (Inc.Wastage)")
                divBody.InnerHtml += "<td >" + (double.Parse(sRow["FAB_QTY"].ToString()) * 1.015).ToString("#,##0.00") + "</td>";
                fabQty2Total += double.Parse(sRow["FAB_QTY"].ToString()) * 1.015;
            }
            divBody.InnerHtml += "</tr>";
            plysTotal += double.Parse(sRow["PLYS"].ToString());
            fabQtyTotal += double.Parse(sRow["FAB_QTY"].ToString());
            if (sRow["COLOR_DESC"].ToString().Length > 15)
            {
                iblank += 1;
            }
            iblank += 1;
        }
        divBody.InnerHtml += "<tr>";
        divBody.InnerHtml += "<td>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "总数" : "Total")*/ + "</td>";
        divBody.InnerHtml += "<td></td>";
        divBody.InnerHtml += "<td></td>";
        divBody.InnerHtml += "<td>" + plysTotal.ToString("#,##0") + "</td>";
        divBody.InnerHtml += "<td></td>";
        divBody.InnerHtml += "<td></td>";
        //Fix bugs Zikuan 12-Dec-13
        if (factoryCd.ToUpper() == "EGM")
        {
            divBody.InnerHtml += "<td></td>";
            divBody.InnerHtml += "<td></td>";
            divBody.InnerHtml += "<td></td>";
        }
        divBody.InnerHtml += "<td>" + fabQtyTotal.ToString("#,##0.00") + "</td>";
        divBody.InnerHtml += "<td></td>";
        divBody.InnerHtml += "<td></td>";
        divBody.InnerHtml += "<td></td>";
        divBody.InnerHtml += "<td></td>";
        if (Show_Include_Wastage)
        {//"领布数" : "Fab Qty (Inc.Wastage)")
            divBody.InnerHtml += "<td>" + fabQty2Total.ToString("#,##0.00") + "</td>";
        }
        divBody.InnerHtml += "</tr>";
        divBody.InnerHtml += "</table></td></tr>";

        switch (FlagList.SelectedValue)
        {//选择显示语言;
            case "English":
                str = new string[] { "JO Allocation", "Job Order No", "Buyer PO DEL Date", "Color Cd", "Ply", "Fab Qty", "Fab Qty(Inc.Wastage)" };
                break;
            case "Chinese":
                str = new string[] { "JO 分配", "Job Order No", "Buyer PO DEL Date", "颜色代码", "层数", "用布数", "领布数" };
                break;
            case "Vietnamese":
                str = new string[] { "SL cho từng JO", "Số JO:", "Buyer PO DEL Date", "Mã màu", "Số lớp", "Số lượng vải", "Số lượng vải( bao gồm hao phí)" };
                break;
        }
        seq = 0;

        divBody.InnerHtml += "<tr><td><STRONG>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "JO 分配" : "JO Allocation")*/ + "</STRONG></td></tr>";
        divBody.InnerHtml += "<tr><td><table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
        divBody.InnerHtml += "<tr>";
        divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*Job Order No*/+ "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "BUYER PO DEL Date" : "BUYER PO DEL Date")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "颜色代码" : "Color Cd")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "层数" : "Ply")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "用布数" : "Fab Qty")*/ + "</td>";
        divBody.InnerHtml += "<td class='tr7style' width='120' nowrap='nowrap'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "领布数" : "Fab Qty(Inc.Wastage)")*/ + "</td>";
        divBody.InnerHtml += "</tr>";
        //DataTable dtAllocation = MESComment.MesOutSourcePriceSql.GetAllocations(txtMoNo.Text.Trim(), row_Header["MARKER_ID"].ToString(), row_Header["BED_NO"].ToString());
        //foreach (DataRow aRow in dtAllocation.Rows)
        foreach (DataRow aRow in row_Allocations)
        {
            divBody.InnerHtml += "<tr style='font-weight:bolder'>";
            divBody.InnerHtml += "<td>" + aRow["JO_NO"] + "</td>";
            divBody.InnerHtml += "<td>" + aRow["BUYER_PO_DEL_DATE"] + "</td>";
            divBody.InnerHtml += "<td>" + aRow["COLOR_CD"] + "</td>";
            divBody.InnerHtml += "<td>" + aRow["PLYS"] + "</td>";
            divBody.InnerHtml += "<td>" + aRow["FABQTY"] + "</td>";
            //divBody.InnerHtml += "<td >" + (double.Parse(aRow["FABQTY"].ToString()) * 1.015).ToString("#,##0.00") + "</td>";
            divBody.InnerHtml += "<td>" + aRow["FABQTY2"] + "</td>";
            divBody.InnerHtml += "</tr>";

            iblank += 1;
        }
        divBody.InnerHtml += "</table></td></tr>";

        if (New_Allocation)
        {
            DataTable dtNewAllocation = MESComment.MesOutSourcePriceSql.GetNewAllcations(factoryCd, txtMoNo.Text.Trim(), row_Header["MARKER_ID"].ToString(), row_Header["BED_NO"].ToString());
            if (dtNewAllocation.Rows.Count > 0)
            {
                int Cols = dtNewAllocation.Columns.Count;
                int[] Total = new int[Cols];
                //Total[Total.Length - 1] = Convert.ToInt32(Total_PLYS);
                //this.gvNewAllocation.DataSource = dtNewAllocation;
                //this.gvNewAllocation.DataBind();
                //this.divNewAllocation.Visible = true;
                this.divBody.InnerHtml += "<tr><td><STRONG>JO New Allocation</STRONG></td></tr>";
                this.divBody.InnerHtml += "<tr><td><table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'><tr>";
                for (int i = 0; i < dtNewAllocation.Columns.Count; i++)
                {
                    this.divBody.InnerHtml += "<td class='tr7style'>" + dtNewAllocation.Columns[i].ColumnName.ToString() + "</td>";
                }
                this.divBody.InnerHtml += "</tr><tr>";
                foreach (DataRow NewRow in dtNewAllocation.Rows)
                {
                    for (int i = 0; i < dtNewAllocation.Columns.Count; i++)
                    {
                        if (i == dtNewAllocation.Columns.Count - 1)
                        {
                            this.divBody.InnerHtml += "<td class='tr7style'>" + NewRow[i] + "</td>";
                        }
                        else
                        {
                            this.divBody.InnerHtml += "<td>" + NewRow[i] + "</td>";
                        }
                        if (i > 0)
                        {
                            try
                            {
                                Total[i] += Int32.Parse(NewRow[i].ToString());
                            }
                            catch (Exception)
                            {
                                Total[i] += 0;
                            }
                        }
                    }
                    this.divBody.InnerHtml += "</tr>";
                }

                for (int i = 0; i < Total.Length; i++)
                {
                    if (i == 0)
                    {
                        this.divBody.InnerHtml += "<tr class='tr7style'><td>Total:</td><td></td>";//新增了颜色COL；
                    }
                    else if (i > 1)
                    {
                        this.divBody.InnerHtml += "<td>" + Total[i].ToString() + "</td>";
                    }
                }
                this.divBody.InnerHtml += "</tr>";
                divBody.InnerHtml += "</table></td></tr>";
            }
        }

        int AllocationCols = str.Length;
        switch (FlagList.SelectedValue)
        {//选择显示语言;
            case "English":
                str = new string[] { "JO Transfer", "Transfer JO From", "Size Cd", "Color Cd", "(-)Trans Qty", "Transfer JO To", "TO Size Cd", "TO Color Cd", "Cut Lot No To", "(+)Trans Qty", "Fabric Barcode", "Actual Plys", "Color (Màu sắc)", "Batch no. (Số batch)", "Roll no. (Số cây vải)", "Sticker yards (Số yard trên tem)", "Actual Yards: (Số yard thực sự khi đo)", "No. of layers (Tổng số lớp)", "Shade no (Phần trăm màu).", "Balance yards (Số yard còn lại)", "Damage yards (Số yard bị hư)", "MU Yards", "Splice (Tổng số chỗ nối)", "Remarks (Chú ý)", "Prepare By", "Spreading by:", "Date", "Cutting Line", "Fabric Roll Info.", "ROLL NO. / YARDAGE", "COLOUR", "LOT NO.", "QTY", "PREPARED BY", "COUNTER", "CHECK SIZE", "DATE" };
                break;
            case "Chinese":
                str = new string[] { "抽单情况", "Transfer JO From", "尺码代码", "颜色代码", "(-)抽数", "Transfer JO To", "TO尺码代码", "TO颜色代码", "裁床号", "(+)抽数", "布料条码", "实际拉布", "Color (Màu sắc)", "Batch no. (Số batch)", "Roll no. (Số cây vải)", "Sticker yards (Số yard trên tem)", "Actual Yards: (Số yard thực sự khi đo)", "No. of layers (Tổng số lớp)", "Shade no (Phần trăm màu).", "Balance yards (Số yard còn lại)", "Damage yards (Số yard bị hư)", "MU Yards", "Splice (Tổng số chỗ nối)", "Remarks (Chú ý)", "记录员", "Spreading by:", "时间", "组别", "布料数据", "卷号/码数", "颜色", "批号", "数量", "PREPARED BY", "COUNTER", "CHECK SIZE", "DATE" };
                break;
            case "Vietnamese":
                str = new string[] { "Chuyển JO", "Chuyển từ JO", "Size", "Mã màu", "(-) Số lượng chuyển", "Transfer JO To", "TO Size Cd", "TO Color Cd", "Chuyển đến bàn", "(+) Số lượng nhận", "Mã Barcode vải", "Số lớp thực tế", "Color (Màu sắc)", "Batch no. (Số batch)", "Roll no. (Số cây vải)", "Sticker yards (Số yard trên tem)", "Actual Yards: (Số yard thực sự khi đo)", "No. of layers (Tổng số lớp)", "Shade no (Phần trăm màu).", "Balance yards (Số yard còn lại)", "Damage yards (Số yard bị hư)", "MU Yards", "Splice (Tổng số chỗ nối)", "Remarks (Chú ý)", "Được ghi bởi", "Người trải vải:", "Ngày & Tháng", "Nhóm cắt", "Thông tin cây vải", "Số cây/ Số yard", "Màu", "Số Lô", "Số lượng", "PREPARED BY", "COUNTER", "CHECK SIZE", "Date" };
                break;
        }
        seq = 0;
        //DataTable dtNTrans = MESComment.MesOutSourcePriceSql.GetNtransfers(txtMoNo.Text.Trim(), row_Header["MARKER_ID"].ToString(), row_Header["BED_NO"].ToString());
        //DataTable dtPTrans = MESComment.MesOutSourcePriceSql.GetPtransfers(txtMoNo.Text.Trim(), row_Header["MARKER_ID"].ToString(), row_Header["BED_NO"].ToString());
        if (1 == 1)//factoryCd.ToUpper() != "EGM")
        {
            //if (dtNTrans.Rows.Count > 0)
            if (row_Ntransfers.Length > 0)
            {
                divBody.InnerHtml += "<tr><td colspan='" + AllocationCols + "'><STRONG>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "抽单情况" : "JO Transfer")*/ + "</STRONG></td></tr>";
                divBody.InnerHtml += "<tr><td colspan='" + AllocationCols + "'><table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
                divBody.InnerHtml += "<tr>";
                divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*Transfer JO From*/ + "</td>";
                divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "尺码代码" : "Size Cd")*/ + "</td>";
                divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "颜色代码" : "Color Cd")*/ + "</td>";
                divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "(-)抽数" : "(-)Trans Qty")*/ + "</td>";
                divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Transfer JO to" : "Transfer JO To")*/ + "</td>";
                divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "TO尺码代码" : "To Size Cd")*/ + "</td>";
                divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "TO颜色代码" : "To Color Cd")*/ + "</td>";
                divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "裁床号" : "Cut Lot No To")*/ + "</td>";
                divBody.InnerHtml += "<td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "(+)抽数" : "Size Cd")*/ + "</td>";
                divBody.InnerHtml += "</tr>";
                iblank = iblank + 2;
                foreach (DataRow nRow in row_Ntransfers)
                {
                    divBody.InnerHtml += "<tr>";
                    divBody.InnerHtml += "<td>" + nRow["JO_NO"] + "</td>";
                    divBody.InnerHtml += "<td >" + nRow["SIZE_CD"] + "</td>";
                    divBody.InnerHtml += "<td >" + nRow["COLOR_CD"] + "</td>";
                    divBody.InnerHtml += "<td >" + nRow["QTY_FROM"] + "</td>";
                    divBody.InnerHtml += "<td>" + nRow["JO_TO"] + "</td>";
                    divBody.InnerHtml += "<td >" + nRow["TO_SIZE_CD"] + "</td>";
                    divBody.InnerHtml += "<td >" + nRow["TO_COLOR_CD"] + "</td>";
                    divBody.InnerHtml += "<td>" + nRow["CUT_LOT_TO"] + "</td>";
                    divBody.InnerHtml += "<td >+" + nRow["QTY_TO"] + "</td>";
                    divBody.InnerHtml += "</tr>";
                    iblank += 1;
                }
                divBody.InnerHtml += "</table></td></tr>";
            }
            else
            {//要考虑if中字段占的位数;
                //divBody.InnerHtml += "</table></td></tr>";
                    seq++;
                    seq++;
                    seq++;
                    seq++;
                    seq++;
                    seq++;
                    seq++;
                    seq++;
                    seq++;
                    seq++;
            }
        }
        AllocationCols = str.Length;
        divBody.InnerHtml += "<tr><td>&nbsp;</td></tr>";//留空;
        //if (factoryCd.ToUpper() != "TIL")
        //{

            divBody.InnerHtml += "<tr>";
            divBody.InnerHtml += "	<td>";
            divBody.InnerHtml += "		<table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
            int colspan = 0;
            if (!cbFabric.Checked)
            {
                //Modified by Zikuan MES-102
                if (!cbNewFormat.Checked)
                {
                    divBody.InnerHtml += "			<tr>";
                    divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "布料条码" : "Fabric Barcode")*/ + "</td>";
                    divBody.InnerHtml += "			     <td></td>";
                    divBody.InnerHtml += "			     <td></td>";
                    divBody.InnerHtml += "			     <td></td>";
                    divBody.InnerHtml += "			     <td></td>";
                    divBody.InnerHtml += "			     <td></td>";
                    divBody.InnerHtml += "		    </tr>";
                    divBody.InnerHtml += "		    <tr>";
                    divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "实际拉布" : "Actual Plys")*/ + "</td>";
                    divBody.InnerHtml += "			     <td></td>";
                    divBody.InnerHtml += "			     <td></td>";
                    divBody.InnerHtml += "			     <td></td>";
                    divBody.InnerHtml += "			     <td></td>";
                    divBody.InnerHtml += "			     <td></td>";
                    divBody.InnerHtml += "		    </tr>";
                    divBody.InnerHtml += "		</table>";
                    divBody.InnerHtml += "	</td>";
                    divBody.InnerHtml += "</tr>";
                    divBody.InnerHtml += "<tr>";
                    divBody.InnerHtml += "	<td>";
                    divBody.InnerHtml += "		<table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
                    divBody.InnerHtml += "			<tr>";

                    seq--;
                    seq--;
                }
                //要考虑else中字段占的位数;
                seq += 14;
                //End Modified Zikuan MES-102
            }
            else
            {
                colspan = 2;
                //要考虑if中字段占的位数;
                seq += 2;
                divBody.InnerHtml += "			<tr>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Color(Màu sắc)" : "Color(Màu sắc)")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Batch no.(Số batch)" : "Batch no.(Số batch)")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Roll no.(Số cây vải)" : "Roll no.(Số cây vải)")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Sticker yards(Số yard trên tem)" : "Sticker yards(Số yard trên tem)"*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Actual Yards:(Số yard thực sự khi đo)" : "Actual Yards:(Số yard thực sự khi đo)")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "No. of layers(Tổng số lớp)" : "No. of layers(Tổng số lớp)")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Shade no(Phần trăm màu)." : "Shade no(Phần trăm màu).")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Balance yards(Số yard còn lại)" : "Balance yards(Số yard còn lại)")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Damage yards(Số yard bị hư)" : "Damage yards(Số yard bị hư)")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "MU Yards" : "MU Yards")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Splice(Tổng số chỗ nối)" : "Splice(Tổng số chỗ nối)")*/ + "</td>";
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Remarks (Chú ý)" : "Remarks (Chú ý)")*/ + "</td>";
                divBody.InnerHtml += "		    </tr>";
                for (int rows = 0; rows < 10; rows++)
                {//添加3行空白行;
                    divBody.InnerHtml += "<tr><td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "<td height='20'>&nbsp;</td>";
                    divBody.InnerHtml += "</tr>";
                }
            }
            divBody.InnerHtml += " <tr>";
            divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "记录员" : "Prepared By")*/ + "</td>";
            divBody.InnerHtml += "			     <td  colspan='" + colspan + "'></td>";
            if (cbFabric.Checked)
            {
                divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "Spreading by:" : "Spreading by:")*/ + "</td>";
                divBody.InnerHtml += "			     <td  colspan='" + colspan + "'></td>";
            }
            else
            {
                seq++;
            }
            divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "时间" : "Date")*/ + "</td>";
            divBody.InnerHtml += "			     <td  colspan='" + colspan + "'></td>";
            divBody.InnerHtml += "			     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "组别" : "Cutting Line")*/ + "</td>";
            divBody.InnerHtml += "			     <td colspan='" + colspan + "'></td>";
            divBody.InnerHtml += "		    </tr>";
            divBody.InnerHtml += "		</table>";
            divBody.InnerHtml += "	</td></tr></table></td>";
            //Added by Zikuan MES-102
            if (cbNewFormat.Checked)
            {
                divBody.InnerHtml += "<td style='width:1%'></td>";
                divBody.InnerHtml += "<td valign='top'>";
                divBody.InnerHtml += "<table width='100%'  border='0' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
                //divBody.InnerHtml += "<tr><td>Barcode<br><br><br></td></tr>";
                //Barcode generation by Jin Song MES102
                string MarkerID = row_Header["MARKER_ID"] + ";" + row_Header["BED_NO"];
                string barcodeMarkerID = row_Header["MARKER_ID"] + "" + row_Header["BED_NO"]; //Added by MunFoong on 2014.07.24, MES-136

                divBody.InnerHtml += "<td><img id='qrrcodeImage' height='80' width='80' src='";
                divBody.InnerHtml += ResolveUrl("~/QrcodeHandler.ashx?" + MarkerID) + "'/></td><td width='1%'></td>";

                divBody.InnerHtml += "<td align='center'><img id='barcodeImage' src='";
                divBody.InnerHtml += ResolveUrl("~/BarcodeHandler.ashx?" + barcodeMarkerID) + "'/>" + "</td>"; //Modified by MunFoong on 2014.07.24, MES-136
                //End Barcode Generation by Jin Song
                //Space between barcodes and table.
                divBody.InnerHtml += "<tr height='5'></tr>";
                divBody.InnerHtml += "<tr>";
                divBody.InnerHtml += "<table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
                divBody.InnerHtml += "<tr><td width=40% align='center'>Color</td><td width=30% align='center'>Shade Lot</td><td align='center'>Spd Plys</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "<tr><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td><td align='center'>&nbsp;</td></tr>";
                divBody.InnerHtml += "</table>";
                divBody.InnerHtml += "</td></tr>";
                divBody.InnerHtml += "</table>";
                divBody.InnerHtml += "</td>";
                //End Added MES-102
            }
            divBody.InnerHtml += "</tr>";
            //divBody.InnerHtml += "</table>";
            iblank = iblank + 3;
        //}
        //else
        //{//要考虑if中字段占的位数;
        //    seq++;
        //    seq++;
        //    seq++;
        //    seq++;
        //    seq++;
        //    seq += 12;
        //    seq++;
        //}
        if (cbPage.Checked == true)
        {
            divBody.InnerHtml += "<tr>";
            divBody.InnerHtml += "	<td height='20' style='font-size:13px;'><STRONG>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "布料数据" : "Fabric Roll Info.")*/ + "</STRONG></td>";
            divBody.InnerHtml += "</tr>";
            divBody.InnerHtml += "<tr><td>";
            divBody.InnerHtml += "	<table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;'>";
            divBody.InnerHtml += "    <tr>";
            divBody.InnerHtml += "     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "卷号/码数" : "ROLL NO. / YARDAGE")*/ + "</td>";
            divBody.InnerHtml += "     <td class='tr7style' width='200'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "颜色" : "COLOUR")*/ + "</td>";
            divBody.InnerHtml += "     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "批号" : "LOT NO.")*/ + "</td>";
            divBody.InnerHtml += "     <td class='tr7style' width='100'>" + str[seq++].ToString()/*(cbFlag.Checked == true ? "数量" : "QTY")*/ + "</td>";
            divBody.InnerHtml += "     <td class='tr7style' width='300' >&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>";
            divBody.InnerHtml += "    </tr>";
            if (factoryCd.ToUpper() != "TIL")
            {
                iblank = 35 - iblank;
            }
            else
            {
                iblank = 32 - iblank;
            }
            if (Cuttabletotal != 1)
            {
                iblank = iblank + 2;
            }
            iblank += 1;
            for (int i = 0; i < iblank; i++)
            {
                divBody.InnerHtml += "    <tr>";
                divBody.InnerHtml += "     <td height='20'>&nbsp;</td>";
                divBody.InnerHtml += "     <td height='20'>&nbsp;</td>";
                divBody.InnerHtml += "     <td height='20'>&nbsp;</td>";
                divBody.InnerHtml += "     <td height='20'>&nbsp;</td>";
                if (i == 0)
                {
                    divBody.InnerHtml += "     <td rowspan=" + iblank + ">&nbsp;</td>";
                }
                divBody.InnerHtml += "     </tr>";
            }
            int ibank2 = 4;

            for (int i = 0; i < ibank2; i++)
            {
                divBody.InnerHtml += "    <tr>";
                divBody.InnerHtml += "     <td height='20' >&nbsp;</td>";
                divBody.InnerHtml += "     <td height='20' >&nbsp;</td>";
                divBody.InnerHtml += "     <td height='20' >&nbsp;</td>";
                divBody.InnerHtml += "     <td  height='20'>&nbsp;</td>";
                if (i == 0)
                {
                    divBody.InnerHtml += "     <td  rowspan=4><TABLE  width='100%'  border='0' cellspacing='0' cellpadding='0' style='font-size:13px;'>";
                    divBody.InnerHtml += "<tr><td  height='20'>" + str[seq + 0].ToString()/*PREPARED BY:*/ + "</td></tr>";
                    divBody.InnerHtml += "<tr><td  height='20'>" + str[seq + 1].ToString()/*COUNTER:*/ + "</td></tr>";
                    divBody.InnerHtml += "<tr><td  height='20'>" + str[seq + 2].ToString()/*CHECK SIZE:*/+ "</td></tr>";
                    divBody.InnerHtml += "<tr><td  height='20'>" + str[seq + 3].ToString()/*DATE:*/+ "</td></tr></table></td>";
                }
                divBody.InnerHtml += "     </tr>";
            }
            //因为for循环不能用seq++;故此这里需要进行字段占位的控制;
            seq++;
            seq++;
            seq++;
            seq++;

            divBody.InnerHtml += "    </table>";
            divBody.InnerHtml += "</td></tr>";
        }
        else
        {//要考虑if中字段占的位数;
            seq++;
            seq++;
            seq++;
            seq++;
            seq++;
            seq++;
            seq++;
            seq++;
            seq++;
        }
        divBody.InnerHtml += "<tr style='page-break-after: always'><td>&nbsp;</td></tr>";
    }

    private void GeneratePageBottom()
    {
        string[] str = null;

        switch (FlagList.SelectedValue)
        {//选择显示语言;
            case "English":
                str = new string[] { "JO Transfer(In)", "Transfer JO", "Size Cd", "Color Cd", "Trans Qty", "Cut Lot No." };
                break;
            case "Chinese":
                str = new string[] { "抽单情况(执尾)", "Transfer JO", "尺码代码", "颜色代码", "抽数", "裁床号" };
                break;
            case "Vietnamese":
                str = new string[] { "Chuyển JO ( vào)", "Chuyển đến JO", "Mã kích cỡ", "Mã màu", "Số lượng chuyển", "Số bàn" };
                break;
        }
        int bottomInt = 0;
        if (factoryCd.ToUpper() == "EGM")
        {
            //divBody.InnerHtml += "<tr>";
            //divBody.InnerHtml += "	<td><STRONG>JO Pull In/Out Summary</STRONG></td>";
            //divBody.InnerHtml += "</tr>";
            //divBody.InnerHtml += "<tr><td>";
            //divBody.InnerHtml += "	<table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
            //divBody.InnerHtml += "<tr><td class='tr7style' width='100'>Date</td><td>"+DateTime.Now.ToShortDateString()+"</td></tr>";
            //divBody.InnerHtml += "<tr><td class='tr7style' width='100'>Time</td><td>" + DateTime.Now.ToShortTimeString() + "</td></tr>";
            //divBody.InnerHtml += "<tr><td class='tr7style' width='100'>Related GO/MO</td><td>" + txtMoNo.Text + "</td></tr></table></td></tr>";
            //divBody.InnerHtml += "<tr><td>";
            //divBody.InnerHtml += "	<table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
            //divBody.InnerHtml += "    <tr>";
            //divBody.InnerHtml += "     <td class='tr7style'>Marker Set No</td>";
            //divBody.InnerHtml += "     <td class='tr7style'>Marker ID</td>";
            //divBody.InnerHtml += "     <td class='tr7style'>Size Desc</td>";
            //divBody.InnerHtml += "     <td class='tr7style'>Cut Lot No</td>";
            //divBody.InnerHtml += "     <td class='tr7style'>Color Code</td>";
            //divBody.InnerHtml += "     <td class='tr7style'>Color Desc</td>";
            //divBody.InnerHtml += "     <td class='tr7style'>Fabric Pattern</td>";
            //divBody.InnerHtml += "     <td class='tr7style'>Job Order</td>";
        }
        else
        {
            divBottom.InnerHtml = "<table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
            DataTable dtPtrans = MESComment.MesOutSourcePriceSql.GetPtransfers(txtMoNo.Text.Trim(), txtMarkerId.Text, "");
            if (dtPtrans.Rows.Count > 0)
            {
                divBottom.InnerHtml += "<tr >";
                divBottom.InnerHtml += "	<td><STRONG>" + str[bottomInt++].ToString()/*(cbFlag.Checked == true ? "抽单情况(执尾)" : "JO Transfer(In)")*/ + "</STRONG></td>";
                divBottom.InnerHtml += "</tr>";
                divBottom.InnerHtml += "<tr><td>";
                divBottom.InnerHtml += "	<table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:13px;border-collapse:collapse'>";
                divBottom.InnerHtml += "    <tr>";
                divBottom.InnerHtml += "     <td class='tr7style' width='100'>" + str[bottomInt++].ToString()/*Transfer JO*/ + "</td>";
                divBottom.InnerHtml += "     <td class='tr7style' width='100'>" + str[bottomInt++].ToString()/*(cbFlag.Checked == true ? "尺码代码" : "Size Cd")*/ + "</td>";
                divBottom.InnerHtml += "     <td class='tr7style' width='100'>" + str[bottomInt++].ToString()/*(cbFlag.Checked == true ? "颜色代码" : "Color Cd")*/ + "</td>";
                divBottom.InnerHtml += "     <td class='tr7style' width='100'>" + str[bottomInt++].ToString()/*(cbFlag.Checked == true ? "抽数" : "Trans Qty")*/ + "</td>";
                divBottom.InnerHtml += "     <td class='tr7style' width='100'>" + str[bottomInt++].ToString()/*(cbFlag.Checked == true ? "裁床号" : "Cut Lot No.")*/ + "</td>";
                divBottom.InnerHtml += "    </tr>";
            }
            foreach (DataRow row in dtPtrans.Rows)
            {
                divBottom.InnerHtml += "    <tr>";
                divBottom.InnerHtml += "     <td>" + row["JO_NO"] + "</td>";
                divBottom.InnerHtml += "     <td >" + row["SIZE_CD"] + "</td>";
                divBottom.InnerHtml += "     <td >" + row["COLOR_CD"] + "</td>";
                divBottom.InnerHtml += "     <td >+" + row["QTY_TO"] + "</td>";
                divBottom.InnerHtml += "     <td >" + row["CUT_TABLE_NO"] + "</td>";
                divBottom.InnerHtml += "    </tr>";
            }
            if (dtPtrans.Rows.Count > 0)
            {
                divBottom.InnerHtml += "  </table>";
                //divBottom.InnerHtml += "</td></tr>";
            }

        }
    }
}
