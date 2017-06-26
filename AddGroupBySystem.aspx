<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddGroupBySystem.aspx.cs" Inherits="Reports_AddGroupBySystem" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>By System</title>
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
                    <asp:GridView runat="server" ID="gvBody" AutoGenerateColumns="False" 
                        onrowdatabound="gvBody_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="sysid" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:CheckBox ID="cbSystem" runat="server" Checked='<%#Eval("checked").ToString()=="1" ? true:false %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="groupname" HeaderText="System" />
                            <asp:BoundField DataField="sysname" HeaderText="Sub-System" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr><td><br />
                <asp:Button runat="server" ID="btnSave" Text="SAVE" CssClass="button" 
                    UseSubmitBehavior="False" onclick="btnSave_Click" /></td></tr>
        </table>
    </div>
    </form>
</body>
</html>
