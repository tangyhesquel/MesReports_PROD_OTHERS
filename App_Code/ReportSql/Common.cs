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

/// <summary>
/// Summary description for Common
/// </summary>
namespace MESComment
{
    public class Common
    {
        public Common()
        {
            //
            // TODO: Add constructor logic here
            //
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
    }
}
