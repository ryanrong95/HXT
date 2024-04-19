<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Oss.Orders.List" %>
<%@ Import Namespace="Needs.Utils.Descriptions" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单列表</title>
    <uc:EasyUI runat="server"></uc:EasyUI>
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = '订单管理';
        gvSettings.menu = '订单列表';
        gvSettings.summary = '';

    </script>

    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                orderid: $.trim($('#_orderid').val()),
                user: $.trim($('#_user').val()),
                status: $('#_status').combobox('getValue')
            };
            return params;
        };
        $(function () {
            $("#tab1").bvgrid({
                queryParams: getQuery()
            });
        });

        var btnformatter = function (val, rec) {
            var arry = [
               '<button style="cursor:pointer;" v_name="detailbtn" v_title="列表项按钮-详情" onclick="detail(\'' + rec.ID + '\');">详情</button>'
            ];
            if (rec.Status == 1 || rec.Status == 2) {
                arry.push('<button style="cursor:pointer;" v_name="completebtn" v_title="列表项按钮-完成" onclick="complete(\'' + rec.ID + '\');">完成</button>');
                arry.push('<button style="cursor:pointer;" v_name="colsebtn" v_title="列表项按钮-关闭" onclick="abandon(\'' + rec.ID + '\');">关闭</button>');
            }
            if (rec.Total > rec.Paid && (rec.Status == 1 || rec.Status == 2)) {
                arry.push('<button style="cursor:pointer;" v_name="paybtn" v_title="列表项按钮-代支付" onclick="pay(\'' + rec.ID + '\');">代支付</button>');
            }

            return arry.join('');
        };
        // 支付
        var pay = function (id) {
            top.$.myWindow({
                url: location.pathname.replace(/Orders\/List.aspx/ig, 'OrderAgent/Edit.aspx') + '?orderid=' + id,
                noheader: false,
                title: '订单代支付',
                onClose: function () {
                    $("#tab1").bvgrid({
                        queryParams: getQuery()
                    });
                }
            }).open();
        };
        // 详情
        var detail = function (id) {
            top.$.myWindow({
                url: location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?orderid=' + id,
                noheader: false,
                title: '订单详情',
                onClose: function () {
                    $("#tab1").bvgrid({
                        queryParams: getQuery()
                    });
                }
            }).open();
        };
        // 完成
        var complete = function (id) {
            $.messager.confirm('提示', '您确定要完成订单 [' + id + '] 吗?', function (r) {
                if (r) {
                    $.post("?action=complete", { orderid: id }, function (data) {
                        if (data) {
                            $.messager.alert('提示', '操作成功');
                            $("#tab1").bvgrid({
                                queryParams: getQuery()
                            }); ''
                        }
                        else {
                            $.messager.alert('提示', '操作失败');
                        }
                    })
                }
            });
        };
        // 关闭
        var abandon = function (id) {
            $.messager.confirm('提示', '您确定要关闭订单 [' + id + '] 吗?', function (r) {
                if (r) {
                    $.post("?action=close", { orderid: id }, function (data) {
                        if (data) {
                            $.messager.alert('提示', '关闭成功');
                            $("#tab1").bvgrid({
                                queryParams: getQuery()
                            });
                        }
                        else {
                            $.messager.alert('提示', '关闭失败');
                        }
                    })
                }
            });
        };
    </script>
    <!--搜索按钮-->
    <script>
        $(function () {
            $('#btnSearch').click(function () {
                $("#tab1").bvgrid({
                    queryParams: getQuery()
                });
            });
            $('#btnClear').click(function () {
                location.href = location.href;
            });
        });
    </script>


</head>
<body class="easyui-layout">
    <div title="搜索项目" data-options="region:'north',border:false" style="height: 155px">
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <th style="width: 90px;">订单号
                </th>
                <td>
                    <input type="text" class="easyui-textbox" prompt="订单号" id="_orderid" maxlength="50" style="width: 160px;" value="" />
                </td>
                <th style="width: 90px;">用户
                </th>
                <td>
                    <input type="text" class="easyui-textbox" prompt="用户ID|用户名" id="_user" maxlength="50" style="width: 160px;" value="" />
                </td>

                <th style="width: 90px;">状态
                </th>
                <td>
                    <select id="_status" class="easyui-combobox" style="width: 160px;">
                        <option value="-1">全部</option>
                        <%
                            foreach (var item in Enum.GetValues(typeof(NtErp.Wss.Oss.Services.OrderStatus)))
                            {
                                var element = (NtErp.Wss.Oss.Services.OrderStatus)item;
                        %>
                        <option value="<%=(int)element %>"><%=element.GetDescription() %></option>
                        <%
                            }
                        %>
                    </select>
                </td>
            </tr>

            <tr>
                <td colspan="6">
                    <button id="btnSearch" class="easyui-linkbutton" iconcls="icon-search">搜索</button>
                    <button id="btnClear" class="easyui-linkbutton" iconcls="icon-clear">清空</button>
                </td>
            </tr>
        </table>

    </div>
    <div data-options="region:'center',border:false">
        <table id="tab1" title="订单列表" data-options="fitColumns:true,border:false,fit:true" class="mygrid">
            <thead>
                <tr>
                    <th g_name="ID" v_title="列表项列-ID" data-options="field:'ID'">ID</th>
                    <th g_name="UserID" v_title="列表项列-用户ID" data-options="field:'UserID'">用户ID</th>
                    <th g_name="UserName" v_title="列表项列-用户名" data-options="field:'UserName'">用户名</th>
                    <th g_name="District" v_title="列表项列-交货地" data-options="field:'District'">交货地</th>
                    <th g_name="Currency" v_title="列表项列-币种" data-options="field:'Currency'">币种</th>
                    <th g_name="Total" v_title="列表项列-订单总额" data-options="field:'Total'">订单总额</th>
                    <th g_name="Paid" v_title="列表项列-已支付" data-options="field:'Paid'">已支付</th>
                    <th g_name="Unpaid" v_title="列表项列-未支付" data-options="field:'Unpaid'">未支付</th>
                    <th g_name="Transport" v_title="列表项列-运输方式" data-options="field:'Transport'">运输方式</th>
                    <th g_name="StatusStr" v_title="列表项列-状态" data-options="field:'StatusStr'">状态</th>
                    <th g_name="CreateDate" v_title="列表项列-创建时间" data-options="field:'CreateDate'">创建时间</th>
                    <th g_name="UpdateDate" v_title="列表项列-更新时间" data-options="field:'UpdateDate'">最后更新时间</th>
                    <th g_name="btn" v_title="列表项按钮组" data-options="field:'btn',formatter:btnformatter" style="width: 100px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

