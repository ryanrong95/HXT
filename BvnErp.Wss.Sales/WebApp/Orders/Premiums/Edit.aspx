<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Orders.Premiums.Edit" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>添加附加费用</title>
    <uc:EasyUI runat="server" />
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.menu = '订单列表';
    </script>
    <script>
        $(function () {
            $('#btn_save').click(function () {
                $('#form1').form({
                    onSubmit: function (param) {
                        var isValid = $(this).form('validate');
                        if (!isValid) {
                            $.messager.alert('提示', '请修改未正确输入项');
                            return false;
                        }
                        var price = $('#_price').val();
                        if (price <= 0) {
                            $.messager.alert('提示', '金额不能为0');
                            return false;
                        }

                        return isValid;
                    },
                    success: function (data) {
                        var data = eval('(' + data + ')');
                        if (data.success) {
                            $.messager.alert('提示', '添加成功', function () {
                                $.myWindow.close();
                            });
                        }
                        else {
                            if (data.code == -1) {
                                $.messager.alert('提示', '金额不能为0');
                            }
                            else {
                                $.messager.alert('提示', '添加失败');
                            }
                        }
                    }
                });
                //$('#form1').submit();
            });

        });
    </script>
</head>
<body>
    <div class="easyui-panel" title="添加附加价值" style="width: 100%; padding: 10px;">
        <form id="form1" action="?action=save" method="post">
            <table class="liebiao">
                <tr>
                    <th style="width: 120px;">附加费名称</th>
                    <td>
                        <input type="hidden" id="id" name="id" value="<%=Order.ID %>" />
                        <input type="text" id="_name" class="easyui-validatebox" data-options="required:true" name="_name" value="" />
                    </td>
                </tr>
                <tr>
                    <th>金额</th>
                    <td>
                        <input type="text" id="_price" class="easyui-numberbox" name="_price" data-options="precision:4,max:100000" style="text-align: left;" />
                    </td>
                </tr>
                <tr>
                    <th>说明</th>
                    <td>
                        <input class="easyui-textbox" id="_summary" name="_summary" data-options="multiline:true" style="width:300px; height:100px;" />
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

