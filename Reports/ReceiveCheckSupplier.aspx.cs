using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MESComment;
using System.Data;

public partial class Reports_ReceiveCheckSupplier : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GetReceiveData(1);
        }
       
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        GetReceiveData(1);
        //gvBody.DataSource = MESComment.MesRpt.GetJO(txtTrxDate.Text,ddlStatus.SelectedItem.Value,txtCustomer.Text.Trim(),txtSCNo.Text.Trim(),CurrentSite.ToString());
        //gvBody.DataBind();
    }
    //protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    switch (e.Row.RowType)
    //    {
    //        case DataControlRowType.Header:
    //            break;
    //        case DataControlRowType.DataRow:
    //            e.Row.Cells[0].Text = "<input type='radio' name='rowsCheck' onclick='chooseJo()' class='noborder' value=" + e.Row.Cells[1].Text + " />";
    //            break;
    //    }
    //}

    private void GetReceiveData(int pageindex)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendFormat(@"DECLARE @RecordCount INT

                        SELECT @RecordCount=COUNT(1) FROM ORA.GEN_SUPPLIER WHERE ACTIVE='Y' {0}

                        begin
                        WITH record AS(SELECT ROW_NUMBER() OVER(ORDER BY SUPPLIER_CD) AS rowid,* FROM ORA.GEN_SUPPLIER WHERE ACTIVE='Y' {0} )

                        SELECT *,@RecordCount AS RecordCount FROM   record WHERE rowid BETWEEN {1} AND {2}		
                        end
                        ", GetWhere(), (pageindex - 1) * 20 + 1, pageindex * 20);
        DataTable dt= DBUtility.GetTable(sb.ToString(), "OAS");
        if (dt.Rows.Count == 0)
            return;
       
        datalist.DataSource = dt;
        datalist.DataBind();
        SetPage(dt.Rows[0]["RecordCount"].ToInt("c"), pageindex, 20);
       
    }
    private string GetWhere()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if (!string.IsNullOrEmpty(txtid.Text))
        {
            sb.AppendFormat(" and SUPPLIER_CD like '{0}%'",txtid.Text.ToUpper());
        }
        if (!string.IsNullOrEmpty(txtname.Text))
        {
            sb.AppendFormat("  and NAME like '{0}%' ", txtname.Text.ToUpper());
        }
       return sb.ToString();

    }
    protected void PageIndexChanging_Command(object sender, CommandEventArgs e)
    {
        GetReceiveData(e.CommandArgument.ToInt("c"));
    }
    private Button GetPageButton(string id)
    {
        //datalist.Controls[datalist.Controls.Count - 1][0].
        return datalist.Controls[datalist.Controls.Count - 1].Controls[0].FindControl(id) as Button;
    }
    private void SetPage(int recordCount,int pageIndex,int pageSize)
    {
        //Label lbl2 = (Label)Repeater1.Controls[Repeater1.Controls.Count - 1].Controls[0].FindControl("Label1");
        Button btnfirst = GetPageButton("btnfirst");
        Button btnpre = GetPageButton("btnpre");
        Button btnnext = GetPageButton("btnnext");
        Button btnlast = GetPageButton("btnlast");
        if (btnfirst == null)
            return;
        int totalPage = (recordCount * 1.0 / pageSize).ToInt("c");
        if (totalPage <= 1)
        {
            btnfirst.Visible = false;
            btnpre.Visible = false;
            btnnext.Visible = false;
            btnlast.Visible = false;
            return;
        }
        btnfirst.Enabled = true;
        btnpre.Enabled = true;
        btnnext.Enabled = true;
        btnlast.Enabled = true;
        if (pageIndex == 1)
        {
            btnfirst.Enabled = false;
            btnpre.Enabled=false;
        }
        else if (pageIndex == totalPage)
        {
            btnnext.Enabled = false;
            btnlast.Enabled = false;
        }
        btnfirst.CommandArgument = "1";
        btnpre.CommandArgument = (pageIndex - 1).ToString();
        btnnext.CommandArgument = (pageIndex + 1).ToString();
        btnlast.CommandArgument = totalPage.ToString();
    }
}
