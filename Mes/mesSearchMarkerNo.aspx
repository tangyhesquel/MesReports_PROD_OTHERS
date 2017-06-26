<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mesSearchMarkerNo.aspx.cs"
    Inherits="Mes_searchMarkerNo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Search Marker NO</title>
    <base target="_self"></base>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script language="javascript">
        function chooseMarkerNo() {
            var cur = window.event.srcElement;
            window.returnValue = cur.value;
            window.close();
        }

        function validate() {

            if (searchMoNo.txtMoNo.value == '') {
                if (searchMoNo.txtJoNo.value == '') {

                    alert('Please choose the Marker No or Jo No!');
                    return false;
                }
            }
        }
    </script>
</head>
<body>
    <form id="searchMoNo" runat="server">
    <table width="90%" border="0" cellpadding="0" cellspacing="1" align="center">
        <tr>
            <td width="10%" height="19" class="tdbackcolor">
                Marker No:
            </td>
            <td>
                <asp:TextBox ID="txtMoNo" runat="server"></asp:TextBox>
            </td>
            <td width="10%" height="19" class="tdbackcolor">
                Jo_No:
            </td>
            <td>
                <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btnQuery" Text="Query" runat="server" CssClass="button_top" OnClientClick="validate()"
                    OnClick="btnQuery_Click" />
            </td>
        </tr>
    </table>
    <hr noshade size="1">
    <div id="reportDiv">
        <asp:GridView ID="gvBody" runat="server" Width="100%" AutoGenerateColumns="False"
            OnRowDataBound="gvBody_RowDataBound">
            <Columns>
                <asp:BoundField />
                <asp:BoundField DataField="MO_NO" HeaderText="Marker No" />
                <asp:BoundField DataField="GO_NO" HeaderText="Go No" />
            </Columns>
        </asp:GridView>
    </div>
    </form>
</body>
</html>
