<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SwapedNotice.aspx.cs" Inherits="WebApp.Finance.Declare.SwapedNotice" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        //gvSettings.fatherMenu = '报关单';
        //gvSettings.menu = '已换汇申请';
        //gvSettings.summary = '';
    </script>
    <script>
        $(function () {
            $('#swapedNotice').myDatagrid({
                border: false,
                fitColumns:true,
                fit:true,
                scrollbarSize:0,
                singleSelect:false,
                nowrap:false,
                checkOnSelect: false,
                selectOnCheck: false,
                //queryParams: { action: 'SwapedNoticeData', },
                actionName: 'SwapedNoticeData',
                singleSelect: false,
                onCheck: function () {
                    TotalAmount()
                },
                onUncheck: function () {
                    TotalAmount()
                },
                onCheckAll: function () {
                    TotalAmount()
                },
                onUncheckAll: function (rows) {
                    TotalAmount()
                },
                onLoadSuccess: function (data) {
                    
                },
                pageSize: 20,
            });

        });

        //查询
        function Search() {
            var OrderID = $('#OrderID').textbox('getValue');
            var ClientCode = $('#ClientCode').textbox('getValue');
            var OwnerName = $('#OwnerName').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');

            var parm = {
                OrderID: OrderID,
                ClientCode: ClientCode,
                OwnerName: OwnerName,
                StartDate: StartDate,
                EndDate: EndDate,
            };
            $('#swapedNotice').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#OrderID').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#OwnerName').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);

            Search();
        }

        //操作
        function Operation(val, row, index) {
            var buttons = '';
            if (row.HandleStatusInt == '<%=Needs.Ccs.Services.Enums.SwapedNoticeHandleStatus.UnHandle.GetHashCode()%>') {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SingleHandle(\''
                    + row.PayExchangeSwapedNoticeID + '\',\'' + row.ContrNo + '\',\'' + row.OrderID + '\',\'' + row.UnHandleAmount + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">处理</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            return buttons;
        }

        //单个处理
        function SingleHandle(PayExchangeSwapedNoticeID, ContrNo, OrderID, UnHandleAmount) {
            var msg = '确定处理完了该项吗？<br/>合同号： ' + ContrNo + ' , <br/>订单号： ' + OrderID + ' , <br/>' + '需补充换汇金额： ' + UnHandleAmount;

            $("#confirm-content").html(msg);

            $('#handle-confirm-dialog').dialog({
                title: '提示',
                width: 350,
                height: 180,
                closed: false,
                //cache: false,
                modal: true,
                closable: true,
                buttons: [{
                    //id: '',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=SingleHandle', {
                            PayExchangeSwapedNoticeID: PayExchangeSwapedNoticeID,
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    $('#handle-confirm-dialog').window('close');
                                    Search();
                                });
                                alert1.window({
                                    modal: true, onBeforeClose: function () {
                                        $('#handle-confirm-dialog').window('close');
                                        Search();
                                    }
                                });
                            } else {
                                $.messager.alert('提示', result.message, 'info', function () {

                                });
                            }
                        });

                    }
                }, {
                    //id: '',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#handle-confirm-dialog').window('close');
                    }
                }],
            });

            $('#handle-confirm-dialog').window('center');

        }

        //批量处理
        function BatchHandle() {
            var data = $('#swapedNotice').myDatagrid('getChecked');
            if (data.length == 0) {
                $.messager.alert('提示', '请勾选要处理的项！');
                return;
            }

            var PayExchangeSwapedNoticeIDs = "";
            for (var i = 0; i < data.length; i++) {
                PayExchangeSwapedNoticeIDs += data[i].PayExchangeSwapedNoticeID + ",";
            }
            PayExchangeSwapedNoticeIDs = PayExchangeSwapedNoticeIDs.substr(0, PayExchangeSwapedNoticeIDs.length - 1);

            var msg = "确定要处理这" + data.length + "项吗？";

            $("#confirm-content").html(msg);

            $('#handle-confirm-dialog').dialog({
                title: '提示',
                width: 350,
                height: 180,
                closed: false,
                //cache: false,
                modal: true,
                closable: true,
                buttons: [{
                    //id: '',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=BatchHandle', {
                            PayExchangeSwapedNoticeIDs: PayExchangeSwapedNoticeIDs,
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    $('#handle-confirm-dialog').window('close');
                                    Search();
                                });
                                alert1.window({
                                    modal: true, onBeforeClose: function () {
                                        $('#handle-confirm-dialog').window('close');
                                        Search();
                                    }
                                });
                            } else {
                                $.messager.alert('提示', result.message, 'info', function () {

                                });
                            }
                        });

                    }
                }, {
                    //id: '',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#handle-confirm-dialog').window('close');
                    }
                }],
            });

            $('#handle-confirm-dialog').window('center');

        }

        //计算换汇总额
        function TotalAmount() {
            var totalAmount = 0;

            var data = $('#swapedNotice').myDatagrid('getChecked');
            for (var i = 0; i < data.length; i++) {
                totalAmount = totalAmount + Number(data[i].UnHandleAmount);
            }
            $('#SelectCount').text(data.length);
            $('#SelectAllUnHandleAmount').text(totalAmount.toFixed(2));
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">订单编号: </td>
                    <td>
                        <input class="easyui-textbox" id="OrderID" data-options="validType:'length[1,50]'" />
                    </td>
                    <td class="lbl" style="padding-left: 5px">客户编号: </td>
                    <td>
                        <input class="easyui-textbox" id="ClientCode" data-options="validType:'length[1,50]'" />
                    </td>
                    <td class="lbl" style="padding-left: 5px">客户名称: </td>
                    <td>
                        <input class="easyui-textbox" id="OwnerName" data-options="validType:'length[1,50]'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">申请时间: </td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="editable:false" />
                    </td>
                    <td class="lbl" style="padding-left: 5px">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="editable:false" />
                    </td>
                    <td colspan="2" style="padding-left: 5px">
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>                       
                    </td>
                </tr>
                <tr>
                    <td colspan="4" class="lbl">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="BatchHandle()">批量处理</a>
                        <span style="color: red; font-size: 14px; margin-left: 15px;">已选择</span>
                        <label id="SelectCount" style="color: red; font-size: 14px;">0</label>
                        <span style="color: red; font-size: 14px;">项，总的需补充换汇金额：</span>
                        <label id="SelectAllUnHandleAmount" style="color: red; font-size: 14px;">0</label>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="swapedNotice" title="客户申请付汇的报关单中，已被用于换汇的金额" data-options="toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'ContrNo',align:'center'" style="width: 100px;">合同号</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 100px;">客户编号</th>
                    <th data-options="field:'OwnerName',align:'left'" style="width: 120px;">客户名称</th>
                    <th data-options="field:'OrderID',align:'center'" style="width: 100px;">订单编号</th>
                    <th data-options="field:'UnHandleAmount',align:'center'" style="width: 60px;">需补充换汇金额</th>
                    <th data-options="field:'ApplyDate',align:'center'" style="width: 90px;">申请时间</th>
                    <th data-options="field:'HandleStatusDes',align:'center'" style="width: 60px;">处理状态</th>
                    <th data-options="field:'btn',width:100,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>

    <div id="handle-confirm-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <form>
            <div id="confirm-content" style="padding: 15px;">

            </div>
        </form>
    </div>

</body>
</html>
