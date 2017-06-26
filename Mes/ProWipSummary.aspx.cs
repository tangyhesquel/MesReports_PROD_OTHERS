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
using System.Data.Common;
public partial class Mes_ProWipSummary : pPage
{
    DataTable dtSource = new DataTable();
    DataTable dtWareHouse = new DataTable();
    DataTable dtTitle = new DataTable();
    string id = "";
    string svType = "PROD";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["id"] != null)
        {
            id = Request.QueryString["id"].ToString();
        }

        if (Request.QueryString["svType"] != null)
        {
            svType = Request.QueryString["svType"].ToString();
            HttpContext.Current.Session["svType"] = Request.QueryString["svType"].ToString();
        }

        //Random ro = new Random(1000);
        //strTableName = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
        //strOutTableName = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();

        //string strOutSql = "";
        //string strSQl = "";
        DbConnection MESConn = MESComment.DBUtility.GetConnection("MES");
        //DataTable dtJoList=MESComment.MesRpt.GetProTranWIPSummaryJoList(id.Split(',')[0], MESConn);
        //if (id.Split(',').Length > 5)
        //{
        //    if (bool.Parse(id.Split(',')[6]))
        //    {
        //        foreach (DataRow row in dtJoList.Rows)
        //        {
        //            strSQl += " INSERT INTO MU_SHIP_JO_INFO (CT_NO,Run_NO) values('" + row[0] + "','" + strTableName + "'); ";
        //        }
        //        if (strSQl != "")

        //            foreach (DataRow row in MESComment.MesRpt.GetProTranWIPSummaryJoList(strSQl, strTableName,id.Split(',')[0],id.Split(',')[7],id.Split(',')[8]).Rows)
        //            {
        //                strOutSql += " select " + row[0] + " as CT_NO into " + strOutTableName + "; ";
        //            }
        //    }
        //}
        dtTitle = MESComment.ProTranWIPSummary.GetProTranWIPTitle(id.Split(',')[2], id.Split(',')[0], id.Split(',')[1], MESConn);
        if (id.Split(',')[1] != "WareHouse")
        {
            if(Request.QueryString["ShowBuyer"] != null)
                dtSource = MESComment.ProTranWIPSummary.GenerateProTranWIPDetail(id.Split(',')[2], id.Split(',')[1],true);
            else
                dtSource = MESComment.ProTranWIPSummary.GenerateProTranWIPDetail(id.Split(',')[2], id.Split(',')[1], false);
                
        }
        else
        {
            //dtSource = MESComment.ProTranWIPSummary.GetProTranWIPJoList(id.Split(',')[2], MESConn);
            //string strJoList = "";
            //foreach (DataRow row in dtSource.Rows)
            //{
            //    strJoList += row[0].ToString() + ";";
            //}
            //dtWareHouse = MESComment.MesOutSourcePriceSql.GetShipQTYAndWarehouseQTY("","",strJoList,"","","","","","","","","","","","","",false,"");
        }
        MESComment.DBUtility.CloseConnection(ref MESConn);

        DataTable dtObject = new DataTable();
        dtObject.Columns.Add("JOB_ORDER_NO");
        if (Request.QueryString["ShowBuyer"] != null)
        {
            //Added by Zikuan MES-093 Begin
            dtObject.Columns.Add("Buyer Short Name");
            dtObject.Columns.Add("Style No");
            dtObject.Columns.Add("Line");
        }
        //End Added MES-093
        dtObject.Columns.Add("Delivery Date");//货期
        dtObject.Columns.Add("Cut Qty");//裁数
        dtObject.Columns.Add(id.Split(',')[1]);
        foreach (DataRow row in dtSource.Rows)
        {
            DataRow newRow;
            newRow = dtObject.NewRow();
            newRow["JOB_ORDER_NO"] = row["JOB_ORDER_NO"];
            //Added & Modified by Zikuan MES-093 Begin
            if (Request.QueryString["ShowBuyer"] != null)
            {
                newRow["Buyer Short Name"] = row["BUYER_SHORT_NAME"];
                newRow["Style No"] = row["STYLE_NO"];
                newRow["Line"] = row["PRODUCTION_LINE_CD"];
                newRow[4] = row["Buyer_Po_Del_date"];
                newRow[5] = row["qty"];

            }
            else
            {
                newRow[1] = row["Buyer_Po_Del_date"];
                newRow[2] = row["qty"];
            }
            //End Added & Modified MES-093
            if (id.Split(',')[1] != "WareHouse")
            {
                newRow[row["PROCESS_CD"].ToString()] = row["WIP"];
            }
            else
            {
                double jo_wareHouse = 0;
                //foreach (DataRow rWareHouse in dtWareHouse.Select("job_order_no='" + row["JOB_ORDER_NO"].ToString() + "'"))
                //{
                //    jo_wareHouse += double.Parse(rWareHouse["warehouse"].ToString() == "" ? "0" : rWareHouse["warehouse"].ToString());
                //}
                newRow[id.Split(',')[1]] = jo_wareHouse;
            }
              dtObject.Rows.Add(newRow);
        }
        gvBody.DataSource = dtObject;
        gvBody.DataBind();
    }
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        string id0 = "";
        switch (e.Row.RowType)
        {
            case DataControlRowType.DataRow:
                for (int i = 0; i < dtTitle.Rows.Count; i++)
                {
                    id0 = "WIP" + "," + id.Split(',')[0] + "," + dtTitle.Rows[i][1].ToString() + "," + e.Row.Cells[0].Text;
                    //MES-093
                    if (Request.QueryString["ShowBuyer"] != null)
                        e.Row.Cells[i + 6].Text = string.Format("<a href='#' onclick='window.open (\"ProTranWIPDetail.aspx?id={0}&svType=" + svType + "\",\"\",\"height=300, width=600, top=150, left=250, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no\")'>{1}</a>", id0, e.Row.Cells[i + 6].Text);
                    else
                        e.Row.Cells[i + 3].Text = "<a href='ProTranWIPDetail.aspx?id=" + id0 + "&svType=" + svType + "'target='blank'>" + e.Row.Cells[i + 3].Text + "</a>";
                }
                break;
            case DataControlRowType.Header:
                e.Row.Cells[0].Width = 100;
                e.Row.Cells[1].Width = 100;
                e.Row.Cells[2].Width = 50;
                break;
        }
    }
}
