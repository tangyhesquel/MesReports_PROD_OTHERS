using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using MESComment;
using System.Data.Common;

public partial class Reports_mesMarkerCSBreakdown : pPage
{
    public string Factory_CD = "", Process_CD = "CUT", Garment_Type = "", Production_Line = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Request.QueryString["site"] != null)
        {
            this.Factory_CD = this.Request.QueryString["site"].ToString().ToUpper();
            if (Factory_CD.Equals("DEV"))
            {
                this.Factory_CD = this.Request.QueryString["factory"].ToString().ToUpper();
            }
        }
        if (this.Request.QueryString["process"] != null)
        {
            this.Process_CD = this.Request.QueryString["process"].ToString().ToUpper();
        }
        if (!this.IsPostBack)
        {
            this.ddlProductionLine.DataSource = MESComment.CuttingDcWipDetail.GetProductionLineKnit();
            this.ddlProductionLine.DataBind();
        }
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.divBody.InnerHtml = "";
        this.divMsg.InnerHtml = "";
        this.gvBody.Visible = true;
        if (CHECK())
        {
            this.Process_CD = "CUT";
            this.Garment_Type = "K";
            this.Production_Line = this.ddlProductionLine.SelectedValue.ToString();
            string Date = this.txtSubmitDate.Text;
            DbConnection MESConn = MESComment.DBUtility.GetConnection("MES");

            DataTable[] dtData = new DataTable[2];
            dtData = MESComment.CuttingDcWipDetail.GetCuttingDcWIPDetailKnit(Factory_CD, Process_CD, Production_Line, Garment_Type, Date, MESConn);
            MESComment.DBUtility.CloseConnection(ref MESConn);
            if (dtData[0].Rows.Count >= 1)
            {
                this.gvBody.DataSource = dtData[0];
                this.gvBody.DataBind();
                this.divGvBody.Visible = true;
            }
            else
            {
                this.divGvBody.Visible = false;
                this.divGvBodySum.Visible = false;
                this.divBody.InnerHtml += "<table width='100%'  style='color:Red; font-size:large'><tr><td align='center'><b>No Data</b></td></tr></table>";
            }

            if (dtData[1].Rows.Count >= 1)
            {
                this.gvBodySum.DataSource = dtData[1];
                this.gvBodySum.DataBind();
                this.divGvBodySum.Visible = true;
            }
        }
        else
        {
            this.divMsg.InnerHtml += "<table width='100%'  style='color:Red; font-size:12px'><tr><td align='left'>PLS Choose the correct date!</td></tr></table>";
        }
    }

    string gvBodyColumnName = "车缝组别,制单号,制单数,裁数,DC Scan-In,DC区之前JoWIP数,DC Scan-Out,Dc区内JoWIP数,DC区内各车缝组WIP</TR><TR>,当天,累计";

    string gvBodySumColumnName = "车缝组别,各组日需求量,DC区内各车缝组WIP,WIP天数</TR><TR>";

    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                string[] Name = gvBodyColumnName.Split(',');
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();
                for (int j = 0; j <= 3; j++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[j].Attributes.Add("rowspan", "2");
                    tcHeader[j].Text = Name[j].ToString();
                    tcHeader[j].Attributes.Add("bgcolor", "#F7F6F3");
                }

                tcHeader.Add(new TableHeaderCell());
                tcHeader[4].Attributes.Add("colspan", "2");
                tcHeader[4].Text = Name[4].ToString();
                tcHeader[4].Attributes.Add("bgcolor", "#F7F6F3");

                tcHeader.Add(new TableHeaderCell());
                tcHeader[5].Attributes.Add("rowspan", "2");
                tcHeader[5].Text = Name[5].ToString();
                tcHeader[5].Attributes.Add("bgcolor", "#F7F6F3");

                tcHeader.Add(new TableHeaderCell());
                tcHeader[6].Attributes.Add("colspan", "2");
                tcHeader[6].Text = Name[6].ToString();
                tcHeader[6].Attributes.Add("bgcolor", "#F7F6F3");

                for (int j = 7; j <= 8; j++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[j].Attributes.Add("rowspan", "2");
                    tcHeader[j].Text = Name[j].ToString();
                    tcHeader[j].Attributes.Add("bgcolor", "#F7F6F3");
                }

                for (int j = 9; j <= 10; j++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[j].Text = Name[j].ToString();
                    tcHeader[j].Attributes.Add("bgcolor", "#F7F6F3");
                }

                for (int j = 9; j <= 10; j++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[j + 2].Text = Name[j].ToString();
                    tcHeader[j + 2].Attributes.Add("bgcolor", "#F7F6F3");
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
                break;

            case DataControlRowType.Footer:
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Attributes.Add("bgcolor", "#efefe7");
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }
                int[] intSumQty = new int[8];

                if (e.Row.RowType == DataControlRowType.Footer)
                {

                    for (int i = 0; i < this.gvBody.Rows.Count; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            intSumQty[j] += Convert.ToInt32(gvBody.Rows[i].Cells[j + 2].Text);
                        }
                    }

                    for (int i = 0; i < this.gvBody.Rows.Count; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            e.Row.Cells[j + 2].Text = intSumQty[j].ToString();

                        }
                    }
                    e.Row.Cells[0].Text = "合计";

                }
                break;
        }
        GroupRows(gvBody, 0);

        GroupRows(gvBody, 10);
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

    protected void gvBodySum_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                string[] Name = gvBodySumColumnName.Split(',');
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();
                for (int j = 0; j <= 3; j++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[j].Attributes.Add("rowspan", "2");
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
                break;

            case DataControlRowType.Footer:
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Attributes.Add("bgcolor", "#efefe7");
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }
                double[] intSumQty = new double[2];
                double DecimalDayQty = 0.0;
                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    for (int i = 0; i < this.gvBodySum.Rows.Count; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            intSumQty[j] += Convert.ToInt32(gvBodySum.Rows[i].Cells[j + 1].Text);
                        }
                    }
                    DecimalDayQty = Math.Round(intSumQty[1] / intSumQty[0], 1);
                    for (int i = 0; i < this.gvBodySum.Rows.Count; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            e.Row.Cells[j + 1].Text = intSumQty[j].ToString();
                        }
                    }
                    e.Row.Cells[0].Text = "合计";
                    e.Row.Cells[3].Text = DecimalDayQty.ToString();
                }
                break;
        }
    }


    /// <summary> 
    /// 合并GridView中某列相同信息的行（单元格） 
    /// </summary> 
    /// <param name="GridView1">GridView</param> 
    /// <param name="cellNum">第几列</param> 
    public static void GroupRows(GridView GridView1, int cellNum)
    {
        int i = 0, rowSpanNum = 1;
        while (i < GridView1.Rows.Count - 1)
        {
            GridViewRow gvr = GridView1.Rows[i];

            for (++i; i < GridView1.Rows.Count; i++)
            {
                GridViewRow gvrNext = GridView1.Rows[i];
                if (gvr.Cells[cellNum].Text == gvrNext.Cells[cellNum].Text)
                {
                    gvrNext.Cells[cellNum].Visible = false;
                    rowSpanNum++;
                }
                else
                {
                    gvr.Cells[cellNum].RowSpan = rowSpanNum;
                    rowSpanNum = 1;
                    break;
                }

                if (i == GridView1.Rows.Count - 1)
                {
                    gvr.Cells[cellNum].RowSpan = rowSpanNum;
                }
            }
        }
    }

}
