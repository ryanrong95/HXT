<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="WebApp.SZWarehouse.Entry.Details" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>详情</title>
    <uc:EasyUI runat="server" />
    <script>
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
            });
        });

        //返回
        function Back() {
            var url = location.pathname.replace(/Details.aspx/ig, 'EntryedList.aspx');
            window.location = url;
        }

        //合并单元格
        function onLoadSuccess(data) {
            var mark = 1;
            for (var i = 1; i < data.rows.length; i++) {
                if (data.rows[i]['CaseNumber'] == data.rows[i - 1]['CaseNumber']) {
                    mark += 1;
                    $("#datagrid").datagrid('mergeCells', {
                        index: i + 1 - mark,
                        field: 'CaseNumber',
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
    <div data-options="region:'center',border:false,title:'已入库产品'">
        <div style="padding:6px">
            <a id="btnBack" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Back()">返回</a>
        </div>
        <table id="datagrid" class="easyui-datagrid" title="" data-options="
            fitColumns:true,
            scrollbarSize:0,
            singleSelect:true,
            onLoadSuccess: onLoadSuccess">
            <thead>
                <tr>
                    <th field="CaseNumber" data-options="align:'center'" style="width: 50px">箱号</th>
                    <th field="Model" data-options="align:'center'" style="width: 50px">型号</th>
                    <th field="ProductName" data-options="align:'center'" style="width: 100px">报关品名</th>
                    <th field="Manufactor" data-options="align:'center'" style="width: 50px">品牌</th>
                    <th field="Quantity" data-options="align:'center'" style="width: 50px">数量</th>
                    <th field="NetWeight" data-options="align:'center'" style="width: 50px">净重</th>
                    <th field="GrossWeight" data-options="align:'center'" style="width: 50px">毛重</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
