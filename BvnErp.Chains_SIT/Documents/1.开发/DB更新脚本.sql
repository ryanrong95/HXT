--管理中心 BG1.0.3
--2019-01-11 荣检 更新报关单表
 --alter table DecHeads add Name varchar(30) not null 
alter table [dbo].[EntryNotices] add ClientCode varchar(50) not null
alter table [dbo].[EntryNotices] add SortingRequire int not null
alter table [dbo].[EntryNoticeItems] add IsSpotCheck bit not null
alter table [dbo].[Sortings] add CaseNumber varchar(50) not null
alter table [dbo].[sortings] drop column WaybillCode
alter table [dbo].[OrderWhesPremiums] add PaymentType int not null
alter table [dbo].[OrderWhesPremiums] add PayerType int not null

--2019-01-12 ryan 新增报关单表体栏位
alter table [dbo].[DecLists] add ContrItem decimal(19, 0) null
alter table [dbo].[DecLists] add OrigPlaceCode varchar(50) null

--2019-01-12 ryan 新增配置表
CREATE TABLE [dbo].[BaseDestCode](
	[ID] [varchar](50) NOT NULL,
	[Code] [varchar](8) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_BaseDestCode] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[BaseOriginArea](
	[ID] [varchar](50) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_BaseOriginArea] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

--2019-01-14 王增超 更新订单管控审核流程表
alter table [dbo].[OrderControlSteps] alter column AdminID varchar(50) null

--2019-01-14 王增超 更新订单附件表
alter table [dbo].[OrderFiles] alter column FileFormat varchar(50) not null

--2019-01-16 王增超 更新订单项表
alter table [dbo].[OrderItems] add IsSampllingCheck bit not null default(0)

--2019-01-16 荣检 更新报关单表
alter table [dbo].[DecHeads] alter column CusDecStatus varchar(1) null


 --管理中心 BG1.0.4

--2019-01-24 王增超 更新订单附件表
alter table [dbo].OrderFiles add FileStatus int not null default(100)

 --管理中心 BG1.0.5
 
 --2019-01-30 荣检 更新报关单表
alter table [dbo].[DecHeads] alter column CusDecStatus varchar(2) not null
alter table [dbo].[ManifestHeads] alter column CusDecStatus varchar(2) not null
alter table [dbo].[BaseCusReceiptCode] alter column Code varchar(2) not null

 --2019-02-12 荣检 更新报关单表
alter table [dbo].[DecHeads] drop column IsMarking;  
alter table [dbo].[Manifests] drop column IsMarking;

--2019-02-13 王增超 更新客户自定义产品税务归类表
alter table [dbo].ClientProductTaxCategories add TaxStatus int not null default(100)

--2019-02-13 王增超 新增产品税务归类基础信息表
CREATE TABLE [dbo].[TaxCategoriesDefaults](
	[ID] [varchar](50) NOT NULL,
	[TaxCode] [varchar](50) NOT NULL,
	[TaxFirstCategory] [nvarchar](50) NOT NULL,
	[TaxSecondCategory] [nvarchar](50) NOT NULL,
	[TaxThirdCategory] [nvarchar](50) NOT NULL,
	[Status] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Summary] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_TaxCategoriesDefault] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 


--2019-02-19 荣检 新增报关单申报人员姓名
alter table [dbo].[DecHeads] add DeclareName nvarchar(50) null

--2019-02-21 荣检 新增报关单订单ID
alter table [dbo].[DecHeads] add OrderID varchar(50) not null

--2019-02-21 王增超 新增订单类型
alter table [dbo].[Orders] add Type int not null default(200)


--2019-02-22 王增超 更新订单附件表
alter table [dbo].[OrderFiles] add OrderPremiumID varchar(50) null

--2019-02-26 董健 新增报关单的换汇状态
ALTER TABLE [dbo].[DecHeads] add SwapStatus int null
UPDATE [dbo].[DecHeads] set SwapStatus = 0;
ALTER TABLE [dbo].[DecHeads] ALTER COLUMN [SwapStatus][int] NOT NULL;


--2019-03-01 陆凯 集装箱规格字段更改
  alter table [ScCustoms].[dbo].[DecContainers] alter column ContainerMd varchar(2)
  

