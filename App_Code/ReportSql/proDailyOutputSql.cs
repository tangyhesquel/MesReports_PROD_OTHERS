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
    ///proDailyOutputSql 的摘要说明
    /// </summary>
    public class proDailyOutputSql
    {
        public static void SP_Pro_DropTmpTable(string uuidContent, string uuidTitle)
        {
            string SQL = "exec SP_Pro_DropTmpTable '" + uuidContent + "','" + uuidTitle + "' ";
            DBUtility.ExecuteNonQuery(SQL, "MES");
        }

        public static DataTable GetResultList(string ProdGroup, string factoryCd, string garmentType, string washType, string dateTime, string processCode, string processType, string prodFactory, string uuidContent, string uuidTitle, DbConnection mesConn)
        {
            if (garmentType == "")
                garmentType = "%";
            if (processCode == "")
                processCode = "%";
            if (processType == "")
                processType = "%";
            if (prodFactory == "")
                prodFactory = "%";
            if (washType.Equals(""))
            {
                washType = "%";
            }
            string SQL = "exec RPT_GMT_STOCK_DAILY '" + ProdGroup + "','" + factoryCd + "','" + garmentType + "','" + washType + "','" + dateTime + "','" + processCode + "','" + processType + "','" + prodFactory + "','" + uuidContent + "','" + uuidTitle + "';";
            SQL += " select * from " + uuidContent + " order by jo_no";
            return DBUtility.GetTable(SQL, mesConn);
        }

        public static DataTable GetcontentIndexList(string factoryCd, string garmentType, string washType, string dateTime, string processCode, string uuidContent, string uuidTitle, DbConnection mesConn)
        {
            if (garmentType == "")
                garmentType = "%";
            if (processCode == "")
                processCode = "%";
            string SQL = "select s_ID as seq,RFNames4 From " + uuidTitle + " order by S_ID";
            return DBUtility.GetTable(SQL, mesConn);
        }

        public static DataTable Getlevel1TitleList(string factoryCd, string garmentType, string washType, string dateTime, string processCode, string uuidContent, string uuidTitle, DbConnection mesConn)
        {
            if (garmentType == "")
                garmentType = "%";
            if (processCode == "")
                processCode = "%";
            string SQL = "select min(s_id) as seq,RFNames1,count(RFNAMES4) as N_NO from " + uuidTitle + " group by RFNames1 order by 1,2";
            return DBUtility.GetTable(SQL, mesConn);
        }

        public static DataTable Getlevel2TitleList(string factoryCd, string garmentType, string washType, string dateTime, string processCode, string uuidContent, string uuidTitle, DbConnection mesConn)
        {
            if (garmentType == "")
                garmentType = "%";
            if (processCode == "")
                processCode = "%";
            string SQL = "select min(s_id) as seq,RFNames1,RFNames2,count(RFNAMES4) as N_NO2 from " + uuidTitle + " group by RFNames1,RFNames2 order by 1,2,3";
            return DBUtility.GetTable(SQL, mesConn);
        }

        public static DataTable Getlevel3TitleList(string factoryCd, string garmentType, string washType, string dateTime, string processCode, string uuidContent, string uuidTitle, DbConnection mesConn)
        {
            if (garmentType == "")
                garmentType = "%";
            if (processCode == "")
                processCode = "%";
            string SQL = "select s_ID as seq,RFNames3 From " + uuidTitle + " order by 1";
            return DBUtility.GetTable(SQL, mesConn);
        }

        public static DataTable GetProcessCode(string factoryCd, string garmentCd)
        {
            //string SQL = "SELECT * FROM (";
            string SQL = "SELECT DISTINCT PROCESS_CD,DISPLAY_SEQ FROM (";
            SQL += " select PROCESS_CD='',SHORT_NAME='',DISPLAY_SEQ=0";
            SQL += " UNION ALL";
            SQL += " SELECT  DISTINCT PROCESS_CD,SHORT_NAME,M.DISPLAY_SEQ FROM PRD_fty_process_flow AS F,";
            
            SQL += " dbo.GEN_PRC_CD_MST AS M WHERE M.PRC_CD = F.PROCESS_CD  AND M.FACTORY_CD=F.FACTORY_CD AND M.GARMENT_TYPE=F.PROCESS_GARMENT_TYPE  ";
            
            SQL += " AND M.FACTORY_CD =  '" + factoryCd + "' ";
            if (garmentCd != "")
            {
                SQL += "AND M.GARMENT_TYPE ='" + garmentCd + "' ";
            }
            SQL += " ) AS T ORDER BY DISPLAY_SEQ";

            return DBUtility.GetTable(SQL, "MES");
        }

    }
}