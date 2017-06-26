using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_proMajorDailyOut : pPage
{
    string userid = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        userid = Request.QueryString["userid"];
        if (!IsPostBack)
        {
            ddlFactoryCd.DataSource = MESComment.MesRpt.GetFactoryList(userid);
            ddlFactoryCd.DataBind();
        }
        if (Request.QueryString["site"] != "")
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                ddlFactoryCd.SelectedValue = Request.QueryString["site"].ToString();
            }
            else
            {
                ddlFactoryCd.SelectedValue = "GEG";
            }
            if (Request.QueryString["site"].ToString().ToUpper().Equals("TIL"))
            {
                this.SplitSEW.Checked = true;
            }
        }
        lblNoData.Visible = false;
    }
    

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        //Added By ZouShiChang ON 2013.11.22 MES024
        //DataTable dtData = MESComment.proMajorDailyOutSql.GetProMajorProcessQtyList(this.ddlFactoryCd.SelectedValue, this.txtJoNo.Text, this.SplitSEW.Checked);
        DataTable dtData = MESComment.proMajorDailyOutSql.GetProMajorProcessQtyListNew(this.ddlFactoryCd.SelectedValue, this.txtJoNo.Text, this.SplitSEW.Checked);
        if (dtData.Rows.Count > 0)
        {
            dtData.Columns.Remove("SEQ");
            this.divBody.InnerHtml = "";
            this.gvData.Visible = true;
            if (dtData.Columns.Count >= 2)
            {
                this.gvData.DataSource = dtData;
                this.DataBind();
            }
            else
            {
                this.divBody.InnerHtml += "<table width='100%'  style='color:Red; font-size:large'><tr><td align='center'><b>No Data</b></td></tr></table>";
                this.gvData.Visible = false;
            }
        }
        else
        {
            lblNoData.Visible = true;
        }
    }

    protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //Added By ZouShiChang ON 2013.11.22 MES024
        /* 
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Attributes.Add("bgcolor", "#F7F6F3");
                    e.Row.Cells[i].Attributes.Add("align", "center");
                }
                break;
            case DataControlRowType.DataRow:
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (i >= 1)
                    {
                        e.Row.Cells[i].Attributes.Add("align", "right");
                        //if (e.Row.Cells[i].Text.Contains("&nbsp;"))
                        //    e.Row.Cells[i].Text = "0";
                    }
                    else
                    {
                        e.Row.Cells[i].Attributes.Add("align", "center");
                    }
                    if (e.Row.Cells[0].Text.Contains("Total") || e.Row.Cells[0].Text.Contains("WIP"))
                    {
                        e.Row.Cells[i].Attributes.Add("bgcolor", "#F7F6F3");
                    }
                    
                }
                break;
            case DataControlRowType.Footer:
                break;
        }
         */
    }
}
