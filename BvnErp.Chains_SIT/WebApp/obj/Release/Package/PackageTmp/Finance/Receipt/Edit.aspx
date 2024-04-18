<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Finance.Receipt.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>新增收款</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        //数据初始化
        var CenterPaymentType = eval('(<%=this.Model.CenterPaymentType%>)');
        var AccountProperty = eval('(<%=this.Model.AccountProperty%>)');
        var financeReceiptData = eval('(<%=this.Model.FinanceReceiptData%>)');
        var currData = eval('(<%=this.Model.CurrData%>)');
        var CenterDepositReceived = '<%=this.Model.CenterDepositReceived%>';
        //console.log(currData);
        var vaultData = eval('(<%=this.Model.FinanceVaultData%>)');
        var accountData = eval('(<%=this.Model.AccountData%>)');

        function bindFinanceValue(data) {

            if (data.length > 0
                && financeReceiptData
                && financeReceiptData['Vault']
                && data.findIndex(f => f.Value == financeReceiptData['Vault']) != -1) {
                $('#FinanceVault').combobox('setValue', financeReceiptData['Vault']);
            }
        }

        $(function () {

            $('#PaymentType').combobox({
                data: CenterPaymentType,
            })

            $('#AccountProperty').combobox({
                data: AccountProperty,
            })

            $('#AccountProperty').combobox('setValue', 1);

            //币种变化-->更新金库和更新汇率
            $('#Currency').combobox({
                data: currData,
                onChange: function () {
                    GetVaultByCurr();
                    //币种—>金库->账户
                    $('#Account').combogrid('setValue', null);
                    // $('#FinanceVault').combobox('setValue', null);
                    //汇率同步变化
                    GetExchangeRateByCurr();
                },
            });


            //收款类型 初始化
            $('#FeeType').combobox({
                onSelect: function (data) {
                    var selectedFeeType = data.value; //$("#FeeType").combobox("getValue");
                    if (selectedFeeType != null) {
                        //其他收款类型，币种汇率可编辑
                        if (selectedFeeType !==
                            "<%=Needs.Ccs.Services.Enums.FinanceFeeType.DepositReceived.GetHashCode()%>") {
                            $("#Currency").combobox({ disabled: false });
                            $("#Rate").textbox({ disabled: false });
                        } else {
                            // 预收账款 币种无法选择，默认的RMB 汇率 1
                            $('#Rate').textbox('setValue', '1');
                            if ($("#Currency").combobox("getValue") != "CNY") {
                                $('#Currency').combobox('setValue', 'CNY');
                            }
                            $("#Rate").textbox({ disabled: true });
                            $("#Currency").combobox('disable');
                        }
                    } else {
                        //默认币种和汇率
                        if ($("#Currency").combobox("getValue") != "CNY") {
                            $('#Currency').combobox('setValue', 'CNY');
                        }
                        $('#Rate').textbox('setValue', '1');
                    }
                    // GetVaultByCurr();
                    // 币种—>金库->账户
                    $('#Account').combogrid('setValue', null);
                }
            });

            //收款金库->账户更新
            $('#FinanceVault').combobox({
                data: vaultData,
                onLoadSuccess: function (data) {
                    bindFinanceValue(data);
                },
                onChange: function () {
                    var selectedCurrency = $('#Currency').combobox("getValue");
                    var selectedVault = $("#FinanceVault").combobox("getValue");

                    if (selectedVault != null) {
                        $.post('?action=GetAccountByVault', { FinanceVault: selectedVault, Currency: selectedCurrency }, function (res) {
                            $("#Account").combogrid("grid").datagrid("loadData", res.data);
                        });
                    }
                }
            });

            //收款账户
            $('#Account').combogrid({
                idField: 'ID',
                textField: 'AccountName',
                panelWidth: 300,
                panelHeight: 150,
                fitColumns: true,
                required: true,
                mode: "local",
                columns: [[
                    { field: 'AccountName', title: '账户名称', width: 150, align: 'left' },
                    { field: 'BankAccount', title: '银行账号', width: 150, align: 'left' }
                ]],
                onSelect: function () {
                    var grid = $('#Account').combogrid('grid');
                    var row = grid.datagrid('getSelected');
                    $('#BankAccount').textbox('setValue', row.BankAccount);
                }
            });

            //收款人
            $('#Admin').textbox('setValue', '<%=this.Model.Admin%>');

            //针对新增收款
            //默认币种和汇率
            if (financeReceiptData == null || financeReceiptData == "") {
                $('#Currency').combobox('setValue', 'CNY');
                $('#Rate').textbox('setValue', '1');
                GetVaultByCurr();
            }
            //编辑界面
            else {
                Init();
            }

            $("input[name=Amount]").prev().on("input", function () {
                var am = $(this).val();
                $(this).val(am.replace(',', ''));
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
                    if (data.length > 0 && financeReceiptData != "") {
                        $("#AccountCatalog").combotree('setValue', financeReceiptData.FeeType);
                    }
                },
            });
        });

        //根据币种获取账户
        function GetVaultByCurr() {
            var selectedValue = $("#Currency").combobox("getValue");
            if (selectedValue != null) {
                $.post('?action=GetVaultByCurrency',
                    { Currency: selectedValue },
                    function (res) {
                        //if (financeReceiptData && res.findIndex(f => f.Value == financeReceiptData['Vault']) == -1) {
                            $('#FinanceVault').combobox('setValue', null); // 此代码解决，每次下拉列表数据更新时，在还没有更新完成之前， 默认选择项就自动匹配的问题
                        //}
                        $("#FinanceVault").combobox("loadData", res);
                        bindFinanceValue(res);
                    });
            }
        }

        //根据币种获取汇率
        function GetExchangeRateByCurr() {
            var selectedCurrency = $("#Currency").combobox("getValue");
            if (selectedCurrency != null) {
                $.post('?action=GetExchangeRate',
                    { Currency: selectedCurrency },
                    function (res) {
                        $("#Rate").textbox('setValue', res);
                    });
            }
        }

        //编辑初始化
        function Init() {
            if (financeReceiptData != null || financeReceiptData != "") {
                $('#Payer').textbox('setValue', financeReceiptData['Payer']);
                $('#SeqNo').textbox('setValue', financeReceiptData['SeqNo']);
                $('#FeeType').combobox('setValue', financeReceiptData['FeeType']);
                $('#PaymentType').combobox('setValue', financeReceiptData['ReceiptType']);
                $('#Date').datebox('setValue', financeReceiptData['ReceiptDate']);
                $('#Amount').textbox('setValue', financeReceiptData['Amount']);
                $('#Currency').combobox('setValue', financeReceiptData['Currency']);

                // GetVaultByCurr();
                $('#Rate').textbox('setValue', financeReceiptData['Rate']);

                // $('#FinanceVault').combobox('loadData', vaultData);
                // $('#FinanceVault').combobox('setValue', financeReceiptData['Vault']);

                $("#Account").combogrid("grid").datagrid("loadData", accountData);
                $('#Account').combogrid('setValue', financeReceiptData['Account']);

                $('#BankAccount').textbox('setValue', financeReceiptData['BankAccount']);
                $('#Admin').textbox('setValue', financeReceiptData['Admin']);
                $('#Summary').textbox('setValue', financeReceiptData['Summary']);
                $('#AccountProperty').combobox('setValue', financeReceiptData['AccountProperty']);
            }
        }

        //保存之前检查
        function SaveCheck() {
            //验证表单数据
            if (!Valid('form1')) {
                return;
            }

            var SeqNo = $("#SeqNo").textbox("getValue");
            $.post('?action=CheckSeqNo', { SeqNo: SeqNo}, function (res) {
                var result = JSON.parse(res);               
                if (!result.success) {
                    $.messager.alert('提示', result.message);
                    return;
                } else {
                    //预收账款需要判断付款人是客户（companyName）
                    //var feeType = $('#FeeType').combobox("getValue");
                    var feeType = $('#AccountCatalog').combotree("getValue");
                    if (feeType == CenterDepositReceived) {
                        var payer = $("#Payer").textbox("getValue");
                        $.post('?action=CheckClient',
                            { Payer: payer },
                            function (res) {
                                var result = JSON.parse(res);
                                if (result.success) {
                                    Save();
                                } else {
                                    $.messager.alert('提示', "付款客户名称有误，请仔细核对！");
                                }
                            });
                    } else {
                        Save();
                    }
                }
            });
        }

        //保存
        function Save() {
            var values = FormValues('form1');          
            values['ID'] = financeReceiptData["ID"];
            //values['FeeType'] = $('#FeeType').combobox("getValue");
            values['PaymentType'] = $('#PaymentType').combobox("getValue");
            values['AccountID'] = $('#Account').combogrid("getValue");
            values['AccountProperty'] = $('#AccountProperty').combobox("getValue");
            values['FeeType'] = $('#AccountCatalog').combotree("getValue");
            MaskUtil.mask();
            $.post('?action=Save', { Model: JSON.stringify(values) },
                function (res) {
                    MaskUtil.unmask()
                    var result = JSON.parse(res);
                    if (result.success) {
                        $.messager.alert('', result.message, 'info', function () {
                            Cancel();
                        });
                    } else {
                        $.messager.alert('提示', result.message);
                    }
                });
        }

        //取消
        function Cancel() {
            $.myWindow.close();
        }

        function ChooseAcceptance() {
            var url = location.pathname.replace(/Edit.aspx/ig, './AcceptanceBillList.aspx') + '?From=select&WindowName=FinanceReceipt';
           
                $.myWindow.setMyWindow("FinanceReceipt", window);

                $.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '选择承兑',
                    width: 800,
                    height: 500,
                    onClose: function () {

                    }
                });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server" method="post">
            <div style="text-align: center; margin-top: 30px">
                <table id="editTable" style="margin: 0 auto;">
                    <tr>
                        <td style="width: 100px;">付款人:</td>
                        <td style="width: 200px;">
                            <input class="easyui-textbox" id="Payer" name="Payer" data-options="required:true,width:200,validType:'length[1,50]',missingMessage:'付款人不能为空'" /></td>
                        <td style="width: 100px;">账户性质:</td>
                        <td>
                            <input class="easyui-combobox" id="AccountProperty" name="AccountProperty" data-options="valueField:'Value',textField:'Text',required:true, width:200",missingMessage:'账户性质不能为空' />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px;">承兑票号/流水号：</td>
                        <td style="width: 200px;">
                            <input class="easyui-textbox" id="SeqNo" name="SeqNo" data-options="required:true,width:200,validType:['seqNo','length[1,50]'],missingMessage:'流水号不能为空'" />
                        </td>
                        <td>
                             <a id="btnAcceptance" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ChooseAcceptance()">承兑汇票</a>
                        </td>
                    </tr>
                    <tr>
                        <td>收款类型:</td>
                        <td>
                             <select id="AccountCatalog" name="AccountCatalog" class="easyui-combotree" style="width: 200px;" data-options="required:true,"></select>
                        </td>
                        <td>收款方式：</td>
                        <td>
                           <%-- <select class="easyui-combobox" id="PaymentType" name="PaymentType" data-options="required:true,width:200,editable:false,missingMessage:'收款方式不能为空'">
                                <option value="<%=Needs.Ccs.Services.Enums.PaymentType.TransferAccount.GetHashCode()%>">转账</option>
                                <option value="<%=Needs.Ccs.Services.Enums.PaymentType.Cash.GetHashCode()%>">现金</option>
                                <option value="<%=Needs.Ccs.Services.Enums.PaymentType.Check.GetHashCode()%>">支票</option>
                                <option value="<%=Needs.Ccs.Services.Enums.PaymentType.AcceptanceBill.GetHashCode()%>">银行承兑</option>
                            </select>--%>
                            <input class="easyui-combobox" id="PaymentType" name="PaymentType" data-options="required:true,width:200,editable:false,valueField:'Value',textField:'Text',panelHeight:'120px',missingMessage:'收款方式不能为空'"  />
                        </td>
                    </tr>
                    <tr>
                        <td>收款日期:</td>
                        <td>
                            <input class="easyui-datebox" id="Date" name="Date" data-options="required:true,editable:false, width:200,missingMessage:'收款日期不能为空'" /></td>
                        <td>金额：</td>
                        <td>
                            <input class="easyui-numberbox" id="Amount" name="Amount" data-options="required:true,min:0,precision: 2,width:200,missingMessage:'金额不能为空'" />
                        </td>
                    </tr>
                    <tr>
                        <td>币种：</td>
                        <td>
                            <input class="easyui-combobox" id="Currency" name="Currency" data-options="valueField:'Value',textField:'Text',required:true, width:200",missingMessage:'币种不能为空' />
                        </td>
                        <td>汇率：</td>
                        <td>
                            <input class="easyui-textbox" id="Rate" name="Rate" data-options="width:200" />
                        </td>
                    </tr>
                    <tr>
                        <td>收款金库：</td>
                        <td>
                            <input class="easyui-combobox" id="FinanceVault" name="FinanceVault" data-options="required:true,valueField:'Value',textField:'Text',width:200",missingMessage:'收款金库不能为空' />
                        </td>
                        <td>收款账户：</td>
                        <td>
                            <input class="easyui-combogrid" id="Account" name="Account" data-options="required:true,valueField:'Value',textField:'Text', width:200,editable:false,missingMessage:'收款账户不能为空'" />
                        </td>
                    </tr>
                    <tr>
                        <td>账号：</td>
                        <td>
                            <input class="easyui-textbox" id="BankAccount" name="BankAccount" data-options="width:200,disabled:true " />
                        </td>
                        <td>收款人：</td>
                        <td>
                            <input class="easyui-textbox" id="Admin" name="Admin" data-options="width:200,disabled:true" />
                        </td>
                    </tr>
                    <tr>
                        <td>备注:</td>
                        <td colspan="3">
                            <input class="easyui-textbox" data-options="multiline:true,width:500,height:50, validType: 'length[1,250]',tipPosition:'bottom'" id="Summary" name="Summary" />
                        </td>
                        <td></td>
                    </tr>                   
                </table>
            </div>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="SaveCheck()">保存</a>
        <a id="btnBack" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Cancel()">取消</a>
    </div>
</body>
</html>
