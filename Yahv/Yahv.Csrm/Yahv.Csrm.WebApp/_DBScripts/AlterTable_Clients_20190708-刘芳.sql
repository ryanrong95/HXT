use [PvbCrm];
Go

--�����
if (not exists (select * from syscolumns where ID=object_id('Clients') and name='AdminID'))
begin
Alter TABLE [dbo].[Clients] ADD AdminID varchar(50) not null default 'SA01'
end;


--��������
if (not exists (select * from syscolumns where ID=object_id('Clients') and name='CreateDate'))
begin
Alter TABLE [dbo].[Clients] ADD CreateDate datetime not null default getdate()
end;
--�޸�����
if (not exists (select * from syscolumns where ID=object_id('Clients') and name='UpdateDate'))
begin
Alter TABLE [dbo].[Clients] ADD UpdateDate datetime not null default getdate()
end;

