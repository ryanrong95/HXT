USE [PvbCrm]
GO
/****** Object:  Table [dbo].[Data_SupplierDeliveryAddress]    Script Date: 2019/10/23 14:49:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[Data_SupplierDeliveryAddress]') AND type in (N'U'))
BEGIN

CREATE TABLE [dbo].[Data_SupplierDeliveryAddress](
	[客户名称] [varchar](255) NULL,
	[供应商的中文名称] [varchar](255) NULL,
	[英文名称] [varchar](255) NULL,
	[联系人] [varchar](255) NULL,
	[联系人电话] [varchar](255) NULL,
	[提货地址] [varchar](255) NULL,
	[备注] [varchar](255) NULL
) ON [PRIMARY]
END
GO
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'北京创新在线电子产品销售有限公司杭州分公司', N'ICGOO ELECTRONICS LIMITED', N'ICGOO ELECTRONICS LIMITED', N'林团裕', N'（852）31019258', N'香港 九龙城区 香港九龙官塘鸿图道', NULL)
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市同聚富电子有限公司', N'SUNRISING LOGISTICS (HK) LIMITED', N'SUNRISING LOGISTICS (HK) LIMITED', N'111', N'00852 34264944', N'香港  中西区 16th Floor, The center 99 queen’s road central, central Hong Kong', N'NULL')
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市麦思浦半导体有限公司', N'MAGNACHIP SEMICONDUCTOR LTD.', N'MAGNACHIP SEMICONDUCTOR LTD.', N'Luke Han', N'2479 1070', N'香港 中西区 香港新界荃灣楊屋道168號國際訊通中心8樓803室', NULL)
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市欧时迈电子有限公司', N'香港品保信息技术有限公司', N'HK PINBALL INFORMATION TECHNOLOGY LIMITED', N'梁慧姿', N'00852-24390300', N'香港 中西区 天水围流浮山新庆村120号-1（新界元朗流浮山DD129一辉物流中心润泰仓）', NULL)
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市金亚诺电子有限公司', N'雅讯电子（香港）有限公司', N'Asia-Communication（HK） Electronics Company limited ', N'lydia deng', N'00852-23192933', N'香港 油尖旺区 香港九龙长沙湾道72号昌明大厦8/FB室', NULL)
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'北京科瑞泰科电子技术有限公司', N'MULTAN TECHNOLOGY LIMITED', N'MULTAN TECHNOLOGY LIMITED', N'chendafa', N'85234264944', N'香港 中西区 2/F,HSBCBUILDINGMONGKOK,673NATHANROAD,MONGKOK,KOWLOON', NULL)
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市斯迈尔电子有限公司', N'深圳市斯迈尔电子有限公司', N'Omron Microscan Systems, Inc.', N'深圳市斯迈尔电子有限公司', N'18925271595', N'香港  荃湾区 工业大厦106', N'NULL')
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市盈科泰电子有限公司', N'嘉兆科技有限公司', N'Corad Technology Ltd.', N'coco', N'0755-8252 3549', N'香港 观塘区 鸿图道57号南洋广场13层1306室', NULL)
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市华控技术科技有限公司', N'日昇科技有限公司', N'RISON TECHNOLOGY(HK)CO.,LINITED', N'曾小姐', N'13620988602', N'香港 中西区 香港皇后大道中181号新纪元广场1501室', NULL)
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市华控技术科技有限公司', N'富昌電子(香港)有限公司', N'Future Electronics (Hong Kong) Ltd', N'陈小姐', N'13620988602', N'香港 中西区 香港新界葵芳興芳路223號', NULL)
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市中电网络技术有限公司', N'IC360', N'IC360 GROUP LIMITED', N'林团裕', N'（852）31019258', N'香港 九龙城区 香港九龙官塘鸿图道', NULL)
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市明智芯科技有限公司', N'合顺芯贸易（香港）有限公司', N'Heshunxin Trading(HK)Limited', N'afa', N'864656123', N'香港  中西区 safhalghdshgdj', N'NULL')
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市宝泰威科技有限公司', N'炬力科技（香港）有限公司', N'ACTIONS TECHNOLOGY(HK) COMPANY LIMITED', N'赵彩霞', N'0756-3392353-1183', N'香港 中西区 香港九龍荔枝角永康街7號西港都會中心六樓F室', NULL)
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'北京时代创兴电子技术有限公司', N'文晔科技股份有限公司', N'WT Micrpelectronics CO., Limited', N'文曄科技股份有限公司', N'13803379480', N'香港 中西区 新北市中和區中正路738號14樓', NULL)
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'厦门致鑫和电子有限公司', N'ihub', N'ihub SOLUTIONS(HK)LTD', N'Kenny Leung', N'(852) 2191 9178', N'香港 中西区 Unit6AYoungYaIndustrialBuilding381-389ShaTsuiRoad,TsuenWanHongKong', NULL)
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'杭州博吉通电子有限公司', N'Connective, Inc', N'Connective, Inc', N'Oscar Ma', N'852 2643 9108', N'香港  中西区 122', N'NULL')
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'北京柯创联华科技有限公司', N'COMTECH INDUSTRIAL(HONGKONG)LIMITED', N'COMTECH INDUSTRIAL(HONGKONG)LIMITED', N'李小姐', N'15223371390', N'香港 东区 火炭坳背湾街53-44号美高工业大厦14楼', N'联系电话00852-23371390 需要提货单号提货')
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市中科信诺电子有限公司', N'易達通實業（香港）有限公司', N'E-REACH INDUSTRIAL LTD.', N'梁小姐 Winnie ', N'00852-35952155', N'香港  中西区 香港九龍觀塘鴻圖道79號 嘉士亞洲工業大廈 10樓A室', N'NULL')
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市快包电子科技有限责任公司', N'source', N'source', N'林团裕', N'（852）31019258', N'香港 九龙城区 香港九龙官塘鸿图道', NULL)
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'江西吉为科技有限公司', N'景汉企业有限公司', N'King Horn Enterprises Ltd', N'chendafa', N'85234264944', N'香港 中西区 ROOM205，2/F,WinFulCenter，30ShingYipStreet，KwunTong，Kowloon，HK', NULL)
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'厦门致鑫和电子有限公司', N'致和企业（香港）有限公司', N'UNISOM ENTERPRISES CO., LIMITED', N'Veronica ', N'15960240336', N'香港 九龙城区 Unit720,7/F.,ChevalierCommercialCentre,8WangHoiRoad,KowloonBay,HongKong', N'852-3427 3790 (ext. 503)')
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'上海旌睿电子科技有限公司', N'Tocom Technologies CO. Limited', N'Tocom Technologies CO. Limited', N'TAMMY MO', N'(852) 3761-7094', N'香港  北区 26/F,  Goodman Tsuen Wan Centre, 68 Wang Lung Street,Tsuen Wan, HK，香港荃灣橫龍街 68 號嘉民荃灣中心 26 樓', N'NULL')
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'厦门致鑫和电子有限公司', N'致和企业（香港）有限公司', N'UNISOM ENTERPRISES CO., LIMITED', N'Kenny Leung', N'(852) 2191 9178', N'香港 中西区 Unit6AYoungYaIndustrialBuilding381-389ShaTsuiRoad,TsuenWanHongKong', NULL)
--INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市玮华电子有限公司', N'TTI', N'TTI ELECTRONICS ASIA PTE LTD', N'Sandy ma', N'3658 4918', N'香港  油尖旺区 香港九龙湾，1-3启兴道九龙货仓1楼', N'NULL')
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市伯乐马电子有限公司', N'億光電子(香港)有限公司', N'EVLITE ELECTRONICS CO.,LTD.', N'Alex Cheung', N'+852 23748868', N'香港 中西区 香港九龍官塘成業街6號泓富廣場1606-1608室', NULL)
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市宝泰威科技有限公司', N'阿尔福微电子（深圳）有限公司', N'Longtern Technology(HK)Limited', N'阿ken', N'852 6096 3777', N'香港 中西区 香港新界葵涌梨木道32--50号金运工业大厦2期18楼I室。', NULL)
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市玮华电子有限公司', N'TTI', N'TTI ELECTRONICS ASIA PTE LTD', N'Sandy ma', N'36584918', N'香港  油尖旺区 香港九龙湾，1-3启兴道九龙货仓1楼', N'NULL')
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市信心智能标签技术有限公司', N'深圳市信心智能标签技术有限公司', N'深圳市信心智能标签技术有限公司', N'谢美琼', N'15919652225', N'香港  中西区 香港沙田火炭山尾街 31-41号华乐工业中心A座22楼13室', N'NULL')
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市盈科泰电子有限公司', N'深圳市汉威防务科技有限公司', N'Hanwell TEchnology CO.,Limited', N'唐小姐', N'13724222490', N'广东 深圳 福田区 新沙路北侧国都高尔夫花园绿怡轩29A', N'NULL')
INSERT [dbo].[Data_SupplierDeliveryAddress] ([客户名称], [供应商的中文名称], [英文名称], [联系人], [联系人电话], [提货地址], [备注]) VALUES (N'深圳市华控技术科技有限公司', N'新烨电子（香港）有限公司', N'SERIAL MICROELECTRONICS (HK) LTD.', N'陈小姐', N'13620988602', N'香港 中西区 香港九龙观塘裕民坊1号九龙东区商业银行中心2楼', NULL)





--******************************  1.导入[Consignors] ******************************

insert into [dbo].[Consignors]
(	
	  [ID]
      ,[EnterpriseID]
      ,[Title]
      ,[DyjCode]
      ,[Postzip]
      ,[Province]
      ,[City]
      ,[Land]
      ,[Address]
      ,[Status]
      ,[Name]
      ,[Tel]
      ,[Mobile]
      ,[Email]
      ,[CreateDate]
      ,[UpdateDate]
      ,[AdminID]
      ,[IsDefault]
    
)
select * from 
(
		select distinct 	
			  --ID:(EnterpriseID,DyjCode,Address,Postzip).MD5()
			  [ID] = Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',
			 Convert(varchar(100), Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',[供应商的中文名称])),3,32))+''+[提货地址]+''))),3,32))	

			  ,[EnterpriseID] = Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',[供应商的中文名称])),3,32)) 
			  ,[Title] = null
			  ,[DyjCode] = ''
			  ,[Postzip] = ''
			  ,[Province] = null
			  ,[City] =null
			  ,[Land] = null
			  ,[Address] = [提货地址]
			  ,[Status] = 200
			  ,[Name] = [联系人]
			  ,[Tel] = [联系人电话]
			  ,[Mobile] =null
			  ,[Email] =null
			  ,[CreateDate] = GETDATE()
			  ,[UpdateDate] = GETDATE()
			  ,[AdminID] = 'SA01'
			  ,[IsDefault] = 0
	 
		from Data_SupplierDeliveryAddress 
) as dsda 

where dsda.ID not in ( select ID from [Consignors] )




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
			  --ID:"WsConsignor_" + string.Join(this.WsClient.ID, [dbo].[Consignors].ID).MD5();
			  --Type:77

			  [ID] ='WsConsignor_'+ Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',
			Convert(varchar(100),  Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',[客户名称])),3,32))
			  +Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',
			 Convert(varchar(100), Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',[供应商的中文名称])),3,32))+''+[提货地址]+''))),3,32))
			  )))
			  ,3,32))


			  ,[Bussiness] = 30
			  ,[Type] = 77
			  ,[EnterpriseID] = Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',[客户名称])),3,32)) 
			  ,[SubID] = Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',
			 Convert(varchar(100), Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',[供应商的中文名称])),3,32))+''+[提货地址]+''))),3,32))	

			  ,[CtreatorID] = ''
			  ,[CreateDate] = GETDATE()
			  ,[IsDefault] = 0
	 
		from Data_SupplierDeliveryAddress
) as dsda


where dsda.ID not in ( select ID from [MapsBEnter] )


IF EXISTS (SELECT * FROM sys.objects WHERE object_id =
OBJECT_ID(N'[dbo].[Data_SupplierDeliveryAddress]') AND type in (N'U'))
BEGIN
drop table [dbo].[Data_SupplierDeliveryAddress];
END

