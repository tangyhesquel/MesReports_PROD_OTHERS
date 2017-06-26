using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;

public partial class Mes_mesSpreadingSummary : pPage
{
    Boolean showNewReport = false;
    public string factoryCD = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        factoryCD = Request.QueryString["site"];
        if (factoryCD != null)
        {
            if (factoryCD.Contains("YMG"))
            {
                factoryCD = "YMG";
            }
            if (factoryCD.ToUpper().Equals("EAV") || factoryCD.ToUpper().Equals("EGV") || factoryCD.ToUpper().Equals("YMG"))
            {
                showNewReport = true;
            }
        }
        if (!IsPostBack)
        {
            string moNo = Request.QueryString["moNo"];
            if (moNo != "" && moNo != null)
            {
                this.txtMoNo.Text = moNo;
                btnQuery_Click(this.btnQuery, null);
            }
            this.cbNewReport.Visible = showNewReport;
            if (showNewReport)
            {
                this.cbNewReport.Checked = true;
            }
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divBody.InnerHtml = "";
        DataTable dtDetail = new DataTable();
        if (showNewReport && this.cbNewReport.Checked)
        {
                dtDetail = MESComment.MesOutSourcePriceSql.GetSpreadingSummaryInfoForEAV(txtMoNo.Text, txtJoNo.Text.Trim(), txtfromCutLotNo.Text.Trim(), txttoCutLotNo.Text.Trim());
        }
        else
        {
            dtDetail = MESComment.MesOutSourcePriceSql.GetSpreadingSummaryInfo(txtMoNo.Text, txtJoNo.Text.Trim(), txtfromCutLotNo.Text.Trim(), txttoCutLotNo.Text.Trim());
        }
        if (dtDetail.Rows.Count > 0)
        {
            if (showNewReport && this.cbNewReport.Checked)
            {
                ShowInReportForEAV(dtDetail);
            }
            else
            {
                ShowInReport(dtDetail);
            }
        }

    }

