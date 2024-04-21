SELECT pyes.[ID]
      ,[EnterpriseID]= ep1.[ID]
      ,[EnterpriseName]= ep1.Name
      ,[RealEnterpriseID] = ep2.ID
      ,[RealEnterpriseName] = ep2.Name
      ,[nGrade] = 0
      ,[Methord]
      ,[Bank]
      ,[BankAddress]
      ,[Account]
      ,[SwiftCode]
      ,[Currency]
      ,Contact= pyes.[Name] 
      ,[Tel]
      ,[Mobile]
      ,[Email]
      ,pyes.[Status]
      ,client.EnterCode
      ,clientGrade=client.Grade
      ,Place = ep1.Place
  FROM [dbo].[Payees] as  pyes  
  join Enterprises as ep1 on pyes.EnterpriseID = ep1.ID
  left join Enterprises as ep2 on pyes.[RealID] = ep1.ID
  join WsClients as client  on ep1.[ID] =client.ID

union all 

SELECT nspplier.[ID]
      ,[OwnID] =  ep1.[ID]
      ,[OwnName]= ep1.Name
      ,[RealEnterpriseID] = ep2.ID
      ,[RealEnterpriseName] = ep2.Name
      --,[Conduct]
      --,[ChineseName]
      --,[EnglishName]
      ,[nGrade] = nspplier.Grade
      ,[Methord]
      ,[Bank]
      ,[BankAddress]
      ,[Account]
      ,[SwiftCode]
      ,[Currency]
      ,Contact= npayee.[Name] 
      ,[Tel]
      ,[Mobile]
      ,[Email]
      ,npayee.[Status]
      ,client.EnterCode
      ,clientGrade=client.Grade
      ,Place = ep2.Place
  FROM [dbo].[nSuppliers] as nspplier
  join Enterprises as ep1 on nspplier.EnterpriseID = ep1.ID
  join [dbo].[nPayees] as npayee on nspplier.[ID]  = npayee.nSupplierID
  left join Enterprises as ep2 on nspplier.[RealID] = ep1.ID
  join WsClients as client  on ep1.[ID] =client.ID
--wsnSupplierPayeesTopView


