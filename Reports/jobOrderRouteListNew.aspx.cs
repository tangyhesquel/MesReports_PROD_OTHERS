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
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlfactoryCd.DataSource = MESComment.MesRpt.GetFactoryList("");
            ddlfactoryCd.DataBind();
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
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        int irowc;
        string JONO;
        irowc = 1;
        int ipage;
        ipage = 1;
        divBody.InnerHtml = "";
        divhead.InnerHtml = "";
        JONO = txtJoNo.Text;
        Page.Header.Title = "JO Route List Report(" + JONO + ")";
        DataTable dt_head = MESComment.MesRpt.GetJobOrderRouteHead(ddlfactoryCd.SelectedItem.Text, txtJoNo.Text.Trim());
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
            divhead.InnerHtml += "<td></td>";
            divhead.InnerHtml += "</tr><tr><td class='RouteListStyle1' >Style No.</td>";
            divhead.InnerHtml += "<td>" + row["STYLE_NO"] + "</td>";
            divhead.InnerHtml += "<td class='RouteListStyle2'>Style Description:</td>";
            divhead.InnerHtml += "<td colspan='7'>" + row["Style_Desc"] + "</td> ";
            divhead.InnerHtml += "</tr></table>";


        }
        DataTable CPLIST = MESComment.MesRpt.GetCustomReportList(ddlfactoryCd.SelectedValue, "REPORT", "REPORT", "JOROUTELISTREPORT", Request.QueryString["User_ID"].ToString());
        if (CPLIST.Rows.Count <= 0)
        {
            CPLIST = MESComment.MesRpt.GetCustomReportList(ddlfactoryCd.SelectedValue, "REPORT", "REPORT", "JOROUTELISTREPORT", "DEFAULT");
        }
        if (CPLIST.Rows.Count <= 0)
        {
            return;

        }
        DataTable dt = MESComment.MesRpt.GetJobOrderRouteListNew(ddlfactoryCd.SelectedItem.Text, txtDate.Text, txtJoNo.Text.Trim(), DDOrderby.SelectedItem.Value, "", CPLIST.Rows[0]["CUSTOM_VALUES"].ToString(), "");

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
                    divBody.InnerHtml += "<td class='tr2style' colspan='2'> JOB ORDER No:</td><td class='tr3style' colspan='2' > " + JONO + "</td><td class='tr3style' align='right' colspan='" + ((dt.Columns.Count - 4) >= 1 ? (dt.Columns.Count - 4) : 1) + "'> (Page:" + ipage + ")</td></tr><tr>";

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        divBody.InnerHtml += "<td class='style3'>" + dt.Columns[j].ColumnName + "</td>";
                    }
                    divBody.InnerHtml += "</tr>";
                    irowc = 1;
                }
            }

            divBody.InnerHtml += "<tr> ";
            for (int s = 0; s < dt.Columns.Count; s++)
            {
                divBody.InnerHtml += "<td class='xl25'>" + row[dt.Columns[s].ColumnName] + "</td>";
            }
            divBody.InnerHtml += "</tr>";
            irowc = irowc + 1;

        }

        divSummary.InnerHtml = "";
        DataTable dsp = MESComment.JobOrderRouteList.GetJobOrderRouteListPieceRateByProcess(ddlfactoryCd.SelectedItem.Text, txtDate.Text, txtJoNo.Text.Trim(), "", "");
        foreach (DataRow row1 in dsp.Rows)
        {
            divSummary.InnerHtml += "<tr> ";
            divSummary.InnerHtml += "<td colspan=2 class='tdbackcolor'>" + row1["PROCESS_CD"] + ":</td><td  colspan=2 align='right'>" + row1["PIECE_RATE"] + "</td></tr>";
        }
    }
}
