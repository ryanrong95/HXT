<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.WmsAdmins.List" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var checkboxer = function (row) {
            //console.log(row);
            return false;
        };

        var getQuery = function () {
            var params = {
                action: 'data',
                s_name: $.trim($('#s_name').textbox("getText"))
            };
            return params;
        };

        function btn_formatter(value, rec) {
            return ['<span class="easyui-formatted">',
                  , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-assign\'" onclick="wareHouse(\'' + rec.ID + '\');return false;">分配库房</a> '
                , '</span>'].join('');
        }
        function wareHouse(id) {
            $.myDialog({
                title: "分配库房",
                url: 'Edit.aspx?id=' + id,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
            return false;
        }
        $(function () {
            var status = [parseInt('<%=(int)Yahv.Underly.AdminStatus.Super%>')];

            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: true
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
                    <input id="s_name" data-options="prompt:'用户名或真实名',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="库房管理员">
        <thead>
            <tr>
                <%--<th data-options="field:'ck',checkbox:true"></th>--%>
                <th data-options="field:'ID',width:109">ID</th>
                <th data-options="field:'UserName',width:150">用户名</th>
                <th data-options="field:'RealName',width:150">真实名</th>
                <th data-options="field:'SelCode',width:150">编码</th>
                <th data-options="field:'RoleName',width:150">角色</th>
                <th data-options="field:'LastLoginDate',width:170">最后登入</th>
                <th data-options="field:'StatusName',width:111">状态</th>
                <th data-options="field:'btn',formatter:btn_formatter,width:120">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
