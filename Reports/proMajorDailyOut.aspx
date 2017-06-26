<%@ Page Language="C#" AutoEventWireup="true" CodeFile="proMajorDailyOut.aspx.cs"
    Inherits="Reports_proMajorDailyOut" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Production Major Process Output Report</title>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" language="javascript">
        function init() {
            if (document.getElementById("ddlFactoryCd").value == "" && document.getElementById("txtJoNo").value == "") {
                document.all.reportDiv.style.display = 'none';
            }
        }
    </script>
    <script type="text/javascript" src="../Script/Common.js"></script>
</head>
<body onload="init()">
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search</legend>
            <table width="100%" id="queryTab" align="center">
                <tr>
                    <td width="12%" height="19" class="tdbackcolor">
                        Factory Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFactoryCd" runat="server" DataTextField="DEPARTMENT_ID"
                            DataValueField="DEPARTMENT_ID" Enabled="false">
                        </asp:DropDownList>
                    </td>
                    <td width="15%" height="19" class="tdbackcolor">
                        Job Order No:
                    </td>
                    <td>
                        <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                    </td>
                    <td width="15%" height="19" class="tdbackcolor">
                        <asp:CheckBox ID="SplitSEW" runat="server" Text="Split SEW QTY" />
                    </td>
                    <td align="right">
                        <asp:Button ID="btnQuery" Text="Query" runat="server" CssClass="button_top" OnClick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade size="1">
    </div>
    <div id="ExcTable">
        <table width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td width="20%">
                    &nbsp;
                </td>
                <td width="60%">
                    &nbsp;
                </td>
                <td width="20%" align="center">
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td align="center">
                    <font face="Arial" size="4">Production Major Process Tranasaction Out Report</font>
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td width="20%">
                    &nbsp;
                </td>
                <td width="60%" align="center">
                    &nbsp;
                </td>
                <td width="20%" align="center">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="3" align="left">
                </td>
            </tr>
        </table>
        <div id="mmPrint" align="right">
            <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv.style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv.style.display='block';document.all.mmPrint.style.visibility='visible'"
                style="font-size: 16; width: 80; height: 26"/>
            <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,'Production Major Process Output Report.htm')"
                style="font-size: 16; width: 80; height: 26"/>
            <input type="button" name="ToExcel" value="ToExcel" onclick="toPcmExcel()" style="font-size: 16;
                width: 80; height: 26"/>
            <input type="button" name="ToWps" value="ToWps" onclick="ToExcelOfWPS()" style="font-size: 16;
                width: 80; height: 26" />
        </div>
        <br>
        <div id="divBody" runat="server">
        </div>
        <asp:Label ID="lblNoData" runat="server" Text="NO DATA" Font-Bold="True" Font-Size="X-Large" ForeColor="Red" Visible="False"></asp:Label>
        <asp:GridView ID="gvData" runat="server" Width="100%" OnRowDataBound="gvData_RowDataBound">
        </asp:GridView>
    </div>
    </form>
</body>
</html>
