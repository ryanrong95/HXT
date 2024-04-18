<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Order.OrderChange.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script>

        //alert('top:' + 1);

    </script>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>我的跟单-订单变更</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%-- <script>
        gvSettings.fatherMenu = '我的跟单(XDT)';
        gvSettings.menu = '订单变更';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        $(function () {
            //代理订单列表初始化
            $('#orders').myDatagrid({
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });
        });

        //查询
        function Search() {
            var orderID = $('#OrderID').textbox('getValue');
            var clientCode = $('#ClientCode').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            $('#orders').myDatagrid('search', { OrderID: orderID, ClientCode: clientCode, StartDate: startDate, EndDate: endDate });
        }

        //重置查询条件
        function Reset() {
            $('#OrderID').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //查看订单详情
        function View(ID) {
            var url = location.pathname.replace(/List.aspx/ig, 'Detail.aspx') + '?ID=' + ID;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '详情',
                width: '80%',
                height: '80%'
            });
        }
        //对账单
        function OrderBill(ID) {
            var url = location.pathname.replace(/List.aspx/ig, '../Bill/OrderBill.aspx') + '?ID=' + ID + '&From=OrderChange';
            window.location = url;
        }

        function EditFee(ID) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?ID=' + ID + '&From=OrderChange';;
            window.location = url;
        }

        function ToDeal(id) {
            $.messager.confirm('确认', '是否确认处理！', function (success) {
                if (success) {
                    $.post('?action=ToDeal', { ID: id }, function (res) {
                        var result = JSON.parse(res);
                        $('#orders').myDatagrid('reload');
                        $.messager.alert('消息', result.message, "info", function (r) {
                        });
                    })
                }
            });
        }

        function DealArrivalException(ID, OrderID) {
            var url = location.pathname.replace(/List.aspx/ig, 'ArrivalEdit.aspx') + '?ID=' + ID + '&OrderID=' + OrderID;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '到货变更',
                width: '80%',
                height: '80%'
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = "";
            if (row.Type == parseInt('<%=Needs.Ccs.Services.Enums.OrderChangeType.TaxChange.GetHashCode()%>')) {
                buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="EditFee(\'' + row.OrderID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看税费</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ToDeal(\'' + row.OrderID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">处理</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';

            }
            else if (row.Type == parseInt('<%=Needs.Ccs.Services.Enums.OrderChangeType.ProductChange.GetHashCode()%>')) {
                buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.OrderID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">详情</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ToDeal(\'' + row.OrderID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">处理</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';

            } else {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="DealArrivalException(\'' + row.ID + '\',\'' + row.OrderID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">到货变更</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            }

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="OrderBill(\'' + row.OrderID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">对账单</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" data-options="validType:'length[1,50]'" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" data-options="validType:'length[1,50]'" />
                    <br />
                    <span class="lbl">报关日期: </span>
                    <input class="easyui-datebox" id="StartDate" data-options="editable:false" />
                    <span class="lbl">至 </span>
                    <input class="easyui-datebox" id="EndDate" data-options="editable:false" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="orders" title="订单变更" data-options="border:false,nowrap:false,fitColumns:true,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'left'" style="width: 10%;">订单编号</th>
                    <th data-options="field:'EntryID',align:'center'" style="width: 12%;">报关单号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 7%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 15%;">客户名称</th>
                    <th data-options="field:'DDdate',align:'center'" style="width: 8%;">报关日期</th>
                    <th data-options="field:'OrderChangeType',align:'center'" style="width: 8%;">类型</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 10%;">添加时间</th>
                    <th data-options="field:'processState',align:'center'" style="width: 8%;">状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 20%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

