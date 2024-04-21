<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.BusinessRelations.List" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
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
            $("#btnCreator").click(function () {
                $.myDialog({
                    title: '新增',
                    url: 'Add.aspx?id=' + model.EnterpriseID,
                    width: '480px',
                    height: '360px',
                    onClose: function () {
                        window.grid.myDatagrid('flush');
                    }
                });
            });
        })
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)AuditStatus.Normal%>') {
                arry.push('<a id="btnUnable" href="#" particle="Name:\'停用\',jField:\'btnUnable\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-disabled\'" onclick="Closed(\'' + rowData.ID + '\')">停用</a> ');
            }
            else if (rowData.Status == '<%=(int)AuditStatus.Closed%>') {
                arry.push('<a id="btnEnable" href="#"  particle="Name:\'启用\',jField:\'btnEnable\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-enabled\'" onclick="Enable(\'' + rowData.ID + '\')">启用</a> ');
            }
           
            arry.push('</span>');
            return arry.join('');
        }
        function Enable(id) {
            $.messager.confirm('确认', '确定启用吗？', function (r) {
                if (r) {
                    $.post('?action=Enable', { ID: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "启用成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });

                }
            });
        }
        function Closed(id) {
            $.messager.confirm('确认', '确定停用吗？', function (r) {
                if (r) {
                    $.post('?action=Closed', { ID: id }, function () {

                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "已停用!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
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
                    <td colspan="8"><a id="btnCreator" particle="Name:\'新增\',jField:\'btnCreator\'"  class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增</a></td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg">
        <thead>
            <tr>
                <th data-options="field:'MainName',width:200">主体公司</th>
                <th data-options="field:'SubName',width:200">关联公司</th>
                <th data-options="field:'Relation',width:200">关联类型</th>
                <th data-options="field:'StatusDes',width:80">状态</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:180">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>