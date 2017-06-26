<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProYMGStatusQuery.aspx.cs"
    Inherits="Mes_ProYMGStatusQuery" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>YMG 生产即时查询</title>
    <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <link rel="Stylesheet" href="../Css/Style.css" />
    <script src="../Script/Common.js" type="text/javascript"></script>
    <script type="text/javascript">
        function ShowDetail(object, factoryCd, strJo, strColor, IsChecked) {
            table = object.parentElement.parentElement.parentElement;
            if (table.rows(1).cells[0].innerHTML == "") {
                if (strColor != "-") {
                    table.rows(1).cells[0].innerHTML = Mes_ProYMGStatusQuery.GetSizeData(factoryCd, strJo, strColor, IsChecked).value;
                }
                else {
                    table.rows(1).cells[0].innerHTML = Mes_ProYMGStatusQuery.GetColorData(factoryCd, strJo, IsChecked).value;
                }
            }
            if (table.rows(1).style.display == "none") {
                table.rows(1).style.display = "block";
                object.innerText = object.innerText.replace('+', '-')
            }
            else {
                table.rows(1).style.display = "none";
                object.innerText = object.innerText.replace('-', '+')
            }
        }
        function toExcel() {

            var myExcel, myBook, xlsColumn, xlsRow;
            try {
                myExcel = new ActiveXObject("Excel.Application");
            } catch (Exception) {
                try {
                    myExcel = new ActiveXObject("ET.Application");
                }
                catch (Exception) {
                    alert("Open Excel Application exception");
                    return;
                }
            }

            if (myExcel != null) {
                window.clipboardData.setData("Text", document.all.ExcTable.outerHTML);
                myBook = myExcel.Workbooks.Add();
                try {
                    myBook.sheets(1).paste;
                } catch (Exception) {
                    myBook.sheets(1).Paste();
                }
                for (xlsColumn = 1, row = 2; ; ) {
                    data = myBook.ActiveSheet.Cells(row, xlsColumn + 1).value;
                    if (data == null) break;
                    xlsColumn++;
                }
                for (xlsRow = 3; ; ) {
                    data = myBook.ActiveSheet.Cells(xlsRow, 2).value;
                    if (data == "TTL") break;
                    xlsRow++;
                }
                for (var i = 4; i <= xlsRow; i++) {
                    var isDelete = true;
                    for (var j = 1; j <= xlsColumn; j++) {
                        data = myBook.ActiveSheet.Cells(i, j).value;
                        if (data != null) {
                            isDelete = false;
                            break;
                        }
                    }
                    if (isDelete) {
                        try {
                            myBook.ActiveSheet.Rows(i).Delete;
                        } catch (Exception) {
                            myBook.ActiveSheet.Rows(i).Delete();
                        }
                        xlsRow = xlsRow - 1;
                        i = i - 1;
                    }
                }
            }
            myExcel.Visible = true;
        }
    </script>
    <style type="text/css">
        .body
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
            width: 100%;
            border: 0;
            cellpadding: 0;
            cellspacing: 0;
            border-collapse: collapse;
        }
        td
        {
            word-break: break-all;
        }
    </style>
</head>
<body topmargin="0" style="font-size: 12px" leftmargin="0" marginwidth="0" marginheight="0">
    <form id="form1" runat="server">
    <div>
        <table width="100%" bgcolor="#c7dff1">
            <tr>
                <td align="center">
                    <h3>
                        <asp:Literal runat="server" ID="Literal6" Text="<%$Resources:GlobalResources,STRING_PRODUCTION_YMG_INQUERY %>"></asp:Literal></h3>
                </td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <table style="width: 800px">
                        <tr>
                            <td>
                                <asp:Literal runat="server" ID="Literal1" Text="<%$Resources:GlobalResources,STRING_JO_NO %>"></asp:Literal>:
                            </td>
                            <td>
                                <asp:TextBox ID="txtJo" runat="server"></asp:TextBox>&nbsp;<asp:CheckBox ID="chbJo"
                                    runat="server" Text="<%$Resources:GlobalResources,STRING_FUZZY %>" />
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal2" Text="<%$Resources:GlobalResources,STRING_GO_NO %>"></asp:Literal>:
                            </td>
                            <td>
                                <asp:TextBox ID="txtGo" runat="server"></asp:TextBox>&nbsp;<asp:CheckBox ID="chbGo"
                                    runat="server" Text="<%$Resources:GlobalResources,STRING_FUZZY %>" />
                            </td>
                            <td>
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
                                    <asp:ListItem Value="BPO" Selected="True">BPO Date</asp:ListItem>
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
                                <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button1" OnClick="btnQuery_Click" />&nbsp;&nbsp;
                                <input type="button" class="button1" value="To Excel" onclick="toExcel()" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <hr />
        <br />
        <table width="100%" id="ExcTable">
            <tr>
                <td align="center">
                    <asp:Literal runat="server" ID="Literal26" Text="<%$Resources:GlobalResources,STRING_TRANSFER_STATUS_AREA%>"></asp:Literal>
                    <div id="divTransInfo">
                    </div>
                    <%
                        for (int i = 0; i < strHtmlList.Count; i++)
                        {
                            Response.Write(strHtmlList[i]);
                            if (i % 20000 == 0)
                            {
                                Response.Flush();
                            }
                        }
                    %>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
