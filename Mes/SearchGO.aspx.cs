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

public partial class Mes_SearchGO : System.Web.UI.Page
{
    string factoryCd = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["site"] != null)
        {
            factoryCd = Request.QueryString["site"].ToString();
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        gvBody.DataSource = MESComment.MesRpt.GetProTranWIPSummaryJoOrGo(factoryCd,"",txtSCNo.Text.Trim());
        gvBody.DataBind();
    }
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                break;
            case DataControlRowType.DataRow:
                e.Row.Cells[0].Text = "<input type='radio' name='rowsCheck' onclick='chooseJo()' class='noborder' value=" + e.Row.Cells[1].Text + " />";
                break;
        }
    }
}
