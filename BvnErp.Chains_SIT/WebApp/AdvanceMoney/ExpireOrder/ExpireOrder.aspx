<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExpireOrder.aspx.cs" Inherits="WebApp.AdvanceMoney.ExpireOrder.ExpireOrder" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../Content/Ccs.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        $(function () {            
            //列表初始化
            $('#datagrid').myDatagrid({
                actionName: 'data',
                fitColumns: true,
                pageSize: 5,
                fit: true,
                loadFilter: function (data) {                   
                    return data;
                }
            });
        });

    </script>
</head>
<body class="easyui-layout">
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <%--<th data-options="field:'ClientCode',align:'left'" style="width: 16%;">序号</th>--%>
                    <th data-options="field:'OrderID',align:'center'" style="width: 35%;">订单编号</th>                   
                    <th data-options="field:'Amount',align:'center'" style="width: 20%;">逾期金额</th>                    
                    <th data-options="field:'ExpireDate',align:'center'" style="width: 25%;">逾期日期</th>
                    <th data-options="field:'ExpiredDays',align:'center'" style="width: 20%;">逾期天数</th>               
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
