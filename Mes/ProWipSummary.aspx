<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProWipSummary.aspx.cs" Inherits="Mes_ProWipSummary" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <link rel="Stylesheet" href="../Css/StyleReport.css" />
    <script src="../Script/Common.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="hfValue" runat="server" />
    <div style="text-align: right">
        <input type="button" class="BUTTON" value="To Excel" onclick="toPaseExcel()" />&nbsp;&nbsp;<input
            type="button" class="BUTTON" value="To WPS" onclick="toPaseWPS()" /></div>
    <div id="ExcTable">
        <asp:GridView ID="gvBody" runat="server" Width="100%" OnRowDataBound="gvBody_RowDataBound"
            BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
            CellPadding="3" EnableViewState="False">
            <RowStyle ForeColor="#000066" />
            <FooterStyle BackColor="White" ForeColor="#000066" />
            <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
            <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
        </asp:GridView>
    </div>
    </form>
</body>
</html>
