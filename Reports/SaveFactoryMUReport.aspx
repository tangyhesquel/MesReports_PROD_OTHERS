<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SaveFactoryMUReport.aspx.cs"
    Inherits="Reports_GEGMUReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Factory MU Report</title>
    <script src="../My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../Script/Common.js"></script>
    <style type="text/css">
        body
        {
            font-size: 8.5pt;
            font-family: Tahoma,Verdana,MS Sans Serif,Courier New;
        }
        table
        {
            font-size: 8.5pt;
            font-family: Tahoma,Verdana,MS Sans Serif,Courier New;
        }
        .style2
        {
            color: #000000;
        }
        BODY Td
        {
            font-size: 12;
            height: 25;
            color: #000000;
        }
        SELECT
        {
            font-size: 8pt;
            font-family: 'MS Sans Serif' ,Arial,Tahoma;
            background-color: #efefe7;
        }
        INPUT
        {
            font-size: 9pt;
            font-family: Tahoma, Verdana, Arial, Helvetica;
            height: 14pt;
        }
        
        .Input1
        {
            font-size: 8pt;
            color: #000000;
            width: 120;
            height: 10pt;
        }
    </style>
</head>
<body style="font-size: smaller">
    <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="Red" Text="Tip Message!"
        Visible="False" Width="350px"></asp:Label>
    <asp:Label ID="lblMessage2" runat="server" Font-Bold="True" ForeColor="#003399" Text="Tip Message!"
        Visible="False" Width="450px"></asp:Label>
    <br />
</body>
</html>
