<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GarmentTransferForAllDepts.aspx.cs"
    Inherits="GarmentTransferForAllDepts" %>

<%@ Register TagPrefix="MES" TagName="MultiLang" Src="~/UserControls/LanguageSelectUserControl.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Garment Transfer For All Depts Report</title>
    <link type="text/css" rel="stylesheet" href="../StyleSheet.css" />
    <style type="text/css" media="print">
        .Noprint
        {
            display: none;
        }
    </style>
    <object id="WebBrowser" classid="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2" height="0"
        width="0">
    </object>
</head>
<body>
    <form id="form1" runat="server">
    <input style="float: right" type="button" class="button Noprint" runat="server" value="<%$Resources:GlobalResources,STRING_BUTTON_PRINT %>"
        onclick="document.all.WebBrowser.ExecWB(6,6)" />
    <div style="float: right" class="Noprint">
        <MES:MultiLang EnableViewState="true" ID="MulLangDiag" runat="server" />
    </div>
    <h3>
        <asp:Literal runat="server" ID="Title" Text="<%$Resources:GlobalResources, STRING_GARMENT_TRANSFER_RPT_TITLE_ALL_DEPTS %>"> </asp:Literal>
    </h3>
    <table style="padding: 2em">
        <tr>
            <td class="FieldStyle">
                <asp:Literal ID="Literal1" runat="server" Text="<%$Resources:GlobalResources, STRING_TRANSFER_TIME %>"> </asp:Literal>
            </td>
            <td>
                <asp:Literal ID="LiteralTransferTime" runat="server"></asp:Literal>
            </td>
            <td class="FieldStyle">
                <asp:Literal ID="Literal3" runat="server" Text="<%$Resources:GlobalResources, STRING_USER %>"> </asp:Literal>
            </td>
            <td>
                <asp:Literal ID="LiteralUser" runat="server"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td class="FieldStyle">
                <asp:Literal ID="Literal11" runat="server" Text="<%$Resources:GlobalResources, STRING_FROM_DEPARTMENT %>"> </asp:Literal>
            </td>
            <td>
                <asp:Literal ID="LiteralFromDept" runat="server"></asp:Literal>
            </td>
            <td class="FieldStyle">
                <asp:Literal ID="Literal5" runat="server" Text="<%$Resources:GlobalResources, STRING_FROM_LINE %>"> </asp:Literal>
            </td>
            <td>
                <asp:Literal ID="LiteralFromLine" runat="server"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td class="FieldStyle">
                <asp:Literal ID="Literal6" runat="server" Text="<%$Resources:GlobalResources, STRING_TO_DEPARTMENT %>"> </asp:Literal>
            </td>
            <td>
                <asp:Literal ID="LiteralToDept" runat="server"></asp:Literal>
            </td>
            <td class="FieldStyle">
                <asp:Literal ID="Literal9" runat="server" Text="<%$Resources:GlobalResources, STRING_TO_LINE %>"> </asp:Literal>
            </td>
            <td>
                <asp:Literal ID="LiteralToLine" runat="server"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td class="FieldStyle">
                <asp:Literal ID="Literal7" runat="server" Text="<%$Resources:GlobalResources, STRING_DOC_NO %>"> </asp:Literal>
            </td>
            <td>
                <asp:Literal ID="LiteralDocNO" runat="server"></asp:Literal>
            </td>
            <td class="FieldStyle">
                <asp:Literal ID="Literal8" runat="server" Text="<%$Resources:GlobalResources, STRING_TOTAL_QTY %>"> </asp:Literal>
            </td>
            <td>
                <asp:Literal ID="LiteralTotalQty" runat="server"></asp:Literal>
            </td>
        </tr>
    </table>
    <div style="clear: both">
        <asp:Repeater ID="RepeaterJo" runat="server" OnItemDataBound="RepeaterJo_ItemDataBound">
            <ItemTemplate>
                <table>
                    <tr>
                        <td class="tableHeaderWithoutColor">
                            <asp:Literal ID="LiteralJobOrder" runat="server" Text="<%$Resources:GlobalResources, STRING_JO_NO %>"> </asp:Literal>
                        </td>
                        <td>
                            <%#Eval("JOB_ORDER_NO")%>
                        </td>
                    </tr>
                    <tr>
                        <td class="tableHeaderWithoutColor">
                            <asp:Literal ID="Literal2" runat="server" Text="<%$Resources:GlobalResources, STRING_CUSTOMER %>"> </asp:Literal>
                        </td>
                        <td>
                            <%#Eval("CUSTOMER_NAME")%>
                        </td>
                    </tr>
                </table>
                <asp:GridView ID="GridViewDetail" runat="server" AutoGenerateColumns="false" ShowFooter="true">
                    <Columns>
                        <asp:BoundField DataField="COLOR_CD" HeaderText="<%$Resources:GlobalResources, STRING_COLOR_CD %>"
                            HeaderStyle-CssClass="tableHeaderWithoutColor" />
                        <asp:BoundField DataField="COLOR_DESC" HeaderText="<%$Resources:GlobalResources, STRING_COLOR_DESC %>"
                            HeaderStyle-CssClass="tableHeaderWithoutColor" />
                        <asp:BoundField DataField="SIZE_CD" HeaderText="<%$Resources:GlobalResources, STRING_SIZE_CD %>"
                            HeaderStyle-CssClass="tableHeaderWithoutColor" />
                        <asp:BoundField DataField="QTY" HeaderText="<%$Resources:GlobalResources, STRING_QTY %>"
                            HeaderStyle-CssClass="tableHeaderWithoutColor" />
                    </Columns>
                </asp:GridView>
                <br />
            </ItemTemplate>
        </asp:Repeater>
    </div>
    </form>
</body>
</html>
