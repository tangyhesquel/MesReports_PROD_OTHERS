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
    ///GarmentTransferNoteSql 的摘要说明
    /// </summary>
    public class GarmentTransferSql
    {
        ////GarmentTransferNote
       
        //NOT YMG
        public static DataTable GetGarmentTransferNoteDetail(string docNO)
        {
            //modified by LimML on 20150727 - display NEW_SEQ_NO from CUT_BUNDLE_HD and NEW_LAY_NO from CUT_LAY if the column is not NULL. If both the columns are NULL, we would need to display the BUNDLE_NO from CUT_BUNDLE_HD and LAY_NO from CUT_LAY

            string sql = " DECLARE @START_SEQ INT;  ";   //Added By ZouShiChang ON 2014.06.04
            sql = sql + "if   object_id( 'tempdb..#temp_detail ')   is   not   null  drop   table   #temp_detail;";
            sql = sql + @" SELECT DISTINCT A.JOB_ORDER_NO, C.SC_NO, MAX(D.SHORT_NAME) AS CUSTOMER_NAME, ISNULL(A.COLOR_CD,'') AS COLOR_CD,
                                    ISNULL(MAX(E.COLOR_DESC),'') AS COLOR_DESC";
            sql = sql + @",ISNULL(A.SIZE_CD,'') AS SIZE_CD,ISNULL(SIZE_TB.SIZE_SEQ,999) AS SIZE_SEQ
                                    ,CASE WHEN ISNULL(CTLAY.NEW_LAY_NO ,0)=0 THEN CAST (A.CUT_LAY_NO AS NVARCHAR ) ELSE CAST (CTLAY.NEW_LAY_NO AS NVARCHAR ) END AS CUT_LAY_NO";
            sql = sql + @",CASE WHEN ISNULL(CTHD.NEW_SEQ_NO,0)=0 THEN CAST (A.BUNDLE_NO AS NVARCHAR ) ELSE CAST (CTHD.NEW_SEQ_NO AS NVARCHAR ) END  AS BUNDLE_NO,
                                    SUM(A.QTY) AS QTY ,TOTAL_CUT_QTY,MAX(ISNULL(OUTPUT_TB.TOTAL_OUTPUT_QTY,0)) AS TOTAL_OUTPUT_QTY,MIN(TRX_ID)  AS ID,SEQ_NO=IDENTITY(INT,0,1), Z.REMARK";
            sql = sql + " into #temp_detail ";
            sql = sql + @" FROM PRD_GARMENT_TRANSFER_DFT A ";
            sql = sql + @" INNER JOIN JO_HD B ON A.JOB_ORDER_NO=B.JO_NO 
                                    INNER JOIN SC_HD C ON B.SC_NO=C.SC_NO
                                    INNER JOIN GEN_CUSTOMER D ON C.CUSTOMER_CD=D.CUSTOMER_CD
                                    INNER JOIN PRD_GARMENT_TRANSFER_HD Z ON Z.DOC_NO = A.DOC_NO
                                    LEFT JOIN (SELECT JOB_ORDER_NO, BUNDLE_NO, NEW_SEQ_NO FROM CUT_BUNDLE_HD  
                                        WHERE STATUS='Y' AND TRX_TYPE = 'NM'
                                    ) CTHD ON A.JOB_ORDER_NO = CTHD.JOB_ORDER_NO
                                    AND A.BUNDLE_NO = CTHD.BUNDLE_NO
                                    LEFT JOIN CUT_LAY CTLAY 
                                    ON A.JOB_ORDER_NO = CTLAY.JOB_ORDER_NO
                                    AND A.CUT_LAY_NO = CTLAY.LAY_NO
                                    LEFT JOIN SC_COLOR E ON C.SC_NO=E.SC_NO AND E.COLOR_CODE=A.COLOR_CD                                    
                                    LEFT JOIN (SELECT   A.SC_NO,A.SEQUENCE * 10 +B.SEQUENCE AS SIZE_SEQ,A.SIZE_CODE+ CASE WHEN B.SIZE_CODE IS NULL THEN '' ELSE ' '+ B.SIZE_CODE END AS SIZE_CODE 
                                    FROM SC_SIZE A LEFT JOIN SC_SIZE B ON A.SC_NO=B.SC_NO AND A.SIZE_TYPE<>B.SIZE_TYPE
                                    WHERE A.SIZE_TYPE=1 AND (B.SIZE_TYPE=2 OR B.SIZE_TYPE IS NULL)
                                    ) AS SIZE_TB ON SIZE_TB.SC_NO=C.SC_NO AND SIZE_TB.SIZE_CODE=A.SIZE_CD
                                    LEFT JOIN
                                    ( SELECT JOB_ORDER_NO,SUM(QTY) AS TOTAL_CUT_QTY FROM CUT_BUNDLE_HD WHERE STATUS='Y' 
                                    AND JOB_ORDER_NO IN (SELECT JOB_ORDER_NO FROM PRD_GARMENT_TRANSFER_DFT WHERE DOC_NO='" + docNO + "')";
            sql = sql + @"   				   GROUP BY JOB_ORDER_NO 
                                    ) CUT_TB ON A.JOB_ORDER_NO=CUT_TB.JOB_ORDER_NO
                                    LEFT JOIN
                                    (
                                    SELECT JOB_ORDER_NO,SUM(CURR_PROCESS_QTY) AS TOTAL_OUTPUT_QTY FROM PRD_JO_STOCK ST WHERE 
                                    JOB_ORDER_NO IN (SELECT JOB_ORDER_NO FROM PRD_GARMENT_TRANSFER_DFT WHERE DOC_NO='" + docNO + "')";
            sql = sql + @"   							AND EXISTS(SELECT 1 FROM PRD_GARMENT_TRANSFER_DFT  DFT INNER JOIN PRD_GARMENT_TRANSFER_HD HD 
                                    ON HD.DOC_NO=DFT.DOC_NO  
                                    WHERE HD.DOC_NO='" + docNO + "' AND DFT.JOB_ORDER_NO=ST.JOB_ORDER_NO";
            sql = sql + " AND ST.CURR_PROCESS_GARMENT_TYPE=HD.PROCESS_GARMENT_TYPE AND ST.CURR_PROCESS_TYPE=HD.PROCESS_TYPE "; //Added By ZouShiChang ON 2013.08.19
            sql = sql + @" 													AND ST.CURR_PROCESS_CD=HD.PROCESS_CD
                                    )
                                    GROUP BY JOB_ORDER_NO
                                    ) OUTPUT_TB
                                    ON OUTPUT_TB.JOB_ORDER_NO=A.JOB_ORDER_NO
                                    WHERE A.DOC_NO='" + docNO + "'  ";
            sql = sql + @"GROUP BY A.JOB_ORDER_NO,A.COLOR_CD, C.SC_NO";
            sql = sql + @",A.SIZE_CD,A.CUT_LAY_NO,CTLAY.NEW_LAY_NO";
            sql = sql + @",A.BUNDLE_NO,CTHD.NEW_SEQ_NO,TOTAL_CUT_QTY ";
            sql = sql + @",SIZE_TB.SIZE_SEQ,SEQ_NO, Z.REMARK ";
            sql = sql + @"ORDER BY ID";//Trx_ID排序
            //更改排序为Trx_ID
            //sql = sql + @"ORDER BY A.JOB_ORDER_NO,A.COLOR_CD,ISNULL(BUNDLE_NO,0)";
            //sql = sql + @",ISNULL(SIZE_TB.SIZE_SEQ,999)";

            //sql = sql + ";select * from #temp_detail where size_cd not in('15.5/34-35');";
            
            //Added By ZouShiChang ON 2014.06.04 Start
            //sql = sql + ";select * from #temp_detail ;";
              sql=sql+" SELECT @START_SEQ=ISNULL(START_SEQ,1) FROM PRD_GARMENT_TRANSFER_HD WHERE DOC_NO='"+docNO +"' ";


              sql=sql+" SELECT JOB_ORDER_NO,SC_NO,CUSTOMER_NAME,COLOR_CD,COLOR_DESC,SIZE_CD,SIZE_SEQ,CUT_LAY_NO,BUNDLE_NO,QTY,TOTAL_CUT_QTY,TOTAL_OUTPUT_QTY,ID,SEQ_NO+@START_SEQ AS SEQ_NO,REMARK ";
              sql = sql + " FROM    #temp_detail ";
           //Added By ZouShiChang ON 2014.06.04 End

            return DBUtility.GetTable(sql, "MES");
        }

        public static DataTable GetGarmentCuttingTransferHeaderInfo(string docNO)
        {
            string sql = @"SELECT HD.*,PRD_LINE.PRODUCTION_LINE_NAME AS PRD_LINE_NAME,
                                    PRD_LINE1.PRODUCTION_LINE_NAME AS NEXT_PRD_LINE_NAME,
                                    CASE WHEN HD.SUBMIT_DATE IS NULL  THEN HD.CREATE_DATE
                                    ELSE HD.SUBMIT_DATE
                                    END  AS TRX_DATE,
                                    CASE WHEN HD.SUBMIT_USER_ID IS NULL THEN USR1.NAME
                                    ELSE USR.NAME
                                    END AS USER_NAME
                                    FROM 	PRD_GARMENT_TRANSFER_HD HD            
                                    LEFT JOIN GEN_PRODUCTION_LINE PRD_LINE
                                    ON PRD_LINE.FACTORY_CD=HD.FACTORY_CD AND PRD_LINE.PRODUCTION_LINE_CD=HD.PRODUCTION_LINE_CD
                                    LEFT JOIN GEN_PRODUCTION_LINE PRD_LINE1
                                    ON PRD_LINE1.FACTORY_CD=HD.FACTORY_CD AND PRD_LINE1.PRODUCTION_LINE_CD=HD.NEXT_PRODUCTION_LINE_CD
                                    LEFT JOIN MES_USER USR ON USR.USER_ID=HD.SUBMIT_USER_ID
                                    LEFT JOIN MES_USER USR1 ON USR1.USER_ID=HD.CREATE_USER_ID
	                                WHERE DOC_NO='" + docNO + "'";

           
            return DBUtility.GetTable(sql, "MES");
        }
        public static DataTable GetGTFNoteDetailPrintInfo(string factorycd, string processcd)
        {
            string sql = "select system_value from  dbo.GEN_SYSTEM_SETTING where factory_cd='" + factorycd + "' ";
            sql = sql + " and SYSTEM_KEY='GTF_NOTE_PRINT' and system_value like '%" + processcd + "%'";
            return DBUtility.GetTable(sql, "MES");
        }
        //YMG
 

        public static DataTable GetGarmentTransferNoteDetail(string docNO, string FactoryCd, string[] columnsName)
        {
            string columns = "";
            string group = "";

            for (int i = 0; i < columnsName.Length; i++)
            {
                if (!columnsName[i].ToString().Equals(""))
                {
                    if (columnsName[i].ToString().Contains("QTY"))
                    {
                        columns += "SUM(" + columnsName[i].ToString() + ") AS " + columnsName[i].ToString() + ",";
                    }
                    else
                    {
                        columns += columnsName[i].ToString() + ",";
                        group += columnsName[i].ToString() + ",";//group字段不需要包括QTY；
                    }
                }
            }
            columns = columns.Substring(0, columns.Length - 1);
            group = group.Substring(0, group.Length - 1);//去掉最后的逗号；


            string SQL = "";
            SQL += @" if   object_id( 'tempdb..#temp_detail ')   is   not   null  drop   table   #temp_detail;  SELECT A.JOB_ORDER_NO,MAX(D.SHORT_NAME) AS CUSTOMER_NAME, ISNULL(A.COLOR_CD,'') AS COLOR_CD,
                                ISNULL(MAX(E.COLOR_DESC),'') AS COLOR_DESC  ,ISNULL(A.SIZE_CD,'') AS SIZE_CD,ISNULL(SIZE_TB.SIZE_SEQ,999) AS SIZE_SEQ
                                ,CASE WHEN ISNULL(CTLAY.NEW_LAY_NO ,0)=0 THEN CAST (A.CUT_LAY_NO AS NVARCHAR ) ELSE CAST (CTLAY.NEW_LAY_NO AS NVARCHAR ) END AS CUT_LAY_NO,CASE WHEN ISNULL(CTHD.NEW_SEQ_NO,0)=0 THEN CAST (A.BUNDLE_NO AS NVARCHAR ) ELSE CAST (CTHD.NEW_SEQ_NO AS NVARCHAR ) END  AS BUNDLE_NO,
                                SUM(A.QTY) AS QTY,MIN(A.TRX_ID)  AS ID into #temp_detail   
                                FROM PRD_GARMENT_TRANSFER_DFT A WITH(NOLOCK) 
                                LEFT JOIN (SELECT JOB_ORDER_NO, BUNDLE_NO, NEW_SEQ_NO FROM CUT_BUNDLE_HD 
                                WHERE STATUS='Y' AND TRX_TYPE = 'NM'
                                )CTHD ON A.JOB_ORDER_NO = CTHD.JOB_ORDER_NO
                                AND A.BUNDLE_NO = CTHD.BUNDLE_NO
                                LEFT JOIN CUT_LAY CTLAY 
                                ON A.JOB_ORDER_NO = CTLAY.JOB_ORDER_NO
                                AND A.CUT_LAY_NO = CTLAY.LAY_NO
                                LEFT JOIN PRD_BARCODE_CENTER BC WITH(NOLOCK) ON BC.TRX_ID = A.TRX_ID  AND BC.JONO=A.JOB_ORDER_NO AND BC.STATUS = 'Y'
                                INNER JOIN JO_HD B WITH(NOLOCK)  ON A.JOB_ORDER_NO=B.JO_NO 
                                INNER JOIN SC_HD C WITH(NOLOCK)  ON B.SC_NO=C.SC_NO
                                INNER JOIN GEN_CUSTOMER D WITH(NOLOCK)  ON C.CUSTOMER_CD=D.CUSTOMER_CD
                                LEFT JOIN SC_COLOR E WITH(NOLOCK)  ON C.SC_NO=E.SC_NO AND E.COLOR_CODE=A.COLOR_CD                                    
                                LEFT JOIN (SELECT   A.SC_NO,A.SEQUENCE * 10 +B.SEQUENCE AS SIZE_SEQ,A.SIZE_CODE+ CASE WHEN B.SIZE_CODE IS NULL THEN '' ELSE ' '+ B.SIZE_CODE END AS SIZE_CODE 
                                FROM SC_SIZE A WITH(NOLOCK) 
                                LEFT JOIN SC_SIZE B WITH(NOLOCK) ON A.SC_NO=B.SC_NO AND A.SIZE_TYPE<>B.SIZE_TYPE
                                WHERE A.SIZE_TYPE=1 AND (B.SIZE_TYPE=2 OR B.SIZE_TYPE IS NULL)
                                ) AS SIZE_TB ON SIZE_TB.SC_NO=C.SC_NO AND SIZE_TB.SIZE_CODE=A.SIZE_CD
                                WHERE A.DOC_NO='" + docNO + "'  AND C.FACTORY_CD='" + FactoryCd + "' ";
            SQL += @" GROUP BY A.JOB_ORDER_NO,A.COLOR_CD  ,A.SIZE_CD,CUT_LAY_NO, CTLAY.NEW_LAY_NO, A.BUNDLE_NO,CTHD.NEW_SEQ_NO, SIZE_TB.SIZE_SEQ   
                                ORDER BY A.JOB_ORDER_NO,A.COLOR_CD,ISNULL(A.BUNDLE_NO,0)  ,ISNULL(SIZE_TB.SIZE_SEQ,999)";
            SQL += @" ;select JOB_ORDER_NO," + columns + " from #temp_detail A ";
            SQL += @" group by JOB_ORDER_NO," + group;
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetGarmentTransferNoteTotalQTY(string docNO, string JONO, string[] columnsName)
        {
            string columns = "";
            string group = "";

            for (int i = 0; i < columnsName.Length; i++)
            {
                if (!columnsName[i].ToString().Equals(""))
                {
                    if (columnsName[i].ToString().Contains("QTY"))
                    {
                        columns += "SUM(" + columnsName[i].ToString() + ") AS " + columnsName[i].ToString() + ",";
                    }
                    else
                    {
                        columns += columnsName[i].ToString() + ",";
                        group += columnsName[i].ToString() + ",";//group字段不需要包括QTY；
                    }
                }
            }
            columns = columns.Substring(0, columns.Length - 1);
            group = group.Substring(0, group.Length - 1);//去掉最后的逗号；


            string SQL = "";
            SQL += " if   object_id( 'tempdb..#TEMP_TOTAL ')   is   not   null  drop   table   #TEMP_TOTAL; ";
            SQL += " SELECT DISTINCT JOB_ORDER_NO,COLOR_CD,SIZE_CD, TOTAL_BUNDLE_NO_QTY = ";
            SQL += " (SELECT COUNT(DISTINCT BUNDLE_NO)";
            SQL += " FROM PRD_GARMENT_TRANSFER_DFT WITH(NOLOCK) ";
            SQL += " WHERE DOC_NO=A.DOC_NO AND JOB_ORDER_NO = A.JOB_ORDER_NO";
            SQL += " AND COLOR_CD = A.COLOR_CD AND SIZE_CD=A.SIZE_CD AND (BUNDLE_NO <>NULL OR BUNDLE_NO <>'')";
            SQL += " ),SUM(QTY) AS TOTAL_QTY_BY_PIC ";
            SQL += " INTO #TEMP_TOTAL ";
            SQL += " FROM PRD_GARMENT_TRANSFER_DFT A WITH(NOLOCK)";
            SQL += " LEFT JOIN PRD_BARCODE_CENTER BC WITH(NOLOCK) ON BC.TRX_ID = A.TRX_ID AND BC.STATUS = 'Y'";//YMG只需要扫描数据;
            SQL += " WHERE A.DOC_NO='" + docNO + "'";
            SQL += " AND JOB_ORDER_NO ='" + JONO + "'";
            SQL += " GROUP BY A.DOC_NO,JOB_ORDER_NO,COLOR_CD,SIZE_CD;";

            SQL += " SELECT JOB_ORDER_NO," + columns + " FROM #TEMP_TOTAL A";
            SQL += @" group by JOB_ORDER_NO," + group;
            return DBUtility.GetTable(SQL, "MES");
        }
     

         public static DataTable GetGarmentTransferNoteDetail_BUNDLE(string docNO, string FactoryCd, string JONO, string ProcessCd, string[] columnsName)
         {
             string columns = "";
             string group = "";

             for (int i = 0; i < columnsName.Length; i++)
             {
                 if (!columnsName[i].ToString().Equals(""))
                 {
                     if (columnsName[i].ToString().Contains("QTY"))
                     {
                         columns += "SUM(" + columnsName[i].ToString() + ") AS " + columnsName[i].ToString() + ",";
                     }
                     else
                     {
                         columns += columnsName[i].ToString() + ",";
                         group += columnsName[i].ToString() + ",";//group字段不需要包括QTY；
                     }
                 }
             }
             columns = columns.Substring(0, columns.Length - 1);
             group = group.Substring(0, group.Length - 1);//去掉最后的逗号；

             string SQL = "";
             SQL += @"  if   object_id( 'tempdb..#TEMP_ALL_TOTAL')   is   not   null  drop   table   #TEMP_ALL_TOTAL;  
                                SELECT A.JOB_ORDER_NO,A.Bundle_no
                                ,ISNULL(A.SIZE_CD,'') AS SIZE_CD
                                ,ISNULL(A.COLOR_CD,'') AS COLOR_CD
                                ,ISNULL(MAX(COLOR_DESC.COLOR_DESC),'') AS COLOR_DESC
                                ,CURRENT_TOTAL_QTY =(SELECT SUM(QTY) FROM PRD_GARMENT_TRANSFER_DFT WITH(NOLOCK)
                                WHERE JOB_ORDER_NO = A.JOB_ORDER_NO AND COLOR_CD = A.COLOR_CD AND SIZE_CD = A.SIZE_CD
                                AND bundle_no=A.bundle_no
                                AND DOC_NO = '" + docNO + @"' GROUP BY JOB_ORDER_NO,COLOR_CD,SIZE_CD) 
                                INTO #TEMP_ALL_TOTAL     
                                FROM PRD_GARMENT_TRANSFER_DFT A WITH(NOLOCK)                                
                                INNER JOIN 
                                (
                                SELECT A.JO_NO,B.COLOR_CODE,B.COLOR_DESC 
                                FROM JO_HD A WITH(NOLOCK)
                                LEFT JOIN SC_COLOR B WITH(NOLOCK)  ON B.SC_NO=A.SC_NO
                                WHERE A.JO_NO = '" + JONO + "'  AND FACTORY_CD = '" + FactoryCd + "'    ";
             SQL += @"  )COLOR_DESC  ON COLOR_DESC.JO_NO = A.JOB_ORDER_NO  AND COLOR_DESC.COLOR_CODE=A.COLOR_CD  
                                LEFT JOIN PRD_BARCODE_CENTER B WITH(NOLOCK)
                                ON B.JONO = A.JOB_ORDER_NO AND B.STATUS = 'Y' AND B.TRX_ID = A.TRX_ID
                                WHERE A.JOB_ORDER_NO ='" + JONO + "' AND A.DOC_NO ='" + docNO + "' GROUP BY A.JOB_ORDER_NO,A.COLOR_CD,A.SIZE_CD,A.Bundle_no";
             SQL += @"  ORDER BY A.JOB_ORDER_NO,A.COLOR_CD";
             
             SQL += @";SELECT JOB_ORDER_NO,[COLOR_CD],sum(case when A.bundle_no=0 then 0 else 1 end ) as CURRENT_TOTAL_BUNDLE_NO,SUM([CURRENT_TOTAL_QTY]) AS [CURRENT_TOTAL_QTY], ";
              SQL += @" TOTAL_CUT_QTY=(  SELECT SUM(QTY) AS TOTAL_CUT_QTY 
                                FROM dbo.V_BUNDLE WITH(NOLOCK)
                                WHERE JOB_ORDER_NO=A.JOB_ORDER_NO AND COLOR_CD = A.COLOR_CD 
                                GROUP BY JOB_ORDER_NO,COLOR_CD
                                )  ,                              
                TOTAL_OUTPUT_QTY=isnull(( 
                                SELECT SUM(B.QTY) AS OUT_QTY
                                FROM  dbo.PRD_GARMENT_TRANSFER_HD HD WITH(NOLOCK)
                                INNER JOIN PRD_GARMENT_TRANSFER_DFT B WITH(NOLOCK) ON HD.DOC_NO = B.DOC_NO ";
                                
             SQL += @"                  WHERE  EXISTS(
                                            SELECT 1 FROM PRD_GARMENT_TRANSFER_HD Z WITH(NOLOCK)
                                           WHERE HD.PROCESS_CD=Z.PROCESS_CD AND HD.NEXT_PROCESS_CD=Z.NEXT_PROCESS_CD 
                                            AND Z.DOC_NO= '" + docNO + @"' 
                                        )
                                    AND HD.STATUS = 'C' AND HD.FACTORY_CD = '" + FactoryCd + "' AND B.JOB_ORDER_NO = '" + JONO + "'  ";
             SQL += @"  AND B.JOB_ORDER_NO=A.JOB_ORDER_NO AND B.COLOR_CD = A.COLOR_CD ";


             SQL += @"                ),0)           
                        FROM #TEMP_ALL_TOTAL A";

             

             SQL += @" GROUP BY JOB_ORDER_NO," + group;
             return DBUtility.GetTable(SQL, "MES");
         }

    }
}