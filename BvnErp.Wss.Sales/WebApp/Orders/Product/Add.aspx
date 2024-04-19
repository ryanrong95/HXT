<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="WebApp.Orders.Product.Add" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>添加产品项</title>
    <uc:EasyUI runat="server"></uc:EasyUI>

    <script>
        $(function () {

            $('#btn_save').click(function () {
                $('#form1').form({
                    queryParams: { action: 'save' },
                    onSubmit: function (param) {

                        var isValid = $(this).form('enableValidation').form('validate');
                        if (!isValid) {
                            $.messager.alert('提示', '请输入必填项');
                            return false;
                        }
                        return isValid;
                    },
                    success: function (data) {
                        var data = eval('(' + data + ')');
                        if (data.success == true) {
                            $.messager.alert('提示', '保存成功', function () {
                                $.myWindow.close();
                            });
                        }
                        else {
                            $.messager.alert('提示', '保存失败');
                        }
                    }
                });
                $('#form1').submit();
            });
        });
    </script>

</head>
<body>
    <div class="easyui-panel" title="新增产品项" style="width: 100%; padding: 10px;">
        <form id="form1">
            <input type="hidden" name="uid" value="<%=uid %>" />
            <input type="hidden" name="oid" value="<%=oid %>" />

            <table class="liebiao">
                <tr>
                    <th>型号：</th>
                    <td>
                        <input type="text" style="width: 200px;" name="_title" class="easyui-textbox" data-options="required:true,validType:'length[0,200]'" />
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <th>供应商：</th>
                    <td>
                        <input type="text" style="width: 200px;" name="_supplier" class="easyui-textbox" data-options="required:true,validType:'length[0,200]'" />
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <th>厂家：</th>
                    <td>
                        <input type="text" style="width: 200px;" name="_mf" class="easyui-textbox" data-options="required:true,validType:'length[0,200]'" />
                    </td>
                    <td></td>
                </tr>

                <tr>
                    <th>批号：</th>
                    <td>
                        <input type="text" style="width: 200px;" name="_batch" class="easyui-textbox" data-options="required:true,validType:'length[0,200]'" />
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <th>交货地：</th>
                    <td>
                        <%=ViewState["district"] %>
                        
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <th>币种：</th>
                    <td>
                         <%=ViewState["currency"] %>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <th>价格：</th>
                    <td>
                        <input type="text" id="_price" name="_price" style="width: 200px;" class="easyui-numberbox" data-options="precision:4,required:true,max:100000,min:0" />
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <th>数量：</th>
                    <td>
                        <input type="text" id="_count" name="_count" style="width: 200px;" class="easyui-numberbox" data-options="precision:0,required:true,min:1,max:10000000," />
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <th>货期（工作日）：</th>
                    <td>
                        <input type="text" name="_leadtime" style="width: 200px;" class="easyui-textbox" data-options="required:true,validType:'length[0,200]'" />
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <th>封装：</th>
                    <td>
                        <input type="text" name="_package" style="width: 200px;" class="easyui-textbox" data-options="validType:'length[0,200]'" />
                    </td>
                    <td></td>
                </tr>
                <%-- <tr>
                    <th>属性：</th>
                    <td>
                        <input type="text" class="easyui-validatebox" data-options="required:true" />
                    </td>
                    <td></td>
                </tr>--%>
                <tr>
                    <th>备注：</th>
                    <td>
                        <input name="_summary" type="text" class="easyui-textbox" data-options="multiline:true,validType:'length[0,300]'" style="width: 300px; height: 100px;" />
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="3" style="text-align: center;">
                        <a href="javascript:void(0)" id="btn_save" class="easyui-linkbutton">提交</a>
                        <a href="javascript:void(0)" class="easyui-linkbutton" onclick="$.myWindow.close();">关闭</a>
                    </td>
                </tr>
            </table>
        </form>
    </div>
</body>
</html>
