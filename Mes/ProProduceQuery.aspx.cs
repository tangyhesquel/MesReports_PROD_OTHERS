using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Threading;
using System.Globalization;

public partial class Mes_ProProduceQuery : pPage
{
    string factoryCd = "";
    DataTable gvTitle = new DataTable();
    DataTable dtShipQTYAndWarehouseQTY = new DataTable();
    double [] Total;
    string strculture = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        AjaxPro.Utility.RegisterTypeForAjax(typeof(Mes_ProProduceQuery));
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                factoryCd = Request.QueryString["site"].ToString();
            }
            else
            {
                factoryCd = "GEG";
            }
            if (factoryCd == "PTX" || factoryCd == "EGM" || factoryCd == "TIL" || factoryCd == "EGV" || factoryCd == "EAV")
            {
                strculture = "en";
            }
            else
            {
                strculture = "zh";
            }
        }
        if (!IsPostBack)
        {
            ddlDept.DataTextField = "NM";
            ddlDept.DataValueField = "PRC_CD";
            ddlDept.DataSource = MESComment.ProTranWIPSummary.GetProTranWIPSummaryDept(factoryCd,ddlWipGarmentType.SelectedItem.Value);
            ddlDept.DataBind();
            ddlPart.DataTextField = "production_line_cd";
            ddlPart.DataValueField = "production_line_cd";
            ddlPart.DataSource = MESComment.ProTranWIPSummary.GetProTranWIPSummaryProductionLine(factoryCd,ddlWipGarmentType.SelectedItem.Value,ddlDept.SelectedItem.Value);
            ddlPart.DataBind();
            CheckBoxIncludeOutSource.Checked = MESComment.ProTranWIPSummary.IsIncludeOutsourceOutput(factoryCd);

            
            ddlProdFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
            ddlProdFactory.DataTextField = "FACTORY_ID";
            ddlProdFactory.DataValueField = "FACTORY_ID";
            ddlProdFactory.DataBind();

            ddlProcessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
            ddlProcessType.DataTextField = "PROCESS_TYPE_ID";
            ddlProcessType.DataValueField = "PROCESS_TYPE_VALUE";
            ddlProcessType.DataBind();


            if (strculture == "en")
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("zh-CN");

            }
        }
        txtGo.Text = txtGo.Text.ToUpper();
    }
    protected void btnWipQuery_Click(object sender, EventArgs e)
    {
        divNoCondition.Visible = false;
        if (txtGo.Text != "" || txtBeginDate.Text != "")
        {
            DataTable dtSource = new DataTable();
            dtSource = MESComment.ProTranWIPSummary.GetProTranWIPProductQty(factoryCd, txtGo.Text.Trim(), chbGo.Checked, "", ddlDept.SelectedItem.Value, ddlWipGarmentType.SelectedItem.Value,ddlProcessType.SelectedItem.Value,ddlProdFactory.SelectedItem.Value,ddlPart.SelectedItem.Value, txtBeginDate.Text, txtEndDate.Text, "Content", CBIsOutsource.Checked,CheckBoxIncludeOutSource.Checked);
            DataTable dtObject = new DataTable();
            dtObject.Columns.Add("TRX_DATE");
            gvTitle = MESComment.ProTranWIPSummary.GetProTranWIPProductQty(factoryCd, txtGo.Text.Trim(), chbGo.Checked, "", ddlDept.SelectedItem.Value, ddlWipGarmentType.SelectedItem.Value,ddlProcessType.SelectedItem.Value,ddlProdFactory.SelectedItem.Value,ddlPart.SelectedItem.Value, txtBeginDate.Text, txtEndDate.Text, "Title", CBIsOutsource.Checked, CheckBoxIncludeOutSource.Checked);
            //新增的Shipped;
            if (chkShippedQty.Checked)
            {
                gvTitle.Rows.Add(new object[] { "ShippedQty", 999 });
                dtShipQTYAndWarehouseQTY = MESComment.MesOutSourcePriceSql.GetShipQTYAndWarehouseQTY(txtBeginDate.Text.Trim(), txtEndDate.Text.Trim(), "", "", "", txtGo.Text, ddlWipGarmentType.SelectedItem.Value, "", factoryCd,"","","","","","", "SummaryByTrxDate", CBIsOutsource.Checked, "Produce");
            }
            foreach (DataRow row in gvTitle.Rows)
            {
                if(row[0].ToString()=="ShippedQty")
                {
                    dtObject.Columns.Add(row[0].ToString() + "(ForReference)");
                }
                else
                {
                    dtObject.Columns.Add(row[0].ToString());
                }               
            }
            string oldDate = "";
            int index = -1;
            foreach (DataRow row in dtSource.Rows)
            {
                if (oldDate != row["TRX_DATE"].ToString())
                {
                    index++;
                    DataRow newRow = dtObject.NewRow();
                    newRow["TRX_DATE"] = row["TRX_DATE"];
                    newRow[row["PROCESS_CD"].ToString()] = row["qty"];
                    oldDate = row["TRX_DATE"].ToString();
                    dtObject.Rows.Add(newRow);
                }
                dtObject.Rows[index][row["PROCESS_CD"].ToString()] = row["qty"];
                oldDate = row["TRX_DATE"].ToString();
               
            }
            if (chkShippedQty.Checked)
            {
                foreach (DataRow dr in dtObject.Rows)
                {//添加Shipped汇总;
                    string TRX_DATE = dr["TRX_DATE"].ToString();
                    string SQL = "TRANS_DATE ='" + TRX_DATE + "' ";
                    double Shipped_QTY = 0;
                    DataRow[] drsdtShipQTYAndWarehouseQTY = dtShipQTYAndWarehouseQTY.Select(SQL);
                    if (drsdtShipQTYAndWarehouseQTY.Length > 0)
                    {
                        for (int i = 0; i < drsdtShipQTYAndWarehouseQTY.Length; i++)
                        {
                            Shipped_QTY = double.Parse(drsdtShipQTYAndWarehouseQTY[i]["ship_qty"].ToString());
                        }
                    }
                    dr["ShippedQty(ForReference)"] = Shipped_QTY;
                }
                string Pre_Trx_Date = "";
                double Shipped_Qty = 0;
                foreach (DataRow dr in dtShipQTYAndWarehouseQTY.Rows)
                {
                    string TRX_DATE = DateTime.Parse(dr["TRANS_DATE"].ToString()).ToString("yyyy-MM-dd");
                    string SQL = "TRX_DATE ='" + TRX_DATE + "' ";
                    DataRow[] drObject = dtObject.Select(SQL);
                    if (drObject.Length == 0 && dr["ship_qty"].ToString() != "" && dr["ship_qty"].ToString() != "0")
                    {
                        if (Pre_Trx_Date != TRX_DATE && Pre_Trx_Date != "")
                        {
                            DataRow dr1 = dtObject.NewRow();
                            dr1["TRX_DATE"] = Pre_Trx_Date;
                            dr1["ShippedQty(ForReference)"] = Shipped_Qty;
                            dtObject.Rows.Add(dr1);
                            Shipped_Qty = 0;
                            Shipped_Qty += double.Parse(dr["ship_qty"].ToString());
                            Pre_Trx_Date = TRX_DATE;
                        }
                        else
                        {
                            Shipped_Qty += double.Parse(dr["ship_qty"].ToString());
                            Pre_Trx_Date = TRX_DATE;
                        }
                    }
                }
                if (Shipped_Qty != 0)
                {
                    DataRow dr0 = dtObject.NewRow();
                    dr0["TRX_DATE"] = Pre_Trx_Date;
                    dr0["ShippedQty(ForReference)"] = Shipped_Qty;
                    dtObject.Rows.Add(dr0);
                    Shipped_Qty = 0;
                }
            }
            Total = new double[gvTitle.Rows.Count];
            dtObject.DefaultView.Sort = "TRX_DATE";           
            gvBody.DataSource = dtObject.DefaultView;           
            gvBody.DataBind();
        }
        else
        {
            divNoCondition.Visible = true;
        }             
    }
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        string id = "";
        switch (e.Row.RowType)
        {
            case DataControlRowType.DataRow:
                for (int i = 0; i < gvTitle.Rows.Count; i++)
                {
                    //Line,factoryCd,strJo,strDept,GarmentType,ProcessType,ProdFactory,strPart,startDate,endDate,type,strGo,isGoLike,Total,isOSjo
                    // 0       1       2      3        4          5             6        7        8         9     10    11   12       13     14
                    //id = "" + "," + factoryCd + "," + "" + "," + gvTitle.Rows[i][0].ToString() + "," + ddlWipGarmentType.SelectedItem.Value + "," +ddlProcessType.SelectedItem.Value+","+ddlProdFactory.SelectedItem.Value+","+ ddlPart.SelectedItem.Value + "," + e.Row.Cells[0].Text + "," + e.Row.Cells[0].Text + "," + "Detail" + "," + txtGo.Text.Trim() + "," + chbGo.Checked + "," + e.Row.Cells[i + 1].Text + "," + CBIsOutsource.Checked;
                    id = "" + "," + factoryCd + "," + "" + "," + gvTitle.Rows[i][0].ToString() + "," + ddlWipGarmentType.SelectedItem.Value + "," + ddlProcessType.SelectedItem.Value + "," + ddlProdFactory.SelectedItem.Value + "," + ddlPart.SelectedItem.Value + "," + e.Row.Cells[0].Text + "," + e.Row.Cells[0].Text + "," + "Detail" + "," + txtGo.Text.Trim() + "," + chbGo.Checked + "," + e.Row.Cells[i + 1].Text + "," + CBIsOutsource.Checked + "," + CheckBoxIncludeOutSource.Checked;
                    if (e.Row.Cells[i + 1].Text == "&nbsp;")
                    {
                        e.Row.Cells[i + 1].Text = string.Format("<a href='#' onclick='window.open (\"ProTranWIPDetail.aspx?id={0}\",\"\",\"height=500, width=600, top=150, left=250, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no\")'>0</a>", id);
                        //e.Row.Cells[i + 1].Text = "<a href='ProTranWIPDetail.aspx?id=" + id + "'target='blank'>0</a>";
                    }
                    else
                    {
                        Total[i] += double.Parse(e.Row.Cells[i + 1].Text);
                        e.Row.Cells[i + 1].Text = string.Format("<a href='#' onclick='window.open (\"ProTranWIPDetail.aspx?id={0}\",\"\",\"height=500, width=600, top=150, left=250, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no\")'>{1}</a>", id, e.Row.Cells[i + 1].Text);
                        //e.Row.Cells[i + 1].Text = "<a href='ProTranWIPDetail.aspx?id=" + id + "'target='blank'>" + e.Row.Cells[i + 1].Text + "</a>";
                    }
                }
                break;
            case DataControlRowType.Footer:
                e.Row.Cells[0].Text = "Total";
                for (int i = 0; i < gvTitle.Rows.Count; i++)
                {
                    e.Row.Cells[i + 1].Text = Total[i].ToString();
                }
                    break;
        }
    }
    protected void ddlWipGarmentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlDept.DataTextField = "NM";
        ddlDept.DataValueField = "PRC_CD";
        ddlDept.DataSource = MESComment.ProTranWIPSummary.GetProTranWIPSummaryDept(factoryCd,ddlWipGarmentType.SelectedItem.Value);
        ddlDept.DataBind();
        ddlPart.DataTextField = "production_line_cd";
        ddlPart.DataValueField = "production_line_cd";
        ddlPart.DataSource = MESComment.ProTranWIPSummary.GetProTranWIPSummaryProductionLine(factoryCd, ddlWipGarmentType.SelectedItem.Value, ddlPart.SelectedItem.Value);
        ddlPart.DataBind();
    }
    protected void ddlDept_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlPart.DataTextField = "production_line_cd";
        ddlPart.DataValueField = "production_line_cd";
        ddlPart.DataSource = MESComment.ProTranWIPSummary.GetProTranWIPSummaryProductionLine(factoryCd,ddlWipGarmentType.SelectedItem.Value,ddlDept.SelectedItem.Value);
        ddlPart.DataBind();
    }
    protected void gvBody_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}
