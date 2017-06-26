using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_proPullout : pPage
{
    string userid = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        userid = Request.QueryString["userid"];
        if (!IsPostBack)
        {
            ddlFactoryCd.DataSource = MESComment.MesRpt.GetFactoryList(userid);
            ddlFactoryCd.DataBind();
            DateTime beginTime = DateTime.Now.AddYears(-10);
            //for (int i = 0; i < 20; i++)
            //{
            //    ddlYear.Items.Add(beginTime.AddYears(i + 1).Year.ToString());
            //}
            //ddlYear.SelectedValue = DateTime.Now.Year.ToString();
            if (Request.QueryString["site"] != "")
            {
                if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
                {
                    ddlFactoryCd.SelectedValue = Request.QueryString["site"].ToString();
                }
                else
                {
                    ddlFactoryCd.SelectedValue = "GEG";
                }
                ddlProcessCode.DataSource = MESComment.MesRpt.GetProcessCode(ddlFactoryCd.SelectedItem.Value, ddlGarmentType.SelectedItem.Value);
                ddlProcessCode.DataBind();

                ddlProcessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
                ddlProcessType.DataTextField = "PROCESS_TYPE_ID";
                ddlProcessType.DataValueField = "PROCESS_TYPE_VALUE";
                ddlProcessType.DataBind();

                ddlProdFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
                ddlProdFactory.DataTextField = "FACTORY_ID";
                ddlProdFactory.DataValueField = "FACTORY_ID";
                ddlProdFactory.DataBind();

                //if (Request.QueryString["site"].ToString().ToUpper().Equals("YMG"))
                //{
                    if (Request.QueryString["show"].ToString().ToUpper().Equals("DISC"))
                    {
                        this.title.InnerHtml = "<h2>Production Discrepancy Monthly Report</h2>";
                    }
                    if (Request.QueryString["show"].ToString().ToUpper().Equals("PULLOUT"))
                    {
                        this.title.InnerHtml = "<h2>Production Pullout Monthly Report</h2>";
                    }
                //}
                if (Request.QueryString["site"].ToString().ToUpper().Equals("YMG"))
                {
                    this.ddGroupName.Enabled = true;
                    MESComment.CommFunction.LoadGroupDropDownList(base.CurrentSite, this.ddGroupName);
                }
            }
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        //if (Request.QueryString["site"].ToString().ToUpper().Equals("YMG") && Request.QueryString["show"].ToString().ToUpper().Equals("DISC"))
        if (Request.QueryString["show"].ToString().ToUpper().Equals("DISC"))
        {
            SetDiscrepancyData();
        }
        else
        {
            SetData();
        }
    }

    private bool CheckQueryCondition()
    {
        string StartDate = txtStartDate.Text.Trim();
        string EndDate = txtEndDate.Text.Trim();
        string JoNo = txtJoNo.Text.Trim();
        if (StartDate.Length == 0 && EndDate.Length == 0 && JoNo.Length == 0)
        {
            this.lblMessage.Text = "Please input Date info or Jo No.!";
            this.lblMessage.Visible = true;
            return false;
        }
        else if ((StartDate.Length > 0 && EndDate.Length == 0) || (StartDate.Length == 0 && EndDate.Length > 0))
        {
            this.lblMessage.Text = "Please input complete Date info !";
            this.lblMessage.Visible = true;
            return false;
        }
        else if (StartDate.Length > 0 && EndDate.Length > 0)
        {
            //比较startdate 与 enddate
            int SDate = Convert.ToInt32(StartDate.Replace("-", ""));
            int EDate = Convert.ToInt32(EndDate.Replace("-", ""));
            if (EDate < SDate)
            {
                //提示enddate必须要大于或等于startdate
                this.lblMessage.Text = "Date From cannot after Date To ! ";
                this.lblMessage.Visible = true;
                return false;
            }
        }
        return true;
    }

    protected void SetData()
    {
        Boolean Show_PullOut = true;
        Boolean Show_Disc = true;
        this.divMsg.InnerHtml = "";
        //if (Request.QueryString["site"].ToString().Equals("YMG"))
        //{
            if (Request.QueryString["show"].ToString().ToUpper().Equals("PULLOUT"))
            {
                Show_PullOut = true;
                Show_Disc = false;
            }
            if (Request.QueryString["show"].ToString().ToUpper().Equals("DISC"))
            {
                Show_Disc = true;
                Show_PullOut = false;
            }
        //}
        if (true == this.CheckQueryCondition())
        {
            this.lblMessage.Text = "";
            this.lblMessage.Visible = false;
            divDetail.InnerHtml = "";            
            DataTable dtDetail = MESComment.proPulloutSql.GetProPulOutList(ddlGarmentType.SelectedItem.Value, ddlFactoryCd.SelectedItem.Value, txtStartDate.Text, txtEndDate.Text, txtJoNo.Text.Trim(), ddlProcessCode.SelectedItem.Value,ddlProcessType.SelectedItem.Value,ddlProdFactory.SelectedItem.Value, this.ddGroupName.SelectedValue);
            if (dtDetail.Rows.Count > 0)
            {
                divDetail.InnerHtml += "<table id='allTable' width='100%' border='1' cellspacing='0' cellPadding='0' style='font-size: 12px;border-collapse: collapse'>";
                divDetail.InnerHtml += " <tr>";
                divDetail.InnerHtml += " <td rowspan='2' class='tr2style' bgColor='#efefe7' width='15%'>Process Code</td>";
                divDetail.InnerHtml += " <td rowspan='2' class='tr2style' bgColor='#efefe7'>Garment Type</td>"; 
                divDetail.InnerHtml += " <td rowspan='2' class='tr2style' bgColor='#efefe7'>Tran Date</td>";
                divDetail.InnerHtml += " <td rowspan='2' class='tr2style' bgColor='#efefe7'>Job Order NO</td>";
                divDetail.InnerHtml += " <td rowspan='2' class='tr2style' bgColor='#efefe7'>Line Code</td>";
                divDetail.InnerHtml += " <td rowspan='2' class='tr2style' bgColor='#efefe7'>Line Name</td>";
                divDetail.InnerHtml += " <td rowspan='2' class='tr2style' bgColor='#efefe7'>Color</td>";
                divDetail.InnerHtml += " <td rowspan='2' class='tr2style' bgColor='#efefe7'>Size</td>";
               
                //if (Show_Disc)
                //{
                //    divDetail.InnerHtml += " <td class='tr2style' bgColor='#efefe7'>Discrepancy Reason</td>";
                //    divDetail.InnerHtml += " <td class='tr2style' bgColor='#efefe7'>Discrepancy QTY</td>";
                //}
                //if (Show_PullOut)
                //{
                    divDetail.InnerHtml += " <td rowspan='2' class='tr2style' bgColor='#efefe7'>Pull Out Reason</td>";
                    divDetail.InnerHtml += " <td colspan='6' align=center class='tr2style' bgColor='#efefe7'>Pull In</td>";
                    divDetail.InnerHtml += " <td colspan='6' align=center class='tr2style' bgColor='#efefe7'>Pull Out</td></tr>";
                    divDetail.InnerHtml += " <tr> <td class='tr2style' bgColor='#efefe7'>Pullout To (JO)</td>";
                    divDetail.InnerHtml += " <td class='tr2style' bgColor='#efefe7'>To Color</td>";
                    divDetail.InnerHtml += " <td class='tr2style' bgColor='#efefe7'>To Size</td>";
                    divDetail.InnerHtml += " <td class='tr2style' bgColor='#efefe7'>To Line CD</td>";
                    divDetail.InnerHtml += " <td class='tr2style' bgColor='#efefe7'>To Line Name</td>";
                    divDetail.InnerHtml += " <td class='tr2style' bgColor='#efefe7'>Pullout Qty</td>";                    
                    divDetail.InnerHtml += " <td class='tr2style' bgColor='#efefe7'>Pullin From (JO)</td>";
                    divDetail.InnerHtml += " <td class='tr2style' bgColor='#efefe7'>Pullin Qty</td>";
                    divDetail.InnerHtml += " <td class='tr2style' bgColor='#efefe7'>From Color</td>";
                    divDetail.InnerHtml += " <td class='tr2style' bgColor='#efefe7'>From Size</td>";
                    divDetail.InnerHtml += " <td class='tr2style' bgColor='#efefe7'>From Line</td>";
                    divDetail.InnerHtml += " <td class='tr2style' bgColor='#efefe7'>From Line Name</td>";
                //}



                divDetail.InnerHtml += " </tr>";

                DataRow[] dr = null;
                //if (Request.QueryString["site"].ToString().Equals("YMG"))
                //{
                //    if (Show_Disc)
                //    {
                //        dr = dtDetail.Select(" DISCREPANCY_QTY <>0 ", "PROCESS_CD DESC,TRX_DATE ASC");
                //    }
                //    if (Show_PullOut)
                //    {
                dr = dtDetail.Select(" PULLOUT_QTY <>0 OR PULLIN_QTY<>0", "PROCESS_CD DESC,TRX_DATE ASC");
                //    }
                //}
                //else
                //{
                //    dr = dtDetail.Select("1=1", "JOB_ORDER_NO ");
                //}
                                
                foreach (DataRow row in dr)
                {
                    divDetail.InnerHtml += "<tr>";
                    divDetail.InnerHtml += "<td>" + row["PROCESS_CD"] + "</td>";
                    divDetail.InnerHtml += "<td>" + row["GARMENT_TYPE"] + "</td>"; 
                    divDetail.InnerHtml += "<td>" + row["TRX_DATE"] + "</td>";
                    divDetail.InnerHtml += "<td>" + row["JOB_ORDER_NO"] + "</td>";
                    divDetail.InnerHtml += "<td>" + row["PRODUCTION_LINE_CD"] + "</td>";
                    divDetail.InnerHtml += "<td>" + row["PRODUCTION_LINE_NAME"] + "</td>";
                    divDetail.InnerHtml += "<td>" + row["COLOR_CODE"] + "</td>";
                    divDetail.InnerHtml += "<td>" + row["SIZE_CODE"] + "</td>";
                    if (Show_Disc)
                    {
                        divDetail.InnerHtml += "<td>" + row["REASON_DESC"] + "</td>";
                        divDetail.InnerHtml += "<td>" + row["DISCREPANCY_QTY"] + "</td>";
                    }
                    if (Show_PullOut)
                    {
                        divDetail.InnerHtml += "<td>" + row["REASON_DESC"] + "</td>";
                        divDetail.InnerHtml += "<td>" + row["PULL_OUT_JO"] + "</td>";
                        divDetail.InnerHtml += "<td>" + row["PULL_OUT_COLOR_CODE"] + "</td>";
                        divDetail.InnerHtml += "<td>" + row["PULL_OUT_SIZE_CODE"] + "</td>";
                        divDetail.InnerHtml += "<td>" + row["PULL_OUT_LINE_CD"] + "</td>";
                        divDetail.InnerHtml += "<td>" + row["PULL_OUT_LINE_NAME"] + "</td>";
                        divDetail.InnerHtml += "<td>" + row["PULLOUT_QTY"] + "</td>";
                        divDetail.InnerHtml += "<td>" + row["PULLIN_JO"] + "</td>";
                        divDetail.InnerHtml += "<td>" + row["PULLIN_QTY"] + "</td>";
                        divDetail.InnerHtml += "<td>" + row["PULLIN_COLOR_CODE"] + "</td>";
                        divDetail.InnerHtml += "<td>" + row["PULLIN_SIZE_CODE"] + "</td>";
                        divDetail.InnerHtml += "<td>" + row["PULLIN_LINE_CD"] + "</td>";
                        divDetail.InnerHtml += "<td>" + row["PULLIN_LINE_NAME"] + "</td>";
                    }



                    divDetail.InnerHtml += "</tr>";
                }
                divDetail.InnerHtml += "</table>";
            }
            else
            {                
                this.divMsg.InnerHtml += "<table width='100%'  style='color:Red; font-size:large'><tr><td align='center'><b>No Data</b></td></tr></table>";
            }
        }
        else
        {
            divDetail.InnerHtml = "<table id='allTable' width='100%' border='1' cellspacing='0' cellPadding='0' style='font-size: 12px;border-collapse: collapse'>";
            divDetail.InnerHtml += "<tr>";
            divDetail.InnerHtml += "<td colspan=9></td>";
            divDetail.InnerHtml += "</tr></table>";
        }
    }

    protected void ddlGarmentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlProcessCode.DataSource = MESComment.MesRpt.GetProcessCode(ddlFactoryCd.SelectedItem.Value, ddlGarmentType.SelectedItem.Value);
        ddlProcessCode.DataBind();
    }

    //Added By ZouShiChang ON 2013.10.08 Start
    protected void SetDiscrepancyData()
    {
        DataTable dt = MESComment.proPulloutSql.GetYMGProDiscList(ddlGarmentType.SelectedItem.Value, ddlFactoryCd.SelectedItem.Value, txtStartDate.Text, txtEndDate.Text, txtJoNo.Text.Trim(), ddlProcessCode.SelectedItem.Value, ddlProcessType.SelectedItem.Value, ddlProdFactory.SelectedItem.Value, this.ddGroupName.SelectedValue);
        gvDiscData.DataSource = dt;
        gvDiscData.DataBind();
        gvDiscData.Visible = true;
    }
    //Added By ZouShiChang ON 2013.10.08 End
}
