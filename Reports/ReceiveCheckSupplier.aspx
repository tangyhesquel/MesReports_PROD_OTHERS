<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReceiveCheckSupplier.aspx.cs" Inherits="Reports_ReceiveCheckSupplier" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script language="javascript" type="text/javascript">
        function chooseJo() {
            var cur = window.event.srcElement;
            window.returnValue = cur.value;
            window.close();
        }
    </script>
    <base target="_self" />
</head>
<body bgcolor="#FFFFFF" text="#000000">
    <form id="form1" runat="server">
    <div>
        <table >
            <tr>
                <td class="tr2style">
                    供应商编码
                </td>
                <td>
                    <asp:TextBox ID="txtid" runat="server"></asp:TextBox>
                </td>
                <td class="tr2style">
                    供应商名称
                </td>
                <td >
                    <asp:TextBox ID="txtname" runat="server" Width="437px"></asp:TextBox>
                </td>
                <td >
                   <asp:Button ID="btnQuery" runat="server" CssClass="button_top" Text="Query" OnClick="btnQuery_Click" />
                </td>
               
            </tr>
            
           
        </table>
        <hr noshade size="1" />
        <br />
        <asp:Repeater ID="datalist" runat="server">
        <HeaderTemplate>
          <table width="100%" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
                        border-collapse: collapse">
                        <tr>
                            <td class="tr2style">
                                选择
                            </td>
                            <td class="tr2style">
                                供应商编码
                            </td>
                            <td class="tr2style">
                                供应商名称
                            </td>
                        </tr>
                    
                    
        </HeaderTemplate>
        <ItemTemplate>
        <tr>
                            <td class="dataTableStyle">
                                <input type="radio" name="rowsCheck" onclick='chooseJo()' class='noborder' value="<%#Eval("SUPPLIER_CD")%>" />
                            </td>
                            <td class="dataTableStyle">
                                <%#Eval("SUPPLIER_CD")%>
                            </td>
                            <td class="dataTableStyle">
                                 <%#Eval("Name")%>
                            </td>
                            
                           
                            
                           
                        </tr>
        </ItemTemplate>
        <FooterTemplate>
        <tr>
          <td colspan="3" align="center">
              <asp:Button ID="btnfirst" runat="server" Text="首页" CssClass="BUTTON" OnCommand="PageIndexChanging_Command" CommandArgument="1"/>
              <asp:Button ID="btnpre" runat="server" Text="上一页" CssClass="BUTTON" OnCommand="PageIndexChanging_Command" CommandArgument="-1"/>
              <asp:Button ID="btnnext" runat="server" Text="下一页" CssClass="BUTTON" OnCommand="PageIndexChanging_Command" CommandArgument="1"/>
              <asp:Button ID="btnlast" runat="server" Text="尾页" CssClass="BUTTON" OnCommand="PageIndexChanging_Command" CommandArgument="1"/>
          </td>
        </tr>
        </table>
        </FooterTemplate>
        </asp:Repeater>
        <%--<asp:GridView ID="gvBody" runat="server" Width="99%" AutoGenerateColumns="False"
            OnRowDataBound="gvBody_RowDataBound" CellPadding="4" ForeColor="#333333" 
            GridLines="None" AllowPaging="True" 
            onpageindexchanging="gvBody_PageIndexChanging" PageSize="50">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <Columns>
                <asp:BoundField />
                <asp:BoundField DataField="SUPPLIER_CD" HeaderText="供应商编码" />
                <asp:BoundField DataField="Name" HeaderText="供应商名称" />
        
            </Columns>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>--%>
        
        
    </div>
    </form>
</body>
</html>
