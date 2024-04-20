<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Clients.List" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>我的客户</title>
    <uc:EasyUI runat="server"></uc:EasyUI>
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = '网站客户管理';
        gvSettings.menu = '我的客户';
        gvSettings.summary = '';
    </script>
    <script>
        $(function () {
            $('#t_client').bvgrid({
                pageSize: 15,
                formatters: {
                    Btns: function (val, rowData, index) {
                        var arry = [
                            '<button type="button" class="btn_view" dataid=' + rowData.ID + '>查看详情</button>',
                            '<button type="button" class="btn_cash" dataid=' + rowData.ID + '>现金充值</button>',
                            '<button type="button" class="btn_creditrecharge" dataid=' + rowData.ID + '>信用充值</button>',
                            '<button type="button" class="btn_credit" dataid=' + rowData.ID + '>代还信用</button>',
                            '<button type="button" class="btn_cart" dataid=' + rowData.ID + '>购物车</button>',
                            '<button type="button" class="btn_bom" dataid=' + rowData.ID + '>bom询价</button>'
                        ];
                        return arry.join('|');
                    }
                },
                onSelect: function () {
                    (window.parent || window).currentData = arguments[1];
                }
            });
            $('.btn_search').click(function () {
                var param = {};
                $('.search [name]').each(function () {
                    var value = $(this).val();
                    if (!!value) {
                        param[$(this).attr('name')] = value;
                    }
                });
                $('#t_client').bvgrid('search', param);
            });
            $('.btn_clear').click(function () {
                $('.search [class^="easyui-"]').each(function () {
                    $(this)[$(this).attr('class').split(' ')[0].replace('easyui-', '')]('clear');
                });
                $('#t_client').bvgrid('reset');
            });
            $('body').delegate('.btn_view', 'click', function () {
                top.$.myWindow({
                    url: location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?id=' + $(this).attr('dataid')
                }).open();
            });
            $('body').delegate('.btn_credit', 'click', function () {
                top.$.myWindow({
                    //url: location.pathname.replace("Clients/List.aspx", 'Accounts/Credits/List.aspx') + '?id=' + $(this).attr('dataid')
                    url: '/Wss/Oss/Oss/Credits/Edit.aspx?id=' + $(this).attr('dataid')
                }).open();
            });
            $('body').delegate('.btn_cash', 'click', function () {
                top.$.myWindow({
                    url: '/wss/oss/Recharge/edit.aspx?id=' + $(this).attr('dataid') + '&type=Cash'
                }).open();
            });
            $('body').delegate('.btn_creditrecharge', 'click', function () {
                top.$.myWindow({
                    url: '/wss/oss/Recharge/edit.aspx?id=' + $(this).attr('dataid') + '&type=Credit'
                }).open();
            });
            $('body').delegate('.btn_cart', 'click', function () {
                top.$.myWindow({
                    url: '/Wss/Sales/Carts/List.aspx?uid=' + $(this).attr('dataid')
                }).open();
            });
            $('body').delegate('.btn_bom', 'click', function () {
                top.$.myWindow({
                    url: '/DvEps/Boms/Edit.aspx?uid=' + $(this).attr('dataid'),
                    noheader: false,
                    title: '新增Bom',
                    width:'80%',
                    height:'65%',
                    onClose: function () {
                        $('#t_client').bvgrid('reload');
                    }
                }).open();
            });
        });
    </script>
</head>
<body class="easyui-layout" fit="true">
    <div data-options="region:'north'" style="height: 125px" title="搜索">
        <table class="liebiao search">
            <tr>
                <td style="width: 15%">ID</td>
                <td style="width: 35%">
                    <input class="easyui-textbox" name="ID" style="width: 80%" data-options="prompt:'输入完整ID'" /></td>
                <td style="width: 15%">用户名</td>
                <td style="width: 35%">
                    <input class="easyui-textbox" name="UserName" style="width: 80%" data-options="prompt:'输入用户名前几个字符或全部'" /></td>
            </tr>
            <tr>
                <td>邮箱</td>
                <td>
                    <input class="easyui-textbox" name="Email" style="width: 80%" data-options="prompt:'输入邮箱任意一段字符或全部'" /></td>
                <td>手机</td>
                <td>
                    <input class="easyui-textbox" name="Mobile" style="width: 80%" data-options="prompt:'输入手机后几位或全部'" /></td>
            </tr>
            <tr>
                <td colspan="4">
                    <button type="button" class="btn_search">搜索</button>
                    <button type="button" class="btn_clear">清空</button>
                </td>
            </tr>
        </table>
    </div>
    <div data-options="region:'center'">
        <table id="t_client" data-options="title:'用户列表',fit:true">
            <thead>
                <tr>
                    <th data-options="field:'ID',width:80,align:'center'">ID</th>
                    <th data-options="field:'UserName',width:100,align:'center'">用户名</th>
                    <th data-options="field:'Email',width:80,align:'center'">邮箱</th>
                    <th data-options="field:'Mobile',width:80,align:'center'">手机</th>
                    <%--<th data-options="field:'StatusName',width:60,align:'center'">状态</th>--%>
                    <th data-options="field:'CreateDate',width:100,align:'center'">创建时间</th>
                    <th data-options="field:'Btns',align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
