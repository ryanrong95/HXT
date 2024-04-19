<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BasicInfo.aspx.cs" Inherits="WebApp.Oss.Orders.Patches.BasicInfo" %>

<%@ Import Namespace="Needs.Utils.Descriptions" %>
<%
    var entity = this.Model as NtErp.Wss.Oss.Services.Models.Order;
%>

<table class="liebiao" id="basicinfo">
    <tbody>
        <tr>
            <th style="width: 90px;">订单号：</th>
            <td><%=entity.ID %></td>
            <th style="width: 90px;">下单时间：</th>
            <td><%=entity.CreateDate.ToString() %></td>
            <th style="width: 90px;">当前状态：</th>
            <td><%=entity.Status.GetDescription() %></td>
            <th style="width: 90px;">订单类型：</th>
            <td><%=entity.Type.GetDescription() %></td>
        </tr>
        <tr>
            <th>交易货币：</th>
            <td><%=entity.Symbol %></td>
            <!--点击能弹出Label显示购买人信息，比如姓名、等级、注册时间、身份、推广人、订单个数、购买订单价值等-->
            <th>订单总额：</th>
            <td><%=entity.Total %></td>
            <th>已支付金额</th>
            <td>&nbsp;&nbsp;
                <%=entity.Paid %>
            </td>
            <th>未支付金额</th>
            <td>&nbsp;&nbsp;
                <%=entity.Total - entity.Paid%>
            </td>
        </tr>
        <tr>
            <th>交货地：</th>
            <td><%=Needs.Underly.Legally.Current[ entity.Consignee.District].ShortName %></td>
            <th>购买人：</th>
            <td><%=entity.Client.UserName %></td>
            <th>客服：</th>
            <td>&nbsp;&nbsp;</td>
            <td>&nbsp;&nbsp;</td>
            <td>&nbsp;&nbsp;</td>
        </tr>
        <tr>
            <th>订单备注：</th>
            <td colspan="7"><%=entity.Summary %></td>
        </tr>
    </tbody>
</table>
