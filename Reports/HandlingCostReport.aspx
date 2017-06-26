<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HandlingCostReport.aspx.cs"
    Inherits="Reports_HandlingCostReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Quoted Price Requisition Report</title>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script language="javascript">
        function SearchContractNo() {
            var urlName = "queryContract.aspx?site=" + document.getElementById("hfValue").value + "";
            var contractNo = window.showModalDialog(urlName, "Contract No.", "dialogWidth=900px;dialogHeight=500px;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (contractNo == null) return;
            document.all.txtcontractNo.value = contractNo;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="hfValue" runat="server" />
    <div>
        <table>
            <tr>
                <td>
                    Contract No.:
                    <asp:TextBox ID="txtcontractNo" runat="server"></asp:TextBox>
                    <input type="button" value="..." onclick="SearchContractNo()">
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    GO #.:
                    <asp:TextBox ID="txtGo" runat="server"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnQuery_Click" />
                    <div id="toPrint" align="right">
                        <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" />
                        <input type="button" name="excel" value="To Wps" onclick="ToExcelOfWPS()" />
                    </div>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <div id="ExcTable" align="left">
                        <table>
                            <tr>
                                <td align="left">
                                    <table cellspacing="0" cellpadding="0" width="100%" class="dataTableStyle" align='center'
                                        bordercolor="#333333">
                                        <tr>
                                            <td rowspan="2" align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                Customer
                                            </td>
                                            <td rowspan="2" align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                GO#
                                            </td>
                                            <td rowspan="2" align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                款号
                                            </td>
                                            <td rowspan="2" align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                Qty(pcs)
                                            </td>
                                            <td align="center" rowspan="2" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                款式
                                            </td>
                                            <td rowspan="2" align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                图片
                                            </td>
                                            <td rowspan="2" align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                Outsourcing mill
                                            </td>
                                            <td rowspan="2" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                Process
                                            </td>
                                            <td align="center" colspan="4" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                本厂工价
                                                <br>
                                                (RMB/pc)
                                                <br>
                                                (含17%税)
                                            </td>
                                            <td align="center" colspan="4" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                Outsourcing price
                                                <br>
                                                (RMB/pc)
                                                <br>
                                                (含17%税)
                                            </td>
                                            <td rowspan="2" align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                SAH
                                            </td>
                                            <td rowspan="2" align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                报价标准
                                            </td>
                                            <td rowspan="2" align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                Customer's CM (RMB)
                                            </td>
                                            <td rowspan="2" align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                P/L (RMB)
                                            </td>
                                            <td align="center" rowspan="2" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                RM
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                裁+车缝+烫包
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                绣花
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                洗水
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                TTL
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                裁+车缝+烫包
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                绣花
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                洗水
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                TTL
                                            </td>
                                        </tr>
                                        <div id="divFirst" runat="server">
                                        </div>
                                    </table>
                                    <br>
                                    <br>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <table cellspacing="0" cellpadding="0" class="dataTableStyle" bordercolor="#333333">
                                        <tr>
                                            <td align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                客户
                                            </td>
                                            <td align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                GO#
                                            </td>
                                            <td align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                Qty/pc
                                            </td>
                                            <td align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                绣花号
                                            </td>
                                            <td align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                图片
                                            </td>
                                            <td align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                外发工序
                                            </td>
                                            <td align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                针数
                                            </td>
                                            <td align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                绣花房参考价（0.16RMB/1000针）<br />
                                                17%税
                                            </td>
                                            <td align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                <%=contractName %>>工价
                                                <br>
                                                (RMB/pc)
                                                <br>
                                                (含17%税)
                                            </td>
                                            <td align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                Sales 报价(RMB/pc)
                                            </td>
                                            <td align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                外发价
                                                <br>
                                                (含17%税)
                                                <br>
                                                RMB/pc
                                            </td>
                                            <td align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                外发价/SALES报价
                                            </td>
                                            <td align="center" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                倍数
                                                <br>
                                                (外发价/0.16RMB/1000针)
                                            </td>
                                        </tr>
                                        <div id="divSecond" runat="server">
                                        </div>
                                    </table>
                                    <br>
                                    <br>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <table cellspacing="0" cellpadding="0" class="dataTableStyle" bordercolor="#333333">
                                        <tr>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                单号
                                            </td>
                                            <td colspan="2" style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                FOB
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                物料价
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                布价
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                洗水价
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                绣花价
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                <%=contractName %>> 生产成本&amp;外发组成本
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                Customer'CMC
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                &nbsp;
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                RMB
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                (RMB/pc)
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                (RMB/pc)
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                (RMB/pc)
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                (RMB/pc)
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                (RMB/pc)
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                (RMB/pc)
                                            </td>
                                            <td style="border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;">
                                                (RMB/pc)
                                            </td>
                                        </tr>
                                        <div id="divThird" runat="server">
                                        </div>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
