<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PriceByFty_Edit.aspx.cs"
    Inherits="Mes_PriceByFty_Edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PriceByFty_Edit.aspx</title>
    <base target="_self" />
    <script language="javascript" type="text/javascript">

        function checknum() {
            if (event.shiftKey) event.returnValue = false;
            if ((event.keyCode >= 37 && event.keyCode <= 40) ||
                  (event.keyCode >= 48 && event.keyCode <= 57) ||
                  (event.keyCode >= 96 && event.keyCode <= 105) ||
                  event.keyCode == 190 || event.keyCode == 110 ||
                  event.keyCode == 8 || event.keyCode == 188 ||
                  event.keyCode == 46 || event.keyCode == 9 ||
                  event.keyCode == 109 || event.keyCode == 189) { return true };
            event.returnValue = false;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="100%" cellpadding="0" cellspacing="2">
            <tr>
                <td colspan="2">
                    <b>View</b>:<asp:Label ID="lbView" runat="server"></asp:Label>
                    <b>Year</b>:<asp:Label ID="lbYear" runat="server"></asp:Label>
                    <b>Fty:</b><asp:Label ID="lblFactory" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td align="right">
                    预算单价(RMB):
                </td>
                <td>
                    <asp:TextBox ID="txtPrice1" runat="server" onkeydown="javascript:checknum();"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    预算外发数量(Pcs):
                </td>
                <td>
                    <asp:TextBox ID="txtPrice2" runat="server" onkeydown="javascript:checknum();"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    预算单价/SAH:
                </td>
                <td>
                    <asp:TextBox ID="txtPrice3" runat="server" onkeydown="javascript:checknum();"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    预算平均SAH:
                </td>
                <td>
                    <asp:TextBox ID="txtPrice4" runat="server" onkeydown="javascript:checknum();"></asp:TextBox>
                </td>
            </tr>
            <tr align="center">
                <td colspan="2">
                    <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save" />
                    <asp:Button ID="btnCancel" runat="server" OnClientClick="window.close();return false;"
                        Text="Cancel" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
