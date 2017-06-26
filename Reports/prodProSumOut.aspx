<%@ Page Language="C#" AutoEventWireup="true" CodeFile="prodProSumOut.aspx.cs" Inherits="Reports_prodProSumOut" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Production Process Summary Output Report</title>
    <link rel="Stylesheet" href="../Css/StyleReport.css" />
    <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../Script/Common.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="queryDiv">
            <fieldset>
                <legend>Search</legend>
                <table width="100%" id="queryTab" align="center">
                    <tr>
                        <td width="12%" height="19" class="tdbackcolor">Factory Code:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlFactoryCd" runat="server" DataTextField="DEPARTMENT_ID"
                                DataValueField="DEPARTMENT_ID" Enabled="false">
                            </asp:DropDownList>
                        </td>
                        <td width="10%" height="19" class="tdbackcolor">Garment Type:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlgarmentType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlgarmentType_SelectedIndexChanged">
                                <asp:ListItem Value="">ALL</asp:ListItem>
                                <asp:ListItem Value="W">Woven</asp:ListItem>
                                <asp:ListItem Value="K">Knit</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td height="19" class="tdbackcolor">Wash Type:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlWashtype" runat="server">
                                <asp:ListItem Value="">All</asp:ListItem>
                                <asp:ListItem Value="WASH">Wash</asp:ListItem>
                                <asp:ListItem Value="NW">No Wash</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td width="12%" height="19" class="tdbackcolor">Process Code:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlProcessCd" runat="server" DataTextField="SHORT_NAME" DataValueField="PROCESS_CD">
                                <asp:ListItem Value="">
                                </asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td width="12%" height="19" class="tdbackcolor">Process Type:</td>
                        <td>
                            <asp:DropDownList ID="ddlProcessType" runat="server" >
                                <asp:ListItem >
                                </asp:ListItem>
                            </asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td width="15%" height="19" class="tdbackcolor">Production Factory</td>
                        <td>
                            <asp:DropDownList ID="ddlProductionFactory" runat="server">
                                <asp:ListItem >
                                </asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td width="15%" height="19" class="tdbackcolor">Job Order No:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                        </td>
                        <td width="8%" height="19" class="tdbackcolor">From Date:
                        </td>
                        <td>
                            <asp:TextBox ID="txtStartDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                        </td>
                        <td width="8%" height="19" class="tdbackcolor">To Date:
                        </td>
                        <td>
                            <asp:TextBox ID="txtEndDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                        </td>
                        <td colspan="2" align="right">
                            <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />&nbsp;
                        </td>
                    </tr>
                </table>
            </fieldset>
            <hr noshade size="1">
        </div>
        <div id="ExcTable">
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td width="20%">&nbsp;
                    </td>
                    <td width="60%">&nbsp;
                    </td>
                    <td width="20%" align="center"></td>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                    <td align="center">
                        <font face="Arial" size="4">Production Process Summary Output Report</font>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
                <tr>
                    <td width="20%">&nbsp;
                    </td>
                    <td width="60%" align="center">&nbsp;
                    </td>
                    <td width="20%" align="center">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="left"></td>
                </tr>
            </table>
            <div id="mmPrint" align="right">
                <input type="button" name="print" value="Print" onclick="javacript: document.all.queryDiv.style.display = 'none'; document.all.mmPrint.style.visibility = 'hidden'; window.print(); document.all.queryDiv.style.display = 'block'; document.all.mmPrint.style.visibility = 'visible'"
                    style="font-size: 16; width: 80; height: 26">
                <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs', false, 'Production Process Summary Output Report.htm')"
                    style="font-size: 16; width: 80; height: 26">
                <input type="button" name="ToExcel" value="ToExcel" onclick="toPaseExcel()" style="font-size: 16; width: 80; height: 26">
                <input type="button" name="ToWps" value="ToWps" onclick="ToExcelOfWPS()" style="font-size: 16; width: 80; height: 26" />
            </div>
            <br>
            <table id="allTable" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px; border-collapse: collapse">
                <tr>
                    <td colspan="13">
                        <table border="1" cellspacing="0" cellpadding="0" style="font-size: 12px; border-collapse: collapse"
                            width="100%">
                            <tr>
                                <td class="tr2style" bgcolor="#efefe7" width="100">Factory CD:
                                </td>
                                <td>
                                    <%=ddlFactoryCd.SelectedItem.Text %>&nbsp;
                                </td>
                                <td class="tr2style" bgcolor="#efefe7" width="100">Garment Type:
                                </td>
                                <td>
                                    <%=ddlgarmentType.SelectedItem.Text %>&nbsp;
                                </td>
                                <td class="tr2style" bgcolor="#efefe7" width="100">Washing Type:
                                </td>
                                <td>
                                    <%=ddlWashtype.SelectedItem.Text %>&nbsp;
                                </td>
                                <td class="tr2style" bgcolor="#efefe7" width="100">Process CD:
                                </td>
                                <td>
                                    <%=ddlProcessCd.SelectedItem.Text %>&nbsp;
                                </td>
                                <td class="tr2style" bgcolor="#efefe7" width="100">Start Date:
                                </td>
                                <td>
                                    <%=txtStartDate.Text %>&nbsp;
                                </td>
                                <td class="tr2style" bgcolor="#efefe7" width="100">End Date:
                                </td>
                                <td>
                                    <%=txtEndDate.Text %>&nbsp;
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="13">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="tr2style" bgcolor="#efefe7" width="15%">Buyer
                    </td>
                    <td class="tr2style" bgcolor="#efefe7">Go No
                    </td>
                    <td class="tr2style" bgcolor="#efefe7">Jo No
                    </td>
                    <td class="tr2style" bgcolor="#efefe7">Order Qty
                    </td>
                    <td class="tr2style" bgcolor="#efefe7">Style No
                    </td>
                    <td class="tr2style" bgcolor="#efefe7">Style Description
                    </td>
                    <td class="tr2style" bgcolor="#efefe7">Wash Type
                    </td>
                    <td class="tr2style" bgcolor="#efefe7">SAH
                    </td>
                    <td class="tr2style" bgcolor="#efefe7">BPD
                    </td>
                    <td class="tr2style" bgcolor="#efefe7">PPCD
                    </td>
                    <td class="tr2style" bgcolor="#efefe7">Route Type
                    </td>
                    <td class="tr2style" bgcolor="#efefe7">Process Code
                    </td>
<%--                    <td class="tr2style" bgcolor="#efefe7">Garment Type
                    </td>--%>
                    <td class="tr2style" bgcolor="#efefe7">Transaction Out Qty
                    </td>
                </tr>
                <div id="divDetail" runat="server">
                </div>
            </table>
        </div>
    </form>
</body>
</html>
