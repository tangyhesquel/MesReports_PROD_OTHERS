<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProWipQuery.aspx.cs" Inherits="Mes_ProWipQuery" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <link rel="Stylesheet" href="../Css/Style.css" />
    <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
    <script src="../Script/Common.js" type="text/javascript"></script>
    <script type="text/javascript">
        function searchDept() {
            var value = window.showModalDialog("SearchDept.aspx", "", "dialogWidth=800px;");
            if (value != undefined) {
                if (value != "") {
                    document.getElementById("txtDept").value = value;
                }
            }
        }
    </script>
    <style type="text/css">
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
        }
        .style2
        {
            width: 22px;
        }
        .DivClose
        {
            display: none;
            position: absolute;
            width: 250px;
            height: 220px;
            border-style: solid;
            border-color: Gray;
            border-width: 1px;
            background-color: #99A479;
        }
        
        .LabelClose
        {
            vertical-align: text-top;
            position: absolute;
            bottom: 0px;
            font-family: Verdana;
        }
        
        .LabelClear
        {
            vertical-align: text-top;
            position:absolute;
            text-align:right;
            bottom: 0px;
            right: 0px;
            font-family: Verdana;
        }
        
        .DivCheckBoxList
        {
            display: none;
            background-color: White;
            width: 250px;
            position: absolute;
            height: 200px;
            overflow-y: auto;
            overflow-x: hidden;
            border-style: solid;
            border-color: Gray;
            border-width: 1px;
        }
        
        .CheckBoxList
        {
            position: relative;
            width: 250px;
            height: 10px;
            overflow: scroll;
            font-size: small;
        }
    </style>
    <script type="text/javascript">
        var timoutID;

        //This function shows the checkboxlist
        function ShowMList() {
            var divRef = document.getElementById("divCheckBoxList");
            divRef.style.display = "block";
            var divRefC = document.getElementById("divCheckBoxListClose");
            divRefC.style.display = "block";
        }

        //This function hides the checkboxlist
        function HideMList() {
            document.getElementById("divCheckBoxList").style.display = "none";
            document.getElementById("divCheckBoxListClose").style.display = "none";
        }

        //This function finds the checkboxes selected in the list and using them,
        //it shows the selected items text in the textbox (comma separated)
        function FindSelectedItems(sender, textBoxID) {
            var cblstTable = document.getElementById(sender.id);
            var checkBoxPrefix = sender.id + "_";
            var noOfOptions = cblstTable.rows.length;
            var selectedText = "";
            for (i = 0; i < noOfOptions; ++i) {
                if (document.getElementById(checkBoxPrefix + i).checked) {
                    if (selectedText == "")
                        selectedText = document.getElementById
                                   (checkBoxPrefix + i).parentNode.innerText;
                    else
                        selectedText = selectedText + "," +
                 document.getElementById(checkBoxPrefix + i).parentNode.innerText;
                }
            }
            document.getElementById(textBoxID.id).value = selectedText;
        }

        //Clear Selected Checkbox
        function ClearSelectedItems(textBoxID) {
            var cblstTable = document.getElementById(lstMultipleValues.id);
            var checkBoxPrefix = lstMultipleValues.id + "_";
            if (cblstTable != null && cblstTable.rows.length > 0) {
                var noOfOptions = cblstTable.rows.length;
                for (i = 0; i < noOfOptions; ++i) {
                    if (document.getElementById(checkBoxPrefix + i).checked) {
                        document.getElementById(checkBoxPrefix + i).checked = false;
                    }
                }
            }
            document.getElementById(textBoxID.id).value = "All";
        }
    </script>
