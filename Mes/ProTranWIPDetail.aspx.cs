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

public partial class Mes_ProTranWIPDetail : pPage
{
    string id = "";
    DataTable dt = new DataTable();
    string svType = "PROD";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["id"] != null)
        {
            id = Request.QueryString["id"].ToString();

            if (Request.QueryString["svType"] != null)
            {
                HttpContext.Current.Session["svType"] = Request.QueryString["svType"].ToString();
            }

            if (id.Split(',')[0] == "WIP")//WIP Detail
            {
                gvWip.Visible = true;
                gvProduction.Visible = false;
                //MES-093
                DataTable dt = new DataTable();
                if (Request.QueryString["line"] != null)
                {
                    if(string.IsNullOrEmpty(Request.QueryString["line"].ToString()))
                    {
                        dt = MESComment.ProTranWIPSummary.GetProTranWIPJoColorAndSizeNew(id.Split(',')[1], id.Split(',')[2], id.Split(',')[3], "All");
                    }
                    else
                    {
                        dt = MESComment.ProTranWIPSummary.GetProTranWIPJoColorAndSizeNew(id.Split(',')[1], id.Split(',')[2], id.Split(',')[3], Request.QueryString["line"].ToString());
                    }
                }
                else
                {
                    dt = MESComment.ProTranWIPSummary.GetProTranWIPJoColorAndSizeNew(id.Split(',')[1], id.Split(',')[2], id.Split(',')[3], "");
                }
                //MES-093 END
                gvWip.DataSource = dt;

                if (Request.QueryString["line"] != null)
                {
                    gvWip.Columns[3].Visible = true;
                    gvWip.Columns[4].Visible = true;
                    gvWip.Columns[5].Visible = true;
                }
                else
                {
                    gvWip.Columns[3].Visible = false;
                    gvWip.Columns[4].Visible = false;
                    gvWip.Columns[5].Visible = false;
                }

                gvWip.DataBind();
            }
            else//Product Qty
            {
                //Line,factoryCd,strJo,strDept,garmentType,ProcesType,ProdFactory,strPart,startDate,endDate,type,strGo,isGoLike,Total,isOSJo
                //  0      1       2      3        4          5            6         7       8         9     10    11    12       13    14      
                if (!MESComment.ProTranWIPSummary.GetProTranWIPIsContainProcessLine(id.Split(',')[1], id.Split(',')[3], id.Split(',')[4]) || id.Split(',')[0] == "Line" || (MESComment.ProTranWIPSummary.GetProTranWIPIsContainProcessLine(id.Split(',')[1], id.Split(',')[3], id.Split(',')[4]) && id.Split(',')[3] != "" && id.Split(',')[0] != "")) //Added By ZouShiChang ON 2013.08.27 MES 024 增加,id.Split(',')[4] 参数
                {
                    gvWip.Visible = false;
                    gvProduction.Visible = true;
                    string Factory_CD = id.Split(',')[1];
                    string GONO = id.Split(',')[11];
                    bool isGOLike = bool.Parse(id.Split(',')[12]);
                    string JONO = id.Split(',')[2];
                    string Process_CD = id.Split(',')[3];
                    string GarmentType = id.Split(',')[4];
                    //Added By ZouShiChang ON 2013.09.24 Start MES024
                    string ProcessType = id.Split(',')[5];
                    string ProdFactory = id.Split(',')[6];
                    //Added By ZouShiChang ON 2013.09.24 End MES024
                    string Part = id.Split(',')[7];
                    string startDate = id.Split(',')[8];
                    string endDate = id.Split(',')[9];
                    string Type = id.Split(',')[10];
                    //bool isOSJo = bool.Parse(id.Split(',')[14]);
                    bool isOSJo;
                    if (id.Split(',')[0].ToString() != "Line")
                    {
                        isOSJo = bool.Parse(id.Split(',')[14]);
                    }
                    else
                    {
                        isOSJo = bool.Parse(id.Split(',')[12]);
                        Part = ProcessType;
                        ProcessType = "";
                        GONO = "";
                    }
                    //Added By ZouShiChang ON 2013.11.28 End
                    if (Process_CD.Equals("ShippedQty"))
                    {
                        dt = MESComment.MesOutSourcePriceSql.GetShipQTYAndWarehouseQTY(startDate, endDate, JONO, "", "", GONO, GarmentType, "", Factory_CD,"","","","","","", "SummaryByJo", isOSJo,"Produce").DefaultView.ToTable(true, new string[] { "job_order_no", "ship_qty" });
                        //<Added by:ZouShCh at;2013-04-01>过滤ship_qty为０的数据
                        DataTable newdt = new DataTable();
                        newdt = dt.Clone();
                        DataRow[] rows = dt.Select("SHIP_QTY>0");
                        foreach (DataRow row in rows)
                        {
                            newdt.Rows.Add(row.ItemArray);
                        }
                        dt.Clear();
                        dt = newdt;
                        //<Added by:ZouShCh at;2013-04-01>过滤ship_qty为０的数据
                    }
                    else
                    {
                        //dt = MESComment.ProTranWIPSummary.GetProTranWIPProductQty(Factory_CD, GONO, isGOLike, JONO, Process_CD, GarmentType, Part, startDate, endDate, Type, isOSJo,true);
                        //dt = MESComment.ProTranWIPSummary.GetProTranWIPProductQty(Factory_CD, GONO, isGOLike, JONO, Process_CD, GarmentType, ProcessType, ProdFactory, Part, startDate, endDate, Type, isOSJo, true);
                        dt = MESComment.ProTranWIPSummary.GetProTranWIPProductQty(Factory_CD, GONO, isGOLike, JONO, Process_CD, GarmentType, ProcessType, ProdFactory, Part, startDate, endDate, Type, isOSJo, bool.Parse(id.Split(',')[14]));
                    }
                  

                    gvProduction.DataSource = dt;
                    gvProduction.DataBind();
                }
                else
                {
                    Response.Redirect("ProLineDetail.aspx?id="+id+"");
                }
            }
        }

    }
}
