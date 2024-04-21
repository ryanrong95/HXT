USE PvbCrm
go
IF not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Vouchers]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Vouchers] (
    [ID]            VARCHAR (50) NOT NULL,
    [OrderID]       VARCHAR (50) NULL,
    [DateIndex]     INT          NULL,
    [ApplicationID] VARCHAR (50) NULL,
    [Type]          INT          NOT NULL,
    [Currency]      INT          NOT NULL,
    [Payer]         VARCHAR (50) NULL,
    [Payee]         VARCHAR (50) NOT NULL,
    [IsSettlement]  BIT          NULL,
    [CreateDate]    DATETIME     NOT NULL,
    [CreatorID]     VARCHAR (50) NOT NULL,
    [Status]        INT          NOT NULL,
    CONSTRAINT [PK_FinancialNotices] PRIMARY KEY CLUSTERED ([ID] ASC)
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付汇使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'ApplicationID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建人', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'CreatorID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'币种   发生币种', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'Currency';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'期号，如果有期号就代表是action 是还款', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'DateIndex';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否结算其他（暂时只是仓储费）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'IsSettlement';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'订单ID，如果有订单就代表订单确认支付', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'OrderID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'收款公司', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'Payee';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款公司', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'Payer';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'状态（待确认、已确认、废除（关闭））', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'Status';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'收款确认、出纳确认、还款确认（针对客户信用）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'Type';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'财务通知单', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers';
END


go
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FlowAccounts]') AND type IN (N'U'))
BEGIN
DROP TABLE [dbo].[FlowAccounts]

CREATE TABLE [dbo].[FlowAccounts] (
    [ID]           VARCHAR (50)    NOT NULL,
    [Type]         INT             NOT NULL,
    [Payer]        VARCHAR (50)    NULL,
    [Payee]        VARCHAR (50)    NULL,
    [Business]     NVARCHAR (50)   NULL,
    [Catalog]      NVARCHAR (50)   NULL,
    [Subject]      NVARCHAR (50)   NULL,
    [Currency]     INT             NOT NULL,
    [Price]        DECIMAL (18, 7) NOT NULL,
    [Currency1]    INT             NOT NULL,
    [Price1]       DECIMAL (18, 7) NOT NULL,
    [ERate1]       DECIMAL (18, 7) NULL,
    [Bank]         NVARCHAR (150)  NULL,
    [Account]      VARCHAR (50)    NULL,
    [FormCode]     VARCHAR (50)    NULL,
    [OrderID]      VARCHAR (50)    NULL,
    [WaybillID]    VARCHAR (50)    NULL,
    [AdminID]      VARCHAR (50)    NOT NULL,
    [CreateDate]   DATETIME        NOT NULL,
    [OriginIndex]  INT             NULL,
    [ChangeIndex]  INT             NULL,
    [OriginalDate] DATETIME        NULL,
    [ChangeDate]   DATETIME        NULL,
    [DateIndex]    INT             NULL,
    [ReceiptDate]  DATETIME        NULL,
    [VoucherID]    VARCHAR (50)    NULL,
    CONSTRAINT [PK_Balances] PRIMARY KEY CLUSTERED ([ID] ASC)
);

CREATE NONCLUSTERED INDEX [Index_Main]
    ON [dbo].[FlowAccounts]([Type] ASC, [Payer] ASC, [Payee] ASC, [Business] ASC, [Catalog] ASC, [Subject] ASC, [Currency] ASC, [Price] ASC);

CREATE NONCLUSTERED INDEX [Index_Overdue1]
    ON [dbo].[FlowAccounts]([Type] ASC, [Payer] ASC, [Payee] ASC, [Business] ASC, [Catalog] ASC, [Subject] ASC, [Currency] ASC, [Price] ASC, [CreateDate] ASC);

CREATE NONCLUSTERED INDEX [Index_Overdue2]
    ON [dbo].[FlowAccounts]([Type] ASC, [Payer] ASC, [Payee] ASC, [Business] ASC, [Catalog] ASC, [Subject] ASC, [Currency] ASC, [Price] ASC, [ChangeDate] ASC);

CREATE NONCLUSTERED INDEX [Index_Repays]
    ON [dbo].[FlowAccounts]([DateIndex] ASC, [Business] ASC, [Type] ASC, [Payer] ASC, [Payee] ASC)
    INCLUDE([ID], [Catalog], [Subject], [Currency], [Price], [Currency1], [Price1], [ERate1], [Bank], [Account], [FormCode], [OrderID], [WaybillID], [AdminID], [CreateDate], [OriginIndex], [ChangeIndex], [OriginalDate], [ChangeDate]);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'银行账号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'Account';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'业务', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'Business';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'分类', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'Catalog';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'结转日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'ChangeDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'结转期号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'ChangeIndex';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'发生时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'币种', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'Currency';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'币种（人民币）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'Currency1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'还款期号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'DateIndex';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'某对人民币汇率', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'ERate1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'银行、现金收付款手续的流水号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'FormCode';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'唯一码，建议：时间pkey方式', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'还款日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'OriginalDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'发生期号 （yyyyMM）datetime.Now.add()', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'OriginIndex';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'收款日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'ReceiptDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'科目', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'Subject';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'信用、信用花费、现金、信用总账、减免账户、优惠券账户、退款账户、银行流水账户', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'Type';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'账单ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'VoucherID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'运单ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'WaybillID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流水账', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts';
