using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Mes_showProcessDetail : pPage
{
    string strculture = "";
    DataTable dtProcessDetail;
    protected void Page_Load(object sender, EventArgs e)
    {
        //id  factoryCd,strJo,strColor, strSize(hd),ColumnName
        if (Request.QueryString["id"] != null)
        {
            string id = Request.QueryString["id"].ToString();
            string factoryCd = "", strJo = "",strColor="", strSize = "", strProcess = "",strNextProcess="";
            //Added By ZouShiChang ON 2013.10.24 Start MES 024
            string strProcessGarmentType = "", strProcessType = "", strProcessProductionFactory = "";
            string strNextProcessGarmentType = "", strNextProcessType = "", strNextProcessProductionFactory = "";
            //Added By ZouShiChang ON 2013.10.24 End MES 024
            if (id.Split('>').Length > 4)
            {
                factoryCd = id.Split('>')[0].ToString();
                strJo = id.Split('>')[1].ToString();
                strColor = id.Split('>')[2].ToString();
                strSize = id.Split('>')[3].ToString();
                //Added By ZouShiChang ON 2013.10.24 Start MES 024
                strProcess = id.Split('>')[4].ToString();
                strNextProcess = id.Split('>')[5].ToString();
                strProcessGarmentType = id.Split('>')[4].ToString().Substring(0, 1);
                strProcess = id.Split('>')[4].ToString().Substring(1, id.Split('>')[4].ToString().IndexOf("(") - 1);
                strProcessType = id.Split('>')[4].ToString().Substring(id.Split('>')[4].ToString().IndexOf("(") + 1, 1);
                if (strProcessType == "I")
                {
                    strProcessProductionFactory = factoryCd;
                }
                else
                {
                    strProcessProductionFactory = id.Split('>')[4].ToString().Substring(id.Split('>')[4].ToString().IndexOf(")") + 1, id.Split('>')[4].ToString().Length - id.Split('>')[4].ToString().IndexOf(")") - 1);
                }
                if (id.Split('>')[5].ToString().Equals("Discrepancy") || id.Split('>')[5].ToString().Equals("Pullout") || id.Split('>')[5].ToString().Equals("PullIn"))
                {
                    strNextProcessGarmentType = "";
                    strNextProcess = "";
                    strNextProcessType = "";
                    strNextProcessProductionFactory = "";
                    dtProcessDetail = MESComment.ProTranWIPSummary.GetProTranWIPSummaryProcessDiscPullInPullOutDetail(factoryCd, strJo, strColor, strSize, strProcess, strProcessGarmentType, strProcessType, strProcessProductionFactory, id.Split('>')[5].ToString());
                }
                else
                {
                    strNextProcessGarmentType = id.Split('>')[5].ToString().Substring(0, 1);
                    strNextProcess = id.Split('>')[5].ToString().Substring(1, id.Split('>')[5].ToString().IndexOf("(") - 1);
                    strNextProcessType = id.Split('>')[5].ToString().Substring(id.Split('>')[5].ToString().IndexOf("(") + 1, 1);
                    if (strNextProcessType == "I")
                    {
                        strNextProcessProductionFactory = factoryCd;
                    }
                    else
                    {
                        strNextProcessProductionFactory = id.Split('>')[5].ToString().Substring(id.Split('>')[5].ToString().IndexOf(")") + 1, id.Split('>')[5].ToString().Length - id.Split('>')[5].ToString().IndexOf(")") - 1);
                    }

                    dtProcessDetail = MESComment.ProTranWIPSummary.GetProTranWIPSummaryProcessDetail(factoryCd, strJo, strColor, strSize, strProcess, strProcessGarmentType, strProcessType, strProcessProductionFactory, strNextProcess, strNextProcessGarmentType, strNextProcessType, strNextProcessProductionFactory);
                }

                //Added By ZouShiChang ON 2013.10.24 End MES 024
                if (factoryCd == "PTX" || factoryCd == "EGM" || factoryCd == "TIL" || factoryCd == "EGV" || factoryCd == "EAV")
                {
                    strculture = "en";
                }

                //DataTable dtProcessDetail = MESComment.ProTranWIPSummary.GetProTranWIPSummaryProcessDetail(factoryCd, strJo, strColor, strSize, strProcess, strNextProcess);
                
                
                
                divDetail.InnerHtml = "<table width='100%' border='1' cellpadding='0' cellspacing='0' style='border-collapse:collapse'><tr><td colspan='2' align='center'><h3>Transfer Process Detail</h3></td></tr>";
                if (strculture == "en")
                {
                    divDetail.InnerHtml += "<tr><td>Transaction Date</td><td>Transfer QTY</td></tr>";
                }
                else
                {
                    divDetail.InnerHtml += "<tr><td>日期</td><td>流转数</td></tr>";
                }
                foreach (DataRow row in dtProcessDetail.Rows)
                {
                    divDetail.InnerHtml += "<tr><td>" + row["DATE"] + "</td><td>" + row["QTY"] + "</td></tr>";
                }
                divDetail.InnerHtml += "</table>";
            }
        }
    }
}
