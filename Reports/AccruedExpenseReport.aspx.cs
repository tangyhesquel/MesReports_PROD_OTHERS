using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reports_AccruedExpenseReport : pPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["site"].ToString().ToUpper() != "DEV")
            {
                hfValue.Value = Request.QueryString["site"];
            }
            else
            {
                hfValue.Value = "GEG";

            }

        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        divBody.InnerHtml = "";
        //DataTable dt = MESComment.AccruedExpenseReportSql.GetAccruedExpense(CurrentSite, txtFromDate.Text, txtToDate.Text, txtJoNo.Text.Trim(), txtContractNo.Text.Trim());
        DataTable dt = GetAccruedExpense(hfValue.Value.ToString(), txtFromDate.Text, txtToDate.Text, txtJoNo.Text.Trim(), txtContractNo.Text.Trim());

        foreach (DataRow row in dt.Rows)
        {
            divBody.InnerHtml += "<tr>";
            divBody.InnerHtml += "<td>" + row["PERIOD"] + "</td> ";//期间
            divBody.InnerHtml += "<td>" + row["CONTRACT_NO"] + "</td> ";//合同编号
            divBody.InnerHtml += "<td>" + row["PROCESS_CD"] + "</td> ";//外发工序
            divBody.InnerHtml += "<td>" + row["JOB_ORDER_NO"] + "</td> ";//CTNO
            divBody.InnerHtml += "<td>" + row["OUTQTY"] + "</td> ";//发出数量
            divBody.InnerHtml += "<td>" + row["RECEIVEQTY"] + "</td> ";//收回正品数量不含零价仓数(PCS)
            divBody.InnerHtml += "<td>" + row["LINGJIAQTY1"] + "</td> ";//过零价仓(正品)
            divBody.InnerHtml += "<td>" + row["INFERIORQTY"] + "</td> ";//次品
            divBody.InnerHtml += "<td>" + row["SAMPLEQTY"] + "</td> ";//抽办
            divBody.InnerHtml += "<td>" + row["LOSTQTY"] + "</td> ";//本厂不见衫
            divBody.InnerHtml += "<td>" + row["BUSSINESSREDUCEQTY"] + "</td> ";//商检
            divBody.InnerHtml += "<td>" + row["FACTORYSTOCKQTY"] + "</td> ";//厂存
            divBody.InnerHtml += "<td>" + row["OUT_LOST"] + "</td> ";//外厂不见衫
            divBody.InnerHtml += "<td>" + row["OTHERPULLOUTQTY"] + "</td> ";// 其他(-)
            divBody.InnerHtml += "<td>" + row["ADD_QTY"] + "</td> ";//其他(+)
            divBody.InnerHtml += "<td>" + row["PULLOUT_QTY"] + "</td> ";//下数总计
            divBody.InnerHtml += "<td>" + row["LINGJIAQTY2"] + "</td> ";//过零价仓(次品)PCS
            divBody.InnerHtml += "<td>" + row["S_M"] + "(" + row["CURRENCY"] + ")</td> ";// 主工序
            divBody.InnerHtml += "<td>" + row["W_M"] + "(" + row["CURRENCY"] + ")</td> ";//洗水
            divBody.InnerHtml += "<td>" + row["P_M"] + "(" + row["CURRENCY"] + ")</td> ";// 印花
            divBody.InnerHtml += "<td>" + row["E_M"] + "(" + row["CURRENCY"] + ")</td> ";//绣花
            divBody.InnerHtml += "<td>" + row["ADJ_AMOUNT"] + "</td> ";//调整款项(RMB) 
            divBody.InnerHtml += "<td>" + row["Fab_Price"] + "</td> ";//布价;
            double d = Double.Parse(row["OUT_LOST"].ToString()) * Double.Parse(row["Fab_Price"].ToString()) / (1 + Double.Parse(row["VALUE_ADD_TAX"].ToString()));
            divBody.InnerHtml += "<td>" + (d.ToString("#,###.##").Equals("") ? "0.00" : d.ToString("#,###.##")) + "</td> ";//应扣布价(不含税)
            //d = Double.Parse(row["S_M"].ToString()) + Double.Parse(row["W_M"].ToString()) + Double.Parse(row["P_M"].ToString()) + Double.Parse(row["E_M"].ToString()) - d - Double.Parse(row["ADJ_AMOUNT"].ToString());
            d = Double.Parse(row["S_M"].ToString()) + Double.Parse(row["W_M"].ToString()) + Double.Parse(row["P_M"].ToString()) + Double.Parse(row["E_M"].ToString()) - d + Double.Parse(row["ADJ_AMOUNT"].ToString()) / (1 + Double.Parse(row["VALUE_ADD_TAX"].ToString()));
            divBody.InnerHtml += "<td>" + (d.ToString("#,###.##").Equals("") ? "0.00" : d.ToString("#,###.##")) + "</td> ";//实付加工费(不含税)
            divBody.InnerHtml += "<td>" + row["SUB_CONTRACT_PRICE"] + "</td> ";// 外发单价(Per PCS含税)/主工序
            divBody.InnerHtml += "<td>" + row["WASH_PRICE"] + "</td> ";// 外发单价(Per PCS含税)/洗水
            divBody.InnerHtml += "<td>" + row["PRINT_PRICE"] + "</td> ";// 外发单价(Per PCS含税)/印花
            divBody.InnerHtml += "<td>" + row["EMB_PRICE"] + "</td> ";// 外发单价(Per PCS含税)/绣花
            divBody.InnerHtml += "<td>" + row["INTERNER_PRICE"] + "</td> ";//本厂工价(Per PCS)/主工序
            divBody.InnerHtml += "<td>" + row["INTERNAL_WASH_PRICE"] + "</td> ";//本厂工价(Per PCS)/洗水
            divBody.InnerHtml += "<td>" + row["INTERNAL_PRINT_PRICE"] + "</td> ";//本厂工价(Per PCS)/印花
            divBody.InnerHtml += "<td>" + row["INTERNAL_EMB_PRICE"] + "</td> ";//本厂工价(Per PCS)/绣花
            divBody.InnerHtml += "<td>" + row["VALUE_ADD_TAX"] + "</td> ";//税率
            divBody.InnerHtml += "<td>" + row["SUBCONTRACTOR_NAME"] + "</td> ";//加工商
            divBody.InnerHtml += "<td>" + row["PROCESS_REMARK"] + "</td> ";//工序描述
            divBody.InnerHtml += "<td>" + row["REASON"] + "</td> ";//外发原因
            divBody.InnerHtml += "<td>&nbsp;</td> ";//Remark
            divBody.InnerHtml += "</tr>";
        }
    }

    DataTable GetAccruedExpense(string factory, string startDate, string endDate, string joNo, string contractNo)
    {
        string SQL = "";
        SQL += @"DECLARE @startDate NVARCHAR(20)
                                DECLARE @endDate NVARCHAR(20)
                                DECLARE @factory NVARCHAR(10)
                                DECLARE @joNo NVARCHAR(20)
                                DECLARE @contractNo NVARCHAR(50)";
        SQL += "    SET @startDate = '" + startDate + "'";
        SQL += "    SET @endDate = '" + endDate + "'";
        SQL += "    SET @factory = '" + factory + "'";
        SQL += "    SET @joNo = '" + joNo + "'";
        SQL += "    SET @contractNo = '" + contractNo + "'";

        SQL += @"    IF OBJECT_ID('TEMPDB..#TEMP_T') IS NOT NULL      DROP TABLE #TEMP_T;
                                IF OBJECT_ID('TEMPDB..#TEMP_P') IS NOT NULL      DROP TABLE #TEMP_P;";

        SQL += @"    SELECT JHD.TRX_DATE, ODT.CONTRACT_NO,ODT.JOB_ORDER_NO,JOX.PROCESS_CD,JOX.PROCESS_GARMENT_TYPE,JOX.PROCESS_TYPE
                                ,OHD.SEND_DPARTMENT,OHD.GARMENT_TYPE
                                ,ODT.SEND_ID,JOX.SEND_ID AS SEND_ID2
                                ,OHD.RECEIVE_POINT,JHD.NEXT_PROCESS_CD,JHD.NEXT_PROCESS_GARMENT_TYPE,JHD.NEXT_PROCESS_TYPE
                                ,JOX.OUTPUT_QTY,0 AS INFERIORQTY, 0 AS SAMPLEQTY,0 AS LOSTQTY,0 AS BUSSINESSREDUCEQTY,0 AS FACTORYSTOCKQTY,0 AS OUT_LOST
                                , 0 AS OTHERPULLOUTQTY,0 AS ADD_QTY,0 AS PULLOUT_QTY 
                                INTO #TEMP_T
                                FROM         PRD_JO_OUTPUT_TRX JOX JOIN PRD_JO_OUTPUT_HD JHD ON JHD.FACTORY_CD=@factory AND JOX.DOC_NO=JHD.DOC_NO ";


        if (!startDate.Equals(""))
        {
            SQL += "    and JHD.TRX_DATE >=dbo.DATE_FORMAT(@startDate,'mm/dd/yyyy' )";
        }
        if (!endDate.Equals(""))
        {
            SQL += "    and JHD.TRX_DATE <=dbo.DATE_FORMAT(@endDate,'mm/dd/yyyy' ) ";
        }
        SQL += @"    LEFT JOIN PRD_OUTSOURCE_CONTRACT_DT ODT ON   ODT.SEND_ID=JOX.SEND_ID AND ODT.JOB_ORDER_NO=JOX.JOB_ORDER_NO         ";
        if (joNo != "")
        {
            SQL += " and ODT.JOB_ORDER_NO = @joNo";
        }
        if (contractNo != "")
        {
            SQL += " and ODT.CONTRACT_NO = @contractNo";
        }
        SQL += @"    JOIN PRD_OUTSOURCE_CONTRACT OHD ON   ODT.CONTRACT_NO=OHD.CONTRACT_NO AND OHD.STATUS!='CAN' 
                                WHERE 1=1  ";
        if (joNo != "")
        {
            SQL += " and JOX.JOB_ORDER_NO = @joNo and ODT.JOB_ORDER_NO = @joNo";
        }



        if (!startDate.Equals(""))
        {
            SQL += "    and JHD.TRX_DATE >=dbo.DATE_FORMAT(@startDate,'mm/dd/yyyy' )";
        }
        if (!endDate.Equals(""))
        {
            SQL += "    and JHD.TRX_DATE <=dbo.DATE_FORMAT(@endDate,'mm/dd/yyyy' ) ";
        }
        if (!joNo.Equals(""))
        {
            SQL += " and ODT.JOB_ORDER_NO = @joNo";
        }
        if (contractNo != "")
        {
            SQL += " and ODT.CONTRACT_NO = @contractNo";
        }

        SQL += " OPTION  ( RECOMPILE ) ;  ";

        SQL += @"    SELECT A.TRX_DATE, OHD.CONTRACT_NO,B.JOB_ORDER_NO,0 AS OUTQTY,0 AS   RECEIVEQTY, 0 AS LINGJIAQTY1 ,0 AS LINGJIAQTY2
                                ,E.SHORT_NAME ,E.qty_affection,D.PULLOUT_QTY
                                INTO #TEMP_P
                                FROM PRD_JO_DISCREPANCY_PULLOUT_HD a 
                                JOIN   PRD_JO_DISCREPANCY_PULLOUT_TRX b ON A.DOC_NO=B.DOC_NO AND a.factory_cd=@factory 
                                AND   A.FACTORY_CD=B.FACTORY_CD AND A.TRX_DATE=B.TRX_DATE         ";
        if (joNo != "")
        {
            SQL += " and B.JOB_ORDER_NO = @joNo";
        }

        SQL += @"    
                                JOIN PRD_OUTSOURCE_CONTRACT_DT ODT ON B.SEND_ID = ODT.SEND_ID   AND B.JOB_ORDER_NO=ODT.JOB_ORDER_NO   ";
        if (joNo != "")
        {
            SQL += " and ODT.JOB_ORDER_NO = @joNo";
        }
        SQL += @"      
                                JOIN PRD_OUTSOURCE_CONTRACT OHD ON ODT.CONTRACT_NO =   OHD.CONTRACT_NO AND A.PROCESS_CD = OHD.RECEIVE_POINT 
                                AND   OHD.STATUS!='CAN'         ";
        SQL += " AND A.GARMENT_TYPE=OHD.GARMENT_TYPE ";
        if (contractNo != "")
        {
            SQL += " and ODT.CONTRACT_NO = @contractNo";
        }
        SQL += @"
                                LEFT JOIN PRD_JO_PULLOUT_REASON D ON D.TRX_ID=B.TRX_ID AND   D.FACTORY_CD=A.FACTORY_CD 
                                LEFT JOIN PRD_REASON_CODE E ON   E.REASON_CD=D.REASON_CD AND E.FACTORY_CD=D.FACTORY_CD 
                                WHERE 1=1  ";
        if (!startDate.Equals(""))
        {
            SQL += "    and A.TRX_DATE >=dbo.DATE_FORMAT(@startDate,'mm/dd/yyyy' )";
        }
        if (!endDate.Equals(""))
        {
            SQL += "    and A.TRX_DATE <=dbo.DATE_FORMAT(@endDate,'mm/dd/yyyy' ) ";
        }
        if (joNo != "")
        {
            SQL += " and B.JOB_ORDER_NO = @joNo and ODT.JOB_ORDER_NO = @joNo";
        }
        if (contractNo != "")
        {
            SQL += " and ODT.CONTRACT_NO = @contractNo";
        }
        SQL += " OPTION  ( RECOMPILE ) ;  ";

        SQL += @"    SELECT * INTO #TEMP_TP FROM (
                                SELECT         PERIOD=(CONVERT(CHAR(4),YEAR(TRX_DATE))+CONVERT(CHAR(2),Right(100+Month(TRX_DATE),2)))         
                                ,CONTRACT_NO , JOB_ORDER_NO
                                ,OUTQTY=(case when         PROCESS_CD=SEND_DPARTMENT and PROCESS_GARMENT_TYPE=GARMENT_TYPE and SEND_ID=SEND_ID2         then OUTPUT_QTY else 0 end)
                                ,RECEIVEQTY=(case when         PROCESS_CD=RECEIVE_POINT AND PROCESS_GARMENT_TYPE=GARMENT_TYPE and SEND_ID=SEND_ID2 and         NEXT_PROCESS_CD NOT IN('TOSTOCKL','TOSTOCKL') AND NEXT_PROCESS_CD         NOT IN('TOSTOCKL2','TOSTOCKL2') then OUTPUT_QTY else 0 end)
                                ,LINGJIAQTY1=(case when PROCESS_CD=RECEIVE_POINT AND PROCESS_GARMENT_TYPE=GARMENT_TYPE and SEND_ID=SEND_ID2 and NEXT_PROCESS_CD IN('TOSTOCKL','TOSTOCKL') then         OUTPUT_QTY else 0 end)
                                ,LINGJIAQTY2=(case when         PROCESS_CD=RECEIVE_POINT AND PROCESS_GARMENT_TYPE=GARMENT_TYPE and SEND_ID=SEND_ID2 and         NEXT_PROCESS_CD IN('TOSTOCKL2','TOSTOCKL2') then OUTPUT_QTY else 0         end)
                                ,INFERIORQTY, SAMPLEQTY,LOSTQTY,BUSSINESSREDUCEQTY, FACTORYSTOCKQTY, OUT_LOST
                                ,OTHERPULLOUTQTY, ADD_QTY, PULLOUT_QTY 
                                FROM        #TEMP_T    
                                UNION ALL 
                                SELECT   CONVERT(CHAR(4),YEAR(TRX_DATE))+CONVERT(CHAR(2),Right(100+Month(TRX_DATE),2))   AS PERIOD
                                ,CONTRACT_NO,JOB_ORDER_NO, OUTQTY,   RECEIVEQTY,  LINGJIAQTY1 , LINGJIAQTY2
                                ,INFERIORQTY=(CASE WHEN SHORT_NAME IN('CPNDF','FABDF','PRTDF','SEWDF','SHADE') THEN   PULLOUT_QTY ELSE 0 END)
                                ,SAMPLEQTY=(CASE WHEN   SHORT_NAME in ('SSMPL','SMPL','SMPBOAT') THEN PULLOUT_QTY ELSE 0 END)
                                ,LOSTQTY=(CASE WHEN SHORT_NAME='GMMTL' THEN PULLOUT_QTY ELSE   0 END)
                                ,BUSSINESSREDUCEQTY=(CASE WHEN SHORT_NAME='CHECK' THEN   PULLOUT_QTY ELSE 0 END)
                                , FACTORYSTOCKQTY=(CASE WHEN   SHORT_NAME='FTYST' THEN PULLOUT_QTY ELSE 0 END)
                                ,OUT_LOST=(CASE WHEN SHORT_NAME='OUMTL' THEN PULLOUT_QTY ELSE   0 END)
                                ,OTHERPULLOUTQTY=(CASE WHEN SHORT_NAME NOT   IN('CPNDF','FABDF','PRTDF','SEWDF','SHADE','SSMPL','SMPL','SMPBOAT','GMMTL','FTYST','OUMTL','CHECK' ) and qty_affection='D' THEN   PULLOUT_QTY ELSE 0 END)
                                ,ADD_QTY=(CASE WHEN   qty_affection='A' and SHORT_NAME NOT   IN('CPNDF','FABDF','PRTDF','SEWDF','SHADE','SSMPL','SMPL','SMPBOAT','GMMTL','FTYST','OUMTL','CHECK' )    THEN PULLOUT_QTY ELSE 0 END)
                                ,PULLOUT_QTY   
                                FROM  #TEMP_P
                                ) T OPTION  ( RECOMPILE );";

        SQL += @"    select B.PERIOD,a.CONTRACT_NO ,a.JOB_ORDER_NO,A.PROCESS_CD,         
                                SUM(OUTQTY) as OUTQTY,
                                SUM(RECEIVEQTY) as         RECEIVEQTY,
                                SUM(LINGJIAQTY1) as LINGJIAQTY1, 
                                SUM(LINGJIAQTY2) as         LINGJIAQTY2,
                                SUM(isnull(INFERIORQTY,0))         INFERIORQTY,
                                SUM(isnull(SAMPLEQTY,0)) SAMPLEQTY,         
                                SUM(isnull(LOSTQTY,0)) LOSTQTY,
                                SUM(isnull(BUSSINESSREDUCEQTY,0))         BUSSINESSREDUCEQTY, 
                                SUM(isnull(FACTORYSTOCKQTY,0))         FACTORYSTOCKQTY,
                                SUM(isnull(OTHERPULLOUTQTY,0)) OTHERPULLOUTQTY,         
                                SUM(isnull(OUT_LOST,0)) OUT_LOST,
                                SUM(isnull(ADD_QTY,0)) ADD_QTY,         
                                SUM(isnull(PULLOUT_QTY,0)) PULLOUT_QTY,         
                                S_M=convert(decimal(18,2),isnull(SUB_CONTRACT_PRICE/(1+VALUE_ADD_TAX)*(SUM(RECEIVEQTY)+SUM(isnull(SAMPLEQTY,0))+SUM(isnull(LOSTQTY,0))+SUM(isnull(BUSSINESSREDUCEQTY,0))+SUM(isnull(FACTORYSTOCKQTY,0))+SUM(isnull(ADD_QTY,0))),0)),         
                                W_M=convert(decimal(18,2),isnull(WASH_PRICE/(1+VALUE_ADD_TAX)*(SUM(RECEIVEQTY)+SUM(isnull(SAMPLEQTY,0))+SUM(isnull(LOSTQTY,0))+SUM(isnull(BUSSINESSREDUCEQTY,0))+SUM(isnull(FACTORYSTOCKQTY,0))+SUM(isnull(ADD_QTY,0))),0)),         
                                P_M=convert(decimal(18,2),isnull(PRINT_PRICE/(1+VALUE_ADD_TAX)*(SUM(RECEIVEQTY)+SUM(isnull(SAMPLEQTY,0))+SUM(isnull(LOSTQTY,0))+SUM(isnull(BUSSINESSREDUCEQTY,0))+SUM(isnull(FACTORYSTOCKQTY,0))+SUM(isnull(ADD_QTY,0))),0)),         
                                E_M=convert(decimal(18,2),isnull(EMB_PRICE/(1+VALUE_ADD_TAX)*(SUM(RECEIVEQTY)+SUM(isnull(SAMPLEQTY,0))+SUM(isnull(LOSTQTY,0))+SUM(isnull(BUSSINESSREDUCEQTY,0))+SUM(isnull(FACTORYSTOCKQTY,0))+SUM(isnull(ADD_QTY,0))),0)),         
                                isNull(ADJ_AMOUNT,0) as ADJ_AMOUNT ,isNULL(SUB_CONTRACT_PRICE,0) as SUB_CONTRACT_PRICE,isNull(INTERNER_PRICE,0) AS INTERNER_PRICE,         isNull(WASH_PRICE,0) as WASH_PRICE,isNull(INTERNAL_WASH_PRICE,0) as INTERNAL_WASH_PRICE,isNull(PRINT_PRICE,0) as PRINT_PRICE
                                ,isNULL(INTERNAL_PRINT_PRICE,0) as INTERNAL_PRINT_PRICE,isNull(EMB_PRICE,0) as EMB_PRICE,isNull(INTERNAL_EMB_PRICE,0) as INTERNAL_EMB_PRICE,isnull(Fab_Price,0) as Fab_Price,         
                                isnull(VALUE_ADD_TAX,0) as VALUE_ADD_TAX ,SUBCONTRACTOR_NAME,INVOICE_NO,REASON,PROCESS_REMARK,CURRENCY,GOOD_NAME         
                                FROM #TEMP_TP B 
                                LEFT JOIN (SELECT ODT.CONTRACT_NO , ODT.JOB_ORDER_NO,   convert(decimal(18,2),isnull(ODT.ADJ_AMOUNT,0)) ADJ_AMOUNT
                                ,convert(decimal(18,3),isnull(ODT.SUB_CONTRACT_PRICE,0))   SUB_CONTRACT_PRICE
                                ,convert(decimal(18,3),isnull(ODT.INTERNER_PRICE,0))   INTERNER_PRICE, convert(decimal(18,2),isnull(WASH_PRICE,'0.00'))   WASH_PRICE
                                ,convert(decimal(18,3),isnull(INTERNAL_WASH_PRICE,'0'))INTERNAL_WASH_PRICE   
                                ,convert(decimal(18,3),isnull(PRINT_PRICE,'0.00')) PRINT_PRICE
                                ,convert(decimal(18,3),isnull(INTERNAL_PRINT_PRICE,'0'))   INTERNAL_PRINT_PRICE 
                                ,convert(decimal(18,3),isnull(EMB_PRICE,'0.00')) EMB_PRICE,   convert(decimal(18,3),isnull(INTERNAL_EMB_PRICE,'0'))   INTERNAL_EMB_PRICE 
                                ,convert(decimal(18,3),isnull(Fab_Price,'0'))   Fab_Price,   convert(decimal(18,2),isnull(OHD.VALUE_ADD_TAX,0))   VALUE_ADD_TAX
                                ,SUB.SUBCONTRACTOR_NAME, ODT.PROCESS_CD,   OHD.INVOICE_NO, isnull(ODT.REASON,' ') REASON
                                ,isnull(ODT.PROCESS_REMARK,' ') PROCESS_REMARK, SUB.CURRENCY,   isnull(ODT.GOOD_NAME,' ' ) GOOD_NAME 
                                FROM   PRD_OUTSOURCE_CONTRACT_DT ODT JOIN PRD_OUTSOURCE_CONTRACT OHD ON OHD.factory_cd=@factory 
                                AND   ODT.CONTRACT_NO=OHD.CONTRACT_NO AND OHD.STATUS!='CAN'         
                                JOIN PRD_SUBCONTRACTOR_MASTER SUB ON   OHD.SUBCONTRACTOR=SUB.SUBCONTRACTOR_CD 
                                JOIN   PRD_SENDER_RECIVER_MASTER SRM ON SRM.CODE=OHD.SENDER) A ON   a.CONTRACT_NO=b.CONTRACT_NO 
                                and a.JOB_ORDER_NO=b.JOB_ORDER_NO   
                                WHERE 1=1   AND A.CONTRACT_NO IS NOT NULL       ";
        if (joNo != "")
        {
            SQL += " and B.JOB_ORDER_NO = @joNo";
        }
        if (contractNo != "")
        {
            SQL += " and B.CONTRACT_NO = @contractNo";
        }
        SQL += @"  
                                GROUP BY B.PERIOD,a.CONTRACT_NO   ,a.JOB_ORDER_NO,A.PROCESS_CD,ADJ_AMOUNT,SUB_CONTRACT_PRICE,   INTERNER_PRICE
                                ,WASH_PRICE,INTERNAL_WASH_PRICE,PRINT_PRICE,INTERNAL_PRINT_PRICE,   EMB_PRICE,INTERNAL_EMB_PRICE,Fab_Price
                                ,VALUE_ADD_TAX,SUBCONTRACTOR_NAME,INVOICE_NO,   REASON,PROCESS_REMARK,CURRENCY,GOOD_NAME  OPTION  ( RECOMPILE ) ; ";

        return MESComment.DBUtility.GetTable(SQL, "MES");
    }
}