<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProLineDetail.aspx.cs" Inherits="Mes_ProLineDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>WIP Transfer by Production Line</title>
    <script src="../Script/Common.js" type="text/javascript"></script>
    <link rel="Stylesheet" href="../Css/StyleReport.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align: right">
        <input type="button" class="BUTTON" value="To Excel" onclick="toPaseExcel()" />&nbsp;&nbsp;<input
            type="button" class="BUTTON" value="To WPS" onclick="toPaseWPS()" />
    </div>
    <div id="ExcTable">
        <div id="divTotal" runat="server">
        </div>
        <asp:GridView ID="gvProduction" runat="server" Width="100%" AutoGenerateColumns="False"
            EnableViewState="False" EmptyDataText="No Data" OnRowDataBound="gvProduction_RowDataBound"
            ShowHeader="False">
            <Columns>
                <asp:BoundField DataField="production_line_cd" ItemStyle-Width="33%"></asp:BoundField>
                <asp:BoundField DataField="qty" ItemStyle-Width="33%"></asp:BoundField>
                <asp:BoundField DataField="sah_prod" ItemStyle-Width="33%"></asp:BoundField>
            </Columns>
        </asp:GridView>
    </div>
    </form>
</body>
</html>
