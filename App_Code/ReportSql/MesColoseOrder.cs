using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;
using System.Data;
using System.Data.Common;
using System.Web.UI.WebControls;


/// <summary>
/// Summary description for Class1
/// </summary>
public class MesColoseOrder
{
	public static DataTable getcloseorderdata(string Fty,string Jo)
	{        
		string sql = "";
        sql = "exec [PROC_CLOSE_ORDER] '" + Fty + "','" + Jo  + "'";
        DbConnection MESConn = MESComment.DBUtility.GetConnection("MES");
        DataTable dt= MESComment.DBUtility.GetTable(sql, MESConn);                 
        return dt;
	}

    public static DataTable getfgisdata(string Fty,string Jo)
    {
        string sql=""; 
        sql=sql+"  SELECT  SUM(gmt_qty_a) AS gmt_qty_a,SUM(gmt_qty_b) AS gmt_qty_b ";
        sql=sql+"  FROM (";
        sql=sql+"  SELECT l.reference_no AS job_order_no,(case when l.grade='A' then sum(nvl(l.qty,0)) else 0 end) as gmt_qty_a, ";
        sql=sql+"  (case when l.grade='B' then sum(nvl(l.qty,0)) else 0 end) as gmt_qty_b";
        sql=sql+"  FROM inv_trans_hd h, inv_trans_lines l, inventory.inv_store_codes s, ";
        sql=sql+"  inv_stock_class c ";
        sql=sql+"  WHERE h.trans_header_id = l.trans_header_id ";
        sql=sql+"  AND h.from_store_cd = s.store_cd ";
        sql=sql+"  AND s.stock_class_cd = c.stock_class_cd ";
        sql=sql+"  AND NVL(h.first_receipt_flag, 'N') = 'Y' ";
        sql=sql+"  AND c.stock_group_cd = 'L' ";
        sql=sql+"  AND l.grade IN ('A', 'B') ";
        //sql=sql+"  AND exists (select f1 from rpt_tmp where F1=l.reference_no )";
        sql = sql + "  AND l.reference_no='"+Jo+"'";
        sql=sql+"  and h.doc_no like '"+Fty+"%' ";
        sql=sql+"  group by l.reference_no,l.grade ) A";

        DbConnection FGISConn = MESComment.DBUtility.GetConnection("INV");
        DataTable dt = MESComment.DBUtility.GetTable(sql, FGISConn);
        return dt;
    }

    public static DataTable geteasndata(string Fty, string Jo)
    {
        string sql = "";
        sql = sql + "  select (";
        sql = sql + "  isnull((SELECT sum(i.pacqty) FROM asninbox b, asninitem i, StyleMaster s";
        sql = sql + "  Where b.buyid = i.buyid And b.inboxno = i.inboxno";
        sql = sql + "  AND i.BuyID = s.BuyID AND i.UPC = s.UPC";
        sql = sql + "  and upper(b.gono)='" + Jo + "'),0) +";
        sql = sql + "  isnull((SELECT sum(CASE WHEN h.trans_type = 'R' THEN l.Qty WHEN h.trans_type = 'I' THEN -1 * l.Qty END)";
        sql = sql + "  FROM auto_receipt_hd h with (nolock), auto_receipt_lines l with (nolock)";
        sql = sql + "  WHERE h.hd_id = l.hd_id and h.factory_cd='" + Fty + "'";
        sql = sql + "  and upper(l.ReFerence_No)='" + Jo + "'),0) ) as pack_qty";
        DbConnection easnConn = MESComment.DBUtility.GetConnection("EASN");
        DataTable dt = MESComment.DBUtility.GetTable(sql, easnConn);
        return dt;
    }
}
