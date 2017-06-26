using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.Hosting;
using System.Data.Common;
using System.Collections.Generic;
using MySql.Data;
using System.Data.OracleClient;
using System.Data.OleDb;

/// <summary>
/// Summary description for DBUtility
/// </summary>
namespace MESComment
{
    public class DBUtility
    {

        /// <summary>
        /// 获取数据库连接语句

        /// </summary>
        /// <param name="serverType">服务器类型</param>
        /// <returns></returns>
        public static DbConnection GetConnectionString(string strServerType)
        {
            string connectionString = "";
            string providerName = "";
            DbConnection conn = null;
            //string site = HttpContext.Current.Request.QueryString.Get("SITE").ToString().ToUpper();
            string site = HttpContext.Current.Session["site"].ToString().ToUpper();
            string svType = null;
            if (HttpContext.Current.Session["svType"].ToString().ToUpper() != null)
            {
                svType = HttpContext.Current.Session["svType"].ToString().ToUpper();
            }
            //switch (HttpContext.Current.Session["site"].ToString().ToUpper())
            //if (HttpContext.Current.Request.QueryString.Get("FACTORY_CD") != null)
            //{
            //    site = HttpContext.Current.Request.QueryString.Get("FACTORY_CD").ToString().ToUpper();
            //}
            //else if (HttpContext.Current.Request.QueryString.Get("FACTORY") != null)
            //{
            //    site = HttpContext.Current.Request.QueryString.Get("FACTORY").ToString().ToUpper();
            //}
            switch (site)
            {
                case "GEG":
                    switch (strServerType)
                    {
                        case "MES":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGEG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGEG"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGEGTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGEGTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGEGSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGEGSTG"].ProviderName;
                                    break;
                                case "DEV":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESDEV_UPDATE"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESDEV_UPDATE"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGEG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGEG"].ProviderName;
                                    break;
                            }
                            break;
                        case "INV":
                            connectionString = ConfigurationManager.ConnectionStrings["INV7.222"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["INV7.222"].ProviderName;
                            break;
                        case "inv_support":
                            connectionString = ConfigurationManager.ConnectionStrings["inv_support"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["inv_support"].ProviderName;
                            break;

                        case "MES_UPDATE":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGEG_UPDATE"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGEG_UPDATE"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGEGTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGEGTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGEGSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGEGSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGEG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGEG"].ProviderName;
                                    break;
                            }
                            break;
                        case "WeeklyReport":
                            connectionString = ConfigurationManager.ConnectionStrings["WeeklyReport"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["WeeklyReport"].ProviderName;
                            break;
                        case "WIP":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_WIP"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_WIP"].ProviderName;
                            break;
                        case "GIS":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ProviderName;
                            break;
                        case "EEL":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_EEL"].ProviderName;
                            break;
                        case "EASN":
                            connectionString = ConfigurationManager.ConnectionStrings["EASNGEG"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["EASNGEG"].ProviderName;
                            break;
                        case "test":
                            connectionString = ConfigurationManager.ConnectionStrings["test"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["test"].ProviderName;
                            break;
                        case "HL":
                            connectionString = ConfigurationManager.ConnectionStrings["HL_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["HL_EEL"].ProviderName;
                            break;
                        default:
                            break;
                    }
                    break;
                case "MDN":
                    switch (strServerType)
                    {
                        case "MES":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGEG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGEG"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGEGTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGEGTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGEGSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGEGSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGEG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGEG"].ProviderName;
                                    break;
                            }

                            break;
                        case "INV":
                            connectionString = ConfigurationManager.ConnectionStrings["INV7.222"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["INV7.222"].ProviderName;
                            break;
                        case "MES_UPDATE":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGEG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGEG"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGEGTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGEGTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGEGSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGEGSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGEG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGEG"].ProviderName;
                                    break;
                            }
                            break;
                        case "WeeklyReport":
                            connectionString = ConfigurationManager.ConnectionStrings["WeeklyReport"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["WeeklyReport"].ProviderName;
                            break;
                        case "WIP":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_WIP"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_WIP"].ProviderName;
                            break;
                        case "GIS":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ProviderName;
                            break;
                        case "EEL":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_EEL"].ProviderName;
                            break;
                        case "EASN":
                            connectionString = ConfigurationManager.ConnectionStrings["EASNGEG"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["EASNGEG"].ProviderName;
                            break;
                        case "inv_support":
                            connectionString = ConfigurationManager.ConnectionStrings["inv_support"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["inv_support"].ProviderName;
                            break;
                        case "HL":
                            connectionString = ConfigurationManager.ConnectionStrings["HL_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["HL_EEL"].ProviderName;
                            break;
                        default:
                            break;
                    }
                    break;
                case "YMG":
                    switch (strServerType)
                    {
                        case "MES":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESYMG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESYMG"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESYMGTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESYMGTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESYMGSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESYMGSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESYMG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESYMG"].ProviderName;
                                    break;
                            }
                            break;
                        case "INV":
                            connectionString = ConfigurationManager.ConnectionStrings["INV7.222"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["INV7.222"].ProviderName;
                            break;
                        case "inv_support":
                            connectionString = ConfigurationManager.ConnectionStrings["inv_support"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["inv_support"].ProviderName;
                            break;
                        case "MES_UPDATE":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESYMG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESYMG"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESYMGTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESYMGTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESYMGSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESYMGSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESYMG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESYMG"].ProviderName;
                                    break;
                            }
                            break;
                        case "WeeklyReport":
                            connectionString = ConfigurationManager.ConnectionStrings["WeeklyReport"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["WeeklyReport"].ProviderName;
                            break;
                        case "WIP":
                            connectionString = ConfigurationManager.ConnectionStrings["MESYMG_WIP"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESYMG_WIP"].ProviderName;
                            break;
                        default:
                            break;
                        case "GIS":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ProviderName;
                            break;
                        case "EEL":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_EEL"].ProviderName;
                            break;
                        case "EASN":
                            connectionString = ConfigurationManager.ConnectionStrings["EASNYMG"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["EASNYMG"].ProviderName;
                            break;
                        case "HL":
                            connectionString = ConfigurationManager.ConnectionStrings["HL_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["HL_EEL"].ProviderName;
                            break;
                    }
                    break;

                case "NBO":
                    switch (strServerType)
                    {
                        case "MES":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESNBO"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESNBO"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESNBOTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESNBOTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESNBOSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESNBOSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESNBO"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESNBO"].ProviderName;
                                    break;
                            }
                            break;
                        case "INV":
                            connectionString = ConfigurationManager.ConnectionStrings["INV7.222"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["INV7.222"].ProviderName;
                            break;
                        case "MES_UPDATE":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESNBO"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESNBO"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESNBOTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESNBOTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESNBOSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESNBOSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESNBO"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESNBO"].ProviderName;
                                    break;
                            }
                            break;
                        case "WeeklyReport":
                            connectionString = ConfigurationManager.ConnectionStrings["WeeklyReport"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["WeeklyReport"].ProviderName;
                            break;
                        case "WIP":
                            connectionString = ConfigurationManager.ConnectionStrings["MESYMG_WIP"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESYMG_WIP"].ProviderName;
                            break;
                        case "GIS":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ProviderName;
                            break;
                        case "EEL":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_EEL"].ProviderName;
                            break;
                        case "inv_support":
                            connectionString = ConfigurationManager.ConnectionStrings["inv_support_NBO"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["inv_support_NBO"].ProviderName;
                            break;
                        case "EASN":
                            connectionString = ConfigurationManager.ConnectionStrings["EASNNBO"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["EASNNBO"].ProviderName;
                            break;
                        case "HL":
                            connectionString = ConfigurationManager.ConnectionStrings["HL_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["HL_EEL"].ProviderName;
                            break;
                        default:
                            break;
                    }
                    break;
                case "CEG":
                    switch (strServerType)
                    {
                        case "MES":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESCEG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESCEG"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESCEGTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESCEGTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESCEGSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESCEGSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESCEG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESCEG"].ProviderName;
                                    break;
                            }
                            break;
                        case "INV":
                            connectionString = ConfigurationManager.ConnectionStrings["INV20.11"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["INV20.11"].ProviderName;
                            break;
                        case "MES_UPDATE":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESCEG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESCEG"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESCEGTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESCEGTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESCEGSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESCEGSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESCEG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESCEG"].ProviderName;
                                    break;
                            }
                            break;
                        case "WeeklyReport":
                            connectionString = ConfigurationManager.ConnectionStrings["WeeklyReport"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["WeeklyReport"].ProviderName;
                            break;
                        case "WIP":
                            connectionString = ConfigurationManager.ConnectionStrings["MESYMG_WIP"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESYMG_WIP"].ProviderName;
                            break;
                        case "GIS":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ProviderName;
                            break;
                        case "EEL":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_EEL"].ProviderName;
                            break;
                        case "inv_support":
                            connectionString = ConfigurationManager.ConnectionStrings["inv_support_CEG"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["inv_support_CEG"].ProviderName;
                            break;
                        case "EASN":
                            connectionString = ConfigurationManager.ConnectionStrings["EASNCEG"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["EASNCEG"].ProviderName;
                            break;
                        case "HL":
                            connectionString = ConfigurationManager.ConnectionStrings["HL_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["HL_EEL"].ProviderName;
                            break;
                        default:
                            break;
                    }
                    break;

                case "CEK":
                    switch (strServerType)
                    {
                        case "MES":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESCEK"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESCEK"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESCEKTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESCEKTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESCEKSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESCEKSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESCEK"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESCEK"].ProviderName;
                                    break;
                            }
                            break;
                        case "INV":
                            connectionString = ConfigurationManager.ConnectionStrings["INV20.11"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["INV20.11"].ProviderName;
                            break;
                        case "MES_UPDATE":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESCEK"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESCEK"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESCEKTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESCEKTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESCEKSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESCEKSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESCEK"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESCEK"].ProviderName;
                                    break;
                            }
                            break;
                        case "WeeklyReport":
                            connectionString = ConfigurationManager.ConnectionStrings["WeeklyReport"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["WeeklyReport"].ProviderName;
                            break;
                        case "WIP":
                            connectionString = ConfigurationManager.ConnectionStrings["MESYMG_WIP"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESYMG_WIP"].ProviderName;
                            break;
                        case "GIS":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ProviderName;
                            break;
                        case "EEL":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_EEL"].ProviderName;
                            break;
                        case "inv_support":
                            connectionString = ConfigurationManager.ConnectionStrings["inv_support_CEG"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["inv_support_CEG"].ProviderName;
                            break;
                        case "EASN":
                            connectionString = ConfigurationManager.ConnectionStrings["EASNCEK"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["EASNCEK"].ProviderName;
                            break;
                        case "HL":
                            connectionString = ConfigurationManager.ConnectionStrings["HL_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["HL_EEL"].ProviderName;
                            break;
                        default:
                            break;
                    }
                    break;

                case "TIL":
                    switch (strServerType)
                    {
                        case "MES":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESTIL"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESTIL"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESTILTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESTILTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESTILSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESTILSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESTIL"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESTIL"].ProviderName;
                                    break;
                            }
                            break;
                        case "INV":
                            connectionString = ConfigurationManager.ConnectionStrings["INV7.222"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["INV7.222"].ProviderName;
                            break;
                        case "MES_UPDATE":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESTIL"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESTIL"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESTILTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESTILTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESTILSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESTILSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESTIL"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESTIL"].ProviderName;
                                    break;
                            }
                            break;
                        case "WeeklyReport":
                            connectionString = ConfigurationManager.ConnectionStrings["WeeklyReport"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["WeeklyReport"].ProviderName;
                            break;
                        case "WIP":
                            connectionString = ConfigurationManager.ConnectionStrings["MESYMG_WIP"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESYMG_WIP"].ProviderName;
                            break;
                        case "inv_support":
                            connectionString = ConfigurationManager.ConnectionStrings["inv_support_TIL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["inv_support_TIL"].ProviderName;
                            break;
                        case "EASN":
                            connectionString = ConfigurationManager.ConnectionStrings["EASNTIL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["EASNTIL"].ProviderName;
                            break;
                        case "HL":
                            connectionString = ConfigurationManager.ConnectionStrings["HL_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["HL_EEL"].ProviderName;
                            break;
                        default:
                            break;
                    }
                    break;
                case "DEV":
                    switch (strServerType)
                    {
                        case "MES":
                            connectionString = ConfigurationManager.ConnectionStrings["MESDEV"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESDEV"].ProviderName;
                            break;
                        case "INV":
                            connectionString = ConfigurationManager.ConnectionStrings["INV7.222"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["INV7.222"].ProviderName;
                            break;
                        case "inv_support":
                            connectionString = ConfigurationManager.ConnectionStrings["inv_support"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["inv_support"].ProviderName;
                            break;
                        case "MES_UPDATE":
                            connectionString = ConfigurationManager.ConnectionStrings["MESDEV_UPDATE"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESDEV_UPDATE"].ProviderName;
                            break;
                        case "WeeklyReport":
                            connectionString = ConfigurationManager.ConnectionStrings["WeeklyReport"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["WeeklyReport"].ProviderName;
                            break;
                        case "WIP":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_WIP"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_WIP"].ProviderName;
                            break;
                        case "GIS":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ProviderName;
                            break;
                        case "EEL":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_EEL"].ProviderName;
                            break;
                        case "EASN":
                            connectionString = ConfigurationManager.ConnectionStrings["EASNGEG"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["EASNGEG"].ProviderName;
                            break;
                        case "HL":
                            connectionString = ConfigurationManager.ConnectionStrings["HL_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["HL_EEL"].ProviderName;
                            break;
                        default:
                            break;
                    }
                    break;

                case "EGM":
                    switch (strServerType)
                    {
                        case "MES":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEGM"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEGM"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEGMTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEGMTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEGMSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEGMSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEGM"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEGM"].ProviderName;
                                    break;
                            }
                            break;
                        case "INV":
                            connectionString = ConfigurationManager.ConnectionStrings["INV4.44"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["INV4.44"].ProviderName;
                            break;
                        case "inv_support":
                            connectionString = ConfigurationManager.ConnectionStrings["inv_support_EGM"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["inv_support_EGM"].ProviderName;
                            break;
                        case "EASN":
                            connectionString = ConfigurationManager.ConnectionStrings["EASNEGM"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["EASNEGM"].ProviderName;
                            break;
                        case "MES_UPDATE":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEGM"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEGM"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEGMTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEGMTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEGMSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEGMSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEGM"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEGM"].ProviderName;
                                    break;
                            }
                            break;
                        case "WeeklyReport":
                            connectionString = ConfigurationManager.ConnectionStrings["WeeklyReport"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["WeeklyReport"].ProviderName;
                            break;
                        case "GIS":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ProviderName;
                            break;
                        case "EEL":
                            connectionString = ConfigurationManager.ConnectionStrings["MESEGM_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESEGM_EEL"].ProviderName;
                            break;
                        case "HL":
                            connectionString = ConfigurationManager.ConnectionStrings["HL_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["HL_EEL"].ProviderName;
                            break;
                        default:
                            break;
                    }
                    break;
                case "PTX":
                    switch (strServerType)
                    {
                        case "MES":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESPTX"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESPTX"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESPTXTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESPTXTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESPTXSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESPTXSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESPTX"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESPTX"].ProviderName;
                                    break;
                            }
                            break;
                        case "INV":
                            connectionString = ConfigurationManager.ConnectionStrings["INV5.3"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["INV5.3"].ProviderName;
                            break;
                        case "MES_UPDATE":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESPTX"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESPTX"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESPTXTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESPTXTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESPTXSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESPTXSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESPTX"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESPTX"].ProviderName;
                                    break;
                            }
                            break;
                        case "WeeklyReport":
                            connectionString = ConfigurationManager.ConnectionStrings["WeeklyReport"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["WeeklyReport"].ProviderName;
                            break;
                        case "inv_support":
                            connectionString = ConfigurationManager.ConnectionStrings["inv_support_PTX"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["inv_support_PTX"].ProviderName;
                            break;
                        case "EASN":
                            connectionString = ConfigurationManager.ConnectionStrings["EASNPTX"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["EASNPTX"].ProviderName;
                            break;
                        case "HL":
                            connectionString = ConfigurationManager.ConnectionStrings["HL_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["HL_EEL"].ProviderName;
                            break;
                        default:
                            break;
                    }
                    break;

                case "EGV":
                    switch (strServerType)
                    {
                        case "MES":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEGV"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEGV"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEGVTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEGVTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEGVSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEGVSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEGV"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEGV"].ProviderName;
                                    break;
                            }
                            break;
                        case "INV":
                            //Added by Zikuan MES-097 14-Oct-2013
                            connectionString = ConfigurationManager.ConnectionStrings["INV152.8"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["INV152.8"].ProviderName;
                            //connectionString = ConfigurationManager.ConnectionStrings["INV7.222"].ConnectionString;
                            //providerName = ConfigurationManager.ConnectionStrings["INV7.222"].ProviderName;
                            break;
                        case "MES_UPDATE":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEGV"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEGV"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEGVTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEGVTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEGVSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEGVSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEGV"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEGV"].ProviderName;
                                    break;
                            }
                            break;
                        case "WeeklyReport":
                            connectionString = ConfigurationManager.ConnectionStrings["WeeklyReport"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["WeeklyReport"].ProviderName;
                            break;
                        case "inv_support":
                            connectionString = ConfigurationManager.ConnectionStrings["inv_support_EGV"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["inv_support_EGV"].ProviderName;
                            break;
                        case "EASN":
                            connectionString = ConfigurationManager.ConnectionStrings["EASNEGV"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["EASNEGV"].ProviderName;
                            break;
                        case "HL":
                            connectionString = ConfigurationManager.ConnectionStrings["HL_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["HL_EEL"].ProviderName;
                            break;
                        default:
                            break;
                    }
                    break;

                case "EAV":
                    switch (strServerType)
                    {
                        case "MES":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEAV"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEAV"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEAVTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEAVTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEAVSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEAVSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEAV"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEAV"].ProviderName;
                                    break;
                            }
                            break;
                        case "INV":
                            //Added by Zikuan MES-097 14-Oct-2013
                            connectionString = ConfigurationManager.ConnectionStrings["INV152.8"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["INV152.8"].ProviderName;
                            //connectionString = ConfigurationManager.ConnectionStrings["INV7.222"].ConnectionString;
                            //providerName = ConfigurationManager.ConnectionStrings["INV7.222"].ProviderName;
                            break;
                        case "MES_UPDATE":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEAV"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEAV"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEAVTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEAVTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEAVSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEAVSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEAV"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEAV"].ProviderName;
                                    break;
                            }
                            break;
                        case "WeeklyReport":
                            connectionString = ConfigurationManager.ConnectionStrings["WeeklyReport"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["WeeklyReport"].ProviderName;
                            break;
                        case "inv_support":
                            connectionString = ConfigurationManager.ConnectionStrings["inv_support_EGV"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["inv_support_EGV"].ProviderName;
                            break;
                        case "EASN":
                            connectionString = ConfigurationManager.ConnectionStrings["EASNEAV"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["EASNEAV"].ProviderName;
                            break;
                        case "HL":
                            connectionString = ConfigurationManager.ConnectionStrings["HL_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["HL_EEL"].ProviderName;
                            break;
                        default:
                            break;
                    }
                    break;
                //Added By ZouShiChang ON 2013.08.09 Start
                case "EHV":
                    switch (strServerType)
                    {
                        case "MES":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEHV"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEHV"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEHVTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEHVTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEHVSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEHVSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEHV"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEHV"].ProviderName;
                                    break;
                            }
                            break;
                        case "INV":
                            connectionString = ConfigurationManager.ConnectionStrings["INV155.18"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["INV155.18"].ProviderName;
                            break;
                        case "MES_UPDATE":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEHV"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEHV"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEHVTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEHVTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEHVSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEHVSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESEHV"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESEHV"].ProviderName;
                                    break;
                            }
                            break;
                        case "WeeklyReport":
                            connectionString = ConfigurationManager.ConnectionStrings["WeeklyReport"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["WeeklyReport"].ProviderName;
                            break;
                        case "inv_support":
                            connectionString = ConfigurationManager.ConnectionStrings["inv_support_EHV"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["inv_support_EHV"].ProviderName;
                            break;
                        case "EASN":
                            connectionString = ConfigurationManager.ConnectionStrings["EASNEHV"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["EASNEHV"].ProviderName;
                            break;
                        case "HL":
                            connectionString = ConfigurationManager.ConnectionStrings["HL_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["HL_EEL"].ProviderName;
                            break;
                        default:
                            break;
                    }
                    break;

                //Added By ZouShiChang ON 2013.08.09 End
                case "GLG":
                    switch (strServerType)
                    {
                        case "MES":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGLG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGLG"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGLGTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGLGTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGLGSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGLGSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGLG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGLG"].ProviderName;
                                    break;
                            }
                            break;
                        case "MES_UPDATE":
                            switch (svType)
                            {
                                case "PROD":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGLG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGLG"].ProviderName;
                                    break;
                                case "TEST":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGLGTEST"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGLGTEST"].ProviderName;
                                    break;
                                case "STG":
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGLGSTG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGLGSTG"].ProviderName;
                                    break;
                                default:
                                    connectionString = ConfigurationManager.ConnectionStrings["MESGLG"].ConnectionString;
                                    providerName = ConfigurationManager.ConnectionStrings["MESGLG"].ProviderName;
                                    break;
                            }
                            break;
                        case "INV":
                            connectionString = ConfigurationManager.ConnectionStrings["INV99.222"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["INV99.222"].ProviderName;
                            break;
                        case "EEL":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGLG_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGLG_EEL"].ProviderName;
                            break;
                        case "GIS":
                            connectionString = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESGEG_GIS"].ProviderName;
                            break;
                        case "inv_support":
                            connectionString = ConfigurationManager.ConnectionStrings["inv_support_GLG"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["inv_support_GLG"].ProviderName;
                            break;
                        case "EASN":
                            connectionString = ConfigurationManager.ConnectionStrings["EASNGLG"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["EASNGLG"].ProviderName;
                            break;
                        case "HL":
                            connectionString = ConfigurationManager.ConnectionStrings["HL_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["HL_EEL"].ProviderName;
                            break;
                    }
                    break;

                default:
                    switch (strServerType)
                    {
                        case "MES":
                            connectionString = ConfigurationManager.ConnectionStrings["MESDEV"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESDEV"].ProviderName;
                            break;
                        case "INV":
                            connectionString = ConfigurationManager.ConnectionStrings["INV7.222"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["INV7.222"].ProviderName;
                            break;
                        case "MES_UPDATE":
                            connectionString = ConfigurationManager.ConnectionStrings["MESDEV_UPDATE"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["MESDEV_UPDATE"].ProviderName;
                            break;
                        case "WeeklyReport":
                            connectionString = ConfigurationManager.ConnectionStrings["WeeklyReport"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["WeeklyReport"].ProviderName;
                            break;
                        case "HL":
                            connectionString = ConfigurationManager.ConnectionStrings["HL_EEL"].ConnectionString;
                            providerName = ConfigurationManager.ConnectionStrings["HL_EEL"].ProviderName;
                            break;
                        default:
                            break;
                    }
                    break;
            }
            if (connectionString != string.Empty && providerName != string.Empty)
            {
                DbProviderFactory dpf = DbProviderFactories.GetFactory(providerName); 
                conn = dpf.CreateConnection();
                conn.ConnectionString = connectionString;
            }
            return conn;

        }

        public static bool InsertGISRptTempData(DataTable jotb, string RunNo, DbConnection conn)
        {
            DbCommand myCommand = GetCommand(null, conn);
            myCommand.CommandText = "set session tx_isolation='READ-UNCOMMITTED';delete from MU_SHIP_JO_INFO where Run_NO='" + RunNo + "';delete from  MU_SHIP_JO_INFO where datediff(now(),create_date) >1; ";
            myCommand.ExecuteNonQuery();
            int j = 0;
            string sql = "";
            for (int i = 0; i < jotb.Rows.Count; i++)
            {
                j += 1;
                sql += "INSERT INTO MU_SHIP_JO_INFO (CT_NO,Run_NO) values('" + jotb.Rows[i][0].ToString() + "','" + RunNo + "');";
                if (j == 100 || i >= jotb.Rows.Count - 1)
                {
                    myCommand.CommandText = string.Format(sql);
                    myCommand.ExecuteNonQuery();
                    sql = "";
                    j = 0;
                }
            }
            return true;
        }

        public static bool InsertRptPPOTempData(DataTable jotb, DbConnection conn)
        {
            DbCommand myCommand = GetCommand(null, conn);
            myCommand.CommandText = "delete from rpt_tmp ";
            myCommand.ExecuteNonQuery();
            for (int i = 0; i < jotb.Rows.Count; i++)
            {
                myCommand.CommandText = string.Format("insert into rpt_tmp(F1) values('{0}') ", jotb.Rows[i][1].ToString());
                myCommand.ExecuteNonQuery();
            }
            return true;
        }

        public static bool InsertMESRptShipData(DataTable jotb, string RunNo, string factoryCd, DbConnection conn)
        {
            //table MU_SHIP_JO_INFO(FTY_CD,SC_NO,JO_NO,SHIP_DATE,SHIP_QTY,LASTJO,CREATE_DATE,RUNNO) 
            //SC_NO,CT_NO AS JO_NO,SHIP_DATE,SHIP_QTY,LASTJO
            DbCommand myCommand = GetCommand(null, conn);
            myCommand.CommandText = "delete from MU_SHIP_JO_INFO where RUNNO='" + RunNo + "';delete from  MU_SHIP_JO_INFO where dateadd(day,-1,getdate())>create_date; ";
            myCommand.ExecuteNonQuery();
            int j = 0;
            string sql = "";
            double qty = 0.00;
            for (int i = 0; i < jotb.Rows.Count; i++)
            {
                j += 1;
                qty = jotb.Rows[i][3].ToString() == "" ? 0.00 : double.Parse(jotb.Rows[i][3].ToString());
                sql += "INSERT INTO MU_SHIP_JO_INFO values('" + factoryCd + "','" + jotb.Rows[i][0].ToString() + "',";
                sql += " '" + jotb.Rows[i][1].ToString() + "','" + jotb.Rows[i][2].ToString() + "'," + qty + ",";
                sql += " '" + jotb.Rows[i][4].ToString() + "',getdate(),'" + RunNo + "');";
                if (j == 200 || i >= jotb.Rows.Count - 1)
                {
                    myCommand.CommandText = string.Format(sql);
                    myCommand.ExecuteNonQuery();
                    sql = "";
                    j = 0;
                }
            }
            return true;
        }

        //Added By Zikuan - MES-097 3-Dec-2013
        public static bool InsertTempShipInfoData(DataTable jotb, string RunNo, DbConnection conn)
        {
            DbCommand myCommand = GetCommand(null, conn);
            myCommand.CommandText = "delete from TmpShipInfo where Run_NO='" + RunNo + "'; delete TmpShipInfo where Create_Date < GETDATE();";
            myCommand.ExecuteNonQuery();
            int j = 0;
            string sql = "";
            for (int i = 0; i < jotb.Rows.Count; i++)
            {
                j += 1;
                sql += "INSERT INTO TmpShipInfo (RUN_NO, JO_NO, BUYER_PO_NO, TRANS_CD, TRANS_DATE, REMARKS, CREATE_DATE) values('" + RunNo + "','" + jotb.Rows[i][0].ToString() + "','" + jotb.Rows[i][1].ToString() + "','" + jotb.Rows[i][2].ToString() + "','" + jotb.Rows[i][3].ToString() + "','" + jotb.Rows[i][4].ToString() + "', GETDATE());";
                if (j == 100 || i >= jotb.Rows.Count - 1)
                {
                    myCommand.CommandText = string.Format(sql);
                    myCommand.ExecuteNonQuery();
                    sql = "";
                    j = 0;
                }
            }
            return true;
        }

        public static bool InsertMESRptWidthNpatternData(DataTable jotb, string RunNo, string factoryCd, DbConnection conn)
        {
            //Table MU_PPO_WidthNpattern (FTY_CD varchar(3),SC_NO varchar(10),WIDTH VARCHAR(100),PATTERN_TYPE VARCHAR(15),
            //PPO_YPD DECIMAL(18,2),YPD_JOB_NO VARCHAR(30),CREATE_DATE DATETIME,RunNo VARCHAR(30),PJO_NO VARCHAR(20)) 
            //jotb SC_NO,PJO_NO,WIDTH,PATTERN_TYPE,PPO_YPD,YPD_JOB_NO

            DbCommand myCommand = GetCommand(null, conn);
            myCommand.CommandText = "delete from MU_PPO_WidthNpattern where RUNNO='" + RunNo + "';delete from  MU_PPO_WidthNpattern where dateadd(day,-1,getdate())>create_date; ";
            myCommand.ExecuteNonQuery();
            int j = 0;
            string sql = "";
            double qty = 0.00;
            for (int i = 0; i < jotb.Rows.Count; i++)
            {
                j += 1;
                qty = jotb.Rows[i][4].ToString() == "" ? 0.00 : double.Parse(jotb.Rows[i][4].ToString());
                sql += "INSERT INTO MU_PPO_WidthNpattern values('" + factoryCd + "','" + jotb.Rows[i][0].ToString() + "',";
                sql += " '" + jotb.Rows[i][2].ToString().Replace("'", "''") + "','" + jotb.Rows[i][3].ToString() + "'," + qty + ",";
                sql += " '" + jotb.Rows[i][5].ToString() + "',getdate(),'" + RunNo + "','" + jotb.Rows[i][1].ToString() + "');";
                if (j == 200 || i >= jotb.Rows.Count - 1)
                {
                    myCommand.CommandText = string.Format(sql);
                    myCommand.ExecuteNonQuery();
                    sql = "";
                    j = 0;
                }
            }
            return true;
        }
        public static bool InsertMESRptFGISData(DataTable jotb, string RunNo, string factoryCd, DbConnection conn)
        {
            //Table MU_GMT_LEFTOVER_QTY FTY_CD varchar(3),
            //JO_NO VARCHAR(13),
            //LEFTOVER_A DECIMAL(18,2),
            //LEFTOVER_B DECIMAL(18,2),
            //LEFTOVER_C DECIMAL(18,2),
            //SEW_WSTG_QTY DECIMAL(18,2),
            //WASH_WSTG_QTY DECIMAL(18,2),
            // RTW_QTY_I DECIMAL(18,2),
            //RTW_QTY DECIMAL(18,2),
            //SRN_QTY_I DECIMAL(18,2),
            //SRN_QTY DECIMAL(18,2),
            //CREATE_DATE DATETIME,RunNo VARCHAR(30)
            //jotb FTY_CD	JO_NO	LEFTOVER_A	LEFTOVER_B	LEFTOVER_C	SEW_WSTG_QTY	WASH_WSTG_QTY	RTW_QTY_I	RTW_QTY	SRN_QTY_I	SRN_QTY	CREATE_DATE

            DbCommand myCommand = GetCommand(null, conn);
            myCommand.CommandText = "delete from MU_GMT_LEFTOVER_QTY where RUNNO='" + RunNo + "';delete from  MU_GMT_LEFTOVER_QTY where dateadd(day,-1,getdate())>create_date; ";
            myCommand.ExecuteNonQuery();
            int j = 0;
            string sql = "";
            double qtyA = 0.00;
            double qtyB = 0.00;
            double qtyS = 0.00;
            double qtyW = 0.00;
            double qtyRI = 0.00;
            double qtyR = 0.00;
            double qtySI = 0.00;
            double qtySS = 0.00;
            for (int i = 0; i < jotb.Rows.Count; i++)
            {
                j += 1;
                qtyA = jotb.Rows[i]["gmt_qty_a"].ToString() == "" ? 0.00 : double.Parse(jotb.Rows[i]["gmt_qty_a"].ToString());
                qtyB = jotb.Rows[i]["gmt_qty_b"].ToString() == "" ? 0.00 : double.Parse(jotb.Rows[i]["gmt_qty_b"].ToString());
                qtyS = jotb.Rows[i]["sew_qty_b"].ToString() == "" ? 0.00 : double.Parse(jotb.Rows[i]["sew_qty_b"].ToString());
                qtyW = jotb.Rows[i]["wash_qty_b"].ToString() == "" ? 0.00 : double.Parse(jotb.Rows[i]["wash_qty_b"].ToString());
                qtyRI = jotb.Rows[i]["rtw_qty_i"].ToString() == "" ? 0.00 : double.Parse(jotb.Rows[i]["rtw_qty_i"].ToString());
                qtyR = jotb.Rows[i]["rtw_qty"].ToString() == "" ? 0.00 : double.Parse(jotb.Rows[i]["rtw_qty"].ToString());
                qtySI = jotb.Rows[i]["srn_qty_i"].ToString() == "" ? 0.00 : double.Parse(jotb.Rows[i]["srn_qty_i"].ToString());
                qtySS = jotb.Rows[i]["srn_qty"].ToString() == "" ? 0.00 : double.Parse(jotb.Rows[i]["srn_qty"].ToString());
                //sql += "INSERT INTO MU_GMT_LEFTOVER_QTY values('" + factoryCd + "','" + jotb.Rows[i][0].ToString() + "',";
                //sql += qtyRI + "," + qtyR + ",0," + qtySI + "," + qtySS;
                //sql += qtyA + "," + qtyB + ",0," + qtyS + "," + qtyW;
                //sql += " ,getdate(),'" + RunNo + "');";
                sql += "INSERT INTO MU_GMT_LEFTOVER_QTY values('" + factoryCd + "','" + jotb.Rows[i][0].ToString() + "',";
                sql += qtyA + "," + qtyB + ",0,";
                sql += qtyS + "," + qtyW + "," + qtyRI + "," + qtyR + "," + qtySI + "," + qtySS;
                sql += " ,getdate(),'" + RunNo + "');";
                if (j == 200 || i >= jotb.Rows.Count - 1)
                {
                    myCommand.CommandText = string.Format(sql);
                    myCommand.ExecuteNonQuery();
                    sql = "";
                    j = 0;
                }
            }
            return true;
        }


        public static bool InsertMESRptStandardOrderQtyData(DataTable jotb, string RunNo, string factoryCd, DbConnection conn)
        {
            //Table MU_PPO_GO_JO_QTY (FTY_CD varchar(3),SC_NO varchar(10),JO_NO VARCHAR(13),GOInPPO_ORDER_QTY DECIMAL(18,2),
            //GO_ORDER_QTY DECIMAL(18,2),JO_ORDER_QTY DECIMAL(18,2),OVERSHIP VARCHAR(15),CREATE_DATE DATETIME,RunNo VARCHAR(30)) 
            //jotb SC_NO,NVL(SUM(order_qty),0) AS ORDER_QTY 

            DbCommand myCommand = GetCommand(null, conn);
            myCommand.CommandText = "delete from MU_PPO_GO_JO_QTY where RUNNO='" + RunNo + "';delete from  MU_PPO_GO_JO_QTY where dateadd(day,-1,getdate())>create_date; ";
            myCommand.ExecuteNonQuery();
            int j = 0;
            string sql = "";
            double qty = 0.00;
            double Go_qty = 0.00;
            for (int i = 0; i < jotb.Rows.Count; i++)
            {
                j += 1;
                qty = jotb.Rows[i][1].ToString() == "" ? 0.00 : double.Parse(jotb.Rows[i][1].ToString());
                Go_qty = jotb.Rows[i][2].ToString() == "" ? 0.00 : double.Parse(jotb.Rows[i][2].ToString());
                sql += "INSERT INTO MU_PPO_GO_JO_QTY(FTY_CD,SC_NO,GOInPPO_ORDER_QTY,GO_ORDER_QTY,CREATE_DATE,RunNo) ";
                sql += " values('" + factoryCd + "','" + jotb.Rows[i][0].ToString() + "',";
                sql += qty + "," + Go_qty + ",getdate(),'" + RunNo + "');";

                if (j == 200 || i >= jotb.Rows.Count - 1)
                {
                    myCommand.CommandText = string.Format(sql);
                    myCommand.ExecuteNonQuery();
                    sql = "";
                    j = 0;
                }
            }
            return true;
        }

        public static bool InsertMESRptLeftOverData(DataTable jotb, string RunNo, string factoryCd, DbConnection conn)
        {
            //table MU_PPO_LEFTOVER_QTY (PJO_NO VARCHAR(20),Leftover_QTY DECIMAL(18,2),CREATE_DATE DATETIME,RunNo VARCHAR(30)) 
            //jotb  PJO_NO,SUM(RATIO*Leftover_QTY) AS Leftover_QTY

            DbCommand myCommand = GetCommand(null, conn);
            myCommand.CommandText = "delete from MU_PPO_LEFTOVER_QTY where RUNNO='" + RunNo + "';delete from  MU_PPO_LEFTOVER_QTY where dateadd(day,-1,getdate())>create_date; ";
            myCommand.ExecuteNonQuery();
            int j = 0;
            string sql = "";
            double qty = 0.00;
            double qty1 = 0.00;
            for (int i = 0; i < jotb.Rows.Count; i++)
            {
                j += 1;
                qty = jotb.Rows[i][1].ToString() == "" ? 0.00 : double.Parse(jotb.Rows[i][2].ToString());
                qty1 = jotb.Rows[i][1].ToString() == "" ? 0.00 : double.Parse(jotb.Rows[i][3].ToString());
                sql += "INSERT INTO MU_PPO_LEFTOVER_QTY ";
                sql += " values('" + jotb.Rows[i][0].ToString() + "',";
                sql += qty + ",getdate(),'" + RunNo + "','" + jotb.Rows[i][1].ToString() + "'," + qty1 + ");";

                if (j == 200 || i >= jotb.Rows.Count - 1)
                {
                    myCommand.CommandText = string.Format(sql);
                    myCommand.ExecuteNonQuery();
                    sql = "";
                    j = 0;
                }
            }
            return true;
        }

        /// <summary>
        /// 创建DbCommand对象
        /// </summary>
        /// <returns>DbCommand对象</returns>
        public static DbCommand GetCommand(string serverType)
        {
            DbCommand comm = null;
            DbConnection conn = GetConnectionString(serverType);
            if (conn != null)
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                comm = conn.CreateCommand();
                comm.CommandType = CommandType.Text;
            }
            return comm;
        }

        /// <summary>
        /// 创建OleDbCommand对象
        /// </summary>
        /// <returns>OleDbCommand对象</returns>
        //public static OleDbCommand GetCommand(OleDbConnection conn)
        //{
        //    OleDbCommand comm = null;
        //    if (conn != null)
        //    {
        //        if (conn.State != ConnectionState.Open)
        //        {
        //            conn.Open();
        //        }
        //        comm = conn.CreateCommand();
        //        comm.CommandType = CommandType.Text;
        //    }
        //    return comm;
        //}

        /// <summary>
        /// 执行查询，返回datatable
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static DataTable ExecuteSelectCommand(DbCommand command)
        {
            return ExecuteSelectCommand(command, true);
        }

        /// <summary>
        /// 执行查询，返回datatable
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static DataTable ExecuteSelectCommand(DbCommand command, bool blnCloseConn)
        {
            DataTable table;
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                    command.CommandTimeout = 1000;
                }
                DbDataReader reader = command.ExecuteReader();
                //MySql.Data.MySqlClient.MySqlDataReader reader = command.ExecuteReader();
                table = new DataTable();
                table.Load(reader);
                reader.Close();
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                if (blnCloseConn == true)
                {
                    command.Connection.Close();
                }
            }
            return table;

        }

        /// <summary>
        /// 执行update insert del操作
        /// </summary>
        /// <param name="command"></param>
        /// <returns>返回影响行数</returns>
        private static int ExecuteNonQuery(DbCommand command)
        {
            int affectRows = -1;
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                affectRows = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                command.Connection.Close();
            }
            return affectRows;
        }

        /// <summary>
        /// 返回第一列第一行

        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private static string ExecuteScalar(DbCommand command)
        {
            string value = "";
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                value = command.ExecuteScalar().ToString();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                command.Connection.Close();
            }
            return value;
        }


        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="site">查询的站点</param>
        /// <param name="serverType">查询服务器类型</param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public static DataTable GetTable(string sql, string serverType)
        {
            DataTable dt;
            //string strSite = CheckStr(site);

            DbCommand myCommand = GetCommand(serverType);
            myCommand.CommandText = sql;
            myCommand.CommandTimeout = 300;
            dt = ExecuteSelectCommand(myCommand);

            return dt;
        }
        public static bool InsertRptTempData(DataTable jotb, DbConnection conn)
        {
            List<string> commandStringList = new List<string>();
            string commandText;
            DbCommand myCommand = GetCommand(null, conn);

            //myCommand.CommandText = "delete from rpt_tmp;";
            //myCommand.ExecuteNonQuery();

            StringBuilder strBuilder = new StringBuilder("delete from rpt_tmp;");

           
            for (int i = 0; i < jotb.Rows.Count; i++)
            {
                commandText = string.Format(" f1 :='{0}';", jotb.Rows[i][0].ToString());
                commandText += string.Format("insert into rpt_tmp(F1) values(f1); ");
                if (strBuilder.Length + commandText.Length > 30000)
                {
                    commandStringList.Add(strBuilder.ToString());
                    strBuilder.Remove(0, strBuilder.Length);
                }
                strBuilder.Append(commandText);
            }
            commandStringList.Add(strBuilder.ToString());
            foreach (var str in commandStringList)
            {
                myCommand.CommandText = " declare f1 VARCHAR2(30 BYTE); begin " + str + " end; ";
                myCommand.ExecuteNonQuery();

            }
            //DataTable tmpdt=DBUtility.GetTable("select * from rpt_tmp", conn);
            return true;
        }



        public static bool InsertRptTempData(string joList, DbConnection conn)
        {
            List<string> commandStringList = new List<string>();
            string commandText;
            DbCommand myCommand = GetCommand(null, conn);

            //myCommand.CommandText = "delete from rpt_tmp;";
            //myCommand.ExecuteNonQuery();

            StringBuilder strBuilder = new StringBuilder("delete from rpt_tmp;");

            string[] joArray = joList.Split(',');

            for (int i = 0; i < joArray.Length; i++)
            {
                if (joArray[i] != "")
                    commandText = string.Format("insert into rpt_tmp(F1) values({0}); ", joArray[i]);
                else
                    commandText = "";


                if (strBuilder.Length + commandText.Length > 30000)
                {
                    commandStringList.Add(strBuilder.ToString());
                    strBuilder.Remove(0, strBuilder.Length);
                }
                strBuilder.Append(commandText);
            }

            commandStringList.Add(strBuilder.ToString());

            foreach (var str in commandStringList)
            {
                myCommand.CommandText = "begin " + str + " end;";
                myCommand.ExecuteNonQuery();
            }
            return true;
        }

        public static bool InsertPPOTempData(DataTable jotb, DbConnection conn)
        {
            List<string> commandStringList = new List<string>();
            string commandText;
            DbCommand myCommand = GetCommand(null, conn);

            //myCommand.CommandText = "delete from rpt_tmp;";
            //myCommand.ExecuteNonQuery();

            StringBuilder strBuilder = new StringBuilder("delete from TMP_MUPPO;");


            for (int i = 0; i < jotb.Rows.Count; i++)
            {
                commandText = string.Format("insert into TMP_MUPPO(SC_NO,FABRIC_TYPE_CD,PPO_NO) values('{0}','{1}','{2}'); ", jotb.Rows[i][0].ToString(), jotb.Rows[i][1].ToString(), jotb.Rows[i][2].ToString());
                if (strBuilder.Length + commandText.Length > 30000)
                {
                    commandStringList.Add(strBuilder.ToString());
                    strBuilder.Remove(0, strBuilder.Length);
                }
                strBuilder.Append(commandText);
            }
            commandStringList.Add(strBuilder.ToString());
            foreach (var str in commandStringList)
            {
                myCommand.CommandText = "begin " + str + " end;";
                myCommand.ExecuteNonQuery();

            }
            //DataTable tmpdt=DBUtility.GetTable("select * from rpt_tmp", conn);
            return true;
        }

        public static DataTable GetTable(string sql, DbConnection conns)
        {
            DataTable dt;
            //string strSite = CheckStr(site);

            DbCommand myCommand = GetCommand(null, conns);
            myCommand.CommandText = sql;
            myCommand.CommandTimeout = 300;
            dt = ExecuteSelectCommand(myCommand, false);

            return dt;
        }

        public static object ExecuteScalar(string sql, string serverType)
        {
            object o;

            DbCommand myCommand = GetCommand(serverType);
            myCommand.CommandText = sql;
            o = myCommand.ExecuteScalar();

            return o;
        }
        public static DataTable GetTable(string sql, string serverType, DbCommand dbcom)
        {
            DataTable dt;
            //string strSite = CheckStr(site);
            if (dbcom.Connection.State != ConnectionState.Open)
            {
                dbcom.Connection.Open();
            }
            DbCommand myCommand = dbcom;
            myCommand.CommandText = sql;
            myCommand.CommandTimeout = 300;
            dt = ExecuteSelectCommand(myCommand);

            return dt;
        }
        public static int ExecuteNonQuery(string sql, string serverType)
        {
            DbCommand myCommand = GetCommand(serverType);
            myCommand.CommandText = sql;
            myCommand.CommandTimeout = 300;
            return ExecuteNonQuery(myCommand);
        }
        public static int ExecuteNonQuery(string serverType, CommandType cmdType, string cmdText, Dictionary<string,string> commandParameters)
        {

            //DbCommand myCommand = GetCommand(serverType);
            DbCommand myCommand = GetCommand(serverType);
            //通过PrePareCommand方法将参数逐个加入到SqlCommand的参数集合中 
            PrepareCommand(myCommand, cmdType, cmdText, commandParameters);
          
            int val = myCommand.ExecuteNonQuery();

            //清空SqlCommand中的参数列表 
            myCommand.Parameters.Clear();
            return val;
        }

        private static void PrepareCommand(DbCommand cmd, CommandType cmdType, string cmdText, Dictionary<string, string> cmdParms)
        {

            //判断数据库连接状态 
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                //foreach (DbParameter parm in cmdParms)
                //    cmd.Parameters.Add(parm);
                foreach (string parm in cmdParms.Keys)
                {
                   DbParameter dbpara=cmd.CreateParameter();
                   dbpara.ParameterName = parm;
                   dbpara.Value = cmdParms[parm];
                   cmd.Parameters.Add(dbpara);
                }
            }
        }
        public static bool Insert(string sql, string serverType)
        {
            try
            {
                DbCommand myCommand = GetCommand(serverType);
                if (myCommand.Connection.State != ConnectionState.Open)
                {
                    myCommand.Connection.Open();
                }
                myCommand.CommandText = sql;
                myCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }


        }

        public static DbCommand GetCommand_conn(DbConnection conn)
        {
            //DbCommand comm = null;
            //DbConnection conn = GetConnectionString(serverType);

            //if (conn != null)
            //{
            //    if (conn.State != ConnectionState.Open)
            //    {
            //        conn.Open();
            //    }
            //    comm = conn.CreateCommand();
            //    comm.CommandType = CommandType.Text;
            //}
            return GetCommand(null, conn);
        }

        public static DbCommand GetCommand_conn(string serverType)
        {
            //DbCommand comm = null;
            //DbConnection conn = GetConnectionString(serverType);

            //if (conn != null)
            //{
            //    if (conn.State != ConnectionState.Open)
            //    {
            //        conn.Open();
            //    }
            //    comm = conn.CreateCommand();
            //    comm.CommandType = CommandType.Text;
            //}
            //return comm;
            return GetCommand(serverType, null);
        }


        private static DbCommand GetCommand(string serverType, DbConnection conn)
        {
            DbCommand comm = null;
            if (conn == null)
            {
                conn = GetConnectionString(serverType);
            }

            if (conn != null)
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                comm = conn.CreateCommand();
                comm.CommandType = CommandType.Text;
            }
            return comm;
        }

        public static DbCommand InsertTempData(DataTable jotb, string serverType)
        {
            DbCommand myCommand = GetCommand(serverType);
            if (myCommand.Connection.State != ConnectionState.Open)
            {
                myCommand.Connection.Open();
            }
            myCommand.CommandText = "delete from inventory.inv_tmp1 ";
            myCommand.ExecuteNonQuery();
            for (int i = 0; i < jotb.Rows.Count; i++)
            {
                myCommand.CommandText = string.Format("insert into inventory.inv_tmp1(F1) values({0}) ", jotb.Rows[i][0].ToString());
                myCommand.ExecuteNonQuery();
            }
            return myCommand;
        }

        //public static DataTable RunProcedure(string storedProcName, DbParameter[] parameters)
        //{
        //    DataTable dt;
        //    DbConnection conn = GetConnectionString("MES");
        //    conn.Open();

        //    try
        //    {
        //        DbCommand comm = conn.CreateCommand();
        //        comm.Connection = conn;
        //        comm.CommandText = storedProcName;
        //        comm.CommandType = CommandType.StoredProcedure;
        //        if (parameters != null)
        //        {

        //            foreach (DbParameter para in parameters)
        //            {

        //                comm.Parameters.Add(para);

        //            }

        //        }

        //        dt = ExecuteSelectCommand(comm);
        //        return dt;

        //    }

        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);

        //    }

        //    finally
        //    {

        //        conn.Close();

        //    }

        //}

        //Added by YeeHou
        public static DataTable RunProcedure(string storedProcName, DbParameter[] parameters, int timeOutDuration)
        {
            DataTable dt;
            DbConnection conn = GetConnectionString("MES");
            conn.Open();
            try
            {
                DbCommand comm = conn.CreateCommand();
                comm.Connection = conn;
                comm.CommandText = storedProcName;
                comm.CommandTimeout = timeOutDuration;
                comm.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    foreach (DbParameter para in parameters)
                    {
                        comm.Parameters.Add(para);
                    }
                }
                dt = ExecuteSelectCommand(comm);
                return dt;
            }
            catch (TimeoutException tex)
            {
                throw new Exception(tex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            finally
            {
                conn.Close();
            }
        }

        public static DbConnection GetConnection(string serverType)
        {
            DbConnection conn = null;
            conn = GetConnectionString(serverType);
            conn.Open();

            return conn;
        }

        public static void CloseConnection(ref DbConnection conn)
        {
            conn.Close();
        }



        public static void InsertMESMUData(DataTable StandardGOList, string strRunNO,string ReportType, DbConnection MESConn)
        {

            StringBuilder sb=new StringBuilder();
            DbCommand myCommand = GetCommand(null, MESConn);
            sb.Append(" delete MU_Report_All_Data where CREATE_DATE<dateadd(day,-2,getdate()) and LOCK_FLAG='N'; ");
            sb.AppendFormat(" delete MU_Report_All_Data where RunNo='{0}';  ",strRunNO);
            myCommand.CommandText = sb.ToString();
            myCommand.ExecuteNonQuery();
            sb.Remove(0, sb.Length);
            string insertstr = "insert into MU_Report_All_Data(FromDate,ToDate,FTY_CD,JO_NO,SC_NO,BPO_Date,ORDER_QTY,OVER_SHIP,buyer,Wash_Type,STYLE_DESC,PATTERN_TYPE,LOCK_FLAG,CREATE_DATE,RunNo,GARMENT_TYPE_CD,ReportType,SHORT_SHIP)";
            int j = 0;
            string sql = "";
            for (int i = 0; i < StandardGOList.Rows.Count; i++)
            {
                j += 1;

                sb.Append(insertstr);
                sb.AppendFormat(" values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',{13},'{14}','{15}','{16}',{17})",
                   StandardGOList.Rows[i]["FromDate"], StandardGOList.Rows[i]["ToDate"], StandardGOList.Rows[i]["FTY_CD"], StandardGOList.Rows[i]["JOB_ORDER_NO"],
                   StandardGOList.Rows[i]["SC_NO"], StandardGOList.Rows[i]["buyer_po_del_date"], StandardGOList.Rows[i]["total_qty"].ToString().ConvertEmptyToZero(), StandardGOList.Rows[i]["PERCENT_OVER_ALLOWED"].ToString().ConvertEmptyToZero(),
                   StandardGOList.Rows[i]["SHORT_NAME"].SqlFormat(), StandardGOList.Rows[i]["wash_type_cd"], StandardGOList.Rows[i]["style_desc"].SqlFormat(), StandardGOList.Rows[i]["PATTERN_TYPE_CD"],
                   "N", "getdate()", strRunNO, StandardGOList.Rows[i]["GARMENT_TYPE_CD"], ReportType, StandardGOList.Rows[i]["PERCENT_SHORT_ALLOWED"].ToDouble());
                if (j == 200 || i >= StandardGOList.Rows.Count - 1)
                {
                    myCommand.CommandText = sb.ToString();
                    myCommand.ExecuteNonQuery();
                    sb.Remove(0, sb.Length);
                    j = 0;
                }
            }
           
        }

        public static bool LockMuReport(string RunNo, DbConnection conn)
        {
            
            DbCommand myCommand = GetCommand(null, conn);
            myCommand.CommandText = " update MU_Report_All_Data set LOCK_FLAG='Y' where RunNo='" + RunNo + "'  ";
            myCommand.ExecuteNonQuery();
            return true;
        }
    }
}