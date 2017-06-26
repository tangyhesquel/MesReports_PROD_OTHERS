<%@ Page Language="C#" AutoEventWireup="true" CodeFile="jobOrderDetail.aspx.cs" Inherits="Reports_jobOrderDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Job Order Transaction Detail Report</title>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Script/Common.js"></script>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        .auto-style1 {
            width: 102px;
        }
        .auto-style2 {
            font-weight: bolder;
            font-size: 11px;
            background-color: #efefe7;
            padding-left: 5px;
            padding-right: 5px;
            padding-top: 2px;
            width: 119px;
        }
        .auto-style4 {
            width: 232px;
        }
        .auto-style5 {
            width: 240px;
        }
        .auto-style6 {
            font-weight: bolder;
            font-size: 11px;
            background-color: #efefe7;
            padding-left: 5px;
            padding-right: 5px;
            padding-top: 2px;
            width: 158px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search</legend>
            <table width="100%" id="queryTab" align="center">
                <tr>
                    <td width="80px" class="tdbackcolor">
                        Factory Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlfactoryCd" runat="server" DataTextField="DEPARTMENT_ID"
                            DataValueField="DEPARTMENT_ID">
                        </asp:DropDownList>
                    </td>
                    <td width="80px" class="tdbackcolor">
                        Garment Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownListGarmentType" runat="server" AutoPostBack="true"  OnSelectedIndexChanged="DropDownListGarmentType_SelectedIndexChanged">
                            <asp:ListItem Text="K" Value="K" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="W" Value="W"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <%--added by zoushichang on 2013.09.23 start mes024 --%>
                    <td class="tdbackcolor">
                        Process Code:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProcessCd" runat="server" >
                        </asp:DropDownList>
                    </td>
                    <td class="tdbackcolor">
                        Process Type:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProcessType" runat="server">
                            <asp:ListItem></asp:ListItem>
                            
                        </asp:DropDownList>
                    </td>
                    <td class="tdbackcolor">
                        Production Factory:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProdFactory" runat="server">
                        </asp:DropDownList>
                    </td>
                    <%--added by zoushichang on 2013.09.23 end mes024 --%>
                </tr>
                <tr>
                    <td width="80px" class="tdbackcolor">
                        Job Order No:
                    </td>
                    <td>
                        <asp:TextBox ID="txtjobOrderNo" runat="server"></asp:TextBox>
                    </td>
                    <td width="80px" class="tdbackcolor">
                        Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtBeginDate" runat="server" Width="120px" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td width="80px" class="tdbackcolor">
                        To Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtEndDate" runat="server" Width="120px" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
                    </td>
                    <td class="tdbackcolor" style="width: 22em">
                        <asp:CheckBox ID="cbConfirm" runat="server" Text="Include Confirm Data" Checked="true" />
                    </td>
                    <td class="tdbackcolor" style="width: 22em">
                        <asp:CheckBox ID="cbDraft" runat="server" Text="Include Draft Data" Checked="true" />
                    </td>
                    <td>
                        <asp:Button ID="btnQuery" Text="Query" runat="server" OnClick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <%--       <hr noshade size="1">--%>
    </div>
    <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="Red" Text="Tip Message!"
        Visible="False" Width="300px"></asp:Label>
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
                    <font face="Arial" size="4">Job Order Transaction Detail Report</font>
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
        <div id="mmPrint" align="right" style="width: 100%">
            <input type="button" name="print" value="Print" onclick="javacript: document.all.queryDiv.style.display = 'none'; document.all.mmPrint.style.visibility = 'hidden'; window.print(); document.all.queryDiv.style.display = 'block'; document.all.mmPrint.style.visibility = 'visible'"
                style="font-size: 16; width: 80; height: 26">
            <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs', false, 'Job Order Transaction Detail Report.htm')"
                style="font-size: 16; width: 80; height: 26">
            <input type="button" name="ToExcel" value="ToExcel" onclick="toPaseExcel()" style="font-size: 16;
                width: 80; height: 26" />
            <input type="button" name="excel" value="To Wps" onclick="ToExcelOfWPS()" />
        </div>
        <br>
        <table id="allTable" border="1" cellspacing="0" cellpadding="0" style="font-size: 12px;
            border-collapse: collapse">
            <tr>
                <td class="tr2style" bgcolor="#efefe7" colspan="2">
                    Factory CD:
                </td>
                <td>
                    <%=ddlfactoryCd.SelectedItem.Value %>
                </td>
                <td class="tr2style" bgcolor="#efefe7" colspan="2">
                    JO No:
                </td>
                <td class="auto-style5">
                    <%=txtjobOrderNo.Text %>
                </td>
                <td colspan="25">
                </td>
            </tr>
            <tr>
                <td colspan="30">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="tr2style" bgcolor="#efefe7" style="width: 15em">
                    Doc No
                </td>
                <td class="auto-style6" bgcolor="#efefe7">
                    Trx Date
                </td>
                <td width="200px" bgcolor="#efefe7">
                    Create Date
                </td>
                <td width="200px" bgcolor="#efefe7">
                    Create User
                </td>
                <td bgcolor="#efefe7" class="auto-style4">
                    Confirm Date
                </td>
                <td bgcolor="#efefe7" class="auto-style5">
                    Confirm User
                </td>
                <td bgcolor="#efefe7" class="auto-style1">
                    Process Type
                </td>
                <div id="divProcess" runat="server">
                </div>
                <td class="auto-style2" bgcolor="#efefe7">
                    Confirm
                </td>
            </tr>
            <div id="divBody" runat="server">
            </div>
        </table>
    </div>
    </form>
</body>
</html>
