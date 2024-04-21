<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Roles.List" %>

<%@ Import Namespace="Yahv.Underly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                s_name: $('#s_name').textbox("getText"),
                s_type: $('#s_type').combobox("getValue")
            };
            return params;
        };

        function edit(id) {
            $.myDialog({ title: "自定义角色", url: 'Edit.aspx?id=' + id });

            var e = jQuery.event.fix(event || window.event);
            e.preventDefault();
            e.stopPropagation();

            return false;
        }

        function compose(id) {
            $.myDialog({ title: "合并角色", url: '../RoleComposes/Edit.aspx?id=' + id });

            var e = jQuery.event.fix(event || window.event);
            e.preventDefault();
            e.stopPropagation();

            return false;
        }

        function btn_formatter(value, rec) {
            if (rec.IsSuper) {
                return '';
            }

            var result = '<span class="easyui-formatted"><button class="easyui-linkbutton" data-options="iconCls:\'icon-yg-setting\'" onclick="edit(\'' + rec.ID + '\');return false;">配置</button></span>';

            if (rec.Type == parseInt('<%=(int)RoleType.Compose%>')) {
                result = '<span class="easyui-formatted"><button class="easyui-linkbutton" data-options="iconCls:\'icon-yg-setting\'" onclick="compose(\'' + rec.ID + '\');return false;">配置</button></span>';
            }

            return result;
        }

        $(function () {
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                fitColumns: true,
                pagination: true,
                singleSelect: false,

            });

            $("#s_type").combobox({
                url: "?action=GetType",
                textField: "text",
                valueField: "value"
            });

            //添加自定义角色
            $('#btnCreator').click(function () {
                edit();
                return false;
            });

            //添加合成角色
            $("#btnCompose").click(function () {
                compose();
                return false;
            });

            //搜索
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });

            // 清空按钮
            $('#btnClear').click(function () {
                location.reload();
                return false;
            });

            //删除
            $('#btnDelete').click(function () {
                var arry = $('#tab1').datagrid('getChecked');

                if (arry && arry.length > 0) {
                    $.messager.confirm('删除提示', '您确定要删除选中的记录吗?', function (r) {
                        if (r) {
                            var ids = arry.map(function (element, index) {
                                return element.ID;
                            });

                            var flag = false;
                            $.each(arry, function (index, element) {
                                if (element.IsSuper) {
                                    flag = true;
                                    return false;
                                }


                                if (element.Status == parseInt('<%=(int)RoleStatus.Fixed%>')) {
                                    flag = true;
                                    return false;
                                }
                            });

                            if (flag) {
                                $.messager.alert('提示', '该权限不能被删除!');
                                return false;
                            }

                            $.post('?action=delete', { ids: ids.join(',') }, function (response) {
                                //$.messager.alert('提示', '删除成功', 'info');
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "删除成功!",
                                    type: "success"
                                });
                                grid.myDatagrid('search', getQuery());
                            });
                        }
                    });
                }
                else {
                    $.messager.alert('提示', '请选择要删除的记录', 'warning');
                }

                return false;
            });
        });

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact" particle="Name:'搜索栏'">
            <tr>
                <td style="width: 90px;">名称</td>
                <td>
                    <input id="s_name" data-options="prompt:'角色名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">类型</td>
                <td>
                    <input id="s_type" class="easyui-combobox" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <em class="toolLine"></em>
                    <button id="btnCreator" particle="Name:'添加自定义角色',jField:'btnCreator'" class="easyui-linkbutton" iconcls="icon-yg-add">添加自定义角色</button>
                    <button id="btnCompose" particle="Name:'添加合并角色',jField:'btnCompose'" class="easyui-linkbutton" iconcls="icon-yg-add">添加合并角色</button>
                    <button id="btnDelete" class="easyui-linkbutton" iconcls="icon-yg-delete">删除</button>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="角色列表">
        <thead>
            <tr>
                <th data-options="field:'ck',checkbox:true">选择</th>
                <th data-options="field:'ID',width:80">ID</th>
                <th data-options="field:'Name',width:150">名称</th>
                <th data-options="field:'TypeName',width:150">类型名称</th>
                <th data-options="field:'CreateDate',width:120">创建日期</th>
                <th data-options="field:'StatusName',width:80">状态</th>
                <th data-options="field:'btn',formatter:btn_formatter,width:120">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
