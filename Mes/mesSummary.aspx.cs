using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Mes_mesSummary : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string moNo = Request.QueryString["moNo"];
            if (moNo != "" && moNo != null)
            {
                this.txtMoNo.Text = moNo;
                btnQuery_Click(this.btnQuery, null);
            }
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divHeader.InnerHtml = "";
        divColor.InnerHtml = "";
        divDetail.InnerHtml = "";
        DataTable dtGoHeader = MESComment.MesOutSourcePriceSql.GetGoHeaderInfo(txtMoNo.Text);
        foreach (DataRow row in dtGoHeader.Rows)
        {
            divHeader.InnerHtml += "<tr>";
            divHeader.InnerHtml += "<td>" + row["GO_NO"] + "</td>";
            divHeader.InnerHtml += "<td>" + row["PART_TYPE"] + "</td>";
            divHeader.InnerHtml += "<td>" + row["ORDER_QTY"] + "</td>";
            divHeader.InnerHtml += "<td>" + row["ACTUAL_QTY"] + "</td>";
            divHeader.InnerHtml += "<td>" + row["ADJUST_QTY"] + "</td>";
            divHeader.InnerHtml += "<td>" + row["PERCENT"] + "%</td>";
            divHeader.InnerHtml += "</tr>";
        }
        DataTable dtFabric = MESComment.MesOutSourcePriceSql.GetGoFabricInfo(txtMoNo.Text);
        foreach (DataRow row in dtFabric.Rows)
        {
            divColor.InnerHtml += "<tr>";
            divColor.InnerHtml += "<td>" + row["Color_Name"] + "</td>";
            divColor.InnerHtml += "<td>" + row["PPO_NO"] + "</td>";
            divColor.InnerHtml += "<td>" + row["RECEIVED_QTY"] + "</td>";
            divColor.InnerHtml += "<td>" + row["SPARE_FABRIC_QTY"] + "</td>";
            divColor.InnerHtml += "<td>" + row["BINDING_FABRIC_QTY"] + "</td>";
            divColor.InnerHtml += "<td>" + row["BULK_FABRIC_QTY"] + "</td>";
            divColor.InnerHtml += "<td>" + row["BALANCE_FABRIC_QTY"] + "</td>";
            divColor.InnerHtml += "</tr>";
        }
        DataTable dtJoHeader = MESComment.MesOutSourcePriceSql.GetJoHeaderInfo(txtMoNo.Text);
        foreach (DataRow row in dtJoHeader.Rows)
        {
            divDetail.InnerHtml += "<tr><td>";
            divDetail.InnerHtml += "<table width='100%' border='1' cellspacing='0' cellpadding='0' style='font-size:12px;border-collapse:collapse'>";
            divDetail.InnerHtml += "<tr>";
            divDetail.InnerHtml += "<td class='tr2style' width= '130'>JO NO:</td>";
            divDetail.InnerHtml += "<td class='tr2style' width= '130'>JO Qty:</td>";
            divDetail.InnerHtml += "<td class='tr2style' width= '130'>JO Plan Cut Qty:</td>";
            divDetail.InnerHtml += "<td class='tr2style' width= '130'>JO Over/Short Plan Cut Qty:</td>";
            divDetail.InnerHtml += "<td class='tr2style' width= '130'>Percentage</td>";
            divDetail.InnerHtml += "</tr>";
            divDetail.InnerHtml += "<tr>";
            divDetail.InnerHtml += "<td>" + row["JO_NO"] + "</td>";
            divDetail.InnerHtml += "<td>" + row["ORDER_QTY"] + "</td>";
            divDetail.InnerHtml += "<td>" + row["ACTUAL_QTY"] + "</td>";
            divDetail.InnerHtml += "<td>" + row["ADJUST_QTY"] + "</td>";
            divDetail.InnerHtml += "<td>" + row["PERCENT"] + "</td>";
            divDetail.InnerHtml += "</tr></table></td></tr>";
            divDetail.InnerHtml += "<tr><td>&nbsp;</td></tr><tr><td>";
            String colorCode = "";
            double joQtyTotal = 0;
            double cutQtyTotal = 0;
            double sampleQtyTotal = 0;
            double adjustTotal = 0;
            decimal ypd = new decimal(0);
            String totalFlag = "X";
            DataTable dtJoDetail;

            //<Added by:ZouShCh at 2013-03-19 Screen Start> EGM CutPlan 需求变更后
            //if (this.Page.Request.QueryString.Get("site").ToString().ToUpper() != "DEV" && this.Page.Request.QueryString.Get("site").ToString().ToUpper() != "EGM")
            //{
            
            
                dtJoDetail = MESComment.MesOutSourcePriceSql.GetJoDetailInfo(txtMoNo.Text.Trim());
            
            //}
            //else
            //{
            //    dtJoDetail = MESComment.MesOutSourcePriceSql.GetJoDetailInfoCutPlan(txtMoNo.Text.Trim());
            //}
            //<Added by:ZouShCh at 2013-03-19 Screen End>
            
            foreach (DataRow row1 in dtJoDetail.Rows)
            {
                decimal joQty = decimal.Parse(row1["ORDER_QTY"].ToString());
                decimal cutQty = decimal.Parse(row1["ACTUAL_QTY"].ToString());
                decimal adjustQty = decimal.Parse(row1["ADJUST_QTY"].ToString());
                decimal sampleQty = decimal.Parse(row1["SAMPLE_QTY"].ToString());
                if (row1["JO_NO"].ToString().Equals(row["JO_NO"].ToString()) && !row1["COLOR_CD"].ToString().Equals(colorCode) && "Y".Equals(totalFlag))
                {
                    divDetail.InnerHtml += "<tr>";
                    divDetail.InnerHtml += "<td width='100' >&nbsp;</td>";
                    divDetail.InnerHtml += "<td ><strong>YPD:</strong></td>";
                    divDetail.InnerHtml += "<td>"+ypd+"</td>";
                    divDetail.InnerHtml += "<td ><strong>Fabric Req:</strong></td>";
                    divDetail.InnerHtml += "<td >"+(cutQtyTotal/12.0*double.Parse(ypd.ToString())).ToString("f2") +"</td>";
                    divDetail.InnerHtml += "<td><strong>Sub Total:</strong></td>";
                    divDetail.InnerHtml += "<td width='100'>&nbsp;</td>";
                    divDetail.InnerHtml += "<td>"+joQtyTotal+"</td>";
                    divDetail.InnerHtml += "<td>"+sampleQtyTotal+"</td>";
                    divDetail.InnerHtml += "<td>"+cutQtyTotal+"</td>";
                    divDetail.InnerHtml += "<td>"+adjustTotal+"</td>";
                    divDetail.InnerHtml += "<td>" + (adjustTotal / joQtyTotal * 100).ToString("f2") + "</td>";
                    divDetail.InnerHtml+="</tr></table></td></tr>";
                    divDetail.InnerHtml += "<tr><td>&nbsp;</td></tr>";
                }
                if (row1["JO_NO"].ToString().Equals(row["JO_NO"].ToString()) && !row1["COLOR_CD"].ToString().Equals(colorCode))
                {
                    colorCode = row1["COLOR_CD"].ToString();
                    totalFlag = "Y";
                    ypd = decimal.Parse(row1["YPD"].ToString());
                    joQtyTotal = 0;
                    cutQtyTotal = 0;
                    adjustTotal = 0;
                    sampleQtyTotal = 0;
                    divDetail.InnerHtml += "<table width='100%' border='1' cellspacing='0' cellpadding='0' style='font-size:12px;border-collapse:collapse'>";
                    divDetail.InnerHtml += "<tr>";
                    divDetail.InnerHtml += "<td class='tr2style' >Color Code</td>";
                    divDetail.InnerHtml += "<td class='tr2style' colspan='4' >Color Desc</td>";
                    divDetail.InnerHtml += "<td class='tr2style' >Pattern</td>";
                    divDetail.InnerHtml += "<td class='tr2style' >Size</td>";
                    divDetail.InnerHtml += "<td class='tr2style' >JO Qty</td>";
                    divDetail.InnerHtml += "<td class='tr2style' >Sample Qty</td>";
                    divDetail.InnerHtml += "<td class='tr2style' >Plan Cut Qty</td>";
                    divDetail.InnerHtml += "<td class='tr2style' >Over/Short Plan Cut Qty</td>";
                    divDetail.InnerHtml += "<td class='tr2style' >Over/Short Plan Cut %</td>";
                    divDetail.InnerHtml += "</tr>";
                }
                if (row1["JO_NO"].ToString().Equals(row["JO_NO"].ToString()) && row1["COLOR_CD"].ToString().Equals(colorCode))
                {
                    joQtyTotal = joQtyTotal + double.Parse(joQty.ToString());
                    cutQtyTotal = cutQtyTotal + double.Parse(cutQty.ToString());
                    adjustTotal = adjustTotal + double.Parse(adjustQty.ToString());
                    sampleQtyTotal = sampleQtyTotal + double.Parse(sampleQty.ToString());
                    divDetail.InnerHtml += "<tr>";
                    divDetail.InnerHtml += "<td width='100'>" + row1["COLOR_CD"] + "</td>";
                    divDetail.InnerHtml += "<td width='100' colspan='4'>" + row1["COLOR_DESC"] + "</td>";
                    divDetail.InnerHtml += "<td width='100'>" + row1["PATTERN_TYPE_DESC"] + "</td>";
                    divDetail.InnerHtml += "<td width='100'>" + row1["SIZE_CD"] + "</td>";
                    divDetail.InnerHtml += "<td width='100'>" + row1["ORDER_QTY"] + "</td>";
                    divDetail.InnerHtml += "<td width='100'>" + row1["SAMPLE_QTY"] + "</td>";
                    divDetail.InnerHtml += "<td width='100'>" + row1["ACTUAL_QTY"] + "</td>";
                    divDetail.InnerHtml += "<td width='200'>" + row1["ADJUST_QTY"] + "</td>";
                    divDetail.InnerHtml += "<td>" + row1["PERCENT"] + "</td>";
                    divDetail.InnerHtml += "</tr>";
                }
            }
            divDetail.InnerHtml += "<tr>";
            divDetail.InnerHtml += "<td width='100' >&nbsp;</td>";
            divDetail.InnerHtml += "<td ><strong>YPD:</strong></td>";
            divDetail.InnerHtml += "<td>" + ypd + "</td>";
            divDetail.InnerHtml += "<td ><strong>Fabric Req:</strong></td>";
            divDetail.InnerHtml += "<td >" + (cutQtyTotal / 12.0 * double.Parse(ypd.ToString())).ToString("f2") + "</td>";
            divDetail.InnerHtml += "<td><strong>Sub Total:</strong></td>";
            divDetail.InnerHtml += "<td width='100'>&nbsp;</td>";
            divDetail.InnerHtml += "<td>" + joQtyTotal + "</td>";
            divDetail.InnerHtml += "<td>" + sampleQtyTotal + "</td>";
            divDetail.InnerHtml += "<td>" + cutQtyTotal + "</td>";
            divDetail.InnerHtml += "<td>" + adjustTotal + "</td>";
            divDetail.InnerHtml += "<td>" + (adjustTotal / joQtyTotal * 100).ToString("f2") + "</td>";
            divDetail.InnerHtml += "</tr></table></td></tr>";
            divDetail.InnerHtml += "<tr><td>&nbsp;</td></tr>";
            divDetail.InnerHtml += "<tr><td>";
            divDetail.InnerHtml += "<table width='100%' border='1' cellspacing='0' cellpadding='0' style='font-size:12px;border-collapse:collapse'>";
            divDetail.InnerHtml += "<tr>";
            divDetail.InnerHtml += "<td width='180'>&nbsp;</td>";
            divDetail.InnerHtml += "<td width='100'><strong>Grand Total:</strong></td>";
            divDetail.InnerHtml += "<td width='100'>&nbsp;</td>";
            divDetail.InnerHtml += "<td width='100'>&nbsp;</td>";
            divDetail.InnerHtml += "<td width='100'>" + row["ORDER_QTY"] + "</td>";
            divDetail.InnerHtml += "<td width='100'>" + row["TOTAL_SAMPLE_QTY"] + "</td>";
            divDetail.InnerHtml += "<td width='100'>" + row["ACTUAL_QTY"] + "</td>";
            divDetail.InnerHtml += "<td width='100'>" + row["ADJUST_QTY"] + "</td>";
            divDetail.InnerHtml += "<td width='100'>" + row["PERCENT"] + "</td>";
            divDetail.InnerHtml += "</tr></table></td></tr>";
            divDetail.InnerHtml += "<tr><td>&nbsp;</td></tr>";
        }
    }
}
