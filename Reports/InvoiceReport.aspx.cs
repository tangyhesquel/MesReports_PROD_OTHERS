using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;

public partial class Reports_InvoiceReport : pPage
{
    string userid = "";
    protected void Page_Load(object sender, EventArgs e)
    {
       
        if (!IsPostBack)
        {
            if (Request.QueryString["userid"] != null)
            {
                userid = Request.QueryString["userid"].ToString();
            }
            string factory = "";
            if (Request.QueryString["site"] != null)
            {
                if (Request.QueryString["site"].ToString() != "DEV")
                {
                    factory = Request.QueryString["site"].ToString();
                }
                else
                {
                    factory = "GEG";
                }

            }
            else
            {
                factory = "GEG";
            }
            Session["site"] = factory;
            ddlFtyCd.DataSource = MESComment.MesRpt.GetFactoryList(userid);
            ddlFtyCd.DataTextField = "DEPARTMENT_ID";
            ddlFtyCd.DataValueField = "DEPARTMENT_ID";
            ddlFtyCd.DataBind();
            ddlFtyCd.SelectedValue = factory;
            //divSummary.Visible = false;
            //if (userid != "")
            //{
            //    DataTable dtAuthorise = MESComment.MUReportSql.LockAuthorise(userid);

            //    if (dtAuthorise.Rows.Count > 0)
            //    {
            //        btnLock.Visible = true;

            //    }
            //    else
            //    {
            //        btnLock.Visible = false;

            //    }
            //}
            //else
            //{
            //    btnLock.Visible = false;

            //}
        }
       
    }

