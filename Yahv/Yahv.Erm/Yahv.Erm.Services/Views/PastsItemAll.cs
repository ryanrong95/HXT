using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Erm.Services.Views
{
    /// <summary>
    /// 获取过去累计值
    /// </summary>
    public class PastsItemAll : UniqueView<PastsItem, PvbErmReponsitory>
    {
        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<PastsItem> GetIQueryable()
        {
            return from p in this.Reponsitory.ReadTable<Pasts_WageItem>()
                   select new PastsItem
                   {
                       ID = p.ID,
                       Accumulative = p.Accumulative,
                       Currency = (Currency)p.Currency,
                       DateIndex = p.DateIndex,
                       EnterpriseID = p.EnterpriseID,
                       StaffID = p.StaffID,
                       Type = (WageItemType)p.Type,
                       Value = p.Value,
                       WorkCityID = p.WorkCityID
                   };
        }

        /// <summary>
        /// 根据payitems重新生成历史累计
        /// </summary>
        /// <param name="dateIndex"></param>
        public void Rebuild(string dateIndex)
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                #region sql
                string sql = @"
                DECLARE @nextDate VARCHAR(50) ,				--下个月工资月
                    @preDate VARCHAR(50) ,		--上一个月
                    @dateIndex VARCHAR(50) ,
                    @itemsName VARCHAR(50) ,
                    @result INT,
                    @admin VARCHAR(50);
	
				SET @dateIndex = {0}
                
                SET @nextDate = SUBSTRING(REPLACE(DATEADD(MONTH, 1,CONVERT(DATE, @dateIndex + '01')),'-', ''), 0, 7);  
                SET @preDate = SUBSTRING(REPLACE(DATEADD(MONTH, -1,CONVERT(DATE, @dateIndex + '01')),'-', ''), 0, 7); 
                
                --开启事务
                BEGIN TRAN;
                
                IF EXISTS(SELECT * FROM dbo.Pasts_WageItem WHERE DateIndex=@dateIndex)
                BEGIN
                	DELETE FROM dbo.Pasts_WageItem WHERE DateIndex=@dateIndex
                END
                

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

                repository.Query<string>(sql, dateIndex);
            }
        }
    }
}
