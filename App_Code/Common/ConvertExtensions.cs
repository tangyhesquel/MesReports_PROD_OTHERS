using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///ConvertExtensions 的摘要说明

/// </summary>
public static class ConvertExtensions
{
    public static int ToInt(this object value,string howtoint)
    {
        int n = 0;
        if (value == null)
            return n;
        if (value.ToString() == "")
            return n;

        double d = value.ToDouble();
        if(howtoint.Equals("f"))//向下取整
            d = Math.Floor(d);
        else if(howtoint.Equals("t"))//忽略小数
            d = Math.Truncate(d);
        else if(howtoint.Equals("c"))//向上取整
            d = Math.Ceiling(d);
        else
            d = Math.Round(d); //四舍五入   
        int.TryParse(d.ToString(), out n);
        return n;
        //int n = 0;
        //if (value == null)
        //    return n;
        //if (value.ToString() == "")
        //    return n;
        //decimal d = 0;
        //decimal.TryParse(value.ToString(), out d);
        //d=Math.Round(d);
        //int.TryParse(d.ToString(), out n);
        
        //return n;
    }
    public static double ToDouble(this object value)
    {
        double n = 0;
        if (value == null)
            return n;
        if (value.ToString() == "")
            return n;
        double.TryParse(value.ToString(), out n);

        return n;
    }
    public static string ConvertZeroToEmpty(this object value)
    {
        if (value == null)
            return "";
        if (value.ToInt("r") == 0)
            return "";
        return value.ToString();
    }
    public static string ConvertEmptyToZero(this string value)
    {
        if (value == null)
            return "0";
        if (value.Trim().Length == 0)
            return "0";
        return value;
    }
    public static string ToTD(this object value, string cssclass)
    {
        string formatvalue;
        if (value == null)
            formatvalue = "&nbsp;";
        else if (value.ToString() == "")
            formatvalue = "&nbsp;";
        else
            formatvalue = value.ToString();
        if (cssclass.Length == 0)
            return " <td>" + value.ToString() + "</td>";
        return " <td class='" + cssclass + "'>" + value.ToString() + "</td>";
    }
    public static string ToPersent(this object value)
    {
        if (value == null)
            return "0";
        else if (value.ToString() == "")
            return "0";
        return value.ToString() + "%";
    }

    public static DateTime ToDate(this object value)
    {
        DateTime d = DateTime.MinValue;
        if (value == null)
            return d;
        if (value.ToString() == "")
            return d;
        DateTime.TryParse(value.ToString(), out d);

        return d;
    }
    public static string DateToDateStr(this object value)
    {
        DateTime d = value.ToDate();
        return d.ToString("yyyy-MM-dd");
    }
    public static string DateToDateTimeStr(this object value)
    {
        DateTime d = value.ToDate();
        return d.ToString("yyyy-MM-dd hh:mm:sss");
    }
    public static string DateToDateStrEmpty(this object value)
    {
        DateTime d = value.ToDate();
        if (d == DateTime.MinValue)
            return "";
        return d.ToString("yyyy-MM-dd");
    }
    public static string DateToDateTimeStrEmpty(this object value)
    {
        DateTime d = value.ToDate();
        if (d == DateTime.MinValue)
            return "";
        return d.ToString("yyyy-MM-dd hh:mm:sss");
    }
    public static double divide(this double value, double value2)
    {
        if (value2 == 0)
            return 0;
        return value / value2;
    }

    public static string SqlFormat(this object value)
    {
        if (value == null)
            return "";
        return value.ToString().Replace("'", "''");
    }



  
   
}