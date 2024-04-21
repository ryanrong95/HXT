<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Shifts.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            //清空按钮
            $('#btnClear').click(function () {
                location.reload();
                return false;
            });
            //添加
            $('#btnCreator').click(function () {
                edit('');
                return false;
            });
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: false,
                fitColumns: false,
                rownumbers: true,
                queryParams: getQuery(),
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                name: $.trim($('#name').textbox("getText")),
            };
            return params;
        };
        //操作
        function btn_formatter(value, rec) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            arry.push('<button class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + rec.ID + '\');return false;">编辑</button>  ');
            arry.push('<button class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + rec.ID + '\');return false;">删除</button>  ');
            arry.push('</span>');
            return arry.join('');
        }
        //编辑
        function edit(id) {
            var title = "新增";
            if (id) {
                title = "编辑";
            }
            $.myWindow({
                title: title,
                url: 'Edit.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: 700,
                height:400,
            });
            return false;
        }
        //删除
        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除所选项！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function (res) {
                        var result = JSON.parse(res);
                        if (result.success) {
                            top.$.timeouts.alert({ position: "TC", msg: result.message, type: "success" });
                            $.myWindow.close();
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: result.message, type: "error" });
                        }
                        $("#tab1").myDatagrid('flush');
                    })
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
                <td style="width: 90px;">名称或工号</td>
                <td>
                    <input id="name" class="easyui-textbox" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnCreator" class="easyui-linkbutton" iconcls="icon-yg-add">添加</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="换班员工列表">
        <thead>
            <tr>
                <th data-options="field:'StaffName',align:'center',width:120">名称</th>
                <th data-options="field:'SelCode',align:'center',width:120">工号</th>
                <th data-options="field:'DepartmentCode',align:'center',width:120">部门</th>
                <th data-options="field:'Current',align:'center',width:120">当前班别</th>
                <th data-options="field:'Next',align:'center',width:120">下次班别</th>
                <th data-options="field:'Creator',align:'center',width:120">创建人</th>
                <th data-options="field:'CreateDate',align:'center',width:150">创建时间</th>
                <th data-options="field:'UpdateDate',align:'center',width:150">更新时间</th>
                <th data-options="field:'btn',align:'center',formatter:btn_formatter,width:150">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
