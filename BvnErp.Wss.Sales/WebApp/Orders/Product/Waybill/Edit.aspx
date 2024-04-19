<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Orders.Product.Waybill.Edit" %>

<%@ Import Namespace="NtErp.Wss.Sales.Services.Utils.Structures" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>添加运单信息</title>
    <uc:EasyUI runat="server" />
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        //gvSettings.menu = '订单列表';
    </script>

    <script>
        $(function () {

            $('#_payer').combobox("select", '0');

            $('#btn_save').click(function () {
                $('#form1').form({
                    queryParams:{ action:'save' },
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

                $('#form1').submit()

            });
        });
    </script>
</head>
<body>
    <div class="easyui-panel" title="添加运单信息" style="width: 100%; padding: 10px;">
        <form id="form1">
            <input type="hidden" name="orderid" value="<%=Order.ID %>" />
            <input type="hidden" name="id" value="<%=ServiceDetail.ServiceOutputID %>" />
            <table class="liebiao">
                <tr>
                    <th style="width: 100px;">服务号：</th>
                    <td><%=ServiceDetail.ServiceOutputID %></td>
                </tr>
                <tr>
                    <th>型号：</th>
                    <td><%=ServiceDetail.Product.Name %></td>
                </tr>
                <tr>
                    <th>应发数量：</th>
                    <td><%=ServiceDetail.Commodity.Receivable %></td>
                </tr>
                <tr>
                    <th>已发数量：</th>
                    <td><%=ServiceDetail.Commodity.Sent %></td>
                </tr>
                <tr>
                    <th>未发数量：</th>
                    <td><%=ServiceDetail.Commodity.Unsent %></td>
                </tr>
                <tr>
                    <th>本次发送数量：</th>
                    <td>
                        <input class="easyui-numberbox" type="text" name="_count" data-options="precision:0,required:true,max:<%=ServiceDetail.Commodity.Unsent %>,min:0" value="<%=ServiceDetail.Commodity.Unsent %>" />
                    </td>
                </tr>
                <tr>
                    <th>运单号：</th>
                    <td>
                        <input class="easyui-textbox" type="text" name="_waybillNumber" data-options="required:true"  />
                    </td>
                </tr>
                <tr>
                    <th>承运商：</th>
                    <td>
                        <input class="easyui-textbox" type="text" name="_carrier" data-options="required:true"  />
                    </td>
                </tr>
                <tr>
                    <th>运费承担方：</th>
                    <td>
                        <select class="easyui-combobox" name="_payer" id="_payer" style="width:146px;">
                            <option value="0">收货方</option>
                            <option value="1">发货方</option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <th>运费金额：</th>
                    <td>
                      <input class="easyui-numberbox" type="text" name="_freight" data-options="precision:2,max:100000,required:true"/>
                     <%--  <%=Currency %> --%>
                    </td>
                </tr>
                <tr>
                    <th>重量：</th>
                    <td>
                         <input class="easyui-textbox" type="text" name="_weight"  />
                    </td>
                </tr>
                <tr>
                    <th>体积：</th>
                    <td>
                        <input class="easyui-textbox" type="text" name="_measurement" />
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center;" colspan="2">
                        <a href="javascript:void(0)" id="btn_save" class="easyui-linkbutton">提交</a>
                        <a href="javascript:void(0)" class="easyui-linkbutton" onclick="$.myWindow.close();">关闭</a>
                    </td>
                </tr>
            </table>
        </form>
    </div>
</body>
</html>
