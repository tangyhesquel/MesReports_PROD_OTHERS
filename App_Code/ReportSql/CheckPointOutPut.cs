using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web.UI.WebControls;

namespace MESComment
{
    /// <summary>
    ///CheckPointOutPut 的摘要说明 
    /// </summary>
    public class CheckPointOutPut
    {
        public static DataTable GetCPProcessList(string factoryCd, string garmentType)
        {
          
            string SQL = "select dpt_cd,MIN(DISSEQ) as DISSEQ from Prd_Output_CP_Setting_Mst ";
            SQL = SQL + " where factory_cd='" + factoryCd + "' ";
            if (garmentType != "")
            {
                SQL = SQL + " AND GMTYPE='" + garmentType + "' ";
            }
            SQL = SQL + " group by dpt_cd  ORDER BY DISSEQ";
            
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetCPOutputResultList(string factoryCd, string garmentType, string washtype, string StartDate, string ToDate, string processCode, string prodLine, string uuidContent, string uuidTitle, string uuidSummary, DbConnection conn)
        {
            string SQL = "exec SP_Pro_CheckOutput '" + factoryCd + "','" + garmentType + "','" + washtype + "','" + StartDate + "','" + ToDate + "','" + processCode + "','" + prodLine + "','" + uuidContent + "','" + uuidTitle + "','" + uuidSummary + "'; select FNAME  from " + uuidTitle + " order by DISSEQ";
            return DBUtility.GetTable(SQL, conn);
        }

        public static DataTable GetCPOutputSummary(string uuidContent, string uuidTitle, string SQLORDER)
        {
            string SQL = "        SELECT  * fROM " + uuidContent + SQLORDER;
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetCPOutputLineSummary(string uuidSummary, string uuidTitle, string SQLORDER)
        {
            string SQL = "        SELECT  * fROM " + uuidSummary;
            return DBUtility.GetTable(SQL, "MES");
        }

        public static void SP_Pro_DropTmpTable(string uuidContent, string uuidTitle)
        {
            string SQL = "exec SP_Pro_DropTmpTable '" + uuidContent + "','" + uuidTitle + "' ";
            DBUtility.ExecuteNonQuery(SQL, "MES");
        }
    }
}