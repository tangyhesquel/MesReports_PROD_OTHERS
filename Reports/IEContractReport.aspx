<%@ Page Language="C#" AutoEventWireup="true" CodeFile="IEContractReport.aspx.cs"
    Inherits="IEContractReport" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>IEContractReport</title>
    <link rel="stylesheet" type="text/css" href="../StyleSheet.css" />
    <script src="../Script/jquery-1.4.2.js" type="text/javascript"></script>
    <script src="../Script/jquery.autocomplete.js" type="text/javascript"></script>
    <script src="../Script/jquery.autocomplete.min.js" type="text/javascript"></script>
    <link href="../Script/jquery.autocomplete.css" rel="stylesheet" type="text/css" />
    <script src="../Script/Common.js" type="text/javascript"></script>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        DoAutoComplete("#txtContractNO", "IEContractReport.aspx");
    </script>
    <style type="text/css">
        .style1
        {
            height: 8px;
        }
        .style6
        {
            height: 21px;
        }
    </style>
</head>
<body class="BodyBackColor">
    <form id="form1" runat="server">
    <div id="divHeader" style="width: 100%; margin-right: 2px;" title="Search">
        <table id="tbHeader" border="0">
            <%--<tr>
                <td class="style1" align="left">
                    <asp:Label ID="Title" runat="server" Text="IE单次合同发货情况统计" Font-Bold="True" ForeColor="Maroon"></asp:Label>
                </td>
            </tr>--%>
            <tr>
                <td class="style1" align="left">
                    Contract NO:<asp:TextBox ID="txtContractNO" runat="server" Width="142px"></asp:TextBox>
                    &nbsp;&nbsp;&nbsp;&nbsp; Contractor:
                    <asp:DropDownList ID="ddlContractor" runat="server">
                    </asp:DropDownList>
                    <%--<asp:TextBox ID="txtContractor" runat="server" Width="142px"></asp:TextBox>--%>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Transaction Date From
                    <asp:TextBox ID="txtBeginDate" runat="server" Width="142px" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>&nbsp;To
                    <asp:TextBox ID="txtEndDate" runat="server" Width="142px" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>&nbsp;
                    Report:<asp:DropDownList ID="ddlReport" runat="server">
                    </asp:DropDownList>
                    <asp:Button ID="btnQuery" runat="server" CssClass="button" Text="Query" OnClick="btnQuery_Click" />
                    &nbsp;<asp:Button ID="btnExcel" runat="server" CssClass="button" Text="Excel" OnClick="btnExcel_Click" />
                    &nbsp;<asp:Button ID="btnWps" runat="server" Text="To Wps" OnClientClick="ToExcelOfWPS()"
                        CssClass="button" />
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
        Visible="False" Width="185px"></asp:Label>
    <div id="ExcTable">
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr align="center">
                <td align="center">
                    <asp:Label ID="ReportTitle" runat="server" Style="font-size: x-large; font-weight: bold;
                        text-align: center;"></asp:Label>
                </td>
            </tr>
        </table>
        <br />
        <%--报表明细--%>
        <asp:GridView ID="gvBody" runat="server" AutoGenerateColumns="true" CellPadding="4"
            ForeColor="#333333" GridLines="Both" OnRowDataBound="gvBody_RowDataBound">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" BorderWidth="1px" BorderStyle="Solid" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Font-Size="10" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <RowStyle Font-Size="8" />
        </asp:GridView>
        <br />
        <%--报表明细--%>
        <asp:GridView ID="gvIssue" runat="server" AutoGenerateColumns="true" CellPadding="4"
            ForeColor="#333333" GridLines="Both" OnRowDataBound="gvIssue_RowDataBound">
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Font-Size="10" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <RowStyle Font-Size="8" />
        </asp:GridView>
        <br />
        <div id="div2" style="width: 100%;">
            <%--报表明细--%>
            <asp:GridView ID="gvPPO" runat="server" AutoGenerateColumns="true" CellPadding="4"
                ForeColor="#333333" GridLines="Both" OnRowDataBound="gvPPO_RowDataBound">
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" BorderWidth="1px" BorderStyle="Solid" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Font-Size="10" />
                <EditRowStyle BackColor="#999999" />
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <RowStyle Font-Size="8" />
            </asp:GridView>
        </div>
    </div>
    </form>
</body>
</html>
