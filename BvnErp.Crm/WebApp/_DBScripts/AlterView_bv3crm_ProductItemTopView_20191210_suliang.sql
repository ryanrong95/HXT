USE [bv3crm]
GO

/****** Object:  View [dbo].[ProductItemTopView]    Script Date: 12/10/2019 16:16:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[ProductItemTopView]
AS
SELECT  dbo.ProductItems.ID, dbo.ProductItems.StandardID, dbo.ProductItems.CompeteID, dbo.ProductItems.RefUnitQuantity, dbo.ProductItems.RefQuantity, dbo.ProductItems.RefUnitPrice, dbo.ProductItems.ExpectRate, 
               dbo.ProductItems.ExpectQuantity, dbo.ProductItems.Status, dbo.ProductItems.CreateDate, dbo.ProductItems.UpdateDate, dbo.ProductItems.PMAdmin, dbo.ProductItems.FAEAdmin, dbo.ProductItems.SaleAdmin, 
               dbo.ProductItems.PurChaseAdmin, dbo.ProductItems.AssistantAdmin, dbo.ProductItems.UnitPrice, dbo.ProductItems.Quantity, dbo.ProductItems.Count, dbo.ProductItems.OriginNumber, 
               dbo.ProductItemEnquiries.ReplyPrice, dbo.ProductItemEnquiries.ReplyDate, dbo.ProductItemEnquiries.RFQ, dbo.ProductItemEnquiries.OriginModel, dbo.ProductItemEnquiries.MOQ, dbo.ProductItemEnquiries.MPQ, 
               dbo.ProductItemEnquiries.Currency, dbo.ProductItemEnquiries.ExchangeRate, dbo.ProductItemEnquiries.TaxRate, dbo.ProductItemEnquiries.Tariff, dbo.ProductItemEnquiries.OtherRate, 
               dbo.ProductItemEnquiries.Cost, dbo.ProductItemEnquiries.Validity, dbo.ProductItemEnquiries.ValidityCount, dbo.ProductItemEnquiries.SalePrice, dbo.ProductItemEnquiries.CreateDate AS EnquiryCreateDate, 
               dbo.ProductItemEnquiries.UpdateDate AS EnquiryUpdateDate, dbo.ProductItemEnquiries.Summary AS EnquirySummary, dbo.ProductItemSamples.Type, dbo.ProductItemSamples.UnitPrice AS SampleUnitPrice, 
               dbo.ProductItemSamples.Quantity AS SampleQuantity, dbo.ProductItemSamples.TotalPrice, dbo.ProductItemSamples.Date, dbo.ProductItemSamples.Contactor, dbo.ProductItemSamples.Phone, 
               dbo.ProductItemSamples.Address, dbo.ProductItemSamples.CreateDate AS SampleCreateDate, dbo.ProductItemSamples.UpdateDate AS SampleUpdateDate, dbo.ProductItems.ReportDate, 
               dbo.ProductItems.IsReport, dbo.ProductItems.ExpectDate, dbo.ProductItemSamples.ID AS SampleID, dbo.ProductItemEnquiries.ID AS EnquiryID, dbo.ProductItems.Summary
FROM     dbo.ProductItems LEFT OUTER JOIN
               dbo.ProductItemEnquiries ON dbo.ProductItems.ID = dbo.ProductItemEnquiries.ID LEFT OUTER JOIN
               dbo.ProductItemSamples ON dbo.ProductItems.ID = dbo.ProductItemSamples.ID

GO
