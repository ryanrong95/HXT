<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="WebApp.Finance.Payment.PaymentOperators.Add" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">

        var avaiblePaymentOperators = eval('(<%=this.Model.AvaiblePaymentOperators%>)');

        $(function () {

            $("#Operator").combobox({
                data: avaiblePaymentOperators,
                required: true,
                valueField: 'AdminID',
                textField: 'AdminName',
                onChange: function (record) {
                    
                },
            });

        });

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }

        //确定
        function Save() {
            if (!$("#form1").form('validate')) {
                return;
            }

            var AdminID = $("#Operator").combobox('getValue');

            MaskUtil.mask();
            $.post('?action=Save', {
                AdminID: AdminID,
            }, function (result) {
                MaskUtil.unmask();
                var resultJson = JSON.parse(result);
                if (resultJson.success) {
                    $.messager.alert('消息', resultJson.message, 'info', function () {
                        $.myWindow.close();
                    });
                } else {
                    $.messager.alert('消息', resultJson.message, 'error', function () {

                    });
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table style="margin-top:50px;margin-left:100px">
                <tr>
                    <td>选择：</td>
                    <td>
                        <input class="easyui-combobox" id="Operator" name="Operator" panelHeight="120"
                            data-options="required:true,editable:false" style="height: 30px; width: 250px"" />
                    </td>
                </tr>
            </table>
            <table style="margin-top:10px;margin-left:10px;font-size:13px">
                <tbody>
                    <tr>
                        <td><div id="check-limit-country-result" style="word-break: break-all;width: 515px;"></div></td>
                    </tr>
                </tbody>
            </table>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnNext" class="easyui-linkbutton" data-options="iconCls:'icon-edit',width:70," onclick="Save()">确定</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel',width:70," onclick="Close()">取消</a>
    </div>
</body>
</html>
