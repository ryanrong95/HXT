USE [bv3crm]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[MapsProductItemEnquiry](
	[ID] [varchar](50) NOT NULL,
	[ProductItemID] [varchar](50) NOT NULL,
	[ProductItemEnquiryID] [varchar](50) NOT NULL,
 CONSTRAINT [PK_MapsProductItemEnquiry] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[MapsProductItemEnquiry]  WITH CHECK ADD  CONSTRAINT [FK_MapsProductItemEnquiry_ProductItemEnquiries] FOREIGN KEY([ProductItemEnquiryID])
REFERENCES [dbo].[ProductItemEnquiries] ([ID])
GO

ALTER TABLE [dbo].[MapsProductItemEnquiry] CHECK CONSTRAINT [FK_MapsProductItemEnquiry_ProductItemEnquiries]
GO

ALTER TABLE [dbo].[MapsProductItemEnquiry]  WITH CHECK ADD  CONSTRAINT [FK_MapsProductItemEnquiry_ProductItems] FOREIGN KEY([ProductItemID])
REFERENCES [dbo].[ProductItems] ([ID])
GO

ALTER TABLE [dbo].[MapsProductItemEnquiry] CHECK CONSTRAINT [FK_MapsProductItemEnquiry_ProductItems]
GO

--添加现有的产品和询价关系
insert into [dbo].[MapsProductItemEnquiry]
select 
Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',convert(varchar(100),''+ ID+ID+''
				 ))),3,32)),ID,ID
 from ProductItemEnquiries

