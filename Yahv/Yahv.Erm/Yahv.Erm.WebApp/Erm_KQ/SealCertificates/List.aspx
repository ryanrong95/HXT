<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.SealCertificates.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: false,
                fitColumns: false,
                rownumbers: true,
                queryParams: getQuery(),
            });
            //添加
            $('#btnCreator').click(function () {
                edit('');
                return false;
            });
            //搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            //清空按钮
            $('#btnClear').click(function () {
                $('#name').textbox("setText", '');
                grid.myDatagrid('search', getQuery());
                return false;
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
        //编辑
        function edit(id) {
            var title = "新增印章证照";
            if (id) {
                title = "编辑印章证照";
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

        function btn_formatter(value, rec) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            arry.push('<button class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + rec.ID + '\');return false;">编辑</button>  ');
            arry.push('<button class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + rec.ID + '\');return false;">删除</button>  ');
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
                <td style="width: 90px;">名称</td>
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
    <table id="tab1" title="印章证照IC卡列表">
        <thead>
            <tr>
                <th data-options="field:'Name',width:200">名称</th>
                <th data-options="field:'Type',align:'center',width:120">类型</th>
                <th data-options="field:'ProcessingDate',align:'center',width:120">办理日期</th>
                <th data-options="field:'DueDate',align:'center',width:120">到期日期</th>
                <th data-options="field:'Staff',align:'center',width:120">保管人</th>
                <th data-options="field:'btn',align:'center',formatter:btn_formatter,width:150">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
