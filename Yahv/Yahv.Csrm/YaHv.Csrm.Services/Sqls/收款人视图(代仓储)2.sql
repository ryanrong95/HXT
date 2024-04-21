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
      ,pyes.Contact 
      ,[Tel]
      ,[Mobile]
      ,[Email]
      ,pyes.[Status]
      ,client.EnterCode
      ,clientGrade=client.Grade
      ,Place = CASE 
         WHEN ep2.ID is null  THEN ep1.Place
         ELSE ep2.Place
      END
  FROM [dbo].[Payees] as  pyes  
  join Enterprises as ep1 on pyes.EnterpriseID = ep1.ID
  left join Enterprises as ep2 on pyes.[RealID] = ep1.ID
  join WsClients as client  on ep1.[ID] =client.ID
  
