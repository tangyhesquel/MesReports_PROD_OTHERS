<%@ Page Language="C#" AutoEventWireup="true" CodeFile="outsourceControlReport.aspx.cs"
    Inherits="Mes_outsourceControlReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Subcontracting Control Form</title>
    <object id="WebBrowser1" width="0" height="0" classid="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2">
    </object>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script type="text/javascript" language="javascript">
        function SearchContractNo() {
            var urlName = "../Reports/queryContract.aspx?site=" + document.getElementById("hfValue").value + "";
            var contractNo = window.showModalDialog(urlName, "Contract No.", "dialogWidth=1030px;dialogHeight=768px;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (contractNo == null) return;
            document.all.txtContractNo.value = trim(contractNo);
        }
        function trim(str) {   //删除左右两端的空格

            return str.replace(/(^\s*)|(\s*$)/g, "");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="hfValue" runat="server" />
    <div id="toPrint">
        <table width="100%">
            <tr>
                <td align="right" colspan="3">
                    <input name="btnPrint" type="button" value="   Print   " onclick="toPrint.style.display='none';WebBrowser1.ExecWB(6,1);toPrint.style.display=''" />
                    <input name="btnPreview" type="button" value="Preview" onclick="toPrint.style.display='none';WebBrowser1.ExecWB(7,1);toPrint.style.display=''" />
                    <input name="btnPageSetup" type="button" value="Page Setup" onclick="toPrint.style.display='none';WebBrowser1.ExecWB(8,1);toPrint.style.display=''" />
                    <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="height: 26"
                        class="button_top"/>
                    <input type="button" name="excel" value="To Wps" onclick="ToExcelOfWPS()" style="height: 26"
                        class="button_top" />
                </td>
            </tr>
            <tr>
                <td width="10%">
                    Contract No:
                </td>
                <td>
                    <asp:TextBox ID="txtContractNo" runat="server"></asp:TextBox>
                    <input type="button" onclick="SearchContractNo()" value="..." class="button_top" />
                </td>
                <td>
                    <div align="right">
                        <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnQuery_Click" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <table width="100%" id="ExcTable">
        <tr>
            <td align="center" colspan="10">
                <h3>
                    Subcontracting Control Form - Out
                </h3>
            </td>
        </tr>
        <tr>
            <td colspan="8">
            </td>
            <td align="right">
                Sequence No:
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td colspan="8">
            </td>
            <td align="right">
                Contract No:
            </td>
            <td>
                <%=txtContractNo.Text %>
            </td>
        </tr>
        <tr>
            <td width="15%">
                Subcontractor:
            </td>
            <td colspan="3">
                <%=name %>
            </td>
            <td align="right">
                Location:
            </td>
            <td colspan="3">
                <%=address %>
            </td>
            <td align="right">
                Tax Rate:
            </td>
            <td>
                <%=rate %>
            </td>
        </tr>
        <tr>
            <td colspan="10">
                <table width="100%" style="border-collapse: collapse;" border="1">
                    <tr>
                        <td>
                            Order/Job No
                        </td>
                        <td>
                            Subcontracing
                            <br />
                            Processing
                        </td>
                        <td>
                            Reasons for
                            <br />
                            Subcontracting
                        </td>
                        <td>
                            Internal
                            <br />
                            Process Cost
                        </td>
                        <td>
                            Qty Planed to
                            <br />
                            Subcontracting(PCS)
                        </td>
                        <td>
                            Qty Delivery to
                            <br />
                            Subcontracting(PCS)
                        </td>
                        <td>
                            Subcontracting<br />
                            Price (With Tax)
                        </td>
                        <td>
                            Goods Exit
                            <br />
                            Factory Date
                        </td>
                        <td>
                            Expected
                            <br />
                            Goods Receive Date
                        </td>
                    </tr>
                    <div id="divIn" runat="server">
                    </div>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="10">
                <table width="100%">
                    <tr>
                        <td width="21%">
                            Qty/Quanlity Received Checked by:
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            Approved By:
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            总经理/厂长:
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Qty Delivery Checked By:
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="10">
                <table width="100%">
                    <tr>
                        <td align="center" colspan="10">
                            <h3>
                                Subcontracting Control Form - In
                            </h3>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8">
                        </td>
                        <td align="right">
                            Sequence No:
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8">
                        </td>
                        <td align="right">
                            Contract No:
                        </td>
                        <td>
                            <%=txtContractNo.Text %>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Subcontractor:
                        </td>
                        <td colspan="3">
                            <%=name %>
                        </td>
                        <td align="right">
                            Location:
                        </td>
                        <td colspan="3">
                            <%=address %>
                        </td>
                        <td align="right">
                            Tax Rate:
                        </td>
                        <td>
                            <%=rate %>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="10">
                            <asp:CheckBox ID="cbSales" runat="server" Text="Selected by Sales" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="10">
                            <table width="100%" style="border-collapse: collapse;" border="1">
                                <tr>
                                    <td>
                                        Order/Job No
                                    </td>
                                    <td>
                                        Actual Goods<br />
                                        Received Date
                                    </td>
                                    <td>
                                        Total Qty
                                        <br />
                                        Received(PCS)
                                    </td>
                                    <td>
                                        Qty Received
                                        <br />
                                        with Defection(PCS)
                                    </td>
                                    <td>
                                        Qty Missing(PCS)
                                    </td>
                                    <td>
                                        Qty Sent with
                                        <br />
                                        Defection(PCS)
                                    </td>
                                    <td>
                                        Subcontracting
                                        <br />
                                        Price (With Tax)
                                    </td>
                                    <td>
                                        Payment
                                        <br />
                                        Deductions
                                    </td>
                                    <td>
                                        Actual
                                        <br />
                                        Payment(Including Rax)<br />
                                        (RMB)
                                    </td>
                                    <td>
                                        Remark
                                    </td>
                                </tr>
                                <div id="divOut" runat="server">
                                </div>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="10">
                            <table width="100%">
                                <tr>
                                    <td width="20%">
                                        Qty/Quanlity Received Checked by:
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        Approved By:
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        总经理/厂长:
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Qty Delivery Checked By:
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
