using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web.UI.WebControls;


namespace MESComment
{
    /// <summary>
    ///IEContractReport 的摘要说明
    /// </summary>
    public class IEContractReport
    {
        public static DataTable GetIEContractReport_JOList_data(string FactoryCD, string strContractNO, string subcontractor, string TranDateBegin, string TranDateEnd)
        {
            string SQL = "";
            SQL += " SELECT DISTINCT JOB_ORDER_NO FROM prd_outsource_contract A,prd_outsource_contract_dt B";
            SQL += " WHERE A.CONTRACT_NO = B.CONTRACT_NO ";
            if (!subcontractor.Equals("") && !subcontractor.Equals("-1"))
            {
                SQL += " AND subcontractor = '" + subcontractor + "'";
            }
            SQL += "  AND FACTORY_CD = '" + FactoryCD + "'";
            if (!strContractNO.Equals(""))
            {
                SQL += "  AND A.CONTRACT_NO = '" + strContractNO + "'";
            }
            if (!TranDateBegin.Equals("") && !TranDateEnd.Equals(""))
            {
                SQL += "  AND ISSUER_DATE >=DATEADD(year,-1,'" + TranDateBegin + "')";
                SQL += "  AND ISSUER_DATE <='" + TranDateEnd + "'";
            }
            DataTable dt = DBUtility.GetTable(SQL, "MES");
            return dt;
        }

