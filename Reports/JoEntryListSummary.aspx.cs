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

public partial class JoEntryListSummary : pPage
{
    int Count = 0, Qty = 0;
    string JO = string.Empty;
    string Factory_CD = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        ddlFactory_cd.DataSource = MESComment.MesRpt.GetFactoryList("");
        ddlFactory_cd.DataBind();
        if (this.Request.QueryString["site"] != null)
        {
            if (this.Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                Factory_CD = this.Request.QueryString["site"].ToString().ToUpper();
                this.ddlFactory_cd.SelectedValue = Factory_CD;
            }
            else
            {
                Factory_CD = "GEG";
                this.ddlFactory_cd.SelectedValue = Factory_CD;
            }
        }
        if (this.Factory_CD.ToString().ToUpper().Equals("YMG"))
        {
            this.ddlGroupName.Enabled = true;
            if (!IsPostBack) 
                    MESComment.CommFunction.LoadGroupDropDownList(this.Factory_CD, this.ddlGroupName);
        }
        else
        {
            this.ddlGroupName.Enabled = false;
        }
        if (!IsPostBack)
        {
            
            ddlProdFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
            ddlProdFactory.DataTextField = "FACTORY_ID";
            ddlProdFactory.DataValueField = "FACTORY_ID";
            ddlProdFactory.DataBind();
            

            MESComment.MesRpt.Ini_TypeKW_DDL(ddlType);
            this.ddlProcess_cd.DataSource = MESComment.MesRpt.GetProcessCode(Factory_CD, "");
            this.ddlProcess_cd.DataBind();

            ddlProcessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
            ddlProcessType.DataTextField = "PROCESS_TYPE_ID";
            ddlProcessType.DataValueField = "PROCESS_TYPE_VALUE";
            ddlProcessType.DataBind();

        }
    }

    private bool CheckQueryCondition()
    {
        if (txtBeginDate.Text.Length == 0 && txtEndDate.Text.Length == 0)
        {
            this.lblMessage.Text = "Please choose condition to query!";
            this.lblMessage.Visible = true;
            this.gvBody.Visible = false;
            return false;
        }
        if (txtBeginDate.Text.Length == 0 && txtEndDate.Text.Length != 0 || txtBeginDate.Text.Length != 0 && txtEndDate.Text.Length == 0)
        {
            this.lblMessage.Text = "Please select accurate  Date!";
            this.lblMessage.Visible = true;
            this.gvBody.Visible = false;
            return false;
        }
        if (txtBeginDate.Text.Substring(0, 4) != txtEndDate.Text.Substring(0, 4) || txtBeginDate.Text.Substring(5, 2) != txtEndDate.Text.Substring(5, 2))
        {
            this.lblMessage.Text = "Selected Date must be in the same month!";
            this.lblMessage.Visible = true;
            this.gvBody.Visible = false;
            return false;
        }
        return true;
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.lblMessage.Visible = false;
        SetQuery();
        if (gvBody.Rows.Count > 0)
        {
            lblFactory.Text = "部门：" + ddlProcess_cd.SelectedItem.Value;
        }
    }

    private void SetQuery()
    {
        this.divMsg.InnerHtml = "";
        if (this.CheckQueryCondition())
        {
            this.gvBody.Visible = true;
            DataTable dt = MESComment.JoEntryListSummarySql.Get_JO_Entry_List_Summary(ddlFactory_cd.SelectedItem.Value, ddlProcess_cd.SelectedItem.Value,ddlProcessType.SelectedItem.Value,ddlProdFactory.SelectedItem.Value,txtBeginDate.Text, txtEndDate.Text, ddlType.SelectedItem.Value, DropDownListStatus.SelectedValue, this.ddlGroupName.SelectedValue);
            if (dt.Rows.Count >= 1)
            {
                this.gvBody.DataSource = dt;
                this.gvBody.DataBind();
            }
            else
            {
                this.gvBody.Visible = false;
                this.divMsg.InnerHtml += "<table width='100%'  style='color:Red; font-size:large'><tr><td align='center'><b>No Data</b></td></tr></table>";
            }
        }
    }

    public override void VerifyRenderingInServerForm(Control control)
    {

    }

    //string gvHeaderStr = "Process,Date,JO,Customer,Production Line,Next_Production_line,Wash Type,Color,Size,Transaction Qty," +
    //                      "Previous Process,Create Date,Operator,Confirmed By,Confirmed Date,Confirmed,First Transfer,Pre-Process Closed";

    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                //已经直接在SQL里面重命名和排序;
                //string[] hStr = gvHeaderStr.Split(new char[] { ',' });
                //for (int i = 0; i < e.Row.Cells.Count; i++)
                //{
                //    e.Row.Cells[i].Text = hStr[i].ToString();
                //}
                break;
            case DataControlRowType.DataRow:
                //Transaction Qty需要Total;
                Count = Count + int.Parse(e.Row.Cells[11].Text.Replace("&nbsp;", "0"));//8 Change 10
                if (e.Row.Cells[3].Text != JO)
                {
                    Qty = Qty + 1;
                }
                JO = e.Row.Cells[3].Text;
                break;
            case DataControlRowType.Footer:
                //注意修改[N];
                e.Row.Cells[0].Text = "Total Count";
                e.Row.Cells[6].Text = Qty.ToString();
                e.Row.Cells[10].Text = "Total Qty";
                e.Row.Cells[11].Text = Count.ToString();
                break;
        }

    }
    protected void btnExcel_Click(object sender, EventArgs e)
    {
        SetQuery();
        MESComment.Excel.ToExcel(this.gvBody, "JoEntryListSummary" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
    }
    protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
    {        
        string GarmentType = this.ddlType.SelectedValue.ToString();
        this.ddlProcess_cd.DataSource = MESComment.MesRpt.GetProcessCode(Factory_CD, GarmentType);
        this.ddlProcess_cd.DataBind();
    }
}
