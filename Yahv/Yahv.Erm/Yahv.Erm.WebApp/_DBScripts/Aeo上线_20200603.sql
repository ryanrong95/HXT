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
GO
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

--Applications
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Applications]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Applications] (
    [ID]          VARCHAR (50)   NOT NULL,
    [VoteFlowID]  VARCHAR (50)   NOT NULL,
    [Title]       NVARCHAR (50)  NOT NULL,
    [Context]     NVARCHAR (MAX) NOT NULL,
    [ApplicantID] VARCHAR (50)   NOT NULL,
    [CreatorID]   VARCHAR (50)   NOT NULL,
    [CreateDate]  DATETIME       NOT NULL,
    [Status]      INT            NOT NULL,
    CONSTRAINT [PK_Applications] PRIMARY KEY CLUSTERED ([ID] ASC));

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'实际申请人ID,', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Applications', @level2type = N'COLUMN', @level2name = N'ApplicantID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'申请的上下文', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Applications', @level2type = N'COLUMN', @level2name = N'Context';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Applications', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建人ID ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Applications', @level2type = N'COLUMN', @level2name = N'CreatorID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'pk生成', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Applications', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'状态： 废弃、草稿、驳回、审批中、已完成', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Applications', @level2type = N'COLUMN', @level2name = N'Status';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'申请的标题', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Applications', @level2type = N'COLUMN', @level2name = N'Title';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'审批流ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Applications', @level2type = N'COLUMN', @level2name = N'VoteFlowID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'申请表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Applications';
end


--Calendars
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Calendars]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Calendars] (
    [ID]    DATE NOT NULL,
    [Year]  INT  NOT NULL,
    [Month] INT  NOT NULL,
    [Day]   INT  NOT NULL,
    [Week]  INT  CONSTRAINT [DF_Calendars_Week] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Calendars] PRIMARY KEY CLUSTERED ([ID] ASC)
);
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Calendars', @level2type = N'COLUMN', @level2name = N'Day';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日期（2020-04-01）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Calendars', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'月', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Calendars', @level2type = N'COLUMN', @level2name = N'Month';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'年', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Calendars', @level2type = N'COLUMN', @level2name = N'Year';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Calendars';
end

--LeaguesPrivate
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaguesPrivate]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[LeaguesPrivate] (
    [AdminMainID] VARCHAR (50) NOT NULL,
    [AdminSubID]  VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_LeaguesPrivate] PRIMARY KEY CLUSTERED ([AdminMainID] ASC, [AdminSubID] ASC)
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'我的ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LeaguesPrivate', @level2type = N'COLUMN', @level2name = N'AdminMainID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交接人ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LeaguesPrivate', @level2type = N'COLUMN', @level2name = N'AdminSubID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'个人组织机构', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LeaguesPrivate';
end


--Logs_ApplyVoteSteps
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Logs_ApplyVoteSteps]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Logs_ApplyVoteSteps] (
    [ID]            VARCHAR (50)   NOT NULL,
    [ApplicationID] VARCHAR (50)   NOT NULL,
    [VoteStepID]    VARCHAR (50)   NULL,
    [AdminID]       VARCHAR (50)   NOT NULL,
    [Status]        INT            NOT NULL,
    [Summary]       NVARCHAR (200) NULL,
    [CreateDate]    DATETIME       NOT NULL,
    CONSTRAINT [PK_Logs_] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Logs_ApplyVoteSteps_Applications] FOREIGN KEY ([ApplicationID]) REFERENCES [dbo].[Applications] ([ID])
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_ApplyVoteSteps', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'状态：（只有） Allow  Veto 赞同、否决', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_ApplyVoteSteps', @level2type = N'COLUMN', @level2name = N'Status';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'审批人提出的摘要', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_ApplyVoteSteps', @level2type = N'COLUMN', @level2name = N'Summary';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'所属审批流步骤', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_ApplyVoteSteps', @level2type = N'COLUMN', @level2name = N'VoteStepID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'审批日志', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_ApplyVoteSteps';
end

--Logs_Attend
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Logs_Attend]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Logs_Attend] (
    [ID]         VARCHAR (50) NOT NULL,
    [Date]       DATE         NOT NULL,
    [StaffID]    VARCHAR (50) NOT NULL,
    [CreateDate] DATETIME     NOT NULL,
    [IP]         VARCHAR (50) NULL,
    CONSTRAINT [PK_Logs_Attend] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Logs_Attend_Calendars] FOREIGN KEY ([Date]) REFERENCES [dbo].[Calendars] ([ID])
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'打卡时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_Attend', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_Attend', @level2type = N'COLUMN', @level2name = N'Date';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'pk生成', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_Attend', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'考勤机IP', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_Attend', @level2type = N'COLUMN', @level2name = N'IP';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'员工ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_Attend', @level2type = N'COLUMN', @level2name = N'StaffID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'考勤原始记录', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs_Attend';
end

