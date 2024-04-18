USE [XDT.ScCustoms]
GO

--为[AdminRoles]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_AdminRoles_Roles')
ALTER TABLE [dbo].[AdminRoles] DROP CONSTRAINT FK_AdminRoles_Roles;
GO

--为[ApiClient]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ApiClient_Clients')
ALTER TABLE [dbo].[ApiClient] DROP CONSTRAINT FK_ApiClient_Clients;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ApiClient_ClientSuppliers')
ALTER TABLE [dbo].[ApiClient] DROP CONSTRAINT FK_ApiClient_ClientSuppliers;
GO

--为[ClientConsignees]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ClientConsignees_Contacts')
ALTER TABLE [dbo].[ClientConsignees] DROP CONSTRAINT FK_ClientConsignees_Contacts;
GO

--为[CustomsElementsDefaults]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_CustomsElementsDefaults_CustomsTariffs')
ALTER TABLE [dbo].[CustomsElementsDefaults] DROP CONSTRAINT FK_CustomsElementsDefaults_CustomsTariffs;
GO

--为[DecContainers]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecContainers_DecHeads')
ALTER TABLE [dbo].[DecContainers] DROP CONSTRAINT FK_DecContainers_DecHeads;
GO

--为[DecHeadFiles]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecHeadFiles_DecHeads')
ALTER TABLE [dbo].[DecHeadFiles] DROP CONSTRAINT FK_DecHeadFiles_DecHeads;
GO

--为[DecHeads]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecHeads_Voyages')
ALTER TABLE [dbo].[DecHeads] DROP CONSTRAINT FK_DecHeads_Voyages;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecHeads_DeclarationNotices')
ALTER TABLE [dbo].[DecHeads] DROP CONSTRAINT FK_DecHeads_DeclarationNotices;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecHeads_Orders')
ALTER TABLE [dbo].[DecHeads] DROP CONSTRAINT FK_DecHeads_Orders

--为[DecLicenseDocus]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecLicenseDocus_DecHeads')
ALTER TABLE [dbo].[DecLicenseDocus] DROP CONSTRAINT FK_DecLicenseDocus_DecHeads;
GO

--为[DecLists]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecLists_DecHeads')
ALTER TABLE [dbo].[DecLists] DROP CONSTRAINT FK_DecLists_DecHeads;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecLists_DeclarationNoticeItems')
ALTER TABLE [dbo].[DecLists] DROP CONSTRAINT FK_DecLists_DeclarationNoticeItems;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecLists_Orders')
ALTER TABLE [dbo].[DecLists] DROP CONSTRAINT FK_DecLists_Orders;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecLists_OrderItems')
ALTER TABLE [dbo].[DecLists] DROP CONSTRAINT FK_DecLists_OrderItems;
GO

--为[DecOtherPacks]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecOtherPacks_DecHeads')
ALTER TABLE [dbo].[DecOtherPacks] DROP CONSTRAINT FK_DecOtherPacks_DecHeads;
GO

--为[DecRequestCerts]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecRequestCerts_DecHeads')
ALTER TABLE [dbo].[DecRequestCerts] DROP CONSTRAINT FK_DecRequestCerts_DecHeads;
GO

--为[DecTaxFlows]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecTaxFlows_DecTaxs')
ALTER TABLE [dbo].[DecTaxFlows] DROP CONSTRAINT FK_DecTaxFlows_DecTaxs;
GO

--为[DecTaxs]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecTaxs_DecHeads')
ALTER TABLE [dbo].[DecTaxs] DROP CONSTRAINT FK_DecTaxs_DecHeads;
GO

--为[DecTraces]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecTraces_DecHeads')
ALTER TABLE [dbo].[DecTraces] DROP CONSTRAINT FK_DecTraces_DecHeads;
GO

--为[DeliveryConsignees]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DeliveryConsignees_DeliveryNotices')
ALTER TABLE [dbo].[DeliveryConsignees] DROP CONSTRAINT FK_DeliveryConsignees_DeliveryNotices;
GO

--为[DeliveryNoticeLogs]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DeliveryNoticeLogs_DeliveryNotices')
ALTER TABLE [dbo].[DeliveryNoticeLogs] DROP CONSTRAINT FK_DeliveryNoticeLogs_DeliveryNotices;
GO

--为[DeliveryNotices]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DeliveryNotices_Orders')
ALTER TABLE [dbo].[DeliveryNotices] DROP CONSTRAINT FK_DeliveryNotices_Orders;
GO

