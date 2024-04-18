<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderList.aspx.cs" Inherits="WebApp.HKWarehouse.Report.OrderList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>报关订单</title>
    <uc:EasyUI runat="server" />
     <%--<script>
        gvSettings.fatherMenu = '报表';
        gvSettings.menu = '报关订单';
        gvSettings.summary = '';
    </script>--%>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        var OrderStatus = eval('(<%=this.Model.OrderStatus%>)');

        $(function () {
            $('#OrderStatus').combobox({
                data: OrderStatus,
            });

            Search();
        });

        //查询
        function Search() {
            var OrderID = $('#OrderID').textbox('getValue');
            var ClientCode = $('#ClientCode').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var OrderStatus = $('#OrderStatus').combobox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');

            $('#orderList-datagrid').myDatagrid({
                queryParams: {
                    ClientCode: ClientCode,
                    ClientName: ClientName,
                    OrderID: OrderID,
                    OrderStatus: OrderStatus,
                    StartDate: StartDate,
                    EndDate: EndDate
                },
                actionName:  'OrderData',
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }

                    return data;
                },
            });
        }

        //重置查询条件
        function Reset() {
            $('#OrderID').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#ClientName').textbox('setValue', null);
            $('#OrderStatus').combobox('select', "");
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //操作
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="AddFee(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">添加费用</span>' +
                '<span class="l-btn-icon icon-add">&nbsp;</span></span></a>';
            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="ViewFees(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span></span></a>';
            return buttons;
        }

        //收款
        function AddFee(id) {
            var url = location.pathname.replace(/OrderList.aspx/ig, '../Fee/FeeAdd.aspx') + '?OrderID=' + id;
            top.$.myWindow({
                iconCls: '',
                url: url,
                noheader: false,
                title: '新增费用',
                width: '720px',
                height: '540px',
                onClose: function () {
                }
            });
        }

        //查看
        function ViewFees(id) {
            var url = location.pathname.replace(/OrderList.aspx/ig, '../Fee/OrderFeeList.aspx') + '?OrderID=' + id;
            top.$.myWindow({
                iconCls: '',
                url: url,
                noheader: false,
                title: '查看费用',
                width: '900px',
                height: '600px',
                onClose: function () {

                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar-orderList-datagrid">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" />
                    <span class="lbl">客户名称: </span>
                    <input class="easyui-textbox" id="ClientName" />
                </li>
                <li>
                    <span class="lbl">订单状态: </span>
                    <input class="easyui-combobox" id="OrderStatus" name="OrderStatus" data-options="valueField:'TypeValue',textField:'TypeText',required:false,editable:false" />
                    <span class="lbl">下单时间: </span>
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
        <table id="orderList-datagrid" class="easyui-datagrid" title="报关订单" data-options="
            border:false,
            nowrap:false,
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            singleSelect:true,
            toolbar:'#topBar-orderList-datagrid'">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'center'" style="width: 15%">订单编号</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 9%">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 15%">客户名称</th>
                    <th data-options="field:'DeclarePrice',align:'center'" style="width: 9%">金额</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 10%">币种</th>
                    <th data-options="field:'OrderConsigneeType',align:'center'" style="width: 10%">交货方式</th>
                    <th data-options="field:'OrderStatus',align:'center'" style="width: 10%">订单状态</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 10%">下单时间</th>
                    <th data-options="field:'btnOperation',formatter:Operation,align:'center'" style="width: 10%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
