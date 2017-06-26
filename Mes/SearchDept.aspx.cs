using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Mes_SearchDept : System.Web.UI.Page
{
    string factoryCd = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                factoryCd = Request.QueryString["site"].ToString();
            }
            else
            {
                factoryCd = "GEG";
            }
        }
        if (!IsPostBack)
        {
            gvBody.DataSource = MESComment.MesRpt.GetProTranWIPSummaryDept(factoryCd);
            gvBody.DataBind();
        }
    }
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        string SelectValue = "";
        for (int i = 0; i < gvBody.Rows.Count; i++)
        {
            CheckBox cb = (CheckBox)gvBody.Rows[i].FindControl("cbCheck");
            if (cb.Checked)
            {
                SelectValue += gvBody.Rows[i].Cells[1].Text + ";";
            }
        }
        Response.Write("<script>window.returnValue='" + SelectValue + "';window.close()</script>");
    }
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.DataRow:
                e.Row.Attributes.Add("ondblclick", "window.returnValue='" + e.Row.Cells[1].Text + "'+';';window.close()");
                break;
        }
    }
}
