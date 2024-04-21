<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.Brands.Edit" %>

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
            $('#cbo_Admins').combobox({
                data: model.Admin,
                valueField: 'ID',
                textField: 'RealName',
                panelHeight: 'auto', //自适应
                multiple: false,
            });
            $("#btnCreator").click(function () {
                var id = $('#cbo_Admins').combobox('getValue');
                
                if (id) {
                    $.post('?action=Add', { BrandID: model.BrandID, AdminID: id }, function (result) {
                        if (result.success) {
                            top.$.timeouts.alert({
                                position: "CC",
                                msg: result.message,
                                type: "success"
                            });
                            grid.myDatagrid('flush');
                        }
                        else {
                            top.$.timeouts.alert({
                                position: "CC",
                                msg: result.message,
                                type: "error"
                            });
                        }
                    });
                }

            })
        })
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnUpd" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="del(\'' + rowData.ID + '\')">删除</a> ');

            arry.push('</span>');
            return arry.join('');
        }
        function del(id) {
            $.messager.confirm('确认', '确认删除吗？', function (r) {
                if (r) {
                    $.post('?action=Del', { ID: id }, function (success) {
                        if (success) {
                            top.$.timeouts.alert({
                                position: "CC",
                                msg: "删除成功!",
                                type: "success"
                            });
                            grid.myDatagrid('flush');
                        }
                        else {
                            top.$.timeouts.alert({
                                position: "CC",
                                msg: "删除失败!",
                                type: "error"
                            });
                        }
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
                    <td>
                        <input id="cbo_Admins" name="Admins" class="easyui-combobox" style="width: 350px;" data-options="width:350">
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">添加</a>


                    </td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <!-- 表格 -->
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <th data-options="field:'RealName',width:200">真实姓名</th>
                <th data-options="field:'RoleName',width:150">角色</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:180">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
