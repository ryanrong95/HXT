USE [ScCustoms]
GO

--Ϊ[ApiClient]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_ApiClient_Clients')
ALTER TABLE [dbo].[ApiClient] ADD CONSTRAINT FK_ApiClient_Clients FOREIGN KEY(ClientID) REFERENCES Clients(ID);
GO

--Ϊ[CustomsElementsDefaults]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_CustomsElementsDefaults_CustomsTariffs')
ALTER TABLE [dbo].[CustomsElementsDefaults] ADD CONSTRAINT FK_CustomsElementsDefaults_CustomsTariffs FOREIGN KEY(CustomsTariffID) REFERENCES CustomsTariffs(ID);
GO

--Ϊ[DecContainers]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecContainers_DecHeads')
ALTER TABLE [dbo].[DecContainers] ADD CONSTRAINT FK_DecContainers_DecHeads FOREIGN KEY(DeclarationID) REFERENCES DecHeads(ID);
GO

--Ϊ[DecHeadFiles]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecHeadFiles_DecHeads')
ALTER TABLE [dbo].[DecHeadFiles] ADD CONSTRAINT FK_DecHeadFiles_DecHeads FOREIGN KEY(DecHeadID) REFERENCES DecHeads(ID);
GO

--Ϊ[DecHeads]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecHeads_DeclarationNotices')
ALTER TABLE [dbo].[DecHeads] ADD CONSTRAINT FK_DecHeads_DeclarationNotices FOREIGN KEY(DeclarationNoticeID) REFERENCES DeclarationNotices(ID);
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecHeads_Orders')
ALTER TABLE [dbo].[DecHeads] ADD CONSTRAINT FK_DecHeads_Orders FOREIGN KEY(OrderID) REFERENCES Orders(ID);
GO

--Ϊ[DecLicenseDocus]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecLicenseDocus_DecHeads')
ALTER TABLE [dbo].[DecLicenseDocus] ADD CONSTRAINT FK_DecLicenseDocus_DecHeads FOREIGN KEY(DeclarationID) REFERENCES DecHeads(ID);
GO

--Ϊ[DecLists]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecLists_DecHeads')
ALTER TABLE [dbo].[DecLists] ADD CONSTRAINT FK_DecLists_DecHeads FOREIGN KEY(DeclarationID) REFERENCES DecHeads(ID);
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecLists_Orders')
ALTER TABLE [dbo].[DecLists] ADD CONSTRAINT FK_DecLists_Orders FOREIGN KEY(OrderID) REFERENCES Orders(ID);
GO

--Ϊ[DecOtherPacks]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecOtherPacks_DecHeads')
ALTER TABLE [dbo].[DecOtherPacks] ADD CONSTRAINT FK_DecOtherPacks_DecHeads FOREIGN KEY(DeclarationID) REFERENCES DecHeads(ID);
GO

--Ϊ[DecRequestCerts]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecRequestCerts_DecHeads')
ALTER TABLE [dbo].[DecRequestCerts] ADD CONSTRAINT FK_DecRequestCerts_DecHeads FOREIGN KEY(DeclarationID) REFERENCES DecHeads(ID);
GO

--Ϊ[DecTaxFlows]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecTaxFlows_DecTaxs')
ALTER TABLE [dbo].[DecTaxFlows] ADD CONSTRAINT FK_DecTaxFlows_DecTaxs FOREIGN KEY(DecTaxID) REFERENCES DecTaxs(ID);
GO

--Ϊ[DecTaxs]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecTaxs_DecHeads')
ALTER TABLE [dbo].[DecTaxs] ADD CONSTRAINT FK_DecTaxs_DecHeads FOREIGN KEY(ID) REFERENCES DecHeads(ID);
GO

--Ϊ[DecTraces]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_DecTraces_DecHeads')
ALTER TABLE [dbo].[DecTraces] ADD CONSTRAINT FK_DecTraces_DecHeads FOREIGN KEY(DeclarationID) REFERENCES DecHeads(ID);
GO

--Ϊ[DeliveryConsignees]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_DeliveryConsignees_DeliveryNotices')
ALTER TABLE [dbo].[DeliveryConsignees] ADD CONSTRAINT FK_DeliveryConsignees_DeliveryNotices FOREIGN KEY(DeliveryNoticeID) REFERENCES DeliveryNotices(ID);
GO

--Ϊ[DeliveryNotices]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_DeliveryNotices_Orders')
ALTER TABLE [dbo].[DeliveryNotices] ADD CONSTRAINT FK_DeliveryNotices_Orders FOREIGN KEY(OrderID) REFERENCES Orders(ID);
GO

