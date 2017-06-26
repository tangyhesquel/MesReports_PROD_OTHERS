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
    /// proShipmentStatusReport 的摘要说明

    /// </summary>
    public class proShipmentStatusReport
    {
        public static DataTable GetPrdShipmentStatus(string factoryCd, string StartDate, string EndDate, string garmentType, string processCode, string processType, string prodFactory, string uuidTitle, DbConnection conn)
        {
            if (garmentType == "ALL")
            { garmentType = "%"; }
            if (processType == "")
            { processType = "%"; }
            if (prodFactory == "")
            { prodFactory = "%"; }
            string SQL = "exec RPT_Pro_ShipmentStatusReport '" + factoryCd + "','" + StartDate + "','" + EndDate + "','" + garmentType + "','" + processCode + "','" + processType + "','" + prodFactory + "','" + uuidTitle + "'; select * from " + uuidTitle;

            return DBUtility.GetTable(SQL, conn);
        }

    }
}