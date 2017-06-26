<%@ Page Language="C#" AutoEventWireup="true" CodeFile="proDailyOutput.aspx.cs" Inherits="Reports_proDailyOutput" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Production Daily Output Report</title>
    <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <link rel="Stylesheet" href="../Css/StyleReport.css" type="text/css" />
    <script language="javascript" type="text/javascript">
        function init() {
            if (!("query".equals(wipForm.getActionFlag()))) {
                document.all.reportDiv.style.display = 'none';
            }
        }
        function queryPage(startingIndex, endIndex, pages) {

            document.all.actionFlag.value = "query";
            document.all.droptable.value = "False";
            document.all.startingIndex.value = startingIndex;
            document.all.endIndex.value = endIndex;
            document.all.pages.value = pages;
            document.frm.submit();
        }
    </script>
    <script type="text/javascript" src="../Script/Common.js"></script>
</head>
<body>
    <form id="frm" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search</legend>
            <table width="100%" id="queryTab" align="center">
                <tr>
                    <td width="12%" height="19" class="tdbackcolor">
                        Factory Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlfactoryCd" runat="server" DataTextField="DEPARTMENT_ID"
                            DataValueField="DEPARTMENT_ID">
                        </asp:DropDownList>
                    </td>
                    <td width="10%" height="19" class="tdbackcolor">
                        Garment Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlgarmentType" runat="server" OnSelectedIndexChanged="ddlgarmentType_SelectedIndexChanged"
                            AutoPostBack="True">
                            <asp:ListItem Value="">ALL</asp:ListItem>
                            <asp:ListItem Value="W">Woven</asp:ListItem>
                            <asp:ListItem Value="K">Knit</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    
                    <td class="tdbackcolor">
                        Production Group:
                    </td>
                    <td class="style2">
                        <asp:DropDownList ID="ddGroupName" runat="server">
                            <asp:ListItem Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    
                    <td width="10%" height="19" class="tdbackcolor">
                        Wash Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlWashType" runat="server">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="Wash">Wash</asp:ListItem>
                            <asp:ListItem Value="NW">No Wash</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <%-- Added By ZouShiChang ON 2013.09.24 End MES024  --%>
                    <td width="12%" height="19" class="tdbackcolor">
                        Transation Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                 </tr>
                 <tr>
                        <td width="12%" height="19" class="tdbackcolor">
                        Process Code:
                    </td>
                    <td>
                        <!-- Added By ZouShiChang On 2013.08.29 MES024-->
                        <asp:DropDownList ID="ddlprocessCd" runat="server" DataTextField="PROCESS_CD" DataValueField="PROCESS_CD">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>                   
                    </td>
                    <%-- Added By ZouShiChang ON 2013.09.24 Start MES024  --%>
                    <td align="center" class="tdbackcolor">
                        Process Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProcessType" runat="server">
                            <asp:ListItem ></asp:ListItem>
                        
                        </asp:DropDownList>
                    </td>
                    <td align="center" class="tdbackcolor">
                        Production Factory:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProdFactory" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                    </td>
                 </tr>
            </table>
        </fieldset>
        <hr noshade size="1">
    </div>
    <div id="ExcTable">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" style="font-size: 12px;
            border-collapse: collapse">
            <tr>
                <td align="center">
                    <h2>
                        Garment Stock Movement Daily Report(By day)</h2>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="mmPrint" align="left">
                        <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv.style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv.style.display='block';document.all.mmPrint.style.visibility='visible'"
                            style="font-size: 16; width: 80; height: 26"/>
                        <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,'Production Daily Output Report.htm')"
                            style="font-size: 16; width: 80; height: 26"/>
                        <input type="button" name="ToExcel" value="ToExcel" onclick="toPcmExcel()" style="font-size: 16;
                            width: 80; height: 26"/>
                        <input type="button" name="ToWps" value="ToWps" onclick="ToExcelOfWPS()" style="font-size: 16;
                            width: 80; height: 26" />
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                        border-collapse: collapse">
                        <tr>
                            <td class="tr2style" bgcolor="#efefe7" width="100">
                                Factory CD:
                            </td>
                            <td>
                                <%=ddlfactoryCd.SelectedItem.Text %>
                            </td>
                            <td class="tr2style" bgcolor="#efefe7" width="100">
                                Garment type:
                            </td>
                            <td>
                                <%=ddlgarmentType.SelectedItem.Text %>
                            </td>
                            <td class="tr2style" bgcolor="#efefe7" width="100">
                                Wash Type:
                            </td>
                            <td>
                                <%=ddlWashType.SelectedItem.Text%>
                            </td>
                            <td class="tr2style" bgcolor="#efefe7" width="100">
                                Process CD:
                            </td>
                            <td>
                                <%=ddlprocessCd.SelectedItem.Text %>
                            </td>
                            <td class="tr2style" bgcolor="#efefe7" width="100">
                                Transaction Date:
                            </td>
                            <td>
                                <%=txtDate.Text %>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView ID="gvBody" runat="server" Width="100%" OnRowDataBound="gvBody_RowDataBound"
                        AutoGenerateColumns="False" ShowFooter="true" ShowHeader="true">
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
