using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Task_GetUserName : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AjaxPro.Utility.RegisterTypeForAjax(typeof(Task_GetUserName));
    }
    [AjaxPro.AjaxMethod]
    public void SetUserName(string id)
    {
        Session["UserName"] = id;
    }
}
