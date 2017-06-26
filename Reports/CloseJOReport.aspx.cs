using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Resources;
using System.Threading;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;

public partial class Reports_CloseJOReport : pPage
{
    int linePerPage = 100;
    protected void Page_Load(object sender, EventArgs e)
    {


        if (!IsPostBack)
        {

            InitControlData(); //初始化控件数据
            //added  by YeeHou
            if (ddlProcessCode.Items.FindByText("ALL") != null)
                ddlProcessCode.Items.Remove(ddlProcessCode.Items.FindByText("ALL"));
        }
    }

    private void InitControlData()
    {
        if (Request.QueryString["userId"] != null)
        {
            ddlFactoryCode.DataSource = MESComment.MesRpt.GetFactoryList(Request.QueryString["userId"].ToString());
        }
        else
        {
            ddlFactoryCode.DataSource = MESComment.MesRpt.GetFactoryList("");
        }
        ddlFactoryCode.DataValueField = "DEPARTMENT_ID";
        ddlFactoryCode.DataTextField = "DEPARTMENT_ID";
        ddlFactoryCode.DataBind();

        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                if (ddlFactoryCode.Items.Contains(new ListItem(Request.QueryString["site"].ToUpper())))
                    ddlFactoryCode.SelectedValue = Request.QueryString["site"].ToString();
            }
            else
            {
                ddlFactoryCode.SelectedValue = "GEG";
            }

            ddlProcessType.DataSource = MESComment.MesRpt.GetProcessType(Request.QueryString["site"].ToString());
            ddlProcessType.DataTextField = "PROCESS_TYPE_ID";
            ddlProcessType.DataValueField = "PROCESS_TYPE_VALUE";
            ddlProcessType.DataBind();

