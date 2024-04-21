<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.FlowAccounts.List" %>

<%@ Import Namespace="Yahv.Usually" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                onLoadSuccess: function (data) {
                    //if (data.footer) {
                    //    $.each(data.footer, function (index, value) {
                    //        if (data.footer[index].CreateDate) {
                    //            grid.myDatagrid('appendRow', data.footer[index]);
                    //        }
                    //    });
                    //}
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <!-- 表格 -->
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field: 'PayeeName',width:200">客户</th>
                <th data-options="field: 'Business',width:120">业务</th>
                <th data-options="field: 'Project',width:120">分类/科目</th>
                <th data-options="field: 'Price',width:120">金额</th>
                <th data-options="field: 'CurrencyName',width:100">币种</th>
                <th data-options="field: 'AdminName',width:100">操作人</th>
                <th data-options="field: 'CreateDate',width:150">创建日期</th>
            </tr>
        </thead>
    </table>
</asp:Content>

