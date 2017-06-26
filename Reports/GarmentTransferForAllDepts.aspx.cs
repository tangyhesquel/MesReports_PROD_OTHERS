using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Data;
using System.Threading;
using MESComment;

public partial class GarmentTransferForAllDepts : pPage
{
    private DataTable dataDetail;


    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            if (Request.QueryString.Get("docNO") == null)
                return;
            string docNO = Request.QueryString.GetValues("docNO")[0];
            dataDetail = MESComment.MesRpt.GetGarmentTransferDetail(docNO);
            DataTable table = MESComment.MesRpt.GetGarmentCuttingTransferHeaderInfo(docNO);

            LiteralFromDept.Text = ConvertEmptyString(table.Rows[0]["PROCESS_CD"].ToString());
            LiteralFromLine.Text = ConvertEmptyString(table.Rows[0]["PRODUCTION_LINE_CD"].ToString());
            LiteralToLine.Text = ConvertEmptyString(table.Rows[0]["NEXT_PRODUCTION_LINE_CD"].ToString());
            LiteralToDept.Text = ConvertEmptyString(table.Rows[0]["NEXT_PROCESS_CD"].ToString());
            LiteralDocNO.Text = docNO;
            LiteralUser.Text = ConvertEmptyString(table.Rows[0]["CREATE_USER_ID"].ToString());
            LiteralTransferTime.Text = ConvertEmptyString(table.Rows[0]["SUBMIT_DATE"].ToString());

            DataTable joList = dataDetail.DefaultView.ToTable(true, new string[] { "JOB_ORDER_NO", "CUSTOMER_NAME" });
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
        gv.FooterRow.Cells[gv.Columns.Count - 2].Text = Resources.GlobalResources.STRING_SUB_TOTAL;
        gv.FooterRow.Cells[gv.Columns.Count - 1].Text = dataDetail.Compute("Sum(QTY)", "JOB_ORDER_NO='" + (e.Item.DataItem as DataRowView)["JOB_ORDER_NO"].ToString() + "'").ToString();
    }


}
