<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.SupplierDetails.Edit" %>

<%@ Import Namespace="YaHv.Csrm.Services" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        YaHv.Csrm.Services.Models.Origins.TradingSupplier supplier = this.Model as YaHv.Csrm.Services.Models.Origins.TradingSupplier;
    %>
    <div style="padding: 10px 60px 20px 60px;">
        <table class="liebiao">
            <tr>
                <td style="width: 100px">供应商名称</td>
                <td colspan="3"><%=supplier.Enterprise.Name %></td>
            </tr>
            <%--<tr>
                <td style="width: 100px">采购公司</td>
                <td colspan="3"><%=supplier.Purchasers.FirstOrDefault()?.Company?.Name %></td>
            </tr>--%>
            <tr>
                <td style="width: 100px">性质</td>
                <td colspan="3"><%=supplier.Nature.GetDescription() %></td>
            </tr>
            <tr>
                <td>类型</td>
                <td colspan="3"><%=supplier.Type.GetDescription() %></td>
            </tr>
            <tr id="trcode">
                <td style="width: 100px">级别</td>
                <td colspan="3"><span class="level<%=(int)supplier.Grade %>"></span></td>
            </tr>

            <tr>
                <td style="width: 100px">大赢家编码</td>
                <td colspan="3"><%=supplier.DyjCode %> </td>
            </tr>
            <tr>
                <td style="width: 100px">管理员编码</td>
                <td colspan="3"><%=supplier.Enterprise.AdminCode %> </td>
            </tr>
            <tr>
                <td style="width: 100px">纳税人识别号</td>
                <td colspan="3"><%=supplier.TaxperNumber %> </td>
            </tr>
            <tr>
                <td style="width: 100px">发票</td>
                <td colspan="3"><%=supplier.InvoiceType.GetDescription() %> </td>
            </tr>
            <tr>
                <td style="width: 100px">原厂</td>
                <td colspan="3"><%=supplier.IsFactory?"是":"否" %></td>
            </tr>
            <%if (supplier.IsFactory)
                {
            %>
            <tr>
                <td style="width: 100px">代理公司</td>
                <td colspan="3"><%=supplier.AgentCompany %> </td>
            </tr>
            <%
                }
            %>
            <tr>
                <td style="width: 100px">货代</td>
                <td colspan="3"><%=supplier.IsForwarder?"是":"否" %></td>
            </tr>
            <tr id="iffactory" hidden="hidden">
                <td style="width: 100px">
                    <label for="male">代理公司</label>
                <td colspan="3"><%=supplier.AgentCompany %> </td>
            </tr>
            <tr>
                <td style="width: 100px">账期/天</td>
                <td colspan="3">
                    <%=supplier.RepayCycle%> </td>
            </tr>
            <tr>
                <td style="width: 100px">额度</td>
                <td colspan="3">
                    <%=supplier.Currency.GetCurrency()+" "+supplier.Price %> </td>
            </tr>
        </table>
    </div>
</asp:Content>
