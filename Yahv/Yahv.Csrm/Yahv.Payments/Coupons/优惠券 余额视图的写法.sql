

select 
Conduct,
SUM (fc.input) ,SUM(fc.ouput ),SUM (fc.input) - SUM(fc.ouput ) as balance
FlowCoupons as fc join Coupons as cup 
group by Conduct,Catalog,Subject --������
--�ϰ벿��������topview

--���������Ƕ�topview ��linq��ѯ
where Conduct 

