<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SearchStyle.aspx.cs" Inherits="Mes_SearchStyle" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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
                                Style No
                            </td>
                            <td>
                                <asp:TextBox ID="txtStyleNo" runat="server" Width="74px"></asp:TextBox>
                            </td>
                            <td>
                                Style Desc
                            </td>
                            <td>
                                <asp:TextBox ID="txtStyleDesc" runat="server" Width="104px"></asp:TextBox>
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
                    <asp:GridView ID="gvBody" runat="server" AutoGenerateColumns="False" Width="100%"
                        OnRowDataBound="gvBody_RowDataBound">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:CheckBox ID="cbCheck" runat="server" Checked='<%#Eval("checked").ToString()=="Y" ? true:false %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="STYLE_NO" />
                            <asp:BoundField DataField="STYLE_DESC" />
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
