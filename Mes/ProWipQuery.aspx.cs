using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.Common;
using System.Threading;
using System.Globalization;

public partial class Mes_ProWipQuery : pPage
{
    string factoryCd = "";
    DataTable gvTitle = new DataTable();
    string strTableName = "";
    string strOutTableName = "";
    string strculture = "";
    string f = "";
    string Type = "Old";
    string svType = "PROD";
    protected void Page_Load(object sender, EventArgs e)
    {
        
        AjaxPro.Utility.RegisterTypeForAjax(typeof(Mes_ProWipQuery));
        if (Request.QueryString["svType"] != null)
        {
            svType = Request.QueryString["svType"].ToString();
            HttpContext.Current.Session["svType"] = Request.QueryString["svType"].ToString();
        }
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                factoryCd = Request.QueryString["site"].ToString();
            }
            else
            {
                factoryCd = "GEG";
            }
            if (factoryCd.Equals("YMG_TEST"))
            {
                if (this.Request.QueryString.Get("factory_cd") != null)
                {
                    factoryCd = Request.QueryString["factory_cd"].ToString();
                    if (factoryCd.Contains("YMG"))
                    {
                        factoryCd = "YMG";
                    }
                }
                if (this.Request.QueryString.Get("Type") != null)
                {
                    this.Type = this.Request.QueryString.Get("Type").ToString();
                }
            }
            hfValue.Value = factoryCd;

