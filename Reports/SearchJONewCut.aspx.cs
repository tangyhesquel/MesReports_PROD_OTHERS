using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_SearchJO : pPage
{
    string strJoList = ""; 
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        gvBody.DataSource = GetJO(txtSCNo.Text.ToString());
        gvBody.DataBind();
        btnSelect.Visible = true;
        
    }
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                break;
            case DataControlRowType.DataRow:
                //e.Row.Cells[0].Text = "<input type='checkbox' name='rowsCheck' onclick='chooseJo()' class='noborder' value="+e.Row.Cells[1].Text+" />";
                break;
        }
    }
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        
        foreach (GridViewRow gvr in this.gvBody.Rows)   
        {   
             Control ctl = gvr.FindControl("ckb");   
             CheckBox ck = (CheckBox)ctl;   
             if (ck.Checked)   
             {   
                 TableCellCollection cell = gvr.Cells;   
                strJoList+= cell[2].Text+",";  
                
             }   
        }
        if (strJoList.Length > 1)
        {
            strJoList = strJoList.Substring(0, strJoList.Length - 1);
        }
        else
        {
           strJoList="";
        }
       //Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "", "chooseJo('" + strJoList + "');", true);
       // ClientScript.RegisterClientScriptBlock(this.GetType(), "", "<script>chooseJo('" + strJoList + "')</script>");
        ClientScript.RegisterClientScriptBlock(this.GetType(), "", "<script>window.returnValue='" + strJoList + "';window.close();</script>");
        
     }
    private static DataTable GetJO(string ScNo)
    {
        string SQL = @"  SELECT  JO_NO ,
                                SC_NO ,
                                B.SHORT_NAME
                        FROM    JO_HD AS A
                                INNER JOIN GEN_CUSTOMER AS B ON A.CUSTOMER_CD = B.CUSTOMER_CD
                        WHERE   SC_NO = '"+ScNo + @"'
                                AND A.STATUS <> 'D' AND NOT EXISTS (SELECT 1 FROM JO_COMBINE_MAPPING WHERE ORIGINAL_JO_NO=A.JO_NO)
                        ORDER BY JO_NO ";
        
        return MESComment.DBUtility.GetTable(SQL, "MES");
    }
}
