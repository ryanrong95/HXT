use [PvbCrm];
Go

--�����
if (not exists (select * from syscolumns where ID=object_id('Companies') and name='AdminID'))
begin
Alter TABLE [dbo].[Companies] ADD AdminID varchar(50) not null default 'SA01'
end;


--��������
if (not exists (select * from syscolumns where ID=object_id('Companies') and name='CreateDate'))
begin
Alter TABLE [dbo].[Companies] ADD CreateDate datetime not null default getdate()
end;
--�޸�����
if (not exists (select * from syscolumns where ID=object_id('Companies') and name='UpdateDate'))
begin
Alter TABLE [dbo].[Companies] ADD UpdateDate datetime not null default getdate()
end;
