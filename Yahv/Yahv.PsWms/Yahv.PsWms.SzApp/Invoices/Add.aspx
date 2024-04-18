<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.PsWms.SzApp.Invoices.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                fitColumns: false,
                onClickRow: onClickRow,
                columns: [[
                    { field: 'PayerName', title: '客户名称', width: 150, align: 'center'},
                    { field: 'CutDateIndex', title: '账单期号', width: 100, align: 'center' },
                    { field: 'Total', title: '账单金额', width: 100, align: 'center' },
                    { field: 'ReceiptTotal', title: '实收金额', width: 100, align: 'center' },
                    { field: 'TaxTotal', title: '含税金额', width: 100, align: 'center' },
                    {
                        field: 'Difference', title: '开票差额', width: 100, align: 'center',
                        editor: { type: 'numberbox', options: { precision: 2 } }
                    },
                    { field: 'Btn', title: '操作', width: 120, align: 'center', formatter: Operation }
                ]],
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        if (row["ReceiptTotal"] == null) {
                            row["ReceiptTotal"] = Number("0").toFixed(2);
                        }
                        else {
                            row["ReceiptTotal"] = Number(row["ReceiptTotal"]).toFixed(2);
                        }
                    }
                    return data;
                },
            });
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery()); 
                endEditing();
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $('#clientName').textbox("setText", "");
                $('#dateIndex').numberbox("setText", "");
                grid.myDatagrid('search', getQuery());
                endEditing();
                return false;
            });
        })
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                Name: $.trim($('#clientName').textbox("getText")),
                DateIndex: $.trim($('#dateIndex').numberbox("getText")),
            };
            return params;
        };
        function Operation(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-man\'" onclick="Apply(' + index + ');return false;">申请开票</a> '
                , '</span>'].join('');
        }
        function Apply(index) {
            endEditing();
            var row = $("#tab1").myDatagrid('getRows')[index];
            var data = new FormData();
            var vouchers = [];
            vouchers.push(row);
            data.append('vouchers', JSON.stringify(vouchers));
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
                        $.myWindow.close();
                    }
                    else {
                        top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                    }
                }
            })
        };
    </script>
    <script>
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#tab1').datagrid('validateRow', editIndex)) {
                $('#tab1').datagrid('endEdit', editIndex);

                loadData();

                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow(index) {
            if (editIndex != index) {
                if (endEditing()) {
                    $('#tab1').datagrid('selectRow', index).datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#tab1').datagrid('selectRow', editIndex);
                }
            }
        }
        //重新加载数据，作用：刷新列表操作按钮的样式，并触发onLoadSuccess事件
        function loadData() {
            var data = $('#tab1').datagrid('getData');
            $('#tab1').datagrid('loadData', data);
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
                    <input id="clientName" data-options="prompt:''," class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">账单期号</td>
                <td>
                    <input id="dateIndex" class="easyui-numberbox" style="width: 150px" />
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <p style="font-size: 14px;color:orangered">注：申请开票前跟单员需在列表中填写好开票差额。</p>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1">
        <thead>
            <tr>
                <th data-options="field:'PayerName',align:'left'" style="width: 200px">客户名称</th>
                <th data-options="field:'CutDateIndex',align:'center'" style="width: 100px">账单期号</th>
                <th data-options="field:'Total',align:'center'" style="width: 100px">账单金额</th>
                <th data-options="field:'ReceiptTotal',align:'center'" style="width: 100px">实收金额</th>
                <th data-options="field:'TaxTotal',align:'center'" style="width: 100px">含税金额</th>
                <th data-options="field:'Difference',align:'center'" style="width: 100px">开票差额</th>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 120px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