END

go
IF not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Coupons]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Coupons] (
    [ID]           VARCHAR (50)    NOT NULL,
    [Name]         NVARCHAR (50)   NOT NULL,
    [Code]         VARCHAR (50)    NOT NULL,
    [Type]         INT             NOT NULL,
    [Conduct]      NVARCHAR (50)   NOT NULL,
    [Catalog]      NVARCHAR (50)   NULL,
    [Subject]      NVARCHAR (50)   NULL,
    [Currency]     INT             NOT NULL,
    [Price]        DECIMAL (18, 7) NULL,
    [InOrderCount] INT             NULL,
    [CreateDate]   DATETIME        NOT NULL,
    [CreatorID]    VARCHAR (50)    NOT NULL,
    [Status]       INT             NOT NULL,
    CONSTRAINT [PK_Coupons] PRIMARY KEY CLUSTERED ([ID] ASC)
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'分类：
货款  
税款  
代理费  
杂费 
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Catalog';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'优惠券编码', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Code';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'业务： 

代仓储   
代报关   
代收货   
代发货   
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Conduct';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Npc Admin  ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'CreatorID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'币种默认为：CNY，未来可能会有 USD CNY HKD', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Currency';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Pick生成', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用做约定或是学习，暂时不做优惠券使用的限制', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'InOrderCount';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'优惠券名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Name';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'价值', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Price';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'状态： Nornal \ Closed', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Status';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'科目：
停车费 
登记费 
特殊手续的收费  
分拣费  
贴签费  
包装费  
代付货款  
货款  
等  
其他手段：免项、折扣（折掉钱数）、免去尾数
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Subject';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'优惠券类型：定额、据实', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Type';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'优惠券', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons';
END

go
IF not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FlowCoupons]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[FlowCoupons] (
    [ID]         VARCHAR (50)   NOT NULL,
    [Payer]      VARCHAR (50)   NOT NULL,
    [Payee]      VARCHAR (50)   NOT NULL,
    [CouponID]   VARCHAR (50)   NOT NULL,
    [Input]      INT            NOT NULL,
    [Output]     INT            NOT NULL,
    [Balance]    INT            NOT NULL,
    [CreateDate] DATETIME       NOT NULL,
    [AdminID]    VARCHAR (50)   NULL,
    [UserID]     VARCHAR (50)   NULL,
    [Summary]    NVARCHAR (200) NULL,
    CONSTRAINT [PK_CouponForms] PRIMARY KEY CLUSTERED ([ID] ASC)
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'余 （收 - 出）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons', @level2type = N'COLUMN', @level2name = N'Balance';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'优惠券ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons', @level2type = N'COLUMN', @level2name = N'CouponID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'收', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons', @level2type = N'COLUMN', @level2name = N'Input';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'支', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons', @level2type = N'COLUMN', @level2name = N'Output';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'EnterpriseID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons', @level2type = N'COLUMN', @level2name = N'Payee';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'EnterpriseID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons', @level2type = N'COLUMN', @level2name = N'Payer';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'摘要，主要管理员记录', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons', @level2type = N'COLUMN', @level2name = N'Summary';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者(网站客户使用的)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons', @level2type = N'COLUMN', @level2name = N'UserID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'优惠券流水', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons';
END

go
IF not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Logs_DebtTerms]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Logs_DebtTerms] (
    [ID]             VARCHAR (50)  NOT NULL,
    [OldID]          VARCHAR (50)  NOT NULL,
    [Payer]          VARCHAR (50)  NOT NULL,
    [Payee]          VARCHAR (50)  NOT NULL,
    [Business]       NVARCHAR (50) NOT NULL,
    [Catalog]        NVARCHAR (50) NOT NULL,
    [SettlementType] INT           NOT NULL,
    [Months]         INT           NOT NULL,
    [Days]           INT           NOT NULL,
    [OriginDate]     DATETIME      NOT NULL,
    [AdminID]        VARCHAR (50)  NOT NULL,
    [CreateDate]     DATETIME      NOT NULL,
    [ERateType]      INT           NULL,
    CONSTRAINT [PK_Logs_DebtTerms] PRIMARY KEY CLUSTERED ([ID] ASC)
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作人，只能后台操作', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'AdminID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'业务', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'Business';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'分类', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'Catalog';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日志日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'天数', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'Days';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'汇率类型 海关汇率、时时汇率（默认汇率）、约定汇率', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'ERateType';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'唯一码：按照  PK  dt前缀方式生成', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'月份', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'Months';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'DebtTermID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'OldID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'原始时期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'OriginDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'结算方式：约定期限、月结、开票后', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'SettlementType';
END

go
IF not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MapsCoperation]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[MapsCoperation] (
    [ID]     VARCHAR (50) NOT NULL,
    [Type]   INT          NOT NULL,
    [MainID] VARCHAR (50) NOT NULL,
    [SubID]  VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_MapsCoperation] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_MapsCoperation_Enterprises] FOREIGN KEY ([MainID]) REFERENCES [dbo].[Enterprises] ([ID]),
    CONSTRAINT [FK_MapsCoperation_Enterprises1] FOREIGN KEY ([SubID]) REFERENCES [dbo].[Enterprises] ([ID])
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ID = MD5(其他的字段)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MapsCoperation', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'合作方（被设置）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MapsCoperation', @level2type = N'COLUMN', @level2name = N'MainID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'合作方（可设置的这方 ， 我们）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MapsCoperation', @level2type = N'COLUMN', @level2name = N'SubID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'合作关系，传统贸易采购、代仓储、代报关', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MapsCoperation', @level2type = N'COLUMN', @level2name = N'Type';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'合作关系', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MapsCoperation';
END

