<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvOms.WebApp.Approval.Orders.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                singleSelect: true,
                fitColumns: false,
                fit: true,
                scrollbarSize: 0,
            });
            $('#OrderType').combobox({
                data: model.OrderType,
                editable:false,
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
                $('#OrderType').combobox("setValue", "")
                $('#OrderPayStatus').combobox("setValue", "")
                $('#StartDate').datebox("setValue", "")
                $('#EndDate').datebox("setValue", "")
                $("#isPayForGoods").checkbox('uncheck')
                grid.myDatagrid('search', getQuery());
                return false;
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var isPayForGoods = $("#isPayForGoods").checkbox('options').checked;
            var params = {
                action: 'data',
                OrderID: $.trim($('#OrderID').textbox("getText")),
                CompanyName: $.trim($('#CompanyName').textbox("getText")),
                ClientCode: $.trim($('#EnterCode').textbox("getText")),
                Supplier: $.trim($('#Supplier').textbox("getText")),
                OrderType: $.trim($('#OrderType').combobox("getValue")),
                OrderPayStatus: $.trim($('#OrderPayStatus').combobox("getValue")),
                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
                IsPayForGoods: isPayForGoods,
            };
            return params;
        };
        function Operation(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-man\'" onclick="Approve(\'' + row.ID + '\');return false;">审批</a> '
                , '</span>'].join('');
        }
        //待审批
        function Approve(id) {
            $.myWindow({
                title: "订单审批",
                url: location.pathname.replace('List.aspx', 'Edit.aspx?ID=' + id),
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
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
                    <input id="EnterCode" class="easyui-textbox" style="width: 150px" />
                </td>
                <td>创建日期</td>
                <td>
                    <input id="StartDate" data-options="prompt:'开始日期'" class="easyui-datebox" style="width: 108px" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="EndDate" data-options="prompt:'结束日期'" class="easyui-datebox" style="width: 108px" />
                </td>
            </tr>
            <tr>
                <td>供应商</td>
                <td>
                    <input id="Supplier" class="easyui-textbox" style="width: 150px" />
                </td>
                <td>业务类型</td>
                <td>
                    <input id="OrderType" class="easyui-combobox" style="width: 150px" />
                </td>
                <td>支付状态</td>
                <td>
                    <input id="OrderPayStatus" class="easyui-combobox" style="width: 150px" />
                </td>
                <td>其它条件</td>
                <td colspan="3">
                    <input id="isPayForGoods" name="isPayForGoods" class="easyui-checkbox" value="true"
                        data-options="label:'是否代付货款',labelPosition:'after'">
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
    <table id="tab1" title="已挂起订单">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">下单时间</th>
                <th data-options="field:'ID',align:'left'" style="width: 150px">订单号</th>
                <th data-options="field:'CompanyName',align:'left'" style="width: 250px">客户名称</th>
                <th data-options="field:'Supplier',align:'left'" style="width: 250px">供应商</th>
                <th data-options="field:'EnterCode',align:'center'" style="width: 80px">客户入仓号</th>
                <th data-options="field:'OrderType',align:'center'" style="width: 80px">业务类型</th>
                <th data-options="field:'IsPayForGoods',align:'center'" style="width: 80px">代付货款</th>
                <th data-options="field:'OrderStatus',align:'center'" style="width: 80px">订单状态</th>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 120px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>

