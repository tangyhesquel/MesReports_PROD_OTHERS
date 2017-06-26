<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PreCutSummary.aspx.cs" Inherits="Reports_PreCutSummary" %>

<%@ Register Src="~/UserControls/LanguageSelectUserControl.ascx" TagPrefix="mes"
    TagName="lang" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Pre-Cut Summary Report </title>
    <style type="text/css">
        .style2
        {
            background-color: #efefe0;
            color: #000000;
            font-weight: bolder;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        .style3
        {
            font-size: 12px;
            color: #000000;
            line-height: 20px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            background-color: #efefd0;
            font-weight: bolder;
        }
        .style4
        {
            font-size: 12px;
            color: #000000;
            line-height: 20px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            background-color: #efefe0;
            font-weight: bolder;
        }
    </style>
    <script language="javascript" type="text/javascript">
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
                window.clipboardData.setData("Text", document.all.printArea.outerHTML);
                myExcel.Visible = true;
                myBook = myExcel.Workbooks.Add();
                myBook.sheets(1).paste;
                //					myBook.sheets(1).Range("A1:H1").Merge
                //                    myBook.sheets(1).Range("A2:H2").Merge
                myBook.sheets(1).Range(myBook.sheets(1).cells(1, 1), myBook.sheets(1).cells(1, 8)).Select(); //选择该列
                myExcel.Selection.HorizontalAlignment = 3;                          //居中
                myExcel.Selection.MergeCells = true;
                myBook.sheets(1).Range(myBook.sheets(1).cells(2, 1), myBook.sheets(1).cells(2, 8)).Select(); //选择该列
                myExcel.Selection.HorizontalAlignment = 3;                          //居中
                myExcel.Selection.MergeCells = true;
                myBook.sheets(1).Rows.AutoFit();
                myBook.sheets(1).Columns.AutoFit();
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <table id="mmPrint" width="100%" style="border-collapse: collapse; font-size: 12px;"
        border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td align="right">
                <input type="button" name="print" value="Print" onclick="javacript:document.all.mmPrint.style.visibility='hidden';window.print();document.all.printArea.style.display='block';document.all.mmPrint.style.visibility='visible'"
                    style="font-size: 16; width: 80; height: 26" />
                <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,'Cut & Shipment.htm')"
                    style="font-size: 16; width: 80; height: 26" />
                <input type="button" name="ToExcel" value="ToExcel" onclick="toPaseExcel()" style="font-size: 16;
                    width: 80; height: 26" />
            </td>
            <td width="10%">
            </td>
        </tr>
    </table>
    <br />
    <div id="printArea">
        <table width="100%" style="border-collapse: collapse; font-size: 12px;" border="0"
            cellspacing="0" cellpadding="0">
            <tr>
                <td align="center" style="width: 100%">
                    <font face="Arial" size="5">Pre-Cut Summary Report</font>
                </td>
            </tr>
            <tr>
                <td align="center" style="width: 100%; font-family: Times New Roman">
                    <%=factoryName%>
                </td>
            </tr>
        </table>
        <br />
        <table id="ExcTable" style="border-style: none; width: 100%">
            <tr>
                <td align="center">
                    <div id="div_Header_Summary" runat="server" style="width: 100%">
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <div id="div_Header_info" runat="server">
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <div id="div_detail" runat="server">
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
