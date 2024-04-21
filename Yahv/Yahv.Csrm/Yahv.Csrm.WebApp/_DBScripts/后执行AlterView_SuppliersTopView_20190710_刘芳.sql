USE [PvbCrm]
GO

/****** Object:  View [dbo].[SuppliersTopView]    Script Date: 2019/7/10 星期三 14:09:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[SuppliersTopView]
AS
SELECT   s.ID, s.Status, e.Name, e.District, s.Type, s.Nature, s.Grade, s.AreaType, s.IsFactory, s.AgentCompany, 
                s.TaxperNumber, s.DyjCode, s.InvoiceType, s.Currency, s.Price, s.RepayCycle
FROM      dbo.Suppliers AS s INNER JOIN
                dbo.Enterprises AS e ON s.ID = e.ID

GO



USE [HvRFQ]
GO

/****** Object:  View [dbo].[SuppliersTopView]    Script Date: 2019/7/10 星期三 14:15:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[SuppliersTopView]
AS
SELECT   ID, Status, Name, District, Type, Nature, Grade, AreaType, IsFactory, AgentCompany, TaxperNumber, DyjCode, 
                InvoiceType, Currency, Price, RepayCycle
FROM      PvbCrm.dbo.SuppliersTopView AS SuppliersTopView_1

GO



