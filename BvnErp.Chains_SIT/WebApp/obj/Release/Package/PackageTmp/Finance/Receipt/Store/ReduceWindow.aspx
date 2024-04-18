<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReduceWindow.aspx.cs" Inherits="WebApp.Finance.Receipt.Store.ReduceWindow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>减免弹框</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var ReceivableID = '<%=this.Model.ReceivableID%>';
        var FeeTypeShowName = '<%=this.Model.FeeTypeShowName%>';
        var ReceivableAmount = '<%=this.Model.ReceivableAmount%>';

        $(function () {
            $("#FeeTypeShowName").html(FeeTypeShowName);

            $("#reduce-number").keyup(function () {
                $(this).val($(this).val().replace(/[^0-9.]/g, ''));
                $(this).val(sliceDecimal($(this).val()));
            });

        });

        //如果该字符串有两个小数点,就返回第二个小数点之前的字符串,否则返回原字符串
        function sliceDecimal(origin) {
            if (origin == null || origin.length <= 0) {
                return "";
            }
            ////没有小数点就返回原字符串
            //if (origin.search(".") == -1) {
            //    return origin;
            //}
            //如果只有一个小数点,返回原字符串. 如果有两个小数点,就返回第二个小数点之前的字符串
            var dotLocArr = [];
            for (var i = 0; i < origin.length; i++) {
                if (origin[i] == '.') {
                    dotLocArr.push(i);
                }
            }
            //没有小数点就返回原字符串
            if (dotLocArr.length == 0) {
                return origin;
            }
            if (dotLocArr.length == 1) {
                //只有一个小数点,如果小数点后有两位以上小数位,则切掉再往后的小数位
                if (origin.length - 1 <= dotLocArr[0] + 2) {
                    return origin;
                }
                return origin.slice(0, dotLocArr[0] + 3);
            }
            //有两个或以上个数的小数点
            return origin.slice(0, dotLocArr[1]);
        }

        //确定减免按钮
        function Ok() {
            var reduceNumber = $("#reduce-number").val().trim();
            if (reduceNumber == null || reduceNumber == '') {
                $.messager.alert('提示', '减免金额不能为空', 'info');
                return;
            }
            if (Number(reduceNumber) <= 0 || Number(reduceNumber) > Number(ReceivableAmount)) {
                $.messager.alert('提示', '减免金额只能在0~' + ReceivableAmount + '之间', 'info');
                return; 
            }

            var url = location.pathname + '?action=Reduce';
            var params = {
                "ReceivableID": ReceivableID,
                "ReduceNumber": reduceNumber,
            };

            MaskUtil.mask();
            $.post(url, params, function (res) {
                MaskUtil.unmask();
                var resData = JSON.parse(res);
                if (resData.success == "true") {
                    $.messager.alert('提示', '操作成功', 'info', function () {
                        $.myWindow.close();
                    });
                } else {
                    $.messager.alert('提示', resData.message);
                }
            });

        }

        //取消
        function Cancel() {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div>
        <div style="font-size: 12px; padding:5px; margin-top: 45px; margin-left: 55px; height: 95px;">
            <span id="FeeTypeShowName"></span>
            <span>减免：</span>
            <input id="reduce-number" type="text" class="textbox-text" style="border-radius:5px;"/>
        </div>
        <div data-options="region:'south',border:false," style="padding:5px; text-align:right; background-color:#fafafa;">
            <a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Ok()">确定</a>
            <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Cancel()">取消</a>
        </div>
    </div>
</body>
</html>
