<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeliveredList.aspx.cs" Inherits="WebApp.HKWarehouse.Delivery.DeliveredList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>已提货</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>

        //gvSettings.fatherMenu = '提货通知(HK)';
        //gvSettings.menu = '已完成';
        //gvSettings.summary = '香港库房的待提货通知';
    </script>
    <script>
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
                }
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

        //详情
        function Detial(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            var ID = rowdata.ID;
            if (rowdata) {
                var url = location.pathname.replace(/DeliveredList.aspx/ig, 'Confirm.aspx') + "?ID=" + rowdata.ID + "&Status=2";;
                window.location = url;

            }
        }

        //操作
        function Operation(val, row, index) {
            var buttons = '<a id="btnDetial" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Detial(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                 '<span class="l-btn-icon icon-search">&nbsp;</span></span></a>';
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
        <table id="datagrid" title="已提货" class="easyui-datagrid" style="width: 100%;" data-options="fitColumns:true,border:false,fit:true,scrollbarSize:0,singleSelect:true" toolbar="#topBar">
            <thead>
                <tr>
                    <th data-options="field:'ID',align:'center', hidden:'true'" style="width: 100px;">通知编号</th>
                    <th data-options="field:'OrderID',align:'center'" style="width: 100px;">订单编号</th>
                    <th data-options="field:'CustomerCode',align:'center'" style="width: 60px;">入仓号</th>
                    <th data-options="field:'Customer',align:'left'" style="width: 100px;">客户名称</th>
                    <th data-options="field:'DeliveryCompany',align:'left'" style="width: 100px">供应商</th>
                    <th data-options="field:'DeliveryTime', align:'center'" style="width: 60px">提货时间</th>
                    <th data-options="field:'DeliveryAddress',align:'left'" style="width: 120px">提货地址</th>
                    <th data-options="field:'Merchandiser',align:'center'" style="width: 60px;">跟单员</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 60px;">创建日期</th>
                    <th data-options="field:'btnPacking',formatter:Operation,align:'center'" style="width: 100px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
