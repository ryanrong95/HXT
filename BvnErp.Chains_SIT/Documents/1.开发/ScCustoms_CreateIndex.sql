USE [ScCustoms]
GO

SET ANSI_PADDING ON

--为[ClientLogs]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ClientLogs_ClientID')
CREATE NONCLUSTERED INDEX [IX_ClientLogs_ClientID] ON [dbo].[ClientLogs]([ClientID] ASC)
INCLUDE ([ID],[CreateDate],[Summary])

--为[CustomsTariffs]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_CustomsTariffs_HSCode')
CREATE NONCLUSTERED INDEX [IX_CustomsTariffs_HSCode] ON [dbo].[CustomsTariffs]([HSCode] ASC)

--为[DecContainers]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecContainers_DeclarationID')
CREATE NONCLUSTERED INDEX [IX_DecContainers_DeclarationID] ON [dbo].[DecContainers]([DeclarationID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecContainers_ContainerID')
CREATE NONCLUSTERED INDEX [IX_DecContainers_ContainerID] ON [dbo].[DecContainers]([ContainerID] ASC)

--为[DecHeadFiles]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecHeadFiles_DecHeadID')
CREATE NONCLUSTERED INDEX [IX_DecHeadFiles_DecHeadID] ON [dbo].[DecHeadFiles]([DecHeadID] ASC)

--为[DecHeads]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecHeads_DeclarationNoticeID')
CREATE NONCLUSTERED INDEX [IX_DecHeads_DeclarationNoticeID] ON [dbo].[DecHeads]([DeclarationNoticeID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecHeads_OrderID')
CREATE NONCLUSTERED INDEX [IX_DecHeads_OrderID] ON [dbo].[DecHeads]([OrderID] ASC)

--为[DeclarationNoticeItems]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DeclarationNoticeItems_DeclarationNoticeID')
CREATE NONCLUSTERED INDEX [IX_DeclarationNoticeItems_DeclarationNoticeID] ON [dbo].[DeclarationNoticeItems]([DeclarationNoticeID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DeclarationNoticeItems_SortingID')
CREATE NONCLUSTERED INDEX [IX_DeclarationNoticeItems_SortingID] ON [dbo].[DeclarationNoticeItems]([SortingID] ASC)

--为[DeclarationNoticeLogs]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DeclarationNoticeLogs_DeclarationNoticeID')
CREATE NONCLUSTERED INDEX [IX_DeclarationNoticeLogs_DeclarationNoticeID] ON [dbo].[DeclarationNoticeLogs]([DeclarationNoticeID] ASC)
INCLUDE ([ID],[CreateDate],[Summary])

--为[DeclarationNotices]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DeclarationNotices_OrderID')
CREATE NONCLUSTERED INDEX [IX_DeclarationNotices_OrderID] ON [dbo].[DeclarationNotices]([OrderID] ASC)

--为[DecLicenseDocus]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecLicenseDocus_DeclarationID')
CREATE NONCLUSTERED INDEX [IX_DecLicenseDocus_DeclarationID] ON [dbo].[DecLicenseDocus]([DeclarationID] ASC)

--为[DecLists]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecLists_DeclarationID')
CREATE NONCLUSTERED INDEX [IX_DecLists_DeclarationID] ON [dbo].[DecLists]([DeclarationID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecLists_DeclarationNoticeItemID')
CREATE NONCLUSTERED INDEX [IX_DecLists_DeclarationNoticeItemID] ON [dbo].[DecLists]([DeclarationNoticeItemID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecLists_OrderID')
CREATE NONCLUSTERED INDEX [IX_DecLists_OrderID] ON [dbo].[DecLists]([OrderID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecLists_OrderItemID')
CREATE NONCLUSTERED INDEX [IX_DecLists_OrderItemID] ON [dbo].[DecLists]([OrderItemID] ASC)

--为[DecOtherPacks]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecOtherPacks_DeclarationID')
CREATE NONCLUSTERED INDEX [IX_DecOtherPacks_DeclarationID] ON [dbo].[DecOtherPacks]([DeclarationID] ASC)

--为[DecRequestCerts]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecRequestCerts_DeclarationID')
CREATE NONCLUSTERED INDEX [IX_DecRequestCerts_DeclarationID] ON [dbo].[DecRequestCerts]([DeclarationID] ASC)

--为[DecTaxFlows]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecTaxFlows_DecTaxID')
CREATE NONCLUSTERED INDEX [IX_DecTaxFlows_DecTaxID] ON [dbo].[DecTaxFlows]([DecTaxID] ASC)

--为[DeclarationID]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecTraces_DeclarationID')
CREATE NONCLUSTERED INDEX [IX_DecTraces_DeclarationID] ON [dbo].[DecTraces]([DeclarationID] ASC)

--为[DeliveryConsignees]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DeliveryConsignees_DeliveryNoticeID')
CREATE NONCLUSTERED INDEX [IX_DeliveryConsignees_DeliveryNoticeID] ON [dbo].[DeliveryConsignees]([DeliveryNoticeID] ASC)

--为[DeliveryNoticeLogs]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DeliveryNoticeLogs_DeliveryNoticeID')
CREATE NONCLUSTERED INDEX [IX_DeliveryNoticeLogs_DeliveryNoticeID] ON [dbo].[DeliveryNoticeLogs]([DeliveryNoticeID] ASC)
INCLUDE ([ID],[CreateDate],[Summary])

--为[DeliveryNotices]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DeliveryNotices_OrderID')
CREATE NONCLUSTERED INDEX [IX_DeliveryNotices_OrderID] ON [dbo].[DeliveryNotices]([OrderID] ASC)

--为[EdocRealations]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_EdocRealations_DeclarationID')
CREATE NONCLUSTERED INDEX [IX_EdocRealations_DeclarationID] ON [dbo].[EdocRealations]([DeclarationID] ASC)

--为[EntryNoticeItems]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_EntryNoticeItems_EntryNoticeID')
CREATE NONCLUSTERED INDEX [IX_EntryNoticeItems_EntryNoticeID] ON [dbo].[EntryNoticeItems]([EntryNoticeID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_EntryNoticeItems_OrderItemID')
CREATE NONCLUSTERED INDEX [IX_EntryNoticeItems_OrderItemID] ON [dbo].[EntryNoticeItems]([OrderItemID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_EntryNoticeItems_DecListID')
CREATE NONCLUSTERED INDEX [IX_EntryNoticeItems_DecListID] ON [dbo].[EntryNoticeItems]([DecListID] ASC)

--为[EntryNoticeLogs]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_EntryNoticeLogs_EntryNoticeID')
CREATE NONCLUSTERED INDEX [IX_EntryNoticeLogs_EntryNoticeID] ON [dbo].[EntryNoticeLogs]([EntryNoticeID] ASC)
INCLUDE ([ID],[CreateDate],[Summary])

--为[EntryNotices]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_EntryNotices_OrderID')
CREATE NONCLUSTERED INDEX [IX_EntryNotices_OrderID] ON [dbo].[EntryNotices]([OrderID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_EntryNotices_DecHeadID')
CREATE NONCLUSTERED INDEX [IX_EntryNotices_DecHeadID] ON [dbo].[EntryNotices]([DecHeadID] ASC)

--为[ExitDelivers]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitDelivers_ExitNoticeID')
CREATE NONCLUSTERED INDEX [IX_ExitDelivers_ExitNoticeID] ON [dbo].[ExitDelivers]([ExitNoticeID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitDelivers_ConsigneeID')
CREATE NONCLUSTERED INDEX [IX_ExitDelivers_ConsigneeID] ON [dbo].[ExitDelivers]([ConsigneeID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitDelivers_DeliverID')
CREATE NONCLUSTERED INDEX [IX_ExitDelivers_DeliverID] ON [dbo].[ExitDelivers]([DeliverID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitDelivers_ExpressageID')
CREATE NONCLUSTERED INDEX [IX_ExitDelivers_ExpressageID] ON [dbo].[ExitDelivers]([ExpressageID] ASC)

--为[ExitNoticeFiles]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitNoticeFiles_ExitNoticeID')
CREATE NONCLUSTERED INDEX [IX_ExitNoticeFiles_ExitNoticeID] ON [dbo].[ExitNoticeFiles]([ExitNoticeID] ASC)

--为[ExitNoticeItems]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitNoticeItems_ExitNoticeID')
CREATE NONCLUSTERED INDEX [IX_ExitNoticeItems_ExitNoticeID] ON [dbo].[ExitNoticeItems]([ExitNoticeID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitNoticeItems_DecListID')
CREATE NONCLUSTERED INDEX [IX_ExitNoticeItems_DecListID] ON [dbo].[ExitNoticeItems]([DecListID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitNoticeItems_SortingID')
CREATE NONCLUSTERED INDEX [IX_ExitNoticeItems_SortingID] ON [dbo].[ExitNoticeItems]([SortingID] ASC)

--为[ExitNoticeLogs]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitNoticeLogs_ExitNoticeID')
CREATE NONCLUSTERED INDEX [IX_ExitNoticeLogs_ExitNoticeID] ON [dbo].[ExitNoticeLogs]([ExitNoticeID] ASC)
INCLUDE ([ID],[CreateDate],[Summary])

--为[ExitNotices]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitNotices_OrderID')
CREATE NONCLUSTERED INDEX [IX_ExitNotices_OrderID] ON [dbo].[ExitNotices]([OrderID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitNotices_DecHeadID')
CREATE NONCLUSTERED INDEX [IX_ExitNotices_DecHeadID] ON [dbo].[ExitNotices]([DecHeadID] ASC)

--为[FinanceAccountFlows]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_FinanceAccountFlows_SourceID')
CREATE NONCLUSTERED INDEX [IX_FinanceAccountFlows_SourceID] ON [dbo].[FinanceAccountFlows]([SourceID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_FinanceAccountFlows_FinanceVaultID')
CREATE NONCLUSTERED INDEX [IX_FinanceAccountFlows_FinanceVaultID] ON [dbo].[FinanceAccountFlows]([FinanceVaultID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_FinanceAccountFlows_FinanceAccountID')
CREATE NONCLUSTERED INDEX [IX_FinanceAccountFlows_FinanceAccountID] ON [dbo].[FinanceAccountFlows]([FinanceAccountID] ASC)

--为[FinancePayments]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_FinancePayments_FinanceVaultID')
CREATE NONCLUSTERED INDEX [IX_FinancePayments_FinanceVaultID] ON [dbo].[FinancePayments]([FinanceVaultID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_FinancePayments_FinanceAccountID')
CREATE NONCLUSTERED INDEX [IX_FinancePayments_FinanceAccountID] ON [dbo].[FinancePayments]([FinanceAccountID] ASC)

--为[FinanceReceipts]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_FinanceReceipts_FinanceVaultID')
CREATE NONCLUSTERED INDEX [IX_FinanceReceipts_FinanceVaultID] ON [dbo].[FinanceReceipts]([FinanceVaultID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_FinanceReceipts_FinanceAccountID')
CREATE NONCLUSTERED INDEX [IX_FinanceReceipts_FinanceAccountID] ON [dbo].[FinanceReceipts]([FinanceAccountID] ASC)

--为[IcgooOrderMap]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_IcgooOrderMap_OrderID')
CREATE NONCLUSTERED INDEX [IX_IcgooOrderMap_OrderID] ON [dbo].[IcgooOrderMap]([OrderID] ASC)

--为[InvoiceNoticeItems]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_InvoiceNoticeItems_InvoiceNoticeID')
CREATE NONCLUSTERED INDEX [IX_InvoiceNoticeItems_InvoiceNoticeID] ON [dbo].[InvoiceNoticeItems]([InvoiceNoticeID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_InvoiceNoticeItems_OrderItemID')
CREATE NONCLUSTERED INDEX [IX_InvoiceNoticeItems_OrderItemID] ON [dbo].[InvoiceNoticeItems]([OrderItemID] ASC)

--为[InvoiceNoticeLogs]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_InvoiceNoticeLogs_InvoiceNoticeID')
CREATE NONCLUSTERED INDEX [IX_InvoiceNoticeLogs_InvoiceNoticeID] ON [dbo].[InvoiceNoticeLogs]([InvoiceNoticeID] ASC)
INCLUDE ([ID],[CreateDate],[Summary])

--为[InvoiceNotices]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_InvoiceNotices_ClientID')
CREATE NONCLUSTERED INDEX [IX_InvoiceNotices_ClientID] ON [dbo].[InvoiceNotices]([ClientID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_InvoiceNotices_ClientInvoiceID')
CREATE NONCLUSTERED INDEX [IX_InvoiceNotices_ClientInvoiceID] ON [dbo].[InvoiceNotices]([ClientInvoiceID] ASC)

--为[InvoiceWaybills]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_InvoiceWaybills_InvoiceNoticeID')
CREATE NONCLUSTERED INDEX [IX_InvoiceWaybills_InvoiceNoticeID] ON [dbo].[InvoiceWaybills]([InvoiceNoticeID] ASC)

--为[ManifestConsignmentContainers]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ManifestConsignmentContainers_ManifestConsignmentID')
CREATE NONCLUSTERED INDEX [IX_ManifestConsignmentContainers_ManifestConsignmentID] ON [dbo].[ManifestConsignmentContainers]([ManifestConsignmentID] ASC)

--为[ManifestConsignmentItems]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ManifestConsignmentItems_ManifestConsignmentID')
CREATE NONCLUSTERED INDEX [IX_ManifestConsignmentItems_ManifestConsignmentID] ON [dbo].[ManifestConsignmentItems]([ManifestConsignmentID] ASC)

--为[ManifestConsignments]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ManifestConsignments_ManifestID')
CREATE NONCLUSTERED INDEX [IX_ManifestConsignments_ManifestID] ON [dbo].[ManifestConsignments]([ManifestID] ASC)

--为[ManifestConsignmentTraces]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ManifestConsignmentTraces_ManifestConsignmentID')
CREATE NONCLUSTERED INDEX [IX_ManifestConsignmentTraces_ManifestConsignmentID] ON [dbo].[ManifestConsignmentTraces]([ManifestConsignmentID] ASC)

--为[OrderConsignees]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderConsignees_OrderID')
CREATE NONCLUSTERED INDEX [IX_OrderConsignees_OrderID] ON [dbo].[OrderConsignees]([OrderID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderConsignees_ClientSupplierID')
CREATE NONCLUSTERED INDEX [IX_OrderConsignees_ClientSupplierID] ON [dbo].[OrderConsignees]([ClientSupplierID] ASC)

--为[OrderConsignors]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderConsignors_OrderID')
CREATE NONCLUSTERED INDEX [IX_OrderConsignors_OrderID] ON [dbo].[OrderConsignors]([OrderID] ASC)

--为[OrderControls]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderControls_OrderID')
CREATE NONCLUSTERED INDEX [IX_OrderControls_OrderID] ON [dbo].[OrderControls]([OrderID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderControls_OrderItemID')
CREATE NONCLUSTERED INDEX [IX_OrderControls_OrderItemID] ON [dbo].[OrderControls]([OrderItemID] ASC)

--为[OrderControlSteps]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderControlSteps_OrderControlID')
CREATE NONCLUSTERED INDEX [IX_OrderControlSteps_OrderControlID] ON [dbo].[OrderControlSteps]([OrderControlID] ASC)

--为[OrderFiles]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderFiles_OrderID')
CREATE NONCLUSTERED INDEX [IX_OrderFiles_OrderID] ON [dbo].[OrderFiles]([OrderID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderFiles_OrderItemID')
CREATE NONCLUSTERED INDEX [IX_OrderFiles_OrderItemID] ON [dbo].[OrderFiles]([OrderItemID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderFiles_OrderPremiumID')
CREATE NONCLUSTERED INDEX [IX_OrderFiles_OrderPremiumID] ON [dbo].[OrderFiles]([OrderPremiumID] ASC)

--为[OrderItemCategories]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderItemCategories_OrderItemID')
CREATE NONCLUSTERED INDEX [IX_OrderItemCategories_OrderItemID] ON [dbo].[OrderItemCategories]([OrderItemID] ASC)

--为[OrderItems]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderItems_OrderID')
CREATE NONCLUSTERED INDEX [IX_OrderItems_OrderID] ON [dbo].[OrderItems]([OrderID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderItems_ProductID')
CREATE NONCLUSTERED INDEX [IX_OrderItems_ProductID] ON [dbo].[OrderItems]([ProductID] ASC)

--为[OrderItemTaxes]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderItemTaxes_OrderItemID')
CREATE NONCLUSTERED INDEX [IX_OrderItemTaxes_OrderItemID] ON [dbo].[OrderItemTaxes]([OrderItemID] ASC)

--为[OrderLogs]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderLogs_OrderID')
CREATE NONCLUSTERED INDEX [IX_OrderLogs_OrderID] ON [dbo].[OrderLogs]([OrderID] ASC)

--为[OrderPayExchangeSuppliers]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderPayExchangeSuppliers_OrderID')
CREATE NONCLUSTERED INDEX [IX_OrderPayExchangeSuppliers_OrderID] ON [dbo].[OrderPayExchangeSuppliers]([OrderID] ASC)

--为[OrderPremiums]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderPremiums_OrderID')
CREATE NONCLUSTERED INDEX [IX_OrderPremiums_OrderID] ON [dbo].[OrderPremiums]([OrderID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderPremiums_OrderItemID')
CREATE NONCLUSTERED INDEX [IX_OrderPremiums_OrderItemID] ON [dbo].[OrderPremiums]([OrderItemID] ASC)

--为[OrderReceipts]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderReceipts_ClientID')
CREATE NONCLUSTERED INDEX [IX_OrderReceipts_ClientID] ON [dbo].[OrderReceipts]([ClientID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderReceipts_OrderID')
CREATE NONCLUSTERED INDEX [IX_OrderReceipts_OrderID] ON [dbo].[OrderReceipts]([OrderID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderReceipts_FeeSourceID')
CREATE NONCLUSTERED INDEX [IX_OrderReceipts_FeeSourceID] ON [dbo].[OrderReceipts]([FeeSourceID] ASC)

--为[Orders]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_Orders_ClientID')
CREATE NONCLUSTERED INDEX [IX_Orders_ClientID] ON [dbo].[Orders]([ClientID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_Orders_ClientAgreementID')
CREATE NONCLUSTERED INDEX [IX_Orders_ClientAgreementID] ON [dbo].[Orders]([ClientAgreementID] ASC)

--为[OrderTraces]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderTraces_OrderID')
CREATE NONCLUSTERED INDEX [IX_OrderTraces_OrderID] ON [dbo].[OrderTraces]([OrderID] ASC)

--为[OrderVoyages]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderVoyages_OrderID')
CREATE NONCLUSTERED INDEX [IX_OrderVoyages_OrderID] ON [dbo].[OrderVoyages]([OrderID] ASC)

--为[OrderWaybillItems]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderWaybillItems_OrderWaybillID')
CREATE NONCLUSTERED INDEX [IX_OrderWaybillItems_OrderWaybillID] ON [dbo].[OrderWaybillItems]([OrderWaybillID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderWaybillItems_SortingID')
CREATE NONCLUSTERED INDEX [IX_OrderWaybillItems_SortingID] ON [dbo].[OrderWaybillItems]([SortingID] ASC)

--为[OrderWaybills]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderWaybills_OrderID')
CREATE NONCLUSTERED INDEX [IX_OrderWaybills_OrderID] ON [dbo].[OrderWaybills]([OrderID] ASC)

--为[OrderWhesPremiumFiles]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderWhesPremiumFiles_OrderWhesPremiumID')
CREATE NONCLUSTERED INDEX [IX_OrderWhesPremiumFiles_OrderWhesPremiumID] ON [dbo].[OrderWhesPremiumFiles]([OrderWhesPremiumID] ASC)

--为[OrderWhesPremiumLogs]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderWhesPremiumLogs_OrderID')
CREATE NONCLUSTERED INDEX [IX_OrderWhesPremiumLogs_OrderID] ON [dbo].[OrderWhesPremiumLogs]([OrderID] ASC)
INCLUDE ([ID],[CreateDate],[Summary])
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderWhesPremiumLogs_OrderWhesPremiumID')
CREATE NONCLUSTERED INDEX [IX_OrderWhesPremiumLogs_OrderWhesPremiumID] ON [dbo].[OrderWhesPremiumLogs]([OrderWhesPremiumID] ASC)
INCLUDE ([ID],[CreateDate],[Summary])

--为[OrderWhesPremiums]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderWhesPremiums_OrderID')
CREATE NONCLUSTERED INDEX [IX_OrderWhesPremiums_OrderID] ON [dbo].[OrderWhesPremiums]([OrderID] ASC)

--为[PackingItems]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PackingItems_PackingID')
CREATE NONCLUSTERED INDEX [IX_PackingItems_PackingID] ON [dbo].[PackingItems]([PackingID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PackingItems_SortingID')
CREATE NONCLUSTERED INDEX [IX_PackingItems_SortingID] ON [dbo].[PackingItems]([SortingID] ASC)

--为[Packings]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_Packings_OrderID')
CREATE NONCLUSTERED INDEX [IX_Packings_OrderID] ON [dbo].[Packings]([OrderID] ASC)

--为[PayExchangeApplies]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PayExchangeApplies_ClientID')
CREATE NONCLUSTERED INDEX [IX_PayExchangeApplies_ClientID] ON [dbo].[PayExchangeApplies]([ClientID] ASC)

--为[PayExchangeApplyFiles]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PayExchangeApplyFiles_PayExchangeApplyID')
CREATE NONCLUSTERED INDEX [IX_PayExchangeApplyFiles_PayExchangeApplyID] ON [dbo].[PayExchangeApplyFiles]([PayExchangeApplyID] ASC)

--为[PayExchangeApplyItems]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PayExchangeApplyItems_PayExchangeApplyID')
CREATE NONCLUSTERED INDEX [IX_PayExchangeApplyItems_PayExchangeApplyID] ON [dbo].[PayExchangeApplyItems]([PayExchangeApplyID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PayExchangeApplyItems_OrderID')
CREATE NONCLUSTERED INDEX [IX_PayExchangeApplyItems_OrderID] ON [dbo].[PayExchangeApplyItems]([OrderID] ASC)

--为[PayExchangeApplyLogs]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PayExchangeApplyLogs_PayExchangeApplyID')
CREATE NONCLUSTERED INDEX [IX_PayExchangeApplyLogs_PayExchangeApplyID] ON [dbo].[PayExchangeApplyLogs]([PayExchangeApplyID] ASC)
INCLUDE ([ID],[CreateDate],[Summary])

--为[PaymentApplies]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PaymentApplies_OrderID')
CREATE NONCLUSTERED INDEX [IX_PaymentApplies_OrderID] ON [dbo].[PaymentApplies]([OrderID] ASC)

--为[PaymentApplyLogs]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PaymentApplyLogs_PaymentApplyID')
CREATE NONCLUSTERED INDEX [IX_PaymentApplyLogs_PaymentApplyID] ON [dbo].[PaymentApplyLogs]([PaymentApplyID] ASC)
INCLUDE ([ID],[CreateDate],[Summary])

--为[PaymentNoticeFiles]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PaymentNoticeFiles_PaymentNoticeID')
CREATE NONCLUSTERED INDEX [IX_PaymentNoticeFiles_PaymentNoticeID] ON [dbo].[PaymentNoticeFiles]([PaymentNoticeID] ASC)

--为[PaymentNoticeItems]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PaymentNoticeItems_PaymentNoticeID')
CREATE NONCLUSTERED INDEX [IX_PaymentNoticeItems_PaymentNoticeID] ON [dbo].[PaymentNoticeItems]([PaymentNoticeID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PaymentNoticeItems_OrderID')
CREATE NONCLUSTERED INDEX [IX_PaymentNoticeItems_OrderID] ON [dbo].[PaymentNoticeItems]([OrderID] ASC)

--为[PaymentNotices]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PaymentNotices_PaymentApplyID')
CREATE NONCLUSTERED INDEX [IX_PaymentNotices_PaymentApplyID] ON [dbo].[PaymentNotices]([PaymentApplyID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PaymentNotices_PayExchangeApplyID')
CREATE NONCLUSTERED INDEX [IX_PaymentNotices_PayExchangeApplyID] ON [dbo].[PaymentNotices]([PayExchangeApplyID] ASC)

--为[PreProductCategories]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PreProductCategories_PreProductID')
CREATE NONCLUSTERED INDEX [IX_PreProductCategories_PreProductID] ON [dbo].[PreProductCategories]([PreProductID] ASC)

--为[PreProductPostLog]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PreProductPostLog_PreProductCategoryID')
CREATE NONCLUSTERED INDEX [IX_PreProductPostLog_PreProductCategoryID] ON [dbo].[PreProductPostLog]([PreProductCategoryID] ASC)
INCLUDE ([ID],[Msg],[CreateDate])

--为[PreProducts]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PreProducts_ClientID')
CREATE NONCLUSTERED INDEX [IX_PreProducts_ClientID] ON [dbo].[PreProducts]([ClientID] ASC)

--为[ProductCategories]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ProductCategories_Model')
CREATE NONCLUSTERED INDEX [IX_ProductCategories_Model] ON [dbo].[ProductCategories]([Model] ASC)

--为[ProductClassifyChangeLogs]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ProductClassifyChangeLogs_Model')
CREATE NONCLUSTERED INDEX [IX_ProductClassifyChangeLogs_Model] ON [dbo].[ProductClassifyChangeLogs]([Model] ASC)
INCLUDE ([ID],[CreateDate],[Summary])

--为[ProductClassifyLogs]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ProductClassifyLogs_OrderItemID')
CREATE NONCLUSTERED INDEX [IX_ProductClassifyLogs_OrderItemID] ON [dbo].[ProductClassifyLogs]([OrderItemID] ASC)
INCLUDE ([ID],[OperationLog],[CreateDate]) WHERE [OrderItemID] IS NOT NULL
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ProductClassifyLogs_PreProductID')
CREATE NONCLUSTERED INDEX [IX_ProductClassifyLogs_PreProductID] ON [dbo].[ProductClassifyLogs]([PreProductID] ASC)
INCLUDE ([ID],[OperationLog],[CreateDate]) WHERE [PreProductID] IS NOT NULL

--为[ProductTaxCategories]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ProductTaxCategories_Name')
CREATE NONCLUSTERED INDEX [IX_ProductTaxCategories_Name] ON [dbo].[ProductTaxCategories]([Name] ASC)
INCLUDE ([ID],[Model],[TaxCode],[TaxName],[CreateTime])

--为[ReceiptNotices]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ReceiptNotices_ClientID')
CREATE NONCLUSTERED INDEX [IX_ReceiptNotices_ClientID] ON [dbo].[ReceiptNotices]([ClientID] ASC)

--为[Sortings]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_Sortings_OrderID')
CREATE NONCLUSTERED INDEX [IX_Sortings_OrderID] ON [dbo].[Sortings]([OrderID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_Sortings_OrderItemID')
CREATE NONCLUSTERED INDEX [IX_Sortings_OrderItemID] ON [dbo].[Sortings]([OrderItemID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_Sortings_EntryNoticeItemID')
CREATE NONCLUSTERED INDEX [IX_Sortings_EntryNoticeItemID] ON [dbo].[Sortings]([EntryNoticeItemID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_Sortings_ProductID')
CREATE NONCLUSTERED INDEX [IX_Sortings_ProductID] ON [dbo].[Sortings]([ProductID] ASC)

--为[StoreStorages]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_StoreStorages_OrderItemID')
CREATE NONCLUSTERED INDEX [IX_StoreStorages_OrderItemID] ON [dbo].[StoreStorages]([OrderItemID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_StoreStorages_SortingID')
CREATE NONCLUSTERED INDEX [IX_StoreStorages_SortingID] ON [dbo].[StoreStorages]([SortingID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_StoreStorages_ProductID')
CREATE NONCLUSTERED INDEX [IX_StoreStorages_ProductID] ON [dbo].[StoreStorages]([ProductID] ASC)

--为[SwapLimitCountryLogs]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_SwapLimitCountryLogs_BankID')
CREATE NONCLUSTERED INDEX [IX_SwapLimitCountryLogs_BankID] ON [dbo].[SwapLimitCountryLogs]([BankID] ASC)
INCLUDE ([ID],[CreateDate],[Summary])

--为[SwapNoticeItems]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_SwapNoticeItems_SwapNoticeID')
CREATE NONCLUSTERED INDEX [IX_SwapNoticeItems_SwapNoticeID] ON [dbo].[SwapNoticeItems]([SwapNoticeID] ASC)
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_SwapNoticeItems_DecHeadID')
CREATE NONCLUSTERED INDEX [IX_SwapNoticeItems_DecHeadID] ON [dbo].[SwapNoticeItems]([DecHeadID] ASC)

--为[TemporaryFiles]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_TemporaryFiles_TemporaryID')
CREATE NONCLUSTERED INDEX [IX_TemporaryFiles_TemporaryID] ON [dbo].[TemporaryFiles]([TemporaryID] ASC)

--为[TemporaryLogs]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_TemporaryLogs_TemporaryID')
CREATE NONCLUSTERED INDEX [IX_TemporaryLogs_TemporaryID] ON [dbo].[TemporaryLogs]([TemporaryID] ASC)
INCLUDE ([ID],[CreateDate],[Summary])

--为[UserLogs]表添加索引
IF NOT EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_UserLogs_UserID')
CREATE NONCLUSTERED INDEX [IX_UserLogs_UserID] ON [dbo].[UserLogs]([UserID] ASC)
INCLUDE ([ID],[CreateDate],[Summary])