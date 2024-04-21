SELECT     rb.OrderID, rb.WaybillID, 
rb.ID AS ReceivableID, rb.Payer, rb.Payee, 
rb.Business, rb.Catalog, rb.Subject, 
rb.Currency AS OriginCurrency, 
rb.Price AS OriginPrice, 
rb.SettlementCurrency AS Currency, 
                      rb.SettlementPrice AS LeftPrice, 
                      rb.CreateDate AS LeftDate, 
                      rd1.RightPrice, 
                      rd1.RightDate, 
                      ReducePrice =rd2.RightPrice,
                      rb.ChangeDate, rb.Summay, 
                      rb.AdminID, 
                      rb.Status, 
                      rb.TinyID, 
                      rb.OriginalDate, 
                      rb.OriginalIndex, rb.ChangeIndex, 
                      rb.ItemID, 
                      rb.ApplicationID
FROM  dbo.Receivables AS rb LEFT OUTER JOIN
                          
(SELECT ReceivableID, SUM(Price) AS RightPrice, MAX(CreateDate) AS RightDate
FROM dbo.Receiveds
where AccountType != 50
GROUP BY ReceivableID) AS rd1 ON rd1.ReceivableID = rb.ID
  LEFT OUTER JOIN      
(
SELECT ReceivableID, SUM(Price) AS RightPrice
FROM dbo.Receiveds
where AccountType = 50
GROUP BY ReceivableID
)AS rd2 ON rd2.ReceivableID = rb.ID