<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" Title="保护申请"  CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Approvals.ProtectApply.List" %>
<%@ Import Namespace="Yahv.Underly" %>
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
        })
        function nameformatter(value, rowData) {
            var result = "";
            result += '<a href="#" onclick="detail(\'' + rowData.MainID + '\')">' + value + '</a>';
            return result;
        }
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == <%=Yahv.Underly.ApplyStatus.Waiting.GetHashCode()%>) {
            arry.push('<a id="btnEdit" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approvalPass\'" onclick="approval(true,\'' + rowData.ID + '\')">审批通过</a> ');
            arry.push('<a id="btnEdit" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approvalNopass\'" onclick="approval(false,\'' + rowData.ID + '\')">审批不通过</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }
        function approval(result, id) {
            $.post('?action=Approve', { id: id, result: result }, function (data) {
                var result = JSON.parse(data)
                if (result.success) {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: "操作成功",
                        type: "success"
                    });
                    grid.myDatagrid('flush');
                }
                else {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: "操作失败",
                        type: "error"
                    });
                }
            });
        }
        function detail(id)
        {
            $.myWindow({
                title: '客户详情',
                url: '../Detail.aspx?id=' + id ,
                width: '1100px',
                height: '600px',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table id="dg">
        <thead>
            <tr>
                <th data-options="field:'Name',formatter:nameformatter,width:200,sortable:true">客户名称</th>
                <th data-options="field:'ApplyerName',width:200,sortable:true">申请人</th>
                <th data-options="field:'CreateDate',width:200,sortable:true">申请时间</th>
                  <th data-options="field:'StatusDes',width:200,sortable:true">申请保护状态</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:300">操作</th>

            </tr>
        </thead>
    </table>
</asp:Content>