            ddlProcessCode.DataSource = MESComment.MesRpt.GetProcessCode(ddlFactoryCode.SelectedItem.Value, ddlGarmentType.SelectedItem.Value);
            ddlProcessCode.DataTextField = "SHORT_NAME";
            ddlProcessCode.DataValueField = "PROCESS_CD";
            ddlProcessCode.DataBind();


        }

    }

    protected void ddlGarmentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlProcessCode.DataSource = MESComment.MesRpt.GetProcessCode(ddlFactoryCode.SelectedItem.Value, ddlGarmentType.SelectedItem.Value);
        ddlProcessCode.DataBind();
    }

    private Boolean isJOInput()
    {
        Boolean result = false;
        if (txtJoNo.Text.Length > 0)
            result = true;
        return result;
    }

    //Created by YeeHou
    private Boolean validateJOInput()
    {
        Boolean result = false;
        if (txtJoNo.Text.Length >= 5)
            result = true;
        return result;
    }

    //Updated by YeeHou on 18/8/2015
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        DataTable dtBody = null;
        dtBody = null;
        this.gvBody.DataSource = dtBody;
        this.gvBody.DataBind();
        //Start update 18/8/2015
        if (isJOInput())
        {
            if (!validateJOInput())
            {
                JoNoErrorLabel.Visible = true;
                JoNoErrorLabel.Text = Resources.GlobalResources.STRING_JO_ERROR_MESSAGE;//Get error message from global resource file in different language
                return;
            }
            else
            {
                JoNoErrorLabel.Visible = false;
                JoNoErrorLabel.Text = "";
            }
        }
        else
        {
            JoNoErrorLabel.Visible = false;
            JoNoErrorLabel.Text = "";
        }

        if (ddlCloseState.SelectedValue.Equals("Y"))
        {
            if (String.IsNullOrEmpty(txtStartDate.Text))
            {
                CloseDateFromErrorLabel.Visible = true;
                CloseDateFromErrorLabel.Text = Resources.GlobalResources.STRING_CLOSE_DATE_FROM_MESSAGE;
                return;
            }
            else
            {
                CloseDateFromErrorLabel.Visible = false;
                CloseDateFromErrorLabel.Text = "";
            }


            if (String.IsNullOrEmpty(txtEndDate.Text))
            {
                CloseDateToErrorLabel.Visible = true;
                CloseDateToErrorLabel.Text = Resources.GlobalResources.STRING_CLOSE_DATE_TO_MESSAGE;
                return;
            }
            else
            {
                CloseDateToErrorLabel.Visible = false;
                CloseDateToErrorLabel.Text = "";
            }

            DateTime startDate, endDate;
            Double different;
            DateTime.TryParse(txtStartDate.Text, out startDate);
            DateTime.TryParse(txtEndDate.Text, out endDate);
            different = ((endDate.AddDays(1)) - startDate).TotalDays;
            if (endDate < startDate)
            {
                CloseDateToErrorLabel.Visible = true;
                CloseDateToErrorLabel.Text = "End Date Should Not Early Than Start Date";
                return;
            }
            if (different > 31)
            {
                CloseDateToErrorLabel.Visible = true;
                CloseDateToErrorLabel.Text = Resources.GlobalResources.DATE_SELECTION_ERROR_MESSAGE;
                return;
            }
            CloseDateToErrorLabel.Text = "";
            CloseDateToErrorLabel.Visible = false;
            CloseDateFromErrorLabel.Visible = false;
            CloseDateFromErrorLabel.Text = "";
            JoNoErrorLabel.Visible = false;
            JoNoErrorLabel.Text = "";
        }
        try
        {
            dtBody = MESComment.DBUtility.RunProcedure
            ("[dbo].[CLOSE_JO_REPORTS_STORED_PROCEDURE]", new DbParameter[] 
            { 
                    new SqlParameter("INPUT_FACTORY_CD",ddlFactoryCode.SelectedItem.Text),
                    new SqlParameter("INPUT_GARMENT_TYPE",ddlGarmentType.SelectedItem.Value),
                    new SqlParameter("INPUT_PROCESS_CD",ddlProcessCode.SelectedItem.Value),
                    new SqlParameter("INPUT_PROCESS_TYPE",ddlProcessType.SelectedItem.Value),
                    new SqlParameter("INPUT_CLOSE_ORDER_STATUS",ddlCloseState.SelectedItem.Value),
                    new SqlParameter("CLOSE_DATE_FROM",txtStartDate.Text.Trim()),
                    new SqlParameter("CLOSE_DATE_TO",txtEndDate.Text.Trim()),
                    new SqlParameter("JOB_ORDER_NO",txtJoNo.Text.Trim()),
                    new SqlParameter("INPUT_WASH_TYPE", ddlWashType.SelectedValue)
                    
            }
            , 600);
            errorLabel.Text = "";


            this.gvBody.DataSource = dtBody;
            this.gvBody.DataBind();
        }
        catch (Exception ex)
        {
            errorLabel.Text = "Please Filter The Results In More Detail";
        }
        try
        {
            if (dtBody == null)
            {
                return;
            }
            //End Update 18/8/2015
            if (dtBody.Rows.Count < 1)
            {
                errorLabel.Text = "No Record Found.";
                rpList.DataSource = null;
                rpList.DataBind();
                rpList.Visible = false;
                return;
            }
            else
            {
                errorLabel.Text = "";
            }
            if ("Y".Equals(ddlCloseState.SelectedItem.Value) && txtJoNo.Text.Trim().Length == 0)
            {
                var newTable = from t in dtBody.AsEnumerable()
                               group t by t.Field<string>("Process") into g
                               select new
                               {
                                   Type = g.Key,
                                   
                                   Less1 = g.Where(k => k.Field<int>("Interval Day") <= 1).Count(),
                                   Less3 = g.Where(k => k.Field<int>("Interval Day") <= 3 && k.Field<int>("Interval Day") > 1).Count(),
                                   Less5 = g.Where(k => k.Field<int>("Interval Day") <= 5 && k.Field<int>("Interval Day") > 3).Count(),
                                   Less7 = g.Where(k => k.Field<int>("Interval Day") <= 7 && k.Field<int>("Interval Day") > 5).Count(),
                                   Less10 = g.Where(k => k.Field<int>("Interval Day") <= 10 && k.Field<int>("Interval Day") > 7).Count(),
                                   Less15 = g.Where(k => k.Field<int>("Interval Day") <= 15 && k.Field<int>("Interval Day") > 10).Count(),
                                   Less20 = g.Where(k => k.Field<int>("Interval Day") <= 20 && k.Field<int>("Interval Day") > 15).Count(),
                                   More20 = g.Where(k => k.Field<int>("Interval Day") > 20).Count()
                               }
                               ;

                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[]{
                new DataColumn("Type",Type.GetType("System.String")),
                new DataColumn("Less1",Type.GetType("System.Int32")),
                new DataColumn("Less3",Type.GetType("System.Int32")),
                new DataColumn("Less5",Type.GetType("System.Int32")),
                new DataColumn("Less7",Type.GetType("System.Int32")),
                new DataColumn("Less10",Type.GetType("System.Int32")),
                new DataColumn("Less15",Type.GetType("System.Int32")),
                new DataColumn("Less20",Type.GetType("System.Int32")),
                new DataColumn("More20",Type.GetType("System.Int32")),
                new DataColumn("Total",Type.GetType("System.Int32")),
              
            });


                foreach (var item in newTable)
                {
                    DataRow dr = dt.NewRow();
                    dr["Type"] = item.Type;
                    dr["Less1"] = item.Less1;
                    dr["Less3"] = item.Less3;
                    dr["Less5"] = item.Less5;
                    dr["Less7"] = item.Less7;
                    dr["Less10"] = item.Less10;
                    dr["Less15"] = item.Less15;
                    dr["Less20"] = item.Less20;
                    dr["More20"] = item.More20;
                    dr["Total"] = item.Less1 + item.Less3 + item.Less5 + item.Less7 + item.Less10 + item.Less15 + item.Less20 + item.More20;
                    dt.Rows.Add(dr);
                }

                rpList.DataSource = dt;
                rpList.DataBind();
                rpList.Visible = true;
                errorLabel.Text = "";
            }
            else
            {
                rpList.DataSource = null;
                rpList.DataBind();
                rpList.Visible = false;
            }

        }
        catch (Exception ex)
        {
            errorLabel.Text = "Application Exception";
        }
        //已关单才有必要
        /*if ("Y".Equals(ddlCloseState.SelectedItem.Value))
        {
            var newTable = from t in dtBody.AsEnumerable()
                           group t by t.Field<string>("PROCESS_CD") into g
                           select new
                           {
                               Type = g.Key,
                               Less1 = g.Where(k => k.Field<int>("DAYS") <= 1).Count(),
                               Less3 = g.Where(k => k.Field<int>("DAYS") <= 3 && k.Field<int>("DAYS") > 1).Count(),
                               Less5 = g.Where(k => k.Field<int>("DAYS") <= 5 && k.Field<int>("DAYS") > 3).Count(),
                               Less7 = g.Where(k => k.Field<int>("DAYS") <= 7 && k.Field<int>("DAYS") > 5).Count(),
                               Less10 = g.Where(k => k.Field<int>("DAYS") <= 10 && k.Field<int>("DAYS") > 7).Count(),
                               Less15 = g.Where(k => k.Field<int>("DAYS") <= 15 && k.Field<int>("DAYS") > 10).Count(),
                               Less20 = g.Where(k => k.Field<int>("DAYS") <= 20 && k.Field<int>("DAYS") > 15).Count(),
                               More20 = g.Where(k => k.Field<int>("DAYS") > 20).Count()
                           };

            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]{
                new DataColumn("Type",Type.GetType("System.String")),
                new DataColumn("Less1",Type.GetType("System.Int32")),
                new DataColumn("Less3",Type.GetType("System.Int32")),
                new DataColumn("Less5",Type.GetType("System.Int32")),
                new DataColumn("Less7",Type.GetType("System.Int32")),
                new DataColumn("Less10",Type.GetType("System.Int32")),
                new DataColumn("Less15",Type.GetType("System.Int32")),
                new DataColumn("Less20",Type.GetType("System.Int32")),
                new DataColumn("More20",Type.GetType("System.Int32")),
                new DataColumn("Total",Type.GetType("System.Int32")),
              
            });

            foreach (var item in newTable)
            {
                DataRow dr = dt.NewRow();
                dr["Type"] = item.Type;
                dr["Less1"] = item.Less1;
                dr["Less3"] = item.Less3;
                dr["Less5"] = item.Less5;
                dr["Less7"] = item.Less7;
                dr["Less10"] = item.Less10;
                dr["Less15"] = item.Less15;
                dr["Less20"] = item.Less20;
                dr["More20"] = item.More20;
                dr["Total"] = item.Less1 + item.Less3 + item.Less5 + item.Less7 + item.Less10 + item.Less15 + item.Less20 + item.More20;
                dt.Rows.Add(dr);
            }

            rpList.DataSource = dt;
            rpList.DataBind();
        }
        if ("ZH-CN".Equals(CultureInfo.CurrentCulture.Name.ToUpper()))
        {
            foreach (DataColumn item in dtBody.Columns)
            {
                item.ColumnName = GetGlobalResources(item.ColumnName) == null ? item.ColumnName : GetGlobalResources(item.ColumnName);
            }
        }*/
        

        

    }


    public static string GetGlobalResources(string rKey)
    {
        ResourceManager rm = new ResourceManager("Resources.GlobalResources", Assembly.Load("App_GlobalResources"));
        return rm.GetString(rKey);
    }

    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //鼠标移动到任意行时，该行自动变成指定颜色
            e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#BEC9F6';this.style.color='buttontext';");
            e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='';this.style.color='';");
        }
    }

    protected void ddlCloseState_SelectedIndexChanged(object sender, EventArgs e)
    {
        rpList.Visible = false;
        errorLabel.Text = String.Empty;
        if (ddlCloseState.SelectedValue.Equals("Y"))
        {
            if (ddlProcessCode.Items.FindByText("ALL") == null)
            {
                ddlProcessCode.Items.Insert(0, new ListItem("ALL", ""));
                ddlProcessCode.SelectedIndex = 0;
            }
            txtStartDate.Visible = true;
            txtEndDate.Visible = true;
            LEndDate.Visible = true;
            LStartDate.Visible = true;
        }
        else
        {
            if (ddlProcessCode.Items.FindByText("ALL") != null)
                ddlProcessCode.Items.Remove(ddlProcessCode.Items.FindByText("ALL"));
            txtStartDate.Visible = false;
            txtEndDate.Visible = false;
            LEndDate.Visible = false;
            LStartDate.Visible = false;
            CloseDateFromErrorLabel.Visible = false;
            CloseDateToErrorLabel.Visible = false;
            CloseDateFromErrorLabel.Text = "";
            CloseDateToErrorLabel.Text = "";
            txtStartDate.Text = "";
            txtEndDate.Text = "";
        }
    }
    protected void ddlPageNumber_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    //protected void SetupDDLPageNumber()
    //{

    //    DataTable rowNumberDataTable;
    //    int totalRecords = 0, totalPages = 0, dividen = 0;
    //    rowNumberDataTable = MESComment.DBUtility.RunProcedure
    //        ("[dbo].[CLOSE_JO_REPORTS_STORED_PROCEDURE]", new DbParameter[] 
    //        { 


    //                new SqlParameter("INPUT_FACTORY_CD",ddlFactoryCode.SelectedItem.Text),
    //                new SqlParameter("INPUT_GARMENT_TYPE",ddlGarmentType.SelectedItem.Value),
    //                new SqlParameter("INPUT_PROCESS_CD",ddlProcessCode.SelectedItem.Value),
    //                new SqlParameter("INPUT_PROCESS_TYPE",ddlProcessType.SelectedItem.Value),
    //                new SqlParameter("INPUT_CLOSE_ORDER_STATUS",ddlCloseState.SelectedItem.Value),
    //                new SqlParameter("CLOSE_DATE_FROM",txtStartDate.Text.Trim()),
    //                new SqlParameter("CLOSE_DATE_TO",txtEndDate.Text.Trim()),
    //                new SqlParameter("JOB_ORDER_NO",txtJoNo.Text.Trim()),
    //                new SqlParameter("INPUT_WASH_TYPE", ddlWashType.SelectedValue),
    //                new SqlParameter("IS_COUNT", "Y"),
    //                new SqlParameter("TOP_N_RECORDS", "")
    //        }
    //        , 150);
    //    if (rowNumberDataTable.Rows.Count > 0)
    //    {
    //        if (rowNumberDataTable.Rows[0]["ROW_NO"].ToString() != null)
    //            int.TryParse(rowNumberDataTable.Rows[0]["ROW_NO"].ToString(), out totalRecords);

    //        if (totalRecords > 0)
    //        {
    //            totalPages = totalRecords / linePerPage;
    //            dividen = totalRecords % linePerPage;
    //            if (dividen > 0)
    //                totalPages += 1;
    //            for (int i = 0; i < totalPages; i++)
    //                ddlPageNumber.Items.Add((i + 1) + "00");
    //        }
    //    }

    //}
}