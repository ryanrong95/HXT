<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.PvOms.WebApp.LsOrders.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var firstLoad = true;
        var ID = getQueryString("ID");

        $(function () {
            //初始化订单数据
            if (!jQuery.isEmptyObject(model.LsOrder)) {
                //$('#Company').textbox('setText', model.Company);
                //$('#Beneficary').textbox('setText', model.Beneficary);
                $('#OrderID').textbox('setText', model.LsOrder.OrderID);
                $('#ClientName').textbox('setText', model.LsOrder.ClientName);
                $('#Currency').textbox('setText', model.LsOrder.Currency);
            };
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                fitColumns: true,
                fit: false,
                singleSelect: true,
                pagination: false,
                onClickRow: onClickRow,
                columns: [[
                    { field: 'Name', title: '库位名称', width: 100, align: 'center' },
                    { field: 'SpecID', title: '库位级别', width: 100, align: 'center' },
                    { field: 'StartDate', title: '开始日期', width: 100, align: 'center' },
                    { field: 'EndDate', title: '结束日期', width: 100, align: 'center' },
                    { field: 'Month', title: '租赁期限', width: 100, align: 'center' },
                    { field: 'Quantity', title: '数量', width: 100, align: 'center' },
                    {
                        field: 'UnitPrice', title: '单价(元/月)', width: 100, align: 'center',
                        formatter: formatUnitPrice,
                        editor: {
                            type: 'numberbox', options: {
                                required: true,
                                min: 1,
                                precision: 0,
                            }
                        },
                    },
                    { field: 'TotalPrice', title: '总价', width: 100, align: 'center' },
                ]],
                onLoadSuccess: function (data) {
                    if (firstLoad) {
                        AddTotalRow();
                        firstLoad = false;
                    }
                }
            });

            //提交
            $("#btnSubmit").click(function () {
                endEditing();
                var data = new FormData();
                
                data.append('ID', ID);
                var rows = $('#tab1').datagrid('getRows');
                var IDPrices = [];
                for (var i = 0; i < rows.length - 1; i++) {
                    IDPrices.push(rows[i]);
                }
                data.append('IDPrices', JSON.stringify(IDPrices));
                
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
            });
            //取消
            $("#btnClose").click(function () {
                $.myWindow.close();
            });
        })
    </script>
    <script>
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#tab1').datagrid('validateRow', editIndex)) {
                $('#tab1').datagrid('endEdit', editIndex);
                
                var row = $("#tab1").myDatagrid('getRows')[editIndex];
                row.TotalPrice = row.UnitPrice * row.Quantity * row.Month;

                loadData();
                RemoveTotalRow();
                AddTotalRow();

                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow(index) {
            var lastIndex = $('#tab1').datagrid('getRows').length - 1;
            if (index == lastIndex) {
                endEditing()
                return;
            }
            if (endEditing()) {
                $('#tab1').datagrid('selectRow', index).datagrid('beginEdit', index);
                editIndex = index;
            } else {
                $('#tab1').datagrid('selectRow', editIndex);
            }
        }
        //添加合计行
        function AddTotalRow() {
            $('#tab1').datagrid('appendRow', {
                Name: '<span class="subtotal">合计：</span>',
                SpecID: '<span class="subtotal">--</span>',
                StartDate: '<span class="subtotal">--</span>',
                EndDate: '<span class="subtotal">--</span>',
                Month: '<span class="subtotal">--</span>',
                Quantity: '<span class="subtotal">--</span>',
                UnitPrice: '--',
                TotalPrice: '<span class="subtotal">' + compute('TotalPrice') + '</span>',
            });
        }
        //删除合计行
        function RemoveTotalRow() {
            var lastIndex = $('#tab1').datagrid('getRows').length - 1;
            $('#tab1').datagrid('deleteRow', lastIndex);
        }
        //重新加载数据
        function loadData() {
            var data = $('#tab1').datagrid('getData');
            $('#tab1').datagrid('loadData', data);
        }
        //计算合计值
        function compute(colName) {
            var rows = $('#tab1').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length; i++) {
                if (rows[i][colName] != undefined) {
                    total += parseFloat(Number(rows[i][colName]));
                }
            }
            return total.toFixed(2);
        }
        function formatUnitPrice(val, row) {
            return '<span style="color:red;font-weight:bold">' + val + '</span>';
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%">
        <div data-options="region:'center'">
            <div id="topper">
                <table class="liebiao">
                   <%-- <tr>
                        <td style="width: 90px;">内部公司:</td>
                        <td>
                            <input id="Company" class="easyui-textbox" style="width: 200px" />
                        </td>
                        <td style="width: 90px;">受益账号:</td>
                        <td colspan="5">
                            <input id="Beneficary" class="easyui-textbox" style="width: 200px" />
                        </td>
                    </tr>--%>
                    <tr>
                        <td style="width: 90px;">订单编号</td>
                        <td>
                            <input id="OrderID" class="easyui-textbox" style="width: 200px" />
                        </td>
                        <td style="width: 90px;">客户名称</td>
                        <td>
                            <input id="ClientName" class="easyui-textbox" style="width: 200px" />
                        </td>
                        <td style="width: 90px;">订单币种:</td>
                        <td>
                            <input id="Currency" class="easyui-textbox" style="width: 200px" />
                        </td>
                    </tr>
                </table>
            </div>
            <table id="tab1">
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">保存</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
</asp:Content>
