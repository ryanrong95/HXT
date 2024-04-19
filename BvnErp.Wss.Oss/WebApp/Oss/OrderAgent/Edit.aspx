<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Oss.OrderAgent.Edit" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单代支付</title>
    <uc:EasyUI runat="server"></uc:EasyUI>

</head>
<body>
    <%
        var order = this.Order as NtErp.Wss.Oss.Services.Models.Order;
        if (order == null)
        {
            Response.Write("订单不存在！");
            Response.End();
        }
    %>
    <form id="form1" runat="server">

        <input runat="server" type="hidden" id="Hidden1" value="订单已支付，请勿重复支付" />
        <input runat="server" type="hidden" id="hSuccess" value="支付成功" />
        <input runat="server" type="hidden" id="hNotEnough" value="余额不足" />

        <input type="hidden" name="oid" value="<%=Order.ID %>" />
        <table class="liebiao">
            <tr>
                <td colspan="2">
                    <h3>用户信息</h3>
                </td>
            </tr>
            <tr>
                <td style="width: 100px;">用户ID:</td>
                <td><%=Order.Client.ID %></td>
            </tr>
            <tr>
                <td>用户名:</td>
                <td><%=Order.Client.UserName %></td>
            </tr>
            <tr>
                <td colspan="2">
                    <h3>用户余额</h3>
                </td>
            </tr>
            <tr>
                <td>现金余额:</td>
                <td><span><%=Cash %></span><span class="r">(<%=Order.Symbol %>)</span> </td>
            </tr>
            <tr>
                <td>信用余额:</td>
                <td><span><%=Credit %></span><span class="r">(<%=Order.Symbol %>)</span></td>
            </tr>
            <tr>
                <td colspan="2">
                    <h3>订单支付情况</h3>
                </td>
            </tr>
            <tr>
                <td>订单总价:</td>
                <td><span><%=Order.Total %></span><span class="r">(<%=Order.Symbol %>)</span></td>
            </tr>
            <tr>
                <td>已支付:</td>
                <td><span><%=Order.Paid %></span><span class="r">(<%=Order.Symbol %>)</span></td>
            </tr>
            <tr>
                <td>待支付</td>
                <td><span><%=(Order.Total-Order.Paid) %></span><span class="r">(<%=Order.Symbol %>)</span></td>
            </tr>
            <tr>
                <td colspan="2">
                    <h3>支付</h3>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <h3>代支付规则:</h3>
                    支付金额小于现金与信用余额之和才可以进行支付,优先扣除现金.
                </td>
            </tr>
            <%
                if (Order.Total - Order.Paid > 0)
                {
            %>
            <tr>
                <td>金额</td>
                <td>
                    <input name="_amount" id="_amount" class="easyui-numberbox" data-options="precision: 4, min: 0.01, max: <%=Order.Total-Order.Paid %>, prompt: '输入支付金额',value:'<%=Order.Total-Order.Paid %>'" />
                    <%--<button type="button" onclick="payment()">支付</button>--%>
                    <asp:Button ID="btnSubmit" runat="server" class="easyui-linkbutton" Text="支付" OnClick="btnSubmit_Click" />
                </td>
            </tr>
            <%
                }
            %>
        </table>

    </form>
</body>
</html>
