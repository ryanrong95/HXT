<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Orders.Delivery.Edit" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>修改发货比例</title>
    <uc:EasyUI runat="server" />

    <script>
        $(function () {
            $('#btn_save').click(function () {
                $('#form1').form({
                    queryParams: { action: 'save' },
                    onSubmit: function (param) {
                        var isValid = $(this).form('validate');
                        if (!isValid) {
                            $.messager.alert('提示', '请修改未正确输入项');
                            return false;
                        }
                        var price = $('#_ratio').val();
                        if (price < 0) {
                            $.messager.alert('提示', '比例不能小于0');
                            return false;
                        }

                        return isValid;
                    },
                    success: function (data) {

                        $.messager.alert('提示', '提交成功', function () {
                            $.myWindow.close();
                        });

                    }
                });
               // $('#form1').submit();
            });

        });
    </script>
</head>
<body>
    <div class="easyui-panel" title="修改发货比例" style="width: 100%; padding: 10px;" data-options="border:false,fit:true,iconCls:'icon-edit',closable:true,onClose:function(){$.myWindow.close();}">
        <form id="form1">
            <table class="liebiao">
                <tr>
                    <th>发货比例</th>
                    <td>
                        <input type="hidden" name="id" value="<%=Order.ID %>" />
                        <input type="text" id="_ratio" class="easyui-numberbox" name="_ratio" data-options="precision:2,max:100,min:0" value="<%=Order.DeliveryRatio %>" style="text-align: left;" />
                        %
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: center;">
                        <%--<asp:Button ID="Save" runat="server" CssClass="easyui-linkbutton" Text="保存" OnClick="Save_Click" Style="width: 100px;" />--%>
                        <button class="easyui-linkbutton" id="btn_save">提交</button>
                        <a href="javascript:void(0)" class="easyui-linkbutton" onclick="$.myWindow.close();">关闭</a>
                    </td>
                </tr>
            </table>
        </form>
    </div>
</body>
</html>
