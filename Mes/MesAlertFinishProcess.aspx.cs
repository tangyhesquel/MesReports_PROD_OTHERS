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

public partial class Mes_MesAlertFinishProcess : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtDate.Text  = Request.QueryString.GetValues("date")[0];
            ddlTimes.Items.Add("DAILY");
            ddlTimes.Items.Add("NIGHT");
            ddlTimes.Text = Request.QueryString.GetValues("time")[0].ToUpper();
            DataTable dtDetail = MESComment.MesOutSourcePriceSql.GetAlertFinishProcess(txtDate.Text, ddlTimes.Text.ToUpper());
            GridView1.DataSource = dtDetail;
            GridView1.DataBind();
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        DataTable dtDetail = MESComment.MesOutSourcePriceSql.GetAlertFinishProcess(txtDate.Text, ddlTimes.Text.ToUpper());
        GridView1.DataSource = dtDetail;
        GridView1.DataBind();
    }

    protected void ddlTimes_onload(object sender, EventArgs e)
    {
        if ((!IsPostBack)==true)
        {
            ddlTimes.Text = Request.QueryString.GetValues("time")[0].ToUpper();
        }
    }
}
