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

public partial class Reports_AddGroupBySystem : System.Web.UI.Page
{
    string GroupName;
    char Type='A';
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Expires = 0;
        Response.Cache.SetNoStore();
        Response.AppendHeader("Pragma", "no-cache");

        Type = char.Parse(Request.QueryString["type"].ToString());
        if (Type == 'E')
        {
            GroupName = Request.QueryString["id"].ToString();
        }
        else
        {
            GroupName = "";
        }
        if (!IsPostBack)
        {
            txtGroupName.Text = GroupName;
            gvBody.DataSource = MESComment.MesRpt.Get_System_Detail(GroupName);
            gvBody.DataBind();
        }
    }
    public void btnSave_Click(object sender,EventArgs e)
    {
        if (Type == 'E')
        {
            MESComment.MesRpt.Delete_Group_By_GroupName(GroupName,"BySystem");
        }
        for (int i = 0; i < gvBody.Rows.Count; i++)
        {
            CheckBox cb = (CheckBox)gvBody.Rows[i].FindControl("cbSystem");
            if (cb.Checked)
            {
                MESComment.MesRpt.Add_Group_Detail(txtGroupName.Text, "ALL", int.Parse(gvBody.Rows[i].Cells[0].Text), Session["UserName"].ToString());
            }
        }
        Response.Write("<script>window.close()</script>");
    }
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        e.Row.Cells[0].Visible = false;
    }
}
