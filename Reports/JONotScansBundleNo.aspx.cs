using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using MESComment;

public partial class Reports_mesMarkerCSBreakdown : pPage
{
    public string Factory_CD = "", Process_CD = "", Garment_Type = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Request.QueryString["site"] != null)
        {
            
            this.Factory_CD = this.Request.QueryString["site"].ToString().ToUpper();
                
            if (Factory_CD.Equals("DEV"))
            {
                this.Factory_CD = "GEG";
                //this.Factory_CD = this.Request.QueryString["factory"].ToString().ToUpper();
            }
        }

        if (this.Request.QueryString["process"] != null)
        {
            this.Process_CD = this.Request.QueryString["process"].ToString().ToUpper();
        }
        if (!this.IsPostBack)
        {
            MESComment.MesRpt.Ini_TypeKW_DDL(ddlType);
            this.ddlProcess_cd.DataSource = MESComment.MesRpt.GetProcessCode(Factory_CD, "");
            this.ddlProcess_cd.DataBind();

            ddlProcessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
            ddlProcessType.DataTextField = "PROCESS_TYPE_ID";
            ddlProcessType.DataValueField = "PROCESS_TYPE_VALUE";
            ddlProcessType.DataBind();

            
            ddlProdFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
            ddlProdFactory.DataTextField = "FACTORY_ID";
            ddlProdFactory.DataValueField = "FACTORY_ID";
            ddlProdFactory.DataBind();
            

            if (!this.Process_CD.Trim().Equals(""))
            {
                this.ddlProcess_cd.SelectedValue = this.Process_CD;
            }
        }
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.divBody.InnerHtml = "";
        this.divMsg.InnerHtml = "";
        this.gvBody.Visible = true;
        if (CHECK())
        {
            this.Process_CD = this.ddlProcess_cd.SelectedValue.ToString();
            this.Garment_Type = this.ddlType.SelectedValue.ToString();
            string JONO = this.txtJONO.Text.ToString();
            DataTable dtData = MESComment.JONotScansBundleNoSql.GetNoScansBundleNo(Factory_CD, Garment_Type, Process_CD,ddlProcessType.SelectedItem.Value,ddlProdFactory.SelectedItem.Value,JONO);
            if (dtData.Rows.Count >= 2)
            {
                this.gvBody.DataSource = dtData;
                this.gvBody.DataBind();
            }
            else
            {
                this.gvBody.Visible = false;
                this.divBody.InnerHtml += "<table width='100%'  style='color:Red; font-size:large'><tr><td align='center'><b>No Data</b></td></tr></table>";
            }
        }
        else
        {
            this.divMsg.InnerHtml += "<table width='100%'  style='color:Red; font-size:12px'><tr><td align='left'>PLS input JO NO!</td></tr></table>";
        }
    }

    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Attributes.Add("bgcolor", "#efefe7");
                }
                break;
            case DataControlRowType.DataRow:
                if (e.Row.Cells[0].Text.Contains("Total"))
                {
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        e.Row.Cells[i].Attributes.Add("bgcolor", "#efefe7");
                        e.Row.Cells[i].Attributes.Add("align", "right");
                    }
                }
                else
                {
                    e.Row.Cells[0].Attributes.Add("align", "left");
                }
                e.Row.Cells[1].Attributes.Add("align", "center");
                for (int i = 2; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }
                //e.Row.Cells[e.Row.Cells.Count-1].Attributes.Add("align", "center");
                //e.Row.Cells[e.Row.Cells.Count - 1].Style["font-weight"] = "bold";

                break;
            case DataControlRowType.Footer:
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Attributes.Add("bgcolor", "#efefe7");
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }
                break;
        }
    }

    protected Boolean CHECK()
    {
        if (this.txtJONO.Text == null)
        {
            return false;
        }
        else if (this.txtJONO.Text.Trim().Equals(""))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
    {
        string GarmentType = "";
        if (!this.ddlType.SelectedValue.ToString().Equals("ALL"))
        {
            GarmentType = this.ddlType.SelectedValue.ToString();
        }
        this.ddlProcess_cd.DataSource = MESComment.MesRpt.GetProcessCode(Factory_CD, GarmentType);
        this.ddlProcess_cd.DataBind();
        this.gvBody.Visible = false;
    }

}
