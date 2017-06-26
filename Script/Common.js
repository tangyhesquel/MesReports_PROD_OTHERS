
/////////////////////////////////////
/////Jquery AutoComplete
/////Update:2010-9-2
///////////////////////////////////

function DoAutoComplete(str_ , page_)
{
    $(document).ready(function() {
     $(str_).autocomplete(
     page_,
     {
        delay: 10, 
        minChars: 1,
        matchSubset: 1,
        matchContains: 1,
        cacheLength: 10,
        onItemSelect: selectItem,
        onFindValue: findValue,
        formatItem: formatItem,
        autoFill: true
     }
   );
 });
 }

function findValue(li) { 
   if( li == null ) return alert("No match!");  
   // if coming from an AJAX call, let's use the CityId as the value  
   if( !!li.extra ) var sValue = li.extra[0];  
   // otherwise, let's just display the value in the text box   
   else var sValue = li.selectValue;
   //alert("The value you selected was: " + sValue);

}

function selectItem(li) {
    findValue(li);
}

function formatItem(row) { 
   return row[0];  
   //return row[0] + " (id: " + row[1] + ")"
   //如果有其他参数调用row[1]，对应输出格式Sparta|896

}

//function lookupAjax() {
//   var oSuggest = $("#txtContractNO")[0].autocompleter;
//   oSuggest.findValue();
//   return false; 
//}
function ToExcelOfWPS() {
            var myExcel, myBook;
            try {
                myExcel = new ActiveXObject("Excel.Application");
            } catch (Exception) {
                try{
                myExcel = new ActiveXObject("ET.Application");
                }
                catch(Exception)
                {
                alert("Your Machine doesn't install related software,can't open this file.");
                 return;   
                }
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

function toPaseWPS() {
    var myExcel, myBook, softType;
    try {
        myExcel = new ActiveXObject("ET.Application");
        softType = "WPS";
    } catch (Exception) {

    }

    if (myExcel != null) {
//        window.clipboardData.setData("Text", document.all.ExcTable.outerHTML);
        var sel = document.body.createTextRange();
        sel.moveToElementText(ExcTable);
        sel.select();
        sel.execCommand("Copy");
        sel.execCommand("Unselect");
        myExcel.Visible = true;
        
        myBook = myExcel.Workbooks.Add();
        myBook.sheets(1).Paste();
    }
    else {
        alert("Your Machine doesn't install WPS,can't open this file.");
        }


}

//function toPaseWPS() {
//    var myExcel, myBook;
//    try {
//        myExcel = new ActiveXObject("ET.Application");
//    } catch (Exception) {
//        alert("Open WPS Application exception");
//        return;
//    }

//    if (myExcel != null) {
//        var sel = document.body.createTextRange();
//        sel.moveToElementText(ExcTable);
//        sel.select();
//        sel.execCommand("Copy");
//        sel.execCommand("Unselect");
//        myExcel.Visible = true;
//        myBook = myExcel.Workbooks.Add();
//        myBook.sheets("Sheet1").Paste();
//    }
//}





function toPaseExcel() {

//    var myExcel, myBook, softType;
//    try {
//        myExcel = new ActiveXObject("Excel.Application");
//        softType = "Excel";
//    } catch (Exception) {

//    }

//    if (myExcel != null) {
//        window.clipboardData.setData("Text", document.all.ExcTable.outerHTML);
//        myExcel.Visible = true;
//        myBook = myExcel.Workbooks.Add();
//        myBook.sheets(1).Paste();
//        myBook.sheets(1).UsedRange.Font.Size = 12;
//    }
//    else {
//        alert("Your Machine doesn't install MS Office,can't open this file.");
    //    }

    var myExcel, myBook;
    try {
        myExcel = new ActiveXObject("Excel.Application");
    } catch (Exception) {
        try {
            myExcel = new ActiveXObject("ET.Application");
        }
        catch (Exception) {
            alert("Your Machine doesn't install related software,can't open this file.");
            return;
        }
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


function toPcmExcel() {

    var myExcel, myBook, softType;
    try {
        myExcel = new ActiveXObject("Excel.Application");
        softType = "Excel";
    } catch (Exception) {

    }

    if (myExcel != null) {
        window.clipboardData.setData("Text", document.all.ExcTable.outerHTML);
        myExcel.Visible = true;
        myBook = myExcel.Workbooks.Add();
        myBook.sheets(1).Paste();
    }
    else {
        alert("Your Machine doesn't install MS Office,can't open this file.");
    }
}

