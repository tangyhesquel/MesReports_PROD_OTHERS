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

public partial class Mes_ProYMGStatusQuery : pPage
{
    string factoryCd = "";
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
    public ArrayList strHtmlList = new ArrayList();
    string strculture = "";
    int[] total;
    DataTable TransJoListInfo = new DataTable();
    DataTable dtShipQTYAndWarehouseQTY = new DataTable();

    protected void Page_Load(object sender, EventArgs e)
    {
        AjaxPro.Utility.RegisterTypeForAjax(typeof(Mes_ProYMGStatusQuery));
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
        //string strJoList = "";
        //foreach (DataRow row in MESComment.ProTranWIPSummary.GetProTranWIPSummaryJoList(factoryCd, ddlGarmentType.SelectedItem.Value, txtJo.Text.Trim(), chbJo.Checked, txtGo.Text.Trim(), chbGo.Checked, txtCustomerCd.Text.Trim(), chbCustomer.Checked, rblDate.SelectedItem.Value, txtBeginDate.Text.Trim(), txtEndDate.Text.Trim(), txtStyleNo.Text.Trim(), chbStyle.Checked, ddlProductCat.SelectedItem.Value, ddlType.SelectedItem.Value).Rows)
        //{
        //    if (row["JO_NO"].ToString() != "")
        //    {
        //        strJoList += row["JO_NO"].ToString() + ";";
        //    }
        //}
        if (chkShippedQty.Checked)
            if (rblDate.SelectedItem.Value == "BPO")
            {
                dtShipQTYAndWarehouseQTY = MESComment.MesOutSourcePriceSql.GetShipQTYAndWarehouseQTY("", "", "", txtBeginDate.Text, txtEndDate.Text, txtGo.Text.Trim(), ddlGarmentType.SelectedItem.Value, "", factoryCd, "", "", "", "", "", txtJo.Text.Trim(), "", chkShippedQty.Checked, "");
            }
            else
            {
                dtShipQTYAndWarehouseQTY = MESComment.MesOutSourcePriceSql.GetShipQTYAndWarehouseQTY(txtBeginDate.Text, txtEndDate.Text, "", "", "", txtGo.Text.Trim(), ddlGarmentType.SelectedItem.Value, "", factoryCd, "", "", "", "", "", txtJo.Text.Trim(), "", chkShippedQty.Checked, "");
            }
        else
            dtShipQTYAndWarehouseQTY = new DataTable();
        //生成JoList临时表名
        string JoListTableName = "";
        Random ro = new Random(1000);
        JoListTableName = "##JoList" + Convert.ToString(string.Format("{0:yyyy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(1000).ToString();
        //创建服务器连接

        DbConnection MESConn = MESComment.DBUtility.GetConnection("MES_UPDATE");
        //对JOList临时表赋值

        MESComment.ProTranWIPSummary.GetProTranWIPSummaryJoList(JoListTableName, MESConn, factoryCd, ddlGarmentType.SelectedItem.Value, txtJo.Text.Trim(), chbJo.Checked, txtGo.Text.Trim(), chbGo.Checked, txtCustomerCd.Text.Trim(), chbCustomer.Checked, rblDate.SelectedItem.Value, txtBeginDate.Text.Trim(), txtEndDate.Text.Trim(), txtStyleNo.Text.Trim(), chbStyle.Checked, ddlProductCat.SelectedItem.Value, ddlType.SelectedItem.Value);
        if (MESComment.ProTranWIPSummary.isChecked(JoListTableName, MESConn) || true)
        {
            GenerateTransInfoView(JoListTableName, MESConn);
        }
        else
        {
            Response.Write("<script>alert('请缩小查询范围，当前数据超出报表最大输出内容量！')</script>");
        }
    }
    public DataTable AddShipQtyToDataTable(DataTable objDt, string strJo, string strColor, string strSize,string IsChecked)
    {
        if (IsChecked=="True")
        {
            int diffQty = 0, finQty = 0;
            dtShipQTYAndWarehouseQTY = (DataTable)Session["dtShipQTYAndWarehouseQTY"];
            objDt.Columns.Add("ShipQty(For reference)");
            objDt.Columns.Add("Difference ShipQty");
            if (dtShipQTYAndWarehouseQTY.Rows.Count > 0)
            {
                foreach (DataRow dr in objDt.Rows)
                {
                    int Ship_QTY = 0;
                    finQty = 0;
                    string SQL = "";
                    if (strJo == "-" && strColor == "-" && strSize == "-")
                    {
                        string objDt_JONO = dr["JO"].ToString().ToUpper();
                        SQL = "JOB_ORDER_NO = '" + objDt_JONO + "'";
                    }
                    else if (strColor == "-" && strSize == "-")
                    {
                        string objDt_JONO = dr["JO"].ToString().ToUpper();
                        string objDt_Col = dr["COLOR"].ToString().ToUpper();
                        SQL = "JOB_ORDER_NO = '" + objDt_JONO + "' AND COLOR_CD = '" + objDt_Col + "' ";
                    }
                    else if (strSize == "-")
                    {
                        string objDt_JONO = dr["JO"].ToString().ToUpper();
                        string objDt_Size = dr["SIZE"].ToString().ToUpper();
                        string objDt_Col = dr["COLOR"].ToString().ToUpper();
                        SQL = "JOB_ORDER_NO = '" + objDt_JONO + "' AND COLOR_CD = '" + objDt_Col + "' AND SIZE_CD = '" + objDt_Size + "' ";
                        if (objDt_Size == "")
                        {
                            SQL = "JOB_ORDER_NO = '" + objDt_JONO + "' AND COLOR_CD = '" + objDt_Col + "' AND SIZE_CD is null ";
                        }
                    }
                    DataRow[] drsFromdtShipQTYAndWarehouseQTY = dtShipQTYAndWarehouseQTY.Select(SQL);
                    for (int j = 0; j < drsFromdtShipQTYAndWarehouseQTY.Length; j++)
                    {
                        Ship_QTY += Int32.Parse(drsFromdtShipQTYAndWarehouseQTY[j]["ship_qty"].ToString());
                    }
                    dr["ShipQTY(For reference)"] = Ship_QTY.Equals(0) ? "" : Ship_QTY.ToString();
                    foreach (DataColumn column in objDt.Columns)
                    {
                        if (column.ColumnName.ToString() == "WFIN(I)>WWHS(I)" || column.ColumnName.ToString() == "KFIN(I)>KWHS(I)")
                        {
                            if (dr[column.ColumnName].ToString() == "")
                            {
                                finQty += 0;
                            }
                            else
                            {
                                finQty += dr[column.ColumnName].ToInt("d");
                            }
                        }
                        else if (column.ColumnName == "Difference ShipQty")
                        {
                            dr[column.ColumnName] = finQty - Ship_QTY;
                        }
                    }
                }
            }
        }
        return objDt;
    }
    public DataTable AddFgisQtyToDataTable(DataTable objDt, string strJo, string strColor, string strSize)
    {
       
            int finQty = 0;
            dtShipQTYAndWarehouseQTY = (DataTable)Session["dtFgisQTYAndWarehouseQTY"];
            objDt.Columns.Add("FGIS INQTY");
            if (!objDt.Columns.Contains("Difference ShipQty"))
                objDt.Columns.Add("Difference ShipQty");
            if (dtShipQTYAndWarehouseQTY.Rows.Count > 0)
            {
                foreach (DataRow dr in objDt.Rows)
                {
                    int Fgis_QTY = 0;
                   
                    finQty = 0;
                    string SQL = "";
                    if (strJo == "-" && strColor == "-" && strSize == "-")
                    {
                        string objDt_JONO = dr["JO"].ToString().ToUpper();
                        SQL = "JOB_ORDER_NO = '" + objDt_JONO + "'";
                    }
                    else if (strColor == "-" && strSize == "-")
                    {
                        string objDt_JONO = dr["JO"].ToString().ToUpper();
                        string objDt_Col = dr["COLOR"].ToString().ToUpper();
                        SQL = "JOB_ORDER_NO = '" + objDt_JONO + "' AND COLOR_CD = '" + objDt_Col + "' ";
                    }
                    else if (strSize == "-")
                    {
                        string objDt_JONO = dr["JO"].ToString().ToUpper();
                        string objDt_Size = dr["SIZE"].ToString().ToUpper();
                        string objDt_Col = dr["COLOR"].ToString().ToUpper();
                        SQL = "JOB_ORDER_NO = '" + objDt_JONO + "' AND COLOR_CD = '" + objDt_Col + "' AND SIZE_CD = '" + objDt_Size + "' ";
                        if (objDt_Size == "")
                        {
                            SQL = "JOB_ORDER_NO = '" + objDt_JONO + "' AND COLOR_CD = '" + objDt_Col + "' AND SIZE_CD is null ";
                        }
                    }
                    DataRow[] drsFromdtShipQTYAndWarehouseQTY = dtShipQTYAndWarehouseQTY.Select(SQL);
                    for (int j = 0; j < drsFromdtShipQTYAndWarehouseQTY.Length; j++)
                    {
                        
                            Fgis_QTY += drsFromdtShipQTYAndWarehouseQTY[j]["qty"].ToInt("d");
                       
                       
                    }
                    dr["FGIS INQTY"] = Fgis_QTY;
                    foreach (DataColumn column in objDt.Columns)
                    {
                        if (column.ColumnName.ToString() == "WFIN(I)>WWHS(I)" || column.ColumnName.ToString() == "KFIN(I)>KWHS(I)" || column.ColumnName.ToString() == "KFIN(I)>Discrepancy" || column.ColumnName.ToString() == "WFIN(I)>Discrepancy")
                        {
                            if (dr[column.ColumnName].ToString() == "")
                            {
                                finQty += 0;
                            }
                            else
                            {
                                finQty += dr[column.ColumnName].ToInt("d");
                            }
                        }
                        else if (column.ColumnName == "Difference ShipQty")
                        {
                            dr[column.ColumnName] = finQty - Fgis_QTY;
                        }
                    }
                }
            }
        
        return objDt;
    }
    [AjaxPro.AjaxMethod]
    public string GetColorData(string factoryCd, string strJo,string IsChecked)
    {
        string strData = "";
        DataTable TransColorInfo = MESComment.ProTranWIPSummary.GetProTranWIPSummaryByColor(factoryCd, strJo, "%");
        TransColorInfo = AddShipQtyToDataTable(TransColorInfo, strJo, "-", "-",IsChecked);
        TransColorInfo = AddFgisQtyToDataTable(TransColorInfo, strJo, "-", "-"); 
        TransJoListInfo = (DataTable)Session["TransJoListInfo"];
        if (TransColorInfo.Rows.Count > 0)
        {
            strData += "<table><tr><td width='1%'></td><td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'></td><td colspan=" + (TransJoListInfo.Columns.Count + 1) + "><hr></td></tr>";
        }
        /*'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''Color Rows''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''*/
        foreach (DataRow colorRow in TransColorInfo.Rows)
        {
            strData += "<tr><td width='1%'></td><td></td><td colspan=" + (TransJoListInfo.Columns.Count + 1) + "><table >";
            /*...............................................................Color Row Value....................................................................*/
            strData += "<tr><td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'><div style='CURSOR: hand' onclick=\"ShowDetail(this,'" + factoryCd + "','" + strJo + "','" + colorRow["Color"] + "','"+IsChecked+"')\">+ " + colorRow["Color"] + "</div></td><td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'></td>";
            foreach (DataColumn column in TransJoListInfo.Columns)
            {
                if (TransColorInfo.Columns.Contains(column.ColumnName) || column.ColumnName == "OrderQTY" || column.ColumnName == "CUTQTY")
                {
                    if (column.ColumnName == "OrderQTY")
                    {
                        strData += "<td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'>" + colorRow["totalcutqty"] + "</td>";
                    }
                    else if (column.ColumnName == "CUTQTY")
                    {
                        strData += "<td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'>" + colorRow["cutQty"] + "</td>";
                    }
                    else if (column.ColumnName == "ShipQty(For reference)")
                    {
                        strData += "<td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'>" + colorRow["ShipQty(For reference)"] + "</td>";
                    }
                    else if (column.ColumnName == "JO")
                    {

                    }
                    else
                    {
                        string id = factoryCd + ">" + strJo + ">" + colorRow["Color"] + ">" + "" + ">" + column.ColumnName;
                        strData += "<td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'><a href=\"#\" onclick=\"javascript:window.open('showYMGProcessDetail.aspx?id=" + id + "','','height=200, width=400, top=200, left=450, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no','')\">" + colorRow[column.ColumnName] + "</a></td>";
                    }
                }
                else
                {
                    strData += "<td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'></td>";
                }
            }
            strData += "</tr>";
            strData += "<tr style='display:none'><td colspan=" + (TransJoListInfo.Columns.Count + 1) + "></td></tr></table></td></tr>";
        }
        strData += "</table>";
        return strData;
    }
    [AjaxPro.AjaxMethod]
    public string GetSizeData(string factoryCd, string strJo, string strColor,string IsChecked)
    {
        string strData = "";
        DataTable TransSizeInfo = MESComment.ProTranWIPSummary.GetProTranWIPSummarySize(factoryCd, strJo, strColor, "%");
        TransSizeInfo = AddShipQtyToDataTable(TransSizeInfo, strJo, strColor, "-",IsChecked);
        TransSizeInfo = AddFgisQtyToDataTable(TransSizeInfo, strJo, "-", "-"); 
        TransJoListInfo = (DataTable)Session["TransJoListInfo"];
        TransSizeInfo.Columns.Remove("ORDER_QTY");
        if (TransSizeInfo.Rows.Count > 0)
        {
            strData += "<table ><tr><td width='" + 100 / (TransJoListInfo.Columns.Count + 1) + "%'></td><td colspan=" + TransJoListInfo.Columns.Count + "><hr></td></tr>";
        }
        //Size Row
        foreach (DataRow sizeRow in TransSizeInfo.Rows)
        {
            strData += "<tr><td width='" + 100 / (TransJoListInfo.Columns.Count + 1) + "%'></td><td width='" + 100 / (TransJoListInfo.Columns.Count + 1) + "%'>" + sizeRow["SIZE"] + "</td>";
            foreach (DataColumn column in TransJoListInfo.Columns)
            {
                if (TransSizeInfo.Columns.Contains(column.ColumnName) || column.ColumnName == "OrderQTY" || column.ColumnName == "CUTQTY")
                {
                    if (column.ColumnName == "OrderQTY")
                    {
                        strData += "<td width='" + 100 / (TransJoListInfo.Columns.Count + 1) + "%'>" + sizeRow["totalcutqty"] + "</td>";
                    }
                    else if (column.ColumnName == "CUTQTY")
                    {
                        strData += "<td width='" + 100 / (TransJoListInfo.Columns.Count + 1) + "%'>" + sizeRow["cutQty"] + "</td>";
                    }
                    else if (column.ColumnName == "ShipQty(For reference)")
                    {
                        strData += "<td width='" + 100 / (TransJoListInfo.Columns.Count + 1) + "%'>" + sizeRow["ShipQty(For reference)"] + "</td>";
                    }
                    else if (column.ColumnName == "JO")
                    {
                    }
                    else
                    {
                        string id = factoryCd + ">" + strJo + ">" + strColor + ">" + sizeRow["SIZE"] + ">" + column.ColumnName;
                        strData += "<td width='" + 100 / (TransJoListInfo.Columns.Count + 1) + "%'><a href=\"#\" onclick=\"javascript:window.open('showYMGProcessDetail.aspx?id=" + id + "','','height=200, width=400, top=200, left=450, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no','')\">" + sizeRow[column.ColumnName] + "</a> </td>";
                    }
                }
                else
                {
                    strData += "<td width='" + 100 / (TransJoListInfo.Columns.Count + 1) + "%'></td>";
                }
            }
            strData += "</tr>";
        }
        return strData;
    }
    private void GenerateTransInfoView(string JoListTableName, DbConnection MESConn)
    {
        Session["dtShipQTYAndWarehouseQTY"] = dtShipQTYAndWarehouseQTY;
        TransJoListInfo = MESComment.ProTranWIPSummary.GetProTranWIPDetail(JoListTableName, MESConn, factoryCd);
        Session["dtFgisQTYAndWarehouseQTY"] = MESComment.ProTranWIPSummary.GetProTranFGISDetail(JoListTableName, MESConn, factoryCd);
       
        TransJoListInfo = AddShipQtyToDataTable(TransJoListInfo, "-", "-", "-",chkShippedQty.Checked==true?"True":"False");

        TransJoListInfo = AddFgisQtyToDataTable(TransJoListInfo, "-", "-", "-");
        
        Session["TransJoListInfo"] = TransJoListInfo;
        if (TransJoListInfo.Rows.Count > 0 && TransJoListInfo.Columns.Count > 1)
        {
            total = new int[TransJoListInfo.Columns.Count - 1];
            strHtmlList.Add("<table id='tbJo'>");
            /*=====================================================================生成transInfo Title=====================================================================*/
            strHtmlList.Add("<tr bgcolor='#8FBBD6'><td width='1%'></td>");
            foreach (DataColumn column in TransJoListInfo.Columns)
            {
                if (column.ColumnName == "JO")
                {
                    strHtmlList.Add("<td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'>" + column.ColumnName + "</td>");
                    strHtmlList.Add("<td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'>Color</td>");
                    strHtmlList.Add("<td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'>Size</td>");
                }
                else
                {
                    strHtmlList.Add("<td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'>" + column.ColumnName + "</td>");
                }
            }
            strHtmlList.Add("</tr>");
            int ii = 0;
            foreach (DataRow JoRow in TransJoListInfo.Rows)
            {
                ii++;
                /*========================================================================生成Jo Row=================================================================*/
                strHtmlList.Add("<tr><td colspan=" + (TransJoListInfo.Columns.Count + 3) + "><table>");
                /*-----------------------------------------------------------------------Jo Row value----------------------------------------------------------------*/
                strHtmlList.Add("<tr><td width='1%'><div style='CURSOR: hand' onclick=\"ShowDetail(this,'" + factoryCd + "','" + JoRow[0].ToString() + "','-','"+chkShippedQty.Checked+"')\">+</div></td>");
                int i = 0;

                foreach (DataColumn column in TransJoListInfo.Columns)
                {
                   
                    if (column.ColumnName == "JO")
                    {
                        strHtmlList.Add("<td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'>" + JoRow[column.ColumnName] + "</td>");
                        strHtmlList.Add("<td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'></td>");
                        strHtmlList.Add("<td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'></td>");
                    }
                    else if (column.ColumnName == "OrderQTY" || column.ColumnName == "CUTQTY" || column.ColumnName == "ShipQty(For reference)" || column.ColumnName == "FGIS INQTY")
                    {
                        strHtmlList.Add("<td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'>" + JoRow[column.ColumnName] + "</td>");
                        total[i++] += JoRow[column.ColumnName].ToString() == "" ? 0 : int.Parse(JoRow[column.ColumnName].ToString());
                    }
                    else
                    {
                        string id = factoryCd + ">" + JoRow["JO"] + ">" + "" + ">" + "" + ">" + column.ColumnName;
                        strHtmlList.Add("<td width='" + 99 / (TransJoListInfo.Columns.Count + 2) + "%'><a href=\"#\" onclick=\"javascript:window.open('showYMGProcessDetail.aspx?id=" + id + "','','height=200, width=400, top=200, left=450, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=n o, status=no','')\">" + JoRow[column.ColumnName] + "</a></td>");
                        total[i++] += JoRow[column.ColumnName].ToString() == "" ? 0 : int.Parse(JoRow[column.ColumnName].ToString());
                    }
                }
                strHtmlList.Add("</tr>");
                strHtmlList.Add("<tr style='display:none'><td colspan=" + (TransJoListInfo.Columns.Count + 3) + "></td></tr>");
                strHtmlList.Add("</table></td></tr>");
            }
            if (TransJoListInfo.Rows.Count > 0)
            {
                strHtmlList.Add("<tr><td></td><td>TTL</td><td>-</td><td>-</td>");
                for (int i = 0; i < total.Length; i++)
                {
                    strHtmlList.Add("<td>" + total[i] + "</td>");
                }
                strHtmlList.Add("</tr>");
            }
            strHtmlList.Add("</table>");
        }
    }
 
}
