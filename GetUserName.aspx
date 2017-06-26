<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GetUserName.aspx.cs" Inherits="Task_GetUserName" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
        function window.onload() {
            var WshNetwork = new ActiveXObject("WScript.Network");
            var userid = WshNetwork.UserDomain + "\\" + WshNetwork.UserName.toUpperCase();
            Task_GetUserName.SetUserName(userid);
            location.href("default.aspx");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
