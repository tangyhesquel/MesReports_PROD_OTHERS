using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Mes_outsourceFGISDif : pPage
{
    double Total_QTY = 0;
    double Total_QTY2 = 0;
    double Total_QtyDif = 0;
    string Row_Color = "#E3E3D3";
    public string Factory_CD = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                Factory_CD = Request.QueryString["site"].ToString();
            }
            else
            {
                Factory_CD = "GEG";
            }
            if (Factory_CD.Equals("DEV") && Request.QueryString["factory_cd"] != null)
            {
                Factory_CD = Request.QueryString["factory_cd"].ToString();                
            }
        }
        if (!IsPostBack)
        {
        }
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        SetData();
    }

    protected void SetData()
    {
        string StartDate = txtStartDate.Text;
        string EndDate = txtEndDate.Text;
        string JONO = txtJoNo.Text.Trim();
        string GarmentType = ddlGarmentType.SelectedValue.ToString();

        DataTable dt1 = MESComment.MesOutSourcePriceSql.GetOutsourceFGISDifForGEG2(StartDate, EndDate, JONO, Factory_CD,GarmentType);
        //DataTable dt2 = MESComment.MesOutSourcePriceSql.GetOutsourceFGISDifForGEG1(StartDate, EndDate, JONO);        
        ShowDetail(dt1);
    }

    protected void ShowDetail(DataTable dt1, DataTable dt2)
    {//旧方法；舍弃2012-08-24
        dt1.Columns.Remove("REMARK");
        dt1.Columns.Add("QTY2");
        dt1.Columns.Add("QTYDif");

        foreach (DataRow dr in dt2.Rows)
        {//找出全部FGIS中有但是MES中没有的JO+COLOR+SIZE;
            string JO2 = dr["JOB_ORDER_NO"].ToString();
            string COLOR2 = dr["COLOR_CODE"].ToString();
            string SIZE2 = dr["SIZE_CODE"].ToString();
            string SQL = "JOB_ORDER_NO = '" + JO2 + "' AND COLOR_CODE = '" + COLOR2 + "' AND SIZE_CODE = '" + SIZE2 + "'   ";
            DataRow[] FindDataRows2 = dt1.Select(SQL);
            if (FindDataRows2.Length <= 0)
            {
                dt1.Rows.Add(JO2, COLOR2, SIZE2, 0, 0, 0);
            }
        }

        foreach (DataRow dr in dt1.Rows)
        {//经过上面的补全JO+COLOR+SIZE之后,开始对MES两边的数据进行对比(以dt1为输出table);
            string JO = dr["JOB_ORDER_NO"].ToString();
            string COLOR = dr["COLOR_CODE"].ToString();
            string SIZE = dr["SIZE_CODE"].ToString();
            string SQL = "JOB_ORDER_NO = '" + JO + "' AND COLOR_CODE = '" + COLOR + "' AND SIZE_CODE = '" + SIZE + "'   ";
            DataRow[] FindDataRows = dt2.Select(SQL);
            if (FindDataRows.Length <= 0)
            {
                dr["QTY2"] = "0";
            }
            else
            {
                dr["QTY2"] = FindDataRows[0]["QTY"].ToString();
            }
            double QtyFromDT1 = Double.Parse((dr["QTY"].ToString().Equals("")) ? "0" : dr["QTY"].ToString());
            double QtyFromDT2 = Double.Parse((dr["QTY2"].ToString().Equals("")) ? "0" : dr["QTY2"].ToString());
            double QtyDif = QtyFromDT1 - QtyFromDT2;
            dr["QTYDif"] = QtyDif.ToString();

            Total_QTY += QtyFromDT1;
            Total_QTY2 += QtyFromDT2;
            Total_QtyDif += QtyDif;
        }

        string FilterZeroSQL = "QTYDif <>0";

        this.gvbody.DataSource = dt1.Select(FilterZeroSQL);//选出全部dif非零的数据;
        this.gvbody.DataBind();
    }

    protected void ShowDetail(DataTable dt1)
    {
        if (dt1.Rows.Count > 0)
        {
            foreach (DataRow dr in dt1.Rows)
            {
                Total_QTY += Double.Parse(dr["QTY"].ToString());
                Total_QTY2 += Double.Parse(dr["QTY2"].ToString());
                Total_QtyDif += Double.Parse(dr["QTYDIF"].ToString());
            }
            this.dvMsg.Visible = false;
            this.gvbody.Visible = true;
            this.gvbody.DataSource = dt1;
            this.gvbody.DataBind();
        }
        else
        {
            this.gvbody.Visible = false;            
            this.dvMsg.Visible = true;
        }
    }

    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:

                break;
            case DataControlRowType.DataRow:
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (i < 1)
                    {
                        e.Row.Cells[i].Attributes.Add("align", "center");
                        if (e.Row.Cells[0].Text.Contains("TOTAL:"))
                        {
                            e.Row.Attributes.Add("bgcolor", "#E3E3D3");
                        }
                    }
                    else
                    {
                        e.Row.Cells[i].Attributes.Add("align", "right");
                        if (e.Row.Cells[2].Text.Contains("TTL"))
                        {
                            e.Row.Attributes.Add("bgcolor", "#CCFFFF");
                        }
                    }
                }
                //Row_Color = (Row_Color.Equals("#E3E3D3")) ? "" : "#E3E3D3";
                //e.Row.Attributes.Add("bgcolor", Row_Color);
                break;
            case DataControlRowType.Footer:

                break;
        }
    }

}
