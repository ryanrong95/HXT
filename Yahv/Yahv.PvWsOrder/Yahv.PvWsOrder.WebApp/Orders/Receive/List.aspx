<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Orders.Recieve.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
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
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                singleSelect: true,
                nowrap: false,
                fitColumns: false,
                fit: true,
            });
            //是否代付货款
            $("#isPayForGoods").checkbox({
                onChange: function (record) {
                    grid.myDatagrid('search', getQuery());
                }
            })
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
                $('#Supplier').textbox("setValue", "")
                $('#OrderStatus').combobox("setValue", "")
                $('#OrderPayStatus').combobox("setValue", "")
                $('#StartDate').datebox("setValue", "")
                $('#EndDate').datebox("setValue", "")
                $("#isPayForGoods").checkbox('uncheck')
                $('#isReceiveForGoods').checkbox('uncheck')

                grid.myDatagrid('search', getQuery());
                return false;
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var isPayForGoods = $("#isPayForGoods").checkbox('options').checked;
            var isReceiveForGoods = $('#isReceiveForGoods').checkbox('options').checked;
            var params = {
                action: 'data',
                OrderID: $.trim($('#OrderID').textbox("getText")),
                CompanyName: $.trim($('#CompanyName').textbox("getText")),
                ClientCode: $.trim($('#ClientCode').textbox("getText")),
                Supplier: $.trim($('#Supplier').textbox("getText")),

                OrderStatus: $.trim($('#OrderStatus').combobox("getValue")),
                OrderPayStatus: $.trim($('#OrderPayStatus').combobox("getValue")),

                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
                IsPayForGoods: isPayForGoods,
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
        //编辑
        function Edit(id) {
            $.myWindow({
                title: "编辑订单",
                minWidth: 1200,
                minHeight: 600,
                url: location.pathname.replace('List.aspx', 'Edit.aspx?ID=' + id),
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
                    $.post('?action=Delete', { ID: id }, function (result) {
                        var rel = JSON.parse(result);
                        $.messager.alert('提示', rel.message);
                        $('#tab1').datagrid('reload');
                    })
                }
            });
        }
        //分拣异常确认
        function ConfirmSorting(id) {
            $.myWindow({
                title: "异常修改",
                minWidth: 1200,
                minHeight: 600,
                url: location.pathname.replace('List.aspx', '../Common/ConfirmSorting.aspx?ID=' + id),
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }
        //驳回
        function Rejected(id) {
            $.messager.confirm('确认', '请您确认是否驳回此订单！', function (success) {
                if (success) {
                    $.post('?action=Reject', { ID: id }, function (result) {
                        var rel = JSON.parse(result);
                        $.messager.alert('提示', rel.message);
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
        //收款确认（财务界面）
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

        var ToBeSubmited = <%=Yahv.Underly.CgOrderStatus.暂存.GetHashCode()%>;
        var HangUp = <%=Yahv.Underly.CgOrderStatus.挂起.GetHashCode()%>;
        var Submited = <%=Yahv.Underly.CgOrderStatus.已提交.GetHashCode()%>;
        var ToBeReceive = <%=Yahv.Underly.CgOrderStatus.待交货.GetHashCode()%>;

        var Confirm = <%=Yahv.Underly.OrderPaymentStatus.Confirm.GetHashCode()%>;
        var ToBePaid = <%=Yahv.Underly.OrderPaymentStatus.ToBePaid.GetHashCode()%>;
        var PartPaid = <%=Yahv.Underly.OrderPaymentStatus.PartPaid.GetHashCode()%>;

        var Anomalous = <%=Yahv.Underly.SortingExcuteStatus.Anomalous.GetHashCode()%>;

        //操作
        function Operation(val, row, index) {
            var mainStatus = row.MainStatus;
            var payStatus = row.PayStatus;
            var buttons = [];
            buttons.push('<span class="easyui-formatted">');
            if (mainStatus == ToBeSubmited || mainStatus == HangUp || mainStatus == Submited) {
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="Edit(\'' + row.ID + '\');return false;">编辑</a> ');
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="Details(\'' + row.ID + '\');return false;">详情</a> ');
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + row.ID + '\');return false;">删除</a> ');
            }
            else if(mainStatus == ToBeReceive)
            {
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-edit\'" onclick="ConfirmSorting(\'' + row.ID + '\');return false;">异常修改</a> ');
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-cancel\'" onclick="Rejected(\'' + row.ID + '\');return false;">退回</a> ');
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="Details(\'' + row.ID + '\');return false;">详情</a> ');
            }
            else {
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-confirm\'" onclick="ConfirmBill(\'' + row.ID + '\');return false;">账单确认</a> ');
                if (payStatus == ToBePaid || payStatus == PartPaid) {
                    buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-sealing\'" onclick="Received(\'' + row.ID + '\');return false;">收款核销</a> ');
                }
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="Details(\'' + row.ID + '\');return false;">详情</a> ');
            }
            buttons.push('</span>')
            return buttons.join('');
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
                <td style="width: 90px;">供应商</td>
                <td style="width: 200px;">
                    <input id="Supplier" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">订单状态</td>
                <td style="width: 200px;">
                    <input id="OrderStatus" class="easyui-combobox" style="width: 150px" />
                </td>
                <td style="width: 90px;">支付状态</td>
                <td style="width: 200px;">
                    <input id="OrderPayStatus" class="easyui-combobox" style="width: 150px" />
                </td>
                <td style="width: 90px;">其它条件</td>
                <td>
                    <input id="isPayForGoods" name="isPayForGoods" class="easyui-checkbox" value="true"
                        data-options="label:'是否代付货款',labelPosition:'after'">&nbsp&nbsp&nbsp&nbsp
                    <input id="isReceiveForGoods" name="isPayForGoods" class="easyui-checkbox" value="true"
                        data-options="label:'是否代收货款',labelPosition:'after',disabled:true">
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
    <table id="tab1" title="代收货订单查询" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">下单时间</th>
                <th data-options="field:'ID',align:'center'" style="width: 150px">订单号</th>
                <th data-options="field:'CompanyName',align:'left'" style="width: 250px">客户名称</th>
                <th data-options="field:'EnterCode',align:'center'" style="width: 80px">客户入仓号</th>
                <th data-options="field:'Supplier',align:'left'" style="width: 250px;">供应商</th>
                <th data-options="field:'Type',align:'center'" style="width: 80px;">交货方式</th>
                <th data-options="field:'LoadingExcuteStatus',align:'center'" style="width: 80px">提货状态</th>
                <th data-options="field:'IsPayForGoods',align:'center'" style="width: 80px">代付货款</th>
                <th data-options="field:'OrderStatus',align:'center'" style="width: 80px">订单状态</th>
                <th data-options="field:'OrderPayStatus',align:'center'" style="width: 80px">支付状态</th>
                <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 350px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
