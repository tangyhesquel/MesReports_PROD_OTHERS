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

public partial class Mes_MesOutsourcePriceByFty : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlView.Items.Clear();
            ddlView.Items.Add(new ListItem("Knit", "K"));
            ddlView.Items.Add(new ListItem("Woven", "W"));
            if(CurrentSite!=String.Empty)
                ddlFactory.DataSource = MESComment.MesOutSourcePriceSql.GetDDLFactoryData(CurrentSite);
            else
                ddlFactory.DataSource = MESComment.MesOutSourcePriceSql.GetDDLFactoryData("GEG");
            ddlFactory.DataTextField = "SUBCONTRACTOR_NAME";
            ddlFactory.DataValueField = "SUBCONTRACTOR_CD";
            ddlFactory.DataBind();

        }
        BuildTable();

    }
    private void BuildTable()
    {
        DataTable mst_tb = MESComment.MesOutSourcePriceSql.GetsourcePriceByFty(ddlView.SelectedValue,ddlFactory.SelectedItem.Value);
        DataTable tb = new DataTable();
        tb.Columns.Add("Year", typeof(System.String));
        tb.Columns.Add("P1", typeof(System.Decimal));
        tb.Columns.Add("P2", typeof(System.Decimal));
        tb.Columns.Add("P3", typeof(System.Decimal));
        tb.Columns.Add("P4", typeof(System.Decimal));

        DataRow row = null;
        DataRow[] rows = null;
        for (int i = DateTime.Now.Year - 1; i < DateTime.Now.Year + 10; i++)
        {
            rows = mst_tb.Select(string.Format("year='{0}'", i.ToString()));
            row = tb.NewRow();
            row["Year"] = i;
            if (rows.Length > 0)
            {
                row["P1"] = rows[0]["pcs_price"];
                row["P2"] = rows[0]["order_qty"];
                row["P3"] = rows[0]["sah_price"];
                row["P4"] = rows[0]["sah"];

            }
            tb.Rows.Add(row);


        }
        gdPrice.DataSource = tb;
        gdPrice.DataBind();
    }
    protected void gdPrice_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                e.Row.Cells[0].Text = "年分";
                e.Row.Cells[1].Text = "预算单价(RMB)";
                e.Row.Cells[2].Text = "预算外发数量(Pcs)";
                e.Row.Cells[3].Text = "预算单价/SAH";
                e.Row.Cells[4].Text = "预算平均SAH";
                break;
            case DataControlRowType.DataRow:
                e.Row.Attributes.Add("OnDblClick", string.Format("gdDBClick({0})", e.Row.Cells[0].Text));
                e.Row.Attributes.Add("onclick", string.Format("SetSelYear({0})", e.Row.Cells[0].Text));
                break;
        }
    }
}
