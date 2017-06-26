using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data.Common;
using System.Text;
using System.Data.OleDb;


/// <summary>
/// Summary description for MesOutSourcePriceSql
/// </summary>
/// 
namespace MESComment
{
    public class MesOutSourcePriceSql
    {
        public MesOutSourcePriceSql()
        {



        }
        public static DataTable GetOutsourceFGISDifForGEG2(string startDate, string endDate, string JoNo, string factoryCd, string GarmentType)
        {
            string SQL = "";
            SQL = @" SELECT DISTINCT JOX.JOB_ORDER_NO
                                    FROM  PRD_JO_OUTPUT_HD JHD  WITH(NOLOCK)
                                    INNER JOIN PRD_JO_OUTPUT_TRX JOX  WITH(NOLOCK)
                                    ON JHD.DOC_NO=JOX.DOC_NO AND JHD.FACTORY_CD = JOX.FACTORY_CD ";
//           SQL+=@"                  WHERE   JOX.PROCESS_CD LIKE 'V_WH_'  AND
//                                    JHD.NEXT_PROCESS_CD LIKE 'TOSTOCKA_' AND ";
            SQL += @"                  WHERE   JOX.PROCESS_CD='V_WH'  AND
                                                JHD.NEXT_PROCESS_CD='TOSTOCKA' AND ";
           SQL+=@"                         DATEADD(HH,-7,JHD.TRX_DATE) >= '" + startDate + "' AND DATEADD(HH,-7,JHD.TRX_DATE) <= '" + endDate + "' AND ";
            SQL += @" JHD.FACTORY_CD='" + factoryCd + "' AND";
            SQL += @" EXISTS(SELECT 1 FROM JO_HD WITH(NOLOCK) WHERE JO_NO = JOX.JOB_ORDER_NO 
                                    AND GARMENT_TYPE_CD LIKE '%" + GarmentType + "%')";
            if (!JoNo.Equals(""))
            {
                SQL += "    AND JOX.JOB_ORDER_NO LIKE '%" + JoNo + "%'";
            }
            DataTable MES_Table = DBUtility.GetTable(SQL, "MES_UPDATE");

            DataTable dtFGIS = GetOutsourceFGISDifForGEG1(startDate, endDate, JoNo, factoryCd, GarmentType, MES_Table);
            string InsertSQL = @"   DECLARE @A1 NVARCHAR(20)
                                                SET @A1 = ''
                                                IF   OBJECT_ID( 'TEMPDB..#TEMP_FGIS')   IS   NOT   NULL  DROP   TABLE   #TEMP_FGIS;  
                                                SELECT @A1 AS JOB_ORDER_NO,@A1 AS COLOR_CODE,@A1 AS SIZE_CODE,0 AS QTY
                                                INTO #TEMP_FGIS
                                                DELETE FROM #TEMP_FGIS
                                                ";
            if (dtFGIS.Rows.Count > 0)
            {
                foreach (DataRow dr in dtFGIS.Rows)
                {
                    InsertSQL += @"   INSERT INTO #TEMP_FGIS VALUES('" + dr["JOB_ORDER_NO"].ToString() + "','" + dr["COLOR_CD"].ToString() + "','" + dr["SIZE_CD"].ToString() + "','" + dr["QTY"] + "');";
                    InsertSQL += @"
                                                ";
                }
            }

            SQL = InsertSQL + "";
            //存储过程中有写死的process和next process;
            SQL += @"EXEC [dbo].[Get_FGISDif] '" + startDate + "','" + endDate + "','','','" + factoryCd + "','" + JoNo + "','" + GarmentType + "'";
            return DBUtility.GetTable(SQL, "MES_UPDATE");
        }
        public static DataTable GetOutsourceFGISDifForGEG1(string startDate, string endDate, string JoNo, string FactoryCD, string GarmentType, DataTable MES_Table)
        {
            DbConnection InvConn = MESComment.DBUtility.GetConnection("inv_support");
            string SQL = "";
            SQL = "DELETE FROM Whq_JOB_ORDER_NO";
            DBUtility.GetTable(SQL, InvConn);
            SQL = @"  INSERT INTO Whq_JOB_ORDER_NO
                               SELECT DISTINCT A.JOB_ORDER_NO
                               FROM (SELECT TRUNC (A.CREATE_DATE - 7 / 24) AS TRANS_DATE,
                                            UPPER (B.REFERENCE_NO) AS JOB_ORDER_NO,
                                            C.QTY * E.MULTIPLIER AS QTY,
                                            CASE WHEN D.TRANS_CD IN ('KBR', 'WBR', 'MBR', 'TO-K','TO-W', 'TO', 'TI-K','TI-W', 'TI', 'ITF-K','ITF-W')
                                                  THEN 'R'
                                               WHEN D.TRANS_CD IN ('KBI', 'WBI', 'RTW','KBI-CFM', 'WBI-CFM')
                                                  THEN 'I'
                                            END AS TRANS_TYPE,
                                            CURRENT_DATE AS CREATE_DATE, A.TRANS_HEADER_ID,
                                            'N/A' AS LINE_CD, PH.GARMENT_TYPE_CD, B.BUYER_PO_NO,
                                            C.ITEM_ID
                                       FROM INV_TRANS_HD A,
                                            INV_TRANS_LINES B,
                                            INV_TRANS_LINE_ITEMS C,
                                            INV_TRANS_CODE D,
                                            INVENTORY.INV_TRANS_TYPE E,
                                            INV_STORE_CODES F,
                                            INV_TRANS_HD H,
                                            ESCMOWNER.PO_HD PH
                                        WHERE A.TRANS_HEADER_ID = B.TRANS_HEADER_ID
                                        AND B.TRANS_LINE_ID = C.TRANS_LINE_ID
                                        AND A.REF_OUT_HEADER_ID = H.TRANS_HEADER_ID(+)
                                        AND A.TRANS_CD = D.TRANS_CD
                                        AND B.REFERENCE_NO = PH.PO_NO(+)
                                        AND ((    D.TRANS_TYPE_CD = 'R' AND D.TRANS_CD IN ('KBR', 'WBR', 'MBR', 'RTW')) 
                                             OR (    D.TRANS_TYPE_CD = 'I' AND D.TRANS_CD IN ('KBI', 'WBI', 'ITF-K', 'ITF-W','KBI-CFM', 'WBI-CFM'))
                                             OR (    D.TRANS_TYPE_CD = 'TO' AND A.TO_STORE_CD = 'GEG-EASN')
                                             OR (    D.TRANS_TYPE_CD = 'TI' AND H.FROM_STORE_CD = 'GEG-EASN'))
                                        AND A.FROM_STORE_CD LIKE '" + FactoryCD + "%' ";
            SQL += @"        AND A.STATUS = 'F'
                                        AND D.TRANS_TYPE_CD = E.TRANS_TYPE_CD
                                        AND A.FROM_STORE_CD = F.STORE_CD
                                        AND F.STOCK_CLASS_CD IN ('A11', 'A12')
                                        AND F.ACTIVE = 'Y' 
                                        AND B.REFERENCE_NO LIKE '%" + JoNo + "%') A,";
            SQL += @"    INV_ITEM_CODE IC,
                                    INV_ITEM_ATTRIBUTE IA,
                                    STYLE_HD ST
                                WHERE A.ITEM_ID = IC.ITEM_ID
                                AND A.ITEM_ID = IA.ITEM_ID(+)
                                AND A.BUYER_PO_NO = IA.BUYER_PO_NO(+)
                                AND IC.STYLE_NO = ST.STYLE_NO(+)
                                AND IC.STYLE_REV_NO = ST.STYLE_REV_NO(+)
                                AND A.GARMENT_TYPE_CD LIKE '%" + GarmentType + "%'";
            if (!startDate.Equals(""))
            {
                SQL += " AND A.TRANS_DATE >= to_date('" + startDate + "','yyyy-MM-dd')";
            }
            if (!endDate.Equals(""))
            {
                SQL += " AND A.TRANS_DATE <= to_date('" + endDate + "','yyyy-MM-dd')";
            }
            DBUtility.GetTable(SQL, InvConn);

            string S = "";
            if (MES_Table.Rows.Count > 0)
            {
                foreach (DataRow dr in MES_Table.Rows)
                {
                    SQL = @"INSERT INTO Whq_JOB_ORDER_NO(JOB_ORDER_NO) VALUES('" + dr["JOB_ORDER_NO"].ToString() + "')";
                    S += SQL;
                    DBUtility.GetTable(SQL, InvConn);
                }
            }
            SQL = "";
            SQL += @"    SELECT DISTINCT W.JOB_ORDER_NO, QTY,COLOR_CD,SIZE_CD
                                    FROM WHQ_JOB_ORDER_NO W
                                    LEFT JOIN (
                                    SELECT DISTINCT A.JOB_ORDER_NO,
                                    UPPER (NVL (DECODE (IA.ESCM_COLOR_CODE,'-', '',IA.ESCM_COLOR_CODE),DECODE (NVL (IC.ESCM_COLOR_CODE, ''),'', IC.COLOR_CODE,IC.ESCM_COLOR_CODE))) AS COLOR_CD,
                                    UPPER (DECODE (IA.ESCM_SIZE_CODE1 || IA.ESCM_SIZE_CODE2,NULL, DECODE (   NVL (IC.ESCM_SIZE_CODE1, '-')|| ' '|| NVL (IC.ESCM_SIZE_CODE2, '-'),'- -', 
                                        DECODE(IC.SIZE_CODE,'-', DECODE(IC.SIZE1_CODE,'-', '',IC.SIZE1_CODE)|| DECODE(IC.SIZE2_CODE,'-', '',IC.SIZE2_CODE),IC.SIZE_CODE),NVL (IC.ESCM_SIZE_CODE1, '')|| ' '|| 
                                        NVL (IC.ESCM_SIZE_CODE2, '')),DECODE (IA.ESCM_SIZE_CODE1,'-', '',IA.ESCM_SIZE_CODE1)|| ''|| DECODE (IA.ESCM_SIZE_CODE2,'-', '',IA.ESCM_SIZE_CODE2))) AS SIZE_CD,
                                    SUM (A.QTY) AS QTY
                               FROM (SELECT TRUNC (A.CREATE_DATE - 7 / 24) AS TRANS_DATE,
                                            UPPER (B.REFERENCE_NO) AS JOB_ORDER_NO,
                                            C.QTY * E.MULTIPLIER AS QTY,
                                            CASE WHEN D.TRANS_CD IN ('KBR', 'WBR', 'MBR', 'TO-K','TO-W', 'TO', 'TI-K','TI-W', 'TI', 'ITF-K','ITF-W')
                                                  THEN 'R'
                                               WHEN D.TRANS_CD IN('KBI', 'WBI', 'RTW','KBI-CFM', 'WBI-CFM')
                                                  THEN 'I'
                                            END AS TRANS_TYPE,
                                            CURRENT_DATE AS CREATE_DATE, A.TRANS_HEADER_ID,
                                            'N/A' AS LINE_CD, PH.GARMENT_TYPE_CD, B.BUYER_PO_NO,
                                            C.ITEM_ID
                                       FROM INV_TRANS_HD A,
                                            INV_TRANS_LINES B,
                                            INV_TRANS_LINE_ITEMS C,
                                            INV_TRANS_CODE D,
                                            INVENTORY.INV_TRANS_TYPE E,
                                            INV_STORE_CODES F,
                                            INV_TRANS_HD H,
                                            ESCMOWNER.PO_HD PH
                                      WHERE A.TRANS_HEADER_ID = B.TRANS_HEADER_ID
                                        AND B.TRANS_LINE_ID = C.TRANS_LINE_ID
                                        AND A.REF_OUT_HEADER_ID = H.TRANS_HEADER_ID(+)
                                        AND A.TRANS_CD = D.TRANS_CD
                                        AND B.REFERENCE_NO = PH.PO_NO(+)
                                        AND ((    D.TRANS_TYPE_CD = 'R' AND D.TRANS_CD IN ('KBR', 'WBR', 'MBR', 'RTW')) 
                                             OR (    D.TRANS_TYPE_CD = 'I' AND D.TRANS_CD IN ('KBI', 'WBI', 'ITF-K', 'ITF-W','KBI-CFM', 'WBI-CFM'))
                                             OR (    D.TRANS_TYPE_CD = 'TO' AND A.TO_STORE_CD = 'GEG-EASN')
                                             OR (    D.TRANS_TYPE_CD = 'TI' AND H.FROM_STORE_CD = 'GEG-EASN'))
                                        AND A.FROM_STORE_CD LIKE '" + FactoryCD + "%' ";
            SQL += @"        AND A.STATUS = 'F'
                                        AND D.TRANS_TYPE_CD = E.TRANS_TYPE_CD
                                        AND A.FROM_STORE_CD = F.STORE_CD
                                        AND F.STOCK_CLASS_CD IN ('A11', 'A12')
                                        AND F.ACTIVE = 'Y'
                                        AND EXISTS(
                                    SELECT 1 FROM Whq_JOB_ORDER_NO WHERE JOB_ORDER_NO =B.REFERENCE_NO)) A,
                                    INV_ITEM_CODE IC,
                                    INV_ITEM_ATTRIBUTE IA,
                                    STYLE_HD ST
                                    WHERE A.ITEM_ID = IC.ITEM_ID
                                    AND A.ITEM_ID = IA.ITEM_ID(+)
                                    AND A.BUYER_PO_NO = IA.BUYER_PO_NO(+)
                                    AND IC.STYLE_NO = ST.STYLE_NO(+)
                                    AND IC.STYLE_REV_NO = ST.STYLE_REV_NO(+)
                                    GROUP BY A.JOB_ORDER_NO,
                                    UPPER (NVL (DECODE (IA.ESCM_COLOR_CODE,'-', '',IA.ESCM_COLOR_CODE),DECODE (NVL (IC.ESCM_COLOR_CODE, ''),'', IC.COLOR_CODE,IC.ESCM_COLOR_CODE))),
                                    UPPER (DECODE (IA.ESCM_SIZE_CODE1 || IA.ESCM_SIZE_CODE2,NULL, DECODE (   NVL (IC.ESCM_SIZE_CODE1, '-')|| ' '|| NVL (IC.ESCM_SIZE_CODE2, '-'),'- -', 
                                        DECODE(IC.SIZE_CODE,'-', DECODE(IC.SIZE1_CODE,'-', '',IC.SIZE1_CODE)|| DECODE(IC.SIZE2_CODE,'-', '',IC.SIZE2_CODE),IC.SIZE_CODE),NVL (IC.ESCM_SIZE_CODE1, '')|| ' '|| 
                                        NVL (IC.ESCM_SIZE_CODE2, '')),DECODE (IA.ESCM_SIZE_CODE1,'-', '',IA.ESCM_SIZE_CODE1)|| ''|| DECODE (IA.ESCM_SIZE_CODE2,'-', '',IA.ESCM_SIZE_CODE2)))                                    
                             ) T ON W.JOB_ORDER_NO = T.JOB_ORDER_NO";
            DataTable dt = DBUtility.GetTable(SQL, InvConn);
            MESComment.DBUtility.CloseConnection(ref InvConn);
            return dt;

        }
        public static DataTable GetMasOutSourcePriceDetail(string strDate, string strType, bool flag_)
        {
            string strYear = Convert.ToDateTime(strDate).Year.ToString();
            strDate = Convert.ToDateTime(strDate).ToString("MM/dd/yyyy");
            string strCondition1 = "";
            if (flag_ == false) { strCondition1 = " and c.customer_cd!='16293' "; }

            string sql = string.Format(" select c.garment_type_cd,a.value_add_tax,c.fab_pattern,d.subcontractor_cd,d.subcontractor_name, b.*  " +
                                       " from prd_outsource_contract a,PRD_OUTSOURCE_CONTRACT_DT b,jo_hd c,PRD_SUBCONTRACTOR_MASTER d " +
                                       " where a.contract_no=b.contract_no and b.job_order_no=c.jo_no " +
                                       "     and a.subcontractor=d.subcontractor_cd " +
                                       "     and a.approved='Y' and a.subcontractor not in ('GEG00010','GEG00064') " +
                                       "     and b.sah <50 " +
                                       "     and c.garment_type_cd='{0}'  " +
                                       "     {1}  " +
                                       "     and charindex('CUT',b.process_cd)>0 and charindex('SEW',b.process_cd)>0 and charindex('IRON',b.process_cd)>0 " +
                                       "     and b.EXPECT_RECEIVE_DATE>=dbo.DATE_FORMAT('01/01/{2}','mm/dd/yyyy' ) and b.EXPECT_RECEIVE_DATE<=dbo.DATE_FORMAT('{3}','mm/dd/yyyy' ) "
                                       , strType, strCondition1, strYear, strDate
                                       );
            DataTable detailtb = DBUtility.GetTable(sql, "MES");
            return detailtb;
        }

        public static DataTable GetPulloutQtynew(string contractNo, string JoNo, string affect, string period)
        {
            string SQL = "   SELECT isnull(sum(isnull(PLR.PULLOUT_QTY,0)),0) AS QTY FROM ";
            SQL = SQL + "  PRD_JO_DISCREPANCY_PULLOUT_TRX JOX, PRD_OUTSOURCE_CONTRACT_DT ";
            SQL = SQL + "  ODT, PRD_JO_DISCREPANCY_PULLOUT_HD JHD, PRD_OUTSOURCE_CONTRACT ";
            SQL = SQL + "  OHD, PRD_JO_PULLOUT_REASON PLR, PRD_REASON_CODE PLC WHERE ";
            SQL = SQL + "  JOX.DOC_NO=JHD.DOC_NO AND ODT.SEND_ID=JOX.SEND_ID AND ";
            SQL = SQL + "  OHD.CONTRACT_NO=ODT.CONTRACT_NO AND JOX.DOC_NO=JHD.DOC_NO AND ";
            SQL = SQL + "  ODT.JOB_ORDER_NO = JOX.JOB_ORDER_NO AND ";
            //Added By ZouShiChang ON 2013.08.21 Satrt MES 024
            //SQL = SQL + "  OHD.RECEIVE_POINT=JHD.PROCESS_CD AND PLR.TRX_ID=JOX.TRX_ID AND ";
            SQL = SQL + "  OHD.RECEIVE_POINT=JHD.PROCESS_CD AND OHD.RECEIVER_PROCESS_TYPE=JHD.PROCESS_TYPE AND OHD.GARMENT_TYPE=JHD.GARMENT_TYPE AND PLR.TRX_ID=JOX.TRX_ID AND ";
            //Added By ZouShiChang ON 2013.08.21 Satrt MES 024
            SQL = SQL + "  PLC.REASON_CD=PLR.REASON_CD and plc.factory_cd=OHD.factory_cd ";
            switch (affect)
            {
                case "A":
                    SQL += " and PLC.qty_affection='A'";
                    break;
                case "D":
                    SQL += " and PLC.qty_affection='D'";
                    SQL += " and PLC.SHORT_NAME not in ('FABDF','CPNDF','SEWDF','PRTDF','SHADE','OUMTL') ";
                    break;
            }
            SQL = SQL + "        AND JOX.JOB_ORDER_NO ='" + JoNo + "' AND OHD.CONTRACT_NO LIKE ";
            SQL = SQL + "  '%" + contractNo + "%' ";
            if (period != "")
            {
                SQL += " AND TO_CHAR(JHD.TRX_DATE,'YYYYMM')='" + period + "'";
            }
            return DBUtility.GetTable(SQL, "MES");
        }


        public static DataTable GetMaster_OutSourcePrice(string strType)
        {
            DataTable pricetb = DBUtility.GetTable(string.Format("select year,pcs_price,order_qty,sah_price,sah from prd_outsource_parameter where garment_type_cd='{0}'", strType), "MES_UPDATE");
            return pricetb;
        }
        public static DataTable GetsourcePriceByFty(string strType, string Factory_cd)
        {
            DataTable pricetb = DBUtility.GetTable(string.Format("select year,pcs_price,order_qty,sah_price,sah from prd_outsource_parameter_dt where garment_type_cd='{0}' and factory_cd='{1}'", strType, Factory_cd), "MES");
            return pricetb;
        }
        public static DataTable GetDDLFactoryData(string Factory_cd)
        {
            string sql = "select SUBCONTRACTOR_CD,SUBCONTRACTOR_NAME from dbo.PRD_SUBCONTRACTOR_MASTER where factory_cd='" + Factory_cd + "' order by subcontractor_name";
            return DBUtility.GetTable(sql, "MES");
        }




