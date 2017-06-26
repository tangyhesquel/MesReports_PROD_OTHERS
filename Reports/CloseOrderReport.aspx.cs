using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_ColseOrder : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlFtyCd.DataSource = MESComment.MesRpt.GetFactoryCd();
            ddlFtyCd.DataTextField = "FACTORY_ID";
            ddlFtyCd.DataValueField = "FACTORY_ID";
            ddlFtyCd.DataBind();

        }
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                ddlFtyCd.SelectedValue = Request.QueryString["site"].ToString();
            }
            else
            {
                ddlFtyCd.SelectedValue = "GEG";
            }
            getproductionlin();
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        string color = "";

        divBody.InnerHtml = "";
        divBody.InnerHtml += "<table border='1' cellspacing='0' cellpadding='0'style='font-size:12px;border-collapse:collapse'>";
        divBody.InnerHtml += "<tr class='tr1style' align='center'>";
        divBody.InnerHtml += "<td>时间</td>";
        divBody.InnerHtml += "<td>制单</td>";
        divBody.InnerHtml += "<td width='50'>制单数</td>";
        divBody.InnerHtml += "<td width='80'>裁数</td>";
        divBody.InnerHtml += "<td width='60'>碎料组欠数</td>";
        divBody.InnerHtml += "<td width='60'>下尾部数</td>";
        divBody.InnerHtml += "<td width='85'>车间抽办</td>";
        divBody.InnerHtml += "<td>B品</td>";
        divBody.InnerHtml += "<td>烂衫</td>";
        divBody.InnerHtml += "<td>车间不见衫</td>";
        divBody.InnerHtml += "<td>欠部位</td>";
        divBody.InnerHtml += "<td>包装数</td>";
        divBody.InnerHtml += "<td>结存数A品</td>";
        divBody.InnerHtml += "<td>尾部抽办</td>";
        divBody.InnerHtml += "<td>尾部不见衫</td>";

        double gmt_qty_a = 0;
        double gmt_qty_b = 0;
        double pack_qty = 0;
        double miss_gmt_qty_sew = 0;
        double miss_gmt_qty_fin = 0;

        DataTable Detail = MesColoseOrder.getcloseorderdata(ddlFtyCd.SelectedItem.Value, txtJoNo.Text.Trim());
        for (int i = 0; i < Detail.Rows.Count; i++)
        {
            if (i % 2 == 0)
            {
                color = "white";
            }
            else
            {
                color = "#f2f2f2";
            }
            DataTable EasnData = MesColoseOrder.geteasndata(ddlFtyCd.SelectedItem.Value, Detail.Rows[i]["制单"].ToString());
            pack_qty = double.Parse(EasnData.Rows[0]["pack_qty"].ToString());

            DataTable FgisData = MesColoseOrder.getfgisdata(ddlFtyCd.SelectedItem.Value, Detail.Rows[i]["制单"].ToString());

            gmt_qty_a = FgisData.Rows[0]["gmt_qty_a"].ToString() == "" ? double.Parse("0.00") : double.Parse(FgisData.Rows[0]["gmt_qty_a"].ToString());

            gmt_qty_b = FgisData.Rows[0]["gmt_qty_b"].ToString() == "" ? double.Parse("0.00") : double.Parse(FgisData.Rows[0]["gmt_qty_b"].ToString());
            miss_gmt_qty_sew = double.Parse(Detail.Rows[i]["车间不见衫"].ToString()) - gmt_qty_b;
            miss_gmt_qty_fin = double.Parse(Detail.Rows[i]["尾部不见衫"].ToString()) - gmt_qty_a - pack_qty;


            divBody.InnerHtml += "<tr bgcolor=" + color + "><td>" + Detail.Rows[i]["时间"] + "</td><td>" + Detail.Rows[i]["制单"] + "</td><td align='right'>" + Detail.Rows[i]["制单数"] + "</td><td align='right'>" + Detail.Rows[i]["裁数"] + "</td>";
            divBody.InnerHtml += "<td align='right'>" + Detail.Rows[i]["碎料组欠数"] + "</td><td align='right'>" + Detail.Rows[i]["下尾部数"] + "</td><td align='center'>" + Detail.Rows[i]["车间抽办"] + "</td><td>" + gmt_qty_b + "</td><td>" + Detail.Rows[i]["烂衫"] + "</td>";
            divBody.InnerHtml += "<td>" + miss_gmt_qty_sew + "</td><td align='right'>" + Detail.Rows[i]["欠部位"] + "</td><td align='center'>" + pack_qty + "</td>";
            divBody.InnerHtml += "<td align='center'>" + gmt_qty_a + "</td><td align='center'>" + Detail.Rows[i]["尾部抽办"] + "</td><td align='center'>" + miss_gmt_qty_fin + "</td></tr>";
        }
        divBody.InnerHtml += "</table>";
    }

    protected void getproductionlin()
    {
        ddlprodLine.DataSource = MESComment.MesRpt.GetPrdLine(ddlFtyCd.SelectedItem.Value, "","");
        ddlprodLine.DataTextField = "PRODUCTION_LINE_NAME";
        ddlprodLine.DataValueField = "PRODUCTION_LINE_CD";
        ddlprodLine.DataBind();
    }

    protected void ddlprocessCd_SelectedIndexChanged(object sender, EventArgs e)
    {
        getproductionlin();
    }
}
