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
    ///PreCutSummarySql 的摘要说明
    /// </summary>
    public class PreCutSummarySql
    {
        public static DataTable GetCutAndShipmentReportFactoryName(string FactoryCd)
        {
            string SQL = "";

            SQL += "select ENG_NAME From gen_factory where factory_id='" + FactoryCd + "'";

            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetCutAndShipmentReportHeader(string PONO)
        {
            string SQL = "";
            SQL += @" SELECT DISTINCT PO_HD.JO_NO JOBNO, GEN_CUSTOMER.SHORT_NAME BUYER, BPO.BUYER_JO_NO PONO,
                            OS.BUYER_JO_NO || '(' || OS.SHIP_MODE_CD || ')' PONODESC,
                            OS.BPO_ID, STYLE_HD.STYLE_DESC STYLENO, BPO.QTY ORDERQTY, '' CUTDATE,
                            OS.PERCENTOVER OVERPERCENT, OS.PERCENTSHORT SHORTPERCENT
                            FROM SC_LOT,
                            PO_HD,
                            STYLE_HD,
                            SC_HD,
                            GEN_CUSTOMER,
                            SCX_LOT_BPO_HD BPO,
                            (SELECT A.BUYER_JO_NO, A.SHIP_MODE_CD, BPO_ID,
                            DECODE (A.PERCENT_OVER_ALLOWED,
                            NULL, B.POA,
                            0, POA,
                            A.PERCENT_OVER_ALLOWED
                            ) PERCENTOVER,
                            DECODE (A.PERCENT_SHORT_ALLOWED,
                            NULL, B.PSA,
                            0, PSA,
                            A.PERCENT_SHORT_ALLOWED
                            ) PERCENTSHORT
                            FROM SCX_LOT_BPO_HD A,
                            (SELECT PO_HD.SC_NO, PO_HD.LOT_NO, PO_HD.JO_NO,
                            SC_LOT.PERCENT_OVER_ALLOWED POA,
                            SC_LOT.PERCENT_SHORT_ALLOWED PSA
                            FROM SC_LOT, PO_HD
                            WHERE SC_LOT.SC_NO = PO_HD.SC_NO
                            AND SC_LOT.LOT_NO = PO_HD.LOT_NO
                            AND PO_HD.JO_NO = '" + PONO + "'";
            SQL += @" AND NOT SC_LOT.PERCENT_OVER_ALLOWED IS NULL) B
                            WHERE A.LOT_NO = B.LOT_NO AND A.SC_NO = B.SC_NO) OS
                            WHERE SC_LOT.SC_NO = PO_HD.SC_NO
                            AND SC_LOT.LOT_NO = PO_HD.LOT_NO
                            AND SC_LOT.LOT_NO = BPO.LOT_NO
                            AND SC_LOT.SC_NO = BPO.SC_NO
                            AND STYLE_HD.STYLE_NO = SC_HD.STYLE_NO
                            AND SC_HD.SC_NO = SC_LOT.SC_NO
                            AND SC_HD.CUSTOMER_CD = GEN_CUSTOMER.CUSTOMER_CD
                            AND OS.BUYER_JO_NO = BPO.BUYER_JO_NO
                            AND PO_HD.JO_NO = '" + PONO + "'";
            return DBUtility.GetTable(SQL, "EEL");
        }

        public static DataTable GetCutAndShipmentReportDetail(string PONO)
        {
            string SQL = "";
            SQL += @" SELECT   SCX_LOT_BPO_HD.BUYER_JO_NO, SCX_LOT_BPO_HD.BPO_ID,
                            SCX_LOT_BPO_DT.COLOR_CODE, SCX_LOT_BPO_DT.SIZE_CODE1,
                            SUM (SCX_LOT_BPO_DT.QTY) ORDER_QTY, SUM (SCX_LOT_BPO_DT.QTY) CUTQTY,
                            STYLE_COLOR.COLOR_DESC CORLORDESC, P.PERCENTOVER, P.PERCENTSHORT,
                            ROUND (SUM (SCX_LOT_BPO_DT.QTY) * (1 + P.PERCENTOVER / 100)
                            ) QTYOVERALLOWED,
                            ROUND (SUM (SCX_LOT_BPO_DT.QTY) * (1 - P.PERCENTSHORT / 100)
                            ) QTYSHORTALLOWED,
                            ROUND (SUM (SCX_LOT_BPO_DT.QTY) * P.PERCENTOVER / 100) QTYOVER,
                            ROUND (SUM (SCX_LOT_BPO_DT.QTY) * P.PERCENTSHORT / 100) QTYSHORT
                            FROM SCX_LOT_BPO_HD,
                            SCX_LOT_BPO_DT,
                            PO_HD,
                            SC_HD,
                            STYLE_COLOR,
                            (SELECT A.BUYER_JO_NO, BPO_ID,
                            DECODE (A.PERCENT_OVER_ALLOWED,
                            NULL, B.POA,
                            0, POA,
                            A.PERCENT_OVER_ALLOWED
                            ) PERCENTOVER,
                            DECODE (A.PERCENT_SHORT_ALLOWED,
                            NULL, B.PSA,
                            0, PSA,
                            A.PERCENT_SHORT_ALLOWED
                            ) PERCENTSHORT
                            FROM SCX_LOT_BPO_HD A,
                            (SELECT PO_HD.SC_NO, PO_HD.LOT_NO, PO_HD.JO_NO,
                            SC_LOT.PERCENT_OVER_ALLOWED POA,
                            SC_LOT.PERCENT_SHORT_ALLOWED PSA
                            FROM SC_LOT, PO_HD
                            WHERE SC_LOT.SC_NO = PO_HD.SC_NO
                            AND SC_LOT.LOT_NO = PO_HD.LOT_NO
                            AND PO_HD.JO_NO = '" + PONO + "'";
            SQL += @" AND NOT SC_LOT.PERCENT_OVER_ALLOWED IS NULL) B
                            WHERE A.LOT_NO = B.LOT_NO AND A.SC_NO = B.SC_NO) P
                            WHERE PO_HD.SC_NO = SCX_LOT_BPO_HD.SC_NO
                            AND SCX_LOT_BPO_HD.LOT_NO = PO_HD.LOT_NO
                            AND SCX_LOT_BPO_DT.BPO_ID = SCX_LOT_BPO_HD.BPO_ID
                            AND SC_HD.SC_NO = PO_HD.SC_NO
                            AND SC_HD.STYLE_NO = STYLE_COLOR.STYLE_NO
                            AND SCX_LOT_BPO_DT.COLOR_CODE = STYLE_COLOR.COLOR_CODE
                            AND SC_HD.STYLE_REV_NO = STYLE_COLOR.STYLE_REV_NO
                            AND SCX_LOT_BPO_HD.BUYER_JO_NO = P.BUYER_JO_NO
                            AND PO_HD.JO_NO = '" + PONO + "'";
            SQL += @" GROUP BY SCX_LOT_BPO_DT.SIZE_CODE1,
                            SCX_LOT_BPO_DT.COLOR_CODE,
                            SCX_LOT_BPO_HD.BPO_ID,
                            SCX_LOT_BPO_HD.BUYER_JO_NO,
                            STYLE_COLOR.COLOR_DESC,
                            P.PERCENTOVER,
                            P.PERCENTSHORT
                            ORDER BY SUM (SCX_LOT_BPO_DT.QTY) DESC";
            return DBUtility.GetTable(SQL, "EEL");
        }

        public static DataTable GetJOColorSizeBreakdown(string JONO)
        {
            //Added by ZouShiChang ON 2013.09.10 将CUT的取数方式更改为新的方式
            
            string SQL = "";
            SQL += @" SELECT A.* FROM(
                                select cut_lay_hd.SIZE_CD,cut_lay_dt.COLOR_CD
                                ,sum(cut_lay_hd.RATIO*cut_lay_dt.PLYS) CUTQTY 
                                from cut_lay, cut_lay_hd,cut_lay_dt
                                where cut_lay_hd.LAY_TRANS_ID=cut_lay_dt.LAY_TRANS_ID
                                and cut_lay.lay_id=cut_lay_hd.lay_id
                                and cut_lay.JOB_ORDER_NO='"+JONO+"'";
            SQL += @" group by cut_lay_hd.SIZE_CD,cut_lay_dt.COLOR_CD
                                ) A
                                LEFT JOIN
                                (SELECT SIZE_CODE,SEQUENCE FROM 
                                dbo.SC_SIZE A WITH(NOLOCK)
                                INNER JOIN JO_HD B WITH(NOLOCK) ON A.SC_NO = B.SC_NO
                                WHERE JO_NO = '"+JONO+"'";
            SQL += @" ) B ON A.SIZE_CD = B.SIZE_CODE
                                order by A.color_cd,B.SEQUENCE";
            
            /*CUT更改为新的取数方式存在问题
            string SQL = "";
            SQL = @"     SELECT A.*
                        FROM   ( SELECT    SIZE_CD ,
                                        Color_CD ,
                                        SUM(QTY) AS CUTQTY
                                FROM      dbo.CUT_BUNDLE_HD
                                WHERE     JOB_ORDER_NO = '" + JONO + @"'
                                GROUP BY  COLOR_CD ,
                                        SIZE_CD
                            ) AS A
                            LEFT JOIN ( SELECT  SIZE_CODE ,
                                                SEQUENCE
                                        FROM    dbo.SC_SIZE A WITH ( NOLOCK )
                                                INNER JOIN JO_HD B WITH ( NOLOCK ) ON A.SC_NO = B.SC_NO
                                        WHERE   JO_NO = '" + JONO + @"'
                                        ) B ON A.SIZE_CD = B.SIZE_CODE
                        ORDER BY A.color_cd ,
                            B.SEQUENCE ";
             */ 
            //Added by ZouShiChang ON 2013.09.10 将CUT的取数方式更改为新的方式

             return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetJobNoHeadInfo(string JONO)
        {
            //Added By ZouShiChang ON 2013.09.10 Start 将CUT的取数方式更改为新的方式
            
            string SQL = "";            
            SQL += @" select sum(qty) as CUTQTY,min(D.create_date) as CUTDATE
                                from cut_lay A WITH(NOLOCK)
                                INNER JOIN cut_lay_hd B WITH(NOLOCK) ON B.lay_id = A.lay_id
                                INNER JOIN cut_lay_dt C WITH(NOLOCK) ON B.LAY_TRANS_ID = C.LAY_TRANS_ID
                                INNER JOIN cut_bundle_hd D WITH(NOLOCK) ON C.LAY_DT_ID = D.LAY_DT_ID
                                AND D.JOB_ORDER_NO = A.JOB_ORDER_NO
                                WHERE A.JOB_ORDER_NO='" + JONO + "'";
            SQL += @" group by A.job_order_no";
            /*CUT更改为新的取数方式存在问题
            string SQL = @"     SELECT SUM(QTY) AS CUTQTY ,
                                       MIN(CREATE_DATE) AS CUTDATE
                                FROM   dbo.CUT_BUNDLE_HD WITH(NOLOCK)
                                WHERE  JOB_ORDER_NO = '" + JONO + @"'
                                GROUP BY JOB_ORDER_NO ";
            */
              //Added By ZouShiChang ON 2013.09.10 End 将CUT的取数方式更改为新的方式
            return DBUtility.GetTable(SQL, "MES");
        }


        public static DataTable GetCutAndShipmentReportSummary(string JONO)
        {
            string SQL = "";

            SQL += @" select a.JO_NO,b.SHORT_NAME,ISNULL(c.CUT_QTY,0) CUT_QTY,e.ORDER_QTY
		                        FROM Jo_hd A
		                        join (select JO_NO,SUM(QTY) AS ORDER_QTY 
		                        from Jo_dt 
		                        where JO_NO='" + JONO + "'";
            SQL += @" group by JO_NO
		                        ) E
		                        on A.JO_NO=E.JO_NO
		                        join gen_customer B
		                        on B.customer_cd=a.customer_cd
		                        left join (select job_order_no,SUM(QTY) AS CUT_QTY 
		                        from cut_bundle_hd 
		                        where job_order_no='" + JONO + "'";
            SQL += @" group by job_order_no
		                        ) C
		                        on C.job_order_no=a.JO_NO
		                        WHERE A.JO_NO='" + JONO + "'";

            return DBUtility.GetTable(SQL, "MES");
        }
    }
}