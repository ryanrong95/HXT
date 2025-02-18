--查询芯达通流水
SELECT Type,f.FeeType,f.SeqNo,f.PaymentType,a.Currency,f.Amount,fp.PayDate,f.CreateDate,
f.* FROM foricScCustoms.dbo.FinanceAccountFlows f
JOIN foricScCustoms.dbo.FinanceAccounts a ON f.FinanceAccountID=a.ID
LEFT JOIN foricScCustoms.dbo.FinancePayments fp ON f.SourceID=fp.ID
WHERE a.BankAccount='764071904447'
AND f.CreateDate>='2021-03-24'
ORDER BY f.CreateDate


--查询中心流水
SELECT f.* FROM PvFinance.dbo.FlowAccounts f
LEFT JOIN PvFinance.dbo.Accounts a ON f.AccountID=a.ID
WHERE a.Code='016478000927794'
ORDER BY f.CreateDate

-------------------------------芯达通、中心 账户比对
SELECT  AccountName ,
        BankAccount ,
        xdt.Balance ,
        center.Balance ,
        ( xdt.Balance - ISNULL(center.Balance, 0) ) Diffent
FROM    foricScCustoms.dbo.FinanceAccounts xdt
        LEFT JOIN ( SELECT  a.ShortName ,
                            a.Code ,
                            SUM(f.Price) Balance
                    FROM    PvFinance.dbo.FlowAccounts f
                            LEFT JOIN PvFinance.dbo.Accounts a ON f.AccountID = a.ID
                    GROUP BY a.Code ,
                            a.ShortName
                  ) center ON xdt.BankAccount = center.Code
WHERE   xdt.Balance <> 0
        AND xdt.BankAccount NOT IN ( '124-247750-838', '000344347',
                                     '000465298', '0002346578',
                                     '016478000502260', '01259120044968' );

--利用触发器 重新计算几日以后的余额
DECLARE @code VARCHAR(50),
		@id VARCHAR(50)

SET @code='41-034100040029140'	--银行卡号

DECLARE id_cursor CURSOR
FOR
    SELECT  f.ID
    FROM    dbo.FlowAccounts f
            LEFT JOIN dbo.Accounts a ON f.AccountID = a.ID
    WHERE   a.Code = @code AND f.CreateDate>='2021-03-30';
OPEN id_cursor
FETCH NEXT FROM id_cursor INTO @id
WHILE @@FETCH_STATUS=0
BEGIN
	UPDATE dbo.FlowAccounts SET Balance=0 WHERE id=@id
	PRINT @id
	FETCH NEXT FROM id_cursor INTO @id
END
CLOSE id_cursor
DEALLOCATE id_cursor


--流水比对查询
SELECT * FROM dbo.XdtFlowAccounts_Temp xdt 
LEFT JOIN dbo.FlowAccounts_Temp c ON xdt.BankAccount=c.Code  AND xdt.SeqNo=c.FormCode
WHERE xdt.CreateDate>='2021-03-30' AND xdt.BankAccount='41-034100040029140'

--芯达通接口日志
SELECT * FROM dbo.Logs
WHERE CreateDate>'2021-05-21 14:02' AND CreateDate<'2021-05-21 14:05'
ORDER BY CreateDate

--芯达通接口长日志
SELECT ID, CAST('<![CDATA[' + Json + ']]>' AS XML) 
FROM dbo.Logs
WHERE ID='Log202106070000000043'

--补录数据，不太好
/*
INSERT  INTO dbo.FlowAccounts
        ( ID ,
          AccountMethord ,
          AccountID ,
          Currency ,
          Price ,
          Balance ,
          PaymentDate ,
          FormCode ,
          Currency1 ,
          ERate1 ,
          Price1 ,
          Balance1 ,
          CreatorID ,
          CreateDate ,
          PaymentMethord
        )
        SELECT  f.ID ,
                1 ,
                'Account00313' ,
                2 ,
                f.Amount ,
                0,
                f.CreateDate ,
                f.SeqNo ,
                1 ,
                6.5574 Erate1 ,
                f.Amount * 6.5574 ,
                0,
                'Npc-Robot' ,
                f.CreateDate,1
        FROM    foricScCustoms.dbo.FinanceAccountFlows f
                JOIN foricScCustoms.dbo.FinanceAccounts a ON f.FinanceAccountID = a.ID
        WHERE   a.BankAccount = '016478000927794'
                AND f.CreateDate >= '2021-03-24'
                AND Type <> 1
*/

USE [PvFinance]
GO

