<%@ Page Language="C#" AutoEventWireup="true" CodeFile="proCycleSummary.aspx.cs"
    Inherits="Reports_proCycleSummary" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Production Cycle Time Summary Report</title>
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
    <script language="javascript" type="text/javascript">
        function toPcmExcel() {

            var myExcel, myBook;
            myExcel = new ActiveXObject("Excel.Application");
            if (myExcel != null) {
                var sel = document.body.createTextRange();
                sel.moveToElementText(reportDiv);
                sel.select();
                document.execCommand('Copy');
                document.execCommand('Unselect');

                myExcel.Visible = true;
                myBook = myExcel.Workbooks.Add();
                myBook.sheets(1).paste

            }
        }
        function toPcmWPS() {

            var myExcel, myBook;
            myExcel = new ActiveXObject("ET.Application");
            if (myExcel != null) {
                var sel = document.body.createTextRange();
                sel.moveToElementText(reportDiv);
                sel.select();
                document.execCommand('Copy');
                myExcel.Visible = true;
                myBook = myExcel.Workbooks.Add();
                myBook.sheets("Sheet1").paste();
                document.execCommand('Unselect');
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="queryDiv" runat="server">
        <fieldset>
            <legend>Search</legend>
            <table width="100%" id="queryTab" style="text-align: center">
                <tr>
                    <td width="10%" height="19" class="tdbackcolor">
                        Factory Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFactoryCode" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td width="10%" height="19" class="tdbackcolor">
                        Garment Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlGarmentType" runat="server">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="W">Woven</asp:ListItem>
                            <asp:ListItem Value="K">Knit</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td width="10%" height="19" class="tdbackcolor">
                        Wash Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlWashType" runat="server">
                            <asp:ListItem></asp:ListItem>
                            <asp:ListItem Value="NOIRON">WRINKLE FREE</asp:ListItem>
                            <asp:ListItem Value="IRON">No WRINKLE FREE</asp:ListItem>
                            <asp:ListItem Value="WASH">Wash</asp:ListItem>
                            <asp:ListItem Value="NW">No Wash</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td width="8%" height="19" class="tdbackcolor">
                        Outsouce Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlOutsourceType" runat="server">
                            <asp:ListItem Value="">ALL</asp:ListItem>
                            <asp:ListItem Value="OUTSOURCE">Outsource</asp:ListItem>
                            <asp:ListItem Value="STANDARD">Standard</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td width="10%" height="19" class="tdbackcolor">
                        Start Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtStartDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td width="10%" height="19" class="tdbackcolor">
                        End Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtEndDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td class="tdbackcolor">
                        <asp:CheckBox ID="cbCheck" runat="server" Text="After 1 Mar" />
                    </td>
                    <td colspan="3" align="right">
                        <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade size="1" />
        <div id="reportDiv" runat="server">
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td width="20%">
                        &nbsp;
                    </td>
                    <td width="60%">
                        &nbsp;
                    </td>
                    <td width="20%" align="center">
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td align="center">
                        <font face="Arial" size="4">Production Cycle Time Summary Report(days)</font>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td width="20%">
                        &nbsp;
                    </td>
                    <td width="60%" align="center">
                        &nbsp;
                    </td>
                    <td width="20%" align="center">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="left">
                    </td>
                </tr>
            </table>
            <div id="mmPrint" align="right" style="width: 100%">
                <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv.style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv.style.display='block';document.all.mmPrint.style.visibility='visible'"
                    style="font-size: 16; width: 80; height: 26" />
                <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,'Production Cycle Time Summary Report.htm')"
                    style="font-size: 16; width: 80; height: 26" />
                <input type="button" name="ToExcel" value="To Excel" onclick="toPcmExcel()" style="font-size: 16;
                    width: 80; height: 26" />
                <input type="button" name="ToExcel" value="To WPS" onclick="toPcmWPS()" style="font-size: 16;
                    width: 80; height: 26" />
            </div>
            <br />
            <table id="allTable" width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                border-collapse: collapse">
                <tr>
                    <td>
                        <table width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                            border-collapse: collapse">
                            <tr>
                                <td class="tr2style" bgcolor="#efefe7" width="100">
                                    Factory CD:
                                </td>
                                <td>
                                    <%=ddlFactoryCode.SelectedItem.Text %>&nbsp;
                                </td>
                                <td class="tr2style" bgcolor="#efefe7" width="100">
                                    Garment Type:
                                </td>
                                <td>
                                    <%=ddlGarmentType.SelectedItem.Text%>&nbsp;
                                </td>
                                <td class="tr2style" bgcolor="#efefe7" width="100">
                                    Wash Type:
                                </td>
                                <td>
                                    <%=ddlWashType.SelectedItem.Text%>&nbsp;
                                </td>
                                <td class="tr2style" bgcolor="#efefe7" width="100">
                                    Start Date:
                                </td>
                                <td>
                                    <%=txtStartDate.Text %>&nbsp;
                                </td>
                                <td class="tr2style" bgcolor="#efefe7" width="100">
                                    End Date:
                                </td>
                                <td>
                                    <%=txtEndDate.Text %>&nbsp;
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
                        <div id="divBody" runat="server">
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
