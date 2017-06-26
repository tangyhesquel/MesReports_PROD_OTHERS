using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web.UI.WebControls;
using MESComment;

/// <summary>
///MUReportSql 的摘要说明

///Created by haiqiang 2012-05-15;
///For MU Report :
/// </summary>
namespace MESComment
{
    public class MUReportSql
    {
        //======================================================================
        //================FactoryMUReport========================================
        public static DataTable GetExistsMuList(string factoryCd, string fromDate, string toDate, string RunNO)
        {
            string SQL = "";
            SQL += " select top 1 RunNo from MU_Report_All_Data where FromDate='" + fromDate + "' and toDate='" + toDate + "' and fty_cd='" + factoryCd + "' and LOCK_FLAG='Y'; ";
            return DBUtility.GetTable(SQL, "MES_UPDATE");
        }

        public static DataTable GetJoList(string JoList, string factoryCd, string GARMENT_TYPE_CD, string OUTSOURCE_TYPE, string fromDate, string toDate, bool shipJo, bool NoPost, string RunNO)
        {


            StringBuilder sql = new StringBuilder();
            sql.Append(@" SELECT    JOB_ORDER_NO ,
                          ISNULL(CREATED_COMBINE_JO_FLAG, 'N') AS CombineFlag
                          INTO      #tmpjo
                          FROM      PRD_CUTTING_COMPLETION a ( NOLOCK )
                                    INNER JOIN JO_HD j ( NOLOCK ) ON a.JOB_ORDER_NO = j.JO_NO
                                    inner join sc_hd h (NOLOCK) ON j.SC_NO=h.SC_NO
                          WHERE 
                                     a.COMPLETE_STATUS = 'Y'");
            if (factoryCd != "")
            {
                sql.AppendFormat(" AND a.FACTORY_CD='{0}'",factoryCd);
            }
            if (fromDate != "")
            {
                 sql.AppendFormat(" AND a.COMPLETION_DATE >='{0}' ",fromDate);
            }
            if (toDate != "")
            {
                sql.AppendFormat(" AND a.COMPLETION_DATE <DATEADD(DAY,1,'{0}') ",toDate);
            }
            if (JoList != "")
            {
                sql.AppendFormat(" and A.JOB_ORDER_NO in ({0}) ",JoList);
            }
            if (GARMENT_TYPE_CD != "")
            {
                sql.AppendFormat(" AND J.GARMENT_TYPE_CD='{0}'",GARMENT_TYPE_CD);
            }

            if (OUTSOURCE_TYPE == "STANDARD")
            {
                sql.AppendFormat(" AND H.SAM_GROUP_CD<>'OUTSOURCE' ");
            }
            if (OUTSOURCE_TYPE == "OUTSOURCE")
            {
               sql.AppendFormat(" AND H.SAM_GROUP_CD='OUTSOURCE' ");
            }
            sql.Append(@"DELETE    FROM #tmpjo
                          WHERE     CombineFlag = 'Y'
                                
                          SELECT    JOB_ORDER_NO AS JO_NO ,
                                    ISNULL(ORIGINAL_JO_NO, JOB_ORDER_NO) AS ORIGINAL_JO_NO ,
                                    ISNULL(ORIGINAL_JO_NO, '') AS ORIGINALJO
                          FROM      #tmpjo a
                                    LEFT JOIN JO_COMBINE_MAPPING b ( NOLOCK ) ON a.JOB_ORDER_NO = b.COMBINE_JO_NO");
            
            
            //string SQL = "delete MU_Report_All_Data where CREATE_DATE<dateadd(day,-2,getdate()) and LOCK_FLAG='N'; ";
            //SQL += "delete MU_Report_All_Data where RunNo='" + RunNO + "'; ";
            //SQL += "insert into MU_Report_All_Data(FromDate,ToDate,FTY_CD,JO_NO,SC_NO,BPO_Date,ORDER_QTY,OVER_SHIP, ";
            //SQL += "buyer,Wash_Type,STYLE_DESC,PATTERN_TYPE,LOCK_FLAG,CREATE_DATE,RunNo) ";
            //SQL += "SELECT FromDate='" + fromDate + "',ToDate='" + toDate + "',FTY_CD='" + factoryCd + "', ";
            //SQL += "upper(a.JOB_ORDER_NO) as JOB_ORDER_NO,J.SC_NO,s.buyer_po_del_date,s.total_qty,s.PERCENT_OVER_ALLOWED, ";
            //SQL += "c.SHORT_NAME,j.wash_type_cd,(case when a.factory_cd in('GEG','YMG','NBO','CEK','CEG') THEN h.Style_CHN_DESC ELSE  h.STYLE_DESC END) as style_desc,PATTERN_TYPE_CD=isnull(j.fab_pattern,'Solid'), ";
            //SQL += "'N' as LOCK_FLAG,Create_date=getdate(),RunNo='" + RunNO + "' ";
            //SQL += "FROM dbo.PRD_CUTTING_COMPLETION a ";
            //SQL += "inner join jo_hd J  ";
            //SQL += "on J.jo_no=a.JOB_ORDER_NO ";
            //SQL += "inner join sc_lot S ";
            //SQL += "on S.sc_no=j.sc_no ";
            //SQL += "and s.lot_no=j.lot_no ";
            //SQL += "inner join GEN_CUSTOMER C ";
            //SQL += "on C.CUSTOMER_CD=j.CUSTOMER_CD ";
            //SQL += "inner join sc_hd h ";
            //SQL += "on h.sc_no=j.sc_no ";
            //SQL += "WHERE 1=1 ";
            //if (factoryCd != "")
            //{
            //    SQL = SQL + " AND a.FACTORY_CD='" + factoryCd + "'";
            //}
            //if (fromDate != "")
            //{
            //    SQL += " AND a.COMPLETION_DATE >='" + fromDate + "' ";
            //}
            //if (toDate != "")
            //{
            //    SQL += " AND a.COMPLETION_DATE <DATEADD(DAY,1,'" + toDate + "') ";
            //}
            //if (JoList != "")
            //{
            //    SQL += " and A.JOB_ORDER_NO in (" + JoList + ") ";
            //}
            //if (GARMENT_TYPE_CD != "")
            //{
            //    SQL = SQL + " AND J.GARMENT_TYPE_CD='" + GARMENT_TYPE_CD + "'";
            //}

            //if (OUTSOURCE_TYPE == "STANDARD")
            //{
            //    SQL = SQL + " AND H.SAM_GROUP_CD<>'OUTSOURCE' ";
            //}
            //if (OUTSOURCE_TYPE == "OUTSOURCE")
            //{
            //    SQL = SQL + " AND H.SAM_GROUP_CD='OUTSOURCE' ";
            //}
            //SQL += "AND a.COMPLETE_STATUS='Y'; ";
            //SQL += " select JO_NO FROM MU_Report_All_Data WHERE RunNo='" + RunNO + "'; ";
            //return DBUtility.GetTable(SQL, "MES_UPDATE");
            return DBUtility.GetTable(sql.ToString(), "MES_UPDATE");
        }

        public static DataTable GetLocalGisInfo(string JoList, string factoryCd, string GARMENT_TYPE_CD, string OUTSOURCE_TYPE, string fromDate, string toDate, bool shipJo, bool NoPost, string RunNO)
        {
            string SQL = "set session tx_isolation='READ-UNCOMMITTED';";
            //SQL = SQL + " UPDATE MU_SHIP_JO_INFO as M, ";
            SQL = SQL + " SELECT M.SC_NO,M.CT_NO AS JO_NO,LJ.SHIP_DATE,LJ.SHIP_QTY,M.LASTJO FROM ";
            SQL = SQL + " MU_SHIP_JO_INFO as M,";
            SQL += " (SELECT C.CT_NO AS JO_NO,SUM(C.QUANTITY) AS SHIP_QTY,MAX(A.SHIP_DATE) AS SHIP_DATE ";
            SQL = SQL + " FROM FTY_INVOICE_HD A,FTY_INVOICE_DT C,MU_SHIP_JO_INFO P ";
            SQL = SQL + " WHERE  C.INVOICE_ID=A.INVOICE_ID  ";
            if (factoryCd != "")
            {
                SQL = SQL + " AND A.FACTORY_CD='" + factoryCd + "'";
            }
            SQL += " and P.Run_No='" + RunNO + "' and P.CT_NO=C.CT_NO";
            SQL = SQL + " GROUP BY C.CT_NO ";
            SQL = SQL + " UNION ALL";
            SQL = SQL + " SELECT C.CT_NO AS JO_NO,SUM(C.QUANTITY) AS SHIP_QTY,MAX(A.SHIP_DATE) AS SHIP_DATE";
            SQL = SQL + " FROM CUST_INVOICE_HD A,CUST_INVOICE_DT C,MU_SHIP_JO_INFO P";
            SQL = SQL + " WHERE  C.INVOICE_ID=A.INVOICE_ID ";
            if (factoryCd != "")
            {
                SQL = SQL + " AND A.FACTORY_CD='" + factoryCd + "'";
            }
            SQL += " and P.Run_No='" + RunNO + "' and P.CT_NO=C.CT_NO";
            SQL = SQL + " GROUP BY C.CT_NO) LJ ";
            //SQL = SQL + " SET M.SHIP_QTY=LJ.SHIP_QTY ,M.SHIP_DATE=LJ.SHIP_DATE ";
            SQL = SQL + " WHERE LJ.JO_NO=M.CT_NO ";
            SQL = SQL + " AND M.Run_No='" + RunNO + "'";
            //SQL = SQL + " ;SELECT SC_NO,CT_NO AS JO_NO,SHIP_DATE,SHIP_QTY,LASTJO FROM MU_SHIP_JO_INFO WHERE Run_No='" + RunNO + "' and ship_qty>0 ";
            SQL += " and LJ.ship_qty>0 ";
            if (!shipJo)
            {
                //SQL = SQL + " and exists (select JOB_ORDER_NO from JO_SHIP_STATUS C where SHIP_STATUS='Y' AND FACTORY_CD='" + factoryCd + "' AND C.JOB_ORDER_NO=MU_SHIP_JO_INFO.CT_NO)";
                SQL = SQL + " and exists (select JOB_ORDER_NO from JO_SHIP_STATUS C where SHIP_STATUS='Y' AND FACTORY_CD='" + factoryCd + "' AND C.JOB_ORDER_NO=M.CT_NO)";
            }
            return DBUtility.GetTable(SQL, "GIS");
        }

        public static DataTable GetStandardLeftGMAndSRNAndRTW(string factoryCd, DbConnection invConn)
        {//弃用,用GetStandardLeftGM 和 GetStandardLeftSRNAndRTW两个方法代替;
            string SQL = " SELECT jo_no,SUM(RTW_QTY_I) AS RTW_QTY_I,SUM(RTW_QTY) AS RTW_QTY,SUM(SRN_QTY_I) AS SRN_QTY_I,";
            SQL = SQL + " SUM(SRN_QTY) AS SRN_QTY,SUM(gmt_qty_a) AS gmt_qty_a,SUM(gmt_qty_b) AS gmt_qty_b,SUM(sew_qty_b) AS sew_qty_b,SUM(wash_qty_b) AS wash_qty_b ";
            SQL = SQL + " FROM ";
            SQL = SQL + "(SELECT jo_no, 0 AS RTW_QTY_I,0 AS RTW_QTY,SUM(RATIO*QTY) AS SRN_QTY_I,";
            SQL = SQL + " SUM(QTY) AS SRN_QTY,0 AS gmt_qty_a,0 AS gmt_qty_b,0 AS sew_qty_b,0 AS wash_qty_b ";
            SQL = SQL + "    FROM (SELECT   LN.job_order_no AS JO_NO, ";
            SQL = SQL + "                   (CASE ";
            SQL = SQL + "                       WHEN po.garment_type_cd = 'K' OR l.grade = 'A' or l.grade is null ";
            SQL = SQL + "                          THEN 1 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade IN ('A*', 'B*') ";
            SQL = SQL + "                          THEN 0.9 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade = 'B' ";
            SQL = SQL + "                          THEN 0.93 ";
            SQL = SQL + "                       ELSE 0 ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                   ) AS ratio, ";
            SQL = SQL + "                   SUM (LN.trans_qty) AS qty ";
            SQL = SQL + "              FROM invsubmat.inv_issue_hd hd, ";
            SQL = SQL + "                   invsubmat.inv_issue_line LN, ";
            SQL = SQL + "                   invsubmat.inv_srn_line sln, ";
            SQL = SQL + "                   invsubmat.inv_store s, ";
            SQL = SQL + "                   inv_item_lot l, ";
            SQL = SQL + "                   po_hd po ";
            SQL = SQL + "             WHERE hd.issue_hd_id = LN.issue_hd_id ";
            SQL = SQL + "               AND LN.store_cd = s.store_cd ";
            SQL = SQL + "               AND LN.lot_id = l.lot_id(+) ";
            SQL = SQL + "               AND LN.srn_line_id = sln.srn_line_id ";
            SQL = SQL + "               AND exists (select f1 from escmreader.rpt_tmp where f1= LN.job_order_no) ";
            SQL = SQL + "               AND sln.job_order_no = LN.job_order_no ";
            SQL = SQL + "               AND s.inv_fty_cd = '" + factoryCd + "' ";
            SQL = SQL + "               AND hd.status = 'F' ";
            SQL = SQL + "               AND hd.item_type_cd = 'F' ";
            SQL = SQL + "               AND sln.main_body_flag = 'Y' ";
            SQL = SQL + "               AND NVL(HD.PROD_LINE_CD,'OK')<>'YMG-SAMPLE(Special)'";
            SQL = SQL + "               AND po.PO_NO = LN.job_order_no ";
            SQL = SQL + " and NVL(s.warehouse_type_cd,'O')<>'C'";
            SQL = SQL + "  AND l.fabric_type IN ('B','A','E','D','BD')";
            SQL = SQL + "  AND (   (    po.garment_type_cd = 'K' AND hd.TRANS_CD = 'ITPK') OR (    po.garment_type_cd = 'W' AND hd.TRANS_CD = 'ITPW'))"; 
            //SQL = SQL + " AND (NVL(s.warehouse_type_cd,'O')<>'C' or (NVL(s.warehouse_type_cd,'O')='C' and  s.store_cd like '%C2'))";
            SQL = SQL + "          GROUP BY LN.job_order_no, l.grade, po.garment_type_cd) ";
            SQL = SQL + " GROUP BY jo_no UNION ALL ";
            SQL = SQL + " SELECT   jo_no, SUM(RATIO*QTY) AS RTW_QTY_I,SUM(QTY) AS RTW_QTY ,0 AS SRN_QTY_I,0 AS SRN_QTY, ";
            SQL = SQL + " 0 AS gmt_qty_a,0 AS gmt_qty_b,0 AS sew_qty_b,0 AS wash_qty_b";
            SQL = SQL + "    FROM (SELECT   LN.job_order_no AS JO_NO, ";
            SQL = SQL + "                   (CASE ";
            SQL = SQL + "                       WHEN po.garment_type_cd = 'K' OR l.grade = 'A' or l.grade is null ";
            SQL = SQL + "                          THEN 1 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade IN ('A*', 'B*') ";
            SQL = SQL + "                          THEN 0.9 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade = 'B' ";
            SQL = SQL + "                          THEN 0.93 ";
            SQL = SQL + "                       ELSE 0 ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                   ) AS ratio, ";
            SQL = SQL + "                   SUM (LN.trans_qty) AS qty ";
            SQL = SQL + "              FROM invsubmat.inv_rec_hd hd, ";
            SQL = SQL + "                   invsubmat.inv_rec_line LN, ";
            SQL = SQL + "                   invsubmat.inv_srn_line sln, ";
            SQL = SQL + "                   invsubmat.inv_issue_line sn, ";
            SQL = SQL + "                   invsubmat.inv_store s, ";
            SQL = SQL + "                   inv_item_lot l, ";
            SQL = SQL + "                   invsubmat.inv_trans_code tc, ";
            SQL = SQL + "                   po_hd po ";
            SQL = SQL + "             WHERE hd.rec_hd_id = LN.rec_hd_id ";
            SQL = SQL + "               AND hd.store_cd = s.store_cd ";
            SQL = SQL + "               AND LN.lot_id = l.lot_id(+) ";
            SQL = SQL + "               AND exists  (select f1 from escmreader.rpt_tmp where f1=LN.job_order_no) ";
            SQL = SQL + "               AND sn.srn_line_id = sln.srn_line_id ";
            SQL = SQL + "               AND LN.issue_line_id = sn.issue_line_id ";
            SQL = SQL + "               AND s.inv_fty_cd = '" + factoryCd + "' ";
            SQL = SQL + "               AND hd.status = 'F' ";
            SQL = SQL + "               AND hd.item_type_cd = 'F' ";
            SQL = SQL + "               AND hd.trans_cd = tc.trans_cd ";
            SQL = SQL + "               AND tc.trans_type_cd = 'RTW' ";
            SQL = SQL + "               AND sln.main_body_flag = 'Y' ";
            SQL = SQL + "               AND po.PO_NO = LN.job_order_no ";
            SQL = SQL + " and s.warehouse_type_cd<>'C'";
            //SQL = SQL + " AND (NVL(s.warehouse_type_cd,'O')<>'C' or (NVL(s.warehouse_type_cd,'O')='C' and  s.store_cd like '%C2'))";
            SQL = SQL + "          GROUP BY LN.job_order_no, l.grade, po.garment_type_cd) ";
            SQL = SQL + "GROUP BY jo_no ";
            SQL = SQL + " UNION ALL ";
            SQL = SQL + " SELECT  job_order_no,0 AS RTW_QTY_I,0 AS RTW_QTY,0 AS SRN_QTY_I,0 AS SRN_QTY,";
            SQL = SQL + "SUM(gmt_qty_a) AS gmt_qty_a,SUM(gmt_qty_b) AS gmt_qty_b,SUM(sew_qty_b) AS sew_qty_b,SUM(wash_qty_b) AS wash_qty_b";
            SQL = SQL + " FROM (";
            SQL = SQL + " SELECT l.reference_no AS job_order_no,(case when l.grade='A' then sum(nvl(l.qty,0)) else 0 end) as gmt_qty_a, ";
            SQL = SQL + "(case when l.grade='B' then sum(nvl(l.qty,0)) else 0 end) as gmt_qty_b,  ";
            SQL = SQL + "(case when l.grade='B' then sum(nvl(l.sew_qty,0)) else 0 end) as sew_qty_b, ";
            SQL = SQL + "(case when l.grade='B' then sum(nvl(l.wash_qty,0)) else 0 end) as wash_qty_b ";
            SQL = SQL + "  FROM inventory.inv_trans_hd h, inventory.inv_trans_lines l, inventory.inv_store_codes s, ";
            SQL = SQL + "       inventory.inv_stock_class c ";
            SQL = SQL + " WHERE h.trans_header_id = l.trans_header_id ";
            SQL = SQL + "   AND h.from_store_cd = s.store_cd ";
            SQL = SQL + "   AND s.stock_class_cd = c.stock_class_cd ";
            SQL = SQL + "   AND NVL(h.first_receipt_flag, 'N') = 'Y' ";
            SQL = SQL + "   AND c.stock_group_cd = 'L'  and s.STOCK_CLASS_CD in  ('L11','L12')  ";
            SQL = SQL + "   AND l.grade IN ('A', 'B') ";
            SQL = SQL + "   AND exists (select f1 from escmreader.rpt_tmp where f1=l.reference_no) ";
            SQL = SQL + "   and h.doc_no like '" + factoryCd + "%' ";
            SQL = SQL + "   group by l.reference_no,l.grade ) A";
            SQL = SQL + " WHERE 1=1 ";
            SQL = SQL + " GROUP BY job_order_no";
            SQL = SQL + ") AL WHERE 1=1    GROUP BY JO_NO";
            return DBUtility.GetTable(SQL, invConn); //lml
        }

        public static DataTable GetStandardLeftGM(string factoryCd, DbConnection invConn)
        {
            string SQL = " SELECT jo_no,SUM(gmt_qty_a) AS gmt_qty_a,SUM(gmt_qty_b) AS gmt_qty_b,SUM(sew_qty_b) AS sew_qty_b,SUM(wash_qty_b) AS wash_qty_b ";
            SQL = SQL + " ,0 AS RTW_QTY_I,0 AS RTW_QTY,0 AS SRN_QTY_I,0 AS SRN_QTY";
            SQL = SQL + " FROM ";
            SQL = SQL + " (SELECT jo_no, 0 AS RTW_QTY_I,0 AS RTW_QTY,SUM(RATIO*QTY) AS SRN_QTY_I,";
            SQL = SQL + " SUM(QTY) AS SRN_QTY,0 AS gmt_qty_a,0 AS gmt_qty_b,0 AS sew_qty_b,0 AS wash_qty_b ";
            SQL = SQL + "    FROM (SELECT   LN.job_order_no AS JO_NO, ";
            SQL = SQL + "                   (CASE ";
            SQL = SQL + "                       WHEN po.garment_type_cd = 'K' OR l.grade = 'A' or l.grade is null ";
            SQL = SQL + "                          THEN 1 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade IN ('A*', 'B*') ";
            SQL = SQL + "                          THEN 0.9 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade = 'B' ";
            SQL = SQL + "                          THEN 0.93 ";
            SQL = SQL + "                       ELSE 0 ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                   ) AS ratio, ";
            SQL = SQL + "                   SUM (LN.trans_qty) AS qty ";
            SQL = SQL + "              FROM invsubmat.inv_issue_hd hd, ";
            SQL = SQL + "                   invsubmat.inv_issue_line LN, ";
            SQL = SQL + "                   invsubmat.inv_srn_line sln, ";
            SQL = SQL + "                   invsubmat.inv_store s, ";
            SQL = SQL + "                   inv_item_lot l, ";
            SQL = SQL + "                   po_hd po ";
            SQL = SQL + "             WHERE hd.issue_hd_id = LN.issue_hd_id ";
            SQL = SQL + "               AND LN.store_cd = s.store_cd ";
            SQL = SQL + "               AND LN.lot_id = l.lot_id(+) ";
            SQL = SQL + "               AND LN.srn_line_id = sln.srn_line_id ";
            SQL = SQL + "               AND exists (select f1 from escmreader.rpt_tmp where f1= LN.job_order_no) ";
            SQL = SQL + "               AND sln.job_order_no = LN.job_order_no ";
            SQL = SQL + "               AND s.inv_fty_cd = '" + factoryCd + "' ";
            SQL = SQL + "               AND hd.status = 'F' ";
            SQL = SQL + "               AND hd.item_type_cd = 'F' ";
            SQL = SQL + "               AND sln.main_body_flag = 'Y' ";
            SQL = SQL + "               AND NVL(HD.PROD_LINE_CD,'OK')<>'YMG-SAMPLE(Special)'";
            SQL = SQL + "               AND po.PO_NO = LN.job_order_no ";
            SQL = SQL + " and NVL(s.warehouse_type_cd,'O')<>'C'";
            SQL = SQL + "  AND l.fabric_type IN ('B','A','E','D','BD')";
            SQL = SQL + "  AND (   (    po.garment_type_cd = 'K' AND hd.TRANS_CD = 'ITPK') OR (    po.garment_type_cd = 'W' AND hd.TRANS_CD = 'ITPW'))"; 
            //SQL = SQL + " AND (NVL(s.warehouse_type_cd,'O')<>'C' or (NVL(s.warehouse_type_cd,'O')='C' and  s.store_cd like '%C2'))";
            SQL = SQL + "          GROUP BY LN.job_order_no, l.grade, po.garment_type_cd) ";
            SQL = SQL + " GROUP BY jo_no UNION ALL ";
            SQL = SQL + " SELECT   jo_no, SUM(RATIO*QTY) AS RTW_QTY_I,SUM(QTY) AS RTW_QTY ,0 AS SRN_QTY_I,0 AS SRN_QTY, ";
            SQL = SQL + " 0 AS gmt_qty_a,0 AS gmt_qty_b,0 AS sew_qty_b,0 AS wash_qty_b";
            SQL = SQL + "    FROM (SELECT   LN.job_order_no AS JO_NO, ";
            SQL = SQL + "                   (CASE ";
            SQL = SQL + "                       WHEN po.garment_type_cd = 'K' OR l.grade = 'A' or l.grade is null ";
            SQL = SQL + "                          THEN 1 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade IN ('A*', 'B*') ";
            SQL = SQL + "                          THEN 0.9 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade = 'B' ";
            SQL = SQL + "                          THEN 0.93 ";
            SQL = SQL + "                       ELSE 0 ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                   ) AS ratio, ";
            SQL = SQL + "                   SUM (LN.trans_qty) AS qty ";
            SQL = SQL + "              FROM invsubmat.inv_rec_hd hd, ";
            SQL = SQL + "                   invsubmat.inv_rec_line LN, ";
            SQL = SQL + "                   invsubmat.inv_srn_line sln, ";
            SQL = SQL + "                   invsubmat.inv_issue_line sn, ";
            SQL = SQL + "                   invsubmat.inv_store s, ";
            SQL = SQL + "                   inv_item_lot l, ";
            SQL = SQL + "                   invsubmat.inv_trans_code tc, ";
            SQL = SQL + "                   po_hd po ";
            SQL = SQL + "             WHERE hd.rec_hd_id = LN.rec_hd_id ";
            SQL = SQL + "               AND hd.store_cd = s.store_cd ";
            SQL = SQL + "               AND LN.lot_id = l.lot_id(+) ";
            SQL = SQL + "               AND exists  (select f1 from escmreader.rpt_tmp where f1=LN.job_order_no) ";
            SQL = SQL + "               AND sn.srn_line_id = sln.srn_line_id ";
            SQL = SQL + "               AND LN.issue_line_id = sn.issue_line_id ";
            SQL = SQL + "               AND s.inv_fty_cd = '" + factoryCd + "' ";
            SQL = SQL + "               AND hd.status = 'F' ";
            SQL = SQL + "               AND hd.item_type_cd = 'F' ";
            SQL = SQL + "               AND hd.trans_cd = tc.trans_cd ";
            SQL = SQL + "               AND tc.trans_type_cd = 'RTW' ";
            SQL = SQL + "               AND sln.main_body_flag = 'Y' ";
            SQL = SQL + "               AND po.PO_NO = LN.job_order_no ";
            SQL = SQL + " and s.warehouse_type_cd<>'C'"; 
            //SQL = SQL + " AND (NVL(s.warehouse_type_cd,'O')<>'C' or (NVL(s.warehouse_type_cd,'O')='C' and  s.store_cd like '%C2'))";
            SQL = SQL + "          GROUP BY LN.job_order_no, l.grade, po.garment_type_cd) ";
            SQL = SQL + "GROUP BY jo_no ";
            SQL = SQL + " UNION ALL ";
            SQL = SQL + " SELECT  job_order_no,0 AS RTW_QTY_I,0 AS RTW_QTY,0 AS SRN_QTY_I,0 AS SRN_QTY,";
            SQL = SQL + "SUM(gmt_qty_a) AS gmt_qty_a,SUM(gmt_qty_b) AS gmt_qty_b,SUM(sew_qty_b) AS sew_qty_b,SUM(wash_qty_b) AS wash_qty_b";
            SQL = SQL + " FROM (";
            SQL = SQL + " SELECT l.reference_no AS job_order_no,(case when l.grade='A' then sum(nvl(l.qty,0)) else 0 end) as gmt_qty_a, ";
            SQL = SQL + "(case when l.grade='B' then sum(nvl(l.qty,0)) else 0 end) as gmt_qty_b,  ";
            SQL = SQL + "(case when l.grade='B' then sum(nvl(l.sew_qty,0)) else 0 end) as sew_qty_b, ";
            SQL = SQL + "(case when l.grade='B' then sum(nvl(l.wash_qty,0)) else 0 end) as wash_qty_b ";
            SQL = SQL + "  FROM inventory.inv_trans_hd h, inventory.inv_trans_lines l, inventory.inv_store_codes s, ";
            SQL = SQL + "       inv_stock_class c ";
            SQL = SQL + " WHERE h.trans_header_id = l.trans_header_id ";
            SQL = SQL + "   AND h.from_store_cd = s.store_cd ";
            SQL = SQL + "   AND s.stock_class_cd = c.stock_class_cd ";
            SQL = SQL + "   AND NVL(h.first_receipt_flag, 'N') = 'Y' ";
            SQL = SQL + "   AND c.stock_group_cd = 'L'  and s.STOCK_CLASS_CD in  ('L11','L12')  ";
            SQL = SQL + "   AND l.grade IN ('A', 'B') ";
            SQL = SQL + "   AND exists (select f1 from escmreader.rpt_tmp where f1=l.reference_no) ";
            SQL = SQL + "   and h.doc_no like '" + factoryCd + "%' ";
            SQL = SQL + "   group by l.reference_no,l.grade ) A";
            SQL = SQL + " WHERE 1=1 ";
            SQL = SQL + " GROUP BY job_order_no";
            SQL = SQL + ") AL WHERE 1=1    GROUP BY JO_NO";
            return DBUtility.GetTable(SQL, invConn);//LML
        }

        public static DataTable GetStandardLeftSRNAndRTW(string factoryCd, DbConnection invConn)
        {
            string SQL = " SELECT jo_no,SUM(RTW_QTY_I) AS RTW_QTY_I,SUM(RTW_QTY) AS RTW_QTY,SUM(SRN_QTY_I) AS SRN_QTY_I,";
            SQL = SQL + " SUM(SRN_QTY) AS SRN_QTY,0 AS gmt_qty_a,0 AS gmt_qty_b,0 AS sew_qty_b,0 AS wash_qty_b";
            SQL = SQL + " FROM ";
            SQL = SQL + "(SELECT jo_no, 0 AS RTW_QTY_I,0 AS RTW_QTY,SUM(RATIO*QTY) AS SRN_QTY_I,";
            SQL = SQL + " SUM(QTY) AS SRN_QTY,0 AS gmt_qty_a,0 AS gmt_qty_b,0 AS sew_qty_b,0 AS wash_qty_b ";
            SQL = SQL + "    FROM (SELECT   LN.job_order_no AS JO_NO, ";
            SQL = SQL + "                   (CASE ";
            SQL = SQL + "                       WHEN po.garment_type_cd = 'K' OR l.grade = 'A' or l.grade is null ";
            SQL = SQL + "                          THEN 1 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade IN ('A*', 'B*') ";
            SQL = SQL + "                          THEN 0.9 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade = 'B' ";
            SQL = SQL + "                          THEN 0.93 ";
            SQL = SQL + "                       ELSE 0 ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                   ) AS ratio, ";
            SQL = SQL + "                   SUM (LN.trans_qty) AS qty ";
            SQL = SQL + "              FROM invsubmat.inv_issue_hd hd, ";
            SQL = SQL + "                   invsubmat.inv_issue_line LN, ";
            SQL = SQL + "                   invsubmat.inv_srn_line sln, ";
            SQL = SQL + "                   invsubmat.inv_store s, ";
            SQL = SQL + "                   inv_item_lot l, ";
            SQL = SQL + "                   po_hd po ";
            SQL = SQL + "             WHERE hd.issue_hd_id = LN.issue_hd_id ";
            SQL = SQL + "               AND LN.store_cd = s.store_cd ";
            SQL = SQL + "               AND LN.lot_id = l.lot_id(+) ";
            SQL = SQL + "               AND LN.srn_line_id = sln.srn_line_id ";
            SQL = SQL + "               AND exists (select f1 from escmreader.rpt_tmp where f1= LN.job_order_no) ";
            SQL = SQL + "               AND sln.job_order_no = LN.job_order_no ";
            SQL = SQL + "               AND s.inv_fty_cd = '" + factoryCd + "' ";
            SQL = SQL + "               AND hd.status = 'F' ";
            SQL = SQL + "               AND hd.item_type_cd = 'F' ";
            SQL = SQL + "               AND sln.main_body_flag = 'Y' ";
            SQL = SQL + "               AND NVL(HD.PROD_LINE_CD,'OK')<>'YMG-SAMPLE(Special)'";
            SQL = SQL + "               AND po.PO_NO = LN.job_order_no ";
            SQL = SQL + " and NVL(s.warehouse_type_cd,'O')<>'C'";
            SQL = SQL + "  AND l.fabric_type IN ('B','A','E','D','BD')";
            SQL = SQL + "  AND (   (    po.garment_type_cd = 'K' AND hd.TRANS_CD = 'ITPK') OR (    po.garment_type_cd = 'W' AND hd.TRANS_CD = 'ITPW'))"; 
            //SQL = SQL + " AND (NVL(s.warehouse_type_cd,'O')<>'C' or (NVL(s.warehouse_type_cd,'O')='C' and  s.store_cd like '%C2'))";
            SQL = SQL + "          GROUP BY LN.job_order_no, l.grade, po.garment_type_cd) ";
            SQL = SQL + " GROUP BY jo_no UNION ALL ";
            SQL = SQL + " SELECT   jo_no, SUM(RATIO*QTY) AS RTW_QTY_I,SUM(QTY) AS RTW_QTY ,0 AS SRN_QTY_I,0 AS SRN_QTY, ";
            SQL = SQL + " 0 AS gmt_qty_a,0 AS gmt_qty_b,0 AS sew_qty_b,0 AS wash_qty_b";
            SQL = SQL + "    FROM (SELECT   LN.job_order_no AS JO_NO, ";
            SQL = SQL + "                   (CASE ";
            SQL = SQL + "                       WHEN po.garment_type_cd = 'K' OR l.grade = 'A' or l.grade is null ";
            SQL = SQL + "                          THEN 1 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade IN ('A*', 'B*') ";
            SQL = SQL + "                          THEN 0.9 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade = 'B' ";
            SQL = SQL + "                          THEN 0.93 ";
            SQL = SQL + "                       ELSE 0 ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                   ) AS ratio, ";
            SQL = SQL + "                   SUM (LN.trans_qty) AS qty ";
            SQL = SQL + "              FROM invsubmat.inv_rec_hd hd, ";
            SQL = SQL + "                   invsubmat.inv_rec_line LN, ";
            SQL = SQL + "                   invsubmat.inv_srn_line sln, ";
            SQL = SQL + "                   invsubmat.inv_issue_line sn, ";
            SQL = SQL + "                   invsubmat.inv_store s, ";
            SQL = SQL + "                   inv_item_lot l, ";
            SQL = SQL + "                   invsubmat.inv_trans_code tc, ";
            SQL = SQL + "                   po_hd po ";
            SQL = SQL + "             WHERE hd.rec_hd_id = LN.rec_hd_id ";
            SQL = SQL + "               AND hd.store_cd = s.store_cd ";
            SQL = SQL + "               AND LN.lot_id = l.lot_id(+) ";
            SQL = SQL + "               AND exists  (select f1 from escmreader.rpt_tmp where f1=LN.job_order_no) ";
            SQL = SQL + "               AND sn.srn_line_id = sln.srn_line_id ";
            SQL = SQL + "               AND LN.issue_line_id = sn.issue_line_id ";
            SQL = SQL + "               AND s.inv_fty_cd = '" + factoryCd + "' ";
            SQL = SQL + "               AND hd.status = 'F' ";
            SQL = SQL + "               AND hd.item_type_cd = 'F' ";
            SQL = SQL + "               AND hd.trans_cd = tc.trans_cd ";
            SQL = SQL + "               AND tc.trans_type_cd = 'RTW' ";
            SQL = SQL + "               AND sln.main_body_flag = 'Y' ";
            SQL = SQL + "               AND po.PO_NO = LN.job_order_no ";
            SQL = SQL + " and s.warehouse_type_cd<>'C'"; 
            //SQL = SQL + " AND (NVL(s.warehouse_type_cd,'O')<>'C' or (NVL(s.warehouse_type_cd,'O')='C' and  s.store_cd like '%C2'))";
            SQL = SQL + "          GROUP BY LN.job_order_no, l.grade, po.garment_type_cd) ";
            SQL = SQL + "GROUP BY jo_no ";
            SQL = SQL + " UNION ALL ";
            SQL = SQL + " SELECT  job_order_no,0 AS RTW_QTY_I,0 AS RTW_QTY,0 AS SRN_QTY_I,0 AS SRN_QTY,";
            SQL = SQL + "SUM(gmt_qty_a) AS gmt_qty_a,SUM(gmt_qty_b) AS gmt_qty_b,SUM(sew_qty_b) AS sew_qty_b,SUM(wash_qty_b) AS wash_qty_b";
            SQL = SQL + " FROM (";
            SQL = SQL + " SELECT l.reference_no AS job_order_no,(case when l.grade='A' then sum(nvl(l.qty,0)) else 0 end) as gmt_qty_a, ";
            SQL = SQL + "(case when l.grade='B' then sum(nvl(l.qty,0)) else 0 end) as gmt_qty_b,  ";
            SQL = SQL + "(case when l.grade='B' then sum(nvl(l.sew_qty,0)) else 0 end) as sew_qty_b, ";
            SQL = SQL + "(case when l.grade='B' then sum(nvl(l.wash_qty,0)) else 0 end) as wash_qty_b ";
            SQL = SQL + "  FROM inventory.inv_trans_hd h, inventory.inv_trans_lines l, inventory.inv_store_codes s, ";
            SQL = SQL + "       inv_stock_class c ";
            SQL = SQL + " WHERE h.trans_header_id = l.trans_header_id ";
            SQL = SQL + "   AND h.from_store_cd = s.store_cd ";
            SQL = SQL + "   AND s.stock_class_cd = c.stock_class_cd ";
            SQL = SQL + "   AND NVL(h.first_receipt_flag, 'N') = 'Y' ";
            SQL = SQL + "   AND c.stock_group_cd = 'L'  and s.STOCK_CLASS_CD in  ('L11','L12')  ";
            SQL = SQL + "   AND l.grade IN ('A', 'B') ";
            SQL = SQL + "   AND exists (select f1 from escmreader.rpt_tmp where f1=l.reference_no) ";
            SQL = SQL + "   and h.doc_no like '" + factoryCd + "%' ";
            SQL = SQL + "   group by l.reference_no,l.grade ) A";
            SQL = SQL + " WHERE 1=1 ";
            SQL = SQL + " GROUP BY job_order_no";
            SQL = SQL + ") AL WHERE 1=1    GROUP BY JO_NO";
            return DBUtility.GetTable(SQL, invConn);
        }

        public static DataTable GetLocalLeftOver(string factoryCd, DbConnection invConn)
        {
            string SQL = " select PO_NO as PPO_NO,reason_cd,SUM(RATIO*Leftover_QTY) AS Leftover_QTY,SUM(Leftover_QTY) as Leftover_QTY1 ";
            SQL = SQL + "from ";
            SQL = SQL + " (SELECT   lot.PO_NO,lot.reason_cd,";
            SQL = SQL + " (case when ppo.FABRIC_NATURE_CD='K' or lot.grade='A' then 1 else ";
            SQL = SQL + "         case when lot.grade in('A*','B*') THEN 0.9 else ";
            SQL = SQL + "         case when lot.grade ='B' then 0.93 else 0 end end end) as ratio, ";
            SQL = SQL + " sum(nvl(on_hand_qty,0)) as Leftover_QTY   ";
            SQL = SQL + "    FROM INVSUBMAT.inv_stock_items_v a,   ";
            SQL = SQL + "         inv_item i,   ";
            SQL = SQL + "         inv_item_lot lot,   ";
            SQL = SQL + "         INVSUBMAT.inv_store s, ";
            SQL = SQL + "         PPO_HD PPO   ";
            SQL = SQL + "   WHERE a.inv_item_id = i.inv_item_id ";
            SQL = SQL + "     AND a.lot_id = lot.lot_id(+) ";
            SQL = SQL + "        AND a.on_hand_qty > 0 ";
            SQL = SQL + "     AND a.store_cd = s.store_cd ";
            SQL = SQL + "     AND i.item_type_cd = 'F' ";
            SQL = SQL + "     AND exists (select f1 from escmreader.rpt_tmp where f1= lot.PO_NO) ";
            SQL = SQL + "     AND s.stock_class_cd in ('L1','L2','L3','L4') ";
            SQL = SQL + "     AND a.inv_fty_cd ='" + factoryCd + "' ";
            SQL = SQL + "     AND s.active = 'Y' ";
            SQL = SQL + "     and fabric_type='B' ";
            SQL = SQL + "     AND PPO.PPO_NO=LOT.PO_NO ";
            SQL = SQL + "     group by lot.PO_NO,lot.reason_cd,lot.grade,ppo.FABRIC_NATURE_CD) ";
            SQL = SQL + " group by  PO_NO,reason_cd ";
            return DBUtility.GetTable(SQL, invConn);

        }

        public static DataTable GetSCList(string RunNO)
        {
            string SQL = " SELECT DISTINCT SC_NO FROM MU_Report_All_Data A ";
            SQL += " WHERE A.RunNo='" + RunNO + "'; ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetLocalOrderQty(DbConnection eelConn)
        {
            string SQL = "";
            SQL = "SELECT f1 FROM rpt_tmp";
            DataTable DT = DBUtility.GetTable(SQL, eelConn);
            SQL = "";
            SQL = SQL + "        SELECT SC_NO,sum(ORDER_QTY) as ORDER_QTY,sum(SC_QTY) as SC_QTY FROM ";
            SQL = SQL + "         (SELECT SC_NO,NVL(SUM(order_qty),0) AS ORDER_QTY,0 AS SC_QTY FROM ";
            SQL = SQL + "        ( SELECT s.sc_no,SUM (d.order_qty) AS order_qty ";
            SQL = SQL + "        FROM ppo_hd h, ppo_dt d, fab_lib f,         ";
            SQL = SQL + "        (select distinct sc_no,fabric_type_cd,pPO_NO from scx_joi_go_fabric aa  ";
            SQL = SQL + "          where exists (select f1 from rpt_tmp where f1=aa.sc_no)";
            SQL = SQL + "            ) s  ";
            SQL = SQL + "        WHERE h.ppO_NO = d.ppO_NO AND  h.flag != 'X'  ";
            SQL = SQL + "        AND d.fab_ref_cd = f.fab_ref_cd AND h.status IN ('L2', 'R')  ";
            SQL = SQL + "        AND H.PpO_NO=s.ppO_NO  and d.fabric_type_cd=s.fabric_type_cd and s.fabric_type_cd in ('B','BD')  ";
            SQL = SQL + "         GROUP BY s.sc_no  ";
            SQL = SQL + "         UNION ALL SELECT s.sc_no,SUM (pl.order_qty) AS order_qty  ";
            SQL = SQL + "         FROM ppo_hd a, ppox_fab_item b,ppox_lot pl, ";
            SQL = SQL + "        (select distinct sc_no,fabric_type_cd,ppO_NO from scx_joi_go_fabric aa  ";
            SQL = SQL + "          where exists (select f1 from rpt_tmp where f1=aa.sc_no)";
            SQL = SQL + "          ) s  ";
            SQL = SQL + "         WHERE a.ppO_NO = b.ppO_NO AND b.fab_item_id = pl.fab_item_id AND a.flag = 'X'  ";
            SQL = SQL + "         AND a.status IN ('L2', 'R') AND  b.status != 'C'  ";
            SQL = SQL + "         AND A.PpO_NO=s.ppO_NO and b.fabric_type_cd=s.fabric_type_cd and s.fabric_type_cd in ('B','BD') ";
            SQL = SQL + "          GROUP BY  s.sc_no,b.fabric_type_cd  ";
            SQL = SQL + "          UNION ALL SELECT s.sc_no,sum(d.qty) AS order_qty  ";
            SQL = SQL + "          FROM fab_sample_order_hd a, fab_so_combo b,fab_so_product c,fab_so_combo_qty d, ";
            SQL = SQL + "        (select distinct sc_no,fabric_type_cd,ppO_NO from scx_joi_go_fabric aa  ";
            SQL = SQL + "          where exists (select f1 from rpt_tmp where f1=aa.sc_no)";
            SQL = SQL + "          ) s  ";
            SQL = SQL + "          WHERE d.req_prod_cd = c.req_prod_cd and  a.fab_so_no = b.fab_so_no AND b.fab_so_no = d.fab_so_no  ";
            SQL = SQL + "          AND b.combo_seq = d.combo_seq AND a.status IN ('P', 'S', 'D')  ";
            SQL = SQL + "          AND A.fab_so_no=s.ppO_NO and c.fabric_part=s.fabric_type_cd and s.fabric_type_cd in ('B','BD') ";
            SQL = SQL + "         group by a.fab_so_no,s.sc_no     ) AAA  ";
            SQL = SQL + "        GROUP BY SC_NO  ";
            SQL = SQL + " UNION ALL ";
            SQL = SQL + " SELECT SC.SC_NO,0 AS ORDER_QTY,Round(SUM(SC.TOTAL_QTY)/12,2) AS SC_QTY ";
            SQL = SQL + "FROM ";
            SQL = SQL + "(select DISTINCT SC_NO from scx_joi_go_fabric a ";
            SQL = SQL + "   where exists(select ppO_NO from scx_joi_go_fabric aa where ";
            SQL = SQL + "   exists (select f1 from rpt_tmp where f1=aa.sc_no) AND FABRIC_TYPE_CD in ('B','BD')  and aa.ppO_NO=a.ppO_NO) ";
            SQL = SQL + "AND A.FABRIC_TYPE_CD in ('B','BD')) PS,SC_HD SC ";
            SQL = SQL + "WHERE PS.SC_NO=SC.SC_NO  GROUP BY SC.SC_NO) PP WHERE 1=1 GROUP BY SC_NO order by sc_no";
            return DBUtility.GetTable(SQL, eelConn);
        }

        public static DataTable GetLocalMUReportList(string factoryCd, string RunNO, string Datatype)
        {
            string SQL = "";
            if (Datatype == "New")
            {
                //Added by ZouShCh ON 2013.07.29 START
                SQL += " exec Proc_Generate_MU_Report '" + factoryCd + "','" + RunNO + "';";
                //SQL += " exec Proc_Generate_MU_Report_TEST '" + factoryCd + "','" + RunNO + "';";
                //Added by ZouShCh ON 2013.07.29 END
            }
            SQL += " SELECT CUT_DATE,JO_NO as JO_NO,YPD_JOB_NO,SHIP_DATE,BUYER,SC_NO ";
            SQL += ",PPO_NO,WIDTH,style_desc,Wash_Type as WASH_TYPE_CD ";
            SQL += "      ,PATTERN_TYPE,CONVERT(VARCHAR(10),MU ) + '%' as MU,PPO_ORDER_YDS as ORDER_QTY,ALLOCATED as allocatedQty";
            SQL += ",Issued,MARKER_AUDITED as MA,ShipYardage,CUT_WSTG_YDS as CutWastageYPD,";
            //SQL += "	  ,CONVERT(VARCHAR(10),CUT_WSTG_PER ) + '%' as CutWastageYPDPer,";
            SQL += " CASE WHEN ISNULL(Issued,0)=0 THEN 'N/A' ELSE CONVERT(VARCHAR(20),CAST(ISNULL(CUT_WSTG_YDS,0)/Issued*100 AS decimal(38,2))) + '%' END as CutWastageYPDPer,";
            SQL += " DEFECT_FAB,DEFECT_PANELS,ODD_LOSS,SPLICE_LOSS,CUT_REJ_PANELS,MATCH_LOSS,END_LOSS,SHORT_YDS,SEW_MATCH_LOSS,";
            SQL += " LEFTOVER_FAB as Leftover,LEFTOVER_REASON,LEFTOVER_desc,REMNANT";
            SQL += "      ,SRN as SRNQty,RTW as RTW_QTY,ORDER_QTY as ORDERQTY,CUT_QTY as CUTQTY ";
            SQL += "      ,SHIPPED_QTY as SHIP_QTY,SAMPLE as SAMPLE_QTY,PULL_OUT as PULLOUT_QTY";
            SQL += "      ,LEFTOVER_A as GMT_QTY_A,LEFTOVER_B as GMT_QTY_B,LEFTOVER_C as DISCREPANCY_QTY";
            SQL += "      ,TOTAL_LEFTOVER as GMT_QTY_TOTAL,SEW_WSTG_QTY as SewingDz,CONVERT(VARCHAR(10),SEW_WSTG_PER) + '%' as SewingPercent";
            SQL += "      ,WASH_WSTG_QTY as WashingDz,CONVERT(VARCHAR(10),WASH_WSTG_PER) + '%' as WashingPercent,UNACC_GMT as UnaccGmt";
            SQL += "      ,PPO_YPD,BULK_YPD as BULK_NET_YPD,GIVEN_CUT_YPD,BULK_MKR_YPD,YPD_VER as YPD_Var";
            SQL += "      ,CUT_YPD as cutYPD,SHIP_YPD as ShipYPD,CONVERT(VARCHAR(10),SHIP_TO_CUT) + '%' as ShipToCut";
            SQL += "      ,CONVERT(VARCHAR(10),SHIP_TO_RECEIPT) + '%' as ShipToRecv,CONVERT(VARCHAR(10),SHIP_TO_ORDER) + '%' as ShipToOrder";
            SQL += "      ,CONVERT(VARCHAR(10),CUT_TO_RECEIPT) + '%' as cut_to_receipt,CONVERT(VARCHAR(10),CUT_TO_ORDER) + '%' as cut_to_order";
            SQL += "      ,CONVERT(VARCHAR(10),OVER_SHIP) + '%' as OVERSHIP,CONVERT(VARCHAR(10),OVER_CUT) + '%' as OVER_CUT      ";
            SQL += "  FROM dbo.MU_Report_All_Data";
            SQL += " WHERE FTY_CD='" + factoryCd + "' and RunNo='" + RunNO + "' and JO_NO<>'TTL'; ";
            return DBUtility.GetTable(SQL, "MES_UPDATE");
        }

        public static DataTable GetLocalMUReportSummary(string factoryCd, string RunNO)
        {
            string SQL = " SELECT CONVERT(VARCHAR(30),MU)+'%' AS MU,PPO_ORDER_YDS,ALLOCATED";
            SQL += ",Issued,MARKER_AUDITED,ShipYardage,CUT_WSTG_YDS";
            //SQL += ",CONVERT(VARCHAR(10),CUT_WSTG_PER ) + '%' as CutWastageYPDPer,";
            SQL += " ,CASE WHEN isnull(issued,0.00)=0.00  THEN 'N/A' ELSE CONVERT(VARCHAR(20),CAST(ISNULL(CUT_WSTG_YDS,0)/Issued*100 AS decimal(38,2))) + '%' END as CutWastageYPDPer,";
            SQL += " DEFECT_FAB,DEFECT_PANELS,ODD_LOSS,SPLICE_LOSS,CUT_REJ_PANELS,MATCH_LOSS,END_LOSS,SHORT_YDS,SEW_MATCH_LOSS,";
            SQL += " LEFTOVER_FAB,REMNANT";
            SQL += "      ,SRN as SRNQty,RTW as RTW_QTY,ORDER_QTY as ORDERQTY,CUT_QTY as CUTQTY ";
            SQL += "      ,SHIPPED_QTY as SHIP_QTY,SAMPLE as SAMPLE_QTY,PULL_OUT as PULLOUT_QTY";
            SQL += "      ,LEFTOVER_A as GMT_QTY_A,LEFTOVER_B as GMT_QTY_B,LEFTOVER_C as DISCREPANCY_QTY";
            SQL += "      ,TOTAL_LEFTOVER as GMT_QTY_TOTAL,SEW_WSTG_QTY as SewingDz,CONVERT(VARCHAR(10),SEW_WSTG_PER) + '%' as SewingPercent";
            SQL += "      ,WASH_WSTG_QTY as WashingDz,CONVERT(VARCHAR(10),WASH_WSTG_PER) + '%' as WashingPercent,UNACC_GMT as UnaccGmt";
            SQL += "      ,PPO_YPD,BULK_YPD as BULK_NET_YPD,GIVEN_CUT_YPD,BULK_MKR_YPD,YPD_VER as YPD_Var";
            SQL += "      ,CUT_YPD as cutYPD,SHIP_YPD as ShipYPD,CONVERT(VARCHAR(10),SHIP_TO_CUT) + '%' as ShipToCut";
            SQL += "      ,CONVERT(VARCHAR(10),SHIP_TO_RECEIPT) + '%' as ShipToRecv,CONVERT(VARCHAR(10),SHIP_TO_ORDER) + '%' as ShipToOrder";
            SQL += "      ,CONVERT(VARCHAR(10),CUT_TO_RECEIPT) + '%' as cut_to_receipt,CONVERT(VARCHAR(10),CUT_TO_ORDER) + '%' as cut_to_order";
            SQL += "      ,CONVERT(VARCHAR(10),OVER_SHIP) + '%' as OVERSHIP,CONVERT(VARCHAR(10),OVER_CUT) + '%' as OVER_CUT      ";
            SQL += "  FROM dbo.MU_Report_All_Data";
            SQL += "  where fty_cd='" + factoryCd + "' and RunNo='" + RunNO + "' and JO_NO='TTL'; ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetLocalMUReportFabricList(string factoryCd, string RunNO, string Datatype)
        {
            string SQL = "";
            if (Datatype == "New")
            {
                //Added by ZouShCh ON 2013.07.29 START

                SQL += " exec Proc_Generate_MU_Report '" + factoryCd + "','" + RunNO + "';";
                //SQL += " exec Proc_Generate_MU_Report_TEST '" + factoryCd + "','" + RunNO + "';";
                //Added by ZouShCh ON 2013.07.29 END
            }
            SQL += "if not (select object_id('Tempdb..#temp_T')) is null  drop table #temp_T;";
            SQL += " SELECT DISTINCT M.CUT_DATE,M.JO_NO,YPD_JOB_NO,SHIP_DATE,BUYER,HD.SC_NO AS [Sale_Contract],PPO_NO ,WIDTH,STYLE_DESC,WASH_TYPE,";
            SQL += " PATTERN_TYPE,MU,PPO_ORDER_YDS,ALLOCATED,ISSUED,MARKER_AUDITED";
            //SQL += " ,CUT_WSTG_YDS,CUT_WSTG_PER";
            SQL += " ,ISNULL(ISSUED,0)-ISNULL(MARKER_AUDITED,0) AS CUT_WSTG_YDS";
            SQL += " ,case when isnull(issued,0)=0 then null else";
            SQL += " convert(numeric(18,2),((ISNULL(ISSUED,0)-ISNULL(MARKER_AUDITED,0))/ISSUED *100)) END AS CUT_WSTG_PER";
            SQL += " ,ISNULL(DEFECT_FAB,0) AS Defect_fabric,ISNULL(DEFECT_PANELS,0)  AS Defect_Panels,ISNULL(ODD_LOSS,0)  AS Odd_loss,ISNULL(SPLICE_LOSS,0)  AS Splice_loss";
            SQL += ",ISNULL(CUT_REJ_PANELS,0)  AS Cutting_Rej_panels,ISNULL(MATCH_LOSS,0)  AS Match_loss,ISNULL(SHORT_YDS,0) AS SHORT_YDS,ISNULL(END_LOSS,0)  AS Endloss";
            SQL += ",ISNULL(SEW_MATCH_LOSS,0)  AS Sewing_match_loss";
            SQL += ",unacc =(ISNULL(CUT_WSTG_YDS,0)-ISNULL(DEFECT_FAB,0)-ISNULL(DEFECT_PANELS,0)-ISNULL(ODD_LOSS,0)-ISNULL(SPLICE_LOSS,0)-ISNULL(CUT_REJ_PANELS,0)-ISNULL(MATCH_LOSS,0)-ISNULL(SHORT_YDS,0)-ISNULL(END_LOSS,0)-ISNULL(SEW_MATCH_LOSS,0))";
            SQL += " ,NULL AS pullout,NULL AS Remarks";
            SQL += " ,LEFTOVER_FAB,LEFTOVER_REASON,";
            SQL += " LEFTOVER_DESC,REMNANT,SRN,RTW,ORDER_QTY,CUT_QTY,PPO_YPD,BULK_YPD,GIVEN_CUT_YPD,";
            SQL += " BULK_MKR_YPD,NULL AS [YPD Var Reason]";
            SQL += " ,YPD_VER,CUT_YPD,CUT_TO_RECEIPT,";
            SQL += " CUT_TO_ORDER,OVER_SHIP,OVER_CUT,M.CUTLINE";
            SQL += " INTO #temp_T";
            SQL += " FROM ";
            SQL += " MU_Report_All_Data M";
            SQL += " INNER JOIN JO_HD HD ON M.JO_NO = HD.JO_NO";
            SQL += " WHERE FTY_CD='" + factoryCd + "' and RunNo='" + RunNO + "' and M.JO_NO<>'TTL'; ";
            SQL += " SELECT CUT_DATE AS [Start Cutting Date],JO_NO AS [Fty Job order],YPD_JOB_NO AS [YPD Job No]";
            SQL += " ,SHIP_DATE AS [Ship Date],BUYER AS [Buyer],[Sale_Contract] AS [Sale Contract]";
            SQL += " ,PPO_NO AS [PPO NO],WIDTH AS [Fabric Width]";
            SQL += " ,STYLE_DESC AS [Long/ short sleeve],WASH_TYPE AS [Wash Type]";
            SQL += " ,PATTERN_TYPE AS [Fabric Pattern],CONVERT(VARCHAR(30),MU)+'%' AS [Marker Utilization (%)],PPO_ORDER_YDS AS [PPO order yds]";
            SQL += " ,ALLOCATED AS [Allocated],ISSUED AS [Issued],MARKER_AUDITED AS [Marker Audited],ISNULL(CUT_WSTG_YDS,'0.00') AS [Yds]";
            SQL += " ,CASE WHEN ISNULL(CUT_WSTG_PER,0)=0 THEN 'N/A' ELSE CONVERT(VARCHAR(10),round(CUT_WSTG_PER,2)) + '%' END AS [%] ";
            SQL += " ,Defect_fabric AS [Defect fabric],Defect_Panels AS [Defect Panels],Odd_loss AS [Odd loss],Splice_loss AS [Splice loss],";
            SQL += " Cutting_Rej_panels AS [Cutting Rej panels],Match_loss AS [Match loss],SHORT_YDS AS [Short Yds],Endloss AS [Endloss],";
            SQL += " Sewing_match_loss AS [Sewing match loss], unacc, pullout, Remarks";
            SQL += " ,LEFTOVER_FAB AS [Leftover fabric]";
            SQL += " ,LEFTOVER_REASON AS [LeftOver Fab Reason Code],LEFTOVER_DESC AS [LeftOver Fab Reason]";
            SQL += ",REMNANT AS [Remnant],SRN,RTW";
            SQL += " ,ORDER_QTY AS [Order(DZ)],CUT_QTY AS [Cut],PPO_YPD AS [PPO MKR YPD],BULK_YPD AS [BULK NET YPD]";
            SQL += " ,GIVEN_CUT_YPD AS [GIVEN CUT YPD],BULK_MKR_YPD AS [BULK MKR YPD]";
            SQL += " ,YPD_VER AS [YPD Var],[YPD Var Reason]";
            SQL += " ,CUT_YPD AS [CUT YPD],CONVERT(VARCHAR(10),CUT_TO_RECEIPT) + '%' as [Cut-to-Receipt]";
            SQL += " ,CONVERT(VARCHAR(10),CUT_TO_ORDER) + '%'  as [Cut-to-Order]";
            SQL += " ,CONVERT(VARCHAR(10),OVER_SHIP) + '%' as [Buyer's max over shipment allowance]";
            SQL += " ,CONVERT(VARCHAR(10),OVER_CUT) + '%' as [Buyer's max over cut allowance],CUTLINE as CUT_LINE";
            SQL += " FROM #temp_T";
            SQL += " UNION ALL";
            SQL += " SELECT NULL,'Total/Weighted Average:',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL";
            SQL += " ,CONVERT(VARCHAR(30),CAST(SUM(ISNULL(MU,0))/COUNT(*) AS DECIMAL(18,2)))+'%' ";
            SQL += " ,CAST(SUM(ISNULL(PPO_ORDER_YDS,0))/COUNT(*) AS DECIMAL(18,2)),CAST(SUM(ISNULL(ALLOCATED,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(ISSUED,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(MARKER_AUDITED,0))/COUNT(*) AS DECIMAL(18,2)),CAST(SUM(ISNULL(CUT_WSTG_YDS,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CONVERT(VARCHAR(10),CAST(SUM(ISNULL(CUT_WSTG_PER,0))/COUNT(*) AS DECIMAL(18,2))) + '%'";
            SQL += " ,CAST(SUM(ISNULL(Defect_fabric,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(Defect_Panels,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(Odd_loss,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(Splice_loss,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(Cutting_Rej_panels,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(Match_loss,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(SHORT_YDS,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(Endloss,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(Sewing_match_loss,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(unacc,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(pullout,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(Remarks,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(LEFTOVER_FAB,0))/COUNT(*) AS DECIMAL(18,2)),NULL,NULL,CAST(SUM(ISNULL(REMNANT,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(SRN,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(RTW,0))/COUNT(*) AS DECIMAL(18,2)),CAST(SUM(ISNULL(ORDER_QTY,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(CUT_QTY,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(PPO_YPD,0))/COUNT(*) AS DECIMAL(18,2)),CAST(SUM(ISNULL(BULK_YPD,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(GIVEN_CUT_YPD,0))/COUNT(*) AS DECIMAL(18,2)),CAST(SUM(ISNULL(BULK_MKR_YPD,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(YPD_VER,0))/COUNT(*) AS DECIMAL(18,2)),NULL,NULL,NULL";
            SQL += " ,CONVERT(VARCHAR(10),CAST(SUM(ISNULL(CUT_TO_ORDER,0))/COUNT(*) AS DECIMAL(18,2))) + '%'";
            SQL += " ,CONVERT(VARCHAR(10),CAST(SUM(ISNULL(OVER_SHIP,0))/COUNT(*) AS DECIMAL(18,2))) + '%',NULL,NULL";
            SQL += " FROM #temp_T";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetLocalMUReportGarmentList(string factoryCd, string RunNO, string fromdate, string todate, string Datatype, bool ProductionLine)
        {
            string SQL = "";
            if (Datatype == "New")
            {
                
                SQL += " exec Proc_Generate_MU_Report '" + factoryCd + "','" + RunNO + "';";
                
            }
            SQL += "IF OBJECT_ID('tempdb..#temp_T') IS NOT NULL ";
            SQL += "    DROP TABLE #temp_T;";
            SQL += " SELECT IDENTITY (INT,1,1) as ID,SHIP_DATE,LINE AS [TEAM],JO_NO,BUYER,ORDER_QTY,CUT_QTY";
            SQL += " ,SHIPPED_QTY,OVER_CUT,OVER_SHIP,CAST(SHIPPED_QTY/ORDER_QTY * 100 AS DECIMAL(18,2)) AS [S/O]";
            SQL += " ,[SAMPLE],PULL_OUT,LEFTOVER_A,LEFTOVER_B";
            SQL += ",ISNULL(SEWDFC_QTY,0) AS [LEFTOVER_C_1],ISNULL(MTRMS_QTY,0) AS [LEFTOVER_C_2]";
            SQL += ",ISNULL(SEWDFD_QTY,0) AS [LEFTOVER_C_3],ISNULL(SEWDF_QTY,0) AS [LEFTOVER_C_4]";
            SQL += ",ISNULL(PRTDF_QTY,0) AS [LEFTOVER_C_5],UNACC_GMT ";
            SQL += " INTO #temp_T";
            SQL += " FROM MU_Report_All_Data ";
            SQL += " WHERE FTY_CD='" + factoryCd + "'";
            SQL += " AND RunNo = '" + RunNO + "'";
            SQL += " and JO_NO<>'TTL'; ";

            if (!ProductionLine)
            {
                SQL += " SELECT DISTINCT CONVERT(VARCHAR(10),SHIP_DATE,120) AS SHIP_DATE";
                SQL += " ,[TEAM],JO_NO, BUYER,";
                SQL += " ORDER_QTY AS [Order Qty],";
                SQL += " CUT_QTY AS [Cut Qty],";
                SQL += " SHIPPED_QTY as [Ship Qty],OVER_CUT AS [Buyer's max over cut allowance]";
                SQL += ",OVER_SHIP AS [Buyer's max over shipment allowance],";
                SQL += " CONVERT(VARCHAR(20),[S/O]) +'%' AS [S/O]";
                SQL += " ,[SAMPLE] as [Sample],PULL_OUT AS [Pull-out],LEFTOVER_A AS [Grade A],LEFTOVER_B AS [Grade B]";
                SQL += " ,[LEFTOVER_C_1],[LEFTOVER_C_2],[LEFTOVER_C_3],[LEFTOVER_C_4],[LEFTOVER_C_5]";
                SQL += " ,UNACC_GMT as [TTL:Unacc Gmt] ";
                SQL += " FROM #temp_T";
            }
            else
            {
                SQL += " select A.*,B.Production_line_cd from ";
                SQL += "(SELECT DISTINCT CONVERT(VARCHAR(10),SHIP_DATE,120) AS SHIP_DATE ,[TEAM],JO_NO, BUYER, ORDER_QTY AS [Order Qty], ";
                SQL += " CUT_QTY AS [Cut Qty], SHIPPED_QTY as [Ship Qty],OVER_CUT AS [Buyer's max over cut allowance],";
                SQL += " OVER_SHIP AS [Buyer's max over shipment allowance], CONVERT(VARCHAR(20),[S/O]) +'%' AS [S/O] ,";
                SQL += " [SAMPLE] as [Sample],PULL_OUT AS [Pull-out],LEFTOVER_A AS [Grade A],LEFTOVER_B AS [Grade B] ,[LEFTOVER_C_1],";
                SQL += " [LEFTOVER_C_2],[LEFTOVER_C_3],[LEFTOVER_C_4],[LEFTOVER_C_5] ,UNACC_GMT as [TTL:Unacc Gmt] FROM #temp_T) as A";
                SQL += " Left join";
                SQL += " (SELECT  DISTINCT job_order_no,STUFF(";
                SQL += " ( SELECT ','+Production_line_cd";
                SQL += " FROM (select Distinct A.Production_line_cd, A.Job_order_No ";               
                SQL += " from prd_jo_wip_hd A inner join Gen_PRC_CD_MST B on A.PROCESS_CD=B.PRC_CD AND A.GARMENT_TYPE=B.GARMENT_TYPE ";                
                SQL += " where   A.PROCESS_CD='SEW' and A.process_type='I' and (in_qty<>0 and out_qty<>0) and a.job_order_no in (select jo_no from #temp_T) ) as B";
                SQL += " WHERE B.Job_order_no = A.Job_order_no";
                SQL += " FOR XML PATH('') )";
                SQL += " ,1,1,'')AS Production_line_cd";
                SQL += " FROM (select Distinct A.Production_line_cd, A.Job_order_No ";

                SQL += " from prd_jo_wip_hd A inner join Gen_PRC_CD_MST B on A.PROCESS_CD=B.PRC_CD  AND A.GARMENT_TYPE=B.GARMENT_TYPE ";

                SQL += " where   A.PROCESS_CD='SEW' and A.process_type='I' and (in_qty<>0 and out_qty<>0) and a.job_order_no in (select jo_no from #temp_T) ) AS A";
                SQL += " ) as B on A.jo_no=B.job_order_no ";
            }
        



            return DBUtility.GetTable(SQL, "MES");
        }
        //================FactoryMUReport========================================
        //======================================================================

        //======================================================================
        //================Common===============================================

        public static DataTable GetStandardWidthNpatternList(DbConnection conn)
        {
//            string SQL = " select SC_NO,PPO_NO,WIDTH,nvl(PATTERN_TYPE_CD,'Stripe') as PATTERN_TYPE";
//            SQL = SQL + ",nvl(max(PPO_YPD),max(BULK_YPD))as PPO_YPD,YPD_JOB_NO";
//            SQL = SQL + " From scx_joi_go_fabric a where SC_no in ( select  SC_NO From PO_HD where 1 = 1 and PO_NO in (select f1 from rpt_tmp)";
//            SQL = SQL + " ) ";
//            SQL = SQL + @"      AND 
//                                   ( EXISTS (
//                                SELECT fabric_type_cd
//                                  FROM ppo_hd ph INNER JOIN ppox_fab_item pfi
//                                       ON ph.ppo_no = pfi.ppo_no
//                                 WHERE ph.ppo_no = a.ppo_no
//                                   AND pfi.fabric_type_cd in ( 'B','BD')
//                                   AND ph.status != 'C'
//                                   AND pfi.status != 'C'
//                                UNION
//                                SELECT fp.fabric_part
//                                  FROM ppo_hd ph INNER JOIN fab_product fp
//                                       ON ph.ppo_no = fp.prod_ppo_no
//                                 WHERE ph.ppo_no = a.ppo_no
//                                   AND fp.fabric_part = 'B'
//                                   AND ph.status != 'C') 
//                                  OR (a.fabric_type_cd in( 'B','BD'))
//                                     )
//                                ";
//            SQL = SQL + " group by SC_NO,PPO_NO,WIDTH,PATTERN_TYPE_CD,YPD_JOB_NO ";
//            SQL = SQL + " Order by SC_NO,PPO_NO,WIDTH,PATTERN_TYPE_CD ";
            StringBuilder SQL = new StringBuilder();
            SQL.Append(@"
            SELECT A.SC_NO,
                 A.PPO_NO,
                 A.WIDTH,
                 NVL (A.PATTERN_TYPE_CD, 'Stripe') AS PATTERN_TYPE,
                 NVL (MAX (A.PPO_YPD), MAX (A.BULK_YPD)) AS PPO_YPD,
                 A.YPD_JOB_NO
            FROM SCX_JOI_GO_FABRIC A
              LEFT JOIN (SELECT PH.SC_NO,SLP.PPO_NO,PH.PO_NO
                  FROM PO_HD PH
                      LEFT JOIN SCX_LOT_PPO SLP ON SLP.SC_NO=PH.SC_NO AND SLP.LOT_NO=PH.LOT_NO 
                  WHERE PH.PO_NO in (select f1 from rpt_tmp) AND PH.STATUS<>'X') PL ON PL.SC_NO=A.SC_NO AND NVL(PL.PPO_NO,'-')=DECODE(NVL(PL.PPO_NO,'-'),'-','-',A.PPO_NO)
            WHERE A.SC_NO IN (SELECT SC_NO FROM PO_HD WHERE 1 = 1 AND PO_NO in (select f1 from rpt_tmp))
              AND A.FABRIC_TYPE_CD IN ('B', 'BD')
            GROUP BY A.SC_NO,
                 A.PPO_NO,
                 A.WIDTH,
                 A.PATTERN_TYPE_CD,
                 A.YPD_JOB_NO
            ORDER BY A.SC_NO,
                 A.PPO_NO,
                 A.WIDTH,
                 A.PATTERN_TYPE_CD 
                 ");
            return DBUtility.GetTable(SQL.ToString(), conn);
        }
        //Added By ZouShiChang ON 2013.10.10 End FI13-057




        public static DataTable GetStandardGisInfo(string JoList, string factoryCd, string GARMENT_TYPE_CD, string OUTSOURCE_TYPE, string fromDate, string toDate, bool shipJo, bool NoPost, string RunNO)
        {
            string SQL = "set session tx_isolation='READ-UNCOMMITTED';";
            SQL += "delete from  MU_SHIP_JO_INFO where Run_No='" + RunNO + "';";
            SQL = SQL + " delete from  MU_SHIP_JO_INFO where datediff(now(),create_date) >1;";

            SQL = SQL + " INSERT INTO MU_SHIP_JO_INFO    ";

            if (factoryCd.ToUpper().Equals("EGM"))
            {
                SQL = SQL + " SELECT CASE WHEN INSTR(DD.SC_NO,'EGM')=1 THEN REPLACE(DD.SC_NO,'EGM','') ELSE DD.SC_NO END AS SC_NO,DD.CT_NO ,";
            }
            else
            {
                SQL = SQL + " SELECT DD.SC_NO,DD.CT_NO ,";
            }

            SQL = SQL + "'1900-01-01' AS SHIP_DATE,0 AS SHIP_QTY,'N' AS LASTJO, ";
            SQL = SQL + " '" + RunNO + "' AS  Run_No,now() as create_date   ";
            SQL = SQL + " FROM FTY_INVOICE_HD DA,FTY_INVOICE_DT DD  WHERE (DA.INVOICE_NO LIKE 'J" + factoryCd + "%'  or DA.INVOICE_NO LIKE 'K" + factoryCd + "%'  )  ";
            SQL = SQL + " AND  DA.INVOICE_ID=DD.INVOICE_ID  ";
            if (factoryCd != "")
            {
                SQL = SQL + " AND DA.FACTORY_CD='" + factoryCd + "'";
            }
            if (JoList != "")
            {
                SQL += " and DD.CT_NO in (" + JoList + ") ";
            }

            SQL = SQL + " AND DD.CT_NO<>'' ";
            if (fromDate != "")
            {
                SQL += " AND DA.SHIP_DATE >='" + fromDate + "'";
            }
            if (toDate != "")
            {
                SQL += " AND DA.SHIP_DATE <='" + toDate + "'";
            }
            SQL = SQL + " GROUP BY DD.SC_NO,DD.CT_NO ";
            SQL = SQL + " UNION ";
            SQL = SQL + " SELECT C.SC_NO,C.CT_NO,'1900-01-01' AS SHIP_DATE,0 AS SHIP_QTY,'N' AS LASTJO, ";
            SQL = SQL + " '" + RunNO + "' AS  Run_No,now() as create_date   ";
            SQL = SQL + " FROM CUST_INVOICE_HD A,  CUST_INVOICE_DT C  ";
            SQL = SQL + " WHERE  C.INVOICE_ID=A.INVOICE_ID  AND A.INVOICE_NO LIKE 'V" + factoryCd + @"%' "; 
            if (factoryCd != "")
            {
                SQL = SQL + " AND A.FACTORY_CD='" + factoryCd + "'";
            }
            if (JoList != "")
            {
                SQL += " and C.CT_NO in (" + JoList + ") ";
            }

            SQL = SQL + " AND C.CT_NO<>'' ";
            if (fromDate != "")
            {
                SQL += " AND A.SHIP_DATE >='" + fromDate + "'";
            }
            if (toDate != "")
            {
                SQL += " AND A.SHIP_DATE <='" + toDate + "'";
            }
            SQL = SQL + " GROUP BY C.SC_NO,C.CT_NO ;";

            SQL = SQL + " update MU_SHIP_JO_INFO set LASTJO='T' WHERE Run_No='" + RunNO + "';";

            SQL = SQL + " INSERT INTO MU_SHIP_JO_INFO  ";
            SQL = SQL + "  SELECT SC_NO,JO_NO, max(SHIP_DATE) as SHIP_DATE,SUM(SHIP_QTY) AS SHIP_QTY,'N' AS LASTJO,'" + RunNO + "' AS  Run_No,now() as create_date ";
            SQL = SQL + " FROM ( ";
            SQL = SQL + " SELECT D.SC_NO,C.CT_NO AS JO_NO,max(A.SHIP_DATE) as SHIP_DATE,SUM(IF (A.UOM_CD='DOZ',C.QUANTITY*12,C.QUANTITY)) AS SHIP_QTY ";
            SQL = SQL + " FROM FTY_INVOICE_HD A, ";
            SQL = SQL + @" FTY_INVOICE_DT C,SC_HD D ,MU_SHIP_JO_INFO T,
                            (
                                SELECT T1.INVOICE_NO,MAX(T1.REVISE_NO) AS MAX_REVISE_NO
			                         FROM FTY_INVOICE_HD T1,FTY_INVOICE_DT T2,MU_SHIP_JO_INFO T3
			                         WHERE T3.Run_No='" + RunNO + @"' AND T2.CT_NO=T3.CT_NO
			                         AND T1.INVOICE_ID=T2.INVOICE_ID
			                         AND (T1.INVOICE_NO LIKE 'J" + factoryCd + @"%'  or T1.INVOICE_NO LIKE 'K" + factoryCd + "%' )";
            if (factoryCd != "")
            {
                SQL = SQL + " AND T1.FACTORY_CD='" + factoryCd + "'";
            }
            //Added By ZouShiChang ON 2014.02.14 Start 
            /*
            if (!NoPost)
            {
                SQL = SQL + " AND T1.STATUS = 'POST'  ";
            }
             */ 
            //Added By ZouShiChang ON 2014.02.14 End
            if (JoList != "")
            {
                SQL += " and T3.CT_NO in (" + JoList + ") ";
            }

            SQL += " GROUP BY T1.INVOICE_NO)E ";


            if (factoryCd.ToUpper().Equals("EGM"))
            {
                SQL = SQL + " WHERE  A.INVOICE_NO=E.INVOICE_NO AND A.REVISE_NO=E.MAX_REVISE_NO AND C.INVOICE_ID=A.INVOICE_ID AND D.SC_NO=CASE WHEN INSTR(C.SC_NO,'EGM')=1 THEN REPLACE(C.SC_NO,'EGM','') ELSE C.SC_NO END  ";
            }
            else
            {
                SQL = SQL + " WHERE  A.INVOICE_NO=E.INVOICE_NO AND A.REVISE_NO=E.MAX_REVISE_NO AND C.INVOICE_ID=A.INVOICE_ID AND D.SC_NO=C.SC_NO ";
            }

            if (factoryCd != "")
            {
                SQL = SQL + " AND A.FACTORY_CD='" + factoryCd + "'";
            }
            if (JoList != "")
            {
                SQL += " and C.CT_NO in (" + JoList + ") ";
            }
            if (GARMENT_TYPE_CD != "")
            {
                SQL = SQL + " AND C.GARMENT_TYPE_CD='" + GARMENT_TYPE_CD + "'";
            }
            if (GARMENT_TYPE_CD != "")
            {
                SQL = SQL + " AND C.GARMENT_TYPE_CD='" + GARMENT_TYPE_CD + "'";
            }
            if (OUTSOURCE_TYPE == "STANDARD")
            {
                SQL = SQL + " AND D.SAM_GROUP_CD<>'OUTSOURCE' ";
            }
            if (OUTSOURCE_TYPE == "OUTSOURCE")
            {
                SQL = SQL + " AND D.SAM_GROUP_CD='OUTSOURCE' ";
            }
            SQL = SQL + " AND T.Run_No='" + RunNO + "' AND T.CT_NO=C.CT_NO";

            if (!NoPost)
            {
                SQL = SQL + " AND A.STATUS = 'POST'  ";
            }

            SQL = SQL + " GROUP BY C.SC_NO,C.CT_NO ";
            SQL = SQL + " UNION ALL";
            SQL = SQL + " SELECT C.SC_NO,C.CT_NO AS JO_NO,max(A.SHIP_DATE) as SHIP_DATE,SUM(C.QUANTITY) AS SHIP_QTY ";
            SQL = SQL + " FROM CUST_INVOICE_HD A, ";
            SQL = SQL + " CUST_INVOICE_DT C,SC_HD D ,MU_SHIP_JO_INFO T,";
            SQL = SQL + @"(SELECT T1.INVOICE_NO,MAX(T1.REVISE_NO) AS MAX_REVISE_NO 
			 		    FROM CUST_INVOICE_HD T1, 
						CUST_INVOICE_DT T2,MU_SHIP_JO_INFO T3  
		                WHERE  T1.INVOICE_ID=T2.INVOICE_ID  AND  T2.CT_NO=T3.CT_NO AND T1.INVOICE_NO LIKE 'V" + factoryCd + @"%'" ; 
            if (factoryCd != "")
            {
                SQL = SQL + " AND T1.FACTORY_CD='" + factoryCd + "'";
            }
            if (!NoPost)
            {
                SQL = SQL + " AND T1.STATUS = 'POST'  ";
            }

            if (JoList != "")
            {
                SQL += " and T3.CT_NO in (" + JoList + ") ";
            }


            SQL = SQL + " GROUP BY T1.INVOICE_NO ) E ";

            SQL = SQL + " WHERE  E.INVOICE_NO=A.INVOICE_NO AND A.REVISE_NO=E.MAX_REVISE_NO AND C.INVOICE_ID=A.INVOICE_ID AND D.SC_NO=C.SC_NO ";
            if (factoryCd != "")
            {
                SQL = SQL + " AND A.FACTORY_CD='" + factoryCd + "'";
            }
            if (JoList != "")
            {
                SQL += " and C.CT_NO in (" + JoList + ") ";
            }
            if (GARMENT_TYPE_CD != "")
            {
                SQL = SQL + " AND C.GARMENT_TYPE_CD='" + GARMENT_TYPE_CD + "'";
            }
            //if (GARMENT_TYPE_CD != "")
            //{
            //    SQL = SQL + " AND C.GARMENT_TYPE_CD='" + GARMENT_TYPE_CD + "'";
            //}
            if (OUTSOURCE_TYPE == "STANDARD")
            {
                SQL = SQL + " AND D.SAM_GROUP_CD<>'OUTSOURCE' ";
            }
            if (OUTSOURCE_TYPE == "OUTSOURCE")
            {
                SQL = SQL + " AND D.SAM_GROUP_CD='OUTSOURCE' ";
            }
            SQL = SQL + " AND T.Run_No='" + RunNO + "' AND T.CT_NO=C.CT_NO";

            if (!NoPost)
            {
                SQL = SQL + " AND A.STATUS = 'POST'  ";
            }
            SQL = SQL + " GROUP BY C.SC_NO,C.CT_NO ";

            SQL = SQL + "  ) FF WHERE 1=1 GROUP BY SC_NO,JO_NO; ";


            SQL = SQL + " DELETE FROM MU_SHIP_JO_INFO WHERE  run_no='" + RunNO + "' AND LASTJO='T';";

            SQL = SQL + " UPDATE MU_SHIP_JO_INFO as M, ";
            SQL = SQL + " ( SELECT maxdate.SC_NO,max(DT.ct_no) AS JO_NO  FROM ( ";
            SQL = SQL + " select I.SC_NO,SHIP_DATE,CC,COUNT(SC.SC_NO) AS TC  ";
            SQL = SQL + "FROM  ";
            SQL = SQL + "(  ";
            SQL = SQL + " SELECT II.SC_NO,MAX(II.SHIP_DATE) as SHIP_DATE,COUNT(II.CT_NO) AS CC FROM (  ";
            SQL = SQL + "SELECT C.SC_NO,MAX(A.SHIP_DATE) as SHIP_DATE,C.CT_NO ";
            SQL = SQL + "FROM FTY_INVOICE_HD A, MU_SHIP_JO_INFO B, ";
            SQL = SQL + "FTY_INVOICE_DT C,SC_HD D ,SC_LOT L ";
            SQL = SQL + "WHERE (A.INVOICE_NO LIKE 'J" + factoryCd + "%'  ";
            SQL = SQL + "or A.INVOICE_NO LIKE 'K" + factoryCd + "%' ) ";
            SQL = SQL + "AND C.INVOICE_ID=A.INVOICE_ID AND D.SC_NO=C.SC_NO  AND L.SC_NO=D.SC_NO ";
            SQL = SQL + "AND  L.PO_NO=C.CT_NO  AND B.Run_No='" + RunNO + "'             ";
            SQL = SQL + "    AND A.FACTORY_CD='" + factoryCd + "' ";
            if (!NoPost)
            {
                SQL = SQL + " AND A.STATUS = 'POST' ";
            }
            SQL = SQL + "and  L.SC_NO=B.SC_NO        ";
            SQL = SQL + "GROUP BY C.SC_NO,C.CT_NO  ";
            SQL = SQL + "UNION ALL ";
            SQL = SQL + "SELECT C.SC_NO,MAX(A.SHIP_DATE) as SHIP_DATE,C.CT_NO ";
            SQL = SQL + "FROM CUST_INVOICE_HD A,MU_SHIP_JO_INFO B,  ";
            SQL = SQL + "CUST_INVOICE_DT C,SC_HD D ,SC_LOT L ";
            SQL = SQL + "WHERE INVOICE_NO LIKE 'V" + factoryCd + @"%' "; 
            SQL = SQL + "AND C.INVOICE_ID=A.INVOICE_ID AND D.SC_NO=C.SC_NO  AND L.SC_NO=D.SC_NO ";
            SQL = SQL + "AND  L.PO_NO=C.CT_NO  AND B.Run_No='" + RunNO + "'   ";
            SQL = SQL + "AND A.FACTORY_CD='" + factoryCd + "' ";
            if (!NoPost)
            {
                SQL = SQL + " AND A.STATUS = 'POST' ";
            }
            SQL = SQL + "AND L.SC_NO=B.SC_NO       ";
            SQL = SQL + "GROUP BY C.SC_NO ,C.CT_NO ";
            SQL = SQL + ") II WHERE 1=1 GROUP BY II.SC_NO )I,SC_LOT SC  ";
            SQL = SQL + "WHERE SC.active='Y' AND I.SC_NO=SC.SC_NO  ";
            SQL = SQL + "GROUP BY SC.SC_NO,I.SHIP_DATE,CC             ";




            SQL = SQL + ") maxdate,  ";
            SQL = SQL + "MU_SHIP_JO_INFO DT WHERE maxdate.CC=maxdate.TC  ";
            SQL = SQL + "AND maxdate.SHIP_DATE=DT.SHIP_DATE  ";
            SQL = SQL + "AND DT.Run_No='" + RunNO + "'   ";
            SQL = SQL + "AND maxdate.SC_NO=DT.SC_NO  ";
            SQL = SQL + "GROUP BY maxdate.SC_NO) as LJ ";
            SQL = SQL + " SET M.LASTJO='Y' ";
            SQL = SQL + " WHERE LJ.JO_NO=M.CT_NO ";
            SQL = SQL + " AND M.Run_No='" + RunNO + "' ;";
            SQL = SQL + " SELECT SC_NO,CT_NO AS JO_NO,SHIP_DATE,SHIP_QTY,LASTJO FROM MU_SHIP_JO_INFO WHERE Run_No='" + RunNO + "' ";
            if (!shipJo)
            {
                SQL = SQL + "AND  exists (select JOB_ORDER_NO from JO_SHIP_STATUS C where SHIP_STATUS='Y' AND FACTORY_CD='" + factoryCd + "' AND C.JOB_ORDER_NO=MU_SHIP_JO_INFO.CT_NO)";
            }
            return DBUtility.GetTable(SQL, "GIS");
        }

        public static DataTable GetStandardGisInfo(string factoryCd, string GARMENT_TYPE_CD, string OUTSOURCE_TYPE, string JoList, string fromDate, string toDate)
        {
            if (JoList.Length > 0)
            {
                fromDate = "";
                toDate = "";
            }
            //and C.CT_NO in (" + JoList + ")
            StringBuilder SQL =new StringBuilder("set session tx_isolation='READ-UNCOMMITTED';");
            SQL.AppendFormat(@"SELECT DD.CT_NO
                         FROM FTY_INVOICE_HD DA,FTY_INVOICE_DT DD,SC_HD D  WHERE (DA.INVOICE_NO LIKE 'J{0}%'  or DA.INVOICE_NO LIKE 'K{0}%'  )   AND  DA.INVOICE_ID=DD.INVOICE_ID
                         AND D.SC_NO = DD.SC_NO AND DA.FACTORY_CD='{0}' AND DD.CT_NO<>'' ", factoryCd);
            if (JoList.Length > 0)
                SQL.AppendFormat(" and DD.CT_NO ='{0}'", JoList);
            if (fromDate.Length > 0)
            {
                SQL.AppendFormat(" and  DA.SHIP_DATE >='{0}'", fromDate);
            }
            if (fromDate.Length > 0)
            {
                SQL.AppendFormat(" and  DA.SHIP_DATE <'{0}'", toDate);
            }
            if (GARMENT_TYPE_CD.Length > 0)
            {
                SQL.AppendFormat(" and  D.GARMENT_TYPE_CD ='{0}'", GARMENT_TYPE_CD);
            }
            if (OUTSOURCE_TYPE == "STANDARD")
            {
                SQL.Append(" AND D.SAM_GROUP_CD<>'OUTSOURCE' ");
            }
            if (OUTSOURCE_TYPE == "OUTSOURCE")
            {
                SQL.Append(" AND D.SAM_GROUP_CD='OUTSOURCE' ");
            }
            SQL.Append(" GROUP BY DD.CT_NO ");
            SQL.Append(" UNION ");
            SQL.AppendFormat(@" SELECT C.CT_NO
                         FROM CUST_INVOICE_HD A,  CUST_INVOICE_DT C,SC_HD D    WHERE  C.INVOICE_ID=A.INVOICE_ID AND D.SC_NO = C.SC_NO   AND A.INVOICE_NO LIKE 'V{0}%'  AND A.FACTORY_CD='{0}'
                           AND C.CT_NO<>''", factoryCd);
            if (JoList.Length > 0)
                SQL.AppendFormat(" and C.CT_NO ='{0}'", JoList);
            if (fromDate.Length > 0)
            {
                SQL.AppendFormat(" and  A.SHIP_DATE >='{0}'", fromDate);
            }
            if (fromDate.Length > 0)
            {
                SQL.AppendFormat(" and  A.SHIP_DATE <='{0}'", toDate);
            }
            if (GARMENT_TYPE_CD.Length > 0)
            {
                SQL.AppendFormat(" and  D.GARMENT_TYPE_CD ='{0}'", GARMENT_TYPE_CD);
            }
            if (OUTSOURCE_TYPE == "STANDARD")
            {
                SQL.Append(" AND D.SAM_GROUP_CD<>'OUTSOURCE' ");
            }
            if (OUTSOURCE_TYPE == "OUTSOURCE")
            {
                SQL.Append(" AND D.SAM_GROUP_CD='OUTSOURCE' ");
            }
            SQL.Append("GROUP BY C.CT_NO");

            return DBUtility.GetTable(SQL.ToString(), "GIS");
         }

        public static DataTable GetStandardGisInfo(string JoList, string factoryCd, string GARMENT_TYPE_CD)
        {
            StringBuilder SQL = new StringBuilder();
            SQL.Append(@"
                        SELECT h.SC_NO,t.JOB_ORDER_NO as JO_NO,PROD_COMPLETE_DATE as Ship_Date,sum(Out_QTY) as Ship_Qty,'N' AS LASTJO  
                        from PRD_JO_WIP_HD t (NOLOCK) 
                        INNER JOIN dbo.JO_HD h (NOLOCK) ON t.JOB_ORDER_NO=h.JO_NO
                        inner join PRD_JO_COMPLETE_STATUS a (NOLOCK) ON t.JOB_ORDER_NO=a.JOB_ORDER_NO AND t.PROCESS_CD=a.PROCESS_CD AND t.PROCESS_TYPE=a.PROCESS_TYPE  and PROD_COMPLETE='Y' 
                       
                        where t.PROCESS_CD='FIN' ");
            if (JoList != "")
            {
                SQL.AppendFormat(" and h.JO_NO in ({0}) ", JoList);
            }
            if (factoryCd != "")
            {
                SQL.AppendFormat(" AND h.FACTORY_CD='{0}' ", factoryCd);
            }
            if (GARMENT_TYPE_CD != "")
            {
                SQL.AppendFormat(" AND h.GARMENT_TYPE_CD='{0}' ", GARMENT_TYPE_CD);
            }
            //SQL.Append(@" --and NOT EXISTS (SELECT 1 FROM JO_COMBINE_MAPPING WHERE ORIGINAL_JO_NO=t.JOB_ORDER_NO)   ");
            SQL.Append(@" group by h.SC_NO,t.JOB_ORDER_NO,a.PROD_COMPLETE_DATE");
            return DBUtility.GetTable(SQL.ToString(), "MES");

        }

        //================Common===============================================
        //======================================================================

        //======================================================================
        //================StanderMUReport========================================

        public static DataTable GetStandardOrderQty(DbConnection eelConn)
        {
            string SQL = "         SELECT SC_NO,NVL(SUM(order_qty),0) AS ORDER_QTY FROM ";
            SQL = SQL + "        ( SELECT s.sc_no,SUM (d.order_qty) AS order_qty ";
            SQL = SQL + "        FROM ppo_hd h, ppo_dt d, fab_lib f,         ";
            SQL = SQL + "        (select distinct sc_no,fabric_type_cd,ppO_NO from scx_joi_go_fabric aa  ";
            SQL = SQL + "          where exists (select 1 From PO_HD where sc_no=aa.sc_no and PO_NO in (select f1 from rpt_tmp)";
            SQL = SQL + "            )) s  ";
            SQL = SQL + "        WHERE h.ppO_NO = d.ppO_NO AND  h.flag != 'X'  ";
            SQL = SQL + "        AND d.fab_ref_cd = f.fab_ref_cd AND h.status IN ('L2', 'R')  ";
            SQL = SQL + "        AND H.PpO_NO=s.ppO_NO  and d.fabric_type_cd=s.fabric_type_cd and s.fabric_type_cd in ('B','BD')  ";
            SQL = SQL + "         GROUP BY s.sc_no  ";
            SQL = SQL + "         UNION ALL SELECT s.sc_no,SUM (pl.order_qty) AS order_qty  ";
            SQL = SQL + "         FROM ppo_hd a, ppox_fab_item b,ppox_lot pl, ";
            SQL = SQL + "        (select distinct sc_no,fabric_type_cd,ppO_NO from scx_joi_go_fabric aa  ";
            SQL = SQL + "          where exists (select 1 From PO_HD where sc_no=aa.sc_no and PO_NO in (select f1 from rpt_tmp)";
            SQL = SQL + "          )) s  ";
            SQL = SQL + "         WHERE a.ppO_NO = b.ppO_NO AND b.fab_item_id = pl.fab_item_id AND a.flag = 'X'  ";
            SQL = SQL + "         AND a.status IN ('L2', 'R') AND  b.status != 'C'  ";
            SQL = SQL + "         AND A.PpO_NO=s.ppO_NO and b.fabric_type_cd=s.fabric_type_cd and s.fabric_type_cd in ('B','BD') ";
            SQL = SQL + "          GROUP BY  s.sc_no,b.fabric_type_cd  ";
            SQL = SQL + "          UNION ALL SELECT s.sc_no,sum(d.qty) AS order_qty  ";
            SQL = SQL + "          FROM fab_sample_order_hd a, fab_so_combo b,fab_so_product c,fab_so_combo_qty d, ";
            SQL = SQL + "        (select distinct sc_no,fabric_type_cd,ppO_NO from scx_joi_go_fabric aa  ";
            SQL = SQL + "          where exists (select 1 From PO_HD where sc_no=aa.sc_no and pO_NO in (select f1 from rpt_tmp)";
            SQL = SQL + "          )) s  ";
            SQL = SQL + "          WHERE d.req_prod_cd = c.req_prod_cd and  a.fab_so_no = b.fab_so_no AND b.fab_so_no = d.fab_so_no  ";
            SQL = SQL + "          AND b.combo_seq = d.combo_seq AND a.status IN ('P', 'S', 'D')  ";
            SQL = SQL + "          AND A.fab_so_no=s.ppO_NO and c.fabric_part=s.fabric_type_cd and s.fabric_type_cd in ('B','BD') ";
            SQL = SQL + "         group by a.fab_so_no,s.sc_no     ) AAA  ";
            SQL = SQL + "        GROUP BY SC_NO order by sc_no ";
            return DBUtility.GetTable(SQL, eelConn);
        }

        public static DataTable GetJoOrderQty(DbConnection eelConn)
        {
            string sql = " select PO_NO as JO_NO,round(sum(Total_QTY)/12,2) as OrderQTY,100+max(NVL(PERCENT_OVER_ALLOWED,0)) AS OVERSHIP From sc_lot b where PO_NO in (select f1 from rpt_tmp) group by PO_NO ";
            return DBUtility.GetTable(sql, eelConn);
        }

        public static DataTable GetStandardGoOrderQty(DbConnection eelConn)
        {
            string sql = "select sc_no as sc_NO,round(sum(Total_QTY)/12,2) as OrderQTY From sc_lot b where sc_no IN(SELECT SC_NO FROM PO_HD WHERE PO_NO in (select f1 from rpt_tmp)) and active='Y' group by sc_no ";
            return DBUtility.GetTable(sql, eelConn);
        }

        public static DataTable GetStandardMUReportList(DbConnection invConn)
        {

            string SQL = " select ph.PO_NO,c.Short_name as buyer,ph.SC_NO,ph.WASH_TYPE_CD,(case when d.factory_cd in('GEG','YMG','NBO','CEK','CEG') ";
            SQL = SQL + "THEN D.Style_CHN_DESC ELSE  STYLE_DESC END) as style_desc        ";
            SQL = SQL + " from  PO_hd ph, gen_customer c,sc_hd d         ";
            SQL = SQL + " where ph.customer_cd = c.customer_cd         ";
            SQL = SQL + " and ph.SC_NO = d.SC_NO         ";
            SQL = SQL + " and exists  (select f1 from escmreader.rpt_tmp where f1=ph.PO_NO) ";
            SQL = SQL + " order by ph.PO_NO";
            return DBUtility.GetTable(SQL, invConn);
        }

        public static DataTable GetStandardFromFGIS(string factoryCd, DbConnection invConn)
        {
            //string SQL = " SELECT  job_order_no, SUM(gmt_qty_a) AS gmt_qty_a,SUM(gmt_qty_b) AS gmt_qty_b,SUM(sew_qty_b) AS sew_qty_b,SUM(wash_qty_b) AS wash_qty_b";
            //SQL = SQL + " FROM (";
            //SQL = SQL + " SELECT l.reference_no AS job_order_no,(case when l.grade='A' then sum(nvl(l.qty,0)) else 0 end) as gmt_qty_a, ";
            //SQL = SQL + "(case when l.grade='B' then sum(nvl(l.qty,0)) else 0 end) as gmt_qty_b,  ";
            //SQL = SQL + "(case when l.grade='B' then sum(nvl(l.sew_qty,0)) else 0 end) as sew_qty_b, ";
            //SQL = SQL + "(case when l.grade='B' then sum(nvl(l.wash_qty,0)) else 0 end) as wash_qty_b ";
            //SQL = SQL + "  FROM inventory.inv_trans_hd h, inventory.inv_trans_lines l, inventory.inv_store_codes s, ";
            //SQL = SQL + "       inv_stock_class c ";
            //SQL = SQL + " WHERE h.trans_header_id = l.trans_header_id ";
            //SQL = SQL + "   AND h.from_store_cd = s.store_cd ";
            //SQL = SQL + "   AND s.stock_class_cd = c.stock_class_cd ";
            //SQL = SQL + "   AND NVL(h.first_receipt_flag, 'N') = 'Y' ";
            //SQL = SQL + "   AND c.stock_group_cd = 'L' ";
            //SQL = SQL + "   AND l.grade IN ('A', 'B') ";
            //SQL = SQL + "   AND exists (select f1 from escmreader.rpt_tmp where f1=l.reference_no) ";
            //SQL = SQL + "   and h.doc_no like '" + factoryCd + "%' ";
            //SQL = SQL + "   group by l.reference_no,l.grade ) A";
            //SQL = SQL + " WHERE 1=1 ";
            //SQL = SQL + " GROUP BY job_order_no";
            string SQL = " SELECT  l.reference_no as job_order_no ,SUM(nvl(l.sew_qty, 0)) AS sew_qty_b ,SUM(nvl(l.wash_qty, 0)) AS wash_qty_b ";
            SQL = SQL + "  FROM inventory.inv_trans_hd h, inventory.inv_trans_lines l, inventory.inv_store_codes s, ";
            SQL = SQL + "       inv_stock_class c ";
            SQL = SQL + " WHERE h.trans_header_id = l.trans_header_id ";
            SQL = SQL + "   AND h.from_store_cd = s.store_cd ";
            SQL = SQL + "   AND s.stock_class_cd = c.stock_class_cd ";
            SQL = SQL + "   AND NVL(h.first_receipt_flag, 'N') = 'Y' ";
            SQL = SQL + "   AND c.stock_group_cd = 'L' ";
            SQL = SQL + "   AND l.grade='B'";
            SQL = SQL + "   AND exists (select f1 from escmreader.rpt_tmp where f1=l.reference_no) ";
            SQL = SQL + "   and h.doc_no like '" + factoryCd + "%' ";
            SQL = SQL + "   group by l.reference_no";
         
          
            return DBUtility.GetTable(SQL, invConn);
        }

        public static DataTable GetStandardRTW(string factoryCd, DbConnection invConn)
        {
            string SQL = " SELECT   jo_no, SUM(RATIO*QTY) AS RTW_QTY_I,SUM(QTY) AS RTW_QTY ";
            SQL = SQL + "    FROM (SELECT   LN.job_order_no AS JO_NO, ";
            SQL = SQL + "                   (CASE ";
            SQL = SQL + "                       WHEN po.garment_type_cd = 'K' OR l.grade = 'A' or l.grade is null ";
            SQL = SQL + "                          THEN 1 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade IN ('A*', 'B*') ";
            SQL = SQL + "                          THEN 0.9 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade = 'B' ";
            SQL = SQL + "                          THEN 0.93 ";
            SQL = SQL + "                       ELSE 0 ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                   ) AS ratio, ";
            SQL = SQL + "                   SUM (LN.trans_qty) AS qty ";
            SQL = SQL + "              FROM invsubmat.inv_rec_hd hd, ";
            SQL = SQL + "                   invsubmat.inv_rec_line LN, ";
            SQL = SQL + "                   invsubmat.inv_srn_line sln, ";
            SQL = SQL + "                   invsubmat.inv_issue_line sn, ";
            SQL = SQL + "                   invsubmat.inv_store s, ";
            SQL = SQL + "                   inv_item_lot l, ";
            SQL = SQL + "                   invsubmat.inv_trans_code tc, ";
            SQL = SQL + "                   po_hd po ";
            SQL = SQL + "             WHERE hd.rec_hd_id = LN.rec_hd_id ";
            SQL = SQL + "               AND hd.store_cd = s.store_cd ";
            SQL = SQL + "               AND LN.lot_id = l.lot_id(+) ";
            SQL = SQL + "               AND exists  (select f1 from escmreader.rpt_tmp where f1=LN.job_order_no) ";
            SQL = SQL + "               AND sn.srn_line_id = sln.srn_line_id ";
            SQL = SQL + "               AND LN.issue_line_id = sn.issue_line_id ";
            SQL = SQL + "               AND s.inv_fty_cd = '" + factoryCd + "' ";
            SQL = SQL + "               AND hd.status = 'F' ";
            SQL = SQL + "               AND hd.item_type_cd = 'F' ";
            SQL = SQL + "               AND hd.trans_cd = tc.trans_cd ";
            SQL = SQL + "               AND tc.trans_type_cd = 'RTW' ";
            if (factoryCd == "GEG" || factoryCd == "GLG" || factoryCd == "YMG")
            {
                SQL = SQL + "               AND sln.main_body_flag = 'Y' ";
            }
            //SQL = SQL + "               AND sln.main_body_flag = 'Y' ";
            SQL = SQL + "               AND po.PO_NO = LN.job_order_no ";
            SQL += " and s.warehouse_type_cd<>'C'";
            SQL += "  and l.fabric_type in('B','A','E','D','BD') "; 
            //SQL = SQL + " AND (NVL(s.warehouse_type_cd,'O')<>'C' or (NVL(s.warehouse_type_cd,'O')='C' and  s.store_cd like '%C2'))";
            SQL = SQL + "          GROUP BY LN.job_order_no, l.grade, po.garment_type_cd) ";
            SQL = SQL + "GROUP BY jo_no ";
            return DBUtility.GetTable(SQL, invConn);
        }



        public static DataTable GetStandardSRN(string factoryCd, DbConnection invConn)
        {
            string SQL = " SELECT   jo_no, SUM(RATIO*QTY) AS SRN_QTY_I,SUM(QTY) AS SRN_QTY ";
            SQL = SQL + "    FROM (SELECT   LN.job_order_no AS JO_NO, ";
            SQL = SQL + "                   (CASE ";
            SQL = SQL + "                       WHEN po.garment_type_cd = 'K' OR l.grade = 'A' or l.grade is null ";
            SQL = SQL + "                          THEN 1 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade IN ('A*', 'B*') ";
            SQL = SQL + "                          THEN 0.9 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade = 'B' ";
            SQL = SQL + "                          THEN 0.93 ";
            SQL = SQL + "                       ELSE 0 ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                   ) AS ratio, ";
            SQL = SQL + "                   SUM (LN.trans_qty) AS qty ";
            SQL = SQL + "              FROM invsubmat.inv_issue_hd hd, ";
            SQL = SQL + "                   invsubmat.inv_issue_line LN, ";
            SQL = SQL + "                   invsubmat.inv_srn_line sln, ";
            SQL = SQL + "                   invsubmat.inv_store s, ";
            SQL = SQL + "                   inv_item_lot l, ";
            SQL = SQL + "                   po_hd po, ";
            SQL = SQL + "                   (select distinct f1 AS job_order_no from escmreader.rpt_tmp ) E";
            SQL = SQL + "             WHERE hd.issue_hd_id = LN.issue_hd_id ";
            SQL = SQL + "               AND LN.store_cd = s.store_cd ";
            SQL = SQL + "               AND LN.lot_id = l.lot_id(+) ";
            SQL = SQL + "               AND LN.srn_line_id = sln.srn_line_id ";
            SQL = SQL + "               AND E.job_order_no= LN.job_order_no ";
            SQL = SQL + "               AND exists (select f1 from escmreader.rpt_tmp where f1= LN.job_order_no) ";
            SQL = SQL + "               AND sln.job_order_no = LN.job_order_no ";
            SQL = SQL + "               AND s.inv_fty_cd = '" + factoryCd + "' ";
            SQL = SQL + "               AND hd.status = 'F' ";
            SQL = SQL + "               AND hd.item_type_cd = 'F' ";
            if (factoryCd == "GEG" || factoryCd == "GLG" || factoryCd == "YMG")
            {
                SQL = SQL + "               AND sln.main_body_flag = 'Y' ";
            }

            //SQL = SQL + "               AND sln.main_body_flag = 'Y' ";
            SQL = SQL + "               AND NVL(HD.PROD_LINE_CD,'OK')<>'YMG-SAMPLE(Special)'";
            SQL = SQL + "               AND po.PO_NO = LN.job_order_no ";
            SQL += " and NVL(s.warehouse_type_cd,'O')<>'C'";
            SQL += "  and l.fabric_type in('B','A','E','D','BD') and ((po.garment_type_cd = 'K' and hd.TRANS_CD='ITPK')or(po.garment_type_cd = 'W' and hd.TRANS_CD='ITPW'))"; 
            //SQL = SQL + " AND (NVL(s.warehouse_type_cd,'O')<>'C' or (NVL(s.warehouse_type_cd,'O')='C' and  s.store_cd like '%C2'))";
            SQL = SQL + "          GROUP BY LN.job_order_no, l.grade, po.garment_type_cd) ";
            SQL = SQL + "GROUP BY jo_no ";
            return DBUtility.GetTable(SQL, invConn);
        }

        public static DataTable GetStatudardGRN(string factoryCd, DbConnection invConn)
        {
            StringBuilder SQL = new StringBuilder();
            SQL.Append("SELECT jo_no,sum(SRN_QTY_I) as SRN_QTY_I,sum(SRN_QTY) as SRN_QTY,sum(RTW_QTY_I) as RTW_QTY_I,sum(RTW_QTY) as RTW_QTY,sum(sew_qty_b) as sew_qty_b,sum(wash_qty_b) as wash_qty_b from(");
            SQL.AppendFormat(@" SELECT   jo_no, SUM(RATIO*QTY) AS SRN_QTY_I,SUM(QTY) AS SRN_QTY,0 AS RTW_QTY_I,0 AS RTW_QTY,0 AS sew_qty_b,0 AS wash_qty_b FROM 
                                (SELECT   LN.job_order_no AS JO_NO, 
                                (CASE WHEN po.garment_type_cd = 'K'
                                                          OR l.grade = 'A'
                                                          OR l.grade IS NULL THEN 1
                                    ELSE CASE WHEN l.grade IN ( 'A*', 'B*' ) THEN 0.9
                                            ELSE CASE WHEN l.grade = 'B' THEN 0.93
                                                        ELSE 0
                                                END
                                        END
                               END) AS ratio,SUM (LN.trans_qty) AS qty  from
                                   invsubmat.inv_issue_hd hd, 
                                   invsubmat.inv_issue_line LN, 
                                   invsubmat.inv_srn_line sln,
                                   invsubmat.inv_store s, 
                                   inv_item_lot l,
                                   po_hd po,
                                   (select distinct f1 AS job_order_no from escmreader.rpt_tmp ) E
                                   where hd.issue_hd_id = LN.issue_hd_id and LN.store_cd = s.store_cd 
                                   AND LN.lot_id = l.lot_id(+) AND LN.srn_line_id = sln.srn_line_id
                                   AND E.job_order_no= LN.job_order_no
                                   AND exists (select f1 from escmreader.rpt_tmp where f1= LN.job_order_no) 
                                   AND sln.job_order_no = LN.job_order_no
                                   AND s.inv_fty_cd = '{0}'
                                   AND hd.status = 'F' 
                                   {1} 
                                   AND NVL(HD.PROD_LINE_CD,'OK')<>'YMG-SAMPLE(Special)'
                                   AND po.PO_NO = LN.job_order_no
                                   and NVL(s.warehouse_type_cd,'O')<>'C'
                                   and l.fabric_type in('B','A','E','D','BD') and ((po.garment_type_cd = 'K' and hd.TRANS_CD='ITPK')or(po.garment_type_cd = 'W' and hd.TRANS_CD='ITPW'))
                                   GROUP BY LN.job_order_no, l.grade, po.garment_type_cd) 
                                   GROUP BY jo_no
                           ", factoryCd, ((factoryCd == "GEG" || factoryCd == "GLG" || factoryCd == "YMG") ? " AND sln.main_body_flag = 'Y' " : ""));
            SQL.Append(" UNION ALL ");
            SQL.AppendFormat(@" SELECT jo_no,0 as SRN_QTY_I,0 as SRN_QTY ,SUM(RATIO * QTY) AS RTW_QTY_I ,SUM(QTY) AS RTW_QTY,0 as sew_qty_b,0 as wash_qty_b
                                FROM   ( SELECT    LN.job_order_no AS JO_NO ,
                                            ( CASE WHEN po.garment_type_cd = 'K'
                                                        OR l.grade = 'A'
                                                        OR l.grade IS NULL THEN 1
                                                   ELSE CASE WHEN l.grade IN ( 'A*', 'B*' ) THEN 0.9
                                                             ELSE CASE WHEN l.grade = 'B' THEN 0.93
                                                                       ELSE 0
                                                                  END
                                                        END
                                              END ) AS ratio ,
                                            SUM(LN.trans_qty) AS qty
                                  FROM      invsubmat.inv_rec_hd hd ,
                                            invsubmat.inv_rec_line LN ,
                                            invsubmat.inv_srn_line sln ,
                                            invsubmat.inv_issue_line sn ,
                                            invsubmat.inv_store s ,
                                            inv_item_lot l ,
                                            invsubmat.inv_trans_code tc ,
                                            po_hd po
                                  WHERE     hd.rec_hd_id = LN.rec_hd_id
                                            AND hd.store_cd = s.store_cd
                                            AND LN.lot_id = l.lot_id(+)
                                            AND EXISTS ( SELECT f1
                                                         FROM   escmreader.rpt_tmp
                                                         WHERE  f1 = LN.job_order_no )
                                            AND sn.srn_line_id = sln.srn_line_id
                                            AND LN.issue_line_id = sn.issue_line_id
                                            AND s.inv_fty_cd = '{0}'
                                            AND hd.status = 'F'
                                            AND hd.item_type_cd = 'F'
                                            AND hd.trans_cd = tc.trans_cd
                                            AND tc.trans_type_cd = 'RTW'
                                            AND po.PO_NO = LN.job_order_no
                                            AND s.warehouse_type_cd <> 'C'
                                            AND l.fabric_type IN ( 'B', 'A', 'E', 'D', 'BD' )
                                  GROUP BY  LN.job_order_no ,
                                            l.grade ,
                                            po.garment_type_cd
                                )
                         GROUP BY jo_no ", factoryCd, ((factoryCd == "GEG" || factoryCd == "GLG" || factoryCd == "YMG") ? " AND sln.main_body_flag = 'Y' " : ""));
            SQL.Append(" UNION ALL ");
            SQL.AppendFormat(@" SELECT l.reference_no AS job_order_no,0 as SRN_QTY_I,0 as SRN_QTY,0 AS RTW_QTY_I,0 AS RTW_QTY ,SUM(nvl(l.sew_qty, 0)) AS sew_qty_b ,SUM(nvl(l.wash_qty, 0)) AS wash_qty_b
                                 FROM   inventory.inv_trans_hd h ,
                                        inventory.inv_trans_lines l ,
                                        inventory.inv_store_codes s ,
                                        inv_stock_class c
                                 WHERE  h.trans_header_id = l.trans_header_id
                                        AND h.from_store_cd = s.store_cd
                                        AND s.stock_class_cd = c.stock_class_cd
                                        AND NVL(h.first_receipt_flag, 'N') = 'Y'
                                        AND c.stock_group_cd = 'L'
                                        AND l.grade = 'B'
                                        AND EXISTS ( SELECT f1
                                                     FROM   escmreader.rpt_tmp
                                                     WHERE  f1 = l.reference_no )
                                        AND h.doc_no LIKE '{0}%'
                                 GROUP BY l.reference_no", factoryCd);
            SQL.Append(")AA group by jo_no");
            return DBUtility.GetTable(SQL.ToString(), invConn);
           
         
          
        }

        public static DataTable GetStatudardLeftoverAB(string factoryCd, string JoList)
        {
            StringBuilder SQL = new StringBuilder();
            SQL.AppendFormat(@" SELECT b.JOB_ORDER_NO as JO_NO,sum(case when b.GRADE_CD='A' then QTY else 0 end) as gmt_qty_a,
			                sum(case when b.GRADE_CD='B' then QTY else 0 end) as gmt_qty_b
			               
			                  FROM PRD_GARMENT_TRANSFER_HD a
			                INNER JOIN dbo.PRD_GARMENT_TRANSFER_DFT b ON a.DOC_NO=b.DOC_NO
			                WHERE a.PROCESS_CD='FIN' AND NEXT_PROCESS_CD='WHS' AND 
			                REASON_CD<>'' AND REASON_CD IS NOT NULL 
			                AND GRADE_CD IN ('A','B')   AND a.FACTORY_CD='{0}' AND b.JOB_ORDER_NO IN({1})
			                GROUP BY b.JOB_ORDER_NO
                          ", factoryCd, JoList);
            return DBUtility.GetTable(SQL.ToString(), "MES");
        }

        public static DataTable GetStatudardLeftoverAB(string factoryCd, DbConnection invConn)
        {
            string SQL ="";
            SQL = SQL + " SELECT  jo_no,";
            SQL = SQL + "SUM(gmt_qty_a) AS gmt_qty_a,SUM(gmt_qty_b) AS gmt_qty_b";
            SQL = SQL + " FROM (";
            SQL = SQL + " SELECT l.reference_no AS jo_no,(case when l.grade='A' then sum(nvl(l.qty,0)) else 0 end) as gmt_qty_a, ";
            SQL = SQL + "(case when l.grade='B' then sum(nvl(l.qty,0)) else 0 end) as gmt_qty_b,  ";
            SQL = SQL + "(case when l.grade='B' then sum(nvl(l.sew_qty,0)) else 0 end) as sew_qty_b, ";
            SQL = SQL + "(case when l.grade='B' then sum(nvl(l.wash_qty,0)) else 0 end) as wash_qty_b ";
            SQL = SQL + "  FROM inventory.inv_trans_hd h, inventory.inv_trans_lines l, inventory.inv_store_codes s, ";
            SQL = SQL + "       inventory.inv_stock_class c ";
            SQL = SQL + " WHERE h.trans_header_id = l.trans_header_id ";
            SQL = SQL + "   AND h.from_store_cd = s.store_cd ";
            SQL = SQL + "   AND s.stock_class_cd = c.stock_class_cd ";
            SQL = SQL + "   AND NVL(h.first_receipt_flag, 'N') = 'Y' ";
            SQL = SQL + "   AND c.stock_group_cd = 'L' and s.STOCK_CLASS_CD in  ('L11','L12')  ";
            SQL = SQL + "   AND l.grade IN ('A', 'B') ";
            SQL = SQL + "   AND exists (select f1 from escmreader.rpt_tmp where f1=l.reference_no) ";
            SQL = SQL + "   and h.doc_no like '" + factoryCd + "%' ";
            SQL = SQL + "   group by l.reference_no,l.grade ) A  group by jo_no";
            return DBUtility.GetTable(SQL, invConn);
        }

        public static DataTable GetStandardFromMES(string factoryCd, string JoList)
        {
            string SQL = "";
            SQL += @"    if   object_id( 'tempdb..#TEMP_PULLOUT')   is   not   null  drop   table   #TEMP_PULLOUT;  
                                if   object_id( 'tempdb..#TEMP_PULLIN')   is   not   null  drop   table   #TEMP_PULLIN;  
                                if   object_id( 'tempdb..#TEMP_CUT')   is   not   null  drop   table   #TEMP_CUT; 
                                if   object_id( 'tempdb..#TEMP')   is   not   null  drop   table   #TEMP; 
                                IF OBJECT_ID('tempdb..#TEMP_GMTQTY') IS   NOT   NULL 
                                    DROP   TABLE   #TEMP_GMTQTY;  
                                 ";

            //PULLOUT
            SQL += @"    SELECT JOB_ORDER_NO, '' AS CUT_DATE, 0 AS cutqty
                                ,0 AS PULLIN_QTY,SUM(isnull(PULLOUT_QTY,0)) as PULLOUT_QTY,
                                SUM(isnull(DISCREPANCY_QTY,0)) as DISCREPANCY_QTY,
                                SUM(ISNULL(SAMPLE_QTY,0)) AS SAMPLE_QTY,SUM(ISNULL(SEW_WASTAGE,0)) AS SEW_WASTAGE
                                ,SUM(ISNULL(WASH_WASTAGE,0)) AS WASH_WASTAGE  ,'PULLOUT' AS REMARK
                                INTO #TEMP_PULLOUT
                                FROM  
                                (select B.JOB_ORDER_NO,cast(round(SUM (isnull(C.PULLOUT_QTY, 0)/12.00),2) as decimal(18,2)) AS PULLOUT_QTY,(CASE WHEN RC.QTY_AFFECTION='D' THEN cast(round(SUM(isnull(RS.PULLOUT_QTY, 0)/12),2) as decimal(18,2)) ELSE 0 END) AS DISCREPANCY_QTY,(CASE WHEN RC.QTY_AFFECTION='A' THEN cast(round(SUM(isnull(RS.PULLOUT_QTY, 0)/12),2) as decimal(18,2)) ELSE 0 END) AS SAMPLE_QTY, (CASE WHEN RC.QTY_AFFECTION='D' AND A.PROCESS_CD LIKE '%SEW%' THEN cast(round(SUM(isnull(RS.PULLOUT_QTY, 0)/12),2) as decimal(18,2)) ELSE 0 END) AS SEW_WASTAGE, 
                                (CASE WHEN RC.QTY_AFFECTION='D' AND A.PROCESS_CD IN ('WET') THEN cast(round(SUM(isnull(RS.PULLOUT_QTY, 0)/12),2) as decimal(18,2)) ELSE 0 END) AS WASH_WASTAGE  
                                from PRD_JO_DISCREPANCY_PULLOUT_HD a   
                                JOIN PRD_JO_DISCREPANCY_PULLOUT_TRX b   
                                ON A.DOC_NO=B.DOC_NO   
                                AND A.FACTORY_CD=B.FACTORY_CD   
                                AND A.TRX_DATE=B.TRX_DATE 
                                LEFT JOIN PRD_JO_PULLOUT_REASON RS 
                                ON RS.TRX_ID=B.TRX_ID 
                                AND RS.FACTORY_CD=B.FACTORY_CD 
                                LEFT JOIN PRD_REASON_CODE RC 
                                ON RC.REASON_CD=RS.REASON_CD 
                                AND RC.FACTORY_CD=RS.FACTORY_CD   
                                and  (RC.REASON_DESC not like '%不见衫%' and RC.REASON_DESC not like '%missing%') 
                                JOIN JO_HD PO 
                                ON B.JOB_ORDER_NO=PO.JO_NO                  
                                left JOIN PRD_JO_PULLOUT C 
                                ON C.TRX_ID=B.TRX_ID 
                                AND C.FACTORY_CD=A.FACTORY_CD 
                                WHERE job_order_no  in (" + JoList + ")     ";
            SQL += @"  GROUP BY B.JOB_ORDER_NO,RC.QTY_AFFECTION,A.PROCESS_CD)AA WHERE 1=1 GROUP BY JOB_ORDER_NO; ";

            //PULLIN
            //Add Factory Code checking by Jin Song 2015-02-10 (Bug fix)
            SQL += @"    SELECT JO_NO AS JOB_ORDER_NO, '' AS CUT_DATE, 0 AS cutqty                                
                                ,cast(round(SUM (isnull(PULLOUT_QTY, 0)/12.00),2) as decimal(18,2)) AS PULLIN_QTY ,
                                 0 AS PULLOUT_QTY,
                                 0 AS DISCREPANCY_QTY,
                                 0 AS SAMPLE_QTY,
                                 0 AS SEW_WASTAGE_C,
                                 0 AS WASH_WASTAGE_C  ,
                                'PULLIN' AS REMARK
                                INTO #TEMP_PULLIN
                                fROM PRD_JO_PULLOUT WHERE JO_NO in (" + JoList + ")     ";
            SQL += @"  and  exists (select trx_id from PRD_JO_DISCREPANCY_PULLOUT_TRX AS A INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_HD AS B ON A.DOC_NO=B.DOC_NO AND A.FACTORY_CD=B.FACTORY_CD and B.TRX_DATE = A.TRX_DATE where trx_id =PRD_JO_PULLOUT.trx_id and A.FACTORY_CD='" + factoryCd + "') GROUP BY JO_NO;";
            //CUT
            SQL += @"    SELECT A.JOB_ORDER_NO,convert(char(10),MIN(A.CUT_DATE),23) AS CUT_DATE
                                ,cast(round(SUM (isnull((A.CUT_QTY-ISNULL(B.REDUCE_QTY,0)), 0)/12),2) as decimal(18,2)) AS cutqty ,
                                 0 AS PULLIN_QTY  ,
                                 0 AS PULLOUT_QTY,
                                 0 AS DISCREPANCY_QTY,
                                 0 AS SAMPLE_QTY,
                                 0 AS SEW_WASTAGE_C,
                                 0 AS WASH_WASTAGE_C,
                                'CUT' AS REMARK
                                INTO #TEMP_CUT
                                FROM 
                                (SELECT JOB_ORDER_NO,MIN([ACTUAL_PRINT_DATE]) AS CUT_DATE,SUM(QTY) AS CUT_QTY 
                                fROM CUT_BUNDLE_HD WHERE JOB_ORDER_NO in (" + JoList + ") ";
            SQL += @"  AND trx_type='NM' AND STATUS='Y' 
                                AND FACTORY_CD='" + factoryCd + "' ";
            SQL += @"  GROUP BY JOB_ORDER_NO) A 
                                LEFT JOIN  
                                (select HD.JOB_ORDER_NO,SUM(REDUCE_QTY) AS REDUCE_QTY 
                                FROM CUT_BUNDLE_REDUCE_HD HD,CUT_BUNDLE_REDUCE_TRX DT 
                                WHERE HD.DOC_NO=DT.DOC_NO 
                                AND HD.FACTORY_CD=DT.FACTORY_CD 
                                AND HD.FACTORY_CD='" + factoryCd + "' ";
            SQL += @"  AND HD.PROCESS_CD LIKE '%CUT%' 
                                AND HD.JOB_ORDER_NO in (" + JoList + ") ";
            SQL += @"  AND HD.STATUS='C' 
                                GROUP BY HD.JOB_ORDER_NO
                                ) B 
                                ON A.JOB_ORDER_NO=B.JOB_ORDER_NO 
                                WHERE 1=1 
                                GROUP BY A.JOB_ORDER_NO ;";
            //gmt_qty  for ming
            SQL += @"          SELECT b.JOB_ORDER_NO,sum(case when b.GRADE_CD='A' then QTY else 0 end) as gmt_qty_a,
			                sum(case when b.GRADE_CD='B' then QTY else 0 end) as gmt_qty_b
			                INTO #TEMP_GMTQTY
			                  FROM PRD_GARMENT_TRANSFER_HD a
			                INNER JOIN dbo.PRD_GARMENT_TRANSFER_DFT b ON a.DOC_NO=b.DOC_NO
			                WHERE a.PROCESS_CD='FIN' AND NEXT_PROCESS_CD='WHS' AND 
			                REASON_CD<>'' AND REASON_CD IS NOT NULL 
			                AND GRADE_CD IN ('A','B') 
			                GROUP BY b.JOB_ORDER_NO";
            //UNION
            SQL += @"    SELECT * INTO #TEMP FROM(
                                SELECT *,0 as gmt_qty_a,0 as gmt_qty_b  FROM #TEMP_CUT A
                                UNION ALL
                                SELECT *,0 as gmt_qty_a,0 as gmt_qty_b FROM #TEMP_PULLIN B
                                UNION ALL
                                SELECT *,0 as gmt_qty_a,0 as gmt_qty_b FROM #TEMP_PULLOUT C
                                UNION ALL
                                SELECT JOB_ORDER_NO, '' AS CUT_DATE, 0 AS cutqty
                                ,0 AS PULLIN_QTY,0 as PULLOUT_QTY,
                                0 as DISCREPANCY_QTY,
                                0 AS SAMPLE_QTY,0 AS SEW_WASTAGE
                                ,0 AS WASH_WASTAGE  ,'GMT_QTY' AS REMARK,gmt_qty_a,gmt_qty_b FROM #TEMP_GMTQTY C
                                )AS T;";

            SQL += @"    SELECT JOB_ORDER_NO,MAX(CUT_DATE) AS CUT_DATE
                                ,SUM(isnull(cutqty,0)) as cutqty,SUM(isnull(PULLIN_QTY,0)-isnull(PULLOUT_QTY,0)) AS PULLOUT_QTY  ,
                                SUM(isnull(DISCREPANCY_QTY,0)) as DISCREPANCY_QTY,
                                SUM(ISNULL(SAMPLE_QTY,0)) AS SAMPLE_QTY,
                                 SUM(ISNULL(SEW_WASTAGE_C,0)) AS SEW_WASTAGE_C,
                                SUM(ISNULL(WASH_WASTAGE_C,0)) AS WASH_WASTAGE_C,sum(gmt_qty_a) as gmt_qty_a,sum(gmt_qty_b) as gmt_qty_b   FROM
                                #TEMP 
                                GROUP BY JOB_ORDER_NO";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetStandardMUCutInfo(string JoList, string factoryCd, string fromDate, string toDate)
        {
            //string SQL = "  SELECT JOB_ORDER_NO,ROUND(SUM(BULK_MARKER_NET_YPD_NEW*ISNULL(BULK_QTY,1))/SUM(ISNULL(BULK_QTY,1)),2) AS BULK_NET_YPD, ";
            //SQL = SQL + " ROUND(SUM(BULK_MARKER_NET_YPD_NEW*ISNULL(BULK_QTY,1))/SUM(ISNULL(BULK_QTY,1))*(1+W.WASTAGE/100),2) AS BULK_MARKER_YPD, ";
            //SQL = SQL + " ROUND(SUM(MU_PERCENT_NEW*ISNULL(BULK_QTY,1))/SUM(ISNULL(BULK_QTY,1)),2) AS MU ";
            //SQL = SQL + ",ROUND(avg(isnull(PPO_NET_YPD_New,0)),2) as PPO_NET_YPD ";
            string SQL = @"  SELECT    JOB_ORDER_NO ,
                        CASE SUM(ISNULL(BULK_QTY, 1))
                          WHEN 0 THEN 0
                          ELSE ROUND(SUM(BULK_MARKER_NET_YPD_NEW * ISNULL(BULK_QTY, 1))
                                     / SUM(ISNULL(BULK_QTY, 1)), 2)
                        END AS BULK_NET_YPD ,
                        CASE SUM(ISNULL(BULK_QTY, 1)) * ( 1 + W.WASTAGE / 100 )
                          WHEN 0 THEN 0
                          ELSE ROUND(SUM(BULK_MARKER_NET_YPD_NEW * ISNULL(BULK_QTY, 1))
                                     / SUM(ISNULL(BULK_QTY, 1)) * ( 1 + W.WASTAGE / 100 ),
                                     2)
                        END AS BULK_MARKER_YPD ,
                        CASE SUM(ISNULL(BULK_QTY, 1))
                          WHEN 0 THEN 0
                          ELSE ROUND(SUM(MU_PERCENT_NEW * ISNULL(BULK_QTY, 1))
                                     / SUM(ISNULL(BULK_QTY, 1)), 2)
                        END AS MU ,
                        ROUND(AVG(ISNULL(PPO_NET_YPD_New, 0)), 2) AS PPO_NET_YPD ";
            //Added by ZouShCh ON 2013.07.29 Start
            //SQL = SQL + " fROM CP_JO_YPD YPD,(select garment_type_cd,PATTERN_TYPE_CD=(case when PATTERN_TYPE_CD='PC' then 'Check' else ";
            //SQL = SQL + " case when PATTERN_TYPE_CD='ST' then 'Stripe' else 'Solid' end end),QTY_RANGE_FROM, ";            
            //SQL = SQL + "QTY_RANGE_TO,(WASTAGE+isnull(SEW_WASTAGE,0)) AS WASTAGE ";
            //SQL = SQL + "From dbo.GEN_CUT_SEW_WASTAGE A ";

            SQL = SQL + " From CP_JO_YPD YPD,(SELECT A.GARMENT_TYPE_CD,PAT.PATTERN_TYPE_DESC AS PATTERN_TYPE_CD,A.QTY_RANGE_FROM, ";
            SQL = SQL + "A.QTY_RANGE_TO,(A.WASTAGE+isnull(A.SEW_WASTAGE,0)) AS WASTAGE ";
            SQL = SQL + "From dbo.GEN_CUT_SEW_WASTAGE AS A INNER JOIN dbo.GEN_FAB_PATTERN_TYPE AS PAT ON A.PATTERN_TYPE_CD = PAT.PATTERN_TYPE_CD ";
            //Added by ZouShCh ON 2013.07.29 END
            SQL = SQL + "WHERE a.factory_cd='" + factoryCd + "') W,(select C.jo_no,B.garment_type_cd,ISNULL(B.FAB_PATTERN,'Stripe') AS FAB_PATTERN,sum(qty) as order_qty from ";
            SQL = SQL + "jo_dt c ,jo_hd B where c.jo_no=b.jo_no  ";
            SQL = SQL + "AND B.jo_no IN (" + JoList + ") ";
            SQL = SQL + "GROUP BY C.JO_NO,B.garment_type_cd,B.FAB_PATTERN) JO ";
            SQL = SQL + " WHERE FACTORY_CD='" + factoryCd + "' ";
            SQL = SQL + " AND JOB_ORDER_NO IN(" + JoList + ") ";
            SQL = SQL + " AND YPD.JOB_ORDER_NO=JO.JO_NO ";
            SQL = SQL + "AND W.garment_type_cd=JO.garment_type_cd ";
            SQL = SQL + "AND UPPER(W.PATTERN_TYPE_CD)=UPPER(JO.FAB_PATTERN) ";
            SQL = SQL + "AND JO.order_qty>=W.QTY_RANGE_FROM ";
            SQL = SQL + " GROUP BY JOB_ORDER_NO,W.WASTAGE ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetStandardGivenCutAllowance(string JoList, string factoryCd)
        {
            string SQL = " SELECT D.JO_NO,(W.WASTAGE/100) AS Given_Cut_Allowance ";
            SQL = SQL + "FROM   ";

            SQL = SQL + " (select A.garment_type_cd,PAT.PATTERN_TYPE_DESC AS PATTERN_TYPE_CD,A.QTY_RANGE_FROM, ";
            SQL = SQL + "A.QTY_RANGE_TO,A.WASTAGE ";
            SQL = SQL + "From dbo.GEN_CUT_SEW_WASTAGE A INNER JOIN dbo.GEN_FAB_PATTERN_TYPE AS PAT ON A.PATTERN_TYPE_CD = PAT.PATTERN_TYPE_CD ";

            SQL = SQL + "WHERE a.factory_cd='" + factoryCd + "') W, (select C.jo_no,B.garment_type_cd,ISNULL(B.FAB_PATTERN,'Stripe') AS FAB_PATTERN,sum(qty) as order_qty from ";
            SQL = SQL + "jo_dt c ,jo_hd B where c.jo_no=b.jo_no  ";
            SQL = SQL + "AND B.jo_no in (" + JoList + ") ";
            SQL = SQL + "GROUP BY C.JO_NO,B.garment_type_cd,B.FAB_PATTERN) d ";
            SQL = SQL + "where W.garment_type_cd=D.garment_type_cd ";
            SQL = SQL + "AND UPPER(W.PATTERN_TYPE_CD)=UPPER(D.FAB_PATTERN) ";
            SQL = SQL + "AND D.order_qty>=W.QTY_RANGE_FROM ";
            SQL = SQL + "AND D.order_qty<=W.QTY_RANGE_TO ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetStandardRemnant(string JoNo)
        {
            string SQL = " SELECT SUM(REMNANT_YARDS) as REMNANT FROM PRD_FABRIC_REMNANT WHERE JOB_ORDER_NO='" + JoNo + "'";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetStandardRemnantByJoList(string JoNoList)
        {
            string SQL = " SELECT JOB_ORDER_NO,SUM(REMNANT_YARDS) as VALUE FROM PRD_FABRIC_REMNANT WHERE JOB_ORDER_NO IN(" + JoNoList + ") GROUP BY JOB_ORDER_NO";
            return DBUtility.GetTable(SQL, "MES");
        }


        public static DataTable GetPO_TTL_SC_QTY(string JoNo, string factoryCd)
        {
            string SQL = " SELECT Round(SUM(SC.TOTAL_QTY)/12,2) AS PO_TTL_SC_QTY ";
            SQL = SQL + "FROM ";
            SQL = SQL + "(select DISTINCT SC_NO ";
            SQL = SQL + "from scx_joi_go_fabric a ";
            SQL = SQL + "where a.pPO_NO in(select pPO_NO from scx_joi_go_fabric A,PO_HD B where A.sc_no=B.sc_no AND B.JO_NO IN('" + JoNo + "') AND FABRIC_TYPE_CD in ('B','BD')) ";
            SQL = SQL + "AND A.FABRIC_TYPE_CD in ('B','BD')) PS,SC_HD SC ";
            SQL = SQL + "WHERE PS.SC_NO=SC.SC_NO and SC.factory_cd='" + factoryCd + "'";
            return DBUtility.GetTable(SQL, "EEL");
        }

        public static DataTable Get_TTL_SC_QTY_BY_PO_LIST(string JoNoList, string factoryCd)
        {

            DbConnection connection = MESComment.DBUtility.GetConnection("EEL");

            MESComment.DBUtility.InsertRptTempData(JoNoList, connection);


            string SQL = @"  select E.PO_NO AS  JOB_ORDER_NO,Round(SUM(SC.TOTAL_QTY)/12,2) AS VALUE                 
                FROM 
                (select distinct C.SC_NO,D.pO_NO
                from scx_joi_go_fabric C INNER JOIN 
             (select distinct  B. pO_NO,A.ppO_NO   from   
                 scx_joi_go_fabric a inner join PO_HD b on A.sc_no=B.sc_no 
                AND EXISTS(select 1 from escmreader.rpt_tmp where f1=  B.PO_NO)                    
                AND FABRIC_TYPE_CD in ('B','BD')) D
                 ON C.PpO_NO=D.ppO_NO
                 WHERE  C.FABRIC_TYPE_CD in ('B','BD')) E                 
                 INNER JOIN SC_HD SC ON SC.SC_NO=E.SC_NO                 
                 and SC.factory_cd='" + factoryCd + @"' 
                 GROUP BY E.pO_NO ";



            return DBUtility.GetTable(SQL, connection);
        }

        public static DataTable GetStandardLeftOver(string factoryCd, DbConnection invConn)
        {
            StringBuilder SQL = new StringBuilder();
            SQL.AppendFormat(@" SELECT PO_NO AS PPO_NO ,
                                reason_cd,
                                SUM(RATIO * Leftover_QTY) AS Leftover_QTY ,
                                SUM(Leftover_QTY) AS Leftover_QTY1
                         FROM   ( SELECT    lot.PO_NO ,
                                            lot.reason_cd ,
                                            ( CASE WHEN ppo.FABRIC_NATURE_CD = 'K'
                                                        OR lot.grade = 'A' THEN 1
                                                   ELSE CASE WHEN lot.grade IN ( 'A*', 'B*' ) THEN 0.9
                                                             ELSE CASE WHEN lot.grade = 'B' THEN 0.93
                                                                       ELSE 0
                                                                  END
                                                        END
                                              END ) AS ratio ,
                                            SUM(nvl(on_hand_qty, 0)) AS Leftover_QTY
                                  FROM      INVSUBMAT.inv_stock_items_v a ,
                                            inv_item i ,
                                            inv_item_lot lot ,
                                            INVSUBMAT.inv_store s ,
                                            PPO_HD PPO
                                  WHERE     a.inv_item_id = i.inv_item_id
                                            AND a.lot_id = lot.lot_id(+) 
                                            AND a.on_hand_qty > 0
                                            AND a.store_cd = s.store_cd
                                            AND i.item_type_cd = 'F'
                                            AND EXISTS ( SELECT 1
                                                         FROM   escmreader.rpt_tmp
                                                         WHERE  f1 = lot.PO_NO )
                                            AND s.stock_class_cd IN ( 'L1', 'L3', 'L4' )
                                            AND a.inv_fty_cd = '{0}'
                                            AND s.active = 'Y'
                                            AND fabric_type in('B','BD')
                                            AND PPO.PPO_NO = LOT.PO_NO
                                  GROUP BY  lot.PO_NO ,
                                            lot.reason_cd ,
                                            lot.grade ,
                                            ppo.FABRIC_NATURE_CD
                                )
                         GROUP BY PO_NO,reason_cd ", factoryCd);
            return DBUtility.GetTable(SQL.ToString(), invConn);
        }

        public static DataTable GetStandardLeftOver(string PPOList, string factoryCd)
        {

            DbConnection connection = MESComment.DBUtility.GetConnection("INV");

            MESComment.DBUtility.InsertRptTempData(PPOList, connection);

            string SQL = " select PO_NO as PPO_NO,SUM(RATIO*Leftover_QTY) AS Leftover_QTY,SUM(Leftover_QTY) AS Leftover_QTY1 ";
            SQL = SQL + "from ";
            SQL = SQL + "(SELECT   lot.PO_NO, ";
            SQL = SQL + " (case when ppo.FABRIC_NATURE_CD='K' or lot.grade='A' then 1 else ";
            SQL = SQL + "         case when lot.grade in('A*','B*') THEN 0.9 else ";
            SQL = SQL + "         case when lot.grade ='B' then 0.93 else 0 end end end) as ratio, ";
            SQL = SQL + "sum(nvl(on_hand_qty,0)) as Leftover_QTY   ";
            SQL = SQL + "    FROM INVSUBMAT.inv_stock_items_v a,   ";
            SQL = SQL + "         inv_item i,   ";
            SQL = SQL + "         inv_item_lot lot,   ";
            SQL = SQL + "         INVSUBMAT.inv_store s, ";
            SQL = SQL + "         PPO_HD PPO   ";
            SQL = SQL + "   WHERE a.inv_item_id = i.inv_item_id ";
            SQL = SQL + "     AND a.lot_id = lot.lot_id(+) ";
            SQL = SQL + "        AND a.on_hand_qty > 0 ";
            SQL = SQL + "     AND a.store_cd = s.store_cd ";
            SQL = SQL + "     AND i.item_type_cd = 'F' ";
            SQL = SQL + "      AND  EXISTS(select 1 from escmreader.rpt_tmp where f1= lot.PO_NO )  ";
            SQL = SQL + "     AND s.stock_class_cd in ('L1','L3','L4') ";
            SQL = SQL + "     AND a.inv_fty_cd ='" + factoryCd + "' ";
            SQL = SQL + "     AND s.active = 'Y' ";
            SQL = SQL + "     AND fabric_type in('B','BD') ";
            SQL = SQL + "     AND PPO.PPO_NO=LOT.PO_NO ";
            SQL = SQL + "     group by lot.PO_NO, lot.grade,ppo.FABRIC_NATURE_CD) ";
            SQL = SQL + "group by  PO_NO ";
            return DBUtility.GetTable(SQL, connection);
        }


        //================StanderMUReport========================================
        //======================================================================

        //================MuGISInvoiceDatastatusReport==============================
        public static DataTable GetGISInvoiceDataStatus(string FactoryCD, string beginDate, string endDate, string strRunNO, string JoNo)
        {
            string SQL = " set session tx_isolation='READ-UNCOMMITTED';";
            SQL += " delete from MU_SHIP_JO_INFO WHERE RUN_NO='" + strRunNO + "';";
            SQL += " delete from MU_SHIP_JO_INFO WHERE datediff(now(),create_date) >1;";
            SQL += " INSERT INTO MU_SHIP_JO_INFO(CT_NO,SHIP_DATE,RUN_NO,CREATE_DATE)";
            SQL += " SELECT JOB_ORDER_NO,CREATE_DATE,'" + strRunNO + "', date_format(now(),'%Y-%m-%d') FROM JO_SHIP_STATUS WHERE FACTORY_CD='" + FactoryCD + "' ";
            if (!beginDate.Equals("") && !endDate.Equals(""))
            {
                SQL += " AND CREATE_DATE >='" + beginDate + "' AND CREATE_DATE <='" + endDate + "' ";
            }
            if (!JoNo.Equals(""))
            {
                SQL += " AND JOB_ORDER_NO ='" + JoNo + "'";
            }
            SQL += " AND SHIP_STATUS='Y';";

            SQL += " SELECT JO_NO AS 'JO No.', date_format(CREATE_DATE,'%Y-%m-%d')  AS 'Upload Date',SHIP_QTY AS 'Ship Qty', date_format(SHIP_DATE,'%Y-%m-%d') AS 'Ship Date',SHIP_STATUS AS 'Ship Status',INVOICE_TYPE AS 'Invoice Type','' AS 'Remarks'";
            SQL += " ,CREATE_TIME AS 'CREATE TIME',last_update_time AS 'Last Update Time' ";//添加cols；

            SQL += " FROM (";

            SQL += " select U.CT_NO AS JO_NO,U.SHIP_DATE AS CREATE_DATE,INVOICE_TYPE,SUM(QUANTITY) AS SHIP_QTY,SHIP_STATUS,P.SHIP_DATE";
            SQL += " ,P.CREATE_TIME,P.last_update_time ";//添加cols；

            SQL += " from ";
            SQL += " MU_SHIP_JO_INFO U";
            SQL += " left join";
            SQL += " (select S.CT_NO,LEFT(B.INVOICE_NO,1) AS INVOICE_TYPE,QUANTITY,B.STATUS AS SHIP_STATUS,B.SHIP_DATE";
            SQL += " ,B.CREATE_TIME,B.last_update_time ";//添加cols；

            SQL += "  from  MU_SHIP_JO_INFO S";
            SQL += "  inner JOIN FTY_INVOICE_DT A";
            SQL += "   ON S.CT_NO = A.CT_NO";
            SQL += "  inner JOIN FTY_INVOICE_HD B";
            SQL += "  ON A.INVOICE_ID=B.INVOICE_ID   ";
            SQL += " WHERE S.RUN_NO='" + strRunNO + "'";
            SQL += " AND B.FACTORY_CD='" + FactoryCD + "'";
            SQL += " UNION  ALL";
            SQL += " select S.CT_NO,LEFT(B.INVOICE_NO,1) AS INVOICE_TYPE,QUANTITY,B.STATUS AS SHIP_STATUS,B.SHIP_DATE";
            SQL += " ,B.CREATE_TIME,B.last_update_time ";//添加cols；

            SQL += " from MU_SHIP_JO_INFO S";
            SQL += " inner JOIN CUST_INVOICE_DT A";
            SQL += " ON  S.CT_NO = A.CT_NO";
            SQL += " inner JOIN CUST_INVOICE_HD B";
            SQL += " ON A.INVOICE_ID=B.INVOICE_ID";
            SQL += " WHERE S.RUN_NO='" + strRunNO + "' ";
            SQL += " AND B.FACTORY_CD='" + FactoryCD + "') P";
            SQL += " ON P.CT_NO=U.CT_NO";
            SQL += " WHERE U.RUN_NO='" + strRunNO + "' ";
            SQL += " GROUP BY U.CT_NO,U.SHIP_DATE,INVOICE_TYPE,SHIP_STATUS,P.SHIP_DATE";

            SQL += " ) AS T";

            return DBUtility.GetTable(SQL, "GIS");
        }

        //Added by ming on 20160623, JO Combination-lock button
        public static DataTable LockAuthorise(string UserId)
        {
            string SQL = @" 
                        SELECT TOP 1 1 FROM MES_USER_ROLE A WITH(NOLOCK) INNER JOIN MES_USER B WITH(NOLOCK) ON A.USER_ID=B.USER_ID
                        INNER JOIN MES_ROLE C WITH(NOLOCK) ON A.ROLE_ID=C.ROLE_ID
                            WHERE B.USER_ID='" + UserId + @"' AND C.ROLE_NAME='LOCK_FOR_MU'";

            return MESComment.DBUtility.GetTable(SQL, "MES");
        }
        //End of added by ming on 20160623,JO Combination-lock button

        //========================================================================

        public static DataTable GetJoListforStandardMu(string JoList, string factoryCd, string GARMENT_TYPE_CD, string fromDate, string toDate, string RunNO)
        {
            StringBuilder SQL = new StringBuilder();
            SQL.AppendFormat(@"          SELECT    UPPER(JOB_ORDER_NO) as JOB_ORDER_NO ,
                    ISNULL(CREATED_COMBINE_JO_FLAG, 'N') AS CombineFlag
          INTO      #tmpjo
          FROM      PRD_JO_COMPLETE_STATUS a ( NOLOCK )
                    INNER JOIN JO_HD h ( NOLOCK ) ON a.JOB_ORDER_NO = h.JO_NO
          WHERE     a.PROCESS_CD = 'FIN'
                    AND a.PROD_COMPLETE = 'Y'
                    {0} {1} {2} {3} {4}
                    AND NOT EXISTS ( SELECT 1
                                     FROM   dbo.MU_Report_All_Data
                                     WHERE  ReportType = 'smr'
                                            AND LOCK_FLAG = 'Y'
                                            AND JO_NO = a.JOB_ORDER_NO )
                                
          DELETE    FROM #tmpjo
          WHERE     CombineFlag = 'Y'
                                
          SELECT    JOB_ORDER_NO AS JO_NO ,
                    ISNULL(ORIGINAL_JO_NO, JOB_ORDER_NO) AS ORIGINAL_JO_NO ,
                    ISNULL(ORIGINAL_JO_NO, '') AS ORIGINALJO
          FROM      #tmpjo a
                    LEFT JOIN JO_COMBINE_MAPPING b ( NOLOCK ) ON a.JOB_ORDER_NO = b.COMBINE_JO_NO ",
                        (factoryCd != "" ? string.Format("AND h.FACTORY_CD='{0}'", factoryCd) : ""),
                        (fromDate != "" ? string.Format("AND a.PROD_COMPLETE_DATE >='{0}'", fromDate) : ""),
                        (toDate != "" ? string.Format("AND a.PROD_COMPLETE_DATE <DATEADD(DAY,1,'{0}')", toDate) : ""),
                        (JoList != "" ? string.Format("AND a.JOB_ORDER_NO in({0})", JoList) : ""),
                        (GARMENT_TYPE_CD != "" ? string.Format("AND h.GARMENT_TYPE_CD ='{0}'", GARMENT_TYPE_CD) : ""));
//            SQL.Append(@" AND NOT EXISTS(
//                                  SELECT 1 FROM dbo.MU_Report_All_Data WHERE ReportType='smr' and LOCK_FLAG='Y' AND JO_NO=a.JOB_ORDER_NO)");
                                
            //SQL.Append(" delete MU_Report_All_Data where CREATE_DATE<dateadd(day,-2,getdate()) and LOCK_FLAG='N'; ");
            //SQL.AppendFormat(" delete MU_Report_All_Data where RunNo='{0}'; ", RunNO);
//            SQL.AppendFormat(@" SELECT  UPPER(JOB_ORDER_NO) AS JOB_ORDER_NO,
//                                    J.SC_NO ,
//                                s.buyer_po_del_date ,
//                                s.total_qty ,
//                                s.PERCENT_OVER_ALLOWED ,
//                                c.SHORT_NAME ,
//                                j.wash_type_cd ,
//                                    ( CASE WHEN t.factory_cd IN ( 'GEG', 'YMG', 'NBO', 'CEK', 'CEG' )
//                                        THEN h.Style_CHN_DESC
//                                        ELSE h.STYLE_DESC
//                                    END ) AS style_desc ,
//                                PATTERN_TYPE_CD = ISNULL(j.fab_pattern, 'Solid'), 
//                                ORIGINAL_JO_NO
//                                INTO #tmpjocombine
//                                    FROM (
//                                SELECT JOB_ORDER_NO,ISNULL(ORIGINAL_JO_NO,JOB_ORDER_NO) AS ORIGINAL_JO_NO,FACTORY_CD from PRD_JO_COMPLETE_STATUS a 
//                                LEFT JOIN dbo.JO_COMBINE_MAPPING b ON a.JOB_ORDER_NO=b.COMBINE_JO_NO
//                                    WHERE  a.PROCESS_CD = 'FIN'
//                                AND a.PROD_COMPLETE = 'Y' {0} {1} {2} 
//                                )t 
//                                INNER JOIN jo_hd J ON J.jo_no = t.JOB_ORDER_NO {3}
//                                INNER JOIN sc_lot S ON S.sc_no = j.sc_no
//                                                        AND s.lot_no = j.lot_no
//                                INNER JOIN GEN_CUSTOMER C ON C.CUSTOMER_CD = j.CUSTOMER_CD
//                                INNER JOIN sc_hd h ON h.sc_no = j.sc_no",
//                                                                                                      (factoryCd != "" ? string.Format("AND a.FACTORY_CD='{0}'", factoryCd) : ""),
//                                                                                                      (fromDate != "" ? string.Format("AND a.PROD_COMPLETE_DATE >='{0}'", fromDate) : ""),
//                                                                                                      (toDate != "" ? string.Format("AND a.PROD_COMPLETE_DATE <DATEADD(DAY,1,'{0}')", toDate) : ""),
//                                                                                                      (JoList != "" ? string.Format("AND a.JOB_ORDER_NO >='{0}'", JoList) : ""),
//                                                                                                      (GARMENT_TYPE_CD != "" ? string.Format("AND J.GARMENT_TYPE_CD ='{0}'", GARMENT_TYPE_CD) : ""));

//            //SQL.Append(" insert into MU_Report_All_Data(FromDate,ToDate,FTY_CD,JO_NO,SC_NO,BPO_Date,ORDER_QTY,OVER_SHIP, ");
//            //SQL.Append(" buyer,Wash_Type,STYLE_DESC,PATTERN_TYPE,LOCK_FLAG,CREATE_DATE,RunNo,SHIP_DATE)  ");
//            SQL.AppendFormat(@" SELECT FromDate='{0}',ToDate='{1}',FTY_CD='{2}', 
//                                JOB_ORDER_NO as JO_NO,SC_NO,min(buyer_po_del_date) buyer_po_del_date,SUM(total_qty) AS total_qty,
//                                CASE WHEN SUM(total_qty)>0 then CAST(ROUND(( ( SUM(TOTAL_QTY * ( 1
//                                                                                        + PERCENT_OVER_ALLOWED
//                                                                                        / 100 ))
//                                                                        / SUM(TOTAL_QTY) ) - 1 ) * 100,
//                                                                    2) AS DECIMAL(38, 2)) 
//                               ELSE 0 END AS PERCENT_OVER_ALLOWED,
//                               SHORT_NAME,
//                               wash_type_cd,style_desc,PATTERN_TYPE_CD
//                               FROM #tmpjocombine
//                                GROUP BY JOB_ORDER_NO,SC_NO,SHORT_NAME,wash_type_cd,style_desc,PATTERN_TYPE_CD", fromDate, toDate, factoryCd);

            //SQL.Append(" select distinct JOB_ORDER_NO as JO_NO,ORIGINAL_JO_NO from #tmpjocombine order by  JOB_ORDER_NO,ORIGINAL_JO_NO ");
            return DBUtility.GetTable(SQL.ToString(), "MES");
            
        }

        public static DataTable GetGoListforStandardMu(string JoStr, string factoryCd, string fromDate, string toDate, string GARMENT_TYPE_CD)
        {
            StringBuilder SQL = new StringBuilder();
             SQL.AppendFormat(@"

                      
                              SELECT  FromDate='{1}',ToDate='{2}',FTY_CD='{3}',GARMENT_TYPE_CD='{4}',UPPER(JO_NO) AS JOB_ORDER_NO,
                                J.SC_NO ,
                                s.buyer_po_del_date ,
                                s.total_qty ,
                                s.PERCENT_OVER_ALLOWED+100 as PERCENT_OVER_ALLOWED,
                                100-s.PERCENT_SHORT_ALLOWED as PERCENT_SHORT_ALLOWED,
                                c.SHORT_NAME ,
                                j.wash_type_cd ,
                                    ( CASE WHEN j.factory_cd IN ( 'GEG', 'YMG', 'NBO', 'CEK', 'CEG' )
                                        THEN h.Style_CHN_DESC
                                        ELSE h.STYLE_DESC
                                    END ) AS style_desc ,
                                PATTERN_TYPE_CD = ISNULL(j.fab_pattern, 'Solid')
                                from jo_hd J 
                                INNER JOIN sc_lot S ON S.sc_no = j.sc_no
                                                        AND s.lot_no = j.lot_no
                                INNER JOIN GEN_CUSTOMER C ON C.CUSTOMER_CD = j.CUSTOMER_CD
                                INNER JOIN sc_hd h ON h.sc_no = j.sc_no
                               where JO_NO in({0}) ", JoStr, fromDate, toDate, factoryCd, GARMENT_TYPE_CD);
                        SQL.Append(factoryCd != "" ? string.Format("AND J.FACTORY_CD='{0}'", factoryCd) : "");
                        SQL.Append(GARMENT_TYPE_CD != "" ? string.Format("AND J.GARMENT_TYPE_CD ='{0}'", GARMENT_TYPE_CD) : "");
                       

            return DBUtility.GetTable(SQL.ToString(), "MES");
        }

     
        public static DataTable GetStandardMUReportList(string factoryCd, string RunNO, string Datatype)
        {
            string SQL = "";
            if (Datatype == "New")
            {
                //Added by ZouShCh ON 2013.07.29 START
                SQL += " exec Proc_Generate_StandandMU_Report '" + factoryCd + "','" + RunNO + "';";
                //SQL += " exec Proc_Generate_MU_Report_TEST '" + factoryCd + "','" + RunNO + "';";
                //Added by ZouShCh ON 2013.07.29 END
            }
            SQL += " SELECT CUT_DATE,JO_NO as PO_NO,YPD_JOB_NO,SHIP_DATE,BUYER,SC_NO ";
            SQL += ",PPO_NO,WIDTH,style_desc,Wash_Type as WASH_TYPE_CD ";
            SQL += "      ,PATTERN_TYPE,CONVERT(VARCHAR(10),MU ) + '%' as MU,PPO_ORDER_YDS as ORDER_QTY,ALLOCATED as allocatedQty,[allocatedQty(nodiscount)]";
            SQL += ",Issued,MARKER_AUDITED as MA,ShipYardage,CUT_WSTG_YDS as CutWastageYPD,";
            //SQL += "	  ,CONVERT(VARCHAR(10),CUT_WSTG_PER ) + '%' as CutWastageYPDPer,";
            SQL += " CASE WHEN ISNULL(Issued,0)=0 THEN 'N/A' ELSE CONVERT(VARCHAR(20),CAST(ISNULL(CUT_WSTG_YDS,0)/Issued*100 AS decimal(38,2))) + '%' END as CutWastageYPDPer,";
            SQL += " DEFECT_FAB,DEFECT_PANELS,ODD_LOSS,SPLICE_LOSS,CUT_REJ_PANELS,MATCH_LOSS,END_LOSS,SHORT_YDS,SEW_MATCH_LOSS,";
            SQL += " LEFTOVER_FAB as Leftover,LEFTOVER_REASON,LEFTOVER_desc,REMNANT";
            SQL += "      ,SRN as SRNQty,RTW as RTW_QTY,ORDER_QTY as ORDERQTY,CUT_QTY as CUTQTY ";
            SQL += "      ,SHIPPED_QTY as SHIP_QTY,SAMPLE as SAMPLE_QTY,PULL_OUT as PULLOUT_QTY";
            SQL += "      ,LEFTOVER_A as GMT_QTY_A,LEFTOVER_B as GMT_QTY_B,LEFTOVER_C as DISCREPANCY_QTY";
            SQL += "      ,TOTAL_LEFTOVER as GMT_QTY_TOTAL,SEW_WSTG_QTY as SewingDz,CONVERT(VARCHAR(10),SEW_WSTG_PER) + '%' as SewingPercent";
            SQL += "      ,WASH_WSTG_QTY as WashingDz,CONVERT(VARCHAR(10),WASH_WSTG_PER) + '%' as WashingPercent,UNACC_GMT as UnaccGmt";
            SQL += "      ,PPO_YPD,BULK_YPD as BULK_NET_YPD,GIVEN_CUT_YPD,BULK_MKR_YPD,YPD_VER as YPD_Var";
            SQL += "      ,CUT_YPD as cutYPD,SHIP_YPD as ShipYPD,CONVERT(VARCHAR(10),SHIP_TO_CUT) + '%' as ShipToCut";
            SQL += "      ,CONVERT(VARCHAR(10),SHIP_TO_RECEIPT) + '%' as ShipToRecv,CONVERT(VARCHAR(10),SHIP_TO_ORDER) + '%' as ShipToOrder";
            SQL += "      ,CONVERT(VARCHAR(10),CUT_TO_RECEIPT) + '%' as cut_to_receipt,CONVERT(VARCHAR(10),CUT_TO_ORDER) + '%' as cut_to_order";
            SQL += "      ,CONVERT(VARCHAR(10),OVER_SHIP) + '%' as OVERSHIP,CONVERT(VARCHAR(10),SHORT_SHIP) + '%' as SHORTSHIP,CONVERT(VARCHAR(10),OVER_CUT) + '%' as OVER_CUT      ";
            SQL += "  FROM dbo.MU_Report_All_Data";
            SQL += " WHERE FTY_CD='" + factoryCd + "' and RunNo='" + RunNO + "' and JO_NO<>'TTL'; ";
            return DBUtility.GetTable(SQL, "MES_UPDATE");
        }
        public static DataTable GetStandardMUReportSummary(string factoryCd, string RunNO)
        {
            string SQL = "";
            SQL += " SELECT CUT_DATE,JO_NO as PO_NO,YPD_JOB_NO,SHIP_DATE,BUYER,SC_NO ";
            SQL += ",PPO_NO,WIDTH,style_desc,Wash_Type as WASH_TYPE_CD ";
            SQL += "      ,PATTERN_TYPE,CONVERT(VARCHAR(10),MU ) + '%' as MU,PPO_ORDER_YDS as ORDER_QTY,ALLOCATED as allocatedQty,[allocatedQty(nodiscount)]";
            SQL += ",Issued,MARKER_AUDITED as MA,ShipYardage,CUT_WSTG_YDS as CutWastageYPD,";
            //SQL += "	  ,CONVERT(VARCHAR(10),CUT_WSTG_PER ) + '%' as CutWastageYPDPer,";
            SQL += " CASE WHEN ISNULL(Issued,0)=0 THEN 'N/A' ELSE CONVERT(VARCHAR(20),CAST(ISNULL(CUT_WSTG_YDS,0)/Issued*100 AS decimal(38,2))) + '%' END as CutWastageYPDPer,";
            SQL += " DEFECT_FAB,DEFECT_PANELS,ODD_LOSS,SPLICE_LOSS,CUT_REJ_PANELS,MATCH_LOSS,END_LOSS,SHORT_YDS,SEW_MATCH_LOSS,";
            SQL += " LEFTOVER_FAB as Leftover,LEFTOVER_REASON,LEFTOVER_desc,REMNANT";
            SQL += "      ,SRN as SRNQty,RTW as RTW_QTY,ORDER_QTY as ORDERQTY,CUT_QTY as CUTQTY ";
            SQL += "      ,SHIPPED_QTY as SHIP_QTY,SAMPLE as SAMPLE_QTY,PULL_OUT as PULLOUT_QTY";
            SQL += "      ,LEFTOVER_A as GMT_QTY_A,LEFTOVER_B as GMT_QTY_B,LEFTOVER_C as DISCREPANCY_QTY";
            SQL += "      ,TOTAL_LEFTOVER as GMT_QTY_TOTAL,SEW_WSTG_QTY as SewingDz,CONVERT(VARCHAR(10),SEW_WSTG_PER) + '%' as SewingPercent";
            SQL += "      ,WASH_WSTG_QTY as WashingDz,CONVERT(VARCHAR(10),WASH_WSTG_PER) + '%' as WashingPercent,UNACC_GMT as UnaccGmt";
            SQL += "      ,PPO_YPD,BULK_YPD as BULK_NET_YPD,GIVEN_CUT_YPD,BULK_MKR_YPD,YPD_VER as YPD_Var";
            SQL += "      ,CUT_YPD as cutYPD,SHIP_YPD as ShipYPD,CONVERT(VARCHAR(10),SHIP_TO_CUT) + '%' as ShipToCut";
            SQL += "      ,CONVERT(VARCHAR(10),SHIP_TO_RECEIPT) + '%' as ShipToRecv,CONVERT(VARCHAR(10),SHIP_TO_ORDER) + '%' as ShipToOrder";
            SQL += "      ,CONVERT(VARCHAR(10),CUT_TO_RECEIPT) + '%' as cut_to_receipt,CONVERT(VARCHAR(10),CUT_TO_ORDER) + '%' as cut_to_order";
            SQL += "      ,CONVERT(VARCHAR(10),OVER_SHIP) + '%' as OVERSHIP,CONVERT(VARCHAR(10),OVER_CUT) + '%' as OVER_CUT,CONVERT(VARCHAR(10),SHORT_SHIP) + '%' as SHORTSHIP      ";
            SQL += "  FROM dbo.MU_Report_All_Data";
            SQL += " WHERE FTY_CD='" + factoryCd + "' and RunNo='" + RunNO + "' and JO_NO='TTL'; ";
            return DBUtility.GetTable(SQL, "MES_UPDATE");
        }

        public static DataTable GetStandardGoQty(DbConnection eelConn)
        {
            string SQL = "";
            SQL = SQL + "        SELECT SC_NO,sum(ORDER_QTY) as ORDER_QTY,sum(SC_QTY) as SC_QTY FROM ";
            SQL = SQL + "         (SELECT SC_NO,NVL(SUM(order_qty),0) AS ORDER_QTY,0 AS SC_QTY FROM ";
            SQL = SQL + "        ( SELECT s.sc_no,SUM (d.order_qty) AS order_qty ";
            SQL = SQL + "        FROM ppo_hd h, ppo_dt d, fab_lib f,         ";
            SQL = SQL + "        (select distinct sc_no,fabric_type_cd,pPO_NO from scx_joi_go_fabric aa  ";
            SQL = SQL + "          where exists (select f1 from rpt_tmp where f1=aa.sc_no)";
            SQL = SQL + "            ) s  ";
            SQL = SQL + "        WHERE h.ppO_NO = d.ppO_NO AND  h.flag != 'X'  ";
            SQL = SQL + "        AND d.fab_ref_cd = f.fab_ref_cd AND h.status IN ('L2', 'R')  ";
            SQL = SQL + "        AND H.PpO_NO=s.ppO_NO  and d.fabric_type_cd=s.fabric_type_cd and s.fabric_type_cd in ('B','BD')  ";
            SQL = SQL + "         GROUP BY s.sc_no  ";
            SQL = SQL + "         UNION ALL SELECT s.sc_no,SUM (pl.order_qty) AS order_qty  ";
            SQL = SQL + "         FROM ppo_hd a, ppox_fab_item b,ppox_lot pl, ";
            SQL = SQL + "        (select distinct sc_no,fabric_type_cd,ppO_NO from scx_joi_go_fabric aa  ";
            SQL = SQL + "          where exists (select f1 from rpt_tmp where f1=aa.sc_no)";
            SQL = SQL + "          ) s  ";
            SQL = SQL + "         WHERE a.ppO_NO = b.ppO_NO AND b.fab_item_id = pl.fab_item_id AND a.flag = 'X'  ";
            SQL = SQL + "         AND a.status IN ('L2', 'R') AND  b.status != 'C'  ";
            SQL = SQL + "         AND A.PpO_NO=s.ppO_NO and b.fabric_type_cd=s.fabric_type_cd and s.fabric_type_cd in ('B','BD') ";
            SQL = SQL + "          GROUP BY  s.sc_no,b.fabric_type_cd  ";
            SQL = SQL + "          UNION ALL SELECT s.sc_no,sum(d.qty) AS order_qty  ";
            SQL = SQL + "          FROM fab_sample_order_hd a, fab_so_combo b,fab_so_product c,fab_so_combo_qty d, ";
            SQL = SQL + "        (select distinct sc_no,fabric_type_cd,ppO_NO from scx_joi_go_fabric aa  ";
            SQL = SQL + "          where exists (select f1 from rpt_tmp where f1=aa.sc_no)";
            SQL = SQL + "          ) s  ";
            SQL = SQL + "          WHERE d.req_prod_cd = c.req_prod_cd and  a.fab_so_no = b.fab_so_no AND b.fab_so_no = d.fab_so_no  ";
            SQL = SQL + "          AND b.combo_seq = d.combo_seq AND a.status IN ('P', 'S', 'D')  ";
            SQL = SQL + "          AND A.fab_so_no=s.ppO_NO and c.fabric_part=s.fabric_type_cd and s.fabric_type_cd in ('B','BD') ";
            SQL = SQL + "         group by a.fab_so_no,s.sc_no     ) AAA  ";
            SQL = SQL + "        GROUP BY SC_NO  ";
            SQL = SQL + " UNION ALL ";
            SQL = SQL + " select sc_no as sc_NO,0 AS ORDER_QTY,round(sum(Total_QTY)/12,2) as OrderQTY ";
            SQL = SQL + " FROM ";
            SQL = SQL + " sc_lot b where exists (select f1 from rpt_tmp where f1=sc_no) and active='Y' group by sc_no)PP group by SC_NO";
            return DBUtility.GetTable(SQL, eelConn);
        }

        public static DataTable GetStandardGoQty_New(DbConnection eelConn)
        {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.AppendFormat(@"
                     SELECT SC_NO,sum(ORDER_QTY) as ORDER_QTY,sum(SC_QTY) as SC_QTY FROM
                      (SELECT X.SC_NO, SUM (D.order_qty) AS ORDER_QTY,0 as SC_QTY
                        FROM ESCMREADER.TMP_MUPPO X
                             INNER JOIN ppo_hd h
                                ON     h.ppo_no = x.ppo_no
                                   AND h.status IN ('L2', 'R')
                                   AND h.flag != 'X'
                             INNER JOIN ppo_dt d
                                ON d.ppO_NO = h.ppO_NO AND d.fabric_type_cd = x.fabric_type_cd
                             INNER JOIN fab_lib f ON f.fab_ref_cd = d.fab_ref_cd
                    GROUP BY x.sc_no
                    UNION ALL
                      SELECT X.SC_NO, SUM (f.order_qty) AS order_qty,0 as SC_QTY
                        FROM ESCMREADER.TMP_MUPPO X
                             INNER JOIN ppo_hd h
                                ON     h.ppo_no = x.ppo_no
                                   AND h.status IN ('L2', 'R')
                                   AND h.flag != 'X'
                             INNER JOIN ppo_item d
                                ON     d.ppO_NO = h.ppO_NO
                                   AND d.fabric_type_cd = x.fabric_type_cd
                                   AND d.status != 'C'
                             INNER JOIN ppo_item_lot_hd e
                                ON e.ppo_item_id = d.ppo_item_id AND e.status != 'C'
                             INNER JOIN ppo_item_lot_dt f
                                ON f.item_lot_hd_id = e.item_lot_hd_id AND f.status != 'C'
                    GROUP BY x.sc_no
                    UNION ALL
                      SELECT X.SC_NO, SUM (f.order_qty) AS order_qty,0 as SC_QTY
                        FROM ESCMREADER.TMP_MUPPO X
                             INNER JOIN ppo_hd h
                                ON h.ppo_no = x.ppo_no AND h.status IN ('L2', 'R') 
                             INNER JOIN ppox_fab_item d
                                ON     d.ppO_NO = h.ppO_NO
                                   AND d.fabric_type_cd = x.fabric_type_cd
                                   AND d.status != 'C'
                             INNER JOIN ppox_lot f ON f.fab_item_id = d.fab_item_id
                    GROUP BY x.sc_no  
                    union all
                    select sc_no as sc_NO,0 AS ORDER_QTY,round(sum(Total_QTY)/12,2) as OrderQTY
                    FROM 
                    sc_lot b where exists (select SC_NO from ESCMREADER.TMP_MUPPO where SC_NO=b.sc_no) and active='Y' group by sc_no) PP group by SC_NO
                    ");
            return DBUtility.GetTable(sql.ToString(), eelConn);
        }


        public static DataTable GetStandardPPONO(DbConnection eelConn)
        {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.AppendFormat(@"SELECT DISTINCT A.SC_NO,A.fabric_type_cd,A.PPO_NO
                                  FROM SCX_JOI_GO_FABRIC A
                                     INNER JOIN (SELECT DISTINCT SL.SC_NO,SLP.PPO_NO
                                       FROM SC_LOT SL
                                     LEFT JOIN SCX_LOT_PPO SLP ON SLP.SC_NO=SL.SC_NO AND SLP.LOT_NO=SL.LOT_NO 
                                          WHERE SL.SC_NO IN(select f1 from rpt_tmp) AND SL.ACTIVE='Y') PL 
                                    ON PL.SC_NO=A.SC_NO AND NVL(PL.PPO_NO,'-')=DECODE(NVL(PL.PPO_NO,'-'),'-','-',A.PPO_NO)
                                   Where A.fabric_type_cd IN ('B', 'BD')");
            return DBUtility.GetTable(sql.ToString(), eelConn);
        }

        public static DataTable GetMUReportByRunNo(string RunNo)
        {
            string SQL = "";
            SQL += " select top 1 * from MU_Report_All_Data where RunNo='" + RunNo + "' ";
            return DBUtility.GetTable(SQL, "MES_UPDATE");
        }

        public static bool LockMuReport(string RunNo)
        {
            DbConnection MESConn = MESComment.DBUtility.GetConnection("MES_UPDATE");
            return DBUtility.LockMuReport(RunNo, MESConn);
        }

        public static DataTable GetLockMuReport(string JoList,string factoryCd, string GARMENT_TYPE_CD, string fromDate, string toDate)
        {
            StringBuilder SQL = new StringBuilder();
           
            SQL.Append(" SELECT CUT_DATE,JO_NO as PO_NO,YPD_JOB_NO,SHIP_DATE,BUYER,SC_NO ");
            SQL.Append(" ,PPO_NO,WIDTH,style_desc,Wash_Type as WASH_TYPE_CD ");
            SQL.Append("      ,PATTERN_TYPE,CONVERT(VARCHAR(10),MU ) + '%' as MU,PPO_ORDER_YDS as ORDER_QTY,ALLOCATED as allocatedQty,[allocatedQty(nodiscount)]");
            SQL.Append(" ,Issued,MARKER_AUDITED as MA,ShipYardage,CUT_WSTG_YDS as CutWastageYPD,");
         
            SQL.Append(" CASE WHEN ISNULL(Issued,0)=0 THEN 'N/A' ELSE CONVERT(VARCHAR(20),CAST(ISNULL(CUT_WSTG_YDS,0)/Issued*100 AS decimal(38,2))) + '%' END as CutWastageYPDPer,");
            SQL.Append(" DEFECT_FAB,DEFECT_PANELS,ODD_LOSS,SPLICE_LOSS,CUT_REJ_PANELS,MATCH_LOSS,END_LOSS,SHORT_YDS,SEW_MATCH_LOSS,");
            SQL.Append(" LEFTOVER_FAB as Leftover,LEFTOVER_REASON,LEFTOVER_desc,REMNANT");
            SQL.Append("      ,SRN as SRNQty,RTW as RTW_QTY,ORDER_QTY as ORDERQTY,CUT_QTY as CUTQTY ");
            SQL.Append("      ,SHIPPED_QTY as SHIP_QTY,SAMPLE as SAMPLE_QTY,PULL_OUT as PULLOUT_QTY");
            SQL.Append("      ,LEFTOVER_A as GMT_QTY_A,LEFTOVER_B as GMT_QTY_B,LEFTOVER_C as DISCREPANCY_QTY");
            SQL.Append("      ,TOTAL_LEFTOVER as GMT_QTY_TOTAL,SEW_WSTG_QTY as SewingDz,CONVERT(VARCHAR(10),SEW_WSTG_PER) + '%' as SewingPercent");
            SQL.Append("      ,WASH_WSTG_QTY as WashingDz,CONVERT(VARCHAR(10),WASH_WSTG_PER) + '%' as WashingPercent,UNACC_GMT as UnaccGmt");
            SQL.Append("      ,PPO_YPD,BULK_YPD as BULK_NET_YPD,GIVEN_CUT_YPD,BULK_MKR_YPD,YPD_VER as YPD_Var");
            SQL.Append("      ,CUT_YPD as cutYPD,SHIP_YPD as ShipYPD,CONVERT(VARCHAR(10),SHIP_TO_CUT) + '%' as ShipToCut");
            SQL.Append("      ,CONVERT(VARCHAR(10),SHIP_TO_RECEIPT) + '%' as ShipToRecv,CONVERT(VARCHAR(10),SHIP_TO_ORDER) + '%' as ShipToOrder");
            SQL.Append("      ,CONVERT(VARCHAR(10),CUT_TO_RECEIPT) + '%' as cut_to_receipt,CONVERT(VARCHAR(10),CUT_TO_ORDER) + '%' as cut_to_order");
            SQL.Append("      ,CONVERT(VARCHAR(10),OVER_SHIP) + '%' as OVERSHIP,CONVERT(VARCHAR(10),SHORT_SHIP) + '%' as SHORTSHIP,CONVERT(VARCHAR(10),OVER_CUT) + '%' as OVER_CUT,RunNo      ");
            SQL.Append("  FROM dbo.MU_Report_All_Data");
            SQL.Append(" WHERE LOCK_FLAG='Y' and ReportType='smr' ");
            if (JoList.Length > 0)
            {
                SQL.AppendFormat(" and JO_NO in({0})", JoList);
            }
            if (factoryCd.Length > 0)
            {
                SQL.AppendFormat(" and FTY_CD='{0}'", factoryCd);
            }
            if (GARMENT_TYPE_CD.Length > 0)
            {
                SQL.AppendFormat(" and GARMENT_TYPE_CD='{0}'", GARMENT_TYPE_CD);
            }
            if (fromDate.Length > 0)
            {
                SQL.AppendFormat(" and  FromDate <='{0}'", fromDate);
            }
            if (fromDate.Length > 0)
            {
                SQL.AppendFormat(" and toDate >= '{0}'", toDate);
            }
             return DBUtility.GetTable(SQL.ToString(), "MES_UPDATE");
        }

        public static string GetJo(string JoList)
        {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.AppendFormat(@"
                             SELECT  JO_NO ,
                                    CREATED_COMBINE_JO_FLAG
                            INTO    #tmpjo
                            FROM    JO_HD (NOLOCK)
                            WHERE   JO_NO IN ( {0} )
                                    AND [STATUS] <> 'D'
                            select distinct COMBINE_JO_NO from (
                            SELECT  JO_NO AS COMBINE_JO_NO ,
                                    JO_NO AS ORIGINAL_JO_NO,'N' CombineFlag
                            FROM    #tmpjo
                            WHERE    ISNULL(CREATED_COMBINE_JO_FLAG,'N') = 'N'
                            UNION
                            SELECT  ISNULL(COMBINE_JO_NO, JO_NO) ,
                                    ISNULL(ORIGINAL_JO_NO, JO_NO) AS ORIGINAL_JO_NO,'Y'
                            FROM    #tmpjo h
                                    INNER JOIN dbo.JO_COMBINE_MAPPING m ( NOLOCK ) ON h.JO_NO = m.COMBINE_JO_NO
                            UNION
                            SELECT  ISNULL(COMBINE_JO_NO, JO_NO) AS JO_NO ,
                                    JO_NO AS ORIGINAL_JO_NO,'Y'
                            FROM    #tmpjo h
                                    INNER JOIN dbo.JO_COMBINE_MAPPING m ( NOLOCK ) ON h.JO_NO = m.ORIGINAL_JO_NO)t ", JoList);

            DataTable dt=MESComment.DBUtility.GetTable(sql.ToString(), "MES");

            JoList = "";
            foreach (DataRow dr in dt.Rows)
            {
                JoList += "'" + dr["COMBINE_JO_NO"] + "',";
            }
            if (JoList.Length > 0)
                JoList = JoList.Substring(0, JoList.Length - 1);
            return JoList;

        }

        public static string GetOriJo(string JoList)
        {
            if(JoList=="")
                return "";
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.AppendFormat(@"
                              SELECT  JO_NO
                            FROM JO_HD
                            WHERE    ISNULL(CREATED_COMBINE_JO_FLAG,'N') = 'N' and JO_NO IN ( {0} )
                            UNION ALL
                            SELECT ORIGINAL_JO_NO FROM JO_COMBINE_MAPPING WHERE COMBINE_JO_NO  IN ( {0} )
             ", JoList);

            DataTable dt = MESComment.DBUtility.GetTable(sql.ToString(), "MES");

            JoList = "";
            foreach (DataRow dr in dt.Rows)
            {
                JoList += "'" + dr["JO_NO"] + "',";
            }
            if (JoList.Length > 0)
                JoList = JoList.Substring(0, JoList.Length - 1);
            return JoList;

        }

        /// <summary>
        /// 通过源始jo,取CB 
        /// </summary>
        /// <param name="JO"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> GetCombineJo(string JO)
        {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.AppendFormat(@"
                              SELECT COMBINE_JO_NO,ORIGINAL_JO_NO FROM JO_COMBINE_MAPPING(nolock) WHERE COMBINE_JO_NO IN(
                              select COMBINE_JO_NO from JO_COMBINE_MAPPING(nolock) where ORIGINAL_JO_NO in({0}))", JO);

            DataTable dt = MESComment.DBUtility.GetTable(sql.ToString(), "MES");
            Dictionary<string, List<string>> cb = new Dictionary<string, List<string>>();
            if (dt.Rows.Count == 0)
                return cb;
            foreach (DataRow dr in dt.Rows)
            {
               
                if (cb.ContainsKey(dr["COMBINE_JO_NO"].ToString()))
                {
                    cb[dr["COMBINE_JO_NO"].ToString()].Add(dr["ORIGINAL_JO_NO"].ToString());
                }
                else
                {
                    cb.Add(dr["COMBINE_JO_NO"].ToString(), new List<string>() { dr["ORIGINAL_JO_NO"].ToString() });
                }
            }
            return cb;

            
        }

    }
}