--MapsAppStaff
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MapsAppStaff]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[MapsAppStaff] (
    [ApplicationID] VARCHAR (50) NOT NULL,
    [StaffID]       VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_MapsAppAdmin] PRIMARY KEY CLUSTERED ([ApplicationID] ASC, [StaffID] ASC)
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'申请ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MapsAppStaff', @level2type = N'COLUMN', @level2name = N'ApplicationID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'受众ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MapsAppStaff', @level2type = N'COLUMN', @level2name = N'StaffID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'受众表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MapsAppStaff';
end


--Pasts_Attend
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Pasts_Attend]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Pasts_Attend] (
    [ID]            VARCHAR (50) NOT NULL,
    [Date]          DATE         NOT NULL,
    [AmOrPm]        VARCHAR (2)  NOT NULL,
    [StaffID]       VARCHAR (50) NOT NULL,
    [SchedulingID]  VARCHAR (50) NOT NULL,
    [StartTime]     DATETIME     NULL,
    [EndTime]       DATETIME     NULL,
    [CreateDate]    DATETIME     NOT NULL,
    [ModifyDate]    DATETIME     NOT NULL,
    [InFact]        INT          NOT NULL,
    [IsLater]       BIT          NULL,
    [IsEarly]       BIT          NULL,
    [OnWorkRemedy]  BIT          NULL,
    [OffWorkRemedy] BIT          NULL,
    CONSTRAINT [PK_Pasts_Attend] PRIMARY KEY CLUSTERED ([ID] ASC)
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上下午枚举：Am Or  Pm', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pasts_Attend', @level2type = N'COLUMN', @level2name = N'AmOrPm';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pasts_Attend', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pasts_Attend', @level2type = N'COLUMN', @level2name = N'Date';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'结束下班时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pasts_Attend', @level2type = N'COLUMN', @level2name = N'EndTime';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'需要增加  Am Pm 的内容   ,例如： yyyyMMdd+Am', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pasts_Attend', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'实际情况  枚举化： 正常、旷工、事假、病假、带薪假、加班、公休日、法定节假日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pasts_Attend', @level2type = N'COLUMN', @level2name = N'InFact';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否早退', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pasts_Attend', @level2type = N'COLUMN', @level2name = N'IsEarly';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否迟到', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pasts_Attend', @level2type = N'COLUMN', @level2name = N'IsLater';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pasts_Attend', @level2type = N'COLUMN', @level2name = N'ModifyDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'下班补签', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pasts_Attend', @level2type = N'COLUMN', @level2name = N'OffWorkRemedy';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上班补签', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pasts_Attend', @level2type = N'COLUMN', @level2name = N'OnWorkRemedy';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Public 中 实际的 班别', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pasts_Attend', @level2type = N'COLUMN', @level2name = N'SchedulingID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'员工ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pasts_Attend', @level2type = N'COLUMN', @level2name = N'StaffID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'开始工作时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pasts_Attend', @level2type = N'COLUMN', @level2name = N'StartTime';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'考勤历史（统计）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pasts_Attend';
end



--PositionsAc
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PositionsAc]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[PositionsAc] (
    [ID]         VARCHAR (50)  NOT NULL,
    [Name]       NVARCHAR (50) NOT NULL,
    [RegionID]   VARCHAR (50)  NOT NULL,
    [Status]     INT           NOT NULL,
    [CreatorID]  VARCHAR (50)  NOT NULL,
    [ModifyID]   VARCHAR (50)  NULL,
    [CreateDate] DATETIME      NOT NULL,
    [ModifyDate] DATETIME      NOT NULL,
    CONSTRAINT [PK_PositionsAc] PRIMARY KEY CLUSTERED ([ID] ASC)
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PositionsAc', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建人ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PositionsAc', @level2type = N'COLUMN', @level2name = N'CreatorID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'MD5（Name）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PositionsAc', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PositionsAc', @level2type = N'COLUMN', @level2name = N'ModifyDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改人ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PositionsAc', @level2type = N'COLUMN', @level2name = N'ModifyID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'岗位名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PositionsAc', @level2type = N'COLUMN', @level2name = N'Name';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'区域ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PositionsAc', @level2type = N'COLUMN', @level2name = N'RegionID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'正常 停用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PositionsAc', @level2type = N'COLUMN', @level2name = N'Status';
end

--RegionsAc
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RegionsAc]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[RegionsAc] (
    [ID]         VARCHAR (50) NOT NULL,
    [FatherID]   VARCHAR (50) NULL,
    [Name]       VARCHAR (50) NOT NULL,
    [Status]     INT          NOT NULL,
    [CreatorID]  VARCHAR (50) NOT NULL,
    [ModifyID]   VARCHAR (50) NULL,
    [CreateDate] DATETIME     NOT NULL,
    [ModifyDate] DATETIME     NOT NULL,
    CONSTRAINT [PK_Ac_Regions] PRIMARY KEY CLUSTERED ([ID] ASC)
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RegionsAc', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建人ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RegionsAc', @level2type = N'COLUMN', @level2name = N'CreatorID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'父级ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RegionsAc', @level2type = N'COLUMN', @level2name = N'FatherID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RegionsAc', @level2type = N'COLUMN', @level2name = N'ModifyDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改人ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RegionsAc', @level2type = N'COLUMN', @level2name = N'ModifyID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RegionsAc', @level2type = N'COLUMN', @level2name = N'Name';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'做假删除', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RegionsAc', @level2type = N'COLUMN', @level2name = N'Status';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'区域表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RegionsAc';
end


--Schedules
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Schedules]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Schedules] (
    [ID]         VARCHAR (50) NOT NULL,
    [Date]       DATE         NOT NULL,
    [Type]       INT          NOT NULL,
    [CreatorID]  VARCHAR (50) NOT NULL,
    [ModifyID]   VARCHAR (50) NULL,
    [CreateDate] DATETIME     NOT NULL,
    [ModifyDate] DATETIME     NOT NULL,
    CONSTRAINT [PK_Schedules] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Schedules_Calendars] FOREIGN KEY ([Date]) REFERENCES [dbo].[Calendars] ([ID])
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedules', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建人ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedules', @level2type = N'COLUMN', @level2name = N'CreatorID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedules', @level2type = N'COLUMN', @level2name = N'Date';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'pk生成', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedules', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'枚举化： 公有、私有', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedules', @level2type = N'COLUMN', @level2name = N'Type';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日程安排', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedules';
end