</head>
<body topmargin="0" style="font-size: 12px" leftmargin="0" marginwidth="0" marginheight="0">
    <form id="form1" runat="server">
    <div>
        <asp:HiddenField ID="hfValue" runat="server" />
        <table width="100%" bgcolor="#c7dff1">
            <tr>
                <td align="center">
                    <h3>
                        <asp:Literal runat="server" ID="Title" Text="<%$Resources:GlobalResources,STRING_WIP_INQUERY %>"> </asp:Literal>
                    </h3>
                </td>
            </tr>
            <tr>
                <td align="right">
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%">
                        <tr>
                            <td style="width: 50px">
                                <asp:Literal runat="server" ID="Literal1" Text="<%$Resources:GlobalResources,STRING_JO_NO %>"> </asp:Literal>
                            </td>
                            <td style="width: 20%">
                                <asp:TextBox ID="txtWipJo" runat="server" Width="214px"></asp:TextBox>&nbsp;
                            </td>
                            <td class="style2" style="width: 173px">
                                <asp:Literal runat="server" ID="Literal2" Text="<%$Resources:GlobalResources,STRING_GMT_TYPE %>"> </asp:Literal>
                            </td>
                            <td style="width: 120px">
                                <asp:DropDownList ID="ddlWipGarmentType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlWipGarmentType_SelectedIndexChanged" Width="221px" >
                                    <asp:ListItem Text="All" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Knit" Value="K"></asp:ListItem>
                                    <asp:ListItem Text="Woven" Value="W"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td  align="left">
                                <asp:Literal runat="server" ID="Literal4" Text="<%$Resources:GlobalResources,STRING_SHIPPED_JO %>"> </asp:Literal>
                            </td>
                            <%--Added By ZouShiChang ON 2013.09.23 Start MES024 --%>
                            <td >
                                
                                <asp:CheckBox ID="ckbIsOut" runat="server" AutoPostBack="True" OnCheckedChanged="ckbIsOut_CheckedChanged" />
                                
                            </td>
                            <td  align="left">
                                <asp:Panel ID="pnOutDate" runat="server" Visible="false">
                                    <asp:TextBox ID="txtOutBeginDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"
                                        Width="94px"></asp:TextBox>&nbsp;To&nbsp;<asp:TextBox ID="txtOutEndDate" runat="server"
                                            onfocus="WdatePicker({skin:'whyGreen'})" Width="97px"></asp:TextBox>
                                </asp:Panel></td>
                            <%--Added By ZouShiChang ON 2013.09.23 End MES024 --%>
                        </tr>
                        <tr>
                            <td>
                                <asp:Literal runat="server" ID="Literal3" Text="<%$Resources:GlobalResources,STRING_PROCESS %>"> </asp:Literal>
                            </td>
                            <td>
                                <%--<asp:DropDownList ID="ddlDept" runat="server">
                                </asp:DropDownList>--%><%--<input type="button" value="..."
                                    onclick="searchDept()" />--%>
                                <asp:DropDownList ID="ddlDept" runat="server" Width="221px" 
                                    onselectedindexchanged="ddlDept_SelectedIndexChanged" AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                            <td class="style2" style="width: 173px">
                                <asp:Literal runat="server" ID="Literal9" Text="<%$Resources:GlobalResources,STRING_PRODUCTION_FACTORY %>"> </asp:Literal>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlWipProductFactory" runat="server" Width="221px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal11" 
                                    Text="<%$Resources:GlobalResources,STRING_LINE_NAME_STYLE %>"> </asp:Literal>
                            </td>
                            <td align="left">
                                <asp:CheckBox ID="ckbShowLineNameStyle" runat="server" AutoPostBack="True" 
                                    oncheckedchanged="ckbShowLineNameStyle_CheckedChanged" />
                            </td>
                            <td>
                                
                            </td>
                        </tr>
                        <%--Added By DaiJ ON 2013.09.23 Start MES024 --%>
                        <tr>
                            <td>
                                <asp:Literal runat="server" ID="Literal8" Text="<%$Resources:GlobalResources,STRING_PROCESS_TYPE %>"> </asp:Literal>
                            </td>
                            <td>                                
                                <asp:DropDownList ID="ddlWipProcessType" runat="server" AutoPostBack="True">
                                    <asp:ListItem ></asp:ListItem>

                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="Literal10" 
                                    Text="<%$Resources:GlobalResources,STRING_LINE_CODE %>"> </asp:Literal></td>
                            <td>                                
                                <div id="divCustomCheckBoxList" runat="server" onmouseover="clearTimeout(timoutID);"
                                    onmouseout="timoutID = setTimeout('HideMList()', 750);">
                                    <table>
                                        <tr>
                                            <td align="right" class="DropDownLook">
                                                <input id="txtSelectedMLValues" type="text" readonly="readonly" onclick="ShowMList()"
                                                    style="width: 195px;" runat="server" />
                                            </td>
                                            <td align="left" class="DropDownLook">
                                                <img id="imgShowHide" runat="server" src="drop.gif" 
                                                    onclick="ShowMList()" align="left" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" class="DropDownLook">
                                                <div>
                                                    <div runat="server" id="divCheckBoxListClose" class="DivClose">
                                                        <label runat="server" onclick="HideMList();" class="LabelClose" id="lblClose">
                                                            OK</label>
                                                        <label runat="server" class="LabelClear" id="lblClear">
                                                            Clear</label>
                                                    </div>
                                                    <div runat="server" id="divCheckBoxList" class="DivCheckBoxList">
                                                        <asp:CheckBoxList ID="lstMultipleValues" runat="server" Width="250px" CssClass="CheckBoxList">
                                                        </asp:CheckBoxList>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div></td>
                                <td>
                                <asp:Literal runat="server" ID="Literal6" Text="<%$Resources:GlobalResources,STRING_INC_OUTSOURCE %>"> </asp:Literal>
                            </td>
                                <td>
                                <asp:CheckBox ID="CBIsOutsource" runat="server" />
                            </td>
                            <td></td>
                        </tr>
                        <%--/Added By DaiJ ON 2013.09.23 Start MES024 --%>
                        <tr>
                            <td>
                                <asp:Literal runat="server" ID="Literal5" Text="<%$Resources:GlobalResources,STRING_PODATE %>"> </asp:Literal>
                            </td>
                            <td>
                                <asp:TextBox ID="txtBeginDate" runat="server" onfocus="WdatePicker({skin:'whyGreen'})"
                                    Width="94px"></asp:TextBox>&nbsp;To&nbsp;<asp:TextBox ID="txtEndDate" runat="server"
                                        onfocus="WdatePicker({skin:'whyGreen'})" Width="97px"></asp:TextBox>
                            </td>
                            <td class="style2" style="width: 173px">
                                <asp:RadioButtonList runat="server" ID="rblDate" RepeatDirection="Horizontal" Height="25px"
                                    Width="172px">
                                    <asp:ListItem Value="BPO" Selected="True">BPO Date</asp:ListItem>
                                    <asp:ListItem Value="PPC">PPC Date</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                            <td>
                                &nbsp;</td>
                            <td colspan="2">
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnWipQuery_Click"
                                    CssClass="button1" />&nbsp;&nbsp;<input type="button" class="button1" value="To Excel"
                                        onclick="toPaseExcel()" />&nbsp;&nbsp;<input type="button" class="button1" value="To WPS"
                                            onclick="toPaseWPS()" />
                            </td>
                            <td></td>
                        </tr>
                        <%--//Added By Zikuan MES093 3-Dec-13 --%>
                    </table>
                </td>
            </tr>
        </table>
        <hr />
        <div style="text-align:center; color:Red">
            <asp:Label ID="NODATA" runat="server" Font-Size="Medium" Font-Bold="True" Visible="false">NO DATA.</asp:Label>
        </div>
        <br />
        <div id="ExcTable">
            <asp:GridView ID="gvBody" runat="server" Width="100%" OnRowDataBound="gvBody_RowDataBound"
                BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                CellPadding="3" EnableViewState="False">
                <RowStyle ForeColor="#000066" />
                <FooterStyle BackColor="White" ForeColor="#000066" />
                <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#0082c6" Font-Bold="True" ForeColor="White" />
            </asp:GridView>
        </div>
        <%--<table width="100%" border="1" cellpadding="0" cellspacing="0" style="border-collapse: collapse"
            id="tbWip" runat="server" visible="false">
            <tr>
                <td colspan="2">
                    <div id="divSummary" runat="server">
                    </div>
                </td>
            </tr>
            <tr>
                <td style="width: 7%" valign="top">
                    <asp:DataList ID="dlWipJo" runat="server" Width="100%" OnSelectedIndexChanged="dlWipJo_SelectedIndexChanged">
                        <HeaderTemplate>
                            <div style="text-align: center">
                                PO NO</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            &nbsp;&nbsp;<asp:LinkButton ID="lbJo" CommandName="Select" runat="server" Text='<%#Eval("JOB_ORDER_NO")%>'></asp:LinkButton>
                        </ItemTemplate>
                    </asp:DataList>
                    <div style="width: 100%; text-align: center" runat="server" id="divWipEmpty" visible="false">
                        <font color='red'>无明细数据</font>
                    </div>
                </td>
                <td valign="top" height="260px">
                    <div id="divWip" runat="server">
                    </div>
                    <iframe id="frame" name="Detail" width="100%" height="245px" runat="server" visible="false"></iframe>
                </td>
            </tr>
        </table>--%>
    </div>
    </form>
</body>
</html>
