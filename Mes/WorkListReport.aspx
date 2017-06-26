<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WorkListReport.aspx.cs" Inherits="WorkListReport" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>WIPReport</title>
    <link rel="stylesheet" type="text/css" href="../StyleSheet.css" />

    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>

    <style type="text/css">
        .style1
        {
            height: 8px;
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
    <div id="divHeader" style="width: 100%; margin-right: 2px;" title="Search">
        <table id="tbHeader" border="0">
            <tr>
            <td height="19" class="tdbackcolor">
                        Factory:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFtyCd" runat="server" Enabled="False">
                        </asp:DropDownList>
                    </td>
                <td class="style1" align="left">
                    From Date:&nbsp;&nbsp;
                    <asp:TextBox ID="txtBeginDate" runat="server" Width="142px" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>&nbsp;&nbsp;To
                    Date:&nbsp;&nbsp;
                    <asp:TextBox ID="txtEndDate" runat="server" Width="142px" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    &nbsp;&nbsp;
                    <asp:Button ID="btnQuery" runat="server" CssClass="button" Text="Query" OnClick="btnQuery_Click" />
                    &nbsp;<asp:Button ID="btnExcel" runat="server" CssClass="button" Text="To Excel" OnClick="btnExcel_Click" />
                    &nbsp;<input type="button" value="To WPS" onclick="ToExcelOfWPS()" class="button" />
                </td>
            </tr>
            <tr>
                <td align="right" style="height: 26px">
                    &nbsp;
                </td>
            </tr>
        </table>
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
    <br />
    <div id="ExcTable" style="width: 100%;" runat="server">
        <%--报表明细--%>
        <asp:GridView ID="gvBody" runat="server" AutoGenerateColumns="true" CellPadding="4"
            EnableViewState="false" ForeColor="#333333" GridLines="Both" OnRowDataBound="gvBody_RowDataBound">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" BorderWidth="1px" BorderStyle="Solid" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Font-Size="10" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <RowStyle Font-Size="8" />
        </asp:GridView>
    </div>
    </form>
</body>
</html>
