<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.Finance.Receipt.Detail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>查看收款</title>
    <link rel="stylesheet" href="../../App_Themes/xp/Style.css" />
    <uc:EasyUI runat="server" />
    <script type="text/javascript">

        $(function () {
            //收款明细
            //收款信息
            var receipt = eval('(<%=this.Model.Receipt%>)');
            var feeType = eval('(<%=this.Model.FeeType%>)');
            $("#Payer").text(receipt.Payer);
            $("#FeeType").text(receipt.FeeType);
            $("#ReceiptType").text(receipt.ReceiptType);
            $("#Date").text(receipt.ReceiptDate);
            $("#Amount").text(receipt.Amount);
            $("#Currency").text(receipt.Currency);
            $("#Rate").text(receipt.Rate);
            debugger
            if (receipt.ReceiptTypeValue ==<%=Needs.Ccs.Services.Enums.PaymentType.AcceptanceBill.GetHashCode()%>) {
                $("#InSeqNoName").css('display', 'block');
                $("#InSeqNo").css('display', 'block');
                $("#InSeqNo").text(receipt.SeqNo);
                $("#DiscountInterestName").css('display', 'block');
                $("#DiscountInterest").css('display', 'block');
                $("#DiscountInterest").text(receipt.DiscountInterest);
            }

            //收款账户

            $("#FinanceVault").text(receipt.Vault);
            $("#Account").text(receipt.AccountName);
            $("#BankAccount").text(receipt.BankAccount);
            $("#Admin").text(receipt.Admin);
            $("#Summary").text(receipt.Summary);

            //--费用类型是预收账款 显示，否则不显示
            CheckFeeType(feeType);

        });

        function CheckFeeType(feeType) {
            if (feeType == '<%=Needs.Ccs.Services.Enums.FinanceFeeType.DepositReceived.GetHashCode()%>') {
                //订单实收款列表初始化
                $('#orderReceipts').myDatagrid({
                    pagination: false, nowrap: false, scrollbarSize: 0,
                    actionName:'data',
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
                    onLoadSuccess: function (data) {
                        var irow = data.total;
                        $("#orderReceipts").find(".datagrid-wrap").height(32 * (irow + 1));
                        $("#orderReceipts").find(".datagrid-view").height(32 * (irow + 1));
                        $("#orderReceipts").find(".datagrid-body").height(32 * irow);
                    }

                });
            }
        }


    </script>
</head>
<body style="margin-right: 12px;">
    <div style="margin-top: 14px; margin-left: 12px;">
        <h1 style="color: #000000; font-size: 15px">收款信息</h1>
        <table id="ReceiptInfo" style="border-spacing: 10px;font-size: 12px">
            <tr style="color: #666666">
                <td>付款人:</td>
                <td style="padding-right: 50px;">
                    <label id="Payer" name="Payer" data-options="height:15,width:250px"></label>
                </td>
                <td style="padding-right: 18px;">收款类型:</td>
                <td>
                    <label id="FeeType" name="FeeType" data-options="height:20,width:200px"></label>
                </td>
            </tr>
            <tr>
                <td style="padding-right: 26px;">收款方式：</td>
                <td>
                    <label id="ReceiptType" name="ReceiptType" data-options="height:20,width:200px"></label>
                </td>
                <td>收款日期:</td>
                <td>
                    <label id="Date" name="Date" data-options="height:20,width:200px"></label>
                </td>
            </tr>
            <tr>
                <td>金额：</td>
                <td>
                    <label id="Amount" name="Amount" data-options="height:20,width:200px"></label>
                </td>
                <td>币种：</td>
                <td>
                    <label id="Currency" name="Currency" data-options="height:20,width:200px"></label>
                </td>
            </tr>
            <tr>
                <td>汇率：</td>
                <td>
                    <label id="Rate" name="Rate" data-options="height:20,width:200px"></label>
                </td>
                 <td><span id="DiscountInterestName" style="display:none">贴现利息：</span></td>
                <td>
                    <label id="DiscountInterest" name="DiscountInterest" data-options="height:20,width:200px" style="display:none"></label>
                </td>
            </tr>
            <tr>
                 <td><span id="InSeqNoName" style="display:none">承兑票号：</span> </td>
                <td>
                    <label id="InSeqNo" name="InSeqNo" data-options="height:20,width:200px" style="display:none"></label>
                </td>
            </tr>
        </table>
    </div>

    <div style="margin-top: 10px; margin-left: 12px;">
        <h1 style="margin-bottom: 8px; color: #000000; font-size: 15px">收款账户</h1>
        <table id="ReceiptInfo" style="border-spacing: 10px;font-size: 12px">
            <tr style="color: #666666">
                <td>收款金库:</td>
                <td style="padding-right: 15px;">
                    <label id="FinanceVault" name="FinanceVault" data-options="width:250px"></label>
                </td>
                <td>收款账户：</td>
                <td>
                    <label id="Account" name="Account" data-options="height:15px,width:250px"></label>
                </td>
            </tr>
            <tr>
                <td>账号:</td>
                <td style="padding-right: 60px;">
                    <label id="BankAccount" name="BankAccount" data-options="height:20,width:250px"></label>
                </td>
                <td>收款人：</td>
                <td>
                    <label id="Admin" name="Admin" data-options="height:20,width:250px"></label>
                </td>
            </tr>
            <tr>
                <td>备注：</td>
                <td colspan="3">
                    <label id="Summary" name="Summary" data-options="height:20,width:500px"></label>
                </td>
            </tr>
        </table>
    </div>

    <%if (this.Model.FeeType == Needs.Ccs.Services.Enums.FinanceFeeType.DepositReceived.GetHashCode())%>
    <%{%>
   <%-- <div style="margin-top: 15px; margin-left: 12px; margin-bottom: 10px;height:auto">--%>
        <h1 style="margin-bottom: 12px; margin-left: 12px;font-size: 15px">已收款明细</h1>
        <table id="orderReceipts" data-options="pagination: false,nowrap:false,scrollbarSize:0">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'left'" style="width: 30%; background-color: #f0f0f0">订单编号</th>
                    <th data-options="field:'Type',align:'center'" style="width: 15%; background-color: #f8f8f8">费用类型</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 15%; background-color: #f0f0f0">金额</th>
                    <th data-options="field:'Date',align:'center'" style="width: 18%; background-color: #f8f8f8">日期</th>
                    <th data-options="field:'Admin',align:'center'" style="width: 16%; background-color: #f0f0f0">操作人</th>
                </tr>
            </thead>
        </table>
   <%-- </div>--%>
    <%}%>
</body>
</html>
