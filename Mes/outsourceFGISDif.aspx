<%@ Page Language="C#" AutoEventWireup="true" CodeFile="outsourceFGISDif.aspx.cs"
    Inherits="Mes_outsourceFGISDif" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Outsource MES & FGIS Issue Difference Report</title>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../Script/Common.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search </legend>
            <table width="100%" id="queryTab" align="center">
                <tr>
                    <td width="10%" height="19" class="tdbackcolor">
                        Factory Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFactoryCd" runat="server" DataTextField="DEPARTMENT_ID"
                            DataValueField="DEPARTMENT_ID">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td width="10%" height="19" class="tdbackcolor">
                        JO No:
                    </td>
                    <td>
                        <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                    </td>
                    <td width="8%" height="19" class="tdbackcolor">
                        Date From:
                    </td>
                    <td>
                        <asp:TextBox ID="txtStartDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td>
                        <td width="8%" height="19" class="tdbackcolor">
                            Date To:
                        </td>
                        <td>
                            <asp:TextBox ID="txtEndDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                        </td>
                        <td>
                            <td>
                                <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                            </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade size="1">
    </div>
    <div id="ExcTable">
        <table width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td align="center">
                    <h2>
                        Outsource MES & FGIS Issue Difference Report</h2>
                    <h3>
                        外发过仓&进仓差异报表</h3>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <div id="mmPrint" align="right">
                        <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv.style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv.style.display='block';document.all.mmPrint.style.visibility='visible'"
                            style="font-size: 16; width: 80; height: 26">
                        <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,'Production Cycle Time Detail Report.htm')"
                            style="font-size: 16; width: 80; height: 26">
                        <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="height: 26"
                            class="button_top">
                        <input type="button" name="excel" value="To Wps" onclick="ToExcelOfWPS()" style="height: 26"
                            class="button_top" />
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" border="1" cellpadding="0" cellspacing="0">
                        <tr>
                            <td width="10%" height="19" class="tdbackcolor">
                                Factory Code:
                            </td>
                            <td>
                                <%=ddlFactoryCd.SelectedItem.Text %>
                                &nbsp;
                            </td>
                            <td width="10%" height="19" class="tdbackcolor">
                                JO No:
                            </td>
                            <td>
                                <%=txtJoNo.Text.Trim() %>
                                &nbsp;
                            </td>
                            <td width="10%" height="19" class="tdbackcolor">
                                Date From:
                            </td>
                            <td>
                                <%=txtStartDate.Text %>
                                &nbsp;
                            </td>
                            <td width="10%" height="19" class="tdbackcolor">
                                Date To:
                            </td>
                            <td>
                                <%=txtEndDate.Text %>
                                &nbsp;
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
                    <table id="allTable" width="100%" border="1" cellpadding="0" cellspacing="0">
                        <tr>
                            <td class="tr2style" bgcolor="#efefe7" width="15%" colspan="2">
                                JO
                            </td>
                            <td class="tr2style" bgcolor="#efefe7" width="15%" colspan="2">
                                MES外发单过A仓数量
                            </td>
                            <td class="tr2style" bgcolor="#efefe7" colspan="2">
                                仓库进O01仓数量
                            </td>
                            <td class="tr2style" bgcolor="#efefe7" colspan="2">
                                差异
                            </td>
                        </tr>
                        <div id="divBody" runat="server">
                        </div>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
