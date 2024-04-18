<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StoreReceiptDialog.aspx.cs" Inherits="WebApp.Finance.Receipt.Store.StoreReceiptDialog" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>代仓储收款弹框</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var NoticeData = eval('(<%=this.Model.NoticeData%>)');
        var MainOrderID = '<%=this.Model.MainOrderID%>';
        var FinanceReceiptId = '<%=this.Model.FinanceReceiptId%>';

        var daiShouAll = (Number(NoticeData.Amount) - Number(NoticeData.ClearAmount)).toFixed(2);
        var yaoshouAll = 0;

        $(function () {

            $("#ClienName-receipt-datagrid").text(NoticeData.ClienName);
            $("#Amount-receipt-datagrid").text(NoticeData.Amount);
            $("#ShengyuAmount-receipt-datagrid").text((Number(NoticeData.Amount) - Number(NoticeData.ClearAmount)).toFixed(2));

            //收款弹框中的表格
            $('#receipt-datagrid').myDatagrid({
                actionName: 'StoreReceivableOrderReceipt',
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
                rownumbers: false,
                pagination: false,

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
                    if (0 < Number(thisHangYaoShou)) {
                        $("input[ReceivableID='" + rowData.ReceivableID + "']").val(thisHangYaoShou);
                        yaoshouAll = (Number(yaoshouAll) + Number(thisHangYaoShou)).toFixed(2);
                        $("#yaoshouBox").html(yaoshouAll);
                    }


                    //清除掉 空输入的行的选中
                    var thisHangShuRu = $("input[ReceivableID='" + rowData.ReceivableID + "']").val();
                    if (thisHangShuRu == null || thisHangShuRu == "") {
                        $('#receipt-datagrid').datagrid('uncheckRow', rowIndex);
                    }
                },
                //取消选中一行
                onUncheck: function (rowIndex, rowData) {
                    var thisHangYaoShou = 0;
                    var $input = $("input[ReceivableID='" + rowData.ReceivableID + "']");
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
                            thisHangShuRu = $("input[ReceivableID='" + rows[i].ReceivableID + "']").val();

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

                                $("input[ReceivableID='" + rows[i].ReceivableID + "']").val(thisHangYaoShou);
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
                        var thisHangShuRu = $("input[ReceivableID='" + rows[i].ReceivableID + "']").val();
                        if (thisHangShuRu == null || thisHangShuRu == "") {
                            $('#receipt-datagrid').datagrid('uncheckRow', i);
                        }
                    }
                },
                //全部不选
                onUncheckAll: function (rows) {
                    for (var i = 0; i < rows.length; i++) {
                        var thisHangYaoShou = 0;
                        var $input = $("input[ReceivableID='" + rows[i].ReceivableID + "']");
                        thisHangYaoShou = $input.val();
                        $input.val("");

                        yaoshouAll = (Number(yaoshouAll) - Number(thisHangYaoShou)).toFixed(2);
                    }
                    $("#yaoshouBox").html(yaoshouAll);
                },




                loadFilter: function (data) {
                    if (data.rows.length > 0) {
                        $("#OrderId-receipt-datagrid").text(data.rows[0].MainOrderID);
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
                    var currentReceivableID = rowData.ReceivableID;
                    var currentIsSelectByThisClick = 0;
                    var allSelections = $(this).datagrid('getSelections');
                    for (var i = 0; i < allSelections.length; i++) {
                        if (allSelections[i].ReceivableID == currentReceivableID) {
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
                        $("input[ReceivableID='" + rows[i].ReceivableID + "']").each(function (j) {
                            $(this).keyup(function () {

                                //清除没有打勾的输入框 Begin
                                for (var i = 0; i < rows.length; i++) {
                                    var isDagou = 0;
                                    var checkedItems = $('#receipt-datagrid').datagrid('getChecked');
                                    $.each(checkedItems, function (index, item) {
                                        if (rows[i].ReceivableID == item.ReceivableID) {
                                            isDagou = 1;
                                        }
                                    });

                                    if (0 == isDagou) {
                                        $("input[ReceivableID='" + rows[i].ReceivableID + "']").val("");
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
                                    var thisHangInputValue = $("input[ReceivableID='" + rows[i].ReceivableID + "']").val();
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


        });




        //收款弹框 查看 按钮
        function ReceiptOperation(val, row, index) {
            var buttons = "";
            if (!row.IsFooter) {
                buttons += '<a href="javascript:void(0);" style="margin:3px; color: #134da5;" onclick="ShowReduceWindow(\''
                    + row.ReceivableID + '\', \'' + row.FeeTypeShowName + '\', \'' + row.ReceivableAmount + '\')">减免</a>';
                buttons += '<a href="javascript:void(0);" style="margin:3px; color: #134da5; margin-left:15px;" onclick="ShowReceiptRecord(\'' + row.ReceivableID + '\')">收款记录</a>';
            }
            return buttons;
        }

        //根据 ReceivableID 查看收款弹框
        function ShowReceiptRecord(receivableID) {
            var url = location.pathname.replace(/StoreReceiptDialog.aspx/ig, 'LittleReceivedList.aspx')
                + '?ReceivableID=' + receivableID;

            top.$.myWindow({
                iconCls: "icon-search",
                url: url,
                noheader: false,
                title: '收款记录',
                width: '600px',
                height: '350px',
                onClose: function () {
                    $('#receipt-datagrid').datagrid('reload');
                }
            });
        }

        //收款弹框 实收 输入框
        function ReceiptReceiveInput(val, row, index) {
            var inputs = "";
            if (!row.IsFooter) {
                inputs = '<input ReceivableID=' + row.ReceivableID + ' type="text" class="textbox-text" style="height:24px; border-radius:5px;">';
            } else {
                inputs = '<span class="datagrid-cell" style="text-align:center; height:32px;" id="yaoshouBox">0</span>';
            }
            return inputs;
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

        //提交
        function Submit() {
            if (Number(yaoshouAll) > Number(daiShouAll)) {
                $.messager.alert('提示','实收总金额不能大于待收总金额！','info');
                return;
            }

            var submitData = [];

            var isReturn = 0;
            var infoMsg = "";

            var checkedItems = $('#receipt-datagrid').datagrid('getChecked');
            $.each(checkedItems, function (index, item) {
                var receiveAmount = 0;
                receiveAmount = $("input[ReceivableID='" + item.ReceivableID + "']").val();

                if (receiveAmount != null && receiveAmount.trim() != "" && receiveAmount.trim() != "0") {

                    ////做一个判断, 限制大于应收款的提交 Begin
                    //if (Number(receiveAmount) > Number(item.ReceivableAmount)) {
                    //    infoMsg = infoMsg + item.FeeTypeShowName + "、";
                    //    isReturn = 1;
                    //}
                    ////做一个判断, 限制大于应收款的提交 End

                    submitData.push({
                        //"FeeTypeValue": item.FeeTypeValue,
                        "ReceivableAmount": item.ReceivableAmount,
                        "ReceivableID": item.ReceivableID,
                        "ReceiveAmount": receiveAmount,
                        "MainOrderID": item.MainOrderID,                      
                        "ApplicationID": item.ApplicationID,
                    });
                }
            }); 

            if (1 == isReturn) {
                infoMsg = infoMsg.slice(0,infoMsg.length-1);
                infoMsg = infoMsg + ' 实收金额不能大于应收金额！';
                $.messager.alert('提示', infoMsg, 'info');
                return;
            }


            //提交
            var url = location.pathname + '?action=Submit' + "&FinanceReceiptId=" + FinanceReceiptId;
            var params = {
                "ReceiveData": JSON.stringify(submitData),
                "ClienName": NoticeData.ClienName,
            };

            MaskUtil.mask();
            //$("div[class*=window-mask]").css('z-index', '9005');
            $("#mask").show();
            $.post(url, params, function (res) {
                //$(".window-mask").css("z-index", "9006");
                MaskUtil.unmask();
                $("#mask").hide();
                var resData = JSON.parse(res);
                if (resData.success == "true") {
                    $.messager.alert('提示', '收款成功', 'info', function () {
                        $.myWindow.close();
                    });

                    //$.myWindow.close();
                    //$("#ClearAmount").text(resData.clearAmount);

                    //daiShouAll = (Number(NoticeData.Amount) - Number(resData.clearAmount)).toFixed(2); //更新"总共待收金额"
                    //更新收款弹框中显示的"总共待收金额"
                    //$("#ShengyuAmount-receipt-datagrid").text(daiShouAll);
                    console.log("收款成功，当前待收总金额变为：" + daiShouAll);
                } else {
                    $.messager.alert('提示', resData.message);
                }
            });
        }

        //取消
        function Cancel() {
            $.myWindow.close();
        }

        //显示减免弹框
        function ShowReduceWindow(ReceivableID, FeeTypeShowName, ReceivableAmount) {
            var url = location.pathname.replace(/StoreReceiptDialog.aspx/ig, 'ReduceWindow.aspx')
                + '?ReceivableID=' + ReceivableID
                + '&FeeTypeShowName=' + FeeTypeShowName
                + '&ReceivableAmount=' + ReceivableAmount;

            top.$.myWindow({
                iconCls: "icon-edit",
                url: url,
                noheader: false,
                title: FeeTypeShowName + '减免',
                width: '400px',
                height: '210px',
                onClose: function () {
                    $('#receipt-datagrid').datagrid('reload');
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar-receipt-datagrid">
        <div style="padding-bottom: 5px;">
            <div style="margin-top:5px;">
                <div class="divstyle">
                    <span style="font-size: 14px">订单编号：</span>
                    <label id="OrderId-receipt-datagrid" style="font-size: 16px"></label>
                </div>
            </div>
            <div style="margin-top:5px">
                <div class="divstyle">
                    <span style="font-size: 14px">客户名称：</span>
                    <label id="ClienName-receipt-datagrid" style="font-size: 16px"></label>
                </div>
                <div class="divstyle"">
                    <span style="font-size: 14px">付款金额：</span>
                    <label id="Amount-receipt-datagrid" style="font-size: 16px"></label>
                </div>
                <div class="divstyle"">
                    <span style="font-size: 14px">待收金额：</span>
                    <label id="ShengyuAmount-receipt-datagrid" style="font-size: 16px"></label>
                </div>
            </div>
        </div>
    </div>

    <div data-options="region:'north',border:false," style="height: 410px;">
        <table id="receipt-datagrid" data-options="">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true," style="width: 10px;"></th>
                    <th data-options="field:'FeeTypeShowName',align:'center'" style="width: 16px;">类型</th>
                    <th data-options="field:'ReceivableAmount',align:'center'" style="width: 12px;">应收</th>
                    <th data-options="field:'Input',align:'center',formatter:ReceiptReceiveInput" style="width: 14px;">实收</th>
                    <th data-options="field:'Btn',align:'center',formatter:ReceiptOperation" style="width: 10px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
    <div data-options="region:'south',border:false," style="padding:5px; text-align:right; background-color:#fafafa;">
        <a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Submit()">提交</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Cancel()">取消</a>
    </div>

    <div id="mask" class="window-mask" style="display: none; z-index: 9000; position: fixed;"></div>

</body>
</html>
