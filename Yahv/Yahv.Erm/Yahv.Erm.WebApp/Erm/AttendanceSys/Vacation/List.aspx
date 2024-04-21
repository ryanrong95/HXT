<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.AttendanceSys.Vacation.List" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: true
            });
            // 假期类型
            $('#Type').combobox({
                data: model.VacationType,
                editable: false,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
            });
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $('#tab1').datagrid('reload');
                return false;
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                Type: $.trim($('#Type').combobox("getValue")),
                StartDate: $.trim($('#StartDate').datebox("getValue")),
                EndDate: $.trim($('#EndDate').datebox("getValue"))
            };
            return params;
        };
        //编辑
        function Edit(id) {
            $.myDialog({
                title: "编辑",
                url: 'Edit.aspx?id=' + id,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
            return false;
        }
        //删除
        function Delete(id) {
            $.messager.confirm('确认', '请您确认是否删除所选项。', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function (res) {
                        var res = JSON.parse(res);
                        if (res.success) {
                            $.messager.alert('删除', '删除成功!');
                        }
                        else {
                            $.messager.alert('删除', '删除失败!');
                        }
                        $('#tab1').datagrid('reload');
                    })
                }
            });
        }
        //操作
        function btn_formatter(value, rec) {
            return ['<span class="easyui-formatted">',
                '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="Edit(\'' + rec.ID + '\');return false;">编辑</a> ',
                '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-cancel\'" onclick="Delete(\'' + rec.ID + '\');return false;">删除</a> ',
                '</span>'].join('');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">假期类型</td>
                <td>
                    <input id="Type" class="easyui-combobox" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">开始时间</td>
                <td>
                    <input id="StartDate" class="easyui-datebox" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">结束时间</td>
                <td>
                    <input id="EndDate" class="easyui-datebox" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnAdd" class="easyui-linkbutton" iconcls="icon-yg-add">新增假期</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="节假日管理">
        <thead>
            <tr>
                <th data-options="field:'Date',align:'center',width:100">假期日期</th>
                <th data-options="field:'Type',align:'center',width:100">假期类型</th>
                <th data-options="field:'Salary',align:'center',width:100">薪资标准</th>
                <th data-options="field:'Summary',width:200">备注</th>
                <th data-options="field:'btn',align:'center',formatter:btn_formatter,width:100">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
