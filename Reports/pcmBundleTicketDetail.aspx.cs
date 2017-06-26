using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_PcmBundleTicketDetail : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //this.meg.Visible = false;
        if (!IsPostBack)
        {
            ddlFactoryCode.DataSource = MESComment.MesRpt.GetFactoryCd();
            ddlFactoryCode.DataTextField = "FACTORY_ID";
            ddlFactoryCode.DataValueField = "FACTORY_ID";
            ddlFactoryCode.DataBind();
        }
        if (Request.QueryString["site"] != null)
        {
            //ddlFactoryCode.SelectedValue = Request.QueryString["site"].ToUpper().ToString().Trim();
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                ddlFactoryCode.SelectedValue = Request.QueryString["site"].ToString();
            }
            else
            {
                ddlFactoryCode.SelectedValue = "GEG";
            }
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        if (check())
        {
            divBody.InnerHtml = "";
            string EndDate = "";
            if (!txtEndDate.Text.Equals(""))
            {
                EndDate = DateTime.Parse(txtEndDate.Text).AddDays(1).ToShortDateString();
            }
            DataTable dt = MESComment.MesRpt.GetBundleTicketDetail(ddlFactoryCode.SelectedItem.Value, txtLayNoFrom.Text.Trim(), txtLayNoTo.Text.Trim(), txtJobOrderNo.Text.Trim(), txtBeginDate.Text, EndDate,cbClearBundle.Checked);
            if (dt.Rows.Count > 0)
            {
                this.divExcTable.Visible = true;
                this.divMeg.Visible = false;
                divBody.InnerHtml += "<table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:12px;border-collapse:collapse'>";
                divBody.InnerHtml += "<tr><td class='tr2style'>Job Order No</td>";
                divBody.InnerHtml += "   <td class='tr2style'>Create User</td>";
                divBody.InnerHtml += "   <td class='tr2style'>Lay No</td>";
                divBody.InnerHtml += "   <td class='tr2style'>Bundle No</td>";
                divBody.InnerHtml += "   <td class='tr2style'>Color</td>";
                divBody.InnerHtml += "   <td class='tr2style'>Size</td>	";
                divBody.InnerHtml += "   <td class='tr2style'>Bundle Qty</td>";
                divBody.InnerHtml += "   <td class='tr2style'>Bundle Original Qty</td>";
                divBody.InnerHtml += "   <td class='tr2style'>Batch No</td>			";
                divBody.InnerHtml += "   <td class='tr2style'>Shade Lot</td>";
                divBody.InnerHtml += "   <td class='tr2style'>Roll ID</td>	";
                divBody.InnerHtml += "   <td class='tr2style'>Cut Line</td>	";
                divBody.InnerHtml += "   <td class='tr2style'>Sew Line</td>	";
                divBody.InnerHtml += "   <td class='tr2style'>Printing date</td>";
                divBody.InnerHtml += "   <td class='tr2style'>Trx Type</td>";
                divBody.InnerHtml += "</tr>";

                Double Bundle_Qty_Total = 0.0;
                Double Bundle_Original_Qty_Total = 0.0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    divBody.InnerHtml += "<tr>";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        //divBody.InnerHtml += "<td>&nbsp;" + dt.Rows[i][j] + "</td>";
                        divBody.InnerHtml += "<td>" + (dt.Rows[i][j].ToString() == "" ? "&nbsp;" : dt.Rows[i][j].ToString()) + "</td>";
                    }
                    Bundle_Qty_Total += Convert.ToDouble(dt.Rows[i]["BUNDLE_QTY"].ToString());
                    Bundle_Original_Qty_Total += Double.Parse(dt.Rows[i]["BUNDLE_ORIGINAL_QTY"].ToString());
                    divBody.InnerHtml += "</tr>";
                }
                divBody.InnerHtml += "<tr><td class='tr2style'  colspan='6'> Total:</td>";
                divBody.InnerHtml += "<td class='tr2style'>" + Bundle_Qty_Total + "</td><td class='tr2style'>" + Bundle_Original_Qty_Total + "</td>";
                divBody.InnerHtml += "<td class='tr2style' colspan='7'></td>";
                divBody.InnerHtml += "</tr></table>";
            }
            else
            {
                this.divExcTable.Visible = false;
                this.divMeg.Visible = true;
                this.divMeg.InnerHtml = "<table><tr><td align='center' style='color:Red; font-size:large'>No Data</td></tr></table>";
            }
        }
        else
        {
            this.divExcTable.Visible = false;
            this.divMeg.Visible = true;
            this.divMeg.InnerHtml = "<table><tr><td align='center' style='color:Red; font-size:large'>请输入有效的查询条件!</td></tr></table>";
        }
    }

    private Boolean check()
    {
        Boolean b = true;
        if ((txtBeginDate.Text.Equals("") && !txtEndDate.Text.Equals("")) || (!txtBeginDate.Text.Equals("") && txtEndDate.Text.Equals("")))
        {
            b = false;
        }
        else if (!txtBeginDate.Text.Equals("") && !txtEndDate.Text.Equals(""))
        {
            DateTime dtStart = DateTime.Parse(txtBeginDate.Text);
            DateTime dtEnd = DateTime.Parse(txtEndDate.Text);
            if (dtEnd >= dtStart)
            {
                b = true;
            }
            else
            {
                b = false;
            }
        }
        if (txtBeginDate.Text.Equals("") && txtEndDate.Text.Equals("") && txtJobOrderNo.Text.Equals("") && txtLayNoFrom.Text.Equals("") && txtLayNoTo.Text.Equals(""))
        {
            b = false;
        }
        return b;
    }
}
