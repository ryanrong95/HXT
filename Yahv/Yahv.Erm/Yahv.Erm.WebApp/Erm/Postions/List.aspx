<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Postions.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                s_name: $.trim($('#s_name').textbox("getText"))
            };
            return params;
        };

        function edit(id) {
            $.myDialog({
                title: "基本信息",
                url: 'Edit.aspx?id=' + id,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });

            return false;
        }

        //分配工资项
        function allot(id) {
            $.myDialog({
                title: "分配工资项",
                url: 'WageItemEdit.aspx?id=' + id,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
        }

        function btn_formatter(value, rec) {
            return ['<span class="easyui-formatted">',
          , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-assign\'" onclick="allot(\'' + rec.ID + '\');return false;">分配工资项</a>'
          , '</span>'].join('');
            return arry.join('');
        }

        $(function () {

            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: false,
                fitColumns: true,
                singleSelect: false,
            });

            //添加
            $('#btnCreator').click(function () {
                edit();
                return false;
            });

            // 搜索按钮
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
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">名称</td>
                <td colspan="3">
                    <input id="s_name" data-options="prompt:'岗位名称',validType:'length[1,50]',isKeydown:true" class="easyui-textbox"/>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnCreator" class="easyui-linkbutton" iconcls="icon-yg-add">添加</a>
                    <a id="btnDelete" class="easyui-linkbutton" iconcls="icon-yg-delete">删除</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="岗位列表">
        <thead>
            <tr>
                <th data-options="field:'ck',checkbox:true"></th>
                <%--<th data-options="field:'ID',width:50">ID</th>--%>
                <th data-options="field:'Name',width:50">名称</th>
                <th data-options="field:'AdminName',width:50">创建人</th>
                <th data-options="field:'CreateDate',width:50">创建日期</th>
                <th data-options="field:'Status',width:50">状态</th>
                <th data-options="field:'btn',formatter:btn_formatter,width:50">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
