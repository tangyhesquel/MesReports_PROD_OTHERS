using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_pcm : pPage
{
    string userid = "";
    public DataTable dt;
    public DataTable dtCol;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["userid"] != null)
        {
            userid = Request.QueryString["userid"].ToString();
        }
        if (!IsPostBack)
        {
            ddlfactoryCd.DataSource = MESComment.MesRpt.GetFactoryList(userid);
            ddlfactoryCd.DataBind();
            DateTime date = DateTime.Now;
            date = date.AddYears(-10);
            for (int i = 1; i < 21; i++)
            {
                ddlyear.Items.Add(date.AddYears(i).Year.ToString());
            }
            ddlyear.SelectedValue = DateTime.Now.Year.ToString(); //Added By ZouShiChang ON 2013.08,29  MES024
        }
        if (Request.QueryString["site"] != null)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                ddlfactoryCd.SelectedValue = Request.QueryString["site"].ToString();
            }
            else
            {
                ddlfactoryCd.SelectedValue = "GEG";
            }
        }
        //ddlyear.SelectedValue = DateTime.Now.Year.ToString();//Annotated By ZouShiChang ON 2013.08.29 MES 024
    }

    ////added By ZouShiChang ON 2013.09.20 Start MES024

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        string[] arry;
        dt = MESComment.PCM.GetPcmDetail(ddlfactoryCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value, ddlwashType.SelectedItem.Value, ddlyear.SelectedItem.Value, ddlmonth.SelectedItem.Value);

        dtCol = new DataTable();
        dtCol.Columns.Add("Col1");
        dtCol.Columns.Add("Col2");
        dtCol.Columns.Add("Col3");


        for (int i = 0; i < dt.Columns.Count; i++)
        {
            DataRow NewRow = dtCol.NewRow();
            arry = dt.Columns[i].ToString().Split('>');
            if (i > 10)
            {
                NewRow["Col1"] = arry[0];
                NewRow["Col2"] = arry[1];
                NewRow["Col3"] = arry[2];
                dtCol.Rows.Add(NewRow);
            }
            else
            {
                NewRow["Col1"] = arry[0];
                dtCol.Rows.Add(NewRow);
            }
        }

        gvPcmDetail.DataSource = dt;
        gvPcmDetail.DataBind();

        return;

    }

    /*
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        
        divBody.InnerHtml = "";
        //head
        divBody.InnerHtml += "<table border='1' cellspacing='0' cellpadding='0' style='font-size:12px;border-collapse:collapse'>";
        divBody.InnerHtml += "<TR><td class='tr2style' ROWSPAN=3 bgcolor='#efefe7' width='20'>C/TNO</td>";
        divBody.InnerHtml += "<td class='tr2style' ROWSPAN=3 bgcolor='#efefe7'>Buyer</td>";
        divBody.InnerHtml += "<td class='tr2style' ROWSPAN=3 bgcolor='#efefe7'>StyleNO</td>";
        divBody.InnerHtml += "<td class='tr2style' ROWSPAN=3 bgcolor='#efefe7'>SC_NO</td> ";
        divBody.InnerHtml += "<td class='tr2style' ROWSPAN=3 bgcolor='#efefe7'>SAM</td> ";
        divBody.InnerHtml += "<td class='tr2style' ROWSPAN=3 bgcolor='#efefe7'>Wash Type</td>";
        divBody.InnerHtml += "<td class='tr2style' ROWSPAN=3 bgcolor='#efefe7'>SAH</td> ";
        divBody.InnerHtml += "<td class='tr2style' ROWSPAN=3 bgcolor='#efefe7'>BPD</td> ";
        divBody.InnerHtml += "<td class='tr2style' ROWSPAN=3 bgcolor='#efefe7'>PPCD</td>";
        divBody.InnerHtml += "<td class='tr2style' ROWSPAN=3 bgcolor='#efefe7'>Order Qty</td>";
        divBody.InnerHtml += "<td class='tr2style' ROWSPAN=3 bgcolor='#efefe7'>Outsource Qty</td>";

        DataTable nInAndOut = MESComment.PCM.GetPcmnInAndOut(ddlfactoryCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value);
        int[] nIn = new int[nInAndOut.Rows.Count];
        int[] nOut = new int[nInAndOut.Rows.Count];
        int ubound = 0;
        for (int i = 0; i < nInAndOut.Rows.Count; i++)
        {
            int n_in = int.Parse(nInAndOut.Rows[i]["N_IN"].ToString());
            int n_out = int.Parse(nInAndOut.Rows[i]["N_OUT"].ToString());
            String processCd1 = nInAndOut.Rows[i]["PROCESS_CD"].ToString();
            nIn[i] = n_in; nOut[i] = n_out;

            int ubnd = 0;
            DataTable qtyOutList = MESComment.PCM.GetPcmQtyOut(ddlfactoryCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value);
            for (int k = 0; k < qtyOutList.Rows.Count; k++)
            {
                String processCd2 = qtyOutList.Rows[k]["PROCESS_CD"].ToString();
                if (processCd1.Equals(processCd2))
                {
                    ubnd++;
                }
            }
            if (ubnd > ubound)
            {
                ubound = ubnd;
            }
            divBody.InnerHtml += "<td align=center colspan='" + (n_in + n_out + 3) + "'>" + nInAndOut.Rows[i]["SHORT_NAME"] + "</td>";
        }
        divBody.InnerHtml += "</tr><tr>";
        for (int i = 0; i < nInAndOut.Rows.Count; i++)
        {
            divBody.InnerHtml += "<td>Openning</td><td colspan='" + nIn[i] + "'>Qty In</td><td colspan='" + nOut[i] + "'>Qty Out</td><td>Wastage Qty</td><td>StkBalance</td>";
        }
        divBody.InnerHtml += "</tr><tr>";
        String[,] qtyIn;
        String[,] qtyOut;
        if (++ubound > nInAndOut.Rows.Count)
        {
            qtyIn = new String[nInAndOut.Rows.Count, ubound];
            qtyOut = new String[nInAndOut.Rows.Count, ubound];
        }
        else
        {
            qtyIn = new String[nInAndOut.Rows.Count, nInAndOut.Rows.Count];
            qtyOut = new String[nInAndOut.Rows.Count, nInAndOut.Rows.Count];
        }

        for (int i = 0; i < nInAndOut.Rows.Count; i++)
        {

            divBody.InnerHtml += "<td>" + nInAndOut.Rows[i]["SHORT_NAME"] + "</td>";
            int seq = int.Parse(nInAndOut.Rows[i]["SEQ"].ToString());
            String processCd = nInAndOut.Rows[i]["PROCESS_CD"].ToString();
            String garmentType = nInAndOut.Rows[i]["GARMENT_TYPE"].ToString();
            bool flag = false;
            int x = 0, y = 0;
            qtyIn[i, x++] = nInAndOut.Rows[i]["PROCESS_CD"].ToString();
            qtyOut[i, y++] = nInAndOut.Rows[i]["PROCESS_CD"].ToString();
            if (seq == 1)
            {
                divBody.InnerHtml += "<td>" + nInAndOut.Rows[i]["SHORT_NAME"] + "</td>";
                qtyIn[i, x++] = nInAndOut.Rows[i]["PROCESS_CD"].ToString();
                flag = true;
            }
            else
            {
                DataTable InList = MESComment.PCM.GetPcmQtyIn(ddlfactoryCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value);
                for (int j = 0; j < InList.Rows.Count; j++)
                {
                    int seqIn = int.Parse(InList.Rows[j]["DISPLAY_SEQ"].ToString());
                    String processCdIn = InList.Rows[j]["NEXT_PROCESS_CD"].ToString();
                    String garmentTypeIn = InList.Rows[j]["GARMENT_TYPE"].ToString();
                    if (seq == seqIn && processCd.Equals(processCdIn) && garmentType.Equals(garmentTypeIn))
                    {
                        divBody.InnerHtml += "<td>" + InList.Rows[j]["SHORT_NAME"] + "</td>";
                        flag = true;
                        qtyIn[i, x++] = InList.Rows[j]["PROCESS_CD"].ToString();
                    }
                }
            }
            if (flag == false)
            {
                divBody.InnerHtml += "<td></td>";
            }
            DataTable qtyOutList = MESComment.PCM.GetPcmQtyOut(ddlfactoryCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value);
            for (int k = 0; k < qtyOutList.Rows.Count; k++)
            {
                int seqOut = int.Parse(qtyOutList.Rows[k]["SEQ"].ToString());
                String processCdOut = qtyOutList.Rows[k]["PROCESS_CD"].ToString();
                String garmentTypeOut = qtyOutList.Rows[k]["GARMENT_TYPE"].ToString();
                if (seq == seqOut && processCd.Equals(processCdOut) && garmentType.Equals(garmentTypeOut))
                {
                    divBody.InnerHtml += "<TD>" + qtyOutList.Rows[k]["SHORT_NAME"] + "</TD>";
                    qtyOut[i, y++] = qtyOutList.Rows[k]["NEXT_PROCESS_CD"].ToString();
                    flag = true;
                }
            }
            if (flag == false)
            {
                divBody.InnerHtml += "<td></td>";
            }
            divBody.InnerHtml += "<td>" + nInAndOut.Rows[i]["SHORT_NAME"] + "</td><td>" + nInAndOut.Rows[i]["SHORT_NAME"] + "</td>";

        }


        divBody.InnerHtml += "</tr>";
        //detail
        DataTable JoNoNew = MESComment.PCM.GetPcmJoNoNew(ddlfactoryCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value, ddlwashType.SelectedItem.Value, ddlyear.SelectedItem.Value, ddlmonth.SelectedItem.Value);
        DataTable pcmList = MESComment.PCM.GetPcmAll(ddlfactoryCd.SelectedItem.Value, ddlgarmentType.SelectedItem.Value, ddlwashType.SelectedItem.Value, ddlyear.SelectedItem.Value, ddlmonth.SelectedItem.Value);
        for (int m = 0; m < JoNoNew.Rows.Count; m++)
        {
            divBody.InnerHtml += "<TR><td>" + JoNoNew.Rows[m]["JOB_ORDER_NO"] + "</td>";
            divBody.InnerHtml += "<td>" + JoNoNew.Rows[m]["BUYER"] + "</td>";
            divBody.InnerHtml += "<td>" + JoNoNew.Rows[m]["STYLE_NO"] + "</td>";
            divBody.InnerHtml += "<td>" + JoNoNew.Rows[m]["SC_NO"] + "</td>";
            divBody.InnerHtml += "<td>" + JoNoNew.Rows[m]["SAM"] + "</td>";
            divBody.InnerHtml += "<td>" + JoNoNew.Rows[m]["WASH_TYPE"] + "</td>";
            divBody.InnerHtml += "<td>" + JoNoNew.Rows[m]["SAH"] + "</td>";
            divBody.InnerHtml += "<td>" + JoNoNew.Rows[m]["BUYER_PO_DEL_DATE"] + "</td>";
            divBody.InnerHtml += "<td>" + JoNoNew.Rows[m]["PROD_COMPLETION_DATE"] + "</td>";
            divBody.InnerHtml += "<td>" + JoNoNew.Rows[m]["ORDER_QTY"] + "</td>";
            divBody.InnerHtml += "<td>" + JoNoNew.Rows[m]["QUANTITY"] + "</td>";

            DataTable tempList = new DataTable();
            for (int i = 0; i < pcmList.Columns.Count; i++)
            {
                tempList.Columns.Add(pcmList.Columns[i].ColumnName);
            }
            foreach (DataRow row in pcmList.Rows)
            {
                if (JoNoNew.Rows[m]["JOB_ORDER_NO"].ToString().Equals(row["JOB_ORDER_NO"].ToString()))
                {
                    tempList.ImportRow(row);
                }
            }
            for (int p = 0; p < nInAndOut.Rows.Count; p++)
            {
                long wastage = 0;
                int qtyInFirstData = 0;
                long stk = 0;
                bool flagOut = false;
                for (int i = 0; i < tempList.Rows.Count; i++)
                {
                    String proc = pcmList.Rows[i]["PROCESS_CD"].ToString();
                    if (proc.Equals(qtyOut[p, 0]))
                    {
                        int opening = int.Parse(pcmList.Rows[i]["OPENING_QTY"].ToString() == "" ? "0" : pcmList.Rows[i]["OPENING_QTY"].ToString());
                        divBody.InnerHtml += "<td>" + opening + "</td>";
                        qtyInFirstData = int.Parse(pcmList.Rows[i]["IN_QTY"].ToString() == "" ? "0" : pcmList.Rows[i]["IN_QTY"].ToString());
                        stk = stk + opening;
                        flagOut = true;
                        break;
                    }
                }
                if (flagOut == false)
                {
                    divBody.InnerHtml += "<TD></TD>";
                }
                if (p == 0)
                {
                    if (flagOut == true)
                    {
                        divBody.InnerHtml += "<TD>" + qtyInFirstData + "</TD>";
                        stk = stk + qtyInFirstData;
                    }
                    else
                    {
                        divBody.InnerHtml += "<TD></TD>";
                    }
                }
                else
                {
                    for (int k = 0; k < tempList.Rows.Count; k++)
                    {
                        if (tempList.Rows[k]["PROCESS_CD"].ToString().Equals(qtyIn[p, 0]))
                        {
                            int qtyInData = int.Parse(pcmList.Rows[k]["IN_QTY"].ToString());
                            if (p != 0)
                            {
                                stk = stk + qtyInData;
                            }
                            break;
                        }
                    }
                    bool flagIn = false;
                    for (int i = 0; i < tempList.Rows.Count; i++)
                    {
                        if (tempList.Rows[i]["NEXT_PROCESS_CD"].ToString().Equals(qtyIn[p, 0]))
                        {
                            flagIn = true;
                            for (int j = 1; j < nInAndOut.Rows.Count; j++)
                            {
                                if (qtyIn[p, j] == null)
                                    break;
                                bool flag = false;
                                for (int k = 0; k < tempList.Rows.Count; k++)
                                {
                                    if (tempList.Rows[k]["NEXT_PROCESS_CD"].ToString().Equals(qtyIn[p, 0]) && tempList.Rows[k]["PROCESS_CD"].ToString().Equals(qtyIn[p, j]))
                                    {
                                        divBody.InnerHtml += " <TD>" + tempList.Rows[i]["OUT_QTY"] + "</TD>";
                                        flag = true;
                                        break;
                                    }
                                }
                                if (flag == false)
                                {
                                    divBody.InnerHtml += "<TD></TD>";
                                }
                            }
                            break;
                        }
                    }
                    if (flagIn == false)
                    {
                        for (int k = 0; k < nIn[p]; k++)
                        {
                            divBody.InnerHtml += "<TD></TD>";
                        }
                    }
                }

                if (flagOut == false)
                {
                    for (int k = 0; k < nOut[p] + 2; k++)
                    {
                        divBody.InnerHtml += "<TD></TD>";
                    }
                }
                else
                {
                    for (int j = 1; j < nInAndOut.Rows.Count; j++)
                    {
                        if (qtyOut[p, j] == null)
                            break;
                        bool flag = false;
                        for (int k = 0; k < tempList.Rows.Count; k++)
                        {
                            if (tempList.Rows[k]["PROCESS_CD"].ToString().Equals(qtyOut[p, 0]) && tempList.Rows[k]["NEXT_PROCESS_CD"].ToString().Equals(qtyOut[p, j]))
                            {
                                divBody.InnerHtml += "<TD>" + tempList.Rows[k]["OUT_QTY"] + "</TD>";
                                int qtyOutData = int.Parse(tempList.Rows[k]["OUT_QTY"].ToString());
                                int wastageOne = int.Parse(tempList.Rows[k]["WASTAGE_QTY"].ToString());
                                wastage = wastage + wastageOne;
                                stk = stk - qtyOutData;
                                flag = true;
                                break;
                            }
                        }
                        if (flag == false)
                        {
                            divBody.InnerHtml += "<TD></TD>";
                        }
                    }
                    divBody.InnerHtml += "<TD>" + wastage + "</TD><TD>" + (stk - wastage) + "</TD>";
                }
            }
            divBody.InnerHtml += "</tr>";
        }
    }
    */
    //added By ZouShiChang ON 2013.09.20 End MES024

    protected void gvPcmDetail_RowCreated(object sender, GridViewRowEventArgs e)
    {
        int i = 0;
        //string[] arry;
        switch (e.Row.RowType)
        {

            case DataControlRowType.Header:

                //总表头清除
                TableCellCollection tcHeader = e.Row.Cells;
                tcHeader.Clear();

                //第一行表头

                int cols = 0;
                while (i < dtCol.Rows.Count)
                {


                    if (i < 11)
                    {
                        tcHeader.Add(new TableHeaderCell());
                        tcHeader[cols].Attributes.Add("bgcolor", "#fbfbfb");
                        tcHeader[cols].Attributes.Add("rowspan", "3");
                        tcHeader[cols].Text = dtCol.Rows[i]["COL1"].ToString();
                        i++;
                        cols++;
                    }
                    else
                    {
                        tcHeader.Add(new TableHeaderCell());
                        tcHeader[cols].Attributes.Add("bgcolor", "#fbfbfb");
                        tcHeader[cols].Attributes.Add("colspan", dtCol.Compute("COUNT(COL1)", "COL1=" + "'" + dtCol.Rows[i]["COL1"].ToString() + "'").ToString());
                        tcHeader[cols].Text = dtCol.Rows[i]["COL1"].ToString();
                        i = i + Convert.ToInt32(dtCol.Compute("COUNT(COL1)", "COL1=" + "'" + dtCol.Rows[i]["COL1"].ToString() + "'"));
                        cols++;
                    }

                }

                tcHeader[cols - 1].Text = dtCol.Rows[i - 1]["COL1"].ToString() + "</th></tr><tr>";


                int j = 11;
                //第二行表头
                while (j < dtCol.Rows.Count)
                {



                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[cols].Attributes.Add("bgcolor", "#fbfbfb");
                    tcHeader[cols].Attributes.Add("colspan", dtCol.Compute("COUNT(COL2)", "COL2=" + "'" + dtCol.Rows[j]["COL2"].ToString() + "' AND COL1=" + "'" + dtCol.Rows[j]["COL1"].ToString() + "'").ToString());
                    tcHeader[cols].Text = dtCol.Rows[j]["COL2"].ToString();

                    j = j + Convert.ToInt32(dtCol.Compute("COUNT(COL2)", "COL2=" + "'" + dtCol.Rows[j]["COL2"].ToString() + "' AND COL1=" + "'" + dtCol.Rows[j]["COL1"].ToString() + "'"));
                    cols++;
                }

                tcHeader[cols - 1].Text = dtCol.Rows[j - 1]["COL2"].ToString() + "</th></tr><tr>";


                int k = 11;

                //第三行表头
                while (k < dtCol.Rows.Count)
                {


                    tcHeader.Add(new TableHeaderCell());
                    tcHeader[cols].Attributes.Add("bgcolor", "#fbfbfb");
                    tcHeader[cols].Text = dtCol.Rows[k]["COL3"].ToString();
                    k++;
                    cols++;

                }

                break;
        }

    }

}
