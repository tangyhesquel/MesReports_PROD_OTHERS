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
    ///PCM 的摘要说明
    /// </summary>
    public class PCM
    {
        public PCM()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        //pcm

        public static DataTable GetPcmDetail(string factoryCd, string garmentType, string washType, string year, string month)
        {
            string SQL = " EXEC PROC_GET_PCM_RPT '" + factoryCd + "','" + garmentType + "','" + washType + "','" + year + "','" + month + "' ";
            return DBUtility.GetTable(SQL, "MES");
        }


        public static DataTable GetPcmnInAndOut(string factoryCd, string garmentType)
        {
            /*
            string SQL = "         SELECT FIR.GARMENT_TYPE,FIR.PROCESS_CD,FIR.DISPLAY_SEQ SEQ,FIR.SHORT_NAME, ";
            SQL = SQL + "         (CASE WHEN ISNULL(FIR.N_OUT,0)=0 THEN 1 ELSE FIR.N_OUT END) N_OUT,(CASE WHEN ISNULL(sec.n_in,0)=0 THEN 1 ELSE sec.n_in END) N_IN ";
            SQL = SQL + "        FROM (SELECT DISTINCT A.GARMENT_TYPE,B.PROCESS_CD,A.DISPLAY_SEQ,A.SHORT_NAME,COUNT(NEXT_PROCESS_CD) N_OUT ";
            SQL = SQL + "        FROM GEN_PRC_CD_MST A,PRD_FTY_PROCESS_FLOW B ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL = SQL + "        WHERE B.PROCESS_CD=A.PRC_CD ";
            SQL = SQL + "        WHERE B.PROCESS_CD=A.PRC_CD AND B.PROCESS_GARMENT_TYPE=A.GARMENT_TYPE ";
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            SQL = SQL + "        AND A.FACTORY_CD=B.FACTORY_CD ";
            SQL = SQL + "        AND B.FACTORY_CD='" + factoryCd + "' ";
            if (garmentType != "")
            {
                SQL = SQL + "        and (A.GARMENT_TYPE='" + garmentType + "' OR A.GARMENT_TYPE_FLAG='N') ";
            }
            SQL = SQL + "        GROUP BY A.GARMENT_TYPE,B.PROCESS_CD,A.DISPLAY_SEQ,A.SHORT_NAME ";
            SQL = SQL + "	    )FIR LEFT JOIN  ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL = SQL + "	    (SELECT B.NEXT_PROCESS_CD,COUNT(B.PROCESS_CD) N_IN ";
            SQL = SQL + "	    (SELECT B.NEXT_PROCESS_CD,B.NEXT_PROCESS_GARMENT_TYPE,COUNT(B.PROCESS_CD) N_IN ";
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            SQL = SQL + "	    FROM PRD_FTY_PROCESS_FLOW B ";
            SQL = SQL + "	    WHERE B.FACTORY_CD='" + factoryCd + "' ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL = SQL + "	    GROUP BY B.NEXT_PROCESS_CD)SEC ON FIR.PROCESS_CD=SEC.NEXT_PROCESS_CD ";
            SQL = SQL + "	    GROUP BY B.NEXT_PROCESS_CD,B.NEXT_PROCESS_GARMENT_TYPE)SEC ON FIR.PROCESS_CD=SEC.NEXT_PROCESS_CD AND FIR.GARMENT_TYPE=SEC.NEXT_PROCESS_GARMENT_TYPE  ";
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            SQL = SQL + "	    ORDER BY GARMENT_TYPE,SEQ,PROCESS_CD ";
             */
            string SQL = "         SELECT FIR.GARMENT_TYPE,FIR.PROCESS_CD,FIR.DISPLAY_SEQ SEQ,FIR.SHORT_NAME, ";
            SQL = SQL + "         (CASE WHEN ISNULL(FIR.N_OUT,0)=0 THEN 1 ELSE FIR.N_OUT END) N_OUT,(CASE WHEN ISNULL(sec.n_in,0)=0 THEN 1 ELSE sec.n_in END) N_IN ";
            SQL = SQL + "          FROM (SELECT DISTINCT A.GARMENT_TYPE, ";
            SQL = SQL + "          B.PROCESS_GARMENT_TYPE+B.PROCESS_CD+B.PROCESS_TYPE+B.PROCESS_PRODUCTION_FACTORY AS PROCESS_CD,";
            SQL = SQL + "          A.DISPLAY_SEQ,A.SHORT_NAME,COUNT(NEXT_PROCESS_CD) N_OUT ";
            SQL = SQL + "        FROM GEN_PRC_CD_MST A,PRD_FTY_PROCESS_FLOW B ";


            SQL = SQL + "        WHERE B.PROCESS_CD=A.PRC_CD AND B.PROCESS_GARMENT_TYPE=A.GARMENT_TYPE ";

            SQL = SQL + "        AND A.FACTORY_CD=B.FACTORY_CD ";
            SQL = SQL + "        AND B.FACTORY_CD='" + factoryCd + "' ";
            if (garmentType != "")
            {
                SQL = SQL + "        and (A.GARMENT_TYPE='" + garmentType + "' OR A.GARMENT_TYPE_FLAG='N') ";
            }
            SQL = SQL + "        GROUP BY A.GARMENT_TYPE,B.PROCESS_CD,B.PROCESS_GARMENT_TYPE,B.PROCESS_TYPE,B.PROCESS_PRODUCTION_FACTORY,A.DISPLAY_SEQ,A.SHORT_NAME ";
            SQL = SQL + "	    )FIR LEFT JOIN  ";

            SQL = SQL + "	    (SELECT B.NEXT_PROCESS_GARMENT_TYPE+B.NEXT_PROCESS_CD+B.NEXT_PROCESS_TYPE+B.NEXT_PROCESS_PRODUCTION_FACTORY AS NEXT_PROCESS_CD, ";
            SQL = SQL + "       COUNT(B.PROCESS_CD) N_IN ";
            SQL = SQL + "	    FROM PRD_FTY_PROCESS_FLOW B ";
            SQL = SQL + "	    WHERE B.FACTORY_CD='" + factoryCd + "' ";

            SQL = SQL + "	    GROUP BY B.NEXT_PROCESS_CD,B.NEXT_PROCESS_GARMENT_TYPE,B.NEXT_PROCESS_TYPE,B.NEXT_PROCESS_PRODUCTION_FACTORY)SEC ON FIR.PROCESS_CD=SEC.NEXT_PROCESS_CD  ";

            SQL = SQL + "	    ORDER BY GARMENT_TYPE,SEQ,PROCESS_CD ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetPcmQtyOut(string factoryCd, string garmentType)
        {
            /*
            string SQL = "         SELECT A.GARMENT_TYPE,A.DISPLAY_SEQ ";
            SQL = SQL + "		SEQ,B.PROCESS_CD,B.NEXT_PROCESS_CD,B.DISPLAY_SEQ ";
            SQL = SQL + "		NEXT_SEQ,C.SHORT_NAME FROM PRD_FTY_PROCESS_FLOW B,GEN_PRC_CD_MST ";
            SQL = SQL + "		A, (SELECT PRC_CD,GARMENT_TYPE,PROCESS_TYPE,SHORT_NAME FROM GEN_PRC_CD_MST WHERE "; //增加GARMENT_TYPE列
            SQL = SQL + "		FACTORY_CD='" + factoryCd + "')C WHERE B.FACTORY_CD='" + factoryCd + "' AND ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL = SQL + "		A.FACTORY_CD=B.FACTORY_CD AND A.PRC_CD=B.PROCESS_CD AND ";
            //SQL = SQL + "		B.NEXT_PROCESS_CD=C.PRC_CD ";
            SQL = SQL + "		A.FACTORY_CD=B.FACTORY_CD AND A.PRC_CD=B.PROCESS_CD AND A.GARMENT_TYPE=B.PROCESS_GARMENT_TYPE AND A.PROCESS_TYPE=B.PROCESS_TYPE AND ";
            SQL = SQL + "		B.NEXT_PROCESS_CD=C.PRC_CD AND B.NEXT_PROCESS_GARMENT_TYPE=C.GARMENT_TYPE AND B.NEXT_PROCESS_TYPE=C.PROCESS_TYPE";
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            if (garmentType != "")
            {
                SQL = SQL + "        and (A.GARMENT_TYPE='" + garmentType + "' OR A.GARMENT_TYPE_FLAG='N') ";
            }
            SQL = SQL + "        ORDER BY A.GARMENT_TYPE,SEQ,PROCESS_CD,NEXT_SEQ,NEXT_PROCESS_CD ";
            */

            string SQL = "      SELECT A.GARMENT_TYPE,A.DISPLAY_SEQ SEQ, ";
            SQL = SQL + "       B.PROCESS_GARMENT_TYPE+B.PROCESS_CD+B.PROCESS_TYPE+B.PROCESS_PRODUCTION_FACTORY AS PROCESS_CD,";
            SQL = SQL + "       B.NEXT_PROCESS_GARMENT_TYPE+B.NEXT_PROCESS_CD+B.NEXT_PROCESS_TYPE+B.NEXT_PROCESS_PRODUCTION_FACTORY AS NEXT_PROCESS_CD,";
            SQL = SQL + "		B.DISPLAY_SEQ  NEXT_SEQ,C.SHORT_NAME FROM PRD_FTY_PROCESS_FLOW B,GEN_PRC_CD_MST ";
            SQL = SQL + "		A, (SELECT PRC_CD,GARMENT_TYPE,PROCESS_TYPE,SHORT_NAME FROM GEN_PRC_CD_MST WHERE "; //增加GARMENT_TYPE列
            SQL = SQL + "		FACTORY_CD='" + factoryCd + "')C WHERE B.FACTORY_CD='" + factoryCd + "' AND ";

            SQL = SQL + "		A.FACTORY_CD=B.FACTORY_CD AND A.PRC_CD=B.PROCESS_CD AND A.GARMENT_TYPE=B.PROCESS_GARMENT_TYPE  AND ";
            SQL = SQL + "		B.NEXT_PROCESS_CD=C.PRC_CD AND B.NEXT_PROCESS_GARMENT_TYPE=C.GARMENT_TYPE  ";

            if (garmentType != "")
            {
                SQL = SQL + "        and (A.GARMENT_TYPE='" + garmentType + "' OR A.GARMENT_TYPE_FLAG='N') ";
            }
            SQL = SQL + "        ORDER BY A.GARMENT_TYPE,SEQ,PROCESS_CD,NEXT_SEQ,NEXT_PROCESS_CD ";

            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetPcmQtyIn(string factoryCd, string garmentType)
        {
            /*
            string SQL = "       SELECT ";
            SQL = SQL + "	  A.GARMENT_TYPE,B.NEXT_PROCESS_CD,B.PROCESS_CD,C.SHORT_NAME,A.DISPLAY_SEQ,C.DISPLAY_SEQ ";
            SQL = SQL + "	  FRONT_SEQ FROM PRD_FTY_PROCESS_FLOW B,GEN_PRC_CD_MST A, (SELECT ";
            SQL = SQL + "	  PRC_CD,GARMENT_TYPE,PROCESS_TYPE,DISPLAY_SEQ,SHORT_NAME FROM GEN_PRC_CD_MST WHERE "; //增加GARMENT_TYPE,PROCESS_TYPE列
            SQL = SQL + "	  FACTORY_CD='" + factoryCd + "')C WHERE B.FACTORY_CD='" + factoryCd + "' AND ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL = SQL + "	  A.FACTORY_CD=B.FACTORY_CD AND A.PRC_CD=B.NEXT_PROCESS_CD AND ";
            //SQL = SQL + "	  B.PROCESS_CD=C.PRC_CD ";
            SQL = SQL + "	  A.FACTORY_CD=B.FACTORY_CD AND A.PRC_CD=B.NEXT_PROCESS_CD AND A.GARMENT_TYPE=B.NEXT_PROCESS_GARMENT_TYPE AND A.PROCESS_TYPE=B.NEXT_PROCESS_TYPE";
            SQL = SQL + "	  AND B.PROCESS_CD=C.PRC_CD AND B.PROCESS_GARMENT_TYPE=C.GARMENT_TYPE AND B.PROCESS_TYPE=C.PROCESS_TYPE ";
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            if (garmentType != "")
            {
                SQL = SQL + "      and (A.GARMENT_TYPE='" + garmentType + "' OR A.GARMENT_TYPE_FLAG='N') ";
            }
            SQL = SQL + "      ORDER BY ";
            SQL = SQL + "	  A.GARMENT_TYPE,A.DISPLAY_SEQ,NEXT_PROCESS_CD,FRONT_SEQ,PROCESS_CD ";
            */

            string SQL = "       SELECT A.GARMENT_TYPE,";
            SQL = SQL + "       B.NEXT_PROCESS_GARMENT_TYPE+B.NEXT_PROCESS_CD+B.NEXT_PROCESS_TYPE+B.NEXT_PROCESS_PRODUCTION_FACTORY AS NEXT_PROCESS_CD,";
            SQL = SQL + "       B.PROCESS_GARMENT_TYPE+B.PROCESS_CD+B.PROCESS_TYPE+B.PROCESS_PRODUCTION_FACTORY AS PROCESS_CD,";
            SQL=SQL+"         C.SHORT_NAME,A.DISPLAY_SEQ,C.DISPLAY_SEQ ";
            SQL = SQL + "	  FRONT_SEQ FROM PRD_FTY_PROCESS_FLOW B,GEN_PRC_CD_MST A, (SELECT ";
            SQL = SQL + "	  PRC_CD,GARMENT_TYPE,PROCESS_TYPE,DISPLAY_SEQ,SHORT_NAME FROM GEN_PRC_CD_MST WHERE "; //增加GARMENT_TYPE,PROCESS_TYPE列
            SQL = SQL + "	  FACTORY_CD='" + factoryCd + "')C WHERE B.FACTORY_CD='" + factoryCd + "' AND ";

            SQL = SQL + "	  A.FACTORY_CD=B.FACTORY_CD AND A.PRC_CD=B.NEXT_PROCESS_CD AND A.GARMENT_TYPE=B.NEXT_PROCESS_GARMENT_TYPE ";
            SQL = SQL + "	  AND B.PROCESS_CD=C.PRC_CD AND B.PROCESS_GARMENT_TYPE=C.GARMENT_TYPE  ";

            if (garmentType != "")
            {
                SQL = SQL + "      and (A.GARMENT_TYPE='" + garmentType + "' OR A.GARMENT_TYPE_FLAG='N') ";
            }
            SQL = SQL + "      ORDER BY ";
            SQL = SQL + "	  A.GARMENT_TYPE,A.DISPLAY_SEQ,NEXT_PROCESS_CD,FRONT_SEQ,PROCESS_CD ";

            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetPcmJoNoNew(string factoryCd, string garmentType, string washType, string year, string month)
        {

            string SQL = "           SELECT HD1.JOB_ORDER_NO JOB_ORDER_NO,BUYER,SC_NO, STYLE_NO,SAM, ";
            SQL = SQL + "		  WASH_TYPE,ORDER_QTY,isnull(SAH,0) ";
            SQL = SQL + "		  SAH,BUYER_PO_DEL_DATE,PROD_COMPLETION_DATE, ";
            SQL = SQL + "		  ISNULL(OST.QUANTITY,0) QUANTITY FROM (SELECT DISTINCT ";
            SQL = SQL + "		  B.JOB_ORDER_NO JOB_ORDER_NO,CU.NAME BUYER,PO.SC_NO SC_NO, ";
            SQL = SQL + "		  SC.STYLE_NO STYLE_NO,ISNULL(SA.SAM,0) AS SAM,PO.Wash_Type_Cd ";
            SQL = SQL + "		  WASH_TYPE,PA.ORDER_QTY, SAH=(case when (exists(select FTY_TYPE ";
            SQL = SQL + "		  from dbo.SC_SAM where TYPE='S' and sah is not null AND ";
            SQL = SQL + "		  SAH<>0 and sc_no=PO.sc_no)) THEN (select sum(sah) as sah from ";
            SQL = SQL + "          dbo.SC_SAM where type='S' and sc_no=PO.sc_no) else (select sum(sah) as sah  ";
            SQL = SQL + "          from dbo.SC_SAM where type='A' and sc_no=PO.sc_no ) END), ";
            SQL = SQL + "          DBO.DATE_FORMAT(PO.BUYER_PO_DEL_DATE,'yyyy-mm-dd') ";
            SQL = SQL + "          BUYER_PO_DEL_DATE, ";
            SQL = SQL + "          DBO.DATE_FORMAT(PO.PROD_COMPLETION_DATE,'yyyy-mm-dd') ";
            SQL = SQL + "          PROD_COMPLETION_DATE from PRD_JO_MONTHLY_STOCK B JOIN ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL = SQL + "          GEN_PRC_CD_MST PC ON PC.PRC_CD=B.PROCESS_CD AND ";
            SQL = SQL + "          GEN_PRC_CD_MST PC ON PC.PRC_CD=B.PROCESS_CD AND PC.GARMENT_TYPE=B.PROCESS_GARMENT_TYPE  AND";
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            SQL = SQL + "          PC.FACTORY_CD=B.FACTORY_CD AND PC.END_PROCESS_FLAG IS NULL LEFT ";
            SQL = SQL + "          JOIN JO_HD PO ON B.JOB_ORDER_NO=PO.JO_NO LEFT JOIN SC_HD SC ON ";
            SQL = SQL + "          PO.SC_NO=SC.SC_NO LEFT JOIN GEN_CUSTOMER CU ON ";
            SQL = SQL + "          PO.CUSTOMER_CD=CU.CUSTOMER_CD LEFT JOIN (SELECT ";
            SQL = SQL + "          JOB_ORDER_NO,SUM(SAM) SAM FROM PRD_JO_ROUTE_LIST WHERE ";
            SQL = SQL + "          FACTORY_CD = '" + factoryCd + "' GROUP BY JOB_ORDER_NO) SA ON ";
            SQL = SQL + "          B.JOB_ORDER_NO=SA.JOB_ORDER_NO LEFT JOIN (SELECT JO_NO,SUM(QTY) ";
            SQL = SQL + "          ORDER_QTY FROM JO_DT GROUP BY JO_NO) PA ON ";
            SQL = SQL + "          B.JOB_ORDER_NO=PA.JO_NO WHERE B.YEAR='" + year + "' AND B.MON='" + month + "' ";
            SQL = SQL + "          AND B.FACTORY_CD='" + factoryCd + "' ";
            if (garmentType != "")
            {
                SQL = SQL + "          and (PO.Garment_Type_Cd='" + garmentType + "' ) ";
            }
            switch (washType)
            {
                case "NW":
                    SQL = SQL + "          AND PO.WASH_TYPE_CD ='" + washType + "' ";
                    break;
                case "WASH":
                    SQL = SQL + "          AND (PO.WASH_TYPE_CD NOT IN('NW') AND Po.WASH_TYPE_CD IS NOT NULL) ";
                    break;
            }
            SQL = SQL + "          and ";
            SQL = SQL + "		  (abs(isnull(B.opening_qty,0))+abs(isnull(B.in_qty,0))+abs(isnull(B.out_qty,0))+abs(isnull(B.wastage_qty,0)))>0) ";
            SQL = SQL + "		  HD1 LEFT JOIN (SELECT JOX.JOB_ORDER_NO, SUM(JOX.OUTPUT_QTY) ";
            SQL = SQL + "		  QUANTITY FROM PRD_OUTSOURCE_CONTRACT_DT ";
            SQL = SQL + "		  ODT,PRD_OUTSOURCE_CONTRACT OHD, PRD_JO_OUTPUT_TRX JOX ";
            SQL = SQL + "		  ,PRD_JO_OUTPUT_HD JHD WHERE OHD.STATUS!='CAN' AND ";
            SQL = SQL + "		  OHD.CONTRACT_NO=ODT.CONTRACT_NO AND JOX.SEND_ID=ODT.SEND_ID AND ";
            SQL = SQL + "		  JOX.JOB_ORDER_NO=ODT.JOB_ORDER_NO AND ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL = SQL + "		  JOX.PROCESS_CD=OHD.RECEIVE_POINT AND JHD.DOC_NO=JOX.DOC_NO AND ";
            SQL = SQL + "		  JOX.PROCESS_CD=OHD.RECEIVE_POINT AND JOX.PROCESS_GARMENT_TYPE=OHD.GARMENT_TYPE AND JHD.DOC_NO=JOX.DOC_NO AND ";
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            SQL = SQL + "		  JOX.JOB_ORDER_NO IN(SELECT DISTINCT JOB_ORDER_NO FROM ";
            SQL = SQL + "		  PRD_JO_MONTHLY_STOCK JMS WHERE JMS.YEAR='" + year + "' AND ";
            SQL = SQL + "		  JMS.MON='" + month + "' AND JMS.FACTORY_CD='" + factoryCd + "' ) AND ";
            SQL = SQL + "		  JHD.FACTORY_CD='" + factoryCd + "' AND OHD.FACTORY_CD=JHD.FACTORY_CD AND ";
            SQL = SQL + "		  JOX.FACTORY_CD=JHD.FACTORY_CD GROUP BY JOX.JOB_ORDER_NO) OST ON ";
            SQL = SQL + "		  OST.JOB_ORDER_NO=HD1.JOB_ORDER_NO WHERE 1=1 ";
            //SQL = SQL + "       AND HD1.JOB_ORDER_NO='11F08569IT03' ";
            SQL = SQL + "		  ORDER BY HD1.JOB_ORDER_NO ";
            SQL = SQL + "  OPTION  ( RECOMPILE ) ";
            
            //string SQL = "exec SP_GetPcmJoNoNew " + year + "," + month + "," + factoryCd + "," + garmentType + "," + washType;

            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetPcmAll(string factoryCd, string garmentType, string washType, string year, string month)
        {
            /*
            string SQL = "         SELECT ";
            SQL = SQL + "		A.FACTORY_CD,A.YEAR,A.MON,A.JOB_ORDER_NO,A.PROCESS_CD,A.NEXT_PROCESS_CD, ";
            SQL = SQL + "		ISNULL(A.OPENING_QTY,0) OPENING_QTY,ISNULL(A.IN_QTY,0) ";
            SQL = SQL + "		IN_QTY,ISNULL(A.OUT_QTY,0) OUT_QTY,ISNULL(A.WASTAGE_QTY,0) ";
            SQL = SQL + "		WASTAGE_QTY FROM PRD_jo_monthly_stock a,JO_HD ";
            SQL = SQL + "		PO,GEN_PRC_CD_MST B where a.JOB_ORDER_NO=PO.JO_NO and ";
            SQL = SQL + "		a.factory_cd='" + factoryCd + "' and a.year='" + year + "' and a.mon='" + month + "' AND ";
            SQL = SQL + "		B.PRC_CD=A.PROCESS_CD AND B.FACTORY_CD=A.FACTORY_CD AND ";
            SQL = SQL + "		B.END_PROCESS_FLAG IS NULL and ";
            SQL = SQL + "		(abs(isnull(opening_qty,0))+abs(isnull(in_qty,0))+abs(isnull(out_qty,0))+abs(isnull(wastage_qty,0)))>0 ";
            if (garmentType != "")
            {
                SQL += " AND PO.Garment_type_Cd='" + garmentType + "'";
            }
            switch (washType)
            {
                case "NW":
                    SQL += " AND PO.WASH_TYPE_CD ='" + washType + "'";
                    break;
                case "WASH":
                    SQL += " AND (PO.WASH_TYPE_CD NOT IN('NW') AND P.WASH_TYPE_CD IS NOT NULL)";
                    break;
            }
            SQL = SQL + "        order by a.job_order_no ";
            */

            string SQL = "select ta.FACTORY_CD,ta.YEAR,ta.MON,ta.JOB_ORDER_NO, ";
            SQL += " ta.PROCESS_GARMENT_TYPE+ta.PROCESS_CD+TA.PROCESS_TYPE+TA.PROCESS_PRODUCTION_FACTORY AS PROCESS_CD, ";
            SQL += " TB.NEXT_PROCESS_GARMENT_TYPE+TB.NEXT_PROCESS_CD+TB.NEXT_PROCESS_TYPE+TB.NEXT_PROCESS_PRODUCTION_FACTORY AS NEXT_PROCESS_CD, ";
            SQL += " ta.OPENING_QTY,ta.IN_QTY,ta.WASTAGE_QTY,tb.OUT_QTY from";
            SQL += " (SELECT A.FACTORY_CD,A.YEAR,A.MON,A.JOB_ORDER_NO,A.PROCESS_CD,A.PROCESS_GARMENT_TYPE,A.PROCESS_TYPE,A.PROCESS_PRODUCTION_FACTORY, ";
            SQL += " sum(ISNULL(A.OPENING_QTY,0)) OPENING_QTY,";
            SQL += " sum(ISNULL(A.IN_QTY,0)) 		IN_QTY,";
            SQL += " sum(ISNULL(A.WASTAGE_QTY,0)) 		WASTAGE_QTY ";
            SQL += " from PRD_jo_monthly_stock A with(nolock),JO_HD 		PO with(nolock),GEN_PRC_CD_MST B with(nolock) ";
            SQL += " where a.JOB_ORDER_NO=PO.JO_NO ";
            SQL += " and 		a.factory_cd='" + factoryCd + "' ";
            SQL += " and     a.year='" + year + "' ";
            SQL += " and     a.mon='" + month + "'";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL += " and 		B.PRC_CD=A.PROCESS_CD ";
            SQL += " and 		B.PRC_CD=A.PROCESS_CD AND B.GARMENT_TYPE=A.PROCESS_GARMENT_TYPE  ";
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            SQL += " and     B.FACTORY_CD=A.FACTORY_CD ";
            SQL += " and 		B.END_PROCESS_FLAG IS NULL ";
            if (garmentType != "")
            {
                SQL += " and PO.Garment_type_Cd='" + garmentType + "'        ";
            }
            switch (washType)
            {
                case "NW":
                    SQL += " AND Po.WASH_TYPE_CD ='" + washType + "'";
                    break;
                case "WASH":
                    SQL += " AND (Po.WASH_TYPE_CD NOT IN('NW') AND Po.WASH_TYPE_CD IS NOT NULL)";
                    break;
            }
            SQL += " group by A.FACTORY_CD,A.YEAR,A.MON,A.JOB_ORDER_NO,A.PROCESS_CD,A.PROCESS_GARMENT_TYPE,A.PROCESS_TYPE,A.PROCESS_PRODUCTION_FACTORY) as ta";
            SQL += " inner join";
            SQL += " (SELECT 		A.FACTORY_CD,A.YEAR,A.MON,A.JOB_ORDER_NO,A.PROCESS_CD,A.PROCESS_GARMENT_TYPE,A.PROCESS_TYPE,A.PROCESS_PRODUCTION_FACTORY,A.NEXT_PROCESS_CD,A.NEXT_PROCESS_GARMENT_TYPE,A.NEXT_PROCESS_TYPE,A.NEXT_PROCESS_PRODUCTION_FACTORY, 		";
            SQL += " sum(ISNULL(A.OUT_QTY,0)) OUT_QTY";
            SQL += " from PRD_jo_monthly_stock a with(nolock),JO_HD 		PO with(nolock),GEN_PRC_CD_MST B with(nolock) ";
            SQL += " where a.JOB_ORDER_NO=PO.JO_NO ";
            SQL += " and 		a.factory_cd='" + factoryCd + "' ";
            SQL += " and a.year='" + year + "' ";
            SQL += " and a.mon='" + month + "'";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL += " and 		B.PRC_CD=A.PROCESS_CD ";
            SQL += " and 		B.PRC_CD=A.PROCESS_CD and B.GARMENT_TYPE=A.PROCESS_GARMENT_TYPE ";
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            SQL += " and B.FACTORY_CD=A.FACTORY_CD ";
            SQL += " and 		B.END_PROCESS_FLAG IS NULL ";
            if (garmentType != "")
            {
                SQL += " and PO.Garment_type_Cd='" + garmentType + "'";
            }
            switch (washType)
            {
                case "NW":
                    SQL += " AND Po.WASH_TYPE_CD ='" + washType + "'";
                    break;
                case "WASH":
                    SQL += " AND (Po.WASH_TYPE_CD NOT IN('NW') AND Po.WASH_TYPE_CD IS NOT NULL)";
                    break;
            }
            SQL += " and isnull(a.out_qty,0)<>0";
            SQL += " group by A.FACTORY_CD,A.YEAR,A.MON,A.JOB_ORDER_NO,A.PROCESS_CD,A.PROCESS_GARMENT_TYPE,A.PROCESS_TYPE,A.PROCESS_PRODUCTION_FACTORY,A.NEXT_PROCESS_CD,A.NEXT_PROCESS_GARMENT_TYPE,A.NEXT_PROCESS_TYPE,A.NEXT_PROCESS_PRODUCTION_FACTORY) as tb";
            SQL += " on ta.FACTORY_CD=tb.FACTORY_CD and  ta.YEAR=tb.YEAR and ta.MON=tb.MON and ta.JOB_ORDER_NO=tb.JOB_ORDER_NO and ";
            SQL += " ta.PROCESS_CD=tb.PROCESS_CD AND ta.PROCESS_GARMENT_TYPE=tb.PROCESS_GARMENT_TYPE and ta.process_type=tb.process_Type";   //MES 024         
            SQL += " and  ta.OPENING_QTY+ta.IN_QTY-ta.WASTAGE_QTY-tb.OUT_QTY<>0";
            //SQL = SQL + "       AND TA.JOB_ORDER_NO='11F08569IT03' ";
            SQL += " order by ta.JOB_ORDER_NO,ta.PROCESS_CD";
            SQL = SQL + "  OPTION  ( RECOMPILE ) ";
            return DBUtility.GetTable(SQL, "MES");
        }
    }
}