<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CheckPointOutput.aspx.cs"
    Inherits="Reports_wipDaily" CodePage="65001" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Check Point Output Report</title>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
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
        .style1
        {
            background-color: #EFEFE0;
            height: 19px;
        }
        .style2
        {
            height: 19px;
        }
        .DivClose
        {
            display: none;
            position: absolute;
            width: 250px;
            height: 220px;
            border-style: solid;
            border-color: Gray;
            border-width: 1px;
            background-color: #99A479;
        }
        
        .LabelClose
        {
            vertical-align: text-top;
            position: absolute;
            bottom: 0px;
            font-family: Verdana;
        }
        
        .DivCheckBoxList
        {
            display: none;
            background-color: White;
            width: 250px;
            position: absolute;
            height: 200px;
            overflow-y: auto;
            overflow-x: hidden;
            border-style: solid;
            border-color: Gray;
            border-width: 1px;
        }
        
        .CheckBoxList
        {
            position: relative;
            width: 250px;
            height: 10px;
            overflow: scroll;
            font-size: small;
        }
    </style>
    <script language="javascript" type="text/javascript">
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
        function searchJo() {
            var urlName = "searchJO.aspx?factory=" + document.all.ddlFtyCd.value + "&userRandom=" + (Math.random() * 100000);
            var jo = window.showModalDialog(urlName, "Job Order No.", "dialogWidth=650pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (jo == null) return;
            document.all.txtJoNo.value = jo;
        }
        function init() {
            if (document.all.txtStartDate.value == '') {
                document.all.ExcTable.style.display = 'none';
            }
        }
    </script>
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
    </script>
    <script type="text/javascript">
        var timoutID;

        //This function shows the checkboxlist
        function ShowMList() {
            var divRef = document.getElementById("divCheckBoxList");
            divRef.style.display = "block";
            var divRefC = document.getElementById("divCheckBoxListClose");
            divRefC.style.display = "block";
        }

        //This function hides the checkboxlist
        function HideMList() {
            document.getElementById("divCheckBoxList").style.display = "none";
            document.getElementById("divCheckBoxListClose").style.display = "none";
        }

        //This function finds the checkboxes selected in the list and using them,
        //it shows the selected items text in the textbox (comma separated)
        function FindSelectedItems(sender, textBoxID) {
            var cblstTable = document.getElementById(sender.id);
            var checkBoxPrefix = sender.id + "_";
            var noOfOptions = cblstTable.rows.length;
            var selectedText = "";
            for (i = 0; i < noOfOptions; ++i) {
                if (document.getElementById(checkBoxPrefix + i).checked) {
                    if (selectedText == "")
                        selectedText = document.getElementById
                                   (checkBoxPrefix + i).parentNode.innerText;
                    else
                        selectedText = selectedText + "," +
                 document.getElementById(checkBoxPrefix + i).parentNode.innerText;
                }
            }
            document.getElementById(textBoxID.id).value = selectedText;
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
                    <td width="5%" class="style1">
                        Factory:
                    </td>
                    <td class="style2">
                        <asp:DropDownList ID="ddlFtyCd" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlFtyCd_SelectedIndexChanged"
                            Enabled="False">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="style1">
                        Garment Type:
                    </td>
                    <td class="style2">
                        <asp:DropDownList ID="ddlgarmentType" runat="server" OnSelectedIndexChanged="ddlGMT_SelectedIndexChanged"
                            AutoPostBack="true">
                            <asp:ListItem Value=""></asp:ListItem>
                            <asp:ListItem Value="K">Knit</asp:ListItem>
                            <asp:ListItem Value="W">Woven</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td height="19" class="tdbackcolor">
                        Process Code:
                    </td>
                    <td>
                        <div id="divCustomCheckBoxList" runat="server" onmouseover="clearTimeout(timoutID);"
                            onmouseout="timoutID = setTimeout('HideMList()', 750);">
                            <table>
                                <tr>
                                    <td align="right" class="DropDownLook">
                                        <input id="txtSelectedMLValues" type="text" readonly="readonly" onclick="ShowMList()"
                                            style="width: 229px;" runat="server" />
                                    </td>
                                    <td align="left" class="DropDownLook">
                                        <img id="imgShowHide" runat="server" src="drop.gif" onclick="ShowMList()" align="left" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" class="DropDownLook">
                                        <div>
                                            <div runat="server" id="divCheckBoxListClose" class="DivClose">
                                                <label runat="server" onclick="HideMList();" class="LabelClose" id="lblClose">
                                                    OK</label>
                                            </div>
                                            <div runat="server" id="divCheckBoxList" class="DivCheckBoxList">
                                                <asp:CheckBoxList ID="lstMultipleValues" runat="server" Width="250px" CssClass="CheckBoxList">
                                                </asp:CheckBoxList>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td height="19" class="tdbackcolor">
                        From Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtStartDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td height="19" class="tdbackcolor">
                        End Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtToDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="6">
                        <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade size="1" />
        <table id="ExcTable" width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="center">
                    <h2>
                        Check Point Output Report</h2>
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
                        <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv .style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv .style.display='block';document.all.mmPrint.style.visibility='visible'"
                            style="font-size: 16; width: 80; height: 26" />
                        <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,'ActualCuttingReport.htm')"
                            style="font-size: 16; width: 80; height: 26" />
                        <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="font-size: 16;
                            width: 80; height: 26" /><input type="button" onclick="ToExcelOfWPS()" value="To WPS"
                                style="font-size: 16; width: 80; height: 26" />
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
                    <table border="1" cellspacing="0" cellpadding="0" style="font-size: 12px; border-collapse: collapse"
                        width="100%">
                        <tr>
                            <td class="tr2style" width="100">
                                Factory CD:
                            </td>
                            <td>
                                <%=ddlFtyCd.SelectedItem.Value %>
                            </td>
                            <td class="tr2style" width="100">
                                Process CD:
                            </td>
                            <td>
                                <%=txtSelectedMLValues.Value%>
                            </td>
                            <td class="tr2style" width="100">
                                Transaction Date:
                            </td>
                            <td>
                                From:&nbsp;&nbsp;<%=txtStartDate.Text %>
                            </td>
                            <td>
                                To:&nbsp;&nbsp;<%=txtToDate.Text%>
                            </td>
                            <td class="tr2style" width="100">
                                Print Date:
                            </td>
                            <td colspan="2">
                                <%=DateTime.Now%>
                            </td>
                        </tr>
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
                    <table border='1' cellspacing='1' cellpadding='1' style='font-size: 12px; border-collapse: collapse'>
                        <tr class='tr2style' align='center'>
                            <td>
                                GONO
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal3" Text="<%$Resources:GlobalResources,STRING_JO_NO%>"> </asp:Literal>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal1" Text="<%$Resources:GlobalResources,STRING_CUSTOMER%>"> </asp:Literal>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal2" Text="<%$Resources:GlobalResources,STRING_STYLE_NO%>"> </asp:Literal>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal4" Text="<%$Resources:GlobalResources,STRING_GMT_TYPE1%>"> </asp:Literal>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal5" Text="<%$Resources:GlobalResources,STRING_WASH_TYPE%>"> </asp:Literal>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal6" Text="<%$Resources:GlobalResources,STRING_FELL_TYPE%>"> </asp:Literal>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal7" Text="<%$Resources:GlobalResources,STRING_FABRIC_TYPE%>"> </asp:Literal>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal8" Text="<%$Resources:GlobalResources,STRING_PODATE1%>"> </asp:Literal>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal9" Text="<%$Resources:GlobalResources,STRING_ORDER_QTY%>"> </asp:Literal>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal10" Text="<%$Resources:GlobalResources,STRING_ACTUAL_QTY%>"> </asp:Literal>
                            </td>
                            <td>
                                SAH
                            </td>
                            <!--td-->
                            <div id="divBody" runat="server">
                            </div>
                            <!--/td-->
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
