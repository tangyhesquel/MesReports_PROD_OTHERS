using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;

/// <summary>
///ShipmentClosingSQL 的摘要说明
/// </summary>
/// 
namespace MESComment
{
    public class ShipmentClosingSQL
    {
        public ShipmentClosingSQL()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        public static DataTable GetShipmentClosingGO(string SC_NO, string Site, string Size, string Size_Null, string Size_EMPTY)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@" 
                               DECLARE @SC_NO NVARCHAR(50) ,
                                    @FACTORY_CD NVARCHAR(50)
                                DECLARE @SIZ_CD_COL_ADD NVARCHAR(500) ,
                                    @TABLE_EMPTY NVARCHAR(500)
                                DECLARE @SQL NVARCHAR(2000)
                                DECLARE @TABLE_COL NVARCHAR(500) ,
                                    @COL_NAME NVARCHAR(30) 
                                SET @SC_NO = '{0}'
                                SET @FACTORY_CD = '{1}'
                                SET @TABLE_COL = '{2}'
                                SET @SIZ_CD_COL_ADD = '{3}'
                                SET @TABLE_EMPTY = '{4}'    

                                DECLARE @SEQ INT 
                                SET @SEQ = 1

                                IF OBJECT_ID('TEMPDB..#TEMP_GO') IS NOT NULL 
                                    DROP TABLE #TEMP_GO ;
                                SELECT DISTINCT  C_1.JO_NO ,--
                                        C_1.SIZE_CD ,--
                                        C_1.COLOR_CD ,--
                                        ISNULL(C_1.ORDER_QTY, 0) ORDER_QTY ,--
                                        ISNULL(A_1.CUT_QTY, 0) CUT_QTY ,--
                                        ISNULL(A_2.SSample_QTY, 0) AS SSample_QTY ,
                                        ISNULL(A_3.CSample_QTY, 0) AS CSample_QTY ,
                                        ISNULL(A_4.ReCut_QTY, 0) AS ReCut_QTY ,
                                        ISNULL(CUT_QTY, 0) + ISNULL(SSample_QTY, 0) + ISNULL(CSample_QTY, 0)
                                        + ISNULL(ReCut_QTY, 0) - ISNULL(CUT_REDUCE, 0) AS ACTUAL_QTY ,--
                                        ISNULL(B_1.CUT_REDUCE, 0) AS CUT_REDUCE ,
                                        ISNULL(E_1.Leftover_Garment, 0) Leftover_Garment ,
                                        ISNULL(E_2.B_Grade_QTY, 0) B_Grade_Qty ,
                                        ISNULL(E_3.C_Grade_QTY, 0) C_Grade_Qty ,
                                        ISNULL(E_4.PULL_IN_QTY, 0) PULL_IN_QTY ,
                                        ISNULL(E_4.PULL_OUT_QTY, 0) PULL_OUT_QTY
                                INTO    #TEMP_GO
                                FROM    ( SELECT DISTINCT
                                                    JO_NO ,
                                                    ( RTRIM(ISNULL(SIZE_CODE1, ''))
                                                      + ( CASE ISNULL(SIZE_CODE2, '')
                                                            WHEN '-' THEN ''
                                                            ELSE ' ' + ISNULL(SIZE_CODE2, '')
                                                          END ) ) AS SIZE_CD ,
                                                    COLOR_CODE AS COLOR_CD ,
                                                    QTY AS ORDER_QTY
                                          FROM      DBO.JO_DT AS C WITH ( NOLOCK )
                                          WHERE     ( JO_NO IN ( SELECT JO_NO
                                                                 FROM   JO_HD WITH ( NOLOCK )
                                                                 WHERE  STATUS NOT IN ( 'D', 'X' )
                                                                        AND SC_NO = @SC_NO ) and not exists(select  1 from JO_COMBINE_MAPPING where ORIGINAL_JO_NO=JO_NO))
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
                                                              SELECT    JO_NO
                                                              FROM      JO_HD WITH ( NOLOCK )
                                                              WHERE     STATUS NOT IN ( 'D', 'X' )
                                                                        AND SC_NO = @SC_NO ) )
                                                            AND ( A.FACTORY_CD = @FACTORY_CD )
                                                            AND ( A.PRINT_STATUS = 'Y' )
                                                            AND ( D.TRX_TYPE = 'NM' )  --Added by MF on 20151105, filter the trx_type for cut_qty
                                                            AND ( A.CUT_TYPE = 'C'
                                                                  OR A.CUT_TYPE IS NULL
                                                                  OR A.CUT_TYPE = ''
                                                                )
                                                            AND EXISTS ( SELECT 1
                                                                         FROM   CUT_BUNDLE_HD AA WITH ( NOLOCK ) ,
                                                                                JO_HD BB WITH ( NOLOCK )
                                                                         WHERE  AA.STATUS = 'Y'
                                                                                AND AA.JOB_ORDER_NO = BB.JO_NO
                                                                                AND BB.SC_NO = @SC_NO
                                                                                AND AA.TRX_TYPE = 'NM' --Added by MF on 20151105, filter the trx_type for cut_qty
                                                                                AND AA.LAY_DT_ID = B.LAY_DT_ID
                                                                         GROUP BY LAY_DT_ID ,
                                                                                BUNDLE_NO
                                                                         HAVING SUM(QTY) <> 0 )
                                                    GROUP BY A.JOB_ORDER_NO ,
                                                            D.COLOR_CD ,
                                                            D.SIZE_CD
                                                  ) AS A_1 ON A_1.SIZE_CD = C_1.SIZE_CD
                                                              AND A_1.COLOR_CD = C_1.COLOR_CD
                                                              AND A_1.JOB_ORDER_NO = C_1.JO_NO
                                        LEFT JOIN ( SELECT  A.JOB_ORDER_NO ,
                                                            CASE C.SIZE_CD2
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
                                                              SELECT    JO_NO
                                                              FROM      JO_HD WITH ( NOLOCK )
                                                              WHERE     STATUS NOT IN ( 'D', 'X' )
                                                                        AND SC_NO = @SC_NO ) )
                                                            AND ( a.FACTORY_CD = @FACTORY_CD )
                                                            AND ( a.PRINT_STATUS = 'Y' )
                                                            AND ( D.TRX_TYPE = 'NM' )
                                                            AND ( A.CUT_TYPE = 'SS' )
                                                    GROUP BY A.JOB_ORDER_NO ,
                                                            b.COLOR_CD ,
                                                            CASE C.SIZE_CD2
                                                              WHEN '-' THEN C.SIZE_CD
                                                              ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                            END
                                                  ) AS A_2 ON A_2.SIZE_CD = C_1.SIZE_CD
                                                              AND A_2.COLOR_CD = C_1.COLOR_CD
                                                              AND A_2.JOB_ORDER_NO = C_1.JO_NO
                                        LEFT JOIN ( SELECT  A.JOB_ORDER_NO ,
                                                            CASE C.SIZE_CD2
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
                                                            SELECT  JO_NO
                                                            FROM    JO_HD WITH ( NOLOCK )
                                                            WHERE   STATUS NOT IN ( 'D', 'X' )
                                                                    AND SC_NO = @SC_NO )
                                                            AND ( a.FACTORY_CD = @FACTORY_CD )
                                                            AND ( a.PRINT_STATUS = 'Y' )
                                                            AND ( D.TRX_TYPE = 'NM' )
                                                            AND ( A.CUT_TYPE = 'CS' )
                                                    GROUP BY A.JOB_ORDER_NO ,
                                                            b.COLOR_CD ,
                                                            CASE C.SIZE_CD2
                                                              WHEN '-' THEN C.SIZE_CD
                                                              ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                            END
                                                  ) AS A_3 ON A_3.SIZE_CD = C_1.SIZE_CD
                                                              AND A_3.COLOR_CD = C_1.COLOR_CD
                                                              AND A_3.JOB_ORDER_NO = C_1.JO_NO
                                        LEFT JOIN ( SELECT  A.JOB_ORDER_NO ,
                                                            CASE C.SIZE_CD2
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
                                                            SELECT  JO_NO
                                                            FROM    JO_HD WITH ( NOLOCK )
                                                            WHERE   STATUS NOT IN ( 'D', 'X' )
                                                                    AND SC_NO = @SC_NO )
                                                            AND ( a.FACTORY_CD = @FACTORY_CD )
                                                            AND ( a.PRINT_STATUS = 'Y' )
                                                            AND ( D.TRX_TYPE = 'NM' )
                                                            AND ( A.CUT_TYPE = 'R' )
                                                    GROUP BY A.JOB_ORDER_NO ,
                                                            b.COLOR_CD ,
                                                            CASE C.SIZE_CD2
                                                              WHEN '-' THEN C.SIZE_CD
                                                              ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                                            END
                                                  ) AS A_4 ON A_4.SIZE_CD = C_1.SIZE_CD
                                                              AND A_4.COLOR_CD = C_1.COLOR_CD
                                                              AND A_4.JOB_ORDER_NO = C_1.JO_NO
                                        LEFT JOIN ( SELECT  JOB_ORDER_NO ,
                                                            SIZE_CD ,
                                                            COLOR_CD ,
                                                            ABS(SUM(QTY)) AS CUT_REDUCE
                                                    FROM    CUT_BUNDLE_HD WITH ( NOLOCK )
                                                    WHERE   JOB_ORDER_NO IN (
                                                            SELECT  JO_NO
                                                            FROM    JO_HD WITH ( NOLOCK )
                                                            WHERE   STATUS NOT IN ( 'D', 'X' )
                                                                    AND SC_NO = @SC_NO )
                                                            AND FACTORY_CD = @FACTORY_CD
                                                            AND TRX_TYPE = 'RD'
                                                            AND STATUS = 'Y'
                                                    GROUP BY JOB_ORDER_NO ,
                                                            COLOR_CD ,
                                                            SIZE_CD
                                                  ) AS B_1 ON B_1.SIZE_CD = C_1.SIZE_CD
                                                              AND B_1.COLOR_CD = C_1.COLOR_CD
                                                              AND B_1.JOB_ORDER_NO = C_1.JO_NO
                                    --Leftover Garment A
                                        LEFT JOIN ( SELECT  JOB_ORDER_NO ,
                                                            SIZE_CODE AS SIZE_CD ,
                                                            COLOR_CODE AS COLOR_CD ,
                                                            SUM(R.PULLOUT_QTY) Leftover_Garment
                                                    FROM    dbo.PRD_JO_PULLOUT_REASON R WITH ( NOLOCK )
                                                            INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_TRX T ON R.FACTORY_CD = @FACTORY_CD
                                                                                              AND R.FACTORY_CD = T.FACTORY_CD
                                                                                              AND R.TRX_ID = T.TRX_ID
                                                                                              AND R.GRADE_CD = 'A'
                                                    WHERE   T.JOB_ORDER_NO IN (
                                                            SELECT  JO_NO
                                                            FROM    JO_HD WITH ( NOLOCK )
                                                            WHERE   STATUS NOT IN ( 'D', 'X' )
                                                                    AND SC_NO = @SC_NO )
                                                    GROUP BY JOB_ORDER_NO ,
                                                            SIZE_CODE ,
                                                            COLOR_CODE
                                                  ) E_1 ON E_1.SIZE_CD = C_1.SIZE_CD
                                                           AND E_1.COLOR_CD = C_1.COLOR_CD
                                                           AND E_1.JOB_ORDER_NO = C_1.JO_NO
                                --B Grade Qty
                                        LEFT JOIN ( SELECT  JOB_ORDER_NO ,
                                                            SIZE_CODE AS SIZE_CD ,
                                                            COLOR_CODE AS COLOR_CD ,
                                                            SUM(R.PULLOUT_QTY) B_Grade_Qty
                                                    FROM    dbo.PRD_JO_PULLOUT_REASON R WITH ( NOLOCK )
                                                            INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_TRX T ON R.FACTORY_CD = @FACTORY_CD
                                                                                              AND R.FACTORY_CD = T.FACTORY_CD
                                                                                              AND R.TRX_ID = T.TRX_ID
                                                                                              AND R.GRADE_CD = 'B'
                                                    WHERE   T.JOB_ORDER_NO IN (
                                                            SELECT  JO_NO
                                                            FROM    JO_HD WITH ( NOLOCK )
                                                            WHERE   STATUS NOT IN ( 'D', 'X' )
                                                                    AND SC_NO = @SC_NO )
                                                    GROUP BY JOB_ORDER_NO ,
                                                            SIZE_CODE ,
                                                            COLOR_CODE
                                                  ) E_2 ON E_1.SIZE_CD = C_1.SIZE_CD
                                                           AND E_2.COLOR_CD = C_1.COLOR_CD
                                                           AND E_2.JOB_ORDER_NO = C_1.JO_NO
                                --C Grade Qty
                                        LEFT JOIN ( SELECT  JOB_ORDER_NO ,
                                                            SIZE_CODE AS SIZE_CD ,
                                                            COLOR_CODE AS COLOR_CD ,
                                                            SUM(R.PULLOUT_QTY) C_Grade_QTY
                                                    FROM    dbo.PRD_JO_PULLOUT_REASON R WITH ( NOLOCK )
                                                            INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_TRX T ON R.FACTORY_CD = @FACTORY_CD
                                                                                              AND R.FACTORY_CD = T.FACTORY_CD
                                                                                              AND R.TRX_ID = T.TRX_ID
                                                                                              AND R.GRADE_CD = 'C'
                                                    WHERE   T.JOB_ORDER_NO IN (
                                                            SELECT  JO_NO
                                                            FROM    JO_HD WITH ( NOLOCK )
                                                            WHERE   STATUS NOT IN ( 'D', 'X' )
                                                                    AND SC_NO = @SC_NO )
                                                    GROUP BY JOB_ORDER_NO ,
                                                            SIZE_CODE ,
                                                            COLOR_CODE
                                                  ) E_3 ON E_3.SIZE_CD = C_1.SIZE_CD
                                                           AND E_3.COLOR_CD = C_1.COLOR_CD
                                                           AND E_3.JOB_ORDER_NO = C_1.JO_NO
                                        LEFT JOIN ( SELECT  JOB_ORDER_NO ,
                                                            COLOR_CODE COLOR_CD ,
                                                            SIZE_CODE SIZE_CD ,
                                                            SUM(PULL_IN_QTY) PULL_IN_QTY ,
                                                            SUM(PULL_OUT_QTY) PULL_OUT_QTY
                                                    FROM    dbo.PRD_JO_WIP_HD
                                                    WHERE   JOB_ORDER_NO IN (
                                                            SELECT  JO_NO
                                                            FROM    JO_HD WITH ( NOLOCK )
                                                            WHERE   STATUS NOT IN ( 'D', 'X' )
                                                                    AND SC_NO = @SC_NO )
                                                            AND FACTORY_CD = @FACTORY_CD
                                                            AND PROCESS_CD = 'CUT'
                                                    GROUP BY JOB_ORDER_NO ,
                                                            COLOR_CODE ,
                                                            SIZE_CODE
                                                  ) E_4 ON E_4.SIZE_CD = C_1.SIZE_CD
                                                           AND E_4.COLOR_CD = C_1.COLOR_CD

                                IF OBJECT_ID('tempdb..#CUT_TEMP_GO') IS NOT NULL 
                                    BEGIN
                                        DROP TABLE #CUT_TEMP_GO
                                    END ;

                                SELECT DISTINCT
                                        COLOR_CD ,
                                        SIZE_CD ,
                                        COLOR_DESC ,
                                        ORDER_QTY AS Order_Qty,
                                        CUT_QTY AS Cut_Qty,
                                        CUT_REDUCE AS Cut_Reduce,
                                        ACTUAL_QTY AS [Actual_Cut-Qty],
                                        [Over_Shortage_Qty] AS [Over/Short_Cut_Qty],
                                        [Over_Shortage_Percent] AS [Over/Short_Cut%],
                                        0 Scan_pack_Qty ,
                                        0 Ship_Qty ,
                                        0 [Over_ShortShip_Qty] ,
										0 [Over_ShortShip_%] ,
                                        Leftover_Garment AS Leftover_Garment_A,
                                        B_Grade_Qty ,
                                        C_Grade_Qty ,
                                        Unaccountable_Qty ,
                                        COLOR_SEQ ,
                                        SIZE_SEQ
                                INTO    #CUT_TEMP_GO
                                FROM    ( SELECT    COLOR_CD ,
                                                    SIZE_CD ,
                                                    SUM(ISNULL(ORDER_QTY, 0)) AS ORDER_QTY ,
                                                    SUM(ISNULL(CUT_QTY, 0)) AS CUT_QTY ,
                                                    SUM(ISNULL(SSample_QTY, 0)) AS SSample_QTY ,
                                                    SUM(ISNULL(CSample_QTY, 0)) AS CSample_QTY ,
                                                    SUM(ISNULL(ReCut_QTY, 0)) AS ReCut_QTY ,
                                                    SUM(ISNULL(CUT_REDUCE, 0)) AS CUT_REDUCE ,
                                                    SUM(ISNULL(ACTUAL_QTY, 0)) AS ACTUAL_QTY ,
                                                    SUM(ISNULL(CUT_QTY, 0)) + SUM(ISNULL(SSample_QTY, 0))
                                                    + SUM(ISNULL(CSample_QTY, 0)) + SUM(ISNULL(ReCut_QTY, 0))
                                                    - SUM(ISNULL(CUT_REDUCE, 0)) - SUM(ISNULL(ORDER_QTY, 0)) AS [Over_Shortage_Qty] ,
                                                    CASE WHEN SUM(ISNULL(ORDER_QTY, 0)) = 0
                                                              OR SUM(ISNULL(CUT_QTY, 0)) = 0 THEN NULL
                                                         ELSE CAST(ROUND(( SUM(CONVERT(DECIMAL(18, 3), ISNULL(ACTUAL_QTY,
                                                                                              0)))
                                                                           - SUM(CONVERT(DECIMAL(18, 3), ISNULL(ORDER_QTY,
                                                                                              0))) ) * 100
                                                                         / SUM(CONVERT(DECIMAL(18, 3), ISNULL(ORDER_QTY,
                                                                                              0))), 1) AS DECIMAL(10,
                                                                                              3))
                                                    END AS [Over_Shortage_Percent] ,
                                                    SUM(ISNULL(Leftover_Garment, 0)) Leftover_Garment ,
                                                    SUM(ISNULL(B_Grade_QTY, 0)) B_Grade_Qty ,
                                                    SUM(ISNULL(C_Grade_QTY, 0)) C_Grade_Qty ,
                                                --
                                                    SUM(ISNULL(ACTUAL_QTY, 0)) + SUM(PULL_IN_QTY)
                                                    - SUM(PULL_OUT_QTY) - ( SUM(ISNULL(Leftover_Garment, 0))
                                                                            + SUM(ISNULL(B_Grade_QTY, 0))
                                                                            + SUM(ISNULL(C_Grade_QTY, 0)) ) Unaccountable_Qty
                                          FROM      #TEMP_GO
                                          GROUP BY  SIZE_CD ,
                                                    COLOR_CD
                                        ) AS A
                                        LEFT JOIN ( SELECT  SIZE_CODE ,
                                                            SEQUENCE AS SIZE_SEQ
                                                    FROM    DBO.SC_SIZE WITH ( NOLOCK )
                                                    WHERE   SC_NO = @SC_NO
                                                  ) B ON A.SIZE_CD = B.SIZE_CODE
                                        LEFT JOIN ( SELECT  COLOR_CODE ,
                                                            COLOR_DESC ,
                                                            SEQUENCE AS COLOR_SEQ
                                                    FROM    SC_COLOR WITH ( NOLOCK )
                                                    WHERE   SC_NO = @SC_NO
                                                    UNION ALL
                                                    SELECT  'TOTAL' AS COLOR_CODE ,
                                                            NULL AS COLOR_DESC ,
                                                            '9999999' AS COLOR_SEQ
                                                    FROM    SC_COLOR WITH ( NOLOCK )
                                                    WHERE   SC_NO = @SC_NO
                                                  ) C ON A.COLOR_CD = C.COLOR_CODE
                                ORDER BY COLOR_SEQ ,
                                        SIZE_SEQ   

                                IF OBJECT_ID('tempdb..#TEMP_GO_SIZE_CD_COL') IS NOT NULL 
                                    BEGIN
                                        DROP TABLE #TEMP_GO_SIZE_CD_COL
                                    END ;       
                                SELECT  DISTINCT
                                        SIZE_CD ,
                                        SEQUENCE
                                INTO    #TEMP_GO_SIZE_CD_COL
                                FROM    ( SELECT DISTINCT
                                                    JO_NO ,
                                                    SIZE_CODE1 ,
                                                    ( RTRIM(ISNULL(SIZE_CODE1, ''))
                                                      + ( CASE ISNULL(SIZE_CODE2, '')
                                                            WHEN '-' THEN ''
                                                            ELSE ' ' + ISNULL(SIZE_CODE2, '')
                                                          END ) ) AS SIZE_CD ,
                                                    COLOR_CODE AS COLOR_CD ,
                                                    QTY AS ORDER_QTY
                                          FROM      DBO.JO_DT AS C WITH ( NOLOCK )
                                          WHERE     ( JO_NO IN ( SELECT JO_NO
                                                                 FROM   JO_HD WITH ( NOLOCK )
                                                                 WHERE  STATUS NOT IN ( 'D', 'X' )
                                                                        AND SC_NO = @SC_NO ) )
                                        ) A
                                        INNER JOIN JO_HD AS B ON A.JO_NO = B.JO_NO
                                        INNER JOIN SC_SIZE AS C ON B.SC_NO = C.SC_NO
                                                                   AND A.SIZE_CODE1 = C.SIZE_CODE
                                ORDER BY SEQUENCE

                                IF ( @TABLE_COL = '' ) 
                                    RETURN ;


                                --创建表结构
                                IF OBJECT_ID('tempdb..##TEMP_FINAL_GO') IS NOT NULL 
                                    BEGIN
                                        DROP TABLE ##TEMP_FINAL_GO
                                    END ;        
                                SET @SQL = 'CREATE TABLE ##TEMP_FINAL_GO ([COLOR_CD] NVARCHAR(30),Description NVARCHAR(30),'
                                    + REPLACE(@TABLE_COL, ',', ' Nvarchar(30),')
                                    + CASE WHEN @TABLE_COL = '' THEN ' Total NVARCHAR(20),SEQ NVARCHAR(5))'
                                           ELSE ' NVARCHAR(30),Total Decimal(10,2),SEQ INT)'
                                      END 
                                --PRINT @SQL
                                EXEC (@SQL)


                                --循环取数
                                DECLARE CUR_COL CURSOR
                                FOR
                                SELECT  name
                                FROM    tempdb.dbo.syscolumns
                                WHERE   id = ( SELECT   MAX(id)
                                FROM     tempdb.dbo.sysobjects
                                WHERE    xtype = 'u'
                                AND id = OBJECT_ID('tempdb..#CUT_TEMP_GO')
                                )
                                AND name NOT IN ( 'SIZE_CD', 'COLOR_CD','COLOR_DESC','COLOR_SEQ','SIZE_SEQ')
                                OPEN CUR_COL ;
                                FETCH NEXT FROM CUR_COL INTO @COL_NAME ;

                                WHILE @@fetch_status = 0 
                                    BEGIN

                                        SET @SEQ = @SEQ + 1  

                                        SET @SQL = ' INSERT INTO ##TEMP_FINAL_GO SELECT COLOR_CD,' + ''''
                                            + @COL_NAME + ''' AS Description,' + @TABLE_EMPTY + ','
                                            + @SIZ_CD_COL_ADD + ' AS Total ,' + CONVERT(NVARCHAR(5), @SEQ)
                                            + ' AS SEQ  FROM   (SELECT SIZE_CD,COLOR_CD,[' + @COL_NAME
                                            + '] FROM #CUT_TEMP_GO) AS P PIVOT( SUM([' + @COL_NAME
                                            + ']) FOR SIZE_CD IN ( ' + @TABLE_COL + ' ) )  AS PVI  '

                                --  PRINT @SQL
                                        EXEC(@SQL)    

                                        FETCH NEXT FROM CUR_COL INTO @COL_NAME ;  
                                    END
                                CLOSE CUR_COL 
                                DEALLOCATE CUR_COL ;


                                SELECT  TF.* ,
                                        COLOR_DESC
                                FROM    ##TEMP_FINAL_GO TF WITH ( NOLOCK )
                                        LEFT JOIN ( SELECT DISTINCT
                                                            COLOR_CD ,
                                                            COLOR_DESC
                                                    FROM    #CUT_TEMP_GO
                                                  ) CT ON TF.COLOR_CD = CT.COLOR_CD  COLLATE DATABASE_DEFAULT
                                ORDER BY COLOR_CD ,
                                        seq 
                        ", SC_NO, Site, Size, Size_Null,Size_EMPTY);
            return DBUtility.GetTable(sb.ToString(), "MES");
        }

        public static DataTable GetGO(string ScNo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT FNField SC_NO FROM FN_SPLIT_STRING_TB('{0}',',')", ScNo);
            return DBUtility.GetTable(sb.ToString(), "MES");
        }


        public static DataTable GetJO(string JoNo, string scNo, string factoryCd)
        {
            string SQL = "";
            SQL += " SELECT SC_NO,JO_NO FROM JO_HD WHERE STATUS<>'D' ";
            if (!scNo.Equals(""))
            {
                SQL += " AND SC_NO = '" + scNo + "'";
            }
            if (!JoNo.Equals(""))
            {
                //SQL += "AND JO_NO = '" + JoNo + "' ";
                SQL += " AND JO_NO IN (SELECT * FROM FN_SPLIT_STRING_TB('" + JoNo + "',',')) ";
            }
            SQL += " AND FACTORY_CD= '" + factoryCd + "'";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetShipmentClosing(string JOB_ORDER_NO, string FACTORY_CD, string Size, string Size_Null, string Size_EMPTY)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
                        DECLARE @JOB_ORDER_NO NVARCHAR(50) ,
                        @FACTORY_CD NVARCHAR(50)
 
                       DECLARE @SQL NVARCHAR(2000)
                       DECLARE @TABLE_COL NVARCHAR(500) ,
                        @TABLE_EMPTY NVARCHAR(500),
                        @COL_NAME NVARCHAR(30)
                       DECLARE @SEQ INT 
                       SET @JOB_ORDER_NO = '{0}'
                       SET @FACTORY_CD = '{1}'
                       SET @SEQ = 1
                      
                       DECLARE @SIZ_CD_COL_ADD NVARCHAR(500) 
                       SET @TABLE_COL = '{2}'
                       SET @SIZ_CD_COL_ADD = '{3}'
                       SET @TABLE_EMPTY = '{4}'
                       IF OBJECT_ID('tempdb..#CUT_TEMP') IS NOT NULL 
                        BEGIN
                            DROP TABLE #CUT_TEMP
                        END ;
                       IF OBJECT_ID('tempdb..#TMPColor') IS NOT NULL 
                        BEGIN
                            DROP TABLE #TMPColor
                        END ;
 
                       SELECT   *
                       INTO     #TMPColor
                       FROM     ( SELECT    CASE C.SIZE_CD2
                                              WHEN '-' THEN C.SIZE_CD
                                              ELSE C.SIZE_CD + ' ' + C.SIZE_CD2
                                            END AS SIZE_CD ,
                                            a.CUT_TYPE ,
                                            b.COLOR_CD ,
                                            D.QTY --A_1 A_2 A_3 A_4
                                  FROM      dbo.CUT_LAY AS a WITH ( NOLOCK )
                                            INNER JOIN dbo.CUT_LAY_HD c WITH ( NOLOCK ) ON c.LAY_ID = A.LAY_ID
                                            INNER JOIN dbo.CUT_LAY_DT AS b WITH ( NOLOCK ) ON c.LAY_TRANS_ID = b.LAY_TRANS_ID
                                            INNER JOIN CUT_BUNDLE_HD AS D ON D.LAY_DT_ID = B.LAY_DT_ID
                                  WHERE     ( a.JOB_ORDER_NO = @JOB_ORDER_NO )
                                            AND ( a.FACTORY_CD = @FACTORY_CD )
                                            AND ( a.PRINT_STATUS = 'Y' )
                                            AND ( D.TRX_TYPE = 'NM' )
                                ) AL
 
                    --总数据
                       SELECT   C_1.SIZE_CD ,--
                                C_1.COLOR_CD ,--
                                COLOR_DESC,
                                ISNULL(C_1.ORDER_QTY, 0) [Order_Qty] ,--
                                ISNULL(A_1.CUT_QTY, 0) [Cut_Qty] ,--
                                ISNULL(B_1.CUT_REDUCE, 0) [Cut_Reduce] ,--Cut-Reduce
                                ISNULL(A_1.CUT_QTY, 0) + ISNULL(SSample_QTY, 0)
                                + ISNULL(CSample_QTY, 0) + ISNULL(ReCut_QTY, 0)
                                - ISNULL(B_1.CUT_REDUCE, 0) AS [Actual_Cut-Qty] ,--Actual Cut-Qty
                                ISNULL(A_1.CUT_QTY, 0) + ISNULL(SSample_QTY, 0)
                                + ISNULL(CSample_QTY, 0) + ISNULL(ReCut_QTY, 0)
                                - ISNULL(B_1.CUT_REDUCE, 0) - ISNULL(C_1.ORDER_QTY, 0) AS [Over/Short_Cut_Qty] , --Over/Short Cut Qty
                                ( CASE WHEN ORDER_QTY = 0 THEN 0
                                       WHEN ISNULL(ORDER_QTY, '') = '' THEN ''
                                       ELSE CONVERT(DECIMAL(18, 3), ROUND(( ISNULL(CUT_QTY, 0)
                                                                            + ISNULL(A_2.SSample_QTY,
                                                                                  0)
                                                                            + ISNULL(A_3.CSample_QTY,
                                                                                  0)
                                                                            + ISNULL(A_4.ReCut_QTY,
                                                                                  0)
                                                                            - ISNULL(CUT_REDUCE, 0)
                                                                            - ISNULL(ORDER_QTY, 0) )
                                                                          * 100 / ORDER_QTY, 3))
                                  END ) AS [Over/Short_Cut%] , -- Over/Short Cut %
                                0 Scan_pack_Qty ,--来自于EASN
                                0 Ship_Qty ,  --来自于ORACLE
                                0 [Over_ShortShip_Qty] ,
                                0 [Over_ShortShip_%] ,
                                ISNULL(Leftover_Garment, 0) Leftover_Garment_A ,
                                ISNULL(B_Grade_QTY, 0) B_Grade_Qty ,
                                ISNULL(C_Grade_QTY, 0) C_Grade_Qty ,
                                -- Scan_pack_Qty 代码程序中计算
                                ( ( ISNULL(A_1.CUT_QTY, 0) + ISNULL(SSample_QTY, 0)
                                    + ISNULL(CSample_QTY, 0) + ISNULL(ReCut_QTY, 0)
                                    - ISNULL(B_1.CUT_REDUCE, 0) ) + ISNULL(E_4.PULL_IN_QTY,0) - ISNULL(E_4.PULL_OUT_QTY,0) )
                                - ( ISNULL(Leftover_Garment,0) + ISNULL(B_Grade_QTY,0) + ISNULL(C_Grade_QTY,0) ) Unaccountable_Qty
                       INTO     #CUT_TEMP
                       FROM     ( SELECT DISTINCT
                                              CASE SIZE_CODE2
                                              WHEN '-' THEN SIZE_CODE1
                                              ELSE SIZE_CODE1 + ' ' + SIZE_CODE2
                                            END AS SIZE_CD ,
                                            DT.COLOR_CODE AS COLOR_CD ,
					                        COLOR_DESC,
                                            QTY AS ORDER_QTY
                                  FROM      dbo.JO_DT AS DT WITH ( NOLOCK )
                                  INNER JOIN JO_HD HD WITH (NOLOCK) ON DT.JO_NO = HD.JO_NO
                                  INNER JOIN dbo.SC_COLOR CR WITH (NOLOCK) ON HD.SC_NO = CR.SC_NO AND CR.COLOR_CODE = DT.COLOR_CODE
                                  WHERE     ( DT.JO_NO = @JOB_ORDER_NO )
                                ) C_1
                                LEFT JOIN ( SELECT  SIZE_CD ,
                                                    COLOR_CD ,
                                                    SUM(QTY) CUT_QTY
                                            FROM    #TMPColor
                                            WHERE   CUT_TYPE = 'C'
                                                    OR CUT_TYPE IS NULL
                                                    OR CUT_TYPE = ''
                                            GROUP BY SIZE_CD ,
                                                    COLOR_CD
                                          ) A_1 ON C_1.COLOR_CD = A_1.COLOR_CD
                                                   AND C_1.SIZE_CD = A_1.SIZE_CD
                                LEFT JOIN ( SELECT  SIZE_CD ,
                                                    COLOR_CD ,
                                                    SUM(QTY) SSample_QTY
                                            FROM    #TMPColor
                                            WHERE   CUT_TYPE = 'SS'
                                            GROUP BY SIZE_CD ,
                                                    COLOR_CD
                                          ) A_2 ON C_1.COLOR_CD = A_2.COLOR_CD
                                                   AND C_1.SIZE_CD = A_2.SIZE_CD
                                LEFT JOIN ( SELECT  SIZE_CD ,
                                                    COLOR_CD ,
                                                    SUM(QTY) CSample_QTY
                                            FROM    #TMPColor
                                            WHERE   CUT_TYPE = 'CS'
                                            GROUP BY SIZE_CD ,
                                                    COLOR_CD
                                          ) A_3 ON C_1.COLOR_CD = A_3.COLOR_CD
                                                   AND C_1.SIZE_CD = A_3.SIZE_CD
                                LEFT JOIN ( SELECT  SIZE_CD ,
                                                    COLOR_CD ,
                                                    SUM(QTY) ReCut_QTY
                                            FROM    #TMPColor
                                            WHERE   CUT_TYPE = 'R'
                                            GROUP BY SIZE_CD ,
                                                    COLOR_CD
                                          ) A_4 ON C_1.COLOR_CD = A_4.COLOR_CD
                                                   AND C_1.SIZE_CD = A_4.SIZE_CD
                                LEFT JOIN ( SELECT  SIZE_CD ,
                                                    COLOR_CD ,
                                                    ABS(SUM(QTY)) AS CUT_REDUCE
                                            FROM    CUT_BUNDLE_HD WITH ( NOLOCK )
                                            WHERE   JOB_ORDER_NO = @JOB_ORDER_NO
                                                    AND FACTORY_CD = @FACTORY_CD
                                                    AND TRX_TYPE = 'RD'
                                                    AND STATUS = 'Y'
                                            GROUP BY COLOR_CD ,
                                                    SIZE_CD
                                          ) AS B_1 ON B_1.SIZE_CD = C_1.SIZE_CD
                                                      AND B_1.COLOR_CD = C_1.COLOR_CD
                                --Leftover Garment A
                                LEFT JOIN ( SELECT  SIZE_CODE AS SIZE_CD ,
                                                    COLOR_CODE AS COLOR_CD ,
                                                    SUM(R.PULLOUT_QTY) Leftover_Garment
                                            FROM    dbo.PRD_JO_PULLOUT_REASON R
                                                    WITH ( NOLOCK )
                                                    INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_TRX T ON R.FACTORY_CD = @FACTORY_CD
                                                              AND R.FACTORY_CD = T.FACTORY_CD
                                                              AND R.TRX_ID = T.TRX_ID
                                                              AND R.GRADE_CD ='A'
                                            WHERE   T.JOB_ORDER_NO = @JOB_ORDER_NO
                                            GROUP BY SIZE_CODE ,
                                                    COLOR_CODE
                                          ) E_1 ON E_1.SIZE_CD = C_1.SIZE_CD
                                                   AND E_1.COLOR_CD = C_1.COLOR_CD
                                --B Grade Qty
                                LEFT JOIN ( SELECT  SIZE_CODE AS SIZE_CD ,
                                                    COLOR_CODE AS COLOR_CD ,
                                                    SUM(R.PULLOUT_QTY) B_Grade_Qty
                                            FROM    dbo.PRD_JO_PULLOUT_REASON R
                                                    WITH ( NOLOCK )
                                                    INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_TRX T ON R.FACTORY_CD = @FACTORY_CD
                                                              AND R.FACTORY_CD = T.FACTORY_CD
                                                              AND R.TRX_ID = T.TRX_ID
                                            WHERE   T.JOB_ORDER_NO = @JOB_ORDER_NO
                                                    AND r.GRADE_CD = 'B'
                                            GROUP BY SIZE_CODE ,
                                                    COLOR_CODE
                                          ) E_2 ON E_2.SIZE_CD = C_1.SIZE_CD
                                                   AND E_2.COLOR_CD = C_1.COLOR_CD
                                --C Grade Qty
                                LEFT JOIN ( SELECT  SIZE_CODE AS SIZE_CD ,
                                                    COLOR_CODE AS COLOR_CD ,
                                                    SUM(R.PULLOUT_QTY) C_Grade_QTY
                                            FROM    dbo.PRD_JO_PULLOUT_REASON R
                                                    WITH ( NOLOCK )
                                                    INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_TRX T ON R.FACTORY_CD = @FACTORY_CD
                                                              AND R.FACTORY_CD = T.FACTORY_CD
                                                              AND R.TRX_ID = T.TRX_ID
                                            WHERE   T.JOB_ORDER_NO = @JOB_ORDER_NO
                                                    AND r.GRADE_CD = 'C'
                                            GROUP BY SIZE_CODE ,
                                                    COLOR_CODE
                                          ) E_3 ON E_3.SIZE_CD = C_1.SIZE_CD
                                                   AND E_3.COLOR_CD = C_1.COLOR_CD
                                --
                                 LEFT JOIN ( SELECT  COLOR_CODE COLOR_CD,
													SIZE_CODE SIZE_CD,
													SUM(PULL_IN_QTY) PULL_IN_QTY ,
													SUM(PULL_OUT_QTY) PULL_OUT_QTY
											FROM    dbo.PRD_JO_WIP_HD
											WHERE   JOB_ORDER_NO = @JOB_ORDER_NO
													AND FACTORY_CD = @FACTORY_CD
													AND PROCESS_CD = 'CUT'
											GROUP BY COLOR_CODE ,
													SIZE_CODE   
										 ) E_4 ON E_4.SIZE_CD = C_1.SIZE_CD
												   AND E_4.COLOR_CD = C_1.COLOR_CD
                                --LEFT JOIN ( SELECT  SIZE_CODE AS SIZE_CD ,
                                --                    COLOR_CODE AS COLOR_CD ,
                                --                    SUM(DISCREPANCY_QTY) AS CUT_DISCREPANCY_QTY ,
                                --                    0 AS EMB_DISCREPANCY_QTY ,
                                --                    0 AS PRINT_DISCREPANCY_QTY
                                --            FROM    dbo.PRD_JO_DISCREPANCY_PULLOUT_HD
                                --                    AS A
                                --                    INNER JOIN dbo.PRD_JO_DISCREPANCY_PULLOUT_TRX
                                --                    AS B ON A.DOC_NO = B.DOC_NO
                                --            WHERE   B.JOB_ORDER_NO = @JOB_ORDER_NO
                                --                    AND ( PROCESS_CD = 'CUT2'
                                --                          OR PROCESS_CD = 'EMB'
                                --                          OR PROCESS_CD = 'PRT'
                                --                        )
                                --            GROUP BY SIZE_CODE ,
                                --                    COLOR_CODE
                                --          ) AS D_1 ON C_1.SIZE_CD = D_1.SIZE_CD
                                --                      AND C_1.COLOR_CD = D_1.COLOR_CD      
  
                    --   IF OBJECT_ID('tempdb..#TEMP_SIZE_CD_COL') IS NOT NULL 
                    --    BEGIN
                    --        DROP TABLE #TEMP_SIZE_CD_COL
                    --    END ;
                        
                        
                    --   SELECT  DISTINCT
                    --            SIZE_CD ,
                    --            SEQUENCE
                    --   INTO     #TEMP_SIZE_CD_COL
                    --   FROM     ( SELECT DISTINCT
                    --                        JO_NO ,
                    --                        SIZE_CODE1 ,
                    --                        CASE SIZE_CODE2
                    --                          WHEN '-' THEN SIZE_CODE1
                    --                          ELSE SIZE_CODE1 + ' ' + SIZE_CODE2
                    --                        END AS SIZE_CD ,
                    --                        COLOR_CODE AS COLOR_CD ,
                    --                        QTY AS ORDER_QTY
                    --              FROM      dbo.JO_DT AS c WITH ( NOLOCK )
                    --              WHERE     ( JO_NO = @JOB_ORDER_NO )
                    --            ) A
                    --            INNER JOIN JO_HD AS B ON A.JO_NO = B.JO_NO
                    --            INNER JOIN SC_SIZE AS C ON B.SC_NO = C.SC_NO
                    --                                       AND A.SIZE_CODE1 = C.SIZE_CODE
                    --   ORDER BY C.SEQUENCE  
	                --
		            --
	                --
                    --   DECLARE @SIZ_CD_COL_ADD NVARCHAR(500) 
                    --   SET @SIZ_CD_COL_ADD = ''
                    --   SELECT   @SIZ_CD_COL_ADD = @SIZ_CD_COL_ADD + 'ISNULL([' + SIZE_CD
                    --            + '],0) +'
                    --   FROM     #TEMP_SIZE_CD_COL
                    --   IF @SIZ_CD_COL_ADD <> '' 
                    --    SELECT  @SIZ_CD_COL_ADD = LEFT(@SIZ_CD_COL_ADD, LEN(@SIZ_CD_COL_ADD) - 1)
                    --
                    -- --ISNULL([S],0) +ISNULL([M],0) +ISNULL([L],0) +ISNULL([XL],0) +ISNULL([XXL],0) 
                    --
                    --
                    --   SET @TABLE_COL = ''
                    --   SELECT   @TABLE_COL = @TABLE_COL + '[' + SIZE_CD + '],'
                    --   FROM     #TEMP_SIZE_CD_COL
                    --   ORDER BY SEQUENCE
                    --   IF @TABLE_COL <> '' 
                    --    SELECT  @TABLE_COL = LEFT(@TABLE_COL, LEN(@TABLE_COL) - 1)
                    -- --PRINT @TABLE_COL
                    -- --[S],[M],[L],[XL],[XXL]
                    --
                       IF ( @TABLE_COL = '' ) 
                         RETURN ;
 
                       IF OBJECT_ID('tempdb..##TEMP_FINAL') IS NOT NULL 
                        BEGIN
                            DROP TABLE ##TEMP_FINAL
                        END ;        
                       SET @SQL = 'CREATE TABLE ##TEMP_FINAL ([COLOR_CD] NVARCHAR(30),Description NVARCHAR(30),'
                        + REPLACE(@TABLE_COL, ',', ' Nvarchar(30),')
                        + CASE WHEN @TABLE_COL = ''
                               THEN ' Total NVARCHAR(20),SEQ NVARCHAR(5))'
                               ELSE ' NVARCHAR(30),Total Decimal(10,2),SEQ INT)'
                          END 
                       --PRINT @SQL
                       EXEC (@SQL)
  
 
                     --SELECT * FROM @TEMP_FINAL
 
                    -- SELECT *,'sdf' sa FROM #TEMP_FINAL
  
                      --循环取数
                       DECLARE CUR_COL CURSOR
                       FOR
                       SELECT  name
                       FROM    tempdb.dbo.syscolumns
                       WHERE   id = ( SELECT   MAX(id)
                       FROM     tempdb.dbo.sysobjects
                       WHERE    xtype = 'u'
                       AND id = OBJECT_ID('tempdb..#CUT_TEMP')
                       )
                       AND name NOT IN ( 'SIZE_CD', 'COLOR_CD','COLOR_DESC')
                       OPEN CUR_COL ;
                       FETCH NEXT FROM CUR_COL INTO @COL_NAME ;
                
                       WHILE @@fetch_status = 0 
                        BEGIN
		
                            SET @SEQ = @SEQ + 1  
       
                            SET @SQL = ' INSERT INTO ##TEMP_FINAL SELECT COLOR_CD,'
                                + '''' + @COL_NAME + ''' AS Description,'
                                + @TABLE_EMPTY + ',' + @SIZ_CD_COL_ADD
                                + ' AS Total ,'
                                + CONVERT(NVARCHAR(5), @SEQ)
                                + ' AS SEQ  FROM   (SELECT SIZE_CD,COLOR_CD,['
                                + @COL_NAME
                                + '] FROM #CUT_TEMP) AS P PIVOT( SUM(['
                                + @COL_NAME + ']) FOR SIZE_CD IN ( '
                                + @TABLE_COL + ' ) )  AS PVI  '
 
                            --PRINT @SQL
                            EXEC(@SQL)    
   
                            FETCH NEXT FROM CUR_COL INTO @COL_NAME ;  
                        END
                       CLOSE CUR_COL 
                       DEALLOCATE CUR_COL ;
                    
                    UPDATE  ##TEMP_FINAL
                    SET     ##TEMP_FINAL.Total = D.[TOT]
                    FROM    ##TEMP_FINAL,
                            ( SELECT    A.COLOR_CD ,
                                        CASE WHEN B.Total = 0 THEN 0
												 ELSE   CONVERT(DECIMAL(10, 2), CONVERT(DECIMAL, A.Total)
                                            / CONVERT(DECIMAL, B.Total) * 100)
                                            END [TOT]
                              FROM      ( SELECT    Total ,
                                                    COLOR_CD
                                          FROM      ##TEMP_FINAL
                                          WHERE     LTRIM(RTRIM(Description)) = 'Over_Shortage_Qty'
                                        ) A ,
                                        ( SELECT    Total ,
                                                    COLOR_CD
                                          FROM      ##TEMP_FINAL
                                          WHERE     LTRIM(RTRIM(Description)) = 'Order_Qty'
                                        ) B
                              WHERE     A.COLOR_CD = B.COLOR_CD
                            ) D
                    WHERE   ##TEMP_FINAL.COLOR_CD = D.COLOR_CD  AND ##TEMP_FINAL.Description = 'Over_Shortage_Percent'

                      SELECT  TF.*,COLOR_DESC
                    FROM    ##TEMP_FINAL TF WITH( NOLOCK)
 
                    LEFT JOIN (SELECT DISTINCT COLOR_CD,COLOR_DESC FROM #CUT_TEMP)CT 
                    ON TF.COLOR_CD = CT.COLOR_CD  COLLATE DATABASE_DEFAULT
                   -- WHERE Description = 'Scan_pack_Qty'
                    ORDER BY COLOR_CD ,
                            seq 
            ", JOB_ORDER_NO, FACTORY_CD, Size, Size_Null, Size_EMPTY);
            return DBUtility.GetTable(sb.ToString(), "MES");
        }

        public static DataTable GetShipmentCloseHeaderGO(string SC_NO, string FACTORY_CD)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
                          DECLARE @SC_NO NVARCHAR(50) ,
                            @FACTORY_CD NVARCHAR(50) ,
                            @ADDJoNo NVARCHAR(600) ,
                            @COMPLETE_DATE NVARCHAR(10)
    
                         SET @SC_NO = '{0}'
                         SET @FACTORY_CD = '{1}'
                         SET @ADDJoNo = ''
                         SELECT @ADDJoNo = @ADDJoNo + ( JO_NO + ';<br/>' )
                         FROM   JO_HD HD WITH ( NOLOCK )
                                LEFT JOIN SC_HD SC ( NOLOCK ) ON HD.SC_NO = SC.SC_NO
                         WHERE  HD.SC_NO = @SC_NO
                                AND HD.STATUS NOT IN ( 'D', 'X' )
 
                         SELECT @COMPLETE_DATE = DBO.DATE_FORMAT(MAX(OM.COMPLETE_DATE), 'yyyy-mm-dd')
                         FROM   SC_HD SC WITH ( NOLOCK )
                                RIGHT JOIN JO_HD PO WITH ( NOLOCK ) ON PO.SC_NO = SC.SC_NO
                                LEFT JOIN PRD_CUTTING_COMPLETION AS D ON PO.JO_NO = D.JOB_ORDER_NO
                                                -- ADD
                                LEFT JOIN [GEN_JOB_ORDER_MASTER] AS OM ON PO.FACTORY_CD = OM.FACTORY_CD
                                                                          AND D.JOB_ORDER_NO = OM.JOB_ORDER_NO
                                                                          AND COMPLETION_DATE IS NOT  NULL
                         WHERE  PO.SC_NO = @SC_NO
                                AND PO.STATUS NOT IN ( 'D', 'X' )
 
                         SELECT DISTINCT
                                PO.SC_NO ScNo ,--3
                                @ADDJoNo JoNo ,--1
                                SC.STYLE_NO StyleNo , --4
                               -- STH.STYLE_DESC StyleDesc ,
                                DBO.DATE_FORMAT(OS.BUYER_PO_DEL_DATE, 'yyyy-mm-dd') GMTDate ,  --5 GMT DATE
                                sc.season_cd season , --6 STYLE DESC
                                DBO.DATE_FORMAT(CD.Cut_Date, 'yyyy-mm-dd') [StartCutDate] , --2 Start Cut Time
                                @COMPLETE_DATE COMPLETIONDATE ,
                                ShippmentAllowance = (   SELECT   CAST(CAST(ROUND(( SUM(PERCENT_OVER_ALLOWED * 0.01 * a.total_qty)
                                          / C.TOTAL_QTY ) * 100, 2) AS DECIMAL(18, 2)) AS NVARCHAR(10))
											+ '%/'
											+ CAST(CAST(ROUND(( SUM(PERCENT_SHORT_ALLOWED * 0.01 * A.total_qty)
																/ C.TOTAL_QTY ) * 100, 2) AS DECIMAL(18, 2)) AS NVARCHAR(10))
											+ '%' 
								   FROM     JO_HD AS A
											INNER JOIN SC_LOT AS B ON A.SC_NO = B.SC_NO
																	  AND A.LOT_NO = B.LOT_NO
											INNER JOIN ( SELECT SC_NO ,
																SUM(TOTAL_QTY) AS TOTAL_QTY
														 FROM   JO_HD
														 WHERE STATUS not in ('D','X') AND  SC_NO = 'S15E00411'
														 GROUP BY SC_NO
													   ) AS C ON A.SC_NO = C.SC_NO
								   WHERE    STATUS not in ('D','X')
											AND A.SC_NO = 'S15E00411'
								   GROUP BY C.TOTAL_QTY)
                         FROM   SC_HD SC WITH ( NOLOCK )
                                RIGHT JOIN JO_HD PO WITH ( NOLOCK ) ON PO.SC_NO = SC.SC_NO
                                LEFT JOIN style_hd STH WITH ( NOLOCK ) ON STH.STYLE_NO = SC.STYLE_NO
                                                                          AND STH.STYLE_REV_NO = SC.STYLE_REV_NO
                                LEFT JOIN ( SELECT  B.SC_NO ,
                                                    MIN(actual_print_date) AS Cut_Date
                                            FROM    dbo.CUT_BUNDLE_HD AS A WITH ( NOLOCK )
                                                    INNER JOIN JO_HD AS B ON A.JOB_ORDER_NO = B.JO_NO
                                            WHERE   B.SC_NO = @SC_NO
                                                    AND B.STATUS NOT IN ( 'D', 'X' )
                                            GROUP BY B.SC_NO
                                          ) CD ON cd.SC_NO = PO.SC_NO
                              --  LEFT JOIN PRD_CUTTING_COMPLETION AS D ON PO.JO_NO = D.JOB_ORDER_NO
                                                -- ADD
                               -- LEFT JOIN [GEN_JOB_ORDER_MASTER] AS OM ON PO.FACTORY_CD = OM.FACTORY_CD
                               --                                           AND D.JOB_ORDER_NO = OM.JOB_ORDER_NO
                                --                                          AND COMPLETION_DATE IS NOT  NULL
                                LEFT JOIN ( SELECT  j.SC_NO ,
                                                    l.PERCENT_OVER_ALLOWED ,
                                                    l.PERCENT_SHORT_ALLOWED ,
                                                    l.BUYER_PO_DEL_DATE
                                            FROM    sc_lot l WITH ( NOLOCK )
                                                    JOIN jo_hd j ( NOLOCK ) ON l.sc_no = j.sc_no
                                                                               AND l.lot_no = j.LOT_NO
                                            WHERE   STATUS NOT IN ( 'D', 'X' )
                                                    AND j.SC_NO = @SC_NO
                                                    AND l.LOT_NO = ( SELECT MAX(LOT_NO)
                                                                     FROM   sc_lot
                                                                     WHERE  SC_NO = @SC_NO
                                                                   )
                                          ) OS ON PO.SC_NO = OS.SC_NO
                         WHERE  PO.SC_NO = @SC_NO
                                AND PO.STATUS NOT IN ( 'D', 'X' )
            ", SC_NO, FACTORY_CD);
            return DBUtility.GetTable(sb.ToString(), "MES");
        }

        public static DataTable GetShipmentCloseHeader(string JOB_ORDER_NO, string FACTORY_CD)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
                         DECLARE @JOB_ORDER_NO NVARCHAR(50) ,  @FACTORY_CD NVARCHAR(50)
  
                       SET @JOB_ORDER_NO = '{0}'
                       SET @FACTORY_CD = '{1}'

                        SELECT DISTINCT
                        PO.SC_NO ScNo,--3
                        PO.JO_NO JoNo,--1
                        SC.STYLE_NO StyleNo , --4
                        STH.STYLE_DESC StyleDesc ,
                        DBO.DATE_FORMAT(BUYER_PO_DEL_DATE, 'yyyy-mm-dd') GMTDate ,  --5 GMT DATE
                        DBO.DATE_FORMAT(GETDATE(), 'yyyy-mm-dd') printDate ,
                        sc.season_cd season , --6 STYLE DESC
                        DBO.DATE_FORMAT(CD.Cut_Date, 'yyyy-mm-dd') [StartCutDate] , --2 Start Cut Time
                        CASE WHEN D.COMPLETE_STATUS = 'Y' THEN 'Finished'
                             ELSE 'Un-Finished'
                        END AS COMPLETE_STATUS ,
                        DBO.DATE_FORMAT(D.COMPLETION_DATE, 'yyyy-mm-dd') COMPLETION_DATE,
                        DBO.DATE_FORMAT(OM.COMPLETE_DATE, 'yyyy-mm-dd') COMPLETIONDATE,
                        '+' + CONVERT(NVARCHAR(10), OS.PERCENT_OVER_ALLOWED)
                                  + ' / -' + CONVERT(NVARCHAR(10), OS.PERCENT_SHORT_ALLOWED)
                                  + '%'
                        AS  ShippmentAllowance
                 FROM   SC_HD SC WITH ( NOLOCK )
                        RIGHT JOIN JO_HD PO WITH ( NOLOCK ) ON PO.SC_NO = SC.SC_NO
                        LEFT JOIN style_hd STH WITH ( NOLOCK ) ON STH.STYLE_NO = SC.STYLE_NO
                                                                  AND STH.STYLE_REV_NO = SC.STYLE_REV_NO
                        LEFT JOIN ( SELECT  job_order_no ,
                                            MIN(actual_print_date) AS Cut_Date
                                    FROM    dbo.CUT_BUNDLE_HD WITH ( NOLOCK )
                                    WHERE   job_order_no = @JOB_ORDER_NO 
                                    GROUP BY job_order_no
                                  ) CD ON cd.job_order_no = PO.JO_NO
                        LEFT JOIN PRD_CUTTING_COMPLETION AS D ON PO.JO_NO = D.JOB_ORDER_NO
                        -- ADD
                        LEFT JOIN [GEN_JOB_ORDER_MASTER] AS OM ON PO.FACTORY_CD = OM.FACTORY_CD
                                                                  AND D.JOB_ORDER_NO = OM.JOB_ORDER_NO
                                                                  AND COMPLETION_DATE IS NOT  NULL
                        LEFT JOIN ( SELECT  j.JO_NO ,
                                            l.PERCENT_OVER_ALLOWED ,
                                            PERCENT_SHORT_ALLOWED
                                    FROM    sc_lot l WITH ( NOLOCK )
                                            JOIN jo_hd j ( NOLOCK ) ON l.sc_no = j.sc_no
                                                                       AND l.lot_no = j.LOT_NO
                                    WHERE   j.JO_NO = @JOB_ORDER_NO 
                                  ) OS ON PO.JO_NO = OS.JO_NO
                 WHERE  PO.JO_NO = @JOB_ORDER_NO 
            ", JOB_ORDER_NO, FACTORY_CD);
            return DBUtility.GetTable(sb.ToString(), "MES");
        }

        public static DataTable GetScanPackQTY(string JOB_ORDER_NO, string TABLE_COL_NULL, string TABLE_COL)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
                
                 SELECT  *, {0} Total
                FROM    ( SELECT    '{1}' CutNo ,
								   CASE  WHEN GOColor = '' THEN Color
										 WHEN GOColor IS NULL THEN Color
										 ELSE GOColor
									END GOColor,
									CASE  WHEN GOSize1 = '' THEN SIZE
										 WHEN GOSize1 IS NULL THEN SIZE
										 ELSE GOSize1
									END GOSize,
                                    SUM(Pacqty) AS PacTotal
                          FROM      ASNItem WITH ( NOLOCK )
                          WHERE     CutNo like '{1}%'
                          GROUP BY   CASE  WHEN GOColor = '' THEN Color
										 WHEN GOColor IS NULL THEN Color
										 ELSE GOColor
									END,
                                    CASE  WHEN GOSize1 = '' THEN SIZE
										 WHEN GOSize1 IS NULL THEN SIZE
										 ELSE GOSize1
									END 
                        ) AS P PIVOT( SUM(PacTotal) FOR GOSize IN ( {2} ) )  AS PVI 
            ", TABLE_COL_NULL, JOB_ORDER_NO, TABLE_COL);
            return DBUtility.GetTable(sb.ToString(), "EASN");
        }

        public static DataTable GetScanPackQTY(string JOB_ORDER_NO, string[] sizecol, string TABLE_COL)
        {
            string casesql = "";
            foreach (string c in sizecol)
            {
                casesql += string.Format("sum(case when GOSize='{0}' then PacTotal else  0 end) as [{0}],", c);
            }
            if (casesql.Length > 0)
            {
                casesql = casesql.Remove(casesql.Length - 1, 1);
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
                
                 SELECT CutNo,GOColor,{0},{2} as Total
                FROM    ( SELECT    '{1}' CutNo ,
								   CASE  WHEN GOColor = '' THEN Color
										 WHEN GOColor IS NULL THEN Color
										 ELSE GOColor
									END GOColor,
									CASE  WHEN GOSize1 = '' THEN SIZE
										 WHEN GOSize1 IS NULL THEN SIZE
										 ELSE GOSize1
									END GOSize,
                                    SUM(Pacqty) AS PacTotal
                          FROM      ASNItem WITH ( NOLOCK )
                          WHERE     CutNo like '{1}%'
                          GROUP BY   CASE  WHEN GOColor = '' THEN Color
										 WHEN GOColor IS NULL THEN Color
										 ELSE GOColor
									END,
                                    CASE  WHEN GOSize1 = '' THEN SIZE
										 WHEN GOSize1 IS NULL THEN SIZE
										 ELSE GOSize1
									END 
                        ) AS P group by CutNo,GOColor
            ", casesql, JOB_ORDER_NO, "sum(PacTotal)");
            return DBUtility.GetTable(sb.ToString(), "EASN");
        }

        public static DataTable GetScanPackQTYGO(string GO_NO, string TABLE_COL_NULL, string TABLE_COL, string joList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
                
                 SELECT  *, {0} Total
                FROM    ( SELECT    '{1}' CutNo ,
								   CASE  WHEN GOColor = '' THEN Color
										 WHEN GOColor IS NULL THEN Color
										 ELSE GOColor
									END GOColor,
									CASE  WHEN GOSize1 = '' THEN SIZE
										 WHEN GOSize1 IS NULL THEN SIZE
										 ELSE GOSize1
									END GOSize,
                                    SUM(Pacqty) AS PacTotal
                          FROM      ASNItem WITH ( NOLOCK )
                          WHERE     CutNo like substring('{1}',2,8)+'%'
                          GROUP BY   CASE  WHEN GOColor = '' THEN Color
										 WHEN GOColor IS NULL THEN Color
										 ELSE GOColor
									END,
                                    CASE  WHEN GOSize1 = '' THEN SIZE
										 WHEN GOSize1 IS NULL THEN SIZE
										 ELSE GOSize1
									END 
                        ) AS P PIVOT( SUM(PacTotal) FOR GOSize IN ( {2} ) )  AS PVI 
            ", TABLE_COL_NULL, GO_NO, TABLE_COL, joList);
            return DBUtility.GetTable(sb.ToString(), "EASN");
        }

         public static DataTable GetScanPackQTYGO(string GO_NO, string[] sizecol, string joList)
        {
            string casesql = "";
            foreach (string c in sizecol)
            {
                casesql += string.Format("sum(case when GOSize='{0}' then PacTotal else  0 end) as [{0}],", c);
            }
            if (casesql.Length > 0)
            {
                casesql = casesql.Remove(casesql.Length - 1, 1);
            }
            StringBuilder sb = new StringBuilder();
             sb.AppendFormat(@"
                
                 SELECT CutNo,GOColor,{0},{2} as Total
                FROM    ( SELECT    '{1}' CutNo ,
								   CASE  WHEN GOColor = '' THEN Color
										 WHEN GOColor IS NULL THEN Color
										 ELSE GOColor
									END GOColor,
									CASE  WHEN GOSize1 = '' THEN SIZE
										 WHEN GOSize1 IS NULL THEN SIZE
										 ELSE GOSize1
									END GOSize,
                                    SUM(Pacqty) AS PacTotal
                          FROM      ASNItem WITH ( NOLOCK )
                          WHERE     CutNo like substring('{1}',2,8)+'%'
                          GROUP BY   CASE  WHEN GOColor = '' THEN Color
										 WHEN GOColor IS NULL THEN Color
										 ELSE GOColor
									END,
                                    CASE  WHEN GOSize1 = '' THEN SIZE
										 WHEN GOSize1 IS NULL THEN SIZE
										 ELSE GOSize1
									END 
                        ) AS P group by CutNo,GOColor
                  ", casesql, GO_NO, "sum(PacTotal)");


             return DBUtility.GetTable(sb.ToString(), "EASN");
        }
                

        //Modified by LimML on 20150817 - add trace code: KBI and WBI
        public static DataTable GetShipQTYGO(string SC_NO, string FactoryCD)
        {
            StringBuilder sb = new StringBuilder();
//            sb.AppendFormat(@"
//                        SELECT  P.SC_NO, case when itc.escm_color_code is null then itc.color_code
//                                                 WHEN   itc.escm_color_code = '' then itc.color_code
//                                                else itc.escm_color_code
//                                                end Color_CD,
//                                          CASE WHEN itc.escm_size_code1 IS NULL THEN itc.size_code
//                                               WHEN itc.escm_size_code1 = '' THEN itc.size_code
//                                               ELSE itc.escm_size_code1
//                                               END Size_CD,
//                             SUM (l.qty) AS Qty
//                             FROM inventory.inv_trans_hd h,inventory.inv_trans_lines l,inventory.inv_trans_code tc,inventory.inv_trans_type tt,inventory.inv_cartons cs, 
//                             inventory.inv_store_codes s,inventory.inv_reason_Code r,inventory.inv_locations b,inventory.inv_location_pallets lp,inventory.inv_req_hd rh,inventory.inv_qa_hd q,
//                             inventory.inv_trans_line_items lit,inventory.inv_item_code itc ,escmowner.po_hd p
//                             Where h.trans_header_id = l.trans_header_id And h.trans_cd = tc.trans_cd And tc.trans_type_cd = tt.trans_type_cd 
//                               AND l.carton_id = cs.carton_id(+)  AND l.reason_cd = r.reason_cd(+)  AND h.req_header_id = rh.req_header_id(+) 
//                               AND rh.qa_header_id = q.qa_header_id(+)  AND l.from_location_id = b.location_id(+)  AND l.from_pallet_id = lp.pallet_id(+) 
//                               AND p.po_no = l.reference_no
//                               AND h.from_store_cd = s.store_cd  AND s.active = 'Y' AND h.status = 'F' 
//                               And tc.active='Y' and l.buyer_po_no<>'-'
//                               And tt.trans_type_desc='Issue'
//                               And l.trans_line_id=lit.trans_line_id
//                               And lit.item_id=itc.item_id
//                               AND s.factory_cd = '{0}' AND P.SC_NO = '{1}'
//                             GROUP BY P.SC_NO,
//                             case when itc.escm_color_code is null then itc.color_code
//                                                 WHEN   itc.escm_color_code = '' then itc.color_code
//                                                else itc.escm_color_code
//                                                end,
//                                          CASE WHEN itc.escm_size_code1 IS NULL THEN itc.size_code
//                                               WHEN itc.escm_size_code1 = '' THEN itc.size_code
//                                               ELSE itc.escm_size_code1
//                                               END
//                                               ORDER BY Size_CD
            sb.AppendFormat(@" 
            SELECT P.SC_NO, 
            CASE WHEN ESCM_COLOR_CODE IS NULL THEN COLOR_CODE WHEN ESCM_COLOR_CODE = '' THEN COLOR_CODE ELSE ESCM_COLOR_CODE END COLOR_CD,
            CASE WHEN ESCM_SIZE_CODE1 IS NULL THEN SIZE_CODE WHEN ESCM_SIZE_CODE1 = '' THEN SIZE_CODE ELSE ESCM_SIZE_CODE1 END SIZE_CD,
            SUM(QTY) AS QTY 
            FROM inventory.inv_jo_ship_for_mes_v V 
            INNER JOIN po_hd p
            ON v.job_order_no=p.po_no
            where P.SC_NO = '{1}'
            GROUP BY p.sc_no, v.color_code,v.size_code,v.escm_color_code,v.escm_size_code1
            ORDER BY SIZE_CD           
            ", FactoryCD, SC_NO);

            return DBUtility.GetTable(sb.ToString(), "inv_support");
        }
        
        public static DataTable GetShipQTY(string JO_NO, string FactoryCD)
        {
            StringBuilder sb = new StringBuilder();
//            sb.AppendFormat(@"
//              select * from (
//   SELECT  l.reference_no AS JobNo, case when itc.escm_color_code is null then itc.color_code
//                                                 WHEN   itc.escm_color_code = '' then itc.color_code
//                                                else itc.escm_color_code
//                                                end Color_CD,
//                                          CASE WHEN itc.escm_size_code1 IS NULL THEN itc.size_code
//                                               WHEN itc.escm_size_code1 = '' THEN itc.size_code
//                                               ELSE itc.escm_size_code1
//                                               END Size_CD,
//                             SUM (l.qty) AS Qty
//                             FROM inventory.inv_trans_hd h,inventory.inv_trans_lines l,inventory.inv_trans_code tc,inventory.inv_trans_type tt,inventory.inv_cartons cs, 
//                             inventory.inv_store_codes s,inventory.inv_reason_Code r,inventory.inv_locations b,inventory.inv_location_pallets lp,inventory.inv_req_hd rh,inventory.inv_qa_hd q,
//                             inventory.inv_trans_line_items lit,inventory.inv_item_code itc ,escmowner.po_hd p
//                             Where h.trans_header_id = l.trans_header_id And h.trans_cd = tc.trans_cd And tc.trans_type_cd = tt.trans_type_cd 
//                               AND l.carton_id = cs.carton_id(+)  AND l.reason_cd = r.reason_cd(+)  AND h.req_header_id = rh.req_header_id(+) 
//                               AND rh.qa_header_id = q.qa_header_id(+)  AND l.from_location_id = b.location_id(+)  AND l.from_pallet_id = lp.pallet_id(+) 
//                               AND p.po_no = l.reference_no
//                               AND h.from_store_cd = s.store_cd  AND s.active = 'Y' AND h.status = 'F' 
//                               And tc.active='Y' and l.buyer_po_no<>'-'
//                               AND l.buyer_po_no<>'-' And tt.trans_type_desc='Issue' 
//                               AND tc.trans_cd in ('KBI','WBI')
//                               And tt.trans_type_desc='Issue'
//                               And l.trans_line_id=lit.trans_line_id
//                               And lit.item_id=itc.item_id
//                               AND s.factory_cd = '{0}' AND L.REFERENCE_NO = '{1}'
//                             GROUP BY l.reference_no,
//                             case when itc.escm_color_code is null then itc.color_code
//                                                 WHEN   itc.escm_color_code = '' then itc.color_code
//                                                else itc.escm_color_code
//                                                end,
//                                          CASE WHEN itc.escm_size_code1 IS NULL THEN itc.size_code
//                                               WHEN itc.escm_size_code1 = '' THEN itc.size_code
//                                               ELSE itc.escm_size_code1
//                                               END
//
//                            ) Order by Size_CD
           
            sb.AppendFormat(@"
            SELECT JOB_ORDER_NO AS JOBNO,
            CASE WHEN ESCM_COLOR_CODE IS NULL THEN COLOR_CODE WHEN ESCM_COLOR_CODE = '' THEN COLOR_CODE ELSE ESCM_COLOR_CODE END COLOR_CD,
            CASE WHEN ESCM_SIZE_CODE1 IS NULL THEN SIZE_CODE WHEN ESCM_SIZE_CODE1 = '' THEN SIZE_CODE ELSE ESCM_SIZE_CODE1 END SIZE_CD,
            QTY from inventory.inv_jo_ship_for_mes_v where JOB_ORDER_NO = '{1}' ORDER BY SIZE_CD
            ", FactoryCD, JO_NO);

            return DBUtility.GetTable(sb.ToString(), "inv_support");
        }

        public static DataTable GetLeftoverGradeAGradeB(string JO, string factory_cd)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
                select mm.grade,mm.JONo, mm.color_code,mm.size_code,sum(mm.Qty) as qty from
                (
                SELECT tt.trans_type_desc AS Trans_Type, h.trans_cd AS Trans_CD, h.doc_no AS Doc_No, h.from_store_cd AS From_Store, 
                 h.to_store_cd AS To_Store, trunc(h.trans_date) AS Trans_Date,h.create_date AS Create_Date,  
                 nvl(q.create_date,rh.create_date) as SRN_Create_Date , l.reference_no As JONo,l.buyer_po_no AS Buyer_PO, l.style_no AS Style_No, 
                 l.grade,tt.multiplier*sum(li.qty) AS Qty, l.customer_cd AS Customer, h.Destination, h.Remarks, ic.color_code,
                CASE WHEN ic.size1_code IS NULL THEN ic.size_code
                                                               WHEN ic.size1_code = '' THEN ic.size_code
                                                               ELSE ic.size1_code
                                                               END size_code, 
                 cs.ucc_no AS UCC_No, cs.carton_no AS Carton_No ,cs.ct_no AS CT_No , rh.reason_cd as Reason_CD,
                r.reason_desc AS Transaction_Reason,b.loc_desc as Location_Desc,lp.pallet_desc  as Pallet_Desc ,
                decode(rh.req_for_qa_flag,'Y',case when h.qa_header_id>0 then 'QA' else 'Rework' end ,'') AS Type   
                 FROM inventory.inv_trans_hd h, inventory.inv_trans_lines l, inventory.inv_trans_code tc,inventory.inv_trans_type tt, inventory.inv_cartons cs,
                  inventory.inv_store_codes s, inventory.inv_reason_Code r,
                inventory.inv_trans_line_items li,inventory.inv_item_code ic,inventory.inv_locations b, inventory.inv_location_pallets lp, inventory.inv_req_hd rh, inventory.inv_qa_hd q   
                 WHERE h.trans_header_id = l.trans_header_id    AND h.trans_cd = tc.trans_cd    AND tc.trans_type_cd = tt.trans_type_cd    
                 AND l.carton_id = cs.carton_id(+)    AND rh.reason_cd = r.reason_cd(+)    AND h.req_header_id = rh.req_header_id(+)     
                 AND rh.qa_header_id = q.qa_header_id(+) AND l.from_location_id = b.location_id(+) AND l.from_pallet_id = lp.pallet_id(+)     
                 AND h.from_store_cd=s.store_cd and s.active='Y' AND s.factory_cd='{0}' AND h.status = 'F'    
                 AND l.trans_line_id=li.trans_line_id AND li.item_id=ic.item_id 
                 and tt.trans_type_desc='Receipt'
                and l.reference_no='{1}'
                and h.from_store_cd = '{0}-L11'
                GROUP BY tt.trans_type_desc, h.trans_cd , h.from_store_cd , h.doc_no , h.to_store_cd, trunc(h.trans_date), h.create_date, l.reference_no, 
                 l.buyer_po_no , l.style_no, l.grade, tt.multiplier, l.customer_cd,  h.Destination, h.Remarks,ic.color_code,
                cs.ucc_no,cs.carton_no, cs.ct_no , rh.reason_cd ,r.reason_desc,b.loc_desc,lp.pallet_desc, 
                 rh.req_for_qa_flag, h.qa_header_id,rh.create_date, q.create_date ,
                 CASE WHEN ic.size1_code IS NULL THEN ic.size_code
                                                               WHEN ic.size1_code = '' THEN ic.size_code
                                                               ELSE ic.size1_code
                                                               END 
                 ) mm
                group by mm.JONo, mm.color_code,mm.size_code,mm.grade
                ORDER BY mm.JONo, mm.color_code,mm.size_code,mm.grade
            ", factory_cd, JO);

            return DBUtility.GetTable(sb.ToString(), "inv_support");
        }

        public static DataTable GetLeftoverGradeAGradeBGO(string joList, string factory_cd)
        {   
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
                select mm.grade, mm.color_code,mm.size_code,sum(mm.Qty) as qty from
                (
                SELECT tt.trans_type_desc AS Trans_Type, h.trans_cd AS Trans_CD, h.doc_no AS Doc_No, h.from_store_cd AS From_Store, 
                 h.to_store_cd AS To_Store, trunc(h.trans_date) AS Trans_Date,h.create_date AS Create_Date,  
                 nvl(q.create_date,rh.create_date) as SRN_Create_Date , l.reference_no As JONo,l.buyer_po_no AS Buyer_PO, l.style_no AS Style_No, 
                 l.grade,tt.multiplier*sum(li.qty) AS Qty, l.customer_cd AS Customer, h.Destination, h.Remarks, ic.color_code,
                CASE WHEN ic.size1_code IS NULL THEN ic.size_code
                                                               WHEN ic.size1_code = '' THEN ic.size_code
                                                               ELSE ic.size1_code
                                                               END size_code, 
                 cs.ucc_no AS UCC_No, cs.carton_no AS Carton_No ,cs.ct_no AS CT_No , rh.reason_cd as Reason_CD,
                r.reason_desc AS Transaction_Reason,b.loc_desc as Location_Desc,lp.pallet_desc  as Pallet_Desc ,
                decode(rh.req_for_qa_flag,'Y',case when h.qa_header_id>0 then 'QA' else 'Rework' end ,'') AS Type   
                 FROM inventory.inv_trans_hd h, inventory.inv_trans_lines l, inventory.inv_trans_code tc,inventory.inv_trans_type tt, inventory.inv_cartons cs,
                  inventory.inv_store_codes s, inventory.inv_reason_Code r,
                inventory.inv_trans_line_items li,inventory.inv_item_code ic,inventory.inv_locations b, inventory.inv_location_pallets lp, inventory.inv_req_hd rh, inventory.inv_qa_hd q   
                 WHERE h.trans_header_id = l.trans_header_id    AND h.trans_cd = tc.trans_cd    AND tc.trans_type_cd = tt.trans_type_cd    
                 AND l.carton_id = cs.carton_id(+)    AND rh.reason_cd = r.reason_cd(+)    AND h.req_header_id = rh.req_header_id(+)     
                 AND rh.qa_header_id = q.qa_header_id(+) AND l.from_location_id = b.location_id(+) AND l.from_pallet_id = lp.pallet_id(+)     
                 AND h.from_store_cd=s.store_cd and s.active='Y' AND s.factory_cd='{0}' AND h.status = 'F'    
                 AND l.trans_line_id=li.trans_line_id AND li.item_id=ic.item_id 
                 and tt.trans_type_desc='Receipt'
                and l.reference_no in ({1})
                and h.from_store_cd = '{0}-L11'
                GROUP BY tt.trans_type_desc, h.trans_cd , h.from_store_cd , h.doc_no , h.to_store_cd, trunc(h.trans_date), h.create_date, l.reference_no, 
                 l.buyer_po_no , l.style_no, l.grade, tt.multiplier, l.customer_cd,  h.Destination, h.Remarks,ic.color_code,
                cs.ucc_no,cs.carton_no, cs.ct_no , rh.reason_cd ,r.reason_desc,b.loc_desc,lp.pallet_desc, 
                 rh.req_for_qa_flag, h.qa_header_id,rh.create_date, q.create_date ,
                 CASE WHEN ic.size1_code IS NULL THEN ic.size_code
                                                               WHEN ic.size1_code = '' THEN ic.size_code
                                                               ELSE ic.size1_code
                                                               END 
                 ) mm
                group by mm.color_code,mm.size_code,mm.grade
                ORDER BY mm.color_code,mm.size_code,mm.grade
            ", factory_cd, joList);

            return DBUtility.GetTable(sb.ToString(), "inv_support");
        }

        public static DataTable GetReduceQuantitySizeGO(string ScNo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
                 IF OBJECT_ID('tempdb..#TEMP_SIZE_GO') IS NOT NULL 
                    BEGIN
                        DROP TABLE #TEMP_SIZE_GO
                    END ;
                 DECLARE @TABLE_COL NVARCHAR(500)
                 DECLARE @TABLE_EMPTY NVARCHAR(500)
                 DECLARE @TABLE_COL_NULL NVARCHAR(500) 
                 SELECT  DISTINCT
                        SIZE_CD ,
                        SEQUENCE
                 INTO   #TEMP_SIZE_GO
                 FROM   ( SELECT DISTINCT
                                    JO_NO ,
                                    SIZE_CODE1 ,
                                    CASE SIZE_CODE2
                                      WHEN '-' THEN SIZE_CODE1
                                      ELSE SIZE_CODE1 + ' ' + SIZE_CODE2
                                    END AS SIZE_CD ,
                                    COLOR_CODE AS COLOR_CD ,
                                    QTY AS ORDER_QTY
                          FROM      dbo.JO_DT AS c WITH ( NOLOCK )
                          WHERE     c.JO_NO IN ( SELECT JO_NO
                                                 FROM   JO_HD WITH ( NOLOCK )
                                                 WHERE  STATUS NOT IN ( 'D', 'X' )
                                                        AND SC_NO = '{0}' )
                        ) A
                        INNER JOIN JO_HD AS B ON A.JO_NO = B.JO_NO
                        INNER JOIN SC_SIZE AS C ON B.SC_NO = C.SC_NO
                                                   AND A.SIZE_CODE1 = C.SIZE_CODE
                 ORDER BY C.SEQUENCE  
                       
                 SET @TABLE_COL = ''
                 SET @TABLE_EMPTY = ''
                  SELECT  @TABLE_COL = @TABLE_COL + '[' + SIZE_CD + '],',
                        @TABLE_EMPTY = @TABLE_EMPTY + 'ISNULL([' + SIZE_CD + '],0) [' + SIZE_CD + '],'
                 FROM   #TEMP_SIZE_GO
                 ORDER BY SEQUENCE
                IF @TABLE_COL <> '' 
                    SELECT  @TABLE_COL = LEFT(@TABLE_COL, LEN(@TABLE_COL) - 1)
                 IF @TABLE_EMPTY <> '' 
                    SELECT  @TABLE_EMPTY = LEFT(@TABLE_EMPTY, LEN(@TABLE_EMPTY) - 1)



                 SET @TABLE_COL_NULL = ''
                 SELECT @TABLE_COL_NULL = @TABLE_COL_NULL + 'ISNULL([' + SIZE_CD + '],0) +'
                 FROM   #TEMP_SIZE_GO
                 IF @TABLE_COL_NULL <> '' 
                    SELECT  @TABLE_COL_NULL = LEFT(@TABLE_COL_NULL, LEN(@TABLE_COL_NULL) - 1)
                 SELECT @TABLE_COL SIZE ,
                        @TABLE_COL_NULL SIZE_NULL ,
                        @TABLE_EMPTY SIZE_EMPTY
                 DROP TABLE  #TEMP_SIZE_GO
            ", ScNo);
            return DBUtility.GetTable(sb.ToString(), "MES");
        }

        public static DataTable GetReduceQuantitySize(string JoNo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
                IF OBJECT_ID('tempdb..#TEMP_SIZE') IS NOT NULL 
                BEGIN
                    DROP TABLE #TEMP_SIZE
                END ;
                DECLARE @TABLE_COL NVARCHAR(500)
                DECLARE @TABLE_EMPTY NVARCHAR(500)
                DECLARE @TABLE_COL_NULL NVARCHAR(500) 
                SELECT  DISTINCT
                        SIZE_CD ,
                        SEQUENCE
                INTO    #TEMP_SIZE
                FROM    ( SELECT DISTINCT
                                    JO_NO ,
                                    SIZE_CODE1 ,
                                    CASE SIZE_CODE2
                                        WHEN '-' THEN SIZE_CODE1
                                        ELSE SIZE_CODE1 + ' ' + SIZE_CODE2
                                    END AS SIZE_CD ,
                                    COLOR_CODE AS COLOR_CD ,
                                    QTY AS ORDER_QTY
                            FROM      dbo.JO_DT AS c WITH ( NOLOCK )
                            WHERE     ( JO_NO = '{0}' )
                        ) A
                        INNER JOIN JO_HD AS B ON A.JO_NO = B.JO_NO
                        INNER JOIN SC_SIZE AS C ON B.SC_NO = C.SC_NO
                                                    AND A.SIZE_CODE1 = C.SIZE_CODE
                ORDER BY C.SEQUENCE  
                       
                SET @TABLE_COL = ''
                SET @TABLE_EMPTY = ''
                SELECT  @TABLE_COL = @TABLE_COL + '[' + SIZE_CD + '],',
                        @TABLE_EMPTY = @TABLE_EMPTY + 'ISNULL([' + SIZE_CD + '],0) [' + SIZE_CD + '],'
                FROM    #TEMP_SIZE
                ORDER BY SEQUENCE
                IF @TABLE_COL <> '' 
                    SELECT  @TABLE_COL = LEFT(@TABLE_COL, LEN(@TABLE_COL) - 1)
                 IF @TABLE_EMPTY <> '' 
                    SELECT  @TABLE_EMPTY = LEFT(@TABLE_EMPTY, LEN(@TABLE_EMPTY) - 1)


                SET @TABLE_COL_NULL = ''
                SELECT  @TABLE_COL_NULL = @TABLE_COL_NULL + 'ISNULL([' + SIZE_CD + '],0) +'
                FROM    #TEMP_SIZE
                IF @TABLE_COL_NULL <> '' 
                    SELECT  @TABLE_COL_NULL = LEFT(@TABLE_COL_NULL, LEN(@TABLE_COL_NULL) - 1)
                SELECT  @TABLE_COL SIZE ,@TABLE_COL_NULL SIZE_NULL,@TABLE_EMPTY SIZE_EMPTY
                DROP TABLE  #TEMP_SIZE
            ", JoNo);
            return DBUtility.GetTable(sb.ToString(), "MES");
        }
    }
}