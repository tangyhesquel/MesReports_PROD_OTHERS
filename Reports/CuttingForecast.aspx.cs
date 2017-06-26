using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Threading;

public partial class Reports_PreCutSummary : pPage
{
    string goNO = "";
    string moNO = "";
    string FactoryCD = "";
    bool EN = true;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString.AllKeys.Contains("GONO"))
        {
            goNO = Request.QueryString.Get("GONO").Trim();
        }

        if (Request.QueryString.AllKeys.Contains("moNo"))
            moNO = Request.QueryString.Get("moNo").Trim();

        if (Thread.CurrentThread.CurrentUICulture.Name.Equals("zh-CN"))
        {
            EN = false;
        }

        if (!this.Page.IsPostBack)
        {
            this.CHVersion.Enabled = EN;
            if (Request.QueryString["site"] != null)
            {
                FactoryCD = Request.QueryString["site"].ToString().ToUpper();
                if (FactoryCD.Equals("EAV") || FactoryCD.Equals("EGV"))
                {
                    this.CHVersion.Checked = true;
                }
            }
        }

        if (goNO != "" || moNO != "")
        {
            txtGO.Text = goNO;
            txtMO.Text = moNO;
            Div0.InnerHtml = "";
            Div1.InnerHtml = "";
            Div2.InnerHtml = "";
            Div3.InnerHtml = "";
            SetQuery();
        }

    }
    //Modification by LimML on 20150907 - add Before Over / Short Cut
    private Boolean Check()
    {
        if (goNO.Equals("") && moNO.Equals(""))
        {
            if (this.txtMO.Text.Trim().Equals("") && this.txtGO.Text.Trim().Equals(""))
            {
                return false;
            }

            else if (this.CHBfOSCut.Checked == true && txtMO.Text == "")
            {
                return false;
            }
            else
            {
                return true;
            }


        }
        else
        {
            return true;
        }


    }
    private void SetQuery()
    {
        DataTable[] tb = new DataTable[3];
        if (txtGO.Text.Trim() == "")
            txtGO.Text = MESComment.CuttingForecast.Get_GO_BY_MO(txtMO.Text.Trim());
        //Modification by LimML on 20150907 - add Before Over / Short Cut
        if (CHBfOSCut.Checked == true)
        {
            //Modified by MF on 20151019, add Before Over / Short Cut
            tb = MESComment.CuttingForecast.Get_PreCut_DetailBfOSCut(txtGO.Text.Trim(), txtMO.Text.Trim(), "Y");
        }
        else
        {
            //Modified by MF on 20160215, JO Combination
            tb = MESComment.CuttingForecast.Get_PreCut_Detail(txtGO.Text.Trim(), txtMO.Text.Trim(), ByLot.Checked);
            //End of modified by MF on 20160215, JO Combination
        }
        if (tb[0].Rows.Count > 0)
        {
            GenerateTable(tb, 0);
        }
        if (tb[1].Rows.Count > 0)
        {
            GenerateTable(tb, 1);
        }
        if (tb[2].Rows.Count > 0)
        {
            //Modified by MF on 20160215, JO Combination
            if (ByLot.Checked == true)
            {
                GenerateTable(tb, 4);
            }
            else
            {
                GenerateTable(tb, 2);
            }
            //End of modified by MF on 20160215, JO Combination
        }
        GenerateTable(tb, 3);

    }

    private void GenerateTable(DataTable[] tb, int index)
    {
        switch (index)
        {
            case 0:
                Div0.InnerHtml += "<table style='font-size:10pt' cellpadding='0' cellspacing='0'  class='ThinBorderTable'>";
                Div0.InnerHtml += "<tr><th class='left top; style2'>" + Resources.GlobalResources.STRING_CUSTOMER + "</th><th class='top; style2' width='80px'>" + tb[0].Rows[0][0] + "</th><th colspan=" + tb[0].Rows.Count + " class='top; style2'>" + Resources.GlobalResources.STRING_SIZE_BREAK_DOWN_PLAN_QTY + "</th></tr>";
                Div0.InnerHtml += "<tr><td rowspan='2' class='left' style='background-color:Silver'>GO</td><td rowspan='2' >" + txtGO.Text.Trim() + "</td>";
                foreach (DataRow row in tb[0].Rows)
                {
                    Div0.InnerHtml += "<td width='50px'>" + row[1] + "</td>";
                }
                Div0.InnerHtml += "</tr><tr>";
                foreach (DataRow row in tb[0].Rows)
                {
                    Div0.InnerHtml += "<td>" + row[2] + "</td>";
                }
                Div0.InnerHtml += "</tr></table>";
                break;
            case 1:
                Div1.InnerHtml += " <table style='font-size:10pt' cellpadding='0' cellspacing='0'  class='ThinBorderTable'>";
                Div1.InnerHtml += " <tr><th class='left top; style2' width='80px'>GO</th>";
                Div1.InnerHtml += " <th class='top; style2' width='50px'>" + Resources.GlobalResources.STRING_COLOR_CD + "</th>";
                Div1.InnerHtml += " <th class='top; style2' width='50px'>" + Resources.GlobalResources.STRING_COLOR_DESC + "</th>"; //Added by Jin Song MES133
                Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_ORDER_QTY + "</th>";
                Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_FABRIC_TYPE + "</th>";
                Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_OS_CUT_PERCENT + "</th>";
                Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_OS_CUT_QTY + "</th>";
                Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_PLAN_CUT_QTY + "</th>";
                if (!this.CHVersion.Checked || !EN)
                {
                    Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_FAB_REQUIRE + "</th>";
                    Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_FAB_BALANCE + "</th>";
                }
                else
                {
                    Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_GO_REC_YDS + "</th>";
                    Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_SPARE_FAB + "</th>";
                    Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_BINDING_FAB + "</th>";
                    Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_BULK_FAB_QTY + "</th>";
                    Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_MARKER_ALLOCTED_YDS + "</th>";
                    Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_BALANCE_QTY + "</th>";
                }
                Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_NET_YPD + "</th>";
                Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_MARKER_YPD + "</th>";
                Div1.InnerHtml += " <th class='top; style2'>PPO YPD</th>";
                Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_MU + "</th>";
                Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_WASTAGE_RATE + "</th>";
                Div1.InnerHtml += " <th class='top; style2'>" + Resources.GlobalResources.STRING_FABRIC_WIDTH + "</th></tr>";

                for (int i = 0; i < tb[1].Rows.Count; i++)
                {
                    if (i != tb[1].Rows.Count - 1)
                    {
                        Div1.InnerHtml += "<tr><td class='left'>" + txtGO.Text.Trim() + "</td>";
                        for (int j = 0; j <= 6; j++) //Bug fix on MES133 by Jin Song 2014-09-04 (adjust column number)
                        {
                            Div1.InnerHtml += "<td>" + tb[1].Rows[i][j] + "</td>";
                        }
                        if (!this.CHVersion.Checked || !EN)
                        {
                            for (int j = 7; j <= 8; j++) //Bug fix on MES133 by Jin Song 2014-09-04 (adjust column number)
                            {
                                Div1.InnerHtml += "<td>" + tb[1].Rows[i][j] + "</td>";
                            }
                        }
                        else
                        {
                            for (int j = 9; j <= 14; j++) //Bug fix on MES133 by Jin Song 2014-09-04 (adjust column number)
                            {
                                Div1.InnerHtml += "<td>" + tb[1].Rows[i][j] + "</td>";
                            }
                        }
                        for (int j = 15; j < tb[1].Columns.Count; j++) //Bug fix on MES133 by Jin Song 2014-09-04 (adjust column number)
                        {
                            Div1.InnerHtml += "<td>" + tb[1].Rows[i][j] + "</td>";
                        }
                        Div1.InnerHtml += "</tr>";
                    }
                    else
                    {
                        Div1.InnerHtml += "<tr><td class='left'>" + txtGO.Text.Trim() + "</td>";
                        for (int j = 0; j <= 6; j++) //Bug fix on MES133 by Jin Song 2014-09-04 (adjust column number)
                        {
                            Div1.InnerHtml += "<td>" + tb[1].Rows[i][j] + "</td>";
                        }
                        if (!this.CHVersion.Checked || !EN)
                        {
                            for (int j = 7; j <= 8; j++) //Bug fix on MES133 by Jin Song 2014-09-04 (adjust column number)
                            {
                                Div1.InnerHtml += "<td>" + tb[1].Rows[i][j] + "</td>";
                            }
                        }
                        else
                        {
                            for (int j = 9; j <= 14; j++) //Bug fix on MES133 by Jin Song 2014-09-04 (adjust column number)
                            {
                                Div1.InnerHtml += "<td>" + tb[1].Rows[i][j] + "</td>";
                            }
                        }
                        for (int j = 15; j < tb[1].Columns.Count; j++) //Bug fix on MES133 by Jin Song 2014-09-04 (adjust column number)
                        {
                            Div1.InnerHtml += "<td>" + tb[1].Rows[i][j] + "</td>";
                        }
                        Div1.InnerHtml += "</tr>";
                    }
                }
                Div1.InnerHtml += "</table>";
                break;

            case 2:
                string[] OldValue = { "", "" };
                string[] Value = new string[tb[3].Rows.Count];
                string[] Value_Order = new string[tb[3].Rows.Count];
                string[] Columns = null;
                if (!this.CHVersion.Checked || !EN)//加入条件;
                {
                    Columns = new string[] { "PLAN_CUT_QTY", "===END===" };
                }
                else
                {
                    Columns = new string[] { "ORDER_QTY_BY_COL_SIZE", "PLAN_CUT_QTY", "Over_Short_qty", "===END===" };
                }
                string[] Type = { "Order Qty", "Plan Cut Qty", "&nbsp;Over/Short Qty(+/-)&nbsp;" };
                int rowspan = Columns.Count() - 1;
                string Color = "#c0c0c0";

                //Added Color Desc and PPO No. by Jin Song MES133
                //modification by LIMML ON 20150701 (ADD MARKER YPD AND PPO YPD)
                if (this.CHVersion.Checked)
                {
                    Div2.InnerHtml += "<table style='font-size:10pt' cellpadding='0' cellspacing='0' class='ThinBorderTable'><tr><th class='left top; style2' rowspan='2' width='80px'>JO</th><th class='top; style2' rowspan='2' width='50px'>" + Resources.GlobalResources.STRING_COLOR_CD + "</th><th class='top; style2' rowspan='2' width='50px'>" + Resources.GlobalResources.STRING_COLOR_DESC + "</th><th class='top; style2' rowspan='2' style='width:50px'>" + Resources.GlobalResources.STRING_ORDER_QTY + "</th><th style='width:100px' class='top; style2' rowspan='2'>" + Resources.GlobalResources.STRING_OS_ALLOWED + "</th><th class='top; style2' rowspan='2' style='width:100px'>" + Resources.GlobalResources.STRING_OS_CUT_PERCENT + "</th><th class='top; style2' rowspan='2' style='width:100px'>" + Resources.GlobalResources.STRING_OS_CUT_QTY + "</th><th class='top; style2' rowspan='2' style='width:50px'>" + Resources.GlobalResources.STRING_PLAN_CUT_QTY + "</th><th class='top; style2' colspan=" + tb[3].Rows.Count + ">" + Resources.GlobalResources.STRING_SIZE_BREAK_DOWN_PLAN_QTY + "</th><th class='top; style2'></th><th class='top; style2' rowspan='2' >" + Resources.GlobalResources.STRING_PPO_NO + "</th><th class='top; style2' rowspan='2' style='width:30px'>" + Resources.GlobalResources.STRING_MARKER_YPD + "</th><th class='top; style2' rowspan='2' style='width:30px'> PPO YPD </th>";
                }
                else
                {
                    Div2.InnerHtml += "<table style='font-size:10pt' cellpadding='0' cellspacing='0' class='ThinBorderTable'><tr><th class='left top; style2' rowspan='2' width='80px'>JO</th><th class='top; style2' rowspan='2' width='50px'>" + Resources.GlobalResources.STRING_COLOR_CD + "</th><th class='top; style2' rowspan='2' width='50px'>" + Resources.GlobalResources.STRING_COLOR_DESC + "</th><th class='top; style2' rowspan='2' style='width:50px'>" + Resources.GlobalResources.STRING_ORDER_QTY + "</th><th style='width:100px' class='top; style2' rowspan='2'>" + Resources.GlobalResources.STRING_OS_ALLOWED + "</th><th class='top; style2' rowspan='2' style='width:100px'>" + Resources.GlobalResources.STRING_OS_CUT_PERCENT + "</th><th class='top; style2' rowspan='2' style='width:100px'>" + Resources.GlobalResources.STRING_OS_CUT_QTY + "</th><th class='top; style2' rowspan='2' style='width:50px'>" + Resources.GlobalResources.STRING_PLAN_CUT_QTY + "</th><th class='top; style2' colspan=" + tb[3].Rows.Count + ">" + Resources.GlobalResources.STRING_SIZE_BREAK_DOWN_PLAN_QTY + "</th><th class='top; style2' rowspan='2' >" + Resources.GlobalResources.STRING_PPO_NO + "</th><th class='top; style2' rowspan='2' style='width:30px'>" + Resources.GlobalResources.STRING_MARKER_YPD + "</th><th class='top; style2' rowspan='2' style='width:30px'> PPO YPD </th>";
                }

                //if (this.CHVersion.Checked && EN)//加入条件;
                //{
                //Div2.InnerHtml += "<th class='style2'></th>";//在标题行中Type列对应的空格;
                //}
                Div2.InnerHtml += "</tr><tr>";
                for (int i = 0; i < tb[3].Rows.Count; i++)
                {//Size行;
                    Div2.InnerHtml += "<th class='style2' >" + tb[3].Rows[i][0] + "</th>";
                }
                //PPO NO;
                //Div2.InnerHtml += "<th class='top; style2' rowspan='2' >" + Resources.GlobalResources.STRING_PPO_NO + "</th>";

                if (this.CHVersion.Checked && EN)//加入条件;
                {
                    Div2.InnerHtml += "<td class='style2'></td>";//在Size行中Type列对应的空格;
                }
                Div2.InnerHtml += "</tr>";

                for (int i = 0; i < tb[2].Rows.Count; i++)
                {

                    if (tb[2].Rows[i][0].ToString() != OldValue[0] || tb[2].Rows[i][1].ToString() != OldValue[1])
                    {
                        OldValue[0] = "";
                        OldValue[1] = "";
                        Value = new string[tb[3].Rows.Count];
                        if (this.CHVersion.Checked && EN)//加入条件;
                        {
                            if (Color.Equals("#c0c0c0"))
                            {
                                Color = "white";
                            }
                            else
                            {
                                Color = "#c0c0c0";
                            }
                        }
                        else
                        {
                            Color = "white";
                        }
                        //每个JO的公共信息;                        
                        Div2.InnerHtml += "<tr bgcolor='" + Color + "'><td rowspan=" + rowspan + " class='left'>" + tb[2].Rows[i][0] + "</td><td rowspan=" + rowspan + ">" + tb[2].Rows[i][1] + "</td><td rowspan=" + rowspan + ">" + tb[2].Rows[i][2] + "</td>";
                        Div2.InnerHtml += "<td rowspan=" + rowspan + " >" + tb[2].Rows[i][3] + "</td><td rowspan=" + rowspan + " >" + tb[2].Rows[i][4] + "</td><td rowspan=" + rowspan + ">" + tb[2].Rows[i][5] + "</td><td rowspan=" + rowspan + ">" + tb[2].Rows[i][6] + "</td><td rowspan=" + rowspan + ">" + tb[2].Rows[i][7] + "</td>"; //Add one column by Jin Song MES133
                        int ii = 0;
                        while (!Columns[ii].ToString().Equals("===END==="))
                        {
                            string Color_D = "white";
                            string FontStyle = "";
                            if (this.CHVersion.Checked && EN)//加入条件;
                            {
                                if (Columns[ii].ToString().Equals("PLAN_CUT_QTY"))
                                {
                                    Color_D = "#c0c0c0";
                                }
                            }
                            for (int j = i; j < tb[2].Rows.Count; j++)
                            {
                                if ((tb[2].Rows[j][0].ToString() == OldValue[0] && tb[2].Rows[j][1].ToString() == OldValue[1]) || (OldValue[0] == "" && OldValue[1] == ""))
                                {
                                    for (int n = 0; n < tb[3].Rows.Count; n++)
                                    {
                                        if (tb[3].Rows[n][0].ToString() == tb[2].Rows[j][8].ToString())
                                        {
                                            if (!Columns[ii].ToString().Equals("Over_Short_qty"))
                                            {//PLAN_CUT_QTY & ORDER_QTY
                                                Value[n] = tb[2].Rows[j][Columns[ii].ToString()].ToString();
                                            }
                                            else
                                            {//Over_Short_qty
                                                double Over_Short_qty = Double.Parse(tb[2].Rows[j]["PLAN_CUT_QTY"].ToString()) - Double.Parse(tb[2].Rows[j]["ORDER_QTY_BY_COL_SIZE"].ToString());

                                                Value[n] = Over_Short_qty.ToString("#,###");
                                                FontStyle = "font-style:italic; font-weight:bolder; font-size:12px";
                                            }
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    for (int n = 0; n < tb[3].Rows.Count; n++)
                                    {
                                        if (Value[n] == "0" || Value[n] == "" || Value[n] == null)
                                        {
                                            Div2.InnerHtml += "<td bgcolor='" + Color_D + "' style='" + FontStyle + "'>&nbsp;</td>";
                                        }
                                        else
                                        {
                                            Div2.InnerHtml += "<td bgcolor='" + Color_D + "' style='" + FontStyle + "'>" + Value[n] + "</td>";
                                        }
                                    }
                                    break;
                                }
                                if (j == tb[2].Rows.Count - 1)
                                {
                                    for (int n = 0; n < tb[3].Rows.Count; n++)
                                    {
                                        if (Value[n] != "0" && Value[n] != null)
                                        {
                                            Div2.InnerHtml += "<td bgcolor='" + Color_D + "' style='" + FontStyle + "'>" + Value[n] + "</td>";
                                        }
                                        else
                                        {
                                            Div2.InnerHtml += "<td bgcolor='" + Color_D + "' style='" + FontStyle + "'>&nbsp;</td>";
                                        }
                                    }
                                }

                                OldValue[0] = tb[2].Rows[j][0].ToString();
                                OldValue[1] = tb[2].Rows[j][1].ToString();
                            }
                            if (this.CHVersion.Checked && EN)//加入条件;
                            {
                                Div2.InnerHtml += "<td  width='80px' bgcolor='" + Color_D + "' style='text-align:center;" + FontStyle + "'>" + Type[ii].ToString() + "</td>";//输入Type;
                            }
                            //Added modification by LIMML ON 20150701 (ADD MARKER YPD AND PPO YPD)
                            //Added by Jin Song MES133 (Add PPO No)
                            if (ii == 0)
                            {
                                Div2.InnerHtml += "<td rowspan=" + rowspan + " >" + tb[2].Rows[i][11] + "</td>";
                                Div2.InnerHtml += "<td rowspan=" + rowspan + " >" + tb[2].Rows[i][12] + "</td>";
                                Div2.InnerHtml += "<td rowspan=" + rowspan + " >" + tb[2].Rows[i][13] + "</td>";
                            }
                            Div2.InnerHtml += "</tr>";
                            OldValue[0] = tb[2].Rows[i][0].ToString();
                            OldValue[1] = tb[2].Rows[i][1].ToString();
                            ii++;
                        }
                    }
                }
                Div2.InnerHtml += "</table>";
                break;
            case 3:
                Div3.InnerHtml += "<table style='font-size:10pt' cellpadding='0' cellspacing='0'  class='ThinBorderTable' width='300px'><tr><th class='left top; style2'>JO</th><th class='top; style2'>" + Resources.GlobalResources.STRING_COLOR_CD + "</th><th class='top; style2'>" + Resources.GlobalResources.STRING_SIZE_CD + "</th><th class='top; style2'>" + Resources.GlobalResources.STRING_REMARK + "</th></tr>";
                for (int i = 0; i < tb[4].Rows.Count; i++)
                {
                    Div3.InnerHtml += "<tr><td class='left'>" + tb[4].Rows[i]["JOB_ORDER_NO"] + "</td>";
                    if (tb[4].Rows[i]["COLOR"] != null && tb[4].Rows[i]["COLOR"].ToString() != "")
                    {
                        Div3.InnerHtml += "<td>" + tb[4].Rows[i]["COLOR"] + "</td>";
                    }
                    else
                    {
                        Div3.InnerHtml += "<td>&nbsp;</td>";
                    }
                    if (tb[4].Rows[i]["size"] != null && tb[4].Rows[i]["size"].ToString() != "")
                    {
                        Div3.InnerHtml += "<td>" + tb[4].Rows[i]["size"] + "</td>";
                    }
                    else
                    {
                        Div3.InnerHtml += "<td>&nbsp;</td>";
                    }
                    if (tb[4].Rows[i]["REMARK"] != null && tb[4].Rows[i]["REMARK"].ToString() != "")
                    {
                        Div3.InnerHtml += "<td>" + tb[4].Rows[i]["REMARK"] + "</td>";
                    }
                    else
                    {
                        Div3.InnerHtml += "<td>&nbsp;</td>";
                    }
                }
                Div3.InnerHtml += "</table>";
                break;


            //Added by MF on 20160215, JO Combination
            case 4:
                string[] OldValueByLot = { "", "", "" };
                string[] ValueByLot = new string[tb[3].Rows.Count];
                string[] Value_Order_ByLot = new string[tb[3].Rows.Count];
                string[] ColumnsByLot = null;
                string[] TypeByLot = { "Order Qty", "Plan Cut Qty", "&nbsp;Over/Short Qty(+/-)&nbsp;" };

                if (!this.CHVersion.Checked || !EN)//加入条件;
                {
                    ColumnsByLot = new string[] { "PLAN_CUT_QTY", "===END===" };
                }
                else
                {
                    ColumnsByLot = new string[] { "ORDER_QTY_BY_COL_SIZE", "PLAN_CUT_QTY", "Over_Short_qty", "===END===" };
                }
                int rowspanByLot = ColumnsByLot.Count() - 1;
                string ColorByLot = "#c0c0c0";

                if (this.CHVersion.Checked)
                {
                    Div2.InnerHtml += "<table style='font-size:10pt' cellpadding='0' cellspacing='0' class='ThinBorderTable'><tr><th class='left top; style2' rowspan='2' width='80px'>JO</th><th class='left top; style2' rowspan='2' width='80px'>Lot No</th><th class='top; style2' rowspan='2' width='50px'>" + Resources.GlobalResources.STRING_COLOR_CD + "</th><th class='top; style2' rowspan='2' width='50px'>" + Resources.GlobalResources.STRING_COLOR_DESC + "</th><th class='top; style2' rowspan='2' style='width:50px'>" + Resources.GlobalResources.STRING_ORDER_QTY + "</th><th style='width:100px' class='top; style2' rowspan='2'>" + Resources.GlobalResources.STRING_OS_ALLOWED + "</th><th class='top; style2' rowspan='2' style='width:100px'>" + Resources.GlobalResources.STRING_OS_CUT_PERCENT + "</th><th class='top; style2' rowspan='2' style='width:100px'>" + Resources.GlobalResources.STRING_OS_CUT_QTY + "</th><th class='top; style2' rowspan='2' style='width:50px'>" + Resources.GlobalResources.STRING_PLAN_CUT_QTY + "</th><th class='top; style2' colspan=" + tb[3].Rows.Count + ">" + Resources.GlobalResources.STRING_SIZE_BREAK_DOWN_PLAN_QTY + "</th><th class='top; style2'></th><th class='top; style2' rowspan='2' >" + Resources.GlobalResources.STRING_PPO_NO + "</th><th class='top; style2' rowspan='2' style='width:30px'>" + Resources.GlobalResources.STRING_MARKER_YPD + "</th><th class='top; style2' rowspan='2' style='width:30px'> PPO YPD </th>";
                }
                else
                {
                    Div2.InnerHtml += "<table style='font-size:10pt' cellpadding='0' cellspacing='0' class='ThinBorderTable'><tr><th class='left top; style2' rowspan='2' width='80px'>JO</th><th class='left top; style2' rowspan='2' width='80px'>Lot No</th><th class='top; style2' rowspan='2' width='50px'>" + Resources.GlobalResources.STRING_COLOR_CD + "</th><th class='top; style2' rowspan='2' width='50px'>" + Resources.GlobalResources.STRING_COLOR_DESC + "</th><th class='top; style2' rowspan='2' style='width:50px'>" + Resources.GlobalResources.STRING_ORDER_QTY + "</th><th style='width:100px' class='top; style2' rowspan='2'>" + Resources.GlobalResources.STRING_OS_ALLOWED + "</th><th class='top; style2' rowspan='2' style='width:100px'>" + Resources.GlobalResources.STRING_OS_CUT_PERCENT + "</th><th class='top; style2' rowspan='2' style='width:100px'>" + Resources.GlobalResources.STRING_OS_CUT_QTY + "</th><th class='top; style2' rowspan='2' style='width:50px'>" + Resources.GlobalResources.STRING_PLAN_CUT_QTY + "</th><th class='top; style2' colspan=" + tb[3].Rows.Count + ">" + Resources.GlobalResources.STRING_SIZE_BREAK_DOWN_PLAN_QTY + "</th><th class='top; style2' rowspan='2' >" + Resources.GlobalResources.STRING_PPO_NO + "</th><th class='top; style2' rowspan='2' style='width:30px'>" + Resources.GlobalResources.STRING_MARKER_YPD + "</th><th class='top; style2' rowspan='2' style='width:30px'> PPO YPD </th>";
                }

                Div2.InnerHtml += "</tr><tr>";
                for (int i = 0; i < tb[3].Rows.Count; i++)
                {//Size行;
                    Div2.InnerHtml += "<th class='style2' >" + tb[3].Rows[i][0] + "</th>";
                }

                if (this.CHVersion.Checked && EN)//加入条件;
                {
                    Div2.InnerHtml += "<td class='style2'></td>";//在Size行中Type列对应的空格;
                }
                Div2.InnerHtml += "</tr>";

                for (int i = 0; i < tb[2].Rows.Count; i++)
                {

                    if (tb[2].Rows[i][0].ToString() != OldValueByLot[0] || tb[2].Rows[i][1].ToString() != OldValueByLot[1] || tb[2].Rows[i][2].ToString() != OldValueByLot[2])
                    {
                        OldValueByLot[0] = "";
                        OldValueByLot[1] = "";
                        OldValueByLot[2] = "";
                        ValueByLot = new string[tb[3].Rows.Count];
                        if (this.CHVersion.Checked && EN)//加入条件;
                        {
                            if (ColorByLot.Equals("#c0c0c0"))
                            {
                                ColorByLot = "white";
                            }
                            else
                            {
                                ColorByLot = "#c0c0c0";
                            }
                        }
                        else
                        {
                            ColorByLot = "white";
                        }
                        //每个JO的公共信息;                        
                        Div2.InnerHtml += "<tr bgcolor='" + ColorByLot + "'><td rowspan=" + rowspanByLot + " class='left'>" + tb[2].Rows[i][0] + "</td><td rowspan=" + rowspanByLot + ">" + tb[2].Rows[i][1] + "</td><td rowspan=" + rowspanByLot + ">" + tb[2].Rows[i][2] + "</td>";
                        Div2.InnerHtml += "<td rowspan=" + rowspanByLot + " >" + tb[2].Rows[i][3] + "</td><td rowspan=" + rowspanByLot + " >" + tb[2].Rows[i][4] + "</td><td rowspan=" + rowspanByLot + ">" + tb[2].Rows[i][5] + "</td><td rowspan=" + rowspanByLot + ">" + tb[2].Rows[i][6] + "</td><td rowspan=" + rowspanByLot + ">" + tb[2].Rows[i][7] + "</td><td rowspan=" + rowspanByLot + ">" + tb[2].Rows[i][8] + "</td>"; //Add one column by Jin Song MES133
                        int ii = 0;
                        while (!ColumnsByLot[ii].ToString().Equals("===END==="))
                        {
                            string Color_D = "white";
                            string FontStyle = "";
                            if (this.CHVersion.Checked && EN)//加入条件;
                            {
                                if (ColumnsByLot[ii].ToString().Equals("PLAN_CUT_QTY"))
                                {
                                    Color_D = "#c0c0c0";
                                }
                            }
                            for (int j = i; j < tb[2].Rows.Count; j++)
                            {
                                if ((tb[2].Rows[j][0].ToString() == OldValueByLot[0] && tb[2].Rows[j][1].ToString() == OldValueByLot[1] && tb[2].Rows[j][2].ToString() == OldValueByLot[2]) || (OldValueByLot[0] == "" && OldValueByLot[1] == "" && OldValueByLot[2] == ""))
                                {
                                    for (int n = 0; n < tb[3].Rows.Count; n++)
                                    {
                                        if (tb[3].Rows[n][0].ToString() == tb[2].Rows[j][9].ToString())
                                        {
                                            if (!ColumnsByLot[ii].ToString().Equals("Over_Short_qty"))
                                            {//PLAN_CUT_QTY & ORDER_QTY
                                                ValueByLot[n] = tb[2].Rows[j][ColumnsByLot[ii].ToString()].ToString();
                                            }
                                            else
                                            {//Over_Short_qty
                                                double Over_Short_qty = Double.Parse(tb[2].Rows[j]["PLAN_CUT_QTY"].ToString()) - Double.Parse(tb[2].Rows[j]["ORDER_QTY_BY_COL_SIZE"].ToString());

                                                ValueByLot[n] = Over_Short_qty.ToString("#,###");
                                                FontStyle = "font-style:italic; font-weight:bolder; font-size:12px";
                                            }
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    for (int n = 0; n < tb[3].Rows.Count; n++)
                                    {
                                        if (ValueByLot[n] == "0" || ValueByLot[n] == "" || ValueByLot[n] == null)
                                        {
                                            Div2.InnerHtml += "<td bgcolor='" + Color_D + "' style='" + FontStyle + "'>&nbsp;</td>";
                                        }
                                        else
                                        {
                                            Div2.InnerHtml += "<td bgcolor='" + Color_D + "' style='" + FontStyle + "'>" + ValueByLot[n] + "</td>";
                                        }
                                    }
                                    break;
                                }
                                if (j == tb[2].Rows.Count - 1)
                                {
                                    for (int n = 0; n < tb[3].Rows.Count; n++)
                                    {
                                        if (ValueByLot[n] != "0" && ValueByLot[n] != null)
                                        {
                                            Div2.InnerHtml += "<td bgcolor='" + Color_D + "' style='" + FontStyle + "'>" + ValueByLot[n] + "</td>";
                                        }
                                        else
                                        {
                                            Div2.InnerHtml += "<td bgcolor='" + Color_D + "' style='" + FontStyle + "'>&nbsp;</td>";
                                        }
                                    }
                                }

                                OldValueByLot[0] = tb[2].Rows[j][0].ToString();
                                OldValueByLot[1] = tb[2].Rows[j][1].ToString();
                                OldValueByLot[2] = tb[2].Rows[j][2].ToString();
                            }
                            if (this.CHVersion.Checked && EN)//加入条件;
                            {
                                Div2.InnerHtml += "<td  width='80px' bgcolor='" + Color_D + "' style='text-align:center;" + FontStyle + "'>" + TypeByLot[ii].ToString() + "</td>";//输入Type;
                            }
                            //Added modification by LIMML ON 20150701 (ADD MARKER YPD AND PPO YPD)
                            //Added by Jin Song MES133 (Add PPO No)
                            if (ii == 0)
                            {
                                Div2.InnerHtml += "<td rowspan=" + rowspanByLot + " >" + tb[2].Rows[i][12] + "</td>";
                                Div2.InnerHtml += "<td rowspan=" + rowspanByLot + " >" + tb[2].Rows[i][13] + "</td>";
                                Div2.InnerHtml += "<td rowspan=" + rowspanByLot + " >" + tb[2].Rows[i][14] + "</td>";
                            }
                            Div2.InnerHtml += "</tr>";
                            OldValueByLot[0] = tb[2].Rows[i][0].ToString();
                            OldValueByLot[1] = tb[2].Rows[i][1].ToString();
                            OldValueByLot[2] = tb[2].Rows[i][2].ToString();
                            ii++;
                        }
                    }
                }
                Div2.InnerHtml += "</table>";
                break;


        }
    }

    //Modification by LimML on 20150907 - add Before Over / Short Cut
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Div0.InnerHtml = "";
        Div1.InnerHtml = "";
        Div2.InnerHtml = "";
        Div3.InnerHtml = "";

        if (Check())
        {
            SetQuery();
        }
        else if (this.CHBfOSCut.Checked == true && txtMO.Text == "")
        {
            this.Div0.InnerHtml += "<table width='100%'  style='color:Red; font-size:small'><tr><td style='text-align: left'><b>Before Over/Short Cut only applied to MO NO, Please key in the MO!</b></td></tr></table>";
        }
        else
        {
            this.Div0.InnerHtml += "<table width='100%'  style='color:Red; font-size:small'><tr><td style='text-align: left'><b>PLS input GO/MO !</b></td></tr></table>";
        }
    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        MESComment.Excel.ToExcel(this.ExcTable, "PreCutSummary" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
    }


    protected void CHBfOSCut_CheckedChanged(object sender, EventArgs e)
    {

    }

    protected void CHVersion_CheckedChanged(object sender, EventArgs e)
    {

    }
}