--Ϊ[Drivers]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_Drivers_Carriers')
ALTER TABLE [dbo].[Drivers] ADD CONSTRAINT FK_Drivers_Carriers FOREIGN KEY(CarrierID) REFERENCES Carriers(ID);
GO

--Ϊ[EntryNoticeItems]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_EntryNoticeItems_EntryNotices')
ALTER TABLE [dbo].[EntryNoticeItems] ADD CONSTRAINT FK_EntryNoticeItems_EntryNotices FOREIGN KEY(EntryNoticeID) REFERENCES EntryNotices(ID);
GO

--Ϊ[EntryNotices]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_EntryNotices_Orders')
ALTER TABLE [dbo].[EntryNotices] ADD CONSTRAINT FK_EntryNotices_Orders FOREIGN KEY(OrderID) REFERENCES Orders(ID);
GO

--Ϊ[ExitDelivers]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitDelivers_ExitNotices')
ALTER TABLE [dbo].[ExitDelivers] ADD CONSTRAINT FK_ExitDelivers_ExitNotices FOREIGN KEY(ExitNoticeID) REFERENCES ExitNotices(ID);
GO

--Ϊ[ExitNoticeFiles]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNoticeFiles_ExitNotices')
ALTER TABLE [dbo].[ExitNoticeFiles] ADD CONSTRAINT FK_ExitNoticeFiles_ExitNotices FOREIGN KEY(ExitNoticeID) REFERENCES ExitNotices(ID);
GO

--Ϊ[ExitNoticeItems]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNoticeItems_ExitNotices')
ALTER TABLE [dbo].[ExitNoticeItems] ADD CONSTRAINT FK_ExitNoticeItems_ExitNotices FOREIGN KEY(ExitNoticeID) REFERENCES ExitNotices(ID);
GO

--Ϊ[ExitNotices]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExitNotices_Orders')
ALTER TABLE [dbo].[ExitNotices] ADD CONSTRAINT FK_ExitNotices_Orders FOREIGN KEY(OrderID) REFERENCES Orders(ID);
GO

--Ϊ[Expressages]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_Expressages_ExpressCompanies')
ALTER TABLE [dbo].[Expressages] ADD CONSTRAINT FK_Expressages_ExpressCompanies FOREIGN KEY(ExpressCompanyID) REFERENCES ExpressCompanys(ID);
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_Expressages_ExpressTypes')
ALTER TABLE [dbo].[Expressages] ADD CONSTRAINT FK_Expressages_ExpressTypes FOREIGN KEY(ExpressTypeID) REFERENCES ExpressTypes(ID);
GO

--Ϊ[ExpressTypes]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_ExpressTypes_ExpressCompanies')
ALTER TABLE [dbo].[ExpressTypes] ADD CONSTRAINT FK_ExpressTypes_ExpressCompanies FOREIGN KEY(ExpressCompanyID) REFERENCES ExpressCompanys(ID);
GO

--Ϊ[FinanceAccountFlows]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinanceAccountFlows_FinanceVaults')
ALTER TABLE [dbo].[FinanceAccountFlows] ADD CONSTRAINT FK_FinanceAccountFlows_FinanceVaults FOREIGN KEY(FinanceVaultID) REFERENCES FinanceVaults(ID);
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinanceAccountFlows_FinanceAccounts')
ALTER TABLE [dbo].[FinanceAccountFlows] ADD CONSTRAINT FK_FinanceAccountFlows_FinanceAccounts FOREIGN KEY(FinanceAccountID) REFERENCES FinanceAccounts(ID);
GO

--Ϊ[FinancePayments]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinancePayments_FinanceVaults')
ALTER TABLE [dbo].[FinancePayments] ADD CONSTRAINT FK_FinancePayments_FinanceVaults FOREIGN KEY(FinanceVaultID) REFERENCES FinanceVaults(ID);
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinancePayments_FinanceAccounts')
ALTER TABLE [dbo].[FinancePayments] ADD CONSTRAINT FK_FinancePayments_FinanceAccounts FOREIGN KEY(FinanceAccountID) REFERENCES FinanceAccounts(ID);
GO

--Ϊ[FinanceReceipts]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinanceReceipts_FinanceVaults')
ALTER TABLE [dbo].[FinanceReceipts] ADD CONSTRAINT FK_FinanceReceipts_FinanceVaults FOREIGN KEY(FinanceVaultID) REFERENCES FinanceVaults(ID);
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_FinanceReceipts_FinanceAccounts')
ALTER TABLE [dbo].[FinanceReceipts] ADD CONSTRAINT FK_FinanceReceipts_FinanceAccounts FOREIGN KEY(FinanceAccountID) REFERENCES FinanceAccounts(ID);
GO

