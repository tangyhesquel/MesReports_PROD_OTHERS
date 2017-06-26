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
using System.Data.Common;

public partial class WorkListReport : pPage
{
    protected string PageHeader;
    protected string gvHeaderStr;
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
            ddlFtyCd.SelectedValue = Request.QueryString["site"].ToString();
        }
        PageHeader = "MES Work List Report";


    }
    private bool CheckQueryCondition()
    {
        string strBeginDate = this.txtBeginDate.Text.Trim();
        string strEndDate = this.txtEndDate.Text.Trim();
        if (strBeginDate.Length == 0 && strEndDate.Length == 0)
        {
            this.lblMessage.Text = "Please choose condition to query!";
            this.lblMessage.Visible = true;
            this.gvBody.Visible = false;
            return false;
        }
        if (strBeginDate.Length == 0 && strEndDate.Length != 0 || strBeginDate.Length != 0 && strEndDate.Length == 0)
        {
            this.lblMessage.Text = "Please select accurate  Date!";
            this.lblMessage.Visible = true;
            this.gvBody.Visible = false;
            return false;
        }
        //if (strBeginDate.Substring(0, 4) != strEndDate.Substring(0, 4) || strBeginDate.Substring(5, 2) != strEndDate.Substring(5, 2))
        //{
        //    this.lblMessage.Text = "Selected Date must be in the same month!";
        //    this.lblMessage.Visible = true;
        //    this.gvBody.Visible = false;
        //    return false;
        //}
        if (ddlFtyCd.SelectedItem == null)
        {
            this.lblMessage.Text = "Please select one item!";
            this.lblMessage.Visible = true;
            this.gvBody.Visible = false;
            return false;
        }
        return true;
    }
    private void SetQuery()
    {
        this.lblMessage.Visible = false;
        if (true == this.CheckQueryCondition())
        {


            DataTable WorklistdataR = MESComment.MesRpt.GetWorkListdata(ddlFtyCd.SelectedItem.Value, this.txtBeginDate.Text, this.txtEndDate.Text);

            for (int i = 0; i < WorklistdataR.Columns.Count; i++)
            {
                if (WorklistdataR.Columns[i].ColumnName != "")
                {
                    if (i == 0)
                    {
                        gvHeaderStr = WorklistdataR.Columns[i].ColumnName;
                    }
                    else
                    {
                        gvHeaderStr = gvHeaderStr + "," + WorklistdataR.Columns[i].ColumnName;
                    }
                }
            }


            this.gvBody.Visible = true;
            this.gvBody.DataSource = WorklistdataR;
            this.gvBody.DataBind();

        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        SetQuery();
    }
    protected void btnExcel_Click(object sender, EventArgs e)
    {
        SetQuery();
        MESComment.Excel.ToExcel(this.gvBody, "WIPReport" + DateTime.Now.ToString("MM/DD/YYYY") + ".xls");
    }
    public override void VerifyRenderingInServerForm(Control control)
    {

    }

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

                break;
        }

    }
}
