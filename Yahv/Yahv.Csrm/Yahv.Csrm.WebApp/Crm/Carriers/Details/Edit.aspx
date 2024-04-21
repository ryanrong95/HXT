<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Carriers.Details.Edit" %>

<%@ Import Namespace="YaHv.Csrm.Services" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        YaHv.Csrm.Services.Models.Origins.Carrier carrier = this.Model.Entity as YaHv.Csrm.Services.Models.Origins.Carrier;
    %>
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="width: 800px">
            <table class="liebiao">
                <tr>
                    <td style="width: 100px">名称</td>
                    <td colspan="3"><%=carrier.Enterprise.Name %></td>
                </tr>
                <tr>
                    <td style="width: 100px">简称</td>
                    <td colspan="3"><%=carrier.Code %></td>
                </tr>
                <tr>
                    <td style="width: 100px">管理员编码</td>
                    <td colspan="3"><%=carrier.Enterprise.AdminCode %> </td>
                </tr>
                <tr>
                    <td style="width: 100px">法人</td>
                    <td colspan="3"><%=carrier.Enterprise.Corporation %></td>
                </tr>
                <tr>
                    <td style="width: 100px">注册地址</td>
                    <td colspan="3"><%=carrier.Enterprise.RegAddress %></td>
                </tr>
                <tr>
                    <td style="width: 100px">统一社会信用代码</td>
                    <td colspan="3"><%=carrier.Enterprise.Uscc %></td>
                </tr>
                <tr>
                    <td style="width: 100px">Logo</td>
                    <td>
                        <img src="<%=carrier.Icon %>">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">备注</td>
                    <td colspan="3"><%=carrier.Summary %></td>
                </tr>
                <tr>
                    <td style="width: 100px">创建时间</td>
                    <td colspan="3"><%=carrier.CreateDate.ToString("yyyy-MM-dd HH:mm:ss") %></td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
