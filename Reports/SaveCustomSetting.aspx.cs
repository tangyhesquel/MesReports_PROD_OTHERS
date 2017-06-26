using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;
using System.IO;

public partial class Reports_SaveCustomSetting : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
       
        string FtyCD = "";
        string strmsg="";
        string ReportName = "";
        string user = "";
        string FieldsName = "";

        if (Request.QueryString["FtyCD"] != null)
        {
            FtyCD = Request.QueryString["FtyCD"].ToString();
        }
        if (Request.QueryString["ReportName"] != null)
        {
            ReportName = Request.QueryString["ReportName"].ToString();
        }
        if(Request.QueryString["User_ID"] != null)
        {
            user = Request.QueryString["User_ID"].ToString();
        }
         
        if (Request.QueryString["Cstring"] != null)
        {
          FieldsName = Request.QueryString["Cstring"].ToString();  //</Added by: DaiJ at: 2012/12/03>
            DataTable SaveData = MESComment.MesRpt.SaveCustomReportList(FtyCD, "REPORT", "REPORT", ReportName, user, FieldsName);
            if (SaveData.Rows.Count == 1)
            {
                strmsg = "Save successfully!";
            }
        }
               
     
            this.lblMessage.Text = strmsg;
            this.lblMessage.Visible = true;
        
    }
     
   
   
}
