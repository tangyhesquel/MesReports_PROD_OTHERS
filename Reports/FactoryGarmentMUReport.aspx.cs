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
    string userid = "";
    public string strRunNO;
    public string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

    DataTable getalldatae = null;

    private void beginProgress()
    {
        //根据ProgressBar.htm显示进度条界面
        string templateFileName = Path.Combine(Server.MapPath("."), "ProgressBar.htm");
        StreamReader reader = new StreamReader(@templateFileName, System.Text.Encoding.GetEncoding("GB2312"));
        string html = reader.ReadToEnd();
        reader.Close();
        Response.Write(html);
        Response.Flush();
    }


    private void SetTiile(string Strt)
    {
        string jsBlock = "<script>SetTiile('" + Strt + "'); </script>";
        Response.Write(jsBlock);
        Response.Flush();
    }
    private void setProgress(int percent,string msg1,string msg2)
    {
        string jsBlock = "<script>SetPorgressBar('" + percent.ToString() + "','" + msg1 + "','" + msg2 + "'); </script>";
        Response.Write(jsBlock);
        Response.Flush();
    }

    private void finishProgress()
    {
        string jsBlock = "<script>SetCompleted();</script>";
        Response.Write(jsBlock);
        Response.Flush();
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["userid"] != null)
        {
            userid = Request.QueryString["userid"].ToString();
            string site = Request.QueryString["site"].ToString();
            if (site.Equals("DEV"))
            {
                ddlFtyCd.SelectedValue = "GEG";
            }
        }
        if (!IsPostBack)
        {
            ddlFtyCd.DataSource = MESComment.MesRpt.GetFactoryList(userid);
            ddlFtyCd.DataTextField = "DEPARTMENT_ID";
            ddlFtyCd.DataValueField = "DEPARTMENT_ID";
            ddlFtyCd.DataBind();
                    
        }
        if (Request.QueryString["site"] != null && Request.QueryString["site"] != "DEV")
        {
            ddlFtyCd.SelectedValue = Request.QueryString["site"].ToString();
        }
    }
    private DataTable GetMasterTable(string JoList)
    {
       // string strRunNO = "";
        Random ro = new Random(1000);
        strRunNO = Convert.ToString(string.Format("{0:yy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(10).ToString();
        string Strmsg = "|--Process MU Data begin(Total 15 Step)";
        beginProgress();
        SetTiile("Factory MU Report");
        Strmsg += "<br/>|--1. Process JO list begin...";
        setProgress(1, Strmsg, "Process JO list ...(1/15)");
        DataTable StandardJOList = MESComment.MesRpt.GetJoList(JoList, ddlFtyCd.SelectedItem.Value, ddlGarmentType.SelectedItem.Value, ddlOutSource.SelectedItem.Value, txtFromDate.Text, txtToDate.Text, cbShipJo.Checked, cbNoPost.Checked, strRunNO);
        if (StandardJOList.Rows.Count <= 0)
        {
            finishProgress();
            return StandardJOList;
            
        }
        JoList = "";
     
        DataTable jobtb = new DataTable();
        jobtb.Columns.Add("jobno");
        DataRow row_ = null;
        for (int i = 0; i < StandardJOList.Rows.Count; i++)
        {
            if (StandardJOList.Rows[i]["JO_NO"].ToString() != "")
            {
                JoList += "'" + StandardJOList.Rows[i]["JO_NO"].ToString() + "',";
                row_ = jobtb.NewRow();
                row_["jobno"] = StandardJOList.Rows[i]["JO_NO"].ToString();
                jobtb.Rows.Add(row_);
            }
        }
        Strmsg += "<br/>|--Process JO list finish.<br/>|--2. Process Invoice Data Begin:<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--2.1 Put Jo List To GIS system begin." ;
        setProgress(2, Strmsg, "Put Jo List To GIS system ...(2/15)");
        DbConnection GisConn = MESComment.DBUtility.GetConnection("GIS");//OK
        MESComment.DBUtility.InsertGISRptTempData(jobtb,strRunNO, GisConn);        //OK
        MESComment.DBUtility.CloseConnection(ref GisConn);//OK
        Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Put Jo List To GIS system end.<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--2.2 Get Invoice Data Begin..";
        setProgress(10, Strmsg, "Get Invoice Data ...(3/15)");
        DataTable StandardGisInfo = MESComment.MesRpt.GetLocalGisInfo(JoList, ddlFtyCd.SelectedItem.Value, ddlGarmentType.SelectedItem.Value, ddlOutSource.SelectedItem.Value, txtFromDate.Text, txtToDate.Text, cbShipJo.Checked, cbNoPost.Checked, strRunNO);
        Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Get Invoice Data End.<br/>|--&nbsp;&nbsp;&nbsp;&nbsp;Process Invoice Data End.<br/>|--3. Get GO/PPO Mapping data Begin:";
        setProgress(20, Strmsg, "Get GO/PPO Mapping data ...(4/15)");
        JoList = JoList.Substring(0, JoList.Length - 1);
        ///conn = Get Connection("eel")
        ///insert temp table
        ///search data
        /////////////////////////// EEL ////////////////////////////////////////
        DbConnection eelConn = MESComment.DBUtility.GetConnection("EEL");
        MESComment.DBUtility.InsertRptTempData(jobtb, eelConn);
        DataTable StandardWidthNpatternList = MESComment.MesRpt.GetStandardWidthNpatternList(eelConn);
        Strmsg += "<br/>|--&nbsp;&nbsp;&nbsp;&nbsp;Get GO/PPO Mapping data End|<br/>--4. Get Fabric Issue/Leftover Garment Data Begin...<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--4.1 Push Jo List to Fab Inv System Begin:";
        setProgress(25, Strmsg, "Push Jo List to Fab Inv System ...(5/15)");
       
        /////////////////////////// INV ////////////////////////////////////////
        
        DbConnection invConn = MESComment.DBUtility.GetConnection("INV");
        MESComment.DBUtility.InsertRptTempData(jobtb, invConn);
        Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Push Jo List to Fab Inv System End<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--4.2 Get Fabric Issue/Leftover Garment Data Begin:";
        setProgress(30, Strmsg, "Get Fabric Issue/Leftover Garment Data ...(6/15)");
        DataTable StandardSRNAndRTW = MESComment.MesRpt.GetStandardLeftGMAndSRNAndRTW(ddlFtyCd.SelectedItem.Value, invConn);
        Strmsg += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Get Fabric Issue/Leftover Garment Data End</br>&nbsp;&nbsp;&nbsp;&nbsp;|--4.3 Push PPO list to Fab Inv system Begin:";
        setProgress(40, Strmsg, "Push PPO list to Fab Inv system...(7/15)");
        MESComment.DBUtility.InsertRptPPOTempData(StandardWidthNpatternList, invConn);//OK
        Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Push PPO list to Fab Inv system End<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--4.4 Get Fabric LeftOver Data Begin:";
        setProgress(45, Strmsg, "Get Fabric LeftOver Data...(8/15)");
        DataTable LeftOver = MESComment.MesRpt.GetLocalLeftOver(ddlFtyCd.SelectedItem.Value, invConn);
        Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Get Fabric LeftOver Data End<br/>|--&nbsp;&nbsp;&nbsp;&nbsp;Get Fabric Issue/Leftover Garment Data End";
        setProgress(50, Strmsg, "Push Data to MES...(9/15)");
        MESComment.DBUtility.CloseConnection(ref invConn);
      
        setProgress(50, Strmsg, "Push Data to MES Begin ...(10/15)");
        /////////////////////////// INV ////////////////////////////////////////

        Strmsg += "</br>|--5. Push Data to MES Begin:" ;
        setProgress(50, Strmsg, "Push Data to MES...(10/15)");
        DbConnection MESConn = MESComment.DBUtility.GetConnection("MES");//OK
        Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--5.1 Push Ship data(GIS) Begin..." ;
        setProgress(55, Strmsg, "Push Ship data(GIS)...(10/15)");
        MESComment.DBUtility.InsertMESRptShipData(StandardGisInfo, strRunNO, ddlFtyCd.SelectedItem.Value, MESConn);//OK
        Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Push Ship data(GIS) End<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--5.2 Push PPO/GO data Begin...";
        setProgress(65, Strmsg, "Push PPO/GO data ...(11/15)");
        MESComment.DBUtility.InsertMESRptWidthNpatternData(StandardWidthNpatternList, strRunNO, ddlFtyCd.SelectedItem.Value, MESConn);//OK
       
        Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Push PPO/GO data End<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--5.3 Push Fabric & Left Garment data Begin...";
        setProgress(70, Strmsg, "Push Fabric & Left Garment data ...(12/15)");
        MESComment.DBUtility.InsertMESRptFGISData(StandardSRNAndRTW, strRunNO, ddlFtyCd.SelectedItem.Value, MESConn);
        Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Push Fabric & Left Garment data  End<br/>&nbsp;&nbsp;&nbsp;&nbsp;|--5.4 Push Fabric LeftOver data Begin...";
        setProgress(80, Strmsg, " Push Fabric LeftOver data ...(13/15)");
        MESComment.DBUtility.InsertMESRptLeftOverData(LeftOver, strRunNO, ddlFtyCd.SelectedItem.Value, MESConn);

        /////////////////////////// EEL ////////////////////////////////////////
        DataTable LeftoverPPOList = MESComment.MesRpt.GetSCList(strRunNO);
        MESComment.DBUtility.InsertRptTempData(LeftoverPPOList, eelConn);

        Strmsg += "</br>&nbsp;&nbsp;&nbsp;&nbsp;|--&nbsp;&nbsp;&nbsp;&nbsp;Push Fabric LeftOver data  End<br/>|--6. Get GO PPO order QTY Begin:";
        setProgress(85, Strmsg, "Get GO PPO order QTY ...(14/15)");
        DataTable StandardOrderQty = MESComment.MesRpt.GetLocalOrderQty(eelConn);//ok
        MESComment.DBUtility.CloseConnection(ref eelConn);
        MESComment.DBUtility.InsertMESRptStandardOrderQtyData(StandardOrderQty, strRunNO, ddlFtyCd.SelectedItem.Value, MESConn);//OK
        Strmsg += "<br/>|--&nbsp;&nbsp;&nbsp;&nbsp;Get GO PPO order QTY End.";
        Strmsg += "<br/>|--&nbsp;&nbsp;&nbsp;&nbsp;Push Data to MES End<br/>|--7. Process all data in mes begin...";
        setProgress(90, Strmsg, "Process all data in mes  ...(15/15)");
        MESComment.DBUtility.CloseConnection(ref MESConn);


        DataTable StandardMUReportList = MESComment.MesRpt.GetLocalMUReportGarmentList(ddlFtyCd.SelectedItem.Value, strRunNO, txtFromDate.Text,txtToDate.Text, "New");
        
        return StandardMUReportList;

    }
    protected void gvDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                int leftoverC = -1;//LEFTOVER_C_1子字段的起始位置;
                int countC = 0;//LEFTOVER_C子字段数;
                string[] Column = new string[getalldatae.Columns.Count];
                string[] ColumnC = {"裁片问题车间下数","车间欠配套下数","车间疵品半成品下数","车间大疵","印花下数"};
                for (int i = 0; i < getalldatae.Columns.Count; i++)
                {
                    Column[i] = getalldatae.Columns[i].ColumnName.ToString();
                    if (Column[i] == "LEFTOVER_C_1")
                    {
                        leftoverC = i;
                    }
                    if (Column[i].Contains("LEFTOVER_C"))
                    {
                        countC++;
                    }
                }
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();
                //First Row
                for (int i = 0; i < leftoverC; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[i].Attributes.Add("rowspan", "2");
                    tcHeader[i].Text = Column[i];
                    tcHeader[i].Attributes.Add("bgcolor", "#F7F6F3"); 
                }

                tcHeader.Add(new TableHeaderCell());
                tcHeader[leftoverC].Attributes.Add("colspan", countC.ToString());
                tcHeader[leftoverC].Text ="Grade C";
                tcHeader[leftoverC].Attributes.Add("bgcolor", "#F7F6F3"); 

                int j = leftoverC + 1;
                for (int i = leftoverC + countC; i < getalldatae.Columns.Count; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[j].Attributes.Add("rowspan", "2");
                    tcHeader[j].Text = Column[ i ];
                    tcHeader[j].Attributes.Add("bgcolor", "#F7F6F3");
                    j++;
                }

                tcHeader[ j - 1 ].Text += "</th></tr><tr>";//换行;
                //Second Row
                for (int i = 0; i < countC; i++)
                {
                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[ j ].Text = ColumnC[i];
                    tcHeader[ j ].Attributes.Add("bgcolor", "#F7F6F3");
                    j++;
                }
                break;

            case DataControlRowType.DataRow:
                e.Row.Cells[5].Style.Add("word-break", "break-all");
                break;

            case DataControlRowType.Footer:
            break;
        }
        finishProgress();
    }   

    
    private bool CheckQueryCondition()
    {
        string Strjo = this.txtJoNo.Text.Trim();
        string strBeginDate = this.txtFromDate.Text.Trim();
        string strEndDate = this.txtToDate.Text.Trim();

        if (Strjo.Length == 0 && strBeginDate.Length == 0 && strEndDate.Length == 0)
        {
            this.lblMessage.Text = "Please choose condition to query!";
            this.lblMessage.Visible = true;
            this.gvDetail.Visible = false;
            return false;
        }

        if (Strjo.Length == 0)
        {
            if (strBeginDate.Length == 0 && strEndDate.Length != 0 || strBeginDate.Length != 0 && strEndDate.Length == 0)
            {
                this.lblMessage.Text = "Please select accurate  Date!";
                this.lblMessage.Visible = true;
                this.gvDetail.Visible = false;
                return false;
            }
        }
        if (strBeginDate.Length != 0 && strEndDate.Length != 0 && Convert.ToDateTime(strBeginDate) > Convert.ToDateTime(strEndDate))
        {
            this.lblMessage.Text = "Selected To Date must after From Date!";
            this.lblMessage.Visible = true;
            this.gvDetail.Visible = false;
            return false;
        }
      
        return true;
    }
   
 
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.lblMessage.Visible = false;
        
        if (true == this.CheckQueryCondition())
        {
            gvDetail.Visible = false;
            gvDetail.DataSource = null;
            gvDetail.DataBind();            
            string JoList = "";
            if (this.txtFromDate.Text.Trim().Length != 0 && this.txtToDate.Text.Trim().Length != 0)
            {
                DataTable CheckMudata = MESComment.MesRpt.GetExistsMuList(ddlFtyCd.SelectedItem.Value, txtFromDate.Text, txtToDate.Text, strRunNO);
                
                if (CheckMudata.Rows.Count == 1)
                {

                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "Select data from existing!";
                    strRunNO = CheckMudata.Rows[0]["RunNo"].ToString();
                    beginProgress();
                    SetTiile("Factory MU Report");
                    setProgress(1, "Process Mu data from MES system", "");                    
                    gvDetail.DataSource = MESComment.MesRpt.GetLocalMUReportGarmentList(ddlFtyCd.SelectedItem.Value, strRunNO, txtFromDate.Text, txtToDate.Text, "Old");

                    if (cbChecked.Checked)
                    {
                        gvDetail.Visible = true;

                    }
                    gvDetail.DataBind();
                }
                else
                {
                    if (txtJoNo.Text != "")
                    {
                        string[] value = txtJoNo.Text.Trim().Split(';');
                        for (int i = 0; i < value.Length; i++)
                        {
                            if (value[i] != "")
                            {
                                JoList += "'" + value[i] + "',";
                            }
                        }
                        JoList = JoList.Substring(0, JoList.Length - 1);
                    }

                    if (cbChecked.Checked)
                    {
                        gvDetail.Visible = true;
                    }
                    getalldatae = GetMasterTable(JoList);

                    if (getalldatae.Rows.Count <= 0)
                    {
                        this.lblMessage.Text = "Not Found any Data!";
                        this.lblMessage.Visible = true;
                        this.gvDetail.Visible = false;
                        finishProgress();
                    }
                    else
                    {
                        gvDetail.DataSource = getalldatae;
                        gvDetail.DataBind();
                    }
                }
            }
            else
            {
                if (txtJoNo.Text != "")
                {
                    string[] value = txtJoNo.Text.Trim().Split(';');
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (value[i] != "")
                        {
                            JoList += "'" + value[i] + "',";
                        }
                    }
                    JoList = JoList.Substring(0, JoList.Length - 1);
                }

                if (cbChecked.Checked)
                {
                    gvDetail.Visible = true;
                }

                getalldatae = GetMasterTable(JoList);


                if (getalldatae.Rows.Count <= 0)
                {
                    this.lblMessage.Text = "Not Found any Data!";
                    this.lblMessage.Visible = true;
                    this.gvDetail.Visible = false;
                    finishProgress();
                }
                else
                {
                    gvDetail.DataSource = getalldatae;
                    gvDetail.DataBind();
                }
            }
            
           
        }
    }
}
