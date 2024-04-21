<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.FinanceSubjects.List" %>

<%@ Import Namespace="Yahv.Underly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            loadtree();
        });

        //加载tree
        var loadtree = function () {
            $('#tg').treegrid('clearChecked');
            $('#tg').treegrid({
                url: '?action=tree',
                method: 'get',
                idField: 'id',
                treeField: 'name',
                rownumbers: true,
                cascadeCheck: false,
                loadMsg: '正在载入数据',
                checkOnSelect: true,
                selectOnCheck: true,
                singleSelect: true,
                lines: true,
                animate: true,
                collapsible: true,
                fitColumns: true,
                formatter: btn_formatter,
                onLoadSuccess: function (data) {
                    $.parser.parse('.easyui-formatted');
                },
            });
        }

        function btn_formatter(value, rec) {
            var arry = [];

            arry.push('<span class="easyui-formatted">');
            arry.push('　<button class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="upd(\'' + rec.id + '\');return false;">编辑</button> ');
            if (rec.type != 4) {
                arry.push('<button class="easyui-linkbutton view_button" data-options="iconCls:\'icon-yg-addChildNode\'" onclick="add(\'' + rec.id + '\');return false;">添加子节点</button> ');
            }
            if (rec.children == null) {
                arry.push('<button class="easyui-linkbutton view_button" data-options="iconCls:\'icon-yg-delete\'" onclick="del(\'' + rec.id + '\');return false;">删除</button>');
            }
            arry.push('</span>');

            return arry.join('');
        }

        //按钮操作
        function upd(id) {
            var node = $('#tg').treegrid('find', id);

            $.myDialog({
                title: "科目信息",
                width: "500px",
                height: "300px",
                url: 'Edit.aspx?id=' + id + '&&fatherid=' + node._parentId, onClose: function () {
                    loadtree();
                }
            });

            return false;
        };

        function add(id) {
            $.myDialog({
                title: "科目信息",
                width: "500px",
                height: "300px",
                url: 'Edit.aspx?fatherid=' + id, onClose: function () {
                    loadtree();
                }
            });
        };
        function del(id) {
            $.messager.confirm('确认', '您确认想要删除该节点吗？', function (r) {
                if (r) {
                    $.get("?action=remove", { id: id }, function () {
                        $("#tg").treegrid("remove", id);
                        loadtree();
                    });
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table id="tg" class="easyui-treegrid" title="">
        <thead>
            <tr>
                <th data-options="field:'name',width:180">名称</th>
                <th data-options="field:'typeName',width:80">类型</th>
                <th data-options="field:'btn',formatter:btn_formatter,width:120">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