            if (factoryCd == "PTX" || factoryCd == "EGM" || factoryCd == "TIL" || factoryCd == "EGV" || factoryCd == "EAV")
            {
                strculture = "en";
            }
            else
            {
                strculture = "zh";
            }
        }
        string url = this.Request.Url.ToString();
        if (Request.QueryString["factory_cd"] != null)
        {

            f = Request.QueryString["factory_cd"].ToString();
        }

        //Added by Zikuan - MES-093
        if (ddlDept.Items.Count > 0)
        {
            if (string.Compare(ddlDept.SelectedItem.Text, "All", true) == 0)
            {
                Literal10.Visible = false;
                divCustomCheckBoxList.Visible = false;
            }
        }
        //End Add

        if (!IsPostBack)
        {
            ddlDept.DataTextField = "NM";
            ddlDept.DataValueField = "PRC_CD";
            ddlDept.DataSource = MESComment.ProTranWIPSummary.GetProTranWIPSummaryDept(factoryCd,ddlWipGarmentType.SelectedItem.Value);
            ddlDept.DataBind();

            //Added by Zikuan - MES-093
            if (ddlDept.Items.Count > 0)
            {
                if (string.Compare(ddlDept.SelectedItem.Text, "All", true) == 0)
                {
                    Literal10.Visible = false;
                    divCustomCheckBoxList.Visible = false;
                }
            }
            //End Add

            
            ddlWipProductFactory.DataTextField = "FACTORY_ID";
            ddlWipProductFactory.DataValueField = "FACTORY_ID";
            ddlWipProductFactory.DataSource = MESComment.MesRpt.GetProdFactoryCd();
            ddlWipProductFactory.DataBind();

            ddlWipProcessType.DataSource = MESComment.MesRpt.GetProcessType(factoryCd);
            ddlWipProcessType.DataTextField = "PROCESS_TYPE_ID";
            ddlWipProcessType.DataValueField = "PROCESS_TYPE_VALUE";
            ddlWipProcessType.DataBind();

            ddlWipProcessType.Items.FindByValue("I").Selected = true;

            //Added By Zikuan MES-093 4-Dec-13
            lstMultipleValues.Attributes.Add("onclick", "FindSelectedItems(this," + txtSelectedMLValues.ClientID + ");");
            lblClear.Attributes.Add("onclick", "ClearSelectedItems(" + txtSelectedMLValues.ClientID + ");");
            Literal10.Visible = false;
            divCustomCheckBoxList.Visible = false;
            //End Add

            if (strculture == "en")
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
             
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("zh-CN");
               
            }

        }
        txtWipJo.Text = txtWipJo.Text.ToUpper();
    }
    protected void btnWipQuery_Click(object sender, EventArgs e)
    {
        DbConnection MESConn = null;
        if (this.Request.QueryString.Get("site").ToString().Equals("YMG_TEST"))
        {
            MESConn = MESComment.DBUtility.GetConnection("MES");
        }
        else
        {
            MESConn = MESComment.DBUtility.GetConnection("MES_UPDATE");
        }
        Random ro = new Random(1000);
        strTableName = Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
        strOutTableName = "##" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
        string strOutSql = "";
        string strSQl="";
        DataTable dtJoList=MESComment.ProTranWIPSummary.GetProTranWIPSummaryJoList(factoryCd,MESConn);
        if (ckbIsOut.Checked)
        {
            foreach (DataRow row in dtJoList.Rows)
            {
                strSQl += " INSERT INTO MU_SHIP_JO_INFO (CT_NO,Run_NO,create_date) values('" + row[0] + "','" + strTableName + "',now()); ";
            }
            if (strSQl != "")
            {
                bool isCreate = true;                
                foreach (DataRow row in MESComment.ProTranWIPSummary.GetProTranWIPSummaryJoList(strSQl, strTableName, factoryCd, txtOutBeginDate.Text.Trim(), txtOutEndDate.Text.Trim()).Rows)
                {
                    if (isCreate)
                    {
                        strOutSql += " select '" + row[0] + "' as CT_NO into " + strOutTableName + "; ";
                        isCreate = false;
                    }
                    else
                    {
                        strOutSql += " insert into " + strOutTableName + " values('" + row[0] + "')";
                    }
                }
            }
        }

        //2013.03.05修改ＷＩＰ新的取数方法ZouShCh

        //DataTable dtSource;
        //if (this.Page.Request.QueryString.Get("site").ToString().ToUpper() == "GEG")
        //{


        //     if (txtWipJo.Text.Trim() == "")
        //     {
        //         dtSource = MESComment.ProTranWIPSummary.GetProTranWIPSummary(strTableName, MESConn);
        //     }
        //     else
        //     {
        //         dtSource = MESComment.ProTranWIPSummary.GenerateProTranWIPDetail(factoryCd, ddlDept.SelectedItem.Value, txtWipJo.Text.Trim(), true, ckbIsOut.Checked, rblDate.SelectedItem.Value, txtBeginDate.Text, txtEndDate.Text, ddlWipGarmentType.SelectedItem.Value, strTableName, strOutSql, strOutTableName, CBIsOutsource.Checked, MESConn);

        //     }
        //    gvTitle = MESComment.ProTranWIPSummary.GetProTranWIPTitle(strTableName, factoryCd, "", MESConn);
        //}
        //else
        //{
        //     if (txtWipJo.Text.Trim() == "")
        //     {
       //         dtSource = MESComment.ProTranWIPSummary.GenerateProTranNewWIPSummary(factoryCd, ddlDept.SelectedItem.Value, txtWipJo.Text.Trim(), true, ckbIsOut.Checked, rblDate.SelectedItem.Value, txtBeginDate.Text, txtEndDate.Text, ddlWipGarmentType.SelectedItem.Value, strTableName, strOutSql, strOutTableName, CBIsOutsource.Checked, MESConn);
        //     }
        //     else
        //     {
        //         dtSource = MESComment.ProTranWIPSummary.GenerateProTranNewWIPDetail(factoryCd, ddlDept.SelectedItem.Value, txtWipJo.Text.Trim(), true, ckbIsOut.Checked, rblDate.SelectedItem.Value, txtBeginDate.Text, txtEndDate.Text, ddlWipGarmentType.SelectedItem.Value, strTableName, strOutSql, strOutTableName, CBIsOutsource.Checked, MESConn);

        //     }
        //     gvTitle = MESComment.ProTranWIPSummary.GetProTranNewWIPTitle(txtWipJo.Text.Trim(), factoryCd, "", MESConn);
        //}
       
        
        //2013.03.05修改ＷＩＰ新的取数方法ZouShCh

        //MESComment.DBUtility.CloseConnection(ref MESConn);
            
        //zhanghao   
        //Added By ZouShiChang ON 2013.09.02 MES024 将旧WIP取数方式更改为新的
        DataTable dtSource = new DataTable();
        if (this.Type.Equals("Old"))
        {
            //dtSource = MESComment.ProTranWIPSummary.GenerateProTranWIPDetail(factoryCd, ddlDept.SelectedItem.Value, txtWipJo.Text.Trim(), true, ckbIsOut.Checked, rblDate.SelectedItem.Value, txtBeginDate.Text, txtEndDate.Text, ddlWipGarmentType.SelectedItem.Value, strTableName, strOutSql, strOutTableName, CBIsOutsource.Checked, MESConn);
            //Modify by Zikuan MES-093 Add Line parameter
            dtSource = MESComment.ProTranWIPSummary.GenerateProTranNewWIPDetail(factoryCd, ddlDept.SelectedItem.Value, txtWipJo.Text.Trim(), true, ckbIsOut.Checked, rblDate.SelectedItem.Value, txtBeginDate.Text, txtEndDate.Text, ddlWipGarmentType.SelectedItem.Value, ddlWipProcessType.SelectedItem.Value, ddlWipProductFactory.SelectedItem.Value, strTableName, strOutSql, strOutTableName, CBIsOutsource.Checked, MESConn, txtSelectedMLValues.Value.ToString());
        }
        else
        {
            //dtSource = MESComment.ProTranWIPSummary.GenerateProTranWIPDetail_DEV(f, ddlDept.SelectedItem.Value, txtWipJo.Text.Trim(), true, ckbIsOut.Checked, rblDate.SelectedItem.Value, txtBeginDate.Text, txtEndDate.Text, ddlWipGarmentType.SelectedItem.Value, strTableName, strOutSql, strOutTableName, CBIsOutsource.Checked, MESConn);
        }
        if (txtWipJo.Text.Trim() == "")
        {
            dtSource = MESComment.ProTranWIPSummary.GetProTranWIPSummary(strTableName, MESConn);
        }       
        gvTitle = MESComment.ProTranWIPSummary.GetProTranWIPTitle(strTableName, factoryCd, "", MESConn);
        string strJoList = "";
        //取出有效JO列表
        dtJoList = MESComment.ProTranWIPSummary.GetProTranWIPJoList(strTableName, MESConn);
        
        foreach (DataRow row in dtJoList.Rows)
        {
            strJoList += row[0].ToString()+";";
        }
        //DataTable dtShipQTYAndWarehouseQTY = MESComment.MesOutSourcePriceSql.GetShipQTYAndWarehouseQTY("","",strJoList,"","","","","",factoryCd,"","","","","","","",false,"");
        DataTable dtShipQTYAndWarehouseQTY = new DataTable();
        MESComment.DBUtility.CloseConnection(ref MESConn);

   
        DataTable dtObject = new DataTable();
        if (txtWipJo.Text.Trim() == "")
        {
            if (strculture == "en")
            {
                dtObject.Columns.Add("Process");
            }
            else
            {
                dtObject.Columns.Add("部门");
            }
            foreach (DataRow row in gvTitle.Rows)
            {
                dtObject.Columns.Add(row[1].ToString());
            }
            //新增warehouse;
            //dtObject.Columns.Add("WareHouse");

            if (dtSource.Rows.Count > 0)
            {
                DataRow newRow = dtObject.NewRow();
                if (strculture == "en")
                {
                    newRow["Process"] = "WIP";
                }
                else
                {
                    newRow["部门"] = "WIP";
                }
               
                foreach (DataRow row in dtSource.Rows)
                {
                    newRow[row["PROCESS_CD"].ToString()] = row["WIP"];
                }
                //新增warehouse;
                //int warehouseWIP = 0;
                //foreach (DataRow dr in dtShipQTYAndWarehouseQTY.Rows)
                //{
                //    warehouseWIP += Int32.Parse((dr["warehouse"].ToString().Equals("") ? "0" : dr["warehouse"].ToString()));
                //}
                //newRow["warehouse"] = warehouseWIP.ToString("#,###");
                dtObject.Rows.Add(newRow);
            }
        }
        else
        {
            string oldJobOrderNo = "";
            int index = -1;
            dtObject.Columns.Add("JOB_ORDER_NO");
            dtObject.Columns.Add("Delivery Date");//货期
            dtObject.Columns.Add("Cut Qty");//裁数
            if (gvTitle.Rows.Count > 0)
            {
                foreach (DataRow row in gvTitle.Rows)
                {
                    dtObject.Columns.Add(row[1].ToString());
                }
                foreach (DataRow row in dtSource.Rows)
                {
                    if (oldJobOrderNo != row["JOB_ORDER_NO"].ToString())
                    {
                        index++;
                        DataRow newRow = dtObject.NewRow();
                        newRow["JOB_ORDER_NO"] = row["JOB_ORDER_NO"];
                        newRow[1] = row["Buyer_Po_Del_date"];
                        newRow[2] = row["qty"];
                        newRow[row["PROCESS_CD"].ToString()] = row["WIP"];
                        oldJobOrderNo = row["JOB_ORDER_NO"].ToString();
                        dtObject.Rows.Add(newRow);
                    }
                    dtObject.Rows[index][row["PROCESS_CD"].ToString()] = row["WIP"];
                    oldJobOrderNo = row["JOB_ORDER_NO"].ToString();
                }
            }
            else
            {
                //新增Warehouse;
                //dtObject.Columns.Add("Warehouse");
                //foreach (DataRow row in dtShipQTYAndWarehouseQTY.Rows)
                //{
                //    if (oldJobOrderNo != row["JOB_ORDER_NO"].ToString())
                //    {
                //        index++;
                //        DataRow newRow = dtObject.NewRow();
                //        newRow["JOB_ORDER_NO"] = row["JOB_ORDER_NO"];
                //        newRow[1] = row["Buyer_Po_Del_date"];
                //        newRow[2] = row["qty"];
                //        newRow["Warehouse"] = row["WIP"];
                //        oldJobOrderNo = row["JOB_ORDER_NO"].ToString();
                //        dtObject.Rows.Add(newRow);
                //    }
                //    dtObject.Rows[index]["Warehouse"] = row["warehouse"];
                //    oldJobOrderNo = row["JOB_ORDER_NO"].ToString();
                //}
            }
        }
        if (dtObject.Rows.Count > 0)
        {
            this.NODATA.Visible = false;
            gvBody.DataSource = dtObject;
            gvBody.DataBind();
        }
        else
        {
            this.NODATA.Visible = true;
        }

    }
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        string id = "";
        switch (e.Row.RowType)
        {
            case DataControlRowType.DataRow:
                if (txtWipJo.Text == "")
                {
                    e.Row.Cells[0].Attributes.Add("align", "center");
                    e.Row.Cells[0].Attributes.Add("bgcolor", "#0082c6");
                    //id = factoryCd + "," + "WareHouse" + "," + strTableName;
                    //e.Row.Cells[gvTitle.Rows.Count + 1].Text = string.Format("<a href='#' onclick='window.open (\"ProWipSummary.aspx?id={0}\",\"\",\"height=300, width=600, top=150, left=250, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no\")'>{1}</a>", id, e.Row.Cells[gvTitle.Rows.Count + 1].Text);

                    //e.Row.Cells[gvTitle.Rows.Count].Text = string.Format("<a href='#' onclick='window.open (\"ProWipSummary.aspx?id={0}\",\"\",\"height=300, width=600, top=150, left=250, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no\")'>{1}</a>", id, e.Row.Cells[gvTitle.Rows.Count].Text);

                }
                for (int i = 0; i < gvTitle.Rows.Count; i++)
                {
                    if (txtWipJo.Text.Trim() == "")
                    {
                        //Testing MES-093
                        id = factoryCd + "," + gvTitle.Rows[i][1].ToString() + "," + strTableName;
                        //id = "DEV" + "," + gvTitle.Rows[i][1].ToString() + "," + strTableName;
                        //MES-093
                        if (ckbShowLineNameStyle.Checked)
                        {
                            e.Row.Cells[i + 1].Text = string.Format("<a href='#' onclick='window.open (\"ProWipSummary.aspx?id={0}&ShowBuyer=Y&svType=" + svType + "\",\"\",\"height=300, width=600, top=150, left=250, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no\")'>{1}</a>", id, e.Row.Cells[i + 1].Text);
                        }
                        else
                        {
                            e.Row.Cells[i + 1].Text = string.Format("<a href='#' onclick='window.open (\"ProWipSummary.aspx?id={0}&svType=" + svType + "\",\"\",\"height=300, width=600, top=150, left=250, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no\")'>{1}</a>", id, e.Row.Cells[i + 1].Text);
                        }
                        //end Ended 093
                    }
                    else
                    {
                        //id = factoryCd + "," + gvTitle.Rows[i][1].ToString()+","+e.Row.Cells[0].Text;
                        //Testing MES-093
                        id = "WIP" + "," + factoryCd + "," + gvTitle.Rows[i][1].ToString() + "," + e.Row.Cells[0].Text;
                        //id = "WIP" + "," + "DEV" + "," + gvTitle.Rows[i][1].ToString() + "," + e.Row.Cells[0].Text;
                        //id = "" + "," + factoryCd + "," + e.Row.Cells[0].Text + "," + gvTitle.Rows[i][1].ToString() + "," + ddlWipGarmentType.SelectedItem.Value + "," + "" + "," + txtBeginDate.Text + "," + txtEndDate.Text + "," + "Detail"+","+e.Row.Cells[i+3].Text ;
                        //MES-093
                        if (ckbShowLineNameStyle.Checked)
                        {
                            e.Row.Cells[i + 3].Text = "<a href='ProTranWIPDetail.aspx?id=" + id + "&svType=" + svType + "&line=" + txtSelectedMLValues.Value.ToString() + " 'target='blank'>" + e.Row.Cells[i + 3].Text + "</a>";
                        }
                        else
                        {
                            e.Row.Cells[i + 3].Text = "<a href='ProTranWIPDetail.aspx?id=" + id + "&svType=" + svType + " 'target='blank'>" + e.Row.Cells[i + 3].Text + "</a>";
                        }
                        //END MES-093
                    }
                }
                    break;
            case DataControlRowType.Header:
                    if (txtWipJo.Text.Trim() != "")
                    {
                        e.Row.Cells[0].Width = 100;
                        e.Row.Cells[1].Width = 100;
                        e.Row.Cells[2].Width = 50;
                    }
                    break;
        }
    }
    protected void ckbIsOut_CheckedChanged(object sender, EventArgs e)
    {
        if (ckbIsOut.Checked)
        {
            pnOutDate.Visible = true;
            txtOutEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtOutBeginDate.Text = DateTime.Now.AddMonths(-2).ToString("yyyy-MM-dd");
        }
        else
        {
            pnOutDate.Visible = false;
        }
    }
    protected void ddlWipGarmentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlDept.DataTextField = "NM";
        ddlDept.DataValueField = "PRC_CD";
        ddlDept.DataSource = MESComment.ProTranWIPSummary.GetProTranWIPSummaryDept(factoryCd,ddlWipGarmentType.SelectedItem.Value);
        ddlDept.DataBind();
    }

    //Added by Zikuan - MES-093
    protected void ckbShowLineNameStyle_CheckedChanged(object sender, EventArgs e)
    {
        if (ckbShowLineNameStyle.Checked)
        {
            if (string.Compare(ddlDept.SelectedItem.Text, "All", true) != 0)
            {
                Literal10.Visible = true;
                divCustomCheckBoxList.Visible = true;
                SetMultiValue();
                txtSelectedMLValues.Value = "All";
            }
            else
            {
                Literal10.Visible = false;
                divCustomCheckBoxList.Visible = false;
                txtSelectedMLValues.Value = string.Empty;
            }
        }
        else
        {
            Literal10.Visible = false;
            divCustomCheckBoxList.Visible = false;
            txtSelectedMLValues.Value = string.Empty;
        }
    }

    //Add by Zikuan - MES-093
    protected void SetMultiValue()
    {
        DataTable dt = new DataTable("Table1");
        DataColumn dc1 = new DataColumn("Text");
        DataColumn dc2 = new DataColumn("Value");
        dt.Columns.Add(dc1);
        dt.Columns.Add(dc2);

        //To get enough data for scroll
        DataTable CPLIST = MESComment.MesRpt.GetPrdLine(factoryCd, ddlDept.SelectedItem.Value, ddlWipGarmentType.SelectedItem.Value);
        dt.Rows.Add("All", "All");
        for (int i = 1; i < CPLIST.Rows.Count; i++)
        {
            dt.Rows.Add(CPLIST.Rows[i]["PRODUCTION_LINE_CD"].ToString(), i + 1);
        }

        lstMultipleValues.DataSource = dt;
        lstMultipleValues.DataTextField = "Text";
        lstMultipleValues.DataValueField = "Value";
        lstMultipleValues.DataBind();
    }

    //Added by Zikuan - MES-093
    protected void ddlDept_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlDept.SelectedIndex > 1)
        {
            if (ckbShowLineNameStyle.Checked)
            {
                Literal10.Visible = true;
                divCustomCheckBoxList.Visible = true;
                SetMultiValue();
                txtSelectedMLValues.Value = "All";
            }
            else
            {
                Literal10.Visible = false;
                divCustomCheckBoxList.Visible = false;
                txtSelectedMLValues.Value = string.Empty;
            }
        }
        else
        {
            Literal10.Visible = false;
            divCustomCheckBoxList.Visible = false;
            txtSelectedMLValues.Value = string.Empty;
        }
    }
}
