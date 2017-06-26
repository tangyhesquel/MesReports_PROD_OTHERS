<%@ Page Language="C#" AutoEventWireup="true" CodeFile="standardMUReport.aspx.cs"
    Inherits="Reports_standardMUReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script>
        function ConfirmLock() {
            var confrimvalue = false;
            confrimvalue = confirm("确定Lock数据吗？");
            if (!confrimvalue)
                return false;
            return true;
            
        }
    </script>
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
                    <td class="tdbackcolor">
                        SAM GROUP
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlOutSource" runat="server">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="STANDARD">STANDARD</asp:ListItem>
                            <asp:ListItem Value="OUTSOURCE">OUTSOURCE</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnQuery_Click" CssClass="button_top" />
                          &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnLock" runat="server" Text="Lock"  CssClass="button_top" OnClientClick="return ConfirmLock()"
                            onclick="btnLock_Click" />
                    </td>
                </tr>
                <tr>
                    <td class="tdbackcolor">
                        Fin Close Date From:
                    </td>
                    <td>
                        <asp:TextBox ID="txtFromDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td class="tdbackcolor">
                        Fin Close Date To:
                    </td>
                    <td>
                        <asp:TextBox ID="txtToDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td colspan="2">
                        <asp:CheckBox runat="server" ID="cbChecked" Text="Include the Detail Report" Checked="true" Visible="false"/>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="cbShipJo" Text="Include All Ship JO" Checked="false"  Visible="false"/>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="cbNoPost" Text="Include No Post JO " Checked="false"  Visible="false"/>
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
                            AutoGenerateColumns="False" ShowFooter="True">
                            <Columns>
                                <asp:BoundField DataField="PO_NO" />
                                <asp:BoundField DataField="YPD_JOB_NO" />
                                <asp:BoundField DataField="SHIP_DATE" />
                                <asp:BoundField DataField="BUYER" />
                                <asp:BoundField DataField="SC_NO" />
                                <asp:BoundField DataField="PPO_NO" />
                                <asp:BoundField DataField="WIDTH" />
                                <asp:BoundField DataField="style_desc" />
                                <asp:BoundField DataField="WASH_TYPE_CD" />
                                <asp:BoundField DataField="PATTERN_TYPE" />
                                <asp:BoundField DataField="MU" />
                                <asp:BoundField DataField="ORDER_QTY" />
                                <asp:BoundField DataField="allocatedQty" />
                                <asp:BoundField DataField="allocatedQty(nodiscount)" />
                                <asp:BoundField DataField="Issued" />
                                <asp:BoundField DataField="MA" />
                                <asp:BoundField DataField="ShipYardage" />
                                <asp:BoundField DataField="CutWastageYPD" />
                                <asp:BoundField DataField="CutWastageYPDPer" />
                                <asp:BoundField DataField="Leftover" />
                               
                                <asp:BoundField DataField="SRNQty" />
                                <asp:BoundField DataField="RTW_QTY" />
                                <asp:BoundField DataField="ORDERQTY" />
                                <asp:BoundField DataField="CUTQTY" />
                                <asp:BoundField DataField="SHIP_QTY" />
                                <asp:BoundField DataField="SAMPLE_QTY" />
                                <asp:BoundField DataField="PULLOUT_QTY" />
                                <asp:BoundField DataField="GMT_QTY_A" />
                                <asp:BoundField DataField="GMT_QTY_B" />
                                <asp:BoundField DataField="DISCREPANCY_QTY" />
                                <asp:BoundField DataField="GMT_QTY_TOTAL" />
                                <asp:BoundField DataField="SewingDz" />
                                <asp:BoundField DataField="SewingPercent" />
                                <asp:BoundField DataField="WashingDz" />
                                <asp:BoundField DataField="WashingPercent" />
                                <asp:BoundField DataField="UnaccGmt" />
                                <asp:BoundField DataField="PPO_YPD" />
                                <asp:BoundField DataField="BULK_NET_YPD" />
                                <asp:BoundField DataField="GIVEN_CUT_YPD" />
                                <asp:BoundField DataField="BULK_MKR_YPD" />
                                <asp:BoundField DataField="YPD_Var" />
                                <asp:BoundField DataField="cutYPD" />
                                <asp:BoundField DataField="ShipYPD" />
                                <asp:BoundField DataField="ShipToCut" />
                                <asp:BoundField DataField="ShipToRecv" />
                                <asp:BoundField DataField="ShipToOrder" />
                                <asp:BoundField DataField="cut_to_receipt" />
                                <asp:BoundField DataField="cut_to_order" />
                                <asp:BoundField DataField="OVERSHIP" />
                                <asp:BoundField DataField="SHORTSHIP" />
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
                                       <%-- <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[42].Text%>--%>
                                       <label id="lblshiptocut" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        SHIP / RECEIVE (%)
                                    </td>
                                    <td>
                                       <%-- <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[43].Text%>--%>
                                         <label id="lblshiptorec" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        SHIP / ORDER (%)
                                    </td>
                                    <td>
                                       <%-- <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[44].Text%>--%>
                                        <label id="lblshiptoorder" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        CUT / ORDER (%)
                                    </td>
                                    <td>
                                    <%--    <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[46].Text%>--%>
                                     <label id="lblcuttoorder" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Cut / RECEIVE(%)
                                    </td>
                                    <td>
                                      <%--  <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[45].Text%>--%>
                                      <label id="lblcuttorec" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        SHIP/ORDER < 95%
                                    </td>
                                    <td>
                                        <%--<%=((double)ship_order_count_95/(double)(gvDetail.Rows.Count-1)*100).ToString("0.00")+"%" %>--%>
                                        <label id="lblshiporder1" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        SHIP/ORDER < 100%
                                    </td>
                                    <td>
                                       <%-- <%=((double)ship_order_count_100/(double)(gvDetail.Rows.Count-1)*100).ToString("0.00")+"%" %>--%>
                                         <label id="lblshiporder2" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        MARKER UTILIZATION (%)
                                    </td>
                                    <td>
                                        <%--<%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[9].Text%>--%>
                                         <label id="lblmu" runat="server"></label>
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
                                        <%--<%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[47].Text%>--%>
                                        <label id="lblttlovership" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        LEFTOVER FABRIC (%)
                                    </td>
                                    <td>
                                        <%--<%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[17].Text%>--%>
                                        <label id="lblttlleftfab" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        YPD VARIANCE > 0.25 (%)
                                    </td>
                                    <td>
                                        <%--<%=((double)YPD_Var_big/(double)(gvDetail.Rows.Count-1)*100).ToString("0.00")+"%" %>--%>
                                        <label id="lblypdvariancebig" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        YPD VARIANCE < - 0.25 (%)
                                    </td>
                                    <td>
                                       <%-- <%=((double)YPD_Var_small/(double)(gvDetail.Rows.Count-1)*100).ToString("0.00")+"%" %>--%>
                                        <label id="lblypdvariancesmall" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        CUTTING WASTAGE (%)
                                    </td>
                                    <td>
                                        <%--<%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[16].Text%>--%>
                                        <label id="lblcutwastper" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        LEFTOVER GARMENT (%)
                                    </td>
                                    <td>
                                        <%--<%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[22].Text=="0.00"?"0.00%":(double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[29].Text) / double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[22].Text) * 100).ToString("0.00") + "%"%>--%>
                                        <label id="lblleftovergar" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        UNACCOUNTABLE GARMENT (%)
                                    </td>
                                    <td>
                                       <%-- <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[22].Text=="0.00"?"0.00%":(double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[34].Text) / double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[22].Text) * 100).ToString("0.00") + "%"%>--%>
                                       <label id="lblunaccountablegar" runat="server"></label>
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
                                        <%--<%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[29].Text%>--%>
                                          <label id="lblleftovertotal" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Order Qty (dz)
                                    </td>
                                    <td>
                                       <%-- <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[21].Text%>--%>
                                       <label id="lblorderqty" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Cut Qty (dz)
                                    </td>
                                    <td>
                                       <%-- <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[22].Text%>--%>
                                        <label id="lblcutqty" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Shipped Qty (dz)
                                    </td>
                                    <td>
                                      <%--  <%=gvDetail.Rows.Count == 0 ? null : (double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[23].Text) + double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[24].Text)).ToString()%>--%>
                                        <label id="lblshippedqty" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Unaccountable Garment (dz)
                                    </td>
                                    <td>
                                     <%--   <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[34].Text%>--%>
                                      <label id="lblunaccountableqty" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Shipped Qty (Yards)
                                    </td>
                                    <td>
                                       <%-- <%=Total_Ship_Yardage %>--%>
                                       <label id="lblshipyardage" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Allocated Qty (Yards)
                                    </td>
                                    <td>
                                        <%--<%=Total_Allocated %>--%>
                                         <label id="lblallocate" runat="server"></label>
                                    </td>
                                </tr>
                                 <tr>
                                    <td>
                                       Allocated Qty (Without Discount)(Yards)
                                    </td>
                                    <td>
                                     <label id="lblallocatewithnodiscount" runat="server"></label>
                                       <%-- <%=Total_Allocated1%>--%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Leftover Qty (Yards)
                                    </td>
                                    <td>
                                       <%-- <%=Total_Leftover %>--%>
                                       <label id="lbltotalleftover" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Total orders
                                    </td>
                                    <td>
                                        <%=gvDetail.Rows.Count %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        # of orders variance > 0.25
                                    </td>
                                    <td>
                                        <%--<%=YPD_Var_big %>--%>
                                        <label id="lblypdvarbig" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        # of orders variance < -0.25
                                    </td>
                                    <td>
                                        <%--<%=YPD_Var_small %>--%>
                                        <label id="lblypdvarsmall" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        # of orders ship/order < 95%
                                    </td>
                                    <td>
                                       <%-- <%=ship_order_count_95 %>--%>
                                        <label id="lblshiporder95" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        # of orders ship/order < 100%
                                    </td>
                                    <td>
                                        <%--<%=ship_order_count_100 %>--%>
                                         <label id="lblshiporder100" runat="server"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Short shipment Order <100% (dz)
                                    </td>
                                    <td>
                                    <label id="lblshipordersum" runat="server"></label>
                                      <%--  <%=ship_order_sum%>--%>
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
