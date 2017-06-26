using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.Common;

/// <summary>
/// Summary description for ProTranWIPSummary
/// </summary>

namespace MESComment
{
    public class ProTranWIPSummary
    {
        public ProTranWIPSummary()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static DataTable GetProTranWIPAcceptQty(string strTableName, string factoryCd, string GarmentType, string startDate, string endDate, bool isOSJo)
        {
            string StrOSJo = "N";
            if (isOSJo)
            {
                StrOSJo = "Y";
            }
            string sql = "exec PROC_INQ_Transaction '" + strTableName + "','" + factoryCd + "','','Y','BPO','" + startDate + "','" + endDate + "','I','','Summary','S','" + GarmentType + "','','','','" + StrOSJo + "'; select *,'' as AUDIT,''as CLOSING from " + strTableName + " mas";
            sql = sql + " where exists(select 1 from sc_hd sc,jo_hd jo where sc.sc_no=jo.sc_no and sc.order_type='G' and jo.jo_no=mas.jo and (jo.status<>'D' or (jo.status='D' and exists(select 1 from cut_bundle_hd where job_order_no=jo.jo_no))))";
            return DBUtility.GetTable(sql, "MES_UPDATE");
        }
        //Added By ZouShiChang ON 2013.08.29 Start MES024
        /*
        public static DataTable GetProTranWIPAcceptQtyTrxTitle(string factoryCd,string GarmentType)
        {
            string SQL = " select PRC_CD,DISPLAY_SEQ from gen_prc_cd_mst where major_flag like '%C%' and factory_cd='"+factoryCd+"' ";
            if (GarmentType != "")
            {
                SQL = SQL + "AND GARMENT_TYPE='"+GarmentType+"' ";
            }
            SQL = SQL + "ORDER BY GARMENT_TYPE,DISPLAY_SEQ ";
            return DBUtility.GetTable(SQL, "MES");
        }
         */
        public static DataTable GetProTranWIPAcceptQtyTrxTitle(string factoryCd, string GarmentType)
        {
            string SQL = "Select * from ( select DISTINCT PRC_CD,DISPLAY_SEQ from gen_prc_cd_mst where major_flag like '%C%' and factory_cd='" + factoryCd + "' ";
            if (GarmentType != "")
            {
                SQL = SQL + "AND GARMENT_TYPE='" + GarmentType + "' ";
            }
            SQL = SQL + " AND ACTIVE='Y' ";
            SQL = SQL + " ) AS A ORDER BY DISPLAY_SEQ ";
            return DBUtility.GetTable(SQL, "MES");
        }
        //Added By ZouShiChang ON 2013.08.29 Start MES024
        public static DataTable GetProTranWIPAcceptQtyTrxBody(string factoryCd, string garmentType, string FromDate, string ToDate, bool IsOSJo)
        {
            string SQL = " if OBJECT_ID('tempdb.. #TMP_INTABLE') is not Null drop table #TMP_INTABLE; ";
            SQL = SQL + "select dbo.DATE_FORMAT(A.TRX_DATE,'YYYY-MM-DD') as TRX_DATE,a.NEXT_PROCESS_CD,sum(B.OUTPUT_QTY) as In_qty  ";
            SQL = SQL + "INTO #TMP_INTABLE ";
            SQL = SQL + "from  ";
            SQL = SQL + "PRD_JO_OUTPUT_HD a WITH(NOLOCK) ";
            SQL = SQL + "inner join PRD_JO_OUTPUT_TRX b WITH(NOLOCK) ";
            SQL = SQL + "on a.DOC_NO=b.DOC_NO ";
            SQL = SQL + "   and a.factory_cd=b.factory_cd ";
            SQL = SQL + " inner join jo_hd jo WITH(NOLOCK) on jo.jo_no=b.job_order_no and (jo.status<>'D' or (jo.status='D' and exists(select 1 from cut_bundle_hd WITH(NOLOCK) where job_order_no=jo.jo_no)))";
            SQL = SQL + "inner join GEN_PRC_CD_MST c WITH(NOLOCK) ";
            SQL = SQL + "on a.factory_cd=c.factory_cd ";
            
            SQL = SQL + "and A.NEXT_PROCESS_CD=c.prc_cd AND A.NEXT_PROCESS_GARMENT_TYPE=C.GARMENT_TYPE ";
            if (garmentType != "")
            {
                SQL = SQL + " AND GARMENT_TYPE='" + garmentType + "' ";
            }
            SQL = SQL + " and major_flag like '%C%' ";
            if (factoryCd == "GEG")
            {
                SQL = SQL + "AND (CHARINDEX('PACK',C.prc_cd)<=0 and CHARINDEX('FIN',C.prc_cd)<=0)";
            }
            SQL = SQL + " where a.factory_cd='" + factoryCd + "' ";
            SQL = SQL + "   AND Complete_Qty<>0 ";
            if (FromDate != "")
            {
                SQL = SQL + "AND  convert(char(10),A.TRX_DATE,120)>='" + FromDate + "' ";
            }
            if (ToDate != "")
            {
                SQL = SQL + "AND convert(char(10),A.TRX_DATE,120)<='" + ToDate + "' ";
            }
            if (!IsOSJo && factoryCd != "GEG")
            {
                SQL = SQL + "AND EXISTS (SELECT TOP 1 1 FROM JO_HD J WITH(NOLOCK)  ";
                SQL = SQL + "                  inner join sc_hd s WITH(NOLOCK) on s.sc_no=j.sc_no  ";
                SQL = SQL + "                  and s.SAM_GROUP_CD<>'OUTSOURCE' ";
                SQL = SQL + "                  WHERE J.JO_NO=b.JOB_ORDER_NO) ";
            }
            SQL = SQL + " AND NEXT_PROCESS_TYPE='I' AND c.ACTIVE='Y'  "; 
            SQL = SQL + "group by a.TRX_DATE,a.NEXT_PROCESS_CD ";
            SQL = SQL + "HAVING sum(B.OUTPUT_QTY)<>0; ";
            SQL = SQL + "INSERT INTO #TMP_INTABLE ";
            SQL = SQL + "select dbo.DATE_FORMAT(actual_print_date,'YYYY-MM-DD') AS TRX_DATE,b.prc_cd as NEXT_PROCESS_CD,sum(a.QTY) as In_qty ";
            SQL = SQL + "    FROM cut_bundle_hd a WITH(NOLOCK)               ";
            SQL = SQL + "    inner join GEN_PRC_CD_MST b WITH(NOLOCK) on a.factory_cd = b.factory_cd AND b.display_seq = 1 and b.major_flag like '%C%'          ";
            SQL = SQL + "    inner join jo_hd c WITH(NOLOCK) on c.jo_no=a.job_order_no AND (c.garment_type_cd=b.garment_type)                   ";
            SQL = SQL + " and (c.status<>'D' or (c.status='D' and exists(select 1 from cut_bundle_hd WITH(NOLOCK) where job_order_no=c.jo_no)))";
            SQL = SQL + "    WHERE A.Factory_cd='" + factoryCd + "'                  ";
            if (FromDate != "")
            {
                SQL = SQL + "       AND actual_print_date >='" + FromDate + "' ";
            }
            if (ToDate != "")
            {
                SQL = SQL + "       AND actual_print_date <DATEADD(d,1,'" + ToDate + "')      ";
            }
            if (garmentType != "")
            {
                SQL = SQL + "       AND C.garment_type_cd='" + garmentType + "'   ";
            }

            if (!IsOSJo)
            {
                SQL = SQL + "AND EXISTS (SELECT TOP 1 1 FROM sc_hd s WITH(NOLOCK) WHERE s.sc_no=C.sc_no  ";
                SQL = SQL + "                  and s.SAM_GROUP_CD<>'OUTSOURCE')     ";
            }
            SQL = SQL + " AND A.PROCESS_TYPE='I' AND b.ACTIVE='Y' "; //Added By ZouShiChang ON 2013.12.12
            SQL = SQL + "    group by dbo.DATE_FORMAT(actual_print_date,'YYYY-MM-DD'),B.prc_cd; ";
            if (factoryCd == "GEG")
            {
                SQL = SQL + "INSERT INTO #TMP_INTABLE ";
                SQL = SQL + "select dbo.DATE_FORMAT(A.TRX_DATE,'YYYY-MM-DD') as TRX_DATE,a.NEXT_PROCESS_CD,sum(B.OUTPUT_QTY) as In_qty from  ";
                SQL = SQL + "PRD_JO_OUTPUT_HD a WITH(NOLOCK) ";
                SQL = SQL + "inner join PRD_JO_OUTPUT_TRX b WITH(NOLOCK) ";
                SQL = SQL + "on a.DOC_NO=b.DOC_NO ";
                SQL = SQL + "   and a.factory_cd=b.factory_cd ";
                SQL = SQL + " inner join jo_hd jo on jo.jo_no=b.job_order_no and (jo.status<>'D' or (jo.status='D' and exists(select 1 from cut_bundle_hd WITH(NOLOCK) where job_order_no=jo.jo_no)))";
                SQL = SQL + "inner join GEN_PRC_CD_MST c WITH(NOLOCK) ";
                SQL = SQL + "on a.factory_cd=c.factory_cd ";

                SQL = SQL + "and A.NEXT_PROCESS_CD=c.prc_cd and A.next_process_garment_type=C.GARMENT_TYPE  ";

                SQL = SQL + "AND (CHARINDEX('PACK',C.prc_cd)>0 OR CHARINDEX('FIN',C.prc_cd)>0)";

                if (garmentType != "")
                {
                    SQL = SQL + "AND GARMENT_TYPE='" + garmentType + "' ";
                }
                SQL = SQL + " and major_flag like '%C%' ";
                SQL = SQL + "where a.factory_cd='" + factoryCd + "' ";
                SQL = SQL + "   AND Complete_Qty<>0 ";
                if (FromDate != "")
                {
                    SQL = SQL + "AND  convert(char(10),A.TRX_DATE,120)>='" + FromDate + "' ";
                }
                if (ToDate != "")
                {
                    SQL = SQL + "AND convert(char(10),A.TRX_DATE,120)<='" + ToDate + "' ";
                }
                SQL = SQL + " AND NEXT_PROCESS_TYPE='I' AND C.ACTIVE='Y'  "; //Added By ZouShiChang ON 2013.12.12
                SQL = SQL + "AND EXISTS (SELECT TOP 1 1 FROM JO_HD J WITH(NOLOCK)  ";
                SQL = SQL + "                  inner join sc_hd s on s.sc_no=j.sc_no  ";
                SQL = SQL + "                  and s.SAM_GROUP_CD<>'OUTSOURCE' ";
                SQL = SQL + "                  WHERE J.JO_NO=b.JOB_ORDER_NO) ";
                SQL = SQL + "group by a.TRX_DATE,a.NEXT_PROCESS_CD ";
                SQL = SQL + "HAVING sum(B.OUTPUT_QTY)<>0; ";
            }
            SQL = SQL + "SELECT * FROM #TMP_INTABLE ";
            SQL = SQL + "order by TRX_DATE,NEXT_PROCESS_CD; ";
            return DBUtility.GetTable(SQL, "MES");
        }


