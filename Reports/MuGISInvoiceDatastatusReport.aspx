<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MuGISInvoiceDatastatusReport.aspx.cs"
    Inherits="Reports_GEGMUReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>MU GIS Invoice Data status Report</title>
    <script src="../My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../Script/Common.js"></script>
    <style type="text/css">
        body
        {
            font-size: 8.5pt;
            font-family: Tahoma,Verdana,MS Sans Serif,Courier New;
        }
        table
        {
            font-size: 8.5pt;
            font-family: Tahoma,Verdana,MS Sans Serif,Courier New;
        }
        .style2
        {
            color: #000000;
        }
        BODY Td
        {
            font-size: 12;
            height: 25;
            color: #000000;
        }
        SELECT
        {
            font-size: 8pt;
            font-family: 'MS Sans Serif' ,Arial,Tahoma;
            background-color: #efefe7;
        }
        INPUT
        {
            font-size: 9pt;
            font-family: Tahoma, Verdana, Arial, Helvetica;
            height: 14pt;
        }
        
        .Input1
        {
            font-size: 8pt;
            color: #000000;
            width: 120;
            height: 10pt;
        }
    </style>
    <script type="text/javascript" language="javascript">
        function Unlock(RunNo, fty) {
            if (RunNo.toString() == "") {
                return false;
            }
            //alert (RunNo.toString());
            if (confirm(" Do You Confirm Unlock the Data？\n System will Clear it after 1 day once unlock it.\n You can re Lock it before the new query in current page.")) {
                window.showModalDialog("SaveFactoryMUReport.aspx?RunNo=" + RunNo.toString() + "&FtyCD=" + fty + "&Utype=N", window, "dialogWidth=350px;dialogHeight=80px");
                return true;

            }
            else {
                return false;
            }

        }
        function Lock(RunNo, fty) {
            if (RunNo.toString() == "") {
                return false;
            }
            if (confirm(" Do You Confirm Lock the Data？\n System will keep the data and \n not capture & generate form other system once lock it.")) {
                window.showModalDialog("SaveFactoryMUReport.aspx?RunNo=" + RunNo.toString() + "&FtyCD=" + fty + "&Utype=Y", window, "dialogWidth=350px;dialogHeight=80px");
                return true;

            }
            else {
                return false;
            }

        }
        function Log() {

            window.showModalDialog("SaveFactoryMUReport.aspx?RunNo=''&FtyCD=''&Utype=M", window, "dialogWidth=450px;dialogHeight=300px");
            return true;
        }
    </script>