--SchedulesPrivate
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SchedulesPrivate]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[SchedulesPrivate] (
    [ID]            VARCHAR (50) NOT NULL,
    [ApplicationID] VARCHAR (50) NOT NULL,
    [Type]          INT          CONSTRAINT [DF_SchedulesPrivate_Type] DEFAULT ((0)) NOT NULL,
    [AmOrPm]        VARCHAR (2)  NOT NULL,
    [StaffID]       VARCHAR (50) NOT NULL,
    [OnWorkRemedy]  BIT          NULL,
    [OffWorkRemedy] BIT          NULL,
    [CreateDate]    DATETIME     CONSTRAINT [DF_SchedulesPrivate_CreateDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_SchedulesPrivate] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SchedulesPrivate_Schedules] FOREIGN KEY ([ID]) REFERENCES [dbo].[Schedules] ([ID])
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上下午枚举：Am Or  Pm', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPrivate', @level2type = N'COLUMN', @level2name = N'AmOrPm';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'申请ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPrivate', @level2type = N'COLUMN', @level2name = N'ApplicationID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPrivate', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'pk生成', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPrivate', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'下班补签', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPrivate', @level2type = N'COLUMN', @level2name = N'OffWorkRemedy';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上班补签', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPrivate', @level2type = N'COLUMN', @level2name = N'OnWorkRemedy';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'员工ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPrivate', @level2type = N'COLUMN', @level2name = N'StaffID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'类型：
公务
公差
年假
事假
病假
调休
婚假
产检假
陪产假
产假
丧假
其它
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPrivate', @level2type = N'COLUMN', @level2name = N'Type';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'个人日程安排', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPrivate';
end


