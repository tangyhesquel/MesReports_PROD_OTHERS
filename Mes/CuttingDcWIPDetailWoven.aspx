<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CuttingDcWIPDetailWoven.aspx.cs"
    Inherits="Mes_CuttingDcWIPDetailWoven" %>

<%@ Register Src="~/UserControls/ExportDataUserControl.ascx" TagPrefix="mes" TagName="ExportData" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Cutting and Sewing WIP Report</title>
    <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <link rel="Stylesheet" href="../Css/Style.css" />
    <script src="../Script/Common.js" type="text/javascript"></script>
    <style>
        body
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
            margin-right: 6px;
        }
        .style2
        {
            height: 28px;
            font-size: 14px;
        }
    </style>
</head>
<body topmargin="0" style="font-size:14px; padding-top:0; padding-left:0" leftmargin="0" marginwidth="0" marginheight="0">
    
    <form id="form1" runat="server">
    <div id="queryDiv" class="HiddenWhilePrint">
        <asp:HiddenField ID="hfValue" runat="server" />
        
        <table width="100%" bgcolor="#c7dff1">
            <tr>
                <td align="center">
                    <h3>
                        <asp:Literal runat="server" ID="Title" Text="<%$Resources:GlobalResources,STRING_CUT_SEW_INQUERY %>"> </asp:Literal>
                    </h3>
                </td>
            </tr>
            <tr>
                <td align="right">
                </td>
            </tr>
            
            <tr>
                <td style="width: 70%">
                    <table width="800px"> 
                        <tr>
                            <td class="style2">
                                <asp:Literal runat="server" ID="Literal1" Text="<%$Resources:GlobalResources,STRING_GMT_TYPE %>"> </asp:Literal>
                                :</td>
                            <td class="style2">
                                <asp:DropDownList ID="ddGarmentType" runat="server" AutoPostBack="True" 
                                    Width="118px" BackColor="#C7DFF1" Font-Italic="true" Height="22px">
                                    <asp:ListItem Text="Knit" Value="K"></asp:ListItem>
                                    <asp:ListItem Text="Woven" Value="W" Selected="True"></asp:ListItem>
                                </asp:DropDownList>
                            </td> 
                            <td class="style2">
                                <asp:Literal runat="server" ID="Literal2" Text="<%$Resources:GlobalResources,STRING_PROCESS_TYPE %>"> </asp:Literal>
                                :</td>
                            <td class="style2">
                                <asp:DropDownList ID="ddProcessType" runat="server" AutoPostBack="True" 
                                    OnSelectedIndexChanged="ddProcessCd_SelectedIndexChanged" Width="134px" 
                                    BackColor="#C7DFF1" Font-Italic="true" Height="22px">
                                    <asp:ListItem Selected="True" Value="I">I</asp:ListItem>
                                    <asp:ListItem Value="P">P</asp:ListItem>
                                    <asp:ListItem Value="S">S</asp:ListItem>
                                    <asp:ListItem Value="O">O</asp:ListItem>
                                    <asp:ListItem Value="E">E</asp:ListItem>
                                    <asp:ListItem Value="T">T</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            
                        </tr> 
                        <tr>
                            <td class="style2">
                                <asp:Literal runat="server" ID="Literal4" Text="<%$Resources:GlobalResources,STRING_PROCESS %>"> </asp:Literal>
                                :</td>
                            <td class="style2">
                                <asp:DropDownList ID="ddProcessCd" runat="server" 
                                    OnSelectedIndexChanged="ddProcessCd_SelectedIndexChanged" AutoPostBack="true" 
                                    Width="117px" BackColor="#C7DFF1" Font-Italic="true" Height="22px">
                                    <asp:ListItem Selected="True" Value="ALL">ALL</asp:ListItem>
                                    <asp:ListItem Value="CUT">CUT</asp:ListItem>
                                    <asp:ListItem Value="SEW">SEW</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="style2">
                                <asp:Literal runat="server" ID="Literal5" Text="<%$Resources:GlobalResources,STRING_LINE_CODE %>"> </asp:Literal>
                                :</td>
                            <td class="style2">
                                <asp:DropDownList ID="ddlProductionLine" runat="server" DataTextField="Production_Line_cd"
                                    DataValueField="PRODUCTION_LINE_cd" Height="22px" Width="135px" 
                                    BackColor="#C7DFF1" Font-Italic="true"> 
                                </asp:DropDownList>
                            </td>
                            <td align="RIGHT" colspan="2" class="style2">
                                <asp:Button ID="btnQuery" Text="Query" runat="server" CssClass="button1" 
                                    OnClick="btnQuery_Click" Height="23px" Width="65px" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" align="right" class="style2">
                                <mes:ExportData ID="exportData" CssClass="button1" runat="server" EnableViewState="false"  />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            
        </table>
        <hr />
    </div>
    <div id="EtTable">
        <table id="ExcTable" width="100%" border="0" cellpadding="0" cellspacing="0">
            
            
            <tr>
                <td>
                    <table width="100%" border="0" cellspacing="2" cellpadding="0" style="font-size: 12px;
                        border-collapse: collapse">
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
                    <div id="divBody" runat="Server" visible="false"></div>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divGvBodySum" runat="Server" visible="false">
                        <table border="0" cellspacing="0" cellpadding="0" width="100%">
                            <tr>
                                <td>
                                    <font size="3"><b>Production Line Level</b></font>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:GridView ID="gvBodySum" runat="server" Width="100%" CellPadding="4" ForeColor="#333333"
                                        GridLines="None" OnRowDataBound="gvBodySum_RowDataBound" ShowFooter="TRUE">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" Font-Size="10" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="left" />
                                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" HorizontalAlign="Left" Font-Size="10" />
                                        <EditRowStyle BackColor="#2461BF" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr><td>&nbsp;</td></tr>
            <tr>
                <td>
                    <div id="divGvBody" runat="Server" visible="false">
                        <table border="0" cellspacing="0" cellpadding="0" width="100%">
                            <tr>
                                <td>
                                    <font size="3"><b>JO Level</b></font>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:GridView ID="gvBody" runat="server" Width="100%" CellPadding="4" ForeColor="#333333"
                                        GridLines="None" ShowFooter="true" OnRowDataBound="gvBody_RowDataBound">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" Font-Size="10" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="left" />
                                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" HorizontalAlign="Left" Font-Size="10" />
                                        <EditRowStyle BackColor="#2461BF" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
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
