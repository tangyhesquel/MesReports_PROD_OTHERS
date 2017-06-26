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

public partial class Mes_SearchStyle : pPage
{
    string factoryCd = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["site"] != null)
        {
            factoryCd = Request.QueryString["site"].ToString();
        }
        if (Request.QueryString["single"] != null)
        {
            gvBody.Columns[0].Visible = false;
            btnSelect.Visible = false;
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
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        DataTable dt = new DataTable();
        DataTable Source = new DataTable();
        dt.Columns.Add("checked");
        dt.Columns.Add("STYLE_NO");
        dt.Columns.Add("STYLE_DESC");
        if (gvBody.Rows.Count > 0)
        {
            for (int i = 0; i < gvBody.Rows.Count; i++)
            {
                CheckBox cb = (CheckBox)gvBody.Rows[i].FindControl("cbCheck");
                if (cb.Checked)
                {
                    dt.Rows.Add('Y', gvBody.Rows[i].Cells[1].Text.ToString(), gvBody.Rows[i].Cells[2].Text.ToString());
                }
            }
        }
        Source = MESComment.MesRpt.GetProTranWIPSummaryStyle(factoryCd, txtStyleNo.Text, txtStyleDesc.Text);
        if (Source.Rows.Count > 0 && dt.Rows.Count > 0)
        {
            for (int i = 0; i < Source.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    if (dt.Rows[j][1].ToString() == Source.Rows[i][1].ToString())
                    {
                        Source.Rows.RemoveAt(i);
                    }
                }
            }
        }
        if (Source.Rows.Count > 0)
        {
            dt.Merge(Source);
        }
        gvBody.DataSource = dt;
        gvBody.DataBind();
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
