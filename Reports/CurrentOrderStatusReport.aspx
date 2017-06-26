<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CurrentOrderStatusReport.aspx.cs" Inherits="Reports_CurrentOrderStatusReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Job Order Transaction Detail Report</title>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    </head>
<body>
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search</legend>
            <table width="100%" id="queryTab" align="center">
                <tr>
                    <td width="80px" class="tdbackcolor">
                        Factory Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlfactoryCd" runat="server" DataTextField="DEPARTMENT_ID"
                            DataValueField="DEPARTMENT_ID">
                        </asp:DropDownList>
                    </td>
                    <td width="80px" class="tdbackcolor">
                        <asp:CheckBox ID="cbJO" runat="server" Text="JO" Checked="true" 
                            AutoPostBack="True" oncheckedchanged="cbJO_CheckedChanged" />
                    </td>
                    <td>
                        <asp:CheckBox ID="cbJOwithWIP" runat="server" Text="JO with WIP" 
                            AutoPostBack="True" oncheckedchanged="cbJOwithWIP_CheckedChanged" />
                    </td>
                    <%--added by zoushichang on 2013.09.23 start mes024 --%>
                    <td class="tdbackcolor">
                        <asp:CheckBox ID="cbShipped" runat="server" Text="Shipped" AutoPostBack="True" 
                            oncheckedchanged="cbShipped_CheckedChanged" />
                    </td>
                    <td>
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                    <%--added by zoushichang on 2013.09.23 end mes024 --%>
                </tr>
                <tr>
                    <td width="80px" class="tdbackcolor">
                        Job Order No:
                    </td>
                    <td>
                        <asp:TextBox ID="txtjobOrderNo" runat="server"></asp:TextBox>
                    </td>
                    <td width="80px" class="tdbackcolor">
                        From Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtBeginDate" runat="server" Width="120px" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td width="80px" class="tdbackcolor">
                        To Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtEndDate" runat="server" Width="120px" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td style="width: 22em">
                        &nbsp;</td>
                    <td style="width: 22em">
                        <%--<asp:Button ID="btnQuery" Text="Query" runat="server" OnClick="btnQuery_Click" />--%>
                        <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                    </td>
                    <td>
                        &nbsp;</td>
                </tr>
            </table>
        </fieldset>
        <%--       <hr noshade size="1">--%>
    </div>
    <div style="text-align:center">
            <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="Red" Text="Tip Message!" 
                Visible="False" Width="300px" Font-Size="Medium"></asp:Label>
        </div>
    <div id="ExcTable">
        <table width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td width="20%">
                    &nbsp;
                </td>
                <td width="60%">
                    &nbsp;
                </td>
                <td width="20%" align="center">
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td align="center">
                    <font face="Arial" size="4">Order Complete Status Report</font>
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td width="20%">
                    &nbsp;
                </td>
                <td width="60%" align="center">
                    &nbsp;
                </td>
                <td width="20%" align="center">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="3" align="left">
                </td>
            </tr>
        </table>
        <div id="mmPrint" align="right" style="width: 100%">
            <input type="button" class="button_top" name="print" value="Print" onclick="javacript:document.all.queryDiv.style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv.style.display='block';document.all.mmPrint.style.visibility='visible'"
                style="font-size: 16; width: 80; height: 26">
            <input type="button" class="button_top" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,'Job Order Transaction Detail Report.htm')"
                style="font-size: 16; width: 80; height: 26">
            <input type="button" class="button_top" name="ToExcel" value="ToExcel" onclick="toPaseExcel()" style="font-size: 16;
                width: 80; height: 26" />
            <input type="button" class="button_top" name="excel" value="To Wps" onclick="ToExcelOfWPS()" />
        </div>
        <br>
        <asp:GridView ID="gvBody" runat="server" Width="99%" AutoGenerateColumns="True"
            CellPadding="4" ForeColor="#333333" GridLines="Vertical">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
    </div>
    </form>
    <p>
&nbsp;&nbsp;&nbsp;
    </p>
</body>
</html>