--SchedulesPublic
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SchedulesPublic]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[SchedulesPublic] (
    [ID]             VARCHAR (50)    NOT NULL,
    [Method]         INT             NOT NULL,
    [Name]           NVARCHAR (50)   NOT NULL,
    [From]           INT             NOT NULL,
    [RegionID]       VARCHAR (50)    NOT NULL,
    [PostionID]      VARCHAR (50)    NULL,
    [SalaryMultiple] DECIMAL (18, 7) CONSTRAINT [DF_SchedulesPublic_Multiple] DEFAULT ((1)) NULL,
    [ShiftID]        VARCHAR (50)    NOT NULL,
    [SchedulingID]   VARCHAR (50)    NOT NULL,
    CONSTRAINT [PK_SchedulesPublic] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SchedulesPublic_Schedules] FOREIGN KEY ([ID]) REFERENCES [dbo].[Schedules] ([ID])
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Government\Company', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPublic', @level2type = N'COLUMN', @level2name = N'From';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'安排方式：工作日、公休日、法定假期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPublic', @level2type = N'COLUMN', @level2name = N'Method';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'0 工作日、1 公休日、2 元旦、3 春节、4 清明、5 劳动节、6 端午、7 国庆节、8 中秋', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPublic', @level2type = N'COLUMN', @level2name = N'Name';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'岗位：一般国家或是公司统一安排的就没有岗位限制', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPublic', @level2type = N'COLUMN', @level2name = N'PostionID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'区域ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPublic', @level2type = N'COLUMN', @level2name = N'RegionID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'薪酬倍数', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPublic', @level2type = N'COLUMN', @level2name = N'SalaryMultiple';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'实际属于的', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPublic', @level2type = N'COLUMN', @level2name = N'SchedulingID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'受众（班别的那些人）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPublic', @level2type = N'COLUMN', @level2name = N'ShiftID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'公共日程安排', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SchedulesPublic';
end


--Schedulings
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Schedulings]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Schedulings] (
    [ID]          VARCHAR (50)   NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [PostionID]   VARCHAR (50)   NULL,
    [AmStartTime] TIME (7)       NULL,
    [AmEndTime]   TIME (7)       NULL,
    [PmStartTime] TIME (7)       NOT NULL,
    [PmEndTime]   TIME (7)       NOT NULL,
    [DomainValue] INT            NOT NULL,
    [Summary]     NVARCHAR (500) NULL,
    [IsMain]      BIT            NOT NULL,
    CONSTRAINT [PK_Schedulings] PRIMARY KEY CLUSTERED ([ID] ASC)
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上午结束时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedulings', @level2type = N'COLUMN', @level2name = N'AmEndTime';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上班时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedulings', @level2type = N'COLUMN', @level2name = N'AmStartTime';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'判断迟到早退的阈值  以分钟为单位', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedulings', @level2type = N'COLUMN', @level2name = N'DomainValue';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ID Pick生成', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedulings', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'主班别，即可以分配给员工', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedulings', @level2type = N'COLUMN', @level2name = N'IsMain';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedulings', @level2type = N'COLUMN', @level2name = N'Name';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'下班时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedulings', @level2type = N'COLUMN', @level2name = N'PmEndTime';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'下午开始时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedulings', @level2type = N'COLUMN', @level2name = N'PmStartTime';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'岗位ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedulings', @level2type = N'COLUMN', @level2name = N'PostionID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备注', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedulings', @level2type = N'COLUMN', @level2name = N'Summary';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'排班', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Schedulings';
end


--Settings
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Settings]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[Settings] (
    [ID]         VARCHAR (50)    NOT NULL,
    [Type]       VARCHAR (50)    NOT NULL,
    [DataType]   VARCHAR (50)    NOT NULL,
    [Name]       NVARCHAR (150)  NOT NULL,
    [Value]      NVARCHAR (2000) NULL,
    [Summary]    NVARCHAR (500)  NOT NULL,
    [CreateDate] DATETIME        NOT NULL,
    [UpdateDate] DATETIME        NOT NULL,
    CONSTRAINT [PK_SystemSettings] PRIMARY KEY CLUSTERED ([ID] ASC)
);
end

--VoteFlows
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VoteFlows]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[VoteFlows] (
    [ID]         VARCHAR (50) NOT NULL,
    [Name]       VARCHAR (50) NOT NULL,
    [Type]       INT          NOT NULL,
    [CreatorID]  VARCHAR (50) NOT NULL,
    [ModifyID]   VARCHAR (50) NULL,
    [CreateDate] DATETIME     NOT NULL,
    [ModifyDate] DATETIME     NOT NULL,
    CONSTRAINT [PK_VoteFlows] PRIMARY KEY CLUSTERED ([ID] ASC)
);

