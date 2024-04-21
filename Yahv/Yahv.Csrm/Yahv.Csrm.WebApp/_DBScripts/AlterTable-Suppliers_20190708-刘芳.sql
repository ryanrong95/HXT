use [PvbCrm]

--�����ֶ�
if (not exists (select * from syscolumns where ID=object_id('Suppliers') and name='RepayCycle'))
begin
Alter TABLE [dbo].[Suppliers] ADD [RepayCycle] int not null default 0
end;

--���
if (not exists (select * from syscolumns where ID=object_id('Suppliers') and name='Price'))
begin
Alter TABLE [dbo].[Suppliers] ADD Price decimal(18,5) not null default 0
end;

--����
if (not exists (select * from syscolumns where ID=object_id('Suppliers') and name='Currency'))
begin
Alter TABLE [dbo].[Suppliers] ADD Currency int not null default 1
end;

--�����
if (not exists (select * from syscolumns where ID=object_id('Suppliers') and name='AdminID'))
begin
Alter TABLE [dbo].[Suppliers] ADD AdminID varchar(50) not null default 'SA01'
end;


--��������
if (not exists (select * from syscolumns where ID=object_id('Suppliers') and name='CreateDate'))
begin
Alter TABLE [dbo].[Suppliers] ADD CreateDate datetime not null default getdate()
end;
--�޸�����
if (not exists (select * from syscolumns where ID=object_id('Suppliers') and name='UpdateDate'))
begin
Alter TABLE [dbo].[Suppliers] ADD UpdateDate datetime not null default getdate()
end;

