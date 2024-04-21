<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Company.Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        Yahv.CrmPlus.Service.Models.Origins.Company company = this.Model.Entity as Yahv.CrmPlus.Service.Models.Origins.Company;
    %>
    <table class="liebiao">
        <tr style="height: 30px">
            <td style="width: 120px;">公司名称</td>
            <td>
                <%=company.Name %>
            </td>
        </tr>
        <tr>
            <td style="width: 120px">法人</td>
            <td colspan="3">
                <%=company.EnterpriseRegister.Corperation %>
            </td>
        </tr>
        <tr>
            <td style="width: 120px">注册地址</td>
            <td colspan="3">
                <%=company.EnterpriseRegister.RegAddress %>
            </td>
        </tr>
        <tr>
            <td style="width: 120px">统一社会信用代码</td>
            <td colspan="3">
                <%=company.EnterpriseRegister.Uscc %>
            </td>
        </tr>
    </table>
</asp:Content>
