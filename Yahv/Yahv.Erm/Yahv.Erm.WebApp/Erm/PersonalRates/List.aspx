<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.PersonalRates.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        //页面加载
        $(function () {
            $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: false,
                fitColumns: true,
                nowrap: false,
            });
        });
        //员工新增
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            $.myDialog({
                title: "基本信息",
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                },
                width: '80%',
                height: '500px'
            });
        }

        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?ID=' + id;
            $.myDialog({
                title: "基本信息",
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                },
                width: '80%',
                height: '500px'
            });
        }

        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除所选项！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        //$.messager.alert('删除', '删除成功！');
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "删除成功!",
                            type: "success"
                        });
                        $("#tab1").myDatagrid('flush');
                    })
                }
            });
        }

        //列表内操作项
        function Operation(val, row, index) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            arry.push('<button class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="Edit(\'' + row.ID + '\');return false;" group>编辑</button> ');
            arry.push('<button class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + row.ID + '\');return false;" group>删除</button>');
            arry.push('</span>');
            return arry.join('');
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td>
                    <a id="btnCreator" class="easyui-linkbutton" iconcls="icon-yg-add" onclick="Add()">新建</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="个人所得税预扣表">
        <thead>
            <tr>
                <th data-options="field:'Levels',width:100">级数</th>
                <th data-options="field:'BeginAmount',width:100">起始金额</th>
                <th data-options="field:'EndAmount',width:100">结束金额</th>
                <th data-options="field:'Rate',width:100">预扣率（%）</th>
                <th data-options="field:'Deduction',width:100">速算扣除数</th>
                <th data-options="field:'CreateDate',width:100">创建时间</th>
                <th data-options="field:'btn',formatter:Operation,width:100">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
