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

public partial class Mes_ProAcceptQtyDetail : pPage
{
    string factoryCd = "";
    string processCd = "";
    string strJo = "";
    string strType = "";
    string DateType = "";
    string garmentType = "";
    string startDate = "";
    string endDate = "";
    bool IsOSJO = false;
    protected void Page_Load(object sender, EventArgs e)
    {
        DataTable dt = new DataTable();
        if (Request.QueryString["id"] != null)
        {
            factoryCd = Request.QueryString["id"].Split(';')[0];
            strJo = Request.QueryString["id"].Split(';')[1];
            if (Request.QueryString["id"].Split(';')[2].Split('>').Length > 1)
            {
                processCd = Request.QueryString["id"].Split(';')[2].Split('>')[0];
                strType = Request.QueryString["id"].Split(';')[2].Split('>')[1];
            }
            else
            {
                processCd = Request.QueryString["id"].Split(';')[2];
                strType = Request.QueryString["id"].Split(';')[2];
            }
            garmentType = Request.QueryString["id"].Split(';')[3];
            DateType = Request.QueryString["id"].Split(';')[4];
            startDate = Request.QueryString["id"].Split(';')[5];
            endDate = Request.QueryString["id"].Split(';')[6];
            IsOSJO = bool.Parse(Request.QueryString["id"].Split(';')[7]);

        }
        if (DateType == "BPO")
        {
            switch (strType)
            {
                case "Discrepancy":
                    dt = MESComment.ProTranWIPSummary.GetProDiscrepancyList(garmentType, factoryCd, DateType, startDate, endDate, strJo, processCd);
                    break;
                case "Pullout/in(-/ )":
                    dt = MESComment.ProTranWIPSummary.GetProPullinOutList(garmentType, factoryCd, DateType, startDate, endDate, strJo);
                    break;
                default:
                    if (processCd != "Warehouse")
                    {
                        dt = MESComment.ProTranWIPSummary.GetProTranWIPAcceptQtyDetail(factoryCd, DateType, startDate, endDate, strJo, processCd);
                    }
                    else
                    {
                        dt = MESComment.MesOutSourcePriceSql.GetShipQTYAndWarehouseQTY("", "", strJo, startDate, endDate, "", garmentType, "", factoryCd,"","","","","","", "SummaryByBpoDate", IsOSJO,"Accept");
                    }
                    break;
            }
        }
        else
        {
            if (processCd != "Warehouse")
            {
                dt = MESComment.ProTranWIPSummary.GetProTranWIPAcceptQtyTrxDetail(factoryCd, garmentType, processCd, startDate, endDate, IsOSJO);
            }
            else
            {
                dt = MESComment.MesOutSourcePriceSql.GetShipQTYAndWarehouseQTY(startDate, endDate, "", "", "", "", garmentType,"", factoryCd,"","","","","","", "SummaryByJo", IsOSJO,"Accept").DefaultView.ToTable(true, new string[] { "job_order_no", "warehouse" });
                //<Added by:ZouShCh at;2013-04-01>过滤ship_qty为０的数据
                DataTable newdt = new DataTable();
                newdt = dt.Clone();
                DataRow[] rows = dt.Select("WareHouse>0");
                foreach (DataRow row in rows)
                {
                    newdt.Rows.Add(row.ItemArray);
                }
                dt.Clear();
                dt = newdt;
                //<Added by:ZouShCh at;2013-04-01>过滤ship_qty为０的数据
            }
        }   
        gvBody.DataSource = dt;       
        gvBody.DataBind();
    }
}