go
IF not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MapsRegion]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[MapsRegion] (
    [AdminID] VARCHAR (50) NOT NULL,
    [MainID]  VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_MapsRegion] PRIMARY KEY CLUSTERED ([AdminID] ASC, [MainID] ASC),
    CONSTRAINT [FK_MapsRegion_Beneficiaries] FOREIGN KEY ([MainID]) REFERENCES [dbo].[Beneficiaries] ([ID]),
    CONSTRAINT [FK_MapsRegion_Enterprises1] FOREIGN KEY ([MainID]) REFERENCES [dbo].[Enterprises] ([ID])
);

ALTER TABLE [dbo].[MapsRegion] NOCHECK CONSTRAINT [FK_MapsRegion_Beneficiaries];
ALTER TABLE [dbo].[MapsRegion] NOCHECK CONSTRAINT [FK_MapsRegion_Enterprises1];
END

go
IF not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MonthSealedBills]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[MonthSealedBills] (
    [ID]         VARCHAR (50)  NOT NULL,
    [DateIndex]  INT           NOT NULL,
    [Sealed]     INT           NOT NULL,
    [Conduct]    NVARCHAR (50) NOT NULL,
    [Payer]      VARCHAR (50)  NOT NULL,
    [CreateDate] DATETIME      NOT NULL,
    [OccurDate]  DATETIME      NOT NULL,
    [ModifyDate] DATETIME      NOT NULL,
    CONSTRAINT [PK_MonthSealedBill] PRIMARY KEY CLUSTERED ([ID] ASC)
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'业务', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MonthSealedBills', @level2type = N'COLUMN', @level2name = N'Conduct';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MonthSealedBills', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'期号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MonthSealedBills', @level2type = N'COLUMN', @level2name = N'DateIndex';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MonthSealedBills', @level2type = N'COLUMN', @level2name = N'ModifyDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'发生日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MonthSealedBills', @level2type = N'COLUMN', @level2name = N'OccurDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'客户', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MonthSealedBills', @level2type = N'COLUMN', @level2name = N'Payer';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否封账', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MonthSealedBills', @level2type = N'COLUMN', @level2name = N'Sealed';
END

go
IF not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payables]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Payables] (
    [ID]             VARCHAR (50)    NOT NULL,
    [Payee]          VARCHAR (50)    NOT NULL,
    [PayeeID]        VARCHAR (50)    NULL,
    [Payer]          VARCHAR (50)    NOT NULL,
    [PayerID]        VARCHAR (50)    NULL,
    [Business]       NVARCHAR (50)   NOT NULL,
    [Catalog]        NVARCHAR (50)   NOT NULL,
    [Subject]        NVARCHAR (50)   NULL,
    [Currency]       INT             NOT NULL,
    [Price]          DECIMAL (18, 7) NULL,
    [OrderID]        VARCHAR (50)    NULL,
    [WaybillID]      VARCHAR (50)    NULL,
    [CreateDate]     DATETIME        NOT NULL,
    [AdminID]        VARCHAR (50)    NOT NULL,
    [Summay]         VARCHAR (200)   NULL,
    [TinyID]         VARCHAR (50)    NULL,
    [ItemID]         VARCHAR (50)    NULL,
    [ApplicationID]  VARCHAR (50)    NULL,
    [Status]         INT             NOT NULL,
    [PayerAnonymous] NVARCHAR (100)  NULL,
    [PayeeAnonymous] NVARCHAR (100)  NULL,
    [VoucherID]      VARCHAR (50)    NULL,
    CONSTRAINT [PK_Payables] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Payables_Vouchers] FOREIGN KEY ([VoucherID]) REFERENCES [dbo].[Vouchers] ([ID])
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'添加人  Npc，实际的人员', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'AdminID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'申请ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'ApplicationID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'业务 
枚举化开发 Enum：
代仓储  
代报关  
代收货 
代发货  
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'Business';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'分类 
货款  
税款  
代理费  
杂费 
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'Catalog';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间  消费时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'币种 发生币种', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'Currency';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流水设计[业务-公司-科目-流水号] 流水号要包涵日期型流水号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'所属订单ID   MainID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'OrderID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'客户（公司）（ClientID）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'Payee';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'收款人匿名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'PayeeAnonymous';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'客户的收款银行及账号   国外的银行必须包含：银行代码。', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'PayeeID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款人   内部公司ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'Payer';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款人匿名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'PayerAnonymous';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款人账户   例如：芯达通的账户', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'PayerID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'金额  发生金额', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'Price';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'科目 
停车费  
登记费  
特殊手续的收费  
分拣费  
贴签费  
包装费  
代付货款  
货款  
采购材料  
采购纸箱  
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'Subject';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'Summay';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'小订单ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'TinyID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'运单ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'WaybillID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应付款项', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables';
END


go

