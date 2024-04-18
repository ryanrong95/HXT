USE [XDT.ScCustoms]
GO

--Ϊ[AdminRoles]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_AdminRoles_Roles')
ALTER TABLE [dbo].[AdminRoles] DROP CONSTRAINT FK_AdminRoles_Roles;
GO

--Ϊ[ApiClient]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ApiClient_Clients')
ALTER TABLE [dbo].[ApiClient] DROP CONSTRAINT FK_ApiClient_Clients;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ApiClient_ClientSuppliers')
ALTER TABLE [dbo].[ApiClient] DROP CONSTRAINT FK_ApiClient_ClientSuppliers;
GO

--Ϊ[ClientConsignees]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ClientConsignees_Contacts')
ALTER TABLE [dbo].[ClientConsignees] DROP CONSTRAINT FK_ClientConsignees_Contacts;
GO

--Ϊ[CustomsElementsDefaults]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_CustomsElementsDefaults_CustomsTariffs')
ALTER TABLE [dbo].[CustomsElementsDefaults] DROP CONSTRAINT FK_CustomsElementsDefaults_CustomsTariffs;
GO

--Ϊ[DecContainers]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecContainers_DecHeads')
ALTER TABLE [dbo].[DecContainers] DROP CONSTRAINT FK_DecContainers_DecHeads;
GO

--Ϊ[DecHeadFiles]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecHeadFiles_DecHeads')
ALTER TABLE [dbo].[DecHeadFiles] DROP CONSTRAINT FK_DecHeadFiles_DecHeads;
GO

--Ϊ[DecHeads]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecHeads_Voyages')
ALTER TABLE [dbo].[DecHeads] DROP CONSTRAINT FK_DecHeads_Voyages;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecHeads_DeclarationNotices')
ALTER TABLE [dbo].[DecHeads] DROP CONSTRAINT FK_DecHeads_DeclarationNotices;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecHeads_Orders')
ALTER TABLE [dbo].[DecHeads] DROP CONSTRAINT FK_DecHeads_Orders

--Ϊ[DecLicenseDocus]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecLicenseDocus_DecHeads')
ALTER TABLE [dbo].[DecLicenseDocus] DROP CONSTRAINT FK_DecLicenseDocus_DecHeads;
GO

--Ϊ[DecLists]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecLists_DecHeads')
ALTER TABLE [dbo].[DecLists] DROP CONSTRAINT FK_DecLists_DecHeads;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecLists_DeclarationNoticeItems')
ALTER TABLE [dbo].[DecLists] DROP CONSTRAINT FK_DecLists_DeclarationNoticeItems;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecLists_Orders')
ALTER TABLE [dbo].[DecLists] DROP CONSTRAINT FK_DecLists_Orders;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecLists_OrderItems')
ALTER TABLE [dbo].[DecLists] DROP CONSTRAINT FK_DecLists_OrderItems;
GO

--Ϊ[DecOtherPacks]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecOtherPacks_DecHeads')
ALTER TABLE [dbo].[DecOtherPacks] DROP CONSTRAINT FK_DecOtherPacks_DecHeads;
GO

--Ϊ[DecRequestCerts]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecRequestCerts_DecHeads')
ALTER TABLE [dbo].[DecRequestCerts] DROP CONSTRAINT FK_DecRequestCerts_DecHeads;
GO

--Ϊ[DecTaxFlows]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecTaxFlows_DecTaxs')
ALTER TABLE [dbo].[DecTaxFlows] DROP CONSTRAINT FK_DecTaxFlows_DecTaxs;
GO

--Ϊ[DecTaxs]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecTaxs_DecHeads')
ALTER TABLE [dbo].[DecTaxs] DROP CONSTRAINT FK_DecTaxs_DecHeads;
GO

--Ϊ[DecTraces]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecTraces_DecHeads')
ALTER TABLE [dbo].[DecTraces] DROP CONSTRAINT FK_DecTraces_DecHeads;
GO

