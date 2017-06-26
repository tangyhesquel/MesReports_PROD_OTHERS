using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Reports_DetailPrice : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(Request.QueryString["scNo"]!="")
        {
            gvDetail.DataSource = MESComment.MesRpt.GetDetailPrice(Request.QueryString["scNo"].ToString());
            gvDetail.DataBind();
        }
    }
}
