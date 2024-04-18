<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Orders.Delivery.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                singleSelect: true,
                fitColumns: false,
                fit: true,
                nowrap: false,
                scrollbarSize: 0,
            });

            $('#OrderType').combobox({
                data: model.OrderType,
                editable: false,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
            });
            $('#OrderStatus').combobox({
                data: model.CgOrderStatus,
                editable: false,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            });
            $('#OrderPayStatus').combobox({
                data: model.OrderPaymentStatus,
                editable: false,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            });
            //是否代收货款
            $("#isReceiveForGoods").checkbox({
                onChange: function (record) {
                    grid.myDatagrid('search', getQuery());
                }
            })
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                $('#OrderID').textbox("setValue", "")
                $('#CompanyName').textbox("setValue", "")
                $('#ClientCode').textbox("setValue", "")
                $('#OrderStatus').combobox("setValue", "")
                $('#OrderPayStatus').combobox("setValue", "")
                $('#StartDate').datebox("setValue", "")
                $('#EndDate').datebox("setValue", "")
                $('#isReceiveForGoods').checkbox('uncheck')
                grid.myDatagrid('search', getQuery());
                return false;
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var isReceiveForGoods = $('#isReceiveForGoods').checkbox('options').checked;
            var params = {
                action: 'data',
                OrderID: $.trim($('#OrderID').textbox("getText")),
                CompanyName: $.trim($('#CompanyName').textbox("getText")),
                ClientCode: $.trim($('#ClientCode').textbox("getText")),

                OrderStatus: $.trim($('#OrderStatus').combobox("getValue")),
                OrderPayStatus: $.trim($('#OrderPayStatus').combobox("getValue")),

                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
                IsReceiveForGoods: isReceiveForGoods,
            };
            return params;
        };
        //详情
        function Details(id) {
            $.myWindow({
                title: "订单详情",
                minWidth: 1200,
                minHeight: 600,
                url: location.pathname.replace('List.aspx', 'Detail.aspx?ID=' + id),
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }
        //删除
        function Delete(id) {
            $.messager.confirm('确认', '请您确认是否删除所选订单！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function (res) {
                        var res = JSON.parse(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }

                        $('#tab1').datagrid('reload');
                    })
                }
            });
        }
        //账单确认
        function ConfirmBill(id) {
            $.myWindow({
                title: "账单确认",
                minWidth: 1200,
                minHeight: 600,
                url: location.pathname.replace('List.aspx', '../Common/ConfirmBillNew.aspx?ID=' + id),
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }
        //跳转至财务收款页面
        function Received(id) {
            $.myWindow({
                title: "收款核销",
                minWidth: 1200,
                minHeight: 600,
                url: location.pathname.replace('List.aspx', '../Common/WriteOffReceive.aspx?ID=' + id),
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }
        //操作
        function Operation(val, row, index) {
            var buttons = [];
            buttons.push('<span class="easyui-formatted">');
            //账单
            if (orderConfirmBill(row.PayStatus)) {
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-confirm\'" onclick="ConfirmBill(\'' + row.ID + '\');return false;">账单确认</a> ')
            }
            //收款
            if (orderReceived(row.PayStatus)) {
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-sealing\'" onclick="Received(\'' + row.ID + '\');return false;">收款核销</a> ')
            }
            //详情
            buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="Details(\'' + row.ID + '\');return false;">详情</a> ')
            //撤销
            if (orderDeleteable(row.OrderStatus)) {
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-cancel\'" onclick="Delete(\'' + row.ID + '\');return false;">撤销</a> ')
            }
            buttons.push('</span>')
            return buttons.join('');
        }
    </script>
    <script>
        //账单确认
        var ConfirmConditions = [<%=Yahv.Underly.OrderPaymentStatus.Confirm.GetHashCode()%>,<%=Yahv.Underly.OrderPaymentStatus.ToBePaid.GetHashCode()%>,<%=Yahv.Underly.OrderPaymentStatus.PartPaid.GetHashCode()%>,<%=Yahv.Underly.OrderPaymentStatus.Paid.GetHashCode()%>];
        //收款
        var ReceivedCondition = [<%=Yahv.Underly.OrderPaymentStatus.ToBePaid.GetHashCode()%>,<%=Yahv.Underly.OrderPaymentStatus.PartPaid.GetHashCode()%>];
        //撤销
        var deleteCondition = [<%=Yahv.Underly.CgOrderStatus.已提交.GetHashCode()%>,<%=Yahv.Underly.CgOrderStatus.待收货.GetHashCode()%>];

        //撤销
        function orderDeleteable(orderStatus) {
            var index = $.inArray(orderStatus, deleteCondition);
            var result = index >= 0 ? true : false;
            return result;
        }
        //确认账单
        function orderConfirmBill(payStatus) {
            var index = $.inArray(payStatus, ConfirmConditions);
            var result = index >= 0 ? true : false;
            return result;
        }
        //收款
        function orderReceived(payStatus) {
            var index = $.inArray(payStatus, ReceivedCondition);
            var result = index >= 0 ? true : false;
            return result;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--查询条件-->
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">订单号</td>
                <td style="width: 200px;">
                    <input id="OrderID" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">客户名称</td>
                <td style="width: 200px;">
                    <input id="CompanyName" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">客户入仓号</td>
                <td style="width: 200px;">
                    <input id="ClientCode" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">创建日期</td>
                <td>
                    <input id="StartDate" data-options="prompt:'开始日期'" class="easyui-datebox" style="width: 108px" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="EndDate" data-options="prompt:'结束日期'" class="easyui-datebox" style="width: 108px" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">订单状态</td>
                <td style="width: 200px;">
                    <input id="OrderStatus" class="easyui-combobox" style="width: 150px" />
                </td>
                <td style="width: 90px;">支付状态</td>
                <td style="width: 200px;">
                    <input id="OrderPayStatus" class="easyui-combobox" style="width: 150px" />
                </td>
                <td style="width: 90px;">其它条件</td>
                <td colspan="3">
                    <input id="isReceiveForGoods" name="isReceiveForGoods" class="easyui-checkbox" value="true"
                        data-options="label:'是否代收货款',labelPosition:'after'">
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="代发货订单">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">下单时间</th>
                <th data-options="field:'ID',align:'center'" style="width: 150px">订单号</th>
                <th data-options="field:'CompanyName',align:'left'" style="width: 250px">客户名称</th>
                <th data-options="field:'EnterCode',align:'center'" style="width: 80px">客户入仓号</th>
                <th data-options="field:'ConsigneeCompany',align:'left'" style="width: 150px">收货人</th>
                <th data-options="field:'DeliveryType',align:'center'" style="width: 80px">交货方式</th>
                <th data-options="field:'IsReciveCharge',align:'center'" style="width: 80px">代收货款</th>
                <th data-options="field:'OrderStatus',align:'center'" style="width: 80px">订单状态</th>
                <th data-options="field:'OrderPayStatus',align:'center'" style="width: 80px">支付状态</th>
                <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 300px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
