USE [ScCustoms]
GO

--Ϊ[ClientLogs]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ClientLogs_ClientID')
DROP INDEX [IX_ClientLogs_ClientID] ON [dbo].[ClientLogs]

--Ϊ[CustomsTariffs]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_CustomsTariffs_HSCode')
DROP INDEX [IX_CustomsTariffs_HSCode] ON [dbo].[CustomsTariffs]

--Ϊ[DecContainers]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecContainers_DeclarationID')
DROP INDEX [IX_DecContainers_DeclarationID] ON [dbo].[DecContainers]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecContainers_ContainerID')
DROP INDEX [IX_DecContainers_ContainerID] ON [dbo].[DecContainers]

--Ϊ[DecHeadFiles]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecHeadFiles_DecHeadID')
DROP INDEX [IX_DecHeadFiles_DecHeadID] ON [dbo].[DecHeadFiles]

--Ϊ[DecHeads]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecHeads_DeclarationNoticeID')
DROP INDEX [IX_DecHeads_DeclarationNoticeID] ON [dbo].[DecHeads]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecHeads_OrderID')
DROP INDEX [IX_DecHeads_OrderID] ON [dbo].[DecHeads]

--Ϊ[DeclarationNoticeItems]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DeclarationNoticeItems_DeclarationNoticeID')
DROP INDEX [IX_DeclarationNoticeItems_DeclarationNoticeID] ON [dbo].[DeclarationNoticeItems]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DeclarationNoticeItems_SortingID')
DROP INDEX [IX_DeclarationNoticeItems_SortingID] ON [dbo].[DeclarationNoticeItems]

--Ϊ[DeclarationNoticeLogs]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DeclarationNoticeLogs_DeclarationNoticeID')
DROP INDEX [IX_DeclarationNoticeLogs_DeclarationNoticeID] ON [dbo].[DeclarationNoticeLogs]

--Ϊ[DeclarationNotices]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DeclarationNotices_OrderID')
DROP INDEX [IX_DeclarationNotices_OrderID] ON [dbo].[DeclarationNotices]

--Ϊ[DecLicenseDocus]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecLicenseDocus_DeclarationID')
DROP INDEX [IX_DecLicenseDocus_DeclarationID] ON [dbo].[DecLicenseDocus]

--Ϊ[DecLists]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecLists_DeclarationID')
DROP INDEX [IX_DecLists_DeclarationID] ON [dbo].[DecLists]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecLists_DeclarationNoticeItemID')
DROP INDEX [IX_DecLists_DeclarationNoticeItemID] ON [dbo].[DecLists]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecLists_OrderID')
DROP INDEX [IX_DecLists_OrderID] ON [dbo].[DecLists]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecLists_OrderItemID')
DROP INDEX [IX_DecLists_OrderItemID] ON [dbo].[DecLists]

--Ϊ[DecOtherPacks]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecOtherPacks_DeclarationID')
DROP INDEX [IX_DecOtherPacks_DeclarationID] ON [dbo].[DecOtherPacks]

--Ϊ[DecRequestCerts]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecRequestCerts_DeclarationID')
DROP INDEX [IX_DecRequestCerts_DeclarationID] ON [dbo].[DecRequestCerts]

--Ϊ[DecTaxFlows]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecTaxFlows_DecTaxID')
DROP INDEX [IX_DecTaxFlows_DecTaxID] ON [dbo].[DecTaxFlows]

--Ϊ[DeclarationID]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DecTraces_DeclarationID')
DROP INDEX [IX_DecTraces_DeclarationID] ON [dbo].[DecTraces]

--Ϊ[DeliveryConsignees]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DeliveryConsignees_DeliveryNoticeID')
DROP INDEX [IX_DeliveryConsignees_DeliveryNoticeID] ON [dbo].[DeliveryConsignees]

