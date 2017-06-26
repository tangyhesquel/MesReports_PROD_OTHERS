<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableEventValidation="false"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <script src="My97DatePicker/WdatePicker.js" type="text/javascript"></script>

    <link rel="Stylesheet" href="StyleSheet.css" />
    <title>Weekly Report</title>

    <script language="javascript" type="text/javascript">

    //获取当前的X坐标值    

    function pageX(elem)
    {
        return elem.offsetParent?(elem.offsetLeft+pageX(elem.offsetParent)):elem.offsetLeft;
        }

　　//获取当前的Y坐标值
    function pageY(elem)
    {
        return elem.offsetParent?(elem.offsetTop+pageY(elem.offsetParent)):elem.offsetTop;
        }
    //显示Div
    function showDiv(elem)
    {
         var inp=document.getElementById(elem);
         var divshow=document.getElementById("divBody");
         var mouseleft=pageX(inp)+inp.offsetWidth;
         var mousetop=pageY(inp)+inp.offsetHeight;
         var mixleft=document.body.scrollWidth-parseInt(divshow.style.width.replace("px",""),10);
         var mixtop=document.body.scrollHeight-parseInt(divshow.style.height.replace("px",""),10);
         divshow.innerHTML=_Default.GenerateDivContent(elem).value;
         divshow.style.position = "absolute";
         if(mouseleft>mixleft)
         {
            divshow.style.left=mixleft;
         }
         else
         {
            divshow.style.left=mouseleft ;
         }
         if(mousetop>mixtop)
         {
            divshow.style.top=mixtop;
         }
         else
         {
            divshow.style.top=mousetop;
         }
         divshow.style.display="block";      
         }    
    function hideDiv()
    {
        var divshow=document.getElementById("divBody");
        divshow.style.display="none";
    }
    
    function ShowGroupMaintenance()
    {
        var returnValue=window.showModalDialog("GroupMaintenance.aspx","Group Maintenance","dialogWidth=800px;dialogHeight=480px;");
        if(returnValue!=undefined)
        {
            if (returnValue=="Y")
            {
                var dt=_Default.GetDllDataSource();
                document.getElementById("ddlGroup").length=0;
                for(var i = 0; i < dt.value.Rows.length; i++)
                {
                    var dr=dt.value.Rows[i];
                    document.getElementById("ddlGroup").options.add(new Option(dr.group_name,dr.group_name));
                }
            }
        }
    }
    function DataBind(elem)
    {
         var dt=_Default.GetDllDataSource();
         document.getElementById("ddlGroup").length=0;
         for(var i = 0; i < dt.value.Rows.length; i++)
         {
              var dr=dt.value.Rows[i];
              document.getElementById("ddlGroup").options.add(new Option(dr.group_name,dr.group_name));
              if(dr.group_name==elem)
              {
                document.getElementById("ddlGroup").options[i].selected = true; 
              } 
         }
    }
    
    function GetGroupSelelctValue()
    { 
        document.getElementById("hfSelectValue").value=document.getElementById("ddlGroup").value;
    }
    function ChartDetail() {
        window.showModalDialog("ChartDetail.aspx","ChartDetail","dialogWidth=600px;dialogHeight=350px");
    }

    </script>
    <style type="text/css">
    th{font-weight:normal ; font-size:12px;}
    </style> 
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:HiddenField runat="server" ID="hfSelectValue" />
    <table style="width: 1024px;">
        <tr>
            <td>
                System：
            </td>
            <td>
                <asp:DropDownList ID="ddlSystem" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSystem_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td >
                Sub-System：
            </td>
            <td>
                <asp:DropDownList ID="ddlSubSystem" runat="server">
                </asp:DropDownList>
            </td>
            <td>
                Handler：
            </td>
            <td>
                <asp:DropDownList ID="ddlHandler" runat="server">
                </asp:DropDownList>
            </td>
            <td>
                Group：
            </td>
            <td>
                <select id="ddlGroup" onchange="GetGroupSelelctValue()">
                </select>
            </td>
            <td>
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
                        <asp:Button ID="btnGroup" runat="server" Text="..." Width="23px" OnClientClick="ShowGroupMaintenance()" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td>
                Start Date：
            </td>
            <td>
                <asp:TextBox ID="txtBeginDate" runat="server" Width="142px" onFocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
            </td>
            <td>
                End Date：
            </td>
            <td>
                <asp:TextBox ID="txtEndDate" runat="server" Width="142px" onfocus="WdatePicker({skin:'whyGreen'})"></asp:TextBox>
            </td>
            <td>
                Order By：
            </td>
            <td>
                <asp:DropDownList runat="server" ID="ddlOrderBy">
                    <asp:ListItem Selected="True" Value="0">Date/System
                    </asp:ListItem>
                    <asp:ListItem Value="1">System/Date</asp:ListItem>
                    <asp:ListItem Value="2">Handler/Date</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:CheckBox ID="cbIsNew" runat="server" Text="Only New SRF" />
            </td>
            <td>
                <asp:Button ID="btnEnter" Text="Query" runat="server" CssClass="button" OnClick="btnEnter_Click" />
                <input type="button" class="button" value="Show Legend" style=" width:80px" onclick="ChartDetail()" />
            </td>
            <td>
            </td>
        </tr>
    </table>
    <%--<div style="overflow:scroll; height: 495px;">--%>
    <asp:GridView ID="gvBody" runat="server" AutoGenerateColumns="False" OnRowDataBound="gvBody_RowDataBound" Width="100%">
        <Columns>
            <asp:BoundField HeaderText="System" DataField="groupname"><ItemStyle Width="40" /></asp:BoundField>
            <asp:BoundField HeaderText="Sub-System" DataField="sysname"><ItemStyle Width="130" /></asp:BoundField>
            <asp:BoundField HeaderText="SRF#" DataField="note_no"><ItemStyle Width="50" /></asp:BoundField>
            <asp:BoundField HeaderText="Handler" DataField="handler"><ItemStyle Width="120" /></asp:BoundField>
            <asp:BoundField HeaderText="Overdue" ><ItemStyle Width="50" /></asp:BoundField>
            <asp:BoundField>
               <%--  <ItemStyle Width="80%" />--%>
            </asp:BoundField>
            <asp:BoundField DataField="status" />
            <asp:BoundField DataField="begin_time" />
            <asp:BoundField DataField="end_time" />
        </Columns>
        <%--<HeaderStyle CssClass="Freezing" /> --%>
    </asp:GridView>
    <asp:GridView ID="gvNewSRF" runat="server" AutoGenerateColumns="False"
        Width="100%" CellPadding="4" ForeColor="#333333" GridLines="None" 
        onrowdatabound="gvNewSRF_RowDataBound">
        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        <Columns>
            <asp:BoundField HeaderText="System" DataField="groupname"></asp:BoundField>
            <asp:BoundField HeaderText="Sub-System" DataField="sysname"></asp:BoundField>
            <asp:BoundField HeaderText="SRF#" DataField="note_no"></asp:BoundField>
            <asp:BoundField HeaderText="CreateBy" DataField="creator"></asp:BoundField>
            <asp:BoundField HeaderText="CreateDate" DataField="create_time"></asp:BoundField>
            <asp:BoundField HeaderText="Subject" DataField="subject"></asp:BoundField>
            <asp:BoundField HeaderText="Description" DataField="content" />
        </Columns>
        <%--<HeaderStyle CssClass="Freezing" /> --%><FooterStyle BackColor="#5D7B9D" 
            Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <EditRowStyle BackColor="#999999" />
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
    </asp:GridView>
    <%--</div>--%>
    <div id="divBody" style="display: none; background-color: White; width: 300px; height: 110px"
        runat="server">
    </div>
    </form>
</body>
</html>
