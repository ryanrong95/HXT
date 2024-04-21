<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Owner.List" %>

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
                pagination: false,
                fit: true,
                nowrap: false,
                queryParams: getQuery(),
                singleSelect: false
            });
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            });
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });

        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table id="dg">
        <thead>
            <tr>
                <th data-options="field:'StaffName',width:150">员工姓名</th>
                <th data-options="field:'Position',width:200">职位</th>
                <th data-options="field:'ClientName',width:200">客户名称</th>
                <th data-options="field:'ConductType',width:80">业务类型</th>
                <th data-options="field:'CorCompany',width:200">合作公司</th>
                <th data-options="field:'CreteDate',width:200">注册时间</th>
                <%--<th data-options="field:'ApproveDate',width:200">审批时间</th>--%>
            </tr>
        </thead>
    </table>
</asp:Content>
