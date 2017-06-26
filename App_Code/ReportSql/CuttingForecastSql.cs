using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web.UI.WebControls;

namespace MESComment
{
    /// <summary>
    ///PreCutSummarySql 的摘要说明

    /// </summary>
    public class CuttingForecast
    {
        public static string Get_GO_BY_MO(string MO)
        {
            string SQL = "SELECT DISTINCT GO_NO FROM  CP_MO_HD WHERE MO_NO='" + MO + "'";
            DataTable DT = DBUtility.GetTable(SQL, "MES");
            if (DT.Rows.Count > 0)
            {
                return DT.Rows[0]["GO_NO"].ToString();
            }
            else
            {
                return "";
            }
        }

        public static DataTable[] Get_PreCut_Detail(string Go, string MO, Boolean ByLot)
        {
            DataTable[] tb = new DataTable[5];
            string SQL = "";
            SQL += " SELECT  CUSTOMER_NAME,A.SIZE_CD ,SUM(ACTUAL_QTY) AS ACTUAL_QTY   ";
            SQL += " FROM FN_GET_GO_PRE_CUT_DETAIL('" + Go + "','" + MO + "','N') A ";
            //knn
            //SQL += " FROM FN_GET_GO_PRE_CUT_DETAIL('" + Go + "','" + MO + "') A ";
            SQL += " inner join (select MAX(D.SHORT_NAME) as CUSTOMER_NAME,SIZE_CD,SEQ ";
            SQL += " from CP_MO_HD B WITH(NOLOCK)"; //Added WITH(NOLOCK) by Jin Song 7/2014
            SQL += " INNER JOIN  SC_HD C WITH(NOLOCK) ON B.GO_NO=C.SC_NO "; //Added WITH(NOLOCK) by Jin Song 7/2014
            SQL += " LEFT JOIN GEN_CUSTOMER D WITH(NOLOCK) ON C.CUSTOMER_CD=D.CUSTOMER_CD "; //Added WITH(NOLOCK) by Jin Song 7/2014
            SQL += " INNER JOIN CP_SIZE_SEQ S WITH(NOLOCK) "; //Added WITH(NOLOCK) by Jin Song 7/2014
            SQL += " on S.MO_NO=B.MO_NO ";
            SQL += " where  B.GO_NO='" + Go + "' ";
            SQL += "  AND B.MO_NO LIKE '%" + MO + "%'";
            SQL += " group by SIZE_CD,SEQ ) SEQ";
            SQL += " on SEQ.SIZE_CD=A.SIZE_CD ";
            SQL += "  WHERE 1=1";
            SQL += " GROUP BY CUSTOMER_NAME,A.SIZE_CD,SEQ.SEQ ";
            SQL += " ORDER BY  SEQ.SEQ";
            tb[0] = DBUtility.GetTable(SQL, "MES");

            //TB[1]
            //Modified by Jin Song MES133 (Add Color Desc)
            SQL = " SELECT X.COLOR_CD,X.COLOR_DESC,Y.ORDER_QTY,X.FAB_PATTERN, ";
            SQL = SQL + "    (str(ROUND(Y.ADJUST_QTY*100.0/Y.ORDER_QTY,2),10,2)+'%') AS ADJUST_PERCENTS,";
            SQL = SQL + "    Y.ADJUST_QTY,str(Y.ACTUAL_QTY,10,2),str(X.FABRIC_QTY,10,2),CASE WHEN X.FABRIC_QTY -Y.ACTUAL_QTY/12.0* (Y.MARKER_YPD+X.BINDING_YPD)<0 then 0 ELSE X.FABRIC_QTY -Y.ACTUAL_QTY/12.0* (Y.MARKER_YPD+X.BINDING_YPD) END AS BALANCE_QTY";
            SQL = SQL + " ,STR(X.RECEIVED_QTY,10,2)";
            SQL = SQL + " ,STR(X.SPARE_FABRIC_QTY,10,2)";
            SQL = SQL + " ,STR(X.BINDING_FABRIC_QTY,10,2)";
            SQL = SQL + " ,STR(X.RECEIVED_QTY-X.BINDING_FABRIC_QTY-X.SPARE_FABRIC_QTY,10,2)";
            SQL = SQL + " ,STR(X.ALLOCATED_QTY,10,2)";
            SQL = SQL + " ,STR(X.RECEIVED_QTY-X.BINDING_FABRIC_QTY-X.SPARE_FABRIC_QTY-X.ALLOCATED_QTY,10,2)";
            SQL = SQL + "      ,str(Y.NET_YPD,10,2),str(Y.MARKER_YPD,10,2),str(X.PPO_YPD,10,2), ";
            SQL = SQL + "    str(Y.MARKER_UTILIZATION,10,2)+'%' ";
            SQL = SQL + "    ,str(Y.MARKER_WASTAGE,10,2)+'%',";
            SQL = SQL + "    X.FABRIC_WIDTH ";
            SQL = SQL + "    FROM ";
            SQL = SQL + "  ( ";
            SQL = SQL + "    SELECT A.COLOR_CD,A.COLOR_DESC,SUM(FABRIC_QTY-ISNULL(SPARE_FABRIC_QTY,0)) AS FABRIC_QTY, ";
            SQL = SQL + "  SUM(ISNULL(FABRIC_QTY,0)) AS RECEIVED_QTY ";
            SQL = SQL + "  ,SUM(ISNULL(ALLOCATED_QTY,0)) AS ALLOCATED_QTY,SUM(ISNULL(SPARE_FABRIC_QTY,0)) AS SPARE_FABRIC_QTY";
            SQL = SQL + "  ,SUM(ISNULL(BINDING_FABRIC_QTY,0)) AS BINDING_FABRIC_QTY,";
            SQL = SQL + "    MAX(A.PPO_YPD) AS PPO_YPD,MAX(ISNULL(BINDING_YPD,0)) AS BINDING_YPD,MAX(A.WIDTH) AS FABRIC_WIDTH,MAX(A.FAB_PATTERN) AS FAB_PATTERN,MAX(PERCENT_OVER_ALLOWED) AS PERCENT_OVER_ALLOWED  ";
            SQL = SQL + "    FROM CP_FABRIC_ITEM A INNER JOIN SC_HD B ON A.GO_NO=B.SC_NO "; //By ZouShiChang ON 2013.08.29 MES024 SC_HD Change SC_HD
            SQL = SQL + "    WHERE  1=1";
            if (!Go.Equals(""))
            {
                SQL = SQL + "    AND A.GO_NO='" + Go + "'  ";
            }
            SQL = SQL + "    AND EXISTS(SELECT 1 FROM  CP_MARKER_SET C ";
            SQL = SQL + "    INNER JOIN CP_MARKER D ON C.MARKER_SET_ID = D.MARKER_SET_ID ";
            SQL = SQL + "    INNER JOIN CP_MARKER_FABRIC E ON D.MARKER_ID = E.MARKER_ID";
            SQL = SQL + "    INNER JOIN CP_MO_HD F ON C.MO_NO = F.MO_NO";
            SQL = SQL + "    WHERE A.FAB_ITEM_ID = E.FAB_ITEM_ID AND A.GO_NO=F.GO_NO AND A.PART_TYPE_CD=F.PART_TYPE";
            if (!MO.Equals(""))
            {
                SQL = SQL + "    AND C.MO_NO ='" + MO + "'";
            }
            SQL = SQL + "    )GROUP BY A.COLOR_CD, A.COLOR_DESC ";
            SQL = SQL + "  ) X ";
            SQL = SQL + "   INNER JOIN ";
            SQL = SQL + "  ( ";
            SQL = SQL + "    SELECT   Z.COLOR_CD ,G.ORDER_QTY,G.ADJUST_QTY,G.ACTUAL_QTY ,Z.MARKER_WASTAGE,   Z.MARKER_UTILIZATION,   Z.NET_YPD ,Z.MARKER_YPD    ";
            SQL = SQL + "  		FROM    ";
            SQL = SQL + "  	  (";
            SQL = SQL + "    	  SELECT REF_COLOR AS COLOR_CD,SUM(TOTAL_LENGTH)/SUM(GMT_QTY)*12 AS NET_YPD,SUM(ACT_QTY)/SUM(GMT_QTY)*100 AS MARKER_UTILIZATION,";
            SQL = SQL + "  	  SUM(TOTAL_LENGTH*(1+MARKER_WASTAGE*0.01))/SUM(GMT_QTY)*12 AS MARKER_YPD,";
            SQL = SQL + "  	  MAX(MARKER_WASTAGE) AS MARKER_WASTAGE";
            SQL = SQL + "  	  FROM  [FN_GET_GO_COLOR_YPD]('" + Go + "','" + MO + "')";
            SQL = SQL + "  	  GROUP BY REF_COLOR";
            SQL = SQL + "  	  ) Z ";
            SQL = SQL + "  	    INNER JOIN (";
            SQL = SQL + "  	       SELECT A.COLOR_CD,SUM(A.ORDER_QTY) AS ORDER_QTY,SUM(ISNULL(ACTUAL_QTY,0)-A.ORDER_QTY) AS ADJUST_QTY,";//change by lijer on 20161111
            SQL = SQL + "  	       SUM(ISNULL(ACTUAL_QTY,0))   AS ACTUAL_QTY               FROM FN_GET_GO_PRE_CUT_DETAIL('" + Go + "','" + MO + "','N')  A      ";
            //SQL = SQL + "  	       SUM(ISNULL(ACTUAL_QTY,0))   AS ACTUAL_QTY               FROM FN_GET_GO_PRE_CUT_DETAIL('" + Go + "','" + MO + "')  A      ";

            SQL = SQL + "  	               GROUP BY A.COLOR_CD   ";
            SQL = SQL + "  	               ) G    ON Z.COLOR_CD=G.COLOR_CD    ";
            SQL = SQL + "    )Y ";
            SQL = SQL + "    ON X.COLOR_CD=Y.COLOR_CD";

            tb[1] = DBUtility.GetTable(SQL, "MES");

            //tb[2]

            ////Modified by Jin Song MES133 (Add Color Desc & PPO No)
            ////Bug Fix by ZK on 2014-09-17 due to double order qty when more than 1 ppo is used
            //SQL = " SELECT DISTINCT C.jo_no,c.color_cd,F.COLOR_DESC,c.order_qty,D.PERCENT_SHORT ,str(c.adjust_percents,10,2)+'%',c.adjust_qty,c.ACTUAL_QTY,D.SIZE_CD,D.ACTUAL_QTY  ";
            ////Start modification by LIMML ON 20150701 (ADD MARKER YPD AND PPO YPD)
            ////SQL = SQL + " AS PLAN_CUT_QTY,isnull(E.ORDER_QTY_BY_COL_SIZE,0) as ORDER_QTY_BY_COL_SIZE, F.PPO_NO";
            //SQL = SQL + " AS PLAN_CUT_QTY,isnull(E.ORDER_QTY_BY_COL_SIZE,0) as ORDER_QTY_BY_COL_SIZE, F.PPO_NO, I.MARKER_YPD, I.PPO_YPD";
            ////End modification by LIMML ON 20150701 (ADD MARKER YPD AND PPO YPD)
            //SQL = SQL + " FROM  ";
            //SQL = SQL + " ( ";
            //SQL = SQL + " SELECT  JO_NO,COLOR_CD,SUM(ORDER_QTY) AS ORDER_QTY, ";
            //SQL = SQL + " ROUND(SUM(ISNULL(ADJUST_QTY,0))*100.0/SUM(ORDER_QTY),2) AS ADJUST_PERCENTS ,SUM(ISNULL(ADJUST_QTY,0)) AS ADJUST_QTY, SUM(ACTUAL_QTY) AS ACTUAL_QTY  ";
            //SQL = SQL + " FROM   FN_GET_GO_PRE_CUT_DETAIL('" + Go + "','" + MO + "','N')";
            //SQL = SQL + "  GROUP BY JO_NO,COLOR_CD ";
            //SQL = SQL + " ) C INNER JOIN  ";
            //SQL = SQL + " (SELECT  JO_NO,COLOR_CD,SIZE_CD,('+'+str(MAX(PERCENT_OVER_ALLOWED),10,1)+'%/'+'-'+str(MAX(PERCENT_SHORT_ALLOWED),10,1)+'%') AS PERCENT_SHORT,SUM(ACTUAL_QTY) AS ACTUAL_QTY  ";
            //SQL = SQL + "    FROM   FN_GET_GO_PRE_CUT_DETAIL('" + Go + "','" + MO + "','N') A  ";
            //SQL = SQL + "  LEFT JOIN SC_LOT C WITH(NOLOCK) ON A.JO_NO=C.PO_NO WHERE  C.SC_NO='" + Go + "' "; //Added WITH(NOLOCK) by Jin Song 7/2014
            //SQL = SQL + "        GROUP BY JO_NO,COLOR_CD,SIZE_CD ";
            //SQL = SQL + " )D ON C.JO_NO=D.JO_NO AND C.COLOR_CD=D.COLOR_CD  ";
            //SQL = SQL + " LEFT JOIN";
            //SQL = SQL + " (SELECT A.JO_NO,COLOR_CODE,SIZE_CODE1+'('+SIZE_CODE2+')' AS SIZE_CODE,SUM(ISNULL(QTY,0)) AS ORDER_QTY_BY_COL_SIZE FROM JO_HD A WITH(NOLOCK)"; //Added WITH(NOLOCK) by Jin Song 7/2014
            ////新添加Order Qty;
            //SQL = SQL + " INNER JOIN JO_DT B WITH(NOLOCK) ON A.JO_NO = B.JO_NO "; //Added WITH(NOLOCK) by Jin Song 7/2014
            //SQL = SQL + " WHERE SC_NO = '" + Go + "'";
            //SQL = SQL + " GROUP BY A.JO_NO,COLOR_CODE,SIZE_CODE1+'('+SIZE_CODE2+')'";
            //SQL = SQL + " )E ON C.JO_NO = E.JO_NO AND C.COLOR_CD = E.COLOR_CODE AND D.SIZE_CD = E.SIZE_CODE";
            //SQL = SQL + " INNER JOIN CP_FABRIC_ITEM F WITH(NOLOCK) ON F.COLOR_CD = C.COLOR_CD AND F.GO_NO = '" + Go + "'"; //Added by Jin Song MES133
            ////Start modification by LIMML ON 20150701 (ADD MARKER YPD AND PPO YPD)
            //SQL = SQL + " INNER JOIN ( SELECT G.MO_NO, G.COLOR_CD, G.YPD AS MARKER_YPD, H.YPD AS PPO_YPD FROM CP_MO_OVER_SHORT_SUMMARY (NOLOCK) G ";
            //SQL = SQL + " INNER JOIN (SELECT COLOR_CD, YPD  FROM FN_GET_GO_COLOR_YPD('" + Go + "','" + MO + "'))H";
            //SQL = SQL + " ON G.COLOR_CD = H.COLOR_CD WHERE G.MO_NO LIKE '%" + Go + "%') I";
            //SQL = SQL + " ON I.COLOR_CD = C.COLOR_CD AND I.MO_NO LIKE '%" + Go + "%'";
            ////End modification by LIMML ON 20150701 (ADD MARKER YPD AND PPO YPD)
            //SQL = SQL + " order by C.jo_no,color_cd";
            //tb[2] = DBUtility.GetTable(SQL, "MES");

            if (ByLot == true)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(@"
                
                    DECLARE @TEMP TABLE (COMBINE_JO NVARCHAR(20), ORIGINAL_JO NVARCHAR(20), COLOR NVARCHAR(20), SIZE NVARCHAR(20),
			            OVER_PER DECIMAL(38,3), SHORT_PER DECIMAL(38,3), ORDER_QTY INT)

		            DECLARE @ACTUAL_CUT TABLE (JO_NO NVARCHAR(20), COLOR NVARCHAR(20), SIZE NVARCHAR(20), GARMENT_TYPE NVARCHAR(1), ORDER_QTY INT, ADJUST_QTY INT, ACTUAL_QTY INT)

		            DECLARE @FINAL TABLE (COMBINE_JO_NO NVARCHAR(20), ORIGINAL_JO_NO NVARCHAR(20), GARMENT_TYPE NVARCHAR(1), CUT_LINE NVARCHAR(10), COLOR NVARCHAR(20), SIZE NVARCHAR(20),
			            ORDER_QTY INT, ACTUAL_QTY_JO_COLOR_SIZE INT, OVER_PER DECIMAL(38,3), SHORT_PER DECIMAL(38,3), DISTR_PER DECIMAL(38,3), DISTR_QTY INT, DISTR_ACTUAL_QTY INT)

		            DECLARE @JO_NO NVARCHAR(20),
		             @COLOR NVARCHAR(20),
		             @SIZE NVARCHAR(20),
		             @ACTUAL_QTY INT,
		             @GARMENT_TYPE NVARCHAR(1),
		             @CUT_LINE NVARCHAR(10),
		             @SUM_ORDER_QTY INT,
		             @OVER_SHORT_QTY INT,
		             @SUM_DISTR_PER DECIMAL(38,0),
		             @SUM_PER DECIMAL(38,0), 
		             @TALLY INT,
                     @MAX DECIMAL(18,4)

		            INSERT INTO @TEMP
		            SELECT DISTINCT a.COMBINE_JO_NO AS COMBINE_JO,a.ORIGINAL_JO_NO AS ORIGINAL_JO,c.COLOR_CODE AS COLOR, 
			            c.SIZE_CODE1 + '(' + c.SIZE_CODE2 + ')' AS SIZE, b.PERCENT_OVER_ALLOWED AS OVER_PER, b.PERCENT_SHORT_ALLOWED AS SHORT_PER, c.QTY AS ORDER_QTY
		            FROM JO_COMBINE_MAPPING a WITH(NOLOCK)
		            INNER JOIN (
			            SELECT PO_NO, PERCENT_OVER_ALLOWED, PERCENT_SHORT_ALLOWED FROM SC_LOT WITH(NOLOCK) WHERE SC_NO='{0}'
		            ) b ON a.ORIGINAL_JO_NO=b.PO_NO
		            INNER JOIN JO_DT c WITH(NOLOCK) ON a.ORIGINAL_JO_NO=c.JO_NO
		            INNER JOIN JO_HD d WITH(NOLOCK) ON a.ORIGINAL_JO_NO=d.JO_NO
		            WHERE d.SC_NO = '{0}'
		            ORDER BY COLOR, SIZE

		            --select * from @TEMP

		            --Actual Cut Qty (forecast)
		            INSERT INTO @ACTUAL_CUT
		            SELECT DISTINCT B.JO_NO, B.COLOR_CD AS COLOR, B.SIZE_CD AS SIZE, A.GARMENT_TYPE_CD AS GARMENT_TYPE, SUM(B.ORDER_QTY) AS ORDER_QTY, ISNULL(SUM(B.ADJUST_QTY),0) AS ADJUST_QTY, SUM(B.ACTUAL_QTY) AS ACTUAL_QTY
		            FROM JO_HD A WITH(NOLOCK) 
		            INNER JOIN (SELECT JO_NO, COLOR_CD, SIZE_CD, SUM(ORDER_QTY) AS ORDER_QTY, SUM(ISNULL(ACTUAL_QTY,0)-ORDER_QTY) AS ADJUST_QTY, SUM(ACTUAL_QTY) AS ACTUAL_QTY FROM FN_GET_GO_PRE_CUT_DETAIL('{0}','{1}','N') 
			            GROUP BY JO_NO, COLOR_CD, SIZE_CD) B ON A.JO_NO=B.JO_NO
		            INNER JOIN (SELECT DISTINCT COMBINE_JO FROM @TEMP) C ON A.JO_NO=C.COMBINE_JO
		            WHERE A.SC_NO = '{0}'
		            GROUP BY B.JO_NO, B.COLOR_CD, A.GARMENT_TYPE_CD, B.SIZE_CD
		            ORDER BY COLOR, SIZE

		            --SELECT * FROM @ACTUAL_CUT

		            DECLARE cursorName CURSOR FOR SELECT JO_NO, COLOR, SIZE, ACTUAL_QTY, GARMENT_TYPE FROM @ACTUAL_CUT
		            OPEN cursorName 
		            FETCH NEXT FROM cursorName INTO @JO_NO, @COLOR, @SIZE, @ACTUAL_QTY, @GARMENT_TYPE;
		            WHILE @@FETCH_STATUS = 0
		            BEGIN
            			
			            SET @SUM_ORDER_QTY = (SELECT SUM(ORDER_QTY) FROM @TEMP WHERE COMBINE_JO=@JO_NO AND SIZE=@SIZE AND COLOR=@COLOR)
            			
			            --STEP2
			            SET @OVER_SHORT_QTY = @ACTUAL_QTY - @SUM_ORDER_QTY
            			
			            --If STEP2 > 0, use short percentage
			            IF @OVER_SHORT_QTY < 0 BEGIN
				            --STEP1
				            INSERT INTO @FINAL
				            SELECT COMBINE_JO AS COMBINE_JO_NO, ORIGINAL_JO AS ORIGINAL_JO_NO, @GARMENT_TYPE AS GARMENT_TYPE, @CUT_LINE AS CUT_LINE, COLOR, SIZE, ORDER_QTY, @ACTUAL_QTY AS ACTUAL_QTY_JO_COLOR_SIZE, 
					            OVER_PER, SHORT_PER, 0 AS DISTR_PER, 0 AS DISTR_QTY, 0 AS DISTR_ACTUAL_QTY
				            FROM @TEMP
				            WHERE COLOR=@COLOR AND SIZE=@SIZE AND COMBINE_JO=@JO_NO

				            --Checking min/max is it all zero%
				            SET @SUM_PER = (SELECT SUM(SHORT_PER) FROM @FINAL WHERE COLOR=@COLOR AND SIZE=@SIZE)

				            IF @SUM_PER > 0 BEGIN
					            --STEP3
					            UPDATE @FINAL SET DISTR_PER = ORDER_QTY*SHORT_PER/100 WHERE COLOR=@COLOR AND SIZE=@SIZE AND COMBINE_JO_NO=@JO_NO
				            END
				            ELSE BEGIN --Use ratio of order_qty
					            UPDATE @FINAL SET DISTR_PER = ORDER_QTY/100 WHERE COLOR=@COLOR AND SIZE=@SIZE AND COMBINE_JO_NO=@JO_NO
				            END 

					            --STEP4
					            SET @SUM_DISTR_PER = (SELECT SUM(DISTR_PER) FROM @FINAL WHERE COLOR=@COLOR AND SIZE=@SIZE AND COMBINE_JO_NO=@JO_NO)
					            UPDATE @FINAL SET DISTR_QTY = ROUND((@OVER_SHORT_QTY * DISTR_PER / (CASE WHEN @SUM_DISTR_PER=0 THEN 1 ELSE @SUM_DISTR_PER END)),0)
						            WHERE COLOR=@COLOR AND SIZE=@SIZE AND COMBINE_JO_NO=@JO_NO

					            --STEP5
					            UPDATE @FINAL SET DISTR_ACTUAL_QTY = ORDER_QTY + DISTR_QTY WHERE COLOR=@COLOR AND SIZE=@SIZE AND COMBINE_JO_NO=@JO_NO
			            END

			            --If STEP2 < 0,use over percentage
			            ELSE IF @OVER_SHORT_QTY > 0 BEGIN
				            --STEP1
				            INSERT INTO @FINAL
				            SELECT COMBINE_JO AS COMBINE_JO_NO, ORIGINAL_JO AS ORIGINAL_JO_NO,  @GARMENT_TYPE AS GARMENT_TYPE, @CUT_LINE AS CUT_LINE, COLOR, SIZE, ORDER_QTY, @ACTUAL_QTY AS ACTUAL_QTY_JO_COLOR_SIZE, 
					            OVER_PER, SHORT_PER, 0 AS DISTR_PER, 0 AS DISTR_QTY, 0 AS DISTR_ACTUAL_QTY
				            FROM @TEMP
				            WHERE COLOR=@COLOR AND SIZE=@SIZE AND COMBINE_JO=@JO_NO

				            --Checking min/max is it all zero%
				            SET @SUM_PER = (SELECT SUM(OVER_PER) FROM @FINAL WHERE COLOR=@COLOR AND SIZE=@SIZE)

				            IF @SUM_PER > 0 BEGIN
					            --STEP3
					            UPDATE @FINAL SET DISTR_PER = ORDER_QTY*OVER_PER/100 WHERE COLOR=@COLOR AND SIZE=@SIZE AND COMBINE_JO_NO=@JO_NO
				            END
				            ELSE BEGIN
					            UPDATE @FINAL SET DISTR_PER = ORDER_QTY WHERE COLOR=@COLOR AND SIZE=@SIZE AND COMBINE_JO_NO=@JO_NO
				            END

				            --SELECT * FROM @FINAL
				            --STEP4
				            SET @SUM_DISTR_PER = (SELECT SUM(DISTR_PER) FROM @FINAL WHERE COLOR=@COLOR AND SIZE=@SIZE AND COMBINE_JO_NO=@JO_NO)
				            UPDATE @FINAL SET DISTR_QTY = ROUND((@OVER_SHORT_QTY * DISTR_PER / (CASE WHEN @SUM_DISTR_PER=0 THEN 1 ELSE @SUM_DISTR_PER END)),0)
					            WHERE COLOR=@COLOR AND SIZE=@SIZE AND COMBINE_JO_NO=@JO_NO

				            --STEP5
				            UPDATE @FINAL SET DISTR_ACTUAL_QTY = ORDER_QTY + DISTR_QTY WHERE COLOR=@COLOR AND SIZE=@SIZE AND COMBINE_JO_NO=@JO_NO
			            END

			            ELSE BEGIN
				            --If STEP2 = 0, direct use order qty
				            INSERT INTO @FINAL
				            SELECT COMBINE_JO AS COMBINE_JO_NO, ORIGINAL_JO AS ORIGINAL_JO_NO,  @GARMENT_TYPE AS GARMENT_TYPE, @CUT_LINE AS CUT_LINE, COLOR, SIZE, ORDER_QTY, @ACTUAL_QTY AS ACTUAL_QTY_JO_COLOR_SIZE, 
					            OVER_PER, SHORT_PER, 0 AS DISTR_PER, 0 AS DISTR_QTY, ORDER_QTY AS DISTR_ACTUAL_QTY
				            FROM @TEMP
				            WHERE COLOR=@COLOR AND SIZE=@SIZE AND COMBINE_JO=@JO_NO
			            END

			            --Checking before final
			            SET @TALLY = (SELECT SUM(DISTR_ACTUAL_QTY) FROM @FINAL WHERE COLOR=@COLOR AND SIZE=@SIZE AND COMBINE_JO_NO=@JO_NO)

			            IF (@TALLY - @ACTUAL_QTY <> 0) BEGIN
                            
                            IF @OVER_SHORT_QTY > 0 BEGIN
                                SET @MAX = (SELECT MAX(DISTR_PER) FROM @FINAL WHERE COMBINE_JO_NO=@JO_NO AND COLOR=@COLOR AND SIZE=@SIZE) 
                            END
                            ELSE IF @OVER_SHORT_QTY < 0 BEGIN
                                SET @MAX = (SELECT MAX(DISTR_PER) FROM @FINAL WHERE COMBINE_JO_NO=@JO_NO AND COLOR=@COLOR AND SIZE=@SIZE)
                            END
                            ELSE BEGIN
                                SET @MAX = (SELECT MAX(ORDER_QTY) FROM @FINAL WHERE COMBINE_JO_NO=@JO_NO AND COLOR=@COLOR AND SIZE=@SIZE)
                            END
                            
		                    IF @OVER_SHORT_QTY = 0 BEGIN
			                    UPDATE @FINAL SET DISTR_ACTUAL_QTY = DISTR_ACTUAL_QTY + (@ACTUAL_QTY - @TALLY) 
			                    WHERE COMBINE_JO_NO=@JO_NO AND ORIGINAL_JO_NO=(SELECT TOP 1 ORIGINAL_JO_NO FROM @FINAL WHERE ORDER_QTY=@MAX AND COMBINE_JO_NO=@JO_NO
				                    AND COLOR=@COLOR AND SIZE=@SIZE) AND COLOR=@COLOR AND SIZE=@SIZE
		                    END
		                    ELSE BEGIN
			                    UPDATE @FINAL SET DISTR_ACTUAL_QTY = DISTR_ACTUAL_QTY + (@ACTUAL_QTY - @TALLY) 
			                    WHERE COMBINE_JO_NO=@JO_NO AND ORIGINAL_JO_NO=(SELECT TOP 1 ORIGINAL_JO_NO FROM @FINAL WHERE DISTR_PER=@MAX AND COMBINE_JO_NO=@JO_NO
				                    AND COLOR=@COLOR AND SIZE=@SIZE) AND COLOR=@COLOR AND SIZE=@SIZE
		                    END
                        END 
            			
			            FETCH NEXT FROM cursorName INTO @JO_NO, @COLOR, @SIZE, @ACTUAL_QTY, @GARMENT_TYPE;
		            END
		            CLOSE cursorName
		            DEALLOCATE cursorName

		            --SELECT * FROM @FINAL


		            (SELECT DISTINCT A.COMBINE_JO_NO AS JO_NO, A.ORIGINAL_JO_NO AS LOT_NO, A.COLOR AS COLOR_CD, B.COLOR_DESC, C.SUM_ORDER AS ORDER_QTY, ('+'+STR(A.OVER_PER,10,1)+'%/'+'-'+STR(A.SHORT_PER,10,1)+'%') AS PERCENT_SHORT, 
			            str(ROUND(((F.DISTR_QTY - F.ORDER_QTY)*100.0/F.ORDER_QTY),2),10,2)+'%' AS ADJUST_PERCENT ,C.SUM_ACTUAL-C.SUM_ORDER AS ADJUST_QTY, C.SUM_ACTUAL AS ACTUAL_QTY, A.SIZE AS SIZE_CD, 
			            A.DISTR_ACTUAL_QTY AS PLAN_CUT_QTY, A.ORDER_QTY AS ORDER_QTY_BY_COL_SIZE, B.PPO_NO, I.MARKER_YPD, I.PPO_YPD
		            FROM @FINAL A
		            INNER JOIN CP_FABRIC_ITEM B WITH(NOLOCK) ON A.COLOR=B.COLOR_CD AND B.GO_NO='{0}'
		            INNER JOIN (SELECT COMBINE_JO_NO, ORIGINAL_JO_NO, COLOR, SUM(DISTR_ACTUAL_QTY) AS SUM_ACTUAL, SUM(ORDER_QTY) AS SUM_ORDER FROM @FINAL GROUP BY COMBINE_JO_NO, ORIGINAL_JO_NO, COLOR) C ON A.COMBINE_JO_NO=C.COMBINE_JO_NO AND A.ORIGINAL_JO_NO=C.ORIGINAL_JO_NO AND A.COLOR=C.COLOR
		            INNER JOIN (
			            SELECT G.MO_NO, G.COLOR_CD, G.YPD AS MARKER_YPD, H.YPD AS PPO_YPD FROM CP_MO_OVER_SHORT_SUMMARY (NOLOCK) G  
			            INNER JOIN 
			            (SELECT REF_COLOR, YPD  
			            FROM FN_GET_GO_COLOR_YPD('{0}','{1}'))H ON G.COLOR_CD = H.REF_COLOR WHERE G.MO_NO LIKE '%{0}%'
		            ) I ON I.COLOR_CD = A.COLOR AND I.MO_NO LIKE '%{0}%'
		            INNER JOIN (SELECT PO_NO, CONVERT(NVARCHAR(5),LOT_NO) AS LOT_NO FROM SC_LOT WITH(NOLOCK) WHERE SC_NO='{0}') E ON E.PO_NO=A.ORIGINAL_JO_NO
                    INNER JOIN (SELECT DISTINCT COMBINE_JO_NO, ORIGINAL_JO_NO, COLOR, SUM(ORDER_QTY) AS ORDER_QTY, SUM(DISTR_ACTUAL_QTY) AS DISTR_QTY
	                    FROM @FINAL GROUP BY COMBINE_JO_NO, ORIGINAL_JO_NO, COLOR) F ON A.ORIGINAL_JO_NO=F.ORIGINAL_JO_NO AND A.COLOR=F.COLOR
		            )
		            UNION ALL
		            (
			             SELECT DISTINCT C.jo_no ,'' as lot_no,c.color_cd,F.COLOR_DESC,c.order_qty,
                         CASE WHEN C.jo_no like '%CB%' THEN ('+' + str(j.PERCENT_OVER_ALLOWED,10,1)+'%/-' + str(j.PERCENT_SHORT_ALLOWED,10,1) + '%') ELSE D.PERCENT_SHORT END ,
                         str(c.adjust_percents,10,2)+'%', c.adjust_qty,c.ACTUAL_QTY,D.SIZE_CD,D.ACTUAL_QTY   AS PLAN_CUT_QTY,isnull(E.ORDER_QTY_BY_COL_SIZE,0) as ORDER_QTY_BY_COL_SIZE, 
			             F.PPO_NO, I.MARKER_YPD, I.PPO_YPD
			             FROM   (  SELECT  JO_NO,COLOR_CD,SUM(ORDER_QTY) AS ORDER_QTY,  
			             ROUND(SUM(ISNULL(ACTUAL_QTY,0)-ORDER_QTY)*100.0/SUM(ORDER_QTY),2) AS ADJUST_PERCENTS ,SUM(ISNULL(ACTUAL_QTY,0)-ORDER_QTY) AS ADJUST_QTY, 
			             SUM(ACTUAL_QTY) AS ACTUAL_QTY   
			             FROM   FN_GET_GO_PRE_CUT_DETAIL('{0}','{1}','N')  GROUP BY JO_NO,COLOR_CD  ) C 
			             INNER JOIN   (SELECT  JO_NO,COLOR_CD,SIZE_CD,CASE WHEN JO_NO LIKE '%CB%' THEN '' ELSE ('+'+str(MAX(PERCENT_OVER_ALLOWED),10,1)+'%/'+'-'+str(MAX(PERCENT_SHORT_ALLOWED),10,1)+'%') END AS PERCENT_SHORT,
				            SUM(ACTUAL_QTY) AS ACTUAL_QTY      FROM   FN_GET_GO_PRE_CUT_DETAIL('{0}','{1}','N') A    
			             LEFT JOIN (SELECT PO_NO,PERCENT_OVER_ALLOWED,PERCENT_SHORT_ALLOWED FROM SC_LOT  WITH(NOLOCK) WHERE SC_NO='{0}') C ON A.JO_NO=C.PO_NO            
				            GROUP BY JO_NO,COLOR_CD,SIZE_CD  )D ON C.JO_NO=D.JO_NO AND C.COLOR_CD=D.COLOR_CD   
			             LEFT JOIN (SELECT A.JO_NO,COLOR_CODE,SIZE_CODE1+'('+SIZE_CODE2+')' AS SIZE_CODE,SUM(ISNULL(QTY,0)) AS ORDER_QTY_BY_COL_SIZE 
			             FROM JO_HD A WITH(NOLOCK) INNER JOIN JO_DT B WITH(NOLOCK) ON A.JO_NO = B.JO_NO  
			             WHERE SC_NO = '{0}' 
			             GROUP BY A.JO_NO,COLOR_CODE,SIZE_CODE1+'('+SIZE_CODE2+')' )E ON C.JO_NO = E.JO_NO AND C.COLOR_CD = E.COLOR_CODE AND D.SIZE_CD = E.SIZE_CODE 
			             INNER JOIN CP_FABRIC_ITEM F WITH(NOLOCK) ON F.COLOR_CD = C.COLOR_CD AND F.GO_NO = '{0}' 
			             INNER JOIN 
			             ( SELECT G.MO_NO, G.COLOR_CD, G.YPD AS MARKER_YPD, H.YPD AS PPO_YPD FROM CP_MO_OVER_SHORT_SUMMARY (NOLOCK) G  
			             INNER JOIN 
			             (SELECT REF_COLOR, YPD  
			             FROM FN_GET_GO_COLOR_YPD('{0}','{1}'))H ON G.COLOR_CD = H.REF_COLOR WHERE G.MO_NO LIKE '%{0}%') I ON I.COLOR_CD = C.COLOR_CD AND I.MO_NO 
			             LIKE '%{0}%' 
                         LEFT JOIN 
	                        (SELECT A.COMBINE_JO_NO,CAST(ROUND(((SUM(B.TOTAL_QTY*(1+C.PERCENT_OVER_ALLOWED/100))/SUM(B.TOTAL_QTY))-1)*100,2)AS DECIMAL(38,2)) AS PERCENT_OVER_ALLOWED, 
		                        CAST(ROUND(((SUM(B.TOTAL_QTY*(1-C.PERCENT_SHORT_ALLOWED/100))/SUM(B.TOTAL_QTY))-1)*-100,2)AS DECIMAL(38,2)) AS PERCENT_SHORT_ALLOWED
		                        FROM JO_COMBINE_MAPPING A WITH(NOLOCK)
		                        INNER JOIN JO_HD B WITH(NOLOCK) ON A.ORIGINAL_JO_NO = B.JO_NO
		                        INNER JOIN SC_LOT C WITH(NOLOCK) ON B.SC_NO = C.SC_NO AND B.LOT_NO = C.LOT_NO
		                     WHERE B.SC_NO='{0}'
		                     GROUP BY A.COMBINE_JO_NO) J ON J.COMBINE_JO_NO=C.jo_no
                         WHERE NOT EXISTS(SELECT ORIGINAL_JO_NO FROM JO_COMBINE_MAPPING WITH(NOLOCK) WHERE ORIGINAL_JO_NO=C.JO_NO) 
		            ) order by JO_NO, COLOR_CD, LOT_NO
            ", Go, MO);

                tb[2] = MESComment.DBUtility.GetTable(sb.ToString(), "MES");
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(@"
                 (SELECT DISTINCT C.jo_no,c.color_cd,F.COLOR_DESC,c.order_qty,D.PERCENT_SHORT ,str(c.adjust_percents,10,2)+'%',
	                c.adjust_qty,c.ACTUAL_QTY,D.SIZE_CD,D.ACTUAL_QTY   AS PLAN_CUT_QTY,isnull(E.ORDER_QTY_BY_COL_SIZE,0) as ORDER_QTY_BY_COL_SIZE, 
	                F.PPO_NO, I.MARKER_YPD, I.PPO_YPD 
	                FROM   
	                (SELECT JO_NO,COLOR_CD,SUM(ORDER_QTY) AS ORDER_QTY,  ROUND(SUM(ISNULL(ACTUAL_QTY,0)-ORDER_QTY)*100.0/SUM(ORDER_QTY),2) AS ADJUST_PERCENTS ,
		                SUM(ISNULL(ACTUAL_QTY,0)-ORDER_QTY) AS ADJUST_QTY, SUM(ACTUAL_QTY) AS ACTUAL_QTY   
		                FROM FN_GET_GO_PRE_CUT_DETAIL('{0}','{1}','N')  GROUP BY JO_NO,COLOR_CD  ) C 
	                INNER JOIN   (SELECT  JO_NO,COLOR_CD,SIZE_CD,
		                ('+'+str(MAX(PERCENT_OVER_ALLOWED),10,1)+'%/'+'-'+str(MAX(PERCENT_SHORT_ALLOWED),10,1)+'%') AS PERCENT_SHORT,
		                SUM(ACTUAL_QTY) AS ACTUAL_QTY 
		                FROM FN_GET_GO_PRE_CUT_DETAIL('{0}','{1}','N') A    
		                LEFT JOIN (
			                SELECT A.COMBINE_JO_NO,CAST(ROUND(((SUM(B.TOTAL_QTY*(1+C.PERCENT_OVER_ALLOWED/100))/SUM(B.TOTAL_QTY))-1)*100,2)AS DECIMAL(38,2)) AS PERCENT_OVER_ALLOWED, 
			                CAST(ROUND(((SUM(B.TOTAL_QTY*(1-C.PERCENT_SHORT_ALLOWED/100))/SUM(B.TOTAL_QTY))-1)*-100,2)AS DECIMAL(38,2)) AS PERCENT_SHORT_ALLOWED
			                FROM JO_COMBINE_MAPPING A WITH(NOLOCK)
			                INNER JOIN JO_HD B WITH(NOLOCK) ON A.ORIGINAL_JO_NO = B.JO_NO
			                INNER JOIN SC_LOT C WITH(NOLOCK) ON B.SC_NO = C.SC_NO AND B.LOT_NO = C.LOT_NO
			                WHERE B.SC_NO='{0}'
			                GROUP BY A.COMBINE_JO_NO
		                ) LOT ON LOT.COMBINE_JO_NO=A.JO_NO    
	                GROUP BY JO_NO,COLOR_CD,SIZE_CD  )D ON C.JO_NO=D.JO_NO AND C.COLOR_CD=D.COLOR_CD   
	                LEFT JOIN (SELECT A.JO_NO,COLOR_CODE,SIZE_CODE1+'('+SIZE_CODE2+')' AS SIZE_CODE,SUM(ISNULL(QTY,0)) AS ORDER_QTY_BY_COL_SIZE 
	                FROM JO_HD A WITH(NOLOCK) INNER JOIN JO_DT B WITH(NOLOCK) ON A.JO_NO = B.JO_NO  
	                WHERE SC_NO = '{0}' GROUP BY A.JO_NO,COLOR_CODE,SIZE_CODE1+'('+SIZE_CODE2+')' )E ON C.JO_NO = E.JO_NO 
		                AND C.COLOR_CD = E.COLOR_CODE AND D.SIZE_CD = E.SIZE_CODE 
	                INNER JOIN CP_FABRIC_ITEM F WITH(NOLOCK) ON F.COLOR_CD = C.COLOR_CD AND F.GO_NO = '{0}' 
	                INNER JOIN ( SELECT G.MO_NO, G.COLOR_CD, G.YPD AS MARKER_YPD, H.YPD AS PPO_YPD 
	                FROM CP_MO_OVER_SHORT_SUMMARY (NOLOCK) G  
	                INNER JOIN (SELECT REF_COLOR, YPD  FROM FN_GET_GO_COLOR_YPD('{0}','{1}'))H ON G.COLOR_CD = H.REF_COLOR 
	                WHERE G.MO_NO LIKE '%{0}%') I ON I.COLOR_CD = C.COLOR_CD AND I.MO_NO LIKE '%{0}%'
                        AND C.JO_NO LIKE '%CB%')
                UNION ALL
                (
                SELECT DISTINCT C.jo_no,c.color_cd,F.COLOR_DESC,c.order_qty,D.PERCENT_SHORT ,
                str(c.adjust_percents,10,2)+'%',c.adjust_qty,c.ACTUAL_QTY,D.SIZE_CD,D.ACTUAL_QTY   AS PLAN_CUT_QTY,
                isnull(E.ORDER_QTY_BY_COL_SIZE,0) as ORDER_QTY_BY_COL_SIZE, F.PPO_NO, I.MARKER_YPD, I.PPO_YPD 
                FROM (  
	                SELECT  JO_NO,COLOR_CD,SUM(ORDER_QTY) AS ORDER_QTY,  ROUND(SUM(ISNULL(ACTUAL_QTY,0)-ORDER_QTY)*100.0/SUM(ORDER_QTY),2) AS ADJUST_PERCENTS ,
	                SUM(ISNULL(ACTUAL_QTY,0)-ORDER_QTY) AS ADJUST_QTY, SUM(ACTUAL_QTY) AS ACTUAL_QTY   FROM   FN_GET_GO_PRE_CUT_DETAIL('{0}','{1}','N')  
	                GROUP BY JO_NO,COLOR_CD) C 
	                INNER JOIN (
		                SELECT  JO_NO,COLOR_CD,SIZE_CD,('+'+str(MAX(PERCENT_OVER_ALLOWED),10,1)+'%/'+'-'+str(MAX(PERCENT_SHORT_ALLOWED),10,1)+'%')
		                AS PERCENT_SHORT,SUM(ACTUAL_QTY) AS ACTUAL_QTY      FROM   FN_GET_GO_PRE_CUT_DETAIL('{0}','{1}','N') A    
		                LEFT JOIN SC_LOT C WITH(NOLOCK) ON A.JO_NO=C.PO_NO WHERE C.SC_NO='{0}'         
		                GROUP BY JO_NO,COLOR_CD,SIZE_CD  )D ON C.JO_NO=D.JO_NO AND C.COLOR_CD=D.COLOR_CD   
		                LEFT JOIN (
			                SELECT A.JO_NO,COLOR_CODE,SIZE_CODE1+'('+SIZE_CODE2+')' AS SIZE_CODE,SUM(ISNULL(QTY,0)) AS ORDER_QTY_BY_COL_SIZE 
			                FROM JO_HD A WITH(NOLOCK) 
			                INNER JOIN JO_DT B WITH(NOLOCK) ON A.JO_NO = B.JO_NO  
			                WHERE SC_NO = '{0}' 
			                GROUP BY A.JO_NO,COLOR_CODE,SIZE_CODE1+'('+SIZE_CODE2+')' )E ON C.JO_NO = E.JO_NO AND C.COLOR_CD = E.COLOR_CODE 
		                AND D.SIZE_CD = E.SIZE_CODE 
		                INNER JOIN CP_FABRIC_ITEM F WITH(NOLOCK) ON F.COLOR_CD = C.COLOR_CD AND F.GO_NO = '{0}' 
		                INNER JOIN ( SELECT G.MO_NO, G.COLOR_CD, G.YPD AS MARKER_YPD, H.YPD AS PPO_YPD 
	                FROM CP_MO_OVER_SHORT_SUMMARY (NOLOCK) G  
	                INNER JOIN (SELECT REF_COLOR, YPD  FROM FN_GET_GO_COLOR_YPD('{0}','{1}'))H ON G.COLOR_CD = H.REF_COLOR 
	                WHERE G.MO_NO LIKE '%{0}%') I ON I.COLOR_CD = C.COLOR_CD AND I.MO_NO LIKE '%{0}%'
                        AND NOT EXISTS(SELECT ORIGINAL_JO_NO FROM JO_COMBINE_MAPPING WITH(NOLOCK) WHERE ORIGINAL_JO_NO=C.JO_NO)) 
                order by jo_no,color_cd", Go, MO);
                tb[2] = MESComment.DBUtility.GetTable(sb.ToString(), "MES");
            }

            SQL = "SELECT  DISTINCT C.SIZE_CD,A.SEQ FROM CP_SIZE_SEQ A,dbo.CP_MO_HD B,CP_MO_CS_BREAKDOWN C WHERE B.GO_NO='" + Go + "'";
            SQL += " AND A.MO_NO LIKE '%" + MO + "%' AND A.MO_NO=B.MO_NO AND B.MO_NO=C.MO_NO AND A.SIZE_CD=C.SIZE_CD ORDER BY 2";
            tb[3] = DBUtility.GetTable(SQL, "MES");

            SQL = " select JOB_ORDER_NO,COLOR,[SIZE]+''+SIZE2 as size,REMARK ";
            SQL = SQL + " From dbo.PRD_JO_REMARKS A, JO_HD B ";
            SQL = SQL + " where remark_type in('CUTPLAN_PPC','CUTPLAN_CUT') ";
            SQL = SQL + " AND A.FACTORY_CD=B.FACTORY_CD ";
            SQL = SQL + " AND A.JOB_ORDER_NO=B.JO_NO ";
            SQL = SQL + " AND B.SC_NO ='" + Go + "' ";
            SQL = SQL + " ORDER BY JOB_ORDER_NO,REMARK_TYPE,COLOR,[SIZE],SIZE2 ";
            tb[4] = DBUtility.GetTable(SQL, "MES");
            return tb;
        }

        public static DataTable[] Get_PreCut_DetailBfOSCut(string Go, string MO, string CHBfOSCut)
        {
            DataTable[] tb = new DataTable[5];
            string SQL = "";
            SQL += " SELECT  CUSTOMER_NAME,A.SIZE_CD ,SUM(ACTUAL_QTY) AS ACTUAL_QTY   ";
            SQL += " FROM FN_GET_GO_PRE_CUT_DETAIL('" + Go + "','" + MO + "', '" + CHBfOSCut + "') A ";
            SQL += " inner join (select MAX(D.SHORT_NAME) as CUSTOMER_NAME,SIZE_CD,SEQ ";
            SQL += " from CP_MO_HD B WITH(NOLOCK)"; //Added WITH(NOLOCK) by Jin Song 7/2014
            SQL += " INNER JOIN  SC_HD C WITH(NOLOCK) ON B.GO_NO=C.SC_NO "; //Added WITH(NOLOCK) by Jin Song 7/2014
            SQL += " LEFT JOIN GEN_CUSTOMER D WITH(NOLOCK) ON C.CUSTOMER_CD=D.CUSTOMER_CD "; //Added WITH(NOLOCK) by Jin Song 7/2014
            SQL += " INNER JOIN CP_SIZE_SEQ S WITH(NOLOCK) "; //Added WITH(NOLOCK) by Jin Song 7/2014
            SQL += " on S.MO_NO=B.MO_NO ";
            SQL += " where  B.GO_NO='" + Go + "' ";
            SQL += "  AND B.MO_NO LIKE '%" + MO + "%'";
            SQL += " group by SIZE_CD,SEQ ) SEQ";
            SQL += " on SEQ.SIZE_CD=A.SIZE_CD ";
            SQL += "  WHERE 1=1";
            SQL += " GROUP BY CUSTOMER_NAME,A.SIZE_CD,SEQ.SEQ ";
            SQL += " ORDER BY  SEQ.SEQ";
            tb[0] = DBUtility.GetTable(SQL, "MES");

            //TB[1]
            //Modified by Jin Song MES133 (Add Color Desc)
            SQL = " SELECT X.COLOR_CD,X.COLOR_DESC,Y.ORDER_QTY,X.FAB_PATTERN, ";
            SQL = SQL + "    (str(ROUND(Y.ADJUST_QTY*100.0/Y.ORDER_QTY,2),10,2)+'%') AS ADJUST_PERCENTS,";
            SQL = SQL + "    Y.ADJUST_QTY,str(Y.ACTUAL_QTY,10,2),str(X.FABRIC_QTY,10,2),CASE WHEN X.FABRIC_QTY -Y.ACTUAL_QTY/12.0* (Y.MARKER_YPD+X.BINDING_YPD)<0 then 0 ELSE X.FABRIC_QTY -Y.ACTUAL_QTY/12.0* (Y.MARKER_YPD+X.BINDING_YPD) END AS BALANCE_QTY";
            SQL = SQL + " ,STR(X.RECEIVED_QTY,10,2)";
            SQL = SQL + " ,STR(X.SPARE_FABRIC_QTY,10,2)";
            SQL = SQL + " ,STR(X.BINDING_FABRIC_QTY,10,2)";
            SQL = SQL + " ,STR(X.RECEIVED_QTY-X.BINDING_FABRIC_QTY-X.SPARE_FABRIC_QTY,10,2)";
            SQL = SQL + " ,STR(X.ALLOCATED_QTY,10,2)";
            SQL = SQL + " ,STR(X.RECEIVED_QTY-X.BINDING_FABRIC_QTY-X.SPARE_FABRIC_QTY-X.ALLOCATED_QTY,10,2)";
            SQL = SQL + "      ,str(Y.NET_YPD,10,2),str(Y.MARKER_YPD,10,2),str(X.PPO_YPD,10,2), ";
            SQL = SQL + "    str(Y.MARKER_UTILIZATION,10,2)+'%' ";
            SQL = SQL + "    ,str(Y.MARKER_WASTAGE,10,2)+'%',";
            SQL = SQL + "    X.FABRIC_WIDTH ";
            SQL = SQL + "    FROM ";
            SQL = SQL + "  ( ";
            SQL = SQL + "    SELECT A.COLOR_CD,A.COLOR_DESC,SUM(FABRIC_QTY-ISNULL(SPARE_FABRIC_QTY,0)) AS FABRIC_QTY, ";
            SQL = SQL + "  SUM(ISNULL(FABRIC_QTY,0)) AS RECEIVED_QTY ";
            SQL = SQL + "  ,SUM(ISNULL(ALLOCATED_QTY,0)) AS ALLOCATED_QTY,SUM(ISNULL(SPARE_FABRIC_QTY,0)) AS SPARE_FABRIC_QTY";
            SQL = SQL + "  ,SUM(ISNULL(BINDING_FABRIC_QTY,0)) AS BINDING_FABRIC_QTY,";
            SQL = SQL + "    MAX(A.PPO_YPD) AS PPO_YPD,MAX(ISNULL(BINDING_YPD,0)) AS BINDING_YPD,MAX(A.WIDTH) AS FABRIC_WIDTH,MAX(A.FAB_PATTERN) AS FAB_PATTERN,MAX(PERCENT_OVER_ALLOWED) AS PERCENT_OVER_ALLOWED  ";
            SQL = SQL + "    FROM CP_FABRIC_ITEM A INNER JOIN SC_HD B ON A.GO_NO=B.SC_NO "; //By ZouShiChang ON 2013.08.29 MES024 SC_HD Change SC_HD
            SQL = SQL + "    WHERE  1=1";
            if (!Go.Equals(""))
            {
                SQL = SQL + "    AND A.GO_NO='" + Go + "'  ";
            }
            SQL = SQL + "    AND EXISTS(SELECT 1 FROM  CP_MARKER_SET C ";
            SQL = SQL + "    INNER JOIN CP_MARKER D ON C.MARKER_SET_ID = D.MARKER_SET_ID ";
            SQL = SQL + "    INNER JOIN CP_MARKER_FABRIC E ON D.MARKER_ID = E.MARKER_ID";
            SQL = SQL + "    INNER JOIN CP_MO_HD F ON C.MO_NO = F.MO_NO";
            SQL = SQL + "    WHERE A.FAB_ITEM_ID = E.FAB_ITEM_ID AND A.GO_NO=F.GO_NO AND A.PART_TYPE_CD=F.PART_TYPE";
            if (!MO.Equals(""))
            {
                SQL = SQL + "    AND C.MO_NO ='" + MO + "'";
            }
            SQL = SQL + "    )GROUP BY A.COLOR_CD, A.COLOR_DESC ";
            SQL = SQL + "  ) X ";
            SQL = SQL + "   INNER JOIN ";
            SQL = SQL + "  ( ";
            SQL = SQL + "    SELECT   Z.COLOR_CD ,G.ORDER_QTY,G.ADJUST_QTY,G.ACTUAL_QTY ,Z.MARKER_WASTAGE,   Z.MARKER_UTILIZATION,   Z.NET_YPD ,Z.MARKER_YPD    ";
            SQL = SQL + "  		FROM    ";
            SQL = SQL + "  	  (";
            SQL = SQL + "    	  SELECT REF_COLOR AS COLOR_CD,SUM(TOTAL_LENGTH)/SUM(GMT_QTY)*12 AS NET_YPD,SUM(ACT_QTY)/SUM(GMT_QTY)*100 AS MARKER_UTILIZATION,";
            SQL = SQL + "  	  SUM(TOTAL_LENGTH*(1+MARKER_WASTAGE*0.01))/SUM(GMT_QTY)*12 AS MARKER_YPD,";
            SQL = SQL + "  	  MAX(MARKER_WASTAGE) AS MARKER_WASTAGE";
            SQL = SQL + "  	  FROM  [FN_GET_GO_COLOR_YPD]('" + Go + "','" + MO + "')";
            SQL = SQL + "  	  GROUP BY REF_COLOR";
            SQL = SQL + "  	  ) Z ";
            SQL = SQL + "  	    INNER JOIN (";
            SQL = SQL + "  	       SELECT A.COLOR_CD,SUM(A.ORDER_QTY) AS ORDER_QTY,SUM(ISNULL(A.ACTUAL_QTY,0)-A.ORDER_QTY) AS ADJUST_QTY,";
            SQL = SQL + "  	       SUM(ISNULL(ACTUAL_QTY,0))   AS ACTUAL_QTY               FROM FN_GET_GO_PRE_CUT_DETAIL('" + Go + "','" + MO + "','" + CHBfOSCut + "')  A      ";
            SQL = SQL + "  	               GROUP BY A.COLOR_CD   ";
            SQL = SQL + "  	               ) G    ON Z.COLOR_CD=G.COLOR_CD    ";
            SQL = SQL + "    )Y ";
            SQL = SQL + "    ON X.COLOR_CD=Y.COLOR_CD";

            tb[1] = DBUtility.GetTable(SQL, "MES");

            //tb[2]
            //Modified by Jin Song MES133 (Add Color Desc & PPO No)
            //Bug Fix by ZK on 2014-09-17 due to double order qty when more than 1 ppo is used
            SQL = " SELECT DISTINCT C.jo_no,c.color_cd,F.COLOR_DESC,c.order_qty,D.PERCENT_SHORT ,str(c.adjust_percents,10,2)+'%',c.adjust_qty,c.ACTUAL_QTY,D.SIZE_CD,D.ACTUAL_QTY  ";
            //Start modification by LIMML ON 20150701 (ADD MARKER YPD AND PPO YPD)
            //SQL = SQL + " AS PLAN_CUT_QTY,isnull(E.ORDER_QTY_BY_COL_SIZE,0) as ORDER_QTY_BY_COL_SIZE, F.PPO_NO";
            SQL = SQL + " AS PLAN_CUT_QTY,isnull(E.ORDER_QTY_BY_COL_SIZE,0) as ORDER_QTY_BY_COL_SIZE, F.PPO_NO, I.MARKER_YPD, I.PPO_YPD";
            //End modification by LIMML ON 20150701 (ADD MARKER YPD AND PPO YPD)
            SQL = SQL + " FROM  ";
            SQL = SQL + " ( ";
            SQL = SQL + " SELECT  JO_NO,COLOR_CD,SUM(ORDER_QTY) AS ORDER_QTY, ";
            SQL = SQL + " ROUND(SUM(ISNULL(ACTUAL_QTY,0)-ORDER_QTY)*100.0/SUM(ORDER_QTY),2) AS ADJUST_PERCENTS ,SUM(ISNULL(ADJUST_QTY,0)) AS ADJUST_QTY, SUM(ACTUAL_QTY) AS ACTUAL_QTY  ";
            SQL = SQL + " FROM   FN_GET_GO_PRE_CUT_DETAIL('" + Go + "','" + MO + "', '" + CHBfOSCut + "')";
            SQL = SQL + "  GROUP BY JO_NO,COLOR_CD ";
            SQL = SQL + " ) C INNER JOIN  ";
            SQL = SQL + " (SELECT  JO_NO,COLOR_CD,SIZE_CD,('+'+str(MAX(PERCENT_OVER_ALLOWED),10,1)+'%/'+'-'+str(MAX(PERCENT_SHORT_ALLOWED),10,1)+'%') AS PERCENT_SHORT,SUM(ACTUAL_QTY) AS ACTUAL_QTY  ";
            SQL = SQL + "    FROM   FN_GET_GO_PRE_CUT_DETAIL('" + Go + "','" + MO + "', '" + CHBfOSCut + "') A  ";
            SQL = SQL + "  LEFT JOIN SC_LOT C WITH(NOLOCK) ON A.JO_NO=C.PO_NO WHERE  C.SC_NO='" + Go + "' "; //Added WITH(NOLOCK) by Jin Song 7/2014
            SQL = SQL + "        GROUP BY JO_NO,COLOR_CD,SIZE_CD ";
            SQL = SQL + " )D ON C.JO_NO=D.JO_NO AND C.COLOR_CD=D.COLOR_CD  ";
            SQL = SQL + " LEFT JOIN";
            SQL = SQL + " (SELECT A.JO_NO,COLOR_CODE,SIZE_CODE1+'('+SIZE_CODE2+')' AS SIZE_CODE,SUM(ISNULL(QTY,0)) AS ORDER_QTY_BY_COL_SIZE FROM JO_HD A WITH(NOLOCK)"; //Added WITH(NOLOCK) by Jin Song 7/2014
            //新添加Order Qty;
            SQL = SQL + " INNER JOIN JO_DT B WITH(NOLOCK) ON A.JO_NO = B.JO_NO "; //Added WITH(NOLOCK) by Jin Song 7/2014
            SQL = SQL + " WHERE SC_NO = '" + Go + "'";
            SQL = SQL + " GROUP BY A.JO_NO,COLOR_CODE,SIZE_CODE1+'('+SIZE_CODE2+')'";
            SQL = SQL + " )E ON C.JO_NO = E.JO_NO AND C.COLOR_CD = E.COLOR_CODE AND D.SIZE_CD = E.SIZE_CODE";
            SQL = SQL + " INNER JOIN CP_FABRIC_ITEM F WITH(NOLOCK) ON F.COLOR_CD = C.COLOR_CD AND F.GO_NO = '" + Go + "'"; //Added by Jin Song MES133
            //Start modification by LIMML ON 20150701 (ADD MARKER YPD AND PPO YPD)
            SQL = SQL + " INNER JOIN (SELECT G.COLOR_CD, G.CURRENTYPD AS MARKER_YPD, H.YPD AS PPO_YPD FROM (SELECT  REF_COLOR AS COLOR_CD,";
            SQL = SQL + " ROUND(SUM(TOTAL_LENGTH)/SUM(GMT_QTY)*12*(1+MAX(MARKER_WASTAGE)*0.01)+MAX(BINDING_YPD),2) AS CurrentYPD";
            SQL = SQL + " FROM FN_GET_MO_COLOR_YPD('" + MO + "') GROUP BY REF_COLOR ) G";
            SQL = SQL + " INNER JOIN (SELECT REF_COLOR, YPD  FROM FN_GET_GO_COLOR_YPD('" + Go + "','" + MO + "')) H";
            SQL = SQL + " ON G.COLOR_CD = H.REF_COLOR)I ON I.COLOR_CD = C.COLOR_CD";
            //End modification by LIMML ON 20150701 (ADD MARKER YPD AND PPO YPD)
            SQL = SQL + " order by C.jo_no,color_cd";
            tb[2] = DBUtility.GetTable(SQL, "MES");

            SQL = "SELECT  DISTINCT C.SIZE_CD,A.SEQ FROM CP_SIZE_SEQ A,dbo.CP_MO_HD B,CP_MO_CS_BREAKDOWN C WHERE B.GO_NO='" + Go + "'";
            SQL += " AND A.MO_NO LIKE '%" + MO + "%' AND A.MO_NO=B.MO_NO AND B.MO_NO=C.MO_NO AND A.SIZE_CD=C.SIZE_CD ORDER BY 2";
            tb[3] = DBUtility.GetTable(SQL, "MES");

            SQL = " select JOB_ORDER_NO,COLOR,[SIZE]+''+SIZE2 as size,REMARK ";
            SQL = SQL + " From dbo.PRD_JO_REMARKS A, JO_HD B ";
            SQL = SQL + " where remark_type in('CUTPLAN_PPC','CUTPLAN_CUT') ";
            SQL = SQL + " AND A.FACTORY_CD=B.FACTORY_CD ";
            SQL = SQL + " AND A.JOB_ORDER_NO=B.JO_NO ";
            SQL = SQL + " AND B.SC_NO ='" + Go + "' ";
            SQL = SQL + " ORDER BY JOB_ORDER_NO,REMARK_TYPE,COLOR,[SIZE],SIZE2 ";
            tb[4] = DBUtility.GetTable(SQL, "MES");
            return tb;
        }

    }
}