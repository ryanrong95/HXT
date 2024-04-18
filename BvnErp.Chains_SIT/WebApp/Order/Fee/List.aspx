<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Order.Fee.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单费用维护</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <%--<script>
        gvSettings.fatherMenu = '订单费用(XDT)';
        gvSettings.menu = '费用维护';
        gvSettings.summary = '跟单维护订单费用';
    </script>--%>
    <script type="text/javascript">

        //数据初始化
        $(function () {
            //下拉框数据初始化
            var orderStatus = eval('(<%=this.Model.OrderStatus%>)');
            $('#OrderStatus').combobox({
                data: orderStatus
            });

            //订单列表初始化
            $('#orders').myDatagrid({
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });
        });

        //查询
        function Search() {
            var orderID = $('#OrderID').textbox('getValue');
            var clientCode = $('#ClientCode').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var orderStatus = $('#OrderStatus').combobox('getValue');
            var parm = {
                OrderID: orderID,
                ClientCode: clientCode,
                StartDate: startDate,
                EndDate: endDate,
                OrderStatus: orderStatus
            };
            $('#orders').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $('#OrderID').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#OrderStatus').combobox('setValue', null);
            Search();
        }

        //新增付款申请
        function AddPayment(ID)
        {
            var url = location.pathname.replace(/List.aspx/ig, '../../Finance/Payment/Apply/List.aspx') + '?ID=' + ID;
            window.location = url;
        }

        //杂费维护
        function AddFee(ID)
        {
            var url = location.pathname.replace(/List.aspx/ig, 'Display.aspx') + '?ID=' + ID + '&From=FeeMaintenance';
            window.location = url;
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="AddPayment(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">付款</span>' +
                '<span class="l-btn-icon icon-add">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="AddFee(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">杂费</span>' +
                '<span class="l-btn-icon icon-add">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" />
                    <span class="lbl">订单状态: </span>
                    <input class="easyui-combobox" id="OrderStatus" data-options="valueField:'Key',textField:'Value',editable:false" />
                    <br />
                    <span class="lbl">下单日期: </span>
                    <input class="easyui-datebox" id="StartDate" data-options="editable:false" />
                    <span class="lbl">至 </span>
                    <input class="easyui-datebox" id="EndDate" data-options="editable:false" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>

    <div id="data" data-options="region:'center',border:false">
        <table id="orders" data-options="nowrap:false,fitColumns:true,fit:true,toolbar:'#topBar'" title="订单列表">
            <thead>
                <tr>
                    <th data-options="field:'ID',align:'left'" style="width: 15%;">订单编号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 5%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 15%;">客户名称</th>
                    <th data-options="field:'DeclarePrice',align:'center'" style="width: 10%;">报关总货值</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 5%;">币种</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 10%;">下单日期</th>
                    <th data-options="field:'InvoiceStatus',align:'center'" style="width: 8%;">开票状态</th>
                    <th data-options="field:'PayExchangeStatus',align:'center'" style="width: 8%;">付汇状态</th>
                    <th data-options="field:'OrderStatus',align:'center'" style="width: 8%;">订单状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
