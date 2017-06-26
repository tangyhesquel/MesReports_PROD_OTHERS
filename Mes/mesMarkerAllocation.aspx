<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mesMarkerAllocation.aspx.cs"
    Inherits="Mes_mesMarkerAllocation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Marker Color Size breakdown</title>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script type="text/javascript" language="javascript">
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
    <style type="text/css">
        .style1
        {
            font-size: medium;
            font-weight: bold;
        }
        .style2
        {
            width: 1094px;
            text-align: center;
        }
    </style>
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
                        <input type="button" value="..." onclick="searchMarkerNo()" class="button_top" />
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
        <hr noshade="noshade" size="1" />
    </div>
    <div id="EtTable">
        <table id="ExcTable" width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="center">
                    <font face="Arial" size="4">Marker Allocation Detail Report</font>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="mmPrint" align="right">
                        <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv .style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv .style.display='block';document.all.mmPrint.style.visibility='visible'"
                            style="height: 26" class="button_top" />
                        <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,document.title+'.htm')"
                            style="height: 26" class="button_top" />
                        <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="height: 26"
                            class="button_top" />
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
                    <table width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                        border-collapse: collapse">
                        <tr>
                            <td class="tr2style">
                                Customer:
                            </td>
                            <td>
                                &nbsp;<%=SHORT_NAME%>
                            </td>
                            <td class="tr2style">
                                GO No:
                            </td>
                            <td>
                                &nbsp;<%=GO_NO %>
                            </td>
                            <td class="tr2style">
                                Part Type:
                            </td>
                            <td>
                                &nbsp;<%=PART_TYPE%>
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
                    <asp:GridView ID="gvBody" runat="server" Width="100%" AutoGenerateColumns="False"
                        OnRowDataBound="gvBody_RowDataBound" ShowFooter="True">
                        <Columns>
                            <asp:BoundField DataField="MARKER_ID" HeaderText="Marker Set ID" />
                            <asp:BoundField DataField="COLOR_CD" HeaderText="Color Code" />
                            <asp:BoundField DataField="COLOR_DESC" HeaderText="Color Desc" />
                            <asp:BoundField DataField="GMT_QTY" HeaderText="Gmt Qty" />
                            <asp:BoundField DataField="PLYS" HeaderText="Ply" />
                            <asp:BoundField DataField="BUYER_PO_DEL_DATE" HeaderText="DEL DATE" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td>
                    <table id="MergeSize" runat="server" visible="False">
                        <tr>
                            <td class="style2">
                                <font face="Arial" size="3" style="text-align: center">Merger Sizes Information</font>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div id="div2" runat="server">
                                    <asp:GridView ID="gvSize" runat="server" Width="100%" BorderColor="#EFEFE7" OnDataBound="gvSize_DataBound">
                                    </asp:GridView>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table id="MergeColor" runat="server" visible="False">
                        <tr>
                            <td class="style2">
                                <font face="Arial" size="3" style="text-align: center">Merger Colors Information</font>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div id="div1" runat="server">
                                    <asp:GridView ID="gvColor" runat="server" BorderColor="#EFEFE7" OnDataBound="gvColor_DataBound"
                                        OnRowDataBound="gvColor_RowDataBound">
                                        <SelectedRowStyle Wrap="False" />
                                    </asp:GridView>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
