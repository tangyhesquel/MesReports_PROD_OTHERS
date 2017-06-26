<%@ Page Language="C#" AutoEventWireup="true" CodeFile="proPullout.aspx.cs" Inherits="Reports_proPullout" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Production Pullout/Discrepancy Summary Report</title>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link rel="Stylesheet" href="../Css/StyleReport.css" />
    <script type="text/javascript" src="../Script/Common.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search</legend>
            <table width="100%" id="queryTab" align="center">
                <tr>
                    <td width="15%" height="19" class="tdbackcolor">
                        Factory Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFactoryCd" runat="server" DataTextField="DEPARTMENT_ID"
                            DataValueField="DEPARTMENT_ID" AutoPostBack="True" Enabled="false">
                        </asp:DropDownList>
                    </td>
                    <td width="10%" height="19" class="tdbackcolor">
                        Garment Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlGarmentType" runat="server" OnSelectedIndexChanged="ddlGarmentType_SelectedIndexChanged"
                            AutoPostBack="True">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="W">Woven</asp:ListItem>
                            <asp:ListItem Value="K">Knit</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td width="15%" height="19" class="tdbackcolor">
                        Process Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProcessCode" runat="server" DataTextField="SHORT_NAME" DataValueField="PROCESS_CD">
                            <asp:ListItem Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td width="15%" height="19" class="tdbackcolor">
                        Process Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProcessType" runat="server">
                            <asp:ListItem ></asp:ListItem>
                    
                        </asp:DropDownList>
                    </td>
                    <td width="15%" height="19" class="tdbackcolor">
                        Production Factory:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProdFactory" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td height="19" class="tdbackcolor">
                        Group Name
                    </td>
                    <td>
                        <asp:DropDownList ID="ddGroupName" runat="server" Enabled="false">
                            <asp:ListItem Value="">&nbsp;</asp:ListItem>
                            <asp:ListItem Value="YM梭织车缝1">梭织车缝1(W17 - W29)</asp:ListItem>
                            <asp:ListItem Value="YM梭织车缝2">梭织车缝2(W11 - W16)</asp:ListItem>
                            <asp:ListItem Value="YM梭织车缝3">梭织车缝3(W1 - W10)</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td width="15%" height="19" class="tdbackcolor">
                        Date From:
                    </td>
                    <td>
                        <asp:TextBox ID="txtStartDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"
                            Width="75px"></asp:TextBox>
                    </td>
                    <td width="15%" height="19" class="tdbackcolor">
                        Date To:
                    </td>
                    <td>
                        <asp:TextBox ID="txtEndDate" runat="server" onFocus="WdatePicker({skin:'whyGreen'})"
                            Width="75px"></asp:TextBox>
                    </td>
                    <td width="15%" height="19" class="tdbackcolor">
                        Job Order No:
                    </td>
                    <td>
                        <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                    </td>
                    <td colspan="2" align="right">
                        <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade size="1">
    </div>
    <div id="ExcTable">
        <table width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="Red" Text="Tip Message!"
                        Visible="False" Width="300px"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <div id="title" runat="server">
                        <h2>
                            Production Pullout Summary Report</h2>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="3" align="left">
                </td>
            </tr>
            <tr>
                <td>
                    <div id="mmPrint" align="right">
                        <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv.style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv.style.display='block';document.all.mmPrint.style.visibility='visible'"
                            style="font-size: 16; width: 80; height: 26"/>
                        <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,'Production Pullout Summary Report.htm')"
                            style="font-size: 16; width: 80; height: 26"/>
                        <input type="button" name="ToExcel" value="ToExcel" onclick="toPaseExcel()" style="font-size: 16;
                            width: 80; height: 26"/>
                        <input type="button" name="ToWps" value="ToWps" onclick="ToExcelOfWPS()" style="font-size: 16;
                            width: 80; height: 26" />
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                        border-collapse: collapse">
                        <tr>
                            <td class="tr2style" bgcolor="#efefe7" width="15%">
                                Factory Code:
                            </td>
                            <td>
                                <%=ddlFactoryCd.SelectedItem.Text %>
                            </td>
                            <td class="tr2style" bgcolor="#efefe7" width="15%">
                                From Date:
                            </td>
                            <td>
                                <%=txtStartDate.Text.Trim()%>&nbsp;
                            </td>
                            <td class="tr2style" bgcolor="#efefe7" width="15%">
                                To Date:
                            </td>
                            <td>
                                <%=txtEndDate.Text.Trim()%>&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="tr2style" bgcolor="#efefe7" width="15%">
                                Process Code:
                            </td>
                            <td>
                                <%=ddlProcessCode.SelectedItem.Text %>
                            </td>
                            <td class="tr2style" bgcolor="#efefe7" width="15%">
                                Garment Type:
                            </td>
                            <td>
                                <%=ddlGarmentType.SelectedItem.Text %>&nbsp;
                            </td>
                            <td class="tr2style" bgcolor="#efefe7" width="15%">
                                Job Order No:
                            </td>
                            <td>
                                <%=txtJoNo.Text.Trim() %>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divDetail" runat="server">
                        <table id="allTable" width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                            border-collapse: collapse">
                            <tr>
                            </tr>
                        </table>
                        <asp:GridView ID="gvDiscData" runat="server" Visible="false" Width="100%">
                        </asp:GridView>
                    </div>
                </td>
            </tr>
        </table>
        <div id="divMsg" runat="server">
        </div>
    </div>
    </form>
</body>
</html>
