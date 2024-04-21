<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Protecteds.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.ApprovalRecords.Protecteds" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
    $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data'
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                nowrap: false,
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table id="dg">
        <thead>
            <tr>
                <th data-options="field:'Name',width:200,sortable:true">客户名称</th>
                <th data-options="field:'ApplyerName',width:200,sortable:true">申请人</th>
                <th data-options="field:'CreateDate',width:200,sortable:true">申请时间</th>
                <th data-options="field:'StatusDes',width:200,sortable:true">申请保护状态</th>

            </tr>
        </thead>
    </table>
    <div>
      <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" />
    </div>
</asp:Content>
