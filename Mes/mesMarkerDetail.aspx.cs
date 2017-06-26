using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;

public partial class Mes_mesMarkerDetail : pPage
{
    public string SHORT_NAME, GO_NO, MARKER_WASTAGE, PART_TYPE, AVG_MU, netYpd, ypd, REMARKS, Total;
    DataTable dtTitle = new DataTable();
    DataTable Detail = new DataTable();
    float[] TotalValue = new float[20];
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string moNo = Request.QueryString["moNo"];
            if (moNo != "" && moNo != null)
            {
                this.txtMoNo.Text = moNo;
                btnQuery_Click(this.btnQuery, null);
            }
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        DataTable header = MESComment.MesOutSourcePriceSql.GetMarkerDetailHeader(txtMoNo.Text.Trim());
        if (header.Rows.Count > 0)
        {
            SHORT_NAME = header.Rows[0]["SHORT_NAME"].ToString();
            GO_NO = header.Rows[0]["GO_NO"].ToString();
            MARKER_WASTAGE = header.Rows[0]["MARKER_WASTAGE"].ToString();
            PART_TYPE = header.Rows[0]["PART_TYPE"].ToString();
            AVG_MU = header.Rows[0]["AVG_MU"].ToString();
            netYpd = double.Parse(header.Rows[0]["NET_YPD"].ToString()).ToString("f2");
            ypd = double.Parse(header.Rows[0]["YPD"].ToString()).ToString("f2");
            REMARKS = header.Rows[0]["REMARKS"].ToString();
            Total = header.Rows[0]["GMT_QTY"].ToString();

            dtTitle = MESComment.MesOutSourcePriceSql.GetMarkerDetailTitle(txtMoNo.Text.Trim());
            Detail = MESComment.MesOutSourcePriceSql.GetMarkerDetailDetail(txtMoNo.Text.Trim());


            for (int i = 0; i < dtTitle.Rows.Count; i++)
            {//set size titles;
                Detail.Columns.Add(dtTitle.Rows[i]["SIZE_CD"].ToString());
                BoundField col = new BoundField();
                col.DataField = dtTitle.Rows[i]["SIZE_CD"].ToString();
                gvBody.Columns.Insert(8 + i, col);
            }
            DataTable DetailSize = MESComment.MesOutSourcePriceSql.GetMarkerDetailDetailSize(txtMoNo.Text.Trim());
            foreach (DataRow row in Detail.Rows)
            {
                foreach (DataRow row1 in DetailSize.Rows)
                {
                    if (row1["MARKER_ID"].ToString() == row["MARKER_ID"].ToString() && row1["COLOR_CD"].ToString() == row["COLOR_CD"].ToString())
                    {
                        row[row1["SIZE_CD"].ToString()] = row1["RATION"].ToString();
                    }
                }
            }
            string oldColorCd = "";
            double dblnetYpd = 0, markerYpd = 0;
            string dblnetYpd_NA = "";
            int dtRowCount = Detail.Rows.Count, k = 0, gmtQty = 0;
            string[] strSumMsg = new string[dtRowCount];
            for (int i = 0; i < dtRowCount; i++)
            {
                if (Detail.Rows[i]["COLOR_CD"].ToString() != oldColorCd && oldColorCd != "")
                {
                    if (i != 0)
                    {
                        strSumMsg[k] = "Marker Net YPD: " + dblnetYpd.ToString("0.00") + "    Gross Marker YPD: " + markerYpd.ToString("0.00") + " Gmt Qty: " + gmtQty + " ";
                        k++;
                        if (Detail.Rows[i]["NET_YPD"].ToString().Trim().Equals("N/A"))
                        {
                            dblnetYpd_NA = "N/A";
                        }
                        else
                        {
                            dblnetYpd = double.Parse(Detail.Rows[i]["NET_YPD"].ToString());
                            markerYpd = double.Parse(Detail.Rows[i]["GROSS_MARKER_YPD"].ToString());
                        }
                        gmtQty = 0;
                    }
                    gmtQty += int.Parse(Detail.Rows[i]["GMT_QTY"].ToString() == "" ? "0" : Detail.Rows[i]["GMT_QTY"].ToString());
                }
                else
                {
                    if (Detail.Rows[i]["NET_YPD"].ToString().Trim().Equals("N/A"))
                    {
                        dblnetYpd_NA = "N/A";
                    }
                    else
                    {
                        dblnetYpd = double.Parse(Detail.Rows[i]["NET_YPD"].ToString());
                        markerYpd = double.Parse(Detail.Rows[i]["GROSS_MARKER_YPD"].ToString());
                    }
                    gmtQty += int.Parse(Detail.Rows[i]["GMT_QTY"].ToString());
                }
                if (i == dtRowCount - 1)
                {
                    if (dblnetYpd_NA.Equals("N/A"))
                    {
                        strSumMsg[k] = "Marker Net YPD: N/A    Gross Marker YPD: N/A Gmt Qty: " + gmtQty + " ";
                    }
                    else
                    {
                        strSumMsg[k] = "Marker Net YPD: " + dblnetYpd.ToString("0.00") + "    Gross Marker YPD: " + markerYpd.ToString("0.00") + " Gmt Qty: " + gmtQty + " ";
                    }
                }
                oldColorCd = Detail.Rows[i]["COLOR_CD"].ToString();
            }
            k = 0;
            oldColorCd = "";
            for (int i = 0; i < dtRowCount; i++)
            {
                if (Detail.Rows[i + k]["COLOR_CD"].ToString() != oldColorCd && oldColorCd != "")
                {
                    DataRow row = Detail.NewRow();
                    row["MARKER_ID"] = "-1";
                    row["COLOR_CD"] = "-1";
                    Detail.Rows.InsertAt(row, i + k);
                    k++;
                }
                if (i == dtRowCount - 1)
                {
                    DataRow row = Detail.NewRow();
                    row["MARKER_ID"] = "-1";
                    row["COLOR_CD"] = "-1";
                    Detail.Rows.InsertAt(row, i + k + 2);
                }
                oldColorCd = Detail.Rows[i + k]["COLOR_CD"].ToString();
            }
            gvBody.DataSource = Detail;
            gvBody.DataBind();
            for (int i = 0; i < dtTitle.Rows.Count; i++)
            {
                gvBody.Columns.RemoveAt(8);
            }
            k = 0;
            int[] SizeTotal = new int[DetailSize.Rows.Count];
            for (int i = 0; i < gvBody.Rows.Count; i++)
            {
                if (gvBody.Rows[i].Cells[0].Text == "-1")
                {
                    gvBody.Rows[i].Cells.Clear();
                    gvBody.Rows[i].Cells.Add(new TableCell());                    
                    gvBody.Rows[i].Cells[0].ColumnSpan = 8;
                    gvBody.Rows[i].Cells[0].Text = strSumMsg[k];                    
                    gvBody.Rows[i].Cells[0].Attributes.Add("bgcolor", "#efefe7");
                    gvBody.Rows[i].Cells[0].Attributes.Add("style", "color:blue");
                    gvBody.Rows[i].Cells[0].Attributes.Add("align", "left");
                    //先使用了SizeTotal[];
                    for (int Totaloutput = 0; Totaloutput < dtTitle.Rows.Count; Totaloutput++)
                    {
                        gvBody.Rows[i].Cells.Add(new TableCell());
                        gvBody.Rows[i].Cells[1 + Totaloutput].Text = SizeTotal[Totaloutput].ToString().Equals("0") ? "&nbsp;" : SizeTotal[Totaloutput].ToString();
                        gvBody.Rows[i].Cells[1 + Totaloutput].Attributes.Add("bgcolor", "#efefe7");
                        gvBody.Rows[i].Cells[1 + Totaloutput].Attributes.Add("style", "color:blue");
                        gvBody.Rows[i].Cells[1 + Totaloutput].Attributes.Add("align", "left");
                    }
                    gvBody.Rows[i].Cells.Add(new TableCell());
                    gvBody.Rows[i].Cells[1 + dtTitle.Rows.Count].ColumnSpan = gvBody.Rows[0].Cells.Count - 8 - dtTitle.Rows.Count;
                    gvBody.Rows[i].Cells[1 + dtTitle.Rows.Count].Attributes.Add("bgcolor", "#efefe7");
                    gvBody.Rows[i].Cells[1 + dtTitle.Rows.Count].Attributes.Add("style", "color:blue");
                    gvBody.Rows[i].Cells[1 + dtTitle.Rows.Count].Attributes.Add("align", "left");
                    k++;
                    for (int sizecol = 0; sizecol < dtTitle.Rows.Count; sizecol++)
                    {//重置SizeTotal[];
                        SizeTotal[sizecol] = 0;
                    }
                }
                else
                {
                    for (int sizecol = 0; sizecol < dtTitle.Rows.Count; sizecol++)
                    {//整行计算Size Total;
                        int size_plys_qty = Int32.Parse(gvBody.Rows[i].Cells[7].Text);
                        string str = gvBody.Rows[i].Cells[8 + sizecol].Text.Equals("&nbsp;") ? "0" : gvBody.Rows[i].Cells[8 + sizecol].Text;
                        int size_qty = Int32.Parse(str);
                        SizeTotal[sizecol] += size_plys_qty * size_qty;
                    }
                }
            }
        }
    }

    string strHeader = "Marker ID,Marker Name,Color Code,Color Desc,Pattern Fabric,Description,Gmt Qty,Ply ";
    string strHeader1 = "Total,Marker Length,Cut/Sew Wastage,Fab Req,YPD,Shrinkage,Batch No,Fab Width</th></tr><tr>";
    string strHeader2 = "CV Yard,Yard,Inch ";

    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();
                //first row
                for (int i = 0; i < strHeader.Split(',').Length; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("rowspan", "2");
                    tcHeader[i].Text = strHeader.Split(',')[i];
                    tcHeader[i].Attributes.Add("bgcolor", "#efefe7");
                }
                for (int i = 0; i < dtTitle.Rows.Count; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i + strHeader.Split(',').Length].Text = dtTitle.Rows[i]["SIZE_CD"].ToString();
                    tcHeader[i + strHeader.Split(',').Length].Attributes.Add("bgcolor", "#efefe7");
                }
                for (int i = 0; i < strHeader1.Split(',').Length; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    if (i == 1)
                    {
                        tcHeader[i + strHeader.Split(',').Length + dtTitle.Rows.Count].Attributes.Add("colspan", "3");
                    }
                    if (i != 0 && i != 1)
                    {
                        tcHeader[i + strHeader.Split(',').Length + dtTitle.Rows.Count].Attributes.Add("rowspan", "2");
                    }
                    tcHeader[i + strHeader.Split(',').Length + dtTitle.Rows.Count].Text = strHeader1.Split(',')[i];
                    tcHeader[i + strHeader.Split(',').Length + dtTitle.Rows.Count].Attributes.Add("bgcolor", "#efefe7");
                }
                //second row
                int colTotal = 0;
                for (int i = 0; i < dtTitle.Rows.Count; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i + strHeader.Split(',').Length + dtTitle.Rows.Count + strHeader1.Split(',').Length].Text = dtTitle.Rows[i]["ORDER_QTY"].ToString();
                    tcHeader[i + strHeader.Split(',').Length + dtTitle.Rows.Count + strHeader1.Split(',').Length].Attributes.Add("bgcolor", "#efefe7");
                    colTotal += int.Parse(dtTitle.Rows[i]["ORDER_QTY"].ToString());
                }
                tcHeader.Add(new TableHeaderCell());
                tcHeader[strHeader.Split(',').Length + dtTitle.Rows.Count + strHeader1.Split(',').Length + dtTitle.Rows.Count].Text = colTotal.ToString();
                tcHeader[strHeader.Split(',').Length + dtTitle.Rows.Count + strHeader1.Split(',').Length + dtTitle.Rows.Count].Attributes.Add("bgcolor", "#efefe7");
                for (int i = 0; i < strHeader2.Split(',').Length; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[strHeader.Split(',').Length + dtTitle.Rows.Count + strHeader1.Split(',').Length + dtTitle.Rows.Count + 1 + i].Text = strHeader2.Split(',')[i];
                    tcHeader[strHeader.Split(',').Length + dtTitle.Rows.Count + strHeader1.Split(',').Length + dtTitle.Rows.Count + 1 + i].Attributes.Add("bgcolor", "#efefe7");
                }
                break;
            case DataControlRowType.DataRow:
                TotalValue[0] += int.Parse(e.Row.Cells[6].Text == "&nbsp;" ? "0" : e.Row.Cells[6].Text);
                TotalValue[1] += int.Parse(e.Row.Cells[7].Text == "&nbsp;" ? "0" : e.Row.Cells[7].Text);
                for (int i = 0; i < dtTitle.Rows.Count; i++)
                {
                    TotalValue[i + 2] += int.Parse(e.Row.Cells[8 + i].Text == "&nbsp;" ? "0" : e.Row.Cells[8 + i].Text) * int.Parse(e.Row.Cells[7].Text == "&nbsp;" ? "0" : e.Row.Cells[7].Text);
                }
                TotalValue[dtTitle.Rows.Count + 2] += int.Parse(e.Row.Cells[8 + dtTitle.Rows.Count].Text == "&nbsp;" ? "0" : e.Row.Cells[8 + dtTitle.Rows.Count].Text) * int.Parse(e.Row.Cells[7].Text == "&nbsp;" ? "0" : e.Row.Cells[7].Text);
                TotalValue[dtTitle.Rows.Count + 3] += float.Parse(e.Row.Cells[8 + dtTitle.Rows.Count + 5].Text == "&nbsp;" ? "0" : e.Row.Cells[8 + dtTitle.Rows.Count + 5].Text);
                break;
            case DataControlRowType.Footer:
                TableCellCollection tcFooter = e.Row.Cells;
                tcFooter.Clear();
                tcFooter.Add(new TableHeaderCell());
                int cols = 17 + dtTitle.Rows.Count;
                for (int i = 0; i < cols; i++)
                {
                    tcFooter.Add(new TableHeaderCell());
                    tcFooter[i].Attributes.Add("align", "left");
                    if (i == 0)
                    {
                        tcFooter[i].Text = "Total";
                    }
                    if (i >= 6 && i < 6 + dtTitle.Rows.Count + 3)
                    {
                        tcFooter[i].Text = TotalValue[i - 6].ToString();
                    }
                    if (i == 7 + dtTitle.Rows.Count + 6)
                    {
                        tcFooter[i].Text = TotalValue[dtTitle.Rows.Count + 3].ToString("f2");
                    }
                }
                break;
        }
    }
}
