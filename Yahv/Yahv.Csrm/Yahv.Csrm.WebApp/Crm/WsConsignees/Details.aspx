<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WsConsignees.Details" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">

    <%
        YaHv.Csrm.Services.Models.Origins.Consignee consignee = this.Model.Entity as YaHv.Csrm.Services.Models.Origins.Consignee;
    %>
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="width: 600px">
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">
                    <tr>
                        <td style="width: 100px">收件单位</td>
                        <td><%=consignee.Title %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">地区</td>
                        <td><%=consignee.District.GetDescription() %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">地址</td>
                        <td><%=consignee.Address %></td>
                    </tr>

                    <tr id="trcode">
                        <td style="width: 100px">邮编</td>
                        <td><%=consignee.Postzip %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">大赢家编码</td>
                        <td></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">联系人姓名</td>
                        <td><%=consignee.Name %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">
                            <label for="male">电话</label>
                        </td>
                        <td><%=consignee.Tel %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">手机号</td>
                        <td><%=consignee.Mobile %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">邮箱</td>
                        <td><%=consignee.Email %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px"></td>
                        <td>
                            <input class="easyui-checkbox" id="IsDefault" name="IsDefault" <%=consignee.IsDefault?"checked":"" %> /><label for="IsDefault" style="margin-right: 30px">设为默认</label>
                        </td>
                    </tr>
                </table>

            </div>
        </div>
    </div>
</asp:Content>