/****** Object:  Table [dbo].[VoteFlows]    Script Date: 06/03/2020 17:10:00 ******/
INSERT [dbo].[VoteFlows] ([ID], [Name], [Type], [CreatorID], [ModifyID], [CreateDate], [ModifyDate]) VALUES (N'BQVF', N'补签审批', 5, N'NPC', NULL, CAST(0x0000ABB4012588FB AS DateTime), CAST(0x0000ABB4012588FB AS DateTime))
INSERT [dbo].[VoteFlows] ([ID], [Name], [Type], [CreatorID], [ModifyID], [CreateDate], [ModifyDate]) VALUES (N'JBVF', N'加班审批', 3, N'NPC', NULL, CAST(0x0000ABB4012588FB AS DateTime), CAST(0x0000ABB4012588FB AS DateTime))
INSERT [dbo].[VoteFlows] ([ID], [Name], [Type], [CreatorID], [ModifyID], [CreateDate], [ModifyDate]) VALUES (N'LZVF', N'离职审批', 2, N'NPC', NULL, CAST(0x0000ABB4012588FB AS DateTime), CAST(0x0000ABB4012588FB AS DateTime))
INSERT [dbo].[VoteFlows] ([ID], [Name], [Type], [CreatorID], [ModifyID], [CreateDate], [ModifyDate]) VALUES (N'QJAbove3VF', N'3天以上请假审批', 4, N'NPC', NULL, CAST(0x0000ABB4012588FB AS DateTime), CAST(0x0000ABB4012588FB AS DateTime))
INSERT [dbo].[VoteFlows] ([ID], [Name], [Type], [CreatorID], [ModifyID], [CreateDate], [ModifyDate]) VALUES (N'QJBelow3VF', N'3天以内请假审批', 4, N'NPC', NULL, CAST(0x0000ABB4012588FB AS DateTime), CAST(0x0000ABB4012588FB AS DateTime))
INSERT [dbo].[VoteFlows] ([ID], [Name], [Type], [CreatorID], [ModifyID], [CreateDate], [ModifyDate]) VALUES (N'RZVF', N'入职审批', 1, N'NPC', NULL, CAST(0x0000ABB4012588FB AS DateTime), CAST(0x0000ABB4012588FB AS DateTime))

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteFlows', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建人ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteFlows', @level2type = N'COLUMN', @level2name = N'CreatorID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Pick ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteFlows', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteFlows', @level2type = N'COLUMN', @level2name = N'ModifyDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改人', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteFlows', @level2type = N'COLUMN', @level2name = N'ModifyID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'审批名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteFlows', @level2type = N'COLUMN', @level2name = N'Name';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'属于某项的统计，例如：XX申请', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteFlows', @level2type = N'COLUMN', @level2name = N'Type';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'审批流程', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteFlows';
end

--VoteSteps
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VoteSteps]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[VoteSteps] (
    [ID]         VARCHAR (50)   NOT NULL,
    [Name]       VARCHAR (50)   NOT NULL,
    [VoteFlowID] VARCHAR (50)   NOT NULL,
    [CreateDate] DATETIME       NOT NULL,
    [OrderIndex] INT            NOT NULL,
    [AdminID]    VARCHAR (50)   NULL,
    [PositionID] VARCHAR (50)   NULL,
    [Uri]        VARCHAR (2000) NULL,
    CONSTRAINT [PK_VoteSteps] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_VoteSteps_VoteFlows] FOREIGN KEY ([VoteFlowID]) REFERENCES [dbo].[VoteFlows] ([ID])
);

