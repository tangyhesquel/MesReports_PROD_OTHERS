<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FactoryMUReport.aspx.cs"
    Inherits="Reports_GEGMUReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Factory MU Report</title>
    <script src="../My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../Script/Common.js"></script>
    <style type="text/css">
        body
        {
            font-size: 8.5pt;
            font-family: Tahoma,Verdana,MS Sans Serif,Courier New;
        }
        table
        {
            font-size: 8.5pt;
            font-family: Tahoma,Verdana,MS Sans Serif,Courier New;
        }
        .style2
        {
            color: #000000;
        }
        BODY Td
        {
            font-size: 12;
            height: 25;
            color: #000000;
        }
        SELECT
        {
            font-size: 8pt;
            font-family: 'MS Sans Serif' ,Arial,Tahoma;
            background-color: #efefe7;
        }
        INPUT
        {
            font-size: 9pt;
            font-family: Tahoma, Verdana, Arial, Helvetica;
            }
        
        .Input1
        {
            font-size: 8pt;
            color: #000000;
            width: 120;
            height: 10pt;
        }
    </style>
    <script type="text/javascript" language="javascript">
        function Unlock(RunNo, fty) {
            if (RunNo.toString() == "") {
                return false;
            }
            //alert (RunNo.toString());
            if (confirm(" Do You Confirm Unlock the Data？\n System will Clear it after 1 day once unlock it.\n You can re Lock it before the new query in current page.")) {
                window.showModalDialog("SaveFactoryMUReport.aspx?RunNo=" + RunNo.toString() + "&FtyCD=" + fty + "&Utype=N", window, "dialogWidth=350px;dialogHeight=80px");
                return true;

            }
            else {
                return false;
            }

        }
        function Lock(RunNo, fty) {
            if (RunNo.toString() == "") {
                return false;
            }
            if (confirm(" Do You Confirm Lock the Data？\n System will keep the data and \n not capture & generate form other system once lock it.")) {
                window.showModalDialog("SaveFactoryMUReport.aspx?RunNo=" + RunNo.toString() + "&FtyCD=" + fty + "&Utype=Y", window, "dialogWidth=350px;dialogHeight=80px");
                return true;

            }
            else {
                return false;
            }

        }
        function Log() {

            window.showModalDialog("SaveFactoryMUReport.aspx?RunNo=''&FtyCD=''&Utype=M", window, "dialogWidth=450px;dialogHeight=300px");
            return true;
        }
    </script>
