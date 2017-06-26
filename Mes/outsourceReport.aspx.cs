using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Mes_outsourceReport : pPage
{
    public string issueDate, factoryName, subcontractorName;
    protected void Page_Load(object sender, EventArgs e)
    {
        hfValue.Value = Request.QueryString["site"];
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divDetail1.InnerHtml = "";
        divDetail2.InnerHtml = "";
        DataTable dtHeader = MESComment.MesOutSourcePriceSql.GetOutsourceReportingHead(txtContractNo.Text.Trim());
        foreach (DataRow row in dtHeader.Rows)
        {
            subcontractorName = row["SUBCONTRACTOR_NAME"].ToString();
            issueDate = row["ISSUER_DATE"].ToString();
            factoryName = row["CHN_NAME"].ToString();
        }

        DataTable dtDetail = MESComment.MesOutSourcePriceSql.GetOutputQty(txtContractNo.Text.Trim());
        foreach (DataRow row in dtDetail.Rows)
        {
            divDetail1.InnerHtml += "<tr><td>" + row["GOOD_NAME"] + "</td><td>" + row["JOB_ORDER_NO"] + "</td><td>" + row["QTY"] + "</td><td>PCS</td></tr>";
        }
        divDetail2.InnerHtml = divDetail1.InnerHtml;
    }
}
