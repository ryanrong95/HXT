<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Prm.CompanyDetails.Edit" %>

<%@ Import Namespace="YaHv.Csrm.Services" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        YaHv.Csrm.Services.Models.Origins.Company company = this.Model as YaHv.Csrm.Services.Models.Origins.Company;
    %>
    <div style="padding: 10px 60px 20px 60px;">
        <table class="liebiao">
            <tr>
                <td style="width: 100px">公司名称</td>
                <td colspan="3"><%=company.Enterprise.Name %></td>
            </tr>
            <tr>
                <td style="width: 100px">公司类型</td>
                <td colspan="3"><%=company.Type.GetDescription() %></td>
            </tr>
            <tr id="trcode">
                <td style="width: 100px">所在地</td>
                <td colspan="3"><%=company.Range.GetDescription() %></td>
            </tr>

            <tr>
                <td style="width: 100px">管理员编码</td>
                <td colspan="3"><%=company.Enterprise.AdminCode %></td>
            </tr>
            <tr>
                <td style="width: 100px">
                    <label for="male">所属国家或地区</label>
                </td>
                <td colspan="3"><%=company.Enterprise.District %></td>
            </tr>
            <tr>
                <td style="width: 100px">法人</td>
                <td colspan="3"><%=company.Enterprise.Corporation %> </td>
            </tr>
            <tr>
                <td style="width: 100px">注册地址</td>
                <td colspan="3"><%=company.Enterprise.RegAddress %> </td>
            </tr>
            <tr>
                <td style="width: 100px">纳税人识别号</td>
                <td colspan="3"><%=company.Enterprise.Uscc %> </td>
            </tr>
        </table>
    </div>
</asp:Content>
