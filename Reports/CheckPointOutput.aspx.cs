using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Globalization;

public partial class Reports_wipDaily : pPage
{
    public string factoryCd = "";
    string strculture = "";
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {

            ddlFtyCd.DataSource = MESComment.MesRpt.GetFactoryCd();
            ddlFtyCd.DataTextField = "FACTORY_ID";
            ddlFtyCd.DataValueField = "FACTORY_ID";
            ddlFtyCd.DataBind();
            lstMultipleValues.Attributes.Add("onclick", "FindSelectedItems(this," + txtSelectedMLValues.ClientID + ");");
            if (strculture == "en")
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("zh-CN");

            }
        }
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                ddlFtyCd.SelectedValue = Request.QueryString["site"].ToString();
                factoryCd = Request.QueryString["site"].ToString();
            }
            else
            {
                ddlFtyCd.SelectedValue = "GEG";
                factoryCd = "GEG";
            }
            factoryCd = Request.QueryString["site"].ToString();
            if (factoryCd == "PTX" || factoryCd == "EGM" || factoryCd == "TIL" || factoryCd == "EGV" || factoryCd == "EAV")
            {
                strculture = "en";
            }
            else
            {
                strculture = "zh";
            }
            SetMultiValue();
        }
    }
    protected void SetMultiValue()
    {
        DataTable dt = new DataTable("Table1");
        DataColumn dc1 = new DataColumn("Text");
        DataColumn dc2 = new DataColumn("Value");
        dt.Columns.Add(dc1);
        dt.Columns.Add(dc2);


        //To get enough data for scroll
        DataTable CPLIST = MESComment.CheckPointOutPut.GetCPProcessList(ddlFtyCd.SelectedValue, ddlgarmentType.SelectedItem.Value);
        for (int i = 0; i < CPLIST.Rows.Count; i++)
        {
            dt.Rows.Add(CPLIST.Rows[i]["dpt_cd"].ToString(), i + 1);
        }


        lstMultipleValues.DataSource = dt;
        lstMultipleValues.DataTextField = "Text";
        lstMultipleValues.DataValueField = "Value";
        lstMultipleValues.DataBind();
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        string strContent = "";
        string strTitle = "";
        string SqlOrder = "";
        string strSummary = "";
        string StrLine = "";
        if (strculture == "en")
        {
            StrLine = "Line";
        }
        else
        {
            StrLine = "组别";
        }
        Random ro = new Random(1000);
        DbConnection MESConn = MESComment.DBUtility.GetConnection("MES");
        strContent = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
        strTitle = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
        strSummary = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
        MESComment.CheckPointOutPut.SP_Pro_DropTmpTable(strContent, strTitle);
        DataTable TTitle = MESComment.CheckPointOutPut.GetCPOutputResultList(ddlFtyCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value, "", txtStartDate.Text, txtToDate.Text, txtSelectedMLValues.Value, "", strContent, strTitle, strSummary, MESConn);

        divBody.InnerHtml = "";

        string OldJo = "";
         double[] ttlqtya = new double[TTitle.Rows.Count];
        double[] ttlqtyt = new double[TTitle.Rows.Count];
        Boolean MitiProcess = false;

        for (int i = 0; i < TTitle.Rows.Count; i++)
        {
            divBody.InnerHtml += "<td >" + TTitle.Rows[i]["FNAME"] + "</td>";
        }

        divBody.InnerHtml += "</tr>";
        if (txtSelectedMLValues.Value.ToString().IndexOf(",") == -1)
        {
            SqlOrder = " order by 17,5";
            MitiProcess = false;
        }
        else
        {
            SqlOrder = " order by JONO";
            MitiProcess = true;
        }
        DataTable Detail = MESComment.CheckPointOutPut.GetCPOutputSummary(strContent, strTitle, SqlOrder);

        string color = "";
        string ProdLine = "";
        int rc = 0;
        for (int i = 0; i < Detail.Rows.Count; i++)
        {
            if (i % 2 == 0)
            {
                color = "white";
            }
            else
            {
                color = "#f2f2f2";
            }



            if (i == 0)
            {
                //    OldJo = Detail.Rows[i]["JONO"].ToString();
                ProdLine = Detail.Rows[i][16].ToString();
            }


            if (!MitiProcess)
            {
                //ProdLine = Detail.Rows[i][16].ToString();

                if (ProdLine != Detail.Rows[i][16].ToString())
                {
                    divBody.InnerHtml += "<tr  bgcolor=#efefe7>";
                    divBody.InnerHtml += "<td  colspan='13' align='right'><b>" + ProdLine + "   Total:</b></td>";
                    for (int j = 1; j < TTitle.Rows.Count; j++)
                    {
                        divBody.InnerHtml += "<td width='200' align='right'>" + ttlqtya[j].ToString() + "</td>";
                        ttlqtya[j] = 0;

                    }
                    divBody.InnerHtml += "</tr>";
                }

            }
            divBody.InnerHtml += "<tr  bgcolor=" + color + ">";

            rc = int.Parse(Detail.Rows[i]["JOCC"].ToString());
            if (MitiProcess && OldJo != Detail.Rows[i]["JONO"].ToString() && rc > 1)
            {

                divBody.InnerHtml += "<td rowspan='" + rc + "'> " + Detail.Rows[i]["GONO"].ToString() + "</td>";
                divBody.InnerHtml += "<td rowspan='" + rc + "'>" + Detail.Rows[i]["JONO"].ToString() + "</td>";
                divBody.InnerHtml += "<td rowspan='" + rc + "'>" + Detail.Rows[i]["BUYER"].ToString() + "</td>";
                divBody.InnerHtml += "<td rowspan='" + rc + "'>" + Detail.Rows[i]["STYLENO"].ToString() + "</td>";
                divBody.InnerHtml += "<td align='center' rowspan='" + rc + "'>" + Detail.Rows[i]["GARMENTTYPE"].ToString() + "</td>";
                divBody.InnerHtml += "<td align='center' rowspan='" + rc + "'>" + Detail.Rows[i]["WASHTYPE"].ToString() + "</td>";
                divBody.InnerHtml += "<td rowspan='" + rc + "'>" + Detail.Rows[i]["SideSeamMethod"].ToString() + "</td>";
                divBody.InnerHtml += "<td rowspan='" + rc + "'>" + Detail.Rows[i]["FABTYPE"].ToString() + "</td>";
                divBody.InnerHtml += "<td width='200' align='left' rowspan='" + rc + "'>" + Convert.ToDateTime(Detail.Rows[i]["BPOD"].ToString()).ToString("d") + "</td>";
                divBody.InnerHtml += "<td rowspan='" + rc + "'>" + Detail.Rows[i]["ORDERQTY"].ToString() + "</td>";
                divBody.InnerHtml += "<td rowspan='" + rc + "'>" + Detail.Rows[i]["CUTQTY"].ToString() + "</td>";
                divBody.InnerHtml += "<td rowspan='" + rc + "'>" + Detail.Rows[i]["SAH"].ToString() + "</td>";

            }
            else
            {
                if (!MitiProcess || rc == 1)
                {
                    divBody.InnerHtml += "<td > " + Detail.Rows[i]["GONO"].ToString() + "</td>";
                    divBody.InnerHtml += "<td >" + Detail.Rows[i]["JONO"].ToString() + "</td>";
                    divBody.InnerHtml += "<td >" + Detail.Rows[i]["BUYER"].ToString() + "</td>";
                    divBody.InnerHtml += "<td >" + Detail.Rows[i]["STYLENO"].ToString() + "</td>";
                    divBody.InnerHtml += "<td align='center' >" + Detail.Rows[i]["GARMENTTYPE"].ToString() + "</td>";
                    divBody.InnerHtml += "<td align='center' >" + Detail.Rows[i]["WASHTYPE"].ToString() + "</td>";
                    divBody.InnerHtml += "<td >" + Detail.Rows[i]["SideSeamMethod"].ToString() + "</td>";
                    divBody.InnerHtml += "<td >" + Detail.Rows[i]["FABTYPE"].ToString() + "</td>";
                    divBody.InnerHtml += "<td width='300' align='left' >" + Convert.ToDateTime(Detail.Rows[i]["BPOD"].ToString()).ToString("d") + "</td>";
                    divBody.InnerHtml += "<td >" + Detail.Rows[i]["ORDERQTY"].ToString() + "</td>";
                    divBody.InnerHtml += "<td >" + Detail.Rows[i]["CUTQTY"].ToString() + "</td>";
                    divBody.InnerHtml += "<td >" + Detail.Rows[i]["SAH"].ToString() + "</td>";
                }
            }


            for (int j = 0; j < TTitle.Rows.Count; j++)
            {
                if (j > 0 && TTitle.Rows[j]["FNAME"].ToString().IndexOf(StrLine) == -1)
                {

                    if (double.Parse(Detail.Rows[i][16 + j].ToString()) == 0)
                    {
                        divBody.InnerHtml += "<td width='200' align='right'>&nbsp;</td>";
                    }
                    else
                    {
                        divBody.InnerHtml += "<td width='200' align='right'>" + Detail.Rows[i][16 + j].ToString() + "</td>";
                    }
                    ttlqtyt[j] += double.Parse(Detail.Rows[i][16 + j].ToString());

                    if (!MitiProcess)
                    {

                        ttlqtya[j] += double.Parse(Detail.Rows[i][16 + j].ToString());
                    }
                }
                else
                {
                    divBody.InnerHtml += "<td width='200' align='right'>" + Detail.Rows[i][16 + j].ToString() + "</td>";
                    ttlqtyt[j] += 0;
                }
            }
            divBody.InnerHtml += "</tr>";

            OldJo = Detail.Rows[i]["JONO"].ToString();
            ProdLine = Detail.Rows[i][16].ToString();
        }
        if (!MitiProcess)
        {

            divBody.InnerHtml += "<tr  bgcolor=#efefe7>";
            divBody.InnerHtml += "<td  colspan='13' align='right'><b>" + ProdLine + "   Total:</b></td>";
            for (int j = 1; j < TTitle.Rows.Count; j++)
            {
                divBody.InnerHtml += "<td width='200' align='right'>" + ttlqtya[j].ToString() + "</td>";
                ttlqtya[j] = 0;

            }
            divBody.InnerHtml += "</tr>";


        }

        divBody.InnerHtml += "<tr  bgcolor=#efefe7>";
        divBody.InnerHtml += "<td  colspan='13' align='right'><b>Process Total:</b></td>";
        for (int j = 1; j < TTitle.Rows.Count; j++)
        {
            divBody.InnerHtml += "<td width='200' align='right'>" + ttlqtyt[j].ToString() + "</td>";

        }


        rc = 0;
        int rc2 = 0;
        int rc3 = 0;
        int ic = 0;
        int ri = 0;
        int ri2 = 0;
        if (MitiProcess)
        {
            divBody.InnerHtml += "</tr>";
            divBody.InnerHtml += "</table>";
            DataTable Summary = MESComment.CheckPointOutPut.GetCPOutputLineSummary(strSummary, strTitle, SqlOrder);
            ic = txtSelectedMLValues.Value.ToString().Length - txtSelectedMLValues.Value.ToString().Replace(",", String.Empty).Length + 1;
            if (strculture == "en")
            {
                divBody.InnerHtml += "<tr><td colspan=3><br/><strong>Summary by Production Line</strong></td></tr>";
            }
            else
            {
                divBody.InnerHtml += "<tr><td colspan=3><br/><strong>部门组别汇总</strong></td></tr>";
            }
            for (int ip = 0; ip < ic; ip++)
            {

                divBody.InnerHtml += "<tr><td><table border='1' cellspacing='1' cellpadding='1' style='font-size:12px;border-collapse:collapse'>";
                divBody.InnerHtml += "<tr class='tr2style' align='center'>";
                rc3 = rc2;
                for (int s = rc3; s < TTitle.Rows.Count; s++)
                {
                    if (TTitle.Rows[s]["FNAME"].ToString().IndexOf(StrLine) != -1 && s != rc3)
                    {
                        break;
                    }
                    divBody.InnerHtml += "<td >" + TTitle.Rows[s]["FNAME"] + "</td>";
                    rc2 = rc2 + 1;
                }
                divBody.InnerHtml += "</tr>";

                for (int i = ri2; i < Summary.Rows.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        color = "white";
                    }
                    else
                    {
                        color = "#f2f2f2";
                    }

                    for (int s = rc3; s < rc2; s++)
                    {

                        if (Summary.Rows[i][rc3 + 1].ToString() == "0")
                        {
                            ri = ri - 1;
                            rc = s;
                            break;
                        }
                        divBody.InnerHtml += "<td width='200' align='right'>" + Summary.Rows[i][1 + s].ToString() + "</td>";

                    }
                    divBody.InnerHtml += "</tr>";

                    rc = 0;
                    ri = ri + 1;
                }

                divBody.InnerHtml += "<tr  bgcolor=#efefe7>";
                divBody.InnerHtml += "<td  align='right'><b>Process Total:</b></td>";
                for (int s = rc3; s < rc2 - 1; s++)
                {
                    divBody.InnerHtml += "<td width='200' align='right'>" + ttlqtyt[s + 1].ToString() + "</td>";

                }
                divBody.InnerHtml += "</tr>";
                divBody.InnerHtml += "</table>";
                divBody.InnerHtml += "</td></tr><tr><td><br/>&nbsp;</td></tr>";
                rc3 = rc2 - 1;
                ri2 = ri;
            }

        }


    }




    protected void ddlFtyCd_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetMultiValue();
    }
    protected void ddlGMT_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetMultiValue();
    }

}
