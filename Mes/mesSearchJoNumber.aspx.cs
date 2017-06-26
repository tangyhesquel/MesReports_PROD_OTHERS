using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Mes_mesSearchJoNumber : pPage
{
    string markerNo = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        divList.InnerHtml = "";
        markerNo=Request.QueryString["markerNo"];
        DataTable dtResult = MESComment.MesOutSourcePriceSql.GetCutPlanSearchJoNumber(markerNo);
        foreach (DataRow row in dtResult.Rows)
        {
            divList.InnerHtml += "<tr>";
            divList.InnerHtml += "<TD><input type='radio' name='rowsCheck' onclick='chooseJoNumber()' value="+row["MARKER_ID"]+" ></TD>";
            divList.InnerHtml += "<td >" + row["MARKER_ID"] + "</td>";
            divList.InnerHtml += "</tr>";
        }
    }
}
