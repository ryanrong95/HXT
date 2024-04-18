<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PayingEdit.aspx.cs" Inherits="WebApp.Finance.FundTransferApplies.PayingEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>金库账户编辑</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var AllData = eval('(<%=this.Model.AllData%>)');
        var FinanceVaultData = eval('(<%=this.Model.FinanceVaultData%>)');
        var isCommitted = false;//表单是否已经提交标识，默认为false
        var PageFunction = getQueryString("PageFunction");
        var PaymentType = eval('(<%=this.Model.PaymentType%>)');

        $(function () {
            if (PageFunction == "View") {
                $("#SaveButton").css('display', 'none');
                $("#CancelButton").css('display', 'none');
                $("#ApproveButton").css('display', 'none');
                $("#DenyButton").css('display', 'none');
            } else if (PageFunction == "Approve") {
                $("#SaveButton").css('display', 'none');
                $("#CancelButton").css('display', 'none');
            } else {
                $("#ApproveButton").css('display', 'none');
                $("#DenyButton").css('display', 'none');
            }

            //付款方式           
            $('#PayType').combobox({
                data: PaymentType,
                editable: false,
            });

            //初始化赋值
            if (AllData != null && AllData != "") {
                $("#OutVault").combobox({
                    data: FinanceVaultData
                });
                $("#OutVault").combobox("setValue", AllData["OutVault"]);
                $("#OutMoney").numberbox("setValue", AllData["OutMoney"]);
                $("#FromSeqNo").textbox("setValue", AllData["FromSeqNo"]);

                $("#InVault").combobox({
                    data: FinanceVaultData
                });
                $("#InVault").combobox("setValue", AllData["InVault"]);
                $("#InMoney").numberbox("setValue", AllData["InMoney"]);
                $("#Summary").textbox("setValue", AllData["Summary"]);

                $.post('?action=getAccounts', { VaultID: AllData["OutVault"] }, function (data) {
                    var accounts = JSON.parse(data);
                    $('#OutAccount').combobox({
                        data: accounts,
                    });
                    $('#OutAccount').combobox('setValue', AllData["OutAccount"]);
                });

                $.post('?action=getAccounts', { VaultID: AllData["InVault"] }, function (data) {
                    var accounts = JSON.parse(data);
                    $('#InAccount').combobox({
                        data: accounts,
                    });
                    $("#InAccount").combobox("setValue", AllData["InAccount"]);
                });

                $('#OutVault').combobox('disable');
                $('#OutAccount').combobox('disable');
                $("#OutMoney").numberbox('disable');
                $('#InVault').combobox('disable');
                $('#InAccount').combobox('disable');
                $("#InMoney").numberbox('disable');

                var ID = AllData["ID"];
                var data = new FormData($('#form1')[0]);
                data.append("ID", ID);
                $.ajax({
                    url: '?action=LoadLogs',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (data) {
                        showLogContent(data);
                    },
                    error: function (msg) {
                        alert("ajax连接异常：" + msg);
                    }
                });

            } else {
                $("#OutVault").combobox({
                    data: FinanceVaultData,
                    onSelect: function (record) {
                        $.post('?action=getAccounts', { VaultID: record.Value }, function (data) {
                            var accounts = JSON.parse(data);
                            $('#OutAccount').combobox({
                                data: accounts,
                            });
                        });
                    }
                });
                $("#InVault").combobox({
                    data: FinanceVaultData,
                    onSelect: function (record) {
                        $.post('?action=getAccounts', { VaultID: record.Value }, function (data) {
                            var accounts = JSON.parse(data);
                            $('#InAccount').combobox({
                                data: accounts,
                            });
                        });
                    }
                });
            }


            $("input", $("#FromSeqNo").next("span")).blur(function () {
                var fromSeqNo = $("#FromSeqNo").textbox("getValue");
                if (fromSeqNo != '') {
                    $('#DiscountInterest').textbox({ required: true });
                } else {
                    $('#DiscountInterest').textbox({ required: false });
                }
            });

            $("#DiscountInterest").numberbox({
                onChange: function (newVal, oldVal) {
                    var OutMoney = $("#OutMoney").numberbox("getValue");
                    var inMoney = OutMoney - newVal;
                    $("#InMoney").numberbox("setValue", inMoney);
                }
            });

            $("#QRCodeFee").numberbox({
                onChange: function (newVal, oldVal) {
                    var OutMoney = $("#OutMoney").numberbox("getValue");
                    var inMoney = OutMoney - newVal;
                    $("#InMoney").numberbox("setValue", inMoney);
                }
            });

            if (AllData["FromSeqNo"] != '') {
                $('#DiscountInterest').textbox({ required: true });
            }
        });
        //关闭弹出页面
        function Close() {
            $.myWindow.close();
        }
        //校验重复提交
        function CheckSubmit() {
            if (isCommitted == false) {
                isCommitted = true;//提交表单后，将表单是否已经提交标识设置为true
                return true;//返回true让表单正常提交
            } else {
                return false;//返回false那么表单将不提交
            }
        }
        //保存校验
        function Save() {
            //验证表单数据
            if (!Valid('form1')) {
                return;
            }
            var data = new FormData($('#form1')[0]);
            var InMoney = $("#InMoney").numberbox("getValue");
            if (AllData != null) {
                data.append('ID', AllData["ID"]);
                data.append('InAmount', InMoney);
            }
            MaskUtil.mask();
            $.ajax({
                url: '?action=Save',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    MaskUtil.unmask();
                    if (res.success) {
                        $.messager.alert('消息', res.message, 'info', function () {
                            Close();
                        });
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            });
        }

        //显示日志数据
        function showLogContent(data) {
            var str = "";//定义用于拼接的字符串
            $.each(data.rows, function (index, row) {
                if (row.Summary != null) {
                    str = "<p>" + row.CreateDate + "&nbsp;&nbsp;" + row.Summary + "</p>"
                }
                //追加到table中
                $("#LogContent").append(str);
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="width: 100%;">
                <tr>
                    <td class="lbl">
                        <span style="font-size: large">调出账户</span>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">调出金库：</td>
                    <td>
                        <input class="easyui-combobox" id="OutVault" name="OutVault" data-options="required:true,height:28,width:250,valueField:'Value',textField:'Text',panelHeight:'120px'" />
                    </td>
                    <td class="lbl">调出账户：</td>
                    <td>
                        <input class="easyui-combobox" id="OutAccount" name="OutAccount" data-options="required:true,height:28,width:250,valueField:'Value',textField:'Text',panelHeight:'120px'" />
                    </td>

                </tr>
                <tr>
                    <td class="lbl">调出金额：</td>
                    <td>
                        <input class="easyui-numberbox" id="OutMoney" name="OutMoney" data-options="required:true,height:28,width:250,validType:'length[1,150]',precision:2" />
                    </td>
                    <td class="lbl">调出流水号：</td>
                    <td>
                        <input class="easyui-textbox" id="OutSeqNo" name="OutSeqNo" data-options="required:true,height:28,width:250,validType:'length[1,250]'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">关联号码：</td>
                    <td>
                        <input class="easyui-textbox" id="FromSeqNo" name="FromSeqNo" data-options="height:28,width:250,validType:'length[1,250]'" />
                    </td>
                    <td class="lbl">贴现利息：</td>
                    <td>
                        <input class="easyui-numberbox" id="DiscountInterest" name="DiscountInterest" data-options="height:28,width:250,validType:'length[1,250]',precision:2" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">快捷支付费用：</td>
                    <td>
                        <input class="easyui-numberbox" id="QRCodeFee" name="QRCodeFee" data-options="height:28,width:250,validType:'length[1,250]',precision:2" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">
                        <span style="font-size: large">调入账户</span>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">调入金库：</td>
                    <td>
                        <input class="easyui-combobox" id="InVault" name="InVault" data-options="required:true,height:28,width:250,valueField:'Value',textField:'Text',panelHeight:'120px'" />
                    </td>
                    <td class="lbl">调入账户：</td>
                    <td>
                        <input class="easyui-combobox" id="InAccount" name="InAccount" data-options="required:true,height:28,width:250,valueField:'Value',textField:'Text',panelHeight:'120px'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">调入金额：</td>
                    <td>
                        <input class="easyui-numberbox" id="InMoney" name="InMoney" data-options="required:true,height:28,width:250,validType:'length[1,50]',precision:2" />
                    </td>
                    <td class="lbl">调入流水号：</td>
                    <td>
                        <input class="easyui-textbox" id="InSeqNo" name="InSeqNo" data-options="required:true,height:28,width:250,validType:'length[1,250]'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">
                        <span style="font-size: large">&nbsp</span>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">手续费金额：</td>
                    <td>
                        <input class="easyui-numberbox" id="Poundage" name="Poundage" data-options="required:false,height:28,width:250,validType:'length[1,50]',precision:2" />
                    </td>
                    <td class="lbl">手续费流水号：</td>
                    <td>
                        <input class="easyui-textbox" id="PoundageSeqNo" name="PoundageSeqNo" data-options="height:28,width:250,validType:'length[1,250]'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">付款方式：</td>
                    <td>
                        <input class="easyui-combobox" id="PayType" name="PayType"
                            data-options="valueField:'Value',textField:'Text',limitToList:true,required:true" style="width: 250px; height: 26px" />
                    </td>
                    <td class="lbl">付款日期：</td>
                    <td>
                        <input class="easyui-datebox" id="PayDate" name="PayDate"
                            data-options="required:true" style="width: 250px; height: 26px" />
                    </td>
                </tr>
            </table>
            <div style="margin-top: 5px; margin-left: 2px;">
                <div class="easyui-panel" title="日志记录" style="width: 100%;">
                    <div class="sub-container">
                        <div class="text-container" id="LogContent">
                        </div>
                    </div>
                </div>
            </div>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a class="easyui-linkbutton" id="SaveButton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a class="easyui-linkbutton" id="CancelButton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
        <a class="easyui-linkbutton" id="ApproveButton" data-options="iconCls:'icon-save'" onclick="Approve()">同意</a>
        <a class="easyui-linkbutton" id="DenyButton" data-options="iconCls:'icon-save'" onclick="Deny()">拒绝</a>
    </div>

</body>
</html>
