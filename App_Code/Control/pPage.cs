using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Hosting;
using System.Threading;

/// <summary>
/// Summary description for pPage
/// </summary>

    public class pPage : System.Web.UI.Page
    {
        private static readonly string compilation = "P";//D:debug
        public const string CONST_LANGUAGE_COOKIE_NAME = "MES_RPT_LANGUAGE";

        protected override void InitializeCulture()
        {
            string factoryCd;

            base.InitializeCulture();
            if (Request.QueryString.Get("Culture") != null)
            {
                string cultureName = Request.QueryString.GetValues("Culture")[0];
                if (cultureName.Trim() != "")
                {
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cultureName.Trim());
                }
            }
            else
            {
                if (Page.Request.Cookies.AllKeys.Contains(CONST_LANGUAGE_COOKIE_NAME))
                {
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Page.Request.Cookies[CONST_LANGUAGE_COOKIE_NAME].Value);
                    Page.Response.AppendCookie(new HttpCookie(CONST_LANGUAGE_COOKIE_NAME, Thread.CurrentThread.CurrentUICulture.Name));

                }
                else
                {
                    if (Request.QueryString.Get("site") != null)
                    {
                        factoryCd = Request.QueryString.GetValues("site")[0].ToUpper();
                        if (factoryCd == "GEG" || factoryCd == "YMG" || factoryCd == "CEG" || factoryCd == "CEK" || factoryCd == "NBO" || factoryCd == "MDN" || factoryCd == "GLG")
                            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
                        else
                            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                    }   
                }
            }

        }

        protected  string CurrentSite
        {
            get { return Session["site"].ToString(); }
        }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["site"] == null)
                {
                    if (compilation == "D")
                    {
                        Session["site"] = "GEG";
                    }                    
                }
                else
                {
                    Session["site"] = (string)Request["site"];                  
                }

                if (Request.QueryString["svType"] != null)
                {
                    Session["svType"] = (string)Request["svType"];
                }
                else
                {
                    Session["svType"] = "PROD";
                }
            }
            else
            {
                //if (HttpContext.Current.Session["site"] == null)
                //        Server.Transfer(Request.Path, true);
            }

            if (HttpContext.Current.Session["site"] == null && !(Request.Cookies.AllKeys.Contains("Site") || Request.Cookies.AllKeys.Contains("site")))
            {
                Response.Redirect(HostingEnvironment.ApplicationVirtualPath + "/Reports/MessagePage.aspx?mes=No Site Information! ");
            }
            else
            {
                if (HttpContext.Current.Session["site"] == null)
                {
                    Session["site"] = Request.Cookies["Site"].Value;
                }
            }


            if (HttpContext.Current.Session["site"] == null)
            {
                Response.Redirect(HostingEnvironment.ApplicationVirtualPath + "/Reports/MessagePage.aspx?mes=Session Time Out! ");
            }
            else if (!HttpContext.Current.Request.Cookies.AllKeys.Contains("Site"))
            {
                var ck = new HttpCookie("Site", Session["site"].ToString());                
                Response.Cookies.Add(ck);
            }
        }

        protected void SetText(string txt)
        {
            //Jquery Autocomplete
            if (Request.QueryString["q"] != null && Request.QueryString["q"] != "")
            {
                Response.Clear(); Response.Charset = "utf-8";
                Response.Buffer = true;
                this.EnableViewState = false;
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.ContentType = "text/plain";
                Response.Write(txt);
                Response.Flush();
                Response.Close();
                Response.End();
            }
        }
        protected void beginProgress()
        {
            //根据ProgressBar.htm显示进度条界面
            string templateFileName = System.IO.Path.Combine(Server.MapPath("."), "ProgressBar.htm");
            System.IO.StreamReader reader = new System.IO.StreamReader(@templateFileName, System.Text.Encoding.GetEncoding("GB2312"));
            string html = reader.ReadToEnd();
            reader.Close();
            Response.Write(html);
            Response.Flush();
        }
        protected void SetTiile(string Strt)
        {
            string jsBlock = "<script>SetTiile('" + Strt + "'); </script>";
            Response.Write(jsBlock);
            Response.Flush();
        }
        protected void setProgress(int percent, string msg1, string msg2)
        {
            string jsBlock = "<script>SetPorgressBar('" + percent.ToString() + "','" + msg1 + "','" + msg2 + "'); </script>";
            Response.Write(jsBlock);
            Response.Flush();
        }

        protected void finishProgress()
        {
            string jsBlock = "<script>SetCompleted();</script>";
            Response.Write(jsBlock);
            Response.Flush();
        }
    }
