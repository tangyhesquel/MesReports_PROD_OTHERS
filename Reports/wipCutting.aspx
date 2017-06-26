<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wipCutting.aspx.cs" Inherits="Reports_Cutting" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Cutting Output Report</title>
    <style type="text/css">
        BODY {
            font-size: 11px;
            padding: 5px;
            font-family: Arial,Times New Roman, 黑体,Verdana,Helvetica, sans-serif;
        }

        A:link {
            color: #000000;
            text-decoration: none;
        }

        A:active {
            color: #000000;
            text-decoration: none;
        }

        A:visited {
            color: #000000;
            text-decoration: none;
        }

        A:hover {
            color: #000000;
            text-decoration: none;
        }

        A:hover {
            left: 1px;
            border-bottom: 1px dotted;
            position: relative;
            top: 1px;
        }

        .BUTTON_Gary {
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            cursor: hand;
            background-color: #efefef;
            border: 1px solid #333333;
        }

        .BUTTON {
            font-weight: bolder;
            font-size: 11px;
            padding: 1px;
            padding-bottom: 2px;
            text-transform: uppercase;
            cursor: hand;
            height: 20px;
            color: #646400;
            background-color: #CCCC99;
            border: 1px solid #333333;
        }

        .BUTTON_down {
            font-weight: bolder;
            font-size: 11px;
            padding: 1px;
            padding-bottom: 2px;
            text-transform: uppercase;
            cursor: hand;
            height: 20px;
            color: #646400;
            background-color: #efefe7;
            border: 1px solid #646400;
        }

        .input_gary {
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            background-color: #efefef;
            border: 1px solid #333333;
        }

        .input_color {
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            background-color: #EEF7FF;
            border: 1px solid #333333;
        }

        .input_white {
            padding-left: 2px;
            font-size: 11px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            height: 18px;
            border: 1px solid #333333;
        }

        INPUT {
            padding-left: 2px;
            font-size: 11px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            height: 18px;
        }

        TABLE {
            padding-right: 0px;
            padding-left: 0px;
            font-size: 11px;
            padding-bottom: 0px;
            padding-top: 0px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }

        .tr1style {
            font-size: 11px;
            background-color: #ffffff;
            padding-left: 5px;
            padding-right: 5px;
            padding-top: 2px;
        }

        .tr2style {
            font-weight: bolder;
            font-size: 11px;
            background-color: #efefe7;
            padding-left: 5px;
            padding-right: 5px;
            padding-top: 2px;
        }

        .tr3style {
            font-weight: bolder;
            font-size: 11px;
            background-color: #ffffff;
            padding-left: 5px;
            padding-top: 2px;
        }

        .bigfont {
            font-weight: bolder;
            font-size: 18px;
            color: #736d00;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }

        .bigfont1 {
            font-weight: bolder;
            font-size: 14px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }

        .red {
            font-weight: 600;
            color: #800000;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }

        .footer {
            font-size: 11px;
            color: #000000;
            line-height: 20px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            background-color: #efefef;
        }

        .button_top {
            font-family: "Arial", "Helvetica", "sans-serif";
            font-size: 12px;
            background-color: #CCCC99;
            border: ridge;
            font-weight: bold;
            border-width: 1px 1px 2px;
            border-color: #000000 #f2f2f2 #f2f2f2 #000000;
            color: #333366;
            text-transform: uppercase;
            cursor: hand;
        }

        .tdbackcolor {
            background-color: #EFEFE0;
        }

        .thstyle {
            background-color: #D2D1B0;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function init() {
            if (document.all.txtJoNo.value == '')
                document.all.ExcTable.style.display = 'none';
        }
        function searchJo() {
            var urlName = "searchJO.aspx?factory=" + document.all.ddlFactory.value + "&userRandom=" + (Math.random() * 100000);
            var jo = window.showModalDialog(urlName, "Job Order No.", "dialogWidth=650pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (jo == null) return;
            document.all.txtJoNo.value = jo;
        }
    </script>
    <script type="text/javascript" src="../Script/Common.js"></script>
</head>
<body onload="init()">
    <form id="form1" runat="server">
        <div id="queryDiv">
            <fieldset>
                <legend>Search</legend>
                <table width="100%" id="queryTab">
                    <tr>
                        <td height="19" class="tdbackcolor">Factory:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlFactory" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td height="19" class="tdbackcolor">Job Order No.:
                        </td>
                        <td width="30%">
                            <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                            <input type="button" value="..." onclick="searchJo()" class="button_top" />
                        </td>
                        <td width="10%">&nbsp;
                        </td>
                        <td width="20%">
                            <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                        </td>
                    </tr>
                </table>
            </fieldset>
            <hr noshade size="1" />
            <br />
            <asp:Label ID="lblNoData" runat="server" Text="NO DATA" Width="100%" Visible="False" ForeColor="Red" Style="font-size: large; font-weight: 700"></asp:Label>
            <div id="divDetail" runat="server">
                <table id="ExcTable" width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td align="center">
                            <font face="Arial" size="4">Cutting Output Report</font>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div id="mmPrint" align="right" style="width: 100%">
                                <input type="button" name="print" value="Print" onclick="javacript: document.all.queryDiv.style.display = 'none'; document.all.mmPrint.style.visibility = 'hidden'; window.print(); document.all.queryDiv.style.display = 'block'; document.all.mmPrint.style.visibility = 'visible'"
                                    style="font-size: 16; width: 80; height: 26" />
                                <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs', false, 'ActualCuttingReport.htm')"
                                    style="font-size: 16; width: 80; height: 26" />
                                <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="font-size: 16; width: 80; height: 26" />
                                <input type="button" name="ToWps" value="ToWps" onclick="ToExcelOfWPS()" style="font-size: 16; width: 80; height: 26" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <strong><font size="3">JO Header</font></strong>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px; border-collapse: collapse">
                                <tr>
                                    <td class="tr2style" width="20%">GO No.
                                    </td>
                                    <td width="30%">
                                        <%=scNo%>
                                    </td>
                                    <td class="tr2style" width="20%">JO No
                                    </td>
                                    <td width="30%">
                                        <%=joNo%>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tr2style">Style No.
                                    </td>
                                    <td>
                                        <%=styleNo%>
                                    </td>
                                    <td class="tr2style">GMT DATE
                                    </td>
                                    <td>
                                        <%=gmtDelDate %>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tr2style">Pring Date
                                    </td>
                                    <td>
                                        <%=printDate %>
                                    </td>
                                    <td class="tr2style">Season
                                    </td>
                                    <td>
                                        <%=season %>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <strong><font size="3">Detail</font></strong>
                            <div id="divBody">
                                <asp:GridView ID="gvCutDeail" runat="server" HeaderStyle-BackColor="#efefe7" Width="100%">
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
