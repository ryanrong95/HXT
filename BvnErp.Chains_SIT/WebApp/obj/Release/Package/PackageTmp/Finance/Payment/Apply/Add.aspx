<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="WebApp.Finance.Payment.Add" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">

        var FeeTypeData = eval('(<%=this.Model.FeeTypeData%>)');
        var PaymentTypeData = eval('(<%=this.Model.PaymentTypeData%>)');
        var CurrencyData = eval('(<%=this.Model.CurrencyData%>)');

        $(function () {
            //付款费用类型
            $('#FeeType').combobox({
                editable: false,
            });
            //付款方式
            $('#PayType').combobox({
                data: PaymentTypeData,
                editable: false,
                valueField: 'Key',
                textField: 'Value'
            });
            //币种
            $('#Currency').combobox({
                data: CurrencyData,
                editable: false,
                valueField: 'Key',
                textField: 'Value',
                onLoadSuccess: function () {
                    $('#Currency').combobox('setValue', 'CNY');
                }
            });
        });

        //关闭弹出页面
        function Close() {
            $.myWindow.close();
        }

        //提交申请
        function Submit() {
            //验证表单数据
            if (!Valid('form1')) {
                return;
            }

            var OrderID = getQueryString("OrderID")
            var data = new FormData($('#form1')[0]);
            data.append('OrderID', OrderID)
            data.append('PayeeName', $('#PayeeName').textbox('getValue'))
            data.append('BankAccount', $('#BankAccount').textbox('getValue'))
            data.append('BankName', $('#BankName').textbox('getValue'))
            data.append('FeeType', $('#FeeType').combobox('getValue'))
            data.append('FeeDesc', $('#FeeDesc').combobox('getValue'))
            data.append('Amount', $('#Amount').numberbox('getValue'))
            data.append('Currency', $('#Currency').combobox('getValue'))
            data.append('PayType', $('#PayType').combobox('getValue'))
            data.append('PayDate', $('#PayDate').datebox('getValue'))

            $.ajax({
                url: '?action=Submit',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
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
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server" method="post">
            <div>
                <table  style="margin: 0 auto; line-height: 35px;">
                    <tr>
                        <td class="lbl">收款人：</td>
                        <td>
                            <input class="easyui-textbox" id="PayeeName" data-options="required:true,height:26,width:200,validType:'length[1,25]'" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">收款账号：</td>
                        <td>
                            <input class="easyui-textbox" id="BankAccount" data-options="height:26,width:200,validType:'length[1,50]'" />
                        </td>
                        <td class="lbl">开户银行：</td>
                        <td>
                            <input class="easyui-textbox" id="BankName" data-options="height:26,width:200,validType:'length[1,50]'" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">费用类型：</td>
                        <td>
                            <select id="FeeType" class="easyui-combobox" style="height: 26px; width: 200px">
                                <option value="<%=Needs.Ccs.Services.Enums.FinanceFeeType.Incidental.GetHashCode()%>">杂费</option>
                                <%--<option value="<%=Needs.Ccs.Services.Enums.FinanceFeeType.Product.GetHashCode()%>">货款</option>
                                <option value="<%=Needs.Ccs.Services.Enums.FinanceFeeType.Tariff.GetHashCode()%>">关税</option>
                                <option value="<%=Needs.Ccs.Services.Enums.FinanceFeeType.AddedValueTax.GetHashCode()%>">增值税</option>--%>
                            </select>
                        </td>
                        <td class="lbl">费用描述：</td>
                        <td>
                            <%--<input class="easyui-textbox" id="FeeDesc" data-options="required:true,height:26,width:200,validType:'length[1,50]'" />--%>
                            <select id="FeeDesc" class="easyui-combobox" data-options="required:true,height:26,width:200,validType:'length[1,50]'">
                                <option>送货费</option>
                                <option>快递费</option>
                                <option>清关费</option>
                                <option>提货费</option>
                                <option>停车费</option>
                                <option>手续费</option>
                                <option>其它</option>
                            </select>
                        </td>
                    </tr>
                </table>
            </div>
            <div>
                <table style="margin: 0 auto; line-height: 35px">
                    <tr>
                        <td class="lbl">金额：</td>
                        <td>
                            <input class="easyui-numberbox" id="Amount" data-options="min:0,precision:4,required:true,height:26,width:200,validType:'length[1,18]'" />
                        </td>
                        <td class="lbl">币种：</td>
                        <td>
                            <input class="easyui-combobox" id="Currency"
                                data-options="required:true,height:26,width:200" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">付款方式：</td>
                        <td>
                            <input class="easyui-combobox" id="PayType"
                                data-options="required:true,height:26,width:200" />
                        </td>
                        <td class="lbl">付款日期：</td>
                        <td>
                            <input class="easyui-datebox" id="PayDate" data-options="required:true,height:26,width:200,validType:'length[1,50]'" />
                        </td>
                    </tr>
                </table>
            </div>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Submit()">保存</a>
        <a id="btnBack" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
