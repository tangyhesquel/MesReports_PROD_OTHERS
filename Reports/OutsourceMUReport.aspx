<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OutsourceMUReport.aspx.cs"
    Inherits="Reports_standardMUReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Script/Common.js"></script>
</head>
<body style="font-size: smaller">
    <object id="WebBrowser1" width="0" height="0" classid="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2">
    </object>
    <form id="form1" runat="server">
    <div>
        <fieldset>
            <legend>Search </legend>
            <table width="100%" border="0px">
                <tr>
                    <td class="tdbackcolor">
                        JO NO:
                    </td>
                    <td>
                        <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                    </td>
                    <td class="tdbackcolor">
                        GO NO:
                    </td>
                    <td>
                        <asp:TextBox ID="txtGoNo" runat="server"></asp:TextBox>
                    </td>
                    <td class="tdbackcolor">
                        Factory Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFtyCd" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td class="tdbackcolor">
                        Garment Type
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlGarmentType" runat="server">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="K">Knit</asp:ListItem>
                            <asp:ListItem Value="W">Woven</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnQuery_Click" CssClass="button_top" />
                    </td>
                </tr>
                <tr>
                    <td class="tdbackcolor">
                        Ship Date From:
                    </td>
                    <td>
                        <asp:TextBox ID="txtFromDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td class="tdbackcolor">
                        Ship Date To:
                    </td>
                    <td>
                        <asp:TextBox ID="txtToDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td colspan="2">
                        <asp:CheckBox runat="server" ID="cbChecked" Text="Include the Detail Report" Checked="true" />
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="cbShipJo" Text="Include All Ship JO" Checked="false" />
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="cbNoPost" Text="Include No Post JO " Checked="false" />
                    </td>
                    <td align="right">
                    </td>
                </tr>
            </table>
        </fieldset>
        <table width="100%">
            <tr>
                <td colspan="12" style="height: 19px">
                    &nbsp;&nbsp;&nbsp;&nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="14" id="toPrint" align="right" style="height: 19px">
                    <input name="btnPrint" type="button" value="   Print   " onclick="toPrint.style.display='none';WebBrowser1.ExecWB(6,1);toPrint.style.display=''" />
                    <input name="btnPreview" type="button" value="Preview" onclick="toPrint.style.display='none';WebBrowser1.ExecWB(7,1);toPrint.style.display=''" />
                    <input name="btnPageSetup" type="button" value="Page Setup" onclick="toPrint.style.display='none';WebBrowser1.ExecWB(8,1);toPrint.style.display=''" />
                    <input name="btnToExcel" type="button" value="ToExcel" onclick="toPaseExcel()" />
                    <input type="button" name="ToWps" value="ToWps" onclick="ToExcelOfWPS()" />
                </td>
            </tr>
        </table>
        <table border="0px">
            <tr>
                <td colspan="16" align="center" style="align: center; color: Red; font-size: x-large">
                    <asp:Label runat="server" ID="nodata" Visible="false"> No Data</asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="15">
                    <br />
                    <div id="ExcTable">
                        <asp:GridView ID="gvDetail" runat="server" Width="2000px" OnRowDataBound="gvDetail_RowDataBound"
                            AutoGenerateColumns="False">
                            <Columns>
                                <asp:BoundField DataField="FACTORY" /> <%--0--%>
                                <asp:BoundField DataField="JOB_ORDER_NO" /> <%--1--%>
                                <asp:BoundField DataField="YPD_JOB_NO" /> <%--2--%>
                                <asp:BoundField DataField="SHIP_DATE" /> <%--3--%>
                                <asp:BoundField DataField="BUYER" /> <%--4--%>
                                <asp:BoundField DataField="SC_NO" /> <%--5--%>
                                <asp:BoundField DataField="PPO_NO" /> <%--6--%>
                                <asp:BoundField DataField="WIDTH" /> <%--7--%>
                                <asp:BoundField DataField="style_desc" /> <%--8--%>
                                <asp:BoundField DataField="WASH_TYPE_CD" /> <%--9--%>
                                <asp:BoundField DataField="PATTERN_TYPE" /> <%--10--%>
                                <asp:BoundField DataField="MU" /> <%--11--%>
                                <asp:BoundField DataField="ORDER_QTY" /> <%--12--%>
                                <asp:BoundField DataField="allocatedQty" /> <%--13--%>
                                <asp:BoundField DataField="Issued" /> <%--14--%>
                                <asp:BoundField DataField="MA" /> <%--15--%>
                                <asp:BoundField DataField="ShipYardage" /> <%--16--%>
                                <asp:BoundField DataField="CutWastageYPD" /> <%--17--%>
                                <asp:BoundField DataField="CutWastageYPDPer" /> <%--18--%>
                                <asp:BoundField DataField="PULLOUT_FAB" /> <%--19--%>
                                <asp:BoundField DataField="SHORT_FAB" /> <%--20--%>
                                <asp:BoundField DataField="DEFECT_FAB" /> <%--21--%>
                                <asp:BoundField DataField="DEFECT_PANEL" /> <%--22--%>
                                <asp:BoundField DataField="FOC_QTY" /> <%--23--%>
                                <asp:BoundField DataField="Leftover" /> <%--24--%>
                                <asp:BoundField DataField="REMNANT" /> <%--25--%>
                                <asp:BoundField DataField="SRNQty" /> <%--26--%>
                                <asp:BoundField DataField="RTW_QTY" /> <%--27--%>
                                <asp:BoundField DataField="UNACC_FAB" /> <%--28--%>
                                <asp:BoundField DataField="ORDERQTY" /> <%--29--%>
                                <asp:BoundField DataField="CUTQTY" /> <%--30--%>
                                <asp:BoundField DataField="CUTDISC" /> <%--31--%>
                                <asp:BoundField DataField="SHIP_QTY" /> <%--32--%>
                                <asp:BoundField DataField="SAMPLE_QTY" /> <%--33--%>
                                <asp:BoundField DataField="PULLOUT_QTY" /> <%--34--%>
                                <asp:BoundField DataField="GMT_QTY_A" /> <%--35--%>
                                <asp:BoundField DataField="GMT_QTY_B" /> <%--36--%>
                                <asp:BoundField DataField="GMT_QTY_C" /> <%--37--%>
                                <asp:BoundField DataField="GMT_QTY_TOTAL" /> <%--38--%>
                                <asp:BoundField DataField="SewingDz" /> <%--39--%>
                                <asp:BoundField DataField="SewingPercent" /> <%--40--%>
                                <asp:BoundField DataField="WashingDz" /> <%--41--%>
                                <asp:BoundField DataField="WashingPercent" /> <%--42--%>
                                <asp:BoundField DataField="UnaccGmt" /> <%--43--%>
                                <asp:BoundField DataField="PPO_YPD" /> <%--44--%>
                                <asp:BoundField DataField="BULK_NET_YPD" /> <%--45--%>
                                <asp:BoundField DataField="GIVEN_CUT_YPD" /> <%--46--%>
                                <asp:BoundField DataField="BULK_MKR_YPD" /> <%--47--%>
                                <asp:BoundField DataField="YPD_Var" /> <%--48--%>
                                <asp:BoundField DataField="cutYPD" /> <%--49--%>
                                <asp:BoundField DataField="ShipYPD" /> <%--50--%>
                                <asp:BoundField DataField="ShipToCut" /> <%--51--%>
                                <asp:BoundField DataField="ShipToRecv" /> <%--52--%>
                                <asp:BoundField DataField="ShipToOrder" /> <%--53--%>
                                <asp:BoundField DataField="cut_to_receipt" /> <%--54--%>
                                <asp:BoundField DataField="cut_to_order" /> <%--55--%>
                                <asp:BoundField DataField="OVERSHIP" /> <%--56--%>
                            </Columns>
                        </asp:GridView>
                        <br />
                        <div id="divSummary" runat="server">
                            <table border='1' cellspacing='0' cellpadding='0' style='font-size: 12px; border-collapse: collapse'>
                                <tr>
                                    <td colspan="2" align="center">
                                        <h3>
                                            MU METRICS REPORT</h3>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Items
                                    </td>
                                    <td>
                                        <%=txtFromDate.Text==""?null:months[DateTime.Parse(txtFromDate.Text).Month-1] %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        SHIP / CUT (%)
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[50].Text%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        SHIP / RECEIVE (%)
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[51].Text%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        SHIP / ORDER (%)
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[52].Text%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        CUT / ORDER (%)
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[54].Text%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Cut / RECEIVE(%)
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[53].Text%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        SHIP/ORDER < 95%
                                    </td>
                                    <td>
                                        <%=((double)ship_order_count_95/(double)(gvDetail.Rows.Count-1)*100).ToString("0.00")+"%" %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        SHIP/ORDER < 100%
                                    </td>
                                    <td>
                                        <%=((double)ship_order_count_100/(double)(gvDetail.Rows.Count-1)*100).ToString("0.00")+"%" %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        MARKER UTILIZATION (%)
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[10].Text%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;&nbsp;CHECK
                                    </td>
                                    <td>
                                        <%=FabricPattern.Split(',')[0] %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;&nbsp;SOLID
                                    </td>
                                    <td>
                                        <%=FabricPattern.Split(',')[1] %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;&nbsp;STRIPE
                                    </td>
                                    <td>
                                        <%=FabricPattern.Split(',')[2] %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;&nbsp;CHECK;SOLID
                                    </td>
                                    <td>
                                        <%=FabricPattern.Split(',')[3] %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;&nbsp;CHECK;STRIPE
                                    </td>
                                    <td>
                                        <%=FabricPattern.Split(',')[4] %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;&nbsp;SOLID;STRIPE
                                    </td>
                                    <td>
                                        <%=FabricPattern.Split(',')[5] %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;&nbsp;CHECK;SOLID;STRIPE
                                    </td>
                                    <td>
                                        <%=FabricPattern.Split(',')[6] %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        MAX SHIPMENT ALLOWANCE
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[55].Text%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        LEFTOVER FABRIC (%)
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[23].Text%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        YPD VARIANCE > 0.25 (%)
                                    </td>
                                    <td>
                                        <%=((double)YPD_Var_big/(double)(gvDetail.Rows.Count-1)*100).ToString("0.00")+"%" %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        YPD VARIANCE < - 0.25 (%)
                                    </td>
                                    <td>
                                        <%=((double)YPD_Var_small/(double)(gvDetail.Rows.Count-1)*100).ToString("0.00")+"%" %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        CUTTING WASTAGE (%)
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[17].Text%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        LEFTOVER GARMENT (%)
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[29].Text=="0.00"?"0.00%":(double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[37].Text) / double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[29].Text) * 100).ToString("0.00") + "%"%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        UNACCOUNTABLE GARMENT (%)
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[29].Text=="0.00"?"0.00%":(double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[42].Text) / double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[29].Text) * 100).ToString("0.00") + "%"%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Leftover Garment (dz)
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[37].Text%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Order Qty (dz)
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[28].Text%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Cut Qty (dz)
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[29].Text%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Shipped Qty (dz)
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count == 0 ? null : (double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[31].Text) + double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[32].Text)).ToString()%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Unaccountable Garment (dz)
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[42].Text%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Shipped Qty (Yards)
                                    </td>
                                    <td>
                                        <%=Total_Ship_Yardage %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Allocated Qty (Yards)
                                    </td>
                                    <td>
                                        <%=Total_Allocated %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Leftover Qty (Yards)
                                    </td>
                                    <td>
                                        <%=Total_Leftover %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Total orders
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count-1 %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        # of orders variance > 0.25
                                    </td>
                                    <td>
                                        <%=YPD_Var_big %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        # of orders variance < -0.25
                                    </td>
                                    <td>
                                        <%=YPD_Var_small %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        # of orders ship/order < 95%
                                    </td>
                                    <td>
                                        <%=ship_order_count_95 %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        # of orders ship/order < 100%
                                    </td>
                                    <td>
                                        <%=ship_order_count_100 %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Short shipment Order <100% (dz)
                                    </td>
                                    <td>
                                        <%=ship_order_sum%>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