--Ϊ[DeliveryConsignees]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DeliveryConsignees_DeliveryNotices')
ALTER TABLE [dbo].[DeliveryConsignees] DROP CONSTRAINT FK_DeliveryConsignees_DeliveryNotices;
GO

--Ϊ[DeliveryNoticeLogs]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DeliveryNoticeLogs_DeliveryNotices')
ALTER TABLE [dbo].[DeliveryNoticeLogs] DROP CONSTRAINT FK_DeliveryNoticeLogs_DeliveryNotices;
GO

--Ϊ[DeliveryNotices]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_DeliveryNotices_Orders')
ALTER TABLE [dbo].[DeliveryNotices] DROP CONSTRAINT FK_DeliveryNotices_Orders;
GO

--Ϊ[Drivers]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Drivers_Carriers')
ALTER TABLE [dbo].[Drivers] DROP CONSTRAINT FK_Drivers_Carriers;
GO

--Ϊ[EntryNoticeItems]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_EntryNoticeItems_EntryNotices')
ALTER TABLE [dbo].[EntryNoticeItems] DROP CONSTRAINT FK_EntryNoticeItems_EntryNotices;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_EntryNoticeItems_OrderItems')
ALTER TABLE [dbo].[EntryNoticeItems] DROP CONSTRAINT FK_EntryNoticeItems_OrderItems;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_EntryNoticeItems_DecLists')
ALTER TABLE [dbo].[EntryNoticeItems] DROP CONSTRAINT FK_EntryNoticeItems_DecLists;
GO

--Ϊ[EntryNoticeLogs]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_EntryNoticeLogs_EntryNotices')
ALTER TABLE [dbo].[EntryNoticeLogs] DROP CONSTRAINT FK_EntryNoticeLogs_EntryNotices;
GO

--Ϊ[EntryNotices]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_EntryNotices_Orders')
ALTER TABLE [dbo].[EntryNotices] DROP CONSTRAINT FK_EntryNotices_Orders;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_EntryNotices_DecHeads')
ALTER TABLE [dbo].[EntryNotices] DROP CONSTRAINT FK_EntryNotices_DecHeads;
GO

--Ϊ[ExchangeRateLogs]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExchangeRateLogs_ExchangeRates')
ALTER TABLE [dbo].[ExchangeRateLogs] DROP CONSTRAINT FK_ExchangeRateLogs_ExchangeRates;
GO

--Ϊ[ExitDelivers]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitDelivers_ExitNotices')
ALTER TABLE [dbo].[ExitDelivers] DROP CONSTRAINT FK_ExitDelivers_ExitNotices;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitDelivers_Consignees')
ALTER TABLE [dbo].[ExitDelivers] DROP CONSTRAINT FK_ExitDelivers_Consignees;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitDelivers_Delivers')
ALTER TABLE [dbo].[ExitDelivers] DROP CONSTRAINT FK_ExitDelivers_Delivers;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitDelivers_Expressages')
ALTER TABLE [dbo].[ExitDelivers] DROP CONSTRAINT FK_ExitDelivers_Expressages;
GO

--Ϊ[ExitNoticeFiles]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNoticeFiles_ExitNotices')
ALTER TABLE [dbo].[ExitNoticeFiles] DROP CONSTRAINT FK_ExitNoticeFiles_ExitNotices;
GO

--Ϊ[ExitNoticeItems]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNoticeItems_ExitNotices')
ALTER TABLE [dbo].[ExitNoticeItems] DROP CONSTRAINT FK_ExitNoticeItems_ExitNotices;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNoticeItems_DecLists')
ALTER TABLE [dbo].[ExitNoticeItems] DROP CONSTRAINT FK_ExitNoticeItems_DecLists;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNoticeItems_Sortings')
ALTER TABLE [dbo].[ExitNoticeItems] DROP CONSTRAINT FK_ExitNoticeItems_Sortings;
GO

