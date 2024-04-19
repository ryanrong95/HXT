<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Oss.Waybills.Edit" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>发货</title>
    <uc:EasyUI runat="server" />

</head>
<body>
    <form id="form1" runat="server">

        <input runat="server" type="hidden" id="hSuccess" value="保存成功" />

        <input type="hidden" name="orderid" value="<%=Order.ID %>" />
        <input type="hidden" name="itemid" value="<%=OrderItem.ID %>" />
        <input type="hidden" name="wid" value="<%=WayItem?.ID %>" />

        <table class="liebiao">
            <tr>
                <th style="width: 100px;">服务号：</th>
                <td><%=OrderItem.Product.ID %></td>
            </tr>
            <tr>
                <th>型号：</th>
                <td><%=OrderItem.Product.Name %></td>
            </tr>
            <tr>
                <th>应发数量：</th>
                <td><%=OrderItem.Quantity %></td>
            </tr>
            <tr>
                <th>已发数量：</th>
                <td><%=Sent %></td>
            </tr>
            <tr>
                <th>未发数量：</th>
                <td><%=(OrderItem.Quantity-Sent) %></td>
            </tr>
            <tr>
                <th>本次发送数量：</th>
                <td>
                    <input class="easyui-numberbox" type="text" name="_count" data-options="precision:0,required:true,max:<%=(OrderItem.Quantity-Sent) %>,min:0" value="<%=(OrderItem.Quantity-Sent) %>" />
                </td>
            </tr>
            <tr>
                <th>运单号：</th>
                <td>
                    <input class="easyui-textbox" type="text" name="_waybillNumber" data-options="required:true" value="<%=WayItem?.WaybillID %>" />
                </td>
            </tr>
            <%--<tr>
                <th>承运商：</th>
                <td>
                    <input class="easyui-textbox" type="text" name="_carrier" data-options="required:true" value="<%=WayItem?.Bill?.Carrier %>" />
                </td>
            </tr>--%>
            <%--<tr>
                <th>运费承担方：</th>
                <td>
                    <%=FreightMode %>
                    <!--<select class="easyui-combobox" name="_payer" id="_payer" style="width: 146px;">
                        <option value="0">收货方</option>
                        <option value="1">发货方</option>
                    </select>-->
                </td>
            </tr>
            <tr>
                <th>运费金额：</th>
                <td>
                    <input class="easyui-numberbox" type="text" name="_freight" data-options="precision:2,max:100000,required:true" />
                    <%=Order.Symbol %> 
                </td>
            </tr>--%>
            <tr>
                <th>重量：</th>
                <td>
                    <input class="easyui-textbox" type="text" name="_weight" value="<%=WayItem?.Weight %>" />
                </td>
            </tr>
            <tr>
                <td style="text-align: center;" colspan="2">
                    <asp:Button ID="btnSubmit" runat="server" class="easyui-linkbutton" Text="保存" OnClick="btnSubmit_Click" />
                    <a href="javascript:void(0)" class="easyui-linkbutton" onclick="$.myWindow.close();">关闭</a>
                </td>
            </tr>
        </table>

    </form>
</body>
</html>
