<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProStatusQuery.aspx.cs" Inherits="Mes_ProStatusQuery" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <link rel="Stylesheet" href="../Css/Style.css" />
    <script src="../Script/Common.js" type="text/javascript"></script>
    <script type="text/javascript">
        function ShowDetail(object) {
            table = object.parentElement.parentElement.parentElement;
            if (table.rows(2).style.display == "none") {
                table.rows(1).style.display = "block";
                table.rows(2).style.display = "block";
                object.innerText = "－"
            }
            else {
                table.rows(1).style.display = "none";
                table.rows(2).style.display = "none";
                object.innerText = "＋";
            }
        }
        function searchCustomer() {
            var value = window.showModalDialog("Search.aspx?Type=C&single=1", "", "dialogWidth=800px;");
            if (value != undefined) {
                if (value != "") {
                    document.getElementById("txtCustomerCd").value = value.split(';')[0];
                }
            }
        }
        function searchStyle() {
            var value = window.showModalDialog("SearchStyle.aspx?single=1", "", "dialogWidth=800px;");
            if (value != undefined) {
                if (value != "") {
                    document.getElementById("txtStyleNo").value = value.split(';')[0];
                }
            }
        }
        function searchJo() {
            var value = window.showModalDialog("../Reports/SearchJO.aspx?site=" + document.getElementById("hfValue").value + "", "", "dialogWidth=800px;");
            if (value != undefined) {
                if (value != "") {
                    document.getElementById("txtJo").value = value;
                }
            }
        }
        function searchGo() {
            var value = window.showModalDialog("SearchGO.aspx?site=" + document.getElementById("hfValue").value + "", "", "dialogWidth=800px;");
            if (value != undefined) {
                if (value != "") {
                    document.getElementById("txtGo").value = value;
                }
            }
        }
        function searchWipJo() {
            var value = window.showModalDialog("../Reports/SearchJO.aspx?site=" + document.getElementById("hfValue").value + "", "", "dialogWidth=800px;");
            if (value != undefined) {
                if (value != "") {
                    document.getElementById("txtWipJo").value = value;
                }
            }
        }
    </script>
    <style>
        body
        {
            font-size: 8.5pt;
            font-family: Tahoma,Verdana,MS Sans Serif,Courier New;
            scrollbar-face-color: #ffffff;
            scrollbar-highlight-color: #ffffff;
            scrollbar-shadow-color: #000000;
            scrollbar-3dlight-color: #000000;
            scrollbar-arrow-color: #000000;
            scrollbar-track-color: #ffffff;
            scrollbar-darkshadow-color: #cccccc;
        }
        table
        {
            font-size: 8.5pt;
            font-family: Tahoma,Verdana,MS Sans Serif,Courier New;
        }
    </style>