--为[Drivers]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Drivers_Carriers')
ALTER TABLE [dbo].[Drivers] DROP CONSTRAINT FK_Drivers_Carriers;
GO

--为[EntryNoticeItems]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_EntryNoticeItems_EntryNotices')
ALTER TABLE [dbo].[EntryNoticeItems] DROP CONSTRAINT FK_EntryNoticeItems_EntryNotices;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_EntryNoticeItems_OrderItems')
ALTER TABLE [dbo].[EntryNoticeItems] DROP CONSTRAINT FK_EntryNoticeItems_OrderItems;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_EntryNoticeItems_DecLists')
ALTER TABLE [dbo].[EntryNoticeItems] DROP CONSTRAINT FK_EntryNoticeItems_DecLists;
GO

--为[EntryNoticeLogs]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_EntryNoticeLogs_EntryNotices')
ALTER TABLE [dbo].[EntryNoticeLogs] DROP CONSTRAINT FK_EntryNoticeLogs_EntryNotices;
GO

--为[EntryNotices]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_EntryNotices_Orders')
ALTER TABLE [dbo].[EntryNotices] DROP CONSTRAINT FK_EntryNotices_Orders;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_EntryNotices_DecHeads')
ALTER TABLE [dbo].[EntryNotices] DROP CONSTRAINT FK_EntryNotices_DecHeads;
GO

--为[ExchangeRateLogs]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExchangeRateLogs_ExchangeRates')
ALTER TABLE [dbo].[ExchangeRateLogs] DROP CONSTRAINT FK_ExchangeRateLogs_ExchangeRates;
GO

--为[ExitDelivers]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitDelivers_ExitNotices')
ALTER TABLE [dbo].[ExitDelivers] DROP CONSTRAINT FK_ExitDelivers_ExitNotices;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitDelivers_Consignees')
ALTER TABLE [dbo].[ExitDelivers] DROP CONSTRAINT FK_ExitDelivers_Consignees;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitDelivers_Delivers')
ALTER TABLE [dbo].[ExitDelivers] DROP CONSTRAINT FK_ExitDelivers_Delivers;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitDelivers_Expressages')
ALTER TABLE [dbo].[ExitDelivers] DROP CONSTRAINT FK_ExitDelivers_Expressages;
GO

--为[ExitNoticeFiles]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNoticeFiles_ExitNotices')
ALTER TABLE [dbo].[ExitNoticeFiles] DROP CONSTRAINT FK_ExitNoticeFiles_ExitNotices;
GO

--为[ExitNoticeItems]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNoticeItems_ExitNotices')
ALTER TABLE [dbo].[ExitNoticeItems] DROP CONSTRAINT FK_ExitNoticeItems_ExitNotices;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNoticeItems_DecLists')
ALTER TABLE [dbo].[ExitNoticeItems] DROP CONSTRAINT FK_ExitNoticeItems_DecLists;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNoticeItems_Sortings')
ALTER TABLE [dbo].[ExitNoticeItems] DROP CONSTRAINT FK_ExitNoticeItems_Sortings;
GO

--为[ExitNoticeLogs]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNoticeLogs_ExitNotices')
ALTER TABLE [dbo].[ExitNoticeLogs] DROP CONSTRAINT FK_ExitNoticeLogs_ExitNotices;
GO

--为[ExitNotices]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNotices_Orders')
ALTER TABLE [dbo].[ExitNotices] DROP CONSTRAINT FK_ExitNotices_Orders;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNotices_DecHeads')
ALTER TABLE [dbo].[ExitNotices] DROP CONSTRAINT FK_ExitNotices_DecHeads;
GO

--为[Expressages]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Expressages_ExpressCompanies')
ALTER TABLE [dbo].[Expressages] DROP CONSTRAINT FK_Expressages_ExpressCompanies;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Expressages_ExpressTypes')
ALTER TABLE [dbo].[Expressages] DROP CONSTRAINT FK_Expressages_ExpressTypes;
GO

--为[ExpressTypes]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExpressTypes_ExpressCompanies')
ALTER TABLE [dbo].[ExpressTypes] DROP CONSTRAINT FK_ExpressTypes_ExpressCompanies;
GO

--为[FinanceAccountFlows]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinanceAccountFlows_FinanceVaults')
ALTER TABLE [dbo].[FinanceAccountFlows] DROP CONSTRAINT FK_FinanceAccountFlows_FinanceVaults;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinanceAccountFlows_FinanceAccounts')
ALTER TABLE [dbo].[FinanceAccountFlows] DROP CONSTRAINT FK_FinanceAccountFlows_FinanceAccounts;
GO

