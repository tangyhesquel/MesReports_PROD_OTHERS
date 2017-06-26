<%@ Page Language="C#" AutoEventWireup="true" CodeFile="proCycleDetail.aspx.cs" Inherits="Reports_proCycleDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//Dtd XHTML 1.0 transitional//EN" "http://www.w3.org/tr/xhtml1/Dtd/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Production Cycle Time Detail Report</title>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        BODY
        {
            font-size: 11px;
            padding: 5px;
            font-family: Arial,Times New Roman, 黑体,Verdana,Helvetica, sans-serif;
        }
        A:link
        {
            color: #000000;
            text-decoration: none;
        }
        A:active
        {
            color: #000000;
            text-decoration: none;
        }
        A:visited
        {
            color: #000000;
            text-decoration: none;
        }
        A:hover
        {
            color: #000000;
            text-decoration: none;
        }
        A:hover
        {
            left: 1px;
            border-bottom: 1px dotted;
            position: relative;
            top: 1px;
        }
        .BUTTON_Gary
        {
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            cursor: hand;
            background-color: #efefef;
            border: 1px solid #333333;
        }
        .BUTTON
        {
            font-weight: bolder;
            font-size: 11px;
            padding: 1px;
            padding-bottom: 2px;
            text-transform: uppercase;
            cursor: hand;
            height: 20px;
            color: #646400;
            background-color: #CCCC99;
            border: 1px solid #333333;
        }
        .BUTTON_down
        {
            font-weight: bolder;
            font-size: 11px;
            padding: 1px;
            padding-bottom: 2px;
            text-transform: uppercase;
            cursor: hand;
            height: 20px;
            color: #646400;
            background-color: #efefe7;
            border: 1px solid #646400;
        }
        .input_gary
        {
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            background-color: #efefef;
            border: 1px solid #333333;
        }
        .input_color
        {
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            background-color: #EEF7FF;
            border: 1px solid #333333;
        }
        .input_white
        {
            padding-left: 2px;
            font-size: 11px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            height: 18px;
            border: 1px solid #333333;
        }
        INPUT
        {
            padding-left: 2px;
            font-size: 11px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        TABLE
        {
            padding-right: 0px;
            padding-left: 0px;
            font-size: 11px;
            padding-bottom: 0px;
            padding-top: 0px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        .tr1style
        {
            font-size: 11px;
            background-color: #ffffff;
            padding-left: 5px;
            padding-right: 5px;
            padding-top: 2px;
        }
        .tr2style
        {
            font-weight: bolder;
            font-size: 11px;
            background-color: #efefe7;
            padding-left: 5px;
            padding-right: 5px;
            padding-top: 2px;
        }
        .tr3style
        {
            font-weight: bolder;
            font-size: 11px;
            background-color: #ffffff;
            padding-left: 5px;
            padding-top: 2px;
        }
        .tr4style
        {
            font-weight: bolder;
            font-size: 11px;
            width: 20em;
            background-color: #efefe7;
            padding-left: 5px;
            padding-right: 5px;
            padding-top: 2px;
        }
        .bigfont
        {
            font-weight: bolder;
            font-size: 18px;
            color: #736d00;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        .bigfont1
        {
            font-weight: bolder;
            font-size: 14px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        .red
        {
            font-weight: 600;
            color: #800000;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        .footer
        {
            font-size: 11px;
            color: #000000;
            line-height: 20px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            background-color: #efefef;
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
        .style1
        {
            width: 168px;
        }
        .style3
        {
            background-color: #EFEFE0;
            width: 129px;
        }
        .style5
        {
            background-color: #EFEFE0;
            width: 168px;
        }
    </style>
    <script type="text/javascript" src="../Script/Common.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search </legend>
            <table width="100%" id="queryTab" align="center">
                <tr>
                    <td width="8%" height="19" class="tdbackcolor">
                        Factory Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFactoryCode" runat="server" Enabled="false">
                        </asp:DropDownList>
                    </td>
                    <td width="5%" height="19" class="tdbackcolor">
                        Garment Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlGarmentType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlGarmentType_SelectedIndexChanged">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="W">Woven</asp:ListItem>
                            <asp:ListItem Value="K">Knit</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td width="7%" height="19" class="tdbackcolor">
                        Process Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProcessCode" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlProcessCode_SelectedIndexChanged"
                            DataTextField="SHORT_NAME" DataValueField="PROCESS_CD">
                        </asp:DropDownList>
                    </td>
                    <%-- Added By ZouShiChang ON 2013.09.24 Start MES024  --%>
                    <td align="center" class="tdbackcolor">
                        Process Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProcessType" runat="server">
                            <asp:ListItem></asp:ListItem>
              
                        </asp:DropDownList>
                    </td>
                    <td align="center" class="tdbackcolor">
                        Production Factory:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProdFactory" runat="server">
                        </asp:DropDownList>
                    </td>
                    <%-- Added By ZouShiChang ON 2013.09.24 End MES024  --%>
                    <td height="19" class="tdbackcolor">
                        Production Line :
                    </td>
                    <td class="style1">
                        <asp:DropDownList ID="ddlProductionLine" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td height="19" class="style3">
                        Wash Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlWashType" runat="server">
                            <asp:ListItem></asp:ListItem>
                            <asp:ListItem Value="NOIRON">WRINKLE FREE</asp:ListItem>
                            <asp:ListItem Value="IRON">No WRINKLE FREE</asp:ListItem>
                            <asp:ListItem Value="WASH">Wash</asp:ListItem>
                            <asp:ListItem Value="NW">No Wash</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td height="19" class="tdbackcolor">
                        OutSource Type
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlOutSource" runat="server">
                            <asp:ListItem Text="All" Value=""></asp:ListItem>
                            <asp:ListItem Text="OutSource" Value="OUTSOURCE"></asp:ListItem>
                            <asp:ListItem Text="Standard" Value="STANDARD"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td width="8%" height="19" class="tdbackcolor">
                        Start Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtStartDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"
                            Width="75px"></asp:TextBox>
                    </td>
                    <td width="8%" height="19" class="tdbackcolor">
                        End Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtEndDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"
                            Width="75px"></asp:TextBox>
                    </td>
                    <td height="19" class="tdbackcolor">
                        Group Name
                    </td>
                    <td class="style1">
                        <asp:DropDownList ID="ddGroupName" runat="server" Enabled="false">
                            <asp:ListItem Value="">&nbsp;</asp:ListItem>
                            <asp:ListItem Value="YM梭织车缝1">梭织车缝1(W17 - W29)</asp:ListItem>
                            <asp:ListItem Value="YM梭织车缝2">梭织车缝2(W11 - W16)</asp:ListItem>
                            <asp:ListItem Value="YM梭织车缝3">梭织车缝3(W1 - W10)</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="tdbackcolor" colspan="5">
                        <%--Added by Jin Song MES-115--%>
                        <asp:CheckBox ID="cbDetail" runat="server" Text="Detail Data" Checked="true" />
                        <asp:CheckBox ID="cbSummary" runat="server" Text="Summary Data" Checked="true" />
                        <%--End MES115--%>
                        &nbsp;
                    </td>
                    <td class="tdbackcolor">
                        <asp:CheckBox ID="cbCheck" runat="server" Text="After 1 Mar" />
                    </td>
                    <td class="tdbackcolor">
                        <asp:CheckBox ID="CheckBoxByProdLine" runat="server" Text="By Prod Line" />
                    </td>
                    <td class="style5">
                        <asp:CheckBox ID="CbLineNm" runat="server" Text="Show Line Name" />
                    </td>
                    <td class="style3">
                        &nbsp;
                        <asp:CheckBox ID="CbProcessCloseStatus" runat="server" Text="Show Process Close Status" />
                    </td>
                    <td align="right">
                        <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade size="1" />
        <div style="text-align:center">
            <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="Red" Text="Tip Message!" 
                Visible="False" Width="300px" Font-Size="Medium"></asp:Label>
        </div>
        <div id="ExcTable" runat="server">
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <font face="Arial" size="4">Production Cycle Time Detail Report</font>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <div id="mmPrint" align="right" style="width: 100%">
                            <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv.style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv.style.display='block';document.all.mmPrint.style.visibility='visible'"
                                style="font-size: 16; width: 80; height: 26" />
                            <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,'Production Cycle Time Detail Report.htm')"
                                style="font-size: 16; width: 80; height: 26" />
                            <input type="button" name="ToExcel" value="ToExcel" onclick="toPaseExcel()" style="font-size: 16;
                                width: 80; height: 26" />
                            <input type="button" name="ToWps" value="ToWps" onclick="ToExcelOfWPS()" style="font-size: 16;
                                width: 80; height: 26" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <table id="allTable" width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                            border-collapse: collapse">
                            <tr>
                                <td colspan="12">
                                    <table width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                                        border-collapse: collapse">
                                        <tr>
                                            <td class="tr2style" bgcolor="#efefe7" width="100">
                                                Factory CD:
                                            </td>
                                            <td>
                                                <%=ddlFactoryCode.SelectedItem.Text %>&nbsp;
                                            </td>
                                            <td class="tr2style" bgcolor="#efefe7" width="100">
                                                Garment Type:
                                            </td>
                                            <td>
                                                <%=ddlGarmentType.SelectedItem.Text%>&nbsp;
                                            </td>
                                            <td class="tr2style" bgcolor="#efefe7" width="100">
                                                Process Code:
                                            </td>
                                            <td>
                                                <%=ddlProcessCode.SelectedItem.Text %>&nbsp;
                                            </td>
                                            <td class="tr2style" bgcolor="#efefe7" width="100">
                                                Production Line:
                                            </td>
                                            <td>
                                                <%=ddlProductionLine.SelectedItem.Text %>&nbsp;
                                            </td>
                                            <td class="tr2style" bgcolor="#efefe7" width="100">
                                                Wash Type:
                                            </td>
                                            <td>
                                                <%=ddlWashType.SelectedItem.Text %>&nbsp;
                                            </td>
                                            <td class="tr2style" bgcolor="#efefe7" width="100">
                                                Start Date:
                                            </td>
                                            <td>
                                                <%=txtStartDate.Text %>&nbsp;
                                            </td>
                                            <td class="tr2style" bgcolor="#efefe7" width="100">
                                                End Date:
                                            </td>
                                            <td>
                                                <%=txtEndDate.Text %>&nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <table width="100%" cellpadding="0" cellspacing="0" id="Table1">
                <tr>
                    <td>
                        <%--报表明细--%>
                        <asp:GridView ID="gvBody" runat="server" AutoGenerateColumns="true" CellPadding="4"
                            EnableViewState="false" ForeColor="#333333" GridLines="Both" OnRowDataBound="gvBody_RowDataBound"
                            Width="100%" ShowHeader="true" ShowFooter="True" Visible="false">
                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" BorderWidth="1px" BorderStyle="Solid" />
                            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Font-Size="10" />
                            <EditRowStyle BackColor="#999999" />
                            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                            <RowStyle Font-Size="8" />
                        </asp:GridView>
                    </td>
                </tr>
            </table>
            <%--Modified by Jin Song MES115 Change visible to false--%>
            <div id="dvTotal" runat="server" visible="false">
                <table width="100%" cellpadding="0" cellspacing="0" border="1" style="text-align: left;
                    font-size: 9pt; border-collapse: collapse;">
                    <tr>
                        <td rowspan="2" style="background-color: #F7F6F3">
                            Total:
                        </td>
                        <td style="background-color: #F7F6F3">
                            Total Wip Qty
                        </td>
                        <td style="background-color: #F7F6F3">
                            Total Out Qty
                        </td>
                        <td style="background-color: #F7F6F3">
                            CT
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblTotalWipQty" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblTotalOutQty" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblCT" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
            <br />
            <table width="100%" cellpadding="0" cellspacing="0" id="Table2">
            <tr>
                    <td>
                        <asp:Label ID="lblTitle" runat="server" Text="Summary" Visible="false" Font-Bold="true" Font-Size="10"></asp:Label>
                    </td></tr>
                <tr>
                    <td>
                        <%--报表明细--%>
                        <asp:GridView ID="gvDct" runat="server" AutoGenerateColumns="true" CellPadding="4"
                            EnableViewState="false" ForeColor="#333333" GridLines="Both" OnRowDataBound="gvBody_RowDataBoundDct"
                            ShowHeader="true" ShowFooter="false">
                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" BorderWidth="1px" BorderStyle="Solid" />
                            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Font-Size="10" />
                            <EditRowStyle BackColor="#999999" />
                            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                            <RowStyle Font-Size="8" />
                        </asp:GridView>
                    </td>
                </tr>
            </table>
            <br />
            <table width="100%" cellpadding="0" cellspacing="0" id="Table3">
                <tr>
                    <td>
                        <asp:Label ID="lblTitle1" runat="server" Text="Summary (by Production Line)" Visible="false" Font-Bold="true" Font-Size="10"></asp:Label>
                    </td></tr>
                <tr>
                    <td>
                        <asp:GridView ID="gvSum" runat="server" AutoGenerateColumns="true" CellPadding="4"
                            EnableViewState="false" ForeColor="#333333" GridLines="Both"
                            ShowHeader="true" ShowFooter="false" Visible="false">
                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" BorderWidth="1px" BorderStyle="Solid" />
                            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Font-Size="10" Wrap="true" />
                            <EditRowStyle BackColor="#999999" />
                            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                            <RowStyle Font-Size="8" />
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>

