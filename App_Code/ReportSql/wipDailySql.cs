using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web.UI.WebControls;
using MESComment;


namespace MESComment
{
    /// <summary>
    ///wipDailySql 的摘要说明
    /// </summary>
    public class wipDailySql
    {

        public static DataTable GetGroupName(string Factory)
        {
            string SQL = "";
            SQL += @" SELECT '' AS SYSTEM_KEY ";
            SQL += @" UNION ALL ";
            SQL += @" SELECT SYSTEM_KEY FROM GEN_SYSTEM_SETTING WHERE FACTORY_CD = '" + Factory + "'";
            SQL += @" AND SYSTEM_NAME ='LINE_GROUP'";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetDailyWip(string garmentType, string washType, string date, string factoryCd, string prodGroup)
        {
            string SQL = "";
            
            //Added by MunFoong on 2014.07.24, MES-139
            if (prodGroup != "")
            {
                SQL = SQL + "       DECLARE @S NVARCHAR(MAX)";
                SQL = SQL + "       SELECT @S = SYSTEM_VALUE FROM GEN_SYSTEM_SETTING WITH(NOLOCK) WHERE FACTORY_CD = '" + factoryCd + "' AND SYSTEM_KEY = '" + prodGroup + "'";
            }
            //End of added by MunFoong on 2014.07.24, MES-139

            SQL = SQL + "        SELECT cu.NAME BUYER, sc.SC_NO, po.JO_NO,PO.WASH_TYPE_CD, ";
            SQL = SQL + "       sc.STYLE_NO, DBO.DATE_FORMAT(BUYER_PO_del_date, 'mm/dd/yyyy') ";
            SQL = SQL + "       GMT_DEL_DATE, DT.qty ORDER_QTY, MAX (a.jo_clear_flag) ";
            SQL = SQL + "       JO_CLEAR_FLAG, isnull(cm.short_name, a.process_cd) PROCESS_CD,a.process_garment_type, "; //Added By ZouShiChang ON 2013.08.28 MES 024 (a.process_garment_type)
            SQL = SQL + "       d.output_qty DAILY, b.output_qty MONTHLY, c.qty JO_TTL FROM ";
            SQL = SQL + "       PRD_jo_output_trx a LEFT JOIN JO_hd po ON a.job_order_no = ";
            SQL = SQL + "       po.JO_no ";

            //Modified by MunFoong on 2014.07.24, MES-139
            if (prodGroup != "")
            {
                SQL = SQL + "       inner join (SELECT JO_NO,SUM(QTY) AS QTY FROM JO_DT ";
                SQL = SQL + "       WHERE JO_NO IN(SELECT job_order_no FROM PRD_jo_output_trx WHERE ";
                SQL = SQL + "       trx_date= '" + date + "' AND factory_cd = ";
                SQL = SQL + "       '" + factoryCd + "'";
                SQL = SQL + "       AND PRODUCTION_LINE_CD IN(select FNField FROM DBO.FN_SPLIT_STRING_TB(@S,',')))";
            }
            else
            {
                SQL = SQL + "       left join (SELECT JO_NO,SUM(QTY) AS QTY FROM JO_DT ";
                SQL = SQL + "       WHERE JO_NO IN(SELECT job_order_no FROM PRD_jo_output_trx WHERE ";
                SQL = SQL + "       trx_date= '" + date + "' AND factory_cd = ";
                SQL = SQL + "       '" + factoryCd + "')";
            }
            //End of modified by MunFoong on 2014.07.24, MES-139

            SQL = SQL + "       GROUP BY JO_NO) DT ON DT.JO_NO=PO.JO_NO LEFT JOIN ";
            SQL = SQL + "       sc_hd sc ON po.sc_no = sc.sc_no LEFT JOIN gen_customer cu ON ";
            SQL = SQL + "       po.customer_cd = cu.customer_cd LEFT JOIN gen_prc_cd_mst cm ON ";
            SQL = SQL + "       a.process_cd = cm.prc_cd AND A.process_garment_type=cm.garment_Type and CM.FACTORY_CD= '" + factoryCd + "' LEFT ";
            SQL = SQL + "       JOIN (SELECT job_order_no,process_cd,Garment_type as Process_garment_type,process_type,SUM(output_qty) AS output_qty "; //Added By ZouShiChang ON 2013.08.28 MES 024(Garment_type as Process_garment_type,process_type)
            SQL = SQL + "      FROM PRD_monthly_balance B";
            SQL = SQL + "      WHERE factory_cd = '" + factoryCd + "' AND [YEAR]=YEAR('" + date + "') AND B.MON=MONTH('" + date + "')";
            SQL = SQL + "      AND EXISTS (SELECT TOP 1 1 FROM PRD_jo_output_trx A left join JO_hd po on";
            SQL = SQL + "      a.job_order_no=po.JO_no  WHERE a.factory_cd = b.factory_cd AND a.job_order_no = b.job_order_no";
            SQL = SQL + "      AND a.process_cd = b.process_cd and A.process_garment_type=b.Garment_type and a.process_type=b.process_type ";
            SQL = SQL + "      AND a.trx_date= '" + date + "' ";
            if (garmentType != "")
            {
                SQL += " AND po.garment_type_cd ='" + garmentType + "'";
            }
            switch (washType)
            {
                case "NW":
                    SQL += " AND po.WASH_TYPE_CD ='" + washType + "'";
                    break;
                case "WASH":
                    SQL += " AND (po.WASH_TYPE_CD NOT IN('NW') AND po.WASH_TYPE_CD IS NOT NULL)";
                    break;
            }
            SQL = SQL + "      AND a.factory_cd = '" + factoryCd + "') GROUP BY job_order_no,process_cd,garment_type,process_type ) b ON a.job_order_no = b.job_order_no AND ";
            SQL = SQL + "       a.process_cd = b.process_cd and a.process_garment_type=b.process_garment_type and a.process_type=b.process_type LEFT JOIN (SELECT a.job_order_no, ";
            SQL = SQL + "      a.process_cd,a.process_garment_type,a.process_type,SUM (c.qty) qty FROM (select distinct factory_cd,  ";
            SQL = SQL + "      process_cd,process_garment_type,process_type,job_order_no   from PRD_jo_output_trx A  WHERE ";
            SQL = SQL + "       (a.trx_date= '" + date + "' )) a LEFT JOIN ";
            SQL = SQL + "       JO_hd po ON a.job_order_no = po.JO_no LEFT JOIN PRD_jo_balance c ";
            SQL = SQL + "       ON a.factory_cd = c.factory_cd AND a.process_cd = c.process_cd ";
            SQL = SQL + "     AND a.process_garment_type=c.garment_type and a.process_type=c.process_type ";
            SQL = SQL + "       AND a.job_order_no = c.job_order_no WHERE 1=1 ";
            if (garmentType != "")
            {
                SQL += " AND po.garment_type_cd ='" + garmentType + "'";
            }

            switch (washType)
            {
                case "NW":
                    SQL += " AND po.WASH_TYPE_CD ='" + washType + "'";
                    break;
                case "WASH":
                    SQL += " AND (po.WASH_TYPE_CD NOT IN('NW') AND po.WASH_TYPE_CD IS NOT NULL)";
                    break;
            }
            SQL = SQL + "       AND a.factory_cd = '" + factoryCd + "' GROUP BY a.job_order_no, ";
            SQL = SQL + "       a.process_cd,a.process_garment_type,a.process_type) c ON a.process_cd = c.process_cd AND ";
            SQL = SQL + "       a.process_garment_type=c.process_garment_type and a.process_type=c.process_type and ";
            SQL = SQL + "       a.job_order_no = c.job_order_no LEFT JOIN (SELECT ";
            SQL = SQL + "       d.job_order_no, d.process_cd,d.garment_type as process_garment_type,d.process_type, SUM (d.output_qty) ";
            SQL = SQL + "       output_qty FROM JO_hd po right join PRD_jo_daily_balance d on ";
            SQL = SQL + "       d.job_order_no = po.JO_no WHERE ";
            SQL = SQL + "       (d.trx_date= '" + date + "' ) ";
            if (garmentType != "")
            {
                SQL += " AND po.garment_type_cd ='" + garmentType + "'";
            }
            switch (washType)
            {
                case "NW":
                    SQL += " AND po.WASH_TYPE_CD ='" + washType + "'";
                    break;
                case "WASH":
                    SQL += " AND (po.WASH_TYPE_CD NOT IN('NW') AND po.WASH_TYPE_CD IS NOT NULL)";
                    break;
            }
            SQL = SQL + "       AND d.factory_cd = '" + factoryCd + "' GROUP BY d.job_order_no, ";
            SQL = SQL + "       d.process_cd,d.garment_type,d.process_type) d ON a.process_cd = d.process_cd and a.process_garment_type=d.process_garment_type and a.process_type=d.process_type AND a.job_order_no ";
            SQL = SQL + "       = d.job_order_no WHERE ";
            SQL = SQL + "       (a.trx_date= '" + date + "' ) ";
            if (garmentType != "")
            {
                SQL += " AND po.garment_type_cd ='" + garmentType + "'";
            }
            switch (washType)
            {
                case "NW":
                    SQL += " AND po.WASH_TYPE_CD ='" + washType + "'";
                    break;
                case "WASH":
                    SQL += " AND (po.WASH_TYPE_CD NOT IN('NW') AND po.WASH_TYPE_CD IS NOT NULL)";
                    break;
            }
            SQL = SQL + " AND A.FACTORY_CD=CASE WHEN A.PROCESS_TYPE='P' THEN po.CO_FACTORY_CD ELSE a.FACTORY_CD END  "; //added By ZouShiChang ON 2014.01.28
            SQL = SQL + "       AND a.factory_cd = '" + factoryCd + "' GROUP BY cu.NAME, sc.sc_no, ";
            SQL = SQL + "       po.JO_no, PO.WASH_TYPE_CD, sc.style_no, po.BUYER_PO_del_date, ";
            SQL = SQL + "       isnull (cm.short_name, a.process_cd),a.process_garment_type,cm.DISPLAY_SEQ , d.output_qty, "; 
            SQL = SQL + "       b.output_qty, c.qty ,DT.qty ORDER BY sc.sc_no, po.JO_no, ";
            SQL = SQL + "       cm.DISPLAY_SEQ ";

            return DBUtility.GetTable(SQL, "MES");

        }

        public static DataTable GetDailyWipTitle(string garmentType, string date, string factoryCd)
        {
            string SQL = "         SELECT A.SHORT_NAME FROM ( SELECT distinct cm.SHORT_NAME ";
            SQL = SQL + "  SHORT_NAME,CM.DISPLAY_SEQ from PRD_jo_output_trx A left join ";
            SQL = SQL + "  JO_HD PO on A.JOB_ORDER_NO=PO.JO_NO,GEN_prc_cd_mst cm WHERE ";
            SQL = SQL + "  a.PROCESS_CD=cm.Prc_cd ";            
            SQL = SQL + " AND cm.ACTIVE='Y' AND A.FACTORY_CD=CASE WHEN A.PROCESS_TYPE='P' THEN po.CO_FACTORY_CD ELSE a.FACTORY_CD END "; //Added by ZouShiChang ON 2014.01.28 
            SQL = SQL + " and a.process_garment_type=cm.garment_type "; 
            if (date != "")
            {
                SQL += " and A.TRX_DATE='" + date + "'";
            }
            if (garmentType != "")
            {
                SQL += " AND PO.GARMENT_TYPE_CD = '" + garmentType + "'";
            }
            SQL = SQL + "        AND A.FACTORY_Cd='" + factoryCd + "' AND A.FACTORY_CD=CM.FACTORY_CD) A  order by A.DISPLAY_SEQ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetDailyOutputByJOSectionDetail(string garmentType, string prodLine, string processCode,string processType,string prodFactory, string ToprodLine, string ToprocessCode,string TogarmentType,string ToprocessType,string ToproductionFactory, string date, string ToDate, string BPOdate, string BPOTodate, string factoryCd, string joNo, string stageCode, string OrderBy, string GroupName)
        {
            string SQL = "";
            SQL += @"EXEC [dbo].[GET_WIP_DAILY_REPORT] '" + garmentType + "','" + prodLine + "','" + processCode + "','" + processType + "','" + prodFactory + "','" + ToprodLine + "','" + ToprocessCode + "','" + TogarmentType + "','" + ToprocessType + "','" +ToproductionFactory+ "','" + date + "','" + ToDate + "','" + BPOdate + "','" + BPOTodate + "','" + factoryCd + "','" + joNo + "','" + stageCode + "','" + OrderBy + "','" + GroupName + "'";

            return DBUtility.GetTable(SQL, "MES");
        }


        public static DataTable GetDailyOutputByJOSectionDetail(string garmentType, string prodLine, string processCode,string processType,string prodFactory, string ToprodLine, string ToprocessCode,string ToGarmentType,string ToProcessType,string ToProdFactory, string date, string ToDate, string factoryCd, string joNo, string stageCode, string OrderBy, string GroupName)
        {
            StringBuilder sqlBuilder = new StringBuilder("");

            if (!GroupName.Equals(""))
            {
                sqlBuilder.Append(@" declare @s nvarchar(max)");
                sqlBuilder.Append(@" select @s=system_value from gen_system_setting where factory_cd='" + factoryCd + "' and system_key='" + GroupName + "';");
            }

            sqlBuilder.Append(@" IF OBJECT_ID('tempdb..#TEMP_OUTPUT') IS NOT NULL 
                                DROP TABLE #TEMP_OUTPUT;
                                IF OBJECT_ID('tempdb..#TEMP_DIS') IS NOT NULL 
                                DROP TABLE #TEMP_DIS;
                                IF OBJECT_ID('tempdb..#TEMP_HeadData') IS NOT NULL
                                    DROP TABLE #TEMP_HeadData;
                                ");


            sqlBuilder.Append(@"SELECT CU.SHORT_NAME,PO.SC_NO,A.JOB_ORDER_NO,A.PROCESS_CD,A.PROCESS_GARMENT_TYPE,A.PROCESS_TYPE,A.PROCESS_PRODUCTION_FACTORY,A.PRODUCTION_LINE_CD,A.DES_PRODUCTION_LINE_CD,PO.BUYER_PO_DEL_DATE,PO.LATEST_EX_FACTORY_DATE,
                                PO.GARMENT_TYPE_CD,SC.STYLE_NO,SC.FAB_TYPE_CD,SC.WASH_TYPE_CD,MAX (ISNULL(a.JO_CLEAR_FLAG,'N')) JO_CLEAR_FLAG
                          INTO #TEMP_HeadData
                                 FROM  PRD_JO_OUTPUT_TRX A WITH(NOLOCK) INNER JOIN PRD_JO_OUTPUT_HD B WITH(NOLOCK)
	                        ON A.DOC_NO=B.DOC_NO 
	                        INNER JOIN JO_HD PO  WITH(NOLOCK) ON PO.JO_NO=A.JOB_ORDER_NO
	                        INNER JOIN  SC_HD SC  WITH(NOLOCK) ON PO.SC_NO=SC.SC_NO
	                        LEFT JOIN GEN_CUSTOMER CU  WITH(NOLOCK) ON PO.CUSTOMER_CD=CU.CUSTOMER_CD
	                          WHERE 1=1 
                                AND A.FACTORY_CD=CASE WHEN A.PROCESS_TYPE='P' THEN po.CO_FACTORY_CD ELSE a.FACTORY_CD END                               
                                ");


            if (!factoryCd.Equals(""))
            {
                sqlBuilder.Append("AND A.FACTORY_CD='" + factoryCd + "' ");
            }

            if (!joNo.Equals(""))
            {
                sqlBuilder.Append("AND A.JOB_ORDER_NO = '" + joNo + "' ");
            }

            if (garmentType != "")
            {
                sqlBuilder.Append(" AND A.PROCESS_GARMENT_TYPE = '" + garmentType + "'");  //Modified by Jin Song 2015-02-26 Bug Fix
            }

            if (processCode != "")
            {
                sqlBuilder.Append(" AND A.PROCESS_CD='" + processCode + "' ");
            }
            
            if (!processType.Equals(""))
            {
                sqlBuilder.Append(" and A.process_type='" + processType + "' ");
            }
            if (!prodFactory.Equals(""))
            {
                sqlBuilder.Append(" AND a.Process_Production_factory='" + prodFactory + "' ");
            }
            if (!ToGarmentType.Equals(""))
            {
                sqlBuilder.Append(" and B.NEXT_PROCESS_GARMENT_TYPE='" + ToGarmentType + "' ");
            }
            if (!ToProcessType.Equals(""))
            {
                sqlBuilder.Append(" and B.NEXT_PROCESS_TYPE='" + ToProcessType + "' ");
            }
            if (!ToProdFactory.Equals(""))
            {
                sqlBuilder.Append(" and B.NEXT_PROCESS_PRODUCTION_FACTORY='" + ToProdFactory + "' ");
            }
            
            if (ToprocessCode != "")
            {
                sqlBuilder.Append(" AND B.NEXT_PROCESS_CD='" + ToprocessCode + "' ");
            }
            if (date != "")
            {
                sqlBuilder.Append(" AND A.TRX_DATE >='" + date + "'");
            }
            if (ToDate != "")
            {
                sqlBuilder.Append(" AND A.TRX_DATE <='" + ToDate + "'");
            }
            if (prodLine != "")
            {
                sqlBuilder.Append(" AND A.PRODUCTION_LINE_CD='" + prodLine + "' ");
            }
            if (ToprodLine != "")
            {
                sqlBuilder.Append(" AND A.DES_PRODUCTION_LINE_CD='" + ToprodLine + "' ");
            }
            sqlBuilder.Append(@" GROUP BY PO.SC_NO,A.JOB_ORDER_NO,A.PROCESS_CD,A.PROCESS_GARMENT_TYPE,A.PROCESS_TYPE,A.PROCESS_PRODUCTION_FACTORY,A.PRODUCTION_LINE_CD,A.DES_PRODUCTION_LINE_CD,PO.BUYER_PO_DEL_DATE,PO.LATEST_EX_FACTORY_DATE,
        PO.GARMENT_TYPE_CD,SC.STYLE_NO,SC.FAB_TYPE_CD,SC.WASH_TYPE_CD,CU.SHORT_NAME ;");




            sqlBuilder.Append(@"SELECT A.SHORT_NAME CUSTOMER_NAME,
                       (CASE WHEN ISNULL (A.SC_NO, '') = '' THEN '-' ELSE A.sc_no END)
                          SC_NO,
                       A.JOB_ORDER_NO,
                       A.GARMENT_TYPE_CD GARMENT_TYPE_CD,
                       PO.ORDER_QTY ORDER_QTY,
                       A.STYLE_NO STYLE_NO,
                       A.FAB_TYPE_CD,
                       A.WASH_TYPE_CD WASH_TYPE_CD,
                       A.PROCESS_CD,
                       A.PROCESS_GARMENT_TYPE,
                       A.PROCESS_TYPE,
                       A.PROCESS_PRODUCTION_FACTORY,
                       (CASE
                           WHEN ISNULL (A.PRODUCTION_LINE_CD, '') = '' THEN ''
                           ELSE A.PRODUCTION_LINE_CD
                        END)
                          PRODUCTION_LINE_CD,
                       (CASE
                           WHEN ISNULL (A.DES_PRODUCTION_LINE_CD, '') = '' THEN ''
                           ELSE A.DES_PRODUCTION_LINE_CD
                        END)
                          TO_PROD_LINE,
                       PL.CUT_QTY AS CUT_QTY,
                       a.JO_CLEAR_FLAG JO_CLEAR_FLAG,
                       CONVERT (NVARCHAR, A.BUYER_PO_DEL_DATE, 23) BUYER_PO_DEL_DATE,
                       CONVERT (NVARCHAR, A.LATEST_EX_FACTORY_DATE, 23) LATEST_EX_FACTORY_DATE
                       INTO #TEMP_OUTPUT
                  FROM  #TEMP_HeadData  A
	                LEFT JOIN 
		                (
                        SELECT  DT.JO_NO,SUM(ISNULL(DT.QTY,0)) AS ORDER_QTY
                        FROM  JO_DT DT WITH(NOLOCK) 
                        WHERE EXISTS (SELECT 1 FROM #TEMP_HeadData B WITH(NOLOCK) 
				                WHERE  B.JOB_ORDER_NO = DT.JO_NO )
                        GROUP BY DT.JO_NO
                        )PO   ON A.JOB_ORDER_NO=PO.JO_NO   
                        LEFT JOIN (
                        SELECT A.JOB_ORDER_NO,SUM(A.QTY) CUT_QTY
                        FROM CUT_BUNDLE_HD A  WITH(NOLOCK)
                        WHERE  EXISTS (SELECT 1 FROM #TEMP_HeadData B WITH(NOLOCK)
                        WHERE B.JOB_ORDER_NO = A.JOB_ORDER_NO ) 
                        GROUP BY A.JOB_ORDER_NO
                        )PL ON A.JOB_ORDER_NO=PL.JOB_ORDER_NO;");


            sqlBuilder.Append(@" 
                                IF OBJECT_ID('tempdb..#TEMP_HeadData1') IS NOT NULL
                                    DROP TABLE #TEMP_HeadData1;");

            sqlBuilder.Append(@" SELECT CU.SHORT_NAME,PO.SC_NO,A.JOB_ORDER_NO,B.PROCESS_CD,B.GARMENT_TYPE AS PROCESS_GARMENT_TYPE,
                                 B.PROCESS_TYPE,B.PRODUCTION_FACTORY AS PROCESS_PRODUCTION_FACTORY,A.PRODUCTION_LINE_CD,'' AS TO_PROD_LINE ,PO.BUYER_PO_DEL_DATE,
                             PO.LATEST_EX_FACTORY_DATE,
                            PO.GARMENT_TYPE_CD,SC.STYLE_NO,SC.FAB_TYPE_CD,SC.WASH_TYPE_CD,'' AS  JO_CLEAR_FLAG
	                    INTO #TEMP_HeadData1
                             FROM  PRD_JO_DISCREPANCY_PULLOUT_TRX A WITH(NOLOCK) INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_HD B WITH(NOLOCK)
	                    ON A.DOC_NO=B.DOC_NO 
	                    INNER JOIN JO_HD PO  WITH(NOLOCK) ON PO.JO_NO=A.JOB_ORDER_NO
	                    INNER JOIN  SC_HD SC  WITH(NOLOCK) ON PO.SC_NO=SC.SC_NO
	                    LEFT JOIN GEN_CUSTOMER CU  WITH(NOLOCK) ON PO.CUSTOMER_CD=CU.CUSTOMER_CD
	                      WHERE 1=1 ");

            if (!factoryCd.Equals(""))
            {
                sqlBuilder.Append("AND A.FACTORY_CD='" + factoryCd + "' ");
            }
            if (!joNo.Equals(""))
            {
                sqlBuilder.Append("AND A.JOB_ORDER_NO = '" + joNo + "' ");
            }
            if (garmentType != "")
            {
                sqlBuilder.Append(" AND B.GARMENT_TYPE = '" + garmentType + "'"); //Modified by Jin Song 2015-02-26 Bug Fix
            }
            if (processCode != "")
            {
                sqlBuilder.Append(" AND B.PROCESS_CD='" + processCode + "' ");
            }
            
            if (!processType.Equals(""))
            {
                sqlBuilder.Append(" and B.PROCESS_TYPE='" + processType + "' ");
            }
            if (!prodFactory.Equals(""))
            {
                sqlBuilder.Append(" and B.PRODUCTION_FACTORY='" + prodFactory + "' ");
            }
            
            if (date != "")
            {
                sqlBuilder.Append(" AND A.TRX_DATE >='" + date + "'");
            }
            if (ToDate != "")
            {
                sqlBuilder.Append(" AND A.TRX_DATE <='" + ToDate + "'");
            }
            if (prodLine != "")
            {
                sqlBuilder.Append(" AND A.PRODUCTION_LINE_CD='" + prodLine + "' ");
            }


            sqlBuilder.Append(@" GROUP BY PO.SC_NO,A.JOB_ORDER_NO,B.PROCESS_CD,B.GARMENT_TYPE,B.PROCESS_TYPE,B.PRODUCTION_FACTORY,A.PRODUCTION_LINE_CD,PO.BUYER_PO_DEL_DATE,PO.LATEST_EX_FACTORY_DATE,
        PO.GARMENT_TYPE_CD,SC.STYLE_NO,SC.FAB_TYPE_CD,SC.WASH_TYPE_CD,CU.SHORT_NAME;");

            sqlBuilder.Append(@" SELECT A.SHORT_NAME CUSTOMER_NAME,
                       (CASE WHEN ISNULL (A.SC_NO, '') = '' THEN '-' ELSE A.SC_NO END)
                          SC_NO,
                       A.JOB_ORDER_NO,
                       A.GARMENT_TYPE_CD GARMENT_TYPE_CD,
                       PO.ORDER_QTY ORDER_QTY,
                       A.STYLE_NO STYLE_NO,
                       A.FAB_TYPE_CD,
                       A.WASH_TYPE_CD WASH_TYPE_CD,
                       A.PROCESS_CD,
                       A.PROCESS_GARMENT_TYPE,
                       A.PROCESS_TYPE, 
                       A.PROCESS_PRODUCTION_FACTORY,
                       (CASE
                           WHEN ISNULL (A.PRODUCTION_LINE_CD, '') = '' THEN ''
                           ELSE A.PRODUCTION_LINE_CD
                        END)
                          PRODUCTION_LINE_CD,
                       A.TO_PROD_LINE,
                       PL.CUT_QTY AS CUT_QTY,
                       A.JO_CLEAR_FLAG,
                       CONVERT (NVARCHAR, A.BUYER_PO_DEL_DATE, 23) BUYER_PO_DEL_DATE,
                       CONVERT (NVARCHAR, A.LATEST_EX_FACTORY_DATE, 23) LATEST_EX_FACTORY_DATE
                  INTO #TEMP_DIS
                  FROM #TEMP_HeadData1 A        
                   LEFT JOIN 
		                (
                        SELECT  DT.JO_NO,SUM(ISNULL(DT.QTY,0)) AS ORDER_QTY
                        FROM  JO_DT DT WITH(NOLOCK) 
                        WHERE EXISTS (SELECT 1 FROM #TEMP_HeadData1 B WITH(NOLOCK) 
				                WHERE  B.JOB_ORDER_NO = DT.JO_NO )
                        GROUP BY DT.JO_NO
                        )PO   ON A.JOB_ORDER_NO=PO.JO_NO   
                        LEFT JOIN (
                        SELECT A.JOB_ORDER_NO,SUM(A.QTY) CUT_QTY
                        FROM CUT_BUNDLE_HD A  WITH(NOLOCK)
                        WHERE  EXISTS (SELECT 1 FROM #TEMP_HeadData1 B WITH(NOLOCK)
                        WHERE B.JOB_ORDER_NO = A.JOB_ORDER_NO ) 
                        GROUP BY A.JOB_ORDER_NO
                        )PL ON A.JOB_ORDER_NO=PL.JOB_ORDER_NO ;");





            sqlBuilder.Append(@" IF OBJECT_ID('tempdb..#TEMP_JOUPToDay') IS NOT NULL 
                                DROP TABLE #TEMP_JOUPToDay;
                                SELECT JOB_ORDER_NO,PROCESS_CD,
                                PROCESS_GARMENT_TYPE,PROCESS_TYPE,PROCESS_PRODUCTION_FACTORY,Production_line_cd,TO_PROD_LINE
                                ,JODAILY=(CASE WHEN  TRX_DATE>='" + date + "' and TRX_DATE<='" + ToDate + "' THEN (JO_TTL) ELSE 0 END),");

            sqlBuilder.Append(@" JOMONTH=(CASE WHEN  YEAR(TRX_DATE)=YEAR('" + date + "') AND MONTH(TRX_DATE)=MONTH('" + date + "') THEN (JO_TTL) ELSE 0 END),");
            sqlBuilder.Append(@" JOUPToDay=(CASE WHEN  TRX_DATE<='" + ToDate + "' THEN (JO_TTL) ELSE 0 END),JOTIL=(JO_TTL)");
            sqlBuilder.Append(@"  INTO #TEMP_JOUPToDay
                                FROM (
                                SELECT A.TRX_DATE,A.JOB_ORDER_NO, A.PROCESS_CD ,A.PROCESS_GARMENT_TYPE,A.PROCESS_TYPE,A.PROCESS_PRODUCTION_FACTORY,A.Production_line_cd,A.DES_Production_line_cd AS TO_PROD_LINE
                                ,SUM(ISNULL(OUTPUT_QTY,0)) AS JO_TTL
                                FROM PRD_JO_OUTPUT_TRX A WITH(NOLOCK)
                                INNER JOIN PRD_JO_OUTPUT_HD B WITH(NOLOCK) ON A.DOC_NO = B.DOC_NO
                                WHERE  1=1 ");
            if (!factoryCd.Equals(""))
            {
                sqlBuilder.Append("AND A.FACTORY_CD='" + factoryCd + "' ");
            }
            if (!joNo.Equals(""))
            {
                sqlBuilder.Append("AND A.JOB_ORDER_NO = '" + joNo + "' ");
            }
            if (processCode != "")
            {
                sqlBuilder.Append(" AND A.PROCESS_CD='" + processCode + "'");
            }

            if (!processType.Equals(""))
            {
                sqlBuilder.Append(" and A.process_type='" + processType + "' ");
            }
            if (!prodFactory.Equals(""))
            {
                sqlBuilder.Append(" AND a.Process_Production_factory='" + prodFactory + "' ");
            }
            if (!ToGarmentType.Equals(""))
            {
                sqlBuilder.Append(" and B.NEXT_PROCESS_GARMENT_TYPE='" + ToGarmentType + "' ");
            }
            if (!ToProcessType.Equals(""))
            {
                sqlBuilder.Append(" and B.NEXT_PROCESS_TYPE='" + ToProcessType + "' ");
            }
            if (!ToProdFactory.Equals(""))
            {
                sqlBuilder.Append(" and B.NEXT_PROCESS_PRODUCTION_FACTORY='" + ToProdFactory + "' ");
            }

            if (ToprocessCode != "")
            {
                sqlBuilder.Append(" AND B.NEXT_PROCESS_CD = '" + ToprocessCode + "'");
            }

            sqlBuilder.Append(@"  AND (EXISTS(SELECT 1 FROM #TEMP_OUTPUT O WHERE O.JOB_ORDER_NO=A.JOB_ORDER_NO)
										OR EXISTS(SELECT 1 FROM #TEMP_DIS O WHERE O.JOB_ORDER_NO=A.JOB_ORDER_NO)
										)	");

            sqlBuilder.Append(@" GROUP BY A.JOB_ORDER_NO, A.PROCESS_CD ,A.PROCESS_GARMENT_TYPE,A.PROCESS_TYPE,A.PROCESS_PRODUCTION_FACTORY,A.Production_line_cd, A.DES_Production_line_cd,A.TRX_DATE
                                ) AS E;");


            sqlBuilder.Append(@" select CUSTOMER_NAME,SC_NO,a.JOB_ORDER_NO ,A.GARMENT_TYPE_CD,ORDER_QTY, STYLE_NO,FAB_TYPE_CD,WASH_TYPE_CD,
                                        CASE WHEN A.PROCESS_TYPE = 'I'
                                                THEN A.PROCESS_GARMENT_TYPE + A.PROCESS_CD + '(' + A.PROCESS_TYPE + ')'
                                                ELSE A.PROCESS_GARMENT_TYPE + A.PROCESS_CD + '(' + A.PROCESS_TYPE + ')'+A.PROCESS_PRODUCTION_FACTORY
                                        END AS PROCESS_CD 
                                ,PRODUCTION_LINE_CD=(CASE WHEN  a.PRODUCTION_LINE_CD =ISNULL(LINE.PRODUCTION_LINE_NAME,'')
                                THEN a.PRODUCTION_LINE_CD ELSE A.PRODUCTION_LINE_CD + '(' + ISNULL(LINE.PRODUCTION_LINE_NAME,'') + ')' END), 
                                TO_PROD_LINE=(CASE WHEN  A.TO_PROD_LINE=ISNULL(LINE1.PRODUCTION_LINE_NAME,'') THEN A.TO_PROD_LINE 
                                ELSE (A.TO_PROD_LINE + '(' + ISNULL(LINE1.PRODUCTION_LINE_NAME,'') + ')') END),CUT_QTY,JO_CLEAR_FLAG 
                                ,SAH=ISNULL((case WHEN  (exists(select FTY_TYPE FROM dbo.SC_SAM WHERE  TYPE='S' and sah is not null AND SAH<>0and sc_no=a.sc_no)) 
                                THEN (select sah FROM dbo.SC_SAM WHERE type='S' and sc_no=a.sc_no) 
                                else (select sah FROM dbo.SC_SAM WHERE  type='A' and sc_no=a.sc_no ) END),0), BUYER_PO_DEL_DATE,LATEST_EX_FACTORY_DATE
                                ,B.DAILY,B.MTH_TTL,B.JO_TTL,B.JOUPToDay,C.DAILY_DEFECT_QTY
                                FROM (
                                SELECT * FROM #TEMP_OUTPUT
                                UNION ALL
                                SELECT * FROM #TEMP_DIS A
                                WHERE NOT EXISTS(SELECT 1 FROM #TEMP_OUTPUT B  WHERE 
                                A.CUSTOMER_NAME=B.CUSTOMER_NAME	 AND
                                A.SC_NO=B.SC_NO	 AND
                                A.JOB_ORDER_NO=B.JOB_ORDER_NO	 AND
                                A.GARMENT_TYPE_CD=B.GARMENT_TYPE_CD	 AND
                                A.ORDER_QTY=B.ORDER_QTY	 AND
                                A.STYLE_NO=B.STYLE_NO	 AND
                                ISNULL(A.FAB_TYPE_CD,'*')=ISNULL(B.FAB_TYPE_CD,'*')	 AND
                                A.WASH_TYPE_CD=B.WASH_TYPE_CD	 AND
                                A.PROCESS_CD=B.PROCESS_CD AND A.PROCESS_GARMENT_TYPE=B.PROCESS_GARMENT_TYPE AND A.PROCESS_TYPE=B.PROCESS_TYPE	 AND 
                                ISNULL(A.PRODUCTION_LINE_CD,'*')=ISNULL(B.PRODUCTION_LINE_CD,'*')	 AND
                                A.CUT_QTY=B.CUT_QTY	 AND
                                ISNULL(A.BUYER_PO_DEL_DATE,'*')=ISNULL(B.BUYER_PO_DEL_DATE,'*')	 AND
                                ISNULL(A.LATEST_EX_FACTORY_DATE,'*') =ISNULL(B.LATEST_EX_FACTORY_DATE,'*'))
                                ) a
                                LEFT join ( SELECT JOB_ORDER_NO,PROCESS_CD,PROCESS_GARMENT_TYPE,PROCESS_TYPE,PROCESS_PRODUCTION_FACTORY,ISNULL(Production_line_cd,'') AS Production_line_cd,isnull(TO_PROD_LINE,'')AS TO_PROD_LINE
                                ,SUM(JODAILY) AS DAILY,SUM(JOMONTH) AS MTH_TTL,SUM(JOTIL) AS JO_TTL,SUM(JOUPToDay) AS JOUPToDay
                                FROM #TEMP_JOUPToDay E
                                GROUP BY JOB_ORDER_NO,PROCESS_CD,PROCESS_GARMENT_TYPE,PROCESS_TYPE,PROCESS_PRODUCTION_FACTORY,Production_line_cd,TO_PROD_LINE
                                ) b on a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD AND a.TO_PROD_LINE=b.TO_PROD_LINE and a.JOB_ORDER_NO=b.JOB_ORDER_NO 
                                AND A.PROCESS_CD=B.PROCESS_CD AND A.PROCESS_GARMENT_TYPE=B.PROCESS_GARMENT_TYPE AND A.PROCESS_TYPE=B.PROCESS_TYPE
                                LEFT JOIN (
                                SELECT (CASE WHEN  ISNULL(E.PROCESS_CD,'')='' THEN '' ELSE E.PROCESS_CD END ) PROCESS_CD
                                ,E.GARMENT_TYPE AS PROCESS_GARMENT_TYPE,E.PROCESS_TYPE,E.PRODUCTION_FACTORY
                                ,(CASE WHEN  ISNULL(E.Production_line_cd,'')='' THEN '' ELSE E.Production_line_cd END ) PRODUCTION_LINE_CD
                                ,A.JOB_ORDER_NO, SUM(ISNULL(a.PULLOUT_QTY,0)+ISNULL(DISCREPANCY_QTY,0)) DAILY_DEFECT_QTY 
                                FROM PRD_JO_DISCREPANCY_PULLOUT_HD E, PRD_JO_DISCREPANCY_PULLOUT_TRX A 
                                WHERE  A.DOC_NO=E.DOC_NO  ");

            if (!factoryCd.Equals(""))
            {
                sqlBuilder.Append("AND A.FACTORY_CD='" + factoryCd + "' ");
            }
            if (!joNo.Equals(""))
            {
                sqlBuilder.Append("AND A.JOB_ORDER_NO = '" + joNo + "' ");
            }
            if (date != "")
            {
                sqlBuilder.Append(" AND A.TRX_DATE>='" + date + "'");
            }
            if (ToDate != "")
            {
                sqlBuilder.Append(" AND A.TRX_DATE <='" + ToDate + "'");
            }

            sqlBuilder.Append(@" AND (EXISTS(SELECT 1 FROM #TEMP_OUTPUT O WHERE O.JOB_ORDER_NO=A.JOB_ORDER_NO)
										OR EXISTS(SELECT 1 FROM #TEMP_DIS O WHERE O.JOB_ORDER_NO=A.JOB_ORDER_NO)
										) ");

            sqlBuilder.Append(@" GROUP BY E.PROCESS_CD,E.GARMENT_TYPE,E.PROCESS_TYPE,E.PRODUCTION_FACTORY, E.PRODUCTION_LINE_CD,A.JOB_ORDER_NO
                                ) C ON C.PROCESS_CD = A.PROCESS_CD
                                AND C.PROCESS_GARMENT_TYPE=A.PROCESS_GARMENT_TYPE AND C.PROCESS_TYPE=A.PROCESS_TYPE
                                AND c.PRODUCTION_LINE_CD=A.PRODUCTION_LINE_CD
                                AND c.JOB_ORDER_NO=A.JOB_ORDER_NO
                                LEFT JOIN dbo.GEN_PRODUCTION_LINE line on line.PRODUCTION_LINE_CD=a.PRODUCTION_LINE_CD  ");

            if (!factoryCd.Equals(""))
            {
                sqlBuilder.Append("AND LINE.FACTORY_CD='" + factoryCd + "' ");
            }

            sqlBuilder.Append(@" LEFT JOIN dbo.GEN_PRODUCTION_LINE line1 on line1.PRODUCTION_LINE_CD=a.TO_PROD_LINE ");

            if (!factoryCd.Equals(""))
            {
                sqlBuilder.Append("AND LINE1.FACTORY_CD='" + factoryCd + "' ");
            }
            sqlBuilder.Append(@" INNER JOIN dbo.GEN_PRC_CD_MST MST ON A.PROCESS_CD = MST.PRC_CD AND A.PROCESS_GARMENT_TYPE=MST.GARMENT_TYPE  ");
            if (!factoryCd.Equals(""))
            {
                sqlBuilder.Append("AND MST.FACTORY_CD='" + factoryCd + "' ");
            }

            sqlBuilder.Append(@" WHERE 1=1 ");
            if (ToprodLine != "")
            {
                sqlBuilder.Append("AND A.TO_PROD_LINE='" + ToprodLine + "' ");
            }

            if (!GroupName.Equals(""))
            {
                sqlBuilder.Append(" AND A.PRODUCTION_LINE_CD IN (select FNFIELD from dbo.FN_SPLIT_STRING_TB(@s,','))");
            }

            switch (OrderBy)
            {
                case "1":
                    // sqlBuilder.Append( "     order by  MST.DISPLAY_SEQ,a.Process_CD,a.JOB_ORDER_NO,PRODUCTION_LINE_CD ");
                    sqlBuilder.Append("     order by  MST.DISPLAY_SEQ,a.Process_CD,PRODUCTION_LINE_CD,TO_PROD_LINE,a.JOB_ORDER_NO ");
                    break;
                case "2":
                    sqlBuilder.Append("     order by  PRODUCTION_LINE_CD,MST.DISPLAY_SEQ,a.Process_CD,a.JOB_ORDER_NO ");
                    break;
                case "3":
                    sqlBuilder.Append("     order by  a.JOB_ORDER_NO,MST.DISPLAY_SEQ,a.Process_CD,PRODUCTION_LINE_CD ");
                    break;
            }

            return DBUtility.GetTable(sqlBuilder.ToString(), "MES");
        }

        //        public static DataTable GetDailyOutputByJOSectionDetail(string garmentType, string prodLine, string processCode, string ToprodLine, string ToprocessCode, string date, string ToDate, string factoryCd, string joNo, string stageCode, string OrderBy, string GroupName)
        //        {
        //            StringBuilder sqlBuilder=new StringBuilder("");

        //            if (!GroupName.Equals(""))
        //            {
        //                sqlBuilder.Append( @" declare @s nvarchar(max)");
        //                sqlBuilder.Append( @" select @s=system_value from gen_system_setting where factory_cd='" + factoryCd + "' and system_key='" + GroupName + "';");
        //            }

        //            sqlBuilder.Append(@" IF OBJECT_ID('tempdb..#TEMP_OUTPUT') IS NOT NULL 
        //                                DROP TABLE #TEMP_OUTPUT;
        //                                IF OBJECT_ID('tempdb..#TEMP_DIS') IS NOT NULL 
        //                                DROP TABLE #TEMP_DIS;
        //                                IF OBJECT_ID('tempdb..#TEMP_HeadData') IS NOT NULL
        //                                    DROP TABLE #TEMP_HeadData;
        //                                ");


        //          sqlBuilder.Append(@"SELECT CU.SHORT_NAME,PO.SC_NO,A.JOB_ORDER_NO,A.PROCESS_CD,A.PRODUCTION_LINE_CD,A.DES_PRODUCTION_LINE_CD,PO.BUYER_PO_DEL_DATE,PO.LATEST_EX_FACTORY_DATE,
        //                                PO.GARMENT_TYPE_CD,SC.STYLE_NO,SC.FAB_TYPE_CD,SC.WASH_TYPE_CD,MAX (ISNULL(a.JO_CLEAR_FLAG,'N')) JO_CLEAR_FLAG
        //                          INTO #TEMP_HeadData
        //                                 FROM  PRD_JO_OUTPUT_TRX A WITH(NOLOCK) INNER JOIN PRD_JO_OUTPUT_HD B WITH(NOLOCK)
        //	                        ON A.DOC_NO=B.DOC_NO 
        //	                        INNER JOIN JO_HD PO  WITH(NOLOCK) ON PO.JO_NO=A.JOB_ORDER_NO
        //	                        INNER JOIN  SC_HD SC  WITH(NOLOCK) ON PO.SC_NO=SC.SC_NO
        //	                        LEFT JOIN GEN_CUSTOMER CU  WITH(NOLOCK) ON PO.CUSTOMER_CD=CU.CUSTOMER_CD
        //	                          WHERE 1=1 " );


        //             if (!factoryCd.Equals(""))
        //            {
        //                sqlBuilder.Append( "AND A.FACTORY_CD='" + factoryCd + "' ");
        //            }

        //            if (!joNo.Equals(""))
        //            {
        //                sqlBuilder.Append( "AND A.JOB_ORDER_NO = '" + joNo + "' ");
        //            }

        //            if (garmentType != "")
        //            {
        //               sqlBuilder.Append(" AND PO.GARMENT_TYPE_CD = '" + garmentType + "'");
        //            }

        //            if (processCode != "")
        //            {
        //               sqlBuilder.Append( " AND A.PROCESS_CD='" + processCode + "' ");
        //            }

        //            if (ToprocessCode != "")
        //            {
        //                sqlBuilder.Append(" AND B.NEXT_PROCESS_CD='" + ToprocessCode + "' ");
        //            }
        //            if (date != "")
        //            {
        //                sqlBuilder.Append( " AND A.TRX_DATE >='" + date + "'");
        //            }
        //            if (ToDate != "")
        //            {
        //                sqlBuilder.Append(" AND A.TRX_DATE <='" + ToDate + "'");
        //            }
        //            if (prodLine != "")
        //            {
        //                sqlBuilder.Append(" AND A.PRODUCTION_LINE_CD='" + prodLine + "' ");
        //            }
        //            if (ToprodLine != "")
        //            {
        //                sqlBuilder.Append( " AND A.DES_PRODUCTION_LINE_CD='" + ToprodLine + "' ");
        //            }
        //        sqlBuilder.Append(@" GROUP BY PO.SC_NO,A.JOB_ORDER_NO,A.PROCESS_CD,A.PRODUCTION_LINE_CD,A.DES_PRODUCTION_LINE_CD,PO.BUYER_PO_DEL_DATE,PO.LATEST_EX_FACTORY_DATE,
        //        PO.GARMENT_TYPE_CD,SC.STYLE_NO,SC.FAB_TYPE_CD,SC.WASH_TYPE_CD,CU.SHORT_NAME ;");




        //        sqlBuilder.Append(@"SELECT A.SHORT_NAME CUSTOMER_NAME,
        //                       (CASE WHEN ISNULL (A.SC_NO, '') = '' THEN '-' ELSE A.sc_no END)
        //                          SC_NO,
        //                       A.JOB_ORDER_NO,
        //                       A.GARMENT_TYPE_CD GARMENT_TYPE_CD,
        //                       PO.ORDER_QTY ORDER_QTY,
        //                       A.STYLE_NO STYLE_NO,
        //                       A.FAB_TYPE_CD,
        //                       A.WASH_TYPE_CD WASH_TYPE_CD,
        //                       A.PROCESS_CD,
        //                       (CASE
        //                           WHEN ISNULL (A.PRODUCTION_LINE_CD, '') = '' THEN ''
        //                           ELSE A.PRODUCTION_LINE_CD
        //                        END)
        //                          PRODUCTION_LINE_CD,
        //                       (CASE
        //                           WHEN ISNULL (A.DES_PRODUCTION_LINE_CD, '') = '' THEN ''
        //                           ELSE A.DES_PRODUCTION_LINE_CD
        //                        END)
        //                          TO_PROD_LINE,
        //                       PL.CUT_QTY AS CUT_QTY,
        //                       a.JO_CLEAR_FLAG JO_CLEAR_FLAG,
        //                       CONVERT (NVARCHAR, A.BUYER_PO_DEL_DATE, 23) BUYER_PO_DEL_DATE,
        //                       CONVERT (NVARCHAR, A.LATEST_EX_FACTORY_DATE, 23) LATEST_EX_FACTORY_DATE
        //                       INTO #TEMP_OUTPUT
        //                  FROM  #TEMP_HeadData  A
        //	                LEFT JOIN 
        //		                (
        //                        SELECT  DT.JO_NO,SUM(ISNULL(DT.QTY,0)) AS ORDER_QTY
        //                        FROM  JO_DT DT WITH(NOLOCK) 
        //                        WHERE EXISTS (SELECT 1 FROM #TEMP_HeadData B WITH(NOLOCK) 
        //				                WHERE  B.JOB_ORDER_NO = DT.JO_NO )
        //                        GROUP BY DT.JO_NO
        //                        )PO   ON A.JOB_ORDER_NO=PO.JO_NO   
        //                        LEFT JOIN (
        //                        SELECT A.JOB_ORDER_NO,SUM(A.QTY) CUT_QTY
        //                        FROM CUT_BUNDLE_HD A  WITH(NOLOCK)
        //                        WHERE  EXISTS (SELECT 1 FROM #TEMP_HeadData B WITH(NOLOCK)
        //                        WHERE B.JOB_ORDER_NO = A.JOB_ORDER_NO ) 
        //                        GROUP BY A.JOB_ORDER_NO
        //                        )PL ON A.JOB_ORDER_NO=PL.JOB_ORDER_NO;");


        //        sqlBuilder.Append(@" 
        //                                IF OBJECT_ID('tempdb..#TEMP_HeadData1') IS NOT NULL
        //                                    DROP TABLE #TEMP_HeadData1;");

        //       sqlBuilder.Append(@" SELECT CU.SHORT_NAME,PO.SC_NO,A.JOB_ORDER_NO,B.PROCESS_CD,A.PRODUCTION_LINE_CD,'' AS TO_PROD_LINE ,PO.BUYER_PO_DEL_DATE,
        //                             PO.LATEST_EX_FACTORY_DATE,
        //                            PO.GARMENT_TYPE_CD,SC.STYLE_NO,SC.FAB_TYPE_CD,SC.WASH_TYPE_CD,'' AS  JO_CLEAR_FLAG
        //	                    INTO #TEMP_HeadData1
        //                             FROM  PRD_JO_DISCREPANCY_PULLOUT_TRX A WITH(NOLOCK) INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_HD B WITH(NOLOCK)
        //	                    ON A.DOC_NO=B.DOC_NO 
        //	                    INNER JOIN JO_HD PO  WITH(NOLOCK) ON PO.JO_NO=A.JOB_ORDER_NO
        //	                    INNER JOIN  SC_HD SC  WITH(NOLOCK) ON PO.SC_NO=SC.SC_NO
        //	                    LEFT JOIN GEN_CUSTOMER CU  WITH(NOLOCK) ON PO.CUSTOMER_CD=CU.CUSTOMER_CD
        //	                      WHERE 1=1 ");

        //    if (!factoryCd.Equals(""))
        //            {
        //                sqlBuilder.Append(  "AND A.FACTORY_CD='" + factoryCd + "' ");
        //            }
        //            if (!joNo.Equals(""))
        //            {
        //                sqlBuilder.Append( "AND A.JOB_ORDER_NO = '" + joNo + "' ");
        //            }
        //            if (garmentType != "")
        //            {
        //                sqlBuilder.Append(  " AND PO.GARMENT_TYPE_CD = '" + garmentType + "'");
        //            }
        //            if (processCode != "")
        //            {
        //                sqlBuilder.Append(  " AND B.PROCESS_CD='" + processCode + "' ");
        //            }
        //            if (date != "")
        //            {
        //                sqlBuilder.Append(  " AND A.TRX_DATE >='" + date + "'");
        //            }
        //            if (ToDate != "")
        //            {
        //               sqlBuilder.Append(  " AND A.TRX_DATE <='" + ToDate + "'");
        //            }
        //            if (prodLine != "")
        //            {
        //              sqlBuilder.Append(  " AND A.PRODUCTION_LINE_CD='" + prodLine + "' ");
        //            }


        //        sqlBuilder.Append( @" GROUP BY PO.SC_NO,A.JOB_ORDER_NO,B.PROCESS_CD,A.PRODUCTION_LINE_CD,PO.BUYER_PO_DEL_DATE,PO.LATEST_EX_FACTORY_DATE,
        //        PO.GARMENT_TYPE_CD,SC.STYLE_NO,SC.FAB_TYPE_CD,SC.WASH_TYPE_CD,CU.SHORT_NAME;");

        //        sqlBuilder.Append(@" SELECT A.SHORT_NAME CUSTOMER_NAME,
        //                       (CASE WHEN ISNULL (A.SC_NO, '') = '' THEN '-' ELSE A.SC_NO END)
        //                          SC_NO,
        //                       A.JOB_ORDER_NO,
        //                       A.GARMENT_TYPE_CD GARMENT_TYPE_CD,
        //                       PO.ORDER_QTY ORDER_QTY,
        //                       A.STYLE_NO STYLE_NO,
        //                       A.FAB_TYPE_CD,
        //                       A.WASH_TYPE_CD WASH_TYPE_CD,
        //                       A.PROCESS_CD,
        //                       (CASE
        //                           WHEN ISNULL (A.PRODUCTION_LINE_CD, '') = '' THEN ''
        //                           ELSE A.PRODUCTION_LINE_CD
        //                        END)
        //                          PRODUCTION_LINE_CD,
        //                       A.TO_PROD_LINE,
        //                       PL.CUT_QTY AS CUT_QTY,
        //                       A.JO_CLEAR_FLAG,
        //                       CONVERT (NVARCHAR, A.BUYER_PO_DEL_DATE, 23) BUYER_PO_DEL_DATE,
        //                       CONVERT (NVARCHAR, A.LATEST_EX_FACTORY_DATE, 23) LATEST_EX_FACTORY_DATE
        //                  INTO #TEMP_DIS
        //                  FROM #TEMP_HeadData1 A        
        //                   LEFT JOIN 
        //		                (
        //                        SELECT  DT.JO_NO,SUM(ISNULL(DT.QTY,0)) AS ORDER_QTY
        //                        FROM  JO_DT DT WITH(NOLOCK) 
        //                        WHERE EXISTS (SELECT 1 FROM #TEMP_HeadData1 B WITH(NOLOCK) 
        //				                WHERE  B.JOB_ORDER_NO = DT.JO_NO )
        //                        GROUP BY DT.JO_NO
        //                        )PO   ON A.JOB_ORDER_NO=PO.JO_NO   
        //                        LEFT JOIN (
        //                        SELECT A.JOB_ORDER_NO,SUM(A.QTY) CUT_QTY
        //                        FROM CUT_BUNDLE_HD A  WITH(NOLOCK)
        //                        WHERE  EXISTS (SELECT 1 FROM #TEMP_HeadData1 B WITH(NOLOCK)
        //                        WHERE B.JOB_ORDER_NO = A.JOB_ORDER_NO ) 
        //                        GROUP BY A.JOB_ORDER_NO
        //                        )PL ON A.JOB_ORDER_NO=PL.JOB_ORDER_NO ;");





        //           sqlBuilder.Append(  @" IF OBJECT_ID('tempdb..#TEMP_JOUPToDay') IS NOT NULL 
        //                                DROP TABLE #TEMP_JOUPToDay;
        //                                SELECT JOB_ORDER_NO,PROCESS_CD,Production_line_cd,TO_PROD_LINE
        //                                ,JODAILY=(CASE WHEN  TRX_DATE>='" + date + "' and TRX_DATE<='" + ToDate + "' THEN (JO_TTL) ELSE 0 END),");

        //            sqlBuilder.Append(  @" JOMONTH=(CASE WHEN  YEAR(TRX_DATE)=YEAR('" + date + "') AND MONTH(TRX_DATE)=MONTH('" + date + "') THEN (JO_TTL) ELSE 0 END),");
        //           sqlBuilder.Append(  @" JOUPToDay=(CASE WHEN  TRX_DATE<='" + ToDate + "' THEN (JO_TTL) ELSE 0 END),JOTIL=(JO_TTL)");
        //            sqlBuilder.Append(  @"  INTO #TEMP_JOUPToDay
        //                                FROM (
        //                                SELECT A.TRX_DATE,A.JOB_ORDER_NO, A.PROCESS_CD ,A.Production_line_cd,A.DES_Production_line_cd AS TO_PROD_LINE
        //                                ,SUM(ISNULL(OUTPUT_QTY,0)) AS JO_TTL
        //                                FROM PRD_JO_OUTPUT_TRX A WITH(NOLOCK)
        //                                INNER JOIN PRD_JO_OUTPUT_HD B WITH(NOLOCK) ON A.DOC_NO = B.DOC_NO
        //                                WHERE  1=1 ");
        //            if (!factoryCd.Equals(""))
        //            {
        //                sqlBuilder.Append(  "AND A.FACTORY_CD='" + factoryCd + "' ");
        //            }
        //            if (!joNo.Equals(""))
        //            {
        //                sqlBuilder.Append(  "AND A.JOB_ORDER_NO = '" + joNo + "' ");
        //            }
        //            if (processCode != "")
        //            {
        //               sqlBuilder.Append( " AND A.PROCESS_CD='" + processCode + "'");
        //            }
        //            if (ToprocessCode != "")
        //            {
        //               sqlBuilder.Append(  " AND B.NEXT_PROCESS_CD = '" + ToprocessCode + "'");
        //            }

        //           sqlBuilder.Append(  @"  AND (EXISTS(SELECT 1 FROM #TEMP_OUTPUT O WHERE O.JOB_ORDER_NO=A.JOB_ORDER_NO)
        //										OR EXISTS(SELECT 1 FROM #TEMP_DIS O WHERE O.JOB_ORDER_NO=A.JOB_ORDER_NO)
        //										)	");

        //           sqlBuilder.Append(  @" GROUP BY A.JOB_ORDER_NO, A.PROCESS_CD ,A.Production_line_cd, A.DES_Production_line_cd,A.TRX_DATE
        //                                ) AS E;");


        //           sqlBuilder.Append(  @" select CUSTOMER_NAME,SC_NO,a.JOB_ORDER_NO ,A.GARMENT_TYPE_CD,ORDER_QTY, STYLE_NO,FAB_TYPE_CD,WASH_TYPE_CD,a.Process_CD
        //                                ,PRODUCTION_LINE_CD=(CASE WHEN  a.PRODUCTION_LINE_CD =ISNULL(LINE.PRODUCTION_LINE_NAME,'')
        //                                THEN a.PRODUCTION_LINE_CD ELSE A.PRODUCTION_LINE_CD + '(' + ISNULL(LINE.PRODUCTION_LINE_NAME,'') + ')' END), 
        //                                TO_PROD_LINE=(CASE WHEN  A.TO_PROD_LINE=ISNULL(LINE1.PRODUCTION_LINE_NAME,'') THEN A.TO_PROD_LINE 
        //                                ELSE (A.TO_PROD_LINE + '(' + ISNULL(LINE1.PRODUCTION_LINE_NAME,'') + ')') END),CUT_QTY,JO_CLEAR_FLAG 
        //                                ,SAH=ISNULL((case WHEN  (exists(select FTY_TYPE FROM dbo.SC_SAM WHERE  TYPE='S' and sah is not null AND SAH<>0and sc_no=a.sc_no)) 
        //                                THEN (select sah FROM dbo.SC_SAM WHERE type='S' and sc_no=a.sc_no) 
        //                                else (select sah FROM dbo.SC_SAM WHERE  type='A' and sc_no=a.sc_no ) END),0), BUYER_PO_DEL_DATE,LATEST_EX_FACTORY_DATE
        //                                ,B.DAILY,B.MTH_TTL,B.JO_TTL,B.JOUPToDay,C.DAILY_DEFECT_QTY
        //                                FROM (
        //                                SELECT * FROM #TEMP_OUTPUT
        //                                UNION ALL
        //                                SELECT * FROM #TEMP_DIS A
        //                                WHERE NOT EXISTS(SELECT 1 FROM #TEMP_OUTPUT B  WHERE 
        //                                A.CUSTOMER_NAME=B.CUSTOMER_NAME	 AND
        //                                A.SC_NO=B.SC_NO	 AND
        //                                A.JOB_ORDER_NO=B.JOB_ORDER_NO	 AND
        //                                A.GARMENT_TYPE_CD=B.GARMENT_TYPE_CD	 AND
        //                                A.ORDER_QTY=B.ORDER_QTY	 AND
        //                                A.STYLE_NO=B.STYLE_NO	 AND
        //                                ISNULL(A.FAB_TYPE_CD,'*')=ISNULL(B.FAB_TYPE_CD,'*')	 AND
        //                                A.WASH_TYPE_CD=B.WASH_TYPE_CD	 AND
        //                                A.PROCESS_CD=B.PROCESS_CD	 AND 
        //                                ISNULL(A.PRODUCTION_LINE_CD,'*')=ISNULL(B.PRODUCTION_LINE_CD,'*')	 AND
        //                                A.CUT_QTY=B.CUT_QTY	 AND
        //                                ISNULL(A.BUYER_PO_DEL_DATE,'*')=ISNULL(B.BUYER_PO_DEL_DATE,'*')	 AND
        //                                ISNULL(A.LATEST_EX_FACTORY_DATE,'*') =ISNULL(B.LATEST_EX_FACTORY_DATE,'*'))
        //                                ) a
        //                                LEFT join ( SELECT JOB_ORDER_NO,PROCESS_CD,ISNULL(Production_line_cd,'') AS Production_line_cd,isnull(TO_PROD_LINE,'')AS TO_PROD_LINE
        //                                ,SUM(JODAILY) AS DAILY,SUM(JOMONTH) AS MTH_TTL,SUM(JOTIL) AS JO_TTL,SUM(JOUPToDay) AS JOUPToDay
        //                                FROM #TEMP_JOUPToDay E
        //                                GROUP BY JOB_ORDER_NO,PROCESS_CD,Production_line_cd,TO_PROD_LINE
        //                                ) b on a.PRODUCTION_LINE_CD=b.PRODUCTION_LINE_CD AND a.TO_PROD_LINE=b.TO_PROD_LINE and a.JOB_ORDER_NO=b.JOB_ORDER_NO 
        //                                AND A.PROCESS_CD=B.PROCESS_CD 
        //                                LEFT JOIN (
        //                                SELECT (CASE WHEN  ISNULL(E.PROCESS_CD,'')='' THEN '' ELSE E.PROCESS_CD END ) PROCESS_CD
        //                                ,(CASE WHEN  ISNULL(E.Production_line_cd,'')='' THEN '' ELSE E.Production_line_cd END ) PRODUCTION_LINE_CD
        //                                ,A.JOB_ORDER_NO, SUM(ISNULL(a.PULLOUT_QTY,0)+ISNULL(DISCREPANCY_QTY,0)) DAILY_DEFECT_QTY 
        //                                FROM PRD_JO_DISCREPANCY_PULLOUT_HD E, PRD_JO_DISCREPANCY_PULLOUT_TRX A 
        //                                WHERE  A.DOC_NO=E.DOC_NO  ");

        //            if (!factoryCd.Equals(""))
        //            {
        //                sqlBuilder.Append(  "AND A.FACTORY_CD='" + factoryCd + "' ");
        //            }
        //            if (!joNo.Equals(""))
        //            {
        //                sqlBuilder.Append(  "AND A.JOB_ORDER_NO = '" + joNo + "' ");
        //            }
        //            if (date != "")
        //            {
        //               sqlBuilder.Append(  " AND A.TRX_DATE>='" + date + "'");
        //            }
        //            if (ToDate != "")
        //            {
        //                sqlBuilder.Append( " AND A.TRX_DATE <='" + ToDate + "'");
        //            }

        //             sqlBuilder.Append(@" AND (EXISTS(SELECT 1 FROM #TEMP_OUTPUT O WHERE O.JOB_ORDER_NO=A.JOB_ORDER_NO)
        //										OR EXISTS(SELECT 1 FROM #TEMP_DIS O WHERE O.JOB_ORDER_NO=A.JOB_ORDER_NO)
        //										) ");

        //             sqlBuilder.Append(@" GROUP BY E.PROCESS_CD, E.PRODUCTION_LINE_CD,A.JOB_ORDER_NO
        //                                ) C ON C.PROCESS_CD = A.PROCESS_CD
        //                                AND c.PRODUCTION_LINE_CD=A.PRODUCTION_LINE_CD
        //                                AND c.JOB_ORDER_NO=A.JOB_ORDER_NO
        //                                LEFT JOIN dbo.GEN_PRODUCTION_LINE line on line.PRODUCTION_LINE_CD=a.PRODUCTION_LINE_CD  ");

        //            if (!factoryCd.Equals(""))
        //            {
        //               sqlBuilder.Append( "AND LINE.FACTORY_CD='" + factoryCd + "' ");
        //            }

        //            sqlBuilder.Append(@" LEFT JOIN dbo.GEN_PRODUCTION_LINE line1 on line1.PRODUCTION_LINE_CD=a.TO_PROD_LINE ");

        //            if (!factoryCd.Equals(""))
        //            {
        //                sqlBuilder.Append(  "AND LINE1.FACTORY_CD='" + factoryCd + "' ");
        //            }
        //            sqlBuilder.Append(  @" INNER JOIN dbo.GEN_PRC_CD_MST MST ON A.PROCESS_CD = MST.PRC_CD ");
        //            if (!factoryCd.Equals(""))
        //            {
        //                sqlBuilder.Append(  "AND MST.FACTORY_CD='" + factoryCd + "' ");
        //            }

        //            sqlBuilder.Append(  @" WHERE 1=1 ");

        //            if (!GroupName.Equals(""))
        //            {
        //                sqlBuilder.Append(  " AND A.PRODUCTION_LINE_CD IN (select FNFIELD from dbo.FN_SPLIT_STRING_TB(@s,','))");
        //            }

        //            switch (OrderBy)
        //            {
        //                case "1":
        //                   // sqlBuilder.Append( "     order by  MST.DISPLAY_SEQ,a.Process_CD,a.JOB_ORDER_NO,PRODUCTION_LINE_CD ");
        //                    sqlBuilder.Append("     order by  MST.DISPLAY_SEQ,a.Process_CD,PRODUCTION_LINE_CD,TO_PROD_LINE,a.JOB_ORDER_NO ");
        //                    break;
        //                case "2":
        //                    sqlBuilder.Append( "     order by  PRODUCTION_LINE_CD,MST.DISPLAY_SEQ,a.Process_CD,a.JOB_ORDER_NO ");
        //                    break;
        //                case "3":
        //                   sqlBuilder.Append( "     order by  a.JOB_ORDER_NO,MST.DISPLAY_SEQ,a.Process_CD,PRODUCTION_LINE_CD ");
        //                    break;
        //            }

        //            return DBUtility.GetTable(sqlBuilder.ToString(), "MES");
        //        }

//Added By ZouShiChang On 2013.08.28 Start 


        public static DataTable GetDailyOutputByJOSectionSummary(string joNo, string prodLine, string garmentType, string processCode, string processType,string productionFactory,string ToprodLine, string TogarmentType, string ToprocessCode, string ToprocessType,string ToproductionFactory,string date, string ToDate, string factoryCd, string GroupName)
        {
            //string SQL = "  ";

            StringBuilder sqlBuilder = new StringBuilder("");

            if (!GroupName.Equals(""))
            {
                sqlBuilder.Append("       declare @s nvarchar(max)");
                sqlBuilder.Append("  select @s=system_value from gen_system_setting where factory_cd='" + factoryCd + "' and system_key='" + GroupName + "';");
            }

            sqlBuilder.Append(@" SELECT 
                                O.Process_CD, PRODUCTION_LINE_CD =  
                                  (CASE WHEN ISNULL(O.Production_line_cd,'')=ISNULL(LINE.PRODUCTION_LINE_NAME,'')  THEN   
		                                ISNULL(O.Production_line_cd,'') 
	                                ELSE ISNULL(O.PRODUCTION_LINE_CD,'') + '(' + ISNULL(LINE.PRODUCTION_LINE_NAME,'') + ')'  END ) ,  
                                   TO_PROD_LINE=(CASE WHEN ISNULL(O.DES_Production_line_cd,'')=ISNULL(LINE1.PRODUCTION_LINE_NAME,'') THEN O.DES_Production_line_cd    
                                     ELSE (ISNULL(O.DES_Production_line_cd,'') + '(' + ISNULL(LINE1.PRODUCTION_LINE_NAME,'') + ')') END),
                                     O.DAILY,O.SAH_PRODUCE, LCC=ISNULL(LC.LCC,0) 
                                 FROM 
                                (
	                                SELECT d.Process_CD,d.PRODUCTION_LINE_CD,DES_Production_line_cd ,SUM(D.OUTPUT_QTY) DAILY,
	                                SAH_PRODUCE=
                                      ROUND(SUM(D.OUTPUT_QTY/12*(CASE when d.Process_CD like '%SEW%' THEN ISNULL(s1.SAH,ISNULL(S2.SAH,0)) 
									                                ELSE 0 END)
				                                )
                                      ,2)	
		                                   FROM PRD_JO_OUTPUT_TRX d       
			                                inner join PRD_JO_OUTPUT_HD h on h.doc_no=d.doc_no     
			                                inner join jo_hd on jo_hd.jo_no=d.job_order_no 
									                                AND JO_HD.FACTORY_CD = D.FACTORY_CD              
			                                left join dbo.SC_SAM s1   
				                                on  s1.TYPE='S'      and s1.sc_no=jo_hd.sc_no     
			                                left  join dbo.SC_SAM s2      on s2.TYPE='A'     
				                                and s2.sc_no=jo_hd.sc_no      AND  NOT EXISTS (select 1 From  dbo.SC_SAM sam      where sam.TYPE='S'   
						                                and sam.sc_no=s2.sc_no) 	
		                                  WHERE 1=1  ");


            if (joNo != "")
            {
                sqlBuilder.Append(" AND (D.JOB_ORDER_NO LIKE '" + joNo + "' )");
            }
            if (prodLine != "")
            {
                sqlBuilder.Append(" AND (D.PRODUCTION_LINE_CD='" + prodLine + "')");
            }

            if (processCode != "")
            {
                sqlBuilder.Append(" AND (D.PROCESS_CD='" + processCode + "')");
            }
            //Added BY ZouShiChang ON 2013.12.09 Start MES024
            if (garmentType != "")
            {
                sqlBuilder.Append(" AND(D.PROCESS_GARMENT_TYPE='" + garmentType + "')");
            }
            if (processType != "")
            {
                sqlBuilder.Append(" AND(D.PROCESS_TYPE='" + processType + "')");
            }
            if (productionFactory != "")
            {
                sqlBuilder.Append(" AND(D.PROCESS_PRODUCTION_FACTORY='" + productionFactory + "')");
            }
            if (ToprocessType != "")
            {
                sqlBuilder.Append(" AND(H.NEXT_PROCESS_TYPE='" + ToprocessType + "')");
            }
            if (ToproductionFactory != "")
            {
                sqlBuilder.Append(" AND(H.NEXT_PROCESS_PRODUCTION_FACTORY='" + ToproductionFactory + "')");
            }

            if (TogarmentType != "")
            {
                sqlBuilder.Append(" AND (h.next_Process_garment_Type='" + TogarmentType + "' )");
            }
            //Added BY ZouShiChang ON 2013.12.09 End MES024

            if (ToprocessCode != "")
            {
                sqlBuilder.Append(" AND (h.Next_PROCESS_CD='" + ToprocessCode + "' )");
            }
            if (ToprodLine != "")
            {
                sqlBuilder.Append(" AND (D.DES_PRODUCTION_LINE_CD='" + ToprodLine + "' )");
            }

            if (date != "")
            {
                sqlBuilder.Append(" AND D.TRX_DATE >='" + date + "' ");
            }
            if (ToDate != "")
            {
                sqlBuilder.Append(" AND D.TRX_DATE <='" + ToDate + "'");
            }
            if (factoryCd != "")
            {
                sqlBuilder.Append(" AND (D.FACTORY_CD='" + factoryCd + "' )");
            }
            if (!GroupName.Equals(""))
            {
                sqlBuilder.Append(" AND        D.PRODUCTION_LINE_CD IN (select FNFIELD from dbo.FN_SPLIT_STRING_TB(@s,','))");
            }

            sqlBuilder.Append(@" GROUP BY d.Process_CD,d.PRODUCTION_LINE_CD,DES_Production_line_cd  )O	
                            inner join dbo.GEN_PRODUCTION_LINE line   on line.PRODUCTION_LINE_CD=O.PRODUCTION_LINE_CD "); //Added By ZouShiChang ON 2013.12.20 Changed by ZK on 22 Jul 14 to remove Active ='Y' filter

            if (factoryCd != "")
            {
                sqlBuilder.Append(" AND (line.FACTORY_CD='" + factoryCd + "' )");
            }

            sqlBuilder.Append(@"                LEFT JOIN dbo.GEN_PRODUCTION_LINE line1 on line1.PRODUCTION_LINE_CD=O.DES_Production_line_cd ");

            if (factoryCd != "")
            {
                sqlBuilder.Append(" AND (line1.FACTORY_CD='" + factoryCd + "' )");
            }

            sqlBuilder.Append(@"LEFT JOIN (SELECT line.Process_CD,COUNT(*) LCC FROM GEN_PRODUCTION_LINE  line WITH(NOLOCK) 
						                            WHERE 1=1  ");

            if (factoryCd != "")
            {
                sqlBuilder.Append(" AND (line.FACTORY_CD='" + factoryCd + "' )");
            }


            sqlBuilder.Append(@"                         AND EXISTS(SELECT 1 FROM dbo.PRD_JO_OUTPUT_TRX d 
											                            INNER JOIN PRD_JO_OUTPUT_HD H ON  H.DOC_NO=D.DOC_NO 
											                            WHERE  1=1  ");

            if (factoryCd != "")
            {
                sqlBuilder.Append(@"  AND (D.FACTORY_CD='" + factoryCd + "' )");
            }
            if (date != "")
            {
                sqlBuilder.Append(" AND D.TRX_DATE >='" + date + "' ");
            }

            if (ToDate != "")
            {
                sqlBuilder.Append(" AND D.TRX_DATE <='" + ToDate + "'");
            }

            if (joNo != "")
            {
                sqlBuilder.Append(" AND (D.JOB_ORDER_NO LIKE '" + joNo + "' )");
            }

            if (prodLine != "")
            {
                sqlBuilder.Append(" AND (D.PRODUCTION_LINE_CD='" + prodLine + "')");
            }
            //Added BY ZouShiChang ON 2013.12.09 Start MES024
            if (garmentType != "")
            {
                sqlBuilder.Append(" AND(D.PROCESS_GARMENT_TYPE='" + garmentType + "')");
            }
            if (processType != "")
            {
                sqlBuilder.Append(" AND(D.PROCESS_TYPE='" + processType + "')");
            }
            if (productionFactory != "")
            {
                sqlBuilder.Append(" AND(D.PROCESS_PRODUCTION_FACTORY='" + productionFactory + "')");
            }         
            //Added BY ZouShiChang ON 2013.12.09 End MES024
            if (processCode != "")
            {
                sqlBuilder.Append(" AND (D.PROCESS_CD='" + processCode + "')");
            }

            if (ToprocessType != "")
            {
                sqlBuilder.Append(" AND(H.NEXT_PROCESS_TYPE='" + ToprocessType + "')");
            }
            if (ToproductionFactory != "")
            {
                sqlBuilder.Append(" AND(H.NEXT_PROCESS_PRODUCTION_FACTORY='" + ToproductionFactory + "')");
            }

            if (ToprocessCode != "")
            {
                sqlBuilder.Append(" AND (h.Next_PROCESS_CD='" + ToprocessCode + "' )");
            }

            //Added BY ZouShiChang ON 2013.12.09 Start MES024
            if (TogarmentType != "")
            {
                sqlBuilder.Append(" AND (h.next_Process_garment_Type='" + TogarmentType + "' )");
            }
            //Added BY ZouShiChang ON 2013.12.09 End MES024
            if (ToprodLine != "")
            {
                sqlBuilder.Append(" AND (D.DES_PRODUCTION_LINE_CD='" + ToprodLine + "' )");
            }

            sqlBuilder.Append(@" AND line.PRODUCTION_LINE_CD=D.PRODUCTION_LINE_CD)  
							                                GROUP BY line.Process_CD
						                            ) LC 
	                        ON LC.Process_CD=O.Process_CD 				
              ORDER BY O.Process_CD,O.PRODUCTION_LINE_CD,O.DES_Production_line_cd");

            return DBUtility.GetTable(sqlBuilder.ToString(), "MES");
        }

    }
}