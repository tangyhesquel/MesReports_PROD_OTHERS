using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_jobOrderRouteList : pPage
{
    public string user_id;
    public double Multiple = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlfactoryCd.DataSource = MESComment.MesRpt.GetFactoryList("");
            ddlfactoryCd.DataBind();
            string fty = Request.QueryString["site"].ToString();
            if (!fty.Equals(""))
            {
                fty = fty.Trim();
            }

            mestxt.Visible = false;
            if (fty != null || fty != "")
            {
                DDPARTCD.DataSource = MESComment.JobOrderRouteList.GetPARTList(fty);
                DDPARTCD.DataBind();
            }

            this.DDPROCESSCD.DataSource = MESComment.JobOrderRouteList.GetJoRouteListProcessCd(fty, DDGARMENTTYPE.SelectedItem.Value);
            this.DDPROCESSCD.DataBind();


            this.DDLProdLine.DataSource = MESComment.JobOrderRouteList.GetProdLineCd(fty, "");
            this.DDLProdLine.DataBind();

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
        if (Request.QueryString["User_ID"] != null)
        {
            user_id = Request.QueryString["User_ID"].ToString();
        }
        //Added by ZK 2014-09-11 as request from EMI
        if (Request.QueryString["JO_No"] != null)
        {
            txtJoNo.Text = Request.QueryString["JO_No"].ToString();
            GetData();
        }
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        string JO_NO = txtJoNo.Text.ToString().Trim();
        string DATE = txtDate.Text.ToString().Trim();
        GetData();
    }

    private void GetData()
    {
        int irowc;
        string JONO;
        irowc = 1;
        int ipage;
        ipage = 1;
        divBody.InnerHtml = "";
        divhead.InnerHtml = "";
        JONO = txtJoNo.Text;

        bool blnShowLineCD = false;
        bool blnShowLineNA = false;

        Page.Header.Title = "JO Route List Report(" + JONO + ")";
        DataTable dt_head = MESComment.JobOrderRouteList.GetJobOrderRouteHead(ddlfactoryCd.SelectedItem.Text, txtJoNo.Text.Trim());
        foreach (DataRow row in dt_head.Rows)
        {
            divhead.InnerHtml = "<table width='100%' border='1' cellspacing='1' cellpadding='1' style='font-size: 12px;border-collapse: collapse'>";
            divhead.InnerHtml += "<tr><td class='RouteListStyle1'>Fty CD:</td>";
            divhead.InnerHtml += "<td>" + ddlfactoryCd.SelectedItem.Text + "</td>";
            divhead.InnerHtml += "<td class='RouteListStyle2'>User:</td>";
            divhead.InnerHtml += "<td>" + row["USERID"] + "</td>";
            divhead.InnerHtml += "<td class='RouteListStyle'>Create Date:</td>";
            divhead.InnerHtml += "<td>" + txtDate.Text + "</td> ";
            divhead.InnerHtml += "<td class='RouteListStyle'>Printed Date:</td>";
            divhead.InnerHtml += "<td>" + row["DatePrint"] + "</td>      ";
            divhead.InnerHtml += "<td class='RouteListStyle'>Job Order No.</td>";
            divhead.InnerHtml += "<td>" + row["JOB_ORDER_NO"] + "</td>";
            divhead.InnerHtml += "</tr><tr><td class='RouteListStyle1' width='7%'>GO No.</td>";
            divhead.InnerHtml += "<td>" + row["GO_NO"] + "</td>";
            divhead.InnerHtml += "<td class='RouteListStyle2'> Job Order Date -entry</td>";
            divhead.InnerHtml += "<td>" + row["Job_Order_Date"] + "</td>";
            divhead.InnerHtml += "<td class='RouteListStyle'>Order Qty (Pcs) :</td>";
            divhead.InnerHtml += "<td>" + row["ORDER_QTY_P"] + "</td>";
            divhead.InnerHtml += "<td class='RouteListStyle'>Order Qty (Doz):</td>";
            divhead.InnerHtml += "<td>" + row["ORDER_QTY_D"] + "</td>";
            divhead.InnerHtml += "<td class='RouteListStyle'>Wash Type:</td>";
            divhead.InnerHtml += "<td>" + row["WASHTYPE"] + "</td>";
            divhead.InnerHtml += "</tr><tr><td class='RouteListStyle1'>Buyer Code:</td>";
            divhead.InnerHtml += "<td>" + row["BUYER_CODE"] + "</td>";
            divhead.InnerHtml += "<td class='RouteListStyle2'>Buyer name:</td>";
            divhead.InnerHtml += "<td colspan='3'>" + row["BUYER_NAME"] + "</td>";
            divhead.InnerHtml += "<td class='RouteListStyle'>FDS No:</td>";
            divhead.InnerHtml += "<td>" + row["FDS_No"] + "</td>";
            divhead.InnerHtml += "<td class='RouteListStyle'>Production Line:</td>";
            divhead.InnerHtml += "<td>" + this.DDLProdLine.SelectedItem.Text + "</td>";
            divhead.InnerHtml += "</tr><tr><td class='RouteListStyle1' >Style No.</td>";
            divhead.InnerHtml += "<td>" + row["STYLE_NO"] + "</td>";
            divhead.InnerHtml += "<td class='RouteListStyle2'>Style Description:</td>";
            divhead.InnerHtml += "<td colspan='5'>" + row["Style_Desc"] + "</td> ";
            divhead.InnerHtml += "<td class='RouteListStyle2'>Process Code:</td>";
            divhead.InnerHtml += "<td >" + this.DDPROCESSCD.SelectedItem.Text + "</td> ";
            divhead.InnerHtml += "</tr><tr><td class='RouteListStyle2'>Remark:</td>";
            divhead.InnerHtml += "<td colspan='9'>" + row["REMARK"] + "</td> ";
            divhead.InnerHtml += "</tr></table>";
        }

        DataTable CPLIST = MESComment.MesRpt.GetCustomReportList(ddlfactoryCd.SelectedValue, "REPORT", "REPORT", "JOROUTELISTREPORT", Request.QueryString["User_ID"].ToString());

        if (CPLIST.Rows.Count <= 0)
        {

            CPLIST = MESComment.MesRpt.GetCustomReportList(ddlfactoryCd.SelectedValue, "REPORT", "REPORT", "JOROUTELISTREPORT", "DEFAULT");
            if (CPLIST.Rows.Count <= 0)
            {
                CPLIST = MESComment.MesRpt.GetCustomReportList(ddlfactoryCd.SelectedValue, "REPORT", "REPORT", "JOROUTELISTREPORT", "DEFAULT");
            }
        }
        if (CPLIST.Rows.Count <= 0)
        {
            return;

        }
        string Cstr = CPLIST.Rows[0]["CUSTOM_VALUES"].ToString();

        string process_cd = this.DDPROCESSCD.SelectedValue.Trim();
        string Prod_Line_Cd = this.DDLProdLine.SelectedValue.Trim();
        bool ShowBlankLine = this.ShowBlankLine.Checked;

        if (Cstr == "")
        { Cstr = "*"; }
        DataTable dt = MESComment.JobOrderRouteList.GetJobOrderRouteList(ddlfactoryCd.SelectedItem.Text, txtDate.Text, txtJoNo.Text.Trim(), DDOrderby.SelectedItem.Value, DDPARTCD.SelectedItem.Value, Cstr, process_cd, Prod_Line_Cd, ShowBlankLine);

        divBody.InnerHtml += "<tr>";

        for (int j = 0; j < dt.Columns.Count; j++)
        {
            divBody.InnerHtml += "<td class='tr2style'>" + dt.Columns[j].ColumnName + "</td>";
        }
        divBody.InnerHtml += "</tr>";
        foreach (DataRow row in dt.Rows)
        {
            if (RowNo.Checked)
            {
                if (TxtRowNo.Text == "")
                {
                    TxtRowNo.Text = "50";
                }
                if (irowc >= int.Parse(TxtRowNo.Text.ToString()))
                {
                    ipage = ipage + 1;
                    divBody.InnerHtml += "<tr style='page-break-after: always'><td> </td></tr><tr>";
                    divBody.InnerHtml += "<td class='tr2style' colspan='2'> JOB ORDER No:</td><td class='tr3style' colspan='2' > " + JONO + "</td><td class='tr3style' align='right' colspan='" + ((dt.Columns.Count - 4) >= 1 ? (dt.Columns.Count - 4) + 2 : 1 + 2) + "'> (Page:" + ipage + ")</td></tr><tr>";

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        divBody.InnerHtml += "<td class='style3'>" + dt.Columns[j].ColumnName + "</td>";
                    }
                    divBody.InnerHtml += "</tr>";
                    irowc = 1;
                }
            }

            divBody.InnerHtml += "<tr> ";
            string colname = "";

            for (int s = 0; s < dt.Columns.Count; s++)
            {
                colname = dt.Columns[s].ColumnName.ToString().Trim();
                if (colname.Equals("PROD_LINE_CD"))
                {
                    blnShowLineCD = true;
                }
                if (colname.Equals("PROD_LINE_NAME"))
                {
                    blnShowLineNA = true;
                }
                if (colname.Equals("Piece Rate / 10pcs") || colname.Equals("Piece/Hr") || colname.Equals("90% Piece/Hr") || colname.Equals("80% Piece/Hr"))
                {
                    Multiple = 10;
                    double ble = Double.Parse(row[colname].ToString().Trim());
                    divBody.InnerHtml += "<td class='xl25'>" + ble.ToString("###,##0.0000") + "</td>";
                }
                else
                {
                    divBody.InnerHtml += "<td class='xl25'>" + row[dt.Columns[s].ColumnName] + "</td>";
                }
            }
            divBody.InnerHtml += "</tr>";
            irowc = irowc + 1;

        }
        divSummary.InnerHtml = "";
        DataTable dsp = MESComment.JobOrderRouteList.GetJobOrderRouteListPieceRateByProcess_EX(ddlfactoryCd.SelectedItem.Text, txtDate.Text, txtJoNo.Text.Trim(), DDPARTCD.SelectedItem.Value, process_cd, Prod_Line_Cd, ShowBlankLine);
        if (Multiple != 0)
        {
            divSummary.InnerHtml += "<tr><td colspan=2 class='style3'>PROCESS_CD</td>";
            if (blnShowLineCD.Equals(true))
            {
                divSummary.InnerHtml += "<td colspan=2 class='style3'>Prod_Line_Cd</td>";
            }
            if (blnShowLineNA.Equals(true))
            {
                divSummary.InnerHtml += "<td colspan=2 class='style3'>Prod_Line_Name</td>";
            }
            divSummary.InnerHtml += "<td colspan=2 class='style3'>SAM</td>";
            divSummary.InnerHtml += "<td colspan=2 class='style3'>PIECE_RATE</td><td colspan=2 class='style3'>PieceRate_Total</td>";
        }
        else
        {
            divSummary.InnerHtml += "<tr><td colspan=2 class='style3'>PROCESS_CD</td>";
            if (blnShowLineCD.Equals(true))
            {
                divSummary.InnerHtml += "<td colspan=2 class='style3'>Prod_Line_Cd</td>";
            }
            if (blnShowLineNA.Equals(true))
            {
                divSummary.InnerHtml += "<td colspan=2 class='style3'>Prod_Line_Name</td>";
            }
            divSummary.InnerHtml += "<td colspan=2 class='style3'>SAM</td>";
            divSummary.InnerHtml += "<td colspan=2 class='style3'>PIECE_RATE</td>";
        }
        divSummary.InnerHtml += "</tr><tr>";
        foreach (DataRow row1 in dsp.Rows)
        {
            if (Multiple != 0)
            {
                divSummary.InnerHtml += "<td colspan=2 class='tdbackcolor'>" + row1["PROCESS_CD"] + "</td>";
                if (blnShowLineCD.Equals(true))
                {
                    divSummary.InnerHtml += "<td colspan=2 class='tdbackcolor'>" + row1["LINE_CD"] + "</td>";
                }
                if (blnShowLineNA.Equals(true))
                {
                    divSummary.InnerHtml += "<td colspan=2 class='tdbackcolor'>" + row1["PRODUCTION_LINE_NAME"] + "</td>";
                }
                divSummary.InnerHtml += "<td colspan=2 align='right'>" + row1["SAM"] + "</td>";
                divSummary.InnerHtml += "<td  colspan=2 align='right'>" + row1["PIECE_RATE"] + "</td><td  colspan=2 align='right'>" + row1["PieceRate_Total"] + "</td>";
            }
            else
            {
                divSummary.InnerHtml += "<td colspan=2 class='tdbackcolor'>" + row1["PROCESS_CD"] + "</td>";
                if (blnShowLineCD.Equals(true))
                {
                    divSummary.InnerHtml += "<td colspan=2 class='tdbackcolor'>" + row1["LINE_CD"] + "</td>";
                }
                if (blnShowLineNA.Equals(true))
                {
                    divSummary.InnerHtml += "<td colspan=2 class='tdbackcolor'>" + row1["PRODUCTION_LINE_NAME"] + "</td>";
                }
                divSummary.InnerHtml += "<td colspan=2 align='right'>" + row1["SAM"] + "</td>";
                divSummary.InnerHtml += "<td  colspan=3 align='right'>" + row1["PIECE_RATE"] + "</td>";
            }
            divSummary.InnerHtml += "</tr>";
        }
        divSummary.InnerHtml += "<tr></tr>";
    }


    protected void txtJoNo_TextChanged(object sender, EventArgs e)
    {
        string Factory_Cd = this.ddlfactoryCd.SelectedValue.Trim();
        string JobOrderNo = this.txtJoNo.Text.Trim();

        this.DDLProdLine.DataSource = MESComment.JobOrderRouteList.GetProdLineCd(Factory_Cd, JobOrderNo);
        this.DDLProdLine.DataBind();
    }
    protected void DDGARMENTTYPE_SelectedIndexChanged(object sender, EventArgs e)
    {
        string Factory_cd = this.ddlfactoryCd.SelectedValue.Trim();        
        this.DDPROCESSCD.DataSource = MESComment.JobOrderRouteList.GetJoRouteListProcessCd(Factory_cd, DDGARMENTTYPE.SelectedItem.Value);
        this.DDPROCESSCD.DataBind();
    }
}
