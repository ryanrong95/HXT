<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnDeliveredList.aspx.cs" Inherits="WebApp.HKWarehouse.Delivery.UnDeliveredList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待提货</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        //gvSettings.fatherMenu = '提货通知(HK)';
        //gvSettings.menu = '待提货';
        //gvSettings.summary = '香港库房的待提货通知';
    </script>
    <script type="text/javascript">
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({

                nowrap: false,
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
                 onLoadSuccess: function (data) {
                    var leftTrs = $(".datagrid-view1>.datagrid-body tr");
                    var rightTrs = $(".datagrid-view2>.datagrid-body tr");

                    for (var i = 0; i < leftTrs.length; i++) {
                        var useHeight = 0;

                        if ($(leftTrs[i]).height() > $(rightTrs[i]).height()) {
                            useHeight = $(leftTrs[i]).height();
                        } else {
                            useHeight = $(rightTrs[i]).height();
                        }

                        $(leftTrs[i]).height(useHeight);
                        $(rightTrs[i]).height(useHeight);
                    }

                },

            });
        });

        //查询
        function Search() {
            var orderId = $("#OrderId").textbox('getValue');
            var customerCode = $('#CustomerCode').textbox('getValue');
            $('#datagrid').myDatagrid('search', { OrderId: orderId, CustomerCode: customerCode });
        }

        //重置查询
        function Reset() {
            $("#OrderId").textbox('setValue', "");
            $("#CustomerCode").textbox('setValue', "");
            Search();
        }

        //提货
        function Packing(id) {
            var url = location.pathname.replace(/UnDeliveredList.aspx/ig, 'Confirm.aspx') + "?ID=" + id + "&Status=1";
            window.location = url;
        }

        //操作
        function Operation(val, row, index) {
            var buttons = '<a id="btnPacking" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Packing(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">提货</span>' +
                '<span class="l-btn-icon icon-ok">&nbsp;</span></span></a>';

            return buttons;
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">订单编号:</span>
                    <input class="easyui-textbox search" id="OrderId" />
                    <span class="lbl">入仓号: </span>
                    <input class="easyui-textbox search" id="CustomerCode" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="待提货" class="easyui-datagrid" style="width: 100%;" data-options="fitColumns:true,border:false,fit:true,scrollbarSize:0,singleSelect:true" toolbar="#topBar">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'center'" style="width: 100px;">订单编号</th>
                    <th data-options="field:'CustomerCode',align:'center'" style="width: 50px;">入仓号</th>
                    <th data-options="field:'Customer',align:'left'" style="width: 100px;">客户名称</th>
                    <th data-options="field:'DeliveryCompany',align:'left'" style="width: 100px">供应商</th>
                    <th data-options="field:'DeliveryTime', align:'center'" style="width: 50px">提货时间</th>
                    <th data-options="field:'DeliveryAddress',align:'left'" style="width: 100px">提货地址</th>
                    <th data-options="field:'Merchandiser',align:'left'" style="width: 60px;">跟单员</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 50px;">创建日期</th>
                    <th data-options="field:'btnPacking',formatter:Operation,align:'center'" style="width: 50px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
