--PayBills 添加列 EnterpriseID
IF NOT EXISTS ( SELECT  *
                FROM    syscolumns
                WHERE   id = OBJECT_ID('PayBills')
                        AND name = 'EnterpriseID' )
    BEGIN
        ALTER TABLE	dbo.PayBills ADD  EnterpriseID VARCHAR(50) NULL;
        
        EXECUTE sp_addextendedproperty N'MS_Description', '所属公司（合同）', N'user', N'dbo', N'TABLE', N'PayBills', N'column', N'EnterpriseID'
    END;