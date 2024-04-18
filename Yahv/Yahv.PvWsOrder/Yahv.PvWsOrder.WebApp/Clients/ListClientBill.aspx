<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListClientBill.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Orders.Clients.ListClientBill" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                singleSelect: false,
                fitColumns: true,
                nowrap: false,
                checkOnSelect: false,
                onLoadSuccess: function (data) {//加载完毕后获取所有的checkbox遍历 
                    $(".datagrid-header-check").html("");
                    if (data.rows.length > 0) {
                        //循环判断操作为新增的不能选择 
                        for (var i = 0; i < data.rows.length; i++) {
                            //根据operate让某些行不可选 
                            if (data.rows[i].IsInvoice == "已开票" || data.rows[i].Currency == "港币") {
                                $("input[type='checkbox']")[i].disabled = true;
                            }
                        }
                    }
                    
                },
                onClickRow: function (rowIndex, rowData) {
                    //加载完毕后获取所有的checkbox遍历 
                    $("input[type='checkbox']").each(function (index, el) {
                        //如果当前的复选框不可选，则不让其选中 
                        if (el.disabled == true) {
                            $("#dg").datagrid('unselectRow', index - 1);
                        }
                    })
                }
            });
            //客户名称
            $('#client').combobox({
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                data: model.ClientData,
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            });
            //币种
            $('#currency').combobox({
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                data: model.currencyData,
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            });
            //是否开票
            $('#isInvoice').combobox({
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                data: model.isInvoiceData,
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            });
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $('#client').textbox("setText", "");
                $('#clientCode').textbox("setText", "");
                $('#currency').combobox("setValue", "");
                $('#isInvoice').combobox("setValue", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
            //新增账单
            $('#btnAddBill').click(function () {
                Add();
            });
            //申请开票 
            $('#btnApplyInvoice').click(function () {
                ApplyInvoice();
            });
        })
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                ClientName: $.trim($('#client').textbox("getText")),
                ClientCode: $.trim($('#clientCode').textbox("getText")),
                Currency: $.trim($('#currency').combobox("getValue")),
                IsInvoiced: $.trim($('#isInvoice').combobox("getValue")),
            };
            return params;
        };
        //下单操作
        function Operation(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="Detail(' + index + ');return false;">详情</a> '
                , '</span>'].join('');
        }
        //新增账单
        function Add() {
            $.myWindow({
                title: '新增账单',
                minWidth: 1000,
                minHeight: 600,
                url: 'BillAdd.aspx',
                onClose: function () {
                    grid.myDatagrid('search', getQuery());
                },
            });
            return false;
        }
        //申请开票
        function ApplyInvoice() {
            var rows = $("#tab1").datagrid("getChecked");
            if (rows.length == 0) {
                top.$.timeouts.alert({ position: "TC", msg: '请勾选订单', type: "error" });
                return;
            }
            else if (rows.length > 1) {
                var entercode = rows[0].EnterCode;
                var clientId = rows[0].ClientID;
                var ids = [];
                for (var i = 0; i < rows.length; i++) {
                    if (rows[i].EnterCode != entercode || rows[i].Currency != "人民币") {
                        top.$.timeouts.alert({ position: "TC", msg: '合开发票需保证同一客户，RMB账单', type: "error" });
                        return;
                    }
                    // id1，id2,id3
                    ids.push(rows[i].ID);
                }
                //开票申请窗口
                $.myWindow({
                    title: '开票申请',
                    minWidth: 1200,
                    minHeight: 600,
                    url: 'ApplyInvoice.aspx?IDs=' + ids.join(',') + '&clientId=' + clientId,
                    onClose: function () {
                        grid.myDatagrid('search', getQuery());
                    },
                });
                return false;
            }
            else {
                var clientId = rows[0].ClientID;
                var ids = [];
                ids.push(rows[0].ID);
                //开票申请窗口
                $.myWindow({
                    title: '开票申请',
                    minWidth: 1200,
                    minHeight: 600,
                    url: 'ApplyInvoice.aspx?IDs=' + ids.join(',') + '&clientId=' + clientId,
                    onClose: function () {
                        grid.myDatagrid('search', getQuery());
                    },
                });
            }
        }
        //账单详情
        function Detail(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: '账单详情',
                minWidth: 1200,
                minHeight: 600,
                url: 'BillDetails.aspx?ID=' + data.ID,
                onClose: function () {
                },
            });
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">客户名称</td>
                <td style="width: 200px;">
                    <input id="client" class="easyui-combobox" style="width: 150px" />
                </td>
                <td style="width: 90px;">客户入仓号</td>
                <td style="width: 200px;">
                    <input id="clientCode" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">币种</td>
                <td style="width: 200px;">
                    <input id="currency" class="easyui-combobox" style="width: 150px" />
                </td>
                <td style="width: 90px;">开票状态</td>
                <td>
                    <input id="isInvoice" class="easyui-combobox" style="width: 150px" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <a id="btnAddBill" class="easyui-linkbutton" iconcls="icon-yg-add">新增账单</a>
                    <a id="btnApplyInvoice" class="easyui-linkbutton" iconcls="icon-man">申请开票</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="客户账单">
        <thead>
            <tr>
                <th data-options="field:'ck', checkbox:true"></th>
                <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">创建日期</th>
                <th data-options="field:'ClientName',align:'left'" style="width: 300px">客户名称</th>
                <th data-options="field:'EnterCode',align:'center'" style="width: 100px">客户入仓号</th>
                <th data-options="field:'Currency',align:'center'" style="width: 100px">币种</th>
                <th data-options="field:'Price',align:'center'" style="width: 100px">总金额</th>
                <th data-options="field:'IsInvoice',align:'center'" style="width: 100px">开票状态</th>
                <th data-options="field:'Creater',align:'center'" style="width: 100px">创建人</th>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 120px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
