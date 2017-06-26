<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wipDailyYMG.aspx.cs" Inherits="Reports_wipDailyYMG" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>JO Daily Report</title>
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
        .style3
        {
            height: 29px;
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
            //            if (document.all.txtStartDate.value == '') {
            //                document.all.ExcTable.style.display = 'none';
            //            }
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
                        <asp:DropDownList ID="ddlFtyCd" runat="server" AutoPostBack="True" Enabled="false"
                            OnSelectedIndexChanged="ddlFtyCd_SelectedIndexChanged">
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
                    <td class="style1">
                        Job Order No.:
                    </td>
                    <td class="style2">
                        <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                        <input type="button" value="..." onclick="searchJo()" class="button_top" />
                    </td>
                    <td height="19" class="tdbackcolor">
                        Group Name
                    </td>
                    <td>
                        <asp:DropDownList ID="ddGroupName" runat="server" Enabled="false">
                            <asp:ListItem Value="">&nbsp;</asp:ListItem>
                            <asp:ListItem Value="YM梭织车缝1">梭织车缝1(W17 - W29)</asp:ListItem>
                            <asp:ListItem Value="YM梭织车缝2">梭织车缝2(W11 - W16)</asp:ListItem>
                            <asp:ListItem Value="YM梭织车缝3">梭织车缝3(W1 - W10)</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td height="19" class="tdbackcolor">
                        Process Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlprocessCd" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlprocessCd_SelectedIndexChanged">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td height="19" class="tdbackcolor">
                        Production Line :
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlprodLine" runat="server">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td height="19" class="tdbackcolor">
                        To Process Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlToprocessCd" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlToprocessCd_SelectedIndexChanged">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td height="19" class="tdbackcolor">
                        To Production Line :
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlToprodLine" runat="server">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td height="19" class="tdbackcolor">
                        Process Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProcessType" runat="server" AutoPostBack="True">
                            <asp:ListItem ></asp:ListItem>
                           
                        </asp:DropDownList>
                    </td>
                    <td height="19" class="tdbackcolor">
                        Production Factory :
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProdFactory" runat="server">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td height="19" class="tdbackcolor">
                        To Process Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlToProcessType" runat="server" AutoPostBack="True">
                            <asp:ListItem ></asp:ListItem>
                         
                        </asp:DropDownList>
                    </td>
                    <td height="19" class="tdbackcolor">
                        To Production Factory :
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlToProdFactory" runat="server">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td height="19" class="tdbackcolor">
                        To Garment Type :
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlToGarmentType" runat="server">
                            <asp:ListItem Value=""></asp:ListItem>
                            <asp:ListItem Value="K">Knit</asp:ListItem>
                            <asp:ListItem Value="W">Woven</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td height="19" class="tdbackcolor">
                        Transaction From Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtStartDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td height="19" class="tdbackcolor">
                        To Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtEndDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td height="19" class="tdbackcolor">
                        Sort :
                    </td>
                    <td>
                        <asp:DropDownList ID="OrderByList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="OrderByList_SelectedIndexChanged">
                            <asp:ListItem Value="1">PROCESS SEQ</asp:ListItem>
                            <asp:ListItem Value="2">PRODUCTION LINE</asp:ListItem>
                            <asp:ListItem Value="3">JO</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td align="right" colspan="2">
                        <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <asp:Label runat="server" ID="Msg" Font-Size="Large" Visible="false"><font color="red">Please enter the correct queries.</font></asp:Label>
        <hr noshade size="1">
        <table id="ExcTable" width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="center" class="style3">
                    <h2>
                        Daily Production Transaction by JO &amp; Section (pcs)</h2>
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
                                <%=ddlprocessCd.SelectedItem.Text %>
                            </td>
                            <td class="tr2style" width="100">
                                To Process CD:
                            </td>
                            <td>
                                <%=ddlToprocessCd.SelectedItem.Text %>
                            </td>
                            <td class="tr2style" width="100">
                                Transaction From Date:
                            </td>
                            <td>
                                <%=txtStartDate.Text %>
                            </td>
                            <td class="tr2style" width="100">
                                To Date:
                            </td>
                            <td>
                                <%=txtEndDate.Text %>
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
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <strong><b>Summary(By Production Line)</b></strong>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divSummary" runat="server">
                    </div>
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
