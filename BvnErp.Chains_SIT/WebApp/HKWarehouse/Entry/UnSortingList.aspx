<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnSortingList.aspx.cs" Inherits="WebApp.HKWarehouse.Entry.UnSortingList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待封箱</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>
        gvSettings.fatherMenu = '入库通知(HK)';
        gvSettings.menu = '待装箱';
        gvSettings.summary = '香港库房的待装箱的入库通知';
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

        //function Packing(Index) {
        //    $('#datagrid').datagrid('selectRow', Index);
        //    var rowdata = $('#datagrid').datagrid('getSelected');
        //    rowdata.OrderID = rowdata.OrderID
        //    if (rowdata) {
        //        var url = location.pathname.replace(/UnSortingList.aspx/ig, 'Packing.aspx') +
        //            "?ID=" + rowdata.ID + "&&OrderID=" + rowdata.OrderID + "&&EntryNumber=" + rowdata.EntryNumber + "&&EntryStatus=" + 1;
        //        window.location = url;
        //    }
        //}

        function Packing(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            rowdata.OrderID = rowdata.OrderID
            if (rowdata) {
                var url = location.pathname.replace(/UnSortingList.aspx/ig, '../Sorting/Sorting.aspx') +
                    "?ID=" + rowdata.ID + "&OrderID=" + rowdata.OrderID;
                window.location = url;
            }
        }

        function Operation(val, row, index) {
            var buttons = '<a id="btnPacking" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Packing(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">装箱</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            return buttons;
        }
    </script>

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
        <table id="datagrid" class="easyui-datagrid" style="width: 100%;" title="待封箱" data-options="
         border:false,
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            singleSelect:true,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th field="OrderID" data-options="align:'left'" style="width: 60px">订单编号</th>
                    <th field="EntryNumber" data-options="align:'center'" style="width: 50px">入仓号</th>
                    <th field="ClientName" data-options="align:'left'" style="width: 100px">客户名称</th>
                    <th field="SupplierName" data-options="align:'left'" style="width: 100px">供应商名称</th>
                    <th field="Type" data-options="align:'center'" style="width: 50px">交货方式</th>
                    <th field="Status" data-options="align:'center'" style="width: 50px">状态</th>
                    <th field="CreateDate" data-options="align:'center'"  style="width: 50px"   >创建时间</th>
                    <th data-options="field:'btnPacking',width:50,formatter:Operation,align:'center'" style="width: 50px">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>



