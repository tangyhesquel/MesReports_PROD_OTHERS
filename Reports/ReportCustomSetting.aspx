<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReportCustomSetting.aspx.cs"
    Inherits="Reports_ReportCustomSetting" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Custom Report</title>
    <object id="WebBrowser1" width="0" height="0" classid="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2">
    </object>
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
            font-size: 12px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            height: 18px;
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
        .tr2style
        {
            font-weight: bolder;
            font-size: 12px;
            background-color: #efefe7;
            padding-left: 5px;
            padding-right: 5px;
            padding-top: 2px;
        }
        .tr3style
        {
            font-size: 13px;
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
        .style2
        {
            width: 174px;
        }
        .style3
        {
            width: 197px;
        }
        .style4
        {
            width: 156px;
        }
        .style5
        {
            width: 107px;
        }
    </style>
    <meta http-equiv="keywords" content="keyword1,keyword2,keyword3">
    <meta http-equiv="description" content="this is my page">
    <meta http-equiv="content-type" content="text/html; charset=GB18030">
    <!--<link rel="stylesheet" type="text/css" href="./styles.css">-->
    <script type="text/javascript">


        //全不选
        function unselectall() {
            if (document.form1.chkAll.checked) {
                document.form1.chkAll.checked = document.form1.chkAll.checked & 0;
            }
        }

        //全选
        function CheckAll(form) {
            for (var i = 0; i < form.elements.length; i++) {
                var e = form.elements[i];
                if (e.name != 'chkAll' && e.disabled == false)
                    e.checked = form.chkAll.checked;
            }
        }

        function clickCancel() {
            //window.returnValue = "";
            window.close();
        }

        function SaveSettings(fm, fty, user_id) {
            var a = "";
            for (var i = 0; i < fm.elements.length; i++) {
                var e = fm.elements[i];
                if (e.name != 'chkAll' && (e.type == 'checkbox' || e.type == 'hidden')) {
                    if (e.type == 'checkbox' && e.checked) {
                        a = a == "" ? e.value : a + "," + e.value;
                    }
                    if (e.type == 'hidden' && e.name == 'cln') {
                        a = a == "" ? e.value : a + "," + e.value;
                    }
                }
            }
            window.showModalDialog("SaveCustomSetting.aspx?User_ID=<%=strusername%>&ReportName=<%=Report_Name %>&FtyCD=" + fty + "&Cstring=" + a + "", window, "dialogWidth=350px;dialogHeight=80px");
            return true;
        }

        function SaveSettings_backup(fm, fty, user_id) {
            var a = "";
            for (var i = 0; i < fm.elements.length; i++) {
                var e = fm.elements[i];
                if (e.name != 'chkAll' && e.type == 'checkbox') {
                    if (e.checked)
                        a = a == "" ? e.value : a + "," + e.value;
                }
            }
            window.showModalDialog("SaveCustomSetting.aspx?User_ID=<%=strusername%>&ReportName=<%=Report_Name %>&FtyCD=" + fty + "&Cstring=" + a + "", window, "dialogWidth=350px;dialogHeight=80px");
            return true;
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <table border="0" align="center" width="100%">
        <tr>
            <td>
                <div id="conditiondiv">
                    <table width="100%" id="queryTab">
                        <tr>
                            <td class="tdbackcolor" width="50">
                                Factory :
                            </td>
                            <td width="50">
                                <asp:DropDownList ID="ddlFactory" runat="server" Enabled="False" Width="50px">
                                </asp:DropDownList>
                            </td>
                            <td class="tdbackcolor" width="50">
                                Report Name:
                            </td>
                            <td width="250" colspan="2">
                                <asp:TextBox ID="ReportName" runat="server" Enabled="False" Width="250"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td width="100" colspan="2">
                                <input type="checkbox" value="" onclick="CheckAll(this.form)" name="chkAll" />
                                Select All
                            </td>
                            <td width="80">
                                <input type="button" name="Save" onclick="SaveSettings(this.form,'<%=ddlFactory.SelectedItem.Text%>','<%=strusername%>')"
                                    value="Save" style="height: 26; width: 80" class="button_top" />
                            </td>
                            <td width="80">
                                <input type="button" name="cancelbtn" onclick="clickCancel()" value="Close" style="height: 26;
                                    width: 80" class="button_top" />
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
    <div id="ExcTable" runat="server">
    </div>
    </form>
</body>
</html>
