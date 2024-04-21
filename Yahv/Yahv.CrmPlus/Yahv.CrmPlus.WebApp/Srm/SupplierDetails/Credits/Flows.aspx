<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Flows.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Credits.Flows" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            window.grid = $("#dg").myDatagrid({
                actionName: 'data',
                //toolbar: '#tb',
                pagination: false,
                fit: true,
                nowrap: false,
                singleSelect: false
            });
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field:'Catalog',width:'20%'">分类</th>
                <th data-options="field:'Subject',width:'20%'">科目</th>
                <th data-options="field:'Currency',width:'10%'">币种</th>
                <th data-options="field:'Price',width:'20%'">金额</th>
                <th data-options="field:'CreateDate',width:'20%'">创建时间</th>
                <th data-options="field:'RealName',width:'10%'">创建人</th>
            </tr>
        </thead>
    </table>
</asp:Content>