--Ϊ[DeliveryNoticeLogs]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DeliveryNoticeLogs_DeliveryNoticeID')
DROP INDEX [IX_DeliveryNoticeLogs_DeliveryNoticeID] ON [dbo].[DeliveryNoticeLogs]
--Ϊ[DeliveryNotices]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_DeliveryNotices_OrderID')
DROP INDEX [IX_DeliveryNotices_OrderID] ON [dbo].[DeliveryNotices]

--Ϊ[EdocRealations]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_EdocRealations_DeclarationID')
DROP INDEX [IX_EdocRealations_DeclarationID] ON [dbo].[EdocRealations]

--Ϊ[EntryNoticeItems]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_EntryNoticeItems_EntryNoticeID')
DROP INDEX [IX_EntryNoticeItems_EntryNoticeID] ON [dbo].[EntryNoticeItems]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_EntryNoticeItems_OrderItemID')
DROP INDEX [IX_EntryNoticeItems_OrderItemID] ON [dbo].[EntryNoticeItems]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_EntryNoticeItems_DecListID')
DROP INDEX [IX_EntryNoticeItems_DecListID] ON [dbo].[EntryNoticeItems]

--Ϊ[EntryNoticeLogs]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_EntryNoticeLogs_EntryNoticeID')
DROP INDEX [IX_EntryNoticeLogs_EntryNoticeID] ON [dbo].[EntryNoticeLogs]

--Ϊ[EntryNotices]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_EntryNotices_OrderID')
DROP INDEX [IX_EntryNotices_OrderID] ON [dbo].[EntryNotices]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_EntryNotices_DecHeadID')
DROP INDEX [IX_EntryNotices_DecHeadID] ON [dbo].[EntryNotices]

--Ϊ[ExitDelivers]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitDelivers_ExitNoticeID')
DROP INDEX [IX_ExitDelivers_ExitNoticeID] ON [dbo].[ExitDelivers]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitDelivers_ConsigneeID')
DROP INDEX [IX_ExitDelivers_ConsigneeID] ON [dbo].[ExitDelivers]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitDelivers_DeliverID')
DROP INDEX [IX_ExitDelivers_DeliverID] ON [dbo].[ExitDelivers]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitDelivers_ExpressageID')
DROP INDEX [IX_ExitDelivers_ExpressageID] ON [dbo].[ExitDelivers]

--Ϊ[ExitNoticeFiles]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitNoticeFiles_ExitNoticeID')
DROP INDEX [IX_ExitNoticeFiles_ExitNoticeID] ON [dbo].[ExitNoticeFiles]

--Ϊ[ExitNoticeItems]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitNoticeItems_ExitNoticeID')
DROP INDEX [IX_ExitNoticeItems_ExitNoticeID] ON [dbo].[ExitNoticeItems]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitNoticeItems_DecListID')
DROP INDEX [IX_ExitNoticeItems_DecListID] ON [dbo].[ExitNoticeItems]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitNoticeItems_SortingID')
DROP INDEX [IX_ExitNoticeItems_SortingID] ON [dbo].[ExitNoticeItems]

--Ϊ[ExitNoticeLogs]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitNoticeLogs_ExitNoticeID')
DROP INDEX [IX_ExitNoticeLogs_ExitNoticeID] ON [dbo].[ExitNoticeLogs]

--Ϊ[ExitNotices]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitNotices_OrderID')
DROP INDEX [IX_ExitNotices_OrderID] ON [dbo].[ExitNotices]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ExitNotices_DecHeadID')
DROP INDEX [IX_ExitNotices_DecHeadID] ON [dbo].[ExitNotices]

--Ϊ[FinanceAccountFlows]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_FinanceAccountFlows_SourceID')
DROP INDEX [IX_FinanceAccountFlows_SourceID] ON [dbo].[FinanceAccountFlows]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_FinanceAccountFlows_FinanceVaultID')
DROP INDEX [IX_FinanceAccountFlows_FinanceVaultID] ON [dbo].[FinanceAccountFlows]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_FinanceAccountFlows_FinanceAccountID')
DROP INDEX [IX_FinanceAccountFlows_FinanceAccountID] ON [dbo].[FinanceAccountFlows]

