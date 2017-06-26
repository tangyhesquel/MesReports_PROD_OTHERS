<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TransactionDocEntryDetail.aspx.cs"
    Inherits="Transaction_Doc_Entry_Detail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Transaction Document Entry Detail</title>
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script language="javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <script language="javascript" type="text/javascript">
        function init() {
            if (document.all.txtStartDate.value == '') {
                document.all.ExcTable.style.display = 'none';
            }
        }
    </script>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
</head>
<body onload="init()">
    <form id="form1" runat="server">
    <div id="queryDiv">
        </br>
        <table width="100%" id="queryTab">
            <tr>
                <td width="90">
                    Date From:
                </td>
                <td width="110">
                    <asp:TextBox ID="txtStartDate" onfocus="WdatePicker({skin:'whyGreen'})" runat="server"
                        Width="98px"></asp:TextBox>
                </td>
                <td width="40">
                    To:
                </td>
                <td width="200">
                    <asp:TextBox ID="txtToDate" onfocus="WdatePicker({skin:'whyGreen'})" runat="server"
                        Width="98px"></asp:TextBox>
                </td>
                <td width="90">
                    <asp:Button runat="server" ID="btnquery" Text="QUERY" OnClick="btnquery_Click" />
                </td>
                <td>
                    <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" />
                </td>
            </tr>
        </table>
        <hr noshade size="1">
        <table width="100%" id="ExcTable">
            <tr>
                <td align="center">
                    <h2>
                        Transaction Doc Entry Detail</h2>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView CssClass="dataTableStyle" ID="GridView1" runat="server" BorderStyle="Solid"
                        BorderWidth="1px" BorderColor="Black" GridLines="Both" Width="100%">
                        <RowStyle BorderStyle="Solid" BorderWidth="1px" BorderColor="Black" />
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblStatus" runat="server"></asp:Label>
                </td>
            </tr>
            <br />
        </table>
    </div>
    </form>
</body>
</html>
