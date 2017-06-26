<%@ Page Language="C#" AutoEventWireup="true" CodeFile="JoEntryListSummary.aspx.cs"
    Inherits="JoEntryListSummary" %>

<%@ Register Src="~/UserControls/ExportDataUserControl.ascx" TagPrefix="mes" TagName="ExportData" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>JO entry list summary report</title>
    <link rel="stylesheet" type="text/css" href="../StyleSheet.css" />
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link rel="Stylesheet" href="../Css/StyleReport.css" />
    <style type="text/css">
        .style1
        {
            height: 8px;
        }
    </style>
    <style type="text/css" media="print">
        .Noprint
        {
            display: none;
        }
    </style>
    <object id="WebBrowser" classid="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2" height="0"
        width="0">
    </object>
</head>
<body>
    <form id="form1" runat="server">
    <div id="div1" style="width: 100%; margin-right: 2px;" title="Search">
        <fieldset>
            <legend>Search</legend>
            <div id="divHeader" style="width: 100%; margin-right: 2px;" title="Search" class="Noprint">
                <table id="tbHeader" border="0" width="100%">
                    <tr>
                        <td class="tdbackcolor" align="left" style="width: 4em">
                            工厂：
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlFactory_cd" runat="server" DataTextField="DEPARTMENT_ID"
                                DataValueField="DEPARTMENT_ID" Enabled="false">
                            </asp:DropDownList>
                        </td>
                        <td class="tdbackcolor" style="width: 7em">
                            类型(针/梭)：
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlType" runat="server" Width="120px" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td class="tdbackcolor" style="width: 4em">
                            部门：
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlProcess_cd" runat="server" DataTextField="SHORT_NAME" DataValueField="PROCESS_CD">
                            </asp:DropDownList>
                        </td>
                        <%-- Added By ZouShiChang ON 2013.09.23 Start MES024 --%>
                        <td class="tdbackcolor" style="width: 6em">
                            部门类型：
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlProcessType" runat="server" >
                                <asp:ListItem></asp:ListItem>

                            </asp:DropDownList>
                        </td>
                        <td class="tdbackcolor" style="width: 6em">
                            生产工厂：
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlProdFactory" runat="server" >                                
                            </asp:DropDownList>
                        </td>
                        <%-- Added By ZouShiChang ON 2013.09.23 Start MES024 --%>
                        <td class="tdbackcolor" style="width: 8em">
                            Group Name：
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlGroupName" runat="server" Enabled="false">
                                <asp:ListItem Value="">&nbsp;</asp:ListItem>
                                <asp:ListItem Value="YM梭织车缝1">梭织车缝1(W17 - W29)</asp:ListItem>
                                <asp:ListItem Value="YM梭织车缝2">梭织车缝2(W11 - W16)</asp:ListItem>
                                <asp:ListItem Value="YM梭织车缝3">梭织车缝3(W1 - W10)</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdbackcolor" style="width: 6em">
                            起始日期：
                        </td>
                        <td>
                            <asp:TextBox ID="txtBeginDate" runat="server" Width="120px" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                        </td>
                        <td class="tdbackcolor" style="width: 6em">
                            结束日期：
                        </td>
                        <td>
                            <asp:TextBox ID="txtEndDate" runat="server" Width="120px" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                        </td>
                        <td class="tdbackcolor" style="width: 4em">
                            状态:
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownListStatus" runat="server" Width="6em">
                                <asp:ListItem Text="全部" Value="A" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="确认" Value="Y"></asp:ListItem>
                                <asp:ListItem Text="提交" Value="S"></asp:ListItem>
                                <asp:ListItem Text="草稿" Value="N"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td colspan="2" align="right">
                            <asp:Button ID="btnQuery" runat="server" CssClass="button" Text="查询" OnClick="btnQuery_Click" />
                        </td>
                    </tr>
                </table>
            </div>
        </fieldset>
    </div>
    <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="Red" Text="Tip Message!"
        Visible="False" Width="300px"></asp:Label>
    <table width="100%" cellpadding="0" cellspacing="0" id="ExcTable">
        <tr align="center">
            <td align="center">
                <asp:Label ID="ReportTitle" runat="server" Style="font-size: x-large; font-weight: bold;
                    text-align: center;">JO entry list summary report</asp:Label>
            </td>
        </tr>
        <tr>
            <td align="right">
                <mes:ExportData ID="dataExport" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblFactory" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                <%--报表明细--%>
                <asp:GridView ID="gvBody" runat="server" AutoGenerateColumns="true" CellPadding="4"
                    EnableViewState="false" ForeColor="#333333" GridLines="Both" OnRowDataBound="gvBody_RowDataBound"
                    Width="100%" ShowFooter="True">
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
    <div id="divMsg" runat="server">
    </div>
    </form>
</body>
</html>
