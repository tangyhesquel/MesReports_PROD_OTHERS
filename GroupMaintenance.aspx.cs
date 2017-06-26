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
using System.Text;

public partial class Reports_GroupMaintenance : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Expires = 0;
        Response.Cache.SetNoStore();
        Response.AppendHeader("Pragma", "no-cache");
        setDataBind();
    }

    private void setDataBind()
    {
        gvHandle.DataSource = MESComment.MesRpt.Get_Group_Detail("ByHandler", Session["UserName"].ToString());
        gvHandle.DataBind();
        gvSystem.DataSource = MESComment.MesRpt.Get_Group_Detail("BySystem", Session["UserName"].ToString());
        gvSystem.DataBind();
    }
    protected void gvHandle_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        MESComment.MesRpt.Delete_Group_By_GroupName(gvHandle.Rows[e.RowIndex].Cells[0].Text,"ByHandler");
        setDataBind();
    }
    protected void gvSystem_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        MESComment.MesRpt.Delete_Group_By_GroupName(gvSystem.Rows[e.RowIndex].Cells[0].Text,"BySystem");
        setDataBind();
    }
    protected void gvHandle_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[1].Attributes.Add("onclick", "window.showModalDialog('AddGroupByHandler.aspx?id="+e.Row.Cells[0].Text+"&type=E','','dialogWidth=400px;');");
        }
    }
    protected void gvSystem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[1].Attributes.Add("onclick", "window.showModalDialog('AddGroupBySystem.aspx?id=" + e.Row.Cells[0].Text + "&type=E','','dialogWidth=400px;');");
        }
    }
    protected void Unnamed1_Click(object sender, EventArgs e)
    {
        Response.Write("<script language=\"javascript\"> window.returnValue='Y';window.close();</script>");
    }
}
