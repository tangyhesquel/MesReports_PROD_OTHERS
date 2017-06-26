using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Data;
using MESComment;


public partial class GarmentTransferNote : pPage
{
    private DataTable table;
    private DataTable dataDetail;
    private int subTotalQty = 0;
    protected int totalQty = 0;
    private int totalRows = 0;
    protected short totalPages = 0;
    private readonly short CONST_ROWS_PER_PAGE = 19;
    private string docNO = "";
    protected string PrintStatus1;

    protected string PageHeader;
    private string headerText = "";
    protected DataTable tb;

    public string FactoryCD = "";
    public string User_id = "";
    string Process_CD = "";
    string Garment_Type = "";
    bool IS_YMG = false;
    public string YMG_Format = "";
    private bool isShowBarcode;
    static bool firstloop = true;

    bool RunNo = false;
    private int[] CurrentTotalSum = { 0, 0 };

    protected string joHeaderString
    {
        get { return Resources.GlobalResources.STRING_JO_NO; }
    }

    protected string actualCutHeaderString
    {
        get { return Resources.GlobalResources.STRING_ACTUAL_QTY; }
    }

    protected string totalHeaderString
    {
        get { return Resources.GlobalResources.STRING_JO_TOTAL_QTY; }
    }

    protected string uptodateHeaderString
    {
        get { return Resources.GlobalResources.STRING_UP_TO_DATE_TOTAL; }
    }

    protected string balanceQtyHeaderString
    {
        get { return Resources.GlobalResources.STRING_BAL_QTY; }
    }

    protected string colorSizeHeaderString
    {
        get { return Resources.GlobalResources.STRING_COLOR_CD + "\\" + Resources.GlobalResources.STRING_SIZE_CD; }
    }

    protected string layHeaderString
    {
        get { return Resources.GlobalResources.STRING_CUT_LOT_NO; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Page.Request.QueryString.Get("site") == null)
        {
            return;
        }
        else
        {
            if (this.Page.Request.QueryString.Get("User_id") != null)
            {
                User_id = this.Page.Request.QueryString.Get("User_id").ToString();
            }
            FactoryCD = this.Page.Request.QueryString.Get("site").ToString();
            if (FactoryCD.Contains("YMG"))
            {
                FactoryCD = "YMG";

            }

            if (this.Page.Request.QueryString.Get("docno") == null)
            {
                return;
            }
            else
            {
                docNO = Request.QueryString.Get("docNO").ToString();
                table = MESComment.GarmentTransferSql.GetGarmentCuttingTransferHeaderInfo(docNO);
                if (this.table.Rows.Count > 0)
                {
                    Process_CD = this.table.Rows[0]["Process_CD"].ToString();
                    Garment_Type = this.table.Rows[0]["PROCESS_Garment_Type"].ToString();
                }
            }

            if (FactoryCD.Equals("YMG") && Process_CD.Contains("CUT"))
            {
                IS_YMG = true;


            }
            else
            {
                IS_YMG = false;

            }
            if (this.Page.Request.QueryString.Get("barcode") != null)
            {
                Boolean.TryParse(this.Page.Request.QueryString.Get("barcode").ToString(), out isShowBarcode);
            }

        }
        if (!IsPostBack)
        {
            firstloop = true;
            if (IS_YMG)
            {
                this.body.Visible = false;
                this.divYMG.Visible = true;
                ShowYMG();
            }
            else
            {
                this.divYMG.Visible = false;
                this.body.Visible = true;
                YMG_Format = "";
                ShowAllButYMG();
            }
        }
    }

    private void ShowAllButYMG()
    {
        if (Request.QueryString.Get("site") == null)
            return;

        dataDetail = MESComment.GarmentTransferSql.GetGarmentTransferNoteDetail(docNO);

        if (dataDetail.Rows.Count < 1)
        {
            Response.Write("NO DATA FOUND!");
            Response.End();
        }

        ShowHeader(table);

        PrintStatus1 = "";
        if (Request.QueryString.Get("site") != "")
        {
            DataTable GTFPrint = MESComment.GarmentTransferSql.GetGTFNoteDetailPrintInfo(Request.QueryString.Get("site"), ConvertEmptyString(table.Rows[0]["PROCESS_CD"].ToString()));
            if (GTFPrint.Rows.Count < 1)
            {
                PrintStatus1 = "Noprint";
            }
        }

        //if (Process_CD == "CUT" || Process_CD == "PRI" || Process_CD == "CUT2")
        //{
        //    DataTable joList = dataDetail.DefaultView.ToTable(true, new string[] { "JOB_ORDER_NO", "SC_NO", "CUSTOMER_NAME", "TOTAL_CUT_QTY", "TOTAL_OUTPUT_QTY" });

        //    //SummaryData(joList);
        //    RepeaterCut.DataSource = joList;
        //    RepeaterCut.DataBind();
        //}

        //else
        //{
            DataTable joList = dataDetail.DefaultView.ToTable(true, new string[] { "JOB_ORDER_NO", "CUSTOMER_NAME", "TOTAL_CUT_QTY", "TOTAL_OUTPUT_QTY" });
            RepeaterJo.DataSource = joList;
            RepeaterJo.DataBind();
        //}
    }