        public static DataTable GetProTranWIPAcceptQtyTrxDetail(string factoryCd, string garmentType, string processCd, string FromDate, string ToDate, bool IsOSJO)
        {
            string SQL = @" SELECT CASE WHEN B.PROCESS_TYPE ='I'
                                         THEN ISNULL(B.PROCESS_GARMENT_TYPE, '') + ISNULL(B.PROCESS_CD, '')
                                              + '('+ISNULL(B.PROCESS_TYPE, '')+')'
                                         ELSE ISNULL(B.PROCESS_GARMENT_TYPE, '')
                                              + ISNULL(B.PROCESS_CD, '') + '('+ISNULL(B.PROCESS_TYPE,
                                                                                          '')+')'
                                              + ISNULL(B.PROCESS_PRODUCTION_FACTORY, '')
                                    END AS PROCESS_CD , ";
            SQL=SQL+" b.job_order_no,sum(B.OUTPUT_QTY) as In_qty from  ";
            SQL = SQL + "PRD_JO_OUTPUT_HD a WITH(NOLOCK) ";
            SQL = SQL + "inner join PRD_JO_OUTPUT_TRX b WITH(NOLOCK) ";
            SQL = SQL + "on a.DOC_NO=b.DOC_NO ";
            SQL = SQL + "   and a.factory_cd=b.factory_cd ";
            SQL = SQL + "inner join GEN_PRC_CD_MST c WITH(NOLOCK) ";
            SQL = SQL + "on a.factory_cd=c.factory_cd ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL = SQL + "and A.NEXT_PROCESS_CD=c.prc_cd ";
            SQL = SQL + "and A.NEXT_PROCESS_CD=c.prc_cd And A.NEXT_PROCESS_GARMENT_TYPE=C.GARMENT_TYPE ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            if (garmentType != "")
            {
                SQL = SQL + "AND GARMENT_TYPE='" + garmentType + "' ";
            }
            SQL = SQL + " and major_flag like '%C%' ";
            if (processCd != "")
            {
                SQL = SQL + "AND C.prc_cd='" + processCd + "' ";
            }
            SQL = SQL + "where a.factory_cd='" + factoryCd + "' ";
            SQL = SQL + "   AND Complete_Qty<>0 ";
            if (FromDate != "")
            {
                SQL = SQL + "AND A.TRX_DATE>='" + FromDate + "' ";
            }
            if (ToDate != "")
            {
                SQL = SQL + "AND A.TRX_DATE<='" + ToDate + "' ";
            }
            if (factoryCd == "GEG" && (processCd.IndexOf("PACK") != -1 || processCd.IndexOf("FIN") != -1))
            {
                SQL = SQL + "AND EXISTS (SELECT TOP 1 1 FROM JO_HD J WITH(NOLOCK)  ";
                SQL = SQL + "                  inner join sc_hd s on s.sc_no=j.sc_no  ";
                SQL = SQL + "                  and s.SAM_GROUP_CD<>'OUTSOURCE' ";
                SQL = SQL + "                  WHERE J.JO_NO=b.JOB_ORDER_NO) ";
            }
            if (factoryCd != "GEG" && !IsOSJO)
            {
                SQL = SQL + "AND EXISTS (SELECT TOP 1 1 FROM JO_HD J WITH(NOLOCK)  ";
                SQL = SQL + "                  inner join sc_hd s on s.sc_no=j.sc_no  ";
                SQL = SQL + "                  and s.SAM_GROUP_CD<>'OUTSOURCE' ";
                SQL = SQL + "                  WHERE J.JO_NO=b.JOB_ORDER_NO) ";
            }
            SQL = SQL + " AND NEXT_PROCESS_TYPE='I' AND C.ACTIVE='Y'  "; //Added By ZouShiChang ON 2014.04.22
            SQL = SQL + "group by B.PROCESS_GARMENT_TYPE,b.PROCESS_CD,B.PROCESS_TYPE,B.PROCESS_PRODUCTION_FACTORY,b.job_order_no ";
            SQL = SQL + "HAVING sum(B.OUTPUT_QTY)<>0 ";
            SQL = SQL + "union all ";
            SQL = SQL + "select b.prc_cd as PROCESS_CD,a.job_order_no,sum(a.QTY) as In_qty ";
            SQL = SQL + "    FROM cut_bundle_hd a WITH(NOLOCK)               ";
            SQL = SQL + "    inner join GEN_PRC_CD_MST b WITH(NOLOCK) on a.factory_cd = b.factory_cd AND b.display_seq = 1          ";
            SQL = SQL + "    inner join jo_hd c WITH(NOLOCK) on c.jo_no=a.job_order_no AND (c.garment_type_cd=b.garment_type)                   ";
            SQL = SQL + "    WHERE A.Factory_cd='" + factoryCd + "'                  ";
            if (FromDate != "")
            {
                SQL = SQL + "       AND actual_print_date >='" + FromDate + "' ";
            }
            if (ToDate != "")
            {
                SQL = SQL + "       AND actual_print_date <DATEADD(d,1,'" + ToDate + "')      ";
            }
            if (garmentType != "")
            {
                SQL = SQL + "       AND C.garment_type_cd='" + garmentType + "'   ";
            }
            if (processCd != "")
            {
                SQL = SQL + "       and b.prc_cd='" + processCd + "' ";
            }
            if (!IsOSJO || factoryCd == "GEG")
            {
                SQL = SQL + "AND EXISTS (SELECT TOP 1 1 FROM sc_hd s WITH(NOLOCK) WHERE s.sc_no=C.sc_no  ";
                SQL = SQL + "                  and s.SAM_GROUP_CD<>'OUTSOURCE')     ";
            }
            SQL = SQL + " AND A.PROCESS_TYPE='I' AND B.ACTIVE='Y'  "; //Added By ZouShiChang ON 2014.04.2
            SQL = SQL + "    group by a.job_order_no,B.prc_cd ";
            SQL = SQL + "order by 2,1 ";

            //string SQL = " select b.job_order_no as JOB_ORDER_NO,sum(B.OUTPUT_QTY) as IN_QTY,b.PROCESS_CD from  ";
            //SQL = SQL + "PRD_JO_OUTPUT_HD a WITH(NOLOCK) ";
            //SQL = SQL + "inner join PRD_JO_OUTPUT_TRX b WITH(NOLOCK) ";
            //SQL = SQL + "on a.DOC_NO=b.DOC_NO ";
            //SQL = SQL + "   and a.factory_cd=b.factory_cd ";
            //SQL = SQL + "inner join GEN_PRC_CD_MST c WITH(NOLOCK) ";
            //SQL = SQL + "on a.factory_cd=c.factory_cd ";
            //SQL = SQL + "and A.NEXT_PROCESS_CD=c.prc_cd ";
            //if (garmentType != "")
            //{
            //    SQL = SQL + "AND GARMENT_TYPE='" + garmentType + "' ";
            //}
            //SQL = SQL + "and (major_flag='C' or major_flag='A,B') ";
            //SQL = SQL + "AND C.prc_cd='"+processCd+"' ";
            //SQL = SQL + "where a.factory_cd='"+factoryCd+"' ";
            //SQL = SQL + "   AND Complete_Qty<>0 ";
            //SQL = SQL + "AND  convert(char(10),A.TRX_DATE,120)>='"+FromDate+"' ";
            //SQL = SQL + "AND convert(char(10),A.TRX_DATE,120)<='"+ToDate+"' ";
            //if (processCd == "PACKK" || processCd == "PACKW")
            //{
            //    SQL = SQL + "AND EXISTS (SELECT TOP 1 1 FROM JO_HD J WITH(NOLOCK)  ";
            //    SQL = SQL + "                  inner join sc_hd s on s.sc_no=j.sc_no  ";
            //    SQL = SQL + "                  and s.SAM_GROUP_CD<>'OUTSOURCE' ";
            //    SQL = SQL + "                  WHERE J.JO_NO=b.JOB_ORDER_NO) ";
            //}
            //SQL = SQL + "group by b.PROCESS_CD,b.job_order_no ";
            //SQL = SQL + "HAVING sum(B.OUTPUT_QTY)<>0 ";
            //SQL = SQL + "order by b.job_order_no,b.PROCESS_CD ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetProTranWIPAcceptQtyDetail(string factoryCd, string DateType, string FromDate, string ToDate, string strJo, string processCd)
        {
            string SQL = "";
            SQL = SQL + "SELECT A1.JOB_ORDER_NO,convert(char(10),A1.TRX_DATE,101) as TRX_DATE, A1.PROCESS_CD,SUM(OUTPUT_QTY) AS QTY ";
            SQL = SQL + "FROM PRD_JO_OUTPUT_TRX  A1 WITH(NOLOCK),PRD_JO_OUTPUT_HD B1 WITH(NOLOCK) ";
            SQL = SQL + "WHERE A1.DOC_NO=B1.DOC_NO ";
            SQL = SQL + "AND A1.JOB_ORDER_NO ='" + strJo + "' ";
            SQL = SQL + "AND (B1.NEXT_PROCESS_CD='" + processCd + "' ) ";
            SQL = SQL + "AND A1.FACTORY_CD='" + factoryCd + "' ";
            switch (DateType)
            {
                case "TRX":
                    if (FromDate != "")
                    {
                        SQL += " and (A1.trx_date>='" + FromDate + "')";
                    }
                    if (ToDate != "")
                    {
                        SQL += " and (A1.trx_date<='" + ToDate + "')";
                    }
                    break;
                case "BPO":
                    SQL += " AND EXISTS (SELECT TOP 1 1 FROM JO_HD WITH(NOLOCK) WHERE JO_NO=A1.JOB_ORDER_NO ";
                    if (FromDate != "")
                    {
                        SQL += " AND convert(char(10),BUYER_PO_DEL_DATE,120) >='" + FromDate + "'";
                    }
                    if (ToDate != "")
                    {
                        SQL += " AND convert(char(10),BUYER_PO_DEL_DATE,120) <='" + ToDate + "'";
                    }
                    SQL += ")";
                    break;
            }
            SQL = SQL + "GROUP BY A1.JOB_ORDER_NO, A1.PROCESS_CD ,A1.TRX_DATE ";
            SQL = SQL + "ORDER BY A1.TRX_DATE";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetProDiscrepancyList(string garmentType, string factoryCd, string DateType, string FromDate, string ToDate, string JoNo, string processCd)
        {
            string SQL = "   select B.JOB_ORDER_NO,convert(char(10),A.TRX_DATE,101) as TRX_DATE,A.PROCESS_CD,SUM(ISNULL(D.PULLOUT_QTY,0)) AS QTY";
            SQL = SQL + "       from         PRD_JO_DISCREPANCY_PULLOUT_HD a WITH(NOLOCK)";
            SQL = SQL + "       JOIN         PRD_JO_DISCREPANCY_PULLOUT_TRX b WITH(NOLOCK)";
            SQL = SQL + "       ON A.DOC_NO=B.DOC_NO AND         A.FACTORY_CD=B.FACTORY_CD AND A.TRX_DATE=B.TRX_DATE ";
            SQL = SQL + "       LEFT JOIN PRD_JO_PULLOUT_REASON D WITH(NOLOCK) ON D.TRX_ID=B.TRX_ID AND D.FACTORY_CD=A.FACTORY_CD ";
            SQL = SQL + "       LEFT JOIN                  PRD_REASON_CODE E WITH(NOLOCK) ON E.REASON_CD=D.REASON_CD AND                    E.FACTORY_CD=D.FACTORY_CD ";
            SQL = SQL + "       WHERE 1=1 and a.factory_cd='" + factoryCd + "'    ";
            if (JoNo != "")
            {
                SQL += " and B.job_order_no = '" + JoNo + "'";
            }
            //if (processCd != "")
            //{
            //    SQL += " and (A.process_cd='" + processCd + "')";
            //}
            switch (DateType)
            {
                case "TRX":
                    if (FromDate != "")
                    {
                        SQL += " and (A.trx_date>='" + FromDate + "')";
                    }
                    if (ToDate != "")
                    {
                        SQL += " and (A.trx_date<='" + ToDate + "')";
                    }
                    break;
                case "BPO":
                    SQL += " AND EXISTS (SELECT TOP 1 1 FROM JO_HD WITH(NOLOCK) WHERE JO_NO=b.JOB_ORDER_NO ";
                    if (FromDate != "")
                    {
                        SQL += " AND convert(char(10),BUYER_PO_DEL_DATE,120) >='" + FromDate + "'";
                    }
                    if (ToDate != "")
                    {
                        SQL += " AND convert(char(10),BUYER_PO_DEL_DATE,120) <='" + ToDate + "'";
                    }
                    SQL += ")";
                    break;
            }
            SQL = SQL + "        GROUP BY                A.PROCESS_CD,A.TRX_DATE,B.JOB_ORDER_NO,E.SHORT_NAME,E.REASON_DESC ";
            SQL = SQL + "       ORDER BY 1,2,3";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetProPullinOutList(string garmentType, string factoryCd, string DateType, string FromDate, string ToDate, string JoNo)
        {
            string SQL = "   select  JOB_ORDER_NO,convert(char(10),TRX_DATE,101) as TRX_DATE,PROCESS_CD,PULL_OUT_JO, ";
            SQL = SQL + "                  SUM(ISNULL(PULLOUT_QTY,0)) AS PULLOUT_QTY, PULLIN_JO,";
            SQL = SQL + "                  SUM(ISNULL(PULLIN_QTY,0)) AS PULLIN_QTY   ";
            SQL = SQL + "       from ";
            SQL = SQL + "          ( select         A.PROCESS_CD,A.TRX_DATE,B.JOB_ORDER_NO,ISNULL(C.JO_NO,'') AS  PULL_OUT_JO, ";
            SQL = SQL + "       SUM(ISNULL(C.PULLOUT_QTY,0)) AS PULLOUT_QTY,'' AS PULLIN_JO,0 AS PULLIN_QTY  ";
            SQL = SQL + "       from         PRD_JO_DISCREPANCY_PULLOUT_HD a WITH(NOLOCK)";
            SQL = SQL + "       JOIN         PRD_JO_DISCREPANCY_PULLOUT_TRX b WITH(NOLOCK)";
            SQL = SQL + "       ON A.DOC_NO=B.DOC_NO AND         A.FACTORY_CD=B.FACTORY_CD AND A.TRX_DATE=B.TRX_DATE ";
            SQL = SQL + "       LEFT JOIN PRD_JO_PULLOUT C WITH(NOLOCK) ON C.TRX_ID=B.TRX_ID ";
            SQL = SQL + "       AND C.FACTORY_CD=A.FACTORY_CD ";
            SQL = SQL + "       WHERE 1=1 and a.factory_cd='" + factoryCd + "'    ";
            if (JoNo != "")
            {
                SQL += " and B.job_order_no = '" + JoNo + "'";
            }
            //if (processCd != "")
            //{
            //    SQL += " and (A.process_cd='" + processCd + "')";
            //}
            switch (DateType)
            {
                case "TRX":
                    if (FromDate != "")
                    {
                        SQL += " and (A.trx_date>='" + FromDate + "')";
                    }
                    if (ToDate != "")
                    {
                        SQL += " and (A.trx_date<='" + ToDate + "')";
                    }
                    break;
                case "BPO":
                    SQL += " AND EXISTS (SELECT TOP 1 1 FROM JO_HD WITH(NOLOCK) WHERE JO_NO=b.JOB_ORDER_NO ";
                    if (FromDate != "")
                    {
                        SQL += " AND convert(char(10),BUYER_PO_DEL_DATE,120) >='" + FromDate + "'";
                    }
                    if (ToDate != "")
                    {
                        SQL += " AND convert(char(10),BUYER_PO_DEL_DATE,120) <='" + ToDate + "'";
                    }
                    SQL += ")";
                    break;
            }

            SQL = SQL + "        GROUP BY                A.PROCESS_CD,A.TRX_DATE,B.JOB_ORDER_NO,C.JO_NO  ";
            SQL = SQL + "       UNION ALL";
            SQL = SQL + "                select         A.PROCESS_CD,A.TRX_DATE, C.JO_NO AS  JOB_ORDER_NO,  ";
            SQL = SQL + "       '' AS PULL_OUT_JO,0 AS PULLOUT_QTY,B.JOB_ORDER_NO AS PULLIN_JO, SUM(ISNULL(C.PULLOUT_QTY,0)) AS PULLIN_QTY";
            SQL = SQL + "       from         PRD_JO_DISCREPANCY_PULLOUT_HD a WITH(NOLOCK) ";
            SQL = SQL + "       JOIN         PRD_JO_DISCREPANCY_PULLOUT_TRX b WITH(NOLOCK) ";
            SQL = SQL + "       ON A.DOC_NO=B.DOC_NO AND         A.FACTORY_CD=B.FACTORY_CD AND A.TRX_DATE=B.TRX_DATE ";
            SQL = SQL + "       JOIN JO_HD  PO WITH(NOLOCK) ON B.JOB_ORDER_NO=PO.JO_NO         ";
            SQL = SQL + "       JOIN PRD_JO_PULLOUT C WITH(NOLOCK) ON C.TRX_ID=B.TRX_ID ";
            SQL = SQL + "       AND           C.FACTORY_CD=A.FACTORY_CD ";
            SQL = SQL + "       WHERE 1=1 and a.factory_cd='" + factoryCd + "' ";
            if (JoNo != "")
            {
                SQL += " and C.JO_NO = '" + JoNo + "'";
            }
            //if (processCd != "")
            //{
            //    SQL += " and (A.process_cd='" + processCd + "')";
            //}
            switch (DateType)
            {
                case "TRX":
                    if (FromDate != "")
                    {
                        SQL += " and (A.trx_date>='" + FromDate + "')";
                    }
                    if (ToDate != "")
                    {
                        SQL += " and (A.trx_date<='" + ToDate + "')";
                    }
                    break;
                case "BPO":
                    SQL += " AND EXISTS (SELECT TOP 1 1 FROM JO_HD WITH(NOLOCK) WHERE JO_NO=c.JO_NO ";
                    if (FromDate != "")
                    {
                        SQL += " AND convert(char(10),BUYER_PO_DEL_DATE,120) >='" + FromDate + "'";
                    }
                    if (ToDate != "")
                    {
                        SQL += " AND convert(char(10),BUYER_PO_DEL_DATE,120) <='" + ToDate + "'";
                    }
                    SQL += ")";
                    break;
            }
            SQL = SQL + "        GROUP BY                A.PROCESS_CD,A.TRX_DATE,B.JOB_ORDER_NO,C.JO_NO ) AAA ";
            SQL = SQL + "        where 1=1   ";
            SQL = SQL + "       GROUP BY PROCESS_CD,TRX_DATE,JOB_ORDER_NO,";
            SQL = SQL + "                  PULL_OUT_JO,PULLIN_JO";
            SQL = SQL + " HAVING SUM(ISNULL(PULLOUT_QTY,0))+SUM(ISNULL(PULLIN_QTY,0))<>0";
            SQL = SQL + "       ORDER BY 1,2,3";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetProTranWIPProductQty(string factoryCd, string strGo, bool isGoLike, string strJo, string strDept, string GarmentType, string ProcessType,string ProdFactory,string strPart, string startDate, string endDate, string type, bool isOSJo, bool isIncludeOutSourceProcessOutput)
        {
            string SQL = "";
            switch (type)
            {
                case "Title":
                    SQL = " SELECT distinct a.process_cd,display_seq ";
                    break;
                case "Content":
                    SQL = " SELECT convert(varchar(10),a.TRX_DATE,120) as TRX_DATE,a.PROCESS_CD,sum(output_qty) as qty ";
                    break;
                case "Detail":
                    if (strDept.IndexOf("WASH") == -1 && (strDept.IndexOf("WSH") == -1 || strDept.IndexOf("WET") == -1))
                    {
                        SQL = " SELECT job_order_no,sum(output_qty) as qty ";
                    }
                    else
                    {
                        SQL = " select m.job_order_no,m.qty_n as wrinkle_free,m.qty_y as no_wrinkle_free,(m.qty_n+m.qty_y) as qty from ( SELECT job_order_no,sum(output_qty) as qty_N,0 as qty_Y ";
                    }
                    break;
                case "Line":
                    SQL = SQL + " SELECT a.process_cd,A.Process_GARMENT_TYPE,A.PROCESS_TYPE,(case a.production_line_cd when '' then 'NA' else a.production_line_cd end) as production_line_cd,sum(output_qty) as qty,d.sc_no INTO #tmp_process_qty ";
                    break;
            }
            SQL = SQL + "FROM PRD_JO_OUTPUT_TRX a with(nolock),jo_hd b with(nolock),gen_prc_cd_mst c with(nolock),sc_hd d with(nolock),PRD_JO_OUTPUT_HD HD with(nolock)";
            SQL = SQL + " where a.doc_no=hd.doc_no and  a.factory_cd=b.factory_cd and a.job_order_no=b.jo_no  ";
            SQL = SQL + " and a.factory_cd=c.factory_cd and a.process_cd=c.prc_cd AND A.PROCESS_GARMENT_TYPE=C.GARMENT_TYPE  and b.sc_no=d.sc_no ";
            SQL = SQL + " AND A.FACTORY_CD=CASE WHEN A.PROCESS_TYPE='P' THEN B.CO_FACTORY_CD ELSE A.FACTORY_CD END  ";//Added By ZouShiChang ON 2014.02.13 屏蔽P的数据

            if (!isIncludeOutSourceProcessOutput)
            {
                SQL = SQL + @"  and  exists 
                     (
		                     select  1 from prd_fty_process_flow f 
		                     where f.factory_cd=a.factory_cd and  case when isnull(FTY_OUTPUT,'')='' then 'N/A' else FTY_OUTPUT end <>'N/A'		  
		                      and a.process_cd=f.process_cd and HD.next_process_cd=f.next_process_cd 
                              and a.PROCESS_GARMENT_TYPE=F.PROCESS_GARMENT_TYPE  AND A.PROCESS_TYPE=F.PROCESS_TYPE and  a.PROCESS_PRODUCTION_FACTORY=f.PROCESS_PRODUCTION_FACTORY
                              AND HD.NEXT_PROCESS_GARMENT_TYPE=f.PROCESS_GARMENT_TYPE AND HD.NEXT_PROCESS_TYPE=F.NEXT_PROCESS_TYPE AND  hd.NEXT_PROCESS_PRODUCTION_FACTORY=f.NEXT_PROCESS_PRODUCTION_FACTORY
                      )";
            }

            if (factoryCd != "")
            {
                SQL = SQL + " and a.factory_cd='" + factoryCd + "'";
            }
            if (strGo != "")
            {
                if (isGoLike)
                {
                    SQL = SQL + " and b.sc_no like '%" + strGo + "%' ";
                }
                else
                {
                    SQL = SQL + " and b.sc_no = '" + strGo + "' ";
                }
            }
            if (strJo != "")
            {
                SQL = SQL + " and job_order_no='" + strJo + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL = SQL + " AND A.PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactory.Equals(""))
            {
                SQL = SQL + " And A.PROCESS_PRODUCTION_FACTORY='" + ProdFactory + "' ";
            }
            //if (strDept != "") //Added By ZouSHiChang On 2014.04.02 
            //{
                if (strDept != "KeyPart")
                {
                    if (strDept != "")
                    {
                        SQL = SQL + " and a.process_cd='" + strDept + "'";
                    }
                    if (!isOSJo)
                    {
                        if (factoryCd == "GEG")
                        {
                            if (strDept.IndexOf("WASH") == -1 && (strDept.IndexOf("WSH") == -1 || strDept.IndexOf("WET") == -1))
                            {
                                SQL = SQL + " and d.SAM_GROUP_CD<>'OUTSOURCE' ";
                            }
                        }
                        else
                        {
                            SQL = SQL + " and d.SAM_GROUP_CD<>'OUTSOURCE' ";
                        }
                    }
                }
                else
                {
                    if (!isOSJo)
                    {
                        if (factoryCd == "GEG")
                        {
                            if (strDept.IndexOf("WASH") == -1 && (strDept.IndexOf("WSH") == -1 || strDept.IndexOf("WET") == -1))
                            {
                                SQL = SQL + " and d.SAM_GROUP_CD<>'OUTSOURCE' ";
                            }
                        }
                        else
                        {
                            SQL = SQL + " and d.SAM_GROUP_CD<>'OUTSOURCE' ";
                        }
                    }
                    SQL = SQL + " and  exists (select 1  from gen_prc_cd_mst where major_flag like '%B%' and factory_cd=a.FACTORY_CD and prc_cd=a.process_cd ";
                    SQL = SQL + "and garment_type=b.garment_type_cd )";
                }
            //} //Added By ZouSHiChang On 2014.04.02 
            if (GarmentType != "")
            {
                SQL += " and b.garment_type_cd='" + GarmentType + "'";
            }
            if (strPart != "")
            {
                //SQL = SQL + " and a.production_line_cd='" + strPart + "'";
                SQL = SQL + (strPart == "NA" ? " and (a.production_line_cd='' OR a.production_line_cd='" + strPart + "')" : " and a.production_line_cd='" + strPart + "'");
            }        
            if (startDate != "")
            {
                SQL = SQL + " and a.trx_date>='" + startDate + "' ";
            }
            if (endDate != "")
            {
                SQL = SQL + " and a.trx_date<='" + endDate + "'  ";
            }
            switch (type)
            {
                case "Title":
                    SQL = SQL + "order by c.display_seq ";
                    break;
                case "Content":
                    SQL = SQL + "group by a.trx_date,a.process_cd ";
                    SQL = SQL + "order by a.trx_date ";
                    break;
                case "Detail":
                    if (strDept.IndexOf("WASH") == -1 && (strDept.IndexOf("WSH") == -1 || strDept.IndexOf("WET") == -1))
                    {
                        SQL = SQL + "group by a.job_order_no ";
                        SQL = SQL + "order by a.job_order_no ";
                    }
                    else
                    {
                        SQL = SQL + " AND EXISTS(SELECT TOP 1 1 FROM gen_wash_type W where wash_grp_cd='WRINKLE_FREE' AND W.WASH_TYPE_CD= b.wash_type_cd) ";
                        SQL = SQL + " group by a.job_order_no ";
                        SQL = SQL + "  union all SELECT job_order_no,0 as qty_N,sum(output_qty) as qty_Y ";
                        SQL = SQL + "FROM PRD_JO_OUTPUT_TRX a,jo_hd b,gen_prc_cd_mst c,sc_hd d";
                        SQL = SQL + " where a.factory_cd=b.factory_cd and a.job_order_no=b.jo_no  ";
                        SQL = SQL + " and a.factory_cd=c.factory_cd and a.process_cd=c.prc_cd AND A.PROCESS_GARMENT_TYPE=C.GARMENT_TYPE AND  b.sc_no=d.sc_no ";
                        if (factoryCd != "")
                        {
                            SQL = SQL + " and a.factory_cd='" + factoryCd + "'";
                        }
                        if (strGo != "")
                        {
                            if (isGoLike)
                            {
                                SQL = SQL + " and b.sc_no like '%" + strGo + "%' ";
                            }
                            else
                            {
                                SQL = SQL + " and b.sc_no = '" + strGo + "' ";
                            }
                        }
                        if (strJo != "")
                        {
                            SQL = SQL + " and job_order_no='" + strJo + "' ";
                        }
                        if (!ProcessType.Equals(""))
                        {
                            SQL = SQL + " AND A.PROCESS_TYPE='" + ProcessType + "' ";
                        }
                        if (!ProdFactory.Equals(""))
                        {
                            SQL = SQL + " And A.PROCESS_PRODUCTION_FACTORY='" + ProdFactory + "' ";
                        }
                        if (strDept != "")
                        {
                            if (strDept != "KeyPart")
                            {
                                SQL = SQL + " and a.process_cd='" + strDept + "'";
                                if (!isOSJo)
                                {
                                    if (factoryCd == "GEG")
                                    {
                                        if (strDept.IndexOf("WASH") == -1 && (strDept.IndexOf("WSH") == -1 || strDept.IndexOf("WET") == -1))
                                        {
                                            SQL = SQL + " and d.SAM_GROUP_CD<>'OUTSOURCE' ";
                                        }
                                    }
                                    else
                                    {
                                        SQL = SQL + " and d.SAM_GROUP_CD<>'OUTSOURCE' ";
                                    }
                                }
                            }
                            else
                            {
                                if (!isOSJo)
                                {
                                    if (factoryCd == "GEG")
                                    {
                                        if (strDept.IndexOf("WASH") == -1 && (strDept.IndexOf("WSH") == -1 || strDept.IndexOf("WET") == -1))
                                        {
                                            SQL = SQL + " and d.SAM_GROUP_CD<>'OUTSOURCE' ";
                                        }
                                    }
                                    else
                                    {
                                        SQL = SQL + " and d.SAM_GROUP_CD<>'OUTSOURCE' ";
                                    }
                                }
                                SQL = SQL + " and  exists (select 1  from gen_prc_cd_mst where major_flag like '%B%' and factory_cd=a.FACTORY_CD and prc_cd=a.process_cd ";
                                SQL = SQL + "and garment_type=b.garment_type_cd )";
                            }
                        }
                        if (GarmentType != "")
                        {
                            SQL += " and b.garment_type_cd='" + GarmentType + "'";
                        }
                        if (strPart != "")
                        {
                            //SQL = SQL + " and production_line_cd='" + strPart + "'";
                            SQL = SQL + (strPart == "NA" ? " and (production_line_cd='' OR production_line_cd='" + strPart + "')" : " and  production_line_cd='" + strPart + "'");                           
                        }
                        if (startDate != "")
                        {
                            SQL = SQL + " and a.trx_date>='" + startDate + "' ";
                        }
                        if (endDate != "")
                        {
                            SQL = SQL + " and a.trx_date<='" + endDate + "'  ";
                        }
                        SQL = SQL + " AND EXISTS(SELECT TOP 1 1 FROM gen_wash_type W where wash_grp_cd<>'WRINKLE_FREE' AND W.WASH_TYPE_CD= b.wash_type_cd) ";
                        SQL = SQL + " group by a.job_order_no ) m";
                        SQL = SQL + " order by m.job_order_no ";
                    }
                    break;
                case "Line":
                    SQL = SQL + "group by c.display_seq,a.process_cd,A.PROCESS_GARMENT_TYPE,A.PROCESS_TYPE,a.production_line_cd,d.sc_no ";
                    SQL = SQL + "order by c.display_seq,a.production_line_cd ";
                    SQL = SQL + " select b.process_cd,b.production_line_cd,sum(b.qty) as qty,sah_prod=sum(round((b.qty/12.00*h.sah),2)) ";
                    SQL = SQL + "from  ";
                    SQL = SQL + "#tmp_process_qty b join gen_prc_cd_mst g with(nolock) on b.process_cd=g.prc_cd and b.process_garment_type=g.garment_Type  and g.factory_cd='" + factoryCd + "' ";
                    SQL = SQL + "left join  ";
                    SQL = SQL + "(select a.sc_no,sah=isnull((case when (exists(select fty_type from  dbo.sc_sam with(nolock) where type='S'  ";
                    SQL = SQL + "and sah is not null and sah<>0  and sc_no=a.sc_no)) then (select sum(sah) from dbo.sc_sam with(nolock) where  type='S'  ";
                    SQL = SQL + "and sc_no=a.sc_no) else (select sum(sah) from dbo.sc_sam with(nolock)  ";
                    SQL = SQL + "where type='A' and sc_no=a.sc_no ) end),0) from sc_sam a with(nolock) where  ";
                    SQL = SQL + "exists (select top 1 1 from #tmp_process_qty t with(nolock) where t.sc_no=a.sc_no) ";
                    SQL = SQL + " group by a.sc_no) h ";
                    SQL = SQL + "on h.sc_no=b.sc_no ";
                    SQL = SQL + "where b.qty<>0 group by g.display_seq,b.process_cd,b.production_line_cd ";
                    SQL = SQL + "order by g.display_seq,b.production_line_cd; ";


                    break;
            }
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetProTranWIPSummaryDept(string factoryCd, string garmentType)
        {
            string SQL = " select '' PRC_CD,'All' NM union all select 'KeyPart' PRC_CD,'KeyProcess(主要部门)' NM union all SELECT DISTINCT PRC_CD,NM FROM GEN_PRC_CD_MST  A ";
            SQL = SQL + "WHERE 1=1 ";
            if (factoryCd != "")
            {
                SQL += " and FACTORY_CD='" + factoryCd + "' ";
            }
            if (garmentType != "")
            {
                SQL += " and a.garment_type='" + garmentType + "'";
            }
            SQL = SQL + "AND EXISTS(SELECT TOP 1 1 fROM dbo.PRD_FTY_PROCESS_FLOW B ";
            SQL = SQL + "WHERE A.ACTIVE='Y' AND B.PROCESS_CD=A.PRC_CD AND B.PROCESS_GARMENT_TYPE=A.GARMENT_TYPE  AND B.FACTORY_CD=A.FACTORY_CD) ";
            
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetProTranWIPSummaryProductionLine(string factoryCd)
        {
            string SQL = "";
            SQL = " select '' as production_line_cd,'All' as production_line_name union all select a.production_line_cd,a.production_line_name from gen_production_line a,gen_process_wip_control_master b   ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL = SQL + "where a.factory_cd=b.factory_cd and a.process_cd=b.process_cd and b.query_line_status='Y' and a.fr_production_line_cd is not null ";
            SQL = SQL + "where a.factory_cd=b.factory_cd and a.process_cd=b.process_cd  AND A.GARMENT_TYPE_CD=A.GARMENT_TYPE and b.query_line_status='Y' and a.fr_production_line_cd is not null ";
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            SQL = SQL + "and a.factory_cd='" + factoryCd + "' ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetProTranWIPSummaryProductionLine(string factoryCd, string garmentType, string processCd)
        {
            string SQL = "";
            SQL = " select '' as production_line_cd,'All' as production_line_name union all select distinct a.production_line_cd,a.production_line_name from gen_production_line a  ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL = SQL + "where a.factory_cd=b.factory_cd and a.process_cd=b.process_cd And A.GARMENT_TYPE_CD=B.GARMENT_TYPE and b.query_line_status='Y' and a.fr_production_line_cd is not null ";
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            SQL = SQL + "where ISNULL(a.ACTIVE,'Y')='Y' AND (a.LINE_TYPE='GTN' OR ISNULL(a.LINE_TYPE,'')='')"; //Added by Jin Song 2014-08-16 (Production Line similar with GTN requested by Hong Wei)
            SQL = SQL + "and a.factory_cd='" + factoryCd + "' ";
            if (garmentType != "")
            {
                SQL = SQL + " and a.garment_type_cd='" + garmentType + "'";
            }
            if (processCd != "")
            {
                SQL = SQL + " and a.process_cd='" + processCd + "'";
            }
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetProTranWIPSummaryProductCat(string facotryCd)
        {
            string SQL = " select '' as PRODUCT_CATEGORY union all select distinct sc.PRODUCT_CATEGORY from sc_hd SC iNNER JOIN  jo_hd j ";
            SQL = SQL + "ON J.SC_NO=SC.SC_NO  ";
            if (facotryCd != "")
            {
                SQL = SQL + "and j.factory_cd='" + facotryCd + "'  ";
            }
            SQL = SQL + "AND  J.BUYER_PO_DEL_DATE >= convert(varchar(10),dateadd(yy,-1,getdate()),101) ";
            SQL = SQL + "and BUYER_PO_DEL_DATE<=convert(varchar(10),DATEADD(yy,1,getdate()),101) ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetProTranWIPSummaryJoList(string factoryCd, string strGarmentType, string strJo, bool isJoLike, string strGo, bool isGoLike, string customer_cd, bool isCustomerLike, string DateType, string beginDate, string endDate, string StyleNo, bool isStyleLike, string ProductCat, string strType)
        {
            string SQL = " select A.JO_NO ";
            SQL = SQL + "FROM JO_HD A WITH(NOLOCK)";
            SQL = SQL + "INNER JOIN  ";
            SQL = SQL + "SC_HD S WITH(NOLOCK)";
            SQL = SQL + "ON S.SC_NO=A.SC_NO ";
            SQL = SQL + " join Jo_hd p  WITH(NOLOCK)";
            SQL = SQL + "on p.JO_NO=a.jo_no ";
            //SQL = SQL + "WHERE 1=1 and (a.status<>'D' and exists(select 1 from CUT_BUNDLE_HD where job_order_no=a.jo_no)) ";
            //SQL = SQL + "WHERE 1=1 and exists(select 1 from CUT_BUNDLE_HD where job_order_no=a.jo_no) ";
            //update by zhanghao 2012/3/15 LiHongWei Request(没开裁且被删除的单不再显示)
            SQL = SQL + "where 1=1 and (a.status<>'D' or (a.status='D' and exists(select 1 from cut_bundle_hd WITH(NOLOCK) where job_order_no=a.jo_no))) AND NOT EXISTS(SELECT ORIGINAL_JO_NO FROM JO_COMBINE_MAPPING WITH(NOLOCK) WHERE ORIGINAL_JO_NO=A.JO_NO)";
            if (factoryCd != "")
            {
                SQL = SQL + " and a.factory_cd='" + factoryCd + "' ";
            }
            if (strGarmentType != "")
            {
                SQL = SQL + " and a.garment_type_cd='" + strGarmentType + "' ";
            }
            if (strJo != "")
            {
                if (isJoLike)
                {
                    SQL = SQL + "AND A.JO_NO LIKE '%" + strJo + "%' ";
                }
                else
                {
                    SQL = SQL + "AND A.JO_NO = '" + strJo + "' ";
                }
            }
            if (strGo != "")
            {
                if (isGoLike)
                {
                    SQL = SQL + "AND A.SC_NO like '%" + strGo + "%' ";
                }
                else
                {
                    SQL = SQL + "AND A.SC_NO = '" + strGo + "' ";
                }
            }
            if (customer_cd != "")
            {
                if (isCustomerLike)
                {
                    SQL = SQL + "AND exists( select 1 from dbo.GEN_CUSTOMER where customer_cd=A.CUSTOMER_CD and short_name like '%" + customer_cd + "%') ";
                }
                else
                {
                    SQL = SQL + "AND exists( select 1 from dbo.GEN_CUSTOMER where customer_cd=A.CUSTOMER_CD and short_name='" + customer_cd + "') ";
                }
            }
            switch (DateType)
            {
                case "BPO":
                    if (beginDate != "")
                    {
                        SQL = SQL + "AND A.BUYER_PO_DEL_DATE >='" + beginDate + "' ";
                    }
                    if (endDate != "")
                    {
                        SQL = SQL + "AND A.BUYER_PO_DEL_DATE <='" + endDate + "' ";
                    }
                    break;
                case "PPC":
                    if (beginDate != "")
                    {
                        SQL = SQL + "AND A.prod_completion_date >='" + beginDate + "' ";
                    }
                    if (endDate != "")
                    {
                        SQL = SQL + "AND A.prod_completion_date <='" + endDate + "' ";
                    }
                    break;
            }
            if (StyleNo != "")
            {
                if (isStyleLike)
                {
                    SQL = SQL + "AND p.CUST_STYLE_NO like '%" + StyleNo + "%' ";
                }
                else
                {
                    SQL = SQL + "AND p.CUST_STYLE_NO = '" + StyleNo + "' ";
                }
            }
            if (ProductCat != "")
            {
                SQL = SQL + "AND S.PRODUCT_CATEGORY='" + ProductCat + "' ";
            }
            switch (strType)
            {
                case "0":
                    SQL = SQL + "and a.BUYER_PO_DEL_DATE >= convert(varchar(10),dateadd(MM,-6,getdate()),101) ";
                    SQL = SQL + "and a.BUYER_PO_DEL_DATE<=convert(varchar(10),dateadd(MM,6,getdate()),101) ";
                    break;
                case "1":
                    SQL = SQL + "AND NOT EXISTS (SELECT TOP 1 1 FROM GEN_JOB_ORDER_MASTER C ";
                    SQL = SQL + "WHERE C.FACTORY_CD='" + factoryCd + "'  AND C.PROD_COMPLETED='Y' AND C.JOB_ORDER_NO=A.JO_NO) ";
                    break;
            }
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetProTranWIPSummaryTransInfo(string factoryCd, string strJo, string strColor, string strSize)
        {
            string tableName = "";
            Random ro = new Random(1000);
            tableName = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
            string SQL = "exec [dbo].[PROC_INQ_WIP_BY_JOCOLORSIZE_NEW] '" + tableName + "','" + factoryCd + "','" + strJo + "','" + strColor + "','" + strSize + "','N';";
            SQL += " update " + tableName + " set totalcutqty=b.qty from " + tableName + " a left  join jo_dt b on a.jo COLLATE DATABASE_DEFAULT =b.jo_no  and a.color COLLATE DATABASE_DEFAULT =b.color_code  and (a.size COLLATE DATABASE_DEFAULT =(b.size_code1 + (case when b.size_code2<>'-' then ' ' +b.size_code2 else '' end) ))";
            SQL = SQL + " insert " + tableName + " (JO,COLOR,[SIZE],TOTALCUTQTY) ";
            SQL = SQL + "SELECT JO_NO,COLOR_CODE,SIZE_CODE=(CASE WHEN SIZE_CODE2='-' THEN SIZE_CODE1 ELSE SIZE_CODE1 + ' ' +SIZE_CODE2 END),SUM(QTY) AS ORDER_QTY ";
            SQL = SQL + "FROM jo_dt WHERE JO_NO='" + strJo + "' ";
            SQL = SQL + "AND NOT EXISTS (SELECT TOP 1 1 FROM " + tableName + " A ";
            SQL = SQL + "WHERE A.JO COLLATE DATABASE_DEFAULT =jo_dt.JO_NO  AND A.COLOR COLLATE DATABASE_DEFAULT =jo_dt.COLOR_CODE  ";
            SQL = SQL + "AND A.[SIZE] COLLATE DATABASE_DEFAULT =(CASE WHEN jo_dt.SIZE_CODE2='-' THEN jo_dt.SIZE_CODE1 ELSE jo_dt.SIZE_CODE1 + ' ' +jo_dt.SIZE_CODE2 END))  ";
            SQL = SQL + "GROUP BY JO_NO,COLOR_CODE,SIZE_CODE1,SIZE_CODE2 ";
            SQL = SQL + " select * from " + tableName + " order by jo,color,size";
            return DBUtility.GetTable(SQL, "MES_UPDATE");
        }
        public static DataTable GetProTranWIPSummaryTransSummary(string factoryCd, string strGo)
        {
            string tableName = "";
            Random ro = new Random(1000);
            tableName = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
            string SQL = " if exists (select 1 from tempdb..sysobjects where id = object_id('tempdb..##tmpjo'))";
            SQL += " drop table ##tmpjo;";
            SQL += " select jo_no into ##tmpjo from (select jo_no from jo_hd where sc_no like '%" + strGo + "%') A ";
            SQL += " exec PROC_INQ_Transaction '" + tableName + "','" + factoryCd + "','','N','','','','O','','Detail','D','','','','','N','##tmpjo';";
            SQL += " select * from " + tableName + " order by 1";
            return DBUtility.GetTable(SQL, "MES_UPDATE");
        }

        public static DataTable GetProTranWIPSummaryByColor(string factoryCd, string strJo, string strColor)
        {
            string tableName = "";
            Random ro = new Random(1000);
            tableName = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
            string SQL = "exec [dbo].[PROC_INQ_WIP_BY_COLOR] '" + tableName + "','" + factoryCd + "','" + strJo + "','" + strColor + "','%';";
            SQL += " update " + tableName + " set totalcutqty=(select sum(qty) from jo_dt b where b.jo_no=a.jo and (a.color=b.color_code or a.color='' )  ) from " + tableName + " a";
            SQL += " update " + tableName + " set CUTQTY=(SELECT ISNULL(SUM(IN_QTY),0) AS IN_QTY FROM PRD_JO_WIP_HD AS B WHERE b.JOB_ORDER_NO=a.jo and (a.color=b.color_code or a.color='') and Process_cd='CUT') from " + tableName + " a";
            SQL += " insert " + tableName + " (JO,COLOR,SIZE,TOTALCUTQTY) ";
            SQL += " SELECT JO_NO,COLOR_CODE,'',SUM(QTY) AS ORDER_QTY FROM jo_dt WHERE JO_NO='" + strJo + "' AND NOT EXISTS (SELECT TOP 1 1 FROM " + tableName + " A ";
            SQL += " WHERE A.JO=jo_dt.JO_NO  AND A.COLOR=jo_dt.COLOR_CODE  )  GROUP BY JO_NO,COLOR_CODE; ";
            SQL = SQL + " select * from " + tableName + " order by jo,color";
            return DBUtility.GetTable(SQL, "MES_UPDATE");
        }
        public static DataTable GetProTranWIPSummaryBasicInfo(string strJo, string strculture)
        {
            //string SQL = " SELECT CU.SHORT_NAME, JOCC=(select sum(qty) as order_qty From jo_dt where JO_NO='" + strJo + "'),'-' + convert(varchar(10),L.PERCENT_SHORT_ALLOWED) + '%/+' + convert(varchar(10),L.PERCENT_OVER_ALLOWED) + '%' as shipallowed,DBO.DATE_FORMAT(PO.BUYER_PO_DEL_DATE,'YYYY-MM-DD') BUYER_PO_DEL_DATE, ";
            //SQL = SQL + "L.MARKET_CD,L.SHIP_MODE_CD, PO.GARMENT_TYPE_CD, P.CUST_STYLE_NO,PRINTING,EMBROIDERY , ";
            //SQL = SQL + "SC.WASH_TYPE_CD,";
            //if (strculture == "en")
            //{
            //    SQL = SQL + "SC.STYLE_DESC as STYLE_CHN_DESC ";
            //}
            //else
            //{
            //    SQL = SQL + "SC.STYLE_CHN_DESC ";
            //}
            //SQL = SQL + " FROM  JO_HD PO with(nolock) INNER JOIN SC_HD SC  with(nolock)  ON SC.SC_NO=PO.SC_NO ";
            //SQL = SQL + "INNER JOIN SC_LOT L  with(nolock)  ";
            //SQL = SQL + "ON L.SC_NO=SC.SC_NO ";
            //SQL = SQL + "AND L.SC_NO=PO.SC_NO ";
            //SQL = SQL + "AND L.LOT_NO=PO.LOT_NO ";
            //SQL = SQL + "LEFT JOIN GEN_CUSTOMER CU  with(nolock) ON  ";
            //SQL = SQL + "PO.CUSTOMER_CD=CU.CUSTOMER_CD ";
            //SQL = SQL + "left join Jo_hd p  with(nolock)  ";
            //SQL = SQL + "on p.JO_NO=po.jo_no ";
            //SQL = SQL + "WHERE PO.JO_NO='" + strJo + "' ";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat(" with jocombine  as(select * from JO_HD with(nolock) where JO_NO='{0}'  and LOT_NO<0),", strJo);
            sb.AppendFormat(" originaljo as(select top 1 * from JO_COMBINE_MAPPING with(nolock) where COMBINE_JO_NO='{0}'),", strJo);
            sb.AppendFormat(" johd       as(select *,JO_NO as COMBINE_JO_NO from JO_HD with(nolock) where JO_NO='{0}'  and LOT_NO>0", strJo);
            sb.Append(" union all ");
            sb.Append(" select a.*,b.COMBINE_JO_NO from JO_HD a with(nolock)  inner join originaljo b on a.JO_NO=b.ORIGINAL_JO_NO) ,");
            sb.AppendFormat(" dtqty as (select sum(qty) as order_qty,JO_NO  from jo_dt where JO_NO='{0}' group by  JO_NO)", strJo);
            sb.Append(" select  CU.SHORT_NAME, ");
            sb.Append(" JOCC=b.order_qty,'-' + convert(varchar(10),L.PERCENT_SHORT_ALLOWED) + '%/+' + ");
            sb.Append("convert(varchar(10),L.PERCENT_OVER_ALLOWED) + '%' as shipallowed,DBO.DATE_FORMAT(PO.BUYER_PO_DEL_DATE,'YYYY-MM-DD') BUYER_PO_DEL_DATE, ");
            sb.AppendFormat(" L.MARKET_CD,L.SHIP_MODE_CD, PO.GARMENT_TYPE_CD, PO.CUST_STYLE_NO,PRINTING,EMBROIDERY , SC.WASH_TYPE_CD,{0} from johd PO inner join dtqty b on PO.COMBINE_JO_NO=b.JO_NO ",
                (strculture == "en" ? "SC.STYLE_DESC as STYLE_CHN_DESC " : "SC.STYLE_CHN_DESC "));
            sb.Append(" INNER JOIN SC_HD SC  with(nolock)  ON SC.SC_NO=PO.SC_NO ");
            sb.Append(" INNER JOIN SC_LOT L  with(nolock)  ON L.SC_NO=SC.SC_NO AND L.SC_NO=PO.SC_NO AND L.LOT_NO=PO.LOT_NO ");
            sb.Append(" LEFT JOIN GEN_CUSTOMER CU  with(nolock) ON  PO.CUSTOMER_CD=CU.CUSTOMER_CD ");
            return DBUtility.GetTable(sb.ToString(), "MES");
        }
        public static bool GetProTranWIPJOStatus(string strJo)
        {
            string sql = "select 1 from jo_hd where jo_no='" + strJo + "' and status='D'";
            if (DBUtility.GetTable(sql, "MES").Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
       
        //public static DataTable GetProTranWIPJoColorAndSize(string factoryCd, string strDept, string strJo)
        //{
        //    string SQL = "";
        //    SQL = SQL + "exec dbo.GET_JO_PROCESS_WIP_DETAIL '" + factoryCd + "','" + strJo + "','" + strDept + "','Y','N','N','N'";
        //    return DBUtility.GetTable(SQL, "MES_UPDATE");
        //}
        //Added By ZouShiChang On 2013.10.14 Start MES024
        public static DataTable GetProTranWIPJoColorAndSizeNew(string factoryCd, string strDept, string strJo, string line)
        {
            //MES-093
            string SQL = @"            
                        SELECT DISTINCT a.COLOR_CODE AS COLOR_CD,
                                a.SIZE_CODE AS SIZE_CD,
                                SUM(a.WIP) AS IN_BALANCE";

            if (line != "")
            {
                SQL = SQL + " ,c.SHORT_NAME, b.STYLE_NO, a.PRODUCTION_LINE_CD";
            }
            else
            {
                SQL = SQL + " ,'' AS SHORT_NAME, '' AS STYLE_NO, '' AS PRODUCTION_LINE_CD";
            }

            SQL = SQL + " FROM dbo.PRD_JO_WIP_HD a";

            if (line != "") 
            {
                SQL = SQL + " INNER JOIN JO_HD b ON a.JOB_ORDER_NO = b.JO_NO INNER JOIN GEN_CUSTOMER c ON b.CUSTOMER_CD = c.CUSTOMER_CD";
            }

            SQL = SQL + @" WHERE a.FACTORY_CD = '"+factoryCd+@"'
                            AND a.JOB_ORDER_NO = '"+strJo+@"'
                            AND a.PROCESS_CD = '"+strDept+@"'
                            AND a.WIP<>0";

            if (line != "") 
            {
                if (!line.Contains("All"))
                {
                    string[] lineSplit = line.Split(',');
                    SQL = SQL + " and a.PRODUCTION_LINE_CD in (";

                    for (int i = 0; i < lineSplit.Count(); i++)
                    {
                        if (i != lineSplit.Count() - 1)
                        {
                            SQL = SQL + "'" + lineSplit[i] + "',";
                        }
                        else
                        {
                            SQL = SQL + "'" + lineSplit[i] + "'";
                        }
                    }
                    SQL = SQL + " )";
                }
                SQL = SQL + " GROUP BY a.COLOR_CODE, a.SIZE_CODE, c.SHORT_NAME, b.STYLE_NO, a.PRODUCTION_LINE_CD";
            }
            else
            {
                SQL = SQL + " GROUP BY a.COLOR_CODE ,a.SIZE_CODE ";   
            }
            //MES-093 END
            return DBUtility.GetTable(SQL, "MES");
        }
        //Added By ZouShiChang On 2013.10.14 End MES024
        public static bool GetProTranWIPIsContainProcessLine(string factoryCd, string strDept, string GarmentType) //Added By ZouShiChang On 2013.08.27 MES 024 (GARMENT_TYPE)
        {
            string SQL = "select 1 from gen_process_wip_control_master where factory_cd='" + factoryCd + "' and process_cd='" + strDept + "' ";
            if (GarmentType != "")
            {
                SQL = SQL + " and Garment_type='" + GarmentType + "'";
            }
            SQL = SQL + " and query_line_status='Y'";
            if (DBUtility.GetTable(SQL, "MES").Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        public static DataTable GetProTranWIPSummaryJoList(string factoryCd, DbConnection MESConn)
        {
            string SQL = "";

            //从MES取出JO列表
            SQL = SQL + " select job_order_no into #tmpjolist from( ";
            SQL = SQL + "select distinct JOB_ORDER_NO From dbo.PRD_JO_DAILY_STOCK with (nolock) ";
            SQL = SQL + "where FACTORY_CD = '" + factoryCd + "' ";
            SQL = SQL + "and trx_date=(SELECT CONVERT(varchar(12),MAX(CONVERT(smalldatetime,TRX_DATE,101)),101) AS MAX_TRX_DATE FROM PRD_CLOSING_HISTORY ";
            SQL = SQL + "WHERE LEN(TRX_DATE)>7 AND FACTORY_CD = '" + factoryCd + "'  and entry_type='JOD') ";
            SQL = SQL + "union all ";
            SQL = SQL + "select distinct JOB_ORDER_NO from dbo.CUT_BUNDLE_HD with (nolock) ";
            SQL = SQL + "where factory_cd='" + factoryCd + "' and CREATE_DATE>=(SELECT CONVERT(varchar(12),MAX(CONVERT(smalldatetime,TRX_DATE,101)),101) AS MAX_TRX_DATE FROM PRD_CLOSING_HISTORY ";
            SQL = SQL + "WHERE LEN(TRX_DATE)>7 AND FACTORY_CD = '" + factoryCd + "'  and entry_type='JOD')) a where 1=1 ";
            SQL = SQL + "select * from #tmpjolist";
            return DBUtility.GetTable(SQL, MESConn);
        }

        public static DataTable GetProTranWIPSummaryJoList(string SQL, string strRunNo, string factoryCd, string beginDate, string endDate)
        {
            SQL = SQL + " UPDATE MU_SHIP_JO_INFO as M,( ";
            SQL = SQL + "SELECT B.CT_NO  ";
            SQL = SQL + "   FROM FTY_INVOICE_HD A, MU_SHIP_JO_INFO B,FTY_INVOICE_DT C  ";
            SQL = SQL + "     WHERE (A.INVOICE_NO LIKE 'J" + factoryCd + "%'  or A.INVOICE_NO LIKE 'K" + factoryCd + "%' ) ";
            SQL = SQL + "     AND C.INVOICE_ID=A.INVOICE_ID   ";
            SQL = SQL + "     AND A.FACTORY_CD='" + factoryCd + "' AND B.CT_NO=C.CT_NO      ";
            SQL = SQL + "     AND A.STATUS='POST' ";
            SQL = SQL + "     AND A.SHIP_DATE >='" + beginDate + "'  AND A.SHIP_DATE <'" + endDate + "'  ";
            SQL = SQL + "     AND B.Run_No='" + strRunNo + "'     ";
            SQL = SQL + "     UNION ALL      ";
            SQL = SQL + "    SELECT B.CT_NO ";
            SQL = SQL + "     FROM CUST_INVOICE_HD A,MU_SHIP_JO_INFO B,CUST_INVOICE_DT C ";
            SQL = SQL + "       WHERE INVOICE_NO LIKE 'V" + factoryCd + "%'  AND C.INVOICE_ID=A.INVOICE_ID        ";
            SQL = SQL + "       AND A.FACTORY_CD='" + factoryCd + "'  AND B.CT_NO=C.CT_NO ";
            SQL = SQL + "     AND A.SHIP_DATE >='" + beginDate + "'  AND A.SHIP_DATE <'" + endDate + "'  ";
            SQL = SQL + "       AND A.STATUS='POST' ";
            SQL = SQL + "      AND B.Run_No='" + strRunNo + "' ) AS S ";
            SQL = SQL + "   SET LASTJO='Y' ";
            SQL = SQL + "   WHERE S.CT_NO=M.CT_NO  ";
            SQL = SQL + "       AND M.Run_No='" + strRunNo + "'; ";
            SQL = SQL + " SELECT CT_NO FROM  MU_SHIP_JO_INFO where Run_No='" + strRunNo + "' AND LASTJO='Y'; ";


            return DBUtility.GetTable(SQL, "GIS");
        }

        //---2013.03.05---ZouShCh
        //旧ＷＩＰ Detail
        public static DataTable GenerateProTranWIPDetail(string factoryCd, string strDeptList, string strJo, bool isJoLike, bool isOut, string dateType, string startDate, string endDate, string garmentType, string strTableName, string strOutSql, string strOutTableName, bool isOSJo, DbConnection MESConn)
        {
            string SQL = "  ";


            //从GIS取出JO列表strOutTableName
            SQL = SQL + strOutSql;

            //取出GIS中的有效JoList
            if (strOutSql != "")
            {
                SQL = SQL + " delete from #tmpjolist where not exists( select 1 from " + strOutTableName + " a where a.CT_NO=job_order_no)";
            }
            else if (strOutSql == "" && isOut)
            {
                SQL = SQL + " delete from #tmpjolist";
            }

            //先把裁数放在临时表（实际操作中发现，先放在临时表，再更新，比直接更新快10倍左右）
            SQL = SQL + " select JOB_ORDER_NO,sum(qty) as qty  ";
            SQL = SQL + "into #cutqty from dbo.V_ACTUAL_CUT_BUNDLE b with (nolock) where  ";
            SQL = SQL + "exists (select top 1 1 from #tmpjolist j where j.JOB_ORDER_NO=b.JOB_ORDER_NO) ";
            SQL = SQL + "group by JOB_ORDER_NO ";

            //根据有效列表，把WIP数据INSERT 到临时表中，#201112710995454151 是报表随机生成的临时表

            SQL = SQL + " delete from TmpWIPTable where Create_Date<convert(varchar(10),dateadd(dd,-2,getdate()),101); ";
            SQL += " insert into TmpWIPTable SELECT G.display_seq,a.Buyer_Po_Del_date,a.qty,A.JOB_ORDER_NO,A.NEXT_PROCESS_CD AS PROCESS_CD,(INQTY-ISNULL(OUTQTY,0)) AS WIP,'" + strTableName + "',getdate(),A.NEXT_PROCESS_GARMENT_TYPE AS GARMENT_TYPE "; //Added By ZouShiChang ON 2013.08.27 MES 024 (NEXT_PROCESS_GARMENT_TYPE)
            SQL = SQL + "FROM  ";
            SQL = SQL + "( ";

            SQL = SQL + " select Buyer_Po_del_date,qty,job_order_no,next_process_cd,NEXT_PROCESS_GARMENT_TYPE,NEXT_PROCESS_TYPE,sum(inqty) as inqty from ("; //Added By ZouShiChang ON 2013.08.27 MES 024 (NEXT_PROCESS_GARMENT_TYPE,NEXT_PROCESS_TYPE)

            SQL = SQL + "select convert(char(10),Buyer_Po_Del_date,120) as Buyer_Po_Del_date,0 as qty,JOB_ORDER_NO,NEXT_PROCESS_CD,NEXT_PROCESS_GARMENT_TYPE,NEXT_PROCESS_TYPE,SUM(NEXT_PROCESS_QTY) AS INQTY "; //Added By ZouShiChang ON 2013.08.27 MES 024 (NEXT_PROCESS_GARMENT_TYPE,NEXT_PROCESS_TYPE)
            SQL = SQL + " from prd_jo_stock p1 with (nolock) inner join jo_hd j with (nolock) ";
            SQL = SQL + "on p1.job_order_no=j.jo_no inner join sc_hd s on j.sc_no=s.sc_no ";


            SQL = SQL + " and p1.curr_PROCESS_CD not like '%CUTI003%'";//处理WIP为负数的数据


            SQL = SQL + " where 1=1 ";
            if (!isOSJo)
            {
                SQL = SQL + " and s.SAM_GROUP_CD<>'OUTSOURCE'  ";
            }

            if (garmentType != "")
            {
                SQL = SQL + "and j.garment_type_cd='" + garmentType + "' ";
            }
            if (factoryCd != "")
            {
                SQL += " and P1.FACTORY_CD='" + factoryCd + "' ";
            }
            switch (dateType)
            {
                case "BPO":
                    if (startDate != "")
                    {
                        SQL += " and j.BUYER_PO_DEL_DATE>='" + startDate + "'";
                    }
                    if (endDate != "")
                    {
                        SQL += " and j.BUYER_PO_DEL_DATE<='" + endDate + "'";
                    }
                    break;
                case "PPC":
                    if (startDate != "")
                    {
                        SQL += " and j.prod_completion_date>='" + startDate + "'";
                    }
                    if (endDate != "")
                    {
                        SQL += " and j.prod_completion_date<='" + endDate + "'";
                    }
                    break;
            }
            if (strJo != "")
            {
                if (isJoLike)
                {
                    SQL = SQL + "AND job_order_no like '%" + strJo + "%'    ";
                }
                else
                {
                    if (strJo.Contains("'"))
                    {
                        SQL = SQL + "and job_order_no in (" + strJo + ") ";
                    }
                    else
                    {
                        SQL = SQL + "AND job_order_no = '" + strJo + "'    ";
                    }
                }
            }
            SQL = SQL + "AND NEXT_PROCESS_CD<>'NA' ";
            if (strDeptList != "" & strDeptList != "KeyPart")
            {
                SQL = SQL + "and NEXT_PROCESS_CD= '" + strDeptList + "' ";
            }
            else if (strDeptList == "KeyPart")
            {
                SQL = SQL + "and  exists (select 1 from gen_prc_cd_mst with (nolock) where major_flag like '%A%' and factory_cd=P1.FACTORY_CD and prc_cd=NEXT_PROCESS_CD ";
                //Added By ZouShiChang ON 2013.08.27 Start MES 024
                SQL = SQL + " AND GARMENT_TYPE=NEXT_PROCESS_GARMENT_TYPE AND PROCESS_TYPE=NEXT_PROCESS_TYPE ";
                //Added By ZouShiChang ON 2013.08.27 Start MES 024
                SQL = SQL + "and garment_type=j.garment_type_cd )";
            }
            SQL = SQL + " and exists (select top 1 1 from #tmpjolist j where j.JOB_ORDER_NO=p1.JOB_ORDER_NO) ";
            SQL = SQL + "GROUP BY Buyer_Po_Del_date,JOB_ORDER_NO,NEXT_PROCESS_CD,NEXT_PROCESS_GARMENT_TYPE,NEXT_PROCESS_TYPE "; //Added By ZouShiChang ON 2013.08.27 MES 024 (NEXT_PROCESS_GARMENT_TYPE,NEXT_PROCESS_TYPE)
            SQL = SQL + "UNION ALL ";
            SQL = SQL + "SELECT  convert(char(10),Buyer_Po_Del_date,120) as Buyer_Po_Del_date,0 as qty,a.job_order_no, b.prc_cd AS NEXT_PROCESS_CD,B.GARMENT_TYPE AS NEXT_PROCESS_GARMENT_TYPE,B.PROCESS_TYPE AS NEXT_PROCESS_TYPE,SUM (a.qty) AS INQTY                     "; //Added By ZouShiChang ON 2013.08.27 MES 024 (GARMENT_TYPE,PROCESS_TYPE)
            SQL = SQL + "                FROM cut_bundle_hd a with (nolock) INNER JOIN GEN_PRC_CD_MST b with (nolock) ";
            SQL = SQL + "    ON a.factory_cd = b.factory_cd AND b.display_seq = 1 ";
            if (strDeptList != "" & strDeptList != "KeyPart")
            {
                SQL = SQL + "    AND B.PRC_CD= '" + strDeptList + "' ";
            }
            else if (strDeptList == "KeyPart")
            {
                SQL = SQL + "and  exists (select 1 from gen_prc_cd_mst with (nolock) where major_flag like '%A%' and factory_cd=a.FACTORY_CD and prc_cd=B.PRC_CD ";
                SQL = SQL + " and garment_type=b.garment_type and process_type=b.process_type ";  //Added By ZouShiChang ON 2013.08.27 MES 024              
                if (garmentType != "")
                {
                    SQL = SQL + "and garment_type='" + garmentType + "' )";
                }
                else
                {
                    SQL = SQL + " )";
                }
            }
            SQL = SQL + "    inner join jo_hd j  ";
            SQL = SQL + "    on A.job_order_no=j.jo_no inner join sc_hd s on s.sc_no=j.sc_no ";
            if (!isOSJo)
            {
                SQL = SQL + " and s.SAM_GROUP_CD<>'OUTSOURCE'  ";
            }

            SQL = SQL + "AND J.GARMENT_TYPE_CD=B.GARMENT_TYPE ";
            if (garmentType != "")
            {
                SQL = SQL + "    and j.garment_type_cd='" + garmentType + "'  ";
            }
            if (factoryCd != "")
            {
                SQL = SQL + "               WHERE A.FACTORY_CD='" + factoryCd + "'  ";
            }
            if (strJo != "")
            {
                if (isJoLike)
                {
                    SQL = SQL + "   AND a.job_order_no like '%" + strJo + "%'         ";
                }
                else
                {
                    if (strJo.Contains("'"))
                    {
                        SQL = SQL + " and a.job_order_no in (" + strJo + ") ";
                    }
                    else
                    {
                        SQL = SQL + " AND a.job_order_no = '" + strJo + "'    ";
                    }
                }
            }
            if (startDate != "")
            {
                SQL += " and j.BUYER_PO_DEL_DATE>='" + startDate + "'";
            }
            if (endDate != "")
            {
                SQL += " and j.BUYER_PO_DEL_DATE<='" + endDate + "'";
            }
            SQL = SQL + " and exists (select top 1 1 from #tmpjolist j where j.JOB_ORDER_NO=a.JOB_ORDER_NO) ";
            SQL = SQL + "            GROUP BY  Buyer_Po_Del_date,a.job_order_no, b.prc_cd,b.garment_type,B.PROCESS_TYPE "; //Added By ZouShiChang ON 2013.08.27 MES024 (GARMENT_TYPE,PROCESS_TYPE)
            SQL = SQL + ") A ";
            SQL = SQL + " group by Buyer_Po_del_date,qty,job_order_no,next_process_cd,next_process_garment_type,NEXT_PROCESS_TYPE) A "; //Added By ZouShiChang On 2013.08.27 MES 024 (NEXT_PROCESS_GARMENT_TYPE,NEXT_PROCESS_TYPE)            
            SQL = SQL + "inner join GEN_PRC_CD_MST G with (nolock) ";
            SQL = SQL + "on a.NEXT_PROCESS_CD=G.PRC_CD ";
            SQL = SQL + " and A.NEXT_PROCESS_GARMENT_TYPE=G.GARMENT_TYPE AND A.NEXT_PROCESS_TYPE=G.PROCESS_TYPE "; //Added By ZouShiChang On 2013.08.27 MES 024 
            if (factoryCd != "")
            {
                SQL = SQL + " AND G.FACTORY_CD='" + factoryCd + "' ";
            }
            SQL = SQL + "left JOIN(select JOB_ORDER_NO,CURR_PROCESS_CD,CURR_PROCESS_GARMENT_TYPE,CURR_PROCESS_TYPE,SUM(ISNULL(CURR_PROCESS_QTY,0)+ISNULL(PULLOUT_QTY,0)+ISNULL(DISCREPANCY_QTY,0)) AS OUTQTY "; //Added By ZouShiChang On 2013.08.27 MES 024 (CURR_PROCESS_GARMENT_TYPE,CURR_PROCESS_TYPE)
            SQL = SQL + " from prd_jo_stock P2 with (nolock) inner join jo_hd j with (nolock) ";
            SQL = SQL + "    on P2.job_order_no=j.jo_no ";

            if (garmentType != "")
            {
                SQL = SQL + "and j.garment_type_cd='" + garmentType + "' ";
            }
            SQL = SQL + " where 1=1 ";
            if (factoryCd != "")
            {
                SQL += " and P2.FACTORY_CD='" + factoryCd + "' ";
            }
            if (strJo != "")
            {
                if (isJoLike)
                {
                    SQL = SQL + "AND job_order_no like '%" + strJo + "%'   ";
                }
                else
                {
                    if (strJo.Contains("'"))
                    {
                        SQL = SQL + "and job_order_no in (" + strJo + ") ";
                    }
                    else
                    {
                        SQL = SQL + "AND job_order_no = '" + strJo + "'    ";
                    }
                }
            }
            if (startDate != "")
            {
                SQL += " and j.BUYER_PO_DEL_DATE>='" + startDate + "'";
            }
            if (endDate != "")
            {
                SQL += " and j.BUYER_PO_DEL_DATE<='" + endDate + "'";
            }
            if (strDeptList != "" & strDeptList != "KeyPart")
            {
                SQL = SQL + "AND CURR_PROCESS_CD = '" + strDeptList + "' ";
            }
            else if (strDeptList == "KeyPart")
            {
                SQL = SQL + "AND  exists (select 1 from gen_prc_cd_mst with (nolock) where major_flag like '%A%' and factory_cd=P2.FACTORY_CD and prc_cd=CURR_PROCESS_CD ";
                SQL = SQL + " and garment_type=CURR_PROCESS_GARMENT_TYPE and process_type=curr_process_type "; //added by zoushichang on 2013.08.27 MES 024
                if (garmentType != "")
                {
                    SQL = SQL + "and garment_type='" + garmentType + "' )";
                }
                else
                {
                    SQL = SQL + " )";
                }
            }
            SQL = SQL + " and exists (select top 1 1 from #tmpjolist j where j.JOB_ORDER_NO=P2.JOB_ORDER_NO) ";
            SQL = SQL + "GROUP BY JOB_ORDER_NO,CURR_PROCESS_CD,CURR_PROCESS_GARMENT_TYPE,CURR_PROCESS_TYPE) B "; //Added By ZouShiChang ON 2013.08.27 MES 024 (CURR_PROCESS_GARMENT_TYPE,CURR_PROCESS_TYPE)
            SQL = SQL + "ON A.NEXT_PROCESS_CD=CURR_PROCESS_CD AND A.JOB_ORDER_NO=B.JOB_ORDER_NO ";
            SQL = SQL + " AND A.NEXT_PROCESS_GARMENT_TYPE=CURR_PROCESS_GARMENT_TYPE AND A.NEXT_PROCESS_TYPE=CURR_PROCESS_TYPE "; //Added By ZouShiChang ON 2013.08.27 MES 024
            SQL = SQL + "where 1=1 ";
            if (factoryCd != "")
            {
                SQL += " and g.factory_cd='" + factoryCd + "' ";
            }

            //更新裁数
            SQL = SQL + "update TmpWIPTable  set qty=b.qty ";
            SQL = SQL + "from #cutqty b ";
            SQL = SQL + "where b.JOB_ORDER_NO=TmpWIPTable.JOB_ORDER_NO and Run_No='" + strTableName + "' ";

            //删除WIP=0的数据，并更新有效JO列表
            SQL = SQL + " delete TmpWIPTable where WIP=0 and Run_No='" + strTableName + "' ;delete #tmpjolist where not exists (select top 1 1 from TmpWIPTable j where j.JOB_ORDER_NO=#tmpjolist.JOB_ORDER_NO and Run_No='" + strTableName + "');select * from TmpWIPTable where Run_No='" + strTableName + "' ";
            return DBUtility.GetTable(SQL, MESConn);
        }

        //旧ＷＩＰ summary
        public static DataTable GetProTranWIPSummary(string strTableName, DbConnection MESConn)
        {
            string sql = " SELECT display_seq,PROCESS_CD ,SUM(WIP) AS WIP FROM  TmpWIPTable where Run_No='" + strTableName + "' group by process_cd,display_seq having sum(WIP)<>0 order by display_seq ";
            return DBUtility.GetTable(sql, MESConn);
        }
        public static DataTable GetProTranWIPJoList(string strTableName, DbConnection MESConn)
        {
            string sql = " SELECT distinct job_order_no,buyer_po_del_date,qty FROM  TmpWIPTable where Run_No='" + strTableName + "'";
            return DBUtility.GetTable(sql, MESConn);
        }
        public static DataTable GetProTranWIPTitle(string strTableName, string strFactoryCd, string strDeptList, DbConnection MESConn)
        {//Added By ZouShiChang ON 2013.09.13 Start MES 024

            //string sql = " SELECT distinct a.display_seq,PROCESS_CD,b.GARMENT_TYPE FROM TmpWIPTable A INNER JOIN gen_prc_cd_mst B ON  ";
            //string sql = " SELECT distinct a.display_seq,PROCESS_CD FROM TmpWIPTable A INNER JOIN gen_prc_cd_mst B ON  ";
            //sql += " A.PROCESS_CD=B.PRC_CD AND A.GARMENT_TYPE=B.GARMENT_TYPE ";
            //sql += " AND B.FACTORY_CD='" + strFactoryCd + "' where a.Run_No='" + strTableName + "' ";
          string sql = "SELECT DISTINCT display_seq,PROCESS_CD FROM TmpWIPTable WHERE Run_No='" + strTableName + "'";
          if (strDeptList != "")
            {
                sql += " AND process_cd='" + strDeptList + "' ";
            }
            //sql += " order by B.GARMENT_TYPE,A.display_seq ";
            sql += " order by display_seq ";
          

            //Added By ZouShiChang ON 2013.09.13 End MES 024
            return DBUtility.GetTable(sql, MESConn);
        }

        public static DataTable GenerateProTranWIPDetail(string strTableName, string strDeptList, bool showLine)
        {   //Added By ZouShiChang ON 2013.09.17 Start MES024
            //string sql = " select * from TmpWIPTable where Run_No='" + strTableName + "' and  process_cd='" + strDeptList + "' order by job_order_no ";
            string sql = " SELECT display_seq,JOB_ORDER_NO,Buyer_Po_Del_date,qty,PROCESS_CD,SUM(WIP) AS WIP,Run_No,Create_date,GARMENT_TYPE ";
            if (showLine)
                sql += " ,BUYER_SHORT_NAME, STYLE_NO, PRODUCTION_LINE_CD"; //Added by Zikuan for MES-093
            sql += "  from TmpWIPTable where WIP<>0 AND Run_No='" + strTableName + "' and  process_cd='" + strDeptList + "' ";
            sql += "  GROUP BY display_seq,JOB_ORDER_NO,Buyer_Po_Del_date,qty,PROCESS_CD,Run_No,Create_date,GARMENT_TYPE ";
            if(showLine)
                sql += " ,BUYER_SHORT_NAME, STYLE_NO, PRODUCTION_LINE_CD"; //Added by Zikuan for MES-093
            sql += " HAVING SUM(WIP)<>0 ORDER BY job_order_no ";
            //Added By ZouShiChang ON 2013.09.17 End MES024
            return DBUtility.GetTable(sql, "MES");
        }

        //新WIP Detail

        public static DataTable GenerateProTranNewWIPDetail(string factoryCd, string strDeptList, string strJo, bool isJoLike, bool isOut, string dateType, string startDate, string endDate, string garmentType,string processType,string productFactory, string strTableName, string strOutSql, string strOutTableName, bool isOSJo, DbConnection MESConn, string line)
        {

            string
            SQL = "     DELETE  FROM TmpWIPTable ";
            SQL += "    WHERE   Create_Date < CONVERT(VARCHAR(10), DATEADD(dd, -2, GETDATE()), 101); ";

            SQL += " insert into TmpWIPTable(display_seq,buyer_po_del_date,Qty,job_order_no,PROCESS_CD,WIP,RUN_NO,CREATE_DATE,GARMENT_TYPE,PROCESS_TYPE,PRODUCTION_FACTORY";
            SQL += " ,BUYER_SHORT_NAME, STYLE_NO, PRODUCTION_LINE_CD"; //Added by Zikuan - MES-093
            /*
            if (garmentType != "")
            {
              SQL = SQL + ",GARMENT_TYPE";
            }
            
            if (processType != "")
            {
              SQL = SQL + ",PROCESS_TYPE";
            }
            if (productFactory != "")
            {
              SQL = SQL + ",PRODUCTION_FACTORY";
            }
             */ 
            SQL = SQL + ")";

            SQL += "    SELECT MIN(A.stage) AS display_seq,MIN(convert(VARCHAR(100),B.buyer_po_del_date,23)) AS buyer_po_del_date,ISNULL(SUM(C.qty)/COUNT(*),0) AS qty,A.job_order_no,";
            SQL += " A.PROCESS_CD,SUM(A.WIP) AS WIP,Run_No='" + strTableName + "',GETDATE() AS Create_date,A.GARMENT_TYPE,A.PROCESS_TYPE,A.PRODUCTION_FACTORY ";
            /*
            if (garmentType != "")
            {
              SQL = SQL + ",A.GARMENT_TYPE";
            }
            
            if (processType != "")
            {
              SQL = SQL + ",A.PROCESS_TYPE";
            }
            if (productFactory != "")
            {
              SQL = SQL + ",A.PRODUCTION_FACTORY ";
            }
             */
            SQL += " ,E.SHORT_NAME, B.STYLE_NO, A.PRODUCTION_LINE_CD"; //Added by Zikuan - MES-093
            SQL += " FROM prd_jo_wip_hd A ";
            SQL += " left join ";
            SQL += " jo_hd B ";
            SQL += " on A.job_Order_no=B.jo_no ";
            SQL += " left join ";
            SQL += " (select job_Order_no,sum(qty) as qty from cut_bundle_hd ";
           //start modification by LimML on 20150908 fixed program bug(qty = 0)
            //if (strJo != "")
            //{
            //    SQL += " where job_order_no='" + strJo + "'";
            //}
            if (strJo != "")
            {
                if (isJoLike)
                {
                    SQL = SQL + "where job_order_no like '%" + strJo + "%'    ";
                }
                else
                {
                    if (strJo.Contains("'"))
                    {
                        SQL = SQL + "where job_order_no in (" + strJo + ") ";
                    }
                    else
                    {
                        SQL = SQL + "where job_order_no = '" + strJo + "'    ";
                    }
                }
            }
            //end modification by LimML on 20150908
            SQL += " group by job_order_no) C";
            SQL += " on C.job_order_no=B.jo_no";
            SQL += " inner join SC_HD D";
            SQL += " on B.SC_NO=D.SC_NO";
            SQL += " left join GEN_CUSTOMER E on B.CUSTOMER_CD = E.CUSTOMER_CD"; //Added by Zikuan, MES-093
            SQL += " LEFT JOIN dbo.GEN_PRC_CD_MST AS F ON F.GARMENT_TYPE = A.GARMENT_TYPE AND F.PRC_CD=A.PROCESS_CD AND F.FACTORY_CD=A.FACTORY_CD "; //Added By ZouShiChang ON 2013.12.17
            SQL += " where  A.wip<>0 ";
            SQL += "  AND F.ACTIVE='Y' "; //Added By ZouShiChang ON 2013.12.17
            switch (dateType)
            {
                case "BPO":
                    if (startDate != "")
                    {
                        SQL += " and B.BUYER_PO_DEL_DATE>='" + startDate + "'";
                    }
                    if (endDate != "")
                    {
                        SQL += " and B.BUYER_PO_DEL_DATE<='" + endDate + "'";
                    }
                    break;
                case "PPC":
                    if (startDate != "")
                    {
                        SQL += " and B.prod_completion_date>='" + startDate + "'";
                    }
                    if (endDate != "")
                    {
                        SQL += " and B.prod_completion_date<='" + endDate + "'";
                    }
                    break;
            }
            if (strJo != "")
            {
                if (isJoLike)
                {
                    SQL = SQL + "AND A.job_order_no like '%" + strJo + "%'    ";
                }
                else
                {
                    if (strJo.Contains("'"))
                    {
                        SQL = SQL + "and Ajob_order_no in (" + strJo + ") ";
                    }
                    else
                    {
                        SQL = SQL + "AND A.job_order_no = '" + strJo + "'    ";
                    }
                }
            }
            if (garmentType != "")
            {
                SQL = SQL + "and B.garment_type_cd='" + garmentType + "' ";
            }

            if (processType != "")
            {
              SQL = SQL + "AND A.PROCESS_TYPE='" + processType + "' ";
            }
            if (productFactory != "")
            {
              SQL = SQL + "AND A.PRODUCTION_FACTORY='" + productFactory + "' ";
            }
            if (factoryCd != "")
            {
                SQL += " and B.FACTORY_CD='" + factoryCd + "' ";
            }
            if (startDate != "")
            {
                SQL += " and B.BUYER_PO_DEL_DATE>='" + startDate + "'";
            }
            if (endDate != "")
            {
                SQL += " and B.BUYER_PO_DEL_DATE<='" + endDate + "'";
            }
            if (strDeptList != "" & strDeptList != "KeyPart")
            {
                SQL = SQL + "AND PROCESS_CD = '" + strDeptList + "' ";
            }
            //Added By ZouShiChang ON 2014.01.14 Start
            else if (strDeptList == "KeyPart")
            {
                SQL = SQL + "AND  exists (select 1 from gen_prc_cd_mst with (nolock) where major_flag like '%A%' and factory_cd=A.FACTORY_CD and prc_cd=A.process_cd ";
                SQL = SQL + " and garment_type=A.GARMENT_TYPE and ACTIVE='Y' ";
                if (garmentType != "")
                {
                    SQL = SQL + "and garment_type='" + garmentType + "' )";
                }
                else
                {
                    SQL = SQL + " )";
                }
            }
            //Added By ZouShiChang ON 2014.01.14 End
            if (!isOSJo)
            {
                SQL = SQL + " and D.SAM_GROUP_CD<>'OUTSOURCE'  ";
            }
            //Added by Zikuan - MES-093
            if (line != "" && !line.Contains("All")) 
            {
                string[] lineSplit = line.Split(',');
                SQL = SQL + " and A.PRODUCTION_LINE_CD in (";

                for (int i = 0; i < lineSplit.Count(); i++)
                {
                    if (i != lineSplit.Count() - 1)
                    {
                        SQL = SQL + "'" + lineSplit[i] + "',";
                    }
                    else
                    {
                        SQL = SQL + "'" + lineSplit[i] + "'";
                    }
                }
                SQL = SQL + ") ";
            }
            //End Added MES-093
            SQL = SQL + "GROUP BY A.JOB_ORDER_NO,A.PROCESS_CD,A.GARMENT_TYPE,A.PROCESS_TYPE,A.PRODUCTION_FACTORY";
            SQL += " ,E.SHORT_NAME, B.STYLE_NO, A.PRODUCTION_LINE_CD"; //Added By Zikuan - MES-093
            /*
            if (garmentType != "")
            {
              SQL = SQL + ",A.GARMENT_TYPE";
            }
            if (processType != "")
            {
              SQL = SQL + ",A.PROCESS_TYPE";
            }
            if (productFactory != "")
            {
              SQL = SQL + ",A.PRODUCTION_FACTORY";
            }
             */

            SQL += "; SELECT display_seq,Buyer_Po_Del_date,qty,JOB_ORDER_NO, PROCESS_CD, SUM(WIP) as WIP, Run_No, Create_date, Garment_type, PROCESS_TYPE, PRODUCTION_FACTORY FROM dbo.TmpWIPTable WHERE Run_No='" + strTableName + "' GROUP BY display_seq,Buyer_Po_Del_date,qty,JOB_ORDER_NO, PROCESS_CD, Run_No, Create_date, Garment_type, PROCESS_TYPE, PRODUCTION_FACTORY ";

            return DBUtility.GetTable(SQL, MESConn);
        }

        public static DataTable GenerateProTranNewWIPSummary(string factoryCd, string strDeptList, string strJo, bool isJoLike, bool isOut, string dateType, string startDate, string endDate, string garmentType, string strTableName, string strOutSql, string strOutTableName, bool isOSJo, DbConnection MESConn)
        {

            string SQL = "  select A.stage as display_seq, A.PROCESS_CD,sum(A.WIP) as wip ";
            SQL += " from prd_jo_wip_hd A  left join  jo_hd B  on A.job_Order_no=B.jo_no  left join ";
            SQL += " (select job_Order_no,sum(qty) as qty from cut_bundle_hd ";
            if (strJo != "")
            {
                SQL += " where job_order_no='" + strJo + "' ";
            }
            SQL += " group by job_order_no) C on C.job_order_no=B.jo_no inner join SC_HD D on B.SC_NO=D.SC_NO ";
            SQL += " where  A.wip<>0 ";
            if (strJo != "")
            {
                SQL += " AND A.job_order_no like '%" + strJo + "%'     ";
            }
            SQL += " and B.FACTORY_CD='EGM'  and D.SAM_GROUP_CD<>'OUTSOURCE'  ";


            switch (dateType)
            {
                case "BPO":
                    if (startDate != "")
                    {
                        SQL += " and B.BUYER_PO_DEL_DATE>='" + startDate + "'";
                    }
                    if (endDate != "")
                    {
                        SQL += " and B.BUYER_PO_DEL_DATE<='" + endDate + "'";
                    }
                    break;
                case "PPC":
                    if (startDate != "")
                    {
                        SQL += " and B.prod_completion_date>='" + startDate + "'";
                    }
                    if (endDate != "")
                    {
                        SQL += " and B.prod_completion_date<='" + endDate + "'";
                    }
                    break;
            }
            if (strJo != "")
            {
                if (isJoLike)
                {
                    SQL = SQL + "AND A.job_order_no like '%" + strJo + "%'    ";
                }
                else
                {
                    if (strJo.Contains("'"))
                    {
                        SQL = SQL + "and Ajob_order_no in (" + strJo + ") ";
                    }
                    else
                    {
                        SQL = SQL + "AND A.job_order_no = '" + strJo + "'    ";
                    }
                }
            }
            if (garmentType != "")
            {
                SQL = SQL + "and B.garment_type_cd='" + garmentType + "' ";
            }

            if (factoryCd != "")
            {
                SQL += " and B.FACTORY_CD='" + factoryCd + "' ";
            }
            if (startDate != "")
            {
                SQL += " and B.BUYER_PO_DEL_DATE>='" + startDate + "'";
            }
            if (endDate != "")
            {
                SQL += " and B.BUYER_PO_DEL_DATE<='" + endDate + "'";
            }
            if (strDeptList != "" & strDeptList != "KeyPart")
            {
                SQL = SQL + "AND PROCESS_CD = '" + strDeptList + "' ";
            }
            if (!isOSJo)
            {
                SQL = SQL + " and D.SAM_GROUP_CD<>'OUTSOURCE'  ";
            }

            SQL += " group by a.stage,A.PROCESS_CD ";


            return DBUtility.GetTable(SQL, MESConn);
        }


        //新WIP summary
        //public static DataTable GetProTranNewWIPSummary(string strTableName, DbConnection MESConn)
        //{
        //   string SQL="   SELECT stage as display_seq,PROCESS_CD ,SUM(WIP) AS WIP ";
        //    SQL+=" FROM  prd_jo_wip_hd  ";

        //    SQL+=" group by process_cd,stage having sum(WIP)<>0";
        //    SQL += " order by stage ";
        //    return DBUtility.GetTable(SQL, MESConn);
        //}

        //新ＷＩＰＴitle

        public static DataTable GetProTranNewWIPTitle(string strJo, string strFactoryCd, string strDeptList, DbConnection MESConn)
        {
            String SQL = " select distinct A.stage as display_seq,A.Process_cd,B.GARMENT_TYPE from prd_jo_wip_hd  A";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL+=" INNER JOIN gen_prc_cd_mst B ON A.PROCESS_CD=B.PRC_CD and B.Factory_cd='"+strFactoryCd+"'";
            SQL += " INNER JOIN gen_prc_cd_mst B ON A.PROCESS_CD=B.PRC_CD and A.PROCESS_TYPE=B.PROCESS_TYPE AND A.GARMENT_TYPE=B.GARMENT_TYPE AND  B.Factory_cd='" + strFactoryCd + "'";
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            SQL += " where  wip<>0 ";
            if (strJo != "")
            {
                SQL += " and job_order_no='" + strJo + "'";
            }
            if (strDeptList != "")
            {
                SQL += " and process_cd='" + strDeptList + "' ";
            }

            SQL += " order by B.Garment_type,display_seq ";
            return DBUtility.GetTable(SQL, MESConn);
        }

        public static DataTable GetProTranNewWIPTitle(string strFactoryCd, string strDeptList, DbConnection MESConn)
        {
            String SQL = " select distinct A.stage as display_seq,A.Process_cd,B.GARMENT_TYPE from prd_jo_wip_hd  A";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL += " INNER JOIN gen_prc_cd_mst B ON A.PROCESS_CD=B.PRC_CD and B.Factory_cd='" + strFactoryCd + "'";
            SQL += " INNER JOIN gen_prc_cd_mst B ON A.PROCESS_CD=B.PRC_CD AND A.PROCESS_TYPE=B.PROCESS_TYPE AND A.GARMENT_TYPE=B.GARMENT_TYPE and B.Factory_cd='" + strFactoryCd + "'";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            SQL += " where  wip<>0 ";
            //if (strJo != "")
            //{
            //    SQL += " and job_order_no='" + strJo + "'";
            //}
            if (strDeptList != "")
            {
                SQL += " and process_cd='" + strDeptList + "' ";
            }

            SQL += " order by B.Garment_type,display_seq ";
            return DBUtility.GetTable(SQL, MESConn);
        }
        //---2013.03.05---ZouShCh


        public static DataTable GenerateProTranNewWIPDetail(string strTableName, string strDeptList)
        {
            string SQL = "  select A.stage as display_seq,B.buyer_po_del_date,Max(C.qty) as qty,A.job_order_no,";
            SQL += " A.PROCESS_CD,sum(A.WIP) as wip";
            SQL += " from prd_jo_wip_hd A ";
            SQL += " left join ";
            SQL += " jo_hd B ";
            SQL += " on A.job_Order_no=B.jo_no ";
            SQL += "left join ";
            SQL += " (select job_Order_no,sum(qty) as qty from cut_bundle_hd ";

            SQL += " group by job_order_no) C";
            SQL += " on C.job_order_no=B.jo_no";
            SQL += " inner join SC_HD D";
            SQL += " on B.SC_NO=D.SC_NO";
            SQL += " where  A.wip<>0 and process_cd='" + strDeptList + "' ";
            SQL += " group by A.stage,B.buyer_po_del_date,A.job_order_no,";
            SQL += " A.PROCESS_CD having Sum(A.wip)<>0";
            SQL += " order by job_order_no ";


            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetProTranWIPSummaryProcessDetail(string factoryCd, string strJo, string strColor, string strSize, string process_cd,string process_garment_type,string process_type,string process_production_factory,string next_process_cd,string next_process_garment_type,string next_process_type,string next_process_production_factory)
        {
            string SQL = " select CONVERT(VARCHAR(10),A.TRX_DATE,101) AS DATE,SUM(A.OUTPUT_QTY) AS QTY ";
            SQL = SQL + "from PRD_JO_OUTPUT_TRX a ";
            SQL = SQL + "inner join PRD_JO_OUTPUT_HD b ";
            SQL = SQL + "on b.doc_no=a.doc_no ";
            SQL = SQL + "and a.job_order_no='" + strJo + "' ";
            SQL = SQL + " and a.color_code='" + strColor + "' ";
            if (strSize != "")
            {
                SQL += " and a.size_code='" + strSize + "' ";
            }
            if (next_process_cd != "")
            {
                SQL = SQL + "and B.NEXT_PROCESS_CD='" + next_process_cd + "' ";
            }
            SQL = SQL + "and A.process_cd='" + process_cd + "' ";
            SQL = SQL + " and a.process_garment_type='" + process_garment_type + "' ";
            SQL = SQL + " and a.process_type='" + process_type + "' ";
            if (next_process_garment_type != "")
            {
                SQL = SQL + " and B.next_process_garment_type='" + next_process_garment_type + "' ";
            }
            if (next_process_type != "")
            {
                SQL = SQL + " and b.next_process_type='" + next_process_type + "' ";
            }
            if (next_process_production_factory != "")
            {
                SQL = SQL + " and b.next_process_production_factory='" + next_process_production_factory + "' ";
            }
            SQL = SQL + "WHERE A.FACTORY_CD='" + factoryCd + "' ";
            SQL = SQL + "GROUP BY  A.TRX_DATE ";
            return DBUtility.GetTable(SQL, "MES");
        }
        //----------------------------------------------------------------------------------YMG------------------------------------------------------------------------------------
        public static DataTable GetProTranWIPSummaryJoList(string strTableName, DbConnection MESConn, string factoryCd, string strGarmentType, string strJo, bool isJoLike, string strGo, bool isGoLike, string customer_cd, bool isCustomerLike, string DateType, string beginDate, string endDate, string StyleNo, bool isStyleLike, string ProductCat, string strType)
        {
            string SQL = "if exists (select 1 from tempdb..sysobjects where id = object_id('tempdb.." + strTableName + "')) drop table " + strTableName + "; select A.JO_NO into " + strTableName + " ";
            SQL = SQL + "FROM JO_HD A WITH(NOLOCK)";
            SQL = SQL + "INNER JOIN  ";
            SQL = SQL + "SC_HD S WITH(NOLOCK)";
            SQL = SQL + "ON S.SC_NO=A.SC_NO ";
            SQL = SQL + " join Jo_hd p  WITH(NOLOCK)";
            SQL = SQL + "on p.JO_NO=a.jo_no ";
            SQL = SQL + "where 1=1 and (a.status<>'D' or (a.status='D' and exists(select 1 from cut_bundle_hd WITH(NOLOCK) where job_order_no=a.jo_no)))";
            if (factoryCd != "")
            {
                SQL = SQL + " and a.factory_cd='" + factoryCd + "' ";
            }
            if (strGarmentType != "")
            {
                SQL = SQL + " and a.garment_type_cd='" + strGarmentType + "' ";
            }
            if (strJo != "")
            {
                if (isJoLike)
                {
                    SQL = SQL + "AND A.JO_NO LIKE '%" + strJo + "%' ";
                }
                else
                {
                    SQL = SQL + "AND A.JO_NO = '" + strJo + "' ";
                }
            }
            if (strGo != "")
            {
                if (isGoLike)
                {
                    SQL = SQL + "AND A.SC_NO like '%" + strGo + "%' ";
                }
                else
                {
                    SQL = SQL + "AND A.SC_NO = '" + strGo + "' ";
                }
            }
            if (customer_cd != "")
            {
                if (isCustomerLike)
                {
                    SQL = SQL + "AND exists( select 1 from dbo.GEN_CUSTOMER where customer_cd=A.CUSTOMER_CD and short_name like '%" + customer_cd + "%') ";
                }
                else
                {
                    SQL = SQL + "AND exists( select 1 from dbo.GEN_CUSTOMER where customer_cd=A.CUSTOMER_CD and short_name='" + customer_cd + "') ";
                }
            }
            switch (DateType)
            {
                case "BPO":
                    if (beginDate != "")
                    {
                        SQL = SQL + "AND A.BUYER_PO_DEL_DATE >='" + beginDate + "' ";
                    }
                    if (endDate != "")
                    {
                        SQL = SQL + "AND A.BUYER_PO_DEL_DATE <='" + endDate + "' ";
                    }
                    break;
                case "PPC":
                    if (beginDate != "")
                    {
                        SQL = SQL + "AND A.prod_completion_date >='" + beginDate + "' ";
                    }
                    if (endDate != "")
                    {
                        SQL = SQL + "AND A.prod_completion_date <='" + endDate + "' ";
                    }
                    break;
            }
            if (StyleNo != "")
            {
                if (isStyleLike)
                {
                    SQL = SQL + "AND p.CUST_STYLE_NO like '%" + StyleNo + "%' ";
                }
                else
                {
                    SQL = SQL + "AND p.CUST_STYLE_NO = '" + StyleNo + "' ";
                }
            }
            if (ProductCat != "")
            {
                SQL = SQL + "AND S.PRODUCT_CATEGORY='" + ProductCat + "' ";
            }
            switch (strType)
            {
                case "0":
                    SQL = SQL + "and a.BUYER_PO_DEL_DATE >= convert(varchar(10),dateadd(MM,-6,getdate()),101) ";
                    SQL = SQL + "and a.BUYER_PO_DEL_DATE<=convert(varchar(10),dateadd(MM,6,getdate()),101) ";
                    break;
                case "1":
                    SQL = SQL + "AND NOT EXISTS (SELECT TOP 1 1 FROM GEN_JOB_ORDER_MASTER C ";
                    SQL = SQL + "WHERE C.FACTORY_CD='" + factoryCd + "'  AND C.PROD_COMPLETED='Y' AND C.JOB_ORDER_NO=A.JO_NO) ";
                    break;
            }
            return DBUtility.GetTable(SQL, MESConn);
        }
        public static bool isChecked(string strTableName, DbConnection MESConn)
        {
            string sql = "select * from " + strTableName + "";
            if (DBUtility.GetTable(sql, MESConn).Rows.Count > 3000)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static DataTable GetProTranWIPDetail(string strTableName, DbConnection MESConn, string factoryCd)
        {
            string tableName = "";
            Random ro = new Random(1000);
            tableName = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
            string SQL = "if exists (select 1 from tempdb..sysobjects where id = object_id('tempdb.." + tableName + "')) drop table " + tableName + ";";
            SQL = SQL + " exec PROC_INQ_Transaction '" + tableName + "','" + factoryCd + "','','N','','','','O','','Detail','D','','','','','N','" + strTableName + "';select * from " + tableName + " order by 1";
            return DBUtility.GetTable(SQL, MESConn);
        }
        public static DataTable GetProTranFGISDetail(string strTableName, DbConnection MESConn, string factoryCd)
        {
            System.Text.StringBuilder SQL = new System.Text.StringBuilder();
            SQL.AppendFormat(@"SELECT t.COMBINE_JO_NO as JOB_ORDER_NO ,COLOR_CD,SIZE_CD1,SUM(QTY) AS QTY FROM dbo.IMP_FGIS_DATA_HD(NOLOCK) a INNER JOIN 
                                dbo.IMP_FGIS_DATA_DT (NOLOCK) b ON a.FGIS_NO=b.FGIS_NO AND ISNULL(a.SOURCE_FACTORY,'')=ISNULL(a.SOURCE_FACTORY,'')
                                INNER JOIN 
                                (SELECT JO_NO AS COMBINE_JO_NO,JO_NO AS ORIGINAL_JO_NO   FROM {0}
                                UNION ALL
                                SELECT b.COMBINE_JO_NO,b.ORIGINAL_JO_NO FROM {0}  a  INNER JOIN	 dbo.JO_COMBINE_MAPPING b ON a.JO_NO=b.COMBINE_JO_NO) t
                                ON b.JOB_ORDER_NO=t.ORIGINAL_JO_NO
                                WHERE a.FACTORY_CD='{1}'
                                 GROUP BY t.COMBINE_JO_NO,COLOR_CD,SIZE_CD1", strTableName, factoryCd);

            return DBUtility.GetTable(SQL.ToString(), MESConn);
        }
        public static DataTable GetProTranFGISDetail(string factoryCd,string strJo, string strColor, string strSize)
        {
            System.Text.StringBuilder SQL = new System.Text.StringBuilder();
            string grncolorsiz="",giscolorsiz="";
            if (strColor != "")
            {
                grncolorsiz = grncolorsiz + " and b.COLOR_CD='" + strColor + "' ";
                giscolorsiz = giscolorsiz + " and b.COLOR_CD='" + strColor + "' ";
            }
            if (strSize != "")
            {
              grncolorsiz = grncolorsiz + " and b.SIZE_CD='" + strSize + "' ";
              giscolorsiz = giscolorsiz + " and b.SIZE_CD1='" + strSize + "' ";
            }
            SQL.AppendFormat(@"                           
                               SELECT  GTN_TYPE,SUM(QTY) AS QTY
                                FROM                          
                                (SELECT CASE WHEN LOWER(GTN_TYPE)='leftover' THEN 'LEFTOVER' ELSE 'BULK' END AS GTN_TYPE ,SUM(QTY) QTY FROM dbo.PRD_GARMENT_TRANSFER_HD(NOLOCK) a INNER JOIN 
 
                                 dbo.PRD_GARMENT_TRANSFER_DFT(NOLOCK) b ON a.DOC_NO = b.DOC_NO
                                 WHERE a.FACTORY_CD='{0}' AND b.JOB_ORDER_NO='{1}'
                                 {2} AND a.PROCESS_CD='FIN'
                                 GROUP BY  GTN_TYPE
                               union all
                                SELECT  'BULK' as GTN_TYPE ,SUM(DISCREPANCY_QTY) QTY FROM dbo.PRD_JO_DISCREPANCY_PULLOUT_HD(NOLOCK) a INNER JOIN 
 
                                 dbo.PRD_JO_DISCREPANCY_PULLOUT_TRX(NOLOCK) b ON a.DOC_NO = b.DOC_NO
                                 WHERE a.FACTORY_CD='{0}' AND b.JOB_ORDER_NO='{1}' 
                                 {2} AND a.PROCESS_CD='FIN'
     
                                UNION all 
 
                                SELECT TRX_TYPE,-SUM(QTY) AS QTY FROM dbo.IMP_FGIS_DATA_HD(NOLOCK) a INNER JOIN 
                                                            dbo.IMP_FGIS_DATA_DT (NOLOCK) b ON a.FGIS_NO=b.FGIS_NO AND ISNULL(a.SOURCE_FACTORY,'')=ISNULL(a.SOURCE_FACTORY,'')
                                                            WHERE 
                                                            a.FACTORY_CD='{0}' AND b.JOB_ORDER_NO in
                                                            ( SELECT	'{1}'
                                                            UNION
                                                            SELECT ORIGINAL_JO_NO FROM dbo.JO_COMBINE_MAPPING WHERE COMBINE_JO_NO='{1}')
                                {3}
                                                            GROUP BY TRX_TYPE)t
                                GROUP BY GTN_TYPE", factoryCd, strJo, grncolorsiz, giscolorsiz);

            return DBUtility.GetTable(SQL.ToString(), "MES");
        }
        public static DataTable GetProStatusQueryProcessDetail(string factoryCd, string strJo, string strColor, string strSize, string process_cd, string process_garment_type, string process_type, string process_production_factory, string next_process_cd, string next_process_garment_type, string next_process_type, string next_process_production_factory)
        {
            string SQL = " select CONVERT(VARCHAR(10),A.TRX_DATE,101) AS DATE,A.Production_Line_CD,SUM(A.OUTPUT_QTY) AS QTY ";
            SQL = SQL + "from PRD_JO_OUTPUT_TRX a ";
            SQL = SQL + "inner join PRD_JO_OUTPUT_HD b ";
            SQL = SQL + "on b.doc_no=a.doc_no ";
            SQL = SQL + "and a.job_order_no='" + strJo + "' ";
            if (strColor != "")
            {
                SQL = SQL + " and a.color_code='" + strColor + "' ";
            }
            if (strSize != "")
            {
                SQL += " and a.size_code='" + strSize + "' ";
            }
            if (next_process_cd != "")
            {
                SQL = SQL + "and B.NEXT_PROCESS_CD='" + next_process_cd + "' ";
            }
            SQL = SQL + "and A.process_cd='" + process_cd + "' ";
            SQL = SQL + " and a.process_garment_type='" + process_garment_type + "' ";
            SQL = SQL + " and a.process_type='" + process_type + "' ";
            if (next_process_garment_type != "")
            {
                SQL = SQL + " and B.next_process_garment_type='" + next_process_garment_type + "' ";
            }
            if (next_process_type != "")
            {
                SQL = SQL + " and b.next_process_type='" + next_process_type + "' ";
            }
            if (next_process_production_factory != "")
            {
                SQL = SQL + " and b.next_process_production_factory='" + next_process_production_factory + "' ";
            }
            SQL = SQL + "WHERE A.FACTORY_CD='" + factoryCd + "' ";
            SQL = SQL + "GROUP BY  A.TRX_DATE,A.Production_Line_CD ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetProStatusQueryProcessDiscPullInPullOutDetail(string factoryCd, string strJo, string strColor, string strSize, string process_cd, string process_garment_type, string process_type, string process_production_factory, string detail_Type)
        {
            string SQL = " SELECT CONVERT(VARCHAR(10), A.TRX_DATE, 101) AS DATE ,B.PRODUCTION_LINE_CD,";
            if (detail_Type.ToString().ToLower().Equals("discrepancy"))
            {
                SQL = SQL + " SUM(B.DISCREPANCY_QTY) AS QTY";
            }
            else
            {
                SQL = SQL + " SUM(C.PULLOUT_QTY) AS QTY";
            }
            SQL = SQL + @" FROM   dbo.PRD_JO_DISCREPANCY_PULLOUT_HD AS A
                                    INNER JOIN dbo.PRD_JO_DISCREPANCY_PULLOUT_TRX AS B ON B.DOC_NO = A.DOC_NO
                                    LEFT JOIN dbo.PRD_JO_PULLOUT AS C ON B.TRX_ID = C.TRX_ID
                             WHERE ";
            if (detail_Type.ToString().ToLower().Equals("pullin"))
            {
                SQL = SQL + " C.JO_NO = '" + strJo + "'";
            }
            else
            {
                SQL = SQL + " B.JOB_ORDER_NO = '" + strJo + "'";

            }

            SQL = SQL + " AND A.PROCESS_CD = '" + process_cd + "' ";
            SQL = SQL + " AND A.GARMENT_TYPE = '" + process_garment_type + "'";
            SQL = SQL + " AND A.PROCESS_TYPE = '" + process_type + "'";
            if (strColor != "")
            {
                SQL = SQL + " AND B.color_code = '" + strColor + "'";
            }
            SQL = SQL + " AND a.FACTORY_CD = '" + factoryCd + "'";
            SQL = SQL + " GROUP BY A.TRX_DATE,B.PRODUCTION_LINE_CD ";

            if (detail_Type.ToString().ToLower().Equals("discrepancy"))
            { SQL = SQL + " HAVING SUM(B.DISCREPANCY_QTY)<>0 "; }
            else
            { SQL = SQL + " HAVING SUM(B.PULLOUT_QTY)<>0 "; }
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetProTranWIPSummaryProcessDiscPullInPullOutDetail(string factoryCd, string strJo, string strColor, string strSize, String process_cd, string process_garment_type, string process_type, string process_production_factory, string detail_Type)
        {
            string SQL = " SELECT CONVERT(VARCHAR(10), A.TRX_DATE, 101) AS DATE ,";
            if (detail_Type.ToString().ToLower().Equals("discrepancy"))
            {
                SQL = SQL + " SUM(B.DISCREPANCY_QTY) AS QTY";
            }
            else
            {
                SQL = SQL + " SUM(C.PULLOUT_QTY) AS QTY";
            }
            SQL = SQL + @" FROM   dbo.PRD_JO_DISCREPANCY_PULLOUT_HD AS A
                                    INNER JOIN dbo.PRD_JO_DISCREPANCY_PULLOUT_TRX AS B ON B.DOC_NO = A.DOC_NO
                                    LEFT JOIN dbo.PRD_JO_PULLOUT AS C ON B.TRX_ID = C.TRX_ID
                             WHERE ";
            if (detail_Type.ToString().ToLower().Equals("pullin"))
            {
                SQL = SQL + " C.JO_NO = '" + strJo + "'";
            }
            else
            {
                SQL = SQL + " B.JOB_ORDER_NO = '" + strJo + "'";

            }

            SQL = SQL + " AND A.PROCESS_CD = '" + process_cd + "' ";
            SQL = SQL + " AND A.GARMENT_TYPE = '" + process_garment_type + "'";
            SQL = SQL + " AND A.PROCESS_TYPE = '" + process_type + "'";
            SQL = SQL + " AND B.color_code = '" + strColor + "'";
            SQL = SQL + " AND a.FACTORY_CD = '" + factoryCd + "'";
            SQL = SQL + " GROUP BY A.TRX_DATE ";

            if (detail_Type.ToString().ToLower().Equals("discrepancy"))
            { SQL = SQL + " HAVING SUM(B.DISCREPANCY_QTY)<>0 "; }
            else
            { SQL = SQL + " HAVING SUM(B.PULLOUT_QTY)<>0 "; }
            return DBUtility.GetTable(SQL, "MES");
        }


        public static DataTable GetProTranWIPSummarySize(string factoryCd, string strJo, string strColor, string strSize)
        {
            string tableName = "";
            Random ro = new Random(1000);
            tableName = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
            string SQL = "exec [dbo].[PROC_INQ_WIP_BY_JOCOLORSIZE_NEW] '" + tableName + "','" + factoryCd + "','" + strJo + "','" + strColor + "','" + strSize + "','N';";
            SQL += " update " + tableName + " set totalcutqty=b.qty from " + tableName + " a left  join jo_dt b on a.jo COLLATE DATABASE_DEFAULT =b.jo_no  and a.color COLLATE DATABASE_DEFAULT =b.color_code  and (a.size COLLATE DATABASE_DEFAULT =(b.size_code1 + (case when b.size_code2<>'-' then ' ' +b.size_code2 else '' end) ))";
            SQL = SQL + " select * from " + tableName + " order by jo,color,size";
            return DBUtility.GetTable(SQL, "MES_UPDATE");
        }


        public static bool IsIncludeOutsourceOutput(string factory)
        {
            string sql;
            sql = "select 1 from gen_system_setting where FACTORY_CD='" + factory + "' and SYSTEM_KEY='WIP_INCLUDE_OS_OUT' and SYSTEM_VALUE='N'";
            DataTable table = DBUtility.GetTable(sql, "MES");
            return table.Rows.Count > 0 ? false : true;
        }

    }
}