<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReceiptDetail.aspx.cs" Inherits="WebApp.GeneralManage.Receipt.ReceiptDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>查看收款</title>
    <link rel="stylesheet" href="../../App_Themes/xp/Style.css" />
    <uc:EasyUI runat="server" />
    <script>

        //收款
        var receipts = eval('(<%= this.Model.Receipts%>)');
        $(function () {
            //订单实收款列表初始化
            $('#receiptRecords').datagrid({
                data: receipts
            });
        });
    </script>
</head>
<body>
    <div data-options="region:'center',border: false," style="width: 99%;">
        <div style="width: 100%;">
            <table id="receiptRecords" data-options="fitColumns: true,pagination:false,scrollbarSize:0"
                style="width: 100%; margin-top: 5px;">
                <thead>
                    <tr>
                        <th data-options="field:'Date',align:'center'" style="width: 22%; background-color: #f0f0f0">时间</th>
                        <th data-options="field:'Payee',align:'center'" style="width:18%; background-color: #f8f8f8">收款人</th>
                        <th data-options="field:'ReceivedAmount',align:'center'" style="width: 18%; background-color: #f0f0f0">实收</th>
                        <th data-options="field:'SeqNo',align:'center'" style="width: 22%; background-color: #f0f0f0">银行流水号</th>
                        <th data-options="field:'ReceiptDate',align:'center'" style="width: 20%; background-color: #f0f0f0">银行收款日期</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
</body>
</html>
