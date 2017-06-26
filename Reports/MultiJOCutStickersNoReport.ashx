<%@ WebHandler Language="C#" Class="MultiJOCutStickersNoReport" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Text;

public class MultiJOCutStickersNoReport : IHttpHandler, IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        //parse the query string
        if (string.IsNullOrEmpty(context.Request.QueryString.ToString()))
        {
            context.Response.Write("Invalid Query Condition!");
            context.Response.End();
        }

        string site1 = context.Session["site"].ToString() == "" ? "TIL" : context.Session["site"].ToString();

        string htmlResponse = "<table border='1' width='600px' cellspacing='0' cellpadding='0' style='margin-top:1.5em;font-size:12px;border-collapse:collapse' ><tr width='100%' style='font-weight:bold;' ><td class='tr2style_noScroll'>JO No:</td><td>" + context.Request["JO"] + " </td><td class='tr2style_noScroll'>Lay No</td><td>" + context.Request["LayNo"] + "</td></tr><tr><td class='tr2style_noScroll'>Production Line:</td><td> " + context.Request["line"] + "</td><td class='tr2style_noScroll'>Dlvy Date:</td><td> " + context.Request["Dvrydate"] + "</td></tr><tr><td class='tr2style_noScroll'>Table/Invoice No:</td><td> " + context.Request["INVOICE"] + "</td><td class='tr2style_noScroll'>Pattern No:</td><td> " + context.Request["TxtPtn"] + "</td></tr><tr><td class='tr2style_noScroll'>Remark:</td><td colspan='3' > " + context.Request["REMARK"] + "</td></tr></table><br/>";
        htmlResponse += "<table class='thinBorderWithoutTopBorder'><tr style='font-weight:bold;' ><td >JO_No &nbsp;</td><td>Bed </td><td>&nbsp;   Color</td><td>Plys &nbsp;</td><td>Size</td>";

        DataTable table = MESComment.MultiJOCutStickersNoReportSql.GetMultiJoBundleTicketData(site1, context.Request.QueryString.ToString());

        for (int i = 5; i < table.Columns.Count; i++)
        {
            htmlResponse += "<td  class='thinBorder'>" + table.Columns[i].Caption + "</td>";
        }
        htmlResponse += "</tr>";

        if (table.Rows.Count <= 0)
        {
            htmlResponse += "<tr><td>No Found Bundle Data!</td></tr>";
        }
        foreach (DataRow r in table.Rows)
        {
            htmlResponse += "<tr>";
            for (int i = 1; i < table.Columns.Count; i++)
            {


                if (i == 4)
                {
                    htmlResponse += "<td class='thinBorder' colspan='2'>";
                }
                else
                {
                    htmlResponse += "<td class='thinBorder'>";
                }


                htmlResponse += r[i].ToString() == "" ? "&nbsp" : r[i].ToString();

                htmlResponse += "</td>";
            }
            htmlResponse += "</tr>";
        }

        htmlResponse += "</table>";

        context.Response.Write(htmlResponse);
    }



    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}