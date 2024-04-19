<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaidInfo.aspx.cs" Inherits="WebApp.Oss.Orders.Patches.PaidInfo" %>
<%@ Import Namespace="Needs.Utils.Descriptions" %>
<%
    var entity = this.Model as NtErp.Wss.Oss.Services.Models.Order;
%>
<table class="liebiao" id="payinfo">
    <tbody>
        <tr>
            <td>类型</td>
            <td>支付方式</td>
            <td>支付金额</td>
            <td>时间</td>
        </tr>
        <%
            foreach (var item in entity.Paids.OrderByDescending(t => t.CreateDate))
            {
        %>
        <tr>
            <td><%=item.From.GetDescription() %></td>
            <td><%=item.Type.ToString() %></td>
            <td><%=item.Amount %></td>
            <td><%=item.CreateDate %></td>
        </tr>
        <%
            }
        %>
        <tr>
            <td colspan="4" style="text-align: right">总计：<%=entity.Symbol %>&nbsp;&nbsp;
                    <%=entity.Paids.Sum(t=>t.Amount) %>
            </td>
        </tr>
    </tbody>
</table>
