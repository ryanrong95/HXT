use PvbCrm;


--ID�ɡ�TradingContact_subID+CtreatorID������Ϊ��TradingContact_subID+CtreatorID��
update  MapsBEnter set ID=Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingContact_'+SubID+CtreatorID))),3,32)) 
where ID=Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingContact'+SubID+CtreatorID))),3,32)) 


--ID�ɡ�TradingConsignee+subID+CtreatorID������Ϊ"TradingConsignee_+subID+CtreatorID"
update  MapsBEnter set ID=Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingConsignee_'+SubID+CtreatorID))),3,32))
 where  ID=Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingConsignee'+SubID+CtreatorID))),3,32)) ;


--ID�ɡ�TradingInvoice_+subID������Ϊ"TradingInvoice_+subID+CtreatorID"
update MapsBEnter set ID=Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',convert(varchar(100),'TradingInvoice_'+ SubID+CtreatorID ))),3,32)) 
where ID=Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',convert(varchar(100),'TradingInvoice'+ SubID))),3,32)) 

--ID�ɡ�TradingBeneficiary_+subID+CtreatorID������Ϊ"TradingBeneficiary_+subID+CtreatorID"		
update MapsBEnter set ID=Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingBeneficiary_'+SubID+CtreatorID))),3,32)) 
where ID=	Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingBeneficiary'+SubID+CtreatorID))),3,32))	

