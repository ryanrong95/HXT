<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Test.List" %>

<%@ Import Namespace="Yahv.PvWsOrder.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                singleSelect: false,
                fitColumns: false
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

            $('#btn').bind('click', function () {
                download();
            });
            $('#btn2').bind('click', function () {
                download2();
            });

        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                Name: $.trim($('#Name').textbox("getText"))
            };
            return params;
        };

        function Edit(id) {
            $.myDialog({
                title: "基本信息",
                url: 'Edit.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
            return false;
        }

        function Operation(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="Edit(\'' + row.ID + '\');return false;">编辑</a> '
                , '</span>'].join('');
        }
    </script>
    <script>
        function download() {
            $('#down').attr('href', '../Content/templates/LsOrder201912020015.xml');
            $('#down').attr('download', '');
            document.getElementById("down").click();
        }
        function download2() {
            $('#down').attr('href', 'http://uuws.b1b.com/Local/LsOrder201912040011.xml');
            $('#down').attr('download', '');
            document.getElementById("down").click();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">名称</td>
                <td>
                    <input id="Name" data-options="prompt:'用户名或真实名',validType:'length[1,75]'" class="easyui-textbox" style="width: 200px" />
                    <a id="btn" class="easyui-linkbutton" iconcls="icon-yg-search">本域下载</a>
                    <a id="btn2" class="easyui-linkbutton" iconcls="icon-yg-search">跨域下载</a>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <a id="down" hidden>点击测试</a>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <a id="btnAdd" class="easyui-linkbutton" iconcls="icon-yg-add">添加</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="管理员列表">
        <thead>
            <tr>
                <th data-options="field:'ID',width:109">ID</th>
                <th data-options="field:'UserName',width:150">用户名</th>
                <th data-options="field:'RealName',width:150">真实名</th>
                <th data-options="field:'RoleName',width:150">角色</th>
                <th data-options="field:'Tel',width:150">电话号码</th>
                <th data-options="field:'Mobile',width:150">手机号码</th>
                <th data-options="field:'Email',width:150">邮箱</th>
                <th data-options="field:'LastLoginDate',width:180">最后登入时间</th>
                <th data-options="field:'btn',formatter:Operation,width:200">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
