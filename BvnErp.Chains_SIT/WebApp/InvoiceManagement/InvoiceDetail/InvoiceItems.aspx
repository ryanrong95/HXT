<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceItems.aspx.cs" Inherits="WebApp.InvoiceManagement.InvoiceDetail.InvoiceItems" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        $(function () {
            $('#InvoiceItems').myDatagrid({
                nowrap: false,
                fitColumns: true,
                fit: true,
                border: false,
                singleSelect: false,

            });
        });
    </script>
</head>
<body class="easyui-layout">
    <table id="InvoiceItems" title="货物明细清单" data-options="nowrap:false,fitColumns:true,fit:true,border:false,singleSelect:false,">
        <thead>
            <tr>
                <th data-options="field:'lineNum',align:'left'" style="width: 5%">行号</th>
                <th data-options="field:'goodserviceName',align:'left'" style="width: 20%">货物(劳务)名称</th>
                <th data-options="field:'model',align:'left'" style="width: 20%">规格型号</th>
                <th data-options="field:'unit',align:'left'" style="width: 10%">单位</th>
                <th data-options="field:'number',align:'left'" style="width: 10%">数量</th>
                <th data-options="field:'price',align:'left'" style="width: 10%">单价</th>
                <th data-options="field:'sum',align:'left'" style="width: 10%">金额</th>
                <th data-options="field:'taxRate',align:'left'" style="width: 5%">税率</th>
                <th data-options="field:'tax',align:'left'" style="width: 10%">税额</th>
            </tr>
        </thead>
    </table>   
</body>
</html>
