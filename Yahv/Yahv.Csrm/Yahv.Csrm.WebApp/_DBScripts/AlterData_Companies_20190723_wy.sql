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
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'深圳市创新恒远科技有限公司北京销售分公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京远大创新科技有限公司深圳南山分公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京远大创新科技有限公司成都分公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'廊坊市比比商贸有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'深圳市创新恒远供应链管理有限公司龙岗分公司')

INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京远大创新科技有限公司上海分公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京远大创新科技有限公司杭州分公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京远大创新科技有限公司西安分公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'深圳市芯达通供应链管理有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'山东搜宝网络科技有限公司')

INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京远大创新科技有限公司武汉分公司')
GO

--添加企业表
insert into dbo.Enterprises(ID,Name,AdminCode,[Status],District )
select UPPER(SUBSTRING(sys.fn_sqlvarbasetostr(HASHBYTES('MD5', Name)),3,32)),Name,'SA01',200,'中国'
from [dbo].[temp_Company] as tc
GO
--添加company表
INSERT  INTO PvbCrm.dbo.Companies(ID,[Type],[Range],[Status])
select UPPER(SUBSTRING(sys.fn_sqlvarbasetostr(HASHBYTES('MD5',tc.Name)),3,32)),1,0,200 
from [dbo].[temp_Company] as tc
GO

--更新简称公司状态为删除状态
update dbo.Companies set [Status]=600 from
(
select c.ID from dbo.Enterprises e
INNER JOIN dbo.Companies c 
ON e.ID = c.ID
WHERE e.Name in (
'北京远大创新深圳南山分公司',
'北京远大创新成都分公司',
'廊坊比比商贸有限公司',
'深圳市创新恒远供应链龙岗分公司',
'上海远大创新分公司',
'北京远大创新杭州分公司',
'北京远大创新西安分公司',
'菏泽搜宝',
'武汉远大创新'
)
) as t where t.ID = dbo.Companies.ID


drop table [dbo].[temp_Company]


