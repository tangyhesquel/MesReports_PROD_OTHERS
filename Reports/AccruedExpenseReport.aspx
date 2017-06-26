<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AccruedExpenseReport.aspx.cs"
    Inherits="Reports_AccruedExpenseReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>预提报表</title>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <style type="text/css">
        .tr2style
        {
            font-weight: bolder;
            font-size: 11px;
            background-color: #efefe7;
            padding-left: 5px;
            padding-right: 5px;
            padding-top: 2px;
            width: auto;
            word-break: keep-all;
        }
    </style>
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function SearchContractNo() {
            var urlName = "queryContract.aspx?site=" + document.getElementById("hfValue").value + "";
            var contractNo = window.showModalDialog(urlName, "Contract No.", "dialogWidth=900px;dialogHeight=500px;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (contractNo == null) return;
            document.all.txtContractNo.value = contractNo;
        }
    </script>
    <object id="WebBrowser1" width="0" height="0" classid="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2">
    </object>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="hfValue" runat="server" />
    <div>
        <table id="toPrint" width="100%">
            <tr>
                <td align="right" colspan="8">
                    <input name="btnPrint" type="button" value="   Print   " onclick="toPrint.style.display='none';WebBrowser1.ExecWB(6,1);toPrint.style.display=''" />
                    <input name="btnPreview" type="button" value="Preview" onclick="toPrint.style.display='none';WebBrowser1.ExecWB(7,1);toPrint.style.display=''" />
                    <input name="btnPageSetup" type="button" value="Page Setup" onclick="toPrint.style.display='none';WebBrowser1.ExecWB(8,1);toPrint.style.display=''" />
                    <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="height: 26"
                        class="button_top"/>
                    <input type="button" name="excel" value="To Wps" onclick="ToExcelOfWPS()" style="height: 26"
                        class="button_top" />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td width="65">
                    Contract No:
                </td>
                <td width="170">
                    <asp:TextBox ID="txtContractNo" runat="server"></asp:TextBox>
                    <input type="button" value="..." onclick="SearchContractNo()"/>
                </td>
                <td width="75">
                    Job Order No.:
                </td>
                <td width="170">
                    <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                </td>
                <td width="90">
                    From Issue Date:
                </td>
                <td width="170">
                    <asp:TextBox ID="txtFromDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                </td>
                <td width="90">
                    To Issue Date:
                </td>
                <td width="170">
                    <asp:TextBox ID="txtToDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                </td>
                <td align="left">
                    <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="BUTTON" OnClick="btnQuery_Click" />
                </td>
            </tr>
        </table>
        <table id="ExcTable" style="border-collapse: collapse; table-layout: auto" bordercolor="#999999"
            cellspacing="0" cellpadding="0" border="1">
            <tr style="white-space: nowrap;">
                <td rowspan="2" class="tr2style">
                    期间
                </td>
                <td rowspan="2" class="tr2style">
                    合同编号
                </td>
                <td rowspan="2" class="tr2style">
                    外发工序
                </td>
                <td rowspan="2" class="tr2style">
                    CT NO
                </td>
                <td rowspan="2" class="tr2style">
                    发出数量
                </td>
                <td align="center" rowspan="2" class="tr2style">
                    收回正品数量<br />
                    不含零价仓数(PCS)
                </td>
                <td rowspan="2" class="tr2style">
                    过零价仓(正品)
                </td>
                <td align="center" colspan="8" class="tr2style">
                    下数(PCS)
                </td>
                <td rowspan="2" class="tr2style">
                    下数总计
                </td>
                <td rowspan="2" class="tr2style">
                    过零价仓(次品)PCS
                </td>
                <td align="center" colspan="4" class="tr2style">
                    应计提加工费<br />
                    (不含税)
                </td>
                <td rowspan="2" class="tr2style">
                    调整款项<br />
                    (RMB)
                </td>
                <td rowspan="2" class="tr2style">
                    布价
                </td>
                <td rowspan="2" class="tr2style">
                    应扣布价<br />
                    (不含税)
                </td>
                <td rowspan="2" class="tr2style">
                    实付加工费<br />
                    (不含税)
                </td>
                <td align="center" colspan="4" class="tr2style">
                    外发单价<br />
                    (Per PCS含税)
                </td>
                <td align="center" colspan="4" class="tr2style">
                    本厂工价(Per PCS)
                </td>
                <td rowspan="2" class="tr2style">
                    税率
                </td>
                <td rowspan="2" class="tr2style">
                    加工商
                </td>
                <td rowspan="2" class="tr2style">
                    工序描述
                </td>
                <td rowspan="2" class="tr2style">
                    外发原因
                </td>
                <td rowspan="2" class="tr2style">
                    Remark
                </td>
            </tr>
            <tr>
                <td class="tr2style">
                    次品
                </td>
                <td class="tr2style">
                    抽办
                </td>
                <td class="tr2style">
                    本厂不见衫
                </td>
                <td class="tr2style">
                    商检
                </td>
                <td class="tr2style">
                    厂存
                </td>
                <td class="tr2style">
                    外厂不见衫
                </td>
                <td class="tr2style">
                    其他(-)
                </td>
                <td class="tr2style">
                    其他(+)
                </td>
                <%--                <td class="tr2style">
                    外厂不见衫

                </td>--%>
                <td class="tr2style">
                    主工序
                </td>
                <td class="tr2style">
                    洗水
                </td>
                <td class="tr2style">
                    印花
                </td>
                <td class="tr2style">
                    绣花
                </td>
                <td class="tr2style">
                    主工序
                </td>
                <td class="tr2style">
                    洗水
                </td>
                <td class="tr2style">
                    印花
                </td>
                <td class="tr2style">
                    绣花
                </td>
                <td class="tr2style">
                    主工序
                </td>
                <td class="tr2style">
                    洗水
                </td>
                <td class="tr2style">
                    印花
                </td>
                <td class="tr2style">
                    绣花
                </td>
            </tr>
            <div id="divBody" runat="server">
            </div>
        </table>
        <br />
        备注:
        <br />
        1、次品 = CPNDF疵裁片+FABDF布疵(包括绣错花,绣烂裁片,粘错朴) +PRTDF印花疵品(包括花疵,烘黄) + SEWDF车缝疵品(包括破洞,污渍)
        +SHADE色差(包括其它疵裁片)
        <br />
        2、抽办 = SMPL SAMPLE+SMPBOAT大货抽走船的办衫+SSMPL内部测试办
        <br />
        3、本厂不见衫=GMMTL不见衫
        <br />
        4、商检 = CHECK商检
        <br />
        5、厂存=FTYST厂存
        <br />
        6、外厂不见衫=OUMTL外厂不见衫
        <br />
        7、应扣布价(不含税)=外厂不见衫*布价/(1+税率)
        <br />
        8、下数总数 = 次品+抽办+本厂不见衫+商检+厂存+ 其他（+）+其他（-）+外厂不见衫
        <br />
        9、应计加工费 = [(收回正品数量不含零价仓数(PCS)+抽办+本厂不见衫+商检+厂存+其它(+)-其它(-))x单价 /(1+税率)
        <br />
        10、实付加工费（不含税）=应计提加工费中主工序加工费+洗水加工费+印花加工费+绣花加工费-应扣布价(不含税)+调整款项/(1+税率)
        <br />
        11、Remark主要输一些其它扣款，如运费。
    </div>
    </form>
</body>
</html>
