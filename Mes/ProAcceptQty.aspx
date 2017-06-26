<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProAcceptQty.aspx.cs" Inherits="Mes_ProAcceptQty" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <link rel="Stylesheet" href="../Css/Style.css" />
    <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <script src="../Script/Common.js" type="text/javascript"></script>
</head>
<body topmargin="0" style="font-size: 12px" leftmargin="0" marginwidth="0" marginheight="0">
    <form id="form1" runat="server">
    <div>
        <asp:HiddenField ID="hfValue" runat="server" />
        <table width="100%" bgcolor="#c7dff1">
            <tr>
                <td align="center">
                    <h3>
                        <asp:Literal runat="server" ID="Title" Text="<%$Resources:GlobalResources,STRING_TRANSACTION_IN_INQUERY %>"></asp:Literal>
                    </h3>
                </td>
            </tr>
            <tr>
                <td>
                    <table width="700px">
                        <tr>
                            <td>
                                <asp:Literal runat="server" ID="Literal1" Text="<%$Resources:GlobalResources,STRING_TRANS_DATE %>"></asp:Literal>
                            </td>
                            <td>
                                <asp:TextBox ID="txtTrxBeginDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"
                                    Width="94px"></asp:TextBox>&nbsp;To&nbsp;<asp:TextBox ID="txtTrxEndDate" runat="server"
                                        onfocus="WdatePicker({skin:'whyGreen'})" Width="97px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal2" Text="<%$Resources:GlobalResources,STRING_GMT_TYPE %>"></asp:Literal>
                            </td>
                            <td colspan="2" align="left">
                                <asp:DropDownList ID="ddlGarmentType" runat="server">
                                    <asp:ListItem Text="All" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Knit" Value="K"></asp:ListItem>
                                    <asp:ListItem Text="Woven" Value="W"></asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;<asp:CheckBox ID="chkWareHouse" runat="server" Text="WareHouse" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Literal runat="server" ID="Literal3" Text="<%$Resources:GlobalResources,STRING_PODATE %>"></asp:Literal>
                            </td>
                            <td>
                                <asp:TextBox ID="txtBeginDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"
                                    Width="94px"></asp:TextBox>&nbsp;To&nbsp;<asp:TextBox ID="txtEndDate" runat="server"
                                        onfocus="WdatePicker({skin:'whyGreen'})" Width="97px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal6" Text="<%$Resources:GlobalResources,STRING_INC_OUTSOURCE %>"> </asp:Literal>
                            </td>
                            <td colspan="2">
                                <asp:CheckBox ID="CBIsOutsource" runat="server" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button1" OnClick="btnQuery_Click" />&nbsp;&nbsp;<input
                                    type="button" class="button1" value="To Excel" onclick="toPaseExcel()" />&nbsp;&nbsp;<input
                                        type="button" class="button1" value="To WPS" onclick="toPaseWPS()" />
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
            <asp:GridView ID="gvBody" runat="server" Width="100%" ShowFooter="True" OnRowDataBound="gvBody_RowDataBound">
            </asp:GridView>
        </div>
    </div>
    </form>
</body>
</html>