--为[FinancePayments]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinancePayments_FinanceVaults')
ALTER TABLE [dbo].[FinancePayments] DROP CONSTRAINT FK_FinancePayments_FinanceVaults;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinancePayments_FinanceAccounts')
ALTER TABLE [dbo].[FinancePayments] DROP CONSTRAINT FK_FinancePayments_FinanceAccounts;
GO

--为[FinanceReceipts]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinanceReceipts_FinanceVaults')
ALTER TABLE [dbo].[FinanceReceipts] DROP CONSTRAINT FK_FinanceReceipts_FinanceVaults;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinanceReceipts_FinanceAccounts')
ALTER TABLE [dbo].[FinanceReceipts] DROP CONSTRAINT FK_FinanceReceipts_FinanceAccounts;
GO

--为[InvoiceNoticeLogs]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_InvoiceNoticeLogs_InvoiceNotices')
ALTER TABLE [dbo].[InvoiceNoticeLogs] DROP CONSTRAINT FK_InvoiceNoticeLogs_InvoiceNotices;
GO

--为[InvoiceNotices]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_InvoiceNotices_Clients')
ALTER TABLE [dbo].[InvoiceNotices] DROP CONSTRAINT FK_InvoiceNotices_Clients;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_InvoiceNotices_ClientInvoices')
ALTER TABLE [dbo].[InvoiceNotices] DROP CONSTRAINT FK_InvoiceNotices_ClientInvoices;
GO

--为[ManifestConsignmentContainers]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ManifestConsignmentContainers_ManifestConsignments')
ALTER TABLE [dbo].[ManifestConsignmentContainers] DROP CONSTRAINT FK_ManifestConsignmentContainers_ManifestConsignments;
GO

--为[ManifestConsignmentItems]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ManifestConsignmentItems_ManifestConsignments')
ALTER TABLE [dbo].[ManifestConsignmentItems] DROP CONSTRAINT FK_ManifestConsignmentItems_ManifestConsignments;
GO

--为[ManifestConsignmentTraces]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ManifestConsignmentTraces_ManifestConsignments')
ALTER TABLE [dbo].[ManifestConsignmentTraces] DROP CONSTRAINT FK_ManifestConsignmentTraces_ManifestConsignments;
GO

--为[OrderControls]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderControls_OrderItems')
ALTER TABLE [dbo].[OrderControls] DROP CONSTRAINT FK_OrderControls_OrderItems;
GO

--为[OrderFiles]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderFiles_OrderItems')
ALTER TABLE [dbo].[OrderFiles] DROP CONSTRAINT FK_OrderFiles_OrderItems;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderFiles_OrderPremiums')
ALTER TABLE [dbo].[OrderFiles] DROP CONSTRAINT FK_OrderFiles_OrderPremiums;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderFiles_Users')
ALTER TABLE [dbo].[OrderFiles] DROP CONSTRAINT FK_OrderFiles_Users;
GO

--为[OrderLogs]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderLogs_Users')
ALTER TABLE [dbo].[OrderLogs] DROP CONSTRAINT FK_OrderLogs_Users;
GO

--为[OrderPayExchangeSuppliers]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderPayExchangeSuppliers_ClientSuppliers')
ALTER TABLE [dbo].[OrderPayExchangeSuppliers] DROP CONSTRAINT FK_OrderPayExchangeSuppliers_ClientSuppliers;
GO

--为[OrderReceipts]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderReceipts_FinanceReceipts')
ALTER TABLE [dbo].[OrderReceipts] DROP CONSTRAINT FK_OrderReceipts_FinanceReceipts;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderReceipts_Clients')
ALTER TABLE [dbo].[OrderReceipts] DROP CONSTRAINT FK_OrderReceipts_Clients;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderReceipts_Orders')
ALTER TABLE [dbo].[OrderReceipts] DROP CONSTRAINT FK_OrderReceipts_Orders;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderReceipts_OrderPremiums')
ALTER TABLE [dbo].[OrderReceipts] DROP CONSTRAINT FK_OrderReceipts_OrderPremiums;
GO

--为[Orders]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Orders_Users')
ALTER TABLE [dbo].[Orders] DROP CONSTRAINT FK_Orders_Users;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Orders_Clients')
ALTER TABLE [dbo].[Orders] DROP CONSTRAINT FK_Orders_Clients;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Orders_ClientAgreements')
ALTER TABLE [dbo].[Orders] DROP CONSTRAINT FK_Orders_ClientAgreements;
GO

