<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WsBeneficiaries.Details" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        YaHv.Csrm.Services.Models.Origins.WsBeneficiary beneficiary = this.Model.Entity as YaHv.Csrm.Services.Models.Origins.WsBeneficiary;
    %>
    <div style="width: 600px">
        <div style="width: 600px">
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">
                    <tr>
                        <td style="width: 110px">实际的企业名称</td>
                        <td>
                            <%=beneficiary.RealName %>
                        </td>
                    </tr>
                    <tr>
                        <td>开户行</td>
                        <td>
                            <%=beneficiary.Bank %>
                        </td>
                    </tr>
                    <tr>
                        <td>账号</td>
                        <td>
                            <%=beneficiary.Account %>
                        </td>
                    </tr>
                    <tr>
                        <td>开户行地址</td>
                        <td>
                            <%=beneficiary.BankAddress %>
                        </td>
                    </tr>

                    <tr>
                        <td>银行编码</td>
                        <td>
                            <%=beneficiary.SwiftCode %>
                        </td>
                    </tr>
                    <tr>
                        <td>支付方式</td>
                        <td>
                            <%=beneficiary.Methord.GetDescription() %>
                        </td>
                    </tr>
                    <tr id="trcode">
                        <td>支付币种</td>
                        <td>
                            <%=beneficiary.Currency.GetDescription() %>
                        </td>
                    </tr>
                    <tr>
                        <td>地区</td>
                        <td>
                            <%=beneficiary.District.GetDescription() %>
                        </td>
                    </tr>
                    <tr>
                        <td>发票</td>
                        <td>
                            <%=beneficiary.InvoiceType.GetDescription() %>
                        </td>
                    </tr>
                    <tr>
                        <td>联系人姓名</td>
                        <td>
                            <%=beneficiary.Name %>
                        </td>
                    </tr>
                    <tr>
                        <td>电话</td>
                        <td>
                            <%=beneficiary.Tel %>
                        </td>
                    </tr>
                    <tr>
                        <td>手机号</td>
                        <td>
                            <%=beneficiary.Mobile %>
                        </td>
                    </tr>
                    <tr>
                        <td>邮箱</td>
                        <td>
                            <%=beneficiary.Email %>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px"></td>
                        <td>
                            <input id="IsDefault" class="easyui-checkbox" name="IsDefault" <%=beneficiary.IsDefault?"checked":"" %> /><label for="IsDefault" style="margin-right: 30px">设为默认</label>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