        public static DataTable GetIEContractReport_WIP_data(string FactoryCD, string ContractNO, string subcontractor, string TranDateBegin, string TranDateEnd)
        {
            string strNO1Where = ""; string strNO2Where = ""; string strDateWhere = ""; string strDate2Where = ""; string subcontractorWhere = "";
            string JOListWhere = "";
            if (ContractNO != string.Empty)
            {
                strNO1Where = " and a.contract_no in( '" + ContractNO + "') ";
                strNO2Where = " and d.contract_no in( '" + ContractNO + "') ";
            }
            else
            {
                JOListWhere = " AND EXISTS(";
                JOListWhere += " SELECT 1 FROM prd_outsource_contract A,prd_outsource_contract_dt B";
                JOListWhere += " WHERE A.CONTRACT_NO = B.CONTRACT_NO";
                if (!subcontractor.Equals(""))
                {
                    JOListWhere += "  AND subcontractor = '" + subcontractor + "'";
                }
                JOListWhere += "  AND FACTORY_CD = '" + FactoryCD + "'";
                if (!TranDateBegin.Equals("") && !TranDateEnd.Equals(""))
                {
                    JOListWhere += "  AND ISSUER_DATE >=DATEADD(year,-1,'" + TranDateBegin + "')";
                    JOListWhere += "  AND ISSUER_DATE <='" + TranDateEnd + "'";
                }
                JOListWhere += "  AND JOB_ORDER_NO = M.JOB_ORDER_NO) ";

            }
            if (TranDateBegin != string.Empty)
            {
                strDateWhere = " and ((a.trx_date >=cast('" + TranDateBegin + "' as datetime)  AND a.trx_date <=cast('" + TranDateEnd + "' as datetime)) OR  a.trx_date IS NULL)  ";
                strDate2Where = " and ((trx_date >=cast('" + TranDateBegin + "' as datetime)  AND trx_date <=cast('" + TranDateEnd + "' as datetime)) OR  trx_date IS NULL) ";
            }
            if (subcontractor != string.Empty)
                subcontractorWhere = " and c.subcontractor_cd = '" + subcontractor + "'  ";

            StringBuilder sqlbu = new StringBuilder();
            sqlbu.AppendFormat("select SUBCONTRACTOR_NAME,CONVERT(varchar(100), N.TRX_DATE, 23) as Trans_date , M.CONTRACT_NO, M.JOB_ORDER_NO,ORDER_QTY,GOOD_NAME,plan_issue_qty,PROCESS_REMARK,SUB_CONTRACT_PRICE,SEND_DPARTMENT,RECEIVE_POINT, " +
                                   "         RECEIPT_DEPARTMENT, isnull(N.output_qty,0) as OUTPUT_QTY,isnull(N.RECEIPT_QTY,0) as RECEIPT_QTY, isnull(N.PULLOUT_QTY,0) as PULLOUT_QTY, STYLE_CHN_DESC  " +
                                   "     from " +
                                   "         (select c.subcontractor_name,a.contract_no,	b.job_order_no,	B.GOOD_NAME ,sum(b.plan_issue_qty) as plan_issue_qty,(select sum(qty) from jo_dt where jo_no=b.job_order_no) AS order_qty,b.process_remark, " +
                                   "            ISNULL(b.SUB_CONTRACT_PRICE,0) AS SUB_CONTRACT_PRICE,a.SEND_Dpartment,	a.receive_point, " +
                                   "             a.Receipt_Department,	b.send_id,	g.style_chn_desc " +
                                   "         from prd_outsource_contract a,prd_outsource_contract_dt b,prd_subcontractor_master c,Jo_hd d,sc_hd g " +
                                   "         where " +
                                   "             c.subcontractor_cd=a.subcontractor AND a.contract_no = b.contract_no  AND b.job_order_no = d.JO_NO AND d.sc_no = g.sc_no " +
                                   "             and d.sc_no=g.sc_no {0} {1}" +
                                   "             group by c.subcontractor_name, a.contract_no, b.job_order_no, g.style_chn_desc, b.process_remark, B.GOOD_NAME, " +
                                   "             a.SEND_Dpartment, a.receive_point, a.Receipt_Department, b.send_id,b.sub_contract_price) M " +
                                   "         left join " +
                                   "         ( select tot.trx_date,tot.JOB_ORDER_NO,tot.contract_no,tot.send_id,sum(isnull(tot.output_qty,0)) as OUTPUT_QTY, " +
                                   "             sum(isnull(tot.receipt_qty,0))as RECEIPT_QTY,sum(isnull(tot.PULLOUT_QTY,0)) as PULLOUT_QTY " +
                                   "           from( " +
                                   "                 select a.trx_date,b.Job_order_no,b.contract_no,b.send_id,output_QTY=(case when b.Process_cd=d.SEND_DPARTMENT and b.PROCESS_GARMENT_TYPE=D.GARMENT_TYPE and b.process_type=d.SENDER_PROCESS_TYPE then sum(isnull(b.output_qty,0)) else 0 end),  " +
                                   "                     RECEIPT_QTY=(case when  b.PROCESS_cd=d.receive_point and b.PROCESS_GARMENT_TYPE=D.GARMENT_TYPE and b.PROCESS_TYPE=d.RECEIVER_PROCESS_TYPE then sum(isnull(b.output_qty,0)) else 0 end),0 as PULLOUT_QTY  " +
                                   "                  from prd_jo_output_hd a join prd_jo_output_trx b on a.doc_no=b.doc_no join prd_outsource_contract d on d.factory_cd='" + FactoryCD + "' " +
                                   "                       and a.factory_cd=d.factory_cd and ((b.Process_cd=d.SEND_DPARTMENT and b.PROCESS_GARMENT_TYPE=D.GARMENT_TYPE and b.process_type=d.SENDER_PROCESS_TYPE ) or (b.PROCESS_cd=d.receive_point and b.PROCESS_GARMENT_TYPE=D.GARMENT_TYPE and b.PROCESS_TYPE=d.RECEIVER_PROCESS_TYPE )) " +
                                   "                       join prd_outsource_contract_dt e on  b.job_order_no=e.job_order_no " +
                                   "                       and b.contract_no=e.contract_no and d.contract_no=e.contract_no and b.send_id=e.send_id and b.output_qty<>0 " +
                                   "                  WHERE a.factory_cd='" + FactoryCD + "'  {3} " +
                                   "             group by a.trx_date,b.job_order_no,b.contract_no,b.send_id,b.Process_cd,B.PROCESS_TYPE,B.PROCESS_GARMENT_TYPE,d.SEND_DPARTMENT,d.GARMENT_TYPE,d.SENDER_PROCESS_TYPE,d.RECEIVER_PROCESS_TYPE,a.Next_PROCESS_cd,d.receive_point  " +
                                   "             union " +
                                   "             select a.trx_date,b.job_order_no,b.contract_no,b.send_id,0 as output_qty,0 as RECEIPT_QTY, " +
                                   "                     SUM(ISNULL(c.PULLOUT_QTY,0)) AS PULLOUT_QTY  " +
                                   "             from PRD_JO_DISCREPANCY_PULLOUT_HD a  " +
                                   "                     JOIN PRD_JO_DISCREPANCY_PULLOUT_TRX b ON A.DOC_NO=B.DOC_NO AND A.FACTORY_CD=B.FACTORY_CD AND A.TRX_DATE=B.TRX_DATE  " +
                                   "                     JOIN PRD_JO_PULLOUT_REASON c ON c.TRX_ID=B.TRX_ID AND c.FACTORY_CD=A.FACTORY_CD and c.REASON_CD in('GEG025','GEG026') and c.PULLOUT_QTY<>0 " +
                                   "                     join prd_outsource_contract d on d.factory_cd='" + FactoryCD + "' and a.factory_cd=d.factory_cd and a.PROCESS_cd=d.receive_point and a.PROCESS_TYPE=d.RECEIVER_PROCESS_TYPE and A.GARMENT_TYPE=D.GARMENT_TYPE " +
                                   "                     join prd_outsource_contract_dt e on  b.job_order_no=e.job_order_no and b.contract_no=e.contract_no and d.contract_no=e.contract_no and b.send_id=e.send_id " +
                                   "             WHERE d.factory_cd='" + FactoryCD + "' {3} " +
                                   "             GROUP BY  a.trx_date,B.JOB_ORDER_NO,b.contract_no,b.send_id  " +
                //Added By ZouShiChang On 2014.04.18 Start
                                   @"   UNION
                                        SELECT  Trans_date AS trx_date ,
                                        CONTRACT_NO AS contract_no ,
                                        JOB_ORDER_NO ,
                                        SEND_ID ,
                                        SUM(sendQty) AS OUTPUT_QTY ,
                                        SUM(recQty) AS RECEIPT_QTY ,
                                        SUM(disqty) AS PULLOUT_QTY
                                FROM    ( SELECT    CONVERT(NVARCHAR(100), a.CONFIRM_DATE, 23) AS Trans_date ,
                                                    a.REF_CONTRACT_NO AS CONTRACT_NO ,
                                                    b.JOB_ORDER_NO ,
                                                    B.SEND_ID ,
                                                    SUM(b.QTY) AS sendQty ,
                                                    0 AS recQty ,
                                                    0 AS disqty
                                          FROM      dbo.OAS_SENDING_RECEIVING_HD a
                                                    INNER JOIN dbo.OAS_SENDING_RECEIVING_DT b ON b.DOC_NO = a.DOC_NO
                                          WHERE     a.TRX_TYPE = 'S'
                                                    AND IS_REAL_SENDING_RECEIVING = 'Y'
                                                    AND STATUS = 'C'
                                                    AND FACTORY_CD='" + FactoryCD + @"'
                                          GROUP BY  a.REF_CONTRACT_NO ,
                                                    b.JOB_ORDER_NO ,
                                                    CONVERT(NVARCHAR(100), a.CONFIRM_DATE, 23) ,
                                                    B.SEND_ID
                                          UNION ALL
                                          SELECT    CONVERT(NVARCHAR(100), a.CONFIRM_DATE, 23) AS Trans_date ,
                                                    a.REF_CONTRACT_NO AS CONTRACT_NO ,
                                                    b.JOB_ORDER_NO ,
                                                    B.SEND_ID ,
                                                    0 AS sendQty ,
                                                    SUM(b.QTY) AS recQty ,
                                                    0 AS disqty
                                          FROM      dbo.OAS_SENDING_RECEIVING_HD a
                                                    INNER JOIN dbo.OAS_SENDING_RECEIVING_DT b ON b.DOC_NO = a.DOC_NO
                                          WHERE     a.TRX_TYPE = 'R'
                                                    AND IS_REAL_SENDING_RECEIVING = 'Y'
                                                    AND STATUS = 'C'
                                                    AND FACTORY_CD='" + FactoryCD + @"'
                                          GROUP BY  a.REF_CONTRACT_NO ,
                                                    b.JOB_ORDER_NO ,
                                                    CONVERT(NVARCHAR(100), a.CONFIRM_DATE, 23) ,
                                                    B.SEND_ID
                                          UNION ALL
                                          SELECT    CONVERT(NVARCHAR(100), a.CONFIRM_DATE, 23) AS Trans_date ,
                                                    a.REF_CONTRACT_NO AS CONTRACT_NO ,
                                                    b.JOB_ORDER_NO ,
                                                    B.SEND_ID ,
                                                    0 AS sendQty ,
                                                    0 AS recQty ,
                                                    SUM(c.QTY) AS disqty
                                          FROM      dbo.OAS_SENDING_RECEIVING_HD A
                                                    INNER JOIN dbo.OAS_SENDING_RECEIVING_DT B ON B.DOC_NO = A.DOC_NO
                                                    INNER JOIN dbo.OAS_SENDING_RECEIVING_DISCREPANCY C ON C.TRX_ID = B.TRX_ID
                                          WHERE     A.TRX_TYPE = 'R'
                                                    AND IS_REAL_SENDING_RECEIVING = 'Y'
                                                    AND STATUS = 'C'
                                                    AND C.REASON_CD IN ( 'GEG025', 'GEG026' )
                                                    AND FACTORY_CD='" + FactoryCD + @"'
                                          GROUP BY  a.REF_CONTRACT_NO ,
                                                    b.JOB_ORDER_NO ,
                                                    CONVERT(NVARCHAR(100), a.CONFIRM_DATE, 23) ,
                                                    B.SEND_ID
                                        ) A
                                WHERE Trans_date>='" + TranDateBegin + @"' AND Trans_date<='" + TranDateEnd + @"'      
                                GROUP BY Trans_date ,
                                        CONTRACT_NO ,
                                        JOB_ORDER_NO ,
                                        SEND_ID " +

                                   //Added By ZouShiChang On 2014.04.18 End
                                   " ) tot " +
                                   "         group by tot.trx_date,tot.JOB_ORDER_NO,tot.contract_no,tot.send_id " +
                                   "         ) N " +
                                   "         on M.job_order_no=N.job_order_no and M.SEND_ID=N.SEND_ID " +
                                    "          where 1=1 {4} {5} order by SUBCONTRACTOR_NAME,TRX_DATE, CONTRACT_NO, JOB_ORDER_NO", strNO1Where, subcontractorWhere, strDateWhere, strNO2Where, strDate2Where, JOListWhere);

            DataTable dt = DBUtility.GetTable(sqlbu.ToString(), "MES");
            return dt;
        }