--为[OrderVoyages]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderVoyages_Orders')
ALTER TABLE [dbo].[OrderVoyages] DROP CONSTRAINT FK_OrderVoyages_Orders;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderVoyages_Voyages')
ALTER TABLE [dbo].[OrderVoyages] DROP CONSTRAINT FK_OrderVoyages_Voyages;
GO

--为[OrderWaybillItems]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWaybillItems_OrderWaybills')
ALTER TABLE [dbo].[OrderWaybillItems] DROP CONSTRAINT FK_OrderWaybillItems_OrderWaybills;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWaybillItems_Sortings')
ALTER TABLE [dbo].[OrderWaybillItems] DROP CONSTRAINT FK_OrderWaybillItems_Sortings;
GO

--为[OrderWaybills]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWaybills_Orders')
ALTER TABLE [dbo].[OrderWaybills] DROP CONSTRAINT FK_OrderWaybills_Orders;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWaybills_Carriers')
ALTER TABLE [dbo].[OrderWaybills] DROP CONSTRAINT FK_OrderWaybills_Carriers;
GO

--为[OrderWhesPremiumFiles]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWhesPremiumFiles_OrderWhesPremiums')
ALTER TABLE [dbo].[OrderWhesPremiumFiles] DROP CONSTRAINT FK_OrderWhesPremiumFiles_OrderWhesPremiums;
GO

--为[OrderWhesPremiumLogs]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWhesPremiumLogs_Orders')
ALTER TABLE [dbo].[OrderWhesPremiumLogs] DROP CONSTRAINT FK_OrderWhesPremiumLogs_Orders;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWhesPremiumLogs_OrderWhesPremiums')
ALTER TABLE [dbo].[OrderWhesPremiumLogs] DROP CONSTRAINT FK_OrderWhesPremiumLogs_OrderWhesPremiums;
GO

--为[OrderWhesPremiums]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWhesPremiums_Orders')
ALTER TABLE [dbo].[OrderWhesPremiums] DROP CONSTRAINT FK_OrderWhesPremiums_Orders;
GO

--为[PackingItems]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PackingItems_Packings')
ALTER TABLE [dbo].PackingItems DROP CONSTRAINT FK_PackingItems_Packings;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PackingItems_Sortings')
ALTER TABLE [dbo].PackingItems DROP CONSTRAINT FK_PackingItems_Sortings;
GO

--为[Packings]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Packings_Orders')
ALTER TABLE [dbo].Packings DROP CONSTRAINT FK_Packings_Orders;
GO

--为[PayExchangeApplies]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PayExchangeApplies_Clients')
ALTER TABLE [dbo].PayExchangeApplies DROP CONSTRAINT FK_PayExchangeApplies_Clients;
GO

--为[PayExchangeApplyFiles]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PayExchangeApplyFiles_PayExchangeApplies')
ALTER TABLE [dbo].PayExchangeApplyFiles DROP CONSTRAINT FK_PayExchangeApplyFiles_PayExchangeApplies;
GO

--为[PayExchangeApplyItems]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PayExchangeApplyItems_Orders')
ALTER TABLE [dbo].PayExchangeApplyItems DROP CONSTRAINT FK_PayExchangeApplyItems_Orders;
GO

--为[PaymentApplies]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentApplies_Orders')
ALTER TABLE [dbo].PaymentApplies DROP CONSTRAINT FK_PaymentApplies_Orders;
GO

--为[PaymentApplyLogs]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentApplyLogs_PaymentApplies')
ALTER TABLE [dbo].PaymentApplyLogs DROP CONSTRAINT FK_PaymentApplyLogs_PaymentApplies;
GO

--为[PaymentNoticeFiles]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentNoticeFiles_PaymentNotices')
ALTER TABLE [dbo].PaymentNoticeFiles DROP CONSTRAINT FK_PaymentNoticeFiles_PaymentNotices;
GO

--为[PaymentNoticeItems]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentNoticeItems_PaymentNotices')
ALTER TABLE [dbo].PaymentNoticeItems DROP CONSTRAINT FK_PaymentNoticeItems_PaymentNotices;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentNoticeItems_Orders')
ALTER TABLE [dbo].PaymentNoticeItems DROP CONSTRAINT FK_PaymentNoticeItems_Orders;
GO

--为[PaymentNotices]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentNotices_PaymentApplies')
ALTER TABLE [dbo].PaymentNotices DROP CONSTRAINT FK_PaymentNotices_PaymentApplies;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentNotices_PayExchangeApplies')
ALTER TABLE [dbo].PaymentNotices DROP CONSTRAINT FK_PaymentNotices_PayExchangeApplies;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentNotices_FinanceVaults')
ALTER TABLE [dbo].PaymentNotices DROP CONSTRAINT FK_PaymentNotices_FinanceVaults;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentNotices_FinanceAccounts')
ALTER TABLE [dbo].PaymentNotices DROP CONSTRAINT FK_PaymentNotices_FinanceAccounts;
GO

