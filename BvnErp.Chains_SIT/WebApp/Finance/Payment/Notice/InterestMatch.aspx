<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InterestMatch.aspx.cs" Inherits="WebApp.Finance.Receipt.Notice.InterestMatch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>金库账户编辑</title>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <script type="text/javascript">

        var isCommitted = false;//表单是否已经提交标识，默认为false
        var PageFunction = getQueryString("PageFunction");
        $(function () {

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
          var orderID = $("#OrderID").textbox("getValue");      
            var id = getQueryString("ID");
            MaskUtil.mask();
            $.post('?action=match', { ID: id, OrderID: orderID }, function (data) {
                MaskUtil.unmask();
                var res = JSON.parse(data);
                if (res.success) {
                    $.messager.alert('消息', res.message, 'info', function () {
                        Close();
                    });
                } else {
                    $.messager.confirm('失败', res.message, function (success) {
                        Close();
                    });
                }
            });
        }

        function CheckOrderID() {
            var id = getQueryString("ID");
            var orderID = $("#OrderID").textbox("getValue");           
            if (orderID == "" || orderID == null) {
                $.messager.alert('提示', "请输入订单号!");
            }
            $.post('?action=OrderCheck', { ID: id, OrderID: orderID}, function (data) {
                 var res = JSON.parse(data);
                if (res.success) {
                    Save();
                } else {
                    $.messager.confirm('失败', res.message, function (success) {
                        Close();
                    });
                }
            });
        }



    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="width: 100%;">
                <tr>
                    <td class="lbl">订单编号：</td>
                    <td>
                        <input class="easyui-textbox" id="OrderID" name="OrderID" data-options="required:false,width:150,validType:'length[1,500]'" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a class="easyui-linkbutton" id="SaveButton" data-options="iconCls:'icon-save'" onclick="CheckOrderID()">保存</a>
        <a class="easyui-linkbutton" id="CancelButton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>

</body>
</html>
