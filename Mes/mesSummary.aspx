<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mesSummary.aspx.cs" Inherits="Mes_mesSummary" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Plan Cut Summary Report</title>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script language="javascript">
        function init() {

            if (document.getElementById("txtMoNo").value == "") {

                document.all.ExcTable.style.display = 'none';
            }
        }
        function searchMarkerNo() {
            var urlName = "mesSearchMarkerNo.aspx";
            var markerNo = window.showModalDialog(urlName, "Marker No.", "dialogWidth=450pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (markerNo == null) return;
            document.all.txtMoNo.value = markerNo;
        }
        function cutPlanToExcel() {

            var myExcel, myBook, softType;
            try {
                myExcel = new ActiveXObject("Excel.Application");
                softType = "MSO";
            } catch (Exception) {
                try {
                    myExcel = new ActiveXObject("ET.Application");
                    softType = "WPS";
                } catch (Exception) {

                }
            }

            if (myExcel != null) {
                if (softType == "MSO") {
                    window.clipboardData.setData("Text", document.all.ExcTable.outerHTML);
                    myExcel.Visible = true;
                    myBook = myExcel.Workbooks.Add();
                    myBook.sheets(1).paste();
                }
                else if (softType == "WPS") {
                    document.all("excel").disabled = true;
                    document.execCommand("SelectAll");
                    document.execCommand("Copy");
                    document.execCommand('Unselect');
                    document.all("excel").disabled = false;
                    myExcel.Visible = true;
                    myBook = myExcel.Workbooks.Add();
                    myBook.sheets(1).paste();
                    myBook.sheets(1).Rows(1).Delete();
                    myBook.sheets(1).Rows(1).Delete();

                }

            }
            else {
                alert("Your Machine isn't install Excel,can't open this file.");
            }
        }






        function cutPlanToWPS() {

            var myExcel, myBook, softType;
            try {
                myExcel = new ActiveXObject("ET.Application");
                softType = "WPS";
            } catch (Exception) {

            }

            if (myExcel != null) {
                window.clipboardData.setData("Text", document.all.ExcTable.outerHTML);
                myExcel.Visible = true;
                myBook = myExcel.Workbooks.Add();
                myBook.sheets(1).paste();
            }
            else {
                alert("Your Machine doesn't install WPS,can't open this file.");
            }
        }
    </script>
</head>
<body onload="init()">
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search </legend>
            <table width="100%" id="queryTab">
                <tr>
                    <td class="tdbackcolor" width="100">
                        Marker Order No:
                    </td>
                    <td>
                        <asp:TextBox ID="txtMoNo" runat="server"></asp:TextBox>
                        <input type="button" value="..." onclick="searchMarkerNo()" class="button_top">
                    </td>
                    <td align="RIGHT">
                        <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade size="1">
    </div>
    <div id="EtTable">
        <table id="ExcTable" width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="center">
                    <font face="Arial" size="4">Plan Cut Summary Report</font>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="mmPrint" align="right">
                        <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv .style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv .style.display='block';document.all.mmPrint.style.visibility='visible'"
                            style="height: 26" class="button_top">
                        <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,document.title+'.htm')"
                            style="height: 26" class="button_top">
                        <input type="button" name="excel" value="To Excel" onclick="cutPlanToExcel()" style="height: 26"
                            class="button_top">
                        <input type="button" name="WPS" value="To WPS" onclick="cutPlanToWPS()" style="height: 26"
                            class="button_top">
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <table border="1" cellspacing="0" cellpadding="0" style="font-size: 12px; border-collapse: collapse">
                        <tr>
                            <td class="tr2style" width="60">
                                GO NO:
                            </td>
                            <td class="tr2style" width="60">
                                Part Type:
                            </td>
                            <td class="tr2style" width="100">
                                GO Qty:
                            </td>
                            <td class="tr2style" width="100">
                                GO Plan Cut Qty:
                            </td>
                            <td class="tr2style" width="200">
                                GO Over/Short Plan Cut Qty:
                            </td>
                            <td class="tr2style" width="100">
                                Percentage
                            </td>
                        </tr>
                        <div id="divHeader" runat="server">
                        </div>
                    </table>
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
                            <td class="tr2style" width="130">
                                Color
                            </td>
                            <td class="tr2style">
                                Fab Desc
                            </td>
                            <td class="tr2style">
                                Fab Rec(YDS)
                            </td>
                            <td class="tr2style">
                                Spare Fab(YDS)
                            </td>
                            <td class="tr2style">
                                Binding Fab(YDS)
                            </td>
                            <td class="tr2style">
                                Net Bulk Fab Req(YDS)
                            </td>
                            <td class="tr2style">
                                Fab Balance(YDS)
                            </td>
                        </tr>
                        <div id="divColor" runat="server">
                        </div>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <div id="divDetail" runat="server">
            </div>
        </table>
    </div>
    </form>
</body>
</html>
