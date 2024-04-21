<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="DetailList.aspx.cs" Inherits="Yahv.Finance.WebApp.Payee.PayeeApply.DetailList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#tab1").myDatagrid({
                nowrap: false,
                toolbar: '#topper',
                pagination: false,
                singleSelect: true,
                fitColumns: false,
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table id="tab1" title="">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'left',width:fixWidth(20)">日期</th>
                <th data-options="field:'SenderName',align:'left',width:fixWidth(15)">来源系统</th>
                <th data-options="field:'AccountCatalogName',align:'left',width:fixWidth(15)">类型</th>
                <th data-options="field:'Currency',align:'left',width:fixWidth(15)">币种</th>
                <th data-options="field:'RightPrice',align:'left',width:fixWidth(15)">金额</th>
                <th data-options="field:'CreatorName',align:'left',width:fixWidth(15)">操作人</th>
            </tr>
        </thead>
    </table>
</asp:Content>
