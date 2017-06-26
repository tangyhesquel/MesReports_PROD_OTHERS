<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ExportDataUserControl.ascx.cs" Inherits="UserControls_ExportDataUserControl" %>
<link type="text/css"  rel="Stylesheet" href='<%= ResolveUrl("~/Css/StyleReport.css") %>' /> 
<link type="text/css"  rel="Stylesheet" media="print" href='<%= ResolveUrl("~/Css/StyleReport_Print.css") %>' /> 
<script type="text/javascript" language="javascript" src='<%= ResolveUrl("~/Script/Common.js") %>'></script>

<div id="mmPrint" align="right" style="float:right">    
    <input type="button" name="print" value="Print" onclick="window.print();"
                            style="height: 26" class="button_top HiddenWhilePrint" />
    <input type="button" name="Save" value="Save" onclick="document.execCommand('SaveAs',false,document.title+'.htm')"
                            style="height: 26" class="button_top HiddenWhilePrint"/>
    <input type="button" name="excel" value="To Excel" onclick="toPaseExcel()" style="height: 26"
                            class="button_top HiddenWhilePrint" />
    <input type="button" name="wps" value="To WPS" onclick="toPaseWPS()" style="height: 26"
                            class="button_top HiddenWhilePrint" />
</div>