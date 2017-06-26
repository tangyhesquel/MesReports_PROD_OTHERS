<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mesSearchJoNumber.aspx.cs"
    Inherits="Mes_mesSearchJoNumber" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Search JoNumber</title>
    <script language="javascript">
        function chooseJoNumber() {
            var cur = window.event.srcElement;
            window.returnValue = cur.value;
            window.close();
        }	
    </script>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <hr noshade size="1">
    <div id="reportDiv">
        <table border="1" cellspacing="0" cellpadding="0" style="font-size: 12px; border-collapse: collapse">
            <tr>
                <td>
                </td>
                <td class="tr2style">
                    Marker Id
                </td>
            </tr>
            <div id="divList" runat="server">
            </div>
        </table>
    </div>
    </form>
</body>
</html>
