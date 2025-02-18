USE [ScCustoms]
GO
/****** Object:  StoredProcedure [dbo].[AutoConfirmReceipt]    Script Date: 2019/10/15 11:13:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AutoConfirmReceipt]
    @WAITTIMESEC_INPUT INT
AS
    BEGIN

        DECLARE @WaitTimeSec INT;
        DECLARE @AminID VARCHAR(50) = 'XDTAdmin';
        DECLARE @ExitNoticeID VARCHAR(50);
        DECLARE @OrderID VARCHAR(50);
        DECLARE @UnCompletedExitNotitceInOneOrderCount INT;
        DECLARE @CompletedItemQuantity INT;
        DECLARE @AllItemQuantity INT;

        DECLARE @OrderLogsPrimaryKey VARCHAR(50);
        DECLARE @OrderTracesPrimaryKey VARCHAR(50);
 

        SET @WaitTimeSec = @WAITTIMESEC_INPUT;


        SELECT  @ExitNoticeID = MIN(ID)
        FROM    dbo.ExitNotices
        WHERE   WarehouseType = 200
                AND ExitType = 4
                AND ExitNoticeStatus = 4
                AND Status = 200
                AND DATEDIFF(ss, UpdateDate, GETDATE()) > @WaitTimeSec;
                --AND OrderID = 'ICG0120190829005';
  -- 测试订单 -- 测试订单 -- 测试订单 -- 测试订单 -- 测试订单 -- 测试订单 -- 测试订单 

        WHILE @ExitNoticeID IS NOT NULL
            BEGIN
		-- 将该 ExitNotice 数据状态置为 5 (已完成)
                UPDATE  dbo.ExitNotices
                SET     ExitNoticeStatus = 5
                WHERE   ID = @ExitNoticeID;

       

		-- 记录一条日志到 ExitNoticeLogs 表
                INSERT  INTO dbo.ExitNoticeLogs
                        ( ID ,
                          ExitNoticeID ,
                          AdminID ,
                          OperType ,
                          CreateDate ,
                          Summary
		                )
                VALUES  ( REPLACE(NEWID(), '-', '') , -- ID - varchar(50)
                          @ExitNoticeID , -- ExitNoticeID - varchar(50)
                          @AminID , -- AdminID - varchar(50)
                          4 , -- OperType - int  (int)Needs.Wl.Models.Enums.ExitOperType.Completed,
                          GETDATE() , -- CreateDate - datetime
                          N'库房快递已发出超过7天，出库通知状态已置为【送货完成】'  -- Summary - nvarchar(500)
		                );

        

		-- 该出库通知对应的订单中，如果所有出库通知都为已完成，则将订单的状态置为 7 (已完成)
                SET @OrderID = ( SELECT OrderID
                                 FROM   dbo.ExitNotices
                                 WHERE  ID = @ExitNoticeID
                               );

                SET @UnCompletedExitNotitceInOneOrderCount = ( SELECT
                                                              COUNT(1)
                                                              FROM
                                                              dbo.ExitNotices
                                                              WHERE
                                                              WarehouseType = 200
                                                              AND ExitNoticeStatus <> 5
                                                              AND Status = 200
                                                              AND OrderID = @OrderID
                                                             );
				
                SET @CompletedItemQuantity = ( SELECT   SUM(ExitNoticeItems.Quantity)
                                               FROM     dbo.ExitNoticeItems
                                               WHERE    ExitNoticeItems.ExitNoticeID IN (
                                                        SELECT
                                                              ExitNotices.ID
                                                        FROM  dbo.ExitNotices
                                                        WHERE ExitNotices.Status = 200
                                                              AND ExitNotices.WarehouseType = 200
															  AND ExitNotices.ExitNoticeStatus = 5
                                                              AND ExitNotices.OrderID = @OrderID )
                                                        AND ExitNoticeItems.Status = 200
                                             );

                SET @AllItemQuantity = ( SELECT SUM(OrderItems.Quantity)
                                         FROM   dbo.OrderItems
                                         WHERE  OrderItems.OrderID = @OrderID
                                                AND OrderItems.Status = 200
                                       );

                IF @UnCompletedExitNotitceInOneOrderCount <= 0
                    AND @CompletedItemQuantity = @AllItemQuantity
                    BEGIN
                

                        UPDATE  dbo.Orders
                        SET     OrderStatus = 7 --(int)Needs.Wl.Models.Enums.OrderStatus.Completed,
                        WHERE   ID = @OrderID;
            

		-- OrderLogs 表插入日志
                        EXEC dbo.PrimaryKeysPick2 @NAME_INPUT = 'OrderLog', -- varchar(10)
                            @PRIMARYKEYTYPE_INPUT = 2, -- int
                            @LENGTH_INPUT = 10, -- int
                            @MODE_INPUT = 2, -- int
                            @PRIMARYKEY_OUTPUT = @OrderLogsPrimaryKey OUTPUT; -- varchar(50)

                        INSERT  INTO dbo.OrderLogs
                                ( ID ,
                                  OrderID ,
                                  OrderItemID ,
                                  AdminID ,
                                  UserID ,
                                  OrderStatus ,
                                  CreateDate ,
                                  Summary
		                        )
                        VALUES  ( @OrderLogsPrimaryKey , -- ID - varchar(50)
                                  @OrderID , -- OrderID - varchar(50)
                                  NULL , -- OrderItemID - varchar(50)
                                  @AminID , -- AdminID - varchar(50)
                                  NULL , -- UserID - varchar(50)
                                  7 , -- OrderStatus - int  (int)Needs.Wl.Models.Enums.OrderStatus.Completed,
                                  GETDATE() , -- CreateDate - datetime
                                  N'该订单已出库完成，已将订单状态置为【完成】'  -- Summary - nvarchar(500)
		                        );


		-- OrderTraces 表中插入订单轨迹
                        EXEC dbo.PrimaryKeysPick2 @NAME_INPUT = 'OrderTrace', -- varchar(10)
                            @PRIMARYKEYTYPE_INPUT = 2, -- int
                            @LENGTH_INPUT = 10, -- int
                            @MODE_INPUT = 2, -- int
                            @PRIMARYKEY_OUTPUT = @OrderTracesPrimaryKey OUTPUT; -- varchar(50)

                        INSERT  INTO dbo.OrderTraces
                                ( ID ,
                                  OrderID ,
                                  AdminID ,
                                  UserID ,
                                  Step ,
                                  CreateDate ,
                                  Summary
		                        )
                        VALUES  ( @OrderTracesPrimaryKey , -- ID - varchar(50)
                                  @OrderID , -- OrderID - varchar(50)
                                  @AminID , -- AdminID - varchar(50)
                                  NULL , -- UserID - varchar(50)
                                  9 , -- Step - int  (int)Needs.Wl.Models.Enums.OrderTraceStep.Completed,
                                  GETDATE() , -- CreateDate - datetime
                                  N'您的订单已完成，感谢使用一站式报关服务，期待与您的下次合作'  -- Summary - nvarchar(500)
		                        );

                    END; 

		-- 换下一个 ExitNoticeID
                SELECT  @ExitNoticeID = MIN(ID)
                FROM    dbo.ExitNotices
                WHERE   WarehouseType = 200
                        AND ExitType = 4
                        AND ExitNoticeStatus = 4
                        AND Status = 200
                        AND DATEDIFF(ss, UpdateDate, GETDATE()) > @WaitTimeSec
                        AND ID > @ExitNoticeID;
                        --AND OrderID = 'ICG0120190829005';  -- 测试订单 -- 测试订单 -- 测试订单 -- 测试订单 -- 测试订单 -- 测试订单 -- 测试订单
            END;
    END;
    
