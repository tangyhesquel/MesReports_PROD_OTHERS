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
    ///wipMonthlySql 的摘要说明

    /// </summary>
    public class wipMonthlySql
    {
        public static DataTable GetMonthlyTitle(string month, string year, string factoryCd)
        {
            string SQL = " SELECT DISTINCT MST.display_seq,PROCESS_CD ";
            SQL = SQL + "    from PRD_MONTHLY_BALANCE B ,GEN_PRC_CD_MST MST ";
            SQL = SQL + "    WHERE B.MON='" + month + "' ";
            SQL = SQL + "    AND B.YEAR='" + year + "' ";
            SQL = SQL + "    and B.FACTORY_CD='" + factoryCd + "' ";
            SQL = SQL + "    AND B.PROCESS_CD=MST.PRC_CD AND B.GARMENT_TYPE=MST.GARMENT_TYPE  ";
            SQL = SQL + "    AND MST.FACTORY_CD='" + factoryCd + "' ";
            SQL = SQL + "    ORDER BY MST.display_seq ";

            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetMonthly(string factoryCd, string month, string year, string garmentType, string orderType, string washType)
        {
            string SQL = "         SELECT CU.NAME BUYER,SC.SC_NO SC_NO,PO.JO_NO JO_NO,PO.Garment_type_Cd Garment_Type,PO.Wash_Type_CD Wash_Type, PO.PROD_COMPLETION_DATE prd_cmp_dt, ";
            SQL = SQL + "  SC.STYLE_NO STYLE_NO,DBO.DATE_FORMAT(BUYER_PO_DEL_DATE,'YYYY-MM-DD') GMT_DEL_DATE, ";
            SQL=SQL+ " B.PROCESS_CD AS PROCESS_CD , ";
            SQL=SQL+" isnull(SUM(OUTPUT_QTY),0) OUTPUT_QTY ";
            SQL = SQL + " ,SAH = DBO.FN_GET_SAH(SC.SC_NO) ";//2012-07-23 EAV需求添加SAH；

            SQL = SQL + " ,ORDER_QTY = (SELECT SUM(QTY) FROM JO_HD C,JO_DT D WHERE C.JO_NO =D.JO_NO AND  C.FACTORY_CD =B.FACTORY_CD AND";
            SQL = SQL + " C.JO_NO = PO.JO_NO GROUP BY C.JO_NO,C.FACTORY_CD)";
            SQL = SQL + "        from  ";
            SQL = SQL + "        PRD_MONTHLY_BALANCE B ";
            SQL = SQL + "        LEFT JOIN JO_HD PO ON PO.JO_NO=B.JOB_ORDER_NO ";
            SQL = SQL + "        LEFT JOIN SC_HD SC ON SC.SC_NO=PO.SC_NO ";
            SQL = SQL + "        INNER JOIN GEN_CUSTOMER CU ON CU.CUSTOMER_CD=PO.CUSTOMER_CD ";
            SQL = SQL + "        WHERE 1=1 AND  B.MON='" + month + "' ";
            SQL = SQL + "        AND B.YEAR='" + year + "' ";
            SQL = SQL + "        and B.FACTORY_CD='" + factoryCd + "' ";
            SQL = SQL + " AND B.FACTORY_CD=CASE WHEN B.PROCESS_TYPE='P' THEN po.CO_FACTORY_CD ELSE B.FACTORY_CD END  ";
            if (garmentType != "")
            {
                SQL += " AND PO.Garment_type_Cd='" + garmentType + "'";
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
            switch (orderType)
            {
                case "Y":
                    SQL += " AND NOT PO.OPA_factory_Cd IS NULL";
                    break;
                case "N":
                    SQL += " AND (PO.OPA_factory_Cd IS NULL )";
                    break;
            }
            SQL = SQL + "        group by CU.NAME, SC.SC_NO,PO.JO_NO,SC.STYLE_NO, ";
            SQL = SQL + "  BUYER_PO_DEL_DATE,B.GARMENT_TYPE,B.PROCESS_CD,B.PROCESS_TYPE,B.PRODUCTION_FACTORY,PO.PROD_COMPLETION_DATE, PO.Garment_type_Cd ,PO.Wash_Type_CD  ";
            SQL = SQL + " ,B.FACTORY_CD";
            SQL = SQL + "  ORDER BY  CU.NAME, SC.SC_NO,PO.JO_NO,SC.STYLE_NO, ";
            SQL = SQL + "  BUYER_PO_DEL_DATE ";

            return DBUtility.GetTable(SQL, "MES");
        }

    }
}