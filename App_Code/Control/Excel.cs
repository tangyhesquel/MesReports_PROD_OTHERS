using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;

namespace MESComment
{
    /// <summary>
    /// Summary description for Excel
    /// </summary>
    public class Excel
    {
        public Excel()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static void ToExcel(HtmlGenericControl gv, string strFileName)
        {
            if (strFileName.ToLower().IndexOf(".xls") <= 0)
            {
                strFileName += ".xls";
            }
            try
            {
                StringWriter tw;
                tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                gv.RenderControl(hw);


                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "UTF8";
                HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;//×¢Òâ±àÂë
                HttpContext.Current.Response.AppendHeader("Content-Disposition",
                    "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8).ToString());
                HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";

                HttpContext.Current.Response.Write(tw.ToString());

                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.End();
                HttpContext.Current.Response.Write("<script>alert('" + ex.Message + "')</script>");

            }
        }

        public static void ToExcel(GridView gv, string strFileName)
        {
            if (strFileName.ToLower().IndexOf(".xls") <= 0)
            {
                strFileName += ".xls";
            }
            try
            {
                StringWriter tw;
                tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                gv.RenderControl(hw);


                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "UTF-8";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                HttpContext.Current.Response.Charset = "UTF-8"; //ÉèÖÃ±àÂëµÄ      

                HttpContext.Current.Response.AppendHeader("Content-Disposition",
                    "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8).ToString());
                HttpContext.Current.Response.ContentType = "application/ms-excel";

                HttpContext.Current.Response.Write(tw.ToString());
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.End();
                HttpContext.Current.Response.Write("<script>alert('" + ex.Message + "')</script>");

            }
        }
        public static void ToExcel(GridView gv, string strFileName, string str, int col)
        {
            if (strFileName.ToLower().IndexOf(".xls") <= 0)
            {
                strFileName += ".xls";
            }
            try
            {
                StringWriter tw;
                tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                gv.RenderControl(hw);


                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "UTF-8";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                HttpContext.Current.Response.Charset = "UTF-8"; //ÉèÖÃ±àÂëµÄ      

                HttpContext.Current.Response.AppendHeader("Content-Disposition",
                    "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8).ToString());
                HttpContext.Current.Response.ContentType = "application/ms-excel";

                string html = tw.ToString();
                if (strFileName.Contains("ProYMGStatusQuety"))
                {
                    int begin = 0;
                    if (html.IndexOf("<table") > 0)
                    {
                        begin = html.IndexOf("<table");
                    }
                    html = html.Substring(begin);
                    html = @"<div><table cellspacing='0' rules='all' border='0' id='Header' style='width:100%;border-collapse:collapse;'><tr>
                                 <td colspan='" + col + "' align='center' style='font-size:medium'><b>" + str + "</b></td><tr></table>" + html;
                    if (html.LastIndexOf("</tr><tr>") > 0)
                    {
                        begin = html.LastIndexOf("</tr><tr>");
                        html = html.Substring(0, begin) + html.Substring(begin).Replace("</tr><tr>", "</tr><tr style='font-weight:bolder'>");
                    }
                    if (html.IndexOf("</div>") <= 0)
                    {
                        html += "</div>";
                    }
                }
                //HttpContext.Current.Response.Write(tw.ToString());
                HttpContext.Current.Response.Write(html);
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.End();
                HttpContext.Current.Response.Write("<script>alert('" + ex.Message + "')</script>");

            }
        }

        public static void ToExcel(List<GridView> gvList, string strFileName)
        {
            if (strFileName.ToLower().IndexOf(".xls") <= 0)
            {
                strFileName += ".xls";
            }
            try
            {
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "UTF-8";
                HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;//×¢Òâ±àÂë
                HttpContext.Current.Response.AppendHeader("Content-Disposition",
                    "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8).ToString());
                HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";

                for (int i = 0; i < gvList.Count; i++)
                {
                    StringWriter tw;
                    tw = new StringWriter();
                    HtmlTextWriter hw = new HtmlTextWriter(tw);
                    GridView gv = (GridView)gvList[i];
                    gv.RenderControl(hw);
                    HttpContext.Current.Response.Write(tw.ToString());
                }


                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write("<script>alert('" + ex.Message + "')</script>");
                HttpContext.Current.Response.End();
            }
        }

        public static void ToExcel2(GridView gv, string strFileName)
        {
            if (strFileName.ToLower().IndexOf(".xls") <= 0)
            {
                strFileName += ".xls";
            }
            if (gv.Rows.Count <= 0)
            {
                ShowMsg("Please Query first");
                return;
            }
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "UTF-8";
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;//×¢Òâ±àÂë
            HttpContext.Current.Response.AppendHeader("Content-Disposition",
                "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8).ToString());
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            gv.Page.EnableViewState = false;
            StringWriter tw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(tw);
            gv.RenderControl(hw);
            HttpContext.Current.Response.Write(tw.ToString());
            HttpContext.Current.Response.End();
        }

        public static void ToWPS(GridView gv, string strFileName, string str, int col)
        {
            if (strFileName.ToLower().IndexOf(".et") <= 0)
            {
                strFileName += ".et";
            }
            try
            {
                StringWriter tw;
                tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                gv.RenderControl(hw);


                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "UTF-8";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                HttpContext.Current.Response.Charset = "UTF-8"; //ÉèÖÃ±àÂëµÄ      

                HttpContext.Current.Response.AppendHeader("Content-Disposition",
                    "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8).ToString());
                HttpContext.Current.Response.ContentType = "application/ms-excel";

                string html = tw.ToString();
                if (strFileName.Contains("ProYMGStatusQuety"))
                {
                    int begin = 0;
                    if (html.IndexOf("<table") > 0)
                    {
                        begin = html.IndexOf("<table");
                    }
                    html = html.Substring(begin);
                    html = @"<div><table cellspacing='0' rules='all' border='0' id='Header' style='width:100%;border-collapse:collapse;'><tr>
                                 <td colspan='" + col + "' align='center' style='font-size:medium'><b>" + str + "</b></td><tr></table>" + html;
                    if (html.LastIndexOf("</tr><tr>") > 0)
                    {
                        begin = html.LastIndexOf("</tr><tr>");
                        html = html.Substring(0, begin) + html.Substring(begin).Replace("</tr><tr>", "</tr><tr style='font-weight:bolder'>");
                    }
                    if (html.IndexOf("</div>") <= 0)
                    {
                        html += "</div>";
                    }
                }
                //HttpContext.Current.Response.Write(tw.ToString());
                HttpContext.Current.Response.Write(html);
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.End();
                HttpContext.Current.Response.Write("<script>alert('" + ex.Message + "')</script>");

            }
        }

        public static void ShowMsg(string strMsg)
        {

            strMsg = strMsg.Replace("\n", "").Replace("<br>", "").Replace("'", "").Replace("\r", "");

            HttpContext.Current.Response.Write(String.Format("<script language='javascript'>alert('{0}');</script>", strMsg));

            return;

        }


    }
}