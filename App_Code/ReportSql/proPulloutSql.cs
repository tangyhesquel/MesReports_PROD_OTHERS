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
    ///proPulloutSql 的摘要说明
    /// </summary>
    public class proPulloutSql
    {

        public static DataTable GetProPulOutList(string garmentType, string factoryCd, string FromDate, string ToDate, string JoNo, string processCd)
        {
            string SQL = "";

            SQL = SQL + "  select  AAA.PROCESS_CD,TRX_DATE,JOB_ORDER_NO,SHORT_NAME,REASON_DESC,";
            SQL = SQL + "                  SUM(ISNULL(DISCREPANCY_QTY,0)) AS DISCREPANCY_QTY,PULL_OUT_JO, ";
            SQL = SQL + "                  SUM(ISNULL(PULLOUT_QTY,0)) AS PULLOUT_QTY, PULLIN_JO,";
            SQL = SQL + "                  SUM(ISNULL(PULLIN_QTY,0)) AS PULLIN_QTY   ";
            SQL = SQL + "   ,L.PRODUCTION_LINE_CD,L.PRODUCTION_LINE_NAME";
            SQL = SQL + "       from ";
            SQL = SQL + "          ( select         A.PROCESS_CD,A.TRX_DATE,B.JOB_ORDER_NO,E.SHORT_NAME,E.REASON_DESC,  ";
            SQL = SQL + "        SUM(ISNULL(D.PULLOUT_QTY,0)) AS DISCREPANCY_QTY,ISNULL(C.JO_NO,'') AS  PULL_OUT_JO, ";
            SQL = SQL + "       SUM(ISNULL(C.PULLOUT_QTY,0)) AS PULLOUT_QTY,'' AS PULLIN_JO,0 AS PULLIN_QTY  ";
            SQL = SQL + "       ,b.PRODUCTION_LINE_CD         ";
            SQL = SQL + "       from         PRD_JO_DISCREPANCY_PULLOUT_HD a WITH(NOLOCK)";
            SQL = SQL + "       JOIN         PRD_JO_DISCREPANCY_PULLOUT_TRX b WITH(NOLOCK)";
            SQL = SQL + "       ON A.DOC_NO=B.DOC_NO AND         A.FACTORY_CD=B.FACTORY_CD AND A.TRX_DATE=B.TRX_DATE ";
            SQL = SQL + "       JOIN JO_HD  PO WITH(NOLOCK) ON B.JOB_ORDER_NO=PO.JO_NO   ";
            if (garmentType != "")
            {
                SQL += " and PO.Garment_Type_Cd='" + garmentType + "'";
            }
            SQL = SQL + "       LEFT JOIN PRD_JO_PULLOUT C WITH(NOLOCK) ON C.TRX_ID=B.TRX_ID ";
            SQL = SQL + "       AND C.FACTORY_CD=A.FACTORY_CD ";
            SQL = SQL + "       LEFT JOIN PRD_JO_PULLOUT_REASON D WITH(NOLOCK) ON D.TRX_ID=B.TRX_ID AND D.FACTORY_CD=A.FACTORY_CD ";
            SQL = SQL + "       LEFT JOIN                  PRD_REASON_CODE E WITH(NOLOCK) ON E.REASON_CD=D.REASON_CD AND                    E.FACTORY_CD=D.FACTORY_CD ";
            SQL = SQL + "       WHERE 1=1 and a.factory_cd='" + factoryCd + "'    ";
            if (JoNo != "")
            {
                SQL += " and B.job_order_no like '" + JoNo + "%'";
            }
            if (processCd != "")
            {
                SQL += " and (A.process_cd='" + processCd + "')";
            }
            if (FromDate != "")
            {
                SQL += " and (A.trx_date>='" + FromDate + "')";
            }
            if (ToDate != "")
            {
                SQL += " and (A.trx_date<='" + ToDate + "')";
            }
            SQL = SQL + "        GROUP BY                A.PROCESS_CD,A.TRX_DATE,B.JOB_ORDER_NO,E.SHORT_NAME,E.REASON_DESC,C.JO_NO  ";
            SQL = SQL + "       ,b.PRODUCTION_LINE_CD         ";
            SQL = SQL + "       UNION ALL";
            SQL = SQL + "                select         A.PROCESS_CD,A.TRX_DATE, C.JO_NO AS  JOB_ORDER_NO,'' AS SHORT_NAME,'' AS REASON_DESC,       ";
            SQL = SQL + "       0 AS DISCREPANCY_QTY,'' AS PULL_OUT_JO,0 AS PULLOUT_QTY,B.JOB_ORDER_NO AS PULLIN_JO, SUM(ISNULL(C.PULLOUT_QTY,0)) AS PULLIN_QTY";
            SQL = SQL + "       ,b.PRODUCTION_LINE_CD         ";
            SQL = SQL + "       from         PRD_JO_DISCREPANCY_PULLOUT_HD a WITH(NOLOCK) ";
            SQL = SQL + "       JOIN         PRD_JO_DISCREPANCY_PULLOUT_TRX b WITH(NOLOCK) ";
            SQL = SQL + "       ON A.DOC_NO=B.DOC_NO AND         A.FACTORY_CD=B.FACTORY_CD AND A.TRX_DATE=B.TRX_DATE ";
            SQL = SQL + "       JOIN JO_HD  PO WITH(NOLOCK) ON B.JOB_ORDER_NO=PO.JO_NO         ";
            SQL = SQL + "       JOIN PRD_JO_PULLOUT C WITH(NOLOCK) ON C.TRX_ID=B.TRX_ID ";
            SQL = SQL + "       AND           C.FACTORY_CD=A.FACTORY_CD ";
            SQL = SQL + "       WHERE 1=1 and a.factory_cd='" + factoryCd + "' ";
            if (garmentType != "")
            {
                SQL += " and PO.Garment_Type_Cd='" + garmentType + "'";
            }
            if (JoNo != "")
            {
                SQL += " and C.JO_NO like '" + JoNo + "%'";
            }
            if (processCd != "")
            {
                SQL += " and (A.process_cd='" + processCd + "')";
            }
            if (FromDate != "")
            {
                SQL += " and (A.trx_date>='" + FromDate + "')";
            }
            if (ToDate != "")
            {
                SQL += " and (A.trx_date<='" + ToDate + "')";
            }
            SQL = SQL + "        GROUP BY                A.PROCESS_CD,A.TRX_DATE,B.JOB_ORDER_NO,C.JO_NO ";
            SQL = SQL + "       ,b.PRODUCTION_LINE_CD         ";
            SQL = SQL + "         ) AAA ";
            SQL = SQL + "        LEFT JOIN dbo.GEN_PRODUCTION_LINE L ON AAA.PRODUCTION_LINE_CD = L.PRODUCTION_LINE_CD ";
            SQL = SQL + "        AND L.FACTORY_CD ='" + factoryCd + "'";
            SQL = SQL + "        WHERE 1=1   ";
            SQL = SQL + "       GROUP BY AAA.PROCESS_CD,TRX_DATE,JOB_ORDER_NO,SHORT_NAME,REASON_DESC,";
            SQL = SQL + "                  PULL_OUT_JO,PULLIN_JO";
            SQL = SQL + "       ,L.PRODUCTION_LINE_CD,L.PRODUCTION_LINE_NAME";
            SQL = SQL + "       ORDER BY 1,2,3";
            return DBUtility.GetTable(SQL, "MES");
        }


        public static DataTable GetDiscrepancyDetail(string docNO)
        {
            string sql = @"SELECT
		                    b.JOB_ORDER_NO,c.GRADE_CD,b.COLOR_CODE AS COLOR_CD,
		                    b.SIZE_CODE AS  SIZE_CD,
		                    c.PULLOUT_QTY AS QTY
		                      FROM  dbo.PRD_JO_DISCREPANCY_PULLOUT_TRX b
		                    INNER JOIN dbo.PRD_JO_PULLOUT_REASON c ON b.TRX_ID=c.TRX_ID
		                    WHERE b.DOC_NO='" + docNO  + "'";
            return DBUtility.GetTable(sql, "MES");
        }

         public static DataTable GetDiscrepancyHeadData(string docNO)
        {
            string sql = @"SELECT FACTORY_CD,PROCESS_CD,PRODUCTION_LINE_CD
                             FROM dbo.PRD_JO_DISCREPANCY_PULLOUT_HD 
                            WHERE DOC_NO='" + docNO  + "'";
            return DBUtility.GetTable(sql, "MES");
        }


        




        public static DataTable GetProPulOutList(string garmentType, string factoryCd, string FromDate, string ToDate, string JoNo, string processCd, string processType, string prodFactory, string GroupName)
        {
            string SQL = "";
            if (!GroupName.Equals(""))
            {
                SQL = SQL + " declare @s NVARCHAR(max)";
                SQL = SQL + " select @s=system_value from gen_system_setting where factory_cd='" + factoryCd + "' and system_key='" + GroupName + "'; ";
            }

            SQL = SQL + @" IF OBJECT_ID('tempdb..#tmp1') IS NOT NULL 
                        BEGIN
                            DROP TABLE #tmp1
                        END ;
                    SELECT  *
                    INTO    #tmp1
                    FROM    ( ";

            //SQL = SQL + "  select  AAA.PROCESS_CD,AAA.GARMENT_TYPE,TRX_DATE,JOB_ORDER_NO,SHORT_NAME,REASON_DESC,"; 
            SQL = SQL + @" SELECT    CASE WHEN AAA.PROCESS_TYPE = 'I'
                         THEN AAA.GARMENT_TYPE + AAA.PROCESS_CD + '('
                              + PROCESS_TYPE + ')'
                         ELSE AAA.GARMENT_TYPE + AAA.PROCESS_CD + '('
                              + PROCESS_TYPE + ')' + AAA.PRODUCTION_FACTORY
                    END AS PROCESS_CD ,AAA.GARMENT_TYPE, ";
            SQL = SQL + " TRX_DATE,JOB_ORDER_NO,L.PRODUCTION_LINE_CD,L.PRODUCTION_LINE_NAME,COLOR_CODE,SIZE_CODE,SHORT_NAME,REASON_DESC,";
            SQL = SQL + "                  SUM(ISNULL(DISCREPANCY_QTY,0)) AS DISCREPANCY_QTY,PULL_OUT_JO,PULL_OUT_COLOR_CODE,PULL_OUT_SIZE_CODE,PULL_OUT_LINE_CD,PULL_OUT_LINE_NAME, ";
            SQL = SQL + "                  SUM(ISNULL(PULLOUT_QTY,0)) AS PULLOUT_QTY, PULLIN_JO,PULLIN_COLOR_CODE,PULLIN_SIZE_CODE,PULLIN_LINE_CD,PULLIN_LINE_NAME,";
            SQL = SQL + "                  SUM(ISNULL(PULLIN_QTY,0)) AS PULLIN_QTY   ";
            //SQL = SQL + "   ,L.PRODUCTION_LINE_CD,L.PRODUCTION_LINE_NAME";
            SQL = SQL + "       from ";
            SQL = SQL + "          ( select         A.PROCESS_CD,A.GARMENT_TYPE,A.PROCESS_TYPE,A.PRODUCTION_FACTORY,A.TRX_DATE,B.JOB_ORDER_NO,B.PRODUCTION_LINE_CD,B.COLOR_CODE,B.SIZE_CODE,E.SHORT_NAME,E.REASON_DESC,  "; 
            SQL = SQL + "        SUM(ISNULL(D.PULLOUT_QTY,0)) AS DISCREPANCY_QTY,ISNULL(C.JO_NO,'') AS  PULL_OUT_JO,C.COLOR_CODE AS PULL_OUT_COLOR_CODE,C.SIZE_CODE AS PULL_OUT_SIZE_CODE,C.PROD_LINE AS PULL_OUT_LINE_CD,LINE.PRODUCTION_LINE_NAME AS PULL_OUT_LINE_NAME, ";
            SQL = SQL + "       SUM(ISNULL(C.PULLOUT_QTY,0)) AS PULLOUT_QTY,'' AS PULLIN_JO,'' AS PULLIN_COLOR_CODE,'' AS PULLIN_SIZE_CODE,'' AS PULLIN_LINE_CD,'' AS PULLIN_LINE_NAME,0 AS PULLIN_QTY  ";
            //SQL = SQL + "       ,b.PRODUCTION_LINE_CD         ";
            SQL = SQL + "       from         PRD_JO_DISCREPANCY_PULLOUT_HD a WITH(NOLOCK)";
            SQL = SQL + "       JOIN         PRD_JO_DISCREPANCY_PULLOUT_TRX b WITH(NOLOCK)";
            SQL = SQL + "       ON A.DOC_NO=B.DOC_NO AND         A.FACTORY_CD=B.FACTORY_CD AND A.TRX_DATE=B.TRX_DATE ";
            SQL = SQL + "       JOIN JO_HD  PO WITH(NOLOCK) ON B.JOB_ORDER_NO=PO.JO_NO   ";
            if (garmentType != "")
            {
                SQL += " and PO.Garment_Type_Cd='" + garmentType + "'";
            }
            SQL = SQL + "       LEFT JOIN PRD_JO_PULLOUT C WITH(NOLOCK) ON C.TRX_ID=B.TRX_ID ";
            SQL = SQL + "       AND C.FACTORY_CD=A.FACTORY_CD ";
            SQL = SQL + "       LEFT JOIN PRD_JO_PULLOUT_REASON D WITH(NOLOCK) ON D.TRX_ID=B.TRX_ID AND D.FACTORY_CD=A.FACTORY_CD ";
            SQL = SQL + "       LEFT JOIN                  PRD_REASON_CODE E WITH(NOLOCK) ON E.REASON_CD=D.REASON_CD AND                    E.FACTORY_CD=D.FACTORY_CD ";
            SQL = SQL + "       LEFT JOIN dbo.GEN_PRODUCTION_LINE AS LINE ON C.PROD_LINE=lINE.PRODUCTION_LINE_CD  ";
            SQL = SQL + "       WHERE 1=1 and a.factory_cd='" + factoryCd + "'    ";
            SQL = SQL + " AND A.FACTORY_CD=CASE WHEN A.PROCESS_TYPE='P' THEN po.CO_FACTORY_CD ELSE A.FACTORY_CD END ";
            if (JoNo != "")
            {
                SQL += " and B.job_order_no like '" + JoNo + "%'";
            }
            if (processCd != "")
            {
                SQL += " and (A.process_cd='" + processCd + "')";
            }

            if (!processType.Equals(""))
            {
                SQL += " And a.process_type='" + processType + "' ";
            }
            if (!prodFactory.Equals(""))
            {
                SQL += " and a.PRODUCTION_FACTORY='" + prodFactory + "' ";
            }

            if (FromDate != "")
            {
                SQL += " and (A.trx_date>='" + FromDate + "')";
            }
            if (ToDate != "")
            {
                SQL += " and (A.trx_date<='" + ToDate + "')";
            }
            SQL = SQL + "        GROUP BY                A.PROCESS_CD,A.GARMENT_TYPE,A.PROCESS_TYPE,A.PRODUCTION_FACTORY,A.TRX_DATE,B.JOB_ORDER_NO,B.PRODUCTION_LINE_CD,B.COLOR_CODE,B.SIZE_CODE,E.SHORT_NAME,E.REASON_DESC,C.JO_NO,C.COLOR_CODE,C.SIZE_CODE,C.PROD_LINE,LINE.PRODUCTION_LINE_NAME  ";
            //SQL = SQL + "       ,b.PRODUCTION_LINE_CD         ";
            SQL = SQL + "       UNION ALL";
            SQL = SQL + "                select         A.PROCESS_CD,A.GARMENT_TYPE,A.PROCESS_TYPE,A.PRODUCTION_FACTORY,A.TRX_DATE, C.JO_NO AS  JOB_ORDER_NO,C.PROD_LINE AS PRODUCTION_LINE_CD,C.COLOR_CODE,C.SIZE_CODE,'' AS SHORT_NAME,'' AS REASON_DESC,       "; 
            SQL = SQL + "       0 AS DISCREPANCY_QTY,'' AS PULL_OUT_JO,'' AS PULL_OUT_COLOR_CODE,'' AS PULL_OUT_SIZE_CODE,'' AS PULL_OUT_LINE_CD,'' AS PULL_OUT_LINE_NAME,0 AS PULLOUT_QTY,B.JOB_ORDER_NO AS PULLIN_JO,B.COLOR_CODE AS PULLIN_COLOR_CODE,B.SIZE_CODE AS PULLIN_SIZE_CODE,B.PRODUCTION_LINE_CD AS PULLIN_LINE_CD,LINE.PRODUCTION_LINE_NAME AS PULLIN_LINE_NAME, SUM(ISNULL(C.PULLOUT_QTY,0)) AS PULLIN_QTY";
            //SQL = SQL + "       ,b.PRODUCTION_LINE_CD         ";
            SQL = SQL + "       from         PRD_JO_DISCREPANCY_PULLOUT_HD a WITH(NOLOCK) ";
            SQL = SQL + "       JOIN         PRD_JO_DISCREPANCY_PULLOUT_TRX b WITH(NOLOCK) ";
            SQL = SQL + "       ON A.DOC_NO=B.DOC_NO AND         A.FACTORY_CD=B.FACTORY_CD AND A.TRX_DATE=B.TRX_DATE ";
            SQL = SQL + "       JOIN JO_HD  PO WITH(NOLOCK) ON B.JOB_ORDER_NO=PO.JO_NO         ";
            SQL = SQL + "       JOIN PRD_JO_PULLOUT C WITH(NOLOCK) ON C.TRX_ID=B.TRX_ID ";
            SQL = SQL + "       AND           C.FACTORY_CD=A.FACTORY_CD ";
            SQL = SQL + "       LEFT JOIN dbo.GEN_PRODUCTION_LINE AS LINE ON B.PRODUCTION_LINE_CD=lINE.PRODUCTION_LINE_CD ";
            SQL = SQL + "       WHERE 1=1 and a.factory_cd='" + factoryCd + "' ";
            SQL = SQL + " AND A.FACTORY_CD=CASE WHEN A.PROCESS_TYPE='P' THEN po.CO_FACTORY_CD ELSE A.FACTORY_CD END ";
            if (garmentType != "")
            {
                SQL += " and PO.Garment_Type_Cd='" + garmentType + "'";
            }
            if (JoNo != "")
            {
                SQL += " and C.JO_NO like '" + JoNo + "%'";
            }
            if (processCd != "")
            {
                SQL += " and (A.process_cd='" + processCd + "')";
            }

            if (!processType.Equals(""))
            {
                SQL += " And a.process_type='" + processType + "' ";
            }
            if (!prodFactory.Equals(""))
            {
                SQL += " and a.PRODUCTION_FACTORY='" + prodFactory + "' ";
            }

            if (FromDate != "")
            {
                SQL += " and (A.trx_date>='" + FromDate + "')";
            }
            if (ToDate != "")
            {
                SQL += " and (A.trx_date<='" + ToDate + "')";
            }
            SQL = SQL + "        GROUP BY                A.PROCESS_CD,A.GARMENT_TYPE,A.PROCESS_TYPE,A.PRODUCTION_FACTORY,A.TRX_DATE,B.JOB_ORDER_NO,B.PRODUCTION_LINE_CD,LINE.PRODUCTION_LINE_NAME,B.COLOR_CODE,B.SIZE_CODE,C.JO_NO,C.PROD_LINE,C.COLOR_CODE,C.SIZE_CODE "; 
            //SQL = SQL + "       ,b.PRODUCTION_LINE_CD         ";
            SQL = SQL + "         ) AAA ";
            SQL = SQL + "        LEFT JOIN dbo.GEN_PRODUCTION_LINE L ON AAA.PRODUCTION_LINE_CD = L.PRODUCTION_LINE_CD ";
            SQL = SQL + "        AND L.FACTORY_CD ='" + factoryCd + "'";
            SQL = SQL + "        WHERE 1=1   ";
            if (!GroupName.Equals(""))
            {
                SQL = SQL + " AND AAA.PRODUCTION_LINE_CD IN (select FNFIELD from dbo.FN_SPLIT_STRING_TB(@s,','))      ";
            }
            SQL = SQL + "       GROUP BY AAA.PROCESS_CD,AAA.GARMENT_TYPE,AAA.PROCESS_TYPE,AAA.PRODUCTION_FACTORY,TRX_DATE,JOB_ORDER_NO,L.PRODUCTION_LINE_CD,L.PRODUCTION_lINE_NAME,COLOR_CODE,SIZE_CODE,SHORT_NAME,REASON_DESC,"; 
            SQL = SQL + "                  PULL_OUT_JO,PULL_OUT_COLOR_CODE,PULL_OUT_SIZE_CODE,PULL_OUT_LINE_CD,PULL_OUT_LINE_NAME,PULLIN_JO,PULLIN_COLOR_CODE,PULLIN_SIZE_CODE,PULLIN_LINE_CD,PULLIN_LINE_NAME";
            SQL = SQL + "       ) AS A ";

            SQL = SQL + @"  SELECT  *
                        FROM    #tmp1
                        UNION ALL
                        SELECT  '' AS Process_CD ,
                                '' AS GARMENT_TYPE,                                
                                NULL AS TRX_DATE ,
                                'TOTAL:' AS job_order_no ,
                                '' AS PRODUCTION_LINE_CD,
                                '' AS PRODUCTION_LINE_NAME,
                                '' AS COLOR_CODE,		
                                '' AS SIZE_CODE,
                                '' AS SHORT_NAME ,
                                '' AS REASON_DESC ,
                                SUM(discrepancy_qty) AS DISCREPANCY_QTY ,
                                '' AS PULL_OUT_JO ,
                                '' AS PULL_OUT_COLOR_CODE,
                                '' AS PULL_OUT_SIZE_CODE,
                                '' AS PULL_OUT_LINE_CD,
                                '' AS PULL_OUT_LINE_NAME,
                                SUM(PULLOUT_QTY) AS PULLOUT_QTY ,
                                '' AS PULLIN_JO ,
                                '' AS PULLIN_COLOR_CODE,
                                '' AS PULLIN_SIZE_CODE,
                                '' AS PULLIN_LINE_CD,
                                '' AS PULLIN_LINE_NAME,
                                SUM(PULLIN_QTY) AS PULLIN_QTY       
                            FROM   #tmp1 "; 

            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetYMGProDiscList(string garmentType, string factoryCd, string FromDate, string ToDate, string JoNo, string processCd, string processType, string prodFactory, string GroupName)
        {//Added By ZouShiChang ON 2013.10.07
         //Added FactoryCd checking on GEN_PRODUCTION_LINE by Jin Song 2015-01-13
            string SQL = "";
            if (!GroupName.Equals(""))
            {
                SQL = SQL + " declare @s NVARCHAR(max)";
                SQL = SQL + " select @s=system_value from gen_system_setting where factory_cd='" + factoryCd + "' and system_key='" + GroupName + "'; ";
            }

            SQL += @"IF OBJECT_ID('tempdb..#temp1') IS NOT NULL 
                BEGIN
                    DROP TABLE #temp1
                END ; 
 
 
            SELECT  
                    A.PROCESS_CD ,
                    A.GARMENT_TYPE,
                    A.TRX_DATE ,
                    B.JOB_ORDER_NO ,
                    C.REASON_CD,
                    D.REASON_DESC ,
                    D.QTY_AFFECTION as [Qty AFFECTION],
                    C.PULLOUT_QTY AS DISCREPANCY_QTY ,
                    B.PRODUCTION_LINE_CD ,
                    E.PRODUCTION_LINE_NAME
            INTO    #TEMP1
            FROM    PRD_JO_DISCREPANCY_PULLOUT_HD AS A
                    INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_TRX AS B ON A.DOC_NO = B.DOC_NO
                                                                      AND A.FACTORY_CD = B.FACTORY_CD
                                                                      AND A.TRX_DATE = B.TRX_DATE
                    INNER JOIN JO_HD AS JO ON B.JOB_ORDER_NO=JO.JO_NO  
                    LEFT JOIN PRD_JO_PULLOUT_REASON AS C ON B.TRX_ID = C.TRX_ID
                                                            AND A.FACTORY_CD = C.FACTORY_CD
                    LEFT JOIN dbo.PRD_REASON_CODE AS D ON C.FACTORY_CD = D.FACTORY_CD
                                                          AND C.REASON_CD = D.REASON_CD
                    LEFT JOIN dbo.GEN_PRODUCTION_LINE AS E ON B.PRODUCTION_LINE_CD = E.PRODUCTION_LINE_CD AND A.FACTORY_CD = E.FACTORY_CD ";
            SQL = SQL + "       WHERE 1=1 and a.factory_cd='" + factoryCd + "'    ";
            if (garmentType != "")
            {
                SQL += " and JO.Garment_Type_CD='" + garmentType + "'";
            }
            if (JoNo != "")
            {
                SQL += " and B.job_order_no like '" + JoNo + "%'";
            }
            if (processCd != "")
            {
                SQL += " and (A.process_cd='" + processCd + "')";
            }
            //Added By ZouShiChang ON 2013.09.24 Start MES024
            if (!processType.Equals(""))
            {
                SQL += " And a.process_type='" + processType + "' ";
            }
            if (!prodFactory.Equals(""))
            {
                SQL += " and a.PRODUCTION_FACTORY='" + prodFactory + "' ";
            }
            //Added By ZouShiChang ON 2013.09.24 Start MES024
            if (FromDate != "")
            {
                SQL += " and (A.trx_date>='" + FromDate + "')";
            }
            if (ToDate != "")
            {
                SQL += " and (A.trx_date<='" + ToDate + "')";
            }
            if (!GroupName.Equals(""))
            {
                SQL = SQL + " AND E.PRODUCTION_LINE_CD IN (select FNFIELD from dbo.FN_SPLIT_STRING_TB(@s,','))      ";
            }

            SQL += @"SELECT *
                     FROM   #temp1
                     WHERE DISCREPANCY_QTY<>0
                     UNION ALL
                     SELECT '' AS Process_CD ,
                            '' AS Garment_Type,
                            NULL AS TRX_DATE ,
                            'TOTAL:' AS job_order_no , 
                            '' as REASON_CD,       
                            '' AS REASON_DESC ,
                            '' as [Qty AFFECTION],
                            SUM(discrepancy_qty) AS DISCREPANCY_QTY ,          
                            '' AS PRODUCTION_LINE_CD ,
                            '' AS PRODUCTION_LINE_NAME
                     FROM   #temp1
                     ";

            return DBUtility.GetTable(SQL, "MES");
        }

    }
}