--为[PreProductCategories]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PreProductCategories_PreProducts')
ALTER TABLE [dbo].PreProductCategories DROP CONSTRAINT FK_PreProductCategories_PreProducts;
GO

--为[PreProductPostLog]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PreProductPostLog_PreProductCategories')
ALTER TABLE [dbo].PreProductPostLog DROP CONSTRAINT FK_PreProductPostLog_PreProductCategories;
GO

--为[PreProducts]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PreProducts_Clients')
ALTER TABLE [dbo].PreProducts DROP CONSTRAINT FK_PreProducts_Clients;
GO

--为[ProductClassifyLogs]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ProductClassifyLogs_OrderItems')
ALTER TABLE [dbo].ProductClassifyLogs DROP CONSTRAINT FK_ProductClassifyLogs_OrderItems;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ProductClassifyLogs_PreProducts')
ALTER TABLE [dbo].ProductClassifyLogs DROP CONSTRAINT FK_ProductClassifyLogs_PreProducts;
GO

--为[ReceiptNotices]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ReceiptNotices_Clients')
ALTER TABLE [dbo].ReceiptNotices DROP CONSTRAINT FK_ReceiptNotices_Clients;
GO

--为[Sortings]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Sortings_Orders')
ALTER TABLE [dbo].Sortings DROP CONSTRAINT FK_Sortings_Orders;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Sortings_OrderItems')
ALTER TABLE [dbo].Sortings DROP CONSTRAINT FK_Sortings_OrderItems;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Sortings_EntryNoticeItems')
ALTER TABLE [dbo].Sortings DROP CONSTRAINT FK_Sortings_EntryNoticeItems;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Sortings_Products')
ALTER TABLE [dbo].Sortings DROP CONSTRAINT FK_Sortings_Products;
GO

--为[StoreStorages]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_StoreStorages_OrderItems')
ALTER TABLE [dbo].StoreStorages DROP CONSTRAINT FK_StoreStorages_OrderItems;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_StoreStorages_Sortings')
ALTER TABLE [dbo].StoreStorages DROP CONSTRAINT FK_StoreStorages_Sortings;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_StoreStorages_Products')
ALTER TABLE [dbo].StoreStorages DROP CONSTRAINT FK_StoreStorages_Products;
GO

--为[SwapLimitCountries]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_SwapLimitCountries_SwapBanks')
ALTER TABLE [dbo].SwapLimitCountries DROP CONSTRAINT FK_SwapLimitCountries_SwapBanks;
GO

--为[SwapLimitCountryLogs]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_SwapLimitCountryLogs_SwapBanks')
ALTER TABLE [dbo].SwapLimitCountryLogs DROP CONSTRAINT FK_SwapLimitCountryLogs_SwapBanks;
GO

--为[SwapNoticeItems]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_SwapNoticeItems_SwapNotices')
ALTER TABLE [dbo].SwapNoticeItems DROP CONSTRAINT FK_SwapNoticeItems_SwapNotices;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_SwapNoticeItems_DecHeads')
ALTER TABLE [dbo].SwapNoticeItems DROP CONSTRAINT FK_SwapNoticeItems_DecHeads;
GO

--为[SwapNotices]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_SwapNotices_InFinanceAccounts')
ALTER TABLE [dbo].SwapNotices DROP CONSTRAINT FK_SwapNotices_InFinanceAccounts;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_SwapNotices_OutFinanceAccounts')
ALTER TABLE [dbo].SwapNotices DROP CONSTRAINT FK_SwapNotices_OutFinanceAccounts;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_SwapNotices_MidFinanceAccounts')
ALTER TABLE [dbo].SwapNotices DROP CONSTRAINT FK_SwapNotices_MidFinanceAccounts;
GO

--为[TemporaryFiles]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_TemporaryFiles_Temporarys')
ALTER TABLE [dbo].TemporaryFiles DROP CONSTRAINT FK_TemporaryFiles_Temporarys;
GO

--为[TemporaryLogs]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_TemporaryLogs_Temporarys')
ALTER TABLE [dbo].TemporaryLogs DROP CONSTRAINT FK_TemporaryLogs_Temporarys;
GO

--为[Vehicles]表删除外键约束
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Vehicles_Carriers')
ALTER TABLE [dbo].Vehicles DROP CONSTRAINT FK_Vehicles_Carriers;
GO