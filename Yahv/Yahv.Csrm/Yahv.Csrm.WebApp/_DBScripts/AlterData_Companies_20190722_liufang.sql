USE [PvbCrm]
GO
/****** Object:  Table [dbo].[temp_Company]    Script Date: 2019/7/22 星期一 16:46:43 ******/
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
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京北方科讯电子技术有限公司深圳分公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'深圳市佛兰德电子有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'深圳市英赛尔电子有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京艾瑞泰克电子技术有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京爱伯乐电子技术有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京奥讯达电子技术有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京北方科讯电子技术有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京航天新兴科技开发有限责任公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京美商利华电子技术有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京欣美科电子技术有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京远大汇能科技有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京远大创新科技有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'深圳市创新恒远供应链管理有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'深圳市创新恒远科技有限公司北京销售分公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京英赛尔科技有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京景瑞丰投资管理有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京景瑞丰投资管理有限公司上海分公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京远大创新科技有限公司廊坊分公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京远大创新科技有限公司深圳南山分公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'上海比亿电子技术有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'苏州比一比电子有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京远大创新科技有限公司成都分公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'廊坊市比比商贸有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'杭州比一比电子科技有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'深圳市创新恒远供应链管理有限公司龙岗分公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京远大创新科技有限公司上海分公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京远大创新科技有限公司杭州分公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京远大创新科技有限公司西安分公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'深圳市芯达通供应链管理有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'山东搜宝网络科技有限公司')
INSERT [dbo].[temp_Company] ([Name]) VALUES (N'北京远大创新科技有限公司武汉分公司')


GO
update [dbo].[Companies] set Status=600

Go


declare @companyName varchar(150)
 DECLARE company_cursor CURSOR
    FOR
        SELECT  Name
        FROM    [dbo].[temp_Company]
        WHERE   Name <> ''
        GROUP BY Name;

    OPEN company_cursor;
    FETCH NEXT FROM company_cursor INTO @companyName;
    WHILE @@fetch_status = 0
        BEGIN
            IF NOT EXISTS ( SELECT  *
                            FROM    PvbCrm.dbo.Enterprises e
                                    RIGHT JOIN PvbCrm.dbo.Companies c ON e.ID = c.ID
                            WHERE   e.Name = @companyName )
                BEGIN
					--添加企业表
                    INSERT  INTO PvbCrm.dbo.Enterprises
                            ( ID ,
                              Name ,
                              AdminCode ,
                              Status ,
                              District
                            )
                    VALUES  ( UPPER(SUBSTRING(sys.fn_sqlvarbasetostr(HASHBYTES('MD5',
                                                              @companyName)),
                                              3, 32)) , -- ID - varchar(50)
                              @companyName , -- Name - nvarchar(150)
                              'SA01' , -- AdminCode - varchar(50)
                              200 ,  -- Business - int
                              '中国'
                            );
 					--添加company表
                    INSERT  INTO PvbCrm.dbo.Companies
                            ( ID ,
                              Type ,
                              Range ,
                              Status
                            )
                    VALUES  ( UPPER(SUBSTRING(sys.fn_sqlvarbasetostr(HASHBYTES('MD5',
                                                              @companyName)),
                                              3, 32)) ,
                              1 ,
                              0 ,
                              200
                            );	
                END;
 			ELSE
				BEGIN
					UPDATE [dbo].[Companies] set Status=200 where ID=(UPPER(SUBSTRING(sys.fn_sqlvarbasetostr(HASHBYTES('MD5',
                                                              @companyName)),3, 32))); 
				END;
            FETCH NEXT FROM company_cursor INTO @companyName;
        END;
    CLOSE company_cursor;
    DEALLOCATE company_cursor;

Go 
drop table [dbo].[temp_Company];
Go