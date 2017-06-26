using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web.UI.WebControls;

namespace MESComment
{
    public class ProCycleAlertSql
    {
        public ProCycleAlertSql()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static void Ini_CT_Alert_DDL(DropDownList ddl)
        {
            string sqlstr = "select FACTORY_ID from gen_factory where active='Y'";
            ddl.DataTextField = "FACTORY_ID";
            ddl.DataValueField = "FACTORY_ID";
            ddl.DataSource = DBUtility.GetTable(sqlstr, "MES");
            ddl.DataBind();
        }

        //Added By ZouShiChang ON 2013.08.29 Start MES029
        public static void Ini_CT_Alert_DDL(string Garment_Type, DropDownList ddl, string factory_cd)
        {

            string SQL = "";
            SQL += " select 'ALL' PROCESS_CD,'ALL' SHORT_NAME union all ";
            SQL += " select distinct PROCESS_CD,SHORT_NAME from PRD_fty_process_flow F,dbo.GEN_PRC_CD_MST M ";
            SQL += " WHERE F.PROCESS_CD=M.PRC_CD AND F.FACTORY_CD = M.FACTORY_CD ";
            SQL += " AND F.PROCESS_GARMENT_TYPE=M.GARMENT_TYPE AND M.ACTIVE='Y' "; 
            SQL += " AND F.factory_cd='" + factory_cd + "'";
            if (!Garment_Type.Equals("ALL"))
            {
                SQL += " AND M.GARMENT_TYPE='" + Garment_Type + "'";
            }
            ddl.DataTextField = "SHORT_NAME";
            ddl.DataValueField = "PROCESS_CD";
            ddl.DataSource = DBUtility.GetTable(SQL, "MES");
            ddl.DataBind();

            /*
            string SQL = "";
            SQL += " select 'ALL' PROCESS_CD,'0' DISPLAY_SEQ union all ";
            SQL += " select distinct M.PRC_CD AS PROCESS_CD,M.DISPLAY_SEQ from PRD_fty_process_flow F,dbo.GEN_PRC_CD_MST M ";
            SQL += " WHERE F.PROCESS_CD=M.PRC_CD AND F.FACTORY_CD = M.FACTORY_CD ";
            SQL += " AND F.PROCESS_GARMENT_TYPE=M.GARMENT_TYPE AND F.PROCESS_TYPE=M.PROCESS_TYPE "; 
            SQL += " AND F.factory_cd='" + factory_cd + "'";
            if (!Garment_Type.Equals("ALL"))
            {
                SQL += " AND M.GARMENT_TYPE='" + Garment_Type + "' ";
            }
            ddl.DataTextField = "PROCESS_CD";
            ddl.DataValueField = "PROCESS_CD";
            ddl.DataSource = DBUtility.GetTable(SQL, "MES");
            ddl.DataBind();
             */
        }

        //Added By ZouShiChang ON 2013.08.29 Start MES029

        public static DataTable Get_CT_Alert(string Factory_cd, string Garment_type, string Process_cd,string Process_Type,string ProdFactory,string BeginDate)
        {
           
            string SQL = "";
            if (Process_Type == "")
            { Process_Type = "ALL"; }
            //Modified by Zikuan Batch 4 - Add STOCK_DAYS 16-Jan-14
            SQL += "select job_order_no,factory_cd,process_cd,garment_Type,Process_type,Production_factory,customer_name,wash_type_cd,sam_group_cd,in_qty,out_qty,pullout_qty,discrepancy_qty,wip_qty,convert(varchar(10),isnull(out_date,in_date),120) date,FREEZING_DAYS,STOCK_DAYS from FN_CT_ALERT('" + Factory_cd + "','" + Process_cd + "','" + Garment_type + "','" + Process_Type + "','" + ProdFactory + "','" + BeginDate + "','ALL','Old') where alert_flag='Y'  ";
            if (!Garment_type.Equals("ALL"))
            {
                SQL += " and GARMENT_TYPE='" + Garment_type + "'";
            }
            SQL += "   order by process_cd,job_order_no";
            return DBUtility.GetTable(SQL, "MES");
        }
        public static DataTable Get_CT_Alert_DEV(string Factory_cd, string Garment_type, string Process_cd, string Process_Type, string ProdFactory, string BeginDate)
        {
            string SQL = "";
            if (Process_Type == "")
            { Process_Type = "ALL"; }
            SQL += "select job_order_no,factory_cd,process_cd,garment_type,Process_type,Production_factory,customer_name,wash_type_cd,sam_group_cd,in_qty,out_qty,pullout_qty,discrepancy_qty,wip_qty,convert(varchar(10),isnull(out_date,in_date),120) date,FREEZING_DAYS from FN_CT_ALERT('" + Factory_cd + "','" + Process_cd + "','" + Garment_type + "','"  + Process_Type+"','"+ ProdFactory + "','" + BeginDate + "','ALL','New') where alert_flag='Y' ";
            if (!Garment_type.Equals("ALL"))
            {
                SQL += " AND GARMENT_TYPE='" + Garment_type + "'";
            }
            SQL += " order by process_cd,job_order_no";

            return DBUtility.GetTable(SQL, "MES");
        }

    }
}
