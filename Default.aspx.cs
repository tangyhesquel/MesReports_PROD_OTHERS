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
using System.Drawing;

public partial class _Default : pPage
{
    double width, Day;
    //0:Title,1:Calendar,2:Left,3:Right,4:Holiday,5:Active,6:Closed 7:Delay 8:Testing
    //string Back_Color = "#c0c0c0,#98fb98,#98fb98,white,#e6e6fa,blue,#778899,#ffb6c1,yellow";

    string Back_Color = "#c0c0c0,#98fb98,#98fb98,white,#e6e6fa,blue,#778899,#A00000,yellow";
    string Handler, Group;
    protected void Page_Load(object sender, EventArgs e)
    {
        AjaxPro.Utility.RegisterTypeForAjax(typeof(_Default));

        if (Session["UserName"] == null)
        {
            Response.Redirect("GetUserName.aspx");
        }

        if (!IsPostBack)
        {
            this.txtBeginDate.Text = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd");
            this.txtEndDate.Text = DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd");
            MESComment.MesRpt.Ini_Weekly_Reports_DDL(ddlSystem, "");
            MESComment.MesRpt.Ini_Weekly_Reports_DDL(ddlSubSystem, ddlSystem.SelectedItem.Value);
            MESComment.MesRpt.Ini_Weekly_Reports_DDL(ddlHandler, "");
            ClientScript.RegisterStartupScript(this.GetType(), "", "<script>DataBind('')</script>");
        }
    }

    private bool CheckQueryCondition()
    {
        Day = double.Parse((DateTime.Parse(txtEndDate.Text) - DateTime.Parse(txtBeginDate.Text)).TotalDays.ToString()) + 1;

        if (Day > 100)
        {
            Response.Write("<script>alert('The days between StrartDate and EndDate must be in three month! Please select again!')</script>");
            Day = 100;
            txtEndDate.Text = DateTime.Parse(txtBeginDate.Text).AddDays(99).ToString("yyyy-MM-dd");
        }
        return true;
    }

    private void SetQueryData()
    {
        if (!cbIsNew.Checked)
        {
            gvNewSRF.Visible = false;
            gvBody.Visible = true;
            if (CheckQueryCondition())
            {
                width = 100 / (int.Parse(Day.ToString()));
                if (ddlHandler.SelectedIndex >= 0)
                {
                    Handler = ddlHandler.SelectedItem.Value;
                }
                else
                {
                    Handler = "";
                }
                if (hfSelectValue.Value != "")
                {
                    Group = hfSelectValue.Value;
                }
                else
                {
                    Group = "";
                }
                this.gvBody.DataSource = MESComment.MesRpt.Get_Weekly_Reports(ddlSystem.SelectedItem.Value, ddlSubSystem.SelectedItem.Value, Handler, Group, txtBeginDate.Text, txtEndDate.Text, ddlOrderBy.SelectedItem.Value, cbIsNew.Checked);
                this.gvBody.DataBind();
                for (int i = 0; i < gvBody.Rows.Count; i++)
                {
                    gvBody.Rows[i].Cells[0].Text = SubStr(gvBody.Rows[i].Cells[0].Text, 6);
                    gvBody.Rows[i].Cells[1].Text = SubStr(gvBody.Rows[i].Cells[1].Text, 24);
                    gvBody.Rows[i].Cells[3].Text = SubStr(gvBody.Rows[i].Cells[3].Text, 13);
                }
            }
        }
        else
        {
            gvNewSRF.Visible = true;
            gvBody.Visible = false;
            if (hfSelectValue.Value != "")
            {
                Group = hfSelectValue.Value;
            }
            else
            {
                Group = "";
            }
            this.gvNewSRF.DataSource = MESComment.MesRpt.Get_Weekly_Reports_New(ddlSystem.SelectedItem.Value, ddlSubSystem.SelectedItem.Value, Group, ddlOrderBy.SelectedItem.Value);
            this.gvNewSRF.DataBind();
        }
    }

