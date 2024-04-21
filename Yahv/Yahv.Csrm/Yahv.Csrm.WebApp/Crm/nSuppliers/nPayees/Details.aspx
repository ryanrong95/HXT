<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.nSuppliers.nPayees.Details" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        YaHv.Csrm.Services.Models.Origins.nPayee nPayee = this.Model.Entity as YaHv.Csrm.Services.Models.Origins.nPayee;
    %>
    <div style="width: 600px">
        <div style="width: 600px">
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">
                    <tr>
                        <td style="width: 110px">代收企业</td>
                        <td>
                            <%=nPayee.RealEnterprise?.Name %>
                        </td>
                    </tr>
                    <tr>
                        <td>支付方式</td>
                        <td>
                            <%=nPayee.Methord.GetDescription() %>
                        </td>
                    </tr>
                    
                    <tr>
                        <td>开户行</td>
                        <td>
                            <%=nPayee.Bank %>
                        </td>
                    </tr>
                    <tr>
                        <td>账号</td>
                        <td>
                            <%=nPayee.Account %>
                        </td>
                    </tr>
                    <tr>
                        <td>开户行地址</td>
                        <td>
                            <%=nPayee.BankAddress %>
                        </td>
                    </tr>

                    <tr>
                        <td>银行编码</td>
                        <td>
                            <%=nPayee.SwiftCode %>
                        </td>
                    </tr>

                    <tr>
                        <td>联系人姓名</td>
                        <td>
                            <%=nPayee.Contact %>
                        </td>
                    </tr>
                    <tr>
                        <td>电话</td>
                        <td>
                            <%=nPayee.Tel %>
                        </td>
                    </tr>
                    <tr>
                        <td>手机号</td>
                        <td>
                            <%=nPayee.Mobile %>
                        </td>
                    </tr>
                    <tr>
                        <td>邮箱</td>
                        <td>
                            <%=nPayee.Email %>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px"></td>
                        <td>
                            <input id="IsDefault" class="easyui-checkbox" name="IsDefault" <%=nPayee.IsDefault?"checked":"" %> /><label for="IsDefault" style="margin-right: 30px">设为默认</label>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>

