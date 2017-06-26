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
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                this.Factory_CD = this.Request.QueryString["site"].ToString().ToUpper();
            }
            else
            {
                Factory_CD = "GEG";
            }

        }
        if (this.Request.QueryString["process"] != null)
        {
            this.Process_CD = this.Request.QueryString["process"].ToString().ToUpper();
        }
        if (!this.IsPostBack)
        {
            MESComment.MesRpt.Ini_TypeKW_DDL(ddlType);

           
            ddlProdFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
            ddlProdFactory.DataTextField = "FACTORY_ID";
            ddlProdFactory.DataValueField = "FACTORY_ID";
            ddlProdFactory.DataBind();
           

            if (Factory_CD.Equals("DEV"))
            {
                this.Factory_CD = "GEG";
                //this.Factory_CD = this.Request.QueryString["factory"].ToString().ToUpper();
            }
            this.ddlProcess_cd.DataSource = MESComment.MesRpt.GetProcessCode(Factory_CD, "");

            this.ddlProcess_cd.DataBind();
            
            ddlProcessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
            ddlProcessType.DataTextField = "PROCESS_TYPE_ID";
            ddlProcessType.DataValueField = "PROCESS_TYPE_VALUE";
            ddlProcessType.DataBind();

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
            string Date = this.txtSubmitDate.Text;
            DataTable dtData = MESComment.JOHaveScannedSql.GetHaveScannedJo(Factory_CD, Garment_Type, Process_CD,ddlProcessType.SelectedItem.Value,ddlProdFactory.SelectedItem.Value,Date);
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
        else
        {
            this.divMsg.InnerHtml += "<table width='100%'  style='color:Red; font-size:12px'><tr><td align='left'>PLS Choose the correct date!</td></tr></table>";
        }
    }

    string ColumnName = "制单号,制单数,裁数,扫描OUT数,剩余WIP数</TR><TR>,当天,累计";

    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                string[] Name = ColumnName.Split(',');
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();
                for (int j = 0; j <= 2; j++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[j].Attributes.Add("rowspan", "2");
                    tcHeader[j].Text = Name[j].ToString();
                    tcHeader[j].Attributes.Add("bgcolor", "#F7F6F3");
                }
                tcHeader.Add(new TableHeaderCell());
                tcHeader[3].Attributes.Add("colspan", "2");
                tcHeader[3].Text = Name[3].ToString();
                tcHeader[3].Attributes.Add("bgcolor", "#F7F6F3");

                tcHeader.Add(new TableHeaderCell());
                tcHeader[4].Attributes.Add("rowspan", "2");
                tcHeader[4].Text = Name[4].ToString();
                tcHeader[4].Attributes.Add("bgcolor", "#F7F6F3");

                for (int j = 5; j <= 6; j++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[j].Text = Name[j].ToString();
                    tcHeader[j].Attributes.Add("bgcolor", "#F7F6F3");
                }

                break;
            case DataControlRowType.DataRow:
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (i == 0)
                    {
                        e.Row.Cells[0].Attributes.Add("align", "left");
                    }
                    else
                    {
                        e.Row.Cells[i].Attributes.Add("align", "right");
                    }
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
        if (this.txtSubmitDate.Text == null)
        {
            return false;
        }
        else
        {
            if (this.txtSubmitDate.Text.Length < 10)
            {
                return false;
            }
            else
            {
                DateTime dt1 = Convert.ToDateTime(this.txtSubmitDate.Text);
                DateTime dt2 = DateTime.Now.Date;
                if (dt1 > dt2)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
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
