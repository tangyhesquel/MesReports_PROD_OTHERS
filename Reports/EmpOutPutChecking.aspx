<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EmpOutPutChecking.aspx.cs" Inherits="Reports_wipDaily"  CodePage="65001" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Employee Output Checking Report</title>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        BODY
        {
            font-size: 11px;
            padding: 5px;
            font-family: Arial,Times New Roman, 黑体,Verdana,Helvetica, sans-serif;
        }
        A:link
        {
            color: #000000;
            text-decoration: none;
        }
        A:active
        {
            color: #000000;
            text-decoration: none;
        }
        A:visited
        {
            color: #000000;
            text-decoration: none;
        }
        A:hover
        {
            color: #000000;
            text-decoration: none;
        }
        A:hover
        {
            left: 1px;
            border-bottom: 1px dotted;
            position: relative;
            top: 1px;
        }
        .BUTTON_Gary
        {
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            cursor: hand;
            background-color: #efefef;
            border: 1px solid #333333;
        }
        .BUTTON
        {
            font-weight: bolder;
            font-size: 11px;
            padding: 1px;
            padding-bottom: 2px;
            text-transform: uppercase;
            cursor: hand;
            height: 20px;
            color: #646400;
            background-color: #CCCC99;
            border: 1px solid #333333;
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
        .input_gary
        {
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            background-color: #efefef;
            border: 1px solid #333333;
        }
        .input_color
        {
            font-size: 11px;
            padding-bottom: 2px;
            text-transform: uppercase;
            background-color: #EEF7FF;
            border: 1px solid #333333;
        }
        .input_white
        {
            padding-left: 2px;
            font-size: 11px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            height: 18px;
            border: 1px solid #333333;
        }
        INPUT
        {
            padding-left: 2px;
            font-size: 11px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        TABLE
        {
            padding-right: 0px;
            padding-left: 0px;
            font-size: 11px;
            padding-bottom: 0px;
            padding-top: 0px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        .tr1style
        {
            font-size: 11px;
            background-color: #ffffff;
            padding-left: 5px;
            padding-right: 5px;
            padding-top: 2px;
        }
        .tr2style
        {
            font-weight: bolder;
            font-size: 11px;
            background-color: #efefe7;
            padding-left: 5px;
            padding-right: 5px;
            padding-top: 2px;
        }
        .tr3style
        {
            font-weight: bolder;
            font-size: 11px;
            background-color: #ffffff;
            padding-left: 5px;
            padding-top: 2px;
        }
        .bigfont
        {
            font-weight: bolder;
            font-size: 18px;
            color: #736d00;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        .bigfont1
        {
            font-weight: bolder;
            font-size: 14px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        .red
        {
            font-weight: 600;
            color: #800000;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
        }
        .footer
        {
            font-size: 11px;
            color: #000000;
            line-height: 20px;
            font-family: Arial,Times New Roman,黑体,Verdana,Helvetica, sans-serif;
            background-color: #efefef;
        }
        .button_top
        {
            font-family: "Arial" , "Helvetica" , "sans-serif";
            font-size: 12px;
            background-color: #CCCC99;
            border: ridge;
            font-weight: bold;
            border-width: 1px 1px 2px;
            border-color: #000000 #f2f2f2 #f2f2f2 #000000;
            color: #333366;
            text-transform: uppercase;
            cursor: hand;
        }
        .tdbackcolor
        {
            background-color: #EFEFE0;
        }
        .thstyle
        {
            background-color: #D2D1B0;
        }
        .style1
        {
            background-color: #EFEFE0;
            height: 19px;
        }
        .style2
        {
            height: 19px;
        }
    </style>
    
    <script language="javascript" type="text/javascript">
        function toPaseExcel() {

            var myExcel, myBook;
            try {
                myExcel = new ActiveXObject("Excel.Application");
            } catch (Exception) {
                alert("Open Excel Application exception");
            }

            if (myExcel != null) {
                window.clipboardData.setData("Text", document.all.ExcTable.outerHTML);
                myExcel.Visible = true;
                myBook = myExcel.Workbooks.Add();
                myBook.sheets(1).paste();

            }
        }
        function searchJo() {
            var urlName = "searchJO.aspx?factory=" + document.all.ddlFtyCd.value + "&userRandom=" + (Math.random() * 100000);
            var jo = window.showModalDialog(urlName, "Job Order No.", "dialogWidth=650pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (jo == null) return;
            document.all.txtJoNo.value = jo;
        }
        function init() {
            if (document.all.txtStartDate.value == '') {
             //  document.all.Table1.style.display = 'none';
            }
        }
    </script>
    <script language="javascript" type="text/javascript">
        function ToExcelOfWPS() {
            var myExcel, myBook;
            try {
                myExcel = new ActiveXObject("ET.Application");
            } catch (Exception) {
                alert("Open WPS Application exception");
                return;
            }

            if (myExcel != null) {
                var sel = document.body.createTextRange();
                sel.moveToElementText(ExcTable);
                sel.select();
                sel.execCommand("Copy");
                sel.execCommand("Unselect");
                myExcel.Visible = true;
                myBook = myExcel.Workbooks.Add();
                myBook.sheets("Sheet1").Paste();
            }

        }
    </script>

</head>
<body >
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search </legend>
            <table width="100%" id="queryTab">
                <tr>
                    <td width="5%" class="style1">
                        Factory:
                    </td>
                    <td class="style2">
                        <asp:DropDownList ID="ddlFtyCd" runat="server" 
                             Enabled="False">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </td>
         
                     <td height="19" class="tdbackcolor">
                        From Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtStartDate" runat="server" 
                            onfocus="WdatePicker({skin:'whyGreen'})" Width="74px"></asp:TextBox>
                    </td>
                    <td height="19" class="tdbackcolor">
                        End Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtToDate" runat="server" 
                            onfocus="WdatePicker({skin:'whyGreen'})" Width="81px"></asp:TextBox>
                    </td>
                    <td height="19" class="tdbackcolor">
                        JO No:
                    </td>
                    <td>
                        <asp:TextBox ID="txtJoNo" runat="server" 
                            Width="103px"></asp:TextBox>
                    </td>
                    <td class="style1">
                        Operaction Code:
                    </td>
                     <td>
                        <asp:TextBox ID="txtOperCd" runat="server" 
                             Width="63px"></asp:TextBox>
                    </td>
               
                 <td >
                        <asp:CheckBox ID="ChkBalanceLess0" Text="View_Balance_QTY<0 only" runat="server" />
                    </td>
                     <td >
                        <asp:CheckBox ID="ChkIncludeRework" Text="Include Rework QTY" runat="server" />
                    </td>
                    <td align="right">
                       
                        <asp:Button ID="btnQuery" runat="server" Text="Query"  CssClass="button_top" 
                            onclick="btnQuery_Click"/>
                    </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade size="1"/>
         <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="Red" Text="Tip Message!"
        Visible="False" Width="300px"></asp:Label>
          <div id="ExcTable" runat="server">
        <table id="Table1" width="100%" border="0" cellpadding="0"
			cellspacing="0">
			<tr>
				<td align="center" >
					<h2>Employee Output Checking Report</h2>
				</td>
			</tr>
			<tr>
				<td>
					&nbsp;
				</td>
			</tr>
			<tr>
				<td>
					<div id="mmPrint" align="right">
						<input type="button" name="print" value="Print"
							onclick="javacript:document.all.queryDiv .style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv .style.display='block';document.all.mmPrint.style.visibility='visible'"
							style="font-size:16;width:80;height:26"/>
						<input type="button" name="Save" value="Save"
							onclick="document.execCommand('SaveAs',false,'EmployeeOutputCheckingReport.htm')"
							style="font-size:16;width:80;height:26"/>
						<input type="button" name="excel" value="To Excel"
							onclick="toPaseExcel()" style="font-size:16;width:80;height:26"/><input 
                            type="button" onclick="ToExcelOfWPS()" value="To WPS"
							 style="font-size:16;width:80;height:26"/>
					</div>
				</td>
			</tr>
			<tr><td>&nbsp;</td></tr>
			
			<tr><td>
					<table border="1" cellspacing="0" cellpadding="0" style="font-size:12px;border-collapse:collapse" width="100%">
						<tr>
							<td class="tr2style" width="80" >Factory CD:</td><td ><%=ddlFtyCd.SelectedItem.Value %></td>
							<td class="tr2style" width="50">JO:</td><td><%=txtJoNo.Text%></td>
                            <td class="tr2style" width="70">Oper Code:</td><td><%=txtOperCd.Text%></td>
							<td class="tr2style" width="100">Date Range:</td><td>From:&nbsp;&nbsp;<%=txtStartDate.Text %></td><td> To:&nbsp;&nbsp;<%=txtToDate.Text%></td>
                            <td class="tr2style" width="80">Print Date:</td><td><%=DateTime.Now%></td>
						</tr>
					</table>
				</td>
			</tr>
			
			<tr><td>&nbsp;</td></tr>
			<tr>
				<td>
					<asp:GridView ID="gvBody" runat="server" AutoGenerateColumns="true" 
            CellPadding="4" EnableViewState="true"
            ForeColor="#333333" GridLines="Both" 
            Width="100%" ShowFooter="True">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" BorderWidth="1px" BorderStyle="Solid" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="False" ForeColor="White" Font-Size="8" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <RowStyle Font-Size="8" />
            </asp:GridView>
            </td>
			
			</tr>
			
		</table>
        </div>
    </div>
    </form>
</body>
</html>
