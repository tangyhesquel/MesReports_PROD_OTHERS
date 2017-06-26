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
    ///JoEntryListSummarySql 的摘要说明
    /// </summary>
    public class JoEntryListSummarySql
    {
        public static DataTable Get_JO_Entry_List_Summary(string Factory_cd, string Process_cd, string BeginDate, string EndDate, string TypeKW, string status)
        {
            DataTable dt;
            string SQL = "";

            if ((status == "Y" || status == "A") && (Process_cd.ToString().IndexOf("CUT") > 0 || Process_cd == ""))
            {
                SQL += " if OBJECT_ID('tempdb..#CUT_QTY') is not Null drop table #CUT_QTY ;";
                SQL += " select  'CUT       ' as Process_CD,trx_date=convert(char(10),b.create_date,120),b.job_order_no,";
                SQL += " 'AAAAAAAAAAAAAAAAAAAAAAAAAAA' AS short_name,isnull(a.Cut_line,'') as PRODUCTION_LINE,";
                SQL += " ISNULL(A.PRODUCTION_LINE_CD,'') AS Next_Production_line,'AAAAAAAAAA' AS wash_type_cd,";
                SQL += " color_code=a.color_cd,size_code=a.size_cd,complete_qty=b.qty,";
                SQL += " SPACE(10) AS pre_process_cd,create_date='1900-01-01',b.create_user_id,'' AS SUBMIT_USER_ID,";
                SQL += " SUBMIT_DATE=null,confirm_flag='Y'";
                SQL += " INTO #CUT_QTY";
                SQL += " from V_LAY A with(nolock) ";
                SQL += " inner join CUT_BUNDLE_HD B with(nolock)";
                SQL += " on A.LAY_DT_ID = B.LAY_DT_ID  ";
                SQL += " and a.job_order_no=b.job_order_no";
                SQL += " where  b.factory_cd='" + Factory_cd + "' ";
                SQL += " and b.create_date>='" + BeginDate + "' ";
                SQL += " and b.create_date<dateadd(d,1,'" + EndDate + "') ";

                if (TypeKW != "ALL")
                {
                    SQL += " AND EXISTS(SELECT TOP 1 1 FROM jo_hd WITH(NOLOCK) WHERE JO_NO=A.JOB_ORDER_NO ";
                    SQL += " AND garment_type_cd ='" + TypeKW + "')";
                }

                SQL += " UPDATE #CUT_QTY SET short_name='',wash_type_cd='',pre_process_cd='',create_date=trx_date;";
                SQL += " UPDATE #CUT_QTY SET short_name=D.short_name,wash_type_cd=C.wash_type_cd";
                SQL += " FROM  jo_hd c WITH(NOLOCK)";
                SQL += " INNER JOIN gen_customer d WITH(NOLOCK)";
                SQL += " ON c.customer_cd=d.customer_cd";
                SQL += " WHERE C.jo_no=#CUT_QTY.job_order_no;";
            }

            SQL += " select m.Process_CD,m.trx_date,m.job_order_no,M.short_name,m.PRODUCTION_LINE,m.Next_Production_line,M.wash_type_cd,m.color_code,";
            SQL += " m.size_code,m.complete_qty,m.pre_process_cd,m.create_date,m.create_user_id,m.SUBMIT_USER_ID,m.SUBMIT_DATE,";
            SQL += " m.confirm_flag,";
            SQL += " IS_FIRST_OUT=(case when CAST(M.trx_date AS SMALLDATETIME)=ISNULL(L.FIRST_OUT_DATE,'1900-1-1') THEN 'Y' ELSE 'N' END),";
            SQL += " ISNULL(P.PROD_COMPLETE,'N') AS PRE_PROCESS_CLOSE from ";
            SQL += "( ";
            if ((status == "Y" || status == "A") && ((Process_cd.IndexOf("CUT") > 0) || (Process_cd == "")))
            {
                SQL += " SELECT Process_CD,trx_date,job_order_no,short_name,PRODUCTION_LINE,";
                SQL += " Next_Production_line,wash_type_cd,color_code,size_code,SUM(complete_qty) AS complete_qty,";
                SQL += " pre_process_cd,create_date,create_user_id,SUBMIT_USER_ID,SUBMIT_DATE,confirm_flag";
                SQL += " FROM #CUT_QTY";
                SQL += " GROUP BY Process_CD,trx_date,job_order_no,short_name,PRODUCTION_LINE,";
                SQL += " Next_Production_line,wash_type_cd,color_code,size_code,";
                SQL += " pre_process_cd,create_date,create_user_id,SUBMIT_USER_ID,SUBMIT_DATE,confirm_flag";
                if (Process_cd == "")
                {
                    SQL += " UNION ALL";
                }

            }
            if ((status == "Y" || status == "A") && (Process_cd.IndexOf("CUT") <= 0))
            {

                SQL += " select a.next_process_cd as Process_CD, ";
                SQL += " trx_date=convert(char(10),b.trx_date,120),b.job_order_no,";
                SQL += " d.short_name,ISNULL(b.PRODUCTION_LINE_CD,'') AS PRODUCTION_LINE,ISNULL(b.DES_PRODUCTION_LINE_CD,'') as Next_Production_line,";
                SQL += " c.wash_type_cd,b.color_code,b.size_code,b.complete_qty,pre_process_cd=b.process_cd,convert(char(16),";
                SQL += " b.create_date,120) AS create_date,a.create_user_id,a.SUBMIT_USER_ID,a.SUBMIT_DATE,confirm_flag='Y' ";
                SQL += " from prd_jo_output_hd a WITH(NOLOCK)";
                SQL += " INNER JOIN prd_jo_output_trx b WITH(NOLOCK)";
                SQL += " ON a.doc_no=b.doc_no ";
                SQL += " INNER JOIN jo_hd c WITH(NOLOCK)";
                SQL += " ON C.jo_no=b.job_order_no";
                SQL += " INNER JOIN gen_customer d WITH(NOLOCK)";
                SQL += " ON c.customer_cd=d.customer_cd";
                SQL += " where  b.job_order_no=c.jo_no";
                SQL += " and a.factory_cd='" + Factory_cd + "' ";
                if (Process_cd != "")
                {
                    SQL += " and (a.next_process_cd='" + Process_cd + "') ";
                }
                if (BeginDate == EndDate)
                {
                    SQL += " and b.trx_date='" + BeginDate + "' ";
                }
                else
                {
                    SQL += " and b.trx_date>='" + BeginDate + "' ";
                    SQL += " and b.trx_date<='" + EndDate + "' ";
                }

                if (TypeKW != "ALL")
                {
                    SQL += " and c.garment_type_cd='" + TypeKW + "' ";
                }
                if (status == "A")
                {
                    SQL += " union all ";
                }
            }

            if ((status == "N" || status == "A") && (Process_cd.IndexOf("CUT") <= 0))
            {
                SQL += " select a.next_process_cd as Process_CD,trx_date=convert(char(10),b.trx_date,120),b.job_order_no,d.short_name,";
                SQL += " ISNULL(b.PRODUCTION_LINE_CD,'') AS PRODUCTION_LINE,ISNULL(b.DES_PRODUCTION_LINE_CD,'') as Next_Production_line,";
                SQL += " c.wash_type_cd,b.color_code,b.size_code,b.complete_qty,pre_process_cd=b.process_cd,";
                SQL += " create_date=convert(char(16),a.create_date,120),a.create_user_id,'' AS SUBMIT_USER_ID,SUBMIT_DATE=NULL,confirm_flag='N'";
                SQL += " from prd_jo_output_hd a WITH(NOLOCK)";
                SQL += " INNER JOIN prd_jo_output_dft b WITH(NOLOCK)";
                SQL += " ON  a.doc_no=b.doc_no";
                SQL += " INNER JOIN jo_hd c WITH(NOLOCK)";
                SQL += " ON b.job_order_no=c.jo_no";
                SQL += " INNER JOIN gen_customer d WITH(NOLOCK) ";
                SQL += " ON c.customer_cd=d.customer_cd  ";
                SQL += " where a.factory_cd='" + Factory_cd + "'";
                if (Process_cd != "ALL")
                {
                    SQL += " and (a.next_process_cd='" + Process_cd + "') ";
                }
                if (BeginDate == EndDate)
                {
                    SQL += " and b.trx_date='" + BeginDate + "' ";
                }
                else
                {
                    SQL += " and b.trx_date>='" + BeginDate + "' ";
                    SQL += " and b.trx_date<='" + EndDate + "' ";
                }
                if (TypeKW != "ALL")
                {
                    SQL += " and c.garment_type_cd='" + TypeKW + "' ";
                }

            }
            SQL += " )M ";
            SQL += " LEFT JOIN ( SELECT JOB_ORDER_NO, PROCESS_CD,MIN(TRX_DATE) AS FIRST_OUT_DATE FROM   ";
            SQL += " prd_jo_output_trx WITH(NOLOCK)";
            SQL += " GROUP BY  JOB_ORDER_NO,PROCESS_CD ) L ON M.job_order_no=L.job_order_no ";
            SQL += " AND M.pre_process_cd=L.PROCESS_CD ";
            SQL += " LEFT JOIN PRD_JO_COMPLETE_STATUS P WITH(NOLOCK) ON P.JOB_ORDER_NO=M.job_order_no AND P.PROCESS_CD=M.pre_process_cd ";
            SQL += " order by m.process_cd,m.trx_date,m.job_order_no ";

            dt = DBUtility.GetTable(SQL, "MES");
            return dt;
        }

        public static DataTable Get_JO_Entry_List_Summary(string Factory_cd, string Process_cd, string Process_Type, string Prod_Factory, string BeginDate, string EndDate, string TypeKW, string status, string GroupName)
        {

            DataTable dt;
            string SQL = "";
            Random ro = new Random(1000);
            string tempTable = "#temp_detail_" + ro.Next().ToString();
            if (!GroupName.Equals(""))
            {
                SQL += " declare @s nvarchar(max)";
                SQL += " select @s=system_value from gen_system_setting where factory_cd='" + Factory_cd + "' and system_key='" + GroupName + "'; ";
            }

            if (!(status.Equals("N") || status.Equals("S")) && (Process_cd.Contains("CUT") || Process_cd == ""))
            {
                SQL += " if OBJECT_ID('tempdb..#CUT_QTY') is not Null drop table #CUT_QTY ;";
                SQL += " select  'CUT       ' as Process_CD,GARMENT_TYPE,PROCESS_TYPE,PRODUCTION_FACTORY,trx_date=b.create_date,b.job_order_no,";
                SQL += " 'AAAAAAAAAAAAAAAAAAAAAAAAAAA' AS short_name,isnull(B.Cut_line,'') as PRODUCTION_LINE,";
                SQL += " ISNULL(B.PRODUCTION_LINE_CD,'') AS Next_Production_line,'AAAAAAAAAA' AS wash_type_cd,";
                SQL += " color_code=B.color_cd,size_code=B.size_cd,complete_qty=b.qty,";
                SQL += " SPACE(10) AS pre_process_cd,SPACE(2) AS pre_garment_type,space(2) as pre_process_type,SPACE(3) AS pre_production_factory,cast('1900-01-01' as datetime) as create_date,b.create_user_id,'' AS SUBMIT_USER_ID,";
                SQL += " SUBMIT_DATE=null,confirm_flag='Y'";
                SQL += " INTO #CUT_QTY";
                SQL += " from  ";
                SQL += " CUT_BUNDLE_HD B with(nolock)";
                SQL += " where  b.factory_cd='" + Factory_cd + "' ";
                SQL += " and b.create_date>='" + BeginDate + "' ";
                SQL += " and b.create_date<dateadd(d,1,'" + EndDate + "') ";

                if (TypeKW != "ALL")
                {
                    SQL += " AND EXISTS(SELECT TOP 1 1 FROM jo_hd WITH(NOLOCK) WHERE JO_NO=B.JOB_ORDER_NO ";
                    SQL += " AND garment_type_cd ='" + TypeKW + "')";
                }
                if (TypeKW != "ALL")
                {

                    SQL += " AND garment_type ='" + TypeKW + "'";
                }

                SQL += " UPDATE #CUT_QTY SET short_name='',wash_type_cd='',pre_process_cd='',pre_garment_type='',pre_process_type='',pre_production_factory='',create_date=trx_date;";
                SQL += " UPDATE #CUT_QTY SET short_name=D.short_name,wash_type_cd=C.wash_type_cd";
                SQL += " FROM  jo_hd c WITH(NOLOCK)";
                SQL += " INNER JOIN gen_customer d WITH(NOLOCK)";
                SQL += " ON c.customer_cd=d.customer_cd";
                SQL += " WHERE C.jo_no=#CUT_QTY.job_order_no;";

            }

            SQL += " if OBJECT_ID('tempdb.." + tempTable + "') is not Null drop table " + tempTable + " ;";

            SQL += " select m.Process_CD,M.Garment_Type,M.Process_Type,M.Production_factory,trx_date,m.job_order_no,M.short_name,m.PRODUCTION_LINE,m.Next_Production_line,M.wash_type_cd,m.color_code,";
            SQL += " m.size_code,m.complete_qty,m.pre_process_cd,M.pre_garment_type,M.pre_process_type,M.pre_production_factory,m.create_date,m.create_user_id,m.SUBMIT_USER_ID,m.SUBMIT_DATE,";
            SQL += " m.confirm_flag,";
            SQL += " 'Y' AS IS_FIRST_OUT,";
            SQL += " 'N' AS PRE_PROCESS_CLOSE into " + tempTable + " from ";
            SQL += "( ";
            if (!(status.Equals("N") || status.Equals("S")) && (Process_cd.Contains("CUT") || (Process_cd.Equals(""))))
            {
                SQL += " SELECT Process_CD,Garment_Type,Process_Type,PRODUCTION_FACTORY,trx_date,job_order_no,short_name,PRODUCTION_LINE,";
                SQL += " Next_Production_line,wash_type_cd,color_code,size_code,SUM(complete_qty) AS complete_qty,";
                SQL += " pre_process_cd,pre_garment_type,pre_process_type,pre_production_factory,create_date,create_user_id,SUBMIT_USER_ID,SUBMIT_DATE,confirm_flag";
                SQL += " FROM #CUT_QTY";
                if (!GroupName.Equals(""))
                {
                    SQL += " WHERE EXISTS (select 1 from dbo.FN_SPLIT_STRING_TB(@s,',') WHERE FNFIELD = PRODUCTION_LINE)";
                    SQL += " AND PROCESS_CD LIKE '%SEW%'";
                }
                SQL += " GROUP BY Process_CD,Garment_type,Process_Type,PRODUCTION_FACTORY,trx_date,job_order_no,short_name,PRODUCTION_LINE,";
                SQL += " Next_Production_line,wash_type_cd,color_code,size_code,";
                SQL += " pre_process_cd,Pre_Garment_Type,Pre_Process_Type,pre_production_factory,create_date,create_user_id,SUBMIT_USER_ID,SUBMIT_DATE,confirm_flag";
                if (Process_cd.Equals(""))
                {
                    SQL += " UNION ALL";
                }

            }
            if (!(status.Equals("N") || status.Equals("S")) && !Process_cd.Contains("CUT"))
            {

                SQL += " select a.next_process_cd as Process_CD,A.Next_process_garment_type as Garment_type,A.next_process_type as process_type,A.next_PROCESS_PRODUCTION_FACTORY AS Production_factory, ";
                SQL += " trx_date=convert(char(10),b.trx_date,120),b.job_order_no,";
                //注意,这里的Next_Production_Line的命名其实意思是Previous_Production_Line,因为全部改动的话影响太大,就保持了这个命名;
                SQL += " d.short_name,ISNULL(b.DES_PRODUCTION_LINE_CD,'') AS PRODUCTION_LINE,ISNULL(b.PRODUCTION_LINE_CD,'') as Next_Production_line,";
                SQL += " c.wash_type_cd,b.color_code,b.size_code,b.complete_qty,pre_process_cd=b.process_cd,pre_garment_type=b.process_garment_type,pre_process_type=b.process_type,pre_production_factory=b.Process_production_factory,convert(char(16),";
                SQL += @" b.create_date,120) AS create_date,a.create_user_id,CASE WHEN ISNULL(a.SUBMIT_USER_ID,'')='' THEN z.SUBMIT_USER_ID ELSE a.SUBMIT_USER_ID END AS SUBMIT_USER_ID,
                            CASE WHEN ISNULL(a.SUBMIT_DATE,'')='' THEN z.SUBMIT_DATE ELSE a.SUBMIT_DATE END AS SUBMIT_DATE,confirm_flag='Y' ";
                SQL += " from prd_jo_output_hd a WITH(NOLOCK)";
                SQL += " INNER JOIN prd_jo_output_trx b WITH(NOLOCK)";
                SQL += " ON a.doc_no=b.doc_no ";
                SQL += " INNER JOIN jo_hd c WITH(NOLOCK)";
                SQL += " ON C.jo_no=b.job_order_no";
                SQL += " INNER JOIN gen_customer d WITH(NOLOCK)";
                SQL += " ON c.customer_cd=d.customer_cd LEFT JOIN dbo.PRD_GARMENT_TRANSFER_HD Z ON z.DOC_NO=a.DOC_NO ";
                SQL += " where  b.job_order_no=c.jo_no";
                SQL += " and a.factory_cd='" + Factory_cd + "' ";
                if (!Process_cd.Equals(""))
                {
                    SQL += " and (a.next_process_cd='" + Process_cd + "') ";
                }

                if (!Process_Type.Equals(""))
                {
                    SQL += " and a.next_process_type='" + Process_Type + "' ";
                }
                if (!TypeKW.Equals("ALL"))
                {
                    SQL += " and a.next_process_garment_type='" + TypeKW + "' ";
                }
                if (!Prod_Factory.Equals(""))
                {
                    SQL += " and a.next_process_production_factory='" + Prod_Factory + "'";
                }


                if (BeginDate == EndDate)
                {
                    SQL += " and b.trx_date='" + BeginDate + "' ";
                }
                else
                {
                    SQL += " and b.trx_date>='" + BeginDate + "' ";
                    SQL += " and b.trx_date<='" + EndDate + "' ";
                }

                if (TypeKW != "ALL")
                {
                    SQL += " and c.garment_type_cd='" + TypeKW + "' ";
                }
                if (!GroupName.Equals(""))
                {
                    SQL += " AND EXISTS (select 1 from dbo.FN_SPLIT_STRING_TB(@s,',') WHERE FNFIELD = B.DES_PRODUCTION_LINE_CD)";
                    SQL += " AND A.NEXT_PROCESS_CD LIKE '%SEW%'";
                }
                if (status == "A")
                {
                    SQL += " union all ";
                }
            }

            if (!status.Equals("Y") && !Process_cd.Contains("CUT"))
            {
                SQL += " select a.next_process_cd as Process_CD,a.next_process_garment_type as garment_type,a.next_process_type as process_type,A.NEXT_PROCESs_production_factory AS Production_factory,trx_date=convert(char(10),b.trx_date,120),b.job_order_no,d.short_name,";
                //注意,这里的Next_Production_Line的命名其实意思是Previous_Production_Line,因为全部改动的话影响太大,就保持了这个命名;
                SQL += " ISNULL(b.DES_PRODUCTION_LINE_CD,'') AS PRODUCTION_LINE,ISNULL(b.PRODUCTION_LINE_CD,'') as Next_Production_line,";
                SQL += " c.wash_type_cd,b.color_code,b.size_code,b.complete_qty,pre_process_cd=b.process_cd,pre_garment_type=b.process_garment_type,pre_process_type=b.process_type , pre_Production_factory=b.Process_production_factory, ";
                SQL += " create_date=convert(char(16),a.create_date,120),a.create_user_id,'' AS SUBMIT_USER_ID,SUBMIT_DATE=NULL,confirm_flag='N'";
                SQL += " from prd_jo_output_hd a WITH(NOLOCK)";
                SQL += " INNER JOIN prd_jo_output_dft b WITH(NOLOCK)";
                SQL += " ON  a.doc_no=b.doc_no";
                SQL += " INNER JOIN jo_hd c WITH(NOLOCK)";
                SQL += " ON b.job_order_no=c.jo_no";
                SQL += " INNER JOIN gen_customer d WITH(NOLOCK) ";
                SQL += " ON c.customer_cd=d.customer_cd  ";
                SQL += " where a.factory_cd='" + Factory_cd + "'";
                if (Process_cd != "")
                {
                    SQL += " and (a.next_process_cd='" + Process_cd + "') ";
                }

                if (!Process_Type.Equals(""))
                {
                    SQL += " and a.next_process_type='" + Process_Type + "' ";
                }
                if (!TypeKW.Equals("ALL"))
                {
                    SQL += " and a.next_process_garment_type='" + TypeKW + "' ";
                }
                if (!Prod_Factory.Equals(""))
                {
                    SQL += " and a.next_process_production_factory='" + Prod_Factory + "'";
                }


                if (BeginDate == EndDate)
                {
                    SQL += " and b.trx_date='" + BeginDate + "' ";
                }
                else
                {
                    SQL += " and b.trx_date>='" + BeginDate + "' ";
                    SQL += " and b.trx_date<='" + EndDate + "' ";
                }
                if (TypeKW != "ALL")
                {
                    SQL += " and c.garment_type_cd='" + TypeKW + "' ";
                }
                if (!GroupName.Equals(""))
                {
                    SQL += " AND EXISTS (select 1 from dbo.FN_SPLIT_STRING_TB(@s,',') WHERE FNFIELD = B.DES_PRODUCTION_LINE_CD)";
                    SQL += " AND A.NEXT_PROCESS_CD LIKE '%SEW%'";
                }
            }

            if (!status.Equals("Y") && !Process_cd.Contains("CUT"))
            {
                SQL += " UNION ALL";
                SQL += " SELECT A.NEXT_PROCESS_CD AS PROCESS_CD,a.next_process_garment_type as garment_type,a.next_process_type as process_type,a.next_process_production_factory AS Production_factory,trx_date=convert(char(10),A.CREATE_DATE,120),b.job_order_no,d.short_name, ";
                SQL += " ISNULL(A.NEXT_PRODUCTION_LINE_CD,'') AS PRODUCTION_LINE_CD,";
                //注意,这里的Next_Production_Line的命名其实意思是Previous_Production_Line,因为全部改动的话影响太大,就保持了这个命名;
                SQL += " ISNULL(A.PRODUCTION_LINE_CD,'') AS NEXT_PRODUCTION_LINE,c.wash_type_cd,b.color_CD,b.size_CD";
                SQL += " ,b.qty,pre_process_cd=A.process_cd,pre_garment_type=A.process_garment_type,pre_process_type=A.process_type,pre_production_factory=A.PROCESS_PRODUCTION_FACTORY, create_date=convert(char(16),a.create_date,120),";
                SQL += " a.create_user_id,'' AS SUBMIT_USER_ID,SUBMIT_DATE=NULL,confirm_flag='N' ";
                SQL += " FROM PRD_GARMENT_TRANSFER_HD A";
                SQL += " INNER JOIN PRD_GARMENT_TRANSFER_DFT B ON A.DOC_NO =B.DOC_NO";
                SQL += " INNER JOIN jo_hd c WITH(NOLOCK) ON b.job_order_no=c.jo_no";
                SQL += " INNER JOIN gen_customer d WITH(NOLOCK)  ON c.customer_cd=d.customer_cd   ";
                SQL += " WHERE A.FACTORY_CD = '" + Factory_cd + "' ";
                //SQL += " AND A.STATUS='N'";
                SQL += " AND A.FACTORY_CD=CASE WHEN A.PROCESS_TYPE='P' THEN C.CO_FACTORY_CD ELSE a.FACTORY_CD END   ";
                if (status.Equals("A"))
                {
                    SQL += " AND A.STATUS in('N','S')";
                }
                else
                {
                    SQL += " AND A.STATUS ='" + status + "' ";
                }
                if (Process_cd != "")
                {
                    SQL += " and (a.next_process_cd='" + Process_cd + "') ";
                }

                if (!Process_Type.Equals(""))
                {
                    SQL += " and a.next_process_type='" + Process_Type + "' ";
                }
                if (!TypeKW.Equals("ALL"))
                {
                    SQL += " and a.next_process_garment_type='" + TypeKW + "' ";
                }
                if (!Prod_Factory.Equals(""))
                {
                    SQL += " and a.next_process_production_factory='" + Prod_Factory + "'";
                }


                if (BeginDate == EndDate)
                {
                    SQL += " and A.create_date>='" + BeginDate + "' ";
                    SQL += " and A.create_date<dateadd(d,1,'" + BeginDate + "') ";
                }
                else
                {
                    SQL += " and A.CREATE_DATE>='" + BeginDate + "' ";
                    SQL += " and A.create_date<dateadd(d,1,'" + EndDate + "') ";
                }
                if (!GroupName.Equals(""))
                {
                    SQL += " AND EXISTS (select 1 from dbo.FN_SPLIT_STRING_TB(@s,',') WHERE FNFIELD = A.NEXT_PRODUCTION_LINE_CD) ";
                    SQL += " AND next_process_cd like '%sew%'";
                }
            }
            SQL += " )M; ";

            SQL += " UPDATE " + tempTable + "  SET IS_FIRST_OUT =CASE WHEN X.trx_date=Y.FIRST_OUT_DATE THEN 'Y' ELSE 'N' END ";
            SQL += " 					FROM " + tempTable + " X ";
            SQL += " 						INNER JOIN ";
            SQL += "	(	SELECT JOB_ORDER_NO, PROCESS_CD,process_garment_type,process_type,Process_production_factory,MIN(TRX_DATE) AS FIRST_OUT_DATE FROM   ";
            SQL += "										prd_jo_output_trx A WITH(NOLOCK) 	";
            SQL += "								WHERE EXISTS(SELECT 1 FROM " + tempTable + " T  ";
            SQL += "													WHERE T.JOB_ORDER_NO=A.JOB_ORDER_NO AND T.pre_process_cd=A.PROCESS_CD and T.pre_garment_type=a.process_garment_type and T.pre_process_type=a.process_type AND T.Pre_PRODUCTION_FACTORY=A.PROCESS_PRODUCTION_FACTORY )	";
            SQL += "									GROUP BY  JOB_ORDER_NO,PROCESS_CD,process_garment_type,process_type,process_production_factory 		";
            SQL += "	)Y ON X.JOB_ORDER_NO=Y.JOB_ORDER_NO AND X.pre_process_cd=Y.PROCESS_CD And x.pre_garment_type=y.process_garment_type and x.pre_process_type=y.process_type AND X.pre_production_factory=y.process_production_factory ;";


            SQL += " UPDATE " + tempTable + " SET PRE_PROCESS_CLOSE=Y.PROD_COMPLETE ";
            SQL += " 			 FROM " + tempTable + " X INNER JOIN  PRD_JO_COMPLETE_STATUS Y with(nolock) ";
            SQL += " 			 ON X.JOB_ORDER_NO=Y.job_order_no AND Y.PROCESS_CD=X.pre_process_cd AND Y.GARMENT_TYPE = X.PRE_GARMENT_TYPE AND Y.PROCESS_TYPE = X.PRE_PROCESS_TYPE AND Y.PRODUCTION_FACTORY=x.pre_production_factory where Y.FACTORY_CD='" + Factory_cd + "' ";


            SQL += " SELECT ";
            SQL += " Process_CD AS [Process], GARMENT_TYPE AS [Garment Type] ,PROCESS_TYPE AS [Process Type] ,PRODUCTION_FACTORY AS [PRODUCTION FACTORY],PRODUCTION_LINE AS [Production Line],CONVERT(varchar(10),trx_date,121) AS [Date],job_order_no AS [JO],short_name AS [Customer],";
            SQL += " wash_type_cd AS [Wash Type],color_code AS [Color],size_code AS [Size],complete_qty AS [Transaction Qty],pre_process_cd AS [Previous Process],pre_GARMENT_TYPE AS [Previous Garment Type] ,pre_Process_type AS [Previous Process Type], pre_production_factory AS [Previous production factory],";
            SQL += " Next_Production_line AS [Previous Production line],create_date AS [Create Date],create_user_id AS [Operator],SUBMIT_USER_ID AS [Confirmed By],";
            SQL += " SUBMIT_DATE AS [Confirmed Date],confirm_flag AS [Confirmed],IS_FIRST_OUT AS [First Transfer],PRE_PROCESS_CLOSE AS [Pre-Process Closed]";
            SQL += " FROM " + tempTable + " m order by m.process_cd,m.Garment_type ,M.PROCESS_TYPE ,m.trx_date,m.job_order_no";

            dt = DBUtility.GetTable(SQL, "MES");
            return dt;
        }

    }
}