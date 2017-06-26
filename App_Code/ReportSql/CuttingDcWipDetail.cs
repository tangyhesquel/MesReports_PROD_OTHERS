using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.Common;
using System.Data;

namespace MESComment
{

/// <summary>
///CuttingDcWipDetail 的摘要说明
/// </summary>
    public class CuttingDcWipDetail
    {

        public static DataTable GetProductionLineKnit()
        {
            string SQL = @" SELECT Production_line_cd FROM GEN_Production_Line WHERE factory_cd='YMG' AND Process_cd='SEW' AND GARMENT_TYPE_CD='K' AND NEW_PRODCUTION_LINE_FLAG='Y' UNION 
                            SELECT Production_line_cd='ALL'       ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable[] GetCuttingDcWIPDetailKnit(string FactoryCd, string ProcessCd,string ProductionLine,string Garment_type, string Date,DbConnection MESConn)
        {
            DataTable[] dtData = new DataTable[2];
            string SQL = "EXEC GET_DC_WIP_RPT '"+FactoryCd+"','"+ProcessCd+"','"+ProductionLine+"','"+Date+"','"+Garment_type+"'";           
           
            DataTable dt = DBUtility.GetTable(SQL, MESConn);

            dtData[0] = dt.DefaultView.ToTable(false, new string[] { "productionLine",
                                                                     "job_order_no",
                                                                     "orderQty" ,
                                                                     "CutQty",
                                                                     "DcScanInQty" ,
                                                                     "ToTalDcScanInQty" ,
                                                                     "DcBeforeJoWip" ,
                                                                     "DcScanOutQty" ,
                                                                     "TotalDcScanOutQty" ,
                                                                     "DcInSideJoWip" ,
                                                                     "DcProductionLineWip"});

            dtData[1] = dt.DefaultView.ToTable(true, new string[] { "productionLine", "DailyDemand", "DcproductionLineWIP", "CompDay" });       
              
            return dtData;
        }


        //Batch8 - MES070 by MF
        public static DataTable GetProductionLineWoven(string Factory, string process_cd, string garment_type)
        {
            //string SQL = @" SELECT Production_line_cd FROM GEN_Production_Line WHERE factory_cd='"+Factory+@"' AND Process_cd='SEW' AND GARMENT_TYPE_CD='W'  AND NEW_PRODCUTION_LINE_FLAG='Y' UNION 
            //SELECT Production_line_cd='ALL'       ";

            string SQL = "";

            if (process_cd == "ALL")
                SQL = SQL + @" SELECT distinct Production_line_cd FROM GEN_PRODUCTION_LINE WITH(NOLOCK) WHERE FACTORY_CD='" + Factory + @"'AND (LINE_TYPE='GTN' OR ISNULL(LINE_TYPE,'')='') AND PROCESS_CD='SEW' AND GARMENT_TYPE_CD='" + garment_type + "' AND ACTIVE='Y'";
            else
                SQL = SQL + @" SELECT distinct Production_line_cd FROM GEN_PRODUCTION_LINE WITH(NOLOCK) WHERE FACTORY_CD='" + Factory + @"'AND PROCESS_CD='" + process_cd + "' AND (LINE_TYPE='GTN' OR ISNULL(LINE_TYPE,'')='') AND GARMENT_TYPE_CD='" + garment_type + "' AND ACTIVE='Y'";

            SQL = SQL + " UNION SELECT Production_line_cd='ALL'";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetCuttingDcWIPDetailWoven(string FactoryCd, string ProcessCd, string ProductionLine, string Garment_type,string Process_type, string Date, DbConnection MESConn)
        {
            string SQL = "EXEC GET_DC_WIP_RPT '" + FactoryCd + "','" + ProcessCd + "','" + ProductionLine + "','" + Date + "','" + Garment_type + "','"+ Process_type +"'";           

            return DBUtility.GetTable(SQL, MESConn);
                    
         }
    }
}
