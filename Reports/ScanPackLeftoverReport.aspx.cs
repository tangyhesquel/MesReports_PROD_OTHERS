using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Drawing;

public partial class Reports_ScanPackLeftoverReport : pPage
{
    public string JoNo = "";
    public string SCNo = "";
    public string UserId = "";  //Added by MF on 20160420, JO Combination-approve button
    public string ApproveJO;
    public string ApproveScanPack;
    public string ApproveScanPackPer;
    string strculture = "";
    private double percent_Short_Allowed=0;
    private double percent_Over_Allowed = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        string FactoryCd = "";
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                FactoryCd = Request.QueryString["site"].ToString();
            }
            else
            {
                FactoryCd = "GEG";
            }
            ddlFactory.SelectedValue = FactoryCd;
        }
        if (Request.QueryString["JoNo"] != null)
        {
            JoNo = Request.QueryString["JoNo"].ToString();
        }
        if (Request.QueryString["GoNo"] != null)
        {
            SCNo = Request.QueryString["GoNo"].ToString();
        }

        //Added by MF on 20160420, JO Combination-approve button
        if (Request.QueryString["UserId"] != null)
        {
            UserId = Request.QueryString["UserId"].ToString();
        }
        //End of added by MF on 20160420, JO Combination-approve button

        hfValue.Value = FactoryCd;
        if (FactoryCd == "PTX" || FactoryCd == "EGM" || FactoryCd == "TIL" || FactoryCd == "EGV" || FactoryCd == "EAV")
        {
            strculture = "en";
        }
        else
        {
            strculture = "zh";
        }

        if (!IsPostBack)
        {
            ddlFactory.DataSource = MESComment.MesRpt.GetFactoryCd();
            ddlFactory.DataTextField = "FACTORY_ID";
            ddlFactory.DataValueField = "FACTORY_ID";
            ddlFactory.DataBind();

            //Added by MF on 20160420, JO Combination-approve button
            if (UserId.ToString() != "")
            {
                DataTable dtAuthorise = ApproveAuthorise(UserId.ToString());

                if (dtAuthorise.Rows.Count > 0)
                {
                    Approved.Visible = true;
                    UnApproved.Visible = true;
                }
                else
                {
                    Approved.Visible = false;
                    UnApproved.Visible = false;
                }
            }
            else
            {
                Approved.Visible = false;
                UnApproved.Visible = false;
            }
            //End of added by MF on 20160420, JO Combination-approve button


            this.txtJoNo.Text = JoNo.ToString();
            //this.txtGO.Text = SCNo.ToString();
            if (JoNo.ToString() != "" || SCNo.ToString() != "")
            {
                queryDiv.Visible = false;
                this.btnQuery_Click1(null, null);
            }
        }
    }

    protected void btnQuery_Click1(object sender, EventArgs e)
    {
        this.gvDetail.InnerHtml = "";
        this.divMeg.InnerHtml = "";

        if (Check())
        {
            //SetQuery();
            SetQuery1();
        }
        else
        {
            this.divMeg.InnerHtml = "<table width='100%'  style='color:Red; font-size:12px'><tr><td align='left'>PLS input GO/JO ! </td></tr></table>";
        }
    }

    private Boolean Check()
    {
        if (this.txtJoNo.Text.Equals(""))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    protected void Approve_Click(object sender, EventArgs e)
    {
        string userId = "";

        if (Request.QueryString["userId"] != null)
        {
            userId = Request.QueryString["userId"].ToString();
        }

        string site = Request.QueryString["site"].ToString();

        if (userId != "" && txtJoNo.Text != "")
        {



            string SQL = "exec PROC_SEND_EMAIL_SCAN_PACK_STATUS '" + ApproveJOLabel.Text + "','" + userId + "','" + site + "','" + ApproveScanPackLabel.Text + "','" + ApproveScanPackPerLabel.Text + "',1";
            MESComment.DBUtility.ExecuteNonQuery(SQL, "MES");

            SetQuery();
        }
        else if (userId == "" && txtJoNo.Text != "")
        {
            this.divMeg.InnerHtml = "<table width='100%'  style='color:Red; font-size:12px'><tr><td align='left'>Please login with MES to approve JO:" + txtJoNo.Text + ".</td></tr></table>";
        }
    }

    protected void UnApprove_Click(object sender, EventArgs e)
    {
        string userId = "";

        if (Request.QueryString["UserId"] != null)
        {
            userId = Request.QueryString["UserId"].ToString();
        }
        string site = Request.QueryString["site"].ToString();

        if (userId != "" && txtJoNo.Text != "")
        {

            string SQL = "exec PROC_SEND_EMAIL_SCAN_PACK_STATUS '" + ApproveJOLabel.Text + "','" + userId + "','" + site + "','" + ApproveScanPackLabel.Text + "','" + ApproveScanPackPerLabel.Text + "%',0";
            MESComment.DBUtility.ExecuteNonQuery(SQL, "MES");

            SetQuery();
        }
        else if (userId == "" && txtJoNo.Text != "")
        {
            this.divMeg.InnerHtml = "<table width='100%'  style='color:Red; font-size:12px'><tr><td align='left'>Please login with MES to un-approve JO:" + txtJoNo.Text + ".</td></tr></table>";
        }
    }

    private static string[] SizeCOL;
    private void SetAllowed(string percent)
    {
       
                    percent_Short_Allowed = percent.Substring(percent.IndexOf("-") + 1, 4).ToDouble();
                        //Convert.ToDecimal(percent.Substring(percent.IndexOf("-") + 1, 4));
                    percent_Over_Allowed = percent.Substring(percent.IndexOf("+") + 1, 4).ToDouble();
                        //Convert.ToDecimal(percent.Substring(percent.IndexOf("+") + 1, 4));
    }
    public void SetQuery()
    {
        this.gvDetail.InnerHtml = "";
        this.divMeg.InnerHtml = "";

        string JO = txtJoNo.Text;
        string JO_String = "";
        //decimal over_percent = 0;
        //decimal short_percent = 0;
        string percent;
        string FactoryCD = ddlFactory.SelectedValue.ToString();
        bool isCombine = true;

        ApproveJO = "";
        ApproveScanPack = "";
        ApproveScanPackPer = "";

        DataTable DT = MESComment.MesRpt.GetJO(JO, "", FactoryCD);

        if (DT.Rows.Count > 0)
        {
            for (int a = 0; a < DT.Rows.Count; a++)
            {
                DataTable dtCombineJo = GetOriginalJo(DT.Rows[a][1].ToString());

                if (dtCombineJo.Rows.Count == 0)
                {
                    isCombine = false;
                    dtCombineJo.Rows.Add(DT.Rows[a][1].ToString(), DT.Rows[a][1].ToString());
                }

                JO_String = DT.Rows[a][1].ToString();
                if (dtCombineJo.Rows.Count > 0)
                {
                    JO_String = "";
                    for (int i = 0; i < dtCombineJo.Rows.Count; i++)
                    {
                        JO_String += "'" + dtCombineJo.Rows[i]["ORIGINAL_JO_NO"].ToString() + "'";

                        if (dtCombineJo.Rows.Count - 1 != i)
                        {
                            JO_String += ",";
                        }
                    }
                }

                DataTable dtHeader = GetReduceQuantityHeadByJO(DT.Rows[a][1].ToString(), FactoryCD);


                DataTable dtEndDate = GetScanPackEndDateByJO(JO_String, FactoryCD);
                if (dtHeader.Rows.Count > 0)
                {
                    ShowJOHeader(dtHeader, dtEndDate);
                    SetAllowed(dtHeader.Rows[0]["ALLOWED_PERCENT"].ToString());
                    ////Get over short percentage
                    //percent = dtHeader.Rows[0]["ALLOWED_PERCENT"].ToString();
                    //percent_Short_Allowed = percent.Substring(percent.IndexOf("-") + 1, 4).ToDouble();
                    //    //Convert.ToDecimal(percent.Substring(percent.IndexOf("-") + 1, 4));
                    //percent_Over_Allowed = percent.Substring(percent.IndexOf("+") + 1, 4).ToDouble();
                    //    //Convert.ToDecimal(percent.Substring(percent.IndexOf("+") + 1, 4));
                }


                //GetSize
                DataTable dtSize = GetSize(DT.Rows[a][1].ToString());
                if (dtSize == null)
                    return;

                SizeCOL = dtSize.Rows[0]["SIZE"].ToString().Replace("[", "").Replace("]", "").Split(new char[] { ',' });
                if (SizeCOL.Length < 1)
                    return;

                //Summary By JO
                DataTable dtSummaryByJO = GetDetailByJO(DT.Rows[a][1].ToString(), FactoryCD);


                //Scan pack qty
                DataTable dtScanPackQty = GetScanPackQty(JO_String, FactoryCD);

                if (dtSummaryByJO.Rows.Count > 0)
                {
                    foreach (DataColumn col in dtSummaryByJO.Columns)
                    {
                        col.ReadOnly = false;
                    }

                    //合并到主体dt数据
                    string value = string.Empty;
                    string value1 = string.Empty;
                    if (dtScanPackQty != null)
                    {
                        foreach (DataRow row in dtScanPackQty.Rows)
                        {
                            value = row["GOColor"].ToString();
                            value1 = row["GOSize"].ToString();

                            DataRow[] dr = dtSummaryByJO.Select("COLOR_CODE='" + value + "' AND SIZE_CODE='" + value1 + "'");
                            if (dr.Length>0)
                              dr[0]["SCAN_QTY"] = row["PacTotal"];
                        }
                    }


                    for (int i = 0; i < dtSummaryByJO.Rows.Count; i++)
                    {
                        dtSummaryByJO.Rows[i]["COLOR_DESC"] = dtSummaryByJO.Rows[i]["COLOR_CODE"].ToString() + " (" + dtSummaryByJO.Rows[i]["COLOR_DESC"].ToString() + ")";
                        //dtSummaryByJO.Rows[i]["PERCENTAGE"] = (Convert.ToInt32(dtSummaryByJO.Rows[i]["SCAN_QTY"].ToString()) > 0) ? (dtSummaryByJO.Rows[i]["ORDER_QTY"].ToDouble()==0?"0":(Math.Round(((Convert.ToDecimal(dtSummaryByJO.Rows[i]["SCAN_QTY"].ToString()) - Convert.ToDecimal(dtSummaryByJO.Rows[i]["ORDER_QTY"].ToString())) / Convert.ToDecimal(dtSummaryByJO.Rows[i]["ORDER_QTY"].ToString()) * 100), 2) + "%")) : "0";
                        dtSummaryByJO.Rows[i]["PERCENTAGE"] = (dtSummaryByJO.Rows[i]["ORDER_QTY"].ToDouble() == 0 ? 0 : Math.Round((dtSummaryByJO.Rows[i]["SCAN_QTY"].ToInt("r") - dtSummaryByJO.Rows[i]["ORDER_QTY"].ToDouble()) / dtSummaryByJO.Rows[i]["ORDER_QTY"].ToDouble() * 100, 2)) ;
                            //(Convert.ToInt32(dtSummaryByJO.Rows[i]["SCAN_QTY"].ToString()) > 0) ? (dtSummaryByJO.Rows[i]["ORDER_QTY"].ToDouble() == 0 ? "0" : (Math.Round(((Convert.ToDecimal(dtSummaryByJO.Rows[i]["SCAN_QTY"].ToString()) - Convert.ToDecimal(dtSummaryByJO.Rows[i]["ORDER_QTY"].ToString())) / Convert.ToDecimal(dtSummaryByJO.Rows[i]["ORDER_QTY"].ToString()) * 100), 2) + "%")) : "0";
  
                        dtSummaryByJO.Rows[i]["DIFF"] = dtSummaryByJO.Rows[i]["SCAN_QTY"].ToInt("c") + dtSummaryByJO.Rows[i]["LEFTOVER_A"].ToInt("c") + dtSummaryByJO.Rows[i]["GRADE_B"].ToInt("c") +
                                                        dtSummaryByJO.Rows[i]["DISCREPANCY_QTY"].ToInt("c") - dtSummaryByJO.Rows[i]["ACTUAL_QTY"].ToInt("c");
                        //dtSummaryByJO.Rows[i]["MAX_SCAN_PACK"] = Math.Round((Convert.ToDecimal(dtSummaryByJO.Rows[i]["ORDER_QTY"].ToString()) * (over_percent + 100) / 100), 2);
                        //dtSummaryByJO.Rows[i]["MIN_SCAN_PACK"] = Math.Round((Convert.ToDecimal(dtSummaryByJO.Rows[i]["ORDER_QTY"].ToString()) * (100 - short_percent) / 100), 2);

                         dtSummaryByJO.Rows[i]["MAX_SCAN_PACK"] = Math.Round(dtSummaryByJO.Rows[i]["ORDER_QTY"].ToDouble() * (percent_Over_Allowed + 100) / 100, 2);
                        dtSummaryByJO.Rows[i]["MIN_SCAN_PACK"] = Math.Round(dtSummaryByJO.Rows[i]["ORDER_QTY"].ToDouble() * (100 - percent_Short_Allowed) / 100, 2);

                        dtSummaryByJO.Rows[i]["AVAILABLE_BALANCE_SCAN_PACK_QTY"] = Math.Floor((Convert.ToDecimal(dtSummaryByJO.Rows[i]["MAX_SCAN_PACK"].ToString())) - Convert.ToDecimal(dtSummaryByJO.Rows[i]["SCAN_QTY"].ToString()));
                        dtSummaryByJO.Rows[i]["OVER_PACK"] = Math.Abs(Convert.ToDecimal(dtSummaryByJO.Rows[i]["SCAN_QTY"].ToString()) - Convert.ToDecimal(dtSummaryByJO.Rows[i]["MAX_SCAN_PACK"].ToString()));
                        dtSummaryByJO.Rows[i]["SHORT_PACK"] = (Math.Abs(Convert.ToDecimal(dtSummaryByJO.Rows[i]["SCAN_QTY"].ToString()) - Convert.ToDecimal(dtSummaryByJO.Rows[i]["MIN_SCAN_PACK"].ToString()))) * -1;
                    }

                    ShowJODetail(dtSummaryByJO);

                    //dtSummaryByJO.Columns.Remove("COMBINE_JO_NO");
                    //dtSummaryByJO.Columns.Remove("COLOR_CODE");
                    //dtSummaryByJO.Columns.Remove("SIZE_CODE1");
                    //dtSummaryByJO.Columns.Remove("SIZE_CODE2");
                    //dtSummaryByJO.Columns.Remove("SEQ1");
                    //dtSummaryByJO.Columns.Remove("SEQ2");
                    //gvList.DataSource = dtSummaryByJO;
                    //gvList.DataBind();
                    //gvList.Visible = true;

                    if (DDSummaryBy.SelectedValue == "ByLOT" || DDSummaryBy.SelectedValue == "ByBPO")
                    {
                        if (dtCombineJo.Rows.Count > 0)
                        {
                            DataTable dtByLot = GetReduceQuantityByLOT(DT.Rows[a][1].ToString(), FactoryCD);
                           
                            DataTable dtByBPO = new DataTable();
                            if (dtByLot.Rows.Count > 0)
                            {
                                foreach (DataColumn col in dtByLot.Columns)
                                {
                                    col.ReadOnly = false;
                                }
                                DataTable dtScanPackQtyJO = GetScanPackQtyByJO(JO_String, FactoryCD);
                                string v = string.Empty;
                                string v1 = string.Empty;
                                string v2 = string.Empty;
                                if (dtScanPackQtyJO != null)
                                {
                                    foreach (DataRow row in dtScanPackQtyJO.Rows)
                                    {
                                        v = row["GOColor"].ToString();
                                        v1 = row["GOSize"].ToString();
                                        v2 = row["CutNo"].ToString();

                                        DataRow[] dr = dtByLot.Select("COLOR_CODE='" + v + "' AND SIZE_CODE='" + v1 + "' AND ORIGINAL_JO_NO='" + v2 + "'");
                                        if (dr.Length > 0)
                                            dr[0]["SCAN_QTY"] = row["PacTotal"];
                                    }
                                    for (int i = 0; i < dtByLot.Rows.Count; i++)
                                    {
                                        //dtByLot.Rows[i]["PERCENTAGE"] = (Convert.ToInt32(dtByLot.Rows[i]["SCAN_QTY"].ToString()) > 0) ? (Math.Round(((Convert.ToDecimal(dtByLot.Rows[i]["SCAN_QTY"].ToString()) - Convert.ToDecimal(dtByLot.Rows[i]["ORDER_QTY"].ToString())) / Convert.ToDecimal(dtByLot.Rows[i]["ORDER_QTY"].ToString()) * 100), 2) + "%") : "0";
                                        dtByLot.Rows[i]["PERCENTAGE"] = dtByLot.Rows[i]["SCAN_QTY"].ToInt("r") > 0 ? (dtByLot.Rows[i]["ORDER_QTY"].ToDouble() == 0 ? 0 : Math.Round((dtByLot.Rows[i]["SCAN_QTY"].ToInt("r") - dtByLot.Rows[i]["ORDER_QTY"].ToDouble()) / dtByLot.Rows[i]["ORDER_QTY"].ToDouble() * 100, 2)) : 0;
                                        dtByLot.Rows[i]["DIFF"] = (Convert.ToInt32(dtByLot.Rows[i]["SCAN_QTY"].ToString()) + Convert.ToInt32(dtByLot.Rows[i]["LEFTOVER_A"].ToString()) - Convert.ToInt32(dtByLot.Rows[i]["ACTUAL_QTY"].ToString()));
                                        dtByLot.Rows[i]["STILL_CAN_PACK"] = Math.Floor(Convert.ToDecimal(dtByLot.Rows[i]["MAX_SCAN_PACK"].ToString()) - Convert.ToDecimal(dtByLot.Rows[i]["SCAN_QTY"].ToString()));
                                    }
                                }
                                if (DDSummaryBy.SelectedValue == "ByBPO")
                                {
                                    dtByBPO = GetReduceQuantityByBPO(DT.Rows[a][1].ToString(), FactoryCD);
                                    if (dtByBPO.Rows.Count > 0)
                                    {
                                        foreach (DataColumn col in dtByBPO.Columns)
                                        {
                                            col.ReadOnly = false;
                                        }

                                        DataTable dtScanPackQtyJO_byBPO = GetScanPackQtyByBPO(JO_String, FactoryCD);
                                        string v_byBPO = string.Empty;
                                        string v1_byBPO = string.Empty;
                                        string v2_byBPO = string.Empty;
                                        string v3_byBPO = string.Empty;
                                        if (dtScanPackQtyJO_byBPO != null)
                                        {
                                            foreach (DataRow row in dtScanPackQtyJO_byBPO.Rows)
                                            {
                                                v_byBPO = row["GOColor"].ToString();
                                                v1_byBPO = row["GOSize"].ToString();
                                                v2_byBPO = row["CutNo"].ToString();
                                                v3_byBPO = row["OrderNo"].ToString();

                                                DataRow[] dr = dtByBPO.Select("COLOR_CODE='" + v_byBPO + "' AND SIZE_CODE='" + v1_byBPO + "' AND ORIGINAL_JO_NO='" + v2_byBPO + "' and BUYER_PO='" + v3_byBPO + "'");
                                                if (dr.Length > 0)
                                                    dr[0]["SCAN_QTY"] = row["PacTotal"];
                                            }
                                        }
                                        for (int i = 0; i < dtByBPO.Rows.Count; i++)
                                        {
                                            //dtByBPO.Rows[i]["PERCENTAGE"] = (Convert.ToInt32(dtByBPO.Rows[i]["SCAN_QTY"].ToString()) > 0) ? (Math.Round(((Convert.ToDecimal(dtByBPO.Rows[i]["SCAN_QTY"].ToString()) - Convert.ToDecimal(dtByBPO.Rows[i]["ORDER_QTY"].ToString())) / Convert.ToDecimal(dtByBPO.Rows[i]["ORDER_QTY"].ToString()) * 100), 2) + "%") : "0";
                                            dtByBPO.Rows[i]["PERCENTAGE"] = dtByBPO.Rows[i]["SCAN_QTY"].ToInt("r") > 0 ? (dtByBPO.Rows[i]["ORDER_QTY"].ToDouble() == 0 ? 0 : Math.Round((dtByBPO.Rows[i]["SCAN_QTY"].ToInt("r") - dtByBPO.Rows[i]["ORDER_QTY"].ToDouble()) / dtByBPO.Rows[i]["ORDER_QTY"].ToDouble() * 100, 2)) : 0;
                                            dtByBPO.Rows[i]["STILL_CAN_PACK"] = dtByBPO.Rows[i]["MAX_SCAN_PACK"].ToInt("t") - dtByBPO.Rows[i]["SCAN_QTY"].ToInt("r");
                                            //Math.Floor(Convert.ToDecimal(dtByBPO.Rows[i]["MAX_SCAN_PACK"].ToString()) - Convert.ToDecimal(dtByBPO.Rows[i]["SCAN_QTY"].ToString()));
                                        }
                                    }
                                }
                                DataTable drLot = dtByLot.Clone();
                                DataTable drBPO = dtByBPO.Clone();
                                for (int dis = 0; dis < dtCombineJo.Rows.Count; dis++)
                                {
                                    drLot.Clear();
                                    drBPO.Clear();

                                    //DataTable drLot = dtByLot.Select("COMBINE_JO_NO='" + dtCombineJo.Rows[dis]["COMBINE_JO_NO"].ToString() + "' AND ORIGINAL_JO_NO='" + dtCombineJo.Rows[dis]["ORIGINAL_JO_NO"].ToString() + "'");
                                    foreach (DataRow dr in dtByLot.Select("COMBINE_JO_NO='" + dtCombineJo.Rows[dis]["COMBINE_JO_NO"].ToString() + "' AND ORIGINAL_JO_NO='" + dtCombineJo.Rows[dis]["ORIGINAL_JO_NO"].ToString() + "'"))
                                    {
                                        drLot.ImportRow(dr);
                                    }
                                    if (dtCombineJo.Rows.Count > 1)
                                        ShowJOByLot(drLot);
                                    if (DDSummaryBy.SelectedValue == "ByBPO")
                                    {
                                        foreach (DataRow dr1 in dtByBPO.Select("COMBINE_JO_NO='" + dtCombineJo.Rows[dis]["COMBINE_JO_NO"] + "' AND ORIGINAL_JO_NO='" + dtCombineJo.Rows[dis]["ORIGINAL_JO_NO"] + "'"))
                                        {
                                            drBPO.ImportRow(dr1);
                                        }
                                        ShowJOByBPO(drBPO);
                                    }
                                }
                            }
                        }
                    }
                    //if (DDSummaryBy.SelectedValue == "ByBPO")
                    //{
                    //    DataTable dtByBPO = new DataTable();
                    //    dtByBPO = GetReduceQuantityByBPO(DT.Rows[a][1].ToString(), FactoryCD);
                    //    if (dtByBPO.Rows.Count > 0)
                    //    {
                    //        foreach (DataColumn col in dtByBPO.Columns)
                    //        {
                    //            col.ReadOnly = false;
                    //        }
                    //        DataTable dtScanPackQtyJO_byBPO = GetScanPackQtyByBPO(JO_String, FactoryCD);
                    //        string v_byBPO = string.Empty;
                    //        string v1_byBPO = string.Empty;
                    //        string v2_byBPO = string.Empty;
                    //        string v3_byBPO = string.Empty;
                    //        if (dtScanPackQtyJO_byBPO != null)
                    //        {
                    //            foreach (DataRow row in dtScanPackQtyJO_byBPO.Rows)
                    //            {
                    //                v_byBPO = row["GOColor"].ToString();
                    //                v1_byBPO = row["GOSize"].ToString();
                    //                v2_byBPO = row["CutNo"].ToString();
                    //                v3_byBPO = row["OrderNo"].ToString();

                    //                DataRow[] dr = dtByBPO.Select("COLOR_CODE='" + v_byBPO + "' AND SIZE_CODE='" + v1_byBPO + "' AND ORIGINAL_JO_NO='" + v2_byBPO + "' and BUYER_PO='" + v3_byBPO+"'");
                    //                if (dr.Length > 0)
                    //                    dr[0]["SCAN_QTY"] = row["PacTotal"];
                    //            }
                    //        }

                    //        for (int i = 0; i < dtByBPO.Rows.Count; i++)
                    //        {
                    //            dtByBPO.Rows[i]["PERCENTAGE"] = (Convert.ToInt32(dtByBPO.Rows[i]["SCAN_QTY"].ToString()) > 0) ? (Math.Round(((Convert.ToDecimal(dtByBPO.Rows[i]["SCAN_QTY"].ToString()) - Convert.ToDecimal(dtByBPO.Rows[i]["ORDER_QTY"].ToString())) / Convert.ToDecimal(dtByBPO.Rows[i]["ORDER_QTY"].ToString()) * 100), 2) + "%") : "0";
                    //            dtByBPO.Rows[i]["STILL_CAN_PACK"] = dtByBPO.Rows[i]["MAX_SCAN_PACK"].ToInt("t") - dtByBPO.Rows[i]["SCAN_QTY"].ToInt("r");
                    //                //Math.Floor(Convert.ToDecimal(dtByBPO.Rows[i]["MAX_SCAN_PACK"].ToString()) - Convert.ToDecimal(dtByBPO.Rows[i]["SCAN_QTY"].ToString()));
                    //        }
                    //        DataTable drBPO = dtByBPO.Clone();
                    //        for (int dis = 0; dis < dtCombineJo.Rows.Count; dis++)
                    //        {
                               
                    //            drBPO.Clear();
                    //             //DataRow[] drBPO = dtByBPO.Select("COMBINE_JO_NO='" + dtCombineJo.Rows[dis]["COMBINE_JO_NO"].ToString() + "' AND ORIGINAL_JO_NO='" + dtCombineJo.Rows[dis]["ORIGINAL_JO_NO"].ToString() + "'");
                    //            foreach (DataRow dr1 in dtByBPO.Select("COMBINE_JO_NO='" + dtCombineJo.Rows[dis]["COMBINE_JO_NO"] + "' AND ORIGINAL_JO_NO='" + dtCombineJo.Rows[dis]["ORIGINAL_JO_NO"] + "'"))
                    //            {
                    //                drBPO.ImportRow(dr1);
                    //            }
                    //            ShowJOByBPO(drBPO);
                                
                    //        }

                    //    }
                    //}
                    

                    //if (DDSummaryBy.SelectedValue == "ByLOT" || DDSummaryBy.SelectedValue == "ByBPO")
                    //{
                    //    DataTable dtByLot = GetReduceQuantityByLOT(DT.Rows[a][1].ToString(), FactoryCD);
                    //    DataTable dtByBPO = new DataTable();

                    //    if (dtByLot.Rows.Count > 0)
                    //    {

                    //        foreach (DataColumn col in dtByLot.Columns)
                    //        {
                    //            col.ReadOnly = false;
                    //        }

                    //        DataTable dtScanPackQtyJO = GetScanPackQtyByJO(JO_String, FactoryCD);

                    //        string v = string.Empty;
                    //        string v1 = string.Empty;
                    //        string v2 = string.Empty;

                    //        if (dtByLot.Rows.Count > 0)
                    //        {
                    //            if (dtScanPackQtyJO != null)
                    //            {
                    //                foreach (DataRow row in dtScanPackQtyJO.Rows)
                    //                {
                    //                    v = row["GOColor"].ToString();
                    //                    v1 = row["GOSize"].ToString();
                    //                    v2 = row["CutNo"].ToString();

                    //                    DataRow[] dr = dtByLot.Select("COLOR_CODE='" + v + "' AND SIZE_CODE='" + v1 + "' AND ORIGINAL_JO_NO='" + v2 + "'");
                    //                    if(dr.Length>0)
                    //                     dr[0]["SCAN_QTY"] = row["PacTotal"];
                    //                }
                    //            }

                    //            for (int i = 0; i < dtByLot.Rows.Count; i++)
                    //            {
                    //                dtByLot.Rows[i]["PERCENTAGE"] = (Convert.ToInt32(dtByLot.Rows[i]["SCAN_QTY"].ToString()) > 0) ? (Math.Round(((Convert.ToDecimal(dtByLot.Rows[i]["SCAN_QTY"].ToString()) - Convert.ToDecimal(dtByLot.Rows[i]["ORDER_QTY"].ToString())) / Convert.ToDecimal(dtByLot.Rows[i]["ORDER_QTY"].ToString()) * 100), 2) + "%") : "0";
                    //                dtByLot.Rows[i]["DIFF"] = (Convert.ToInt32(dtByLot.Rows[i]["SCAN_QTY"].ToString()) + Convert.ToInt32(dtByLot.Rows[i]["LEFTOVER_A"].ToString()) - Convert.ToInt32(dtByLot.Rows[i]["ACTUAL_QTY"].ToString()));
                    //                dtByLot.Rows[i]["STILL_CAN_PACK"] = Math.Floor(Convert.ToDecimal(dtByLot.Rows[i]["MAX_SCAN_PACK"].ToString()) - Convert.ToDecimal(dtByLot.Rows[i]["SCAN_QTY"].ToString()));
                    //            }
                    //        }

                    //        if (DDSummaryBy.SelectedValue == "ByBPO")
                    //        {
                    //            dtByBPO = GetReduceQuantityByBPO(DT.Rows[a][1].ToString(), FactoryCD);

                    //            if (dtByBPO.Rows.Count > 0)
                    //            {
                    //                foreach (DataColumn col in dtByBPO.Columns)
                    //                {
                    //                    col.ReadOnly = false;
                    //                }

                    //                DataTable dtScanPackQtyJO_byBPO = GetScanPackQtyByJO(JO_String, FactoryCD);

                    //                string v_byBPO = string.Empty;
                    //                string v1_byBPO = string.Empty;
                    //                string v2_byBPO = string.Empty;
                    //                if (dtScanPackQtyJO != null)
                    //                {
                    //                    foreach (DataRow row in dtScanPackQtyJO.Rows)
                    //                    {
                    //                        v_byBPO = row["GOColor"].ToString();
                    //                        v1_byBPO = row["GOSize"].ToString();
                    //                        v2_byBPO = row["CutNo"].ToString();

                    //                        DataRow[] dr = dtByBPO.Select("COLOR_CODE='" + v_byBPO + "' AND SIZE_CODE='" + v1_byBPO + "' AND ORIGINAL_JO_NO='" + v2_byBPO + "'");
                    //                        if(dr.Length>0)
                    //                         dr[0]["SCAN_QTY"] = row["PacTotal"];
                    //                    }
                    //                }

                    //                for (int i = 0; i < dtByBPO.Rows.Count; i++)
                    //                {
                    //                    dtByBPO.Rows[i]["PERCENTAGE"] = (Convert.ToInt32(dtByBPO.Rows[i]["SCAN_QTY"].ToString()) > 0) ? (Math.Round(((Convert.ToDecimal(dtByBPO.Rows[i]["SCAN_QTY"].ToString()) - Convert.ToDecimal(dtByBPO.Rows[i]["ORDER_QTY"].ToString())) / Convert.ToDecimal(dtByBPO.Rows[i]["ORDER_QTY"].ToString()) * 100), 2) + "%") : "0";
                    //                    dtByBPO.Rows[i]["STILL_CAN_PACK"] = Math.Floor(Convert.ToDecimal(dtByBPO.Rows[i]["MAX_SCAN_PACK"].ToString()) - Convert.ToDecimal(dtByBPO.Rows[i]["SCAN_QTY"].ToString()));
                    //                }
                    //            }
                    //        }

                    //        DataTable drLot = dtByLot.Clone();
                    //        DataTable drBPO = dtByBPO.Clone();

                    //        //Display JO1, BPO1a...JO2, BPO2a, BPO2b...
                    //        for (int dis = 0; dis < dtCombineJo.Rows.Count; dis++)
                    //        {
                    //            drLot.Clear();
                    //            drBPO.Clear();
                    //            //DataTable drLot = dtByLot.Select("COMBINE_JO_NO='" + dtCombineJo.Rows[dis]["COMBINE_JO_NO"].ToString() + "' AND ORIGINAL_JO_NO='" + dtCombineJo.Rows[dis]["ORIGINAL_JO_NO"].ToString() + "'");
                    //            foreach (DataRow dr in dtByLot.Select("COMBINE_JO_NO='" + dtCombineJo.Rows[dis]["COMBINE_JO_NO"].ToString() + "' AND ORIGINAL_JO_NO='" + dtCombineJo.Rows[dis]["ORIGINAL_JO_NO"].ToString() + "'"))
                    //            {
                    //                drLot.ImportRow(dr);
                    //            }
                    //            ShowJOByLot(drLot);

                    //            if (DDSummaryBy.SelectedValue == "ByBPO")
                    //            {
                    //                //DataRow[] drBPO = dtByBPO.Select("COMBINE_JO_NO='" + dtCombineJo.Rows[dis]["COMBINE_JO_NO"].ToString() + "' AND ORIGINAL_JO_NO='" + dtCombineJo.Rows[dis]["ORIGINAL_JO_NO"].ToString() + "'");
                    //                foreach (DataRow dr1 in dtByBPO.Select("COMBINE_JO_NO='" + dtCombineJo.Rows[dis]["COMBINE_JO_NO"].ToString() + "' AND ORIGINAL_JO_NO='" + dtCombineJo.Rows[dis]["ORIGINAL_JO_NO"].ToString() + "'"))
                    //                {
                    //                    drBPO.ImportRow(dr1);
                    //                }
                    //                ShowJOByBPO(drBPO);
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            ApproveJOLabel.Text = ApproveJO;
            ApproveScanPackLabel.Text = ApproveScanPack;
            ApproveScanPackPerLabel.Text = ApproveScanPackPer;
        }
        else
        {
            this.gvDetail.InnerHtml = "<table width='100%'  style='color:Red; font-size:large'><tr><td align='center'><b>No Data</b></td></tr></table>";
     
        }
    }

    #region add by ming
    private void Retsetvar()
    {
         showlot = false;
         showpbo = false;
         CombineJo = true;
         percent_Short_Allowed = 0;
         percent_Over_Allowed = 0;
    }
    bool showlot = false;
    bool showpbo = false;
    bool CombineJo = true;
    public void SetQuery1()
    {
        string JO = txtJoNo.Text;
        string FactoryCD = ddlFactory.SelectedValue.ToString();
        DataTable DT = MESComment.MesRpt.GetJO(JO, "", FactoryCD);
        if (DT.Rows.Count > 0)
        {
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                Retsetvar();
                string JoString = GetOriginalString(DT.Rows[i][1].ToString());
                string level = GetLevel();

                GetHeader(DT.Rows[i][1].ToString(), JoString, FactoryCD);
                //ShowJOHeader(GetHeader(DT.Rows[i][1].ToString(), FactoryCD), GetTotalPackQty(packdt));
                
                //DataTable detaildt = GetDetailQty(DT.Rows[i][1].ToString(), FactoryCD, level);
                DataTable packdt = GetScanPackQty(JoString, FactoryCD, level);
                //EnbleEditColumn(detaildt);
              
                //CombineScanPackQty(detaildt, packdt, level);
                GetDetail(DT.Rows[i][1].ToString(), FactoryCD, packdt);

                GetDetailLot(packdt, DT.Rows[i][1].ToString(), FactoryCD);
              

            }
        }
        else
        {
            this.gvDetail.InnerHtml = "<table width='100%'  style='color:Red; font-size:large'><tr><td align='center'><b>No Data</b></td></tr></table>";

        }
    }
    private string GetLevel()
    {
        string level = "JO";
          
        switch (DDSummaryBy.SelectedValue)
        {
            case "ByJO":
                level="JO";
                break;
            case "ByLOT":
                 showlot = true;
                 level = "LOT";
                 if (!CombineJo)
                 {
                     level = "JO";
                 }
                 break;
            case "ByBPO":
                showpbo = true;
                showlot = true;
                level="BPO";
                break;

        }
        if (!CombineJo)
        {
            showlot = false;
        }
        return level;
    }
    private void GetHeader(string JoNo, string JO_String, string FactoryCD)
    {
        DataTable dtHeader = GetReduceQuantityHeadByJO(JoNo, FactoryCD);
        DataTable dtEndDate = GetScanPackEndDateByJO(JO_String, FactoryCD);
        SetAllowed(dtHeader.Rows[0]["ALLOWED_PERCENT"].ToString());
        ShowJOHeader(dtHeader, dtEndDate);
    }
    private void GetDetail(string JoNo, string FactoryCD, DataTable scanpack)
    {
        DataTable detail = GetDetailByJO(JoNo, FactoryCD);
        CombineScanPackQty(detail, scanpack, "JO");
        for (int i = 0; i < detail.Rows.Count; i++)
        {
            detail.Rows[i]["COLOR_DESC"] = detail.Rows[i]["COLOR_CODE"].ToString() + " (" + detail.Rows[i]["COLOR_DESC"].ToString() + ")";
            detail.Rows[i]["PERCENTAGE"] = (detail.Rows[i]["ORDER_QTY"].ToDouble() == 0 ? 0 : Math.Round((detail.Rows[i]["SCAN_QTY"].ToInt("r") - detail.Rows[i]["ORDER_QTY"].ToDouble()) / detail.Rows[i]["ORDER_QTY"].ToDouble() * 100, 2));
            detail.Rows[i]["DIFF"] = detail.Rows[i]["SCAN_QTY"].ToInt("c") + detail.Rows[i]["LEFTOVER_A"].ToInt("c") + detail.Rows[i]["GRADE_B"].ToInt("c") +
                                                       detail.Rows[i]["DISCREPANCY_QTY"].ToInt("c") - detail.Rows[i]["ACTUAL_QTY"].ToInt("c");
            detail.Rows[i]["MAX_SCAN_PACK"] = Math.Round(detail.Rows[i]["ORDER_QTY"].ToDouble() * (percent_Over_Allowed + 100) / 100, 2);
            detail.Rows[i]["MIN_SCAN_PACK"] = Math.Round(detail.Rows[i]["ORDER_QTY"].ToDouble() * (100 - percent_Short_Allowed) / 100, 2);
            detail.Rows[i]["AVAILABLE_BALANCE_SCAN_PACK_QTY"] = Math.Floor((Convert.ToDecimal(detail.Rows[i]["MAX_SCAN_PACK"].ToString())) - Convert.ToDecimal(detail.Rows[i]["SCAN_QTY"].ToString()));
            detail.Rows[i]["OVER_PACK"] = Math.Abs(Convert.ToDecimal(detail.Rows[i]["SCAN_QTY"].ToString()) - Convert.ToDecimal(detail.Rows[i]["MAX_SCAN_PACK"].ToString()));
            detail.Rows[i]["SHORT_PACK"] = (Math.Abs(Convert.ToDecimal(detail.Rows[i]["SCAN_QTY"].ToString()) - Convert.ToDecimal(detail.Rows[i]["MIN_SCAN_PACK"].ToString()))) * -1;
        }
        ShowJODetail(detail);
    }
    private void GetDetailLot(DataTable dt, string JoNo, string FactoryCD)
    {
        if (showlot)
        {
            DataTable oridetaildt = GetDetailQty(JoNo, FactoryCD, "LOT");
            CombineScanPackQty(oridetaildt, dt, "LOT");
            Dictionary<string, DataTable> oridetail = GetDetailLotSum(oridetaildt);
            DataTable detail = null;

            foreach (string key in oridetail.Keys)//key=JO
            {
                detail = oridetail[key];
                SetAllowed(detail.Rows[0]["ALLOWED_PERCENT"].ToString());
                for (int i = 0; i < detail.Rows.Count; i++)
                {


                    detail.Rows[i]["PERCENTAGE"] = (detail.Rows[i]["ORDER_QTY"].ToDouble() == 0 ? 0 : Math.Round((detail.Rows[i]["SCAN_QTY"].ToInt("r") - detail.Rows[i]["ORDER_QTY"].ToDouble()) / detail.Rows[i]["ORDER_QTY"].ToDouble() * 100, 2));
                    detail.Rows[i]["DIFF"] = (Convert.ToInt32(detail.Rows[i]["SCAN_QTY"].ToString()) + Convert.ToInt32(detail.Rows[i]["LEFTOVER_A"].ToString()) - Convert.ToInt32(detail.Rows[i]["ACTUAL_QTY"].ToString()));
                    detail.Rows[i]["MAX_SCAN_PACK"] = Math.Round(detail.Rows[i]["ORDER_QTY"].ToDouble() * (percent_Over_Allowed + 100) / 100, 2);
                    detail.Rows[i]["MIN_SCAN_PACK"] = Math.Round(detail.Rows[i]["ORDER_QTY"].ToDouble() * (100 - percent_Short_Allowed) / 100, 2);
                    //detail.Rows[i]["AVAILABLE_BALANCE_SCAN_PACK_QTY"] = Math.Floor((Convert.ToDecimal(detail.Rows[i]["MAX_SCAN_PACK"].ToString())) - Convert.ToDecimal(detail.Rows[i]["SCAN_QTY"].ToString()));
                    //detail.Rows[i]["OVER_PACK"] = Math.Abs(Convert.ToDecimal(detail.Rows[i]["SCAN_QTY"].ToString()) - Convert.ToDecimal(detail.Rows[i]["MAX_SCAN_PACK"].ToString()));
                    //detail.Rows[i]["SHORT_PACK"] = (Math.Abs(Convert.ToDecimal(detail.Rows[i]["SCAN_QTY"].ToString()) - Convert.ToDecimal(detail.Rows[i]["MIN_SCAN_PACK"].ToString()))) * -1;
                    detail.Rows[i]["STILL_CAN_PACK"] = Math.Floor(Convert.ToDecimal(detail.Rows[i]["MAX_SCAN_PACK"].ToString()) - Convert.ToDecimal(detail.Rows[i]["SCAN_QTY"].ToString()));
                }
                ShowJOByLot(detail);
                if (showpbo)
                    GetDetailLotBPO(dt, key, FactoryCD);
            }
        }
        else
        {
            if (showpbo)
                GetDetailLotBPO(dt, JoNo, FactoryCD);
        }

        
       
    }
    private void GetDetailLotBPO(DataTable dt, string JO, string FactoryCD)
    {
        DataTable dtbpo = GetDetailQty(JO, FactoryCD, "BPO");
        CombineScanPackQty(dtbpo, dt, "BPO");
        DataRow[] drs = dtbpo.Select(string.Format("ORIGINAL_JO_NO='{0}'", JO));
        
        for (int i = 0; i < drs.Length; i++)
        {
            drs[i]["MAX_SCAN_PACK"] = Math.Round(drs[i]["ORDER_QTY"].ToDouble() * (percent_Over_Allowed + 100) / 100, 2);
            drs[i]["MIN_SCAN_PACK"] = Math.Round(drs[i]["ORDER_QTY"].ToDouble() * (100 - percent_Short_Allowed) / 100, 2);
            drs[i]["PERCENTAGE"] = (drs[i]["ORDER_QTY"].ToDouble() == 0 ? 0 : Math.Round((drs[i]["SCAN_QTY"].ToInt("r") - drs[i]["ORDER_QTY"].ToDouble()) / drs[i]["ORDER_QTY"].ToDouble() * 100, 2));
            drs[i]["STILL_CAN_PACK"] = drs[i]["MAX_SCAN_PACK"].ToInt("t") - drs[i]["SCAN_QTY"].ToInt("r");
            //dtbpo.Rows.Add(drs[i].ItemArray);
        }
        ShowJOByBPO(dtbpo);
    }
    
    private DataTable GetScanPackQty(string JO_String, string FactoryCD, string level)
    {
        DataTable dt = new DataTable();
        switch (level)
        {
            case "LOT":
                dt = GetScanPackQtyByJO(JO_String, FactoryCD);
                break;
            case "BPO":
                dt = GetScanPackQtyByBPO(JO_String, FactoryCD);
                break;
            default:
                dt = GetScanPackQty(JO_String, FactoryCD);
                break;
        }
        return dt;
    }
    private DataTable GetDetailQty(string JoNo, string FactoryCD, string level)
    {
        DataTable dt = new DataTable();
        switch (level)
        {
            case "LOT":
                dt = GetReduceQuantityByLOT(JoNo, FactoryCD);
                break;
            case "BPO":
                dt = GetReduceQuantityByBPO(JoNo, FactoryCD);
                break;
        }
        return dt;
    }
    private void CombineScanPackQty(DataTable detaildt, DataTable packdt, string level)
    {
        EnbleEditColumn(detaildt);
       System.Text.StringBuilder sbcondition=new System.Text.StringBuilder();

        for (int i = 0; i < detaildt.Rows.Count; i++)
        {
            sbcondition.Remove(0, sbcondition.Length);
            if (level == "JO")
            {
                sbcondition.AppendFormat("GOColor='{0}' and GOSize='{1}'", detaildt.Rows[i]["COLOR_CODE"], detaildt.Rows[i]["SIZE_CODE"]);
            }
            else if (level == "LOT")
            {
                sbcondition.AppendFormat("CutNo='{0}' and GOColor='{1}' and GOSize='{2}'", detaildt.Rows[i]["ORIGINAL_JO_NO"], detaildt.Rows[i]["COLOR_CODE"], detaildt.Rows[i]["SIZE_CODE"]);
            }
            else
            {
                sbcondition.AppendFormat("CutNo='{0}' and GOColor='{1}' and GOSize='{2}' and OrderNo='{3}'", detaildt.Rows[i]["ORIGINAL_JO_NO"], detaildt.Rows[i]["COLOR_CODE"],
                    detaildt.Rows[i]["SIZE_CODE"], detaildt.Rows[i]["BUYER_PO"]);
            }
            DataRow[] drs = packdt.Select(sbcondition.ToString());
            if (drs.Length > 0)
            {
                double total = 0;
                foreach (DataRow dr in drs)
                {
                    total += dr["PacTotal"].ToDouble();
                }
                detaildt.Rows[i]["SCAN_QTY"] = total;
            }
        }

        //foreach (DataRow row in detaildt.Rows)
        //{
        //    v_byBPO=row["GOColor"].ToString();
        //    v_byBPO=row["GOColor"].ToString();
        //    v_byBPO=row["GOColor"].ToString();
        //    v_byBPO=row["GOColor"].ToString();
        //}
        

        //if (dtScanPackQtyJO_byBPO != null)
        //{
        //    foreach (DataRow row in dtScanPackQtyJO_byBPO.Rows)
        //    {
        //        v_byBPO = row["GOColor"].ToString();
        //        v1_byBPO = row["GOSize"].ToString();
        //        v2_byBPO = row["CutNo"].ToString();
        //        v3_byBPO = row["OrderNo"].ToString();

        //        DataRow[] dr = dtByBPO.Select("COLOR_CODE='" + v_byBPO + "' AND SIZE_CODE='" + v1_byBPO + "' AND ORIGINAL_JO_NO='" + v2_byBPO + "' and BUYER_PO='" + v3_byBPO + "'");
        //        if (dr.Length > 0)
        //            dr[0]["SCAN_QTY"] = row["PacTotal"];
        //    }
        //}
    }
    private string GetOriginalString(string JoNo)
    {
        DataTable dtCombineJo = GetOriginalJo(JoNo);

        if (dtCombineJo.Rows.Count == 0)
        {
            CombineJo = false;
            dtCombineJo.Rows.Add(JoNo, JoNo);
        }

        string JO_String = JoNo;
        if (dtCombineJo.Rows.Count > 0)
        {
            JO_String = "";
            for (int i = 0; i < dtCombineJo.Rows.Count; i++)
            {
                JO_String += "'" + dtCombineJo.Rows[i]["ORIGINAL_JO_NO"].ToString() + "'";

                if (dtCombineJo.Rows.Count - 1 != i)
                {
                    JO_String += ",";
                }
            }
           
        }
        return JO_String;
    }
    private DataTable GetTotalPackQty(DataTable packdt)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("CloseDate");
        dt.Columns.Add("PacQty");
        double totalqty = 0;
        for (int i = 0; i < packdt.Rows.Count; i++)
        {
            totalqty += packdt.Rows[i]["PacQty"].ToDouble();
        }
        if (packdt.Rows.Count > 0)
        {

            dt.Rows.Add(packdt.Rows[0]["CloseDate"], totalqty);
        }
        return dt;
       
    }
    private void EnbleEditColumn(DataTable detaildt)
    {
        foreach (DataColumn c in detaildt.Columns)
        {
            c.ReadOnly = false;
        }
    }

    //private DataTable GetDetailSum(DataTable dt)
    //{
    //    Dictionary<string, DataRow> sumrow = new Dictionary<string, DataRow>();
    //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
    //    DataTable sumdt = dt.Clone();
    //    DataRow selectdr;
    //    foreach (DataRow dr in dt.Rows)
    //    {
    //        sb.Remove(0, sb.Length);
    //        sb.AppendFormat("{0}_", dr["COMBINE_JO_NO"]);
    //        sb.AppendFormat("{0}_", dr["COLOR_CODE"]);
    //        sb.AppendFormat("{0}_", dr["COLOR_DESC"]);
    //        sb.AppendFormat("{0}_", dr["SIZE_CODE"]);
    //        sb.AppendFormat("{0}_", dr["SIZE_CODE1"]);
    //        sb.AppendFormat("{0}_", dr["SIZE_CODE2"]);
    //        sb.AppendFormat("{0}_", dr["SEQ1"]);
    //        sb.AppendFormat("{0}_", dr["SEQ2"]);
    //        if (sumrow.ContainsKey(sb.ToString()))
    //        {
    //            selectdr = sumrow[sb.ToString()];
    //            selectdr["ORDER_QTY"] = selectdr["ORDER_QTY"].ToDouble() + dr["ORDER_QTY"].ToDouble();
    //            selectdr["ACTUAL_QTY"] = selectdr["ACTUAL_QTY"].ToDouble() + dr["ACTUAL_QTY"].ToDouble();
    //            selectdr["SCAN_QTY"] = selectdr["SCAN_QTY"].ToDouble() + dr["SCAN_QTY"].ToDouble();
    //            selectdr["LEFTOVER_A"] = selectdr["LEFTOVER_A"].ToDouble() + dr["LEFTOVER_A"].ToDouble();
    //            selectdr["GRADE_B"] = selectdr["GRADE_B"].ToDouble() + dr["GRADE_B"].ToDouble();
    //            selectdr["DISCREPANCY_QTY"] = selectdr["DISCREPANCY_QTY"].ToDouble() + dr["DISCREPANCY_QTY"].ToDouble();
    //        }
    //        else
    //        {
    //            selectdr = sumdt.NewRow();
    //            selectdr.ItemArray = dr.ItemArray;
    //            sumdt.Rows.Add(selectdr);
    //            sumrow.Add(sb.ToString(), selectdr);
    //        }
    //    }
    //    return sumdt;
    //}
    private Dictionary<string, DataTable> GetDetailLotSum(DataTable dt)
    {
        //DataTable dt = GetDetailQty(JoNo, FactoryCD, "LOT");
        Dictionary<string, DataRow> sumrow = new Dictionary<string, DataRow>();

        Dictionary<string, DataTable> oridt = new Dictionary<string, DataTable>();


        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        DataTable sumdt = dt.Clone();
        DataRow selectdr;
        foreach (DataRow dr in dt.Rows)
        {
            if (oridt.ContainsKey(dr["ORIGINAL_JO_NO"].ToString()))
            {
                sumdt = oridt[dr["ORIGINAL_JO_NO"].ToString()];
            }
            else
            {
                 sumdt = dt.Clone();
                 oridt.Add(dr["ORIGINAL_JO_NO"].ToString(), sumdt);
            }
            sb.Remove(0, sb.Length);
            sb.AppendFormat("{0}_", dr["COMBINE_JO_NO"]);
            sb.AppendFormat("{0}_", dr["ORIGINAL_JO_NO"]);
            sb.AppendFormat("{0}_", dr["COLOR_CODE"]);
            sb.AppendFormat("{0}_", dr["COLOR_DESC"]);
            sb.AppendFormat("{0}_", dr["SIZE_CODE"]);
            sb.AppendFormat("{0}_", dr["SIZE_CODE1"]);
            sb.AppendFormat("{0}_", dr["SIZE_CODE2"]);
            sb.AppendFormat("{0}_", dr["SEQ1"]);
            sb.AppendFormat("{0}_", dr["SEQ2"]);
            if (sumrow.ContainsKey(sb.ToString()))
            {
                selectdr = sumrow[sb.ToString()];
                selectdr["ORDER_QTY"] = selectdr["ORDER_QTY"].ToDouble() + dr["ORDER_QTY"].ToDouble();
                selectdr["ACTUAL_QTY"] = selectdr["ACTUAL_QTY"].ToDouble() + dr["ACTUAL_QTY"].ToDouble();
                selectdr["SCAN_QTY"] = selectdr["SCAN_QTY"].ToDouble() + dr["SCAN_QTY"].ToDouble();
                selectdr["LEFTOVER_A"] = selectdr["LEFTOVER_A"].ToDouble() + dr["LEFTOVER_A"].ToDouble();
                //selectdr["GRADE_B"] = selectdr["GRADE_B"].ToDouble() + dr["GRADE_B"].ToDouble();
                //selectdr["DISCREPANCY_QTY"] = selectdr["DISCREPANCY_QTY"].ToDouble() + dr["DISCREPANCY_QTY"].ToDouble();
            }
            else
            {
                selectdr = sumdt.NewRow();
                selectdr.ItemArray = dr.ItemArray;
                sumdt.Rows.Add(selectdr);
                sumrow.Add(sb.ToString(), selectdr);
            }
        }
        return oridt;
    }
    //private DataTable GetDetailBPOSum(DataTable dt)
    //{
    //    Dictionary<string, DataRow> sumrow = new Dictionary<string, DataRow>();
    //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
    //    DataTable sumdt = dt.Clone();
    //    DataRow selectdr;
    //    foreach (DataRow dr in dt.Rows)
    //    {
    //        sb.Remove(0, sb.Length);
    //        sb.AppendFormat("{0}_", dr["COMBINE_JO_NO"]);
    //        sb.AppendFormat("{0}_", dr["ORIGINAL_JO_NO"]);
    //        sb.AppendFormat("{0}_", dr["BUYER_PO"]);
    //        sb.AppendFormat("{0}_", dr["COLOR_CODE"]);
    //        sb.AppendFormat("{0}_", dr["COLOR_DESC"]);
    //        sb.AppendFormat("{0}_", dr["SIZE_CODE"]);
    //        sb.AppendFormat("{0}_", dr["SIZE_CODE1"]);
    //        sb.AppendFormat("{0}_", dr["SIZE_CODE2"]);
    //        sb.AppendFormat("{0}_", dr["SEQ1"]);
    //        sb.AppendFormat("{0}_", dr["SEQ2"]);
    //        if (sumrow.ContainsKey(sb.ToString()))
    //        {
    //            selectdr = sumrow[sb.ToString()];
    //            selectdr["ORDER_QTY"] = selectdr["ORDER_QTY"].ToDouble() + dr["ORDER_QTY"].ToDouble();
    //            selectdr["ACTUAL_QTY"] = selectdr["ACTUAL_QTY"].ToDouble() + dr["ACTUAL_QTY"].ToDouble();
    //            selectdr["SCAN_QTY"] = selectdr["SCAN_QTY"].ToDouble() + dr["SCAN_QTY"].ToDouble();
    //            selectdr["LEFTOVER_A"] = selectdr["LEFTOVER_A"].ToDouble() + dr["LEFTOVER_A"].ToDouble();
    //            selectdr["GRADE_B"] = selectdr["GRADE_B"].ToDouble() + dr["GRADE_B"].ToDouble();
    //            selectdr["DISCREPANCY_QTY"] = selectdr["DISCREPANCY_QTY"].ToDouble() + dr["DISCREPANCY_QTY"].ToDouble();
    //        }
    //        else
    //        {
    //            selectdr = sumdt.NewRow();
    //            selectdr.ItemArray = dr.ItemArray;
    //            sumdt.Rows.Add(selectdr);
    //            sumrow.Add(sb.ToString(), selectdr);
    //        }
    //    }
    //    return sumdt;
    //}

   
    #endregion

    private static DataTable GetReduceQuantityHeadByJO(string JoNo, string FactoryCd)
    {
        string SQL = @" 
                          DECLARE @JO_LOT TABLE
    (
      JO NVARCHAR(20) ,
      LOT_NO NVARCHAR(25) ,
      PERCENT_OVER_ALLOWED NUMERIC(5, 2) ,
      PERCENT_SHORT_ALLOWED NUMERIC(5, 2) ,
      TOTAL_QTY INT
    )
    DECLARE @JOBPO  TABLE 
    (
      JO NVARCHAR(20) ,
      BPO VARCHAR(40) 
    )

  DECLARE @LEFTOVER_REASON TABLE
    (
      JO NVARCHAR(20) ,
      REASON_CD NVARCHAR(20)
    )

  IF EXISTS ( SELECT TOP 1
                        1
              FROM      JO_COMBINE_MAPPING WITH ( NOLOCK )
              WHERE     COMBINE_JO_NO = '" + JoNo + @"' ) 
    BEGIN
        INSERT  INTO @JO_LOT
                SELECT  A.COMBINE_JO_NO AS JO ,
                        B.PO_NO ,
                        --B.BUYER_PO ,
                        B.PERCENT_OVER_ALLOWED ,
                        B.PERCENT_SHORT_ALLOWED ,
                        C.total_qty
                FROM    JO_COMBINE_MAPPING A WITH ( NOLOCK )
                        INNER JOIN SC_LOT B WITH ( NOLOCK ) ON A.ORIGINAL_JO_NO = B.PO_NO
                        INNER JOIN JO_HD C WITH ( NOLOCK ) ON A.ORIGINAL_JO_NO = C.JO_NO
                WHERE   A.COMBINE_JO_NO = '" + JoNo + @"'
          INSERT INTO @JOBPO
                  ( JO, BPO )
          SELECT distinct a.COMBINE_JO_NO,C.BUYER_PO_NO FROM JO_COMBINE_MAPPING A WITH ( NOLOCK )
                        INNER JOIN JO_HD B WITH ( NOLOCK ) ON A.ORIGINAL_JO_NO = B.JO_NO
                        INNER JOIN SCX_LOT_BPO_HD C WITH ( NOLOCK ) ON B.LOT_NO = C.LOT_NO
                                                              AND B.SC_NO = C.SC_NO
          WHERE   A.COMBINE_JO_NO = '" + JoNo + @"'
    END
  ELSE 
    BEGIN
        INSERT  INTO @JO_LOT
                SELECT  A.JO_NO AS JO ,
                        B.PO_NO ,
                        --B.BUYER_PO ,
                        B.PERCENT_OVER_ALLOWED ,
                        B.PERCENT_SHORT_ALLOWED ,
                        A.total_qty
                FROM    JO_HD A WITH ( NOLOCK )
                        INNER JOIN SC_LOT B WITH ( NOLOCK ) ON A.JO_NO = B.PO_NO
                WHERE   A.JO_NO = '" + JoNo + @"'
          INSERT INTO @JOBPO
                  ( JO, BPO )
          SELECT distinct B.JO_NO,C.BUYER_PO_NO from JO_HD B WITH ( NOLOCK ) 
                        INNER JOIN SCX_LOT_BPO_HD C WITH ( NOLOCK ) ON B.LOT_NO = C.LOT_NO
                                                              AND B.SC_NO = C.SC_NO
          WHERE B.JO_NO = '" + JoNo + @"'
    END

                        --SELECT * FROM @JO_LOT
                        
  IF EXISTS ( SELECT TOP 1
                        1
              FROM      JO_COMBINE_MAPPING WITH ( NOLOCK )
              WHERE     COMBINE_JO_NO = '" + JoNo + @"' ) 
    BEGIN
        INSERT  INTO @LEFTOVER_REASON
                SELECT  A.JOB_ORDER_NO AS JO ,
                        A.REASON_CD
                FROM    PRD_GARMENT_TRANSFER_DFT A WITH ( NOLOCK )
                        INNER JOIN PRD_GARMENT_TRANSFER_HD B WITH ( NOLOCK ) ON A.DOC_NO = B.DOC_NO
                WHERE   B.STATUS IN ( 'S', 'C' )
                        AND A.GRADE_CD IN ( 'A', 'B' )
                        AND A.REASON_CD <> ''
                        AND JOB_ORDER_NO IN (
                        SELECT  ORIGINAL_JO_NO
                        FROM    JO_COMBINE_MAPPING WITH ( NOLOCK )
                        WHERE   COMBINE_JO_NO = '" + JoNo + @"' )                     
		                        --AND JOB_ORDER_NO IN ('" + JoNo + @"')
    END
  ELSE 
    BEGIN
        INSERT  INTO @LEFTOVER_REASON
                SELECT  A.JOB_ORDER_NO AS JO ,
                        A.REASON_CD
                FROM    PRD_GARMENT_TRANSFER_DFT A WITH ( NOLOCK )
                        INNER JOIN PRD_GARMENT_TRANSFER_HD B WITH ( NOLOCK ) ON A.DOC_NO = B.DOC_NO
                WHERE   B.STATUS IN ( 'S', 'C' )
                        AND A.GRADE_CD IN ( 'A', 'B' )
                        AND A.REASON_CD <> ''
		                        --AND JOB_ORDER_NO IN (SELECT ORIGINAL_JO_NO FROM JO_COMBINE_MAPPING WITH(NOLOCK) WHERE COMBINE_JO_NO ='16F03977HK01')                     
                        AND JOB_ORDER_NO IN ( '" + JoNo + @"' )
    END

  SELECT DISTINCT
            CUS.SHORT_NAME AS CUSTOMER ,
            SC.SC_NO ,
            JO.JO_NO ,
            LEFT(LOT.LOT_NO, LEN(LOT.LOT_NO) - 1) AS LOT_NO ,
            LEFT(BUYER.BPO, LEN(BUYER.BPO) - 1) AS BPO ,
            SC.STYLE_NO ,
            SC.STYLE_DESC ,
            SC.STYLE_CHN_DESC ,
            SC.SEASON_CD ,
            '-' + CONVERT(NVARCHAR(10), ALLOW.PERCENT_SHORT_ALLOWED) + '/+'
            + CONVERT(NVARCHAR(10), ALLOW.PERCENT_OVER_ALLOWED) + '%' AS ALLOWED_PERCENT ,
            ISNULL(DIS_B.DISCREPANCY_QTY, 0) AS DISCREPANCY_QTY ,
            ISNULL(TRANS_B.DISCREPANCY_QTY, 0) AS GRADE_B ,
            ISNULL(F.ACTUAL_QTY, 0) - ISNULL(G.QTY, 0)
            - ISNULL(DIS_B.DISCREPANCY_QTY, 0)
            - ISNULL(TRANS_B.DISCREPANCY_QTY, 0) - ISNULL(DIS.DISCREPANCY_QTY,0)
            -ISNULL(DIS_B.SAMPLE_QTY, 0) AS MISSING_GARMENT ,
            ISNULL(DIS.DISCREPANCY_QTY, 0) AS Pullout,
            ISNULL(DIS_B.SAMPLE_QTY, 0) AS SAMPLE_QTY,
            ISNULL(LEFT(AB_REASON.REASON_CD, LEN(AB_REASON.REASON_CD) - 1), '') AS REASON_CD ,
            '' AS FNH_DATE ,
            CASE WHEN SUBMIT.SUBMIT_STATUS = 'N' THEN 'UN-APPROVED'
                 ELSE 'APPROVED'
            END AS STATUS
  FROM      JO_HD JO WITH ( NOLOCK )
            INNER JOIN SC_HD SC WITH ( NOLOCK ) ON JO.SC_NO = SC.SC_NO
            INNER JOIN GEN_JOB_ORDER_MASTER SUBMIT WITH ( NOLOCK ) ON JO.JO_NO = SUBMIT.JOB_ORDER_NO
            INNER JOIN GEN_CUSTOMER CUS WITH ( NOLOCK ) ON SC.CUSTOMER_CD = CUS.CUSTOMER_CD
            LEFT JOIN ( SELECT  JO ,
                                ( SELECT    LOT_NO + '; '
                                  FROM      @JO_LOT
                                  WHERE     JO = A.JO
                                FOR
                                  XML PATH('')
                                ) AS LOT_NO
                        FROM    @JO_LOT A
                        GROUP BY JO
                      ) LOT ON JO.JO_NO = LOT.JO
            INNER JOIN ( SELECT JO ,
                                ( SELECT    BPO + '; '
                                  FROM      @JOBPO
                                  WHERE     JO = A.JO
                                FOR
                                  XML PATH('')
                                ) AS BPO
                         FROM   @JO_LOT A
                         GROUP BY JO
                       ) BUYER ON JO.JO_NO = BUYER.JO
            INNER JOIN ( SELECT JO ,
                                CAST(ROUND(( ( SUM(TOTAL_QTY * ( 1
                                                              + PERCENT_OVER_ALLOWED
                                                              / 100 ))
                                               / SUM(TOTAL_QTY) ) - 1 ) * 100,
                                           2) AS DECIMAL(38, 2)) AS PERCENT_OVER_ALLOWED ,
                                CAST(ROUND(( ( SUM(TOTAL_QTY * ( 1
                                                              - PERCENT_SHORT_ALLOWED
                                                              / 100 ))
                                               / SUM(TOTAL_QTY) ) - 1 ) * -100,
                                           2) AS DECIMAL(38, 2)) AS PERCENT_SHORT_ALLOWED
                         FROM   @JO_LOT
                         GROUP BY JO
                       ) ALLOW ON ALLOW.JO = JO.JO_NO
            LEFT JOIN ( SELECT JOB_ORDER_NO,SUM(pull_qty) AS DISCREPANCY_QTY FROM (
                             SELECT c.jo_no AS JOB_ORDER_NO,
                              -sum(ISNULL(c.pullout_qty,0)) AS pull_qty  
                            FROM prd_jo_discrepancy_pullout_hd A (NOLOCK)  
                         INNER JOIN prd_jo_discrepancy_pullout_trx B (NOLOCK) ON a.doc_no=b.doc_no AND a.factory_cd=b.factory_cd  
                              INNER JOIN prd_jo_pullout C (NOLOCK) ON b.trx_id=c.trx_id  
                           AND C.jo_no = '" + JoNo + @"'
     
                           GROUP BY c.jo_no
                            UNION ALL
                           --pull out and discrepancy
                             SELECT b.job_order_no,
                              SUM(ISNULL(b.pullout_qty,0)) AS pullOut_qty
                            FROM prd_jo_discrepancy_pullout_hd A (NOLOCK)  
                         INNER JOIN prd_jo_discrepancy_pullout_trx B (NOLOCK) ON a.doc_no=b.doc_no AND a.factory_cd=b.factory_cd  
                             AND B.JOB_ORDER_NO= '" + JoNo + @"' 
    
                            GROUP BY b.job_order_no)t
                            GROUP BY JOB_ORDER_NO
                                         --PULL OUT/in
                      ) DIS ON DIS.JOB_ORDER_NO = JO.JO_NO
            LEFT JOIN ( SELECT  A.JOB_ORDER_NO ,
                                SUM(CASE WHEN R.QTY_AFFECTION='D' then C.PULLOUT_QTY ELSE 0 end) AS DISCREPANCY_QTY,
                                SUM(CASE WHEN R.QTY_AFFECTION='A' then C.PULLOUT_QTY ELSE 0 end) AS SAMPLE_QTY
                                                FROM    PRD_JO_DISCREPANCY_PULLOUT_TRX A
                                                        WITH ( NOLOCK )
                                                        INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_HD B
                                                        WITH ( NOLOCK ) ON A.DOC_NO = B.DOC_NO
                                                        INNER JOIN PRD_JO_PULLOUT_REASON C
                                                        WITH ( NOLOCK ) ON A.TRX_ID = C.TRX_ID
                                                        INNER JOIN dbo.PRD_REASON_CODE R
                                                        WITH ( NOLOCK ) ON C.REASON_CD = R.REASON_CD
                                                WHERE   A.JOB_ORDER_NO =  '" + JoNo + @"'
                                                        AND B.STATUS = 'C'
                                                        AND C.GRADE_CD ='C'
                                                        AND R.FACTORY_CD ='" + FactoryCd + @"'
                                                        AND R.REASON_TYPE = 'Unaccountable'
                                                        AND R.QTY_AFFECTION in ('D','A')
                                                        AND A.PULLOUT_QTY IS NULL
                                                        AND R.REASON_DESC NOT LIKE '%不见衫%'
                                                GROUP BY A.JOB_ORDER_NO
                                          ----下数,抽办
                      ) DIS_B ON DIS_B.JOB_ORDER_NO = JO.JO_NO
            LEFT JOIN ( SELECT '" + JoNo + @"' AS JOB_ORDER_NO ,
                                                        SUM(A.QTY) AS DISCREPANCY_QTY
                                                FROM    PRD_GARMENT_TRANSFER_DFT A
                                                        WITH ( NOLOCK )
                                                        INNER JOIN PRD_GARMENT_TRANSFER_HD B
                                                        WITH ( NOLOCK ) ON A.DOC_NO = B.DOC_NO
                                                WHERE   A.JOB_ORDER_NO IN (
                                                        SELECT
                                                              LOT_NO
                                                        FROM  @JO_LOT )
                                                        AND B.STATUS = 'C'
                                                        AND A.REASON_CD IS NOT NULL
                                                        AND A.REASON_CD <> ''
                                                        AND A.GRADE_CD = 'B'
                       --grade B
                      ) TRANS_B ON TRANS_B.JOB_ORDER_NO = JO.JO_NO
            LEFT JOIN ( SELECT  JO ,
                                ( SELECT    REASON_CD + '; '
                                  FROM      @LEFTOVER_REASON
                                  WHERE     JO = A.JO
                                FOR
                                  XML PATH('')
                                ) AS REASON_CD
                        FROM    @LEFTOVER_REASON A
                        GROUP BY JO
                      ) AB_REASON ON AB_REASON.JO = JO.JO_NO
            LEFT JOIN (  SELECT  '" + JoNo + @"' AS JOB_ORDER_NO ,
                                                        SUM(A.QTY) AS QTY
                                                FROM    PRD_GARMENT_TRANSFER_DFT A
                                                        WITH ( NOLOCK )
                                                        INNER JOIN PRD_GARMENT_TRANSFER_HD B
                                                        WITH ( NOLOCK ) ON A.DOC_NO = B.DOC_NO
                                                WHERE   A.JOB_ORDER_NO IN (
                                                        SELECT
                                                              LOT_NO
                                                        FROM  @JO_LOT )
                                                        AND B.STATUS IN ( 'S',
                                                              'C' )
                                                        AND A.GRADE_CD = 'A'
                                                        AND A.REASON_CD IS NOT NULL
                                                        AND A.REASON_CD <> ''
                        --结存正品数
                      ) G ON JO.JO_NO = G.JOB_ORDER_NO
            LEFT JOIN ( SELECT  JOB_ORDER_NO ,
                                SUM(QTY) AS ACTUAL_QTY
                        FROM    CUT_BUNDLE_HD WITH ( NOLOCK )
                        WHERE   JOB_ORDER_NO = '" + JoNo + @"'
                                AND STATUS = 'Y'
                        GROUP BY JOB_ORDER_NO
                      ) F ON F.JOB_ORDER_NO = JO.JO_NO
  WHERE     JO.JO_NO = '" + JoNo + @"'";


        return MESComment.DBUtility.GetTable(SQL, "MES");
    }

    private static DataTable GetDetailByJO(string JoNo, string FactoryCD)
    {
//        string SQL = @" 
//                        SELECT DISTINCT C.JO_NO AS COMBINE_JO_NO, B.COLOR_CODE, E.COLOR_DESC, B.SIZE_CODE, B.SIZE_CODE1, B.SIZE_CODE2, B.ORDER_QTY AS ORDER_QTY,
//                            ISNULL(F.ACTUAL_QTY,0) AS ACTUAL_QTY, 0 AS SCAN_QTY, ISNULL(G.QTY,0) AS LEFTOVER_A, '         ' AS PERCENTAGE, 
//                            0 AS DIFF, 0.00 AS MAX_SCAN_PACK, 0.00 AS MIN_SCAN_PACK, 0 AS AVAILABLE_BALANCE_SCAN_PACK_QTY, 0.00 AS OVER_PACK, 0.00 AS SHORT_PACK,
//                            ISNULL(DIS_B.DISCREPANCY_QTY,0)+ISNULL(TRANS_B.DISCREPANCY_QTY,0) AS GRADE_B, ISNULL(DIS.DISCREPANCY_QTY,0) AS DISCREPANCY_QTY, 
//                            SIZE_SEQ.SEQUENCE AS SEQ1, ISNULL(SIZE_SEQ2.SEQUENCE,0) AS SEQ2 
//                        FROM JO_HD C WITH(NOLOCK)
//                        INNER JOIN (
//                            SELECT DISTINCT DT.JO_NO AS COMBINE_JO_NO, DT.COLOR_CODE, CASE WHEN DT.SIZE_CODE2='-' THEN DT.SIZE_CODE1 ELSE DT.SIZE_CODE1 + ' ' + DT.SIZE_CODE2 END AS SIZE_CODE, 
//		                        DT.SIZE_CODE1, DT.SIZE_CODE2,
//		                        SUM(DT.QTY) AS ORDER_QTY 
//                            FROM JO_DT DT WITH(NOLOCK)
//                            LEFT JOIN JO_COMBINE_MAPPING MAP WITH(NOLOCK) ON DT.JO_NO=MAP.ORIGINAL_JO_NO
//                            WHERE DT.JO_NO='" + JoNo + @"'
//                            GROUP BY DT.JO_NO, DT.COLOR_CODE, DT.SIZE_CODE1, DT.SIZE_CODE2, CASE WHEN DT.SIZE_CODE2='-' THEN DT.SIZE_CODE1 ELSE DT.SIZE_CODE1 + ' ' + DT.SIZE_CODE2 END
//                        ) B ON B.COMBINE_JO_NO=C.JO_NO
//                        LEFT JOIN JO_COMBINE_MAPPING A WITH(NOLOCK) ON A.COMBINE_JO_NO=C.JO_NO
//                        INNER JOIN SC_HD D WITH(NOLOCK) ON C.SC_NO=D.SC_NO
//                        INNER JOIN SC_COLOR E WITH(NOLOCK) ON D.SC_NO=E.SC_NO AND B.COLOR_CODE=E.COLOR_CODE
//                        LEFT JOIN (
//                            SELECT JOB_ORDER_NO, COLOR_CD, SIZE_CD, SUM(QTY) AS ACTUAL_QTY 
//                            FROM CUT_BUNDLE_HD WITH(NOLOCK) WHERE JOB_ORDER_NO='" + JoNo + @"' AND STATUS='Y' AND FACTORY_CD='" + FactoryCD + @"'
//                            GROUP BY JOB_ORDER_NO, COLOR_CD, SIZE_CD
//                        ) F ON F.JOB_ORDER_NO=C.JO_NO AND F.COLOR_CD=B.COLOR_CODE AND F.SIZE_CD=B.SIZE_CODE
//                        LEFT JOIN (
//                          SELECT COLOR_CD, SIZE_CD, SUM(A.QTY) AS QTY
//                            FROM PRD_GARMENT_TRANSFER_DFT A WITH(NOLOCK)
//                            INNER JOIN PRD_GARMENT_TRANSFER_HD B WITH(NOLOCK) ON A.DOC_NO=B.DOC_NO
//                            LEFT JOIN JO_COMBINE_MAPPING C WITH(NOLOCK) ON A.JOB_ORDER_NO=C.ORIGINAL_JO_NO
//                            WHERE B.STATUS IN ('S','C') AND A.GRADE_CD='A' AND A.REASON_CD<>'' AND C.COMBINE_JO_NO='" + JoNo + @"'
//                            GROUP BY COLOR_CD, SIZE_CD
//                        ) G ON G.COLOR_CD=B.COLOR_CODE AND G.SIZE_CD=B.SIZE_CODE
//                        LEFT JOIN (
//                            SELECT A.COLOR_CODE, A.SIZE_CODE, SUM(C.PULLOUT_QTY) AS DISCREPANCY_QTY
//                            FROM PRD_JO_DISCREPANCY_PULLOUT_TRX A WITH(NOLOCK)
//                            INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_HD B WITH(NOLOCK) ON A.DOC_NO=B.DOC_NO
//                            INNER JOIN PRD_JO_PULLOUT_REASON C WITH(NOLOCK) ON A.TRX_ID=C.TRX_ID
//                            WHERE B.STATUS='C' AND C.GRADE_CD='C' AND C.REASON_CD IN ('GEG005','GEG033','GEG034','GEG036','GEG037','GEG038')
//                            AND A.JOB_ORDER_NO='" + JoNo + @"' AND A.PULLOUT_QTY IS NULL
//                            GROUP BY A.COLOR_CODE, A.SIZE_CODE
//                        ) DIS ON DIS.COLOR_CODE=B.COLOR_CODE AND DIS.SIZE_CODE=B.SIZE_CODE
//                        LEFT JOIN (
//                            SELECT A.COLOR_CODE, A.SIZE_CODE, SUM(C.PULLOUT_QTY) AS DISCREPANCY_QTY
//                            FROM PRD_JO_DISCREPANCY_PULLOUT_TRX A WITH(NOLOCK)
//                            INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_HD B WITH(NOLOCK) ON A.DOC_NO=B.DOC_NO
//                            INNER JOIN PRD_JO_PULLOUT_REASON C WITH(NOLOCK) ON A.TRX_ID=C.TRX_ID
//                            WHERE B.STATUS='C' AND C.GRADE_CD IN ('B','C') AND C.REASON_CD NOT IN ('GEG003','GEG005','GEG033','GEG034','GEG036','GEG037','GEG038')
//                            AND A.JOB_ORDER_NO='" + JoNo + @"' AND A.PULLOUT_QTY IS NULL
//                            GROUP BY A.COLOR_CODE, A.SIZE_CODE
//                        ) DIS_B ON DIS_B.COLOR_CODE=B.COLOR_CODE AND DIS_B.SIZE_CODE=B.SIZE_CODE
//                        LEFT JOIN (
//                            SELECT A.COLOR_CD, A.SIZE_CD, SUM(A.QTY) AS DISCREPANCY_QTY
//                            FROM PRD_GARMENT_TRANSFER_DFT A WITH(NOLOCK)
//                            INNER JOIN PRD_GARMENT_TRANSFER_HD B WITH(NOLOCK) ON A.DOC_NO=B.DOC_NO
//                            INNER JOIN JO_COMBINE_MAPPING C WITH(NOLOCK) ON A.JOB_ORDER_NO=C.ORIGINAL_JO_NO
//                            WHERE B.STATUS='C' AND A.GRADE_CD='B' AND C.COMBINE_JO_NO='" + JoNo + @"'
//                            GROUP BY A.COLOR_CD, A.SIZE_CD
//                        ) TRANS_B ON TRANS_B.COLOR_CD=B.COLOR_CODE AND TRANS_B.SIZE_CD=B.SIZE_CODE
//                        LEFT JOIN (
//	                        SELECT * FROM SC_SIZE WITH(NOLOCK) WHERE SIZE_TYPE=1
//                        ) SIZE_SEQ ON D.SC_NO=SIZE_SEQ.SC_NO AND B.SIZE_CODE1=SIZE_SEQ.SIZE_CODE
//                        LEFT JOIN (
//	                        SELECT * FROM SC_SIZE WITH(NOLOCK) WHERE SIZE_TYPE=2
//                        ) SIZE_SEQ2 ON D.SC_NO=SIZE_SEQ2.SC_NO AND B.SIZE_CODE2=SIZE_SEQ2.SIZE_CODE
//                        WHERE C.JO_NO='" + JoNo + @"' AND C.FACTORY_CD='" + FactoryCD + @"'
//                        ORDER BY COLOR_CODE, SEQ1, SEQ2";
        string SQL = @"

 DECLARE @JO_LOT TABLE
                            (
                             
                              LOT_NO NVARCHAR(25) 
                            )
INSERT INTO  @JO_LOT                      
SELECT ISNULL(b.ORIGINAL_JO_NO,a.JO_NO) FROM dbo.JO_HD a
LEFT JOIN dbo.JO_COMBINE_MAPPING b ON a.JO_NO=b.COMBINE_JO_NO
 WHERE JO_NO= '" + JoNo + @"'

SELECT DISTINCT
        C.JO_NO AS COMBINE_JO_NO ,
        B.COLOR_CODE ,
        E.COLOR_DESC ,
        B.SIZE_CODE ,
        B.SIZE_CODE1 ,
        B.SIZE_CODE2 ,
        B.ORDER_QTY AS ORDER_QTY ,
        ISNULL(F.ACTUAL_QTY, 0) AS ACTUAL_QTY ,
        0 AS SCAN_QTY ,
        ISNULL(G.QTY, 0) AS LEFTOVER_A ,
        '         ' AS PERCENTAGE ,
        0 AS DIFF ,
        0.00 AS MAX_SCAN_PACK ,
        0.00 AS MIN_SCAN_PACK ,
        0 AS AVAILABLE_BALANCE_SCAN_PACK_QTY ,
        0.00 AS OVER_PACK ,
        0.00 AS SHORT_PACK ,
        ISNULL(DIS_B.DISCREPANCY_QTY, 0) + ISNULL(TRANS_B.DISCREPANCY_QTY, 0) AS GRADE_B ,
        ISNULL(DIS.DISCREPANCY_QTY, 0) AS DISCREPANCY_QTY ,
        SIZE_SEQ.SEQUENCE AS SEQ1 ,
        ISNULL(SIZE_SEQ2.SEQUENCE, 0) AS SEQ2
FROM    JO_HD C WITH ( NOLOCK )
        INNER JOIN ( SELECT DISTINCT
                            DT.JO_NO AS COMBINE_JO_NO ,
                            DT.COLOR_CODE ,
                            CASE WHEN DT.SIZE_CODE2 = '-' THEN DT.SIZE_CODE1
                                 ELSE DT.SIZE_CODE1 + ' ' + DT.SIZE_CODE2
                            END AS SIZE_CODE ,
                            DT.SIZE_CODE1 ,
                            DT.SIZE_CODE2 ,
                            SUM(DT.QTY) AS ORDER_QTY
                     FROM   JO_DT DT WITH ( NOLOCK )
                            --LEFT JOIN JO_COMBINE_MAPPING MAP WITH ( NOLOCK ) ON DT.JO_NO = MAP.ORIGINAL_JO_NO
                     WHERE  DT.JO_NO = '" + JoNo + @"'
                     GROUP BY DT.JO_NO ,
                            DT.COLOR_CODE ,
                            DT.SIZE_CODE1 ,
                            DT.SIZE_CODE2 ,
                            CASE WHEN DT.SIZE_CODE2 = '-' THEN DT.SIZE_CODE1
                                 ELSE DT.SIZE_CODE1 + ' ' + DT.SIZE_CODE2
                            END
                   ) B ON B.COMBINE_JO_NO = C.JO_NO
        LEFT JOIN JO_COMBINE_MAPPING A WITH ( NOLOCK ) ON A.COMBINE_JO_NO = C.JO_NO
        INNER JOIN SC_HD D WITH ( NOLOCK ) ON C.SC_NO = D.SC_NO
        INNER JOIN SC_COLOR E WITH ( NOLOCK ) ON D.SC_NO = E.SC_NO
                                                 AND B.COLOR_CODE = E.COLOR_CODE
        LEFT JOIN ( SELECT  JOB_ORDER_NO ,
                            COLOR_CD ,
                            SIZE_CD ,
                            SUM(QTY) AS ACTUAL_QTY
                    FROM    CUT_BUNDLE_HD WITH ( NOLOCK )
                    WHERE   JOB_ORDER_NO = '" + JoNo + @"'
                            AND STATUS = 'Y'
                            AND FACTORY_CD = '" + FactoryCD + @"'
                    GROUP BY JOB_ORDER_NO ,
                            COLOR_CD ,
                            SIZE_CD
                  ) F ON F.JOB_ORDER_NO = C.JO_NO
                         AND F.COLOR_CD = B.COLOR_CODE
                         AND F.SIZE_CD = B.SIZE_CODE
        LEFT JOIN ( SELECT   A.COLOR_CD,A.SIZE_CD,
															SUM(A.QTY) AS QTY
													FROM    PRD_GARMENT_TRANSFER_DFT A
															WITH ( NOLOCK )
															INNER JOIN PRD_GARMENT_TRANSFER_HD B
															WITH ( NOLOCK ) ON A.DOC_NO = B.DOC_NO
													WHERE   A.JOB_ORDER_NO IN (
															SELECT
																  LOT_NO
															FROM  @JO_LOT )
															AND B.STATUS IN ( 'S',
																  'C' )
															AND A.GRADE_CD = 'A'
															AND A.REASON_CD IS NOT NULL
															AND A.REASON_CD <> ''
							--结存正品数
                    GROUP BY COLOR_CD ,
                            SIZE_CD
                  ) G ON G.COLOR_CD = B.COLOR_CODE
                         AND G.SIZE_CD = B.SIZE_CODE
        LEFT JOIN ( SELECT A.COLOR_CODE ,
                            A.SIZE_CODE ,
                            SUM(C.PULLOUT_QTY) AS DISCREPANCY_QTY
                                                FROM    PRD_JO_DISCREPANCY_PULLOUT_TRX A
                                                        WITH ( NOLOCK )
                                                        INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_HD B
                                                        WITH ( NOLOCK ) ON A.DOC_NO = B.DOC_NO
                                                        INNER JOIN PRD_JO_PULLOUT_REASON C
                                                        WITH ( NOLOCK ) ON A.TRX_ID = C.TRX_ID
                                                        INNER JOIN dbo.PRD_REASON_CODE R
                                                        WITH ( NOLOCK ) ON C.REASON_CD = R.REASON_CD
                                                WHERE   A.JOB_ORDER_NO =  '" + JoNo + @"'
                                                        AND B.STATUS = 'C'
                                                        AND C.GRADE_CD ='C'
                                                        AND R.FACTORY_CD = '" + FactoryCD + @"'
                                                        AND R.REASON_TYPE = 'Unaccountable'
                                                        AND R.QTY_AFFECTION in ('A')
                                                        AND A.PULLOUT_QTY IS NULL
                                                        AND R.REASON_DESC NOT LIKE '%不见衫%'
                                                GROUP BY A.COLOR_CODE ,
                            A.SIZE_CODE
                  ) DIS ON DIS.COLOR_CODE = B.COLOR_CODE
                           AND DIS.SIZE_CODE = B.SIZE_CODE
        LEFT JOIN ( SELECT A.COLOR_CD AS COLOR_CODE , A.SIZE_CD AS SIZE_CODE,
                                                        SUM(A.QTY) AS DISCREPANCY_QTY
                                                FROM    PRD_GARMENT_TRANSFER_DFT A
                                                        WITH ( NOLOCK )
                                                        INNER JOIN PRD_GARMENT_TRANSFER_HD B
                                                        WITH ( NOLOCK ) ON A.DOC_NO = B.DOC_NO
                                                WHERE   A.JOB_ORDER_NO IN (
                                                        SELECT
                                                              LOT_NO
                                                        FROM  @JO_LOT )
                                                        AND B.STATUS = 'C'
                                                        AND A.REASON_CD IS NOT NULL
                                                        AND A.REASON_CD <> ''
                                                        AND A.GRADE_CD = 'B'
                    GROUP BY A.COLOR_CD ,
                            A.SIZE_CD
                  ) DIS_B ON DIS_B.COLOR_CODE = B.COLOR_CODE
                             AND DIS_B.SIZE_CODE = B.SIZE_CODE
        LEFT JOIN (  SELECT A.COLOR_CODE ,
                            A.SIZE_CODE ,
                            SUM(C.PULLOUT_QTY) AS DISCREPANCY_QTY
                                                FROM    PRD_JO_DISCREPANCY_PULLOUT_TRX A
                                                        WITH ( NOLOCK )
                                                        INNER JOIN PRD_JO_DISCREPANCY_PULLOUT_HD B
                                                        WITH ( NOLOCK ) ON A.DOC_NO = B.DOC_NO
                                                        INNER JOIN PRD_JO_PULLOUT_REASON C
                                                        WITH ( NOLOCK ) ON A.TRX_ID = C.TRX_ID
                                                        INNER JOIN dbo.PRD_REASON_CODE R
                                                        WITH ( NOLOCK ) ON C.REASON_CD = R.REASON_CD
                                                WHERE   A.JOB_ORDER_NO = '" + JoNo + @"'
                                                        AND B.STATUS = 'C'
                                                        AND C.GRADE_CD ='C'
                                                        AND R.FACTORY_CD = '" + FactoryCD + @"'
                                                        AND R.REASON_TYPE = 'Unaccountable'
                                                        AND R.QTY_AFFECTION in ('D')
                                                        AND A.PULLOUT_QTY IS NULL
                                                        AND R.REASON_DESC NOT LIKE '%不见衫%'
                                                GROUP BY A.COLOR_CODE ,
                            A.SIZE_CODE
                  ) TRANS_B ON TRANS_B.COLOR_CODE = B.COLOR_CODE
                               AND TRANS_B.SIZE_CODE = B.SIZE_CODE
        LEFT JOIN ( SELECT  *
                    FROM    SC_SIZE WITH ( NOLOCK )
                    WHERE   SIZE_TYPE = 1
                  ) SIZE_SEQ ON D.SC_NO = SIZE_SEQ.SC_NO
                                AND B.SIZE_CODE1 = SIZE_SEQ.SIZE_CODE
        LEFT JOIN ( SELECT  *
                    FROM    SC_SIZE WITH ( NOLOCK )
                    WHERE   SIZE_TYPE = 2
                  ) SIZE_SEQ2 ON D.SC_NO = SIZE_SEQ2.SC_NO
                                 AND B.SIZE_CODE2 = SIZE_SEQ2.SIZE_CODE
WHERE   C.JO_NO = '" + JoNo + @"'
        AND C.FACTORY_CD = '" + FactoryCD + @"'
ORDER BY COLOR_CODE ,
        SEQ1 ,
        SEQ2";

        return MESComment.DBUtility.GetTable(SQL, "MES");
    }


    private static DataTable GetReduceQuantityByLOT(string JoNo, string FactoryCd)
    {
//        string SQL = @"  
//                    SELECT DISTINCT A.COMBINE_JO_NO, A.ORIGINAL_JO_NO, C.LOT_NO, B.COLOR_CODE, E.COLOR_DESC, B.SIZE_CODE, B.SIZE_CODE1, B.SIZE_CODE2, B.ORDER_QTY AS ORDER_QTY,
//                        ISNULL(F.ACTUAL_QTY,0) AS ACTUAL_QTY, 0 AS SCAN_QTY, ISNULL(G.QTY,0) AS LEFTOVER_A, '         ' AS PERCENTAGE, 
//                        0 AS DIFF, 
//                        (CASE WHEN H.PERCENT_OVER_ALLOWED=0 THEN B.ORDER_QTY ELSE CAST(ROUND((B.ORDER_QTY*(100+H.PERCENT_OVER_ALLOWED)/100),2) AS DECIMAL(18,2)) END) AS MAX_SCAN_PACK, 
//                        (CASE WHEN H.PERCENT_SHORT_ALLOWED=0 THEN B.ORDER_QTY ELSE CAST(ROUND((B.ORDER_QTY*(100-H.PERCENT_SHORT_ALLOWED)/100),2) AS DECIMAL(18,2)) END) AS MIN_SCAN_PACK, 
//                        0 AS STILL_CAN_PACK,
//                        H.PERCENT_OVER_ALLOWED, H.PERCENT_SHORT_ALLOWED, SIZE_SEQ.SEQUENCE AS SEQ1, ISNULL(SIZE_SEQ2.SEQUENCE,0) AS SEQ2 
//                    FROM JO_COMBINE_MAPPING A WITH(NOLOCK)
//                    INNER JOIN (
//                        SELECT DISTINCT DT.JO_NO, DT.COLOR_CODE, CASE WHEN DT.SIZE_CODE2='-' THEN DT.SIZE_CODE1 ELSE DT.SIZE_CODE1 + ' ' + DT.SIZE_CODE2 END AS SIZE_CODE,
//                        DT.SIZE_CODE1, DT.SIZE_CODE2, SUM(DT.QTY) AS ORDER_QTY 
//                        FROM JO_DT DT WITH(NOLOCK)
//                        GROUP BY DT.JO_NO, DT.COLOR_CODE, DT.SIZE_CODE1, DT.SIZE_CODE2, CASE WHEN DT.SIZE_CODE2='-' THEN DT.SIZE_CODE1 ELSE DT.SIZE_CODE1 + ' ' + DT.SIZE_CODE2 END
//                    ) B ON B.JO_NO=A.ORIGINAL_JO_NO
//                    INNER JOIN JO_HD C WITH(NOLOCK) ON A.ORIGINAL_JO_NO=C.JO_NO
//                    INNER JOIN SC_HD D WITH(NOLOCK) ON C.SC_NO=D.SC_NO
//                    INNER JOIN SC_COLOR E WITH(NOLOCK) ON D.SC_NO=E.SC_NO AND B.COLOR_CODE=E.COLOR_CODE
//                    LEFT JOIN (
//                        SELECT * FROM SC_SIZE WITH(NOLOCK) WHERE SIZE_TYPE=1
//                        ) SIZE_SEQ ON D.SC_NO=SIZE_SEQ.SC_NO AND B.SIZE_CODE1=SIZE_SEQ.SIZE_CODE
//                    LEFT JOIN (
//                        SELECT * FROM SC_SIZE WITH(NOLOCK) WHERE SIZE_TYPE=2
//                        ) SIZE_SEQ2 ON D.SC_NO=SIZE_SEQ2.SC_NO AND B.SIZE_CODE2=SIZE_SEQ2.SIZE_CODE
//                    LEFT JOIN (
//                        SELECT ORIGINAL_JO_NO, COLOR_CD, SIZE_CD, QTY AS ACTUAL_QTY 
//                        FROM PRD_DISTR_COMBINE_TO_ORIGINAL WITH(NOLOCK) 
//                        WHERE COMBINE_JO_NO='" + JoNo + @"' AND FACTORY_CD='" + FactoryCd + @"' AND TRX_TYPE='ACTUAL_CUT'
//                    ) F ON F.ORIGINAL_JO_NO=A.ORIGINAL_JO_NO AND F.COLOR_CD=B.COLOR_CODE AND F.SIZE_CD=B.SIZE_CODE
//                    LEFT JOIN (
//                        SELECT A.JOB_ORDER_NO, A.COLOR_CD, A.SIZE_CD, SUM(A.QTY) AS QTY
//                        FROM PRD_GARMENT_TRANSFER_DFT A WITH(NOLOCK)
//                        INNER JOIN PRD_GARMENT_TRANSFER_HD B WITH(NOLOCK) ON A.DOC_NO=B.DOC_NO
//                        INNER JOIN JO_COMBINE_MAPPING C WITH(NOLOCK) ON A.JOB_ORDER_NO=C.ORIGINAL_JO_NO
//                        WHERE B.STATUS IN ('S','C') AND A.GRADE_CD='A' AND A.REASON_CD<>'' AND C.COMBINE_JO_NO='" + JoNo + @"'
//                            AND B.FACTORY_CD='" + FactoryCd + @"'
//                        GROUP BY A.JOB_ORDER_NO, A.COLOR_CD, A.SIZE_CD
//                    ) G ON G.COLOR_CD=B.COLOR_CODE AND G.SIZE_CD=B.SIZE_CODE AND A.ORIGINAL_JO_NO=G.JOB_ORDER_NO
//                    INNER JOIN (
//                        SELECT A.PO_NO, A.LOT_NO, A.PERCENT_OVER_ALLOWED, A.PERCENT_SHORT_ALLOWED
//                        FROM SC_LOT A WITH(NOLOCK)
//                        INNER JOIN JO_COMBINE_MAPPING B WITH(NOLOCK) ON A.PO_NO=B.ORIGINAL_JO_NO
//                        WHERE B.COMBINE_JO_NO='" + JoNo + @"') H ON H.LOT_NO=C.LOT_NO AND H.PO_NO=A.ORIGINAL_JO_NO
//                    WHERE A.COMBINE_JO_NO='" + JoNo + @"'
//                    ORDER BY ORIGINAL_JO_NO, COLOR_CODE, SEQ1, SEQ2";
        string SQL = @"DECLARE @tempCOMBINE_MAPPING TABLE
    (
      COMBINE_JO_NO NVARCHAR(40) ,
      ORIGINAL_JO_NO NVARCHAR(40)
    )
INSERT  INTO @tempCOMBINE_MAPPING
        SELECT  COMBINE_JO_NO ,
                ORIGINAL_JO_NO
        FROM    JO_COMBINE_MAPPING WITH ( NOLOCK )
        WHERE   COMBINE_JO_NO = '" + JoNo + @"'
INSERT  INTO @tempCOMBINE_MAPPING
        SELECT  JO_NO ,
                JO_NO
        FROM    dbo.JO_HD WITH ( NOLOCK )
        WHERE   JO_NO = '" + JoNo + @"'
                AND NOT EXISTS ( SELECT 1
                                 FROM   JO_COMBINE_MAPPING
                                 WHERE  COMBINE_JO_NO = '" + JoNo + @"' )
SELECT DISTINCT
        A.COMBINE_JO_NO ,
        A.ORIGINAL_JO_NO ,
        C.LOT_NO ,
        B.COLOR_CODE ,
        E.COLOR_DESC ,
        B.SIZE_CODE ,
        B.SIZE_CODE1 ,
        B.SIZE_CODE2 ,
        B.ORDER_QTY AS ORDER_QTY ,
        ISNULL(F.ACTUAL_QTY, 0) AS ACTUAL_QTY ,
        0 AS SCAN_QTY ,
        ISNULL(G.QTY, 0) AS LEFTOVER_A ,
        '         ' AS PERCENTAGE ,
        0 AS DIFF ,
        ( CASE WHEN H.PERCENT_OVER_ALLOWED = 0 THEN B.ORDER_QTY
               ELSE CAST(ROUND(( B.ORDER_QTY * ( 100 + H.PERCENT_OVER_ALLOWED )
                                 / 100 ), 2) AS DECIMAL(18, 2))
          END ) AS MAX_SCAN_PACK ,
        ( CASE WHEN H.PERCENT_SHORT_ALLOWED = 0 THEN B.ORDER_QTY
               ELSE CAST(ROUND(( B.ORDER_QTY * ( 100 - H.PERCENT_SHORT_ALLOWED )
                                 / 100 ), 2) AS DECIMAL(18, 2))
          END ) AS MIN_SCAN_PACK ,
        0 AS STILL_CAN_PACK ,
        H.PERCENT_OVER_ALLOWED ,
        H.PERCENT_SHORT_ALLOWED ,
        SIZE_SEQ.SEQUENCE AS SEQ1 ,
        ISNULL(SIZE_SEQ2.SEQUENCE, 0) AS SEQ2,
        '-' + CONVERT(NVARCHAR(10), H.PERCENT_SHORT_ALLOWED) + '/+'
            + CONVERT(NVARCHAR(10), H.PERCENT_OVER_ALLOWED) + '%' AS ALLOWED_PERCENT 
FROM    @tempCOMBINE_MAPPING A
        INNER JOIN ( SELECT DISTINCT
                            DT.JO_NO ,
                            DT.COLOR_CODE ,
                            CASE WHEN DT.SIZE_CODE2 = '-' THEN DT.SIZE_CODE1
                                 ELSE DT.SIZE_CODE1 + ' ' + DT.SIZE_CODE2
                            END AS SIZE_CODE ,
                            DT.SIZE_CODE1 ,
                            DT.SIZE_CODE2 ,
                            SUM(DT.QTY) AS ORDER_QTY
                     FROM   JO_DT DT WITH ( NOLOCK )
                     GROUP BY DT.JO_NO ,
                            DT.COLOR_CODE ,
                            DT.SIZE_CODE1 ,
                            DT.SIZE_CODE2 ,
                            CASE WHEN DT.SIZE_CODE2 = '-' THEN DT.SIZE_CODE1
                                 ELSE DT.SIZE_CODE1 + ' ' + DT.SIZE_CODE2
                            END
                   ) B ON B.JO_NO = A.ORIGINAL_JO_NO
        INNER JOIN JO_HD C WITH ( NOLOCK ) ON A.ORIGINAL_JO_NO = C.JO_NO
        INNER JOIN SC_HD D WITH ( NOLOCK ) ON C.SC_NO = D.SC_NO
        INNER JOIN SC_COLOR E WITH ( NOLOCK ) ON D.SC_NO = E.SC_NO
                                                 AND B.COLOR_CODE = E.COLOR_CODE
        LEFT JOIN ( SELECT  *
                    FROM    SC_SIZE WITH ( NOLOCK )
                    WHERE   SIZE_TYPE = 1
                  ) SIZE_SEQ ON D.SC_NO = SIZE_SEQ.SC_NO
                                AND B.SIZE_CODE1 = SIZE_SEQ.SIZE_CODE
        LEFT JOIN ( SELECT  *
                    FROM    SC_SIZE WITH ( NOLOCK )
                    WHERE   SIZE_TYPE = 2
                  ) SIZE_SEQ2 ON D.SC_NO = SIZE_SEQ2.SC_NO
                                 AND B.SIZE_CODE2 = SIZE_SEQ2.SIZE_CODE
        LEFT JOIN (  
                  SELECT ORIGINAL_JO_NO ,
                            COLOR_CD ,
                            SIZE_CD ,SUM(ACTUAL_QTY) AS ACTUAL_QTY FROM(
                   SELECT  ORIGINAL_JO_NO ,
                            COLOR_CD ,
                            SIZE_CD ,
                            sum(QTY) AS ACTUAL_QTY
                    FROM    PRD_DISTR_COMBINE_TO_ORIGINAL WITH ( NOLOCK )
                    WHERE   COMBINE_JO_NO = '" + JoNo + @"'
                            AND FACTORY_CD = '" + FactoryCd + @"'
                            AND TRX_TYPE = 'ACTUAL_CUT'
                     group by  ORIGINAL_JO_NO ,COLOR_CD ,SIZE_CD
                    UNION ALL
                    SELECT  A.JOB_ORDER_NO ,
                            A.COLOR_CD ,
                            A.SIZE_CD ,
                            SUM(QTY) AS ACTUAL_QTY
                    FROM    dbo.CUT_BUNDLE_HD A WITH ( NOLOCK )
                            INNER JOIN @tempCOMBINE_MAPPING B ON A.JOB_ORDER_NO = B.ORIGINAL_JO_NO
                    WHERE   FACTORY_CD = '" + FactoryCd + @"'
                    GROUP BY A.JOB_ORDER_NO ,
                            A.COLOR_CD ,
                            A.SIZE_CD)t  GROUP BY ORIGINAL_JO_NO ,
                            COLOR_CD ,
                            SIZE_CD 
                  ) F ON F.ORIGINAL_JO_NO = A.ORIGINAL_JO_NO
                         AND F.COLOR_CD = B.COLOR_CODE
                         AND F.SIZE_CD = B.SIZE_CODE
        LEFT JOIN ( SELECT  A.JOB_ORDER_NO ,
                            A.COLOR_CD ,
                            A.SIZE_CD ,
                            SUM(A.QTY) AS QTY
                    FROM    PRD_GARMENT_TRANSFER_DFT A WITH ( NOLOCK )
                            INNER JOIN PRD_GARMENT_TRANSFER_HD B WITH ( NOLOCK ) ON A.DOC_NO = B.DOC_NO
                            INNER JOIN @tempCOMBINE_MAPPING C ON A.JOB_ORDER_NO = C.ORIGINAL_JO_NO
                    WHERE   B.STATUS IN ( 'S', 'C' )
                            AND A.GRADE_CD = 'A'
                            AND A.REASON_CD <> ''
                            AND B.FACTORY_CD = '" + FactoryCd + @"'
                    GROUP BY A.JOB_ORDER_NO ,
                            A.COLOR_CD ,
                            A.SIZE_CD
                  ) G ON G.COLOR_CD = B.COLOR_CODE
                         AND G.SIZE_CD = B.SIZE_CODE
                         AND A.ORIGINAL_JO_NO = G.JOB_ORDER_NO
        INNER JOIN ( SELECT A.PO_NO ,
                            A.LOT_NO ,
                            A.PERCENT_OVER_ALLOWED ,
                            A.PERCENT_SHORT_ALLOWED
                     FROM   SC_LOT A WITH ( NOLOCK )
                            INNER JOIN @tempCOMBINE_MAPPING B ON A.PO_NO = B.ORIGINAL_JO_NO
                   ) H ON H.LOT_NO = C.LOT_NO
                          AND H.PO_NO = A.ORIGINAL_JO_NO
WHERE   C.FACTORY_CD = '" + FactoryCd + @"'
ORDER BY ORIGINAL_JO_NO ,
        COLOR_CODE ,
        SEQ1 ,
        SEQ2";

        return MESComment.DBUtility.GetTable(SQL, "MES");
    }


    private static DataTable GetReduceQuantityByBPO(string JoNo, string FactoryCd)
    {
//        string SQL = @" 
//                    SELECT DISTINCT A.COMBINE_JO_NO, A.ORIGINAL_JO_NO, B.BUYER_PO, B.COLOR_CODE, E.COLOR_DESC, B.SIZE_CODE, B.SIZE_CODE1, B.SIZE_CODE2, B.ORDER_QTY AS ORDER_QTY,
//                        ISNULL(F.ACTUAL_QTY,0) AS ACTUAL_QTY, 0 AS SCAN_QTY, ISNULL(G.QTY,0) AS LEFTOVER_A, '         ' AS PERCENTAGE, 
//                        0 AS DIFF, 
//                        (CASE WHEN H.PERCENT_OVER_ALLOWED=0 THEN B.ORDER_QTY ELSE CAST(ROUND((B.ORDER_QTY*(100+H.PERCENT_OVER_ALLOWED)/100),2) AS DECIMAL(18,2)) END) AS MAX_SCAN_PACK, 
//                        (CASE WHEN H.PERCENT_SHORT_ALLOWED=0 THEN B.ORDER_QTY ELSE CAST(ROUND((B.ORDER_QTY*(100-H.PERCENT_SHORT_ALLOWED)/100),2) AS DECIMAL(18,2)) END) AS MIN_SCAN_PACK, 
//                        0 AS STILL_CAN_PACK,
//                        H.PERCENT_OVER_ALLOWED, H.PERCENT_SHORT_ALLOWED, SIZE_SEQ.SEQUENCE AS SEQ1, ISNULL(SIZE_SEQ2.SEQUENCE,0) AS SEQ2 
//                    FROM JO_COMBINE_MAPPING A WITH(NOLOCK)
//                    INNER JOIN (
//                        SELECT DISTINCT A.ORIGINAL_JO_NO AS JO_NO, C.BUYER_PO_NO AS BUYER_PO, D.COLOR_CODE, CASE WHEN D.SIZE_CODE2='-' THEN D.SIZE_CODE1 ELSE D.SIZE_CODE1 + ' ' + D.SIZE_CODE2 END AS SIZE_CODE, 
//                        D.SIZE_CODE1, D.SIZE_CODE2, SUM(D.QTY) AS ORDER_QTY
//                           FROM JO_COMBINE_MAPPING A WITH(NOLOCK)
//                           INNER JOIN JO_HD B WITH(NOLOCK) ON A.ORIGINAL_JO_NO=B.JO_NO
//                           INNER JOIN SCX_LOT_BPO_HD C WITH(NOLOCK) ON B.LOT_NO=C.LOT_NO AND B.SC_NO=C.SC_NO
//                           INNER JOIN SCX_LOT_BPO_DT D WITH(NOLOCK) ON C.BPO_ID=D.BPO_ID
//                           WHERE A.COMBINE_JO_NO='" + JoNo + @"'
//                           GROUP BY A.ORIGINAL_JO_NO, D.COLOR_CODE, C.BUYER_PO_NO, D.SIZE_CODE1, D.SIZE_CODE2, CASE WHEN D.SIZE_CODE2='-' THEN D.SIZE_CODE1 ELSE D.SIZE_CODE1 + ' ' + D.SIZE_CODE2 END
//                    ) B ON B.JO_NO=A.ORIGINAL_JO_NO
//                    INNER JOIN JO_HD C WITH(NOLOCK) ON A.ORIGINAL_JO_NO=C.JO_NO
//                    INNER JOIN SC_HD D WITH(NOLOCK) ON C.SC_NO=D.SC_NO
//                    INNER JOIN SC_COLOR E WITH(NOLOCK) ON D.SC_NO=E.SC_NO AND B.COLOR_CODE=E.COLOR_CODE
//                    LEFT JOIN (
//                    SELECT * FROM SC_SIZE WITH(NOLOCK) WHERE SIZE_TYPE=1
//		                ) SIZE_SEQ ON D.SC_NO=SIZE_SEQ.SC_NO AND B.SIZE_CODE1=SIZE_SEQ.SIZE_CODE
//	                LEFT JOIN (
//                    SELECT * FROM SC_SIZE WITH(NOLOCK) WHERE SIZE_TYPE=2
//		                ) SIZE_SEQ2 ON D.SC_NO=SIZE_SEQ2.SC_NO AND B.SIZE_CODE2=SIZE_SEQ2.SIZE_CODE
//                    LEFT JOIN (
//                        SELECT ORIGINAL_JO_NO, COLOR_CD, SIZE_CD, QTY AS ACTUAL_QTY 
//                        FROM PRD_DISTR_COMBINE_TO_ORIGINAL WITH(NOLOCK) 
//                        WHERE COMBINE_JO_NO='" + JoNo + @"' AND FACTORY_CD='" + FactoryCd + @"' AND TRX_TYPE='ACTUAL_CUT'
//                    ) F ON F.ORIGINAL_JO_NO=A.ORIGINAL_JO_NO AND F.COLOR_CD=B.COLOR_CODE AND F.SIZE_CD=B.SIZE_CODE
//                    LEFT JOIN (
//                        SELECT A.JOB_ORDER_NO, A.COLOR_CD, A.SIZE_CD, SUM(A.QTY) AS QTY
//                        FROM PRD_GARMENT_TRANSFER_DFT A WITH(NOLOCK)
//                        INNER JOIN PRD_GARMENT_TRANSFER_HD B WITH(NOLOCK) ON A.DOC_NO=B.DOC_NO
//                        INNER JOIN JO_COMBINE_MAPPING C WITH(NOLOCK) ON A.JOB_ORDER_NO=C.ORIGINAL_JO_NO
//                        WHERE B.STATUS IN ('S','C') AND A.GRADE_CD='A' AND A.REASON_CD<>'' AND C.COMBINE_JO_NO='" + JoNo + @"'
//                            AND B.FACTORY_CD='" + FactoryCd + @"'
//                        GROUP BY A.JOB_ORDER_NO, A.COLOR_CD, A.SIZE_CD
//                    ) G ON G.COLOR_CD=B.COLOR_CODE AND G.SIZE_CD=B.SIZE_CODE AND A.ORIGINAL_JO_NO=G.JOB_ORDER_NO
//                    INNER JOIN (
//                        SELECT A.PO_NO, A.LOT_NO, C.BUYER_PO_NO AS BUYER_PO, A.PERCENT_OVER_ALLOWED, A.PERCENT_SHORT_ALLOWED
//                        FROM SC_LOT A WITH(NOLOCK)
//                        INNER JOIN JO_COMBINE_MAPPING B WITH(NOLOCK) ON A.PO_NO=B.ORIGINAL_JO_NO
//                        INNER JOIN SCX_LOT_BPO_HD C WITH(NOLOCK) ON A.SC_NO=C.SC_NO AND A.LOT_NO=C.LOT_NO
//                        WHERE B.COMBINE_JO_NO='" + JoNo + @"') H ON H.LOT_NO=C.LOT_NO AND H.PO_NO=A.ORIGINAL_JO_NO AND B.BUYER_PO=H.BUYER_PO
//                    WHERE A.COMBINE_JO_NO='" + JoNo + @"'
//                    ORDER BY ORIGINAL_JO_NO, BUYER_PO, COLOR_CODE, SEQ1, SEQ2";
        string SQL = @"DECLARE @tempCOMBINE_MAPPING TABLE
    (
      COMBINE_JO_NO NVARCHAR(40) ,
      ORIGINAL_JO_NO NVARCHAR(40)
    )
INSERT  INTO @tempCOMBINE_MAPPING
        SELECT  COMBINE_JO_NO ,
                ORIGINAL_JO_NO
        FROM    JO_COMBINE_MAPPING WITH ( NOLOCK )
        WHERE   COMBINE_JO_NO = '" + JoNo + @"'
INSERT  INTO @tempCOMBINE_MAPPING
        SELECT  JO_NO ,
                JO_NO
        FROM    dbo.JO_HD WITH ( NOLOCK )
        WHERE   JO_NO = '" + JoNo + @"'
                AND NOT EXISTS ( SELECT 1
                                 FROM   JO_COMBINE_MAPPING
                                 WHERE  COMBINE_JO_NO = '" + JoNo + @"' )


SELECT DISTINCT
        A.COMBINE_JO_NO ,
        A.ORIGINAL_JO_NO ,
         BUYER_PO=dbo.FN_GET_BPO_FOR_IE(D.CUSTOMER_CD,H.MARKET_CD,H.CITY,B.BUYER_PO),
        B.COLOR_CODE ,
        E.COLOR_DESC ,
        B.SIZE_CODE ,
        B.SIZE_CODE1 ,
        B.SIZE_CODE2 ,
        B.ORDER_QTY AS ORDER_QTY ,
        ISNULL(F.ACTUAL_QTY, 0) AS ACTUAL_QTY ,
        0 AS SCAN_QTY ,
        ISNULL(G.QTY, 0) AS LEFTOVER_A ,
        '         ' AS PERCENTAGE ,
        0 AS DIFF ,
        ( CASE WHEN H.PERCENT_OVER_ALLOWED = 0 THEN B.ORDER_QTY*1.00
               ELSE CAST(ROUND(( B.ORDER_QTY * ( 100 + H.PERCENT_OVER_ALLOWED )
                                 / 100 ), 2) AS DECIMAL(18, 2))
          END ) AS MAX_SCAN_PACK ,
        ( CASE WHEN H.PERCENT_SHORT_ALLOWED = 0 THEN B.ORDER_QTY*1.00
               ELSE CAST(ROUND(( B.ORDER_QTY * ( 100 - H.PERCENT_SHORT_ALLOWED )
                                 / 100 ), 2) AS DECIMAL(18, 2))
          END ) AS MIN_SCAN_PACK ,
        0 AS STILL_CAN_PACK ,
        H.PERCENT_OVER_ALLOWED ,
        H.PERCENT_SHORT_ALLOWED ,
        SIZE_SEQ.SEQUENCE AS SEQ1 ,
        ISNULL(SIZE_SEQ2.SEQUENCE, 0) AS SEQ2,
        '-' + CONVERT(NVARCHAR(10), H.PERCENT_SHORT_ALLOWED) + '/+'
            + CONVERT(NVARCHAR(10), H.PERCENT_OVER_ALLOWED) + '%' AS ALLOWED_PERCENT 
FROM    @tempCOMBINE_MAPPING A
        INNER JOIN ( SELECT DISTINCT
                            A.ORIGINAL_JO_NO AS JO_NO ,
                            C.BUYER_PO_NO AS BUYER_PO ,
                            D.COLOR_CODE ,
                            CASE WHEN D.SIZE_CODE2 = '-' THEN D.SIZE_CODE1
                                 ELSE D.SIZE_CODE1 + ' ' + D.SIZE_CODE2
                            END AS SIZE_CODE ,
                            D.SIZE_CODE1 ,
                            D.SIZE_CODE2 ,
                            SUM(D.QTY) AS ORDER_QTY
                     FROM   @tempCOMBINE_MAPPING A
                            INNER JOIN JO_HD B WITH ( NOLOCK ) ON A.ORIGINAL_JO_NO = B.JO_NO
                            INNER JOIN SCX_LOT_BPO_HD C WITH ( NOLOCK ) ON B.LOT_NO = C.LOT_NO
                                                              AND B.SC_NO = C.SC_NO
                            INNER JOIN SCX_LOT_BPO_DT D WITH ( NOLOCK ) ON C.BPO_ID = D.BPO_ID
                     GROUP BY A.ORIGINAL_JO_NO ,
                            D.COLOR_CODE ,
                            C.BUYER_PO_NO ,
                            D.SIZE_CODE1 ,
                            D.SIZE_CODE2 ,
                            CASE WHEN D.SIZE_CODE2 = '-' THEN D.SIZE_CODE1
                                 ELSE D.SIZE_CODE1 + ' ' + D.SIZE_CODE2
                            END
                   ) B ON B.JO_NO = A.ORIGINAL_JO_NO
        INNER JOIN JO_HD C WITH ( NOLOCK ) ON A.ORIGINAL_JO_NO = C.JO_NO
        INNER JOIN SC_HD D WITH ( NOLOCK ) ON C.SC_NO = D.SC_NO
        INNER JOIN SC_COLOR E WITH ( NOLOCK ) ON D.SC_NO = E.SC_NO
                                                 AND B.COLOR_CODE = E.COLOR_CODE
        LEFT JOIN ( SELECT  *
                    FROM    SC_SIZE WITH ( NOLOCK )
                    WHERE   SIZE_TYPE = 1
                  ) SIZE_SEQ ON D.SC_NO = SIZE_SEQ.SC_NO
                                AND B.SIZE_CODE1 = SIZE_SEQ.SIZE_CODE
        LEFT JOIN ( SELECT  *
                    FROM    SC_SIZE WITH ( NOLOCK )
                    WHERE   SIZE_TYPE = 2
                  ) SIZE_SEQ2 ON D.SC_NO = SIZE_SEQ2.SC_NO
                                 AND B.SIZE_CODE2 = SIZE_SEQ2.SIZE_CODE
        LEFT JOIN (
               SELECT ORIGINAL_JO_NO ,
                            COLOR_CD ,
                            SIZE_CD ,SUM(ACTUAL_QTY) AS ACTUAL_QTY FROM(
                   SELECT  ORIGINAL_JO_NO ,
                            COLOR_CD ,
                            SIZE_CD ,
                            sum(QTY) AS ACTUAL_QTY
                    FROM    PRD_DISTR_COMBINE_TO_ORIGINAL WITH ( NOLOCK )
                    WHERE   COMBINE_JO_NO = '" + JoNo + @"'
                            AND FACTORY_CD = '" + FactoryCd + @"'
                            AND TRX_TYPE = 'ACTUAL_CUT'
                     group by ORIGINAL_JO_NO ,COLOR_CD ,SIZE_CD
                    UNION ALL
                    SELECT  A.JOB_ORDER_NO ,
                            A.COLOR_CD ,
                            A.SIZE_CD ,
                            SUM(QTY) AS ACTUAL_QTY
                    FROM    dbo.CUT_BUNDLE_HD A WITH ( NOLOCK )
                            INNER JOIN @tempCOMBINE_MAPPING B ON A.JOB_ORDER_NO = B.ORIGINAL_JO_NO
                    WHERE   FACTORY_CD = '" + FactoryCd + @"'
                    GROUP BY A.JOB_ORDER_NO ,
                            A.COLOR_CD ,
                            A.SIZE_CD)t GROUP BY ORIGINAL_JO_NO ,
                            COLOR_CD ,
                            SIZE_CD 
                  ) F ON F.ORIGINAL_JO_NO = A.ORIGINAL_JO_NO
                         AND F.COLOR_CD = B.COLOR_CODE
                         AND F.SIZE_CD = B.SIZE_CODE
        LEFT JOIN ( SELECT  A.JOB_ORDER_NO ,
                            A.COLOR_CD ,
                            A.SIZE_CD ,
                            SUM(A.QTY) AS QTY
                    FROM    PRD_GARMENT_TRANSFER_DFT A WITH ( NOLOCK )
                            INNER JOIN PRD_GARMENT_TRANSFER_HD B WITH ( NOLOCK ) ON A.DOC_NO = B.DOC_NO
                            INNER JOIN @tempCOMBINE_MAPPING C ON A.JOB_ORDER_NO = C.ORIGINAL_JO_NO
                    WHERE   B.STATUS IN ( 'S', 'C' )
                            AND A.GRADE_CD = 'A'
                            AND A.REASON_CD <> ''
                            AND B.FACTORY_CD = '" + FactoryCd + @"'
                    GROUP BY A.JOB_ORDER_NO ,
                            A.COLOR_CD ,
                            A.SIZE_CD
                  ) G ON G.COLOR_CD = B.COLOR_CODE
                         AND G.SIZE_CD = B.SIZE_CODE
                         AND A.ORIGINAL_JO_NO = G.JOB_ORDER_NO
        INNER JOIN ( SELECT A.PO_NO ,
                            A.LOT_NO ,
                            C.BUYER_PO_NO AS BUYER_PO ,
                            A.PERCENT_OVER_ALLOWED ,
                            A.PERCENT_SHORT_ALLOWED,
                            A.MARKET_CD,
                            A.CITY
                     FROM   SC_LOT A WITH ( NOLOCK )
                            INNER JOIN @tempCOMBINE_MAPPING B ON A.PO_NO = B.ORIGINAL_JO_NO
                            INNER JOIN SCX_LOT_BPO_HD C WITH ( NOLOCK ) ON A.SC_NO = C.SC_NO
                                                              AND A.LOT_NO = C.LOT_NO
                   ) H ON H.LOT_NO = C.LOT_NO
                          AND H.PO_NO = A.ORIGINAL_JO_NO
                          AND B.BUYER_PO = H.BUYER_PO
WHERE   C.FACTORY_CD = '" + FactoryCd + @"'
ORDER BY ORIGINAL_JO_NO ,
        BUYER_PO ,
        COLOR_CODE ,
        SEQ1 ,
        SEQ2 ";
        return MESComment.DBUtility.GetTable(SQL, "MES");
    }

    private static DataTable GetScanPackQty(string JoNo, string FactoryCD)
    {
        string SQL = @" 
                        SELECT 
	                       CASE  WHEN GOColor = '' THEN Color
			                     WHEN GOColor IS NULL THEN Color
			                     ELSE GOColor 
		                    END GOColor,
		                    CASE  WHEN GOSize1 = '' THEN SIZE
			                     WHEN GOSize1 IS NULL THEN SIZE
			                     ELSE GOSize1 + ' ' + GOSize2
		                    END GOSize,
                            SUM(Pacqty) AS PacTotal
                      
                    FROM      ASNItem WITH ( NOLOCK )
                    WHERE     LEFT(CutNo,12) in (" + JoNo + @") AND Pacqty<>0
                    GROUP BY   CASE  WHEN GOColor = '' THEN Color
			                     WHEN GOColor IS NULL THEN Color
			                     ELSE GOColor
		                    END,
                            CASE  WHEN GOSize1 = '' THEN SIZE
			                     WHEN GOSize1 IS NULL THEN SIZE
			                     ELSE GOSize1 + ' ' + GOSize2
		                    END";

        return MESComment.DBUtility.GetTable(SQL, "EASN");
    }


    private static DataTable GetScanPackQtyByJO(string JoNo, string FactoryCD)
    {
        string SQL = @" 
                        SELECT LEFT(CutNo,12) as CutNo,
	                       CASE  WHEN GOColor = '' THEN Color
			                     WHEN GOColor IS NULL THEN Color
			                     ELSE GOColor
		                    END GOColor,
		                    CASE  WHEN GOSize1 = '' THEN SIZE
			                     WHEN GOSize1 IS NULL THEN SIZE
			                     ELSE GOSize1 + ' ' + GOSize2
		                    END GOSize,
                            SUM(Pacqty) AS PacTotal
                           
                    FROM      ASNItem WITH ( NOLOCK )
                    WHERE   LEFT(CutNo,12) in (" + JoNo + @") AND Pacqty<>0
                    GROUP BY   LEFT(CutNo,12), CASE  WHEN GOColor = '' THEN Color
			                     WHEN GOColor IS NULL THEN Color
			                     ELSE GOColor
		                    END,
                            CASE  WHEN GOSize1 = '' THEN SIZE
			                     WHEN GOSize1 IS NULL THEN SIZE
			                     ELSE GOSize1 + ' ' + GOSize2
		                    END";

        return MESComment.DBUtility.GetTable(SQL, "EASN");
    }
    private static DataTable GetScanPackQtyByBPO(string JoNo, string FactoryCD)
    {
        string SQL = @" 
                        SELECT LEFT(A.CutNo,12) as CutNo,PONo as OrderNo,
	                       CASE  WHEN GOColor = '' THEN Color
			                     WHEN GOColor IS NULL THEN Color
			                     ELSE GOColor
		                    END GOColor,
		                    CASE  WHEN GOSize1 = '' THEN SIZE
			                     WHEN GOSize1 IS NULL THEN SIZE
			                     ELSE GOSize1 + ' ' + GOSize2
		                    END GOSize,
                            SUM(A.Pacqty) AS PacTotal
                          
                    FROM      ASNItem A WITH ( NOLOCK )
                    INNER JOIN dbo.ASNOrder B WITH ( NOLOCK ) ON A.CutNo=B.CutNo AND A.OrderNo=B.OrderNo
                    WHERE   LEFT(A.CutNo,12) in (" + JoNo + @") AND A.Pacqty<>0
                    GROUP BY   LEFT(A.CutNo,12), CASE  WHEN GOColor = '' THEN Color
			                     WHEN GOColor IS NULL THEN Color
			                     ELSE GOColor
		                    END,
                            CASE  WHEN GOSize1 = '' THEN SIZE
			                     WHEN GOSize1 IS NULL THEN SIZE
			                     ELSE GOSize1 + ' ' + GOSize2
		                    END, PONo";

        return MESComment.DBUtility.GetTable(SQL, "EASN");
    }

    private static DataTable GetOriginalJo(string JoNo)
    {
        string SQL = @" 
                        SELECT COMBINE_JO_NO, ORIGINAL_JO_NO
                        FROM JO_COMBINE_MAPPING WITH(NOLOCK)
                        WHERE COMBINE_JO_NO='" + JoNo + @"' ORDER BY ORIGINAL_JO_NO";

        return MESComment.DBUtility.GetTable(SQL, "MES");
    }


    private static DataTable GetSize(string JoNo)
    {
        string SQL = @"
                IF OBJECT_ID('tempdb..#TEMP_SIZE') IS NOT NULL 
                BEGIN
                    DROP TABLE #TEMP_SIZE
                END ;
                DECLARE @TABLE_COL NVARCHAR(500)
                DECLARE @TABLE_EMPTY NVARCHAR(500)
                DECLARE @TABLE_COL_NULL NVARCHAR(500) 
                SELECT  DISTINCT
                        SIZE_CD ,
                        SEQUENCE
                INTO    #TEMP_SIZE
                FROM    ( SELECT DISTINCT
                                    JO_NO ,
                                    SIZE_CODE1 ,
                                    CASE SIZE_CODE2
                                        WHEN '-' THEN SIZE_CODE1
                                        ELSE SIZE_CODE1 + ' ' + SIZE_CODE2
                                    END AS SIZE_CD ,
                                    COLOR_CODE AS COLOR_CD ,
                                    QTY AS ORDER_QTY
                            FROM      dbo.JO_DT AS c WITH ( NOLOCK )
                            WHERE     ( JO_NO = '" + JoNo + @"' )
                        ) A
                        INNER JOIN JO_HD AS B ON A.JO_NO = B.JO_NO
                        INNER JOIN SC_SIZE AS C ON B.SC_NO = C.SC_NO
                                                    AND A.SIZE_CODE1 = C.SIZE_CODE
                ORDER BY C.SEQUENCE  
                       
                SET @TABLE_COL = ''
                SET @TABLE_EMPTY = ''
                SELECT  @TABLE_COL = @TABLE_COL + '[' + SIZE_CD + '],',
                        @TABLE_EMPTY = @TABLE_EMPTY + 'ISNULL([' + SIZE_CD + '],0) [' + SIZE_CD + '],'
                FROM    #TEMP_SIZE
                ORDER BY SEQUENCE
                IF @TABLE_COL <> '' 
                    SELECT  @TABLE_COL = LEFT(@TABLE_COL, LEN(@TABLE_COL) - 1)
                 IF @TABLE_EMPTY <> '' 
                    SELECT  @TABLE_EMPTY = LEFT(@TABLE_EMPTY, LEN(@TABLE_EMPTY) - 1)


                SET @TABLE_COL_NULL = ''
                SELECT  @TABLE_COL_NULL = @TABLE_COL_NULL + 'ISNULL([' + SIZE_CD + '],0) +'
                FROM    #TEMP_SIZE
                IF @TABLE_COL_NULL <> '' 
                    SELECT  @TABLE_COL_NULL = LEFT(@TABLE_COL_NULL, LEN(@TABLE_COL_NULL) - 1)
                SELECT  @TABLE_COL SIZE ,@TABLE_COL_NULL SIZE_NULL,@TABLE_EMPTY SIZE_EMPTY
                DROP TABLE  #TEMP_SIZE
            ";
        return MESComment.DBUtility.GetTable(SQL, "MES");
    }


    private static DataTable GetScanPackEndDateByJO(string JoNo, string FactoryCd)
    {
        string SQL = @" 
                        select ISNULL(CONVERT(NVARCHAR(10),MAX(CloseDate),120),'') as CloseDate,sum(PacQty) as PacQty from ASNPack with(nolock)
                        where LEFT(CutNo,12) in (" + JoNo+ @")";

        return MESComment.DBUtility.GetTable(SQL, "EASN");
    }


    //Added by MF on 20160420, JO Combination-Approve button
    private static DataTable ApproveAuthorise(string UserId)
    {
        string SQL = @" 
                        SELECT TOP 1 1 FROM MES_USER_ROLE A WITH(NOLOCK) INNER JOIN MES_USER B WITH(NOLOCK) ON A.USER_ID=B.USER_ID
                        INNER JOIN MES_ROLE C WITH(NOLOCK) ON A.ROLE_ID=C.ROLE_ID
                            WHERE B.USER_ID='" + UserId + @"' AND C.ROLE_NAME='APPROVE_FOR_SCAN_PACK_REPORT'";

        return MESComment.DBUtility.GetTable(SQL, "MES");
    }
    //End of added by MF on 20160420, JO Combination-Approve button


    private void ShowJOHeader(DataTable dtHeader, DataTable dtEndDate)
    {
        this.gvDetail.InnerHtml += " <tr ><td><table width='95%' border='1' cellspacing='0' cellpadding='0' style='font-size: 12px; border-collapse: collapse'>"; 
        this.gvDetail.InnerHtml += " <tr>";
        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>Customer :</td>";
        this.gvDetail.InnerHtml += " <td width='170' colspan='5'>" + dtHeader.Rows[0]["CUSTOMER"].ToString() + "</td></tr>";
        this.gvDetail.InnerHtml += " <tr><td class='tr2style' width='100'>S/C No :</td>";
        this.gvDetail.InnerHtml += " <td colspan='5'>" + dtHeader.Rows[0]["SC_NO"].ToString() + "</td></tr>";
        this.gvDetail.InnerHtml += " <tr><td class='tr2style' width='100'>Jo No :</td>";
        this.gvDetail.InnerHtml += " <td>" + dtHeader.Rows[0]["JO_NO"].ToString() + "</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>Lot No :</td>";
        this.gvDetail.InnerHtml += " <td colspan='3'>" + dtHeader.Rows[0]["LOT_NO"].ToString() + "</td></tr>";
        this.gvDetail.InnerHtml += " <tr><td class='tr2style' width='100'>Buyer PO :</td>";
        this.gvDetail.InnerHtml += " <td>" + dtHeader.Rows[0]["BPO"].ToString() + "</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>Style No :</td>";
        this.gvDetail.InnerHtml += " <td colspan='3'>" + dtHeader.Rows[0]["STYLE_NO"].ToString() + "</td></tr>";

        this.gvDetail.InnerHtml += " <tr><td class='tr2style' width='100'>Style Desc :</td>";
        this.gvDetail.InnerHtml += " <td width='500'>" + dtHeader.Rows[0]["STYLE_DESC"].ToString() + "</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>Season :</td>";
        this.gvDetail.InnerHtml += " <td width='200'>" + dtHeader.Rows[0]["SEASON_CD"].ToString() + "</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>Over/ Short Ship% :</td>";
        this.gvDetail.InnerHtml += " <td >" + dtHeader.Rows[0]["ALLOWED_PERCENT"].ToString() + "</td></tr>";

        this.gvDetail.InnerHtml += " <tr><td class='tr2style' width='100'>Discrepancy Qty :</td>";
        this.gvDetail.InnerHtml += " <td >" + dtHeader.Rows[0]["DISCREPANCY_QTY"].ToString() + "</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>Sample :</td>";
        this.gvDetail.InnerHtml += " <td colspan='3'>" + dtHeader.Rows[0]["SAMPLE_QTY"].ToString() + "</td></tr>";
        this.gvDetail.InnerHtml += " <tr><td class='tr2style' width='100'>Grade B Garment :</td>";
        this.gvDetail.InnerHtml += " <td colspan='5'>" + dtHeader.Rows[0]["GRADE_B"].ToString() + "</td></tr>";

        this.gvDetail.InnerHtml += " <tr><td class='tr2style' width='100'>Pullout :</td>";
        this.gvDetail.InnerHtml += " <td colspan='5'>" + dtHeader.Rows[0]["Pullout"].ToString() + "</td></tr>";

        this.gvDetail.InnerHtml += " <tr><td class='tr2style' width='100'>Missing Garment :</td>";
        this.gvDetail.InnerHtml += " <td colspan='5'>" + (dtHeader.Rows[0]["MISSING_GARMENT"].ToInt("t") - dtEndDate.Rows[0]["PacQty"].ToInt("t")) + "</td></tr>";
        this.gvDetail.InnerHtml += " <tr><td class='tr2style' width='100'>Leftover Reason :</td>";
        this.gvDetail.InnerHtml += " <td colspan='5'>" + dtHeader.Rows[0]["REASON_CD"].ToString() + "</td></tr>";

        this.gvDetail.InnerHtml += " <tr><td class='tr2style' width='100'>End date :</td>";
        this.gvDetail.InnerHtml += " <td>" + dtEndDate.Rows[0]["CloseDate"].ToString() + "</td>";
        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>Status :</td>";
        this.gvDetail.InnerHtml += " <td colspan='3'>" + dtHeader.Rows[0]["STATUS"].ToString() + "</td></tr>";

        this.gvDetail.InnerHtml += " </table></td></tr>";
        this.gvDetail.InnerHtml += " <tr><td>&nbsp;</td></tr>";//空一行,美观;
    }
    private string getTrClass(double persent)
    {
        if (persent == 0)
            return "";
        if (persent > 0)
        {
            if (persent > percent_Over_Allowed)
                return "over";
        }
        else
        {
            if (Math.Abs(persent) > percent_Short_Allowed)
                return "short";
        }
        return "";
    }
    public void ShowJODetail(DataTable dtBySummary)
    {
        DataRow[] dr;
        int totalOrder = 0;
        int totalActual = 0;
        int totalScanPack = 0;
        int gradeA = 0;
        int diff = 0;
        Decimal maxPack = 0;
        Decimal minPack = 0;
        Decimal balance = 0;
        Decimal overPack = 0;
        Decimal shortPack = 0;
        int gradeB = 0;
        int discrepancy = 0;
        string zero_percent = "";
        string old_color = "";

        this.gvDetail.InnerHtml += " <tr><td><table width='95%' border='1' cellspacing='0' cellpadding='4' style='font-size: 12px; border-collapse: collapse'>";

        switch (strculture)
        {//选择显示语言;
            case "en":
                this.gvDetail.InnerHtml += " <tr><td class='tr2style' width='25%'>Color</td><td class='tr2style' width='100'>Size</td><td class='tr2style' width='100'>Order Qty</td><td class='tr2style' width='100'>Actual Qty</td><td class='tr2style' width='100'>Scan Pack Qty</td><td class='tr2style' width='100'>Grade A Leftover</td>";
                this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>Percentage</td><td class='tr2style' width='100'>Diferrences</td><td class='tr2style' width='100'>Max Scan Pack</td><td class='tr2style' width='100'>Min Scan Pack</td><td class='tr2style' width='8%'>Available Balance</td><td class='tr2style' width='100'>Over Pack</td>";
                this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>Short Pack</td><td class='tr2style' width='100'>Grade B Leftover</td><td class='tr2style' width='100'>Discrepancy Qty</td></tr>";
                break;
            case "zh":
                this.gvDetail.InnerHtml += " <tr><td class='tr2style' width='25%'>颜色</td><td class='tr2style' width='100'>尺码</td><td class='tr2style' width='100'>订单数</td><td class='tr2style' width='100'>实裁数</td><td class='tr2style' width='100'>扫描数</td><td class='tr2style' width='100'>结存正品数</td>";
                this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>百分比</td><td class='tr2style' width='100'>差异</td><td class='tr2style' width='100'>最多出货数</td><td class='tr2style' width='100'>最小出货数</td><td class='tr2style' width='8%'>还可以出货数量</td><td class='tr2style' width='100'>多装数量</td>";
                this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>短装数量</td><td class='tr2style' width='100'>次品细数</td><td class='tr2style' width='100'>抽办细数</td></tr>";
                break;
        }

        for (int i = 0; i < dtBySummary.Rows.Count; i++)
        {
            if (old_color != dtBySummary.Rows[i]["COLOR_DESC"].ToString())
            {
                dr = dtBySummary.Select("COLOR_DESC='" + dtBySummary.Rows[i]["COLOR_DESC"].ToString() + "' AND COMBINE_JO_NO='" + dtBySummary.Rows[i]["COMBINE_JO_NO"].ToString() + "'");

                //this.gvDetail.InnerHtml += string.Format("<tr {0}>", getTrClass(dtBySummary.Rows[i]["PERCENTAGE"].ToDouble()));
                this.gvDetail.InnerHtml += "<tr> <td rowspan='" + dr.Count() + "'>" + dtBySummary.Rows[i]["COLOR_DESC"].ToString() + "</td>";
                old_color = dtBySummary.Rows[i]["COLOR_DESC"].ToString();
            }
            SetOverAndShortPack(dtBySummary.Rows[i]);
            //this.gvDetail.InnerHtml += " <td>" + dtBySummary.Rows[i]["SIZE_CODE"].ToString() + "</td>";
            //this.gvDetail.InnerHtml += " <td>" + dtBySummary.Rows[i]["ORDER_QTY"].ToString() + "</td>";
            //this.gvDetail.InnerHtml += " <td>" + dtBySummary.Rows[i]["ACTUAL_QTY"].ToString() + "</td>";
            //this.gvDetail.InnerHtml += " <td>" + dtBySummary.Rows[i]["SCAN_QTY"].ToString() + "</td>";
            //this.gvDetail.InnerHtml += " <td>" + dtBySummary.Rows[i]["LEFTOVER_A"].ConvertZeroToEmpty() + "</td>";
            //this.gvDetail.InnerHtml += " <td>" + dtBySummary.Rows[i]["PERCENTAGE"].ToString() + "</td>";
            //this.gvDetail.InnerHtml += " <td>" + dtBySummary.Rows[i]["DIFF"].ToString() + "</td>";
            //this.gvDetail.InnerHtml += " <td>" + dtBySummary.Rows[i]["MAX_SCAN_PACK"].ToInt("t") + "</td>";
            //this.gvDetail.InnerHtml += " <td>" + dtBySummary.Rows[i]["MIN_SCAN_PACK"].ToInt("c") + "</td>";
            //this.gvDetail.InnerHtml += " <td>" + (dtBySummary.Rows[i]["AVAILABLE_BALANCE_SCAN_PACK_QTY"].ToInt("t") <= 0 ? "" : dtBySummary.Rows[i]["AVAILABLE_BALANCE_SCAN_PACK_QTY"].ToInt("t").ToString()) + "</td>";
            //this.gvDetail.InnerHtml += " <td>" + (dtBySummary.Rows[i]["OVER_PACK"].ToString() == "0" ? "&nbsp;" : dtBySummary.Rows[i]["OVER_PACK"].ToString()) + "</td>";
            //this.gvDetail.InnerHtml += " <td>" +  (dtBySummary.Rows[i]["SHORT_PACK"].ToString()== "0" ? "&nbsp;" :dtBySummary.Rows[i]["SHORT_PACK"].ToString()) + "</td>";
            //this.gvDetail.InnerHtml += " <td>" + dtBySummary.Rows[i]["GRADE_B"].ConvertZeroToEmpty() + "</td>";
            //this.gvDetail.InnerHtml += " <td>" + dtBySummary.Rows[i]["DISCREPANCY_QTY"].ConvertZeroToEmpty() + "</td></tr>";
            string cssclass = getTrClass(dtBySummary.Rows[i]["PERCENTAGE"].ToDouble());
            this.gvDetail.InnerHtml += dtBySummary.Rows[i]["SIZE_CODE"].ToTD(cssclass);
            this.gvDetail.InnerHtml += dtBySummary.Rows[i]["ORDER_QTY"].ToTD(cssclass);
            this.gvDetail.InnerHtml += dtBySummary.Rows[i]["ACTUAL_QTY"].ToTD(cssclass);
            this.gvDetail.InnerHtml += dtBySummary.Rows[i]["SCAN_QTY"].ToTD(cssclass);
            this.gvDetail.InnerHtml += dtBySummary.Rows[i]["LEFTOVER_A"].ConvertZeroToEmpty().ToTD(cssclass);
            this.gvDetail.InnerHtml += dtBySummary.Rows[i]["PERCENTAGE"].ToPersent().ToTD(cssclass);
            this.gvDetail.InnerHtml += dtBySummary.Rows[i]["DIFF"].ToTD(cssclass);
            this.gvDetail.InnerHtml += dtBySummary.Rows[i]["MAX_SCAN_PACK"].ToInt("t").ToTD(cssclass);
            this.gvDetail.InnerHtml += dtBySummary.Rows[i]["MIN_SCAN_PACK"].ToInt("c").ToTD(cssclass);
            this.gvDetail.InnerHtml += (dtBySummary.Rows[i]["AVAILABLE_BALANCE_SCAN_PACK_QTY"].ToInt("t") <= 0 ? 0 : dtBySummary.Rows[i]["AVAILABLE_BALANCE_SCAN_PACK_QTY"].ToInt("t")).ConvertZeroToEmpty().ToTD(cssclass);
            this.gvDetail.InnerHtml += dtBySummary.Rows[i]["OVER_PACK"].ConvertZeroToEmpty().ToTD(cssclass);
            this.gvDetail.InnerHtml += dtBySummary.Rows[i]["SHORT_PACK"].ConvertZeroToEmpty().ToTD(cssclass);
            this.gvDetail.InnerHtml += dtBySummary.Rows[i]["GRADE_B"].ConvertZeroToEmpty().ToTD(cssclass);
            this.gvDetail.InnerHtml += dtBySummary.Rows[i]["DISCREPANCY_QTY"].ConvertZeroToEmpty().ToTD(cssclass);
            this.gvDetail.InnerHtml += "</tr>";

            totalOrder += Convert.ToInt32(dtBySummary.Rows[i]["ORDER_QTY"].ToString());
            totalActual += Convert.ToInt32(dtBySummary.Rows[i]["ACTUAL_QTY"].ToString());
            totalScanPack += Convert.ToInt32(dtBySummary.Rows[i]["SCAN_QTY"].ToString());
            gradeA += Convert.ToInt32(dtBySummary.Rows[i]["LEFTOVER_A"].ToString());
            diff += Convert.ToInt32(dtBySummary.Rows[i]["DIFF"].ToString());
            maxPack += dtBySummary.Rows[i]["MAX_SCAN_PACK"].ToInt("t");
            minPack += dtBySummary.Rows[i]["MIN_SCAN_PACK"].ToInt("c");
            balance += Convert.ToDecimal(dtBySummary.Rows[i]["AVAILABLE_BALANCE_SCAN_PACK_QTY"].ToString());
            //overPack += Convert.ToDecimal(dtBySummary.Rows[i]["OVER_PACK"].ToString());
            //shortPack += Convert.ToDecimal(dtBySummary.Rows[i]["SHORT_PACK"].ToString());
            overPack += dtBySummary.Rows[i]["OVER_PACK"].ToInt("t");
            shortPack += dtBySummary.Rows[i]["SHORT_PACK"].ToInt("t");
            gradeB += Convert.ToInt32(dtBySummary.Rows[i]["GRADE_B"].ToString());
            discrepancy += Convert.ToInt32(dtBySummary.Rows[i]["DISCREPANCY_QTY"].ToString());
        }

        zero_percent = (Convert.ToInt32(totalScanPack) > 0) ? (Convert.ToString(Math.Round((Convert.ToDecimal(totalScanPack) - Convert.ToDecimal(totalOrder)) / Convert.ToDecimal(totalOrder)*100, 2)) + "%") : "0%";

        maxPack = (totalOrder * (percent_Over_Allowed + 100) / 100).ToInt("f");
        minPack = (totalOrder * (100 - percent_Short_Allowed) / 100).ToInt("c");
        balance = maxPack - totalScanPack;
        this.gvDetail.InnerHtml += " <tr class='tr2style'><td>TOTAL BY JO:</td><td></td><td>" + totalOrder + "</td><td>" + totalActual + "</td><td>" + totalScanPack + "</td><td>" + gradeA.ConvertZeroToEmpty() + "</td>";
        this.gvDetail.InnerHtml += " <td>" + zero_percent + "</td>";
        this.gvDetail.InnerHtml += " <td>" + diff + "</td><td>" + maxPack + "</td><td>" + minPack + "</td><td>" + (balance < 0 ? 0 : balance).ConvertZeroToEmpty() + "</td><td>" + overPack + "</td><td>" + shortPack + "</td><td>" + gradeB.ConvertZeroToEmpty() + "</td><td>" + discrepancy.ConvertZeroToEmpty() + "</td></tr>";
        this.gvDetail.InnerHtml += " </table></td></tr><tr><td>&nbsp;</td></tr>";//空一行,美观;

        ApproveJO += dtBySummary.Rows[0]["COMBINE_JO_NO"].ToString() + ",";
        ApproveScanPack += dtBySummary.Rows[0]["COMBINE_JO_NO"].ToString() + "_" + totalScanPack + ",";
        //ApproveScanPackPer += dtBySummary.Rows[0]["COMBINE_JO_NO"].ToString() + "_" + (totalActual == 0 ? "0" : Convert.ToString(Math.Round((Convert.ToDecimal(totalScanPack) - Convert.ToDecimal(totalActual)) / Convert.ToDecimal(totalActual) * 100, 2))) + ",";
        ApproveScanPackPer += dtBySummary.Rows[0]["COMBINE_JO_NO"].ToString() + "_" + (totalOrder == 0 ? "0" : Convert.ToString(Math.Round((Convert.ToDecimal(totalScanPack) - Convert.ToDecimal(totalOrder)) / Convert.ToDecimal(totalOrder) * 100, 2))) + ",";
        //ApproveScanPackPer += dtBySummary.Rows[0]["COMBINE_JO_NO"].ToString() + "_" + Convert.ToString(Math.Round((Convert.ToDecimal(totalScanPack) - Convert.ToDecimal(totalActual)) / Convert.ToDecimal(totalActual) * 100, 2)) + ",";
        
    }

    private void ShowJOByLot(DataTable dtByLot)
    {
        DataRow[] dr;
        string OldJO = "";
        int totalOrder = 0;
        int totalActual = 0;
        int totalScanPack = 0;
        int gradeA = 0;
        int diff = 0;
        Decimal maxPack = 0;
        Decimal minPack = 0;
        int balance = 0;
        string zero_percent = "";

        this.gvDetail.InnerHtml += " <tr><td>&nbsp;</td></tr><tr><td><table width='95%' border='1' cellspacing='0' cellpadding='4' style='font-size: 12px; border-collapse: collapse'>";

        for(int i=0;i<dtByLot.Rows.Count;i++)
        {   
            if (dtByLot.Rows[i]["ORIGINAL_JO_NO"].ToString() != OldJO)
            {
              
                if (dtByLot.Rows[i]["ORIGINAL_JO_NO"].ToString() != OldJO && OldJO != "")
                {

                    zero_percent = (Convert.ToInt32(totalScanPack) > 0) ? (Convert.ToString(Math.Round((Convert.ToDecimal(totalScanPack) - Convert.ToDecimal(totalOrder)) / Convert.ToDecimal(totalOrder)*100,2)) + "%") : "0%";
                    maxPack = (totalOrder * (percent_Over_Allowed + 100) / 100).ToInt("f");
                    minPack = (totalOrder * (100 - percent_Short_Allowed) / 100).ToInt("c");
                    balance = maxPack.ToInt("f") - totalScanPack;
                    this.gvDetail.InnerHtml += " <tr class='tr2style'><td>TOTAL BY LOT:</td><td></td><td>" + totalOrder + "</td><td>" + totalActual + "</td><td>" + totalScanPack + "</td><td>" + gradeA + "</td>";
                    this.gvDetail.InnerHtml += " <td>" + zero_percent + "</td>";
                    this.gvDetail.InnerHtml += " <td>" + diff + "</td><td>" + maxPack + "</td><td>" + minPack + "</td><td>" + balance + "</td></tr>";
                    this.gvDetail.InnerHtml += " <tr><td colspan='11'>&nbsp;</td></tr>";//空一行,美观;
                }

                totalOrder = 0;
                totalActual = 0;
                totalScanPack = 0;
                gradeA = 0;
                diff = 0;
                maxPack = 0;
                minPack = 0;
                balance = 0;

                this.gvDetail.InnerHtml += " <tr>";
                this.gvDetail.InnerHtml += " <td width='100'class='tr2style'>LOT: </td>";
                this.gvDetail.InnerHtml += " <td width='100'>" + dtByLot.Rows[i]["ORIGINAL_JO_NO"].ToString() + " </td>";
                this.gvDetail.InnerHtml += " <td width='100'class='tr2style'>Over/ Short Ship% : </td>";
                this.gvDetail.InnerHtml += " <td width='100' colspan='8'>" + dtByLot.Rows[i]["ALLOWED_PERCENT"].ToString() + " </td></tr>";

                switch (strculture)
                {//选择显示语言;
                    case "en":
                        this.gvDetail.InnerHtml += " <tr><td class='tr2style' width='100'>Color</td><td class='tr2style' width='100'>Size</td><td class='tr2style' width='100'>Order Qty</td><td class='tr2style' width='100'>Actual Qty</td><td class='tr2style' width='100'>Scan Pack Qty</td><td class='tr2style' width='100'>Grade A Leftover</td>";
                        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>Percentage</td><td class='tr2style' width='100'>Diferrences</td><td class='tr2style' width='100'>Max Scan Pack</td><td class='tr2style' width='100'>Min Scan Pack</td><td class='tr2style' width='100'>Available Balance</td></tr>";
                        break;
                    case "zh":
                        this.gvDetail.InnerHtml += " <tr><td class='tr2style' width='100'>颜色</td><td class='tr2style' width='100'>尺码</td><td class='tr2style' width='100'>订单数</td><td class='tr2style' width='100'>实裁数</td><td class='tr2style' width='100'>扫描数</td><td class='tr2style' width='100'>结存正品数</td>";
                        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>百分比</td><td class='tr2style' width='100'>差异</td><td class='tr2style' width='100'>最多出货数</td><td class='tr2style' width='100'>最小出货数</td><td class='tr2style' width='100'>还可以出货数量</td></tr>";
                        break;
                }

                OldJO = dtByLot.Rows[i]["ORIGINAL_JO_NO"].ToString();
            }

            dr = dtByLot.Select("COLOR_CODE='" + dtByLot.Rows[i]["COLOR_CODE"].ToString() + "' AND ORIGINAL_JO_NO='" + dtByLot.Rows[i]["ORIGINAL_JO_NO"].ToString() + "'");

            this.gvDetail.InnerHtml += " <tr><td rowspan='" + dr.Count() + "'>" + dtByLot.Rows[i]["COLOR_CODE"].ToString() + " (" + dtByLot.Rows[i]["COLOR_DESC"].ToString() + ")" + "</td>";


            for (int j = 0; j < dr.Count(); j++)
            {
                
                //this.gvDetail.InnerHtml += " <td>" + dtByLot.Rows[i]["SIZE_CODE"].ToString() + "</td>";
                //this.gvDetail.InnerHtml += " <td>" + dtByLot.Rows[i]["ORDER_QTY"].ToString() + "</td>";
                //this.gvDetail.InnerHtml += " <td>" + dtByLot.Rows[i]["ACTUAL_QTY"].ToString() + "</td>";
                //this.gvDetail.InnerHtml += " <td>" + dtByLot.Rows[i]["SCAN_QTY"].ToString() + "</td>";
                //this.gvDetail.InnerHtml += " <td>" + dtByLot.Rows[i]["LEFTOVER_A"].ToString() + "</td>";
                //this.gvDetail.InnerHtml += " <td>" + dtByLot.Rows[i]["PERCENTAGE"].ToString() + "</td>";
                //this.gvDetail.InnerHtml += " <td>" + dtByLot.Rows[i]["DIFF"].ToString() + "</td>";
                //this.gvDetail.InnerHtml += " <td>" + dtByLot.Rows[i]["MAX_SCAN_PACK"].ToInt("t") + "</td>";
                //this.gvDetail.InnerHtml += " <td>" + dtByLot.Rows[i]["MIN_SCAN_PACK"].ToInt("c") + "</td>";
                //this.gvDetail.InnerHtml += " <td>" + dtByLot.Rows[i]["STILL_CAN_PACK"].ToString() + "</td></tr>";

                string cssclass = getTrClass(dtByLot.Rows[i]["PERCENTAGE"].ToDouble());
                this.gvDetail.InnerHtml += dtByLot.Rows[i]["SIZE_CODE"].ToTD(cssclass);
                this.gvDetail.InnerHtml += dtByLot.Rows[i]["ORDER_QTY"].ToTD(cssclass);
                this.gvDetail.InnerHtml += dtByLot.Rows[i]["ACTUAL_QTY"].ToTD(cssclass);
                this.gvDetail.InnerHtml += dtByLot.Rows[i]["SCAN_QTY"].ToTD(cssclass);
                this.gvDetail.InnerHtml += dtByLot.Rows[i]["LEFTOVER_A"].ConvertZeroToEmpty().ToTD(cssclass);
                this.gvDetail.InnerHtml += dtByLot.Rows[i]["PERCENTAGE"].ToPersent().ToTD(cssclass);
                this.gvDetail.InnerHtml += dtByLot.Rows[i]["DIFF"].ToTD(cssclass);
                this.gvDetail.InnerHtml += dtByLot.Rows[i]["MAX_SCAN_PACK"].ToInt("t").ToTD(cssclass);
                this.gvDetail.InnerHtml += dtByLot.Rows[i]["MIN_SCAN_PACK"].ToInt("c").ToTD(cssclass);
                this.gvDetail.InnerHtml += dtByLot.Rows[i]["STILL_CAN_PACK"].ToInt("t").ConvertZeroToEmpty().ToTD(cssclass);
                this.gvDetail.InnerHtml += "</tr>";

                totalOrder += Convert.ToInt32(dtByLot.Rows[i]["ORDER_QTY"].ToString());
                totalActual += Convert.ToInt32(dtByLot.Rows[i]["ACTUAL_QTY"].ToString());
                totalScanPack += Convert.ToInt32(dtByLot.Rows[i]["SCAN_QTY"].ToString());
                gradeA += Convert.ToInt32(dtByLot.Rows[i]["LEFTOVER_A"].ToString());
                diff += Convert.ToInt32(dtByLot.Rows[i]["DIFF"].ToString());
                //maxPack += dtByLot.Rows[i]["MAX_SCAN_PACK"].ToInt("t");
                //minPack +=dtByLot.Rows[i]["MIN_SCAN_PACK"].ToInt("c");
                //balance += Convert.ToInt32(dtByLot.Rows[i]["STILL_CAN_PACK"].ToString());

                i++;
            }

            i--;
        }

        zero_percent = (Convert.ToInt32(totalScanPack) > 0) ? (Convert.ToString(Math.Round((Convert.ToDecimal(totalScanPack) - Convert.ToDecimal(totalOrder)) / Convert.ToDecimal(totalOrder)*100,2)) + "%") : "0%";
        
        maxPack = (totalOrder * (percent_Over_Allowed + 100) / 100).ToInt("f");
        minPack = (totalOrder * (100 - percent_Short_Allowed) / 100).ToInt("c");
        balance = maxPack.ToInt("f") - totalScanPack;
        this.gvDetail.InnerHtml += " <tr class='tr2style'><td>TOTAL BY LOT:</td><td></td><td>" + totalOrder + "</td><td>" + totalActual + "</td><td>" + totalScanPack + "</td><td>" + gradeA + "</td>";
        this.gvDetail.InnerHtml += " <td>" + zero_percent + "</td>";
        this.gvDetail.InnerHtml += " <td>" + diff + "</td><td>" + maxPack + "</td><td>" + minPack + "</td><td>" + balance + "</td></tr>";
        this.gvDetail.InnerHtml += " </table>";
        this.gvDetail.InnerHtml += " </td></tr>";
        this.gvDetail.InnerHtml += " <tr><td>&nbsp;</td></tr>";//空一行,美观;
    }
    private void SetOverAndShortPack(DataRow dr)
    {
        dr["OVER_PACK"] = 0;
        dr["SHORT_PACK"] = 0;
        if (dr["SCAN_QTY"].ToInt("t") - dr["MAX_SCAN_PACK"].ToInt("f") >= 0)
        {
            dr["OVER_PACK"] = dr["SCAN_QTY"].ToInt("t") - dr["MAX_SCAN_PACK"].ToInt("f");
        }
        else
        {
            if (dr["SCAN_QTY"].ToInt("t") - dr["MIN_SCAN_PACK"].ToInt("c") < 0)
            {
                dr["SHORT_PACK"] = dr["SCAN_QTY"].ToInt("t") - dr["MIN_SCAN_PACK"].ToInt("c");
            }
        }
    }

    private void ShowJOByBPO(DataTable dtByBPO)
    {
        DataRow[] dr;
        string OldBPO = "";
        int totalOrder = 0;
        int totalScanPack = 0;
        Decimal maxPack = 0;
        Decimal minPack = 0;
        int balance = 0;
        string zero_percent = "";

        this.gvDetail.InnerHtml += " <tr><td>&nbsp;</td></tr><tr><td><table width='95%' border='1' cellspacing='0' cellpadding='4' style='font-size: 12px; border-collapse: collapse'>";

        for(int i=0;i<dtByBPO.Rows.Count;i++)
        {   
            if (dtByBPO.Rows[i]["BUYER_PO"].ToString() != OldBPO)
            {
                
                if (dtByBPO.Rows[i]["BUYER_PO"].ToString() != OldBPO && OldBPO != "")
                {
                    zero_percent = (Convert.ToInt32(totalScanPack) > 0) ? (Convert.ToString(Math.Round((Convert.ToDecimal(totalScanPack) - Convert.ToDecimal(totalOrder)) / Convert.ToDecimal(totalOrder)*100,2)) + "%") : "0%";
                     maxPack = (totalOrder * (percent_Over_Allowed + 100) / 100).ToInt("f");
                     minPack = (totalOrder * (100 - percent_Short_Allowed) / 100).ToInt("c");
                     balance = maxPack.ToInt("f") - totalScanPack;
                    this.gvDetail.InnerHtml += " <tr class='tr2style'><td>TOTAL BY Buyer PO:</td><td></td><td>" + totalOrder + "</td><td>" + totalScanPack + "</td>";
                    this.gvDetail.InnerHtml += " <td>" + zero_percent + "</td>";
                    this.gvDetail.InnerHtml += " <td>" + maxPack + "</td><td>" + minPack + "</td><td>" + balance + "</td></tr>";
                    this.gvDetail.InnerHtml += " <tr><td colspan='8'>&nbsp;</td></tr>";//空一行,美观;
                }

                totalOrder = 0;
                totalScanPack = 0;
                maxPack = 0;
                minPack = 0;
                balance = 0;

                this.gvDetail.InnerHtml += " <tr>";
                this.gvDetail.InnerHtml += " <td width='100'class='tr2style'>Buyer PO: </td>";
                this.gvDetail.InnerHtml += " <td width='100' colspan='7'>" + dtByBPO.Rows[i]["BUYER_PO"].ToString() + " </td></tr>";

                switch (strculture)
                {//选择显示语言;
                    case "en":
                        this.gvDetail.InnerHtml += " <tr><td class='tr2style' width='100'>Color</td><td class='tr2style' width='100'>Size</td><td class='tr2style' width='100'>Order Qty</td><td class='tr2style' width='100'>Scan Pack Qty</td>";
                        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>Percentage</td><td class='tr2style' width='100'>Max Scan Pack</td><td class='tr2style' width='100'>Min Scan Pack</td><td class='tr2style' width='100'>Available Balance</td></tr>";
                        break;
                    case "zh":
                        this.gvDetail.InnerHtml += " <tr><td class='tr2style' width='100'>颜色</td><td class='tr2style' width='100'>尺码</td><td class='tr2style' width='100'>订单数</td><td class='tr2style' width='100'>扫描数</td>";
                        this.gvDetail.InnerHtml += " <td class='tr2style' width='100'>百分比</td><td class='tr2style' width='100'>最多出货数</td><td class='tr2style' width='100'>最小出货数</td><td class='tr2style' width='100'>还可以出货数量</td></tr>";
                        break;
                }

                OldBPO = dtByBPO.Rows[i]["BUYER_PO"].ToString();
            }

            dr = dtByBPO.Select("COLOR_CODE='" + dtByBPO.Rows[i]["COLOR_CODE"].ToString() + "' AND ORIGINAL_JO_NO='" + dtByBPO.Rows[i]["ORIGINAL_JO_NO"].ToString() + "' AND BUYER_PO='" + dtByBPO.Rows[i]["BUYER_PO"].ToString() + "'");

            this.gvDetail.InnerHtml += " <tr><td rowspan='" + dr.Count() + "'>" + dtByBPO.Rows[i]["COLOR_CODE"].ToString() + " (" + dtByBPO.Rows[i]["COLOR_DESC"].ToString() + ")" + "</td>";


            for (int j = 0; j < dr.Count(); j++)
            {
               
                //this.gvDetail.InnerHtml += " <td>" + dtByBPO.Rows[i]["SIZE_CODE"].ToString() + "</td>";
                //this.gvDetail.InnerHtml += " <td>" + dtByBPO.Rows[i]["ORDER_QTY"].ToString() + "</td>";
                //this.gvDetail.InnerHtml += " <td>" + dtByBPO.Rows[i]["SCAN_QTY"].ToString() + "</td>";
                //this.gvDetail.InnerHtml += " <td>" + dtByBPO.Rows[i]["PERCENTAGE"].ToString() + "</td>";
                //this.gvDetail.InnerHtml += " <td>" + dtByBPO.Rows[i]["MAX_SCAN_PACK"].ToInt("t") + "</td>";
                //this.gvDetail.InnerHtml += " <td>" + dtByBPO.Rows[i]["MIN_SCAN_PACK"].ToInt("c") + "</td>";
                //this.gvDetail.InnerHtml += " <td>" + dtByBPO.Rows[i]["STILL_CAN_PACK"].ToString() + "</td></tr>";

                string cssclass = getTrClass(dtByBPO.Rows[i]["PERCENTAGE"].ToDouble());
                this.gvDetail.InnerHtml += dtByBPO.Rows[i]["SIZE_CODE"].ToTD(cssclass);
                this.gvDetail.InnerHtml += dtByBPO.Rows[i]["ORDER_QTY"].ToTD(cssclass);
                this.gvDetail.InnerHtml += dtByBPO.Rows[i]["SCAN_QTY"].ToTD(cssclass);
                this.gvDetail.InnerHtml += dtByBPO.Rows[i]["PERCENTAGE"].ToPersent().ToTD(cssclass);
                this.gvDetail.InnerHtml += dtByBPO.Rows[i]["MAX_SCAN_PACK"].ToInt("t").ToTD(cssclass);
                this.gvDetail.InnerHtml += dtByBPO.Rows[i]["MIN_SCAN_PACK"].ToInt("c").ToTD(cssclass);
                this.gvDetail.InnerHtml += dtByBPO.Rows[i]["STILL_CAN_PACK"].ToInt("t").ConvertZeroToEmpty().ToTD(cssclass);
                this.gvDetail.InnerHtml += "</tr>";

                totalOrder += Convert.ToInt32(dtByBPO.Rows[i]["ORDER_QTY"].ToString());
                totalScanPack += Convert.ToInt32(dtByBPO.Rows[i]["SCAN_QTY"].ToString());
                //maxPack += dtByBPO.Rows[i]["MAX_SCAN_PACK"].ToInt("t");
                //minPack += dtByBPO.Rows[i]["MIN_SCAN_PACK"].ToInt("c");
                //balance += Convert.ToInt32(dtByBPO.Rows[i]["STILL_CAN_PACK"].ToString());

                i++;
            }

            i--;
        }

        zero_percent = (Convert.ToInt32(totalScanPack) > 0) ? (Convert.ToString(Math.Round((Convert.ToDecimal(totalScanPack) - Convert.ToDecimal(totalOrder)) / Convert.ToDecimal(totalOrder)*100,2)) + "%") : "0%";
        maxPack = (totalOrder * (percent_Over_Allowed + 100) / 100).ToInt("f");
        minPack = (totalOrder * (100 - percent_Short_Allowed) / 100).ToInt("c");
        balance = maxPack.ToInt("f") - totalScanPack;
        this.gvDetail.InnerHtml += " <tr class='tr2style'><td>TOTAL BY Buyer PO:</td><td></td><td>" + totalOrder + "</td><td>" + totalScanPack + "</td>";
        this.gvDetail.InnerHtml += " <td>" + zero_percent + "</td>";
        this.gvDetail.InnerHtml += " <td>" + maxPack + "</td><td>" + minPack + "</td><td>" + balance + "</td></tr>";
        this.gvDetail.InnerHtml += " </table>";
        this.gvDetail.InnerHtml += " </td></tr>";
        this.gvDetail.InnerHtml += " <tr><td>&nbsp;</td></tr>";//空一行,美观;
    }
}
