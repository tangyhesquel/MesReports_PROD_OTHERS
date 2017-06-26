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
using System.Threading;
using System.Globalization;
using System.Data.Common;

public partial class Mes_ProStatusQuery : pPage
{
    string factoryCd = "";
    string strJo = "";
    public string id;
    public string SHORT_NAME;
    public string JOCC;
    public string shipallowed;
    public string BUYER_PO_DEL_DATE;
    public string MARKET_CD;
    public string SHIP_MODE_CD;
    public string CUST_STYLE_NO;
    public string EMBROIDERY;
    public string PRINTING;
    public string WASH_TYPE_CD;
    public string STYLE_CHN_DESC;
    public string ProductLine;
    string strculture = "";
    DataTable dtShipQTYAndWarehouseQTY;
    DataTable TransParentInfo;

    public string CutQty;

    protected void Page_Load(object sender, EventArgs e)
    {
        AjaxPro.Utility.RegisterTypeForAjax(typeof(Mes_ProStatusQuery));
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
        if (!IsPostBack)
        {
            ddlProductCat.DataTextField = "PRODUCT_CATEGORY";
            ddlProductCat.DataValueField = "PRODUCT_CATEGORY";
            ddlProductCat.DataSource = MESComment.ProTranWIPSummary.GetProTranWIPSummaryProductCat(factoryCd);
            ddlProductCat.DataBind();

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
        txtGo.Text = txtGo.Text.ToUpper();
        txtJo.Text = txtJo.Text.ToUpper();
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        //只按GO查询
       if (txtGo.Text != "" && ddlGarmentType.SelectedItem.Value == "" && txtJo.Text == "" && txtCustomerCd.Text == "" && txtBeginDate.Text == "" && txtEndDate.Text == "" && txtStyleNo.Text == "" && ddlProductCat.SelectedItem.Value == "" && ddlType.SelectedItem.Value == "")
        {
            if (chkShippedQty.Checked)
                dtShipQTYAndWarehouseQTY = MESComment.MesOutSourcePriceSql.GetShipQTYAndWarehouseQTY("", "", "", "", "", txtGo.Text, "", "", factoryCd,"","","","","","", "SummaryByJo", CBIsOutsource.Checked, "Accept");
            else
                dtShipQTYAndWarehouseQTY = new DataTable();
         
            ////生成JoList临时表名
            //string JoListTableName = "";
            //Random ro = new Random(1000);
            //JoListTableName = "##JoList" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
            ////创建服务器连接

            //DbConnection MESConn = MESComment.DBUtility.GetConnection("MES_UPDATE");
            ////对JOList临时表赋值

            //MESComment.ProTranWIPSummary.GetProTranWIPSummaryJoList(JoListTableName, MESConn, factoryCd, ddlGarmentType.SelectedItem.Value, txtJo.Text.Trim(), chbJo.Checked, txtGo.Text.Trim(), chbGo.Checked, txtCustomerCd.Text.Trim(), chbCustomer.Checked, rblDate.SelectedItem.Value, txtBeginDate.Text.Trim(), txtEndDate.Text.Trim(), txtStyleNo.Text.Trim(), chbStyle.Checked, ddlProductCat.SelectedItem.Value, ddlType.SelectedItem.Value);


            DataTable dtBody = MESComment.ProTranWIPSummary.GetProTranWIPSummaryTransSummary(factoryCd, txtGo.Text.Trim());
            if (dtBody.Rows[0][0].ToString() != "Not Found Any Data")
            {
                if (dtShipQTYAndWarehouseQTY.Rows.Count > 0)
                {
                    dtBody.Columns.Add("ShipQty(For Reference)");
                    foreach (DataRow dr in dtBody.Rows)
                    {
                        int Ship_QTY = 0;
                        string objDt_JONO = dr["JO"].ToString().ToUpper();
                        string SQL = "JOB_ORDER_NO = '" + objDt_JONO + "'";
                        DataRow[] drsFromdtShipQTYAndWarehouseQTY = dtShipQTYAndWarehouseQTY.Select(SQL);
                        if (drsFromdtShipQTYAndWarehouseQTY.Length > 0)
                        {
                            Ship_QTY = Int32.Parse(drsFromdtShipQTYAndWarehouseQTY[0]["ship_qty"].ToString());
                        }
                        dr["ShipQTY(For Reference)"] = Ship_QTY.Equals(0) ? "" : Ship_QTY.ToString();
                    }
                }

                total = new int[dtBody.Columns.Count - 1];
                gvBody.Visible = true;
                ExcTable.Visible = false;
                gvBody.DataSource = dtBody;
                gvBody.DataBind();
                gvExcel.DataSource = dtBody;
                gvExcel.DataBind();
            }
            else
            {
                gvBody.Visible = false;
            }
        }
        else//按其它条件查询
        {
            if (txtGo.Text == "" && ddlGarmentType.SelectedItem.Value == "" && txtJo.Text == "" && txtCustomerCd.Text == "" && txtBeginDate.Text == "" && txtEndDate.Text == "" && txtStyleNo.Text == "" && ddlProductCat.SelectedItem.Value == "" && ddlType.SelectedItem.Value == "")
            {
                //DataBindJoList(dlJo, MESComment.ProTranWIPSummary.GetProTranWIPSummaryJoList(factoryCd, ddlGarmentType.SelectedItem.Value, txtJo.Text.Trim(), chbJo.Checked, txtGo.Text.Trim(), chbGo.Checked, txtCustomerCd.Text.Trim(), chbCustomer.Checked, rblDate.SelectedItem.Value, txtBeginDate.Text.Trim(), txtEndDate.Text.Trim(), txtStyleNo.Text.Trim(), chbStyle.Checked, ddlProductCat.SelectedItem.Value, "0"));
                divNoCondition.Visible = true;
            }
            else
            {
                DataBindJoList(dlJo, MESComment.ProTranWIPSummary.GetProTranWIPSummaryJoList(factoryCd, ddlGarmentType.SelectedItem.Value, txtJo.Text.Trim(), chbJo.Checked, txtGo.Text.Trim(), chbGo.Checked, txtCustomerCd.Text.Trim(), chbCustomer.Checked, rblDate.SelectedItem.Value, txtBeginDate.Text.Trim(), txtEndDate.Text.Trim(), txtStyleNo.Text.Trim(), chbStyle.Checked, ddlProductCat.SelectedItem.Value, ddlType.SelectedItem.Value));
                divNoCondition.Visible = false;
            }
            divTransInfo.InnerHtml = "";
            ExcTable.Visible = true;
            gvBody.Visible = false;
        }
    }
    private DataTable GenerateDetail(string strJo)
    {
        DataTable objDt = MESComment.ProTranWIPSummary.GetProTranWIPSummaryTransInfo(factoryCd, strJo, "%", "%");
        objDt.Columns.Remove("ORDER_QTY");
        if (chkShippedQty.Checked)
            objDt.Columns.Add("ShipQTY(For Reference)");


        if (dtShipQTYAndWarehouseQTY.Rows.Count > 0)
        {
            foreach (DataRow dr in objDt.Rows)
            {
                int Ship_QTY = 0;
                string objDt_JONO = dr["JO"].ToString().ToUpper();
                string objDt_Size = dr["SIZE"].ToString().ToUpper();
                string objDt_Col = dr["COLOR"].ToString().ToUpper();
                string SQL = "JOB_ORDER_NO = '" + objDt_JONO + "' AND COLOR_CD = '" + objDt_Col + "' AND SIZE_CD = '" + objDt_Size + "' ";
                if (objDt_Size == "")
                {
                    SQL = "JOB_ORDER_NO = '" + objDt_JONO + "' AND COLOR_CD = '" + objDt_Col + "' AND SIZE_CD is null ";
                }
                DataRow[] drsFromdtShipQTYAndWarehouseQTY = dtShipQTYAndWarehouseQTY.Select(SQL);
                if (drsFromdtShipQTYAndWarehouseQTY.Length > 0)
                {
                    for (int i = 0; i < drsFromdtShipQTYAndWarehouseQTY.Length; i++)
                    {
                        Ship_QTY += Int32.Parse(drsFromdtShipQTYAndWarehouseQTY[i]["ship_qty"].ToString());
                    }
                }
                dr["ShipQTY(For Reference)"] = Ship_QTY.Equals(0) ? "" : Ship_QTY.ToString("");
            }
        }
        return objDt;
    }
    private DataTable GenerateSummary(string strJo)
    {
        DataTable objDt = MESComment.ProTranWIPSummary.GetProTranWIPSummaryByColor(factoryCd, strJo, "%");
        CutQty = objDt.Compute("SUM(CUTQTY)","true").ToString();        
        if (chkShippedQty.Checked)
            objDt.Columns.Add("ShipQTY(For Reference)");
        if (dtShipQTYAndWarehouseQTY.Rows.Count > 0)
        {
            foreach (DataRow dr in objDt.Rows)
            {
                int Ship_QTY = 0;
                string objDt_JONO = dr["JO"].ToString().ToUpper();
                string objDt_Col = dr["COLOR"].ToString().ToUpper();
                string SQL = "JOB_ORDER_NO = '" + objDt_JONO + "' AND COLOR_CD = '" + objDt_Col + "' ";
                DataRow[] drsFromdtShipQTYAndWarehouseQTY = dtShipQTYAndWarehouseQTY.Select(SQL);
                if (drsFromdtShipQTYAndWarehouseQTY.Length > 0)
                {
                    for (int i = 0; i < drsFromdtShipQTYAndWarehouseQTY.Length; i++)
                    {
                        Ship_QTY += Int32.Parse(drsFromdtShipQTYAndWarehouseQTY[i]["ship_qty"].ToString());
                    }
                }
                dr["ShipQTY(For Reference)"] = Ship_QTY.Equals(0) ? "" : Ship_QTY.ToString("");
            }
        }
        return objDt;
    }
    private DataTable GenerateSummary(DataTable detail)
    {
        //List<string> group = new List<string>();
        //DataTable dt = detail.Clone();
        //foreach (DataRow dr in detail.Rows)
        //{
        //    if (group.Contains(dr["JO"].ToString() + dr["COLOR"].ToString() + dr["SIZE"].ToString()))
        //    {
        //    }
        //    else
        //    {

        //    }
        //}
        //return dt;
         var groupby = from t in detail.AsEnumerable()
                      group t by new { t1 = t.Field<string>("JO"), t2 = t.Field<string>("COLOR") } into m
                      select new
                      {

                          JO = m.Key.t1,
                          COLOR = m.Key.t2,
                         
                      };
         DataTable dt = detail.Clone();
        foreach(var dr in groupby.ToList())
        {
            DataRow newdr = dt.NewRow();
            newdr["JO"] = dr.JO;
            newdr["COLOR"] = dr.COLOR;
            

            foreach (DataRow detaildr in detail.Rows)
            {
                if (dr.JO == detaildr["JO"].ToString() && dr.COLOR == detaildr["COLOR"].ToString())
                {
                    for (int i = 4; i < detail.Columns.Count; i++)
                    {
                        newdr[i] = newdr[i].ToInt("d") + detaildr[i].ToInt("d");
                    }
                }
            }

            dt.Rows.Add(newdr);

        }
        return dt;
      
       
    }
    private void GenerateTransInfoView(string strJo)
    {
        divTransInfo.InnerHtml = "";
        if (chkShippedQty.Checked)
            dtShipQTYAndWarehouseQTY = MESComment.MesOutSourcePriceSql.GetShipQTYAndWarehouseQTY("", "", strJo, "", "", "", "", "", factoryCd,"","","","","","", "", chkShippedQty.Checked, "");
        else
            dtShipQTYAndWarehouseQTY = new DataTable();
        TransParentInfo = GenerateDetail(strJo);
        //DataTable TransParentSummary = GenerateSummary(strJo);
        DataTable TransParentSummary = GenerateSummary(TransParentInfo);
        int[] totalValue = new int[TransParentInfo.Columns.Count - 4];
        string strColor = "-1";
        if (TransParentSummary.Rows.Count > 0)
        {
           
            //生成transInfo Title
            if (TransParentInfo.Rows.Count > 0)
            {
                divTransInfo.InnerHtml = "<table width='100%' border='1' cellpadding='0' cellspacing='0' style='border-collapse: collapse'><tr bgcolor='#8FBBD6'><td width='1%'></td>";
                foreach (DataColumn column in TransParentInfo.Columns)
                {
                    if (column.ColumnName != "COLOR_DESC")
                    {
                        column.ColumnName = column.ColumnName == "TOTALCUTQTY" ? "ORDERQTY" : column.ColumnName;
                        divTransInfo.InnerHtml += "<td style='word-break:break-all' width='" + 99 / (TransParentInfo.Columns.Count) + "%'>" + column.ColumnName + "</td>";
                        column.ColumnName = column.ColumnName == "ORDERQTY" ? "TOTALCUTQTY" : column.ColumnName;
                    }
                }
                divTransInfo.InnerHtml += "</tr>";
            }
            

            //生成数据主体
            foreach (DataRow row in TransParentInfo.Rows)
            {
                //运算Total数据
                for (int i = 0; i < totalValue.Length; i++)
                {
                    totalValue[i] += int.Parse(row[i + 4].ToString() == "" ? "0" : row[i + 4].ToString());
                }


                if (strColor != row["COLOR"].ToString())
                {
                    DataRow SummaryRow = null;
                    foreach (DataRow nRow in TransParentSummary.Rows)
                    {
                        if (row["COLOR"].ToString().ToUpper() == nRow["COLOR"].ToString().ToUpper())//Added By ZouShiChang ON 2013.08.16 增加ToUpper()
                        {
                            SummaryRow = nRow;
                            break;
                        }
                    }
                    if (strColor != "-1")
                    {
                        divTransInfo.InnerHtml += "</table></td></tr></table></td></tr>";
                    }
                    divTransInfo.InnerHtml += "<tr><td colspan=" + TransParentInfo.Columns.Count + "><table width='100%'><tr><td width='1%'><div style='CURSOR: hand' onclick='ShowDetail(this)'>+</div></td>";
                    foreach (DataColumn column in TransParentInfo.Columns)
                    {
                        if (column.ColumnName != "SIZE")
                        {
                            string id = factoryCd + ">" + strJo + ">" + row["COLOR"] + ">" + "" + ">" + column.ColumnName;
                            switch (column.ColumnName)
                            {
                                case "JO":
                                case "COLOR":
                                case "CUTQTY":
                                case "TOTALCUTQTY":
                                case "ShipQTY(For Reference)":
                                    if (SummaryRow[column.ColumnName] != null)
                                    {
                                        divTransInfo.InnerHtml += "<td style='word-break:break-all' width='" + 99 / (TransParentInfo.Columns.Count) + "%'>" + SummaryRow[column.ColumnName] + "</td>";
                                    }
                                    else
                                    {
                                        divTransInfo.InnerHtml += "<td style='word-break:break-all' width='" + 99 / (TransParentInfo.Columns.Count) + "%'>" + "</td>";
                                    }
                                    break;
                                case "COLOR_DESC":
                                    break;
                                default:
                                    if (TransParentSummary.Columns.Contains(column.ColumnName))
                                    {
                                        divTransInfo.InnerHtml += "<td id='" + id + "' style='word-break:break-all' width='" + 99 / (TransParentInfo.Columns.Count) + "%'><a href=\"#\" onclick=\"javascript:window.open('showProcessDetail.aspx?id=" + id.Replace("#","%23") + "','','height=200, width=400, top=200, left=450, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no','')\">" + SummaryRow[column.ColumnName] + "</a></td>";
                                    }

                                    else
                                    {
                                        divTransInfo.InnerHtml += "<td id='" + id + "' style='word-break:break-all' width='" + 99 / (TransParentInfo.Columns.Count) + "%'>0</td>";
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            divTransInfo.InnerHtml += "<td style='word-break:break-all' width='" + 99 / (TransParentInfo.Columns.Count) + "%'></td>";
                        }
                    }
                    divTransInfo.InnerHtml += "</tr><tr style='display:none'><td colspan=" + TransParentInfo.Columns.Count + "><hr></td></tr>";
                    divTransInfo.InnerHtml += "<tr style='display:none'><td colspan=" + TransParentInfo.Columns.Count + "><table width='100%'>";
                    divTransInfo.InnerHtml += "<tr><td width='1%'></td>";
                    foreach (DataColumn column in TransParentInfo.Columns)
                    {
                        string id = factoryCd + ">" + strJo + ">" + row["COLOR"] + ">" + row["SIZE"] + ">" + column.ColumnName;
                        switch (column.ColumnName)
                        {
                            case "JO":
                            case "COLOR":
                            case "SIZE":
                            case "CUTQTY":
                            case "TOTALCUTQTY":
                            case "ShipQTY(For Reference)":
                                divTransInfo.InnerHtml += "<td style='word-break:break-all' width='" + 99 / (TransParentInfo.Columns.Count) + "%'>" + row[column.ColumnName] + "</td>";
                                break;
                            case "COLOR_DESC":
                                break;
                            default:
                                divTransInfo.InnerHtml += "<td style='word-break:break-all' width='" + 99 / (TransParentInfo.Columns.Count) + "%'><a href=\"#\" onclick=\"javascript:window.open('showProcessDetail.aspx?id=" + id.Replace("#", "%23") + "','','height=200, width=400, top=200, left=450, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no','')\">" + row[column.ColumnName] + "</a></td>";
                                break;
                        }
                    }
                    divTransInfo.InnerHtml += "</tr>";
                }
                else
                {
                    divTransInfo.InnerHtml += "<tr><td width='1%'></td>";
                    foreach (DataColumn column in TransParentInfo.Columns)
                    {
                        string id = factoryCd + ">" + strJo + ">" + row["COLOR"] + ">" + row["SIZE"] + ">" + column.ColumnName;
                        switch (column.ColumnName)
                        {
                            case "JO":
                            case "COLOR":
                            case "SIZE":
                            case "CUTQTY":
                            case "TOTALCUTQTY":
                            case "ShipQTY(For Reference)":
                                divTransInfo.InnerHtml += "<td style='word-break:break-all' width='" + 99 / (TransParentInfo.Columns.Count) + "%'>" + row[column.ColumnName] + "</td>";
                                break;
                            case "COLOR_DESC":
                                break;
                            default:
                                divTransInfo.InnerHtml += "<td style='word-break:break-all' width='" + 99 / (TransParentInfo.Columns.Count) + "%'><a href=\"#\" onclick=\"javascript:window.open('showProcessDetail.aspx?id=" + id.Replace("#", "%23") + "','','height=200, width=400, top=200, left=450, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no','')\">" + row[column.ColumnName] + "</a></td>";
                                break;
                        }
                    }
                    divTransInfo.InnerHtml += "</tr>";
                }
                strColor = row["COLOR"].ToString();
            }
            divTransInfo.InnerHtml += "</table></td></tr></table></td></tr>";
            divTransInfo.InnerHtml += "<tr><td colspan='4' align='left'><b>Total</b></td>";
            for (int i = 0; i < totalValue.Length; i++)
            {
               /*
                if (totalValue[1].ToString().Equals("0"))
                {
                    divTransInfo.InnerHtml += "<td><b>" + CutQty+ "</b></td>";
                }
                else
                {
                    divTransInfo.InnerHtml += "<td><b>" + totalValue[i] + "</b></td>";
                }
                */
                //修复Total数为0的问题。
                if (totalValue[i].ToString().Equals("0"))
                {
                    divTransInfo.InnerHtml += "<td><b>" + CutQty + "</b></td>";
                }
                else
                {
                    divTransInfo.InnerHtml += "<td><b>" + totalValue[i] + "</b></td>";
                }
            }
            divTransInfo.InnerHtml += "</tr>";
            divTransInfo.InnerHtml += "</table>";
        }
    }
    private void DataBindBasicList(string strJo)
    {
        DataTable dtSource = MESComment.ProTranWIPSummary.GetProTranWIPSummaryBasicInfo(strJo, strculture);
        if (dtSource.Rows.Count <= 0)
        {
            if (strculture == "en")
            {
                Response.Write("<script>alert('Can not found the JO baisc Info!')</script>");
            }
            else
            {
                Response.Write("<script>alert('制单基本信息不完整!')</script>");
            }
        }
        else
        {
            SHORT_NAME = dtSource.Rows[0]["SHORT_NAME"].ToString();
            JOCC = dtSource.Rows[0]["JOCC"].ToString();
            shipallowed = dtSource.Rows[0]["shipallowed"].ToString();
            BUYER_PO_DEL_DATE = dtSource.Rows[0]["BUYER_PO_DEL_DATE"].ToString();
            MARKET_CD = dtSource.Rows[0]["MARKET_CD"].ToString();
            SHIP_MODE_CD = dtSource.Rows[0]["SHIP_MODE_CD"].ToString();
            CUST_STYLE_NO = dtSource.Rows[0]["CUST_STYLE_NO"].ToString();
            EMBROIDERY = dtSource.Rows[0]["EMBROIDERY"].ToString();
            PRINTING = dtSource.Rows[0]["PRINTING"].ToString();
            WASH_TYPE_CD = dtSource.Rows[0]["WASH_TYPE_CD"].ToString();
            STYLE_CHN_DESC = dtSource.Rows[0]["STYLE_CHN_DESC"].ToString();
        }
    }
    private void DataBindJoList(DataList dlJo, DataTable dtSource)
    {
        divEmpty.Visible = false;
        if (dtSource.Rows.Count <= 0)
        {
            divEmpty.Visible = true;
        }
        else
        {
            dlJo.DataSource = dtSource;
            dlJo.DataBind();
            ExcTable.Visible = false;
        }
    }
    public override void VerifyRenderingInServerForm(Control control)
    {

    }
    int[] total;
    protected void dlJo_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataList dlObject = (DataList)sender;
        LinkButton lbObject = (LinkButton)dlObject.SelectedItem.FindControl("lbJo");
        GenerateTransInfoView(lbObject.Text);
        DataBindBasicList(lbObject.Text);
        strJo = lbObject.Text;
        //DataTable objDt = TransParentInfo;
        //objDt.Columns.Add("Ship QTY");
        total = new int[TransParentInfo.Columns.Count - 4];

        //dtShipQTYAndWarehouseQTY = MESComment.MesOutSourcePriceSql.GetShipQTYAndWarehouseQTY(factoryCd, txtJo.Text.Trim(), ddlGarmentType.SelectedValue.ToString(), rblDate.SelectedValue.ToString(), txtBeginDate.Text, txtEndDate.Text, "");
        //if (dtShipQTYAndWarehouseQTY.Rows.Count > 0)
        //{
        //    foreach (DataRow dr in objDt.Rows)
        //    {
        //        int Ship_QTY = 0;
        //        string objDt_JONO = dr["JOB_ORDER_NO"].ToString().ToUpper();
        //        string objDt_Size = dr["SIZE"].ToString().ToUpper();
        //        string objDt_Col = dr["COLOR"].ToString().ToUpper();
        //        string SQL = "JOB_ORDER_NO = '" + objDt_JONO + "' AND COLOR_CD = '" + objDt_Col + "' AND SIZE_CD = '" + objDt_Size + "' ";
        //        DataRow[] drsFromdtShipQTYAndWarehouseQTY = dtShipQTYAndWarehouseQTY.Select(SQL);
        //        if (drsFromdtShipQTYAndWarehouseQTY.Length > 0)
        //        {
        //            for (int i = 0; i < drsFromdtShipQTYAndWarehouseQTY.Length; i++)
        //            {
        //                Ship_QTY += Int32.Parse(drsFromdtShipQTYAndWarehouseQTY[i]["AB_OUT_QTY"].ToString());
        //            }
        //        }
        //        dr["Ship QTY"] = Ship_QTY.Equals(0) ? "" : Ship_QTY.ToString("#,###");
        //    }
        //object[] J = new object[objDt.Columns.Count];
        //for (int j = 4; j < objDt.Columns.Count - 5; j++)
        //{
        //    J[j] = "";
        //}
        //foreach (DataRow dr in dtShipQTYAndWarehouseQTY.Rows)
        //{//从Warehouse那里添加没有在MES出现的数据;
        //    string dtShipQTYAndWarehouseQTY_JONO = dr["JOB_ORDER_NO"].ToString().ToUpper();
        //    string dtShipQTYAndWarehouseQTY_Size = dr["SIZE_CD"].ToString().ToUpper();
        //    string dtShipQTYAndWarehouseQTY_Col = dr["COLOR_CD"].ToString().ToUpper();
        //    string SQL = "JOB_ORDER_NO = '" + dtShipQTYAndWarehouseQTY_JONO + "' AND COLOR = '" + dtShipQTYAndWarehouseQTY_Col + "' AND SIZE = '" + dtShipQTYAndWarehouseQTY_Size + "' ";
        //    DataRow[] drsobjDt = objDt.Select(SQL);
        //    if (drsobjDt.Length <= 0)
        //    {
        //        J[0] = dtShipQTYAndWarehouseQTY_JONO;
        //        J[1] = dtShipQTYAndWarehouseQTY_Col;
        //        J[2] = "";
        //        J[3] = dtShipQTYAndWarehouseQTY_Size;
        //        J[objDt.Columns.Count - 1] = dr["AB_OUT_QTY"].ToString();
        //        objDt.Rows.Add(J);
        //    }
        //}
        //}
        gvExcel.DataSource = TransParentInfo;
        gvExcel.DataBind();
        id = "Status" + "," + hfValue.Value + "," + strJo;
        foreach (DataRow row in MESComment.ProTranWIPSummary.GetProTranWIPProductQty(id.Split(',')[1], "", false, id.Split(',')[2], "", "", "","","", "", "", "Line", CBIsOutsource.Checked, true).Rows)
        {
            ProductLine = row["production_line_cd"].ToString();
        }
        //是否被Cancelled
        if (MESComment.ProTranWIPSummary.GetProTranWIPJOStatus(strJo))
        {
            divCancelled.Visible = true;
        }
        else
        {
            divCancelled.Visible = false;
        }
    }

    protected void gvExcel_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (!gvBody.Visible)
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.DataRow:
                    for (int i = 0; i < total.Length; i++)
                    {
                        total[i] += int.Parse(e.Row.Cells[i + 4].Text == "&nbsp;" ? "0" : e.Row.Cells[i + 4].Text);
                    }
                    break;
                case DataControlRowType.Footer:
                    e.Row.Cells[0].Text = "Total";
                    for (int i = 0; i < total.Length; i++)
                    {
                        e.Row.Cells[i + 4].Text = total[i].ToString();
                    }
                    break;
            }
        }
        else
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.Footer:
                    e.Row.Cells[0].Text = "Total";
                    for (int i = 0; i < total.Length; i++)
                    {
                        e.Row.Cells[i + 1].Text = total[i].ToString();
                    }
                    break;
            }
        }
    }
    protected void txtToExcel_Click(object sender, EventArgs e)
    {
        gvExcel.Visible = true;
        MESComment.Excel.ToExcel(gvExcel, "ProStatusQuety " + DateTime.Now.ToString("yyyyMMdd") + ".xls");
        gvExcel.Visible = false;
    }
    protected void gvBody_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        string strJo = gvBody.Rows[e.NewSelectedIndex].Cells[1].Text;
        gvBody.Visible = false;
        DataBindJoList(dlJo, MESComment.ProTranWIPSummary.GetProTranWIPSummaryJoList(factoryCd, ddlGarmentType.SelectedItem.Value, strJo, chbJo.Checked, txtGo.Text.Trim(), chbGo.Checked, txtCustomerCd.Text.Trim(), chbCustomer.Checked, rblDate.SelectedItem.Value, txtBeginDate.Text.Trim(), txtEndDate.Text.Trim(), txtStyleNo.Text.Trim(), chbStyle.Checked, ddlProductCat.SelectedItem.Value, ddlType.SelectedItem.Value));
        ExcTable.Visible = true;
        GenerateTransInfoView(strJo);
        DataBindBasicList(strJo);
        //DataTable objDt = MESComment.ProTranWIPSummary.GetProTranWIPSummaryTransInfo(factoryCd, strJo, "%", "%");
        total = new int[TransParentInfo.Columns.Count - 4];
        gvExcel.DataSource = TransParentInfo;
        gvExcel.DataBind();
        id = "Status" + "," + hfValue.Value + "," + strJo;
        foreach (DataRow row in MESComment.ProTranWIPSummary.GetProTranWIPProductQty(id.Split(',')[1], "", false, id.Split(',')[2], "", "","","", "", "", "", "Line", CBIsOutsource.Checked,true).Rows)
        {
            ProductLine = row["production_line_cd"].ToString();
        }
        //是否已经开裁，并且被Cancelled
        if (MESComment.ProTranWIPSummary.GetProTranWIPJOStatus(strJo))
        {
            divCancelled.Visible = true;
        }
        else
        {
            divCancelled.Visible = false;
        }
    }
    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                if (strculture == "en")
                {
                    e.Row.Cells[0].Text = "View";
                }
                else
                {
                    e.Row.Cells[0].Text = "查看";
                }
                break;
            case DataControlRowType.DataRow:
                for (int i = 0; i < total.Length; i++)
                {
                    total[i] += int.Parse(e.Row.Cells[i + 2].Text == "&nbsp;" ? "0" : e.Row.Cells[i + 2].Text);
                }
                break;
            case DataControlRowType.Footer:
                e.Row.Cells[0].Text = "Total";
                for (int i = 0; i < total.Length; i++)
                {
                    e.Row.Cells[i + 2].Text = total[i].ToString();
                }
                break;
        }
    }
}
