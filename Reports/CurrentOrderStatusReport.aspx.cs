using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;

public partial class Reports_CurrentOrderStatusReport : pPage
{
    //Created by Zikuan - MES-097
    public string strRunNO;

    protected void Page_Load(object sender, EventArgs e)
    {
        ddlfactoryCd.DataSource = MESComment.MesRpt.GetFactoryCd();
        ddlfactoryCd.DataTextField = "FACTORY_ID";
        ddlfactoryCd.DataValueField = "FACTORY_ID";
        ddlfactoryCd.DataBind();
        if (base.CurrentSite != "")
        {
            if (string.Compare(base.CurrentSite, "DEV", true) == 0)
                ddlfactoryCd.SelectedValue = "GEG";
            else
                ddlfactoryCd.SelectedValue = base.CurrentSite;
        }
    }

    protected void cbJO_CheckedChanged(object sender, EventArgs e)
    {
        txtjobOrderNo.Focus();
        txtjobOrderNo.Text = string.Empty;
        txtjobOrderNo.Enabled = true;

        if (cbJOwithWIP.Checked)
        {
            cbJOwithWIP.Checked = false;
        }
        if (cbShipped.Checked)
        {
            cbShipped.Checked = false;
        }
        txtBeginDate.Text = string.Empty;
        txtEndDate.Text = string.Empty;
        txtBeginDate.Enabled = true;
        txtEndDate.Enabled = true;
    }
    protected void cbJOwithWIP_CheckedChanged(object sender, EventArgs e)
    {
        txtBeginDate.Text = string.Empty;
        txtEndDate.Text = string.Empty;
        txtBeginDate.Enabled = false;
        txtEndDate.Enabled = false;

        if (cbJO.Checked)
        {
            cbJO.Checked = false;
        }
        if (cbShipped.Checked)
        {
            cbShipped.Checked = false;
        }
        txtjobOrderNo.Text = string.Empty;
        txtjobOrderNo.Enabled = false;
    }
    protected void cbShipped_CheckedChanged(object sender, EventArgs e)
    {
        txtBeginDate.Text = string.Empty;
        txtEndDate.Text = string.Empty;
        txtBeginDate.Enabled = true;
        txtEndDate.Enabled = true;

        if (cbJO.Checked)
        {
            cbJO.Checked = false;
        }
        if (cbJOwithWIP.Checked)
        {
            cbJOwithWIP.Checked = false;
        }
        txtjobOrderNo.Text = string.Empty;
        txtjobOrderNo.Enabled = false;
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.lblMessage.Visible = false;
        if (CheckQueryCondition())
        {
            SetData();
        }
    }

    private void SetData()
    {
        Random ro = new Random(1000);
        strRunNO = Convert.ToString(string.Format("{0:yy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(10).ToString();

        DbConnection MESConn = MESComment.DBUtility.GetConnection("MES");

        if (cbJO.Checked || cbShipped.Checked)
        {
            DbConnection invConn = MESComment.DBUtility.GetConnection("INV");
            DataTable dt = MESComment.CurrentOrderStatus.GetShipInfo(txtjobOrderNo.Text, txtBeginDate.Text, txtEndDate.Text, invConn);
            MESComment.DBUtility.CloseConnection(ref invConn);

            MESComment.DBUtility.InsertTempShipInfoData(dt, strRunNO, MESConn);
        }

        DataTable resultDt = new DataTable();
        if (cbJOwithWIP.Checked)
        {
            resultDt = MESComment.CurrentOrderStatus.GetCurrentOrderStatusInfo(strRunNO, txtjobOrderNo.Text, txtBeginDate.Text, txtEndDate.Text, "JOWithWIP", MESConn, ddlfactoryCd.SelectedValue);
        }
        else
        {
            resultDt = MESComment.CurrentOrderStatus.GetCurrentOrderStatusInfo(strRunNO, txtjobOrderNo.Text, txtBeginDate.Text, txtEndDate.Text, "Others", MESConn, ddlfactoryCd.SelectedValue);
        }
        MESComment.DBUtility.CloseConnection(ref MESConn); 

        if (resultDt.Rows.Count > 0)
        {
            gvBody.DataSource = resultDt;
            gvBody.DataBind();
        }
        else
        {
            this.lblMessage.Text = "NO DATA.";
            this.lblMessage.Visible = true;
            gvBody.DataSource = null;
            gvBody.DataBind();
        }
    }

    private bool CheckQueryCondition()
    {
        string StartDate = txtBeginDate.Text.Trim();
        string EndDate = txtEndDate.Text.Trim();

        if (cbJO.Checked)
        {
            if (string.IsNullOrEmpty(txtjobOrderNo.Text))
            {
                this.lblMessage.Text = "Please input JO to query!";
                this.lblMessage.Visible = true;
                txtjobOrderNo.Focus();
                return false;
            }
        }
        else if (cbShipped.Checked)
        {
            if (StartDate.Length <= 0 || EndDate.Length <= 0)
            {
                this.lblMessage.Text = "Please input from and to date info !";
                this.lblMessage.Visible = true;
                return false;
            }
            if (StartDate.Length > 0 && EndDate.Length > 0)
            {
                int SDate = Convert.ToInt32(StartDate.Replace("-", ""));
                int EDate = Convert.ToInt32(EndDate.Replace("-", ""));
                if (EDate < SDate)
                {
                    this.lblMessage.Text = "Date From cannot after Date To ! ";
                    this.lblMessage.Visible = true;
                    return false;
                }
            }
        }

        if ((StartDate.Length > 0 && EndDate.Length <= 0) || (StartDate.Length <= 0 && EndDate.Length > 0))
        {
            this.lblMessage.Text = "Please input from and to date info !";
            this.lblMessage.Visible = true;
            return false;
        }
        if (StartDate.Length > 0 && EndDate.Length > 0)
        {
            int SDate = Convert.ToInt32(StartDate.Replace("-", ""));
            int EDate = Convert.ToInt32(EndDate.Replace("-", ""));
            if (EDate < SDate)
            {
                this.lblMessage.Text = "Date From cannot after Date To ! ";
                this.lblMessage.Visible = true;
                return false;
            }
        }

        return true;
    }
}
