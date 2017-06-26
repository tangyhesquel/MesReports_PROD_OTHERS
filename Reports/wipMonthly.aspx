<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wipMonthly.aspx.cs" Inherits="Mes_wipMonthly" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Production Monthly Output Report</title>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script language="javascript">
        function init() {
            if (document.getElementById("ddlMonth").value == "") {
                document.all.ExcTable.style.display = 'none';
            }
        }
    </script>
</head>
<body onload="init()">
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search</legend>
            <table width="100%" id="queryTab" align="center">
                <tr>
                    <td width="5%" height="19" class="tdbackcolor">
                        Factory:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFactoryCd" runat="server" DataTextField="FACTORY_ID" DataValueField="FACTORY_ID">
                        </asp:DropDownList>
                    </td>
                    <td align="right">
                        Garment Type
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlGarmentType" runat="server">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="W">Woven</asp:ListItem>
                            <asp:ListItem Value="K">Knit</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td width="75" height="19" class="tdbackcolor">
                        Wash Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlWashType" runat="server">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="WASH">Wash</asp:ListItem>
                            <asp:ListItem Value="NW">No Wash</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        Order Type
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlOrderType" runat="server">
                            <asp:ListItem Value="A">All</asp:ListItem>
                            <asp:ListItem Value="N">Turnkey</asp:ListItem>
                            <asp:ListItem Value="Y">OPA</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td width="5%" height="19" class="tdbackcolor">
                        Year:
                    </td>
                    <td width="20%">
                        <asp:DropDownList ID="ddlYear" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td width="5%" height="19" class="tdbackcolor">
                        Month:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlMonth" runat="server">
                            <asp:ListItem Value="1">January</asp:ListItem>
                            <asp:ListItem Value="2">February</asp:ListItem>
                            <asp:ListItem Value="3">March</asp:ListItem>
                            <asp:ListItem Value="4">April</asp:ListItem>
                            <asp:ListItem Value="5">May</asp:ListItem>
                            <asp:ListItem Value="6">June</asp:ListItem>
                            <asp:ListItem Value="7">July</asp:ListItem>
                            <asp:ListItem Value="8">August</asp:ListItem>
                            <asp:ListItem Value="9">September</asp:ListItem>
                            <asp:ListItem Value="10">October</asp:ListItem>
                            <asp:ListItem Value="11">November</asp:ListItem>
                            <asp:ListItem Value="12">December</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Button ID="btnQuery" runat="server" CssClass="button_top" Text="Query" OnClick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade size="1">
    </div>
    <table id="ExcTable" width="100%" border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td align="center">
                <h2>
                    Production Monthly Transaction(Out) Report
                </h2>
            </td>
        </tr>
        <tr>
            <td align="center">
                <h4>
                    Monthly:
                    <%=ddlMonth.SelectedItem.Text %>
                </h4>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td width="100%">
                <div id="mmPrint" align="right">
                    <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv .style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv .style.display='block';document.all.mmPrint.style.visibility='visible'"
                        style="font-size: 16; width: 80; height: 26">
                    <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,'ActualCuttingReport.htm')"
                        style="font-size: 16; width: 80; height: 26">
                    <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="font-size: 16;
                        width: 80; height: 26">
                    <input type="button" name="ToWps" value="ToWps" onclick="ToExcelOfWPS()" style="font-size: 16;
                        width: 80; height: 26" />
                </div>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <table border="1" cellspacing="0" cellpadding="0" style="font-size: 12px; border-collapse: collapse"
                    width="100%">
                    <tr>
                        <td class="tr2style" width="100">
                            Factory Code:
                        </td>
                        <td>
                            <%=ddlFactoryCd.SelectedItem.Text %>
                            &nbsp;
                        </td>
                        <td class="tr2style" width="100">
                            Garment Type:
                        </td>
                        <td>
                            <%=ddlGarmentType.SelectedItem.Text %>
                        </td>
                        <td class="tr2style" width="100">
                            Wash Type:
                        </td>
                        <td>
                            <%=ddlWashType.SelectedItem.Text %>
                        </td>
                        <td class="tr2style" width="100">
                            Order Type:
                        </td>
                        <td>
                            <%=ddlOrderType.SelectedItem.Text %>
                        </td>
                        <td class="tr2style" width="100">
                            Year:
                        </td>
                        <td>
                            <%=ddlYear.SelectedItem.Text %>
                            &nbsp;
                        </td>
                        <td class="tr2style" width="100">
                            Monthly:
                        </td>
                        <td>
                            <%=ddlMonth.SelectedItem.Text %>
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
                <table border="1" cellspacing="0" cellpadding="0" style="font-size: 12px; border-collapse: collapse">
                    <tr>
                        <td rowspan="2" class="tr2style">
                            Buyer
                        </td>
                        <td rowspan="2" class="tr2style">
                            S/C No
                        </td>
                        <td rowspan="2" class="tr2style" width="120" align="right">
                            J/O No
                        </td>
                        <td rowspan="2" class="tr2style">
                            Garment Type
                        </td>
                        <td rowspan="2" class="tr2style">
                            Wash Type
                        </td>
                        <td rowspan="2" class="tr2style">
                            Style No.
                        </td>
                        <td rowspan="2" class="tr2style" width="90" nowrap="nowrap">
                            Gmt Del Date
                        </td>
                        <td rowspan="2" class="tr2style" width="90" nowrap="nowrap">
                            &nbsp;&nbsp;&nbsp;&nbsp; PPCD&nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                        <td align="center" axis rowspan="2" class="tr2style" width="30" nowrap="nowrap">
                            ORDER QTY
                        </td>
                        <td align="center" axis rowspan="2" class="tr2style" width="30" nowrap="nowrap">
                            SAH
                        </td>
                        <div id="divTitle1" runat="server">
                        </div>
                    </tr>
                    <tr>
                        <div id="divTitle2" runat="server">
                        </div>
                    </tr>
                    <div id="divDetail" runat="server">
                    </div>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
