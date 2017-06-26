using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;

public partial class Mes_MasOutSourcePriceReports : pPage
{
    string OldValue = "";
    string facotry_cd;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            
        }
        if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
        {
            facotry_cd = Request.QueryString["site"].ToString();
        }
        else
        {
            facotry_cd = "GEG";
        }
    }
    protected void gvBody_RowCreated(object sender, GridViewRowEventArgs e)
    {
        string HeaderStr = "";
        switch (rblOrderBy.SelectedValue)
        {
            case "F":
                HeaderStr = "工厂;客户;合同号;GO NO; 合同单价(RMB/件);合同数量;标准单价(RMB/件);SAH(Sew+Iron&Pack);SAH(Sewing);预算平均单价(RMB/件);预算工厂平均单价(RMB/件);计划回货期";
                break;
            case "C":
                HeaderStr = "客户;工厂;合同号;GO NO; 合同单价(RMB/件);合同数量;标准单价(RMB/件);SAH(Sew+Iron&Pack);SAH(Sewing);预算平均单价(RMB/件);预算工厂平均单价(RMB/件);计划回货期";
                break;
        }
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();
                for (int i = 0; i < HeaderStr.Split(';').Length-1; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("rowspan","2");
                    tcHeader[i].Text = HeaderStr.Split(';')[i].ToString();
                }
                tcHeader.Add(new TableHeaderCell());
                tcHeader[11].Attributes.Add("colspan","2");
                tcHeader[11].Text = "差异1(合同价格-标准价格)";
                tcHeader.Add(new TableHeaderCell());
                tcHeader[12].Attributes.Add("colspan","2");
                tcHeader[12].Text = "差异2(合同价格-预算工厂平均价格)";

                tcHeader.Add(new TableHeaderCell());
                tcHeader[13].Attributes.Add("rowspan", "2");
                tcHeader[13].Text = "计划回货期</tr><tr>";
                

                tcHeader.Add(new TableHeaderCell());
                tcHeader[14].Text = "单价(RMB/件)";
                tcHeader.Add(new TableHeaderCell());
                tcHeader[15].Text = "金额(RMB)";
                tcHeader.Add(new TableHeaderCell());
                tcHeader[16].Text = "单价(RMB/件)";
                tcHeader.Add(new TableHeaderCell());
                tcHeader[17].Text = "金额(RMB)";

                
                break;
            case DataControlRowType.DataRow:

                break;
        }
    }
    private void SetQuery()
    {
        double Cell4 = 0, Cell5 = 0, Cell6 = 0, Cell7 = 0, Cell8 = 0;
        gvBody.Visible = true;
        DataTable dt;
        if (rblOrderBy.SelectedValue == "F")
        {
            //dt = MESComment.MesOutSourcePriceSql.GetMasOutSourceDetail("F",CurrentSite, txtDate.Text, ddlType.SelectedItem.Value, txtFactoryName.Text, txtCustomer.Text);
            dt = MESComment.MesOutSourcePriceSql.GetMasOutSourceDetail("F", facotry_cd, txtDate.Text, ddlType.SelectedItem.Value, txtFactoryName.Text, txtCustomer.Text);
        }
        else
        {
            //dt = MESComment.MesOutSourcePriceSql.GetMasOutSourceDetail("C", CurrentSite, txtDate.Text, ddlType.SelectedItem.Value, txtFactoryName.Text, txtCustomer.Text);
            dt = MESComment.MesOutSourcePriceSql.GetMasOutSourceDetail("C", facotry_cd, txtDate.Text, ddlType.SelectedItem.Value, txtFactoryName.Text, txtCustomer.Text);
        }
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i <= dt.Rows.Count; i++)
            {
                string CurrentValue = "";
                try
                {
                    CurrentValue = dt.Rows[i][0].ToString();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    CurrentValue = "";
                }
                finally
                {

                }
                if (CurrentValue != OldValue && i != 0)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = "Sub Total";
                    dr[1] = "";
                    dr[2] = "";
                    dr[4] = Math.Round((Cell4 / Cell5), 2, MidpointRounding.AwayFromZero);
                    dr[5] = Cell5;
                    dr[6] = Math.Round((Cell6 / Cell5), 2, MidpointRounding.AwayFromZero);
                    dr[7] = Math.Round((Cell7 / Cell5), 4, MidpointRounding.AwayFromZero);
                    dr[8] = Math.Round((Cell8 / Cell5), 4, MidpointRounding.AwayFromZero);
                    dt.Rows.InsertAt(dr, i);
                    if (dt.Rows.Count > i + 1)
                    {
                        OldValue = dt.Rows[i + 1][0].ToString();
                    }
                    else
                    {
                        break;
                    }
                    Cell4 = 0; Cell5 = 0; Cell6 = 0; Cell7 = 0; Cell8 = 0;
                }
                else
                {
                    Cell4 += double.Parse(dt.Rows[i][4].ToString()) * double.Parse(dt.Rows[i][5].ToString());
                    Cell6 += double.Parse(dt.Rows[i][6].ToString()) * double.Parse(dt.Rows[i][5].ToString());
                    Cell7 += double.Parse(dt.Rows[i][7].ToString()) * double.Parse(dt.Rows[i][5].ToString());
                    Cell8 += double.Parse(dt.Rows[i][8].ToString()) * double.Parse(dt.Rows[i][5].ToString());
                    Cell5 += double.Parse(dt.Rows[i][5].ToString());
                    OldValue = dt.Rows[i][0].ToString();
                }

            }
        }
        gvBody.DataSource = dt;
        gvBody.DataBind();
    }
    protected void btnEnter_Click(object sender, EventArgs e)
    {
        
        SetQuery();
    }
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                //e.Row.BackColor = Color.LightGray;
                break;
            case DataControlRowType.DataRow:
                if (e.Row.Cells[0].Text == "Sub Total")
                {
                    e.Row.BackColor = Color.LightGreen;
                }
                break;
        }
    }
    protected void rblOrderBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        gvBody.Visible = false;
        if (rblOrderBy.SelectedValue == "F")
        {
            txtFactoryName.Visible = true;
            lblFactory.Visible = true;
            txtCustomer.Visible = false;
            txtCustomer.Text = "";
            lblCustomer.Visible = false;
            hfType.Value = "F";
        }
        else
        {
            hfType.Value = "C";
            txtFactoryName.Visible = false;
            txtFactoryName.Text = "";
            lblFactory.Visible = false;
            txtCustomer.Visible = true;
            lblCustomer.Visible = true;
        }
    }
    public override void VerifyRenderingInServerForm(Control control)
    {

    }
    protected void btnExcel_Click(object sender, EventArgs e)
    {
        SetQuery();
        MESComment.Excel.ToExcel(this.gvBody, "MesOutSourceDetailReport" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
    }
}
