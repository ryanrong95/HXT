<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Erp.Orders.List" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单列表</title>
    <uc:EasyUI runat="server"></uc:EasyUI>
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        window.gvSettings.fatherMenu = '订单管理';
        window.gvSettings.menu = '订单列表';
        window.gvSettings.summary = '';
    </script>

    <script>

        $(function () {
            $("#tab1").bvgrid();
        });

        var enumformatter = function () {

        };

        var btnformatter = function (val, rec) {
            var arry = [
                '<button style="cursor:pointer;" v_name="detailbtn" v_title="列表项详情按钮" onclick="detail(\'' + rec.ID + '\');">详情</button>',
                '<button style="cursor:pointer;" v_name="completebtn" v_title="列表项完成按钮" onclick="complete(\'' + rec.ID + '\');">完成</button>',
                '<button style="cursor:pointer;" v_name="colsebtn" v_title="列表项关闭按钮" onclick="close(\'' + rec.ID + '\');">关闭</button>'
            ];

            return arry.join('');
        };

        var detail = function (id) {
            top.$.myWindow({
                url: location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?id=' + id, onClose: function () {
                    $("#tab1").bvgrid();
                }
            }).open();
        };
        var complete = function (id) {
            $.messager.confirm('提示', '您确定要完成订单 [' + id + '] 吗?', function (r) {
                if (r) {
                    $.post("?action=complete", { id: id }, function (data) {
                        $.messager.alert('提示', '关闭成功');
                        $("#tab1").bvgrid();
                    })
                }
            });
        };
        var close = function (id) {
            $.messager.confirm('提示', '您确定要关闭订单 [' + id + '] 吗?', function (r) {
                if (r) {
                    $.post("?action=close", { id: id }, function (data) {
                        $.messager.alert('提示', '关闭成功');
                        $("#tab1").bvgrid();
                    })
                }
            });
        };
    </script>



</head>
<body class="easyui-layout">
    <div title="搜索项目" data-options="region:'north',border:false" style="height: 110px; padding: 10px 0px;">
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <th style="width: 90px;">角色
                </th>
                <td>
                    <input type="text" class="easyui-textbox" prompt="名称&ID" id="_rolename" maxlength="50" style="width: 264px;" value="" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <button id="btnSearch" class="easyui-linkbutton" iconcls="icon-search">搜索</button>
                    <button id="btnClear" class="easyui-linkbutton" iconcls="icon-clear">清空</button>
                </td>
            </tr>
        </table>

    </div>
    <div data-options="region:'center',border:false">
        <table id="tab1" title="开发手记" data-options="fitColumns:true,border:false,fit:true" class="mygrid">
            <thead>
                <tr>
                    <th data-options="field:'ID'">ID</th>
                    <th data-options="field:'UserID'">用户ID</th>
                    <th data-options="field:'UserName'">用户名</th>
                    <th data-options="field:'District',formatter:enumformatter">交货地</th>
                    <th data-options="field:'Currency'">币种</th>
                    <th data-options="field:'Transport'">运输公司</th>
                    <th data-options="field:'Status'">状态</th>
                    <th data-options="field:'CreateDate'">创建时间</th>
                    <th data-options="field:'UpdateDate'">最后更新时间</th>
                    <th data-options="field:'Summary'">备注</th>
                    <th data-options="field:'btn',formatter:btnformatter" g_name="btn" v_title="列表项操作按钮组" style="width: 100px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