</head>
<body style="font-size: smaller">
    <object id="WebBrowser1" width="0" height="0" classid="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2">
    </object>
    <form id="form1" runat="server">
    <div>
        <div id="TabHead">
            <table width="100%" bgcolor="#efefe7">
                <tr>
                    <td>
                        <span class="style2">Factory Code:</span>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFtyCd" runat="server" Enabled="False">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <span class="style2">JO NO:</span>
                    </td>
                    <td>
                        <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                    </td>
                    <%--<td>
                    <span class="style2">Garment Type</span>
                </td>
                <td>
                    <asp:DropDownList ID="ddlGarmentType" runat="server">
                        <asp:ListItem Value="">All</asp:ListItem>
                        <asp:ListItem Value="K">Knit</asp:ListItem>
                        <asp:ListItem Value="W">Woven</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    <span class="style2">OutSource Type</span>
                </td>
                <td>
                    <asp:DropDownList ID="ddlOutSource" runat="server">
                        <asp:ListItem Value="">All</asp:ListItem>
                        <asp:ListItem Value="STANDARD">STANDARD</asp:ListItem>
                        <asp:ListItem Value="OUTSOURCE">OUTSOURCE</asp:ListItem>
                    </asp:DropDownList>
                </td>--%>
                    <td>
                        <span class="style2">Ship Date From :</span>
                    </td>
                    <td>
                        <asp:TextBox ID="txtFromDate" runat="server" onfocus="WdatePicker({skin:'default'})"></asp:TextBox>
                    </td>
                    <td>
                        <span class="style2">Ship Date To:</span>
                    </td>
                    <td>
                        <asp:TextBox ID="txtToDate" runat="server" onfocus="WdatePicker({skin:'default'})"></asp:TextBox>
                    </td>
                    <%--         <td>
                    <asp:CheckBox runat="server" ID="cbChecked" 
                        Text="Include the Detail Report" Checked="true" 
                        CssClass="Input1" Visible="False"/>
                </td>
                <td>
                      <asp:CheckBox runat="server" ID="cbShipJo" CssClass="Input1" Text="Include All Ship JO" 
                        Checked="true" />
                </td>
                <td>
                    <asp:CheckBox runat="server" ID="cbNoPost" CssClass="Input1"  Text="Include No Post JO " 
                        Checked="false" Visible="False"  />
                </td>--%>
                    <td>
                        <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnQuery_Click" CssClass="button_top"
                            BackColor="#c2defa" Font-Names="Arial" Font-Size="12px" BorderColor="#6FE7FF"
                            ForeColor="#CC3300" BorderStyle="Solid" BorderWidth="1px" Height="20" Width="60"
                            Font-Bold="True" />
                    </td>
                    <%--<td><input type="button" name="Help" value="Help" style="border: 1px solid #6FE7FF; text-align: left; width: 40px; font-family: Arial; font-size: 12px; color: #CC3300; background-color: #efefe7;" onclick="window.open('MUHelp.htm')"/>
                </td>
                <td><input type="button" name="btnLog" value="Log" style="border: 1px solid #6FE7FF; text-align: left; width: 30px; font-family: Arial; font-size: 12px; color: #000000; background-color: #efefe7;" onclick="Log()"/>
                </td>--%>
                </tr>
            </table>
        </div>
        <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="Red" Text="Tip Message!"
            Visible="False" Width="300px"></asp:Label>
        <br />
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
                    <td align="center" colspan="10">
                        <font face="Arial" size="4">MU GIS Invoice Data status Report</font>
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
                <input name="btnPrint" type="button" value="   Print   " style="border: 1px solid #6FE7FF;
                    text-align: left; width: 80px; font-family: Arial; font-size: 12px; color: #000000;
                    background-color: #efefe7;" onclick="mmPrint.style.display='none';TabHead.style.display='none';WebBrowser1.ExecWB(6,1);TabHead.style.display='';mmPrint.style.display=''" />
                <input name="btnPreview" type="button" value="Preview" style="border: 1px solid #6FE7FF;
                    text-align: left; width: 80px; font-family: Arial; font-size: 12px; color: #000000;
                    background-color: #efefe7;" onclick="mmPrint.style.display='none';TabHead.style.display='none';WebBrowser1.ExecWB(7,1);TabHead.style.display='';mmPrint.style.display=''" />
                <%--<input name="btnPageSetup" type="button" value="Page Setup" style="border: 1px solid #6FE7FF; text-align: left; width: 80px; font-family: Arial; font-size: 12px; color: #000000; background-color: #efefe7;" onclick="mmPrint.style.display='none';TabHead.style.display='none';WebBrowser1.ExecWB(8,1);TabHead.style.display='';mmPrint.style.display=''" />--%>
                <input name="btnToExcel" type="button" value="ToExcel" style="border: 1px solid #6FE7FF;
                    text-align: left; width: 80px; font-family: Arial; font-size: 12px; color: #000000;
                    background-color: #efefe7;" onclick="toPaseExcel()" />
                <input type="button" name="ToWps" value="ToWps" style="border: 1px solid #6FE7FF;
                    text-align: left; width: 80px; font-family: Arial; font-size: 12px; color: #000000;
                    background-color: #efefe7;" onclick="ToExcelOfWPS()" />
                <%--<input type="button" name="btnSave" value="Lock" style="border: 1px solid #6FE7FF; text-align: left; width: 80px; font-family: Arial; font-size: 12px; color: #CC3300; background-color: #efefe7;" onclick="Lock('<%=strRunNO%>','<%=ddlFtyCd.SelectedItem.Text%>')"/>
                     <input type="button" name="UnLock" value="UnLock" style="border: 1px solid #6FE7FF; text-align: left; width: 80px; font-family: Arial; font-size: 12px; color: #CC3300; background-color: #efefe7;" onclick="Unlock('<%=strRunNO%>','<%=ddlFtyCd.SelectedItem.Text%>')"/>--%>
            </div>
            <br />
            <div id="div_all" runat="server">
                <table id="allTable" width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                    border-collapse: collapse">
                    <tr>
                        <td>
                            <table width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                                border-collapse: collapse">
                                <tr>
                                    <td class="tr2style" bgcolor="#efefe7" width="100">
                                        Factory CD:
                                    </td>
                                    <td>
                                        <%=ddlFtyCd.SelectedItem.Text%>&nbsp;
                                    </td>
                                    <%--<td class="tr2style" bgcolor="#efefe7" width="100">
                                    Garment Type:
                                </td>
                                <td>
                                    <%=ddlGarmentType.SelectedItem.Text%>&nbsp;
                                </td>
                                <td class="tr2style" bgcolor="#efefe7" width="100">
                                    JO Type:
                                </td>
                                <td>
                                    <%=ddlOutSource.SelectedItem.Text%>&nbsp;
                                </td>--%>
                                    <td class="tr2style" bgcolor="#efefe7" width="100">
                                        Start Date:
                                    </td>
                                    <td>
                                        <%=txtFromDate.Text%>&nbsp;
                                    </td>
                                    <td class="tr2style" bgcolor="#efefe7" width="100">
                                        End Date:
                                    </td>
                                    <td>
                                        <%=txtToDate.Text%>&nbsp;
                                    </td>
                                    <td class="tr2style" bgcolor="#efefe7" width="100">
                                        Print Date:
                                    </td>
                                    <td>
                                        <asp:Label ID="txtPrintDate" runat="server"></asp:Label>
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
                            <div id="divBody" runat="server">
                            </div>
                        </td>
                    </tr>
                </table>
                <asp:GridView ID="gvDetail" runat="server" Width="100%" AutoGenerateColumns="true"
                    ShowHeader="true" ShowFooter="True" OnRowDataBound="gvDetail_RowDataBound">
                    <FooterStyle BackColor="#F7F6F3" />
                    <HeaderStyle BackColor="#F7F6F3" />
                </asp:GridView>
                <br />
            </div>
            <div id="divMsg" runat="server">
                <table style="width: 100%">
                    <tr>
                        <td align="center" style="color: Red; font-size: large">
                            No Data
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
