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
    /// JobOrderRouteList 的摘要说明


    /// </summary>
    public class JobOrderRouteList
    {
        public static DataTable GetJobOrderRouteHead(string factoryCd, string JoNo)
        {
            string SQL = " SELECT A.create_user_id AS USERID,A.JOB_ORDER_NO,B.SC_NO AS GO_NO, ";
            SQL += " a.Job_Order_Date,B.BUYER_PO_DEL_DATE,S.STYLE_NO,B.CUSTOMER_CD AS BUYER_CODE,G.SHORT_NAME AS BUYER_NAME, ";
            SQL += " Style_Desc=(case when S.factory_cd in('TIL','PTX','EGM','EGV','EAV') THEN S.style_desc else S.style_chn_desc end), ";
            SQL += " L.TOTAL_QTY AS ORDER_QTY_P,cast(L.TOTAL_QTY/12.00   as   decimal(18,2)) AS ORDER_QTY_D,B.WASH_TYPE_CD AS WASHTYPE,S.SAMPLE_ORDER AS FDS_No, ";
            SQL += " GETDATE() AS DatePrint,A.REMARK ";
            SQL += " FROM  ";
            SQL += " (select top 1 a.JOB_ORDER_NO,CREATE_BY AS create_user_id,CREATE_DATE as Job_Order_Date,REMARK ";
            SQL += "from PRD_JO_ROUTE_LIST_DEF a with(nolock)  ";
            SQL += "inner join dbo.PRD_GO_ROUTE_LIST_HD b with(nolock)  ";
            SQL += "on a.ROUTE_HD_ID=b.ROUTE_HD_ID ";
            SQL += "where A.FACTORY_CD='" + factoryCd + "' ";
            SQL += "AND A.JOB_ORDER_NO='" + JoNo + "' ";
            SQL += "AND APPROVE_STATUS='Y')  A ";
            SQL += " LEFT JOIN jo_hd B WITH(NOLOCK) ";
            SQL += " ON B.JO_NO=A.job_order_no ";
            SQL += "AND B.jo_no='" + JoNo + "' ";
            SQL += " LEFT JOIN SC_HD S WITH(NOLOCK) ";
            SQL += " ON S.sc_no=B.SC_NO ";
            SQL += " LEFT JOIN SC_LOT L WITH(NOLOCK) ";
            SQL += " ON L.SC_NO=B.SC_NO ";
            SQL += " AND L.LOT_NO=B.LOT_NO ";
            SQL += " left join GEN_CUSTOMER G ";
            SQL += " ON G.CUSTOMER_CD=B.CUSTOMER_CD ";
            SQL += " where 1=1 ";

            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetJobOrderRouteList(string factoryCd, string Date, string JoNo, string Orderby, string PartCD, string Cstr, string Process_cd, string Prod_Line_Cd, bool ShowBlankLine)
        {
            string SQL = "  SELECT  " + Cstr;
            SQL = SQL + "   FROM  PRD_JO_ROUTE_LIST A WITH (NOLOCK) INNER JOIN GEN_OPERATION_CODE_MASTER B WITH (NOLOCK) ON A.OPER_CODE =B.OPERATION_CD AND  A.FACTORY_CD = B.FACTORY_CD ";
            SQL = SQL + "  LEFT JOIN  (SELECT S.ROUTE_DT_ID,S.LINE_CD,MAX(S.PIECE_RATE) AS PIECE_RATE FROM ";
            SQL = SQL + "        (SELECT E.ROUTE_DT_ID,F.LINE_CD,dbo.FN_GET_ACTUAL_PIECE_RATE(E.FACTORY_CD,E.JOB_ORDER_NO,E.JOB_CD,F.LINE_CD) AS PIECE_RATE FROM PRD_JO_ROUTE_LIST E WITH (NOLOCK) ";
            SQL = SQL + "    INNER JOIN PRD_GO_ROUTE_LIST_MULTI_PR F WITH (NOLOCK) ON E.ROUTE_DT_ID=F.ROUTE_DT_ID) S GROUP BY S.ROUTE_DT_ID,S.LINE_CD) C ON A.ROUTE_DT_ID=C.ROUTE_DT_ID ";
            SQL = SQL + "  LEFT JOIN GEN_PRODUCTION_LINE D WITH (NOLOCK) ON C.LINE_CD=D.PRODUCTION_LINE_CD ";
            SQL = SQL + " LEFT JOIN PRD_JO_PROCESS_ROUTE_HD F on A.Job_Order_No=F.Job_Order_No AND PROCESS_TYPE='I' and ((A.PROCESS_CD='IRON' and F.PROCESS_CD='FIN') or (A.PROCESS_CD=F.PROCESS_CD)) ";
            SQL = SQL + "    WHERE A.FACTORY_CD='" + factoryCd + "' ";
            if (Date != "")
            {
                SQL += "  AND A.CREATE_DATE >= '" + Date + "' AND  A.CREATE_DATE <= DATEADD(d, 1, '" + Date + "')";
            }
            if (JoNo != "")
            {
                SQL += " AND A.Job_Order_No = '" + JoNo + "' ";
            }
            if (PartCD != "")
            {
                SQL += " AND B.PART_CD = '" + PartCD + "' ";
            }
            if (!Process_cd.Equals("ALL"))
            {
                SQL += " AND A.PROCESS_CD = '" + Process_cd + "'";
            }
            if (Prod_Line_Cd != "")
            {
                if (ShowBlankLine.Equals(true))
                {
                    SQL += " AND (C.LINE_CD='" + Prod_Line_Cd + "' OR C.LINE_CD IS NULL)";
                }
                else
                {
                    SQL += " AND C.LINE_CD='" + Prod_Line_Cd + "'";
                }
            }

            SQL += " ORDER BY A.Job_Order_No";
            switch (Orderby)
            {
                case "DISPLAY_SEQ":
                    SQL += " ,CAST(A.DISPLAY_SEQ AS INT)";
                    break;
                case "JOB_SEQUENCE_NO":
                    SQL += "  ,CAST(A.JOB_SEQUENCE_NO AS INT)";
                    break;
                case "JOB_CD":
                    SQL += "  ,CAST(A.JOB_CD AS INT)";
                    break;
                case "OPER_CODE":
                    SQL += "  ,A.OPER_CODE";
                    break;
                case "PROCESS_CD":
                    SQL += "  ,F.SEQ";
                    break;
                case "PART_CD":
                    SQL += "  ,A.PART_CD,CAST(A.DISPLAY_SEQ AS INT)";
                    break;
            }
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetJobOrderRouteListPieceRateByProcess_EX(string factoryCd, string Date, string JoNo, string PartCD, string Process_cd, string Prod_Line_Cd, bool ShowBlankLine)
        {
            string SQL = "  SELECT A.PROCESS_CD,C.LINE_CD,D.PRODUCTION_LINE_NAME,SUM(CASE WHEN C.LINE_CD IS NULL THEN A.PIECE_RATE ELSE C.PIECE_RATE END) AS PIECE_RATE,";
            SQL = SQL + "10 * SUM(CASE WHEN C.LINE_CD IS NULL THEN A.PIECE_RATE ELSE C.PIECE_RATE END) AS PieceRate_Total,Sum(A.SAM) as SAM ";
            SQL = SQL + "   FROM  PRD_JO_ROUTE_LIST A WITH (NOLOCK) INNER JOIN GEN_OPERATION_CODE_MASTER B WITH (NOLOCK) ON A.OPER_CODE =B.OPERATION_CD AND  A.FACTORY_CD = B.FACTORY_CD ";
            SQL = SQL + "  LEFT JOIN  (SELECT S.ROUTE_DT_ID,S.LINE_CD,MAX(S.PIECE_RATE) AS PIECE_RATE FROM ";
            SQL = SQL + "        (SELECT E.ROUTE_DT_ID,F.LINE_CD,dbo.FN_GET_ACTUAL_PIECE_RATE(E.FACTORY_CD,E.JOB_ORDER_NO,E.JOB_CD,F.LINE_CD) AS PIECE_RATE FROM PRD_JO_ROUTE_LIST E WITH (NOLOCK) ";
            SQL = SQL + "    INNER JOIN PRD_GO_ROUTE_LIST_MULTI_PR F WITH (NOLOCK) ON E.ROUTE_DT_ID=F.ROUTE_DT_ID) S GROUP BY S.ROUTE_DT_ID,S.LINE_CD) C ON A.ROUTE_DT_ID=C.ROUTE_DT_ID ";
            SQL = SQL + "    LEFT JOIN GEN_PRODUCTION_LINE D WITH (NOLOCK) ON C.LINE_CD=D.PRODUCTION_LINE_CD ";
            SQL = SQL + "    WHERE A.FACTORY_CD='" + factoryCd + "' ";

            if (JoNo != "")
            {
                SQL += " AND A.Job_Order_No = '" + JoNo + "'";
            }
            if (Date != "")
            {
                SQL += " AND A.CREATE_DATE >= '" + Date + "' AND  A.CREATE_DATE <= DATEADD(d, 1, '" + Date + "')";
            }
            if (PartCD != "")
            {
                SQL += " AND B.PART_CD = '" + PartCD + "' ";
            }
            if (!Process_cd.Equals("ALL"))
            {
                SQL += " AND A.PROCESS_CD = '" + Process_cd + "' ";
            }
            if (Prod_Line_Cd != "")
            {
                if (ShowBlankLine.Equals(true))
                {
                    SQL += " AND (C.LINE_CD='" + Prod_Line_Cd + "' OR C.LINE_CD IS NULL)";
                }
                else
                {
                    SQL += " AND C.LINE_CD='" + Prod_Line_Cd + "'";
                }
            }
            SQL += " Group by A.PROCESS_CD,C.LINE_CD,D.PRODUCTION_LINE_NAME";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetJobOrderRouteListPieceRateByProcess(string factoryCd, string Date, string JoNo, string PartCD, string Process_cd)
        {
            string SQL = "         SELECT  ";
            SQL = SQL + "		PRD_JO_ROUTE_LIST.PROCESS_CD,  ";
            SQL = SQL + "		SUM(PRD_JO_ROUTE_LIST.PIECE_RATE) AS PIECE_RATE, ";
            SQL = SQL + "		10 * SUM(PRD_JO_ROUTE_LIST.PIECE_RATE) AS PieceRate_Total ";
            SQL = SQL + "		FROM ";
            SQL = SQL + "		PRD_JO_ROUTE_LIST ,GEN_OPERATION_CODE_MASTER WHERE ";
            SQL = SQL + "		PRD_JO_ROUTE_LIST.FACTORY_CD='" + factoryCd + "' AND ";
            SQL = SQL + "		PRD_JO_ROUTE_LIST.Oper_Code = ";
            SQL = SQL + "		GEN_OPERATION_CODE_MASTER.OPERATION_CD and ";
            SQL = SQL + "		PRD_JO_ROUTE_LIST.FACTORY_CD = ";
            SQL = SQL + "		GEN_OPERATION_CODE_MASTER.FACTORY_CD ";
            if (JoNo != "")
            {
                SQL += " AND PRD_JO_ROUTE_LIST.Job_Order_No = '" + JoNo + "'";
            }
            if (Date != "")
            {
                SQL += " AND PRD_JO_ROUTE_LIST.CREATE_DATE >= '" + Date + "' AND  PRD_JO_ROUTE_LIST.CREATE_DATE <= dateadd(d, 1, '" + Date + "')";
            }
            if (PartCD != "")
            {
                SQL += " AND GEN_OPERATION_CODE_MASTER.PART_CD = '" + PartCD + "' ";
            }
            if (!Process_cd.Equals(""))
            {
                SQL += " AND PRD_JO_ROUTE_LIST.PROCESS_CD = '" + Process_cd + "' ";
            }
            SQL += " Group by PROCESS_CD";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetJoRouteListProcessCd(string factoryCd, string GarmnetTypeCd)
        {
            string SQL = @" SELECT  'ALL' PRC_CD ,
        
                    1 AS DISPLAY_SEQ
            UNION ALL
            SELECT  DISTINCT PRC_CD ,
        
                    DISPLAY_SEQ
            FROM    gen_prc_cd_mst
            WHERE   1 = 1  AND ACTIVE='Y' AND factory_cd = '" + factoryCd + "' ";

            if (GarmnetTypeCd != "ALL")
            {
                SQL = SQL + " AND garment_type='" + GarmnetTypeCd + "'";
            }
            SQL = SQL + " ORDER BY display_seq ";

            return DBUtility.GetTable(SQL, "MES");

        }

        public static DataTable GetProdLineCd(string factoryCd, string JobOrderNo)
        {
            string SQL = " SELECT Prod_Line_Cd='',Prod_Line_Name='ALL' UNION ALL ";
            SQL = SQL + " SELECT DISTINCT B.LINE_CD AS Prod_Line_Cd,C.PRODUCTION_LINE_NAME AS Prod_Line_Name FROM PRD_JO_ROUTE_LIST A ";
            SQL = SQL + " INNER JOIN PRD_GO_ROUTE_LIST_MULTI_PR B ON A.ROUTE_DT_ID=B.ROUTE_DT_ID ";
            SQL = SQL + " INNER JOIN GEN_PRODUCTION_LINE C ON B.LINE_CD=C.PRODUCTION_LINE_CD WHERE 1=1 ";
            if (factoryCd != "")
            {
                SQL += " AND A.FACTORY_CD='" + factoryCd + "'";
            }
            if (JobOrderNo != "")
            {
                SQL += " And A.JOB_ORDER_NO='" + JobOrderNo + "'";
            }
            SQL += " ORDER BY Prod_Line_Cd";

            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetPARTList(string factoryCd)
        {
            string SQL = "SELECT '' AS PART_CD,'ALL' AS PART_DESC";
            SQL += "    UNION ALL";
            SQL += "    select DISTINCT PART_CD,PART_DESC";
            SQL += "    from dbo.GEN_PART_MASTER WITH(NOLOCK)";
            SQL += "    where factory_CD='" + factoryCd + "'";
            SQL += "    ORDER BY PART_CD";
            return DBUtility.GetTable(SQL, "MES");
        }


    }
}