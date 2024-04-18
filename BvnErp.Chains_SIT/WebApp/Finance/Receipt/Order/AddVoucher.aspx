<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddVoucher.aspx.cs" Inherits="WebApp.Finance.Receipt.Order.AddVoucher" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>添加抵用券</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var FinanceReceiptId = '<%=this.Model.FinanceReceiptId%>';

        $(function () {

        });

        //校验抵用券号
        function CheckVoucher() {
            var InputVoucherID = $('#InputVoucherID').textbox('getValue');
            InputVoucherID = InputVoucherID.trim();
            $('#InputVoucherID').textbox('setValue', InputVoucherID);

            if (InputVoucherID.length <= 0) {
                $("#checkResult").html('请输入抵用券号');
                $("#checkResult").css('color', 'red');
                $('#btnAdd').linkbutton('disable');
                var t1 = window.setTimeout(function () {
                    $("#checkResult").html('');
                }, 1000);
                return;
            }

            var params = {
                InputVoucherID: InputVoucherID,
            };

            MaskUtil.mask();
            $.post('?action=CheckVoucher', params, function (res) {
                MaskUtil.unmask();
                var resData = JSON.parse(res);
                if (resData.success == "true") {
                    $("#checkResult").html(resData.message);
                    $("#checkResult").css('color', 'green');
                    $('#btnAdd').linkbutton('enable');
                } else {
                    $("#checkResult").html(resData.message);
                    $("#checkResult").css('color', 'red');
                    $('#btnAdd').linkbutton('disable');
                }
            });
        }

        //添加抵用券
        function Add() {
            var InputVoucherID = $('#InputVoucherID').textbox('getValue');
            InputVoucherID = InputVoucherID.trim();
            $('#InputVoucherID').textbox('setValue', InputVoucherID);

            if (InputVoucherID.length <= 0) {
                $("#checkResult").html('请输入抵用券号');
                $("#checkResult").css('color', 'red');
                $('#btnAdd').linkbutton('disable');
                var t1 = window.setTimeout(function () {
                    $("#checkResult").html('');
                }, 1000);
                return;
            }

            var params = {
                InputVoucherID: InputVoucherID,
                FinanceReceiptId: FinanceReceiptId,
            };

            MaskUtil.mask();
            $.post('?action=Add', params, function (res) {
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
    <div style="margin-top:25px;margin-left:40px">
        <span style="font-size: 14px;">抵用券号：</span>
        <span>
            <input class="easyui-textbox" id="InputVoucherID" data-options="" style="height: 26px; width: 200px"" />
        </span>
        <span>
            <a class="easyui-linkbutton" data-options="" onclick="CheckVoucher()" style="margin-left: 10px; height: 26px; width: 45px;">校验</a>
        </span>
    </div>
    <div id="checkResult" style="margin-left: 140px; margin-top: 5px; font-size: 14px; height: 20px;"></div>
    <div style="margin-top: 18px;">
        <a id="btnAdd" class="easyui-linkbutton" data-options="iconCls:'icon-ok',disabled:true," onclick="Add()" style="margin-left: 245px; height: 26px; width: 60px;">确定</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Cancel()" style="margin-left: 5px; height: 26px; width: 60px;">取消</a>
    </div>
</body>
</html>
