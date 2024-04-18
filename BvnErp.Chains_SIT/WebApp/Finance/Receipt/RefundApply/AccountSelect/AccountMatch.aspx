<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountMatch.aspx.cs" Inherits="WebApp.Finance.Receipt.RefundApply.AccountMatch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../../Scripts/Ccs.js"></script>
    <link href="../../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var receipt = eval('(<%=this.Model.Receipt%>)');
        $(function () {

            var ReceiptID = getQueryString("ReceiptID");
            $("#ReceiptID").val(ReceiptID);
            var ApplyID = getQueryString("ApplyID");
            $("#ApplyID").val(ApplyID);

            $("#Payer").text(receipt.Payer);
            $("#FeeType").text(receipt.FeeType);
            $("#ReceiptType").text(receipt.ReceiptType);
            $("#Date").text(receipt.ReceiptDate);
            $("#Amount").text(receipt.Amount);
            $("#Currency").text(receipt.Currency);
            $("#SeqNo").text(receipt.SeqNo);
            $("#FinanceVault").text(receipt.Vault);
            $("#Account").text(receipt.AccountName);
            $("#BankAccount").text(receipt.BankAccount);
           


            $("#PayeeName").next().find("a").click(function () {
                var url = location.pathname.replace(/AccountMatch.aspx/ig, '../../../AcceptanceBill/Payee/PayeeList.aspx') + '?From=select&WindowName=RefundApplyAccountMatch';

                $.myWindow.setMyWindow("RefundApplyAccountMatch", window);

                $.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '选择退款人',
                    width: 800,
                    height: 500,
                    onClose: function () {

                    }
                });
            });



        });


        function Cancel() {
            $.myWindow.close();
        }

        function NormalClose() {
            $.myWindow.close();
        }

        function Match() {
            MaskUtil.mask();
            $.post('?action=Approve', {
                ApplyID: $("#ApplyID").val(),
                PayeeAccountID: $("#PayeeAccountID").val(),
                PayeeAccountNo: $("#PayeeAccount").textbox("getValue"),
                PayeeName: $("#PayeeName").textbox("getValue"),
                PayeeBank:$("#PayeeBank").textbox("getValue")
            }, function (res) {
                MaskUtil.unmask();
                var result = JSON.parse(res);
                if (result.success) {
                    var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                        NormalClose();
                    });
                    alert1.window({
                        modal: true, onBeforeClose: function () {
                            NormalClose();
                        }
                    });
                } else {
                    $.messager.alert('提示', result.message, 'info', function () {

                    });
                }
            });
        }
    </script>

</head>
<body class="easyui-layout">
    <div style="margin-top: 10px; margin-left: 1%; float: left; width: 370px;">
        <!-- 信息列 -->
        <div style="float: left; width: 370px;">
            <div class="big-row-one">
                <div class="easyui-panel" title="退款信息" style="height: 370px;">
                    <form id="form1">
                        <div class="sub-container left-block-one">
                            <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="lbl">收款方名称：</td>
                                    <td>
                                        <input class="easyui-textbox" id="PayeeName" data-options="validType:'length[1,50]',width: 250,required:true,editable:false,buttonText:'选择'," />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl">收款方账号：</td>
                                    <td>
                                        <input class="easyui-textbox" id="PayeeAccount" data-options="validType:'length[1,50]',width: 250,required:true,readonly:true," />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl">收款方银行：</td>
                                    <td>
                                        <input class="easyui-textbox" id="PayeeBank" data-options="validType:'length[1,50]',width: 250,required:true,readonly:true," />
                                        <input id="PayeeAccountID" type="hidden" />
                                        <input id="ReceiptID" type="hidden" />
                                        <input id="ApplyID" type="hidden" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
    <div style="margin-top: 10px; margin-left: 1%; float: left; width: 300px;">
        <div class="big-row-one">
            <div class="easyui-panel" title="收款信息" style="height: 370px;">

                <div class="sub-container left-block-one">
                    <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                        <tr>
                            <td class="lbl">付款人：</td>
                            <td>
                                <label id="Payer" name="Payer" data-options="height:15,width:250px"></label>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">收款类型：</td>
                            <td>
                                <label id="FeeType" name="FeeType" data-options="height:20,width:200px"></label>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">收款方式：</td>
                            <td>
                                <label id="ReceiptType" name="ReceiptType" data-options="height:20,width:200px"></label>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">收款日期：</td>
                            <td>
                                <label id="Date" name="Date" data-options="height:20,width:200px"></label>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">金额：</td>
                            <td>
                                <label id="Amount" name="Amount" data-options="height:20,width:200px"></label>
                            </td>
                        </tr>
                       <tr>
                           <td class="lbl">币种：</td>
                            <td>
                                <label id="Currency" name="Currency" data-options="height:20,width:200px"></label>
                            </td>
                       </tr>
                        <tr>
                            <td class="lbl">流水号：</td>
                            <td>
                                <label id="SeqNo" name="SeqNo" data-options="height:20,width:200px"></label>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">收款金库：</td>
                            <td>
                                <label id="FinanceVault" name="FinanceVault" data-options="width:250px"></label>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">收款账户：</td>
                            <td>
                                <label id="Account" name="Account" data-options="height:15px,width:250px"></label>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">收款账号：</td>
                            <td>
                                <label id="BankAccount" name="BankAccount" data-options="height:20,width:250px"></label>
                            </td>
                        </tr>
                    </table>
                </div>

            </div>
        </div>
    </div>
    <div id="divApply" style="float: left; margin-top: 20px">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Match()" style="margin-left: 300px">保存</a>
        <a id="btnBack" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Cancel()">取消</a>
    </div>
</body>
</html>

