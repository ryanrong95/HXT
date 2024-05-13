SELECT [Name]
      ,[Code]
  FROM [PvCenter].[dbo].[CarriersTopView]order by [Name]
      ,[Code]
      

SELECT id, [Name]
      ,[Code]
  FROM ScCustoms.[dbo].[Carriers]order by [Name]
      ,[Code]

SELECT [Name] ,COUNT (1)
  FROM ScCustoms.[dbo].[Carriers] 
  group by [Name]
  order by [Name]
      
      
      
delete from [PvbCrm].[dbo].[Enterprises] where id  in(select ID from [PvbCrm].[dbo].[Carriers])
delete from [PvbCrm].[dbo].[Carriers]
  
  INSERT INTO [PvbCrm].[dbo].[Enterprises]
           ([ID]
           ,[Name]
           ,[AdminCode]
           ,[Status]
           ,[District]
           ,[Corporation]
           ,[RegAddress]
           ,[Uscc]
           ,[Place])
   SELECT [ID]  =  upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',[Name])),3,32)) 
		,[Name]
		,[AdminCode] = ''
		,[Status] =200
		,[District] =null 
           ,[Corporation]=null 
           ,[RegAddress]=null 
           ,[Uscc]=null 
      ,[Place] = CASE CarrierType
         WHEN 100 THEN 'HKG'
         WHEN 200 THEN 'HKG'
         WHEN 300 THEN 'CHN'
         WHEN 400 THEN 'CHN'
      END
  FROM [ScCustoms].[dbo].[Carriers] where ID ='60B4889115AE1A046D0C2BB0D0431728'

INSERT INTO [PvbCrm].[dbo].[Carriers]
           ([ID]
           ,[Code]
           ,[Icon]
           ,[CreateDate]
           ,[UpdateDate]
           ,[Summary]
           ,[Creator]
           ,[Status]
           ,[Type])
   SELECT [ID]  =  upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',[Name])),3,32)) 
		,[Code]
		,[Icon] = ''
		,[CreateDate]
      ,[UpdateDate]
      ,[Summary]=''
      ,[Creator]='Admin00057'
      ,[Status]= 200
      ,[Type] = CASE CarrierType
         WHEN 100 THEN 1
         WHEN 200 THEN 2
         WHEN 300 THEN 1
         WHEN 400 THEN 2
      END
  FROM [ScCustoms].[dbo].[Carriers] where ID ='60B4889115AE1A046D0C2BB0D0431728'
         