go
IF not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Payments] (
    [ID]          VARCHAR (50)    NOT NULL,
    [PayableID]   VARCHAR (50)    NOT NULL,
    [AccountType] INT             NOT NULL,
    [Price]       DECIMAL (18, 7) NOT NULL,
    [Currency1]   INT             NOT NULL,
    [Price1]      DECIMAL (18, 7) NOT NULL,
    [Rate1]       DECIMAL (18, 7) NOT NULL,
    [CreateDate]  DATETIME        NOT NULL,
    [AdminID]     VARCHAR (50)    NOT NULL,
    [Summay]      VARCHAR (200)   NULL,
    [OrderID]     VARCHAR (50)    NULL,
    [WaybillID]   VARCHAR (50)    NULL,
    [AccountCode] VARCHAR (50)    NULL,
    [FlowID]      VARCHAR (50)    NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED ([ID] ASC)
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'出账编码', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'AccountCode';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'信用、信用花费、现金、信用总账、减免账户、优惠券账户、退款账户', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'AccountType';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'添加人  Npc，实际的人员', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'AdminID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'狭义币种  本位币币种（默认跟着发生）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'Currency1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流水表ID，花费那一条', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'FlowID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'唯一码 主键：四位年+2位月+2日+6位流水', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'所属订单ID   MainID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'OrderID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应付款项ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'PayableID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'金额  发生金额', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'Price';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'狭义金额  本位币金额 ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'Price1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'狭义汇率  本位币汇率', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'Rate1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'Summay';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'运单ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'WaybillID';
END


go
IF not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Receivables]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Receivables] (
    [ID]                 VARCHAR (50)    NOT NULL,
    [Payer]              VARCHAR (50)    NULL,
    [PayerID]            VARCHAR (50)    NULL,
    [Payee]              VARCHAR (50)    NULL,
    [PayeeID]            VARCHAR (50)    NULL,
    [Business]           NVARCHAR (50)   NOT NULL,
    [Catalog]            NVARCHAR (50)   NOT NULL,
    [Subject]            NVARCHAR (50)   NULL,
    [Currency]           INT             NOT NULL,
    [Price]              DECIMAL (18, 7) NOT NULL,
    [Quantity]           INT             NULL,
    [Currency1]          INT             NOT NULL,
    [Price1]             DECIMAL (18, 7) NOT NULL,
    [Rate1]              DECIMAL (18, 7) NOT NULL,
    [SettlementCurrency] INT             NOT NULL,
    [SettlementPrice]    DECIMAL (18, 7) NOT NULL,
    [SettlementRate]     DECIMAL (18, 7) NOT NULL,
    [CreateDate]         DATETIME        NOT NULL,
    [OriginalDate]       DATETIME        NULL,
    [ChangeDate]         DATETIME        NULL,
    [OriginalIndex]      INT             NULL,
    [ChangeIndex]        INT             NULL,
    [AdminID]            VARCHAR (50)    NOT NULL,
    [Summay]             VARCHAR (300)   NULL,
    [WaybillID]          VARCHAR (50)    NULL,
    [OrderID]            VARCHAR (50)    NULL,
    [TinyID]             VARCHAR (50)    NULL,
    [ItemID]             VARCHAR (50)    NULL,
    [ApplicationID]      VARCHAR (50)    NULL,
    [Status]             INT             NOT NULL,
    [PayerAnonymous]     NVARCHAR (100)  NULL,
    [PayeeAnonymous]     NVARCHAR (100)  NULL,
    CONSTRAINT [PK_Receivables_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);

CREATE NONCLUSTERED INDEX [Index_TinyOrderID]
    ON [dbo].[Receivables]([TinyID] ASC)
    INCLUDE([ID], [Payer], [PayerID], [Payee], [PayeeID], [Business], [Catalog], [Subject], [Currency], [Price], [Quantity], [Currency1], [Price1], [Rate1], [SettlementCurrency], [SettlementPrice], [SettlementRate], [CreateDate], [OriginalDate], [ChangeDate], [OriginalIndex], [ChangeIndex], [AdminID], [Summay], [WaybillID], [OrderID], [ItemID], [ApplicationID], [Status]);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'添加人  Npc，实际的人员', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'AdminID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'申请ID，付汇使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'ApplicationID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'业务  
枚举化开发 Enum：
代仓储   
代报关   
代收货   
代发货   
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Business';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'分类 
货款  
税款  
代理费  
杂费 
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Catalog';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'变更账期还款日  未出账时可为空', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'ChangeDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'变更账期还款期   未出账时可为空，yyyyMM', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'ChangeIndex';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间  消费时间 cost', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'币种   发生币种', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Currency';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'狭义币种  本位币币种（默认跟着发生）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Currency1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'编号  流水设计[业务-公司-科目-流水号] 流水号要包涵日期型流水号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'订单型号ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'ItemID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'所属订单ID    MainID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'OrderID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'发生账期还款日  未出账时可为空', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'OriginalDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'发生账期还款期  未出账时可为空，yyyyMM', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'OriginalIndex';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'收款人  内部公司ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Payee';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'收款人匿名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'PayeeAnonymous';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'收款人ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'PayeeID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'客户（公司）（ClientID）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Payer';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款人匿名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'PayerAnonymous';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款人ID（null 代表未知）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'PayerID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'金额  发生金额', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Price';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'狭义金额  本位币金额 ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Price1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'数量', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Quantity';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'狭义汇率  本位币汇率', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Rate1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'结算币种  用于结算时候使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'SettlementCurrency';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'结算价格  用于结算时候使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'SettlementPrice';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'结算汇率  用于结算时候使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'SettlementRate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'正常、废除', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Status';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'科目   
停车费 
登记费 
特殊手续的收费  
分拣费  
贴签费  
包装费  
代付货款  
货款  
等  
其他手段：免项、折扣（折掉钱数）、免去尾数
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Subject';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述  备注', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Summay';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'小订单ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'TinyID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'运单ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'WaybillID';
END

go
IF not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Receiveds]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Receiveds] (
    [ID]           VARCHAR (50)    NOT NULL,
    [ReceivableID] VARCHAR (50)    NOT NULL,
    [AccountType]  INT             NULL,
    [Price]        DECIMAL (18, 7) NOT NULL,
    [Currency1]    INT             NOT NULL,
    [Price1]       DECIMAL (18, 7) NOT NULL,
    [Rate1]        DECIMAL (18, 7) NOT NULL,
    [AdminID]      VARCHAR (50)    NOT NULL,
    [OrderID]      VARCHAR (50)    NULL,
    [WaybillID]    VARCHAR (50)    NULL,
    [CreateDate]   DATETIME        NOT NULL,
    [Summay]       VARCHAR (200)   NULL,
    [AccountCode]  VARCHAR (50)    NULL,
    [FlowID]       VARCHAR (50)    NULL,
    [CouponID]     VARCHAR (50)    NULL,
    CONSTRAINT [PK_Receiveds] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Receiveds_Receivables] FOREIGN KEY ([ReceivableID]) REFERENCES [dbo].[Receivables] ([ID])
);

