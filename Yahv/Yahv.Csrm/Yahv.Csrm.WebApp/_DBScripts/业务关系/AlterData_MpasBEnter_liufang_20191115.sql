Use PvbCrm
--***************************1.��MapsAdmin�е���Ȩ�޹�ϵ
--1.1�ͻ������Ա��ϵ��MapsAdmin����MapsBEnter
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

--1.2��ͳó�׷�Ʊ���ݵ����ϵ��[MapsBEnter]
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


--1.3��ϵ��������ӹ�ϵ

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

--1.4��ͳó�׿ͻ��ĵ�ַ�����ϵ��
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





--**************************2.�����ݱ��е��봫ͳó�׹�ϵ 

--2.1������ַConsignees
--�ͻ�������ַ

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
--��Ӧ�̵�����ַ
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

--2.2�ͻ���ƱInvoices

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
--��Ӧ�̷�Ʊ
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


--��ϵ��
--�ͻ���ϵ��
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
--��Ӧ����ϵ��
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



--��Ӧ��������
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