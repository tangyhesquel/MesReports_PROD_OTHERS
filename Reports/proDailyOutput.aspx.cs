using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_proDailyOutput : pPage
{
    public int allCol, allpage, cpage, pages;
    string userid = "";
    string strContent = "";
    string strTitle = "";
    System.Data.Common.DbConnection MESConn;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["userid"] != null)
        {
            userid = Request.QueryString["userid"];
        }
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                ddlfactoryCd.SelectedValue = Request.QueryString["site"].ToString();
            }
            else
            {
                ddlfactoryCd.SelectedValue = "GEG";
            }
        }
        if (!IsPostBack)
        {
            ddlfactoryCd.DataSource = MESComment.MesRpt.GetFactoryList(userid);
            ddlfactoryCd.DataBind();           
            ddlprocessCd.DataSource = MESComment.proDailyOutputSql.GetProcessCode(ddlfactoryCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value);
            ddlprocessCd.DataBind();

            
            ddlProdFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
            ddlProdFactory.DataTextField = "FACTORY_ID";
            ddlProdFactory.DataValueField = "FACTORY_ID";
            ddlProdFactory.DataBind();

            ddlProcessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
            ddlProcessType.DataTextField = "PROCESS_TYPE_ID";
            ddlProcessType.DataValueField = "PROCESS_TYPE_VALUE";
            ddlProcessType.DataBind();


            //Added by MunFoong on 2014.07.24, MES-139
            if (ddlfactoryCd.SelectedValue == "PTX") //if (Request.QueryString["site"].ToString() == "DEV") 
            {
                this.ddGroupName.Enabled = true;
                this.ddGroupName.DataSource = MESComment.wipDailySql.GetGroupName(ddlfactoryCd.SelectedValue);
                //this.ddGroupName.DataSource = MESComment.wipDailySql.GetGroupName("GEG");
                this.ddGroupName.DataTextField = "SYSTEM_KEY";
                this.ddGroupName.DataValueField = "SYSTEM_KEY";
                ddGroupName.DataBind();
            }
            else 
            {
                this.ddGroupName.Enabled = false;
            }
            //End of added by MunFoong on 2014.07.24, MES-139
        }
        
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        
        Random ro = new Random(1000);
        strContent = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
        strTitle = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
        gvBody.Visible = false;
        MESComment.MesRpt.SP_Pro_DropTmpTable(strContent, strTitle);
        MESConn = MESComment.DBUtility.GetConnection("MES_UPDATE");
        DataTable dt = MESComment.proDailyOutputSql.GetResultList(ddGroupName.SelectedItem.Value, ddlfactoryCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value, ddlWashType.SelectedItem.Value, txtDate.Text, ddlprocessCd.SelectedItem.Value, ddlProcessType.SelectedItem.Value, ddlProdFactory.SelectedItem.Value, strContent, strTitle, MESConn);
        if (dt.Rows.Count > 0)
        {
            gvBody.Visible = true;
            gvBody.Columns.Clear();            
            DataTable Column = MESComment.proDailyOutputSql.GetcontentIndexList(ddlfactoryCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value, ddlWashType.SelectedItem.Value, txtDate.Text, ddlprocessCd.SelectedItem.Value, strContent, strTitle, MESConn);
            for (int i = 1; i < 8; i++)
            {
                BoundField col = new BoundField();
                col.DataField = dt.Columns[i].ColumnName;
                col.Visible = true;
                gvBody.Columns.Add(col);
            }
                for (int i = 0; i < Column.Rows.Count; i++)
                {
                    BoundField col = new BoundField();
                    col.DataField = Column.Rows[i]["RFNames4"].ToString();
                    col.Visible = true;
                    gvBody.Columns.Add(col);
                }
            gvBody.DataSource = dt;
            gvBody.DataBind();
        }
    }
    string strHeader = "C/TNO,Buyer,StyleNO,Garment type,Wash Type,GO No.,SAH";
    object[] array = null;
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {        
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();
                //first row
                for (int i = 0; i < 7; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("rowspan", "3");
                    tcHeader[i].Attributes.Add("bgcolor", "#efefe7");
                    tcHeader[i].Text = strHeader.Split(',')[i];
                }                
                DataTable title1 = MESComment.proDailyOutputSql.Getlevel1TitleList(ddlfactoryCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value, ddlWashType.SelectedItem.Value, txtDate.Text, ddlprocessCd.SelectedItem.Value, strContent, strTitle, MESConn);
                for (int i = 0; i < title1.Rows.Count; i++)
                {
                    if (i != title1.Rows.Count - 1)
                    {
                        tcHeader.Add(new TableHeaderCell());
                        tcHeader[i + 7].Attributes.Add("colspan", "" + title1.Rows[i]["N_NO"].ToString() + "");
                        tcHeader[i + 7].Attributes.Add("bgcolor", "#efefe7");
                        tcHeader[i + 7].Text = title1.Rows[i]["RFNames1"].ToString();
                    }
                    else
                    {
                        tcHeader.Add(new TableHeaderCell());
                        tcHeader[i + 7].Attributes.Add("colspan", "" + title1.Rows[i]["N_NO"].ToString() + "");
                        tcHeader[i + 7].Attributes.Add("bgcolor", "#efefe7");
                        tcHeader[i + 7].Text = title1.Rows[i]["RFNames1"].ToString() + "</th></tr><tr>";
                    }
                }
                //second row                
                DataTable title2 = MESComment.proDailyOutputSql.Getlevel2TitleList(ddlfactoryCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value, ddlWashType.SelectedItem.Value, txtDate.Text, ddlprocessCd.SelectedItem.Value, strContent, strTitle, MESConn);
                for (int i = 0; i < title2.Rows.Count; i++)
                {
                    if (i != title2.Rows.Count - 1)
                    {
                        tcHeader.Add(new TableHeaderCell());
                        tcHeader[i + 7 + title1.Rows.Count].Attributes.Add("colspan", "" + title2.Rows[i]["N_NO2"].ToString() + "");
                        tcHeader[i + 7 + title1.Rows.Count].Attributes.Add("bgcolor", "#efefe7");
                        tcHeader[i + 7 + title1.Rows.Count].Text = title2.Rows[i]["RFNames2"].ToString();
                    }
                    else
                    {
                        tcHeader.Add(new TableHeaderCell());
                        tcHeader[i + 7 + title1.Rows.Count].Attributes.Add("colspan", "" + title2.Rows[i]["N_NO2"].ToString() + "");
                        tcHeader[i + 7 + title1.Rows.Count].Attributes.Add("bgcolor", "#efefe7");
                        tcHeader[i + 7 + title1.Rows.Count].Text = title2.Rows[i]["RFNames2"].ToString() + "</th></tr><tr>";
                    }
                }                
                DataTable title3 = MESComment.proDailyOutputSql.Getlevel3TitleList(ddlfactoryCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value, ddlWashType.SelectedItem.Value, txtDate.Text, ddlprocessCd.SelectedItem.Value, strContent, strTitle, MESConn);
                for (int i = 0; i < title3.Rows.Count; i++)
                {
                    if (i != title3.Rows.Count - 1)
                    {
                        tcHeader.Add(new TableHeaderCell());
                        tcHeader[i + 7 + title1.Rows.Count + title2.Rows.Count].Text = title3.Rows[i]["RFNames3"].ToString();
                        tcHeader[i + 7 + title1.Rows.Count + title2.Rows.Count].Attributes.Add("bgcolor", "#efefe7");
                    }
                    else
                    {
                        tcHeader.Add(new TableHeaderCell());
                        tcHeader[i + 7 + title1.Rows.Count + title2.Rows.Count].Text = title3.Rows[i]["RFNames3"].ToString() + "</th></tr><tr>";
                        tcHeader[i + 7 + title1.Rows.Count + title2.Rows.Count].Attributes.Add("bgcolor", "#efefe7");
                    }
                }
                break;
            case DataControlRowType.DataRow:
                if (array == null)
                {
                    array = new object[e.Row.Cells.Count];
                }
                for (int col = 0; col < e.Row.Cells.Count; col++)
                {
                    if (col < 7)
                    {
                        array[col] = "&nbsp";
                    }
                    array[5] = "Total :";
                    if (col >= 7)
                    {
                        if (array[col] == null)
                        {
                            array[col] = Int32.Parse(e.Row.Cells[col].Text.Equals("") ? "0" : e.Row.Cells[col].Text);
                        }
                        else
                        {
                            array[col] = Int32.Parse(e.Row.Cells[col].Text.Equals("") ? "0" : e.Row.Cells[col].Text) + Int32.Parse(array[col].ToString());
                        }
                    }
                }
                break;
            case DataControlRowType.Footer:
                for (int col = 0; col < e.Row.Cells.Count; col++)
                {
                    e.Row.Cells[col].Text = array[col].ToString();
                    e.Row.Cells[col].Attributes.Add("bgcolor", "#efefe7");
                }
                    break;
        }
    }
    protected void ddlgarmentType_SelectedIndexChanged(object sender, EventArgs e)
    {        
        ddlprocessCd.DataSource = MESComment.proDailyOutputSql.GetProcessCode(ddlfactoryCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value);
        ddlprocessCd.DataBind();
    }

   
}
