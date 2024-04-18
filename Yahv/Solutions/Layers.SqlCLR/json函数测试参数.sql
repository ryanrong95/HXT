/****

--∫Ø ˝≤‚ ‘
SELECT [dbo].[SqlToJson] (
 'SELECT *
    FROM [dbo].[Orders]
where [Name]=Name'
  ,'{
  "ID": @ID,
  "Name": @Name,
  "CreateDate": @CreateDate,
  "UpdateDate": @UpdateDate,
  "Deliverer2ID": @Deliverer2ID
}');
GO

--≤È—Ø≤‚ ‘
SELECT [ID]
      ,[Name]
      ,[CreateDate]
      ,[UpdateDate]
      ,[Deliverer2ID]
      ,[dbo].[SqlToJson] (
 'SELECT *
    FROM [dbo].[Orders]
where [id]='+''''+[ID]+''''
  ,'{
  "ID": @ID,
  "Name": @Name,
  "CreateDate": @CreateDate,
  "UpdateDate": @UpdateDate,
  "Deliverer2ID": @Deliverer2ID
}')

  FROM [BvTester].[dbo].[Orders]
GO

****/






