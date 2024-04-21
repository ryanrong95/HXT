<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.EnumsDictionaries.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
                    Enum: model.Enum
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

            //新增
            $("#btnCreator").click(function () {
                $.myDialog({
                    title: '新增',
                    url: 'Add.aspx?Enum=' + model.Enum,
                    width: "40%",
                    height: "30%",
                    isHaveOk: true,
                    onClose: function () {
                        $("#dg").myDatagrid('search', getQuery());
                    }
                });
            })
            $("#btnInitial").click(function () {
                $.post('?action=InitialEnum', {}, function () {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: "初始化成功!",
                        type: "success"
                    });
                    grid.myDatagrid('flush');
                });
            })
        })
        function fixedformatter(value, RowData) {
            return value ? "是" : "否";
        }
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnUpd" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="EditPage(\'' + rowData.ID + '\')">编辑</a> ');
            if (!rowData.IsFixed) {
                arry.push('<a id="btnDel" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="del(\'' + rowData.ID + '\')">删除</a> ');
            }
            arry.push('</span>');
            return arry.join('');;
        }
        function EditPage(id) {
            $.myDialog({
                title: "编辑",
                url: 'Edit.aspx?&id=' + id,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                isHaveOk: true,
                width: "40%",
                height: "30%",
            });
        }
        function del(id) {
            $.messager.confirm('确认', '确认删除？', function (r) {
                if (r) {
                    $.post('?action=del', { id: id }, function (success) {
                        if (success) {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "删除成功!",
                                type: "success"
                            });
                            grid.myDatagrid('flush');
                        }
                        else {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "删除失败!",
                                type: "error"
                            });
                        }
                    });
                }
            })
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <!--工具-->
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td colspan="8">
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增</a>
                        <%--<a id="btnInitial" class="easyui-linkbutton" data-options="iconCls:'icon-yg-setting'">初始化固定数据</a>--%>
                    </td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <%-- 表格--%>
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <%--<th data-options="field:'Enum',width:200">类别</th>--%>
                <th data-options="field:'IsFixed',width:80,formatter:fixedformatter">是否固定</th>
                <th data-options="field:'Field',width:200">名称</th>
                <th data-options="field:'Description',width:300">描述</th>
                <th data-options="field:'CreateDate',width:100">创建时间</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
