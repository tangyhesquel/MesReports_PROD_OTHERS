using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Mes_outsourceControlReport : pPage
{
    public string name = "";
    public string address = "";
    public string date = "";
    public string rate = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["site"] != "")
        {
            hfValue.Value = Request.QueryString["site"].ToString();
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divIn.InnerHtml = "";
        divOut.InnerHtml = "";
        DataTable dtHeader = MESComment.MesOutSourcePriceSql.GetOutsourceReportingHead(txtContractNo.Text,"MES");
        foreach (DataRow row in dtHeader.Rows)
        {
            name = row["SUBCONTRACTOR_NAME"].ToString();
            address = row["ADDRESS"].ToString();
            date = row["ISSUER_DATE"].ToString();
            rate = row["VALUE_ADD_TAX"].ToString();
        }
        DataTable dtIn = MESComment.MesOutSourcePriceSql.GetSubcontractControlFormIn(txtContractNo.Text);
        foreach (DataRow row in dtIn.Rows)
        {
            divIn.InnerHtml += "<tr>";
            divIn.InnerHtml += "<td>" + row["JOB_ORDER_NO"] + "</td>";
            divIn.InnerHtml += "<td>" + row["PROCESS"] + "</td>";
            divIn.InnerHtml += "<td>" + row["REASON"] + "</td>";
            divIn.InnerHtml += "<td>￥" + row["INTERNAL_PRICE"] + "</td>";
            divIn.InnerHtml += "<td>" + row["PLAN_ISSUE_QTY"] + "</td>";
            divIn.InnerHtml += "<td>" + row["OUTPUT_QTY"] + "</td>";
            divIn.InnerHtml += "<td>￥" + row["OUT_PRICE"] + "</td>";
            DataTable dtDate = MESComment.MesOutSourcePriceSql.GetOutsourceSendDate(row["JOB_ORDER_NO"].ToString(),txtContractNo.Text);
            if (dtDate.Rows.Count > 0)
            {
                divIn.InnerHtml += "<td>" + dtDate.Rows[0]["SEND_DATE"] + "</td>";
            }
            else
            {
                divIn.InnerHtml += "<td>" + row["TRX_DATE"] + "</td>";
            }
            divIn.InnerHtml += "<td>" + row["EXPECT_RECEIVE_DATE"] + "</td>";
            divIn.InnerHtml += "</tr>";
        }


        DataTable dtOut = MESComment.MesOutSourcePriceSql.GetQuerySubcontractControlFormOut(txtContractNo.Text);
        foreach (DataRow row in dtOut.Rows)
        {
            int addQty = 0;
            int receiveQty = int.Parse(row["RECEIVEQTY"].ToString() == "" ? "0" : row["RECEIVEQTY"].ToString());
            int pulloutQty = 0;
            int outLostQty=0;
            double fabPrice = double.Parse(row["FAB_PRICE"].ToString() == "" ? "0" : row["FAB_PRICE"].ToString());
            double adjAmount = double.Parse(row["ADJ_AMOUNT"].ToString() == "" ? "0" : row["ADJ_AMOUNT"].ToString());
            double price = double.Parse(row["OUT_PRICE"].ToString() == "" ? "0" : row["OUT_PRICE"].ToString());

            divOut.InnerHtml += "<tr>";
            divOut.InnerHtml += "<td>" + row["JONO"] + "</td>";
            DataTable dtOutDate = MESComment.MesOutSourcePriceSql.GetOutsourceReceiveDate(row["JONO"].ToString(),txtContractNo.Text);
            if (dtOutDate.Rows.Count > 0)
            {
                divOut.InnerHtml += "<td>" + dtOutDate.Rows[0]["RECEIVE_DATE"] + "</td>";
            }
            else
            {
                divOut.InnerHtml += "<td>" + row["TRX_DATE"] + "</td>";
            }

            divOut.InnerHtml += "<td>" + (receiveQty + addQty + pulloutQty).ToString("#0") + "</td>";
            DataTable dtDefect = MESComment.MesOutSourcePriceSql.GetDefectQty(txtContractNo.Text, row["JONO"].ToString());
            if (dtDefect.Rows.Count <= 0)
            {
                DataTable dtDeduct = MESComment.MesOutSourcePriceSql.GetPulloutQty(txtContractNo.Text, row["JONO"].ToString(), "D", "");
                if (dtDefect.Rows.Count > 0)
                {
                    divOut.InnerHtml += "<td>" + dtDefect.Rows[0]["QTY"] + "</td>";
                    addQty = int.Parse(dtDefect.Rows[0]["QTY"].ToString() == "" ? "0" : dtDefect.Rows[0]["QTY"].ToString());
                }
                else
                {
                    divOut.InnerHtml += "<td>&nbsp;</td>";
                }
            }
            else
            {
                divOut.InnerHtml += "<td>" + dtDefect.Rows[0]["QUANTITY"] + "</td>";
            }
            DataTable dtLost = MESComment.MesOutSourcePriceSql.GetLostQty(txtContractNo.Text, row["JONO"].ToString(),"");
            if (dtLost.Rows.Count > 0)
            {
                divOut.InnerHtml += "<td>" + dtLost.Rows[0]["LOSTQTY"] + "</td>";
                outLostQty = int.Parse(dtLost.Rows[0]["LOSTQTY"].ToString() == "" ? "0" : dtLost.Rows[0]["LOSTQTY"].ToString());
            }
            else
            {
                divOut.InnerHtml += "<td>" + row["OUT_LOST_QTY"] + "</td>";
            }
            DataTable dtCp = MESComment.MesOutSourcePriceSql.GetCPQty(txtContractNo.Text, row["JONO"].ToString(), "");
            if (dtCp.Rows.Count > 0)
            {
                divOut.InnerHtml += "<td>" + dtCp.Rows[0]["QTY"] + "</td>";
                pulloutQty = int.Parse(dtCp.Rows[0]["QTY"].ToString() == "" ? "0" : dtCp.Rows[0]["QTY"].ToString());
            }
            else
            {
                divOut.InnerHtml += "<td>" + row["INTERNAL_REDUCE"] + "</td>";
            }
            divOut.InnerHtml += "<td>￥" + double.Parse(row["OUT_PRICE"].ToString() == "" ? "0" : row["OUT_PRICE"].ToString()).ToString("#0.000") + "</td>";
            divOut.InnerHtml += "<td>￥" + double.Parse(row["FAB_PRICE"].ToString()).ToString("#0.000") + "</td>";
            divOut.InnerHtml += "<td>￥" + ((receiveQty + addQty) * price - (outLostQty * fabPrice) + adjAmount).ToString("#0.00") + "</td>";
            divOut.InnerHtml += "<td>" + row["ADJ_REASON"] + "</td>";
            divOut.InnerHtml += "</tr>";
        }
    }
}
