<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mesSpreadingSummary.aspx.cs"
    Inherits="Mes_mesSpreadingSummary" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Spreading Planning Summary Report</title>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script language="javascript">
        function init() {

            if (document.getElementById("txtMoNo").value == "" && document.getElementById("txtJoNo").value == "" && document.getElementById("txtfromCutLotNo").value == "" && document.getElementById("txtfromCutLotNo").value == "") {
                document.all.ExcTable.style.display = 'none';
            }
        }
        function searchMarkerNo() {
            var urlName = "mesSearchMarkerNo.aspx";
            var markerNo = window.showModalDialog(urlName, "Marker No.", "dialogWidth=450pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (markerNo == null) return;
            document.all.txtMoNo.value = markerNo;
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
                    <td class="tdbackcolor" width="100">
                        Marker Order No:
                    </td>
                    <td>
                        <asp:TextBox ID="txtMoNo" runat="server"></asp:TextBox>
                        <input type="button" value="..." onclick="searchMarkerNo()" class="button_top">
                    </td>
                    <td class="tdbackcolor" width="100">
                        JO No:
                    </td>
                    <td>
                        <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                    </td>
                    <td class="tdbackcolor" width="100">
                        From Cut Lot No:
                    </td>
                    <td>
                        <asp:TextBox ID="txtfromCutLotNo" runat="server"></asp:TextBox>
                    </td>
                    <td class="tdbackcolor" width="100">
                        To Cut Lot No:
                    </td>
                    <td>
                        <asp:TextBox ID="txttoCutLotNo" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                    </td>
                    <td>
                        <asp:CheckBox ID="cbNewReport" runat="server" Text="ShowNewReport" />
                    </td>
                    <td align="RIGHT">
                        <asp:Button ID="btnQuery" Text="Query" runat="server" CssClass="button_top" OnClick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade size="1">
    </div>
    <div id="EtTable">
        <table id="ExcTable" width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="center">
                    <font face="Arial" size="4">Spreading Planning Summary Report</font>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="mmPrint" align="right">
                        <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv .style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv .style.display='block';document.all.mmPrint.style.visibility='visible'"
                            style="height: 26" class="button_top">
                        <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,document.title+'.htm')"
                            style="height: 26" class="button_top">
                        <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="height: 26"
                            class="button_top">
                        <input type="button" name="excel" value="To Wps" onclick="ToExcelOfWPS()" style="height: 26"
                            class="button_top" />
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
                                MO No
                            </td>
                            <td class="tr2style">
                                Customer
                            </td>
                            <td class="tr2style">
                                Job Order No
                            </td>
                            <td class="tr2style">
                                Color Code
                            </td>
                            <td class="tr2style">
                                Styling
                            </td>
                            <td class="tr2style">
                                Cut Lot No
                            </td>
                            <td class="tr2style">
                                Marker Length
                            </td>
                            <td class="tr2style">
                                Qty Lay
                            </td>
                            <td class="tr2style">
                                Qty Roll
                            </td>
                            <td class="tr2style">
                                Plys
                            </td>
                            <td class="tr2style">
                                Size Num
                            </td>
                            <td class="tr2style">
                                Color L/D
                            </td>
                            <td class="tr2style">
                                Gmt Qty
                            </td>
                            <td class="tr2style">
                                Marker ID
                            </td>
                            <td class="tr2style">
                                Fab Req
                            </td>
                            <td class="tr2style">
                                Fab Req + Wastage
                            </td>
                            <td class="tr2style">
                                Fabric Width
                            </td>
                            <td class="tr2style">
                                Fabric Desc
                            </td>
                            <%if (factoryCD != "EGM" && factoryCD != "DEV")
                              {
                            %>
                            <td class="tr2style">
                                Batch No#
                            </td>
                            <td class="tr2style">
                                Size Ratio#
                            </td>
                            <% } %>>
                        </tr>
                        <div id="divBody" runat="server">
                        </div>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
