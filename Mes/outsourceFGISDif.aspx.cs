using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Mes_outsourceFGISDif : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string userid = "";
        userid = Request.QueryString["userid"];
        if (!IsPostBack)
        {
            ddlFactoryCd.DataSource = MESComment.MesRpt.GetFactoryList(userid);
            ddlFactoryCd.DataBind();
        }
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                ddlFactoryCd.SelectedValue = Request.QueryString["site"].ToString();
            }
            else
            {
                ddlFactoryCd.SelectedValue = "GEG";
            }
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divBody.InnerHtml = "";
        string JoListStr = "";
        DataTable dt1 = MESComment.MesOutSourcePriceSql.GetOutsourceFGISDif2(txtStartDate.Text,txtEndDate.Text,txtJoNo.Text.Trim(),ddlFactoryCd.SelectedItem.Value);
        DataTable dt2 = MESComment.MesOutSourcePriceSql.GetOutsourceFGISDif1(txtStartDate.Text,txtEndDate.Text,txtJoNo.Text.Trim());
        DataTable ResultList = new DataTable();
        ResultList.Columns.Add("JOB_ORDER_NO");
        ResultList.Columns.Add("OUT_QTY");
        ResultList.Columns.Add("IN_QTY");
        ResultList.Columns.Add("DIF_QTY");
        int totalInQty = 0;
        int totalOutQty = 0;
        int totalDiffQty = 0;
        String joStr = "";
        foreach (DataRow row in dt1.Rows)
        {
            joStr = row["JOB_ORDER_NO"] == null ? "" : row["JOB_ORDER_NO"].ToString();
            JoListStr =JoListStr + joStr + ",";
        }
        foreach (DataRow row in dt2.Rows)
        {
            bool isAdd = true;
            joStr = row["JOB_ORDER_NO"] == null ? "" : row["JOB_ORDER_NO"].ToString();
            for (int i = 0; i < JoListStr.Split(',').Length; i++)
            {
                if (joStr == JoListStr.Split(',')[i])
                {
                    isAdd = false;
                    break;
                }
            }
            if (isAdd)
            {
                JoListStr = JoListStr + joStr + ",";
            }
        }
        for (int i = 0; i < JoListStr.Split(',').Length; i++)
        {
            String jo = JoListStr.Split(',')[i];
            if (jo != "")
            {
                int inQty = 0;
                int outQty = 0;
                int difQty = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    joStr = row["JOB_ORDER_NO"] == null ? "" : row["JOB_ORDER_NO"].ToString();
                    if (joStr.Equals(jo))
                    {
                        inQty = row["QTY"] == null ? 0 : int.Parse(row["QTY"].ToString());
                    }
                }
                foreach (DataRow row in dt2.Rows)
                {
                    joStr = row["JOB_ORDER_NO"] == null ? "" : row["JOB_ORDER_NO"].ToString();
                    if (joStr.Equals(jo))
                    {
                        outQty = row["QTY"] == null ? 0 : int.Parse(row["QTY"].ToString());
                    }
                }
                difQty = inQty - outQty;
                totalInQty = totalInQty + inQty;
                totalOutQty = totalOutQty + outQty;
                DataRow NewRow = ResultList.NewRow();
                NewRow["JOB_ORDER_NO"] = jo;
                NewRow["OUT_QTY"] = outQty;
                NewRow["IN_QTY"] = inQty;
                NewRow["DIF_QTY"] = difQty;
                ResultList.Rows.Add(NewRow);
            }
        }
        totalDiffQty = totalInQty - totalOutQty;
        DataRow TotalRow = ResultList.NewRow();
        TotalRow["JOB_ORDER_NO"] = "Total";
        TotalRow["OUT_QTY"] = totalOutQty;
        TotalRow["IN_QTY"] = totalInQty;
        TotalRow["DIF_QTY"] = totalDiffQty;
        ResultList.Rows.Add(TotalRow);
        foreach (DataRow row in ResultList.Rows)
        {
            divBody.InnerHtml += "<tr>";
            if (row["JOB_ORDER_NO"].ToString() == "Total")
            {
                divBody.InnerHtml += "<td colspan='2'  class='tr2style' bgcolor='#efefe7'>" + row["JOB_ORDER_NO"] + "</td>";
                divBody.InnerHtml += "<td colspan='2'  class='tr2style' bgcolor='#efefe7'>" + row["IN_QTY"] + "</td>";
                divBody.InnerHtml += "<td colspan='2'  class='tr2style' bgcolor='#efefe7'>" + row["OUT_QTY"] + "</td>";
                divBody.InnerHtml += "<td colspan='2'  class='tr2style' bgcolor='#efefe7'>" + row["DIF_QTY"] + "</td>";
            }
            else
            {
                divBody.InnerHtml += "<td colspan='2'>" + row["JOB_ORDER_NO"] + "</td>";
                divBody.InnerHtml += "<td colspan='2'>" + row["IN_QTY"] + "</td>";
                divBody.InnerHtml += "<td colspan='2'>" + row["OUT_QTY"] + "</td>";
                divBody.InnerHtml += "<td colspan='2'>" + row["DIF_QTY"] + "</td>";
            }
            divBody.InnerHtml += "</tr>";
        }
    }
}
