using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;


    public partial class Transaction_Doc_Entry_Detail :pPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnquery_Click(object sender, EventArgs e)
        {
            if (txtStartDate.Text == "" || txtToDate.Text == "")
            {
                return;
            }
            Sqlconnection();
            lblStatus.Text = "completed";

        }
        private void Sqlconnection()
        {
            string sql;
            sql = "SELECT a.Factory_Cd as 'Factory Cd', a.Doc_No as 'Doc No', B.Process_cd as 'Process cd', a.Next_Process_Cd as 'Next Process Cd',substring(CONVERT(nvarchar(10) ,A.trx_date, 120 ),0,11) as 'Trx Date',ISNULL(usr.NAME,a.Create_User_id) as 'User Name', a.Create_Date as 'Create Date', b.Last_Modi_Date as 'Last Modi Date',ISNULL(A.SUBMIT_DATE,B.SUBMIT_DATE) AS 'Submit Date' FROM PRD_JO_OUTPUT_HD A LEFT JOIN PRD_GARMENT_TRANSFER_HD B ON A.DOC_NO=B.DOC_NO LEFT JOIN dbo.MES_USER usr ON usr.USER_ID= a.Create_User_id";
            sql += " where trx_date between '" + txtStartDate.Text + "' and '" + txtToDate.Text + "'";
            DataTable dt = MESComment.DBUtility.GetTable(sql,"MES");
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
    }

