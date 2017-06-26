using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Reports_SearchJO : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        gvBody.DataSource = MESComment.MesRpt.GetJO(txtTrxDate.Text,ddlStatus.SelectedItem.Value,txtCustomer.Text.Trim(),txtSCNo.Text.Trim(),CurrentSite.ToString());
        gvBody.DataBind();
    }
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                break;
            case DataControlRowType.DataRow:
                e.Row.Cells[0].Text = "<input type='radio' name='rowsCheck' onclick='chooseJo()' class='noborder' value="+e.Row.Cells[1].Text+" />";
                break;
        }
    }
}
