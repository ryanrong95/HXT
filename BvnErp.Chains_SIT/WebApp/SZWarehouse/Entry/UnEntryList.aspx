<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnEntryList.aspx.cs" Inherits="WebApp.SZWarehouse.Entry.UnEntryList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待入库-入库通知(SZ)</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
   <%-- <script>
        gvSettings.fatherMenu = '入库通知(SZ)';
        gvSettings.menu = '待入库';
        gvSettings.summary = '';
    </script>--%>
    <script>
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
                nowrap: false,
            });
        });

        function Search() {
            var orderID = $('#OrderID').textbox('getValue');
            var entryNumber = $('#EntryNumber').textbox('getValue');
            $('#datagrid').myDatagrid('search', { OrderID: orderID, EntryNumber: entryNumber });
        }

        function Reset() {
            $("#OrderID").textbox('setValue', "");
            $("#EntryNumber").textbox('setValue', "");
            Search();
        }

        //导出Excel
        function Export() {
            var orderID = $('#OrderID').textbox('getValue');
            var entryNumber = $('#EntryNumber').textbox('getValue');
            var data = $('#datagrid').myDatagrid('getRows');
            if (data.length == 0) {
                $.messager.alert('提示', '表格数据为空！');
                return;
            }
            MaskUtil.mask();
            //验证成功
            $.post('?action=Export', {
                OrderID: orderID,
                EntryNumber: entryNumber 
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
                            a.href = rel.url;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    }
                });
            })
        }

        //入库
        function Entry(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/UnEntryList.aspx/ig, 'Entrying.aspx') + "?ID=" + rowdata.ID + "&OrderId=" + rowdata.OrderID;
                window.location = url;
            }
        }

        function Operation(val, row, index) {

            var buttons = '<a id="btnEntry" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Entry(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">入库</span>' +
                '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Export()">导出Excel</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox search" data-options="height:26,width:180" id="OrderID" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox search" data-options="height:26,width:180" id="EntryNumber" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" class="easyui-datagrid" title="待入库" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            singleSelect:true,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th field="OrderID" data-options="align:'center'" style="width: 50px">订单编号</th>
                    <th field="ClientCode" data-options="align:'center'" style="width: 50px">客户编号</th>
                    <th field="ClientName" data-options="align:'left'" style="width: 100px">客户名称</th>
                    <th field="PackNo" data-options="align:'center'" style="width: 50px">件数</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 50px">创建日期</th>
                    <th field="NoticeStatus" data-options="align:'center'" style="width: 50px">状态</th>
                    <th data-options="field:'btn',width:50,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
