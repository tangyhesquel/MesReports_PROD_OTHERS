<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProTranWIPDetail.aspx.cs"
    Inherits="Mes_ProTranWIPDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>WIP Transafer Detail</title>
    <script src="../Script/Common.js" type="text/javascript"></script>
    <link rel="Stylesheet" href="../Css/StyleReport.css" />
</head>
<body style="width: 97%;">
    <form id="form1" runat="server">
    <div style="text-align: right">
        <input type="button" class="BUTTON" value="To Excel" onclick="toPaseExcel()" />&nbsp;&nbsp;<input
            type="button" class="BUTTON" value="To WPS" onclick="toPaseWPS()" />
    </div>
    <div id="ExcTable">
        <asp:GridView ID="gvWip" runat="server" Width="100%" AutoGenerateColumns="False"
            EnableViewState="false" EmptyDataText="<%$ Resources:GlobalResources,STRING_NO_DATE %>">
            <Columns>
                <asp:BoundField DataField="COLOR_CD" HeaderText="COLOR" />
                <asp:BoundField DataField="SIZE_CD" HeaderText="SIZE" />
                <asp:BoundField DataField="IN_BALANCE" HeaderText="WIP" />
                <asp:BoundField DataField="SHORT_NAME" HeaderText="BUYER SHORT NAME" />
                <asp:BoundField DataField="STYLE_NO" HeaderText="STYLE NO" />
                <asp:BoundField DataField="PRODUCTION_LINE_CD" HeaderText="LINE" />
            </Columns>
        </asp:GridView>
        <asp:GridView ID="gvProduction" runat="server" Width="100%" AutoGenerateColumns="true"
            EnableViewState="false" EmptyDataText="<%$ Resources:GlobalResources,STRING_NO_DATE %>">
        </asp:GridView>
    </div>
    </form>
</body>
</html>
