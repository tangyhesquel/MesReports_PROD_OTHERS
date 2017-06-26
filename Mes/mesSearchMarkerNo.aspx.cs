using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Mes_searchMarkerNo : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        gvBody.DataSource = MESComment.MesOutSourcePriceSql.GetCutPlanSearchMarkerNo(txtMoNo.Text.Trim(),txtJoNo.Text.Trim());
        gvBody.DataBind();
    }
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.DataRow:
                e.Row.Cells[0].Text = "<input type='radio' name='rowsCheck' onclick='chooseMarkerNo()' value="+e.Row.Cells[1].Text+" class='noborder' >";
                break;
        }
    }
}
