<%@ Page Language="C#" AutoEventWireup="true" CodeFile="jobOrderRouteList.aspx.cs"
    Inherits="Reports_jobOrderRouteList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Job Order Route List</title>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        /*
        var curArray = [];
        function toPaseExcel() {
        var myExcel, myBook, c;
        try {
        myExcel = new ActiveXObject("Excel.Application");
        } catch (Exception) {
        alert("Open Excel Application exception");
        }
        if (myExcel != null) {
        var sel = document.body.createTextRange();
        //sel.moveToElementText(ExcTable);
        sel.moveToElementText(DetailTable);                
        sel.select();
        sel.execCommand("Copy");
        myExcel.Visible = true;
        myBook = myExcel.Workbooks.Add();
        //myBook.sheets(1).Paste;
        myBook.sheets(1).Range("A8").PasteSpecial("HTML");
        for (var i = (curArray.length - 1); i >= 0; i--) {
        if (curArray[i] == "") {
        myBook.sheets(1).Columns(i + 1).Delete();
        }
        }
        sel.moveToElementText(headTable);
        sel.select();
        sel.execCommand("Copy");
        //myBook.sheets(1).Range("A2:J7").Clear();
        myBook.sheets(1).Range("A2").PasteSpecial("HTML");
        //myBook.sheets(1).Cells(1, 1).value = "JO Order Route List Report";
        sel.moveToElementText(Titletable);
        sel.select();
        sel.execCommand("Copy");
        myBook.sheets(1).Range("A1").PasteSpecial("HTML");
        myBook.sheets(1).Range("A1:Q1").Merge();
        myBook.sheets(1).Cells(1, 1).value = "JO Order Route List Report";
        myBook.sheets(1).Columns.AutoFit();
        myBook.sheets(1).Rows.AutoFit();
        sel.execCommand("Unselect");
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
        //sel.moveToElementText(ExcTable);
        sel.moveToElementText(DetailTable);
        sel.select();
        sel.execCommand("Copy");                
        myExcel.Visible = true;
        myBook = myExcel.Workbooks.Add();
        myBook.sheets(1).Range("A8").Select();
        myBook.sheets(1).Paste();
        //myBook.sheets("Sheet1").Paste();
        for (var i = (curArray.length - 1); i >= 0; i--) {
        if (curArray[i] == "") {
        myBook.sheets(1).Columns(i + 1).Delete();
        }
        }
        sel.moveToElementText(headTable);
        sel.select();
        sel.execCommand("Copy");
        //myBook.sheets(1).Range("A2:J7").Clear();
        //myBook.sheets(1).Range("A2").Select();
        //myBook.sheets(1).Paste();
        myBook.sheets(1).Range("A2").PasteSpecial("HTML");                
        sel.moveToElementText(Titletable);
        sel.select();
        sel.execCommand("Copy");
        myBook.sheets(1).Range("A1").Select();
        myBook.sheets(1).Paste();
        myBook.sheets(1).Range("A1:Q1").Merge();
        myBook.sheets(1).Cells(1, 1).value = "JO Order Route List Report";
        myBook.sheets(1).Columns.AutoFit();
        myBook.sheets(1).Rows.AutoFit();
        sel.execCommand("Unselect");
        }
        }
        function selectColumn() {
        var str = window.showModalDialog("ColumnSelector.htm", null, "font-size:10px;dialogWidth:30em;dialogHeight:40em;scrollbars=no;status=no");
        SetColumnsDisplay(str);
        }
        function SetColumnsDisplay(str) {
        if (str == null || str == "")
        return;
        //var tbl = document.getElementById("contentTable");
        var tbl = document.getElementById("DetailTable");
        for (i = 1; i < tbl.rows.length - 1; i++) {//Total行不能影响;
        for (var iCol = 0; iCol < tbl.rows[i].cells.length; iCol++)
        tbl.rows[i].cells[iCol].style.display = "none";
        }
        curArray = str.split(",");
        for (var k = 0; k < curArray.length; k++) {
        for (i = 1; i < tbl.rows.length - 1; i++) {//Total行不能影响;
        for (j = 0; j < tbl.rows[i].cells.length; j++) {
        if (curArray[k] == j + 1)
        tbl.rows[i].cells[j].style.display = "block";
        }
        }
        }
        }
        function getCookieVal(offset) {
        var endstr = document.cookie.indexOf(";", offset);
        if (endstr == -1) {
        endstr = document.cookie.length;
        }
        return unescape(document.cookie.substring(offset, endstr));
        }
        function getCookie(name) {
        var arg = name + "=";
        var alen = arg.length;
        var clen = document.cookie.length;
        var i = 0;
        while (i < clen) {
        var j = i + alen;
        if (document.cookie.substring(i, j) == arg) {
        return getCookieVal(j);
        }
        i = document.cookie.indexOf(" ", i) + 1;
        if (i == 0) break;
        }
        return;
        }
        */
        function selectColumn(user_id) {
            var num = Math.random();
            var str = window.showModalDialog("ReportCustomSetting.aspx?site=" + document.all.ddlfactoryCd.value + "&ReportName=JORouteListReport&User_ID=" + user_id + "&randnum=" + num, null, "font-size:10px;dialogWidth:60em;dialogHeight:50em;scrollbars=no;status=no");
            if (document.getElementById('txtJoNo').value != "") {
                document.getElementById('btnQuery').click();
            }
            else if (document.getElementById('txtDate').value != "") {
                document.getElementById('btnQuery').click();
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
            }
        }

        function toPaseWPS() {
            var myExcel, myBook, softType;
            try {
                myExcel = new ActiveXObject("ET.Application");
                softType = "WPS";
            } catch (Exception) {
            }
            if (myExcel != null) {
                //window.clipboardData.setData("Text", document.all.ExcTable.outerHTML);
                var sel = document.body.createTextRange();
                sel.moveToElementText(ExcTable);
                sel.select();
                myExcel.Visible = true;
                myBook = myExcel.Workbooks.Add();
                myBook.sheets(1).Paste();
            }
            else {
                alert("Your Machine doesn't install WPS,can't open this file.");
            }
        }

        function toPaseExcel() {
            var myExcel, myBook, softType;
            try {
                myExcel = new ActiveXObject("Excel.Application");
                softType = "Excel";
            } catch (Exception) {
            }
            if (myExcel != null) {
                window.clipboardData.setData("Text", document.all.ExcTable.outerHTML);
                myExcel.Visible = true;
                myBook = myExcel.Workbooks.Add();
                myBook.sheets(1).Paste();
            }
            else {
                alert("Your Machine doesn't install MS Office,can't open this file.");
            }
        }

        function searchJo() {
            var urlName = "searchJO.aspx?factory=" + document.all.ddlfactoryCd.value + "&site=" + document.all.ddlfactoryCd.value + "&userRandom=" + (Math.random() * 100000);
            var jo = window.showModalDialog(urlName, "Job Order No.", "dialogWidth=650pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (jo == null) return;
            document.all.txtJoNo.value = jo;
        }
        /*
        function LoadSettting() {
        var settting = getCookie("JoRouteListColSelector");
        SetColumnsDisplay(settting);
        }
        */
        function ToPrint1() {
            var RowNo = document.getElementById("RowNo").checked
            if (!RowNo) {
                pagesetup_set("&b&w");
            }
            document.all.queryDiv.style.display = 'none';
            window.print();
            document.all.queryDiv.style.display = 'block';
            pagesetup_set("");
        }

        var hkey_root, hkey_path, hkey_key;
        hkey_root = "HKEY_CURRENT_USER";
        hkey_path = "\\Software\\Microsoft\\Internet Explorer\\PageSetup\\";

        //打印时设置页眉
        function pagesetup_set(StrTitle) {
            try {
                var RegWsh = new ActiveXObject("WScript.Shell")
                hkey_key = "header"
                RegWsh.RegWrite(hkey_root + hkey_path + hkey_key, StrTitle)
                //     hkey_key = "footer"
                //     RegWsh.RegWrite(hkey_root + hkey_path + hkey_key, "")
            } catch (e) {
            }
        }

    </script>
    <style type="text/css">
        .style4
        {
            font-family: "Arial Unicode MS";
            font-size: large;
        }
        .style5
        {
            background-color: #EFEFE0;
        }
        .style7
        {
            background-color: #EFEFE0;
            visibility: hidden;
        }
        .style8
        {
            width: 125px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="queryDiv">
        <table border="0" id="toPrint" width="100%">
            <tr>
                <td class="tdbackcolor">
                    Factory Code:
                </td>
                <td>
                    <asp:DropDownList ID="ddlfactoryCd" runat="server" DataTextField="DEPARTMENT_ID"
                        Enabled="false" DataValueField="DEPARTMENT_ID">
                    </asp:DropDownList>
                </td>
                <td class="tdbackcolor">
                    Job Order No.:
                </td>
                <td class="style8">
                    <asp:TextBox ID="txtJoNo" runat="server" Width="8em" OnTextChanged="txtJoNo_TextChanged"
                        AutoPostBack="True"></asp:TextBox>
                    <input type="button" value="..." onclick="searchJo()" class="button_top" />
                </td>
                <td class="tdbackcolor">
                    Create Date:
                </td>
                <td style="width: 7em">
                    <asp:TextBox ID="txtDate" Width="7em" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                </td>
                <td class="tdbackcolor">
                    GarmentType:
                </td>
                <td>
                    <asp:DropDownList ID="DDGARMENTTYPE" runat="server" AutoPostBack="true"
                        onselectedindexchanged="DDGARMENTTYPE_SelectedIndexChanged">
                        <asp:ListItem>ALL</asp:ListItem>
                        <asp:ListItem>K</asp:ListItem>
                        <asp:ListItem>W</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="tdbackcolor">
                    Process Code
                </td>
                <td>
                    <asp:DropDownList ID="DDPROCESSCD" runat="server" DataTextField="PRC_CD" DataValueField="PRC_CD"
                        AutoPostBack="true">
                    </asp:DropDownList>
                </td>
                <td class="tdbackcolor">
                    PART CODE:
                </td>
                <td>
                    <asp:DropDownList ID="DDPARTCD" runat="server" DataTextField="PART_DESC" DataValueField="PART_CD"
                        AutoPostBack="True">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="style5">
                    Order By:
                </td>
                <td>
                    <asp:DropDownList ID="DDOrderby" runat="server">
                        <asp:ListItem Selected="True" Value="DISPLAY_SEQ">DISPLAY SEQ</asp:ListItem>
                        <asp:ListItem Value="JOB_SEQUENCE_NO">INPUT SEQ</asp:ListItem>
                        <asp:ListItem Value="JOB_CD">JOB CODE</asp:ListItem>
                        <asp:ListItem Value="OPER_CODE">OPER CODE</asp:ListItem>
                        <asp:ListItem Value="PROCESS_CD">PROCESS CODE</asp:ListItem>
                        <asp:ListItem Value="PART_CD">PART CODE</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="style5">
                    RowNo:
                    <asp:CheckBox runat="server" ID="RowNo" Text="" Checked="false" />
                </td>
                <td class="style8">
                    <asp:TextBox ID="TxtRowNo" Text="50" runat="server" Width="3em"></asp:TextBox>
                </td>
                <td class="tdbackcolor">
                    Production Line:
                </td>
                <td style="width: 7em">
                    <asp:DropDownList ID="DDLProdLine" runat="server" DataTextField="Prod_Line_Name"
                        DataValueField="Prod_Line_Cd" AutoPostBack="false">
                    </asp:DropDownList>
                </td>
                <td class="style6">
                    <asp:CheckBox runat="server" ID="ShowBlankLine" Text="Include Blank Line" TextAlign="Right" />
                </td>
                <td class="style7">
                    <asp:CheckBox runat="server" ID="ShowPieceHr" Text="Show Piece/Hr" TextAlign="Right" />
                </td>
                <td colspan="5" align="right">
                    <div id="mmPrint" style="text-align: right;">
                        <!--<input type="button" name="excel" value="Custom Column" onclick="javascript:selectColumn()"
                            style="height: 26" class="button_top" />-->
                        <input type="button" name="excel" value="Custom Column" onclick="javascript:selectColumn('<%=user_id%>')"
                            style="height: 26" class="button_top" />
                        <input type="button" name="print" value="Print" onclick="ToPrint1()" style="height: 26"
                            class="button_top" />
                        <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,document.title+'.htm')"
                            style="height: 26" class="button_top" />
                        <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="height: 26"
                            class="button_top" />
                        <input type="button" onclick="ToExcelOfWPS()" value="To WPS" style="height: 26" class="button_top" />
                        <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click"
                            Width="7em" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <input id="mestxt" type="text" runat="server" readonly="readonly" style="visibility: hidden;
        width: 30%" />
    <table id="ExcTable" width="100%" border="0" cellspacing="0" cellpadding="1" style="font-size: 12px;
        border-collapse: collapse">
        <tr>
            <td colspan="10" align="center" class="style4">
                JO Order Route List Report
            </td>
        </tr>
        <tr>
            <td>
                <table id="contentTable" width="100%" border="0" cellspacing="1" cellpadding="1"
                    style="font-size: 12px; border-collapse: collapse">
                    <tr>
                        <td colspan="10">
                            <div id="divhead" runat="server" style="width: 100%">
                                <table width="100%" border="1" cellspacing="1" cellpadding="1" style="font-size: 12px;
                                    border-collapse: collapse">
                                    <tr>
                                        <td class="RouteListStyle1">
                                            Fty CD:
                                        </td>
                                        <td>
                                        </td>
                                        <td class="RouteListStyle2">
                                            User:
                                        </td>
                                        <td>
                                        </td>
                                        <td class="RouteListStyle">
                                            Create Date:
                                        </td>
                                        <td>
                                        </td>
                                        <td class="RouteListStyle">
                                            Printed Date:
                                        </td>
                                        <td>
                                        </td>
                                        <td class="RouteListStyle">
                                            Job Order No.
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="RouteListStyle1">
                                            GO No.
                                        </td>
                                        <td>
                                        </td>
                                        <td class="RouteListStyle2">
                                            Job Order Date -entry
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td class="RouteListStyle">
                                            Order Qty (Pcs) :
                                        </td>
                                        <td>
                                        </td>
                                        <td class="RouteListStyle">
                                            Order Qty (Doz):
                                        </td>
                                        <td>
                                        </td>
                                        <td class="RouteListStyle">
                                            Wash Type:
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="RouteListStyle1">
                                            Buyer Code:
                                        </td>
                                        <td>
                                        </td>
                                        <td class="RouteListStyle2">
                                            Buyer name:
                                        </td>
                                        <td colspan="3">
                                        </td>
                                        <td class="RouteListStyle">
                                            FDS No:
                                        </td>
                                        <td>
                                        </td>
                                        <td class="RouteListStyle">
                                            Production Line:
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="RouteListStyle1">
                                            Style No.
                                        </td>
                                        <td>
                                        </td>
                                        <td class="RouteListStyle2">
                                            Style Description:
                                        </td>
                                        <td colspan="5">
                                        </td>
                                        <td class="RouteListStyle2">
                                            Process Code
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="RouteListStyle1">
                                            Remarks:
                                        </td>
                                        <td colspan="16">
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" border="1" cellspacing="1" cellpadding="1" style="font-size: 12px;
                                border-collapse: collapse">
                                <tr>
                                    <td>
                                        <div id="divBody" runat="server">
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="4" align="left">
                <table border="1" cellspacing="1" cellpadding="1">
                    <tr>
                        <td colspan="6">
                            <br />
                            <br />
                            <strong>Total Piece Rate By Process</strong>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div id="divSummary" runat="server">
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr align="right">
            <td colspan="17">
                <table>
                    <tr>
                        <td class="RouteListStyle" style='border: 0'>
                            Prepared by:
                        </td>
                        <td style="width: 8em">
                        </td>
                        <td class="RouteListStyle" style="border: 0">
                            Checked by:
                        </td>
                        <td style="width: 8em">
                        </td>
                        <td class="RouteListStyle" style="border: 0">
                            Approved by:
                        </td>
                        <td style="width: 8em">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
