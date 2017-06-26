<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChartDetail.aspx.cs" Inherits="ChartDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Legend</title>
    <style type="text/css">

 p.MsoNormal
	{margin:0cm;
	margin-bottom:.0001pt;
	text-align:justify;
	text-justify:inter-ideograph;
	font-size:10.5pt;
	font-family:"Times New Roman";}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="100%">
            <%--<tr>
                <td colspan="2">
                    <br />
                </td>
            </tr>--%>
            <tr>
                <td style="background-color: blue ; width:30%">
                </td>
                <td>
                    A blue strip means the task is active.
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <br />
                </td>
            </tr>
            <tr>
                <td style="background-color: #778899 ; width:30%">
                </td>
                <td>
                    A brown strip means the task is closed.
                </td>
                <td>
                    <br />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <br />
                </td>
            </tr>
            <tr>
                <td style="background-color: #A00000 ; width:30%" >
                </td>
                <td>
                    A red strip means the task is delay.
                </td>
            </tr>
            <tr><td colspan="2"><br /></td></tr>
             <tr>
                <td style="background-color: yellow ; width:30%">
                </td>
                <td>
                    A yellow strip means the task is testing.
                </td>
            </tr>
            <tr><td colspan="2"><br /></td></tr>
            <tr>
                <td style="background-color: #e6e6fa; width:30%">
                </td>
                <td>
                    A gray background means it is holiday.
                </td>
            </tr>
             <tr><td colspan="2"><br /></td></tr>
            <tr><td colspan="2">
                <p class="MsoNormal">
                    <font face="Arial" size="1"><span lang="EN-US" 
                        style="FONT-SIZE: 9pt; FONT-FAMILY: Arial">Remarks: 
<o:p></o:p></span></font>
                </p>
                <p class="MsoNormal">
                    <font face="Arial" size="1"><span lang="EN-US" 
                        style="FONT-SIZE: 9pt; FONT-FAMILY: Arial">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
                    System will show all tasks except “New”,” Canceled” and ”Paused” tasks in normal 
                    board.<o:p></o:p></span></font></p>
                <p class="MsoNormal">
                    <font face="Arial" size="1"><span lang="EN-US" 
                        style="FONT-SIZE: 9pt; FONT-FAMILY: Arial">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
                    Overdue: “End Date” is before “Calendar Start Date” and status is not in “New”,” 
                    Canceled”,” Paused” and ”Closed”.<o:p></o:p></span></font></p>
                </td></tr>
        </table>
    </div>
    </form>
</body>
</html>