CREATE NONCLUSTERED INDEX [Mains]
    ON [dbo].[Receiveds]([ReceivableID] ASC, [Price] ASC, [CreateDate] ASC);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'出账编码', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'AccountCode';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'信用、信用花费、现金、信用总账、减免账户、优惠券账户、退款账户', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'AccountType';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'添加人  Npc，实际的人员', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'AdminID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'优惠券ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'CouponID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间  发生时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'狭义币种  本位币币种（默认跟着发生）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'Currency1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流水表ID，花费那一条', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'FlowID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'编号  主键：四位年+2位月+2日+6位流水', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'所属订单ID    MainID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'OrderID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'金额   实际金额(按照结算的币种进行结算与支付)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'Price';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'狭义金额  本位币金额 ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'Price1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'狭义汇率  本位币汇率', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'Rate1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应收款项ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'ReceivableID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'Summay';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'运单ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'WaybillID';
END

go
IF not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subjects]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Subjects] (
    [ID]           VARCHAR (50)    NOT NULL,
    [Type]         INT             NOT NULL,
    [Conduct]      NVARCHAR (50)   NOT NULL,
    [Catalog]      NVARCHAR (50)   NOT NULL,
    [Name]         NVARCHAR (50)   NOT NULL,
    [Currency]     INT             NULL,
    [Price]        DECIMAL (18, 7) NULL,
    [Steps]        NVARCHAR (MAX)  NULL,
    [IsCount]      BIT             NOT NULL,
    [IsToCustomer] BIT             NOT NULL,
    CONSTRAINT [PK_Subjects_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'分类 
货款  
税款  
代理费  
杂费 
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'Catalog';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'业务  
枚举化开发 Enum：
代仓储   
代报关   
代收货   
代发货   
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'Conduct';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'默认计算币种', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'Currency';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Pick 生成，唯一性用  Conduct Catalog Subject Type 自行保障一下', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否需要录入个数', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'IsCount';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否需要转给客户', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'IsToCustomer';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'科目   
停车费 
登记费 
特殊手续的收费  
分拣费  
贴签费  
包装费  
代付货款  
货款  
等  
其他手段：免项、折扣（折掉钱数）、免去尾数
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'Name';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'默认结算金额', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'Price';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'json格式存储 后续选择步骤。如果本次段不为null 就表示有后续步骤', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'Steps';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'类型：Input Output   应收 ， 应付', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'Type';
END


go





go
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreditCostsStatisticsView]') AND type IN (N'V'))
BEGIN
EXEC('
ALTER VIEW [dbo].[CreditCostsStatisticsView]
AS
SELECT Payer, Payee, Business, Catalog, Currency, SUM(LeftPrice-ISNULL(RightPrice,0)) AS Total 
FROM dbo.VouchersStatisticsView
WHERE Status=200
GROUP BY Payer, Payee, Business, Catalog, Currency
')
END

go
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreditsStatisticsView]') AND type IN (N'V'))
BEGIN
EXEC('
ALTER VIEW [dbo].[CreditsStatisticsView]
AS
SELECT  cr.Payer ,
            cr.Payee ,
            cr.Business ,
            cr.Catalog ,
            cr.Currency ,
            ISNULL(cr.Total, 0) AS Total ,
            ISNULL(cc.Total, 0) AS Cost
    FROM    dbo.CreditGrantsStatisticsView AS cr
            LEFT OUTER JOIN dbo.CreditCostsStatisticsView AS cc ON cr.Payer = cc.Payer
                                                              AND cc.Business = cr.Business
                                                              AND cc.Catalog = cr.Catalog
                                                              AND cc.Currency = cr.Currency
                                                              AND cr.Payee = cc.Payee;
')
END



go
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FlowAccountsTopView]') AND type IN (N'V'))
BEGIN
EXEC('
ALTER VIEW [dbo].[FlowAccountsTopView]
AS
SELECT     ID, Type, Payer, Payee, Business, Catalog, Subject, Currency, Price, Currency1, Price1, ERate1, Bank, FormCode, OrderID, WaybillID, AdminID, CreateDate, OriginIndex, ChangeIndex, OriginalDate, 
                      ChangeDate, Account, DateIndex
FROM         dbo.FlowAccounts
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AccountBillsTopView]') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW [dbo].[AccountBillsTopView]
AS
SELECT a.ID AS ReceivableID, a.OrderID, a.WaybillID, a.Payer, a.Payee
	, a.Business, a.Catalog, a.Subject, a.SettlementCurrency AS Currency, a.SettlementPrice AS LeftPrice
	, a.CreateDate AS LeftDate, a.OriginalDate, a.Summay, a.PayeeBeneficiaryID, a.AccountCode
	, a.AdminID, b.ID AS ReceivedID, b.AccountType, b.Price, b.PaidPrice
	, b.CreateDate AS RightDate, cp.Name AS CouponName, srs.Price AS CouponPrice
FROM Receivables a
	LEFT JOIN Receiveds b ON a.ID = b.ReceivableID
	LEFT JOIN dbo.Receiveds srs
	ON b.ID = srs.ReceivableID
		AND srs.CouponID IS NOT NULL
	LEFT JOIN dbo.Coupons cp ON srs.CouponID = cp.ID
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.BalancesTopView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.BalancesTopView
AS
SELECT     Business, Payer, Payee, Currency, SUM(Price) AS Price
FROM         dbo.FlowAccounts
WHERE     (Type = 30)
GROUP BY Currency, Payer, Payee, Business
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.CashRecordsTopView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.CashRecordsTopView
AS
SELECT     ID, Type, Payer, Payee, Business, Catalog, Subject, Currency, Price, Currency1, Price1, ERate1, Bank, Account, FormCode, OrderID, WaybillID, AdminID, CreateDate, OriginIndex, ChangeIndex, 
                      OriginalDate, ChangeDate, DateIndex
FROM         dbo.FlowAccounts
WHERE     (Type = 30)
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.CashStatisticsView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.CashStatisticsView
AS
SELECT     Payer, Payee, Business, Currency, ISNULL(SUM(Price), 0) AS Available
FROM         dbo.FlowAccounts AS fa
WHERE     (Type = 30)
GROUP BY Payer, Payee, Business, Catalog, Currency
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConsignorsTopView]') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW [dbo].[ConsignorsTopView]
AS
SELECT   ID, EnterpriseID, Title, DyjCode, Postzip, Address, Status, Name, Tel, Mobile, Email, CreateDate, AdminID, 
                IsDefault
