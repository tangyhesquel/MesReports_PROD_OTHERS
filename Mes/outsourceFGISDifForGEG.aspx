<%@ Page Language="C#" AutoEventWireup="true" CodeFile="outsourceFGISDifForGEG.aspx.cs"
    Inherits="Mes_outsourceFGISDif" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>GEG MES & FGIS Issue Difference Report(V_WH->TOSTOCKA)</title>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script type="text/javascript">
        function searchJo() {
            var urlName = "../Reports/searchJO.aspx?factory=<%=Factory_CD %>&userRandom=" + (Math.random() * 100000);
            var jo = window.showModalDialog(urlName, "Job Order No.", "dialogWidth=650pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (jo == null) return;
            document.all.txtJoNo.value = jo;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search </legend>
            <table width="100%" id="queryTab" align="center">
                <tr>
                    <td width="8%" height="19" class="tdbackcolor">
                        Garment Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlGarmentType" runat="server">
                            <asp:ListItem Value="" Text="All"></asp:ListItem>
                            <asp:ListItem Value="K" Text="Kint"></asp:ListItem>
                            <asp:ListItem Value="W" Text="Wovent"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td width="10%" height="19" class="tdbackcolor">
                        JO No:
                    </td>
                    <td>
                        <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                        <input type="button" value="..." onclick="searchJo()" class="button_top" />
                    </td>
                    <td width="8%" height="19" class="tdbackcolor">
                        Date From:
                    </td>
                    <td>
                        <asp:TextBox ID="txtStartDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td />
                    <td width="8%" height="19" class="tdbackcolor">
                        Date To:
                    </td>
                    <td>
                        <asp:TextBox ID="txtEndDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade="noshade" size="1" />
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
                        GEG MES & FGIS Issue Difference Report(V_WH-&gt;TOSTOCKA)</h2>
                    <h3>
                        V_WH-&gt;TOSTOCKA 过仓数与仓库进仓数差异报表</h3>
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
                            class="button_top" />
                        <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,'Production Cycle Time Detail Report.htm')"
                            class="button_top" />
                        <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="height: 26"
                            class="button_top" />
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
                                <%=Factory_CD%>
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
        </table>
        <asp:GridView ID="gvbody" runat="server" Width="100%" ShowFooter="false" AutoGenerateColumns="False"
            OnRowDataBound="gvBody_RowDataBound">
            <FooterStyle CssClass="tr2style" />
            <HeaderStyle CssClass="tr2style" />
            <Columns>
                <asp:BoundField DataField="JOB_ORDER_NO" HeaderText="JO NO" />
                <asp:BoundField DataField="COLOR_CODE" HeaderText="颜色" />
                <asp:BoundField DataField="SIZE_CODE" HeaderText="尺码" />
                <asp:BoundField DataField="QTY" HeaderText="MES V_WH->TOSTOCKA过仓数" />
                <asp:BoundField DataField="QTY2" HeaderText="仓库进仓数" />
                <asp:BoundField DataField="QtyDif" HeaderText="差异" />
            </Columns>
        </asp:GridView>
        <div id="dvMsg" runat="server" visible="false">
            <table id="TMsg" width="100%">
                <tr style="color: Red; font-size: large">
                    <td align="center">
                        No Data!
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
