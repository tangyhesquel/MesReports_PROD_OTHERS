using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using MESComment;

public partial class Reports_mesMarkerCSBreakdown : pPage
{
    public string MO_NO, JO_NO;
    int[] value=new int[30];//Added by ZouShCh ON 2013.07.26 修改数组长度从20变为30
    int sumValue = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        string moNO = "";
        if (Request.QueryString.AllKeys.Contains("moNo"))
        {
            moNO = Request.QueryString["moNO"].ToString();
        }
        if (moNO != "")
        {
            txtMoNo.Text = moNO;
            btnQuery_Click(this.btnQuery,null);
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        string sizeRow = "1";
        bool flag;
        DataTable Header = MESComment.MesOutSourcePriceSql.GetMarkerCSBreakdownHeader(txtMoNo.Text.Trim());
        MO_NO = Header.Rows[0]["MO_NO"].ToString();
        JO_NO = Header.Rows[0]["JO_NO"].ToString();
        sizeRow = Header.Rows[0]["SIZE_ROWSPAN"].ToString();

        divFirst.InnerHtml = "";

        divFirst.InnerHtml += "<table border='1' cellspacing='0' cellpadding='0' style='font-size:12px;border-collapse:collapse'><tr><td class='tr2style' bgcolor='#efefe7' width='100' >Markers</td>";
        DataTable Title = MESComment.MesOutSourcePriceSql.GetMarkerCSBreakdownTitle(txtMoNo.Text.Trim());
        foreach (DataRow row in Title.Rows)
        {
            divFirst.InnerHtml += "<td>" + row["MARKER_ID"] + "</td>";
        }
        divFirst.InnerHtml += "</tr><tr><td class='tr2style' bgcolor='#efefe7' > Length<p/>Yard/Inch</td>";
        foreach (DataRow row in Title.Rows)
        {
            divFirst.InnerHtml += "<td>" + row["MARKER_LEN_YDS"] + "/" + row["MARKER_LEN_INCH"] + "</td>";
        }
        divFirst.InnerHtml += "</tr><tr><td class='tr2style' bgcolor='#efefe7'  rowspan='" + sizeRow + "'>Sizes</td>";
        DataTable Size = MESComment.MesOutSourcePriceSql.GetMarkerCSBreakdownSizeDetail(txtMoNo.Text.Trim());
        ArrayList count = new ArrayList();
        
        for (int i = 0; i < Size.Rows.Count; i++)
        {
            count.Add(0);
        }

        for (int k = 0; k <MesRpt.ParseInt(sizeRow.Trim()); k++)
        {
            int m = 0;
            for (int i = m; i < Title.Rows.Count; i++)
            {
                flag = false;
                for (int j = 0; j < Size.Rows.Count; j++)
                {
                    if (Size.Rows[j]["MARKER_ID"].ToString() == Title.Rows[i]["MARKER_ID"].ToString())
                    {
                        divFirst.InnerHtml += "<td>" + Size.Rows[j]["SIZE_CD"] + "</td>";
                        m++;
                        flag = true;
                        if (MesRpt.ParseInt(Size.Rows[j]["RATION"].ToString()) == MesRpt.ParseInt(count[j].ToString()) + 1)
                        {
                            Size.Rows.RemoveAt(j);
                            count.RemoveAt(j);
                        }
                        else
                        {
                            count[j] = MesRpt.ParseInt(count[j].ToString()) + 1;
                        }
                        break;
                    }                    
                }
                if(!flag)
                    divFirst.InnerHtml += "<td></td>";
            }
            divFirst.InnerHtml += "</tr>";
        }

        divFirst.InnerHtml += "<tr><td class='tr2style' bgcolor='#efefe7'>  Number Of Sizes</td>";
        foreach (DataRow row in Title.Rows)
        {
            divFirst.InnerHtml += "<td>" + row["TOTAL_RATION"] + "</td>";
        }
        divFirst.InnerHtml += "</tr></table>";

        //gvBody
        string COLOR_CD = "";
        DataTable dtDetail = new DataTable();
        dtDetail.Columns.Add("COLOR_CD");

        foreach (DataRow row in Title.Rows)
        {
            dtDetail.Columns.Add(row["MARKER_ID"].ToString());
        }

        DataTable colorDetail = MESComment.MesOutSourcePriceSql.GetMarkerCSBreakdownColorDetail(txtMoNo.Text.Trim());
        int index = -1;
        for (int i = 0; i < colorDetail.Rows.Count; i++)
        {
            if (COLOR_CD != colorDetail.Rows[i]["COLOR_CD"].ToString())
            {
                index++;
                DataRow row = dtDetail.NewRow();
                row["COLOR_CD"] = colorDetail.Rows[i]["COLOR_CD"].ToString();
                dtDetail.Rows.Add(row);
            }
            dtDetail.Rows[index][colorDetail.Rows[i]["MARKER_ID"].ToString()] = colorDetail.Rows[i]["PLYS"];
            COLOR_CD = colorDetail.Rows[i]["COLOR_CD"].ToString();
        }

        gvBody.AutoGenerateColumns = false;
        gvBody.Columns.Clear();
        BoundField col = new BoundField();
        col.DataField = "COLOR_CD";
        col.Visible = true;
        gvBody.Columns.Add(col);
        foreach(DataRow row in Title.Rows)
        {
            BoundField col1 = new BoundField();
            col1.DataField = row["MARKER_ID"].ToString();
            gvBody.Columns.Add(col1);
        }
        BoundField col2 = new BoundField();
        gvBody.Columns.Add(col2);
        col.Visible = true;
        gvBody.DataBind();
        gvBody.DataSource = dtDetail;
        gvBody.DataBind();
    }

    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtTitle = MESComment.MesOutSourcePriceSql.GetMarkerCSBreakdownTitle(txtMoNo.Text.Trim());
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();
                tcHeader.Add(new TableHeaderCell());
                tcHeader[0].Text = "Color/Pattern";
                tcHeader[0].Attributes.Add("bgcolor", "#efefe7");
                for (int i = 0; i < dtTitle.Rows.Count; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i + 1].Text = dtTitle.Rows[i]["MARKER_ID"].ToString();
                }
                tcHeader.Add(new TableHeaderCell());
                tcHeader[dtTitle.Rows.Count+1].Text = "Totals";
                tcHeader[dtTitle.Rows.Count+1].Attributes.Add("bgcolor", "#efefe7");
                break;
            case DataControlRowType.DataRow:
                sumValue = 0;
                for (int i = 0; i < dtTitle.Rows.Count; i++)
                {
                    sumValue +=  MesRpt.ParseInt(e.Row.Cells[i + 1].Text.Trim()) * MesRpt.ParseInt(dtTitle.Rows[i]["TOTAL_RATION"].ToString() );
                }
                e.Row.Cells[dtTitle.Rows.Count + 1].Text = sumValue.ToString();
                for (int i = 0; i <= dtTitle.Rows.Count; i++)
                {
                    value[i] += MesRpt.ParseInt(e.Row.Cells[i+1].Text.ToString());
                }
                break;
            case DataControlRowType.Footer:
                e.Row.Cells[0].Text = "Totals(Plies)";
                for (int i = 0; i <= dtTitle.Rows.Count; i++)
                {
                    e.Row.Cells[i+1].Text = value[i].ToString();
                }
                break;
        }
    }
}
