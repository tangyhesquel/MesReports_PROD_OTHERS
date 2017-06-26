using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Reports_queryContract : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlSubcontractor.DataSource = MESComment.MesRpt.GetSubcontractor();
            ddlSubcontractor.DataBind();
            ddlfactoryCd.DataSource = MESComment.MesRpt.GetFactoryList("");
            ddlfactoryCd.DataBind();
        }
        if (Request.QueryString["site"] != null)
        {
            ddlfactoryCd.SelectedValue = Request.QueryString["site"].ToString();
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        gvBody.DataSource = MESComment.MesRpt.GetQueryContract(ddlfactoryCd.SelectedItem.Value,txtcontractNo.Text.Trim(),ddlSubcontractor.SelectedItem.Value,txtfromDate.Text,txtToDate.Text,txtJoNo.Text.Trim());
        gvBody.DataBind();
    }
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.DataRow:
                e.Row.Cells[0].Attributes.Add("onclick", "returnResult('"+e.Row.Cells[0].Text+"')");
                break;
        }
    }
}
