using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;

public partial class Reports_CutStickersNoReport : pPage
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
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {

        string strTitle = "";
        int layNoFrom, layNoTo;
        int lay = 0;
        //int Flay=int.Parse(txtlayNoFrom.Text.Trim().ToString());
        double swidth = 0;
        Random ro = new Random(1000);
        DbConnection MESConn = MESComment.DBUtility.GetConnection("MES");

        if (string.IsNullOrEmpty(txtlayNoFrom.Text.Trim().ToString()))
        {
            Response.Write(" <script language=\"javascript\" type=\"text/javascript\">alert('Please input the (From)Lay No.')</script>");
            return;
        }


        ExcTable.InnerHtml = "";
        int.TryParse(txtlayNoFrom.Text.Trim().ToString(), out layNoFrom);
        int.TryParse(txtlayNoTo.Text.Trim().ToString(), out layNoTo);
        if (layNoTo == 0)
            layNoTo = layNoFrom;
        for (int l = layNoFrom; l <= layNoTo; l++)
        {
            strTitle = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
            //strTitle = "##SONGYUTEST01";
            DataTable dt = MESComment.MesRpt.GetCuttingStickers(txtJoNo.Text.Trim(), l.ToString(), txtlayNoTo.Text.Trim(), ddlFactory.SelectedItem.Value, strTitle, MESConn);

            string color = "";

            swidth = 220 + (dt.Columns.Count - 2) * 70;
            if (lay != 0)
            {
                ExcTable.InnerHtml += "<br><br><br><table width=" + swidth + "  border='0' cellspacing='0' cellpadding='0'  style='font-size:12px;border-collapse:collapse;page-break-before: always'>";
            }
            else
            {
                ExcTable.InnerHtml += "<table width=" + swidth + "   border='0' cellspacing='1' cellpadding='0'  style='font-size:12px;border-collapse:collapse'>";
            }
            DataTable hd = MESComment.MesRpt.GetCuttingStickershead(txtJoNo.Text.Trim(), l.ToString(), txtlayNoTo.Text.Trim(), ddlFactory.SelectedItem.Value, strTitle);

            ExcTable.InnerHtml += "	<tr><td style='text-align: center'><font size='4' ><strong><b>Job Stickers Numbering Report</b></strong></font><br></td> </tr><tr><td style='text-align: left'>Print Date:&nbsp;" + DateTime.Now.ToString() + "&nbsp;&nbsp;&nbsp;</td></table>";
            ExcTable.InnerHtml += "	  <table width=600 border='1' cellspacing='0' cellpadding='0'  style='font-size:12px;border-collapse:collapse'><tr><td width='120' class='tr2style' style='text-align: left'> Job Order:</td><td width='80' style='text-align: left'>" + txtJoNo.Text.Trim() + "</td><td width='60' class='tr2style'  style='text-align: left'>Lay No</td><td width='120' style='text-align: left'>" + l.ToString() + " </td></tr>";
            ExcTable.InnerHtml += "	  <tr><td  class='tr2style' style='text-align: left'> Procution Line:</td><td width='80' style='text-align: left'>" + hd.Rows[0]["PRODUCTION_LINE_CD"].ToString() + "</td><td class='tr2style'>Dlvy Date</td><td style='text-align: left'>" + Convert.ToDateTime(hd.Rows[0]["Buyer_PO_DEl_Date"].ToString()).ToString("d") + " </td></tr>";
            ExcTable.InnerHtml += "	  <tr><td  class='tr2style'  style='text-align: left'> Table/Invoice No</td><td width='80' style='text-align: left'></td><td class='tr2style' >Pattern No</td><td  style='text-align: left'>" + hd.Rows[0]["pattern_no"].ToString() + "</td></tr>";
            ExcTable.InnerHtml += "	  <tr><td  class='tr2style' height='49' style='text-align: left'> Remarks</td><td  colspan=3  style='text-align: left'></td></tr>";
            ExcTable.InnerHtml += "	</table><br><table  width=" + swidth + "   border='1' cellspacing='0' cellpadding='0'  style='font-size:12px;border-collapse:collapse'> ";
            ExcTable.InnerHtml += "	  <tr>";

            ExcTable.InnerHtml += "	    <td class='tr2style' width='40' style='text-align: center'>Lay</td>";
            ExcTable.InnerHtml += "	    <td class='tr2style' width='120' style='text-align: center'>Color</td>";
            ExcTable.InnerHtml += "	    <td class='tr2style' width='60' style='text-align: right'>Plys Size</td>";

            for (int j = 0; j < dt.Columns.Count; j++)
            {
                if (j > 2)
                {
                    ExcTable.InnerHtml += "	    <td class='tr2style' width='70' style='text-align: center'>" + dt.Columns[j].ColumnName + "</td>";
                }
            }
            ExcTable.InnerHtml += "	  </tr>";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i % 2 == 0)
                {
                    color = "white";
                }
                else
                {
                    color = "#f2f2f2";
                }

                ExcTable.InnerHtml += "	  <tr bgcolor=" + color + ">";
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    if (i == 0 && k == 0)
                    {
                        ExcTable.InnerHtml += "	    <td  rowspan=" + dt.Rows.Count + " class='tr3style' style='text-align: center'>" + dt.Rows[i][k] + "</td>";
                    }
                    else
                    {
                        if (k != 0)
                        {
                            ExcTable.InnerHtml += "	    <td  class='tr3style' style='text-align: center'>" + dt.Rows[i][k] + "</td>";
                        }
                    }
                }

                ExcTable.InnerHtml += "</tr>";
            }

            ExcTable.InnerHtml += "</table>";

            lay = lay + 1;
        }
        //ExcTable.InnerHtml += "</tr></table>";
    }
}
