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
using System.Xml.Linq;


public partial class Reports_mesMarkerCSBreakdown : pPage
{
    public string Factory_CD = "", Process_CD = "CUT", Garment_Type = "", Production_Line = "", Process_Type = "";

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
            //Batch8 - MES070
            //Get Production Line drop down
            this.ddlProductionLine.DataSource = MESComment.CuttingDcWipDetail.GetProductionLineWoven(Factory_CD, ddProcessCd.SelectedItem.Value, ddGarmentType.SelectedItem.Value);
            this.ddlProductionLine.DataBind();
        }
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.divBody.InnerHtml = "";
        this.divMsg.InnerHtml = "";
        this.gvBody.Visible = true;

        this.Process_CD = ddProcessCd.Text;
        this.Garment_Type = ddGarmentType.SelectedItem.Value;
        this.Process_Type = ddProcessType.SelectedItem.Value;
        this.Production_Line = this.ddlProductionLine.SelectedValue.ToString();
        DbConnection MESConn = MESComment.DBUtility.GetConnection("MES");

        DataTable dtData = MESComment.CuttingDcWipDetail.GetCuttingDcWIPDetailWoven(Factory_CD, Process_CD, Production_Line, Garment_Type, Process_Type, "", MESConn);

        DataTable dtDataSum = dtData.DefaultView.ToTable(false, new string[] { });
        dtDataSum.Columns.Remove("JOB_ORDER_NO");
        dtDataSum.Columns.Remove("ORDERQTY");
        dtDataSum.Columns.Remove("CUTQTY");
        var query = from q in dtDataSum.AsEnumerable()
                    group q by q.Field<string>("Production_line") into g
                    select new
                    {
                        Production_LineBy = g.Key,
                        CutWipGroupBy = g.Sum(q => q.Field<int>("CutWip")),
                        CutAssortWipGroupBy = g.Sum(q => q.Field<int>("CutAssortWip")),
                        SewWipGroupBy = g.Sum(q => q.Field<int>("SewWip"))
                    };
        DataTable dt1 = dtDataSum.Clone();
        DataRow dr;
        foreach (var re in query)
        {
            dr = dt1.NewRow();
            dr["Production_line"] = re.Production_LineBy;
            dr["CutWip"] = re.CutWipGroupBy;
            dr["CutAssortWip"] = re.CutAssortWipGroupBy;
            dr["SewWip"] = re.SewWipGroupBy;
            dt1.Rows.Add(dr);
        }
        dt1.DefaultView.Sort = "Production_line";
        dtDataSum.Clear();
        dtDataSum = dt1;
        //2013.06.06隐藏配套WIP CutAssortWip
        dtData.Columns.Remove("CutAssortWip");
        dtDataSum.Columns.Remove("CutAssortWip");

        MESComment.DBUtility.CloseConnection(ref MESConn);
        dtData.DefaultView.Sort = " Production_line ASC";
        if (dtData.Rows.Count >= 1)
        {
            this.gvBody.DataSource = dtData;
            this.gvBody.DataBind();
            this.divGvBody.Visible = true;

            //if (ddProcessCd.Text == "CUT")
                //dtDataSum.Columns.Remove("SewWip");
            //else if (ddProcessCd.Text == "SEW")
                //dtDataSum.Columns.Remove("CutWip");
            this.gvBodySum.DataSource = dtDataSum;
            this.gvBodySum.DataBind();
            this.divGvBodySum.Visible = true;
        }
        else
        {
            this.divGvBody.Visible = false;
            this.divGvBodySum.Visible = false;
            this.divBody.InnerHtml += "<table width='100%'  style='color:Red; font-size:large'><tr><td align='center'><b>No Data</b></td></tr></table>";
        }

    }

    //string gvBodyColumnName = "车缝组别,制单号,制单数,裁数,裁床WIP,裁床配套WIP,车缝WIP</TR><TR>";

    //string gvBodySumColumnName = "车缝组别,裁床WIP,裁床配套WIP,车缝WIP</TR><TR>";

    string gvBodyColumnName = "组别,制单号,制单数,裁数,裁床WIP,车缝WIP</TR><TR>";

    //string gvBodySumColumnName = "组别,CUTW,SEWW</TR><TR>";

    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (FlagList.SelectedValue)
        {//选择显示语言;
            case "English":
                gvBodyColumnName = "Production Line, Job Order No, Order Qty,Actual Cut,Cut WIP,Sew WIP</TR><TR>";
                break;
            case "Chinese":
                gvBodyColumnName = "组别,制单号,制单数,裁数,裁床WIP,车缝WIP</TR><TR>";
                break;
        }

        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                string[] Name = gvBodyColumnName.Split(',');
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();
                for (int j = 0; j <= 5; j++)
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

                //Batch 8, MES-070
                for (int j = 1; j < gvBody.Rows.Count; j++)
                {
                    GridViewRow row = gvBody.Rows[j];
                    GridViewRow previousRow = gvBody.Rows[j - 1];

                    if (previousRow.Cells[1].Text == row.Cells[1].Text)
                    {
                        if (ddProcessCd.Text == "CUT")
                        {
                            if (previousRow.Cells[2].RowSpan == 0 && previousRow.Cells[3].RowSpan == 0 && previousRow.Cells[5].RowSpan == 0)
                            {
                                previousRow.Cells[2].RowSpan += 2;
                                previousRow.Cells[3].RowSpan += 2;
                                previousRow.Cells[5].RowSpan += 2;
                            }
                            else
                            {
                                previousRow.Cells[2].RowSpan = row.Cells[2].RowSpan + 2;
                                previousRow.Cells[3].RowSpan = row.Cells[3].RowSpan + 2;
                                previousRow.Cells[5].RowSpan = row.Cells[5].RowSpan + 2;
                            }
                            row.Cells[2].Visible = false;
                            row.Cells[3].Visible = false;
                            row.Cells[5].Visible = false;
                        }
                        else
                        {
                            if (previousRow.Cells[2].RowSpan == 0 && previousRow.Cells[3].RowSpan == 0 && previousRow.Cells[4].RowSpan == 0)
                            {
                                previousRow.Cells[2].RowSpan += 2;
                                previousRow.Cells[3].RowSpan += 2;
                                previousRow.Cells[4].RowSpan += 2;
                            }
                            else
                            {
                                previousRow.Cells[2].RowSpan = row.Cells[2].RowSpan + 2;
                                previousRow.Cells[3].RowSpan = row.Cells[3].RowSpan + 2;
                                previousRow.Cells[4].RowSpan = row.Cells[4].RowSpan + 2;
                            }
                            row.Cells[2].Visible = false;
                            row.Cells[3].Visible = false;
                            row.Cells[4].Visible = false;
                        }
                    }
                }

                break;
            case DataControlRowType.Footer:
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Attributes.Add("bgcolor", "#efefe7");
                    e.Row.Cells[i].Attributes.Add("align", "right");

                }
                int[] intSumQty = new int[4];
                if (e.Row.RowType == DataControlRowType.Footer)
                {

                    for (int i = 0; i < this.gvBody.Rows.Count; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            intSumQty[j] += Convert.ToInt32(gvBody.Rows[i].Cells[j + 2].Text);
                        }

                    }

                    for (int i = 0; i < this.gvBody.Rows.Count; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            e.Row.Cells[j + 2].Text = intSumQty[j].ToString();

                        }

                    }

                    switch (FlagList.SelectedValue)
                    {//选择显示语言;
                        case "English":
                            e.Row.Cells[0].Text = "Total";
                            break;
                        case "Chinese":
                            e.Row.Cells[0].Text = "合计";
                            break;
                    }
                }
                break;
        }
    }

    //Batch8-MES070
    protected void ddProcessCd_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlProductionLine.DataSource = MESComment.CuttingDcWipDetail.GetProductionLineWoven(Factory_CD, ddProcessCd.SelectedItem.Value, ddGarmentType.SelectedItem.Value);
        ddlProductionLine.DataBind();
    }

    string gvBodySumColumnName = "组别, 裁床WIP, 车缝WIP</TR><TR>";

    protected void gvBodySum_RowDataBound(object sender, GridViewRowEventArgs e)
    {


        switch (FlagList.SelectedValue)
        {//选择显示语言;
            case "English":
                //if (ddProcessCd.Text == "CUT")
                    //gvBodySumColumnName = "Production Line, Cut WIP</TR><TR>";
                //else if (ddProcessCd.Text == "SEW")
                    //gvBodySumColumnName = "Production Line, Sew WIP</TR><TR>";
                //else
                    gvBodySumColumnName = "Production Line, Cut WIP, Sew WIP</TR><TR>";
                break;
            case "Chinese":
                //if (ddProcessCd.Text == "CUT")
                    //gvBodySumColumnName = "组别, 裁床WIP</TR><TR>";
                //else if (ddProcessCd.Text == "SEW")
                    //gvBodySumColumnName = "组别, 车缝WIP</TR><TR>";
                //else
                    gvBodySumColumnName = "组别, 裁床WIP, 车缝WIP</TR><TR>";
                break;
        }

        int rowCount = 0;
        //if (ddProcessCd.Text == "ALL")
            rowCount = 2;
        //else
            //rowCount = 1;

        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                string[] Name = gvBodySumColumnName.Split(',');
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();
                for (int j = 0; j <= rowCount; j++)
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

                int[] intSumQty = new int[2];

                //if (ddProcessCd.Text != "ALL")
                //{
                //    intSumQty = new int[1];
                //}


                if (e.Row.RowType == DataControlRowType.Footer)
                {

                    for (int i = 0; i < this.gvBodySum.Rows.Count; i++)
                    {
                        for (int j = 0; j < e.Row.Cells.Count - 1; j++)
                        {
                            intSumQty[j] += Convert.ToInt32(gvBodySum.Rows[i].Cells[j + 1].Text);
                        }

                    }

                    for (int i = 0; i < this.gvBodySum.Rows.Count; i++)
                    {
                        for (int j = 0; j < e.Row.Cells.Count - 1; j++)
                        {
                            e.Row.Cells[j + 1].Text = intSumQty[j].ToString();

                        }

                    }
                    switch (FlagList.SelectedValue)
                    {//选择显示语言;
                        case "English":
                            e.Row.Cells[0].Text = "Total";
                            break;
                        case "Chinese":
                            e.Row.Cells[0].Text = "合计";
                            break;
                    }
                }

                break;
        }
    }
}

