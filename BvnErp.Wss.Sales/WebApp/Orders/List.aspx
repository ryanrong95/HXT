<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Orders.List" %>

<%@ Import Namespace="NtErp.Wss.Sales.Services.Utils.Structures" %>

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
        $(function () {
            $("#tab1").bvgrid({
                queryParams: getQuery()
            });
        });

        var getQuery = function () {
            var params = {
                action: 'data',
                orderid: $.trim($('#_orderid').val()),
                user: $.trim($('#_user').val()),
                status: $.trim($('#_status').combobox('getValue')),
                district: $.trim($('#_district').combobox('getValue')),
                currency: $.trim($('#_currency').combobox('getValue')),
                transport: $.trim($('#_transport').combobox('getValue')),
                starttime: $.trim($('#starttime').datebox('getValue')),
                endtime: $.trim($('#endtime').datebox('getValue')),
                dratio: $('#_dratio').combobox('getValue'),
                pratio: $('#_pratio').combobox('getValue')
            };
            return params;
        };

        var btnformatter = function (val, rec) {
            var arry = [
               '<button style="cursor:pointer;" v_name="detailbtn" v_title="列表项详情按钮" onclick="detail(\'' + rec.ID + '\');">详情</button>'
            ];
            if (rec.StatusStr == '等待支付') {
                arry.push('<button style="cursor:pointer;" v_name="completebtn" v_title="列表项完成按钮" onclick="complete(\'' + rec.ID + '\');">完成</button>');
                if (!rec.IsSend) {
                    arry.push('<button style="cursor:pointer;" v_name="colsebtn" v_title="列表项关闭按钮" onclick="abandon(\'' + rec.ID + '\');">关闭</button>');
                }
                arry.push('<button style="cursor:pointer;" v_name="paymentbtn" v_title="列表项待支付按钮"  onclick="payment(\'' + rec.ID + '\');">代支付</button>');
                //arry.push('<button style="cursor:pointer;" v_name="deliverybtn" v_title="列表项发货比例按钮" onclick="delivery(\'' + rec.ID + '\');">发货比例</button>');
            }

            return arry.join('');
        };

        var delivery = function (id) {
            top.$.myWindow({
                url: location.pathname.replace(/List.aspx/ig, 'Delivery/Edit.aspx') + '?id=' + id,
                onClose: function () {
                    $("#tab1").bvgrid({
                        queryParams: getQuery()
                    });
                }
            }).open();
        };

        var detail = function (id) {
            top.$.myWindow({
                url: location.pathname.replace(/List.aspx/ig, 'Detail.aspx') + '?id=' + id,
                onClose: function () {
                    $("#tab1").bvgrid({
                        queryParams: getQuery()
                    });
                }
            }).open();
        };
        var complete = function (id) {
            $.messager.confirm('提示', '您确定要完成订单 [' + id + '] 吗?', function (r) {
                if (r) {
                    $.post("?action=complete", { id: id }, function (data) {
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
        var abandon = function (id) {
            $.messager.confirm('提示', '您确定要关闭订单 [' + id + '] 吗?', function (r) {
                if (r) {
                    $.post("?action=close", { id: id }, function (data) {
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
        var payment = function (id) {
            top.$.myWindow({
                url: '/Erp/Accounts/Payment.aspx?id=' + id,
                onClose: function () {
                    $("#tab1").bvgrid('flush');
                }
            }).open();
        }

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
    <!--搜索时间-->
    <script type="text/javascript">

        function onSelect(d) {
            var sd = new Date($('#starttime').datebox('getValue'));
            var ed = new Date($('#endtime').datebox('getValue'));
            $('#starttime').val(sd);
            if (ed < sd) {
                $.messager.alert("提示", "结束日期小于开始日期!");
                //只要选择了日期，不管是开始或者结束都对比一下，如果结束小于开始，则清空结束日期的值并弹出日历选择框  
                $('#endtime').datebox('setValue', '').datebox('showPanel');
            }
            else {
                $('#endtime').val(ed);
            }
        }
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
                            foreach (var item in Enum.GetValues(typeof(NtErp.Wss.Sales.Services.Underly.Orders.OrderStatus)))
                            {
                                var element = (NtErp.Wss.Sales.Services.Underly.Orders.OrderStatus)item;
                        %>
                        <option value="<%=(int)element %>"><%=element.GetTitle() %></option>
                        <%
                            }
                        %>
                    </select>
                </td>
            </tr>
            <tr>
                <th style="width: 90px;">交货地
                </th>
                <td>
                    <select id="_district" class="easyui-combobox" style="width: 160px;">
                        <option value="-1">全部</option>
                        <%
                            foreach (var item in Enum.GetValues(typeof(NtErp.Wss.Sales.Services.Underly.District)))
                            {
                                var element = (NtErp.Wss.Sales.Services.Underly.District)item;
                        %>
                        <option value="<%=(int)element %>"><%=element.GetTitle() %></option>
                        <%
                            }
                        %>
                    </select>
                </td>
                <th style="width: 90px;">币种
                </th>
                <td>
                    <select id="_currency" class="easyui-combobox" style="width: 160px;">
                        <option value="-1">全部</option>
                        <%
                            foreach (var item in Enum.GetValues(typeof(NtErp.Wss.Sales.Services.Underly.Currency)))
                            {
                                var element = (NtErp.Wss.Sales.Services.Underly.Currency)item;
                        %>
                        <option value="<%=(int)element %>"><%=element %></option>
                        <%
                            }
                        %>
                    </select>
                </td>
                <th style="width: 90px;">运输方式
                </th>
                <td>
                    <select id="_transport" class="easyui-combobox" style="width: 160px;">
                        <option value="-1">全部</option>
                        <%
                            foreach (var item in Enum.GetValues(typeof(NtErp.Wss.Sales.Services.Underly.Orders.TransportTerm)))
                            {
                                var element = (NtErp.Wss.Sales.Services.Underly.Orders.TransportTerm)item;
                        %>
                        <option value="<%=element %>"><%=element.GetTitle() %></option>
                        <%
                            }
                        %>
                    </select>
                </td>
            </tr>
            <tr>
                <th style="width: 90px;">支付状态
                </th>
                <td>
                    <select id="_pratio" class="easyui-combobox" style="width: 160px;">
                        <option value="-1">全部</option>
                        <option value="1">待支付</option>
                        <option value="2">支付部分</option>
                        <option value="3">支付完成</option>

                    </select>
                </td>
                <th style="width: 90px;">发货状态
                </th>
                <td>
                    <select id="_dratio" class="easyui-combobox" style="width: 160px;">
                        <option value="-1">全部</option>
                        <option value="1">待发货</option>
                        <option value="2">发货部分</option>
                        <option value="3">发货完成</option>
                    </select>
                </td>
                 <th style="width: 90px;">创建时间
                </th>
                <td>
                    <input class="easyui-datebox" style="width: 160px" id="starttime" value="" data-options="onSelect:onSelect" />
                    至
                    <input class="easyui-datebox" style="width: 160px" id="endtime" value="" data-options="onSelect:onSelect" />
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
                    <th data-options="field:'ID'">ID</th>
                    <th data-options="field:'UserID'">用户ID</th>
                    <th data-options="field:'SiteUserName'">用户名</th>
                    <th data-options="field:'District'">交货地</th>
                    <th data-options="field:'Currency'">币种</th>
                    <th data-options="field:'Transport'">运输方式</th>
                    <th data-options="field:'StatusStr'">状态</th>
                    <th data-options="field:'DeliveryRatio'">发货比例</th>
                    <th data-options="field:'PaidRatio'">支付比例</th>
                    <th data-options="field:'CreateDate'">创建时间</th>
                    <th data-options="field:'UpdateDate'">最后更新时间</th>
                    <th data-options="field:'btn',formatter:btnformatter" g_name="btn" v_title="列表项操作按钮组" style="width: 100px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
