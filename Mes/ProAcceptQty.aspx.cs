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

public partial class Mes_ProAcceptQty : pPage
{
    string strFactoryCd = "";
    DataTable dtTitle = new DataTable();
    DataTable dtShipQTYAndWarehouseQTY = new DataTable();
    string strculture = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {           
            if (Request.QueryString["site"] != null)
            {
                if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
                {
                    strFactoryCd = Request.QueryString["site"].ToString();
                }
                else
                {
                    strFactoryCd = "GEG";
                }
                hfValue.Value = strFactoryCd;
                if (strFactoryCd == "PTX" || strFactoryCd == "EGM" || strFactoryCd == "TIL" || strFactoryCd == "EGV" || strFactoryCd == "EAV")
                {
                    strculture = "en";
                }
                else
                {
                    strculture = "zh";
                }
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
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divNoCondition.Visible = false;
        Random ro = new Random(1000);
        string strTableName = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();

        if (txtTrxBeginDate.Text != "" && txtBeginDate.Text != "")
        {
            Response.Write("<script>alert('Can not choice Two Type Date!')</script>");
        }
        else if (txtTrxBeginDate.Text != "")
        {
            generateTrxBody();
        }
        else if (txtBeginDate.Text != "")
        {
            DataTable dtSource = MESComment.ProTranWIPSummary.GetProTranWIPAcceptQty(strTableName, hfValue.Value, ddlGarmentType.SelectedItem.Value, txtBeginDate.Text, txtEndDate.Text, CBIsOutsource.Checked);
            if (chkWareHouse.Checked)
            {
                dtShipQTYAndWarehouseQTY = MESComment.MesOutSourcePriceSql.GetShipQTYAndWarehouseQTY(txtTrxBeginDate.Text, txtTrxEndDate.Text, "", txtBeginDate.Text, txtEndDate.Text, "", ddlGarmentType.SelectedItem.Value, "", hfValue.Value,"","","","","","", "SummaryByJo", CBIsOutsource.Checked, "Accept");
                dtSource.Columns.Add("Warehouse");
                if (dtShipQTYAndWarehouseQTY.Rows.Count > 0)
                    foreach (DataRow row in dtSource.Rows)
                    {
                        DataRow[] rows = dtShipQTYAndWarehouseQTY.Select("job_order_no='" + row["jo"] + "'");
                        if (rows.Length > 0)
                        {
                            row["Warehouse"] = rows[0]["warehouse"];
                        }
                    }
                string Job_Order_No = "";
                double Ware_House = 0;
                foreach (DataRow dr in dtShipQTYAndWarehouseQTY.Rows)
                {
                    string JoNo = dr["job_order_no"].ToString();
                    string SQL = "jo ='" + JoNo + "' ";
                    DataRow[] drObject = dtSource.Select(SQL);
                    if (drObject.Length == 0 && dr["warehouse"].ToString() != "" && dr["warehouse"].ToString() != "0")
                    {
                        if (Job_Order_No != JoNo && Job_Order_No != "")
                        {
                            DataRow dr1 = dtSource.NewRow();
                            dr1["jo"] = Job_Order_No;
                            dr1["Warehouse"] = Ware_House;
                            dr1["AUDIT"] = "";
                            dr1[""] = "";
                            dtSource.Rows.Add(dr1);
                            Ware_House = 0;
                            Ware_House += double.Parse(dr["Warehouse"].ToString());
                            Job_Order_No = JoNo;
                        }
                        else
                        {
                            Ware_House += double.Parse(dr["Warehouse"].ToString());
                            Job_Order_No = JoNo;
                        }
                    }
                }
                if (Ware_House != 0)
                {
                    DataRow dr0 = dtSource.NewRow();
                    dr0["jo"] = Job_Order_No;
                    dr0["Warehouse"] = Ware_House;
                    dr0["AUDIT"] = "";
                    dr0["CLOSING"] = "";
                    dtSource.Rows.Add(dr0);
                    Ware_House = 0;
                }
            }
            gvBody.DataSource = dtSource;
            gvBody.DataBind();
        }
        else
        {
            divNoCondition.Visible = true;
        }
    }

    private void generateTrxBody()
    {
        DataTable dtSource = new DataTable();
        dtSource = MESComment.ProTranWIPSummary.GetProTranWIPAcceptQtyTrxBody(hfValue.Value, ddlGarmentType.SelectedItem.Value, txtTrxBeginDate.Text, txtTrxEndDate.Text, CBIsOutsource.Checked);
        DataTable dtObject = new DataTable();
        dtObject.Columns.Add("TRX_DATE");
        dtTitle = MESComment.ProTranWIPSummary.GetProTranWIPAcceptQtyTrxTitle(hfValue.Value, ddlGarmentType.SelectedItem.Value);
        if (chkWareHouse.Checked)
        {
            dtShipQTYAndWarehouseQTY = MESComment.MesOutSourcePriceSql.GetShipQTYAndWarehouseQTY(txtTrxBeginDate.Text, txtTrxEndDate.Text, "", "", "", "", ddlGarmentType.SelectedItem.Value, "", hfValue.Value,"","","","","","", "SummaryByTrxDate", CBIsOutsource.Checked, "Accept");
            dtTitle.Rows.Add(new object[] { "Warehouse", 999 });//新增Warhouse;
        }
        else
        {
            dtShipQTYAndWarehouseQTY = new DataTable();
        }
        foreach (DataRow row in dtTitle.Rows)
        {
            dtObject.Columns.Add(row[0].ToString());
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
                newRow[row["NEXT_PROCESS_CD"].ToString()] = row["in_qty"];
                oldDate = row["TRX_DATE"].ToString();
                dtObject.Rows.Add(newRow);
            }
            dtObject.Rows[index][row["NEXT_PROCESS_CD"].ToString()] = row["in_qty"];
            oldDate = row["TRX_DATE"].ToString();
        }
        if (chkWareHouse.Checked)
            foreach (DataRow dr in dtObject.Rows)
            {
                string TRX_DATE = dr["TRX_DATE"].ToString();
                string SQL = "trans_date = '" + TRX_DATE + "' ";
                int IN_QTY = 0;
                DataRow[] drsdtShipQTYAndWarehouseQTY = dtShipQTYAndWarehouseQTY.Select(SQL);
                if (drsdtShipQTYAndWarehouseQTY.Length > 0)
                {
                    for (int i = 0; i < drsdtShipQTYAndWarehouseQTY.Length; i++)
                    {
                        IN_QTY += Int32.Parse(drsdtShipQTYAndWarehouseQTY[i]["warehouse"].ToString().Equals("") ? "0" : drsdtShipQTYAndWarehouseQTY[i]["warehouse"].ToString());
                    }
                }
                dr["Warehouse"] = IN_QTY;
            }
        gvBody.DataSource = dtObject;
        gvBody.DataBind();
    }

