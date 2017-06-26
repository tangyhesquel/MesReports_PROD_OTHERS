<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReceiveCheckView.aspx.cs" Inherits="Reports_ReceiveCheckView" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
        <title>外发加工对数单</title>
    <object id="WebBrowser1" width="0" height="0" classid="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2">
    </object>
    <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Script/Common.js"></script>
        <script language="javascript">
            function SearchContractNo() {
                var urlName = "../Reports/queryContract.aspx?site=GEG";
                var contractNo = window.showModalDialog(urlName, "Contract No.", "dialogWidth=1030px;dialogHeight=768px;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
                if (contractNo == null) return;
                document.all.txtContract.value = trim(contractNo);
            }
            function trim(str) {   //删除左右两端的空格


                return str.replace(/(^\s*)|(\s*$)/g, "");
            }
            function searchJo() {
                //var urlName = "searchJONewCut.aspx?factory=" + document.all.ddlFactory.value + "&userRandom=" + (Math.random() * 100000);
                var urlName = "ReceiveCheckSupplier.aspx" + window.location.search + "&userRandom=" + (Math.random() * 100000);
                //var jo = window.showModalDialog(urlName, "Job Order No.", "dialogWidth=650pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
                var jo = window.showModalDialog(urlName, "strJoList.", "dialogWidth=650pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
                if (jo == null) return;
                document.all.txtsuppid.value = jo;
            }
    </script>
    <style type="text/css">
        .auto-style1 {
            width: 86px;
        }
        .auto-style2 {
            width: 123px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
          <table width="100%">
            <tr>
                <td class="tr2style">
                    Contract.
                </td>
                <td>
                    <asp:TextBox ID="txtContract" runat="server"></asp:TextBox>
                </td>
                <td class="tr2style">
                    Supplier:
                </td>
                <td>
                    <asp:TextBox ID="txtsuppid" runat="server"></asp:TextBox>
                    <input id="Button1" type="button" runat="server" value="..." onclick="searchJo()" class="button_top" />
                </td>
            </tr>
            <tr>
                <td class="tr2style">
                    Start Create Time.
                </td>
                <td>
                    <asp:TextBox ID="txtStartTrxDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                </td>
                <td class="tr2style">
                    End
                    Create Time.</td>
                <td>
                 <asp:TextBox ID="txtEndTrxDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4" align="right">
                    <asp:Button ID="btnQuery" runat="server" CssClass="button_top" Text="Query" OnClick="btnQuery_Click" />
                    
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <input name="btnToExcel" type="button" class="button_top" value="ToExcel" onclick="toPaseExcel()" />
                
                
                </td>
            </tr>
        </table>
        <hr noshade size="1" />
        <div id="ExcTable">
        <asp:Repeater ID="datalist" runat="server">
        <HeaderTemplate>
          <table width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                        border-collapse: collapse">
                        <tr>
                            <td class="tr2style">
                                承揽企业
                            </td>
                            <td class="tr2style">
                                单次合同号
                            </td>
                            <td class="tr2style">
                                JO
                            </td>
                             <td class="tr2style">
                                币种
                            </td>
                             <td class="tr2style">
                                工序
                            </td>
                             <td class="tr2style">
                               YPD
                            </td>
                            <td class="tr2style">
                                发出数量
                            </td>
                            <td class="tr2style">
                                收回正品
                            </td>
                            <td class="tr2style">
                                收回次品
                            </td>
                            <td class="tr2style">
                                外厂遗失
                            </td>
                            <td class="tr2style">
                                本厂疵品
                            </td>
                             <td class="tr2style">
                                含税单价
                            </td>
                             <td class="tr2style">
                                正品加工费
                            </td>
                            <td class="tr2style">
                                遗失扣款
                            </td>
                             <td class="tr2style">
                                其它调整
                            </td>
                             <td class="tr2style">
                                实付加工费
                            </td>
                              <td class="tr2style">
                                调整原因
                            </td>
                            
                           
                        </tr>
                    
                    
        </HeaderTemplate>
        <ItemTemplate>
        <tr>
                            <td class="dataTableStyle">
                                <%#Eval("NAME")%>
                            </td>
                            <td class="dataTableStyle">
                                 <%#Eval("ContractNo")%>
                            </td>
                            <td class="dataTableStyle">
                               <%#Eval("JOB_ORDER_NO")%>
                            </td>
                             <td class="dataTableStyle">
                                <%#Eval("CCY_CD")%>
                            </td>
                             <td class="dataTableStyle">
                                <%#Eval("PROCESS_DESC")%>
                            </td>
                             <td class="dataTableStyle" align="right">
                               <%#Eval("YPD")%>
                            </td>
                            <td class="dataTableStyle" align="right">
                                 <%#Eval("OUTQty")%>
                            </td>
                            <td class="dataTableStyle" align="right">
                                 <%#Eval("STAQty")%>
                            </td>
                            <td class="dataTableStyle" align="right">
                                 <%#Eval("STL2Qty")%>
                            </td>
                               <td class="dataTableStyle" align="right">
                                 <%#Eval("OUMTLQty")%>
                            </td>
                            <td class="dataTableStyle" align="right">
                                 <%#Eval("FAULTQty")%>
                            </td>
                            <td class="dataTableStyle" align="right">
                                <%#Eval("All_CONTRACT_PRICE")%>
                            </td>
                             <td class="dataTableStyle" align="right">
                                <%#(Eval("All_CONTRACT_PRICE").ToDouble()*Eval("STAQty").ToInt("d"))%>
                            </td>
                            <td class="dataTableStyle" align="right">
                                 <%#Eval("CompensationPrice")%>
                            </td>
                             <td class="dataTableStyle" align="right">
                                <%#Eval("ADJ_AMOUNT")%>
                            </td>
                             <td class="dataTableStyle" align="right">
                                 <%#(Eval("All_CONTRACT_PRICE").ToDouble() * Eval("STAQty").ToInt("d") - Eval("CompensationPrice").ToDouble() * Eval("OUMTLQty").ToInt("d") + Eval("ADJ_AMOUNT").ToDouble())%>
                            </td>
                              <td class="dataTableStyle">
                                 <%#Eval("ADJ_REASON")%>
                            </td>
                            
                           
                        </tr>
        </ItemTemplate>
        <FooterTemplate>
        </table>
        </FooterTemplate>
        </asp:Repeater>
        </div>
    </div>
    </form>
</body>
</html>
