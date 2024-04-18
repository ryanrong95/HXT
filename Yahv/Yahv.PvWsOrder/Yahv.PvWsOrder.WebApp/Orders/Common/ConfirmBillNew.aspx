<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ConfirmBillNew.aspx.cs" Inherits="Yahv.PvOms.WebApp.Orders.Common.ConfirmBillNew" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var id = getQueryString("ID");
        var firstLoad = true;

        $(function () {
            //应收账款初始化
            $('#tab1').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                rownumbers: true,
                onClickRow: onClickRow,
                onLoadSuccess: onLoadSuccess,
                columns: [[
                    { field: 'OriginDate', title: '创建日期', width: 80, align: 'center' },
                    { field: 'Catalog', title: '分类', width: 80, align: 'center' },
                    { field: 'Subject', title: '科目', width: 80, align: 'center' },
                    { field: 'OriginCurrency', title: '原始币种', width: 80, align: 'center' },
                    { field: 'OriginPrice', title: '原始成本', width: 80, align: 'center' },
                    { field: 'Rate', title: '汇率', width: 80, align: 'center' },
                    { field: 'Currency', title: '币种', width: 80, align: 'center' },
                    { field: 'LeftPrice', title: '成本', width: 80, align: 'center' },
                    {
                        field: 'ConfirmPrice', title: '确认收款金额', width: 120, align: 'center',
                        formatter: function (value) {
                            return '<span style="color:red;">' + value + '</span>';
                        },
                        editor: { type: 'numberbox', options: { min: 0, precision: 4, } }
                    },
                    { field: 'RightPrice', title: '已收金额', width: 80, align: 'center' },
                ]]
            });
            ////查看客户仓储费
            //$('#btn').click(function () {
            //    $.myWindow({
            //        title: "客户仓储费",
            //        url: location.pathname.replace('ConfirmBillNew.aspx', 'UnReceivedStorageCharges.aspx'),
            //    });
            //});
            //新增费用
            $('#btnAdd').click(function () {
                $.myWindow({
                    title: "新增费用",
                    url: 'AddFeeNew.aspx?ID=' + id,
                    width: 600,
                    height: 400,
                    onClose: function () {
                        location.reload();
                    },
                });
                return false;
            });
            //关闭窗口
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
            //提交
            $("#btnSubmit").click(function () {
                var data = new FormData();
                var rows = $('#tab1').datagrid('getRows');
                var receivables = [];
                for (var i = 0; i < rows.length - 1; i++) {
                    receivables.push(rows[i]);
                }
                data.append('receivables', JSON.stringify(receivables));
                data.append('orderId', id);

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
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                        $.myWindow.close();
                    }
                })
            })
        });
    </script>
    <script>
        //修改本位币应收
        function Operation(val, row, index) {
            if (row.OriginDate == '<span class="subtotal">合计：</span>') {
                return;
            }
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-edit\'" onclick="Edit(\'' + row.ID + '\');return false;">修改应收</a> '
                , '</span>'].join('');
        }
        //加载tabl数据
        function onLoadSuccess(data) {
            if (firstLoad) {
                if (data.rows.length > 0) {
                    $('#payee').textbox('setValue', data.rows[0].PayeeName);
                    $('#payer').textbox('setValue', data.rows[0].PayerName);
                }
                AddTotalRow();
                firstLoad = false;
            }
        }
    </script>
    <script>
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#tab1').datagrid('validateRow', editIndex)) {
                $('#tab1').datagrid('endEdit', editIndex);

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
        //重新加载数据
        function loadData() {
            var data = $('#tab1').datagrid('getData');
            $('#tab1').datagrid('loadData', data);
        }
        //添加合计行
        function AddTotalRow() {
            $('#tab1').datagrid('appendRow', {
                OriginDate: '<span class="subtotal">合计：</span>',
                Catalog: '<span class="subtotal">--</span>',
                Subject: '<span class="subtotal">--</span>',
                OriginCurrency: '<span class="subtotal">--</span>',
                OriginPrice: '<span class="subtotal">--</span>',
                Rate: '<span class="subtotal">--</span>',
                Currency: '<span class="subtotal">--</span>',
                LeftPrice: '<span class="subtotal">' + compute('LeftPrice') + '</span>',
                ConfirmPrice: '<span class="subtotal">' + compute('ConfirmPrice') + '</span>',
                Btn: '<span class="subtotal">--</span>',
                RightPrice: '<span class="subtotal">--</span>',
            });
        }
        //删除合计行
        function RemoveTotalRow() {
            var lastIndex = $('#tab1').datagrid('getRows').length - 1;
            $('#tab1').datagrid('deleteRow', lastIndex);
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
            return total.toFixed(4);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'north'" style="height: 60px; border: none">
            <table class="liebiao">
                <tr>
                    <td style="width: 100px">收款公司</td>
                    <td>
                        <input id="payee" class="easyui-textbox" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">付款公司</td>
                    <td>
                        <input id="payer" class="easyui-textbox" style="width: 250px;" />
                        <%--<a id="btn" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">查看仓储费</a>--%>
                        <a id="btnAdd" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增费用</a>
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'center'" style="border: none">
            <table id="tab1" title="客户应收账款">
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5;">
            <div style="float: right; margin-right: 2px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">确认</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">取消</a>
            </div>
        </div>
    </div>
</asp:Content>
