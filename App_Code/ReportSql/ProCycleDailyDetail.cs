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
    /// ProCycleDailyDetail 的摘要说明
    /// </summary>
    public class ProCycleDailyDetail
    {
        public static DataTable Get_proCycleDetail(string Factory_cd, string Process_cd, string BeginDate, string EndDate, string Job_Order_NO)
        {
            Factory_cd = Factory_cd == null ? "" : Factory_cd;
            Process_cd = Process_cd == null ? "" : Process_cd;
            BeginDate = BeginDate == null ? "" : BeginDate;
            EndDate = EndDate == null ? "" : EndDate;
            Job_Order_NO = Job_Order_NO == null ? "" : Job_Order_NO;
            string SQL = "";
            SQL += " select  A.PROCESS_CD,A.PRODUCTION_LINE_CD,A.JOB_ORDER_NO,PO.STYLE_NO,PO.CUSTOMER_CD,B.Short_NAME as CUSTOMER_NAME,po.WASH_TYPE_CD ,  ";
            SQL += "        DBO.DATE_FORMAT(A.trx_date,'yyyy-mm-dd') TRX_DATE,SUM(ISNULL(OPENING_QTY,0)) OPENING_QTY,SUM(isnull(in_qty,0)) IN_QTY, ";
            SQL += "        sum(isnull(out_qty,0)) as OUT_QTY, ";
            SQL += "        SUM(ISNULL(POUT.DISCREPANCY_QTY,0)) AS DISCREPANCY_QTY,";
            SQL += "        SUM(ISNULL(POUT.PULLOUT_QTY,0)) AS PULLOUT_QTY,";
            SQL += "        SUM(isnull(opening_qty,0))+SUM(isnull(in_qty,0))-sum(isnull(out_qty,0)) - SUM(ISNULL(POUT.DISCREPANCY_QTY,0))-SUM(ISNULL(POUT.PULLOUT_QTY,0)) as WIP_QTY ";
            SQL += "        FROM (SELECT DISTINCT JOB_ORDER_NO,FACTORY_CD,DBO.DATE_FORMAT(trx_date,'yyyy-mm-dd') AS trx_date ,PROCESS_CD,PROCESS_GARMENT_TYPE,PROCESS_TYPE";//增加GARMENT_TYPE,PROCESS_TYPE列
            SQL += "        ,PRODUCTION_LINE_CD,SUM(ISNULL(OPENING_QTY,0)) AS OPENING_QTY";
            SQL += "        ,SUM(ISNULL(in_qty,0)) AS in_qty";
            SQL += "        ,SUM(ISNULL(out_qty,0)) AS out_qty";
            SQL += "        ,SUM(ISNULL(wastage_qty,0)) AS wastage_qty";
            SQL += "        FROM  prd_jo_daily_stock WHERE 1=1 ";

            SQL += "        AND Factory_cd =  '" + Factory_cd + "'";
            if (!BeginDate.Equals(""))
            {
                SQL += "        and trx_date >= '" + BeginDate + "'         ";
            }
            if (!EndDate.Equals(""))
            {
                SQL += "        and trx_date <= '" + EndDate + "'      ";
            }
            if (!Job_Order_NO.Equals(""))
            {
                SQL += "        AND JOB_ORDER_NO = '" + Job_Order_NO + "'   ";
            }
            SQL += "        group by         job_order_no,Factory_cd,PROCESS_CD,PROCESS_GARMENT_TYPE,PROCESS_TYPE,PRODUCTION_LINE_CD,      "; //Added By ZouShiChang ON 2013.08.26 MES 024 增加GARMENT_TYPE,PROCESS_TYPE列
            SQL += "        DBO.DATE_FORMAT(trx_date,'yyyy-mm-dd')    ) AS A    ";
            SQL += "        INNER JOIN JO_HD PO ON A.JOB_ORDER_NO=PO.JO_NO";
            SQL += "        INNER JOIN GEN_CUSTOMER B ON  PO.CUSTOMER_CD=B.CUSTOMER_CD";
            SQL += "        INNER JOIN GEN_PRC_CD_MST C ON C.PRC_CD=A.PROCESS_CD AND C.GARMENT_TYPE=A.PROCESS_GARMENT_TYPE  AND C.FACTORY_CD = A.FACTORY_CD AND C.END_PROCESS_FLAG IS NULL ";
            SQL += "        INNER JOIN (";
            SQL += "        SELECT DISTINCT HD.PROCESS_CD,HD.GARMENT_TYPE,HD.PROCESS_TYPE,TRX.TRX_DATE,TRX.JOB_ORDER_NO, ";//增加GARMENT_TYPE,PROCESS_TYPE列
            SQL += "        SUM(ISNULL(TRX.PULLOUT_QTY,0)) AS PULLOUT_QTY,SUM(ISNULL(TRX.DISCREPANCY_QTY,0)) AS DISCREPANCY_QTY";
            SQL += "        FROM PRD_JO_DISCREPANCY_PULLOUT_TRX TRX,PRD_JO_DISCREPANCY_PULLOUT_HD HD";
            SQL += "        WHERE 1=1";
            SQL += "        AND TRX.DOC_NO=HD.DOC_NO AND HD.FACTORY_CD = '" + Factory_cd + "'";
            if (!BeginDate.Equals(""))
            {
                SQL += "        AND TRX.trx_date >= '" + BeginDate + "' ";
            }
            if (!EndDate.Equals(""))
            {
                SQL += "        AND TRX.trx_date <= '" + EndDate + "' ";
            }
            if (!Process_cd.Equals(""))
            {
                SQL += "        AND PROCESS_CD = '" + Process_cd + "' ";
            }
            if (!Job_Order_NO.Equals(""))
            {
                SQL += "        AND TRX.JOB_ORDER_NO = '" + Job_Order_NO + "'";
            }
            SQL += "        GROUP BY JOB_ORDER_NO,PROCESS_CD,HD.GARMENT_TYPE,HD.PROCESS_TYPE,TRX.TRX_DATE";
            SQL += "        ) AS POUT ON POUT.JOB_ORDER_NO=A.JOB_ORDER_NO AND A.PROCESS_CD = POUT.PROCESS_CD AND A.PROCESS_GARMENT_TYPE=POUT.GARMENT_TYPE AND A.PROCESS_TYPE=POUT.PROCESS_TYPE";
            SQL += "        AND POUT.TRX_DATE = A.TRX_DATE";
            SQL += "        where   A.JOB_ORDER_NO=PO.JO_NO ";
            SQL += "        AND A.factory_cd = '" + Factory_cd + "'  ";
            if (!Process_cd.Equals(""))
            {
                SQL += "        and A.process_cd = '" + Process_cd + "'  ";
            }
            if (!Job_Order_NO.Equals(""))
            {
                SQL += "        and a.job_order_no = '" + Job_Order_NO + "' ";
            }
            if (!BeginDate.Equals(""))
            {
                SQL += "        and A.trx_date >= '" + BeginDate + "' ";
            }
            if (!EndDate.Equals(""))
            {
                SQL += "        and A.trx_date <= '" + EndDate + "' ";
            }
            SQL += "        group by ";
            SQL += "        A.job_order_no,PO.STYLE_NO,PO.CUSTOMER_CD,A.PROCESS_CD,A.PRODUCTION_LINE_CD,B.Short_NAME,po.WASH_TYPE_CD, ";
            SQL += "        DBO.DATE_FORMAT(A.trx_date,'yyyy-mm-dd')  ";
            SQL += "        order by ";
            SQL += "        DBO.DATE_FORMAT(A.trx_date,'yyyy-mm-dd') ";
            return DBUtility.GetTable(SQL, "MES");
        }

    }
}