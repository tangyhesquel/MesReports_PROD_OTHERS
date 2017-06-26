using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Threading;
using System.Globalization;

public partial class Mes_Default : pPage
{
    public string factoryCd = "";
    string strculture = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["site"] != null)
        {
            factoryCd = Request.QueryString["site"].ToString();
            if (factoryCd == "PTX" || factoryCd == "EGM" || factoryCd == "TIL" || factoryCd == "EGV" || factoryCd == "EAV")
            {
                strculture = "en";
            }
            else
            {
                strculture = "zh";
            }
        }
        if (!IsPostBack)
        {
           
            if (strculture == "en")
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("zh-CN");

            }

        }
    }
}
