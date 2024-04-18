<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaymentDetail.aspx.cs" Inherits="WebApp.GeneralManage.Receipt.PaymentDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" href="../../App_Themes/xp/Style.css" />
    <uc:EasyUI runat="server" />
    <script>

        //收款
        var payments = eval('(<%= this.Model.Payments%>)');
        $(function () {
            //订单实收款列表初始化
            $('#paymentRecords').datagrid({
                data: payments
            });
        });
    </script>
</head>
<body>
<div data-options="region:'center',border: false," style="width: 99%;">
    <div style="width: 100%;">
        <table id="paymentRecords" data-options="fitColumns: true,pagination:false,scrollbarSize:0"
               style="width: 100%; margin-top: 5px;">
            <thead>
            <tr>
                <th data-options="field:'PayDate',align:'center'" style="width: 20%; background-color: #f0f0f0">时间</th>
                <th data-options="field:'Payor',align:'center'" style="width: 15%; background-color: #f0f0f0">付款人</th>
                <th data-options="field:'Payee',align:'center'" style="width:40%; background-color: #f8f8f8">收款人</th>
                <th data-options="field:'PayAmount',align:'center'" style="width:25%; background-color: #f8f8f8">实付</th>
            </tr>
            </thead>
        </table>
    </div>
</div>
</body>
</html>
