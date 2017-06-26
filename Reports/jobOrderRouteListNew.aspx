<%@ Page Language="C#" AutoEventWireup="true" CodeFile="jobOrderRouteListNew.aspx.cs"
    Inherits="Reports_jobOrderRouteList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Job Order Route List</title>
    <%--<link href="../Css/StyleReport.css" rel="Stylesheet" />--%>
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
            width: 290px;
            position: absolute;
            height: 300px;
            overflow-y: auto;
            overflow-x: hidden;
            border-style: solid;
            border-color: Gray;
            border-width: 1px;
            font-size: 8px;
        }
        
        .CheckBoxList
        {
            position: relative;
            width: 260px;
            height: 10px;
            overflow: scroll;
            font-size: small;
        }
    </style>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script language="javascript" type="text/javascript">

        function selectColumn(user_id) {
            var str = window.showModalDialog("ReportCustomSetting.aspx?site=" + document.all.ddlfactoryCd.value + "&ReportName=JORouteListReport&User_ID=" + user_id, null, "font-size:10px;dialogWidth:60em;dialogHeight:50em;scrollbars=no;status=no");
            //SetColumnsDisplay(str);
        }

        function searchJo() {
            var urlName = "searchJO.aspx?factory=" + document.all.ddlfactoryCd.value + "&site=" + document.all.ddlfactoryCd.value + "&userRandom=" + (Math.random() * 100000);
            var jo = window.showModalDialog(urlName, "Job Order No.", "dialogWidth=650pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (jo == null) return;
            document.all.txtJoNo.value = jo;
        }


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
        .style2
        {
            width: 291px;
        }
        .style4
        {
            font-family: "Arial Unicode MS";
            font-size: large;
        }
        .style6
        {
            font-weight: bolder;
            font-size: 11px;
            width: 183px;
            background-color: #efefe7;
            padding-left: 5px;
            padding-right: 5px;
            padding-top: 2px;
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
                        DataValueField="DEPARTMENT_ID">
                    </asp:DropDownList>
                </td>
                <td class="tdbackcolor">
                    Job Order No.:
                </td>
                <td>
                    <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                    <input type="button" value="..." onclick="searchJo()" class="button_top" />
                </td>
                <td class="tdbackcolor">
                    Create Date:
                </td>
                <td>
                    <asp:TextBox ID="txtDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                </td>
                <td class="tdbackcolor">
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
                <td class="tdbackcolor">
                    RowNo:
                </td>
                <td class="tdbackcolor">
                    <asp:CheckBox runat="server" ID="RowNo" Text="" Checked="false" />
                </td>
                <td>
                    <asp:TextBox ID="TxtRowNo" Text="50" runat="server" Width="83px"></asp:TextBox>
                </td>
                <td align="right">
                    <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="9" align="right">
                    <div id="mmPrint" style="text-align: right;">
                        <input type="button" name="excel" value="Custom Column" onclick="javascript:selectColumn('<%=user_id%>')"
                            style="height: 26" class="button_top" />
                        <input type="button" name="print" value="Print" onclick="ToPrint1()" style="height: 26"
                            class="button_top" />
                        <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,document.title+'.htm')"
                            style="height: 26" class="button_top" />
                        <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="height: 26"
                            class="button_top" />
                        <input type="button" onclick="ToExcelOfWPS()" value="To WPS" style="height: 26" class="button_top" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
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
                                        <td colspan="7">
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
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="4" align="left">
                <table border="1" cellspacing="1" cellpadding="1">
                    <tr>
                        <td colspan="4">
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
    </table>
    </td> </tr> </table>
    </form>
</body>
</html>
