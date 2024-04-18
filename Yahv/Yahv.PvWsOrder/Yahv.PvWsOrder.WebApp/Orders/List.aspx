<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Orders.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {

            $('#OrderType').combobox({
                data: model.OrderType,
                editable: false,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
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
                $('#OrderType').combobox("setValue", "")
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
                OrderType: $.trim($('#OrderType').combobox("getValue")),
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
        function Details(id, orderType) {
            var loca = "";
            if (orderType == "仓储收货") {
                loca = 'Receive/Detail.aspx?ID=' + id;
            }
            else if (orderType == "即收即发") {
                loca = 'Transport/Detail.aspx?ID=' + id;
            }
            else if (orderType == "代发货") {
                loca = 'Delivery/Detail.aspx?ID=' + id;
            }
            else if (orderType == "转报关") {
                loca = 'TurnDeclare/Detail.aspx?ID=' + id;
            }
            else {
                loca = 'Declare/Detail.aspx?ID=' + id;
            }
            $.myWindow({
                title: "订单详情",
                minWidth: 1200,
                minHeight: 600,
                url: location.pathname.replace('List.aspx', loca),
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
            buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="Details(\'' + row.ID + '\',\'' + row.OrderType + '\');return false;">详情</a> ');
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
                <td>供应商</td>
                <td>
                    <input id="Supplier" class="easyui-textbox" style="width: 150px" />
                </td>
            </tr>
            <tr>
                <td>订单类型</td>
                <td>
                    <input id="OrderType" class="easyui-combobox" style="width: 150px" />
                </td>
                <td>订单状态</td>
                <td>
                    <input id="OrderStatus" class="easyui-combobox" style="width: 150px" />
                </td>
                <td>支付状态</td>
                <td>
                    <input id="OrderPayStatus" class="easyui-combobox" style="width: 150px" />
                </td>
                <td>下单日期</td>
                <td>
                    <input id="StartDate" data-options="prompt:'开始日期'" class="easyui-datebox" style="width: 150px" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="EndDate" data-options="prompt:'结束日期'" class="easyui-datebox" style="width: 150px" />
                </td>
            </tr>
            <tr>
                <td>其它条件</td>
                <td colspan="8">
                    <input id="isPayForGoods" name="isPayForGoods" class="easyui-checkbox" value="false"
                        data-options="label:'是否代付货款',labelPosition:'after'">&nbsp&nbsp&nbsp&nbsp
                    <input id="isReceiveForGoods" name="isPayForGoods" class="easyui-checkbox" value="false"
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
    <table id="tab1" title="订单查询" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px;">操作</th>
                <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">下单日期</th>
                <th data-options="field:'ID',align:'center'" style="width: 150px">订单号</th>
                <th data-options="field:'OrderType',align:'center'" style="width: 100px;">订单类型</th>
                <th data-options="field:'EnterCode',align:'center'" style="width: 80px">客户入仓号</th>
                <th data-options="field:'CompanyName',align:'left'" style="width: 250px">客户名称</th>
                <th data-options="field:'Supplier',align:'left'" style="width: 250px;">供应商</th>
                <th data-options="field:'Type',align:'center'" style="width: 80px;">交货方式</th>
                <th data-options="field:'DeliveryType',align:'center'" style="width: 80px;">发货方式</th>
                <th data-options="field:'IsPayForGoods',align:'center'" style="width: 80px">代付货款</th>
                <th data-options="field:'isReceiveForGoods',align:'center'" style="width: 80px">代收货款</th>
                <th data-options="field:'OrderStatus',align:'center'" style="width: 80px">订单状态</th>
                <th data-options="field:'OrderPayStatus',align:'center'" style="width: 80px">支付状态</th>
            </tr>
        </thead>
    </table>
</asp:Content>
