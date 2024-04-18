<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Orders.Clients.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>

        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: false,
                nowrap: false,
            });
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $('#CompanyName').textbox("setText", "");
                $('#ClientCode').textbox("setText", "");
                $('#StartDate').datebox("setText", "");
                $('#EndDate').datebox("setText", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
        })
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                CompanyName: $.trim($('#CompanyName').textbox("getText")),
                ClientCode: $.trim($('#ClientCode').textbox("getText")),
                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
            };
            return params;
        };
        //仓储收货订单
        function AddRecieve(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: '新增收货订单',
                minWidth: 1200,
                minHeight: 600,
                url: '../Orders/AddRecieve.aspx?ID=' + data.ID + '&EnterCode=' + data.EnterCode,
                onClose: function () {
                },
            });
            return false;
        }
        //即收即发订单
        function AddTransport(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: '新增转运订单',
                minWidth: 1200,
                minHeight: 600,
                url: '../Orders/AddTransport.aspx?ID=' + data.ID + '&EnterCode=' + data.EnterCode,
                onClose: function () {
                },
            });
            return false;
        }
        //代报关订单
        function AddDeclare(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: '新增报关订单',
                minWidth: 1200,
                minHeight: 600,
                url: '../Orders/AddDeclare.aspx?ID=' + data.ID + '&EnterCode=' + data.EnterCode,
                onClose: function () {
                },
            });
            return false;
        }
        //我的库存
        function MyStock(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: '我的库存',
                minWidth: 1200,
                minHeight: 600,
                url: '../Orders/ClientStock.aspx?ID=' + data.ID + '&EnterCode=' + data.EnterCode,
                onClose: function () {
                },
            });
            return false;
        }
        //付款申请
        function Payment(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: '代付货款订单',
                minWidth: 1200,
                minHeight: 600,
                url: '../Applications/Payments/ClientOrderList.aspx?ID=' + data.ID + '&EnterCode=' + data.EnterCode,
                onClose: function () {
                },
            });
            return false;
        }
        //收款申请
        function Receivable(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: '代收货款订单',
                minWidth: 1200,
                minHeight: 600,
                url: '../Applications/Receivables/ClientOrderList.aspx?ID=' + data.ID + '&EnterCode=' + data.EnterCode,
                onClose: function () {
                },
            });
            return false;
        }
        //下单操作
        function Operation(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-add\'" onclick="AddRecieve(' + index + ');return false;">代收货</a> '
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-add\'" onclick="AddTransport(' + index + ');return false;">即收即发</a> '
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-add\'" onclick="AddDeclare(' + index + ');return false;">代报关</a> '
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="MyStock(' + index + ');return false;">我的库存</a> '
                , '</span>'].join('');
        }
        //申请操作
        function OperationApply(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-assign\'" onclick="Payment(' + index + ');return false;">付款申请</a> '
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-assign\'" onclick="Receivable(' + index + ');return false;">收款申请</a> '
                , '</span>'].join('');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">客户名称</td>
                <td style="width: 200px;">
                    <input id="CompanyName" data-options="prompt:'',validType:'length[1,75]'" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">客户入仓号</td>
                <td style="width: 200px;">
                    <input id="ClientCode" data-options="prompt:'',validType:'length[1,10]'" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">创建日期</td>
                <td>
                    <input id="StartDate" data-options="prompt:''" class="easyui-datebox" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="EndDate" data-options="prompt:''" class="easyui-datebox" />
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
    <table id="tab1" title="我的会员">
        <thead>
            <tr>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 350px;">下单操作</th>
                <%--<th data-options="field:'Btn2',align:'center',formatter:OperationApply" style="width: 200px;">申请操作</th>--%>
                <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">创建日期</th>
                <th data-options="field:'CompanyName',align:'left'" style="width: 300px">客户名称</th>
                <th data-options="field:'CompanyCode',align:'left'" style="width: 200px;">统一社会信用代码</th>
                <th data-options="field:'EnterCode',align:'center'" style="width: 80px">客户入仓号</th>
                <th data-options="field:'Grade',align:'center'" style="width: 80px">客户等级</th>
                <th data-options="field:'ContactName',align:'center'" style="width: 180px;">联系人</th>
                <th data-options="field:'ContactTel',align:'center'" style="width: 100px;">联系电话</th>
                <th data-options="field:'Status',align:'center'" style="width: 80px;">状态</th>
            </tr>
        </thead>
    </table>
</asp:Content>
