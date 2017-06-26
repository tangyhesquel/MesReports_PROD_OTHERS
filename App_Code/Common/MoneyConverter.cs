using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

/// <summary>
/// Summary description for MoneyConverter
/// </summary>
namespace common
{
    public class MoneyConverter
    {
        public MoneyConverter()
        {
        }

        public static String ConvetC(decimal bds)
        {
            String strZero = "零";
            String strYuan = "圆";
            String strJiao = "角";
            String strFen = "分";
            String strZheng = "整";
            String[] strItem = { "", "万", "亿", "兆", "??", "!!", "**" };
            String[] strC = { "", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖", "拾" };
            String[] strB = { "", "拾", "佰", "仟" };

            String strChinese = "";
            String strSource = bds.ToString();
            String[] strDesk;
            String strPrec = "";
            int i = 0, j = 0, iNumber = 0, iItem = 0, iLoc = 0;
            iNumber = strSource.IndexOf(".");
            if (iNumber == -1)
            {
                strSource = strSource + ".0000";
                iNumber = strSource.IndexOf(".");
            }
            strPrec = strSource.Substring(iNumber + 1);

            iItem = (int)((iNumber + 3) / 4);
            strDesk = new String[iItem];

            for (i = 0; i < iItem; i++)
            {
                iLoc = (iNumber - 4 * i) > 4 ? 4 : (iNumber - 4 * i);
                strDesk[i] = "";
                int[] iNowItem = { -1, -1, -1, -1 };
                for (j = (i * 4) + iLoc - 1; j > (i * 4) - 1; j--)
                {
                    iNowItem[j - i * 4] = (int)(char.Parse(strSource.Substring((iNumber - j - 1), 1))) - 48;
                }
                strDesk[i] = strItem[i];
                if (iNowItem[0] > 0)
                {
                    strDesk[i] = strC[iNowItem[0]] + strB[0] + strDesk[i];
                }

                if (iNowItem[1] > 0)
                {
                    strDesk[i] = strC[iNowItem[1]] + strB[1] + strDesk[i];
                }
                else if (iNowItem[1] == 0 && iNowItem[0] > 0)
                {
                    strDesk[i] = strZero + strDesk[i];
                }

                if (iNowItem[2] > 0)
                {
                    strDesk[i] = strC[iNowItem[2]] + strB[2] + strDesk[i];
                }
                else if (iNowItem[2] == 0 && iNowItem[1] > 0)
                {
                    strDesk[i] = strZero + strDesk[i];
                }

                if (iNowItem[3] > 0)
                {
                    strDesk[i] = strC[iNowItem[3]] + strB[3] + strDesk[i];
                }
                else if (iNowItem[3] == 0 && iNowItem[2] > 0)
                {
                    strDesk[i] = strZero + strDesk[i];
                }
                if (iNowItem[0] == 0 && iNowItem[1] == 0 && iNowItem[2] == 0
                        && iNowItem[3] == 0)
                {
                    strDesk[i] = strZero;
                    if (i == 0)
                    {
                        strDesk[i] = "";
                    }
                    else if (strDesk[i - 1].Equals("")
                            && strDesk[i - 1].IndexOf(strZero) == 0)
                    {
                        strDesk[i] = "";
                    }
                }
                strChinese = strDesk[i] + strChinese;
            }
            strChinese = strChinese + strYuan;

            int iJiao = 0, iFen = 0;
            if (strPrec.Length < 4)
            {
                strPrec += "0000";
            }
            strPrec = (Math.Round(double.Parse("1" + strPrec.Substring(0, 4)) / 100) * 100) + "";
            if (strPrec.Length > 0)
            {
                strPrec = strPrec.Substring(1);
            }
            try
            {
                iJiao = (int)(char.Parse(strPrec.Substring(0, 1))) - 48;
            }
            //catch (Exception e1)
            catch (Exception)
            {
            }
            try
            {
                iFen = (int)(char.Parse(strPrec.Substring(1, 1))) - 48;

            }
            //catch (Exception e2)
            catch (Exception)
            {
            }
            if (iJiao > 0)
            {
                strChinese = strChinese + strC[iJiao] + strJiao;
            }
            if (iFen > 0)
            {
                strChinese = strChinese + strC[iFen] + strFen;
            }
            else
                strChinese = strChinese + strZheng;
            return (strChinese);
        }
    }
}