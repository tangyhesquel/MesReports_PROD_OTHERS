<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SearchJONew.aspx.cs" Inherits="Reports_SearchJO" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script language="javascript" type="text/javascript">
        function chooseJo() {
            var cur = window.event.srcElement;
            window.returnValue = cur.value;
            window.close();
        }
    </script>
    <base target="_self" />
</head>
<body bgcolor="#FFFFFF" text="#000000">
    <form id="form1" runat="server">
    <div>
        <table width="100%">
            <tr>
                <td class="tr2style" id="SearchJONew">
                    S/C NO.
                </td>
                <td>
                    <asp:TextBox ID="txtSCNo" runat="server"></asp:TextBox>
                </td>
                <td class="tr2style">
                    Customer:
                </td>
                <td>
                    <asp:TextBox ID="txtCustomer" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="tr2style">
                    Translate Date.
                </td>
                <td>
                    <asp:TextBox ID="txtTrxDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                </td>
                <td class="tr2style">
                    Status:
                </td>
                <td>
                    <asp:DropDownList ID="ddlStatus" runat="server">
                        <asp:ListItem Value=""></asp:ListItem>
                        <asp:ListItem Value="Y">Yes</asp:ListItem>
                        <asp:ListItem Value="N">No</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="4" align="right">
                    <asp:Button ID="btnQuery" runat="server" CssClass="button_top" Text="Query" OnClick="btnQuery_Click" />
                </td>
            </tr>
        </table>
        <hr noshade size="1" />
        <br />
        <asp:GridView ID="gvBody" runat="server" Width="99%" AutoGenerateColumns="False"
            OnRowDataBound="gvBody_RowDataBound" CellPadding="4" ForeColor="#333333" GridLines="None">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <Columns>
                <asp:BoundField />
                <asp:BoundField DataField="job_order_no" HeaderText="Job Order NO." />
                <asp:BoundField DataField="sc_no" HeaderText="S/C NO." />
                <asp:BoundField DataField="customer" HeaderText="Customer" />
                <asp:BoundField DataField="trx_date" HeaderText="Transtion Date" />
                <asp:BoundField DataField="jo_clear_flag" HeaderText="Status" />
            </Columns>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
    </div>
    </form>
</body>
</html>
