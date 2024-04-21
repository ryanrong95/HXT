<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Schedulings.List" %>

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
            //员工班别分配
            $('#btnAssignment').click(function () {
                Assignment();
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
                s_name: $.trim($('#s_name').textbox("getText")),
            };
            return params;
        };
        //编辑
        function edit(id) {
            var title = "新增班别";
            if (id) {
                title = "编辑班别";
            }
            $.myWindow({
                title: title,
                url: 'Edit.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "1000px",
                height: "500px"
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
        //分配班别
        function Assignment() {
            $.myWindow({
                title: "员工班别分配",
                minWidth: 1000,
                url: 'Assignment.aspx', onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }

        function btn_formatter(value, rec) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            arry.push('<button class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + rec.ID + '\');return false;">编辑</button>  ');
            arry.push('<button class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + rec.ID + '\');return false;">删除</button>  ');
            arry.push('</span>');
            return arry.join('');
        }

        function summary_formatter(value, row) {
            if (value && value.length > 15) {
                return value.substring(0, 15) + "...";
            } else {
                return value;
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">名称</td>
                <td>
                    <input id="s_name" class="easyui-textbox" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnCreator" class="easyui-linkbutton" iconcls="icon-yg-add">添加</a>
                    <em class="toolLine"></em>
                    <a id="btnAssignment" class="easyui-linkbutton" iconcls="icon-yg-assign">员工班别分配</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="班别列表">
        <thead>
            <tr>
                <th data-options="field:'Name',width:120">名称</th>
                <th data-options="field:'AmStartTime',width:120">上午开始时间</th>
                <th data-options="field:'AmEndTime',width:120">上午结束时间</th>
                <th data-options="field:'PmStartTime',width:120">下午开始时间</th>
                <th data-options="field:'PmEndTime',width:120">下午结束时间</th>
                <th data-options="field:'DomainValue',width:100">阈值(分钟)</th>
                <th data-options="field:'Summary',width:250,formatter:summary_formatter">备注</th>
                <th data-options="field:'btn',formatter:btn_formatter,width:150">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
