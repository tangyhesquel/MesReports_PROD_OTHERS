using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Mes_mesMarkerAllocation : pPage
{
    public string SHORT_NAME, GO_NO, PART_TYPE;
    public int DetailSizeNO=0;
    public string strCombineSize; 
   
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
        SHORT_NAME = "";
        GO_NO = "";
        PART_TYPE = "";
        if (gvBody.Columns.Count > 6)
        {
            int i = gvBody.Columns.Count - 6;
            while (i > 0)
            {
                gvBody.Columns.RemoveAt(6);
                i--;
            }
        }
        DataTable MakerSizeList = MESComment.MesOutSourcePriceSql.GetMarkerDetailTitle(txtMoNo.Text.Trim());
        foreach (DataRow row in MESComment.MesOutSourcePriceSql.GetMarkerAlloDetailHeader(txtMoNo.Text.Trim()).Rows)
        {
            SHORT_NAME = row["SHORT_NAME"].ToString();
            GO_NO = row["GO_NO"].ToString();
            PART_TYPE = row["PART_TYPE"].ToString();
        }
        DataTable ResultList = MESComment.MesOutSourcePriceSql.GetMarkerAlloctionDetail(txtMoNo.Text.Trim());
        for (int i = 0; i < MakerSizeList.Rows.Count; i++)
        {
            ResultList.Columns.Add(MakerSizeList.Rows[i]["SIZE_CD"].ToString());
        }
        ResultList.Columns.Add("Total");
        DataTable DetailSize = new DataTable();
        foreach (DataRow row in ResultList.Rows)
        {
            //int j = 0;
            DetailSize.Clear();
            //if (this.Page.Request.QueryString.Get("site").ToString().ToUpper() == "DEV" || this.Page.Request.QueryString.Get("site").ToString().ToUpper() == "EGM" || this.Page.Request.QueryString.Get("site").ToString().ToUpper() == "YMG")
             if (this.Page.Request.QueryString.Get("site").ToString().ToUpper() == "DEV" || this.Page.Request.QueryString.Get("site").ToString().ToUpper() == "EGM")

            {
                DetailSize = MESComment.MesOutSourcePriceSql.GetMarkerAlloctionDetailSizeCutPlan(txtMoNo.Text.Trim());
            }
            else
            {
                DetailSize = MESComment.MesOutSourcePriceSql.GetMarkerAlloctionDetailSize(txtMoNo.Text.Trim());

            }
            foreach (DataRow row1 in DetailSize.Rows)
            {
                if (row["MARKER_ID"].ToString().Trim().Equals(row1["MARKER_ID"].ToString().Trim()) && row["COLOR_CD"].ToString().Trim().Equals(row1["COLOR_CD"].ToString().Trim()))
                {//找到同marker_id以及color_cd的marker_id,需要循环输入size的数;
                    for (int i =0; i < MakerSizeList.Rows.Count; i++)
                    {
                        if (row1["SIZE_CD"].ToString().Trim().Equals(MakerSizeList.Rows[i]["SIZE_CD"].ToString().Trim()))
                        {
                            if (row[row1["SIZE_CD"].ToString().Trim()].ToString().Equals(""))
                            {//如果resultlist中的size数是空的话,就设置size数;
                                row[row1["SIZE_CD"].ToString().Trim()] = row1["RATION"];
                            }
                        }
                    }
                }
            }
            row["Total"] = row["RATIONS"];
        }

      

        foreach (DataRow row in MakerSizeList.Rows)
        {
            BoundField col = new BoundField();
            col.DataField = row["SIZE_CD"].ToString();
            gvBody.Columns.Add(col);
        }
        BoundField col1 = new BoundField();
        col1.DataField = "Total";
        gvBody.Columns.Add(col1);
        gvBody.DataSource = ResultList;
        gvBody.DataBind();
        //if (this.Page.Request.QueryString.Get("site").ToString().ToUpper() == "DEV" || this.Page.Request.QueryString.Get("site").ToString().ToUpper() == "EGM" || this.Page.Request.QueryString.Get("site").ToString().ToUpper() == "YMGTEST")
        //{
            DataTable CombineSizeRelation = MESComment.MesOutSourcePriceSql.GetMarkerCombineSizeAndRelation(txtMoNo.Text.Trim());
            if (CombineSizeRelation.Rows.Count > 0)
            {
                gvSize.DataSource = CombineSizeRelation;
                gvSize.DataBind();
                this.MergeSize.Visible = true;
            }
            else
            {
                this.MergeSize.Visible = false;
            }

            DataTable CombineColorRelation = MESComment.MesOutSourcePriceSql.GetMarkerCombineColorAndRelation(txtMoNo.Text.Trim());

            if (CombineColorRelation.Rows.Count > 0)
            {
                gvColor.DataSource = CombineColorRelation;
                gvColor.DataBind();
                this.MergeColor.Visible = true;
            }
            else
            {
                this.MergeColor.Visible = false;
            }
        //}
    }



    string strHeader = "Marker Set ID,Color Code,Color Desc,Gmt Qty,Ply,DEL DATE";
    int gmtQty = 0;
    int plyQty = 0;
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();
                for (int i = 0; i < 6; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("rowspan", "2");
                    tcHeader[i].Attributes.Add("bgcolor", "#efefe7");
                    tcHeader[i].Text = strHeader.Split(',')[i];
                }
                DataTable dt = MESComment.MesOutSourcePriceSql.GetMarkerDetailTitle(txtMoNo.Text.Trim());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i + 6].Attributes.Add("bgcolor", "#efefe7");
                    tcHeader[i + 6].Text = dt.Rows[i]["SIZE_CD"].ToString();
                }
                tcHeader.Add(new TableHeaderCell());
                tcHeader[dt.Rows.Count + 6].Attributes.Add("bgcolor", "#efefe7");
                tcHeader[dt.Rows.Count + 6].Text = "Total</th></tr><tr>";
                int total = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i + 6 + dt.Rows.Count + 1].Attributes.Add("bgcolor", "#efefe7");
                    tcHeader[i + 6 + dt.Rows.Count + 1].Text = dt.Rows[i]["ORDER_QTY"].ToString();
                    total += dt.Rows[i]["ORDER_QTY"].ToString() == "" ? 0 : int.Parse(dt.Rows[i]["ORDER_QTY"].ToString());
                }
                tcHeader.Add(new TableHeaderCell());
                tcHeader[dt.Rows.Count * 2 + 6 + 1].Attributes.Add("bgcolor", "#efefe7");//#efefe7
                tcHeader[dt.Rows.Count * 2 + 6 + 1].Text = total.ToString();
                break;
            case DataControlRowType.DataRow:
                gmtQty += int.Parse(e.Row.Cells[3].Text);
                plyQty += int.Parse(e.Row.Cells[4].Text);
                break;
            case DataControlRowType.Footer:
                e.Row.Cells[0].Text = "Total";
                e.Row.Cells[3].Text = gmtQty.ToString();
                e.Row.Cells[4].Text = plyQty.ToString();
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Attributes.Add("bgcolor", "#efefe7");
                }
                    break;
        }

        


    }

    
    #region 合并单元格 合并某一行的所有列

    ///　 <summary>　
    ///　 合并GridView中某行相同信息的行（单元格）
    ///　 </summary>　
    ///　 <param　 name="GridView1">GridView对象</param>　
    ///　 <param　 name="cellNum">需要合并的行</param>
    ///　 <param name="sCol">开始列</param>
    ///　 <param name="eCol">结束列</param>
    public static void GroupRow(GridView gridView, int rows,int sCol,int eCol)
    {
        TableCell oldTc = gridView.Rows[rows].Cells[sCol];
        for (int i = 1; i < eCol-sCol; i++)
        {
            
            TableCell tc = gridView.Rows[rows].Cells[i+sCol];　 //Cells[0]就是你要合并的列
            if (oldTc.Text == tc.Text)
            {
                tc.Visible = false;
                if (oldTc.ColumnSpan == 0)
                {
                    oldTc.ColumnSpan = 1;
                }
                oldTc.ColumnSpan++;
                oldTc.VerticalAlign = VerticalAlign.Middle;
            }
            else
            {
                oldTc = tc;
            }
        }
    }
    #endregion

    //<Added by:ZouShCh at:2013.03.21>




    public static void GroupRow(GridView gridView)
    {
        for (int rowIndex = gridView.Columns.Count - 2; rowIndex >= 0; rowIndex--)
        {
            GridViewRow row = gridView.Rows[rowIndex];
            GridViewRow previousRow = gridView.Rows[rowIndex + 1];

            for (int i = 0; i < row.Cells.Count; i++)
            {
                if (row.Cells[i].Text == previousRow.Cells[i].Text)
                {
                    row.Cells[i].RowSpan = previousRow.Cells[i].RowSpan < 2 ? 2 :
                                           previousRow.Cells[i].RowSpan + 1;
                    previousRow.Cells[i].Visible = false;
                }
            }
        }
    }



    #region 合并单元格 合并某一行的所有列
    ///　 <summary>　
    ///　 合并GridView中某行相同信息的行（单元格）
    ///　 </summary>　
    ///　 <param　 name="GridView1">GridView对象</param>　
    ///　 <param　 name="cellNum">需要合并的行</param>
    public static void GroupRow(GridView GridView1, int rows)
    {
        TableCell oldTc = GridView1.Rows[rows].Cells[0];
        for (int i = 1; i < GridView1.Rows[rows].Cells.Count; i++)
        {
            TableCell tc = GridView1.Rows[rows].Cells[i];　 //Cells[0]就是你要合并的列
           
            if (oldTc.Text == tc.Text)
            {
                if (tc.Text.ToString() == "&nbsp;")
                {
                    continue;
                }
                tc.Visible = false;
                if (oldTc.ColumnSpan == 0)
                {
                    oldTc.ColumnSpan = 1;
                }
                oldTc.ColumnSpan++;
                oldTc.VerticalAlign = VerticalAlign.Middle;
                oldTc.HorizontalAlign = HorizontalAlign.Center;
                

            }
            else
            {
                oldTc = tc;
            }
        }
    }
    #endregion


    protected void gvSize_DataBound(object sender, EventArgs e)
    {
        for (int i = 0; i < gvSize.Rows.Count; i++)
        {
            GroupRow(gvSize,i);
        }
        GroupRow(gvSize);
    }

 
    
    protected void gvColor_DataBound(object sender, EventArgs e)
    {
        for (int i = 0; i <  gvColor.Rows.Count; i++)
        {
            GroupRow(gvColor, i);     
        }
        GroupRow(gvColor);
    }

    protected void gvColor_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            { 
                e.Row.Cells[i].ControlStyle.Width = Unit.Pixel(100);              
            }
        }
    }
}
