<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="Mes_Search" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <base target="_self" />
    <link rel="Stylesheet" href="../StyleSheet.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="100%">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblCode" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtCode" runat="server" Width="74px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="lblName" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtName" runat="server" Width="104px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="lblMark" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtMark" runat="server" Width="121px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Button ID="btnQuery" runat="server" Text="Search" CssClass="button" OnClick="btnQuery_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView ID="gvBody" runat="server" OnRowDataBound="gvBody_RowDataBound" AutoGenerateColumns="False"
                        Width="100%">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:CheckBox ID="cbCheck" runat="server" Checked='<%#Eval("checked").ToString()=="Y" ? true:false %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="code" />
                            <asp:BoundField DataField="name" />
                            <asp:BoundField DataField="mark" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Button ID="btnSelect" runat="server" Text="Select" CssClass="button" Height="19px"
                        Width="108px" OnClick="btnSelect_Click" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
