using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Mes_Search : pPage
{
    string Type = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        Type = Request.QueryString["type"].ToString();
        if (!IsPostBack)
        {
            switch (Type)
            {
                case "F":
                    lblCode.Text = "Contractor Code：";
                    lblName.Text = "Contractor Name：";
                    lblMark.Text = "Address：";
                    break;
                case "C":
                    lblCode.Text = "Customer Code：";
                    lblName.Text = "Customer Name：";
                    lblMark.Text = "Short Name：";
                    break;
            }
        }
        if (Request.QueryString["single"] != null)
        {
            gvBody.Columns[0].Visible = false;
            btnSelect.Visible = false;
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        DataTable dt = new DataTable();
        DataTable Source=new DataTable();
        dt.Columns.Add("checked");
        dt.Columns.Add("code");
        dt.Columns.Add("name");
        dt.Columns.Add("mark");
        if (gvBody.Rows.Count > 0)
        {
            for (int i = 0; i < gvBody.Rows.Count; i++)
            {
                CheckBox cb = (CheckBox)gvBody.Rows[i].FindControl("cbCheck");
                if (cb.Checked)
                {
                    dt.Rows.Add('Y',gvBody.Rows[i].Cells[1].Text.ToString(), gvBody.Rows[i].Cells[2].Text.ToString(), gvBody.Rows[i].Cells[3].Text.ToString());
                }
            }
        }
        Source=MESComment.MesOutSourcePriceSql.GetMasOutSourceFactoryOrCustomer(Type, txtCode.Text, txtName.Text, txtMark.Text);
        if(Source.Rows.Count>0&&dt.Rows.Count>0)
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
            case DataControlRowType.Header:
                switch (Type)
                {
                    case "F":
                        e.Row.Cells[1].Text = "Contractor Code";
                        e.Row.Cells[2].Text = "Contractor Name";
                        e.Row.Cells[3].Text = "Address";
                        break;
                    case "C":
                        e.Row.Cells[1].Text = "Customer Code";
                        e.Row.Cells[2].Text = "Short Name";
                        e.Row.Cells[3].Text = "Customer Name";
                        break;
                }
                break;
            case DataControlRowType.DataRow:
                e.Row.Attributes.Add("ondblclick", "window.returnValue='" + e.Row.Cells[1].Text + "'+';';window.close()");
                break;
        }
    }
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        string SelectValue = "";
        for (int i = 0; i < gvBody.Rows.Count; i++)
        {
            CheckBox cb=(CheckBox)gvBody.Rows[i].FindControl("cbCheck");
            if (cb.Checked)
            {
                SelectValue += gvBody.Rows[i].Cells[1].Text + ";";
            }
        }
        Response.Write("<script>window.returnValue='"+SelectValue+"';window.close()</script>");
    }
}
