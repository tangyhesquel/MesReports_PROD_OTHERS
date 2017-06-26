<%@ Page Language="C#" AutoEventWireup="true" CodeFile="cuttingDateReport.aspx.cs"
    Inherits="Reports_CuttingDateReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Cutting Date Report</title>
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
        .style1
        {
            width: 107px;
        }
        .style2
        {
            width: 98px;
        }
    </style>
    <script type="text/javascript" src="../Script/Common.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <table border="0" align="center" width="100%">
        <tr>
            <td>
                <div id="conditiondiv">
                    <fieldset>
                        <legend>Search</legend>
                        <table width="100%" id="queryTab">
                            <tr>
                                <td class="tdbackcolor">
                                    Factory :
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlFactory" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td class="tdbackcolor">
                                    JO No.:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                                </td>
                                
                                <td class="tdbackcolor">
                                    Production Group:
                                </td>
                                <td class="style2">
                                    <asp:DropDownList ID="ddGroupName" runat="server">
                                        <asp:ListItem Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                
                                <td class="tdbackcolor">
                                    Date From :
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBeginDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                                </td>
                                <td class="tdbackcolor">
                                    Date To:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtEndDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdbackcolor">
                                    Buyer Code.:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBuyerCode" runat="server"></asp:TextBox>
                                </td>
                                <td class="tdbackcolor">
                                    Garment Type:
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlGarmentType" runat="server">
                                        <asp:ListItem Value="">All</asp:ListItem>
                                        <asp:ListItem Value="W">Woven</asp:ListItem>
                                        <asp:ListItem Value="K">Knit</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:CheckBox ID="cbLine" runat="server" Text="Show Prod.Line" />
                                </td>
                                <td colspan="3">
                                    <input type="button" name="reSet" value="Reset" onclick="reset()" style="height: 26"
                                        class="button_top" />
                                    <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" 
                                        OnClick="btnQuery_Click" Width="76px" />
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div id="mmPrint" align="right">
                    <input type="button" name="print3" value="print" style="font-size: 16; width: 80;
                        height: 26" onclick="mmPrint.style.display='none';conditiondiv.style.display='none';WebBrowser1.ExecWB(6,1);mmPrint.style.display='';conditiondiv.style.display=''" />
                    <input type="button" name="print2" value="printview" style="font-size: 16; width: 80;
                        height: 26" onclick="mmPrint.style.display='none';conditiondiv.style.display='none';WebBrowser1.ExecWB(7,1);mmPrint.style.display='';conditiondiv.style.display=''" />
                    <input type="button" name="print" value="page setup" style="font-size: 16; width: 100;
                        height: 26" onclick="mmPrint.style.display='none';conditiondiv.style.display='none';WebBrowser1.ExecWB(8,1);mmPrint.style.display='';conditiondiv.style.display=''" />
                    <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" />
                    <input type="button" name="excel" value="To Wps" onclick="ToExcelOfWPS()" />
                </div>
            </td>
        </tr>
    </table>
    <div id="ExcTable" runat="server">
    </div>
    </form>
</body>
</html>
