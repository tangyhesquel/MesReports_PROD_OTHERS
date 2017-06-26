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
    ///JODNotCloseSql 的摘要说明
    /// </summary>
    public class JODNotCloseSql
    {


        public static DataTable GetAllDataByProcess(string Factory_CD, string Process_CD, string Garment_Type, string Process_Type, string Prod_Factory, string Wash_Type)
        {
            string SQL = "";
            SQL += @" EXEC [DBO].[GET_UPTODATE_COMPLETED_JO_LIST_FOR_PROCESS_CLOSE] '" + Factory_CD + "','" + Process_CD + "','" + (Garment_Type.Equals("ALL") ? "" : Garment_Type) + "','" + Process_Type + "','" + (Prod_Factory.Equals("") ? "" : Prod_Factory) + "','" + (Wash_Type.Equals("ALL") ? "" : Wash_Type) + "'  ";

            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetAllDataByProcess_dev(string Factory_CD, string Process_CD, string Garment_Type, string Process_Type, string ProdFactory, string Wash_Type)
        {

            string SQL = "";
            SQL += " IF OBJECT_ID('tempdb..#tmpjolist') IS NOT NULL      DROP TABLE #tmpjolist; ";
            SQL += " IF OBJECT_ID('tempdb..#tmpjodetail') IS NOT NULL      DROP TABLE #tmpjodetail; ";
            //获取日结的JO;
            SQL += " select distinct a.factory_cd,job_order_no,CUS.NAME,JH.WASH_TYPE_CD,0 as order_qty,0 as cut_qty into #tmpjolist  ";
            SQL += " from(  select distinct factory_cd,JOB_ORDER_NO From dbo.PRD_JO_DAILY_STOCK with (nolock)  where FACTORY_CD = '" + Factory_CD + "' ";
            SQL += " and trx_date=(SELECT CONVERT(varchar(12),MAX(CONVERT(smalldatetime,TRX_DATE,101)),101) AS MAX_TRX_DATE ";
            SQL += " FROM PRD_CLOSING_HISTORY  WHERE LEN(TRX_DATE)>7 AND FACTORY_CD = '" + Factory_CD + "' and entry_type='JOD')  ";
            SQL += " union all  ";
            SQL += " select distinct h.factory_cd,h.job_order_no  from CUT_BUNDLE_HD H with (nolock)  where h.factory_cd='" + Factory_CD + "' ";
            SQL += " and h.CREATE_DATE>=(SELECT CONVERT(varchar(12),MAX(CONVERT(smalldatetime,TRX_DATE,101)),101) AS MAX_TRX_DATE  ";
            SQL += " FROM PRD_CLOSING_HISTORY  WHERE LEN(TRX_DATE)>7 AND FACTORY_CD = '" + Factory_CD + "' and entry_type='JOD') ";
            SQL += " ) a  INNER JOIN  JO_HD JH WITH(NOLOCK) ON a.JOB_ORDER_NO = JH.JO_NO AND a.FACTORY_CD = JH.FACTORY_CD  ";
            SQL += " INNER JOIN	DBO.GEN_CUSTOMER CUS WITH(NOLOCK) ON JH.CUSTOMER_CD = CUS.CUSTOMER_CD  ";
            SQL += "  where 1=1 ";


            if (!Wash_Type.Equals("ALL"))
            {//2012-08-01添加查询条件；
                SQL += " AND EXISTS(SELECT 1 FROM JO_HD WITH(NOLOCK) WHERE JO_NO = A.JOB_ORDER_NO AND FACTORY_CD = A.FACTORY_CD ";
                switch (Wash_Type)
                {
                    case "W":
                        SQL += " AND WASH_TYPE_CD <>'NW'";
                        break;
                    case "NW":
                        SQL += " AND WASH_TYPE_CD ='NW'";
                        break;
                }
                SQL += " )";
            }
            //Order Qty
            SQL += " UPDATE #tmpjolist set order_qty=B.QTY FROM ";
            SQL += " (select jo_no,sum(qty) AS QTY  from jo_dt WITH(NOLOCK)  ";
            SQL += " WHERE exists (select top 1 1 from #tmpjolist where job_order_no= jo_dt.jo_no)  GROUP BY jo_no) B ";
            SQL += " WHERE B.jo_no=#tmpjolist.JOB_ORDER_NO; ";
            //Cut Qty
            SQL += " UPDATE #tmpjolist set cut_qty=B.CUT_QTY FROM ";
            SQL += " (select JOB_ORDER_NO,sum(qty) AS CUT_QTY FROM CUT_BUNDLE_HD WITH(NOLOCK)  ";
            SQL += " WHERE exists (select TOP 1 1 FROM #tmpjolist  WHERE FACTORY_CD=CUT_BUNDLE_HD.FACTORY_CD ";
            SQL += " and JOB_ORDER_NO=CUT_BUNDLE_HD.job_order_no) GROUP BY JOB_ORDER_NO) B ";
            SQL += " WHERE B.JOB_ORDER_NO=#tmpjolist.JOB_ORDER_NO; ";



            


            SQL += @" IF OBJECT_ID('tempdb..#TEMP_RESULT') IS NOT NULL      DROP TABLE #TEMP_RESULT;";
            SQL += @" select B.PROCESS_CD AS [Process],B.GARMENT_TYPE,B.PROCESS_TYPE,B.PRODUCTION_LINE_CD,A.JOB_ORDER_NO AS [JO#],A.NAME AS [BUYER NAME],
                            A.WASH_TYPE_CD AS [WASH TYPE]
                            ,A.ORDER_QTY AS [Order Qty],A.CUT_QTY AS [Cut Qty]  
                            ,B.IN_QTY AS [In Qty(Include Pullin)],
                            B.PULLIN_QTY AS [Pull In], B.OUT_QTY AS [Out Qty],B.PULLOUT_QTY AS [Pull Out] 
                            ,B.discrepancy_qty AS [Discrepancy],B.WIP,B.PREPROCESSSTATUS AS [PRE-PROCESS STATUS],'Pls check and do JO Close' as Remarks 
                            INTO #TEMP_RESULT
                            from #tmpjolist a 
                            inner join 
                            ( 
                            select job_order_no,process_cd,Garment_type,process_type,PRODUCTION_FACTORY,PRODUCTION_LINE_CD,sum(in_qty)+SUM(pull_in_qty) as in_qty,SUM(pull_in_qty) AS pullin_qty,SUM(OUT_QTY) AS OUT_QTY
                            ,SUM(pull_OUT_qty) AS pullout_qty,SUM(DISCREPANCY_QTY) AS DISCREPANCY_QTY,SUM(WIP) AS WIP, (SELECT DBO.FN_GET_PREPRC_STATUS(FACTORY_CD,PROCESS_CD,garment_type,process_type,JOB_ORDER_NO)) AS PREPROCESSSTATUS from PRD_JO_WIP_HD P WITH(NOLOCK) 
                            where process_cd<>'NA' and factory_cd='" + Factory_CD + @"' and exists  (select top 1 1 from #tmpjolist where job_order_no= P.job_order_no ) 
                            group by FACTORY_CD,job_order_no,process_cd,garment_type,process_type,PRODUCTION_FACTORY,PRODUCTION_LINE_CD
                            ) b 
                            ON a.job_order_no=b.job_order_no
                            inner join jo_hd M with(nolock) on a.job_order_no = M.jo_no
                            where 1=1 ";
            SQL += @"              and not exists(
                            select top 1 1 FROM PRD_JO_COMPLETE_STATUS p with(nolock) 
                            WHERE  p.FACTORY_CD=a.FACTORY_CD and p.job_order_no=a.job_order_no  and p.process_cd=B.PROCESS_CD and p.garment_type=b.garment_type and p.process_type=b.process_type    and isnull(prod_complete,'N')='Y'
                            )  ";
           
            if (!Garment_Type.Equals("ALL"))
            {
                SQL += " And B.GARMENT_TYPE='" + Garment_Type + "' ";
            }
            if (!Process_Type.Equals(""))
            {
                SQL += " And B.PROCESS_TYPE='" + Process_Type + "' ";
            }
            if (!ProdFactory.Equals(""))
            {
                SQL += " And B.PRODUCTION_FACTORY='" + ProdFactory + "' ";
            }
            
            if (!Process_CD.Equals(""))
            {
                SQL += " and B.process_cd='" + Process_CD + "'";
            }
            else if (!Garment_Type.Equals("ALL"))
            {
                SQL += " AND M.GARMENT_TYPE_CD = '" + Garment_Type + "'";
            }


            SQL += @"; DELETE FROM #TEMP_RESULT WHERE EXISTS
                          (  
							
							SELECT 1 FROM 
							(select Process,garment_type,process_type,JO#  FROM #TEMP_RESULT 				 							
                            GROUP BY Process,garment_type,process_type,JO#
                            having sum([CUT QTY])<SUM([Order Qty]) OR SUM([Out Qty]+Discrepancy+[Pull Out])<MAX([Order Qty])
                            ) R WHERE R.JO#=#TEMP_RESULT.JO# AND R.Process=#TEMP_RESULT.Process and R.garment_type=#TEMP_RESULT.Garment_type and R.PROCESS_TYPE=#TEMP_RESULT.PROCESS_TYPE
                           ); ";
            SQL += "   SELECT * FROM #TEMP_RESULT ";
            SQL += "  AS A INNER JOIN JO_HD AS PO ON A.[JO#]=PO.JO_NO ";
            SQL += "  WHERE  PO.FACTORY_CD=CASE WHEN A.PROCESS_TYPE='P' THEN po.CO_FACTORY_CD ELSE PO.FACTORY_CD END ";


            return DBUtility.GetTable(SQL, "MES");
        }

        
    }
}