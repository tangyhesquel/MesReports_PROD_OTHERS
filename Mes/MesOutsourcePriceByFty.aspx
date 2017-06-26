<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MesOutsourcePriceByFty.aspx.cs"
    Inherits="Mes_MesOutsourcePriceByFty" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MesOutsourcePriceByFty</title>
    <script language="javascript" type="text/javascript">
        function gdDBClick(strYear) {
            var dialogFeatures = "status:no;help:no;dialogHeight:260px;dialogWidth:400px;scroll:off";
            var index = document.getElementById("ddlFactory").selectedIndex;
            var formUrl = "PriceByFty_Edit.aspx?year=" + strYear + "&viewtype=" + document.getElementById("<%= ddlView.ClientID%>").value + "&Factory_cd=" + document.getElementById("<%=ddlFactory.ClientID%>").value + "&Factory=" + document.getElementById("<%=ddlFactory.ClientID%>").options[index].text;

            var retr = window.showModalDialog(formUrl, "", dialogFeatures);
            if (retr != undefined) {
                if (retr == 'change') {
                    document.forms[0].submit();
                }
            }
        }
        function SetSelYear(strYear) {
            document.getElementById("<%= hfYear.ClientID%>").value
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:HiddenField ID="hfYear" runat="server" />
        Type:&nbsp;<asp:DropDownList ID="ddlView" runat="server" AutoPostBack="true">
        </asp:DropDownList>
        &nbsp;&nbsp;Code:&nbsp;<asp:DropDownList ID="ddlFactory" runat="server" AutoPostBack="True">
        </asp:DropDownList>
        <%--&nbsp;&nbsp;Description:&nbsp;<asp:TextBox ID="txtDes" runat="server"></asp:TextBox>--%>
        <asp:GridView ID="gdPrice" runat="server" AutoGenerateColumns="true" CellPadding="4"
            ForeColor="#333333" Width="100%" OnRowDataBound="gdPrice_RowDataBound">
            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            <RowStyle ForeColor="#000066" />
            <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
            <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
        </asp:GridView>
    </div>
    </form>
</body>
</html>