--Ϊ[FinancePayments]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_FinancePayments_FinanceVaultID')
DROP INDEX [IX_FinancePayments_FinanceVaultID] ON [dbo].[FinancePayments]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_FinancePayments_FinanceAccountID')
DROP INDEX [IX_FinancePayments_FinanceAccountID] ON [dbo].[FinancePayments]

--Ϊ[FinanceReceipts]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_FinanceReceipts_FinanceVaultID')
DROP INDEX [IX_FinanceReceipts_FinanceVaultID] ON [dbo].[FinanceReceipts]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_FinanceReceipts_FinanceAccountID')
DROP INDEX [IX_FinanceReceipts_FinanceAccountID] ON [dbo].[FinanceReceipts]

--Ϊ[IcgooOrderMap]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_IcgooOrderMap_OrderID')
DROP INDEX [IX_IcgooOrderMap_OrderID] ON [dbo].[IcgooOrderMap]

--Ϊ[InvoiceNoticeItems]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_InvoiceNoticeItems_InvoiceNoticeID')
DROP INDEX [IX_InvoiceNoticeItems_InvoiceNoticeID] ON [dbo].[InvoiceNoticeItems]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_InvoiceNoticeItems_OrderItemID')
DROP INDEX [IX_InvoiceNoticeItems_OrderItemID] ON [dbo].[InvoiceNoticeItems]

--Ϊ[InvoiceNoticeLogs]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_InvoiceNoticeLogs_InvoiceNoticeID')
DROP INDEX [IX_InvoiceNoticeLogs_InvoiceNoticeID] ON [dbo].[InvoiceNoticeLogs]

--Ϊ[InvoiceNotices]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_InvoiceNotices_ClientID')
DROP INDEX [IX_InvoiceNotices_ClientID] ON [dbo].[InvoiceNotices]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_InvoiceNotices_ClientInvoiceID')
DROP INDEX [IX_InvoiceNotices_ClientInvoiceID] ON [dbo].[InvoiceNotices]

--Ϊ[InvoiceWaybills]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_InvoiceWaybills_InvoiceNoticeID')
DROP INDEX [IX_InvoiceWaybills_InvoiceNoticeID] ON [dbo].[InvoiceWaybills]

--Ϊ[ManifestConsignmentContainers]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ManifestConsignmentContainers_ManifestConsignmentID')
DROP INDEX [IX_ManifestConsignmentContainers_ManifestConsignmentID] ON [dbo].[ManifestConsignmentContainers]

--Ϊ[ManifestConsignmentItems]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ManifestConsignmentItems_ManifestConsignmentID')
DROP INDEX [IX_ManifestConsignmentItems_ManifestConsignmentID] ON [dbo].[ManifestConsignmentItems]

--Ϊ[ManifestConsignments]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ManifestConsignments_ManifestID')
DROP INDEX [IX_ManifestConsignments_ManifestID] ON [dbo].[ManifestConsignments]

--Ϊ[ManifestConsignmentTraces]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ManifestConsignmentTraces_ManifestConsignmentID')
DROP INDEX [IX_ManifestConsignmentTraces_ManifestConsignmentID] ON [dbo].[ManifestConsignmentTraces]

--Ϊ[OrderConsignees]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderConsignees_OrderID')
DROP INDEX [IX_OrderConsignees_OrderID] ON [dbo].[OrderConsignees]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderConsignees_ClientSupplierID')
DROP INDEX [IX_OrderConsignees_ClientSupplierID] ON [dbo].[OrderConsignees]

--Ϊ[OrderConsignors]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderConsignors_OrderID')
DROP INDEX [IX_OrderConsignors_OrderID] ON [dbo].[OrderConsignors]

