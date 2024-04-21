<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.WsSupplierDetails.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
        <%
            YaHv.Csrm.Services.Models.Origins.WsSupplier wssuppliers = this.Model.Entity as YaHv.Csrm.Services.Models.Origins.WsSupplier;
        %>
        <div style="padding: 10px 60px 20px 60px;">
            <table class="liebiao">
                <tr>
                    <td style="width: 100px">企业名称</td>
                    <td colspan="3">
                        <%=wssuppliers.Enterprise.Name %>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">中文名称</td>
                    <td colspan="3">
                        <%=wssuppliers.ChineseName %>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">英文名称</td>
                    <td colspan="3">
                        <%=wssuppliers.EnglishName %>
                    </td>
                </tr>
                <tr class="tr_grade">
                    <td style="width: 100px">级别</td>
                    <td colspan="3">
                        <span class="level<%=(int)wssuppliers.Grade %>"></span>
                    </td>
                </tr>

                <tr>
                    <td style="width: 100px">管理员编码</td>
                    <td colspan="3">
                        <%=wssuppliers.Enterprise.AdminCode %>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">法人</td>
                    <td colspan="3">
                        <%=wssuppliers.Enterprise.Corporation %>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">注册地址</td>
                    <td colspan="3">
                        <%=wssuppliers.Enterprise.RegAddress %>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">统一社会信用代码</td>
                    <td colspan="3">
                        <%=wssuppliers.Enterprise.Uscc %>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">备注</td>
                    <td colspan="3">
                        <%=wssuppliers.Summary %>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Content>
