USE [PvbCrm]
GO
/****** Object:  Table [dbo].[Data_Contract]    Script Date: 2019/10/25 10:02:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[Data_Contract]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Data_Contract](
	[客户名称] [varchar](255) NULL,
	[开始日期] [datetime] NULL,
	[结束日期] [datetime] NULL,
	[代理费率] [float] NULL,
	[最低代理费] [float] NULL,
	[是否预换汇] [float] NULL,
	[是否90天内换汇] [float] NULL,
	[开票类型] [float] NULL,
	[对应税点] [float] NULL,
	[文件名称] [varchar](255) NULL,
	[文件地址] [varchar](255) NULL,
	[文件格式] [varchar](255) NULL,
	[文件类型] [varchar](255) NULL
) ON [PRIMARY]
END
GO
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'上海傲研实业发展有限公司', CAST(0x0000A95100000000 AS DateTime), CAST(0x0000AC2C00000000 AS DateTime), 0.006, 300, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'成都能通科技有限公司', CAST(0x0000A8CD00000000 AS DateTime), CAST(0x0000ABA800000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京时代创兴电子技术有限公司', CAST(0x0000A8D600000000 AS DateTime), CAST(0x0000ABB500000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'苏州拓品微信息科技有限公司', CAST(0x0000A96000000000 AS DateTime), CAST(0x0000ADA700000000 AS DateTime), 0.006, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市中科信诺电子有限公司', CAST(0x0000A94A00000000 AS DateTime), CAST(0x0000AD9100000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'苏州丰芯昌电子科技有限公司', CAST(0x0000AA6C00000000 AS DateTime), CAST(0x0000AD4600000000 AS DateTime), 0.006, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市拓普仪器有限公司', CAST(0x0000A9F900000000 AS DateTime), CAST(0x0000ACD300000000 AS DateTime), 0.006, 500, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市宝泰威数码有限公司', CAST(0x0000A8DB00000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.0035, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'安华合（北京）科技有限公司', CAST(0x0000AAE100000000 AS DateTime), CAST(0x0000ADBB00000000 AS DateTime), 0.005, 500, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'苏州视码特电子科技有限公司', CAST(0x0000A9DF00000000 AS DateTime), CAST(0x0000ACBA00000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市凯迪微科技有限公司', CAST(0x0000A89600000000 AS DateTime), CAST(0x0000AA1F00000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市奇芯达电子有限公司', CAST(0x0000A9D200000000 AS DateTime), CAST(0x0000AB3E00000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市弘昌贸易有限公司', CAST(0x0000A8A800000000 AS DateTime), CAST(0x0000AB8200000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京艾瑞泰克电子技术有限公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.035, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京展扬金卡科技有限公司', CAST(0x0000A8FF00000000 AS DateTime), CAST(0x0000ABD900000000 AS DateTime), 0.006, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京北方科讯电子技术有限公司深圳分公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.035, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京铭芯微科技有限公司', CAST(0x0000AA7900000000 AS DateTime), CAST(0x0000AD5300000000 AS DateTime), 0.005, 400, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市壹比特科技有限公司', CAST(0x0000AA2200000000 AS DateTime), CAST(0x0000ACFD00000000 AS DateTime), 0.004, 400, 1, 0, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市晶开元科技有限公司', CAST(0x0000A83A00000000 AS DateTime), CAST(0x0000AC8100000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市宏英微科技有限公司', CAST(0x0000A95800000000 AS DateTime), CAST(0x0000AAC500000000 AS DateTime), 0.0035, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市中电辰星科技有限公司', CAST(0x0000A8D500000000 AS DateTime), CAST(0x0000ABC900000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京航天新兴科技开发有限责任公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.035, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市飞康德电子科技有限公司', CAST(0x0000A56200000000 AS DateTime), CAST(0x0000A83C00000000 AS DateTime), 0.006, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市恩堤西科技有限公司', CAST(0x0000AAE100000000 AS DateTime), CAST(0x0000ADBB00000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市国泰盛科技有限公司', CAST(0x0000A95800000000 AS DateTime), CAST(0x0000AAC500000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'领沃（上海）国际贸易有限公司', CAST(0x0000A8D500000000 AS DateTime), CAST(0x0000ABB100000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'惠州市弘翔电子有限公司', CAST(0x0000A8C600000000 AS DateTime), CAST(0x0000ABA800000000 AS DateTime), 0.005, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市天玖隆科技有限公司', CAST(0x0000A8E200000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'常德沁音科技有限公司', CAST(0x0000A89600000000 AS DateTime), CAST(0x0000A8AB00000000 AS DateTime), 0.004, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'廊坊市比比商贸有限公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.01, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市凯创达电子技术有限公司', CAST(0x0000A8DE00000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市维纳亿电子有限公司', CAST(0x0000A98400000000 AS DateTime), CAST(0x0000AC5F00000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'辽宁瑞华实业集团高新科技有限公司', CAST(0x0000A98A00000000 AS DateTime), CAST(0x0000ADD100000000 AS DateTime), 0.004, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市毅创辉电子科技有限公司', CAST(0x0000AAA400000000 AS DateTime), CAST(0x0000AD7E00000000 AS DateTime), 0.005, 400, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'杭州齐飞科技有限公司', CAST(0x0000A8F700000000 AS DateTime), CAST(0x0000AD3E00000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京广盛集源科技有限公司', CAST(0x0000A8F500000000 AS DateTime), CAST(0x0000AD3C00000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'天津军航科技有限公司', CAST(0x0000A96700000000 AS DateTime), CAST(0x0000AC4300000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市斯迈尔电子有限公司', CAST(0x0000A8B300000000 AS DateTime), CAST(0x0000ACF900000000 AS DateTime), 0.004, 500, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'广东中科臻恒信息技术有限公司', CAST(0x0000AA8100000000 AS DateTime), CAST(0x0000AD5C00000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'上海卓罡乐电子有限公司', CAST(0x0000AAAE00000000 AS DateTime), CAST(0x0000AD8800000000 AS DateTime), 0.005, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳世讯科技有限公司', CAST(0x0000A8FC00000000 AS DateTime), CAST(0x0000AD4300000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'徐州帝意电子有限公司', CAST(0x0000AAA000000000 AS DateTime), CAST(0x0000AD7A00000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市先洲科技有限公司', CAST(0x0000A8B500000000 AS DateTime), CAST(0x0000ACFD00000000 AS DateTime), 0.003, 300, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳昂科鑫智能科技有限公司', CAST(0x0000AAA400000000 AS DateTime), CAST(0x0000AD7C00000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京欣美科电子技术有限公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.035, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'重庆市俊杰科技有限公司', CAST(0x0000AA4F00000000 AS DateTime), CAST(0x0000AD2A00000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市铭尔达科技有限公司', CAST(0x0000A95B00000000 AS DateTime), CAST(0x0000ADA200000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京爱伯乐电子技术有限公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.035, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市圳耀电子有限公司', CAST(0x0000A9D200000000 AS DateTime), CAST(0x0000ACAD00000000 AS DateTime), 0.005, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市鑫锋盛科技有限公司', CAST(0x0000A95E00000000 AS DateTime), CAST(0x0000ADA500000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市鼎芯美电子有限公司', CAST(0x0000A8F600000000 AS DateTime), CAST(0x0000ABD800000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市捷捷电子有限公司', CAST(0x0000A8E700000000 AS DateTime), CAST(0x0000AD2E00000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京瑞博科创商贸中心', CAST(0x0000A7EC00000000 AS DateTime), CAST(0x0000AAC500000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市天诚丽海电子有限公司', CAST(0x0000A92000000000 AS DateTime), CAST(0x0000ABFB00000000 AS DateTime), 0.004, 300, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳金信诺高新技术股份有限公司', CAST(0x0000A95400000000 AS DateTime), CAST(0x0000AC2F00000000 AS DateTime), 0.005, 400, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'西安诺驰电子科技有限公司', CAST(0x0000A94200000000 AS DateTime), CAST(0x0000AC1D00000000 AS DateTime), 0.004, 400, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京鑫众创展科技有限公司', CAST(0x0000A8F100000000 AS DateTime), CAST(0x0000AD3800000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京科润阳光技术有限公司', CAST(0x0000A8E700000000 AS DateTime), CAST(0x0000AD2E00000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳达元贸科技有限公司', CAST(0x0000A95700000000 AS DateTime), CAST(0x0000AAC400000000 AS DateTime), 0.004, 300, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'河北顶控新能源科技有限公司', CAST(0x0000A55900000000 AS DateTime), CAST(0x0000A83300000000 AS DateTime), 0.007, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'友信宏科新能源（徐州）有限公司', CAST(0x0000A8F100000000 AS DateTime), CAST(0x0000AD3800000000 AS DateTime), 0.006, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市佛兰德电子有限公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.035, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市优贝美实业有限公司', CAST(0x0000A8D900000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市聚丰恒业电子有限公司', CAST(0x0000A8E800000000 AS DateTime), CAST(0x0000ABCB00000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京宏宇亮达科技有限公司', CAST(0x0000AA9C00000000 AS DateTime), CAST(0x0000AD7700000000 AS DateTime), 0.003, 300, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'四川智钡迩科技有限公司', CAST(0x0000A92700000000 AS DateTime), CAST(0x0000AC0200000000 AS DateTime), 0.004, 500, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市华光新科技术有限公司', CAST(0x0000A8CD00000000 AS DateTime), CAST(0x0000AD1400000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市欣诺泰电子有限公司', CAST(0x0000A8CD00000000 AS DateTime), CAST(0x0000ABA800000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳东方启峰科技有限公司', CAST(0x0000A7F200000000 AS DateTime), CAST(0x0000AACB00000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市大成明华科技有限公司', CAST(0x0000A9E600000000 AS DateTime), CAST(0x0000AE2D00000000 AS DateTime), 0.006, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京天信佑利科技有限公司', CAST(0x0000A95B00000000 AS DateTime), CAST(0x0000AC3500000000 AS DateTime), 0.004, 300, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市港驰科讯电子有限公司', CAST(0x0000A8D600000000 AS DateTime), CAST(0x0000ABC900000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京强联通讯技术有限公司', CAST(0x0000A80900000000 AS DateTime), CAST(0x0000AC5000000000 AS DateTime), 0.006, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市信心智能标签技术有限公司', CAST(0x0000A8C000000000 AS DateTime), CAST(0x0000AB9D00000000 AS DateTime), 0.003, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市时代芯电子有限公司', CAST(0x0000AA7200000000 AS DateTime), CAST(0x0000AD4D00000000 AS DateTime), 0.004, 500, 1, 0, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳晶福源科技股份有限公司', CAST(0x0000A99E00000000 AS DateTime), CAST(0x0000ADE500000000 AS DateTime), 0.007, 450, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'上海旌睿电子科技有限公司', CAST(0x0000A7A300000000 AS DateTime), CAST(0x0000AABA00000000 AS DateTime), 0.006, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京卓智视联电子科技有限公司', CAST(0x0000A9D600000000 AS DateTime), CAST(0x0000ACB100000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市玮华电子有限公司', CAST(0x0000A8BF00000000 AS DateTime), CAST(0x0000AA2C00000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'江苏华讯电子技术有限公司', CAST(0x0000A83C00000000 AS DateTime), CAST(0x0000AB8D00000000 AS DateTime), 0.004, 300, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京北方科讯电子技术有限公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.035, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市翊智科技有限公司', CAST(0x0000A93F00000000 AS DateTime), CAST(0x0000AC5700000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京科瑞泰科电子技术有限公司', CAST(0x0000A7EC00000000 AS DateTime), CAST(0x0000AAC500000000 AS DateTime), 0.004, 300, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市宝泰威电子有限公司', CAST(0x0000A95400000000 AS DateTime), CAST(0x0000AAC100000000 AS DateTime), 0.0035, 400, 0, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳福迈科技有限公司', CAST(0x0000A73C00000000 AS DateTime), CAST(0x0000AD0800000000 AS DateTime), 0.004, 400, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳启航开创科技有限公司', CAST(0x0000AA1000000000 AS DateTime), CAST(0x0000ACEB00000000 AS DateTime), 0.003, 300, 1, 0, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京华盈润航自动化科技有限公司', CAST(0x0000AAB700000000 AS DateTime), CAST(0x0000AC2500000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'均威科技（深圳）有限公司', CAST(0x0000A8DB00000000 AS DateTime), CAST(0x0000ABCA00000000 AS DateTime), 0.005, 500, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京英赛尔科技有限公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.035, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'苏州比一比电子有限公司', CAST(0x0000A63600000000 AS DateTime), CAST(0x0000AEC500000000 AS DateTime), 0.01, 1, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市拓海通用电气有限公司', CAST(0x0000A98900000000 AS DateTime), CAST(0x0000AC6400000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市贝尔赛思电子有限公司', CAST(0x0000A70D00000000 AS DateTime), CAST(0x0000ABA800000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京友信宏科电子科技股份有限公司', CAST(0x0000A8F100000000 AS DateTime), CAST(0x0000AD3800000000 AS DateTime), 0.006, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市恒迈伟业电子技术有限公司', CAST(0x0000A8C700000000 AS DateTime), CAST(0x0000AD0E00000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市博金凯科技有限公司', CAST(0x0000AA3800000000 AS DateTime), CAST(0x0000AD3100000000 AS DateTime), 0.004, 300, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'天津睿祺电子科技有限公司', CAST(0x0000A8D800000000 AS DateTime), CAST(0x0000ABB500000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市爱舍尔科技有限公司', CAST(0x0000A85E00000000 AS DateTime), CAST(0x0000AB9900000000 AS DateTime), 0.005, 400, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'上海诚用电子有限公司', CAST(0x0000A89E00000000 AS DateTime), CAST(0x0000AB7900000000 AS DateTime), 0.003, 300, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市积聚电子有限公司', CAST(0x0000A96100000000 AS DateTime), CAST(0x0000AC3C00000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京祯泰恒盛电子技术有限公司', CAST(0x0000A8D900000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
GO
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市富联芯微科技有限公司', CAST(0x0000A8D800000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.003, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市伯乐马电子有限公司', CAST(0x0000A89600000000 AS DateTime), CAST(0x0000AD1B00000000 AS DateTime), 0.004, 500, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'苏州拓时达电子有限公司 ', CAST(0x0000A85D00000000 AS DateTime), CAST(0x0000ACA400000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市圣禾堂科技有限公司', CAST(0x0000A85000000000 AS DateTime), CAST(0x0000AC9700000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市冠亚达电子有限公司', CAST(0x0000A97400000000 AS DateTime), CAST(0x0000ADBB00000000 AS DateTime), 0.004, 400, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市宽远科技有限公司', CAST(0x0000A8CD00000000 AS DateTime), CAST(0x0000ABA800000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'中电硅创（北京）科技有限公司', CAST(0x0000A8E300000000 AS DateTime), CAST(0x0000AD2A00000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市昌纶电子有限公司', CAST(0x0000A5DB00000000 AS DateTime), CAST(0x0000AD0800000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'泗水星航标电子有限公司', CAST(0x0000AA4500000000 AS DateTime), CAST(0x0000AD2000000000 AS DateTime), 0.005, 400, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京福赛尔安全消防设备有限公司', CAST(0x0000AAC400000000 AS DateTime), CAST(0x0000AD9F00000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市亿赛斯科技有限公司', CAST(0x0000AA9300000000 AS DateTime), CAST(0x0000AEDA00000000 AS DateTime), 0.006, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')

--INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市希玛科技有限公司', CAST(0x0000AAED00000000 AS DateTime), CAST(0x0000ADC800000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')


INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'济宁领傲电子科技有限公司', CAST(0x0000AAAE00000000 AS DateTime), CAST(0x0000AD8800000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市捷迈扬科技有限公司', CAST(0x0000A5BC00000000 AS DateTime), CAST(0x0000A89500000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市中电网络技术有限公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.003, 0, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市天景泰科技有限公司', CAST(0x0000A8CC00000000 AS DateTime), CAST(0x0000AD1300000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市普光电子有限公司', CAST(0x0000A8DC00000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.006, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'城创电子（深圳）有限公司', CAST(0x0000A8DB00000000 AS DateTime), CAST(0x0000ABB600000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京美商利华电子技术有限公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.035, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'江西吉为科技有限公司', CAST(0x0000A89600000000 AS DateTime), CAST(0x0000AD4200000000 AS DateTime), 0.004, 400, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市汇瑞鑫光电有限公司', CAST(0x0000A89E00000000 AS DateTime), CAST(0x0000AB7900000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'纳恩博（北京）科技有限公司', CAST(0x0000A9AC00000000 AS DateTime), CAST(0x0000AC8700000000 AS DateTime), 0.003, 400, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市捷创兴科技有限公司', CAST(0x0000A92F00000000 AS DateTime), CAST(0x0000AABD00000000 AS DateTime), 0.003, 300, 0, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'上海仟沐机电科技有限公司', CAST(0x0000A93B00000000 AS DateTime), CAST(0x0000AC1600000000 AS DateTime), 0.003, 400, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京精驰电子科技有限公司', CAST(0x0000A62B00000000 AS DateTime), CAST(0x0000AA7100000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京天成海业科技有限公司', CAST(0x0000A8D900000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京远大创新科技有限公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.01, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'聚仟亿科技（深圳）有限公司', CAST(0x0000A8D800000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.004, 300, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'上海比亿电子技术有限公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.01, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市奇盛芯片技术研发有限公司', CAST(0x0000A9DD00000000 AS DateTime), CAST(0x0000AE2400000000 AS DateTime), 0.005, 400, 1, 0, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市捷龙存储科技有限公司', CAST(0x0000AA4800000000 AS DateTime), CAST(0x0000AD2200000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市创新恒远供应链管理有限公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.01, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京奥讯达电子技术有限公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.035, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京杰赛华科技有限公司', CAST(0x0000A90A00000000 AS DateTime), CAST(0x0000AD5100000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京奥思来特电子技术有限公司', CAST(0x0000A7EC00000000 AS DateTime), CAST(0x0000AAC500000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京创新在线电子产品销售有限公司杭州分公司', CAST(0x0000A9B400000000 AS DateTime), CAST(0x0000AC8E00000000 AS DateTime), 0.003, 0, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'杭州归亚科技有限公司', CAST(0x0000A8D600000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市旭日天峰科技有限公司', CAST(0x0000A8D600000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.003, 300, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'福州创裕电子有限公司', CAST(0x0000A92700000000 AS DateTime), CAST(0x0000AC0100000000 AS DateTime), 0.004, 300, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳科芯通电子有限公司', CAST(0x0000A8D600000000 AS DateTime), CAST(0x0000AD1D00000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市意柏威电子有限公司', CAST(0x0000A97D00000000 AS DateTime), CAST(0x0000ADC400000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京博睿翔远科技有限公司', CAST(0x0000AA9C00000000 AS DateTime), CAST(0x0000AD7700000000 AS DateTime), 0.005, 500, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市欧时迈电子有限公司', CAST(0x0000A91000000000 AS DateTime), CAST(0x0000AA7D00000000 AS DateTime), 0.003, 300, 0, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市快包电子科技有限责任公司', CAST(0x0000AA6200000000 AS DateTime), CAST(0x0000AF0200000000 AS DateTime), 0.003, 0, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'成都金诺信高科技有限公司', CAST(0x0000AA1800000000 AS DateTime), CAST(0x0000ACF300000000 AS DateTime), 0.004, 400, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'厦门美芯电子科技有限公司', CAST(0x0000AAC500000000 AS DateTime), CAST(0x0000ADA500000000 AS DateTime), 0.004, 400, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'常州品联电子科技有限公司', CAST(0x0000A9C300000000 AS DateTime), CAST(0x0000AC9E00000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市晖腾科技有限公司', CAST(0x0000A8D500000000 AS DateTime), CAST(0x0000ABB100000000 AS DateTime), 0.005, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京远大汇能科技有限公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.01, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'天津慧海云创科技有限公司', CAST(0x0000A96200000000 AS DateTime), CAST(0x0000ADA900000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'福诺达科技（深圳）有限公司', CAST(0x0000A90400000000 AS DateTime), CAST(0x0000ABE500000000 AS DateTime), 0.004, 400, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市华控技术科技有限公司', CAST(0x0000A8D500000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.004, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市华宇世纪电子有限公司', CAST(0x0000AA8C00000000 AS DateTime), CAST(0x0000AD6B00000000 AS DateTime), 0.004, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳友金辉科技有限公司', CAST(0x0000A8D900000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.003, 300, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京润德康诺科技有限公司', CAST(0x0000AA5600000000 AS DateTime), CAST(0x0000AD3100000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市盈科泰电子有限公司', CAST(0x0000A71400000000 AS DateTime), CAST(0x0000ABA800000000 AS DateTime), 0.004, 400, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市唯科盛电子科技有限公司', CAST(0x0000A99300000000 AS DateTime), CAST(0x0000AB0000000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'上海协谐电子科技有限公司', CAST(0x0000AA5E00000000 AS DateTime), CAST(0x0000AEA700000000 AS DateTime), 0.003, 300, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳肯沃科技有限公司', CAST(0x0000AAB500000000 AS DateTime), CAST(0x0000AD8F00000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市盈智鑫科技有限公司', CAST(0x0000A9CF00000000 AS DateTime), CAST(0x0000ACAA00000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京创智优品科技有限公司', CAST(0x0000A91B00000000 AS DateTime), CAST(0x0000ABF600000000 AS DateTime), 0.003, 300, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京力维科技有限公司', CAST(0x0000AAA300000000 AS DateTime), CAST(0x0000AD7E00000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市海芯斯特科技有限公司', CAST(0x0000A96600000000 AS DateTime), CAST(0x0000ADAD00000000 AS DateTime), 0.003, 300, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京澜阔科技有限公司', CAST(0x0000A8F100000000 AS DateTime), CAST(0x0000AD3800000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市倬昱讯电子有限公司', CAST(0x0000A8FD00000000 AS DateTime), CAST(0x0000ABD800000000 AS DateTime), 0.005, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市顺瑞和科技有限公司', CAST(0x0000A9AC00000000 AS DateTime), CAST(0x0000AC8700000000 AS DateTime), 0.005, 400, 0, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'杭州博吉通电子有限公司', CAST(0x0000A91C00000000 AS DateTime), CAST(0x0000A93B00000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市杰世特电子有限公司', CAST(0x0000A8F600000000 AS DateTime), CAST(0x0000ABD800000000 AS DateTime), 0.006, 300, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市明智芯科技有限公司', CAST(0x0000A74600000000 AS DateTime), CAST(0x0000A8B300000000 AS DateTime), 0.005, 300, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳哈里斯功率半导体有限公司', CAST(0x0000A8D400000000 AS DateTime), CAST(0x0000ABC900000000 AS DateTime), 0.004, 400, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京远大创新科技有限公司廊坊分公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.01, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'上海茂天电子科技有限公司', CAST(0x0000A8E100000000 AS DateTime), CAST(0x0000ABBC00000000 AS DateTime), 0.005, 300, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'至上兴科技（北京）有限公司', CAST(0x0000A8BE00000000 AS DateTime), CAST(0x0000AA2A00000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'无锡钧芯科技有限公司', CAST(0x0000A8D900000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京佰思创科技有限公司', CAST(0x0000A5B400000000 AS DateTime), CAST(0x0000A9FB00000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'智云星（武汉）科技有限公司', CAST(0x0000A9FF00000000 AS DateTime), CAST(0x0000ACD900000000 AS DateTime), 0.004, 300, 1, 0, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市疆谱电子科技有限公司', CAST(0x0000AAB500000000 AS DateTime), CAST(0x0000AD9000000000 AS DateTime), 0.004, 400, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'广州市车厘子电子科技有限公司', CAST(0x0000A8F900000000 AS DateTime), CAST(0x0000ABDF00000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市创乐实业有限公司', CAST(0x0000A92000000000 AS DateTime), CAST(0x0000ABFB00000000 AS DateTime), 0.004, 300, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'慧灵科技（深圳）有限公司', CAST(0x0000AA2D00000000 AS DateTime), CAST(0x0000AE7400000000 AS DateTime), 0.005, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市同聚富电子有限公司', CAST(0x0000A89600000000 AS DateTime), CAST(0x0000AB7500000000 AS DateTime), 0.003, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳乾瀚科技有限公司', CAST(0x0000A50F00000000 AS DateTime), CAST(0x0000A7E900000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京北方科讯电子技术有限公司上海长宁分公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.035, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市关爱星科技有限公司', CAST(0x0000A91C00000000 AS DateTime), CAST(0x0000ABF700000000 AS DateTime), 0.005, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'奕科科技（深圳）有限公司', CAST(0x0000AACD00000000 AS DateTime), CAST(0x0000ADAC00000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市英赛尔电子有限公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.035, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'上海钧翊电子有限公司', CAST(0x0000A9C900000000 AS DateTime), CAST(0x0000AE1000000000 AS DateTime), 0.005, 350, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市宇柏芯科技有限公司', CAST(0x0000AA8000000000 AS DateTime), CAST(0x0000AD5A00000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'厦门致鑫和电子有限公司', CAST(0x0000A89E00000000 AS DateTime), CAST(0x0000ABB600000000 AS DateTime), 0.006, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'石家庄博亚电子科技有限公司', CAST(0x0000A90B00000000 AS DateTime), CAST(0x0000AD5200000000 AS DateTime), 0.006, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市朗存电子有限公司', CAST(0x0000A72900000000 AS DateTime), CAST(0x0000AB7500000000 AS DateTime), 0.0035, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'东莞市耀亚电子有限公司', CAST(0x0000A8D500000000 AS DateTime), CAST(0x0000ABBA00000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳龙氏盛科技有限公司', CAST(0x0000AA9300000000 AS DateTime), CAST(0x0000AD6D00000000 AS DateTime), 0.005, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市麦思浦半导体有限公司', CAST(0x0000A9C700000000 AS DateTime), CAST(0x0000A9E600000000 AS DateTime), 0.003, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'博创恒星（北京）科技有限公司', CAST(0x0000AAD100000000 AS DateTime), CAST(0x0000ADAB00000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市盈麦科技有限公司', CAST(0x0000A90C00000000 AS DateTime), CAST(0x0000AD5300000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市宝泰威科技有限公司', CAST(0x0000A8D500000000 AS DateTime), CAST(0x0000ABBC00000000 AS DateTime), 0.0035, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京柯创联华科技有限公司', CAST(0x0000A8B500000000 AS DateTime), CAST(0x0000ACFD00000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市旭联信息技术有限公司', CAST(0x0000AAD200000000 AS DateTime), CAST(0x0000ADAC00000000 AS DateTime), 0.003, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京乐美惠商贸有限公司', CAST(0x0000A93000000000 AS DateTime), CAST(0x0000AD7700000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
GO
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'盯盯拍（东莞）视觉设备有限公司', CAST(0x0000A8D500000000 AS DateTime), CAST(0x0000ABC900000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'上海楷贝电子科技有限公司', CAST(0x0000A72B00000000 AS DateTime), CAST(0x0000AA0400000000 AS DateTime), 0.003, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市创凯荣科技有限公司', CAST(0x0000A92000000000 AS DateTime), CAST(0x0000ABFB00000000 AS DateTime), 0.006, 500, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'南京研图电子科技有限公司', CAST(0x0000AA3100000000 AS DateTime), CAST(0x0000AE7800000000 AS DateTime), 0.006, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市瑾通科技有限公司', CAST(0x0000A8D400000000 AS DateTime), CAST(0x0000ABC200000000 AS DateTime), 0.006, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'苏州英诺迅科技股份有限公司', CAST(0x0000A8DC00000000 AS DateTime), CAST(0x0000AA4800000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市深港鸿裕光学玻璃材料有限公司', CAST(0x0000A90500000000 AS DateTime), CAST(0x0000ABE600000000 AS DateTime), 0.005, 600, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京科唯特技术有限公司', CAST(0x0000A61900000000 AS DateTime), CAST(0x0000AA5F00000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳华康捷电子有限公司', CAST(0x0000A56200000000 AS DateTime), CAST(0x0000A83C00000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市神牛摄影器材有限公司', CAST(0x0000A5BC00000000 AS DateTime), CAST(0x0000AA4000000000 AS DateTime), 0.003, 500, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市恩智成科技有限公司', CAST(0x0000A97B00000000 AS DateTime), CAST(0x0000ADC200000000 AS DateTime), 0.004, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京诺鑫致远科技有限责任公司', CAST(0x0000A94200000000 AS DateTime), CAST(0x0000AD8900000000 AS DateTime), 0.006, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市聚泉鑫科技有限公司', CAST(0x0000A8D500000000 AS DateTime), CAST(0x0000ABC900000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京远大创新科技有限公司深圳南山分公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.01, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京依利兴达电子有限公司', CAST(0x0000A8D900000000 AS DateTime), CAST(0x0000AD3400000000 AS DateTime), 0.005, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市凯芯诺科技有限公司', CAST(0x0000AAD200000000 AS DateTime), CAST(0x0000ADAC00000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳速美通网络有限公司', CAST(0x0000A8E800000000 AS DateTime), CAST(0x0000AA5500000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市海富睿科技有限公司', CAST(0x0000A96600000000 AS DateTime), CAST(0x0000ADAD00000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市艾莫贝斯科技有限公司', CAST(0x0000A8D900000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.003, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市强茂晟电子有限公司', CAST(0x0000A96100000000 AS DateTime), CAST(0x0000AC3C00000000 AS DateTime), 0.004, 400, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市东奥科技有限公司', CAST(0x0000A8D800000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.005, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市德派科技有限公司', CAST(0x0000A8D400000000 AS DateTime), CAST(0x0000ABB400000000 AS DateTime), 0.004, 350, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'东莞市勤微电子有限公司', CAST(0x0000A8D900000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市富昌雅利电子科技有限公司', CAST(0x0000A9DD00000000 AS DateTime), CAST(0x0000ACB800000000 AS DateTime), 0.005, 300, 1, 0, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京远大创新科技有限公司成都分公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.01, 0, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市金亚诺电子有限公司', CAST(0x0000A8E800000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.006, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市沁音创新科技有限公司', CAST(0x0000A8D900000000 AS DateTime), CAST(0x0000ABCB00000000 AS DateTime), 0.004, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市伯伦克实业有限公司', CAST(0x0000A93000000000 AS DateTime), CAST(0x0000AC1100000000 AS DateTime), 0.004, 500, 1, 0, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'成都方程式电子有限公司', CAST(0x0000A9A700000000 AS DateTime), CAST(0x0000ADEE00000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京华凯创新电子有限公司', CAST(0x0000A8D400000000 AS DateTime), CAST(0x0000ABC200000000 AS DateTime), 0.004, 400, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市高胜科研电子有限公司', CAST(0x0000A8CD00000000 AS DateTime), CAST(0x0000ABC900000000 AS DateTime), 0.006, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'杭州怀海电子科技有限公司', CAST(0x0000A8D500000000 AS DateTime), CAST(0x0000AD1C00000000 AS DateTime), 0.004, 500, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'深圳市恒晟辉电子有限公司', CAST(0x0000A8D600000000 AS DateTime), CAST(0x0000ABC300000000 AS DateTime), 0.005, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'北京日盛瑞达商贸中心', CAST(0x0000A7EC00000000 AS DateTime), CAST(0x0000AAC500000000 AS DateTime), 0.004, 300, 0, 1, 0, 0.13, N'NULL', N'NULL', N'NULL', N'NULL')
INSERT [dbo].[Data_Contract] ([客户名称], [开始日期], [结束日期], [代理费率], [最低代理费], [是否预换汇], [是否90天内换汇], [开票类型], [对应税点], [文件名称], [文件地址], [文件格式], [文件类型]) VALUES (N'杭州比一比电子科技有限公司', CAST(0x0000A8D300000000 AS DateTime), CAST(0x0000ABAE00000000 AS DateTime), 0.004, 0, 0, 1, 1, 0.06, N'NULL', N'NULL', N'NULL', N'NULL')

GO
--******************************  1.导入[Contracts] ******************************
insert into [dbo].[Contracts]
(	
	  [ID]
      ,[EnterpriseID]
      ,[StartDate]
      ,[EndDate]
      ,[ExchangeMode]
      ,[AgencyRate]
      ,[MinAgencyFee]
      ,[InvoiceType]
      ,[InvoiceTaxRate]
      ,[Status]
      ,[Creator]
      ,[CreateDate]
      ,[UpdateDate]
      ,[Summary]	     
)
select * from 
(
		select distinct 
		
			  --ID:string.Concat("DBAEAB43B47EB4299DD1D62F764E6B6A",EnterpriseID, StartDate, EndDate, 
			  --AgencyRate, MinAgencyFee, ExchangeMode, InvoiceType, InvoiceTaxRate).MD5()

			  [ID] = Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',
			 Convert(varchar(100), 'DBAEAB43B47EB4299DD1D62F764E6B6A'
			  +Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',[客户名称])),3,32))
			  +Convert(varchar(50),[开始日期])
			  +Convert(varchar(50),[结束日期])
			  +Convert(varchar(50),[代理费率])
			  +Convert(varchar(50),[最低代理费])
			  +case [是否预换汇] when 1 then 'PrePayExchange' else (case [是否90天内换汇] when 1 then 'LimitNinetyDays' else '0' end ) end
			  --+Convert(varchar(50),[开票类型])
			  +case [开票类型] when 0 then 'Full' else 'Service' end
			  +Convert(varchar(50),[对应税点])))),3,32))
			  
			  ,[EnterpriseID] = Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',[客户名称])),3,32))		
			  ,[StartDate] = [开始日期]
			  ,[EndDate] = [结束日期]
			  ,[ExchangeMode] = case [是否预换汇] when 1 then 1 else (case [是否90天内换汇] when 1 then 2 else 0 end ) end 			
			  ,[AgencyRate] = [代理费率]
			  ,[MinAgencyFee] = [最低代理费]
			  ,[InvoiceType] = [开票类型]
			  ,[InvoiceTaxRate] = [对应税点]
			  ,[Status] = 200
			  ,[Creator] = 'SA01'
			  ,[CreateDate] = GETDATE()
			  ,[UpdateDate] = GETDATE()
			  ,[Summary] = ''
		from Data_Contract
) as dc

where dc.ID not in ( select [ID] from [PvbCrm].[dbo].[Contracts] )

GO
--******************************  2.导入[MapsBEnter] ******************************

insert into [dbo].[MapsBEnter]
(		
      [ID]
      ,[Bussiness]
      ,[Type]
      ,[EnterpriseID]
      ,[SubID]
      ,[CtreatorID]
      ,[CreateDate]
      ,[IsDefault]
)
select * from 
(
		select distinct 	
			  --ID "Contract_"+合同ID 	  [SubID]=合同ID

			  [ID] ='Contract_'+ Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',
			 Convert(varchar(100), 'DBAEAB43B47EB4299DD1D62F764E6B6A'
			  +Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',[客户名称])),3,32))
			  +Convert(varchar(50),[开始日期])
			  +Convert(varchar(50),[结束日期])
			  +Convert(varchar(50),[代理费率])
			  +Convert(varchar(50),[最低代理费])
			  +case [是否预换汇] when 1 then 'PrePayExchange' else (case [是否90天内换汇] when 1 then 'LimitNinetyDays' else '0' end ) end
			  --+Convert(varchar(50),[开票类型])
			  +case [开票类型] when 0 then 'Full' else 'Service' end
			  +Convert(varchar(50),[对应税点])))),3,32))

			  ,[Bussiness] = 30
			  ,[Type] = 79
			  ,[EnterpriseID] = 'DBAEAB43B47EB4299DD1D62F764E6B6A'

			  ,[SubID] = Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',
			 Convert(varchar(100), 'DBAEAB43B47EB4299DD1D62F764E6B6A'
			  +Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',[客户名称])),3,32))
			  +Convert(varchar(50),[开始日期])
			  +Convert(varchar(50),[结束日期])
			  +Convert(varchar(50),[代理费率])
			  +Convert(varchar(50),[最低代理费])
			  +case [是否预换汇] when 1 then 'PrePayExchange' else (case [是否90天内换汇] when 1 then 'LimitNinetyDays' else '0' end ) end
			  --+Convert(varchar(50),[开票类型])
			  +case [开票类型] when 0 then 'Full' else 'Service' end
			  +Convert(varchar(50),[对应税点])))),3,32))

			  ,[CtreatorID] = 'SA01'
			  ,[CreateDate] = GETDATE()
			  ,[IsDefault] = 0
		from Data_Contract
) as dc

where dc.ID not in ( select [ID] from [PvbCrm].[dbo].[MapsBEnter] )

GO
--删除
IF EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[Data_Contract]') AND type in (N'U'))
BEGIN
drop table Data_Contract;
END

