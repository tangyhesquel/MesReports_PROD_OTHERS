using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web.UI.WebControls;
using MESComment;

/// <summary>
///proMajorDailyOutSql 的摘要说明
/// </summary>

namespace MESComment
{
    public class proMajorDailyOutSql
    {
        public static DataTable GetProMajorBuyerList(string JoNo)
        {
            string SQL = "         select DT.ORDER_QTY,CONVERT(VARCHAR(10),BUYER_PO_DEL_DATE,120) AS BUYER_PO_DEL_DATE ";
            SQL = SQL + "       ,WASH_TYPE_DESC,PO.CUSTOMER_CD,CU.NAME,A.STYLE_DESC,PO.JO_NO ";
            SQL = SQL + "        from JO_HD PO,STYLE_HD A,GEN_CUSTOMER CU,GEN_WASH_TYPE WY, ";
            SQL = SQL + "        (select JO_NO,sum(qty) Order_Qty from JO_dt group by JO_NO) DT ";
            SQL = SQL + "        where PO.JO_NO=DT.JO_NO ";
            SQL = SQL + "        AND A.STYLE_NO=PO.STYLE_NO ";
            SQL = SQL + "        AND CU.CUSTOMER_CD=PO.CUSTOMER_CD ";
            SQL = SQL + "        AND PO.WASH_TYPE_CD = WY.WASH_TYPE_CD ";
            SQL = SQL + "        AND PO.JO_NO='" + JoNo + "' ";
            return DBUtility.GetTable(SQL, "MES");
        }


        public static DataTable GetProMajorProcessQtyList(string factoryCd, string JoNo, Boolean SplitSEW)
        {
            string SQL = "";
            string Split = "N";
            if (SplitSEW)
            {
                Split = "Y";
            }
            SQL += " EXEC [RPT_PROC_INQ_Major_OUT] '" + factoryCd + "','" + JoNo + "','"+Split+"' ";
            return DBUtility.GetTable(SQL, "MES");
        }

        //Added By ZouShiChang ON 2013.11.22 MES 024 Start
        public static DataTable GetProMajorProcessQtyListNew(string factoryCd, string JoNo, Boolean SplitSEW)
        {
            string SQL = "";
            string Split = "N";
            if (SplitSEW)
            {
                Split = "Y";
            }
            SQL += " EXEC [PROC_GET_MAJOR_OUTPUT_RPT] '" + factoryCd + "','" + JoNo + "','" + Split + "' ";
            
            return DBUtility.GetTable(SQL, "MES");
        }
        //Added By ZouShiChang ON 2013.11.22 MES 024 End


        public static DataTable GetProMajorProcessQtyList(string factoryCd, string JoNo, string ProcessCd, string ProductionLineCd)
        {
            string SQL = "";
            SQL += "EXEC dbo.GET_WIP_DATA '" + factoryCd + "','" + JoNo + "','" + ProcessCd + "','%','%','" + ProductionLineCd + "'";
            return DBUtility.GetTable(SQL, "MES");
        }

    }
}