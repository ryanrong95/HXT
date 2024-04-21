<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.Dishonests.Detail" %>

<%@ Import Namespace="Yahv.CrmPlus.Service" %>
<%@ Import Namespace="Yahv.Underly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        Yahv.CrmPlus.Service.Models.Origins.SupplierDisHonest entity = this.Model.Entity as Yahv.CrmPlus.Service.Models.Origins.SupplierDisHonest;
    %>
    <table class="liebiao">
        <tr>
            <td class="tdtitle">供应商</td>
            <td><%=entity.EnterpriseName %></td>
        </tr>
        <tr>
            <td class="tdtitle">失信原因</td>
            <td><%=entity.Reason %></td>
        </tr>
        <tr>
            <td class="tdtitle">发生时间</td>
            <td><%=entity.OccurTime.ToShortDateString() %></td>

        </tr>
        <tr>
            <td class="tdtitle">相关单据</td>
            <td><%=entity.Code %></td>

        </tr>
        <tr>
            <td class="tdtitle">备注</td>
            <td><%=entity.Summary %></td>
        </tr>
    </table>
</asp:Content>
