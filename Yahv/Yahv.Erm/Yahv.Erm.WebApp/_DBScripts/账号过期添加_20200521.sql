USE [PvbErm]
GO

/****** Object:  Table [dbo].[Pasts_AdminPassword]    Script Date: 05/21/2020 09:33:49 ******/
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Pasts_AdminPassword]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Pasts_AdminPassword](
	[ID] [VARCHAR](50) NOT NULL,
	[AdminID] [VARCHAR](50) NOT NULL,
	[Password] [VARCHAR](50) NOT NULL,
	[CreateDate] [DATETIME] NOT NULL,
 CONSTRAINT [PK_Pasts_AdminPassword] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'pk 生成' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pasts_AdminPassword', @level2type=N'COLUMN',@level2name=N'ID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pasts_AdminPassword', @level2type=N'COLUMN',@level2name=N'AdminID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pasts_AdminPassword', @level2type=N'COLUMN',@level2name=N'Password'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密码修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pasts_AdminPassword', @level2type=N'COLUMN',@level2name=N'CreateDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密码修改历史表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Pasts_AdminPassword'
end

--Admins 添加列 PwdModifyDate
IF NOT EXISTS ( SELECT  *
                FROM    syscolumns
                WHERE   id = OBJECT_ID('Admins')
                        AND name = 'PwdModifyDate' )
    BEGIN
        ALTER TABLE	dbo.Admins ADD  PwdModifyDate DATETIME NULL;
        
        EXEC sys.sp_addextendedproperty @name = N'MS_Description',
            @value = N'密码修改时间', @level0type = N'SCHEMA', @level0name = N'dbo',
            @level1type = N'TABLE', @level1name = N'Admins',
            @level2type = N'COLUMN', @level2name = N'PwdModifyDate';
    END;