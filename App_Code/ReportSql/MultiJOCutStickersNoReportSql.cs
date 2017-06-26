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
    /// Summary description for MultiJOCutStickersNoReportSql
    /// </summary>
    public class MultiJOCutStickersNoReportSql
    {
        public static DataTable GetMultiJoBundleTicketData(string factory, string queryString)
        {
            string sql;
            //            int i=0;
            //            int j = 1;

            //            string sqlTemplate = @" SELECT A.JOB_ORDER_NO,A.LAY_NO,C.COLOR_CD,MAX(ISNULL(E.COLOR_DESC,C.COLOR_CD)) AS COLOR_DESC, B.SIZE_CD + CASE WHEN ISNULL(B.SIZE_CD2,'')='-' THEN  '' ELSE ' ' + B.SIZE_CD2 END  AS SIZE_CD 
            //            ,SUM(C.PLYS) AS PLYS,SUM(C.PLYS*B.RATIO) AS QTY ,{3} as seq  FROM CUT_LAY A INNER JOIN CUT_LAY_HD B
            //            ON A.lAY_ID=B.LAY_ID INNER JOIN CUT_LAY_DT C
            //            ON B.LAY_TRANS_ID=C.LAY_TRANS_ID
            //            INNER JOIN JO_HD D ON D.JO_NO=A.JOB_ORDER_NO
            //            LEFT JOIN SC_COLOR E ON D.SC_NO=E.SC_NO AND E.COLOR_CODE=C.COLOR_CD
            //            WHERE A.JOB_ORDER_NO = '{0}' AND A.LAY_NO>={1} AND A.LAY_NO<={2}
            //            AND A.PRINT_STATUS='Y'
            //            GROUP BY A.JOB_ORDER_NO,A.LAY_NO,C.COLOR_CD,B.SIZE_CD + CASE WHEN ISNULL(B.SIZE_CD2,'')='-' THEN  '' ELSE ' ' + B.SIZE_CD2 END";

            //            string builtSql = "";


            //            for (i = 0; i < queryString.Length; i++)
            //            {
            //                string temp = queryString[i];
            //                string[] query = temp.Split('-');
            //                string sql = string.Format(sqlTemplate, query[0], String.IsNullOrEmpty(query[1]) ? Int32.MinValue.ToString() : query[1], string.IsNullOrEmpty(query[2]) ? Int32.MaxValue.ToString() : query[2],j);
            //                builtSql += builtSql == "" ? sql : " UNION ALL " + sql;
            //                j++;
            //            }

            //            builtSql = "SELECT * FROM (" + builtSql + ") A ORDER BY COLOR_CD,seq,LAY_NO";


            sql = "EXEC Proc_Gen_Multi_Cut_Ticket_syl '" + factory + "','" + queryString + "'";
            return DBUtility.GetTable(sql, "MES");

        }
    }
}