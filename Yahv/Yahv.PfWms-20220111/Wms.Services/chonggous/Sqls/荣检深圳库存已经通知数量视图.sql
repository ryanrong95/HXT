
select input.OrderID as vastOrderID, noticed.StorageID ,noticed.Quantity ,storage.Total from 
(
select notice.StorageID , Quantity =SUM(notice.Quantity) 
from Notices as notice with (nolock )
WHERE notice.WareHouseID LIKE 'SZ%'
 group by StorageID  --having  SUM(notice.Quantity)
 ) as noticed
 join Storages as  Storage on noticed.StorageID  =  Storage.id 
 join Inputs  as input on Storage.InputID = input.ID
 where storage.Total >  noticed.Quantity
--OPTION (MAXDOP 1);


select input.OrderID as vastOrderID, noticed.StorageID ,noticed.Quantity ,storage.Total from 
(
select notice.StorageID , Quantity =SUM(notice.Quantity) 
from Notices as notice with (nolock )
WHERE notice.WareHouseID LIKE 'SZ%'
 group by StorageID  --having  SUM(notice.Quantity)
 ) as noticed
 join Storages as  Storage on noticed.StorageID  =  Storage.id  and storage.Total >  noticed.Quantity
 join Inputs  as input on Storage.InputID = input.ID
 --where storage.Total >  noticed.Quantity
--OPTION (MAXDOP 1);