/****** Object:  Trigger [dbo].[trg_flowAccount_update]    Script Date: 03/30/2021 09:35:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/* 重新计算新作数据的时候，需要修改一下触发器

ALTER TRIGGER [dbo].[trg_flowAccount_update] ON [dbo].[FlowAccounts]
    AFTER UPDATE
AS
    --定义变量
    DECLARE @price DECIMAL(22, 10) ,
        @price1 DECIMAL(22, 10) ,
        @accountID VARCHAR(50) ,
        @createDate DATETIME,
        @id VARCHAR(50);
    SELECT  @id = deleted.ID ,
            @accountID = deleted.AccountID ,
            @price = deleted.Price ,
            @price1 = deleted.Price1,
            @createDate=deleted.CreateDate
    FROM    deleted;

    UPDATE  dbo.FlowAccounts
    SET     Balance = ( SELECT  ISNULL(SUM(Price), 0)
                        FROM    dbo.FlowAccounts
                        WHERE   AccountID = @accountID
                                AND CreateDate <= @createDate
                      ) ,
            Balance1 = ( SELECT ISNULL(SUM(Price1), 0)
                         FROM   dbo.FlowAccounts
                         WHERE  AccountID = @accountID
                                AND CreateDate <= @createDate
                       )
    WHERE   ID = @id;





GO
*/

--删除付款
DECLARE @flowId VARCHAR(50) ,
    @leftId VARCHAR(50) ,
    @applyId VARCHAR(50);
    
SET @flowId = 'FlowAcc12563';
    
SELECT  @leftId = PayerLeftID
FROM    dbo.PayerRights
WHERE   FlowID = @flowId;

SELECT  @applyId = ApplyID
FROM    dbo.PayerLefts
WHERE   ID = @leftId;

IF ( @leftId IS NULL
     OR @applyId IS NULL
   )
    BEGIN
        RETURN;
    END;

PRINT @flowId + '-' + @leftId + '-' + @applyId;

BEGIN TRAN;
BEGIN TRY
    DELETE  FROM dbo.PayerRights
    WHERE   FlowID = @flowId;
    
    DELETE  FROM dbo.FlowAccounts
    WHERE   ID = @flowId;
    
    DELETE  FROM dbo.PayerLefts
    WHERE   ID = @leftId;
    
    DELETE  FROM dbo.PayerApplies
    WHERE   ID = @applyId;
END TRY
BEGIN CATCH
    SELECT  ERROR_NUMBER() AS ErrorNumber ,  --错误代码
            ERROR_SEVERITY() AS ErrorSeverity ,  --错误严重级别，级别小于10 try catch 捕获不到
            ERROR_STATE() AS ErrorState ,  --错误状态码
            ERROR_PROCEDURE() AS ErrorProcedure , --出现错误的存储过程或触发器的名称。
            ERROR_LINE() AS ErrorLine ,  --发生错误的行号
            ERROR_MESSAGE() AS ErrorMessage;  --错误的具体信息

    IF ( @@trancount > 0 )
        ROLLBACK TRAN;
END CATCH;

IF ( @@TRANCOUNT > 0 )
    COMMIT TRAN;

--删除费用
DECLARE @flowId VARCHAR(50) ,
    @applyId VARCHAR(50);
    
SET @flowId = 'FlowAcc12563';
    
SELECT  @applyId = ApplyID
FROM    dbo.ChargeApplyItems
WHERE   FlowID = @flowId;
BEGIN TRAN;
BEGIN TRY
    DELETE  FROM dbo.ChargeApplyItems
    WHERE   FlowID = @flowId;

    DELETE  FROM dbo.FlowAccounts
    WHERE   ID = @flowId;
       
    DELETE  FROM dbo.ChargeApplies
    WHERE   ID = @applyId;
END TRY
BEGIN CATCH
    SELECT  ERROR_NUMBER() AS ErrorNumber ,  --错误代码
            ERROR_SEVERITY() AS ErrorSeverity ,  --错误严重级别，级别小于10 try catch 捕获不到
            ERROR_STATE() AS ErrorState ,  --错误状态码
            ERROR_PROCEDURE() AS ErrorProcedure , --出现错误的存储过程或触发器的名称。
            ERROR_LINE() AS ErrorLine ,  --发生错误的行号
            ERROR_MESSAGE() AS ErrorMessage;  --错误的具体信息

    IF ( @@trancount > 0 )
        ROLLBACK TRAN;
END CATCH;

IF ( @@TRANCOUNT > 0 )
    COMMIT TRAN;