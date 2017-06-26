using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Mes_outsourceCheckingReport : pPage
{
    public string factoryName, subcontractorName, createUserName;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["site"] != null)
        {
            hfValue.Value = Request.QueryString["site"].ToString();
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divDetail.InnerHtml = "";
        DataTable dtHeader = MESComment.MesOutSourcePriceSql.GetOutsourceReportingHead(txtContractNo.Text, "MES");
        foreach (DataRow row in dtHeader.Rows)
        {
            factoryName = row["CHN_NAME"].ToString();
            subcontractorName = row["SUBCONTRACTOR_NAME"].ToString();
            createUserName = row["NAME"].ToString();
        }
        DataTable dtDetail = MESComment.MesOutSourcePriceSql.GetOsChecking(txtContractNo.Text, "", "", "");
        int ttlOutQty = 0;
        int ttlReceivQty = 0;
        int ttladdQty = 0;
        int ttlPullOutQty = 0;
        int ttlInnerReduce = 0;
        int ttlOutLost = 0;

        double ttlCompleteAmount = 0;
        double ttlActualAmount = 0;
        double ttlAdjAmount = 0;
        foreach (DataRow row in dtDetail.Rows)
        {
            int addQty = 0;
            int receiveQty = int.Parse(row["RECEIVEQTY"].ToString() == "" ? "0" : row["RECEIVEQTY"].ToString());
            int outQty = int.Parse(row["OUTQTY"].ToString() == "" ? "0" : row["OUTQTY"].ToString());
            //int pulloutQty = 0;
            int outLostQty = 0;
            double fabPrice = double.Parse(row["FAB_PRICE"].ToString() == "" ? "0" : row["FAB_PRICE"].ToString());
            double adjAmount = double.Parse(row["ADJ_AMOUNT"].ToString() == "" ? "0" : row["ADJ_AMOUNT"].ToString());
            double price = double.Parse(row["OUT_PRICE"].ToString() == "" ? "0" : row["OUT_PRICE"].ToString());

            DataTable dtAeduct = MESComment.MesOutSourcePriceSql.GetPulloutQty(txtContractNo.Text, row["JONO"].ToString(), "A", "");
            if (dtAeduct.Rows.Count > 0)
            {
                addQty = int.Parse(dtAeduct.Rows[0]["QTY"].ToString() == "" ? "0" : dtAeduct.Rows[0]["QTY"].ToString());
            }

            divDetail.InnerHtml += "<tr>";
            divDetail.InnerHtml += "<td>" + row["JONO"] + "</td>";
            divDetail.InnerHtml += "<td>" + row["PROCESS"] + "</td>";
            divDetail.InnerHtml += "<td>" + row["OUTQTY"] + "</td>";
            //divDetail.InnerHtml += "<td>" + (receiveQty + addQty).ToString("#0") + "</td>";
            divDetail.InnerHtml += "<td>" + (receiveQty + addQty).ToString("#0") + "(" + addQty + ")" + "</td>";

            DataTable dtDefect = MESComment.MesOutSourcePriceSql.GetDefectQty(txtContractNo.Text, row["JONO"].ToString());
            if (dtDefect.Rows.Count <= 0)
            {
                DataTable dtDeduct = MESComment.MesOutSourcePriceSql.GetPulloutQtynew(txtContractNo.Text, row["JONO"].ToString(), "D", "");
                if (dtDeduct.Rows.Count > 0)
                {
                    divDetail.InnerHtml += "<td>" + dtDeduct.Rows[0]["QTY"] + "</td>";
                    ttlPullOutQty += int.Parse(dtDeduct.Rows[0]["QTY"].ToString() == "" ? "0" : dtDeduct.Rows[0]["QTY"].ToString());
                }
                else
                {
                    divDetail.InnerHtml += "<td>&nbsp;</td>";
                }
            }
            else
            {
                divDetail.InnerHtml += "<td>" + dtDefect.Rows[0]["QUANTITY"] + "</td>";
                ttlPullOutQty += int.Parse(dtDefect.Rows[0]["QUANTITY"].ToString() == "" ? "0" : dtDefect.Rows[0]["QUANTITY"].ToString());

            }

            DataTable dtLost = MESComment.MesOutSourcePriceSql.GetLostQty(txtContractNo.Text, row["JONO"].ToString(), "");
            if (dtLost.Rows.Count > 0)
            {
                divDetail.InnerHtml += "<td>" + dtLost.Rows[0]["LOSTQTY"] + "</td>";
                outLostQty = int.Parse(dtLost.Rows[0]["LOSTQTY"].ToString() == "" ? "0" : dtLost.Rows[0]["LOSTQTY"].ToString());
            }
            else
            {
                divDetail.InnerHtml += "<td>" + row["OUT_LOST_QTY"] + "</td>";
            }
            DataTable dtCp = MESComment.MesOutSourcePriceSql.GetCPQty(txtContractNo.Text, row["JONO"].ToString(), "");
            if (dtCp.Rows.Count > 0)
            {
                divDetail.InnerHtml += "<td>" + dtCp.Rows[0]["QTY"] + "</td>";
                ttlInnerReduce += int.Parse(dtCp.Rows[0]["QTY"].ToString());
            }
            else
            {
                divDetail.InnerHtml += "<td>" + row["INTERNAL_REDUCE"] + "</td>";
            }
            divDetail.InnerHtml += "<td>￥" + double.Parse(row["OUT_PRICE"].ToString() == "" ? "0" : row["OUT_PRICE"].ToString()).ToString("#0.000") + "</td>";
            divDetail.InnerHtml += "<td>￥" + ((receiveQty + addQty) * price).ToString("#0.00") + "</td>";
            divDetail.InnerHtml += "<td>￥" + double.Parse(row["FAB_PRICE"].ToString()).ToString("#0.000") + "</td>";
            divDetail.InnerHtml += "<td>￥" + row["ADJ_AMOUNT"] + "</td>";
            divDetail.InnerHtml += "<td>￥" + ((receiveQty + addQty) * price - (outLostQty * fabPrice) + adjAmount).ToString("#0.00") + "</td>";
            divDetail.InnerHtml += "<td>" + row["ADJ_REASON"] + "</td>";
            divDetail.InnerHtml += "</tr>";
            ttlOutQty += outQty;
            ttladdQty += addQty;
            ttlReceivQty += receiveQty + addQty;
            ttlActualAmount += ((receiveQty + addQty) * price - outLostQty * fabPrice + adjAmount);
            ttlCompleteAmount += (receiveQty + addQty) * price;
            ttlOutLost += outLostQty;
            ttlAdjAmount += adjAmount;
        }
        divDetail.InnerHtml += "<tr>";
        divDetail.InnerHtml += "<td>计总</td>";
        divDetail.InnerHtml += "<td>---</td>";
        divDetail.InnerHtml += "<td>" + ttlOutQty + "</td>";
        divDetail.InnerHtml += "<td>" + ttlReceivQty + "(" + ttladdQty + ")" + "</td>";
        divDetail.InnerHtml += "<td>" + ttlPullOutQty + "</td>";
        divDetail.InnerHtml += "<td>" + ttlOutLost + "</td>";
        divDetail.InnerHtml += "<td>" + ttlInnerReduce + "</td>";
        divDetail.InnerHtml += "<td>---</td>";
        divDetail.InnerHtml += "<td>￥" + ttlCompleteAmount.ToString("#0.00") + "</td>";
        divDetail.InnerHtml += "<td>---</td>";
        divDetail.InnerHtml += "<td>￥" + ttlAdjAmount.ToString("#0.00") + "</td>";
        divDetail.InnerHtml += "<td>￥" + ttlActualAmount.ToString("#0.00") + "</td>";
        divDetail.InnerHtml += "<td>---</td>";
        divDetail.InnerHtml += "</tr>";
    }
}
