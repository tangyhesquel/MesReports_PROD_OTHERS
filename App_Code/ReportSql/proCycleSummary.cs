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
    /// proCycleSummary 的摘要说明

    /// </summary>
    public class proCycleSummary
    {
        public static DataTable GetProCycleSummaryHeardList(string factoryCd)
        {
            string SQL = " select key_cd,key_name,key_seq,count(*) count from ";
            SQL = SQL + "        PRD_CT_PROCESS where factory_cd='" + factoryCd + "' GROUP BY ";
            SQL = SQL + "        key_cd,key_name,key_seq order by key_seq ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetProCycleSummaryList(string factoryCd, string garmentType, string outSouce, string washType, string startDate, string endDate, bool blCheck)
        {
            string SQL = " SELECT c.KEY_NAME AS PROCESS_CD,CT=(CASE when ";
            SQL = SQL + "		(isnull(OUTPUT_QTY,0))=0 then 0 else ";
            //Modified by Zikuan MES-069 10-Dec-13

            //Modified by MF on 20160126, change dCT formula to Day End WIP/ WIP OUT
            SQL = SQL + "		(isnull(END_WIP,0))/(isnull(OUTPUT_QTY,0)) end) FROM PRD_CT_PROCESS ";
            //SQL = SQL + "		(((isnull(WIP,0)+isnull(END_WIP,0))/2)/(isnull(OUTPUT_QTY,0))) end) FROM PRD_CT_PROCESS ";
            //End of modified by MF on 20160126, change dCT formula to Day End WIP/ WIP OUT

            //SQL = SQL + "		c left join (select KEY_NAME,WIP=sum(isnull(WIP,0)) FROM (select ";
            SQL = SQL + "		c left join (select KEY_NAME,WIP=sum(isnull(WIP,0)), END_WIP=sum(isnull(END_WIP,0)) FROM (select ";
            //SQL = SQL + "        C.KEY_NAME,sum(isnull(opening_qty,0))+sum(isnull(in_qty,0))- ";
            //SQL = SQL + "        sum(isnull(out_qty,0))-sum(isnull(wastage_qty,0)) WIP from ";
            SQL = SQL + "        C.KEY_NAME,sum(isnull(opening_qty,0)) WIP, sum(isnull(END_WIP,0)) END_WIP from ";
            //End Modified MES-069
            SQL = SQL + "        PRD_JO_DAILY_STOCK A,PRD_FTY_PROCESS_FLOW B,JO_HD ";
            SQL = SQL + "        PO,PRD_CT_PROCESS C,SC_HD SC, dbo.GEN_WASH_TYPE gwt WHERE A.PROCESS_CD=B.PROCESS_CD and A.PROCESS_GARMENT_TYPE=B.PROCESS_GARMENT_TYPE AND A.PROCESS_TYPE=B.PROCESS_TYPE AND A.PROCESS_PRODUCTION_FACTORY=B.PROCESS_PRODUCTION_FACTORY ";//By ZouShiChang ON 2013.08.29 SC_HD Change SC_HD
            SQL = SQL + "        AND A.NEXT_PROCESS_CD=B.NEXT_PROCESS_CD AND A.NEXT_PROCESS_GARMENT_TYPE=B.NEXT_PROCESS_GARMENT_TYPE AND A.NEXT_PROCESS_TYPE=B.NEXT_PROCESS_TYPE AND A.NEXT_PROCESS_PRODUCTION_FACTORY=B.NEXT_PROCESS_PRODUCTION_FACTORY AND  ";
            SQL = SQL + "        B.Corp_WIP=C.KEY_NAME AND A.JOB_ORDER_NO=PO.JO_NO  ";
            SQL = SQL + "        AND A.FACTORY_CD=CASE WHEN A.PROCESS_TYPE='P' THEN po.CO_FACTORY_CD ELSE a.FACTORY_CD END "; //Added By ZouShiChang ON 2014.02.11
            SQL = SQL + "        and a.factory_cd=c.factory_cd ";
            SQL = SQL + "        and a.factory_cd=b.factory_cd ";
            SQL = SQL + "        and gwt.wash_type_cd = po.wash_type_cd";
            SQL = SQL + "         AND SC.SC_NO=PO.SC_NO AND ";
            SQL = SQL + "        A.factory_cd='" + factoryCd + "' and trx_date >= '" + startDate + "' and ";
            SQL = SQL + "        trx_date <='" + endDate + "' ";
            //Modified by Zikuan MES-069 10-Dec-13
            SQL = SQL + "       AND A.WORK_DAY = 'Y' ";
            //End Modified MES-069
            if (garmentType != "")
            {
                SQL += "and PO.Garment_Type_Cd='" + garmentType + "'";
            }
            switch (outSouce)
            {
                case "OUTSOURCE":
                    SQL += " AND SC.SAM_GROUP_CD='OUTSOURCE'";
                    break;
                case "STANDARD":
                    SQL += " AND SC.SAM_GROUP_CD<>'OUTSOURCE'";
                    break;
            }
            switch (washType)
            {
                case "NW":
                    SQL += " AND po.WASH_TYPE_CD ='" + washType + "'";
                    break;
                case "WASH":
                    SQL += " AND (po.WASH_TYPE_CD NOT IN('NW') AND po.WASH_TYPE_CD IS NOT NULL)";
                    break;
                case "NOIRON":
                    SQL += " AND gwt.wash_grp_cd = 'WRINKLE_FREE' ";
                    break;
                case "IRON":
                    SQL += " AND ( gwt.wash_grp_cd <> 'WRINKLE_FREE')";
                    break;
            }
            SQL = SQL + "        and (B.NEXT_PROCESS_CD NOT IN('EGV_PTX','SEW_OUT','OTHER_FTY') ";
            SQL = SQL + "        or (A.PROCESS_CD='SEW' AND B.NEXT_PROCESS_CD='SEW_OUT')) group ";
            SQL = SQL + "        by C.KEY_NAME union all select ";
            //Modified By Zikuan MES-069
            //SQL = SQL + "        C.KEY_NAME,sum(isnull(opening_qty,0))+sum(isnull(in_qty,0))- ";
            //SQL = SQL + "        sum(isnull(out_qty,0))-sum(isnull(wastage_qty,0)) WIP from ";
            SQL = SQL + "        C.KEY_NAME,sum(isnull(opening_qty,0)) WIP, sum(isnull(END_WIP,0)) END_WIP from ";
            //End Modified MES-069
            SQL = SQL + "        PRD_JO_DAILY_STOCK A,PRD_FTY_PROCESS_FLOW_RPT B,JO_HD ";

            SQL = SQL + "        PO,PRD_CT_PROCESS C,SC_HD SC WHERE A.PROCESS_CD=B.PROCESS_CD AND A.PROCESS_GARMENT_TYPE=B.PROCESS_GARMENT_TYPE AND A.PROCESS_TYPE=B.PROCESS_TYPE AND A.PROCESS_PRODUCTION_FACTORY=B.PROCESS_PRODUCTION_FACTORY ";
            SQL = SQL + "        AND A.NEXT_PROCESS_CD=B.NEXT_PROCESS_CD AND isnull(A.NEXT_PROCESS_GARMENT_TYPE,'')=B.NEXT_PROCESS_GARMENT_TYPE AND isnull(A.NEXT_PROCESS_TYPE,'')=B.NEXT_PROCESS_TYPE AND A.NEXT_PROCESS_PRODUCTION_FACTORY=B.NEXT_PROCESS_PRODUCTION_FACTORY AND ";
            SQL = SQL + "        B.Corp_WIP=C.KEY_NAME AND A.JOB_ORDER_NO=PO.JO_NO  ";
            SQL = SQL + "        AND A.FACTORY_CD=CASE WHEN A.PROCESS_TYPE='P' THEN po.CO_FACTORY_CD ELSE a.FACTORY_CD END "; //Added By ZouShiChang ON 2014.02.11
            SQL = SQL + "        AND SC.SC_NO=PO.SC_NO  ";
            SQL = SQL + "        and a.factory_cd=c.factory_cd ";
            SQL = SQL + "        and a.factory_cd=b.factory_cd ";
            SQL = SQL + "        AND    A.factory_cd='" + factoryCd + "' and trx_date >= '" + startDate + "' and ";
            SQL = SQL + "        trx_date <='" + endDate + "' ";
            //Modified by Zikuan MES-069 10-Dec-13
            SQL = SQL + "       AND A.WORK_DAY = 'Y' ";
            //End Modified MES-069
            if (garmentType != "")
            {
                SQL += " and PO.Garment_Type_Cd='" + garmentType + "'";
            }
            switch (outSouce)
            {
                case "OUTSOURCE":
                    SQL += " AND SC.SAM_GROUP_CD='OUTSOURCE'";
                    break;
                case "STANDARD":
                    SQL += " AND SC.SAM_GROUP_CD<>'OUTSOURCE'";
                    break;
            }
            switch (washType)
            {
                case "NW":
                    SQL += " AND po.WASH_TYPE_CD ='" + washType + "'";
                    break;
                case "WASH":
                    SQL += " AND (po.WASH_TYPE_CD NOT IN('NW') AND po.WASH_TYPE_CD IS NOT NULL)";
                    break;
            }
            if (blCheck)
            {
                SQL += " AND exists (SELECT job_order_no, MIN(ACTUAL_PRINT_DATE) from ";
                SQL += " V_ACTUAL_CUT_BUNDLE bd WHERE  factory_cd=A.factory_cd AND job_order_no=A.JOB_ORDER_NO ";
                SQL += " GROUP BY  job_order_no ";
                SQL += " HAVING  MIN(ACTUAL_PRINT_DATE)>=DBO.DATE_FORMAT('2011-03-01','yyyy-mm-dd')) ";
            }
            SQL = SQL + "        and (B.NEXT_PROCESS_CD NOT IN('EGV_PTX','SEW_OUT','OTHER_FTY') ";
            SQL = SQL + "        or (A.PROCESS_CD='SEW' AND B.NEXT_PROCESS_CD='SEW_OUT')) group ";
            SQL = SQL + "        by C.KEY_NAME ) A WHERE 1=1 GROUP BY KEY_NAME ) a on ";
            SQL = SQL + "        c.KEY_NAME=a.KEY_NAME left join (select ";
            SQL = SQL + "        C.KEY_NAME,sum(isnull(out_qty,0)) OUTPUT_QTY from ";
            SQL = SQL + "        PRD_JO_DAILY_STOCK A,PRD_FTY_PROCESS_FLOW B,JO_HD ";
            SQL = SQL + "        PO,PRD_CT_PROCESS C,SC_HD SC WHERE A.PROCESS_CD=B.PROCESS_CD AND A.PROCESS_GARMENT_TYPE=B.PROCESS_GARMENT_TYPE AND A.PROCESS_TYPE=B.PROCESS_TYPE AND A.PROCESS_PRODUCTION_FACTORY=B.PROCESS_PRODUCTION_FACTORY ";
            SQL = SQL + "        AND A.NEXT_PROCESS_CD=B.NEXT_PROCESS_CD  AND ISNULL(A.NEXT_PROCESS_GARMENT_TYPE,'')=B.NEXT_PROCESS_GARMENT_TYPE AND ISNULL(A.NEXT_PROCESS_TYPE,'')=B.NEXT_PROCESS_TYPE AND A.NEXT_PROCESS_PRODUCTION_FACTORY=B.NEXT_PROCESS_PRODUCTION_FACTORY AND ";
            SQL = SQL + "        B.Corp_Output=C.KEY_NAME AND A.JOB_ORDER_NO=PO.JO_NO  ";
            SQL = SQL + "        AND A.FACTORY_CD=CASE WHEN A.PROCESS_TYPE='P' THEN po.CO_FACTORY_CD ELSE a.FACTORY_CD END "; //Added By ZouShiChang ON 2014.02.11
            SQL = SQL + "        AND SC.SC_NO=PO.SC_NO  ";
            SQL = SQL + "        and a.factory_cd=c.factory_cd ";
            SQL = SQL + "        and a.factory_cd=b.factory_cd ";
            SQL = SQL + "        AND    A.factory_cd='" + factoryCd + "' and trx_date >= '" + startDate + "' and ";
            SQL = SQL + "        trx_date <='" + endDate + "' ";
            //Modified by Zikuan MES-069 10-Dec-13
            SQL = SQL + "       AND A.WORK_DAY = 'Y' ";
            //End Modified MES-069
            if (garmentType != "")
            {
                SQL += " and PO.Garment_Type_Cd='" + garmentType + "'";
            }
            switch (outSouce)
            {
                case "OUTSOURCE":
                    SQL += " AND SC.SAM_GROUP_CD='OUTSOURCE'";
                    break;
                case "STANDARD":
                    SQL += " AND SC.SAM_GROUP_CD<>'OUTSOURCE'";
                    break;
            }
            switch (washType)
            {
                case "NW":
                    SQL += " AND po.WASH_TYPE_CD ='" + washType + "'";
                    break;
                case "WASH":
                    SQL += " AND (po.WASH_TYPE_CD NOT IN('NW') AND po.WASH_TYPE_CD IS NOT NULL)";
                    break;
            }
            if (blCheck)
            {
                SQL += " AND exists (SELECT job_order_no, MIN(ACTUAL_PRINT_DATE) from ";
                SQL += " V_ACTUAL_CUT_BUNDLE bd WHERE  factory_cd=A.factory_cd AND job_order_no=A.JOB_ORDER_NO ";
                SQL += " GROUP BY  job_order_no ";
                SQL += " HAVING  MIN(ACTUAL_PRINT_DATE)>=DBO.DATE_FORMAT('2011-03-01','yyyy-mm-dd')) ";
            }
            SQL = SQL + "        and (B.NEXT_PROCESS_CD NOT IN('EGV_PTX','SEW_OUT','OTHER_FTY') ";
            SQL = SQL + "        or (A.PROCESS_CD='SEW' AND B.NEXT_PROCESS_CD='SEW_OUT')) group ";
            SQL = SQL + "        by C.KEY_NAME) b on c.KEY_NAME=b.KEY_NAME  ";
            SQL = SQL + "        WHERE C.FACTORY_CD='" + factoryCd + "' ";
            SQL = SQL + "        order by c.key_seq ";
            return DBUtility.GetTable(SQL, "MES");
        }

    }
}