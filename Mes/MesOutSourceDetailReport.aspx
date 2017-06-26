<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MesOutSourceDetailReport.aspx.cs"
    Inherits="Mes_MasOutSourcePriceReports" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../StyleSheet.css" type="text/css" rel="Stylesheet" />
    <style type="text/css">
        th
        {
            background-color: Silver;
        }
    </style>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        function Show() {
            switch (document.getElementById("hfType").value) {
                case "F":
                    var value = window.showModalDialog("Search.aspx?Type=F", "", "dialogWidth=800px;");
                    if (value != undefined) {
                        if (value != "") {
                            document.getElementById("txtFactoryName").value = value;
                        }
                    }
                    break;
                case "C":
                    var value = window.showModalDialog("Search.aspx?Type=C", "", "dialogWidth=800px;");
                    if (value != undefined) {
                        if (value != "") {
                            document.getElementById("txtCustomer").value = value;
                        }
                    }
                    break;
            }
        }
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
        <asp:HiddenField ID="hfType" runat="server" Value="F" />
        <table>
            <tr>
                <td>
                    <asp:RadioButtonList ID="rblOrderBy" runat="server" RepeatDirection="Horizontal"
                        OnSelectedIndexChanged="rblOrderBy_SelectedIndexChanged" AutoPostBack="True">
                        <asp:ListItem Text="By Factory" Value="F" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="By Customer" Value="C"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                Date：
                            </td>
                            <td>
                                <asp:TextBox ID="txtDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                            </td>
                            <td>
                                Type：
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlType" runat="server">
                                    <asp:ListItem Value="K">Knit</asp:ListItem>
                                    <asp:ListItem Value="W">Woven</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="lblFactory" runat="server" Text="Factory Name："></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtFactoryName" runat="server"></asp:TextBox>
                            </td>
                            <td>
                            </td>
                            <td>
                                <asp:Label ID="lblCustomer" runat="server" Text="Customer Name：" Visible="False"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtCustomer" runat="server" Visible="False"></asp:TextBox>
                            </td>
                            <td>
                                <input type="button" class="button" value="..." style="width: 30px" runat="server"
                                    id="btnShow" onclick="Show()" />
                            </td>
                            <td>
                                <asp:Button ID="btnEnter" runat="server" Text="Query" CssClass="button" OnClick="btnEnter_Click" />
                            </td>
                            <td>
                                <input type="button" class="button" onclick="ToExcelOfWPS()" value="To WPS" style="width: 70px" />
                            </td>
                            <td>
                                <asp:Button ID="btnExcel" runat="server" CssClass="button" Text="To Excel" OnClick="btnExcel_Click"
                                    Style="width: 70px" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="ExcTable" style="width: 100%;" runat="server">
                        <asp:GridView ID="gvBody" runat="server" OnRowCreated="gvBody_RowCreated" Width="100%"
                            OnRowDataBound="gvBody_RowDataBound" EnableViewState="False">
                        </asp:GridView>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
