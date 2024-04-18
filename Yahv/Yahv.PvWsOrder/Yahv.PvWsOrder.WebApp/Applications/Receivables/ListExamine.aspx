<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListExamine.aspx.cs" Inherits="Yahv.PvOms.WebApp.Applications.Receivables.ListExamine" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                singleSelect: true,
                fitColumns: false,
                fit: true,
            });
            //审批状态
            $('#ApplicationStatus').combobox({
                data: model.ApplicationStatus,
                editable: false,
                valueField: 'value',
                textField: 'text',
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            });
            //收款状态
            $('#ReceiveStatus').combobox({
                data: model.ReceiveStatus,
                editable: false,
                valueField: 'value',
                textField: 'text',
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            });
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                $('#ApplicationID').textbox("setValue", "");
                $('#OrderID').textbox("setValue", "")
                $('#ClientName').textbox("setValue", "")
                $('#EnterCode').textbox("setValue", "")
                $('#ApplicationStatus').combobox("setValue", "")
                $('#ReceiveStatus').combobox("setValue", "")
                $('#StartDate').datebox("setValue", "");
                $('#EndDate').datebox("setValue", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                ApplicationID: $.trim($('#ApplicationID').textbox("getText")),
                OrderID: $.trim($('#OrderID').textbox("getText")),
                ClientName: $.trim($('#ClientName').textbox("getText")),
                EnterCode: $.trim($('#EnterCode').textbox("getText")),
                ApplicationStatus: $.trim($('#ApplicationStatus').combobox("getValue")),
                ReceiveStatus: $.trim($('#ReceiveStatus').combobox("getValue")),
                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
            };
            return params;
        };
        //审核
        function Examine(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            var url = location.pathname.replace('ListExamine.aspx', 'Examine.aspx?ID=' + data.ID)
            window.location = url;
            return false;
        }
        //详情
        function Detail(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            var url = location.pathname.replace('ListExamine.aspx', 'Details.aspx?ID=' + data.ID + '&Source=1')
            window.location = url;
            return false;
        }
        //收款核销
        function Receive(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: "收款核销",
                url: '../Finance/WriteOffReceive.aspx?ID=' + data.ID,
                width: 1200,
                height: 600,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }
        //已收支票
        function CheckReceived(index) {
            var ID = $("#tab1").myDatagrid('getRows')[index].ID
            $.messager.confirm('确认', '请您确认是否收票完成！', function (success) {
                if (success) {
                    $.post('?action=CheckReceived', { ID: ID }, function (result) {
                        var res = JSON.parse(result);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                        $('#tab1').datagrid('reload');
                    })
                }
            });
            return false;
        }
        //已送支票
        function CheckDelivered(index) {
            var ID = $("#tab1").myDatagrid('getRows')[index].ID
            $.messager.confirm('确认', '请您确认是否送票完成！', function (success) {
                if (success) {
                    $.post('?action=CheckDelivered', { ID: ID }, function (result) {
                        var res = JSON.parse(result);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                        $('#tab1').datagrid('reload');
                    })
                }
            });
            return false;
        }
        //操作
        var Examining = <%=Yahv.PvWsOrder.Services.Enums.ApplicationStatus.Examining.GetHashCode()%>;
        var Examined = <%=Yahv.PvWsOrder.Services.Enums.ApplicationStatus.Examined.GetHashCode()%>;
        var Approved = <%=Yahv.PvWsOrder.Services.Enums.ApplicationStatus.Approved.GetHashCode()%>;
        var UnReceive = <%=Yahv.PvWsOrder.Services.Enums.ApplicationReceiveStatus.UnReceive.GetHashCode()%>;
        var UnPay = <%=Yahv.PvWsOrder.Services.Enums.ApplicationPaymentStatus.UnPay.GetHashCode()%>;
        function Operation(val, row, index) {
            var buttons = [];
            buttons.push('<span class="easyui-formatted">');
            if (row.ApplicationStatus == Examining) {
                buttons.push('<a class="easyui-linkbutton"  data-options="iconCls:\'icon-man\'" onclick="Examine(' + index + ');return false;">审核</a> ');
            }
            else {
                buttons.push('<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="Detail(' + index + ');return false;">详情</a> ');
                if (row.ApplicationStatus == Examined) {
                    if (row.IsEntry == true) {
                        if (row.ReceiveStatus == UnReceive) {
                            buttons.push('<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-sealing\'" onclick="Receive(' + index + ');return false;">收款核销</a> ');
                        }
                    }
                    else {
                        if (row.ReceiveStatus == UnReceive) {
                            buttons.push('<a class="easyui-linkbutton"  data-options="iconCls:\'icon-man\'" onclick="CheckReceived(' + index + ');return false;">已收支票</a> ');
                        }
                        else {
                            if (row.PaymentStatus == UnPay) {
                                buttons.push('<a class="easyui-linkbutton"  data-options="iconCls:\'icon-man\'" onclick="CheckDelivered(' + index + ');return false;">已送支票</a> ');
                            }
                        }
                    }
                }
            }
            buttons.push('</span>')
            return buttons.join('');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--查询条件-->
        <table class="liebiao">
            <tr>
                <td style="width: 90px">客户名称</td>
                <td style="width: 200px">
                    <input id="ClientName" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px">入仓号</td>
                <td style="width: 200px">
                    <input id="EnterCode" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">订单编号</td>
                <td colspan="3">
                    <input id="OrderID" class="easyui-textbox" style="width: 150px" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">申请编号</td>
                <td style="width: 200px">
                    <input id="ApplicationID" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">审批状态</td>
                <td style="width: 200px">
                    <input id="ApplicationStatus" class="easyui-combobox" style="width: 150px" />
                </td>
                <td style="width: 90px;">收款状态</td>
                <td style="width: 200px">
                    <input id="ReceiveStatus" class="easyui-combobox" style="width: 150px" />
                </td>
                <td style="width: 90px">申请日期</td>
                <td>
                    <input id="StartDate" data-options="prompt:'开始日期'" class="easyui-datebox" style="width: 150px" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="EndDate" data-options="prompt:'结束日期'" class="easyui-datebox" style="width: 150px" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="代收货款申请">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">申请时间</th>
                <th data-options="field:'ID',align:'center'" style="width: 150px">申请编号</th>
                <th data-options="field:'ClientName',align:'left'" style="width: 200px">客户名称</th>
                <th data-options="field:'EnterCode',align:'center'" style="width: 100px">入仓号</th>
                <th data-options="field:'Type',align:'center'" style="width: 100px">申请类型</th>
                <th data-options="field:'TotalPrice',align:'center'" style="width: 100px">代收金额</th>
                <th data-options="field:'CurrencyDec',align:'center'" style="width: 100px">币种</th>
                <th data-options="field:'MethodDec',sortable :true,align:'center'" style="width: 100px">支付方式</th>
                <th data-options="field:'IsEntryDec',sortable :true,align:'center'" style="width: 100px">是否入账</th>
                <th data-options="field:'DelivaryOpportunity',sortable :true,align:'center'" style="width: 100px">发货时机</th>
                <th data-options="field:'ApplicationStatusDec',sortable :true,align:'center'" style="width: 100px">审批状态</th>
                <th data-options="field:'ReceiveStatusDec',sortable :true,align:'center'" style="width: 100px">收款状态</th>
                <%--<th data-options="field:'PaymentStatusDec',align:'center'" style="width: 100px">付款状态</th>--%>
                <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 180px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>