    protected void ShowInReport(DataTable dt)
    {
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow row in dt.Rows)
            {
                divBody.InnerHtml += "<tr>";
                divBody.InnerHtml += "<td>" + row["MO_NO"] + "</td>";
                divBody.InnerHtml += "<td>" + row["Customer"] + "</td>";
                divBody.InnerHtml += "<td>" + row["JO_NO"] + "</td>";
                divBody.InnerHtml += "<td  align='right'>" + row["COLOR_CD"] + "</td>";
                divBody.InnerHtml += "<td>" + row["STYLING"] + "</td>";
                divBody.InnerHtml += "<td>" + row["CUT_LOT_NO"] + "</td>";
                divBody.InnerHtml += "<td  align='right'>" + row["MARKER_LEN"] + "</td>";
                divBody.InnerHtml += "<td>" + row["QTY_LAY"] + "</td>";
                divBody.InnerHtml += "<td>" + row["QTY_ROLL"] + "</td>";
                divBody.InnerHtml += "<td  align='right'>" + row["PLYS"] + "</td>";
                divBody.InnerHtml += "<td  align='right'>" + row["SIZE_NUM"] + "</td>";
                divBody.InnerHtml += "<td>" + row["COLOR_L_D"] + "</td>";
                divBody.InnerHtml += "<td  align='right'>" + row["GMT_QTY"] + "</td>";
                divBody.InnerHtml += "<td  align='center'>" + row["MARKER_ID"] + "</td>";
                divBody.InnerHtml += "<td  align='right'>" + Double.Parse(row["FABQTY"].ToString()).ToString("#,###.00") + "</td>";
                divBody.InnerHtml += "<td  align='right'>" + Double.Parse(row["TotalFABQTY"].ToString()).ToString("#,###.00") + "</td>";
                divBody.InnerHtml += "<td  align='center'>" + row["WIDTH"] + "</td>";
                divBody.InnerHtml += "<td>" + row["FABRIC_DESC"] + "</td>";
                if (!(factoryCD.Contains("EGM")||factoryCD.Contains("DEV")))
                {
                    divBody.InnerHtml += "<td align='center'>" + row["BATCH_NO"] + "</td>";
                    divBody.InnerHtml += "<td>" + row["SIZE_RATIO"] + "</td>";
                }
                    divBody.InnerHtml += "</tr>";
            }
        }
    }


    protected void ShowInReportForEAV(DataTable dt)
    {
        if (dt.Rows.Count > 0)
        {
            double FABQTY = 0;
            double TotalFABQTY = 0;
            //Get All the JOs In The Same Cut_Lot_NO
            string Cut_Lot_NO = "";
            for (int row = 0; row < dt.Rows.Count; row++)
            {
                if (!Cut_Lot_NO.Contains(dt.Rows[row]["Cut_Lot_NO"].ToString()))
                {
                    Cut_Lot_NO += dt.Rows[row]["Cut_Lot_NO"].ToString() + ",";
                }
            }
            string[] Cut_Lot_NOs = Cut_Lot_NO.Split(',');
            for (int row = 0; row < Cut_Lot_NOs.Length - 1; row++)
            {
                string JO = "";
                Cut_Lot_NO = Cut_Lot_NOs[row].ToString();
                string sql = "Cut_Lot_NO = '" + Cut_Lot_NO + "'";
                DataRow[] dt_JOs = dt.Select(sql);
                foreach (DataRow rw in dt_JOs)
                {
                    if (!JO.Contains(rw["JO_NO"].ToString()))
                    {
                        if (JO.Equals(""))
                        {
                            JO += rw["JO_NO"].ToString();
                        }
                        else
                        {
                            JO += "/<br/>" + rw["JO_NO"].ToString();
                        }
                    }
                }
                FABQTY = Double.Parse(dt_JOs[0]["MARKER_LEN"].ToString()) * Double.Parse(dt_JOs[0]["PLYS"].ToString());
                TotalFABQTY = FABQTY * Double.Parse(dt_JOs[0]["MARKER_WASTAGE"].ToString());
                divBody.InnerHtml += "<tr>";
                divBody.InnerHtml += "<td>" + dt_JOs[0]["MO_NO"] + "</td>";
                divBody.InnerHtml += "<td>" + dt_JOs[0]["Customer"] + "</td>";
                divBody.InnerHtml += "<td>" + JO + "</td>";
                divBody.InnerHtml += "<td  align='right'>" + dt_JOs[0]["COLOR_CD"] + "</td>";
                divBody.InnerHtml += "<td>" + dt_JOs[0]["STYLING"] + "</td>";
                divBody.InnerHtml += "<td>" + Cut_Lot_NO + "</td>";
                divBody.InnerHtml += "<td  align='right'>" + dt_JOs[0]["MARKER_LEN"] + "</td>";
                divBody.InnerHtml += "<td>" + dt_JOs[0]["QTY_LAY"] + "</td>";
                divBody.InnerHtml += "<td>" + dt_JOs[0]["QTY_ROLL"] + "</td>";
                divBody.InnerHtml += "<td  align='right'>" + dt_JOs[0]["PLYS"] + "</td>";
                divBody.InnerHtml += "<td  align='right'>" + dt_JOs[0]["SIZE_NUM"] + "</td>";
                divBody.InnerHtml += "<td>" + dt_JOs[0]["COLOR_L_D"] + "</td>";
                divBody.InnerHtml += "<td  align='right'>" + dt_JOs[0]["GMT_QTY"] + "</td>";
                divBody.InnerHtml += "<td  align='center'>" + dt_JOs[0]["MARKER_ID"] + "</td>";
                divBody.InnerHtml += "<td  align='right'>" + FABQTY.ToString("#,###.##") + "</td>";
                divBody.InnerHtml += "<td  align='right'>" + TotalFABQTY.ToString("#,###.##") + "</td>";
                divBody.InnerHtml += "<td  align='center'>" + dt_JOs[0]["WIDTH"] + "</td>";
                divBody.InnerHtml += "<td>" + dt_JOs[0]["FABRIC_DESC"] + "</td>";
                divBody.InnerHtml += "<td align='center'>" + dt_JOs[0]["BATCH_NO"] + "</td>";
                divBody.InnerHtml += "<td>" + dt_JOs[0]["SIZE_RATIO"] + "</td>";
                divBody.InnerHtml += "</tr>";
            }
        }
    }
}