--Ϊ[ExitNoticeLogs]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNoticeLogs_ExitNotices')
ALTER TABLE [dbo].[ExitNoticeLogs] DROP CONSTRAINT FK_ExitNoticeLogs_ExitNotices;
GO

--Ϊ[ExitNotices]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNotices_Orders')
ALTER TABLE [dbo].[ExitNotices] DROP CONSTRAINT FK_ExitNotices_Orders;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNotices_DecHeads')
ALTER TABLE [dbo].[ExitNotices] DROP CONSTRAINT FK_ExitNotices_DecHeads;
GO

--Ϊ[Expressages]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Expressages_ExpressCompanies')
ALTER TABLE [dbo].[Expressages] DROP CONSTRAINT FK_Expressages_ExpressCompanies;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Expressages_ExpressTypes')
ALTER TABLE [dbo].[Expressages] DROP CONSTRAINT FK_Expressages_ExpressTypes;
GO

--Ϊ[ExpressTypes]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExpressTypes_ExpressCompanies')
ALTER TABLE [dbo].[ExpressTypes] DROP CONSTRAINT FK_ExpressTypes_ExpressCompanies;
GO

--Ϊ[FinanceAccountFlows]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinanceAccountFlows_FinanceVaults')
ALTER TABLE [dbo].[FinanceAccountFlows] DROP CONSTRAINT FK_FinanceAccountFlows_FinanceVaults;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinanceAccountFlows_FinanceAccounts')
ALTER TABLE [dbo].[FinanceAccountFlows] DROP CONSTRAINT FK_FinanceAccountFlows_FinanceAccounts;
GO

--Ϊ[FinancePayments]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinancePayments_FinanceVaults')
ALTER TABLE [dbo].[FinancePayments] DROP CONSTRAINT FK_FinancePayments_FinanceVaults;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinancePayments_FinanceAccounts')
ALTER TABLE [dbo].[FinancePayments] DROP CONSTRAINT FK_FinancePayments_FinanceAccounts;
GO

--Ϊ[FinanceReceipts]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinanceReceipts_FinanceVaults')
ALTER TABLE [dbo].[FinanceReceipts] DROP CONSTRAINT FK_FinanceReceipts_FinanceVaults;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinanceReceipts_FinanceAccounts')
ALTER TABLE [dbo].[FinanceReceipts] DROP CONSTRAINT FK_FinanceReceipts_FinanceAccounts;
GO

--Ϊ[InvoiceNoticeLogs]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_InvoiceNoticeLogs_InvoiceNotices')
ALTER TABLE [dbo].[InvoiceNoticeLogs] DROP CONSTRAINT FK_InvoiceNoticeLogs_InvoiceNotices;
GO

--Ϊ[InvoiceNotices]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_InvoiceNotices_Clients')
ALTER TABLE [dbo].[InvoiceNotices] DROP CONSTRAINT FK_InvoiceNotices_Clients;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_InvoiceNotices_ClientInvoices')
ALTER TABLE [dbo].[InvoiceNotices] DROP CONSTRAINT FK_InvoiceNotices_ClientInvoices;
GO

--Ϊ[ManifestConsignmentContainers]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ManifestConsignmentContainers_ManifestConsignments')
ALTER TABLE [dbo].[ManifestConsignmentContainers] DROP CONSTRAINT FK_ManifestConsignmentContainers_ManifestConsignments;
GO

--Ϊ[ManifestConsignmentItems]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ManifestConsignmentItems_ManifestConsignments')
ALTER TABLE [dbo].[ManifestConsignmentItems] DROP CONSTRAINT FK_ManifestConsignmentItems_ManifestConsignments;
GO

--Ϊ[ManifestConsignmentTraces]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ManifestConsignmentTraces_ManifestConsignments')
ALTER TABLE [dbo].[ManifestConsignmentTraces] DROP CONSTRAINT FK_ManifestConsignmentTraces_ManifestConsignments;
GO

--Ϊ[OrderControls]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderControls_OrderItems')
ALTER TABLE [dbo].[OrderControls] DROP CONSTRAINT FK_OrderControls_OrderItems;
GO

