Use PvbCrm
--***************************1.从MapsAdmin中导入权限关系
--1.1客户与管理员关系从MapsAdmin表导入MapsBEnter
insert into [PvbCrm].[dbo].[MapsBEnter]
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
				 [ID] =  Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',convert(varchar(100),'TradingClient_'+ RealID+AdminID
				 ))),3,32)) 

				 ,[Bussiness] = 10
				 ,[Type] = 20
				 ,[EnterpriseID] = RealID
		 
				 ,[SubID] =  AdminID	 	
	
				 ,[CtreatorID] =  AdminID
				 ,[CreateDate] = GETDATE()
				 ,[IsDefault] = IsDefault
		from  MapsAdmin where  [Type]=20 and (select count(ID) from Clients where ID =RealID)=1
) as dcii

where dcii.ID not in ( select [ID] from [PvbCrm].[dbo].[MapsBEnter] )

--1.2传统贸易发票数据导入关系表[MapsBEnter]
insert into [PvbCrm].[dbo].[MapsBEnter]
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
				 [ID] =  Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',convert(varchar(100),'TradingInvoice_'+ RealID
				 ))),3,32)) 

				 ,[Bussiness] = 10
				 ,[Type] = 74
				 ,[EnterpriseID] = (select EnterpriseID from Invoices where ID =RealID)
				 ,[SubID] =  RealID	 	
	
				 ,[CtreatorID] =  AdminID
				 ,[CreateDate] = getdate()
				 ,[IsDefault] = IsDefault
		from  MapsAdmin where [Type]=74 and (select count(ID) from Clients where ID =RealID)=1
) as dcii



where dcii.ID not in ( select [ID] from [PvbCrm].[dbo].[MapsBEnter] )


--1.3联系人数据添加关系

insert into [PvbCrm].[dbo].[MapsBEnter]
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
		
				 [ID] = Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingContact_'+RealID+AdminID))),3,32))
		
				,[Bussiness] = 10
				,[Type] = 73
				,[EnterpriseID] = (select EnterpriseID from Contacts where ID =RealID)

				,[SubID] =  RealID
		
				,[CtreatorID] = AdminID		
				,[CreateDate] =GETDATE()
				,[IsDefault] = IsDefault
		from  MapsAdmin where [Type]=73 and (select count(ID) from Clients where ID =RealID)=1
) as dcbi

where dcbi.ID not in ( select [ID] from [PvbCrm].[dbo].[MapsBEnter] )

GO

--1.4传统贸易客户的地址导入关系表
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
			  [ID] =   Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingConsignee_'+RealID+AdminID))),3,32))

			  ,[Bussiness] = 10
			  ,[Type] = 72
			  ,[EnterpriseID] =(select EnterpriseID from Consignees where ID =RealID)
			  ,[SubID] =RealID

			  ,[CtreatorID] = AdminID
			  ,[CreateDate] =  GETDATE()
			  ,[IsDefault] = IsDefault 
		from  MapsAdmin where [Type]=72 and  (select count(ID) from Clients where ID =RealID)=1
) as dcda 


where dcda.ID not in ( select [ID] from [PvbCrm].[dbo].[MapsBEnter] )





--**************************2.从数据表中导入传统贸易关系 

--2.1到货地址Consignees
--客户到货地址

GO
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
			  [ID] =   Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingConsignee_'+ID+AdminID))),3,32))

			  ,[Bussiness] = 10
			  ,[Type] = 72
			  ,[EnterpriseID] =(select ID from Enterprises where ID =EnterpriseID)
			  ,[SubID] =ID

			  ,[CtreatorID] = AdminID
			  ,[CreateDate] =  GETDATE()
			  ,[IsDefault] = 0 
		from  Consignees where  (select count(ID) from Clients where ID =EnterpriseID)=1
) as dcda 


where dcda.ID not in ( select [ID] from [PvbCrm].[dbo].[MapsBEnter] )

