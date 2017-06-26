<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mesMarkerDetail.aspx.cs"
    Inherits="Mes_mesMarkerDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Marker Detail Report</title>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script language="javascript">
        function init() {
            if (document.getElementById("txtMoNo").value == "") {
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
            <legend>Search</legend>
            <table width="100%" id="queryTab">
                <tr>
                    <td class="tdbackcolor" width="100">
                        Marker Order No:
                    </td>
                    <td>
                        <asp:TextBox ID="txtMoNo" runat="server"></asp:TextBox>
                        <input type="button" value="..." onclick="searchMarkerNo()" class="button_top">
                    </td>
                    <td align="RIGHT">
                        <asp:Button ID="btnQuery" Text="Query" runat="server" CssClass="button_top" OnClick="btnQuery_Click" />
                    </td>
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
                <font face="Arial" size="4">Marker Detail Report</font>
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
                Marker Order No:&nbsp;&nbsp;&nbsp;<%=txtMoNo.Text.Trim() %>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <div>
                    <table width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                        border-collapse: collapse">
                        <tr>
                            <td class="tr2style">
                                Customer:
                            </td>
                            <td>
                                &nbsp;<%=SHORT_NAME %>
                            </td>
                            <td class="tr2style">
                                GO No:
                            </td>
                            <td>
                                &nbsp;<%=GO_NO %>
                            </td>
                            <td class="tr2style">
                                Cut Sew Wastage:
                            </td>
                            <td>
                                &nbsp;<%=MARKER_WASTAGE %>
                            </td>
                            <td class="tr2style">
                                Part Type:
                            </td>
                            <td>
                                &nbsp;<%=PART_TYPE %>
                            </td>
                            <td class="tr2style">
                                AVG MU%
                            </td>
                            <td>
                                &nbsp;<%=AVG_MU %>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr2style">
                                PPO YPD:
                            </td>
                            <td>
                                &nbsp;<!-- nested:write  property="SHORT_NAME" / -->
                            </td>
                            <td class="tr2style">
                                Bulk Net Marker YPD:
                            </td>
                            <td>
                                &nbsp;<%=netYpd %>
                                <td class="tr2style">
                                    Bulk Marker YPD :
                                </td>
                                <td>
                                    &nbsp;<%=ypd %>
                                </td>
                                <td class="tr2style">
                                    PPO Marker YPD:
                                </td>
                                <td>
                                    &nbsp;<!--nested:write  property="GO_NO" / -->
                                </td>
                                <td class="tr2style">
                                    YPD Variance:
                                </td>
                                <td>
                                    &nbsp;<!--nested:write  property="GO_NO" / -->
                                </td>
                        </tr>
                        <tr>
                            <td class="tr2style" width="10%">
                                Remarks:
                            </td>
                            <td colspan="9">
                                &nbsp;<%=REMARKS %>
                            </td>
                        </tr>
                    </table>
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
                <asp:GridView ID="gvBody" runat="server" Width="100%" OnRowDataBound="gvBody_RowDataBound"
                    AutoGenerateColumns="False" ShowFooter="True">
                    <Columns>
                        <asp:BoundField DataField="MARKER_ID" />
                        <asp:BoundField DataField="MARKER_NAME" />
                        <asp:BoundField DataField="COLOR_CD" />
                        <asp:BoundField DataField="COLOR_DESC" />
                        <asp:BoundField DataField="PATTERN_TYPE_DESC" />
                        <asp:BoundField DataField="FAB_DESC" />
                        <asp:BoundField DataField="GMT_QTY" />
                        <asp:BoundField DataField="PLYS" />
                        <asp:BoundField DataField="RATION_TOTAL" />
                        <asp:BoundField DataField="MARKER_LENGTH" />
                        <asp:BoundField DataField="MARKER_LEN_YDS" />
                        <asp:BoundField DataField="MARKER_LEN_INCH" />
                        <asp:BoundField DataField="CUT_SEW_WASTAGE" />
                        <asp:BoundField DataField="FAB_REQ" />
                        <asp:BoundField DataField="YPD" />
                        <asp:BoundField DataField="SHRINKAGE" />
                        <asp:BoundField DataField="BATCH_NO" />
                        <asp:BoundField DataField="WIDTH" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
