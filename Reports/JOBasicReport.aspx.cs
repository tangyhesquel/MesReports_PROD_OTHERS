using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


public partial class Reports_JOSAHReport : pPage
{
    string Factory_CD = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                Factory_CD = Request.QueryString["site"].ToString();
            }
            else
            {
                Factory_CD = "GEG";
            }
        }
        if (!IsPostBack)
        {
            ddlFactory.DataSource = MESComment.MesRpt.GetFactoryCd();
            ddlFactory.DataTextField = "FACTORY_ID";
            ddlFactory.DataValueField = "FACTORY_ID";
            ddlFactory.DataBind();
            ddlFactory.SelectedValue = Factory_CD;

        }

    }
    private bool CheckQueryCondition()
    {
        string StartDate = txtStartDate.Text.Trim();
        string EndDate = txtToDate.Text.Trim();
        if (StartDate.Length <= 0 && EndDate.Length <= 0)
        {
            if ((txtJoNo.Text == null || txtJoNo.Text.Trim() == "") && (txtGONo.Text == null || txtGONo.Text.Trim() == ""))
            {
                this.lblMessage.Text = "Please input JO or GO or Date info to query!";
                this.lblMessage.Visible = true;
                this.gvBody.Visible = false;
                return false;
            }
        }
        if ((StartDate.Length <= 0 && EndDate.Length > 0) || (StartDate.Length > 0 && EndDate.Length <= 0))
        {
            this.lblMessage.Text = "Please input Complete date info !";
            this.lblMessage.Visible = true;
            this.gvBody.Visible = false;
            return false;
        }
        if (StartDate.Length > 0 && EndDate.Length > 0)
        {
            int SDate = Convert.ToInt32(StartDate.Replace("-", ""));
            int EDate = Convert.ToInt32(EndDate.Replace("-", ""));
            if (EDate < SDate)
            {
                //提示enddate必须要大于或等于startdate
                this.lblMessage.Text = "Date From cannot after Date To ! ";
                this.lblMessage.Visible = true;
                this.gvBody.Visible = false;
                return false;
            }
        }
        return true;
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.lblMessage.Visible = false;
        if (true == this.CheckQueryCondition())
        {
            SetData();
        }
    }
    protected void SetData()
    {
        string JONO = txtJoNo.Text.Trim();
        string GONO = txtGONo.Text.Trim();
        string Factory_CD = ddlFactory.SelectedItem.Value;
        string StartDate = txtStartDate.Text;
        string ToDate = txtToDate.Text;
        bool NoCut = this.cbNoCut.Checked;
        DataTable dt = MESComment.MesRpt.GetJOSAH(JONO, GONO, Factory_CD, StartDate, ToDate, NoCut);
        if (dt.Rows.Count > 0)
        {
            this.dvMsg.Visible = false;
            this.gvBody.Visible = true;
            gvBody.DataSource = dt;
            gvBody.DataBind();
        }
        else
        {
            this.gvBody.Visible = false;
            this.dvMsg.Visible = true;
        }
    }
}