--Ϊ[OrderControls]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderControls_OrderID')
DROP INDEX [IX_OrderControls_OrderID] ON [dbo].[OrderControls]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderControls_OrderItemID')
DROP INDEX [IX_OrderControls_OrderItemID] ON [dbo].[OrderControls]

--Ϊ[OrderControlSteps]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderControlSteps_OrderControlID')
DROP INDEX [IX_OrderControlSteps_OrderControlID] ON [dbo].[OrderControlSteps]

--Ϊ[OrderFiles]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderFiles_OrderID')
DROP INDEX [IX_OrderFiles_OrderID] ON [dbo].[OrderFiles]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderFiles_OrderItemID')
DROP INDEX [IX_OrderFiles_OrderItemID] ON [dbo].[OrderFiles]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderFiles_OrderPremiumID')
DROP INDEX [IX_OrderFiles_OrderPremiumID] ON [dbo].[OrderFiles]

--Ϊ[OrderItemCategories]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderItemCategories_OrderItemID')
DROP INDEX [IX_OrderItemCategories_OrderItemID] ON [dbo].[OrderItemCategories]

--Ϊ[OrderItems]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderItems_OrderID')
DROP INDEX [IX_OrderItems_OrderID] ON [dbo].[OrderItems]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderItems_ProductID')
DROP INDEX [IX_OrderItems_ProductID] ON [dbo].[OrderItems]

--Ϊ[OrderItemTaxes]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderItemTaxes_OrderItemID')
DROP INDEX [IX_OrderItemTaxes_OrderItemID] ON [dbo].[OrderItemTaxes]

--Ϊ[OrderLogs]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderLogs_OrderID')
DROP INDEX [IX_OrderLogs_OrderID] ON [dbo].[OrderLogs]

--Ϊ[OrderPayExchangeSuppliers]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderPayExchangeSuppliers_OrderID')
DROP INDEX [IX_OrderPayExchangeSuppliers_OrderID] ON [dbo].[OrderPayExchangeSuppliers]

--Ϊ[OrderPremiums]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderPremiums_OrderID')
DROP INDEX [IX_OrderPremiums_OrderID] ON [dbo].[OrderPremiums]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderPremiums_OrderItemID')
DROP INDEX [IX_OrderPremiums_OrderItemID] ON [dbo].[OrderPremiums]

--Ϊ[OrderReceipts]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderReceipts_ClientID')
DROP INDEX [IX_OrderReceipts_ClientID] ON [dbo].[OrderReceipts]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderReceipts_OrderID')
DROP INDEX [IX_OrderReceipts_OrderID] ON [dbo].[OrderReceipts]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderReceipts_FeeSourceID')
DROP INDEX [IX_OrderReceipts_FeeSourceID] ON [dbo].[OrderReceipts]

--Ϊ[Orders]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_Orders_ClientID')
DROP INDEX [IX_Orders_ClientID] ON [dbo].[Orders]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_Orders_ClientAgreementID')
DROP INDEX [IX_Orders_ClientAgreementID] ON [dbo].[Orders]

--Ϊ[OrderTraces]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderTraces_OrderID')
DROP INDEX [IX_OrderTraces_OrderID] ON [dbo].[OrderTraces]

--Ϊ[OrderVoyages]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderVoyages_OrderID')
DROP INDEX [IX_OrderVoyages_OrderID] ON [dbo].[OrderVoyages]

--Ϊ[OrderWaybillItems]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderWaybillItems_OrderWaybillID')
DROP INDEX [IX_OrderWaybillItems_OrderWaybillID] ON [dbo].[OrderWaybillItems]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderWaybillItems_SortingID')
DROP INDEX [IX_OrderWaybillItems_SortingID] ON [dbo].[OrderWaybillItems]

--Ϊ[OrderWaybills]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderWaybills_OrderID')
DROP INDEX [IX_OrderWaybills_OrderID] ON [dbo].[OrderWaybills]

