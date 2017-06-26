<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wipDailyWip.aspx.cs" Inherits="Reports_wipDailyWip" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>JO Daily WIP Report</title>
    <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <script language="javascript" type="text/javascript">
        function init() {
            if (document.all.txtStartDate.value == '') {
                document.all.ExcTable.style.display = 'none';
            }
        }
    </script>
    <script type="text/javascript" src="../Script/Common.js"></script>
    <style type="text/css">
        BODY
        {
            scrollbar-face-color: #eeeee7;
            font-size: 11px;
            padding: 5px;
            scrollbar-highlight-color: #eeeeee;
            scrollbar-shadow-color: #000000;
            scrollbar-3dlight-color: #ffffff;
            scrollbar-arrow-color: #000000;
            scrollbar-track-color: #efefef;
            font-family: Arial,Times New Roman, 黑体,Verdana,Helvetica, sans-serif;
            scrollbar-darkshadow-color: #ffffff;
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
</head>
<body>
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search</legend>
            <table width="100%" id="queryTab">
                <tr>
                    <td height="19" class="tdbackcolor">
                        Factory:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFtyCd" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td height="19" class="tdbackcolor">
                        Garment Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlGarmentType" runat="server">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="K">Knit</asp:ListItem>
                            <asp:ListItem Value="W">Woven</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    
                    <td class="tdbackcolor">
                        Production Group:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddGroupName" runat="server">
                            <asp:ListItem Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    
                    <td height="19" class="tdbackcolor">
                        Wash Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlWashType" runat="server">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="WASH">Wash</asp:ListItem>
                            <asp:ListItem Value="NW">No Wash</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td height="19" class="tdbackcolor">
                        Transation Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                        <td>
                            &nbsp;
                        </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade size="1">
    </div>
    <table id="ExcTable" width="100%" border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td align="center">
                <font face="Arial" size="4">JO Daily WIP Report</font>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <div id="mmPrint" align="right" width="100%">
                    <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv .style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv .style.display='block';document.all.mmPrint.style.visibility='visible'"
                        style="font-size: 16; width: 80; height: 26" />
                    <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,'ActualCuttingReport.htm')"
                        style="font-size: 16; width: 80; height: 26" />
                    <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="font-size: 16;
                        width: 80; height: 26" />
                    <input type="button" name="ToWps" value="ToWps" onclick="ToExcelOfWPS()" style="font-size: 16;
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
                <table border="1" cellspacing="0" cellpadding="0" style="font-size: 12px; border-collapse: collapse">
                    <tr>
                        <td class="tr2style" width="100">
                            Factory CD:
                        </td>
                        <td width="100">
                            <%=ddlFtyCd.SelectedItem.Text%>&nbsp;
                        </td>
                        <td class="tr2style" width="100">
                            Garment type:
                        </td>
                        <td width="100">
                            <%=ddlGarmentType.SelectedItem.Text%>&nbsp;
                        </td>
                        <td class="tr2style" width="100">
                            Wash Type:
                        </td>
                        <td width="100">
                            <%=ddlWashType.SelectedItem.Value%>&nbsp;
                        </td>
                        <td class="tr2style" width="100">
                            Transaction Date:
                        </td>
                        <td width="100">
                            <%=txtDate.Text%>&nbsp;
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
    </form>
</body>
</html>
