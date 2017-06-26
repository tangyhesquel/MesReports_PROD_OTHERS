<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mesSpreadingPlan.aspx.cs"
    Inherits="Mes_mesSpreadingPlan" CodePage="65001" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Spreading & Allocation Planning Report</title>
    <script type="text/javascript" language="javascript">
        function init() {
            if (document.all.txtMoNo.value == '') {
                //document.all.ExcTable.style.display = 'none';
                //                document.all.ExcTableEGM.style.display = 'none';
            }
        }
        function searchMarkerNo() {
            var urlName = "mesSearchMarkerNo.aspx?site=" + document.all.TxtFactroyCd.value + "&svType=" + document.all.HiddenField1.value;
            var markerNo = window.showModalDialog(urlName, "Marker No.", "dialogWidth=450pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (markerNo == null) return;
            document.all.txtMoNo.value = markerNo;
        }

        function searchJoNumber() {
            if (form1.txtMoNo.value == '') { alert('Please choose the Marker Order No.'); return false; }
            var moNo = document.all.txtMoNo.value;
            var urlName = "mesSearchJoNumber.aspx?markerNo=" + moNo + "&site=" + document.all.TxtFactroyCd.value + "&svType=" + document.all.HiddenField1.value;
            var markerNo = window.showModalDialog(urlName, "Marker No.", "dialogWidth=450pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
            if (markerNo == null) return;
            document.all.txtMarkerId.value = markerNo;
        }

        function validate() {
            if (form1.txtMoNo.value == '') { alert('Please choose the Marker Order No.'); return false; }
            else {
                return true;
            }
        }
        
    </script>
    <link href="../Css/StyleReport.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Script/Common.js"></script>
</head>
<body onload="init()">
    <form id="form1" runat="server">
    <div id="queryDiv">
        <fieldset>
            <legend>Search</legend>
            <table width="100%" id="queryTab">
                <tr>
                    <td class="tdbackcolor">
                        Factory:
                    </td>
                    <td>
                        <asp:TextBox ID="TxtFactroyCd" runat="server" Enabled="False"></asp:TextBox>
                    </td>
                    <td class="tdbackcolor">
                        Marker Order No:
                    </td>
                    <td>
                        <asp:TextBox ID="txtMoNo" runat="server"></asp:TextBox>
                        <input type="button" value="..." onclick="searchMarkerNo()" class="button_top" />
                    </td>
                    <td class="tdbackcolor">
                        Marker Id:
                    </td>
                    <td>
                        <asp:TextBox ID="txtMarkerId" runat="server"></asp:TextBox>
                        <input type="button" value="..." onclick="searchJoNumber()" class="button_top" />
                    </td>
                    <td class="tdbackcolor">
                        Language :
                    </td>
                    <td>
                        <asp:DropDownList ID="FlagList" runat="server" AutoPostBack="false">
                            <asp:ListItem Value="English">English</asp:ListItem>
                            <asp:ListItem Value="Chinese">中文(Chinese)</asp:ListItem>
                            <asp:ListItem Value="Vietnamese">Việt(Vietnamese)</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="cbSort" runat="server" Text="Order by Color" Visible="true" />
                    </td>
                    <td>
                        <asp:CheckBox ID="cbNewFormat" runat="server" Text="New Format" Visible="true"
                            Checked="true" />
                    </td>
                    <td>
                        <asp:CheckBox ID="cbMergSize" runat="server" Text="Show Merge Sizes" Visible="false"
                            Checked="true" />
                    </td>
                    <td>
                        <asp:CheckBox ID="cbPage" runat="server" Text="Print Blank Roll Page" />
                    </td>
                    <td>
                        <asp:CheckBox ID="cbFabric" runat="server" Text="Print Fabric Detail" />
                    </td>
                    <td>
                        <asp:CheckBox ID="cbBnm" runat="server" Text="Show Buyers Name" />
                    </td>
                    <td colspan="2">
                        <asp:CheckBox ID="cbNA" runat="server" Text="Show New Allocation by Priorities" Enabled="false" />
                    </td>
                    <td align="right">
                        <asp:Button ID="btnQuery" runat="server" Text="Query" CssClass="button_top" OnClientClick="return validate()"
                            OnClick="btnQuery_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <hr noshade="noshade" size="1" />
    </div>
    <table id="ExcTable" width="100%" border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td align="center">
                <font face="Arial" size="4">Spreading & Allocation Planning Report</font>
            </td>
        </tr>
        <tr>
            <td>
                <div id="mmPrint" style="text-align: right">
                    <input type="button" name="print" value="Print" onclick="javacript:document.all.queryDiv .style.display='none';document.all.mmPrint.style.visibility='hidden';window.print();document.all.queryDiv .style.display='block';document.all.mmPrint.style.visibility='visible'"
                        style="height: 26" class="button_top" />
                    <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,document.title+'.htm')"
                        style="height: 26" class="button_top" />
                    <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="height: 26"
                        class="button_top" />
                    <input type="button" name="excel" value="To Wps" onclick="toPaseWPS()" style="height: 26"
                        class="button_top" />
                </div>
            </td>
        </tr>
        <tr style="width: 100%">
            <td align="center" style="color: Red">
                <asp:Label ID="NODATA" runat="server" Font-Size="Medium" Font-Bold="True" Visible="false">NO DATA.</asp:Label>
            </td>
        </tr>
        <div id="divBody" runat="server">
        </div>
        <div id="divBottom" runat="server" style="width: 100%">
        </div>
    </table>
    <asp:HiddenField ID="HiddenField1" runat="server" />
    </form>
</body>
</html>
