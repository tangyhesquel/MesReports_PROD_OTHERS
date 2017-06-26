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
    public class OutsourceMUReportSql
    {
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

        
        public static DataTable GetStandardInfo(DbConnection invConn)
        {

            string SQL = " select ph.SC_NO,c.Short_name as BUYER,ph.WASH_TYPE_CD,(case when d.factory_cd in('GEG','YMG','NBO','CEK','CEG') ";
            SQL = SQL + "THEN D.Style_CHN_DESC ELSE  STYLE_DESC END) as STYLE_DESC        ";
            SQL = SQL + " from  PO_hd ph, gen_customer c,sc_hd d         ";
            SQL = SQL + " where ph.customer_cd = c.customer_cd         ";
            SQL = SQL + " and ph.SC_NO = d.SC_NO         ";
            SQL = SQL + " and exists  (select f1 from escmreader.rpt_tmp where f1=ph.PO_NO) ";
            SQL = SQL + " order by ph.SC_NO";
            return DBUtility.GetTable(SQL, invConn);
        }

        
        public static DataTable GetJoNoList(string GoNo)
        {
            string SQL = "SELECT JO_NO + ';' FROM JO_HD WITH(NOLOCK) WHERE SC_NO='" + GoNo + "' FOR XML PATH('')";

            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetOutsourceJODetail(string GoList)
        {
            string SQL = "IF OBJECT_ID(N'tempdb..#TEMP') IS NOT NULL DROP TABLE #TEMP; ";
            SQL += " IF OBJECT_ID(N'tempdb..#TEMP1') IS NOT NULL DROP TABLE #TEMP1; ";
            SQL += " SELECT SC_NO, JOB_ORDER_NO, ISNULL(DT.QTY,0) AS DT_QTY,ISNULL(DISC.QTY,0) AS DISC_QTY, PRODUCTION_LINE, COLOR_CD, SIZE_CD, HD.FACTORY_CD, "; 
            SQL += " TRX_TYPE, REF_CONTRACT_NO, PROCESS_CD, IS_REAL_SENDING_RECEIVING, WAREHOUSE, CD.SHORT_NAME, DISC.REASON_CD, CD.REASON_DESC    ";
            SQL += " INTO #TEMP FROM OAS_SENDING_RECEIVING_HD HD WITH(NOLOCK)  ";
            SQL += " INNER JOIN OAS_SENDING_RECEIVING_DT DT WITH(NOLOCK) ON HD.DOC_NO=DT.DOC_NO ";
            SQL += " LEFT JOIN OAS_SENDING_RECEIVING_DISCREPANCY DISC WITH(NOLOCK) ON DT.TRX_ID=DISC.TRX_ID  ";
            SQL += " LEFT JOIN PRD_REASON_CODE CD WITH(NOLOCK) ON DISC.REASON_CD=CD.REASON_CD  ";
            SQL += " INNER JOIN JO_HD PO WITH(NOLOCK) ON DT.JOB_ORDER_NO=PO.JO_NO   ";
            SQL += " WHERE PO.SC_NO IN (" + GoList + ");";


            SQL += " SELECT SUB.SUBCONTRACTOR_NAME AS FACTORY, JO.SC_NO, JO.JO_NO AS JOB_ORDER_NO, HD.REF_CONTRACT_NO, 0 AS CUT_DISC, 0 AS SAMPLE_QTY, 0 AS GMT_QTY_A, 0 AS GMT_QTY_B, 0 AS GMT_QTY_C, 0 AS CUTQTY, "; 
	        SQL += " 0 AS SHIP_QTY, 0 AS SEW_WASTAGE, 0 AS WET_WASTAGE ";
	        SQL += " INTO #TEMP1 ";
	        SQL += " FROM OAS_SENDING_RECEIVING_DT DT WITH(NOLOCK) ";
	        SQL += " INNER JOIN OAS_SENDING_RECEIVING_HD HD WITH(NOLOCK) ON HD.DOC_NO=DT.DOC_NO ";
	        SQL += " INNER JOIN JO_HD JO WITH(NOLOCK) ON JO.JO_NO=DT.JOB_ORDER_NO ";
	        SQL += " INNER JOIN PRD_OUTSOURCE_CONTRACT OSC WITH(NOLOCK) ON HD.REF_CONTRACT_NO=OSC.CONTRACT_NO  ";
	        SQL += " INNER JOIN PRD_SUBCONTRACTOR_MASTER SUB WITH(NOLOCK) ON OSC.SUBCONTRACTOR=SUB.SUBCONTRACTOR_CD ";
	        SQL += " WHERE 1=2 GROUP BY SUB.SUBCONTRACTOR_NAME, JO.SC_NO, JO.JO_NO, HD.REF_CONTRACT_NO ";
 
            SQL += " INSERT INTO #TEMP1 ";
            SQL += " SELECT DISTINCT '' AS FACTORY, SC_NO, JOB_ORDER_NO, REF_CONTRACT_NO, 0 AS CUT_DISC, 0 AS SAMPLE_QTY, 0 AS GMT_QTY_A, 0 AS GMT_QTY_B, 0 AS GMT_QTY_C, 0 AS CUT_QTY,  ";
	        SQL += " 0 AS SHIP_QTY, 0 AS SEW_WASTAGE, 0 AS WET_WASTAGE  ";
	        SQL += " FROM #TEMP ";

            SQL += " UPDATE A ";
            SQL += " SET A.FACTORY=ISNULL(B.SUBCONTRACTOR_NAME,'') ";
            SQL += " FROM #TEMP1 A LEFT JOIN  ";
            SQL += " (SELECT A.CONTRACT_NO, B.SUBCONTRACTOR_NAME FROM PRD_OUTSOURCE_CONTRACT A WITH(NOLOCK)  ";
            SQL += " INNER JOIN PRD_SUBCONTRACTOR_MASTER B WITH(NOLOCK) ON A.SUBCONTRACTOR=B.SUBCONTRACTOR_CD GROUP BY A.CONTRACT_NO, B.SUBCONTRACTOR_NAME ";
            SQL += " ) B ON A.REF_CONTRACT_NO=B.CONTRACT_NO ";

            SQL += " UPDATE A ";
            SQL += " SET A.CUT_DISC=ISNULL(B.QTY,0)/12 ";
            SQL += " FROM #TEMP1 A LEFT JOIN ";
            SQL += " (SELECT JOB_ORDER_NO, SUM(DISC_QTY) AS QTY FROM #TEMP WHERE SHORT_NAME='CPNDF' GROUP BY JOB_ORDER_NO ";
            SQL += " ) B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO ";

            SQL += " UPDATE A ";
            SQL += " SET A.SAMPLE_QTY=ISNULL(B.QTY,0)/12 ";
            SQL += " FROM #TEMP1 A LEFT JOIN ";
            SQL += " (SELECT JOB_ORDER_NO, SUM(DISC_QTY) AS QTY FROM #TEMP WHERE SHORT_NAME='SMPL' GROUP BY JOB_ORDER_NO ";
            SQL += " ) B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO ";

            SQL += " UPDATE A ";
            SQL += " SET A.GMT_QTY_A=ISNULL(B.QTY,0)/12 ";
            SQL += " FROM #TEMP1 A LEFT JOIN ";
            SQL += " (SELECT JOB_ORDER_NO, SUM(DT_QTY) AS QTY FROM #TEMP WHERE WAREHOUSE='A' GROUP BY JOB_ORDER_NO ";
            SQL += " ) B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO ";

            SQL += " UPDATE A ";
            SQL += " SET A.GMT_QTY_B=ISNULL(B.QTY,0)/12 ";
            SQL += " FROM #TEMP1 A LEFT JOIN ";
            SQL += " (SELECT JOB_ORDER_NO, SUM(DT_QTY) AS QTY FROM #TEMP WHERE WAREHOUSE='L2' GROUP BY JOB_ORDER_NO ";
            SQL += " ) B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO ";

            SQL += " UPDATE A ";
            SQL += " SET A.GMT_QTY_C=ISNULL(B.QTY,0)/12 ";
            SQL += " FROM #TEMP1 A LEFT JOIN ";
            SQL += " (SELECT JOB_ORDER_NO, SUM(DISC_QTY) AS QTY FROM #TEMP WHERE REASON_DESC<>'SAMPLE' AND SHORT_NAME<>'CPNDF' GROUP BY JOB_ORDER_NO ";
            SQL += " ) B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO ";

            SQL += " UPDATE A ";
            SQL += " SET A.SHIP_QTY=ISNULL(B.QTY,0)/12 ";
            SQL += " FROM #TEMP1 A LEFT JOIN ";
            SQL += " (SELECT JOB_ORDER_NO, SUM(DT_QTY) AS QTY FROM #TEMP WHERE IS_REAL_SENDING_RECEIVING='Y' AND TRX_TYPE='R' GROUP BY JOB_ORDER_NO ";
            SQL += " ) B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO ";

            SQL += " UPDATE A ";
            SQL += " SET A.SEW_WASTAGE=ISNULL(B.QTY,0)/12 ";
            SQL += " FROM #TEMP1 A LEFT JOIN ";
            SQL += " (SELECT JOB_ORDER_NO, SUM(DISC_QTY) AS QTY FROM #TEMP WHERE SHORT_NAME IN ('SEWDFC','SEWDFD') GROUP BY JOB_ORDER_NO ";
            SQL += " ) B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO ";

            SQL += " UPDATE A ";
            SQL += " SET A.WET_WASTAGE=ISNULL(B.QTY,0)/12 ";
            SQL += " FROM #TEMP1 A LEFT JOIN ";
            SQL += " (SELECT JOB_ORDER_NO, SUM(DISC_QTY) AS QTY FROM #TEMP WHERE SHORT_NAME='WSHDF' GROUP BY JOB_ORDER_NO ";
            SQL += " ) B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO ";

            SQL += " UPDATE A ";
            SQL += " SET A.CUTQTY=(ISNULL(B.QTY,0)/12)-A.CUT_DISC  ";
            SQL += " FROM #TEMP1 A LEFT JOIN ";
            SQL += " (SELECT JOB_ORDER_NO, SUM(DT_QTY) AS QTY FROM #TEMP WHERE IS_REAL_SENDING_RECEIVING='Y' AND TRX_TYPE='S' GROUP BY JOB_ORDER_NO ";
            SQL += " ) B ON A.JOB_ORDER_NO=B.JOB_ORDER_NO ";

            SQL += " SELECT * FROM #TEMP1 ";
            

            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetFabWastage(string JoList)
        {
            string SQL = "SELECT GO_NO, JOB_ORDER_NO, DEFECT_FABRIC, PULL_OUT, SHORTAGE_FABRIC FROM CUT_FABRIC_WASTAGE_JO WITH(NOLOCK) WHERE JOB_ORDER_NO IN (" + JoList + ") ";
         

            return DBUtility.GetTable(SQL, "MES");

        }

        public static DataTable GetFocQty(string factoryCd,  DbConnection invConn)
        {
            string SQL = " SELECT   tl.JOB_ORDER_NO, CAST(ROUND(SUM(FOC_QTY)/12,2) AS DECIMAL(18,2)) AS FOC_QTY ";
            SQL += "    FROM inv_trans_line_v tl, invsubmat.inv_store s, ";
            SQL += "    (select distinct f1 AS job_order_no from escmreader.rpt_tmp ) E";
            SQL += "    WHERE tl.store_cd = s.store_cd ";
            SQL += "    AND E.job_order_no= tl.job_order_no ";
            SQL += "               AND exists (select f1 from escmreader.rpt_tmp where f1= tl.job_order_no) ";
            SQL += "               AND s.inv_fty_cd = '" + factoryCd + "' ";
            SQL += " GROUP BY tl.JOB_ORDER_NO ";

            return DBUtility.GetTable(SQL, invConn);
        }

        //fixed bug by LimML on 20150804 - cannot selcet by GO and JO
        public static DataTable GetOASInfo(string GoList, string JoList, string factoryCd, string GARMENT_TYPE_CD, string fromDate, string toDate)
        {
            string SQL = " SELECT PO.SC_NO, JOB_ORDER_NO, CONFDT.CONFIRM_DATE AS SHIP_DATE FROM OAS_SENDING_RECEIVING_HD HD WITH(NOLOCK) ";
            SQL += " INNER JOIN OAS_SENDING_RECEIVING_DT DT WITH(NOLOCK) ON HD.DOC_NO=DT.DOC_NO ";
            SQL += " INNER JOIN JO_HD PO WITH(NOLOCK) ON DT.JOB_ORDER_NO=PO.JO_NO ";
            SQL += "  INNER JOIN ( SELECT SC_NO,MAX(CONFIRM_DATE) AS CONFIRM_DATE FROM OAS_SENDING_RECEIVING_HD OHD, OAS_SENDING_RECEIVING_DT ODT, JO_HD JO with(nolock) where OHD.DOC_NO=ODT.DOC_NO AND ODT.JOB_ORDER_NO=JO.JO_NO AND SC_NO not in (select distinct SC_NO from OAS_SENDING_RECEIVING_HD OHD, OAS_SENDING_RECEIVING_DT ODT, JO_HD JO with(nolock) where OHD.DOC_NO=ODT.DOC_NO AND ODT.JOB_ORDER_NO=JO.JO_NO  and CONFIRM_DATE IS NULL) GROUP BY SC_NO) CONFDT ON PO.SC_NO=CONFDT.SC_NO ";
            SQL += " WHERE HD.FACTORY_CD = '" + factoryCd + "' ";
            if (GoList != "")
                SQL += " AND PO.SC_NO IN (" + GoList + ")";
            if (JoList != "")
                SQL += " AND JOB_ORDER_NO IN (" + JoList + ")";
            if (GARMENT_TYPE_CD != "")
                SQL += " AND GARMENT_TYPE_CD = '" + GARMENT_TYPE_CD + "' ";
            if (fromDate != "" || toDate != "")
            {
                if (fromDate != "")
                    SQL += " AND CONFDT.CONFIRM_DATE >= '" + fromDate + "' ";
                if (toDate != "")
                    SQL += " AND CONFDT.CONFIRM_DATE <= '" + toDate + "' ";
            }
            SQL += " AND HD.STATUS='C' AND HD.TRX_TYPE='R' ";

            return DBUtility.GetTable(SQL, "MES");
        }
    }
}