using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Drawing;
using Zen.Barcode;

namespace MESComment
{
    public class CommFunction
    {
        public static void SetDropDownList(DropDownList ddl, string tbName, string FValue, string FName, string serverType)
        {
            string sql = "";
            if (FValue == FName)
            {
                sql = string.Format("select {0} from {1} order by {0}", FName, tbName);
            }
            else
            {
                sql = string.Format("select {0},{1} from {2} order by {1}", FValue, FName, tbName);
            }
            DataTable tb = DBUtility.GetTable(sql, serverType);
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem("-All-", "-1"));
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                ddl.Items.Add(new ListItem(tb.Rows[i][FName].ToString(), tb.Rows[i][FValue].ToString()));
            }

        }

        public static void SetDropDownList(DropDownList ddl, string tbName, string FValue, string FName, string serverType,string factoryCode)
        {
            string sql = "";
            if (FValue == FName)
            {
                sql = string.Format("select {0} from {1} where Factory_CD='{2}' order by {0}", FName, tbName, factoryCode);
            }
            else
            {
                sql = string.Format("select {0},{1} from {2} where Factory_CD='{3}'  order by {1}", FValue, FName, tbName, factoryCode);
            }
            DataTable tb = DBUtility.GetTable(sql, serverType);
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem("-All-", "-1"));
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                ddl.Items.Add(new ListItem(tb.Rows[i][FName].ToString(), tb.Rows[i][FValue].ToString()));
            }

        }



        /// <summary>
        /// Jquery Autocomplet调用函数
        /// </summary>
        /// <param name="returnName">返回的值</param>
        /// <param name="tbName">查询的表名称</param>
        /// <param name="conditionCol">查询条件列</param>
        /// <param name="serverType"></param>
        public static void DoAutoComplete(string returnName, string tbName, string conditionCol, string serverType)
        {
            //Jquery Autocomplete
            if (HttpContext.Current.Request.QueryString["q"] != null && HttpContext.Current.Request.QueryString["q"] != "")
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Charset = "utf-8";
                HttpContext.Current.Response.Buffer = true;
                //EnableViewState = false;
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                HttpContext.Current.Response.ContentType = "text/plain";
                HttpContext.Current.Response.Write(CommFunction.GetSource(returnName, tbName, conditionCol, HttpContext.Current.Request.QueryString["q"].ToString(), serverType));
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.Close();
                HttpContext.Current.Response.End();
            }
        }


        /// <summary>
        /// 获取jquery Source
        /// </summary>
        /// <param name="returnName"></param>
        /// <param name="tbName"></param>
        /// <param name="conditionCol"></param>
        /// <param name="condition"></param>
        /// <param name="serverType"></param>
        /// <returns></returns>
        public static string GetSource(string returnName, string tbName, string conditionCol, string condition, string serverType)
        {
            string sql = string.Format("select {0} from {1} where {2} like '{3}' + '%' order by {0}", returnName, tbName, conditionCol, condition);

            DataTable dt = DBUtility.GetTable(sql, serverType);

            //此处用于获取数据源
            List<string> source = new List<string>();
            for (int count = 0; count < dt.Rows.Count; count++)
            {
                source.Add(dt.Rows[count][returnName].ToString());
            }

            StringBuilder sbstr = new StringBuilder();
            for (int i = 0; i < source.Count; i++)
            {
                sbstr.Append(source[i].ToString());
                sbstr.Append("\n");
            }
            return sbstr.ToString();
        }



        public  static void LoadGroupDropDownList(string factoryCode,DropDownList ddlGroupName )
        {
            ddlGroupName.Items.Clear();            
            ddlGroupName.Items.Add(new ListItem("", ""));
            DataTable groupDataTable = MESComment.WIPReportSql.GetGroupTable(factoryCode);

            foreach (DataRow r in groupDataTable.Rows)
            {
                string[] lines = r[1].ToString().Split(',');
                StringBuilder itemTextStringBuilder = new StringBuilder(); ;
                if (lines.Length >= 2)
                {
                    itemTextStringBuilder.Append("(");
                    itemTextStringBuilder.Append(lines[0]);
                    itemTextStringBuilder.Append("-");
                    itemTextStringBuilder.Append(lines[lines.Length - 1]);
                    itemTextStringBuilder.Append(")");
                }
                else
                {
                    if (lines.Length > 0)
                    {
                        itemTextStringBuilder.Append("(");
                        itemTextStringBuilder.Append(lines[0]);
                        itemTextStringBuilder.Append(")");
                    }
                }

                string listItemText = itemTextStringBuilder.ToString();
                listItemText = listItemText == "" ? r[0].ToString() : r[0].ToString() + listItemText;
                ddlGroupName.Items.Add(
                    new ListItem(listItemText
                    , r[0].ToString()));
            }

        }

        public static System.Drawing.Image DrawBarCode(string code)
        {
            BarcodeDraw draw = BarcodeDrawFactory.Code128WithChecksum;
            return draw.Draw(code, 50);
        }

        public static System.Drawing.Image DrawBarCode_AGV(string code)
        {
            BarcodeDraw draw = BarcodeDrawFactory.Code128WithChecksum;
            return draw.Draw(code, 220, 3);
        }

        //Generate QRCode MES-102 by Jin Song
        public static System.Drawing.Image DrawQrCode(string code)
        {
            BarcodeDraw draw = BarcodeDrawFactory.CodeQr;
            return draw.Draw(code, 50);
        }
        //End generate QRCode MES-102
    }
}
