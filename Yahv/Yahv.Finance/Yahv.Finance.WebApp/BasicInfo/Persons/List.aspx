<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.Persons.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#tab1").myDatagrid({
                nowrap: false,
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: false,
                rownumbers: true,
                queryParams: getQuery()
            });

            // 搜索按钮
            $('#btnSearch').click(function () {
                $("#tab1").myDatagrid('search', getQuery());
                return false;
            });

            $("#btnClear").click(function () {
                location.reload();
                return false;
            });

            $('#s_district').combobox({
                data: model.Districts,
                valueField: "value",
                textField: "text",
                multiple: false,
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                },
            });

            $('#s_status').combobox({
                data: model.Statuses,
                valueField: "value",
                textField: "text"
            });

            //添加
            $("#btnAdd").click(function () {
                $.myDialog({
                    title: '添加',
                    url: '/Finance/BasicInfo/Persons/Edit.aspx',
                    width: "576",
                    height: "300",
                    onClose: function () {
                        $("#tab1").myDatagrid('search', getQuery());
                    }
                });
            });

            //启用
            $("#btnEnable").click(function () {
                var rows = $('#tab1').datagrid('getChecked');

                var arry = $.map(rows, function (item, index) {
                    return item.ID;
                });

                if (arry.length == 0) {
                    top.$.messager.alert('提示', '至少选择一项');
                    return false;
                }

                $.messager.confirm('操作说明', '确定要启用吗？', function (r) {
                    if (r) {
                        $.post('?action=enable', { items: arry.toString() }, function () {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "启用成功!",
                                type: "success"
                            });
                            $('#tab1').myDatagrid('search', getQuery());
                        });
                    }
                });
            });

            //停用
            $("#btnUnable").click(function () {
                var rows = $('#tab1').datagrid('getChecked');

                var arry = $.map(rows, function (item, index) {
                    return item.ID;
                });

                if (arry.length == 0) {
                    top.$.messager.alert('提示', '至少选择一项');
                    return false;
                }

                $.messager.confirm('操作说明', '确定要停用吗？', function (r) {
                    if (r) {
                        $.post('?action=disable', { items: arry.toString() }, function () {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "停用成功!",
                                type: "success"
                            });
                            $('#tab1').myDatagrid('search', getQuery());
                        });
                    }
                });
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                s_name: $.trim($('#s_name').textbox("getText")),
                s_status: $.trim($('#s_status').combobox('getValue')),
            };
            return params;
        };

        function btnFormatter(value, row) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + row.ID + '\');return false;">编辑</a> '
                , '</span>'].join('');
        }

        function edit(ID) {
            $.myDialog({
                title: '详情',
                url: '/Finance/BasicInfo/Persons/Edit.aspx?id=' + ID,
                width: "576",
                height: "300",
                onClose: function () {
                    $("#tab1").myDatagrid('search', getQuery());
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td>姓名</td>
                <td>
                    <input id="s_name" data-options="prompt:'请输入姓名或手机号'" style="width: 200px;" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">状态</td>
                <td>
                    <select id="s_status" data-options="editable: false," class="easyui-combobox" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">刷新</a>
                    <em class="toolLine"></em>
                    <a id="btnAdd" class="easyui-linkbutton" iconcls="icon-yg-add">添加</a>
                    <a id="btnEnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-enabled'">启用</a>
                    <a id="btnUnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-disabled'">停用</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="人员管理">
        <thead>
            <tr>
                <th data-options="field:'ID',align:'center',width:fixWidth(12)">编码</th>
                <th data-options="field:'RealName',align:'left',width:fixWidth(12)">名称</th>
                <th data-options="field:'Gender',align:'left',width:fixWidth(8)">性别</th>
                <th data-options="field:'IDCard',align:'left',width:fixWidth(12)">身份证</th>
                <th data-options="field:'Mobile',align:'left',width:fixWidth(8)">手机号</th>
                <th data-options="field:'CreatorName',align:'left',width:fixWidth(10)">创建人</th>
                <th data-options="field:'CreateDate',align:'left',width:fixWidth(12)">创建时间</th>
                <th data-options="field:'StatusName',align:'left',width:fixWidth(8)">状态</th>
                <th data-options="field:'btn',align:'center',formatter:btnFormatter,width:fixWidth(10)">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