</head>
<body topmargin="0" style="font-size: 12px" leftmargin="0" marginwidth="0" marginheight="0">
    <asp:HiddenField ID="hfValue" runat="server" />
    <form id="form1" runat="server">
    <div>
        <table width="100%" bgcolor="#c7dff1">
            <tr>
                <td align="center">
                    <h3>
                        <asp:Literal runat="server" ID="Literal6" Text="<%$Resources:GlobalResources,STRING_PRODUCTION_INQUERY %>"></asp:Literal></h3>
                </td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td style="width: 70%">
                    <table width="800px">
                        <tr>
                            <td style="width: 100px">
                                <asp:Literal runat="server" ID="Literal1" Text="<%$Resources:GlobalResources,STRING_JO_NO %>"></asp:Literal>:
                            </td>
                            <td style="width: 180px">
                                <asp:TextBox ID="txtJo" runat="server"></asp:TextBox>&nbsp;<asp:CheckBox ID="chbJo"
                                    runat="server" Text="<%$Resources:GlobalResources,STRING_FUZZY %>" />
                            </td>
                            <td style="width: 80px">
                                <asp:Literal runat="server" ID="Literal2" Text="<%$Resources:GlobalResources,STRING_GO_NO %>"></asp:Literal>:
                            </td>
                            <td style="width: 180px">
                                <asp:TextBox ID="txtGo" runat="server"></asp:TextBox>&nbsp;<asp:CheckBox ID="chbGo"
                                    runat="server" Text="<%$Resources:GlobalResources,STRING_FUZZY %>" />
                            </td>
                            <td style="width: 170px">
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Literal runat="server" ID="Literal3" Text="<%$Resources:GlobalResources,STRING_STYLE_NO %>"></asp:Literal>:
                            </td>
                            <td>
                                <asp:TextBox ID="txtStyleNo" runat="server"></asp:TextBox>&nbsp;<asp:CheckBox ID="chbStyle"
                                    runat="server" Text="<%$Resources:GlobalResources,STRING_FUZZY %>" />
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal4" Text="<%$Resources:GlobalResources,STRING_CUSTOMER_CD %>"></asp:Literal>:
                            </td>
                            <td>
                                <asp:TextBox ID="txtCustomerCd" runat="server"></asp:TextBox>&nbsp;<asp:CheckBox
                                    ID="chbCustomer" runat="server" Text="<%$Resources:GlobalResources,STRING_FUZZY %>" />
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal27" Visible="false" Text="<%$ Resources:GlobalResources,STRING_INC_OUTSOURCE %>"></asp:Literal>&nbsp;
                                <asp:CheckBox ID="CBIsOutsource" runat="server" Visible="false" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Literal runat="server" ID="Literal5" Text="<%$Resources:GlobalResources,STRING_PRODUCT_CATEGORY %>"></asp:Literal>:
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlProductCat" runat="server">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal9" Text="<%$Resources:GlobalResources,STRING_PODATE %>"></asp:Literal>:
                            </td>
                            <td>
                                <asp:TextBox ID="txtBeginDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"
                                    Width="80"></asp:TextBox>&nbsp;To&nbsp;<asp:TextBox ID="txtEndDate" runat="server"
                                        onfocus="WdatePicker({skin:'whyGreen'})" Width="80"></asp:TextBox>&nbsp;&nbsp;
                            </td>
                            <td>
                                <asp:RadioButtonList ID="rblDate" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="BPO" Selected>BPO Date</asp:ListItem>
                                    <asp:ListItem Value="PPC">PPC Date</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Literal runat="server" ID="Literal8" Text="<%$Resources:GlobalResources,STRING_INQUERY_RANGE %>"></asp:Literal>:
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlType" runat="server">
                                    <asp:ListItem Text="<%$Resources:GlobalResources,STRING_ALL_JO %>" Value=""></asp:ListItem>
                                    <asp:ListItem Text="<%$Resources:GlobalResources,STRING_ANNUAL_JO %>" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="<%$Resources:GlobalResources,STRING_UNCLOSE_JO %>" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal7" Text="<%$Resources:GlobalResources,STRING_GMT_TYPE %>"></asp:Literal>:
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlGarmentType" runat="server" Width="80">
                                    <asp:ListItem Text="All" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Woven" Value="W"></asp:ListItem>
                                    <asp:ListItem Text="Knit" Value="K"></asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:CheckBox ID="chkShippedQty" runat="server" Text="Shipped Qty" />
                            </td>
                            <td align="left">
                                &nbsp;
                                <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button1" OnClick="btnQuery_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                &nbsp;<asp:Button runat="server" CssClass="button1" Text="To Excel" ID="txtToExcel"
                                    OnClick="txtToExcel_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <hr />
        <br />
        <div style="text-align: Left; font-size: small;" runat="server" id="divNoCondition"
            visible="false">
            <font color="red">Because Did Not Choose any Condition, Only show the JOs of the BPO
                Date from Last 6 month to Next 6 Month.</font>
        </div>
        <div style="text-align: center; font-size: xx-large;" runat="server" id="divCancelled"
            visible="false">
            <font color="red">Cancelled</font>
        </div>
        <br />
        <div style="text-align: right">
            <a href="#" onclick="window.open ('ProLineDetail.aspx?id=<%=id==""?"":id %>','','height=300, width=600, top=150, left=250, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no')"
                style="text-decoration: underline">
                <asp:Literal runat="server" ID="Literal10" Text="<%$Resources:GlobalResources,STRING_LINE_DATA%>"></asp:Literal></a></div>
        <br />
        <table width="100%" border="1" cellpadding="0" cellspacing="0" style="border-collapse: collapse"
            id="ExcTable" runat="server" visible="false">
            <tr>
                <td rowspan="2" style="width: 7%" valign="top">
                    <asp:DataList ID="dlJo" runat="server" Width="100%" OnSelectedIndexChanged="dlJo_SelectedIndexChanged">
                        <HeaderTemplate>
                            <div style="text-align: center">
                                PO NO</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            &nbsp;&nbsp;<asp:LinkButton ID="lbJo" CommandName="Select" runat="server" Text='<%#Eval("JO_NO")%>'></asp:LinkButton>
                        </ItemTemplate>
                    </asp:DataList>
                    <div style="width: 100%; text-align: center" runat="server" id="divEmpty" visible="false">
                        <font color='red'>
                            <asp:Literal runat="server" ID="Literal11" Text="<%$Resources:GlobalResources,STRING_NO_DATE%>"></asp:Literal></font>
                    </div>
                </td>
                <td valign="top">
                    <table width="100%">
                        <tr>
                            <td valign="top">
                                <div style="width: 100%; text-align: center" runat="server" id="divBasicInfo" visible="false">
                                    <font color='red'>
                                        <asp:Literal runat="server" ID="Literal12" Text="<%$Resources:GlobalResources,STRING_NO_DATE%>"></asp:Literal></font>
                                </div>
                                <%--<asp:DataList ID="dlBasinInfo" runat="server" Width="100%" 
                                    EnableViewState="False">
                                    <ItemTemplate>
                                        
                                    </ItemTemplate>
                                </asp:DataList>--%>
                                <table width="100%">
                                    <tr>
                                        <td colspan="12" align="center">
                                            <asp:Literal runat="server" ID="Literal13" Text="<%$Resources:GlobalResources,STRING_JO_BASIC_INFO%>"></asp:Literal>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Literal runat="server" ID="Literal14" Text="<%$Resources:GlobalResources,STRING_CUSTOMER%>"></asp:Literal>:
                                        </td>
                                        <td>
                                            <%=SHORT_NAME%>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="Literal15" Text="<%$Resources:GlobalResources,STRING_ORDER_QTY%>"></asp:Literal>:
                                        </td>
                                        <td>
                                            <%=JOCC%>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="Literal16" Text="<%$Resources:GlobalResources,STRING_OS_ALLOWED%>"></asp:Literal>:
                                        </td>
                                        <td>
                                            <%=shipallowed%>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="Literal17" Text="<%$Resources:GlobalResources,STRING_SHIP_DATE%>"></asp:Literal>:
                                        </td>
                                        <td>
                                            <%=BUYER_PO_DEL_DATE%>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="Literal18" Text="<%$Resources:GlobalResources,STRING_MARKER_CD%>"></asp:Literal>:
                                        </td>
                                        <td>
                                            <%=MARKET_CD%>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="Literal19" Text="<%$Resources:GlobalResources,STRING_LINE_CODE%>"></asp:Literal>:
                                        </td>
                                        <td>
                                            <%=ProductLine %>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="Literal20" Text="<%$Resources:GlobalResources,STRING_SHIP_MODE%>"></asp:Literal>:
                                        </td>
                                        <td>
                                            <%=SHIP_MODE_CD%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Literal runat="server" ID="Literal21" Text="<%$Resources:GlobalResources,STRING_CUST_STYLE%>"></asp:Literal>:
                                        </td>
                                        <td>
                                            <%=CUST_STYLE_NO%>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="Literal22" Text="<%$Resources:GlobalResources,STRING_EMB_STATUS%>"></asp:Literal>:
                                        </td>
                                        <td>
                                            <%=EMBROIDERY%>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="Literal23" Text="<%$Resources:GlobalResources,STRING_PRINT_STATUS%>"></asp:Literal>:
                                        </td>
                                        <td>
                                            <%=PRINTING%>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="Literal24" Text="<%$Resources:GlobalResources,STRING_WASH_TYPE%>"></asp:Literal>:
                                        </td>
                                        <td>
                                            <%=WASH_TYPE_CD%>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="Literal25" Text="<%$Resources:GlobalResources,STRING_STYLE_DESC%>"></asp:Literal>:
                                        </td>
                                        <td colspan="3">
                                            <%=STYLE_CHN_DESC%>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                                <hr />
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:Literal runat="server" ID="Literal26" Text="<%$Resources:GlobalResources,STRING_TRANSFER_STATUS_AREA%>"></asp:Literal>
                                <div id="divTransInfo" runat="server">
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <asp:GridView ID="gvExcel" runat="server" Width="100%" Visible="false" OnRowDataBound="gvExcel_RowDataBound"
            ShowFooter="True">
        </asp:GridView>
        <asp:GridView ID="gvBody" runat="server" Width="100%" CellPadding="4" ForeColor="#333333"
            GridLines="None" OnRowDataBound="gvBody_RowDataBound" OnSelectedIndexChanging="gvBody_SelectedIndexChanging"
            ShowFooter="True">
            <RowStyle BackColor="#EFF3FB" />
            <Columns>
                <asp:CommandField SelectText="<%$Resources:GlobalResources,STRING_VIEW%>" ShowSelectButton="True" />
            </Columns>
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" HorizontalAlign="Left" />
            <EditRowStyle BackColor="#2461BF" />
            <AlternatingRowStyle BackColor="White" />
        </asp:GridView>
    </div>
    </form>
</body>
</html>
