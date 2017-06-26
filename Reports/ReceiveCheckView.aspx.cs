using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.Data.Common;
using MESComment;
using System.Text;

public partial class Reports_ReceiveCheckView : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
           
        }
   

    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
      
        BindData();
        
    }
    private void BindData()
    {
        DataTable contractdt=GetReceiveData();
        DataTable ypddt = GetEscmYpd(contractdt);
        contractdt.Columns.Add("YPD");
        foreach (DataRow dr in contractdt.Rows)
        {
            DataRow[] ypddrs = ypddt.Select(string.Format("SC_NO='{0}'",dr["SC_NO"]));
            if (ypddrs.Length == 0)
                dr["YPD"] = 0;
            else
                dr["YPD"] = ypddrs[0]["PPO_YPD"];
        }
        datalist.DataSource = contractdt;
        datalist.DataBind();
    }
    private DataTable GetReceiveData()
    {
        string where = GetWhere();
        if(string.IsNullOrEmpty(where))
            return new DataTable();
        string SQL = string.Format("exec [OAS].[usp_OAS_ReceiveCheckReport] '{0}'",where);
        return DBUtility.GetTable(SQL, "OAS");
    }
    public string GetWhere()
    {
        if ((string.IsNullOrEmpty(txtStartTrxDate.Text) && string.IsNullOrEmpty(txtStartTrxDate.Text)) && string.IsNullOrEmpty(txtContract.Text) && string.IsNullOrEmpty(txtsuppid.Text))
        {
                return "";
        }
        else
        {
            if (string.IsNullOrEmpty(txtsuppid.Text)&&string.IsNullOrEmpty(txtContract.Text) && (string.IsNullOrEmpty(txtStartTrxDate.Text) || string.IsNullOrEmpty(txtStartTrxDate.Text)))
                return "";
        }
        StringBuilder sb = new StringBuilder();
        if (!string.IsNullOrEmpty(txtStartTrxDate.Text) && !string.IsNullOrEmpty(txtEndTrxDate.Text))
        {
            sb.AppendFormat(" and a.LastUpdatedTime between ''{0}'' and ''{1}''", txtStartTrxDate.Text,txtEndTrxDate.Text);
        }
        if (!string.IsNullOrEmpty(txtContract.Text))
        {
            sb.AppendFormat(" and a.ContractNo like ''{0}%''", txtContract.Text);
        }
        if (!string.IsNullOrEmpty(txtsuppid.Text))
        {
            sb.AppendFormat(" and a.SUPPLIER_CD = ''{0}''", txtsuppid.Text);
        }
        return sb.ToString();
    }
    public DataTable GetEscmYpd(DataTable dt)
    {
        if (dt.Rows.Count == 0)
            return null;
        DbConnection eelConn = MESComment.DBUtility.GetConnection("EEL");
        DataTable godt = GetScTable(dt);
        MESComment.DBUtility.InsertRptTempData(godt, eelConn);
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat(" select sc_no,max(PPO_YPD) as PPO_YPD from scx_JOI_go_fabric  where exists (select f1 from escmreader.rpt_tmp where f1=sc_no) and fabric_type_cd in ('B','BD') group by sc_no");
        DataTable ypddt= DBUtility.GetTable(sb.ToString(), eelConn);
        DBUtility.CloseConnection(ref eelConn);
        return ypddt;
    }
    private DataTable GetScTable(DataTable dt)
    {
        DataTable godt = new DataTable();
        godt.Columns.Add("SC_NO");
        List<string> golist = new List<string>();
        foreach(DataRow dr in dt.Rows)
        {
            if(golist.Contains(dr["SC_NO"].ToString()))
            {
                continue;
            }
            godt.Rows.Add(new object[] { dr["SC_NO"].ToString() });
            golist.Add(dr["SC_NO"].ToString());
        }
        return godt;
    }
    //private void Init()
    //{
    //    DataTable dt= MESComment.MesRpt.GetFactoryCd();
    //    DataRow newdr=dt.NewRow();
    //    newdr["FACTORY_ID"]="ALL";
    //    dt.Rows.InsertAt(newdr, 0);
    //    ddlFtyCd.DataSource = dt;
    //    ddlFtyCd.DataTextField = "FACTORY_ID";
    //    ddlFtyCd.DataValueField = "FACTORY_ID";
    //    ddlFtyCd.DataBind();
    //}
    


}