        public static DataTable GetMasOutSourcePrice(string strDate, string strType, bool flag_)
        {

            //string strYear = Convert.ToDateTime(strDate).Year.ToString();
            //strDate = Convert.ToDateTime(strDate).ToString("MM/dd/yyyy");
            //string strYearStart = string.Format("01/01/{0}", Convert.ToDateTime(strDate).Year);
            //string strMonthStart = string.Format("{1}/01/{0}", Convert.ToDateTime(strDate).Year, Convert.ToDateTime(strDate).Month);
            string strYear = strDate.Substring(0, 4);
            string strYearStart = "01/01/" + strYear;
            string strMonthStart = strDate.Substring(5, 2) + "/01/" + strYear;
            strDate = strDate.Substring(5, 5).Replace("-", "/") + "/" + strYear;

            string strCondition1 = ""; string strCondition2 = "";
            if (flag_ == false) { strCondition1 = " and jo.customer_cd!='16293'"; strCondition2 = " and c.customer_cd!='16293'"; }

            string sql = string.Format("select avg_price_m=max(avg_price_m),avg_sew_sah_m=max(avg_sew_sah_m),avg_sah_m=max(avg_sah_m),avg_sew_sah_price_m=max(avg_sew_sah_price_m), " +
                       "         avg_price_act_m=max(avg_price_act_m),avg_sew_sah_act_m=max(avg_sew_sah_act_m),avg_sah_act_m=max(avg_sah_act_m),avg_sew_sah_price_act_m=max(avg_sew_sah_price_act_m)," +
                       "         avg_price_y=max(avg_price_y),avg_sew_sah_y=max(avg_sew_sah_y),avg_sah_y=max(avg_sah_y),avg_sew_sah_price_y=max(avg_sew_sah_price_y)," +
                       "         budget_price=max(budget_price),budget_sah=max(budget_sah),budget_sah_price=max(budget_sah_price)," +
                       "         standard_price=max(standard_price),standard_sah_price=max(standard_sah_price)," +
                       "         avg_price_act_y=max(avg_price_act_y),avg_sew_sah_act_y=max(avg_sew_sah_act_y),avg_sah_act_y=max(avg_sah_act_y),avg_sew_sah_price_act_y=max(avg_sew_sah_price_act_y)," +
                       "        avg_price_last_y=max(avg_price_last_y), " +
                       "        avg_sew_sah_last_y=max(avg_sew_sah_last_y), " +
                       "        avg_sah_last_y=max(avg_sah_last_y), " +
                       "        avg_sew_sah_price_last_y=max(avg_sew_sah_price_last_y), " +
                       "        bal_qty=max(bal_qty) " +
                       "     from " +
                       "     ( " +
                       "     select avg_price_m=(sum(b.plan_issue_qty*sub_contract_price/(1+isnull(a.value_add_tax,0)))/sum(b.plan_issue_qty)), " +
                       "         avg_sew_sah_m=( sum(b.plan_issue_qty*b.sew_sah)/sum(b.plan_issue_qty) ), " +
                       "         avg_sah_m=( sum(b.plan_issue_qty*b.sah)/sum(b.plan_issue_qty) ), " +
                       "         avg_sew_sah_price_m=(sum(b.plan_issue_qty*sub_contract_price/(1+isnull(a.value_add_tax,0)))/sum(b.plan_issue_qty/12*b.sew_sah)), " +
                       "         avg_price_act_m=(select sum(JOX.OUTPUT_QTY*odt.sub_contract_price/(1+isnull(ohd.value_add_tax,0)))/sum(JOX.OUTPUT_QTY) " +
                       "                             from PRD_JO_OUTPUT_TRX JOX,PRD_JO_OUTPUT_HD JHD,PRD_OUTSOURCE_CONTRACT_DT ODT,PRD_OUTSOURCE_CONTRACT OHD,jo_hd jo " +
                       "                             where JOX.DOC_NO=JHD.DOC_NO and ODT.SEND_ID=JOX.SEND_ID AND ODT.JOB_ORDER_NO=JOX.JOB_ORDER_NO " +
                       //Added By ZouShiChang ON 2013.08.20 Start MES 024
                       //"                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT  " +
                       "                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT AND JOX.PROCESS_GARMENT_TYPE=OHD.GARMENT_TYPE  " +
                       //Added By ZouShiChang ON 2013.08.20 End MES 024
                       "                                 and JHD.NEXT_PROCESS_CD NOT IN('TOSTOCKL','TOSTOCKL') AND JHD.NEXT_PROCESS_CD NOT IN('TOSTOCKL2','TOSTOCKL2')  " +
                       "                                 and ODT.CONTRACT_NO=OHD.CONTRACT_NO  " +
                       "                                 AND OHD.STATUS!='CAN' and OHD.subcontractor not in ('GEG00010','GEG00064') and OHD.approved='Y'  " +
                       "                                 and ODT.job_order_no=jo.jo_no and jo.garment_type_cd='{0}' {4} " +
                       "                                 and JOX.TRX_DATE>=dbo.DATE_FORMAT('{1}','mm/dd/yyyy' ) and JOX.TRX_DATE<=dbo.DATE_FORMAT('{6}','mm/dd/yyyy' ) " +
                       "                                 and charindex('CUT',ODT.process_cd)>0 and charindex('SEW',ODT.process_cd)>0 and charindex('IRON',ODT.process_cd)>0), " +
                       "         avg_sew_sah_act_m=(select sum(JOX.OUTPUT_QTY*odt.sew_sah)/sum(JOX.OUTPUT_QTY) " +
                       "                             from PRD_JO_OUTPUT_TRX JOX,PRD_JO_OUTPUT_HD JHD,PRD_OUTSOURCE_CONTRACT_DT ODT,PRD_OUTSOURCE_CONTRACT OHD,jo_hd jo " +
                       "                             where JOX.DOC_NO=JHD.DOC_NO and ODT.SEND_ID=JOX.SEND_ID AND ODT.JOB_ORDER_NO=JOX.JOB_ORDER_NO " +
                       //Added By ZouShiChang ON 2013.08.20 Start MES 024
                       //"                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT  " +
                       "                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT AND JOX.PROCESS_GARMENT_TYPE=OHD.GARMENT_TYPE " +
                       //Added By ZouShiChang ON 2013.08.20 End MES 024
                       "                                and JHD.NEXT_PROCESS_CD  NOT IN('TOSTOCKL','TOSTOCKL') AND JHD.NEXT_PROCESS_CD NOT IN('TOSTOCKL2','TOSTOCKL2')  " +
                       "                                 and ODT.CONTRACT_NO=OHD.CONTRACT_NO  " +
                       "                                AND OHD.STATUS!='CAN' and OHD.subcontractor not in ('GEG00010','GEG00064') and OHD.approved='Y'  " +
                       "                                 and ODT.job_order_no=jo.jo_no and jo.garment_type_cd='{0}' {4} " +
                       "                                 and JOX.TRX_DATE>=dbo.DATE_FORMAT('{1}','mm/dd/yyyy' ) and JOX.TRX_DATE<=dbo.DATE_FORMAT('{6}','mm/dd/yyyy' ) " +
                       "                                 and charindex('CUT',ODT.process_cd)>0 and charindex('SEW',ODT.process_cd)>0 and charindex('IRON',ODT.process_cd)>0), " +
                       "         avg_sah_act_m=(select sum(JOX.OUTPUT_QTY*odt.sah)/sum(JOX.OUTPUT_QTY) " +
                       "                             from PRD_JO_OUTPUT_TRX JOX,PRD_JO_OUTPUT_HD JHD,PRD_OUTSOURCE_CONTRACT_DT ODT,PRD_OUTSOURCE_CONTRACT OHD,jo_hd jo " +
                       "                             where JOX.DOC_NO=JHD.DOC_NO and ODT.SEND_ID=JOX.SEND_ID AND ODT.JOB_ORDER_NO=JOX.JOB_ORDER_NO " +
                       //Added By ZouShiChang ON 2013.08.20 Start MES 024
                       //"                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT  " +
                       "                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT AND JOX.PROCESS_GARMENT_TYPE=OHD.GARMENT_TYPE " +
                       //Added By ZouShiChang ON 2013.08.20 End MES 024
                       "                                and JHD.NEXT_PROCESS_CD  NOT IN('TOSTOCKL','TOSTOCKL') AND JHD.NEXT_PROCESS_CD NOT IN('TOSTOCKL2','TOSTOCKL2')  " +
                       "                                 and ODT.CONTRACT_NO=OHD.CONTRACT_NO  " +
                       "                                AND OHD.STATUS!='CAN' and OHD.subcontractor not in ('GEG00010','GEG00064') and OHD.approved='Y'  " +
                       "                                 and ODT.job_order_no=jo.jo_no and jo.garment_type_cd='{0}' {4} " +
                       "                                 and JOX.TRX_DATE>=dbo.DATE_FORMAT('{1}','mm/dd/yyyy' ) and JOX.TRX_DATE<=dbo.DATE_FORMAT('{6}','mm/dd/yyyy' ) " +
                       "                                 and charindex('CUT',ODT.process_cd)>0 and charindex('SEW',ODT.process_cd)>0 and charindex('IRON',ODT.process_cd)>0), " +
                       "         avg_sew_sah_price_act_m=(select sum(JOX.OUTPUT_QTY*odt.sub_contract_price/(1+isnull(ohd.value_add_tax,0)))/sum(JOX.OUTPUT_QTY/12*odt.sew_sah) " +
                       "                             from PRD_JO_OUTPUT_TRX JOX,PRD_JO_OUTPUT_HD JHD,PRD_OUTSOURCE_CONTRACT_DT ODT,PRD_OUTSOURCE_CONTRACT OHD,jo_hd jo " +
                       "                             where JOX.DOC_NO=JHD.DOC_NO and ODT.SEND_ID=JOX.SEND_ID AND ODT.JOB_ORDER_NO=JOX.JOB_ORDER_NO " +
                       //Added By ZouShiChang ON 2013.08.20 Start MES 024
                       //"                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT  " +
                       "                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT AND JOX.PROCESS_GARMENT_TYPE=OHD.GARMENT_TYPE " +
                       //Added By ZouShiChang ON 2013.08.20 End MES 024
                       "                                 and JHD.NEXT_PROCESS_CD  NOT IN('TOSTOCKL','TOSTOCKL') AND JHD.NEXT_PROCESS_CD NOT IN('TOSTOCKL2','TOSTOCKL2')  " +
                       "                                 and ODT.CONTRACT_NO=OHD.CONTRACT_NO  " +
                       "                                 AND OHD.STATUS!='CAN' and OHD.subcontractor not in ('GEG00010','GEG00064') and OHD.approved='Y'  " +
                       "                                 and ODT.job_order_no=jo.jo_no and jo.garment_type_cd='{0}' {4} " +
                       "                                 and JOX.TRX_DATE>=dbo.DATE_FORMAT('{1}','mm/dd/yyyy' ) and JOX.TRX_DATE<=dbo.DATE_FORMAT('{6}','mm/dd/yyyy' ) " +
                       "                                 and charindex('CUT',ODT.process_cd)>0 and charindex('SEW',ODT.process_cd)>0 and charindex('IRON',ODT.process_cd)>0), " +
                       "        avg_price_y=null, " +
                       "        avg_sew_sah_y=null, " +
                       "        avg_sah_y=null, " +
                       "        avg_sew_sah_price_y=null, " +
                       "        budget_price=null, " +
                       "        budget_sah=null, " +
                       "        budget_sah_price=null, " +
                       "        standard_price=null, " +
                       "        standard_sah_price=null, " +
                       "        avg_price_act_y=null, " +
                       "        avg_sew_sah_act_y=null, " +
                       "        avg_sah_act_y=null, " +
                       "        avg_sew_sah_price_act_y=null, " +
                       "        avg_price_last_y=null, " +
                       "        avg_sew_sah_last_y=null, " +
                       "        avg_sah_last_y=null, " +
                       "        avg_sew_sah_price_last_y=null, " +
                       "        bal_qty=null " +
                       "     from prd_outsource_contract a,PRD_OUTSOURCE_CONTRACT_DT b,jo_hd c  " +
                       "     where a.contract_no=b.contract_no and b.job_order_no=c.jo_no " +
                       "         and a.approved='Y' and a.subcontractor not in ('GEG00010','GEG00064') " +
                       "         and b.sah <50 " +
                       "         and c.garment_type_cd='{0}'  " +
                       "         {5} " +
                       "         and charindex('CUT',b.process_cd)>0 and charindex('SEW',b.process_cd)>0 and charindex('IRON',b.process_cd)>0 " +
                       "         and b.EXPECT_RECEIVE_DATE>=dbo.DATE_FORMAT('{1}','mm/dd/yyyy' ) and b.EXPECT_RECEIVE_DATE<=dbo.DATE_FORMAT('{6}','mm/dd/yyyy' ) " +
                       "     union " +
                       "     select avg_price_m=null,avg_sew_sah_m=null,avg_sah_m=null,avg_sew_sah_price_m=null,avg_price_act_m=null,avg_sew_sah_act_m=null,avg_sah_act_m=null,avg_sew_sah_price_act_m=null, " +
                       "         avg_price_y=(sum(b.plan_issue_qty*sub_contract_price/(1+isnull(a.value_add_tax,0)))/sum(b.plan_issue_qty)), " +
                       "         avg_sew_sah_y=( sum(b.plan_issue_qty*b.sew_sah)/sum(b.plan_issue_qty) ), " +
                       "         avg_sah_y=( sum(b.plan_issue_qty*b.sah)/sum(b.plan_issue_qty) ), " +
                       "         avg_sew_sah_price_y=(sum(b.plan_issue_qty*sub_contract_price/(1+isnull(a.value_add_tax,0)))/sum(b.plan_issue_qty/12*b.sew_sah)), " +
                       "         budget_price=(select pcs_price from prd_outsource_parameter where garment_type_cd='{0}' and year={2}), " +
                       "         budget_sah=(select sah from prd_outsource_parameter where garment_type_cd='{0}' and year={2}), " +
                       "         budget_sah_price=(select sah_price from prd_outsource_parameter where garment_type_cd='{0}' and year={2}), " +
                       "         standard_price=(sum(b.plan_issue_qty* (case when c.garment_type_cd='W' then (case when c.fab_pattern='Solid' or c.fab_pattern is null then b.sah*2.55 else b.sah*2.65  end) else (case when c.fab_pattern='Solid' or c.fab_pattern is null then b.sah*3.16+0.9 else b.sah*3.16+1.3  end)  end)/(1+isnull(a.value_add_tax,0)))/sum(b.plan_issue_qty)), " +
                       "         standard_sah_price=(sum(b.plan_issue_qty* (case when c.garment_type_cd='W' then (case when c.fab_pattern='Solid' or c.fab_pattern is null then b.sah*2.55 else b.sah*2.65  end) else (case when c.fab_pattern='Solid' or c.fab_pattern is null then b.sah*3.16+0.9 else b.sah*3.16+1.3  end)  end)/(1+isnull(a.value_add_tax,0)))/sum(b.plan_issue_qty/12*b.sah)), " +
                       "         avg_price_act_y=(select sum(JOX.OUTPUT_QTY*odt.sub_contract_price/(1+isnull(ohd.value_add_tax,0)))/sum(JOX.OUTPUT_QTY) " +
                       "                             from PRD_JO_OUTPUT_TRX JOX,PRD_JO_OUTPUT_HD JHD,PRD_OUTSOURCE_CONTRACT_DT ODT,PRD_OUTSOURCE_CONTRACT OHD,jo_hd jo " +
                       "                             where JOX.DOC_NO=JHD.DOC_NO and ODT.SEND_ID=JOX.SEND_ID AND ODT.JOB_ORDER_NO=JOX.JOB_ORDER_NO " +
                       //Added By ZouShiChang ON 2013.08.20 Start MES 024
                       //"                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT  " +
                       "                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT  AND JOX.PROCESS_GARMENT_TYPE=OHD.GARMENT_TYPE " +
                       //Added By ZouShiChang ON 2013.08.20 End MES 024
                       "                                 and JHD.NEXT_PROCESS_CD  NOT IN('TOSTOCKL','TOSTOCKL') AND JHD.NEXT_PROCESS_CD NOT IN('TOSTOCKL2','TOSTOCKL2')  " +
                       "                                 and ODT.CONTRACT_NO=OHD.CONTRACT_NO  " +
                       "                                 AND OHD.STATUS!='CAN' and OHD.subcontractor not in ('GEG00010','GEG00064') and OHD.approved='Y'  " +
                       "                                 and ODT.job_order_no=jo.jo_no and jo.garment_type_cd='{0}' {4} " +
                       "                                 and JOX.TRX_DATE>=dbo.DATE_FORMAT('{3}','mm/dd/yyyy' ) and JOX.TRX_DATE<=dbo.DATE_FORMAT('{6}','mm/dd/yyyy' ) " +
                       "                                 and charindex('CUT',ODT.process_cd)>0 and charindex('SEW',ODT.process_cd)>0 and charindex('IRON',ODT.process_cd)>0), " +
                       "         avg_sew_sah_act_y=(select sum(JOX.OUTPUT_QTY*odt.sew_sah)/sum(JOX.OUTPUT_QTY) " +
                       "                             from PRD_JO_OUTPUT_TRX JOX,PRD_JO_OUTPUT_HD JHD,PRD_OUTSOURCE_CONTRACT_DT ODT,PRD_OUTSOURCE_CONTRACT OHD,jo_hd jo " +
                       "                             where JOX.DOC_NO=JHD.DOC_NO and ODT.SEND_ID=JOX.SEND_ID AND ODT.JOB_ORDER_NO=JOX.JOB_ORDER_NO " +
                       //Added By ZouShiChang ON 2013.08.20 Start MES 024
                       //"                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT  " +
                       "                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT AND JOX.PROCESS_GARMENT_TYPE=OHD.GARMENT_TYPE " +
                       //Added By ZouShiChang ON 2013.08.20 End MES 024
                       "                                 and JHD.NEXT_PROCESS_CD  NOT IN('TOSTOCKL','TOSTOCKL') AND JHD.NEXT_PROCESS_CD NOT IN('TOSTOCKL2','TOSTOCKL2')  " +
                       "                                 and ODT.CONTRACT_NO=OHD.CONTRACT_NO  " +
                       "                                 AND OHD.STATUS!='CAN' and OHD.subcontractor not in ('GEG00010','GEG00064') and OHD.approved='Y'  " +
                       "                                 and ODT.job_order_no=jo.jo_no and jo.garment_type_cd='{0}' {4} " +
                       "                                 and JOX.TRX_DATE>=dbo.DATE_FORMAT('{3}','mm/dd/yyyy' ) and JOX.TRX_DATE<=dbo.DATE_FORMAT('{6}','mm/dd/yyyy' ) " +
                       "                                 and charindex('CUT',ODT.process_cd)>0 and charindex('SEW',ODT.process_cd)>0 and charindex('IRON',ODT.process_cd)>0), " +
                       "         avg_sah_act_y=(select sum(JOX.OUTPUT_QTY*odt.sah)/sum(JOX.OUTPUT_QTY) " +
                       "                             from PRD_JO_OUTPUT_TRX JOX,PRD_JO_OUTPUT_HD JHD,PRD_OUTSOURCE_CONTRACT_DT ODT,PRD_OUTSOURCE_CONTRACT OHD,jo_hd jo " +
                       "                             where JOX.DOC_NO=JHD.DOC_NO and ODT.SEND_ID=JOX.SEND_ID AND ODT.JOB_ORDER_NO=JOX.JOB_ORDER_NO " +
                       //Added By ZouShiChang ON 2013.08.20 Start MES 024
                       //"                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT  " +
                       "                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT AND JOX.PROCESS_GARMENT_TYPE=OHD.GARMENT_TYPE " +
                       //Added By ZouShiChang ON 2013.08.20 End MES 024
                       "                                 and JHD.NEXT_PROCESS_CD  NOT IN('TOSTOCKL','TOSTOCKL') AND JHD.NEXT_PROCESS_CD NOT IN('TOSTOCKL2','TOSTOCKL2')  " +
                       "                                 and ODT.CONTRACT_NO=OHD.CONTRACT_NO  " +
                       "                                 AND OHD.STATUS!='CAN' and OHD.subcontractor not in ('GEG00010','GEG00064') and OHD.approved='Y'  " +
                       "                                 and ODT.job_order_no=jo.jo_no and jo.garment_type_cd='{0}' {4} " +
                       "                                 and JOX.TRX_DATE>=dbo.DATE_FORMAT('{3}','mm/dd/yyyy' ) and JOX.TRX_DATE<=dbo.DATE_FORMAT('{6}','mm/dd/yyyy' ) " +
                       "                                 and charindex('CUT',ODT.process_cd)>0 and charindex('SEW',ODT.process_cd)>0 and charindex('IRON',ODT.process_cd)>0), " +
                       "     avg_sew_sah_price_act_y=(select sum(JOX.OUTPUT_QTY*odt.sub_contract_price/(1+isnull(ohd.value_add_tax,0)))/sum(JOX.OUTPUT_QTY/12*odt.sew_sah) " +
                       "                             from PRD_JO_OUTPUT_TRX JOX,PRD_JO_OUTPUT_HD JHD,PRD_OUTSOURCE_CONTRACT_DT ODT,PRD_OUTSOURCE_CONTRACT OHD,jo_hd jo " +
                       "                             where JOX.DOC_NO=JHD.DOC_NO and ODT.SEND_ID=JOX.SEND_ID AND ODT.JOB_ORDER_NO=JOX.JOB_ORDER_NO " +
                       //Added By ZouShiChang ON 2013.08.20 Start MES 024
                       //"                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT  " +
                       "                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT  AND JOX.PROCESS_GARMENT_TYPE=OHD.GARMENT_TYPE " +
                        //Added By ZouShiChang ON 2013.08.20 End MES 024
                       "                                 and JHD.NEXT_PROCESS_CD  NOT IN('TOSTOCKL','TOSTOCKL') AND JHD.NEXT_PROCESS_CD NOT IN('TOSTOCKL2','TOSTOCKL2')  " +
                       "                                 and ODT.CONTRACT_NO=OHD.CONTRACT_NO  " +
                       "                                 AND OHD.STATUS!='CAN' and OHD.subcontractor not in ('GEG00010','GEG00064') and OHD.approved='Y'  " +
                       "                                 and ODT.job_order_no=jo.jo_no and jo.garment_type_cd='{0}' {4} " +
                       "                                 and JOX.TRX_DATE>=dbo.DATE_FORMAT('{3}','mm/dd/yyyy' ) and JOX.TRX_DATE<=dbo.DATE_FORMAT('{6}','mm/dd/yyyy' ) " +
                       "                                 and charindex('CUT',ODT.process_cd)>0 and charindex('SEW',ODT.process_cd)>0 and charindex('IRON',ODT.process_cd)>0), " +
                       "        avg_price_last_y=null, " +
                       "        avg_sew_sah_last_y=null, " +
                       "        avg_sah_last_y=null, " +
                       "        avg_sew_sah_price_last_y=null, " +
                       "     bal_qty=((select order_qty from prd_outsource_parameter where garment_type_cd='{0}' and year={2})- sum(b.plan_issue_qty)) " +
                       "                     from prd_outsource_contract a,PRD_OUTSOURCE_CONTRACT_DT b,jo_hd c " +
                       "                     where a.contract_no=b.contract_no and b.job_order_no=c.jo_no " +
                       "                     and a.approved='Y' " +
                       "                     and a.subcontractor not in ('GEG00010','GEG00064') " +
                       "                     and b.sah <50 " +
                       "                     and c.garment_type_cd='{0}'  " +
                       "                     {5} " +
                       "                     and charindex('CUT',b.process_cd)>0 and charindex('SEW',b.process_cd)>0 and charindex('IRON',b.process_cd)>0 " +
                       "                     and b.EXPECT_RECEIVE_DATE>=dbo.DATE_FORMAT('{3}','mm/dd/yyyy' ) and b.EXPECT_RECEIVE_DATE<=dbo.DATE_FORMAT('{6}','mm/dd/yyyy' ) " +
                       "     union " +
                       "     select avg_price_m=null,avg_sew_sah_m=null,avg_sah_m=null,avg_sew_sah_price_m=null,avg_price_act_m=null,avg_sew_sah_act_m=null,avg_sah_act_m=null,avg_sew_sah_price_act_m=null, " +
                       "        avg_price_y=null, " +
                       "        avg_sew_sah_y=null, " +
                       "        avg_sah_y=null, " +
                       "        avg_sew_sah_price_y=null, " +
                       "        budget_price=null, " +
                       "        budget_sah=null, " +
                       "        budget_sah_price=null, " +
                       "        standard_price=null, " +
                       "        standard_sah_price=null, " +
                       "        avg_price_act_y=null, " +
                       "        avg_sew_sah_act_y=null, " +
                       "        avg_sah_act_y=null, " +
                       "        avg_sew_sah_price_act_y=null, " +
                       "        avg_price_last_y=(sum(JOX.OUTPUT_QTY*odt.sub_contract_price/(1+isnull(ohd.value_add_tax,0)))/sum(JOX.OUTPUT_QTY)), " +
                       "        avg_sew_sah_last_y=(sum(JOX.OUTPUT_QTY*odt.sew_sah)/sum(JOX.OUTPUT_QTY))," +
                       "        avg_sah_last_y=(sum(JOX.OUTPUT_QTY*odt.sah)/sum(JOX.OUTPUT_QTY))," +
                       "        avg_sew_sah_price_last_y=(sum(JOX.OUTPUT_QTY*odt.sub_contract_price/(1+isnull(ohd.value_add_tax,0)))/sum(JOX.OUTPUT_QTY/12*odt.sew_sah)), " +
                       "        bal_qty=null " +
                       "                             from PRD_JO_OUTPUT_TRX JOX,PRD_JO_OUTPUT_HD JHD,PRD_OUTSOURCE_CONTRACT_DT ODT,PRD_OUTSOURCE_CONTRACT OHD,jo_hd jo " +
                       "                             where JOX.DOC_NO=JHD.DOC_NO and ODT.SEND_ID=JOX.SEND_ID AND ODT.JOB_ORDER_NO=JOX.JOB_ORDER_NO " +
                       //Added By ZouShiChang ON 2013.08.20 Start
                       //"                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT  " +
                       "                                 and JOX.PROCESS_CD=OHD.RECEIVE_POINT  AND JOX.PROCESS_GARMENT_TYPE=OHD.GARMENT_TYPE " +
                       //Added By ZouShiChang ON 2013.08.20 Start
                       "                                 and JHD.NEXT_PROCESS_CD  NOT IN('TOSTOCKL','TOSTOCKL') AND JHD.NEXT_PROCESS_CD NOT IN('TOSTOCKL2','TOSTOCKL2')  " +
                       "                                 and ODT.CONTRACT_NO=OHD.CONTRACT_NO  " +
                       "                                 AND OHD.STATUS!='CAN' and OHD.subcontractor not in ('GEG00010','GEG00064') and OHD.approved='Y'  " +
                       "                                 and ODT.job_order_no=jo.jo_no and jo.garment_type_cd='{0}' {4} " +
                       "                                 and JOX.TRX_DATE>=dbo.DATE_FORMAT(DATEADD(yy,-1,'{3}'),'mm/dd/yyyy' ) and JOX.TRX_DATE<=dbo.DATE_FORMAT(DATEADD(yy,-1,'{6}'),'mm/dd/yyyy' ) " +
                       "                                 and charindex('CUT',ODT.process_cd)>0 and charindex('SEW',ODT.process_cd)>0 and charindex('IRON',ODT.process_cd)>0 " +
                       "     ) m ", strType, strMonthStart, strYear, strYearStart, strCondition1, strCondition2, strDate);

            DataTable pricetb = DBUtility.GetTable(sql, "MES");
            return pricetb;
        }





