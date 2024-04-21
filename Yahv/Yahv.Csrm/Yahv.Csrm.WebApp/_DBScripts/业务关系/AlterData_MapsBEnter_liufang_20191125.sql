use PvbCrm;


--ID由“TradingContact_subID+CtreatorID”订正为“TradingContact_subID+CtreatorID”
update  MapsBEnter set ID=Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingContact_'+SubID+CtreatorID))),3,32)) 
where ID=Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingContact'+SubID+CtreatorID))),3,32)) 


--ID由“TradingConsignee+subID+CtreatorID”订正为"TradingConsignee_+subID+CtreatorID"
update  MapsBEnter set ID=Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingConsignee_'+SubID+CtreatorID))),3,32))
 where  ID=Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingConsignee'+SubID+CtreatorID))),3,32)) ;


--ID由“TradingInvoice_+subID”订正为"TradingInvoice_+subID+CtreatorID"
update MapsBEnter set ID=Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',convert(varchar(100),'TradingInvoice_'+ SubID+CtreatorID ))),3,32)) 
where ID=Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',convert(varchar(100),'TradingInvoice'+ SubID))),3,32)) 

--ID由“TradingBeneficiary_+subID+CtreatorID”订正为"TradingBeneficiary_+subID+CtreatorID"		
update MapsBEnter set ID=Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingBeneficiary_'+SubID+CtreatorID))),3,32)) 
where ID=	Upper(substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',CONVERT(varchar(100),'TradingBeneficiary'+SubID+CtreatorID))),3,32))	

