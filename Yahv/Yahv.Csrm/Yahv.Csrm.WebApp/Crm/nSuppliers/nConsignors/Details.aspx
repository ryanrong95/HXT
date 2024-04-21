<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.nSuppliers.nConsignors.Details" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">

    <%
        YaHv.Csrm.Services.Models.Origins.nConsignor nConsignor = this.Model.Entity as YaHv.Csrm.Services.Models.Origins.nConsignor;
    %>
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="width: 600px">
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">

                    <tr>
                        <td style="width: 100px">国家/地区</td>
                        <td><%=string.IsNullOrWhiteSpace(nConsignor.Place)?"":Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == nConsignor.Place).GetOrigin().ChineseName %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">地址</td>
                        <td><%=nConsignor.Address %></td>
                    </tr>

                    <tr id="trcode">
                        <td style="width: 100px">邮编</td>
                        <td><%=nConsignor.Postzip %></td>
                    </tr>

                    <tr>
                        <td style="width: 100px">联系人姓名</td>
                        <td><%=nConsignor.Contact %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">
                            <label for="male">电话</label>
                        </td>
                        <td><%=nConsignor.Tel %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">手机号</td>
                        <td><%=nConsignor.Mobile %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">邮箱</td>
                        <td><%=nConsignor.Email %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px"></td>
                        <td>
                            <input class="easyui-checkbox" id="IsDefault" name="IsDefault" <%=nConsignor.IsDefault?"checked":"" %> /><label for="IsDefault" style="margin-right: 30px">设为默认</label>
                        </td>
                    </tr>
                </table>

            </div>
        </div>
    </div>
</asp:Content>