    private void ShowYMG()
    {
        string[] cols;
        if (table.Rows.Count <= 0)
        {
            return;
        }

        ShowHeader(table);

        DataTable CPLISTUser = MESComment.MesRpt.GetCustomReportList(FactoryCD, "REPORT", "REPORT", "GarmentTransferNote", User_id);
        DataTable CPLISTDefault = MESComment.MesRpt.GetCustomReportList(FactoryCD, "REPORT", "REPORT", "GarmentTransferNote", "DEFAULT");

        string[] SelectColumnsInEachPart;
        //获取每个部分的选取字段；
        if (CPLISTUser.Rows.Count > 0)
        {
            SelectColumnsInEachPart = CPLISTUser.Rows[0]["CUSTOM_VALUES"].ToString().Trim().Split('|');

        }
        if (CPLISTDefault.Rows.Count > 0)
        {
            SelectColumnsInEachPart = CPLISTDefault.Rows[0]["CUSTOM_VALUES"].ToString().Trim().Split('|');

        }
        else
        {
            return;
        }

        cols = SelectColumnsInEachPart[1].ToString().Trim().Split(',');
        string[] SelectColumnsInHeader = new string[cols.Length - 2];
        for (int i = 1; i < cols.Length - 1; i++)
        {
            SelectColumnsInHeader[i - 1] = cols[i].ToString();
        }
        for (int i = 0; i < SelectColumnsInHeader.Length; i++)
        {
            DisplayHeader(false, SelectColumnsInHeader[i].ToString());
        }

        cols = SelectColumnsInEachPart[2].ToString().Trim().Split(',');
        string[] SelectColumnsInDetail = new string[cols.Length - 2];
        for (int i = 1; i < cols.Length - 1; i++)
        {
            SelectColumnsInDetail[i - 1] = cols[i].ToString();
        }

        DataTable DetailTable = MESComment.GarmentTransferSql.GetGarmentTransferNoteDetail(docNO, FactoryCD, SelectColumnsInDetail);
        if (DetailTable.Rows.Count <= 0)
        {
            return;
        }

        cols = SelectColumnsInEachPart[3].ToString().Trim().Split(',');
        string[] SelectColumnsInTotalCount = new string[cols.Length - 2];
        for (int i = 1; i < cols.Length - 1; i++)
        {
            SelectColumnsInTotalCount[i - 1] = cols[i].ToString();
        }

        DataTable Distinct_JonoList = DetailTable.DefaultView.ToTable(true, new string[] { "JOB_ORDER_NO" });

        if (Garment_Type.Equals("W"))
        {
            this.divYMGdetail.Visible = true;
            DataTable YMGdataDetail = MESComment.GarmentTransferSql.GetGarmentTransferNoteDetail(docNO);
            grYMGDetail.DataSource = YMGdataDetail;
            grYMGDetail.DataBind();
        }

        foreach (DataRow dr in Distinct_JonoList.Rows)
        {
            string JONO = dr["JOB_ORDER_NO"].ToString();

            DataRow[] DetailByJono = DetailTable.Select("JOB_ORDER_NO = '" + JONO + "'");

            DataTable TotalCountTable = MESComment.GarmentTransferSql.GetGarmentTransferNoteTotalQTY(docNO, JONO, SelectColumnsInTotalCount);
            if (TotalCountTable.Rows.Count <= 0)
            {
                return;
            }
            //ShowTotalCount(TotalCountTable);

            cols = SelectColumnsInEachPart[4].ToString().Trim().Split(',');
            string[] SelectColumnsInTotalSummary = new string[cols.Length - 1];
            for (int i = 0; i < cols.Length - 1; i++)
            {
                SelectColumnsInTotalSummary[i] = cols[i + 1].ToString();
            }
            DataTable TotalSummary = MESComment.GarmentTransferSql.GetGarmentTransferNoteDetail_BUNDLE(docNO, FactoryCD, JONO, table.Rows[0]["PROCESS_CD"].ToString(), SelectColumnsInTotalSummary);

            if (TotalSummary.Rows.Count <= 0)
            {
                return;
            }

            ShowYMGTotalSummary(TotalSummary);
        }
        this.divYMG.InnerHtml += "</table>";
        this.div1.InnerHtml += "<tr><td align= 'center' colspan='2' bgcolor=#A4A4A4 >" + "ToTal" + "</td>";
        this.div1.InnerHtml += "<td align= 'center'  bgcolor=#A4A4A4>" + CurrentTotalSum[0] + "</td>";
        this.div1.InnerHtml += "<td align= 'center' bgcolor=#A4A4A4>" + CurrentTotalSum[1] + "</td>";

        this.div1.InnerHtml += "</table>";

    }

    private static string ConvertEmptyString(string s)
    {
        if (s == string.Empty)
            return "";
        else
            return s;
    }

    protected void RepeaterJo_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        string jobOrderNO = (e.Item.DataItem as DataRowView)["JOB_ORDER_NO"].ToString();
        Repeater gv = e.Item.FindControl("GridViewDetail") as Repeater;
        DataView dv = dataDetail.DefaultView;
        dv.RowFilter = "JOB_ORDER_NO='" + jobOrderNO + "'";

        gv.DataSource = dv;
        totalQty = dataDetail.Select("JOB_ORDER_NO='" + jobOrderNO + "'").Sum(a => (int)a["QTY"]);
        (e.Item.FindControl("literalTotal") as Literal).Text = totalQty.ToString();
        totalRows = dataDetail.Select("JOB_ORDER_NO='" + jobOrderNO + "'").Count();
        totalPages = (short)decimal.Ceiling((decimal)totalRows / CONST_ROWS_PER_PAGE);
        gv.DataBind();

        var q = from DataRowView rv in dv
                group rv by new { color = rv["COLOR_CD"].ToString(), colordesc = rv["COLOR_DESC"].ToString(), size = rv["SIZE_CD"].ToString(), seq = Convert.ToInt16(rv["SIZE_SEQ"]) } into g
                orderby g.Key.seq
                select new { ID = g.Key, Total = g.Sum(a => Convert.ToDecimal(a["Qty"].ToString())) };
        tb = new DataTable();

        tb.Columns.Add("COLOR_CD", typeof(String));

        DataView t = dv.ToTable(true, new string[] { "SIZE_CD", "SIZE_SEQ" }).DefaultView;
        t.Sort = "SIZE_SEQ";

        for (int i = 0; i < t.Count; i++)
        {
            tb.Columns.Add(t[i]["SIZE_CD"].ToString().Trim() == "" ? "NA" : t[i]["SIZE_CD"].ToString().Trim(), typeof(int));
        }

        tb.Columns.Add("TOTAL", typeof(int));

        foreach (var r in q)
        {
            string color = r.ID.color + "(" + r.ID.colordesc + ")";
            string size = r.ID.size.ToString().Trim() == "" ? "NA" : r.ID.size.ToString().Trim();
            decimal total = r.Total;
            bool isExists = false;
            for (int j = 0; j < tb.Rows.Count; j++)
            {
                if (tb.Rows[j]["COLOR_CD"].ToString() == color)
                {
                    isExists = true;
                    tb.Rows[j][size] = total;
                    tb.Rows[j]["TOTAL"] = Convert.ToDecimal(tb.Rows[j]["TOTAL"]) + total;
                }
            }
            if (!isExists)
            {
                DataRow row = tb.NewRow();
                row["COLOR_CD"] = color;
                row[size] = total;
                row["TOTAL"] = total;
                tb.Rows.Add(row);
            }
        }

        GridView gv1 = e.Item.FindControl("GridViewSummary") as GridView;

