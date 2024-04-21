<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WsConsignors.Details" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <%
            YaHv.Csrm.Services.Models.Origins.Consignor consignor = this.Model.Entity as YaHv.Csrm.Services.Models.Origins.Consignor;
        %>
        <div style="width: 600px">
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">
                    <tr id="warehouseTilte" hidden="hidden">
                        <td style="width: 100px">门牌</td>
                        <td><%=consignor.Title%></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">地址</td>
                        <td><%=consignor.Address%></td>
                    </tr>
                    <tr id="trcode">
                        <td style="width: 100px">邮编</td>
                        <td><%=consignor.Postzip%></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">大赢家编码</td>
                        <td><%=consignor.DyjCode%></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">联系人姓名</td>
                        <td><%=consignor.Name%></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">手机号</td>
                        <td><%=consignor.Mobile%></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">
                            <label for="male">电话</label>
                        </td>
                        <td><%=consignor.Tel%></td>
                    </tr>

                    <tr>
                        <td style="width: 100px">邮箱</td>
                        <td><%=consignor.Email%></td>
                    </tr>
                    <tr>
                        <td style="width: 100px"></td>
                        <td>
                            <input id="IsDefault" class="easyui-checkbox" name="IsDefault" <%=consignor.IsDefault?"checked":"" %> /><label for="IsDefault" style="margin-right: 30px">设为默认</label>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
