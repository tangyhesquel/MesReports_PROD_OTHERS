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
    ///jobOrderDetailSql 的摘要说明

    /// </summary>
    public class jobOrderDetailSql
    {
        public static DataTable GetJobOrderGarmentType(string JoNo)
        {
            string SQL = " SELECT GARMENT_TYPE_CD FROM JO_HD WHERE JO_NO='" + JoNo + "'";
            return DBUtility.GetTable(SQL, "MES");
        }
       
        public static DataTable GetProcessCode(string factoryCd, string garmentType)
        {
            string SQL = "        SELECT null AS seq,NULL  AS DISPLAY_SEQ,'' AS SHORT_NAME,'ALL' AS PRC_CD,'' AS NM,'' AS GARMENT_TYPE UNION ";
            SQL = SQL + "         select DISPLAY_SEQ AS  seq,DISPLAY_SEQ,SHORT_NAME,PRC_CD,NM,GARMENT_TYPE  ";
            SQL = SQL + "        from gen_prc_cd_mst ";
            SQL = SQL + "        where ACTIVE='Y' and factory_cd='" + factoryCd + "'  ";
            SQL = SQL + "        and (isnull(GARMENT_TYPE,'')='' OR GARMENT_TYPE='" + garmentType + "' ) ";
            SQL = SQL + "        ORDER BY DISPLAY_SEQ ";
            return DBUtility.GetTable(SQL, "MES");
        }
        
        public static DataTable GetJobOrderDetailProcessHeadList(string factoryCd, string garmentType, string processCd)
        {
            string SQL = "         select DISPLAY_SEQ AS  seq,DISPLAY_SEQ,SHORT_NAME,PRC_CD,NM,GARMENT_TYPE  ";
            SQL = SQL + "        from gen_prc_cd_mst ";
            SQL = SQL + "        where ACTIVE='Y' AND factory_cd='" + factoryCd + "'  ";
            SQL = SQL + "        and (isnull(GARMENT_TYPE,'')='' OR GARMENT_TYPE='" + garmentType + "' ) ";
            if (!processCd.Equals("ALL"))
            {
                SQL = SQL + " and PRC_CD='" + processCd + "'";
            }
            SQL = SQL + "        ORDER BY DISPLAY_SEQ ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetJobOrderDetailResultList(string JoNo, string factoryCd, string garmentType, string processCd, string processType, string prodFactory, bool confirm, bool draft, string fromDate, string endDate)
        {
            DateTime d;

            string SQL = "         select a.DOC_NO,a.TRX_DATE,";
            SQL = SQL + " CASE WHEN confirm = 'Y' THEN c.CREATE_DATE ELSE A.CREATE_DATE END AS CREATE_DATE";
            SQL = SQL + " ,ISNULL(C.CREATE_USER_ID,D.CREATE_USER_ID) AS CREATE_USER_ID,CONVERT(NVARCHAR(100),isnull(c.SUBMIT_DATE,D.SUBMIT_DATE),120) AS 	SUBMIT_DATE,ISNULL(C.SUBMIT_USER_ID,D.SUBMIT_USER_ID) AS SUBMIT_USER_ID,A.PROCESS_TYPE,a.PROCESS_CD,a.QTY,a.CONFIRM,b.* ";
            SQL = SQL + "		from( ";
            SQL = SQL + "		select DOC_NO,dbo.DATE_FORMAT(TRX_DATE,'yyyy-mm-dd') TRX_DATE,MAX(CONVERT(NVARCHAR(100),CREATE_DATE,120)) AS CREATE_DATE, PROCESS_CD,PROCESS_GARMENT_TYPE,PROCESS_TYPE,Process_PRODUCTION_FACTORY,sum(ISNULL(complete_qty,0)) qty,'Y' confirm ";
            SQL = SQL + "        from prd_jo_output_trx   WITH(NOLOCK) WHERE 1=2  GROUP BY DOC_NO,TRX_DATE,PROCESS_CD,PROCESS_GARMENT_TYPE,PROCESS_TYPE,Process_PRODUCTION_FACTORY ";
            if (confirm)
            {
                SQL = SQL + "	 union all	select DOC_NO,dbo.DATE_FORMAT(TRX_DATE,'yyyy-mm-dd') TRX_DATE,MAX(CONVERT(NVARCHAR(100),CREATE_DATE,120)) AS CREATE_DATE, PROCESS_CD,PROCESS_GARMENT_TYPE,PROCESS_TYPE,Process_PRODUCTION_FACTORY,sum(ISNULL(complete_qty,0)) qty,'Y' confirm ";
                SQL = SQL + "        from prd_jo_output_trx   WITH(NOLOCK)";
                SQL = SQL + "        WHERE FACTORY_CD='" + factoryCd + "' ";
                if (!JoNo.Equals(""))
                {
                    SQL = SQL + "        AND JOB_ORDER_NO = '" + JoNo + "' ";
                }
                if (DateTime.TryParse(fromDate, out d))
                {
                    SQL = SQL + " AND TRX_DATE>='" + fromDate + "'";
                }
                if (DateTime.TryParse(endDate, out d))
                {
                    SQL = SQL + " AND TRX_DATE<='" + endDate + "'";
                }
                SQL = SQL + "        GROUP BY DOC_NO,TRX_DATE,PROCESS_CD,PROCESS_GARMENT_TYPE,PROCESS_TYPE,Process_PRODUCTION_FACTORY ";
            }
            if (draft)
            {
                SQL = SQL + "        union all ";
                SQL = SQL + "        select DOC_NO,dbo.DATE_FORMAT(TRX_DATE,'yyyy-mm-dd')TRX_DATE,NULL AS CREATE_DATE,PROCESS_CD,PROCESS_GARMENT_TYPE,PROCESS_TYPE,Process_PRODUCTION_FACTORY,sum(ISNULL(complete_qty,0)) qty,'N' confirm ";
                SQL = SQL + "        from prd_jo_output_dft OD   WITH(NOLOCK)";
                SQL = SQL + "        WHERE FACTORY_CD='" + factoryCd + "' ";
                if (!JoNo.Equals(""))
                {
                    SQL = SQL + "        AND JOB_ORDER_NO = '" + JoNo + "' ";
                }
                if (DateTime.TryParse(fromDate, out d))
                {
                    SQL = SQL + " AND TRX_DATE>='" + fromDate + "'";
                }
                if (DateTime.TryParse(endDate, out d))
                {
                    SQL = SQL + " AND TRX_DATE<='" + endDate + "'";
                }
                //因为有自动生成的单,所以需要过滤prd_jo_output_dft数据;
                //按照流程,confirm的时候,PRD_GARMENT_TRANSFER_DFT -(copy)->prd_jo_output_dft-(remove)->prd_jo_output_trx,也就是说,
                //prd_jo_output_dft不应该存在PRD_GARMENT_TRANSFER_DFT的DOC;
                SQL = SQL + " AND NOT EXISTS (SELECT 1 FROM PRD_GARMENT_TRANSFER_DFT WITH(NOLOCK) WHERE DOC_NO = OD.DOC_NO ";
                if (!JoNo.Equals(""))
                {
                    SQL = SQL + " AND JOB_ORDER_NO ='" + JoNo + "'";
                }
                SQL = SQL + " )";
                SQL = SQL + "        GROUP BY DOC_NO,TRX_DATE,PROCESS_CD,PROCESS_GARMENT_TYPE,PROCESS_TYPE,Process_PRODUCTION_FACTORY ";
                //新添加GTN的DFT数据;                
                SQL = SQL + "  union all ";
                SQL = SQL + " SELECT B.DOC_NO,NULL AS TRX_DATE,B.CREATE_DATE,B.PROCESS_CD,B.PROCESS_GARMENT_TYPE,B.PROCESS_TYPE,Process_PRODUCTION_FACTORY,SUM(ISNULL(A.QTY,0)) AS QTY,'N' confirm         ";
                SQL = SQL + " FROM dbo.PRD_GARMENT_TRANSFER_DFT A  WITH(NOLOCK)";
                SQL = SQL + " INNER JOIN ";
                SQL = SQL + " DBO.PRD_GARMENT_TRANSFER_HD B  WITH(NOLOCK) ON A.DOC_NO =B.DOC_NO AND B.FACTORY_CD = '" + factoryCd + "'";
                SQL = SQL + " AND B.STATUS = 'N'";
                if (!JoNo.Equals(""))
                {
                    SQL = SQL + "        AND JOB_ORDER_NO = '" + JoNo + "' ";
                }
                SQL = SQL + " GROUP BY B.DOC_NO,B.PROCESS_CD,B.PROCESS_GARMENT_TYPE,B.PROCESS_TYPE,B.CREATE_DATE,Process_PRODUCTION_FACTORY";
            }
            SQL = SQL + "        ) a left join  prd_jo_output_HD C ON  A.DOC_NO=C.DOC_NO LEFT JOIN prd_GARMENT_TRANSFER_HD D ON A.DOC_NO=D.DOC_NO , ";
            SQL = SQL + "        (select isnull(DISPLAY_SEQ,0) AS  SEQ,isNull(DISPLAY_SEQ,0) AS DISPLAY_SEQ,SHORT_NAME,PRC_CD,NM,GARMENT_TYPE,PROCESS_TYPE from gen_prc_cd_mst  WITH(NOLOCK) ";//Added By ZouShiChang ON 2013.08.29 MES024 (PROCESS_TYPE)
            SQL = SQL + "        where factory_cd='" + factoryCd + "' and (isnull(GARMENT_TYPE,'')='' OR GARMENT_TYPE='" + garmentType + "' )  )b  ";
            SQL = SQL + "        where a.process_cd=b.prc_cd  ";
            SQL = SQL + "      AND A.process_garment_type=b.garment_type  ";

            if (!processCd.Equals("ALL"))
            {
                SQL = SQL + " and a.process_cd='" + processCd + "' ";
            }
            if (!garmentType.Equals("ALL"))
            {
                SQL = SQL + " and a.process_garment_type='" + garmentType + "' ";
            }
            if (!prodFactory.Equals(""))
            {
                SQL = SQL + " and a.process_production_factory='" + prodFactory + "' ";
            }

            SQL = SQL + "        order by DOC_NO ";
            return DBUtility.GetTable(SQL, "MES");
        }
    }
}