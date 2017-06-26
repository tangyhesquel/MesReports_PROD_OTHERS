using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web.UI.WebControls;

namespace MESComment
{
    public class MesRpt
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

        public static void Ini_CT_Alert_DDL(DropDownList ddl, string ddlGarmenType, string factory_cd)
        {
           
            string SQL = "";
            SQL += " select 'ALL' PROCESS_CD,'ALL' SHORT_NAME union all ";
            SQL += " select distinct PROCESS_CD,SHORT_NAME from PRD_fty_process_flow F,dbo.GEN_PRC_CD_MST M ";
            SQL += " WHERE F.PROCESS_CD=M.PRC_CD AND F.FACTORY_CD = M.FACTORY_CD AND M.ACTIVE='Y' ";
            SQL += " AND F.PROCESS_GARMENT_TYPE=M.GARMENT_TYPE  "; 
            SQL += " AND M.factory_cd='" + factory_cd + "'";
            if (!ddlGarmenType.Equals("ALL"))
            {
                SQL += " AND M.GARMENT_TYPE='" + ddlGarmenType + "'";
            }
            ddl.DataTextField = "SHORT_NAME";
            ddl.DataValueField = "PROCESS_CD";
            ddl.DataSource = DBUtility.GetTable(SQL, "MES");
            ddl.DataBind();
        }

        public static void Ini_TypeKW_DDL(DropDownList ddl)
        {
            string sqlstr = "select 'ALL' Type union all select 'K' union all select 'W'";
            ddl.DataTextField = "Type";
            ddl.DataValueField = "Type";
            ddl.DataSource = DBUtility.GetTable(sqlstr, "MES");
            ddl.DataBind();
        }

