<%@ Page Language="C#" AutoEventWireup="true" CodeFile="outsourceCheckingReport.aspx.cs"
    Inherits="Mes_outsourceCheckingReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>外发加工对数单</title>
    <object id="WebBrowser1" width="0" height="0" classid="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2">
    </object>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script language="javascript">
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
    <div id="ExcTable">
        <table id="toPrint" width="100%">
            <tr>
                <td>
                    Contract No:&nbsp;&nbsp;
                    <asp:TextBox ID="txtContractNo" runat="server"></asp:TextBox>
                    <input type="button" value="..." onclick="SearchContractNo()">
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnQuery_Click" />
                </td>
                <td align="right">
                    <input name="btnPrint" type="button" value="   Print   " onclick="toPrint.style.display='none';WebBrowser1.ExecWB(6,1);toPrint.style.display=''" />
                    <input name="btnPreview" type="button" value="Preview" onclick="toPrint.style.display='none';WebBrowser1.ExecWB(7,1);toPrint.style.display=''" />
                    <input name="btnPageSetup" type="button" value="Page Setup" onclick="toPrint.style.display='none';WebBrowser1.ExecWB(8,1);toPrint.style.display=''" />
                    <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="height: 26"
                        class="button_top">
                    <input type="button" name="excel" value="To Wps" onclick="toPaseWPS()" style="height: 26"
                        class="button_top" />
                </td>
            </tr>
        </table>
        <table width="100%">
            <tr>
                <td colspan="14" align="center">
                    <h1>
                        <%=factoryName %>
                    </h1>
                    <h3>
                        外发加工对数单
                    </h3>
                </td>
            </tr>
            <tr>
                <td>
                    外发加工商:
                </td>
                <td align="left" colspan="10">
                    <%=subcontractorName %>
                </td>
                <td>
                    合同编号:
                </td>
                <td align="left" colspan="3">
                    <%=txtContractNo.Text %>
                </td>
            </tr>
            <tr>
                <td colspan="14">
                    <table width="100%" style="border-collapse: collapse;" border="1" bordercolor="#AAAAAA">
                        <tr>
                            <td>
                                制单号
                            </td>
                            <td>
                                工序
                            </td>
                            <td>
                                发出数量
                            </td>
                            <td>
                                收回正品
                            </td>
                            <td>
                                收回次品
                            </td>
                            <td>
                                外厂遗失
                            </td>
                            <td>
                                本厂疵品
                            </td>
                            <td>
                                含税单价
                            </td>
                            <td>
                                正品加工费
                            </td>
                            <td>
                                应扣布价
                            </td>
                            <td>
                                其它调整
                            </td>
                            <td>
                                实付加工费
                            </td>
                            <td>
                                调整原因
                            </td>
                        </tr>
                        <div id="divDetail" runat="server">
                        </div>
                    </table>
                </td>
            </tr>
        </table>
        <table width="100%">
            <tr>
                <td rowspan="5" width="50%">
                    说明:<br>
                    1. [发出数量]= [收回正品] + [收回次品] + [外厂遗失] + [本厂疵品]<br>
                    2.[实付加工费] = 正品加工费-应扣布价*外厂遗失+其他调整<br>
                    3.[次品加工费] = [收回次品] * 双方确认次品加工单价<br>
                    4.[应扣布价] = [外厂遗失] * 双方确认布料单价<br>
                    5.[其他调整]包括退赔次品布价、再加工工价及其他索赔,请参见调整原因<br />
                    6.[收回正品]包括PULLOUT中一些正常数据(SAMPLE,抽办......)<br>
                </td>
                <td width="10%">
                    经手人:
                    <td width="15%">
                        <%=createUserName %>
                    </td>
                    <td width="10%">
                        加工商签章确认:
                    </td>
                    <td width="15%">
                        &nbsp;
                    </td>
            </tr>
            <tr>
                <td width="10%">
                    审批人:
                </td>
                <td colspan="3">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td width="10%">
                    打印日期:
                </td>
                <td colspan="3">
                    <%=DateTime.Now.ToShortDateString() + "&nbsp;"+ DateTime.Now.ToLongTimeString()%>
                </td>
            </tr>
            <tr>
                <td width="10%">
                    总经理/厂长:
                </td>
                <td colspan="3">
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td style="border-bottom: .5pt solid windowtext;">
                    &nbsp;
                </td>
                <td>
                    &nbsp;
                </td>
                <td style="border-bottom: .5pt solid windowtext;">
                    &nbsp;
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
