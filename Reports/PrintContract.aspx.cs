using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.Data.Common;
using MESComment;

public partial class Reports_PrintContract :pPage 
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        { }
        lblMessage.Visible = false;
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        //string strContractNo = txtContract.Text.Trim();
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
            "/OAS.Report/PrintContract";
        reportViewer.ServerReport.ReportServerCredentials = new CustomReportCredentials("gfg1_029", "Sa2011", "GFG1");


        List<ReportParameter> parameters = new List<ReportParameter>();
        //parameters.Add(new ReportParameter("ContractNo", "OAS-GEG-1311-0006"));
        parameters.Add(new ReportParameter("ContractNo", dtJoList.Rows[0][0].ToString()));
        reportViewer.ServerReport.SetParameters(parameters);
        this.reportViewer.ShowParameterPrompts = false;
        this.reportViewer.ServerReport.Refresh();


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