--Ϊ[InvoiceNotices]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_InvoiceNotices_Clients')
ALTER TABLE [dbo].[InvoiceNotices] ADD CONSTRAINT FK_InvoiceNotices_Clients FOREIGN KEY(ClientID) REFERENCES Clients(ID);
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_InvoiceNotices_ClientInvoices')
ALTER TABLE [dbo].[InvoiceNotices] ADD CONSTRAINT FK_InvoiceNotices_ClientInvoices FOREIGN KEY(ClientInvoiceID) REFERENCES ClientInvoices(ID);
GO

--Ϊ[ManifestConsignmentContainers]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_ManifestConsignmentContainers_ManifestConsignments')
ALTER TABLE [dbo].[ManifestConsignmentContainers] ADD CONSTRAINT FK_ManifestConsignmentContainers_ManifestConsignments FOREIGN KEY(ManifestConsignmentID) REFERENCES ManifestConsignments(ID);
GO

--Ϊ[ManifestConsignmentItems]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_ManifestConsignmentItems_ManifestConsignments')
ALTER TABLE [dbo].[ManifestConsignmentItems] ADD CONSTRAINT FK_ManifestConsignmentItems_ManifestConsignments FOREIGN KEY(ManifestConsignmentID) REFERENCES ManifestConsignments(ID);
GO

--Ϊ[ManifestConsignmentTraces]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_ManifestConsignmentTraces_ManifestConsignments')
ALTER TABLE [dbo].[ManifestConsignmentTraces] ADD CONSTRAINT FK_ManifestConsignmentTraces_ManifestConsignments FOREIGN KEY(ManifestConsignmentID) REFERENCES ManifestConsignments(ID);
GO

--Ϊ[OrderPayExchangeSuppliers]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderPayExchangeSuppliers_ClientSuppliers')
ALTER TABLE [dbo].[OrderPayExchangeSuppliers] ADD CONSTRAINT FK_OrderPayExchangeSuppliers_ClientSuppliers FOREIGN KEY(ClientSupplierID) REFERENCES ClientSuppliers(ID);
GO

--Ϊ[OrderReceipts]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderReceipts_Clients')
ALTER TABLE [dbo].[OrderReceipts] ADD CONSTRAINT FK_OrderReceipts_Clients FOREIGN KEY(ClientID) REFERENCES Clients(ID);
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderReceipts_Orders')
ALTER TABLE [dbo].[OrderReceipts] ADD CONSTRAINT FK_OrderReceipts_Orders FOREIGN KEY(OrderID) REFERENCES Orders(ID);
GO

--Ϊ[Orders]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_Orders_Clients')
ALTER TABLE [dbo].[Orders] ADD CONSTRAINT FK_Orders_Clients FOREIGN KEY(ClientID) REFERENCES Clients(ID);
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_Orders_ClientAgreements')
ALTER TABLE [dbo].[Orders] ADD CONSTRAINT FK_Orders_ClientAgreements FOREIGN KEY(ClientAgreementID) REFERENCES ClientAgreements(ID);
GO

--Ϊ[OrderVoyages]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderVoyages_Orders')
ALTER TABLE [dbo].[OrderVoyages] ADD CONSTRAINT FK_OrderVoyages_Orders FOREIGN KEY(OrderID) REFERENCES Orders(ID);
GO

--Ϊ[OrderWaybillItems]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWaybillItems_OrderWaybills')
ALTER TABLE [dbo].[OrderWaybillItems] ADD CONSTRAINT FK_OrderWaybillItems_OrderWaybills FOREIGN KEY(OrderWaybillID) REFERENCES OrderWaybills(ID);
GO

--Ϊ[OrderWaybills]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWaybills_Orders')
ALTER TABLE [dbo].[OrderWaybills] ADD CONSTRAINT FK_OrderWaybills_Orders FOREIGN KEY(OrderID) REFERENCES Orders(ID);
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWaybills_Carriers')
ALTER TABLE [dbo].[OrderWaybills] ADD CONSTRAINT FK_OrderWaybills_Carriers FOREIGN KEY(CarrierID) REFERENCES Carriers(ID);
GO

--Ϊ[OrderWhesPremiumFiles]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWhesPremiumFiles_OrderWhesPremiums')
ALTER TABLE [dbo].[OrderWhesPremiumFiles] ADD CONSTRAINT FK_OrderWhesPremiumFiles_OrderWhesPremiums FOREIGN KEY(OrderWhesPremiumID) REFERENCES OrderWhesPremiums(ID);
GO

--Ϊ[OrderWhesPremiums]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_OrderWhesPremiums_Orders')
ALTER TABLE [dbo].[OrderWhesPremiums] ADD CONSTRAINT FK_OrderWhesPremiums_Orders FOREIGN KEY(OrderID) REFERENCES Orders(ID);
GO