    public string SubStr(string sString, int nLeng)
    {
        if (sString.Length <= nLeng)
        {
            return sString;
        }
        string sNewStr = sString.Substring(0, nLeng);
        sNewStr = sNewStr + "...";
        return sNewStr;
    }

    protected void btnEnter_Click(object sender, EventArgs e)
    {
        SetQueryData();
        ClientScript.RegisterStartupScript(this.GetType(), "", "<script>DataBind('" + Group + "')</script>");
    }

    private string GenarateCaption()
    {
        string HtmlStr = "";
        DateTime begin = Convert.ToDateTime(txtBeginDate.Text), time;
        double count = 0;
        HtmlStr += "<table width='100%' border='1' cellpadding='0' cellspacing='0' style='border-collapse: collapse;'><tr style='background:" + Back_Color.Split(',')[1].ToString() + ";font-weight:bold'>";
        for (int i = 0; i < Day; i++)
        {
            if (begin.DayOfWeek.ToString().Contains("M"))
            {
                double num = (Convert.ToDateTime(txtEndDate.Text) - begin).TotalDays + 1;
                if (num >= 7)//中日期
                {
                    HtmlStr += "<td style='text-align:left' colspan='7' width='" + 7 * width + "%'>" + begin.ToString("yyyy-MM-dd") + "</td>";
                }
                else//尾日期
                {
                    if (num == 6)
                    {
                        HtmlStr += "<td style='text-align:left' colspan=" + num + " width='" + (num * width) + "%'>" + begin.ToString("yyyy-MM-dd") + "</td>";
                    }
                    else
                    {
                        HtmlStr += "<td colspan=" + num + " width='" + (num * width) + "%'></td>";
                    }
                }
            }
            else if (begin == Convert.ToDateTime(txtBeginDate.Text))//头日期
            {
                time = begin;
                for (int j = 0; j < 7; j++)
                {
                    if (time.DayOfWeek.ToString().Contains("M"))
                    {
                        count = (time - begin).TotalDays;
                        break;
                    }
                    time = time.AddDays(1);
                }
                if (count >= 6)
                {
                    HtmlStr += "<td style='text-align:left' colspan=" + count + " width='" + count + "%'>" + begin.ToString("yyyy-MM-dd") + "</td>";
                }
                else
                {
                    HtmlStr += "<td colspan=" + count + " width='" + (count * width) + "%'></td>";
                }
            }
            begin = begin.AddDays(1);
        }
        HtmlStr += "</tr><tr style='background:" + Back_Color.Split(',')[1].ToString() + ";font-size:10'>";
        begin = Convert.ToDateTime(txtBeginDate.Text);
        for (int i = 0; i < Day; i++)
        {
            if (begin.Day < 10)
            {
                HtmlStr += "<td width='" + width + "%'>0" + begin.Day + "</td>";
            }
            else
            {
                HtmlStr += "<td width='" + width + "%'>" + begin.Day + "</td>";
            }
            begin = begin.AddDays(1);
        }
        HtmlStr += "</tr><tr style='background:" + Back_Color.Split(',')[1].ToString() + ";font-size:10'>";
        begin = Convert.ToDateTime(txtBeginDate.Text);
        for (int i = 0; i < Day; i++)
        {
            switch (begin.DayOfWeek.ToString().Substring(2, 1))
            {
                case "n":
                    if (begin.DayOfWeek.ToString().Substring(0, 1) == "M")
                    {
                        HtmlStr += "<td width='" + width + "%'>一</td>";
                    }
                    else
                    {
                        HtmlStr += "<td width='" + width + "%'>日</td>";
                    }
                    break;
                case "e":
                    HtmlStr += "<td width='" + width + "%'>二</td>";
                    break;
                case "d":
                    HtmlStr += "<td width='" + width + "%'>三</td>";
                    break;
                case "u":
                    HtmlStr += "<td width='" + width + "%'>四</td>";
                    break;
                case "i":
                    HtmlStr += "<td width='" + width + "%'>五</td>";
                    break;
                case "t":
                    HtmlStr += "<td width='" + width + "%'>六</td>";
                    break;
            }
            begin = begin.AddDays(1);
        }
        HtmlStr += "</tr></table>";
        return HtmlStr;
    }

