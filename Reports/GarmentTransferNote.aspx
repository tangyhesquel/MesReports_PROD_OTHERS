<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GarmentTransferNote.aspx.cs"
    Inherits="GarmentTransferNote" %>

<%@ Register TagPrefix="MES" TagName="MultiLang" Src="~/UserControls/LanguageSelectUserControl.ascx" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Garment Transfer Note</title>
    <link type="text/css" rel="stylesheet" href="../StyleSheet.css" />
    <meta http-equiv="pragma" content="no-cache" />
    <style type="text/css" media="print">
        .Noprint
        {
            display: none;
        }
        .PaperWidth
        {
            width: 100%;
        }
    </style>
    <style type="text/css">
        .PaperWidth
        {
            width: 26em;
        }
        .borderTalbe, .borderTalbe td
        {
            border-width: 1px;
            border-style: dotted;
            border-collapse: collapse;
        }
        .centerAlignTable
        {
            width: 97%;
        }
        .NoBorder
        {
            border-width: 0px;
            border-style: hidden;
        }
        .NoLeftRightBorder
        {
            border-left-style: none;
            border-right-style: none;
        }
        .NoBorderExceptTop
        {
            border-width: 0px;
            border-style: hidden;
            border-top-width: 1px;
            border-top-style: solid;
        }
        .NoBottomBorder
        {
            border-bottom-style:none;
        }
        .style1
        {
            font-weight: bold;
            width: 9em;
        }
        .style2
        {
            width: 180px;
        }
        .style3
        {
            font-weight: bold;
            width: 122px;
        }
    </style>
    <object id="WebBrowser" classid="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2" height="0"
        width="0">
    </object>
    <script type="text/javascript" language="javascript">
        function selectColumn() {
            var num = Math.random();
            var str = window.showModalDialog("ReportCustomSetting.aspx?site=<%=FactoryCD %>&ReportName=GarmentTransferNote&User_ID=<%=User_id %>&randnum=" + num, null, "font-size:10px;dialogWidth:60em;dialogHeight:50em;scrollbars=no;status=no");
            window.location.reload();
        }
    </script>
