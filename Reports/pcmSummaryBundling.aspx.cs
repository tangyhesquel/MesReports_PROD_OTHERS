using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_pcmSummaryBundling : pPage
{
    public string ORDER_QTY, SC_NO, CUSTOMER_NAME;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlfactoryCd.DataSource = MESComment.MesRpt.GetFactoryCd();
            ddlfactoryCd.DataBind();
            LayNos.Text = "";//重置
        }
        if (Request.QueryString["site"] != null)
        {
            ddlfactoryCd.SelectedValue = Request.QueryString["site"].ToString();
        }
    }
    protected void btnQurey_Click(object sender, EventArgs e)
    {
        if (Check())
        {
            this.NoData.Visible = false;
            this.QueryMsg.Visible = false;
            setData();
        }
        else
        {
            this.QueryMsg.Visible = true;
            this.NoData.Visible = false;
        }
    }

    protected Boolean Check()
    {
        if (!txtJoNo.Text.Equals(""))
        {
            if (!txtStartDate.Text.Equals("") && txtEndDate.Text.Equals(""))
            {
                return false;
            }
            else if (txtStartDate.Text.Equals("") && !txtEndDate.Text.Equals(""))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    protected string getString()
    {
        string LayNo = LayNos.Text;

        if (!LayNo.Equals("") && LayNo .IndexOf (",")>=0)
        {
            LayNo = LayNo.Replace(",", "','");
        }
        return LayNo;
    }

    protected void setData()
    {
        divTitle.InnerHtml = "";
        divResult.InnerHtml = "";
        divTotal.InnerHtml = "";
        DataTable dtHeader = MESComment.MesRpt.GetSummaryBundlingHeader(txtJoNo.Text.Trim());
        foreach (DataRow row in dtHeader.Rows)
        {
            ORDER_QTY = row["ORDER_QTY"].ToString();
            SC_NO = row["SC_NO"].ToString();
            CUSTOMER_NAME = row["CUSTOMER_NAME"].ToString();
        }
        string LayNo = getString();
        DataTable dtTitle = MESComment.MesRpt.GetSummaryBundlingSize(ddlfactoryCd.SelectedItem.Value, txtlayNoFrom.Text.Trim(), txtlayNoTo.Text.Trim(), txtJoNo.Text.Trim(), LayNo);
        foreach (DataRow row in dtTitle.Rows)
        {
            divTitle.InnerHtml += "<td class='tr2style'>" + row[0] + "</td>";
        }
        string StartTime = txtStartDate.Text;
        string EndTime = txtEndDate.Text;
        DataTable dtResult = MESComment.MesRpt.GetSummaryBundling(ddlfactoryCd.SelectedItem.Value, txtlayNoFrom.Text.Trim(), txtlayNoTo.Text.Trim(), txtJoNo.Text.Trim(), LayNo, StartTime, EndTime);
        if (dtResult.Rows.Count > 0)
        {
            this.QueryMsg.Visible = false;
            this.NoData.Visible = false;
            this.ExcTable.Visible = true;
            DataTable dtDetail = new DataTable();
            dtDetail.Columns.Add("COLOR_CD");
            foreach (DataRow row in dtTitle.Rows)
            {
                DataColumn col = new DataColumn();
                col.ColumnName = row[0].ToString();
                dtDetail.Columns.Add(col);
            }
            dtDetail.Columns.Add("KEY_TOTAL");
            string colorCd = "";
            int total = 0;
            int j = -1;
            for (int i = 0; i < dtResult.Rows.Count; i++)
            {
                if (colorCd != dtResult.Rows[i]["COLOR_CD"].ToString())
                {
                    if (colorCd != "")
                    {
                        dtDetail.Rows[j]["KEY_TOTAL"] = total;
                    }
                    total = 0;
                    DataRow newRow = dtDetail.NewRow();
                    newRow["COLOR_CD"] = dtResult.Rows[i]["COLOR_CD"];
                    dtDetail.Rows.Add(newRow);
                    total += int.Parse(dtResult.Rows[i]["QTY"].ToString());
                    j++;
                    dtDetail.Rows[j][dtResult.Rows[i]["SIZE_CD"].ToString()] = dtResult.Rows[i]["QTY"];
                    colorCd = dtResult.Rows[i]["COLOR_CD"].ToString();
                }
                else
                {
                    dtDetail.Rows[j][dtResult.Rows[i]["SIZE_CD"].ToString()] = dtResult.Rows[i]["QTY"];
                    total += int.Parse(dtResult.Rows[i]["QTY"].ToString());
                }
                if (i == dtResult.Rows.Count - 1)
                {
                    dtDetail.Rows[j]["KEY_TOTAL"] = total;
                }
            }
            int[] lastTotal = new int[dtTitle.Rows.Count + 1];
            foreach (DataRow row in dtDetail.Rows)
            {
                divResult.InnerHtml += "<tr>";
                divResult.InnerHtml += "<td >" + row["COLOR_CD"] + "</td>";
                for (int i = 0; i <= dtTitle.Rows.Count; i++)
                {
                    if (i < dtTitle.Rows.Count)
                    {
                        divResult.InnerHtml += "<td align=right>" + row[dtTitle.Rows[i][0].ToString()] + "</td>";
                    }
                    lastTotal[i] += int.Parse(row[i + 1].ToString() == "" ? "0" : row[i + 1].ToString());
                }
                divResult.InnerHtml += "<td align=right>" + row["KEY_TOTAL"] + "</td>";
                divResult.InnerHtml += "</tr>";
            }
            for (int i = 0; i < lastTotal.Length; i++)
            {
                divTotal.InnerHtml += "<td class='tr2style' align=right>" + lastTotal[i] + "</td>";
            }
        }
        else
        {
            //this.ExcTable.Visible = false;
            this.QueryMsg.Visible = false;
            this.NoData.Visible = false;
        }
    }
    
}