    private string GenerateTable(string id, string status)
    {
        string table = "";
        table += "<table width='100%' border='1' cellpadding='0' cellspacing='0' style='border-collapse: collapse;height:20px;border-right-width:0'><tr style='background:" + Back_Color.Split(',')[3].ToString() + "'>";
        if (status == "OT")
        {
            table += "<td id='" + id + "' onmousemove='showDiv(id)' onmouseout='hideDiv()' style='background:" + Back_Color.Split(',')[8].ToString() + "' width='40px'></td></tr></table>";
        }
        else if (status == "OD")
        {
            table += "<td id='" + id + "' onmousemove='showDiv(id)' onmouseout='hideDiv()' style='background:" + Back_Color.Split(',')[7].ToString() + "' width='40px'></td></tr></table>";
        }
        else
        {
            table += "<td width='40px'></td></tr></table>";
        }
        return table;
    }

    private string GenerateTable(string id, DateTime BeginDate, DateTime EndDate, string status)
    {
        string table = "";
        DateTime begin = Convert.ToDateTime(txtBeginDate.Text);
        table += "<table width='100%' border='1' cellpadding='0' cellspacing='0' style='border-collapse: collapse;height:20px;border-right-width:0'><tr style='background:" + Back_Color.Split(',')[3].ToString() + "'>";
        for (int i = 1; i <= Day; i++)
        {
            if (begin < BeginDate || begin > EndDate)
            {
                if (begin.DayOfWeek.ToString().Contains("S"))
                {
                    table += "<td style='background:" + Back_Color.Split(',')[4].ToString() + "' width='" + width + "%'></td>";
                }
                else
                {
                    table += "<td width='" + width + "%'></td>";
                }
            }
            if (begin == BeginDate)
            {
                switch (status)
                {
                    case "C":
                        table += "<td id='" + id + "' onmousemove='showDiv(id)' onmouseout='hideDiv()' colspan='" + ((EndDate - BeginDate).TotalDays + 1) + "' width='" + ((EndDate - BeginDate).TotalDays + 1) * width + "%' style='background:" + Back_Color.Split(',')[6].ToString() + "'></td>";
                        break;
                    case "D":
                        table += "<td id='" + id + "' onmousemove='showDiv(id)' onmouseout='hideDiv()' colspan='" + ((EndDate - BeginDate).TotalDays + 1) + "' width='" + ((EndDate - BeginDate).TotalDays + 1) * width + "%' style='background:" + Back_Color.Split(',')[7].ToString() + "'></td>";
                        break;
                    case "A":
                        table += "<td id='" + id + "' onmousemove='showDiv(id)' onmouseout='hideDiv()' colspan='" + ((EndDate - BeginDate).TotalDays + 1) + "' width='" + ((EndDate - BeginDate).TotalDays + 1) * width + "%' style='background:" + Back_Color.Split(',')[5].ToString() + "'></td>";
                        break;
                    case "T":
                        table += "<td id='" + id + "' onmousemove='showDiv(id)' onmouseout='hideDiv()' colspan='" + ((EndDate - BeginDate).TotalDays + 1) + "' width='" + ((EndDate - BeginDate).TotalDays + 1) * width + "%' style='background:" + Back_Color.Split(',')[8].ToString() + "'></td>";
                        break;
                    case "OT":
                    case "OD":
                        if (begin.DayOfWeek.ToString().Contains("S"))
                        {
                            table += "<td style='background:" + Back_Color.Split(',')[4].ToString() + "' width='" + width + "%'></td>";
                        }
                        else
                        {
                            table += "<td width='" + width + "%'></td>";
                        }
                        break;
                }
            }
            begin = begin.AddDays(1);
        }
        table += "</tr>";

        table += "</table>";
        return table;
    }

