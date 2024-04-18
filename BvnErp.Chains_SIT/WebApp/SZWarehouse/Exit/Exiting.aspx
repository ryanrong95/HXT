<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Exiting.aspx.cs" Inherits="WebApp.SZWareHouse.Exit.Exiting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>出库</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <%--<script>
        gvSettings.fatherMenu = '出库通知(SZ)';
        gvSettings.menu = '扫码出库';
        gvSettings.summary = '扫码出库';
    </script>--%>
    <script>

        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
                nowrap: false
            });

            //捕获回车事件
            $("#ExitNoticeID").textbox('textbox').bind('keyup', function (e) {
                if (e.keyCode == 13) {
                    Search();
                    OutStock();
                }
            });
        });


        window.onload = function () {
            $('#ExitNoticeID').textbox('textbox').focus()
        }

        function Search() {

            var ExitNoticeID = $('#ExitNoticeID').textbox('getValue');
            if (ExitNoticeID == "" || ExitNoticeID == null) {
                $('#ExitResult').text("请输入通知编号。");
                return;
            }
            $('#ExitResult').text("");
            $('#datagrid').myDatagrid('search', { ExitNoticeID: ExitNoticeID });
        }

        //出库
        function OutStock() {
            var ExitNoticeID = $('#ExitNoticeID').textbox('getValue');
            if (ExitNoticeID == "" || ExitNoticeID == null) {
                $('#ExitResult').text("请输入通知编号。");
                return;
            }
            MaskUtil.mask();
            $.post('?action=OutStock', {
                ExitNoticeID: ExitNoticeID,
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                var ExitNoticeID = $('#ExitNoticeID').textbox('getValue');
                $('#ExitResult').text("结果：" + ExitNoticeID + rel.message);
                $('#ExitNoticeID').textbox('setValue', "");
            });
        }

        //合并单元格
        function onLoadSuccess(data) {
            var mark = 1;
            for (var i = 1; i < data.rows.length; i++) {
                if (data.rows[i]['BoxIndex'] == data.rows[i - 1]['BoxIndex']||data.rows[i]['StockCode']==data.rows[i - 1]['StockCode']) {
                    mark += 1;
                    $("#datagrid").datagrid('mergeCells', {
                        index: i + 1 - mark,
                        field: 'BoxIndex',
                        rowspan: mark
                    });
                    $("#datagrid").datagrid('mergeCells', {
                        index: i + 1 - mark,
                        field: 'StockCode',
                        rowspan: mark
                    });
                }
                else {
                    mark = 1;
                }
            }
        }

    </script>


</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="margin: 5px 0">
                <tr>
                    <td class="lbl" style="font-size: 16px">扫描条形码：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:200" id="ExitNoticeID" />
                    </td>
                    <td>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    </td>
                    <td>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="OutStock()">出库</a>
                    </td>
                    <td class="lbl" style="padding-left: 15px; color: red" id="ExitResult"></td>
                </tr>
            </table>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" class="mygrid" title="扫描出库" data-options="
            fitColumns:true,
            fit:true,
            border:false,
            scrollbarSize:0,
            pagination: false, 
            rownumbers: false, 
            onLoadSuccess: onLoadSuccess,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'StockCode',align:'center'," style="width: 10%;">库位号</th>
                    <th data-options="field:'BoxIndex',align:'center'," style="width: 10%;">箱号</th>
                    <th data-options="field:'OrderID',align:'left'," style="width: 10%;">订单编号</th>
                    <th data-options="field:'Model',align:'center'," style="width: 14%;">型号</th>
                    <th data-options="field:'ProductName',align:'left'," style="width: 14%;">品名</th>
                    <th data-options="field:'Manufactors',align:'center'," style="width: 10%;">品牌</th>
                    <th data-options="field:'Qty',align:'center'," style="width: 6%;">数量</th>
                    <th data-options="field:'NetWeight',align:'center'," style="width: 6%;">净重</th>
                    <th data-options="field:'GrossWeight',align:'center'," style="width: 6%;">毛重</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