        public static DataTable GetIEContractReportData_ppo(string job_order_no, string FactoryCD, string txtBeginDate, string txtEndDate)
        {
            //在以下写入sql语句
            DataTable jobtb = new DataTable();
            jobtb.Columns.Add("jobno");
            string[] jobstr = job_order_no.Split(new char[] { ',' });
            DataRow row = null;
            for (int i = 0; i < jobstr.Length; i++)
            {
                row = jobtb.NewRow();
                row["jobno"] = jobstr[i].ToString().Trim().Replace(" ", "");
                jobtb.Rows.Add(row);
            }

            DbCommand dbcom = DBUtility.InsertTempData(jobtb, "INV");

            string sql = "";
            sql += " SELECT DISTINCT  h.inv_fty_cd, to_char(h.trans_date,'yyyy-mm-dd') AS Trans_date, l.job_order_no, ";
            sql += " SUM (NVL (l.trans_qty, 0)) AS issue_qty,          ";
            sql += " SUM (NVL (l.rtw_qty, 0)) AS return_qty, ";
            sql += " max(a.style_chn_desc) as style_chn_desc,";
            sql += " max(replace(replace(h.remarks,chr(13)||chr(10),''),chr(13),'')) as remarks,'n' as status     ";
            sql += " FROM inv_issue_hd h, inv_issue_line l, inv_item i, sc_hd a, po_hd b    ";
            sql += " WHERE h.issue_hd_id = l.issue_hd_id      AND l.inv_item_id = i.inv_item_id      ";
            sql += " AND h.inv_fty_cd = '" + FactoryCD + "'      AND h.status = 'F'      AND h.item_type_cd = 'F'      ";
            sql += " AND l.job_order_no = b.PO_NO      AND a.sc_no = b.sc_no      AND i.product_category IN ('K', 'W')      ";
            sql += " AND h.trans_cd NOT IN ('ITSK', 'ITSW')      AND l.job_order_no IN (select f1 From inventory.inv_tmp1)  ";
            if (!txtBeginDate.Equals("") && !txtEndDate.Equals(""))
            {
                sql += " AND to_char(h.trans_date,'yyyy-mm-dd') >='" + txtBeginDate + "' AND to_char(h.trans_date,'yyyy-mm-dd') <='" + txtEndDate + "'";
            }
            sql += " OR h.trans_date IS NULL";
            sql += " GROUP BY h.inv_fty_cd,          l.job_order_no,          h.trans_date     ";
            sql += " ORDER BY h.inv_fty_cd,l.job_order_no, to_char(h.trans_date,'yyyy-mm-dd')";
            DataTable dt = DBUtility.GetTable(sql, "INV", dbcom);
            return dt;
        }

    }
}