/****** Object:  Table [dbo].[VoteSteps]    Script Date: 06/03/2020 17:10:00 ******/
INSERT [dbo].[VoteSteps] ([ID], [Name], [VoteFlowID], [CreateDate], [OrderIndex], [AdminID], [PositionID], [Uri]) VALUES (N'BQVS1', N'行政部审批', N'BQVF', CAST(0x0000ABB4012588FC AS DateTime), 1, N'Admin00523', NULL, NULL)
INSERT [dbo].[VoteSteps] ([ID], [Name], [VoteFlowID], [CreateDate], [OrderIndex], [AdminID], [PositionID], [Uri]) VALUES (N'JBVS1', N'部门负责人审批', N'JBVF', CAST(0x0000ABB4012588FC AS DateTime), 1, N'', NULL, NULL)
INSERT [dbo].[VoteSteps] ([ID], [Name], [VoteFlowID], [CreateDate], [OrderIndex], [AdminID], [PositionID], [Uri]) VALUES (N'JBVS2', N'行政部审批', N'JBVF', CAST(0x0000ABB4012588FC AS DateTime), 2, N'Admin00523', NULL, NULL)
INSERT [dbo].[VoteSteps] ([ID], [Name], [VoteFlowID], [CreateDate], [OrderIndex], [AdminID], [PositionID], [Uri]) VALUES (N'LZVS1', N'部门负责人审批', N'LZVF', CAST(0x0000ABB4012588FC AS DateTime), 1, N'', NULL, N'Approval.aspx')
INSERT [dbo].[VoteSteps] ([ID], [Name], [VoteFlowID], [CreateDate], [OrderIndex], [AdminID], [PositionID], [Uri]) VALUES (N'LZVS2', N'总经理批准', N'LZVF', CAST(0x0000ABB4012588FC AS DateTime), 2, N'Admin00526', NULL, N'Approval.aspx')
INSERT [dbo].[VoteSteps] ([ID], [Name], [VoteFlowID], [CreateDate], [OrderIndex], [AdminID], [PositionID], [Uri]) VALUES (N'LZVS3', N'行政部审批', N'LZVF', CAST(0x0000ABB4012588FC AS DateTime), 3, N'Admin00523', NULL, N'Approval_HR.aspx')
INSERT [dbo].[VoteSteps] ([ID], [Name], [VoteFlowID], [CreateDate], [OrderIndex], [AdminID], [PositionID], [Uri]) VALUES (N'QJAbove3VS1', N'部门负责人审批', N'QJAbove3VF', CAST(0x0000ABB4012588FC AS DateTime), 1, N'', NULL, NULL)
INSERT [dbo].[VoteSteps] ([ID], [Name], [VoteFlowID], [CreateDate], [OrderIndex], [AdminID], [PositionID], [Uri]) VALUES (N'QJAbove3VS2', N'总经理核准', N'QJAbove3VF', CAST(0x0000ABB4012588FC AS DateTime), 2, N'Admin00526', NULL, NULL)
INSERT [dbo].[VoteSteps] ([ID], [Name], [VoteFlowID], [CreateDate], [OrderIndex], [AdminID], [PositionID], [Uri]) VALUES (N'QJAbove3VS3', N'行政部审批', N'QJAbove3VF', CAST(0x0000ABB4012588FC AS DateTime), 3, N'Admin00523', NULL, NULL)
INSERT [dbo].[VoteSteps] ([ID], [Name], [VoteFlowID], [CreateDate], [OrderIndex], [AdminID], [PositionID], [Uri]) VALUES (N'QJBelow3VS1', N'部门负责人审批', N'QJBelow3VF', CAST(0x0000ABB4012588FC AS DateTime), 1, N'', NULL, NULL)
INSERT [dbo].[VoteSteps] ([ID], [Name], [VoteFlowID], [CreateDate], [OrderIndex], [AdminID], [PositionID], [Uri]) VALUES (N'QJBelow3VS2', N'行政部审批', N'QJBelow3VF', CAST(0x0000ABB4012588FC AS DateTime), 2, N'Admin00523', NULL, NULL)
INSERT [dbo].[VoteSteps] ([ID], [Name], [VoteFlowID], [CreateDate], [OrderIndex], [AdminID], [PositionID], [Uri]) VALUES (N'RZVS1', N'部门负责人审批', N'RZVF', CAST(0x0000ABB4012588FC AS DateTime), 1, N'', NULL, NULL)
INSERT [dbo].[VoteSteps] ([ID], [Name], [VoteFlowID], [CreateDate], [OrderIndex], [AdminID], [PositionID], [Uri]) VALUES (N'RZVS2', N'总经理核准', N'RZVF', CAST(0x0000ABB4012588FC AS DateTime), 2, N'Admin00526', NULL, NULL)
INSERT [dbo].[VoteSteps] ([ID], [Name], [VoteFlowID], [CreateDate], [OrderIndex], [AdminID], [PositionID], [Uri]) VALUES (N'RZVS3', N'行政部审批', N'RZVF', CAST(0x0000ABB4012588FC AS DateTime), 3, N'Admin00523', NULL, NULL)


EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'固定审批人', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteSteps', @level2type = N'COLUMN', @level2name = N'AdminID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteSteps', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Pick', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteSteps', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'审批步骤名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteSteps', @level2type = N'COLUMN', @level2name = N'Name';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'排序', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteSteps', @level2type = N'COLUMN', @level2name = N'OrderIndex';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'（岗位、职务）审批 （暂时不做）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteSteps', @level2type = N'COLUMN', @level2name = N'PositionID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用审批的页面（可以忽略）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteSteps', @level2type = N'COLUMN', @level2name = N'Uri';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'所属审批流', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteSteps', @level2type = N'COLUMN', @level2name = N'VoteFlowID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'审批步骤', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VoteSteps';
end

