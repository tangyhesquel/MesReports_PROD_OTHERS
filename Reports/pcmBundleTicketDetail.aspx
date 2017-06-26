<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pcmBundleTicketDetail.aspx.cs"
    Inherits="Reports_PcmBundleTicketDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Bundle Ticket Detail Report</title>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
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
        .BUTTON_Gary
        {
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            cursor: hand;
            background-color: #efefef;
            border: 1px solid #333333;
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
        .BUTTON_down
        {
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
        .input_gary
        {
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            background-color: #efefef;
            border: 1px solid #333333;
        }
        .input_color
        {
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            background-color: #EEF7FF;
            border: 1px solid #333333;
        }
        .input_white
        {
            padding-left: 2px;
            font-size: 11px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            height: 18px;
            border: 1px solid #333333;
        }
        INPUT
        {
            padding-left: 2px;
            font-size: 11px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
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
        .tr1style
        {
            font-size: 11px;
            background-color: #ffffff;
            padding-left: 5px;
            padding-right: 5px;
            padding-top: 2px;
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
        .bigfont
        {
            font-weight: bolder;
            font-size: 18px;
            color: #736d00;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        .bigfont1
        {
            font-weight: bolder;
            font-size: 14px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        .red
        {
            font-weight: 600;
            color: #800000;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        .footer
        {
            font-size: 11px;
            color: #000000;
            line-height: 20px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            background-color: #efefef;
        }
        .button_top
        {
            border-left: 1px ridge #000000;
            border-right: 1px ridge #f2f2f2;
            border-top: 1px ridge #000000;
            border-bottom: 2px ridge #f2f2f2;
            font-family: "Arial" , "Helvetica" , "sans-serif";
                font-size: 12px;
                background-color: #CCCC99;
                font-weight: bold;
                color: #333366;
                text-transform: uppercase;
                cursor: hand;
            height: 24px;
        }
        .tdbackcolor
        {
            background-color: #EFEFE0;
        }
        .thstyle
        {
            background-color: #D2D1B0;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function init() {
            if (document.all.txtJobOrderNo.value == '') {
                //document.all.ExcTable.style.display = 'none';
            }
        }
        function toPaseExcel() {

            var myExcel, myBook;
            try {
                myExcel = new ActiveXObject("Excel.Application");
            } catch (Exception) {
                alert("Open Excel Application exception");
            }

            if (myExcel != null) {
                window.clipboardData.setData("Text", document.all.ExcTable.outerHTML);
                myExcel.Visible = true;
                myBook = myExcel.Workbooks.Add();
                myBook.sheets(1).paste();

            }
        }

        function toPaseWPS() {

            var myExcel, myBook;
            try {
                myExcel = new ActiveXObject("ET.Application");
            } catch (Exception) {
                alert("Open WPS Application exception");
            }

            if (myExcel != null) {
                var sel = document.body.createTextRange();
                sel.moveToElementText(ExcTable);
                sel.select();
                sel.execCommand("Copy");
                sel.execCommand("Unselect");
                myExcel.Visible = true;
                myBook = myExcel.Workbooks.Add();
                myBook.sheets("Sheet1").Paste();
                myBook.sheets("Sheet1").Columns.Font.Size = 9;
                myBook.sheets("Sheet1").Columns.AutoFit();
            }
        }
    </script>
</head>
<body onload="init()">
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search</legend>
            <table width="100%" id="queryTab">
                <tr>
                    <td height="19" class="tdbackcolor">
                        Factory Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFactoryCode" runat="server" Enabled="false">
                        </asp:DropDownList>
                    </td>
                    <td height="19" class="tdbackcolor">
                        Lay No From:
                    </td>
                    <td>
                        <asp:TextBox ID="txtLayNoFrom" runat="server"></asp:TextBox>
                    </td>
                    <td height="19" class="tdbackcolor">
                        Lay No To:
                    </td>
                    <td>
                        <asp:TextBox ID="txtLayNoTo" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td height="19" class="tdbackcolor">
                        Job Order No.:
                    </td>
                    <td>
                        <asp:TextBox ID="txtJobOrderNo" runat="server"></asp:TextBox>
                    </td>
                    <td height="19" class="tdbackcolor">
                        Print Date From:
                    </td>
                    <td>
                        <asp:TextBox ID="txtBeginDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td height="19" class="tdbackcolor">
                        Print Date To:
                    </td>
                    <td>
                        <asp:TextBox ID="txtEndDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="cbClearBundle" runat="server" Text="Clear Bundle" Checked="true" />
                    </td>
                    <td align="center" colspan="4" style="color: Red; font-size: large">
                        &nbsp;
                    </td>
                    <td align="right">
                        <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                    </td>
                   
                </tr>
                <tr>
                 <td class="red">Note:Please input the JO full prefix</td>
                </tr>
            </table>
        </fieldset>
        <hr noshade size="1">
        <br />
        <div id="divExcTable" runat="server">
            <table id="ExcTable" width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td align="center">
                        <font face="Arial" size="4">Bundle Ticket Detail Report</font>
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
                            <input type="button" name="WPS" value="To WPS" onclick="toPaseWPS()" style="font-size: 16;
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
                                <td class="tr2style" width="8%">
                                    Factory CD:
                                </td>
                                <td width="10%">
                                    <%=ddlFactoryCode.SelectedItem.Value%>
                                </td>
                                <td class="tr2style" width="7%">
                                    JO No:
                                </td>
                                <td width="10%">
                                    <%=txtJobOrderNo.Text.Trim()%>
                                </td>
                                <td class="tr2style" width="8%">
                                    Lay No From:
                                </td>
                                <td width="10%">
                                    <%=txtLayNoFrom.Text.Trim()%>
                                </td>
                                <td class="tr2style" width="8%">
                                    Lay No To:
                                </td>
                                <td width="10%">
                                    <%=txtLayNoTo.Text.Trim()%>
                                </td>
                                <td class="tr2style" width="8%">
                                    Print Date :
                                </td>
                                <td width="21%">
                                    <%=txtBeginDate.Text%>:<%=txtEndDate.Text%>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div id="divBody" runat="server">
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div id="divMeg" runat="server">
        </div>
    </div>
    </form>
</body>
</html>
