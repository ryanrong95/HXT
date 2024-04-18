<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OneKeyReceipt.aspx.cs" Inherits="WebApp.Finance.Receipt.Order.OneKeyReceipt" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var OrderIDs = '<%=this.Model.OrderIDs%>';
        var financeReceiptId = '<%=this.Model.financeReceiptId%>';
        var OrderIDArray = [];

        $(function () {
            OrderIDArray = OrderIDs.split(",");
            $("#orderCount").html(OrderIDArray.length);




        });

        //执行一键收款
        function DoOneKeyReceipt() {
            var ReceiptTypeItems = $('input[name=ReceiptType]:checked');
            var ReceiptTypeArray = [];
            for (var i = 0; i < ReceiptTypeItems.length; i++) {
                ReceiptTypeArray.push($(ReceiptTypeItems[i]).val());
            }

            if (ReceiptTypeArray.length <= 0) {
                $.messager.alert('提示', '请选择需要收款的类型');
                return;
            }

            var params = {
                ReceiptTypeArray: JSON.stringify(ReceiptTypeArray),
                OrderIDArray: JSON.stringify(OrderIDArray),
                FinanceReceiptId: financeReceiptId,
            };

            MaskUtil.mask();
            $.post('?action=DoOneKeyReceipt', params, function (res) {
                MaskUtil.unmask();
                var resData = JSON.parse(res);
                if (resData.success == "true") {
                    $.messager.alert('提示', resData.message, 'info', function () {
                        $.myWindow.close();
                    });
                } else {
                    $.messager.alert('提示', resData.message);
                    return;
                }
            });
        }

        //取消，关闭窗口
        function Cancel() {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div style="margin-top:25px; margin-left:40px; font-size: 14px;">
        <span>请选择需要收款的类型：</span>
        <span>
            <input type="checkbox" name="ReceiptType" value="1" id="ProductFee" style="display: none;"/>
            <label for="ProductFee">货款</label>
        </span>
        <span style="margin-left: 5px;">
            <input type="checkbox" name="ReceiptType" value="2" id="ShuiDaiFee" style="display: none;"/>
            <label for="ShuiDaiFee">税代费</label>
        </span>
    </div>
    <div style="margin-top: 18px; margin-left:40px; font-size: 14px;">
        <span>确定要对已勾选的</span><span id="orderCount"></span><span>个订单进行收款操作吗？</span>
    </div>
    <div style="margin-top: 45px;">
        <a class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="DoOneKeyReceipt()" style="margin-left: 245px; height: 26px; width: 60px;">确定</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Cancel()" style="margin-left: 5px; height: 26px; width: 60px;">取消</a>
    </div>
</body>
</html>
