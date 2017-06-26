using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_FabricUnitPriceQuery :pPage
{
    public string joNo, scNo, exRate;
    protected void Page_Load(object sender, EventArgs e)
    {
        joNo = Request.QueryString["joNo"];
        scNo = Request.QueryString["scNo"];
        exRate = Request.QueryString["exRate"];
        GenerateTables();

    }

    private void GenerateTables()
    {
        table1.InnerHtml = "";
        table2.InnerHtml = "";
        DataTable dt1 = MESComment.MesRpt.GetFabricUnitPriceQuery1(scNo);
        table1.InnerHtml += "<table style='border-collapse: collapse' bordercolor='#999999' cellspacing='0' cellpadding='0' width='100%' border='1'>";
        table1.InnerHtml += "  <tr>";
        table1.InnerHtml += "      <td>GO# </td>";
        table1.InnerHtml += "      <td colspan='9'> <%=joNo%></td>";
        table1.InnerHtml += "  </tr>";
        table1.InnerHtml += "  <tr>";
        table1.InnerHtml += "      <td colspan='10'>布价按部位/PPO汇总</td>";
        table1.InnerHtml += "  </tr>";
        table1.InnerHtml += "  <tr>";
        table1.InnerHtml += "      <td rowspan='2'>部位</td>";
        table1.InnerHtml += "      <td rowspan='2'>PPO</td>";
        table1.InnerHtml += "      <td rowspan='2'>币种</td>";
        table1.InnerHtml += "      <td colspan='2'>布价</td>";
        table1.InnerHtml += "      <td rowspan='2'>数量</td>";
        table1.InnerHtml += "      <td rowspan='2'>YPD</td>";
        table1.InnerHtml += "      <td rowspan='2'>（用布信息）备注</td>";
        table1.InnerHtml += "      <td colspan='2'>成衣布价</td>";
        table1.InnerHtml += "  </tr>";
        table1.InnerHtml += "  <tr>";
        table1.InnerHtml += "      <td>外币</td>";
        table1.InnerHtml += "      <td>RMB</td>";
        table1.InnerHtml += "      <td>(USD/PC)</td>";
        table1.InnerHtml += "      <td>(RMB/pc)</td>";
        table1.InnerHtml += "  </tr>";
        for (int i = 0; i < dt1.Rows.Count; i++)
        {
            table1.InnerHtml += "<tr><td>" + dt1.Rows[i]["FABRIC_TYPE_DESC_CHN"] + "</td><td>" + dt1.Rows[i]["PPO_NO"] + "</td><td>" + dt1.Rows[i]["CCY_CD"] + "</td><td>" + dt1.Rows[i]["UNITPRICE"] + "</td><td>" + double.Parse(dt1.Rows[i]["UNITPRICE"].ToString() == "" ? "0" : dt1.Rows[i]["UNITPRICE"].ToString()) * double.Parse(exRate) + "</td><td>&nbsp;</td><td>" + dt1.Rows[i]["PPO_YPD"] + "</td><td>" + dt1.Rows[i]["REMARKS"] + "</td><td>" + dt1.Rows[i]["UNITPRICE2"] + "</td><td>" + double.Parse(dt1.Rows[i]["UNITPRICE2"].ToString() == "" ? "0" : dt1.Rows[i]["UNITPRICE2"].ToString()) * double.Parse(exRate) + "</td></tr>";
        }
        table1.InnerHtml += "</table>";

        DataTable dt2 = MESComment.MesRpt.GetFabricUnitPriceQuery2(scNo);

        table2.InnerHtml += "<table style='BORDER-COLLAPSE: collapse' bordercolor='#999999' cellspacing='0' cellpadding='0' width='100%' border='1'>";
        table2.InnerHtml += "<tr >";
        table2.InnerHtml += "<td colspan='4'>布价按部位汇总</td>";
        table2.InnerHtml += "</tr>";
        table2.InnerHtml += "<tr >";
        table2.InnerHtml += "<td rowspan='2' >部位</td>";
        table2.InnerHtml += "<td rowspan='2'>币种</td>";
        table2.InnerHtml += "<td colspan='2'>布价</td>";
        table2.InnerHtml += "</tr>";
        table2.InnerHtml += "<tr >";
        table2.InnerHtml += "<td>(USD/PC)</td>";
        table2.InnerHtml += "<td >(RMB/pc)</td>";
        table2.InnerHtml += "</tr>";
        double ttPrice = 0;
        double ttPrice1 = 0;
        for (int i = 0; i < dt2.Rows.Count; i++)
        {
            table2.InnerHtml += "<tr><td>" + dt2.Rows[i]["FABRIC_TYPE_DESC_CHN"] + "</td><td>" + dt2.Rows[i]["CCY_CD"] + "</td><td>" + dt2.Rows[i]["UNITPRICE"].ToString() + "</td><td>" + double.Parse(dt2.Rows[i]["UNITPRICE"].ToString() == "" ? "0" : dt2.Rows[i]["UNITPRICE"].ToString()) * double.Parse(exRate) + "</td></tr>";
            ttPrice += double.Parse(dt2.Rows[i]["UNITPRICE"].ToString() == "" ? "0" : dt2.Rows[i]["UNITPRICE"].ToString());
            ttPrice1 += double.Parse(dt2.Rows[i]["UNITPRICE"].ToString() == "" ? "0" : dt2.Rows[i]["UNITPRICE"].ToString()) * double.Parse(exRate);
        }
        table2.InnerHtml += "<tr>";
        table2.InnerHtml += " <td colspan='2'>计总：</td>";
        table2.InnerHtml += " <td>"+ttPrice+"</td>";
        table2.InnerHtml += " <td>"+ttPrice1+"</td>";
        table2.InnerHtml += "</tr>";
        table2.InnerHtml += "</table>";
    }
}