    protected void gvBody_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //e.Row.Cells[4].Visible = false;
        e.Row.Cells[6].Visible = false;
        e.Row.Cells[7].Visible = false;
        e.Row.Cells[8].Visible = false;
        switch (e.Row.RowType)
        {
            case DataControlRowType.Header:
                e.Row.BackColor = Color.FromName(Back_Color.Split(',')[0].ToString());
                e.Row.Cells[4].BackColor = Color.FromName(Back_Color.Split(',')[1].ToString());
                e.Row.Cells[4].Text = "Overdue";
                e.Row.Cells[5].Text = GenarateCaption();
                break;
            case DataControlRowType.DataRow:
                e.Row.Cells[0].BackColor = Color.FromName(Back_Color.Split(',')[2].ToString());
                e.Row.Cells[1].BackColor = Color.FromName(Back_Color.Split(',')[2].ToString());
                e.Row.Cells[3].BackColor = Color.FromName(Back_Color.Split(',')[2].ToString());
                e.Row.Cells[4].Text = GenerateTable(e.Row.Cells[2].Text, e.Row.Cells[6].Text);
                e.Row.Cells[5].Text = GenerateTable(e.Row.Cells[2].Text, DateTime.Parse(Convert.ToDateTime(e.Row.Cells[7].Text).ToShortDateString()), DateTime.Parse(Convert.ToDateTime(e.Row.Cells[8].Text).ToShortDateString()), e.Row.Cells[6].Text);
                e.Row.Cells[2].Text = string.Format("<a href='#' onclick='window.open (\"http://get03c0062/WeeklyReport/ReportBodyDetail.aspx?Note_No={0}\",\"\",\"\")'>{0}</a>", e.Row.Cells[2].Text);
                e.Row.Cells[2].BackColor = Color.FromName(Back_Color.Split(',')[2].ToString());
                break;
        }
    }

    protected void ddlSystem_SelectedIndexChanged(object sender, EventArgs e)
    {
        MESComment.MesRpt.Ini_Weekly_Reports_DDL(ddlSubSystem, ddlSystem.SelectedItem.Value);
        gvBody.Visible = false;
        ClientScript.RegisterStartupScript(this.GetType(), "", "<script>DataBind('')</script>");
    }

    [AjaxPro.AjaxMethod]
    public string GenerateDivContent(string id)
    {
        string DivHtml = "";
        DataTable dt = MESComment.MesRpt.Get_SRF_ByNo(id);
        DivHtml += "Begin Date：" + dt.Rows[0]["ori_begin_time"] + "<br/>";
        DivHtml += "End   Date：" + dt.Rows[0]["ori_end_time"] + "<br/>";
        DivHtml += "Handler ：" + dt.Rows[0]["handler"] + "<br/>";
        DivHtml += "WorkDays  ：" + dt.Rows[0]["workdays"] + "<br/>";
        DivHtml += "Subject   ：" + dt.Rows[0]["subject"] + "<br/>";
        return DivHtml;
    }

    [AjaxPro.AjaxMethod]
    public DataTable GetDllDataSource()
    {
        return MESComment.MesRpt.Get_DDL_DataSource("ddlGroup", Session["UserName"].ToString());
    }
    
    protected void gvNewSRF_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        switch (e.Row.RowType)
        {
            case DataControlRowType.DataRow:
                e.Row.Cells[2].Text = string.Format("<a href='#' onclick='window.open (\"http://get03c0062/WeeklyReport/ReportBodyDetail.aspx?Note_No={0}\",\"\",\"\")'>{0}</a>", e.Row.Cells[2].Text);
                break;
        }
    }
}
