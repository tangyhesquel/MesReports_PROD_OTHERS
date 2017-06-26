using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;

public partial class Mes_wipMonthly : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlFactoryCd.DataSource = MESComment.MesRpt.GetFactoryCd();
            ddlFactoryCd.DataBind();
            if (Request.QueryString["site"] != null)
            {
                ddlFactoryCd.SelectedValue = Request.QueryString["site"].ToString();
            }
            DateTime beginTime = DateTime.Now.AddYears(-10);
            for (int i = 0; i < 20; i++)
            {
                ddlYear.Items.Add(beginTime.AddYears(i + 1).Year.ToString());
            }
            ddlYear.SelectedValue = DateTime.Now.Year.ToString();
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
        divTitle1.InnerHtml = "";
        divTitle2.InnerHtml = "";
        divDetail.InnerHtml = "";

        DataTable dtTitle = MESComment.wipMonthlySql.GetMonthlyTitle(ddlMonth.SelectedItem.Value, ddlYear.SelectedItem.Value, ddlFactoryCd.SelectedItem.Value);
        DataTable dtResult = MESComment.wipMonthlySql.GetMonthly(ddlFactoryCd.SelectedItem.Value, ddlMonth.SelectedItem.Value, ddlYear.SelectedItem.Value, ddlGarmentType.SelectedItem.Value, ddlOrderType.SelectedItem.Value, ddlWashType.SelectedItem.Value);

        DataTable dtDetail = new DataTable();
        foreach (DataColumn column in dtResult.Columns)
        {
            if (column.ColumnName != "PROCESS_CD")
            {
                DataColumn column1 = new DataColumn();
                column1.ColumnName = column.ColumnName;
                dtDetail.Columns.Add(column1);
            }
        }
        foreach (DataRow row in dtTitle.Rows)
        {
            divTitle1.InnerHtml += "<td class='tr2style'>" + row["PROCESS_CD"] + "</td>";
            divTitle2.InnerHtml += "<td class='tr2style'>Out Qty </td>";
            dtDetail.Columns.Add(row["PROCESS_CD"].ToString());
        }
        string JoNo = "";
        int j = -1;
        foreach (DataRow row in dtResult.Rows)
        {
            if (JoNo != row["JO_NO"].ToString())
            {
                DataRow newRow = dtDetail.NewRow();
                newRow["BUYER"] = row["BUYER"];
                newRow["SC_NO"] = row["SC_NO"];
                newRow["JO_NO"] = row["JO_NO"];
                newRow["Garment_Type"] = row["Garment_Type"];
                newRow["Wash_Type"] = row["Wash_Type"];
                newRow["STYLE_NO"] = row["STYLE_NO"];
                newRow["GMT_DEL_DATE"] = row["GMT_DEL_DATE"];
                newRow["prd_cmp_dt"] = row["prd_cmp_dt"];
                newRow["ORDER_QTY"] = row["ORDER_QTY"];//新添加ORDER_QTY；
                newRow["SAH"] = row["SAH"];//新添加SAH；
                newRow[row["PROCESS_CD"].ToString()] = row["OUTPUT_QTY"];
                dtDetail.Rows.Add(newRow);
                JoNo = row["JO_NO"].ToString();
                j++;
            }
            else
            {
                dtDetail.Rows[j][row["PROCESS_CD"].ToString()] = row["OUTPUT_QTY"];
            }
        }
        double[] Total = new double[dtTitle.Rows.Count];
        double Total_order = 0.00;
        for (int i = 0; i < dtDetail.Rows.Count; i++)
        {
            if (i % 2 == 0)
            {
                divDetail.InnerHtml += "<tr bgcolor='white'>";
            }
            else
            {
                divDetail.InnerHtml += "<tr bgcolor='#f2f2f2'>";
            }
            divDetail.InnerHtml += "<td >" + dtDetail.Rows[i]["BUYER"] + "</td>";
            divDetail.InnerHtml += "<td >" + dtDetail.Rows[i]["SC_NO"] + "</td>";
            divDetail.InnerHtml += "<td align='right'>" + dtDetail.Rows[i]["JO_NO"] + "</td>";
            divDetail.InnerHtml += "<td align='center'>" + dtDetail.Rows[i]["Garment_Type"] + "</td>";
            divDetail.InnerHtml += "<td align='center'>" + dtDetail.Rows[i]["Wash_Type"] + "</td>";
            divDetail.InnerHtml += "<td >" + dtDetail.Rows[i]["STYLE_NO"] + "</td>";
            divDetail.InnerHtml += "<td >" + dtDetail.Rows[i]["GMT_DEL_DATE"] + "</td>";
            divDetail.InnerHtml += "<td >" + Convert.ToDateTime(dtDetail.Rows[i]["prd_cmp_dt"].ToString()).ToString("d") + "</td>";
            divDetail.InnerHtml += "<td >" + dtDetail.Rows[i]["ORDER_QTY"] + "</td>";//新添加ORDER_QTY；            
            divDetail.InnerHtml += "<td >" + dtDetail.Rows[i]["SAH"] + "</td>";//新添加SAH；
            
            Total_order += Double.Parse(dtDetail.Rows[i]["ORDER_QTY"].ToString());
            foreach (DataRow row in dtTitle.Rows)
            {
                divDetail.InnerHtml += "<td>" + dtDetail.Rows[i][row["PROCESS_CD"].ToString()] + "</td>";
            }
            for (int k = 0; k < dtTitle.Rows.Count; k++)
            {
                Total[k] += double.Parse(dtDetail.Rows[i][11 + k].ToString() == "" ? "0" : dtDetail.Rows[i][11 + k].ToString());
            }
            divDetail.InnerHtml += "</tr>";
        }
        divDetail.InnerHtml += "<tr class='tr2style'><td colspan='8'>Total</td>";
        divDetail.InnerHtml += "<td>" + Total_order.ToString("#,###.###") + "</td><td></td>";
        for (int i = 0; i < dtTitle.Rows.Count; i++)
        {
            divDetail.InnerHtml += "<td>" + Total[i].ToString("#,###.###") + "</td>";
        }
        divDetail.InnerHtml += "</tr>";
    }
}
