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
    public string Factory_CD = "";
    public string Process_CD = "";
    public string Garment_Type = "";
    public string Wash_Type = "";
    public string Type = "Old";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                Factory_CD = this.Request.QueryString["site"].ToString().ToUpper();
            }
            else
            {
                Factory_CD = "GEG";
            }
        }
        if (this.Request.QueryString.Get("Factory_CD") != null)
        {
            Factory_CD = this.Request.QueryString.Get("Factory_CD").ToString();
            if (Factory_CD.Contains("YMG"))
            {
                Factory_CD = "YMG";
            }
        }
        if (this.Request.QueryString.Get("Type") != null)
        {
            this.Type = this.Request.QueryString.Get("Type").ToString();
        }

        if (Factory_CD.Equals("DEV"))
        {
            if (this.Request.QueryString.Get("factory_cd") != null)
            {
                Factory_CD = "GEG";
                
            }
            else
            {
                return;
            }
        }

        if (!this.IsPostBack)
        {
            MESComment.MesRpt.Ini_TypeKW_DDL(ddlType);
            this.ddlProcess_cd.DataSource = MESComment.MesRpt.GetProcessCode(Factory_CD, "");
            this.ddlProcess_cd.DataBind();

            
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

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.divBody.InnerHtml = "";
        this.gvBody.Visible = true;
        this.Process_CD = this.ddlProcess_cd.SelectedValue.ToString();
        this.Garment_Type = this.ddlType.SelectedValue.ToString();
        this.Wash_Type = this.ddlWashType.SelectedValue.ToString();
        DataTable dtData = new DataTable();
        if (this.Type.Equals("Old"))
        {
            dtData = MESComment.JODNotCloseSql.GetAllDataByProcess(Factory_CD, Process_CD, Garment_Type,ddlProcessType.SelectedItem.Value,ddlProdFactory.SelectedItem.Value, Wash_Type);
        }
        else
        {
            dtData = MESComment.JODNotCloseSql.GetAllDataByProcess_dev(Factory_CD, Process_CD, Garment_Type, ddlProcessType.SelectedItem.Value, ddlProdFactory.SelectedItem.Value, Wash_Type);
        }
        if (dtData.Rows.Count >= 1)
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
                e.Row.Cells[0].Attributes.Add("align", "center");
                e.Row.Cells[1].Attributes.Add("align", "left");
                e.Row.Cells[2].Attributes.Add("align", "center");
                for (int i = 3; i < e.Row.Cells.Count - 1; i++)
                {
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }
                e.Row.Cells[e.Row.Cells.Count - 1].Attributes.Add("align", "center");
                e.Row.Cells[e.Row.Cells.Count - 1].Style["font-weight"] = "bold";
                break;
            case DataControlRowType.Footer:
                e.Row.Attributes.Add("bgcolor", "#efefe7");
                e.Row.Attributes.Add("align", "right");              
                break;
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
    }

}
