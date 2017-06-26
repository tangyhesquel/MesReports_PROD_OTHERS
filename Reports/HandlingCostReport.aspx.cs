using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_HandlingCostReport : pPage
{
    public string contractName = "";
    string style = "style='border-bottom: .5pt solid windowtext; border-right: .5pt solid windowtext;'";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            hfValue.Value = Request.QueryString["site"];
            txtcontractNo.Text = Request.QueryString["ContractNo"];
            if (Request.QueryString["site"].Trim().Length > 0 && txtcontractNo.Text.Length > 0)
            {
                GetData();
            }
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        GetData();
    }

    protected void GetData()
    {
        divFirst.InnerHtml = "";
        divSecond.InnerHtml = "";
        divThird.InnerHtml = "";
        DataTable List1 = table1();
        DataTable List2 = table2();
        DataTable List3 = table3();
        contractName = MESComment.MesRpt.GetOutsourceCurrency(txtcontractNo.Text.Trim()).Rows[0]["local_fty"].ToString();
        foreach (DataRow row in List1.Rows)
        {
            divFirst.InnerHtml += "<tr>";
            divFirst.InnerHtml += "<td " + style + ">" + row["NAME"] + "</td><td " + style + ">" + row["SC_NO"] + "</td><td " + style + ">" + row["STYLE_NO"] + "</td><td " + style + ">" + row["PLAN_ISSUE_QTY"] + "</td><td " + style + ">" + row["STYLE_CHN_DESC"] + "</td><td " + style + ">&nbsp;</td>";
            divFirst.InnerHtml += "<td " + style + ">" + row["SUBCONTRACTOR_NAME"] + "</td><td " + style + ">" + row["PROCESS_REMARK"] + "</td><td " + style + ">" + row["INTERNER_PRICE"] + "</td><td " + style + ">" + row["INTERNAL_EMB_PRICE"] + "</td><td " + style + ">" + row["INTERNAL_WASH_PRICE"] + "</td><td " + style + ">" + row["TTL_INNER"] + "</td>";
            divFirst.InnerHtml += "<td " + style + ">" + row["SUB_CONTRACT_PRICE"] + "</td><td " + style + ">" + row["EMB_PRICE"] + "</td><td " + style + ">" + row["WASH_PRICE"] + "</td><td " + style + ">" + row["TTL_OUTTER"] + "</td><td " + style + ">" + row["SAH"] + "</td><td " + style + ">" + row["STANDARD1"] + "</td>";
            divFirst.InnerHtml += "<td " + style + ">&nbsp;</td><td " + style + ">&nbsp;</td><td " + style + ">" + row["PATTERN_TYPE_CD1"] + "</td></tr>";
        }
        foreach (DataRow row in List2.Rows)
        {
            divSecond.InnerHtml += "<tr>";
            divSecond.InnerHtml += "<td " + style + ">" + row["NAME"] + "</td><td " + style + ">" + row["SC_NO"] + "</td><td " + style + ">" + row["PLAN_ISSUE_QTY"] + "</td><td " + style + ">" + row["EMBRIODERY_CODE1"] + "</td><td " + style + ">&nbsp;</td><td " + style + ">EMB</td><td " + style + ">" + row["STITCHES"] + "</td>";
            divSecond.InnerHtml += "<td " + style + ">" + row["CAL1"] + "</td><td " + style + ">" + row["INTERNAL_EMB_PRICE"] + "</td><td " + style + ">&nbsp;</td><td " + style + ">" + row["EMB_PRICE"] + "</td><td " + style + ">&nbsp;</td><td " + style + ">" + row["CAL2"] + "</td></tr>";
        }
        foreach (DataRow row in List3.Rows)
        {
            String fobUnitPrice = "";
            double bomUPrice = 0;
            double washPrice = 0;
            double embPrice = 0;
            double fabricPrice = 0;
            double hanlingCost = 0;
            double cmc = 0;
            double fob1 = 0;
            try
            {
                if (row["FOB_UNIT_PRICE1"] != null && !"".Equals(row["FOB_UNIT_PRICE1"].ToString()))
                {
                    fob1 = Double.Parse(row["FOB_UNIT_PRICE1"].ToString());
                    fobUnitPrice = fob1.ToString("f2");
                }
                else
                {
                    fob1 = 0;
                    fobUnitPrice = "0.00";
                }
                if (row["BOM_U_PRICE1"] != null && !"".Equals(row["BOM_U_PRICE1"].ToString()))
                {
                    bomUPrice = Double.Parse(row["BOM_U_PRICE1"].ToString());
                }
                if (row["FOB_FABRIC_PRICE"] != null && !"".Equals(row["FOB_FABRIC_PRICE"].ToString()))
                {
                    fabricPrice = Double.Parse(row["FOB_FABRIC_PRICE"].ToString());
                }
                if (row["WASH_PRICE"] != null && !"".Equals(row["WASH_PRICE"].ToString()))
                {
                    washPrice = Double.Parse(row["WASH_PRICE"].ToString());
                }
                if (row["EMB_PRICE"] != null && !"".Equals(row["EMB_PRICE"].ToString()))
                {
                    embPrice = Double.Parse(row["EMB_PRICE"].ToString());
                }
                if (row["HANDLING_COST"] != null && !"".Equals(row["HANDLING_COST"].ToString()))
                {
                    hanlingCost = Double.Parse(row["HANDLING_COST"].ToString());
                }
            }
            //catch (Exception ex)
            catch (Exception)
            {
                Response.Write("Price converter Exception");
            }
            cmc = fob1 - bomUPrice - washPrice - embPrice - hanlingCost - fabricPrice;
            divThird.InnerHtml += "<tr><td " + style + "><a href='DetailPrice.aspx?scNo=" + row["SC_NO"] + "' target='_blank'><font color='blue'>" + row["SC_NO"] + "</font></a></td>";
            divThird.InnerHtml += "<td " + style + ">" + fobUnitPrice + "</td><td " + style + ">" + fobUnitPrice + "</td><td " + style + ">" + row["BOM_U_PRICE1"] + "</td><td " + style + ">" + row["FOB_FABRIC_PRICE"] + "</td>";
            divThird.InnerHtml += "<td " + style + ">" + row["WASH_PRICE"] + "</td><td " + style + ">" + row["EMB_PRICE"] + "</td><td " + style + ">" + row["HANDLING_COST"] + "</td><td " + style + ">" + cmc.ToString("f2") + "</td></tr>";
        }
    }

    private DataTable table1()
    {
        DataTable dt = MESComment.MesRpt.GetHandlingCostReport1(txtGo.Text.Trim(), txtcontractNo.Text.Trim());
        dt.Columns.Add("STANDARD1");
        dt.Columns.Add("PATTERN_TYPE_CD1");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            String pattern = "";
            DataTable dt1 = MESComment.MesRpt.GetPattern(dt.Rows[i]["SC_NO"].ToString());
            for (int j = 0; j < dt1.Rows.Count; j++)
            {
                pattern = dt1.Rows[j][0].ToString();
            }
            String standard = "";
            String gmtType = "" + dt.Rows[i]["GARMENT_TYPE_CD"].ToString();
            if ("K".Equals(gmtType))
            {
                if ("solid".Equals(pattern))
                {
                    standard = (double.Parse(dt.Rows[i]["SAH"].ToString()) * 3.16 + 0.9).ToString("f2");
                }
                else
                {
                    standard = (double.Parse(dt.Rows[i]["SAH"].ToString()) * 3.16 + 1.3).ToString("f2");
                }
            }
            else
            {
                if ("solid".Equals(pattern))
                {
                    standard = (double.Parse(dt.Rows[i]["SAH"].ToString()) * 2.55).ToString("f2");
                }
                else
                {
                    standard = (double.Parse(dt.Rows[i]["SAH"].ToString()) * 2.65).ToString("f2");
                }
            }
            dt.Rows[i]["STANDARD1"] = standard;
            dt.Rows[i]["PATTERN_TYPE_CD1"] = pattern;
        }
        return dt;
    }

    private DataTable table2()
    {
        DataTable dt = MESComment.MesRpt.GetHandlingCostReport2(txtGo.Text.Trim(), txtcontractNo.Text.Trim());
        dt.Columns.Add("EMBRIODERY_CODE1");
        dt.Columns.Add("FOB_UNIT_PRICE1");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            String emb = "";
            DataTable dt1 = MESComment.MesRpt.GetEmb(dt.Rows[i]["SC_NO"].ToString());
            for (int j = 0; j < dt1.Rows.Count; j++)
            {
                emb = dt1.Rows[j][0].ToString();
            }
            dt.Rows[i]["EMBRIODERY_CODE1"] = emb;
            DataTable dt2 = MESComment.MesRpt.GetFOB(dt.Rows[i]["SC_NO"].ToString());
            for (int j = 0; j < dt2.Rows.Count; j++)
            {
                dt.Rows[i]["FOB_UNIT_PRICE1"] = dt2.Rows[j]["FOB_UNIT_PRICE"];
            }
        }
        return dt;
    }

    private DataTable table3()
    {
        DataTable dt = MESComment.MesRpt.GetHandlingCostReport3(txtGo.Text.Trim(), txtcontractNo.Text.Trim());
        dt.Columns.Add("FOB_UNIT_PRICE1");
        dt.Columns.Add("BOM_U_PRICE1");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            DataTable dt1 = MESComment.MesRpt.GetFOB(dt.Rows[i]["SC_NO"].ToString());
            for (int j = 0; j < dt1.Rows.Count; j++)
            {
                dt.Rows[i]["FOB_UNIT_PRICE1"] = dt1.Rows[j]["FOB_UNIT_PRICE"];
            }
            DataTable dt2 = MESComment.MesRpt.GetAcceserie(dt.Rows[i]["SC_NO"].ToString());
            for (int j = 0; j < dt2.Rows.Count; j++)
            {
                dt.Rows[i]["BOM_U_PRICE1"] = double.Parse(dt2.Rows[j]["BOM_U_PRICE"].ToString() == "" ? "0" : dt2.Rows[j]["BOM_U_PRICE"].ToString()).ToString("f2");
            }
        }
        return dt;
    }
}
