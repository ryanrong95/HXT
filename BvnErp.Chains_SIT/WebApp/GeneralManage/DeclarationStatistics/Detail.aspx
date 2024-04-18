<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.GeneralManage.DeclarationStatistics.Detail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>报关量统计明细</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">

        $(function () {
            //订单列表初始化
            window.grid = $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                toolbar: '#topBar',
                queryParams: { action: "data" },
                pagination: false,
            });
        });
        //返回
        function Back() {
            var url = location.pathname.replace(/Detail.aspx/ig, 'List.aspx');
            window.location = url;
        }
    </script>
    <style>
        .divstyle {
            display: inline;
            margin: 20px 5px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Back()"
            data-options="iconCls:'icon-back'">返回</a>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="报关量统计明细">
            <thead>
                <tr>
                    <th data-options="field:'ClientName',align:'left'" style="width: 100px;">客户名称</th>
                    <th data-options="field:'OrderID',align:'left'" style="width: 100px;">订单编号</th>
                    <th data-options="field:'DeclarePrice',align:'center'" style="width: 100px;">报关货值</th>
                    <th data-options="field:'OrderDate',align:'center'" style="width: 100px;">下单日期</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 100px;">币种</th>
                    <th data-options="field:'OrderStatus',align:'center'" style="width: 100px;">订单状态</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
