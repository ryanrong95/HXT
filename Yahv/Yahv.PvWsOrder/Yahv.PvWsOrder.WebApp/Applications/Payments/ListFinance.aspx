<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListFinance.aspx.cs" Inherits="Yahv.PvOms.WebApp.Applications.Payments.ListFinance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                singleSelect: true,
                fitColumns: false,
                fit: true,
                nowrap: true,
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
            //付款状态
            $('#PaymentStatus').combobox({
                data: model.PaymentStatus,
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
                $('#PaymentStatus').combobox("setValue", "")
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
                PaymentStatus: $.trim($('#PaymentStatus').combobox("getValue")),
                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
            };
            return params;
        };
        //银行收款
        function Receive(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: "银行收款录入",
                minWidth: 1200,
                minHeight: 600,
                url: location.pathname.replace('ListFinance.aspx', '../Finance/BankReceive.aspx?ID=' + data.ID),
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }
        //银行付款
        function Payment(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: "银行付款录入",
                minWidth: 1200,
                minHeight: 600,
                url: location.pathname.replace('ListFinance.aspx', '../Finance/BankPayment.aspx?ID=' + data.ID),
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }
        //申请详情
        function Detail(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            var url = location.pathname.replace('ListFinance.aspx', 'Details.aspx?ID=' + data.ID+'&Source=3')
            window.location = url;
            return false;
        }
        //操作
        var Examined = <%=Yahv.PvWsOrder.Services.Enums.ApplicationStatus.Examined.GetHashCode()%>;
        var Approved = <%=Yahv.PvWsOrder.Services.Enums.ApplicationStatus.Approved.GetHashCode()%>;
        var UnReceive = <%=Yahv.PvWsOrder.Services.Enums.ApplicationReceiveStatus.UnReceive.GetHashCode()%>;
        var UnPay = <%=Yahv.PvWsOrder.Services.Enums.ApplicationPaymentStatus.UnPay.GetHashCode()%>;
        function Operation(val, row, index) {
            var buttons = [];
            buttons.push('<span class="easyui-formatted">');
            buttons.push('<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="Detail(' + index + ');return false;">详情</a> ');
            if ((row.ApplicationStatus == Examined || row.ApplicationStatus == Approved) && row.ReceiveStatus == UnReceive) {
                buttons.push('<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-sealing\'" onclick="Receive(' + index + ');return false;">录入银行收款</a> ');
            }
            if (row.ApplicationStatus == Approved && row.PaymentStatus == UnPay) {
                buttons.push('<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-quote\'" onclick="Payment(' + index + ');return false;">录入银行付款</a> ');
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
                <td style="width: 90px;">申请编号</td>
                <td style="width: 200px">
                    <input id="ApplicationID" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">订单编号</td>
                <td>
                    <input id="OrderID" class="easyui-textbox" style="width: 150px" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">审批状态</td>
                <td style="width: 200px">
                    <input id="ApplicationStatus" class="easyui-combobox" style="width: 150px" />
                </td>
                <td style="width: 90px;">收款状态</td>
                <td style="width: 200px">
                    <input id="ReceiveStatus" class="easyui-combobox" style="width: 150px" />
                </td>
                <td style="width: 90px;">付款状态</td>
                <td style="width: 200px">
                    <input id="PaymentStatus" class="easyui-combobox" style="width: 150px" />
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
    <table id="tab1" title="代付货款申请">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">申请时间</th>
                <th data-options="field:'ID',align:'center'" style="width: 150px">申请编号</th>
                <th data-options="field:'ClientName',align:'left'" style="width: 200px">客户名称</th>
                <th data-options="field:'EnterCode',align:'center'" style="width: 100px">入仓号</th>
                <th data-options="field:'PayerName',align:'left'" style="width: 200px">付款人</th>
                <th data-options="field:'Type',align:'center'" style="width: 100px">申请类型</th>
                <th data-options="field:'TotalPrice',align:'center'" style="width: 100px">代付金额</th>
                <th data-options="field:'CurrencyDec',align:'center'" style="width: 100px">币种</th>
                <th data-options="field:'ApplicationStatusDec',sortable :true,align:'center'" style="width: 100px">审批状态</th>
                <th data-options="field:'ReceiveStatusDec',sortable :true,align:'center'" style="width: 100px">收款状态</th>
                <th data-options="field:'PaymentStatusDec',sortable :true,align:'center'" style="width: 100px">付款状态</th>
                <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 300px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>