GO
--供应商到货地址
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
			  [ID] =   Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingConsignee_'+ID+AdminID))),3,32))

			  ,[Bussiness] = 10
			  ,[Type] = 72
			  ,[EnterpriseID] =(select ID from Enterprises where ID =EnterpriseID)
			  ,[SubID] =ID

			  ,[CtreatorID] = AdminID
			  ,[CreateDate] =  GETDATE()
			  ,[IsDefault] = 0 
		from  Consignees where  (select count(ID) from Suppliers where ID =EnterpriseID)=1
) as dcda 


where dcda.ID not in ( select [ID] from [PvbCrm].[dbo].[MapsBEnter] )

--2.2客户发票Invoices

insert into [PvbCrm].[dbo].[MapsBEnter]
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
				 [ID] =  Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',convert(varchar(100),'TradingInvoice_'+ ID
				 ))),3,32)) 

				 ,[Bussiness] = 10
				 ,[Type] = 74
				 ,[EnterpriseID] = (select EnterpriseID from Enterprises where ID =EnterpriseID)
				 ,[SubID] =  ID	 	
	
				 ,[CtreatorID] =  AdminID
				 ,[CreateDate] = CreateDate
				 ,[IsDefault] = 0
		from  Invoices where (select count(ID) from Clients where ID =EnterpriseID)=1
) as dcii



where dcii.ID not in ( select [ID] from [PvbCrm].[dbo].[MapsBEnter] )

GO
--供应商发票
insert into [PvbCrm].[dbo].[MapsBEnter]
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
				 [ID] =  Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',convert(varchar(100),'TradingInvoice_'+ ID
				 ))),3,32)) 

				 ,[Bussiness] = 10
				 ,[Type] = 74
				 ,[EnterpriseID] = (select EnterpriseID from Enterprises where ID =EnterpriseID)
				 ,[SubID] =  ID	 	
	
				 ,[CtreatorID] =  AdminID
				 ,[CreateDate] = CreateDate
				 ,[IsDefault] = 0
		from  Invoices where (select count(ID) from Suppliers where ID =EnterpriseID)=1
) as dcii



where dcii.ID not in ( select [ID] from [PvbCrm].[dbo].[MapsBEnter] )

GO


--联系人
--客户联系人
insert into [PvbCrm].[dbo].[MapsBEnter]
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
		
				 [ID] = Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingContact_'+Contacts.ID+AdminID))),3,32))
		
				,[Bussiness] = 10
				,[Type] = 73
				,[EnterpriseID] = (select EnterpriseID from Enterprises where  ID=EnterpriseID)

				,[SubID] =  ID
		
				,[CtreatorID] = AdminID		
				,[CreateDate] =GETDATE()
				,[IsDefault] = 0
		from  Contacts where (select count(ID) from Clients where ID =Contacts.EnterpriseID)=1
) as dcbi

where dcbi.ID not in ( select [ID] from [PvbCrm].[dbo].[MapsBEnter] )


GO
--供应商联系人
insert into [PvbCrm].[dbo].[MapsBEnter]
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
		
				 [ID] = Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingContact_'+Contacts.ID+AdminID))),3,32))
		
				,[Bussiness] = 10
				,[Type] = 73
				,[EnterpriseID] = (select EnterpriseID from Enterprises where  ID=EnterpriseID)

				,[SubID] =  ID
		
				,[CtreatorID] = AdminID		
				,[CreateDate] =GETDATE()
				,[IsDefault] = 0
		from  Contacts where (select count(ID) from Suppliers where ID =Contacts.EnterpriseID)=1
) as dcbi

where dcbi.ID not in ( select [ID] from [PvbCrm].[dbo].[MapsBEnter] )



--供应商受益人
insert into [PvbCrm].[dbo].[MapsBEnter]
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
		
				 [ID] = Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingBeneficiary_'+Beneficiaries.ID+AdminID))),3,32))
		
				,[Bussiness] = 10
				,[Type] = 71
				,[EnterpriseID] = (select EnterpriseID from Enterprises where  ID=EnterpriseID)

				,[SubID] =  ID
		
				,[CtreatorID] = AdminID		
				,[CreateDate] =GETDATE()
				,[IsDefault] = 0
		from  Beneficiaries where (select count(ID) from Suppliers where ID =Beneficiaries.EnterpriseID)=1
) as dcbi

where dcbi.ID not in ( select [ID] from [PvbCrm].[dbo].[MapsBEnter] )