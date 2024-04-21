

select 
Conduct,
SUM (fc.input) ,SUM(fc.ouput ),SUM (fc.input) - SUM(fc.ouput ) as balance
FlowCoupons as fc join Coupons as cup 
group by Conduct,Catalog,Subject --，币种
--上半部分是我们topview

--以下是我们对topview 的linq查询
where Conduct 

