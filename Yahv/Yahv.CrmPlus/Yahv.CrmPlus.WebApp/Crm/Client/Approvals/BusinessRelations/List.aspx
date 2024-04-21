<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Approvals.BusinessRelations.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
                    Name: $.trim($('#txtname').textbox("getText")),
                    RelationType: $('#cbbType').combobox("getValue")
                };
                return params;
            };
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                nowrap: false,
            });
            $("#cbbType").combobox({
                data: model.Type,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    $(this).combobox('setValue', data[0].value);
                },
            });

            // 搜索按钮
            $('#btnSearch').click(function () {
                $("#dg").myDatagrid('search', getQuery());
                return false;
            });

            $("#btnClear").click(function () {
                location.reload();
                return false;
            });

        })
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnApproval" href="#" particle="Name:\'审批\',jField:\'Approval\'"  class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approval\'" onclick="approval(\'' + rowData.ID + '\')">审批</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function approval(id) {
            $.myWindow({
                title: '审批',
                url: 'Edit.aspx?id=' + id,
                width: '650px',
                height: '500px',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td>企业</td>
                    <td>
                        <input id="txtname" class="easyui-textbox" style="width: 200px" /></td>
                    <td>关联类型</td>
                    <td>
                        <input id="cbbType" class="easyui-combobox" style="width: 200px"></td>
                    <td>
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em></em>
                        <a id="btnApplyLog" class="easyui-linkbutton" data-options="iconCls:'icon-yg-details'">审批记录</a>
                    </td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg">
        <thead>
            <tr>
                <th data-options="field:'MainName',width:200,sortable:true">主体公司</th>
                <th data-options="field:'SubName',width:200,sortable:true">关联公司</th>
                <th data-options="field:'Relation',width:200,sortable:true">关联类型</th>
                <th data-options="field:'CreateDate',width:180,sortable:true">申请时间</th>
                <th data-options="field:'Creator',width:80,sortable:true">申请人</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>

            </tr>
        </thead>
    </table>
</asp:Content>
