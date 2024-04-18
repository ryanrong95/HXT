 


SELECT  fd.*
  FROM [PvCenter].[dbo].[FilesDescription] as fd 
  join [PvCenter].[dbo].[FileProperties] as fp1 on fd .ID = fp1.FileID and fp1.Name = 'WsOrderID'  
  join [PvCenter].[dbo].[FileProperties] as fp2 on fd .ID = fp2.FileID and fp2.Name = 'WaybillID'  
  where fp1.Value ='Order001'