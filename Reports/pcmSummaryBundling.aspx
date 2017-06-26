<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pcmSummaryBundling.aspx.cs"
    Inherits="Reports_pcmSummaryBundling" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Summary Bundling Report</title>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script language="javascript">
        function init() {
            if (document.all.txtJoNo.value == '') {
                document.all.ExcTable.style.display = 'none';
            }
        }

        function searchJo() {
            //var url = "/reports/jsp/wip/SearchJO.jsp?factory="+document.all.factoryCd.value+"&userRandom="+(Math.random() * 100000);
            var url = "/msReports/searchJO.do?factory=" + document.all.factoryCd.value + "&userRandom=" + (Math.random() * 100000);

            var urlName = "SearchJO.aspx?factory=" + document.all.txtfactoryCd.value + "&userRandom=" + (Math.random() * 100000) + "";
            var jo = window.showModalDialog(urlName, "Job Order No.", "dialogWidth=550pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (jo == null) return;
            document.all.txtJoNo.value = jo;
        }
        function query() {
            if (document.all.txtJoNo.value == '') {
                alert("Job Order No. is Empty.");
                document.all.txtJoNo.focus();
                return false;
            }
            else {
                //document.all.actionFlag.value='query'
                return true;
            }

        }
    </script>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
</head>
<body onload="init()">
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search</legend>
            <table width="100%" id="queryTab">
                <tr>
                    <td width="12%" class="tdbackcolor">
                        Factory Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlfactoryCd" runat="server" DataTextField="FACTORY_ID" DataValueField="FACTORY_ID">
                        </asp:DropDownList>
                    </td>
                    <td width="12%" class="tdbackcolor">
                        Job Order No.:
                    </td>
                    <td>
                        <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                    </td>
                    <td width="12%" class="tdbackcolor">
                        Lay No From:
                    </td>
                    <td>
                        <asp:TextBox ID="txtlayNoFrom" runat="server"></asp:TextBox>
                    </td>
                    <td width="10%" class="tdbackcolor">
                        Lay No To:
                    </td>
                    <td>
                        <asp:TextBox ID="txtlayNoTo" runat="server"></asp:TextBox>
                    </td>
                    <td width="24%" class="tdbackcolor">
                        Actual Printed Date From:
                    </td>
                    <td>
                        <asp:TextBox ID="txtStartDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td width="20%" class="tdbackcolor">
                        Actual Printed Date To:
                    </td>
                    <td>
                        <asp:TextBox ID="txtEndDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <tr>
                        <td colspan="2" class="tdbackcolor">
                            Lay No (Use "," to split)
                        </td>
                        <td colspan="8">
                            <asp:TextBox ID="LayNos" runat="server" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                        </td>
                        <td width="20%">
                            <asp:Button ID="btnQurey" runat="server" Text="Query" CssClass="button_top" OnClick="btnQurey_Click"
                                OnClientClick="query()" />
                        </td>
                    </tr>
                </tr>
                <tr>
                    <td colspan="12" style="color: Red">
                        <asp:Label Font-Size="Large" runat="server" ID="QueryMsg" Visible="false">Please input crrect queries.</asp:Label>
                    </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade size="1">
    </div>
    <table id="ExcTable" width="100%" border="0" cellpadding="0" cellspacing="0" runat="server">
        <tr>
            <td align="center">
                <font face="Arial" size="4">Summary Bundling Report</font>
            </td>
        </tr>
        <tr>
            <td>
                <div id="mmPrint" align="right">
                    <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv .style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv .style.display='block';document.all.mmPrint.style.visibility='visible'"
                        style="font-size: 16; width: 80; height: 26" />
                    <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,document.title+'.htm')"
                        style="font-size: 16; width: 80; height: 26" />
                    <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="font-size: 16;
                        width: 80; height: 26" />
                    <input type="button" name="excel" value="To Wps" onclick="ToExcelOfWPS()" style="font-size: 16;
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
                <table width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                    border-collapse: collapse">
                    <tr>
                        <td class="tr2style">
                            Order Qty
                        </td>
                        <td>
                            &nbsp;<%=ORDER_QTY %>
                        </td>
                        <td class="tr2style">
                            GO No.
                        </td>
                        <td>
                            &nbsp;<%=SC_NO %>
                        </td>
                        <td class="tr2style">
                            Buyer Name
                        </td>
                        <td>
                            &nbsp;<%=CUSTOMER_NAME %>
                        </td>
                    </tr>
                </table>
                <br />
                <table width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                    border-collapse: collapse">
                    <tr>
                        <td class="tr2style">
                            Colour/Size
                        </td>
                        <div id="divTitle" runat="server">
                        </div>
                        <td class="tr2style">
                            Total
                        </td>
                    </tr>
                    <div id="divResult" runat="server">
                    </div>
                    <tr>
                        <td class="tr2style">
                            Total Qty
                        </td>
                        <div id="divTotal" runat="server">
                        </div>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table id="NoData" runat="server" visible="false" width="100%">
        <tr>
            <td align="center" style="font-size: large; color: Red">
                No Data
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
