using System;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 员工月账单
    /// </summary>
    public class PayBillsRoll : UniqueView<PayBill, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PayBillsRoll()
        {
        }

        protected override IQueryable<PayBill> GetIQueryable()
        {
            return new Origins.PayBillsOrigin();
        }

        /// <summary>
        /// 封账
        /// </summary>
        /// <param name="dateIndex">封账日期</param>
        /// <param name="items">清空项</param>
        /// <param name="adminId">adminId</param>
        public void Closed(string dateIndex, string items, string adminId)
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                string sql = @"EXEC	[dbo].[P_Closed]
		                            @dateIndex={0},
                                    @itemsName={1},
                                    @admin = {2}";
                repository.Query<string>(sql, dateIndex, items, adminId);
            }
        }

        public void Close(string dateIndex, string items, string adminId)
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                #region sql
                string sql = @"
                DECLARE @nextDate VARCHAR(50) ,				--下个月工资月
                    @preDate VARCHAR(50) ,		--上一个月
                    @dateIndex VARCHAR(50) ,
                    @itemsName NVARCHAR(500) ,
                    @result INT,
                    @admin VARCHAR(50);
	
				SET @dateIndex = {0}
                SET @itemsName = {1}
                SET @admin = {2}
                
                SET @nextDate = SUBSTRING(REPLACE(DATEADD(MONTH, 1,CONVERT(DATE, @dateIndex + '01')),'-', ''), 0, 7);  
                SET @preDate = SUBSTRING(REPLACE(DATEADD(MONTH, -1,CONVERT(DATE, @dateIndex + '01')),'-', ''), 0, 7); 
                
                
                PRINT @dateIndex
                PRINT @nextDate
                PRINT @preDate
                
                --开启事务
                BEGIN TRAN;
                --封账
                UPDATE  dbo.PayBills SET Status = 2 WHERE DateIndex = @dateIndex;

                --如果下个月有数据直接退出
                IF EXISTS (SELECT * FROM dbo.PayBills WHERE DateIndex = @nextDate )
                    BEGIN
                        COMMIT TRAN;
                        GOTO thenext;
                    END;

                --根据这个月生成下个月数据
                INSERT  INTO dbo.PayBills (ID,StaffID,DateIndex ,CreaetDate,EnterpriseID,Status,PostionID)
                        SELECT  REPLACE(StaffID, 'Staff', '') + '-' + @nextDate,
                                StaffID ,
                                @nextDate ,
                                GETDATE() ,
                                pb.EnterpriseID ,
                                1,
                                s.PostionID
                        FROM    dbo.PayBills pb
                                LEFT JOIN dbo.Staffs s ON pb.StaffID = s.ID
                        WHERE   DateIndex = @dateIndex
                                AND (s.Status = 200 OR s.Status=102 OR s.Status=300);

                IF ( @@ERROR <> 0 )
                    BEGIN
                        ROLLBACK TRAN;
                        SET @result= -1;
                        GOTO thenext;
                    END;
   
                        
                INSERT  INTO dbo.PayItems (ID,PayID,Name,Value,DateIndex,WageItemFormula,ActualFormula,Description,CreateDate,UpdateDate,AdminID)
                        SELECT  UPPER(SUBSTRING(sys.fn_sqlvarbasetostr(HASHBYTES('MD5',SUBSTRING(PayID,0,CHARINDEX('-',PayID) + 1)+ CONVERT(VARCHAR(50), @nextDate)+ CONVERT(VARCHAR(50), pi.Name))),3, 32)),
                                SUBSTRING(PayID, 0, CHARINDEX('-', PayID) + 1) + @nextDate ,
                                pi.Name ,
                                Value ,
                                @nextDate ,
                                WageItemFormula ,
                                ActualFormula ,
                                Description ,
                                GETDATE() ,
                                GETDATE() ,
                                @admin
                        FROM    dbo.PayItems pi
                                LEFT JOIN dbo.PayBills pb ON pi.PayID = pb.ID
                                LEFT JOIN dbo.Staffs s ON pb.StaffID = s.ID
                        WHERE   pi.DateIndex = @dateIndex
                                AND (s.Status = 200 OR s.Status=102 OR s.Status=300);

                IF ( @@ERROR <> 0 )
                    BEGIN
                        ROLLBACK TRAN;
                        SET @result= -1;
                        GOTO thenext;
                    END;

                --根据itemsName将数据归0
                IF @itemsName IS NOT NULL
                    AND LEN(@itemsName) > 0
                    BEGIN
                        UPDATE  dbo.PayItems
                        SET     Value = 0
                        WHERE   DateIndex = @nextDate
                                AND Name IN ( SELECT    items
                                              FROM      dbo.splitl(@itemsName, ',') );
                                  
                        IF ( @@ERROR <> 0 )
						BEGIN
							ROLLBACK TRAN;
							SET @result= -1;
							GOTO thenext;
						END;
                    END;


                --将计算列清空
                UPDATE  dbo.PayItems
                SET     Value = 0
                WHERE   Name IN ( SELECT    Name
                                  FROM      dbo.WageItems
                                  WHERE     Type <> 1 )
                        AND DateIndex = @nextDate;


				IF ( @@ERROR <> 0 )
                    BEGIN
                        ROLLBACK TRAN;
                        SET @result= -1;
                        GOTO thenext;
                    END;


                DECLARE @workCityId VARCHAR(50) ,	--所属城市
                    @enterpriseId VARCHAR(50) ,	--所属公司
                    @staffId VARCHAR(50) ,		--员工ID
                    @wageItemType INT ,			--工资项类型
                    @itemName NVARCHAR(50) ,		--工资项名称
                    @itemValue DECIMAL(18, 5) ,			--工资值
                    @accum INT;
					                --累计次数

                DECLARE data_cursor CURSOR
                FOR
                    SELECT  s.WorkCity ,
                            pb.EnterpriseID ,
                            pb.StaffID ,
                            wi.Type ,
                            pi.Name ,
                            pi.Value ,
                            ( ISNULL(( SELECT TOP 1
                                                Accumulative
                                       FROM     dbo.Pasts_WageItem
                                       WHERE    DateIndex = @preDate
                                                AND EnterpriseID = pb.EnterpriseID
                                                AND StaffID = pb.StaffID
                                                AND Type = wi.Type
                                                AND LEFT(DateIndex, 4) = LEFT(@nextDate, 4)
                                     ), 0) + 1 ) Accum
                    FROM    dbo.PayItems pi
                            LEFT JOIN dbo.PayBills pb ON pi.PayID = pb.ID
                            LEFT JOIN dbo.WageItems wi ON pi.Name = wi.Name
                            LEFT JOIN dbo.Staffs s ON pb.StaffID = s.ID
                    WHERE   wi.Type >= 10
                            AND pi.DateIndex = @dateIndex;

                OPEN data_cursor;
                FETCH NEXT FROM data_cursor INTO @workCityId, @enterpriseId, @staffId,
                    @wageItemType, @itemName, @itemValue, @accum;
                WHILE @@FETCH_STATUS = 0
                    BEGIN
                        INSERT  INTO dbo.Pasts_WageItem
                                ( ID ,
                                  DateIndex ,
                                  WorkCityID ,
                                  EnterpriseID ,
                                  StaffID ,
                                  Type ,
                                  Name ,
                                  Value ,
                                  Currency ,
                                  Accumulative
	                            )
                        VALUES  ( UPPER(SUBSTRING(sys.fn_sqlvarbasetostr(HASHBYTES('MD5',
                                                                              CONVERT(VARCHAR(50), @dateIndex)
                                                                              + CONVERT(VARCHAR(50), @staffId)
                                                                              + CONVERT(VARCHAR(50), @wageItemType))),
                                                  3, 32)) ,
                                  @dateIndex , -- DateIndex - int
                                  @workCityId , -- WorkCityID - varchar(50)
                                  @enterpriseId , -- EnterpriseID - varchar(50)
                                  @staffId , -- StaffID - varchar(50)
                                  @wageItemType , -- Type - int
                                  @itemName ,
                                  @itemValue , -- Value - decimal
                                  1 , -- Currency - int		默认人民币
                                  @accum  -- Accumulative - int
	                            );
	                
	                
					IF ( @@ERROR <> 0 )
                    BEGIN
                        ROLLBACK TRAN;
                        SET @result= -1;
                        GOTO thenext;
                    END;
	                
	                
                        FETCH NEXT FROM data_cursor INTO @workCityId, @enterpriseId, @staffId,
                            @wageItemType, @itemName, @itemValue, @accum;
                    END;
                CLOSE data_cursor;
                DEALLOCATE data_cursor;
    
                IF ( @@ERROR <> 0 )
                    BEGIN
                        SET @result=-1;
                        ROLLBACK TRAN;
                    END;
                ELSE
                    BEGIN
                        SET @result=0;
                        COMMIT TRAN;
                    END;
                    
                    thenext:
                    SELECT @result";
                #endregion

                repository.Query<string>(sql, dateIndex, items, adminId);
            }
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="dateIndex"></param>
        /// <param name="status"></param>
        public void UpdateStatus(string dateIndex, PayBillStatus status)
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Update<PayBills>(new
                {
                    Status = (int)status
                }, item => item.DateIndex == dateIndex);
            }
        }

        /// <summary>
        /// 清空本月所有数据
        /// </summary>
        public void DeleteAll(string dateIndex)
        {
            if (string.IsNullOrWhiteSpace(dateIndex)) return;

            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Command($"delete from PayItems where dateIndex='{dateIndex}'");
                repository.Command($"delete from PayBills where dateIndex='{dateIndex}'");
            }
        }

        /// <summary>
        /// 清空本月所有数据
        /// </summary>
        public void DeleteByPayIds(string payIds)
        {
            if (string.IsNullOrWhiteSpace(payIds)) return;

            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                foreach (var payId in payIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    repository.Command($"delete from PayItems where PayID='{payId}'");
                    repository.Command($"delete from PayBills where ID='{payId}'");
                }
            }
        }

        /// <summary>
        /// 获取工资月状态
        /// </summary>
        /// <param name="dateIndex"></param>
        /// <returns></returns>
        public PayBillStatus GetStatus(string dateIndex)
        {
            var model = this.Reponsitory.ReadTable<PayBills>().FirstOrDefault(t => t.DateIndex == dateIndex);
            if (model == null)
            {
                return PayBillStatus.Check;
            }

            return (PayBillStatus)model.Status;
        }
    }
}