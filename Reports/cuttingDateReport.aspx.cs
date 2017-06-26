using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_CuttingDateReport : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlFactory.DataSource = MESComment.MesRpt.GetFactoryCd();
            ddlFactory.DataTextField = "FACTORY_ID";
            ddlFactory.DataValueField = "FACTORY_ID";
            ddlFactory.DataBind();
            if (base.CurrentSite != "")
            {
                ddlFactory.SelectedValue = base.CurrentSite;
            }
            if (ddlFactory.SelectedValue == "TIL")
            {
                cbLine.Checked = true;
            }

            //Added by MunFoong on 2014.07.24, MES-139
            if (ddlFactory.SelectedValue == "PTX") //if (Request.QueryString["site"].ToString() == "DEV")
            {
                this.ddGroupName.Enabled = true;
                this.ddGroupName.DataSource = MESComment.wipDailySql.GetGroupName(ddlFactory.SelectedValue);
                //this.ddGroupName.DataSource = MESComment.wipDailySql.GetGroupName("GEG");
                this.ddGroupName.DataTextField = "SYSTEM_KEY";
                this.ddGroupName.DataValueField = "SYSTEM_KEY";
                ddGroupName.DataBind();
            }
            else
            {
                this.ddGroupName.Enabled = false;
            }
            //End of added by MunFoong on 2014.07.24, MES-139
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        int R1 = 0;
        int R2 = 0;
        ExcTable.InnerHtml = "";
        ExcTable.InnerHtml += "<table width='100%'  border='1' cellspacing='0' cellpadding='0' style='font-size:12px;border-collapse:collapse'>";
        ExcTable.InnerHtml += "	  <tr>";
        ExcTable.InnerHtml += "	    <td class='tr2style' style='width=150'>Cutting.Date</td>";
        ExcTable.InnerHtml += "	    <td class='tr2style' style='width=150'>BPO.Del.Date</td>";
        ExcTable.InnerHtml += "	    <td class='tr2style'>Fabric Pattern</td>";
        ExcTable.InnerHtml += "	    <td class='tr2style'>SAM(Pcs/M)</td>";
        ExcTable.InnerHtml += "	    <td class='tr2style'>Style Desc</td>";
        ExcTable.InnerHtml += "	    <td class='tr2style'>Wash Type</td>";
        ExcTable.InnerHtml += "	    <td class='tr2style'>Print ing</td>";
        ExcTable.InnerHtml += "	    <td class='tr2style'>Emb</td>";
        ExcTable.InnerHtml += "	    <td class='tr2style'>Fus ing</td>";
        ExcTable.InnerHtml += "	    <td class='tr2style'>Buyer</td>";
        ExcTable.InnerHtml += "	    <td class='tr2style'>Gmt_Order.</td>";
        ExcTable.InnerHtml += "	    <td class='tr2style'>Job_Order_No.</td>";
        if (cbLine.Checked == true)
        {
            ExcTable.InnerHtml += "	    <td class='tr2style' >Prod.line</td>";
            R1 = 19;
            R2 = 1;
        }
        else
        {
            R1 = 18;
            R2 = 0;
        }
        ExcTable.InnerHtml += "	    <td class='tr2style'>Order Qty</td>";
        ExcTable.InnerHtml += "	    <td class='tr2style'>Cut Qty</td>";
        ExcTable.InnerHtml += "	    <td class='tr2style'>TTL Cut Qty</td>";
        ExcTable.InnerHtml += "	    <td class='tr2style'>UP To Day Qty</td>";
        ExcTable.InnerHtml += "	    <td class='tr2style'>Cut To Order(%)</td>";
        ExcTable.InnerHtml += "	    <td class='tr2style' style='width=150'>Cut Complete Date</td>";
        ExcTable.InnerHtml += "	  </tr>";

        //Modified by MunFoong on 2014.07.24, MES-139
        DataTable dt = MESComment.MesRpt.GetCuttingDateReport(txtBeginDate.Text, txtEndDate.Text, txtJoNo.Text.Trim(), ddlGarmentType.SelectedItem.Value, txtBuyerCode.Text.Trim(), ddlFactory.SelectedItem.Value, cbLine.Checked, ddGroupName.SelectedItem.Value);
        double C2O = 0.00;
        double TTCQTY = 0;

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            ExcTable.InnerHtml += "<tr>";
            for (int j = 0; j < R1; j++)
            {
                if (j == (16 + R2))
                {
                    if (dt.Rows[i][j].ToString() != "" && dt.Rows[i][j].ToString() != "NULL")
                    {
                        C2O = double.Parse(dt.Rows[i][j].ToString());
                    }
                    else
                    {
                        C2O = 0.00;
                    }
                    ExcTable.InnerHtml += "<td class='xl25' style='border-top:none;border-left:none'  align='right'>" + C2O.ToString("#0.00") + "</td>";
                }
                else
                {
                    if (j == 0 || j == 1 || j == (17 + R2))
                    {
                        if (dt.Rows[i][j].ToString() != "" && dt.Rows[i][j].ToString() != "NULL")
                        {
                            ExcTable.InnerHtml += "<td class='xl25' style='border-top:none;border-left:none;width=150'  align='right'>" + Convert.ToDateTime(dt.Rows[i][j].ToString()).ToString("d") + "</td>";
                        }
                        else
                        {
                            ExcTable.InnerHtml += "<td class='xl25' style='border-top:none;border-left:none;width=150'  align='right'>" + dt.Rows[i][j].ToString() + "</td>";
                        }

                    }
                    else
                    {
                        ExcTable.InnerHtml += "<td class='xl25' style='border-top:none;border-left:none'  align='right'>" + dt.Rows[i][j] + "</td>";
                    }
                }
            }
            if (dt.Rows[i]["JOQTY"].ToString() != "" && dt.Rows[i]["JOQTY"].ToString() != "NULL")
            {
                TTCQTY += double.Parse(dt.Rows[i]["JOQTY"].ToString());
            }

            ExcTable.InnerHtml += "</tr>";
        }
        ExcTable.InnerHtml += "	  <tr>";
        ExcTable.InnerHtml += "	    <td class='tr2style' style='width=150'>Total:</td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td>";
        if (cbLine.Checked == true)
        {
            ExcTable.InnerHtml += "<td></td>";
        }
        ExcTable.InnerHtml += "<td>" + TTCQTY + "</td><td></td><td></td><td></td><td></td></tr>";
        ExcTable.InnerHtml += "</table>";
    }
}
