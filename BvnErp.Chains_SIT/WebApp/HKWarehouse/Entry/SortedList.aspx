<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SortedList.aspx.cs" Inherits="WebApp.HKWarehouse.Entry.SortedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>已封箱</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--<script>
        gvSettings.fatherMenu = '入库通知(HK)';
        gvSettings.menu = '已封箱';
        gvSettings.summary = '';
    </script>--%>
    <script>
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
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
                nowrap: false
            });
        });

        function Search() {
            var Supplier = $('#Supplier').textbox('getValue');
            var EntryNumber = $('#EntryNumber').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var Model = $('#Model').textbox('getValue');
            $('#datagrid').myDatagrid('search', { Supplier: Supplier, EntryNumber: EntryNumber, ClientName: ClientName, Model: Model });
        }

        function Reset() {
            $("#Supplier").textbox('setValue', "");
            $("#EntryNumber").textbox('setValue', "");
            $('#ClientName').textbox('setValue', "");
            $('#Model').textbox('setValue', "");
            Search();
        }

        //function Details(Index) {
        //    $('#datagrid').datagrid('selectRow', Index);
        //    var rowdata = $('#datagrid').datagrid('getSelected');
        //    rowdata.OrderID = rowdata.OrderID
        //    if (rowdata) {
        //        var url = location.pathname.replace(/SortedList.aspx/ig, 'Packing.aspx') +
        //           "?ID=" + rowdata.ID + "&&OrderID=" + rowdata.OrderID + "&&EntryNumber=" + rowdata.EntryNumber + "&&EntryStatus=" + 3
        //        window.location = url;
        //    }
        //}

        function Details(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            rowdata.OrderID = rowdata.OrderID
            if (rowdata) {
                 var url = location.pathname.replace(/SortedList.aspx/ig, '../Sorting/Sorting.aspx') +
                    "?ID=" + rowdata.ID + "&OrderID=" + rowdata.OrderID;
                window.location = url;
            }
        }

        function Operation(val, row, index) {
            var buttons = '<a id="btnDetails" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Details(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">详情</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
    <style>
    </style>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table>
                <tr>
                    <td class="lbl">供应商：</td>
                    <td>
                        <input class="easyui-textbox" id="Supplier" />
                    </td>
                    <td class="lbl" style="padding-left: 5px">入仓号：</td>
                    <td>
                        <input class="easyui-textbox" id="EntryNumber" />
                    </td>
                    
                </tr>
                <tr>
                    <td class="lbl">客户名称：</td>
                    <td>
                        <input class="easyui-textbox" id="ClientName" />
                    </td>
                    <td class="lbl" style="padding-left: 5px">型号：</td>
                    <td>
                        <input class="easyui-textbox" id="Model" />
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
        <table id="datagrid" class="easyui-datagrid" style="width: 100%;" title="已封箱" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            singleSelect:true,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th field="OrderID" data-options="align:'left'" style="width: 60px">订单编号</th>
                    <th field="EntryNumber" data-options="align:'center'" style="width: 30px">入仓号</th>
                    <th field="ClientName" data-options="align:'left'" style="width: 100px">客户名称</th>
                    <th field="SupplierName" data-options="align:'left'" style="width: 100px">供应商名称</th>
                    <th field="UpdateDate" data-options="align:'center'" style="width: 30px">封箱日期</th>
                    <%--<th field="Status" data-options="align:'center'" style="width: 50px">状态</th>--%>
                    <th data-options="field:'btnPacking',width:40,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
