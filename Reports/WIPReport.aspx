<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WIPReport.aspx.cs" Inherits="WIPReport" %>

<%@ Import Namespace="System.Data" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>WIPReport</title>
    <link rel="stylesheet" type="text/css" href="../StyleSheet.css" />
    <link rel="Stylesheet" href="../Css/StyleReport.css" />
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script language="javascript" type="text/javascript">
        /* function ToExcelOfWPS() {
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

        }*/
    </script>
    <style type="text/css">
        .style1
        {
            background-color: #EFEFE0;
            width: 34.6em;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="divHeader" style="width: 100%; margin-right: 2px;" title="Search">
        <fieldset>
            <legend>Search</legend>
            <table id="tbHeader" border="0">
                <tr>
                    <td class="tdbackcolor" align="left" style="width: 32em">
                        From Date:
                    </td>
                    <td class="tdbackcolor">
                        <asp:TextBox ID="txtBeginDate" runat="server" Width="100px" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td class="tdbackcolor" style="width: 32em">
                        To Date:
                    </td>
                    <td class="tdbackcolor">
                        <asp:TextBox ID="txtEndDate" runat="server" Width="96px" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td class="tdbackcolor" style="width: 32em">
                        Garment Type:
                    </td>
                    <td class="tdbackcolor">
                        <asp:DropDownList ID="ddlGarmentType" runat="server" AutoPostBack="True" 
                            onselectedindexchanged="ddlGarmentType_SelectedIndexChanged">
                            <asp:ListItem>ALL</asp:ListItem>
                            <asp:ListItem>K</asp:ListItem>
                            <asp:ListItem>W</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="tdbackcolor" style="width: 32em">
                        Process CD:
                    </td>
                    <td class="tdbackcolor">
                        <asp:DropDownList ID="ddlProcessCd" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td class="tdbackcolor" style="width: 32em">
                        Process Type:
                    </td>
                    <td class="tdbackcolor">
                        <asp:DropDownList ID="ddlProcessType" runat="server">
                            <asp:ListItem></asp:ListItem>
                            
                        </asp:DropDownList>
                    </td>
                    <td class="tdbackcolor" style="width: 32em">
                        Production Factory:
                    </td>
                    <td class="tdbackcolor">
                        <asp:DropDownList ID="ddlProdFactory" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td class="tdbackcolor" style="width: 64em">
                        <asp:CheckBox ID="CbChoose" runat="server" Text="Include End Process" />
                    </td>
                    <td class="tdbackcolor" style="width: 64em">
                        <asp:CheckBox ID="CbByDaily" runat="server" Text="By Daily Table" />
                    </td>
                    <td class="style1" runat="server">
                        Group Name:
                    </td>
                    <td class="tdbackcolor" runat="server">
                        <asp:DropDownList ID="ddlGroupName" runat="server" Enabled="false">
                            <asp:ListItem Value="">&nbsp;</asp:ListItem>
                            <asp:ListItem Value="YM梭织车缝1">梭织车缝1(W17 - W29)</asp:ListItem>
                            <asp:ListItem Value="YM梭织车缝2">梭织车缝2(W11 - W16)</asp:ListItem>
                            <asp:ListItem Value="YM梭织车缝3">梭织车缝3(W1 - W10)</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td align="center" class="tdbackcolor" style="width: 32em">
                        <asp:Button ID="btnQuery" runat="server" CssClass="button" Text="Query" OnClick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="Red" Text="Tip Message!"
        Visible="False" Width="300px"></asp:Label>
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr align="center">
            <td align="center">
                <asp:Label ID="ReportTitle" runat="server" Style="font-size: x-large; font-weight: bold;
                    text-align: center;"><%=PageHeader%></asp:Label>
            </td>
        </tr>
    </table>
    <table width="100%">
        <tr>
            <td align="right">
                <input type="button" style="width: 5em" value="To Excel" onclick="toPaseExcel()"
                    class="button" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input type="button" style="width: 5em" value="To WPS" onclick="ToExcelOfWPS()" class="button" />
            </td>
        </tr>
    </table>
    <br />
    <div id="ExcTable" style="width: 100%;" runat="server">
        <%--报表明细--%>
        <table style="width: 100%;">
            <tr>
                <td>
                    <asp:GridView ID="gvBody" runat="server" Width="100%" AutoGenerateColumns="true"
                        CellPadding="4" EnableViewState="false" ForeColor="#333333" GridLines="Both"
                        OnRowDataBound="gvBody_RowDataBound" ShowFooter="True">
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" BorderWidth="1px" BorderStyle="Solid" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Font-Size="10" />
                        <EditRowStyle BackColor="#999999" />
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <RowStyle Font-Size="8" />
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <div id="msgTable" runat="server" visible="false">
        <table width="100%">
            <tr>
                <td align="center" style="color: Red; font-size: large; font-weight: bold">
                    No Data
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
