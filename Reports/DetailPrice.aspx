<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DetailPrice.aspx.cs" Inherits="Reports_DetailPrice" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DetailPrice</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:GridView ID="gvDetail" runat="server" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="TRIM_ID" HeaderText="TRIM_ID" />
                <asp:BoundField DataField="BOM_NO" HeaderText="BOM NO" />
                <asp:BoundField DataField="SC_NO" HeaderText="SC NO" />
                <asp:BoundField DataField="STYLE_NO" HeaderText="STYLE NO" />
                <asp:BoundField DataField="CUSTOMER" HeaderText="CUSTOMER" />
                <asp:BoundField DataField="PRODUCT_CATEGORY" HeaderText="PRODUCT CATEGORY" />
                <asp:BoundField DataField="PRODUCT_CLASS" HeaderText="PRODUCT CLASS" />
                <asp:BoundField DataField="SUPP_REF_NO" HeaderText="SUPP_REF_NO" />
                <asp:BoundField DataField="PRODUCT_OTHER_DESC" HeaderText="PRODUCT OTHER DESC" />
                <asp:BoundField DataField="SUPPLIER" HeaderText="SUPPLIER" />
                <asp:BoundField DataField="QUANTITY" HeaderText="QUANTITY" />
                <asp:BoundField DataField="PERXGMT" HeaderText="PERXGMT" />
                <asp:BoundField DataField="CCY_CD" HeaderText="CCY CD" />
                <asp:BoundField DataField="UNIT_PRICE" HeaderText="UNIT PRICE" />
                <asp:BoundField DataField="CONVERSION_RATE" HeaderText="CONVERSION RATE" />
                <asp:BoundField DataField="UOM_CD" HeaderText="UOM CD" />
                <asp:BoundField DataField="DEFAULT_FLAG" HeaderText="DEFAULT FLAG" />
            </Columns>
        </asp:GridView>
    </div>
    </form>
</body>
</html>
