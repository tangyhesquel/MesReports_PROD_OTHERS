using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using System.Data.Common;
using System.IO;


public partial class MES_MesMain : pPage
{
    DataTable data_ = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlView.Items.Clear();

            ddlView.Items.Add(new ListItem("Knit", "K"));
            ddlView.Items.Add(new ListItem("Woven", "W"));
            txtDate.Text = DateTime.Now.Date.ToString("yyyy-MM-dd");
            BuildHtml();
        }
        
    }

    private DataTable BuildDataSource()
    {
        DataTable tb = new DataTable();
        tb.Columns.Add("Key");
        tb.Columns.Add("f1");
        tb.Columns.Add("f2");
        tb.Columns.Add("f3");
        tb.Columns.Add("f4");
        tb.Columns.Add("f5");
        tb.Columns.Add("f6");
        tb.Columns.Add("f7");
        DataRow row_ = tb.NewRow();
        row_["Key"] = "AVG PRICE(RMB/PCS)"; tb.Rows.Add(row_);
        row_ = tb.NewRow(); row_["Key"] = "AVG SAH(Sewing/garment)"; tb.Rows.Add(row_);
        row_ = tb.NewRow(); row_["Key"] = "AVG SAH(Total/garment)"; tb.Rows.Add(row_);
        row_ = tb.NewRow(); row_["Key"] = "AVG SAH PRICE(RMB/Sewing SAH)"; tb.Rows.Add(row_);

        //get datasource from sql
        DataTable outTb = data_; //GetOutTable();

        DataRow[] avg_price_row = null;
        DataRow[] avg_sew_sah_row = null;
        DataRow[] avg_sah_row = null;
        DataRow[] avg_sah_price_row = null;
        string strTotalQty = "";
        if (outTb != null)
        {
            for (int i = 0; i < outTb.Rows.Count; i++)
            {
                if (strTotalQty == "") strTotalQty = outTb.Rows[0]["BAL_QTY"].ToString();
                avg_price_row = tb.Select("Key='AVG PRICE(RMB/PCS)'");
                if (avg_price_row.Length > 0)
                {
                    avg_price_row[0]["f1"] = outTb.Rows[i]["AVG_PRICE_M"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_PRICE_M"]).ToString("##,##0.###") : "";
                    avg_price_row[0]["f2"] = outTb.Rows[i]["AVG_PRICE_Y"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_PRICE_Y"]).ToString("##,##0.###") : "";
                    avg_price_row[0]["f3"] = outTb.Rows[i]["BUDGET_PRICE"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["BUDGET_PRICE"]).ToString("##,##0.###") : "";
                    avg_price_row[0]["f4"] = outTb.Rows[i]["STANDARD_PRICE"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["STANDARD_PRICE"]).ToString("##,##0.###") : "";
                    avg_price_row[0]["f5"] = outTb.Rows[i]["AVG_PRICE_ACT_M"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_PRICE_ACT_M"]).ToString("##,##0.###") : "";
                    avg_price_row[0]["f6"] = outTb.Rows[i]["AVG_PRICE_ACT_Y"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_PRICE_ACT_Y"]).ToString("##,##0.###") : "";
                    avg_price_row[0]["f7"] = outTb.Rows[i]["avg_price_last_y"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["avg_price_last_y"]).ToString("##,##0.###") : "";
                }

                avg_sew_sah_row = tb.Select("Key='AVG SAH(Sewing/garment)'");
                if (avg_sew_sah_row.Length > 0)
                {
                    avg_sew_sah_row[0]["f1"] = outTb.Rows[i]["AVG_SEW_SAH_M"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_SEW_SAH_M"]).ToString("##,##0.###") : "";
                    avg_sew_sah_row[0]["f2"] = outTb.Rows[i]["AVG_SEW_SAH_Y"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_SEW_SAH_Y"]).ToString("##,##0.###") : "";
                    avg_sew_sah_row[0]["f3"] = outTb.Rows[i]["BUDGET_SAH"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["BUDGET_SAH"]).ToString("##,##0.###") : "";
                    avg_sew_sah_row[0]["f4"] = outTb.Rows[i]["AVG_SEW_SAH_Y"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_SEW_SAH_Y"]).ToString("##,##0.###") : "";
                    avg_sew_sah_row[0]["f5"] = outTb.Rows[i]["AVG_SEW_SAH_ACT_M"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_SEW_SAH_ACT_M"]).ToString("##,##0.###") : "";
                    avg_sew_sah_row[0]["f6"] = outTb.Rows[i]["AVG_SEW_SAH_ACT_Y"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_SEW_SAH_ACT_Y"]).ToString("##,##0.###") : "";
                    avg_sew_sah_row[0]["f7"] = outTb.Rows[i]["avg_sew_sah_last_y"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["avg_sew_sah_last_y"]).ToString("##,##0.###") : "";
                }

                avg_sah_row = tb.Select("Key='AVG SAH(Total/garment)'");
                if (avg_sah_row.Length > 0)
                {
                    avg_sah_row[0]["f1"] = outTb.Rows[i]["AVG_SAH_M"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_SAH_M"]).ToString("##,##0.###") : "";
                    avg_sah_row[0]["f2"] = outTb.Rows[i]["AVG_SAH_Y"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_SAH_Y"]).ToString("##,##0.###") : "";
                    avg_sah_row[0]["f3"] = "X";
                    avg_sah_row[0]["f4"] = outTb.Rows[i]["AVG_SAH_Y"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_SAH_Y"]).ToString("##,##0.###") : "";
                    avg_sah_row[0]["f5"] = outTb.Rows[i]["AVG_SAH_ACT_M"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_SAH_ACT_M"]).ToString("##,##0.###") : "";
                    avg_sah_row[0]["f6"] = outTb.Rows[i]["AVG_SAH_ACT_Y"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_SAH_ACT_Y"]).ToString("##,##0.###") : "";
                    avg_sah_row[0]["f7"] = outTb.Rows[i]["avg_sah_last_y"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["avg_sah_last_y"]).ToString("##,##0.###") : "";
                }
                avg_sah_price_row = tb.Select("Key='AVG SAH PRICE(RMB/Sewing SAH)'");
                if (avg_sah_price_row.Length > 0)
                {
                    avg_sah_price_row[0]["f1"] = outTb.Rows[i]["AVG_SEW_SAH_PRICE_M"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_SEW_SAH_PRICE_M"]).ToString("##,##0.###") : "";
                    avg_sah_price_row[0]["f2"] = outTb.Rows[i]["AVG_SEW_SAH_PRICE_Y"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_SEW_SAH_PRICE_Y"]).ToString("##,##0.###") : "";
                    avg_sah_price_row[0]["f3"] = outTb.Rows[i]["BUDGET_SAH_PRICE"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["BUDGET_SAH_PRICE"]).ToString("##,##0.###") : "";
                    avg_sah_price_row[0]["f4"] = outTb.Rows[i]["STANDARD_SAH_PRICE"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["STANDARD_SAH_PRICE"]).ToString("##,##0.###") : "";
                    avg_sah_price_row[0]["f5"] = outTb.Rows[i]["AVG_SEW_SAH_PRICE_ACT_M"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_SEW_SAH_PRICE_ACT_M"]).ToString("##,##0.###") : "";
                    avg_sah_price_row[0]["f6"] = outTb.Rows[i]["AVG_SEW_SAH_PRICE_ACT_Y"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["AVG_SEW_SAH_PRICE_ACT_Y"]).ToString("##,##0.###") : "";
                    avg_sah_price_row[0]["f7"] = outTb.Rows[i]["avg_sew_sah_price_last_y"].ToString() != "" ? Convert.ToDecimal(outTb.Rows[i]["avg_sew_sah_price_last_y"]).ToString("##,##0.###") : "";
                }
            }
        }

        hfBalQty.Value = strTotalQty.ToString() != "" ? Convert.ToDecimal(strTotalQty).ToString("##,##0.###") : "";

        return tb;
    }
    private void GetOutTable()
    {
        data_ = MESComment.MesOutSourcePriceSql.GetMasOutSourcePrice(txtDate.Text, ddlView.SelectedValue, chkShirtstop.Checked);
    }

    private void BuildHtml()
    {
        DataTable tb = BuildDataSource();
        string strYear = Convert.ToDateTime(txtDate.Text).AddYears(-1).Year.ToString();
        StringBuilder strHTML = new StringBuilder();
        strHTML.Append("   <table width=\"100%\" border=\"1\" cellpadding=\"0\" cellspacing=\"0\" id=\"table1\"> " +
        "                <tr> " +
        "                    <td rowspan=2 style=\"background-color:#C0C0C0;\">&nbsp;</td>" +
        "                    <td colspan=\"4\" align=\"center\" style=\"background-color:#C0C0C0;\">" +
        "                        Contract Expect Received Date" +
        "                    </td>" +
        "                    <td colspan=\"3\" align=\"center\" style=\"background-color:#C0C0C0;\"> " +
        "                        Actual Return Date " +
        "                    </td> " +
        "                </tr> " +
        "                <tr> " +
        "                    <td align=\"center\" style=\"background-color:#C0C0C0;\">Month</td> " +
        "                    <td align=\"center\" style=\"background-color:#C0C0C0;\">YTD</td> " +
        "                    <td align=\"center\" style=\"background-color:#C0C0C0;\">Budget</td> " +
        "                    <td align=\"center\" style=\"background-color:#C0C0C0;\">Standard</td> " +
        "                    <td align=\"center\" style=\"background-color:#C0C0C0;\">Month</td> " +
        "                    <td align=\"center\" style=\"background-color:#C0C0C0;\">YTD</td> " +
        "                    <td align=\"center\" style=\"background-color:#C0C0C0;\">YTD " + strYear +"</td> " +
        "                </tr> ");
        for (int i = 0; i < tb.Rows.Count; i++)
        {
            switch (tb.Rows[i]["Key"].ToString())
            {
                case "AVG PRICE(RMB/PCS)":
                    strHTML.Append(
                        "                <tr> " +
                        "                    <td style=\"background-color:#C0C0C0;\">AVG PRICE(RMB/PCS)</td> "
                                   );
                    break;
                case "AVG SAH(Sewing/garment)":
                    strHTML.Append(
                        "                <tr> " +
                        "                    <td style=\"background-color:#C0C0C0;\">AVG SAH(Sewing/garment)</td> "
                                  );
                    break;
                case "AVG SAH(Total/garment)":
                    strHTML.Append(
                        "                <tr> " +
                        "                    <td style=\"background-color:#C0C0C0;\">AVG SAH(Total/garment)</td> "
                                  );
                    break;
                case "AVG SAH PRICE(RMB/Sewing SAH)":
                    strHTML.Append(
                        "                <tr> " +
                        "                   <td style=\"background-color:#C0C0C0;\">AVG SAH PRICE(RMB/Sewing SAH)</td> "
                                  );
                    break;
            }
            strHTML.AppendFormat(
                            "                    <td align=\"right\">{0}</td> " +
                            "                    <td align=\"right\">{1}</td> " +
                            "                    <td align=\"right\">{2}</td> " +
                            "                    <td align=\"right\">{3}</td> " +
                            "                    <td align=\"right\">{4}</td> " +
                            "                    <td align=\"right\">{5}</td> " +
                            "                    <td align=\"right\">{06}</td> " +
                            "                </tr> ", tb.Rows[i]["f1"].ToString() == "" ? "&nbsp;" : tb.Rows[i]["f1"].ToString(),
                                                        tb.Rows[i]["f2"].ToString() == "" ? "&nbsp;" : tb.Rows[i]["f2"].ToString(),
                                                        tb.Rows[i]["f3"].ToString() == "" ? "&nbsp;" : tb.Rows[i]["f3"].ToString(),
                                                        tb.Rows[i]["f4"].ToString() == "" ? "&nbsp;" : tb.Rows[i]["f4"].ToString(),
                                                        tb.Rows[i]["f5"].ToString() == "" ? "&nbsp;" : tb.Rows[i]["f5"].ToString(),
                                                        tb.Rows[i]["f6"].ToString() == "" ? "&nbsp;" : tb.Rows[i]["f6"].ToString(),
                                                        tb.Rows[i]["f7"].ToString() == "" ? "&nbsp;" : tb.Rows[i]["f7"].ToString()
                            );
        }
        strHTML.AppendFormat("<tr><td colspan=\"8\"> <b>Bal.Qty:</b>{0}(pcs)</td></tr> </table> ", hfBalQty.Value);
        divGV.InnerHtml = strHTML.ToString();

    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        MESComment.Excel.ToExcel(divGV, "Outsourcing Price Monitor " + DateTime.Now.ToString("yyyyMMdd") + ".xls");
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        GetOutTable();
        BuildHtml();
    }

    protected void btnDetailExcel_Click(object sender, EventArgs e)
    {
        DataTable tb= GetDetailTable();
        GridView gvDetail = new GridView();
        gvDetail.ID = "gvDetail";
        gvDetail.DataSource = tb;
        gvDetail.DataBind();
        MESComment.Excel.ToExcel(gvDetail, "Detail Report" + DateTime.Now.ToString("yyyyMMdd") + ".xls");

        gvDetail.Dispose();

    }
    private DataTable GetDetailTable()
    {
        DataTable tb = null;
        tb = MESComment.MesOutSourcePriceSql.GetMasOutSourcePriceDetail(txtDate.Text, ddlView.SelectedValue, chkShirtstop.Checked);
        return tb;
    }
}
