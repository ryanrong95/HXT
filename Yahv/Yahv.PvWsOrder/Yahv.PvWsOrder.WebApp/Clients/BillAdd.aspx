<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="BillAdd.aspx.cs" Inherits="Yahv.PvOms.WebApp.Clients.BillAdd" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var ClientID = getQueryString("ClientID");
        var BillDate = getQueryString("Date");
        $(function () {
            //订单列表初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                singleSelect: false,
                pagination: false,
                rownumbers: true,
                fitColumns: true,
                fit: true,
                onLoadSuccess: function (data) {
                }
            });
            //客户名称
            $('#client').combobox({
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                data: model.ClientData,
            });
            //账单币种
            $('#currency').combobox({
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                data: model.Currency,
            });
            $('#currency').combobox("setValue", 1)
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $('#client').textbox("setText", "");
                $('#clientCode').textbox("setText", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 新增账单
            $('#btnAdd').click(function () {
                //验证必填项
                var isValid = $('#form1').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                //验证勾选项
                var rows = $("#tab1").datagrid("getChecked");
                if (rows.length == 0) {
                    top.$.timeouts.alert({ position: "TC", msg: '请选择相同客户的一个或多个订单', type: "error" });
                    return false;
                }
                var orders = [];
                var entercode = "";
                for (var i = 0; i < rows.length; i++) {
                    if (entercode == "") {
                        entercode = rows[i].ClientCode;
                        orders.push(rows[i].ID);
                    }
                    else {
                        if (entercode == rows[i].ClientCode) {
                            orders.push(rows[i].ID);
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: '请选择相同客户的一个或多个订单', type: "error" });
                            return false;
                        }
                    }
                }
                var data = new FormData();
                data.append('orders', orders);
                data.append('currency', $('#currency').combobox("getValue"));
                data.append('clientID', rows[0].ClientID);

                ajaxLoading();
                $.ajax({
                    url: '?action=Submit',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        var res = eval(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                            grid.myDatagrid('search', getQuery());
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                    }
                })
            })
        })
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                ClientName: $.trim($('#client').textbox("getText")),
                ClientCode: $.trim($('#clientCode').textbox("getText")),
            };
            return params;
        };
        //操作
        function Operation(val, row, index) {
            var buttons = [];
            buttons.push('<span class="easyui-formatted">');
            buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="Details(\'' + row.ID + '\');return false;">应收详情</a> ');
            buttons.push('</span>')
            return buttons.join('');
        }
        //应收
        function Details(id) {
            $.myWindow({
                title: '订单账单详情',
                minWidth: 1200,
                minHeight: 600,
                url: '../Orders/Common/BillNew.aspx?ID=' + id,
                onClose: function () {
                },
            });
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%">
        <div data-options="region:'center'" style="border: none">
            <div id="topper">
                <!--搜索按钮-->
                <table class="liebiao">
                    <tr>
                        <td style="width: 90px;">客户名称</td>
                        <td>
                            <input id="client" class="easyui-combobox" data-options="prompt:'客户名称'" style="width: 200px" />
                            <input id="clientCode" class="easyui-textbox" data-options="prompt:'客户入仓号'" style="width: 200px" />
                            <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                            <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 90px; color: orangered">账单币种</td>
                        <td>
                            <input id="currency" class="easyui-combobox" style="width: 200px" data-options="prompt:'账单币种',required:true" />
                            <a id="btnAdd" class="easyui-linkbutton" iconcls="icon-yg-add">新增账单</a>
                        </td>
                    </tr>
                </table>
            </div>
            <table id="tab1">
                <thead>
                    <tr>
                        <th data-options="field:'ck',checkbox:true" style="width: 50px;"></th>
                        <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">创建日期</th>
                        <th data-options="field:'ID',align:'center'" style="width: 150px;">订单编号</th>
                        <th data-options="field:'ClientName',align:'left'" style="width: 200px">客户名称</th>
                        <th data-options="field:'ClientCode',align:'center'" style="width: 100px">客户入仓号</th>
                        <th data-options="field:'Type',align:'center'" style="width: 100px;">订单类型</th>
                        <th data-options="field:'MainStatus',align:'center'" style="width: 100px;">订单状态</th>
                        <th data-options="field:'PaymentStatus',align:'center'" style="width: 100px;">支付状态</th>
                        <th data-options="field:'Currency',align:'center'" style="width: 100px;">应收币种</th>
                        <th data-options="field:'Price',align:'center'" style="width: 100px;">应收金额</th>
                        <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 120px;">操作</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
</asp:Content>
