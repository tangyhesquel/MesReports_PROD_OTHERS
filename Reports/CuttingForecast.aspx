<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CuttingForecast.aspx.cs"
    Inherits="Reports_PreCutSummary" %>

<%@ Register Src="~/UserControls/LanguageSelectUserControl.ascx" TagPrefix="mes"
    TagName="lang" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Cutting Forecast Report</title>
    <link href="../StyleSheet.css" type="text/css" rel="Stylesheet" />
    <style type="text/css">
        td
        {
            font-size: 9pt;
            text-align: right;
        }
        .ThinBorderTable
        {
            border-width: 1px;
            border-style: solid;
            border-collapse: collapse;
            border-color: Black;
        }
        .ThinBorderTable TD
        {
            border-width: 1px;
            border-style: solid;
        }
        .ThinBorderTable TH
        {
            border-width: 1px;
            border-style: solid;
        }
        </style>
    <script language="javascript" type="text/javascript">
        function ToExcelOfWPS() {
            var myExcel, myBook;
            try {
                myExcel = new ActiveXObject("ET.Application");
            } catch (Exception) {
                alert("Open WPS Application exception");
                return;
            }

            if (myExcel != null) {
                var sel = document.body.createTextRange();
                sel.moveToElementText(ExcTable);
                sel.select();
                sel.execCommand("Copy");
                sel.execCommand("Unselect");
                myExcel.Visible = true;
                myBook = myExcel.Workbooks.Add();
                myBook.sheets("Sheet1").Paste();
            }

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="100%">
            <tr>
                <td style="text-align: left">
                    <div id="divQuery">
                        GO：&nbsp;&nbsp;<asp:TextBox ID="txtGO" runat="server"></asp:TextBox>
                        MO：&nbsp;&nbsp;<asp:TextBox ID="txtMO" runat="server"></asp:TextBox>
                        &nbsp;&nbsp;
                        <asp:Checkbox ID="CHBfOSCut" runat="server"
                            oncheckedchanged="CHBfOSCut_CheckedChanged" Text="Before Over/Short Cut" Visible=false />&nbsp;
                        
                        <%--<Added by MF on 20160215, JO Combination>--%>
                        <asp:Checkbox ID="ByLot" runat="server" Text="By Lot" Font-Size="13px" GroupName="BeforeOrLot" />&nbsp;
                        <%--<End of added by MF on 20160215, JO Combination>--%>
                        
                        <mes:lang ID="langControl" runat="server" />
                        <asp:CheckBox ID="CHVersion" runat="server" Text="New Version" Font-Size="13px" 
                            oncheckedchanged="CHVersion_CheckedChanged" />&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button" OnClick="btnQuery_Click" />
                        &nbsp;<asp:Button ID="btnExcel" runat="server" CssClass="button" Text="To Excel"
                            OnClick="btnExcel_Click" />
                        &nbsp;<input type="button" value="To WPS" onclick="ToExcelOfWPS()" class="button" />
                        <br />
                        <br />
                    </div>
                </td>
            </tr>
            <tr>
                <td style="text-align: left">
                    <div id="ExcTable" runat="server">
                        <table width="100%" border="0" cellspacing="2" cellpadding="0" style="font-size: 12px;
                            border-collapse: collapse">
                            <tr>
                                <td style="text-align: center">
                                    <h2>
                                        Cutting Forecast Report</h2>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: left">
                                    <div runat="server" id="Div0">
                                    </div>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: left">
                                    <div id="Div1" runat="server">
                                    </div>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: left">
                                    <div runat="server" id="Div2">
                                        <br />
                                    </div>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: left">
                                    <h4>
                                        Remark:</h4>
                                    <div runat="server" id="Div3">
                                    </div>
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
