<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Order.UnCollected.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待收款订单</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js?time=20190910"></script>
    <script type="text/javascript">
        $(function () {
            //下拉框数据初始化
            var orderStatus = eval('(<%=this.Model.OrderStatus%>)');
            var currencies = eval('(<%=this.Model.Currencies%>)');
            $('#OrderStatus').combobox({
                data: orderStatus,
            });
            $('#Currency').combobox({
                data: currencies,
            });

            //隐藏申请付汇按钮
            $('#btnDelete').css('display', 'none');
            $('#note').css('display', 'none');

            //代理订单列表初始化
            $('#orders').myDatagrid({
                fitColumns: true,
                fit: true,
                singleSelect: false,
                queryParams: { ClientType:'<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>', action: "data" },
                nowrap: false,
                onCheck: function (index) {
                    if (!CheckConsignorCode()) {
                        $(this).datagrid('uncheckRow', index);
                        return;
                    }
                    TotalAmount()
                },
                onUncheck: function () {
                    TotalAmount()
                },
                onCheckAll: function () {
                    if (!CheckConsignorCode()) {
                        $(this).datagrid('uncheckAll');
                        return;
                    }
                    TotalAmount()
                },
                onUncheckAll: function (rows) {
                    TotalAmount()
                }
            });
        });

        //查询
        function Search() {
            var orderID = $('#OrderID').textbox('getValue');
            var clientCode = $('#ClientCode').textbox('getValue');
            var currency = $('#Currency').combobox("getValue");
            var orderStatus = $('#OrderStatus').combobox("getValue");
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');

            $('#orders').myDatagrid('search', { OrderID: orderID, ClientCode: clientCode, Currency: currency, OrderStatus: orderStatus, StartDate: startDate, EndDate: endDate, });
        }

        //重置查询条件
        function Reset() {
            $('#OrderID').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#Currency').combobox('setValue', null);
            $('#OrderStatus').combobox("setValue", null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //申请付汇
        function ApplyPay(index) {
            var rows = $('#orders').datagrid('getRows');
            var row = rows[index];
            if (!isAllowPayment(row)) {
                return;
            }

            var url = location.pathname.replace(/List.aspx/ig, '../../PayExchange/Add.aspx') + '?ids=' + row.ID;
            window.location = url;
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SingleReceive(\'' + row.ID + '\',\'' + row.DeclarePrice + '\',\'' + row.Currency + '\',\'' + row.PaidAmount + '\',\'' + row.CollectedAmouont + '\',\'' + row.ClientID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">收款</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        function SingleReceive(orderId, declarePrice, currency, paidAmount, receivedAmount,clientId) {
            var SplitInfo = [];
            SplitInfo.push({
                OrderID: orderId,
                DeclarePrice: declarePrice,
                Currency: currency,
                PaidAmount: paidAmount,
                ReceivedAmount: receivedAmount,
                ClientID:clientId
            });
            MaskUtil.mask();
            $.post('?action=ReceiveCheck', { Model: JSON.stringify(SplitInfo) }, function (data) {
                MaskUtil.unmask();
                var Result = JSON.parse(data);
                $.messager.alert('提示', Result.message);
                Search();
            });
        }

        function batchReceive() {
            var data = $('#orders').myDatagrid('getChecked');
            if (data.length == 0) {
                $.messager.alert('提示', '请勾选要收款订单！');
                return;
            }
            //验证是否同币种
            for (var i = 0; i < data.length; i++) {
                if (data[0].Currency != data[i].Currency) {
                    $.messager.alert('提示', '请勾选币种相同的订单！');
                    return;
                }
            }

            var SplitInfo = [];

            for (var i = 0; i < data.length; i++) {
                SplitInfo.push({
                    OrderID: data[i]["ID"],
                    DeclarePrice: data[i]["DeclarePrice"],
                    Currency: data[i]["Currency"],
                    PaidAmount: data[i]["PaidAmount"],
                    ReceivedAmount: data[i]["CollectedAmouont"],
                    ClientID:data[i]["ClientID"],
                });
            }

            MaskUtil.mask();
            $.post('?action=ReceiveCheck', { Model: JSON.stringify(SplitInfo) }, function (data) {
                MaskUtil.unmask();
                var Result = JSON.parse(data);
                $.messager.alert('提示', Result.message);
                Search();
            });

        }

        //勾选时判断是否为同一个境外发货人
        function CheckConsignorCode() {
            var data = $('#orders').myDatagrid('getChecked');
            for (var i = 0; i < data.length; i++) {
                for (var j = 0; j < data.length; j++) {
                    if (data[i].Currency != data[j].Currency) {
                        //alert(data[i]);
                        $.messager.alert('提示', '请勾选币种相同的订单！');
                        return false;
                    }
                }
            }
            return true;
        }

        //计算换汇总额
        function TotalAmount() {
            var totalAmount = 0;
            var totalUserCurrentPayApply = 0;
            var tobeReceivedAmount = 0;
            var data = $('#orders').myDatagrid('getChecked');
            for (var i = 0; i < data.length; i++) {
                totalAmount = totalAmount + Number(data[i].DeclarePrice);
                tobeReceivedAmount = 0;
                tobeReceivedAmount = Number(data[i].PaidAmount) - Number(data[i].CollectedAmouont)
                totalUserCurrentPayApply = totalUserCurrentPayApply + tobeReceivedAmount;
            }
            $('#SelectCount').text(data.length);
            $('#SwapAmount').text(totalAmount.toFixed(2));
            $('#UserCurrentAllSwapAmount').text(totalUserCurrentPayApply.toFixed(2));
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="BatchApplyPay()">申请付汇</a>
            <span id="note" style="font-style: italic; color: orangered; font-size: 13px">*多个订单一起付汇，需要选择相同客户、交易币种及供应商的订单</span>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox input" id="OrderID" data-options="validType:'length[1,50]'" />
                    <span class="lbl">下单日期: </span>
                    <input class="easyui-datebox input" id="StartDate" data-options="editable:false" />
                    <span class="lbl">至 </span>
                    <input class="easyui-datebox input" id="EndDate" data-options="editable:false" />
                    <br />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox input" id="ClientCode" data-options="validType:'length[1,50]'" />
                    <span class="lbl">交易币种: </span>
                    <input class="easyui-combobox input" id="Currency" data-options="valueField:'Key',textField:'Value'" />
                    <span class="lbl">订单状态: </span>
                    <input class="easyui-combobox input" id="OrderStatus" data-options="valueField:'Key',textField:'Value',editable:false" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton ml10" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    <br />
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Receive()">批量收款</a>
                    <span style="color: red; font-size: 14px; margin-left: 15px;">已选择</span>
                    <label id="SelectCount" style="color: red; font-size: 14px;">0</label>
                    <span style="color: red; font-size: 14px;">份订单，总金额：</span>
                    <label id="SwapAmount" style="color: red; font-size: 14px;">0</label>
                    <span style="color: red; font-size: 14px; margin-left: 20px;">本次收款总金额：</span>
                    <label id="UserCurrentAllSwapAmount" style="color: red; font-size: 14px;">0</label>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="orders" title="待收款" data-options="border:false,nowrap:false,fitColumns:true,fit:true,singleSelect:false,toolbar:'#topBar'">
            <thead data-options="frozen:true">
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 5%">全选</th>
                    <th data-options="field:'ID',align:'left'" style="width: 10%;">订单编号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 6%;">客户编号</th>
                </tr>
            </thead>
            <thead>
                <tr>
                    <th data-options="field:'DeclarePrice',align:'center'" style="width: 6%;">报关总货值</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 5%;">币种</th>
                    <th data-options="field:'PaidAmount',align:'center'" style="width: 6%;">已付汇金额</th>
                    <th data-options="field:'CollectedAmouont',align:'center'" style="width: 6%;">已收款金额</th>
                    <th data-options="field:'OrderStatus',align:'center'" style="width: 7%;">订单状态</th>
                    <th data-options="field:'DeclareDate',align:'center'" style="width: 7%;">报关日期</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
