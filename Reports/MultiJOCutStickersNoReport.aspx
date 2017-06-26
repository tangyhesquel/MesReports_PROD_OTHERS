<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MultiJOCutStickersNoReport.aspx.cs" Inherits="Reports_MultiJOCutStickersNoReport" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Cut Stickers NO. Report For Multi JO</title>
     <link rel="stylesheet" type="text/css" href="../StyleSheet.css" />
     <style media="print" type="text/css">
        .noPrint
        {
            display:none;
        }
            
     </style>
<script type="text/javascript" language="javascript" src="../Script/jquery-1.4.2.js"></script>
<script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
 <script type="text/javascript" language="javascript">
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
     function BoundClickEvent() {
         $(".RemoveBtn").click(function () { $($(this).parent().get(0)).fadeOut(700); });
         $(".searchJo").click(searchJoNew);
     }
     function UnBoundClickEvent() {
         $(".RemoveBtn").unbind("click")
         $(".searchJo").unbind("click");
     }


     function ShowAjaxEffect() {

         $("#ExcTable").html("<img src='../Images/ajax.gif' id='ajaxImage' alt='data is loading' style='position:relative; left:40%; top:10em; '/>");
      }

  

     function searchJoNew() {     
         var urlName = "searchJO.aspx?" +"userRandom=" + (Math.random() * 100000);
         var jo = window.showModalDialog(urlName, "Job Order No.", "dialogWidth=650pt;dialogHeight=380pt;center=yes;scrollbars=no;status=yes;help=no;resizable=yes;");
         if (jo == null) return;
         $($($(this).parent().get(0)).children().get(1)).val(jo);
     }

      function ConvertString2Int( s) { 
        if(s=="")
            s='*';
        return s;
     }

     function BuildFormQueryString() {
         var queryString = "";
         var JO ="";
         if (document.getElementsByName("TXTJO").length == 1) {
             JO = form1.TXTJO.value.toString();
         }
         else {
             JO = form1.TXTJO(0).value.toString();
         }

         var LayNo = ""

         if (document.getElementsByName("TXTLAY1").length == 1) {
             LayNo = form1.TXTLAY1.value.toString();
         }
         else {
             LayNo = form1.TXTLAY1(0).value.toString();
         }

         if (document.getElementsByName("TXTLAY2").length == 1) {
             LayNo += "-" + form1.TXTLAY2.value.toString();
         }
         else {
             LayNo += "-" + form1.TXTLAY2(0).value.toString();
         }               

         $(".SubQueryDiv:visible").each(function () { if ($(this).children(".txtJobOrder").attr("value") != "") { if (queryString != "") queryString += "&"; queryString += ($(this).children(".txtJobOrder").attr("value") + '-' + ConvertString2Int($(this).children(".txtFromBed").attr("value")) + '-' + ConvertString2Int($(this).children(".txtToBed").attr("value"))) } });
         
         if (queryString == "") {
            alert ("Invalid Query Condition!");
            return;
        }
        ShowAjaxEffect();
        $.get("MultiJOCutStickersNoReport.ashx?JO=" + JO + "&LayNo=" + LayNo + "&INVOICE=" + form1.txtInvoice.value.toString() + "&line=" + form1.txtLine.value.toString() + "&Dvrydate=" + form1.Txtdvrydate.value.toString() + "&TxtPtn=" + form1.TxtPtn.value.toString() + "&REMARK=" + form1.TxtRm.value.toString() + "&ff=SQLALL", queryString, function (result) { $("#ajaxImage").hide(); $("#ExcTable").html(result); }, "text");
         
     }

    

 </script>
</head>
<body style="font-family:'lucida grande',tahoma,helvetica,arial,'bitstream vera sans',sans-serif;" >
    <h3 style=" text-align:center;">Cut Stickers NO. Report For Multi JO</h3>
    <form id="form1" method="post"  >
        <div id="QueryDiv" class="noPrint">
            <div style="border-width:0px; border-bottom-width:0px; border-style:none; padding:0em; border-collapse:collapse" 
                class="SubQueryDiv">
                <label title="" >Job Order:</label>
                <input style="border-width:1px; border-style:solid" type="text"  class="txtJobOrder"  id="TXTJO" />
                <input  class="searchJo" value="..." name="" type="button"  />   

                <label  title="" style="margin-left:3em" >From Bed NO.</label>
                <input id="TXTLAY1" style="border-width:1px; border-style:solid; width:5em;" type="text" class="txtFromBed"  />

                <label title="" >To Bed NO.</label>
                <input style="border-width:1px; border-style:solid;width:5em;" type="text" class="txtToBed" id="TXTLAY2" />

                <input type="button"  value="Remove" class="RemoveBtn" name="removeDiv"  />
            </div>
        </div>  
     
        <div style="top:2em" class="noPrint">
         <label title="" >Production Line:</label>
             <input style="border-width:1px;margin-top:3px; border-style:solid;margin-left:1em; width:11em;" type="text" id="txtLine" /> 
             <label title="" style="margin-left:3em">Table/Invoice No:</label>
             <input style="border-width:1px; border-style:solid;margin-left:4px; width:185px;" type="text"  id="txtInvoice" /> 
             <br /><label title="" >Develivery Date:</label>
             <input style="border-width:1px; margin-top:3px; border-style:solid;margin-left:1em;" type="text" onclick="WdatePicker({skin:'whyGreen'})"  id="Txtdvrydate" /> 
             <label title="" style="margin-left:3em">Pattern No.</label>
             <input style="border-width:1px; border-style:solid;margin-left:40px; width:185px;" type="text"  id="TxtPtn" /> 
             <br /> <label title="">Remarks:</label>
             <input style="border-width:1px; margin-top:3px; border-style:solid;margin-left:50px;width:488px;" type="text"  id="TxtRm" /> 
        </div>
        <div class="noPrint">             
            <input type="button" class="btnQuery" name="btnQuery"   value="Query" alt="Query"  style="width:10em; float:left"  />            
            <input type="button" class="btnAddBundle" name="btnAddBundle"    value="Add Job Order"  style="width:10em;float:left"/>
            <input type="button" class="btnRemoveAll" name="btnRemoveAll"  value="Remove All" alt="Remove All"  style="width:10em;float:left" />             
             <input type="button" class="" name="btnPrint"   value="Print" alt="Print"  style="width:10em;float:left"  onclick="window.print();"/>  
             <input type="button" name="excel" value="To Excel"
							onclick="toPaseExcel()" style="font-size:16;width:80;height:26"/><input 
                            type="button" onclick="ToExcelOfWPS()" value="To WPS"
							 style="font-size:16;width:80;height:26"/>
         </div>          
    </form>

    
    <div id="ExcTable" style=" clear:both;" >
         
    </div>

    <script language="javascript" type="text/javascript">
        $(".RemoveBtn").click(function () { $($(this).parent().get(0)).fadeOut(500); });
        $(".btnRemoveAll").click(function () { $(".SubQueryDiv").hide(); })
        $(".btnAddBundle").click(function () { $($(".SubQueryDiv").get(0)).clone().appendTo($("#QueryDiv")).show().children("input:text").val(""); UnBoundClickEvent(); BoundClickEvent(); });
        $(".searchJo").click(searchJoNew);
        $(".btnQuery").click(function () { BuildFormQueryString(); })
</script>
</body>
</html>
