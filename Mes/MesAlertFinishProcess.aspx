<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MesAlertFinishProcess.aspx.cs"
    Inherits="Mes_MesAlertFinishProcess" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script language="javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <script language="javascript" type="text/javascript"></script>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="100%">
            <tr>
                <td width="50">
                    Date:
                </td>
                <td width="150">
                    <asp:TextBox ID="txtDate" onfocus="WdatePicker({skin:'whyGreen'})" runat="server"
                        Width="100"></asp:TextBox>
                </td>
                <td width="50">
                    Times:
                </td>
                <td width="150">
                    <asp:DropDownList ID="ddlTimes" runat="server" OnLoad="ddlTimes_onload" Width="100">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Button ID="btnQuery" runat="server" Text="Query" Height="22px" OnClick="btnQuery_Click" />
                </td>
            </tr>
        </table>
    </div>
    <h2 align="center">
        MES Alert Finish Process </h1>
        <div>
            <asp:GridView ID="GridView1" runat="server" Width="100%">
            </asp:GridView>
        </div>
    </form>
</body>
</html>