--Ϊ[OrderFiles]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderFiles_OrderItems')
ALTER TABLE [dbo].[OrderFiles] DROP CONSTRAINT FK_OrderFiles_OrderItems;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderFiles_OrderPremiums')
ALTER TABLE [dbo].[OrderFiles] DROP CONSTRAINT FK_OrderFiles_OrderPremiums;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderFiles_Users')
ALTER TABLE [dbo].[OrderFiles] DROP CONSTRAINT FK_OrderFiles_Users;
GO

--Ϊ[OrderLogs]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderLogs_Users')
ALTER TABLE [dbo].[OrderLogs] DROP CONSTRAINT FK_OrderLogs_Users;
GO

--Ϊ[OrderPayExchangeSuppliers]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderPayExchangeSuppliers_ClientSuppliers')
ALTER TABLE [dbo].[OrderPayExchangeSuppliers] DROP CONSTRAINT FK_OrderPayExchangeSuppliers_ClientSuppliers;
GO

--Ϊ[OrderReceipts]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderReceipts_FinanceReceipts')
ALTER TABLE [dbo].[OrderReceipts] DROP CONSTRAINT FK_OrderReceipts_FinanceReceipts;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderReceipts_Clients')
ALTER TABLE [dbo].[OrderReceipts] DROP CONSTRAINT FK_OrderReceipts_Clients;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderReceipts_Orders')
ALTER TABLE [dbo].[OrderReceipts] DROP CONSTRAINT FK_OrderReceipts_Orders;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderReceipts_OrderPremiums')
ALTER TABLE [dbo].[OrderReceipts] DROP CONSTRAINT FK_OrderReceipts_OrderPremiums;
GO

--Ϊ[Orders]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Orders_Users')
ALTER TABLE [dbo].[Orders] DROP CONSTRAINT FK_Orders_Users;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Orders_Clients')
ALTER TABLE [dbo].[Orders] DROP CONSTRAINT FK_Orders_Clients;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Orders_ClientAgreements')
ALTER TABLE [dbo].[Orders] DROP CONSTRAINT FK_Orders_ClientAgreements;
GO

--Ϊ[OrderVoyages]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderVoyages_Orders')
ALTER TABLE [dbo].[OrderVoyages] DROP CONSTRAINT FK_OrderVoyages_Orders;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderVoyages_Voyages')
ALTER TABLE [dbo].[OrderVoyages] DROP CONSTRAINT FK_OrderVoyages_Voyages;
GO

--Ϊ[OrderWaybillItems]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWaybillItems_OrderWaybills')
ALTER TABLE [dbo].[OrderWaybillItems] DROP CONSTRAINT FK_OrderWaybillItems_OrderWaybills;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWaybillItems_Sortings')
ALTER TABLE [dbo].[OrderWaybillItems] DROP CONSTRAINT FK_OrderWaybillItems_Sortings;
GO

--Ϊ[OrderWaybills]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWaybills_Orders')
ALTER TABLE [dbo].[OrderWaybills] DROP CONSTRAINT FK_OrderWaybills_Orders;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWaybills_Carriers')
ALTER TABLE [dbo].[OrderWaybills] DROP CONSTRAINT FK_OrderWaybills_Carriers;
GO

--Ϊ[OrderWhesPremiumFiles]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWhesPremiumFiles_OrderWhesPremiums')
ALTER TABLE [dbo].[OrderWhesPremiumFiles] DROP CONSTRAINT FK_OrderWhesPremiumFiles_OrderWhesPremiums;
GO

--Ϊ[OrderWhesPremiumLogs]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWhesPremiumLogs_Orders')
ALTER TABLE [dbo].[OrderWhesPremiumLogs] DROP CONSTRAINT FK_OrderWhesPremiumLogs_Orders;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWhesPremiumLogs_OrderWhesPremiums')
ALTER TABLE [dbo].[OrderWhesPremiumLogs] DROP CONSTRAINT FK_OrderWhesPremiumLogs_OrderWhesPremiums;
GO

