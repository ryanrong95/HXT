<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Contacts.Detail" %>

<%@ Register Src="~/Uc/PcFiles.ascx" TagPrefix="uc1" TagName="PcFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style>
        .tdname {
            background-color: #F9FAFC;
            width: 150px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        Yahv.CrmPlus.Service.Models.Origins.Contact entity = this.Model.Entity as Yahv.CrmPlus.Service.Models.Origins.Contact;
    %>

    <table class="liebiao">
        <tr>
            <td class="tdname">姓名</td>
            <td><%=entity.Name %> </td>
            <td class="tdname">部门</td>
            <td><%=entity.Department %></td>
        </tr>
        <tr>
            <td class="tdname">职位</td>
            <td><%=entity.Positon %></td>
            <td class="tdname">手机号</td>
            <td><%=entity.Mobile %></td>
        </tr>
        <tr>
            <td class="tdname">联系电话</td>
            <td><%=entity.Tel %></td>
            <td class="tdname">邮箱 </td>
            <td><%=entity.Email %></td>
        </tr>

        <tr>
            <td class="tdname">性别</td>
            <td><%=entity.Gender %></td>
            <td class="tdname">QQ</td>
            <td><%=entity.QQ %></td>
        </tr>
        <tr>
            <td class="tdname">Wx</td>
            <td><%=entity.Wx %></td>
            <td class="tdname">Skype</td>
            <td><%=entity.Skype %></td>
        </tr>
        <tr>
            <td class="tdname">性格</td>
            <td><%=entity.Character %></td>
            <td class="tdname">忌讳</td>
            <td><%=entity.Taboo %></td>
        </tr>
        <%--<tr>
            <td class="tdname">个人名片</td>
            <td colspan="3"><a href="<%=card?.Url %>" target='_blank'><%=card?.CustomName %></a></td>
        </tr>--%>
        <tr>
            <td class="tdname">备注</td>
            <td colspan="3"><%=entity.Summary %></td>
        </tr>
    </table>
    <uc1:PcFiles runat="server" id="PcFiles" IsPc="false" />
</asp:Content>
