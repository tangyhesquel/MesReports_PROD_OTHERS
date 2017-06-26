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
    ///WIPReportSql 的摘要说明
    /// </summary>
    public class WIPReportSql
    {
        public static DataTable GetIE_WIP_ReportDailyExist(string FactoryCd, string BeginDate)
        {
            string SQL = " select top 1 1 from PRD_JO_DAILY_STOCK  WITH(NOLOCK) where Factory_cd='" + FactoryCd + "' and convert(char(10), trx_date,120)='" + BeginDate + "'";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetIE_WIP_Report(bool IsCbChoose, string FactoryCd, string BeginDate, string EndDate, string GarmentType, string ProcessType, string ProcessCD, string ProdFactoryCd)
        {
            DateTime date = DateTime.Parse(BeginDate);
            int Year = date.Year;
            int Month = date.Month;
            if (Month == 1)
            {
                Year--;
                Month = 12;
            }
            else
            {
                Month--;
            }

            string SQL = "";
            SQL += " if not (select object_id('Tempdb..#TMPOP')) is null  drop table #TMPOP ;";
            SQL += " if not (select object_id('Tempdb..#temp_T')) is null  drop table #temp_T ;";

            SQL += " select job_order_no,process_cd,isnull(opening_qty,0)+isnull(in_qty,0)-isnull(out_qty,0)-isnull(wastage_qty,0) as opening  ";
            SQL += " INTO #TMPOP   ";
            //SQL += " FROM        (select job_order_no,process_cd,opening_qty=max(opening_qty),in_qty=max(in_qty),sum(out_qty)as out_qty,sum(wastage_qty) as wastage_qty       ";
            SQL += " FROM        (select job_order_no, LTRIM(ISNULL(PROCESS_GARMENT_TYPE,''))+LTRIM(ISNULL(process_cd,''))+LTRIM(ISNULL(PROCESS_TYPE,''))+LTRIM(ISNULL(PROCESS_PRODUCTION_FACTORY,'')) AS process_cd, ";
            SQL += " opening_qty=sum(opening_qty),in_qty=SUM(in_qty),sum(out_qty)as out_qty,sum(wastage_qty) as wastage_qty       ";
            SQL += " FROM prd_jo_monthly_stock  WITH(NOLOCK)      ";
            SQL += " 	WHERE Factory_cd='" + FactoryCd + "' ";
            
            SQL += " 		AND year='" + Year + "' ";
            SQL += " 		AND mon='" + Month + "' ";
            if (IsCbChoose)
            {
            }
            else
            {
                SQL += " 		AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK) ";
                SQL += " 	WHERE factory_cd='" + FactoryCd + "'  ";
                SQL += " 		AND END_PROCESS_FLAG='Y' ";
                SQL += "        AND Garment_Type=prd_jo_monthly_stock.Process_garment_Type  "; //Added By ZouShiChang ON 2013.08.22 MES 024
                SQL += " 		AND prc_cd=prd_jo_monthly_stock.process_cd)         ";
            }
            //Added By ZouShiChang ON 2013.09.20 Start MES024
            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND PROCESS_garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND process_production_factory='" + ProdFactoryCd + "' ";
            }
            //Added By ZouShiChang ON 2013.09.20 End MES024
            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " 		AND process_cd='" + ProcessCD + "'";
            }
            SQL += "       group by  job_order_no,PROCESS_GARMENT_TYPE,process_cd,PROCESS_TYPE,PROCESS_PRODUCTION_FACTORY) ff         ";
            SQL += " 	WHERE isnull(opening_qty,0)+isnull(in_qty,0)-isnull(out_qty,0)-isnull(wastage_qty,0)<>0  ; ";

            SQL += " select m.job_order_no,buyer=z.name+'('+z.customer_cd+')',x.style_no,x.sc_no,";
            SQL += " 		sam=(select sum(sam) FROM sc_sam WHERE type='S' AND sc_no=x.sc_no) ,x.wash_type_cd,";
            SQL += " 		sah=(select sum(sah) FROM sc_sam 	WHERE type='S' AND sc_no=x.sc_no), ";
            SQL += " 		bpd=convert(varchar(20),x.buyer_po_del_date,101),";
            SQL += " 		ppcd=convert(varchar(20),x.prod_completion_date,101),";
            SQL += " 		order_qty=(select sum(qty) FROM jo_dt WHERE jo_no=x.jo_no),  ";
            SQL += " 		m.process_cd,m.opening,";
            SQL += " 		m.in_qty,";
            SQL += " 		m.pullin_qty,";
            SQL += " 		m.out_qty,";
            SQL += " 		m.pullout_qty,";
            SQL += " 		m.dis_qty,";
            SQL += " 		wip_qty=isnull(m.opening,0)+isnull(m.in_qty,0)+isnull(m.pullin_qty,0)-isnull(m.out_qty,0)-isnull(m.pullout_qty,0)-isnull(m.dis_qty,0)  ";
            SQL += "    INTO #temp_T ";
            SQL += " FROM  ";
            SQL += " (";
            SQL += " select job_order_no,process_cd,sum(isnull(opening,0)) as opening,";
            SQL += " 		sum(isnull(in_qty,0)) as in_qty,sum(isnull(pullin_qty,0)) as pullin_qty,";
            SQL += " 		sum(isnull(out_qty,0)) as out_qty,sum(isnull(pullout_qty,0)) as pullout_qty,sum(isnull(dis_qty,0)) as dis_qty";
            SQL += " from (";
            SQL += " select op.job_order_no,op.process_cd,op.opening,0 as in_qty,0 as pullin_qty,0 as out_qty,0 as pullout_qty,0 as dis_qty";
            SQL += " FROM ";
            SQL += " #TMPOP op ";
            SQL += " union all";
            SQL += " select Job_order_no, LTRIM(ISNULL(PROCESS_GARMENT_TYPE,''))+LTRIM(ISNULL(process_cd,''))+LTRIM(ISNULL(PROCESS_TYPE,''))+LTRIM(ISNULL(PROCESS_PRODUCTION_FACTORY,'')) AS process_cd, ";
            //SQL += " 0 as opening,0 as in_qty,0 as pullin_qty,sum(isnull(output_qty,0)+isnull(pullout_qty,0)) as out_qty, sum(isnull(pullout_qty,0)) as pullout_qty,0 as dis_qty";
            //对Peer的数据进行特殊处理
            SQL += " 0 as opening,CASE WHEN B.FACTORY_CD<>B.CO_FACTORY_CD AND PROCESS_TYPE='P' THEN SUM(ISNULL(output_qty, 0) + ISNULL(pullout_qty,0))  ELSE 0 END  AS  in_qty , ";
            SQL += " 0 as pullin_qty,sum(isnull(output_qty,0)+isnull(pullout_qty,0)) as out_qty, sum(isnull(pullout_qty,0)) as pullout_qty,0 as dis_qty";

            SQL += " 		FROM prd_jo_output_trx AS A  WITH(NOLOCK) LEFT JOIN JO_HD AS B ON A.JOB_ORDER_NO=B.JO_NO            ";
            SQL += " 		WHERE A.Factory_cd='" + FactoryCd + "' ";
            SQL += " 			AND convert(char(10),trx_date,120) >='" + BeginDate + "' ";
            SQL += " 			AND convert(char(10),trx_date,120)<='" + EndDate + "' ";
            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " 			AND process_cd='" + ProcessCD + "'";
            }
            //Added By ZouShiChang ON 2013.09.20 Start MES024
            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND process_garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND process_production_factory='" + ProdFactoryCd + "' ";
            }
            //Added By ZouShiChang ON 2013.09.20 End MES024
            if (IsCbChoose)
            {
            }
            else
            {
                SQL += " 		AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK) ";
                SQL += " 	WHERE factory_cd='" + FactoryCd + "'  ";
                SQL += " 		AND END_PROCESS_FLAG='Y' ";
                SQL += "        AND Garment_Type=A.Process_garment_Type  "; //Added By ZouShiChang ON 2013.08.22 MES 024
                SQL += " 		AND prc_cd=A.process_cd)         ";
            }
            SQL += " 	group by B.FACTORY_CD,job_order_no,PROCESS_GARMENT_TYPE,process_cd,PROCESS_TYPE,PROCESS_PRODUCTION_FACTORY,B.CO_FACTORY_CD";
            SQL += " union all";
            SQL += " select a.Job_order_no, LTRIM(ISNULL(b.NEXT_PROCESS_GARMENT_TYPE,''))+LTRIM(ISNULL(b.next_process_cd,''))+LTRIM(ISNULL(NEXT_PROCESS_TYPE,''))+LTRIM(ISNULL(NEXT_PROCESS_PRODUCTION_FACTORY,'')) AS  next_process_cd, ";
            SQL += " 0 as opening,sum(isnull(output_qty,0))  as in_qty,0 as pullin_qty,0 as out_qty,0 as pullout_qty,0 as dis_qty";
            SQL += " 		FROM prd_jo_output_trx a WITH(NOLOCK),prd_jo_output_hd b WITH(NOLOCK)             ";
            SQL += " 		WHERE  a.doc_no=b.doc_no              ";
            SQL += " AND a.Factory_cd='" + FactoryCd + "' "; 			
            SQL += " 			AND b.trx_date >= '" + BeginDate + "' ";
            SQL += " 			AND b.trx_date <= '" + EndDate + "' ";
            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " 			AND next_process_cd = '" + ProcessCD + "'";
            }
            //Added By ZouShiChang ON 2013.09.20 Start MES024
            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND next_process_garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND next_PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND next_process_production_factory='" + ProdFactoryCd + "' ";
            }
            //Added By ZouShiChang ON 2013.09.20 End MES024
            if (IsCbChoose)
            {
            }
            else
            {
                SQL += " 		AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK) ";
                SQL += " 	WHERE factory_cd='" + FactoryCd + "'  ";
                SQL += " 		AND END_PROCESS_FLAG='Y' ";
                SQL += "          AND Garment_type=b.next_process_garment_type "; //Added By ZouShiChang ON 2013.08.22 MES 024
                SQL += " 		AND prc_cd=b.next_process_cd)         ";
            }
            SQL += " 		group by a.job_order_no,b.NEXT_PROCESS_GARMENT_TYPE,b.next_process_cd,b.NEXT_PROCESS_TYPE,b.NEXT_PROCESS_PRODUCTION_FACTORY             ";
            SQL += " 		union all";
            SQL += " 		SELECT JO_NO AS JOB_ORDER_NO,LTRIM(ISNULL(a.garment_type,''))+LTRIM(ISNULL(a.process_cd,''))+LTRIM(ISNULL(PROCESS_TYPE,''))+LTRIM(ISNULL(PRODUCTION_FACTORY,'')) AS next_process_cd, ";
            SQL += " 0 as opening,0 as in_qty,sum(isnull(C.PULLOUT_QTY,0)) as pullin_qty,0 as out_qty,0 as pullout_qty,0 as dis_qty";
            SQL += " 		FROM PRD_JO_PULLOUT c WITH(NOLOCK)      ";
            SQL += " 		INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_TRX B WITH(NOLOCK)      ";
            SQL += " 		ON b.trx_id=c.trx_id      ";
            SQL += " 		INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_HD A WITH(NOLOCK)      ";
            SQL += " 		ON A.DOC_NO=B.DOC_NO  AND A.FACTORY_CD=B.FACTORY_CD AND A.TRX_DATE=B.TRX_DATE       ";
            SQL += " 		WHERE  A.FACTORY_CD='" + FactoryCd + "'  ";
            SQL += " 		AND a.status='C'      ";
            SQL += " 		AND convert(char(10),a.trx_date,120) >= '" + BeginDate + "' ";
            SQL += " 		AND convert(char(10),a.trx_date,120) <= '" + EndDate + "'";
            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " 		AND process_cd='" + ProcessCD + "'";
            }
            //Added By ZouShiChang ON 2013.09.20 Start MES024
            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND production_factory='" + ProdFactoryCd + "' ";
            }
            //Added By ZouShiChang ON 2013.09.20 End MES024
            if (IsCbChoose)
            {
            }
            else
            {
                SQL += " 		AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK) ";
                SQL += " 	WHERE factory_cd='" + FactoryCd + "'  ";
                SQL += " 		AND END_PROCESS_FLAG='Y' ";
                SQL += "          AND Garment_type=A.Garment_type ";  //Added By ZouShiChang ON 2013.08.22 MES 024
                SQL += " 		AND prc_cd=A.process_cd)         ";
            }
            SQL += " 		GROUP BY JO_NO,a.GARMENT_TYPE,a.process_cd,a.PROCESS_TYPE,a.PRODUCTION_FACTORY ";
            SQL += " 		union all";
            SQL += " 	select a.job_order_no,LTRIM(ISNULL(a.garment_type,''))+LTRIM(ISNULL(b.prc_cd,''))+LTRIM(ISNULL(a.process_type,''))+LTRIM(ISNULL(PRODUCTION_FACTORY,'')) AS process_cd, ";
            SQL += " 0 as opening,sum(a.QTY) as in_qty ,0 as pullin_qty,0 as out_qty,0 as pullout_qty,0 as dis_qty";
            SQL += " 	FROM cut_bundle_hd a WITH(NOLOCK)               ";
            SQL += " 	inner join GEN_PRC_CD_MST b on a.factory_cd = b.factory_cd AND b.display_seq = 1 ";
            if (!ProcessCD.Equals("ALL"))
            {
                SQL += "    AND B.prc_cd ='" + ProcessCD + "'";
            }
            SQL += " 	inner join jo_hd c WITH(NOLOCK) on c.jo_no=a.job_order_no ";
            if (!FactoryCd.Equals("MDN"))
            {
                SQL += "  AND (c.garment_type_cd=b.garment_type)                   ";
            }
            SQL += " 	WHERE A.Factory_cd='" + FactoryCd + "'                  ";
            SQL += " 		AND actual_print_date >='" + BeginDate + "' ";
            SQL += " 		AND actual_print_date <DATEADD(d,1,'" + EndDate + "') ";
            //Added By ZouShiChang ON 2013.09.20 Start MES024
            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND A.garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND A.PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND production_factory='" + ProdFactoryCd + "' ";
            }
            //Added By ZouShiChang ON 2013.09.20 End MES024
            if (IsCbChoose)
            {
            }
            else
            {
                SQL += " 		AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK) ";
                SQL += " 	WHERE factory_cd='" + FactoryCd + "'  ";
                SQL += " 		AND END_PROCESS_FLAG='Y' ";
                SQL += "          AND garment_type=B.Garment_type  "; //Added By ZouShiChang ON 2013.08.22 MES 024
                SQL += " 		AND prc_cd=B.prc_cd)         ";
            }
            SQL += " 	group by A.job_order_no,a.GARMENT_TYPE,B.prc_cd,a.PROCESS_TYPE,a.PRODUCTION_FACTORY	";
            SQL += " union all";
            SQL += " select B.JOB_ORDER_NO,LTRIM(ISNULL(a.GARMENT_TYPE,''))+LTRIM(ISNULL(A.PROCESS_CD,''))+LTRIM(ISNULL(PROCESS_TYPE,''))+LTRIM(ISNULL(PRODUCTION_FACTORY,'')) AS PROCESS_CD, ";
            SQL += " 0 as opening,0 as in_qty,0 as pullin_qty,0 as out_qty,sum(isnull(b.PULLOUT_QTY,0)) AS pullout_qty,SUM(ISNULL(b.DISCREPANCY_QTY,0)) as dis_qty                ";
            SQL += " 	FROM PRD_JO_DISCREPANCY_PULLOUT_HD a WITH(NOLOCK),PRD_JO_DISCREPANCY_PULLOUT_TRX b  WITH(NOLOCK)              ";
            SQL += " 		WHERE a.factory_cd='" + FactoryCd + "' ";
            SQL += " 			AND a.doc_no=b.doc_no ";
            SQL += " 			AND convert(char(10),a.trx_date,120) >= '" + BeginDate + "' ";
            SQL += " 			AND convert(char(10),a.trx_date,120)<='" + EndDate + "' ";
            SQL += " 			AND a.status='C' ";
            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " 			AND process_cd='" + ProcessCD + "'";
            }
            //Added By ZouShiChang ON 2013.09.20 Start MES024
            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND production_factory='" + ProdFactoryCd + "' ";
            }
            //Added By ZouShiChang ON 2013.09.20 End MES024
            if (IsCbChoose)
            {
            }
            else
            {
                SQL += " 		AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK) ";
                SQL += " 	WHERE factory_cd='" + FactoryCd + "'  ";
                SQL += " 		AND END_PROCESS_FLAG='Y' ";
                SQL += "          AND garment_type=A.Garment_Type  ";
                SQL += " 		AND prc_cd=A.process_cd)         ";
            }
            SQL += " 		group by B.JOB_ORDER_NO,a.GARMENT_TYPE,A.PROCESS_CD,a.PROCESS_TYPE,a.PRODUCTION_FACTORY  ";
            SQL += " 	) t where 1=1 group by job_order_no,process_cd) m,";
            SQL += " jo_hd x,gen_customer z   ";
            SQL += " 	WHERE m.job_order_no=x.jo_no ";
            SQL += " 		AND x.customer_cd=z.customer_cd  ";
            SQL += " AND CASE WHEN SUBSTRING(M.PROCESS_CD,5,1)='P' THEN X.CO_FACTORY_CD ELSE '" + FactoryCd + "' END ='" + FactoryCd + "'  "; //Added By ZouShiChang On 2014.01.24
            SQL += " ORDER BY m.process_cd,m.job_order_no ;";

            SQL += " SELECT * FROM #temp_T ";
            SQL += "  WHERE (opening<>0 OR in_qty<>0 OR pullin_qty<>0 OR out_qty<>0 OR pullout_qty<>0 OR dis_qty<>0) ";
            SQL += " UNION ALL";
            SQL += " SELECT NULL AS job_order_no,NULL AS buyer,NULL AS style_no,NULL AS sc_no,NULL AS sam,NULL AS wash_type_cd,NULL AS sah,NULL AS bpd,'Total:' AS ppcd,";
            SQL += " SUM(ISNULL(order_qty,0)) AS order_qty,'' AS process_cd,";
            SQL += " SUM(ISNULL(opening,0)) AS opening,SUM(ISNULL(in_qty,0)) AS in_qty,";
            SQL += " SUM(ISNULL(pullin_qty,0)) AS pullin_qty,SUM(ISNULL(out_qty,0)) AS out_qty,";
            SQL += " SUM(ISNULL(pullout_qty,0)) AS pullout_qty,SUM(ISNULL(dis_qty,0)) AS dis_qty,SUM(ISNULL(wip_qty,0)) AS wip_qty";
            SQL += " FROM #temp_T ";
            DataTable dt = DBUtility.GetTable(SQL, "MES");
            return dt;
        }

        //<added by ZouShCh at:2013-03-28修改成从PRD_JO_DAILY_STOCK拿数Start>
        public static DataTable GetIE_WIP_ReportDaily(bool IsCbChoose, string FactoryCd, string BeginDate, string EndDate, string GarmentType, string ProcessType, string ProcessCD, string ProdFactoryCd)
        {
            DateTime date = DateTime.Parse(BeginDate);
            int Year = date.Year;
            int Month = date.Month;
            if (Month == 1)
            {
                Year--;
                Month = 12;
            }
            else
            {
                Month--;
            }

            string SQL = "";
            SQL += " if not (select object_id('Tempdb..#TMPOP')) is null  drop table #TMPOP ;";
            SQL += " if not (select object_id('Tempdb..#temp_T')) is null  drop table #temp_T ;";

            SQL += " select job_order_no, LTRIM(ISNULL(process_garment_type,''))+LTRIM(ISNULL(process_cd,''))+LTRIM(ISNULL(process_type,''))+LTRIM(ISNULL(process_production_factory,'')) AS PROCESS_CD ,";
            SQL += " opening_qty as opening";

            SQL += " INTO #TMPOP   ";
            SQL += " FROM        (select A.*,B.opening_qty from (select job_order_no,process_cd,process_garment_type,process_type,process_production_factory,in_qty=sum(in_qty),sum(out_qty)as out_qty,sum(wastage_qty) as wastage_qty       ";
            SQL += " FROM PRD_JO_DAILY_STOCK  WITH(NOLOCK)      ";
            SQL += " 	WHERE Factory_cd='" + FactoryCd + "' ";
            if (IsCbChoose)
            {
            }
            else
            {
                SQL += " 		AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK) ";
                SQL += " 	WHERE factory_cd='" + FactoryCd + "'  ";
                SQL += " 		AND END_PROCESS_FLAG='Y' ";
                SQL += "          AND garment_type=PRD_JO_DAILY_STOCK.Process_garment_type  ";
                SQL += " 		AND prc_cd=PRD_JO_DAILY_STOCK.process_cd)         ";
            }

            SQL += " and convert(char(10),trx_date,120)>='" + BeginDate + "' AND convert(char(10),trx_date,120)<='" + EndDate + "'";

            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND Process_garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND process_production_factory='" + ProdFactoryCd + "' ";
            }

            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " 		AND process_cd='" + ProcessCD + "'";
            }
            SQL += "       group by  job_order_no,process_cd,process_garment_type,process_type,process_production_factory) A         ";
            SQL += "    left join ( select job_order_no,Process_cd,Process_garment_type,process_type,process_production_factory,sum(opening_qty) as opening_qty ";
            SQL += " from PRD_JO_DAILY_STOCK where Factory_cd='" + FactoryCd + "'";
            if (IsCbChoose)
            {
                SQL += " AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK) ";
                SQL += " WHERE factory_cd='" + FactoryCd + "' AND END_PROCESS_FLAG='Y' ";
                SQL += "  AND prc_cd=PRD_JO_DAILY_STOCK.process_cd AND garment_type=PRD_JO_DAILY_STOCK.PROCESS_GARMENT_TYPE  ) ";

            }
            SQL += " and convert(char(10),trx_date,120)='" + BeginDate + "'";
            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND Process_garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND process_production_factory='" + ProdFactoryCd + "' ";
            }

            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " AND process_cd='" + ProcessCD + "'";
            }
            SQL += " group by  job_order_no,process_cd,process_garment_type,process_type,process_production_factory";
            SQL += " ) B on A.job_order_no=B.Job_order_no and A.Process_cd=B.Process_cd  AND A.process_garment_type=B.process_garment_type AND a.process_type=b.process_type AND  A.process_production_factory=b.process_production_factory ) ff ";
            SQL += " select m.job_order_no,buyer=z.name+'('+z.customer_cd+')',x.style_no,x.sc_no,";
            SQL += " 		sam=(select sum(sam) FROM sc_sam WHERE type='S' AND sc_no=x.sc_no) ,x.wash_type_cd,";
            SQL += " 		sah=(select sum(sah) FROM sc_sam 	WHERE type='S' AND sc_no=x.sc_no), ";
            SQL += " 		bpd=convert(varchar(20),x.buyer_po_del_date,101),";
            SQL += " 		ppcd=convert(varchar(20),x.prod_completion_date,101),";
            SQL += " 		order_qty=(select sum(qty) FROM jo_dt WHERE jo_no=x.jo_no),  ";
            SQL += " 		m.process_cd,m.opening,";
            SQL += " 		m.in_qty,";
            SQL += " 		m.pullin_qty,";
            SQL += " 		m.out_qty,";
            SQL += " 		m.pullout_qty,";
            SQL += " 		m.dis_qty,";
            SQL += " 		wip_qty=isnull(m.opening,0)+isnull(m.in_qty,0)+isnull(m.pullin_qty,0)-isnull(m.out_qty,0)-isnull(m.pullout_qty,0)-isnull(m.dis_qty,0)  ";
            SQL += "    INTO #temp_T ";
            SQL += " FROM  ";
            SQL += " (";
            SQL += " select job_order_no,process_cd,sum(isnull(opening,0)) as opening,";
            SQL += " 		sum(isnull(in_qty,0)) as in_qty,sum(isnull(pullin_qty,0)) as pullin_qty,";
            SQL += " 		sum(isnull(out_qty,0)) as out_qty,sum(isnull(pullout_qty,0)) as pullout_qty,sum(isnull(dis_qty,0)) as dis_qty";
            SQL += " from (";
            SQL += " select op.job_order_no,op.process_cd,op.opening,0 as in_qty,0 as pullin_qty,0 as out_qty,0 as pullout_qty,0 as dis_qty";
            SQL += " FROM ";
            SQL += " #TMPOP op ";
            SQL += " union all";
            SQL += " select Job_order_no,LTRIM(ISNULL(PROCESS_GARMENT_TYPE,''))+LTRIM(ISNULL(process_cd,''))+LTRIM(ISNULL(PROCESS_TYPE,''))+LTRIM(ISNULL(PROCESS_PRODUCTION_FACTORY,'')) AS PROCESS_CD , ";
            //SQL += " 0 as opening,0 as in_qty,0 as pullin_qty,sum(isnull(output_qty,0)+isnull(pullout_qty,0)) as out_qty, sum(isnull(pullout_qty,0)) as pullout_qty,0 as dis_qty";
            //对Peer厂的数据进行特殊处理
            SQL += " 0 as opening, CASE WHEN B.Factory_cd<>B.CO_Factory_cd AND PROCESS_TYPE='P' THEN SUM(ISNULL(output_qty, 0) + ISNULL(pullout_qty,0))  ELSE 0 END  AS  in_qty , ";
            SQL += " 0 as pullin_qty,sum(isnull(output_qty,0)+isnull(pullout_qty,0)) as out_qty, sum(isnull(pullout_qty,0)) as pullout_qty,0 as dis_qty";
            SQL += " 		FROM prd_jo_output_trx AS A WITH(NOLOCK) LEFT JOIN JO_HD AS B ON A.JOB_ORDER_NO=B.JO_NO             ";
            SQL += " 		WHERE A.Factory_cd='" + FactoryCd + "' ";
            SQL += " 			AND convert(char(10),trx_date,120) >='" + BeginDate + "' ";
            SQL += " 			AND convert(char(10),trx_date,120)<='" + EndDate + "' ";

            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND Process_garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND process_production_factory='" + ProdFactoryCd + "' ";
            }

            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " 			AND process_cd='" + ProcessCD + "'";
            }
            if (IsCbChoose)
            {
            }
            else
            {
                SQL += " 		AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK) ";
                SQL += " 	WHERE factory_cd='" + FactoryCd + "'  ";
                SQL += " 		AND END_PROCESS_FLAG='Y' ";
                SQL += "          AND Garment_type=A.PROCESS_GARMENT_TYPE ";
                SQL += " 		AND prc_cd=A.process_cd)         ";
            }
            SQL += " 	group by B.factory_cd,job_order_no,process_cd,PROCESS_GARMENT_TYPE,PROCESS_TYPE,PROCESS_PRODUCTION_FACTORY,B.CO_factory_cd ";
            SQL += " union all";
            SQL += " select a.Job_order_no,  LTRIM(ISNULL(B.NEXT_PROCESS_GARMENT_TYPE,''))+LTRIM(ISNULL(b.next_process_cd,''))+LTRIM(ISNULL(B.NEXT_PROCESS_TYPE,''))+LTRIM(ISNULL(B.NEXT_PROCESS_PRODUCTION_FACTORY,'')) AS NEXT_PROCESS_CD , ";
            SQL += " 0 as opening,sum(isnull(output_qty,0))  as in_qty,0 as pullin_qty,0 as out_qty,0 as pullout_qty,0 as dis_qty";
            SQL += " 		FROM prd_jo_output_trx a WITH(NOLOCK),prd_jo_output_hd b WITH(NOLOCK)             ";
            SQL += " 		WHERE a.Factory_cd='" + FactoryCd + "' ";
            SQL += " 			AND a.doc_no=b.doc_no              ";
            SQL += " 			AND convert(char(10),a.trx_date,120) >= '" + BeginDate + "' ";
            SQL += " 			AND convert(char(10),a.trx_date,120)<= '" + EndDate + "' ";

            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND next_Process_garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND next_PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND next_process_production_factory='" + ProdFactoryCd + "' ";
            }

            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " 			AND next_process_cd = '" + ProcessCD + "'";
            }
            if (IsCbChoose)
            {
            }
            else
            {
                SQL += " 		AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK) ";
                SQL += " 	WHERE factory_cd='" + FactoryCd + "'  ";
                SQL += " 		AND END_PROCESS_FLAG='Y' ";
                SQL += "         AND GARMENT_TYPE=B.NEXT_PROCESS_GARMENT_TYPE  ";
                SQL += " 		AND prc_cd=b.next_process_cd)         ";
            }
            SQL += " 		group by a.job_order_no,b.next_process_cd,b.NEXT_PROCESS_GARMENT_TYPE,b.NEXT_PROCESS_TYPE,b.NEXT_PROCESS_PRODUCTION_FACTORY             ";
            SQL += " 		union all";
            SQL += " 		SELECT JO_NO AS JOB_ORDER_NO,LTRIM(ISNULL(GARMENT_TYPE,''))+LTRIM(ISNULL(a.process_cd,''))+LTRIM(ISNULL(PROCESS_TYPE,''))+LTRIM(ISNULL(PRODUCTION_FACTORY,'')) AS next_process_cd , ";
            SQL += " 0 as opening,0 as in_qty,sum(isnull(C.PULLOUT_QTY,0)) as pullin_qty,0 as out_qty,0 as pullout_qty,0 as dis_qty";
            SQL += " 		FROM PRD_JO_PULLOUT c WITH(NOLOCK)      ";
            SQL += " 		INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_TRX B WITH(NOLOCK)      ";
            SQL += " 		ON b.trx_id=c.trx_id      ";
            SQL += " 		INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_HD A WITH(NOLOCK)      ";
            SQL += " 		ON A.DOC_NO=B.DOC_NO  AND A.FACTORY_CD=B.FACTORY_CD AND A.TRX_DATE=B.TRX_DATE       ";
            SQL += " 		WHERE  A.FACTORY_CD='" + FactoryCd + "'  ";
            SQL += " 		AND a.status='C'      ";
            SQL += " 		AND convert(char(10),a.trx_date,120) >= '" + BeginDate + "' ";
            SQL += " 		AND convert(char(10),a.trx_date,120) <= '" + EndDate + "'";

            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND production_factory='" + ProdFactoryCd + "' ";
            }

            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " 		AND process_cd='" + ProcessCD + "'";
            }
            if (IsCbChoose)
            {
            }
            else
            {
                SQL += " 		AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK) ";
                SQL += " 	WHERE factory_cd='" + FactoryCd + "'  ";
                SQL += " 		AND END_PROCESS_FLAG='Y' ";
                SQL += "          AND garment_type=A.garment_type  ";
                SQL += " 		AND prc_cd=A.process_cd)         ";
            }
            SQL += " 		GROUP BY JO_NO,a.process_cd,A.GARMENT_TYPE,PROCESS_TYPE,PRODUCTION_FACTORY ";
            SQL += " 		union all";
            SQL += " 	select a.job_order_no,                                LTRIM(ISNULL(a.GARMENT_TYPE,''))+LTRIM(ISNULL(b.prc_cd,''))+LTRIM(ISNULL(A.PROCESS_TYPE,''))+LTRIM(ISNULL(PRODUCTION_FACTORY,'')) AS process_cd, ";
            SQL += "  0 as opening,sum(a.QTY) as in_qty ,0 as pullin_qty,0 as out_qty,0 as pullout_qty,0 as dis_qty";
            SQL += " 	FROM cut_bundle_hd a WITH(NOLOCK)               ";
            SQL += " 	inner join GEN_PRC_CD_MST b on a.factory_cd = b.factory_cd AND b.display_seq = 1 ";

            if (!ProcessCD.Equals("ALL"))
            {
                SQL += "    AND B.prc_cd ='" + ProcessCD + "'";
            }
            SQL += " 	inner join jo_hd c WITH(NOLOCK) on c.jo_no=a.job_order_no ";
            if (!FactoryCd.Equals("MDN"))
            {
                SQL += "  AND (c.garment_type_cd=b.garment_type)                   ";
            }
            SQL += " 	WHERE A.Factory_cd='" + FactoryCd + "'                  ";
            SQL += " 		AND actual_print_date >='" + BeginDate + "' ";
            SQL += " 		AND actual_print_date <DATEADD(d,1,'" + EndDate + "') ";

            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND A.garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND A.PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND production_factory='" + ProdFactoryCd + "' ";
            }

            if (IsCbChoose)
            {
            }
            else
            {
                SQL += " 		AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK) ";
                SQL += " 	WHERE factory_cd='" + FactoryCd + "'  ";
                SQL += " 		AND END_PROCESS_FLAG='Y' ";
                SQL += "        AND Garment_type=B.Garment_type  ";
                SQL += " 		AND prc_cd=B.prc_cd)         ";
            }
            SQL += " 	group by A.job_order_no,B.prc_cd,a.GARMENT_TYPE,a.PROCESS_TYPE,a.PRODUCTION_FACTORY	";
            SQL += " union all";
            SQL += " select B.JOB_ORDER_NO, LTRIM(ISNULL(GARMENT_TYPE,''))+LTRIM(ISNULL(A.PROCESS_CD,''))+LTRIM(ISNULL(PROCESS_TYPE,''))+LTRIM(ISNULL(PRODUCTION_FACTORY,'')) AS PROCESS_CD, ";
            SQL += " 0 as opening,0 as in_qty,0 as pullin_qty,0 as out_qty,sum(isnull(b.PULLOUT_QTY,0)) AS pullout_qty,SUM(ISNULL(b.DISCREPANCY_QTY,0)) as dis_qty                ";
            SQL += " 	FROM PRD_JO_DISCREPANCY_PULLOUT_HD a WITH(NOLOCK),PRD_JO_DISCREPANCY_PULLOUT_TRX b  WITH(NOLOCK)              ";
            SQL += " 		WHERE a.factory_cd='" + FactoryCd + "' ";
            SQL += " 			AND a.doc_no=b.doc_no ";
            SQL += " 			AND convert(char(10),a.trx_date,120) >= '" + BeginDate + "' ";
            SQL += " 			AND convert(char(10),a.trx_date,120)<='" + EndDate + "' ";
            SQL += " 			AND a.status='C' ";

            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND production_factory='" + ProdFactoryCd + "' ";
            }

            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " 			AND process_cd='" + ProcessCD + "'";
            }
            if (IsCbChoose)
            {
            }
            else
            {
                SQL += " 		AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK) ";
                SQL += " 	WHERE factory_cd='" + FactoryCd + "'  ";
                SQL += " 		AND END_PROCESS_FLAG='Y' ";
                SQL += "        AND garment_type=A.garment_type  ";
                SQL += " 		AND prc_cd=A.process_cd)         ";
            }
            SQL += " 		group by B.JOB_ORDER_NO, A.PROCESS_CD,a.GARMENT_TYPE,PROCESS_TYPE,PRODUCTION_FACTORY ";
            SQL += " 	) t where 1=1 group by job_order_no,process_cd) m,";
            SQL += " jo_hd x,gen_customer z   ";
            SQL += " 	WHERE m.job_order_no=x.jo_no ";
            SQL += " 		AND x.customer_cd=z.customer_cd  ";
            SQL += " AND CASE WHEN SUBSTRING(M.PROCESS_CD,5,1)='P' THEN X.CO_FACTORY_CD ELSE '" + FactoryCd + "' END ='" + FactoryCd + "'  "; //Added By ZouShiChang On 2014.01.24

            SQL += " ORDER BY m.process_cd,m.job_order_no ;";

            SQL += " SELECT * FROM #temp_T ";
            SQL += "  WHERE (opening<>0 OR in_qty<>0 OR pullin_qty<>0 OR out_qty<>0 OR pullout_qty<>0 OR dis_qty<>0) ";
            SQL += " UNION ALL";
            SQL += " SELECT NULL AS job_order_no,NULL AS buyer,NULL AS style_no,NULL AS sc_no,NULL AS sam,NULL AS wash_type_cd,NULL AS sah,NULL AS bpd,'Total:' AS ppcd,";
            SQL += " SUM(ISNULL(order_qty,0)) AS order_qty,'' AS process_cd,";
            SQL += " SUM(ISNULL(opening,0)) AS opening,SUM(ISNULL(in_qty,0)) AS in_qty,";
            SQL += " SUM(ISNULL(pullin_qty,0)) AS pullin_qty,SUM(ISNULL(out_qty,0)) AS out_qty,";
            SQL += " SUM(ISNULL(pullout_qty,0)) AS pullout_qty,SUM(ISNULL(dis_qty,0)) AS dis_qty,SUM(ISNULL(wip_qty,0)) AS wip_qty";
            SQL += " FROM #temp_T ";
            DataTable dt = DBUtility.GetTable(SQL, "MES");
            return dt;
        }
        //<added by ZouShCh at:2013-03-28修改成从PRD_JO_DAILY_STOCK拿数End>

        public static DataTable GetIE_WIP_ReportForGroupName(bool IsCbChoose, string FactoryCd, string BeginDate, string EndDate, string GarmentType, string ProcessCD, string ProcessType, string ProdFactoryCd, string GroupName)
        {
            DateTime date = DateTime.Parse(BeginDate);
            int Year = date.Year;
            int Month = date.Month;
            if (Month == 1)
            {
                Year--;
                Month = 12;
            }
            else
            {
                Month--;
            }
            string SQL = " ";
            SQL += " if not (select object_id('Tempdb..#TMPOP')) is null  drop table #TMPOP ;";
            SQL += " if not (select object_id('Tempdb..#temp_T')) is null  drop table #temp_T ;";

            SQL += " declare @s nvarchar(max) ";
            SQL += " SET @S = ''";
            SQL += " select @s=@s+system_value+','  FROM gen_system_setting  WHERE factory_cd='" + FactoryCd + "' and system_key = '" + GroupName + "';";

            SQL += " SELECT T1.JOB_ORDER_NO,T1.PROCESS_CD,T2.PRODUCTION_LINE_CD,T2.OPENING ";
            SQL += " INTO #TMPOP   ";
            SQL += " FROM (";
            SQL += " select job_order_no,process_cd ";
            SQL += " FROM";
            SQL += " (";
            SQL += " select job_order_no,process_cd,opening_qty=max(opening_qty),in_qty=max(in_qty),sum(out_qty)as out_qty,sum(wastage_qty) as wastage_qty        ";
            SQL += " FROM prd_jo_monthly_stock  WITH(NOLOCK)       	";
            SQL += " WHERE Factory_cd='" + FactoryCd + "'  		";
            if (!IsCbChoose)
            {
                SQL += " AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK) WHERE factory_cd='" + FactoryCd + "'   		";
                SQL += " AND END_PROCESS_FLAG='Y'  		AND prc_cd=prd_jo_monthly_stock.process_cd and garment_type=prd_jo_monthly_stock.process_garment_type  )";
            }
            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND Process_garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND process_production_factory='" + ProdFactoryCd + "' ";
            }


            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " 		AND process_cd='" + ProcessCD + "'";
            }
            SQL += " AND year='" + Year + "'  		AND mon='" + Month + "'";
            SQL += " GROUP BY  job_order_no,process_cd";
            SQL += " ) ff          	";
            SQL += " WHERE isnull(opening_qty,0)+isnull(in_qty,0)-isnull(out_qty,0)-isnull(wastage_qty,0)<>0 ";
            SQL += " ) AS T1";
            SQL += " LEFT JOIN";
            SQL += " (select job_order_no,process_cd,PRODUCTION_LINE_CD,OPENING_QTY as opening ";
            SQL += " from (";
            SQL += " SELECT DISTINCT FACTORY_CD,TRX_DATE,JOB_ORDER_NO,PROCESS_CD,PRODUCTION_LINE_CD,DES_PRODUCTION_LINE_CD";
            SQL += " ,SUM(ISNULL(OPENING_QTY,0)) AS OPENING_QTY";
            SQL += " FROM PRD_JO_daily_STOCK";
            SQL += " WHERE TRX_DATE = '" + BeginDate + "'";
            SQL += " AND FACTORY_CD = '" + FactoryCd + "'";
            if (!IsCbChoose)
            {
                SQL += " AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK) WHERE factory_cd='" + FactoryCd + "'   		";

                SQL += " AND END_PROCESS_FLAG='Y'  		AND prc_cd=PRD_JO_daily_STOCK.process_cd and garment_type=PRD_JO_daily_STOCK.process_garment_type )";

            }

            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND Process_garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND process_production_factory='" + ProdFactoryCd + "' ";
            }

            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " 		AND process_cd='" + ProcessCD + "'";
            }
            SQL += " AND ISNULL(OPENING_QTY,0)<>0";
            SQL += " GROUP BY FACTORY_CD,TRX_DATE,JOB_ORDER_NO,PROCESS_CD,PRODUCTION_LINE_CD,DES_PRODUCTION_LINE_CD";
            SQL += " ) t)AS T2";
            SQL += " ON T1.JOB_ORDER_NO =T2.JOB_ORDER_NO AND T1.PROCESS_CD = T2.PROCESS_CD";

            SQL += " select m.job_order_no,buyer=z.name+'('+z.customer_cd+')',x.style_no,x.sc_no,";
            SQL += " sam=(select sum(sam) FROM sc_sam WHERE type='S' AND sc_no=x.sc_no) ,x.wash_type_cd, 		";
            SQL += " sah=(select sum(sah) FROM sc_sam 	WHERE type='S' AND sc_no=x.sc_no),  		";
            SQL += " bpd=convert(varchar(20),x.buyer_po_del_date,101), 		ppcd=convert(varchar(20),x.prod_completion_date,101), 		";
            SQL += " order_qty=(select sum(qty) FROM jo_dt WHERE jo_no=x.jo_no), m.process_cd, PRODUCTION_LINE_CD, m.opening, 		m.in_qty, 		m.pullin_qty, 		m.out_qty, 		";
            SQL += " m.pullout_qty, 		m.dis_qty, 		";
            SQL += " wip_qty=isnull(m.opening,0)+isnull(m.in_qty,0)+isnull(m.pullin_qty,0)-isnull(m.out_qty,0)-isnull(m.pullout_qty,0)-isnull(m.dis_qty,0)      ";
            SQL += " INTO #temp_T  ";
            SQL += " FROM   ( ";
            SQL += " select job_order_no,process_cd,PRODUCTION_LINE_CD,sum(isnull(opening,0)) as opening, 		sum(isnull(in_qty,0)) as in_qty,sum(isnull(pullin_qty,0)) as pullin_qty, 		";
            SQL += " sum(isnull(out_qty,0)) as out_qty,sum(isnull(pullout_qty,0)) as pullout_qty,sum(isnull(dis_qty,0)) as dis_qty ";
            SQL += " FROM ( ";
            SQL += " select op.job_order_no,op.process_cd,OP.PRODUCTION_LINE_CD,op.opening,0 as in_qty,0 as pullin_qty,0 as out_qty,0 as pullout_qty,0 as dis_qty ";
            SQL += " FROM  #TMPOP op    	";
            SQL += " WHERE EXISTS (select 1 from dbo.FN_SPLIT_STRING_TB(@s,',') WHERE FNFIELD <>'' AND FNFIELD = PRODUCTION_LINE_CD)  ";
            SQL += " union all ";
            SQL += " select Job_order_no,process_cd,PRODUCTION_LINE_CD,0 as opening,0 as in_qty,0 as pullin_qty,sum(isnull(output_qty,0)+isnull(pullout_qty,0)) as out_qty, ";
            SQL += " sum(isnull(pullout_qty,0)) as pullout_qty,0 as dis_qty 		";
            SQL += " FROM prd_jo_output_trx A  WITH(NOLOCK)              		";
            SQL += " WHERE Factory_cd='" + FactoryCd + "'  			AND convert(char(10),trx_date,120) >='" + BeginDate + "'  			AND convert(char(10),trx_date,120)<='" + EndDate + "'  		";
            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " AND process_cd='" + ProcessCD + "'";
            }

            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND Process_garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND process_production_factory='" + ProdFactoryCd + "' ";
            }

            if (!IsCbChoose)
            {
                SQL += " AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK)  	WHERE factory_cd='" + FactoryCd + "'   		AND END_PROCESS_FLAG='Y'  		";
                SQL += " AND prc_cd=A.process_cd AND garment_type=A.process_garment_type and process_type=a.process_type ";
                SQL += " )         	";
            }
            SQL += " AND EXISTS (select 1 from dbo.FN_SPLIT_STRING_TB(@s,',') WHERE FNFIELD <>'' AND FNFIELD = PRODUCTION_LINE_CD)    	";
            SQL += " GROUP BY job_order_no,process_cd ,PRODUCTION_LINE_CD";
            SQL += " union all ";
            SQL += " select a.Job_order_no,b.next_process_cd,A.DES_PRODUCTION_LINE_CD,0 as opening,sum(isnull(output_qty,0))  as in_qty,0 as pullin_qty,0 as out_qty,0 as pullout_qty,0 as dis_qty 		";
            SQL += " FROM prd_jo_output_trx a WITH(NOLOCK)";
            SQL += " INNER JOIN prd_jo_output_hd b WITH(NOLOCK)	 ON a.doc_no=b.doc_no   ";
            SQL += " WHERE a.Factory_cd='" + FactoryCd + "'  AND convert(char(10),a.trx_date,120) >= '" + BeginDate + "'  			AND convert(char(10),a.trx_date,120)<= '" + EndDate + "'  		";
            if (!IsCbChoose)
            {


                SQL += " AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK)  WHERE factory_cd='" + FactoryCd + "'   		AND END_PROCESS_FLAG='Y'  		AND prc_cd=b.next_process_cd   ";
                SQL += " AND garment_type=b.next_process_garment_type ) ";

            }

            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND next_Process_garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND next_PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND next_process_production_factory='" + ProdFactoryCd + "' ";
            }

            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " AND next_process_cd = '" + ProcessCD + "'";
            }
            SQL += " AND EXISTS (select 1 from dbo.FN_SPLIT_STRING_TB(@s,',') WHERE FNFIELD <>'' AND FNFIELD = A.DES_PRODUCTION_LINE_CD)  ";
            SQL += " GROUP BY a.job_order_no,b.next_process_cd,A.DES_PRODUCTION_LINE_CD";
            SQL += " union all ";
            SQL += " SELECT JO_NO AS JOB_ORDER_NO,a.process_cd as next_process_cd,A.PRODUCTION_LINE_CD,0 as opening,0 as in_qty,sum(isnull(C.PULLOUT_QTY,0)) as pullin_qty,0 as out_qty,0 as pullout_qty,0 as dis_qty 		";
            SQL += " FROM PRD_JO_PULLOUT c WITH(NOLOCK)       		";
            SQL += " INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_TRX B WITH(NOLOCK)       		ON b.trx_id=c.trx_id       		";
            SQL += " INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_HD A WITH(NOLOCK)       		ON A.DOC_NO=B.DOC_NO  AND A.FACTORY_CD=B.FACTORY_CD AND A.TRX_DATE=B.TRX_DATE        		";
            SQL += " WHERE  A.FACTORY_CD='" + FactoryCd + "'   		AND a.status='C'       		AND convert(char(10),a.trx_date,120) >= '" + BeginDate + "'  		AND convert(char(10),a.trx_date,120) <= '" + EndDate + "' 		";
            if (!IsCbChoose)
            {
                SQL += " AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK)  	WHERE factory_cd='" + FactoryCd + "'   		AND END_PROCESS_FLAG='Y'  		AND prc_cd=A.process_cd       ";
                SQL += " AND garment_type=A.garment_Type  ) ";
            }

            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND Process_garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND process_production_factory='" + ProdFactoryCd + "' ";
            }

            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " AND A.process_cd = '" + ProcessCD + "'";
            }
            SQL += " AND EXISTS (select 1 from dbo.FN_SPLIT_STRING_TB(@s,',') WHERE FNFIELD <>'' AND FNFIELD = A.PRODUCTION_LINE_CD)    		";
            SQL += " GROUP BY JO_NO,a.process_cd,A.PRODUCTION_LINE_CD";
            SQL += " union all 	";
            SQL += " select a.job_order_no,b.prc_cd as process_cd,L.PRODUCTION_LINE_CD,0 as opening,sum(a.QTY) as in_qty ,0 as pullin_qty,0 as out_qty,0 as pullout_qty,0 as dis_qty 	";
            SQL += " FROM cut_bundle_hd a WITH(NOLOCK)                	";
            SQL += " inner join GEN_PRC_CD_MST b WITH(NOLOCK) on a.factory_cd = b.factory_cd  	";
            SQL += " inner join jo_hd c WITH(NOLOCK) on c.jo_no=a.job_order_no   ";
            if (!FactoryCd.Equals("MDN"))
            {
                SQL += "  AND (c.garment_type_cd=b.garment_type)                   ";
            }
            SQL += " INNER JOIN GEN_PRODUCTION_LINE L WITH(NOLOCK) ON B.PRC_CD = L.PROCESS_CD    	";
            SQL += " AND B.GARMENT_TYPE=L.GARMENT_TYPE_CD ";
            SQL += " WHERE A.Factory_cd='" + FactoryCd + "'                   		";
            SQL += " AND actual_print_date >='" + BeginDate + "'  		";
            SQL += " AND actual_print_date < DATEADD(d,1,'" + EndDate + "')";
            SQL += " AND b.display_seq = 1";
            if (!ProcessCD.Equals("ALL"))
            {
                SQL += "    AND B.prc_cd ='" + ProcessCD + "'";
            }

            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND Process_garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND process_production_factory='" + ProdFactoryCd + "' ";
            }

            if (!IsCbChoose)
            {
                SQL += " AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK) ";
                SQL += " WHERE factory_cd='" + FactoryCd + "'  ";
                SQL += " AND END_PROCESS_FLAG='Y' ";
                SQL += " AND Garment_type=B.garment_type and Process_type=B.Process_type ";
                SQL += " AND prc_cd=B.prc_cd) ";
            }
            SQL += " AND EXISTS (select 1 from dbo.FN_SPLIT_STRING_TB(@s,',') WHERE FNFIELD <>'' AND FNFIELD = L.PRODUCTION_LINE_CD) ";
            SQL += " GROUP BY A.job_order_no,B.prc_cd	,L.PRODUCTION_LINE_CD";
            SQL += " union all ";
            SQL += " select B.JOB_ORDER_NO,A.PROCESS_CD,A.PRODUCTION_LINE_CD,0 as opening,0 as in_qty,0 as pullin_qty,0 as out_qty,sum(isnull(b.PULLOUT_QTY,0)) AS pullout_qty,SUM(ISNULL(b.DISCREPANCY_QTY,0)) as dis_qty                 	";
            SQL += " FROM PRD_JO_DISCREPANCY_PULLOUT_HD a WITH(NOLOCK)";
            SQL += " INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_TRX b  WITH(NOLOCK)    ON      a.doc_no=b.doc_no      		";
            SQL += " WHERE a.factory_cd='" + FactoryCd + "' AND convert(char(10),a.trx_date,120) >= '" + BeginDate + "'  			AND convert(char(10),a.trx_date,120)<='" + EndDate + "'  			AND a.status='C'  		";
            if (!ProcessCD.Equals("ALL"))
            {
                SQL += " AND A.process_cd='" + ProcessCD + "'";
            }

            if (!GarmentType.Equals("ALL"))
            {
                SQL += " AND Process_garment_TYPE='" + GarmentType + "' ";
            }
            if (!ProcessType.Equals(""))
            {
                SQL += " AND PROCESS_TYPE='" + ProcessType + "' ";
            }
            if (!ProdFactoryCd.Equals(""))
            {
                SQL += " AND process_production_factory='" + ProdFactoryCd + "' ";
            }

            if (!IsCbChoose)
            {


                SQL += " AND not exists (select top 1 1 FROM dbo.GEN_PRC_CD_MST WITH(NOLOCK)  	WHERE factory_cd='" + FactoryCd + "'   		AND END_PROCESS_FLAG='Y'  	AND Garment_type=A.Garment_type	AND prc_cd=A.process_cd)          		";

            }
            SQL += " AND EXISTS (select 1 from dbo.FN_SPLIT_STRING_TB(@s,',') WHERE FNFIELD <>'' AND FNFIELD = A.PRODUCTION_LINE_CD) ";
            SQL += " GROUP BY B.JOB_ORDER_NO,A.PROCESS_CD,A.PRODUCTION_LINE_CD  	";
            SQL += " ) t ";
            SQL += " WHERE 1=1 ";
            SQL += " GROUP BY job_order_no,process_cd,PRODUCTION_LINE_CD";
            SQL += " ) m";
            SQL += " INNER JOIN jo_hd x WITH(NOLOCK) ON m.job_order_no=x.jo_no";
            SQL += " INNER JOIN gen_customer z WITH(NOLOCK)  ON x.customer_cd=z.customer_cd";

            SQL += " SELECT * FROM #temp_T";
            SQL += " UNION ALL ";
            SQL += " SELECT NULL AS job_order_no,NULL AS buyer,NULL AS style_no,NULL AS sc_no,NULL AS sam,NULL AS wash_type_cd,NULL AS sah,NULL AS bpd,'Total:' AS ppcd, SUM(ISNULL(order_qty,0)) AS order_qty,'' AS process_cd,NULL AS PRODUCTION_LINE_CD, SUM(ISNULL(opening,0)) AS opening,SUM(ISNULL(in_qty,0)) AS in_qty, SUM(ISNULL(pullin_qty,0)) AS pullin_qty,SUM(ISNULL(out_qty,0)) AS out_qty, SUM(ISNULL(pullout_qty,0)) AS pullout_qty,SUM(ISNULL(dis_qty,0)) AS dis_qty,SUM(ISNULL(wip_qty,0)) AS wip_qty ";
            SQL += " FROM #temp_T";
            DataTable dt = DBUtility.GetTable(SQL, "MES");
            return dt;
        }


        public static DataTable GetGroupTable(string factoryCD)
        {
            string sql = @"SELECT SYSTEM_KEY,SYSTEM_VALUE FROM GEN_SYSTEM_SETTING 
                       where system_name='LINE_GROUP' AND FACTORY_CD='" + factoryCD + "'";
            return DBUtility.GetTable(sql, "MES");
        }

    }
}