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

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����ʹ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'ApplicationID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'CreatorID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����   ��������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'Currency';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�ںţ�������ںžʹ�����action �ǻ���', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'DateIndex';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�Ƿ������������ʱֻ�ǲִ��ѣ�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'IsSettlement';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����ID������ж����ʹ�����ȷ��֧��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'OrderID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�տ˾', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'Payee';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���˾', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'Payer';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'״̬����ȷ�ϡ���ȷ�ϡ��ϳ����رգ���', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'Status';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�տ�ȷ�ϡ�����ȷ�ϡ�����ȷ�ϣ���Կͻ����ã�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers', @level2type = N'COLUMN', @level2name = N'Type';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����֪ͨ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Vouchers';
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

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�����˺�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'Account';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ҵ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'Business';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'Catalog';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��ת����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'ChangeDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��ת�ں�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'ChangeIndex';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����ʱ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'Currency';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���֣�����ң�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'Currency1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�����ں�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'DateIndex';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ĳ������һ���', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'ERate1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���С��ֽ��ո�����������ˮ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'FormCode';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Ψһ�룬���飺ʱ��pkey��ʽ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'OriginalDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�����ں� ��yyyyMM��datetime.Now.add()', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'OriginIndex';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�տ�����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'ReceiptDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��Ŀ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'Subject';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���á����û��ѡ��ֽ��������ˡ������˻����Ż�ȯ�˻����˿��˻���������ˮ�˻�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'Type';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�˵�ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'VoucherID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�˵�ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts', @level2type = N'COLUMN', @level2name = N'WaybillID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��ˮ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowAccounts';
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

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���ࣺ
����  
˰��  
�����  
�ӷ� 
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Catalog';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�Ż�ȯ����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Code';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ҵ�� 

