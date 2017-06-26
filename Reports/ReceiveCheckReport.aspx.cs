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

public partial class Reports_ReceiveCheckReport : pPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        { }
        lblMessage.Visible = false;

    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {

        DataTable dtJoList = GetOASJoList(txtContract.Text.Trim().ToString());
        if (!(dtJoList.Rows.Count > 0))
        {
            lblMessage.Visible = true;
            reportViewer.Visible = false;
            return;
        }
        reportViewer.Visible = true;
        // Set the processing mode for the ReportViewer to Remote        
        reportViewer.ProcessingMode = ProcessingMode.Remote;

        ServerReport serverReport = reportViewer.ServerReport;

        // Set the report server URL and report path
        serverReport.ReportServerUrl =
            new Uri("http://192.168.7.112/ReportServer");
        serverReport.ReportPath =
            "/OAS.Report/ReceiveCheckReport";
        reportViewer.ServerReport.ReportServerCredentials = new CustomReportCredentials("gfg1_029", "Sa2011", "GFG1");


        List<ReportParameter> parameters = new List<ReportParameter>();
        string CNJO = "";
        string JOs = dtJoList.Rows[0][1].ToString();
        var JO = JOs.Split(';');
        //parameters.Add(new ReportParameter("ContractNo", "OAS-GEG-1311-0006"));
        for (int i = 0; i < JO.Length; i++)
        {
            CNJO += dtJoList.Rows[0][0].ToString() + ":" + JO[i].ToString();
            if (i < (JO.Length - 1))
            {
                CNJO += ";";
            }
        }
        parameters.Add(new ReportParameter("where", GetWhere(CNJO)));
        //parameters.Add(new ReportParameter("ContractNo",dtJoList.Rows[0][0].ToString()));
        //parameters.Add(new ReportParameter("JobOrderNo", dtJoList.Rows[0][1].ToString()));
        reportViewer.ServerReport.SetParameters(parameters);
        this.reportViewer.ShowParameterPrompts = false;
        this.reportViewer.ServerReport.Refresh();
    }


    private string GetWhere(string pairsCNJO)
    {
        if (string.IsNullOrEmpty(pairsCNJO)) return "1=1";
        var pairs = pairsCNJO.Split(';');
        StringBuilder strWhere = new StringBuilder();
        strWhere.Append(" and (1 = 0 ");

        foreach (var pair in pairs)
        {
            var keyvalue = pair.Split(':');
            if (keyvalue.Length < 2) throw new Exception("pairsCNJO 的值有错误!");
            var cn = keyvalue[0];
            var jo = keyvalue[1];

            strWhere.Append(" or (a.ContractNo = '" + cn + "' and b.JOB_ORDER_NO='" + jo + "')");
        }

        strWhere.Append(" )");

        return strWhere.ToString();

    }



    private static DataTable GetOASJoList(string ContractNo)
    {

        string SQL = "";
        SQL = @" SELECT B.CONTRACT_NO,LEFT(StuList,LEN(StuList)-1) as JOB_ORDER_NO FROM (
        SELECT CONTRACT_NO,
        (SELECT JOB_ORDER_NO+';' FROM PRD_OUTSOURCE_CONTRACT_DT 
          WHERE CONTRACT_NO=A.CONTRACT_NO AND CONTRACT_NO='" + ContractNo + @"'
          FOR XML PATH('')) AS StuList
        FROM PRD_OUTSOURCE_CONTRACT_DT A 
        WHERE CONTRACT_NO='" + ContractNo + @"'
        GROUP BY A.CONTRACT_NO
        ) B ";

        return DBUtility.GetTable(SQL, "MES");
    }


}