FROM      dbo.Consignors
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.CouponStatisticsTopView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.CouponStatisticsTopView
AS
SELECT   cp.ID, cp.Name, cp.Code, cp.Type, cp.Conduct, cp.Catalog, cp.Subject, cp.Currency, cp.Price, cp.InOrderCount, 
                cp.CreateDate, cp.CreatorID, cp.Status, fc.Payer, fc.Payee, fc.Input, fc.Output, fc.Balance
FROM      (SELECT   Payer, Payee, CouponID, SUM(Input) AS Input, SUM(Output) AS Output, SUM(Input) - SUM(Output) 
                                 AS Balance
                 FROM      dbo.FlowCoupons
                 GROUP BY Payer, Payee, CouponID) AS fc INNER JOIN
                dbo.Coupons AS cp ON fc.CouponID = cp.ID
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.CouponsTopView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.CouponsTopView
AS
SELECT   ID, Name, Code, Type, Conduct, Catalog, Subject, Currency, Price, InOrderCount, CreateDate, CreatorID, Status
FROM      dbo.Coupons
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.CurrentWsOrderStatusTopView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.CurrentWsOrderStatusTopView
AS
SELECT     MainID, MainStatus, PaymentStatus, ExecutionStatus, InvoiceStatus, RemittanceStatus, ConfirmStatus
FROM         PvWsOrder.dbo.CurrentWsOrderStatusTopView AS CurrentWsOrderStatusTopView_1
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomsInvoiceSynonymTopView]') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW [dbo].[CustomsInvoiceSynonymTopView]
AS
SELECT  MainOrderID,Name, OrderID, LeftTotal, RightTotal ,InvoiceDate from dbo.CustomsInvoiceSynonym
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.CustomsRecordsTopView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.CustomsRecordsTopView
AS
SELECT     ID, ClientID, WaybillID, Price, CreateDate, ConfirmDate
FROM         PvWsOrder.dbo.CustomsRecordsTopView
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.FinancialBudgetStatisticsView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.FinancialBudgetStatisticsView
AS
--是否逾期
SELECT  ''1'' Type ,
        Payer ,
        Payee ,
        Business ,
        Currency ,
        ISNULL(SUM(Price), 0) Total
FROM    dbo.FlowAccounts
WHERE   Type = 20
        AND DateIndex <= LEFT(CONVERT(VARCHAR(100), GETDATE(), 112), 6)
GROUP BY Payee ,
        Payer ,
        Business ,
        Currency
UNION
--信用花费        
SELECT  ''2'' Type ,
        Payer ,
        Payee ,
        Business ,
        Currency ,
        ISNULL(SUM(Price), 0) Total
