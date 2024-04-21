<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.nSuppliers.Details" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">

    <%
        YaHv.Csrm.Services.Models.Origins.nSupplier supplier = this.Model.Entity as YaHv.Csrm.Services.Models.Origins.nSupplier;
    %>
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="width: 600px">
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">
                    <tr>
                        <td style="width: 100px">中文名称</td>
                        <td colspan="3"><%=supplier.ChineseName %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">英文名称</td>
                        <td colspan="3"><%=supplier.EnglishName %></td>
                    </tr>

                    <tr class="tr_grade">
                        <td style="width: 100px">级别</td>
                        <td colspan="3"><span class="level<%=(int)supplier.Grade %>"></span></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">国家/地区</td>

                        <td colspan="3"><%= Enum.GetValues(typeof(Origin)).Cast<Origin>().Any(i => i.GetOrigin().Code == supplier.RealEnterprise.Place) ? Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == supplier.RealEnterprise.Place).GetOrigin()?.ChineseName : null   %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">法人</td>
                        <td colspan="3"><%=supplier.RealEnterprise.Corporation %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">注册地址</td>
                        <td colspan="3"><%=supplier.RealEnterprise.RegAddress %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">统一社会信用代码</td>
                        <td colspan="3"><%=supplier.RealEnterprise.Uscc %></td>
                    </tr>


                    <tr>
                        <td style="width: 100px">备注</td>
                        <td colspan="3"><%=supplier.Summary %></td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
