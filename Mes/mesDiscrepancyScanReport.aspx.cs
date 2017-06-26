using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Mes_mesDiscrepancyScanReport :pPage
{

    protected string doc_NO;
    protected string user;
    protected int total=0;

    protected void Page_Load(object sender, EventArgs e)
    {

        doc_NO = Request.QueryString["docNO"].ToString();
        user=Request.QueryString["userID"].ToString();
        if (!string.IsNullOrEmpty(doc_NO))
        {
            DataTable headerTable=  MESComment.proPulloutSql.GetDiscrepancyHeadData(doc_NO);
            
            if(headerTable!=null && headerTable.Rows.Count>0 )
            {
                LiteralFactory.Text=headerTable.Rows[0]["FACTORY_CD"].ToString();
                LiteralProcess.Text=headerTable.Rows[0]["PROCESS_CD"].ToString();
                LiteralLine.Text=headerTable.Rows[0]["PRODUCTION_LINE_CD"].ToString();
                LiteralUser.Text=user;
                LiteralDate.Text=DateTime.Now.ToString();
                LiteralDocNO.Text = doc_NO;
            }


            DataTable detailTable = MESComment.proPulloutSql.GetDiscrepancyDetail(doc_NO);

            if (detailTable != null )
            {
                total = detailTable.Rows.Count==0?0:Convert.ToInt16(detailTable.Compute("Sum(QTY)", "1=1"));
                RepeaterData.DataSource = detailTable;
                RepeaterData.DataBind();
            }
            
            
        }
    }


}