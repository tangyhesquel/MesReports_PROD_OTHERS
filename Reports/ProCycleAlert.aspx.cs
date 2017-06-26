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

public partial class Reports_proCycleAlert : pPage
{
    bool IsHyperlink = true;
    string Factory_CD = "";
    string Type = "Old";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Request.QueryString.Get("site") != null)
        {
            if (this.Request.QueryString.Get("site").ToString().ToUpper() != "DEV")
            {
                Factory_CD = this.Request.QueryString.Get("site").ToString();
            }
            else
            {
                Factory_CD = "GEG";
            }
            if (this.Request.QueryString.Get("factory_cd") != null)
            {
                this.Factory_CD = this.Request.QueryString.Get("factory_cd").ToString();
                if (Factory_CD.Contains("YMG"))
                {
                    this.Factory_CD = "YMG";
                }
            }
            if (this.Request.QueryString.Get("Type") != null)
            {
                this.Type = this.Request.QueryString.Get("Type").ToString();
            }

        }
        if (!IsPostBack)
        {
            MESComment.ProCycleAlertSql.Ini_CT_Alert_DDL(ddlFactory_cd);
            ddlFactory_cd.SelectedValue = Factory_CD;
            MESComment.ProCycleAlertSql.Ini_CT_Alert_DDL(ddlGarment_type.SelectedItem.Value, ddlProcess_cd, ddlFactory_cd.SelectedItem.Value);
            divBody.Visible = false;
            txtBeginDate.Text = DateTime.Now.AddMonths(-2).ToString("yyyy-MM-dd");

            
            ddlProdFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
            ddlProdFactory.DataTextField = "FACTORY_ID";
            ddlProdFactory.DataValueField = "FACTORY_ID";
            ddlProdFactory.DataBind();

            ddlProcessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
            ddlProcessType.DataTextField = "PROCESS_TYPE_ID";
            ddlProcessType.DataValueField = "PROCESS_TYPE_VALUE";
            ddlProcessType.DataBind();

        }


    }

    private bool CheckQueryCondition()
    {
        if (txtBeginDate.Text.Length == 0)
        {
            txtBeginDate.Text = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd");
        }
        return true;
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.lblMessage.Visible = false;
        IsHyperlink = true;
        SetQuery();
    }

    private void SetQuery()
    {
        if (true == this.CheckQueryCondition())
        {
            this.divBody.Visible = true;
            lblFactoryID.Text = ddlFactory_cd.SelectedItem.Value;
            lblProcessCD.Text = ddlProcess_cd.SelectedItem.Value;
            lblBeginDate.Text = DateTime.Parse(txtBeginDate.Text).ToString("yyyy-MM-dd");
            lblEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            if (this.Type.Equals("Old"))
            {
                this.gvBody.DataSource = MESComment.ProCycleAlertSql.Get_CT_Alert(ddlFactory_cd.SelectedItem.Value,ddlGarment_type.SelectedItem.Value,ddlProcess_cd.SelectedItem.Value,ddlProcessType.SelectedItem.Value,ddlProdFactory.SelectedItem.Value,txtBeginDate.Text);                
            }
            else
            {
                this.gvBody.DataSource = MESComment.ProCycleAlertSql.Get_CT_Alert_DEV(ddlFactory_cd.SelectedItem.Value, ddlGarment_type.SelectedItem.Value, ddlProcess_cd.SelectedItem.Value, ddlProcessType.SelectedItem.Value, ddlProdFactory.SelectedItem.Value,txtBeginDate.Text);
            }
            this.gvBody.DataBind();
        }
    }

    public override void VerifyRenderingInServerForm(Control control)
    {

    }

    //Modified by ZK Batch 4 - Add Stock Days 16-Jan-14
    string gvHeaderStr = "JOB_ORDER_NO,Factory_CD,Process_CD,Garment Type,Process type,Production factory,Buyer Name,Wash Type,Outsourced Type,IN_QTY,OUT_QTY,Pullout QTY,Discrepancy QTY," +
                          "WIP_QTY,DATE,Stagnation Days,Stock Days";
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                string[] hStr = gvHeaderStr.Split(new char[] { ',' });
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Text = hStr[i].ToString();
                }
                break;
            case DataControlRowType.DataRow:
                if (IsHyperlink)
                {
                    string startDate = txtBeginDate.Text;
                    string endDate = DateTime.Today.ToShortDateString();
                    e.Row.Cells[0].Text = string.Format("<a href='#' style=\"font-size:8pt\" onclick='window.open (\"proCycleDailyDetail.aspx?joNo={0}&factoryCd={1}&site={1}&processCd={2}&startDate={3}&endDate={4}\", \"Cycle\", \"height=600, width=1100, toolbar=no, menubar=no, scrollbars=yes, resizable=no, location=no, status=no\")'>{0}</a>", e.Row.Cells[0].Text, e.Row.Cells[1].Text, e.Row.Cells[2].Text, startDate, endDate);
                }
                break;
        }

    }
    protected void btnExcel_Click(object sender, EventArgs e)
    {
        IsHyperlink = false;
        SetQuery();
        MESComment.Excel.ToExcel(this.divBody, "ProCycleAlert" + DateTime.Now.ToString("yyyyMMdd") + ".xls");

    }
    protected void ddlFactory_cd_SelectedIndexChanged(object sender, EventArgs e)
    {
        MESComment.ProCycleAlertSql.Ini_CT_Alert_DDL(ddlGarment_type.SelectedItem.Value,ddlProcess_cd, ddlFactory_cd.SelectedItem.Value);
    }
    protected void ddlGarment_type_SelectedIndexChanged(object sender, EventArgs e)
    {
        string GarmentType = "";
        if (!this.ddlGarment_type.SelectedValue.ToString().Equals("ALL"))
        {
            GarmentType = this.ddlGarment_type.SelectedValue.ToString();
        }
        MESComment.ProCycleAlertSql.Ini_CT_Alert_DDL(ddlGarment_type.SelectedItem.Value, ddlProcess_cd, ddlFactory_cd.SelectedItem.Value);
        divBody.Visible = false;
        txtBeginDate.Text = DateTime.Now.AddMonths(-2).ToString("yyyy-MM-dd");
    }
}
