using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_jobOrderDetail : pPage
{
    string userid = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["userid"] != null)
        {
            userid = Request.QueryString["userid"].ToString();
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
        if (!IsPostBack)
        {
            ddlfactoryCd.DataSource = MESComment.MesRpt.GetFactoryList(userid);
            ddlfactoryCd.DataBind();

            ddlProdFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
            ddlProdFactory.DataTextField = "FACTORY_ID";
            ddlProdFactory.DataValueField = "FACTORY_ID";
            ddlProdFactory.DataBind();

            ddlProcessCd.DataSource = MESComment.jobOrderDetailSql.GetProcessCode(ddlfactoryCd.SelectedItem.Value, DropDownListGarmentType.SelectedItem.Value);
            ddlProcessCd.DataTextField = "PRC_CD";
            ddlProcessCd.DataValueField = "PRC_CD";
            ddlProcessCd.DataBind();

            ddlProcessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
            ddlProcessType.DataTextField = "PROCESS_TYPE_ID";
            ddlProcessType.DataValueField = "PROCESS_TYPE_VALUE";
            ddlProcessType.DataBind();
        }

    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divProcess.InnerHtml = "";
        divBody.InnerHtml = "";
        this.lblMessage.Visible = false;
        if (txtjobOrderNo.Text != "" || txtBeginDate.Text != "" || txtEndDate.Text != "")
        {
            string garmentTypeString = "";
            DataTable GT = MESComment.jobOrderDetailSql.GetJobOrderGarmentType(txtjobOrderNo.Text.Trim());
            if (GT.Rows.Count > 0)
            {
                garmentTypeString = GT.Rows[0][0].ToString();
                DropDownListGarmentType.SelectedValue = garmentTypeString;
            }

            DataTable dt = MESComment.jobOrderDetailSql.GetJobOrderDetailProcessHeadList(ddlfactoryCd.SelectedItem.Value, DropDownListGarmentType.SelectedValue, ddlProcessCd.SelectedItem.Value);
            int nProcess = dt.Rows.Count;
            if (nProcess <= 0)
            {
                this.lblMessage.Text = "Not Found any transaction data!";
                this.lblMessage.Visible = true;
                return;
            }
            int[] totalQty = new int[nProcess + 1];
            foreach (DataRow row in dt.Rows)
            {
                divProcess.InnerHtml += "<td class='tr2style' bgcolor='#efefe7'>" + row["PRC_CD"] + "</td>";
            }
            DataTable dt1 = MESComment.jobOrderDetailSql.GetJobOrderDetailResultList(txtjobOrderNo.Text.Trim(), ddlfactoryCd.SelectedItem.Value, DropDownListGarmentType.SelectedValue, ddlProcessCd.SelectedItem.Value, ddlProcessType.SelectedItem.Value, ddlProdFactory.SelectedItem.Value, cbConfirm.Checked, cbDraft.Checked, txtBeginDate.Text, txtEndDate.Text);
            foreach (DataRow row in dt1.Rows)
            {
                divBody.InnerHtml += "<TR><td>" + row["DOC_NO"] + "</td><td width='300px'>" + row["TRX_DATE"] + "</td><td width='300px'>" + row["CREATE_DATE"] + "</td>";
                divBody.InnerHtml += "<td>" + row["CREATE_USER_ID"] + "</td><td>" + row["SUBMIT_DATE"] + "</td><td>" + row["SUBMIT_USER_ID"] + "</td><td align='center' >" + row["PROCESS_TYPE"] + "</td>";

                int seq = Convert.ToInt32(row["SEQ"].ToString());
                int qty = Convert.ToInt32(row["QTY"].ToString());
                int n = 0;
                foreach (DataRow row1 in dt.Rows)
                {
                    n = n + 1;
                    if (row1["PRC_CD"].ToString().Equals(row["PROCESS_CD"].ToString()))
                    {
                        break;
                    }
                }
                int j = n;
                while (n > 1)
                {
                    divBody.InnerHtml += "<td></td>";
                    n--;
                }
                totalQty[j] = totalQty[j] + qty;
                divBody.InnerHtml += "<td>" + row["QTY"] + "</td>";
                for (int i = j; i < nProcess; i++)
                {
                    divBody.InnerHtml += "<td></td>";
                }
                divBody.InnerHtml += "<td>" + row["CONFIRM"] + "</td></tr>";
            }
            divBody.InnerHtml += "<TR class='tr2style' bgcolor='#efefe7'><TD colspan=5>TTL</TD><TD></TD><TD></TD>";
            for (int i = 1; i <= nProcess; i++)
            {
                divBody.InnerHtml += "<TD>" + totalQty[i] + "</TD>";
            }
            divBody.InnerHtml += "<TD></TD></TR> ";
        }
    }

    //Added by Jin Song (Bug Fix) 10/23/2014
    protected void DropDownListGarmentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlProcessCd.DataSource = MESComment.jobOrderDetailSql.GetProcessCode(ddlfactoryCd.SelectedItem.Value, DropDownListGarmentType.SelectedItem.Value);
        ddlProcessCd.DataTextField = "PRC_CD";
        ddlProcessCd.DataValueField = "PRC_CD";
        ddlProcessCd.DataBind();
    }
    //End of Add
}
