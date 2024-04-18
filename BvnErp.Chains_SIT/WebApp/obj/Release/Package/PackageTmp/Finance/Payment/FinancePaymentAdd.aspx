<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FinancePaymentAdd.aspx.cs" Inherits="WebApp.Finance.Payment.FinancePaymentAdd" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../Content/Ccs.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/chainsupload.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">

        var PayerData = eval('(<%=this.Model.PayerData%>)');
        var PaymentType = eval('(<%=this.Model.PaymentType%>)');     
        var FinanceVaultData = eval('(<%=this.Model.FinanceVaultData%>)');
        var FinanceAccountData = eval('(<%=this.Model.FinanceAccountData%>)');
        var CurrencyData = eval('(<%=this.Model.CurrencyData%>)');
        var FinancePaymentData = eval('(<%=this.Model.FinancePaymentData%>)');

        $(function () {
          
            //付款方式           
            $('#PayType').combobox({
                data: PaymentType,
                editable: false,
            });           
            //付款人
            $('#Payer').combobox({
                data: PayerData,
                editable: false,
            });
            //币种
            $('#Currency').combobox({
                data: CurrencyData,
                editable: false,
                onSelect: function (record) {
                    $.post('?action=GetExchangeRate', { Currency: record.Value }, function (data) {
                        var data = JSON.parse(data);
                        $('#ExchangeRate').textbox("setValue", data.ExchangeRate);
                    });
                }
            });
            //付款金库
            $('#FinanceVault').combobox({
                data: FinanceVaultData,
                editable: false,
                onSelect: function (record) {
                    var Currency = $('#Currency').combobox('getValue');
                    var FinanceAccount = $('#FinanceAccount').combobox('getValue');
                    $.post('?action=GetAccounts', { VaultID: record.Value, Currency: Currency }, function (data) {
                        var accounts = JSON.parse(data);
                        $('#FinanceAccount').combobox({
                            data: accounts,
                            onLoadSuccess: function () {
                                if (FinanceAccount != null)
                                    $('#FinanceAccount').combobox("setValue", FinancePaymentData['FinanceAccount']);
                           }
                        });
                    });
                }
            });
            //付款账户
            $('#FinanceAccount').combobox({
                data: FinanceAccountData,
                editable: false,
            });

              //收款类型
            $("#AccountCatalog").combotree({
                panelWidth: 200,
                panelHeight: 850,
                url: "?action=AccountCatalogsTree",
                lines: true,
                animate: true,
                onBeforeSelect: function (node) {
                    if (!$(this).tree('isLeaf', node.target)) {
                        $(this).combo("showPanel");
                        return false;
                    }
                },
                onLoadSuccess: function (node, data) {
                    //if (data.length > 0 && financeReceiptData != "") {
                    //    $("#AccountCatalog").combotree('setValue', financeReceiptData.FeeType);
                    //}
                    //付款固定只能付 货款
                    $("#AccountCatalog").combotree('setValue', "AccCatType0077");
                },
            });
            //付款固定只能付 货款
            $("#AccountCatalog").combotree('disable');

              $.post('?action=GetAccountByVault', function (res) {
                            $("#Account").combogrid("grid").datagrid("loadData", res.data);
                        });

              //收款账户
            $('#Account').combogrid({
                idField: 'ID',
                textField: 'BankAccount',
                panelWidth: 300,
                panelHeight: 150,
                fitColumns: true,
                required: true,
                mode: "local",
                columns: [[
                    { field: 'AccountName', title: '账户名称', width: 150, align: 'left' },
                    { field: 'BankAccount', title: '银行账号', width: 150, align: 'left' },
                    { field: 'BankName', title: '银行名称', width: 150, align: 'left' }
                ]],
                onSelect: function () {
                    var grid = $('#Account').combogrid('grid');
                    var row = grid.datagrid('getSelected');
                    $('#PayeeName').textbox('setValue', row.AccountName);
                    $('#BankName').textbox('setValue', row.BankName);
                    $('#BankAccount').val('');
                    $('#BankAccount').val(row.BankAccount);
                }
            });

            Init();
        });

        function Init() {
            if (FinancePaymentData != null&&FinancePaymentData !="") {
                $('#PayeeName').textbox('setValue', FinancePaymentData['PayeeName']);
                $('#BankAccount').textbox('setValue', FinancePaymentData['BankAccount']);
                $('#BankName').textbox('setValue', FinancePaymentData['BankName']);
                //$('#PayFeeType').combobox('setValue', FinancePaymentData['PayFeeType']);
                $('#Amount').textbox('setValue', FinancePaymentData['Amount']);
                $('#Currency').combobox('setValue', FinancePaymentData['Currency']);
                $('#PayType').combobox('setValue', FinancePaymentData['PayType']);
                $('#PayDate').datebox('setValue', FinancePaymentData['PayDate']);
                $('#SeqNo').textbox('setValue', FinancePaymentData['SeqNo']);
                $('#Payer').combobox('setValue', FinancePaymentData['Payer']);
                $('#ExchangeRate').textbox('setValue', FinancePaymentData['ExchangeRate']);
                $('#FinanceVault').combobox('setValue', FinancePaymentData['FinanceVault']);
                $('#FinanceAccount').combobox('setValue', FinancePaymentData['FinanceAccount']);
            } else {
                $('#PayType').combobox('setValue', '3');
            }

            $('#PayeeName').textbox('disable');
            $('#BankName').textbox('disable');
        }

        function Save() {
            if (!Valid("form1")) {
                return;
            }
            var ID = getQueryString("ID");
            var PayeeName = $('#PayeeName').textbox('getValue');
            var BankName = $('#BankName').textbox('getValue');
            var BankAccount = $('#BankAccount').val();

            var data = new FormData($('#form1')[0]);
            data.append("ID", ID);
            data.append("PayeeName",PayeeName);
            data.append("BankName", BankName);
            data.append("BankAccount",BankAccount);
            MaskUtil.mask();
            $.ajax({
                url: '?action=SavePaymentNotice',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        $.messager.alert('消息', res.message, 'info', function () {
                            $.myWindow.close();
                        });
                    }
                    else {
                        $.messager.alert('错误', "保存付款错误");
                    }
                }
            }).done(function () {
                MaskUtil.unmask();//关闭遮挡层
            });
        }

        function Cancel() {
            $.myWindow.close();
        }
    </script>
    <style>
        .lab{
            padding-left:5px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table style="line-height: 36px; margin: 0 auto;font-size:12px">
                <tr>
                    <td class="lbl">费用类型：</td>
                    <td>
                       <%-- <input class="easyui-combobox" id="PayFeeType" name="PayFeeType"
                            data-options="valueField:'Value',textField:'Text',limitToList:true,required:true" style="width: 250px; height: 26px" />--%>
                        <select id="AccountCatalog" name="AccountCatalog" class="easyui-combotree" style="width: 200px;" data-options="required:true,"></select>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">收款方账号：</td>
                    <td>
                        <%--<input class="easyui-textbox" id="BankAccount" name="BankAccount"
                            data-options="validType:'length[1,25]'" style="width: 250px; height: 26px" />--%>
                         <input class="easyui-combogrid" id="Account" name="Account" data-options="required:true,valueField:'Value',textField:'Text', width:250,editable:false,missingMessage:'收款账户不能为空'" />
                         <input type="hidden" id="BankAccount" />
                    </td>
                    
                </tr>
                <tr>
                    <td class="lbl">收款方名称：</td>
                    <td>
                        <input class="easyui-textbox" id="PayeeName" name="PayeeName"
                            data-options="validType:'length[1,50]',multiline:true" style="width: 250px; height: 26px;" />
                    </td>
                    <td class="lbl lab">收款方银行：</td>
                    <td>
                        <input class="easyui-textbox" id="BankName" name="BankName"
                            data-options="validType:'length[1,100]',multiline:true" style="width: 250px; height: 26px" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4" style="padding-top: 5px">
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">金额：</td>
                    <td>
                        <input class="easyui-textbox" id="Amount" name="Amount"
                            data-options="required:true,validType:'length[1,18]'" style="width: 250px; height: 26px" />
                    </td>
                    <td class="lbl lab">付款方式：</td>
                    <td>
                        <input class="easyui-combobox" id="PayType" name="PayType"
                            data-options="valueField:'Value',textField:'Text',limitToList:true,required:true" style="width: 250px; height: 26px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">币种：</td>
                    <td>
                        <input class="easyui-combobox" id="Currency" name="Currency"
                            data-options="valueField:'Value',textField:'Text',limitToList:true,required:true" style="width: 250px; height: 26px" />
                    </td>
                    <td class="lbl lab">付款汇率：</td>
                    <td>
                        <input class="easyui-textbox" id="ExchangeRate" name="ExchangeRate"
                            data-options="required:true,validType:'length[1,18]',tipPosition:'bottom',missingMessage:'请输入汇率'" style="width: 250px; height: 26px" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4" style="padding-top: 5px">
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">付款人：</td>
                    <td>
                        <input class="easyui-combobox" id="Payer" name="Payer"
                            data-options="valueField:'Value',textField:'Text',limitToList:true,required:true" style="width: 250px; height: 26px" />
                    </td>
                    <td class="lbl lab">付款日期：</td>
                    <td>
                        <input class="easyui-datebox" id="PayDate" name="PayDate"
                            data-options="required:true" style="width: 250px; height: 26px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">付款金库：</td>
                    <td>
                        <input class="easyui-combobox" id="FinanceVault" name="FinanceVault"
                            data-options="valueField:'Value',textField:'Text',limitToList:true,required:true" style="width: 250px; height: 26px" />
                    </td>
                    <td class="lbl lab">付款账户：</td>
                    <td>
                        <input class="easyui-combobox" id="FinanceAccount" name="FinanceAccount"
                            data-options="valueField:'Value',textField:'Text',limitToList:true,required:true" style="width: 250px; height: 26px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">付款流水号：</td>
                    <td>
                        <input class="easyui-textbox" id="SeqNo" name="SeqNo"
                            data-options="required:true,validType:'length[1,50]'" style="width: 250px; height: 26px" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a id="btnBack" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Cancel()">取消</a>
    </div>
</body>
</html>