--Ϊ[PackingItems]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_PackingItems_Packings')
ALTER TABLE [dbo].PackingItems ADD CONSTRAINT FK_PackingItems_Packings FOREIGN KEY(PackingID) REFERENCES Packings(ID);
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_PackingItems_Sortings')
ALTER TABLE [dbo].PackingItems ADD CONSTRAINT FK_PackingItems_Sortings FOREIGN KEY(SortingID) REFERENCES Sortings(ID);
GO

--Ϊ[Packings]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_Packings_Orders')
ALTER TABLE [dbo].Packings ADD CONSTRAINT FK_Packings_Orders FOREIGN KEY(OrderID) REFERENCES Orders(ID);
GO

--Ϊ[PayExchangeApplies]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_PayExchangeApplies_Clients')
ALTER TABLE [dbo].PayExchangeApplies ADD CONSTRAINT FK_PayExchangeApplies_Clients FOREIGN KEY(ClientID) REFERENCES Clients(ID);
GO

--Ϊ[PayExchangeApplyFiles]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_PayExchangeApplyFiles_PayExchangeApplies')
ALTER TABLE [dbo].PayExchangeApplyFiles ADD CONSTRAINT FK_PayExchangeApplyFiles_PayExchangeApplies FOREIGN KEY(PayExchangeApplyID) REFERENCES PayExchangeApplies(ID);
GO

--Ϊ[PaymentApplies]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentApplies_Orders')
ALTER TABLE [dbo].PaymentApplies ADD CONSTRAINT FK_PaymentApplies_Orders FOREIGN KEY(OrderID) REFERENCES Orders(ID);
GO

--Ϊ[PaymentNoticeFiles]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentNoticeFiles_PaymentNotices')
ALTER TABLE [dbo].PaymentNoticeFiles ADD CONSTRAINT FK_PaymentNoticeFiles_PaymentNotices FOREIGN KEY(PaymentNoticeID) REFERENCES PaymentNotices(ID);
GO

--Ϊ[PaymentNoticeItems]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_PaymentNoticeItems_PaymentNotices')
ALTER TABLE [dbo].PaymentNoticeItems ADD CONSTRAINT FK_PaymentNoticeItems_PaymentNotices FOREIGN KEY(PaymentNoticeID) REFERENCES PaymentNotices(ID);
GO

--Ϊ[PreProductCategories]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_PreProductCategories_PreProducts')
ALTER TABLE [dbo].PreProductCategories ADD CONSTRAINT FK_PreProductCategories_PreProducts FOREIGN KEY(PreProductID) REFERENCES PreProducts(ID);
GO

--Ϊ[PreProducts]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_PreProducts_Clients')
ALTER TABLE [dbo].PreProducts ADD CONSTRAINT FK_PreProducts_Clients FOREIGN KEY(ClientID) REFERENCES Clients(ID);
GO

--Ϊ[ReceiptNotices]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_ReceiptNotices_Clients')
ALTER TABLE [dbo].ReceiptNotices ADD CONSTRAINT FK_ReceiptNotices_Clients FOREIGN KEY(ClientID) REFERENCES Clients(ID);
GO

--Ϊ[Sortings]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_Sortings_Orders')
ALTER TABLE [dbo].Sortings ADD CONSTRAINT FK_Sortings_Orders FOREIGN KEY(OrderID) REFERENCES Orders(ID);
GO

--Ϊ[SwapLimitCountries]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_SwapLimitCountries_SwapBanks')
ALTER TABLE [dbo].SwapLimitCountries ADD CONSTRAINT FK_SwapLimitCountries_SwapBanks FOREIGN KEY(BankID) REFERENCES SwapBanks(ID);
GO

--Ϊ[SwapNoticeItems]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_SwapNoticeItems_SwapNotices')
ALTER TABLE [dbo].SwapNoticeItems ADD CONSTRAINT FK_SwapNoticeItems_SwapNotices FOREIGN KEY(SwapNoticeID) REFERENCES SwapNotices(ID);
GO

--Ϊ[TemporaryFiles]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_TemporaryFiles_Temporarys')
ALTER TABLE [dbo].TemporaryFiles ADD CONSTRAINT FK_TemporaryFiles_Temporarys FOREIGN KEY(TemporaryID) REFERENCES Temporarys(ID);
GO

--Ϊ[Vehicles]��������Լ��
IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='FK_Vehicles_Carriers')
ALTER TABLE [dbo].Vehicles ADD CONSTRAINT FK_Vehicles_Carriers FOREIGN KEY(CarrierID) REFERENCES Carriers(ID);
GO