<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MesMain(old).aspx.cs" Inherits="MES_MesMain" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>

    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>

</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table>
            <tr>
                <td>
                    <b>Expect Received Date</b>:
                </td>
                <td>
                    <asp:TextBox ID="txtDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                </td>
                <td>
                    <asp:DropDownList ID="ddlView" runat="server">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:CheckBox ID="chkShirtstop" runat="server" Text="<b>Include ShirtStop</b>" />
                </td>
                <td>
                    <asp:Button ID="btnQuery" runat="server" Text="Query" OnClientClick="return CheckCDate();"
                        OnClick="btnQuery_Click" />
                </td>
            </tr>
        </table>
    </div>
    <div id="htmlDiv" runat="server">
        <table width="100%" border="1" cellpadding="2" cellspacing="2">
            <tr>
                <td>
                    <table width="100%">
                        <tr>
                            <td style="width: 60%" align="center">
                                <b>Outsourcing Price Monitor </b>
                            </td>
                            <td style="width: 40%" align="center">
                                <asp:Button ID="btnExcel" runat="server" Text="ToExcel" OnClick="btnExcel_Click" />&nbsp;&nbsp;
                                <asp:Button ID="btnDetailExcel" runat="server" Text="Download Detail To Excel" OnClick="btnDetailExcel_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divGV" runat="server">
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div>
        <asp:HiddenField ID="hfBalQty" runat="server" Visible="false" />
    </div>
    <div>
       <table style="font-size:12px ">
        <tr><td>Remarks:</td></tr>
        <tr><td>1. Month(Contract Expect Received Date): 截止到指定日期，当月的平均单价/SAH/平均SAH单价</td></tr>						
        <tr><td>2. YTD(Contract Expect Received Date): 截止到指定日期，当年的平均单价/SAH/平均SAH单价</td></tr>						
        <tr><td>3. Budget: 当年的预算的平均单价/SAH/平均SAH单价</td></tr>						
        <tr><td>4. Standard: 截止到指定日期，已经外发的订单的当年的平均单价/SAH/平均SAH单价</td></tr>						
        <tr><td>5. Month(Actual Return Date):截止到指定日期，已经回货的订单，当月的平均单价/SAH/平均SAH单价</td></tr>
        <tr><td>6. YTD(Actual Return Date):截止到指定日期，已经回货的订单，当年的平均单价/SAH/平均SAH单价</td></tr>
        <tr><td>7. YTD Year(2010):(前一年)全年的实际回货平均单价/SAH/平均SAH单价</td></tr>    
        </table>
    </div>
    </form>

    <script language="javascript">
        // OnClientClick="javascript:exportExcel(document.all.table1);" 
        function   exportExcel(atblData)   
        {      
             var   w   =   window.open("about:blank",   "Excel",   "widht=0,   height=0");   
             w.document.open( "text/html ",   "replace ");  
             var strHTML =atblData.outerHTML;
             //alert(strHTML);
             w.document.write(strHTML); 
              //w.document.write("123adgfdas");  
             w.document.execCommand('Saveas',false,   'C:\\log.xls'); 
             w.close();  
         }

        function CheckCDate()
        {
            if (window.document.getElementById("<%=txtDate.ClientID %>").value=="")
            {
                alert("please select date!");
                return false;   
            }
            return true;
        }

    
    </script>

</body>
</html>
