<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.nSuppliers.nContacts.Details" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        YaHv.Csrm.Services.Models.Origins.nContact nContact = this.Model.Entity as YaHv.Csrm.Services.Models.Origins.nContact;
    %>
    <div style="width: 600px">
        <div style="width: 600px">
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">
                    <tr>
                        <td>联系人姓名</td>
                        <td>
                            <%=nContact.Name %>
                        </td>
                    </tr>
                    <tr>
                        <td>电话</td>
                        <td>
                            <%=nContact.Tel %>
                        </td>
                    </tr>
                    <tr>
                        <td>手机号</td>
                        <td>
                            <%=nContact.Mobile %>
                        </td>
                    </tr>
                    <tr>
                        <td>邮箱</td>
                        <td>
                            <%=nContact.Email %>
                        </td>
                    </tr>
                    <tr>
                        <td>传真</td>
                        <td>
                            <%=nContact.Fax %>
                        </td>
                    </tr>
                    <tr>
                        <td>QQ</td>
                        <td>
                            <%=nContact.QQ %>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>

