using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_Cutting : pPage
{
    public string scNo;
    public string joNo;
    public string styleNo;
    public string gmtDelDate;
    public string printDate;
    public string season;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlFactory.DataSource = MESComment.MesRpt.GetFactoryCd();
            ddlFactory.DataTextField = "FACTORY_ID";
            ddlFactory.DataValueField = "FACTORY_ID";
            ddlFactory.DataBind();
        }
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                ddlFactory.SelectedValue = Request.QueryString["site"].ToString();
            }
            else
            {
                ddlFactory.SelectedValue = "GEG";
            }
        }
        lblNoData.Visible = false;
        divDetail.Visible = true;
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        DataTable dt = MESComment.WipCutting.GetCuttingHead(txtJoNo.Text, ddlFactory.SelectedItem.Value);

        if (dt.Rows.Count == 0)
        {
            lblNoData.Visible = true;
            divDetail.Visible = false;
            return;

        }
        else
        {
            scNo = dt.Rows[0]["scNo"].ToString();
            joNo = dt.Rows[0]["joNo"].ToString();
            styleNo = dt.Rows[0]["styleNo"].ToString();
            gmtDelDate = dt.Rows[0]["gmtDelDate"].ToString();
            printDate = dt.Rows[0]["printDate"].ToString();
            season = dt.Rows[0]["season"].ToString();
        }
        DataTable dtDetail = MESComment.WipCutting.GetCuttingDetail(txtJoNo.Text, ddlFactory.SelectedItem.Value);
        gvCutDeail.DataSource = dtDetail;
        gvCutDeail.DataBind();
    }


}
