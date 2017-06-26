using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

public partial class MES_MasOutSourcePrice_Form : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Expires   =   0; 
        Response.Cache.SetNoStore(); 
        Response.AppendHeader( "Pragma",   "no-cache");
        if (!Page.IsPostBack)
        {
            if (Request.QueryString["year"] != null && Request.QueryString["viewtype"] != null)
            {
                lbYear.Text = Request.QueryString["year"].ToString();
                lbView.Text = Request.QueryString["viewtype"].ToString() == "K" ? "Knit" : "Woven";

                DataTable tb = MESComment.DBUtility.GetTable(string.Format("select pcs_price,order_qty,sah_price,sah from prd_outsource_parameter where garment_type_cd='{0}' and year='{1}'",
                                                                            Request.QueryString["viewtype"], Request.QueryString["year"]), "MES_UPDATE");
                if (tb!=null && tb.Rows.Count > 0)
                {
                    txtPrice1.Text = tb.Rows[0][0].ToString();
                    txtPrice2.Text = tb.Rows[0][1].ToString();
                    txtPrice3.Text = tb.Rows[0][2].ToString();
                    txtPrice4.Text = tb.Rows[0][3].ToString();
                }

            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            object o = MESComment.DBUtility.ExecuteScalar(string.Format("select count(*) from prd_outsource_parameter where garment_type_cd='{0}' and year='{1}'", Request.QueryString["viewtype"], Request.QueryString["year"]), "MES_UPDATE");
            if (Convert.ToInt16(o) > 0)
            {
                MESComment.DBUtility.Insert(string.Format("update prd_outsource_parameter set pcs_price='{0}',order_qty='{1}',sah_price='{2}',sah='{3}' ,last_modi_date='{4}',last_modi_user_id='{5}' where garment_type_cd='{6}' and year='{7}'",
                                     txtPrice1.Text, txtPrice2.Text, txtPrice3.Text, txtPrice4.Text, DateTime.Now.ToString(), "SA", Request.QueryString["viewtype"], Request.QueryString["year"]), "MES_UPDATE");
            }
            else
            {
                MESComment.DBUtility.Insert(string.Format("insert into prd_outsource_parameter(garment_type_cd,year,pcs_price,order_qty,sah_price,sah,create_date,create_user_id) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                                    Request.QueryString["viewtype"], Request.QueryString["year"], txtPrice1.Text, txtPrice2.Text, txtPrice3.Text, txtPrice4.Text, DateTime.Now.ToString(), "SA"), "MES_UPDATE");

            }

            Page.ClientScript.RegisterStartupScript(Page.GetType(), "ems_price_save", string.Format("alert('{0}');window.returnValue='change';window.close();", "保存成功！"), true);
        }
        catch (Exception ex)
        { 
            throw ex;
        }
    }
}
