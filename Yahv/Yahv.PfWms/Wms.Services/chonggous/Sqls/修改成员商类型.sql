UPDATE [PvbCrm].[dbo].[Carriers]
   SET [Type] = 2
 WHERE  id in  (select ID from CarriersTopView where 
 Name in ('DHL','ACS','EMS','FedEx','RPX','SF','TNT','UPS','�°���','��Խ����'))