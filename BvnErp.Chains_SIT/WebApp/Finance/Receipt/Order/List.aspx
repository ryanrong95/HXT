<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Finance.Receipt.Order.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单收款明细</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <style>
        #receipt-dialog tr {
            height: 33px;
        }
    </style>
    <script type="text/javascript">
        var NoticeData = eval('(<%=this.Model.NoticeData%>)');
        var clientID = getQueryString("ClientID");
        var financeReceiptId = getQueryString("ID");
        var SeqNo = getQueryString("SeqNo");

        var unReceiveListPageNumber = getQueryString("UnReceiveListPageNumber");
        var unReceiveListPageSize = getQueryString("UnReceiveListPageSize");

        var unReceiveListClientName = NoticeData.UnReceiveListClientName;
        var unReceiveListQuerenStatus = getQueryString("UnReceiveListQuerenStatus");
        var unReceiveListStartDate = getQueryString("UnReceiveListStartDate");
        var unReceiveListEndDate = getQueryString("UnReceiveListEndDate");

        var daiShouAll = (Number(NoticeData.Amount) - Number(NoticeData.ClearAmount)).toFixed(2);
        var yaoshouAll = 0;

        $(function () {

            //初始化报关订单列表
            $('#datagrid').myDatagrid({
                nowrap: false,
                border: false,
                fitColumns: true,
                scrollbarSize: 0,
                fit: true,
                singleSelect: false,
                checkOnSelect: false,
                selectOnCheck: false,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    if (data.total == 0) {
                        //$(this).datagrid('appendRow', { ID: '<div>未查询到该付款人的应收款订单信息</div>' }).datagrid('mergeCells', { index: 0, field: 'ID', colspan: 9 });
                        return;
                    }

                    if (data.rows.length > 0) {
                        //循环判断可勾选行
                        for (var i = 0; i < data.rows.length; i++) {
                            //可选已缴税状态的报关单
                            if (data.rows[i].ClientTypeInt == "<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode()%>") {
                                //$("input[type='checkbox']")[i + 1].disabled = false;
                                $('#datagrid').prev().find("input[type='checkbox']")[i + 1].disabled = false;
                            }
                            else {
                                //$("input[type='checkbox']")[i + 1].disabled = 'disabled';
                                $('#datagrid').prev().find("input[type='checkbox']")[i + 1].disabled = 'disabled';
                            }
                        }
                    }
                },
                onCheckAll: function (rows) {
                    for (var i = 0; i < rows.length; i++) {
                        //可选已缴税状态的报关单
                        if (rows[i].ClientTypeInt == "<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode()%>") {
                            $('#datagrid').myDatagrid('checkRow', i);
                        }
                        else {
                            $('#datagrid').myDatagrid('uncheckRow', i);
                        }
                    }
                    //$("input[type='checkbox']")[0].checked = true
                    $('#datagrid').prev().find("input[type='checkbox']")[0].checked = true;
                },
            });

            //初始化香港代仓储订单列表
            $('#datagrid-store').myDatagrid({
                actionName: 'DataStore',
                queryParams: {
                    ClienName: NoticeData.ClienName,
                },
                nowrap: false,
                border: false,
                fitColumns: true,
                scrollbarSize: 0,
                fit: true,
                singleSelect: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    if (data.total == 0) {
                        //$(this).datagrid('appendRow', { ID: '<div>未查询到该付款人的代仓储应收款订单信息</div>' }).datagrid('mergeCells', { index: 0, field: 'ID', colspan: 9 });
                        return;
                    }
                },
            });

            //初始化深圳代仓储账单列表
            $('#datagrid-store-sz').myDatagrid({
                actionName: 'SzDataStore',
                queryParams: {
                    ClienName: NoticeData.ClienName,
                },
                nowrap: false,
                border: false,
                fitColumns: true,
                scrollbarSize: 0,
                fit: true,
                singleSelect: true,
                toolbar: '#topBar-store-sz',
                rownumbers: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    if (data.total == 0) {
                        //$(this).datagrid('appendRow', { ID: '<div>未查询到该付款人的代仓储应收款订单信息</div>' }).datagrid('mergeCells', { index: 0, field: 'ID', colspan: 9 });
                        return;
                    }
                },
            });

            //初始化其它界面数据
            IntData();

            //返回 UnReceiveList.aspx 按钮事件
            $('#btnBackToUnReceiveList').bind('click', function () {
                window.location.href = location.pathname.replace(/List.aspx/ig, '../Notice/UnReceiveList.aspx')
                    + "?pageNumber=" + unReceiveListPageNumber + "&pageSize=" + unReceiveListPageSize
                    + "&ClientName=" + encodeURI(unReceiveListClientName)
                    + "&QuerenStatus=" + unReceiveListQuerenStatus
                    + "&StartDate=" + unReceiveListStartDate
                    + "&EndDate=" + unReceiveListEndDate
            });
           
            //收款弹框 一些设置
            $('#receipt-dialog').dialog({
                buttons: [{
                    text: '提交',
                    iconCls: 'icon-save',
                    handler: function () {
                        if (Number(yaoshouAll) > Number(daiShouAll)) {
                            $.messager.alert('提示', '实收总金额不能大于待收总金额！', 'info');
                            return;
                        }

                        var submitData = [];

                        var isReturn = 0;
                        var isPayExchangeNull = 0;
                        var infoMsg = "";

                        var checkedItems = $('#receipt-datagrid').datagrid('getChecked');
                        $.each(checkedItems, function (index, item) {
                            var receiveAmount = 0;
                            receiveAmount = $("input[orderReceiptId='" + item.OrderReceiptId + item.FeeSourceID + "']").val();
                            //var payExchangeID = $("input[orderPayExchangeId='" + item.OrderReceiptId + item.FeeSourceID + "']").val();

                            if (receiveAmount != null && receiveAmount.trim() != "" && receiveAmount.trim() != "0") {

                                //如果不是货款类别, 做一个判断, 限制大于应收款的提交 Begin
                                //if (item.FeeTypeValue != "1") {
                                    //2022-12-08 ryan 鲁亚慧：货款的实收也不能大于应收
                                    if (Number(receiveAmount) > Number(item.ReceivableAmount)) {
                                        infoMsg = infoMsg + item.FeeTypeShowName + "、";
                                        isReturn = 1;
                                    }
                                //}
                                //else {
                                //    //货款，验证收货款，必须填上付汇ID ryan 2020-11-08
                                //    if (payExchangeID.trim() == "") {
                                //        //$.messager.alert('提示', '收货款，请录入付汇申请ID！', 'info');
                                //        isPayExchangeNull = 1;
                                //    }
                                //}
                                //如果不是货款类别, 做一个判断, 限制大于应收款的提交 End

                                submitData.push({
                                    "FeeTypeValue": item.FeeTypeValue,
                                    "FeeSourceID": item.FeeSourceID,
                                    "ReceivableAmount": item.ReceivableAmount,
                                    "OrderReceiptId": item.OrderReceiptId,
                                    //"PayExchangeID": payExchangeID,
                                    "ReceiveAmount": receiveAmount,
                                });
                            }
                        });

                        if (1 == isReturn) {
                            infoMsg = infoMsg.slice(0, infoMsg.length - 1);
                            infoMsg = infoMsg + ' 实收金额不能大于应收金额！';
                            $.messager.alert('提示', infoMsg, 'info');
                            return;
                        }

                        if (1 == isPayExchangeNull) {
                            $.messager.alert('提示', '收货款，请录入付汇申请ID！', 'info');
                            return;
                        }

                        //提交
                        var url = location.pathname.replace(/List.aspx/ig, 'List.aspx?action=Submit')
                            + "&FinanceReceiptId=" + financeReceiptId + "&OrderID=" + $("#OrderId-receipt-datagrid").html();
                        var params = {
                            "ReceiveData": JSON.stringify(submitData),
                        };

                        //$(".window-mask").css("z-index", "19000");
                        $('#receipt-dialog').dialog('close');
                        MaskUtil.mask();
                        $.post(url, params, function (res) {
                            //$(".window-mask").css("z-index", "9006");
                            MaskUtil.unmask();
                            var resData = JSON.parse(res);
                            if (resData.success == "true") {
                                $('#receipt-dialog').dialog('close');
                                $("#ClearAmount").text(resData.clearAmount);

                                daiShouAll = (Number(NoticeData.Amount) - Number(resData.clearAmount)).toFixed(2); //更新"总共待收金额"
                                //更新收款弹框中显示的"总共待收金额"
                                $("#ShengyuAmount-receipt-datagrid").text(daiShouAll);
                                console.log("收款成功，当前待收总金额变为：" + daiShouAll);
                            } else {
                                //$.messager.alert('提示', resData.message);
                                $.messager.alert('消息', resData.message, 'info', function () {
                                    location.reload();
                                });
                            }
                        });



                    }
                }, {
                    text: '取消',
                    iconCls: 'icon-cancel',
                    handler: function () {
                        $('#receipt-dialog').dialog('close');
                    }
                }]
            });

            //一笔款项的收款记录 弹框, 一些设置
            $('#receiptRecordFinane-dialog').dialog({
                buttons: [{
                    text: '关闭',
                    width: '52px',
                    handler: function () {
                        $('#receiptRecordFinane-dialog').dialog('close');
                    }
                }]
            });

            //某个订单,某个类别的收款记录 弹框, 一些设置
            $('#receiptRecordOrder-dialog').dialog({
                buttons: [{
                    text: '关闭',
                    width: '52px',
                    handler: function () {
                        $('#receiptRecordOrder-dialog').dialog('close');
                    }
                }]
            });

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////// 收款弹框中 checkbox 选择事件 Begin ////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////

            //收款弹框中的表格
            $('#receipt-datagrid').datagrid({
                //选中一行
                onCheck: function (rowIndex, rowData) {
                    var thisHangYingShou = rowData.ReceivableAmount;
                    var thisHangYaoShou = 0;
                    //当前待收总金额 - 当前要收总金额 >= 该行应收, 可以收该行全部
                    if ((Number(daiShouAll) - Number(yaoshouAll)).toFixed(2) >= Number(thisHangYingShou)) {
                        if (Number(thisHangYingShou) < 0) {
                            thisHangYaoShou = 0;
                        } else {
                            thisHangYaoShou = thisHangYingShou;
                        }
                    } else {
                        //只能收 (daiShouAll - yaoshouAll) 这么多钱
                        thisHangYaoShou = (Number(daiShouAll) - Number(yaoshouAll)).toFixed(2);
                    }

                    ///////////////////////////////////// 这里区别出了“货款”
                    if (0 < Number(thisHangYaoShou) || rowData.FeeTypeValue == "1") {
                        $("input[orderReceiptId='" + rowData.OrderReceiptId + rowData.FeeSourceID + "']").val(thisHangYaoShou);
                        yaoshouAll = (Number(yaoshouAll) + Number(thisHangYaoShou)).toFixed(2);
                        $("#yaoshouBox").html(yaoshouAll);
                    }


                    //清除掉 空输入的行的选中
                    var thisHangShuRu = $("input[orderReceiptId='" + rowData.OrderReceiptId + rowData.FeeSourceID + "']").val();
                    if (thisHangShuRu == null || thisHangShuRu == "") {
                        $('#receipt-datagrid').datagrid('uncheckRow', rowIndex);
                    }
                },
                //取消选中一行
                onUncheck: function (rowIndex, rowData) {
                    var thisHangYaoShou = 0;
                    var $input = $("input[orderReceiptId='" + rowData.OrderReceiptId + rowData.FeeSourceID + "']");
                    thisHangYaoShou = $input.val();
                    $input.val("");

                    yaoshouAll = (Number(yaoshouAll) - Number(thisHangYaoShou)).toFixed(2);

                    $("#yaoshouBox").html(yaoshouAll);
                },
                //全选
                onCheckAll: function (rows) {
                    console.log(rows);

                    for (var i = 0; i < rows.length; i++) {
                        if (!rows[i].IsFooter) {
                            var thisHangShuRu = 0;
                            thisHangShuRu = $("input[orderReceiptId='" + rows[i].OrderReceiptId + rows[i].FeeSourceID + "']").val();

                            if (thisHangShuRu == null || thisHangShuRu == "") {
                                var isBreak = 0;

                                //未输入的
                                var thisHangYingShou = rows[i].ReceivableAmount;
                                var thisHangYaoShou = 0;
                                //当前待收总金额 - 当前要收总金额 >= 该行应收, 可以收该行全部
                                if ((Number(daiShouAll) - Number(yaoshouAll)).toFixed(2) >= Number(thisHangYingShou)) {
                                    if (Number(thisHangYingShou) < 0) {
                                        thisHangYaoShou = 0;
                                    } else {
                                        thisHangYaoShou = thisHangYingShou;
                                    }
                                } else {
                                    //只能收 (daiShouAll - yaoshouAll) 这么多钱
                                    thisHangYaoShou = (Number(daiShouAll) - Number(yaoshouAll)).toFixed(2);
                                    isBreak = 1;
                                }

                                //如果这行只能的钱 <= 0元,不用进行下面的逻辑了,直接跳出
                                if (0 >= Number(thisHangYaoShou)) {
                                    break;
                                }

                                $("input[orderReceiptId='" + rows[i].OrderReceiptId + rows[i].FeeSourceID + "']").val(thisHangYaoShou);
                                yaoshouAll = (Number(yaoshouAll) + Number(thisHangYaoShou)).toFixed(2);
                                $("#yaoshouBox").html(yaoshouAll);

                                if (1 == isBreak) {
                                    break;
                                }
                            } else {
                                //已经输入的,不处理

                            }
                        }
                    }

                    //清除掉 空输入的行的选中
                    for (var i = 0; i < rows.length; i++) {
                        var thisHangShuRu = $("input[orderReceiptId='" + rows[i].OrderReceiptId + rows[i].FeeSourceID + "']").val();
                        if (thisHangShuRu == null || thisHangShuRu == "") {
                            $('#receipt-datagrid').datagrid('uncheckRow', i);
                        }
                    }
                },
                //全部不选
                onUncheckAll: function (rows) {
                    for (var i = 0; i < rows.length; i++) {
                        var thisHangYaoShou = 0;
                        var $input = $("input[orderReceiptId='" + rows[i].OrderReceiptId + rows[i].FeeSourceID + "']");
                        thisHangYaoShou = $input.val();
                        $input.val("");

                        yaoshouAll = (Number(yaoshouAll) - Number(thisHangYaoShou)).toFixed(2);
                    }
                    $("#yaoshouBox").html(yaoshouAll);
                },
            });

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////// 收款弹框中 checkbox 选择事件 End /////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
           
        });

        //初始化页面上的显示数据
        function IntData() {
            if (NoticeData != null) {
                $("#ClienName").text(NoticeData.ClienName);
                $("#Amount").text(NoticeData.Amount);
                $("#ClearAmount").text(NoticeData.ClearAmount);
                $("#ReceiptDate").text(NoticeData.ReceiptDate);

                //收款弹框中的内容 Begin
                $("#ClienName-receipt-datagrid").text(NoticeData.ClienName);
                $("#Amount-receipt-datagrid").text(NoticeData.Amount);
                $("#ShengyuAmount-receipt-datagrid").text((Number(NoticeData.Amount) - Number(NoticeData.ClearAmount)).toFixed(2));
                //收款弹框中的内容 End
            }
        }

        //显示收款弹框
        function ShowReceiptDialog(orderId) {
            if (orderId) {
                $('#receipt-datagrid').datagrid({
                    url: location.pathname.replace(/List.aspx/ig, 'List.aspx?action=data&SubAction=ReceivableOrderReceipt&OrderID=' + orderId),
                    loadFilter: function (data) {
                        if (data.rows.length > 0) {
                            $("#OrderId-receipt-datagrid").text(data.rows[0].OrderID);
                        }

                        for (var index = 0; index < data.rows.length; index++) {
                            var row = data.rows[index];
                            for (var name in row.item) {
                                row[name] = row.item[name];
                            }
                            delete row.item;
                        }
                        return data;
                    },
                    onClickRow: function (rowIndex, rowData) {
                        var currentOrderReceiptId = rowData.OrderReceiptId;
                        var currentIsSelectByThisClick = 0;
                        var allSelections = $(this).datagrid('getSelections');
                        for (var i = 0; i < allSelections.length; i++) {
                            if (allSelections[i].OrderReceiptId == currentOrderReceiptId) {
                                currentIsSelectByThisClick = 1;
                                break;
                            }
                        }

                        if (1 == currentIsSelectByThisClick) {
                            $(this).datagrid('unselectRow', rowIndex);
                        } else {
                            $(this).datagrid('selectRow', rowIndex);
                        }
                    },
                    onLoadSuccess: function () {
                        //给所有输入框加上事件
                        var rows = $("#receipt-datagrid").datagrid("getRows");
                        for (var i = 0; i < rows.length; i++) {
                            //限制输入框 只允许输入数字和小数点, 并且要进行计算 Begin
                            $("input[orderReceiptId='" + rows[i].OrderReceiptId + rows[i].FeeSourceID + "']").each(function (j) {
                                $(this).keyup(function () {

                                    //清除没有打勾的输入框 Begin
                                    for (var i = 0; i < rows.length; i++) {
                                        var isDagou = 0;
                                        var checkedItems = $('#receipt-datagrid').datagrid('getChecked');
                                        $.each(checkedItems, function (index, item) {
                                            if (rows[i].OrderReceiptId == item.OrderReceiptId) {
                                                isDagou = 1;
                                            }
                                        });

                                        if (0 == isDagou) {
                                            $("input[orderReceiptId='" + rows[i].OrderReceiptId + rows[i].FeeSourceID + "']").val("");
                                        }
                                    }
                                    //清除没有打勾的输入框 End

                                    //只允许输入数字和小数点
                                    $(this).val($(this).val().replace(/[^0-9.]/g, ''));

                                    //不让输入两个及以上的小数点,并且只允许输入最多小数点后两位
                                    $(this).val(sliceDecimal($(this).val()));

                                    //要收总金额 计算 并且实时显示 Begin
                                    var tempsum = 0;
                                    for (var i = 0; i < rows.length; i++) {
                                        var thisHangInputValue = $("input[orderReceiptId='" + rows[i].OrderReceiptId + rows[i].FeeSourceID + "']").val();
                                        if (thisHangInputValue != null && thisHangInputValue != "") {
                                            tempsum = (Number(tempsum) + Number(thisHangInputValue)).toFixed(2);
                                        }
                                    }

                                    yaoshouAll = tempsum;
                                    $("#yaoshouBox").html(yaoshouAll);

                                    //要收总金额 计算 并且实时显示 End

                                });
                            });
                            //限制输入框 只允许输入数字和小数点, 并且要进行计算  End
                        }
                        var trs = $("#receipt-dialog").find(".datagrid-view").find(".datagrid-view2").find("tr");
                        for (var i = 0; i < trs.length; i++) {
                            var beishu = 12;
                            $(trs[i]).find("td:nth-child(2)").find("div").width(10 * beishu); //10
                            $(trs[i]).find("td:nth-child(3)").find("div").width(16 * beishu); //16
                            $(trs[i]).find("td:nth-child(4)").find("div").width(16 * beishu); //16
                            $(trs[i]).find("td:nth-child(5)").find("div").width(20 * beishu); //20
                            $(trs[i]).find("td:nth-child(6)").find("div").width(8 * beishu);  //8
                        }
                    },
                });
                //显示收款弹框时, 要收总金额置为0
                yaoshouAll = 0;
                console.log("收款弹框打开,要收总金额置为0");
                console.log("当前待收金额：" + daiShouAll);

                $('#receipt-dialog').dialog('open');
            }
        }

        //如果该字符串有两个小数点,就返回第二个小数点之前的字符串,否则返回原字符串
        function sliceDecimal(origin) {
            if (origin == null || origin.length <= 0) {
                return "";
            }
            ////没有小数点就返回原字符串
            //if (origin.search(".") == -1) {
            //    return origin;
            //}
            //如果只有一个小数点,返回原字符串. 如果有两个小数点,就返回第二个小数点之前的字符串
            var dotLocArr = [];
            for (var i = 0; i < origin.length; i++) {
                if (origin[i] == '.') {
                    dotLocArr.push(i);
                }
            }
            //没有小数点就返回原字符串
            if (dotLocArr.length == 0) {
                return origin;
            }
            if (dotLocArr.length == 1) {
                //只有一个小数点,如果小数点后有两位以上小数位,则切掉再往后的小数位
                if (origin.length - 1 <= dotLocArr[0] + 2) {
                    return origin;
                }
                return origin.slice(0, dotLocArr[0] + 3);
            }
            //有两个或以上个数的小数点
            return origin.slice(0, dotLocArr[1]);
        }

        function UpdateClearAmount() {
            var url = location.pathname + '?action=GetCurrentClearAmount';

            var params = {
                "FinanceReceiptId": financeReceiptId
            };

            $.post(url, params, function (res) {
                var resData = JSON.parse(res);
                if (resData.success == "true") {
                    $("#ClearAmount").text(resData.clearAmount);
                }
            });
        }

        // ----------------------------------------------------------- 收款弹框 js Begin -----------------------------------------------------------

        //收款弹框 查看 按钮
        function ReceiptOperation(val, row, index) {
            var buttons = "";
            if (!row.IsFooter) {
                buttons = '<a href="javascript:void(0);" style="margin:3px; color: #134da5;" onclick="ShowReceiptRecordOrder(\'' + row.OrderReceiptId + '\')">收款记录</a>';
            }
            return buttons;
        }

        //收款弹框 实收 输入框
        function ReceiptReceiveInput(val, row, index) {
            var inputs = "";
            if (!row.IsFooter) {

                if (row.FeeTypeValue == "1") {
                    inputs += '<input orderReceiptId=' + row.OrderReceiptId + row.FeeSourceID + ' type="text" class="textbox-text" style="height:18px;">';
                    if (row.FeeSourceID != "" && row.FeeSourceID != null) {
                        inputs += '<br>付汇编号:' + row.FeeSourceID;
                    }
                    //inputs += '<input orderReceiptId=' + row.OrderReceiptId + ' type="text" class="textbox-text" style="height:18px;">';

                    //如果是货款， 需要录入付汇申请ID  ryan 2020-11-08
                    //inputs += '<input orderPayExchangeId=' + row.OrderReceiptId + ' type="text" class="textbox-text" placeholder="请输入付汇ID" style="height:18px;">'
                }
                else {
                    inputs += '<input orderReceiptId=' + row.OrderReceiptId + row.FeeSourceID + ' type="text" class="textbox-text" style="height:18px;">';
                }

            } else {
                inputs = '<span class="datagrid-cell" style="text-align:center; height:32px;" id="yaoshouBox">0</span>';
            }
            return inputs;
        }

        // ----------------------------------------------------------- 收款弹框 js End -----------------------------------------------------------

        // ------------------------------------------------------ 一笔款项的收款记录 js Begin ----------------------------------------------------

        function ShowReceiptRecordFinance() {
            $('#receiptRecordFinane-datagrid').myDatagrid({
                nowrap: false,
                border: false,
                fitColumns: true,
                scrollbarSize: 0,
                fit: true,
                singleSelect: false,
                actionName: 'GetReceiptRecordFinance',
                queryParams: {
                    FinanceReceiptId: financeReceiptId,
                },
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },

                onLoadSuccess: function (data) {
                    var trs = $("#receiptRecordFinane-dialog").find(".datagrid-view").find(".datagrid-view2").find("tr");
                    for (var i = 0; i < trs.length; i++) {
                        var beishu = 11;
                        $(trs[i]).find("td:nth-child(1)").find("div").width(12 * beishu);
                        $(trs[i]).find("td:nth-child(2)").find("div").width(10 * beishu);
                        $(trs[i]).find("td:nth-child(3)").find("div").width(20 * beishu);
                        $(trs[i]).find("td:nth-child(4)").find("div").width(12 * beishu);
                        $(trs[i]).find("td:nth-child(5)").find("div").width(10 * beishu);
                        $(trs[i]).find("td:nth-child(6)").find("div").width(20 * beishu);
                    }
                },
            });

            $('#receiptRecordFinane-dialog').dialog('open');
        }

       

        // ------------------------------------------------------ 一笔款项的收款记录 js End ----------------------------------------------------

        //------------------------------------------------- 某个订单,某个类别的收款记录 js Begin -----------------------------------------------

        function ShowReceiptRecordOrder(orderReceiptId) {
            $('#receiptRecordOrder-datagrid').datagrid({
                url: "?action=ReceiptRecordOrder&OrderReceiptId=" + orderReceiptId,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    var trs = $("#receiptRecordOrder-dialog").find(".datagrid-view").find(".datagrid-view2").find("tr");
                    for (var i = 0; i < trs.length; i++) {
                        var bei = 10;
                        $(trs[i]).find("td:nth-child(1)").find("div").width(12 * bei);
                        $(trs[i]).find("td:nth-child(2)").find("div").width(8 * bei);
                        $(trs[i]).find("td:nth-child(3)").find("div").width(8 * bei);
                        $(trs[i]).find("td:nth-child(4)").find("div").width(12 * bei);
                        $(trs[i]).find("td:nth-child(5)").find("div").width(12 * bei);
                        $(trs[i]).find("td:nth-child(6)").find("div").width(11 * bei);
                    }
                },
            });

            $('#receiptRecordOrder-dialog').dialog('open');
        }

        //------------------------------------------------- 某个订单,某个类别的收款记录 js End -----------------------------------------------  
    </script>
    <%--待报关订单--%>
    <script>
        function GetQuery() {
            var DecMainOrderID = $('#DecMainOrderID').textbox('getValue');
            var DecOrderStartDate = $('#DecOrderStartDate').datebox('getValue');
            var DecOrderEndDate = $('#DecOrderEndDate').datebox('getValue');
            var params = {
                action: 'DataStore',
                ClienName: NoticeData.ClienName,
                DecMainOrderID: DecMainOrderID,
                DecOrderStartDate: DecOrderStartDate,
                DecOrderEndDate: DecOrderEndDate,
            };
            return params;
        }
        //代报关订单查询
        function DecSearch() {
            $('#datagrid').myDatagrid('search', GetQuery());
        }
        //代报关订单重置
        function DecReset() {
            $('#DecMainOrderID').textbox('setValue', null);
            $('#DecOrderStartDate').datebox('setValue', null);
            $('#DecOrderEndDate').datebox('setValue', null);

            DecSearch();
        }
        //一键收款
        function OneKeyReceipt() {
            var data = $('#datagrid').myDatagrid('getChecked');
            if (data.length == 0) {
                $.messager.alert('提示', '请勾选要收款的订单');
                return;
            }

            //拼接OrderIDs字符串
            var OrderIDs = "";
            for (var i = 0; i < data.length; i++) {
                OrderIDs += data[i].ID + ",";
            }
            OrderIDs = OrderIDs.substr(0, OrderIDs.length - 1);

            var url = location.pathname.replace(/List.aspx/ig, 'OneKeyReceipt.aspx') + "?OrderIDs=" + OrderIDs + "&financeReceiptId=" + financeReceiptId;

            $.myWindow({
                iconCls: "",
                noheader: false,
                title: '一键收款',
                width: '400',
                height: '200',
                url: url,
                onClose: function () {
                    location.reload();
                }
            });

        }
        //列表按钮
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ShowReceiptDialog(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">收款</span>' +
                '<span class="l-btn-icon icon-add">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
    <%--香港代仓储订单--%>
    <script>
        function StoreGetQuery() {
            var StoreMainOrderID = $('#StoreMainOrderID').textbox('getValue');
            var StoreOrderStartDate = $('#StoreOrderStartDate').datebox('getValue');
            var StoreOrderEndDate = $('#StoreOrderEndDate').datebox('getValue');
            var params = {
                action: 'DataStore',
                ClienName: NoticeData.ClienName,
                StoreMainOrderID: StoreMainOrderID,
                StoreOrderStartDate: StoreOrderStartDate,
                StoreOrderEndDate: StoreOrderEndDate,
            };
            return params;
        }
        //查询
        function StoreSearch() {
            $('#datagrid-store').myDatagrid({
                actionName: 'DataStore',
                queryParams: StoreGetQuery(),
                nowrap: false,
                border: false,
                fitColumns: true,
                scrollbarSize: 0,
                fit: true,
                singleSelect: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    if (data.total == 0) {
                        //$(this).datagrid('appendRow', { ID: '<div>未查询到该付款人的代仓储应收款订单信息</div>' }).datagrid('mergeCells', { index: 0, field: 'ID', colspan: 9 });
                        return;
                    }
                },
            });
        }
        //重置
        function StoreReset() {
            $('#StoreMainOrderID').textbox('setValue', null);
            $('#StoreOrderStartDate').datebox('setValue', null);
            $('#StoreOrderEndDate').datebox('setValue', null);

            StoreSearch();
        }
        //查看收款记录
        function ShowStoreReceiptRecordFinance() {
            var url = location.pathname.replace(/List.aspx/ig, '../Store/ReceivedStoreList.aspx')
                + '?ClientName=' + NoticeData.ClienName;

            top.$.myWindow({
                iconCls: "icon-search",
                url: url,
                noheader: false,
                title: '收款记录',
                width: '1000px',
                height: '600px',
                onClose: function () {
                    $('#datagrid-store').datagrid('reload');
                }
            });
        }
        //列表按钮
        function StoreOperation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ShowStoreReceiptDialog(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">收款</span>' +
                '<span class="l-btn-icon icon-add">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }        
        //显示待仓储订单收款弹框
        function ShowStoreReceiptDialog(mainOrderID) {
            var url = location.pathname.replace(/List.aspx/ig, '../Store/StoreReceiptDialog.aspx')
                + '?MainOrderID=' + mainOrderID + "&FinanceReceiptId=" + financeReceiptId;

            top.$.myWindow({
                iconCls: "icon-edit",
                url: url,
                noheader: false,
                title: '待仓储收款',
                width: '750px',
                height: '480px',
                onClose: function () {
                    $('#datagrid-store').datagrid('reload');
                    UpdateClearAmount();
                }
            });
        }
    </script>
    <%--深圳代仓储账单--%>
    <script>
        function SzStoreGetQuery() {
            var CutDateIndex = $('#CutDateIndex').numberbox('getValue');
            var params = {
                action: 'SzDataStore',
                ClienName: NoticeData.ClienName,
                CutDateIndex: CutDateIndex,
            };
            return params;
        }
        //查询
        function SzStoreSearch() {
            $('#datagrid-store-sz').myDatagrid({
                actionName: 'SzDataStore',
                queryParams: SzStoreGetQuery(),
                nowrap: false,
                border: false,
                fitColumns: true,
                scrollbarSize: 0,
                fit: true,
                singleSelect: true,
                toolbar: '#topBar-store-sz',
                rownumbers: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    if (data.total == 0) {
                        //$(this).datagrid('appendRow', { ID: '<div>未查询到该付款人的代仓储应收款订单信息</div>' }).datagrid('mergeCells', { index: 0, field: 'ID', colspan: 9 });
                        return;
                    }
                },
            });
        }
        //重置
        function SzStoreReset() {
            $('#CutDateIndex').numberbox('setValue', "");
            SzStoreSearch();
        }
        //查看收款记录
        function SzStoreReceiptRecords() {
            var url = location.pathname.replace(/List.aspx/ig, '../SzStore/ReceiptRecordsList.aspx')
                + '?ClientName=' + NoticeData.ClienName + "&SeqNo=" + SeqNo;
            top.$.myWindow({
                iconCls: "icon-search",
                url: url,
                noheader: false,
                title: '收款记录',
                width: '1200px',
                height: '600px',
            });
        }
        //列表按钮
        function SzStoreOperation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ShowSzStoreReceiptDialog(\'' + row.CutDateIndex + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">收款</span>' +
                '<span class="l-btn-icon icon-add">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
        //收款弹框
        function ShowSzStoreReceiptDialog(CutDateIndex) {
            var url = location.pathname.replace(/List.aspx/ig, '../SzStore/PayeeLeftsList.aspx')
                + '?CutDateIndex=' + CutDateIndex + "&ClientName=" + NoticeData.ClienName + "&SeqNo=" + SeqNo + "&FinanceReceiptId=" + financeReceiptId;

            top.$.myWindow({
                iconCls: "icon-edit",
                url: url,
                title: '账单收款',
                width: '1200px',
                height: '600px',
                onClose: function () {
                    $('#datagrid-store-sz').datagrid('reload');
                    UpdateClearAmount();
                }
            });
        }
    </script>
    <style>
        .divstyle {
            display: inline;
            margin: 20px 5px;
        }

        .lbl {
            width: 70px;
            font-weight: bold;
        }
    </style>
</head>
<body class="easyui-layout" style="width: 100%; height: 100%">
    <div data-options="region:'north',border:false," style="height: 80px; margin-left: 10px;">
        <div style="margin: 5px 0px;">
            <a id="btnBackToUnReceiveList" class="easyui-linkbutton" data-options="iconCls:'icon-back'">返回</a>
        </div>
        <table>
            <tr>
                <td class="lbl">
                    <label>付款人:</label></td>
                <td colspan="5">
                    <label id="ClienName"></label>
                </td>
            </tr>
            <tr>
                <td class="lbl">
                    <label>付款时间:</label></td>
                <td style="width: 100px">
                    <label id="ReceiptDate"></label>
                </td>
                <td class="lbl">
                    <label>付款金额:</label></td>
                <td style="width: 100px">
                    <label id="Amount"></label>
                </td>
                <td style="width: 90px; font-weight: bold">
                    <label>已确认金额:</label></td>
                <td>
                    <label id="ClearAmount"></label>
                </td>
            </tr>
        </table>
    </div>
    <div data-options="region:'center',border:false,">
        <div id="tt" class="easyui-tabs" style="height: 100%; margin-left: 10px">
            <div title="代报关订单">
                <div id="topBar">
                    <div style="margin: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" style="margin-top: -5px;" onclick="ShowReceiptRecordFinance()">查看收款记录</a>
                        <label style="margin-left: 20px;">订单编号：</label>
                        <input class="easyui-textbox" id="DecMainOrderID" style="width: 200px;" data-options="validType:'length[1,50]'" />
                        <label style="margin-left: 10px;">下单时间：</label>
                        <input class="easyui-datebox" id="DecOrderStartDate" data-options="width:200," />
                        <label style="margin-left: 10px;">至</label>
                        <input class="easyui-datebox" id="DecOrderEndDate" data-options="width:200," />
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="DecSearch()" style="margin-left: 10px;">查询</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="DecReset()">重置</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="OneKeyReceipt()" style="margin-left: 10px;">一键收款</a>
                    </div>
                </div>
                <table id="datagrid" title="" data-options="
                    nowrap:false,
                    border:false,
                    fitColumns:true,
                    scrollbarSize:0,
                    fit:true,
                    singleSelect:true,
                    toolbar:'#topBar',
                    rownumbers:true">
                    <thead>
                        <tr>
                            <th data-options="field:'CheckBox',align:'center',checkbox:true," style="width: 10px;"></th>
                            <th data-options="field:'ID',align:'center'" style="width: 100px;">订单编号</th>
                            <th data-options="field:'ClientCode',align:'center'" style="width: 70px;">客户编号</th>
                            <th data-options="field:'ClientName',align:'left'" style="width: 100px;">客户名称</th>
                            <th data-options="field:'Currency',align:'center'" style="width: 100px;">币种</th>
                            <th data-options="field:'DeclarePrice',align:'center'" style="width: 100px;">报关总价</th>
                            <th data-options="field:'TotalDeclarePrice',align:'center'" style="width: 100px;">报关总金额（CNY）</th>
                            <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">下单时间 </th>
                            <th data-options="field:'OrderStatus',align:'center'" style="width: 100px;">订单状态</th>
                            <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px;">操作</th>
                        </tr>
                    </thead>
                </table>
            </div>
            <div title="香港代仓储订单">
                <div id="topBar-store" style="padding: 5px">
                    <div style="margin: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" style="margin-top: -5px;" onclick="ShowStoreReceiptRecordFinance()">查看收款记录</a>
                        <label style="margin-left: 20px;">订单编号：</label>
                        <input class="easyui-textbox" id="StoreMainOrderID" style="width: 200px;" data-options="validType:'length[1,50]'" />
                        <label style="margin-left: 10px;">下单时间：</label>
                        <input class="easyui-datebox" id="StoreOrderStartDate" data-options="width:200," />
                        <label style="margin-left: 10px;">至</label>
                        <input class="easyui-datebox" id="StoreOrderEndDate" data-options="width:200," />
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="StoreSearch()" style="margin-left: 10px;">查询</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="StoreReset()">重置</a>
                    </div>
                </div>
                <table id="datagrid-store" title="" data-options="
                    nowrap:false,
                    border:false,
                    fitColumns:true,
                    scrollbarSize:0,
                    fit:false,
                    singleSelect:true,
                    toolbar:'#topBar-store',
                    rownumbers:true">
                    <thead>
                        <tr>
                            <th data-options="field:'ID',align:'center'" style="width: 100px;">订单编号</th>
                            <th data-options="field:'ClientName',align:'left'" style="width: 120px;">客户名称</th>
                            <th data-options="field:'SettlementCurrency',align:'center'" style="width: 40px;">币种</th>
                            <th data-options="field:'SupplierName',align:'left'" style="width: 120px;">供应商</th>
                            <th data-options="field:'CreateDate',align:'center'" style="width: 80px;">下单时间 </th>
                            <th data-options="field:'MainStatus',align:'center'" style="width: 80px;">订单状态</th>
                            <th data-options="field:'Btn',align:'center',formatter:StoreOperation" style="width: 100px;">操作</th>
                        </tr>
                    </thead>
                </table>
            </div>
            <div title="深圳代仓储账单">
                <div id="topBar-store-sz">
                    <div style="margin: 10px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" style="margin-top: -5px;" onclick="SzStoreReceiptRecords()">查看收款记录</a>
                        <label style="margin-left: 20px;">账单期号：</label>
                        <input class="easyui-numberbox" id="CutDateIndex" style="width: 200px;" data-options="validType:'length[6,6]'" />
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="SzStoreSearch()" style="margin-left: 10px;">查询</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="SzStoreReset()">重置</a>
                    </div>
                </div>
                <table id="datagrid-store-sz">
                    <thead>
                        <tr>
                            <th data-options="field:'CutDateIndex',align:'center'" style="width: 100px;">账单期号</th>
                            <th data-options="field:'PayerName',align:'left'" style="width: 200px;">客户名称</th>
                            <th data-options="field:'Currency',align:'center'" style="width: 100px;">币种</th>
                            <th data-options="field:'Total',align:'center'" style="width: 100px;">账单总金额</th>
                            <th data-options="field:'ReceiptTotal',align:'center'" style="width: 100px;">实收总金额</th>
                            <th data-options="field:'Btn',align:'center',formatter:SzStoreOperation" style="width: 120px;">操作</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
    </div>

    <!------------------------------------------------------------ 收款弹框 html Begin ------------------------------------------------------------>
    <div id="receipt-dialog" class="easyui-dialog" title="收款" style="width: 800px; height: 500px;"
        data-options="iconCls:'icon-edit', resizable:false, modal:true, closed: true,">
        <table id="receipt-datagrid" data-options="
            nowrap:false,
            border:false,
            showFooter:true,
            autoRowHeight:true,
            checkOnSelect:false,
            selectOnCheck:true,
            fitColumns:true,
            scrollbarSize:0,
            fit:true,
            singleSelect:false,
            toolbar:'#topBar-receipt-datagrid',
            rownumbers:false">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true," style="width: 10px;"></th>
                    <th data-options="field:'FeeTypeShowName',align:'center'" style="width: 16px;">类型</th>
                    <th data-options="field:'ReceivableAmount',align:'center'" style="width: 10px;">应收</th>
                    <th data-options="field:'Input',align:'center',formatter:ReceiptReceiveInput" style="width: 26px;">实收</th>
                    <th data-options="field:'Btn',align:'center',formatter:ReceiptOperation" style="width: 8px;">查看</th>
                </tr>
            </thead>
        </table>
    </div>

    <!-- 收款弹框中 表格工具栏 -->
    <div id="topBar-receipt-datagrid">
        <div style="padding-bottom: 5px;">
            <div style="margin-top: 5px;">
                <div class="divstyle">
                    <span style="font-size: 14px">订单编号：</span>
                    <label id="OrderId-receipt-datagrid" style="font-size: 16px"></label>
                </div>
            </div>
            <div style="margin-top: 5px">
                <div class="divstyle">
                    <span style="font-size: 14px">客户名称：</span>
                    <label id="ClienName-receipt-datagrid" style="font-size: 16px"></label>
                </div>
                <div class="divstyle">
                    <span style="font-size: 14px">付款金额：</span>
                    <label id="Amount-receipt-datagrid" style="font-size: 16px"></label>
                </div>
                <%--<div class="divstyle"">
                    <span style="font-size: 14px">抵用券金额：</span>
                    <label id="VoucherAmount-receipt-datagrid" style="font-size: 16px"></label>
                </div>
                <div class="divstyle"">
                    <span style="font-size: 14px">总的可用金额：</span>
                    <label id="AllAmount-receipt-datagrid" style="font-size: 16px"></label>
                </div>--%>
                <div class="divstyle">
                    <span style="font-size: 14px">待收金额：</span>
                    <label id="ShengyuAmount-receipt-datagrid" style="font-size: 16px"></label>
                </div>
            </div>
        </div>
    </div>
    <!------------------------------------------------------------ 收款弹框 html End ------------------------------------------------------------>

    <!------------------------------------------------------------ 一笔款项的收款记录 html Begin ------------------------------------------------------------>

    <div id="receiptRecordFinane-dialog" class="easyui-dialog" title="收款记录" style="width: 1000px; height: 600px;"
        data-options="iconCls:'icon-edit', resizable:false, modal:true, closed: true,">
        <table id="receiptRecordFinane-datagrid" data-options="
            nowrap:false,
            border:false,
            fitColumns:true,
            scrollbarSize:0,
            fit:true,
            singleSelect:false,
            rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 12px;">时间</th>
                    <th data-options="field:'AdminName',align:'center'" style="width: 10px;">收款人</th>
                    <th data-options="field:'OrderID',align:'center'" style="width: 20px;">订单编号</th>
                    <th data-options="field:'FeeType',align:'center'" style="width: 12px;">费用类型</th>
                    <th data-options="field:'ReceiptType',align:'center'" style="width: 10px;">收款类型</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 20px;">实收</th>
                </tr>
            </thead>
        </table>
    </div>

    <!------------------------------------------------------------ 一笔款项的收款记录 html End ------------------------------------------------------------>

    <!------------------------------------------------------------ 某个订单,某个类别的收款记录 html Begin ------------------------------------------------------------>

    <div id="receiptRecordOrder-dialog" class="easyui-dialog" title="收款记录" style="width: 750px; height: 350px;"
        data-options="iconCls:'icon-edit', resizable:false, modal:true, closed: true,">
        <table id="receiptRecordOrder-datagrid" data-options="
            nowrap:false,
            border:false,
            fitColumns:true,
            scrollbarSize:0,
            fit:true,
            singleSelect:false,
            rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 12px;">时间</th>
                    <th data-options="field:'AdminName',align:'center'" style="width: 12px;">收款人</th>
                    <th data-options="field:'ReceiptType',align:'center'" style="width: 8px;">收款类型</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 16px;">实收</th>
                    <th data-options="field:'SeqNo',align:'left'" style="width: 16px;">银行流水号</th>
                    <th data-options="field:'ReceiptDate',align:'center'" style="width: 16px;">银行收款日期</th>
                </tr>
            </thead>
        </table>
    </div>

    <!------------------------------------------------------------ 某个订单,某个类别的收款记录 html End ------------------------------------------------------------>
</body>
</html>
