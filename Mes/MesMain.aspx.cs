using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using System.Data.Common;
using System.IO;


public partial class MES_MesMain : pPage
{

    string Facotry_Cd="";
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (!IsPostBack)
        {
            txtFactoryName.Visible = false;
            lblFactory.Visible = false;
            txtFactoryName.Text = "";
            txtCustomer.Visible = false;
            txtCustomer.Text = "";
            lblCustomer.Visible = false;
            btnShow.Visible = false;
            hfType.Value = "A";
            txtDate.Text = DateTime.Now.Date.ToShortDateString();
        }
        warning.Visible = false;
        if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
        {
            Facotry_Cd = Request.QueryString["site"].ToString();
        }
        else
        {
            Facotry_Cd = "GEG";
        }
    }
    protected void rblOrderBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        reportDiv.Visible = false;
        if (rblOrderBy.SelectedValue == "A")
        {
            txtFactoryName.Visible = false;
            lblFactory.Visible = false;
            txtFactoryName.Text = "";
            txtCustomer.Visible = false;
            txtCustomer.Text = "";
            lblCustomer.Visible = false;
            btnShow.Visible = false;
            hfType.Value = "A";
        }
        else
        {
            if (rblOrderBy.SelectedValue == "F")
            {
                txtFactoryName.Visible = true;
                lblFactory.Visible = true;
                txtCustomer.Visible = false;
                txtCustomer.Text = "";
                lblCustomer.Visible = false;
                btnShow.Visible = true;
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
                btnShow.Visible = true;
            }
        }
    }
    protected void btnEnter_Click(object sender, EventArgs e)
    {
        reportDiv.InnerHtml = "";
        string factoryCd = "%";
        string customerCd = "%";
        string garmentType = "%";
        string dateTime = "%";
        int isCheck = 0;
        if (txtFactoryName.Text != "")
        {
            factoryCd = txtFactoryName.Text.Trim();
        }
        if (txtCustomer.Text != "")
        {
            customerCd = txtCustomer.Text.Trim();
        }
        if (!txtDate.Text.Equals(""))
        {
            dateTime = txtDate.Text;
        }
        else
        {
            warning.Text = "请输入日期!";
            warning.Visible = true;
            return;
        }

        if (cbInclude.Checked)
        {
            isCheck = 1;
        }
        garmentType = ddlType.SelectedItem.Value;
        DataTable Detail = new DataTable();
        switch (hfType.Value)
        {
            case "A":
                //Detail = MESComment.MesOutSourcePriceSql.GetReportDetailByAll(CurrentSite,dateTime, garmentType, isCheck);//Added By ZouShiChang On 2013.09.03 MES024
                Detail = MESComment.MesOutSourcePriceSql.GetReportDetailByAll(Facotry_Cd, dateTime, garmentType, isCheck);
                break;
            case "F":
                Detail = MESComment.MesOutSourcePriceSql.GetReportDetailBySubcontractor(factoryCd, dateTime, garmentType, isCheck);
                break;
            case "C":
                Detail = MESComment.MesOutSourcePriceSql.GetReportDetailByCustomer(customerCd, dateTime, garmentType, isCheck);
                break;
        }
        if (Detail.Rows.Count > 0)
        {
            reportDiv.Visible = true;
            reportDiv.InnerHtml += "<table border='1' cellspacing='0' cellpadding='0' style='font-size:12px;border-collapse:collapse' width='100%'><tr><td colspan='8' align='center'>OutSourcing Price Monitor<td><tr>";
            foreach (DataRow row in Detail.Rows)
            {
                if (hfType.Value=="A")
                {
                    reportDiv.InnerHtml += "<tr><td rowspan='2' bgcolor='#efefe7'></td>";
                }
                else if (hfType.Value == "F")
                {
                    reportDiv.InnerHtml += "<tr><td rowspan='2' bgcolor='#efefe7'>" + row["SUBCONTRACTOR_NAME"] + "(" + row["SUBCONTRACTOR_CD"] + ")</td>";
                }
                else
                {
                    reportDiv.InnerHtml += "<tr><td rowspan='2' bgcolor='#efefe7'>" + row["Short_Name"] + "(" + row["CUSTOMER_CD"] + ")</td>";
                }
                reportDiv.InnerHtml += "<td colspan='4' bgcolor='#efefe7'>Contract Expect Recived Date</td><td colspan='3' bgcolor='#efefe7'>Actual Return Date</td></tr>";
                reportDiv.InnerHtml += "<tr><td bgcolor='#efefe7'>Month</td><td bgcolor='#efefe7'>YTD</td><td bgcolor='#efefe7'>Budget</td><td bgcolor='#efefe7'>Standard</td><td bgcolor='#efefe7'>Month</td><td bgcolor='#efefe7'>YTD</td>";
                DateTime dtime = DateTime.Parse(txtDate.Text);//以最后更改的日期为准;
                reportDiv.InnerHtml += "<td bgcolor='#efefe7'>YTD" + dtime.AddYears(-1).Year + "</td></tr>";
                reportDiv.InnerHtml += "<tr><td>AVG PRICE(RMB/PCS)</td><td>" + row["avg_price_m"] + "</td><td>" + row["avg_price_y"] + "</td><td>" + row["budget_price"] + "</td><td>" + row["standard_price"] + "</td><td>" + row["avg_price_act_m"] + "</td><td>" + row["avg_price_act_y"] + "</td><td>" + row["avg_price_last_y"] + "</td></tr>";
                reportDiv.InnerHtml += "<tr><td>AVG SAH(Sewing/garment)</td><td>" + row["avg_sew_sah_m"] + "</td><td>" + row["avg_sew_sah_y"] + "</td><td>" + row["budget_sew_sah"] + "</td><td>" + row["avg_sew_sah_y"] + "</td><td>" + row["avg_sew_sah_act_m"] + "</td><td>" + row["avg_sew_sah_act_y"] + "</td><td>" + row["avg_sew_sah_last_y"] + "</td></tr>";
                reportDiv.InnerHtml += "<tr><td>AVG SAH(Total/garment)</td><td>" + row["avg_sah_m"] + "</td><td>" + row["avg_sah_y"] + "</td><td>X</td><td>" + row["avg_sah_y"] + "</td><td>" + row["avg_sah_act_m"] + "</td><td>" + row["avg_sah_act_y"] + "</td><td>" + row["avg_sah_last_y"] + "</td></tr>";
                reportDiv.InnerHtml += "<tr><td>AVG SAH PRICE(RMB/Sewing SAH)</td><td>" + row["avg_sew_sah_price_m"] + "</td><td>" + row["avg_sew_sah_price_y"] + "</td><td>" + row["budget_sew_sah_price"] + "</td><td>" + row["standard_sah_price"] + "</td><td>" + row["avg_sew_sah_price_act_m"] + "</td><td>" + row["avg_sew_sah_price_act_y"] + "</td><td>" + row["avg_sew_sah_price_last_y"] + "</td></tr>";
                reportDiv.InnerHtml += "<tr><td colspan='8'><b>Bal.Qty:</b>" + row["bal_qty"] + "</td></tr>";
                reportDiv.InnerHtml += "<tr><td colspan='8'>&nbsp;</td></tr>";
            }
            reportDiv.InnerHtml += "</table>";
        }
    }
    protected void btnToDetail_Click(object sender, EventArgs e)
    {
        DataTable dt = new DataTable();
        switch (hfType.Value)
        {
            case "A":
                //dt = MESComment.MesOutSourcePriceSql.GetMesOutSourcePriceDetail(CurrentSite,txtDate.Text,ddlType.SelectedItem.Value,cbInclude.Checked);//Added By ZouShiChang On 2013.09.03 MES024
                dt = MESComment.MesOutSourcePriceSql.GetMesOutSourcePriceDetail(Facotry_Cd, txtDate.Text, ddlType.SelectedItem.Value, cbInclude.Checked); 
                break;
            case "F":
                dt = MESComment.MesOutSourcePriceSql.GetMesOutSourcePriceDetailBySUBCONTRACTOR(txtDate.Text,ddlType.SelectedItem.Value,cbInclude.Checked,txtFactoryName.Text);
                break ;
            case "C":
                dt = MESComment.MesOutSourcePriceSql.GetMesOutSourcePriceDetailByCustomer(txtDate.Text,ddlType.SelectedItem.Value,cbInclude.Checked,txtCustomer.Text);
                break;
        }
        GridView gvDetail = new GridView();
        gvDetail.DataSource = dt;
        gvDetail.DataBind();
        MESComment.Excel.ToExcel(gvDetail, "Detail Report" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
    }
    protected void btnExcel_Click(object sender, EventArgs e)
    {
        MESComment.Excel.ToExcel(reportDiv, "Outsourcing Price Monitor " + DateTime.Now.ToString("yyyyMMdd") + ".xls");
    }
    protected void txtCustomer_TextChanged(object sender, EventArgs e)
    {

    }
}
