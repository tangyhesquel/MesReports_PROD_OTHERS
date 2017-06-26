<%@ Page Language="C#" AutoEventWireup="true" CodeFile="JOBasicReport.aspx.cs" Inherits="Reports_JOSAHReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Cutting Date Report</title>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        BODY
        {
            font-size: 11px;
            padding: 5px;
            font-family: Arial,Times New Roman, 黑体,Verdana,Helvetica, sans-serif;
        }
        A:link
        {
            color: #000000;
            text-decoration: none;
        }
        A:active
        {
            color: #000000;
            text-decoration: none;
        }
        A:visited
        {
            color: #000000;
            text-decoration: none;
        }
        A:hover
        {
            color: #000000;
            text-decoration: none;
        }
        A:hover
        {
            left: 1px;
            border-bottom: 1px dotted;
            position: relative;
            top: 1px;
        }
        .BUTTON
        {
            font-weight: bolder;
            font-size: 11px;
            padding: 1px;
            padding-bottom: 2px;
            text-transform: uppercase;
            cursor: hand;
            height: 20px;
            color: #646400;
            background-color: #CCCC99;
            border: 1px solid #333333;
        }
        INPUT
        {
            padding-left: 2px;
            font-size: 11px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            height: 18px;
        }
        TABLE
        {
            padding-right: 0px;
            padding-left: 0px;
            font-size: 11px;
            padding-bottom: 0px;
            padding-top: 0px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        .tr2style
        {
            font-weight: bolder;
            font-size: 11px;
            background-color: #efefe7;
            padding-left: 5px;
            padding-right: 5px;
            padding-top: 2px;
        }
        .tr3style
        {
            font-weight: bolder;
            font-size: 11px;
            background-color: #ffffff;
            padding-left: 5px;
            padding-top: 2px;
        }
        .tdbackcolor
        {
            background-color: #EFEFE0;
        }
        .button_top
        {
            font-family: "Arial" , "Helvetica" , "sans-serif";
            font-size: 14px;
            background-color: #CCCC99;
            border: ridge;
            font-weight: bold;
            border-width: 1px 1px 2px;
            border-color: #000000 #f2f2f2 #f2f2f2 #000000;
            color: #333366;
            text-transform: uppercase;
            cursor: hand;
        }
    </style>
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script language="javascript" type="text/javascript">
        function toPaseExcel() {

            var myExcel, myBook;
            try {
                myExcel = new ActiveXObject("Excel.Application");
            } catch (Exception) {
                alert("Open Excel Application exception");
            }

            if (myExcel != null) {
                window.clipboardData.setData("Text", document.all.ExcTable.outerHTML);
                myExcel.Visible = true;
                myBook = myExcel.Workbooks.Add();
                myBook.sheets(1).paste();

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
        function searchJo() {
            var urlName = "searchJO.aspx?factory=" + document.all.ddlFactory.value + "&userRandom=" + (Math.random() * 100000);
            var jo = window.showModalDialog(urlName, "Job Order No.", "dialogWidth=650pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (jo == null) return;
            document.all.txtJoNo.value = jo;
        }
        function init() {
            if (document.all.txtStartDate.value == '') {
                //                 document.all.ExcTable.style.display = 'none';
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <table border="0" align="center" width="100%">
        <tr>
            <td>
                <div id="conditiondiv">
                    <fieldset>
                        <legend>Search</legend>
                        <table width="100%" id="queryTab">
                            <tr>
                                <td class="tdbackcolor">
                                    Factory :
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlFactory" runat="server" Enabled="False">
                                    </asp:DropDownList>
                                </td>
                                <td class="tdbackcolor">
                                    JO No.:
                                </td>
                                <td class="style2">
                                    <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                                    <input type="button" value="..." onclick="searchJo()" class="button_top" />
                                </td>
                                <td class="tdbackcolor">
                                    GO No:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtGONo" runat="server"></asp:TextBox>
                                </td>
                                <td height="19" class="tdbackcolor">
                                    From Date:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtStartDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                                </td>
                                <td height="19" class="tdbackcolor">
                                    End Date:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtToDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                                </td>
                                <td class="tdbackcolor">
                                    <asp:CheckBox ID="cbNoCut" runat="server" Text="No Cut" />
                                </td>
                                <td>
                                    <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                                </td>
                                <td>
                                    <input type="button" name="reSet" value="Reset" onclick="reset()" style="height: 26"
                                        class="button_top" />
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
                <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="Red" Text="Tip Message!"
                    Visible="False" Width="300px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <div id="mmPrint" align="right">
                    <input type="button" name="print3" value="print" style="font-size: 16; width: 80;
                        height: 26" onclick="mmPrint.style.display='none';conditiondiv.style.display='none';WebBrowser1.ExecWB(6,1);mmPrint.style.display='';conditiondiv.style.display=''" />
                    <input type="button" name="print2" value="printview" style="font-size: 16; width: 80;
                        height: 26" onclick="mmPrint.style.display='none';conditiondiv.style.display='none';WebBrowser1.ExecWB(7,1);mmPrint.style.display='';conditiondiv.style.display=''" />
                    <input type="button" name="print" value="page setup" style="font-size: 16; width: 100;
                        height: 26" onclick="mmPrint.style.display='none';conditiondiv.style.display='none';WebBrowser1.ExecWB(8,1);mmPrint.style.display='';conditiondiv.style.display=''" />
                    <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" />
                    <input type="button" name="excel" value="To Wps" onclick="ToExcelOfWPS()" />
                </div>
            </td>
        </tr>
    </table>
    <div id="ExcTable" runat="server">
        <asp:GridView ID="gvBody" runat="server" Width="99%" AutoGenerateColumns="False"
            CellPadding="4" ForeColor="#333333" GridLines="Vertical">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <Columns>
                <asp:BoundField DataField="SHORT_NAME" HeaderText="Buyer" ItemStyle-Wrap="false">
                    <ItemStyle Wrap="False"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="JO_NO" HeaderText="JO NO." />
                <asp:BoundField DataField="SC_NO" HeaderText="GO NO." />
                <asp:BoundField DataField="SAMPLE_ORDER" HeaderText="FDS" />
                <asp:BoundField DataField="STYLE_NO" HeaderText="Style_No" ItemStyle-Wrap="false">
                    <ItemStyle Wrap="False"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="STYLE_DESC" HeaderText="Style_Desc" ItemStyle-Wrap="false">
                    <ItemStyle Wrap="False"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="SAM_SO_NO" HeaderText="SAM" />
                <asp:BoundField DataField="SAH" HeaderText="SAH" />
                <asp:BoundField DataField="BUYER_PO_DEL_DATE" HeaderText="DEL.Date" ItemStyle-Wrap="false">
                    <ItemStyle Wrap="False"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="PROD_COMPLETION_DATE" HeaderText="PPOD.Date" ItemStyle-Wrap="false">
                    <ItemStyle Wrap="False"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Order_QTY" HeaderText="Order_QTY" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="shipallowed" HeaderText="Ship Allowed" ItemStyle-Wrap="false">
                    <ItemStyle Wrap="False"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="MARKET_CD" HeaderText="MARKET_CD" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="SHIP_MODE_CD" HeaderText="SHIP_MODE" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="GARMENT_TYPE_CD" HeaderText="GARMENT_TYPE" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="CUST_STYLE_NO" HeaderText="CUST_STYLE" ItemStyle-HorizontalAlign="Right">
                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="PRINTING" HeaderText="PRINTING" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="EMBROIDERY" HeaderText="EMBROIDERY" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="WASH_TYPE_CD" HeaderText="WASH_TYPE" ItemStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="STYLE_CHN_DESC" HeaderText="STYLE_CHN_DESC" ItemStyle-Wrap="false">
                    <ItemStyle Wrap="False"></ItemStyle>
                </asp:BoundField>
            </Columns>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
    </div>
    <div id="dvMsg" runat="server" visible="false">
        <table id="TMsg" width="100%">
            <tr style="color: Red; font-size: large">
                <td align="center">
                    No Data!
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
