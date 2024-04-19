<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Orders.Product.Edit" %>

<%@ Import Namespace="NtErp.Wss.Sales.Services.Utils.Structures" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>产品修改</title>
    <uc:EasyUI runat="server" />

    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.menu = '订单列表';
    </script>
    <script>
        var orderid = '<%=Order.ID%>';
        // 强制保留位小数，如：2，会在2后面补上00.即2.00 

        function toDecimal(x, digit) {
            var f = parseFloat(x);
            if (isNaN(f)) {
                return;
            }
            var l = Math.pow(10, digit);
            f = Math.round(x * l) / l;
            return f;
        }
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
                        if (data.success) {
                            $.messager.alert('提示', '修改成功', function () {
                                $.myWindow.close();
                            });
                        }
                        else {
                            if (data.code == -1) {
                                $.messager.alert('提示', '数量必须大于0');
                            }
                            else if (data.code == -2) {
                                $.messager.alert('提示', '单价必须大于0');
                            }
                            else if (data.code == -3) {
                                $.messager.alert('提示', '订单不存在');
                            }
                            else if (data.code == -5) {
                                $.messager.alert('提示', '未检测到修改，已取消提交');
                            }
                            else {
                                $.messager.alert('提示', '修改失败');
                            }
                        }
                    }
                });
                $('#form1').submit();
            });

        });

    </script>

</head>
<body>
    <div class="easyui-panel" title="修改产品" style="width: 100%; padding: 10px;">
        <% if (this.ServiceDetail == null)
            {
                Response.Write("产品项不存在");
                Response.End();
            }
        %>
        <div>
            <form id="form1">
                <input type="hidden" name="orderid" value="<%=Order.ID %>" />
                <input type="hidden" name="id" value="<%=ServiceDetail.ServiceOutputID %>" />
                <table class="liebiao">
                    <tr>
                        <th style="width: 100px;">服务号：</th>
                        <td><%=ServiceDetail.ServiceOutputID %></td>
                    </tr>
                    <tr>
                        <th>客户编号：</th>
                        <td><%=ServiceDetail.Product.CustomerCode %></td>
                    </tr>
                    <tr>
                        <th>型号：</th>
                        <td><%=ServiceDetail.Name %></td>
                    </tr>
                    <tr>
                        <th>品牌：</th>
                        <td><%=ServiceDetail.Product.Manufacturer %></td>
                    </tr>
                    <tr>
                        <th>供应商：</th>
                        <td><%=ServiceDetail.Product.Supplier %></td>
                    </tr>
                    <tr>
                        <th>单价：</th>
                        <td>
                            <input type="text" class="easyui-numberbox" data-options="precision:4,required:true, onChange:function(newValue,oldValue){
                                    var count = $('#_count').numberbox('getValue');
                                    var sum = toDecimal(newValue*count,4);
                                    $('#_sum').html(sum);
                                }"
                                id="_price" name="_price" value="<%=ServiceDetail.Price %>" />
                            <%=ServiceDetail.Currency.GetTitle()%>
                        </td>
                    </tr>
                    <tr>
                        <th>数量：</th>
                        <td>
                            <input type="text" class="easyui-numberbox" data-options="precision:0,required:true,min:1,max:10000000,
                                onChange:function(newValue,oldValue){
                                    var price = $('#_price').numberbox('getValue');
                                    var sum = toDecimal(newValue*price,4);
                                    $('#_sum').html(sum);
                                }"
                                id="_count" name="_count" value="<%=ServiceDetail.Quantity %>" />
                        </td>
                    </tr>
                    <tr>
                        <th>小计：</th>
                        <td><%=ServiceDetail.Currency.GetTitle()%><span id="_sum"><%=ServiceDetail.SubTotal %></span> </td>
                    </tr>
                    <tr>
                        <th>修改备注：</th>
                        <td>
                            <input class="easyui-textbox" id="_summary" name="_summary" data-options="multiline:true" style="width: 300px; height: 100px;" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2"  style="text-align: center;">
                            <a href="javascript:void(0)" id="btn_save" class="easyui-linkbutton" >提交</a>
                            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="$.myWindow.close();">关闭</a>
                        </td>
                    </tr>

                </table>

            </form>
        </div>
    </div>
</body>
</html>
