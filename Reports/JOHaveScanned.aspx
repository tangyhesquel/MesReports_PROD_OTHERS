<%@ Page Language="C#" AutoEventWireup="true" CodeFile="JOHaveScanned.aspx.cs" Inherits="Reports_mesMarkerCSBreakdown" %>

<%@ Register Src="~/UserControls/ExportDataUserControl.ascx" TagPrefix="mes" TagName="ExportData" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Have already scanned Report</title>
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
        .style1
        {
            width: 91px;
        }
        .style2
        {
            background-color: #EFEFE0;
            width: 94px;
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
                    <td class="tdbackcolor" style="width: 12em">
                        <b>(针/梭)Garment Type</b>
                    </td>
                    <td class="style1">
                        <asp:DropDownList ID="ddlType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                    <td class="style2" align="center">
                        <b>(部门)Process:</b>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProcess_cd" runat="server" DataTextField="SHORT_NAME" DataValueField="PROCESS_CD">
                        </asp:DropDownList>
                    </td>
                    <%-- Added By ZouShiChang ON 2013.09.24 Start MES024  --%>
                    <td class="style2" align="center">
                        <b>(部门类型)Process Type:</b>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProcessType" runat="server">
                            <asp:ListItem></asp:ListItem>

                        </asp:DropDownList>
                    </td>
                    <td class="style2" align="center">
                        <b>(生产工厂)Production Factory:</b>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProdFactory" runat="server">
                        </asp:DropDownList>
                    </td>
                    <%-- Added By ZouShiChang ON 2013.09.24 End MES024  --%>
                    <td width="8%" height="19" class="tdbackcolor">
                        Transaction Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtSubmitDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"
                            Width="75px"></asp:TextBox>
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
                    Have already scanned Report
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
                    <asp:GridView ID="gvBody" runat="server" Width="100%" AutoGenerateColumns="TRUE"
                        OnRowDataBound="gvBody_RowDataBound" ShowFooter="false">
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