    string[] processCd = new string[50];
    int[] total = new int[50];

    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    processCd[i] = e.Row.Cells[i].Text;
                }
                break;
            case DataControlRowType.DataRow:
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    total[i] = total[i] + int.Parse(e.Row.Cells[i].Text == "&nbsp;" ? "0" : e.Row.Cells[i].Text);
                    if (txtBeginDate.Text != "")//按货期

                    {
                        if (i > 2)
                        {
                            e.Row.Cells[i].Text = string.Format("<a href='#' onclick='window.open (\"ProAcceptQtyDetail.aspx?id={0}\",\"\",\"height=300, width=600, top=150, left=250, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no\")'>{1}</a>", hfValue.Value + ";" + e.Row.Cells[0].Text + ";" + processCd[i] + ";" + ddlGarmentType.SelectedItem.Value + ";" + "BPO" + ";" + txtBeginDate.Text + ";" + txtEndDate.Text + ";" + CBIsOutsource.Checked, e.Row.Cells[i].Text);
                        }
                    }
                    else//按日期

                    {
                        e.Row.Cells[i].Text = string.Format("<a href='#' onclick='window.open (\"ProAcceptQtyDetail.aspx?id={0}\",\"\",\"height=300, width=600, top=150, left=250, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no\")'>{1}</a>", hfValue.Value + ";" + "" + ";" + processCd[i] + ";" + ddlGarmentType.SelectedItem.Value + ";" + "TRX" + ";" + e.Row.Cells[0].Text + ";" + e.Row.Cells[0].Text + ";" + CBIsOutsource.Checked, e.Row.Cells[i].Text);
                    }
                }
                break;
            case DataControlRowType.Footer:
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (i == 0)
                    {
                        e.Row.Cells[i].Text = "Total";
                    }
                    else
                    {
                        e.Row.Cells[i].Text = total[i].ToString();
                    }
                }
                break;
        }
    }
}
