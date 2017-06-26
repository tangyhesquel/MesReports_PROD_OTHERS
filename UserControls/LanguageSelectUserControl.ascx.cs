using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.Globalization;

/// <summary>
/// 语言选择用户控件
/// created by tangp  at 2011-2-17
/// </summary>
public partial class UserControls_LanguageSelectUserControl : System.Web.UI.UserControl
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Thread.CurrentThread.CurrentUICulture.Name == "zh-CN")
            {
                RadioButton_ZH_CHS.Checked = true;
                RadioButton_EN_US.Checked = false;
            }
            else
            {
                RadioButton_ZH_CHS.Checked = false;
                RadioButton_EN_US.Checked = true;
            }
        }
    }

    private void UpdateLangCookie(string cultureName)
    {
        HttpCookie cookie;
        if (Page.Request.Cookies.AllKeys.Contains(pPage.CONST_LANGUAGE_COOKIE_NAME))
            Page.Request.Cookies[pPage.CONST_LANGUAGE_COOKIE_NAME].Value = cultureName;
        else
        {
            cookie = new HttpCookie(pPage.CONST_LANGUAGE_COOKIE_NAME, cultureName);
            cookie.Expires = DateTime.MaxValue;
            Page.Request.Cookies.Add(cookie);
        }
    }

    private CultureInfo SelectedCulture
    {
        get 
        {
            if (RadioButton_ZH_CHS.Checked)
                return new CultureInfo("zh-CN");
            else
                return new CultureInfo("en-US");
        }
    }


    protected void RadioButton_ZH_CHS_CheckedChanged(object sender, EventArgs e)
    {
        UpdateLangCookie(SelectedCulture.Name);
        Server.Transfer(Page.Request.Path);
    }
    protected void RadioButton_EN_US_CheckedChanged(object sender, EventArgs e)
    {
        UpdateLangCookie(SelectedCulture.Name);
        Server.Transfer(Page.Request.Path);
    }



}
