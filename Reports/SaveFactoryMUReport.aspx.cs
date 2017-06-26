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

public partial class Reports_GEGMUReport : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string strRunNO = "";
        string FtyCD = "";
        string Utype = "";
        string strmsg="";
        if (Request.QueryString["RunNo"] != null)
        {
            strRunNO = Request.QueryString["RunNo"].ToString();
        }
        if (Request.QueryString["FtyCD"] != null)
        {
            FtyCD = Request.QueryString["FtyCD"].ToString();
        }
        if (Request.QueryString["Utype"] != null)
        {
            Utype = Request.QueryString["Utype"].ToString();
        }
        if (Utype == "M")
        {
            strmsg = "Factory Report Issue Log:";
            strmsg += "<br/>2011.12.22: Release Test Version To User.";
            strmsg += "<br/>2011.12.21: Release Data Source & Foumula Document.";
            strmsg += "<br/>2011.12.21: Release Internet Test Version.";
            strmsg += "<br/>2011.12.20: Begin Internal Testing.";
            strmsg += "<br/>2011.12.14: Begin Design.";            
        }
        if (Utype != "" && strRunNO != "" && FtyCD != "")
        {
            DataTable LockMudata = MESComment.MesRpt.LockMudata(FtyCD, strRunNO, Utype);
            if (LockMudata.Rows.Count == 1)
            {
                switch (Utype)
                {
                    case "Y":

                        strmsg = "Lock successfully!";
                        strmsg += "<br/>System will keep the data ";
                        strmsg += "<br/>and will not capture & generate from other system next time for same date.";
                        break;
                    case "N":

                        strmsg = "UnLock successfully!";
                        strmsg += "<br/>System will Clear it after 1 day once unlock it.";
                        strmsg += "<br/>You can re Lock it before the new query in current page";
                        break;                  
                        
                }

            }
        }
        else
        {
            this.lblMessage.Text = "Found Error!"; //+ Utype + strRunNO + FtyCD;             
        }
        if (Utype == "M")
        {
            this.lblMessage2.Text = strmsg;
            this.lblMessage2.Visible = true;
        }
        else
        {
            this.lblMessage.Text = strmsg;
            this.lblMessage.Visible = true;
        }
    }
     
   
   
}
