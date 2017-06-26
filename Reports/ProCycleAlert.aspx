<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProCycleAlert.aspx.cs" Inherits="Reports_proCycleAlert" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Cycle Time Alert Report</title>
    <link rel="stylesheet" type="text/css" href="../StyleSheet.css" />
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        .style1
        {
            height: 8px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="divHeader" style="width: 100%; margin-right: 2px;" title="Search">
        <table id="tbHeader" border="0">
            <tr>
                <td class="style1" align="left">
                    Factory CD:&nbsp;
                    <asp:DropDownList ID="ddlFactory_cd" runat="server" Width="142px" AutoPostBack="True"
                        OnSelectedIndexChanged="ddlFactory_cd_SelectedIndexChanged">
                    </asp:DropDownList>
                    <!--Added By ZouShiChang ON 2013 08.29 Start MES 024 -->
                    &nbsp;Garment Type:
                    <asp:DropDownList ID="ddlGarment_type" runat="server" Width="142px" OnSelectedIndexChanged="ddlGarment_type_SelectedIndexChanged"
                        AutoPostBack="true">
                        <asp:ListItem>ALL</asp:ListItem>
                        <asp:ListItem>K</asp:ListItem>
                        <asp:ListItem>W</asp:ListItem>
                    </asp:DropDownList>
                    <!--Added By ZouShiChang ON 2013 08.29 End MES 024 -->
                    &nbsp;Process CD:
                    <asp:DropDownList ID="ddlProcess_cd" runat="server" Width="142px">
                    </asp:DropDownList>
                    <%-- Added By ZouShiChang ON 2013.09.24 Start MES024  --%>
                    &nbsp;Process Type:
                    <asp:DropDownList ID="ddlProcessType" runat="server">
                        <asp:ListItem></asp:ListItem>

                    </asp:DropDownList>
                    &nbsp;Production Factory:
                    <asp:DropDownList ID="ddlProdFactory" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <%-- Added By ZouShiChang ON 2013.09.24 End MES024  --%>
                   PPCD From:&nbsp;&nbsp;
                    <asp:TextBox ID="txtBeginDate" runat="server" Width="142px" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>&nbsp;&nbsp;
                    <asp:Button ID="btnQuery" runat="server" CssClass="button" Text="Query" OnClientClick="return CheckCDate();"
                        OnClick="btnQuery_Click" />
                    &nbsp;<asp:Button ID="btnExcel" runat="server" CssClass="button" Text="Excel" OnClick="btnExcel_Click" />
                    &nbsp;<asp:Button ID="btnWps" runat="server" CssClass="button" Text="Wps" OnClientClick="ToExcelOfWPS()" />
                </td>
            </tr>
            <tr>
                <td align="right" style="height: 26px">
                    &nbsp;
                </td>
            </tr>
        </table>
    </div>
    <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="Red" Text="Tip Message!"
        Visible="False" Width="300px"></asp:Label>
    <div id="divBody" style="width: 100%;" runat="server">
        <%--报表明细--%>
        <table width="100%" border="0" cellpadding="0" cellspacing="0" id="ExcTable">
            <tr>
                <td align="center">
                    <asp:Label ID="ReportTitle" runat="server" Style="font-size: x-large; font-weight: bold;
                        text-align: center;">Cycle Time Alert Report</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <table width="40%" border="1" cellpadding="0" cellspacing="0" style="text-align: left;
                        font-size: 9pt; border-collapse: collapse;">
                        <tr>
                            <td style="background-color: #F7F6F3">
                                Factory CD:
                            </td>
                            <td>
                                <asp:Label ID="lblFactoryID" runat="server" />
                            </td>
                            <td style="background-color: #F7F6F3">
                                Process CD:
                            </td>
                            <td>
                                <asp:Label ID="lblProcessCD" runat="server" />
                            </td>
                            <td style="background-color: #F7F6F3">
                                Begin Date:
                            </td>
                            <td>
                                <asp:Label ID="lblBeginDate" runat="server" />
                            </td>
                            <td style="background-color: #F7F6F3">
                                End Date:
                            </td>
                            <td>
                                <asp:Label ID="lblEndDate" runat="server" />
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
                    <asp:GridView ID="gvBody" runat="server" AutoGenerateColumns="true" CellPadding="4"
                        EnableViewState="false" ForeColor="#333333" GridLines="Both" OnRowDataBound="gvBody_RowDataBound"
                        Width="100%">
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
    </div>
    </form>
    <script language="javascript" type="text/javascript">
        function CheckCDate() {
            if (window.document.getElementById("<%=txtBeginDate.ClientID %>").value == "") {
                alert("please select date!");
                return false;
            }
            return true;
        }
    </script>
</body>
</html>
