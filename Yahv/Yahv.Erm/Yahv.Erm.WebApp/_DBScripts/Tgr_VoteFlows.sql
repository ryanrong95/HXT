-- 用途：数据库修改审批流配置时，通知程序的WebService/WebApi重新初始化

-- 1.启用Ole Automation Procesures
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;

EXEC sp_configure 'Ole Automation Procedures', 1;
RECONFIGURE;

EXEC sp_configure 'Ole Automation Procedures';

USE [PvbErm]
GO

-- 2.创建触发器
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

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


