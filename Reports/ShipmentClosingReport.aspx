<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ShipmentClosingReport.aspx.cs"
    Inherits="Reports_ShipmentClosingReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Order Actual Production and Shipment Closing Report</title>
    <style type="text/css">
        BODY
        {
            font-size: 12px;
            padding: 5px;
            font-family: Arial,Times New Roman, 黑体,Verdana,Helvetica, sans-serif;
        }
        a
        {
            text-decoration: none;
            color: #057fac;
        }
        hr
       {
           
            margin-top:20px;
            height:2px;
            background-color: Green;
        }
        a:hover
        {
            text-decoration: none;
            color: #999;
        }
        .button_top
        {
            font-family: "Arial" , "Helvetica" , "sans-serif";
            font-size: 12px;
            background-color: #CCCC99;
            border: ridge;
            font-weight: bold;
            border-width: 1px 1px 2px;
            border-color: #000000 #f2f2f2 #f2f2f2 #000000;
            color: #333366;
            text-transform: uppercase;
            cursor: hand;
        }
        .tdbackcolor
        {
            background-color: #EFEFE0;
        }
        .thstyle
        {
            background-color: #D2D1B0;
        }
        .bkColorW
        {
            background-color: White;
        }
        .bkColorG
        {
            background-color: #C0C0C0;
        }
        .bkColorR
        {
            background-color: Red;
        }
        .tr2style
        {
            font-weight: 900;
        }
        .textAlign
        {
            text-align: center;
        }
        .textRight
        {
            text-align: right;
        }
        .textLeft
        {
            text-align: left;
        }
    </style>
    <script src="../Script/jquery-1.4.2.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script type="text/javascript">

        function searchJo() {
            var urlName = "searchJONewCut.aspx" + window.location.search + "&factory=" + document.all.ddlFactory.value + "&userRandom=" + (Math.random() * 100000);
            var jo = window.showModalDialog(urlName, "strJoList.", "dialogWidth=650pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (jo == null) return;
            document.all.txtJoNo.value = jo;
        }
        function searchGo() {
            var urlName = "searchGO.aspx?factory=" + document.all.ddlFactory.value + "&userRandom=" + (Math.random() * 100000);
            var jo = window.showModalDialog(urlName, "Go No.", "dialogWidth=650pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (jo == null) return;
            document.all.txtJoNo.value = jo;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="queryDiv" runat="server">
        <table width="100%" id="queryTab">
            <tr>
                <td height="19" class="tdbackcolor">
                    Factory:
                </td>
                <td>
                    <asp:DropDownList ID="ddlFactory" Enabled="false" runat="server">
                    </asp:DropDownList>
                </td>
                <td height="19" class="tdbackcolor">
                    Go No:
                </td>
                <td style="">
                    <asp:TextBox ID="txtGO" runat="server"></asp:TextBox>&nbsp;
                    <input type="button" value="..." onclick="searchGo()" class="button_top" style="visibility: hidden" />
                </td>
                <td height="19" class="tdbackcolor">
                    Job Order No.:
                </td>
                <td>
                    <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>&nbsp;
                    <input id="Button1" type="button" runat="server" value="..." onclick="searchJo()"
                        class="button_top" />
                </td>
                <td height="19" class="tdbackcolor">
                    Summary By
                </td>
                <td>
                    <asp:DropDownList ID="DDSummaryBy" runat="server">
                        <asp:ListItem Value="JO" Text="JO NO"></asp:ListItem>
                        <asp:ListItem Value="GO" Text="GO NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td width="20%">
                    <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                </td>
            </tr>
        </table>
    </div>
    <div id="divMeg" runat="server" style="margin-top: 20px">
    </div>
    <table id="ButtonTbale"  width="100%" border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td align="center">
                <font face="Arial" size="4">Order Actual Production and Shipment Closing Report</font>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <div id="mmPrint" align="right">
                    <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv.style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv .style.display='block';document.all.mmPrint.style.visibility='visible'"
                        style="font-size: 16; width: 80; height: 26" />
                    <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,'ActualCuttingReport.htm')"
                        style="font-size: 16; width: 80; height: 26" />
                    <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="font-size: 16;
                        width: 80; height: 26" />
                    <input type="button" name="excel" value="To WPS" onclick="ToExcelOfWPS()" style="font-size: 12;
                        width: 80; height: 26" />
                </div>
            </td>
        </tr>
    </table>
    &nbsp; &nbsp;
    <div id="ExcTable" style="width: 100%">
        <asp:Repeater ID="rpTotal" runat="server" OnItemDataBound="rpTotal_ItemDataBound">
            <ItemTemplate>
                <table border="0" cellpadding="0" cellspacing="0"  width="100%">
                    <tr>
                        <td>
                            <asp:Repeater ID="rpHeader" runat="server">
                                <HeaderTemplate>
                                    <table width="100%" style="border-collapse: collapse; background-color: #C0C0C0;
                                        text-align: center; font-size: 12px" border="1" cellspacing="0" cellpadding="0">
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr style="height: 50px;">
                                        <td class="tr2style" width="100">
                                            JO No
                                        </td>
                                        <td width="150">
                                            <%# Eval("JoNo") %>
                                        </td>
                                        <td class="tr2style" width="100">
                                            S/C No.
                                        </td>
                                        <td>
                                            <%# Eval("ScNo") %>
                                        </td>
                                        <td class="tr2style" width="100">
                                            GMT DATE
                                        </td>
                                        <td>
                                            <%# Eval("GMTDate") %>
                                        </td>
                                        <td class="tr2style" width="100">
                                            Buyer<br />
                                            Shippment<br />
                                            Allowance %
                                        </td>
                                        <td>
                                            <%# Eval("ShippmentAllowance")%>
                                        </td>
                                    </tr>
                                    <tr style="height: 40px;">
                                        <td class="tr2style" width="100">
                                            Start Cut Date
                                        </td>
                                        <td width="150">
                                            <%# Eval("StartCutDate") %>
                                        </td>
                                        <td class="tr2style" width="100">
                                            Style No.
                                        </td>
                                        <td>
                                            <%# Eval("StyleNo") %>
                                        </td>
                                        <td class="tr2style" width="100">
                                            Style Desc.
                                        </td>
                                        <td>
                                            <%# Eval("season")%>
                                        </td>
                                        <td class="tr2style" width="100">
                                            Order Close
                                            <br />
                                            Date
                                        </td>
                                        <td>
                                            <%# Eval("COMPLETIONDATE")%>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody> </table>
                                </FooterTemplate>
                            </asp:Repeater>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:GridView ID="gvList" Width="100%" runat="server" OnRowDataBound="gvList_RowDataBound"
                                OnRowCreated="gvList_RowCreated">
                                <%--<Columns>
                                    <asp:TemplateField HeaderText="Color/Size">
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("Color_CD").ToString() + "("+ Eval("COLOR_DESC") +")" %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                 
                                </Columns>--%>
                            </asp:GridView>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Repeater ID="rpListFoot" runat="server">
                                <HeaderTemplate>
                                    <table width="100%" border="1" cellpadding="0" cellspacing="0" style="text-align: center;">
                                        <tr style="height: 50px; background-color: #C0C0C0;">
                                            <td>
                                                Total Order-Qty
                                            </td>
                                            <td>
                                                Total Cut-Qty
                                            </td>
                                            <td>
                                                Total Cut-Reduce
                                            </td>
                                            <td>
                                                Total Actual-Qty
                                            </td>
                                            <td>
                                                Total Over/Short<br />
                                                Cut-Qty
                                            </td>
                                            <td>
                                                Total Over/Short Cut%
                                            </td>
                                            <td style="color: Red; font-weight: 500">
                                                Total Scan-PackQty
                                            </td>
                                            <td style="color: Red; font-weight: 500">
                                                Total Ship-Qty
                                            </td>
                                            <td>
                                                Total Over/Short Qty<br />
                                                Ship Qty
                                            </td>
                                            <td>
                                                Total Over/Short Ship%
                                            </td>
                                            <td>
                                                Total Leftover<br />
                                                Garment A
                                            </td>
                                            <td>
                                                Total B Grade Qty
                                            </td>
                                            <td>
                                                Total C Grade Qty
                                            </td>
                                            <td>
                                                Total Unaccountable Qty
                                            </td>
                                        </tr>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <%# Eval("TOrder_QTY")%>
                                        </td>
                                        <td>
                                            <%# Eval("TCut_Qty")%>
                                        </td>
                                        <td>
                                            <%# Eval("TCut_Reduce")%>
                                        </td>
                                        <td>
                                            <%# Eval("TActual_Qty")%>
                                        </td>
                                        <td>
                                            <%# Eval("TOverShortCut_QTY")%>
                                        </td>
                                        <td>
                                            <%# Eval("TOverShortCut_PER")%>%
                                        </td>
                                        <td>
                                            <%# Eval("TScanPack_QTY")%>
                                        </td>
                                        <td>
                                            <%# Eval("TShip_Qty")%>
                                        </td>
                                        <td>
                                            <%# Eval("TOverShortShip_QTY")%>
                                        </td>
                                        <td>
                                            <%# Eval("TOverShortShip_PER")%>%
                                        </td>
                                        <td>
                                            <%# Eval("TLeftOverGarment_A")%>
                                        </td>
                                        <td>
                                            <%# Eval("TBGrade_QTY")%>
                                        </td>
                                        <td>
                                            <%# Eval("TCGrade_QTY")%>
                                        </td>
                                        <td>
                                            <%# Eval("TUnaccountable_QTY")%>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody> </table>
                                </FooterTemplate>
                            </asp:Repeater>
                             <hr />
                        </td>
                    </tr>
                    <tr>
                       
                    </tr>
                </table>
            </ItemTemplate>
            
        </asp:Repeater>
    </div>
    </form>
    <script type="text/javascript">
        $("#gvList td:nth-child(1)").addClass("bkColorG");
        // $("#rpList td:eq(1)").addClass("bkColorG");
        $("#btnQuery").click(function () {
            if ($("#DDSummaryBy").val() == "GO") {
                if ($("#txtGO").val() == "") {
                    alert("GO NO NOT EMPTY!");
                    return false;
                }
            }

            if ($("#txtGO").val() == "" && $("#txtJoNo").val() == "") {
                alert("GO/JO NOT EMPTY!");
                return false;
            }
        });
        //        $(".bkColorG").each(function () {
        //            if ($.trim($(this).html()) == "Scan_pack_Qty" || $.trim($(this).html()) == "Ship_Qty") {
        //                $(this).css("color", "red");
        //            }

        //        });
    </script>
</body>
</html>
