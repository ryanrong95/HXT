<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Carriers.Contacts.Detail" %>

<%@ Import Namespace="YaHv.Csrm.Services" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        YaHv.Csrm.Services.Models.Origins.Contact contact = this.Model.Contact as YaHv.Csrm.Services.Models.Origins.Contact;
    %>
    <div class="easyui-panel" id="tt" data-options="fit:true">
       
            <table class="liebiao">
                <tr>
                    <td style="width: 100px">名称</td>
                    <td colspan="3"><%=contact.Name %></td>
                </tr>
                <tr>
                    <td style="width: 100px">类型</td>
                    <td colspan="3"><%=contact.Type.GetDescription() %></td>
                </tr>
                <tr>
                    <td style="width: 100px">手机号</td>
                    <td colspan="3"><%=contact.Mobile %></td>
                </tr>
                <tr>
                    <td style="width: 100px">电话</td>
                    <td colspan="3"><%=contact.Tel %> </td>
                </tr>

                <tr>
                    <td style="width: 100px">邮箱</td>
                    <td colspan="3"><%=contact.Email %></td>
                </tr>
            </table>
        </div>
  
</asp:Content>

