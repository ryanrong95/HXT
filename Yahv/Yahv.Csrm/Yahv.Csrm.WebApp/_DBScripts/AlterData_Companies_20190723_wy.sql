use PvbCrm

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[temp_Company]') AND type in (N'U'))
BEGIN
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON
	SET ANSI_PADDING ON
	CREATE TABLE [dbo].[temp_Company](
		[Name] [varchar](150) NULL
	) ON [PRIMARY]
END;
GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'�����д��º�Զ�Ƽ����޹�˾�������۷ֹ�˾')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'����Զ���¿Ƽ����޹�˾������ɽ�ֹ�˾')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'����Զ���¿Ƽ����޹�˾�ɶ��ֹ�˾')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'�ȷ��бȱ���ó���޹�˾')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'�����д��º�Զ��Ӧ���������޹�˾���ڷֹ�˾')

INSERT [dbo].[temp_Company] ([Name]) VALUES (N'����Զ���¿Ƽ����޹�˾�Ϻ��ֹ�˾')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'����Զ���¿Ƽ����޹�˾���ݷֹ�˾')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'����Զ���¿Ƽ����޹�˾�����ֹ�˾')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'������о��ͨ��Ӧ���������޹�˾')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'ɽ���ѱ�����Ƽ����޹�˾')

INSERT [dbo].[temp_Company] ([Name]) VALUES (N'����Զ���¿Ƽ����޹�˾�人�ֹ�˾')
GO

--�����ҵ��
insert into dbo.Enterprises(ID,Name,AdminCode,[Status],District )
select UPPER(SUBSTRING(sys.fn_sqlvarbasetostr(HASHBYTES('MD5', Name)),3,32)),Name,'SA01',200,'�й�'
from [dbo].[temp_Company] as tc
GO
--���company��
INSERT  INTO PvbCrm.dbo.Companies(ID,[Type],[Range],[Status])
select UPPER(SUBSTRING(sys.fn_sqlvarbasetostr(HASHBYTES('MD5',tc.Name)),3,32)),1,0,200 
from [dbo].[temp_Company] as tc
GO

--���¼�ƹ�˾״̬Ϊɾ��״̬
update dbo.Companies set [Status]=600 from
(
select c.ID from dbo.Enterprises e
INNER JOIN dbo.Companies c 
ON e.ID = c.ID
WHERE e.Name in (
'����Զ����������ɽ�ֹ�˾',
'����Զ���³ɶ��ֹ�˾',
'�ȷ��ȱ���ó���޹�˾',
'�����д��º�Զ��Ӧ�����ڷֹ�˾',
'�Ϻ�Զ���·ֹ�˾',
'����Զ���º��ݷֹ�˾',
'����Զ���������ֹ�˾',
'�����ѱ�',
'�人Զ����'
)
) as t where t.ID = dbo.Companies.ID


drop table [dbo].[temp_Company]