--Ϊ[OrderWhesPremiums]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWhesPremiums_Orders')
ALTER TABLE [dbo].[OrderWhesPremiums] DROP CONSTRAINT FK_OrderWhesPremiums_Orders;
GO

--Ϊ[PackingItems]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PackingItems_Packings')
ALTER TABLE [dbo].PackingItems DROP CONSTRAINT FK_PackingItems_Packings;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PackingItems_Sortings')
ALTER TABLE [dbo].PackingItems DROP CONSTRAINT FK_PackingItems_Sortings;
GO

--Ϊ[Packings]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Packings_Orders')
ALTER TABLE [dbo].Packings DROP CONSTRAINT FK_Packings_Orders;
GO

--Ϊ[PayExchangeApplies]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PayExchangeApplies_Clients')
ALTER TABLE [dbo].PayExchangeApplies DROP CONSTRAINT FK_PayExchangeApplies_Clients;
GO

--Ϊ[PayExchangeApplyFiles]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PayExchangeApplyFiles_PayExchangeApplies')
ALTER TABLE [dbo].PayExchangeApplyFiles DROP CONSTRAINT FK_PayExchangeApplyFiles_PayExchangeApplies;
GO

--Ϊ[PayExchangeApplyItems]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PayExchangeApplyItems_Orders')
ALTER TABLE [dbo].PayExchangeApplyItems DROP CONSTRAINT FK_PayExchangeApplyItems_Orders;
GO

--Ϊ[PaymentApplies]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentApplies_Orders')
ALTER TABLE [dbo].PaymentApplies DROP CONSTRAINT FK_PaymentApplies_Orders;
GO

--Ϊ[PaymentApplyLogs]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentApplyLogs_PaymentApplies')
ALTER TABLE [dbo].PaymentApplyLogs DROP CONSTRAINT FK_PaymentApplyLogs_PaymentApplies;
GO

--Ϊ[PaymentNoticeFiles]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentNoticeFiles_PaymentNotices')
ALTER TABLE [dbo].PaymentNoticeFiles DROP CONSTRAINT FK_PaymentNoticeFiles_PaymentNotices;
GO

--Ϊ[PaymentNoticeItems]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentNoticeItems_PaymentNotices')
ALTER TABLE [dbo].PaymentNoticeItems DROP CONSTRAINT FK_PaymentNoticeItems_PaymentNotices;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentNoticeItems_Orders')
ALTER TABLE [dbo].PaymentNoticeItems DROP CONSTRAINT FK_PaymentNoticeItems_Orders;
GO

--Ϊ[PaymentNotices]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentNotices_PaymentApplies')
ALTER TABLE [dbo].PaymentNotices DROP CONSTRAINT FK_PaymentNotices_PaymentApplies;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentNotices_PayExchangeApplies')
ALTER TABLE [dbo].PaymentNotices DROP CONSTRAINT FK_PaymentNotices_PayExchangeApplies;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentNotices_FinanceVaults')
ALTER TABLE [dbo].PaymentNotices DROP CONSTRAINT FK_PaymentNotices_FinanceVaults;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentNotices_FinanceAccounts')
ALTER TABLE [dbo].PaymentNotices DROP CONSTRAINT FK_PaymentNotices_FinanceAccounts;
GO

--Ϊ[PreProductCategories]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PreProductCategories_PreProducts')
ALTER TABLE [dbo].PreProductCategories DROP CONSTRAINT FK_PreProductCategories_PreProducts;
GO

--Ϊ[PreProductPostLog]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PreProductPostLog_PreProductCategories')
ALTER TABLE [dbo].PreProductPostLog DROP CONSTRAINT FK_PreProductPostLog_PreProductCategories;
GO

--Ϊ[PreProducts]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_PreProducts_Clients')
ALTER TABLE [dbo].PreProducts DROP CONSTRAINT FK_PreProducts_Clients;
GO