���ִ�   
������   
���ջ�   
������   
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Conduct';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����ʱ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Npc Admin  ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'CreatorID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����Ĭ��Ϊ��CNY��δ�����ܻ��� USD CNY HKD', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Currency';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Pick����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����Լ������ѧϰ����ʱ�����Ż�ȯʹ�õ�����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'InOrderCount';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�Ż�ȯ����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Name';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��ֵ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Price';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'״̬�� Nornal \ Closed', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Status';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��Ŀ��
ͣ���� 
�ǼǷ� 
�����������շ�  
�ּ��  
��ǩ��  
��װ��  
��������  
����  
��  
�����ֶΣ�����ۿۣ��۵�Ǯ��������ȥβ��
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Subject';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�Ż�ȯ���ͣ������ʵ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons', @level2type = N'COLUMN', @level2name = N'Type';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�Ż�ȯ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Coupons';
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

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�� ���� - ����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons', @level2type = N'COLUMN', @level2name = N'Balance';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�Ż�ȯID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons', @level2type = N'COLUMN', @level2name = N'CouponID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons', @level2type = N'COLUMN', @level2name = N'Input';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'֧', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons', @level2type = N'COLUMN', @level2name = N'Output';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'EnterpriseID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons', @level2type = N'COLUMN', @level2name = N'Payee';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'EnterpriseID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons', @level2type = N'COLUMN', @level2name = N'Payer';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ժҪ����Ҫ����Ա��¼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons', @level2type = N'COLUMN', @level2name = N'Summary';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ʹ����(��վ�ͻ�ʹ�õ�)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons', @level2type = N'COLUMN', @level2name = N'UserID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�Ż�ȯ��ˮ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FlowCoupons';
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

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�����ˣ�ֻ�ܺ�̨����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'AdminID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ҵ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'Business';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'Catalog';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��־����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'Days';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�������� ���ػ��ʡ�ʱʱ���ʣ�Ĭ�ϻ��ʣ���Լ������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'ERateType';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Ψһ�룺����  PK  dtǰ׺��ʽ����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�·�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'Months';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'DebtTermID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'OldID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ԭʼʱ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'OriginDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���㷽ʽ��Լ�����ޡ��½ᡢ��Ʊ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_DebtTerms', @level2type = N'COLUMN', @level2name = N'SettlementType';
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

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ID = MD5(�������ֶ�)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MapsCoperation', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�������������ã�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MapsCoperation', @level2type = N'COLUMN', @level2name = N'MainID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�������������õ��ⷽ �� ���ǣ�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MapsCoperation', @level2type = N'COLUMN', @level2name = N'SubID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'������ϵ����ͳó�ײɹ������ִ���������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MapsCoperation', @level2type = N'COLUMN', @level2name = N'Type';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'������ϵ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MapsCoperation';
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

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ҵ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MonthSealedBills', @level2type = N'COLUMN', @level2name = N'Conduct';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MonthSealedBills', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�ں�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MonthSealedBills', @level2type = N'COLUMN', @level2name = N'DateIndex';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�޸�����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MonthSealedBills', @level2type = N'COLUMN', @level2name = N'ModifyDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MonthSealedBills', @level2type = N'COLUMN', @level2name = N'OccurDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�ͻ�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MonthSealedBills', @level2type = N'COLUMN', @level2name = N'Payer';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�Ƿ����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MonthSealedBills', @level2type = N'COLUMN', @level2name = N'Sealed';
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

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�����  Npc��ʵ�ʵ���Ա', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'AdminID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'ApplicationID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ҵ�� 
ö�ٻ����� Enum��
���ִ�  
������  
���ջ� 
������  
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'Business';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���� 
����  
˰��  
�����  
�ӷ� 
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'Catalog';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����ʱ��  ����ʱ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���� ��������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'Currency';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��ˮ���[ҵ��-��˾-��Ŀ-��ˮ��] ��ˮ��Ҫ������������ˮ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��������ID   MainID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'OrderID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�ͻ�����˾����ClientID��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'Payee';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�տ�������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'PayeeAnonymous';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�ͻ����տ����м��˺�   ��������б�����������д��롣', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'PayeeID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'������   �ڲ���˾ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'Payer';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'PayerAnonymous';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�������˻�   ���磺о��ͨ���˻�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'PayerID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���  �������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'Price';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��Ŀ 
ͣ����  
�ǼǷ�  
�����������շ�  
�ּ��  
��ǩ��  
��װ��  
��������  
����  
�ɹ�����  
�ɹ�ֽ��  
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'Subject';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'Summay';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'С����ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'TinyID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�˵�ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables', @level2type = N'COLUMN', @level2name = N'WaybillID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Ӧ������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payables';
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

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���˱���', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'AccountCode';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���á����û��ѡ��ֽ��������ˡ������˻����Ż�ȯ�˻����˿��˻�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'AccountType';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�����  Npc��ʵ�ʵ���Ա', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'AdminID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����ʱ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�������  ��λ�ұ��֣�Ĭ�ϸ��ŷ�����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'Currency1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��ˮ��ID��������һ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'FlowID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Ψһ�� ��������λ��+2λ��+2��+6λ��ˮ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��������ID   MainID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'OrderID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Ӧ������ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'PayableID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���  �������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'Price';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'������  ��λ�ҽ�� ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'Price1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�������  ��λ�һ���', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'Rate1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'Summay';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�˵�ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Payments', @level2type = N'COLUMN', @level2name = N'WaybillID';
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

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�����  Npc��ʵ�ʵ���Ա', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'AdminID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����ID������ʹ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'ApplicationID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ҵ��  
ö�ٻ����� Enum��
���ִ�   
������   
���ջ�   
������   
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Business';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���� 
����  
˰��  
�����  
�ӷ� 
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Catalog';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'������ڻ�����  δ����ʱ��Ϊ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'ChangeDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'������ڻ�����   δ����ʱ��Ϊ�գ�yyyyMM', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'ChangeIndex';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����ʱ��  ����ʱ�� cost', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����   ��������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Currency';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�������  ��λ�ұ��֣�Ĭ�ϸ��ŷ�����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Currency1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���  ��ˮ���[ҵ��-��˾-��Ŀ-��ˮ��] ��ˮ��Ҫ������������ˮ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�����ͺ�ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'ItemID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��������ID    MainID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'OrderID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�������ڻ�����  δ����ʱ��Ϊ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'OriginalDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�������ڻ�����  δ����ʱ��Ϊ�գ�yyyyMM', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'OriginalIndex';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�տ���  �ڲ���˾ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Payee';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�տ�������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'PayeeAnonymous';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�տ���ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'PayeeID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�ͻ�����˾����ClientID��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Payer';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'PayerAnonymous';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'������ID��null ����δ֪��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'PayerID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���  �������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Price';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'������  ��λ�ҽ�� ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Price1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Quantity';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�������  ��λ�һ���', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Rate1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�������  ���ڽ���ʱ��ʹ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'SettlementCurrency';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����۸�  ���ڽ���ʱ��ʹ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'SettlementPrice';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�������  ���ڽ���ʱ��ʹ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'SettlementRate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�������ϳ�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Status';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��Ŀ   
ͣ���� 
�ǼǷ� 
�����������շ�  
�ּ��  
��ǩ��  
��װ��  
��������  
����  
��  
�����ֶΣ�����ۿۣ��۵�Ǯ��������ȥβ��
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Subject';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����  ��ע', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'Summay';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'С����ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'TinyID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�˵�ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receivables', @level2type = N'COLUMN', @level2name = N'WaybillID';
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

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���˱���', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'AccountCode';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���á����û��ѡ��ֽ��������ˡ������˻����Ż�ȯ�˻����˿��˻�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'AccountType';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�����  Npc��ʵ�ʵ���Ա', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'AdminID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�Ż�ȯID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'CouponID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����ʱ��  ����ʱ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�������  ��λ�ұ��֣�Ĭ�ϸ��ŷ�����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'Currency1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��ˮ��ID��������һ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'FlowID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���  ��������λ��+2λ��+2��+6λ��ˮ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��������ID    MainID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'OrderID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���   ʵ�ʽ��(���ս���ı��ֽ��н�����֧��)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'Price';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'������  ��λ�ҽ�� ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'Price1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�������  ��λ�һ���', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'Rate1';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Ӧ�տ���ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'ReceivableID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'Summay';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�˵�ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Receiveds', @level2type = N'COLUMN', @level2name = N'WaybillID';
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

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���� 
����  
˰��  
�����  
�ӷ� 
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'Catalog';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ҵ��  
ö�ٻ����� Enum��
���ִ�   
������   
���ջ�   
������   
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'Conduct';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Ĭ�ϼ������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'Currency';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Pick ���ɣ�Ψһ����  Conduct Catalog Subject Type ���б���һ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�Ƿ���Ҫ¼�����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'IsCount';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'�Ƿ���Ҫת���ͻ�', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'IsToCustomer';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'��Ŀ   
ͣ���� 
�ǼǷ� 
�����������շ�  
�ּ��  
��ǩ��  
��װ��  
��������  
����  
��  
�����ֶΣ�����ۿۣ��۵�Ǯ��������ȥβ��
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'Name';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Ĭ�Ͻ�����', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'Price';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'json��ʽ�洢 ����ѡ���衣������ζβ�Ϊnull �ͱ�ʾ�к�������', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'Steps';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'���ͣ�Input Output   Ӧ�� �� Ӧ��', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Subjects', @level2type = N'COLUMN', @level2name = N'Type';
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
--�Ƿ�����
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
--���û���        
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
 --δ֧���˵�
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
    @payer VARCHAR(50) ,		--���˾ID
    @payee VARCHAR(50) ,		--�տ˾ID
    @business VARCHAR(50) ,	--ҵ��
    @date DATETIME ,		--����
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
                        AND rb.Catalog<>''����''
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
                        AND rb.Catalog<>''����''
                        AND SettlementCurrency = @currency
                        AND rb.ChangeDate <= @date
                GROUP BY rb.Catalog;
                RETURN;
            END;
    END;
')
END
