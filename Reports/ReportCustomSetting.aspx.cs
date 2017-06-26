using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;



public partial class Reports_ReportCustomSetting : pPage
{

    public string strusername;
    public string Report_Name = "";

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
                if (Request.QueryString["ReportName"] != null)
                {
                    ReportName.Text = Request.QueryString["ReportName"].ToString();
                    Report_Name = Request.QueryString["ReportName"].ToString();
                }
                if (Request.QueryString.Get("User_ID") != null)
                {
                    strusername = Request.QueryString.Get("User_ID").ToString();
                    SetMultiValue(strusername);
                }


            }
        }

    }


    protected void SetMultiValue(string user_id)
    {
        DataTable CPLISTUser = MESComment.MesRpt.GetCustomReportList(ddlFactory.SelectedValue, "REPORT", "REPORT", ReportName.Text.Trim(), user_id);
        DataTable CPLISTDefault = MESComment.MesRpt.GetCustomReportList(ddlFactory.SelectedValue, "REPORT", "REPORT", ReportName.Text.Trim(), "DEFAULT");

        string checks = "";

        if (CPLISTDefault.Rows.Count <= 0)
        {
            return;
        }
        else
        {
            string[] hStrD = CPLISTDefault.Rows[0]["CUSTOM_VALUES"].ToString().Split(new char[] { ',' });
            string[] hStrU = null;
            if (CPLISTUser.Rows.Count > 0)
            {
                hStrU = CPLISTUser.Rows[0]["CUSTOM_VALUES"].ToString().Split(new char[] { ',' });
            }

            ExcTable.InnerHtml = "";
            ExcTable.InnerHtml += "	<table>";
            checks = "false";
            for (int i = 0; i < hStrD.Count(); i++)
            {
                checks = "false";
                if (hStrU != null)
                {
                    for (int j = 0; j < hStrU.Count(); j++)
                    {
                        if (hStrD[i].ToString() == hStrU[j].ToString())
                        {
                            checks = "checked";
                            break;
                        }
                    }
                }
                if (checks == "checked")
                {
                    //ExcTable.InnerHtml += "<tr><td><input type=\"checkbox\" value=\"" + hStrD[i].ToString() + "\" checked=\"checked\" onclick=\"unselectall()\" name=\"cln\">" + hStrD[i].ToString() + "</td></tr>";
                    if (hStrD[i].ToString().Contains("}|"))
                    {
                        ExcTable.InnerHtml += "<tr class='tr2style'><td><input type=\"hidden\" value=\"" + hStrD[i].ToString() + "\" name=\"cln\">" + hStrD[i].ToString() + "</td></tr>";
                    }
                    else
                    {
                        ExcTable.InnerHtml += "<tr><td><input type=\"checkbox\" value=\"" + hStrD[i].ToString() + "\" checked=\"checked\" name=\"cln\">" + hStrD[i].ToString() + "</td></tr>";
                    }
                }
                else
                {
                    //ExcTable.InnerHtml += "<tr><td><input type=\"checkbox\" value=\"" + hStrD[i].ToString() + "\"  onclick=\"unselectall()\" name=\"cln\">" + hStrD[i].ToString() + "</td></tr>";
                    ExcTable.InnerHtml += "<tr><td><input type=\"checkbox\" value=\"" + hStrD[i].ToString() + "\"  name=\"cln\">" + hStrD[i].ToString() + "</td></tr>";

                }
            }

            ExcTable.InnerHtml += "</table>";

        }

    }

}
