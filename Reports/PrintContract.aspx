<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PrintContract.aspx.cs" Inherits="Reports_PrintContract" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>
<%--<identity impersonate="true"/>--%>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>外发加工单次合同</title>
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
            width: 78px;
        }
        .auto-style2 {
            width: 159px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <table style="width:50%;">
            <tr>
                <td class="auto-style1">Contract：</td>
                <td class="auto-style2">
                    <asp:TextBox ID="txtContract" runat="server"></asp:TextBox>
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
        <rsweb:reportviewer ID="reportViewer" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Size="14pt" Height="575px" Width="1258px">
            
        </rsweb:reportviewer>
        
    </div>
    </form>
</body>
</html>
