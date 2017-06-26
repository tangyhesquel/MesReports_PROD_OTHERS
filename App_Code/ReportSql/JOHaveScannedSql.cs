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
    ///JOHaveScansSql 的摘要说明
    /// </summary>
    public class JOHaveScannedSql
    {
        public static DataTable GetHaveScannedJo(string FactoryCd, string GarmentType, string ProcessCD, string ProcessType, string ProdFactoryCd, string Date)
        {
            string SQL = "";
            //扫描后查询此单还未扫描送货的资料";
            SQL += " declare @process varchar(50)";
            SQL += " declare @factory_cd varchar(3)";
            SQL += " declare @date datetime";
            SQL += " DECLARE  @GARMENT_TYPE VARCHAR(1)";
            SQL += " set @date='" + Date + "'";
            SQL += " set @process= '" + ProcessCD + "'";
            SQL += " set @factory_cd='" + FactoryCd + "'";
            SQL += " SET @GARMENT_TYPE = '" + GarmentType + "'";


            //输入当天日期能够查询当天送货扫描的资料数量";
            SQL += " declare @table1 table(submit_date datetime, job_order_no varchar(50),orderQTY int,qty int,outQTY int ,grandTotal int,wip int)";
            SQL += " declare @table2 table(job_order_no varchar(50),qty int)";

            //out数";
            SQL += " insert into @table1(job_order_no,outQTY)";
            SQL += " select b.job_order_no,sum(b.qty) as outQTY";
            SQL += " from dbo.PRD_GARMENT_TRANSFER_HD a";
            SQL += " inner join PRD_GARMENT_TRANSFER_DFT b with(nolock) ";
            SQL += " on a.doc_no=b.doc_no ";
            SQL += " inner join PRD_BARCODE_CENTER c with(nolock) ";
            SQL += " on b.job_order_no=c.jono and b.bundle_no=c.bundleno and a.process_cd = c.process and a.PROCESS_GARMENT_TYPE=c.GARMENT_TYPE and A.PROCESS_TYPE=C.PROCESS_TYPE INNER JOIN prd_jo_output_hd D with(nolock) ON D.DOC_NO=A.DOC_NO ";
            SQL += " where a.status='C'";
            SQL += "     and D.TRX_DATE=@date ";
            SQL += " and c.status='Y'";
            if (!GarmentType.Equals("ALL"))
            {
                SQL += " And A.Process_Garment_type='" + GarmentType + "'";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " And A.PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " And A.PROCESS_PRODUCTION_FACTORY='" + ProdFactoryCd + "' ";
            }
            if (!ProcessCD.Equals(""))
            {
                SQL += " and a.process_cd=@process";
                SQL += " and a.process_Garment_Type='" + GarmentType + "'";
            }
            else if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND ISNULL(A.PROCESS_CD,'') IN (SELECT DISTINCT PRC_CD FROM GEN_PRC_CD_MST  with(nolock) WHERE GARMENT_TYPE =@GARMENT_TYPE";
                SQL += " UNION SELECT DISTINCT '' AS PRC_NO FROM GEN_PRC_CD_MST WHERE  GARMENT_TYPE =@GARMENT_TYPE)";
            }
            SQL += " and a.factory_cd=@factory_cd"; 
            SQL += " group by b.job_order_no";

            //累计数grand total";
            SQL += " update @table1 set grandTotal=isnull(b.outQty,0)";
            SQL += " from @table1 a inner join ";
            SQL += " (";
            SQL += "     select b.job_order_no,sum(b.qty) as outQTY";
            SQL += "     from dbo.PRD_GARMENT_TRANSFER_HD a";
            SQL += "     inner join PRD_GARMENT_TRANSFER_DFT b with(nolock)";
            SQL += "     on a.doc_no=b.doc_no";
            SQL += "     inner join PRD_BARCODE_CENTER c with(nolock) ";
            SQL += "     on b.job_order_no=c.jono and b.bundle_no=c.bundleno and a.process_cd = c.process and a.PROCESS_GARMENT_TYPE=c.GARMENT_TYPE and A.PROCESS_TYPE=C.PROCESS_TYPE INNER JOIN prd_jo_output_hd D with(nolock) ON D.DOC_NO=A.DOC_NO ";
            SQL += "     where a.status='C'";
            SQL += "     and D.TRX_DATE<=@date AND B.JOB_ORDER_NO IN (SELECT job_order_no FROM @table1 )";
            SQL += "     and c.status='Y'";
            if (!GarmentType.Equals("ALL"))
            {
                SQL += " And A.Process_Garment_type='" + GarmentType + "'";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " And A.PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " And A.PROCESS_PRODUCTION_FACTORY='" + ProdFactoryCd + "' ";
            }
            if (!ProcessCD.Equals(""))
            {
                SQL += " and a.process_cd=@process";                
            }
            else if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND ISNULL(A.PROCESS_CD,'') IN (SELECT DISTINCT PRC_CD FROM GEN_PRC_CD_MST  with(nolock) WHERE GARMENT_TYPE =@GARMENT_TYPE";
                SQL += " UNION SELECT DISTINCT '' AS PRC_NO FROM GEN_PRC_CD_MST WHERE  GARMENT_TYPE =@GARMENT_TYPE)";
            }
            SQL += "     and a.factory_cd=@factory_cd";
            SQL += "     group by b.job_order_no";
            SQL += " )b on a.job_order_no=b.job_order_no";
            //制单数";
            SQL += " update @table1 set orderQTY=b.qty";
            SQL += " from @table1 a inner join ";
            SQL += " (select jo_no,sum(qty) as qty ";
            SQL += " from jo_dt ";
            SQL += " group by jo_no";
            SQL += " )b";
            SQL += " on a.job_order_no=b.jo_no";

            //实裁数";
            SQL += " insert into @table2(job_order_no,qty)";
            SQL += " select a.job_order_no,sum(a.qty) as qty ";
            SQL += " from CUT_BUNDLE_HD a ";
            SQL += " inner join @table1 b";
            SQL += " on a.job_order_no=b.job_order_no";
            SQL += " and a.factory_cd=@factory_cd";
            SQL += " and (isnull(a.status,'')='' or a.status<>'N')";
            SQL += " group by a.job_order_no";

            SQL += " update @table1 set submit_date=@date,qty=b.qty,wip=b.qty-a.grandTotal";
            SQL += " from @table1 a inner join @table2 b";
            SQL += " on a.job_order_no=b.job_order_no";
            
            SQL += " select job_order_no,ORDERQTY,qty,outQTY,GRANDTOTAL,Wip from @table1";
            return DBUtility.GetTable(SQL, "MES");
        }
    }
}