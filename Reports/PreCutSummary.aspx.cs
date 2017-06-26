using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Threading;

public partial class Reports_PreCutSummary : pPage
{
    string JONO = "";
    string FactoryCD = "";
    public string factoryName = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Request.QueryString.Get("SITE") != null)
        {
            this.FactoryCD = this.Request.QueryString.Get("SITE").ToString().ToUpper();
        }
        if (this.Request.QueryString.Get("JONO") != null)
        {
            this.JONO = this.Request.QueryString.Get("JONO").ToString().ToUpper();
        }
        if (!this.Page.IsPostBack)
        {
        }
        if (!JONO.Equals(""))
        {
            SetQuery();
        }

    }

    private void SetQuery()
    {
        div_Header_Summary.InnerHtml = "";
        string PONO = JONO;
        double Summary_Cut = 0;
        double Summary_Order = 0;
        DataTable tmpReportContentList = MESComment.PreCutSummarySql.GetCutAndShipmentReportHeader(PONO);
        DataTable cutQtyPerColorSizeList = MESComment.PreCutSummarySql.GetJOColorSizeBreakdown(JONO);
        DataTable cutDetailPerColorSize = MESComment.PreCutSummarySql.GetCutAndShipmentReportDetail(PONO);
        DataTable cutDateMap = MESComment.PreCutSummarySql.GetJobNoHeadInfo(JONO);
        DataTable summaryMap = MESComment.PreCutSummarySql.GetCutAndShipmentReportSummary(JONO);
        DataTable factoryMap = MESComment.PreCutSummarySql.GetCutAndShipmentReportFactoryName(FactoryCD);

        Merge(cutDetailPerColorSize, cutQtyPerColorSizeList);

        for (int i = 0; i < cutQtyPerColorSizeList.Rows.Count; i++)
        {
            Summary_Cut += Double.Parse(cutQtyPerColorSizeList.Rows[i]["CUTQTY"].ToString());
            Summary_Order += Double.Parse(cutQtyPerColorSizeList.Rows[i]["ORDER_QTY"].ToString());
        }

        if (factoryMap.Rows.Count > 0)
        {
            if (factoryMap.Rows[0]["ENG_NAME"] != null)
            {
                factoryName = factoryMap.Rows[0]["ENG_NAME"].ToString();
                if (factoryName.IndexOf("-") != -1)
                {
                    factoryName = factoryName.Substring(factoryName.IndexOf("-") + 1);
                }
            }
        }

        if (summaryMap.Rows.Count > 0)
        {//header info;
            div_Header_Summary.InnerHtml += "<table width='87%' style='border-collapse: collapse; font-size: 12px;' border='1' cellspacing='0' cellpadding='0'>";
            div_Header_Summary.InnerHtml += "<tr><td class='style3' width='180'>JOB NO.</td><td class='style2'>" + summaryMap.Rows[0]["JO_NO"].ToString() + "</td>";
            div_Header_Summary.InnerHtml += "<td class='style3' width='180'>Buyer</td><td class='style2'>" + summaryMap.Rows[0]["SHORT_NAME"].ToString() + "</td></tr>";
            div_Header_Summary.InnerHtml += "<tr><td class='style3' width='180'>Total Cut Qty</td><td class='style2'>" + summaryMap.Rows[0]["CUT_QTY"].ToString() + "</td>";
            div_Header_Summary.InnerHtml += "<td class='style3' width='180'>Total Order Qty</td><td class='style2'>" + summaryMap.Rows[0]["ORDER_QTY"].ToString() + "</td></tr></table>";
            div_Header_Summary.InnerHtml += "<br/>";
        }
        div_Header_info.InnerHtml = "";
        if (tmpReportContentList.Rows.Count > 0)
        {
            div_Header_info.InnerHtml += "<table width='87%' style='border-collapse: collapse; font-size: 12px;' border='1' cellspacing='0' cellpadding='0'>";
            div_Header_info.InnerHtml += "<tr><td class='style3'>JOB NO.</td><td class='style2'>" + tmpReportContentList.Rows[0]["JOBNO"].ToString() + "</td>";
            div_Header_info.InnerHtml += "<td class='style3'>Buyer</td><td class='style2'>" + tmpReportContentList.Rows[0]["BUYER"].ToString() + "</td>";
            div_Header_info.InnerHtml += "<td class='style3'>PO#</td><td class='style2'>" + tmpReportContentList.Rows[0]["PONODESC"].ToString() + "</td>";
            div_Header_info.InnerHtml += "<td class='style3'>Style No :</td><td class='style2'>" + tmpReportContentList.Rows[0]["STYLENO"].ToString() + "</td></tr>";
            div_Header_info.InnerHtml += "<tr><td class='style3'>Cut Qty</td><td class='style2'>" + Summary_Cut + "</td>";
            div_Header_info.InnerHtml += "<td class='style3'>Order Qty</td><td class='style2'>" + Summary_Order + "</td>";
            div_Header_info.InnerHtml += "<td class='style3'>Over/Shipment</td><td class='style2'>" + tmpReportContentList.Rows[0]["OVERPERCENT"].ToString() + "%/~" + tmpReportContentList.Rows[0]["SHORTPERCENT"].ToString() + "%</td>";
            div_Header_info.InnerHtml += "<td class='style3'>Destination</td><td class='style2'></td></tr>";
        }

        if (cutDateMap.Rows.Count > 0)
        {
            div_Header_info.InnerHtml += "<td class='style3'>Cut Date :</td><td class='style2'>" + cutDateMap.Rows[0]["CUTDATE"].ToString() + "</td>";
        }
        else
        {
            div_Header_info.InnerHtml += "<td class='style3'>Cut Date :</td><td class='style2'></td>";
        }

        double percent = Summary_Cut / Summary_Order * 100;
        div_Header_info.InnerHtml += "<td class='style3'>Shipment/Order Qty</td><td class='style2'></td><td colspan='2' class='style2'></td>";
        div_Header_info.InnerHtml += "<td class='style3'>PSA Qty</td><td class='style2'></td></tr>";
        div_Header_info.InnerHtml += "<tr><td class='style3'>Shipment Date :</td><td class='style2'></td><td class='style3'>Cut/Order Qty</td><td class='style2'>" + percent.ToString("##0.00") + "%</td><td colspan='2' class='style2'></td>";
        div_Header_info.InnerHtml += "<td class='style3'>Shipment Qty</td><td class='style2'></td></tr></table>";
        div_Header_info.InnerHtml += "<br/>";

        Show_Color_Size_Qty(cutDetailPerColorSize, cutQtyPerColorSizeList);

    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        SetQuery();
    }

    protected void Show_Color_Size_Qty(DataTable cutDetailPerColorSize, DataTable cutQtyPerColorSizeList)
    {
        div_detail.InnerHtml = "";

        DataTable Colors = cutQtyPerColorSizeList.DefaultView.ToTable(true, new string[] { "COLOR_CD", "CORLORDESC" });
        foreach (DataRow color in Colors.Rows)
        {

            string SQL = "COLOR_CD = '" + color["COLOR_CD"] + "'";
            DataRow[] Sizes = cutQtyPerColorSizeList.Select(SQL);
            int Size_Count = Sizes.Length + 2;
            int Columns_Count = 6;
            string[,] data = new string[Columns_Count, Size_Count];
            data[0, 0] = "Size";
            data[1, 0] = "Order Qty";
            data[2, 0] = "Cut Qty";
            data[3, 0] = "Over/Short Cut%";
            data[4, 0] = "Over/Short Shipment%";
            data[5, 0] = "Left Garment In Fty";
            div_detail.InnerHtml += "<table width='87%' style='border-collapse: collapse; font-size: 12px;' border='1' cellspacing='0' cellpadding='0'>";
            div_detail.InnerHtml += "<tr><td class='style4' width='180'>Color :</td><td colspan='" + (Size_Count - 1).ToString() + "' class='style2'>" + color["CORLORDESC"].ToString() + "</td>";
            for (int i = 0; i < Columns_Count; i++)
            {
                for (int j = 0; j < Sizes.Length; j++)
                {
                    switch (i)
                    {
                        case 0:
                            data[i, j + 1] = Sizes[j]["SIZE_CD"].ToString();
                            break;
                        case 1:
                            data[i, j + 1] = Sizes[j]["ORDER_QTY"].ToString();
                            break;
                        case 2:
                            data[i, j + 1] = Sizes[j]["CUTQTY"].ToString();
                            break;
                        case 3:
                            data[i, j + 1] = "";
                            break;
                        case 4:
                            data[i, j + 1] = "";
                            break;
                        case 5:
                            data[i, j + 1] = "";
                            break;
                    }
                }
            }

            double total_Order = 0;
            double total_Cut = 0;
            string align = "";
            string classType = "";
            for (int i = 0; i < Columns_Count; i++)
            {
                div_detail.InnerHtml += "<tr>";
                double total = 0;
                for (int j = 0; j < Size_Count; j++)
                {
                    if (j != 0)
                    {
                        align = "align='right'";
                        if (i == 0)
                        {
                            classType = " class='style4'";
                        }
                        else
                        {
                            classType = "";
                        }
                    }
                    else
                    {
                        align = "";
                        classType = " class='style4'";
                    }
                    if (j != Size_Count - 1)
                    {
                        if (data[i, j] == null)
                        {
                            if (j == 0)
                            {
                                div_detail.InnerHtml += "<td " + classType + "  width='180'></td>";
                            }
                            else
                            {
                                div_detail.InnerHtml += "<td " + classType + "></td>";
                            }
                        }
                        else
                        {
                            switch (data[i, 0].ToString())
                            {
                                case "Over/Short Cut%":
                                    if (j == 0)
                                    {
                                        div_detail.InnerHtml += "<td " + classType + align + ">" + data[i, j].ToString() + "</td>";
                                    }
                                    else
                                    {
                                        double percent = 0;
                                        percent = System.Math.Abs(Double.Parse(data[i - 2, j].ToString()) - Double.Parse(data[i - 1, j].ToString())) / Double.Parse(data[i - 2, j].ToString());
                                        percent = percent * 100;
                                        div_detail.InnerHtml += "<td " + classType + align + ">" + percent.ToString("##0.00") + "%</td>";
                                    }
                                    break;
                                default:
                                    div_detail.InnerHtml += "<td " + classType + align + ">" + data[i, j].ToString() + "</td>";
                                    break;
                            }

                        }
                        if (!i.Equals(0) && !j.Equals(0))
                        {
                            if (!(data[i, j] == null) && !data[i, j].ToString().Equals(""))
                            {
                                total += Double.Parse(data[i, j].ToString());
                            }
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            div_detail.InnerHtml += "<td " + classType + align + ">Total</td>";
                        }
                        else
                        {
                            string T = "";
                            switch (data[i, 0].ToString())
                            {
                                case "Order Qty":
                                    total_Order = total;
                                    T = total.ToString();
                                    break;
                                case "Cut Qty":
                                    total_Cut = total;
                                    T = total.ToString();
                                    break;
                                case "Over/Short Cut%":
                                    total = System.Math.Abs(total_Order - total_Cut) / total_Order;
                                    total = total * 100;
                                    T = total.ToString("##0.00") + "%";
                                    break;
                                    //break;
                            }
                            div_detail.InnerHtml += "<td " + classType + align + ">" + T + "</td>";
                        }
                    }
                }
                div_detail.InnerHtml += "</tr>";
            }
            div_detail.InnerHtml += "</table><br/>";
        }

    }

    protected void Merge(DataTable cutDetailPerColorSize, DataTable cutQtyPerColorSizeList)
    {
        div_detail.InnerHtml = "";
        double overBase = 0;
        double shortBase = 0;
        double allowedOverQty = 0;
        double allowedShortQty = 0;
        double orderQtyPerColorSize = 0;
        cutQtyPerColorSizeList.Columns.Add(new DataColumn("ORDER_QTY"));
        cutQtyPerColorSizeList.Columns.Add(new DataColumn("CORLORDESC"));
        cutQtyPerColorSizeList.Columns.Add(new DataColumn("orderQtyPerColorSize"));
        cutQtyPerColorSizeList.Columns.Add(new DataColumn("overBase"));
        cutQtyPerColorSizeList.Columns.Add(new DataColumn("shortBase"));
        cutQtyPerColorSizeList.Columns.Add(new DataColumn("allowedOverQty"));
        cutQtyPerColorSizeList.Columns.Add(new DataColumn("allowedShortQty"));

        if (cutDetailPerColorSize.Rows.Count > 0 && cutQtyPerColorSizeList.Rows.Count > 0)
        {
            int row_count1 = cutQtyPerColorSizeList.Rows.Count;
            int row_count2 = cutDetailPerColorSize.Rows.Count;//这两个表的行数应该都一样;
            string Color = "";

            for (int i = 0; i < row_count1; i++)
            {
                double Qty = 0;

                for (int j = 0; j < row_count2; j++)
                {
                    if (cutQtyPerColorSizeList.Rows[i]["COLOR_CD"].ToString().Equals(cutDetailPerColorSize.Rows[j]["COLOR_CODE"].ToString())
                        && cutQtyPerColorSizeList.Rows[i]["SIZE_CD"].ToString().Equals(cutDetailPerColorSize.Rows[j]["SIZE_CODE1"].ToString()))
                    {
                        Color = cutDetailPerColorSize.Rows[j]["CORLORDESC"].ToString();
                        Qty = Double.Parse(cutDetailPerColorSize.Rows[j]["ORDER_QTY"].ToString());
                        orderQtyPerColorSize += Qty;
                        allowedOverQty += Qty;
                        allowedShortQty += Qty;
                        if (!cutDetailPerColorSize.Rows[j]["PERCENTOVER"].ToString().Equals("0"))
                        {
                            overBase += Qty;
                            allowedOverQty += Double.Parse(cutDetailPerColorSize.Rows[j]["QTYOVER"].ToString());
                        }
                        if (!cutDetailPerColorSize.Rows[j]["PERCENTSHORT"].ToString().Equals("0"))
                        {
                            shortBase += Qty;
                            allowedShortQty += Double.Parse(cutDetailPerColorSize.Rows[j]["QTYSHORT"].ToString());
                        }
                    }
                }

                cutQtyPerColorSizeList.Rows[i]["ORDER_QTY"] = Qty;
                cutQtyPerColorSizeList.Rows[i]["CORLORDESC"] = Color;
                cutQtyPerColorSizeList.Rows[i]["orderQtyPerColorSize"] = orderQtyPerColorSize;
                cutQtyPerColorSizeList.Rows[i]["overBase"] = overBase;
                cutQtyPerColorSizeList.Rows[i]["shortBase"] = shortBase;
                cutQtyPerColorSizeList.Rows[i]["allowedOverQty"] = allowedOverQty;
                cutQtyPerColorSizeList.Rows[i]["allowedShortQty"] = allowedShortQty;
            }

        }

        for (int i = 0; i < cutQtyPerColorSizeList.Rows.Count; i++)
        {
            string colorCode = cutQtyPerColorSizeList.Rows[i]["COLOR_CD"].ToString();
            string sizeCode = cutQtyPerColorSizeList.Rows[i]["SIZE_CD"].ToString();
            overBase = Double.Parse(cutQtyPerColorSizeList.Rows[i]["overBase"].ToString());
            shortBase = Double.Parse(cutQtyPerColorSizeList.Rows[i]["shortBase"].ToString());
            allowedOverQty = Double.Parse(cutQtyPerColorSizeList.Rows[i]["allowedOverQty"].ToString());
            allowedShortQty = Double.Parse(cutQtyPerColorSizeList.Rows[i]["allowedShortQty"].ToString());
            orderQtyPerColorSize = Double.Parse(cutQtyPerColorSizeList.Rows[i]["orderQtyPerColorSize"].ToString());
            double cutQtyPerColorSize = Double.Parse(cutQtyPerColorSizeList.Rows[i]["CUTQTY"].ToString());

            if (cutQtyPerColorSize >= orderQtyPerColorSize)
            {//超裁处理;
                if (cutQtyPerColorSize > allowedOverQty)
                {
                    double overQty = cutQtyPerColorSize - allowedOverQty;
                    double remainQty = cutQtyPerColorSize - orderQtyPerColorSize;
                    for (int j = 0; j < cutQtyPerColorSizeList.Rows.Count; j++)
                    {
                        if (cutDetailPerColorSize.Rows[j]["COLOR_CODE"].ToString().Equals(colorCode)
                            && cutDetailPerColorSize.Rows[j]["SIZE_CODE1"].ToString().Equals(sizeCode))
                        {
                            if (!cutDetailPerColorSize.Rows[j]["PERCENTOVER"].ToString().Equals("0"))
                            {
                                double Qty = 0;
                                double orderQty = Double.Parse(cutDetailPerColorSize.Rows[j]["ORDER_QTY"].ToString());
                                double allowedQty = (Double.Parse(cutDetailPerColorSize.Rows[j]["PERCENTOVER"].ToString()) + 1) * orderQty;
                                double allocatedQty = overQty * orderQty / overBase;

                                //if ((allowedQty.doubleValue() + allocatedQty.doubleValue()) > remainQty)
                                if ((allowedQty + allocatedQty) > remainQty)
                                {
                                    //Qty = orderQty.doubleValue() + remainQty;
                                    Qty = orderQty + remainQty;
                                    remainQty = 0;
                                }
                                else
                                {
                                    //Qty = orderQty.doubleValue() + allowedQty.doubleValue() + allocatedQty.doubleValue();
                                    //remainQty -= (allowedQty.doubleValue() + allocatedQty.doubleValue());
                                    Qty = orderQty + allowedQty + allocatedQty;
                                    remainQty -= (allowedQty + allocatedQty);
                                }
                                cutDetailPerColorSize.Rows[j]["CUTQTY"] = Qty;
                            }
                        }
                    }
                }
                else
                {
                    double remainQty = cutQtyPerColorSize - orderQtyPerColorSize;
                    for (int j = 0; j < cutDetailPerColorSize.Rows.Count; j++)
                    {
                        if (cutDetailPerColorSize.Rows[j]["COLOR_CODE"].ToString().Equals(colorCode)
                                && cutDetailPerColorSize.Rows[j]["SIZE_CODE1"].ToString().Equals(sizeCode))
                        {
                            if (!cutDetailPerColorSize.Rows[i]["PERCENTOVER"].ToString().Equals("0"))
                            {
                                double Qty = 0;
                                double orderQty = Double.Parse(cutDetailPerColorSize.Rows[i]["ORDER_QTY"].ToString());
                                double allowedQty = (Double.Parse(cutDetailPerColorSize.Rows[i]["PERCENTOVER"].ToString()) / 100 + 1) * orderQty;

                                //if (allowedQty.doubleValue() > remainQty)
                                //{
                                //    qty = orderQty.doubleValue() + remainQty;
                                //    remainQty = 0;
                                //}
                                //else
                                //{
                                //    qty = orderQty.doubleValue() + allowedQty.doubleValue();
                                //    remainQty -= allowedQty.doubleValue();
                                //}
                                if (allowedQty > remainQty)
                                {
                                    Qty = orderQty + remainQty;
                                    remainQty = 0;
                                }
                                else
                                {
                                    Qty = orderQty + allowedQty;
                                    remainQty -= allowedQty;
                                }
                                cutDetailPerColorSize.Rows[j]["CUTQTY"] = Qty;
                            }
                        }
                    }
                }
            }
            else
            {//短裁处理;
                if (cutQtyPerColorSize < allowedShortQty)
                {
                    double shortQty = allowedOverQty - cutQtyPerColorSize;
                    double remainQty = orderQtyPerColorSize - cutQtyPerColorSize;
                    //List tmpList = new ArrayList();
                    for (int j = 0; j < cutDetailPerColorSize.Rows.Count; j++)
                    {
                        //HashMap cutDetailAnalysePerColorSizeMap = (HashMap)cutDetailPerColorSize.get(j);
                        if (cutDetailPerColorSize.Rows[j]["COLOR_CODE"].ToString().Equals(colorCode)
                                && cutDetailPerColorSize.Rows[j]["SIZE_CODE1"].ToString().Equals(sizeCode))
                        {
                            if (!cutDetailPerColorSize.Rows[i]["PERCENTOVER"].ToString().Equals("0"))
                            {
                                double Qty = 0;
                                double orderQty = Double.Parse(cutDetailPerColorSize.Rows[i]["ORDER_QTY"].ToString());
                                double allowedQty = (Double.Parse(cutDetailPerColorSize.Rows[i]["PERCENTSHORT"].ToString()) / 100 + 1) * orderQty;
                                double allocatedQty = shortQty * orderQty / shortBase;

                                if ((allowedQty + allocatedQty) > remainQty)
                                {
                                    Qty = orderQty - remainQty;
                                    remainQty = 0;
                                }
                                else
                                {
                                    Qty = orderQty - allowedQty - allocatedQty;
                                    remainQty -= (allowedQty + allocatedQty);
                                }
                                cutDetailPerColorSize.Rows[j]["CUTQTY"] = Qty;
                            }
                            //tmpList.add(cutDetailAnalysePerColorSizeMap);
                        }
                    }
                    if (remainQty > 0)
                    {
                        for (int j = 0; j < cutDetailPerColorSize.Rows.Count; j++)
                        {
                            //HashMap tmpMap = (HashMap)tmpList.get(j);
                            double Qty = Double.Parse((cutDetailPerColorSize.Rows[j]["CUTQTY"] == null) ? "0" : cutDetailPerColorSize.Rows[j]["CUTQTY"].ToString());

                            if (remainQty == 0)
                            {
                                //cutDetailAnalysePerColorSizeList.add(tmpMap);
                            }
                            else if (remainQty > Qty)
                            {
                                //tmpMap.put("CUTQTY", "0");
                                cutDetailPerColorSize.Rows[j]["CUTQTY"] = 0;
                                remainQty -= Qty;
                                //cutDetailAnalysePerColorSizeList.add(tmpMap);
                            }
                            else if (remainQty < Qty)
                            {
                                //tmpMap.put("CUTQTY", new doubleeger(qty - remainQty));
                                cutDetailPerColorSize.Rows[j]["CUTQTY"] = Qty - remainQty;
                                remainQty = 0;
                                //cutDetailAnalysePerColorSizeList.add(tmpMap);
                            }
                        }
                    }
                }
                else
                {
                    double remainQty = orderQtyPerColorSize - cutQtyPerColorSize;
                    for (int j = 0; j < cutDetailPerColorSize.Rows.Count; j++)
                    {
                        //HashMap cutDetailAnalysePerColorSizeMap = (HashMap)cutDetailPerColorSize.get(j);
                        if (cutDetailPerColorSize.Rows[j]["COLOR_CODE"].ToString().Equals(colorCode)
                                && cutDetailPerColorSize.Rows[j]["SIZE_CODE1"].ToString().Equals(sizeCode))
                        {
                            if (!cutDetailPerColorSize.Rows[j]["PERCENTOVER"].ToString().Equals("0"))
                            {
                                double Qty = 0;
                                double orderQty = Double.Parse(cutDetailPerColorSize.Rows[j]["ORDER_QTY"].ToString());
                                double allowedQty = (Double.Parse(cutDetailPerColorSize.Rows[j]["PERCENTSHORT"].ToString()) / 100 + 1) * orderQty;

                                if (allowedQty > remainQty)
                                {
                                    Qty = orderQty - remainQty;
                                    remainQty = 0;
                                }
                                else
                                {
                                    Qty = orderQty - allowedQty;
                                    remainQty -= allowedQty;
                                }
                                //cutDetailAnalysePerColorSizeMap.put("CUTQTY", new doubleeger(qty));
                                cutDetailPerColorSize.Rows[j]["CUTQTY"] = Qty;
                            }
                            //cutDetailAnalysePerColorSizeList.add(cutDetailAnalysePerColorSizeMap);
                        }
                    }
                }
            }
        }
    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        //MESComment.Excel.ToExcel(this.ExcTable, "PreCutSummary" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
    }

}
