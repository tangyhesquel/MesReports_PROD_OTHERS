using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Data;
using MESComment;

public partial class Reports_GarmentTransferForCutting : pPage
{
    private DataTable dataDetail;
    protected DataTable tb;

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            if (Request.QueryString.Get("docNO") == null)
                return;
            string docNO = Request.QueryString.GetValues("docNO")[0];
            dataDetail = MESComment.MesRpt.GetGarmentCuttingTransferDetail(docNO);
            DataTable table = MESComment.MesRpt.GetGarmentCuttingTransferHeaderInfo(docNO);

            LiteralFromDept.Text = ConvertEmptyString(table.Rows[0]["PROCESS_CD"].ToString());
            LiteralFromLine.Text = ConvertEmptyString(table.Rows[0]["PRODUCTION_LINE_CD"].ToString());
            LiteralToLine.Text = ConvertEmptyString(table.Rows[0]["NEXT_PRODUCTION_LINE_CD"].ToString());
            LiteralDocNO.Text = docNO;
            LiteralUserID.Text = ConvertEmptyString(table.Rows[0]["CREATE_USER_ID"].ToString());
            LiteralDeliverDate.Text = ConvertEmptyString(table.Rows[0]["SUBMIT_DATE"].ToString());

            DataTable joList = dataDetail.DefaultView.ToTable(true, new string[] { "JOB_ORDER_NO" });
            RepeaterJo.DataSource = joList;
            RepeaterJo.DataBind();

        }
    }

    private static string ConvertEmptyString(string s)
    {
        if (s == string.Empty)
            return "NA";
        else
            return s;
    }



    protected void RepeaterJo_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        GridView gv = e.Item.FindControl("GridViewDetail") as GridView;
        DataView dv = dataDetail.DefaultView;
        dv.RowFilter = "JOB_ORDER_NO='" + (e.Item.DataItem as DataRowView)["JOB_ORDER_NO"].ToString() + "'";
        gv.DataSource = dv;
        gv.DataBind();
        gv.FooterRow.Cells[gv.Columns.Count - 1].Text = dataDetail.Compute("Sum(QTY)", "JOB_ORDER_NO='" + (e.Item.DataItem as DataRowView)["JOB_ORDER_NO"].ToString() + "'").ToString();
        var q = from DataRowView rv in dv
                group rv by new { color = rv["COLOR_CD"].ToString(), size = rv["SIZE_CD"].ToString() } into g
                select new { ID = g.Key, Total = g.Sum(a => Convert.ToDecimal(a["Qty"].ToString())) };
        tb = new DataTable();
        tb.Columns.Add("COLOR_CD", typeof(String));
        DataTable t = dv.ToTable(true, new string[] { "SIZE_CD" });
        for (int i = 0; i < t.Rows.Count; i++)
        {
            tb.Columns.Add(t.Rows[i]["SIZE_CD"].ToString(), typeof(int));
        }

        tb.Columns.Add("TOTAL", typeof(int));

        foreach (var r in q)
        {
            string color = r.ID.color;
            string size = r.ID.size;
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

    private DataTable FormatDataTable(DataTable summaryTable, int SizeCountPerRow)
    {
        string headerText = Resources.GlobalResources.STRING_COLOR_CD + "\\" + Resources.GlobalResources.STRING_SIZE_CD;
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

                rowHeader["SIZE" + ((j - 1) % SizeCountPerRow).ToString()] = summaryTable.Columns[j].ColumnName;
                rowData["SIZE" + ((j - 1) % SizeCountPerRow).ToString()] = summaryTable.Rows[i][j].ToString();
            }

            DataRow footer = table.NewRow();
            table.Rows.Add(footer);
            footer[0] = "Total";
            footer[1] = summaryTable.Rows[i]["TOTAL"].ToString();
            footer = table.NewRow();
            table.Rows.Add(footer);
        }

        return table;
    }

}