    private bool CheckCondition()
    {
        if (txtJoNo.Text == "")
        {
            if (txtFromDate.Text == "" && txtToDate.Text == "")
                return false;
            if (txtFromDate.Text == "" || txtToDate.Text == "")
                return false;
            return true;
        }
        return true;
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
       
        if (!CheckCondition())
            return;
        this.nodata.Visible = false;
        gvDetail.Visible = false;
        DataTable jodt = GetAllJo();
        if (jodt.Rows.Count == 0)
        {
            this.nodata.Visible = true;
            return;
        }
        gvDetail.Visible = true;
        beginProgress();
        SetTiile("Invoice Report");
        
        Random ro = new Random(1000);
        string strRunNO = Convert.ToString(string.Format("{0:yy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(10).ToString();
        setProgress(1, "Process data from Gis system", "Get Shipment Data Begin..");
        //查所有ori jo 的inv qty
        DataTable gisshipinfodt = MESComment.MUReportSql.GetStandardGisInfo(GetJoList(jodt), ddlFtyCd.SelectedItem.Value,"", "","", "", true, false, strRunNO);
        //所有CB的单
        DataTable cbdt = GetJo(jodt);
        Dictionary<string, List<string>> combinejodic = GetCBDic(cbdt);
        gisshipinfodt = CombineGisToCB(combinejodic, gisshipinfodt);
        setProgress(10, "Process data from Gis system End", "Process data from Inv system Begin..");
        string wh = GetIssueWhs(ddlFtyCd.SelectedItem.Value);
        if (wh == "")
            return;
        DbConnection invConn = null;
        invConn = MESComment.DBUtility.GetConnection("inv_support");
        MESComment.DBUtility.InsertRptTempData(jodt, invConn);
        //取出INV数
        DataTable invoutputinfodt = GetInvOutPutQty("", wh, invConn);
        invConn.Close();
       
        invoutputinfodt = CombineInvToCB(combinejodic, invoutputinfodt);
        setProgress(80, "Process data from Inv system End", "Process data from MES system  Begin..");
        DataTable mestransferdt = GetMesInfo(GetCBAndOriJo(combinejodic, jodt));
       //取leftover
        DataTable leftover = GetMesLeftoverInfo(GetJoList(jodt));
        DeelwithLeftover(cbdt, mestransferdt, leftover);
        setProgress(90, "Process data from MES system End", "Process data from Fgis system  Begin..");
        //FGIS Grade A & B
        invConn = MESComment.DBUtility.GetConnection("INV");

        MESComment.DBUtility.InsertRptTempData(jodt, invConn);

       DataTable FgisLeftoverAB = MESComment.MUReportSql.GetStatudardLeftoverAB(ddlFtyCd.SelectedItem.Value, invConn);
       invConn.Close();
       DeelwithFgisLeftover(cbdt, mestransferdt, FgisLeftoverAB);


        setProgress(95, "All data Ready", "Process All data Begin..");
        CombineGisAndMes(gisshipinfodt, mestransferdt, invoutputinfodt);
        finishProgress();
        DataView dv = mestransferdt.AsDataView();
        dv.Sort = "JO_NO";
        gvDetail.DataSource = dv.ToTable();

        gvDetail.DataBind();
        GenSumRow(mestransferdt);
        gvDetail.FooterRow.Cells[0].Text = "Total:";
    }
    private void DeelwithLeftover(DataTable cbdt, DataTable mestransferdt, DataTable leftover)
    {
        DataRow[] selectedrow = null;
        //mestransferdt.Columns.Add("LeftOverQty", typeof(int));
        mestransferdt.Columns["InWHS"].ReadOnly = false;
        if (leftover.Rows.Count > 0)
        {
            foreach (DataRow dr in leftover.Rows)
            {
                selectedrow = cbdt.Select("ORIGINAL_JO_NO='" + dr["JOB_ORDER_NO"] + "'");
                if (selectedrow.Length > 0)//Conbine Jo
                {
                    selectedrow = mestransferdt.Select("JO_NO='" + selectedrow[0]["COMBINE_JO_NO"] + "'");
                    //selectedrow[0]["LeftOverQty"] = selectedrow[0]["LeftOverQty"].ToDouble() + dr["Qty"].ToDouble();
                    selectedrow[0]["InWHS"] = selectedrow[0]["InWHS"].ToDouble() - dr["Qty"].ToDouble();
                }
                else
                {
                    selectedrow = mestransferdt.Select("JO_NO='" + dr["JOB_ORDER_NO"] + "'");
                    //selectedrow[0]["LeftOverQty"] = selectedrow[0]["LeftOverQty"].ToDouble() + dr["Qty"].ToDouble();
                    selectedrow[0]["InWHS"] = selectedrow[0]["InWHS"].ToDouble() - dr["Qty"].ToDouble();
                }

            }

            //mestransferdt.Columns["InWHS"].ReadOnly = false;
            //double cbleftoverqty = 0;
            //foreach (string key in combinejodic.Keys)
            //{
            //    cbleftoverqty = 0;
            //    foreach (string orijostr in combinejodic[key])
            //    {
            //        selectedrow = leftover.Select("JOB_ORDER_NO='" + orijostr + "'");
            //        if (selectedrow.Length > 0)
            //        {
            //            cbleftoverqty += selectedrow[0]["Qty"].ToDouble();
            //        }
            //    }
            //    if (cbleftoverqty == 0)
            //        continue;
            //    selectedrow = mestransferdt.Select("JO_NO='" + key + "'");
            //    if (selectedrow.Length > 0)
            //    {
            //        selectedrow[0]["InWHS"] = selectedrow[0]["InWHS"].ToDouble() - cbleftoverqty;
            //    }

            //}
        }
    }

    private void DeelwithFgisLeftover(DataTable cbdt, DataTable mestransferdt, DataTable leftover)
    {
        DataRow[] selectedrow = null;
        mestransferdt.Columns.Add("LeftOverQty", typeof(int));
        
        if (leftover.Rows.Count > 0)
        {
            foreach (DataRow dr in leftover.Rows)
            {
                selectedrow = cbdt.Select("ORIGINAL_JO_NO='" + dr["JO_NO"] + "'");
                if (selectedrow.Length > 0)//Conbine Jo
                {
                    selectedrow = mestransferdt.Select("JO_NO='" + selectedrow[0]["COMBINE_JO_NO"] + "'");
                    selectedrow[0]["LeftOverQty"] = selectedrow[0]["LeftOverQty"].ToDouble() + dr["gmt_qty_a"].ToDouble() + dr["gmt_qty_b"].ToDouble();
                    //selectedrow[0]["InWHS"] = selectedrow[0]["InWHS"].ToDouble() - dr["Qty"].ToDouble();
                }
                else
                {
                    selectedrow = mestransferdt.Select("JO_NO='" + dr["JO_NO"] + "'");
                    selectedrow[0]["LeftOverQty"] = selectedrow[0]["LeftOverQty"].ToDouble() + dr["gmt_qty_a"].ToDouble() + dr["gmt_qty_b"].ToDouble();
                    //selectedrow[0]["InWHS"] = selectedrow[0]["InWHS"].ToDouble() - dr["Qty"].ToDouble();
                }

            }

           
        }
    }
    private DataTable GetAllJo()
    {
       
        string jo = txtJoNo.Text.ToUpper().Trim();
        DataTable jodt = MESComment.MUReportSql.GetStandardGisInfo(ddlFtyCd.SelectedItem.Value, ddlGarmentType.SelectedItem.Value, ddlOutSource.SelectedItem.Value, jo, txtFromDate.Text, txtToDate.Text);
        if (jodt.Rows.Count == 0 && jo.Length > 0)
       //当用户输入CB单时，GIS查不到数据，在此处理
        {
            jodt = GetJo(jo);
            
        }
        if(jodt.Rows.Count==0)
            return jodt;
        //因为当按日期查询时，CB的单，有可能一部分ori jo不在日期范围内，程序要处理这些单
        jodt = GetAllOriJoByJo(jodt);
        return jodt;
    }
    private string GetCBAndOriJo(Dictionary<string, List<string>> combinejodic,DataTable jodt)
    {
        System.Text.StringBuilder cbsb = new System.Text.StringBuilder();
        List<string> cbjolist = new List<string>();
        foreach (string key in combinejodic.Keys)
        {
             cbsb.AppendFormat("'{0}',", key);
             foreach (string jo in combinejodic[key])
             {
                 cbjolist.Add(jo);
             }
        }
        foreach (DataRow dr in jodt.Rows)
        {
            if (cbjolist.Contains(dr[0].ToString()))
                continue;
            cbsb.AppendFormat("'{0}',", dr[0]);
        }
        if(cbsb.Length>0)
            cbsb=cbsb.Remove(cbsb.Length-1,1);
        return cbsb.ToString();
        
    }
    private Dictionary<string, List<string>> GetCBDic(DataTable cbdt)
    {
         Dictionary<string, List<string>> combinejodic = new Dictionary<string, List<string>>();
        foreach (DataRow dr in cbdt.Rows)
        {
            if (combinejodic.ContainsKey(dr[0].ToString()))
            {
                combinejodic[dr[0].ToString()].Add(dr[1].ToString());
            }
            else
            {
                combinejodic.Add(dr[0].ToString(), new List<string> { dr[1].ToString() });
            }
        }
        return combinejodic;
    }
    //protected void btnQuery_Click2(object sender, EventArgs e)
    //{

    //    if (!CheckCondition())
    //        return;
       
    //    this.nodata.Visible = false;
    //    gvDetail.Visible = false;
    //    //divSummary.Visible = false;
    //    string JoList = "";
    //    string DateFrom = "";
    //    string DateTo = "";
    //    if (txtJoNo.Text != "")
    //    {
    //        string[] value = txtJoNo.Text.Trim().Split(';');
    //        for (int i = 0; i < value.Length; i++)
    //        {
    //            if (value[i] != "")
    //            {
    //                JoList += "'" + value[i] + "',";
    //            }
    //        }
    //        JoList = JoList.Substring(0, JoList.Length - 1);
    //        txtFromDate.Text = "";
    //        txtToDate.Text = "";

    //    }
    //    DateFrom = txtFromDate.Text;
    //    DateTo = txtToDate.Text;
    //    if (txtFromDate.Text != "" || txtToDate.Text != "")
    //    {
    //        lblshipinfo.Text = "Ship Date:";
    //        if (txtFromDate.Text != "")
    //        {
    //            lblshipinfo.Text += " From " + txtFromDate.Text;
    //        }
    //        if (txtToDate.Text != "")
    //        {
    //            lblshipinfo.Text += " To " + txtToDate.Text;
    //        }
    
    //    }
    //    System.Text.StringBuilder orijo = new System.Text.StringBuilder();
    //    System.Text.StringBuilder comjo = new System.Text.StringBuilder();
    //    Dictionary<string, List<string>> combinejodic = new Dictionary<string, List<string>>();
    //    //Dictionary<string, List<string>> onlycombinejodic = new Dictionary<string, List<string>>();
       
     
    //    beginProgress();
    //    SetTiile("Invoice Report");
        
    //    setProgress(1, "", "");
    //    if (JoList.Length > 0)
    //    {
    //        DataTable combinejodt = GetJo(JoList);
           

    //        foreach (DataRow dr in combinejodt.Rows)
    //        {
    //            orijo.AppendFormat("'{0}',", dr["ORIGINAL_JO_NO"]);
               
    //        }
    //        if(orijo.Length>0)
    //            orijo.Remove(orijo.Length - 1, 1);
    //    }
    //    Random ro = new Random(1000);
    //   string strRunNO = Convert.ToString(string.Format("{0:yy%M%d%H%mm%ss}", DateTime.Now)) + ro.Next(10).ToString();
    //   setProgress(1, "Process data from Gis system", "Get Shipment Data Begin..");
    //   DataTable gisshipinfodt = MESComment.MUReportSql.GetStandardGisInfo(orijo.ToString(), ddlFtyCd.SelectedItem.Value, ddlGarmentType.SelectedItem.Value, ddlOutSource.SelectedItem.Value, txtFromDate.Text, txtToDate.Text, true, false, strRunNO);
     
    //   if (gisshipinfodt.Rows.Count == 0)
    //   {
    //       this.nodata.Visible = true;
    //       return;
    //   }
    //   gvDetail.Visible = true;
    //   JoList=GetJoList(gisshipinfodt);
    //   DataTable combinejodtfromgis = GetJo(JoList);
    //   DataRow _newdr = null;
    //   if (combinejodtfromgis.Rows.Count <= 0)
    //   {
    //       foreach (DataRow dr in gisshipinfodt.Rows)
    //       {
    //           _newdr = combinejodtfromgis.NewRow();
    //           _newdr["COMBINE_JO_NO"] = dr["JO_NO"];
    //           _newdr["ORIGINAL_JO_NO"] = dr["JO_NO"];
    //           _newdr["CombineFlag"] = "N";
    //           combinejodtfromgis.Rows.Add(_newdr);
    //       }
    //   }

    //   DataTable comjodt = new DataTable();
    
    //   orijo.Remove(0, orijo.Length);
    //   comjo.Remove(0, comjo.Length);
    //   comjodt.Columns.Add("jobno");
       
    //       foreach (DataRow dr in combinejodtfromgis.Rows)
    //       {
    //           if (dr["CombineFlag"].ToString() == "Y")
    //           {
    //               if (combinejodic.ContainsKey(dr["COMBINE_JO_NO"].ToString()))
    //               {
    //                   combinejodic[dr["COMBINE_JO_NO"].ToString()].Add(dr["ORIGINAL_JO_NO"].ToString());
    //                   //加上被Combine单
    //                   _newdr = comjodt.NewRow();
    //                   _newdr[0] = dr["ORIGINAL_JO_NO"];
    //                   comjodt.Rows.Add(_newdr);
    //               }
    //               else
    //               {
    //                   comjo.AppendFormat("'{0}',", dr["COMBINE_JO_NO"]);
    //                   //加上Combine单
    //                   _newdr = comjodt.NewRow();
    //                   _newdr[0] = dr["COMBINE_JO_NO"];
    //                   comjodt.Rows.Add(_newdr);
    //                   //加上被Combine单
    //                   _newdr = comjodt.NewRow();
    //                   _newdr[0] = dr["ORIGINAL_JO_NO"];
    //                   comjodt.Rows.Add(_newdr);
    //                   combinejodic.Add(dr["COMBINE_JO_NO"].ToString(), new List<string> { dr["ORIGINAL_JO_NO"].ToString() });
    //               }
    //               orijo.AppendFormat("'{0}',", dr["ORIGINAL_JO_NO"]);
    //           }
    //           else
    //           {
    //               comjo.AppendFormat("'{0}',", dr["COMBINE_JO_NO"]);
    //               orijo.AppendFormat("'{0}',", dr["ORIGINAL_JO_NO"]);
    //               _newdr = comjodt.NewRow();
    //               _newdr[0] = dr["COMBINE_JO_NO"];
    //               comjodt.Rows.Add(_newdr);
    //           }

    //       }
    //       if (orijo.Length > 0)
    //           orijo.Remove(orijo.Length - 1, 1);
    //       if (comjo.Length > 0)
    //           comjo.Remove(comjo.Length - 1, 1);
       
      
    //   DbConnection invConn = null;
    //   gisshipinfodt = CombineGisToCB(combinejodic, gisshipinfodt);
    //   setProgress(10, "Process data from Gis system End", "");
    //   setProgress(10, "Process data from Inv system", "Get Out Storage Data Begin..");
    //   //DataTable invoutputinfodt = GetInvOutPutQty(JoList, "'GEG-A01'");
    //   //invoutputinfodt=CombineInvToCB(combinejodic, invoutputinfodt);
    //   string wh = GetIssueWhs(ddlFtyCd.SelectedItem.Value);
    //   if (wh == "")
    //       return;
    //   invConn = MESComment.DBUtility.GetConnection("inv_support");
    //   MESComment.DBUtility.InsertRptTempData(comjodt, invConn);
    //   DataTable invoutputinfodt = GetInvOutPutQty(orijo.ToString(), wh, invConn);
    //   invoutputinfodt = CombineInvToCB(combinejodic, invoutputinfodt);
    //   //setProgress(80, "Process data from Inv system", "Get Out Storage Data Begin..");
    //   //setProgress(80, "Process data from MES system", "Get In Storage Data Begin..");
    //   DataTable mestransferdt = GetMesInfo(comjo.ToString());

    //   DataTable leftover = GetMesLeftoverInfo(orijo.ToString());
    //   DataRow[] selectedrow = null;
    //   if (leftover.Rows.Count > 0)
    //   {

    //       mestransferdt.Columns["InWHS"].ReadOnly = false;
    //       double cbleftoverqty = 0;
    //       foreach (string key in combinejodic.Keys)
    //       {
    //           cbleftoverqty = 0;
    //           foreach (string orijostr in combinejodic[key])
    //           {
    //               selectedrow = leftover.Select("JOB_ORDER_NO='" + orijostr + "'");
    //               if (selectedrow.Length > 0)
    //               {
    //                   cbleftoverqty+=selectedrow[0]["Qty"].ToDouble();
    //               }
    //           }
    //           if (cbleftoverqty == 0)
    //               continue;
    //           selectedrow = mestransferdt.Select("JO_NO='" + key + "'");
    //           if (selectedrow.Length > 0)
    //           {
    //               selectedrow[0]["InWHS"] = selectedrow[0]["InWHS"].ToDouble() - cbleftoverqty;
    //           }

    //       }
    //   }
    //   setProgress(90, "Process data from MES system End", "");
    //   setProgress(90, "All Data Ready", "Combine Data");
    //   CombineGisAndMes(gisshipinfodt, mestransferdt, invoutputinfodt);
    //   finishProgress();
    //   DataView dv = mestransferdt.AsDataView();
    //   dv.Sort = "JO_NO";
    //   gvDetail.DataSource = dv.ToTable();

    //   gvDetail.DataBind();
    //   GenSumRow(mestransferdt);
    //   gvDetail.FooterRow.Cells[0].Text = "Total:";
    //}
    private string GetJoList(DataTable dt)
    {
        System.Text.StringBuilder comjo = new System.Text.StringBuilder();
        foreach (DataRow dr in dt.Rows)
        {
            comjo.AppendFormat("'{0}',", dr[0]);
        }
        if (comjo.Length > 0)
            comjo.Remove(comjo.Length - 1, 1);
        return comjo.ToString();
    }
    private DataTable GetMesInfo(string JoList)
    {
        System.Text.StringBuilder sql = new System.Text.StringBuilder();
        sql.AppendFormat(@"WITH    joqty
          AS ( SELECT   c.SHORT_NAME ,
                        JO_NO ,
                        total_qty
               FROM     dbo.JO_HD h ( NOLOCK )
                        LEFT JOIN dbo.GEN_CUSTOMER c ( NOLOCK ) ON h.CUSTOMER_CD = c.CUSTOMER_CD
               WHERE    JO_NO IN ( {0} )   AND FACTORY_CD = '{1}'
                        AND h.[STATUS] <> 'D'
             ),
        transferinfo
          AS ( SELECT   b.JOB_ORDER_NO ,
                        SUM(b.COMPLETE_QTY) AS InWHS ,
                        MAX(a.TRX_DATE) AS InDate 
                      
               FROM     PRD_JO_OUTPUT_HD a ( NOLOCK )
                        INNER JOIN PRD_JO_OUTPUT_TRX b ( NOLOCK ) ON a.DOC_NO = b.DOC_NO
               WHERE    JOB_ORDER_NO IN ( {0} )
                        and b.PROCESS_CD = 'FIN'
                        AND a.FACTORY_CD = '{1}'
               GROUP BY b.JOB_ORDER_NO
             )
    SELECT  a.SHORT_NAME as Buyer,a.JO_NO,a.total_qty,b.InWHS,b.InDate 
    FROM    joqty a
            LEFT JOIN transferinfo b ON a.JO_NO = b.JOB_ORDER_NO", JoList, ddlFtyCd.SelectedItem.Value);

        return MESComment.DBUtility.GetTable(sql.ToString(), "MES");
    }

    private DataTable GetMesLeftoverInfo(string JoList)
    {
        System.Text.StringBuilder sql = new System.Text.StringBuilder();
        sql.AppendFormat(@"
             
              SELECT   b.JOB_ORDER_NO ,SUM(QTY) as Qty
                      
               FROM     PRD_GARMENT_TRANSFER_HD a ( NOLOCK )
                        INNER JOIN PRD_GARMENT_TRANSFER_DFT b ( NOLOCK ) ON a.DOC_NO = b.DOC_NO
               WHERE    JOB_ORDER_NO IN ( {0} )
                        and a.[STATUS]='C'
                        and a.PROCESS_CD = 'FIN'
                        AND a.FACTORY_CD = '{1}'
                        AND REASON_CD='G15'
               GROUP BY b.JOB_ORDER_NO
              ", JoList, ddlFtyCd.SelectedItem.Value);

        return MESComment.DBUtility.GetTable(sql.ToString(), "MES");
    }

    private string GetIssueWhs(string factory)
    {
        System.Text.StringBuilder sql = new System.Text.StringBuilder();
        sql.AppendFormat(@"SELECT SYSTEM_VALUE FROM  dbo.GEN_SYSTEM_SETTING WHERE SYSTEM_KEY='ISSWHS' and FACTORY_CD='{0}' ", factory);

        object dt = MESComment.DBUtility.ExecuteScalar(sql.ToString(), "MES_UPDATE");
        if (dt == null)
            return "";
        string result = string.Format("'{0}'", dt.ToString().Replace(",", "','"));
        return result;
       
    }
    /// <summary>
    ///把CB单分解
    /// </summary>
    /// <param name="JoList"></param>
    /// <returns></returns>
    private DataTable GetJo(string JoList)
    {
        System.Text.StringBuilder sql = new System.Text.StringBuilder();
//        sql.AppendFormat(@"
//                            SELECT  JO_NO ,
//                                    CREATED_COMBINE_JO_FLAG
//                            INTO    #tmpjo
//                            FROM    JO_HD (NOLOCK)
//                            WHERE   JO_NO IN ( {0} )
//                                    AND [STATUS] <> 'D'
//                            SELECT  JO_NO AS COMBINE_JO_NO ,
//                                    JO_NO AS ORIGINAL_JO_NO,'N' CombineFlag
//                            FROM    #tmpjo
//                            WHERE    ISNULL(CREATED_COMBINE_JO_FLAG,'N') = 'N'
//                            UNION
//                            SELECT  ISNULL(COMBINE_JO_NO, JO_NO) ,
//                                    ISNULL(ORIGINAL_JO_NO, JO_NO) AS ORIGINAL_JO_NO,'Y'
//                            FROM    #tmpjo h
//                                    INNER JOIN dbo.JO_COMBINE_MAPPING m ( NOLOCK ) ON h.JO_NO = m.COMBINE_JO_NO
//                            UNION
//                            SELECT  ISNULL(COMBINE_JO_NO, JO_NO) AS JO_NO ,
//                                    JO_NO AS ORIGINAL_JO_NO,'Y'
//                            FROM    #tmpjo h
//                                    INNER JOIN dbo.JO_COMBINE_MAPPING m ( NOLOCK ) ON h.JO_NO = m.ORIGINAL_JO_NO ", JoList);
        sql.AppendFormat(@"
                            SELECT ORIGINAL_JO_NO as JO_NO  from JO_COMBINE_MAPPING where COMBINE_JO_NO='{0}' ", JoList);


       return MESComment.DBUtility.GetTable(sql.ToString(), "MES");



    }
    private DataTable GetJo(DataTable dt)
    {
        System.Text.StringBuilder sql = new System.Text.StringBuilder();
        string jo = GetJoList(dt);
        sql.AppendFormat(@"SELECT COMBINE_JO_NO,ORIGINAL_JO_NO FROM dbo.JO_COMBINE_MAPPING WHERE ORIGINAL_JO_NO IN({0})", jo);
        return MESComment.DBUtility.GetTable(sql.ToString(), "MES");
    }

    /// <summary>
    ///通过JO加所有Origal Jo
    /// </summary>
    /// <param name="JoList">Jo</param>
    /// <returns>Origal Jo 和 Jo</returns>
    private DataTable GetAllOriJoByJo(DataTable dt)
    {

        System.Text.StringBuilder sql = new System.Text.StringBuilder();
        string jo = GetJoList(dt);
        sql.AppendFormat(@"
                         
                            SELECT distinct b.ORIGINAL_JO_NO FROM dbo.JO_COMBINE_MAPPING a 
                            INNER JOIN JO_COMBINE_MAPPING b ON a.COMBINE_JO_NO=b.COMBINE_JO_NO
                            WHERE a.ORIGINAL_JO_NO in ({0}) and b.ORIGINAL_JO_NO not in({0})
        ", jo);

        DataTable newdt=MESComment.DBUtility.GetTable(sql.ToString(), "MES");
        DataRow newdr;
        foreach(DataRow dr in newdt.Rows)
        {
            newdr = dt.NewRow();
            newdr[0] = dr[0];
            dt.Rows.Add(newdr[0]);
        }
        //if (newdt.Rows.Count > 0)
        //{
        //    return jo + JoList.ToString();
        //}
        return dt;


    }
  
    private DataTable CombineGisToCB(Dictionary<string, List<string>> combinejodic, DataTable Originaldt)
    {
        DataTable dt = Originaldt.Copy();
        bool iscombine = false;
        DataRow _dr = null;
        foreach (string key in combinejodic.Keys)
        {
            iscombine = false;

            foreach (string jo in combinejodic[key])
            {
                DataRow[] jorow = dt.Select("JO_NO='" + jo + "'");
                if (jorow.Count() == 0)
                    continue;
                if (!iscombine)
                {
                    _dr = jorow[0];
                    _dr[1] = key;
                    iscombine = true;
                    if (jorow.Count() > 1)
                    {
                        for (int i = 1; i < jorow.Count(); i++)
                        {
                            if (_dr["SHIP_DATE"].ToDate() < jorow[i]["SHIP_DATE"].ToDate())
                            {
                                _dr["SHIP_DATE"] = jorow[i]["SHIP_DATE"];
                            }
                            _dr["SHIP_QTY"] = _dr["SHIP_QTY"].ToDouble() + jorow[i]["SHIP_QTY"].ToDouble();

                            dt.Rows.Remove(jorow[i]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < jorow.Count(); i++)
                    {
                        if (_dr["SHIP_DATE"].ToDate() < jorow[i]["SHIP_DATE"].ToDate())
                        {
                            _dr["SHIP_DATE"] = jorow[i]["SHIP_DATE"];
                        }
                        _dr["SHIP_QTY"] = _dr["SHIP_QTY"].ToDouble() + jorow[i]["SHIP_QTY"].ToDouble();

                        dt.Rows.Remove(jorow[i]);
                    }
                }


            }


        }

            //iscombine = false;
            //foreach (string job in combinejodic[key])
            //{
            //    foreach (DataRow dr in Originaldt.Rows)
            //    {
            //        if (dr[1].ToString() == job)
            //        {
            //            if (!iscombine)
            //            {
            //                _dr = dt.NewRow();
            //                _dr.ItemArray = dr.ItemArray;
            //                _dr[1] = key;
            //                dt.Rows.Add(_dr);
            //                iscombine = true;
            //            }
            //            else
            //            {
            //                if (_dr["SHIP_DATE"].ToDate() < dr["SHIP_DATE"].ToDate())
            //                {
            //                    _dr["SHIP_DATE"] = dr["SHIP_DATE"];
            //                }
            //                _dr["SHIP_QTY"] = _dr["SHIP_QTY"].ToDouble() + dr["SHIP_QTY"].ToDouble();
            //            }
            //        }
            //        else
            //        {
            //            _dr = dt.NewRow();
            //            _dr.ItemArray = dr.ItemArray;
            //            dt.Rows.Add(_dr);
            //        }
            //    }

            //}
        
        return dt;
    }

    

    private DataTable CombineInvToCB(Dictionary<string, List<string>> combinejodic, DataTable invoutputinfodt)
    {
        DataTable dt = invoutputinfodt.Copy();
        bool iscombine = false;
        DataRow _dr = null;
        foreach (string key in combinejodic.Keys)
        {
            iscombine = false;

            foreach (string jo in combinejodic[key])
            {
                DataRow[] jorow = dt.Select("JO_NO='" + jo + "'");
                if (jorow.Count() == 0)
                    continue;
                if (!iscombine)
                {
                    _dr = jorow[0];
                    _dr[0] = key;
                    iscombine = true;
                    if (jorow.Count() > 1)
                    {
                        for (int i = 1; i < jorow.Count(); i++)
                        {
                            if (_dr["trans_date"].ToDate() < jorow[i]["trans_date"].ToDate())
                            {
                                _dr["trans_date"] = jorow[i]["trans_date"];
                            }
                            _dr["qty"] = _dr["qty"].ToDouble() + jorow[i]["qty"].ToDouble();

                            dt.Rows.Remove(jorow[i]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < jorow.Count(); i++)
                    {
                        if (_dr["trans_date"].ToDate() < jorow[i]["trans_date"].ToDate())
                        {
                            _dr["trans_date"] = jorow[i]["trans_date"];
                        }
                        _dr["qty"] = _dr["qty"].ToDouble() + jorow[i]["qty"].ToDouble();

                        dt.Rows.Remove(jorow[i]);
                    }
                }


            }


        }

        return dt;
    }
    /// <summary>
    /// 合并
    /// </summary>
    /// <param name="gisshipinfodt"></param>
    /// <param name="mestransferdt"></param>
    private void CombineGisAndMes(DataTable gisshipinfodt, DataTable mestransferdt, DataTable invoutputinfodt)
    {
        mestransferdt.Columns.Add("InDateStr");
        mestransferdt.Columns.Add("OutQty",typeof(int));
        mestransferdt.Columns.Add("OutDate");
        mestransferdt.Columns.Add("ShipDate");
        mestransferdt.Columns.Add("ShipQty", typeof(int));
        mestransferdt.Columns.Add("Diff", typeof(int));
        //mestransferdt.Columns["InDate"].ReadOnly = false;
        foreach (DataRow dr in mestransferdt.Rows)
        {

         
            DataRow[] gisrows = gisshipinfodt.Select("JO_NO='" + dr["JO_NO"] + "'");
            DataRow[] invrows = invoutputinfodt.Select("JO_NO='" + dr["JO_NO"] + "'");

            if (gisrows.Length > 0)
            {
                foreach (DataRow gisrow in gisrows)
                {
                    dr["ShipDate"] = gisrow["SHIP_DATE"].DateToDateStrEmpty();
                    dr["ShipQty"] = gisrow["SHIP_QTY"];
                    dr["Diff"] = gisrow["SHIP_QTY"].ToDouble() - dr["InWHS"].ToDouble();
                    gisshipinfodt.Rows.Remove(gisrow);
                }
            }
            if (invrows.Length > 0)
            {
                foreach (DataRow invrow in invrows)
                {
                    dr["OutDate"] = invrow["trans_date"].DateToDateStrEmpty();
                    dr["OutQty"] = invrow["qty"];
                    invoutputinfodt.Rows.Remove(invrow);
                }
            }


            dr["InDateStr"] = dr["InDate"].DateToDateStrEmpty();
        }
        if (gisshipinfodt.Rows.Count > 0)
        {
            DataRow _newdr;
            foreach (DataRow dr in gisshipinfodt.Rows)
            {
                _newdr = mestransferdt.NewRow();
                _newdr["JO_NO"] = dr["JO_NO"];
                _newdr["ShipDate"] = dr["SHIP_DATE"].DateToDateStr();
                _newdr["ShipQty"] = dr["SHIP_QTY"];
                _newdr["Diff"] = dr["SHIP_QTY"].ToDouble();
                mestransferdt.Rows.Add(_newdr);
                DataRow[] invrows = invoutputinfodt.Select("JO_NO='" + dr["JO_NO"] + "'");
                if (invrows.Length > 0)
                {
                    foreach (DataRow invrow in invrows)
                    {
                        _newdr["OutDate"] = invrow["trans_date"].DateToDateStrEmpty();
                        _newdr["OutQty"] = invrow["qty"];
                        invoutputinfodt.Rows.Remove(invrow);
                    }
                }
            }
        }
        if (invoutputinfodt.Rows.Count > 0)
        {
            DataRow _newdr;
            foreach (DataRow dr in gisshipinfodt.Rows)
            {
                _newdr = mestransferdt.NewRow();
                _newdr["JO_NO"] = dr["JO_NO"];
                _newdr["OutDate"] = dr["SHIP_DATE"].DateToDateStrEmpty();
                _newdr["OutQty"] = dr["SHIP_QTY"];
           
                mestransferdt.Rows.Add(_newdr);
               
               
            }
        }

        
    }
    /// <summary>
    /// 从INV取出仓数和出仓时间
    /// </summary>
    /// <param name="JoList"></param>
    /// <param name="whs"></param>
    /// <returns></returns>
    private DataTable GetInvOutPutQty(string jo,string whs, DbConnection invConn)
    {
        System.Text.StringBuilder sql = new System.Text.StringBuilder();
        sql.AppendFormat(@"
                           select l.reference_no as JO_NO,max(h.trans_date) as trans_date,sum(l.qty) as qty from 
  inv_trans_hd h,
         inv_trans_lines l,
         inv_trans_code tc,
          inv_trans_type tt
 WHERE     h.trans_header_id = l.trans_header_id
   AND h.trans_cd = tc.trans_cd
    AND tc.trans_type_cd = tt.trans_type_cd
    and l.reference_no in(select distinct f1 AS job_order_no from rpt_tmp )
    --and l.reference_no in({1} )
    and tc.trans_type_cd='I' and h.from_store_cd in ({0})
    group by l.reference_no", whs, jo);

        return MESComment.DBUtility.GetTable(sql.ToString(), invConn);

    }

    private void GenSumRow(DataTable dt)
    {
        Dictionary<string, int> SumColumns = GetSumColumn();
        gvDetail.FooterRow.Cells[0].Text = "Total";
        double Total = 0;
        foreach (string key in SumColumns.Keys)
        {
            Total = 0;
            foreach(DataRow dr in dt.Rows)
            {
                Total += dr[key].ToDouble();
            }

            gvDetail.FooterRow.Cells[SumColumns[key]].Text = Total.ToString("N0");
        }

    }
    private Dictionary<string, int> GetSumColumn()
    {
        Dictionary<string, int> sum = new Dictionary<string, int>();
        sum.Add("total_qty", 2);
        sum.Add("InWHS", 3);
        sum.Add("OutQty", 5);
        sum.Add("ShipQty", 7);
        sum.Add("Diff", 9);
        sum.Add("LeftOverQty", 10);
        return sum;
    }
}