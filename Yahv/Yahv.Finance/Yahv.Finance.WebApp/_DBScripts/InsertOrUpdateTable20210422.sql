--USE [PvFinance]
--GO

--/****** Object:  Table [dbo].[MoneyOrders]    Script Date: 04/25/2021 10:22:16 ******/
--SET ANSI_NULLS ON
--GO

--SET QUOTED_IDENTIFIER ON
--GO

--SET ANSI_PADDING ON
--GO

--CREATE TABLE [dbo].[MoneyOrders](
--	[ID] [varchar](50) NOT NULL,
--	[Type] [int] NOT NULL,
--	[Name] [varchar](50) NOT NULL,
--	[Code] [varchar](50) NOT NULL,
--	[BankCode] [varchar](50) NULL,
--	[BankName] [varchar](50) NULL,
--	[BankNo] [varchar](50) NULL,
--	[Currency] [int] NOT NULL,
--	[Price] [decimal](22, 10) NOT NULL,
--	[PayerAccountID] [varchar](50) NOT NULL,
--	[PayeeAccountID] [varchar](50) NOT NULL,
--	[IsTransfer] [bit] NOT NULL,
--	[StartDate] [datetime] NOT NULL,
--	[EndDate] [datetime] NOT NULL,
--	[Nature] [int] NOT NULL,
--	[IsMoney] [bit] NOT NULL,
--	[ExchangeDate] [datetime] NULL,
--	[ExchangePrice] [decimal](22, 10) NULL,
--	[CreatorID] [varchar](50) NOT NULL,
--	[CreateDate] [datetime] NOT NULL,
--	[ModifierID] [varchar](50) NOT NULL,
--	[ModifyDate] [datetime] NOT NULL,
--	[Status] [int] NOT NULL,
-- CONSTRAINT [PK_MoneyOrders] PRIMARY KEY CLUSTERED 
--(
--	[ID] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
--) ON [PRIMARY]

--GO

--SET ANSI_PADDING OFF
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'类型 银行承兑、商业承兑' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'Type'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'Name'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'票据号码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'Code'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'银行账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'BankCode'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'开户行名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'BankName'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'开户行行号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'BankNo'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'币种' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'Currency'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汇票金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'Price'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出票人账户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'PayerAccountID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'持票人账户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'PayeeAccountID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否允许背书转让' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'IsTransfer'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出票日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'StartDate'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汇票到期日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'EndDate'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'承兑性质 电子承兑、纸质承兑' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'Nature'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否能贴现' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'IsMoney'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'兑换日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'ExchangeDate'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'兑换金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'ExchangePrice'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'CreatorID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'CreateDate'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'ModifierID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders', @level2type=N'COLUMN',@level2name=N'ModifyDate'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汇票' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MoneyOrders'
--GO



--USE [PvFinance]
--GO

--/****** Object:  Table [dbo].[Endorsements]    Script Date: 04/22/2021 14:20:31 ******/
--SET ANSI_NULLS ON
--GO

--SET QUOTED_IDENTIFIER ON
--GO

--SET ANSI_PADDING ON
--GO

--CREATE TABLE [dbo].[Endorsements](
--	[ID] [varchar](50) NOT NULL,
--	[MoneyOrderID] [varchar](50) NOT NULL,
--	[PayerAccountID] [varchar](50) NOT NULL,
--	[PayeeAccountID] [varchar](50) NOT NULL,
--	[EndorseDate] [datetime] NOT NULL,
--	[IsTransfer] [bit] NOT NULL,
--	[Summary] [nvarchar](50) NULL,
--	[CreatorID] [varchar](50) NOT NULL,
--	[CreateDate] [datetime] NOT NULL,
-- CONSTRAINT [PK_Endorsements] PRIMARY KEY CLUSTERED 
--(
--	[ID] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
--) ON [PRIMARY]

--GO

--SET ANSI_PADDING OFF
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汇票ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Endorsements', @level2type=N'COLUMN',@level2name=N'MoneyOrderID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'背书人账户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Endorsements', @level2type=N'COLUMN',@level2name=N'PayerAccountID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'被背书人账户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Endorsements', @level2type=N'COLUMN',@level2name=N'PayeeAccountID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'背书日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Endorsements', @level2type=N'COLUMN',@level2name=N'EndorseDate'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否允许背书转让' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Endorsements', @level2type=N'COLUMN',@level2name=N'IsTransfer'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Endorsements', @level2type=N'COLUMN',@level2name=N'CreatorID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Endorsements', @level2type=N'COLUMN',@level2name=N'CreateDate'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'背书转让' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Endorsements'
--GO
--USE [PvFinance]
--GO

--/****** Object:  Table [dbo].[RefundApplies]    Script Date: 05/06/2021 10:29:10 ******/
--SET ANSI_NULLS ON
--GO

--SET QUOTED_IDENTIFIER ON
--GO

--SET ANSI_PADDING ON
--GO

--CREATE TABLE [dbo].[RefundApplies](
--	[ID] [VARCHAR](50) NOT NULL,
--	[Type] [INT] NOT NULL,
--	[PayeeLeftID] [VARCHAR](50) NOT NULL,
--	[AccountCatalogID] [VARCHAR](50) NOT NULL,
--	[PayerAccountID] [VARCHAR](50) NULL,
--	[PayeeAccountID] [VARCHAR](50) NOT NULL,
--	[FlowID] [VARCHAR](50) NULL,
--	[Currency] [INT] NOT NULL,
--	[Price] [DECIMAL](22, 10) NOT NULL,
--	[Summary] [NVARCHAR](500) NULL,
--	[SenderID] [VARCHAR](50) NOT NULL,
--	[ApplierID] [VARCHAR](50) NOT NULL,
--	[ExcuterID] [VARCHAR](50) NULL,
--	[CreatorID] [VARCHAR](50) NOT NULL,
--	[CreateDate] [DATETIME] NOT NULL,
--	[ApproverID] [VARCHAR](50) NULL,
--	[Status] [INT] NOT NULL,
-- CONSTRAINT [PK_RefundApplies] PRIMARY KEY CLUSTERED 
--(
--	[ID] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
--) ON [PRIMARY]

--GO

--SET ANSI_PADDING OFF
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'承兑户、现金户' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RefundApplies', @level2type=N'COLUMN',@level2name=N'Type'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'收款ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RefundApplies', @level2type=N'COLUMN',@level2name=N'PayeeLeftID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'类型ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RefundApplies', @level2type=N'COLUMN',@level2name=N'AccountCatalogID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'付款账户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RefundApplies', @level2type=N'COLUMN',@level2name=N'PayerAccountID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'收款账户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RefundApplies', @level2type=N'COLUMN',@level2name=N'PayeeAccountID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流水ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RefundApplies', @level2type=N'COLUMN',@level2name=N'FlowID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'币种' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RefundApplies', @level2type=N'COLUMN',@level2name=N'Currency'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RefundApplies', @level2type=N'COLUMN',@level2name=N'Price'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RefundApplies', @level2type=N'COLUMN',@level2name=N'Summary'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'系统' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RefundApplies', @level2type=N'COLUMN',@level2name=N'SenderID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'申请人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RefundApplies', @level2type=N'COLUMN',@level2name=N'ApplierID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'执行人（付款人ID）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RefundApplies', @level2type=N'COLUMN',@level2name=N'ExcuterID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'制单人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RefundApplies', @level2type=N'COLUMN',@level2name=N'CreatorID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'制单时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RefundApplies', @level2type=N'COLUMN',@level2name=N'CreateDate'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'下一个审批人
-- 空：等待有权限人员审批' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RefundApplies', @level2type=N'COLUMN',@level2name=N'ApproverID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'退款申请' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RefundApplies'
--GO





--USE PvFinance
--IF NOT EXISTS ( SELECT  *
--                FROM    syscolumns
--                WHERE   id = OBJECT_ID('FlowAccounts')
--                        AND name = 'Type' )
--    BEGIN
--		 ALTER TABLE	dbo.FlowAccounts ADD  Type int NULL;
--		 ALTER TABLE	dbo.FlowAccounts ADD  MoneyOrderID VARCHAR(50) NULL;
--    END
--GO

--修改触发器
USE [PvFinance]
GO
/****** Object:  Trigger [dbo].[trg_flowAccount_update]    Script Date: 04/22/2021 16:54:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[trg_flowAccount_update] ON [dbo].[FlowAccounts]
    AFTER UPDATE
AS
    --定义变量
    DECLARE @price DECIMAL(22, 10) ,
        @price1 DECIMAL(22, 10) ,
        @accountID VARCHAR(50) ,
        @createDate DATETIME,
        @type INT,
        @id VARCHAR(50);
    SELECT  @id = deleted.ID ,
            @accountID = deleted.AccountID ,
            @price = deleted.Price ,
            @price1 = deleted.Price1,
            @createDate=deleted.CreateDate,
            @type =Deleted.Type
    FROM    deleted;

    UPDATE  dbo.FlowAccounts
    SET     Balance = ( SELECT  ISNULL(SUM(Price), 0)
                        FROM    dbo.FlowAccounts
                        WHERE   AccountID = @accountID AND Type=@type
                                AND id <= @id
                      ) ,
            Balance1 = ( SELECT ISNULL(SUM(Price1), 0)
                         FROM   dbo.FlowAccounts
                         WHERE  AccountID = @accountID AND Type=@type
                                AND id <= @id
                       )
    WHERE   ID = @id;

go

USE [PvFinance]
GO
/****** Object:  Trigger [dbo].[trg_flowAccount_insert]    Script Date: 04/22/2021 16:53:19 ******/
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
ALTER TRIGGER [dbo].[trg_flowAccount_insert] ON [dbo].[FlowAccounts]
    AFTER INSERT
AS
    --定义变量
    DECLARE @price DECIMAL(22, 10) ,
        @price1 DECIMAL(22, 10) ,
        @accountID VARCHAR(50) ,
        @type INT,
        @id VARCHAR(50);
    SELECT  @id = Inserted.ID ,
            @accountID = Inserted.AccountID ,
            @price = Inserted.Price ,
            @price1 = Inserted.Price1,
            @type=Inserted.Type
    FROM    Inserted;

    UPDATE  dbo.FlowAccounts
    SET     Balance = ( SELECT  ISNULL(SUM(Price), 0)
                        FROM    dbo.FlowAccounts
                        WHERE   AccountID = @accountID AND Type=@type
                                AND ID <= @id
                      ) ,
            Balance1 = ( SELECT ISNULL(SUM(Price1), 0)
                         FROM   dbo.FlowAccounts
                         WHERE  AccountID = @accountID AND Type=@type
                                AND ID <= @id
                       )
    WHERE   ID = @id;
go

--USE [PvFinance]
--GO

--/****** Object:  Table [dbo].[AcceptanceApplies]    Script Date: 05/20/2021 15:56:13 ******/
--SET ANSI_NULLS ON
--GO

--SET QUOTED_IDENTIFIER ON
--GO

--SET ANSI_PADDING ON
--GO

--CREATE TABLE [dbo].[AcceptanceApplies](
--	[ID] [VARCHAR](50) NOT NULL,
--	[MoneyOrderID] [VARCHAR](50) NOT NULL,
--	[Type] [INT] NOT NULL,
--	[PayerAccountID] [VARCHAR](50) NOT NULL,
--	[PayeeAccountID] [VARCHAR](50) NOT NULL,
--	[Currency] [INT] NOT NULL,
--	[Price] [DECIMAL](22, 10) NOT NULL,
--	[Summary] [NVARCHAR](500) NULL,
--	[SenderID] [VARCHAR](50) NOT NULL,
--	[ApplierID] [VARCHAR](50) NOT NULL,
--	[ExcuterID] [VARCHAR](50) NULL,
--	[CreatorID] [VARCHAR](50) NOT NULL,
--	[CreateDate] [DATETIME] NOT NULL,
--	[ApproverID] [VARCHAR](50) NULL,
--	[Status] [INT] NOT NULL,
-- CONSTRAINT [PK_AcceptanceApplies] PRIMARY KEY CLUSTERED 
--(
--	[ID] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
--) ON [PRIMARY]

--GO

--SET ANSI_PADDING OFF
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汇票ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceApplies', @level2type=N'COLUMN',@level2name=N'MoneyOrderID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'类型 贴现、背书转让、到期承兑' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceApplies', @level2type=N'COLUMN',@level2name=N'Type'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'付款账户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceApplies', @level2type=N'COLUMN',@level2name=N'PayerAccountID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'收款账户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceApplies', @level2type=N'COLUMN',@level2name=N'PayeeAccountID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'币种' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceApplies', @level2type=N'COLUMN',@level2name=N'Currency'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceApplies', @level2type=N'COLUMN',@level2name=N'Summary'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'系统' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceApplies', @level2type=N'COLUMN',@level2name=N'SenderID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'申请人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceApplies', @level2type=N'COLUMN',@level2name=N'ApplierID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceApplies', @level2type=N'COLUMN',@level2name=N'CreatorID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'承兑调拨申请' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceApplies'
--GO


--USE [PvFinance]
--GO

--/****** Object:  Table [dbo].[AcceptanceLefts]    Script Date: 05/20/2021 15:56:17 ******/
--SET ANSI_NULLS ON
--GO

--SET QUOTED_IDENTIFIER ON
--GO

--SET ANSI_PADDING ON
--GO

--CREATE TABLE [dbo].[AcceptanceLefts](
--	[ID] [varchar](50) NOT NULL,
--	[ApplyID] [varchar](50) NOT NULL,
--	[PayerAccountID] [varchar](50) NOT NULL,
--	[PayeeAccountID] [varchar](50) NOT NULL,
--	[AccountMethord] [int] NOT NULL,
--	[Currency] [int] NOT NULL,
--	[Price] [decimal](22, 10) NOT NULL,
--	[CreatorID] [varchar](50) NOT NULL,
--	[CreateDate] [datetime] NOT NULL,
--	[Status] [int] NOT NULL,
-- CONSTRAINT [PK_AcceptanceLefts] PRIMARY KEY CLUSTERED 
--(
--	[ID] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
--) ON [PRIMARY]

--GO

--SET ANSI_PADDING OFF
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'申请ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceLefts', @level2type=N'COLUMN',@level2name=N'ApplyID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'付款账户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceLefts', @level2type=N'COLUMN',@level2name=N'PayerAccountID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'收款账户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceLefts', @level2type=N'COLUMN',@level2name=N'PayeeAccountID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceLefts', @level2type=N'COLUMN',@level2name=N'AccountMethord'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'币种' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceLefts', @level2type=N'COLUMN',@level2name=N'Currency'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceLefts', @level2type=N'COLUMN',@level2name=N'Price'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceLefts', @level2type=N'COLUMN',@level2name=N'CreatorID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceLefts', @level2type=N'COLUMN',@level2name=N'CreateDate'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceLefts', @level2type=N'COLUMN',@level2name=N'Status'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'承兑调拨左表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceLefts'
--GO

--USE [PvFinance]
--GO

--/****** Object:  Table [dbo].[AcceptanceRights]    Script Date: 05/20/2021 15:56:28 ******/
--SET ANSI_NULLS ON
--GO

--SET QUOTED_IDENTIFIER ON
--GO

--SET ANSI_PADDING ON
--GO

--CREATE TABLE [dbo].[AcceptanceRights](
--	[ID] [varchar](50) NOT NULL,
--	[AcceptanceLeftID] [varchar](50) NOT NULL,
--	[Price] [decimal](22, 10) NOT NULL,
--	[FlowID] [varchar](50) NOT NULL,
--	[CreatorID] [varchar](50) NOT NULL,
--	[CreateDate] [datetime] NOT NULL,
-- CONSTRAINT [PK_AcceptanceRights] PRIMARY KEY CLUSTERED 
--(
--	[ID] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
--) ON [PRIMARY]

--GO

--SET ANSI_PADDING OFF
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'左表ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceRights', @level2type=N'COLUMN',@level2name=N'AcceptanceLeftID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceRights', @level2type=N'COLUMN',@level2name=N'Price'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流水表ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceRights', @level2type=N'COLUMN',@level2name=N'FlowID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceRights', @level2type=N'COLUMN',@level2name=N'CreatorID'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceRights', @level2type=N'COLUMN',@level2name=N'CreateDate'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'承兑调拨右表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AcceptanceRights'
--GO

