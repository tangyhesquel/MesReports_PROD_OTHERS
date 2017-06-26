<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CuttingDcWIPDetailWoven.aspx.cs"
    Inherits="Reports_mesMarkerCSBreakdown" %>

<%@ Register Src="~/UserControls/ExportDataUserControl.ascx" TagPrefix="mes" TagName="ExportData" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Cutting and Sewing WIP Report</title>
    <link rel="Stylesheet" href="../Css/StyleReport.css" />
    <link rel="Stylesheet" href="../Css/StyleReport_Print.css" media="print" />
    <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <meta http-equiv="pragma" content="no-cache" />
    <script type="text/javascript" language="javascript">
        function init() {

            //            if (!"query".equals(cutPlanForm.getActionFlag())) {

            //                document.all.ExcTable.style.display = 'none';
            //            }
        }
        function searchMarkerNo() {
            //            var urlName = "mesSearchMarkerNo.aspx";
            //            var markerNo = window.showModalDialog(urlName, "Marker No.", "dialogWidth=450pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            //            if (markerNo == null) return;
            //            document.all.txtMoNo.value = markerNo;
        }
    </script>
    <style type="text/css">
        .style2
        {
            background-color: #EFEFE0;
            width: 209px;
        }
        .style5
        {
            width: 183px;
        }
        .style6
        {
            background-color: #EFEFE0;
            width: 18em;
        }
        .style7
        {
            background-color: #EFEFE0;
            width: 185px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="queryDiv" class="HiddenWhilePrint">
        <fieldset>
            <legend>Search</legend>
            <table width="100%" id="queryTab">
                <tr>
                    <td class="style6" align="center">
                        <b><%=Factory_CD%></b>
                    </td>
                    <%--<td class="style1">
                        <asp:DropDownList ID="ddlType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>--%>
                    <td class="style2" align="center">
                        <b>Garment Type</b>
                    </td>
                    <td class="style5">
                        <asp:DropDownList ID="ddGarmentType" runat="server" Height="21px" Width="169px" OnSelectedIndexChanged="ddProcessCd_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Selected="True" Value="W">Woven</asp:ListItem>
                            <asp:ListItem Value="K">Knit</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="style2" align="center">
                        <b>Process Type</b>
                    </td>
                    <td class="style5">
                        <asp:DropDownList ID="ddProcessType" runat="server" Height="21px" Width="169px" OnSelectedIndexChanged="ddProcessCd_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Selected="True" Value="I">I</asp:ListItem>
                            <asp:ListItem Value="P">P</asp:ListItem>
                            <asp:ListItem Value="O">O</asp:ListItem>
                            <asp:ListItem Value="T">T</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="style2" align="center">
                        <b>Department</b>
                    </td>
                    <td class="style5">
                        <asp:DropDownList ID="ddProcessCd" runat="server" Height="21px" Width="169px" OnSelectedIndexChanged="ddProcessCd_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Selected="True" Value="ALL">ALL</asp:ListItem>
                            <asp:ListItem Value="CUT">CUT</asp:ListItem>
                            <asp:ListItem Value="SEW">SEW</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <%--<td class="style3">
                        <asp:DropDownList ID="ddlProcess_cd" runat="server" DataTextField="SHORT_NAME" DataValueField="PROCESS_CD">
                        </asp:DropDownList>
                    </td>--%>
                    <td class="style2" align="center">
                        <b>Production Line</b>
                    </td>
                    <td class="style5">
                        <asp:DropDownList ID="ddlProductionLine" runat="server" DataTextField="Production_Line_cd"
                            DataValueField="PRODUCTION_LINE_cd" Height="21px" Width="169px">
                        </asp:DropDownList>
                    </td>
                    <td class="style2" align="center">
                        <b>Language</b>
                    </td>
                    <td class="style5">
                        <asp:DropDownList ID="FlagList" runat="server" AutoPostBack="false">
                            <asp:ListItem Value="English" Selected="True">English</asp:ListItem>
                            <asp:ListItem Value="Chinese">中文(Chinese)</asp:ListItem>
                        </asp:DropDownList>
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
    <div id="divMsg" runat="server">
    </div>
    <div id="EtTable">
        <table id="ExcTable" width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="center" style="font-size: large">
                    Cutting and Sewing WIP Report
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
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divBody" runat="server">
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
                    <div id="divGvBodySum" runat="Server" visible="false">
                        <table border="0" cellspacing="0" cellpadding="0" width="100%">
                            <tr>
                                <td>
                                    <font size="3"><b>Production Line Level</b></font>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:GridView ID="gvBodySum" runat="server" Width="100%" AutoGenerateColumns="TRUE"
                                        OnRowDataBound="gvBodySum_RowDataBound" ShowFooter="TRUE" HeaderStyle-BackColor="#F7F6F3">
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divGvBody" runat="Server" visible="false">
                        <table border="0" cellspacing="0" cellpadding="0" width="100%">
                            <tr>
                                <td>
                                    <font size="3"><b>JO Level</b></font>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:GridView ID="gvBody" runat="server" Width="100%" AutoGenerateColumns="true"
                                        ShowFooter="true" OnRowDataBound="gvBody_RowDataBound">
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
