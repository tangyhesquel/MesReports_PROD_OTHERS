<%@ Page Language="C#" AutoEventWireup="true" CodeFile="outsourceContractReport2.aspx.cs"
    Inherits="Reports_outsourceContractReport2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>外发加工单次合同</title>
    <style type="text/css">
        .bigfont2
        {
            font-weight: bold;
            font-size: 14px;
            font-family: 黑体,Arial,Times New Roman,Verdana,Helvetica, sans-serif;
        }
        UNKNOWN
        {
            padding-right: 5px;
            padding-left: 5px;
            scrollbar-face-color: #eeeee7;
            font-size: 11px;
            padding-bottom: 5px;
            scrollbar-highlight-color: #eeeeee;
            scrollbar-shadow-color: #000000;
            scrollbar-3dlight-color: #ffffff;
            scrollbar-arrow-color: #000000;
            padding-top: 5px;
            scrollbar-track-color: #efefef;
            font-family: Arial,Times New Roman, 黑体,Verdana,Helvetica, sans-serif;
            scrollbar-darkshadow-color: #ffffff;
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
            border-right: #333333 1px solid;
            border-top: #333333 1px solid;
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            border-left: #333333 1px solid;
            cursor: hand;
            border-bottom: #333333 1px solid;
            background-color: #efefef;
        }
        .BUTTON
        {
            border-right: #333333 1px solid;
            padding-right: 1px;
            border-top: #333333 1px solid;
            padding-left: 1px;
            font-weight: bolder;
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            border-left: #333333 1px solid;
            cursor: hand;
            color: #646400;
            padding-top: 1px;
            border-bottom: #333333 1px solid;
            height: 20px;
            background-color: #cccc99;
        }
        .BUTTON_down
        {
            border-right: #646400 1px solid;
            padding-right: 1px;
            border-top: #646400 1px solid;
            padding-left: 1px;
            font-weight: bolder;
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            border-left: #646400 1px solid;
            cursor: hand;
            color: #646400;
            padding-top: 1px;
            border-bottom: #646400 1px solid;
            height: 20px;
            background-color: #efefe7;
        }
        .input_gary
        {
            border-right: #333333 1px solid;
            border-top: #333333 1px solid;
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            border-left: #333333 1px solid;
            border-bottom: #333333 1px solid;
            background-color: #efefef;
        }
        .input_color
        {
            border-right: #333333 1px solid;
            border-top: #333333 1px solid;
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            border-left: #333333 1px solid;
            border-bottom: #333333 1px solid;
            background-color: #eef7ff;
        }
        .input_white
        {
            border-right: #333333 1px solid;
            border-top: #333333 1px solid;
            padding-left: 2px;
            font-size: 11px;
            border-left: #333333 1px solid;
            border-bottom: #333333 1px solid;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            height: 18px;
        }
        INPUT
        {
            padding-left: 2px;
            font-size: 11px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            height: 18px;
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
            padding-right: 5px;
            padding-left: 5px;
            font-size: 11px;
            padding-top: 2px;
            background-color: #ffffff;
        }
        .tr2style
        {
            padding-right: 5px;
            padding-left: 5px;
            font-weight: bolder;
            font-size: 11px;
            padding-top: 2px;
            background-color: #efefe7;
            border-right: .5pt solid windowtext;
            border-bottom: .5pt solid windowtext;
        }
        .tr3style
        {
            padding-right: 0px;
            padding-left: 0px;
            padding-top: 0px;
            padding-bottom: 0px;
            border-right: .5pt solid windowtext;
            border-bottom: .5pt solid windowtext;
        }
        .tr4style
        {
            padding-right: 0px;
            padding-left: 0px;
            padding-top: 0px;
            padding-bottom: 0px;
            border-right: .5pt solid windowtext;
        }
        .tr5style
        {
            padding-right: 0px;
            padding-left: 0px;
            padding-top: 0px;
            padding-bottom: 0px;
            border-bottom: .5pt solid windowtext;
        }
        .tr6style
        {
            padding-right: 5px;
            padding-left: 5px;
            font-weight: bolder;
            font-size: 11px;
            padding-top: 2px;
            background-color: #efefe7;
            border-bottom: .5pt solid windowtext;
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
        .bule
        {
            font-weight: 600;
            color: #006699;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        TD
        {
            word-break: break-all;
            word-wrap: break-word;
        }
    </style>
    <script language="javascript">
        function SearchContractNo() {
            var urlName = "../Reports/queryContract.aspx?site=" + document.getElementById("hfValue").value + "";
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
                                <asp:TextBox ID="txtContractNo" runat="server" />
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
        <table width="750" cellspacing="0" cellpadding="0">
            <tr>
                <td colspan="10">
                    <p align="center">
                        <span lang="AR-SA" style="font-size: 22pt; font-family: 宋体; color: black;"><b>外发加工单次合同</b></span></p>
                    <p align="right">
                        <b>合同编号:</b><%=txtContractNo.Text.Trim() %></p>
                    <br />
                    <b>定作方</b>:<%=companyName %>(以下简称甲方)<br />
                    <br />
                    <b>承揽方</b>:<%=subcontractorName %>
                    (以下简称乙方)<br />
                    <br />
                    甲乙双方经过协商,在平等互利的基础上,根据我国《合同法》及有关规定,就甲方委托乙方加工业务,特订立本合同，以便共同遵守。<br />
                    <br />
                    <b>第一条</b>&nbsp;&nbsp;加工成品名称、GO号、原材料送货单号、规格、质量、数量、交货时间、加工费：<br />
                    <br />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <table bordercolor="#999999" width="750" cellspacing="0" cellpadding="0" style="table-layout: fixed;
                        border: .5pt solid windowtext;">
                        <tr>
                            <td class="tr2style" rowspan="2" valign="bottom">
                                <p align="center">
                                    <b>名称</b>
                                </p>
                            </td>
                            <td class="tr2style" rowspan="2" valign="bottom">
                                <p align="center">
                                    <b>GO号</b>
                                </p>
                            </td>
                            <td class="tr2style" rowspan="2" valign="bottom" width="11%">
                                <p align="center">
                                    <b>原材料送货单号</b>
                                </p>
                            </td>
                            <td class="tr2style" rowspan="2" valign="bottom" width="5%">
                                <p align="center">
                                    <b>规格</b>
                                </p>
                            </td>
                            <td class="tr2style" valign="bottom" rowspan="2">
                                <p align="center">
                                    <b>质量<br />
                                        要求</b>
                                </p>
                            </td>
                            <td class="tr2style" rowspan="2" valign="bottom">
                                <p align="center">
                                    <b>数量(件)</b>
                                </p>
                            </td>
                            <td class="tr2style" rowspan="2" valign="bottom">
                                <p align="center">
                                    <b>交货时间</b>
                                </p>
                            </td>
                            <td class="tr2style" rowspan="2" valign="bottom">
                                <p align="center">
                                    <b>名称</b>
                                </p>
                            </td>
                            <td class="tr6style" colspan="2" valign="bottom">
                                <p align="center">
                                    <b>加工费</b>
                                </p>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr2style" valign="bottom">
                                <p align="center">
                                    <b>加工费单价(含<%=valueAddTax %>
                                        %税)</b>
                                    <%=currency %>
                                </p>
                            </td>
                            <td class="tr6style" valign="bottom">
                                <p align="center">
                                    <b>总价</b><br />
                                    <%=currency %>
                                </p>
                            </td>
                        </tr>
                        <div id="divBody" runat="server">
                        </div>
                        <tr>
                            <td colspan="2" valign="bottom" class="tr4style">
                                <p>
                                    <br />
                                    <b>合计</b>
                                </p>
                            </td>
                            <td colspan="8" valign="bottom">
                                <p>
                                    <br />
                                    <%=ttOutAmount %>
                                </p>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="10">
                    <br />
                    <b>第二条</b>&nbsp;&nbsp;<%=subString2 %>
                    <br />
                    <br />
                    <b>第三条</b>&nbsp;&nbsp;数量及加工费总价的特别约定：本合同第一条所规定的数量为甲方拟外发加工的数量，实际数量以双方授权代表签字并加盖公章的
                    <br />
                    <br />
                    《<%=companyName %>外发加工对数单》为准。 并根据本合同第一条所规定的单价及《<%=companyName %>外发加工对数单》所规定的实<br />
                    <br />
                    际数量来计算加工费总价。
                    <br />
                    <br />
                    <b>第四条</b>&nbsp;&nbsp;本合同自双方授权代表签字并加盖公章后生效。
                    <br />
                    <br />
                    <div id="divFour" runat="server">
                    </div>
                    <br />
                    <div id="divfive" runat="server">
                    </div>
                    <b>
                        <%=strLine1 %></b>&nbsp;&nbsp;本合同未涉及的事项，以双方签订的《外发加工总合同》（编号<%=subcontractNo%>）为准。<br />
                    <br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <u>甲方(<%=companyName %>)特别提醒乙方注意,甲方禁止乙方以任何形式给予甲方相关人员商业贿赂,包括不只限于公司回扣,礼品,旅游,<br />
                        <br />
                        等一经发现,并查证属实,则视为乙方违约,乙方将赔偿甲方合同总额百分之二十且不低于伍万的违约金,同时甲将提请司法机关追究乙方及甲方<br />
                        <br />
                        相关员的法律责任. </u>
                    <br />
                    <br />
                    <b>
                        <%=strLine2 %></b>&nbsp;&nbsp;<%=subString6 %>
                    <br />
                    <br />
                    <b>附件:</b>&nbsp;&nbsp;甲乙双方授权代表签字并加盖公章的《<%=companyName %>外发加工对数单》为本合同不可分割的一部份，与本合同有同等法律效力。<br />
                    <br />
                    且乙方在加工定作期间及交货后12个月内,不得与产品涉及的甲方客户有任何形式的业务往来。 否则，视为乙方违约，乙方应按总合同第十条<br />
                    <br />
                    第13款承担违约责任。&nbsp;
                    <br />
                    <br />
                    <br />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <table width="100%">
                        <tr>
                            <td width="50%">
                                <b>甲 方：</b><%=companyName %>
                            </td>
                            <td>
                                <b>乙 方：</b><%=subcontractorName %>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>授权代表： </b>
                            </td>
                            <td>
                                <b>授权代表： </b>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>日 期：</b><script>
                                               var newDate = new Date();
                                               var dateString = newDate.getYear() + "-" + ((newDate.getMonth() + 1) < 10 ? "0" : "") + (newDate.getMonth() + 1) + "-" + (newDate.getDate() < 10 ? "0" : "") + newDate.getDate();
                                               document.write(dateString);
                                </script>
                            </td>
                            <td>
                                <b>日 期：</b>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>合同履行责任人:</b><%=Receiver %>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>地 址：</b><%=companyAddress %>
                            </td>
                            <td>
                                <b>地 址：</b><%=subcontractAddress %>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>电 话：</b><%=companyTel %>
                            </td>
                            <td>
                                <b>电 话：</b><%=subTel %>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>传 真：</b><%=companyFax %>
                            </td>
                            <td>
                                <b>传 真：</b><%=subFax %>
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
