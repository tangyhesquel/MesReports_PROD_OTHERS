using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.Common;

/// <summary>
/// Summary description for ProTranWIPSummary
/// </summary>

namespace MESComment
{
    public class Employeeoutput
    {
        public Employeeoutput()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static DataTable GetEmpOutputCheckData(string factoryCd, string startDate, string endDate, string JoNo, string OperCD, bool ChkLess, bool ChkRework, string RunNo)
        {
            string strChkless, strChkRework;
            if (ChkLess == false)
            {
                strChkless = "";
            }
            else
            {
                strChkless = "Y";
            }

            if (ChkRework == false)
            {
                strChkRework = "";
            }
            else
            {
                strChkRework = "Y";
            }
            //string sql = "EXEC [PRC_RPT_EMPLOYEE_OUTPUT_CHK] '" + factoryCd + "','','" + startDate + "','" + endDate + "','" + JoNo + "','" + OperCD + "','" + ChkLess + "','" + ChkRework + "','" + RunNo + "'";
            string sql = "EXEC [PRC_RPT_EMPLOYEE_OUTPUT_CHK] '" + factoryCd + "','','" + startDate + "','" + endDate + "','" + JoNo + "','" + OperCD + "','" + strChkless + "','" + strChkRework + "','" + RunNo + "'";
            return DBUtility.GetTable(sql, "MES_UPDATE");   
        }

    }
}