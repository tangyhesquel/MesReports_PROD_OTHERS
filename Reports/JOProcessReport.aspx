<%@ Page Language="C#" AutoEventWireup="true" CodeFile="JOProcessReport.aspx.cs"
    Inherits="Reports_JOProcessReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>JO Process List</title>
    <object id="WebBrowser1" width="0" height="0" classid="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2">
    </object>
    <script type="text/javascript" src="../Script/Common.js"></script>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script src="../My97DatePicker/WdatePicker.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table id="toPrint" width="100%">
            <tr>
                <td align="right" colspan="8">
                    <input name="btnPrint" type="button" value="   Print   " onclick="toPrint.style.display='none';WebBrowser1.ExecWB(6,1);toPrint.style.display=''" />
                    <input name="btnPreview" type="button" value="Preview" onclick="toPrint.style.display='none';WebBrowser1.ExecWB(7,1);toPrint.style.display=''" />
                    <input name="btnPageSetup" type="button" value="Page Setup" onclick="toPrint.style.display='none';WebBrowser1.ExecWB(8,1);toPrint.style.display=''" />
                    <input name="btnToExcel" type="button" value="ToExcel" onclick="toPaseExcel()" />
                    <input type="button" name="excel" value="To Wps" onclick="ToExcelOfWPS()" />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td width="75">
                    Factory Code.:
                </td>
                <td width="75">
                    <asp:DropDownList ID="ddlFactoryCd" runat="server" DataTextField="FACTORY_ID" DataValueField="FACTORY_ID">
                    </asp:DropDownList>
                </td>
                <td width="75">
                    Contract No.
                </td>
                <td width="75">
                    <asp:TextBox ID="txtContractNo" runat="server"></asp:TextBox>
                </td>
                <td width="90">
                    From Issue Date:
                </td>
                <td nowrap width="130">
                    <asp:TextBox ID="txtstartDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                </td>
                <td width="90">
                    To Issue Date:
                </td>
                <td nowrap width="130">
                    <asp:TextBox ID="txtendDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                </td>
                <td align="left">
                    <asp:Button runat="server" ID="btnQuery" Text="Query" OnClick="btnQuery_Click" />
                </td>
            </tr>
        </table>
        <table id="ExcTable" style="border-collapse: collapse" bordercolor="#999999" cellspacing="0"
            cellpadding="0" width="100%" border="1">
            <tr class="tr2style">
                <td>
                    JO No.
                </td>
                <td>
                    Contract No.
                </td>
                <td>
                    Sub Contract Price
                </td>
                <td>
                    Emb Price
                </td>
                <td>
                    Print Price
                </td>
                <td>
                    Wash Price
                </td>
                <td>
                    Out Source Process
                </td>
                <td>
                    Subcontractor
                </td>
                <td>
                    Process Remark
                </td>
                <td>
                    Issuer Date:
                </td>
            </tr>
            <div id="divBody" runat="server">
            </div>
        </table>
    </div>
    </form>
</body>
</html>
