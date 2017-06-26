using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;

public partial class Reports_wipDaily : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlFtyCd.DataSource = MESComment.MesRpt.GetFactoryCd();
            ddlFtyCd.DataTextField = "FACTORY_ID";
            ddlFtyCd.DataValueField = "FACTORY_ID";
            ddlFtyCd.DataBind();
        }
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() == "DEV")
            {
                ddlFtyCd.SelectedValue = "GEG";
            }
            else
            {
                ddlFtyCd.SelectedValue = Request.QueryString["site"].ToString();
            }
        }
    }
    private bool CheckQueryCondition()
    {
        string strBeginDate = this.txtStartDate.Text.Trim();
        string strEndDate = this.txtToDate.Text.Trim();
        if ((strBeginDate.Length == 0 && strEndDate.Length == 0) && txtJoNo.Text.Length == 0)
        {
            this.lblMessage.Text = "Please choose condition to query!";
            this.lblMessage.Visible = true;
            this.gvBody.Visible = false;
            return false;
        }

        return true;
    }
    private void SetQuery()
    {
        if (true == this.CheckQueryCondition())
        {
            string strContent = "";

            Random ro = new Random(1000);
            strContent = Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();

            this.lblMessage.Visible = false;
            this.gvBody.Visible = true;
            this.gvBody.DataSource = MESComment.Employeeoutput.GetEmpOutputCheckData(ddlFtyCd.SelectedItem.Value, txtStartDate.Text, txtToDate.Text, txtJoNo.Text, txtOperCd.Text, ChkBalanceLess0.Checked, ChkIncludeRework.Checked, strContent);
            this.gvBody.DataBind();


            if (gvBody.Rows.Count > 0)
            {
                ExcTable.Visible = true;
                lblMessage.Visible = false;
            }
            else
            {
                ExcTable.Visible = false;
                lblMessage.Text = "Not Found any  data!";
                lblMessage.Visible = true;
            }
        }

    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.lblMessage.Visible = false;
        SetQuery();


    }

}



