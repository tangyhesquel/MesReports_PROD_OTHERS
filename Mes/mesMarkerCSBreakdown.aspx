<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mesMarkerCSBreakdown.aspx.cs"
    Inherits="Reports_mesMarkerCSBreakdown" %>

<%@ Register Src="~/UserControls/ExportDataUserControl.ascx" TagPrefix="mes" TagName="ExportData" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Marker Color Size breakdown</title>
    <link rel="Stylesheet" href="../Css/StyleReport.css" />
    <link rel="Stylesheet" href="../Css/StyleReport_Print.css" media="print" />
    <meta http-equiv="pragma" content="no-cache" />
    <script language="javascript">
        function init() {

            if (!"query".equals(cutPlanForm.getActionFlag())) {

                document.all.ExcTable.style.display = 'none';
            }
        }
        function searchMarkerNo() {
            var urlName = "mesSearchMarkerNo.aspx";
            var markerNo = window.showModalDialog(urlName, "Marker No.", "dialogWidth=450pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (markerNo == null) return;
            document.all.txtMoNo.value = markerNo;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="queryDiv" class="HiddenWhilePrint">
        <fieldset>
            <legend>Search</legend>
            <table width="100%" id="queryTab">
                <tr>
                    <td class="tdbackcolor" width="100">
                        Marker Order No:
                    </td>
                    <td>
                        <asp:TextBox ID="txtMoNo" runat="server"></asp:TextBox>
                        <input type="button" value="..." onclick="searchMarkerNo()" class="button_top">
                    </td>
                    <td align="RIGHT">
                        <asp:Button ID="btnQuery" Text="Query" runat="server" CssClass="button_top" OnClick="btnQuery_Click" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade size="1">
    </div>
    <div id="EtTable">
        <table id="ExcTable" width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="center">
                    <font face="Arial" size="4">Marker Color Size breakdown</font>
                </td>
            </tr>
            <tr>
                <td>
                    <mes:ExportData ID="exportData" runat="server" EnableViewState="false" />
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" border="0" cellspacing="2" cellpadding="0" style="font-size: 12px;
                        border-collapse: collapse">
                        <tr>
                            <td width="100" nowrap>
                                Marker Order No:
                            </td>
                            <td>
                                &nbsp;<%=MO_NO %>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap>
                                JO Number :
                            </td>
                            <td>
                                &nbsp;<%=JO_NO %>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divFirst" runat="server">
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
                    <asp:GridView ID="gvBody" runat="server" AutoGenerateColumns="False" OnRowDataBound="gvBody_RowDataBound"
                        ShowFooter="True">
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