--Tgr_VoteFlow1s
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tgr_VoteFlow1s]') AND type IN (N'tr'))
BEGIN
EXEC('
CREATE  TRIGGER [dbo].[Tgr_VoteFlow1s]
ON [dbo].[VoteFlows]
AFTER INSERT, UPDATE, DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here

	print 1;

END

print 2;
');
end

--Tgr_VoteFlows
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tgr_VoteFlows]') AND type IN (N'tr'))
BEGIN
DROP TRIGGER [dbo].[Tgr_VoteFlows]
end
go
CREATE TRIGGER [dbo].[Tgr_VoteFlows]
ON [dbo].[VoteFlows]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	-- 变量定义
	DECLARE @object int,
			@id VARCHAR(50),
			@value NVARCHAR(2000),
			@serviceUrl NVARCHAR(2000),
			@parameters NVARCHAR(500),
			@response NVARCHAR(4000);

	-- 统计变化的ID
	DECLARE @idTable Table([ID] VARCHAR(50));

	INSERT INTO @idTable SELECT id FROM inserted;

	SET @parameters = '';
	WHILE EXISTS(SELECT 1 FROM @idTable)
	BEGIN
		SET @id = (SELECT TOP 1 [ID] FROM @idTable);
		IF((SELECT COUNT(*) FROM @idTable) > 1)
		BEGIN
			SET @parameters += @id + ',';
		END
		ELSE
		BEGIN
			SET @parameters += @id;
		END
		DELETE FROM @idTable WHERE [ID]=@id;
	END

	SET @parameters = 'id=' + @parameters;
	print @parameters;

	-- 创建dbo.Settings的临时表
	DECLARE @settingsTemp TABLE
	(
		[ID] VARCHAR(50),
		[Value] NVARCHAR(2000)
	);

	-- 将Settings表的数据写入临时表
	INSERT INTO @settingsTemp([ID],[Value]) SELECT [ID],[Value] FROM [dbo].[Settings];

	-- 遍历临时表，调用相应接口
	WHILE EXISTS(SELECT 1 FROM @settingsTemp)
	BEGIN
		-- 取出一个配置数据
		SELECT TOP 1 @id=[ID], @value=[Value] FROM @settingsTemp;
		-- 接口地址
		SET @serviceUrl = @value;

		-- form参数
		--SET @parameters = 'id=' + (SELECT id FROM inserted);
		-- 接口调用
		EXEC sp_OACreate 'MSXML2.XMLHTTP', @object OUT; --创建OLE组件对象
		EXEC sp_OAMethod @object, 'open', NULL, 'post',@serviceUrl,'false'; --打开链接，注意是get还是post	
		EXEC sp_OAMethod @object,'setRequestHeader',NULL,'Content-Type','application/x-www-form-urlencoded;charset=UTF-8';
		EXEC sp_OAMethod @object, 'send', NULL, @parameters;
		EXEC sp_OADestroy @object;
		-- 删除已经使用的配置数据
		DELETE FROM @settingsTemp WHERE [ID]=@id;
	END
END
GO


--ApplyVoteSteps
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ApplyVoteSteps]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[ApplyVoteSteps] (
    [ID]            VARCHAR (50)   NOT NULL,
    [ApplicationID] VARCHAR (50)   NOT NULL,
    [VoteStepID]    VARCHAR (50)   NULL,
    [IsCurrent]     BIT            NOT NULL,
    [AdminID]       VARCHAR (50)   NOT NULL,
    [Status]        INT            NOT NULL,
    [Summary]       NVARCHAR (200) NULL,
    [CreateDate]    DATETIME       NOT NULL,
    [ModifyDate]    DATETIME       NOT NULL,
    CONSTRAINT [PK_AppVoteSteps] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ApplyVoteSteps_Applications] FOREIGN KEY ([ApplicationID]) REFERENCES [dbo].[Applications] ([ID]),
    CONSTRAINT [FK_ApplyVoteSteps_VoteSteps] FOREIGN KEY ([VoteStepID]) REFERENCES [dbo].[VoteSteps] ([ID])
);

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'审批人ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ApplyVoteSteps', @level2type = N'COLUMN', @level2name = N'AdminID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'申请ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ApplyVoteSteps', @level2type = N'COLUMN', @level2name = N'ApplicationID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ApplyVoteSteps', @level2type = N'COLUMN', @level2name = N'CreateDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'唯一值', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ApplyVoteSteps', @level2type = N'COLUMN', @level2name = N'ID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否是当前步骤', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ApplyVoteSteps', @level2type = N'COLUMN', @level2name = N'IsCurrent';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ApplyVoteSteps', @level2type = N'COLUMN', @level2name = N'ModifyDate';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'状态：（只有） Allow  Veto 赞同、否决', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ApplyVoteSteps', @level2type = N'COLUMN', @level2name = N'Status';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'审批人意见', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ApplyVoteSteps', @level2type = N'COLUMN', @level2name = N'Summary';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'所属审批流步骤ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ApplyVoteSteps', @level2type = N'COLUMN', @level2name = N'VoteStepID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'申请的审批步骤', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ApplyVoteSteps';
end

