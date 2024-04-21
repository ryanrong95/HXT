<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.ClientDetails.Edit" %>

<%@ Import Namespace="YaHv.Csrm.Services" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        YaHv.Csrm.Services.Models.Origins.TradingClient client = this.Model.Entity as YaHv.Csrm.Services.Models.Origins.TradingClient;

    %>
    <div style="padding: 10px 60px 20px 60px;">
        <table class="liebiao">
            <tr>
                <td style="width: 100px">客户名称</td>
                <td colspan="3">
                    <%
                        if ((int)client.Vip < 0)
                        {
                    %>
                    <span class='vip'></span>
                    <%
                        }
                    %>
                    <%
                        else if ((int)client.Vip > 0)
                        {
                    %>
                    <span class='vip<%=(int)client.Vip %>'></span>
                    <%
                        }
                    %>

                    <%=client.Enterprise.Name %></td>
                <%-- <td id="Vip" colspan="3"></td>--%>
            </tr>
           <%-- <tr <%= this.Model.IsSale? null:"hidden=\"hidden\"" %>>
                <td style="width: 100px">销售公司</td>
                <td colspan="3"><%= this.Model.IsSale? client.Sales.FirstOrDefault(item=>item.ID== Yahv.Erp.Current.ID)?.Company?.Name:null %></td>
            </tr>--%>
            <tr>
                <td style="width: 100px">性质</td>
                <td colspan="3"><%=client.Nature.GetDescription() %></td>
            </tr>
            <tr>
                <td style="width: 100px">类型</td>
                <td colspan="3"><%=client.AreaType.GetDescription() %></td>
            </tr>
            <tr>
                <td style="width: 100px">客户级别</td>
                <td colspan="3"><span class="level<%=(int)client.Grade%>"></span></td>
            </tr>

            <tr>
                <td style="width: 100px">大赢家编码</td>
                <td colspan="3"><%=client.DyjCode %> </td>
            </tr>
            <tr>
                <td style="width: 100px">管理员编码</td>
                <td colspan="3"><%=client.Enterprise.AdminCode %></td>
            </tr>
            <tr>
                <td style="width: 100px">法人</td>
                <td colspan="3"><%=client.Enterprise.Corporation %> </td>
            </tr>
            <tr>
                <td style="width: 100px">注册地址</td>
                <td colspan="3"><%=client.Enterprise.RegAddress %> </td>
            </tr>
            <tr>
                <td style="width: 100px">纳税人识别号</td>
                <td colspan="3"><%=client.Enterprise.Uscc %> </td>
            </tr>

        </table>
    </div>
</asp:Content>
