<%@ Page Language="C#" AutoEventWireup="true" CodeFile="queryContract.aspx.cs" Inherits="Reports_queryContract" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../My97DatePicker/WdatePicker.js"></script>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <base target="_self" />
    <script language="javascript">
        function returnResult(contractNo) {
            window.returnValue = contractNo;
            window.close();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="100%">
            <tr>
                <td>
                    Contract No:
                </td>
                <td>
                    <asp:TextBox ID="txtcontractNo" runat="server"></asp:TextBox>
                </td>
                <td>
                    Subcontractor:
                </td>
                <td>
                    <asp:DropDownList ID="ddlSubcontractor" runat="server" DataTextField="SUBCONTRACTOR_CD"
                        DataValueField="SUBCONTRACTOR_CD">
                    </asp:DropDownList>
                </td>
                <td>
                    Issue Date From:
                </td>
                <td>
                    <asp:TextBox ID="txtfromDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                </td>
                <td>
                    Issue Date To:
                </td>
                <td>
                    <asp:TextBox ID="txtToDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                </td>
                <td>
                    <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnQuery_Click" CssClass="BUTTON" />
                </td>
            </tr>
            <tr>
                <td>
                    JO No:
                </td>
                <td>
                    <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                </td>
                <td>
                    Factory:
                </td>
                <td>
                    <asp:DropDownList ID="ddlfactoryCd" runat="server" DataTextField="DEPARTMENT_ID"
                        DataValueField="DEPARTMENT_ID">
                    </asp:DropDownList>
                </td>
                <td>
                    &nbsp;
                </td>
                <td>
                    &nbsp;
                </td>
                <td>
                    &nbsp;
                </td>
                <td>
                    &nbsp;
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
        </table>
        <br />
        <asp:GridView ID="gvBody" runat="server" AutoGenerateColumns="False" CellPadding="4"
            ForeColor="#333333" GridLines="None" OnRowDataBound="gvBody_RowDataBound" Width="100%"
            ShowHeader="False">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <Columns>
                <asp:BoundField DataField="contract_no" />
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