--Ϊ[OrderWhesPremiumFiles]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderWhesPremiumFiles_OrderWhesPremiumID')
DROP INDEX [IX_OrderWhesPremiumFiles_OrderWhesPremiumID] ON [dbo].[OrderWhesPremiumFiles]

--Ϊ[OrderWhesPremiumLogs]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderWhesPremiumLogs_OrderID')
DROP INDEX [IX_OrderWhesPremiumLogs_OrderID] ON [dbo].[OrderWhesPremiumLogs]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderWhesPremiumLogs_OrderWhesPremiumID')
DROP INDEX [IX_OrderWhesPremiumLogs_OrderWhesPremiumID] ON [dbo].[OrderWhesPremiumLogs]

--Ϊ[OrderWhesPremiums]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_OrderWhesPremiums_OrderID')
DROP INDEX [IX_OrderWhesPremiums_OrderID] ON [dbo].[OrderWhesPremiums]

--Ϊ[PackingItems]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PackingItems_PackingID')
DROP INDEX [IX_PackingItems_PackingID] ON [dbo].[PackingItems]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PackingItems_SortingID')
DROP INDEX [IX_PackingItems_SortingID] ON [dbo].[PackingItems]

--Ϊ[Packings]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_Packings_OrderID')
DROP INDEX [IX_Packings_OrderID] ON [dbo].[Packings]

--Ϊ[PayExchangeApplies]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PayExchangeApplies_ClientID')
DROP INDEX [IX_PayExchangeApplies_ClientID] ON [dbo].[PayExchangeApplies]

--Ϊ[PayExchangeApplyFiles]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PayExchangeApplyFiles_PayExchangeApplyID')
DROP INDEX [IX_PayExchangeApplyFiles_PayExchangeApplyID] ON [dbo].[PayExchangeApplyFiles]

--Ϊ[PayExchangeApplyItems]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PayExchangeApplyItems_PayExchangeApplyID')
DROP INDEX [IX_PayExchangeApplyItems_PayExchangeApplyID] ON [dbo].[PayExchangeApplyItems]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PayExchangeApplyItems_OrderID')
DROP INDEX [IX_PayExchangeApplyItems_OrderID] ON [dbo].[PayExchangeApplyItems]

--Ϊ[PayExchangeApplyLogs]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PayExchangeApplyLogs_PayExchangeApplyID')
DROP INDEX [IX_PayExchangeApplyLogs_PayExchangeApplyID] ON [dbo].[PayExchangeApplyLogs]

--Ϊ[PaymentApplies]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PaymentApplies_OrderID')
DROP INDEX [IX_PaymentApplies_OrderID] ON [dbo].[PaymentApplies]

--Ϊ[PaymentApplyLogs]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PaymentApplyLogs_PaymentApplyID')
DROP INDEX [IX_PaymentApplyLogs_PaymentApplyID] ON [dbo].[PaymentApplyLogs]

--Ϊ[PaymentNoticeFiles]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PaymentNoticeFiles_PaymentNoticeID')
DROP INDEX [IX_PaymentNoticeFiles_PaymentNoticeID] ON [dbo].[PaymentNoticeFiles]

--Ϊ[PaymentNoticeItems]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PaymentNoticeItems_PaymentNoticeID')
DROP INDEX [IX_PaymentNoticeItems_PaymentNoticeID] ON [dbo].[PaymentNoticeItems]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PaymentNoticeItems_OrderID')
DROP INDEX [IX_PaymentNoticeItems_OrderID] ON [dbo].[PaymentNoticeItems]

--Ϊ[PaymentNotices]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PaymentNotices_PaymentApplyID')
DROP INDEX [IX_PaymentNotices_PaymentApplyID] ON [dbo].[PaymentNotices]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PaymentNotices_PayExchangeApplyID')
DROP INDEX [IX_PaymentNotices_PayExchangeApplyID] ON [dbo].[PaymentNotices]

