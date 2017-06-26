using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_outsourceContractReport2 : pPage
{
    public string companyName, subcontractorName, subString2, subString6, Receiver, currency, valueAddTax;
    public string companyAddress, subcontractAddress, companyTel, subTel, companyFax, subFax, subcontractNo, ttOutAmount;
    public string strLine1,strLine2;
    protected void Page_Load(object sender, EventArgs e)
    {
        string FactoryCD = "";
        if (!IsPostBack)
        {
            hfValue.Value = Request.QueryString["site"];
        }
        if(Request.QueryString["site"] != null){
            FactoryCD = Request.QueryString["site"].ToString().ToUpper();
        }
        if (FactoryCD.Equals("GEG") || FactoryCD.Equals("CEG"))
        {
            string strFour = "&nbsp;&nbsp;&nbsp; &nbsp; &nbsp;乙方保证，乙方在履行本合同过程中所提供的技术及产品等，不侵犯任何第三方的合法权益。如发生第三方指控乙方提供的技术、产品";
            strFour += "等侵权的，乙方应当承担一切经济和法律责任，包括但不限于赔偿甲方因参与此类侵权指控的诉讼费用、律师费用、所受损失及弥补纠正侵权";
            strFour += "产品而支付的所有费用。";
            this.divFour.InnerHtml = strFour;
        }
        else
        {
            string strFour = "&nbsp;&nbsp;&nbsp; &nbsp; &nbsp;卖方保证，卖方在履行本合同过程中所提供的技术、产品及设备，不侵犯任何第三方的合法权益。如发生第三方指控卖方提供的技术、产品、";
            strFour += "设备侵权的，卖方应当承担一切经济和法律责任，包括但不限于赔偿买方因参与此类侵权指控的诉讼费用、律师费用、所受损失及弥补纠正侵权";
            strFour += "产品而支付的所有费用。";
            this.divFour.InnerHtml = strFour;
        }

        if (FactoryCD.Equals("CEK") || FactoryCD.Equals("CEG"))
        {
            string strFive = "<b>第五条</b>&nbsp;&nbsp;本合同内涉及的包装材料是由甲方提供。<br /><br/>";
            this.divfive.InnerHtml = strFive;
            this.strLine1 = "第六条";
            this.strLine2 = "第七条";
        }
        else
        {
            this.strLine1 = "第五条";
            this.strLine2 = "第六条";
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divBody.InnerHtml = "";
        DataTable currencyList = MESComment.MesOutSourcePriceSql.GetOutsourceCurrency(txtContractNo.Text.Trim());

        foreach (DataRow row in currencyList.Rows)
        {
            currency = row["CURRENCY"].ToString();
            subcontractorName = row["CHN_NAME"].ToString();
            subcontractNo = row["MAIN_CONTRACT"].ToString();
            subFax = row["FAX"].ToString();
            subTel = row["TEL_NO"].ToString();
            subcontractAddress = row["ADDRESS"].ToString();
            Receiver = row["USER_NAME"].ToString();
            valueAddTax = row["VALUE_ADD_TAX"].ToString();
            companyName = row["CompanyName"].ToString();
            companyAddress = row["CompanyAddress"].ToString();
            companyTel = row["CompanyTel"].ToString();
            companyFax = row["CompanyFax"].ToString();
            subString2 = row["subString2"].ToString();
            subString6 = row["subString6"].ToString();
        }

        DataTable contractList = MESComment.MesOutSourcePriceSql.GetOutsourceContract(txtContractNo.Text.Trim());
        double ttAmount = 0;
        double amount = 0;
        double price = 0;
        double quantity = 0;
        foreach (DataRow row in contractList.Rows)
        {
            price = double.Parse(row["SUB_CONTRACT_PRICE"].ToString());
            quantity = double.Parse(row["PLAN_ISSUE_QTY"].ToString());
            amount = price * quantity;
            ttAmount = ttAmount + amount;

            divBody.InnerHtml += "<tr>";
            divBody.InnerHtml += "<td valign='top' class='tr3style'><p align='center'>" + row["STYLE_CHN_DESC"] + "&nbsp;</p></td>";
            divBody.InnerHtml += "<td valign='top' class='tr3style'><p align='center'>" + row["SC_NO"] + "&nbsp;</p></td>";
            divBody.InnerHtml += "<td valign='top' class='tr3style'><p align='center'>" + row["JOB_ORDER_NO"] + "&nbsp;</p></td>";
            divBody.InnerHtml += "<td valign='top' class='tr3style'><p align='center'>" + row["GOOD_NAME"] + "&nbsp;</p></td>";
            divBody.InnerHtml += "<td valign='top' class='tr3style' >按《外发加工合同》（编号" + subcontractNo + "）及双方特别约定执行</td>";
            divBody.InnerHtml += "<td valign='top' class='tr3style'><p align='center'>" + row["PLAN_ISSUE_QTY"] + "&nbsp;</p></td>";
            divBody.InnerHtml += "<td valign='top' class='tr3style'><p align='center'>" + row["EXPECT_RECEIVE_DATE"] + "&nbsp;</p></td>";
            divBody.InnerHtml += "<td valign='top' class='tr3style'><p align='center'>" + row["PROCESS_CD"] + "&nbsp;</p></td>";
            divBody.InnerHtml += "<td valign='top' class='tr3style'><p align='center'>" + price.ToString("#,##0.00") + "&nbsp;</p></td>";
            divBody.InnerHtml += "<td valign='top' class='tr5style'><p align='center'>" + amount + "&nbsp;</p></td>";
            divBody.InnerHtml += "</tr>";
        }
        if (ttAmount > 0)
        {
            ttOutAmount = ttAmount.ToString("#,##0.00") + "(" + common.MoneyConverter.ConvetC(decimal.Parse(ttAmount.ToString("#.##"))) + ")";
        }

    }
}
