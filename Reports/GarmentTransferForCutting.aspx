<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GarmentTransferForCutting.aspx.cs"
    Inherits="Reports_GarmentTransferForCutting" %>

<%@ Register TagPrefix="MES" TagName="MultiLang" Src="~/UserControls/LanguageSelectUserControl.ascx" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Garment Transfer For Cutting Report</title>
    <link type="text/css" rel="stylesheet" href="../StyleSheet.css" />
    <style type="text/css" media="print">
        .Noprint
        {
            display: none;
        }
        .PaperWidth
        {
            width: 50px;
            font-size: xx-large;
            font-weight: 900;
        }
    </style>
    <style type="text/css">
        .PaperWidth
        {
            width: 20em;
        }
    </style>
    <object id="WebBrowser" classid="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2" height="0"
        width="0">
    </object>
</head>
<body class="PaperWidth">
    <form id="form1" runat="server">
    <input style="float: right" type="button" class="button Noprint" runat="server" value="<%$Resources:GlobalResources,STRING_BUTTON_PRINT %>"
        onclick="document.all.WebBrowser.ExecWB(6,6)" />
    <div style="float: right" class="Noprint">
        <MES:MultiLang EnableViewState="true" ID="MulLangDiag" runat="server" />
    </div>
    <table style="padding: 2em">
        <tr>
            <td class="FieldStyle">
                <asp:Literal ID="Literal1" runat="server" Text="<%$Resources:GlobalResources, STRING_FROM_DEPARTMENT %>"> </asp:Literal>
            </td>
            <td>
                <asp:Literal ID="LiteralFromDept" runat="server"></asp:Literal>
            </td>
            <td class="FieldStyle">
                <asp:Literal ID="Literal3" runat="server" Text="<%$Resources:GlobalResources, STRING_FROM_LINE %>"> </asp:Literal>
            </td>
            <td>
                <asp:Literal ID="LiteralFromLine" runat="server"></asp:Literal>
            </td>
            <td class="FieldStyle">
                <asp:Literal ID="Literal5" runat="server" Text="<%$Resources:GlobalResources, STRING_TO_LINE %>"> </asp:Literal>
            </td>
            <td>
                <asp:Literal ID="LiteralToLine" runat="server"></asp:Literal>
            </td>
            <td class="FieldStyle">
                <asp:Literal ID="Literal7" runat="server" Text="<%$Resources:GlobalResources, STRING_DOC_NO %>"> </asp:Literal>
            </td>
            <td>
                <asp:Literal ID="LiteralDocNO" runat="server"></asp:Literal>
            </td>
        </tr>
    </table>
    <div style="clear: both">
        <div style="float: left">
            <span class="FieldStyle">
                <asp:Literal ID="Literal2" runat="server" Text="<%$Resources:GlobalResources, STRING_CARD_USER_ID %>"> </asp:Literal></span>
            <asp:Literal ID="LiteralUserID" runat="server"></asp:Literal>
        </div>
        <div style="float: left; margin-left: 2em;">
            <span class="FieldStyle">
                <asp:Literal ID="Literal4" runat="server" Text="<%$Resources:GlobalResources, STRING_DELIVERY_DATE %>"> </asp:Literal></span>
            <span>
                <asp:Literal ID="LiteralDeliverDate" runat="server"></asp:Literal>
            </span>
        </div>
    </div>
    <div style="clear: both">
        <asp:Repeater ID="RepeaterJo" runat="server" OnItemDataBound="RepeaterJo_ItemDataBound">
            <ItemTemplate>
                <table>
                    <tr>
                        <td class="tableHeader">
                            <asp:Literal ID="LiteralJobOrder" runat="server" Text="<%$Resources:GlobalResources, STRING_JO_NO %>"> </asp:Literal>
                        </td>
                        <td>
                            <%#Eval("JOB_ORDER_NO")%>
                        </td>
                    </tr>
                </table>
                <asp:GridView ID="GridViewDetail" runat="server" AutoGenerateColumns="false" ShowFooter="true">
                    <Columns>
                        <asp:BoundField DataField="CUT_LAY_NO" HeaderText="<%$Resources:GlobalResources, STRING_CUT_LOT_NO %>"
                            HeaderStyle-CssClass="tableHeader" />
                        <asp:BoundField DataField="COLOR_CD" HeaderText="<%$Resources:GlobalResources, STRING_COLOR_CD %>"
                            HeaderStyle-CssClass="tableHeader" />
                        <asp:BoundField DataField="BUNDLE_NO" HeaderText="<%$Resources:GlobalResources, STRING_BUNDLE_NO %>"
                            HeaderStyle-CssClass="tableHeader" />
                        <asp:BoundField DataField="SIZE_CD" HeaderText="<%$Resources:GlobalResources, STRING_SIZE_CD %>"
                            HeaderStyle-CssClass="tableHeader" FooterText="TOTAL:" FooterStyle-CssClass="tableHeader" />
                        <asp:BoundField DataField="QTY" HeaderText="<%$Resources:GlobalResources, STRING_QTY %>"
                            HeaderStyle-CssClass="tableHeader" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:GridView ID="GridViewSummary" runat="server" HeaderStyle-CssClass="tableHeader"
                    AutoGenerateColumns="true" ShowFooter="false" ShowHeader="false" RowStyle-CssClass="tableHeader"
                    AlternatingRowStyle-CssClass="">
                </asp:GridView>
                <br />
            </ItemTemplate>
        </asp:Repeater>
    </div>
    </form>
</body>
</html>
