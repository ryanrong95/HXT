<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderList.aspx.cs" Inherits="WebApp.HKWarehouse.Finance.OrderList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>财务(HK)</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>
        gvSettings.fatherMenu = '财务(HK)';
        gvSettings.menu = '订单合同发票';
        gvSettings.summary = '';
    </script>--%>
    <script>
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
                nowrap: false
            });
        });

        function Search() {
            var orderID = $('#OrderID').textbox('getValue');
            var entryNumber = $('#EntryNumber').textbox('getValue');
            $('#datagrid').myDatagrid('search', { OrderID: orderID, EntryNumber: entryNumber});
        }

        function Reset() {
            $("#OrderID").textbox('setValue', "");
            $("#EntryNumber").textbox('setValue', "");
            Search();
        }

        function SearchBill(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            rowdata.OrderID = rowdata.OrderID
            if (rowdata) {
                var url = location.pathname.replace(/OrderList.aspx/ig, 'OrderBillDisplay.aspx') +
                    "?ID=" + rowdata.ID + "&&OrderID=" + rowdata.OrderID;
                window.location = url;
            }
        }

        function Operation(val, row, index) {
            var buttons = '<a id="btnSearchBill" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="SearchBill(' + index + ')" group >' +
                            '<span class =\'l-btn-left l-btn-icon-left\'>' +
                            '<span class="l-btn-text">查看文件</span>' +
                            '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                            '</span>' +
                            '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table id="table1" style="margin: 5px 0 5px 0">
                <tr>
                    <td class="lbl" style="padding-left: 0px">订单编号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:180" id="OrderID" />
                    </td>
                    <td class="lbl">入仓号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:180" id="EntryNumber" />
                    </td>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    </td>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" class="mygrid" title="订单列表" data-options="
            fitColumns:true,
            fit:true,
            border:false,
            scrollbarSize:0,
            toolbar:'#topBar',
            queryParams:{ action: 'data' }">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',width:50,align:'left'" style="width: 12%;">订单编号</th>
                    <th data-options="field:'EntryNumber',width:50,align:'center'" style="width: 10%;">入仓号</th>
                    <th data-options="field:'ClientName',width:100,align:'left'" style="width: 16%;">客户名称</th>
                    <th data-options="field:'SupplierName',width:100,align:'left'" style="width: 16%;">供应商名称</th>
                    <th data-options="field:'CreateDate',width:50,align:'center'" style="width: 10%;">创建日期</th>
                    <th data-options="field:'Status',width:50,align:'center'" style="width: 12%;">订单状态</th>
                    <th data-options="field:'btnSearch',width:50,formatter:Operation,align:'center'" style="width: 12%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
