use [PvbCrm];


--[dbo].[Beneficiaries][dbo].[Consignees][dbo].[Contacts][dbo].[Invoices]

--受益人添加人
if (not exists (select * from syscolumns where ID=object_id('Beneficiaries') and name='AdminID'))
begin
Alter TABLE [dbo].[Beneficiaries] ADD AdminID varchar(50) not null default 'SA01'
end;
--到货地址添加人
if (not exists (select * from syscolumns where ID=object_id('Consignees') and name='AdminID'))
begin
Alter TABLE [dbo].[Consignees] ADD AdminID varchar(50) not null default 'SA01'
end;
--发票添加人
if (not exists (select * from syscolumns where ID=object_id('Invoices') and name='AdminID'))
begin
Alter TABLE [dbo].[Invoices] ADD AdminID varchar(50) not null default 'SA01'
end;
--联系人添加人
if (not exists (select * from syscolumns where ID=object_id('Contacts') and name='AdminID'))
begin
Alter TABLE [dbo].[Contacts] ADD AdminID varchar(50) not null default 'SA01'
end;