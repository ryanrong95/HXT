use  PvbCrm;

GO
--[dbo].[Invoices]发票添加企业电话

if (not exists (select * from syscolumns where ID=object_id('Invoices') and name='CompanyTel'))
begin
Alter TABLE [dbo].[Invoices] add CompanyTel varchar(50) null;
end


use  PvbCrm;

---[dbo].[ContactsTopView]添加Fax
GO
ALTER VIEW [dbo].[ContactsTopView]
AS
SELECT   ID, EnterpriseID, Type, Name, Tel, Mobile, Email, Status, Fax
FROM      dbo.Contacts


--修改发票视图,添加企业信息中的纳税人识别号和企业电话
GO
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[InvoicesTopView]'))
DROP VIEW [dbo].[InvoicesTopView]
GO
Create VIEW [dbo].[InvoicesTopView]
AS
SELECT   dbo.Invoices.ID, dbo.Invoices.Type, dbo.Invoices.Bank, dbo.Invoices.BankAddress, dbo.Invoices.Account, 
                dbo.Invoices.TaxperNumber, dbo.Invoices.Name, dbo.Invoices.Tel, dbo.Invoices.Mobile, dbo.Invoices.Email, 
                dbo.Invoices.District, dbo.Invoices.Postzip, dbo.Invoices.Status, dbo.Invoices.Address, dbo.Invoices.EnterpriseID, 
                dbo.Invoices.CreateDate, dbo.Invoices.UpdateDate, dbo.Invoices.AdminID, dbo.Invoices.DeliveryType, 
                dbo.Enterprises.Uscc, dbo.Invoices.CompanyTel
FROM      dbo.Invoices INNER JOIN
                dbo.Enterprises ON dbo.Invoices.EnterpriseID = dbo.Enterprises.ID

GO
--添加视图[dbo].[MapsBEnterTopView]

IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[MapsBEnterTopView]'))
DROP VIEW [dbo].[MapsBEnterTopView]
GO
CREATE VIEW [dbo].[MapsBEnterTopView]
AS
SELECT   ID, Bussiness, Type, EnterpriseID, SubID, CtreatorID, CreateDate, IsDefault
FROM      dbo.MapsBEnter
GO


--添加电子合同表
/****** Object:  Table [dbo].[Contracts]    Script Date: 2019/9/26 星期四 17:42:37 ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[Contracts]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[Contracts](
	[ID] [varchar](50) NOT NULL,
	[EnterpriseID] [varchar](50) NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
	[AgencyRate] [decimal](18, 4) NOT NULL,
	[MinAgencyFee] [decimal](18, 4) NOT NULL,
	[IsPrePayExchange] [bit] NOT NULL,
	[IsLimitNinetyDays] [bit] NOT NULL,
	[InvoiceType] [int] NOT NULL,
	[InvoiceTaxRate] [decimal](18, 4) NOT NULL,
	[Status] [int] NOT NULL,
	[Creator] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Summary] [nvarchar](400) NULL,
 CONSTRAINT [PK_Contracts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



SET ANSI_PADDING OFF


ALTER TABLE [dbo].[Contracts]  WITH CHECK ADD  CONSTRAINT [FK_Contracts_Enterprises] FOREIGN KEY([EnterpriseID])
REFERENCES [dbo].[Enterprises] ([ID])


ALTER TABLE [dbo].[Contracts] CHECK CONSTRAINT [FK_Contracts_Enterprises]

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'开始日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'StartDate'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'结束日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'EndDate'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'代理费率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'AgencyRate'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最低代理费' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'MinAgencyFee'


EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否预付汇' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'IsPrePayExchange'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否90天内换汇' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'IsLimitNinetyDays'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发票对应税点' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'InvoiceTaxRate'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'Status'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Admin' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'Creator'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'CreateDate'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'UpdateDate'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'摘要备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contracts', @level2type=N'COLUMN',@level2name=N'Summary'

END


