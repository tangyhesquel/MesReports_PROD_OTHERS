<%@ Page Language="C#" AutoEventWireup="true" CodeFile="outsourceReport.aspx.cs"
    Inherits="Mes_outsourceReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>外发加工货物移交清单</title>
    <object id="WebBrowser1" width="0" height="0" classid="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2">
    </object>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script>

        function SearchContractNo() {
            var url = "./.do";
            var urlName = "../Reports/queryContract.aspx?site=" + document.getElementById("hfValue").value + "";
            var contractNo = window.showModalDialog(urlName, "Contract No.", "dialogWidth=1030px;dialogHeight=768px;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (contractNo == null) return;
            document.all.txtContractNo.value = contractNo;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="hfValue" runat="server" />
    <div>
        <table id="Header" width="100%">
            <tr>
                <td colspan="4" align="right">
                    <input name="btnPrint" type="button" value="   Print   " onclick="Header.style.display='none';WebBrowser1.ExecWB(6,1);Header.style.display=''" />
                    <input name="btnPreview" type="button" value="Preview" onclick="Header.style.display='none';WebBrowser1.ExecWB(7,1);Header.style.display=''" />
                    <input name="btnPageSetup" type="button" value="Page Setup" onclick="Header.style.display='none';WebBrowser1.ExecWB(8,1);Header.style.display=''" />
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%">
                        <tr>
                            <td align="right" width="100">
                                Contract No:
                            </td>
                            <td width="200">
                                <asp:TextBox ID="txtContractNo" runat="server"></asp:TextBox>
                                <input type="button" value="..." id="btnSearchContractNo" onclick="SearchContractNo()" />
                            </td>
                            <td align="right">
                                <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnQuery_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div id="Print">
        <table width="100%" style="border-top: .5pt solid windowtext; border-bottom: .5pt solid windowtext;
            border-left: .5pt solid windowtext; border-right: .5pt solid windowtext;">
            <tr>
                <td style="border-right: .5pt solid windowtext;">
                    <table width="100%">
                        <tr>
                            <td align="center" colspan="6">
                                <h1>
                                    外发加工货物移交清单
                                </h1>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td align="right">
                                合同编号:
                            </td>
                            <td>
                                <%=txtContractNo.Text %>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                出门凭证联
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td align="right">
                                发出日期:
                            </td>
                            <td>
                                <%=issueDate %>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" align="left">
                                兹有
                                <%=factoryName %>
                                1部,委托
                                <br />
                                <%=subcontractorName %>
                                加工以下货物,请准以放行.
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6">
                                <table width="100%" style="border-collapse: collapse;" border="1">
                                    <tr>
                                        <td>
                                            发出品名
                                        </td>
                                        <td>
                                            制单号
                                        </td>
                                        <td>
                                            发出数量
                                        </td>
                                        <td>
                                            计量单位
                                        </td>
                                    </tr>
                                    <div id="divDetail1" runat="server">
                                    </div>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                经手人:
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                                审批人:
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                签名:
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                                签名:
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table width="100%">
                        <tr>
                            <td align="center" colspan="6">
                                <h1>
                                    外发加工货物移交清单
                                </h1>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td align="right">
                                合同编号:
                            </td>
                            <td>
                                <%=txtContractNo.Text%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                加工商接收确认联
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td align="right">
                                发出日期:
                            </td>
                            <td>
                                <%=issueDate %>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" align="left">
                                兹有
                                <%=factoryName %>
                                1部,委托
                                <br />
                                <%=subcontractorName %>
                                加工以下货物,请贵厂确认签收为感。
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6">
                                <table width="100%" style="border-collapse: collapse;" border="1">
                                    <tr>
                                        <td>
                                            发出品名
                                        </td>
                                        <td>
                                            制单号
                                        </td>
                                        <td>
                                            发出数量
                                        </td>
                                        <td>
                                            计量单位
                                        </td>
                                    </tr>
                                    <div id="divDetail2" runat="server">
                                    </div>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                经手人:
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                                审批人:
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                签名:
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                                签名:
                            </td>
                            <td>
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