</head>
<body style="font-size: smaller">
    <object id="WebBrowser1" width="0" height="0" classid="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2">
    </object>
    <form id="form1" runat="server">
    <div>
        <div id="TabHead">
            <table width="100%" bgcolor="#efefe7">
                <tr>
                    <td>
                        <span class="style2">JO NO:</span>
                    </td>
                    <td>
                        <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                    </td>
                    <td>
                        <span class="style2">Factory Code:</span>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFtyCd" runat="server" Enabled="true">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <span class="style2">Garment Type</span>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlGarmentType" runat="server">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="K">Knit</asp:ListItem>
                            <asp:ListItem Value="W">Woven</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <span class="style2">OutSource Type</span>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlOutSource" runat="server">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="STANDARD">STANDARD</asp:ListItem>
                            <asp:ListItem Value="OUTSOURCE">OUTSOURCE</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Label ID="DateFromLabel" runat="server">Cut Completion Date From:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtFromDate" runat="server" onfocus="WdatePicker({skin:'default'})"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="DateToLable" runat="server">Cut Completion Date To:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtToDate" runat="server" onfocus="WdatePicker({skin:'default'})"></asp:TextBox>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="cbChecked" Text="Include the Detail Report" Checked="true"
                            CssClass="Input1" Visible="False" />
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="cbShipJo" CssClass="Input1" Text="Include All Ship JO"
                            Checked="true" />
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="cbNoPost" CssClass="Input1" Text="Include No Post JO "
                            Checked="false" Visible="False" />
                    </td>
                    <td>
                        <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnQuery_Click" CssClass="button_top"
                            BackColor="#c2defa" Font-Names="Arial" Font-Size="12px" BorderColor="#6FE7FF"
                            ForeColor="#CC3300" BorderStyle="Solid" BorderWidth="1px" Height="20" Width="60"
                            Font-Bold="True" />
                    </td>
                    <td>
                        <input type="button" name="Help" value="Help" style="border: 1px solid #6FE7FF; text-align: left;
                            width: 40px; font-family: Arial; font-size: 12px; color: #CC3300; background-color: #efefe7;"
                            onclick="window.open('MUHelp.htm')" />
                    </td>
                    <td>
                        <input type="button" name="btnLog" value="Log" style="border: 1px solid #6FE7FF;
                            text-align: left; width: 30px; font-family: Arial; font-size: 12px; color: #000000;
                            background-color: #efefe7;" onclick="Log()" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Report :
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlReport" runat="server" AutoPostBack="True">
                        </asp:DropDownList>
                    </td>
                    <td colspan="2">
                        <asp:CheckBox runat="server" ID="cbProductionLine" CssClass="Input1" Text="Show Production Line "
                            Visible="False" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="Red" Text="Tip Message!"
            Visible="False" Width="300px"></asp:Label>
        <br />
        <div id="ExcTable">
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td width="20%">
                        &nbsp;
                    </td>
                    <td width="60%">
                        &nbsp;
                    </td>
                    <td width="20%" align="center">
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="10">
                        <asp:Label ID="ReportTitle" runat="server" Style="font-size: 22px; font-family: Arial;
                            font-weight: bold; text-align: center;"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td width="20%">
                        &nbsp;
                    </td>
                    <td width="60%" align="center">
                        &nbsp;
                    </td>
                    <td width="20%" align="center">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="left">
                    </td>
                </tr>
            </table>
            <div id="mmPrint" align="right" style="width: 100%">
                <input name="btnPrint" type="button" value="   Print   " style="border: 1px solid #6FE7FF;
                    text-align: left; width: 80px; font-family: Arial; font-size: 12px; color: #000000;
                    background-color: #efefe7;" onclick="mmPrint.style.display='none';TabHead.style.display='none';WebBrowser1.ExecWB(6,1);TabHead.style.display='';mmPrint.style.display=''" />
                <input name="btnPreview" type="button" value="Preview" style="border: 1px solid #6FE7FF;
                    text-align: left; width: 80px; font-family: Arial; font-size: 12px; color: #000000;
                    background-color: #efefe7;" onclick="mmPrint.style.display='none';TabHead.style.display='none';WebBrowser1.ExecWB(7,1);TabHead.style.display='';mmPrint.style.display=''" />
                <input name="btnPageSetup" type="button" value="Page Setup" style="border: 1px solid #6FE7FF;
                    text-align: left; width: 80px; font-family: Arial; font-size: 12px; color: #000000;
                    background-color: #efefe7;" onclick="mmPrint.style.display='none';TabHead.style.display='none';WebBrowser1.ExecWB(8,1);TabHead.style.display='';mmPrint.style.display=''" />
                <input name="btnToExcel" type="button" value="ToExcel" style="border: 1px solid #6FE7FF;
                    text-align: left; width: 80px; font-family: Arial; font-size: 12px; color: #000000;
                    background-color: #efefe7;" onclick="toPaseExcel()" />
                <input type="button" name="ToWps" value="ToWps" style="border: 1px solid #6FE7FF;
                    text-align: left; width: 80px; font-family: Arial; font-size: 12px; color: #000000;
                    background-color: #efefe7;" onclick="ToExcelOfWPS()" />
                <input type="button" name="btnSave" value="Lock" style="border: 1px solid #6FE7FF;
                    text-align: left; width: 80px; font-family: Arial; font-size: 12px; color: #CC3300;
                    background-color: #efefe7;" onclick="Lock('<%=strRunNO%>','<%=ddlFtyCd.SelectedItem.Text%>')" />
                <input type="button" name="UnLock" value="UnLock" style="border: 1px solid #6FE7FF;
                    text-align: left; width: 80px; font-family: Arial; font-size: 12px; color: #CC3300;
                    background-color: #efefe7;" onclick="Unlock('<%=strRunNO%>','<%=ddlFtyCd.SelectedItem.Text%>')" />
            </div>
            <br />
            <table id="allTable" width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                border-collapse: collapse">
                <tr>
                    <td>
                        <table width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                            border-collapse: collapse">
                            <tr>
                                <td class="tr2style" bgcolor="#efefe7" width="100">
                                    Factory CD:
                                </td>
                                <td style="width: 5%">
                                    <%=ddlFtyCd.SelectedItem.Text%>&nbsp;
                                </td>
                                <td class="tr2style" bgcolor="#efefe7" width="100">
                                    Garment Type:
                                </td>
                                <td style="width: 8%">
                                    <%=ddlGarmentType.SelectedItem.Text%>&nbsp;
                                </td>
                                <td class="tr2style" bgcolor="#efefe7" width="100">
                                    JO Type:
                                </td>
                                <td>
                                    <%=ddlOutSource.SelectedItem.Text%>&nbsp;
                                </td>
                                <td class="tr2style" bgcolor="#efefe7" width="100">
                                    Start Date:
                                </td>
                                <td style="width: 8%">
                                    <%=txtFromDate.Text%>&nbsp;
                                </td>
                                <td class="tr2style" bgcolor="#efefe7" width="100">
                                    End Date:
                                </td>
                                <td style="width: 8%">
                                    <%=txtToDate.Text%>&nbsp;
                                </td>
                                <td class="tr2style" bgcolor="#efefe7" width="100">
                                    Print Date Time :
                                </td>
                                <td style="width: 8%">
                                    <asp:Label ID="PrintDate" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <div id="divBody" runat="server">
                        </div>
                    </td>
                </tr>
            </table>
            <!--MU Standard Report-->
            <asp:GridView ID="gvDetail" runat="server" Width="2100px" OnRowDataBound="gvDetail_RowDataBound"
                AutoGenerateColumns="false" ShowFooter="True" HeaderStyle-Width="9em" HeaderStyle-Wrap="false"
                HeaderStyle-Height="3em">
                <Columns>
                    <asp:BoundField DataField="CUT_DATE" HtmlEncode="False" DataFormatString="{0:d}" />
                    <asp:BoundField DataField="JO_NO" />
                    <asp:BoundField DataField="YPD_JOB_NO" />
                    <asp:BoundField DataField="SHIP_DATE" HtmlEncode="False" DataFormatString="{0:d}" />
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
                    <asp:BoundField DataField="Issued" />
                    <asp:BoundField DataField="MA" />
                    <asp:BoundField DataField="ShipYardage" />
                    <asp:BoundField DataField="CutWastageYPD" />
                    <asp:BoundField DataField="CutWastageYPDPer" />
                    <asp:BoundField DataField="DEFECT_FAB" />
                    <asp:BoundField DataField="DEFECT_PANELS" />
                    <asp:BoundField DataField="ODD_LOSS" />
                    <asp:BoundField DataField="SPLICE_LOSS" />
                    <asp:BoundField DataField="CUT_REJ_PANELS" />
                    <asp:BoundField DataField="MATCH_LOSS" />
                    <asp:BoundField DataField="END_LOSS" />
                    <asp:BoundField DataField="SHORT_YDS" />
                    <asp:BoundField DataField="SEW_MATCH_LOSS" />
                    <asp:BoundField DataField="Leftover" />
                    <asp:BoundField DataField="LEFTOVER_REASON" />
                    <asp:BoundField DataField="LEFTOVER_desc" />
                    <asp:BoundField DataField="REMNANT" />
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
                    <asp:BoundField DataField="OVER_CUT" />
                </Columns>
            </asp:GridView>
            <br />
            <!--布损耗本厂报表-->
            <asp:GridView ID="gvFabricDetail" runat="server" Width="2100px" OnRowDataBound="gvFabricDetail_RowDataBound"
                AutoGenerateColumns="true" ShowHeader="true" ShowFooter="True" HeaderStyle-Width="9em"
                HeaderStyle-Wrap="false" HeaderStyle-Height="3em">
                <FooterStyle BackColor="#F7F6F3" />
                <HeaderStyle BackColor="#F7F6F3" />
            </asp:GridView>
            <br />
            <!--成衣损耗本厂报表-->
            <asp:GridView ID="gvGarmentDetail" runat="server" Width="2100px" OnRowDataBound="gvGarmentDetail_RowDataBound"
                AutoGenerateColumns="true" ShowHeader="true" ShowFooter="false" HeaderStyle-Width="9em"
                HeaderStyle-Wrap="false" HeaderStyle-Height="3em">
            </asp:GridView>
            <br />
            <div id="divSummary" runat="server">
            </div>
            <%--  <div id="divSummary" runat="server">
                        <table border='1' cellspacing='0' cellpadding='0' style='font-size: 12px; border-collapse: collapse'>
                        <tr><td colspan="2" align="center"><h3>MU METRICS REPORT</h3></td></tr>
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
                                    <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[42].Text%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    SHIP / RECEIVE (%)
                                </td>
                                <td>
                                <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[43].Text%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    SHIP / ORDER (%)
                                </td>
                                <td>
                                <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[44].Text%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    CUT / ORDER (%)
                                </td>
                                <td>
                                <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[46].Text%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Cut / RECEIVE(%)
                                </td>
                                <td>
                                <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[45].Text%>
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
                                <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[9].Text%>
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
                                <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[47].Text%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    LEFTOVER FABRIC (%)
                                </td>
                                <td>
                                <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[17].Text%>
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
                                <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[16].Text%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    LEFTOVER GARMENT (%)
                                </td>
                                <td>
                                <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[22].Text=="0.00"?"0.00%":(double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[29].Text) / double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[22].Text) * 100).ToString("0.00") + "%"%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    UNACCOUNTABLE GARMENT (%)
                                </td>
                                <td>
                                <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[22].Text=="0.00"?"0.00%":(double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[34].Text) / double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[22].Text) * 100).ToString("0.00") + "%"%>
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
                                <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[29].Text%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Order Qty (dz)
                                </td>
                                <td>
                                <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[21].Text%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Cut Qty (dz)
                                </td>
                                <td>
                                <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[22].Text%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Shipped Qty (dz)
                                </td>
                                <td>
                                <%=gvDetail.Rows.Count == 0 ? null : (double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[23].Text) + double.Parse(gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[24].Text)).ToString()%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Unaccountable Garment (dz)
                                </td>
                                <td>
                                <%=gvDetail.Rows.Count == 0 ? null : gvDetail.Rows[gvDetail.Rows.Count - 1].Cells[34].Text%>
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
                    </div>--%>
        </div>
    </div>
    </form>
</body>
</html>
