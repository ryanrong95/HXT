<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.Finance.Receipt.Notice.Detail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单收款明细</title>
    <%--<link rel="stylesheet" href="../../App_Themes/xp/Style.css" />--%>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        $(function () {
            //收款明细
            //收款信息
            var receiptNotice = eval('(<%= this.Model.ReceiptNotice%>)');
            $("#Client").text(receiptNotice.ClientName);
            $("#ReceiptType").text(receiptNotice.ReceiptType);
            $("#Date").text(receiptNotice.ReceiptDate);
            $("#Amount").text(receiptNotice.Amount);
            $("#Currency").text(receiptNotice.Currency);
            $("#Rate").text(receiptNotice.Rate);
            $("#Vault").text(receiptNotice.Vault);
            $("#Account").text(receiptNotice.Account);
            $("#BankAccount").text(receiptNotice.BankAccount);
            $("#Admin").text(receiptNotice.Admin);

            //已收款明细
            $('#ReceivedDetail').myDatagrid({
                border: false,
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                rownumbers: false,
                nowrap: false,
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
                pagination: false
            });
        });

    </script>
</head>
<body>
    <div style="margin-top: 5px; margin-left: 12px;">
        <h1 style="margin-bottom: 5px; font-size: 12px">收款信息</h1>
        <table id="ReceiptInfo" style="border-spacing: 5px; color: #666666; font-size: 12px; width: 514px;">
            <tr>
                <td>客户名称:</td>
                <td>
                    <label id="Client" name="Client" style="width: 250px;"></label>
                </td>
            </tr>
            <tr>
                <td>收款方式：</td>
                <td>
                    <label id="ReceiptType" name="ReceiptType"></label>
                </td>
                <td>收款日期:</td>
                <td>
                    <label id="Date" name="Date"></label>
                </td>
            </tr>
            <tr>
                <td>金额：</td>
                <td>
                    <label id="Amount" name="Amount"></label>
                </td>
                <td>币种：</td>
                <td>
                    <label id="Currency" name="Currency"></label>
                </td>
                <td>汇率：</td>
                <td>
                    <label id="Rate" name="Rate"></label>
                </td>
            </tr>

        </table>
    </div>

    <div style="margin-top: 5px; margin-left: 12px;">
        <h1 style="margin-bottom: 5px; font-size: 12px">收款账户</h1>
        <table id="ReceiptAccount" style="border-spacing: 5px; color: #666666; font-size: 12px; width: 600px;">
            <tr>
                <td>收款金库:</td>
                <td>
                    <label id="Vault" name="Vault"></label>
                </td>
                <td>收款账户：</td>
                <td>
                    <label id="Account" name="Account"></label>
                </td>
            </tr>
            <tr>
                <td>账号:</td>
                <td>
                    <label id="BankAccount" name="BankAccount" style="width: 250px;"></label>
                </td>
                <td>收款人：</td>
                <td>
                    <label id="Admin" name="Admin"></label>
                </td>
            </tr>
        </table>
    </div>
    <div style="margin-top: 5px; margin-left: 12px; margin-bottom: 2px;height:90%">
        <h1 style="margin-bottom: 12px; font-size: 12px;">已收款明细</h1>
        <table id="ReceivedDetail" data-options="toolbar:'#topBar',">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'left'" style="width: 23%; background-color: #f0f0f0">订单编号</th>
                    <th data-options="field:'FeeType',align:'center'" style="width: 14%; background-color: #f8f8f8">费用类型</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 14%; background-color: #f0f0f0">金额</th>
                    <th data-options="field:'OrderStatus',align:'center'" style="width: 14%; background-color: #f8f8f8">订单状态</th>
                    <th data-options="field:'Date',align:'center'" style="width: 23%; background-color: #f8f8f8">操作时间</th>
                    <th data-options="field:'Admin',align:'center'" style="width: 12%; background-color: #f0f0f0">操作人</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
