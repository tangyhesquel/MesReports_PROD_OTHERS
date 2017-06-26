using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web.UI.WebControls;

namespace MESComment
{
    /// <summary>
    ///proCycleDetailSql 的摘要说明
    /// </summary>
    public class proCycleDetailSql
    {
       
        public static DataTable GetProCycleDetailList(string factoryCd, string garmentType, string washType, string processCd,string processType,string prodFactory, string prodLine, string outSourceType, string startDate, string endDate, bool blCheck, bool byProdLine, bool SLnm, string GroupName, bool ProcessCloseStatus)
        {
            string SQL = " ";

            SQL += " DECLARE @JO TABLE (JO nvarchar(15), production_line_cd nvarchar(60))"; //Added By MunFoong on 2014.08.19 MES-135
            SQL += " DECLARE @ACTUAL TABLE(JO nvarchar(15), production_line_cd nvarchar(60), actual_cut_qty int)"; //Added By MunFoong on 2014.08.19 MES-135
            SQL += " DECLARE @FRDATA TABLE (JO nvarchar(15), ex_fty_date smalldatetime)"; //Added by Jin Song 2014-09-15 (Bug fix)
            SQL += "IF OBJECT_ID('TEMPDB..#TEMP1') IS NOT NULL DROP TABLE #TEMP1 ";
            if (!GroupName.Equals(""))
            {
                SQL = SQL + "       declare @s nvarchar(max)";
                SQL = SQL + "       select @s=system_value from gen_system_setting  WITH(NOLOCK)  where factory_cd='" + factoryCd + "' and system_key='" + GroupName + "';";
            }
            SQL += " SELECT   a.process_cd,A.PROCESS_GARMENT_TYPE,A.PROCESS_TYPE,A.PROCESS_PRODUCTION_FACTORY,C.DISPLAY_SEQ ,"; //Added By ZouShiChang ON 2014.01.09 (PROCESS_PRODUCTION_FACTORY)
            if (byProdLine)
            {
                if (SLnm)
                {
                    SQL += " production_line_cd=(CASE WHEN ISNULL(a.Production_line_cd,'')=ISNULL(LINE.PRODUCTION_LINE_NAME,'')  THEN";
                    SQL += " ISNULL(a.Production_line_cd,'') ELSE ISNULL(a.PRODUCTION_LINE_CD,'') + '(' +  ISNULL(LINE.PRODUCTION_LINE_NAME,'') + ')'  END ) ";
                    SQL += " ,  dbo.DATE_FORMAT (po.buyer_po_del_date, 'yyyy-mm-dd') AS BPO_Date,A.job_order_no, po.style_no, ";
                }
                else
                {
                    SQL += "     a.production_line_cd, dbo.DATE_FORMAT (po.buyer_po_del_date, 'yyyy-mm-dd') AS BPO_Date, A.job_order_no, po.style_no, ";
                }
            }
            else if (string.IsNullOrEmpty(prodLine))
            {
                SQL += "     '' AS production_line_cd,  dbo.DATE_FORMAT (po.buyer_po_del_date, 'yyyy-mm-dd') AS BPO_Date, A.job_order_no, po.style_no, ";
            }
            else
            {
                SQL += "    '" + prodLine + "' AS production_line_cd,dbo.DATE_FORMAT (po.buyer_po_del_date, 'yyyy-mm-dd') AS BPO_Date, A.job_order_no, po.style_no, ";
            }
            SQL += " po.customer_cd, b.short_name AS customer_name, po.wash_type_cd AS wash_type, ";
            SQL += " 0 AS actual_cut_qty, '1900-01-01' AS ex_fty_date,"; //Modified by MunFoong on 2014.08.19 MES-135 //Modified by Jin Song 2014-09-15 (Bug fix)
            SQL += " dbo.DATE_FORMAT (A.trx_date, 'yyyy-mm-dd') trx_date,  ";
            SQL += " SUM (isnull (opening_qty, 0)) opening_qty,SUM (isnull (in_qty, 0)) in_qty,SUM (isnull (out_qty, 0)) AS out_qty, ";
            SQL += " SUM(ISNULL(DISCREPANCY_QTY,0)) AS DISCREPANCY_QTY,";
            SQL += " SUM(ISNULL(PULLOUT_QTY,0)) AS PULLOUT_QTY,";
            //Modified By Zikuan MES-069
            //SQL += " SUM (isnull (opening_qty, 0))+ SUM (isnull (in_qty, 0))- SUM (isnull (out_qty, 0))-SUM(ISNULL(DISCREPANCY_QTY,0))- SUM(ISNULL(PULLOUT_QTY,0))  AS wip_qty ";  
            SQL += " SUM (isnull (wip_qty, 0)) AS wip_qty ";
            //End Modified MES-069
            if (ProcessCloseStatus)
            {
                SQL += " ,prd.prod_complete_date,prd.Prod_complete";
            }

            SQL += " INTO #TEMP1 ";
            SQL += " FROM   (  ";
            SQL += " SELECT  job_order_no,Factory_cd,PROCESS_CD,PROCESS_GARMENT_TYPE,PROCESS_TYPE,PROCESS_PRODUCTION_FACTORY,CASE WHEN PRODUCTION_LINE_CD='NA' THEN '' ELSE PRODUCTION_LINE_CD END AS PRODUCTION_LINE_CD, TRX_DATE ,"; //Added By ZouShiChang ) ON 2014.08.26 MES 024 增加GARMENT_TYPE,PROCESS_TYPE
            SQL += "  SUM(OPENING_QTY) AS OPENING_QTY,SUM(in_qty) AS in_qty,SUM(out_qty) AS out_qty,SUM(PULLOUT_QTY) AS PULLOUT_QTY, ";
            //Modified By Zikuan MES-069
            //SQL += "  SUM(DISCREPANCY_QTY) AS DISCREPANCY_QTY FROM (";
            SQL += "  SUM(DISCREPANCY_QTY) AS DISCREPANCY_QTY, SUM(wip_qty) AS wip_qty FROM (";
            //End Modified MES-069
            SQL += "  SELECT  JOB_ORDER_NO,FACTORY_CD,DBO.DATE_FORMAT(trx_date,'yyyy-mm-dd') AS trx_date , ";
            SQL += "  PROCESS_CD,PROCESS_GARMENT_TYPE,PROCESS_TYPE,PROCESS_PRODUCTION_FACTORY,CASE WHEN PRODUCTION_LINE_CD='NA' THEN '' ELSE PRODUCTION_LINE_CD END AS PRODUCTION_LINE_CD,"; //增加PROCESS_GARMENT_TYPE,PROCESS_TYPE
            SQL += "  ISNULL(OPENING_QTY,0) AS OPENING_QTY ,ISNULL(in_qty,0) AS in_qty ,ISNULL(out_qty,0) AS out_qty , ";
            //Modified By Zikuan MES-069
            //SQL += " 0 As PULLOUT_QTY,0 AS DISCREPANCY_QTY  FROM  prd_jo_daily_stock  WITH  (NOLOCK)    WHERE 1=1 ";
            SQL += " 0 As PULLOUT_QTY,0 AS DISCREPANCY_QTY, ISNULL(END_WIP,0) AS wip_qty  FROM  prd_jo_daily_stock  WITH  (NOLOCK)    WHERE 1=1 ";
            //End Modified MES-069
            SQL += " AND Factory_cd =  '" + factoryCd + "'";
            SQL += " and trx_date >= '" + startDate + "'";
            SQL += " and trx_date <= '" + endDate + "'";
            //Modified By Zikuan MES-069
            SQL += " and WORK_DAY = 'Y' ";
            //End Modified MES-069
            SQL += "           UNION ALL  SELECT  TRX.JOB_ORDER_NO, HD.FACTORY_CD,TRX.TRX_DATE,HD.PROCESS_CD,HD.GARMENT_TYPE AS PROCESS_GARMENT_TYPE,HD.PROCESS_TYPE,Hd.PRODUCTION_FACTORY AS PROCESS_PRODUCTION_FACTORY, CASE WHEN HD.PRODUCTION_LINE_CD='NA' THEN '' ELSE HD.PRODUCTION_LINE_CD END  AS PRODUCTION_LINE_CD, "; //Added By ZouShiChang ) ON 2014.08.26 MES 024 增加GARMENT_TYPE,PROCESS_TYPE
            SQL += " 0 AS OPENING_QTY,0 AS in_qty,0 AS out_qty,    ISNULL(TRX.PULLOUT_QTY,0) AS PULLOUT_QTY,	ISNULL(TRX.DISCREPANCY_QTY,0) AS DISCREPANCY_QTY  ";
            //Modified By Zikuan MES-069
            SQL += " ,0 AS wip_qty ";
            //End Modified MES-069
            SQL += "  FROM PRD_JO_DISCREPANCY_PULLOUT_TRX TRX  WITH(NOLOCK) ,	PRD_JO_DISCREPANCY_PULLOUT_HD HD  WITH(NOLOCK)  WHERE TRX.DOC_NO=HD.DOC_NO ";
            SQL += "    AND HD.FACTORY_CD = '" + factoryCd + "'";
            SQL += " AND TRX.trx_date >= '" + startDate + "'";
            SQL += " AND TRX.trx_date <= '" + endDate + "' ";
            SQL += " and trx.trx_date <=(SELECT CONVERT(varchar(12),MAX(CONVERT(smalldatetime,TRX_DATE,101)),101) AS MAX_TRX_DATE ";
            SQL += " From PRD_CLOSING_HISTORY  WITH(NOLOCK)";
            SQL += " WHERE LEN(TRX_DATE)>7 AND FACTORY_CD = '" + factoryCd + "'  and entry_type='JOD')";
            if (!processCd.Equals(""))
            {
                SQL += " AND PROCESS_CD = '" + processCd + "'";
            }
            //Added By ZouShiChang ON 2013.09.23 Start MES024
            if (!garmentType.Equals(""))
            {
                SQL += " and GARMENT_TYPE='" + garmentType + "' ";
            }
            if (!processType.Equals(""))
            {
                SQL += " and PROCESS_TYPE='" + processType + "' ";
            }
            if (!prodFactory.Equals(""))
            {
                SQL += " and PRODUCTION_FACTORY='" + prodFactory + "' ";
            }
            //Added By ZouShiChang ON 2013.09.23 End MES024
            SQL += ") Z GROUP BY job_order_no,Factory_cd,PROCESS_CD,PROCESS_GARMENT_TYPE,PROCESS_TYPE,PROCESS_PRODUCTION_FACTORY,TRX_DATE,PRODUCTION_LINE_CD )A "; //Added By ZouShiChang ) ON 2014.08.26 MES 024 增加GARMENT_TYPE,PROCESS_TYPE
            SQL += " inner join jo_hd po    WITH(NOLOCK)    on a.job_order_no = po.jo_no     ";
            //SQL += " left join dbo.FR_JO_PLAN_DATA fr WITH(NOLOCK) on po.jo_no = fr.jo "; //Added by MunFoong on 2014.07.24 MES-135 
            SQL += " inner join dbo.GEN_WASH_TYPE gwt   WITH(NOLOCK) on po.wash_type_cd = gwt.wash_type_cd  ";
            if (garmentType != "")
            {
                SQL += " and (PO.Garment_Type_Cd='" + garmentType + "' OR '" + garmentType + "' is null)";
            }
            switch (washType)
            {
                case "NW":
                    SQL += " AND po.WASH_TYPE_CD ='" + washType + "'";
                    break;
                case "WASH":
                    SQL += " AND (po.WASH_TYPE_CD NOT IN('NW') AND po.WASH_TYPE_CD IS NOT NULL)";
                    break;
                case "NOIRON":
                    SQL += " AND gwt.wash_grp_cd = 'WRINKLE_FREE' ";
                    break;
                case "IRON":
                    SQL += " AND ( ISNULL(gwt.wash_grp_cd,'NULL') <> 'WRINKLE_FREE')";
                    break;
            }
            SQL += " inner join gen_customer b    WITH(NOLOCK) on  po.customer_cd = b.customer_cd  ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL += " inner join gen_prc_cd_mst c    WITH(NOLOCK) on   c.prc_cd = a.process_cd      AND a.factory_cd = c.factory_cd     AND c.end_process_flag IS NULL  ";
            SQL += " inner join gen_prc_cd_mst c    WITH(NOLOCK) on   c.prc_cd = a.process_cd  AND C.GARMENT_TYPE=A.PROCESS_GARMENT_TYPE   AND a.factory_cd = c.factory_cd     AND c.end_process_flag IS NULL  ";
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            SQL += " inner join SC_HD SC    WITH(NOLOCK) on sc.sc_no=po.sc_no   ";
            if (ProcessCloseStatus)
            {
                //Added By ZouShiChang ON 2013.08.21 Start MES 024
                //SQL += " left join PRD_JO_COMPLETE_STATUS prd with(NOLOCK) on prd.job_order_no=po.jo_no and prd.process_cd=a.process_cd";
                SQL += " left join PRD_JO_COMPLETE_STATUS prd with(NOLOCK) on prd.job_order_no=po.jo_no and prd.process_cd=a.process_cd AND prd.GARMENT_TYPE=A.PROCESS_GARMENT_TYPE AND PRD.PROCESS_TYPE=A.PROCESS_TYPE ";
                //Added By ZouShiChang ON 2013.08.21 End MES 024
            }
            switch (outSourceType)
            {
                case "OUTSOURCE":
                    SQL += " AND SC.SAM_GROUP_CD='OUTSOURCE'";
                    break;
                case "STANDARD":
                    SQL += " AND SC.SAM_GROUP_CD<>'OUTSOURCE'";
                    break;
            }
            if (SLnm)
            {
                SQL = SQL + " left join GEN_PRODUCTION_LINE line   WITH(NOLOCK) ";
                SQL = SQL + " on line.PRODUCTION_LINE_CD=a.PRODUCTION_LINE_CD ";
                //Added By ZouShiChang ON 2013.08.21 Start MES 024
                //SQL = SQL + " AND line.PROCESS_CD=a.process_cd ";
                SQL = SQL + " AND line.PROCESS_CD=a.process_cd and lINE.GARMENT_TYPE_CD=A.PROCESS_GARMENT_TYPE";
                //Added By ZouShiChang ON 2013.08.21 Start MES 024
                SQL = SQL + " AND line.FACTORY_CD=a.factory_cd";
            }
            SQL += " WHERE  a.factory_cd = '" + factoryCd + "'  ";
            SQL += " AND A.FACTORY_CD=CASE WHEN A.PROCESS_TYPE='P' THEN po.CO_FACTORY_CD ELSE a.FACTORY_CD END  "; //Added By ZouShiChang ON 2013.01.24 
            if (blCheck)
            {
                SQL += " AND exists (SELECT job_order_no, MIN(ACTUAL_PRINT_DATE) from ";
                SQL += " V_ACTUAL_CUT_BUNDLE bd   WITH(NOLOCK) WHERE  factory_cd=A.factory_cd AND job_order_no=A.JOB_ORDER_NO ";
                SQL += " GROUP BY  job_order_no ";
                SQL += " HAVING  MIN(ACTUAL_PRINT_DATE)>=DBO.DATE_FORMAT('2011-03-01','yyyy-mm-dd')) ";
            }
            if (processCd != "")
            {
                SQL += " and (a.process_cd = '" + processCd + "' )";
            }

            if (!garmentType.Equals(""))
            {
                SQL += " and A.PROCESS_GARMENT_TYPE='" + garmentType + "' ";
            }
            if (!processType.Equals(""))
            {
                SQL += " and A.PROCESS_TYPE='" + processType + "' ";
            }
            if (!prodFactory.Equals(""))
            {
                SQL += " and A.PROCESS_PRODUCTION_FACTORY='" + prodFactory + "' ";
            }
            if (prodLine != "")
            {
                SQL += " and (a.production_line_cd = '" + prodLine + "' )";
            }
            SQL += " AND A.trx_date >= '" + startDate + "'";
            SQL += " AND A.trx_date <= '" + endDate + "' ";
            if (!GroupName.Equals(""))
            {
                SQL = SQL + "AND A.PRODUCTION_LINE_CD IN (select FNFIELD from dbo.FN_SPLIT_STRING_TB(@s,','))";
            }
            SQL += " GROUP BY A.job_order_no, po.style_no, po.customer_cd, a.process_cd,A.PROCESS_GARMENT_TYPE,A.PROCESS_TYPE,A.PROCESS_PRODUCTION_FACTORY, C.DISPLAY_SEQ , ";
            SQL += " dbo.DATE_FORMAT (po.buyer_po_del_date, 'yyyy-mm-dd'), b.short_name, po.wash_type_cd,"; //fr.plan_ex_fty_date, "; //Modified by MunFoong 2014.07.24 MES-135 //Modified by Jin Song 2014-09-15 (Bug fix)
            SQL += " dbo.DATE_FORMAT (A.trx_date, 'yyyy-mm-dd') ";
            if (byProdLine)
            {
                SQL += " ,a.production_line_cd";
            }
            if (SLnm)
            {
                SQL += ",PRODUCTION_LINE_NAME";
            }
            if (ProcessCloseStatus)
            {
                SQL += " ,prd.Prod_complete_date,prd.Prod_complete ";
            }


            //Added By MunFoong on 2014.08.19 MES-135
            //Bug fix by Jin Song 2014-08-25, checking by Prod Line
            SQL += " INSERT INTO @JO  SELECT distinct job_order_no, production_line_cd from #TEMP1";
            SQL += " WHERE (opening_qty<>0 OR in_qty<>0 OR out_qty<>0 OR discrepancy_qty<>0 OR pullout_qty<>0)";

            SQL += " INSERT INTO @ACTUAL SELECT a.JO, a.production_line_cd, isnull(SUM(b.QTY),0) from @JO a";
            SQL += " LEFT JOIN CUT_BUNDLE_HD b with(nolock) on a.JO = b.JOB_ORDER_NO ";
            //if (byProdLine) 
            //{
            //SQL += " and a.production_line_cd = b.CUT_LINE";
            //}
            SQL += " group by a.JO, a.production_line_cd";

            SQL += " UPDATE A  SET A.actual_cut_qty = B.actual_cut_qty";
            SQL += " FROM #TEMP1 A INNER JOIN @ACTUAL B on A.job_order_no = B.JO";
            //if (byProdLine)
            //{
            //SQL += " and A.production_line_cd = B.production_line_cd";
            //}
            //End By MunFoong on 2014.08.19 MES-135
            //Added by Jin Song 2014-09-15 (Bug fix)
            //Bug Fix by Jin Song 2015-01-12
            SQL += " INSERT INTO @FRDATA SELECT DISTINCT JO_NO,CASE WHEN LATEST_EX_FACTORY_DATE IS NULL THEN CASE WHEN ACT_EX_FTY_DATE IS NULL THEN PLAN_EX_FTY_DATE ELSE ACT_EX_FTY_DATE END ELSE LATEST_EX_FACTORY_DATE END AS ex_fty_date from JO_HD A WITH(NOLOCK) LEFT JOIN FR_JO_PLAN_DATA B WITH(NOLOCK) ON A.JO_NO = B.JO WHERE JO_NO IN (SELECT DISTINCT JO FROM @JO)";
            SQL += " UPDATE A SET A.ex_fty_date = CASE WHEN B.ex_fty_date IS NOT NULL THEN CONVERT(NVARCHAR(10),B.ex_fty_date,121) ELSE 'N/A' END";
            SQL += " FROM #TEMP1 A INNER JOIN @FRDATA B ON A.JOB_ORDER_NO = B.JO";

            //SQL += " ) AS MAIN_T ";
            //Added By ZouShiChang ON 2014.01.09 Start
            //SQL += "; SELECT process_cd,PROCESS_GARMENT_TYPE,PROCESS_TYPE,";
            SQL += "; SELECT PROCESS_GARMENT_TYPE+PROCESS_CD+PROCESS_TYPE+CASE WHEN PROCESS_TYPE IN ('I','E','O') THEN '' ELSE '('+PROCESS_PRODUCTION_FACTORY+')' END AS PROCESS_CD, ";
            //Added By ZouShiChang ON 2014.01.09 End
            SQL += " production_line_cd,BPO_Date, job_order_no, style_no, ";
            SQL += " customer_cd,customer_name, wash_type, actual_cut_qty, CASE ex_fty_date WHEN '1900-01-01' THEN '' ELSE CONVERT(nvarchar(10), ex_fty_date) END AS ex_fty_date, trx_date, opening_qty,in_qty,out_qty,DISCREPANCY_QTY,PULLOUT_QTY,wip_qty "; //Modified by MunFoong 2014.07.24 MES-135 //Modified by Jin Song 2014-09-15 (Bug fix)
            if (ProcessCloseStatus)
            {
                SQL += " ,dbo.DATE_FORMAT (Prod_complete_date, 'yyyy-mm-dd'),Prod_complete";
            }
            SQL += " FROM  #TEMP1 ";
            SQL += " WHERE (opening_qty<>0 OR in_qty<>0 OR out_qty<>0 OR discrepancy_qty<>0 OR pullout_qty<>0) ";
            //Modified by Jin Song MES115: Add Order by PROCESS_CD
            if (byProdLine)
            {
                SQL += " ORDER BY DISPLAY_SEQ, PROCESS_GARMENT_TYPE, PROCESS_TYPE, PROCESS_PRODUCTION_FACTORY, production_line_cd, job_order_no, trx_date ";
            }
            else
            {
                SQL += " ORDER BY DISPLAY_SEQ, PROCESS_GARMENT_TYPE, PROCESS_TYPE, PROCESS_PRODUCTION_FACTORY, job_order_no, trx_date ";
            }
            //End of modification MES115
            return DBUtility.GetTable(SQL, "MES");
        }


    }
}