</head>
<body class="PaperWidth" style="padding: 1em; padding-top: 0px;">
    <form id="form1" runat="server" visible="True">
    <div class="Noprint">
        <MES:MultiLang EnableViewState="true" ID="MulLangDiag" runat="server" />
        <input id="Button1" style="float: right" type="button" class="button Noprint" runat="server"
            value="<%$Resources:GlobalResources,STRING_BUTTON_PRINT %>" onclick="document.all.WebBrowser.ExecWB(6,6)" />
        <input id="Button3" style="float: right; visibility: hidden; width: 2em" type="text"
            value="&nbsp;" />
        <input id="btCustom_Column" style="float: right; width: 8em" type="button" class="button Noprint"
            runat="server" value="Custom Column" onclick="javascript:selectColumn()" visible="False" />
    </div>
    <div style="font-weight: bold; font-size: 1em; clear: both;">
        <%=PageHeader%></div>
    <br />
    <asp:Label CssClass="FieldStyle" ID="lblDocNO" runat="server" Text="<%$Resources:GlobalResources,STRING_DOC_NO %>"></asp:Label>
    <asp:Literal ID="lblDocNOText" runat="server" Text=""></asp:Literal>
    <asp:Label CssClass="FieldStyle" ID="lblUserName" runat="server" Text="<%$Resources:GlobalResources, STRING_USER%>"></asp:Label>
    <asp:Literal ID="lblUserNameText" runat="server" Text=""></asp:Literal>
    <img id="barcodeImage" src="<%=ResolveUrl("~/BarcodeHandler.ashx")%>?<%=lblDocNOText.Text %>" />

    <table class="borderTalbe centerAlignTable">
        <tr>
            <td class="style1">
                <asp:Literal ID="Literal6" runat="server" Text="<%$Resources:GlobalResources, STRING_DATE_TIME %>"> </asp:Literal>
            </td>
            <td class="style2" colspan="2">
                <asp:Literal ID="LiteralDateTime" runat="server"></asp:Literal>
            </td>
            <td class="style3">
                <asp:Literal ID="Literal1" runat="server" Text="<%$Resources:GlobalResources, STRING_FROM_DEPARTMENT %>"> </asp:Literal>
            </td>
            <td>
                <asp:Literal ID="LiteralFromDept" runat="server"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Literal ID="Literal3" runat="server" Text="<%$Resources:GlobalResources, STRING_FROM_LINE %>"> </asp:Literal>
            </td>
            <td class="style2">
                <asp:Literal ID="LiteralFromLine" runat="server"></asp:Literal>
            </td> 
            <td class="style3" rowspan="2">
                <asp:Literal ID="remark" runat="server" Text="<%$Resources:GlobalResources, STRING_REMARK %>"> </asp:Literal>
            </td>
            <td class="NoBottomBorder" colspan="2" rowspan="2" style="vertical-align:top">
                <asp:Literal ID="LiteraltoRemark" runat="server"></asp:Literal></td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Literal ID="Literal5" runat="server" Text="<%$Resources:GlobalResources, STRING_TO_LINE %>"> </asp:Literal>
            </td>
            <td>
                <asp:Literal ID="LiteralToLine" runat="server"></asp:Literal>
            </td>
        </tr>
    </table>
    <%-- <div style="clear:both">
        <div style="float:left">
            <span class="FieldStyle"><asp:Literal ID="Literal2" runat="server" Text="<%$Resources:GlobalResources, STRING_CARD_USER_ID %>"> </asp:Literal></span>
            <asp:Literal ID="LiteralUserID" runat="server"></asp:Literal>
        </div>
        <div style="float:left; margin-left:2em;">
                 <span class="FieldStyle"><asp:Literal ID="Literal4" runat="server" Text="<%$Resources:GlobalResources, STRING_DELIVERY_DATE %>"> </asp:Literal></span>
                <span><asp:Literal ID="LiteralDeliverDate" runat="server"></asp:Literal> </span>
        </div>
    </div>--%>
    <div id="divYMGdetail" runat="server" style="clear: both" visible="False">
        <br />
        <asp:GridView ID="grYMGDetail" runat="server" Width="97%" AutoGenerateColumns="False"
            CellPadding="4" ForeColor="#333333" EnableModelValidation="True">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="center" />
            <Columns>
                <%--<asp:TemplateField   HeaderText="<%$Resources:GlobalResources, STRING_SERIAL_NUMBER %>" > 
                 <ItemTemplate>  
                   <%# (((GridViewRow)Container).DataItemIndex + 1) %>  
                   </ItemTemplate>  
                    <ItemStyle Width="3em" />
                </asp:TemplateField >--%>
                <asp:BoundField DataField="SEQ_NO" HeaderText="<%$Resources:GlobalResources, STRING_SERIAL_NUMBER %>">
                    <ItemStyle Width="3em"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="job_order_no" HeaderText="<%$Resources:GlobalResources, STRING_JO_NO %>">
                    <ItemStyle Width="6em"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="color_CD" HeaderText="<%$Resources:GlobalResources, STRING_COLOR_CD %>">
                    <ItemStyle Width="6em"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Size_CD" HeaderText="<%$Resources:GlobalResources, STRING_SIZE_CD %>">
                    <ItemStyle Width="4em"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="CUT_LAY_NO" HeaderText="<%$Resources:GlobalResources, STRING_CUT_LOT_NO %>">
                    <ItemStyle Width="3em"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="BUNDLE_NO" HeaderText="<%$Resources:GlobalResources, STRING_BUNDLE_NO %>">
                    <ItemStyle Width="3em"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="QTY" HeaderText="<%$Resources:GlobalResources, STRING_QTY %>">
                    <ItemStyle Width="3em"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="" HeaderText="<%$Resources:GlobalResources, STRING_REMARK %>">
                    <%--<ItemStyle Width="8em"></ItemStyle>--%>
                </asp:BoundField>
            </Columns>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" BorderStyle="Double" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
    </div>
    <div id="div1" runat="server" style="clear: both">
        <br />
    </div>
    <div id="divYMG" runat="server" style="clear: both">
    </div>
    <div id="body" runat="server" style="clear: both">
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
                    <tr>
                        <td class="tableHeader">
                            <asp:Literal ID="Literal2" runat="server" Text="<%$Resources:GlobalResources, STRING_CUSTOMER %>"> </asp:Literal>
                        </td>
                        <td>
                            <%#Eval("CUSTOMER_NAME")%>
                        </td>
                    </tr>
                </table>
                <%--   <asp:GridView ID="GridViewDetail" runat="server" AutoGenerateColumns="false" ShowFooter="false" CssClass="borderTalbe" OnRowCreated="DetailGridRowCreated"  OnRowDataBound="">
                        <Columns>
                            <asp:BoundField DataField="CUT_LAY_NO" HeaderText="<%$Resources:GlobalResources, STRING_CUT_LOT_NO %>" HeaderStyle-CssClass="tableHeader" />
                            <asp:BoundField DataField="COLOR_CD" HeaderText="<%$Resources:GlobalResources, STRING_COLOR_CD %>" HeaderStyle-CssClass="tableHeader" />
                            <asp:BoundField DataField="BUNDLE_NO" HeaderText="<%$Resources:GlobalResources, STRING_BUNDLE_NO %>" HeaderStyle-CssClass="tableHeader" />
                            <asp:BoundField DataField="SIZE_CD" HeaderText="<%$Resources:GlobalResources, STRING_SIZE_CD %>" HeaderStyle-CssClass="tableHeader" />
                            <asp:BoundField DataField="QTY" HeaderText="<%$Resources:GlobalResources, STRING_QTY %>" HeaderStyle-CssClass="tableHeader" />                            
                        </Columns>                                                
                    </asp:GridView>--%>
                <%--<div id="divPrint" class="<%=PrintStatus1%>"> --%>
                <div id="divPrint" class="<%=PrintStatus1%>">
                    <table class="borderTalbe centerAlignTable">
                        <asp:Repeater ID="GridViewDetail" runat="server" OnItemDataBound="GridRowDataBound">
                            <HeaderTemplate>
                                <tr class="FieldStyle">
                                    <td>
                                        <asp:Literal ID="Literal1" runat="server" Text="<%$Resources:GlobalResources, STRING_CUT_LOT_NO %>"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:Literal ID="Literal4" runat="server" Text="<%$Resources:GlobalResources, STRING_COLOR_CD %>"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:Literal ID="Literal7" runat="server" Text="<%$Resources:GlobalResources, STRING_BUNDLE_NO %>"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:Literal ID="Literal8" runat="server" Text="<%$Resources:GlobalResources, STRING_SIZE_CD %>"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:Literal ID="Literal9" runat="server" Text="<%$Resources:GlobalResources, STRING_QTY %>"></asp:Literal>
                                    </td>
                                </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <%# Eval("CUT_LAY_NO")%>
                                    </td>
                                    <td>
                                        <%# Eval("COLOR_CD")%>
                                    </td>
                                    <td>
                                        <%# Eval("BUNDLE_NO")%>
                                    </td>
                                    <td>
                                        <%# Eval("SIZE_CD")%>
                                    </td>
                                    <td>
                                        <%# Eval("QTY")%>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </div>
                <br />
                <div class="FieldStyle">
                    <asp:Literal runat="server" ID="Literal10" Text="<%$Resources:GlobalResources,STRING_SUMMARY %>"></asp:Literal></div>
                <%if (FactoryCD == "GEG")
                  {
                %>
                <font size="1">
                    <% } %>
                    <asp:GridView ID="GridViewSummary" OnRowDataBound="GridViewSummaryRowBounded" runat="server"
                        CssClass="borderTalbe centerAlignTable" HeaderStyle-CssClass="tableHeader" AutoGenerateColumns="true"
                        ShowFooter="false" ShowHeader="false">
                    </asp:GridView>
                    <%if (FactoryCD == "GEG")
                      {
                    %>
                </font>
                <% } %>
                <br />
                <table class="borderTalbe centerAlignTable">
                    <tr>
                        <td class="FieldStyle">
                            <asp:Literal runat="server" ID="Literal11" Text="<%$Resources:GlobalResources,STRING_ACTUAL_QTY %>"></asp:Literal>
                        </td>
                        <td class="FieldStyle">
                            <asp:Literal runat="server" ID="Literal12" Text="<%$Resources:GlobalResources,STRING_JO_TOTAL_QTY %>"></asp:Literal>
                        </td>
                        <td class="FieldStyle">
                            <asp:Literal runat="server" ID="Literal13" Text="<%$Resources:GlobalResources,STRING_UP_TO_DATE_TOTAL %>"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <%#Eval("TOTAL_CUT_QTY")%>
                        </td>
                        <td>
                            <asp:Literal ID="literalTotal" runat="server"></asp:Literal>
                        </td>
                        <td>
                            <%# Eval("TOTAL_OUTPUT_QTY")%>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <%--<div class="FieldStyle" style="text-align:right">Page:<%=totalPages%> of <%=totalPages%></div>--%>
            </ItemTemplate>
        </asp:Repeater>
        
   
        <asp:Repeater ID="RepeaterCut" runat="server" OnItemDataBound="RepeaterCut_ItemDataBound">
            <ItemTemplate>
                <table>
                    <tr>
                        <td class="tableHeader">
                            <asp:Literal ID="Literaljo" runat="server" Text="GO NO." Visible="false"></asp:Literal>
                        </td>
                        <td>
                            <asp:Literal ID="lblCustomer" runat="server" Text=""></asp:Literal>
                        </td>
                        <td>&nbsp;</td>
                        <td class="tableHeader">
                            <asp:Literal ID="Literal2" runat="server" Text="<%$Resources:GlobalResources, STRING_CUSTOMER %>" Visible="false"> </asp:Literal>
                        </td>
                        <td>
                            <asp:Literal ID="lblSCNO" runat="server" Text=""></asp:Literal>
                        </td>
                    </tr> 
                </table>
                <br />
                <div class="FieldStyle">
                    <asp:Literal runat="server" ID="Literal10" Text="<%$Resources:GlobalResources,STRING_SUMMARY %>" Visible="false"></asp:Literal></div>
                <%--<%if (FactoryCD == "GEG")
                  {
                %>
                <font size="1">
                    <% } %>--%>
                    <asp:GridView ID="GridViewSummary" OnRowDataBound="GridViewSummaryRowBounded" runat="server"
                        CssClass="borderTalbe centerAlignTable" HeaderStyle-CssClass="tableHeader"
                        ShowFooter="false" ShowHeader="false">
                    </asp:GridView>
                    <%if (FactoryCD == "GEG")
                      {
                    %>
                </font>
                <% } %>
                <br />
                    <asp:GridView ID="GridViewTotal" runat="server"
                        CssClass="borderTalbe centerAlignTable" HeaderStyle-CssClass="tableHeader"
                        ShowFooter="false" ShowHeader="false">
                    </asp:GridView>
                <%--<table class="borderTalbe centerAlignTable">
                    <tr>
                        <td class="FieldStyle">
                            <asp:Literal runat="server" ID="Literal14" Text="<%$Resources:GlobalResources,STRING_JO_NO %>"></asp:Literal>
                        </td>
                        <td class="FieldStyle">
                            <asp:Literal runat="server" ID="Literal11" Text="<%$Resources:GlobalResources,STRING_ACTUAL_QTY %>"></asp:Literal>
                        </td>
                        <td class="FieldStyle">
                            <asp:Literal runat="server" ID="Literal12" Text="<%$Resources:GlobalResources,STRING_JO_TOTAL_QTY %>"></asp:Literal>
                        </td>
                        <td class="FieldStyle">
                            <asp:Literal runat="server" ID="Literal13" Text="<%$Resources:GlobalResources,STRING_UP_TO_DATE_TOTAL %>"></asp:Literal>
                        </td>
                        <td class="FieldStyle">
                            <asp:Literal runat="server" ID="Literal15" Text="<%$Resources:GlobalResources,STRING_BAL_QTY %>"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <%#Eval("JOB_ORDER_NO")%>
                        </td>
                        <td>
                            <%#Eval("TOTAL_CUT_QTY")%>
                        </td>
                        <td>
                            <asp:Literal ID="literalTotal" runat="server"></asp:Literal>
                        </td>
                        <td>
                            <%# Eval("TOTAL_OUTPUT_QTY")%>
                        </td>
                        <td>
                            <%#Eval("TOTAL_CUT_QTY")%> - <%#Eval("TOTAL_OUTPUT_QTY")%>
                        </td>
                    </tr>
                </table>--%>
                <br />
                <div id="divPrint" class="<%=PrintStatus1%>">
                    <asp:GridView ID="GridViewDetail" runat="server"
                        CssClass="borderTalbe centerAlignTable" HeaderStyle-CssClass="tableHeader"
                        ShowFooter="false" ShowHeader="false">
                    </asp:GridView>
                    <%--<table class="borderTalbe centerAlignTable">
                        <asp:Repeater ID="GridViewDetail" runat="server" OnItemDataBound="GridRowDataBound">
                            <HeaderTemplate>
                                <tr class="FieldStyle">
                                    <td>
                                        <asp:Literal ID="Literal1" runat="server" Text="<%$Resources:GlobalResources, STRING_CUT_LOT_NO %>"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:Literal ID="Literal4" runat="server" Text="<%$Resources:GlobalResources, STRING_COLOR_CD %>"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:Literal ID="Literal7" runat="server" Text="<%$Resources:GlobalResources, STRING_BUNDLE_NO %>"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:Literal ID="Literal8" runat="server" Text="<%$Resources:GlobalResources, STRING_SIZE_CD %>"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:Literal ID="Literal9" runat="server" Text="<%$Resources:GlobalResources, STRING_QTY %>"></asp:Literal>
                                    </td>
                                </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <%# Eval("CUT_LAY_NO")%>
                                    </td>
                                    <td>
                                        <%# Eval("COLOR_CD")%>
                                    </td>
                                    <td>
                                        <%# Eval("BUNDLE_NO")%>
                                    </td>
                                    <td>
                                        <%# Eval("SIZE_CD")%>
                                    </td>
                                    <td>
                                        <%# Eval("QTY")%>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>--%>
                </div>
                <br />
                <br />
                <%--<div class="FieldStyle" style="text-align:right">Page:<%=totalPages%> of <%=totalPages%></div>--%>
            </ItemTemplate>
        </asp:Repeater>
       
    </div>
    </form>
</body>
</html>
