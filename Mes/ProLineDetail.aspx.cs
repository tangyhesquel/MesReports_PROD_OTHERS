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
using System.Threading;
using System.Globalization;

public partial class Mes_ProLineDetail : System.Web.UI.Page
{
    public string id = "";
    int int_sah = 2;
    string strculture = "";
    string factoryCd = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["id"] != null)
        {
            id = Request.QueryString["id"].ToString();
            if (id != "")
            {
                factoryCd = id.Split(',')[1];
                if (factoryCd == "PTX" || factoryCd == "EGM" || factoryCd == "TIL" || factoryCd == "EGV" || factoryCd == "EAV")
                {
                    strculture = "en";
                }
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
                if (id.Split(',')[0] == "")//From ProProductQuery
                {
                    //Line,factoryCd,strJo,strDept,GarmentType,ProcessType,ProdFactory,strPart,startDate,endDate,type,strGo,isGoLike
                    //  0      1       2      3         4         5              6        7      8         9      10    11    12
                    //string value = id.Split(',')[6] == "" ? id : id.Replace(id.Split(',')[6], "");
                    string value = id.Split(',')[8] == "" ? id : id.Replace(id.Split(',')[8], "");
                    //gvProduction.DataSource = MESComment.ProTranWIPSummary.GetProTranWIPProductQty(id.Split(',')[1], id.Split(',')[11], bool.Parse(id.Split(',')[12]), id.Split(',')[2], id.Split(',')[3], id.Split(',')[4],id.Split(',')[5],id.Split(',')[6], id.Split(',')[7], id.Split(',')[8], id.Split(',')[9], "Line", bool.Parse(id.Split(',')[14]),true);
                    gvProduction.DataSource = MESComment.ProTranWIPSummary.GetProTranWIPProductQty(id.Split(',')[1], id.Split(',')[11], bool.Parse(id.Split(',')[12]), id.Split(',')[2], id.Split(',')[3], id.Split(',')[4], id.Split(',')[5], id.Split(',')[6], id.Split(',')[7], id.Split(',')[8], id.Split(',')[9], "Line", bool.Parse(id.Split(',')[14]), bool.Parse(id.Split(',')[15]));
                    gvProduction.DataBind();

                                          
                      
           
                    if (strculture == "en")
                    {
                        divTotal.InnerHtml = "<table width='100%'><tr><td>Production Total Out</td><td><table><tr><td><input value=" + id.Split(',')[13] + " style='width:50px'></input></td><td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Total SAH</td></tr></table></td><td><input value=" + tot_SAH.ToString("0.00") + " style='width:50px'></input></td></tr><tr><td width='33%'>Production Line</td><td width='33%'>Out QTY</td><td>SAH</td></tr></table>";
                    }
                    else
                    {
                        divTotal.InnerHtml = "<table width='100%'><tr><td>车间总体产量</td><td><table><tr><td><input value=" + id.Split(',')[13] + " style='width:50px'></input></td><td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;总SAH</td></tr></table></td><td><input value=" + tot_SAH.ToString("0.00") + " style='width:50px'></input></td></tr><tr><td width='33%'>组别</td><td width='33%'>产量</td><td>SAH</td></tr></table>";
                    }
                }
                else//From proStatusQuery
                {
                    //Line,factoryCd,strJo,strDept,GarmentType,ProcessType,ProdFactory,strPart,startDate,endDate,type,strGo,isGoLike
                    //  0      1       2      3         4         5        6        7      8    9      10
                    if (strculture == "en")
                    {
                        divTotal.InnerHtml = "<table width='100%'><tr><td width='25%'>Process</td><td width='25%'>Production Line</td><td width='25%'>Out QTY</td><td>SAH</td></tr></table>";
                    }
                    else
                    {
                        divTotal.InnerHtml = "<table width='100%'><tr><td width='25%'>工序</td><td width='25%'>组别</td><td width='25%'>产量</td><td>SAH</td></tr></table>";
                    }
                    BoundField filed=new BoundField();
                    filed.DataField = "process_cd";
                    gvProduction.Columns.Insert(0,filed);
                    gvProduction.DataSource = MESComment.ProTranWIPSummary.GetProTranWIPProductQty(id.Split(',')[1],"",false, id.Split(',')[2], "", "", "","","","", "", "Line",false,true);
                    gvProduction.DataBind();
                }
            }
        }
    }
    double tot_SAH = 0;
    protected void gvProduction_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (id.Split(',')[0] == "")
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.DataRow:
                    //Line,factoryCd,strJo,strDept,GarmentType,strPart,startDate,endDate,type,strGo,isGoLike
                    //string id0 = "Line" + "," + id.Split(',')[1] + "," + id.Split(',')[2] + "," + id.Split(',')[3] + "," + id.Split(',')[4] + "," + e.Row.Cells[0].Text + "," + id.Split(',')[6] + "," + id.Split(',')[7] + "," + id.Split(',')[8] + "," + id.Split(',')[9] + "," + id.Split(',')[10] + "," + id.Split(',')[12] + "," + id.Split(',')[12] ;
                    string id0 = "Line" + "," + id.Split(',')[1] + "," + id.Split(',')[2] + "," + id.Split(',')[3] + "," + id.Split(',')[4] + "," + e.Row.Cells[0].Text + "," + id.Split(',')[6] + "," + id.Split(',')[7] + "," + id.Split(',')[8] + "," + id.Split(',')[9] + "," + id.Split(',')[10] + "," + id.Split(',')[12] + "," + id.Split(',')[12] + "," + id.Split(',')[14] + "," + id.Split(',')[15];
                    e.Row.Cells[1].Text = string.Format("<a href='#' onclick='window.open (\"ProTranWIPDetail.aspx?id={0}\",\"\",\"height=300, width=600, top=150, left=250, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no\")'>{1}</a>", id0, e.Row.Cells[1].Text);
                    tot_SAH = tot_SAH + double.Parse(e.Row.Cells[int_sah].Text == "&nbsp;" ? "0" : e.Row.Cells[int_sah].Text);
                    break;
            }
        }
    }
}
