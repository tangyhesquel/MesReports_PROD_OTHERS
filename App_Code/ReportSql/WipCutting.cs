using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web.UI.WebControls;

/// <summary>
///WipCutting 的摘要说明
/// </summary>
namespace MESComment
{
    public class WipCutting
    {
        public WipCutting()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        public static DataTable GetCuttingHead(string JoNo, string FactoryCd)
        {
            string SQL = " SELECT DISTINCT  ";
            SQL = SQL + "      SC.SC_NO AS scNo, PO.JO_NO AS joNo, SC.STYLE_NO AS styleNo,  ";
            SQL = SQL + "      dbo.STYLE_HD.STYLE_DESC AS styleDesc,  ";
            SQL = SQL + "      dbo.DATE_FORMAT(PO.BUYER_PO_DEL_DATE, 'yyyy-mm-dd') AS gmtDelDate,  ";
            SQL = SQL + "      dbo.DATE_FORMAT(GETDATE(), 'yyyy-mm-dd') AS printDate,  ";
            SQL = SQL + "      SC.SEASON_CD AS season, a.FACTORY_CD AS factoryCd ";
            SQL = SQL + "FROM dbo.SC_HD AS SC LEFT OUTER JOIN ";
            SQL = SQL + "      dbo.JO_HD AS PO ON SC.SC_NO = PO.SC_NO INNER JOIN ";
            SQL = SQL + "      dbo.PRD_JO_OUTPUT_TRX AS a ON  ";
            SQL = SQL + "      PO.JO_NO = a.JOB_ORDER_NO LEFT OUTER JOIN ";
            SQL = SQL + "      dbo.STYLE_HD ON SC.STYLE_NO = dbo.STYLE_HD.STYLE_NO ";
            SQL = SQL + "WHERE (a.JOB_ORDER_NO = '" + JoNo + "') AND (a.FACTORY_CD = '" + FactoryCd + "') ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetCuttingDetail(string JoNo, string FactoryCd)
        {
            string SQL = @"   DECLARE @sqlText NVARCHAR(MAX) 
                            SELECT @sqlText = 'select COLOR_CODE AS [Colour\Size],' 
  
                            SELECT @sqlText = @sqltext + 'SUM(CASE SIZE_CODE WHEN ''' + SIZE_CODE
                                + ''' THEN OUTPUT_QTY ELSE 0 END) AS '''
                                + CASE WHEN ISNULL(SIZE_CODE, '') = '' THEN 'NO SIZE'
                                        ELSE SIZE_CODE
                                    END + ''','
                            FROM   ( SELECT DISTINCT
                                            SIZE_CODE
                                    FROM      PRD_JO_OUTPUT_TRX
                                    WHERE     job_order_no = '" + JoNo+@"'
                                            AND SIZE_CODE <> ''
                                            AND FACTORY_CD = '"+FactoryCd+@"'
                                            AND ( PROCESS_CD IN (
                                                    SELECT    PRC_CD
                                                    FROM      dbo.GEN_PRC_CD_MST
                                                    WHERE     ( dbo.PRD_JO_OUTPUT_TRX.FACTORY_CD = '"+FactoryCd+@"' )
                                                            AND ( DISPLAY_SEQ = 1 ) ) )
                                    UNION
                                    SELECT DISTINCT
                                            SIZE_CODE
                                    FROM      PRD_JO_OUTPUT_TRX
                                    WHERE     job_order_no = '"+JoNo+@"'
                                            AND SIZE_CODE = ''
                                            AND FACTORY_CD = '"+FactoryCd+@"'
                                            AND ( PROCESS_CD IN (
                                                    SELECT    PRC_CD
                                                    FROM      dbo.GEN_PRC_CD_MST
                                                    WHERE     ( dbo.PRD_JO_OUTPUT_TRX.FACTORY_CD = '"+JoNo+@"' )
                                                            AND ( DISPLAY_SEQ = 1 ) ) )
                                ) AS a 


                            SELECT @sqlText = LEFT(@sqlText, LEN(@sqlText) - 1)
                                + ',SUM(OUTPUT_QTY) AS TTL_QTY  FROM PRD_JO_OUTPUT_TRX WHERE job_order_no='
                                + '''"+JoNo+@"''' + 'AND FACTORY_CD=' + '''"+FactoryCd+@"'''
                                + 'AND ( PROCESS_CD IN (
                                        SELECT    PRC_CD
                                        FROM      dbo.GEN_PRC_CD_MST
                                        WHERE     ( dbo.PRD_JO_OUTPUT_TRX.FACTORY_CD = ' + '''"+FactoryCd+@"'''
                                + ' )
                                                AND ( DISPLAY_SEQ = 1 ) ) )' + ' GROUP BY Color_CODE'  
                            PRINT @sqltext 
                            EXEC(@sqlText) ";

            return DBUtility.GetTable(SQL, "MES");
        }
    }
}