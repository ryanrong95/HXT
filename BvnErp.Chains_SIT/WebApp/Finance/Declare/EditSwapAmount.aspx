<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditSwapAmount.aspx.cs" Inherits="WebApp.Finance.Declare.EditSwapAmount" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var bankID = '<%=this.Model.BankID%>';
        var bankName = '<%=this.Model.BankName%>';
        var CleanIDs = getQueryString("CleanIDs");
        var TheInputAmounts = getQueryString("TheInputAmounts"); //已经输入的自定义换汇金额的值
        //格式： XDTCDO201908280000002|243.38,XDTCDO201908290000001|8.99

        var Currency = getQueryString("Currency");  

        var ForAddDecHeadIDs = '';
        var ForAddTheInputAmounts = '';

        var initUserCurrentPayApplyHeJi = 0;

        $(function () {
            $('#decheads').datagrid({
                nowrap:false,
                border:false,
                fitColumns:true,
                scrollbarSize:0,
                
                singleSelect:false,
                rownumbers:true,
                url: "?action=data&CleanIDs=" + CleanIDs,
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
                    //初始化查看报关单的查看框
                    $("a[name=btnView]").on('click', function () {
                        var $this = $(this);
                        var fileUrl = $this.data("fileurl");

                        $('#viewfileImg').css("display", "none");
                        $('#viewfilePdf').css("display", "none");
                        if (fileUrl.toLowerCase().indexOf('pdf') > 0) {
                            $('#viewfilePdf').attr('src', fileUrl);
                            $('#viewfilePdf').css("display", "block");

                        }
                        else {
                            $('#viewfileImg').attr('src', fileUrl);
                            $('#viewfileImg').css("display", "block");
                        }
                        $("#viewFileDialog").window('open').window('center');
                    });

                    //初始报关金额和已换汇金额
                    var initSwapAmountHeJi = 0;
                    var initSwapedAmountHeJi = 0;
                    //initUserCurrentPayApplyHeJi 已定义
                    for (var i = 0; i < data.rows.length; i++) {
                        if (data.rows[i].ID != 'appendRow') {
                            initSwapAmountHeJi = initSwapAmountHeJi + Number(data.rows[i].SwapAmount);
                            initSwapedAmountHeJi = initSwapedAmountHeJi + Number(data.rows[i].SwapedAmount);
                            initUserCurrentPayApplyHeJi = initUserCurrentPayApplyHeJi + Number(data.rows[i].UserCurrentPayApply);
                        }
                    }

                    $('#decheads').datagrid('appendRow', {
                        ID: 'appendRow', //这个 ID 指定为 appendRow, Operation() 方法中用到
                        ContrNo: '合计',
                        EntryID: '',
                        Currency: '',
                        SwapAmount: initSwapAmountHeJi.toFixed(2),
                        SwapedAmount: initSwapedAmountHeJi.toFixed(2),
                        
                        btn: '',
                    });

                    //取消最后一行增加行的序号显示
                    CancelLastRowSerialNoDisplay();

                    //给每一个自定义换汇金额输入框初始化
                    for (var i = 0; i < data.rows.length; i++) {
                        if (data.rows[i].ID != 'appendRow') {
                            $("input[decheadid='" + data.rows[i].ID + "']").each(function (j) {
                                $(this).keyup(function () {
                                    //只允许输入数字和小数点
                                    //$(this).val($(this).val().replace(/[^0-9.]/g, ''));//ryan 20200902 付汇金额*1.002 与报关单金额有差额

                                    //只允许输入数字和小数点
                                    $(this).val(sliceDecimal($(this).val()));

                                    //计算当前输入的各个“自定义换汇金额”的总和，并实时显示
                                    var tempsum = 0;
                                    for (var i = 0; i < data.rows.length; i++) {
                                        if (data.rows[i].ID != 'appendRow') {
                                            var thisHangInputValue = $("input[decheadid='" + data.rows[i].ID + "']").val();
                                            if (thisHangInputValue != null && thisHangInputValue != "") {
                                                tempsum = (Number(tempsum) + Number(thisHangInputValue)).toFixed(2);
                                            }
                                        }
                                    }
                                    //显示当前输入的换汇金额总和
                                    SetCurrentInputSwapAmountSum(tempsum);

                                    //显示这一行的“客户本次申请金额”加上“输入换汇金额”总和得到本次换汇金额 (KeyUp 中) Begin

                                    var inputNum = 0;
                                    if ($(this).val() != null && $(this).val() != 'undefined' && $(this).val() != '') {
                                        inputNum = Number($(this).val());
                                    }
                                    var userApplyPayAmount = Number($("a[decheadid='" + "UserApplyPayAmount_" + $(this).attr("decheadid") + "']").html());
                                    $("span[decheadid='" + "ThisTimeSwapAmount_" + $(this).attr("decheadid") + "']").html((userApplyPayAmount + inputNum).toFixed(2));

                                    //显示这一行的“客户本次申请金额”加上“输入换汇金额”总和得到本次换汇金额 (KeyUp 中) End
                                });
                            });
                        }
                    }

                    //如果移除一行，刷新页面，带入之前输入的值，从 TheInputAmounts 中获取
                    //格式： XDTCDO201908280000002|243.38,XDTCDO201908290000001|8.99
                    if (TheInputAmounts != null && TheInputAmounts != "") {
                        var TheInputAmountsArray = TheInputAmounts.split(',');
                        for (var i = 0; i < TheInputAmountsArray.length; i++) {
                            var theValue = TheInputAmountsArray[i].split('|');
                            $("input[decheadid='" + theValue[0] + "']").val(theValue[1]);
                        }
                    } else {
                        //如果 TheInputAmounts 为 "", 则是一开始打开这个窗口的
                        var data = $('#decheads').datagrid('getData');
                        for (var i = 0; i < data.rows.length; i++) {
                            if ('appendRow' != data.rows[i].ID) {
                                //$("input[decheadid='" + data.rows[i].ID + "']").val(data.rows[i].MaxInputSwapAmount);
                                //一开始进入窗口，自定义换汇金额
                                //如果用户本次申请金额小于等于0，则填入最大值；如果用户本次申请金额大于0，填入 0
                                //if (Number(data.rows[i].UserCurrentPayApply) <= 0) {
                                //    $("input[decheadid='" + data.rows[i].ID + "']").val(Number(data.rows[i].MaxInputSwapAmount));
                                //} else {
                                //    $("input[decheadid='" + data.rows[i].ID + "']").val(0);
                                //}

                                $("input[decheadid='" + data.rows[i].ID + "']").val(Number(data.rows[i].MaxInputSwapAmount));
                            }
                        }
                    }





                    //计算当前输入的各个“自定义换汇金额”的总和，并实时显示
                    var tempsum = 0;
                    var tabdata = $('#decheads').datagrid('getData');
                    for (var i = 0; i < tabdata.rows.length; i++) {
                        if (tabdata.rows[i].ID != 'appendRow') {
                            var thisHangInputValue = $("input[decheadid='" + tabdata.rows[i].ID + "']").val();
                            if (thisHangInputValue != null && thisHangInputValue != "") {
                                tempsum = (Number(tempsum) + Number(thisHangInputValue)).toFixed(2);
                            }
                        }
                    }
                    //显示当前输入的换汇金额总和
                    SetCurrentInputSwapAmountSum(tempsum);

                    //显示这一行的“客户本次申请金额”加上“输入换汇金额”总和得到本次换汇金额 (Init) Begin

                    for (var i = 0; i < data.rows.length; i++) {
                        if (data.rows[i].ID != 'appendRow') {
                            $("input[decheadid='" + data.rows[i].ID + "']").each(function (j) {
                                var inputNum = 0;
                                if ($(this).val() != null && $(this).val() != 'undefined' && $(this).val() != '') {
                                    inputNum = Number($(this).val());
                                }
                                var userApplyPayAmount = Number($("a[decheadid='" + "UserApplyPayAmount_" + $(this).attr("decheadid") + "']").html());
                                $("span[decheadid='" + "ThisTimeSwapAmount_" + $(this).attr("decheadid") + "']").html((userApplyPayAmount + inputNum).toFixed(2));
                            });
                        }
                    }

                    //显示这一行的“客户本次申请金额”加上“输入换汇金额”总和得到本次换汇金额 (Init) End


                },
                onClickRow: function () {
                    //始终禁止选中行
                    $('#decheads').datagrid('clearSelections');
                },
            });







        });

        //保存
        function Save() {
            var ewindow = $.myWindow.getMyWindow("SwapApply2EditSwapAmount");

            //检查列表中还有数据，如果都被移除光了，阻止提交
            var data = $('#decheads').datagrid('getData');
            var isYouShuJu = false;
            for (var i = 0; i < data.rows.length; i++) {
                if ('appendRow' != data.rows[i].ID) {
                    isYouShuJu = true;
                    break;
                }
            }
            if (!isYouShuJu) {
                $.messager.alert('提示消息', "未提交任何数据，请重新选择需要换汇的报关单！", 'info', function () {

                });
                return;
            }

            //检查输入的“自定义换汇金额”，不能为空，不能小于等于0，不能大于可换汇金额（报关金额 - 已换汇金额）
            var errors = "";

            var data = $('#decheads').datagrid('getData');
            for (var i = 0; i < data.rows.length; i++) {
                if ('appendRow' != data.rows[i].ID) {
                    var maxInputSwapAmount = data.rows[i].MaxInputSwapAmount;
                    var theCurrentInput = $("input[decheadid='" + data.rows[i].ID + "']").val();
                    var userPayExchangeApplyAmount = data.rows[i].UserCurrentPayApply;

                    if (theCurrentInput == null || theCurrentInput == "") {
                        errors += "报关单 <label style=\"color:green\">" + data.rows[i].ContrNo + "</label> 自定义换汇金额<label style=\"color:red\">不能为空</label>！<br>"
                    } else {
                        var theCurrentInput_Num = Number(theCurrentInput).toFixed(2);
                        var userPayExchangeApplyAmount_Num = Number(userPayExchangeApplyAmount).toFixed(2);

                        //ryan 20200902 付汇金额*1.002 与报关单金额有差额
                        //if (theCurrentInput_Num < 0) {
                        //    errors += "报关单 <label style=\"color:green\">" + data.rows[i].ContrNo + "</label> 自定义换汇金额<label style=\"color:red\">不能小于0</label>！<br>"
                        //} else if (theCurrentInput_Num <= 0 && userPayExchangeApplyAmount_Num <= 0) {
                        //    errors += "报关单 <label style=\"color:green\">" + data.rows[i].ContrNo + "</label> 客户本次申请金额 和 自定义换汇金额<label style=\"color:red\">不能都为0</label>！<br>"
                        //} else if (maxInputSwapAmount < theCurrentInput_Num) {
                        //    errors += "报关单 <label style=\"color:green\">" + data.rows[i].ContrNo + "</label> 自定义换汇金额<label style=\"color:red\">不能大于" + maxInputSwapAmount + "</label>！<br>"
                        //}
                    }
                }
            }

            if (errors != "") {
                $.messager.alert('提示消息', errors, 'info', function () {

                });
                return;
            }


            //组装固定格式的输入换汇金额
            var currentTheInputAmounts = "";
            var userPayExchangeApplyAmounts = "";
            var data = $('#decheads').datagrid('getData');
            for (var i = 0; i < data.rows.length; i++) {
                if ('appendRow' != data.rows[i].ID) {
                    var theCurrentInput = $("input[decheadid='" + data.rows[i].ID + "']").val();
                    if (theCurrentInput != null && theCurrentInput != "") {
                        currentTheInputAmounts += data.rows[i].ID + '|' + theCurrentInput + ',';
                    } else {
                        currentTheInputAmounts += data.rows[i].ID + '|' + "0" + ',';
                    }

                    userPayExchangeApplyAmounts += data.rows[i].ID + '|' + data.rows[i].UserCurrentPayApply + ',';
                }
            }

            currentTheInputAmounts = (currentTheInputAmounts.substring(currentTheInputAmounts.length - 1) == ',')
                ? currentTheInputAmounts.substring(0, currentTheInputAmounts.length - 1) : currentTheInputAmounts;
            userPayExchangeApplyAmounts = (userPayExchangeApplyAmounts.substring(userPayExchangeApplyAmounts.length - 1) == ',')
                ? userPayExchangeApplyAmounts.substring(0, userPayExchangeApplyAmounts.length - 1) : userPayExchangeApplyAmounts;

            MaskUtil.mask();
            $.post('?action=SubmitApply', {
                BankID: bankID,
                BankName: bankName,
                TheInputAmounts: currentTheInputAmounts,
                UserPayExchangeApplyAmounts: userPayExchangeApplyAmounts,
            }, function (result) {
                MaskUtil.unmask();
                var resultJson = JSON.parse(result);
                if (resultJson.success) {
                    $.messager.alert('消息', resultJson.message, 'info', function () {
                        ewindow.cleanIDs = "";
                        ewindow.bankID = "";
                        ewindow.bankName = "";

                        $.myWindow.close();
                    });
                } else {
                    $.messager.alert('消息', resultJson.message, 'error', function () {

                    });
                }
            });
        }

        //取消
        function Close() {
            var ewindow = $.myWindow.getMyWindow("SwapApply2EditSwapAmount");
            ewindow.cleanIDs = "";
            $.myWindow.close();
        }

        //取消最后一行增加行的序号显示
        function CancelLastRowSerialNoDisplay() {
            $("#edit-swap-amount-content div[class='datagrid-cell-rownumber']:last").html("");
        }

        //显示当前输入的换汇金额总和, 增加显示本次换汇金额总和
        function SetCurrentInputSwapAmountSum(sum) {
            $("#edit-swap-amount-content div[class$='InputSwapAmount']:last").html(sum);

            //显示本次换汇金额总和
            var theBigSum = Number(initUserCurrentPayApplyHeJi) + Number(sum);
            $("#edit-swap-amount-content div[class$='ThisTimeSwapAmount']:last").html(theBigSum.toFixed(2));
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

        //移除一行
        function RemoveOneRow(decHeadId, contrNo) {
            var ewindow = $.myWindow.getMyWindow("SwapApply2EditSwapAmount");

            $.messager.confirm('确认', '确认要移除该报关单（合同号： <label style=\"color:green\">' + contrNo + '</label> ）吗？', function(r){
                if (r){
                    //console.log("decHeadId = " + decHeadId);
                    //得到一个新的 CleanIDs， 更新 ewindow.cleanIDs 后，直接刷新该页面
                    var newCleanIDs = "";

                    var oldCleanIDArray = CleanIDs.split(',');
                    for (var i = 0; i < oldCleanIDArray.length; i++) {
                        if (decHeadId != oldCleanIDArray[i]) {
                            newCleanIDs += oldCleanIDArray[i] + ",";
                        }
                    }

                    newCleanIDs = (newCleanIDs.substring(newCleanIDs.length - 1) == ',')
                        ? newCleanIDs.substring(0, newCleanIDs.length - 1) : newCleanIDs;

                    ewindow.cleanIDs = newCleanIDs;

                    var currentTheInputAmounts = "";
                    var data = $('#decheads').datagrid('getData');
                    for (var i = 0; i < data.rows.length; i++) {
                        if ('appendRow' != data.rows[i].ID) {
                            var theCurrentInput = $("input[decheadid='" + data.rows[i].ID + "']").val();
                            if (theCurrentInput != null && theCurrentInput != "") {
                                currentTheInputAmounts += data.rows[i].ID + '|' + theCurrentInput + ',';
                            } else {
                                currentTheInputAmounts += data.rows[i].ID + '|' + "" + ',';
                            }
                        }
                    }

                    currentTheInputAmounts = (currentTheInputAmounts.substring(currentTheInputAmounts.length - 1) == ',')
                        ? currentTheInputAmounts.substring(0, currentTheInputAmounts.length - 1) : currentTheInputAmounts;

                    var url = location.pathname.replace(/EditSwapAmount.aspx/ig, 'EditSwapAmount.aspx')
                        + "?BankID=" + bankID
                        + "&BankName=" + bankName
                        + "&CleanIDs=" + newCleanIDs
                        + "&TheInputAmounts=" + currentTheInputAmounts
                        + "&Currency=" + Currency;
                    window.location = url;
                }
            });
        }

        //新增若干行
        function AddDecHeads() {
            var ewindow = $.myWindow.getMyWindow("SwapApply2EditSwapAmount");
            
            var newCleanIDs = "";

            var oldCleanIDArray = CleanIDs.split(',');
            for (var i = 0; i < oldCleanIDArray.length; i++) {
                newCleanIDs += oldCleanIDArray[i] + ",";
            }

            var addCleanIDArray = ForAddDecHeadIDs.split(',');
            for (var i = 0; i < addCleanIDArray.length; i++) {
                newCleanIDs += addCleanIDArray[i] + ",";
            }

            newCleanIDs = (newCleanIDs.substring(newCleanIDs.length - 1) == ',')
                ? newCleanIDs.substring(0, newCleanIDs.length - 1) : newCleanIDs;

            ewindow.cleanIDs = newCleanIDs;


            var currentTheInputAmounts = "";
            var data = $('#decheads').datagrid('getData');
            for (var i = 0; i < data.rows.length; i++) {
                if ('appendRow' != data.rows[i].ID) {
                    var theCurrentInput = $("input[decheadid='" + data.rows[i].ID + "']").val();
                    if (theCurrentInput != null && theCurrentInput != "") {
                        currentTheInputAmounts += data.rows[i].ID + '|' + theCurrentInput + ',';
                    } else {
                        currentTheInputAmounts += data.rows[i].ID + '|' + "" + ',';
                    }
                }
            }

            currentTheInputAmounts = (currentTheInputAmounts.substring(currentTheInputAmounts.length - 1) == ',')
                ? currentTheInputAmounts.substring(0, currentTheInputAmounts.length - 1) : currentTheInputAmounts;

            if (currentTheInputAmounts != null || currentTheInputAmounts != 'undefined' || currentTheInputAmounts != '') {
                currentTheInputAmounts += ',';
            }
            currentTheInputAmounts += ForAddTheInputAmounts;

            var url = location.pathname.replace(/EditSwapAmount.aspx/ig, 'EditSwapAmount.aspx')
                + "?BankID=" + bankID
                + "&BankName=" + bankName
                + "&CleanIDs=" + newCleanIDs
                + "&TheInputAmounts=" + currentTheInputAmounts
                + "&Currency=" + Currency;
            window.location = url;
        }

        //输入换汇金额操作
        function InputSwapAmountOperation(val, row, index) {
            if ('appendRow' == row.ID) {
                return '0';
            }

            var inputs = '';

            inputs += '<input decheadid=\'' + row.ID + '\' type="text" class="textbox-text" style="height:18px; width: 100px; margin-top:4px;"><br/>';
            inputs += '<span style="font-size: 8px; color: #999; margin-left: 5px;">最大: ' + row.MaxInputSwapAmount + '</span>';

            return inputs;
        }

        function ThisTimeSwapAmountOperation(val, row, index) {
            var inputs = '';

            inputs += '<span decheadid=\'' + 'ThisTimeSwapAmount_' + row.ID + '\' type="text" class="textbox-text" style="height:18px; width: 100px; margin-top:4px;"></span>';

            return inputs;
        }

        //当前用户当前申请金额列表
        function openUserCurrentPayApplyList(index, orderID, decHeadID, contrNo, entryID) {
            var url = location.pathname.replace(/EditSwapAmount.aspx/ig, 'UserCurrentPayApplyList.aspx')
                + "?OrderID=" + orderID
                + "&ContrNo=" + contrNo
                + "&EntryID=" + entryID;

            $.myWindow({
                iconCls: "",
                noheader: false,
                title: '用户当前申请金额',
                width: '300',
                height: '435',
                url: url,
                onClose: function () {
                    
                }
            });
        }

        //操作
        function Operation(val, row, index) {
            if ('appendRow' == row.ID) {
                return '';
            }

            var buttons = '<a id="btnView" name="btnView" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" data-fileurl="' + row.URL + '" style="margin:3px" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看文件</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="RemoveOneRow(\'' + row.ID  + '\',\'' + row.ContrNo + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">移除</span>' +
                '<span class="l-btn-icon icon-cancel">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }

        //用户本次申请金额操作
        function OperationUserCurrentPayApply(val, row, index) {
            var buttons = '';
            if ('appendRow' == row.ID) {
                buttons += '<span>' + initUserCurrentPayApplyHeJi + '</span>';
            } else {
                buttons += '<a decheadid=\'' + 'UserApplyPayAmount_' + row.ID + '\' href="javascript:void(0);" style="cursor: pointer; color: #6495ed;" '
                    + 'onclick="openUserCurrentPayApplyList(\'' + index + '\',\'' + row.OrderID + '\',\'' + row.ID + '\',\'' + row.ContrNo
                    + '\',\'' + row.EntryID + '\')">' + row.UserCurrentPayApply + '</a>';
            }
            
            return buttons;
        }

        //添加报关单
        function AddDecHead() {
            $.myWindow.setMyWindow("EditSwapAmount2AddDecHead", window);

            var url = location.pathname.replace(/EditSwapAmount.aspx/ig, '/AddDecHead.aspx') + '?CleanIDs=' + CleanIDs;

            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '添加报关单',
                width: 1600,
                height: 600,
                onClose: function () {

                }
            });
        }
    </script>
    <style>
        #edit-swap-amount-content .datagrid-btable tr {
            height: 34px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="edit-swap-amount-content" style="overflow-y:auto; width:1700px; height:481px;">
        <div id="topBar">
            <div id="search" style="margin: 0;">
                <table style="line-height: 30px">   
                    <tr>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="AddDecHead()">添加报关单</a>
                    </tr>
                </table>
            </div>
        </div>
        <div data-options="region:'center',border:false">
            <table id="decheads" data-options="
                nowrap:false,
                border:false,
                fitColumns:true,
                scrollbarSize:0,
                toolbar:'#topBar',
                singleSelect:false,
                rownumbers:true">
                <thead>
                    <tr>
                        <th data-options="field:'ContrNo',align:'center'" style="width: 140px;">合同号</th>
                        <th data-options="field:'EntryID',align:'center'" style="width: 150px;">海关编号</th>
                        <th data-options="field:'OwnerName',align:'left'" style="width: 160px;">客户名称</th>
                        <th data-options="field:'Currency',align:'center'" style="width: 70px;">币种</th>
                        <th data-options="field:'SwapAmount',align:'center'" style="width: 110px;">报关金额</th>
                        <th data-options="field:'SwapedAmount',align:'center'" style="width: 110px;">已换汇金额</th>
                        <th data-options="field:'btn1',width:110,formatter:OperationUserCurrentPayApply,align:'center'">客户本次申请金额</th>
                        <th data-options="field:'InputSwapAmount',formatter:InputSwapAmountOperation,align:'left'" style="width: 130px;">自定义换汇金额</th>
                        <th data-options="field:'ThisTimeSwapAmount',formatter:ThisTimeSwapAmountOperation,align:'center'" style="width: 110px;">本次换汇金额</th>
                        <th data-options="field:'btn',width:160,formatter:Operation,align:'left'">操作</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnNext" class="easyui-linkbutton" data-options="iconCls:'icon-save',width:70," onclick="Save()">保存</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel',width:70," onclick="Close()">取消</a>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 750px; height: 500px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
</body>
</html>
