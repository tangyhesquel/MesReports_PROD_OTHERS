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
    /// pcmDailyBundlingQty 的摘要说明

    /// </summary>
    public class pcmDailyBundlingQty
    {
        public static DataTable GetDailyBundlingQty(string factoryCd, string JoNo, string cutLine, string startDate, string endDate)
        {

            string SQL = "      SELECT A.TRX_DATE,A.JOB_ORDER_NO,A.CUT_LINE, A.CUT_TABLE_NO ";
            SQL = SQL + "		CUTLOT_NO, TODAY_BUNDLING_QTY,O.ORDER_QTY,B.TOTAL_BUNDLING_QTY ";
            SQL = SQL + "		FROM ( ";
            SQL += @" SELECT    CONVERT(VARCHAR(10), actual_print_date, 120) TRX_DATE ,
                            A.job_order_no ,
                            A.cut_line ,
                            CUT_TABLE_NO ,
                            SUM(qty) TODAY_BUNDLING_QTY
                  FROM      CUT_BUNDLE_HD AS A
                            INNER JOIN cut_lay AS B ON a.JOB_ORDER_NO = B.JOB_ORDER_NO
                                                       AND A.LAY_NO = B.lay_no
                  where ";
            SQL += "  a.factory_cd='" + factoryCd + "'";
            if (JoNo != "")
            {
                SQL += " and a.job_order_no LIKE '%" + JoNo + "%'";
            }
            if (cutLine != "")
            {
                SQL += " and A.cut_line = '" + cutLine + "'";
            }
            if (JoNo != "")
            {
                SQL += " and a.job_order_no LIKE '%" + JoNo + "%'";
            }
            if (startDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)>=convert(varchar(10),'" + startDate + "',120)";
            }
            if (endDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)<=convert(varchar(10),'" + endDate + "',120)";
            }
            SQL = SQL + "        group by ";
            SQL = SQL + "		convert(varchar(10),actual_print_date,120),A.job_order_no,A.cut_line ";
            SQL = SQL + "		,CUT_TABLE_NO ) A, (select A.job_order_no ,sum(qty) ";
            SQL = SQL + "		TOTAL_BUNDLING_QTY from CUT_BUNDLE_HD a ";

            SQL = SQL + "		 where  a.factory_cd='" + factoryCd + "'";
            if (JoNo != "")
            {
                SQL += " and a.job_order_no LIKE '%" + JoNo + "%'";
            }
            if (cutLine != "")
            {
                SQL += " and cut_line = '" + cutLine + "'";
            }
            if (startDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)>=convert(varchar(10),'" + startDate + "',120)";
            }
            if (endDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)<=convert(varchar(10),'" + endDate + "',120)";
            }
            SQL = SQL + "        group by A.job_order_no ) B, (SELECT PO_NO JOB_ORDER_NO,SUM(QTY) ";
            SQL = SQL + "		ORDER_QTY FROM EX_PO_DT PD WHERE 1=1 ";
            if (JoNo != "")
            {
                SQL += " and PD.PO_NO LIKE '%" + JoNo + "%'";
            }
            SQL = SQL + "        AND PD.PO_NO IN( SELECT DISTINCT JOB_ORDER_NO FROM CUT_BUNDLE_HD ";
            SQL = SQL + "		WHERE 1=1 ";
            if (factoryCd != "")
            {
                SQL += " and factory_cd='" + factoryCd + "'";
            }
            if (JoNo != "")
            {
                SQL += " and job_order_no LIKE '%" + JoNo + "%'";
            }
            if (startDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)>=convert(varchar(10),'" + startDate + "',120)";
            }
            if (endDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)<=convert(varchar(10),'" + endDate + "',120)";
            }
            SQL = SQL + "        ) GROUP BY PO_NO) O WHERE A.JOB_ORDER_NO=O.JOB_ORDER_NO ORDER BY ";
            SQL = SQL + "		TRX_DATE,JOB_ORDER_NO,A.cut_line,A.CUT_TABLE_NO ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetDailyBundlingQtyLineSummary(string factoryCd, string JoNo, string cutLine, string startDate, string endDate)
        {

            string SQL = "      SELECT Bb.CUT_LINE,SUM(O.ORDER_QTY) AS ";
            SQL = SQL + "		ORDER_QTY,sum(aa.TODAY_BUNDLING_QTY) as TODAY_BUNDLING_QTY, ";
            SQL = SQL + "		SUM(Bb.TOTAL_BUNDLING_QTY) AS TOTAL_BUNDLING_QTY FROM (select ";
            SQL = SQL + "		A.job_order_no , A.cut_line , sum(qty) TODAY_BUNDLING_QTY from ";
            SQL = SQL + "		CUT_BUNDLE_HD a ";
            SQL += " Where a.factory_cd='" + factoryCd + "'";
            if (JoNo != "")
            {
                SQL += " and a.job_order_no LIKE '%" + JoNo + "%'";
            }
            if (startDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)>=convert(varchar(10),'" + startDate + "',120)";
            }
            if (endDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)<=convert(varchar(10),'" + endDate + "',120)";
            }
            SQL = SQL + "        group by A.job_order_no,A.cut_line ) Aa, (select ";
            SQL = SQL + "		A.job_order_no,A.cut_line ,sum(qty) TOTAL_BUNDLING_QTY from ";
            SQL = SQL + "		CUT_BUNDLE_HD a ";
            SQL += " Where a.factory_cd='" + factoryCd + "'";
            if (JoNo != "")
            {
                SQL += " and a.job_order_no LIKE '%" + JoNo + "%'";
            }
            if (startDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)>=convert(varchar(10),'" + startDate + "',120)";
            }
            if (endDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)<=convert(varchar(10),'" + endDate + "',120)";
            }
            SQL = SQL + "        group by A.CUT_LINE,A.job_order_no) Bb, (SELECT PO_NO ";
            SQL = SQL + "		JOB_ORDER_NO,SUM(QTY) ORDER_QTY FROM EX_PO_DT PD WHERE 1=1 ";
            if (JoNo != "")
            {
                SQL += " and PD.PO_NO LIKE '%" + JoNo + "%'";
            }
            SQL = SQL + "        AND PD.PO_NO IN( SELECT DISTINCT JOB_ORDER_NO FROM CUT_BUNDLE_HD ";
            SQL = SQL + "		WHERE 1=1 ";
            if (factoryCd != "")
            {
                SQL += " and factory_cd='" + factoryCd + "'";
            }
            if (JoNo != "")
            {
                SQL += " and job_order_no LIKE '%" + JoNo + "%'";
            }
            if (startDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)>=convert(varchar(10),'" + startDate + "',120)";
            }
            if (endDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)<=convert(varchar(10),'" + endDate + "',120)";
            }
            SQL = SQL + "        ) GROUP BY PO_NO) O WHERE Bb.JOB_ORDER_NO=O.JOB_ORDER_NO and ";
            SQL = SQL + "		aa.cut_line=bb.cut_line and aa.JOB_ORDER_NO=bb.JOB_ORDER_NO ";
            SQL = SQL + "		GROUP BY Bb.cut_line ORDER BY Bb.cut_line ";

            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetDailyBundlingQtySummary(string factoryCd, string JoNo, string cutLine, string startDate, string endDate)
        {

            string SQL = "         SELECT SUM(O.ORDER_QTY) AS ORDER_QTY,SUM(TODAY_BUNDLING_QTY) AS ";
            SQL = SQL + "		TODAY_BUNDLING_QTY, SUM(B.TOTAL_BUNDLING_QTY) AS ";
            SQL = SQL + "		TOTAL_BUNDLING_QTY FROM (select A.job_order_no, sum(qty) ";
            SQL = SQL + "		TODAY_BUNDLING_QTY from CUT_BUNDLE_HD a ";
            SQL += " WHERE a.factory_cd='" + factoryCd + "'";
            if (JoNo != "")
            {
                SQL += " and a.job_order_no LIKE '%" + JoNo + "%'";
            }
            if (startDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)>=convert(varchar(10),'" + startDate + "',120)";
            }
            if (endDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)<=convert(varchar(10),'" + endDate + "',120)";
            }
            SQL = SQL + "        group by A.job_order_no ) A,(select A.job_order_no,sum(qty) ";
            SQL = SQL + "		TOTAL_BUNDLING_QTY from CUT_BUNDLE_HD a ";
            SQL += " WHERE a.factory_cd='" + factoryCd + "'";
            if (JoNo != "")
            {
                SQL += " and a.job_order_no LIKE '%" + JoNo + "%'";
            }
            if (startDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)>=convert(varchar(10),'" + startDate + "',120)";
            }
            if (endDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)<=convert(varchar(10),'" + endDate + "',120)";
            }
            SQL = SQL + "        group by A.job_order_no ) B, (SELECT PO_NO JOB_ORDER_NO,SUM(QTY) ";
            SQL = SQL + "		ORDER_QTY FROM EX_PO_DT PD WHERE 1=1 ";
            if (JoNo != "")
            {
                SQL += " and PD.PO_NO LIKE '%" + JoNo + "%'";
            }
            SQL = SQL + "        AND PD.PO_NO IN( SELECT DISTINCT JOB_ORDER_NO FROM CUT_BUNDLE_HD ";
            SQL = SQL + "		WHERE 1=1 ";
            if (factoryCd != "")
            {
                SQL += " and factory_cd='" + factoryCd + "'";
            }
            if (JoNo != "")
            {
                SQL += " and job_order_no LIKE '%" + JoNo + "%'";
            }
            if (startDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)>=convert(varchar(10),'" + startDate + "',120)";
            }
            if (endDate != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)<=convert(varchar(10),'" + endDate + "',120)";
            }
            SQL = SQL + "        ) GROUP BY PO_NO) O WHERE B.JOB_ORDER_NO=O.JOB_ORDER_NO and ";
            SQL = SQL + "		a.JOB_ORDER_NO=b.JOB_ORDER_NO ";
            return DBUtility.GetTable(SQL, "MES");

        }

    }
}