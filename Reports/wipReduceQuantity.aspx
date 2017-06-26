<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wipReduceQuantity.aspx.cs"
    Inherits="Reports_WIPReduceQuantity" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Actual Cutting Report</title>
    <style type="text/css">
        BODY
        {
            font-size: 12px;
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
            font-size: 12px;
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
            font-family: "Arial" , "Helvetica" , "sans-serif";
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
        .tdbackcolor
        {
            background-color: #EFEFE0;
        }
        .thstyle
        {
            background-color: #D2D1B0;
        }
    </style>
    <script type="text/javascript">
        function keyDown(btn) {
            if (event.keyCode = 13) {
                __doPostBack(btn, '');
                return false;
            }
        }

        function init() {
            //            if (document.all.txtJoNo.value == '') {
            //                document.all.ExcTable.style.display = 'none';
            //            }
        }
        function searchJo() {
            //var urlName = "searchJONewCut.aspx?factory=" + document.all.ddlFactory.value + "&userRandom=" + (Math.random() * 100000);
            var urlName = "searchJONewCut.aspx"+window.location.search+"&factory=" + document.all.ddlFactory.value +"&userRandom=" + (Math.random() * 100000);
            //var jo = window.showModalDialog(urlName, "Job Order No.", "dialogWidth=650pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            var jo = window.showModalDialog(urlName, "strJoList.", "dialogWidth=650pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (jo == null) return;
            document.all.txtJoNo.value = jo;
        }
        function searchGo() {
            var urlName = "searchGO.aspx?factory=" + document.all.ddlFactory.value + "&userRandom=" + (Math.random() * 100000);
            var jo = window.showModalDialog(urlName, "Go No.", "dialogWidth=650pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (jo == null) return;
            document.all.txtJoNo.value = jo;
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
                myBook.sheets(1).paste
                myBook.sheets(1).Range("A5", "Z1000").Font.Size = 12;
            }
        }
        function ToExcelOfWPS() {
            var myExcel, myBook;
            try {
                myExcel = new ActiveXObject("ET.Application");
            } catch (Exception) {
                alert("Open WPS Application exception");
                return;
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
                myBook.sheets(1).Range("A5", "Z1000").Font.Size = 12;
                myBook.sheets(1).Range("A3", "H4").Merge(true);
                myBook.sheets(1).Range("A3", "H4").Font.Size = 24;
                //                myBook.sheets(1).Range("A3", "H4").Font.weight= bold
            }

        }
       

    </script>
</head>
<body onload="init()">
    <form id="form1" runat="server" defaultbutton="btnQuery">
    <div id="queryDiv" runat="server">
        <fieldset>
            <legend>Search</legend>
            <table width="100%" id="queryTab" >
                <tr>
                    <td height="19" class="tdbackcolor">
                        Factory:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFactory" runat="server" Enabled="false">
                        </asp:DropDownList>
                    </td>
                    <td height="19" class="tdbackcolor">
                        Go No:
                    </td>
                    <td style="">
                        <asp:TextBox ID="txtGO" runat="server"></asp:TextBox>&nbsp;
                        <input type="button" value="..." onclick="searchGo()" class="button_top" style="visibility: hidden" />
                    </td>
                    <td height="19" class="tdbackcolor">
                        Job Order No.:
                    </td>
                    <td>
                        <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>&nbsp;
                        <input type="button" runat="server" value="..." onclick="searchJo()" class="button_top" />
                    </td>
                    <td height="19" class="tdbackcolor">
                        Summary By
                    </td>
                    <td>
                        <asp:DropDownList ID="DDSummaryBy" runat="server">
                            <asp:ListItem Value="JO" Text="JO NO"></asp:ListItem>
                            <asp:ListItem Value="GO" Text="GO NO"></asp:ListItem>
                            <%--Added by MF on 20150812, JO Combination--%>
                            <asp:ListItem Value="LOT" Text="JO By LOT"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td width="20%">
                        <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click1" />
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    <div id="divMeg" runat="server">
    </div>
    <table id="ButtonTbale" width="100%" border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td align="center">
                <font face="Arial" size="4">Actual Cutting Report</font>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <div id="mmPrint" align="right">
                    <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv.style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv .style.display='block';document.all.mmPrint.style.visibility='visible'"
                        style="font-size: 16; width: 80; height: 26" />
                    <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,'ActualCuttingReport.htm')"
                        style="font-size: 16; width: 80; height: 26" />
                    <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="font-size: 16;
                        width: 80; height: 26" />
                    <input type="button" name="excel" value="To WPS" onclick="ToExcelOfWPS()" style="font-size: 12;
                        width: 80; height: 26" />
                </div>
            </td>
        </tr>
    </table>
    <table id="ExcTable" width="100%" border="0" cellpadding="0" cellspacing="0">
        <div id="gvDetail" runat="server">
        </div>
    </table>
    </form>
</body>
</html>
