<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MasOutSourcePrice.aspx.cs"
    Inherits="MES_MasOutSourcePrice" %>

<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:HiddenField ID="hfYear" runat="server" />
        View:<asp:DropDownList ID="ddlView" runat="server" AutoPostBack="true">
        </asp:DropDownList>
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
    <script language="javascript">
        var frmMain = document.forms[0];
        function gdDBClick(strYear) {
            var dialogFeatures = "status:no;help:no;dialogHeight:260px;dialogWidth:400px;scroll:off";
            var formUrl = "MasOutSourcePrice_Form.aspx?year=" + strYear + "&viewtype=" + document.getElementById("<%= ddlView.ClientID%>").value;

            var retr = window.showModalDialog(formUrl, "", dialogFeatures);
            if (retr != undefined) {
                if (retr == 'change') {
                    frmMain.submit();
                }
            }
        }
        function SetSelYear(strYear) {
            document.getElementById("<%= hfYear.ClientID%>").value
        }
    </script>
</body>
</html> 