<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pcmDailyBundlingQty.aspx.cs"
    Inherits="Reports_pcmDailyBundlingQty" %>

<!DOCTYPE html PUBLIC "-//W3C//Dtd XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/Dtd/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Daily Bundling Qty Report</title>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script language="javascript" type="text/jscript">
        function init() {
            if (document.all.txtJoNo.value == '') {
                document.all.ExcTable.style.display = 'none';
            }
        }

    </script>
    <style>
        BODY
        {
            font-size: 11px;
            padding: 5px;
            font-family: Arial,Times New Roman, 黑体,Verdana,Helvetica, sans-serif;
        }
        A:link
        {
            color: #000000;
            text-decoration: none;
        }
        A:active
        {
            color: #000000;
            text-decoration: none;
        }
        A:visited
        {
            color: #000000;
            text-decoration: none;
        }
        A:hover
        {
            color: #000000;
            text-decoration: none;
        }
        A:hover
        {
            left: 1px;
            border-bottom: 1px dotted;
            position: relative;
            top: 1px;
        }
        .BUTTON
        {
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
        INPUT
        {
            padding-left: 2px;
            font-size: 11px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            height: 18px;
        }
        TABLE
        {
            padding-right: 0px;
            padding-left: 0px;
            font-size: 11px;
            padding-bottom: 0px;
            padding-top: 0px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        .tr2style
        {
            font-weight: bolder;
            font-size: 11px;
            background-color: #efefe7;
            padding-left: 5px;
            padding-right: 5px;
            padding-top: 2px;
        }
        .tr3style
        {
            font-weight: bolder;
            font-size: 11px;
            background-color: #ffffff;
            padding-left: 5px;
            padding-top: 2px;
        }
        .tdbackcolor
        {
            background-color: #EFEFE0;
        }
        .button_top
        {
            font-family: "Arial" , "Helvetica" , "sans-serif";
            font-size: 14px;
            background-color: #CCCC99;
            border: ridge;
            font-weight: bold;
            border-width: 1px 1px 2px;
            border-color: #000000 #f2f2f2 #f2f2f2 #000000;
            color: #333366;
            text-transform: uppercase;
            cursor: hand;
        }
    </style>
</head>
<body onload="init()">
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search</legend>
            <table width="100%" id="queryTab">
                <tr>
                    <td width="12%" height="19" class="tdbackcolor">
                        Factory Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFtyCd" runat="server" />
                    </td>
                    <td height="19" class="tdbackcolor">
                        Job Order No.:
                    </td>
                    <td>
                        <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                    </td>
                    <td height="19" class="tdbackcolor">
                        Cut Line:
                    </td>
                    <td>
                        <asp:TextBox ID="txtCutLine" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td height="19" class="tdbackcolor">
                        Trx Date From:
                    </td>
                    <td>
                        <asp:TextBox ID="txtStartDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td height="19" class="tdbackcolor">
                        Trx Date To:
                    </td>
                    <td>
                        <asp:TextBox ID="txtEndDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td align="right">
                        <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade size="1">
        <table id="ExcTable" width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="center">
                    <font face="Arial" size="4">Daily Bundling Qty Report</font>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="mmPrint" align="right" style="width: 100%">
                        <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv .style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv .style.display='block';document.all.mmPrint.style.visibility='visible'"
                            style="font-size: 16; width: 80; height: 26" />
                        <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,document.title+'.htm')"
                            style="font-size: 16; width: 80; height: 26" />
                        <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="font-size: 16;
                            width: 80; height: 26" />
                        <input type="button" name="excel" value="To Wps" onclick="ToExcelOfWPS()" style="font-size: 16;
                            width: 80; height: 26" />
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                        border-collapse: collapse">
                        <tr>
                            <td class="tr2style" width="100">
                                Factory Code:
                            </td>
                            <td>
                                <%=ddlFtyCd.SelectedItem.Text %>
                            </td>
                            <td class="tr2style" width="100">
                                Job Order No.:
                            </td>
                            <td>
                                <%=txtJoNo.Text.Trim() %>
                            </td>
                            <td class="tr2style" width="100" colspan="">
                                Cut Line:
                            </td>
                            <td>
                                <%=txtCutLine.Text.Trim() %>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="tr2style" width="100">
                                Trx Date From:
                            </td>
                            <td>
                                <%=txtStartDate.Text.Trim() %>
                            </td>
                            <td class="tr2style" width="100">
                                Trx Date To:
                            </td>
                            <td>
                                <%=txtEndDate.Text.Trim() %>
                            </td>
                            <td colspan="3">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="7">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="7">
                                <div id="divBody" runat="server">
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
