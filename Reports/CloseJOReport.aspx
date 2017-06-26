<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CloseJOReport.aspx.cs" Inherits="Reports_CloseJOReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Script/jquery-1.4.2.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        BODY
        {
            font-size: 12px;
            padding: 5px;
            font-family: Arial, Helvetica, sans-serif;
            padding-bottom: 20px;
        }
        a
        {
            text-decoration: none;
            color: #057fac;
        }
        
        a:hover
        {
            text-decoration: none;
            color: #999;
        }
        
        .BUTTON_down
        {
            font-weight: bolder;
            font-size: 11px;
            padding: 1px;
            padding-bottom: 2px;
            text-transform: uppercase;
            cursor: hand;
            height: 20px;
            color: #646400;
            background-color: #efefe7;
            border: 1px solid #646400;
        }
        
        INPUT
        {
            padding-left: 2px;
            font-size: 12px;
            outline: none;
            vertical-align: middle;
            font: 12px Arial, Helvetica, sans-serif;
        }
        TABLE
        {
            width: 100%;
            border-collapse: collapse;
            margin: 1em 0;
        }
        table, td
        {
            font: 100% Arial, Helvetica, sans-serif;
           
        }
        tr td
        {
            padding: .5em;
            text-align: center;
        }
        
        .tdbackcolor
        {
            background-color: #EFEFE0;
        }
        .hidden
        {
            display: none;
        }
        .button_top
        {
            border-left: 1px ridge #000000;
            border-right: 1px ridge #f2f2f2;
            border-top: 1px ridge #000000;
            border-bottom: 2px ridge #f2f2f2;
            font-family: "Arial" , "Helvetica" , "sans-serif";
                font-size: 12px;
                background-color: #CCCC99;
                font-weight: bold;
                color: #333366;
                text-transform: uppercase;
                cursor: hand;
            height: 24px;
        }
        td.over
        {
            background: #ecfbd4;
        }
        .txtAlign
        {
            text-align:center;    
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search</legend>
            <table id="queryTab" align="center">
                <tr>
                    <td class="tdbackcolor">
                        <asp:Localize ID="LGarmentType" 
                            runat="server" 
                            Text="<%$ Resources:GlobalResources, GarmentType %>" />
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlGarmentType" runat="server" OnSelectedIndexChanged="ddlGarmentType_SelectedIndexChanged" Width="124px">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="W">Woven</asp:ListItem>
                            <asp:ListItem Value="K">Knit</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="tdbackcolor">
                        <asp:Localize ID="LProcessCode" 
                            runat="server" 
                            Text="<%$ Resources:GlobalResources, ProcessCode %>" />
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProcessCode" 
                            runat="server" 
                            DataTextField="SHORT_NAME" 
                            DataValueField="PROCESS_CD"  Width="124px">
                        </asp:DropDownList>
                    </td>
                    <td class="tdbackcolor">
                        <asp:Localize ID="LProcessType" 
                            runat="server" 
                            Text="<%$ Resources:GlobalResources, ProcessType %>" />
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProcessType" runat="server"  Width="124px">
                        </asp:DropDownList>
                    </td>
                    <td class="tdbackcolor">
                        <asp:Localize ID="FACTORY_CD" 
                            runat="server" 
                            Text="<%$ Resources:GlobalResources, FACTORY_CD %>" />
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFactoryCode" runat="server" Enabled="true"  Width="124px"> 
                        </asp:DropDownList>
                    </td>
                    <td class="tdbackcolor">
                        <asp:Localize ID="STRING_WASH_TYPE" 
                            runat="server" 
                            
                            Text="<%$ Resources:GlobalResources, STRING_WASH_TYPE %>" />
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlWashType" runat="server"  Width="124px">
                            <asp:ListItem Value="">ALL</asp:ListItem>
                            <asp:ListItem Value="W">WASH</asp:ListItem>
                            <asp:ListItem Value="NW">NON-WASH</asp:ListItem>
                        </asp:DropDownList>
                        </td>
                </tr>
                <tr>
                    <td class="tdbackcolor">
                        <asp:Localize ID="Localize2" 
                            runat="server" 
                            Text="<%$ Resources:GlobalResources,  STRING_JO %>" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtJoNo" runat="server" Width="120px"></asp:TextBox>
                    </td>
                    <td class="tdbackcolor">
                        <asp:Localize ID="Localize1" 
                            runat="server" 
                            Text="<%$ Resources:GlobalResources, CloseState %>" />
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlCloseState" AutoPostBack="true"
                            runat="server" 
                            onselectedindexchanged="ddlCloseState_SelectedIndexChanged"
                             Width="124px" >
                            <asp:ListItem Value="N">No</asp:ListItem>
                            <asp:ListItem Value="Y">Yes</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="tdbackcolor">
                        <asp:Localize ID="LStartDate" 
                            runat="server" 
                            
                            Text="<%$ Resources:GlobalResources, STRING_CLOSE_DATE_FROM %>"  
                            Visible="False"/>
                    </td>
                    <td>
                        <asp:TextBox ID="txtStartDate" 
                            runat="server" onFocus="WdatePicker({skin:'whyGreen'})"
                            Visible="false"   Width="120px"></asp:TextBox>
                    </td>
                    <td class="tdbackcolor">
                        <asp:Localize ID="LEndDate" 
                            runat="server" 
                            
                            Text="<%$ Resources:GlobalResources,  STRING_CLOSE_DATE_TO %>"  
                            Visible="False"/>
                    </td>
                    <td>
                        <asp:TextBox ID="txtEndDate" 
                            runat="server" onFocus="WdatePicker({skin:'whyGreen'})"
                            Visible="false"   Width="120px"></asp:TextBox>
                    </td>
                    
                    <td colspan="2">
                    <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClick="btnQuery_Click" />
                    </td>

                </tr>
                <tr>
                    <td><asp:Label ID="JoNoErrorLabel" 
                            runat="server" 
                            Font-Italic="True" Visible="false"
                            ForeColor="Red"></asp:Label></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td><asp:Label ID="CloseDateFromErrorLabel" 
                            runat="server" 
                            Font-Italic="True" Visible="false"
                            ForeColor="Red"></asp:Label></td>
                    <td></td>
                    <td><asp:Label ID="CloseDateToErrorLabel" 
                            runat="server" 
                            Font-Italic="True" Visible="false" 
                            ForeColor="Red"></asp:Label>
                    </td>
                </tr>
            </table>
        </fieldset>
        <table width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="center">
                    <font face="Arial" size="4">
                        <asp:Localize ID="LCloseJOListReport" 
                        runat="server" 
                        Text="<%$ Resources:GlobalResources, CloseJOListReport %>" />
                    <br />
                    </font>
                                <asp:Label ID="errorLabel" 
                                    runat="server" 
                                    ForeColor="Red" 
                                    style="text-align: center"></asp:Label>
                                    
                </td>
            </tr>
            <tr>
                <td>
                    <div id="mmPrint" align="right" style="width: 100%">
                        <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv.style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv.style.display='block';document.all.mmPrint.style.visibility='visible'"
                            style="font-size: 16; width: 60px; height: 26; margin-left: 0px;" />&nbsp;&nbsp;
                        <input type="button" name="ToExcel" value="ToExcel" onclick="toPaseExcel()" style="font-size: 16;
                            width: 60px; height: 26" />&nbsp;&nbsp;
                        <input type="button" name="ToWps" value="ToWps" onclick="ToExcelOfWPS()" style="font-size: 16;
                            width: 60px; height: 26" />
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" cellpadding="0" cellspacing="0" id="Table1">
                        <tr>
                            <td>
                                <%--报表明细--%>
                                <div id="ExcTable">                               
                                    <asp:GridView ID="gvBody" 
                                    runat="server" 
                                    Width="100%" AutoGenerateColumns="TRUE"
                        
                                    OnRowDataBound="gvBody_RowDataBound" 
                                    ShowFooter="false" 
                                    Font-Names="Arial" 
                                    
                                    Font-Size="8.5pt">
                                        <HeaderStyle BackColor="#EFEFE0" />
                    </asp:GridView>                                    
                       </div>             
                                <asp:Repeater ID="rpList" runat="server">
                                    <HeaderTemplate>
                                        <table width="100%" cellpadding="0" cellspacing="0" border="1px solid #808080" style="text-align:center">
                                            <tr style="background-color: #5D7B9D; color: White; font-size: 14px;">
                                                <td>
                                                    <asp:Literal ID="LIntervalD" runat="server" Text="<%$ Resources: GlobalResources, IntervalD%>" />
                                                </td>
                                                <td>
                                                    <=1
                                                </td>
                                                <td>
                                                    <=3
                                                </td>
                                                <td>
                                                    <=5
                                                </td>
                                                <td>
                                                    <=7
                                                </td>
                                                <td>
                                                    <=10
                                                </td>
                                                <td>
                                                    <=15
                                                </td>
                                                <td>
                                                    <=20
                                                </td>
                                                <td>
                                                    >20
                                                </td>
                                                <td>
                                                    <asp:Literal ID="LTotal" runat="server" Text="<%$ Resources:GlobalResources, Total%>" />
                                                </td>
                                            </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <%# Eval("Type")%>
                                            </td>
                                            <td>
                                                <%# Eval("Less1")%>
                                            </td>
                                            <td>
                                                <%# Eval("Less3")%>
                                            </td>
                                            <td>
                                                <%# Eval("Less5")%>
                                            </td>
                                            <td>
                                                <%# Eval("Less7")%>
                                            </td>
                                            <td>
                                                <%# Eval("Less10")%>
                                            </td>
                                            <td>
                                                <%# Eval("Less15")%>
                                            </td>
                                            <td>
                                                <%# Eval("Less20")%>
                                            </td>
                                            <td>
                                                <%# Eval("More20")%>
                                            </td>
                                            <td>
                                                <%# Eval("Total")%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
    <script language="javascript" type="text/jscript">
        $(document).ready(function () {
            $("#btnQuery").live("click", function () {
                //if ($("#txtStartDate").val() == "" && $("#txtStartDate").val() == "") {
                //                 if ($("#txtStartDate").val() == "" && $("#txtEndDate").val() == "" && $("#txtJoNo").val() == "") {
                //                     alert("JoNo Can not be Empty");
                //                     return false;
                //                 }

                /*if ($("#txtJoNo").val() == "") {
                    if ($("#ddlCloseState").val() == "Y") {
                        if ($("#txtStartDate").val() == "" || $("#txtEndDate").val() == "") {
                            alert("Please select a time");
                            return false;
                        }
                    }
                }*/
                
                

                //参数Site没输
                if ($("#ddlProcessCode").val() == null) {
                    return false;
                }

            });

            //            $("#ddlCloseState").change(function () {

            //            });
        });
    </script>
</body>
</html>
