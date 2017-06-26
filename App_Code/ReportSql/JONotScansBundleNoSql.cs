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
    ///JONotScansBundleNoSql 的摘要说明
    /// </summary>
    public class JONotScansBundleNoSql
    {
        public static DataTable GetNoScansBundleNo(string FactoryCd, string GarmentType, string ProcessCD,string ProcessType,string ProdFactory,string JONO)
        {           
            Random r = new Random();
            int I = r.Next(1000, 10000);
            string TempTable = "#TEMP" + I.ToString();
            string SQL = "";
            SQL += " declare @process varchar(50)";
            SQL += " declare @factory_cd varchar(3)";
            SQL += " declare @job_order_no varchar(50)";
            SQL += " declare @Garment_Type varchar(1)";
            SQL += " set @process= '" + ProcessCD + "'";
            SQL += " set @factory_cd='" + FactoryCd + "'";
            SQL += " set @job_order_no='" + JONO + "' ";
            SQL += " set @Garment_Type = '" + GarmentType + "'";

            SQL += " SELECT * INTO " + TempTable + " FROM (";
            SQL += " select DISTINCT b.jono ,c.color_desc+'('+c.color_cd+')' AS COLOR,b.bedno,b.bundleNo,sum(a.qty) as qty";
            SQL += " from CUT_BUNDLE_HD a  with(nolock)";
            SQL += " inner join PRD_BARCODE_CENTER  b  with(nolock) on a.job_order_no=b.jono and a.bundle_no=b.bundleno ";
            SQL += " inner join cut_lay_dt c  with(nolock) on a.lay_dt_id=c.lay_dt_id ";
            //SQL += " inner join cut_lay_hd d  with(nolock) on c.lay_trans_id=d.lay_trans_id ";
            //SQL += " inner join cut_lay e  with(nolock) on e.lay_id=d.lay_id and a.job_order_no=e.job_order_no";
            SQL += " where  b.status='N' and (A.STATUS ='Y')";
            
            if (!GarmentType.Equals("ALL"))
            {
                SQL += " and b.Garment_type='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " and b.Process_Type='" + ProcessType + "' ";
            }
            if (!ProdFactory.Equals(""))
            {
                SQL += " and b.PRODUCTION_FACTORY='" + ProdFactory + "' ";
            }
            
            if (!ProcessCD.Equals(""))
            {
                SQL += " and  (b.process=@process)";
            }
            else if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND ISNULL(B.PROCESS,'') IN (SELECT DISTINCT PRC_CD FROM GEN_PRC_CD_MST  with(nolock) WHERE GARMENT_TYPE =@GARMENT_TYPE";
                SQL += " UNION SELECT DISTINCT '' AS PRC_NO FROM GEN_PRC_CD_MST WHERE  GARMENT_TYPE =@GARMENT_TYPE)";
            }
            SQL += " and a.job_order_no=@job_order_no ";
            SQL += " and A.factory_cd=@factory_cd";
            SQL += " group by b.jono,b.bundleno,b.bedno,c.color_cd,c.color_desc";
            if (!ProcessCD.Equals(""))
            {
                SQL += " UNION ALL";
                SQL += " select DISTINCT b.jono,c.color_desc+'('+c.color_cd+')' AS COLOR,b.bedno,b.bundleNo,sum(a.qty) as qty";
                SQL += " from CUT_BUNDLE_HD a  with(nolock)";
                SQL += " inner join PRD_BARCODE_CENTER  b  with(nolock) on a.job_order_no=b.jono and a.bundle_no=b.bundleno ";
                SQL += " inner join cut_lay_dt c  with(nolock) on a.lay_dt_id=c.lay_dt_id ";
                //SQL += " inner join cut_lay_hd d  with(nolock) on c.lay_trans_id=d.lay_trans_id ";
                //SQL += " inner join cut_lay e  with(nolock) on e.lay_id=d.lay_id and a.job_order_no=e.job_order_no";
                SQL += " where  b.status='N' and (A.STATUS ='Y')";
                SQL += " and  (isnull(b.process,'')='' AND @process = ";
                SQL += " (SELECT PRC_CD FROM dbo.GEN_PRC_CD_MST  with(nolock) WHERE FACTORY_CD = @factory_cd ";
                SQL += " AND GARMENT_TYPE=";
                SQL += " (SELECT GARMENT_TYPE_CD FROM JO_HD  with(nolock) WHERE JO_NO = @job_order_no)";
                SQL += " AND DISPLAY_SEQ = 1))";
                SQL += " and a.job_order_no=@job_order_no ";
                SQL += " and A.factory_cd=@factory_cd";
                SQL += " group by b.jono,b.bundleno,b.bedno,c.color_cd,c.color_desc";
            }
            SQL += ") AS T";
            SQL += " ORDER BY JONO,BUNDLENO;";

            SQL += " SELECT JONO AS [JO NO] ,COLOR,BedNo,BundleNo,Qty FROM " + TempTable;
            SQL += " UNION ALL";
            SQL += " SELECT 'Total :',NULL,NULL,COUNT(1),SUM(ISNULL(QTY,0)) FROM " + TempTable;
            //Added By ZouShiChang ON 2013.09.10 End 将CUT的取数方式改为新的方式
            return DBUtility.GetTable(SQL, "MES");
        }


    }
}