FROM    dbo.FlowAccounts
WHERE   Type = 20
GROUP BY Payee ,
        Payer ,
        Business ,
        Currency
UNION
 --未支付账单
SELECT  ''3'' Type ,
        Payer ,
        Payee ,
        Business ,
        Currency ,
        SUM(LeftPrice - ISNULL(RightPrice, 0)) Total
FROM    dbo.VouchersStatisticsView
WHERE   ( LeftPrice - ISNULL(RightPrice, 0) ) > 0
GROUP BY Payee ,
        Payer ,
        Business ,
        Currency;
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.FlowAccountsStatisticsView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.FlowAccountsStatisticsView
AS
SELECT   Payer, Payee, Business, Currency, ISNULL(SUM(Price), 0) AS Price, Type
FROM      dbo.FlowAccounts AS fa
GROUP BY Payer, Payee, Business, Catalog, Currency, Type
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceFilesTopView]') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW [dbo].[InvoiceFilesTopView]
AS
SELECT OrderID, fileID,FileType, FileFormat,Name,Url,Status,Summary from [dbo].[InvoiceFilesTopViewSynonym]
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.KdnRequestsTopView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.KdnRequestsTopView
AS
SELECT     ID, OrderCode, ExpType, Quantity, PayType, MonthCode, SenderAddress, SenderCompany, SenderName, SenderMobile, SenderTel, ReceiverAddress, ReceiverCompany, ReceiverName, 
                      ReceiverMobile, ReceiverTel, Remark, Currency, Cost, OtherCost, CreateDate
FROM         PvCenter.dbo.KdnRequestsTopView
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.Logs_PvLsOrderCurrentTopView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.Logs_PvLsOrderCurrentTopView
AS
SELECT     MainID, MainStatus, InvoiceStatus
FROM         PvCenter.dbo.Logs_PvLsOrderCurrentTopView
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.LsOrderTopView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.LsOrderTopView
AS
SELECT     ID, FatherID, Type, Source, ClientID, PayeeID, BeneficiaryID, Currency, InvoiceID, IsInvoiced, Status, InheritStatus, Creator, CreateDate, ModifyDate, Summary, StartDate, EndDate
FROM         PvLsOrder.dbo.LsOrderTopView AS LsOrderTopView_1
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OpenedOrderInvoicesTopView]') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW [dbo].[OpenedOrderInvoicesTopView]
AS
SELECT * from [dbo].[OpenedOrderInvoicesTopViewSynonym]
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderInvoicesTopView]') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW [dbo].[OrderInvoicesTopView]
AS
SELECT * from dbo.OrderInvoicesTopViewSynonym
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.OrdersTopView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.OrdersTopView
AS
SELECT     * 
FROM         PvWsOrder.dbo.OrdersTopView AS OrdersTopView_1
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PaymentsStatisticsView]') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW [dbo].[PaymentsStatisticsView]
AS
SELECT     pb.OrderID, pb.WaybillID, pb.ID AS PayableID, pb.Payer, pb.Payee, pb.Business, pb.Catalog, pb.Subject, pb.Currency, pb.Price AS LeftPrice, pb.CreateDate AS LeftDate, pm1.RightPrice, 
                      pm1.RightDate, pm2.RightPrice AS ReducePrice, pb.Summay, pb.AdminID, pb.Status, pb.TinyID, pb.ItemID, pb.ApplicationID,pb.PayerAnonymous,pb.PayeeAnonymous,pb.VoucherID
FROM         dbo.Payables AS pb LEFT OUTER JOIN
                          (SELECT     PayableID, SUM(Price) AS RightPrice, MAX(CreateDate) AS RightDate
                            FROM          dbo.Payments
                            WHERE      (AccountType <> 50)
                            GROUP BY PayableID) AS pm1 ON pm1.PayableID = pb.ID LEFT OUTER JOIN
                          (SELECT     PayableID, SUM(Price) AS RightPrice, MAX(CreateDate) AS RightDate
                            FROM          dbo.Payments AS Payments_1
                            WHERE      (AccountType = 50)
                            GROUP BY PayableID) AS pm2 ON pm2.PayableID = pb.ID
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.PaymentsTopView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.PaymentsTopView
AS
SELECT     dbo.Payments.*
FROM         dbo.Payments
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReceivedsStatisticsView]') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW [dbo].[ReceivedsStatisticsView]
AS
SELECT     rd.ID AS ReceivedID, rd.ReceivableID, rb.Business, rb.Catalog, rb.Subject, rb.Payer, epPayer.Name AS PayerName, rb.Payee, epPayee.Name AS PayeeName, rd.AccountType, 
                      rb.SettlementCurrency, rd.Price, rd.AdminID, rd.CreateDate, rb.OrderID, rb.TinyID
