<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FinancePaymentDetails.aspx.cs" Inherits="WebApp.Finance.Payment.FinancePaymentDetails" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../Content/Ccs.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/chainsupload.js"></script>
    <script type="text/javascript">

        var FinancePaymentData = eval('(<%=this.Model.FinancePaymentData%>)');       

        $(function () {
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
                    if (data.length > 0 && FinancePaymentData != "") {
                        $("#AccountCatalog").combotree('setValue', FinancePaymentData.PayFeeType);
                    }
                },
            });

            Init()
        });

        function Init() {
            if (FinancePaymentData != null) {
                $('#PayFeeType').combobox('setValue', FinancePaymentData['PayFeeType']);
                $('#PayeeName').textbox('setValue', FinancePaymentData['PayeeName']);
                $('#Account').textbox('setValue', FinancePaymentData['BankAccount']);
                $('#BankName').textbox('setValue', FinancePaymentData['BankName']);                
                $('#Amount').textbox('setValue', FinancePaymentData['Amount']);
                $('#Currency').combobox('setValue', FinancePaymentData['Currency']);
                $('#PayType').combobox('setValue', FinancePaymentData['PayType']);
                $('#PayDate').datebox('setValue', FinancePaymentData['PayDate']);
                $('#SeqNo').textbox('setValue', FinancePaymentData['SeqNo']);
                $('#Payer').combobox('setValue', FinancePaymentData['Payer']);
                $('#ExchangeRate').textbox('setValue', FinancePaymentData['ExchangeRate']);
                $('#FinanceVault').combobox('setValue', FinancePaymentData['FinanceVault']);
                $('#FinanceAccount').combobox('setValue', FinancePaymentData['FinanceAccount']);
            }
        }

        function Back() {
            $.myWindow.close();
        }
    </script>
    <style>
        .easyui-combobox {
            width: 250px;
            height: 26px;
        }

        .easyui-textbox {
            width: 250px;
            height: 26px;
        }

        .easyui-datebox {
            width: 250px;
            height: 26px;
        }

        .lab {
            padding-left: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="margin: 5px auto;">
            <table style="line-height: 36px; margin: 0 auto;font-size:12px">
                <tr>
                    <td class="lbl">费用类型：</td>
                    <td>
                      <%--  <input class="easyui-combobox" id="PayFeeType" name="PayFeeType"
                            data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,disabled:true" style="width: 250px; height: 26px"  />--%>
                        <select id="AccountCatalog" name="AccountCatalog" class="easyui-combotree" style="width: 200px;" data-options="required:true,disabled:true"></select>
                    </td>
                </tr>
                <tr>
                     <td class="lbl">收款方账号：</td>
                    <td>
                      <%--  <input class="easyui-textbox" id="BankAccount" name="BankAccount"
                            data-options="validType:'length[1,25]',disabled:true"  style="width: 250px; height: 26px" />--%>
                         <input class="easyui-textbox" id="Account" name="Account"  data-options="required:true,validType:'length[1,50]',multiline:true,disabled:true" style="width: 250px; height: 26px"/>                         
                    </td>                  
                </tr>
                <tr>
                     <td class="lbl">收款方名称：</td>
                    <td>
                        <input class="easyui-textbox" id="PayeeName" name="PayeeName"
                            data-options="required:true,validType:'length[1,50]',multiline:true,disabled:true"  style="width: 250px; height: 26px" />
                    </td>
                    <td class="lbl lab">收款方银行：</td>
                    <td>
                        <input class="easyui-textbox" id="BankName" name="BankName"
                            data-options="validType:'length[1,100]',multiline:true,disabled:true" style="width: 250px; height: 26px"  />
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
                            data-options="required:true,validType:'length[1,18]',disabled:true"  style="width: 250px; height: 26px" />
                    </td>
                    <td class="lbl lab">付款方式：</td>
                    <td>
                        <input class="easyui-combobox" id="PayType" name="PayType"
                            data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,disabled:true" style="width: 250px; height: 26px"  />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">币种：</td>
                    <td>
                        <input class="easyui-combobox" id="Currency" name="Currency"
                            data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,disabled:true"  style="width: 250px; height: 26px" />
                    </td>
                    <td class="lbl lab">付款汇率：</td>
                    <td>
                        <input class="easyui-textbox" id="ExchangeRate" name="ExchangeRate"
                            data-options="required:true,validType:'length[1,18]',disabled:true" style="width: 250px; height: 26px"  />
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
                            data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,disabled:true" style="width: 250px; height: 26px"  />
                    </td>
                    <td class="lbl lab">付款日期：</td>
                    <td>
                        <input class="easyui-datebox" id="PayDate" name="PayDate"
                            data-options="required:true,disabled:true"  style="width: 250px; height: 26px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">付款金库：</td>
                    <td>
                        <input class="easyui-combobox" id="FinanceVault" name="FinanceVault"
                            data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,disabled:true"  style="width: 250px; height: 26px" />
                    </td>
                    <td class="lbl lab">付款账户：</td>
                    <td>
                        <input class="easyui-combobox" id="FinanceAccount" name="FinanceAccount"
                            data-options="valueField:'Value',textField:'Text',limitToList:true,required:true,disabled:true"  style="width: 250px; height: 26px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">付款流水号：</td>
                    <td>
                        <input class="easyui-textbox" id="SeqNo" name="SeqNo"
                            data-options="required:true,validType:'length[1,50]',disabled:true"  style="width: 250px; height: 26px" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
