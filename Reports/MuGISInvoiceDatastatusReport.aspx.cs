using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;
using System.IO;

public partial class Reports_GEGMUReport : pPage
{
    string[] strHeader = { 
                             "Start Cutting_Date","Job_Order_No", "YPD Job No", "Ship_Date", "Customer", "GarmentOrder", "PPO Number", "Fabric Width", "Long/short_sleeve", "Wash type", "Fabric Pattern", "Marker Utilization (%)","Fabric (YDS)","Garment (DZ)","YPD","RATIO","Buyer's max over shipment allowance","Buyer's max over cut allowance</th></tr><tr>",
                             "PPO order yds","Allocated","Issued","Marker Audited","Ship Yardage","Cutting Wastage","Leftover fabric","LeftOver Fab Reason Code","LeftOver Fab Reason","Remnant","SRN","RTW","Order(DZ)","Cut","Shipped","Sample","Pull-out","Leftover Garment","Sewing Wastage","Washing Wastage","Unacc Gmt","PPO MKR YPD","BULK NET YPD","GIVEN CUT YPD","BULK MKR YPD","YPD Var","CUT YPD","SHIP YPD","Ship-to-Cut","Ship-to-Receipt","Ship-to-Order","Cut-to-Receipt","Cut-to-Order</th></tr><tr>",
                             "Yds","%","Defect Fab","Defect Panels","Odd Loss","Splice","Cutting Rej Panels","Match Lost","EndLost","Short Fab","Sewing match loss","Grade A","Grade B","Grade C","Total","DZ","%","DZ","%"
                         };
    string userid = "";

    // public int ship_order_count_95, ship_order_count_100, YPD_Var_big, YPD_Var_small;
    // public double ship_order_sum;
    public string strRunNO;
    // public double Total_Allocated = 0;//开放数据
    //  public double Total_Ship_Yardage = 0;//开放数据
    // public double Total_Leftover = 0;//开放数据
    public string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["userid"] != null)
        {
            userid = Request.QueryString["userid"].ToString();
        }
        if (!IsPostBack)
        {
            ddlFtyCd.DataSource = MESComment.MesRpt.GetFactoryList(userid);
            ddlFtyCd.DataTextField = "DEPARTMENT_ID";
            ddlFtyCd.DataValueField = "DEPARTMENT_ID";
            ddlFtyCd.DataBind();
            divMsg.Visible = false;
            div_all.Visible = false;
        }
        if (Request.QueryString["site"] != null)
        {
            ddlFtyCd.SelectedValue = Request.QueryString["site"].ToString();
        }

    }

    private bool CheckQueryCondition()
    {
        string Strjo = this.txtJoNo.Text;
        string strBeginDate = this.txtFromDate.Text;
        string strEndDate = this.txtToDate.Text;

        if (Strjo.Length == 0 &&
            strBeginDate.Trim().Length == 0 && strEndDate.Trim().Length == 0)
        {
            this.lblMessage.Text = "Please choose condition to query!";
            this.lblMessage.Visible = true;
            this.gvDetail.Visible = false;
            return false;
        }

        if (Strjo.Length == 0)
        {
            if (strBeginDate.Length == 0 && strEndDate.Length != 0 || strBeginDate.Length != 0 && strEndDate.Length == 0)
            {
                this.lblMessage.Text = "Please select accurate  Date!";
                this.lblMessage.Visible = true;
                this.gvDetail.Visible = false;
                return false;
            }
        }
        if (strBeginDate.Length != 0 && strEndDate.Length != 0 && Convert.ToDateTime(strBeginDate) > Convert.ToDateTime(strEndDate))
        {
            this.lblMessage.Text = "Selected To Date must after From Date!";
            this.lblMessage.Visible = true;
            this.gvDetail.Visible = false;
            return false;
        }

        return true;
    }


    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.lblMessage.Visible = false;

        if (true == this.CheckQueryCondition())
        {
            Random ro = new Random(1000);
            strRunNO = Convert.ToString(string.Format("{0:yy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(10).ToString();
            string ToDate = "";
            if (!txtToDate.Text.Equals(""))
            {
                ToDate = DateTime.Parse(txtToDate.Text).AddDays(1).ToShortDateString();
            }
            gvDetail.Visible = false;
            gvDetail.DataSource = null;
            gvDetail.DataBind();

            //DataTable ShipDataDT = MESComment.MesRpt.GetGISInvoiceDataStatus(ddlFtyCd.SelectedValue, txtFromDate.Text, ToDate, strRunNO, txtJoNo.Text);
            DataTable ShipDataDT = MESComment.MUReportSql.GetGISInvoiceDataStatus(ddlFtyCd.SelectedValue, txtFromDate.Text, ToDate, strRunNO, txtJoNo.Text);
            if (ShipDataDT.Rows.Count > 0)
            {
                gvDetail.DataSource = ShipDataDT;
                gvDetail.DataBind();
                gvDetail.Visible = true;
                div_all.Visible = true;
                divMsg.Visible = false;
                DateTime DTime = DateTime.Now;
                txtPrintDate.Text = DTime.ToString();
            }
            else
            {
                div_all.Visible = false;
                divMsg.Visible = true;
            }

        }
    }


    protected void gvDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                break;
            case DataControlRowType.DataRow:
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
                e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Center;
                e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Center;
                e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Center;
                e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Center;
                e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[8].HorizontalAlign = HorizontalAlign.Right;
                break;
            case DataControlRowType.Footer:
                break;
        }
    }
}
