<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FabricUnitPriceQuery.aspx.cs"
    Inherits="Reports_FabricUnitPriceQuery" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>FabricUnitPriceQuery</title>
    <style type="text/css">
        .bigfont2
        {
            font-weight: bold;
            font-size: 14px;
            font-family: 黑体,Arial,Times New Roman,Verdana,Helvetica, sans-serif;
        }
        UNKNOWN
        {
            padding-right: 5px;
            padding-left: 5px;
            scrollbar-face-color: #eeeee7;
            font-size: 11px;
            padding-bottom: 5px;
            scrollbar-highlight-color: #eeeeee;
            scrollbar-shadow-color: #000000;
            scrollbar-3dlight-color: #ffffff;
            scrollbar-arrow-color: #000000;
            padding-top: 5px;
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
            border-right: #333333 1px solid;
            border-top: #333333 1px solid;
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            border-left: #333333 1px solid;
            cursor: hand;
            border-bottom: #333333 1px solid;
            background-color: #efefef;
        }
        .BUTTON
        {
            border-right: #333333 1px solid;
            padding-right: 1px;
            border-top: #333333 1px solid;
            padding-left: 1px;
            font-weight: bolder;
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            border-left: #333333 1px solid;
            cursor: hand;
            color: #646400;
            padding-top: 1px;
            border-bottom: #333333 1px solid;
            height: 20px;
            background-color: #cccc99;
        }
        .BUTTON_down
        {
            border-right: #646400 1px solid;
            padding-right: 1px;
            border-top: #646400 1px solid;
            padding-left: 1px;
            font-weight: bolder;
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            border-left: #646400 1px solid;
            cursor: hand;
            color: #646400;
            padding-top: 1px;
            border-bottom: #646400 1px solid;
            height: 20px;
            background-color: #efefe7;
        }
        .input_gary
        {
            border-right: #333333 1px solid;
            border-top: #333333 1px solid;
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            border-left: #333333 1px solid;
            border-bottom: #333333 1px solid;
            background-color: #efefef;
        }
        .input_color
        {
            border-right: #333333 1px solid;
            border-top: #333333 1px solid;
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            border-left: #333333 1px solid;
            border-bottom: #333333 1px solid;
            background-color: #eef7ff;
        }
        .input_white
        {
            border-right: #333333 1px solid;
            border-top: #333333 1px solid;
            padding-left: 2px;
            font-size: 11px;
            border-left: #333333 1px solid;
            border-bottom: #333333 1px solid;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            height: 18px;
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
            padding-right: 5px;
            padding-left: 5px;
            font-size: 11px;
            padding-top: 2px;
            background-color: #ffffff;
        }
        .tr2style
        {
            padding-right: 5px;
            padding-left: 5px;
            font-weight: bolder;
            font-size: 11px;
            padding-top: 2px;
            background-color: #efefe7;
        }
        .tr3style
        {
            padding-left: 5px;
            font-weight: bolder;
            font-size: 11px;
            padding-top: 2px;
            background-color: #ffffff;
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
        .bule
        {
            font-weight: 600;
            color: #006699;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        TD
        {
            word-break: break-all;
            word-wrap: break-word;
        }
    </style>
    <script type="text/javascript" src="../Script/Common.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <table width="100%">
        <tr>
            <td colspan="10" align="right">
                <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="height: 26"
                    class="button_top">
                <input type="button" name="excel" value="To Wps" onclick="ToExcelOfWPS()" style="height: 26"
                    class="button_top" />
            </td>
        </tr>
        <tr>
            <td>
                <div id="ExcTable">
                    <div id="table1" runat="server">
                    </div>
                    <br />
                    <br />
                    <br />
                    <div id="table2" runat="server">
                    </div>
                </div>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
