<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pcm.aspx.cs" Inherits="Reports_pcm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Garment stock movement summary report</title>
    <link type="text/css" href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Script/Common.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search</legend>
            <table width="100%" id="queryTab" align="center">
                <tr>
                    <td width="75" height="19" class="tdbackcolor">
                        Factory:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlfactoryCd" runat="server" DataTextField="DEPARTMENT_ID"
                            DataValueField="DEPARTMENT_ID">
                        </asp:DropDownList>
                    </td>
                    <td width="75" height="19" class="tdbackcolor">
                        Garment Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlgarmentType" runat="server">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="W">Woven</asp:ListItem>
                            <asp:ListItem Value="K">Knit</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td width="75" height="19" class="tdbackcolor">
                        Wash Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlwashType" runat="server">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="WASH">Wash</asp:ListItem>
                            <asp:ListItem Value="NW">No Wash</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td width="75" height="19" class="tdbackcolor">
                        Year:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlyear" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td width="75" height="19" class="tdbackcolor">
                        Month:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlmonth" runat="server">
                            <asp:ListItem Value="1">January</asp:ListItem>
                            <asp:ListItem Value="2">February</asp:ListItem>
                            <asp:ListItem Value="3">March</asp:ListItem>
                            <asp:ListItem Value="4">April</asp:ListItem>
                            <asp:ListItem Value="5">May</asp:ListItem>
                            <asp:ListItem Value="6">June</asp:ListItem>
                            <asp:ListItem Value="7">July</asp:ListItem>
                            <asp:ListItem Value="8">August</asp:ListItem>
                            <asp:ListItem Value="9">September</asp:ListItem>
                            <asp:ListItem Value="10">October</asp:ListItem>
                            <asp:ListItem Value="11">November</asp:ListItem>
                            <asp:ListItem Value="12">December</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Button ID="btnQuery" Text="Query" CssClass="button_top" runat="server" OnClick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade size="1">
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
                    <font face="Arial" size="4">Garment stock movement summary report(By Month)</font>
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
        <div id="mmPrint" align="right">
            <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv .style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv .style.display='block';document.all.mmPrint.style.visibility='visible'"
                style="font-size: 16; width: 80; height: 26">
            <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,'Garment stock movement summary report.htm')"
                style="font-size: 16; width: 80; height: 26">
            <input type="button" name="ToExcel" value="ToExcel" onclick="toPcmExcel()" style="font-size: 16;
                width: 80; height: 26">
            <input type="button" name="excel" value="To Wps" onclick="ToExcelOfWPS()" style="font-size: 16;
                width: 80; height: 26" />
        </div>
        <br>
        <table border="1" cellspacing="0" cellpadding="0" style="font-size: 12px; border-collapse: collapse">
            <tr>
                <td class="tr2style" bgcolor="#efefe7" width="100">
                    Factory CD:
                </td>
                <td>
                    <%=ddlfactoryCd.SelectedItem.Value %>
                </td>
                <td class="tr2style" bgcolor="#efefe7" width="100">
                    Garment type:
                </td>
                <td>
                    <%=ddlgarmentType.SelectedItem.Value %>
                </td>
                <td class="tr2style" width="100">
                    Wash Type:
                </td>
                <td>
                    <%=ddlwashType.SelectedItem.Value %>
                </td>
                <td class="tr2style" bgcolor="#efefe7" width="100">
                    Year:
                </td>
                <td>
                    <%=ddlyear.SelectedItem.Text %>
                </td>
                <td class="tr2style" bgcolor="#efefe7" width="100">
                    Month:
                </td>
                <td>
                    <%=ddlmonth.SelectedItem.Text %>
                </td>
            </tr>
        </table>
        <br />
        <div id="divBody" runat="server">
            <asp:GridView ID="gvPcmDetail" runat="server" Width="100%" 
                onrowcreated="gvPcmDetail_RowCreated">
            </asp:GridView>
        </div>
    </div>
    </form>
</body>
</html>
