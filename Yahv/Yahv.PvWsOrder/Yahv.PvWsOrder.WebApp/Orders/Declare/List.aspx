<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Orders.Declare.List" %>

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

                OrderStatus: $.trim($('#OrderStatus').combobox("getValue")),
                OrderPayStatus: $.trim($('#OrderPayStatus').combobox("getValue")),

                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
                IsPayForGoods: isPayForGoods,
                IsReceiveForGoods: isReceiveForGoods,
            };
            return params;
        };
        //编辑
        function Edit(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: "订单编辑",
                minWidth: 1200,
                minHeight: 600,
                url: location.pathname.replace('List.aspx', 'Edit.aspx?ID=' + data.ID+'&EnterCode='+data.EnterCode),
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }
        //退回修改
        function BackEdit(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: "订单编辑",
                minWidth: 1200,
                minHeight: 600,
                url: location.pathname.replace('List.aspx', 'BackEdit.aspx?ID=' + data.ID + '&EnterCode=' + data.EnterCode),
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }
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
                    $.post('?action=Delete', { ID: id }, function () {
                        $.messager.alert('删除', '删除成功！');
                        $('#tab1').datagrid('reload');
                    })
                }
            });
        }
        //操作
        var Draft = <%=Yahv.Underly.CgOrderStatus.暂存.GetHashCode()%>;
        var Reject = <%=Yahv.Underly.CgOrderStatus.退回.GetHashCode()%>;
        function Operation(val, row, index) {
            var buttons = [];
            buttons.push('<span class="easyui-formatted">'); 
            if (row.MainStatus == Draft) {
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="Edit(' + index + ');return false;">编辑</a> ')
            }
            if (row.MainStatus == Reject) {
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="BackEdit(' + index + ');return false;">退回修改</a> ')
            }
            buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="Details(\'' + row.ID + '\');return false;">详情</a> ')
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
                <td>订单号</td>
                <td>
                    <input id="OrderID" class="easyui-textbox" style="width: 150px" />
                </td>
                <td>客户名称</td>
                <td>
                    <input id="CompanyName" class="easyui-textbox" style="width: 150px" />
                </td>
                <td>客户入仓号</td>
                <td>
                    <input id="ClientCode" class="easyui-textbox" style="width: 150px" />
                </td>
                <td>创建日期</td>
                <td>
                    <input id="StartDate" data-options="prompt:'开始日期'" class="easyui-datebox" style="width: 108px" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="EndDate" data-options="prompt:'结束日期'" class="easyui-datebox" style="width: 108px" />
                </td>
            </tr>
            <tr>
                <td>订单状态</td>
                <td>
                    <input id="OrderStatus" class="easyui-combobox" style="width: 150px" />
                </td>
                <td>支付状态</td>
                <td>
                    <input id="OrderPayStatus" class="easyui-combobox" style="width: 150px" />
                </td>
                <td>其它条件</td>
                <td colspan="3">
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
    <table id="tab1" title="代报关订单">
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
