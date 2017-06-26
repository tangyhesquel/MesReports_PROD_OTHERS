<%@ Page Language="C#" AutoEventWireup="true" CodeFile="proCycleDailyDetail.aspx.cs"
    Inherits="Reports_proCycleDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Cycle Time Daily Detail Report</title>
    <link rel="stylesheet" type="text/css" href="../StyleSheet.css" />
    <script type="text/javascript" src="../Script/Common.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="ExcTable" style="text-align: center" runat="server">
        <asp:Label ID="ReportTitle" runat="server" Style="font-size: x-large; font-weight: bold;
            text-align: center;">Cycle Time Daily Detail Report</asp:Label>
        <br />
        <div style="text-align: right">
            <asp:Button ID="btnExcel" runat="server" CssClass="button" Text="Excel" OnClick="btnExcel_Click" />
            &nbsp:<asp:Button ID="btnWps" runat="server" CssClass="button" Text="Wps" OnClientClick="ToExcelOfWPS()" />
        </div>
        <br />
        <table width="100%" cellpadding="0" cellspacing="0" border="1" style="text-align: left;
            font-size: 9pt; border-collapse: collapse;">
            <tr>
                <td style="background-color: #F7F6F3">
                    JOB_ORDER_NO
                </td>
                <td>
                    <asp:Label ID="lblJobOrderNo" runat="server"></asp:Label>
                </td>
                <td style="background-color: #F7F6F3">
                    Factory CD:
                </td>
                <td>
                    <asp:Label ID="lblFactoryCd" runat="server"></asp:Label>
                </td>
                <td style="background-color: #F7F6F3">
                    Process Code:
                </td>
                <td>
                    <asp:Label ID="lblProcessCd" runat="server"></asp:Label>
                </td>
                <td style="background-color: #F7F6F3">
                    Start Date:
                </td>
                <td>
                    <asp:Label ID="lblStartDate" runat="server"></asp:Label>
                </td>
                <td style="background-color: #F7F6F3">
                    End Date:
                </td>
                <td>
                    <asp:Label ID="lblEndDate" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
        <br />
        <asp:GridView ID="gvDetail" runat="server" AutoGenerateColumns="true" EnableViewState="false"
            ForeColor="#333333" GridLines="Both" Width="100%" OnRowDataBound="gvBody_RowDataBound">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" BorderWidth="1px" BorderStyle="Solid"
                Font-Size="8" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Font-Size="10" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
        <br />
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
    </form>
</body>
</html>
