<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InvoiceReport.aspx.cs" Inherits="Reports_InvoiceReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Invoic QTY & Warehouse Receive Qty & Issue to Customer Qty Compare </title>
      <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
      <script type="text/javascript" src="../Script/Common.js"></script>
     <link href="../Css/StyleReport.css" rel="Stylesheet" />
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
            }
        
        .Input1
        {
            font-size: 8pt;
            color: #000000;
            width: 120;
            height: 10pt;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <fieldset>
            <legend>Search </legend>
            <table width="100%" border="0px">
                <tr>
                    <td class="tdbackcolor">
                        JO NO:
                    </td>
                    <td>
                        <asp:TextBox ID="txtJoNo" runat="server"></asp:TextBox>
                    </td>
                    <td class="tdbackcolor">
                        Factory Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFtyCd" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td class="tdbackcolor">
                        Garment Type
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlGarmentType" runat="server">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="K">Knit</asp:ListItem>
                            <asp:ListItem Value="W">Woven</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                   
                    <td >
                        <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnQuery_Click" CssClass="button_top" />
                          &nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                </tr>
                <tr>
                    <td class="tdbackcolor">
                        Ship Date From:
                    </td>
                    <td>
                        <asp:TextBox ID="txtFromDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td class="tdbackcolor">
                        Ship Date To:
                    </td>
                    <td>
                        <asp:TextBox ID="txtToDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td class="tdbackcolor">
                        SAM GROUP</td>
                         <td >
                         <asp:DropDownList ID="ddlOutSource" runat="server">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="STANDARD">STANDARD</asp:ListItem>
                            <asp:ListItem Value="OUTSOURCE">OUTSOURCE</asp:ListItem>
                         </asp:DropDownList>
                        </td>
                    <td>
                         <input name="btnPrint" type="button" value="   Print   " onclick="toPrint.style.display='none';WebBrowser1.ExecWB(6,1);toPrint.style.display=''" />
                    <input name="btnPreview" type="button" value="Preview" onclick="toPrint.style.display='none';WebBrowser1.ExecWB(7,1);toPrint.style.display=''" />
                    <input name="btnPageSetup" type="button" value="Page Setup" onclick="toPrint.style.display='none';WebBrowser1.ExecWB(8,1);toPrint.style.display=''" />
                    <input name="btnToExcel" type="button" value="ToExcel" onclick="toPaseExcel()" />
                    <input type="button" name="ToWps" value="ToWps" onclick="ToExcelOfWPS()" /></td>
                    <td>
                        &nbsp;</td>
                    <td align="right">
                    </td>
                </tr>
            </table>
        </fieldset>
      
        <table border="0px" width="100%">
            <tr>
                <td colspan="16" align="center" style="align: center; color: Red; font-size: x-large">
                    <asp:Label runat="server" ID="nodata" Visible="false"> No Data</asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="15">
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
                    <td align="center" colspan="3">
                        <Label ID="ReportTitle"  Style="font-size: 18px; font-family: Arial;
                            font-weight: bold; text-align: center;">Invoic QTY & Warehouse Receive Qty & Issue to Customer Qty Compare</Label>
                    </td>
                </tr>
                 <tr>
                    <td width="20%">
                        <asp:Label ID="lblshipinfo" runat="server"></asp:Label>
                    </td>
                    <td width="60%">
                        &nbsp;
                    </td>
                    <td width="20%" align="center">
                       &nbsp;
                    </td>
                </tr>
                       <tr>
                   
                    <td colspan="3">
                        <asp:GridView ID="gvDetail" runat="server" 
                            AutoGenerateColumns="False" ShowFooter="True">
                            <Columns>
                                <asp:BoundField DataField="Buyer" HeaderText="Buyer" ItemStyle-Width="5em" />
                                <asp:BoundField DataField="JO_NO" HeaderText="Jo Order No" ItemStyle-Width="7em"/>
                                <asp:BoundField DataField="total_qty" HeaderText="Order Qty" ItemStyle-Width="6em" DataFormatString="{0:N0}"/>
                                <asp:BoundField DataField="InWHS" HeaderText="Warehouse Receive QTY" ItemStyle-Width="9em" DataFormatString="{0:N0}"/>
                                <asp:BoundField DataField="InDateStr" HeaderText="Warehouse Receive Max Date" ItemStyle-Width="13em"/>
                                <asp:BoundField DataField="OutQty" HeaderText="Warehouse Issue To Customer QTY" ItemStyle-Width="13em" DataFormatString="{0:N0}"/>
                                <asp:BoundField DataField="OutDate" HeaderText="Warehouse Issue To Customer Max Date" ItemStyle-Width="13em"/>
                                <asp:BoundField DataField="ShipQty" HeaderText="Invoice Shipment QTY" ItemStyle-Width="9em" DataFormatString="{0:N0}"/>
                                <asp:BoundField DataField="ShipDate" HeaderText="Invoice Shipment Date" ItemStyle-Width="9em"/>
                               <asp:BoundField DataField="Diff" HeaderText="Shipment and Warehouse Receive QTY Discrepancy" ItemStyle-Width="15em" DataFormatString="{0:N0}"/>
                               <asp:BoundField DataField="LeftOverQty" HeaderText="Fgis Grade A&B" ItemStyle-Width="9em" DataFormatString="{0:N0}"/>
                            </Columns>
                            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Font-Size="9" />
                        </asp:GridView>
                    </td>
                  
                </tr>
                       <tr>
                        <td colspan="3" align="left">
                        </td>
                       </tr>
                     </table>
                       
                       
                       
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
