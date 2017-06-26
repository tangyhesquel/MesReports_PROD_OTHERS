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
using System.Text;

public partial class WIPReport : pPage
{
    protected string PageHeader;
    protected string gvHeaderStr;
    int order_qty = 0;
    int opening = 0;
    int in_qty = 0;
    int pullin_qty = 0;
    int out_qty = 0;
    int pullout_qty = 0;
    int dis_qty = 0;
    int wip_qty = 0;
    string Factory_cd;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
        {
            Factory_cd = Request.QueryString["site"].ToString();
        }
        else
        {
            Factory_cd = "GEG";
        }
        if (!IsPostBack)
        {
           
            ddlProdFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
            ddlProdFactory.DataTextField = "FACTORY_ID";
            ddlProdFactory.DataValueField = "FACTORY_ID";
            ddlProdFactory.DataBind();

            ddlProcessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
            ddlProcessType.DataTextField = "PROCESS_TYPE_ID";
            ddlProcessType.DataValueField = "PROCESS_TYPE_VALUE";
            ddlProcessType.DataBind();

            //MESComment.MesRpt.Ini_CT_Alert_DDL(ddlProcessCd, base.CurrentSite);
            MESComment.MesRpt.Ini_CT_Alert_DDL(ddlProcessCd,ddlGarmentType.SelectedItem.Value,Factory_cd);
            if (base.CurrentSite == "YMG" || base.CurrentSite == "GEG" || base.CurrentSite == "CEG" || base.CurrentSite == "CEK" || base.CurrentSite == "NBO")
            {
                PageHeader = "MES月度进出存报表";
            }
            else
            {
                PageHeader = "MES Monthly Stock Movement Report";
            }
            this.ExcTable.Visible = false;
            this.msgTable.Visible = false;
            if (base.CurrentSite.ToString().ToUpper().Equals("YMG"))
            {
                this.ddlGroupName.Enabled = true;
                MESComment.CommFunction.LoadGroupDropDownList(base.CurrentSite, this.ddlGroupName);
            }
            else
            {
                this.ddlGroupName.Enabled = false;
            }

        }
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
            this.msgTable.Visible = false;
            return false;
        }
        if (strBeginDate.Length == 0 && strEndDate.Length != 0 || strBeginDate.Length != 0 && strEndDate.Length == 0)
        {
            this.lblMessage.Text = "Please select accurate  Date!";
            this.lblMessage.Visible = true;
            this.gvBody.Visible = false;
            this.msgTable.Visible = false;
            return false;
        }
        if (strBeginDate.Substring(0, 4) != strEndDate.Substring(0, 4) || strBeginDate.Substring(5, 2) != strEndDate.Substring(5, 2))
        {
            this.lblMessage.Text = "Selected Date must be in the same month!";
            this.lblMessage.Visible = true;
            this.gvBody.Visible = false;
            this.msgTable.Visible = false;
            return false;
        }
        if (ddlProcessCd.SelectedItem == null)
        {
            this.lblMessage.Text = "Please select one item!";
            this.lblMessage.Visible = true;
            this.gvBody.Visible = false;
            this.msgTable.Visible = false;
            return false;
        }
        return true;
    }

    private void SetQuery()
    {
        this.lblMessage.Visible = false;
        this.msgTable.Visible = false;
        if (true == this.CheckQueryCondition())
        {
            this.ExcTable.Visible = true;
            if (base.CurrentSite == "YMG" || base.CurrentSite == "GEG" || base.CurrentSite == "CEG" || base.CurrentSite == "CEK" || base.CurrentSite == "NBO")
            {
                PageHeader = "MES月度进出存报表";
                if (!this.ddlGroupName.SelectedValue.Equals(""))
                {
                    gvHeaderStr = "制单,客户,款式,GO,SAM,洗水方式,SAH,交期," +
                          "PPCD,订单数,部门,Production Line,期初数,接收数,Pull In,发出数,Pull Out,下数,结存数";
                }
                else
                {
                    gvHeaderStr = "制单,客户,款式,GO,SAM,洗水方式,SAH,交期," +
                          "PPCD,订单数,部门,期初数,接收数,Pull In,发出数,Pull Out,下数,结存数";
                }
            }
            else
            {
                PageHeader = "MES Monthly Stock Movement Report";
                gvHeaderStr = "JOB_ORDER_NO,Buyer,StyleNO,SC_NO,SAM,Wash Type,SAH,BPD," +
                       "PPCD,Order Qty,PROCESS_CD,OPENING,IN_QTY,PullIn_QTY,OUT_QTY,PullOut_QTY,Discrepancy QTY,WIP QTY";
            }
            this.gvBody.Visible = true;
            DataTable dt = new DataTable();
            if (!this.ddlGroupName.SelectedValue.Equals(""))
            {
                //dt = MESComment.WIPReportSql.GetIE_WIP_ReportForGroupName(CbChoose.Checked, base.CurrentSite, this.txtBeginDate.Text, this.txtEndDate.Text, ddlProcessCd.SelectedItem.Value, this.ddlGroupName.SelectedValue);
                dt = MESComment.WIPReportSql.GetIE_WIP_ReportForGroupName(CbChoose.Checked, Factory_cd, this.txtBeginDate.Text, this.txtEndDate.Text,ddlGarmentType.SelectedItem.Value,ddlProcessCd.SelectedItem.Value,ddlProcessType.SelectedItem.Value,ddlProdFactory.SelectedItem.Value,this.ddlGroupName.SelectedValue);
            }
            else
            {
                if (!CbByDaily.Checked)
                {
                    //dt = MESComment.WIPReportSql.GetIE_WIP_Report(CbChoose.Checked, base.CurrentSite, this.txtBeginDate.Text, this.txtEndDate.Text, ddlProcessCd.SelectedItem.Value);
                    dt = MESComment.WIPReportSql.GetIE_WIP_Report(CbChoose.Checked, Factory_cd, this.txtBeginDate.Text, this.txtEndDate.Text,ddlGarmentType.SelectedItem.Value,ddlProcessType.SelectedItem.Value, ddlProcessCd.SelectedItem.Value,ddlProdFactory.SelectedItem.Value);

                }
                else
                {
                    //dt = MESComment.WIPReportSql.GetIE_WIP_ReportDaily(CbChoose.Checked, base.CurrentSite, this.txtBeginDate.Text, this.txtEndDate.Text, ddlProcessCd.SelectedItem.Value);
                    dt = MESComment.WIPReportSql.GetIE_WIP_ReportDaily(CbChoose.Checked, Factory_cd, this.txtBeginDate.Text, this.txtEndDate.Text,ddlGarmentType.SelectedItem.Value,ddlProcessType.SelectedItem.Value,ddlProcessCd.SelectedItem.Value,ddlProdFactory.SelectedItem.Value);
                }
            }
            //DataTable dt = MESComment.WIPReportSql.GetIE_WIP_Report(CbChoose.Checked, base.CurrentSite, this.txtBeginDate.Text, this.txtEndDate.Text, ddlProcessCd.SelectedItem.Value,this.ddlGroupName .SelectedValue);
            if (dt.Rows.Count > 1)
            {//GetIE_WIP_Report 中有合计行,固定最少有1行数据,故此要>1;
                int row = dt.Rows.Count - 1;
                order_qty = int.Parse(dt.Rows[row]["order_qty"].ToString());//9
                opening = int.Parse(dt.Rows[row]["opening"].ToString());//11
                in_qty = int.Parse(dt.Rows[row]["in_qty"].ToString());//12
                pullin_qty = int.Parse(dt.Rows[row]["pullin_qty"].ToString());//13
                out_qty = int.Parse(dt.Rows[row]["out_qty"].ToString());//14
                pullout_qty = int.Parse(dt.Rows[row]["pullout_qty"].ToString());//15
                dis_qty = int.Parse(dt.Rows[row]["dis_qty"].ToString());//16
                wip_qty = int.Parse(dt.Rows[row]["wip_qty"].ToString());//17
                dt.Rows[row].Delete();
                this.gvBody.DataSource = dt;
                this.gvBody.DataBind();
            }
            else
            {
                //show "no data";
                this.ExcTable.Visible = false;
                this.msgTable.Visible = true;
                return;
            }

        }
        else
        {
            this.ExcTable.Visible = false;
        }
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        //DataTable dailyExist = MESComment.WIPReportSql.GetIE_WIP_ReportDailyExist(base.CurrentSite, this.txtBeginDate.Text);
        DataTable dailyExist = MESComment.WIPReportSql.GetIE_WIP_ReportDailyExist(Factory_cd, this.txtBeginDate.Text);
        if (!CbByDaily.Checked)
        {
            SetQuery();
        }
        else
        {
            if (dailyExist.Rows.Count == 0)
            {
                Response.Write(" <script   language=javascript> alert( 'No Daily Data,Please select again!'); </script> ");
            }
            else
            {
                SetQuery();
            }
        }
       
    }
    protected void btnExcel_Click(object sender, EventArgs e)
    {//改用js的toExcel()方法 2012-02-24 by haiqiang
        //SetQuery();
        //MESComment.Excel.ToExcel(this.gvBody, "WIPReport" + DateTime.Now.ToString("yyyyMMdd") + ".xls");        
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
            case DataControlRowType.Footer:
                int J = 1;
                if (!this.ddlGroupName.SelectedValue.Equals(""))
                {
                    J = 0;
                }
                    e.Row.Cells[8].Text = "Total:";
                    e.Row.Cells[9].Text = order_qty.ToString();
                    e.Row.Cells[12 - J].Text = opening.ToString();
                    e.Row.Cells[13 - J].Text = in_qty.ToString();
                    e.Row.Cells[14 - J].Text = pullin_qty.ToString();
                    e.Row.Cells[15 - J].Text = out_qty.ToString();
                    e.Row.Cells[16 - J].Text = pullout_qty.ToString();
                    e.Row.Cells[17 - J].Text = dis_qty.ToString();
                    e.Row.Cells[18 - J].Text = wip_qty.ToString();
                

                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    //e.Row.Cells[i].Attributes.Add("bgcolor", "#efefe7");
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }
                break;

        }
    }
    protected void ddlGarmentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        MESComment.MesRpt.Ini_CT_Alert_DDL(ddlProcessCd, ddlGarmentType.SelectedItem.Value, Factory_cd);
    }
}
