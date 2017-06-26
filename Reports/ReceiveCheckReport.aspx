<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReceiveCheckReport.aspx.cs" Inherits="Reports_ReceiveCheckReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
        <title>外发加工对数单</title>
    <object id="WebBrowser1" width="0" height="0" classid="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2">
    </object>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Script/Common.js"></script>
        <script language="javascript">
            function SearchContractNo() {
                var urlName = "../Reports/queryContract.aspx?site=GEG";
                var contractNo = window.showModalDialog(urlName, "Contract No.", "dialogWidth=1030px;dialogHeight=768px;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
                if (contractNo == null) return;
                document.all.txtContract.value = trim(contractNo);
            }
            function trim(str) {   //删除左右两端的空格

                return str.replace(/(^\s*)|(\s*$)/g, "");
            }
    </script>
    <style type="text/css">
        .auto-style1 {
            width: 86px;
        }
        .auto-style2 {
            width: 123px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
            <table style="width:30%;">
            <tr>
                <td class="auto-style1">Contract：</td>
                <td class="auto-style2">
                    <asp:TextBox ID="txtContract" runat="server" style="margin-left: 0px"></asp:TextBox>
                </td>
                  <td>
                    <input type="button" value="..." onclick="SearchContractNo()"/>
                </td>
                <td>
                <asp:Button ID="btnQuery" runat="server" OnClick="btnQuery_Click" Text="Query" Height="18px" />
                </td>
            </tr>          
        </table>
        <asp:Label ID="lblMessage" runat="server" Text="NO Data" style="font-weight: 700" ForeColor="Red"></asp:Label>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:reportViewer ID="reportViewer" runat="server" Width="1177px"></rsweb:reportViewer>
    </div>
    </form>
</body>
</html>
