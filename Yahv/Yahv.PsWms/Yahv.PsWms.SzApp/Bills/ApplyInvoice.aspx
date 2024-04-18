<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ApplyInvoice.aspx.cs" Inherits="Yahv.PsWms.SzApp.Bills.ApplyInvoice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //表格初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: false,
                nowrap: false,
            });
            //开票类型
            $('#InvoiceType').combobox({
                data: model.InvoiceTypeOption,
                editable: false,
                valueField: 'value',
                textField: 'text',
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            });
            //开票状态
            $('#InvoiceStatus').combobox({
                data: model.InvoiceStatusOption,
                editable: false,
                valueField: 'value',
                textField: 'text',
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            });
            //交付方式
            $('#DeliveryType').combobox({
                data: model.InvoiceDeliveryOption,
                editable: false,
                valueField: 'value',
                textField: 'text',
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            });
            //申请人选项
            $('#Apply').combobox({
                data: model.ApplyOption,
                editable: true,
                valueField: 'value',
                textField: 'text',
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
                $('#Title').textbox("setText", "");
                $('#InvoiceType').combobox("setValue", "");
                $('#InvoiceStatus').combobox("setValue", "");
                $('#DeliveryType').combobox("setValue", "");
                $('#Apply').combobox("setValue", "");
                $('#StartDate').datebox("setText", "");
                $('#EndDate').datebox("setText", "");
                $('#InvoiceStartDate').datebox("setText", "");
                $('#InvoiceEndDate').datebox("setText", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
            //申请开票 
            $('#btnAddBill').click(function () {
                Add();
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                Title: $.trim($('#Title').textbox("getText")),
                InvoiceStatus: $.trim($('#InvoiceStatus').combobox("getValue")),
                InvoiceType: $.trim($('#InvoiceType').combobox("getValue")),
                DeliveryType: $.trim($('#DeliveryType').combobox("getValue")),
                AdminID: $.trim($('#Apply').combobox("getValue")),

                InvoiceStartDate: $.trim($('#InvoiceStartDate').datebox("getText")),
                InvoiceEndDate: $.trim($('#InvoiceEndDate').datebox("getText")),
                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
            };
            return params;
        };
        //操作
        function Operation(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="Details(' + index + ');return false;">详情</a> '
                , '</span>'].join('');
        }
        //新增开票
        function Add() {
            $.myWindow({
                title: '新增开票',
                minWidth: 1000,
                minHeight: 600,
                url: 'BillList.aspx',
                onClose: function () {
                    grid.myDatagrid('search', getQuery());
                },
            });
            return false;
        }
        //开票详情
        function Details(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: '开票详情',
                minWidth: 1200,
                minHeight: 600,
                url: 'Detail.aspx?ID=' + data.ID,
                onClose: function () {
                },
            });
            return false;
        }
    </script>
    <style>
        .lbl {
            width: 200px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">发票抬头：</td>
                <td class="lbl">
                    <input class="easyui-textbox" id="Title" data-options="width:150,validType:'length[1,50]'" />
                </td>
                <td style="width: 90px;">申请人：</td>
                <td class="lbl">
                    <input class="easyui-combobox" id="Apply" data-options="width:150,valueField:'Value',textField:'Text'" />
                </td>
                <td style="width: 90px;">申请日期：</td>
                <td colspan="5">
                    <input id="StartDate" data-options="prompt:'',width:150," class="easyui-datebox" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="EndDate" data-options="prompt:'',width:150," class="easyui-datebox" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">开票状态：</td>
                <td class="lbl">
                    <input class="easyui-combobox" id="InvoiceStatus" data-options="width:150,valueField:'Value',textField:'Text'" />
                </td>
                <td style="width: 90px;">交付方式：</td>
                <td class="lbl">
                    <input class="easyui-combobox" id="DeliveryType" data-options="width:150,valueField:'Value',textField:'Text'" />
                </td>
                <td style="width: 90px;">开票类型：</td>
                <td class="lbl">
                    <input class="easyui-combobox" id="InvoiceType" data-options="width:150,valueField:'Value',textField:'Text'" />
                </td>
                <td style="width: 90px;">开票日期：</td>
                <td colspan="3">
                    <input id="InvoiceStartDate" data-options="prompt:'',width:150," class="easyui-datebox" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="InvoiceEndDate" data-options="prompt:'',width:150," class="easyui-datebox" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <a id="btnAddBill" class="easyui-linkbutton" iconcls="icon-yg-add">新增开票</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="开票申请">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center'" style="width: 150px;">申请日期</th>
                <th data-options="field:'ID',align:'center'" style="width: 150px;">申请编号</th>
                <th data-options="field:'Title',align:'left'" style="width: 200px;">发票抬头</th>
                <th data-options="field:'InvoiceTypeDec',align:'center'" style="width: 100px;">开票类型</th>
                <th data-options="field:'Amount',align:'center'" style="width: 100px;">开票金额</th>
                <th data-options="field:'InvoiceDeliveryTypeDec',align:'center'" style="width: 100px;">交付方式</th>
                <th data-options="field:'InvoiceNoticeStatusDec',align:'center'" style="width: 100px;">开票状态</th>
                <th data-options="field:'InvoiceDate',align:'center'" style="width: 150px;">开票日期</th>
                <th data-options="field:'InvoiceNos',align:'left'" style="width: 200px;">发票号</th>
                <th data-options="field:'AdminName',align:'center'" style="width: 100px;">申请人</th>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 120px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