--2019-03-09 王增超 订单表件数字段更改
alter table [ScCustoms].[dbo].[Orders] alter column PackNo int NULL

--2019-03-09 荣检 舱单表字段更改
alter table [ScCustoms].[dbo].[Manifest] drop column AdminID;
alter table [ScCustoms].[dbo].[Manifest] drop column CusDecStatus;
alter table [ScCustoms].[dbo].[Manifest] drop column MarkingUrl;

alter table [dbo].[ManifestConsignments] add AdminID varchar(50) null;
alter table [dbo].[ManifestConsignments] add CreateDate DateTime null;
drop table [dbo].[ManifestTraces];


--2019-03-21 荣检 添加提成比例表
CREATE TABLE [dbo].[CommissionProportions](
	[ID] [varchar](50) NOT NULL,
	[RegeisterMonth] [int] NOT NULL,
	[Proportion] [decimal](18, 4) NOT NULL,
	[Status] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Summary] [nvarchar](500) NULL,
 CONSTRAINT [PK_CommissionProportions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

--2019-03-21 荣检 会员账户添加真实姓名栏位
alter table [dbo].[Users] add RealName nvarchar(150) not null;

--2019-04-12 区分大小写
alter table [ScCustoms].[dbo].[BaseDocuCode] ALTER COLUMN Code varchar(1) COLLATE Chinese_PRC_CS_AS
alter table [ScCustoms].[dbo].[DecLicenseDocus] ALTER COLUMN DocuCode varchar(1) COLLATE Chinese_PRC_CS_AS

--==================================================
--以上全部已更新到测试库，新的脚本请在下面添加↓↓↓
--==================================================
--2019-03-21 高超禹 银行账户修改银行代码为不必填，新增加列：自定义代码（可空）
alter table [ScCustoms].[dbo].[FinanceAccounts] alter column [SwiftCode] nvarchar(50) null;
alter table [ScCustoms].[dbo].[FinanceAccounts] add [CustomizedCode] nvarchar(50) null;

--2019-04-23 董健 快递公司表扩展自承运商表，删除重复列
alter table [ScCustoms].[dbo].[ExpressCompanys] drop column Name
alter table [ScCustoms].[dbo].[ExpressCompanys] drop column ShortName
alter table [ScCustoms].[dbo].[ExpressCompanys] drop column [Status]
alter table [ScCustoms].[dbo].[ExpressCompanys] drop column CreateDate
alter table [ScCustoms].[dbo].[ExpressCompanys] drop column UpdateDate

--2019-04-24 董健 新增报关单税费和其流水表
CREATE TABLE [ScCustoms].[dbo].[DecTaxs](
	[ID] [varchar](50) NOT NULL,
	[InvoiceType] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Summary] [nvarchar](300) NULL,
 CONSTRAINT [PK_DecTaxs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

CREATE TABLE [ScCustoms].[dbo].[DecTaxFlows](
	[ID] [varchar](50) NOT NULL,
	[DecTaxID] [varchar](50) NOT NULL,
	[BankName] [nvarchar](50) NOT NULL,
	[TaxNumber] [varchar](50) NOT NULL,
	[TaxType] [int]  NOT NULL,
	[PayDate] [datetime] NOT NULL,
	[DeductionTime] [datetime] NULL,
	[Amount] decimal(18,4) NOT NULL,
	[Status] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Summary] [nvarchar](300) NULL,
 CONSTRAINT [PK_DecTaxFlows] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

--2019-05-06 王增超 归类历史记录表添加AdminID
alter table [ScCustoms].[dbo].[ProductCategories] add AdminID varchar(50) not null;

--2019-05-06 王增超 新增产品归类锁定表
CREATE TABLE [ScCustoms].[dbo].[ProductClassifyLocks](
	[ID] [varchar](50) NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[LockDate] [datetime] NOT NULL,
	[AdminID] [varchar](50) NOT NULL,
	[Status] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Summary] [nvarchar](500) NULL,
 CONSTRAINT [PK_ProductClassifyLocks] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


--2019-08-15 王增超 新增订单产品变更日志表
CREATE TABLE [ScCustoms].[dbo].[OrderItemChangeLogs](
	[ID] [varchar](50) NOT NULL,
	[OrderItemID] [varchar](50) NOT NULL,
	[AdminID] [varchar](50) NOT NULL,
	[Type] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Summary] [nvarchar](500) NULL,
 CONSTRAINT [PK_OrderItemChangeLogs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_OrderItemChangeLogs_OrderItemID] ON [dbo].[OrderItemChangeLogs]([OrderItemID] ASC)
INCLUDE ([Type],[CreateDate],[Summary])

alter table [dbo].OrderItemChangeLogs add OrderID varchar(50)  not null
--2019-08-15 王增超 订单项变更通知表添加字段
alter table [dbo].[OrderItemChangeNotices] add OldValue nvarchar(150) null;
alter table [dbo].[OrderItemChangeNotices] add NewValue nvarchar(150) null;
alter table [dbo].[OrderItemChangeNotices] add IsSplited bit not null default(0);

--2019-08-16  靳珊珊     向车辆司机 承运商 表中添加字段;
--------------------start------------------------
alter table [dbo].[Carriers] add [Address] nvarchar(250) null;
alter table [dbo].[Vehicles] add [Weight] nvarchar(50) null;
alter table [dbo].[Drivers] add HSCode varchar(50) null;
alter table [dbo].[Drivers] add DriverCardNo varchar(50) null;
alter table [dbo].[Drivers] add HKMobile varchar(50) null;
alter table [dbo].[Drivers] add PortElecNo varchar(50) null;
alter table [dbo].[Drivers] add LaoPaoCode varchar(50) null;
-----------------------end ---------------------

--2019-08-26  杨樱   归类部分表调整
--------------------start------------------------

ALTER TABLE [dbo].[ProductCategoriesDefaults] DROP column tariffrate,addedvaluerate,classifytype,unit1,unit2;

alter table orderitems    add Name nvarchar(150)  null, Model nvarchar(150)  null,Manufacturer nvarchar(50)  null,Batch varchar(50) null;

update [dbo].[OrderItems] 
SET OrderItems.name=c.name,model=c.model,Manufacturer=c.Manufacturer,batch=c.batch
from(select b.id, b.name ,b.model,b.Manufacturer,b.batch
from [dbo].[Products] b) as c
where productid=c.id;

  ALTER TABLE [dbo].[OrderItems]   ALTER COLUMN
Name nvarchar(150) not null;

  ALTER TABLE [dbo].[OrderItems]   ALTER COLUMN
Model nvarchar(150) not null;

  ALTER TABLE [dbo].[OrderItems]   ALTER COLUMN
Manufacturer nvarchar(50) not null;

alter table orderitems drop constraint FK_OrderItems_Products
drop index IX_OrderItems_ProductID on orderitems ;
ALTER TABLE orderitems DROP column productid;

alter table [dbo].[ClientProducts]    add Name nvarchar(150)  null, Model nvarchar(150)  null,Manufacturer nvarchar(50)  null,Batch varchar(50) null;

update [dbo].[clientproducts] 
SET name=c.name,model=c.model,Manufacturer=c.Manufacturer,batch=c.batch
from(select b.id, b.name ,b.model,b.Manufacturer,b.batch
from [dbo].[Products] b) as c
where productid=c.id;

   ALTER TABLE [dbo].[clientproducts]   ALTER COLUMN
Name nvarchar(150) not null;

  ALTER TABLE [dbo].[clientproducts]   ALTER COLUMN
Model nvarchar(150) not null;

  ALTER TABLE [dbo].[clientproducts]   ALTER COLUMN
Manufacturer nvarchar(50) not null;

alter table [ClientProducts] drop constraint FK_ClientProducts_Products

ALTER TABLE [dbo].[ClientProducts] DROP column productid;

ALTER TABLE [dbo].[ProductCategories]   ALTER COLUMN
  name nvarchar(150) null;

  ALTER TABLE [dbo].[ProductCategories]   ALTER COLUMN
  HSCode varchar(50) null;

  ALTER TABLE [dbo].[ProductCategories]   ALTER COLUMN
  [TariffRate] decimal(18,4) null;

ALTER TABLE [dbo].[ProductCategories]   ALTER COLUMN
[AddedValueRate] decimal(18,4) null;

  ALTER TABLE [dbo].[ProductCategories]   ALTER COLUMN
[UnitPrice] decimal(19,5) null;

  ALTER TABLE [dbo].[ProductCategories]   ALTER COLUMN
[InspectionFee] decimal(18,4) null;

  ALTER TABLE [dbo].[ProductCategories]   ALTER COLUMN
[Quantity] decimal(18,4) null;

drop index IX_Sortings_ProductID on [Sortings] ;

ALTER TABLE [dbo].[Sortings] DROP column productid;

drop index IX_StoreStorages_ProductID on [StoreStorages] ;

ALTER TABLE [dbo].[StoreStorages] DROP column productid;
-----------------------end ---------------------

--2019-08-27  陈猛   OrderItemTaxes 表增加字段 ReceiptRate, 并将旧数据的 Rate 字段导入 ReceiptRate 字段
--------------------start------------------------
ALTER TABLE dbo.OrderItemTaxes ADD ReceiptRate DECIMAL(18, 4) NULL;

UPDATE  dbo.OrderItemTaxes
SET     ReceiptRate = Rate;

ALTER TABLE dbo.OrderItemTaxes ALTER COLUMN ReceiptRate DECIMAL(18, 4) NOT NULL;
-----------------------end ---------------------

--2019-09-12  陈猛   Voyages 表增加 16 个字段
--------------------start------------------------
ALTER TABLE dbo.Voyages ADD CarrierType INT NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Carriers.CarrierType', @level0type = N'SCHEMA',
    @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Voyages',
    @level2type = N'COLUMN', @level2name = N'CarrierType';

ALTER TABLE dbo.Voyages ADD CarrierName NVARCHAR(150) NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Carriers.Name', @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'Voyages', @level2type = N'COLUMN',
    @level2name = N'CarrierName';

ALTER TABLE dbo.Voyages ADD CarrierQueryMark NVARCHAR(150) NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Carriers.QueryMark', @level0type = N'SCHEMA',
    @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Voyages',
    @level2type = N'COLUMN', @level2name = N'CarrierQueryMark';

ALTER TABLE dbo.Voyages ADD ContactMobile VARCHAR(50) NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Contacts.Mobile', @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'Voyages', @level2type = N'COLUMN',
    @level2name = N'ContactMobile';

ALTER TABLE dbo.Voyages ADD CarrierAddress NVARCHAR(250) NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Carriers.Address', @level0type = N'SCHEMA',
    @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Voyages',
    @level2type = N'COLUMN', @level2name = N'CarrierAddress';

ALTER TABLE dbo.Voyages ADD ContactName NVARCHAR(150) NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Contacts.Name', @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'Voyages', @level2type = N'COLUMN',
    @level2name = N'ContactName';

ALTER TABLE dbo.Voyages ADD ContactFax VARCHAR(50) NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Contacts.Fax', @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'Voyages', @level2type = N'COLUMN',
    @level2name = N'ContactFax';

ALTER TABLE dbo.Voyages ADD VehicleType INT NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Vehicles.VehicleType', @level0type = N'SCHEMA',
    @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Voyages',
    @level2type = N'COLUMN', @level2name = N'VehicleType';

ALTER TABLE dbo.Voyages ADD VehicleLicence VARCHAR(50) NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Vehicles.Licence', @level0type = N'SCHEMA',
    @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Voyages',
    @level2type = N'COLUMN', @level2name = N'VehicleLicence';

ALTER TABLE dbo.Voyages ADD VehicleWeight NVARCHAR(50) NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Vehicles.Weight', @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'Voyages', @level2type = N'COLUMN',
    @level2name = N'VehicleWeight';

ALTER TABLE dbo.Voyages ADD DriverMobile VARCHAR(50) NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Drivers.Mobile', @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'Voyages', @level2type = N'COLUMN',
    @level2name = N'DriverMobile';

ALTER TABLE dbo.Voyages ADD DriverHSCode VARCHAR(50) NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Drivers.HSCode', @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'Voyages', @level2type = N'COLUMN',
    @level2name = N'DriverHSCode';

ALTER TABLE dbo.Voyages ADD DriverHKMobile VARCHAR(50) NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Drivers.HKMobile', @level0type = N'SCHEMA',
    @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Voyages',
    @level2type = N'COLUMN', @level2name = N'DriverHKMobile';

ALTER TABLE dbo.Voyages ADD DriverCardNo VARCHAR(50) NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Drivers.DriverCardNo', @level0type = N'SCHEMA',
    @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Voyages',
    @level2type = N'COLUMN', @level2name = N'DriverCardNo';

ALTER TABLE dbo.Voyages ADD DriverPortElecNo VARCHAR(50) NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Drivers.PortElecNo', @level0type = N'SCHEMA',
    @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Voyages',
    @level2type = N'COLUMN', @level2name = N'DriverPortElecNo';

ALTER TABLE dbo.Voyages ADD DriverLaoPaoCode VARCHAR(50) NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Drivers.LaoPaoCode', @level0type = N'SCHEMA',
    @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Voyages',
    @level2type = N'COLUMN', @level2name = N'DriverLaoPaoCode';
-----------------------end ---------------------

--2019-09-30  陈猛   ExitNotices 表增加 OutStockTime 字段
--------------------start------------------------
ALTER TABLE dbo.ExitNotices ADD OutStockTime DATETIME NULL;
-----------------------end ---------------------

--2019-10-23  陈猛   DecTaxs 表增加 HandledType 字段
--------------------start------------------------
ALTER TABLE dbo.DecTaxs ADD HandledType INT NULL;
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'已处理类型',
    @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE',
    @level1name = N'DecTaxs', @level2type = N'COLUMN',
    @level2name = N'HandledType';
ALTER TABLE dbo.DecTaxs ADD DEFAULT (0) FOR HandledType WITH VALUES;
-----------------------end ---------------------

ALTER TABLE dbo.OrderItems ADD AutoClassifyFlag INT NOT NULL DEFAULT(2);

--2019-11-08  陈猛   OrderControlSteps 表增加 UserID 字段
--------------------start------------------------
ALTER TABLE dbo.OrderControlSteps ADD UserID VARCHAR(50) NULL;
-----------------------end ---------------------


--2019-11-14  liuzuoxiang  视图 OrderItemRateView 增加 ImportTaxValue、 AddedValue 、ConsumeTaxValue

GO

/****** Object:  View [dbo].[OrderItemRateView]    Script Date: 2019/11/14 14:19:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER VIEW [dbo].[OrderItemRateView]
AS
   
SELECT  OrderItems.ID AS OrderItemID ,
            OrderItemTaxesTab.ImportTaxRate ,
            OrderItemTaxesTab.ImportTaxReceiptRate,
			 OrderItemTaxesTab.ImportTaxValue,
            OrderItemTaxesTab.AddedValueTaxRate ,
            OrderItemTaxesTab.AddedValueTaxReceiptRate ,
			OrderItemTaxesTab.AddedValue,
            OrderItemTaxesTab.ConsumeTaxRate ,
            OrderItemTaxesTab.ConsumeTaxReceiptRate,
			OrderItemTaxesTab.ConsumeTaxValue,
            Orders.CustomsExchangeRate,
            Orders.RealExchangeRate
    FROM    dbo.OrderItems
            LEFT JOIN ( SELECT  OrderItemTaxes.OrderItemID,
                                SUM(CASE OrderItemTaxes.Type
                                      WHEN 0 THEN OrderItemTaxes.Rate
                                      ELSE NULL
                                    END) AS ImportTaxRate,
                                SUM(CASE OrderItemTaxes.Type
                                      WHEN 0 THEN OrderItemTaxes.ReceiptRate
                                      ELSE NULL
                                    END) AS ImportTaxReceiptRate,
							   SUM(CASE OrderItemTaxes.Type
                                      WHEN 0 THEN OrderItemTaxes.Value
                                      ELSE NULL
                                    END) AS ImportTaxValue,
                                SUM(CASE OrderItemTaxes.Type
                                      WHEN 2 THEN OrderItemTaxes.Rate
                                      ELSE NULL
                                    END) AS AddedValueTaxRate,
                                SUM(CASE OrderItemTaxes.Type
                                      WHEN 2 THEN OrderItemTaxes.ReceiptRate
                                      ELSE NULL
                                    END) AS AddedValueTaxReceiptRate ,
						      SUM(CASE OrderItemTaxes.Type
                                      WHEN 2 THEN OrderItemTaxes.Value
                                      ELSE NULL
                                    END) AS AddedValue,
                                SUM(CASE OrderItemTaxes.Type
                                      WHEN 3 THEN OrderItemTaxes.Rate
                                      ELSE NULL
                                    END) AS ConsumeTaxRate,
                                SUM(CASE OrderItemTaxes.Type
                                      WHEN 3 THEN OrderItemTaxes.ReceiptRate
                                      ELSE NULL
                                    END) AS ConsumeTaxReceiptRate,
							  SUM(CASE OrderItemTaxes.Type
                                      WHEN 3 THEN OrderItemTaxes.Value
                                      ELSE NULL
                                    END) AS ConsumeTaxValue
                        FROM    dbo.OrderItemTaxes
                        WHERE   OrderItemTaxes.Status = 200
                        GROUP BY OrderItemTaxes.OrderItemID
                      ) OrderItemTaxesTab ON OrderItems.ID = OrderItemTaxesTab.OrderItemID
            LEFT JOIN dbo.Orders ON OrderItems.OrderID = Orders.ID
                                    AND Orders.Status = 200
    WHERE   OrderItems.Status = 200;

GO

-----------------------end ---------------------

--2019-11-21  liuzuoxiang  新增视图 [OrderPayExchangeSuppliersView 

GO

/****** Object:  View [dbo].[OrderPayExchangeSuppliersView]    Script Date: 2019/11/21 17:52:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[OrderPayExchangeSuppliersView]
AS
SELECT   a.ID, a.OrderID, b.ID AS SupplierID, b.Name, b.ChineseName
FROM      dbo.OrderPayExchangeSuppliers AS a LEFT OUTER JOIN
                dbo.ClientSuppliers AS b ON a.ClientSupplierID = b.ID
WHERE   (a.Status = 200)

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "a"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 220
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "b"
            Begin Extent = 
               Top = 6
               Left = 258
               Bottom = 146
               Right = 427
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OrderPayExchangeSuppliersView'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OrderPayExchangeSuppliersView'
GO


-----------------------end ---------------------

--2019-11-23  陈猛   PayExchangeApplyItems 表增加 ApplyStatus 字段，并设置默认值为 1
--------------------start------------------------
ALTER TABLE dbo.PayExchangeApplyItems ADD ApplyStatus INT NULL;
ALTER TABLE dbo.PayExchangeApplyItems ADD DEFAULT (1) FOR ApplyStatus WITH VALUES;
-----------------------end ---------------------

--2019-11-25  陈猛   新建表 PayApplySwapNoticeItemRelation
--------------------start------------------------
CREATE TABLE [dbo].[PayApplySwapNoticeItemRelation]
    (
      [ID] [VARCHAR](50) NOT NULL ,
      [PayExchangeApplyItemID] [VARCHAR](50) NOT NULL ,
      [SwapNoticeItemID] [NVARCHAR](150) NOT NULL ,
      [Status] [INT] NOT NULL ,
      [CreateDate] [DATETIME] NOT NULL ,
      [UpdateDate] [DATETIME] NOT NULL ,
      [Summary] [NVARCHAR](400) NULL
    );

ALTER TABLE dbo.PayApplySwapNoticeItemRelation ADD PRIMARY KEY(ID);
-----------------------end ---------------------

--2019-11-25  陈猛   SwapNoticeItems 表增加字段 CustomizeAmount, IsOld
--------------------start------------------------
ALTER TABLE dbo.SwapNoticeItems ADD CustomizeAmount DECIMAL(18, 4) NULL, IsOld BIT NULL;

UPDATE  dbo.SwapNoticeItems
SET     CustomizeAmount = 0 ,
        IsOld = 1;

ALTER TABLE dbo.SwapNoticeItems ALTER COLUMN CustomizeAmount DECIMAL(18, 4) NOT NULL;
-----------------------end ---------------------

--2019-12-05  陈猛   增加表 PayExchangeSensitiveAreas、PayExchangeSensitiveWords
--------------------start------------------------
CREATE TABLE [dbo].[PayExchangeSensitiveAreas]
    (
      [ID] [VARCHAR](50) NOT NULL ,
      [Type] [INT] NOT NULL ,
	  [Name] [NVARCHAR](50) NOT NULL,
      [Status] [INT] NOT NULL ,
      [CreateDate] [DATETIME] NOT NULL ,
      [UpdateDate] [DATETIME] NOT NULL ,
      [Summary] [NVARCHAR](400) NULL
    );

ALTER TABLE dbo.PayExchangeSensitiveAreas ADD PRIMARY KEY(ID);

CREATE TABLE [dbo].[PayExchangeSensitiveWords]
    (
      [ID] [VARCHAR](50) NOT NULL ,
      [AreaID] [VARCHAR](50) NOT NULL ,
      [Content] [VARCHAR](200) NOT NULL ,
      [Status] [INT] NOT NULL ,
      [CreateDate] [DATETIME] NOT NULL ,
      [UpdateDate] [DATETIME] NOT NULL ,
      [Summary] [NVARCHAR](400) NULL
    );

ALTER TABLE dbo.PayExchangeSensitiveWords ADD PRIMARY KEY(ID);
-----------------------end ---------------------

--2020-03-04  陈猛   增加表 AttachApprovalLogs
--------------------start------------------------
USE [foricScCustoms]
GO

/****** Object:  Table [dbo].[AttachApprovalLogs]    Script Date: 2020/3/17 9:49:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[AttachApprovalLogs](
	[ID] [VARCHAR](50) NOT NULL,
	[OrderControlID] [VARCHAR](50) NOT NULL,
	[Status] [INT] NOT NULL,
	[CreateDate] [DATETIME] NOT NULL,
	[Summary] [NVARCHAR](1000) NULL,
 CONSTRAINT [PK_AttachApprovalLogs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主键ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AttachApprovalLogs', @level2type=N'COLUMN',@level2name=N'ID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'日志文字内容' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AttachApprovalLogs', @level2type=N'COLUMN',@level2name=N'Summary'
GO
-----------------------end ---------------------

--2020-03-05  陈猛   表 OrderControls 增加字段 EventInfo
--------------------start------------------------
ALTER TABLE dbo.OrderControls ADD EventInfo NVARCHAR(MAX) NULL;
-----------------------end ---------------------

--2020-03-05  陈猛   表 OrderControls 增加字段 Applicant
--------------------start------------------------
ALTER TABLE dbo.OrderControls ADD Applicant VARCHAR(50) NULL;
-----------------------end ---------------------

--2020-03-05  陈猛   表 OrderControls 增加字段 MainOrderID
--------------------start------------------------
ALTER TABLE dbo.OrderControls ADD MainOrderID VARCHAR(50) NULL;
-----------------------end ---------------------

--2020-03-06  陈猛   删除 表Orders与表OrderControls的主外键关系, 将表OrderControls中OrderID字段设置为可空
--------------------start------------------------
SELECT  name
FROM    sys.foreign_key_columns f
        JOIN sys.objects o ON f.constraint_object_id = o.object_id
WHERE   f.parent_object_id = OBJECT_ID('OrderControls');

ALTER TABLE dbo.OrderControls DROP CONSTRAINT FK_OrderControls_Orders;

ALTER TABLE dbo.OrderControls ALTER COLUMN OrderID VARCHAR(50) NULL;
-----------------------end ---------------------

--2020-03-09  陈猛   表 OrderControlSteps 增加字段
--------------------start------------------------
ALTER TABLE dbo.OrderControlSteps ADD ApproveReason VARCHAR(1024) NULL;
ALTER TABLE dbo.OrderControlSteps ADD ReferenceInfo NVARCHAR(MAX) NULL;
-----------------------end ---------------------

--2020-03-24  陈猛   表 PaymentNotices 增加字段 CostApplyID
--------------------start------------------------
ALTER TABLE dbo.PaymentNotices ADD CostApplyID VARCHAR(50) NULL;
-----------------------end ---------------------