--Ϊ[PreProductCategories]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PreProductCategories_PreProductID')
DROP INDEX [IX_PreProductCategories_PreProductID] ON [dbo].[PreProductCategories]

--Ϊ[PreProductPostLog]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PreProductPostLog_PreProductCategoryID')
DROP INDEX [IX_PreProductPostLog_PreProductCategoryID] ON [dbo].[PreProductPostLog]

--Ϊ[PreProducts]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_PreProducts_ClientID')
DROP INDEX [IX_PreProducts_ClientID] ON [dbo].[PreProducts]

--Ϊ[ProductCategories]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ProductCategories_Model')
DROP INDEX [IX_ProductCategories_Model] ON [dbo].[ProductCategories]

--Ϊ[ProductClassifyChangeLogs]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ProductClassifyChangeLogs_Model')
DROP INDEX [IX_ProductClassifyChangeLogs_Model] ON [dbo].[ProductClassifyChangeLogs]

--Ϊ[ProductClassifyLogs]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ProductClassifyLogs_OrderItemID')
DROP INDEX [IX_ProductClassifyLogs_OrderItemID] ON [dbo].[ProductClassifyLogs]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ProductClassifyLogs_PreProductID')
DROP INDEX [IX_ProductClassifyLogs_PreProductID] ON [dbo].[ProductClassifyLogs]

--Ϊ[ProductTaxCategories]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ProductTaxCategories_Name')
DROP INDEX [IX_ProductTaxCategories_Name] ON [dbo].[ProductTaxCategories]

--Ϊ[ReceiptNotices]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_ReceiptNotices_ClientID')
DROP INDEX [IX_ReceiptNotices_ClientID] ON [dbo].[ReceiptNotices]

--Ϊ[Sortings]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_Sortings_OrderID')
DROP INDEX [IX_Sortings_OrderID] ON [dbo].[Sortings]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_Sortings_OrderItemID')
DROP INDEX [IX_Sortings_OrderItemID] ON [dbo].[Sortings]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_Sortings_EntryNoticeItemID')
DROP INDEX [IX_Sortings_EntryNoticeItemID] ON [dbo].[Sortings]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_Sortings_ProductID')
DROP INDEX [IX_Sortings_ProductID] ON [dbo].[Sortings]

--Ϊ[StoreStorages]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_StoreStorages_OrderItemID')
DROP INDEX [IX_StoreStorages_OrderItemID] ON [dbo].[StoreStorages]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_StoreStorages_SortingID')
DROP INDEX [IX_StoreStorages_SortingID] ON [dbo].[StoreStorages]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_StoreStorages_ProductID')
DROP INDEX [IX_StoreStorages_ProductID] ON [dbo].[StoreStorages]

--Ϊ[SwapLimitCountryLogs]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_SwapLimitCountryLogs_BankID')
DROP INDEX [IX_SwapLimitCountryLogs_BankID] ON [dbo].[SwapLimitCountryLogs]

--Ϊ[SwapNoticeItems]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_SwapNoticeItems_SwapNoticeID')
DROP INDEX [IX_SwapNoticeItems_SwapNoticeID] ON [dbo].[SwapNoticeItems]
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_SwapNoticeItems_DecHeadID')
DROP INDEX [IX_SwapNoticeItems_DecHeadID] ON [dbo].[SwapNoticeItems]

--Ϊ[TemporaryFiles]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_TemporaryFiles_TemporaryID')
DROP INDEX [IX_TemporaryFiles_TemporaryID] ON [dbo].[TemporaryFiles]

--Ϊ[TemporaryLogs]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_TemporaryLogs_TemporaryID')
DROP INDEX [IX_TemporaryLogs_TemporaryID] ON [dbo].[TemporaryLogs]

--Ϊ[UserLogs]��ɾ������
IF EXISTS(SELECT * FROM SYS.INDEXES WHERE name='IX_UserLogs_UserID')
DROP INDEX [IX_UserLogs_UserID] ON [dbo].[UserLogs]