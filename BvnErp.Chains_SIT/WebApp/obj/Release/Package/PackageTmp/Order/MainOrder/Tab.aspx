<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tab.aspx.cs" Inherits="WebApp.MainOrder.Tab" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单信息</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        $(function () {
            $('#order').tabs({
                border: false,
                tabWidth: 120,
            });

            var id = getQueryString('ID');
            var from = getQueryString('From');

            AddTab('order', '详情', '../MainDetail.aspx?ID=' + id + '&From=' + from, 'detail',2000);
            AddTab('order', '对账单', '../Bill/OrderBill.aspx?ID=' + id + '&From=' + from, 'bill',2000);
            AddTab('order', '委托书', '../AgentProxy/Instrument.aspx?ID=' + id + '&From=' + from, 'instrument', 1900);      
            AddTab('order', '销售合同', '../SalesContract/SalesContract.aspx?ID=' + id + '&From=' + from, 'salesContract', 1900);  
            AddTab('order', '附件', '../File/Display.aspx?ID=' + id + '&From=' + from, 'file', 900);
            //AddTab('order', '日志', '../Query/OrderLog.aspx?ID=' + id + '&From=' + from, 'file',900);
        });
    </script>
</head>
<body>
    <div id="order" class="easyui-tabs">
    </div>
</body>
</html>
