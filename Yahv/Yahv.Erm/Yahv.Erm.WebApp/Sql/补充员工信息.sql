USE PvbErm

 DECLARE @pKey VARCHAR(50); 
 
 --Admin00781	候新花	3993
 EXEC [dbo].[P_CreatePKey] @NAME = N'Staff', @LENGTH = 5, @VAL = @pKey OUTPUT;

 INSERT INTO dbo.Staffs(ID ,Name ,Code ,SelCode ,Gender ,DyjCompanyCode ,DyjDepartmentCode ,DyjCode ,WorkCity ,LeagueID ,PostionID ,AssessmentMethod ,AssessmentTime ,AdminID ,CreateDate ,UpdateDate ,Status) 
 VALUES ( @pKey ,'候新花' ,REPLACE(@pKey, 'Staff', '') ,0 ,0 ,'0' ,'0' ,'3993' ,'E79C74F2139A20DC3B1627D48C9F964F' ,NULL ,'A1DCD2F6A9ED3B2447FE651E41A1FD51' ,NULL ,NULL ,'SA01' ,GETDATE() ,GETDATE() ,300);
         
 INSERT INTO dbo.Personals( ID ,IDCard)
 VALUES(@pKey,'-' + REPLACE(@pKey, 'Staff', ''));

 UPDATE dbo.Admins SET StaffID = @pKey WHERE ID = 'Admin00781';
 
  --Admin00782	何秉军	3867
 EXEC [dbo].[P_CreatePKey] @NAME = N'Staff', @LENGTH = 5, @VAL = @pKey OUTPUT;

 INSERT INTO dbo.Staffs(ID ,Name ,Code ,SelCode ,Gender ,DyjCompanyCode ,DyjDepartmentCode ,DyjCode ,WorkCity ,LeagueID ,PostionID ,AssessmentMethod ,AssessmentTime ,AdminID ,CreateDate ,UpdateDate ,Status) 
 VALUES ( @pKey ,'何秉军' ,REPLACE(@pKey, 'Staff', '') ,0 ,0 ,'0' ,'0' ,'3867' ,'E79C74F2139A20DC3B1627D48C9F964F' ,NULL ,'A1DCD2F6A9ED3B2447FE651E41A1FD51' ,NULL ,NULL ,'SA01' ,GETDATE() ,GETDATE() ,300);
         
 INSERT INTO dbo.Personals( ID ,IDCard)
 VALUES(@pKey,'-' + REPLACE(@pKey, 'Staff', ''));

 UPDATE dbo.Admins SET StaffID = @pKey WHERE ID = 'Admin00782';
 