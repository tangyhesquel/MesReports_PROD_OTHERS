using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Drawing;

public partial class Reports_ShipmentClosingReport : pPage
{
    private static string Site = string.Empty;
    string over_short_percent = "";
    string over_per = "";
    string short_per = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["site"] == null)
            return;
        else
            Site = Request.QueryString["site"].ToString().ToUpper();

        if (!IsPostBack)
        {
            Init();
        }
    }

    private void Init()
    {
        string FactoryCd = string.Empty;
        string JoNo = string.Empty;
        string SCNo = string.Empty;
        string SummaryType = string.Empty;

        if (Request.QueryString["site"] != null)
        {
            ddlFactory.DataSource = MESComment.MesRpt.GetFactoryCd();
            ddlFactory.DataTextField = "FACTORY_ID";
            ddlFactory.DataValueField = "FACTORY_ID";
            ddlFactory.DataBind();

            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                FactoryCd = Request.QueryString["site"].ToString().ToUpper();
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
        if (Request.QueryString["SummaryType"] != null)
        {
            SummaryType = Request.QueryString["SummaryType"].ToString();
        }

        if (FactoryCd.ToUpper().Equals("EAV") || FactoryCd.ToUpper().Equals("EGV"))
        {
            this.DDSummaryBy.SelectedValue = "GO";
        }

        this.txtJoNo.Text = JoNo.ToString();
        this.txtGO.Text = SCNo.ToString();
        //this.DDSummaryBy.SelectedValue = SummaryType.ToString();
        if (JoNo.ToString() != "" || SCNo.ToString() != "" || SummaryType.ToString() != "")
        {
            queryDiv.Visible = false;
            btnQuery.Click += new EventHandler(btnQuery_Click);
        }
    }

    private void SelectGO(object sender, RepeaterItemEventArgs e)
    {
        string SC_NO = DataBinder.Eval(e.Item.DataItem, "SC_NO").ToString();
        //JOHeader
        Repeater rpHeader = e.Item.FindControl("rpHeader") as Repeater;
        DataTable dtHeader = MESComment.ShipmentClosingSQL.GetShipmentCloseHeaderGO(SC_NO, Site);
        rpHeader.DataSource = dtHeader;
        rpHeader.DataBind();

        if (dtHeader.Rows[0]["ShippmentAllowance"].ToString() != "") //add by LimML on 20150910 - handle error when no input wrong GO
        {
            //Added by MF on 20150720, fore color changes
            if (dtHeader.Rows[0]["ShippmentAllowance"].ToString() != "")
            {
                over_short_percent = dtHeader.Rows[0]["ShippmentAllowance"].ToString();

                if (over_short_percent.Contains("+/-"))
                {
                    over_per = over_short_percent.Substring(3, 4);
                    short_per = over_short_percent.Substring(3, 4);
                }
                else
                {
                    over_per = over_short_percent.Substring(over_short_percent.IndexOf("+") + 1, 4);
                    short_per = over_short_percent.Substring(over_short_percent.IndexOf("-") + 1, 4);
                }
            }
        }
        //End of added by MF on 20150720, fore color changes


        // 尺寸列表 
        DataTable dtSize = MESComment.ShipmentClosingSQL.GetReduceQuantitySizeGO(SC_NO);
        if (dtSize == null)
            return;

        SizeCOL = dtSize.Rows[0]["SIZE"].ToString().Replace("[", "").Replace("]", "").Split(new char[] { ',' });
        if (SizeCOL.Length < 1)
            return;

        //JOList  主体数据
        DataTable dt = MESComment.ShipmentClosingSQL.GetShipmentClosingGO(SC_NO, Site, dtSize.Rows[0]["SIZE"].ToString(), dtSize.Rows[0]["SIZE_NULL"].ToString(), dtSize.Rows[0]["SIZE_EMPTY"].ToString());
        if (dt == null)
            return;
        if (dt.Rows.Count == 0) //add by LimML on 20150910 - handle error when no input wrong GO
            return;
        dt.Columns.Remove("SEQ");

        
        DataTable dtJOList = MESComment.ShipmentClosingSQL.GetJO("", SC_NO, Site);
        string joList = "";
        for (int i = 0; i < dtJOList.Rows.Count; i++)
        {
            if (i==0)
                joList = "'" + dtJOList.Rows[i]["JO_NO"].ToString() + "'";
            else
            joList = joList + ",'" + dtJOList.Rows[i]["JO_NO"].ToString() + "'";
        }

        // Scan_pack_Qty 值
        //DataTable dtScan = MESComment.ShipmentClosingSQL.GetScanPackQTYGO(SC_NO, dtSize.Rows[0]["SIZE_NULL"].ToString(), dtSize.Rows[0]["SIZE"].ToString(), joList);
        DataTable dtScan = MESComment.ShipmentClosingSQL.GetScanPackQTYGO(SC_NO, SizeCOL, joList);

        // Ship_Qty 值
        DataTable dtShip = MESComment.ShipmentClosingSQL.GetShipQTYGO(SC_NO, Site);

        //Leftover Grade A and B 值
        //Added by MF on 20150609, change the source for Leftover grade A and grade B
        DataTable dtGradeA_B = MESComment.ShipmentClosingSQL.GetLeftoverGradeAGradeBGO(joList, Site);


        //合并到主体dt数据
        string value = string.Empty;
        if (dtScan != null)
        {
            foreach (DataRow row in dtScan.Rows)
            {
                value = row["GOColor"].ToString();
                DataRow[] dr = dt.Select("Description='Scan_pack_Qty'and (COLOR_CD='" + value + "' or COLOR_DESC='" + value + "' )"); //数据库有CASE WHEN GOCOLOR, COLOR 的情况
                DataRow[] dr1 = dt.Select("Description='Unaccountable_Qty'and (COLOR_CD='" + value + "' or COLOR_DESC='" + value + "' )");

                if (dr.Length > 0)
                {
                    foreach (string item in SizeCOL)
                    {
                        dr[0][item] = row[item];
                        //dr1[0][item] = Convert.ToInt32(dr1[0][item]) - Convert.ToInt32(row[item]);
                        dr1[0][item] = dr1[0].ToInt("r") - row[item].ToInt("r");
                    }
                    dr[0]["Total"] = row["Total"];
                    //dr1[0]["Total"] = Convert.ToInt32(dr1[0]["Total"]) - Convert.ToInt32(row["Total"]);
                    dr1[0]["total"] = dr1[0]["Total"].ToInt("r") - row["Total"].ToInt("r");
                }
            }
        }

        DataTable dtDistinct = dt.DefaultView.ToTable(true, "COLOR_CD");
        int Ship = 0;
        int ShortShip = 0;
        if (dtShip != null)
        {
            foreach (DataRow row in dtDistinct.Rows)
            {
                DataRow[] dr = dt.Select("Description='Ship_Qty'and Color_CD='" + row["Color_CD"] + "'");
                DataRow[] dr2 = dt.Select("Description='Over_ShortShip_Qty'and Color_CD='" + row["Color_CD"] + "'"); //Over_ShortShip_Qty
                DataRow[] dr3 = dt.Select("Description='Order_Qty'and Color_CD='" + row["Color_CD"] + "'"); //Order_Qty
                DataRow[] dr4 = dt.Select("Description='Over_ShortShip_%'and Color_CD='" + row["Color_CD"] + "'"); //Over_ShortShip_%
                DataRow[] dr5 = dt.Select("Description='Over/Short_Cut_Qty'and Color_CD='" + row["Color_CD"] + "'");
                DataRow[] dr6 = dt.Select("Description='Over/Short_Cut%'and Color_CD='" + row["Color_CD"] + "'");

                // Over_ShortShip_Qty = Ship_Qty - Order_Qty
                // Over_ShortShip_Per = Over_ShortShip_Qty / Order_Qty
                foreach (string col in SizeCOL)
                {
                    DataRow[] dr1 = dtShip.Select("Color_CD='" + row["Color_CD"] + "' and Size_CD='" + col + "'");
                    if (dr.Length > 0 && dr1.Length > 0)
                    {
                        dr[0][col] = dr1[0]["QTY"];
                        Ship += Convert.ToInt32(dr1[0]["QTY"]);
                        if (dr2.Length > 0 && dr1.Length > 0)
                        {
                            dr2[0][col] = Convert.ToInt32(dr1[0]["QTY"]) - Convert.ToInt32(dr3[0][col]);
                            ShortShip += Convert.ToInt32(dr1[0]["QTY"]) - Convert.ToInt32(dr3[0][col]);
                            if (!"0".Equals(dr3[0][col]))
                                dr4[0][col] = Math.Round((Convert.ToDecimal(dr2[0][col]) / Convert.ToDecimal(dr3[0][col]) * 100), 3);
                        }
                    }
                    else
                    { break; } //

                }
                dr[0]["Total"] = Ship;
                dr2[0]["total"] = ShortShip;
                if (!"0".Equals(dr3[0]["Total"]))   //Get Total Over_ShortShip%
                    dr4[0]["Total"] = Math.Round((Convert.ToDecimal(dr2[0]["Total"]) / Convert.ToDecimal(dr3[0]["Total"]) * 100), 3);
                if (!"0".Equals(dr5[0]["Total"]))   //Get Total Over/Short_Cut%
                    dr6[0]["Total"] = Math.Round((Convert.ToDecimal(dr5[0]["Total"]) / Convert.ToDecimal(dr3[0]["Total"]) * 100), 3);
            }
        }

        //Added by MF on 20150609, change source for Leftover Grade A and Grade B
        if (dtGradeA_B != null)
        {
            foreach (DataRow row in dtDistinct.Rows)
            {
                DataRow[] dr = dt.Select("Description='Leftover_Garment_A'and Color_CD='" + row["Color_CD"] + "'");
                DataRow[] dr1 = dt.Select("Description='B_Grade_Qty'and Color_CD='" + row["Color_CD"] + "'");

                int a_ttl_each_color = 0;
                int b_ttl_each_color = 0;
                foreach (string col in SizeCOL)
                {
                    DataRow[] gradeA = dtGradeA_B.Select("COLOR_CODE='" + row["Color_CD"] + "' and SIZE_CODE='" + col + "' and GRADE='A'");
                    if (gradeA.Length > 0 && dr.Length > 0)
                    {
                        dr[0][col] = gradeA[0]["QTY"];
                        a_ttl_each_color += Convert.ToInt32(gradeA[0]["QTY"]);
                    }

                    DataRow[] gradeB = dtGradeA_B.Select("COLOR_CODE='" + row["Color_CD"] + "' and SIZE_CODE='" + col + "' and GRADE='B'");
                    if (gradeB.Length > 0 && dr1.Length > 0)
                    {
                        dr1[0][col] = gradeB[0]["QTY"];
                        b_ttl_each_color += Convert.ToInt32(gradeB[0]["QTY"]);
                    }
                }
                dr[0]["Total"] = a_ttl_each_color;
                dr1[0]["total"] = b_ttl_each_color;
            }
        }

        dt.AcceptChanges();
        GridView gvList = e.Item.FindControl("gvList") as GridView;

        gvList.DataSource = dt;
        gvList.DataBind();

        Repeater rpListFoot = e.Item.FindControl("rpListFoot") as Repeater;
        if (dt != null && dt.Rows.Count > 0)
        {
            DataTable dtFoot = new DataTable();
            dtFoot.Columns.AddRange(
                 new DataColumn[] 
                { 
                    new DataColumn("TOrder_QTY", typeof(Int32)), 
                    new DataColumn("TCut_QTY", typeof(Int32)),
                    new DataColumn("TCut_Reduce", typeof(Int32)),
                    new DataColumn("TActual_QTY", typeof(Int32)),
                    new DataColumn("TOverShortCut_QTY", typeof(Int32)),
                    new DataColumn("TOverShortCut_PER", typeof(string)),
                    new DataColumn("TScanPack_QTY", typeof(Int32)),
                    new DataColumn("TShip_QTY", typeof(Int32)),
                    new DataColumn("TOverShortShip_QTY", typeof(Int32)),
                    new DataColumn("TOverShortShip_PER", typeof(string)),
                    new DataColumn("TLeftOverGarment_A", typeof(Int32)),
                    new DataColumn("TBGrade_QTY", typeof(Int32)),
                    new DataColumn("TCGrade_QTY", typeof(Int32)),
                    new DataColumn("TUnaccountable_QTY", typeof(Int32)) 
                }
              );



            DataRow drGO = dtFoot.NewRow();
            drGO["TOrder_QTY"] = dt.Compute("SUM(Total)", "Description='Order_QTY'");
            drGO["TCut_QTY"] = dt.Compute("SUM(Total)", "Description='Cut_QTY'");
            drGO["TCut_Reduce"] = dt.Compute("SUM(Total)", "Description='Cut_Reduce'");
            drGO["TActual_QTY"] = dt.Compute("SUM(Total)", "Description='Actual_Cut-Qty'");
            drGO["TOverShortCut_QTY"] = dt.Compute("SUM(Total)", "Description='Over/Short_Cut_Qty'");
            drGO["TOverShortCut_PER"] = dt.Compute("SUM(Total)", "Description='Over/Short_Cut%'");
            drGO["TScanPack_QTY"] = dt.Compute("SUM(Total)", "Description='Scan_Pack_Qty'");
            drGO["TShip_QTY"] = dt.Compute("SUM(Total)", "Description='Ship_Qty'");
            drGO["TOverShortShip_QTY"] = dt.Compute("SUM(Total)", "Description='Over_ShortShip_Qty'");
            drGO["TOverShortShip_PER"] = dt.Compute("SUM(Total)", "Description='Over_ShortShip_%'");
            drGO["TLeftOverGarment_A"] = dt.Compute("SUM(Total)", "Description='Leftover_Garment_A'");
            drGO["TBGrade_QTY"] = dt.Compute("SUM(Total)", "Description='B_Grade_Qty'");
            drGO["TCGrade_QTY"] = dt.Compute("SUM(Total)", "Description='C_Grade_Qty'");
            drGO["TUnaccountable_QTY"] = dt.Compute("SUM(Total)", "Description='Unaccountable_Qty'");
            dtFoot.Rows.Add(drGO);

            rpListFoot.DataSource = dtFoot;
            rpListFoot.DataBind();

            for (int rowIndex = gvList.Rows.Count - 1; rowIndex > 0; rowIndex--)
            {
                GridViewRow previousRow = gvList.Rows[rowIndex - 1];
                GridViewRow row = gvList.Rows[rowIndex];

                row.Cells[0].RowSpan = (row.Cells[0].RowSpan == 0) ? 1 : row.Cells[0].RowSpan;
                previousRow.Cells[0].RowSpan = (previousRow.Cells[0].RowSpan == 0) ? 1 : previousRow.Cells[0].RowSpan;

                if (row.Cells[0].Text == previousRow.Cells[0].Text)
                {
                    row.Cells[0].Visible = false;
                    previousRow.Cells[0].RowSpan += row.Cells[0].RowSpan;
                }
            }
        }


    }

    private static string[] SizeCOL;

    private void SelectJO(object sender, RepeaterItemEventArgs e)
    {
        string JoNo = DataBinder.Eval(e.Item.DataItem, "Jo_No").ToString();

        //JOHeader
        Repeater rpHeader = e.Item.FindControl("rpHeader") as Repeater;
        DataTable dtHeader = MESComment.ShipmentClosingSQL.GetShipmentCloseHeader(JoNo, Site);
        rpHeader.DataSource = dtHeader;
        rpHeader.DataBind();

        //Added by MF on 20150720, fore color changes
        if (dtHeader.Rows[0]["ShippmentAllowance"].ToString() != "")
        {
            over_short_percent = dtHeader.Rows[0]["ShippmentAllowance"].ToString();

            if (over_short_percent.Contains("+/-"))
            {
                over_per = over_short_percent.Substring(3, 4);
                short_per = over_short_percent.Substring(3, 4);
            }
            else
            {
                over_per = over_short_percent.Substring(over_short_percent.IndexOf("+")+1, 4);
                short_per = over_short_percent.Substring(over_short_percent.IndexOf("-")+1, 4);
            }
        }
        //End of added by MF on 20150720, fore color changes

        // 尺寸列表 
        DataTable dtSize = MESComment.ShipmentClosingSQL.GetReduceQuantitySize(JoNo);
        if (dtSize == null)
            return;

        SizeCOL = dtSize.Rows[0]["SIZE"].ToString().Replace("[", "").Replace("]", "").Split(new char[] { ',' });
        if (SizeCOL.Length < 1)
            return;

        //JOList  主体数据
        DataTable dt = MESComment.ShipmentClosingSQL.GetShipmentClosing(JoNo, Site, dtSize.Rows[0]["SIZE"].ToString(), dtSize.Rows[0]["SIZE_NULL"].ToString(), dtSize.Rows[0]["SIZE_EMPTY"].ToString());
        dt.Columns.Remove("SEQ");
        // Scan_pack_Qty 值
        //DataTable dtScan = MESComment.ShipmentClosingSQL.GetScanPackQTY(JoNo, dtSize.Rows[0]["SIZE_NULL"].ToString(), dtSize.Rows[0]["SIZE"].ToString());
        DataTable dtScan = MESComment.ShipmentClosingSQL.GetScanPackQTY(JoNo, SizeCOL, dtSize.Rows[0]["SIZE"].ToString());
        // Ship_Qty 值
        DataTable dtShip = MESComment.ShipmentClosingSQL.GetShipQTY(JoNo, Site);

        //Leftover Grade A and B 值
        //Added by MF on 20150609, change the source for Leftover grade A and grade B
        DataTable dtGradeA_B = MESComment.ShipmentClosingSQL.GetLeftoverGradeAGradeB(JoNo, Site);

        //合并到主体数据
        string value = string.Empty;
        foreach (DataRow row in dtScan.Rows)
        {
            value = row["GOColor"].ToString();
            DataRow[] dr = dt.Select("Description='Scan_pack_Qty'and (COLOR_CD='" + value + "' or COLOR_DESC='" + value + "' )"); //数据库有CASE WHEN GOCOLOR, COLOR 的情况
            DataRow[] dr1 = dt.Select("Description='Unaccountable_Qty'and (COLOR_CD='" + value + "' or COLOR_DESC='" + value + "' )"); //Unaccountable_Qty
            if (dr.Length > 0)
            {
                foreach (string item in SizeCOL)
                {
                    dr[0][item] = row[item];
                    //dr1[0][item] = Convert.ToInt32(dr1[0][item]) - Convert.ToInt32(row[item]);
                    dr1[0][item] = dr1[0].ToInt("r") - row[item].ToInt("r");
                }
                dr[0]["Total"] = row["Total"];
                //dr1[0]["Total"] = Convert.ToInt32(dr1[0]["Total"]) - Convert.ToInt32(row["Total"]);
                dr1[0]["Total"] = dr1[0]["Total"].ToInt("r") - row["Total"].ToInt("r");
            }
        }

        DataTable dtDistinct = dt.DefaultView.ToTable(true, "COLOR_CD");
   
        if (dtShip != null)
        {
            foreach (DataRow row in dtDistinct.Rows)
            {
                int Ship = 0;
                int ShortShip = 0;
                DataRow[] dr = dt.Select("Description='Ship_Qty'and Color_CD='" + row["Color_CD"] + "'");
                DataRow[] dr2 = dt.Select("Description='Over_ShortShip_Qty'and Color_CD='" + row["Color_CD"] + "'"); //Over_ShortShip_Qty
                DataRow[] dr3 = dt.Select("Description='Order_Qty'and Color_CD='" + row["Color_CD"] + "'"); //Order_Qty
                DataRow[] dr4 = dt.Select("Description='Over_ShortShip_%'and Color_CD='" + row["Color_CD"] + "'"); //Over_ShortShip_%
                DataRow[] dr5 = dt.Select("Description='Over/Short_Cut_Qty'and Color_CD='" + row["Color_CD"] + "'");
                DataRow[] dr6 = dt.Select("Description='Over/Short_Cut%'and Color_CD='" + row["Color_CD"] + "'");
                
                // Over_ShortShip_Qty = Ship_Qty - Order_Qty
                // Over_ShortShip_Per = Over_ShortShip_Qty / Order_Qty
                foreach (string col in SizeCOL)
                {
                    DataRow[] dr1 = dtShip.Select("Color_CD='" + row["Color_CD"] + "' and Size_CD='" + col + "'");
                    if (dr.Length > 0 && dr1.Length > 0)
                    {
                        dr[0][col] = dr1[0]["QTY"];
                        Ship += Convert.ToInt32(dr1[0]["QTY"]);
                        if (dr2.Length > 0 && dr1.Length > 0)
                        {
                            dr2[0][col] = Convert.ToInt32(dr1[0]["QTY"]) - Convert.ToInt32(dr3[0][col]);
                            ShortShip += Convert.ToInt32(dr1[0]["QTY"]) - Convert.ToInt32(dr3[0][col]);
                            if (!"0".Equals(dr3[0][col]))
                                dr4[0][col] = Math.Round((Convert.ToDecimal(dr2[0][col]) / Convert.ToDecimal(dr3[0][col]) * 100),3);
                        }
                    }
                    else
                    { break; } //

                }
                dr[0]["Total"] = Ship;
                dr2[0]["total"] = ShortShip;
                if (!"0".Equals(dr3[0]["Total"]))   //Get Total Over_ShortShip%
                    dr4[0]["Total"] = Math.Round((Convert.ToDecimal(dr2[0]["Total"]) / Convert.ToDecimal(dr3[0]["Total"]) * 100),3);
                if (!"0".Equals(dr5[0]["Total"]))   //Get Total Over/Short_Cut%
                    dr6[0]["Total"] = Math.Round((Convert.ToDecimal(dr5[0]["Total"]) / Convert.ToDecimal(dr3[0]["Total"]) * 100),3);
            }
        }
        
        //Added by MF on 20150609, change source for Leftover Grade A and Grade B
        if (dtGradeA_B != null)
        {
            foreach (DataRow row in dtDistinct.Rows)
            {
                DataRow[] dr = dt.Select("Description='Leftover_Garment_A'and Color_CD='" + row["Color_CD"] + "'");
                DataRow[] dr1 = dt.Select("Description='B_Grade_Qty'and Color_CD='" + row["Color_CD"] + "'");

                int a_ttl_each_color = 0;
                int b_ttl_each_color = 0;
                foreach (string col in SizeCOL)
                {
                    DataRow[] gradeA = dtGradeA_B.Select("COLOR_CODE='" + row["Color_CD"] + "' and SIZE_CODE='" + col + "' and GRADE='A'");
                    if (gradeA.Length > 0 && dr.Length > 0)
                    {
                        dr[0][col] = gradeA[0]["QTY"];
                        a_ttl_each_color += Convert.ToInt32(gradeA[0]["QTY"]);
                    }

                    DataRow[] gradeB = dtGradeA_B.Select("COLOR_CODE='" + row["Color_CD"] + "' and SIZE_CODE='" + col + "' and GRADE='B'");
                    if (gradeB.Length > 0 && dr1.Length > 0)
                    {
                        dr1[0][col] = gradeB[0]["QTY"];
                        b_ttl_each_color += Convert.ToInt32(gradeB[0]["QTY"]);
                    }
                }
                dr[0]["Total"] = a_ttl_each_color;
                dr1[0]["total"] = b_ttl_each_color;
            }
        }

        dt.AcceptChanges();
        GridView gvList = e.Item.FindControl("gvList") as GridView;

        if (dt != null && dt.Rows.Count > 0)
        {

            DataTable dtFoot = new DataTable();
            dtFoot.Columns.AddRange(
                 new DataColumn[] 
                { 
                    new DataColumn("TOrder_QTY", typeof(Int32)), 
                    new DataColumn("TCut_QTY", typeof(Int32)),
                    new DataColumn("TCut_Reduce", typeof(Int32)),
                    new DataColumn("TActual_QTY", typeof(Int32)),
                    new DataColumn("TOverShortCut_QTY", typeof(Int32)),
                    new DataColumn("TOverShortCut_PER", typeof(string)),
                    new DataColumn("TScanPack_QTY", typeof(Int32)),
                    new DataColumn("TShip_QTY", typeof(Int32)),
                    new DataColumn("TOverShortShip_QTY", typeof(Int32)),
                    new DataColumn("TOverShortShip_PER", typeof(decimal)),
                    new DataColumn("TLeftOverGarment_A", typeof(Int32)),
                    new DataColumn("TBGrade_QTY", typeof(Int32)),
                    new DataColumn("TCGrade_QTY", typeof(Int32)),
                    new DataColumn("TUnaccountable_QTY", typeof(Int32)) 
                }
              );

            //foreach (var item in foot)
            //{
            DataRow dr = dtFoot.NewRow();
            dr["TOrder_QTY"] = dt.Compute("SUM(Total)", "Description='Order_QTY'");
            dr["TCut_QTY"] = dt.Compute("SUM(Total)", "Description='Cut_QTY'");
            dr["TCut_Reduce"] = dt.Compute("SUM(Total)", "Description='Cut_Reduce'");
            dr["TActual_QTY"] = dt.Compute("SUM(Total)", "Description='Actual_Cut-Qty'");
            dr["TOverShortCut_QTY"] = dt.Compute("SUM(Total)", "Description='Over/Short_Cut_Qty'");
            dr["TOverShortCut_PER"] = dt.Compute("SUM(Total)", "Description='Over/Short_Cut%'");
            dr["TScanPack_QTY"] = dt.Compute("SUM(Total)", "Description='Scan_pack_Qty'");
            dr["TShip_QTY"] = dt.Compute("SUM(Total)", "Description='Ship_Qty'");
            dr["TOverShortShip_QTY"] = dt.Compute("SUM(Total)", "Description='Over_ShortShip_Qty'");
            dr["TOverShortShip_PER"] = dt.Compute("SUM(Total)", "Description='Over_ShortShip_%'");
            dr["TLeftOverGarment_A"] = dt.Compute("SUM(Total)", "Description='Leftover_Garment_A'");
            dr["TBGrade_QTY"] = dt.Compute("SUM(Total)", "Description='B_Grade_Qty'");
            dr["TCGrade_QTY"] = dt.Compute("SUM(Total)", "Description='C_Grade_Qty'");
            dr["TUnaccountable_QTY"] = dt.Compute("SUM(Total)", "Description='Unaccountable_Qty'");
            dtFoot.Rows.Add(dr);
            gvList.Attributes.Add("style", "table-layout:fixed");
            gvList.DataSource = dt;
            gvList.DataBind();

            Repeater rpListFoot = e.Item.FindControl("rpListFoot") as Repeater;
            rpListFoot.DataSource = dtFoot;
            rpListFoot.DataBind();

        }

        for (int rowIndex = gvList.Rows.Count - 1; rowIndex > 0; rowIndex--)
        {
            GridViewRow previousRow = gvList.Rows[rowIndex - 1];
            GridViewRow row = gvList.Rows[rowIndex];

            row.Cells[0].RowSpan = (row.Cells[0].RowSpan == 0) ? 1 : row.Cells[0].RowSpan;
            previousRow.Cells[0].RowSpan = (previousRow.Cells[0].RowSpan == 0) ? 1 : previousRow.Cells[0].RowSpan;

            if (row.Cells[0].Text == previousRow.Cells[0].Text)
            {
                row.Cells[0].Visible = false;
                previousRow.Cells[0].RowSpan += row.Cells[0].RowSpan;
            }
        }
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {

        string JO = txtJoNo.Text;  //可以多个  用,分割
        string GO = txtGO.Text;
        string FactoryCD = ddlFactory.SelectedValue.ToString();

        //SUMMARY BY分支
        if (this.DDSummaryBy.SelectedValue.Contains("GO"))
        {
            SelectGOBind(GO, Site);
        }
        else
        {
            SelectJoBind(JO, GO, Site);
        }

    }

    private void SelectGOBind(string GO, string Site)
    {
        DataTable dt = MESComment.ShipmentClosingSQL.GetGO(GO);
        this.rpTotal.DataSource = dt; // dt.Select("COLOR_CD <> 'TOTAL'");
        this.rpTotal.DataBind();
    }

    private void SelectJoBind(string JO, string GO, string Site)
    {
        DataTable dt = MESComment.ShipmentClosingSQL.GetJO(JO, GO, Site);
        this.rpTotal.DataSource = dt;
        this.rpTotal.DataBind();

    }

    protected void rpTotal_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            if (this.DDSummaryBy.SelectedValue.Contains("JO"))
            {
                SelectJO(sender, e);
            }
            else
                SelectGO(sender, e);
        }
    }
    private List<int> col;
    protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.Header)
        {
            col = new List<int>();
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                // e.Row.Cells[i].CssClass = "locked";
                e.Row.Cells[i].BackColor = Color.FromArgb(192, 192, 192);
                if (e.Row.Cells[i].Text.ToUpper() == "COLOR_CD")
                {
                    e.Row.Cells[i].Text = "Color/Size";
                }
                if (e.Row.Cells[i].Text.ToUpper() == "COLOR_DESC")
                {
                    e.Row.Cells[i].Visible = false;
                    col.Add(i);
                }
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = (DataRowView)e.Row.DataItem;
            foreach (int it in col)
            {
                e.Row.Cells[it].Visible = false;
            }
            //  if (e.Row.Cells[0].Text.ToUpper() == "COLOR_CD")
            //{
            e.Row.CssClass = "textRight";
            e.Row.Cells[0].CssClass = "textAlign";
            e.Row.Cells[1].CssClass = "textLeft";
            if (dr != null && (dr.Row["Description"].ToString().Trim() != "Over/Short_Cut%" || dr.Row["Description"].ToString().Trim() != "Over_ShortShip_%"))
            {
                e.Row.Cells[dr.Row.Table.Columns.IndexOf("Total")].Text = e.Row.Cells[dr.Row.Table.Columns.IndexOf("Total")].Text.Replace(".00", "");
            }
            e.Row.Cells[0].Text = e.Row.Cells[dr.Row.Table.Columns.IndexOf("COLOR_CD")].Text + "<br/>(" + e.Row.Cells[dr.Row.Table.Columns.IndexOf("COLOR_DESC")].Text + ")";
            //}
            if (dr != null && (dr.Row["Description"].ToString().Trim() == "Over/Short_Cut%" || dr.Row["Description"].ToString().Trim() == "Over_ShortShip_%"))
            {

                foreach (string item in SizeCOL)
                {
                    if (dr.Row.Table.Columns.IndexOf(item) > -1)
                        e.Row.Cells[dr.Row.Table.Columns.IndexOf(item)].Text += "%";
                }
                e.Row.Cells[dr.Row.Table.Columns.IndexOf("Total")].Text += "%";
                //dr.Row["S"] += "%";
                //dr.Row["M"] += "%";
                //dr.Row["L"] += "%";
                //dr.Row["XL"] += "%";
                //dr.Row["XXL"] += "%";
            }

            //Added by MF on 20150720, fore color changes
            if (dr != null && (dr.Row["Description"].ToString().Trim() == "Over/Short_Cut%"))
            {
                //Forloop cells
                if (over_per != "" && short_per != "")
                {
                    string overshort_cut = "";
                    for (int j = 2; j < e.Row.Cells.Count - 1; j++)
                    {
                        overshort_cut = e.Row.Cells[j].Text;
                        overshort_cut = overshort_cut.Substring(0, overshort_cut.Length-1);
                        if (overshort_cut.Substring(0, 1) == "-")
                        {
                            if (decimal.Parse(overshort_cut) * -1 > decimal.Parse(short_per))
                            {
                                e.Row.Cells[j].ForeColor = Color.Red;
                                e.Row.Cells[j].BackColor = Color.Yellow;

                                if (e.Row.Cells[j] == e.Row.Cells[dr.Row.Table.Columns.IndexOf("Total")])
                                {
                                    e.Row.Cells[dr.Row.Table.Columns.IndexOf("Description")].ForeColor = Color.Red;
                                    e.Row.Cells[dr.Row.Table.Columns.IndexOf("Description")].BackColor = Color.Yellow;
                                }
                            }
                            else
                            {
                                e.Row.Cells[j].ForeColor = Color.Red;

                                if (e.Row.Cells[j] == e.Row.Cells[dr.Row.Table.Columns.IndexOf("Total")])
                                {
                                    e.Row.Cells[dr.Row.Table.Columns.IndexOf("Description")].ForeColor = Color.Red;
                                }
                            }
                        }
                        else
                        {
                            if (decimal.Parse(overshort_cut) > decimal.Parse(over_per))
                            {
                                e.Row.Cells[j].ForeColor = Color.Blue;

                                if (e.Row.Cells[j] == e.Row.Cells[dr.Row.Table.Columns.IndexOf("Total")])
                                {
                                    e.Row.Cells[dr.Row.Table.Columns.IndexOf("Description")].ForeColor = Color.Blue;
                                }
                            }
                        }
                    }
                }
            }
            //End of added by MF on 20150720, fore color changes
        }
    }

    protected void gvList_RowCreated(object sender, GridViewRowEventArgs e)
    {
        e.Row.Cells[0].BackColor = Color.FromArgb(192, 192, 192);
    }
}