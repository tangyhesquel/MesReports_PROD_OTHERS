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
    ///wipReduceQuantitySql 的摘要说明
    ///Actual Cutting Report 
    /// </summary>
    public class wipReduceQuantitySql
    {

        public static bool GetCompletionStatus(string JoNo, string FactoryCd)
        {
            string SQL = "SELECT COMPLETE_STATUS from PRD_CUTTING_COMPLETION   WITH(NOLOCK) WHERE JOB_ORDER_NO='" + JoNo + "' AND FACTORY_CD='" + FactoryCd + "' AND COMPLETE_STATUS='Y'";
            if (DBUtility.GetTable(SQL, "MES").Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static DataTable GetReduceQuantityHead(string JoNo)
        {
            string SQL = "";
            SQL = " SELECT distinct PO.SC_NO scNo, ";
            SQL = SQL + "        PO.JO_NO joNo, ";
            SQL = SQL + "        SC.STYLE_NO styleNo, ";
            SQL = SQL + "        STH.STYLE_DESC styleDesc, ";
            SQL = SQL + "        DBO.DATE_FORMAT(BUYER_PO_DEL_DATE,'yyyy-mm-dd') gmtDelDate, ";
            SQL = SQL + "        DBO.DATE_FORMAT(getdate(),'yyyy-mm-dd') printDate, ";
            SQL = SQL + "        sc.season_cd season,CD.Cut_Date [date], ";
            SQL = SQL + "        CASE WHEN D.COMPLETE_STATUS='Y' THEN 'Finished' ELSE 'Un-Finished' END  AS COMPLETE_STATUS,D.COMPLETION_DATE";
            SQL = SQL + "    from SC_HD SC   WITH(NOLOCK) RIGHT JOIN JO_HD PO   WITH(NOLOCK) ON PO.SC_NO=SC.SC_NO ";
            SQL = SQL + "    LEFT JOIN style_hd STH   WITH(NOLOCK) ON STH.STYLE_NO=SC.STYLE_NO  ";
            SQL = SQL + "    AND STH.STYLE_REV_NO=SC.STYLE_REV_NO ";
            SQL = SQL + "    LEFT JOIN (select job_order_no,min(actual_print_date) as Cut_Date  ";
            SQL = SQL + "    From dbo.CUT_BUNDLE_HD   WITH(NOLOCK) where job_order_no='" + JoNo + "' ";
            SQL = SQL + "    GROUP BY job_order_no) CD ";
            SQL = SQL + "    on cd.job_order_no=PO.JO_NO ";
            //Added By ZouShiChang ON 2014.04.28 Start
            SQL = SQL + "    LEFT JOIN PRD_CUTTING_COMPLETION AS D ON PO.JO_NO=D.JOB_ORDER_NO    ";
            //Added By ZouShiChang ON 2014.04.28 End
            SQL = SQL + "    WHERE  PO.JO_NO='" + JoNo + "' ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetReduceQuantityHeadByGO(string GoNo, string FactoryCd)
        {

            string SQL = "";
            SQL += @"    SELECT A.SC_NO,SHORT_NAME ,
                                DEL_DATE=(SELECT CONVERT(VARCHAR(10),MIN(BUYER_PO_DEL_DATE),120) FROM JO_HD WITH(NOLOCK) WHERE SC_NO = '" + GoNo + "') ";
            SQL += @"    , CAST(SUM(TOTAL_QTY) AS DECIMAL(18,0)) AS TOTAL_ORDER_QTY ,    
                                TOTAL_CUT_QTY=(SELECT A.CutQty-ISNULL(B.ReduceQty,0) FROM ( 
                                SELECT SUM(c.RATIO * b.PLYS) AS CutQty FROM dbo.CUT_LAY AS a   WITH(NOLOCK) 
                                INNER JOIN  dbo.CUT_LAY_HD c   WITH(NOLOCK) ON c.LAY_ID=A.LAY_ID 
                                INNER JOIN   dbo.CUT_LAY_DT AS b   WITH(NOLOCK) ON c.LAY_TRANS_ID = b.LAY_TRANS_ID  
                                INNER JOIN JO_DT E WITH(NOLOCK) ON A.JOB_ORDER_NO = E.JO_NO 
                                AND B.COLOR_CD = E.COLOR_CODE AND (C.SIZE_CD + ' ' + C.SIZE_CD2) = (E.SIZE_CODE1+' '+E.SIZE_CODE2)                                
                                WHERE  EXISTS(SELECT JO_NO FROM JO_HD WITH(NOLOCK) WHERE SC_NO = '" + GoNo + "' AND JO_NO =A.JOB_ORDER_NO)     ";
            SQL += @"    AND (A.FACTORY_CD = '" + FactoryCd + "') AND  (A.PRINT_STATUS = 'Y')    ";
            SQL += @"                   ) A ,
                      (SELECT  SUM(REDUCE_QTY)  AS  ReduceQty FROM CUT_bundle_reduce_TRX A WITH(NOLOCK)
					where EXISTS(SELECT JO_NO FROM JO_HD WITH(NOLOCK) 
								WHERE SC_NO =  '" + GoNo + @"' AND JO_NO =A.JOB_ORDER_NO)        
				AND (A.FACTORY_CD =  '" + FactoryCd + @"') )B 	)						
                                ,LAST_BED_NO = (
	                                SELECT MAX(LAY_NO) FROM dbo.CUT_LAY AS a   WITH(NOLOCK)  
	                                INNER JOIN JO_HD B WITH(NOLOCK) ON B.JO_NO =a.JOB_ORDER_NO WHERE SC_NO = '" + GoNo + "' ";
            SQL += @"    )     
                                FROM SC_HD A WITH(NOLOCK) 
                                INNER JOIN GEN_CUSTOMER B WITH(NOLOCK) ON A.CUSTOMER_CD = B.CUSTOMER_CD 
                                INNER JOIN JO_HD C WITH(NOLOCK)  ON A.SC_NO = C.SC_NO 
                                WHERE A.SC_NO ='" + GoNo + "'     ";
            SQL += @"    GROUP BY A.SC_NO,SHORT_NAME";

            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetReduceQuantitySize(string JoNo)
        {
            string SQL = "";
            SQL = " SELECT A.SIZE_CODE FROM ";
            SQL = SQL + "		  (  ";
            SQL = SQL + @"		   select DISTINCT                             CASE A.SIZE_CODE2
                              WHEN '-' THEN A.SIZE_CODE1
                              ELSE A.SIZE_CODE1 + ' ' + A.SIZE_CODE2
                            END SIZE_CODE,s1.SEQUENCE SEQ1  ";
            SQL = SQL + "		   from ( ";
            SQL = SQL + "	         SELECT A.SC_NO,B.SIZE_CODE1,B.SIZE_CODE2 ";
            SQL = SQL + "                 FROM JO_hd a   WITH(NOLOCK), JO_dt b   WITH(NOLOCK)";
            SQL = SQL + "             where a.JO_no=b.JO_no  ";
            SQL = SQL + "             and a.JO_no='" + JoNo + "' ";
            SQL = SQL + "            ) A LEFT JOIN  ";
            SQL = SQL + "           (SELECT * FROM sc_size   WITH(NOLOCK) WHERE SIZE_TYPE='1') s1   ";
            SQL = SQL + "           ON a.sc_no=s1.sc_no and A.size_code1=s1.SIZE_CODE ";
            SQL = SQL + "          ) A LEFT JOIN ( ";
            SQL = SQL + @"           select DISTINCT                             CASE A.SIZE_CODE2
                              WHEN '-' THEN A.SIZE_CODE1
                              ELSE A.SIZE_CODE1 + ' ' + A.SIZE_CODE2
                            END SIZE_CODE,s1.SEQUENCE SEQ2  ";
            SQL = SQL + "           from ( ";
            SQL = SQL + "            SELECT A.SC_NO,B.SIZE_CODE1,B.SIZE_CODE2 ";
            SQL = SQL + "                FROM JO_hd a   WITH(NOLOCK), JO_dt b   WITH(NOLOCK)";
            SQL = SQL + "            where a.JO_no=b.JO_no  ";
            SQL = SQL + "            and a.JO_no='" + JoNo + "' ";
            SQL = SQL + "           ) A left join ";
            SQL = SQL + "           (SELECT * FROM sc_size   WITH(NOLOCK) WHERE SIZE_TYPE='2') s1   ";
            SQL = SQL + "           on a.sc_no=s1.sc_no and A.size_code1=s1.SIZE_CODE ";
            SQL = SQL + "          ) B ON A.SIZE_CODE=B.SIZE_CODE ";
            SQL = SQL + "        ORDER BY SEQ1,SEQ2 ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetReduceQuantitySizeByGO(string GoNo)
        {
            string SQL = "";
            SQL += " SELECT SIZE_CODE,SEQUENCE";
            SQL += " FROM dbo.SC_SIZE WIEH(NOLOCK) WHERE SC_NO = '" + GoNo + "'";
            SQL += " ORDER BY SEQUENCE";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetReduceQuantityColor(string JoNo, bool Just_Color)
        {
            string SQL = "";
            SQL = SQL + " if object_id('tempdb..#TEMP_001') is not null begin drop table #TEMP_001 end ; ";

            SQL = SQL + " SELECT A.COLOR_CD,A.JO_NO";
            SQL = SQL + " ,WASTAGE = (SELECT WASTAGE FROM FN_CUT_SEW_WASTAGE(A.MO_NO) e WHERE COLORCD = A.COLOR_CD) ";
            SQL = SQL + " INTO #TEMP_001";
            SQL = SQL + " FROM CP_MO_JO A WITH(NOLOCK) WHERE JO_NO = '" + JoNo + "';";

            SQL = SQL + " if object_id('tempdb..#temp_t') is not null begin drop table #temp_t end ; ";
            SQL = SQL + " select DISTINCT B.COLOR_CODE,C.COLOR_DESC";
            if (!Just_Color)
            {
                //Added By ZouShiChang ON 2014.04.30 Start
                //SQL = SQL + " ,CAST(MAX(D.NET_YPD) as decimal(38, 2)) AS MARKER_NET_YPD ,F.PPO_NO AS PPO ";
                SQL = SQL + " ,J.PPO_NET_YPD_NEW ";
                SQL = SQL + " ,CAST(MAX(D.NET_YPD) as decimal(38, 2)) AS MARKER_NET_YPD ";
                //Added By ZouShiChang ON 2014.04.30 End

                SQL = SQL + " ,CAST((MAX(D.NET_YPD)*(1+ISNULL(E.wastage,0)/100)) as decimal(38, 2)) AS MARKER_YPD ";
                SQL = SQL + " ,CAST((MAX(D.NET_YPD)*(1+ISNULL(E.wastage,0)/100)+D.BINDING_YPD) as decimal(38, 2)) AS BULK_YPD";
            }
            SQL = SQL + " into #temp_t ";
            SQL = SQL + "    from JO_HD A   WITH(NOLOCK)";
            SQL = SQL + " inner join JO_DT B   WITH(NOLOCK)";
            SQL = SQL + " ON  A.JO_NO=B.JO_NO AND B.JO_NO ='" + JoNo + "'";
            SQL = SQL + " inner join SC_COLOR C   WITH(NOLOCK)";
            SQL = SQL + " ON C.SC_NO=A.SC_NO AND C.COLOR_CODE=B.COLOR_CODE ";
            SQL = SQL + " left join FN_GET_JO_YPD('" + JoNo + "') D  ";
            SQL = SQL + " on D.JO_NO=B.JO_NO AND D.COLOR_CD=B.COLOR_CODE";
            SQL = SQL + " left join #TEMP_001 E";
            SQL = SQL + " on E.COLOR_CD = B.COLOR_CODE";
            SQL = SQL + " LEFT JOIN ";
            SQL = SQL + " (SELECT DISTINCT C.COLOR_CD,B.PPO_NO FROM CP_MO_HD A    WITH(NOLOCK)";
            SQL = SQL + " INNER JOIN CP_FABRIC_ITEM B   WITH(NOLOCK)";
            SQL = SQL + " ON A.GO_NO=B.GO_NO AND A.PART_TYPE=B.PART_TYPE_CD ";
            SQL = SQL + " INNER JOIN CP_MO_JO C   WITH(NOLOCK) ON  ";
            SQL = SQL + " A.MO_NO=C.MO_NO AND B.COLOR_CD=C.COLOR_CD ";
            SQL = SQL + " WHERE C.JO_NO='" + JoNo + "' ) AS F";
            SQL = SQL + " ON F.COLOR_CD = B.COLOR_CODE ";
            SQL = SQL + " LEFT JOIN CP_JO_YPD AS J ON A.JO_NO=J.JOB_ORDER_NO AND J.COLOR_CODE=B.COLOR_CODE ";//Added By ZouShiChang ON 2014.04.30
            SQL = SQL + "    WHERE A.JO_NO='" + JoNo + "'";
            //SQL = SQL + " GROUP BY B.COLOR_CODE,C.COLOR_DESC,E.WASTAGE,D.BINDING_YPD,F.PPO_NO ";
            SQL = SQL + " GROUP BY B.COLOR_CODE,C.COLOR_DESC,E.WASTAGE,D.BINDING_YPD,J.PPO_NET_YPD_NEW ";
            SQL = SQL + "    order by B.COLOR_CODE,COLOR_DESC; ";

            SQL = SQL + " select * from #temp_t ";

            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetReduceQuantityColorByGO(string GoNo)
        {
            string SQL = "";
            SQL += " SELECT COLOR_CODE,COLOR_DESC,SEQUENCE FROM SC_COLOR WITH(NOLOCK)";
            SQL += " WHERE SC_NO = '" + GoNo + "'";
            SQL += " UNION";
            SQL += " SELECT 'TOTAL' AS COLOR_CODE,'' AS COLOR_DESC , SEQUENCE = 9999";
            SQL += " ORDER BY SEQUENCE";
            return DBUtility.GetTable(SQL, "MES");
        }
        /*
                public static DataTable GetReduceQuantity(string JoNo, string FactoryCd)
                {
                    //Added By ZouShiChang ON 2013.09.10 Start 将CUT的取数更改为新方式

                    string SQL = "";
                    SQL = @"IF OBJECT_ID('tempdb..#TEMP1') IS NOT NULL 
                                        BEGIN
                                            DROP TABLE #TEMP1
                                        END ;
                                    SELECT  *
                                    INTO    #Temp1
                                    FROM    (
                                               select SIZE_CD, COLOR_CD,ABS(SUM(QTY)) AS CUT_REDUCE FROM CUT_BUNDLE_HD WITH ( NOLOCK )
                                               WHERE  JOB_ORDER_NO = '" + JoNo + @"' 
                                                        AND FACTORY_CD = '" + FactoryCd + @"' 
                                                        AND TRX_TYPE = 'RD'
                                                        AND STATUS='Y'
                                               GROUP BY COLOR_CD,SIZE_CD

                                            ) AS a ;

                                    SELECT  C_1.SIZE_CD ,
                                            C_1.COLOR_CD ,
                                             ISNULL(C_1.ORDER_QTY, 0) ORDER_QTY ,
                                            ISNULL(A_1.CUT_QTY, 0) CUT_QTY ,
                                            ISNULL(A_2.SSample_QTY,0) AS SSample_QTY,
                                            ISNULL(A_3.CSample_QTY,0) AS CSample_QTY,
                                            ISNULL(A_4.ReCut_QTY,0) AS ReCut_QTY,
                                            ( CASE WHEN ORDER_QTY = 0 THEN 0
                                                   WHEN ISNULL(ORDER_QTY, '') = '' THEN ''
                                                   ELSE CONVERT(DECIMAL(18, 3), ROUND(( ISNULL(CUT_QTY, 0)+ISNULL(SSample_QTY,0)+ISNULL(CSample_QTY,0)+ISNULL(ReCut_QTY,0)
                                                                                        - ISNULL(CUT_REDUCE, 0)
                                                                                        - ISNULL(ORDER_QTY, 0) )
                                                                                      * 100 / ORDER_QTY, 3))
                                              END ) AS OVER_SHORTAGE_PERCENT ,
                                            ISNULL(B_1.CUT_REDUCE, 0) CUT_REDUCE ,
                                            ISNULL(A_1.CUT_QTY, 0)+ISNULL(SSample_QTY,0)+ISNULL(CSample_QTY,0)+ISNULL(ReCut_QTY,0) - ISNULL(B_1.CUT_REDUCE, 0) AS ACTUAL_QTY ,
                                            ISNULL(A_1.CUT_QTY, 0)++ISNULL(SSample_QTY,0)+ISNULL(CSample_QTY,0)+ISNULL(ReCut_QTY,0) - ISNULL(B_1.CUT_REDUCE, 0)
                                            - ISNULL(C_1.ORDER_QTY, 0) AS OVER_SHORTAGE_QTY ,
                                            ISNULL(D.CUT_DISCREPANCY_QTY, 0) AS CUT_DISCREPANCY_QTY ,
                                            ISNULL(D.EMB_DISCREPANCY_QTY, 0) AS EMB_DISCREPANCY_QTY ,
                                            ISNULL(D.PRINT_DISCREPANCY_QTY, 0) AS PRINT_DISCREPANCY_QTY,
                                            ISNULL(A_1.CUT_QTY, 0)+ISNULL(SSample_QTY,0)+ISNULL(CSample_QTY,0)+ISNULL(ReCut_QTY,0) - ISNULL(B_1.CUT_REDUCE, 0)
                                            - ISNULL(D.CUT_DISCREPANCY_QTY, 0) - ISNULL(D.EMB_DISCREPANCY_QTY, 0)
                                            - ISNULL(D.PRINT_DISCREPANCY_QTY, 0) AS Final_Qty
                                    FROM    ( SELECT DISTINCT
                                                        CASE SIZE_CODE2 WHEN '-' THEN SIZE_CODE1 ELSE SIZE_CODE1+' '+SIZE_CODE2 END AS SIZE_CD, 
                                                        COLOR_CODE AS COLOR_CD ,
                                                        QTY AS ORDER_QTY
                                              FROM      dbo.JO_DT AS c WITH ( NOLOCK )
                                              WHERE     ( JO_NO = '" + JoNo + @"' )
                                            ) AS C_1
                                            LEFT OUTER JOIN ( SELECT    CASE C.SIZE_CD2 WHEN '-' THEN C.SIZE_CD ELSE C.SIZE_CD+' '+C.SIZE_CD2 END AS SIZE_CD ,
                                                                        b.COLOR_CD ,
                                                                        SUM(D.QTY) AS CUT_QTY  --SUM(c.RATIO * b.PLYS) AS CUT_QTY
                                                              FROM      dbo.CUT_LAY AS a WITH ( NOLOCK )
                                                                        INNER JOIN dbo.CUT_LAY_HD c WITH ( NOLOCK ) ON c.LAY_ID = A.LAY_ID
                                                                        INNER JOIN dbo.CUT_LAY_DT AS b WITH ( NOLOCK ) ON c.LAY_TRANS_ID = b.LAY_TRANS_ID
                                                                        INNER JOIN CUT_BUNDLE_HD AS D ON D.LAY_DT_ID=B.LAY_DT_ID
                                                              WHERE     ( a.JOB_ORDER_NO = '" + JoNo + @"' )
                                                                        AND ( a.FACTORY_CD = '" + FactoryCd + @"' )
                                                                        AND ( a.PRINT_STATUS = 'Y' )
                                                                        AND (D.TRX_TYPE='NM')
                                                                        AND (A.CUT_TYPE='C' OR A.CUT_TYPE IS NULL OR A.CUT_TYPE='')
                                                              GROUP BY  b.COLOR_CD ,
                                                                        CASE C.SIZE_CD2 WHEN '-' THEN C.SIZE_CD ELSE C.SIZE_CD+' '+C.SIZE_CD2 END
                                                            ) AS A_1 ON A_1.SIZE_CD = C_1.SIZE_CD
                                                                        AND A_1.COLOR_CD = C_1.COLOR_CD

                                             LEFT JOIN ( SELECT    CASE C.SIZE_CD2 WHEN '-' THEN C.SIZE_CD ELSE C.SIZE_CD+' '+C.SIZE_CD2 END AS SIZE_CD ,
                                                                        b.COLOR_CD ,                                                                
                                                                        SUM(D.QTY) AS SSample_QTY  --SUM(c.RATIO * b.PLYS) AS CUT_QTY
                                                              FROM      dbo.CUT_LAY AS a WITH ( NOLOCK )
                                                                        INNER JOIN dbo.CUT_LAY_HD c WITH ( NOLOCK ) ON c.LAY_ID = A.LAY_ID
                                                                        INNER JOIN dbo.CUT_LAY_DT AS b WITH ( NOLOCK ) ON c.LAY_TRANS_ID = b.LAY_TRANS_ID
                                                                        INNER JOIN CUT_BUNDLE_HD AS D ON D.LAY_DT_ID=B.LAY_DT_ID
                                                              WHERE     ( a.JOB_ORDER_NO = '"+ JoNo +@"' )
                                                                        AND ( a.FACTORY_CD = '"+FactoryCd +@"' )
                                                                        AND ( a.PRINT_STATUS = 'Y' )
                                                                        AND (D.TRX_TYPE='NM')
                                                                        AND (A.CUT_TYPE='SS')
                                                              GROUP BY  b.COLOR_CD ,
                                                                        CASE C.SIZE_CD2 WHEN '-' THEN C.SIZE_CD ELSE C.SIZE_CD+' '+C.SIZE_CD2 END
                                                                
                                                            ) AS A_2 ON A_2.SIZE_CD = C_1.SIZE_CD
                                                                        AND A_2.COLOR_CD = C_1.COLOR_CD        
                                            LEFT JOIN ( SELECT    CASE C.SIZE_CD2 WHEN '-' THEN C.SIZE_CD ELSE C.SIZE_CD+' '+C.SIZE_CD2 END AS SIZE_CD ,
                                                                        b.COLOR_CD ,
                                                                        SUM(D.QTY) AS CSample_QTY  --SUM(c.RATIO * b.PLYS) AS CUT_QTY
                                                              FROM      dbo.CUT_LAY AS a WITH ( NOLOCK )
                                                                        INNER JOIN dbo.CUT_LAY_HD c WITH ( NOLOCK ) ON c.LAY_ID = A.LAY_ID
                                                                        INNER JOIN dbo.CUT_LAY_DT AS b WITH ( NOLOCK ) ON c.LAY_TRANS_ID = b.LAY_TRANS_ID
                                                                        INNER JOIN CUT_BUNDLE_HD AS D ON D.LAY_DT_ID=B.LAY_DT_ID
                                                              WHERE     ( a.JOB_ORDER_NO = '"+JoNo+@"' )
                                                                        AND ( a.FACTORY_CD = '"+FactoryCd+@"' )
                                                                        AND ( a.PRINT_STATUS = 'Y' )
                                                                        AND (D.TRX_TYPE='NM')
                                                                        AND (A.CUT_TYPE='CS')
                                                              GROUP BY  b.COLOR_CD ,
                                                                        CASE C.SIZE_CD2 WHEN '-' THEN C.SIZE_CD ELSE C.SIZE_CD+' '+C.SIZE_CD2 END
                                                            ) AS A_3 ON A_3.SIZE_CD = C_1.SIZE_CD
                                                                        AND A_3.COLOR_CD = C_1.COLOR_CD    
                                            LEFT JOIN ( SELECT    CASE C.SIZE_CD2 WHEN '-' THEN C.SIZE_CD ELSE C.SIZE_CD+' '+C.SIZE_CD2 END AS SIZE_CD ,
                                                                        b.COLOR_CD ,                                                                
                                                                        SUM(D.QTY) AS ReCut_QTY  --SUM(c.RATIO * b.PLYS) AS CUT_QTY
                                                              FROM      dbo.CUT_LAY AS a WITH ( NOLOCK )
                                                                        INNER JOIN dbo.CUT_LAY_HD c WITH ( NOLOCK ) ON c.LAY_ID = A.LAY_ID
                                                                        INNER JOIN dbo.CUT_LAY_DT AS b WITH ( NOLOCK ) ON c.LAY_TRANS_ID = b.LAY_TRANS_ID
                                                                        INNER JOIN CUT_BUNDLE_HD AS D ON D.LAY_DT_ID=B.LAY_DT_ID
                                                              WHERE     ( a.JOB_ORDER_NO = '"+JoNo +@"' )
                                                                        AND ( a.FACTORY_CD = '"+FactoryCd+@"' )
                                                                        AND ( a.PRINT_STATUS = 'Y' )
                                                                        AND (D.TRX_TYPE='NM')
                                                                        AND (A.CUT_TYPE='R')
                                                              GROUP BY  b.COLOR_CD ,
                                                                        CASE C.SIZE_CD2 WHEN '-' THEN C.SIZE_CD ELSE C.SIZE_CD+' '+C.SIZE_CD2 END                                                                       ) AS A_4 ON A_4.SIZE_CD = C_1.SIZE_CD
                                                                        AND A_4.COLOR_CD = C_1.COLOR_CD     
                                    
                                            LEFT OUTER JOIN #TEMP1 AS B_1 ON B_1.SIZE_CD = C_1.SIZE_CD
                                                                             AND B_1.COLOR_CD = C_1.COLOR_CD
                                            LEFT JOIN ( SELECT  SIZE_CODE AS SIZE_CD ,
                                                                COLOR_CODE AS COLOR_CD ,
                                                                SUM(DISCREPANCY_QTY) AS CUT_DISCREPANCY_QTY ,
                                                                NULL AS EMB_DISCREPANCY_QTY ,
                                                                NULL AS PRINT_DISCREPANCY_QTY
                                                        FROM    dbo.PRD_JO_DISCREPANCY_PULLOUT_HD AS A
                                                                INNER JOIN dbo.PRD_JO_DISCREPANCY_PULLOUT_TRX AS B ON A.DOC_NO = B.DOC_NO
                                                        WHERE   B.JOB_ORDER_NO = '" + JoNo + @"'
                                                                AND PROCESS_CD='CUT'
                                                        GROUP BY SIZE_CODE ,
                                                                COLOR_CODE
                                                        UNION ALL
                                                        SELECT  SIZE_CODE AS SIZE_CD ,
                                                                COLOR_CODE AS COLOR_CD ,
                                                                NULL AS CUT_DISCREPANCY_QTY ,
                                                                SUM(DISCREPANCY_QTY) AS EMB_DISCREPANCY_QTY ,
                                                                NULL AS PRINT_DISCREPANCY_QTY
                                                        FROM    dbo.PRD_JO_DISCREPANCY_PULLOUT_HD AS A
                                                                INNER JOIN dbo.PRD_JO_DISCREPANCY_PULLOUT_TRX AS B ON A.DOC_NO = B.DOC_NO
                                                        WHERE   B.JOB_ORDER_NO = '" + JoNo + @"'
                                                                AND PROCESS_CD='EMB'
                                                        GROUP BY SIZE_CODE ,
                                                                COLOR_CODE
                                                        UNION ALL
                                                        SELECT  SIZE_CODE AS SIZE_CD ,
                                                                COLOR_CODE AS COLOR_CD ,
                                                                NULL AS CUT_DISCREPANCY_QTY ,
                                                                NULL AS EMB_DISCREPANCY_QTY ,
                                                                SUM(DISCREPANCY_QTY) AS PRINT_DISCREPANCY_QTY
                                                        FROM    dbo.PRD_JO_DISCREPANCY_PULLOUT_HD AS A
                                                                INNER JOIN dbo.PRD_JO_DISCREPANCY_PULLOUT_TRX AS B ON A.DOC_NO = B.DOC_NO
                                                        WHERE   B.JOB_ORDER_NO = '" + JoNo + @"'
                                                                AND PROCESS_CD='PRT'
                                                        GROUP BY SIZE_CODE ,
                                                                COLOR_CODE
                                                      ) AS D ON B_1.SIZE_CD = D.SIZE_CD
                                                                AND B_1.COLOR_CD = D.COLOR_CD";

           
                    return DBUtility.GetTable(SQL, "MES");
                }
        */


        public static DataTable GetReduceQuantity(string JoNo, string FactoryCd)
        {
            //Bug fix by ZK 2014-08-04 Add Order Qty = 0 checking
            //Bug fix by Jin Song 2014-08-04 Arrange ReduceQuantity Header (Arrange Size Cd 2014-08-05)
            string SQL = @"
                        DECLARE @CUT_TYPE NVARCHAR(500)
                        DECLARE @ReCol NVARCHAR(500)                                            
                        DECLARE @SQL VARCHAR(MAX)
                        DECLARE @JOB_ORDER_NO NVARCHAR(30)
                        DECLARE @FACTORY_CD NVARCHAR(3)


                        DECLARE @SIZ_CD_COL NVARCHAR(MAX)
                        DECLARE @SIZ_CD_COL_ADD NVARCHAR(MAX)
                        DECLARE @COL_NAME NVARCHAR(30)
                        DECLARE @SEQ CHAR(3)
                        DECLARE @TEMP_TABLE_COL NVARCHAR(MAX)
 
                        SET @JOB_ORDER_NO = '" + JoNo + @"'
                        SET @FACTORY_CD = '" + FactoryCd + @"'
                        SET @SEQ = '1';
 
                        IF OBJECT_ID('tempdb..#TEMP1') IS NOT NULL
                        BEGIN
                            DROP TABLE #TEMP1
                        END;
                        SELECT *
                        INTO   #Temp1
                        FROM   ( SELECT    SIZE_CD ,
                                        COLOR_CD ,
                                        ABS(SUM(QTY)) AS CUT_REDUCE
                                FROM      CUT_BUNDLE_HD WITH ( NOLOCK )
                                WHERE     JOB_ORDER_NO = @JOB_ORDER_NO
                                        AND FACTORY_CD = @FACTORY_CD
                                        AND TRX_TYPE = 'RD'
                                        AND STATUS = 'Y'
                                GROUP BY  COLOR_CD ,
                                        SIZE_CD
                            ) AS a;
                        IF OBJECT_ID('tempdb..#CUT_TEMP') IS NOT NULL
                        BEGIN
                            DROP TABLE #CUT_TEMP
                        END;
                        SELECT C_1.SIZE_CD ,
                            C_1.COLOR_CD ,
                            ISNULL(C_1.ORDER_QTY, 0) ORDER_QTY ,
                            ISNULL(A_1.CUT_QTY, 0) CUT_QTY ,
                            ISNULL(A_2.SSample_QTY, 0) AS SSample_QTY ,
                            ISNULL(A_3.CSample_QTY, 0) AS CSample_QTY ,
                            ISNULL(A_4.ReCut_QTY, 0) AS ReCut_QTY ,
                            ( CASE WHEN ORDER_QTY = 0 THEN 0
                                    WHEN ISNULL(ORDER_QTY, '') = '' THEN ''
                                    ELSE CONVERT(DECIMAL(18, 3), ROUND(( ISNULL(CUT_QTY, 0)
                                                                        + ISNULL(SSample_QTY, 0)
                                                                        + ISNULL(CSample_QTY, 0)
                                                                        + ISNULL(ReCut_QTY, 0)
                                                                        - ISNULL(CUT_REDUCE, 0)
                                                                        - ISNULL(ORDER_QTY, 0) )
                                                                        * 100 / ORDER_QTY, 3))
                                END ) AS OVER_SHORTAGE_PERCENT ,
                            ISNULL(B_1.CUT_REDUCE, 0) CUT_REDUCE ,
                            ISNULL(A_1.CUT_QTY, 0) + ISNULL(SSample_QTY, 0) + ISNULL(CSample_QTY,
                                                                                    0)
                            + ISNULL(ReCut_QTY, 0) - ISNULL(B_1.CUT_REDUCE, 0) AS ACTUAL_QTY ,
                            ISNULL(A_1.CUT_QTY, 0) + ISNULL(SSample_QTY, 0) + ISNULL(CSample_QTY,
                                                                                    0)
                            + ISNULL(ReCut_QTY, 0) - ISNULL(B_1.CUT_REDUCE, 0)
                            - ISNULL(C_1.ORDER_QTY, 0) AS OVER_SHORTAGE_QTY ,
                            ISNULL(D.CUT_DISCREPANCY_QTY, 0) AS CUT_DISCREPANCY_QTY ,
                            ISNULL(D.EMB_DISCREPANCY_QTY, 0) AS EMB_DISCREPANCY_QTY ,
                            ISNULL(D.PRINT_DISCREPANCY_QTY, 0) AS PRINT_DISCREPANCY_QTY ,
                            ISNULL(A_1.CUT_QTY, 0) + ISNULL(SSample_QTY, 0) + ISNULL(CSample_QTY,
                                                                                    0)
                            + ISNULL(ReCut_QTY, 0) - ISNULL(B_1.CUT_REDUCE, 0)
                            - ISNULL(D.CUT_DISCREPANCY_QTY, 0) - ISNULL(D.EMB_DISCREPANCY_QTY, 0)
                            - ISNULL(D.PRINT_DISCREPANCY_QTY, 0) AS Final_Qty
                        INTO   #CUT_TEMP
                        FROM   ( SELECT DISTINCT
                                        CASE SIZE_CODE2
                                            WHEN '-' THEN SIZE_CODE1
                                            ELSE SIZE_CODE1 + ' ' + SIZE_CODE2
                                        END AS SIZE_CD ,
                                        COLOR_CODE AS COLOR_CD ,
                                        QTY AS ORDER_QTY
                                FROM      dbo.JO_DT AS c WITH ( NOLOCK )
                                WHERE     ( JO_NO = @JOB_ORDER_NO )
                            ) AS C_1
                            LEFT OUTER JOIN ( SELECT    CASE C.SIZE_CD2
                                                            WHEN '-' THEN C.SIZE_CD
                                                            ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                        END AS SIZE_CD ,
                                                        b.COLOR_CD ,
                                                        SUM(D.QTY) AS CUT_QTY
                                                FROM      dbo.CUT_LAY AS a WITH ( NOLOCK )
                                                        INNER JOIN dbo.CUT_LAY_HD c WITH ( NOLOCK ) ON c.LAY_ID = A.LAY_ID
                                                        INNER JOIN dbo.CUT_LAY_DT AS b WITH ( NOLOCK ) ON c.LAY_TRANS_ID = b.LAY_TRANS_ID
                                                        INNER JOIN CUT_BUNDLE_HD AS D ON D.LAY_DT_ID = B.LAY_DT_ID
                                                WHERE     ( a.JOB_ORDER_NO = @JOB_ORDER_NO )
                                                        AND ( a.FACTORY_CD = @FACTORY_CD )
                                                        AND ( a.PRINT_STATUS = 'Y' )
                                                        AND ( D.TRX_TYPE = 'NM' )
                                                        AND ( A.CUT_TYPE = 'C'
                                                                OR A.CUT_TYPE IS NULL
                                                                OR A.CUT_TYPE = ''
                                                            )
                                                GROUP BY  b.COLOR_CD ,
                                                        CASE C.SIZE_CD2
                                                            WHEN '-' THEN C.SIZE_CD
                                                            ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                        END
                                            ) AS A_1 ON A_1.SIZE_CD = C_1.SIZE_CD
                                                        AND A_1.COLOR_CD = C_1.COLOR_CD
                            LEFT JOIN ( SELECT  CASE C.SIZE_CD2
                                                    WHEN '-' THEN C.SIZE_CD
                                                    ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                END AS SIZE_CD ,
                                                b.COLOR_CD ,
                                                SUM(D.QTY) AS SSample_QTY  --SUM(c.RATIO * b.PLYS) AS CUT_QTY
                                        FROM    dbo.CUT_LAY AS a WITH ( NOLOCK )
                                                INNER JOIN dbo.CUT_LAY_HD c WITH ( NOLOCK ) ON c.LAY_ID = A.LAY_ID
                                                INNER JOIN dbo.CUT_LAY_DT AS b WITH ( NOLOCK ) ON c.LAY_TRANS_ID = b.LAY_TRANS_ID
                                                INNER JOIN CUT_BUNDLE_HD AS D ON D.LAY_DT_ID = B.LAY_DT_ID
                                        WHERE   ( a.JOB_ORDER_NO = @JOB_ORDER_NO )
                                                AND ( a.FACTORY_CD = @FACTORY_CD )
                                                AND ( a.PRINT_STATUS = 'Y' )
                                                AND ( D.TRX_TYPE = 'NM' )
                                                AND ( A.CUT_TYPE = 'SS' )
                                        GROUP BY b.COLOR_CD ,
                                                CASE C.SIZE_CD2
                                                    WHEN '-' THEN C.SIZE_CD
                                                    ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                END
                                        ) AS A_2 ON A_2.SIZE_CD = C_1.SIZE_CD
                                                    AND A_2.COLOR_CD = C_1.COLOR_CD
                            LEFT JOIN ( SELECT  CASE C.SIZE_CD2
                                                    WHEN '-' THEN C.SIZE_CD
                                                    ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                END AS SIZE_CD ,
                                                b.COLOR_CD ,
                                                SUM(D.QTY) AS CSample_QTY  --SUM(c.RATIO * b.PLYS) AS CUT_QTY
                                        FROM    dbo.CUT_LAY AS a WITH ( NOLOCK )
                                                INNER JOIN dbo.CUT_LAY_HD c WITH ( NOLOCK ) ON c.LAY_ID = A.LAY_ID
                                                INNER JOIN dbo.CUT_LAY_DT AS b WITH ( NOLOCK ) ON c.LAY_TRANS_ID = b.LAY_TRANS_ID
                                                INNER JOIN CUT_BUNDLE_HD AS D ON D.LAY_DT_ID = B.LAY_DT_ID
                                        WHERE   ( a.JOB_ORDER_NO = @JOB_ORDER_NO )
                                                AND ( a.FACTORY_CD = @FACTORY_CD )
                                                AND ( a.PRINT_STATUS = 'Y' )
                                                AND ( D.TRX_TYPE = 'NM' )
                                                AND ( A.CUT_TYPE = 'CS' )
                                        GROUP BY b.COLOR_CD ,
                                                CASE C.SIZE_CD2
                                                    WHEN '-' THEN C.SIZE_CD
                                                    ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                END
                                        ) AS A_3 ON A_3.SIZE_CD = C_1.SIZE_CD
                                                    AND A_3.COLOR_CD = C_1.COLOR_CD
                            LEFT JOIN ( SELECT  CASE C.SIZE_CD2
                                                    WHEN '-' THEN C.SIZE_CD
                                                    ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                END AS SIZE_CD ,
                                                b.COLOR_CD ,
                                                SUM(D.QTY) AS ReCut_QTY  --SUM(c.RATIO * b.PLYS) AS CUT_QTY
                                        FROM    dbo.CUT_LAY AS a WITH ( NOLOCK )
                                                INNER JOIN dbo.CUT_LAY_HD c WITH ( NOLOCK ) ON c.LAY_ID = A.LAY_ID
                                                INNER JOIN dbo.CUT_LAY_DT AS b WITH ( NOLOCK ) ON c.LAY_TRANS_ID = b.LAY_TRANS_ID
                                                INNER JOIN CUT_BUNDLE_HD AS D ON D.LAY_DT_ID = B.LAY_DT_ID
                                        WHERE   ( a.JOB_ORDER_NO = @JOB_ORDER_NO )
                                                AND ( a.FACTORY_CD = @FACTORY_CD )
                                                AND ( a.PRINT_STATUS = 'Y' )
                                                AND ( D.TRX_TYPE = 'NM' )
                                                AND ( A.CUT_TYPE = 'R' )
                                        GROUP BY b.COLOR_CD ,
                                                CASE C.SIZE_CD2
                                                    WHEN '-' THEN C.SIZE_CD
                                                    ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                END
                                        ) AS A_4 ON A_4.SIZE_CD = C_1.SIZE_CD
                                                    AND A_4.COLOR_CD = C_1.COLOR_CD
                            LEFT OUTER JOIN #TEMP1 AS B_1 ON B_1.SIZE_CD = C_1.SIZE_CD
                                                                AND B_1.COLOR_CD = C_1.COLOR_CD
                            LEFT JOIN ( SELECT  SIZE_CODE AS SIZE_CD ,
                                                COLOR_CODE AS COLOR_CD ,
                                                SUM(DISCREPANCY_QTY) AS CUT_DISCREPANCY_QTY ,
                                                NULL AS EMB_DISCREPANCY_QTY ,
                                                NULL AS PRINT_DISCREPANCY_QTY
                                        FROM    dbo.PRD_JO_DISCREPANCY_PULLOUT_HD AS A
                                                INNER JOIN dbo.PRD_JO_DISCREPANCY_PULLOUT_TRX AS B ON A.DOC_NO = B.DOC_NO
                                        WHERE   B.JOB_ORDER_NO = @JOB_ORDER_NO
                                                AND PROCESS_CD = 'CUT2'
                                        GROUP BY SIZE_CODE ,
                                                COLOR_CODE
                                        UNION ALL
                                        SELECT  SIZE_CODE AS SIZE_CD ,
                                                COLOR_CODE AS COLOR_CD ,
                                                NULL AS CUT_DISCREPANCY_QTY ,
                                                SUM(DISCREPANCY_QTY) AS EMB_DISCREPANCY_QTY ,
                                                NULL AS PRINT_DISCREPANCY_QTY
                                        FROM    dbo.PRD_JO_DISCREPANCY_PULLOUT_HD AS A
                                                INNER JOIN dbo.PRD_JO_DISCREPANCY_PULLOUT_TRX AS B ON A.DOC_NO = B.DOC_NO
                                        WHERE   B.JOB_ORDER_NO = @JOB_ORDER_NO
                                                AND PROCESS_CD = 'EMB'
                                        GROUP BY SIZE_CODE ,
                                                COLOR_CODE
                                        UNION ALL
                                        SELECT  SIZE_CODE AS SIZE_CD ,
                                                COLOR_CODE AS COLOR_CD ,
                                                NULL AS CUT_DISCREPANCY_QTY ,
                                                NULL AS EMB_DISCREPANCY_QTY ,
                                                SUM(DISCREPANCY_QTY) AS PRINT_DISCREPANCY_QTY
                                        FROM    dbo.PRD_JO_DISCREPANCY_PULLOUT_HD AS A
                                                INNER JOIN dbo.PRD_JO_DISCREPANCY_PULLOUT_TRX AS B ON A.DOC_NO = B.DOC_NO
                                        WHERE   B.JOB_ORDER_NO = @JOB_ORDER_NO
                                                AND PROCESS_CD = 'PRT'
                                        GROUP BY SIZE_CODE ,
                                                COLOR_CODE
                                        ) AS D ON C_1.SIZE_CD = D.SIZE_CD
                                                AND C_1.COLOR_CD = D.COLOR_CD
                              
 
                        SELECT @CUT_TYPE = ( SELECT  DISTINCT
                                                    '[Re-Cut'
                                                    + +LTRIM(RTRIM(CAST(REPAIR_CUT_NUM AS CHAR(4))))
                                                    + ']' + ','
                                            FROM      CUT_LAY
                                            WHERE     job_order_no = @JOB_ORDER_NO
                                                    AND CUT_TYPE = 'R'
                                                    AND FACTORY_CD = @FACTORY_CD
                                        FOR
                                            XML PATH('')
                                        ) 
                        SET @CUT_TYPE = SUBSTRING(@CUT_TYPE, 0, LEN(@CUT_TYPE))
                        PRINT @CUT_TYPE
                                                        
                        SELECT @ReCol = ( SELECT  DISTINCT
                                                'ISNULL([Re-Cut'
                                                + +LTRIM(RTRIM(CAST(REPAIR_CUT_NUM AS CHAR(4))))
                                                + '],0) AS ' + '[Re-Cut'
                                                + +LTRIM(RTRIM(CAST(REPAIR_CUT_NUM AS CHAR(4))))
                                                + '],'
                                        FROM     CUT_LAY
                                        WHERE    job_order_no = @JOB_ORDER_NO
                                                AND CUT_TYPE = 'R'
                                                AND FACTORY_CD = @FACTORY_CD
                                        FOR
                                        XML PATH('')
                                        ) 
                        SET @ReCol = SUBSTRING(@ReCol, 0, LEN(@ReCol))
                        PRINT '@ReCol'
                        PRINT @ReCol
                         
                        IF @CUT_TYPE<>'' 
                            BEGIN    
                        IF OBJECT_ID('tempdb..##TEMP_RECUT_QTY') IS NOT NULL
                        BEGIN
                            DROP TABLE ##TEMP_RECUT_QTY
                        END;
							
                        SET @SQL = 'SELECT * INTO ##TEMP_RECUT_QTY FROM (SELECT  CASE C.SIZE_CD2 WHEN '''
                        + '-'
                        + ''' THEN C.SIZE_CD ELSE C.SIZE_CD + '' '' + C.SIZE_CD2 END AS SIZE_CD ,b.COLOR_CD ,'''
                        + 'RE-Cut'
                        + '''+ CAST(A.REPAIR_CUT_NUM AS CHAR(4)) AS CUT_TYPE ,SUM(D.QTY) AS ReCut_QTY  --SUM(c.RATIO * b.PLYS) AS CUT_QTY
                        FROM    dbo.CUT_LAY AS a WITH ( NOLOCK ) INNER JOIN dbo.CUT_LAY_HD c WITH ( NOLOCK ) ON c.LAY_ID = A.LAY_ID
                        INNER JOIN dbo.CUT_LAY_DT AS b WITH ( NOLOCK ) ON c.LAY_TRANS_ID = b.LAY_TRANS_ID INNER JOIN CUT_BUNDLE_HD AS D ON D.LAY_DT_ID = B.LAY_DT_ID
                        WHERE   ( a.JOB_ORDER_NO = ''' + @JOB_ORDER_NO
                        + ''' ) AND ( a.FACTORY_CD = ''' + @FACTORY_CD
                        + ''' ) AND ( a.PRINT_STATUS = ''' + 'Y' + ''' ) AND ( D.TRX_TYPE = '''
                        + 'NM' + ''' ) AND ( A.CUT_TYPE = ''' + 'R'
                        + ''' ) GROUP BY b.COLOR_CD ,CASE C.SIZE_CD2 WHEN ''' + '-'
                        + ''' THEN C.SIZE_CD ELSE C.SIZE_CD + '' '' + C.SIZE_CD2 END ,A.CUT_TYPE ,A.REPAIR_CUT_NUM) AS P PIVOT (SUM(ReCut_QTY) FOR P.CUT_TYPE IN ('
                        + @CUT_TYPE + ')) AS PVT'
        
                        PRINT @SQL
                        EXEC(@SQl)    
                        END
     
                        IF @ReCol <> ''
                        BEGIN
                            SET @ReCol = ',' + @ReCol
                        
                        IF OBJECT_ID('tempdb..##TEMP_20140509') IS NOT NULL
                        BEGIN
                            DROP TABLE ##TEMP_20140509
                        END;
    
                        SET @SQL = ' SELECT A.SIZE_CD,A.COLOR_CD,A.ORDER_QTY AS [Order-Qty],A.CUT_QTY AS [Cut-Qty],A.SSample_QTY AS [SSample-Qty],A.CSample_QTY AS [Csample-Qty]'
                        + @ReCol
                        + ',A.CUT_REDUCE AS [Cut-Reduce],A.ACTUAL_QTY AS [Actual-Qty],A.OVER_SHORTAGE_PERCENT AS [Over/Shortage Percent],A.OVER_SHORTAGE_QTY AS [Over/Shortage Qty],A.CUT_DISCREPANCY_QTY AS [Cut2DiscrepancyQty],A.EMB_DISCREPANCY_QTY AS [EmbDiscrepancyQty],A.PRINT_DISCREPANCY_QTY AS [PrintDiscrepancyQty],A.         Final_Qty AS [FinalQty] '
                        + ' INTO ##TEMP_20140509 FROM #CUT_TEMP AS A LEFT  JOIN ##TEMP_RECUT_QTY AS B ON A.SIZE_CD=B.SIZE_CD AND A.COLOR_CD=B.COLOR_CD '
                        PRINT @SQL
                        EXEC(@SQL) 
                        END
                        ELSE
                        BEGIN
                        
                        
                        IF OBJECT_ID('tempdb..##TEMP_20140509') IS NOT NULL
                        BEGIN
                            DROP TABLE ##TEMP_20140509
                        END;
    
                        SET @SQL = ' SELECT A.SIZE_CD,A.COLOR_CD,A.ORDER_QTY AS [Order-Qty],A.CUT_QTY AS [Cut-Qty],A.SSample_QTY AS [SSample-Qty],A.CSample_QTY AS [Csample-Qty]
                        ,A.CUT_REDUCE AS [Cut-Reduce],A.ACTUAL_QTY AS [Actual-Qty],A.OVER_SHORTAGE_PERCENT AS [Over/Shortage Percent],A.OVER_SHORTAGE_QTY AS [Over/Shortage Qty],A.CUT_DISCREPANCY_QTY AS [Cut2DiscrepancyQty],A.EMB_DISCREPANCY_QTY AS [EmbDiscrepancyQty],A.PRINT_DISCREPANCY_QTY AS [PrintDiscrepancyQty],A.         Final_Qty AS [FinalQty] '
                        + ' INTO ##TEMP_20140509 FROM #CUT_TEMP AS A  '
                        PRINT @SQL
                        EXEC(@SQL) 
                        END         
                    ------------------------------------------------------------------------------------------------------------------------------------------------------------
 
                        --ORDER_QTY

                        IF OBJECT_ID('tempdb..#TEMP_JO_DT') IS NOT NULL
                        BEGIN
                            DROP TABLE #TEMP_JO_DT
                        END;
                                
                        SELECT DISTINCT
                            JO_NO , SIZE_CODE1,
                            CASE SIZE_CODE2
                                WHEN '-' THEN SIZE_CODE1
                                ELSE SIZE_CODE1 + ' ' + SIZE_CODE2
                            END AS SIZE_CD ,
                            COLOR_CODE AS COLOR_CD ,
                            QTY AS ORDER_QTY
                        INTO   #TEMP_JO_DT
                        FROM   dbo.JO_DT AS c WITH ( NOLOCK )
                        WHERE  ( JO_NO = @JOB_ORDER_NO )  




                        IF OBJECT_ID('tempdb..#TEMP_SIZE_CD_COL') IS NOT NULL
                        BEGIN
                            DROP TABLE #TEMP_SIZE_CD_COL
                        END;

                        SELECT  DISTINCT
                            SIZE_CD ,
                            SEQUENCE
                        INTO   #TEMP_SIZE_CD_COL
                        FROM   #TEMP_JO_DT A
                            INNER JOIN JO_HD AS B ON A.JO_NO = B.JO_NO
                            INNER JOIN SC_SIZE AS C ON B.SC_NO = C.SC_NO
                                                        AND A.SIZE_CODE1 = C.SIZE_CODE
                        ORDER BY C.SEQUENCE 
                                
                        SELECT @SIZ_CD_COL = ( SELECT  '['+SIZE_CD + '],'
                                            FROM    #TEMP_SIZE_CD_COL
                                            ORDER BY SEQUENCE
                                            FOR
                                            XML PATH('')
                                            ) 
                        SET @SIZ_CD_COL = SUBSTRING(@SIZ_CD_COL, 0, LEN(@SIZ_CD_COL))
                        PRINT @SIZ_CD_COL
 
  
                        SELECT @SIZ_CD_COL_ADD = ( SELECT  'ISNULL([' + SIZE_CD + '],0)' + '+'
                                                FROM    #TEMP_SIZE_CD_COL
                                                ORDER BY SEQUENCE
                                                FOR
                                                XML PATH('')
                                                ) 
                        SET @SIZ_CD_COL_ADD = SUBSTRING(@SIZ_CD_COL_ADD, 0, LEN(@SIZ_CD_COL_ADD))
                        PRINT @SIZ_CD_COL_ADD
 
 

 
                        --Order-Qty
                        IF OBJECT_ID('tempdb..##TEMP_FINAL_DATA') IS NOT NULL
                        BEGIN
                            DROP TABLE ##TEMP_FINAL_DATA
                        END;
 
                        SET @SQL = ' SELECT * ,' + @SIZ_CD_COL_ADD + ' AS Total ,'''
                        + 'Order-Qty                  ' + ''' AS Remark,' + @SEQ
                        + 'AS SEQ INTO ##TEMP_FINAL_DATA FROM   (SELECT SIZE_CD,COLOR_CD,ORDER_QTY FROM #TEMP_JO_DT ) AS P PIVOT( SUM(ORDER_QTY) FOR SIZE_CD IN ( '
                        + @SIZ_CD_COL + ' ) )  AS PVI  '
 
                        PRINT @SQL
 
                        EXEC(@SQL)
 
                        --Add checking if no data in ##TEMP_FINAL_DATA, then return to avoid runtime error
						IF OBJECT_ID('tempdb..##TEMP_FINAL_DATA') IS NULL
                        BEGIN
                            RETURN;
                        END;
 
                        SELECT @TEMP_TABLE_COL = '[COLOR_CD] NVARCHAR(30),'+( SELECT  '['+name + '] NVARCHAR(30),'
                                                FROM    tempdb.dbo.syscolumns A
                                                INNER JOIN #TEMP_SIZE_CD_COL B ON A.name = B.SIZE_CD COLLATE DATABASE_DEFAULT 
                                                WHERE   id = ( SELECT   MAX(id)
                                                                FROM     tempdb.dbo.sysobjects
                                                                WHERE    xtype = 'u'
                                                                        AND name = '##TEMP_FINAL_DATA'
                                                                )
                                                AND name NOT IN ('COLOR_CD', 'Total', 'Remark', 'SEQ')
                                                ORDER BY B.SEQUENCE
                                                FOR
                                                XML PATH('')
                                                ) + '[Total] NVARCHAR(30), [Remark] NVARCHAR(30), [SEQ] NVARCHAR(30)'                 
                        
     
                        PRINT @TEMP_TABLE_COL     
                        IF OBJECT_ID('tempdb..##TEMP_FINAL_RESULT') IS NOT NULL
                        BEGIN
                            DROP TABLE ##TEMP_FINAL_RESULT
                        END;        
                        SET @SQL = 'CREATE TABLE ##TEMP_FINAL_RESULT (' + @TEMP_TABLE_COL + ')'

                        EXEC(@SQL)
 
                        INSERT INTO ##TEMP_FINAL_RESULT
                            SELECT  *
                            FROM    ##TEMP_FINAL_DATA

 
                        -------------------------------------------------------------------------------------
                        -------------------------------------------------------------------------------------
                        DECLARE CUR_COL CURSOR
                        FOR
                        SELECT  name
                        FROM    tempdb.dbo.syscolumns
                        WHERE   id = ( SELECT   MAX(id)
                                        FROM     tempdb.dbo.sysobjects
                                        WHERE    xtype = 'u'
                                                AND name = '##TEMP_20140509'
                                        )
                                AND name NOT IN ( 'SIZE_CD', 'COLOR_CD', 'ORDER-QTY' )
                        OPEN CUR_COL;
                        FETCH NEXT FROM CUR_COL INTO @COL_NAME;
                
                        WHILE @@fetch_status = 0
                        BEGIN
    
                            SET @SEQ = CAST(CAST(@SEQ AS INT) + 1 AS CHAR(4)) 
            
                            SET @SQL = ' INSERT INTO ##TEMP_FINAL_RESULT   SELECT * ,'
                                + @SIZ_CD_COL_ADD + ' AS Total ,''' + @COL_NAME + ''' AS Remark,'
                                + @SEQ + ' AS SEQ
                                                FROM   (SELECT SIZE_CD,COLOR_CD,[' + @COL_NAME
                                + '] FROM ##TEMP_20140509) AS P PIVOT( SUM([' + @COL_NAME
                                + ']) FOR SIZE_CD IN ( ' + @SIZ_CD_COL + ' ) )  AS PVI  '
 
                            PRINT @SQL
                            EXEC(@SQL)    
   
                            FETCH NEXT FROM CUR_COL INTO @COL_NAME;  
                        END
                        CLOSE CUR_COL;
                        DEALLOCATE CUR_COL;
                        -------------------------------------------------------------------------------------
                        -------------------------------------------------------------------------------------
                        SELECT @SIZ_CD_COL = ( SELECT  '['+SIZE_CD + ']=[' + SIZE_CD + ']+''' + '%' + ''','
                                            FROM    #TEMP_SIZE_CD_COL
                                            FOR
                                            XML PATH('')
                                            ) 
                        SET @SIZ_CD_COL = SUBSTRING(@SIZ_CD_COL, 0, LEN(@SIZ_CD_COL))
                        PRINT @SIZ_CD_COL   
 
                        DELETE FROM ##TEMP_FINAL_RESULT
                        WHERE  TOTAL = '0'
                            AND ( Remark = 'SSample-Qty'
                                    OR Remark = 'Csample-Qty'
                                    OR Remark LIKE 'Re-Cut%'
                                ) 
    
                                 
                        SET @SQL = 'UPDATE ##TEMP_FINAL_RESULT SET ' + @SIZ_CD_COL
                        + 'WHERE Remark=''' + 'Over/Shortage Percent' + ''''
                        EXEC(@SQL)        


                        UPDATE ##TEMP_FINAL_RESULT
                        SET    Total = B.OVER_SHORTAGE_PERCENT
                        FROM   ##TEMP_FINAL_RESULT AS A
                            INNER JOIN ( SELECT A.COLOR_CODE ,
                                            CASE WHEN A.total_qty = 0 THEN '' ELSE CAST (CONVERT(DECIMAL(18, 3), ROUND(( ISNULL(B.Actual_QTY,
                                                                                    0)
                                                                                    - ISNULL(C.CUT_REDUCE,
                                                                                    0)
                                                                                    - ISNULL(A.total_qty,
                                                                                    0) ) * 100
                                                                                    / A.total_qty, 3)) AS NVARCHAR(10))
                                                + '%' END AS OVER_SHORTAGE_PERCENT
                                            FROM   ( SELECT    JO_NO ,
                                                            COLOR_CODE ,
                                                            SUM(QTY) AS TOTAL_QTY
                                                    FROM      JO_DT
                                                    GROUP BY  JO_NO ,
                                                            COLOR_CODE
                                                ) AS A
                                                LEFT JOIN ( SELECT  JOB_ORDER_NO ,
                                                                    COLOR_CD ,
                                                                    SUM(QTY) AS Actual_QTY
                                                            FROM    dbo.CUT_BUNDLE_HD
                                                            WHERE   JOB_ORDER_NO = @JOB_ORDER_NO
                                                                    AND FACTORY_CD = @FACTORY_CD
                                                                    AND STATUS = 'Y'
                                                                    -- Add filter TRX_TYPE='NM' to correct the calculation for Over/Shortage Percent
                                                                    AND TRX_TYPE = 'NM'
                                                            GROUP BY JOB_ORDER_NO ,
                                                                    COLOR_CD
                                                            ) AS B ON A.JO_NO = B.JOB_ORDER_NO
                                                                    AND A.COLOR_CODE = B.COLOR_CD
                                                LEFT JOIN ( SELECT  JOB_ORDER_NO ,
                                                                    COLOR_CD ,
                                                                    ABS(SUM(QTY)) AS CUT_REDUCE
                                                            FROM    CUT_BUNDLE_HD WITH ( NOLOCK )
                                                            WHERE   JOB_ORDER_NO = @JOB_ORDER_NO
                                                                    AND FACTORY_CD = @FACTORY_CD
                                                                    AND TRX_TYPE = 'RD'
                                                                    AND STATUS = 'Y'
                                                            GROUP BY JOB_ORDER_NO ,
                                                                    COLOR_CD
                                                            ) AS C ON A.JO_NO = C.JOB_ORDER_NO
                                                                    AND A.COLOR_CODE = C.COLOR_CD
                                            WHERE  JO_NO = @JOB_ORDER_NO
                                        ) AS B ON A.COLOR_CD COLLATE DATABASE_DEFAULT = B.COLOR_CODE
                        WHERE  Remark COLLATE DATABASE_DEFAULT = 'Over/Shortage Percent'    
        
                        SELECT *
                        FROM   ##TEMP_FINAL_RESULT
                        ORDER BY COLOR_CD ,
                            CAST(SEQ AS INT) ";

            return DBUtility.GetTable(SQL, "MES");
        }



        public static DataTable GetReduceQuantityReCut(String JoNo, String FactoryCd)
        {
            string SQL = @"
                            DECLARE @CUT_TYPE NVARCHAR(40)
                            DECLARE @SQL VARCHAR(MAX)
                            DECLARE @JOB_ORDER_NO NVARCHAR(30)
                            DECLARE @FACTORY_CD NVARCHAR(3)

                            SET @JOB_ORDER_NO='" + JoNo + @"'
                            SET @FACTORY_CD='" + FactoryCd + @"'
                            SELECT @CUT_TYPE=(
                            SELECT  DISTINCT '['+CUT_TYPE + LTRIM(RTRIM(CAST(REPAIR_CUT_NUM AS CHAR(4))))+']'+','
                            FROM CUT_LAY WHERE job_order_no=@JOB_ORDER_NO AND CUT_TYPE='R' AND FACTORY_CD=@FACTORY_CD
                             FOR XML PATH('')) 
                            SET @CUT_TYPE=SUBSTRING(@CUT_TYPE,0,LEN(@CUT_TYPE))
                            PRINT @CUT_TYPE

                            SET @SQL='
                            SELECT * FROM (
                            SELECT  CASE C.SIZE_CD2
                                      WHEN '''+'-'+''' THEN C.SIZE_CD
                                      ELSE C.SIZE_CD + '' '' + C.SIZE_CD2
                                    END AS SIZE_CD ,
                                    b.COLOR_CD ,
                                    A.CUT_TYPE + CAST(A.REPAIR_CUT_NUM AS CHAR(4)) AS CUT_TYPE ,
                                    SUM(D.QTY) AS ReCut_QTY  --SUM(c.RATIO * b.PLYS) AS CUT_QTY
                            FROM    dbo.CUT_LAY AS a WITH ( NOLOCK )
                                    INNER JOIN dbo.CUT_LAY_HD c WITH ( NOLOCK ) ON c.LAY_ID = A.LAY_ID
                                    INNER JOIN dbo.CUT_LAY_DT AS b WITH ( NOLOCK ) ON c.LAY_TRANS_ID = b.LAY_TRANS_ID
                                    INNER JOIN CUT_BUNDLE_HD AS D ON D.LAY_DT_ID = B.LAY_DT_ID
                            WHERE   ( a.JOB_ORDER_NO = '''+@JOB_ORDER_NO+''' )
                                    AND ( a.FACTORY_CD = '''+@FACTORY_CD+''' )
                                    AND ( a.PRINT_STATUS = '''+'Y'+''' )
                                    AND ( D.TRX_TYPE = '''+'NM'+''' )
                                    AND ( A.CUT_TYPE = '''+'R'+''' )
                            GROUP BY b.COLOR_CD ,
                                    CASE C.SIZE_CD2
                                      WHEN '''+'-'+''' THEN C.SIZE_CD
                                      ELSE C.SIZE_CD + '' '' + C.SIZE_CD2
                                    END ,
                                    A.CUT_TYPE ,
                                    A.REPAIR_CUT_NUM
                                    ) AS P
                                    PIVOT (SUM(ReCut_QTY) FOR P.CUT_TYPE IN ('+@CUT_TYPE+')) AS PVT'
        
                             PRINT @SQL
                             EXEC(@SQl) ";

            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetReduceQuantityByGO(string GoNo, string FactoryCd)
        {
            string SQL = @"     
                                 DECLARE @dtjo TABLE(
                                  JO_NO VARCHAR(20)
                                 )
                                 INSERT INTO @dtjo
                                 SELECT JO_NO FROM JO_HD WHERE SC_NO='" + GoNo + @"' AND isnull(CREATED_COMBINE_JO_FLAG,'N')='N' AND STATUS not in ('D','X')
                                 UNION
                                 SELECT JO_NO FROM JO_HD a WITH ( NOLOCK )
                                 inner JOIN dbo.JO_COMBINE_MAPPING b  WITH ( NOLOCK ) ON a.JO_NO=b.COMBINE_JO_NO
                                  WHERE SC_NO='" + GoNo + @"' AND STATUS not in ('D','X')
                            IF OBJECT_ID('TEMPDB..#TEMP') IS NOT NULL
                                DROP TABLE #TEMP;
                             SELECT C_1.JO_NO ,
                                    C_1.SIZE_CD ,
                                    C_1.COLOR_CD ,
                                    ISNULL(C_1.ORDER_QTY, 0) ORDER_QTY ,
                                    ISNULL(A_1.CUT_QTY, 0) CUT_QTY ,
                                    ISNULL(A_2.SSample_QTY, 0) AS SSample_QTY ,
                                    ISNULL(A_3.CSample_QTY, 0) AS CSample_QTY ,
                                    ISNULL(A_4.ReCut_QTY, 0) AS ReCut_QTY ,
                                    ISNULL(CUT_QTY, 0) + ISNULL(SSample_QTY, 0) + ISNULL(CSample_QTY,
                                                                                      0)
                                    + ISNULL(ReCut_QTY, 0) - ISNULL(CUT_REDUCE, 0) AS ACTUAL_QTY ,
                                    ISNULL(B_1.CUT_REDUCE, 0) AS CUT_REDUCE ,
                                    ISNULL(CUT_DISCREPANCY_QTY, 0) AS CUT_DISCREPANCY_QTY ,
                                    ISNULL(EMB_DISCREPANCY_QTY, 0) AS EMB_DISCREPANCY_QTY ,
                                    ISNULL(PRINT_DISCREPANCY_QTY, 0) AS PRINT_DISCREPANCY_QTY ,
                                    ISNULL(CUT_QTY, 0) + ISNULL(SSample_QTY, 0) + ISNULL(CSample_QTY,
                                                                                      0)
                                    + ISNULL(ReCut_QTY, 0) - ISNULL(CUT_REDUCE, 0)
                                    - ISNULL(CUT_DISCREPANCY_QTY, 0) - ISNULL(EMB_DISCREPANCY_QTY, 0)
                                    - ISNULL(PRINT_DISCREPANCY_QTY, 0) AS Final_Qty
                             INTO   #TEMP
                             FROM   ( SELECT DISTINCT
                                                JO_NO ,
                                                ( RTRIM(ISNULL(SIZE_CODE1, ''))
                                                  + ( CASE ISNULL(SIZE_CODE2, '')
                                                        WHEN '-' THEN ''
                                                        ELSE ' ' + ISNULL(SIZE_CODE2, '')
                                                      END ) ) AS SIZE_CD ,
                                                COLOR_CODE AS COLOR_CD ,
                                                QTY AS ORDER_QTY
                                      FROM      DBO.JO_DT AS C WITH ( NOLOCK )
                                      WHERE     ( JO_NO IN (  SELECT JO_NO
                                                             FROM   @dtjo ) )
                                    ) AS C_1
                                    LEFT JOIN ( SELECT  A.JOB_ORDER_NO ,
                                                        D.SIZE_CD ,
                                                        D.COLOR_CD ,
                                                        SUM(D.QTY) AS CUT_QTY
                                                FROM    DBO.CUT_LAY AS A WITH ( NOLOCK )
                                                        INNER JOIN DBO.CUT_LAY_HD C WITH ( NOLOCK ) ON C.LAY_ID = A.LAY_ID
                                                        INNER JOIN DBO.CUT_LAY_DT AS B WITH ( NOLOCK ) ON C.LAY_TRANS_ID = B.LAY_TRANS_ID
                                                        INNER JOIN CUT_BUNDLE_HD AS D WITH ( NOLOCK ) ON D.LAY_DT_ID = B.LAY_DT_ID
                                                WHERE   ( A.JOB_ORDER_NO IN (
                                                         SELECT JO_NO
                                                             FROM   @dtjo ) )
                                                        AND ( A.FACTORY_CD = '" + FactoryCd+ @"' )
                                                        AND ( A.PRINT_STATUS = 'Y' )
                                                        AND ( D.TRX_TYPE = 'NM' )
                                                        AND ( A.CUT_TYPE = 'C'
                                                              OR A.CUT_TYPE IS NULL
                                                              OR A.CUT_TYPE = ''
                                                            )
                                                        
                                                GROUP BY A.JOB_ORDER_NO ,
                                                        D.COLOR_CD ,
                                                        D.SIZE_CD
                                              ) AS A_1 ON A_1.SIZE_CD = C_1.SIZE_CD
                                                          AND A_1.COLOR_CD = C_1.COLOR_CD
                                                          AND A_1.JOB_ORDER_NO = C_1.JO_NO
                                    LEFT JOIN ( SELECT  A.JOB_ORDER_NO,CASE C.SIZE_CD2
                                                          WHEN '-' THEN C.SIZE_CD
                                                          ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                        END AS SIZE_CD ,
                                                        b.COLOR_CD ,
                                                        SUM(D.QTY) AS SSample_QTY
                                                FROM    dbo.CUT_LAY AS a WITH ( NOLOCK )
                                                        INNER JOIN dbo.CUT_LAY_HD c WITH ( NOLOCK ) ON c.LAY_ID = A.LAY_ID
                                                        INNER JOIN dbo.CUT_LAY_DT AS b WITH ( NOLOCK ) ON c.LAY_TRANS_ID = b.LAY_TRANS_ID
                                                        INNER JOIN CUT_BUNDLE_HD AS D ON D.LAY_DT_ID = B.LAY_DT_ID
                                                WHERE   ( A.JOB_ORDER_NO IN (
                                                         SELECT JO_NO
                                                             FROM   @dtjo ) )
                                                        AND ( a.FACTORY_CD = '" + FactoryCd+ @"' )
                                                        AND ( a.PRINT_STATUS = 'Y' )
                                                        AND ( D.TRX_TYPE = 'NM' )
                                                        AND ( A.CUT_TYPE = 'SS' )
                                                GROUP BY A.JOB_ORDER_NO,b.COLOR_CD ,
                                                        CASE C.SIZE_CD2
                                                          WHEN '-' THEN C.SIZE_CD
                                                          ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                        END
                                              ) AS A_2 ON A_2.SIZE_CD = C_1.SIZE_CD
                                                          AND A_2.COLOR_CD = C_1.COLOR_CD AND A_2.JOB_ORDER_NO=C_1.JO_NO
                                    LEFT JOIN ( SELECT  A.JOB_ORDER_NO,CASE C.SIZE_CD2
                                                          WHEN '-' THEN C.SIZE_CD
                                                          ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                        END AS SIZE_CD ,
                                                        b.COLOR_CD ,
                                                        SUM(D.QTY) AS CSample_QTY
                                                FROM    dbo.CUT_LAY AS a WITH ( NOLOCK )
                                                        INNER JOIN dbo.CUT_LAY_HD c WITH ( NOLOCK ) ON c.LAY_ID = A.LAY_ID
                                                        INNER JOIN dbo.CUT_LAY_DT AS b WITH ( NOLOCK ) ON c.LAY_TRANS_ID = b.LAY_TRANS_ID
                                                        INNER JOIN CUT_BUNDLE_HD AS D ON D.LAY_DT_ID = B.LAY_DT_ID
                                                WHERE   A.JOB_ORDER_NO IN (
                                                       SELECT JO_NO
                                                             FROM   @dtjo )
                                                        AND ( a.FACTORY_CD = '" + FactoryCd+ @"' )
                                                        AND ( a.PRINT_STATUS = 'Y' )
                                                        AND ( D.TRX_TYPE = 'NM' )
                                                        AND ( A.CUT_TYPE = 'CS' )
                                                GROUP BY A.JOB_ORDER_NO,b.COLOR_CD ,
                                                        CASE C.SIZE_CD2
                                                          WHEN '-' THEN C.SIZE_CD
                                                          ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                        END
                                              ) AS A_3 ON A_3.SIZE_CD = C_1.SIZE_CD
                                                          AND A_3.COLOR_CD = C_1.COLOR_CD AND A_3.JOB_ORDER_NO=C_1.JO_NO
                                    LEFT JOIN ( SELECT  A.JOB_ORDER_NO,CASE C.SIZE_CD2
                                                          WHEN '-' THEN C.SIZE_CD
                                                          ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                        END AS SIZE_CD ,
                                                        b.COLOR_CD ,
                                                        SUM(D.QTY) AS ReCut_QTY
                                                FROM    dbo.CUT_LAY AS a WITH ( NOLOCK )
                                                        INNER JOIN dbo.CUT_LAY_HD c WITH ( NOLOCK ) ON c.LAY_ID = A.LAY_ID
                                                        INNER JOIN dbo.CUT_LAY_DT AS b WITH ( NOLOCK ) ON c.LAY_TRANS_ID = b.LAY_TRANS_ID
                                                        INNER JOIN CUT_BUNDLE_HD AS D ON D.LAY_DT_ID = B.LAY_DT_ID
                                                WHERE   A.JOB_ORDER_NO IN (
                                                        SELECT JO_NO
                                                             FROM   @dtjo )
                                                        AND ( a.FACTORY_CD = '" + FactoryCd+ @"' )
                                                        AND ( a.PRINT_STATUS = 'Y' )
                                                        AND ( D.TRX_TYPE = 'NM' )
                                                        AND ( A.CUT_TYPE = 'R' )
                                                GROUP BY A.JOB_ORDER_NO,b.COLOR_CD ,
                                                        CASE C.SIZE_CD2
                                                          WHEN '-' THEN C.SIZE_CD
                                                          ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                        END
                                              ) AS A_4 ON A_4.SIZE_CD = C_1.SIZE_CD
                                                          AND A_4.COLOR_CD = C_1.COLOR_CD AND A_4.JOB_ORDER_NO=C_1.JO_NO
                                    LEFT JOIN ( SELECT  JOB_ORDER_NO,SIZE_CD ,
                                                        COLOR_CD ,
                                                        ABS(SUM(QTY)) AS CUT_REDUCE
                                                FROM    CUT_BUNDLE_HD WITH ( NOLOCK )
                                                WHERE   JOB_ORDER_NO IN (
                                                       SELECT JO_NO
                                                             FROM   @dtjo )
                                                        AND FACTORY_CD = '" + FactoryCd+ @"'
                                                        AND TRX_TYPE = 'RD'
                                                        AND STATUS = 'Y'
                                                GROUP BY JOB_ORDER_NO,COLOR_CD ,
                                                        SIZE_CD
                                              ) AS B_1 ON B_1.SIZE_CD = C_1.SIZE_CD
                                                          AND B_1.COLOR_CD = C_1.COLOR_CD AND B_1.JOB_ORDER_NO=C_1.JO_NO
                                    LEFT JOIN ( SELECT  JOB_ORDER_NO,SIZE_CODE AS SIZE_CD ,
                                                        COLOR_CODE AS COLOR_CD ,
                                                        SUM(DISCREPANCY_QTY) AS CUT_DISCREPANCY_QTY ,
                                                        NULL AS EMB_DISCREPANCY_QTY ,
                                                        NULL AS PRINT_DISCREPANCY_QTY
                                                FROM    dbo.PRD_JO_DISCREPANCY_PULLOUT_HD AS A
                                                        INNER JOIN dbo.PRD_JO_DISCREPANCY_PULLOUT_TRX
                                                        AS B ON A.DOC_NO = B.DOC_NO
                                                WHERE   JOB_ORDER_NO IN (
                                                       SELECT JO_NO
                                                             FROM   @dtjo )
                                                        AND PROCESS_CD = 'CUT'
                                                GROUP BY JOB_ORDER_NO,SIZE_CODE ,
                                                        COLOR_CODE
                                                UNION ALL
                                                SELECT  JOB_ORDER_NO,SIZE_CODE AS SIZE_CD ,
                                                        COLOR_CODE AS COLOR_CD ,
                                                        NULL AS CUT_DISCREPANCY_QTY ,
                                                        SUM(DISCREPANCY_QTY) AS EMB_DISCREPANCY_QTY ,
                                                        NULL AS PRINT_DISCREPANCY_QTY
                                                FROM    dbo.PRD_JO_DISCREPANCY_PULLOUT_HD AS A
                                                        INNER JOIN dbo.PRD_JO_DISCREPANCY_PULLOUT_TRX
                                                        AS B ON A.DOC_NO = B.DOC_NO
                                                WHERE   JOB_ORDER_NO IN (
                                                       SELECT JO_NO
                                                             FROM   @dtjo )
                                                        AND PROCESS_CD = 'EMB'
                                                GROUP BY JOB_ORDER_NO,SIZE_CODE ,
                                                        COLOR_CODE
                                                UNION ALL
                                                SELECT  JOB_ORDER_NO,SIZE_CODE AS SIZE_CD ,
                                                        COLOR_CODE AS COLOR_CD ,
                                                        NULL AS CUT_DISCREPANCY_QTY ,
                                                        NULL AS EMB_DISCREPANCY_QTY ,
                                                        SUM(DISCREPANCY_QTY) AS PRINT_DISCREPANCY_QTY
                                                FROM    dbo.PRD_JO_DISCREPANCY_PULLOUT_HD AS A
                                                        INNER JOIN dbo.PRD_JO_DISCREPANCY_PULLOUT_TRX
                                                        AS B ON A.DOC_NO = B.DOC_NO
                                                WHERE   JOB_ORDER_NO IN (
                                                       SELECT JO_NO
                                                             FROM   @dtjo )
                                                        AND PROCESS_CD = 'PRT'
                                                GROUP BY JOB_ORDER_NO,SIZE_CODE ,
                                                        COLOR_CODE
                                              ) AS D ON C_1.SIZE_CD = D.SIZE_CD
                                                        AND C_1.COLOR_CD = D.COLOR_CD AND D.JOB_ORDER_NO=C_1.JO_NO

                             SELECT DISTINCT
                                    COLOR_CODE ,
                                    COLOR_DESC ,
                                    SIZE_CD AS SIZE_CODE ,
                                    ORDER_QTY ,
                                    CUT_QTY ,
                                    SSample_QTY ,
                                    CSample_QTY ,
                                    CUT_REDUCE ,
                                    ReCut_QTY ,
                                    ACTUAL_QTY ,
                                    OVER_SHORTAGE_PERCENT ,
                                    CUT_DISCREPANCY_QTY ,
                                    EMB_DISCREPANCY_QTY ,
                                    PRINT_DISCREPANCY_QTY ,
                                    Final_Qty ,
                                    OVER_SHORTAGE_PERCENT_FINAL_QTY,
                                    COLOR_SEQ ,
                                    SIZE_SEQ
                             FROM   ( SELECT    COLOR_CD ,
                                                SIZE_CD ,
                                                SUM(ISNULL(ORDER_QTY, 0)) AS ORDER_QTY ,
                                                SUM(ISNULL(CUT_QTY, 0)) AS CUT_QTY ,
                                                SUM(ISNULL(SSample_QTY, 0)) AS SSample_QTY ,
                                                SUM(ISNULL(CSample_QTY, 0)) AS CSample_QTY ,
                                                SUM(ISNULL(ReCut_QTY, 0)) AS ReCut_QTY ,                                                
                                                SUM(ISNULL(CUT_REDUCE, 0)) AS CUT_REDUCE ,
                                                SUM(ISNULL(ACTUAL_QTY, 0)) AS ACTUAL_QTY ,  
                                                SUM(ISNULL(CUT_DISCREPANCY_QTY, 0)) AS CUT_DISCREPANCY_QTY ,
                                                SUM(ISNULL(EMB_DISCREPANCY_QTY, 0)) AS EMB_DISCREPANCY_QTY ,
                                                SUM(ISNULL(PRINT_DISCREPANCY_QTY, 0)) AS PRINT_DISCREPANCY_QTY ,
                                                SUM(ISNULL(Final_Qty, 0)) AS Final_Qty ,
                                                CASE WHEN SUM(ISNULL(ORDER_QTY, 0)) = 0
                                                          OR SUM(ISNULL(CUT_QTY, 0)) = 0 THEN NULL
                                                     ELSE CAST(ROUND(( SUM(CONVERT(DECIMAL(18, 3), ISNULL(ACTUAL_QTY,
                                                                                      0)))                                                                    
                                                                       - SUM(CONVERT(DECIMAL(18, 3), ISNULL(ORDER_QTY,
                                                                                      0))) ) * 100
                                                                     / SUM(CONVERT(DECIMAL(18, 3), ISNULL(ORDER_QTY,
                                                                                      0))), 1) AS DECIMAL(10,
                                                                                      3))
                                                END AS OVER_SHORTAGE_PERCENT,
                                                CASE WHEN SUM(ISNULL(ORDER_QTY, 0)) = 0
                                                          OR SUM(ISNULL(Final_Qty, 0)) = 0 THEN NULL
                                                     ELSE CAST(ROUND(( SUM(CONVERT(DECIMAL(18, 3), ISNULL(Final_Qty,
                                                                                      0)))                                                                      
                                                                       - SUM(CONVERT(DECIMAL(18, 3), ISNULL(ORDER_QTY,
                                                                                      0))) ) * 100
                                                                     / SUM(CONVERT(DECIMAL(18, 3), ISNULL(ORDER_QTY,
                                                                                      0))), 1) AS DECIMAL(10,
                                                                                      3))
                                                END AS OVER_SHORTAGE_PERCENT_FINAL_QTY
                                      FROM      #TEMP
                                      GROUP BY  SIZE_CD ,
                                                COLOR_CD
                                      UNION ALL
                                      SELECT    'TOTAL' AS COLOR_CD ,
                                                SIZE_CD ,
                                                SUM(ISNULL(ORDER_QTY, 0)) AS ORDER_QTY ,
                                                SUM(ISNULL(CUT_QTY, 0)) AS CUT_QTY ,
                                                SUM(ISNULL(SSample_QTY, 0)) AS SSample_QTY ,
                                                SUM(ISNULL(CSample_QTY, 0)) AS CSample_QTY ,
                                                SUM(ISNULL(ReCut_QTY, 0)) AS ReCut_QTY ,
                                                SUM(ISNULL(CUT_REDUCE, 0)) AS CUT_REDUCE ,
                                                SUM(ISNULL(ACTUAL_QTY, 0)) AS ACTUAL_QTY ,
                                                SUM(ISNULL(CUT_DISCREPANCY_QTY, 0)) AS CUT_DISCREPANCY_QTY ,
                                                SUM(ISNULL(EMB_DISCREPANCY_QTY, 0)) AS EMB_DISCREPANCY_QTY ,
                                                SUM(ISNULL(PRINT_DISCREPANCY_QTY, 0)) AS PRINT_DISCREPANCY_QTY ,
                                                SUM(ISNULL(Final_Qty, 0)) AS Final_Qty ,
                                                CASE WHEN SUM(ISNULL(ORDER_QTY, 0)) = 0
                                                          OR SUM(ISNULL(CUT_QTY, 0)) = 0 THEN NULL
                                                     ELSE CAST(ROUND(( SUM(CONVERT(DECIMAL(18, 3), ISNULL(ACTUAL_QTY,
                                                                                      0)))
                                                                       - SUM(CONVERT(DECIMAL(18, 3), ISNULL(ORDER_QTY,
                                                                                      0))) ) * 100
                                                                     / SUM(CONVERT(DECIMAL(18, 3), ISNULL(ORDER_QTY,
                                                                                      0))), 1) AS DECIMAL(10,
                                                                                      3))
                                                END AS OVER_SHORTAGE_PERCENT ,
                                                CASE WHEN SUM(ISNULL(ORDER_QTY, 0)) = 0
                                                          OR SUM(ISNULL(Final_Qty, 0)) = 0 THEN NULL
                                                     ELSE CAST(ROUND(( SUM(CONVERT(DECIMAL(18, 3), ISNULL(Final_Qty,
                                                                                      0)))                                                                      
                                                                       - SUM(CONVERT(DECIMAL(18, 3), ISNULL(ORDER_QTY,
                                                                                      0))) ) * 100
                                                                     / SUM(CONVERT(DECIMAL(18, 3), ISNULL(ORDER_QTY,
                                                                                      0))), 1) AS DECIMAL(10,
                                                                                      3))
                                                END AS OVER_SHORTAGE_PERCENT_FINAL_QTY
                                      FROM      #TEMP
                                      GROUP BY  SIZE_CD
                                    ) AS A
                                    LEFT JOIN ( SELECT  SIZE_CODE ,
                                                        SEQUENCE AS SIZE_SEQ
                                                FROM    DBO.SC_SIZE WITH ( NOLOCK )
                                                WHERE   SC_NO = '" + GoNo+@"'
                                              ) B ON A.SIZE_CD = B.SIZE_CODE
                                    LEFT JOIN ( SELECT  COLOR_CODE ,
                                                        COLOR_DESC ,
                                                        SEQUENCE AS COLOR_SEQ
                                                FROM    SC_COLOR WITH ( NOLOCK )
                                                WHERE   SC_NO = '"+GoNo+@"'
                                                UNION ALL
                                                SELECT  'TOTAL' AS COLOR_CODE ,
                                                        NULL AS COLOR_DESC ,
                                                        '9999999' AS COLOR_SEQ
                                                FROM    SC_COLOR WITH ( NOLOCK )
                                                WHERE   SC_NO = '"+GoNo+@"'
                                              ) C ON A.COLOR_CD = C.COLOR_CODE
                             ORDER BY COLOR_SEQ ,
                                    SIZE_SEQ     ";   


            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetCanceledColor_Size_CUTQTY(string JoNoORGoNo, bool IsGO, string FactoryCD)
        {
            //Added By ZouShiChang ON 2013.09.10 Start 将CUT取数更改为新方式

            string SQL = "";
            SQL += @"    IF OBJECT_ID('TEMPDB..#TEMP_JO')IS NOT NULL DROP TABLE #TEMP_JO    ;
                                IF OBJECT_ID('TEMPDB..#TEMP_CUTQTY')IS NOT NULL DROP TABLE #TEMP_CUTQTY    ;";
            if (IsGO)
            {
                SQL += @"    SELECT DISTINCT JO_NO INTO #TEMP_JO FROM JO_HD WITH(NOLOCK) WHERE SC_NO = '" + JoNoORGoNo + "';  ";
            }
            else
            {
                SQL += @"    SELECT '" + JoNoORGoNo + "' AS JO_NO INTO #TEMP_JO";
            }
            SQL += @"    SELECT A.JOB_ORDER_NO,c.SIZE_CD + ' ' + c.SIZE_CD2 AS SIZE_CD,b.COLOR_CD,c.SIZE_CD + ' ' + c.SIZE_CD2+b.COLOR_CD AS SIZE_CD_COLOR_CD,
                                SUM(c.RATIO * b.PLYS) AS CUT_QTY  INTO #TEMP_CUTQTY
                                FROM dbo.CUT_LAY AS a    WITH(NOLOCK)  
                                INNER JOIN    dbo.CUT_LAY_HD c   WITH(NOLOCK)  ON c.LAY_ID=A.LAY_ID  
                                INNER JOIN  dbo.CUT_LAY_DT AS b   WITH(NOLOCK)  ON c.LAY_TRANS_ID = b.LAY_TRANS_ID 
                                WHERE EXISTS(SELECT 1 FROM #TEMP_JO WHERE JO_NO = a.JOB_ORDER_NO)
                                AND (a.FACTORY_CD = '" + FactoryCD + "') AND  (a.PRINT_STATUS = 'Y')";
            SQL += @"    AND EXISTS
	                                (SELECT 1 FROM CUT_BUNDLE_HD AA WITH(NOLOCK), JO_HD BB WITH(NOLOCK) ";
            if (IsGO)
            {
                SQL += @"    WHERE  AA.STATUS ='Y' AND AA.JOB_ORDER_NO =BB.JO_NO	AND BB.SC_NO ='" + JoNoORGoNo + "'	";
            }
            else
            {
                SQL += @"    WHERE  AA.STATUS ='Y' AND AA.JOB_ORDER_NO =BB.JO_NO	AND BB.SC_NO =(SELECT SC_NO FROM JO_HD WITH(NOLOCK) WHERE JO_NO = '" + JoNoORGoNo + "')	";
            }
            SQL += @"    AND AA.LAY_DT_ID = B.LAY_DT_ID";
            SQL += @"    GROUP BY LAY_DT_ID,BUNDLE_NO 
	                                HAVING SUM(QTY)<>0 )  
                                GROUP BY b.COLOR_CD,c.SIZE_CD + ' ' + c.SIZE_CD2,b.COLOR_CD, c.SIZE_CD + ' ' + c.SIZE_CD2,A.JOB_ORDER_NO;

                                SELECT A_1.SIZE_CD, A_1.COLOR_CD ,SUM(CUT_QTY ) AS CUT_QTY
                                FROM #TEMP_CUTQTY AS A_1
                                WHERE NOT EXISTS
                                (
                                SELECT * FROM (
	                                SELECT DISTINCT JO_NO,SIZE_CODE1 + ' ' + SIZE_CODE2 + COLOR_CODE AS SIZE_CD_COLOR_CD 
	                                FROM dbo.JO_DT AS C WHERE EXISTS (SELECT 1 FROM #TEMP_JO AA WHERE AA.JO_NO = C.JO_NO)
	                                ) AS C_1 WHERE A_1.JOB_ORDER_NO = C_1.JO_NO AND A_1.SIZE_CD_COLOR_CD = C_1.SIZE_CD_COLOR_CD
                                )
                                GROUP BY A_1.SIZE_CD, A_1.COLOR_CD";


            /*更改为新的取数方式后size1和size2同时存在的时候有问题
            string SQL = "";
            SQL += @"    IF OBJECT_ID('TEMPDB..#TEMP_JO')IS NOT NULL DROP TABLE #TEMP_JO    ;
                                IF OBJECT_ID('TEMPDB..#TEMP_CUTQTY')IS NOT NULL DROP TABLE #TEMP_CUTQTY    ;";
            if (IsGO)
            {
                SQL += @"    SELECT DISTINCT JO_NO INTO #TEMP_JO FROM JO_HD WITH(NOLOCK) WHERE SC_NO = '" + JoNoORGoNo + "';  ";
            }
            else
            {
                SQL += @"    SELECT '" + JoNoORGoNo + "' AS JO_NO INTO #TEMP_JO";
            }

            SQL += @" SELECT  JOB_ORDER_NO ,
                            SIZE_CD ,
                            COLOR_CD ,
                            SIZE_CD + COLOR_CD AS SIZE_CD_COLOR_CD ,
                            SUM(QTY) AS CUT_QTY
                    INTO #TEMP_CUTQTY        
                    FROM    dbo.CUT_BUNDLE_HD AS A
                    WHERE   EXISTS ( SELECT 1
                                     FROM   #TEMP_JO
                                     WHERE  JO_NO = a.JOB_ORDER_NO )
                                    AND FACTORY_CD='" + FactoryCD + @"'
                    GROUP BY COLOR_CD ,
                            SIZE_CD ,
                            COLOR_CD ,
                            SIZE_CD ,
                            JOB_ORDER_NO ;";

            SQL += @"             SELECT A_1.SIZE_CD, A_1.COLOR_CD ,SUM(CUT_QTY ) AS CUT_QTY
                                FROM #TEMP_CUTQTY AS A_1
                                WHERE NOT EXISTS
                                (
                                SELECT * FROM (
                                    SELECT DISTINCT JO_NO,
                                                     CASE SIZE_CODE2
                                                     WHEN '-'
                                                     THEN SIZE_CODE1 + COLOR_CODE
                                                     ELSE SIZE_CODE1 + SIZE_CODE2
                                                           + COLOR_CODE
                                                     END AS SIZE_CD_COLOR_CD
                                    FROM dbo.JO_DT AS C WHERE EXISTS (SELECT 1 FROM #TEMP_JO AA WHERE AA.JO_NO = C.JO_NO)
                                    ) AS C_1 WHERE A_1.JOB_ORDER_NO = C_1.JO_NO AND A_1.SIZE_CD_COLOR_CD = C_1.SIZE_CD_COLOR_CD
                                )
                                GROUP BY A_1.SIZE_CD, A_1.COLOR_CD";
            */
            //Added By ZouShiChang ON 2013.09.10 End 将CUT取数更改为新方式
            return DBUtility.GetTable(SQL, "MES");
        }

    }
}