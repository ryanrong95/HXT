<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Specials.Detail" %>
<%@ Import Namespace="Yahv.Underly" %>
<%@ Register Src="~/Uc/PcFiles.ascx" TagPrefix="uc1" TagName="PcFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        Yahv.CrmPlus.Service.Models.Origins.Special entity = this.Model.Entity as Yahv.CrmPlus.Service.Models.Origins.Special;
       
    %>
    <div>
        <table class="liebiao">
            <tr>
                <td>品牌名称:</td>
                <td><%=entity.Brand %></td>
            </tr>
            <tr>
                <td>型号:</td>
                <td><%=entity.PartNumber %></td>
            </tr>
            <tr>
                <td>特色类型:</td>
                <td><%=entity.Type.GetDescription() %></td>
            </tr>
            <tr>
                <td>备注:</td>
                <td><%=entity.Summary %></td>
            </tr>
        </table>

        <uc1:PcFiles runat="server" id="PcFiles" IsPc="false"/>
    </div>
</asp:Content>
