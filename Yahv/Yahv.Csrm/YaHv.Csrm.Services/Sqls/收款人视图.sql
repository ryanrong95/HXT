SELECT pyes.[ID]
      ,[EnterpriseID]= ep1.[ID]
      ,[EnterpriseName]= ep1.Name
      ,[RealEnterpriseID] = ep2.ID
      ,[RealEnterpriseName] = ep2.Name
      ,[Methord]
      ,[Bank]
      ,[BankAddress]
      ,[Account]
      ,[SwiftCode]
      ,[Currency]
      ,[Tel]
      ,[Mobile]
      ,[Email]
      ,[UpdateDate]
      ,  pyes.[Status]
  FROM [dbo].[Payees] as  pyes  
  join Enterprises as ep1 on pyes.EnterpriseID = ep1.ID
  left join Enterprises as ep2 on pyes.[RealID] = ep1.ID

--wsPayeesTopView

SELECT nspplier.[ID]
      ,[OwnID] =  ep1.[ID]
      ,[OwnName]= ep1.Name
      ,[RealEnterpriseID] = ep2.ID
      ,[RealEnterpriseName] = ep2.Name
      ,[FromID]
      ,[Conduct]
      ,[ChineseName]
      ,[EnglishName]
      ,[Grade]
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
  FROM [dbo].[nSuppliers] as nspplier
  join Enterprises as ep1 on nspplier.EnterpriseID = ep1.ID
  join [dbo].[nPayees] as npayee on nspplier.[ID]  = npayee.nSupplierID
  left join Enterprises as ep2 on nspplier.[RealID] = ep1.ID

--wsnSupplierPayeesTopView

