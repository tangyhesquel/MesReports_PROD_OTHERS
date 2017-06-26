<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mesDiscrepancyScanReport.aspx.cs" Inherits="Mes_mesDiscrepancyScanReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<link type="text/css" rel="stylesheet" href="../StyleSheet.css" />
    <title>JO Discrepancy Detail</title>

      <style type="text/css">
      
        .borderTalbe, .borderTalbe td, .borderTalbe th
        {
            border-width: 1px;
            border-style: solid;
            border-collapse: collapse;
        }
        </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table>
        <tr>
            <td colspan="2"> <asp:Literal ID="LiteralFactory" runat="server"></asp:Literal>
            &nbsp;Discrepancy Detail</td>
            <td rowspan="2"> <img id="barcodeImage" src="<%=ResolveUrl("~/BarcodeHandler.ashx")%>?<%= doc_NO %>" /></td>
        </tr>
        <tr>
            <td>DOC NO.: <asp:Literal ID="LiteralDocNO" runat="server"></asp:Literal>
            </td>
             <td>User: <asp:Literal ID="LiteralUser" runat="server"></asp:Literal>
            </td>
        </tr>
        <tr>
             <td>Date: <asp:Literal ID="LiteralDate" runat="server"></asp:Literal>
            </td>
            <td>Process: <asp:Literal ID="LiteralProcess" runat="server"></asp:Literal>
            </td>
            <td>Line: <asp:Literal ID="LiteralLine" runat="server"></asp:Literal>
            </td>
        </tr>
        
    </table>
        </div>

       

        <table class="borderTalbe centerAlignTable">
        <asp:Repeater ID="RepeaterData" runat="server">
            <HeaderTemplate>
                <tr>
                    <th>
                        JO
                    </th>
                    <th>
                        Color
                    </th>
                    <th>
                        Size
                    </th>
                    <th>
                        Grade
                    </th>
                    <th>
                        Qty
                    </th>
                    <th>
                        Remark
                    </th>
                </tr>
            </HeaderTemplate>

            <ItemTemplate>
                <tr>
                <td>
                     <%#Eval("JOB_ORDER_NO")%>
                </td>
                <td>
                     <%#Eval("COLOR_CD")%>
                </td>
                <td>
                     <%#Eval("SIZE_CD")%>
                </td>
                <td>
                     <%#Eval("GRADE_CD")%>
                </td>
                <td>
                                   
                     <%#Eval("QTY")%>
                </td>
                 <td>
                  
                </td>
                 </tr>
            </ItemTemplate>
            <FooterTemplate>
              <td colspan="4">Total</td>  
              <td colspan="2"><%= total %></td>  
            </FooterTemplate>
        </asp:Repeater>
        </table>
    </form>
</body>
</html>
