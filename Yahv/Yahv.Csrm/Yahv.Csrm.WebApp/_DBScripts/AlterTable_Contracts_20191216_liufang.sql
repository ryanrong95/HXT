use PvbCrm;


GO
--���[ContractsTopView]
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[ContactsTopView]'))
DROP VIEW [dbo].[ContractsTopView]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ContractsTopView]
AS
SELECT   Summary, InvoiceTaxRate, Status, InvoiceType, MinAgencyFee, AgencyRate, ExchangeMode, EndDate, StartDate, ID, 
                EnterpriseID
FROM      dbo.Contracts


GO
--��ӱ�[dbo].[WsContracts]
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[WsContracts]') AND type in (N'U'))
BEGIN
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
CREATE TABLE [dbo].[WsContracts](
	[ID] [varchar](50) NOT NULL,
	[Trustee] [varchar](50) NOT NULL,
	[WsClientID] [varchar](50) NOT NULL,
	[ContainerNum] [int] NOT NULL,
	[Currency] [int] NOT NULL,
	[Charges] [decimal](18, 5) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[Status] [int] NOT NULL,
	[CreatorID] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Summary] [nvarchar](400) NULL,
 CONSTRAINT [PK_WsContracts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_PADDING OFF
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'���з����ڲ���˾���ҷ�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsContracts', @level2type=N'COLUMN',@level2name=N'Trustee'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ί�з����ͻ�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsContracts', @level2type=N'COLUMN',@level2name=N'WsClientID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsContracts', @level2type=N'COLUMN',@level2name=N'ContainerNum'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsContracts', @level2type=N'COLUMN',@level2name=N'Currency'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ִ���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsContracts', @level2type=N'COLUMN',@level2name=N'Charges'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ͬ��ʼʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsContracts', @level2type=N'COLUMN',@level2name=N'StartDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ͬ��ֹʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsContracts', @level2type=N'COLUMN',@level2name=N'EndDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'״̬' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsContracts', @level2type=N'COLUMN',@level2name=N'Status'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsContracts', @level2type=N'COLUMN',@level2name=N'CreatorID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsContracts', @level2type=N'COLUMN',@level2name=N'CreateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ע' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsContracts', @level2type=N'COLUMN',@level2name=N'Summary'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ί�д��ա���������������Э����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WsContracts'


END;

---�����ͼ[dbo].[DriversTopView]
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[DriversTopView]'))
DROP VIEW [dbo].[DriversTopView]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[DriversTopView]
AS
SELECT   ID, EnterpriseID, Name, IDCard, Mobile, [Status]
FROM      dbo.Drivers


GO
---�����ͼ[dbo].[TransportsTopView]
IF EXISTS (SELECT * FROM sys.views WHERE object_id =
OBJECT_ID(N'[dbo].[TransportsTopView]'))
DROP VIEW [dbo].[TransportsTopView]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TransportsTopView]
AS
SELECT   ID, EnterpriseID, Type, CarNumber1, CarNumber2, [Weight], [Status]
FROM      dbo.Transports




GO