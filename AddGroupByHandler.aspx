<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddGroupByHandler.aspx.cs"
    Inherits="Reports_AddGroupByHandler" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>By Handler</title>
    <link href="StyleSheet.css" rel="Stylesheet" />
    <base target="_self" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table>
            <tr>
                <td>
                    Group Name：<asp:TextBox runat="server" ID="txtGroupName" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView runat="server" ID="gvBody" AutoGenerateColumns="False" OnRowDataBound="gvBody_RowDataBound">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:CheckBox ID="cbHandler" runat="server" Text='<%#Eval("MaintMan_Mail") %>' Checked='<%#Eval("checked").ToString()=="1" ? true:false %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="maintman" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                    <asp:Button runat="server" Text="SAVE" CssClass="button" OnClick="btnSave_Click" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