--AdminsWmsView
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AdminsWmsView]') AND type IN (N'V'))
BEGIN
EXEC('CREATE VIEW [dbo].[AdminsWmsView]
AS
SELECT *
FROM  dbo.AdminsBussinessTopView AS ab  
WHERE     (ab.bussiness = ''库房管理'')
')
END

--ApprovalsStatisticsView
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.ApprovalsStatisticsView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.ApprovalsStatisticsView
AS
SELECT     app.ID AS ApplicationID, app.VoteFlowID, app.Title, app.Context, app.ApplicantID, app.CreatorID, app.CreateDate, app.Status, vf.Type, vf.Name AS VoteFlowName, vf.ID AS VoteFlowsID, 
                      vs.ID AS ApplyVoteStepsID, vs.VoteStepID, vs.AdminID AS ApproveID, vs.Name AS VoteStepName, vs.PositionID, vs.Uri
FROM         dbo.Applications AS app LEFT OUTER JOIN
                      dbo.VoteFlows AS vf ON app.VoteFlowID = vf.ID LEFT OUTER JOIN
                          (SELECT     avss.ID, avss.ApplicationID, avss.VoteStepID, vss.Name, vss.VoteFlowID, avss.AdminID, vss.PositionID, vss.Uri
                            FROM          dbo.ApplyVoteSteps AS avss LEFT OUTER JOIN
                                                   dbo.VoteSteps AS vss ON avss.VoteStepID = vss.ID
                            WHERE      (avss.IsCurrent = 1)) AS vs ON app.ID = vs.ApplicationID
WHERE     (app.Status = 2)
')
END

--LogsAttendTopView
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.LogsAttendTopView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.LogsAttendTopView
AS
SELECT     TOP (100) PERCENT la.Date, la.StaffID, s.Name, CONVERT(VARCHAR(100), MIN(la.CreateDate), 24) AS Am, CONVERT(VARCHAR(100), MAX(la.CreateDate), 24) AS Pm
FROM         dbo.Logs_Attend AS la INNER JOIN
                      dbo.Staffs AS s ON la.StaffID = s.ID
GROUP BY la.Date, la.StaffID, s.Name
')
END

--MapsWareHouseView
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.MapsWareHouseView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.MapsWareHouseView
AS
SELECT     AdminID, WareHouseID
FROM         dbo.MapsWareHouse
')
END


--PastsAttendTopView
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.PastsAttendTopView') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW dbo.PastsAttendTopView
AS
SELECT     TOP (100) PERCENT pa.Date, pa.ID, pa.AmOrPm, s.Name, pa.StaffID, pa.SchedulingID, pa.StartTime, pa.EndTime, pa.InFact, pa.IsLater, pa.IsEarly, pa.OnWorkRemedy, pa.OffWorkRemedy
FROM         dbo.Pasts_Attend AS pa INNER JOIN
                      dbo.Staffs AS s ON pa.StaffID = s.ID
')
END

--SubsWarehousesTopView
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubsWarehousesTopView]') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW [dbo].[SubsWarehousesTopView]
AS
SELECT     * 
FROM         PvbCrm.dbo.SubsWarehousesTopView AS SubsWarehousesTopView_1
')
END

--WarehousePlatesTopView
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WarehousePlatesTopView]') AND type IN (N'V'))
BEGIN
EXEC('
CREATE VIEW [dbo].[WarehousePlatesTopView]
AS
SELECT     * 
FROM         PvbCrm.dbo.WarehousePlatesTopView
')
END

go
--将离职改为注销
UPDATE PvbErm.dbo.Staffs SET Status=500 WHERE Status=400 or Status=300