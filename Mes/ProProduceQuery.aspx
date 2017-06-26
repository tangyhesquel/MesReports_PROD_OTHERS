<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProProduceQuery.aspx.cs"
    Inherits="Mes_ProProduceQuery" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <link rel="Stylesheet" href="../Css/Style.css" />
    <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <script src="../Script/Common.js" type="text/javascript"></script>
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
            font-family: Tahoma,Verdana,MS Sans Serif,Courier New; /* width: 809px;*/
        }
        .style2
        {
            width: 275px;
        }
    </style>
</head>
<body topmargin="0" style="font-size: 12px" leftmargin="0" marginwidth="0" marginheight="0">
    <form id="form1" runat="server">
    <div>
        <asp:HiddenField ID="hfValue" runat="server" />
        <table width="100%" bgcolor="#c7dff1">
            <tr>
                <td align="center">
                    <h3>
                        <asp:Literal runat="server" ID="Literal1" Text="<%$ Resources:GlobalResources,STRING_TRANSACTION_OUT_INQUERY %>"></asp:Literal></h3>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td style="width: 60px">
                                <asp:Literal runat="server" ID="Literal2" Text="<%$ Resources:GlobalResources,STRING_GO_NO %>"></asp:Literal>
                            </td>
                            <td style="width: 224">
                                <asp:TextBox ID="txtGo" runat="server" Width="157px" Height="19px"></asp:TextBox>&nbsp;&nbsp;<asp:CheckBox
                                    ID="chbGo" runat="server" Text="<%$ Resources:GlobalResources,STRING_FUZZY %>" />
                            </td>
                            <td style="width: 80px">
                                <asp:Literal runat="server" ID="Literal3" Text="<%$ Resources:GlobalResources,STRING_GMT_TYPE %>"></asp:Literal>
                            </td>
                            <td class="style2">
                                <asp:DropDownList ID="ddlWipGarmentType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlWipGarmentType_SelectedIndexChanged">
                                    <asp:ListItem Text="All" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Knit" Value="K"></asp:ListItem>
                                    <asp:ListItem Text="Woven" Value="W"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:CheckBox ID="chkShippedQty" runat="server" Text="Shipped Qty" />
                                <asp:CheckBox ID="CheckBoxIncludeOutSource" runat="server" Text="<%$ Resources:GlobalResources,STRING_WIP_INCLUDE_OUTSOURCE_PROCESS_OUT %>" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 60px">
                                <asp:Literal runat="server" ID="Literal4" Text="<%$ Resources:GlobalResources,STRING_PROCESS %>"></asp:Literal>
                            </td>
                            <td style="width: 224px">
                                <asp:DropDownList ID="ddlDept" runat="server" Width="224px" Height="16px" AutoPostBack="True"
                                    OnSelectedIndexChanged="ddlDept_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <td style="width: 80px">
                                <asp:Literal runat="server" ID="Literal5" Text="<%$ Resources:GlobalResources,STRING_PROD_LINE %>"></asp:Literal>
                            </td>
                            <td class="style2">
                                <asp:DropDownList ID="ddlPart" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <%--Added By ZouShiChang ON 2013.09.24 Start MES024 --%>
                        <tr>
                            <td style="width: 60px">
                                <asp:Literal runat="server" ID="Literal7" Text="<%$ Resources:GlobalResources,STRING_PROCESS_TYPE %>"></asp:Literal>
                            </td>
                            <td style="width: 224px">
                                <asp:DropDownList ID="ddlProcessType" runat="server" Width="224px" Height="16px"
                                    AutoPostBack="True">
                                    <asp:ListItem ></asp:ListItem>
                                   
                                </asp:DropDownList>
                            </td>
                            <td style="width: 80px">
                                <asp:Literal runat="server" ID="Literal8" Text="<%$ Resources:GlobalResources,STRING_PRODUCTION_FACTORY %>"></asp:Literal>
                            </td>
                            <td class="style2">
                                <asp:DropDownList ID="ddlProdFactory" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <%--Added By ZouShiChang ON 2013.09.24 End MES024 --%>
                        <tr>
                            <td style="width: 60px">
                                <asp:Literal runat="server" ID="Literal6" Text="<%$ Resources:GlobalResources,STRING_TRANS_DATE %>"></asp:Literal>
                            </td>
                            <td style="width: 240px">
                                <asp:TextBox ID="txtBeginDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"
                                    Width="99px"></asp:TextBox>&nbsp;To&nbsp;<asp:TextBox ID="txtEndDate" runat="server"
                                        onfocus="WdatePicker({skin:'whyGreen'})" Width="101px"></asp:TextBox>
                            </td>
                            <td style="width: 80px">
                                <asp:Literal runat="server" ID="Literal27" Text="<%$ Resources:GlobalResources,STRING_INC_OUTSOURCE %>"></asp:Literal>&nbsp;
                            </td>
                            <td class="style2">
                                <asp:CheckBox ID="CBIsOutsource" runat="server" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnWipQuery_Click"
                                    CssClass="button1" />&nbsp;&nbsp;<input type="button" class="button1" value="To Excel"
                                        onclick="toPaseExcel()" />&nbsp;&nbsp;<input type="button" class="button1" value="To WPS"
                                            onclick="toPaseWPS()" />
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
            <font color="red">Because Did Not Choose any Condition, Please Input Reference Condition.</font>
        </div>
        <div id="ExcTable">
            <asp:GridView ID="gvBody" runat="server" Width="100%" BackColor="White" BorderColor="#CCCCCC"
                BorderStyle="None" BorderWidth="1px" CellPadding="3" OnRowDataBound="gvBody_RowDataBound"
                EnableViewState="False" ShowFooter="True" OnSelectedIndexChanged="gvBody_SelectedIndexChanged">
                <RowStyle ForeColor="#000066" />
                <FooterStyle BackColor="#0082c6" ForeColor="#000066" />
                <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#0082c6" Font-Bold="True" ForeColor="White" />
            </asp:GridView>
        </div>
    </div>
    </form>
</body>
</html>
