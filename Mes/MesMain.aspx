<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MesMain.aspx.cs" Inherits="MES_MesMain" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
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
                sel.moveToElementText(reportDiv);
                sel.select();
                sel.execCommand("Copy");
                sel.execCommand("Unselect");
                myExcel.Visible = true;
                myBook = myExcel.Workbooks.Add();
                myBook.sheets("Sheet1").Paste();
            }

        }
        function ToExcel() {
            var myExcel, myBook;
            try {
                myExcel = new ActiveXObject("Excel.Application");
            } catch (Exception) {
                alert("Open Excel Application exception");
            }
            if (myExcel != null) {
                window.clipboardData.setData("Text", document.all.reportDiv.outerHTML);
                myExcel.Visible = true;
                myBook = myExcel.Workbooks.Add();
                myBook.sheets(1).paste;
                myBook.sheets(1).Rows.AutoFit();
                myBook.sheets(1).Columns.AutoFit();
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
                        <asp:ListItem Selected="True" Value="A">All</asp:ListItem>
                        <asp:ListItem Text="By Factory" Value="F"></asp:ListItem>
                        <asp:ListItem Text="By Customer" Value="C"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td style="color: Red; font-size: larger">
                    <asp:Label Visible="false" ID="warning" runat="server"></asp:Label>
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
                                <asp:CheckBox ID="cbInclude" runat="server" Text="Include ShirtStop" />
                            </td>
                            <td>
                                <asp:Label ID="lblFactory" runat="server" Text="Factory Name："></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtFactoryName" runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="lblCustomer" runat="server" Text="Customer Name：" Visible="False"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtCustomer" runat="server" Visible="False"></asp:TextBox>
                            </td>
                            <td>
                                <input type="button" class="BUTTON" value="..." style="width: 20px" runat="server"
                                    id="btnShow" onclick="Show()" />
                            </td>
                            <td>
                                <asp:Button ID="btnEnter" runat="server" Text="Query" CssClass="BUTTON" OnClick="btnEnter_Click" />
                            </td>
                            <td>
                                <input type="button" class="BUTTON" onclick="ToExcelOfWPS()" value="To WPS" />
                            </td>
                            <td>
                                <%--<input type="button" class="BUTTON" onclick="ToExcel()" value="To Excel" />--%>
                                <asp:Button ID="btnExcel" Text="To Excel" runat="server" CssClass="BUTTON" OnClick="btnExcel_Click" />
                            </td>
                            <td>
                                <asp:Button ID="btnToDetail" Text="DownLoad Detail To Excel" runat="server" CssClass="BUTTON"
                                    OnClick="btnToDetail_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div id="reportDiv" runat="server">
    </div>
    <div>
        <table style="font-size: 12px">
            <tr>
                <td>
                    Remarks:
                </td>
            </tr>
            <tr>
                <td>
                    1. Month(Contract Expect Received Date): 截止到指定日期，当月的平均单价/SAH/平均SAH单价
                </td>
            </tr>
            <tr>
                <td>
                    2. YTD(Contract Expect Received Date): 截止到指定日期，当年的平均单价/SAH/平均SAH单价
                </td>
            </tr>
            <tr>
                <td>
                    3. Budget: 当年的预算的平均单价/SAH/平均SAH单价
                </td>
            </tr>
            <tr>
                <td>
                    4. Standard: 截止到指定日期，已经外发的订单的当年的平均单价/SAH/平均SAH单价
                </td>
            </tr>
            <tr>
                <td>
                    5. Month(Actual Return Date):截止到指定日期，已经回货的订单，当月的平均单价/SAH/平均SAH单价
                </td>
            </tr>
            <tr>
                <td>
                    6. YTD(Actual Return Date):截止到指定日期，已经回货的订单，当年的平均单价/SAH/平均SAH单价
                </td>
            </tr>
            <tr>
                <td>
                    7. YTD Year(年份):(前一年)全年的实际回货平均单价/SAH/平均SAH单价
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
