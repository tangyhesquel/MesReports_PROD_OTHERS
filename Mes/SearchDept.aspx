<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SearchDept.aspx.cs" Inherits="Mes_SearchDept" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <base target="_self" />
</head>
<body bgcolor="#FFFFFF" text="#000000">
    <form id="form1" runat="server">
    <div>
        <br />
        <table width="100%">
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
                            <asp:BoundField DataField="PRC_CD" />
                            <asp:BoundField DataField="NM" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Button ID="btnSelect" runat="server" Text="Select" CssClass="BUTTON" Height="19px"
                        Width="108px" OnClick="btnSelect_Click" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
