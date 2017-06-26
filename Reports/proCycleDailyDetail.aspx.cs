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

public partial class Reports_proCycleDetail : pPage
{
    int totalWipQty = 0, totalOutQty = 0;
    string joNo, factoryCd, processCd, startDate, endDate;
    protected void Page_Load(object sender, EventArgs e)
    {
        joNo=Request.QueryString["joNo"];
        factoryCd=Request.QueryString["factoryCd"];
        processCd=Request.QueryString["processCd"];
        startDate=Request.QueryString["startDate"];
        endDate=Request.QueryString["endDate"];
        lblJobOrderNo.Text = joNo;
        lblFactoryCd.Text = factoryCd;
        lblProcessCd.Text = processCd;
        lblStartDate.Text = startDate;
        lblEndDate.Text = endDate;
        SetQuery();
    }

    public override void VerifyRenderingInServerForm(Control control)
    {

    }

    private void SetQuery()
    {
        gvDetail.DataSource = MESComment.ProCycleDailyDetail.Get_proCycleDetail(factoryCd, processCd, startDate, endDate, joNo);
        gvDetail.DataBind();
    }
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        
        if (e.Row.RowIndex >= 0)
        {
            totalOutQty += Convert.ToInt32(e.Row.Cells[10].Text);
            totalWipQty += Convert.ToInt32(e.Row.Cells[13].Text);
        }
        lblTotalWipQty.Text = totalWipQty.ToString();
        lblTotalOutQty.Text = totalOutQty.ToString() ;
        lblCT.Text = (double.Parse(totalWipQty.ToString()) / double.Parse(totalOutQty.ToString())).ToString();

    }
    protected void btnExcel_Click(object sender, EventArgs e)
    {
        SetQuery();
        MESComment.Excel.ToExcel(this.gvDetail, "proCycleDetail.aspx" + DateTime.Today.ToShortDateString() + ".xls");
    }
}