        //MesMain
        public static DataTable GetReportDetailByAll(string factory, string date, string garmentType, int isCheck)
        {
            string SQL = "    select  ";
            SQL = SQL + "     avg_price_m=round(max(avg_price_m),2),avg_sew_sah_m=round(max(avg_sew_sah_m),3),avg_sah_m=round(max(avg_sah_m),3), ";
            SQL = SQL + "     avg_sew_sah_price_m=round(max(avg_sew_sah_price_m),3),avg_price_act_m=round(max(avg_price_act_m),3), ";
            SQL = SQL + "     avg_sew_sah_act_m=round(max(avg_sew_sah_act_m),3),avg_sah_act_m=round(max(avg_sah_act_m),3), ";
            SQL = SQL + "     avg_sew_sah_price_act_m=round(max(avg_sew_sah_price_act_m),3),avg_price_y=round(max(avg_price_y),3), ";
            SQL = SQL + "     avg_sew_sah_y=round(max(avg_sew_sah_y),3),avg_sah_y=round(max(avg_sah_y),3),avg_sew_sah_price_y=round(max(avg_sew_sah_price_y),3), ";
            SQL = SQL + "     budget_price=round(max(budget_price),3),budget_sew_sah=round(max(budget_sew_sah),3),budget_sew_sah_price=round(max(budget_sew_sah_price),3), ";
            SQL = SQL + "     standard_price=round(max(standard_price),3),standard_sah_price=round(max(standard_sah_price),3), ";
            SQL = SQL + "     avg_price_act_y=round(max(avg_price_act_y),3),avg_sew_sah_act_y=round(max(avg_sew_sah_act_y),3),avg_sah_act_y=round(max(avg_sah_act_y),3), ";
            SQL = SQL + "     avg_sew_sah_price_act_y=round(max(avg_sew_sah_price_act_y),3),avg_price_last_y=round(max(avg_price_last_y),3), ";
            SQL = SQL + "     avg_sew_sah_last_y=round(max(avg_sew_sah_last_y),3),avg_sah_last_y=round(max(avg_sah_last_y),3), ";
            SQL = SQL + "     avg_sew_sah_price_last_y=round(max(avg_sew_sah_price_last_y),3),bal_qty=round(max(bal_qty),3) from  ";
            SQL = SQL + "     [FN_OUTSOURCING_PRICE_MONITOR]('" + factory + "','" + date + "','" + garmentType + "'," + isCheck + ") ";

            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetReportDetailBySubcontractor(string Subcontractor, string date, string garmentType, int isCheck)
        {
            string SQL = "   select a.SUBCONTRACTOR_CD,b.SUBCONTRACTOR_NAME,";
            SQL = SQL + "    avg_price_m=max(avg_price_m),avg_sew_sah_m=max(avg_sew_sah_m),avg_sah_m=max(avg_sah_m),";
            SQL = SQL + "    avg_sew_sah_price_m=max(avg_sew_sah_price_m),avg_price_act_m=max(avg_price_act_m),";
            SQL = SQL + "    avg_sew_sah_act_m=max(avg_sew_sah_act_m),avg_sah_act_m=max(avg_sah_act_m),";
            SQL = SQL + "    avg_sew_sah_price_act_m=max(avg_sew_sah_price_act_m),avg_price_y=max(avg_price_y),";
            SQL = SQL + "    avg_sew_sah_y=max(avg_sew_sah_y),avg_sah_y=max(avg_sah_y),avg_sew_sah_price_y=max(avg_sew_sah_price_y),";
            SQL = SQL + "    budget_price=max(budget_price),budget_sew_sah=max(budget_sew_sah),budget_sew_sah_price=max(budget_sew_sah_price),";
            SQL = SQL + "    standard_price=max(standard_price),standard_sah_price=max(standard_sah_price),";
            SQL = SQL + "    avg_price_act_y=max(avg_price_act_y),avg_sew_sah_act_y=max(avg_sew_sah_act_y),avg_sah_act_y=max(avg_sah_act_y),";
            SQL = SQL + "    avg_sew_sah_price_act_y=max(avg_sew_sah_price_act_y),avg_price_last_y=max(avg_price_last_y),";
            SQL = SQL + "    avg_sew_sah_last_y=max(avg_sew_sah_last_y),avg_sah_last_y=max(avg_sah_last_y),";
            SQL = SQL + "    avg_sew_sah_price_last_y=max(avg_sew_sah_price_last_y),bal_qty=max(bal_qty) from ";
            SQL = SQL + "    [FN_OUTSOURCING_PRICE_MONITOR_BY_SUBCONTRACTOR]('" + Subcontractor + "','" + date + "','" + garmentType + "'," + isCheck + ") a,PRD_SUBCONTRACTOR_MASTER b";
            SQL = SQL + "    Where a.SUBCONTRACTOR_CD=b.SUBCONTRACTOR_CD";
            SQL = SQL + "    GROUP BY a.SUBCONTRACTOR_CD,b.SUBCONTRACTOR_NAME";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetReportDetailByCustomer(string customerCd, string date, string garmentType, int isCheck)
        {
            string SQL = " select a.CUSTOMER_CD,b.Short_Name, ";
            SQL = SQL + "    avg_price_m=max(avg_price_m),avg_sew_sah_m=max(avg_sew_sah_m),avg_sah_m=max(avg_sah_m), ";
            SQL = SQL + "    avg_sew_sah_price_m=max(avg_sew_sah_price_m),avg_price_act_m=max(avg_price_act_m), ";
            SQL = SQL + "    avg_sew_sah_act_m=max(avg_sew_sah_act_m),avg_sah_act_m=max(avg_sah_act_m), ";
            SQL = SQL + "    avg_sew_sah_price_act_m=max(avg_sew_sah_price_act_m),avg_price_y=max(avg_price_y), ";
            SQL = SQL + "    avg_sew_sah_y=max(avg_sew_sah_y),avg_sah_y=max(avg_sah_y),avg_sew_sah_price_y=max(avg_sew_sah_price_y), ";
            SQL = SQL + "    budget_price=max(budget_price),budget_sew_sah=max(budget_sew_sah),budget_sew_sah_price=max(budget_sew_sah_price), ";
            SQL = SQL + "    standard_price=max(standard_price),standard_sah_price=max(standard_sah_price), ";
            SQL = SQL + "    avg_price_act_y=max(avg_price_act_y),avg_sew_sah_act_y=max(avg_sew_sah_act_y),avg_sah_act_y=max(avg_sah_act_y), ";
            SQL = SQL + "    avg_sew_sah_price_act_y=max(avg_sew_sah_price_act_y),avg_price_last_y=max(avg_price_last_y), ";
            SQL = SQL + "    avg_sew_sah_last_y=max(avg_sew_sah_last_y),avg_sah_last_y=max(avg_sah_last_y), ";
            SQL = SQL + "    avg_sew_sah_price_last_y=max(avg_sew_sah_price_last_y),bal_qty=max(bal_qty) from  ";
            SQL = SQL + "    [FN_OUTSOURCING_PRICE_MONITOR_BY_CUSTOMER]('" + customerCd + "','" + date + "','" + garmentType + "'," + isCheck + ") a,GEN_CUSTOMER b ";
            SQL = SQL + "    Where a.CUSTOMER_CD=b.CUSTOMER_CD ";
            SQL = SQL + "    GROUP BY a.CUSTOMER_CD,b.Short_Name ";

            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetMesOutSourcePriceDetail(string factory, string strDate, string strType, bool flag_)
        {
            string strYear = Convert.ToDateTime(strDate).Year.ToString();
            strDate = Convert.ToDateTime(strDate).ToString("MM/dd/yyyy");
            string strCondition1 = "";
            if (flag_ == false) { strCondition1 = " and c.customer_cd!='16293' "; }

            string sql = string.Format(" select c.garment_type_cd,a.value_add_tax,c.fab_pattern,d.subcontractor_cd,d.subcontractor_name, b.*  " +
                                       " from prd_outsource_contract a,PRD_OUTSOURCE_CONTRACT_DT b,jo_hd c,PRD_SUBCONTRACTOR_MASTER d " +
                                       " where a.factory_cd='{4}' and a.contract_no=b.contract_no and b.job_order_no=c.jo_no " +
                                       "     and a.subcontractor=d.subcontractor_cd " +
                                       "     and a.approved='Y' and a.subcontractor not in ('GEG00010','GEG00064') " +
                                       "     and b.sah <50 " +
                                       "     and c.garment_type_cd='{0}'  " +
                                       "     {1}  " +
                                       "     and charindex('CUT',b.process_cd)>0 and charindex('SEW',b.process_cd)>0 and charindex('IRON',b.process_cd)>0 " +
                                       "     and b.EXPECT_RECEIVE_DATE>=dbo.DATE_FORMAT('01/01/{2}','mm/dd/yyyy' ) and b.EXPECT_RECEIVE_DATE<=dbo.DATE_FORMAT('{3}','mm/dd/yyyy' ) "
                                       , strType, strCondition1, strYear, strDate, factory
                                       );
            DataTable detailtb = DBUtility.GetTable(sql, "MES");
            return detailtb;
        }
        public static DataTable GetMesOutSourcePriceDetailByCustomer(string strDate, string strType, bool flag_, string strCustomer)
        {
            string strYear = Convert.ToDateTime(strDate).Year.ToString();
            strDate = Convert.ToDateTime(strDate).ToString("MM/dd/yyyy");
            string strCondition1 = "";
            if (flag_ == false) { strCondition1 = " and c.customer_cd!='16293' "; }

            string sql = string.Format(" select c.garment_type_cd,a.value_add_tax,c.fab_pattern,d.subcontractor_cd,d.subcontractor_name, b.*  " +
                                       " from prd_outsource_contract a,PRD_OUTSOURCE_CONTRACT_DT b,jo_hd c,PRD_SUBCONTRACTOR_MASTER d " +
                                       " where a.contract_no=b.contract_no and b.job_order_no=c.jo_no " +
                                       "     and a.subcontractor=d.subcontractor_cd " +
                                       "     and a.approved='Y' and a.subcontractor not in ('GEG00010','GEG00064') " +
                                       "     and b.sah <50 " +
                                       "     and c.garment_type_cd='{0}'  " +
                                       "     {1}  " +
                                       "     and charindex('CUT',b.process_cd)>0 and charindex('SEW',b.process_cd)>0 and charindex('IRON',b.process_cd)>0 " +
                                       "     and b.EXPECT_RECEIVE_DATE>=dbo.DATE_FORMAT('01/01/{2}','mm/dd/yyyy' ) and b.EXPECT_RECEIVE_DATE<=dbo.DATE_FORMAT('{3}','mm/dd/yyyy' ) " +
                                                                              "          and b.customer_cd in (select FNField from FN_SPLIT_STRING_TB('" + strCustomer + "',';'))"
                                       , strType, strCondition1, strYear, strDate
                                       );
            DataTable detailtb = DBUtility.GetTable(sql, "MES");
            return detailtb;
        }
        public static DataTable GetMesOutSourcePriceDetailBySUBCONTRACTOR(string strDate, string strType, bool flag_, string strSUBCONTRACTOR)
        {
            string strYear = Convert.ToDateTime(strDate).Year.ToString();
            strDate = Convert.ToDateTime(strDate).ToString("MM/dd/yyyy");
            string strCondition1 = "";
            if (flag_ == false) { strCondition1 = " and c.customer_cd!='16293' "; }

            string sql = string.Format(" select c.garment_type_cd,a.value_add_tax,c.fab_pattern,d.subcontractor_cd,d.subcontractor_name, b.*  " +
                                       " from prd_outsource_contract a,PRD_OUTSOURCE_CONTRACT_DT b,jo_hd c,PRD_SUBCONTRACTOR_MASTER d " +
                                       " where a.contract_no=b.contract_no and b.job_order_no=c.jo_no " +
                                       "     and a.subcontractor=d.subcontractor_cd " +
                                       "     and a.approved='Y' and a.subcontractor not in ('GEG00010','GEG00064') " +
                                       "     and b.sah <50 " +
                                       "     and c.garment_type_cd='{0}'  " +
                                       "     {1}  " +
                                       "     and charindex('CUT',b.process_cd)>0 and charindex('SEW',b.process_cd)>0 and charindex('IRON',b.process_cd)>0 " +
                                       "     and b.EXPECT_RECEIVE_DATE>=dbo.DATE_FORMAT('01/01/{2}','mm/dd/yyyy' ) and b.EXPECT_RECEIVE_DATE<=dbo.DATE_FORMAT('{3}','mm/dd/yyyy' ) " +
                                                                              "          and a.SUBCONTRACTOR in (select FNField from FN_SPLIT_STRING_TB('" + strSUBCONTRACTOR + "',';'))"
                                       , strType, strCondition1, strYear, strDate
                                       );
            DataTable detailtb = DBUtility.GetTable(sql, "MES");
            return detailtb;
        }

        //MesOutSourceDetailReport
        public static DataTable GetMasOutSourceDetail(string Type, string factoryCd, string Date, string GarmentType, string GarmentCode, string CustomerCode)
        {
            string SQL = "";
            if (Type != "C")
            {
                SQL = " select m.subcontractor_name,m.short_name,m.contract_no,m.sc_no,sub_contract_price=str(m.sub_contract_price,10,2),m.plan_issue_qty,standard_price=str(m.standard_price,10,2),sah=str(m.sah,10,4),sew_sah=str(m.sew_sah,10,4), ";
                SQL = SQL + "budget_price=str(m.budget_price,10,2),budget_price_fty=str(m.budget_price_fty,10,2),diff_with_standard_price=str(isnull(m.sub_contract_price,0)-isnull(m.standard_price,0),10,2),diff_with_standard_amount=str((isnull(m.sub_contract_price,0)-isnull(m.standard_price,0))*m.plan_issue_qty,10,2), ";
                SQL = SQL + "diff_with_fty_price=str(isnull(m.sub_contract_price,0)-isnull(m.budget_price_fty,0),10,2),diff_with_fty_qty=str((isnull(m.sub_contract_price,0)-isnull(m.budget_price_fty,0))*m.plan_issue_qty,10,2),dbo.DATE_FORMAT(EXPECT_RECEIVE_DATE,'yyyy-mm-dd')  ";
                SQL = SQL + "from  ";
                SQL = SQL + "(select d.subcontractor_name,a.subcontractor,e.short_name,c.customer_cd,a.contract_no,c.sc_no,c.garment_type_cd,sub_contract_price=sum(b.sub_contract_price*b.plan_issue_qty/(1+isnull(a.value_add_tax,0)))/sum(b.plan_issue_qty),plan_issue_qty=sum(b.plan_issue_qty), ";
                SQL = SQL + " standard_price=(sum(b.plan_issue_qty* (case when c.garment_type_cd='W' then (case when c.fab_pattern='Solid' or c.fab_pattern is null then b.sah*2.55 else b.sah*2.65  end) else (case when c.fab_pattern='Solid' or c.fab_pattern is null then b.sah*3.16+0.9 else b.sah*3.16+1.3  end)  end)/(1+isnull(a.value_add_tax,0)))/sum(b.plan_issue_qty)),  ";
                SQL = SQL + " sah=sum(b.sah*b.plan_issue_qty)/sum(b.plan_issue_qty),sew_sah=sum(b.sew_sah*b.plan_issue_qty)/sum(b.plan_issue_qty), ";
                SQL = SQL + " budget_price=(select pcs_price from prd_outsource_parameter where garment_type_cd=c.garment_type_cd and year={5}), ";
                SQL = SQL + " budget_price_fty=(select pcs_price from prd_outsource_parameter_dt where garment_type_cd=c.garment_type_cd and factory_cd=a.subcontractor and year={5}),EXPECT_RECEIVE_DATE=min(EXPECT_RECEIVE_DATE) ";
                SQL = SQL + " from prd_outsource_contract a,PRD_OUTSOURCE_CONTRACT_DT b,jo_hd c,PRD_SUBCONTRACTOR_MASTER d,gen_customer e ";
                SQL = SQL + " where a.factory_CD='{6}' AND  a.contract_no=b.contract_no and b.job_order_no=c.jo_no  ";
                SQL = SQL + " and c.customer_cd=e.customer_cd and a.subcontractor=d.subcontractor_cd ";
                SQL = SQL + " and a.approved='Y'  ";
                SQL = SQL + " and a.subcontractor not in ('GEG00010','GEG00064')  ";
                SQL = SQL + " and b.sah <50  ";
                SQL = SQL + " and {2}   ";
                SQL = SQL + " and {3}   ";
                SQL = SQL + " and charindex('CUT',b.process_cd)>0 and charindex('SEW',b.process_cd)>0 and charindex('IRON',b.process_cd)>0  ";
                SQL = SQL + " and b.EXPECT_RECEIVE_DATE>=dbo.DATE_FORMAT('{0}','mm/dd/yyyy' ) and b.EXPECT_RECEIVE_DATE<=dbo.DATE_FORMAT('{1}','mm/dd/yyyy' )  ";
                SQL = SQL + " group by d.subcontractor_name,a.subcontractor,e.short_name,c.customer_cd,a.contract_no,c.sc_no,c.garment_type_cd ) m ";
                SQL = SQL + " order by m.subcontractor_name,m.short_name,m.contract_no,m.sc_no ";
            }
            else if (Type != "F")
            {
                SQL = " select m.short_name,m.subcontractor_name,m.contract_no,m.sc_no,sub_contract_price=str(m.sub_contract_price,10,2),m.plan_issue_qty,standard_price=str(m.standard_price,10,2),sah=str(m.sah,10,4),sew_sah=str(m.sew_sah,10,4), ";
                SQL = SQL + "budget_price=str(m.budget_price,10,2),budget_price_fty=str(m.budget_price_fty,10,2),diff_with_standard_price=str(isnull(m.sub_contract_price,0)-isnull(m.standard_price,0),10,2),diff_with_standard_amount=str((isnull(m.sub_contract_price,0)-isnull(m.standard_price,0))*m.plan_issue_qty,10,2), ";
                SQL = SQL + "diff_with_fty_price=str(isnull(m.sub_contract_price,0)-isnull(m.budget_price_fty,0),10,2),diff_with_fty_qty=str((isnull(m.sub_contract_price,0)-isnull(m.budget_price_fty,0))*m.plan_issue_qty,10,2),dbo.DATE_FORMAT(EXPECT_RECEIVE_DATE,'yyyy-mm-dd')  ";
                SQL = SQL + "from  ";
                SQL = SQL + "(select d.subcontractor_name,a.subcontractor,e.short_name,c.customer_cd,a.contract_no,c.sc_no,c.garment_type_cd,sub_contract_price=sum(b.sub_contract_price*b.plan_issue_qty/(1+isnull(a.value_add_tax,0)))/sum(b.plan_issue_qty),plan_issue_qty=sum(b.plan_issue_qty), ";
                SQL = SQL + " standard_price=(sum(b.plan_issue_qty* (case when c.garment_type_cd='W' then (case when c.fab_pattern='Solid' or c.fab_pattern is null then b.sah*2.55 else b.sah*2.65  end) else (case when c.fab_pattern='Solid' or c.fab_pattern is null then b.sah*3.16+0.9 else b.sah*3.16+1.3  end)  end)/(1+isnull(a.value_add_tax,0)))/sum(b.plan_issue_qty)),  ";
                SQL = SQL + " sah=sum(b.sah*b.plan_issue_qty)/sum(b.plan_issue_qty),sew_sah=sum(b.sew_sah*b.plan_issue_qty)/sum(b.plan_issue_qty), ";
                SQL = SQL + " budget_price=(select pcs_price from prd_outsource_parameter where garment_type_cd=c.garment_type_cd and year={5}), ";
                SQL = SQL + " budget_price_fty=(select pcs_price from prd_outsource_parameter_dt where garment_type_cd=c.garment_type_cd and factory_cd=a.subcontractor and year={5}),EXPECT_RECEIVE_DATE=min(EXPECT_RECEIVE_DATE) ";
                SQL = SQL + " from prd_outsource_contract a,PRD_OUTSOURCE_CONTRACT_DT b,jo_hd c,PRD_SUBCONTRACTOR_MASTER d,gen_customer e ";
                SQL = SQL + " where  a.factory_CD='{6}' AND a.contract_no=b.contract_no and b.job_order_no=c.jo_no  ";
                SQL = SQL + " and c.customer_cd=e.customer_cd and a.subcontractor=d.subcontractor_cd ";
                SQL = SQL + " and a.approved='Y'  ";
                SQL = SQL + " and a.subcontractor not in ('GEG00010','GEG00064')  ";
                SQL = SQL + " and b.sah <50  ";
                SQL = SQL + " and {2}   ";
                SQL = SQL + " and {4}   ";
                SQL = SQL + " and charindex('CUT',b.process_cd)>0 and charindex('SEW',b.process_cd)>0 and charindex('IRON',b.process_cd)>0  ";
                SQL = SQL + " and b.EXPECT_RECEIVE_DATE>=dbo.DATE_FORMAT('{0}','mm/dd/yyyy' ) and b.EXPECT_RECEIVE_DATE<=dbo.DATE_FORMAT('{1}','mm/dd/yyyy' )  ";
                SQL = SQL + " group by d.subcontractor_name,a.subcontractor,e.short_name,c.customer_cd,a.contract_no,c.sc_no,c.garment_type_cd ) m ";
                SQL = SQL + " order by m.short_name,m.subcontractor_name,m.contract_no,m.sc_no ";
            }
            StringBuilder sqlstr = new StringBuilder();
            if (GarmentType != "")
            {
                GarmentType = "c.garment_type_cd='" + GarmentType + "'";
            }
            else
            {
                GarmentType = "1=1";
            }
            if (GarmentCode != "")
            {
                GarmentCode = "d.subcontractor_cd in ('" + GarmentCode.Replace(";", "','") + "')";
            }
            else
            {
                GarmentCode = "1=1";
            }
            if (CustomerCode != "")
            {
                CustomerCode = "c.customer_cd in ('" + CustomerCode.Replace(";", "','") + "')";
            }
            else
            {
                CustomerCode = "1=1";
            }
            sqlstr.AppendFormat(SQL, Date.Substring(0, 5) + "01-01", Date, GarmentType, GarmentCode, CustomerCode, Date.Substring(0, 4), factoryCd);
            return DBUtility.GetTable(sqlstr.ToString(), "MES");
        }
        public static DataTable GetMasOutSourceFactoryOrCustomer(string Type, string Code, string Name, string Mark)
        {
            string sqlstr = "";
            switch (Type)
            {
                case "F":
                    sqlstr = "select checked='N',code=subcontractor_cd,name=subcontractor_name,mark=address from dbo.PRD_SUBCONTRACTOR_MASTER where 1=1 ";
                    if (HttpContext.Current.Session["site"].ToString() == "DEV")
                    {
                        sqlstr = sqlstr + "and factory_cd='GEG' ";
                    }
                    else
                    {
                        sqlstr = sqlstr + "and factory_cd='" + HttpContext.Current.Session["site"].ToString() + "' ";
                    }

                    if (Code != "")
                    {
                        sqlstr += " and subcontractor_cd = '" + Code + "' ";
                    }
                    if (Name != "")
                    {
                        sqlstr += " and subcontractor_name like '%" + Name + "%' ";
                    }
                    if (Mark != "")
                    {
                        sqlstr += " and address like '%" + Mark + "%'";
                    }
                    sqlstr += " order by subcontractor_cd ";
                    break;
                case "C":
                    sqlstr = "select m.* from (select top 500 checked='N',code=customer_cd,mark=name,name=short_name from gen_customer where 1=1 ";
                    if (Code != "")
                    {
                        sqlstr += " and customer_cd like '" + Code + "%' ";
                    }
                    if (Name != "")
                    {
                        sqlstr += " and name like '%" + Name + "%' ";
                    }
                    if (Mark != "")
                    {
                        sqlstr += " and short_name like '%" + Mark + "%'";
                    }
                    sqlstr += ") m order by isnull(m.name,m.mark),m.code ";
                    break;
            }
            return DBUtility.GetTable(sqlstr, "MES");
        }

        //mesMarkerAllocation
        public static DataTable GetMarkerAlloDetailHeader(string MoNo)
        {
            string SQL = "         select b.GO_NO,b.MARKER_WASTAGE, GC.SHORT_NAME, B.PART_TYPE ";
            SQL = SQL + "		from  CP_MO_HD B,SC_HD SH,GEN_CUSTOMER GC ";
            SQL = SQL + "		where b.go_no=SH.sc_no ";
            SQL = SQL + "		and SH.customer_cd=GC.customer_cd  ";
            SQL = SQL + "		and b.MO_NO='" + MoNo + "' ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetMarkerDetailTitle(string MoNo)
        {
            string SQL = "             select d.SIZE_CD,SUM(ORDER_QTY) ORDER_QTY ";
            SQL = SQL + "           	from  cp_mo_cs_breakdown d,CP_SIZE_SEQ E ";
            SQL = SQL + "           	where D.MO_NO=E.MO_NO AND D.size_cd=E.size_cd ";
            SQL = SQL + "           	AND d.MO_NO='" + MoNo + "' ";
            SQL = SQL + "            GROUP BY d.SIZE_CD,E.seq ";
            SQL = SQL + "            order by E.seq ASC ";
            return DBUtility.GetTable(SQL, "MES");
        }

        //<added by:ZouShCh at 2013-03-20>


        public static DataTable GetMarkerCombineSizeCd(string MoNo, string Marker_Set_Id)
        {
            string SQL = "  select A.marker_set_id,A.size_cd,A.combine_size_cd from cp_marker_set_size_combine A inner join  cp_marker_set B ";
            SQL += " on A.marker_set_id=B.marker_set_id where B.mo_no='" + MoNo + "' and A.marker_set_id='" + Marker_Set_Id + "'";
            
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetMarkerCombineSizeNum(string MoNo, string Marker_Set_Id)
        {
            string SQL="    select A.marker_set_id,count(size_cd) as sizecdnum,A.combine_size_cd";
                SQL+=" from cp_marker_set_size_combine A inner join  cp_marker_set B ";
                SQL+=" on A.marker_set_id=B.marker_set_id where B.mo_no='"+MoNo+"' and A.marker_set_id='"+Marker_Set_Id+"'";
                SQL += " group by A.marker_set_id,A.combine_size_cd";

            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetMarkerCombineSizeAndRelation(string MoNo)
        {

          string SQL="  declare @sqlText varchar(max) ";
            SQL+=" declare @str varchar(2000) ";
            SQL+=" select @str='(select A.marker_set_id,A.size_cd,A.combine_size_cd from cp_marker_set_size_combine A ";
            SQL+=" inner join  cp_marker_set B on A.marker_set_id=B.marker_set_id where B.mo_no='''+'"+MoNo+"'''+ ";
            SQL+=" 'and A.marker_set_id in ( select Distinct A.marker_set_id from cp_marker_set_size_combine A ";
            SQL+=" inner join  cp_marker_set B on A.marker_set_id=B.marker_set_id ";
            SQL+=" where B.mo_no='''+'"+MoNo+"'''+' group by A.marker_set_id,A.combine_size_cd having count(size_cd)>1)'";
            SQL+=" +')B'"; 
            SQL+=" select @sqlText='select MARKER_SET_ID,'  ";
            SQL+=" select @sqlText=@sqltext+'max(CASE size_cd WHEN '''+size_cd+''' THEN combine_size_cd ELSE null END) AS ''' +size_cd+  ''',' from ";
            SQL += @" ( 
                      SELECT    MAX(B.SEQ) AS SEQ,                    
                                B.size_cd
                      FROM      ( SELECT    A.marker_set_id ,
                                            A.size_cd ,
                                            A.combine_size_cd,
                                            C.seq
                                  FROM      cp_marker_set_size_combine A
                                            INNER JOIN cp_marker_set B ON A.marker_set_id = B.marker_set_id
                                            INNER JOIN CP_SIZE_SEQ AS C ON C.SIZE_CD=A.SIZE_CD AND C.MO_NO='"+MoNo+@"'
                                  WHERE     B.mo_no = '"+MoNo+@"'
                          
                                ) B  
                     GROUP BY B.SIZE_CD           
             
             
                    ) AS a
                    ORDER BY a.seq ";
            SQL+=" select @sqlText=left(@sqlText,Len(@sqlText)-1)+' from '+@str+' group by marker_set_id'   ";
            //print @sqlText
            SQL += "exec(@sqlText)";

      
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetMarkerCombineColorAndRelation(string MoNo)
        {

            string SQL = "  declare @sqlText varchar(max) ";
            SQL += " declare @str varchar(2000) ";
            SQL += " select @str='(select A.marker_set_id,A.Color_cd,A.combine_Color_cd from cp_marker_set_Color_combine A ";
            SQL += " inner join  cp_marker_set B on A.marker_set_id=B.marker_set_id where B.mo_no='''+'" + MoNo + "'''+ ";
            SQL += " 'and A.marker_set_id in ( select Distinct A.marker_set_id from cp_marker_set_Color_combine A ";
            SQL += " inner join  cp_marker_set B on A.marker_set_id=B.marker_set_id ";
            SQL += " where B.mo_no='''+'" + MoNo + "'''+' group by A.marker_set_id,A.combine_Color_cd having count(Color_cd)>1)'";
            SQL += " +')B'";
            SQL += " select @sqlText='select MARKER_SET_ID,'  ";
            SQL += " select @sqlText=@sqltext+'max(CASE Color_cd WHEN '''+Color_cd+''' THEN combine_Color_cd ELSE null END) AS ''' +Color_cd+  ''',' from ";
            SQL += " (select distinct Color_cd  from (select A.marker_set_id,A.Color_cd,A.combine_Color_cd from cp_marker_set_Color_combine A inner join ";
            SQL += " cp_marker_set B on A.marker_set_id=B.marker_set_id where B.mo_no='" + MoNo + "')B) as a ";
            SQL += " select @sqlText=left(@sqlText,Len(@sqlText)-1)+' from '+@str+' group by marker_set_id'   ";
            //print @sqlText
            SQL += "exec(@sqlText)";


            return DBUtility.GetTable(SQL, "MES");
        }



        
       
        //<added by:ZouShCh at 2013-03-20>

        public static DataTable GetMarkerCombineSizeDetailTitle(string MoNo,string CombineSize)
        {
            string SQL = " select  Size_cd,sum(assign_qty) as Assign_qty from CP_MO_COLOR_SIZE_COMBINE_INFO where MO_NO='"+MoNo+"' and Combine_Size='" + CombineSize + "'";
            SQL += " group by size_cd";
    
        

            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetMarkerAlloctionDetail(string MoNo)
        {
            string SQL = "        SELECT   C.MARKER_SET_ID MARKER_ID, C.COLOR_CD, C.PLYS, C.COLOR_DESC ,SUM (D.RATIO) RATIONS, ";
            SQL = SQL + "		         C.PLYS * SUM (D.RATIO) GMT_QTY, ";
            SQL = SQL + "                CONVERT(VARCHAR(10),C.BUYER_PO_DEL_DATE,120) AS BUYER_PO_DEL_DATE";
            SQL = SQL + "		    FROM (SELECT A.MO_NO, B.COLOR_CD, A.MARKER_SET_ID, B.PLYS,D.COLOR_DESC,B.REF_JO,JO_HD.BUYER_PO_DEL_DATE  ";
            SQL = SQL + "		            FROM JO_HD, CP_MARKER_SET A, CP_MARKER_SET_COLOR_PLYS B,CP_MO_HD C,V_FABRIC_INFO D LEFT JOIN GEN_FAB_PATTERN_TYPE E ";
            SQL = SQL + "                       ON D.FAB_PATTERN=E.PATTERN_TYPE_CD ";
            SQL = SQL + "                   WHERE B.REF_JO = JO_HD.JO_NO AND A.MO_NO=C.MO_NO AND C.GO_NO=D.GO_NO  ";
            SQL = SQL + "                                AND C.PART_TYPE=D.PART_TYPE_CD AND D.COLOR_CD=B.REF_COLOR ";
            SQL = SQL + "                   AND A.MARKER_SET_ID = B.MARKER_SET_ID  ";
            SQL = SQL + "                   AND A.STATUS<>' DA' AND B.STATUS<>' DA'  ";
            SQL = SQL + "                   AND A.MO_NO = '" + MoNo + "') C, ";
            SQL = SQL + "                 CP_MARKER_SET_SIZE_RATIO D ";
            SQL = SQL + "           WHERE C.MARKER_SET_ID = D.MARKER_SET_ID ";
            SQL = SQL + "        GROUP BY C.MARKER_SET_ID, C.COLOR_CD,C.COLOR_DESC, C.PLYS, C.REF_JO,C.BUYER_PO_DEL_DATE ";
            SQL = SQL + "        ORDER BY C.MARKER_SET_ID, C.COLOR_CD, C.PLYS ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetMarkerAlloctionDetailSizeCutPlan(string MoNo)
        {
            string SQL = " select B.marker_id,B.Color_cd,A.size_cd,B.RATION from cp_marker_set_size_combine A inner join";
            SQL += " ( SELECT A.MARKER_SET_ID MARKER_ID, B.COLOR_CD, C.SIZE_CD, C.RATIO 		RATION ";
            SQL += " FROM CP_MARKER_SET A, CP_MARKER_SET_COLOR_PLYS B, CP_MARKER_SET_SIZE_RATIO C";
            SQL += " WHERE A.MARKER_SET_ID = B.MARKER_SET_ID AND A.MARKER_SET_ID = C.MARKER_SET_ID AND";
            SQL += " A.MO_NO = '" + MoNo + "' ) B on A.marker_set_id=B.marker_id and A.Combine_size_cd=B.size_cd";
            return DBUtility.GetTable(SQL, "MES");
        }
        
        public static DataTable GetMarkerAlloctionDetailSize(string MoNo)
        {
           
            string SQL = "         SELECT A.MARKER_SET_ID MARKER_ID, B.COLOR_CD, C.SIZE_CD, C.RATIO ";
            SQL = SQL + "		RATION FROM CP_MARKER_SET A, CP_MARKER_SET_COLOR_PLYS B, ";
            SQL = SQL + "		CP_MARKER_SET_SIZE_RATIO C WHERE A.MARKER_SET_ID = ";
            SQL = SQL + "		B.MARKER_SET_ID AND A.MARKER_SET_ID = C.MARKER_SET_ID AND ";
            SQL = SQL + "		A.MO_NO = '" + MoNo + "' ";           
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetDetailSizeCount(string MoNo)
        {
           string SQL=" select combine_size,count(size_cd) as detailSizeCount from ";
            SQL+=" (select distinct size_cd,combine_size from CP_MO_COLOR_SIZE_COMBINE_INFO where mo_no='"+MoNo+"') A";
            SQL += " group by A.combine_size";
            return DBUtility.GetTable(SQL, "MES");
        }

        //mesCutPlanAndSpreadPlanReport
      
      public static DataTable GetMarkerRatioInfo(string MoNo,string MarkerID,string bedNo)
        { 
         
            string SQL = " select marker_id,Combine_size_cd,ratio_seq,sum(ply) as ply ";
                SQL+=" from FN_GET_MO_MARKER_SIZE_DETAIL('"+MoNo+"')";
                SQL+=" where cut_table_no='"+bedNo+"'and  marker_id='"+MarkerID+"'";
                SQL += " group by marker_id,combine_size_cd,ratio_seq";
               return DBUtility.GetTable(SQL, "MES");
        }

      public static DataTable GetMarkerRatioInfoNew(string MoNo, string MarkerID,string bedNo)
      {
          string SQL = @"
                      SELECT    COMBINE_SIZE_CD ,
                                
                                SUM(QTY) AS ply
                      FROM      cp_marker_set AS A
                                INNER JOIN dbo.CP_MARKER AS B ON a.MARKER_SET_ID = b.MARKER_SET_ID
                                INNER JOIN dbo.CP_CUT_TABLE_SIZE_QTY AS C ON b.MARKER_ID = c.MARKER_ID
                      WHERE     A.MO_NO = '"+MoNo+@"'
                                AND CUT_TABLE_NO = '"+bedNo+@"'
                                AND B.MARKER_ID = '"+MarkerID+@"'
                      GROUP BY COMBINE_SIZE_CD";
            
          return DBUtility.GetTable(SQL, "MES");
      }
       
        public static DataTable GetMarkerCombineSizeGatherInfo(string MoNo)
        {
            string SQL = "exec PROC_GET_MO_MARKER_SIZE_SUMMARY '" + MoNo + "'";
            return DBUtility.GetTable(SQL, "MES"); 
        }


        public static DataTable GetMarkerCombineSizeRatioDetailSize(string MoNo,string MarkerID,string bedNo,string CombineSize,string Ratio_Seq)
        {
           
                  string SQL="  select  combine_size_cd,Ratio_seq,size_cd,ply as qty from FN_GET_MO_MARKER_SIZE_DETAIL('"+MoNo+"') ";

                  SQL += "  where Cut_TABLE_NO='"+bedNo+"' and  Combine_size_cd='" + CombineSize + "' and marker_id='" + MarkerID + "' and ratio_seq='" + Ratio_Seq + "' order by size_cd ";
                return DBUtility.GetTable(SQL, "MES"); 
        }

        public static DataTable GetMarkerCombineSizeDetailSize(string MoNo,string MarkerID,string bedNo,string CombineSize)
        {
            string SQL = @"   SELECT    COMBINE_SIZE_CD ,
                            SIZE_CD ,
                            QTY
                  FROM      cp_marker_set AS A
                            INNER JOIN dbo.CP_MARKER AS B ON a.MARKER_SET_ID = b.MARKER_SET_ID
                            INNER JOIN dbo.CP_CUT_TABLE_SIZE_QTY AS C ON b.MARKER_ID = c.MARKER_ID
                  WHERE     A.MO_NO = '"+MoNo+@"'
                            AND CUT_TABLE_NO = '"+bedNo+@"'
                            AND B.MARKER_ID = '"+MarkerID+@"'
                            AND Combine_size_cd = '"+CombineSize+@"'";

                return DBUtility.GetTable(SQL, "MES"); 
        }
        public static DataTable GetMarkerRatioSizeCountMax(string MoNo,string MarkerID,string bedNo)
        {
           

                    string SQL=" select max(sizeCount) as SizeCountMax  from  (";
                    SQL+=" select count(Size_cd) as SizeCount from FN_GET_MO_MARKER_SIZE_DETAIL('"+MoNo+"') ";
                    SQL += " where cut_table_no='" + bedNo + "'and  marker_id='" + MarkerID + "' group by marker_id,Ratio_seq,combine_size_cd) A";
                return DBUtility.GetTable(SQL, "MES"); 
        }
       public static DataTable GetMarkerRatioSizeCountMaxNew(string MoNo,string MarkerID,string bedNo)
        {
            string SQL = @"
                        SELECT MAX(sizeCount) AS SizeCountMax
                        FROM   ( SELECT    COUNT(Size_CD) AS SizeCount
                        FROM      cp_marker_set AS A
                                INNER JOIN dbo.CP_MARKER AS B ON a.MARKER_SET_ID = b.MARKER_SET_ID
                                INNER JOIN dbo.CP_CUT_TABLE_SIZE_QTY AS C ON b.MARKER_ID = c.MARKER_ID
                        WHERE     A.MO_NO = '"+MoNo+@"'
                                AND CUT_TABLE_NO = '"+bedNo+@"'
                                AND B.MARKER_ID = '"+MarkerID+@"'
                        GROUP BY  B.MARKER_ID ,
                                COMBINE_SIZE_CD
                        ) A ";
            return DBUtility.GetTable(SQL, "MES"); 
        }
        public static DataTable GetMakerNO(string MoNo)
        {

            //string SQL = "select marker_id from FN_GET_MO_MARKER_SIZE_DETAIL('" + MoNo + "') group by marker_id";
            string SQL = " SELECT MARKER_ID,CUT_TABLE_NO FROM CP_CUT_TABLE ";
                   SQL+="WHERE MARKER_ID IN (select marker_id from FN_GET_MO_MARKER_SIZE_DETAIL('"+MoNo+"') GROUP BY MARKER_ID)";


            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetMakerCombineCountNew(string MoNo, string Marker_id,string bedNo)
        {
            string SQL = @"
                        SELECT    DISTINCT
                                COMBINE_SIZE_CD 
                    
                        FROM      cp_marker_set AS A
                                INNER JOIN dbo.CP_MARKER AS B ON a.MARKER_SET_ID = b.MARKER_SET_ID
                                INNER JOIN dbo.CP_CUT_TABLE_SIZE_QTY AS C ON b.MARKER_ID = c.MARKER_ID
                        WHERE     A.MO_NO = '"+MoNo+@"'
                                AND CUT_TABLE_NO = '"+bedNo+@"'
                                AND B.MARKER_ID = '"+Marker_id+@"' ";  
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetMakerCombineCount(string MoNo, string Marker_id,string bedNo)
        {
            string SQL;
            if (Marker_id.ToString() != "")
            {
                //SQL = "    select count(*)*2 as CountNum,combine_size_cd,priority";
                SQL = "    select combine_size_cd,ratio_seq";

                SQL += " from (select marker_id,Combine_size_cd,ratio_seq,sum(ply) as ply ";
                SQL += " from FN_GET_MO_MARKER_SIZE_DETAIL('" + MoNo + "') ";

                SQL += " where cut_table_no='" + bedNo + "'and  marker_id='" + Marker_id + "'";
                SQL += " group by marker_id,combine_size_cd,ratio_seq";
                SQL += " ) A left join ";
                SQL += " (select * from cp_mo_size_priority where mo_no='" + MoNo + "') B";
                SQL += " on A.Combine_size_cd=b.size_cd";
                //SQL += " group by Combine_size_cd,priority";
                SQL += " ORDER BY ratio_seq";
            }
            else
            {
           
                SQL = "   select combine_size_cd as combine_size from cp_marker_set_size_combine A inner join cp_marker_set B on  A.marker_set_id=B.marker_set_id";
                SQL+=" where b.MO_NO='"+MoNo+"' ";
                SQL+=" group by combine_size_cd having count(combine_size_cd)=1";

            }

            return DBUtility.GetTable(SQL, "MES");
        }


   


        //mesSearchMarketNo
        public static DataTable GetCutPlanSearchMarkerNo(string MoNo, string JoNo)
        {
            string SQL = "             SELECT DISTINCT A.MO_NO,A.GO_NO FROM CP_MO_HD A INNER JOIN CP_MO_JO B  ";
            SQL = SQL + "       			ON A.MO_NO=B.MO_NO ";
            SQL = SQL + "       		WHERE A.MO_NO LIKE '%" + MoNo + "%' AND B.JO_NO LIKE '%" + JoNo + "%' ";
            SQL = SQL + "       		ORDER BY A.MO_NO ";
            return DBUtility.GetTable(SQL, "MES");
        }

        //mesMarkerCSBreakdown
        public static DataTable GetMarkerCSBreakdownTitle(string MoNo)
        {
            string SQL = "             SELECT E.MARKER_ID,SUM(H.RATIO) TOTAL_RATION,J.MARKER_LEN_INCH,J.MARKER_LEN_YDS      ";
            SQL = SQL + "            FROM (     ";
            SQL = SQL + "                select  R.MARKER_SET_ID,R.MO_NO,E.MARKER_ID ";
            SQL = SQL + "                FROM CP_MARKER_SET R LEFT JOIN  CP_MARKER E ";
            SQL = SQL + "                 ON  R.MARKER_SET_ID=E.MARKER_SET_ID ";
            SQL = SQL + "                WHERE R.MO_NO='" + MoNo + "' AND R.STATUS<>'DA' ";
            SQL = SQL + "                )  E LEFT JOIN CP_MARKER_SIZE_RATIO H ON  E.MARKER_ID=H.MARKER_ID ";
            SQL = SQL + "                     LEFT JOIN CP_MARKER_LEN_UTIL J    ON E.MARKER_ID=J.MARKER_ID ";
            SQL = SQL + "            WHERE  E.mo_no='" + MoNo + "' ";
            SQL = SQL + "            GROUP BY E.MARKER_ID,J.MARKER_LEN_INCH,J.MARKER_LEN_YDS     ";
            SQL = SQL + "            ORDER BY E.MARKER_ID ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetMarkerCSBreakdownHeader(string MoNo)
        {
            string SQL = "             select '" + MoNo + "' MO_NO,     ";
            SQL = SQL + "                dbo.fn_strConcat('" + MoNo + "', 2) JO_NO, ";
            SQL = SQL + "                MAX(X.RATIO) SIZE_ROWSPAN     ";
            SQL = SQL + "            FROM (SELECT E.MARKER_ID,SUM(H.RATIO) RATIO     ";
            SQL = SQL + "                    from  (     ";
            SQL = SQL + "                        select  R.MARKER_SET_ID,R.MO_NO,E.MARKER_ID ";
            SQL = SQL + "                        FROM CP_MARKER_SET R LEFT JOIN  CP_MARKER E ";
            SQL = SQL + "                            ON  R.MARKER_SET_ID=E.MARKER_SET_ID ";
            SQL = SQL + "                        WHERE R.MO_NO='" + MoNo + "' AND R.STATUS<>'DA' ";
            SQL = SQL + "                            ) E LEFT JOIN CP_MARKER_SIZE_RATIO H ";
            SQL = SQL + "                        ON   E.MARKER_ID=H.MARKER_ID     ";
            SQL = SQL + "            WHERE E.MO_NO='" + MoNo + "' ";
            SQL = SQL + "            GROUP BY  E.MARKER_ID ";
            SQL = SQL + "			) X ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetMarkerCSBreakdownSizeDetail(string MoNo)
        {
            string SQL = "         SELECT H.SIZE_CD,E.MARKER_ID,H.RATIO RATION		 	 ";
            SQL = SQL + "		FROM (			 ";
            SQL = SQL + "			select  R.MARKER_SET_ID,R.MO_NO,E.MARKER_ID		 ";
            SQL = SQL + "			FROM CP_MARKER_SET R LEFT JOIN  CP_MARKER E		 ";
            SQL = SQL + "			ON  R.MARKER_SET_ID=E.MARKER_SET_ID		 ";
            SQL = SQL + "			WHERE R.MO_NO='" + MoNo + "' AND R.STATUS<>'DA'		 ";
            SQL = SQL + "			) E LEFT JOIN CP_MARKER_LEN_UTIL J ON E.MARKER_ID=J.MARKER_ID,CP_MARKER_SIZE_RATIO H		  ";
            SQL = SQL + "			WHERE E.MARKER_ID=H.MARKER_ID ";
            SQL = SQL + "		AND E.mo_no='" + MoNo + "' ";
            SQL = SQL + "		ORDER BY H.SIZE_CD desc,E.MARKER_ID ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetMarkerCSBreakdownColorDetail(string MoNo)
        {
            string SQL = "         SELECT L.COLOR_CD,E.MARKER_ID,L.PLYS      ";
            SQL = SQL + "        FROM (     ";
            SQL = SQL + "            select  R.MARKER_SET_ID,R.MO_NO,E.MARKER_ID ";
            SQL = SQL + "            FROM CP_MARKER_SET R LEFT JOIN  CP_MARKER E ";
            SQL = SQL + "            ON  R.MARKER_SET_ID=E.MARKER_SET_ID ";
            SQL = SQL + "            WHERE R.MO_NO='" + MoNo + "' AND R.STATUS<>'DA' ";
            SQL = SQL + "            )E, ";
            SQL = SQL + "            CP_MARKER_COLOR_PLYS L ";
            SQL = SQL + "        WHERE E.MARKER_ID=L.MARKER_ID     ";
            SQL = SQL + "        AND E.mo_no='" + MoNo + "' ";
            SQL = SQL + "        ORDER BY L.COLOR_CD,E.MARKER_ID ";
            return DBUtility.GetTable(SQL, "MES");
        }

        //mesMarkerDetail
        /* public static DataTable GetMarkerDetailTitle(string MoNo)*/
        public static DataTable GetMarkerDetailHeader(string MoNo)
        {
            string SQL = "         SELECT B.GO_NO,B.MARKER_WASTAGE,B.REMARKS, GC.SHORT_NAME, B.PART_TYPE, ";
            SQL = SQL + "			X.TOTAL_LENGTH * 12 AS NET_TOTAL, ";
            SQL = SQL + "			X.TOTAL_LENGTH*(100+B.MARKER_WASTAGE)/100*12 AS TOTAL, ";
            SQL = SQL + "			X.TOTAL_LENGTH/X.GMT_QTY*12 AS NET_YPD , ";
            SQL = SQL + "			X.TOTAL_LENGTH*(100+B.MARKER_WASTAGE)/100/X.GMT_QTY*12 AS YPD, ";
            SQL = SQL + "				X.GMT_QTY,X.ACT_QTY ,cast(ROUND(X.ACT_QTY/X.GMT_QTY*100,2) AS decimal(10,2)) AS  AVG_MU ";
            SQL = SQL + "        FROM  CP_MO_HD B,SC_HD SH,GEN_CUSTOMER GC, ";
            SQL = SQL + "        ( ";
            SQL = SQL + "            SELECT SUM(TOTAL_LENGTH) AS TOTAL_LENGTH,SUM(GMT_QTY) AS GMT_QTY,SUM(ACT_QTY) AS ACT_QTY  ";
            SQL = SQL + "            FROM FN_GET_MO_COLOR_YPD('" + MoNo + "') ";
            SQL = SQL + "        )X ";
            SQL = SQL + "        WHERE B.GO_NO=SH.SC_NO ";
            SQL = SQL + "           AND SH.CUSTOMER_CD=GC.CUSTOMER_CD             ";
            SQL = SQL + "           AND B.MO_NO='" + MoNo + "' ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetMarkerDetailDetail(string moNo)
        {
            string SQL = "";
            SQL += " select cc.*,case cc.NET_YPD when  'N/A' then cc.NET_YPD else";
            SQL += " convert(varchar(100),cc.NET_YPD*(1+isnull(cc.CUT_SEW_WASTAGE,0)/100)) end";
            SQL += " as GROSS_MARKER_YPD";
            SQL += " FROM (select aa.*,(select isnull(convert(varchar(100),sum(bb.PLYS*bb.MARKER_LENGTH)/nullif(sum(bb.gmt_qty),0)*12),'N/A')";
            SQL += " FROM (SELECT x.MARKER_ID,x.COLOR_CD,x.COLOR_DESC,x.FAB_DESC,x.PATTERN_TYPE_CD,x.PATTERN_TYPE_DESC,";
            SQL += " SHRINKAGE,x.BATCH_NO,x.WIDTH,                                   x.PLYS,x.YPD,x.GMT_QTY,x.RATIO_TOTAL RATION_TOTAL,";
            SQL += " cast(x.FAB_REQ as decimal(10,2)) FAB_REQ,                                      ";
            SQL += " cast(ROUND(SUM(y.MARKER_LEN_YDS+y.MARKER_LEN_INCH/36),2) as decimal(10,2)) MARKER_LENGTH,                        ";
            SQL += " MARKER_LEN_YDS=sum(y.MARKER_LEN_YDS),MARKER_LEN_INCH=sum(y.MARKER_LEN_INCH),MIN(Z.WASTAGE) ";
            SQL += " AS CUT_SEW_WASTAGE                 ";
            SQL += " FROM (select A.MARKER_ID,A.MARKER_NAME,A.REF_COLOR,A.COLOR_CD,A.COLOR_DESC,A.FAB_DESC,";
            SQL += " A.PATTERN_TYPE_CD,A.PATTERN_TYPE_DESC,A.SHRINKAGE,A.BATCH_NO,A.WIDTH,                                      ";
            SQL += " A.PLYS,M.YPD,A.PLYS*sum(h.RATIO) GMT_QTY,SUM(H.RATIO) RATIO_TOTAL,                                     ";
            SQL += " ROUND(M.YPD*A.PLYS*sum(h.RATIO)/12,2) FAB_REQ                                     ";
            SQL += " FROM (SELECT E.MARKER_ID,E.MARKER_NAME,L.REF_COLOR, L.COLOR_CD,C.COLOR_DESC,C.PPO_NO AS FAB_DESC,";
            SQL += " A.PATTERN_TYPE_CD,A.PATTERN_TYPE_DESC,C.SHRINKAGE,C.BATCH_NO,C.WIDTH,SUM(ISNULL(H.PLYS,L.PLYS)) PLYS                                              ";
            SQL += " FROM GEN_FAB_PATTERN_TYPE A ";
            SQL += " right join CP_FABRIC_ITEM C ON C.FAB_PATTERN=A.PATTERN_TYPE_CD                            ";
            SQL += " INNER JOIN CP_MO_HD B ON B.GO_NO=C.GO_NO AND B.PART_TYPE=C.PART_TYPE_CD                                                   ";
            SQL += " right join CP_MARKER E  ON  E.SHRINKAGE=C.SHRINKAGE AND E.WIDTH=C.WIDTH, CP_MARKER_SET R       ,";
            SQL += " CP_MARKER_COLOR_PLYS L  ";
            SQL += " LEFT join CP_MARKER_PLYS H ON L.MARKER_ID=H.MARKER_ID AND L.COLOR_CD=H.COLOR_CD                                ";
            SQL += " WHERE  B.MO_NO=R.MO_NO                                       AND R.MARKER_SET_ID=E.MARKER_SET_ID ";
            SQL += " AND E.MARKER_ID=L.MARKER_ID AND R.STATUS<>'DA' AND (L.STATUS<>'DA' OR L.STATUS IS NULL)                                     ";
            SQL += " AND L.REF_COLOR=C.COLOR_CD                                            AND B.MO_NO='" + moNo + "'                            ";
            SQL += " GROUP BY E.MARKER_ID,E.MARKER_NAME,L.COLOR_CD,L.REF_COLOR,C.COLOR_DESC,A.PATTERN_TYPE_CD,";
            SQL += " A.PATTERN_TYPE_DESC,C.SHRINKAGE,C.BATCH_NO,C.WIDTH,C.PPO_NO                    ) A                              ";
            SQL += " LEFT join CP_MARKER_SIZE_RATIO H ON A.MARKER_ID=H.MARKER_ID                                    ";
            SQL += " LEFT join CP_MARKER_YPD M   ON  A.MARKER_ID=M.MARKER_ID                             ";
            SQL += " GROUP BY A.MARKER_ID,A.MARKER_NAME,A.COLOR_CD,A.REF_COLOR,A.COLOR_DESC,A.PATTERN_TYPE_CD,";
            SQL += " A.PATTERN_TYPE_DESC,A.SHRINKAGE,A.BATCH_NO,A.WIDTH,A.PLYS,M.YPD,A.FAB_DESC                              ) x ";
            SQL += " LEFT join CP_MARKER_LEN_UTIL y ON  x.MARKER_ID=y.MARKER_ID                 ";
            SQL += " LEFT join FN_CUT_SEW_WASTAGE('" + moNo + "') Z ON X.REF_COLOR=Z.COLORCD                          ";
            SQL += " GROUP BY x.MARKER_ID,x.COLOR_CD,x.PATTERN_TYPE_CD,x.PATTERN_TYPE_DESC,x.COLOR_DESC,";
            SQL += " x.SHRINKAGE,x.BATCH_NO,x.WIDTH ,x.FAB_DESC,                         x.PLYS,x.YPD,x.GMT_QTY,x.RATIO_TOTAL,x.FAB_REQ) bb ";
            SQL += " WHERE bb.COLOR_CD=aa.COLOR_CD) as NET_YPD";
            SQL += " FROM (SELECT x.MARKER_ID,x.MARKER_NAME,x.COLOR_CD,x.COLOR_DESC,x.FAB_DESC,x.PATTERN_TYPE_CD,x.PATTERN_TYPE_DESC,SHRINKAGE,x.BATCH_NO,x.WIDTH,                                   x.PLYS,x.YPD,x.GMT_QTY,x.RATIO_TOTAL RATION_TOTAL,cast(x.FAB_REQ as decimal(10,2)) FAB_REQ,                                      cast(ROUND(SUM(y.MARKER_LEN_YDS+y.MARKER_LEN_INCH/36),2) as decimal(10,2)) MARKER_LENGTH,                        MARKER_LEN_YDS=sum(y.MARKER_LEN_YDS),MARKER_LEN_INCH=sum(y.MARKER_LEN_INCH),MIN(Z.WASTAGE) AS CUT_SEW_WASTAGE                 ";
            SQL += " FROM (select A.MARKER_ID,A.MARKER_NAME,A.REF_COLOR,A.COLOR_CD,A.COLOR_DESC,A.FAB_DESC,A.PATTERN_TYPE_CD,A.PATTERN_TYPE_DESC,A.SHRINKAGE,A.BATCH_NO,A.WIDTH,                                      A.PLYS,M.YPD,A.PLYS*sum(h.RATIO) GMT_QTY,SUM(H.RATIO) RATIO_TOTAL,                                     ROUND(M.YPD*A.PLYS*sum(h.RATIO)/12,2) FAB_REQ                                     ";
            SQL += " FROM (SELECT E.MARKER_ID,E.MARKER_NAME,L.REF_COLOR, L.COLOR_CD,C.COLOR_DESC,C.PPO_NO AS FAB_DESC,A.PATTERN_TYPE_CD,A.PATTERN_TYPE_DESC,C.SHRINKAGE,C.BATCH_NO,C.WIDTH,SUM(L.PLYS) PLYS                                              ";
            SQL += " FROM GEN_FAB_PATTERN_TYPE A ";
            SQL += " right join CP_FABRIC_ITEM C ON C.FAB_PATTERN=A.PATTERN_TYPE_CD                            ";
            SQL += " INNER JOIN CP_MO_HD B ON B.GO_NO=C.GO_NO AND B.PART_TYPE=C.PART_TYPE_CD                                                   ";
            SQL += " right join CP_MARKER E  ON  E.SHRINKAGE=C.SHRINKAGE AND E.WIDTH=C.WIDTH, CP_MARKER_SET R       ,CP_MARKER_COLOR_PLYS L                                     ";
            SQL += " WHERE  B.MO_NO=R.MO_NO                                       AND R.MARKER_SET_ID=E.MARKER_SET_ID AND E.MARKER_ID=L.MARKER_ID AND R.STATUS<>'DA' AND (L.STATUS<>'DA' OR L.STATUS IS NULL)                                     AND L.REF_COLOR=C.COLOR_CD                                            AND B.MO_NO='" + moNo + "'                            ";
            SQL += " GROUP BY E.MARKER_ID,E.MARKER_NAME,L.COLOR_CD,L.REF_COLOR,C.COLOR_DESC,A.PATTERN_TYPE_CD,A.PATTERN_TYPE_DESC,C.SHRINKAGE,C.BATCH_NO,C.WIDTH,C.PPO_NO                    ) A                              ";
            SQL += " LEFT join CP_MARKER_SIZE_RATIO H ON A.MARKER_ID=H.MARKER_ID                                    ";
            SQL += " LEFT join CP_MARKER_YPD M   ON  A.MARKER_ID=M.MARKER_ID                             ";
            SQL += " GROUP BY A.MARKER_ID,A.MARKER_NAME,A.COLOR_CD,A.REF_COLOR,A.COLOR_DESC,A.PATTERN_TYPE_CD,A.PATTERN_TYPE_DESC,A.SHRINKAGE,A.BATCH_NO,A.WIDTH,A.PLYS,M.YPD,A.FAB_DESC                              ) x ";
            SQL += " LEFT join CP_MARKER_LEN_UTIL y ON  x.MARKER_ID=y.MARKER_ID                 ";
            SQL += " LEFT join FN_CUT_SEW_WASTAGE('" + moNo + "') Z ON X.REF_COLOR=Z.COLORCD                          ";
            SQL += " GROUP BY x.MARKER_ID,x.MARKER_NAME,x.COLOR_CD,x.PATTERN_TYPE_CD,x.PATTERN_TYPE_DESC,";
            SQL += " x.COLOR_DESC,x.SHRINKAGE,x.BATCH_NO,x.WIDTH ,x.FAB_DESC,                         ";
            SQL += " x.PLYS,x.YPD,x.GMT_QTY,x.RATIO_TOTAL,x.FAB_REQ) aa ) cc                 ";
            SQL += " ORDER BY cc.COLOR_CD,cc.MARKER_ID";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetMarkerDetailDetailSize(string MoNo)
        {
            string SQL = "         select H.MARKER_ID,H.SIZE_CD,RATIO RATION,L.COLOR_CD		 ";
            SQL = SQL + "		FROM  (		 ";
            SQL = SQL + "			select  R.MARKER_SET_ID,R.MO_NO,E.MARKER_ID	 ";
            SQL = SQL + "			FROM CP_MARKER_SET R LEFT JOIN  CP_MARKER E	 ";
            SQL = SQL + "			ON R.MARKER_SET_ID=E.MARKER_SET_ID	 ";
            SQL = SQL + "			WHERE R.MO_NO='" + MoNo + "' AND R.STATUS<>'DA'	 ";
            SQL = SQL + "			) E LEFT JOIN CP_MARKER_SIZE_RATIO H ON E.MARKER_ID=H.MARKER_ID 	 ";
            SQL = SQL + "				LEFT JOIN CP_MARKER_COLOR_PLYS L ON  E.MARKER_ID=L.MARKER_ID ";
            SQL = SQL + "			ORDER BY H.MARKER_ID,L.COLOR_CD ";
            return DBUtility.GetTable(SQL, "MES");
        }

        //outsourceContractReport2
        public static DataTable GetOutsourceContract(string ContractNo)
        {
            string SQL = "         SELECT SH.STYLE_CHN_DESC, DT.GOOD_NAME, PH.SC_NO, ";
            SQL = SQL + "		DT.JOB_ORDER_NO, CONVERT(VARCHAR(10),DT.EXPECT_RECEIVE_DATE,120) ";
            SQL = SQL + "		as EXPECT_RECEIVE_DATE, DT.PROCESS_REMARK PROCESS_CD, ";
            SQL = SQL + "		ISNULL(DT.SUB_CONTRACT_PRICE,0)+ISNULL(DT.WASH_PRICE,0)+ISNULL(DT.PRINT_PRICE,0)+ISNULL(DT.EMB_PRICE,0) ";
            SQL = SQL + "		SUB_CONTRACT_PRICE, ISNULL(DT.PLAN_ISSUE_QTY,0) PLAN_ISSUE_QTY ";
            SQL = SQL + "		FROM prd_outsource_contract_dt dt, prd_outsource_contract hd, ";
            SQL = SQL + "		jo_hd ph, sc_hd sh WHERE hd.contract_no ='" + ContractNo + "' AND ";
            SQL = SQL + "		hd.STATUS!='CAN' AND sh.sc_no=ph.sc_no ANd ph.JO_NO = ";
            SQL = SQL + "		dt.job_order_no AND hd.contract_no = dt.contract_no ORDER BY ";
            SQL = SQL + "		EXPECT_RECEIVE_DATE ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetOutsourceCurrency(string contractNo)
        {
            string SQL = "         SELECT DISTINCT SUB.SUBCONTRACTOR_NAME CHN_NAME, SUB.CURRENCY,ISNULL(SUB.FAX,' ') FAX, ISNULL(SUB.TEL_NO,' ') TEL_NO,ISNULL(SUB.ADDRESS,' ') ADDRESS ,  ";
            SQL = SQL + "        ISNULL(SUB.MAIN_CONTRACT,' ') MAIN_CONTRACT, USER_NAME=(case when isnull((select system_value from GEN_SYSTEM_SETTING where factory_cd=hd.factory_cd and system_key='OSRPT2_RECIVER'),'N')='Y' then ' ' else ISNULL(R.USER_NAME,' ') end), ";
            SQL = SQL + "        CONVERT(NUMERIC(10,0),ISNULL(SUB.VALUE_ADD_TAX,0)*100) VALUE_ADD_TAX,local_fty=hd.factory_cd, ";
            SQL = SQL + "        CompanyName=(select system_value from GEN_SYSTEM_SETTING where factory_cd=hd.factory_cd and system_key='OSRPT2_FTY_NAME'), ";
            SQL = SQL + "        CompanyAddress=(select system_value from GEN_SYSTEM_SETTING where factory_cd=hd.factory_cd and system_key='OSRPT2_FTY_ADDR'), ";
            SQL = SQL + "        CompanyTel=(select system_value from GEN_SYSTEM_SETTING where factory_cd=hd.factory_cd and system_key='OSRPT2_FTY_TEL'), ";
            SQL = SQL + "        CompanyFax=(select system_value from GEN_SYSTEM_SETTING where factory_cd=hd.factory_cd and system_key='OSRPT2_FTY_FAX'), ";
            SQL = SQL + "        subString2=(select system_value from GEN_SYSTEM_SETTING where factory_cd=hd.factory_cd and system_key='OSRPT2_RMK_2'), ";
            SQL = SQL + "        subString6=(select system_value from GEN_SYSTEM_SETTING where factory_cd=hd.factory_cd and system_key='OSRPT2_RMK_6') ";
            //Added By ZouShiChang ON 2014.02.14 Start
            /*
            SQL = SQL + "        FROM prd_outsource_contract hd,prd_subcontractor_master sub,prd_SENDER_RECIVER_MASTER r  ";
            SQL = SQL + "        WHERE hd.contract_no ='" + contractNo + "' and r.code=hd.receiver  AND hd.STATUS!='CAN'  ";
            SQL = SQL + "        AND sub.subcontractor_cd=hd.subcontractor ";
            */
            SQL = SQL + @" FROM      prd_outsource_contract hd
                    INNER JOIN prd_subcontractor_master sub ON sub.subcontractor_cd = hd.subcontractor
                    LEFT JOIN prd_SENDER_RECIVER_MASTER r ON r.code = hd.receiver
            WHERE     hd.contract_no = '" + contractNo + @"'
		            AND  hd.STATUS!='CAN' ";
            //Added By ZouShiChang ON 2014.02.14 End

            return DBUtility.GetTable(SQL, "MES");
        }

        //outsourceFGISDif
        public static DataTable GetOutsourceFGISDif1(string startDate, string endDate, string JoNo)
        {
            string SQL = "         SELECT B.REFERENCE_NO AS JOB_ORDER_NO, ROUND(SUM (DECODE ";
            SQL = SQL + "        (A.TRANS_CD, 'ITO', -C.QTY, C.QTY)), 0) AS QTY FROM INV_TRANS_HD ";
            SQL = SQL + "        A, INV_TRANS_LINES B, INV_TRANS_LINE_ITEMS C, INV_TRANS_CODE D ";
            SQL = SQL + "        WHERE A.TRANS_HEADER_ID = B.TRANS_HEADER_ID AND B.TRANS_LINE_ID ";
            SQL = SQL + "        = C.TRANS_LINE_ID AND A.TRANS_CD = D.TRANS_CD AND A.STATUS = 'F' ";
            SQL = SQL + "        AND A.FROM_STORE_CD = 'GEG-O01' AND D.TRANS_CD IN ('KBR-O', ";
            SQL = SQL + "        'WBR-O', 'ITO') ";
            if (startDate != "")
            {
                SQL += " AND A.TRANS_DATE >= TO_DATE ('" + startDate + "', 'yyyy/mm/dd')";
            }
            if (endDate != "")
            {
                SQL += " AND A.TRANS_DATE <= TO_DATE ('" + endDate + "', 'yyyy/mm/dd')";
            }
            if (JoNo != "")
            {
                SQL += " AND B.REFERENCE_NO = '" + JoNo + "'";
            }
            SQL += " GROUP BY B.REFERENCE_NO ORDER BY JOB_ORDER_NO";
            return DBUtility.GetTable(SQL, "WIP");
        }

        public static DataTable GetOutsourceFGISDif2(string startDate, string endDate, string JoNo, string factoryCd)
        {
            string SQL = "         SELECT JOX.JOB_ORDER_NO, SUM(JOX.OUTPUT_QTY) QTY FROM ";
           /*
            SQL = SQL + "  PRD_OUTSOURCE_CONTRACT_DT ODT,PRD_OUTSOURCE_CONTRACT OHD, ";
            SQL = SQL + "  PRD_JO_OUTPUT_TRX JOX ,PRD_JO_OUTPUT_HD JHD,PRD_OUTSOURCE_WAREHOUSE_TYPE OWT  WHERE ";
            */
            SQL = SQL + @" PRD_OUTSOURCE_CONTRACT_DT ODT INNER JOIN  
                PRD_OUTSOURCE_CONTRACT OHD ON OHD.CONTRACT_NO = ODT.CONTRACT_NO
                INNER JOIN PRD_JO_OUTPUT_TRX JOX  ON odt.JOB_ORDER_NO=jox.JOB_ORDER_NO
                INNER JOIN PRD_JO_OUTPUT_HD JHD  ON JOX.DOC_NO=jhd.DOC_NO
                INNER JOIN PRD_OUTSOURCE_WAREHOUSE_TYPE OWT ON JOX.DOC_NO=OWT.DOC_NO AND JOX.TRX_ID=owt.TRX_ID WHERE";

            SQL = SQL + "  OHD.STATUS!='CAN' AND OHD.CONTRACT_NO=ODT.CONTRACT_NO AND ";
            SQL = SQL + "  JOX.SEND_ID=ODT.SEND_ID AND JOX.JOB_ORDER_NO=ODT.JOB_ORDER_NO ";
            //Added By ZouShiChang ON 2013.08.20 Start MES 024
            //SQL = SQL + "  AND JOX .PROCESS_CD=OHD.RECEIVE_POINT AND ";
            SQL = SQL + "  AND jox.TRX_ID=OWT.TRX_ID AND jox.DOC_NO=OWT.DOC_NO AND JOX .PROCESS_CD=OHD.RECEIVE_POINT AND JOX.PROCESS_TYPE=OHD.RECEIVER_PROCESS_TYPE AND JOX.PROCESS_GARMENT_TYPE=OHD.GARMENT_TYPE  ";
            //Added By ZouShiChang ON 2013.08.20 End MES 024
            SQL = SQL + " AND (JHD.NEXT_PROCESS_CD='WHS' AND OWT.WAREHOUSE_TYPE='A') ";
            //SQL = SQL + "  JHD.NEXT_PROCESS_CD IN('TOSTOCKA','TOSTOCKA') AND JHD.DOC_NO=JOX.DOC_NO ";
            if (startDate != "")
            {
                SQL += " AND JHD.TRX_DATE >= '" + startDate + "'";
            }
            if (endDate != "")
            {
                SQL += " AND JHD.TRX_DATE <= '" + endDate + "'";
            }
            if (factoryCd != "")
            {
                SQL += " AND JHD.FACTORY_CD='" + factoryCd + "'";
            }
            if (JoNo != "")
            {
                SQL += " AND JOX.JOB_ORDER_NO='" + JoNo + "'";
            }
            SQL += " AND OHD.FACTORY_CD=JHD.FACTORY_CD AND JOX.FACTORY_CD=JHD.FACTORY_CD GROUP BY JOX.JOB_ORDER_NO";
            return DBUtility.GetTable(SQL, "MES");
        }




        //outsourceReport.
        public static DataTable GetOutsourceReportingHead(string contractNo)
        {
            string SQL = "         SELECT OHD.CONTRACT_NO, OHD.SUBCONTRACTOR, FTY.CHN_NAME, ";
            SQL = SQL + "		OHD.ISSUER_DATE,MST.ADDRESS , MST.SUBCONTRACTOR_NAME, ";
            SQL = SQL + "		OHD.VALUE_ADD_TAX, U.NAME FROM PCM_OUTSOURCE_CONTRACT OHD, ";
            SQL = SQL + "		SUBCONTRACTOR_MASTER MST , GEN_FACTORY FTY, GEN_USERS U WHERE ";
            SQL = SQL + "		OHD.SUBCONTRACTOR =MST.SUBCONTRACTOR_CD AND ";
            SQL = SQL + "		OHD.CREATE_USER_ID=U.USER_ID AND FTY.FACTORY_ID=OHD.FACTORY_CD ";
            SQL = SQL + "		AND OHD.CONTRACT_NO='" + contractNo + "' AND ohd.STATUS!='CAN' ";
            return DBUtility.GetTable(SQL, "WIP");
        }
        public static DataTable GetOutputQty(string contractNo)
        {
            string SQL = "         SELECT good_name, dt.job_order_no, dt.PLAN_ISSUE_QTY AS qty FROM ";
            SQL = SQL + "		pcm_outsource_contract_dt dt, pcm_outsource_contract hd WHERE ";
            SQL = SQL + "		hd.contract_no ='" + contractNo + "' AND hd.STATUS!='CAN' AND hd.contract_no ";
            SQL = SQL + "		= dt.contract_no ";
            return DBUtility.GetTable(SQL, "WIP");
        }

        //mesSummary 
        public static DataTable GetGoHeaderInfo(string MoNo)
        {
            string SQL = "         SELECT A.MO_NO, B.PART_TYPE, MAX (B.GO_NO) AS GO_NO, SUM ";
            SQL = SQL + "		(A.ORDER_GMT_QTY) AS ORDER_QTY, SUM (A.ACTUAL_GMT_QTY) AS ";
            SQL = SQL + "		ACTUAL_QTY, SUM (A.ACTUAL_GMT_QTY) - SUM (A.ORDER_GMT_QTY) AS ";
            SQL = SQL + "		ADJUST_QTY, CASE SUM(A.ORDER_GMT_QTY) WHEN 0 THEN 0 ELSE ";
            SQL = SQL + "		cast(ROUND((SUM (A.ACTUAL_GMT_QTY) - SUM (A.ORDER_GMT_QTY)) * ";
            SQL = SQL + "		100.0 / SUM (A.ORDER_GMT_QTY),2) as decimal(10,2)) END AS ";
            SQL = SQL + "		[PERCENT] FROM CP_MO_OVER_SHORT_SUMMARY A, CP_MO_HD B WHERE ";
            SQL = SQL + "		A.MO_NO = '" + MoNo + "' AND A.MO_NO = B.MO_NO GROUP BY A.MO_NO, ";
            SQL = SQL + "        B.PART_TYPE ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetGoFabricInfo(string MoNo)
        {
            string SQL = "         SELECT A.*,CAST(ROUND(B.YPD*C.ACTUAL_GMT_QTY/12.0,2) AS DECIMAL(10,2)) AS BULK_FABRIC_QTY, ";
            SQL = SQL + "        CAST(ROUND(A.RECEIVED_QTY-A.SPARE_FABRIC_QTY-ISNULL(A.BINDING_FABRIC_QTY,0)-B.YPD*C.ACTUAL_GMT_QTY/12.0,2) AS DECIMAL(10,2)) AS BALANCE_FABRIC_QTY FROM  ";
            SQL = SQL + "        ( ";
            SQL = SQL + "        SELECT COLOR_CD, COLOR_CD+CASE WHEN LTRIM(ISNULL(COLOR_DESC,'')) ='' THEN '' ELSE '-'+COLOR_DESC END AS Color_Name ";
            SQL = SQL + "        , PPO_NO,RECEIVED_QTY,SPARE_FABRIC_QTY,BINDING_FABRIC_QTY ";
            SQL = SQL + "        FROM CP_FABRIC_ITEM A INNER JOIN CP_MO_HD B ";
            SQL = SQL + "        ON A.GO_NO=B.GO_NO AND A.PART_TYPE_CD=B.PART_TYPE ";
            SQL = SQL + "        WHERE B.MO_NO='" + MoNo + "' ";
            SQL = SQL + "        ) A INNER JOIN  ";
            SQL = SQL + "        ( ";
            SQL = SQL + "        SELECT REF_COLOR AS COLOR_CD,ROUND(SUM(TOTAL_LENGTH)/SUM(GMT_QTY*1.0)*12*(1+MAX(MARKER_WASTAGE)*0.01),2) AS YPD  ";
            SQL = SQL + "        FROM dbo.FN_GET_MO_COLOR_YPD('" + MoNo + "') ";
            SQL = SQL + "        GROUP BY REF_COLOR ";
            SQL = SQL + "        ) B  ON A.COLOR_CD=B.COLOR_CD ";
            SQL = SQL + "        INNER JOIN  ";
            SQL = SQL + "        ( ";
            SQL = SQL + "            SELECT COLOR_CD, ACTUAL_GMT_QTY FROM CP_MO_OVER_SHORT_SUMMARY WHERE MO_NO='" + MoNo + "' ";
            SQL = SQL + "        ) C ";
            SQL = SQL + "        ON B.COLOR_CD=C.COLOR_CD ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetJoHeaderInfo(string MoNo)
        {
            string SQL = "         SELECT A.MO_NO, A.JO_NO, SUM (ORDER_QTY) AS ORDER_QTY, SUM(ISNULL(B.QTY,0)) AS TOTAL_SAMPLE_QTY, ";
            SQL = SQL + "		SUM(ACTUAL_QTY) AS ACTUAL_QTY, SUM (ADJUST_QTY) AS ADJUST_QTY, CASE ";
            SQL = SQL + "		SUM(ORDER_QTY) WHEN 0 THEN 0 ELSE cast(ROUND (SUM (ADJUST_QTY) * ";
            SQL = SQL + "		100.0 / SUM (ORDER_QTY), 2) as decimal(10,2)) END AS [PERCENT] ";
            SQL = SQL + "		FROM CP_MO_OVER_SHORT_ALLOCATION  A ";
            SQL = SQL + "		LEFT JOIN  CP_MO_PREPROD_SAMPLE B ON A.MO_NO=B.MO_NO AND A.JO_NO=B.JO_NO AND A.COLOR_CD=B.COLOR_CD AND A.SIZE_CD=B.SIZE_CD ";
            SQL = SQL + "		WHERE A.MO_NO = '" + MoNo + "' GROUP BY ";
            SQL = SQL + "		A.MO_NO, A.JO_NO ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetJoDetailInfo(string MoNo)
        {
            string SQL = "     SELECT A.*,ISNULL(B.QTY,0) AS SAMPLE_QTY FROM  ";
            SQL = SQL + "	( ";
            SQL = SQL + "	SELECT A.MO_NO, A.JO_NO, A.COLOR_CD,H.YPD, A.SIZE_CD,B.SEQ, A.ORDER_QTY, ";
            SQL = SQL + "			A.ACTUAL_QTY, A.ADJUST_QTY,D.COLOR_DESC,E.PATTERN_TYPE_DESC, ";
            SQL = SQL + "			CASE A.ORDER_QTY WHEN 0 THEN 0 ELSE cast(ROUND(100.0*A.ADJUST_QTY/A.ORDER_QTY,2) as decimal(10,2)) END AS [PERCENT]  ";
            SQL = SQL + "			FROM CP_MO_OVER_SHORT_ALLOCATION A INNER JOIN CP_MO_JO G ON A.MO_NO=G.MO_NO AND A.JO_NO=G.JO_NO AND A.COLOR_CD=G.COLOR_CD  ";
            SQL = SQL + "			INNER JOIN dbo.FN_GET_MO_COLOR_YPD('" + MoNo + "') H ON H.COLOR_CD=G.ASSOCIATED_COLOR AND H.REF_COLOR=G.COLOR_CD ";
            SQL = SQL + "            , CP_SIZE_SEQ B ";
            SQL = SQL + "            ,CP_MO_HD C,V_FABRIC_INFO D LEFT JOIN GEN_FAB_PATTERN_TYPE E ON ";
            SQL = SQL + "            D.FAB_PATTERN=E.PATTERN_TYPE_CD WHERE A.MO_NO = B.MO_NO AND ";
            SQL = SQL + "            A.SIZE_CD = B.SIZE_CD AND A.MO_NO=C.MO_NO AND C.GO_NO=D.GO_NO ";
            SQL = SQL + "            AND C.PART_TYPE=D.PART_TYPE_CD AND D.COLOR_CD=A.COLOR_CD AND A.MO_NO= '" + MoNo + "'  ";
            SQL = SQL + "            AND A.ORDER_QTY>0  ";
            SQL = SQL + "             ";
            SQL = SQL + "    ) A LEFT JOIN          ";
            SQL = SQL + "        (     ";
            SQL = SQL + "        SELECT JO_NO, COLOR_CD, SIZE_CD, SUM(QTY) AS QTY  FROM  ";
            SQL = SQL + "            CP_MO_PREPROD_SAMPLE ";
            SQL = SQL + "        WHERE MO_NO='" + MoNo + "' ";
            SQL = SQL + "        GROUP BY JO_NO, COLOR_CD, SIZE_CD ";
            SQL = SQL + "    ) B ON A.JO_NO=B.JO_NO AND A.COLOR_CD=B.COLOR_CD AND A.SIZE_CD=B.SIZE_CD ";
            SQL = SQL + "    ORDER BY A.JO_NO, A.COLOR_CD, A.SEQ ";
            return DBUtility.GetTable(SQL, "MES");
        }
        //EGMCutPlan
        public static DataTable GetJoDetailInfoCutPlan(string MoNo)
        {
           string SQL="     select A.MO_NO,A.JO_NO,A.Color_CD,A.YPD,B.SIZE_CD,";
                  SQL += " B.Assign_qty as Order_qty,B.Assign_qty+B.Adjust_QTY as Actual_qty,b.Adjust_qty,A.color_DESC,A.Pattern_type_desc,A.[PERCENT],A.SAMPLE_QTY";
                SQL+=" from (SELECT A.*,ISNULL(B.QTY,0) AS SAMPLE_QTY FROM "; 
                SQL+=" ( 	SELECT A.MO_NO, A.JO_NO, A.COLOR_CD,H.YPD, A.SIZE_CD,B.SEQ, A.ORDER_QTY,"; 		
                SQL+=" A.ACTUAL_QTY, A.ADJUST_QTY,D.COLOR_DESC,E.PATTERN_TYPE_DESC, ";			
                SQL+=" CASE A.ORDER_QTY WHEN 0 THEN 0 ELSE cast(ROUND(100.0*A.ADJUST_QTY/A.ORDER_QTY,2) as decimal(10,2)) END AS [PERCENT]";  		
                SQL+=" FROM CP_MO_OVER_SHORT_ALLOCATION A INNER JOIN CP_MO_JO G ON A.MO_NO=G.MO_NO AND A.JO_NO=G.JO_NO AND A.COLOR_CD=G.COLOR_CD "; 		
                SQL+=" INNER JOIN dbo.FN_GET_MO_COLOR_YPD('"+MoNo+"') H ON H.COLOR_CD=G.ASSOCIATED_COLOR AND H.REF_COLOR=G.COLOR_CD             ,"; 
                SQL+=" CP_SIZE_SEQ B             ,CP_MO_HD C,V_FABRIC_INFO D LEFT JOIN GEN_FAB_PATTERN_TYPE E ";
                SQL+=" ON             D.FAB_PATTERN=E.PATTERN_TYPE_CD ";
                SQL+=" WHERE A.MO_NO = B.MO_NO AND             A.SIZE_CD = B.SIZE_CD AND A.MO_NO=C.MO_NO AND C.GO_NO=D.GO_NO            ";
                SQL+=" AND C.PART_TYPE=D.PART_TYPE_CD AND D.COLOR_CD=A.COLOR_CD AND A.MO_NO= '"+MoNo+"'             ";
                SQL+=" AND A.ORDER_QTY>0                   ) A LEFT JOIN                  ";
                SQL+=" (             SELECT JO_NO, COLOR_CD, SIZE_CD, SUM(QTY) AS QTY  FROM              CP_MO_PREPROD_SAMPLE       ";
                SQL+= " WHERE MO_NO='"+MoNo+"'         GROUP BY JO_NO, COLOR_CD, SIZE_CD     ) B ";
                SQL+=" ON A.JO_NO=B.JO_NO AND A.COLOR_CD=B.COLOR_CD AND A.SIZE_CD=B.SIZE_CD    ";
                SQL+=" ) A right join";
                SQL+=" (select * from CP_MO_COLOR_SIZE_COMBINE_INFO where Mo_no='"+MoNo+"') B";
                SQL+=" on A.jo_no=B.JO_no and A.Color_cd=B.Combine_color and A.size_cd=B.combine_size";
                SQL+=" ORDER BY A.JO_NO, A.COLOR_CD, A.SEQ ";

            return DBUtility.GetTable(SQL, "MES");
        }
      
       
        //mesSpreadingSummary
        public static DataTable GetSpreadingSummaryInfo(string moNo, string joNo, string fromCutLotNo, string toCutLotNo)
        {
            string SQL = "";
            //将每个Marker_set_id下的size信息全部整理成横向;
            SQL = SQL + " IF OBJECT_ID('tempdb..#tmp_T') IS NOT NULL ";
            SQL = SQL + "    DROP TABLE #tmp_T;";
            SQL = SQL + " select  convert(varchar(MAX),MARKER_SET_ID) as MARKER_SET_ID,size_cd+'*'+convert(varchar(MAX),ratio) as SIZE_RATIO into #tmp_T ";
            SQL = SQL + " from CP_MARKER_SET_SIZE_RATIO where 1=2;";
            SQL = SQL + " declare @s varchar(MAX)";
            SQL = SQL + " declare @marker_set_id varchar(MAX)";
            SQL = SQL + " declare cur CURSOR";
            SQL = SQL + " FORWARD_ONLY STATIC FOR";
            SQL = SQL + " SELECT  DISTINCT       R.MARKER_SET_ID FROM CP_MARKER_SET AS R         ";
            SQL = SQL + " LEFT OUTER JOIN CP_MARKER AS E ON R.MARKER_SET_ID =         E.MARKER_SET_ID ";
            SQL = SQL + " WHERE (R.MO_NO LIKE '%" + moNo + "%')";
            SQL = SQL + "     OPEN cur;";
            SQL = SQL + " fetch next from cur into @marker_set_id";
            SQL = SQL + " while @@fetch_status = 0";
            SQL = SQL + " begin";
            SQL = SQL + " set @s = null;";
            SQL = SQL + " select @s=isnull(@s+';' , '')+size_cd+'*'+convert(varchar(MAX),ratio) from dbo.CP_MARKER_SET_SIZE_RATIO";
            SQL = SQL + " WHERE MARKER_SET_ID = @marker_set_id;";
            SQL = SQL + " insert into #tmp_T (MARKER_SET_ID,SIZE_RATIO) values(@marker_set_id,@S)";
            SQL = SQL + " fetch next from cur into @marker_set_id";
            SQL = SQL + " end";
            SQL = SQL + " close cur;";
            SQL = SQL + " deallocate cur;";
            // SQL = SQL + " select * from #tmp_T"; 
            //-------------------------------------------------------------------Created by haiqiang
            SQL = SQL + " ";
            SQL = SQL + " SELECT E_1.MO_NO, GC.SHORT_NAME AS Customer, V.JO_NO,         V.COLOR_CD, ";
            SQL = SQL + " '' AS STYLING, T.CUT_TABLE_NO AS CUT_LOT_NO,         E_1.MARKER_ID, ";
            SQL = SQL + " cast(ROUND(L.MARKER_LEN_YDS + L.MARKER_LEN_INCH /         36, 2) as decimal(10,2)) AS MARKER_LEN,";
            SQL = SQL + "  '' AS QTY_LAY, '' AS         QTY_ROLL, V.PLYS, V.SIZE_NUM, V.SIZE_NUM * V.PLYS AS GMT_QTY, ";
            SQL = SQL + " '' AS COLOR_L_D, V.FABQTY, cast(ROUND(V.FABQTY * (1 +B.MARKER_WASTAGE / 100), 2) as decimal(10,2)) AS TotalFABQTY,";
            SQL = SQL + " E_1.WIDTH, V.FABRIC_DESC,V.BATCH_NO,SIZE_RATIO.SIZE_RATIO FROM ";
            SQL = SQL + " (";
            SQL = SQL + " SELECT R.MARKER_SET_ID, R.MO_NO, E.MARKER_ID,E.WIDTH, E.MARKER_NAME";
            SQL = SQL + " FROM CP_MARKER_SET AS R ";
            SQL = SQL + " LEFT OUTER JOIN CP_MARKER AS E ON         R.MARKER_SET_ID = E.MARKER_SET_ID";
            SQL = SQL + " WHERE (R.MO_NO LIKE '%" + moNo + "%') AND (R.STATUS NOT IN ('DA'))";
            SQL = SQL + " GROUP BY R.MARKER_SET_ID, R.MO_NO, E.MARKER_ID,E.WIDTH, E.MARKER_NAME";
            SQL = SQL + " ) AS E_1 ";
            SQL = SQL + " INNER JOIN         CP_MO_HD AS B ON E_1.MO_NO = B.MO_NO ";
            SQL = SQL + " INNER JOIN SC_HD AS SH ";
            SQL = SQL + " LEFT OUTER JOIN STYLE_HD AS U ON SH.STYLE_NO = U.STYLE_NO AND         SH.STYLE_REV_NO = U.STYLE_REV_NO ";
            SQL = SQL + " LEFT OUTER JOIN GEN_CUSTOMER AS         GC ON SH.CUSTOMER_CD = GC.CUSTOMER_CD ON B.GO_NO = SH.SC_NO         ";
            SQL = SQL + " INNER JOIN CP_FABRIC_HD AS D ON B.GO_NO = D.GO_NO and D.PART_TYPE_CD=b.PART_TYPE";
            SQL = SQL + " INNER JOIN         CP_MARKER_LEN_UTIL AS L ON E_1.MARKER_ID = L.MARKER_ID";
            SQL = SQL + " INNER JOIN CP_CUT_TABLE AS T ON E_1.MARKER_ID = T.MARKER_ID";
            SQL = SQL + " INNER JOIN         ";
            SQL = SQL + " (";
            SQL = SQL + " SELECT M.MARKER_ID,         C.CUT_TABLE_NO,M.MARKER_SET_ID, P.JO_NO, P.COLOR_CD, P.PLYS, G.SIZE_NUM,         ";
            SQL = SQL + " CAST(ROUND(SUM(y.MARKER_LEN_YDS + y.MARKER_LEN_INCH / 36) * P.PLYS, 2) AS decimal(10, 2)) AS FABQTY ,";
            SQL = SQL + " I.PPO_NO AS FABRIC_DESC,I.BATCH_NO";
            SQL = SQL + " FROM         (SELECT         R.MARKER_SET_ID, R.MO_NO, ";
            SQL = SQL + " E.MARKER_ID FROM CP_MARKER_SET AS R";
            SQL = SQL + " LEFT OUTER JOIN CP_MARKER AS E ON R.MARKER_SET_ID =         E.MARKER_SET_ID ";
            SQL = SQL + " WHERE (R.MO_NO LIKE '%" + moNo + "%')) AS M         ";
            SQL = SQL + " INNER JOIN CP_CUT_TABLE AS C ON M.MARKER_ID = C.MARKER_ID ";
            SQL = SQL + " INNER         JOIN CP_CUT_TABLE_JO_PLYS AS P ON C.MARKER_ID = P.MARKER_ID ";
            // SQL = SQL + " AND         C.CUT_TABLE_NO = P.CUT_TABLE_NO AND C.REF_COLOR = P.COLOR_CD ";
             SQL = SQL + " AND         C.CUT_TABLE_NO = P.CUT_TABLE_NO ";
            SQL = SQL + " AND         (ISNULL(C.REF_JO,'')='' OR C.REF_JO=P.JO_NO)         ";
            SQL = SQL + " INNER JOIN CP_MARKER_FABRIC H ON C.MARKER_ID=H.MARKER_ID AND         C.REF_COLOR=H.COLOR_CD";
            SQL = SQL + " INNER JOIN CP_FABRIC_ITEM I ON H.FAB_ITEM_ID=I.FAB_ITEM_ID         ";
            SQL = SQL + " INNER JOIN CP_MARKER_LEN_UTIL AS y ON M.MARKER_ID = y.MARKER_ID         ";
            SQL = SQL + " INNER JOIN         (SELECT MARKER_ID, SUM(RATIO) AS SIZE_NUM FROM         CP_MARKER_SIZE_RATIO";
            SQL = SQL + " WHERE (MARKER_ID IN (SELECT A.MARKER_ID         FROM CP_MARKER AS A ";
            SQL = SQL + " INNER JOIN CP_MARKER_SET AS B ON         A.MARKER_SET_ID = B.MARKER_SET_ID ";
            SQL = SQL + " WHERE (B.MO_NO LIKE          '%" + moNo + "%'))) GROUP BY MARKER_ID) AS G ";
            SQL = SQL + " ON M.MARKER_ID =         G.MARKER_ID         ";
            SQL = SQL + " GROUP BY M.MARKER_ID,M.MARKER_SET_ID, C.CUT_TABLE_NO, P.JO_NO,         ";
            SQL = SQL + " P.COLOR_CD, P.PLYS, G.SIZE_NUM,I.PPO_NO,I.BATCH_NO";
            SQL = SQL + " ) AS V";
            //SQL = SQL + " ON L.MARKER_ID =         V.MARKER_ID AND T.CUT_TABLE_NO = V.CUT_TABLE_NO AND T.REF_COLOR         = V.COLOR_CD ";
            SQL = SQL + " ON L.MARKER_ID =         V.MARKER_ID AND T.CUT_TABLE_NO = V.CUT_TABLE_NO ";
            SQL = SQL + " AND (ISNULL(T.REF_JO,'')='' OR T.REF_JO=V.JO_NO)";
            SQL = SQL + " INNER JOIN";
            SQL = SQL + " #tmp_T AS SIZE_RATIO ON SIZE_RATIO.MARKER_SET_ID = V.MARKER_SET_ID";
            SQL = SQL + " WHERE 1 = 1  ";
            if (joNo != "")
            {
                SQL += " AND (V.JO_NO LIKE '%" + joNo + "%')";
            }
            if (fromCutLotNo != "")
            {
                SQL += " AND T.CUT_TABLE_NO >= '" + fromCutLotNo + "'";
            }
            if (toCutLotNo != "")
            {
                SQL += " AND T.CUT_TABLE_NO <= '" + toCutLotNo + "'";
            }
            SQL += " ORDER BY E_1.MARKER_ID, CUT_LOT_NO";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetSpreadingSummaryInfoForEAV(string moNo, string joNo, string fromCutLotNo, string toCutLotNo)
        {
            string SQL = "";

            SQL += " IF OBJECT_ID('tempdb..#tmp_T') IS NOT NULL     ";
            SQL += " DROP TABLE #tmp_T;";
            SQL += " ";
            SQL += " select  convert(varchar(MAX),MARKER_SET_ID) as MARKER_SET_ID,size_cd+'*'+convert(varchar(MAX),ratio) as SIZE_RATIO into #tmp_T  ";
            SQL += " FROM CP_MARKER_SET_SIZE_RATIO ";
            SQL += " WHERE  1=2;";
            SQL += "  declare @s varchar(MAX) declare @marker_set_id varchar(MAX) declare cur CURSOR ";
            SQL += " FORWARD_ONLY STATIC FOR ";
            SQL += " SELECT  DISTINCT       R.MARKER_SET_ID ";
            SQL += " FROM CP_MARKER_SET AS R          LEFT OUTER JOIN CP_MARKER AS E ";
            SQL += " ON R.MARKER_SET_ID =         E.MARKER_SET_ID  ";
            SQL += " WHERE  (R.MO_NO LIKE '%" + moNo + "%')     OPEN cur;";
            SQL += " fetch next FROM cur into @marker_set_id ";
            SQL += " while @@fetch_status = 0 ";
            SQL += " begin ";
            SQL += " 	set @s = null;";
            SQL += " 	select @s=isnull(@s+';' , '')+size_cd+'*'+convert(varchar(MAX),ratio) ";
            SQL += " 	FROM dbo.CP_MARKER_SET_SIZE_RATIO ";
            SQL += " 	WHERE  MARKER_SET_ID = @marker_set_id;";
            SQL += " 	insert into #tmp_T (MARKER_SET_ID,SIZE_RATIO) values(@marker_set_id,@S) fetch next ";
            SQL += " FROM cur into @marker_set_id end ";
            SQL += " close cur;";
            SQL += " deallocate cur;";
            SQL += " ";
            SQL += " SELECT DISTINCT E_1.MO_NO, GC.SHORT_NAME AS Customer, V.JO_NO,         ";
            SQL += " V.COLOR_CD,  '' AS STYLING, T.CUT_TABLE_NO AS CUT_LOT_NO,         ";
            SQL += " E_1.MARKER_ID,  cast(ROUND(L.MARKER_LEN_YDS + L.MARKER_LEN_INCH /         36, 2) as decimal(10,2)) AS MARKER_LEN,  ";
            SQL += " '' AS QTY_LAY, '' AS         QTY_ROLL, V.PLYS, V.SIZE_NUM, V.SIZE_NUM * V.PLYS AS GMT_QTY,  ";
            SQL += " '' AS COLOR_L_D, V.FABQTY, ";
            SQL += " (1 +B.MARKER_WASTAGE / 100) AS MARKER_WASTAGE, ";
            SQL += " E_1.WIDTH, V.FABRIC_DESC,V.BATCH_NO,SIZE_RATIO.SIZE_RATIO ";
            SQL += " FROM  ";
            SQL += " ( ";
            SQL += " SELECT R.MARKER_SET_ID, R.MO_NO, E.MARKER_ID,E.WIDTH, E.MARKER_NAME ";
            SQL += " FROM CP_MARKER_SET AS R  LEFT OUTER JOIN CP_MARKER AS E ";
            SQL += " ON         R.MARKER_SET_ID = E.MARKER_SET_ID ";
            SQL += " WHERE  (R.MO_NO LIKE '%" + moNo + "%') AND (R.STATUS NOT IN ('DA')) ";
            SQL += " GROUP BY R.MARKER_SET_ID, R.MO_NO, E.MARKER_ID,E.WIDTH, E.MARKER_NAME ";
            SQL += " ) AS E_1  ";
            SQL += " INNER JOIN         CP_MO_HD AS B ";
            SQL += " ON E_1.MO_NO = B.MO_NO  ";
            SQL += " INNER JOIN SC_HD AS SH  ";
            SQL += " LEFT OUTER JOIN STYLE_HD AS U ";
            SQL += " ON SH.STYLE_NO = U.STYLE_NO AND         SH.STYLE_REV_NO = U.STYLE_REV_NO  LEFT OUTER JOIN GEN_CUSTOMER AS         GC ";
            SQL += " ON SH.CUSTOMER_CD = GC.CUSTOMER_CD ";
            SQL += " ON B.GO_NO = SH.SC_NO          ";
            SQL += " INNER JOIN CP_FABRIC_HD AS D ";
            SQL += " ON B.GO_NO = D.GO_NO and D.PART_TYPE_CD=b.PART_TYPE ";
            SQL += " INNER JOIN         CP_MARKER_LEN_UTIL AS L ";
            SQL += " ON E_1.MARKER_ID = L.MARKER_ID ";
            SQL += " INNER JOIN CP_CUT_TABLE AS T ";
            SQL += " ON E_1.MARKER_ID = T.MARKER_ID ";
            SQL += " INNER JOIN          ";
            SQL += " ( ";
            SQL += " SELECT M.MARKER_ID,         C.CUT_TABLE_NO,M.MARKER_SET_ID";
            SQL += " ,P.JO_NO, P.COLOR_CD";
            SQL += " ,C.PLYS, G.SIZE_NUM,          ";
            SQL += " CAST(ROUND(SUM(y.MARKER_LEN_YDS + y.MARKER_LEN_INCH / 36) * P.QTY, 2) AS decimal(10, 2)) AS FABQTY , ";
            SQL += " I.PPO_NO AS FABRIC_DESC,I.BATCH_NO ";
            SQL += " FROM         ";
            SQL += " (";
            SQL += " SELECT         R.MARKER_SET_ID, R.MO_NO,  E.MARKER_ID ";
            SQL += " FROM CP_MARKER_SET AS R LEFT OUTER JOIN CP_MARKER AS E ";
            SQL += " ON R.MARKER_SET_ID =         E.MARKER_SET_ID  ";
            SQL += " WHERE  (R.MO_NO LIKE '%" + moNo + "%')";
            SQL += " ) AS M          ";
            SQL += " INNER JOIN CP_CUT_TABLE AS C ";
            SQL += " ON M.MARKER_ID = C.MARKER_ID  ";
            SQL += " INNER JOIN CP_CUT_TABLE_SIZE_QTY P ";
            SQL += " ON C.MARKER_ID=P.MARKER_ID AND C.CUT_TABLE_NO = P.CUT_TABLE_NO AND C.REF_COLOR = P.COLOR_CD AND         (ISNULL(C.REF_JO,'')='' OR C.REF_JO=P.JO_NO)";
            SQL += " INNER JOIN CP_MARKER_FABRIC H ";
            SQL += " ON C.MARKER_ID=H.MARKER_ID AND         C.REF_COLOR=H.COLOR_CD ";
            SQL += " INNER JOIN CP_FABRIC_ITEM I ";
            SQL += " ON H.FAB_ITEM_ID=I.FAB_ITEM_ID          ";
            SQL += " INNER JOIN CP_MARKER_LEN_UTIL AS y ";
            SQL += " ON M.MARKER_ID = y.MARKER_ID          ";
            SQL += " INNER JOIN         ";
            SQL += " (";
            SQL += " SELECT MARKER_ID, SUM(RATIO) AS SIZE_NUM ";
            SQL += " FROM         CP_MARKER_SIZE_RATIO ";
            SQL += " WHERE  (MARKER_ID IN (SELECT A.MARKER_ID         ";
            SQL += " FROM CP_MARKER AS A  ";
            SQL += " INNER JOIN CP_MARKER_SET AS B ";
            SQL += " ON         A.MARKER_SET_ID = B.MARKER_SET_ID  ";
            SQL += " WHERE  (B.MO_NO LIKE          '%" + moNo + "%'))) ";
            SQL += " GROUP BY MARKER_ID";
            SQL += " ) AS G  ";
            SQL += " ON M.MARKER_ID =         G.MARKER_ID          ";
            SQL += " GROUP BY M.MARKER_ID,M.MARKER_SET_ID, C.CUT_TABLE_NO, P.JO_NO,          P.COLOR_CD,";
            SQL += " C.PLYS,P.QTY,G.SIZE_NUM,I.PPO_NO,I.BATCH_NO ";
            SQL += " ) AS V ";
            SQL += " ON L.MARKER_ID =         V.MARKER_ID AND T.CUT_TABLE_NO = V.CUT_TABLE_NO AND T.REF_COLOR         = V.COLOR_CD  AND (ISNULL(T.REF_JO,'')='' OR T.REF_JO=V.JO_NO) ";
            SQL += " INNER JOIN #tmp_T AS SIZE_RATIO ";
            SQL += " ON SIZE_RATIO.MARKER_SET_ID = V.MARKER_SET_ID ";
            SQL += " WHERE  1 = 1 ";
            if (joNo != "")
            {
                SQL += " AND (V.JO_NO LIKE '%" + joNo + "%')";
            }
            if (fromCutLotNo != "")
            {
                SQL += " AND T.CUT_TABLE_NO >= '" + fromCutLotNo + "'";
            }
            if (toCutLotNo != "")
            {
                SQL += " AND T.CUT_TABLE_NO <= '" + toCutLotNo + "'";
            }
            SQL += " ORDER BY E_1.MARKER_ID, CUT_LOT_NO";

            return DBUtility.GetTable(SQL, "MES");
        }


        //mesSpreadingPlan

        public static DataTable GetHeaders(string moNo, string markerId, bool cbBnm)
        {
            string SQL = "         SELECT DISTINCT E.MARKER_ID,B.PART_TYPE, E.MO_NO,B.GO_NO, ";
            if (cbBnm)
            {
                SQL = SQL + "     STYLE_DESC=U.STYLE_DESC + '(' + CM.SHORT_NAME + ')',";
            }
            else
            {
                SQL = SQL + "     U.STYLE_DESC,";
            }
            SQL = SQL + "  T.CUT_TABLE_NO ";
            SQL = SQL + "  BED_NO,CONVERT(VARCHAR(10),GETDATE(),121)as PRINT_DATE, ";
            SQL = SQL + "  E.SIZE_REMARK AS SIZE_DESC, L.MARKER_LEN_YDS YARD, ";
            SQL = SQL + "  L.MARKER_LEN_INCH INCH, E.MARKER_NAME, D.REMARKS ";
            SQL = SQL + "  DESCRIPTION,D.CREATOR,B.REMARKS AS MO_REMARKS FROM ( SELECT ";
            SQL = SQL + "  R.MARKER_SET_ID,dbo.FN_GET_MARKERSET_SIZE_REMARK(R.MARKER_SET_ID) ";
            SQL = SQL + "  AS SIZE_REMARK, R.MO_NO,E.MARKER_ID, E.MARKER_NAME FROM ";
            SQL = SQL + "  CP_MARKER_SET R LEFT JOIN CP_MARKER E ON ";
            SQL = SQL + "  R.MARKER_SET_ID=E.MARKER_SET_ID WHERE R.MO_NO='" + moNo + "' ";
            if (markerId != "")
            {
                SQL += " AND E.MARKER_ID='" + markerId + "'";
            }
            SQL = SQL + "        AND R.STATUS not in ('DA') ) E,CP_MO_HD B,CP_FABRIC_HD D,SC_HD ";
            SQL = SQL + "  SH LEFT JOIN STYLE_HD U ON SH.STYLE_NO=U.STYLE_NO AND ";
            SQL = SQL + "  SH.STYLE_REV_NO=U.STYLE_REV_NO ";
            if (cbBnm)
            {
                SQL = SQL + "  LEFT JOIN GEN_CUSTOMER CM ";
                SQL = SQL + "  ON CM.CUSTOMER_CD=SH.CUSTOMER_CD";
            }
            SQL = SQL + "        ,CP_MARKER_LEN_UTIL ";
            SQL = SQL + "  L,CP_CUT_TABLE T WHERE E.MO_NO=B.MO_NO AND B.GO_NO=SH.SC_NO AND ";
            SQL = SQL + "  B.GO_NO=D.GO_NO AND B.PART_TYPE=D.PART_TYPE_CD AND E.MARKER_ID=L.MARKER_ID AND ";
            SQL = SQL + "  E.MARKER_ID=T.MARKER_ID ORDER BY ";
            SQL = SQL + "  B.GO_NO,E.MARKER_ID,T.CUT_TABLE_NO ";
            return DBUtility.GetTable(SQL, "MES");
        }


        public static DataTable JudageNewOrOld(string MO)
        {
            string SQL = "";
            SQL += " Select  1  from CP_MARKER_SET_SEQ  where MO_NO ='" + MO + "'";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetSpreadings(string moNo, string markerId, string bedNo)
        {
            string SQL = "         SELECT ";
            SQL = SQL + "		A.MARKER_ID,A.COLOR_CD,F.PPO_NO,F.COMBO_NAME,F.SHADE_LOT,F.BATCH_NO,F.WIDTH,A.PLYS,A.SHRINKAGE, ";
            SQL = SQL + "		cast(ROUND(A.PLYS*(L.MARKER_LEN_YDS+L.MARKER_LEN_INCH/36),2) as ";
            SQL = SQL + "		decimal(10,2)) FAB_QTY,A.CUT_TABLE_NO BED_NO, A.COLOR_DESC, ";
            SQL = SQL + "		A.PATTERN_TYPE_DESC, A.CUT_DIRECTION_TYPE, A.MATERIAL_TYPE, ";
            SQL = SQL + "		A.FAB_THICKNESS_TYPE FROM ( SELECT ";
            SQL = SQL + "		H.GO_NO,S.MARKER_SET_ID,M.MARKER_ID,T.CUT_TABLE_NO,T.REF_COLOR ";
            SQL = SQL + "		AS ";
            SQL = SQL + "		COLOR_CD,M.SHRINKAGE,M.WIDTH,T.PLYS,H.PART_TYPE,Q.COLOR_DESC,R.PATTERN_TYPE_DESC,Q.CUT_DIRECTION_TYPE,Q.MATERIAL_TYPE,Q.FAB_THICKNESS_TYPE ";
            SQL = SQL + "        FROM CP_MO_HD H,CP_FABRIC_ITEM Q LEFT JOIN GEN_FAB_PATTERN_TYPE ";
            SQL = SQL + "        R ON Q.FAB_PATTERN=R.PATTERN_TYPE_CD, CP_MARKER_SET S,CP_MARKER ";
            SQL = SQL + "        M,CP_CUT_TABLE T WHERE H.MO_NO=S.MO_NO AND H.GO_NO=Q.GO_NO AND ";
            SQL = SQL + "        H.PART_TYPE=Q.PART_TYPE_CD AND Q.COLOR_CD=T.REF_COLOR AND ";
            SQL = SQL + "        S.MARKER_SET_ID=M.MARKER_SET_ID AND Q.WIDTH=M.WIDTH AND ";
            SQL = SQL + "        Q.SHRINKAGE=M.SHRINKAGE AND M.MARKER_ID=T.MARKER_ID AND S.STATUS ";
            SQL = SQL + "        not in ('DA') AND H.MO_NO='" + moNo + "' and t.cut_table_no='" + bedNo + "' ";
            if (markerId != "")
            {
                SQL += " AND M.MARKER_ID='" + markerId + "'";
            }
            SQL = SQL + "        )A LEFT JOIN CP_FABRIC_ITEM F ON A.GO_NO=F.GO_NO AND ";
            SQL = SQL + "		A.COLOR_CD=F.COLOR_CD AND A.SHRINKAGE=F.SHRINKAGE AND ";
            SQL = SQL + "		A.WIDTH=F.WIDTH AND A.PART_TYPE=F.PART_TYPE_CD LEFT JOIN ";
            SQL = SQL + "		CP_MARKER_LEN_UTIL L ON A.MARKER_ID=L.MARKER_ID ";
            return DBUtility.GetTable(SQL, "MES");
        }
        
        //2013.07.15 ZouShCh add Start
        public static DataTable GetMergeSizesByMo(string moNo)
        {
            string SQL = @"SELECT  
                                B.MARKER_ID,
		                        CUT_TABLE_NO,
                                COMBINE_SIZE_CD ,
                                SIZE_CD ,
                                QTY
                        FROM    cp_marker_set AS A
                                INNER JOIN dbo.CP_MARKER AS B ON a.MARKER_SET_ID = b.MARKER_SET_ID
                                INNER JOIN dbo.CP_CUT_TABLE_SIZE_QTY AS C ON b.MARKER_ID = c.MARKER_ID
                        WHERE   A.MO_NO = '" + moNo+@"' ";
            return DBUtility.GetTable(SQL, "MES");
        }

        //Added By ZouShiChang ON 2013.08.14 Start
//        public static DataTable GetCombineSizeCdByMo(string moNo,string markerId,string bedNo)
//        {
//            string SQL = @"SELECT  
//                                DISTINCT
//                                COMBINE_SIZE_CD 
//                                
//                        FROM    cp_marker_set AS A
//                                INNER JOIN dbo.CP_MARKER AS B ON a.MARKER_SET_ID = b.MARKER_SET_ID
//                                INNER JOIN dbo.CP_CUT_TABLE_SIZE_QTY AS C ON b.MARKER_ID = c.MARKER_ID
//                        WHERE   A.MO_NO = '" + moNo + @"' AND B.MARKER_ID='"+markerId+@"'AND CUT_TABLE_NO='"+bedNo+@"'";                           
//                       //WHERE   A.MO_NO = '" + moNo + @"'";
                
//            return DBUtility.GetTable(SQL, "MES");
//        }
        //Added By ZouShiChang ON 2013.08.14 End

        public static DataTable GetSizeCdByMo(string moNo, string markerId, string bedNo)
        {
            string SQL = @"SELECT  
                                DISTINCT
                                SIZE_CD 
                                
                        FROM    cp_marker_set AS A
                                INNER JOIN dbo.CP_MARKER AS B ON a.MARKER_SET_ID = b.MARKER_SET_ID
                                INNER JOIN dbo.CP_CUT_TABLE_SIZE_QTY AS C ON b.MARKER_ID = c.MARKER_ID
                        WHERE   A.MO_NO = '" + moNo + @"' AND B.MARKER_ID='"+markerId+@"' AND CUT_TABLE_NO='"+bedNo+@"'";                        
                        //WHERE   A.MO_NO = '" + moNo + @"'";
               
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetMergeSizeMarkerCutTableNo(string moNo, string markerId, string bedNo)
        {

            string SQL = " SELECT 1 FROM dbo.FN_GET_MO_MARKER_SIZE_DETAIL('" + moNo + "') WHERE MARKER_ID='" + markerId + "' AND CUT_TABLE_NO='" + bedNo + "' AND SIZE_CD<>COMBINE_SIZE_CD ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetMergeSizeCrossByMo(string moNo, string markerId, string bedNo)
        {
            string SQL=@"DECLARE @SQLTEXT NVARCHAR(MAX)

                        SELECT  @SQLTEXT = 'select '


                        SELECT  @sqlText = @sqltext + '(CASE COMBINE_SIZE_CD WHEN '''
                                + COMBINE_SIZE_CD
                                + ''' THEN SIZE_CD+''|''+CONVERT(nvarchar(10),QTY) ELSE NULL END) AS '''
                                + COMBINE_SIZE_CD + ''','
                        FROM    ( SELECT DISTINCT
                                            COMBINE_SIZE_CD
                                  FROM      cp_marker_set AS A
                                            INNER JOIN dbo.CP_MARKER AS B ON a.MARKER_SET_ID = b.MARKER_SET_ID
                                            INNER JOIN dbo.CP_CUT_TABLE_SIZE_QTY AS C ON b.MARKER_ID = c.MARKER_ID
                                  WHERE     A.MO_NO = '"+moNo+@"'
                                            AND B.MARKER_ID = '"+markerId+@"'
                                            AND CUT_TABLE_NO = '"+bedNo+@"'
                                ) AS a 
                        PRINT @SQLTEXT
                        SELECT  @sqlText = LEFT(@sqlText, LEN(@sqlText) - 1) + ' from (SELECT  
		
		                        COMBINE_SIZE_CD,
                                SIZE_CD,
                                QTY
                        FROM    cp_marker_set AS A
                                INNER JOIN dbo.CP_MARKER AS B ON a.MARKER_SET_ID = b.MARKER_SET_ID
                                INNER JOIN dbo.CP_CUT_TABLE_SIZE_QTY AS C ON b.MARKER_ID = c.MARKER_ID
                        WHERE   A.MO_NO = ' + '''' + '"+moNo+@"' + '''' + ' AND B.MARKER_ID='
                                + '''' + '"+markerId+@"' + +'''' + ' AND CUT_TABLE_NO=' + '''' + '"+bedNo+@"'
                                + '''' + ') AS A '

                        PRINT @sqltext 
                        EXEC (@SQLTEXT)";

            return DBUtility.GetTable(SQL, "MES");
        }
     
        //2013.07.15 ZouShCh add End

        public static DataTable GetSpreadingsByMo(string moNo)
        {
            string SQL = "";
            SQL += @"SELECT A.MARKER_ID,A.COLOR_CD,A.PPO_NO,A.COMBO_NAME,A.SHADE_LOT,A.BATCH_NO,A.WIDTH,A.PLYS,A.SHRINKAGE, 
                            cast(ROUND(A.PLYS*(L.MARKER_LEN_YDS+L.MARKER_LEN_INCH/36),2) as 
                            decimal(10,2)) FAB_QTY,A.CUT_TABLE_NO BED_NO, A.COLOR_DESC, 
                            A.PATTERN_TYPE_DESC, A.CUT_DIRECTION_TYPE, A.MATERIAL_TYPE, 
                            A.FAB_THICKNESS_TYPE FROM ( 
                            SELECT 
                            H.GO_NO,S.MARKER_SET_ID,M.MARKER_ID,T.CUT_TABLE_NO,T.REF_COLOR AS 
                            COLOR_CD,M.SHRINKAGE,T.PLYS,H.PART_TYPE,Q.COLOR_DESC,R.PATTERN_TYPE_DESC
                            ,Q.CUT_DIRECTION_TYPE,Q.MATERIAL_TYPE,Q.FAB_THICKNESS_TYPE
                            ,Q.PPO_NO,Q.COMBO_NAME,Q.SHADE_LOT,Q.BATCH_NO,Q.WIDTH
                            FROM CP_MO_HD H WITH(NOLOCK) 
                            INNER JOIN CP_MARKER_SET S WITH(NOLOCK) ON S.MO_NO=H.MO_NO
                            INNER JOIN CP_MARKER M WITH(NOLOCK) ON M.MARKER_SET_ID = S.MARKER_SET_ID
                            INNER JOIN CP_CUT_TABLE T WITH(NOLOCK) ON T.MARKER_ID=M.MARKER_ID 
                            INNER JOIN CP_FABRIC_ITEM Q WITH(NOLOCK)  ON Q.GO_NO=H.GO_NO AND Q.PART_TYPE_CD = H.PART_TYPE 
                            AND Q.WIDTH=M.WIDTH AND Q.SHRINKAGE=M.SHRINKAGE AND Q.COLOR_CD = T.REF_COLOR
                            LEFT JOIN GEN_FAB_PATTERN_TYPE R WITH(NOLOCK)  ON R.PATTERN_TYPE_CD = Q.FAB_PATTERN
                            WHERE  S.STATUS not in ('DA') AND H.MO_NO='" + moNo + "'";
            SQL += @")A 
                            LEFT JOIN CP_MARKER_LEN_UTIL L WITH(NOLOCK)  ON A.MARKER_ID=L.MARKER_ID 
                            ORDER BY A.COLOR_CD";//Added by Jin Song -  Bug Fix 7/23/14
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetAllocations(string moNo, string markerId, string bedNo)
        {
            //string SQL = "         SELECT M.MARKER_ID, C.CUT_TABLE_NO, P.JO_NO, P.COLOR_CD, P.PLYS, ";
            string SQL = "         SELECT M.MARKER_ID, CONVERT(VARCHAR(10),BUYER_PO_DEL_DATE,120) AS BUYER_PO_DEL_DATE,";
            SQL = SQL + "       C.CUT_TABLE_NO, P.JO_NO, P.COLOR_CD, P.PLYS, ";
            SQL = SQL + "		cast(ROUND (SUM (Y.MARKER_LEN_YDS + Y.MARKER_LEN_INCH / ";
            SQL = SQL + "		36)*P.PLYS, 2) as decimal(10,2)) FABQTY ,";
            SQL = SQL + "       cast(ROUND (SUM (Y.MARKER_LEN_YDS + Y.MARKER_LEN_INCH / 		36)*P.PLYS*(1+ISNULL(MARKER_WASTAGE,0)/100), 2) as decimal(10,2)) [FABQTY2]";
            SQL = SQL + "       FROM (SELECT ";
            SQL = SQL + "		R.MARKER_SET_ID, R.MO_NO, E.MARKER_ID FROM CP_MARKER_SET R LEFT ";
            SQL = SQL + "		JOIN CP_MARKER E ON R.MARKER_SET_ID = E.MARKER_SET_ID WHERE ";
            SQL = SQL + "		R.MO_NO ='" + moNo + "' ";
            if (markerId != "")
            {
                SQL += " AND E.MARKER_ID='" + markerId + "'";
            }
            SQL = SQL + "        ) M, CP_CUT_TABLE C, CP_CUT_TABLE_JO_PLYS P, CP_MARKER_LEN_UTIL Y";
            //SQL = SQL + "		 WHERE M.MARKER_ID = C.MARKER_ID AND C.MARKER_ID = P.MARKER_ID ";
            SQL = SQL + "       ,CP_MO_HD MH";
            SQL = SQL + "		,JO_HD HD WHERE M.MARKER_ID = C.MARKER_ID AND C.MARKER_ID = P.MARKER_ID ";
            SQL = SQL + "       AND MH.MO_NO =M.MO_NO";
            SQL = SQL + "		AND C.CUT_TABLE_NO = P.CUT_TABLE_NO AND C.REF_COLOR = P.COLOR_CD ";
            SQL = SQL + "       AND P.JO_NO = HD.JO_NO ";
            SQL = SQL + "		AND (ISNULL(C.REF_JO,'')='' OR C.REF_JO=P.JO_NO) AND M.MARKER_ID ";
            SQL = SQL + "		= Y.MARKER_ID and c.cut_table_no='" + bedNo + "' GROUP BY M.MARKER_ID, C.CUT_TABLE_NO, P.JO_NO, ";
            SQL = SQL + "       MARKER_WASTAGE,";
            SQL = SQL + "       HD.BUYER_PO_DEL_DATE,";
            SQL = SQL + "		P.COLOR_CD, P.PLYS ORDER BY P.JO_NO, P.COLOR_CD ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetAllocationsByMo(string moNo, string factoryCD, int SortBy)
        {
            //Modified by Jin Song (remove inner join on CP_CUT_TABLE for YMG, EGM, EGV, EAV and EHV to display color details)
            string SQL = "";
            //</Added by: DaiJ at: 2014/04/24>
            SQL += @"SELECT X.* FROM(";
            SQL += @"SELECT M.MARKER_ID, CONVERT(VARCHAR(10),BUYER_PO_DEL_DATE,120) AS BUYER_PO_DEL_DATE,
                            P.CUT_TABLE_NO, P.JO_NO, P.COLOR_CD, P.PLYS, 
                            cast(ROUND (SUM (Y.MARKER_LEN_YDS + Y.MARKER_LEN_INCH /36)*P.PLYS, 2) as decimal(10,2)) FABQTY ,
                            cast(ROUND (SUM (Y.MARKER_LEN_YDS + Y.MARKER_LEN_INCH /36)*P.PLYS*(1+ISNULL(MARKER_WASTAGE,0)/100), 2) as decimal(10,2)) 
                            [FABQTY2]
                            FROM (
                            SELECT R.MARKER_SET_ID, R.MO_NO, E.MARKER_ID FROM CP_MARKER_SET R WITH(NOLOCK)
                            LEFT JOIN CP_MARKER E ON R.MARKER_SET_ID = E.MARKER_SET_ID";
            SQL += @" WHERE R.MO_NO ='" + moNo + "'";
            SQL += @" ) M";
            if (!factoryCD.Contains("YMG") && !factoryCD.Contains("EGM") && !factoryCD.Contains("EGV") && !factoryCD.Contains("EAV") && !factoryCD.Contains("EHV"))
            {
                SQL += "                INNER JOIN CP_CUT_TABLE C WITH(NOLOCK) ON C.MARKER_ID = M.MARKER_ID";
            }
            SQL += "            INNER JOIN CP_CUT_TABLE_JO_PLYS P WITH(NOLOCK) ON P.MARKER_ID = M.MARKER_ID ";

            if (!factoryCD.Contains("YMG") && !factoryCD.Contains("EGM") && !factoryCD.Contains("EGV") && !factoryCD.Contains("EAV") && !factoryCD.Contains("EHV"))
            {
                SQL += "              AND P.CUT_TABLE_NO = C.CUT_TABLE_NO AND P.COLOR_CD =C.REF_COLOR";
            }
            SQL += @"                INNER JOIN CP_MARKER_LEN_UTIL Y WITH(NOLOCK) ON  Y.MARKER_ID = M.MARKER_ID
                            INNER JOIN CP_MO_HD MH WITH(NOLOCK) ON M.MO_NO =MH.MO_NO
                            INNER JOIN JO_HD HD WITH(NOLOCK) ON HD.JO_NO = P.JO_NO
                            WHERE";
            if (!factoryCD.Contains("YMG") && !factoryCD.Contains("EGM") && !factoryCD.Contains("EGV") && !factoryCD.Contains("EAV") && !factoryCD.Contains("EHV"))
            {
                SQL += "    (ISNULL(C.REF_JO,'')='' OR C.REF_JO=P.JO_NO) AND";
            }
            SQL += "             M.MO_NO='" + moNo + "'";
            SQL += @" GROUP BY M.MARKER_ID, P.CUT_TABLE_NO, P.JO_NO, 
                            MARKER_WASTAGE,
                            HD.BUYER_PO_DEL_DATE,
                            P.COLOR_CD, P.PLYS) X";
            //</Added by: DaiJ at: 2014/04/24>
            SQL += @" LEFT JOIN PRD_JO_SEQ_LIST S ON S.JO_NO=X.JO_NO
                            ORDER BY S.SEQ,";//X.JO_NO,X.COLOR_CD";
            SQL += SortBy == 1 ? "X.COLOR_CD,X.JO_NO" : "X.JO_NO,X.COLOR_CD";

            return DBUtility.GetTable(SQL, "MES");
        }

      
        public static DataTable GetNewAllcations(string factoryCD, string moNo, string markerId, string bedNo)
        {//新的逻辑;
            string Language = "";
            switch (factoryCD)
            {
                case "EGV": Language = "Vietnamese_CI_AS";
                    break;
                case "EAV": Language = "Vietnamese_CI_AS";
                    break;
                case "EHV": Language = "Vietnamese_CI_AS";
                    break;
                default: Language = "Chinese_PRC_90_CI_AS";
                    break;
            }
            string temTable = "#tam_" + DateTime.Now.Millisecond.ToString();
            string SizeTable = "#temSize_" + DateTime.Now.Millisecond.ToString();
            string OutTable = "#TEMP2_" + DateTime.Now.Millisecond.ToString();
            string SQL = "";
            SQL += @"    DECLARE @MO_NO VARCHAR(30) SET @MO_NO= '" + moNo + "' ";
            SQL += @"    DECLARE @MARKER_ID VARCHAR(30) SET @MARKER_ID = '" + markerId + "' ";
            SQL += @"    DECLARE @CUT_TABLE_NO VARCHAR(30) SET @CUT_TABLE_NO = '" + bedNo + "' ";
            SQL += @"    if   object_id( 'tempdb.." + temTable + " ')   is   not   null    drop   table   " + temTable;
            SQL += @"    SELECT DISTINCT M.MARKER_SET_NO,M.MARKER_ID,SEQ.SEQ AS MARKER_SEQ,'NULL' AS [Size Desc],C.CUT_TABLE_NO,   
                                C.COLOR_CD,FAB.COLOR_DESC,ISNULL(FAB.PATTERN_TYPE_DESC,'') AS PATTERN_TYPE_DESC, CTSQ.JO_NO, CTSQ.SIZE_CD,CSS.SEQ AS SIZE_SEQ,CTSQ.QTY ,
                                C.PLYS ,cast(ROUND (SUM (Y.MARKER_LEN_YDS + Y.MARKER_LEN_INCH /   36)*CTSQ.QTY, 2) as decimal(10,2)) FABQTY  
                                INTO " + temTable;
            SQL += @"    FROM  ( 
                                SELECT   R.MARKER_SET_ID, R.MO_NO, E.MARKER_ID ,R.MARKER_SET_NO 
                                FROM CP_MARKER_SET R  
                                LEFT   JOIN CP_MARKER E ON R.MARKER_SET_ID = E.MARKER_SET_ID  
                                WHERE   R.MO_NO =@MO_NO AND MARKER_ID = @MARKER_ID 
                                ) M 
                                INNER JOIN CP_CUT_TABLE C ON C.REF_COLOR = C.COLOR_CD AND C.MARKER_ID = M.MARKER_ID 
	                                AND C.CUT_TABLE_NO = @CUT_TABLE_NO 
                                INNER JOIN CP_MARKER_LEN_UTIL   Y ON  M.MARKER_ID   = Y.MARKER_ID 
                                INNER JOIN CP_CUT_TABLE_SIZE_QTY CTSQ ON CTSQ.MARKER_ID = M.MARKER_ID  	AND CTSQ.CUT_TABLE_NO = C.CUT_TABLE_NO 
	                                AND C.COLOR_CD = CTSQ.COLOR_CD 
                                INNER JOIN CP_MARKER_SET_SEQ SEQ ON M.MO_NO = SEQ.MO_NO AND CTSQ.JO_NO = SEQ.JO_NO 	AND SEQ.MO_NO = @MO_NO 
                                INNER JOIN  ( 
                                SELECT DISTINCT CPHD.MO_NO,CPHD.GO_NO,CMJ.JO_NO,CFI.FAB_PATTERN,GFPT.PATTERN_TYPE_DESC,CFI.COLOR_CD,COLOR_DESC 
                                FROM dbo.CP_FABRIC_ITEM CFI 
                                INNER JOIN CP_MO_HD CPHD ON CFI.GO_NO = CPHD.GO_NO AND CFI.PART_TYPE_CD=CPHD.PART_TYPE AND CPHD.MO_NO = @MO_NO 
                                LEFT JOIN dbo.GEN_FAB_PATTERN_TYPE GFPT ON GFPT.PATTERN_TYPE_CD = CFI.FAB_PATTERN 
                                INNER JOIN CP_MO_JO CMJ ON CMJ.MO_NO = CPHD.MO_NO AND CMJ.COLOR_CD = CFI.COLOR_CD 
                                ) FAB ON FAB.MO_NO = M.MO_NO AND FAB.JO_NO = CTSQ.JO_NO AND FAB.COLOR_CD = C.COLOR_CD  
                                INNER JOIN CP_SIZE_SEQ CSS ON CSS.MO_NO =M.MO_NO AND CSS.SIZE_CD = CTSQ.SIZE_CD 
                                WHERE  (ISNULL(C.REF_JO,'')='' OR C.REF_JO=CTSQ.JO_NO) AND M.MARKER_ID = @MARKER_ID 
                                GROUP BY M.MARKER_SET_NO,M.MARKER_ID,C.CUT_TABLE_NO,  SEQ.SEQ, C.COLOR_CD,FAB.COLOR_DESC,FAB.PATTERN_TYPE_DESC, 
	                                CTSQ.JO_NO, CTSQ.SIZE_CD,CSS.SEQ,CTSQ.QTY,C.PLYS 
                                ORDER BY C.CUT_TABLE_NO,SEQ.SEQ,CSS.SEQ ";
            SQL += @"    DECLARE @SQL VARCHAR(MAX) 
                                IF OBJECT_ID('TEMPDB.." + OutTable + "') IS NOT NULL  DROP TABLE " + OutTable;
            SQL += @"    CREATE TABLE " + OutTable;
            SQL += @"    (
                                    [MARKER SET NO] varchar(30)  not null DEFAULT '',";
            SQL += @"    [Marker ID] varchar(30)  COLLATE " + Language + " not null DEFAULT '',";
            SQL += @"    [Size Desc] varchar(MAX) COLLATE " + Language + " not null DEFAULT '',";
            SQL += @"    [Cut Lot No] varchar(30) COLLATE " + Language + " not null,";
            SQL += @"    [Color Code] nvarchar(30) COLLATE " + Language + " not null DEFAULT '',";
            SQL += @"    [Color Desc] varchar(50) COLLATE " + Language + " not null DEFAULT '',";
            SQL += @"    [Fabric Pattern]  varchar(50) COLLATE " + Language + " not null DEFAULT '',";
            SQL += @"    [Job Order] nvarchar(20) COLLATE " + Language + " not null DEFAULT '',";
            SQL += @"    [PLYS] INT
                                );
                                DECLARE SIZE_CUR CURSOR FOR 	
	                                SELECT MO_NO,SIZE_CD,SEQ FROM CP_SIZE_SEQ WHERE MO_NO = @MO_NO ORDER BY SEQ 
                                OPEN SIZE_CUR; 
                                DECLARE @MO_NO2 VARCHAR(30) 
                                DECLARE @SIZE_CD VARCHAR(100) 
                                DECLARE @SIZE_STRING VARCHAR(MAX) 
                                DECLARE @SIZE_SUM VARCHAR(MAX) 
                                DECLARE @SEQ INT 
	                                FETCH NEXT FROM SIZE_CUR INTO @MO_NO2,@SIZE_CD,@SEQ 
	                                WHILE @@FETCH_STATUS = 0 	
	                                BEGIN 		
	                                SET @SIZE_STRING = ISNULL(@SIZE_STRING,'')+',['+@SIZE_CD+']' 		
	                                SET @SIZE_SUM = ISNULL(@SIZE_SUM,'0')+'+['+@SIZE_CD+']' 		
	                                SET @SQL = '['+@SIZE_CD+']' 		";
            SQL += @"    EXEC ('ALTER TABLE " + OutTable + " ADD '+@SQL+' nvarchar(30) COLLATE " + Language + " not null DEFAULT '''' '); ";
            SQL += @"    FETCH NEXT FROM SIZE_CUR INTO @MO_NO2,@SIZE_CD,@SEQ 	
	                                END 
                                CLOSE SIZE_CUR; 
                                DEALLOCATE SIZE_CUR;  
                                SET @SQL = '' 
                                SET @SQL = 'INSERT INTO " + OutTable + " ([MARKER SET NO],[Marker ID],[Size Desc],[Cut Lot No],[Color Code] ,[Color Desc],[Fabric Pattern] ";
            SQL += @"    ,[Job Order],[PLYS]) SELECT DISTINCT [MARKER_SET_NO],[MARKER_ID], [Size Desc],[CUT_TABLE_NO],[COLOR_CD],[COLOR_DESC]";
            SQL += @"    ,[PATTERN_TYPE_DESC],[JO_NO],[PLYS] FROM " + temTable + " WHERE 1=1' ";
            SQL += @"    EXEC (@SQL) 
                                SET @SQL = '' 
                                DECLARE SIZE_CUR CURSOR FOR 	
	                                SELECT MO_NO,SIZE_CD,SEQ FROM CP_SIZE_SEQ WHERE MO_NO = @MO_NO ORDER BY SEQ 
                                OPEN SIZE_CUR; 
	                                FETCH NEXT FROM SIZE_CUR INTO @MO_NO2,@SIZE_CD,@SEQ 
	                                WHILE @@FETCH_STATUS = 0 	
	                                BEGIN 		";
            SQL += @"    SET @SQL = 'UPDATE " + OutTable + " SET ['+@SIZE_CD+'] = " + temTable + ".[QTY] FROM " + temTable + " WHERE  			";
            SQL += @"    convert(bigint," + temTable + ".[MARKER_SET_NO]    )     =" + OutTable + ".[MARKER SET NO]    								";
            SQL += @"    AND convert(nvarchar(30)," + temTable + ".[MARKER_ID]        )  =" + OutTable + ".[Marker ID]      								 ";
            SQL += @"    AND convert(varchar(50)," + temTable + ".[Size Desc]        )	=" + OutTable + ".[Size Desc]       								";
            SQL += @"    AND convert(varchar(50)," + temTable + ".[CUT_TABLE_NO]     )  =" + OutTable + ".[Cut Lot No]      					";
            SQL += @"    AND convert(varchar(50)," + temTable + ".[COLOR_CD]         )  =" + OutTable + ".[Color Code]      						";
            SQL += @"    AND convert(varchar(50)," + temTable + ".[COLOR_DESC]       )  =" + OutTable + ".[Color Desc]      					";
            SQL += @"    AND convert(varchar(50)," + temTable + ".[PATTERN_TYPE_DESC])  =" + OutTable + ".[Fabric Pattern]  			";
            SQL += @"    AND convert(varchar(50)," + temTable + ".[JO_NO]            )  =" + OutTable + ".[Job Order]    ' 		";
            SQL += @"    SET @SQL = @SQL + 'AND " + temTable + ".SIZE_CD = '''+@SIZE_CD +'''' 		";
            SQL += @"    EXEC (@SQL) 		
	                                FETCH NEXT FROM SIZE_CUR INTO @MO_NO2,@SIZE_CD,@SEQ 	
	                                END 
                                CLOSE SIZE_CUR; 
                                DEALLOCATE SIZE_CUR; ";
            SQL += @"    if   object_id( 'tempdb.." + SizeTable + " ')   is   not   null  drop   table   " + SizeTable;
            SQL += @"    SELECT SIZE_RATIO = [Size Desc] ,MARKER_ID =[Marker ID] INTO " + SizeTable + " FROM " + OutTable + " WHERE 1=2 ";
            SQL += @"    DECLARE @SIZE_RATIO VARCHAR(MAX) DECLARE @M_ID VARCHAR(30) 
                                DECLARE M_ID CURSOR FOR 	
	                                SELECT MARKER_ID FROM CP_MARKER_SET S 	
	                                INNER JOIN CP_MARKER M ON S.MARKER_SET_ID = M.MARKER_SET_ID 	
	                                WHERE MO_NO= @MO_NO OPEN M_ID; 
	                                FETCH NEXT FROM M_ID INTO @M_ID 
	                                WHILE @@FETCH_STATUS = 0 	
	                                BEGIN 		
	                                SET @SIZE_RATIO = NULL 		
	                                SELECT @SIZE_RATIO=ISNULL(@SIZE_RATIO+';','')+SIZE_CD+'*'+CONVERT(VARCHAR(10),RATIO)  		
	                                FROM dbo.CP_MARKER_SIZE_RATIO WHERE MARKER_ID = @M_ID 		";
            SQL += @"    INSERT INTO " + SizeTable + " VALUES(@SIZE_RATIO ,@M_ID)		 		";
            SQL += @"    FETCH NEXT FROM M_ID INTO @M_ID 	  
	                                END 
                                CLOSE M_ID; 
                                DEALLOCATE M_ID; 
                                UPDATE " + OutTable + " SET " + OutTable + ".[Size Desc] = SIZE_RATIO FROM " + SizeTable + " ";
            SQL += @"    WHERE " + OutTable + ".[Marker ID]  =" + SizeTable + ".MARKER_ID ";
            SQL += @"    SET @SQL = 'SELECT T.[JOB ORDER],[Color Code] =T.[Color Code]+''(''+T.[Color Desc]+'')'''+@SIZE_STRING+',TOTAL='+@SIZE_SUM+' FROM " + OutTable + " AS T INNER JOIN CP_MARKER_SET_SEQ SEQ ON T.[Job Order] = SEQ.JO_NO ";
            //start modification by LimML on 20150722 for SRF201502096 - sort by color first
            //SQL += @"    WHERE SEQ.MO_NO = '''+@MO_NO+''' GROUP BY T.[Color Code]+''(''+T.[Color Desc]+'')'''+@SIZE_STRING+', SEQ, [JOB ORDER] ORDER BY [COLOR CODE],SEQ'
            SQL += @"    WHERE SEQ.MO_NO = '''+@MO_NO+''' ORDER BY [MARKER SET NO], [Cut Lot No],SEQ'             
                                PRINT @SQL
                                EXEC (@SQL)";            
            return DBUtility.GetTable(SQL, "MES");
        }
       
        public static DataTable GetPtransfers(string moNo, string markerId, string bedNo)
        {
          string SQL = "";
          //</Added by: DaiJ at: 2014/04/24>
          SQL = @"SELECT X.MARKER_ID,X.CUT_TABLE_NO,X.COLOR_CD,X.COLOR_DESC,X.SIZE_CD,X.JO_NO,X.QTY_TO FROM (";
            SQL = SQL+"         select MARKER_ID=(select mm.marker_id from (select distinct aa.marker_id,aa.cut_table_no     ";
            SQL = SQL + "		from cp_cut_table aa,cp_marker bb,cp_marker_set cc   where aa.marker_id=bb.marker_id  ";
            SQL = SQL + "		and bb.marker_set_id =cc.marker_set_id and cc.mo_no='" + moNo + "'   ) mm where mm.cut_table_no=a.cut_lot_to ),    ";
            SQL = SQL + "		CUT_TABLE_NO=A.CUT_LOT_to,A.COLOR_CD,A.COLOR_DESC,A.SIZE_CD,JO_NO=A.JO_to,A.QTY_TO,B.SEQ   ";
            SQL = SQL + "		from fn_mo_jo_pullin_allocation('" + moNo + "') a  ,CP_SIZE_SEQ B  ";
            SQL = SQL + "		where a.qty_from is null  AND B.MO_NO='" + moNo + "' AND B.SIZE_CD=A.SIZE_CD  ";
            if (markerId != "")
            {
                SQL = SQL + "         and exists (select 1 from         (select distinct aa.marker_id,aa.cut_table_no           ";
                SQL = SQL + "		from cp_cut_table aa,cp_marker bb,cp_marker_set cc          ";
                SQL = SQL + "		where aa.marker_id=bb.marker_id and bb.marker_set_id =cc.marker_set_id and cc.mo_no='" + moNo + "'";
                if (bedNo != "")
                {
                    SQL = SQL + " AND CUT_TABLE_NO ='" + bedNo + "'";
                }
                SQL = SQL + " ) mm  ";
                SQL = SQL + "		where mm.marker_id='" + markerId + "' and mm.cut_table_no=a.cut_lot_to ) ";
            }
            //</Added by: DaiJ at: 2014/04/24>
            SQL += @") X LEFT JOIN PRD_JO_SEQ_LIST S ON S.JO_NO=X.JO_NO";
            SQL += @" ORDER By S.SEQ,X.JO_NO,X.CUT_TABLE_NO,X.SEQ,X.COLOR_CD,X.QTY_TO";
            //SQL = SQL + "        order by a.cut_lot_to,a.jo_to,B.SEQ,a.color_cd,a.qty_to ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetNtransfers(string moNo, string markerId, string bedNo)
        {
          string SQL = "";
          //</Added by: DaiJ at: 2014/04/24>
          SQL = @"SELECT X.MARKER_ID,X.CUT_TABLE_NO,X.COLOR_CD,X.COLOR_DESC,X.SIZE_CD,X.JO_NO,X.QTY_FROM,X.JO_TO,X.CUT_LOT_TO,X.QTY_TO FROM (";
          SQL = SQL + "     select MARKER_ID=(select mm.marker_id from (select distinct aa.marker_id,aa.cut_table_no  ";
            SQL = SQL + "        from cp_cut_table aa,cp_marker bb,cp_marker_set cc ";
            SQL = SQL + "        where aa.marker_id=bb.marker_id and bb.marker_set_id =cc.marker_set_id and cc.mo_no='" + moNo + "' ";
            SQL = SQL + "        ) mm where mm.cut_table_no=a.cut_lot_from ), ";
            SQL = SQL + "       CUT_TABLE_NO=A.CUT_LOT_FROM,A.COLOR_CD,A.COLOR_DESC,A.SIZE_CD,JO_NO=A.JO_FROM,A.QTY_FROM,A.JO_TO,A.CUT_LOT_TO,A.QTY_TO,B.SEQ   ";
            SQL = SQL + "        from fn_mo_jo_pullin_allocation('" + moNo + "') a,CP_SIZE_SEQ B ";
            SQL = SQL + "        where a.qty_from < 0 AND  B.MO_NO='" + moNo + "' AND B.SIZE_CD=A.SIZE_CD ";
            if (markerId != "")
            {
                SQL = SQL + "             and exists (select 1 from  ";
                SQL = SQL + "		     (select distinct aa.marker_id,aa.cut_table_no  ";
                SQL = SQL + "		      from cp_cut_table aa,cp_marker bb,cp_marker_set cc ";
                SQL = SQL + "		      where aa.marker_id=bb.marker_id and bb.marker_set_id =cc.marker_set_id and cc.mo_no='" + moNo + "'";
                SQL = SQL + " AND CUT_TABLE_NO ='" + bedNo + "'";
                SQL = SQL + " ) mm  ";
                SQL = SQL + "		    where mm.marker_id='" + markerId + "' and mm.cut_table_no=a.cut_lot_from ) ";

            }

            //</Added by: DaiJ at: 2014/04/24>
            SQL += @") X LEFT JOIN PRD_JO_SEQ_LIST S ON S.JO_NO=X.JO_FROM";
            SQL += @" ORDER By S.SEQ,X.JO_NO,X.CUT_TABLE_NO,X.SEQ,X.COLOR_CD,X.QTY_FROM";
            //SQL = SQL + "        order by a.cut_lot_from,a.jo_from,B.SEQ,a.color_cd ,a.qty_from ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetPtransfersByMo(string moNo)
        {
            string SQL = "";
            SQL += @"if   object_id( 'tempdb..#TEMP ')   is   not   null 
                            drop   table   #TEMP;

                            select distinct cc.mo_no, aa.marker_id,aa.cut_table_no  
                            INTO #TEMP 
                            from cp_cut_table aa WITH(NOLOCK)
                            INNER JOIN cp_marker bb WITH(NOLOCK) ON aa.marker_id=bb.marker_id
                            INNER JOIN cp_marker_set cc  WITH(NOLOCK)  ON bb.marker_set_id =cc.marker_set_id
                            where  cc.mo_no='" + moNo + "' ;";

            SQL += @"select MARKER_ID=(select mm.marker_id from #TEMP mm where mm.cut_table_no=a.cut_lot_to ),    
                            CUT_TABLE_NO=A.CUT_LOT_to,A.COLOR_CD,A.COLOR_DESC,A.SIZE_CD,JO_NO=A.JO_to,A.QTY_TO   
                            from fn_mo_jo_pullin_allocation('" + moNo + "') a";
            SQL += @" INNER JOIN CP_SIZE_SEQ B  WITH(NOLOCK)  ON B.SIZE_CD=A.SIZE_CD";
            //</Added by: DaiJ at: 2014/04/24>
            SQL+=@"   LEFT JOIN PRD_JO_SEQ_LIST X WITH(NOLOCK) ON X.JO_NO=A.JO_TO
                            where a.qty_from is null  AND B.MO_NO='" + moNo + "' ";
            //
            SQL += @" order by X.SEQ,a.jo_to,a.cut_lot_to,B.SEQ,a.color_cd,a.qty_to";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetNtransfersByMo(string moNo)
        {
            string SQL = "";
            SQL += @"if   object_id( 'tempdb..#TEMP ')   is   not   null 
                            drop   table   #TEMP;

                            select distinct cc.mo_no, aa.marker_id,aa.cut_table_no  
                            INTO #TEMP
                            from cp_cut_table aa WITH(NOLOCK)
                            INNER JOIN cp_marker bb WITH(NOLOCK) ON aa.marker_id=bb.marker_id
                            INNER JOIN cp_marker_set cc  WITH(NOLOCK)  ON bb.marker_set_id =cc.marker_set_id
                            where  cc.mo_no='" + moNo + "' ;";

            SQL += @"select MARKER_ID=(select mm.marker_id from #TEMP  mm where mm.cut_table_no=a.cut_lot_from ), 
                            CUT_TABLE_NO=A.CUT_LOT_FROM,A.COLOR_CD,A.COLOR_DESC,A.SIZE_CD,JO_NO=A.JO_FROM,A.TO_SIZE_CD,A.TO_COLOR_CD,A.TO_COLOR_DESC,A.QTY_FROM,A.JO_TO
                            ,A.CUT_LOT_TO,A.QTY_TO   
                            FROM fn_mo_jo_pullin_allocation('" + moNo + "') a";
            SQL += @" INNER JOIN CP_SIZE_SEQ B  WITH(NOLOCK)  ON B.SIZE_CD=A.SIZE_CD";
            //</Added by: DaiJ at: 2014/04/24>
            SQL+=@"   LEFT JOIN PRD_JO_SEQ_LIST X WITH(NOLOCK) ON X.JO_NO=A.JO_FROM
                            where a.qty_from < 0 AND B.MO_NO = '" + moNo + "'";
            //
            SQL += @" order by X.SEQ,a.jo_from,a.cut_lot_from,B.SEQ,a.color_cd ,a.qty_from";

//            string SQL = "";
//            SQL += @"if   object_id( 'tempdb..#TEMP ')   is   not   null 
//                            drop   table   #TEMP;
//
//                            select distinct cc.mo_no, aa.marker_id,aa.cut_table_no  
//                            INTO #TEMP
//                            from cp_cut_table aa WITH(NOLOCK)
//                            INNER JOIN cp_marker bb WITH(NOLOCK) ON aa.marker_id=bb.marker_id
//                            INNER JOIN cp_marker_set cc  WITH(NOLOCK)  ON bb.marker_set_id =cc.marker_set_id
//                            where  cc.mo_no='" + moNo + "' ;";

//            SQL += @"select MARKER_ID=(select mm.marker_id from #TEMP  mm where mm.cut_table_no=a.cut_lot_from ), 
//                            CUT_TABLE_NO=A.CUT_LOT_FROM,A.COLOR_CD,A.COLOR_DESC,A.SIZE_CD,JO_NO=A.JO_FROM,A.QTY_FROM,A.JO_TO
//                            ,A.CUT_LOT_TO,A.QTY_TO   
//                            FROM fn_mo_jo_pullin_allocation('" + moNo + "') a";
//            SQL += @" INNER JOIN CP_SIZE_SEQ B  WITH(NOLOCK)  ON B.SIZE_CD=A.SIZE_CD
//                            where a.qty_from < 0 AND B.MO_NO = '" + moNo + "'";
//            SQL += @" order by a.cut_lot_from,a.jo_from,B.SEQ,a.color_cd ,a.qty_from";
            return DBUtility.GetTable(SQL, "MES");
        }

        //outsourceControlReport
        public static DataTable GetOutsourceReportingHead(string contractNo, string serverType)
        {
            string SQL = " 		SELECT OHD.CONTRACT_NO, OHD.SUBCONTRACTOR,        ";
            SQL = SQL + "        CHN_NAME=(select system_value from GEN_SYSTEM_SETTING where factory_cd=OHD.factory_cd and system_key='OSRPT2_FTY_NAME'), ";
            SQL = SQL + "		OHD.ISSUER_DATE,MST.ADDRESS , MST.SUBCONTRACTOR_NAME,OHD.VALUE_ADD_TAX, OHD.CREATE_USER_ID AS NAME  ";
            SQL = SQL + "        FROM PRD_OUTSOURCE_CONTRACT OHD, PRD_SUBCONTRACTOR_MASTER MST   ";
            SQL = SQL + "        WHERE OHD.SUBCONTRACTOR =MST.SUBCONTRACTOR_CD AND ohd.STATUS!='CAN' ";
            SQL = SQL + "		and	OHD.CONTRACT_NO='" + contractNo + "' ";

            return DBUtility.GetTable(SQL, serverType);
        }
        public static DataTable GetQuerySubcontractControlFormOut(string contractNo)
        {
            string SQL = "         SELECT CONTRACT_NO, JONO, PROCESS, SUM(OUTQTY) AS OUTQTY, ";
            SQL = SQL + "		SUM(RECEIVEQTY) AS RECEIVEQTY, 0 AS PULLOUTQTY, 0 AS ";
            SQL = SQL + "		OUT_LOST_QTY, 0 AS INTERNAL_REDUCE, 0 AS COMPLETE_AMOUNT, 0 AS ";
            SQL = SQL + "		FABRIC_REDUCE, SUM(ADJ_AMOUNT) ADJ_AMOUNT, 0 AS ACTUAL_AMOUNT, ";
            SQL = SQL + "		ADJ_REASON, SUM(FAB_PRICE) FAB_PRICE, SUM(OUT_PRICE) AS ";
            SQL = SQL + "		OUT_PRICE, NULL AS TRX_DATE FROM (SELECT odt.fab_price, ";
            SQL = SQL + "		odt.contract_no, odt.job_order_no jono, odt.process_cd process, ";
            SQL = SQL + "		0 outqty, 0 receiveqty , 0 pulloutqty, 0 out_lost_qty, 0 AS ";
            SQL = SQL + "		internal_reduce, 0 complete_amount, 0 fabric_reduce, ";
            SQL = SQL + "		odt.adj_amount, 0 AS actual_amount, adj_reason, ";
            SQL = SQL + "		(isnull(WASH_PRICE,0)+isnull(PRINT_PRICE,0)+isnull(EMB_PRICE,0)+isnull(odt.sub_contract_price,0)) ";
            SQL = SQL + "        AS OUT_PRICE FROM prd_outsource_contract_dt odt, ";
            SQL = SQL + "        prd_outsource_contract ohd WHERE ohd.contract_no = ";
            SQL = SQL + "        odt.contract_no AND odt.contract_no = '" + contractNo + "' AND ";
            SQL = SQL + "        ohd.STATUS!='CAN' UNION ALL SELECT 0 fab_price, contract_no, ";
            SQL = SQL + "        jono, process, sum(outqty) outqty, SUM( receiveqty) AS ";
            SQL = SQL + "        receiveqty, 0 AS pulloutqty, 0 out_lost_qty, 0 AS ";
            SQL = SQL + "        internal_reduce, 0 AS complete_amount, 0 AS fabric_reduce, 0 ";
            SQL = SQL + "        adj_amount, 0 AS actual_amount, adj_reason, 0 AS OTHER_PRICE ";
            SQL = SQL + "        FROM (SELECT odt.contract_no, odt.job_order_no jono, ";
            SQL = SQL + "        odt.process_cd process, jox1.OUTPUT_QTY outqty, 0 receiveqty, 0 ";
            SQL = SQL + "        pulloutqty, 0 out_lost_qty, 0 AS internal_reduce, 0 ";
            SQL = SQL + "        complete_amount, 0 AS fabric_reduce, odt.adj_amount, 0 AS ";
            SQL = SQL + "        actual_amount, odt.adj_reason FROM prd_outsource_contract_dt ";
            SQL = SQL + "        odt, prd_jo_output_trx jox1, prd_jo_output_hd jhd1, ";
            SQL = SQL + "        prd_outsource_contract ohd WHERE ohd.contract_no = ";
            SQL = SQL + "        odt.contract_no AND odt.send_id = jox1.send_id AND ";
            //Added By ZouShiChang ON 2013.08.20 Start MES 024
            //SQL = SQL + "        ohd.send_dpartment = jox1.process_cd AND odt.job_order_no = ";
            SQL = SQL + "        ohd.send_dpartment = jox1.process_cd AND OHD.SENDER_PROCESS_TYPE=JOX1.PROCESS_TYPE AND OHD.GARMENT_TYPE=JOX1.PROCESS_GARMENT_TYPE AND odt.job_order_no = ";
            //Added By ZouShiChang ON 2013.08.20 End MES 024
            SQL = SQL + "        jox1.job_order_no AND jhd1.doc_no = jox1.doc_no AND ";
            SQL = SQL + "        odt.contract_no = '" + contractNo + "' AND ohd.STATUS!='CAN' UNION ALL SELECT ";
            SQL = SQL + "        odt.contract_no, odt.job_order_no jono, odt.process_cd process, ";
            SQL = SQL + "        0 outqty, SUM(jox2.OUTPUT_QTY) receiveqty, 0 pulloutqty, 0 ";
            SQL = SQL + "        out_lost_qty, 0 AS internal_reduce, 0 complete_amount, 0 ";
            SQL = SQL + "        fabric_reduce, 0 adj_amount, 0 actual_amount, odt.adj_reason ";
            SQL = SQL + "        FROM prd_outsource_contract_dt odt, prd_jo_output_trx jox2, ";
            SQL = SQL + "        prd_jo_output_hd jhd2, prd_outsource_contract ohd WHERE ";
            SQL = SQL + "        ohd.contract_no = odt.contract_no AND ohd.RECEIVE_POINT = ";
            //Added By ZouShiChang ON 2013.08.20 Start MES 024            
            //SQL = SQL + "        jox2.process_cd AND ohd.factory_cd = jox2.factory_cd AND ";            
            SQL = SQL + "        jox2.process_cd AND OHD.RECEIVER_PROCESS_TYPE=JOX2.PROCESS_TYPE AND OHD.GARMENT_TYPE=JOX2.PROCESS_GARMENT_TYPE AND ohd.factory_cd = jox2.factory_cd AND ";
            //Added By ZouShiChang ON 2013.08.20 End MES 024
            SQL = SQL + "        odt.SEND_ID=jox2.SEND_ID AND odt.job_order_no = ";
            SQL = SQL + "        jox2.job_order_no AND jhd2.doc_no = jox2.doc_no AND ";
            SQL = SQL + "        odt.contract_no = '" + contractNo + "' AND ohd.STATUS!='CAN' group by ";
            SQL = SQL + "        odt.contract_no, odt.job_order_no , odt.process_cd, ";
            SQL = SQL + "        odt.adj_amount, odt.adj_reason) a where 1=1 GROUP BY ";
            SQL = SQL + "        contract_no, jono, process, adj_reason) b where 1=1 GROUP BY ";
            SQL = SQL + "        jono, process,contract_no, adj_reason ";

            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetPulloutQty(string contractNo, string JoNo, string affect, string period)
        {
            string SQL = " 		SELECT isnull(sum(isnull(PLR.PULLOUT_QTY,0)),0) AS QTY FROM ";
            SQL = SQL + "		PRD_JO_DISCREPANCY_PULLOUT_TRX JOX, PRD_OUTSOURCE_CONTRACT_DT ";
            SQL = SQL + "		ODT, PRD_JO_DISCREPANCY_PULLOUT_HD JHD, PRD_OUTSOURCE_CONTRACT ";
            SQL = SQL + "		OHD, PRD_JO_PULLOUT_REASON PLR, PRD_REASON_CODE PLC WHERE ";
            SQL = SQL + "		JOX.DOC_NO=JHD.DOC_NO AND ODT.SEND_ID=JOX.SEND_ID AND ";
            SQL = SQL + "		OHD.CONTRACT_NO=ODT.CONTRACT_NO AND JOX.DOC_NO=JHD.DOC_NO AND ";
            SQL = SQL + "		ODT.JOB_ORDER_NO = JOX.JOB_ORDER_NO AND ";
            //Added By ZouShiChang ON 2013.08.20 Start MES 024    
            //SQL = SQL + "		OHD.RECEIVE_POINT=JHD.PROCESS_CD AND PLR.TRX_ID=JOX.TRX_ID AND ";
            SQL = SQL + "		OHD.RECEIVE_POINT=JHD.PROCESS_CD AND OHD.RECEIVER_PROCESS_TYPE=JHD.PROCESS_TYPE AND OHD.GARMENT_TYPE=JHD.GARMENT_TYPE AND PLR.TRX_ID=JOX.TRX_ID AND ";
            //Added By ZouShiChang ON 2013.08.20 End MES 024    
            SQL = SQL + "		PLC.REASON_CD=PLR.REASON_CD and plc.factory_cd=OHD.factory_cd ";
            switch (affect)
            {
                case "A":
                    SQL += " and PLC.qty_affection='A'";
                    break;
                case "D":
                    SQL += " and PLC.qty_affection='D'";
                    break;
            }
            SQL = SQL + "        AND JOX.JOB_ORDER_NO ='" + JoNo + "' AND OHD.CONTRACT_NO LIKE ";
            SQL = SQL + "		'%" + contractNo + "%' ";
            if (period != "")
            {
                SQL += " AND TO_CHAR(JHD.TRX_DATE,'YYYYMM')='" + period + "'";
            }
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetDefectQty(string contractNo, string JoNo)
        {
            string SQL = "   SELECT ISNULL(SUM(JOX.OUTPUT_QTY),0) AS QUANTITY FROM ";
            SQL = SQL + "  PRD_OUTSOURCE_CONTRACT_dt dt, PRD_OUTSOURCE_CONTRACT hd, ";
            SQL = SQL + "  prd_jo_output_hd jhd, prd_jo_output_trx jox,PRD_OUTSOURCE_WAREHOUSE_TYPE OWT WHERE hd.contract_no ";
            SQL = SQL + "  ='" + contractNo + "' AND hd.STATUS!='CAN' AND jox.job_order_no=dt.job_order_no and jox.job_order_no ";
            SQL = SQL + "  ='" + JoNo + "' AND jhd.doc_no=jox.doc_no AND jox.send_id = ";
            //Added By ZouShiChang ON 2013.08.20 Start MES 024
            //SQL = SQL + "  dt.send_id AND jox.process_cd = hd.RECEIVE_POINT AND ";
            SQL = SQL + "  dt.send_id  and  jox.TRX_ID=OWT.TRX_ID and  jox.DOC_NO=OWT.DOC_NO AND jox.process_cd = hd.RECEIVE_POINT AND JOX.PROCESS_TYPE=HD.RECEIVER_PROCESS_TYPE AND JOX.PROCESS_GARMENT_TYPE=HD.GARMENT_TYPE AND ";
            //Added By ZouShiChang ON 2013.08.20 END MES 024
            SQL = SQL + " jox.process_cd='FIN' and  (jhd.NEXT_PROCESS_CD='WHS' AND OWT.WAREHOUSE_TYPE='L2') ";  
            //SQL = SQL + "  jox.process_cd like 'PACK%' AND jhd.next_process_cd IN('TOSTOCKL2','TOSTOCKL2') AND ";
            //SQL = SQL + "  jox.process_cd IN('PACKK','PACKW') AND jhd.next_process_cd IN('TOSTOCKL2K','TOSTOCKL2W') AND ";
            SQL = SQL + "  and hd.contract_no = dt.contract_no having ISNULL(SUM(JOX.OUTPUT_QTY),0)>0";

            return DBUtility.GetTable(SQL, "MES");

        }


        public static DataTable GetLostQty(string contractNo, string JoNo, string peroid)
        {
            string SQL = " 		SELECT ISNULL(SUM(ISNULL(R1.PULLOUT_QTY,0)),0) LOSTQTY FROM ";
            SQL = SQL + "		PRD_OUTSOURCE_CONTRACT_DT ODT, PRD_OUTSOURCE_CONTRACT OHD, ";
            SQL = SQL + "		PRD_JO_DISCREPANCY_PULLOUT_HD JHD, ";
            SQL = SQL + "		PRD_JO_DISCREPANCY_PULLOUT_TRX JOX, PRD_JO_PULLOUT_REASON R1, ";
            SQL = SQL + "		PRD_REASON_CODE E WHERE JOX.TRX_ID = R1.TRX_ID AND JHD.DOC_NO = ";
            SQL = SQL + "		JOX.DOC_NO AND JOX.SEND_ID = ODT.SEND_ID AND ODT.CONTRACT_NO = ";
            SQL = SQL + "		OHD.CONTRACT_NO AND OHD.CONTRACT_NO = '" + contractNo + "' AND ";
            SQL = SQL + "		JOX.JOB_ORDER_NO ='" + JoNo + "' AND ";
            SQL = SQL + "		JOX.JOB_ORDER_NO=ODT.JOB_ORDER_NO AND E.REASON_CD=R1.REASON_CD ";
            if (peroid != "")
            {
                SQL += " AND TO_CHAR(JHD.TRX_DATE,'YYYYMM')='" + peroid + "'";
            }
            SQL = SQL + "        AND E.SHORT_NAME = 'OUMTL' AND JHD.PROCESS_CD = ";
            //Added By ZouShiChang ON 2013.08.20 Start MES 024
            //SQL = SQL + "		OHD.RECEIVE_POINT and OHD.FACTORY_CD=e.FACTORY_CD GROUP BY JOX.JOB_ORDER_NO, ODT.CONTRACT_NO ";
            SQL = SQL + "		OHD.RECEIVE_POINT AND JHD.PROCESS_TYPE=OHD.RECEIVER_PROCESS_TYPE AND JHD.GARMENT_TYPE=OHD.GARMENT_TYPE and OHD.FACTORY_CD=e.FACTORY_CD GROUP BY JOX.JOB_ORDER_NO, ODT.CONTRACT_NO ";
            //Added By ZouShiChang ON 2013.08.20 End MES 024

            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetCPQty(string contractNo, string JoNo, string peroid)
        {
            string SQL = " 		SELECT ISNULL(SUM(ISNULL(R1.PULLOUT_QTY,0)),0) QTY FROM ";
            SQL = SQL + "		PRD_OUTSOURCE_CONTRACT_DT ODT, PRD_OUTSOURCE_CONTRACT OHD, ";
            SQL = SQL + "		PRD_JO_DISCREPANCY_PULLOUT_HD JHD, ";
            SQL = SQL + "		PRD_JO_DISCREPANCY_PULLOUT_TRX JOX, PRD_JO_PULLOUT_REASON R1, ";
            SQL = SQL + "		PRD_REASON_CODE E WHERE JOX.TRX_ID = R1.TRX_ID AND JHD.DOC_NO = ";
            SQL = SQL + "		JOX.DOC_NO AND JOX.SEND_ID = ODT.SEND_ID  ";
            SQL = SQL + "		AND	E.REASON_CD=R1.REASON_CD AND ODT.CONTRACT_NO = OHD.CONTRACT_NO ";
            SQL = SQL + "		and e.factory_cd=ohd.factory_cd ";
            SQL = SQL + "		AND OHD.CONTRACT_NO = '" + contractNo + "' AND JOX.JOB_ORDER_NO ";
            SQL = SQL + "		='" + JoNo + "' AND JOX.JOB_ORDER_NO=ODT.JOB_ORDER_NO AND ";
            SQL = SQL + "        E.SHORT_NAME in ('FABDF','CPNDF','SEWDF','PRTDF','SHADE') ";
            if (peroid != "")
            {
                SQL += " AND TO_CHAR(JHD.TRX_DATE,'YYYYMM')='" + peroid + "'";
            }
            //Added By ZouShiChang ON 2013.08.20 Start MES 024
            //SQL = SQL + "        AND JHD.PROCESS_CD = OHD.RECEIVE_POINT GROUP BY ";
            SQL = SQL + "        AND JHD.PROCESS_CD = OHD.RECEIVE_POINT AND JHD.PROCESS_TYPE=OHD.RECEIVER_PROCESS_TYPE AND JHD.GARMENT_TYPE=OHD.GARMENT_TYPE GROUP BY ";
            //Added By ZouShiChang ON 2013.08.20 Start MES 024
            SQL = SQL + "		JOX.JOB_ORDER_NO, ODT.CONTRACT_NO ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetOutsourceReceiveDate(string JoNo, string contractNo)
        {
            string SQL = " 		SELECT ISNULL(dbo.DATE_FORMAT(MAX(JHD.TRX_DATE),'YYYY-MM-DD'),' ";
            SQL = SQL + "		') AS RECEIVE_DATE FROM PRD_JO_OUTPUT_TRX JOX INNER JOIN ";
            SQL = SQL + "		PRD_JO_OUTPUT_HD JHD ON JOX.DOC_NO=JHD.DOC_NO INNER JOIN ";
            SQL = SQL + "		PRD_OUTSOURCE_CONTRACT_DT ODT ON ODT.SEND_ID=JOX.SEND_ID INNER ";
            SQL = SQL + "		JOIN PRD_OUTSOURCE_CONTRACT OHD ON ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL = SQL + "		OHD.RECEIVE_POINT=JOX.PROCESS_CD AND ODT.CONTRACT_NO = ";
            SQL = SQL + "		OHD.RECEIVE_POINT=JOX.PROCESS_CD AND OHD.RECEIVER_PROCESS_TYPE=JOX.PROCESS_TYPE AND OHD.GARMENT_TYPE=JOX.PROCESS_GARMENT_TYPE AND ODT.CONTRACT_NO = ";            
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            SQL = SQL + "		OHD.CONTRACT_NO AND OHD.FACTORY_CD = JOX.FACTORY_CD WHERE ";
            SQL = SQL + "		JOX.JOB_ORDER_NO = '" + JoNo + "' AND OHD.CONTRACT_NO='" + contractNo + "' AND ";
            SQL = SQL + "        OHD.STATUS!='CAN' ";

            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetSubcontractControlFormIn(string contractNo)
        {
            string SQL = "         SELECT JOB_ORDER_NO, PROCESS, REASON, ";
            SQL = SQL + "        CONVERT(DECIMAL(18,3),ISNULL(SUM(INTERNAL_PRICE),0)) AS ";
            SQL = SQL + "        INTERNAL_PRICE , SUM(PLAN_ISSUE_QTY) AS PLAN_ISSUE_QTY, ";
            SQL = SQL + "        SUM(COMPLETE_QTY) AS OUTPUT_QTY, ";
            SQL = SQL + "        CONVERT(DECIMAL(18,3),ISNULL(SUM(OUT_PRICE),0)) AS OUT_PRICE, ";
            SQL = SQL + "        NULL AS TRX_DATE, EXPECT_RECEIVE_DATE, SUM(COMPLETE_QTY) AS ";
            SQL = SQL + "        COMPLETE_QTY FROM ( SELECT odt.job_order_no, odt.process_remark ";
            SQL = SQL + "        process, odt.reason, (ISNULL(odt.INTERNER_PRICE ,0) ";
            SQL = SQL + "        +ISNULL(INTERNAL_WASH_PRICE,0) +ISNULL(INTERNAL_PRINT_PRICE,0) ";
            SQL = SQL + "        +ISNULL(INTERNAL_EMB_PRICE,0)) AS internal_price, ";
            SQL = SQL + "        odt.plan_issue_qty, 0 output_qty, (ISNULL(SUB_CONTRACT_PRICE,0) ";
            SQL = SQL + "        +ISNULL(WASH_PRICE,0) +ISNULL(EMB_PRICE,0) ";
            SQL = SQL + "        +ISNULL(PRINT_PRICE,0)) as OUT_PRICE, odt.EXPECT_RECEIVE_DATE, 0 ";
            SQL = SQL + "        COMPLETE_QTY FROM prd_outsource_contract_dt odt, ";
            SQL = SQL + "        prd_outsource_contract ohd WHERE ohd.contract_no=odt.contract_no ";
            SQL = SQL + "        AND odt.contract_no = '" + contractNo + "' AND ohd.STATUS!='CAN' UNION ALL ";
            SQL = SQL + "        SELECT odt.job_order_no, odt.process_remark as process, ";
            SQL = SQL + "        odt.reason, 0 AS internal_price, 0 plan_issue_qty, ";
            SQL = SQL + "        jox.output_qty, 0 OUT_PRICE, odt.EXPECT_RECEIVE_DATE, ";
            SQL = SQL + "        jox.COMPLETE_QTY FROM prd_outsource_contract_dt odt, ";
            SQL = SQL + "        prd_outsource_contract ohd, prd_jo_output_trx jox, ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL = SQL + "        prd_jo_output_hd jhd WHERE ohd.RECEIVE_POINT = jox.process_cd ";
            SQL = SQL + "        prd_jo_output_hd jhd WHERE ohd.RECEIVE_POINT = jox.process_cd AND OHD.RECEIVER_PROCESS_TYPE=JOX.PROCESS_TYPE AND OHD.GARMENT_TYPE=JOX.PROCESS_GARMENT_TYPE ";
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            SQL = SQL + "        AND ohd.factory_cd=jhd.factory_cd AND odt.send_id = jox.send_id ";
            SQL = SQL + "        AND odt.job_order_no=jox.job_order_no AND jox.doc_no = ";
            SQL = SQL + "        jhd.doc_no AND ohd.contract_no=odt.contract_no AND ";
            SQL = SQL + "        odt.contract_no = '" + contractNo + "' AND ohd.STATUS!='CAN' ) a GROUP BY ";
            SQL = SQL + "        job_order_no, process, reason,EXPECT_RECEIVE_DATE ";

            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetOutsourceSendDate(string joNo, string contractNo)
        {
            string SQL = " 		SELECT ISNULL(dbo.DATE_FORMAT(MIN(JHD.TRX_DATE),'YYYY-MM-DD'),' ";
            SQL = SQL + "		') AS SEND_DATE FROM PRD_JO_OUTPUT_TRX JOX INNER JOIN ";
            SQL = SQL + "		PRD_JO_OUTPUT_HD JHD ON JOX.DOC_NO=JHD.DOC_NO INNER JOIN ";
            SQL = SQL + "		PRD_OUTSOURCE_CONTRACT_DT ODT ON ODT.SEND_ID=JOX.SEND_ID INNER ";
            SQL = SQL + "		JOIN PRD_OUTSOURCE_CONTRACT OHD ON ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL = SQL + "		OHD.SEND_DPARTMENT=JHD.PROCESS_CD AND ODT.CONTRACT_NO = ";
            SQL = SQL + "		OHD.SEND_DPARTMENT=JOX.PROCESS_CD AND OHD.SENDER_PROCESS_TYPE=JOX.PROCESS_TYPE AND OHD.GARMENT_TYPE=JOX.PROCESS_GARMENT_TYPE AND ODT.CONTRACT_NO = ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            SQL = SQL + "		OHD.CONTRACT_NO AND OHD.FACTORY_CD = JOX.FACTORY_CD WHERE ";
            SQL = SQL + "		JOX.JOB_ORDER_NO = '" + joNo + "' AND OHD.CONTRACT_NO='" + contractNo + "' AND ";
            SQL = SQL + "        OHD.STATUS!='CAN' ";

            return DBUtility.GetTable(SQL, "MES");
        }

        //outsourceCheckingReport
        public static DataTable GetOsChecking(string contractNo, string subcontractorCd, string startDate, string endDate)
        {
            string SQL = "   SELECT JONO, PROCESS, CONTRACT_NO, SUM(OUTQTY) AS OUTQTY, ";
            SQL = SQL + "  SUM(RECEIVEQTY) AS RECEIVEQTY, 0 AS PULLOUTQTY, isnull(SUM(FAB_PRICE),0) ";
            SQL = SQL + "  AS FAB_PRICE, 0 AS OUT_LOST_QTY, 0 AS INTERNAL_REDUCE, 0 AS ";
            SQL = SQL + "  COMPLETE_AMOUNT, 0 AS FABRIC_REDUCE, SUM(ISNULL(ADJ_AMOUNT,0)) ";
            SQL = SQL + "  ADJ_AMOUNT, 0 AS ACTUAL_AMOUNT, SUM(OUT_PRICE) AS OUT_PRICE, ";
            SQL = SQL + "  ADJ_REASON FROM (SELECT odt.job_order_no jono, ";
            SQL = SQL + "  odt.process_remark process, odt.contract_no, 0 outqty, 0 ";
            SQL = SQL + "  receiveqty , 0 pulloutqty, odt.Fab_price, 0 out_lost_qty, 0 AS ";
            SQL = SQL + "        internal_reduce, 0 complete_amount, 0 fabric_reduce, ";
            SQL = SQL + "        odt.adj_amount, 0 AS actual_amount, ";
            SQL = SQL + "        (ISNULL(WASH_PRICE,0)+ISNULL(PRINT_PRICE,0)+ISNULL(EMB_PRICE,0)+ISNULL(sub_contract_price,0)) ";
            SQL = SQL + "        AS OUT_PRICE, adj_reason FROM PRD_OUTSOURCE_CONTRACT_dt odt, ";
            SQL = SQL + "        PRD_OUTSOURCE_CONTRACT ohd WHERE ohd.contract_no = ";
            SQL = SQL + "        odt.contract_no AND odt.contract_no = '" + contractNo + "' AND ";
            SQL = SQL + "        ohd.STATUS!='CAN' ";
            if (subcontractorCd != "")
            {
                SQL += " and ohd.SUBCONTRACTOR='" + subcontractorCd + "'";
            }
            if (startDate != "")
            {
                SQL += " and ohd.ISSUER_DATE >=dbo.DATE_FORMAT('" + startDate + "','mm/dd/yyyy')";
            }
            if (endDate != "")
            {
                SQL += " and ohd.ISSUER_DATE &lt;=dbo.DATE_FORMAT('" + endDate + "','mm/dd/yyyy')";
            }
            SQL = SQL + "        UNION ALL SELECT jono, process, contract_no, SUM(outqty) outqty, ";
            SQL = SQL + "  SUM( receiveqty) AS receiveqty, 0 AS pulloutqty, 0 FAB_PRICE, 0 ";
            SQL = SQL + "  out_lost_qty, 0 AS internal_reduce, 0 AS complete_amount, 0 AS ";
            SQL = SQL + "  fabric_reduce, 0 adj_amount, 0 AS actual_amount, 0 AS OUT_PRICE, ";
            SQL = SQL + "  adj_reason FROM (SELECT odt.job_order_no jono, ";
            SQL = SQL + "  odt.process_remark process, odt.contract_no, jox1.OUTPUT_QTY ";
            SQL = SQL + "  outqty, 0 receiveqty, 0 pulloutqty, 0 out_lost_qty, 0 AS ";
            SQL = SQL + "  internal_reduce, 0 complete_amount, 0 AS fabric_reduce, 0 ";
            SQL = SQL + "  adj_amount, 0 AS actual_amount, odt.adj_reason FROM ";
            SQL = SQL + "  PRD_OUTSOURCE_CONTRACT_dt odt, prd_jo_output_trx jox1, ";
            SQL = SQL + "  prd_jo_output_hd jhd1, PRD_OUTSOURCE_CONTRACT ohd WHERE ";
            SQL = SQL + "  ohd.contract_no = odt.contract_no AND odt.send_id = jox1.send_id ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL = SQL + "  AND ohd.send_dpartment = jox1.process_cd AND odt.job_order_no = ";
            SQL = SQL + "  AND ohd.send_dpartment = jox1.process_cd AND OHD.SENDER_PROCESS_TYPE=JOX1.PROCESS_TYPE AND OHD.GARMENT_TYPE=JOX1.PROCESS_GARMENT_TYPE AND odt.job_order_no = ";
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            SQL = SQL + "  jox1.job_order_no AND jhd1.doc_no = jox1.doc_no AND ";
            SQL = SQL + "  odt.contract_no = '" + contractNo + "' AND ohd.STATUS!='CAN' ";
            if (subcontractorCd != "")
            {
                SQL += " and ohd.SUBCONTRACTOR='" + subcontractorCd + "'";
            }
            if (startDate != "")
            {
                SQL += " and ohd.ISSUER_DATE >=dbo.DATE_FORMAT('" + startDate + "','mm/dd/yyyy')";
            }
            if (endDate != "")
            {
                SQL += " and ohd.ISSUER_DATE &lt;=dbo.DATE_FORMAT('" + endDate + "','mm/dd/yyyy')";
            }
           
            
            SQL = SQL + "        UNION ALL SELECT odt.job_order_no jono, odt.process_remark ";
            SQL = SQL + "  process, odt.contract_no, 0 outqty, sum(jox2.OUTPUT_QTY) ";
            SQL = SQL + "  receiveqty, 0 pulloutqty, 0 out_lost_qty, 0 AS internal_reduce, ";
            SQL = SQL + "  0 complete_amount, 0 fabric_reduce, 0 adj_amount, 0 AS ";
            SQL = SQL + "  actual_amount, odt.adj_reason FROM PRD_OUTSOURCE_CONTRACT_dt ";
            SQL = SQL + "  odt, prd_jo_output_trx jox2, prd_jo_output_hd jhd2, ";
            SQL = SQL + " PRD_OUTSOURCE_WAREHOUSE_TYPE OWT, "; //Added by ZouShiChang On 2013.10.15 Start MES024
            SQL = SQL + "  PRD_OUTSOURCE_CONTRACT ohd WHERE ohd.contract_no = ";
            //Added By ZouShiChang ON 2013.08.21 Start MES 024
            //SQL = SQL + "  odt.contract_no AND ohd.RECEIVE_POINT = jox2.process_cd AND ";
            SQL = SQL + "  odt.contract_no AND ohd.RECEIVE_POINT = jox2.process_cd AND OHD.RECEIVER_PROCESS_TYPE=JOX2.PROCESS_TYPE and OHD.GARMENT_TYPE=JOX2.PROCESS_GARMENT_TYPE AND ";
            //Added By ZouShiChang ON 2013.08.21 End MES 024
            SQL = SQL + "  ohd.factory_cd = jox2.factory_cd and odt.SEND_ID=jox2.SEND_ID and jox2.trx_id=owt.trx_id and jox2.doc_no=owt.doc_no ";
            SQL = SQL + " /*AND jhd2.NEXT_PROCESS_CD<>'WHS'*/ AND OWT.WAREHOUSE_TYPE<>'L2' AND odt.job_order_no= ";
            //SQL = SQL + "  AND jhd2.next_process_cd not in('TOSTOCKL2','TOSTOCKL2') AND odt.job_order_no = ";
            SQL = SQL + "  jox2.job_order_no AND jhd2.doc_no = jox2.doc_no AND ";
            SQL = SQL + "  odt.contract_no = '" + contractNo + "' AND ohd.STATUS!='CAN' ";
            
            /*
            SQL = SQL + @" UNION ALL 
                           SELECT    ODT.JOB_ORDER_NO AS JO_NO ,
                                ODT.PROCESS_REMARK AS PROCESS ,
                                ODT.CONTRACT_NO ,
                                0 outqty ,
                                SUM(JOX.OUTPUT_QTY) receiveqty ,
                                0 pulloutqty ,
                                0 out_lost_qty ,
                                0 AS internal_reduce ,
                                0 complete_amount ,
                                0 fabric_reduce ,
                                0 adj_amount ,
                                0 AS actual_amount ,
                                odt.adj_reason
                      FROM      PRD_OUTSOURCE_CONTRACT AS OHD
                                INNER JOIN PRD_OUTSOURCE_CONTRACT_DT AS ODT ON OHD.CONTRACT_NO = ODT.CONTRACT_NO
                                INNER JOIN PRD_JO_OUTPUT_TRX AS JOX ON ODT.JOB_ORDER_NO = JOX.JOB_ORDER_NO
                                                              AND ODT.SEND_ID = jox.SEND_ID
                                                              AND OHD.RECEIVE_POINT = JOX.PROCESS_CD
                                                              AND OHD.RECEIVER_PROCESS_TYPE = JOX.PROCESS_TYPE
                                                              AND OHD.GARMENT_TYPE = JOX.PROCESS_GARMENT_TYPE
                                                              AND OHD.FACTORY_CD = JOX.FACTORY_CD ";
             
             SQL=SQL+ " WHERE 1=1";
              

             SQL = SQL + "  and odt.contract_no = '" + contractNo + "' AND ohd.STATUS!='CAN' ";
            */
            if (subcontractorCd != "")
            {
                SQL += " and ohd.SUBCONTRACTOR='" + subcontractorCd + "'";
            }
            if (startDate != "")
            {
                SQL += " and ohd.ISSUER_DATE >=dbo.DATE_FORMAT('" + startDate + "','mm/dd/yyyy')";
            }
            if (endDate != "")
            {
                SQL += " and ohd.ISSUER_DATE &lt;=dbo.DATE_FORMAT('" + endDate + "','mm/dd/yyyy')";
            }
            SQL = SQL + "        group by odt.job_order_no , odt.contract_no, odt.process_remark, ";
            SQL = SQL + "  odt.adj_reason ) MM GROUP BY jono, process, contract_no, ";
            SQL = SQL + "  adj_reason) CC GROUP BY jono, process, adj_reason,contract_no ";
            return DBUtility.GetTable(SQL, "MES");
        }




        //mesSearchJoNumber
        public static DataTable GetCutPlanSearchJoNumber(string markerId)
        {
            string SQL = " SELECT * ";
            SQL = SQL + "  FROM cp_marker ";
            SQL = SQL + " WHERE marker_set_id IN (SELECT marker_set_id ";
            SQL = SQL + "                           FROM cp_marker_set ";
            SQL = SQL + "                          WHERE mo_no = '" + markerId + "') ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetAlertFinishProcess(string date, string times)
        {
            string SQL = " SELECT CUSTOMER as Customer,JOB_ORDER_NO AS 'Job Order No',CUT_QTY as 'Cut Qty',IN_QTY as 'Finished Qty',PRE_PROCESS as Process,DIS_QTY as 'Different Qty',CONVERT(VARCHAR(10),BUYER_PO_DEL_DATE,111) as 'Buyer PO Del Date' ";
            SQL = SQL + "  FROM Mes_Alert_Finish_Process ";
            if (times == "DAILY" || times == "daily")
            {
                SQL = SQL + " WHERE date between '" + date + " 00:00:00' and '" + date + " 12:00:00'";
            }
            else if (times == "NIGHT" || times == "night")
            {
                SQL = SQL + " WHERE date between '" + date + " 12:00:00' and '" + date + " 23:59:59'";
            }
            return DBUtility.GetTable(SQL, "MES");
        }

        //ProWipQuery
        /// <summary>
        /// 取出WareHouse及ShipQty
        /// </summary>
        /// <param name="trxFromDate">开始生产日期</param>
        /// <param name="trxToDate">结束生产日期</param>
        /// <param name="strJoList">JobNo列表</param>
        /// <param name="bpoFromDate">开始货期</param>
        /// <param name="bpoToDate">结束货期</param>
        /// <param name="strGo">大货单号</param>
        /// <param name="strGarmentType">成衣类型</param>
        /// <param name="strDownFlag"></param>
        /// <param name="strFactoryCd">工厂</param>
        /// <param name="summaryType">汇总类型</param>
        /// <param name="isIncludeOutSource">是否包含外发</param>
        /// <param name="reportType">报表类型</param>
        /// <returns></returns>
        public static DataTable GetShipQTYAndWarehouseQTY(string trxFromDate, string trxToDate, string strJoList, string bpoFromDate, string bpoToDate, string strGo, string strGarmentType, string strDownFlag, string strFactoryCd, string strCustStyleNo, string strCustomer, string strProductCategory, string strFromProdCompDate, string strToProdCompDate, string strJo, string summaryType, bool isIncludeOutSource, string reportType)       
        {
            string SQL = "";
            //DbConnection conn0 = MESComment.DBUtility.GetConnectionString("INV");
            //OleDbConnection conn = new OleDbConnection(conn0.ConnectionString + "Provider=MSDAORA");

            //DBUtility.ExecuteNonQuery(conn, CommandType.StoredProcedure, "inventory.INV_COMM_FUNCTION.SET_TRANS_VIEW_CONDITION", new OleDbParameter[] { new OleDbParameter("@v_From_Date", trxFromDate), new OleDbParameter("@v_To_Date", trxToDate), new OleDbParameter("@v_JO_List", strJoList), new OleDbParameter("@v_Fm_BPO_Date", bpoFromDate), new OleDbParameter("@v_To_BPO_Date", bpoToDate), new OleDbParameter("@v_GO", strGo), new OleDbParameter("@v_Gartment_Type", strGarmentType), new OleDbParameter("@v_DOWNLOAD_FLAG", strDownFlag), new OleDbParameter("@v_Factory", strFactoryCd), new OleDbParameter("@v_cust_style_No", strCustStyleNo), new OleDbParameter("@v_customer", strCustomer), new OleDbParameter("@v_Product_category", strProductCategory), new OleDbParameter("@v_From_prod_comp_Date", strFromProdCompDate), new OleDbParameter("@v_To_prod_comp_Date", strToProdCompDate), new OleDbParameter("@v_JO_NO", strJo) });
            //OleDbConnection conn=new OleDbConnection(MESComment.DBUtility.ExecuteNonQuery("INV"));
            DBUtility.ExecuteNonQuery("INV", CommandType.StoredProcedure, "inventory.INV_COMM_FUNCTION.SET_TRANS_VIEW_CONDITION",
           new System.Collections.Generic.Dictionary<string, string>{{ "v_From_Date",trxFromDate},{ "v_To_Date",trxToDate},{ "v_JO_List",strJoList},{ "v_From_BPO_Date",bpoFromDate},{ "v_To_BPO_Date",bpoToDate},{ "v_GO",strGo},
            { "v_Gartment_Type",strGarmentType},{ "v_DOWNLOAD_FLAG",strDownFlag},{ "v_Factory",strFactoryCd},{ "v_cust_style_No",strCustStyleNo},{ "v_customer",strCustomer},
            { "v_Product_category",strProductCategory},{ "v_From_prod_comp_Date",strFromProdCompDate},{ "v_To_prod_comp_Date",strToProdCompDate},{ "v_JO_NO",strJo}});
            switch (summaryType)
            {
                case "SummaryByJo":
                    SQL = "select job_order_no,sum(ab_out_qty) ship_qty,sum(in_qty) warehouse from inventory.INV_TRANS_FOR_MES_V v where 1=1";
                    if (!isIncludeOutSource)
                    {
                        switch (reportType)
                        {
                            case "Produce":
                                switch (strFactoryCd)
                                {
                                    //Changed JO_NO to PO_NO by Jin Song 2015-01-20 (Bug Fix)
                                    case "GEG": 
                                        SQL += " and exists(select 1 from po_hd a,sc_hd b where a.sc_no=b.sc_no and a.PO_NO=v.job_order_no";
                                        SQL += " and case v.wash_type_cd when 'WASH' then case b.sam_group_cd when 'outsource' then 'standard' end when 'WSH' then case b.sam_group_cd when 'outsource' then 'standard' end else b.sam_group_cd end <>'outsource')";
                                        break;
                                    default:
                                        SQL += " and exists(select 1 from po_hd a,sc_hd b where a.sc_no=b.sc_no and a.PO_NO=v.job_order_no";
                                        SQL += " and b.sam_group_cd <>'outsource')";
                                        break;
                                }
                                break;
                            case "Accept":
                                SQL += " and exists(select 1 from po_hd a,sc_hd b where a.sc_no=b.sc_no and a.PO_NO=v.job_order_no";
                                SQL += " and b.sam_group_cd <>'outsource')";
                                break;
                        }
                    }
                    SQL += " group by job_order_no";
                    break;
                case "SummaryByTrxDate":
                    SQL = "select trans_date,sum(ab_out_qty) ship_qty,sum(in_qty) warehouse from inventory.INV_TRANS_FOR_MES_V v where 1=1 ";
                    if (!isIncludeOutSource)
                    {
                        switch (reportType)
                        {
                            case "Produce":
                                switch (strFactoryCd)
                                {
                                    case "GEG":
                                        SQL += " and exists(select 1 from po_hd a,sc_hd b where a.sc_no=b.sc_no and a.PO_NO=v.job_order_no";
                                        SQL += " and case v.wash_type_cd when 'WASH' then case b.sam_group_cd when 'outsource' then 'standard' end when 'WSH' then case b.sam_group_cd when 'outsource' then 'standard' end else b.sam_group_cd end <>'outsource')";
                                        break;
                                    default:
                                        SQL += " and exists(select 1 from po_hd a,sc_hd b where a.sc_no=b.sc_no and a.PO_NO=v.job_order_no";
                                        SQL += " and b.sam_group_cd <>'outsource')";
                                        break;
                                }
                                break;
                            case "Accept":
                                SQL += " and exists(select 1 from po_hd a,sc_hd b where a.sc_no=b.sc_no and a.PO_NO=v.job_order_no";
                                SQL += " and b.sam_group_cd <>'outsource')";
                                break;
                        }
                    }
                    SQL += " group by trans_date order by trans_date";
                    break;
                case "SummaryByBpoDate":
                    SQL = "select job_order_no,trans_date,sum(in_qty) warehouse from inventory.INV_TRANS_FOR_MES_V v where 1=1";
                    if (!isIncludeOutSource)
                    {
                        SQL += " and exists(select 1 from po_hd a,sc_hd b where a.sc_no=b.sc_no and a.PO_NO=v.job_order_no";
                        SQL += " and b.sam_group_cd <>'outsource')";
                    }
                    SQL += " group by job_order_no,trans_date";
                    break;
                default:
                    SQL = "select job_order_no,color_cd,size_cd,trans_date,ab_out_qty ship_qty,(in_qty) warehouse from inventory.INV_TRANS_FOR_MES_V";
                    break;
            }
            return DBUtility.GetTable(SQL, "INV");
        }

        
    }
}