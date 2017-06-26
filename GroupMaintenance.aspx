<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GroupMaintenance.aspx.cs"
    Inherits="Reports_GroupMaintenance" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Group Maintenance</title>
    <link href="StyleSheet.css" rel="Stylesheet" />
    <base target="_self" />
    <script language="javascript">
    function window.onunload()
    {
         window.returnValue='Y';
    }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width: 100%; text-align:center">
    <br />
    <div style="text-align:center"><h3>Group  Maintenace</h3></div>
        <table style="width: 96%">
            <tr>
                <td style="width: 48%; border-style: inset; height:300px; overflow:auto" align="center" valign="top" >
                    <asp:GridView runat="server" ID="gvHandle" AutoGenerateColumns="False" Width="100%"
                        OnRowDeleting="gvHandle_RowDeleting" 
                        
                        onrowdatabound="gvHandle_RowDataBound">
                        <Columns>
                            <asp:BoundField HeaderText="Group Name" DataField="group_name" />
                            <asp:CommandField HeaderText="Edit" SelectText="Edit" ShowCancelButton="False" ShowSelectButton="True"/>
                            <asp:CommandField HeaderText="Delete"  DeleteText="Delete"    ShowDeleteButton="True"/>
                        </Columns>
                    </asp:GridView><br />
                    <asp:Button runat="server" ID="btnHandler" Text="Add By Handler" 
                        CssClass="button" Width="100px" OnClientClick="window.showModalDialog('AddGroupByHandler.aspx?type=A','','dialogWidth=400px;');"  />
                </td><td style="width:4%"></td>
                <td style="width: 48%; border-style: inset;overflow:auto;height:300px;" align="center" valign="top">
                    <asp:GridView runat="server" ID="gvSystem" AutoGenerateColumns="False" Width="100%"
                        OnRowDeleting="gvSystem_RowDeleting" 
                        onrowdatabound="gvSystem_RowDataBound">
                        <Columns>
                            <asp:BoundField HeaderText="Group Name" DataField="group_name"></asp:BoundField>
                            <asp:CommandField HeaderText="Edit" SelectText="Edit" ShowSelectButton="True" />
                            <asp:CommandField HeaderText="Delete" DeleteText="Delete" ShowDeleteButton="True" />
                        </Columns>
                    </asp:GridView><br />
                    <asp:Button runat="server" ID="btnSystem" Text="Add By System" 
                        CssClass="button"  Width="100px" OnClientClick="window.showModalDialog('AddGroupBySystem.aspx?type=A','','dialogWidth=400px;'); " />
                </td>
            </tr>
            <tr><td colspan="3"><br /></td></tr>
            <tr><td colspan="3" align="center"><asp:Button runat="server" Text="Close Window" 
                    onclick="Unnamed1_Click"/></td></tr>
        </table>
    </div>
    </form>
</body>
</html>
