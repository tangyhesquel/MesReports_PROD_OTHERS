using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web.UI.WebControls;

/// <summary>
/// Added By Zikuan - MES-097 3-Dec-2013
/// </summary>
namespace MESComment
{
    public class CurrentOrderStatus
    {
        public CurrentOrderStatus()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static DataTable GetShipInfo(string JO, string FromDate, string ToDate, DbConnection conns)
        {
            string SQL = " SELECT DISTINCT a.REFERENCE_NO, a.BUYER_PO_NO, b.TRANS_CD, b.TRANS_DATE, b.REMARKS ";
            SQL = SQL + " FROM INVENTORY.INV_TRANS_LINES a ";
            SQL = SQL + " INNER JOIN INVENTORY.INV_TRANS_HD b ON a.TRANS_HEADER_ID = b.TRANS_HEADER_ID ";
            SQL = SQL + " WHERE TRANS_CD in ('KBI','WBI','KBI-FS') ";

            if (!string.IsNullOrEmpty(JO))
            {
                SQL = SQL + " AND a.REFERENCE_NO = '" + JO + "' ";
            }

            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                SQL = SQL + " AND TRANS_DATE BETWEEN TO_DATE('" + FromDate + "','YYYY-MM-DD') AND TO_DATE('" + ToDate + "','YYYY-MM-DD') ";
            }

            return DBUtility.GetTable(SQL, conns);
        }

        public static DataTable GetCurrentOrderStatusInfo(string RunNO, string JO, string FromDate, string ToDate, string QueryType, DbConnection conns, string Factory)
        {
            string SQL = "exec [SP_GET_CurrentOrderStatus] '" + RunNO + "','" + JO + "','" + FromDate + "','" + ToDate + "','" + QueryType + "','" + Factory + "';";

            return DBUtility.GetTable(SQL, conns);
        }
    }
}