        public static DataTable Get_Weekly_Reports(string System, string SubSystem, string Handler, string Group, string StartDate, string EndDate, string OrderBy, bool IsNew)
        {
            string isNewSRF = "";
            string SQL = "select m.groupname,m.sysname,m.note_no,m.handler,m.ori_begin_time,m.ori_end_time,m.begin_time,end_time=(case when m.end_time<m.begin_time then m.begin_time else m.end_time end),m.status ";
            SQL = SQL + "from ( ";
            SQL = SQL + " select b.groupname,b.sysname,a.note_no, ";
            SQL = SQL + "handler=(case when dbo.udf_wrgethandlernamelist(a.note_no) is null or dbo.udf_wrgethandlernamelist(a.note_no)='' then '' else substring(dbo.udf_wrgethandlernamelist(a.note_no),1,len(dbo.udf_wrgethandlernamelist(a.note_no))-1) end), ";
            SQL = SQL + "ori_begin_time=a.begin_time,ori_end_time=a.end_time, ";
            SQL = SQL + "begin_time=(case when a.begin_time<'{4}' then '{4}' else a.begin_time end), ";
            SQL = SQL + "end_time=(case when a.end_time>'{5}' then '{5}' else a.end_time end), ";
            SQL = SQL + "status=(case when a.status='Closed' then 'C' when a.status='Testing' then 'T' else (case when convert(char,a.end_time,102)<convert(char,getdate(),102) then 'D' else 'A' end)  end) ";
            SQL = SQL + "from wrreportbody a,wrsystemdetail b ";
            SQL = SQL + "where a.system_id=b.sysid and a.status not in ('New','Canceled','Paused') ";
            SQL = SQL + "and {0} ";
            SQL = SQL + "and ((a.begin_time>='{4}' and a.begin_time<='{5}') ";
            SQL = SQL + "or (a.end_time>='{4}' and a.end_time<='{5}')) ";
            SQL = SQL + "and {1} and {2} and {3} and {7} ";
            SQL = SQL + "union all ";
            SQL = SQL + " select b.groupname,b.sysname,a.note_no, ";
            SQL = SQL + "handler=(case when dbo.udf_wrgethandlernamelist(a.note_no) is null or dbo.udf_wrgethandlernamelist(a.note_no)='' then '' else substring(dbo.udf_wrgethandlernamelist(a.note_no),1,len(dbo.udf_wrgethandlernamelist(a.note_no))-1) end), ";
            SQL = SQL + "ori_begin_time=a.begin_time,ori_end_time=a.end_time, ";
            SQL = SQL + "begin_time=(case when a.begin_time<'{4}' then '{4}' else a.begin_time end), ";
            SQL = SQL + "end_time=(case when a.end_time>'{5}' then '{5}' else a.end_time end), ";
            SQL = SQL + "status=(case when a.status='Testing' then 'OT' else 'OD' end) ";
            SQL = SQL + "from wrreportbody a,wrsystemdetail b ";
            SQL = SQL + "where a.system_id=b.sysid and a.status not in ('New','Canceled','Paused','Closed') ";
            SQL = SQL + "and {0} ";
            SQL = SQL + "and a.end_time<'{4}' ";
            SQL = SQL + "and {1} and {2} and {3} and {7} ";
            SQL = SQL + ") m ";
            SQL = SQL + "{6} ";

            if (System == "ALL")
            {
                System = "1=1";
            }
            else
            {
                System = "b.groupname = '" + System + "'";
            }
            if (SubSystem == "ALL")
            {
                SubSystem = "1=1";
            }
            else
            {
                SubSystem = "b.sysname='" + SubSystem + "'";
            }
            if (Handler == "")
            {
                Handler = "1=1";
            }
            else
            {
                Handler = "exists (select 1 from wrreporthandler where note_no=a.note_no and handler='" + Handler + "') ";
            }
            if (Group == "")
            {
                Group = "1=1";
            }
            else
            {
                Group = "(exists (select 1 from wrReportGroup nn,wrreporthandler mm where nn.system_id=0 and nn.handler=mm.handler and mm.note_no=a.note_no and nn.group_name='" + Group + "') or exists (select 1 from wrReportGroup nn,wrsystemdetail mm where nn.handler='ALL' and nn.system_id=mm.sysid and mm.sysid=a.system_id and nn.group_name='" + Group + "')) ";
            }
            switch (OrderBy)
            {
                case "0":
                    OrderBy = "order by m.ori_begin_time,m.groupname,m.sysname,m.note_no ";
                    break;
                case "1":
                    OrderBy = "order by m.groupname,m.sysname,m.ori_begin_time,m.note_no ";
                    break;
                case "2":
                    OrderBy = "order by m.handler,m.ori_begin_time,m.groupname,m.sysname,m.note_no ";
                    break;
            }
            if (!IsNew)
            {
                isNewSRF = "dbo.udf_wrgethandlernamelist(a.note_no) is not null and dbo.udf_wrgethandlernamelist(a.note_no)<>'' ";
            }
            else
            {
                isNewSRF = "1=1 ";
            }
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat(SQL, System, SubSystem, Handler, Group, StartDate, EndDate, OrderBy, isNewSRF);
            return DBUtility.GetTable(sqlstr.ToString(), "WeeklyReport");
        }
        public static DataTable Get_Weekly_Reports_New(string System, string SubSystem, string Group, string OrderBy)
        {
            string SQL = "select * ";
            SQL = SQL + "from ( ";
            SQL = SQL + " select b.groupname,b.sysname,a.note_no,creator=n.username,create_time=convert(char(10),a.create_time,120),a.subject,a.content, ";
            SQL = SQL + "ori_begin_time=a.begin_time,ori_end_time=a.end_time, ";
            SQL = SQL + "status=(case when a.status='Closed' then 'C' when a.status='Testing' then 'T' else (case when a.end_time<getdate() then 'D' else 'A' end)  end) ";
            SQL = SQL + "from wrreportbody a,wrsystemdetail b,(select maintman,username=max(maintman_mail) from wrsystemsupport group by maintman) n  ";
            SQL = SQL + "where a.system_id=b.sysid and a.status='New' ";
            SQL = SQL + " and a.creator=n.maintman ";
            SQL = SQL + "and {0} ";
            SQL = SQL + "and {1} and {2}  ";
            SQL = SQL + ") m ";
            SQL = SQL + "{3} ";

            if (System == "ALL")
            {
                System = "1=1";
            }
            else
            {
                System = "b.groupname = '" + System + "'";
            }
            if (SubSystem == "ALL")
            {
                SubSystem = "1=1";
            }
            else
            {
                SubSystem = "b.sysname='" + SubSystem + "'";
            }
            if (Group == "")
            {
                Group = "1=1";
            }
            else
            {
                Group = "(exists (select 1 from wrReportGroup nn,wrreporthandler mm where nn.system_id=0 and nn.handler=mm.handler and mm.note_no=a.note_no and nn.group_name='" + Group + "') or exists (select 1 from wrReportGroup nn,wrsystemdetail mm where nn.handler='ALL' and nn.system_id=mm.sysid and mm.sysid=a.system_id and nn.group_name='" + Group + "')) ";
            }
            switch (OrderBy)
            {
                case "0":
                    OrderBy = "order by m.ori_begin_time,m.groupname,m.sysname,m.note_no ";
                    break;
                case "1":
                    OrderBy = "order by m.groupname,m.sysname,m.ori_begin_time,m.note_no ";
                    break;
                case "2":
                    OrderBy = "order by m.handler,m.ori_begin_time,m.groupname,m.sysname,m.note_no ";
                    break;
            }
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat(SQL, System, SubSystem, Group, OrderBy);
            return DBUtility.GetTable(sqlstr.ToString(), "WeeklyReport");
        }
        public static DataTable Get_SRF_ByNo(string SRF_NO)
        {
            string SQL = " select ori_begin_time=convert(char(16),begin_time,120),ori_end_time=convert(char(16),end_time,120),workdays,handler=(case when dbo.udf_wrgethandlernamelist(a.note_no) is null or dbo.udf_wrgethandlernamelist(a.note_no)='' then '' else substring(dbo.udf_wrgethandlernamelist(a.note_no),1,len(dbo.udf_wrgethandlernamelist(a.note_no))-1) end),subject ";
            SQL = SQL + "from wrreportbody a ";
            SQL = SQL + "where a.note_no='{0}'";
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.AppendFormat(SQL, SRF_NO);
            return DBUtility.GetTable(sqlstr.ToString(), "WeeklyReport");
        }
        public static void Ini_Weekly_Reports_DDL(DropDownList ddl, string Parent)
        {
            string sqlstr = "";
            switch (ddl.ID)
            {
                case "ddlSystem":
                    sqlstr = "select group_name='ALL',seq=1 union all select distinct group_name,seq=2 from wrsystemgroup order by seq,group_name";
                    ddl.DataTextField = "group_name";
                    ddl.DataValueField = "group_name";
                    break;
                case "ddlSubSystem":
                    sqlstr = "select sysname='ALL',seq=0 union all select distinct sysname,seq=2 from wrsystemdetail where groupname='" + Parent + "' order by seq,sysname";
                    ddl.DataTextField = "sysname";
                    ddl.DataValueField = "sysname";
                    break;
                case "ddlHandler":
                    sqlstr = "select MaintMan_Mail='',maintman='' union all select distinct MaintMan_Mail,maintman from wrsystemsupport order by MaintMan_Mail";
                    ddl.DataTextField = "MaintMan_Mail";
                    ddl.DataValueField = "maintman";
                    break;
                case "ddlGroup":
                    sqlstr += "select group_name='' union all select distinct group_name from wrReportGroup  where Create_UserID='" + Parent + "' order by group_name";
                    ddl.DataTextField = "group_name";
                    ddl.DataValueField = "group_name";
                    break;
            }
            ddl.DataSource = DBUtility.GetTable(sqlstr, "WeeklyReport");
            ddl.DataBind();
        }
        public static DataTable Get_Group_Detail(string Type, string UserName)
        {
            string sqlstr = "";
            if (Type == "ByHandler")
            {
                sqlstr += "select distinct group_name from wrReportGroup where system_id=0 and Create_UserID='" + UserName + "' order by group_name";
            }
            else if (Type == "BySystem")
            {
                sqlstr += "select distinct group_name from wrReportGroup where handler='ALL' and Create_UserID='" + UserName + "' order by group_name";
            }
            return DBUtility.GetTable(sqlstr, "WeeklyReport");
        }
        public static DataTable Get_DDL_DataSource(string ddlName, string Parent)
        {
            string sqlstr = "";
            switch (ddlName)
            {
                case "ddlSystem":
                    sqlstr = "select distinct group_name from wrsystemgroup order by group_name";
                    break;
                case "ddlSubSystem":
                    sqlstr = "select distinct sysname from wrsystemdetail where groupname='" + Parent + "' order by sysname";
                    break;
                case "ddlHandler":
                    sqlstr = "select distinct maintman from wrsystemsupport order by maintman";
                    break;
                case "ddlGroup":
                    sqlstr = "select group_name='' union all select distinct group_name from wrReportGroup  where (Create_UserID='" + Parent + "' or public_flag='Y') order by group_name";
                    break;
            }
            return DBUtility.GetTable(sqlstr, "WeeklyReport");

        }
        public static void Add_Group_Detail(string GroupName, string Handler, int SystemId, string UserId)
        {
            try
            {
                if (GroupName != "")
                {
                    DBUtility.ExecuteNonQuery("insert into wrReportGroup(group_name,system_id,handler,create_userid,create_date) values('" + GroupName.Replace("'", "`").Trim() + "'," + SystemId + ",'" + Handler + "','" + UserId + "',getdate())", "WeeklyReport");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
        public static void Delete_Group_By_GroupName(string GroupName, string Type)
        {
            string sqlstr = "";
            if (Type == "ByHandler")
            {
                sqlstr += "delete wrReportGroup where group_name='" + GroupName + "' and system_id=0 ";
            }
            else
            {
                sqlstr += "delete wrReportGroup where group_name='" + GroupName + "' and handler='ALL' ";
            }
            DBUtility.ExecuteNonQuery(sqlstr, "WeeklyReport");
        }

        public static DataTable Get_Handler_Detail(string GroupName)
        {
            return DBUtility.GetTable("select distinct MaintMan_Mail,maintman,checked=(case when maintman in (select handler from wrReportGroup where group_name='" + GroupName + "') then 1 else 0 end) from wrsystemsupport order by MaintMan_Mail", "WeeklyReport");
        }
        public static DataTable Get_System_Detail(string GroupName)
        {
            return DBUtility.GetTable("select distinct sysid,groupname,sysname,checked=( case when sysid in(select system_id from wrReportGroup where group_name='" + GroupName + "') then 1 else 0 end) from wrsystemdetail order by groupname,sysname", "WeeklyReport");
        }

        public static DataTable GetGarmentCuttingTransferDetail(string docNO)
        {
            string sql = @"SELECT JOB_ORDER_NO,CUT_LAY_NO,COLOR_CD,SIZE_CD,BUNDLE_NO,SUM(QTY) AS QTY FROM PRD_GARMENT_TRANSFER_DFT 
                           WHERE DOC_NO='" + docNO + "' GROUP BY JOB_ORDER_NO,CUT_LAY_NO,COLOR_CD,SIZE_CD,BUNDLE_NO ";
            return DBUtility.GetTable(sql, "MES");
        }
        public static DataTable GetGarmentTransferDetail(string docNO)
        {
            string sql = @"SELECT JOB_ORDER_NO,MAX(D.SHORT_NAME) AS CUSTOMER_NAME, ISNULL(COLOR_CD,'') AS COLOR_CD,ISNULL(MAX(E.COLOR_DESC),'') AS COLOR_DESC,ISNULL(SIZE_CD,'') AS SIZE_CD,SUM(QTY) AS QTY FROM PRD_GARMENT_TRANSFER_DFT A
                            INNER JOIN JO_HD B ON A.JOB_ORDER_NO=B.JO_NO 
                            INNER JOIN SC_HD C ON B.SC_NO=C.SC_NO
                            INNER JOIN GEN_CUSTOMER D ON C.CUSTOMER_CD=D.CUSTOMER_CD
                            LEFT JOIN SC_COLOR E ON C.SC_NO=E.SC_NO AND E.COLOR_CODE=A.COLOR_CD
                            WHERE DOC_NO='" + docNO + "'  GROUP BY JOB_ORDER_NO,COLOR_CD,SIZE_CD";
            return DBUtility.GetTable(sql, "MES");
        }

        public static DataTable GetGarmentCuttingTransferHeaderInfo(string docNO)
        {
            string sql = @"SELECT HD.*,PRD_LINE.PRODUCTION_LINE_NAME AS PRD_LINE_NAME,
                            PRD_LINE1.PRODUCTION_LINE_NAME AS NEXT_PRD_LINE_NAME,
                            CASE WHEN HD.SUBMIT_DATE IS NULL  THEN HD.CREATE_DATE
	                            ELSE HD.SUBMIT_DATE
	                        END  AS TRX_DATE,
                            CASE WHEN HD.SUBMIT_USER_ID IS NULL THEN USR1.NAME
	                            ELSE USR.NAME
	                        END AS USER_NAME
                            FROM 	PRD_GARMENT_TRANSFER_HD HD
	                            LEFT JOIN GEN_PRODUCTION_LINE PRD_LINE
	                            ON PRD_LINE.FACTORY_CD=HD.FACTORY_CD AND PRD_LINE.PRODUCTION_LINE_CD=HD.PRODUCTION_LINE_CD
	                            LEFT JOIN GEN_PRODUCTION_LINE PRD_LINE1
	                            ON PRD_LINE1.FACTORY_CD=HD.FACTORY_CD AND PRD_LINE1.PRODUCTION_LINE_CD=HD.NEXT_PRODUCTION_LINE_CD
	                            LEFT JOIN MES_USER USR ON USR.USER_ID=HD.SUBMIT_USER_ID
                                LEFT JOIN MES_USER USR1 ON USR1.USER_ID=HD.CREATE_USER_ID
	                        WHERE DOC_NO='" + docNO + "'";

            return DBUtility.GetTable(sql, "MES");
        }

        public static DataTable GetFactoryCd()
        {
            string SQL = "";
            SQL = " select FACTORY_ID,ENG_NAME ";
            SQL = SQL + "	from gen_factory ";
            SQL = SQL + "	where (active='Y' or FACTORY_ID IN ('YMA', 'YMB', 'YMC')) ";
            SQL = SQL + "	ORDER BY FACTORY_ID ";
            return DBUtility.GetTable(SQL, "MES");
        }


        public static DataTable GetProdFactoryCd()
        {
            string SQL = "Select '' AS FACTORY_ID,'' ENG_NAME UNION";
            SQL += " select FACTORY_ID,ENG_NAME ";
            SQL = SQL + "	from gen_factory ";
            SQL = SQL + "	where (active='Y' or FACTORY_ID IN ('YMA', 'YMB', 'YMC')) ";
            SQL = SQL + "	ORDER BY FACTORY_ID ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetJOSAH(string JoNo, string GONo, string FactoryCd, string FromDate, string ToDate, bool NoCut)
        {
            string SQL = "";
            SQL += @"    IF OBJECT_ID('TEMPDB..#TEMP') IS NOT NULL DROP TABLE #TEMP
                                SELECT DISTINCT CU.SHORT_NAME,SC.SC_NO,PO.JO_NO,SC.SAMPLE_ORDER,
                                SC.STYLE_NO,SC.STYLE_DESC,SC.SAM_SO_NO
                                ,SAH=(SELECT DBO.FN_GET_SAH(SC.SC_NO)),
                                DBO.DATE_FORMAT(PO.BUYER_PO_DEL_DATE,'yyyy-MM-dd') AS BUYER_PO_DEL_DATE
                                ,DBO.DATE_FORMAT(PO.PROD_COMPLETION_DATE,'yyyy-MM-dd') AS PROD_COMPLETION_DATE,
                                --ORDER_QTY=(SELECT SUM(QTY) AS ORDER_QTY FROM JO_DT WHERE JO_NO=PO.JO_NO),
                                ORDER_QTY=(SELECT CAST(TOTAL_QTY AS DECIMAL(18,0)) FROM JO_HD WITH(NOLOCK) WHERE JO_NO=PO.JO_NO),
                                '-' + CONVERT(VARCHAR(10),L.PERCENT_SHORT_ALLOWED) + '%/+' + CONVERT(VARCHAR(10),L.PERCENT_OVER_ALLOWED) + '%' AS SHIPALLOWED,
                                L.MARKET_CD,L.SHIP_MODE_CD, PO.GARMENT_TYPE_CD, P.CUST_STYLE_NO,PRINTING,EMBROIDERY ,
                                SC.WASH_TYPE_CD,SC.STYLE_CHN_DESC
                                INTO #TEMP
                                FROM  JO_HD PO WITH(NOLOCK) 
                                INNER JOIN SC_HD SC WITH(NOLOCK) ON SC.SC_NO=PO.SC_NO AND PO.FACTORY_CD = SC.FACTORY_CD
                                INNER JOIN SC_LOT L WITH(NOLOCK) ON L.SC_NO=SC.SC_NO AND L.SC_NO=PO.SC_NO AND L.LOT_NO=PO.LOT_NO
                                LEFT JOIN GEN_CUSTOMER CU WITH(NOLOCK) ON PO.CUSTOMER_CD=CU.CUSTOMER_CD
                                LEFT JOIN JO_HD P WITH(NOLOCK) ON P.JO_NO=PO.JO_NO 
                                WHERE 1=1 ";
            if (FactoryCd != "")
            {
                SQL += " AND PO.FACTORY_CD = '" + FactoryCd + "'";
            }
            if (JoNo != "")
            {
                SQL += " AND PO.JO_NO LIKE '" + JoNo + "%'";
            }
            if (GONo != "")
            {
                SQL += " AND SC.SC_NO LIKE '" + GONo + "%'";
            }
            if (FromDate != "")
            {
                SQL += " AND PO.BUYER_PO_DEL_DATE >= '" + FromDate + "'";
            }
            if (ToDate != "")
            {
                SQL += " AND PO.BUYER_PO_DEL_DATE <= '" + ToDate + "'";
            }
            if (NoCut)
            {
                SQL += @"    AND NOT EXISTS(SELECT 1 FROM  CUT_BUNDLE_HD CB WITH(NOLOCK) WHERE CB.FACTORY_CD = PO.FACTORY_CD AND CB.JOB_ORDER_NO = PO.JO_NO)";
            }
            SQL += @"    SELECT * FROM #TEMP
                                ORDER BY SC_NO,RIGHT(JO_NO,2)";

            return DBUtility.GetTable(SQL, "MES");

        }

        public static DataTable GetJoList(string JoList, string factoryCd, string GARMENT_TYPE_CD, string OUTSOURCE_TYPE, string fromDate, string toDate, bool shipJo, bool NoPost, string RunNO)
        {
            string SQL = "delete MU_Report_All_Data where CREATE_DATE<dateadd(day,-2,getdate()) and LOCK_FLAG='N'; ";
            SQL += "delete MU_Report_All_Data where RunNo='" + RunNO + "'; ";
            SQL += "insert into MU_Report_All_Data(FromDate,ToDate,FTY_CD,JO_NO,SC_NO,BPO_Date,ORDER_QTY,OVER_SHIP, ";
            SQL += "buyer,Wash_Type,STYLE_DESC,PATTERN_TYPE,LOCK_FLAG,CREATE_DATE,RunNo) ";
            SQL += "SELECT FromDate='" + fromDate + "',ToDate='" + toDate + "',FTY_CD='" + factoryCd + "', ";
            SQL += "upper(a.JOB_ORDER_NO) as JOB_ORDER_NO,J.SC_NO,s.buyer_po_del_date,s.total_qty,s.PERCENT_OVER_ALLOWED, ";
            SQL += "c.SHORT_NAME,j.wash_type_cd,(case when a.factory_cd in('GEG','YMG','NBO','CEK','CEG') THEN h.Style_CHN_DESC ELSE  h.STYLE_DESC END) as style_desc,PATTERN_TYPE_CD=isnull(j.fab_pattern,'Solid'), ";
            SQL += "'N' as LOCK_FLAG,Create_date=getdate(),RunNo='" + RunNO + "' ";
            SQL += "FROM dbo.PRD_CUTTING_COMPLETION a ";
            SQL += "inner join jo_hd J  ";
            SQL += "on J.jo_no=a.JOB_ORDER_NO ";
            SQL += "inner join sc_lot S ";
            SQL += "on S.sc_no=j.sc_no ";
            SQL += "and s.lot_no=j.lot_no ";
            SQL += "inner join GEN_CUSTOMER C ";
            SQL += "on C.CUSTOMER_CD=j.CUSTOMER_CD ";
            SQL += "inner join sc_hd h ";
            SQL += "on h.sc_no=j.sc_no ";
            SQL += "WHERE 1=1 ";
            if (factoryCd != "")
            {
                SQL = SQL + " AND a.FACTORY_CD='" + factoryCd + "'";
            }
            if (fromDate != "")
            {
                SQL += " AND a.COMPLETION_DATE >='" + fromDate + "' ";
            }
            if (toDate != "")
            {
                SQL += " AND a.COMPLETION_DATE <DATEADD(DAY,1,'" + toDate + "') ";
            }

            if (JoList != "")
            {
                SQL += " and A.JOB_ORDER_NO in (" + JoList + ") ";
            }
            if (GARMENT_TYPE_CD != "")
            {
                SQL = SQL + " AND J.GARMENT_TYPE_CD='" + GARMENT_TYPE_CD + "'";
            }

            if (OUTSOURCE_TYPE == "STANDARD")
            {
                SQL = SQL + " AND H.SAM_GROUP_CD<>'OUTSOURCE' ";
            }
            if (OUTSOURCE_TYPE == "OUTSOURCE")
            {
                SQL = SQL + " AND H.SAM_GROUP_CD='OUTSOURCE' ";
            }
            SQL += "AND a.COMPLETE_STATUS='Y'; ";
            SQL += " select JO_NO FROM MU_Report_All_Data WHERE RunNo='" + RunNO + "'; ";
            return DBUtility.GetTable(SQL, "MES_UPDATE");
        }

        public static DataTable GetExistsMuList(string factoryCd, string fromDate, string toDate, string RunNO)
        {
            string SQL = "select top 1 RunNo from MU_Report_All_Data where FromDate='" + fromDate + "' and toDate='" + toDate + "' and fty_cd='" + factoryCd + "' and LOCK_FLAG='Y'; ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable LockMudata(string factoryCd, string RunNO, string LType)
        {
            string SQL = "update MU_Report_All_Data set LOCK_FLAG='" + LType + "' where  fty_cd='" + factoryCd + "' and RunNo='" + RunNO + "'; select top 1 create_date from MU_Report_All_Data where  RunNo='" + RunNO + "' and fty_cd='" + factoryCd + "' and LOCK_FLAG='" + LType + "'; ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetSCList(string RunNO)
        {
            string SQL = " SELECT DISTINCT SC_NO FROM MU_Report_All_Data A ";
            SQL += " WHERE A.RunNo='" + RunNO + "'; ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetLocalGisInfo(string JoList, string factoryCd, string GARMENT_TYPE_CD, string OUTSOURCE_TYPE, string fromDate, string toDate, bool shipJo, bool NoPost, string RunNO)
        {
            string SQL = "set session tx_isolation='READ-UNCOMMITTED';";
            SQL = SQL + " UPDATE MU_SHIP_JO_INFO as M, ";
            SQL += " (SELECT C.CT_NO AS JO_NO,SUM(C.QUANTITY) AS SHIP_QTY,MAX(A.SHIP_DATE) AS SHIP_DATE ";
            SQL = SQL + " FROM FTY_INVOICE_HD A,FTY_INVOICE_DT C,MU_SHIP_JO_INFO P ";
            SQL = SQL + " WHERE  C.INVOICE_ID=A.INVOICE_ID  ";
            if (factoryCd != "")
            {
                SQL = SQL + " AND A.FACTORY_CD='" + factoryCd + "'";
            }
            SQL += " and P.Run_No='" + RunNO + "' and P.CT_NO=C.CT_NO";
            SQL = SQL + " GROUP BY C.CT_NO ";
            SQL = SQL + " UNION ALL";
            SQL = SQL + " SELECT C.CT_NO AS JO_NO,SUM(C.QUANTITY) AS SHIP_QTY,MAX(A.SHIP_DATE) AS SHIP_DATE";
            SQL = SQL + " FROM CUST_INVOICE_HD A,CUST_INVOICE_DT C,MU_SHIP_JO_INFO P";
            SQL = SQL + " WHERE  C.INVOICE_ID=A.INVOICE_ID ";
            if (factoryCd != "")
            {
                SQL = SQL + " AND A.FACTORY_CD='" + factoryCd + "'";
            }
            SQL += " and P.Run_No='" + RunNO + "' and P.CT_NO=C.CT_NO";
            SQL = SQL + " GROUP BY C.CT_NO) LJ ";
            SQL = SQL + " SET M.SHIP_QTY=LJ.SHIP_QTY ,M.SHIP_DATE=LJ.SHIP_DATE ";
            SQL = SQL + " WHERE LJ.JO_NO=M.CT_NO ";
            SQL = SQL + " AND M.Run_No='" + RunNO + "' ;";
            SQL = SQL + " SELECT SC_NO,CT_NO AS JO_NO,SHIP_DATE,SHIP_QTY,LASTJO FROM MU_SHIP_JO_INFO WHERE Run_No='" + RunNO + "' and ship_qty>0 ";
            if (!shipJo)
            {
                SQL = SQL + " and exists (select JOB_ORDER_NO from JO_SHIP_STATUS C where SHIP_STATUS='Y' AND FACTORY_CD='" + factoryCd + "' AND C.JOB_ORDER_NO=MU_SHIP_JO_INFO.CT_NO)";
            }

            return DBUtility.GetTable(SQL, "GIS");
        }

        public static DataTable GetLocalMUReportFabricList(string factoryCd, string RunNO, string Datatype)
        {
            string SQL = "";
            if (Datatype == "New")
            {
                SQL += " exec Proc_Generate_MU_Report '" + factoryCd + "','" + RunNO + "';";
            }
            SQL += "if not (select object_id('Tempdb..#temp_T')) is null  drop table #temp_T;";
            SQL += " SELECT DISTINCT CUT_DATE,JO_NO,YPD_JOB_NO,SHIP_DATE,BUYER,NULL AS [Sale_Contract],PPO_NO ,WIDTH,STYLE_DESC,WASH_TYPE,";
            SQL += " PATTERN_TYPE,MU,PPO_ORDER_YDS,ALLOCATED,ISSUED,MARKER_AUDITED,CUT_WSTG_YDS,CUT_WSTG_PER";
            SQL += " ,ISNULL(DEFECT_FAB,0) AS Defect_fabric,ISNULL(DEFECT_PANELS,0)  AS Defect_Panels,ISNULL(ODD_LOSS,0)  AS Odd_loss,ISNULL(SPLICE_LOSS,0)  AS Splice_loss";
            SQL += ",ISNULL(CUT_REJ_PANELS,0)  AS Cutting_Rej_panels,ISNULL(MATCH_LOSS,0)  AS Match_loss,ISNULL(SHORT_YDS,0) AS SHORT_YDS,ISNULL(END_LOSS,0)  AS Endloss";
            SQL += ",ISNULL(SEW_MATCH_LOSS,0)  AS Sewing_match_loss";
            SQL += ",unacc =(ISNULL(CUT_WSTG_YDS,0)-ISNULL(DEFECT_FAB,0)-ISNULL(DEFECT_PANELS,0)-ISNULL(ODD_LOSS,0)-ISNULL(SPLICE_LOSS,0)-ISNULL(CUT_REJ_PANELS,0)-ISNULL(MATCH_LOSS,0)-ISNULL(SHORT_YDS,0)-ISNULL(END_LOSS,0)-ISNULL(SEW_MATCH_LOSS,0))";
            SQL += " ,NULL AS pullout,NULL AS Remarks";
            SQL += " ,LEFTOVER_FAB,LEFTOVER_REASON,";
            SQL += " LEFTOVER_DESC,REMNANT,SRN,RTW,ORDER_QTY,CUT_QTY,PPO_YPD,BULK_YPD,GIVEN_CUT_YPD,";
            SQL += " BULK_MKR_YPD,NULL AS [YPD Var Reason]";
            SQL += " ,YPD_VER,CUT_YPD,CUT_TO_RECEIPT,";
            SQL += " CUT_TO_ORDER,OVER_SHIP,OVER_CUT";
            SQL += " INTO #temp_T";
            SQL += " FROM ";
            SQL += " MU_Report_All_Data";
            SQL += " WHERE FTY_CD='" + factoryCd + "' and RunNo='" + RunNO + "' and JO_NO<>'TTL'; ";
            SQL += " SELECT CUT_DATE AS [Start Cutting Date],JO_NO AS [Fty Job order],YPD_JOB_NO AS [YPD Job No]";
            SQL += " ,SHIP_DATE AS [Ship Date],BUYER AS [Buyer],[Sale_Contract]";
            SQL += " ,PPO_NO AS [PPO NO],WIDTH AS [Fabric Width]";
            SQL += " ,STYLE_DESC AS [Long/ short sleeve],WASH_TYPE AS [Wash Type]";
            SQL += " ,PATTERN_TYPE AS [Fabric Pattern],MU AS [Marker Utilization (%)],PPO_ORDER_YDS AS [PPO order yds]";
            SQL += " ,ALLOCATED AS [Allocated],ISSUED AS [Issued],MARKER_AUDITED AS [Marker Audited],CUT_WSTG_YDS AS [Yds]";
            SQL += " ,CONVERT(VARCHAR(10),CUT_WSTG_PER) + '%'  AS [%]";
            SQL += " ,Defect_fabric AS [Defect fabric],Defect_Panels AS [Defect Panels],Odd_loss AS [Odd loss],Splice_loss AS [Splice loss],";
            SQL += " Cutting_Rej_panels AS [Cutting Rej panels],Match_loss AS [Match loss],SHORT_YDS AS [短码],Endloss AS [Endloss],";
            SQL += " Sewing_match_loss AS [Sewing match loss], unacc, pullout, Remarks";
            SQL += " ,LEFTOVER_FAB AS [Leftover fabric]";
            SQL += " ,LEFTOVER_REASON AS [LeftOver Fab Reason Code],LEFTOVER_DESC AS [LeftOver Fab Reason]";
            SQL += ",REMNANT AS [Remnant],SRN,RTW";
            SQL += " ,ORDER_QTY AS [Order(DZ)],CUT_QTY AS [Cut],PPO_YPD AS [PPO MKR YPD],BULK_YPD AS [BULK NET YPD]";
            SQL += " ,GIVEN_CUT_YPD AS [GIVEN CUT YPD],BULK_MKR_YPD AS [BULK MKR YPD],[YPD Var Reason]";
            SQL += " ,YPD_VER AS [YPD Var]";
            SQL += " ,CUT_YPD AS [CUT YPD],CONVERT(VARCHAR(10),CUT_TO_RECEIPT) + '%' as [Cut-to-Receipt]";
            SQL += " ,CONVERT(VARCHAR(10),CUT_TO_ORDER) + '%'  as [Cut-to-Order]";
            SQL += " ,CONVERT(VARCHAR(10),OVER_SHIP) + '%' as [Buyer's max over shipment allowance]";
            SQL += " ,CONVERT(VARCHAR(10),OVER_CUT) + '%' as [Buyer's max over cut allowance]";
            SQL += " FROM #temp_T";
            SQL += " UNION ALL";
            SQL += " SELECT NULL,'Total/Weighted Average:',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL";
            SQL += " ,CAST(SUM(ISNULL(MU,0))/COUNT(*) AS DECIMAL(18,2)) ";
            SQL += " ,CAST(SUM(ISNULL(PPO_ORDER_YDS,0))/COUNT(*) AS DECIMAL(18,2)),CAST(SUM(ISNULL(ALLOCATED,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(ISSUED,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(MARKER_AUDITED,0))/COUNT(*) AS DECIMAL(18,2)),CAST(SUM(ISNULL(CUT_WSTG_YDS,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CONVERT(VARCHAR(10),CAST(SUM(ISNULL(CUT_WSTG_PER,0))/COUNT(*) AS DECIMAL(18,2))) + '%'";
            SQL += " ,CAST(SUM(ISNULL(Defect_fabric,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(Defect_Panels,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(Odd_loss,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(Splice_loss,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(Cutting_Rej_panels,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(Match_loss,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(SHORT_YDS,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(Endloss,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(Sewing_match_loss,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(unacc,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(pullout,0))/COUNT(*) AS DECIMAL(18,2)) ,";
            SQL += " CAST(SUM(ISNULL(Remarks,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(LEFTOVER_FAB,0))/COUNT(*) AS DECIMAL(18,2)),NULL,NULL,CAST(SUM(ISNULL(REMNANT,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(SRN,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(RTW,0))/COUNT(*) AS DECIMAL(18,2)),CAST(SUM(ISNULL(ORDER_QTY,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(CUT_QTY,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(PPO_YPD,0))/COUNT(*) AS DECIMAL(18,2)),CAST(SUM(ISNULL(BULK_YPD,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,CAST(SUM(ISNULL(GIVEN_CUT_YPD,0))/COUNT(*) AS DECIMAL(18,2)),CAST(SUM(ISNULL(BULK_MKR_YPD,0))/COUNT(*) AS DECIMAL(18,2))";
            SQL += " ,NULL,CAST(SUM(ISNULL(YPD_VER,0))/COUNT(*) AS DECIMAL(18,2)),NULL,NULL";
            SQL += " ,CONVERT(VARCHAR(10),CAST(SUM(ISNULL(CUT_TO_ORDER,0))/COUNT(*) AS DECIMAL(18,2))) + '%'";
            SQL += " ,CONVERT(VARCHAR(10),CAST(SUM(ISNULL(OVER_SHIP,0))/COUNT(*) AS DECIMAL(18,2))) + '%',NULL";
            SQL += " FROM #temp_T";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetLocalMUReportGarmentList(string factoryCd, string RunNO, string fromdate, string todate, string Datatype)
        {
            string SQL = "";
            if (Datatype == "New")
            {
                SQL += " exec Proc_Generate_MU_Report '" + factoryCd + "','" + RunNO + "';";
            }
            SQL += "IF OBJECT_ID('tempdb..#temp_T') IS NOT NULL ";
            SQL += "    DROP TABLE #temp_T;";
            SQL += " SELECT IDENTITY (INT,1,1) as ID,SHIP_DATE,LINE AS [TEAM],JO_NO,BUYER,ORDER_QTY,CUT_QTY";
            SQL += " ,SHIPPED_QTY,OVER_CUT,OVER_SHIP,CAST(SHIPPED_QTY/ORDER_QTY * 100 AS DECIMAL(18,2)) AS [S/O]";
            SQL += " ,[SAMPLE],PULL_OUT,LEFTOVER_A,LEFTOVER_B";
            SQL += ",ISNULL(SEWDFC_QTY,0) AS [LEFTOVER_C_1],ISNULL(MTRMS_QTY,0) AS [LEFTOVER_C_2]";
            SQL += ",ISNULL(SEWDFD_QTY,0) AS [LEFTOVER_C_3],ISNULL(SEWDF_QTY,0) AS [LEFTOVER_C_4]";
            SQL += ",ISNULL(PRTDF_QTY,0) AS [LEFTOVER_C_5],UNACC_GMT";
            SQL += " INTO #temp_T";
            SQL += " FROM MU_Report_All_Data ";
            SQL += " WHERE FTY_CD='" + factoryCd + "'";
            SQL += " AND RunNo = '" + RunNO + "'";
            if (fromdate != "" && todate != "")
            {
                SQL += " and SHIP_DATE >='" + fromdate + "' and SHIP_DATE <='" + todate + "'";
            }
            SQL += " and JO_NO<>'TTL'; ";

            SQL += " SELECT DISTINCT CONVERT(VARCHAR(10),SHIP_DATE,120) AS SHIP_DATE";
            SQL += " ,[TEAM],JO_NO, BUYER,";
            SQL += " ORDER_QTY AS [Order Qty],";
            SQL += " CUT_QTY AS [Cut Qty],";
            SQL += " SHIPPED_QTY as [Ship Qty],OVER_CUT AS [Buyer's max over cut allowance]";
            SQL += ",OVER_SHIP AS [Buyer's max over shipment allowance],";
            SQL += " CONVERT(VARCHAR(20),[S/O]) +'%' AS [S/O]";
            SQL += " ,[SAMPLE] as [Sample],PULL_OUT AS [Pull-out],LEFTOVER_A AS [Grade A],LEFTOVER_B AS [Grade B]";
            SQL += " ,[LEFTOVER_C_1],[LEFTOVER_C_2],[LEFTOVER_C_3],[LEFTOVER_C_4],[LEFTOVER_C_5]";
            SQL += " ,UNACC_GMT as [TTL:Unacc Gmt]";
            SQL += " FROM #temp_T";

            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetLocalOrderQty(DbConnection eelConn)
        {
            string SQL = "         SELECT SC_NO,sum(ORDER_QTY) as ORDER_QTY,sum(SC_QTY) as SC_QTY FROM ";
            SQL = SQL + "         (SELECT SC_NO,NVL(SUM(order_qty),0) AS ORDER_QTY,0 AS SC_QTY FROM ";
            SQL = SQL + "        ( SELECT s.sc_no,SUM (d.order_qty) AS order_qty ";
            SQL = SQL + "        FROM ppo_hd h, ppo_dt d, fab_lib f,         ";
            SQL = SQL + "        (select distinct sc_no,fabric_type_cd,pPO_NO from scx_joi_go_fabric aa  ";
            SQL = SQL + "          where exists (select f1 from rpt_tmp where f1=aa.sc_no)";
            SQL = SQL + "            ) s  ";
            SQL = SQL + "        WHERE h.pPO_NO = d.pPO_NO AND  h.flag != 'X'  ";
            SQL = SQL + "        AND d.fab_ref_cd = f.fab_ref_cd AND h.status IN ('L2', 'R')  ";
            SQL = SQL + "        AND H.PPO_NO=s.pPO_NO  and d.fabric_type_cd=s.fabric_type_cd and s.fabric_type_cd in ('B','BD')  ";
            SQL = SQL + "         GROUP BY s.sc_no  ";
            SQL = SQL + "         UNION ALL SELECT s.sc_no,SUM (pl.order_qty) AS order_qty  ";
            SQL = SQL + "         FROM ppo_hd a, ppox_fab_item b,ppox_lot pl, ";
            SQL = SQL + "        (select distinct sc_no,fabric_type_cd,pPO_NO from scx_joi_go_fabric aa  ";
            SQL = SQL + "          where exists (select f1 from rpt_tmp where f1=aa.sc_no)";
            SQL = SQL + "          ) s  ";
            SQL = SQL + "         WHERE a.pPO_NO = b.pPO_NO AND b.fab_item_id = pl.fab_item_id AND a.flag = 'X'  ";
            SQL = SQL + "         AND a.status IN ('L2', 'R') AND  b.status != 'C'  ";
            SQL = SQL + "         AND A.PPO_NO=s.pPO_NO and b.fabric_type_cd=s.fabric_type_cd and s.fabric_type_cd in ('B','BD') ";
            SQL = SQL + "          GROUP BY  s.sc_no,b.fabric_type_cd  ";
            SQL = SQL + "          UNION ALL SELECT s.sc_no,sum(d.qty) AS order_qty  ";
            SQL = SQL + "          FROM fab_sample_order_hd a, fab_so_combo b,fab_so_product c,fab_so_combo_qty d, ";
            SQL = SQL + "        (select distinct sc_no,fabric_type_cd,pPO_NO from scx_joi_go_fabric aa  ";
            SQL = SQL + "          where exists (select f1 from rpt_tmp where f1=aa.sc_no)";
            SQL = SQL + "          ) s  ";
            SQL = SQL + "          WHERE d.req_prod_cd = c.req_prod_cd and  a.fab_so_no = b.fab_so_no AND b.fab_so_no = d.fab_so_no  ";
            SQL = SQL + "          AND b.combo_seq = d.combo_seq AND a.status IN ('P', 'S', 'D')  ";
            SQL = SQL + "          AND A.fab_so_no=s.pPO_NO and c.fabric_part=s.fabric_type_cd and s.fabric_type_cd in ('B','BD') ";
            SQL = SQL + "         group by a.fab_so_no,s.sc_no     ) AAA  ";
            SQL = SQL + "        GROUP BY SC_NO  ";
            SQL = SQL + " UNION ALL ";
            SQL = SQL + " SELECT SC.SC_NO,0 AS ORDER_QTY,Round(SUM(SC.TOTAL_QTY)/12,2) AS SC_QTY ";
            SQL = SQL + "FROM ";
            SQL = SQL + "(select DISTINCT SC_NO from scx_joi_go_fabric a ";
            SQL = SQL + "   where exists(select pPO_NO from scx_joi_go_fabric aa where ";
            SQL = SQL + "   exists (select f1 from rpt_tmp where f1=aa.sc_no) AND FABRIC_TYPE_CD in ('B','BD')  and aa.pPO_NO=a.pPO_NO) ";
            SQL = SQL + "AND A.FABRIC_TYPE_CD in ('B','BD')) PS,SC_HD SC ";
            SQL = SQL + "WHERE PS.SC_NO=SC.SC_NO  GROUP BY SC.SC_NO) PP WHERE 1=1 GROUP BY SC_NO order by sc_no";
            return DBUtility.GetTable(SQL, eelConn);
        }

        public static DataTable GetStandardLeftGMAndSRNAndRTW(string factoryCd, DbConnection invConn)
        {//弃用,用GetStandardLeftGM 和 GetStandardLeftSRNAndRTW两个方法代替;
            string SQL = " SELECT jo_no,SUM(RTW_QTY_I) AS RTW_QTY_I,SUM(RTW_QTY) AS RTW_QTY,SUM(SRN_QTY_I) AS SRN_QTY_I,";
            SQL = SQL + " SUM(SRN_QTY) AS SRN_QTY,SUM(gmt_qty_a) AS gmt_qty_a,SUM(gmt_qty_b) AS gmt_qty_b,SUM(sew_qty_b) AS sew_qty_b,SUM(wash_qty_b) AS wash_qty_b ";
            SQL = SQL + " FROM ";
            SQL = SQL + "(SELECT jo_no, 0 AS RTW_QTY_I,0 AS RTW_QTY,SUM(RATIO*QTY) AS SRN_QTY_I,";
            SQL = SQL + " SUM(QTY) AS SRN_QTY,0 AS gmt_qty_a,0 AS gmt_qty_b,0 AS sew_qty_b,0 AS wash_qty_b ";
            SQL = SQL + "    FROM (SELECT   LN.job_order_no AS JO_NO, ";
            SQL = SQL + "                   (CASE ";
            SQL = SQL + "                       WHEN po.garment_type_cd = 'K' OR l.grade = 'A' or l.grade is null ";
            SQL = SQL + "                          THEN 1 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade IN ('A*', 'B*') ";
            SQL = SQL + "                          THEN 0.9 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade = 'B' ";
            SQL = SQL + "                          THEN 0.93 ";
            SQL = SQL + "                       ELSE 0 ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                   ) AS ratio, ";
            SQL = SQL + "                   SUM (LN.trans_qty) AS qty ";
            SQL = SQL + "              FROM invsubmat.inv_issue_hd hd, ";
            SQL = SQL + "                   invsubmat.inv_issue_line LN, ";
            SQL = SQL + "                   invsubmat.inv_srn_line sln, ";
            SQL = SQL + "                   invsubmat.inv_store s, ";
            SQL = SQL + "                   inv_item_lot l, ";
            SQL = SQL + "                   po_hd po ";
            SQL = SQL + "             WHERE hd.issue_hd_id = LN.issue_hd_id ";
            SQL = SQL + "               AND LN.store_cd = s.store_cd ";
            SQL = SQL + "               AND LN.lot_id = l.lot_id(+) ";
            SQL = SQL + "               AND LN.srn_line_id = sln.srn_line_id ";
            SQL = SQL + "               AND exists (select f1 from escmreader.rpt_tmp where f1= LN.job_order_no) ";
            SQL = SQL + "               AND sln.job_order_no = LN.job_order_no ";
            SQL = SQL + "               AND s.inv_fty_cd = '" + factoryCd + "' ";
            SQL = SQL + "               AND hd.status = 'F' ";
            SQL = SQL + "               AND hd.item_type_cd = 'F' ";
            SQL = SQL + "               AND sln.main_body_flag = 'Y' ";
            SQL = SQL + "               AND NVL(HD.PROD_LINE_CD,'OK')<>'YMG-SAMPLE(Special)'";
            SQL = SQL + "               AND po.JO_NO = LN.job_order_no ";
            SQL += " and NVL(s.warehouse_type_cd,'O')<>'C'";
            //SQL = SQL + " AND (NVL(s.warehouse_type_cd,'O')<>'C' or (NVL(s.warehouse_type_cd,'O')='C' and  s.store_cd like '%C2'))";
            SQL = SQL + "          GROUP BY LN.job_order_no, l.grade, po.garment_type_cd) ";
            SQL = SQL + "GROUP BY jo_no UNION ALL ";
            SQL = SQL + " SELECT   jo_no, SUM(RATIO*QTY) AS RTW_QTY_I,SUM(QTY) AS RTW_QTY ,0 AS SRN_QTY_I,0 AS SRN_QTY, ";
            SQL = SQL + " 0 AS gmt_qty_a,0 AS gmt_qty_b,0 AS sew_qty_b,0 AS wash_qty_b";
            SQL = SQL + "    FROM (SELECT   LN.job_order_no AS JO_NO, ";
            SQL = SQL + "                   (CASE ";
            SQL = SQL + "                       WHEN po.garment_type_cd = 'K' OR l.grade = 'A' or l.grade is null ";
            SQL = SQL + "                          THEN 1 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade IN ('A*', 'B*') ";
            SQL = SQL + "                          THEN 0.9 ";
            SQL = SQL + "                       ELSE CASE ";
            SQL = SQL + "                       WHEN l.grade = 'B' ";
            SQL = SQL + "                          THEN 0.93 ";
            SQL = SQL + "                       ELSE 0 ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                    END ";
            SQL = SQL + "                   ) AS ratio, ";
            SQL = SQL + "                   SUM (LN.trans_qty) AS qty ";
            SQL = SQL + "              FROM invsubmat.inv_rec_hd hd, ";
            SQL = SQL + "                   invsubmat.inv_rec_line LN, ";
            SQL = SQL + "                   invsubmat.inv_srn_line sln, ";
            SQL = SQL + "                   invsubmat.inv_issue_line sn, ";
            SQL = SQL + "                   invsubmat.inv_store s, ";
            SQL = SQL + "                   inv_item_lot l, ";
            SQL = SQL + "                   invsubmat.inv_trans_code tc, ";
            SQL = SQL + "                   po_hd po ";
            SQL = SQL + "             WHERE hd.rec_hd_id = LN.rec_hd_id ";
            SQL = SQL + "               AND hd.store_cd = s.store_cd ";
            SQL = SQL + "               AND LN.lot_id = l.lot_id(+) ";
            SQL = SQL + "               AND exists  (select f1 from escmreader.rpt_tmp where f1=LN.job_order_no) ";
            SQL = SQL + "               AND sn.srn_line_id = sln.srn_line_id ";
            SQL = SQL + "               AND LN.issue_line_id = sn.issue_line_id ";
            SQL = SQL + "               AND s.inv_fty_cd = '" + factoryCd + "' ";
            SQL = SQL + "               AND hd.status = 'F' ";
            SQL = SQL + "               AND hd.item_type_cd = 'F' ";
            SQL = SQL + "               AND hd.trans_cd = tc.trans_cd ";
            SQL = SQL + "               AND tc.trans_type_cd = 'RTW' ";
            SQL = SQL + "               AND sln.main_body_flag = 'Y' ";
            SQL = SQL + "               AND po.JO_NO = LN.job_order_no ";
            SQL = SQL + " and s.warehouse_type_cd<>'C'";
            //SQL = SQL + " AND (NVL(s.warehouse_type_cd,'O')<>'C' or (NVL(s.warehouse_type_cd,'O')='C' and  s.store_cd like '%C2'))";
            SQL = SQL + "          GROUP BY LN.job_order_no, l.grade, po.garment_type_cd) ";
            SQL = SQL + "GROUP BY jo_no ";
            SQL = SQL + " UNION ALL ";
            SQL = SQL + " SELECT  job_order_no,0 AS RTW_QTY_I,0 AS RTW_QTY,0 AS SRN_QTY_I,0 AS SRN_QTY,";
            SQL = SQL + "SUM(gmt_qty_a) AS gmt_qty_a,SUM(gmt_qty_b) AS gmt_qty_b,SUM(sew_qty_b) AS sew_qty_b,SUM(wash_qty_b) AS wash_qty_b";
            SQL = SQL + " FROM (";
            SQL = SQL + " SELECT l.reference_no AS job_order_no,(case when l.grade='A' then sum(nvl(l.qty,0)) else 0 end) as gmt_qty_a, ";
            SQL = SQL + "(case when l.grade='B' then sum(nvl(l.qty,0)) else 0 end) as gmt_qty_b,  ";
            SQL = SQL + "(case when l.grade='B' then sum(nvl(l.sew_qty,0)) else 0 end) as sew_qty_b, ";
            SQL = SQL + "(case when l.grade='B' then sum(nvl(l.wash_qty,0)) else 0 end) as wash_qty_b ";
            SQL = SQL + "  FROM inv_trans_hd h, inv_trans_lines l, inventory.inv_store_codes s, ";
            SQL = SQL + "       inv_stock_class c ";
            SQL = SQL + " WHERE h.trans_header_id = l.trans_header_id ";
            SQL = SQL + "   AND h.from_store_cd = s.store_cd ";
            SQL = SQL + "   AND s.stock_class_cd = c.stock_class_cd ";
            SQL = SQL + "   AND NVL(h.first_receipt_flag, 'N') = 'Y' ";
            SQL = SQL + "   AND c.stock_group_cd = 'L' ";
            SQL = SQL + "   AND l.grade IN ('A', 'B') ";
            SQL = SQL + "   AND exists (select f1 from escmreader.rpt_tmp where f1=l.reference_no) ";
            SQL = SQL + "   and h.doc_no like '" + factoryCd + "%' ";
            SQL = SQL + "   group by l.reference_no,l.grade ) A";
            SQL = SQL + " WHERE 1=1 ";
            SQL = SQL + " GROUP BY job_order_no";
            SQL = SQL + ") AL WHERE 1=1    GROUP BY JO_NO";

            return DBUtility.GetTable(SQL, invConn);
        }

        public static DataTable GetLocalLeftOver(string factoryCd, DbConnection invConn)
        {
            string SQL = " select PO_NO as PPO_NO,reason_cd,SUM(RATIO*Leftover_QTY) AS Leftover_QTY ";
            SQL = SQL + "from ";
            SQL = SQL + "(SELECT   lot.PO_NO,lot.reason_cd,";
            SQL = SQL + " (case when ppo.FABRIC_NATURE_CD='K' or lot.grade='A' then 1 else ";
            SQL = SQL + "         case when lot.grade in('A*','B*') THEN 0.9 else ";
            SQL = SQL + "         case when lot.grade ='B' then 0.93 else 0 end end end) as ratio, ";
            SQL = SQL + "sum(nvl(on_hand_qty,0)) as Leftover_QTY   ";
            SQL = SQL + "    FROM INVSUBMAT.inv_stock_items_v a,   ";
            SQL = SQL + "         inv_item i,   ";
            SQL = SQL + "         inv_item_lot lot,   ";
            SQL = SQL + "         INVSUBMAT.inv_store s, ";
            SQL = SQL + "         PPO_HD PPO   ";
            SQL = SQL + "   WHERE a.inv_item_id = i.inv_item_id ";
            SQL = SQL + "     AND a.lot_id = lot.lot_id(+) ";
            SQL = SQL + "        AND a.on_hand_qty > 0 ";
            SQL = SQL + "     AND a.store_cd = s.store_cd ";
            SQL = SQL + "     AND i.item_type_cd = 'F' ";
            SQL = SQL + "     AND exists (select f1 from escmreader.rpt_tmp where f1= lot.JO_NO) ";
            SQL = SQL + "     AND s.stock_class_cd in ('L1','L2','L3','L4') ";
            SQL = SQL + "     AND a.inv_fty_cd ='" + factoryCd + "' ";
            SQL = SQL + "     AND s.active = 'Y' ";
            SQL = SQL + "     and fabric_type='B' ";
            SQL = SQL + "     AND PPO.PPO_NO=LOT.PO_NO ";
            SQL = SQL + "     group by lot.PO_NO,lot.reason_cd,lot.grade,ppo.FABRIC_NATURE_CD) ";
            SQL = SQL + "group by  PO_NO,reason_cd ";

            return DBUtility.GetTable(SQL, invConn);

        }
        public static DataTable GetJO(string trxDate, string status, string customerCd, string scNo, string factoryCd)
        {
            string SQL = "";
            SQL = " SELECT DISTINCT top 50   A.JOB_ORDER_NO, SC.SC_NO, ";
            SQL = SQL + "        dbo.DATE_FORMAT(A.TRX_DATE,'MM/DD/YYYY') TRX_DATE, ";
            SQL = SQL + "        A.JO_CLEAR_FLAG,CU.NAME + '(' + PO.CUSTOMER_CD + ')' CUSTOMER ";
            SQL = SQL + "        FROM PRD_JO_OUTPUT_TRX A INNER JOIN JO_HD PO ON ";
            SQL = SQL + "        A.JOB_ORDER_NO=PO.JO_NO LEFT JOIN SC_HD SC ON PO.SC_NO=SC.SC_NO ";
            SQL = SQL + "        LEFT JOIN GEN_CUSTOMER CU ON PO.CUSTOMER_CD=CU.CUSTOMER_CD WHERE ";
            SQL = SQL + "        A.FACTORY_CD='" + factoryCd + "' ";
            if (trxDate != "")
            {
                SQL += " and (CONVERT(varchar(10), A.TRX_DATE, 120 )='" + trxDate + "')";
            }
            if (status != "")
            {
                SQL += " and A.JO_CLEAR_FLAG='" + status + "'";
            }
            if (customerCd != "")
            {
                SQL += " and PO.CUSTOMER_CD='" + customerCd + "'";
            }
            if (scNo != "")
            {
                SQL += " and SC.SC_NO like '" + scNo + "%'";
            }
            SQL += " --order by a.job_order_no  ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetJO(string JoNo, string scNo, string factoryCd)
        {
            string SQL = "";
            SQL += " SELECT SC_NO,JO_NO FROM JO_HD WHERE STATUS<>'D' ";
            if (!scNo.Equals(""))
            {
                SQL += " AND SC_NO = '" + scNo + "'";
            }
            if (!JoNo.Equals(""))
            {
                //SQL += "AND JO_NO = '" + JoNo + "' ";
                SQL += " AND JO_NO IN (SELECT * FROM FN_SPLIT_STRING_TB('"+JoNo+"',',')) ";
            }
            //Modified by MF on 20160215, JO Combination - bug fix for not display original JO data
            SQL += " AND FACTORY_CD= '" + factoryCd + "' ";
            SQL += "   AND NOT EXISTS (SELECT 1 FROM JO_COMBINE_MAPPING WHERE ORIGINAL_JO_NO=JO_HD.JO_NO) ";
            //SQL += " AND FACTORY_CD= '" + factoryCd + "'";
            //End of modified by MF on 20160215, JO Combination - bug fix for not display original JO data
            return DBUtility.GetTable(SQL, "MES");
        }


        //modify by xuzm on 2017/03/30 SQL阻塞 加上Nolock和去掉JO的前面%
        //Modify by blake on 2015/06/27 for NBO and YMG enhancement
        public static DataTable GetBundleTicketDetail(string ftyCd, string layNoFrom, string layNoTo, string jobOrderNo, string dateFrom, string dateTo, bool cbClearBundle)
        {
            string SQL = "";
            SQL = " select A.JOB_ORDER_NO,A.CREATE_USER_ID,ISNULL(d.new_lay_no,A.LAY_NO) AS  LAY_NO,  CASE WHEN A.FACTORY_CD = 'NBO' THEN ISNULL(NEW_SEQ_LETTER + NEW_SEQ_NO,A.BUNDLE_NO) ELSE ISNULL(A.new_seq_no, A.BUNDLE_NO) END AS BUNDLE_NO,A.COLOR_CD, isnull(A.NEWSIZE_CD,A.SIZE_CD) as SIZE_CD,A.QTY AS ";
            SQL = SQL + "        BUNDLE_QTY, isnull(B.PLYS,'') AS BUNDLE_ORIGINAL_QTY,isnull(B.BATCH_NO,'') AS BATCH_NO,isnull(B.SHADE_LOT,'') AS SHADE_LOT, ";
            SQL = SQL + "        isnull(B.ROLL_ID,'') AS ROLL_ID,A.CUT_LINE,C.PRODUCTION_LINE_NAME,convert(varchar(10),A.ACTUAL_PRINT_DATE,120) PRINTING_DATE, ";
            SQL = SQL + @"   CASE WHEN A.TRX_TYPE='NM' THEN 'NM(Normal)'ELSE CASE WHEN A.TRX_TYPE='RD' THEN 'RD(Reduce)' 
                             ELSE CASE WHEN A.TRX_TYPE='NR' THEN 'NR(Normal Remove)' ELSE CASE WHEN A.TRX_TYPE='RM' THEN 'RM(Remove)' END END END END AS TRX_TYPE";
            SQL += @" FROM   CUT_BUNDLE_HD AS A WITH(NOLOCK)
                    LEFT JOIN dbo.CUT_LAY_DT AS B WITH(NOLOCK) ON A.COLOR_CD = B.COLOR_CD
                                                     AND A.LAY_DT_ID = B.LAY_DT_ID
                                                     AND A.LAY_TRANS_ID = B.LAY_TRANS_ID
                    INNER JOIN dbo.GEN_PRODUCTION_LINE AS C WITH(NOLOCK) ON a.PRODUCTION_LINE_CD = c.PRODUCTION_LINE_CD and C.factory_cd='" + ftyCd + "' LEFT JOIN dbo.CUT_LAY AS d ON a.JOB_ORDER_NO = d.JOB_ORDER_NO AND a.LAY_NO =d.LAY_NO";
            SQL += " WHERE 1=1";

            if (ftyCd != "")
            {
                SQL += " and a.factory_cd='" + ftyCd + "'";
            }
            if (layNoFrom != "")
            {
                SQL += " and ISNULL(d.NEW_LAY_NO,a.LAY_NO) >='" + layNoFrom + "'";
            }
            if (layNoTo != "")
            {
                SQL += " and ISNULL(d.NEW_LAY_NO,a.LAY_NO) <='" + layNoTo + "'";
            }
            if (jobOrderNo != "")
            {
                SQL += " and a.job_order_no LIKE '" + jobOrderNo + "%'";
            }
            if (dateFrom != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)>=convert(varchar(10),cast('" + dateFrom + "' as datetime),120)";
            }
            if (dateTo != "")
            {
                SQL += " and convert(varchar(10),ACTUAL_PRINT_DATE,120)<=convert(varchar(10),cast('" + dateTo + "' as datetime),120)";
            }
            if (cbClearBundle == false)
            {
                SQL += " and TRX_TYPE in ('NM','RD')";
            }
            SQL += " AND STATUS='Y' "; //Added By ZouShiChang On 2014.02.10 
            SQL += "order by a.bundle_no";
            return DBUtility.GetTable(SQL, "MES");
        }


        public static DataTable GetCuttingDateReport(string fromDate, string toDate, string joNo, string gmtType, string buyerCd, string factoryCd, bool line, string prodGroup)
        {
            string SQL = "";

            //Added by MunFoong on 2014.07.24, MES-139
            if (prodGroup != "")
            {
                SQL += " DECLARE @S NVARCHAR(MAX)";
                SQL += " SELECT @S = SYSTEM_VALUE FROM GEN_SYSTEM_SETTING WITH(NOLOCK) WHERE FACTORY_CD = '" + factoryCd + "' AND SYSTEM_KEY = '" + prodGroup + "'";
            }
            //End od Added by MunFoong on 2014.07.24, MES-139 

            SQL = SQL + "   SELECT CUTDATE,BUYER_PO_DEL_DATE, Fab_Pattern,SAM_SO_NO,";
            SQL = SQL + "   STYLE_CHN_DESC,WASH_TYPE_CD,PRINTING,EMBROIDERY,FUSING_FLAG,BUYERNAME, GONO,JONO ";
            if (line)
            {
                SQL = SQL + "   ,PRODUCTION_LINE_CD=(CASE WHEN v.PRODUCTION_LINE_CD =ISNULL(LINE.PRODUCTION_LINE_NAME,'') ";
                SQL = SQL + "        THEN  v.PRODUCTION_LINE_CD  ELSE v.PRODUCTION_LINE_CD + '(' + ISNULL(LINE.PRODUCTION_LINE_NAME,'') + ')' END)";
            }
            //控制订单数为0的为情况
            SQL = SQL + "  ,ORDERQTY,JOQTY,TTL_QTY,UPTOD_QTY,case when ORDERQTY=0 then 0 else  round(TTL_QTY/ORDERQTY*100,2) end  as Cut2Order,COMPLETION_DATE";
            //不控制订单数为0的为情况
            //SQL = SQL + "  ,ORDERQTY,JOQTY,TTL_QTY,UPTOD_QTY, round(TTL_QTY/ORDERQTY*100,2)  as Cut2Order,COMPLETION_DATE";
            SQL = SQL + "  from (";
            SQL = SQL + "  SELECT A.CUTDATE,B.BUYER_PO_DEL_DATE,ISNULL(B.SAM_SO_NO,'') SAM_SO_NO,Fab_Pattern, ";
            if (line)
            {
                SQL = SQL + "  Production_line_cd,";
            }
            SQL = SQL + "   ISNULL(B.STYLE_CH_DESC,'') STYLE_CHN_DESC,B.WASH_TYPE_CD,B.PRINTING,B.EMBROIDERY,ISNULL(B.FUSING_FLAG,'') FUSING_FLAG, ";
            SQL = SQL + "   B.BUYERNAME,B.GONO,B.JO_no JONO,B.ORDERQTY,A.JOQTY,";
            SQL = SQL + "     TTL_QTY=(SELECT SUM(QTY) FROM CUT_BUNDLE_HD WHERE FACTORY_CD='" + factoryCd + "'";
            SQL = SQL + "     AND JOB_ORDER_NO=B.JO_no and status='Y'),"; //Added by MF on 20150528, added status='Y'filter for CUT_BUNDLE_HD
            SQL = SQL + "     UPTOD_QTY=(SELECT SUM(QTY) FROM CUT_BUNDLE_HD WHERE FACTORY_CD='" + factoryCd + "'";
            SQL = SQL + "     AND JOB_ORDER_NO=B.JO_no and status='Y' ";  //Added by MF on 20150528, added status='Y'filter for CUT_BUNDLE_HD
            if (toDate != "")
            {
                SQL += " and actual_print_date <(cast('" + toDate + "' as datetime)+1)";
            }
            SQL = SQL + " ),";
            SQL = SQL + "     c.COMPLETION_DATE ";
            SQL = SQL + "      FROM (SELECT ";
            SQL = SQL + "      JO_hd.JO_no, JO_HD.BUYER_PO_DEL_DATE, SC_HD.WASH_TYPE_CD, ";
            SQL = SQL + "      SC_HD.PRINTING, SC_HD.EMBROIDERY, SC_HD.SAM_SO_NO, ";
            SQL = SQL + "      SC_HD.FUSING_FLAG,JO_HD.Fab_Pattern,STYLE_HD.STYLE_DESC AS STYLE_CH_DESC, ";
            SQL = SQL + "      SUM(JO_DT.QTY) ORDERQTY, GEN_CUSTOMER.SHORT_NAME BUYERNAME, ";
            SQL = SQL + "      JO_HD.SC_NO GONO FROM JO_HD, JO_DT, SC_HD, GEN_CUSTOMER,STYLE_HD ";
            SQL = SQL + "      WHERE sc_hd.sc_no = JO_hd.sc_no AND JO_DT.JO_NO =JO_HD.JO_NO and ";
            SQL = SQL + "      JO_HD.JO_NO IN(SELECT DISTINCT JOB_ORDER_NO FROM cut_bundle_hd ";
            SQL = SQL + "      WHERE 1=1 ";

            //Added by MunFoong on 2014.07.24, MES-139
            if (prodGroup != "")
            {
                SQL += " and CUT_LINE IN (select FNField FROM DBO.FN_SPLIT_STRING_TB(@S,','))";
            }
            //End of added by MunFoong on 2014.07.24, MES-139

            if (joNo != "")
            {
                SQL += " and JOB_ORDER_NO='" + joNo + "'";
            }
            else
            {
                if (fromDate != "")
                {
                    SQL += " and actual_print_date >(cast('" + fromDate + "' as datetime))";
                }
                if (toDate != "")
                {
                    SQL += " and actual_print_date <(cast('" + toDate + "' as datetime)+1)";
                }
            }
            SQL = SQL + "      ) ";
            if (joNo != "")
            {
                SQL += " and JO_hd.JO_no='" + joNo + "'";
            }
            if (gmtType != "")
            {
                SQL += " AND JO_hd.garment_type_cd='" + gmtType + "'";
            }
            if (buyerCd != "")
            {
                SQL += " and JO_hd.CUSTOMER_CD='" + buyerCd + "' and B.CUSTOMER_CD='" + buyerCd + "'";
            }
            SQL = SQL + "      AND sc_hd.customer_cd = gen_customer.customer_cd AND ";
            SQL = SQL + "      STYLE_HD.STYLE_NO=SC_HD.STYLE_NO AND ";
            SQL = SQL + "      STYLE_HD.STYLE_REV_NO=SC_HD.STYLE_REV_NO GROUP BY JO_hd.JO_no, ";
            SQL = SQL + "      gen_customer.short_name, JO_hd.sc_no ,JO_HD.Fab_Pattern, JO_hd.BUYER_PO_del_date, ";
            SQL = SQL + "      sc_hd.wash_type_cd, sc_hd.printing, sc_hd.embroidery, ";
            SQL = SQL + "      sc_hd.sam_so_no, sc_hd.Fusing_flag, STYLE_HD.STYLE_DESC) B ";
            //Added by ZouShiChang On 2013.09.06 Start MES024
            /*
            SQL = SQL + "      left join (SELECT CUT_LAY.JOB_ORDER_NO,";
            if (line)
            {
                SQL = SQL + "   CUT_BUNDLE_HD.Production_line_cd,";
            }
            SQL = SQL + "   SUM(CUT_BUNDLE_HD.QTY) AS ";
            SQL = SQL + "   JOQTY,DBO.DATE_FORMAT(MAX(CUT_BUNDLE_HD.ACTUAL_PRINT_DATE),'YYYY-MM-DD') ";
            SQL = SQL + "   AS CUTDATE FROM CUT_LAY_HD, CUT_LAY_DT, CUT_BUNDLE_HD,CUT_LAY ";
            SQL = SQL + "      WHERE cut_lay_hd.lay_trans_id = cut_lay_dt.lay_trans_id AND ";
            SQL = SQL + "      cut_lay_dt.lay_dt_id = cut_bundle_hd.lay_dt_id AND ";
            SQL = SQL + "      cut_lay.LAY_ID=cut_lay_hd.LAY_ID AND ";
            SQL = SQL + "      cut_lay.factory_cd='" + factoryCd + "' AND ";
            SQL = SQL + "      cut_bundle_hd.factory_cd='" + factoryCd + "' and ";
            SQL = SQL + "      cut_bundle_hd.JOB_ORDER_NO=cut_lay.JOB_ORDER_NO ";
            SQL = SQL + "      and cut_lay.PRINT_STATUS = 'Y' AND CUT_BUNDLE_HD.TRX_TYPE='NM' ";
            */
            SQL = SQL + "   LEFT JOIN (  SELECT  JOB_ORDER_NO ,";
            if (line)
            {
                SQL = SQL + "    Production_line_cd ,";
            }
            SQL = SQL + @"    SUM(CUT_BUNDLE_HD.QTY) AS JOQTY ,
                DBO.DATE_FORMAT(MAX(CUT_BUNDLE_HD.ACTUAL_PRINT_DATE), 'YYYY-MM-DD') AS CUTDATE
                FROM    CUT_BUNDLE_HD
                WHERE  FACTORY_CD='" + factoryCd + "' and status='Y'";  //Added by MF on 20150528, added status='Y' filter for CUT_BUNDLE_HD
            //Added by ZouShiChang On 2013.09.06 End MES024
            if (fromDate != "")
            {
                SQL += " and actual_print_date >(cast('" + fromDate + "' as datetime))";
            }
            if (toDate != "")
            {
                SQL += " and actual_print_date <(cast('" + toDate + "' as datetime)+1)";
            }
            if (joNo != "")
            {
                SQL += " and JOB_ORDER_NO='" + joNo + "'";
            }
            SQL = SQL + "      GROUP BY job_order_no ";
            if (line)
            {
                SQL = SQL + "   ,Production_line_cd";
            }
            SQL = SQL + "   ) A ";
            SQL = SQL + "    on  A.JOB_ORDER_NO=B.JO_no ";
            SQL = SQL + " left join PRD_CUTTING_COMPLETION c ";
            SQL = SQL + " on c.JOB_ORDER_NO=B.JO_no";
            SQL = SQL + " WHERE 1=1 ) v ";
            if (line)
            {
                SQL = SQL + " left join dbo.GEN_PRODUCTION_LINE line ";
                SQL = SQL + " on line.PRODUCTION_LINE_CD=v.PRODUCTION_LINE_CD";
                SQL += " AND (line.FACTORY_CD='" + factoryCd + "' )";
            }
            SQL = SQL + " where 1=1 and CUTDATE is not null"; //Modified by MunFoong on 2014.07.24, MES-139
            SQL = SQL + "     Order by ";
            SQL = SQL + "      CUTDATE,BUYERNAME,JONO ";

            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetFactoryList(string userid)
        {
            string SQL = " select DEPARTMENT_ID,0 CODE from gen_users ";
            SQL = SQL + "		where user_id='" + userid + "' ";
            SQL = SQL + "		union  ";
            SQL = SQL + "		select FACTORY_ID,1 code ";
            SQL = SQL + "		from gen_factory ";
            SQL = SQL + "		where (active='Y' or FACTORY_ID IN ('YMA', 'YMB', 'YMC')) ";
            SQL = SQL + "		order by code ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetProcessCode(string factoryCd, string garmentCd)
        {
            string SQL = "SELECT * FROM (";
            SQL += " select PROCESS_CD='',SHORT_NAME='ALL'";
            SQL += " UNION ALL";
            SQL += " SELECT  DISTINCT PROCESS_CD,SHORT_NAME FROM PRD_fty_process_flow AS F,";

            SQL += " dbo.GEN_PRC_CD_MST AS M WHERE M.ACTIVE='Y' AND M.PRC_CD = F.PROCESS_CD AND M.GARMENT_TYPE=F.PROCESS_GARMENT_TYPE ";

            SQL += " AND M.FACTORY_CD =  '" + factoryCd + "' ";
            if (garmentCd != "")
            {
                SQL += "AND M.GARMENT_TYPE ='" + garmentCd + "' ";
            }
            SQL += " ) AS T ORDER BY PROCESS_CD";

            return DBUtility.GetTable(SQL, "MES");
        }



        public static DataTable GetPrdLine(string factoryCd, string processCode, string garmentType)
        {
            string SQL = " select production_line_name='',production_line_cd='' union all select distinct p.PRODUCTION_LINE_NAME, p.PRODUCTION_LINE_CD from ";
            SQL = SQL + "		gen_production_line p where 1=1 AND ACTIVE='Y' ";
            if (factoryCd != "")
            {
                SQL += "and factory_cd='" + factoryCd + "'";
            }
            if (processCode != "")
            {
                SQL += " And p.process_cd='" + processCode + "'";
            }
            if (garmentType != "")
            {
                SQL += " and p.garment_type_CD='" + garmentType + "' ";
            }
            SQL += " order by production_line_name";
            return DBUtility.GetTable(SQL, "MES");

        }



        //proCycleSummary




        //pcmDailyBundlingQty

        public static DataTable GetProcessType(string factoryCd)
        {
            string SQL = @"
            DECLARE @STR_PROCESS_TYPE NVARCHAR(100)
            SELECT @STR_PROCESS_TYPE=SYSTEM_VALUE FROM dbo.GEN_SYSTEM_SETTING WHERE SYSTEM_NAME LIKE '%PROCESS_TYPE_LIST%' ";
            SQL += " AND FACTORY_CD='" + factoryCd + "' ";
            SQL += " SELECT 'ALL' AS PROCESS_TYPE_ID,'' AS PROCESS_TYPE_VALUE UNION ALL SELECT Fnfield AS PROCESS_TYPE_ID,Fnfield AS PROCESS_TYPE_VAULE  FROM FN_SPLIT_STRING_TB(@STR_PROCESS_TYPE,';')";

            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetProcessCd(string factoryCd, string GarmnetTypeCd)
        {

            string SQL = " select 0 AS SEQ,'' PRC_CD,'All' NM union all select 0 AS SEQ,'KeyPart' PRC_CD,'KeyProcess(主要部门)' NM union all SELECT DISTINCT DISPLAY_SEQ AS SEQ,PRC_CD,NM FROM GEN_PRC_CD_MST  A ";
            SQL = SQL + "WHERE 1=1 ";
            if (factoryCd != "")
            {
                SQL += " and FACTORY_CD='" + factoryCd + "' ";
            }
            if (GarmnetTypeCd != "")
            {
                SQL += " and a.garment_type='" + GarmnetTypeCd + "'";
            }
            SQL = SQL + "AND EXISTS(SELECT TOP 1 1 fROM dbo.PRD_FTY_PROCESS_FLOW B ";
            SQL = SQL + "WHERE  A.ACTIVE='Y' AND B.PROCESS_CD=A.PRC_CD AND B.FACTORY_CD=A.FACTORY_CD) ";
            SQL = SQL + " ORDER BY SEQ ";

            return DBUtility.GetTable(SQL, "MES");

        }



  

        public static DataTable GetCuttingStickers(string JoNo, string layfrom, string layto, string FactoryCd, string Tmptable, DbConnection conn)
        {
            string SQL = "exec Proc_Gen_Cut_Ticket '" + FactoryCd + "','" + JoNo + "','" + layfrom + "','" + Tmptable + "'; SELECT *fROM " + Tmptable;
            return DBUtility.GetTable(SQL, conn);
        }

        public static DataTable GetCuttingStickershead(string JoNo, string layfrom, string layto, string FactoryCd, string Tmptable)
        {
            string SQL = "        select PRODUCTION_LINE_CD=(CASE WHEN ISNULL(l.Production_line_cd,'')= ";
            SQL = SQL + "    ISNULL(p.PRODUCTION_LINE_NAME,'')  THEN ";
            SQL = SQL + "    ISNULL(l.Production_line_cd,'') ELSE ISNULL(l.PRODUCTION_LINE_CD,'') + '(' + ";
            SQL = SQL + "    ISNULL(p.PRODUCTION_LINE_NAME,'') + ')'  END ),j.Buyer_PO_DEl_Date,l.pattern_no ";
            SQL = SQL + "    from cut_lay l";
            SQL = SQL + "    left join jo_hd j";
            SQL = SQL + "    on j.jo_no=l.job_order_no";
            SQL = SQL + "    and j.factory_cd=l.factory_cd";
            SQL = SQL + "    left join GEN_PRODUCTION_LINE p";
            SQL = SQL + "    on p.Production_line_cd=l.Production_line_cd";
            SQL = SQL + "    and p.factory_cd=l.factory_cd";
            SQL = SQL + "    where l.job_order_no='" + JoNo + "' and l.lay_no='" + layfrom + "' ";
            SQL = SQL + "   and l.factory_cd='" + FactoryCd + "'";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetDailyOutputByJOSectionLineSummary(string uuidContent, string uuidTitle)
        {
            string SQL = "        SELECT * FROM " + uuidContent + " order by TRXDATE,DTYPE";
            return DBUtility.GetTable(SQL, "MES");
        }
        //NEW
        public static DataTable GetWIPLineResultList(string factoryCd, string garmentType, string washtype, string JONo, string StartDate, string ToDate, string processCode, string prodLine, string uuidContent, string uuidTitle, DbConnection conn)
        {
            string SQL = "exec SP_Pro_DailyOutputByLine '" + factoryCd + "','" + garmentType + "','" + washtype + "','" + JONo + "','" + StartDate + "','" + ToDate + "','" + processCode + "','" + prodLine + "','" + uuidContent + "','" + uuidTitle + "'; ";
            SQL = SQL + " select distinct PROCESSCD,productionline,SUBSTRING(productionline,CHARINDEX('(',productionline),LEN(productionline)-CHARINDEX('(',productionline)+1) AS ProductionLineName from " + uuidTitle + " order by PROCESSCD,ProductionLineName";
            return DBUtility.GetTable(SQL, conn);
        }



        public static DataTable GetStandardWidthNpatternList(DbConnection conn)
        {
            string SQL = " select SC_NO,PPO_NO,WIDTH,nvl(PATTERN_TYPE_CD,'Stripe') as PATTERN_TYPE";
            SQL = SQL + ",nvl(max(PPO_YPD),max(BULK_YPD))as PPO_YPD,YPD_JOB_NO";
            SQL = SQL + " From scx_joi_go_fabric a where fabric_type_cd in ('B','BD') AND SC_no in ( select distinct SC_NO From PO_HD where 1 = 1 and JO_NO in (select f1 from rpt_tmp)";
            SQL = SQL + " )group by SC_NO,PPO_NO,WIDTH,PATTERN_TYPE_CD,YPD_JOB_NO ";
            SQL = SQL + " Order by SC_NO,PPO_NO,WIDTH,PATTERN_TYPE_CD ";
            // return DBUtility.GetTable(SQL,"EEL");
            return DBUtility.GetTable(SQL, conn);
        }


        //FabricUnitPriceQuery
        public static DataTable GetFabricUnitPriceQuery1(string scNo)
        {
            string SQL = " SELECT A.FABRIC_TYPE_DESC_CHN, A.PPO_NO, A.CCY_CD, round(A.UNITPRICE,5) UNITPRICE, ";
            SQL = SQL + "		A.QTY, A.PPO_YPD, NVL ((SELECT REMARKS FROM SCX_JOI_GO_FABRIC ";
            SQL = SQL + "		WHERE SC_NO = '" + scNo + "' AND PPO_NO = A.PPO_NO AND FABRIC_TYPE_CD = ";
            SQL = SQL + "		A.FABRIC_TYPE_CD and rownum<2), A.REMARKS ) AS REMARKS, round(NVL(A.UNITPRICE2,0),5) ";
            SQL = SQL + "		UNITPRICE2, round(NVL(A.UNITPRICE2 * DECODE (A.CCY_CD, 'RMB', 1, ";
            SQL = SQL + "		G.CONVERSION_RATE),0),5) AS RMB_PRICE FROM (SELECT ";
            SQL = SQL + "		D.FABRIC_TYPE_DESC_CHN, D.FABRIC_TYPE_CD, A.PPO_NO, A.CCY_CD, ";
            SQL = SQL + "		SUM (B.ORDER_QTY * B.UNIT_PRICE) / SUM (B.ORDER_QTY) AS ";
            SQL = SQL + "		UNITPRICE, SUM (B.ORDER_QTY) AS QTY, E.MIN_ORDER AS PPO_YPD, ";
            SQL = SQL + "		A.REMARKS, SUM (B.ORDER_QTY * B.UNIT_PRICE * NVL (E.MIN_ORDER, ";
            SQL = SQL + "		12) ) / SUM (B.ORDER_QTY) / 12 AS UNITPRICE2 FROM PPO_HD A, ";
            SQL = SQL + "		PPO_DT B, VIEW_SC_PPO C, FAB_FABRIC_TYPE D, FAB_LIB E WHERE ";
            SQL = SQL + "		A.PPO_NO = B.PPO_NO AND A.PPO_NO = C.PPO_NO AND D.FABRIC_TYPE_CD ";
            SQL = SQL + "		= B.FABRIC_TYPE_CD AND B.FAB_REF_CD = E.FAB_REF_CD AND A.PPO_NO ";
            SQL = SQL + "		= C.PPO_NO AND B.FABRIC_TYPE_CD = D.FABRIC_TYPE_CD AND C.SC_NO = ";
            SQL = SQL + "		'" + scNo + "' AND A.FABRIC_NATURE_CD = 'K' GROUP BY ";
            SQL = SQL + "		D.FABRIC_TYPE_DESC_CHN, D.FABRIC_TYPE_CD, A.REMARKS, A.PPO_NO, ";
            SQL = SQL + "		A.CCY_CD, E.MIN_ORDER UNION ALL SELECT D.FABRIC_TYPE_DESC_CHN, ";
            SQL = SQL + "		D.FABRIC_TYPE_CD, A.PPO_NO, A.CCY_CD, round(SUM (C.ORDER_QTY * ";
            SQL = SQL + "		C.PPO_PRICE) / SUM (C.ORDER_QTY),5) AS UNITPRICE, round(SUM (C.ORDER_QTY),5) ";
            SQL = SQL + "		AS QTY, C.MARKER_YPD AS PPO_YPD, A.REMARKS, round(SUM (C.ORDER_QTY * ";
            SQL = SQL + "		C.PPO_PRICE * NVL (C.MARKER_YPD, 12) ) / SUM (C.ORDER_QTY) / 12,5) ";
            SQL = SQL + "		AS UNITPRICE2 FROM ESCMOWNER.PPO_HD A, PPOX_FAB_ITEM B, PPOX_LOT ";
            SQL = SQL + "		C, FAB_FABRIC_TYPE D, SCX_LOT_PPO E, SCX_JOI_GO_FABRIC F WHERE ";
            SQL = SQL + "		A.PPO_NO = B.PPO_NO AND B.FAB_ITEM_ID = C.FAB_ITEM_ID AND ";
            SQL = SQL + "		B.FABRIC_TYPE_CD = D.FABRIC_TYPE_CD AND A.PPO_NO = E.PPO_NO AND ";
            SQL = SQL + "		A.FABRIC_NATURE_CD = 'W' AND E.SC_NO = '" + scNo + "' GROUP BY ";
            SQL = SQL + "		D.FABRIC_TYPE_DESC_CHN, D.FABRIC_TYPE_CD, A.REMARKS, A.PPO_NO, ";
            SQL = SQL + "		A.CCY_CD, C.MARKER_YPD) A, (SELECT FROM_CURRENCY, ";
            SQL = SQL + "		CONVERSION_RATE FROM GL_DAILY_RATES B WHERE TO_CURRENCY = 'RMB' ";
            SQL = SQL + "		AND B.CONVERSION_TYPE = 'Spot' AND CONVERSION_DATE = TO_DATE ";
            SQL = SQL + "		(TO_CHAR (SYSDATE, 'yyyymmdd'), 'yyyymmdd')) G WHERE ";
            SQL = SQL + "		G.FROM_CURRENCY(+) = A.CCY_CD ORDER BY 1, 2 ";

            return DBUtility.GetTable(SQL, "EEL");
        }
        public static DataTable GetFabricUnitPriceQuery2(string scNo)
        {
            string SQL = " SELECT D.FABRIC_TYPE_DESC_CHN, A.CCY_CD, round(SUM (B.ORDER_QTY * ";
            SQL = SQL + "		B.UNIT_PRICE * NVL (E.MIN_ORDER, 12) ) / SUM (B.ORDER_QTY) / 12,5) ";
            SQL = SQL + "		AS UNITPRICE, round(SUM (B.ORDER_QTY * B.UNIT_PRICE * NVL ";
            SQL = SQL + "		(E.MIN_ORDER, 12) ) / SUM (B.ORDER_QTY) / 12 * DECODE (A.CCY_CD, ";
            SQL = SQL + "		'RMB', 1, G.CONVERSION_RATE),5) AS RMB_PRICE FROM PPO_HD A, PPO_DT ";
            SQL = SQL + "		B, VIEW_SC_PPO C, FAB_FABRIC_TYPE D, FAB_LIB E, (SELECT ";
            SQL = SQL + "		FROM_CURRENCY, CONVERSION_RATE FROM GL_DAILY_RATES B WHERE ";
            SQL = SQL + "		TO_CURRENCY = 'RMB' AND B.CONVERSION_TYPE = 'Spot' AND ";
            SQL = SQL + "		CONVERSION_DATE = TO_DATE (TO_CHAR (SYSDATE, 'yyyymmdd'), ";
            SQL = SQL + "		'yyyymmdd')) G WHERE A.PPO_NO = B.PPO_NO AND A.PPO_NO = C.PPO_NO ";
            SQL = SQL + "		AND D.FABRIC_TYPE_CD = B.FABRIC_TYPE_CD AND B.FAB_REF_CD = ";
            SQL = SQL + "		E.FAB_REF_CD AND A.PPO_NO = C.PPO_NO AND B.FABRIC_TYPE_CD = ";
            SQL = SQL + "        D.FABRIC_TYPE_CD AND C.SC_NO = '" + scNo + "' AND A.FABRIC_NATURE_CD = ";
            SQL = SQL + "        'K' AND G.FROM_CURRENCY(+) = A.CCY_CD GROUP BY ";
            SQL = SQL + "        D.FABRIC_TYPE_DESC_CHN, A.CCY_CD, G.CONVERSION_RATE UNION ALL ";
            SQL = SQL + "        SELECT D.FABRIC_TYPE_DESC_CHN, A.CCY_CD, round(SUM (C.ORDER_QTY * ";
            SQL = SQL + "        C.PPO_PRICE * NVL (C.MARKER_YPD, 12) ) / SUM (C.ORDER_QTY) / 12,5) ";
            SQL = SQL + "        AS UNITPRICE, round(SUM (C.ORDER_QTY * C.PPO_PRICE * NVL ";
            SQL = SQL + "        (C.MARKER_YPD, 12) ) / SUM (C.ORDER_QTY) / 12 * DECODE ";
            SQL = SQL + "        (A.CCY_CD, 'RMB', 1, G.CONVERSION_RATE),5) AS RMB_PRICE FROM ";
            SQL = SQL + "        ESCMOWNER.PPO_HD A, PPOX_FAB_ITEM B, PPOX_LOT C, FAB_FABRIC_TYPE ";
            SQL = SQL + "        D, SCX_LOT_PPO E, (SELECT FROM_CURRENCY, CONVERSION_RATE FROM ";
            SQL = SQL + "        GL_DAILY_RATES B WHERE TO_CURRENCY = 'RMB' AND B.CONVERSION_TYPE ";
            SQL = SQL + "        = 'Spot' AND CONVERSION_DATE = TO_DATE (TO_CHAR (SYSDATE, ";
            SQL = SQL + "        'yyyymmdd'), 'yyyymmdd')) G WHERE A.PPO_NO = B.PPO_NO AND ";
            SQL = SQL + "        B.FAB_ITEM_ID = C.FAB_ITEM_ID AND B.FABRIC_TYPE_CD = ";
            SQL = SQL + "        D.FABRIC_TYPE_CD AND A.PPO_NO = E.PPO_NO AND A.FABRIC_NATURE_CD ";
            SQL = SQL + "        = 'W' AND E.SC_NO = '" + scNo + "' AND G.FROM_CURRENCY(+) = A.CCY_CD ";
            SQL = SQL + "        GROUP BY D.FABRIC_TYPE_DESC_CHN, A.CCY_CD, G.CONVERSION_RATE ";
            SQL = SQL + "        ORDER BY 1, 2 ";
            return DBUtility.GetTable(SQL, "EEL");
        }
        public static void SP_Pro_DropTmpTable(string uuidContent, string uuidTitle)
        {
            string SQL = "exec SP_Pro_DropTmpTable '" + uuidContent + "','" + uuidTitle + "' ";
            DBUtility.ExecuteNonQuery(SQL, "MES");
        }

        //HandlingCostReport
        public static DataTable GetHandlingCostReport1(string scNo, string contractNo)
        {
            string SQL = "         SELECT C.NAME, D.SC_NO, E.STYLE_NO, SUM(B.PLAN_ISSUE_QTY) ";
            SQL = SQL + "        PLAN_ISSUE_QTY, E.STYLE_CHN_DESC, F.SUBCONTRACTOR_NAME, ";
            SQL = SQL + "        B.PROCESS_REMARK, E.GARMENT_TYPE_CD, ";
            SQL = SQL + "        convert(decimal(18,2),ISNULL(B.INTERNER_PRICE,0)) ";
            SQL = SQL + "        INTERNER_PRICE, ";
            SQL = SQL + "        convert(decimal(18,2),ISNULL(B.INTERNAL_EMB_PRICE,0)) ";
            SQL = SQL + "        INTERNAL_EMB_PRICE, ";
            SQL = SQL + "        convert(decimal(18,2),ISNULL(B.INTERNAL_WASH_PRICE,0)) ";
            SQL = SQL + "        INTERNAL_WASH_PRICE, ";
            SQL = SQL + "        convert(decimal(18,2),(ISNULL(B.INTERNER_PRICE,0) ";
            SQL = SQL + "        +ISNULL(B.INTERNAL_EMB_PRICE,0) ";
            SQL = SQL + "        +ISNULL(B.INTERNAL_WASH_PRICE,0))) AS TTL_INNER, ";
            SQL = SQL + "        convert(decimal(18,2),ISNULL(B.SUB_CONTRACT_PRICE,0)) ";
            SQL = SQL + "        SUB_CONTRACT_PRICE, convert(decimal(18,2),ISNULL(B.EMB_PRICE,0)) ";
            SQL = SQL + "        EMB_PRICE, convert(decimal(18,2),ISNULL(B.WASH_PRICE,0)) ";
            SQL = SQL + "        WASH_PRICE, ";
            SQL = SQL + "        convert(decimal(18,2),(ISNULL(B.SUB_CONTRACT_PRICE,0) ";
            SQL = SQL + "        +ISNULL(B.EMB_PRICE,0) +ISNULL(B.WASH_PRICE,0))) AS TTL_OUTTER, ";
            SQL = SQL + "        ISNULL(B.SAH,0) SAH, '0.00' STANDARD, '' AS PATTERN_TYPE_CD FROM ";
            SQL = SQL + "        PRD_OUTSOURCE_CONTRACT A JOIN PRD_OUTSOURCE_CONTRACT_DT B ON ";
            SQL = SQL + "        A.CONTRACT_NO=B.CONTRACT_NO LEFT JOIN GEN_CUSTOMER C ON ";
            SQL = SQL + "        B.CUSTOMER_CD=C.CUSTOMER_CD JOIN JO_HD D ON "; //By ZouShiChang On 2013.08.29 MES024 JO_HD Change JO_HD
            SQL = SQL + "        B.JOB_ORDER_NO=D.JO_NO JOIN SC_HD E ON D.SC_NO=E.SC_NO JOIN ";//By ZouShiChang On 2013.08.29 MES024 JO_NO Change JO_NO
            SQL = SQL + "        PRD_SUBCONTRACTOR_MASTER F ON F.SUBCONTRACTOR_CD=A.SUBCONTRACTOR ";
            SQL = SQL + "        WHERE 1=1 ";
            if (scNo != "")
            {
                SQL += " AND D.SC_NO = '" + scNo + "'";
            }
            if (contractNo != "")
            {
                SQL += " AND A.CONTRACT_NO = '" + contractNo + "'";
            }
            SQL = SQL + "        GROUP BY C.NAME, D.SC_NO, E.STYLE_NO, E.STYLE_CHN_DESC, ";
            SQL = SQL + "		F.SUBCONTRACTOR_NAME, B.PROCESS_REMARK, ";
            SQL = SQL + "		B.INTERNER_PRICE,B.INTERNAL_EMB_PRICE,B.INTERNAL_WASH_PRICE,B.SUB_CONTRACT_PRICE, ";
            SQL = SQL + "		B.EMB_PRICE,B.WASH_PRICE,B.SAH,E.GARMENT_TYPE_CD ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetHandlingCostReport2(string scNo, string contractNo)
        {
            string SQL = "         SELECT C.NAME, E.SC_NO, SUM(B.PLAN_ISSUE_QTY) PLAN_ISSUE_QTY, '' ";
            SQL = SQL + "		AS EMBRIODERY_CODE, B.PROCESS_CD, MAX(B.STITCHES) STITCHES, ";
            SQL = SQL + "		convert(decimal(18,2),max(B.STITCHES)*0.00016) AS CAL1, ";
            SQL = SQL + "		convert(decimal(18,2),B.INTERNAL_EMB_PRICE) INTERNAL_EMB_PRICE, ";
            SQL = SQL + "		convert(decimal(18,2),B.EMB_PRICE) EMB_PRICE, CAL2=(CASE WHEN ";
            SQL = SQL + "		SUM(ISNULL(B.STITCHES,0))=0 THEN 0 ELSE ";
            SQL = SQL + "		convert(decimal(18,2),B.EMB_PRICE*10000/(1.2*SUM(B.STITCHES))) ";
            SQL = SQL + "		END) FROM PRD_OUTSOURCE_CONTRACT A JOIN ";
            SQL = SQL + "		PRD_OUTSOURCE_CONTRACT_DT B ON A.CONTRACT_NO=B.CONTRACT_NO LEFT ";
            SQL = SQL + "		JOIN GEN_CUSTOMER C ON B.CUSTOMER_CD=C.CUSTOMER_CD JOIN JO_HD "; //By ZouShiChang On 2013.08.29 MES024 JO_HD Change JO_HD
            SQL = SQL + "		D ON B.JOB_ORDER_NO=D.JO_NO JOIN SC_HD E ON D.SC_NO=E.SC_NO ";  //By ZouShiChang On 2013.08.29 MES024 JO_NO Change JO_NO
            SQL = SQL + "		WHERE 1=1 ";
            if (scNo != "")
            {
                SQL += " AND D.SC_NO = '" + scNo + "'";
            }
            if (contractNo != "")
            {
                SQL += " AND A.CONTRACT_NO = '" + contractNo + "'";
            }
            SQL = SQL + "        GROUP BY C.NAME, E.SC_NO, B.PROCESS_CD, B.INTERNAL_EMB_PRICE, ";
            SQL = SQL + "		B.EMB_PRICE ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetHandlingCostReport3(string scNo, string contractNo)
        {
            string SQL = "         SELECT DISTINCT D.SC_NO, B.FOB_FABRIC_PRICE, B.WASH_PRICE, ";
            SQL = SQL + "		B.EMB_PRICE, convert(decimal(18,2),ISNULL(A.HANDLING_COST,0)) ";
            SQL = SQL + "		HANDLING_COST FROM PRD_OUTSOURCE_CONTRACT A , ";
            SQL = SQL + "		PRD_OUTSOURCE_CONTRACT_DT B, JO_HD D WHERE "; //By ZouShiChang On 2013.08.29 MES024 JO_HD Change JO_HD
            SQL = SQL + "		A.CONTRACT_NO=B.CONTRACT_NO AND B.JOB_ORDER_NO=D.JO_NO ";  //By ZouShiChang On 2013.08.29 MES024 JO_NO Change JO_NO
            if (scNo != "")
            {
                SQL += " AND D.SC_NO = '" + scNo + "'";
            }
            if (contractNo != "")
            {
                SQL += " AND A.CONTRACT_NO = '" + contractNo + "'";
            }
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetPattern(string scNo)
        {
            string SQL = " SELECT PATTERN_TYPE_CD FROM SCX_JOI_GO_FABRIC WHERE SC_NO='" + scNo + "' AND fabric_type_cd in ('B','BD')";
            return DBUtility.GetTable(SQL, "EEL");
        }
        public static DataTable GetEmb(string scNo)
        {
            string SQL = " SELECT EMBRIODERY_CODE FROM SCX_JOI_EMBRIODERY WHERE SC_NO='" + scNo + "'";
            return DBUtility.GetTable(SQL, "EEL");
        }
        public static DataTable GetFOB(string scNo)
        {
            string SQL = "         SELECT ";
            SQL = SQL + "		round(sum(a.Unit_Price*a.Total_qty*decode(a.CCY_CD,'RMB',1,b.Conversion_rate))/SUM(A.TOTAL_QTY),2) ";
            SQL = SQL + "		AS FOB_UNIT_PRICE from sc_lot a,(select ";
            SQL = SQL + "		from_currency,Conversion_rate from gl_daily_rates b where ";
            SQL = SQL + "		to_currency='RMB' and b.conversion_type='Spot' and ";
            SQL = SQL + "		CONVERSION_DATE=to_date(to_char(sysdate,'yyyymmdd'),'yyyymmdd')) ";
            SQL = SQL + "        b where a.sc_no='" + scNo + "' AND a.ACTIVE='Y' and ";
            SQL = SQL + "        from_currency(+)=a.CCY_CD ";
            return DBUtility.GetTable(SQL, "EEL");
        }
        public static DataTable GetAcceserie(string scNo)
        {
            string SQL = "         SELECT round(SUM ( A.QUANTITY * UNIT_PRICE / A.PERXGMT * DECODE ";
            SQL = SQL + "		(A.CCY_CD, 'RMB', 1, B.CONVERSION_RATE) ),2) AS BOM_U_PRICE FROM ";
            SQL = SQL + "		(SELECT A.QUANTITY, A.PERXGMT, C.CCY_CD, C.UNIT_PRICE AS ";
            SQL = SQL + "		UNIT_PRICE FROM BOMX_TRIM_LIST A, SUBX_ITEM C, SC_HD E, ";
            SQL = SQL + "		GEN_CUSTOMER F, GEN_SUPPLIER G WHERE A.ITEM_ID = C.ITEM_ID AND ";
            SQL = SQL + "		NVL (A.STATUS, 'D') <> 'X' AND A.BOM_NO = E.BOM_NO AND ";
            SQL = SQL + "		E.CUSTOMER_CD = F.CUSTOMER_CD AND A.SUPPLIER_CD = ";
            SQL = SQL + "        G.SUPPLIER_CD(+) AND NOT EXISTS (SELECT 1 FROM BOMX_TRIM_DT ";
            SQL = SQL + "        WHERE TRIM_ID = A.TRIM_ID) AND A.BOM_NO = '" + scNo + "' UNION ALL ";
            SQL = SQL + "        SELECT A.QUANTITY, A.PERXGMT, NVL (ESCMUSER.GET_BOM_UNITPRICE ";
            SQL = SQL + "        (E.SC_NO, A.ITEM_ID, 'CCY'), C.CCY_CD ) AS CCY_CD, NVL ";
            SQL = SQL + "        (TO_NUMBER (ESCMUSER.GET_BOM_UNITPRICE (E.SC_NO, A.ITEM_ID, ";
            SQL = SQL + "        'PRICE')), C.UNIT_PRICE ) AS UNIT_PRICE FROM BOMX_TRIM_LIST A, ";
            SQL = SQL + "        SUBX_ITEM C, SC_HD E, GEN_CUSTOMER F, GEN_SUPPLIER G WHERE ";
            SQL = SQL + "        A.ITEM_ID = C.ITEM_ID AND NVL (A.STATUS, 'D') <> 'X' AND ";
            SQL = SQL + "        A.BOM_NO = E.BOM_NO AND E.CUSTOMER_CD = F.CUSTOMER_CD AND ";
            SQL = SQL + "        A.SUPPLIER_CD = G.SUPPLIER_CD(+) AND EXISTS (SELECT 1 FROM ";
            SQL = SQL + "        BOMX_TRIM_DT WHERE TRIM_ID = A.TRIM_ID) AND A.BOM_NO = '" + scNo + "') ";
            SQL = SQL + "        A, (SELECT FROM_CURRENCY, CONVERSION_RATE FROM GL_DAILY_RATES B ";
            SQL = SQL + "        WHERE TO_CURRENCY = 'RMB' AND B.CONVERSION_TYPE = 'Spot' AND ";
            SQL = SQL + "        CONVERSION_DATE = TO_DATE (TO_CHAR (SYSDATE, 'yyyymmdd'), ";
            SQL = SQL + "        'yyyymmdd')) B WHERE B.FROM_CURRENCY(+) = A.CCY_CD ";
            return DBUtility.GetTable(SQL, "WIP");
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
            SQL = SQL + "        FROM prd_outsource_contract hd,prd_subcontractor_master sub,prd_SENDER_RECIVER_MASTER r  ";
            SQL = SQL + "        WHERE hd.contract_no ='" + contractNo + "' and r.code=hd.receiver  AND hd.STATUS!='CAN'  ";
            SQL = SQL + "        AND sub.subcontractor_cd=hd.subcontractor ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetDetailPrice(string scNo)
        {
            string SQL = "         SELECT A.TRIM_ID, A.BOM_NO, A.SC_NO, A.STYLE_NO, A.CUSTOMER, ";
            SQL = SQL + "		A.PRODUCT_CATEGORY, A.PRODUCT_CLASS, A.SUPP_REF_NO, ";
            SQL = SQL + "		A.PRODUCT_OTHER_DESC, A.SUPPLIER, A.QUANTITY, A.PERXGMT, ";
            SQL = SQL + "		A.CCY_CD, A.UNIT_PRICE, DECODE (A.CCY_CD, 'RMB', 1, ";
            SQL = SQL + "		B.CONVERSION_RATE) AS CONVERSION_RATE, A.UOM_CD, A.DEFAULT_FLAG ";
            SQL = SQL + "		FROM (SELECT A.TRIM_ID, A.BOM_NO, E.SC_NO, E.STYLE_NO, F.NAME || ";
            SQL = SQL + "		'(' || F.CUSTOMER_CD || ')' AS CUSTOMER, C.PRODUCT_CATEGORY, ";
            SQL = SQL + "		C.PRODUCT_CLASS, C.SUPP_REF_NO, C.PRODUCT_OTHER_DESC, G.NAME AS ";
            SQL = SQL + "		SUPPLIER, A.QUANTITY, A.PERXGMT, C.CCY_CD, C.UNIT_PRICE AS ";
            SQL = SQL + "		UNIT_PRICE, A.UOM_CD, 'Y' AS DEFAULT_FLAG FROM BOMX_TRIM_LIST A, ";
            SQL = SQL + "		SUBX_ITEM C, SC_HD E, GEN_CUSTOMER F, GEN_SUPPLIER G WHERE ";
            SQL = SQL + "		A.ITEM_ID = C.ITEM_ID AND NVL (A.STATUS, 'D') <> 'X' AND ";
            SQL = SQL + "		A.BOM_NO = E.BOM_NO AND E.CUSTOMER_CD = F.CUSTOMER_CD AND ";
            SQL = SQL + "		A.SUPPLIER_CD = G.SUPPLIER_CD(+) AND NOT EXISTS (SELECT 1 FROM ";
            SQL = SQL + "		BOMX_TRIM_DT WHERE TRIM_ID = A.TRIM_ID) AND A.BOM_NO = '" + scNo + "' ";
            SQL = SQL + "		UNION ALL SELECT A.TRIM_ID, A.BOM_NO, E.SC_NO, E.STYLE_NO, ";
            SQL = SQL + "		F.NAME || '(' || F.CUSTOMER_CD || ')' AS CUSTOMER, ";
            SQL = SQL + "		C.PRODUCT_CATEGORY, C.PRODUCT_CLASS, C.SUPP_REF_NO, ";
            SQL = SQL + "		C.PRODUCT_OTHER_DESC, G.NAME AS SUPPLIER, A.QUANTITY, A.PERXGMT, ";
            SQL = SQL + "		NVL (ESCMUSER.GET_BOM_UNITPRICE (E.SC_NO, A.ITEM_ID, 'CCY'), ";
            SQL = SQL + "        C.CCY_CD ) AS CCY_CD, NVL (TO_NUMBER (ESCMUSER.GET_BOM_UNITPRICE ";
            SQL = SQL + "        (E.SC_NO, A.ITEM_ID, 'PRICE')), C.UNIT_PRICE ) AS UNIT_PRICE, ";
            SQL = SQL + "        NVL (ESCMUSER.GET_BOM_UNITPRICE (E.SC_NO, A.ITEM_ID, 'UOM'), ";
            SQL = SQL + "        A.UOM_CD ) AS UOM_CD, DECODE (ESCMUSER.GET_BOM_UNITPRICE ";
            SQL = SQL + "        (E.SC_NO, A.ITEM_ID, 'CCY'), '', 'Y', 'N' ) AS DEFAULT_FLAG FROM ";
            SQL = SQL + "        BOMX_TRIM_LIST A, SUBX_ITEM C, SC_HD E, GEN_CUSTOMER F, ";
            SQL = SQL + "        GEN_SUPPLIER G WHERE A.ITEM_ID = C.ITEM_ID AND NVL (A.STATUS, ";
            SQL = SQL + "        'D') <> 'X' AND A.BOM_NO = E.BOM_NO AND E.CUSTOMER_CD = ";
            SQL = SQL + "        F.CUSTOMER_CD AND A.SUPPLIER_CD = G.SUPPLIER_CD(+) AND EXISTS ";
            SQL = SQL + "        (SELECT 1 FROM BOMX_TRIM_DT WHERE TRIM_ID = A.TRIM_ID) AND ";
            SQL = SQL + "        A.BOM_NO = '" + scNo + "') A, (SELECT FROM_CURRENCY, CONVERSION_RATE ";
            SQL = SQL + "        FROM GL_DAILY_RATES B WHERE TO_CURRENCY = 'RMB' AND ";
            SQL = SQL + "        B.CONVERSION_TYPE = 'Spot' AND CONVERSION_DATE = TO_DATE ";
            SQL = SQL + "        (TO_CHAR (SYSDATE, 'yyyymmdd'), 'yyyymmdd')) B WHERE ";
            SQL = SQL + "        B.FROM_CURRENCY(+) = A.CCY_CD ";
            return DBUtility.GetTable(SQL, "WIP");
        }
        public static DataTable GetSubcontractor()
        {
            string SQL = " select subcontractor_cd='' union all SELECT SUBCONTRACTOR_CD FROM PRD_SUBCONTRACTOR_MASTER";
            return DBUtility.GetTable(SQL, "MES");
        }

        //queryContract

        public static DataTable GetQueryContract(string factoryCd, string contractNo, string subContractor, string fromDate, string toDate, string JoNo)
        {
            string SQL = "         SELECT distinct OHD.contract_no FROM PRD_OUTSOURCE_CONTRACT OHD, ";
            SQL = SQL + "		PRD_OUTSOURCE_CONTRACT_DT ODT WHERE ";
            SQL = SQL + "		ODT.CONTRACT_NO=OHD.CONTRACT_NO AND OHD.STATUS <>'CAN' ";
            if (factoryCd != "")
            {
                SQL += " and OHD.factory_cd='" + factoryCd + "'";
            }
            if (contractNo != "")
            {
                SQL += " and OHD.contract_no like '%" + contractNo + "%'";
            }
            if (subContractor != "")
            {
                SQL += " and OHD.SUBCONTRACTOR = '" + subContractor + "'";
            }
            if (fromDate != "")
            {
                SQL += " and dbo.DATE_FORMAT(OHD.ISSUER_DATE,'mm/dd/yyyy') >=dbo.DATE_FORMAT('" + fromDate + "','mm/dd/yyyy')";
            }
            if (toDate != "")
            {
                SQL += " and dbo.DATE_FORMAT(OHD.ISSUER_DATE,'mm/dd/yyyy') <=dbo.DATE_FORMAT('" + toDate + "','mm/dd/yyyy')";
            }
            if (JoNo != "")
            {
                SQL += " and ODT.JOB_ORDER_NO like '%" + JoNo + "%'";
            }
            return DBUtility.GetTable(SQL, "MES");
        }

        //jobOrderRouteListNew
        public static DataTable GetJobOrderRouteListNew(string factoryCd, string Date, string JoNo, string Orderby, string PartCD, string Cstr, string Process_cd)
        {
            string SQL = "         SELECT  " + Cstr;
            SQL = SQL + "        FROM  PRD_JO_ROUTE_LIST ,GEN_OPERATION_CODE_MASTER WHERE ";
            SQL = SQL + "        PRD_JO_ROUTE_LIST.FACTORY_CD='" + factoryCd + "' AND ";
            SQL = SQL + "        PRD_JO_ROUTE_LIST.Oper_Code = ";
            SQL = SQL + "        GEN_OPERATION_CODE_MASTER.OPERATION_CD and ";
            SQL = SQL + "        PRD_JO_ROUTE_LIST.FACTORY_CD = ";
            SQL = SQL + "        GEN_OPERATION_CODE_MASTER.FACTORY_CD ";
            if (Date != "")
            {
                SQL += "  and PRD_JO_ROUTE_LIST.CREATE_DATE >= '" + Date + "' AND  PRD_JO_ROUTE_LIST.CREATE_DATE <= dateadd(d, 1, '" + Date + "')";
            }
            if (JoNo != "")
            {
                SQL += " AND PRD_JO_ROUTE_LIST.Job_Order_No = '" + JoNo + "' ";
            }
            if (PartCD != "")
            {
                SQL += " AND GEN_OPERATION_CODE_MASTER.PART_CD = '" + PartCD + "' ";
            }
            if (!Process_cd.Equals(""))
            {
                SQL += " AND PRD_JO_ROUTE_LIST.PROCESS_CD = '" + Process_cd + "'";
            }
            SQL += " ORDER BY PRD_JO_ROUTE_LIST.Job_Order_No";
            switch (Orderby)
            {
                case "DISPLAY_SEQ":
                    SQL += " ,cast(PRD_JO_ROUTE_LIST.DISPLAY_SEQ as int)";
                    break;
                case "JOB_SEQUENCE_NO":
                    SQL += "  ,cast(PRD_JO_ROUTE_LIST.JOB_SEQUENCE_NO as int)";
                    break;
                case "JOB_CD":
                    SQL += "  ,cast(PRD_JO_ROUTE_LIST.JOB_CD as int)";
                    break;
                case "OPER_CODE":
                    SQL += "  ,PRD_JO_ROUTE_LIST.OPER_CODE";
                    break;
                case "PROCESS_CD":
                    SQL += "  ,PRD_JO_ROUTE_LIST.PROCESS_CD,cast(PRD_JO_ROUTE_LIST.DISPLAY_SEQ as int)";
                    break;
                case "PART_CD":
                    SQL += "  ,PRD_JO_ROUTE_LIST.PART_CD,cast(PRD_JO_ROUTE_LIST.DISPLAY_SEQ as int)";
                    break;
            }
            return DBUtility.GetTable(SQL, "MES");
        }

        //JobOrderRouteList  // <Changed by: DaiJ at: 2012-12-04 11:49:18>

        public static DataTable GetCustomReportList(string factoryCd, string Funcion, string custometype, string customname, string user_id)
        {
            string SQL = "SELECT CUSTOM_VALUES FROM MES_USER_CUSTOM_FUNCTION ";
            SQL = SQL + " WHERE FACTORY_CD='" + factoryCd + "' AND [USER_ID]='" + user_id + "'";
            SQL = SQL + " AND [FUNCTION]='" + Funcion + "' AND CUSTOM_TYPE='" + custometype + "'";
            SQL = SQL + " AND CUSTOM_NAME='" + customname + "'";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable SaveCustomReportList(string factoryCd, string Funcion, string custometype, string customname, string user_id, string customvalue)
        {
            string SQL = "DELETE FROM MES_USER_CUSTOM_FUNCTION  ";
            SQL = SQL + " WHERE FACTORY_CD='" + factoryCd + "' AND [USER_ID]='" + user_id + "'";
            SQL = SQL + " AND [FUNCTION]='" + Funcion + "' AND CUSTOM_TYPE='" + custometype + "'";
            SQL = SQL + " AND CUSTOM_NAME='" + customname + "';";
            SQL = SQL + " INSERT INTO MES_USER_CUSTOM_FUNCTION ([FACTORY_CD],[USER_ID],[FUNCTION],[CUSTOM_TYPE],[CUSTOM_NAME],[CUSTOM_VALUES])";
            SQL = SQL + " VALUES('" + factoryCd + "','" + user_id + "','" + Funcion + "','" + custometype + "','" + customname + "','" + customvalue + "');";
            SQL = SQL + " SELECT TOP 1 1 FROM MES_USER_CUSTOM_FUNCTION WHERE FACTORY_CD='" + factoryCd + "' AND [USER_ID]='" + user_id + "'";
            SQL = SQL + " AND [FUNCTION]='" + Funcion + "' AND CUSTOM_TYPE='" + custometype + "'";
            SQL = SQL + " AND CUSTOM_NAME='" + customname + "'";
            return DBUtility.GetTable(SQL, "MES_UPDATE");
        }


        //JOProcessReport
        public static DataTable GetOsJoProcess(string startDate, string endDate, string factoryCd, string contractNo)
        {
            string SQL = "         SELECT DISTINCT ODT.JOB_ORDER_NO , ODT.PROCESS_CD, ";
            SQL = SQL + "		ODT.PROCESS_REMARK, ";
            SQL = SQL + "		dbo.DATE_FORMAT(OHD.ISSUER_DATE,'YYYY-MM-DD') ISSUER_DATE, ";
            SQL = SQL + "		ODT.PRINT_PRICE, ODT.EMB_PRICE, ODT.WASH_PRICE, ";
            SQL = SQL + "		ODT.SUB_CONTRACT_PRICE, ODT.CONTRACT_NO, SUB.SUBCONTRACTOR_NAME ";
            SQL = SQL + "		FROM PRD_OUTSOURCE_CONTRACT OHD, PRD_OUTSOURCE_CONTRACT_DT ODT, ";
            SQL = SQL + "		PRD_SUBCONTRACTOR_MASTER SUB WHERE OHD.STATUS!='CAN' AND ";
            SQL = SQL + "		ODT.CONTRACT_NO = OHD.CONTRACT_NO AND SUB.SUBCONTRACTOR_CD ";
            SQL = SQL + "		=OHD.SUBCONTRACTOR ";
            if (factoryCd != "")
            {
                SQL += " AND OHD.FACTORY_CD ='" + factoryCd + "'";
            }
            if (startDate != "")
            {
                SQL += " AND OHD.ISSUER_DATE >=dbo.DATE_FORMAT('" + startDate + "','MM-DD-YYYY')";
            }
            if (endDate != "")
            {
                SQL += " AND OHD.ISSUER_DATE <=DATEADD(d,1,dbo.DATE_FORMAT('" + endDate + "','MM/DD/YYYY'))";
            }
            if (contractNo != "")
            {
                SQL += " AND OHD.CONTRACT_NO LIKE '%" + contractNo + "%'";
            }
            return DBUtility.GetTable(SQL, "MES");
        }

        //prodProSumOut
        public static DataTable GetProdProcessSummaryOutputList(string factoryCd, string startDate, string endDate, string processCd, string processType,string productionFactory,string JoNo, string garmentType, string washType)
        {
            //string SQL = "      SELECT '' AS ROUTE_TYPE,SL.STYLE_DESC,B.JOB_ORDER_NO,B.PROCESS_CD,B.PROCESS_GARMENT_TYPE AS GARMENT_TYPE,B.OUTPUT_QTY, ";
            string SQL = "      SELECT '' AS ROUTE_TYPE,SL.STYLE_DESC,B.JOB_ORDER_NO,CASE WHEN B.PROCESS_TYPE IN ('I','E','O') THEN B.PROCESS_GARMENT_TYPE+B.PROCESS_CD+B.PROCESS_TYPE ELSE B.PROCESS_GARMENT_TYPE+B.PROCESS_CD+B.PROCESS_TYPE+B.PROCESS_PRODUCTION_FACTORY END AS PROCESS_CD,B.OUTPUT_QTY, ";
            SQL = SQL + "              (select SHORT_NAME from dbo.GEN_CUSTOMER where CUSTOMER_CD= PO.CUSTOMER_CD) as CUSTOMER_NAME,PO.SC_NO,PO.STYLE_NO,PO.WASH_TYPE_CD,DT.ORDER_QTY, SAH=isnull((case when (exists(select FTY_TYPE from dbo.SC_SAM ";
            SQL = SQL + "      where TYPE='S' and sah is not null AND SAH<>0 and sc_no=PO.sc_no)) ";
            SQL = SQL + "       THEN (select sah from dbo.SC_SAM where type='S' and sc_no=PO.sc_no) ";
            SQL = SQL + "      else (select sah from dbo.SC_SAM where type='A' and sc_no=PO.sc_no ) END), 0),  ";
            SQL = SQL + "          DBO.DATE_FORMAT(PO.BUYER_PO_DEL_DATE,'YYYY-MM-DD') BPD,DBO.DATE_FORMAT(PO.PROD_COMPLETION_DATE,'YYYY-MM-DD') PPCD ";
            SQL = SQL + "          from  ";
            SQL = SQL + "          (select factory_cd,job_order_no,process_cd,process_garment_type,process_type,process_production_factory,SUM(isnull(out_qty,0)) OUTPUT_QTY    "; 
            SQL = SQL + "          from prd_jo_daily_stock B ";
            SQL = SQL + "          where factory_cd='" + factoryCd + "' ";
            if (startDate != "")
            {
                SQL += " and (B.trx_date>='" + startDate + "' )";
            }
            if (endDate != "")
            {
                SQL += " and (B.trx_date<='" + endDate + "' )";
            }
            if (processCd != "")
            {
                SQL += " AND (B.PROCESS_CD='" + processCd + "' )";
            }
            if (JoNo != "")
            {
                SQL += " AND B.JOB_ORDER_NO ='" + JoNo + "'";
            }
            SQL = SQL + "          GROUP BY factory_cd,job_order_no,process_cd,process_garment_type,process_type,process_production_factory )B JOIN JO_HD PO ON "; 
            SQL = SQL + "          B.JOB_ORDER_NO=PO.JO_NO JOIN STYLE_HD SL ON ";  
            SQL = SQL + "          SL.STYLE_NO=PO.STYLE_NO AND SL.STYLE_REV_NO=PO.STYLE_REV_NO  ";
            SQL = SQL + "          JOIN (select A.jo_no,sum(A.qty) ";
            SQL = SQL + "          Order_Qty from jo_dt A WHERE EXISTS(SELECT TOP 1 1 FROM prd_jo_daily_stock B where ";
            SQL = SQL + "          B.factory_cd='" + factoryCd + "' ";
            if (startDate != "")
            {
                SQL += " and (B.trx_date>='" + startDate + "' )";
            }
            if (endDate != "")
            {
                SQL += " and (B.trx_date<='" + endDate + "' )";
            }
            if (processCd != "")
            {
                SQL += " AND (B.PROCESS_CD='" + processCd + "' )";
            }
            if (processType != "")
            {
                SQL += " AND B.PROCESS_TYPE='" + processType + "' ";
            }
            if (productionFactory != "")
            {
                SQL += " AND B.PROCESS_PRODUCTION_FACTORY='" + productionFactory + "' ";
            }
            if (JoNo != "")
            {
                SQL += " AND B.JOB_ORDER_NO ='" + JoNo + "'";
            }
            SQL = SQL + "      AND A.JO_NO=B.JOB_ORDER_NO) group by A.jo_no) DT ON ";
            SQL = SQL + "          PO.JO_NO=DT.JO_NO where 1=1 AND B.OUTPUT_QTY<>0 AND B.FACTORY_CD=CASE WHEN B.PROCESS_TYPE='P' THEN po.CO_FACTORY_CD ELSE B.FACTORY_CD END    ";
            if (garmentType != "")
            {
                SQL += " and (PO.Garment_Type_Cd='" + garmentType + "')";
            }
            switch (washType)
            {
                case "NW":
                    SQL += " AND Po.WASH_TYPE_CD ='" + washType + "'";
                    break;
                case "WASH":
                    SQL += " AND (Po.WASH_TYPE_CD NOT IN('NW') AND Po.WASH_TYPE_CD IS NOT  NULL)";
                    break;
            }
            return DBUtility.GetTable(SQL, "MES");
        }




        //pcmSummaryBundling
            
        public static DataTable GetSummaryBundlingSize(string factoryCd, string layNoFrom, string layNoTo, string JoNo, string LayNo)
        {
           
            string SQL = @"SELECT E.SIZE_CODE,
	                            E.SEQ1,
	                            F.SEQ2	
                        FROM 
                            (	             
                                    SELECT  DISTINCT 
                                            B.SC_NO,
					                        A.SIZE_CD AS SIZE_CODE,
					                        C.SEQUENCE AS SEQ1
                                    FROM    dbo.CUT_BUNDLE_HD AS A
                                            INNER JOIN dbo.JO_HD AS B ON a.JOB_ORDER_NO = b.JO_NO
                                            INNER JOIN dbo.SC_SIZE AS C ON B.SC_NO=C.SC_NO AND a.SIZE_CD=c.SIZE_CODE 
                                    WHERE   C.SIZE_TYPE = '1' ";

            if (factoryCd != "")
            {
                SQL += " and A.factory_cd='" + factoryCd + "'";
            }
            if (layNoFrom != "")
            {
                SQL += " and A.lay_no >= '" + layNoFrom + "'";
            }
            if (layNoTo != "")
            {
                SQL += " and A.lay_no <='" + layNoTo + "'";
            }
            if (JoNo != "")
            {
                SQL += " and A.job_order_no LIKE '%" + JoNo + "%'";
            }
            SQL = SQL + @"        ) E left join (
                             SELECT  DISTINCT
                                    B.SC_NO ,
                                    A.SIZE_CD AS SIZE_CODE ,
                                    C.SEQUENCE AS SEQ2
                             FROM   dbo.CUT_BUNDLE_HD AS A
                                    INNER JOIN dbo.JO_HD AS B ON a.JOB_ORDER_NO = b.JO_NO
                                    INNER JOIN dbo.SC_SIZE AS C ON B.SC_NO = C.SC_NO
                                                                   AND a.SIZE_CD = c.SIZE_CODE
                             WHERE   C.SIZE_TYPE = '2' ";
            if (factoryCd != "")
            {
                SQL += " and A.factory_cd='" + factoryCd + "'";
            }
            if (layNoFrom != "")
            {
                SQL += " and A.lay_no >= '" + layNoFrom + "'";
            }
            if (layNoTo != "")
            {
                SQL += " and A.lay_no <='" + layNoTo + "'";
            }
            if (!LayNo.Equals(""))
            {
                SQL += " AND A.LAY_NO IN ('" + LayNo + "')";
            }
            if (JoNo != "")
            {
                SQL += " and A.job_order_no LIKE '%" + JoNo + "%'";
            }
            SQL = SQL + "        ) F ON   ";
            SQL = SQL + "		E.sc_no=F.sc_no and E.SIZE_CODE=F.SIZE_CODE order by SEQ1,SEQ2 ";
            return DBUtility.GetTable(SQL, "MES");
        }
        
        public static DataTable GetSummaryBundlingHeader(string JoNo)
        {
            string SQL = " 		select a.SC_NO,d.NAME CUSTOMER_NAME,sum(c.qty) ORDER_QTY  ";
            SQL = SQL + "        FROM jo_hd a,jo_dt c,gen_customer d ";
            SQL = SQL + "		WHERE a.jo_no=c.jo_no AND ";
            SQL = SQL + "		a.customer_cd=d.customer_cd  ";
            if (JoNo != "")
            {
                SQL += " and a.jo_no = '" + JoNo + "'";
            }
            SQL = SQL + "        GROUP BY A.SC_NO,D.NAME ";
            return DBUtility.GetTable(SQL, "MES");
        }

       
        public static DataTable GetSummaryBundling(string factoryCd, string layNoFrom, string layNoTo, string JoNo, string LayNo, string StartDate, string EndDate)
        {


            string SQL = @"  SELECT COLOR_CD,
			                        SIZE_CD,
			                        SUM(QTY) AS QTY
                             FROM   dbo.CUT_BUNDLE_HD ";
            SQL += " WHERE factory_cd='" + factoryCd + "' and status='Y'";
            if (!layNoFrom.Equals(""))
            {
                SQL += " and lay_no >= '" + layNoFrom + "'";
            }
            if (!layNoTo.Equals(""))
            {
                SQL += " and lay_no <='" + layNoTo + "'";
            }
            if (!LayNo.Equals(""))
            {
                SQL += " AND LAY_NO IN ('" + LayNo + "')";
            }
            if (JoNo != "")
            {
                SQL += " and job_order_no LIKE '%" + JoNo + "%'";
            }

            if (!StartDate.Equals(""))
            {
                StartDate += " 00:00:00.000";
                SQL += " AND actual_print_date >= '" + StartDate + "'";
            }
            if (!EndDate.Equals(""))
            {
                EndDate += " 23:59:59.999";
                SQL += " AND actual_print_date <= '" + EndDate + "'";
            }
            SQL = SQL + "        group by color_cd,size_cd order by color_cd ";
            return DBUtility.GetTable(SQL, "MES");
        }
        
        public static int ParseInt(string s)
        {
            int value = 0;
            int.TryParse(s, out value);
            return value;
        }

        


        //GetProTranWIPSummary
        public static DataTable GetProTranWIPSummaryJoOrGo(string factoryCd, string strJo, string strGo)
        {
            string SQL = " ";
            if (strJo != "")
            {
                SQL = SQL + " select SC_NO,JO_NO FROM JO_HD WHERE 1=1 AND JO_NO LIKE '" + strJo + "%' ";
            }
            if (strGo != "")
            {
                SQL = SQL + " select distinct SC_NO FROM JO_HD WHERE 1=1 AND SC_NO like '" + strGo + "%' ";
            }
            if (factoryCd != "")
            {
                SQL = SQL + "and factory_cd='" + factoryCd + "'";
            }
            SQL = SQL + "AND  BUYER_PO_DEL_DATE >= convert(varchar(10),dateadd(yy,-1,getdate()),101) ";
            SQL = SQL + "and BUYER_PO_DEL_DATE<=convert(varchar(10),DATEADD(yy,1,getdate()),101) ";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable GetProTranWIPSummaryStyle(string factoryCd, string styleNo, string styleDesc)
        {
            string SQL = " select TOP 100  STYLE_NO,STYLE_DESC from  STYLE_HD s ";
            SQL = SQL + "where 1=1 ";
            if (styleNo != "")
            {
                SQL = SQL + "and STYLE_NO like '%" + styleNo + "%' ";
            }
            if (styleDesc != "")
            {
                SQL = SQL + "and STYLE_DESC like '%" + styleDesc + "%' ";
            }
            SQL = SQL + "and exists(select top 1 1 from jo_hd j INNER JOIN sc_hd SC ";
            SQL = SQL + "ON J.SC_NO=SC.SC_NO  where 1=1 ";
            if (factoryCd != "")
            {
                SQL = SQL + " and j.factory_cd='" + factoryCd + "'";
            }
            SQL = SQL + " AND J.STYLE_NO=s.STYLE_NO  ";
            SQL = SQL + "AND SC.STYLE_REV_NO=S.STYLE_REV_NO ";
            SQL = SQL + "AND  J.BUYER_PO_DEL_DATE >= convert(varchar(10),dateadd(yy,-1,getdate()),101) ";
            SQL = SQL + "and BUYER_PO_DEL_DATE<=convert(varchar(10),DATEADD(yy,1,getdate()),101)) ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetProTranWIPSummaryDept(string factoryCd)
        {
            string SQL = " select '' PRC_CD,'All' NM union all select 'KeyPart' PRC_CD,'KeyProcess(主要部门)' NM union all SELECT PRC_CD,NM FROM GEN_PRC_CD_MST  A ";
            SQL = SQL + "WHERE 1=1 ";
            if (factoryCd != "")
            {
                SQL += " and FACTORY_CD='" + factoryCd + "' ";
            }
            SQL = SQL + "AND EXISTS(SELECT TOP 1 1 fROM dbo.PRD_FTY_PROCESS_FLOW B ";
            SQL = SQL + "WHERE B.PROCESS_CD=A.PRC_CD AND B.PROCESS_GARMENT_TYPE=A.GARMENT_TYPE ";
            SQL = SQL + " AND B.FACTORY_CD=A.FACTORY_CD) ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetToProcessCd(string factoryCd, string GarmnetTypeCd, string ProcessCD)
        {

            string SQL = "select '' PRC_CD,'ALL' AS NM,1 AS DISPLAY_SEQ union all select DISTINCT PRC_CD,NM,DISPLAY_SEQ from gen_prc_cd_mst a where 1=1 AND ACTIVE='Y'";
            if (factoryCd != "")
            {
                SQL += " and factory_cd='" + factoryCd + "'";
            }
            if (GarmnetTypeCd != "" && (factoryCd == "YMG" || factoryCd == "GEG"))
            {
                SQL += " and GARMENT_TYPE='" + GarmnetTypeCd + "'";
            }
            if (ProcessCD != "")
            {
                SQL += " and exists (select top 1 1 from dbo.PRD_FTY_PROCESS_FLOW b ";
                SQL += " where 1=1";
                if (factoryCd != "")
                {
                    SQL += " and b.factory_cd='" + factoryCd + "'";
                }
                SQL += " and b.process_cd='" + ProcessCD + "'";
                SQL += " AND a.prc_cd=b.next_process_cd )";
            }
            SQL += " ORDER BY DISPLAY_SEQ ";
            return DBUtility.GetTable(SQL, "MES");
        }

        public static DataTable GetWorkListdata(string FactoryCd, string BeginDate, string EndDate)
        {
            string sql = " exec Rpt_Gen_WorkList '" + FactoryCd + "','" + BeginDate + "','" + EndDate + "'";
            return DBUtility.GetTable(sql, "MES");
        }
    }
}
