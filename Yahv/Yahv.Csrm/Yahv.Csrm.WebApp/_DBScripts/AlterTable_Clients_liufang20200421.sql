--[dbo].[Clients]加Major,是否重点客户
if (not exists (select * from syscolumns where ID=object_id('Clients') and name='Major'))
begin
Alter TABLE [dbo].[Clients] add Major bit null;
end;
--把Major改为not null
go
if (exists (select * from syscolumns where ID=object_id('Clients') and name='Major'))
begin
update [dbo].[Clients] set Major=0;
Alter TABLE [dbo].[Clients] alter column Major bit not null;
end;