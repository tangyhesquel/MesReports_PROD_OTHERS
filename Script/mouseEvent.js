function ButtonOver()
{
	var oSource
	oSource = window.event.srcElement
	oSource.style.color = "#000000"
	oSource.style.backgroundcolor = "#FFCF84"
	oSource.style.bordertopcolor = "#ffffff"
	oSource.style.borderleftcolor = "#ffffff"
	oSource.style.borderrightcolor = "#000000"
	oSource.style.borderbottomcolor = "#000000"
	oSource.style.textDecorationUnderline = false
}

function ButtonOut()
{
	var oSource
	oSource = window.event.srcElement
	oSource.style.color = "white"
	oSource.style.backgroundcolor = "#0082C6"
	oSource.style.bordercolor = "#8CDBFF"
	oSource.style.textDecorationUnderline = false
}

function Buttondown()
{

     var oSource

     oSource = window.event.srcElement

     oSource.style.color = "#000000"

     oSource.style.backgroundcolor = "#8CDBFF"

     oSource.style.bordertopcolor = "#000000"

     oSource.style.borderleftcolor = "#000000"

     oSource.style.borderrightcolor = "#ffffff"

     oSource.style.borderbottomcolor = "#ffffff"

     oSource.style.textDecorationUnderline = false

}

function ButtonUp()
{

     var oSource

     oSource = window.event.srcElement

     oSource.style.color = "#000000"

     oSource.style.backgroundcolor = "#FFCF84"

     oSource.style.bordertopcolor = "#ffffff"

     oSource.style.borderleftcolor = "#ffffff"

     oSource.style.borderrightcolor = "#000000"

     oSource.style.borderbottomcolor = "#000000"

     oSource.style.textDecorationUnderline = false

}

function Onclicked(ValueEd,siteValue)
{
    switch(ValueEd)
    {
         case "Status":

              mainFrame.location="ProStatusQuery.aspx?site="+siteValue;
              break;
         
         case "YMGStatus":
              mainFrame.location="ProYMGStatusQuery.aspx?site="+siteValue;
              break;
         
         case "Wip":
              mainFrame.location="ProWipQuery.aspx?site="+siteValue ;
              break;

          case "CutSewWip":
              mainFrame.location = "CuttingDcWIPDetailWoven.aspx?site=" + siteValue;
              break;

         case "Produce":

              mainFrame.location="ProProduceQuery.aspx?site="+siteValue;
              break;
         
         case "AcceptQty":
              mainFrame.location="ProAcceptQty.aspx?site="+siteValue;
              break;
    }
}