FROM         dbo.ReceivedsTopView AS rd LEFT OUTER JOIN
                      dbo.Receivables AS rb ON rd.ReceivableID = rb.ID LEFT OUTER JOIN
                      dbo.EnterprisesTopView AS epPayee ON rb.Payee = epPayee.ID LEFT OUTER JOIN
                      dbo.EnterprisesTopView AS epPayer ON rb.Payer = epPayer.ID
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.ReceivedsTopView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.ReceivedsTopView
AS
SELECT     ID, ReceivableID, AccountType, Price, Currency1, Price1, Rate1, AdminID, OrderID, WaybillID, CreateDate, Summay, AccountCode, FlowID, CouponID
FROM         dbo.Receiveds
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VouchersStatisticsView]') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW [dbo].[VouchersStatisticsView]
AS
SELECT     rb.OrderID, rb.WaybillID, rb.ID AS ReceivableID, rb.Payer, rb.Payee, rb.Business, rb.Catalog, rb.Subject, rb.Currency AS OriginCurrency, rb.Price AS OriginPrice, rb.SettlementCurrency AS Currency, 
                      rb.SettlementPrice AS LeftPrice, rb.CreateDate AS LeftDate, rd1.RightPrice, rd1.RightDate, rd2.RightPrice AS ReducePrice, rb.ChangeDate, rb.Summay, rb.AdminID, rb.Status, rb.TinyID, 
                      rb.OriginalDate, rb.OriginalIndex, rb.ChangeIndex, rb.ItemID, rb.ApplicationID, rb.PayerID, rb.PayeeID, rb.Quantity,rb.PayerAnonymous,rb.PayeeAnonymous
FROM         dbo.Receivables AS rb LEFT OUTER JOIN
                          (SELECT     ReceivableID, SUM(Price) AS RightPrice, MAX(CreateDate) AS RightDate
                            FROM          dbo.Receiveds
                            WHERE      (AccountType <> 50)
                            GROUP BY ReceivableID) AS rd1 ON rd1.ReceivableID = rb.ID LEFT OUTER JOIN
                          (SELECT     ReceivableID, SUM(Price) AS RightPrice
                            FROM          dbo.Receiveds AS Receiveds_1
                            WHERE      (AccountType = 50)
                            GROUP BY ReceivableID) AS rd2 ON rd2.ReceivableID = rb.ID
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.VouchersTopView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.VouchersTopView
AS
SELECT     ID, OrderID, DateIndex, ApplicationID, Type, Currency, Payer, Payee, IsSettlement, CreateDate, CreatorID, Status
FROM         dbo.Vouchers
')
END

go
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.WaybillsTopView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.WaybillsTopView
AS
SELECT     * 
FROM         PvCenter.dbo.WaybillsTopView AS WaybillsTopView_1
')
END

go
IF NOT EXISTS(SELECT * FROM sysobjects WHERE id=object_id(N'[dbo].[dp_Overdue]') and xtype='P')
BEGIN
EXEC('
CREATE PROCEDURE [dbo].[dp_Overdue]
    @payer VARCHAR(50) ,		--付款公司ID
    @payee VARCHAR(50) ,		--收款公司ID
    @business VARCHAR(50) ,	--业务
    @date DATETIME ,		--日期
    @catalog VARCHAR(50) = NULL ,
    @currency INT = 1
AS
    BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
        SET NOCOUNT ON;

        IF ( @catalog IS NULL
             OR @catalog = ''All''
           )
            BEGIN
    	--SELECT ''all'' Catalog,ISNULL(SUM(Price),0) Total FROM [dbo].[FlowAccounts] 
    	--WHERE type=20 
    	--AND ChangeDate<@date 
    	--AND Payee=@payee AND Payer=@payer AND Business=@business
    	
    	
                                      
                       
                SELECT  ''all'' Catalog ,
                        ( SUM(ISNULL(rb.SettlementPrice, 0))
                          - SUM(ISNULL(rd.RightPrice, 0)) ) Total
                FROM    dbo.Receivables AS rb
                        LEFT OUTER JOIN ( SELECT    ReceivableID ,
                                                    SUM(Price) AS RightPrice ,
                                                    MAX(CreateDate) AS RightDate
                                          FROM      dbo.Receiveds
                                          GROUP BY  ReceivableID
                                        ) AS rd ON rd.ReceivableID = rb.ID
                WHERE   rb.Status = 200
                        AND Payee = @payee
                        AND Payer = @payer
                        AND Business = @business
                        AND rb.Catalog<>''货款''
                        AND SettlementCurrency = @currency
                        AND rb.ChangeDate <= @date;
                        
                RETURN;
            END;
    
        IF ( @catalog IS NULL
             OR @catalog = ''Catalog''
           )
            BEGIN
		--SELECT  [Catalog], ISNULL(sum([Price]) , 0) Total
		--	  FROM [dbo].[FlowAccounts]
		--	 WHERE type=20 
		--	 AND ChangeDate<@date 
		--	 AND Payee=@payee AND Payer=@payer AND Business=@business
		--	group by [Catalog],Payer,Payee,Business
		
			
                
                SELECT  Catalog ,
                        ( SUM(ISNULL(rb.SettlementPrice, 0))
                          - SUM(ISNULL(rd.RightPrice, 0)) ) Total
                FROM    dbo.Receivables AS rb
                        LEFT OUTER JOIN ( SELECT    ReceivableID ,
                                                    SUM(Price) AS RightPrice ,
                                                    MAX(CreateDate) AS RightDate
                                          FROM      dbo.Receiveds
                                          GROUP BY  ReceivableID
                                        ) AS rd ON rd.ReceivableID = rb.ID
                WHERE   rb.Status = 200
                        AND Payee = @payee
                        AND Payer = @payer
                        AND Business = @business
                        AND rb.Catalog<>''货款''
                        AND SettlementCurrency = @currency
                        AND rb.ChangeDate <= @date
                GROUP BY rb.Catalog;
                RETURN;
            END;
    END;
')
END