        tb = FormatDataTable(tb, 5);
        gv1.DataSource = tb;
        gv1.DataBind();
    }

    protected void RepeaterCut_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (!firstloop)
        { return; }

        Literal lbl3 = e.Item.FindControl("Literaljo") as Literal;
        Literal lbl4 = e.Item.FindControl("Literal2") as Literal;
        Literal lbl5 = e.Item.FindControl("Literal10") as Literal;
        lbl3.Visible = true;
        lbl4.Visible = true;
        lbl5.Visible = true;

        string customer = (e.Item.DataItem as DataRowView)["CUSTOMER_NAME"].ToString();
        string sc_no = (e.Item.DataItem as DataRowView)["SC_NO"].ToString();
        Literal lbl1 = e.Item.FindControl("lblCustomer") as Literal;
        Literal lbl2 = e.Item.FindControl("lblSCNO") as Literal;
        lbl1.Text = sc_no;
        lbl2.Text = customer;

        //string jobOrderNO = (e.Item.DataItem as DataRowView)["JOB_ORDER_NO"].ToString();
        //Repeater gv = e.Item.FindControl("GridViewDetail") as Repeater;
        DataView dv = dataDetail.DefaultView;
        //dv.RowFilter = "JOB_ORDER_NO='" + jobOrderNO + "'";


        //gv.DataSource = dv;
        //totalQty = dataDetail.Select("JOB_ORDER_NO='" + jobOrderNO + "'").Sum(a => (int)a["QTY"]);
        //(e.Item.FindControl("literalTotal") as Literal).Text = totalQty.ToString();
        //totalRows = dataDetail.Select("JOB_ORDER_NO='" + jobOrderNO + "'").Count();
        //totalPages = (short)decimal.Ceiling((decimal)totalRows / CONST_ROWS_PER_PAGE);
        //gv.DataBind();

        //var s = from DataRowView rv in dv
        //        group rv by new {jo = rv["JOB_ORDER_NO"].ToString() ,color = rv["COLOR_CD"].ToString(), colordesc = rv["COLOR_DESC"].ToString(), cutlay = rv["LAY_NO"].ToString() } into g
        //        //orderby g.Key.seq
        //        select new { ID = g.Key};

        //Code to group Cut Lay with comma seperator 
        var result = from DataRowView row in dv
                     group row by new
                     {
                         JOB_ORDER_NO = row["JOB_ORDER_NO"],
                         COLOR_CD = row["COLOR_CD"],
                         COLOR_DESC = row["COLOR_DESC"],
                         TOTAL_CUT_QTY = row["TOTAL_CUT_QTY"],
                         TOTAL_OUTPUT_QTY = row["TOTAL_OUTPUT_QTY"]
                     } into grp
                     orderby grp.Key.JOB_ORDER_NO descending
                     select new
                     {
                         grp.Key.JOB_ORDER_NO,
                         grp.Key.COLOR_CD,
                         grp.Key.COLOR_DESC,
                         grp.Key.TOTAL_CUT_QTY,
                         grp.Key.TOTAL_OUTPUT_QTY,
                         CUT_LAY = String.Join(", ", grp.Select(r => r["CUT_LAY_NO"].ToString()).Distinct().ToArray()),
                     };

        var total_data = from DataRowView row in dv
                         group row by new
                         {
                             JOB_ORDER_NO = row["JOB_ORDER_NO"],
                             TOTAL_CUT_QTY = row["TOTAL_CUT_QTY"],
                             TOTAL_OUTPUT_QTY = row["TOTAL_OUTPUT_QTY"]
                         } into grp
                         orderby grp.Key.JOB_ORDER_NO descending
                         select new
                         {
                             grp.Key.JOB_ORDER_NO,
                             grp.Key.TOTAL_CUT_QTY,
                             grp.Key.TOTAL_OUTPUT_QTY,
                         };


        var detail = from DataRowView row in dv
                     group row by new
                     {
                         JOB_ORDER_NO = row["JOB_ORDER_NO"],
                         COLOR_CD = row["COLOR_CD"],
                         CUT_LAY_NO = row["CUT_LAY_NO"],
                         BUNDLE_NO = row["BUNDLE_NO"],
                         SIZE_CD = row["SIZE_CD"],
                         QTY = row["QTY"]
                     } into grp
                     orderby grp.Key.JOB_ORDER_NO descending, grp.Key.BUNDLE_NO
                     select new
                     {
                         grp.Key.JOB_ORDER_NO,
                         grp.Key.COLOR_CD,
                         grp.Key.CUT_LAY_NO,
                         grp.Key.BUNDLE_NO,
                         grp.Key.SIZE_CD,
                         grp.Key.QTY
                     };

        DataTable tb1 = new DataTable();
        tb1.Columns.Add("JOB_ORDER_NO", typeof(string));
        tb1.Columns.Add("COLOR_CD", typeof(string));
        tb1.Columns.Add("CUT_LAY_NO", typeof(string));

        DataTable tb2 = new DataTable();
        tb2.Columns.Add("JOB_ORDER_NO", typeof(string));
        tb2.Columns.Add("TOTAL_CUT_QTY", typeof(string));
        tb2.Columns.Add("CURRENT_OUTPUT", typeof(string));
        tb2.Columns.Add("TOTAL_OUTPUT_QTY", typeof(string));
        tb2.Columns.Add("BALANCE_QTY", typeof(string));

        DataTable tb3 = new DataTable();
        tb3.Columns.Add("JOB_ORDER_NO", typeof(string));
        tb3.Columns.Add("CUT_LAY_NO", typeof(string));
        tb3.Columns.Add("COLOR_CD", typeof(string));
        tb3.Columns.Add("BUNDLE_NO", typeof(string));
        tb3.Columns.Add("SIZE_CD", typeof(string));
        tb3.Columns.Add("QTY", typeof(string));

        foreach (var row in result)
        {
            tb1.Rows.Add(row.JOB_ORDER_NO, row.COLOR_CD + "(" + row.COLOR_DESC + ")", row.CUT_LAY);
        }

        foreach (var row in total_data)
        {
            tb2.Rows.Add(row.JOB_ORDER_NO, row.TOTAL_CUT_QTY, "", row.TOTAL_OUTPUT_QTY, (Convert.ToInt32(row.TOTAL_CUT_QTY) - Convert.ToInt32(row.TOTAL_OUTPUT_QTY)).ToString());
        }

        foreach (var row in detail)
        {
            tb3.Rows.Add(row.JOB_ORDER_NO, row.CUT_LAY_NO, row.COLOR_CD, row.BUNDLE_NO, row.SIZE_CD, row.QTY);
        }
        //End Code to group Cut Lay with comma seperator 


        tb = new DataTable();

        tb.Columns.Add("JOB_ORDER_NO", typeof(String));
        tb.Columns.Add("COLOR_CD", typeof(String));
        tb.Columns.Add("CUT_LAY_NO", typeof(String));

        var q = from DataRowView rv in dv
                group rv by new
                {
                    jo = rv["JOB_ORDER_NO"].ToString(),
                    color = rv["COLOR_CD"].ToString(),
                    colordesc = rv["COLOR_DESC"].ToString(),
                    size = rv["SIZE_CD"].ToString(),
                    seq = Convert.ToInt16(rv["SIZE_SEQ"])
                } into g
                orderby g.Key.jo descending, g.Key.seq
                select new { ID = g.Key, Total = g.Sum(a => Convert.ToDecimal(a["Qty"].ToString())) };

        DataView t = dv.ToTable(true, new string[] { "SIZE_CD", "SIZE_SEQ" }).DefaultView;
        t.Sort = "SIZE_SEQ";

        for (int i = 0; i < t.Count; i++)
        {
            tb.Columns.Add(t[i]["SIZE_CD"].ToString().Trim() == "" ? "NA" : t[i]["SIZE_CD"].ToString().Trim(), typeof(int));
        }

        tb.Columns.Add("TOTAL", typeof(int));

        foreach (var r in q)
        {
            string jo = r.ID.jo.ToString().Trim();
            string color = r.ID.color + "(" + r.ID.colordesc + ")";
            string size = r.ID.size.ToString().Trim() == "" ? "NA" : r.ID.size.ToString().Trim();
            decimal total = r.Total;
            bool isExists = false;
            for (int j = 0; j < tb.Rows.Count; j++)
            {
                if (tb.Rows[j]["JOB_ORDER_NO"].ToString() == jo && tb.Rows[j]["COLOR_CD"].ToString() == color)
                {
                    isExists = true;
                    tb.Rows[j][size] = total;
                    tb.Rows[j]["TOTAL"] = Convert.ToDecimal(tb.Rows[j]["TOTAL"]) + total;
                }
            }
            if (!isExists)
            {
                DataRow row = tb.NewRow();
                row["JOB_ORDER_NO"] = jo;
                row["COLOR_CD"] = color;
                row["CUT_LAY_NO"] = "";
                row[size] = total;
                row["TOTAL"] = total;
                tb.Rows.Add(row);
            }
        }

        GridView gv1 = e.Item.FindControl("GridViewSummary") as GridView;
        GridView gv2 = e.Item.FindControl("GridViewTotal") as GridView;
        GridView gv3 = e.Item.FindControl("GridViewDetail") as GridView;

        //Code to add Cut Lay into final table
        for (int tbcount = 0; tbcount < tb.Rows.Count; tbcount++)
        {
            for (int tb1count = 0; tb1count < tb1.Rows.Count; tb1count++)
            {
                if (tb.Rows[tbcount]["JOB_ORDER_NO"].ToString() == tb1.Rows[tb1count]["JOB_ORDER_NO"].ToString() && tb.Rows[tbcount]["COLOR_CD"].ToString() == tb1.Rows[tb1count]["COLOR_CD"].ToString())
                {
                    tb.Rows[tbcount]["CUT_LAY_NO"] = tb1.Rows[tb1count]["CUT_LAY_NO"].ToString();
                    break;
                }
            }
        }
        //End Code to add Cut Lay into final table

        //Add CURRENT_OUTPUT column from tb into tb2
        for (int tbcount = 0; tbcount < tb.Rows.Count; tbcount++)
        {
            for (int tb2count = 0; tb2count < tb2.Rows.Count; tb2count++)
            {
                if (tb.Rows[tbcount]["JOB_ORDER_NO"].ToString() == tb2.Rows[tb2count]["JOB_ORDER_NO"].ToString())
                {
                    if (tbcount != tb2count)
                    {
                        tb2.Rows[tb2count]["CURRENT_OUTPUT"] = Convert.ToDecimal(tb2.Rows[tb2count]["CURRENT_OUTPUT"]) + Convert.ToDecimal(tb.Rows[tbcount]["TOTAL"]);
                    }
                    else
                    {
                        tb2.Rows[tb2count]["CURRENT_OUTPUT"] = tb.Rows[tbcount]["TOTAL"].ToString();
                    }

                    break;
                }
            }
        }
        //Add CURRENT_OUTPUT column from tb into tb2


        //Format TotalTable
        DataTable tb2Format = new DataTable();
        string joHeaderText = joHeaderString;
        string actualCutText = actualCutHeaderString;
        string totalText = totalHeaderString;
        string uptodateText = uptodateHeaderString;
        string balanceQtyText = balanceQtyHeaderString;

        tb2Format.Columns.Add("JO", typeof(string));
        tb2Format.Columns.Add("TOTAL_CUT_QTY", typeof(string));
        tb2Format.Columns.Add("CURRENT_OUTPUT", typeof(string));
        tb2Format.Columns.Add("TOTAL_OUTPUT_QTY", typeof(string));
        tb2Format.Columns.Add("BALANCE_QTY", typeof(string));
        tb2Format.Columns.Add("TYPE", typeof(string));

        DataRow rowHeader = null;
        DataRow rowData = null;

        rowHeader = tb2Format.NewRow();
        tb2Format.Rows.Add(rowHeader);

        rowHeader["JO"] = joHeaderText;
        rowHeader["TOTAL_CUT_QTY"] = actualCutText;
        rowHeader["CURRENT_OUTPUT"] = totalText;
        rowHeader["TOTAL_OUTPUT_QTY"] = uptodateText;
        rowHeader["BALANCE_QTY"] = balanceQtyText;
        rowHeader["TYPE"] = "HEADER";

        for (int i = 0; i < tb2.Rows.Count; i++)
        {
            rowData = tb2Format.NewRow();
            tb2Format.Rows.Add(rowData);

            rowData["JO"] = tb2.Rows[i][0].ToString();
            rowData["TOTAL_CUT_QTY"] = tb2.Rows[i][1].ToString();
            rowData["CURRENT_OUTPUT"] = tb2.Rows[i][2].ToString();
            rowData["TOTAL_OUTPUT_QTY"] = tb2.Rows[i][3].ToString();
            rowData["BALANCE_QTY"] = tb2.Rows[i][4].ToString();
            rowData["TYPE"] = "DATA";
        }
        //Format TotalTable

        //Format DetailTable
        DataTable tb3Format = new DataTable();
        int count = 0;

        tb3Format.Columns.Add("CUT_LAY_NO", typeof(string));
        tb3Format.Columns.Add("COLOR_CD", typeof(string));
        tb3Format.Columns.Add("BUNDLE_NO", typeof(string));
        tb3Format.Columns.Add("SIZE_CD", typeof(string));
        tb3Format.Columns.Add("QTY", typeof(string));
        tb3Format.Columns.Add("TYPE", typeof(string));

        DataRow detailHeader = null;
        DataRow detailData = null;

        detailHeader = tb3Format.NewRow();
        tb3Format.Rows.Add(detailHeader);

        detailHeader["CUT_LAY_NO"] = Resources.GlobalResources.STRING_JO_NO;
        detailHeader["COLOR_CD"] = tb1.Rows[count]["JOB_ORDER_NO"].ToString();
        detailHeader["BUNDLE_NO"] = Resources.GlobalResources.STRING_CUSTOMER;
        detailHeader["SIZE_CD"] = (e.Item.DataItem as DataRowView)["CUSTOMER_NAME"].ToString();
        detailHeader["QTY"] = "";
        detailHeader["TYPE"] = "NAME";

        detailHeader = tb3Format.NewRow();
        tb3Format.Rows.Add(detailHeader);

        detailHeader["CUT_LAY_NO"] = Resources.GlobalResources.STRING_CUT_LOT_NO;
        detailHeader["COLOR_CD"] = Resources.GlobalResources.STRING_COLOR_CD;
        detailHeader["BUNDLE_NO"] = Resources.GlobalResources.STRING_BUNDLE_NO;
        detailHeader["SIZE_CD"] = Resources.GlobalResources.STRING_SIZE_CD;
        detailHeader["QTY"] = Resources.GlobalResources.STRING_QTY;
        detailHeader["TYPE"] = "HEADER";

        for (int i = 0; i < tb3.Rows.Count; i++)
        {
            detailData = tb3Format.NewRow();
            tb3Format.Rows.Add(detailData);

            if (tb3.Rows[i]["JOB_ORDER_NO"].ToString() == tb1.Rows[count]["JOB_ORDER_NO"].ToString())
            {
                detailData["CUT_LAY_NO"] = tb3.Rows[i][1].ToString();
                detailData["COLOR_CD"] = tb3.Rows[i][2].ToString();
                detailData["BUNDLE_NO"] = tb3.Rows[i][3].ToString();
                detailData["SIZE_CD"] = tb3.Rows[i][4].ToString();
                detailData["QTY"] = tb3.Rows[i][5].ToString();
                detailData["TYPE"] = "DATA";

                if (i == tb3.Rows.Count - 1)
                {
                    detailData = tb3Format.NewRow();
                    tb3Format.Rows.Add(detailData);
                    detailData["CUT_LAY_NO"] = Resources.GlobalResources.STRING_TOTAL;
                    detailData["COLOR_CD"] = "";
                    detailData["BUNDLE_NO"] = "";
                    detailData["SIZE_CD"] = "";
                    detailData["QTY"] = tb.Rows[count]["TOTAL"].ToString();
                    detailData["TYPE"] = "TOTAL";
                    break;
                }
            }
            else
            {
                detailData["CUT_LAY_NO"] = Resources.GlobalResources.STRING_TOTAL;
                detailData["COLOR_CD"] = "";
                detailData["BUNDLE_NO"] = "";
                detailData["SIZE_CD"] = "";
                detailData["QTY"] = tb.Rows[count]["TOTAL"].ToString();
                detailData["TYPE"] = "TOTAL";
                count++;

                if (i != tb3.Rows.Count)
                {
                    detailHeader = tb3Format.NewRow();
                    tb3Format.Rows.Add(detailHeader);

                    detailHeader["CUT_LAY_NO"] = Resources.GlobalResources.STRING_JO_NO;
                    detailHeader["COLOR_CD"] = tb1.Rows[count]["JOB_ORDER_NO"].ToString();
                    detailHeader["BUNDLE_NO"] = Resources.GlobalResources.STRING_CUSTOMER;
                    detailHeader["SIZE_CD"] = (e.Item.DataItem as DataRowView)["CUSTOMER_NAME"].ToString();
                    detailHeader["QTY"] = "";
                    detailHeader["TYPE"] = "NAME";

                    detailHeader = tb3Format.NewRow();
                    tb3Format.Rows.Add(detailHeader);

                    detailHeader["CUT_LAY_NO"] = Resources.GlobalResources.STRING_CUT_LOT_NO;
                    detailHeader["COLOR_CD"] = Resources.GlobalResources.STRING_COLOR_CD;
                    detailHeader["BUNDLE_NO"] = Resources.GlobalResources.STRING_BUNDLE_NO;
                    detailHeader["SIZE_CD"] = Resources.GlobalResources.STRING_SIZE_CD;
                    detailHeader["QTY"] = Resources.GlobalResources.STRING_QTY;
                    detailHeader["TYPE"] = "HEADER";
                }
                i--;
            }
        }
        //Format DetailTable

        tb = FormatDataTable_cut(tb, 10);
        gv1.DataSource = tb;
        gv1.DataBind();

        for (int i = 0; i < gv1.Rows.Count; i++)
        {
            if (gv1.Rows[i].Cells[0].Text == "HEADER")
            {
                for (int j = 0; j < gv1.Rows[i].Cells.Count; j++)
                {
                    gv1.Rows[i].Cells[j].Font.Bold = true;
                    gv1.Rows[i].Cells[j].BackColor = System.Drawing.Color.LightGreen;
                }
            }
            else if (gv1.Rows[i].Cells[0].Text == "TOTAL")
            {
                for (int j = 0; j < gv1.Rows[i].Cells.Count; j++)
                {
                    gv1.Rows[i].Cells[j].Font.Bold = true;
                    gv1.Rows[i].Cells[j].BackColor = System.Drawing.Color.LightGreen;
                }
            }
            else if (gv1.Rows[i].Cells[0].Text == "NAME")
            {
                for (int j = 0; j < gv1.Rows[i].Cells.Count; j++)
                {
                    gv1.Rows[i].Cells[j].Font.Bold = true;
                }
            }
        }

        for (int i = 0; i < gv1.Rows.Count; i++)
        {
            gv1.Rows[i].Cells[0].Visible = false;
        }

        gv2.DataSource = tb2Format;
        gv2.DataBind();

        for (int i = 0; i < gv2.Rows.Count; i++)
        {
            if (gv2.Rows[i].Cells[5].Text == "HEADER")
            {
                for (int j = 0; j < gv2.Rows[i].Cells.Count; j++)
                {
                    gv2.Rows[i].Cells[j].Font.Bold = true;
                    gv2.Rows[i].Cells[j].BackColor = System.Drawing.Color.LightGreen;
                }
            }
            else if (gv2.Rows[i].Cells[5].Text == "TOTAL")
            {
                for (int j = 0; j < gv2.Rows[i].Cells.Count; j++)
                {
                    gv2.Rows[i].Cells[j].Font.Bold = true;
                    gv2.Rows[i].Cells[j].BackColor = System.Drawing.Color.LightGreen;
                }
            }
            else if (gv2.Rows[i].Cells[5].Text == "NAME")
            {
                for (int j = 0; j < gv2.Rows[i].Cells.Count; j++)
                {
                    gv2.Rows[i].Cells[j].Font.Bold = true;
                }
            }
        }

        for (int i = 0; i < gv2.Rows.Count; i++)
        {
            gv2.Rows[i].Cells[5].Visible = false;
        }

        gv3.DataSource = tb3Format;
        gv3.DataBind();

        for (int i = 0; i < gv3.Rows.Count; i++)
        {
            if (gv3.Rows[i].Cells[5].Text == "HEADER")
            {
                for (int j = 0; j < gv3.Rows[i].Cells.Count; j++)
                {
                    gv3.Rows[i].Cells[j].Font.Bold = true;
                    gv3.Rows[i].Cells[j].BackColor = System.Drawing.Color.LightGreen;
                }
            }
            else if (gv3.Rows[i].Cells[5].Text == "TOTAL")
            {
                for (int j = 0; j < gv3.Rows[i].Cells.Count; j++)
                {
                    gv3.Rows[i].Cells[j].Font.Bold = true;
                    gv3.Rows[i].Cells[j].BackColor = System.Drawing.Color.LightGreen;
                }
            }
            else if (gv3.Rows[i].Cells[5].Text == "NAME")
            {
                for (int j = 0; j < gv3.Rows[i].Cells.Count; j++)
                {
                    gv3.Rows[i].Cells[j].Font.Bold = true;
                }
            }
        }

        for (int i = 0; i < gv3.Rows.Count; i++)
        {
            gv3.Rows[i].Cells[5].Visible = false;
        }

        firstloop = false;
    }

    protected void GridViewSummaryRowBounded(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.DataItem == null) return;
        string col1String = (e.Row.DataItem as DataRowView)[0].ToString();
        if (col1String == colorSizeHeaderString || col1String == Resources.GlobalResources.STRING_TOTAL)
        {
            e.Row.CssClass = "tableHeader";
        }
    }

    protected void GridViewSummaryRowCreated(object sender, GridViewRowEventArgs e)
    {
        //string col1String=(e.Row.DataItem as DataRowView)[0].ToString();
        //if (col1String == colorSizeHeaderString || col1String ==Resources.GlobalResources.STRING_TOTAL)
        //{
        //    e.Row.CssClass = "tableHeader";
        //}
    }

    protected void GridRowDataBound(object sender, RepeaterItemEventArgs e)
    {
        //Repeater repeater = sender as Repeater;
        //System.IO.StringWriter writer;

        //if (e.Item.ItemType == ListItemType.Header)
        //{
        //    writer = new System.IO.StringWriter();
        //    e.Item.RenderControl(new HtmlTextWriter(writer));
        //    headerText = writer.ToString();
        //}

        //if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        //{
        //    subTotalQty += (int)(e.Item.DataItem as DataRowView)["QTY"];
        //}
    }

    private DataTable FormatDataTable(DataTable summaryTable, int SizeCountPerRow)
    {
        string headerText = colorSizeHeaderString;
        DataTable table = new DataTable();

        int sizeCols = SizeCountPerRow > summaryTable.Columns.Count - 2 ? summaryTable.Columns.Count - 2 : SizeCountPerRow;

        table.Columns.Add("COLOR", typeof(string));

        for (int i = 0; i < sizeCols; i++)
        {
            table.Columns.Add("SIZE" + i.ToString(), typeof(string));
        }

        for (int i = 0; i < summaryTable.Rows.Count; i++)
        {
            DataRow rowHeader = null;
            DataRow rowData = null;

            for (int j = 1; j < summaryTable.Columns.Count - 1; j++)
            {
                if (j % SizeCountPerRow == 1)
                {
                    rowHeader = table.NewRow();
                    rowData = table.NewRow();

                    table.Rows.Add(rowHeader);
                    table.Rows.Add(rowData);

                    rowHeader["COLOR"] = headerText;
                    rowData["COLOR"] = summaryTable.Rows[i][0].ToString();
                }

                rowHeader["SIZE" + ((j - 1) % SizeCountPerRow).ToString()] = summaryTable.Columns[j].ColumnName == "NA" ? "" : summaryTable.Columns[j].ColumnName;
                rowData["SIZE" + ((j - 1) % SizeCountPerRow).ToString()] = summaryTable.Rows[i][j].ToString();
            }

            DataRow footer = table.NewRow();
            table.Rows.Add(footer);
            footer[0] = Resources.GlobalResources.STRING_TOTAL;
            footer[1] = summaryTable.Rows[i]["TOTAL"].ToString();
        }

        return table;
    }

    private DataTable FormatDataTable_cut(DataTable summaryTable, int SizeCountPerRow)
    {
        string headerText = colorSizeHeaderString;
        string joHeaderText = joHeaderString;
        string layHeaderText = layHeaderString;
        DataTable table = new DataTable();
        int rowCount;

        int sizeCols = SizeCountPerRow > summaryTable.Columns.Count - 4 ? summaryTable.Columns.Count - 4 : SizeCountPerRow;
        rowCount = (summaryTable.Columns.Count - 4) / SizeCountPerRow;
        if ((summaryTable.Columns.Count - 4) % SizeCountPerRow > 3)
        {
            rowCount = +1;
        }

        table.Columns.Add("TYPE", typeof(string));
        table.Columns.Add("JO", typeof(string));
        table.Columns.Add("COLOR", typeof(string));
        table.Columns.Add("CUT_LAY_NO", typeof(string));

        for (int i = 0; i < sizeCols; i++)
        {
            table.Columns.Add("SIZE" + i.ToString(), typeof(string));
        }

        if (sizeCols < SizeCountPerRow)
        {
            table.Columns.Add("TOTAL", typeof(string));
            table.Columns.Add("TTL_BY_GO_COLOR", typeof(string));
        }

        DataRow rowHeader = null;
        DataRow rowData = null;
        int a = 0;

        for (int i = 0; i <= rowCount; i++)
        {
            for (int j = 3; j < SizeCountPerRow + 3; j++)
            {
                if (j % SizeCountPerRow == 3 && j < SizeCountPerRow)
                {
                    rowHeader = table.NewRow();
                    table.Rows.Add(rowHeader);

                    rowHeader["TYPE"] = "HEADER";
                    rowHeader["JO"] = joHeaderText;
                    rowHeader["COLOR"] = headerText;
                    rowHeader["CUT_LAY_NO"] = layHeaderText;
                }

                if (sizeCols < SizeCountPerRow)
                {
                    rowHeader["SIZE" + ((j - 3) % SizeCountPerRow).ToString()] = summaryTable.Columns[j].ColumnName == "NA" ? "" : summaryTable.Columns[j].ColumnName;
                    if (j == summaryTable.Columns.Count - 2)
                    {
                        rowHeader["TOTAL"] = Resources.GlobalResources.STRING_TOTAL;
                        rowHeader["TTL_BY_GO_COLOR"] = Resources.GlobalResources.STRING_TTL_GO_COLOR;
                        break;
                    }
                }
                else if ((j + a) == summaryTable.Columns.Count - 1)
                {
                    rowHeader["SIZE" + (j - 3)] = Resources.GlobalResources.STRING_TOTAL;
                    rowHeader["SIZE" + (j - 3 + 1)] = Resources.GlobalResources.STRING_TTL_GO_COLOR;
                    break;
                }
                else
                    rowHeader["SIZE" + ((j - 3) % SizeCountPerRow).ToString()] = summaryTable.Columns[j + a].ColumnName == "NA" ? "" : summaryTable.Columns[j + a].ColumnName;
            }

            for (int k = 0; k < summaryTable.Rows.Count; k++)
            {
                rowData = table.NewRow();
                table.Rows.Add(rowData);

                for (int l = 3; l < SizeCountPerRow + 3; l++)
                {
                    if (l % SizeCountPerRow == 3)
                    {
                        rowData["TYPE"] = "DATA";
                        rowData["JO"] = summaryTable.Rows[k][0].ToString();
                        rowData["COLOR"] = summaryTable.Rows[k][1].ToString();
                        rowData["CUT_LAY_NO"] = summaryTable.Rows[k][2].ToString();
                    }

                    if (sizeCols < SizeCountPerRow)
                    {
                        rowData["SIZE" + ((l - 3) % SizeCountPerRow).ToString()] = summaryTable.Rows[k][l + a].ToString();
                        if (l == summaryTable.Columns.Count - 2)
                        {
                            rowData["TOTAL"] = summaryTable.Rows[k]["TOTAL"].ToString();

                            int byColor = 0;
                            for (int b = 0; b < summaryTable.Rows.Count; b++)
                            {
                                byColor = byColor + Convert.ToInt32(summaryTable.Rows[b]["TOTAL"].ToString());
                            }

                            rowData["TTL_BY_GO_COLOR"] = (summaryTable.Rows.Count - 1) == k ? byColor.ToString() : "";
                            if ((summaryTable.Rows.Count - 1) == k)
                            {
                                return table;
                            }
                            break;
                        }
                    }
                    else if ((l + a) == summaryTable.Columns.Count - 1)
                    {
                        rowData["SIZE" + (l - 3)] = summaryTable.Rows[k]["TOTAL"].ToString();

                        int byColor = 0;
                        for (int b = 0; b < summaryTable.Rows.Count; b++)
                        {
                            byColor = byColor + Convert.ToInt32(summaryTable.Rows[b]["TOTAL"].ToString());
                        }

                        rowData["SIZE" + (l - 3 + 1)] = (summaryTable.Rows.Count - 1) == k ? byColor.ToString() : "";
                        break;
                    }
                    else
                        rowData["SIZE" + ((l - 3) % SizeCountPerRow).ToString()] = summaryTable.Rows[k][l + a].ToString();
                }
            }
            a = a + SizeCountPerRow;
        }
        return table;
    }

    private void ShowHeader(DataTable table)
    {
        PageHeader = "(" + base.CurrentSite + ")" + "Garment Transfer Note" + "-[" + ConvertEmptyString(table.Rows[0]["PROCESS_CD"].ToString()) + "->";
        //Added By ZouShIChang ON 2013.09.23 Start MES024
        //PageHeader = PageHeader + ConvertEmptyString(table.Rows[0]["NEXT_PROCESS_CD"].ToString()) + "]";
        if (table.Rows[0]["PROCESS_TYPE"].ToString() == table.Rows[0]["NEXT_PROCESS_TYPE"].ToString())
        {
            PageHeader = PageHeader + ConvertEmptyString(table.Rows[0]["NEXT_PROCESS_CD"].ToString()) + "]";
        }
        else
        {
            PageHeader = PageHeader + ConvertEmptyString(table.Rows[0]["NEXT_PROCESS_CD"].ToString()) + "|" + ConvertEmptyString(table.Rows[0]["NEXT_PROCESS_PRODUCTION_FACTORY"].ToString()) + "]";
        }
        //Added By ZouShIChang ON 2013.09.23 Start MES024
        if (table.Rows[0]["STATUS"].ToString() == "N")
        {
            PageHeader = PageHeader + " S";
        }
        else
        {
            PageHeader = PageHeader + " R";
        }

        LiteralFromDept.Text = ConvertEmptyString(table.Rows[0]["PROCESS_CD"].ToString());
        LiteralFromLine.Text = ConvertEmptyString(table.Rows[0]["PRODUCTION_LINE_CD"].ToString());

        lblUserNameText.Text = ConvertEmptyString(table.Rows[0]["USER_NAME"].ToString());

        if (!string.IsNullOrEmpty(table.Rows[0]["PRD_LINE_NAME"].ToString()))
            LiteralFromLine.Text = LiteralFromLine.Text + "(" + table.Rows[0]["PRD_LINE_NAME"].ToString() + ")";

        LiteralToLine.Text = ConvertEmptyString(table.Rows[0]["NEXT_PRODUCTION_LINE_CD"].ToString());
        if (!string.IsNullOrEmpty(table.Rows[0]["NEXT_PRD_LINE_NAME"].ToString()))
            LiteralToLine.Text = LiteralToLine.Text + "(" + table.Rows[0]["NEXT_PRD_LINE_NAME"].ToString() + ")";

        LiteraltoRemark.Text = ConvertEmptyString(table.Rows[0]["REMARK"].ToString());

        lblDocNOText.Text = docNO;
        LiteralDateTime.Text = table.Rows[0]["TRX_DATE"].ToString();
    }

    private void DisplayHeader(bool IS_First, string str)
    {
        if (IS_First)
        {//当时YMG的时候，首先全部隐藏起来，然后根据用户自定义的字段，再显示出来；
            this.lblDocNO.Visible = false;
            this.lblDocNOText.Visible = false;
            this.lblUserName.Visible = false;
            this.lblUserNameText.Visible = false;
            this.Literal6.Visible = false;
            this.LiteralDateTime.Visible = false;
            this.Literal1.Visible = false;
            this.LiteralFromDept.Visible = false;
            this.Literal3.Visible = false;
            this.LiteralFromLine.Visible = false;
            this.Literal5.Visible = false;
            this.LiteralToLine.Visible = false;
        }
        else
        {
            switch (str)
            {
                case "[DOC_NO]":
                    this.lblDocNO.Visible = true;
                    this.lblDocNOText.Visible = true;
                    break;
                case "[USER_NAME]":
                    this.lblUserName.Visible = true;
                    this.lblUserNameText.Visible = true;
                    break;
                case "[TRX_DATE]":
                    this.Literal6.Visible = true;
                    this.LiteralDateTime.Visible = true;
                    break;
                case "[PROCESS_CD]":
                    this.Literal1.Visible = true;
                    this.LiteralFromDept.Visible = true;
                    break;
                case "[PRODUCTION_LINE_CD]":
                    this.Literal3.Visible = true;
                    this.LiteralFromLine.Visible = true;
                    break;
                case "[NEXT_PRODUCTION_LINE_CD]":
                    this.Literal5.Visible = true;
                    this.LiteralToLine.Visible = true;
                    break;
            }
        }
    }

    private void ShowDetail(DataRow[] rows)
    {
        bool ShowCustomer = false;
        string html = "";
        this.divYMG.InnerHtml += "<table class='borderTalbe centerAlignTable'>";
        html += "<tr  class='FieldStyle'>";
        for (int i = 1; i < rows[0].Table.Columns.Count; i++)
        {
            if (rows[0].Table.Columns[i].ColumnName.ToString().Contains("CUSTOMER_NAME"))
            {
                ShowCustomer = true;
            }
            else
            {
                html += "<td align= 'center' style='font-size:11px; height:11px'>" + ChangeText(rows[0].Table.Columns[i].ColumnName.ToString()) + "</td>";
            }
        }
        html += "</tr>";
        this.divYMG.InnerHtml += "<tr  class='FieldStyle'><td>" + ChangeText("JOB_ORDER_NO") + "</td><td colspan='" + (rows[0].Table.Columns.Count - 1) + "'>" + rows[0]["JOB_ORDER_NO"].ToString() + "</td></tr>";
        if (ShowCustomer)
        {
            this.divYMG.InnerHtml += "<tr  class='FieldStyle'><td>" + ChangeText("CUSTOMER_NAME") + "</td><td colspan='" + (rows[0].Table.Columns.Count - 1) + "'>" + rows[0]["CUSTOMER_NAME"].ToString() + "</td></tr>";
        }
        this.divYMG.InnerHtml += html;
        foreach (DataRow dr in rows)
        {
            this.divYMG.InnerHtml += "<tr style='font-size:10px; height:10px'>";
            for (int i = 1; i < dr.Table.Columns.Count; i++)
            {
                if (ShowCustomer && i == 1)
                {
                }
                else
                {
                    this.divYMG.InnerHtml += "<td align= 'right'>" + dr[i].ToString() + "</td>";
                }
            }
            this.divYMG.InnerHtml += "</tr>";
        }
    }

    private void ShowYMGDetail(DataRow[] rows)
    {
        bool ShowCustomer = false;
        string html = "";
        this.divYMG.InnerHtml += "<table class='borderTalbe centerAlignTable'>";

        this.divYMG.InnerHtml += html;
        foreach (DataRow dr in table.Rows)
        {
            this.divYMG.InnerHtml += "<tr style='font-size:11px; height:11px'>";
            for (int i = 1; i < dr.Table.Columns.Count; i++)
            {
                if (ShowCustomer && i == 1)
                {
                }
                else
                {
                    this.divYMG.InnerHtml += "<td align= 'right'style='font-size:11px; height:11px'>" + dr[i].ToString() + "</td>";
                }
            }
            this.divYMG.InnerHtml += "</tr>";
        }
    }

    private void ShowYMGTotalSummary(DataTable table)
    {
        int[] Sum = { 0, 0, 0, 0 };
        if (RunNo == false)
        {
            this.divYMG.InnerHtml += "<table class='borderTalbe centerAlignTable'>";
            this.divYMG.InnerHtml += "<tr class='FieldStyle'>";
            this.div1.InnerHtml += "<table class='borderTalbe centerAlignTable'>";
            this.div1.InnerHtml += "<tr class='FieldStyle'>";
            for (int i = 0; i < table.Columns.Count; i++)
            {

                if (i == 0 || i == 1)
                {
                    this.div1.InnerHtml += "<td align= 'center'style='font-size:11px; height:11px'>" + ChangeText(table.Columns[i].ColumnName.ToString()) + "</td>";

                    this.divYMG.InnerHtml += "<td align= 'center' style='font-size:11px; height:11px'>" + ChangeText(table.Columns[i].ColumnName.ToString()) + "</td>";
                }
                else
                {
                    if (i == 3 || i == 4)
                    {
                        this.div1.InnerHtml += "<td align= 'center' style='font-size:11px; height:11px'>" + ChangeText(table.Columns[i].ColumnName.ToString()) + "</td>";
                    }
                    else
                    {
                        this.divYMG.InnerHtml += "<td align= 'center' style='font-size:11px; height:11px'>" + ChangeText(table.Columns[i].ColumnName.ToString()) + "</td>";

                    }

                }
            }
            RunNo = true;
            this.divYMG.InnerHtml += "</tr>";
            this.div1.InnerHtml += "</tr>";
        }

        foreach (DataRow dr in table.Rows)
        {

            this.divYMG.InnerHtml += "<tr  align='center' style='font-size:11px; height:11px'>";


            for (int i = 0; i < table.Columns.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Sum[j] = Convert.ToInt32(dr[j + 2].ToString());
                }
                if (i == 3 || i == 4)
                {
                    continue;
                }
                this.divYMG.InnerHtml += "<td align= 'center' style='font-size:11px; height:11px'>" + dr[i].ToString() + "</td>";
            }
            this.div1.InnerHtml += "</tr>";
            for (int i = 0; i < 3; i++)
            {
                if (i == 0)
                {

                    this.div1.InnerHtml += "<td align= 'center' style='font-size:11px; height:11px'>" + dr[0].ToString() + "</td>";
                    this.div1.InnerHtml += "<td align= 'center' style='font-size:11px; height:11px'>" + dr[1].ToString() + "</td>";

                }
                else
                {
                    this.div1.InnerHtml += "<td align= 'center' style='font-size:11px; height:11px'>" + Sum[i] + "</td>";
                }
            }
            CurrentTotalSum[0] += Sum[1];
            CurrentTotalSum[1] += Sum[2];
        }
    }

    private string ChangeText(string str)
    {
        string reStr = "";
        switch (str)
        {
            case "JOB_ORDER_NO":
                reStr = Resources.GlobalResources.STRING_JO_NO;
                break;
            case "GO_NO":
                reStr = Resources.GlobalResources.STRING_GO_NO;
                break;
            case "CUSTOMER_NAME":
                reStr = Resources.GlobalResources.STRING_CUSTOMER;
                break;
            case "COLOR_CD":
                reStr = Resources.GlobalResources.STRING_COLOR_CD;
                break;
            case "COLOR_DESC":
                reStr = Resources.GlobalResources.STRING_COLOR_DESC;
                break;
            case "SIZE_CD":
                reStr = Resources.GlobalResources.STRING_SIZE_CD;
                break;
            case "CUT_LAY_NO":
                reStr = Resources.GlobalResources.STRING_CUT_LOT_NO;
                break;
            case "BUNDLE_NO":
                reStr = Resources.GlobalResources.STRING_BUNDLE_NO;
                break;
            case "QTY":
                reStr = Resources.GlobalResources.STRING_QTY;//detail
                break;
            case "TOTAL_BUNDLE_NO_QTY":
                reStr = Resources.GlobalResources.STRING_TOTAL_BUNDLE_NO;
                break;
            case "TOTAL_QTY_BY_PIC":
                reStr = Resources.GlobalResources.STRING_TOTAL_QTY_BY_PIC;
                break;
            case "TOTAL_CUT_QTY":
                reStr = Resources.GlobalResources.STRING_TOTAL_CUT_QTY;
                break;
            case "CURRENT_TOTAL_QTY":
                reStr = Resources.GlobalResources.STRING_CURRENT_TOTAL_QTY;
                break;
            case "TOTAL_OUTPUT_QTY":
                reStr = Resources.GlobalResources.STRING_TOTAL_OUTPUT_QTY;
                break;
            case "CURRENT_TOTAL_BUNDLE_NO":
                reStr = Resources.GlobalResources.STRING_CURRENT_TOTAL_BUNDLE_NO;
                break;
        }
        return reStr;
    }
}
