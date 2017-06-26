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
    /// proDailyTransReport 的摘要说明
    /// </summary>
    public class proDailyTransReport
    {
        public static DataTable GetPrdDailyOutput(string prodGroup, string factoryCd, string garmentType, string washtype, string StartDate, string processCode, string processType, string productionFactory, string Line, string uuidTitle, DbConnection conn)
        {
            //Modified by MunFoong on 2014.07.24, MES-139
            string SQL = "exec SP_Pro_DailyTransReport '" + prodGroup + "','" + factoryCd + "','" + garmentType + "','" + washtype + "','" + StartDate + "','" + processCode + "','" + processType + "','" + productionFactory + "','" + Line + "','" + uuidTitle + "'; select * from " + uuidTitle;

            return DBUtility.GetTable(SQL, conn);
        }

    }
}