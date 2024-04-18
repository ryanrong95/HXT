<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Pay.aspx.cs" Inherits="WebApp.Finance.Payment.TaxPayment.Pay" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var DecTaxFlowID = '<%=this.Model.DecTaxFlowID%>';
        var Amount = '<%=this.Model.Amount%>';
        var TaxTypeInt = '<%=this.Model.TaxTypeInt%>';

        var FinanceVaultData = eval('(<%=this.Model.FinanceVaultData%>)');

        $(function () {
            if (TaxTypeInt == '<%=Needs.Ccs.Services.Enums.DecTaxType.AddedValueTax.GetHashCode()%>') {
                $("#AddedValueTaxAmount").html(Amount);
            } else if(TaxTypeInt == '<%=Needs.Ccs.Services.Enums.DecTaxType.Tariff.GetHashCode()%>') {
                $("#TariffAmount").html(Amount);
            }

            $("#TotalAmount").html(Amount);


            //付款金库
            $('#FinanceVault').combobox({
                data: FinanceVaultData,
                editable: false,
                onSelect: function (record) {
                    //var Currency = $('#Currency').combobox('getValue');
                    var FinanceAccount = $('#FinanceAccount').combobox('getValue');
                    $.post('?action=GetAccounts', { VaultID: record.Value, /*Currency: Currency*/ }, function (data) {
                        var accounts = JSON.parse(data);
                        $('#FinanceAccount').combobox({
                            data: accounts,
                        });
                    });
                }
            });

            //付款账户
            $('#FinanceAccount').combobox({
                editable: false,
            });


        });

        //关闭窗口
        function Close() {
            var ewindow = $.myWindow.getMyWindow("TaxPaymentList2Pay");

            $.myWindow.close();
        }

        //确定
        function Pay() {
            var ewindow = $.myWindow.getMyWindow("TaxPaymentList2Pay");

            if (!$("#form1").form('validate')) {
                return;
            }

            var PayDate = $("#PayDate").datebox("getValue");
            var PayeeName = $("#PayeeName").textbox("getValue");
            var BankName = $("#BankName").textbox("getValue");
            var BankAccount = $("#BankAccount").textbox("getValue");
            var FinanceVaultID = $('#FinanceVault').combobox('getValue');
            var FinanceAccountID = $('#FinanceAccount').combobox('getValue');

            MaskUtil.mask();
            $.post('?action=OnePay', {
                DecTaxFlowID: DecTaxFlowID,
                PayDate: PayDate,
                PayeeName: PayeeName,
                BankName: BankName,
                BankAccount: BankAccount,
                FinanceVaultID: FinanceVaultID,
                FinanceAccountID: FinanceAccountID,
            }, function (res) {
                var result = JSON.parse(res);
                MaskUtil.unmask();

                if (result.success) {
                    $.messager.alert('消息', result.message, 'info', function () {
                        Close();
                    });
                } else {
                    $.messager.alert('消息', result.message, 'info', function () {
                        Close();
                    });
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table style="margin-top:20px; margin-left:100px; font-size:13px; border-collapse:separate; border-spacing:0px 10px;">
                <tr>
                    <td>扣款日期：</td>
                    <td>
                        <input class="easyui-datebox" id="PayDate" name="PayDate" data-options="required:true,editable:false" style="width: 250px; height: 26px;" />
                    </td>
                </tr>
                <tr>
                    <td>收款方：</td>
                    <td>
                        <input class="easyui-textbox" id="PayeeName" name="PayeeName" data-options="value: '暂收款',editable:false" style="width: 250px; height: 26px;" />
                    </td>
                </tr>
                <tr>
                    <td>银行名称：</td>
                    <td>
                        <input class="easyui-textbox" id="BankName" name="BankName" data-options="required:true," style="width: 250px; height: 26px;" />
                    </td>
                </tr>
                <tr>
                    <td>银行账户：</td>
                    <td>
                        <input class="easyui-textbox" id="BankAccount" name="BankAccount" data-options="value: '380100000003371002',editable:false," style="width: 250px; height: 26px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">付款金库：</td>
                    <td>
                        <input class="easyui-combobox" id="FinanceVault" name="FinanceVault"
                            data-options="valueField:'Value',textField:'Text',limitToList:true,required:true" style="width: 250px; height: 26px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl lab">付款账户：</td>
                    <td>
                        <input class="easyui-combobox" id="FinanceAccount" name="FinanceAccount"
                            data-options="valueField:'Value',textField:'Text',limitToList:true,required:true" style="width: 250px; height: 26px;" />
                    </td>
                </tr>
            </table>
            <table style="margin-top:5px; margin-left:100px; font-size:13px; border-collapse:separate; border-spacing:0px 10px;">
                <tbody>
                    <tr>
                        <td>
                            <span style="word-break: break-all;">合计:</span>
                            <span id="TotalAmount">0</span>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span style="word-break: break-all;">其中&nbsp;&nbsp;增值税:</span>
                            <span id="AddedValueTaxAmount">0</span>
                            <span style="word-break: break-all; margin-left: 25px;">关税:</span>
                            <span id="TariffAmount">0</span>
                        </td>
                    </tr>
                </tbody>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a class="easyui-linkbutton" data-options="iconCls:'icon-edit',width:70," onclick="Pay()">确定</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel',width:70," onclick="Close()">取消</a>
    </div>
</body>
</html>