--Ϊ[ProductClassifyLogs]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ProductClassifyLogs_OrderItems')
ALTER TABLE [dbo].ProductClassifyLogs DROP CONSTRAINT FK_ProductClassifyLogs_OrderItems;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ProductClassifyLogs_PreProducts')
ALTER TABLE [dbo].ProductClassifyLogs DROP CONSTRAINT FK_ProductClassifyLogs_PreProducts;
GO

--Ϊ[ReceiptNotices]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_ReceiptNotices_Clients')
ALTER TABLE [dbo].ReceiptNotices DROP CONSTRAINT FK_ReceiptNotices_Clients;
GO

--Ϊ[Sortings]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Sortings_Orders')
ALTER TABLE [dbo].Sortings DROP CONSTRAINT FK_Sortings_Orders;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Sortings_OrderItems')
ALTER TABLE [dbo].Sortings DROP CONSTRAINT FK_Sortings_OrderItems;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Sortings_EntryNoticeItems')
ALTER TABLE [dbo].Sortings DROP CONSTRAINT FK_Sortings_EntryNoticeItems;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Sortings_Products')
ALTER TABLE [dbo].Sortings DROP CONSTRAINT FK_Sortings_Products;
GO

--Ϊ[StoreStorages]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_StoreStorages_OrderItems')
ALTER TABLE [dbo].StoreStorages DROP CONSTRAINT FK_StoreStorages_OrderItems;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_StoreStorages_Sortings')
ALTER TABLE [dbo].StoreStorages DROP CONSTRAINT FK_StoreStorages_Sortings;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_StoreStorages_Products')
ALTER TABLE [dbo].StoreStorages DROP CONSTRAINT FK_StoreStorages_Products;
GO

--Ϊ[SwapLimitCountries]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_SwapLimitCountries_SwapBanks')
ALTER TABLE [dbo].SwapLimitCountries DROP CONSTRAINT FK_SwapLimitCountries_SwapBanks;
GO

--Ϊ[SwapLimitCountryLogs]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_SwapLimitCountryLogs_SwapBanks')
ALTER TABLE [dbo].SwapLimitCountryLogs DROP CONSTRAINT FK_SwapLimitCountryLogs_SwapBanks;
GO

--Ϊ[SwapNoticeItems]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_SwapNoticeItems_SwapNotices')
ALTER TABLE [dbo].SwapNoticeItems DROP CONSTRAINT FK_SwapNoticeItems_SwapNotices;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_SwapNoticeItems_DecHeads')
ALTER TABLE [dbo].SwapNoticeItems DROP CONSTRAINT FK_SwapNoticeItems_DecHeads;
GO

--Ϊ[SwapNotices]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_SwapNotices_InFinanceAccounts')
ALTER TABLE [dbo].SwapNotices DROP CONSTRAINT FK_SwapNotices_InFinanceAccounts;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_SwapNotices_OutFinanceAccounts')
ALTER TABLE [dbo].SwapNotices DROP CONSTRAINT FK_SwapNotices_OutFinanceAccounts;
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_SwapNotices_MidFinanceAccounts')
ALTER TABLE [dbo].SwapNotices DROP CONSTRAINT FK_SwapNotices_MidFinanceAccounts;
GO

--Ϊ[TemporaryFiles]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_TemporaryFiles_Temporarys')
ALTER TABLE [dbo].TemporaryFiles DROP CONSTRAINT FK_TemporaryFiles_Temporarys;
GO

--Ϊ[TemporaryLogs]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_TemporaryLogs_Temporarys')
ALTER TABLE [dbo].TemporaryLogs DROP CONSTRAINT FK_TemporaryLogs_Temporarys;
GO

--Ϊ[Vehicles]��ɾ�����Լ��
IF EXISTS(SELECT * FROM sysobjects WHERE name='FK_Vehicles_Carriers')
ALTER TABLE [dbo].Vehicles DROP CONSTRAINT FK_Vehicles_Carriers;
GO