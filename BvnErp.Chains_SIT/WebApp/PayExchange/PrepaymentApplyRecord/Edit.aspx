<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.PayExchange.PrepaymentApplyRecord.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var payID = '<%=this.Model.PayID%>';
        var orderID = '<%=this.Model.OrderID%>';
        var remainingAmount = '<%=this.Model.RemainingAmount%>';
        var DeclarePrice = '<%=this.Model.DeclarePrice%>';
        var PaidExchangeAmount = '<%=this.Model.PaidExchangeAmount%>';
        var KSQ = parseFloat(DeclarePrice) - parseFloat(PaidExchangeAmount);//可申请金额
        var oldOrderID ='<%=this.Model.OldOrderID%>';
        $(function () {

        });

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }
        function Save() {
            debugger;
            var amount = parseFloat($('#Amount').textbox('getValue').trim());
            if (amount == "") {
                $.messager.alert('提示', '匹配金额不能为空!');
                return;
            }
            //else {
            //    var regu = "^[0-9]+[\.][0-9]{0,3}$";  
            //    var re = new RegExp(regu);  
            //    if (!re.test(amount)) {  
            //    $.messager.alert('提示', '匹配金额只能是数字!');
            //    return; 
            //    }  
            //}
            if (amount > parseFloat(KSQ.toFixed(2))) {
                $.messager.alert('提示', '匹配金额不能大于剩余订单可申请金额！');
                return;
            }
            if (amount > parseFloat(remainingAmount)) {
                $.messager.alert('提示', '匹配金额不能大于剩余预付金额！!');
                return;
            }
            var data = new FormData($('#form1')[0]);
            data.append("Amount", amount);
            data.append("OrderID", orderID);
            data.append("PayID", payID);
            data.append("OldOrderID", oldOrderID);
            data.append("RemainingAmount", remainingAmount);
            $.ajax({
                url: '?action=Save',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        $.myWindow.close();
                        //$.messager.alert('消息', res.message, '', function () {
                        //    var url = location.pathname.replace(/Edit.aspx/ig, 'Match.aspx');
                        //});
                    }
                    else {
                        $.messager.alert('错误', res.message);
                    }
                }
            })
        }

    </script>
    <style>
        #dlg-buttons {
            height: 40px;
            text-align: right;
            margin-top: 30px;
            padding-right: 40px;
            background-color: #F3F3F3;
            vertical-align: central;
        }

            #dlg-buttons a {
                margin-top: 5px;
                margin-bottom: 5px;
            }
    </style>
</head>
<body class="easyui-layout">
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true,closable:true,onClose:function(){$.myWindow.close();}" style="margin-top: 10px">
        <form id="form1" runat="server" method="post">
            <div data-options="region:'center',border:false" style="text-align: center">
                <input id="Amount" class="easyui-textbox" data-options="validType:'length[1,50]',width: 90, height:40,required:true," />
            </div>

            <div id="dlg-buttons">
                <a id="btnSave" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-save'" runat="server" onclick="Save()">保存</a>
                <a id="btnCancel" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
            </div>
        </form>
    </div>
</body>
</html>
