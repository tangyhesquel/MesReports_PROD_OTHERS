<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProTranWIPSummary.aspx.cs"
    Inherits="Mes_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <%=factoryCd %>&nbsp;<asp:Literal runat="server" ID="Literal4" Text="<%$Resources:GlobalResources,STRING_PRODUCTION_STATUS%>"> </asp:Literal>
    </title>
    <link rel="Stylesheet" href="../Css/Style.css" />
    <script type="text/javascript" src="../Script/mouseEvent.js"></script>
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
    </style>
</head>
<body topmargin="0">
    <form id="form1" runat="server">
    <asp:HiddenField ID="hfValue" runat="server" />
    <div>
        <%--<div style="text-align: right;">
            <h4>
                GEG即时生产数据查询</h4>
        </div>--%>
        <table width="100%" cellpadding="0" cellspacing="0" border="0">
            <%--<tr>
                <td style="height: 25px; background-color: Gray" colspan="2">
                </td>
            </tr>--%>
            <tr>
                <td style="height: 400px; width: 150px; background-color: #0082c6; text-align: center;
                    vertical-align: top">
                    <table>
                        <tr>
                            <td>
                                <asp:Image ID="Image1" Width="150px" Height="82px" runat="server" ImageUrl="~/Images/esquel.jpg" />
                            </td>
                        </tr>
                        <tr>
                            <td height="40px">
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <%--<a href="ProStatusQuery.aspx?site=<%=factoryCd %>" target="Body">生产状态<br />查询</a>--%>
                                <button id="Status" type="button" style="border: 1px solid #6FE7FF; text-align: left;
                                    width: 140px" onmouseover='ButtonOver()' onmouseout='ButtonOut()' onfocus="vbs:window.event.srcElement.blur"
                                    onmousedown="Buttondown()" onmouseup="ButtonUp()" onclick='Onclicked("Status","<%=factoryCd %>")'>
                                    &nbsp;<img src="../Images/Loss_Menu_1.gif" width="15" align="top">
                                    <asp:Literal runat="server" ID="Literal6" Text="<%$Resources:GlobalResources,STRING_PRODUCTION_INQUERY %>"> </asp:Literal></button>
                            </td>
                        </tr>
                        <tr>
                            <td height="5px">
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <%--<a href="ProStatusQuery.aspx?site=<%=factoryCd %>" target="Body">生产状态<br />查询</a>--%>
                                <button id="YMGStatus" type="button" style="border: 1px solid #6FE7FF; text-align: left;
                                    width: 140px" onmouseover='ButtonOver()' onmouseout='ButtonOut()' onfocus="vbs:window.event.srcElement.blur"
                                    onmousedown="Buttondown()" onmouseup="ButtonUp()" onclick='Onclicked("YMGStatus","<%=factoryCd %>")'>
                                    &nbsp;<img src="../Images/Loss_Menu_2.gif" width="15" align="top">
                                    <asp:Literal runat="server" ID="Literal5" Text="<%$Resources:GlobalResources,STRING_PRODUCTION_YMG_INQUERY %>"> </asp:Literal></button>
                            </td>
                        </tr>
                        <tr>
                            <td height="5px">
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <%--<a href="ProWipQuery.aspx?site=<%=factorycd %>" target="Body">WIP数<br />
                                    查询</a>--%>
                                <button id="Wip" type="button" style="border: 1px solid #6FE7FF; text-align: left;
                                    width: 140px" onmouseover='ButtonOver()' onmouseout='ButtonOut()' onfocus="vbs:window.event.srcElement.blur"
                                    onmousedown="Buttondown()" onmouseup="ButtonUp()" onclick='Onclicked("Wip","<%=factoryCd %>")'>
                                    &nbsp;<img src="../Images/Loss_Menu_3.gif" width="15" align="top">
                                    <asp:Literal runat="server" ID="Literal1" Text="<%$Resources:GlobalResources,STRING_WIP_INQUERY %>"> </asp:Literal></button>
                            </td>
                        </tr>
                        <tr>
                            <td height="5px">
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <button id="CutSewWip" type="button" style="border: 1px solid #6FE7FF; text-align: left;
                                    width: 140px" onmouseover='ButtonOver()' onmouseout='ButtonOut()' onfocus="vbs:window.event.srcElement.blur"
                                    onmousedown="Buttondown()" onmouseup="ButtonUp()" onclick='Onclicked("CutSewWip","<%=factoryCd %>")'>
                                    &nbsp;<img src="../Images/Loss_Menu_3.gif" width="15" align="top">
                                    <asp:Literal runat="server" ID="Literal7" Text="<%$Resources:GlobalResources,STRING_CUT_SEW_INQUERY %>"> </asp:Literal></button>
                            </td>
                        </tr>
                        <tr>
                            <td height="5px">
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <%--<a href="ProProduceQuery.aspx?site=<%=factoryCd %>" target="Body">产量<br />
                                    查询</a>--%>
                                <button id="Produce" type="button" style="border: 1px solid #6FE7FF; text-align: left;
                                    width: 140px" onmouseover='ButtonOver()' onmouseout='ButtonOut()' onfocus="vbs:window.event.srcElement.blur"
                                    onmousedown="Buttondown()" onmouseup="ButtonUp()" onclick='Onclicked("Produce","<%=factoryCd %>")'>
                                    &nbsp;<img src="../Images/Loss_Menu_4.gif" width="15" align="top">
                                    <asp:Literal runat="server" ID="Literal2" Text="<%$Resources:GlobalResources,STRING_TRANSACTION_OUT_INQUERY %>"> </asp:Literal></button>
                            </td>
                        </tr>
                        <tr>
                            <td height="5px">
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <%--<a href="ProProduceQuery.aspx?site=<%=factoryCd %>" target="Body">产量<br />
                                    查询</a>--%>
                                <button id="AcceptQty" type="button" style="border: 1px solid #6FE7FF; text-align: left;
                                    width: 140px" onmouseover='ButtonOver()' onmouseout='ButtonOut()' onfocus="vbs:window.event.srcElement.blur"
                                    onmousedown="Buttondown()" onmouseup="ButtonUp()" onclick='Onclicked("AcceptQty","<%=factoryCd %>")'>
                                    &nbsp;<img src="../Images/Loss_Menu_5.gif" width="15" align="top">
                                    <asp:Literal runat="server" ID="Literal3" Text="<%$Resources:GlobalResources,STRING_TRANSACTION_IN_INQUERY %>"> </asp:Literal></button>
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="vertical-align: top">
                    <iframe id="mainFrame" name="mainFrame" src="ProStatusQuery.aspx?site=<%=factoryCd %>"
                        width="100%" frameborder="0" height="600px"></iframe>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
